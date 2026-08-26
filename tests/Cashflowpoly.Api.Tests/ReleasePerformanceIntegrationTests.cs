// Fungsi file: Memverifikasi target beban rilis dengan 100 akun, 20 sesi aktif, dan 20 klien bersamaan.
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Tests.Infrastructure;
using Dapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Performance")]
public sealed class ReleasePerformanceIntegrationTests
{
    private const string JwtSigningKey = "release-performance-signing-key-minimum-32-chars";
    private const string SeedPassword = "SeedLocal!2026";
    [Fact]
    public async Task ReleaseDataset_MeetsConcurrencyAndP95Targets()
    {
        await using var database = new PostgreSqlBuilder("postgres:16")
            .WithDatabase("cashflowpoly_release_performance")
            .WithUsername("cashflowpoly")
            .WithPassword("cashflowpoly")
            .Build();
        await database.StartAsync();

        var previousConnection = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
        var previousSigningKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY");
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", database.GetConnectionString());
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", JwtSigningKey);

        try
        {
            await using var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey, seedSimulation: true);
            using var bootstrapClient = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            Assert.True((await bootstrapClient.GetAsync("/health/ready")).IsSuccessStatusCode);

            var performanceScopes = await PrepareReleaseDatasetAsync(database.GetConnectionString());
            var tokens = await LoginConcurrentUsersAsync(factory);
            Assert.Equal(20, tokens.Count);
            Assert.Equal(20, performanceScopes.Count);
            using (var accessClient = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }))
            {
                accessClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens[0]);
                var sessionList = await accessClient.GetFromJsonAsync<SessionListResponse>("/api/v1/sessions");
                Assert.Contains(Assert.IsType<SessionListResponse>(sessionList).Items, item => item.SessionId == performanceScopes[0].SessionId);
                var analyticsAccess = await accessClient.GetAsync(
                    $"/api/v1/analytics/sessions/{performanceScopes[0].SessionId}/players/{performanceScopes[0].PlayerUserId}/gameplay");
                Assert.True(
                    analyticsAccess.IsSuccessStatusCode,
                    $"Preflight analitik gagal: {(int)analyticsAccess.StatusCode} {await analyticsAccess.Content.ReadAsStringAsync()}");
            }

            var commonP95 = await MeasureP95Async(
                factory,
                tokens,
                Enumerable.Repeat("/api/v1/sessions", 20).ToArray(),
                requestsPerUser: 10);
            var analyticsP95 = await MeasureP95Async(
                factory,
                tokens,
                performanceScopes
                    .Select(scope => $"/api/v1/analytics/sessions/{scope.SessionId}/players/{scope.PlayerUserId}/gameplay")
                    .ToArray(),
                requestsPerUser: 10);

            Assert.True(commonP95 < 500, $"API umum p95 {commonP95:F1} ms, target < 500 ms.");
            Assert.True(analyticsP95 < 2_000, $"Analitik p95 {analyticsP95:F1} ms, target < 2.000 ms.");
        }
        finally
        {
            Environment.SetEnvironmentVariable("ConnectionStrings__Default", previousConnection);
            Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", previousSigningKey);
        }
    }

    private static async Task<IReadOnlyList<PerformanceScope>> PrepareReleaseDatasetAsync(string connectionString)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync(
            """
            insert into app_users (user_id, username, display_name, password_hash, role, is_active, is_demo)
            select
                gen_random_uuid(),
                'perf_user_' || lpad(number::text, 3, '0'),
                'Performance User ' || number,
                source.password_hash,
                case when number <= 20 then 'INSTRUCTOR' else 'PLAYER' end,
                true,
                true
            from generate_series(1, 95) as number
            cross join lateral (
                select password_hash
                from app_users
                where lower(username::text) = 'rina.kartika'
                limit 1
            ) source
            on conflict (username) do nothing;

            create temporary table performance_session_scope (
                number int primary key,
                session_id uuid not null unique
            ) on commit drop;

            insert into performance_session_scope (number, session_id)
            select number, gen_random_uuid()
            from generate_series(1, 20) as number;

            insert into sessions (
                session_id,
                session_name,
                ruleset_version_id,
                mode,
                status,
                player_count,
                started_at,
                instructor_user_id
            )
            select
                scope.session_id,
                'Performance Active Session ' || scope.number,
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524'::uuid,
                'PEMULA',
                'CREATED',
                0,
                null,
                (
                    select user_id
                    from app_users
                    where lower(username::text) = 'perf_user_' || lpad(scope.number::text, 3, '0')
                )
            from performance_session_scope scope;

            insert into session_participants (
                session_id,
                user_id,
                player_order_no,
                player_name
            )
            select
                scope.session_id,
                participant.user_id,
                participant.player_order_no,
                participant.display_name
            from performance_session_scope scope
            cross join lateral (
                select
                    user_id,
                    display_name,
                    row_number() over (order by username)::int as player_order_no
                from app_users
                where lower(username::text) in (
                    'perf_user_' || lpad((20 + ((scope.number - 1) * 2) + 1)::text, 3, '0'),
                    'perf_user_' || lpad((20 + ((scope.number - 1) * 2) + 2)::text, 3, '0')
                )
            ) participant;

            update sessions session
            set status = 'STARTED', player_count = 2, started_at = now()
            from performance_session_scope scope
            where session.session_id = scope.session_id;
            """);

        var accountCount = await connection.ExecuteScalarAsync<int>("select count(*) from app_users;");
        var activeSessionCount = await connection.ExecuteScalarAsync<int>("select count(*) from sessions where status = 'STARTED';");
        Assert.True(accountCount >= 100, $"Dataset hanya memiliki {accountCount} akun.");
        Assert.Equal(20, activeSessionCount);

        var scopes = await connection.QueryAsync<PerformanceScope>(
            """
            select
                session.session_id,
                participant.user_id as player_user_id,
                session.instructor_user_id as owner_user_id
            from sessions session
            join session_participants participant on participant.session_id = session.session_id
            join app_users player on player.user_id = participant.user_id
            join app_users owner on owner.user_id = session.instructor_user_id
            where session.session_name like 'Performance Active Session %'
              and lower(player.username::text) = 'perf_user_' || lpad(
                  (20 + ((substring(session.session_name from '[0-9]+$')::int - 1) * 2) + 1)::text,
                  3,
                  '0'
              )
            order by lower(owner.username::text)
            """);
        return scopes.ToList();
    }

    private static async Task<IReadOnlyList<string>> LoginConcurrentUsersAsync(WebApplicationFactory<Program> factory)
    {
        var loginTasks = Enumerable.Range(1, 20).Select(async number =>
        {
            return await LoginAsync(factory, $"perf_user_{number:000}");
        });

        return await Task.WhenAll(loginTasks);
    }

    private static async Task<string> LoginAsync(WebApplicationFactory<Program> factory, string username)
    {
        return (await LoginDetailsAsync(factory, username)).AccessToken;
    }

    private static async Task<LoginResponse> LoginDetailsAsync(WebApplicationFactory<Program> factory, string username)
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new LoginRequest(username, SeedPassword));
        response.EnsureSuccessStatusCode();
        var login = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return Assert.IsType<LoginResponse>(login);
    }

    private static async Task<double> MeasureP95Async(
        WebApplicationFactory<Program> factory,
        IReadOnlyList<string> tokens,
        IReadOnlyList<string> paths,
        int requestsPerUser)
    {
        Assert.Equal(tokens.Count, paths.Count);
        var durations = new ConcurrentBag<double>();
        var clients = tokens.Select(_ => factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        })).ToArray();

        try
        {
            var tasks = tokens.Select((token, index) => Task.Run(async () =>
            {
                for (var requestIndex = 0; requestIndex < requestsPerUser; requestIndex += 1)
                {
                    var path = paths[index];
                    using var request = new HttpRequestMessage(HttpMethod.Get, path);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var started = Stopwatch.GetTimestamp();
                    using var response = await clients[index].SendAsync(request);
                    durations.Add(Stopwatch.GetElapsedTime(started).TotalMilliseconds);
                    if (!response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"GET {path} menghasilkan {(int)response.StatusCode}: {body}");
                    }
                }
            }));
            await Task.WhenAll(tasks);
        }
        finally
        {
            foreach (var client in clients)
            {
                client.Dispose();
            }
        }

        var ordered = durations.Order().ToArray();
        Assert.Equal(tokens.Count * requestsPerUser, ordered.Length);
        var p95Index = Math.Max(0, (int)Math.Ceiling(ordered.Length * 0.95) - 1);
        return ordered[p95Index];
    }

    private sealed record PerformanceScope(Guid SessionId, Guid PlayerUserId, Guid OwnerUserId);
}
