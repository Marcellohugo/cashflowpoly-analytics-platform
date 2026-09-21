// Fungsi file: Memverifikasi poin awal, hadiah misi, finalisasi, dan koreksi data historis pada database terisolasi.
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
public sealed class ScoringConsistencyIntegrationTests(ApiIntegrationTestFixture fixture)
{
    [Theory]
    [InlineData(0, 0, 10, true)]
    [InlineData(20, 0, 10, true)]
    [InlineData(0, 20, 10, true)]
    [InlineData(20, 20, 30, true)]
    [InlineData(20, 20, 30, false)]
    public async Task InitialAndMissionPointsSurviveFinalizationRecomputeAndRepeatedProjection(
        int initialHappiness, int reward, int penalty, bool complete)
    {
        var account = await Register();
        var json = JsonSerializer.SerializeToNode(EventAnalyticsIntegrationTests.BuildRulesetDefinition(50, mode: "MAHIR"))!;
        json["settings"]!["initial_happiness"] = initialHappiness;
        var definition = json.Deserialize<RulesetDefinitionDto>()!;
        for (var i = 0; i < definition.CollectionMissions.Count; i++)
            definition.CollectionMissions[i] = new RulesetCollectionMissionDto
            {
                Id = definition.CollectionMissions[i].Id, Nama = "Book mission", SuccessPoints = reward,
                FailurePoints = -penalty, // The signed alias alone is accepted and normalized on persistence.
                KebutuhanTarget = [new() { Order = 1, Type = "ASSET", Value = "buku" }]
            };
        var setup = await new EventAnalyticsIntegrationTests(fixture)
            .CreateReadySessionAsync(account.AccessToken, Guid.NewGuid().ToString("N")[..8], definition);
        await using var db = new NpgsqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__Default"));
        await db.OpenAsync();
        var parameters = new { setup.SessionId, userId = setup.ActingUserId };
        Assert.Equal(penalty, await db.QuerySingleAsync<int>("""
            select catalog.penalty_points from session_participant_collection_missions mission
            join session_participants player using(session_id, session_participant_id)
            join ruleset_collection_missions catalog using(ruleset_collection_mission_id)
            where player.session_id=@SessionId and player.user_id=@userId
            """, parameters));
        if (complete)
        {
            using var purchased = await Send(HttpMethod.Post, "/api/v1/events", account.AccessToken, new
            {
                event_id = Guid.NewGuid(), session_id = setup.SessionId, user_id = setup.ActingUserId,
                actor_type = "PLAYER", timestamp = DateTimeOffset.UtcNow, day_index = 1, weekday = "MON",
                turn_number = 1, action_slot = 1, sequence_number = setup.NextSequenceNumber,
                action_type = "Kebutuhan", ruleset_version_id = setup.RulesetVersionId,
                payload = new { card_id = "buku", amount = 3, points = 1, need_tier = "primer" }
            });
            Assert.True(purchased.StatusCode == HttpStatusCode.Created, await purchased.Content.ReadAsStringAsync());
        }
        else
        {
            await SessionSetupTestHelper.PlayFirstActionAsync(fixture.Client, account.AccessToken,
                setup.SessionId, setup.RulesetVersionId, TestContext.Current.CancellationToken);
        }

        double? originalTotal = null;
        foreach (var stage in new[] { "live", "refresh", "refresh_again", "ended", "recomputed", "recomputed_again", "legacy_migration" })
        {
            if (stage.StartsWith("refresh", StringComparison.Ordinal))
                await db.ExecuteAsync("""
                    update session_participant_collection_missions set is_completed=is_completed
                    where session_id=@SessionId
                    """, parameters);
            if (stage == "legacy_migration")
            {
                // Restore the pre-fix representation in this isolated fixture, then run the actual migration.
                await using var tx = await db.BeginTransactionAsync();
                await db.ExecuteAsync("""
                    with removed as (
                        delete from session_final_score_components where session_id=@SessionId
                        and component_code in ('INITIAL_HAPPINESS','MISSION_REWARD')
                        returning session_final_score_id,points
                    ), delta as (select session_final_score_id,sum(points)::int amount from removed group by session_final_score_id)
                    update session_final_scores score set total_points=total_points-delta.amount
                    from delta where delta.session_final_score_id=score.session_final_score_id;
                    update session_participant_balances balance set happiness=happiness-catalog.success_points
                    from session_participant_collection_missions mission
                    join ruleset_collection_missions catalog using(ruleset_collection_mission_id)
                    where balance.session_participant_id=mission.session_participant_id
                    and mission.session_id=@SessionId and mission.reward_applied;
                    update session_participant_collection_missions set reward_applied=false where session_id=@SessionId;
                    drop trigger trg_collection_mission_reward on session_participant_collection_missions;
                    """, parameters, tx);
                await db.ExecuteAsync(await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory,
                    "database", "migrations", "V006__initial_and_mission_happiness.sql")), transaction: tx);
                await tx.CommitAsync();
            }
            if (stage == "ended" || stage.StartsWith("recomputed", StringComparison.Ordinal))
            {
                var path = stage == "ended" ? $"/api/v1/sessions/{setup.SessionId}/end" : $"/api/v1/analytics/sessions/{setup.SessionId}/recompute";
                using var changed = await Send(HttpMethod.Post, path, account.AccessToken);
                Assert.True(changed.IsSuccessStatusCode, await changed.Content.ReadAsStringAsync());
            }
            using var response = await Send(HttpMethod.Get,
                $"/api/v1/analytics/sessions/{setup.SessionId}/players/{setup.ActingUserId}/gameplay", account.AccessToken);
            response.EnsureSuccessStatusCode();
            var gameplay = (await response.Content.ReadFromJsonAsync<GameplayMetricsResponse>())!;
            var score = gameplay.Score;
            Assert.Equal(initialHappiness, score.InitialHappinessPoints);
            Assert.Equal(complete ? reward : 0, score.MissionRewardTotal);
            Assert.Equal(complete ? 0 : penalty, score.MissionPenaltyTotal);
            Assert.Equal(score.InitialHappinessPoints + score.MissionRewardTotal + score.NeedPointsTotal + score.NeedSetBonusPoints +
                score.DonationPointsTotal + score.GoldPointsTotal + score.PensionPointsTotal + score.SavingGoalPointsTotal -
                score.MissionPenaltyTotal - score.LoanPenaltyTotal, score.HappinessPointsTotal);
            originalTotal ??= score.HappinessPointsTotal;
            Assert.Equal(originalTotal, score.HappinessPointsTotal);
            var composition = gameplay.DerivedJson!.Value.GetProperty("happiness_points_composition");
            Assert.Equal(initialHappiness, composition.GetProperty("initial_happiness_points").GetInt32());
            Assert.Equal(complete ? reward : 0, composition.GetProperty("mission_reward_points").GetInt32());
            var state = await db.QuerySingleAsync<(int Happiness, bool Complete, bool RewardApplied)>("""
                select balance.happiness, mission.is_completed, mission.reward_applied
                from session_participants player
                join session_participant_balances balance using(session_id,session_participant_id)
                join session_participant_collection_missions mission using(session_id,session_participant_id)
                where player.session_id=@SessionId and player.user_id=@userId
                """, parameters);
            Assert.Equal(initialHappiness + (complete ? 1 + reward : 0), state.Happiness);
            Assert.Equal(complete, state.Complete);
            Assert.Equal(complete && reward > 0, state.RewardApplied);
            if (stage is "ended" or "recomputed" or "recomputed_again" or "legacy_migration")
            {
                var final = await db.QuerySingleAsync<(int Total, int Components)>("""
                    select final.total_points, sum(component.points)::int
                    from session_final_scores final join session_participants player using(session_id,session_participant_id)
                    join session_final_score_components component using(session_final_score_id)
                    where player.session_id=@SessionId and player.user_id=@userId
                    group by final.session_final_score_id
                    """, parameters);
                Assert.Equal(score.HappinessPointsTotal, final.Total);
                Assert.Equal(final.Total, final.Components);
            }
        }
    }

    [Theory]
    [InlineData(20, -30, 10, "MUST_MATCH_NEGATIVE_PENALTY")]
    [InlineData(-1, -10, 10, "OUT_OF_RANGE")]
    [InlineData(20, 10, 10, "OUT_OF_RANGE")]
    public async Task InvalidMissionScoringReturnsFieldError(int reward, int failure, int penalty, string issue)
    {
        var account = await Register();
        var definition = EventAnalyticsIntegrationTests.BuildRulesetDefinition(50);
        var old = definition.CollectionMissions[0];
        definition.CollectionMissions[0] = new RulesetCollectionMissionDto
        {
            Id = old.Id, Nama = old.Nama, KebutuhanTarget = old.KebutuhanTarget,
            SuccessPoints = reward, FailurePoints = failure, PenaltyPoints = penalty
        };
        using var response = await Send(HttpMethod.Post, "/api/v1/rulesets", account.AccessToken, new { name = "Invalid scoring", definition });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(issue, await response.Content.ReadAsStringAsync());
    }

    private async Task<RegisterResponse> Register()
    {
        using var response = await fixture.Client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterRequest($"score_{Guid.NewGuid():N}", "ScoringRegression!2026", "INSTRUCTOR", "Scoring regression"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<RegisterResponse>())!;
    }

    private async Task<HttpResponseMessage> Send(HttpMethod method, string path, string token, object? body = null)
    {
        using var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body is not null) request.Content = JsonContent.Create(body);
        return await fixture.Client.SendAsync(request, TestContext.Current.CancellationToken);
    }
}
