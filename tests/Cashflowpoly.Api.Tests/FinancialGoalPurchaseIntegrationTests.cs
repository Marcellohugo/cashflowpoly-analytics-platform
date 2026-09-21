// Fungsi file: Memastikan kartu tujuan dimiliki pembeli pertama yang mampu membayar, tanpa reservasi saat menabung.
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Tests.Infrastructure;
using Dapper;
using Npgsql;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
public sealed class FinancialGoalPurchaseIntegrationTests(ApiIntegrationTestFixture fixture)
{
    [Fact]
    public async Task SavingDoesNotReserveCardsAndOwnSavingsCanBuyAnotherAvailableGoal()
    {
        var session = await CreateSession(10,
            new RulesetFinancialGoalDto { Id = "rumah", Nama = "Rumah", HargaBeli = 10, PoinKebahagiaan = 5 },
            new RulesetFinancialGoalDto { Id = "mobil", Nama = "Mobil", HargaBeli = 25, PoinKebahagiaan = 7 },
            new RulesetFinancialGoalDto { Id = "sekolah", Nama = "Sekolah", HargaBeli = 20, PoinKebahagiaan = 6 });
        var first = session.Players[0].UserId!.Value;
        var second = session.Players[1].UserId!.Value;
        var third = session.Players[2].UserId!.Value;
        var sequence = session.Sequence;

        await AssertEvent(first, "Menabung", new { goal_id = "rumah", amount = 10 }, slot: 1);
        var saved = await ReadState(session);
        Assert.All(saved, player => Assert.Empty(player.TujuanFinansial));
        Assert.Equal(20, saved.Single(player => player.UserId == first).Saving);
        Assert.Equal(session.Players[0].Coins - 10, saved.Single(player => player.UserId == first).Coins);
        await AssertGoalRowCount(0);

        await AssertEvent(second, "TujuanFinansial", new { goal_id = "rumah", cost = 10, points = 5 });
        var beforeRejection = Balances(await ReadState(session));
        await AssertEvent(first, "TujuanFinansial", new { goal_id = "rumah", cost = 10, points = 5 },
            expected: HttpStatusCode.UnprocessableEntity);
        await AssertEvent(second, "TujuanFinansial", new { goal_id = "rumah", cost = 10, points = 5 },
            expected: HttpStatusCode.UnprocessableEntity);
        Assert.Equal(beforeRejection, Balances(await ReadState(session)));

        await AssertEvent(first, "Menabung", new { amount = 5 }, slot: 2);
        var beforeUnaffordable = Balances(await ReadState(session));
        await AssertEvent(third, "TujuanFinansial", new { goal_id = "sekolah", cost = 20, points = 6 },
            expected: HttpStatusCode.UnprocessableEntity);
        Assert.Equal(beforeUnaffordable, Balances(await ReadState(session)));
        await AssertEvent(first, "TujuanFinansial", new { goal_id = "mobil", cost = 25, points = 7 });

        var final = await ReadState(session);
        Assert.Equal(0, final.Single(player => player.UserId == first).Saving);
        Assert.Equal(0, final.Single(player => player.UserId == second).Saving);
        Assert.Equal(10, final.Single(player => player.UserId == third).Saving);
        Assert.Equal(session.Players[0].Coins - 15, final.Single(player => player.UserId == first).Coins);
        Assert.Equal(session.Players[0].Happiness + 7, final.Single(player => player.UserId == first).Happiness);
        Assert.Equal(session.Players[1].Happiness + 5, final.Single(player => player.UserId == second).Happiness);
        Assert.Equal("Mobil", Assert.Single(final.Single(player => player.UserId == first).TujuanFinansial).Nama);
        Assert.Equal("Rumah", Assert.Single(final.Single(player => player.UserId == second).TujuanFinansial).Nama);
        Assert.Empty(final.Single(player => player.UserId == third).TujuanFinansial);
        Assert.All(final.SelectMany(player => player.TujuanFinansial), goal => Assert.Equal("COMPLETED", goal.Status));
        await AssertGoalRowCount(2);
        foreach (var player in final)
        {
            using var response = await Send(session.Token, HttpMethod.Get,
                $"/api/v1/analytics/sessions/{session.Id}/players/{player.UserId}/gameplay");
            response.EnsureSuccessStatusCode();
            var metrics = (await response.Content.ReadFromJsonAsync<GameplayMetricsResponse>())!;
            Assert.Equal(player.Saving, metrics.RawJson!.Value.GetProperty("coins").GetProperty("coins_saved").GetInt32());
        }

        async Task AssertEvent(Guid player, string action, object payload, int slot = 0,
            HttpStatusCode expected = HttpStatusCode.Created)
        {
            using var response = await PostEvent(session, player, sequence, action, payload, slot);
            Assert.True(response.StatusCode == expected, $"{action}: {await response.Content.ReadAsStringAsync()}");
            if (response.IsSuccessStatusCode) sequence++;
        }

        async Task AssertGoalRowCount(int count)
        {
            await using var db = new NpgsqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__Default"));
            Assert.Equal(count, await db.ExecuteScalarAsync<int>(
                "select count(*) from session_participant_financial_goals where session_id=@Id", new { session.Id }));
        }
    }

