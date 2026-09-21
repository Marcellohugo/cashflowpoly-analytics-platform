// Fungsi file: Menguji heartbeat, penutupan timeout, pembersihan sesi kosong, dan perlombaan transaksi sesi.
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Infrastructure;
using Cashflowpoly.Api.Tests.Infrastructure;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
public sealed class SessionLifecycleIntegrationTests(ApiIntegrationTestFixture fixture)
{
    private record Session(string Token, Guid Id, Guid UserId, Guid RulesetId, long Sequence);
    private CancellationToken Ct => TestContext.Current.CancellationToken;

    [Theory]
    [InlineData("PEMULA")]
    [InlineData("MAHIR")]
    public async Task End_SetupOnlySession_DeletesSessionAndChildrenButKeepsUsers(string mode)
    {
        var session = await Ready(mode);
        await using var db = await OpenDb();
        Assert.True(await db.ExecuteScalarAsync<int>("select count(*) from events where session_id=@id", new { id = session.Id }) > 0);
        using var ended = await Send(session, $"sessions/{session.Id}/end");
        Assert.Equal(HttpStatusCode.OK, ended.StatusCode);
        Assert.Equal("DELETED", (await ended.Content.ReadFromJsonAsync<SessionStatusResponse>(Ct))!.Status);
        Assert.Equal(0, await db.ExecuteScalarAsync<int>("select count(*) from sessions where session_id=@id", new { id = session.Id }));
        Assert.Equal(0, await db.ExecuteScalarAsync<int>("select count(*) from events where session_id=@id", new { id = session.Id }));
        Assert.Equal(0, await db.ExecuteScalarAsync<int>("select count(*) from session_setup_revisions where session_id=@id", new { id = session.Id }));
        Assert.Equal(1, await db.ExecuteScalarAsync<int>("select count(*) from app_users where user_id=@id", new { id = session.UserId }));
        using var lateEvent = await Play(session);
        Assert.Equal(HttpStatusCode.NotFound, lateEvent.StatusCode);
    }

    [Fact]
    public async Task End_CreatedSessionWithoutPlayers_DeletesIt()
    {
        var ready = await Ready();
        var session = await Created(ready);
        using var heartbeat = await Send(session, $"sessions/{session.Id}/heartbeat");
        heartbeat.EnsureSuccessStatusCode();
        var lease = (await heartbeat.Content.ReadFromJsonAsync<SessionHeartbeatResponse>(Ct))!;
        Assert.Equal("CREATED", lease.Status);
        Assert.Equal(TimeSpan.FromHours(1), lease.ExpiresAt - lease.LastActivityAt);
        Assert.Equal(1800, lease.HeartbeatIntervalSeconds);
        using var ended = await Send(session, $"sessions/{session.Id}/end");
        Assert.Equal(HttpStatusCode.OK, ended.StatusCode);
        Assert.Equal("DELETED", (await ended.Content.ReadFromJsonAsync<SessionStatusResponse>(Ct))!.Status);
        using var retry = await Send(session, $"sessions/{session.Id}/end");
        Assert.Equal(HttpStatusCode.NotFound, retry.StatusCode);
    }

