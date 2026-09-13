// Fungsi file: Menguji cakupan akses, kelengkapan data besar, dan kesamaan perhitungan statistik batch.
using System.Diagnostics;
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
public sealed class StatisticsBatchIntegrationTests(ITestOutputHelper output)
{
    [Fact]
    public async Task Batches_Keep160SessionsScopedAndMatchIndividualMetrics()
    {
        // Synthetic sessions must not lock the shared fixture's default rulesets for other tests.
        await using var fixture = new ApiIntegrationTestFixture();
        await fixture.InitializeAsync();
        var client = fixture.Client;
        async Task<RegisterResponse> Register(string role)
        {
            using var response = await client.PostAsJsonAsync("/api/v1/auth/register",
                new RegisterRequest($"batch_{Guid.NewGuid():N}", "StatisticsTest!2026", role, "Batch participant"));
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RegisterResponse>())!;
        }
        async Task<HttpResponseMessage> Get(string path, string? token)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            if (token is not null) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await client.SendAsync(request);
        }
        async Task<T> Read<T>(string path, string token)
        {
            using var response = await Get(path, token);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<T>())!;
        }

        var owner = await Register("INSTRUCTOR");
        var other = await Register("INSTRUCTOR");
        var player = await Register("PLAYER");
        var peer = await Register("PLAYER");
        var outsider = await Register("PLAYER");
        var filler = await Register("PLAYER");
        await using var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__Default"));
        await connection.OpenAsync();
        var ids = (await connection.QueryAsync<Guid>("""
            create temporary table batch_scope (number int primary key, session_id uuid not null);
            insert into batch_scope select n, gen_random_uuid() from generate_series(1, 166) n;
            insert into sessions(session_id, session_name, ruleset_version_id, mode, status, instructor_user_id, created_at)
            select scope.session_id, 'Batch ' || scope.number, rules.ruleset_version_id, rules.mode, 'CREATED',
                case when scope.number in (163,165) then @otherId else @ownerId end,
                timestamptz '2026-01-01 00:00:00+00' + scope.number * interval '1 day'
            from batch_scope scope
            cross join lateral (select ruleset_version_id, mode from ruleset_versions
                where mode = case when scope.number = 162 then 'PEMULA' else 'MAHIR' end limit 1) rules;
            insert into session_participants(session_id,user_id,player_order_no,player_name)
            select session_id, case when number = 165 then @outsiderId else @playerId end, 1, 'Session alias'
            from batch_scope where number <> 166;
            insert into session_participants(session_id,user_id,player_order_no,player_name)
            select session_id, case when number = 160 then @peerId else @fillerId end, 2, 'Peer alias'
            from batch_scope where number not in (161,166);
            update sessions s set status = case when scope.number in (161,166) then 'CREATED' else 'ENDED' end,
                player_count = case when scope.number = 166 then 0 when scope.number = 161 then 1 else 2 end,
                started_at = case when scope.number in (161,166) then null else s.created_at end,
                ended_at = case when scope.number in (161,166) then null else s.created_at + interval '1 hour' end,
                is_archived = scope.number = 164,
                archived_at = case when scope.number = 164 then s.created_at + interval '2 hours' else null end
            from batch_scope scope where s.session_id = scope.session_id;
            insert into events(event_id,session_id,session_player_id,user_id,actor_type,"timestamp",day_index,weekday,
                turn_number,action_slot,sequence_number,ruleset_action_id,action_type,ruleset_version_id,payload)
            select gen_random_uuid(),s.session_id,p.session_participant_id,p.user_id,'PLAYER',s.started_at + interval '10 minutes',
                1,'MON',p.player_order_no,1,p.player_order_no - 1,a.ruleset_action_id,a.action_id,s.ruleset_version_id,'{"amount":1}'::jsonb
            from batch_scope scope join sessions s on s.session_id = scope.session_id
            join session_participants p on p.session_id = s.session_id
            join ruleset_actions a on a.ruleset_version_id = s.ruleset_version_id and a.action_id = 'KerjaLepas'
            where s.status = 'ENDED';
            insert into event_cashflow_projections(session_id,user_id,event_pk,event_id,"timestamp",direction,amount,category)
            select e.session_id,e.user_id,e.event_pk,e.event_id,e."timestamp",'IN',1,'FREELANCE'
            from events e join batch_scope s on s.session_id = e.session_id;
            insert into session_final_scores(session_id,session_participant_id,total_points,rank_no)
            select p.session_id,p.session_participant_id,20-p.player_order_no,p.player_order_no
            from batch_scope scope join sessions s on s.session_id = scope.session_id
            join session_participants p on p.session_id = s.session_id
            where s.status = 'ENDED' and scope.number <> 158;
            select session_id from batch_scope order by number;
            """, new { ownerId = owner.UserId, otherId = other.UserId, playerId = player.UserId,
                peerId = peer.UserId, outsiderId = outsider.UserId, fillerId = filler.UserId })).ToList();

        var watch = Stopwatch.StartNew();
        var historyPath = $"/api/v1/analytics/players/{player.UserId}/gameplay";
        var history = await Read<PlayerGameplayHistoryResponse>($"{historyPath}?mode=MAHIR", owner.AccessToken);
        watch.Stop();
        output.WriteLine("161 scoped MAHIR sessions (160 played + 1 created) fetched in {0} ms.", watch.ElapsedMilliseconds);
        Assert.Equal(ids.Take(161), history.Items.Select(i => i.SessionId));
        Assert.Null(history.Items.Single(i => i.SessionId == ids[160]).Gameplay);
        Assert.All(history.Items.Take(160), item =>
        {
            Assert.Equal(item.SessionId, item.Gameplay!.SessionId);
            Assert.Equal(player.UserId, item.Gameplay.UserId);
            Assert.NotNull(item.Gameplay.ComputedAt);
            Assert.Equal(1, item.Gameplay.Economy.CashInTotal);
        });

        foreach (var id in new[] { ids[0], ids[157], ids[159] })
        {
            var detail = await Read<GameplayMetricsResponse>($"/api/v1/analytics/sessions/{id}/players/{player.UserId}/gameplay", owner.AccessToken);
            Assert.Equal(JsonSerializer.Serialize(detail), JsonSerializer.Serialize(history.Items.Single(i => i.SessionId == id).Gameplay));
        }
        var rosters = await Read<SessionRostersResponse>("/api/v1/analytics/session-rosters", owner.AccessToken);
        Assert.Equal(163, rosters.Items.Count);
        Assert.DoesNotContain(rosters.Items, r => new[] { ids[162], ids[163], ids[164] }.Contains(r.SessionId));
        Assert.Empty(rosters.Items.Single(r => r.SessionId == ids[165]).Players);
        Assert.False(rosters.Items.Single(r => r.SessionId == ids[160]).ResultsAvailable);
        Assert.All(rosters.Items.Where(r => ids.Take(160).Contains(r.SessionId)), r => Assert.True(r.ResultsAvailable));
        Assert.Equal(2, rosters.Items.Single(r => r.SessionId == ids[159]).Players.Count);
        var noResults = await Read<SessionRostersResponse>("/api/v1/analytics/session-rosters?includeResults=false", owner.AccessToken);
        Assert.All(noResults.Items, r =>
        {
            Assert.False(r.ResultsAvailable);
            Assert.All(r.Players, p => { Assert.Null(p.FinalRank); Assert.Null(p.HappinessPointsTotal); });
        });

        var created = await Read<PlayerGameplayHistoryResponse>($"{historyPath}?mode=MAHIR&status=CREATED", owner.AccessToken);
        Assert.Equal(ids[160], Assert.Single(created.Items).SessionId);
        Assert.Null(created.Items[0].Gameplay);
        var beginner = await Read<PlayerGameplayHistoryResponse>($"{historyPath}?mode=pemula&status=ended", owner.AccessToken);
        Assert.Equal(ids[161], Assert.Single(beginner.Items).SessionId);
        var otherHistory = await Read<PlayerGameplayHistoryResponse>($"{historyPath}?mode=MAHIR", other.AccessToken);
        Assert.Equal(ids[162], Assert.Single(otherHistory.Items).SessionId);
        var ownHistory = await Read<PlayerGameplayHistoryResponse>($"{historyPath}?mode=MAHIR", player.AccessToken);
        Assert.Equal(162, ownHistory.Items.Count);
        Assert.DoesNotContain(ownHistory.Items, r => r.SessionId == ids[163] || r.SessionId == ids[164]);
        var peerRosters = await Read<SessionRostersResponse>("/api/v1/analytics/session-rosters", peer.AccessToken);
        Assert.Equal(ids[159], Assert.Single(peerRosters.Items).SessionId);
        using var deniedPlayer = await Get($"{historyPath}?mode=MAHIR", peer.AccessToken);
        Assert.Equal(HttpStatusCode.Forbidden, deniedPlayer.StatusCode);
        using var deniedOwner = await Get($"/api/v1/analytics/players/{outsider.UserId}/gameplay?mode=MAHIR", owner.AccessToken);
        Assert.Equal(HttpStatusCode.NotFound, deniedOwner.StatusCode);
        foreach (var query in new[] { "", "?mode=ALL", "?mode=MAHIR&status=invalid" })
        {
            using var invalid = await Get(historyPath + query, owner.AccessToken);
            Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        }
        using var anonymous = await Get("/api/v1/analytics/session-rosters", null);
        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);
    }
}