    [Fact]
    public async Task ExtraPhysicalCopyCanBeBoughtByAnotherPlayerButNotBoughtTwiceByTheSamePlayer()
    {
        var session = await CreateSession(20,
            new RulesetFinancialGoalDto { Id = "rumah", Nama = "Rumah", HargaBeli = 10, PoinKebahagiaan = 5, CardQty = 2 });
        var payload = new { goal_id = "rumah", cost = 10, points = 5 };
        var sequence = session.Sequence;
        foreach (var (playerIndex, expected) in new[]
        {
            (0, HttpStatusCode.Created), (0, HttpStatusCode.UnprocessableEntity), (1, HttpStatusCode.Created)
        })
        {
            using var response = await PostEvent(session, session.Players[playerIndex].UserId!.Value,
                sequence, "TujuanFinansial", payload);
            Assert.True(response.StatusCode == expected, await response.Content.ReadAsStringAsync());
            if (response.IsSuccessStatusCode) sequence++;
        }
        var state = await ReadState(session);
        Assert.All(state.Take(2), player =>
        {
            Assert.Equal(10, player.Saving);
            Assert.Single(player.TujuanFinansial);
        });
        Assert.Equal(20, state[2].Saving);
        Assert.Empty(state[2].TujuanFinansial);
    }

    [Fact]
    public async Task ConcurrentPurchasesOfLastCardChargeExactlyOnePlayer()
    {
        var session = await CreateSession(10,
            new RulesetFinancialGoalDto { Id = "rumah", Nama = "Rumah", HargaBeli = 10, PoinKebahagiaan = 5, CardQty = 1 });
        var ready = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var arrived = 0;
        var outcomes = await Task.WhenAll(AttemptPurchase(0), AttemptPurchase(1));
        Assert.Single(outcomes, outcome => outcome);
        var state = await ReadState(session);
        Assert.Equal(20, state.Sum(player => player.Saving));
        Assert.Single(state.SelectMany(player => player.TujuanFinansial));
        for (var index = 0; index < 2; index++)
        {
            Assert.Equal(outcomes[index] ? 0 : 10, state[index].Saving);
            Assert.Equal(outcomes[index] ? 1 : 0, state[index].TujuanFinansial.Count);
        }
        Assert.All(state, player => Assert.Equal(
            session.Players.Single(initial => initial.UserId == player.UserId).Coins, player.Coins));

        async Task<bool> AttemptPurchase(int playerIndex)
        {
            await using var db = new NpgsqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__Default"));
            await db.OpenAsync();
            await using var transaction = await db.BeginTransactionAsync();
            if (Interlocked.Increment(ref arrived) == 2) ready.SetResult();
            await ready.Task;
            try
            {
                var player = session.Players[playerIndex];
                var eventPk = await db.QuerySingleAsync<Guid>("""
                    insert into events (event_id, session_id, session_player_id, user_id, actor_type,
                        timestamp, day_index, weekday, turn_number, action_slot, sequence_number,
                        ruleset_action_id, action_type, ruleset_version_id, payload)
                    select @EventId, @Id, @SessionPlayerId, @UserId, 'SYSTEM',
                        now(), 1, 'MON', 0, 0, @Sequence,
                        ruleset_action_id, 'TujuanFinansial', @RulesetId,
                        '{"goal_id":"rumah","cost":10,"points":5}'::jsonb
                    from ruleset_actions where ruleset_version_id=@RulesetId and action_id='TujuanFinansial'
                    returning event_pk
                    """, new
                {
                    EventId = Guid.NewGuid(), session.Id, session.RulesetId, player.SessionPlayerId,
                    player.UserId, Sequence = session.Sequence + playerIndex
                }, transaction);
                await db.ExecuteAsync("select project_session_event(@eventPk)", new { eventPk }, transaction);
                await transaction.CommitAsync();
                return true;
            }
            catch (PostgresException error)
            {
                await transaction.RollbackAsync();
                Assert.Equal(PostgresErrorCodes.CheckViolation, error.SqlState);
                Assert.Contains("no longer available", error.MessageText);
                return false;
            }
        }
    }