    [Fact]
    public async Task Heartbeat_EnforcesOwnershipAndRole_RefreshesLeaseWithoutEvents()
    {
        var session = await Ready();
        var foreign = await Ready();
        await Expire(session);
        await using var db = await OpenDb();
        var before = await db.ExecuteScalarAsync<int>("select count(*) from events where session_id=@id", new { id = session.Id });
        using var forbidden = await Send(foreign, $"sessions/{session.Id}/heartbeat");
        Assert.Equal(HttpStatusCode.NotFound, forbidden.StatusCode);
        using var anonymous = await fixture.Client.PostAsync($"/api/v1/sessions/{session.Id}/heartbeat", null, Ct);
        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);
        using var registration = await fixture.Client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterRequest($"heartbeat_player_{Guid.NewGuid():N}", "LifecycleTest!2026", "PLAYER", "Player"), Ct);
        registration.EnsureSuccessStatusCode();
        var playerToken = (await registration.Content.ReadFromJsonAsync<RegisterResponse>(Ct))!.AccessToken;
        using var playerHeartbeat = await Send(session with { Token = playerToken }, $"sessions/{session.Id}/heartbeat");
        Assert.Equal(HttpStatusCode.Forbidden, playerHeartbeat.StatusCode);
        using var response = await Send(session, $"sessions/{session.Id}/heartbeat");
        response.EnsureSuccessStatusCode();
        var heartbeat = (await response.Content.ReadFromJsonAsync<SessionHeartbeatResponse>(Ct))!;
        Assert.Equal("STARTED", heartbeat.Status);
        Assert.Equal(TimeSpan.FromHours(1), heartbeat.ExpiresAt - heartbeat.LastActivityAt);
        Assert.Equal(1800, heartbeat.HeartbeatIntervalSeconds);
        Assert.Equal(before, await db.ExecuteScalarAsync<int>("select count(*) from events where session_id=@id", new { id = session.Id }));
        using var scope = fixture.Services.CreateScope();
        Assert.Null(await scope.ServiceProvider.GetRequiredService<SessionStateRepository>()
            .EndSessionAsync(session.Id, Ct, TimeSpan.FromHours(1), TimeSpan.FromHours(1)));
    }

    [Fact]
    public async Task Sweep_EndsGameplay_DeletesExpiredAndLegacyEmptySessions_PreservesFreshSetup()
    {
        var played = await Ready();
        var empty = await Ready();
        var legacy = await Ready();
        var created = await Created(played);
        var fresh = await Created(played);
        using var stored = await Play(played);
        stored.EnsureSuccessStatusCode();
        await using var db = await OpenDb();
        await db.ExecuteAsync("update sessions set last_activity_at=clock_timestamp()-interval '59 minutes' where session_id=any(@ids)",
            new { ids = new[] { played.Id, fresh.Id } });
        await Sweep();
        Assert.Equal("STARTED", await db.ExecuteScalarAsync<string>("select status from sessions where session_id=@id", new { id = played.Id }));
        Assert.Equal("CREATED", await db.ExecuteScalarAsync<string>("select status from sessions where session_id=@id", new { id = fresh.Id }));
        await db.ExecuteAsync("update sessions set status='ENDED', ended_at=clock_timestamp() where session_id=@id", new { id = legacy.Id });
        foreach (var session in new[] { played, empty, created }) await Expire(session);
        await Sweep();
        var result = await db.QuerySingleAsync<(string Status, string Reason, bool GameOver, int Scores)>("""
            select s.status, s.end_reason, st.is_game_over,
                (select count(*)::int from session_final_scores where session_id=s.session_id)
            from sessions s join session_states st using(session_id) where s.session_id=@id
            """, new { id = played.Id });
        Assert.Equal(("ENDED", "HEARTBEAT_TIMEOUT", true, 3), result);
        foreach (var session in new[] { empty, created, legacy })
            Assert.Equal(0, await db.ExecuteScalarAsync<int>("select count(*) from sessions where session_id=@id", new { id = session.Id }));
        Assert.Equal("CREATED", await db.ExecuteScalarAsync<string>("select status from sessions where session_id=@id", new { id = fresh.Id }));
        using var lateHeartbeat = await Send(played, $"sessions/{played.Id}/heartbeat");
        Assert.Equal(HttpStatusCode.Conflict, lateHeartbeat.StatusCode);
        using var lateEvent = await Play(played with { Sequence = played.Sequence + 1 });
        Assert.Equal(HttpStatusCode.UnprocessableEntity, lateEvent.StatusCode);
        var version = await db.ExecuteScalarAsync<long>("select state_version from session_states where session_id=@id", new { id = played.Id });
        await Sweep();
        Assert.Equal(version, await db.ExecuteScalarAsync<long>("select state_version from session_states where session_id=@id", new { id = played.Id }));
    }

    [Fact]
    public async Task AcceptedEvent_RefreshesLease_AndConcurrentClosuresFinalizeOnlyOnce()
    {
        var session = await Ready();
        await Expire(session);
        using var stored = await Play(session);
        stored.EnsureSuccessStatusCode();
        await Sweep();
        await using var db = await OpenDb();
        Assert.Equal("STARTED", await db.ExecuteScalarAsync<string>("select status from sessions where session_id=@id", new { id = session.Id }));
        await Expire(session);
        using var firstScope = fixture.Services.CreateScope();
        using var secondScope = fixture.Services.CreateScope();
        var results = await Task.WhenAll(
            firstScope.ServiceProvider.GetRequiredService<SessionStateRepository>().EndSessionAsync(session.Id, Ct, TimeSpan.FromHours(1)),
            secondScope.ServiceProvider.GetRequiredService<SessionStateRepository>().EndSessionAsync(session.Id, Ct, TimeSpan.FromHours(1)));
        Assert.Single(results, result => result == "ENDED");
        Assert.Single(results, result => result is null);
    }

    [Fact]
    public async Task Timeout_ClosesCommittedStateEvenWithPendingLifeRisk()
    {
        var definition = EventAnalyticsIntegrationTests.BuildRulesetDefinition(50, mode: "MAHIR", riskAmount: 100);
        var session = await Ready("MAHIR", definition);
        var orderId = Guid.NewGuid();
        using var order = await Play(session, "JualMasakan", new { order_card_id = "nasi_goreng" }, orderId);
        order.EnsureSuccessStatusCode();
        using var risk = await Play(session with { Sequence = session.Sequence + 1 }, "RisikoKehidupan",
            new { risk_id = "risk_cost_4", source_order_event_id = orderId }, slot: 0);
        risk.EnsureSuccessStatusCode();
        using var manualEnd = await Send(session, $"sessions/{session.Id}/end");
        Assert.Equal(HttpStatusCode.UnprocessableEntity, manualEnd.StatusCode);
        await Expire(session);
        await Sweep();
        await using var db = await OpenDb();
        Assert.Equal("HEARTBEAT_TIMEOUT", await db.ExecuteScalarAsync<string>("select end_reason from sessions where session_id=@id", new { id = session.Id }));
    }

    private async Task<Session> Ready(string mode = "PEMULA", RulesetDefinitionDto? definition = null)
    {
        var suffix = Guid.NewGuid().ToString("N")[..10];
        using var registration = await fixture.Client.PostAsJsonAsync("/api/v1/auth/register",
            new RegisterRequest($"lifecycle_{suffix}", "LifecycleTest!2026", "INSTRUCTOR", "Lifecycle"), Ct);
        registration.EnsureSuccessStatusCode();
        var token = (await registration.Content.ReadFromJsonAsync<RegisterResponse>(Ct))!.AccessToken;
        var setup = await new EventAnalyticsIntegrationTests(fixture).CreateReadySessionAsync(token, suffix,
            definition ?? EventAnalyticsIntegrationTests.BuildRulesetDefinition(50, mode: mode));
        return new Session(token, setup.SessionId, setup.ActingUserId, setup.RulesetVersionId, setup.NextSequenceNumber);
    }

    private async Task<Session> Created(Session source)
    {
        using var response = await Send(source, "sessions", new
        {
            session_name = "Never started", mode = "PEMULA", ruleset_version_id = source.RulesetId
        });
        response.EnsureSuccessStatusCode();
        return source with { Id = (await response.Content.ReadFromJsonAsync<CreateSessionResponse>(Ct))!.SessionId };
    }

    private Task<HttpResponseMessage> Play(Session session, string action = "KerjaLepas", object? payload = null, Guid? eventId = null, int slot = 1) =>
        Send(session, "events", new
        {
            event_id = eventId ?? Guid.NewGuid(), session_id = session.Id, user_id = session.UserId,
            actor_type = "PLAYER", timestamp = DateTimeOffset.UtcNow, day_index = 1, weekday = "MON",
            turn_number = 1, action_slot = slot, sequence_number = session.Sequence,
            action_type = action, ruleset_version_id = session.RulesetId, payload = payload ?? new { amount = 1 }
        });

    private async Task<HttpResponseMessage> Send(Session session, string path, object? body = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/{path}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.Token);
        if (body is not null) request.Content = JsonContent.Create(body);
        return await fixture.Client.SendAsync(request, Ct);
    }

    private async Task Expire(Session session)
    {
        await using var db = await OpenDb();
        await db.ExecuteAsync("update sessions set last_activity_at=clock_timestamp()-interval '61 minutes' where session_id=@id", new { id = session.Id });
    }

    private async Task<NpgsqlConnection> OpenDb()
    {
        var db = new NpgsqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__Default"));
        await db.OpenAsync(Ct);
        return db;
    }

    private Task Sweep() => fixture.Services.GetServices<IHostedService>().OfType<SessionLifecycleWorker>().Single().SweepAsync(Ct);
}
