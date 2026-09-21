// Fungsi file: Membuktikan ingestion memvalidasi state sesudah undo yang lebih dahulu memperoleh lock sesi.
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Tests.Infrastructure;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Xunit;

namespace Cashflowpoly.Api.Tests;

[Collection("ApiIntegration")]
[Trait("Category", "Integration")]
public sealed class EventUndoConcurrencyIntegrationTests(ApiIntegrationTestFixture fixture)
{
    private CancellationToken Ct => TestContext.Current.CancellationToken;

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task IngestionQueuedAfterUndo_RejectsStaleSecondSlotWithoutSavingSnapshot(bool batch)
    {
        using var registration = await fixture.Client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            username = $"undo_race_{Guid.NewGuid():N}", password = "UndoRace!2026", role = "INSTRUCTOR"
        }, Ct);
        registration.EnsureSuccessStatusCode();
        var token = (await registration.Content.ReadFromJsonAsync<RegisterResponse>(Ct))!.AccessToken;
        var game = await new EventAnalyticsIntegrationTests(fixture).CreateReadySessionAsync(token, Guid.NewGuid().ToString("N")[..10]);
        var initial = await ReadStateAsync(token, game.SessionId);
        EventRequest Action(int slot, long sequence) => new(
            EventId: Guid.NewGuid(), SessionId: game.SessionId, UserId: game.ActingUserId,
            ActorType: "PLAYER", Timestamp: DateTimeOffset.UtcNow, DayIndex: 1, Weekday: "MON",
            TurnNumber: 1, ActionSlot: slot, SequenceNumber: sequence, ActionType: "KerjaLepas",
            RulesetVersionId: game.RulesetVersionId, ClientRequestId: Guid.NewGuid().ToString("N"),
            Payload: JsonSerializer.SerializeToElement(new { amount = 1 }));
        var first = Action(1, initial.NextSequenceNumber);
        using var firstResponse = await SendAsync(token, "/api/v1/events", first, Ct);
        Assert.True(firstResponse.StatusCode == HttpStatusCode.Created, await firstResponse.Content.ReadAsStringAsync(Ct));
        var afterFirst = await ReadStateAsync(token, game.SessionId);
        var staleSecond = Action(2, afterFirst.NextSequenceNumber);

        await using var blocker = await fixture.Services.GetRequiredService<NpgsqlDataSource>().OpenConnectionAsync(Ct);
        await using var blockTransaction = await blocker.BeginTransactionAsync(Ct);
        await blocker.ExecuteAsync(new CommandDefinition(
            "select pg_advisory_xact_lock(hashtextextended(@sessionId::text, 0))",
            new { sessionId = game.SessionId }, blockTransaction, cancellationToken: Ct));
        using var raceCancellation = CancellationTokenSource.CreateLinkedTokenSource(Ct);
        raceCancellation.CancelAfter(TimeSpan.FromSeconds(30));
        try
        {
            var undoTask = SendAsync(token, $"/api/v1/sessions/{game.SessionId}/events/{first.EventId}/undo", new UndoEventRequest
            {
                ClientRequestId = Guid.NewGuid().ToString("N"), ExpectedStateVersion = afterFirst.StateVersion,
                Reason = "Correct first action while the next request is queued"
            }, raceCancellation.Token);
            // Observe the actual database lock queue before starting the second request; no timing assumption orders them.
            await WaitForWaitersAsync(blocker, blockTransaction, 1, raceCancellation.Token);
            var ingestTask = SendAsync(token, batch ? "/api/v1/events/batch" : "/api/v1/events",
                batch ? new EventBatchRequest([staleSecond]) : staleSecond, raceCancellation.Token);
            await WaitForWaitersAsync(blocker, blockTransaction, 2, raceCancellation.Token);
            await blockTransaction.CommitAsync(Ct);

            using var undone = await undoTask;
            using var ingested = await ingestTask;
            Assert.True(undone.StatusCode == HttpStatusCode.Created, await undone.Content.ReadAsStringAsync(Ct));
            if (batch)
            {
                Assert.Equal(HttpStatusCode.OK, ingested.StatusCode);
                var result = (await ingested.Content.ReadFromJsonAsync<EventBatchResponse>(Ct))!;
                Assert.Equal(0, result.StoredCount);
                var rejected = Assert.Single(result.Failed);
                Assert.Equal(staleSecond.EventId, rejected.EventId);
                Assert.Equal("DOMAIN_RULE_VIOLATION", rejected.ErrorCode);
            }
            else
            {
                Assert.True(ingested.StatusCode == HttpStatusCode.UnprocessableEntity, await ingested.Content.ReadAsStringAsync(Ct));
                var error = (await ingested.Content.ReadFromJsonAsync<ErrorResponse>(Ct))!;
                Assert.Equal("DOMAIN_RULE_VIOLATION", error.ErrorCode);
                Assert.Contains(error.Details, detail => detail.Field == "action_slot" && detail.Issue == "OUT_OF_SEQUENCE");
            }

            var restored = await ReadStateAsync(token, game.SessionId);
            Assert.Equal(afterFirst.StateVersion + 1, restored.StateVersion);
            Assert.Equal(afterFirst.NextSequenceNumber, restored.NextSequenceNumber);
            Assert.Equal(initial.ActionSlotsLeft, restored.ActionSlotsLeft);
            Assert.Equal(initial.Players.Single(player => player.UserId == game.ActingUserId).Coins,
                restored.Players.Single(player => player.UserId == game.ActingUserId).Coins);
            Assert.Equal(1, await blocker.ExecuteScalarAsync<int>(
                "select count(*) from event_undos where session_id = @sessionId and event_id = @eventId",
                new { sessionId = game.SessionId, eventId = first.EventId }));
            Assert.Equal(0, await blocker.ExecuteScalarAsync<int>("""
                select (select count(*) from events where session_id = @sessionId and event_id = @eventId)
                    + (select count(*) from event_undo_snapshots where session_id = @sessionId and event_id = @eventId)
                    + (select count(*) from event_cashflow_projections where session_id = @sessionId and event_id = @eventId)
                """, new { sessionId = game.SessionId, eventId = staleSecond.EventId }));
        }
        finally
        {
            await raceCancellation.CancelAsync();
        }
    }

    private static async Task WaitForWaitersAsync(NpgsqlConnection blocker, NpgsqlTransaction transaction, int expected, CancellationToken ct)
    {
        var watch = Stopwatch.StartNew();
        while (watch.Elapsed < TimeSpan.FromSeconds(10))
        {
            var count = await blocker.ExecuteScalarAsync<int>(new CommandDefinition("""
                select count(*) from pg_locks waiting join pg_locks held
                    on waiting.locktype = held.locktype and waiting.database = held.database
                    and waiting.classid = held.classid and waiting.objid = held.objid and waiting.objsubid = held.objsubid
                where held.pid = pg_backend_pid() and held.locktype = 'advisory' and held.granted and not waiting.granted
                """, transaction: transaction, cancellationToken: ct));
            if (count == expected) return;
            await Task.Delay(20, ct);
        }
        Assert.Fail($"Expected {expected} requests to wait for the session advisory lock within 10 seconds.");
    }

    private async Task<SessionStateResponse> ReadStateAsync(string token, Guid sessionId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/sessions/{sessionId}/state");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var response = await fixture.Client.SendAsync(request, Ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SessionStateResponse>(Ct))!;
    }

    private async Task<HttpResponseMessage> SendAsync(string token, string path, object body, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(body);
        return await fixture.Client.SendAsync(request, ct);
    }
}
