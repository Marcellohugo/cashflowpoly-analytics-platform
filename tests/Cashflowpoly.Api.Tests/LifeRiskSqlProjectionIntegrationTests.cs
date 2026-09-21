// Fungsi file: Memverifikasi proyeksi SQL, penyelesaian per pemain, dan kompatibilitas kartu risiko lama.
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
public sealed class LifeRiskSqlProjectionIntegrationTests(ApiIntegrationTestFixture fixture)
{
    [Theory]
    [InlineData("risk_bencana_banjir", "ALL_PLAYERS_COIN_EFFECT", true)]
    [InlineData("risk_pemadaman_listrik", "ALL_PLAYERS_COIN_EFFECT", true)]
    [InlineData("risk_bencana_banjir", "ALL_PLAYERS_COIN_EFFECT", false)]
    [InlineData("risk_ulang_tahun", "PLAYER_TO_PLAYER_TRANSFER", false)]
    public async Task SqlProjectionPreservesPerPlayerEffectsAndDoesNotRepeatSettledCards(
        string riskCode, string effectType, bool perPlayerResolution)
    {
        var suffix = Guid.NewGuid().ToString("N")[..10];
        using var registration = await fixture.Client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            username = $"sql_risk_{suffix}", password = "SqlRiskRegression!2026", role = "INSTRUCTOR"
        });
        registration.EnsureSuccessStatusCode();
        var account = (await registration.Content.ReadFromJsonAsync<RegisterResponse>())!;
        var birthday = effectType == "PLAYER_TO_PLAYER_TRANSFER";
        var definition = EventAnalyticsIntegrationTests.BuildRulesetDefinition(50, mode: "MAHIR");
        definition.LifeRisks.Add(new RulesetLifeRiskDto
        {
            RiskCode = riskCode, ItemName = riskCode, EffectType = effectType,
            Direction = birthday ? "IN" : "OUT", Amount = birthday ? 1 : 3,
            TargetScope = birthday ? "OTHER_PLAYERS" : "ALL_PLAYERS", DurationDays = 1, CardQty = 1
        });
        var setup = await new EventAnalyticsIntegrationTests(fixture)
            .CreateReadySessionAsync(account.AccessToken, suffix, definition);
        await using var db = new NpgsqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__Default"));
        await db.OpenAsync();
        var players = (await db.QueryAsync<(Guid ParticipantId, Guid UserId, int TurnNumber)>("""
            select session_participant_id, user_id, player_order_no
            from session_participants where session_id=@SessionId order by player_order_no
            """, new { setup.SessionId })).ToArray();
        Assert.Equal(3, players.Length);
        Assert.Equal(setup.ActingUserId, players[0].UserId);
        var sequence = setup.NextSequenceNumber;
        var orderId = Guid.NewGuid();
        using var orderRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/events");
        orderRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", account.AccessToken);
        orderRequest.Content = JsonContent.Create(new
        {
            event_id = orderId, session_id = setup.SessionId, user_id = setup.ActingUserId,
            actor_type = "PLAYER", timestamp = DateTimeOffset.UtcNow, day_index = 1, weekday = "MON",
            turn_number = players[0].TurnNumber, action_slot = 1, sequence_number = sequence,
            action_type = "JualMasakan", ruleset_version_id = setup.RulesetVersionId,
            payload = new { order_card_id = "nasi_goreng" }
        });
        using var orderResponse = await fixture.Client.SendAsync(orderRequest, TestContext.Current.CancellationToken);
        Assert.True(orderResponse.StatusCode == HttpStatusCode.Created, await orderResponse.Content.ReadAsStringAsync());
        sequence++;
        var before = await ReadBalances();
        var riskPayload = new Dictionary<string, object>
        {
            ["risk_id"] = riskCode, ["source_order_event_id"] = orderId
        };
        if (perPlayerResolution) riskPayload["resolution_mode"] = "PER_PLAYER";

        // Raw inserts exercise the active database guard and SQL projector without API enrichment.
        var risk = await Insert("RisikoKehidupan", riskPayload, 0);
        await Project(risk.EventPk);
        var afterDraw = await ReadBalances();
        if (perPlayerResolution)
        {
            Assert.Equal(before, afterDraw);
            Assert.Equal(0, await CountLedger(risk.EventId));
            Assert.All(await ReadPending(), pending => Assert.True(pending));
            await AssertRejected("AkhirGiliran", new { }, null,
                "All players must resolve their life risks before ending the turn/session");

            for (var player = 0; player < players.Length; player++)
            {
                var action = player < 2 ? "Asuransi" : "BayarRisiko";
                var responsePayload = new { risk_event_id = risk.EventId };
                var response = await Insert(action, responsePayload, player);
                await Project(response.EventPk);
                var expectedBalances = before.ToArray();
                if (player == 2) expectedBalances[2] -= 3;
                Assert.Equal(expectedBalances, await ReadBalances());
                var pending = await ReadPending();
                for (var index = 0; index < players.Length; index++)
                    Assert.Equal(index > player, pending[index]);
                Assert.Equal(player < 2 ? 2 : 1, await CountLedger(response.EventId));
                await AssertRejected("BayarRisiko", responsePayload, player,
                    "Risk event is already resolved for this player");
                await AssertRejected("Asuransi", responsePayload, player,
                    "Risk event is already resolved for this player");
                await Project(response.EventPk);
                Assert.Equal(expectedBalances, await ReadBalances());
            }

            Assert.Equal(new[] { 0, 0, 1 }, (await db.QueryAsync<int>("""
                select insurance.remaining_uses from session_participant_insurances insurance
                join session_participants player using(session_id, session_participant_id)
                where player.session_id=@SessionId order by player.player_order_no
                """, new { setup.SessionId })).ToArray());
            var settlements = (await db.QueryAsync<(Guid UserId, string Direction, int Amount, string Category)>("""
                select user_id, direction, amount, category from event_cashflow_projections
                where session_id=@SessionId and reference=@Reference
                """, new { setup.SessionId, Reference = risk.EventId.ToString() })).ToArray();
            Assert.Equal(5, settlements.Length);
            foreach (var player in players)
                Assert.Single(settlements, row => row.UserId == player.UserId
                    && row.Direction == "OUT" && row.Amount == 3 && row.Category == "RISK_LIFE");
        }
        else
        {
            var expected = before.Select((coins, index) => coins + (birthday ? index == 0 ? 2 : -1 : -3)).ToArray();
            Assert.Equal(expected, afterDraw);
            Assert.Equal(3, await CountLedger(risk.EventId));
            Assert.All(await ReadPending(), pending => Assert.False(pending));
            if (birthday) Assert.Equal(before.Sum(), afterDraw.Sum());
            else
                for (var player = 0; player < players.Length; player++)
                    await AssertRejected("BayarRisiko", new { risk_event_id = risk.EventId }, player,
                        "Risk event is already resolved for this player");
        }

        var settledBalances = await ReadBalances();
        var settledRows = await CountLedger(risk.EventId);
        await Project(risk.EventPk);
        Assert.Equal(settledBalances, await ReadBalances());
        Assert.Equal(settledRows, await CountLedger(risk.EventId));

        async Task<int[]> ReadBalances() => (await db.QueryAsync<int>("""
            select balance.coins from session_participant_balances balance
            join session_participants player using(session_id, session_participant_id)
            where player.session_id=@SessionId order by player.player_order_no
            """, new { setup.SessionId })).ToArray();

        async Task<bool[]> ReadPending() => (await db.QueryAsync<bool>("""
            select has_pending_life_risk(session_id, user_id) from session_participants
            where session_id=@SessionId order by player_order_no
            """, new { setup.SessionId })).ToArray();

        Task<int> CountLedger(Guid eventId) => db.ExecuteScalarAsync<int>("""
            select count(*) from event_cashflow_projections where session_id=@SessionId and event_id=@eventId
            """, new { setup.SessionId, eventId });

        Task Project(Guid eventPk) => db.ExecuteAsync("select project_session_event(@eventPk)", new { eventPk });

        async Task AssertRejected(string action, object payload, int? player, string message)
        {
            var error = await Assert.ThrowsAsync<PostgresException>(async () => await Insert(action, payload, player));
            Assert.Equal(PostgresErrorCodes.CheckViolation, error.SqlState);
            Assert.Contains(message, error.MessageText);
        }

        async Task<(Guid EventId, Guid EventPk)> Insert(string action, object payload, int? player)
        {
            var eventId = Guid.NewGuid();
            var eventPk = await db.QuerySingleAsync<Guid>("""
                insert into events (event_id, session_id, session_player_id, user_id, actor_type,
                    timestamp, day_index, weekday, turn_number, action_slot, sequence_number,
                    ruleset_action_id, action_type, ruleset_version_id, payload)
                select @EventId, @SessionId, @ParticipantId, @UserId, @ActorType,
                    now(), 1, 'MON', @TurnNumber, 0, @SequenceNumber,
                    action.ruleset_action_id, @ActionType, @RulesetVersionId, @Payload::jsonb
                from ruleset_actions action
                where action.ruleset_version_id=@RulesetVersionId and action.action_id=@ActionType
                returning event_pk
                """, new
            {
                EventId = eventId, setup.SessionId, setup.RulesetVersionId,
                ParticipantId = player.HasValue ? players[player.Value].ParticipantId : (Guid?)null,
                UserId = player.HasValue ? players[player.Value].UserId : (Guid?)null,
                ActorType = player.HasValue ? "PLAYER" : "SYSTEM",
                TurnNumber = player.HasValue ? players[player.Value].TurnNumber : 0,
                SequenceNumber = sequence, ActionType = action, Payload = JsonSerializer.Serialize(payload)
            });
            sequence++;
            return (eventId, eventPk);
        }
    }
}
