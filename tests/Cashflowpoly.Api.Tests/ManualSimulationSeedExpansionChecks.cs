// Fungsi file: Menguji variasi 24 sesi demo, pembatasan instruktur, sumber analitika, dan seed yang dapat diterapkan ulang.
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Dapper;
using Npgsql;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed partial class ManualSimulationSeedIntegrationTests
{
    private static async Task AssertExpandedSeedAccessAndMetricsAsync(HttpClient client, string connectionString)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        var seen = new HashSet<Guid>();
        foreach (var username in new[] { "hadziq", "pratama" })
        {
            var response = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(username, SeedInstructorPassword));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var auth = await response.Content.ReadFromJsonAsync<LoginResponse>();
            Assert.NotNull(auth);
            Assert.Equal("INSTRUCTOR", auth.Role);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
            var sessions = await client.GetFromJsonAsync<SessionListResponse>("/api/v1/sessions");
            Assert.NotNull(sessions);
            Assert.Equal(12, sessions.Items.Count);
            Assert.Equal(8, sessions.Items.Count(s => s.Status == "ENDED"));
            Assert.Equal(2, sessions.Items.Count(s => s.Status == "CREATED"));
            Assert.Equal(2, sessions.Items.Count(s => s.Status == "STARTED"));
            Assert.Equal(6, sessions.Items.Count(s => s.Mode == "MAHIR"));
            Assert.Equal(6, sessions.Items.Count(s => s.Mode == "PEMULA"));
            var rulesets = await client.GetFromJsonAsync<RulesetListResponse>("/api/v1/rulesets");
            Assert.NotNull(rulesets);
            var owned = rulesets.Items.Where(r => !r.IsDefault).ToList();
            Assert.Equal(10, owned.Count);
            Assert.Equal(5, owned.Count(r => r.Mode == "PEMULA"));
            Assert.Equal(5, owned.Count(r => r.Mode == "MAHIR"));
            Assert.Equal(10, owned.Count(r => r.IsLockedBySession));
            var definitions = new HashSet<string>();
            foreach (var ruleset in owned)
            {
                var detail = await client.GetFromJsonAsync<RulesetDetailResponse>($"/api/v1/rulesets/{ruleset.RulesetId}");
                Assert.NotNull(detail?.Definition);
                Assert.True(definitions.Add(JsonSerializer.Serialize(detail.Definition)), "Setiap ruleset instruktur harus memiliki konfigurasi yang berbeda.");
            }
            Assert.Equal(0, await connection.QuerySingleAsync<int>("""
                select count(*)::int from sessions s
                join ruleset_versions v using(ruleset_version_id) join rulesets r using(ruleset_id)
                where s.session_id::text like '91000000-%'
                  and (r.instructor_user_id is distinct from s.instructor_user_id or s.mode<>v.mode)
                """));
            foreach (var session in sessions.Items)
            {
                Assert.True(seen.Add(session.SessionId), "Daftar sesi instruktur tidak boleh tumpang tindih.");
                if (session.Status == "CREATED")
                {
                    Assert.Equal(0, await connection.QuerySingleAsync<int>("select count(*)::int from events where session_id=@id", new { id = session.SessionId }));
                    continue;
                }
                // Dua sesi acuan sudah dihitung oleh pemeriksaan dasar sebelum helper ini dipanggil.
                if (session.SessionName != SeedPemulaSessionName && session.SessionName != SeedMahirSessionName)
                {
                    var recompute = await client.PostAsync($"/api/v1/analytics/sessions/{session.SessionId}/recompute", null);
                    Assert.Equal(HttpStatusCode.OK, recompute.StatusCode);
                }
                var players = (await connection.QueryAsync<(Guid UserId, int Coins)>("""
                    select sp.user_id, b.coins from session_participants sp
                    join session_participant_balances b using(session_id, session_participant_id)
                    where sp.session_id = @id
                    """, new { id = session.SessionId })).ToList();
                Assert.Equal(4, players.Count);
                foreach (var player in players)
                {
                    var gameplay = await client.GetFromJsonAsync<GameplayMetricsResponse>(
                        $"/api/v1/analytics/sessions/{session.SessionId}/players/{player.UserId}/gameplay");
                    Assert.NotNull(gameplay);
                    Assert.NotNull(gameplay.RawJson);
                    Assert.NotNull(gameplay.DerivedJson);
                    Assert.Equal(player.Coins, gameplay.RawJson.Value.GetProperty("coins").GetProperty("coins_held_current").GetInt32());
                    Assert.Equal(player.Coins, gameplay.Economy.StartingCash + gameplay.Economy.CashflowNetTotal);
                }
            }
            // Marco mengikuti kedua instruktur; kepesertaan bersama tidak membuka sesi instruktur lain.
            var foreign = username == "hadziq" ? "009" : "001";
            var denied = await client.GetAsync($"/api/v1/analytics/sessions/91000000-0000-0000-0000-000000000{foreign}/players/90000000-0000-0000-0000-000000000011/gameplay");
            Assert.Equal(HttpStatusCode.NotFound, denied.StatusCode);
        }
        Assert.Equal(24, seen.Count);
        Assert.Equal(0, await connection.QuerySingleAsync<int>("""
            select count(*)::int from sessions s
            where s.session_id=any(@ids) and (
                (s.status='ENDED' and s.ended_at is distinct from (select max(e.timestamp) from events e where e.session_id=s.session_id))
                or (s.status<>'ENDED' and (s.ended_at is not null or exists(select 1 from session_final_scores f where f.session_id=s.session_id)))
                or (s.status='CREATED' and s.started_at is not null))
            """, new { ids = seen.ToArray() }));
        Assert.Equal(0, await connection.QuerySingleAsync<int>("""
            select count(*)::int from events e join ruleset_game_settings gs using(ruleset_version_id)
            where e.session_id=any(@ids) and e.action_type='KerjaLepas'
              and (e.payload->>'amount')::int<>gs.freelance_income
            """, new { ids = seen.ToArray() }));
        Assert.Equal("hadziq", await connection.QuerySingleAsync<string>(
            "select username from app_users where user_id='90000000-0000-0000-0000-000000000001'"));
        var variations = (await connection.QueryAsync<int>("""
            select count(distinct b.coins)::int from sessions s
            join session_participants sp using(session_id)
            join session_participant_balances b using(session_id, session_participant_id)
            where s.session_id = any(@ids) group by sp.user_id, s.mode
            """, new { ids = seen.ToArray() })).ToList();
        Assert.Equal(12, variations.Count);
        Assert.All(variations, count => Assert.True(count >= 2, "Saldo tiap pemain/mode harus bervariasi antarsesi."));
        Assert.Equal(80, await connection.QuerySingleAsync<int>("""
            select count(*)::int from metric_snapshots
            where session_id = any(@ids) and metric_name='gameplay.derived.metrics'
              and metric_payload_json is not null
            """, new { ids = seen.ToArray() }));
    }

    private static async Task AssertExpandedSeedCanBeReappliedAsync(NpgsqlConnection connection)
    {
        const string fingerprintSql = """
            select md5(string_agg(concat_ws('|', e.session_id, e.event_id, e.session_player_id,
                e.user_id, e.timestamp, e.action_slot, e.sequence_number, e.payload), E'\n'
                order by e.session_id, e.sequence_number))
            from events e where e.session_id::text like '91000000-%'
            """;
        var before = await connection.QuerySingleAsync<string>(fingerprintSql);
        await connection.ExecuteAsync("""
            insert into sessions(session_id,session_name,ruleset_version_id,mode,status,player_count,instructor_user_id)
            values('92000000-0000-0000-0000-000000000001','Sesi di luar seed',
                '98100000-0000-0000-0000-000000000003','PEMULA','CREATED',0,
                '90000000-0000-0000-0000-000000000001')
            """);
        // Model an existing session pinned to the first published version; seed must leave its rules intact.
        await connection.ExecuteAsync("""
            update ruleset_versions set status='ARCHIVED' where ruleset_version_id='98100000-0000-0000-0000-000000000003';
            update ruleset_versions set status='ACTIVE' where ruleset_version_id='98000000-0000-0000-0000-000000000003';
            update sessions set ruleset_version_id='98000000-0000-0000-0000-000000000003'
            where session_id='92000000-0000-0000-0000-000000000001';
            update ruleset_versions set status='ARCHIVED' where ruleset_version_id='98000000-0000-0000-0000-000000000003';
            update ruleset_versions set status='ACTIVE' where ruleset_version_id='98100000-0000-0000-0000-000000000003';
            """);
        var settingsBefore = await connection.QuerySingleAsync<string>("select to_jsonb(s)::text from ruleset_game_settings s where ruleset_version_id='98000000-0000-0000-0000-000000000003'");
        var seed = await File.ReadAllTextAsync(Path.Combine(RepoRoot, "database", "02_seed_simulation_sessions_events.sql"));
        // Ulang dua kali pada koneksi yang sama untuk memeriksa objek sementara dan kestabilan pilihan acak.
        for (var run = 0; run < 2; run++)
        {
            await connection.ExecuteAsync(seed, commandTimeout: 300);
            Assert.Equal(before, await connection.QuerySingleAsync<string>(fingerprintSql));
            Assert.Equal(25, await connection.QuerySingleAsync<int>("select count(*)::int from sessions"));
            Assert.Equal(20, await connection.QuerySingleAsync<int>("select count(*)::int from rulesets where ruleset_id::text like '97000000-%'"));
            Assert.Equal("Sesi di luar seed", await connection.QuerySingleAsync<string>(
                "select session_name from sessions where session_id='92000000-0000-0000-0000-000000000001'"));
            Assert.Equal(settingsBefore, await connection.QuerySingleAsync<string>("select to_jsonb(s)::text from ruleset_game_settings s where ruleset_version_id='98000000-0000-0000-0000-000000000003'"));
            Assert.Equal("98000000-0000-0000-0000-000000000003", await connection.QuerySingleAsync<string>("select ruleset_version_id::text from sessions where session_id='92000000-0000-0000-0000-000000000001'"));
            await AssertScenarioReplayIsValidAsync(connection);
        }
    }
}
