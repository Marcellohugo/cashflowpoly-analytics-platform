// Fungsi file: Menguji variasi 16 sesi demo, pembatasan instruktur, sumber analitika, dan seed yang dapat diterapkan ulang.
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
            Assert.Equal(8, sessions.Items.Count);
            Assert.Equal(4, sessions.Items.Count(s => s.Mode == "MAHIR"));
            Assert.Equal(4, sessions.Items.Count(s => s.Mode == "PEMULA"));
            var rulesets = await client.GetFromJsonAsync<RulesetListResponse>("/api/v1/rulesets");
            Assert.NotNull(rulesets);
            var owned = rulesets.Items.Where(r => !r.IsDefault).ToList();
            Assert.Equal(10, owned.Count);
            Assert.Equal(5, owned.Count(r => r.Mode == "PEMULA"));
            Assert.Equal(5, owned.Count(r => r.Mode == "MAHIR"));
            Assert.Equal(8, owned.Count(r => r.IsLockedBySession));
            var defaults = new Dictionary<string, string>();
            foreach (var mode in new[] { "PEMULA", "MAHIR" })
            {
                var baseline = rulesets.Items.Single(r => r.IsDefault && r.Mode == mode);
                var detail = await client.GetFromJsonAsync<RulesetDetailResponse>($"/api/v1/rulesets/{baseline.RulesetId}");
                Assert.NotNull(detail?.Definition);
                defaults[mode] = JsonSerializer.Serialize(detail.Definition);
            }
            foreach (var ruleset in owned)
            {
                var detail = await client.GetFromJsonAsync<RulesetDetailResponse>($"/api/v1/rulesets/{ruleset.RulesetId}");
                Assert.NotNull(detail?.Definition);
                Assert.Equal(defaults[ruleset.Mode!], JsonSerializer.Serialize(detail.Definition));
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
                Assert.Equal("ENDED", session.Status);
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
        Assert.Equal(16, seen.Count);
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
        Assert.Equal(64, await connection.QuerySingleAsync<int>("""
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
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524','PEMULA','CREATED',0,
                '90000000-0000-0000-0000-000000000001')
            """);
        var seed = await File.ReadAllTextAsync(Path.Combine(RepoRoot, "database", "02_seed_simulation_sessions_events.sql"));
        // Ulang dua kali pada koneksi yang sama untuk memeriksa objek sementara dan kestabilan pilihan acak.
        for (var run = 0; run < 2; run++)
        {
            await connection.ExecuteAsync(seed, commandTimeout: 120);
            Assert.Equal(before, await connection.QuerySingleAsync<string>(fingerprintSql));
            Assert.Equal(17, await connection.QuerySingleAsync<int>("select count(*)::int from sessions"));
            Assert.Equal(20, await connection.QuerySingleAsync<int>("select count(*)::int from rulesets where ruleset_id::text like '97000000-%'"));
            Assert.Equal("Sesi di luar seed", await connection.QuerySingleAsync<string>(
                "select session_name from sessions where session_id='92000000-0000-0000-0000-000000000001'"));
            await AssertScenarioReplayIsValidAsync(connection);
        }
    }
}