    private async Task<GoalSession> CreateSession(int initialSaving, params RulesetFinancialGoalDto[] goals)
    {
        var suffix = Guid.NewGuid().ToString("N")[..10];
        using var registration = await fixture.Client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterRequest($"goal_buy_{suffix}", "GoalPurchase!2026", "INSTRUCTOR", "Goal instructor"));
        registration.EnsureSuccessStatusCode();
        var account = (await registration.Content.ReadFromJsonAsync<RegisterResponse>())!;
        var json = JsonSerializer.SerializeToNode(EventAnalyticsIntegrationTests.BuildRulesetDefinition(50, mode: "MAHIR"))!;
        json["settings"]!["initial_saving"] = initialSaving;
        var definition = json.Deserialize<RulesetDefinitionDto>()!;
        definition.Actions.Add(new() { ActionId = "Menabung" });
        definition.Actions.Add(new() { ActionId = "TujuanFinansial" });
        definition.FinancialGoals.Clear();
        definition.FinancialGoals.AddRange(goals);
        var setup = await new EventAnalyticsIntegrationTests(fixture)
            .CreateReadySessionAsync(account.AccessToken, suffix, definition);
        using var response = await Send(account.AccessToken, HttpMethod.Get, $"/api/v1/sessions/{setup.SessionId}/state");
        response.EnsureSuccessStatusCode();
        var state = (await response.Content.ReadFromJsonAsync<SessionStateResponse>())!;
        return new(account.AccessToken, setup.SessionId, setup.RulesetVersionId, setup.NextSequenceNumber,
            state.Players.OrderBy(player => player.PlayerIndex).ToArray());
    }

    private Task<HttpResponseMessage> PostEvent(GoalSession session, Guid userId, long sequence,
        string action, object payload, int slot = 0) => Send(session.Token, HttpMethod.Post, "/api/v1/events", new
        {
            event_id = Guid.NewGuid(), session_id = session.Id, user_id = userId,
            actor_type = action == "TujuanFinansial" ? "SYSTEM" : "PLAYER", timestamp = DateTimeOffset.UtcNow,
            day_index = 1, weekday = "MON", turn_number = action == "TujuanFinansial" ? 0 :
                session.Players.Single(player => player.UserId == userId).PlayerIndex,
            action_slot = slot, sequence_number = sequence, action_type = action,
            ruleset_version_id = session.RulesetId, payload
        });

    private async Task<SessionPlayerStateDto[]> ReadState(GoalSession session)
    {
        using var response = await Send(session.Token, HttpMethod.Get, $"/api/v1/sessions/{session.Id}/state");
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SessionStateResponse>())!.Players
            .OrderBy(player => player.PlayerIndex).ToArray();
    }

    private static (Guid?, int, int, int)[] Balances(IEnumerable<SessionPlayerStateDto> state) => state
        .Select(player => (player.UserId, player.Coins, player.Saving, player.Happiness)).ToArray();

    private async Task<HttpResponseMessage> Send(string token, HttpMethod method, string path, object? body = null)
    {
        using var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body is not null) request.Content = JsonContent.Create(body);
        return await fixture.Client.SendAsync(request, TestContext.Current.CancellationToken);
    }

    private sealed record GoalSession(string Token, Guid Id, Guid RulesetId, long Sequence, SessionPlayerStateDto[] Players);
}
