// Fungsi file: Memastikan kegagalan sumber, cakupan sesi, dan versi aturan tidak menghasilkan data UI yang menyesatkan.
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Controllers;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class UiDataIntegrityTests
{
    private static readonly Guid SessionId = Guid.NewGuid();
    private static readonly Guid PlayerId = Guid.NewGuid();
    private static readonly Guid RulesetId = Guid.NewGuid();
    private static readonly Guid FrozenVersionId = Guid.NewGuid();
    private static readonly Guid LatestVersionId = Guid.NewGuid();
    private static readonly DateTimeOffset Date = DateTimeOffset.Parse("2026-09-01T00:00:00Z");

    [Theory]
    [InlineData("http")]
    [InlineData("network")]
    [InlineData("json")]
    public async Task Home_FailedSourceIsUnavailableWhileOtherCountsRemainValid(string failure)
    {
        var factory = new Factory(path => path.EndsWith("sessions") ? failure switch
        {
            "network" => throw new HttpRequestException("offline"),
            "json" => new(HttpStatusCode.OK) { Content = new StringContent("not json") },
            _ => new(HttpStatusCode.ServiceUnavailable)
        } : path.Contains("players") ? Json(new PlayerListResponse([])) : Json(new RulesetListResponse([])));
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var controller = Context(new HomeController(factory, cache, new ConfigurationBuilder().Build()));
        var model = Assert.IsType<HomeIndexViewModel>(Assert.IsType<ViewResult>(await controller.Index(TestContext.Current.CancellationToken)).Model);
        Assert.Null(model.TotalSessions);
        Assert.Null(model.ActiveSessions);
        Assert.Equal(0, model.TotalPlayers);
        Assert.Equal(0, model.TotalRulesets);
        Assert.NotNull(model.ErrorMessage);
    }

    [Theory]
    [InlineData("valid")]
    [InlineData("uncomputed")]
    [InlineData("other-player")]
    [InlineData("other-session")]
    [InlineData("unavailable")]
    public async Task Player_UsesVerifiedSnapshotAndNeverInfersOpeningCash(string condition)
    {
        var snapshot = Gameplay();
        if (condition == "uncomputed") snapshot = snapshot with { ComputedAt = null };
        if (condition == "other-player") snapshot = snapshot with { UserId = Guid.NewGuid() };
        if (condition == "other-session") snapshot = snapshot with { SessionId = Guid.NewGuid() };
        var factory = new Factory(path => path.EndsWith("/players") ? Roster()
            : path.EndsWith("/gameplay") ? condition == "unavailable" ? new(HttpStatusCode.ServiceUnavailable) : Json(snapshot)
            : Analytics());
        var controller = Context(new PlayersController(factory));
        var model = Assert.IsType<PlayerDetailViewModel>(Assert.IsType<ViewResult>(await controller.Details(SessionId, PlayerId, TestContext.Current.CancellationToken)).Model);
        if (condition == "valid")
        {
            Assert.Equal(37, model.Gameplay!.Economy.StartingCash);
            Assert.Equal(45, model.Gameplay.Economy.StartingCash + model.Gameplay.Economy.CashflowNetTotal);
        }
        else
        {
            Assert.Null(model.Gameplay);
            Assert.Null(model.GameplayRaw);
            Assert.NotNull(model.GameplayErrorMessage);
        }
        Assert.DoesNotContain(factory.Paths, path => path.Contains("/transactions"));
        Assert.DoesNotContain("/api/v1/players", factory.Paths);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Home_PlayerProfileIsNotCountedAsSessionParticipation(bool sessionsFailed)
    {
        var factory = new Factory(path => path.EndsWith("sessions")
            ? sessionsFailed ? new(HttpStatusCode.ServiceUnavailable) : Json(new SessionListResponse([]))
            : path.Contains("players") ? Json(new PlayerListResponse([new(PlayerId, "New account")]))
            : Json(new RulesetListResponse([])));
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var controller = Context(new HomeController(factory, cache, new ConfigurationBuilder().Build()));
        controller.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity([new(ClaimTypes.NameIdentifier, PlayerId.ToString()), new(ClaimTypes.Role, "PLAYER")], "test"));
        var model = Assert.IsType<HomeIndexViewModel>(Assert.IsType<ViewResult>(await controller.Index(TestContext.Current.CancellationToken)).Model);
        if (sessionsFailed) Assert.Null(model.TotalPlayers);
        else Assert.Equal(0, model.TotalPlayers);
    }

    [Fact]
    public async Task Player_RejectsInstructorSelectionOutsideSessionRoster()
    {
        var factory = new Factory(_ => Json(new SessionPlayerListResponse([])));
        var result = await Context(new PlayersController(factory)).Details(SessionId, PlayerId, TestContext.Current.CancellationToken);
        Assert.IsType<NotFoundResult>(result);
        Assert.Single(factory.Paths);
    }

    [Fact]
    public async Task Session_UsesFrozenRulesetVersionAndSessionScopedNames()
    {
        var factory = new Factory(path => path == $"/api/v1/analytics/sessions/{SessionId}" ? Analytics()
            : path == "/api/v1/sessions" ? Json(new SessionListResponse([new(SessionId, "Session", "MAHIR", "ENDED", Date, Date, Date)]))
            : path.EndsWith("/players") ? Roster()
            : path.Contains("/events?") ? Json(new { items = Array.Empty<object>(), has_more = false })
            : path.EndsWith("/components?version=1") ? Json(new { ruleset_id = RulesetId, ruleset_version_id = FrozenVersionId, version = 1, mode = "MAHIR", definition = new { } })
            : path == $"/api/v1/rulesets/{RulesetId}" ? Json(new RulesetDetailResponse(RulesetId, "Custom", null,
                [new(FrozenVersionId, 1, "RETIRED", Date), new(LatestVersionId, 2, "ACTIVE", Date)], LatestVersionId, 2, "PEMULA"))
            : throw new InvalidOperationException(path));
        var model = Assert.IsType<SessionDetailViewModel>(Assert.IsType<ViewResult>(await Context(new SessionsController(factory)).Details(SessionId, TestContext.Current.CancellationToken)).Model);
        Assert.Equal(FrozenVersionId, model.ActiveRulesetDetail!.Ruleset!.RulesetVersionId);
        Assert.Equal(1, model.ActiveRulesetDetail.Ruleset.Version);
        Assert.Equal("MAHIR", model.ActiveRulesetDetail.Ruleset.Mode);
        Assert.Equal("Participant", model.PlayerDisplayNames[PlayerId]);
        Assert.DoesNotContain("/api/v1/players", factory.Paths);
    }

    [Theory]
    [InlineData("http")]
    [InlineData("network")]
    [InlineData("json")]
    public async Task SessionList_FailureDoesNotClaimZeroSessions(string failure)
    {
        var factory = new Factory(_ => failure switch
        {
            "network" => throw new HttpRequestException("offline"),
            "json" => Json(new { items = (object?)null }),
            _ => new(HttpStatusCode.ServiceUnavailable)
        });
        var model = Assert.IsType<SessionListViewModel>(Assert.IsType<ViewResult>(await Context(new SessionsController(factory)).Index(TestContext.Current.CancellationToken)).Model);
        Assert.False(model.SessionsAvailable);
        Assert.Null(model.MonitoredPlayers);
        Assert.NotNull(model.ErrorMessage);
    }

    [Fact]
    public async Task SessionTimeline_RepeatedCursorCannotBecomeACompleteHistory()
    {
        var factory = new Factory(path => path.Contains("/events?")
            ? Json(new { items = Array.Empty<object>(), has_more = true, next_cursor = "repeated" })
            : path.EndsWith("/players") ? Roster()
            : path == "/api/v1/sessions" ? Json(new SessionListResponse([]))
            : path.Contains("/rulesets/") ? new(HttpStatusCode.ServiceUnavailable) : Analytics());
        var model = Assert.IsType<SessionDetailViewModel>(Assert.IsType<ViewResult>(await Context(new SessionsController(factory)).Details(SessionId, TestContext.Current.CancellationToken)).Model);
        Assert.Empty(model.Timeline);
        Assert.NotNull(model.TimelineErrorMessage);
        Assert.Equal(2, factory.Paths.Count(path => path.Contains("/events?")));
    }

    [Fact]
    public async Task Ruleset_FailedOlderVersionDoesNotShowLatestConfiguration()
    {
        var factory = new Factory(path => path.Contains("/components?") ? new(HttpStatusCode.ServiceUnavailable)
            : Json(new { ruleset_id = RulesetId, name = "Custom", versions = Array.Empty<object>(), version = 2,
                ruleset_version_id = LatestVersionId, mode = "MAHIR", definition = new { } }));
        var controller = Context(new RulesetsController(factory));
        controller.TempData = new TempDataDictionary(controller.HttpContext, new EmptyTempData());
        var model = Assert.IsType<RulesetDetailViewModel>(Assert.IsType<ViewResult>(await controller.Details(RulesetId, 1, null, null, TestContext.Current.CancellationToken)).Model);
        Assert.Null(model.CompatibilityDefinitionJson);
        Assert.Null(model.CompatibilityComponentCatalog);
        Assert.NotNull(model.ComponentsErrorMessage);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Statistics_IncompleteRosterNeverClaimsCompleteSessionCount(bool gameplayFailed)
    {
        var unavailableSession = Guid.NewGuid();
        var factory = new Factory(path => path == "/api/v1/sessions"
            ? Json(new SessionListResponse([new(SessionId, "Verified", "MAHIR", "ENDED", Date, Date, Date), new(unavailableSession, "Unknown", "MAHIR", "ENDED", Date, Date, Date)]))
            : path.Contains("/session-rosters")
                ? Json(new SessionRostersResponse([new(SessionId, [new(PlayerId, "Participant", 1, null, null)], false)]))
                : gameplayFailed ? new(HttpStatusCode.ServiceUnavailable)
                : Json(new PlayerGameplayHistoryResponse([new(SessionId, Gameplay())])));
        var result = await Context(new PlayerStatisticsController(factory)).Index("MAHIR", null, TestContext.Current.CancellationToken, PlayerId);
        var model = Assert.IsType<PlayerStatisticsViewModel>(Assert.IsType<ViewResult>(result).Model);
        Assert.Null(model.TotalSessions);
        Assert.Single(model.Sessions);
        Assert.Contains("Hanya peserta dan sesi yang berhasil diverifikasi", model.ErrorMessage);
        if (gameplayFailed) Assert.Contains("Sebagian analitika sesi", model.ErrorMessage);
    }

    [Fact]
    public async Task Statistics_FailedSessionListIsUnavailable()
    {
        var factory = new Factory(_ => new(HttpStatusCode.ServiceUnavailable));
        var result = await Context(new PlayerStatisticsController(factory)).Index("MAHIR", null, TestContext.Current.CancellationToken);
        var model = Assert.IsType<PlayerStatisticsViewModel>(Assert.IsType<ViewResult>(result).Model);
        Assert.Null(model.TotalSessions);
        Assert.NotNull(model.ErrorMessage);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task TimelinePolling_InvalidPageIsNotAnEmptySuccessfulHistory(bool repeatedCursor)
    {
        var factory = new Factory(_ => repeatedCursor
            ? Json(new { items = Array.Empty<object>(), has_more = true, next_cursor = "same" })
            : Json(new { items = (object?)null }));
        var result = Assert.IsType<JsonResult>(await Context(new SessionsController(factory)).Timeline(SessionId, "same", 100, TestContext.Current.CancellationToken));
        var payload = System.Text.Json.JsonSerializer.SerializeToElement(result.Value);
        Assert.False(string.IsNullOrWhiteSpace(payload.GetProperty("errorMessage").GetString()));
        Assert.Single(factory.Paths);
    }

    [Fact]
    public async Task Session_RejectsAnalyticsBelongingToAnotherSession()
    {
        var factory = new Factory(path => path.Contains("/analytics/")
            ? Json(new { session_id = Guid.NewGuid(), summary = new { }, by_player = Array.Empty<object>() })
            : path.EndsWith("/players") ? Roster()
            : path == "/api/v1/sessions" ? Json(new SessionListResponse([]))
            : Json(new { items = Array.Empty<object>(), has_more = false }));
        var model = Assert.IsType<SessionDetailViewModel>(Assert.IsType<ViewResult>(await Context(new SessionsController(factory)).Details(SessionId, TestContext.Current.CancellationToken)).Model);
        Assert.Null(model.Analytics);
        Assert.NotNull(model.ErrorMessage);
    }

    private static GameplayMetricsResponse Gameplay() => new(SessionId, PlayerId, Date,
        new(37, 20, 12, 8, 0), new(0, 1, 0, 2), new(30, 0, 0, 0, 0, 0, 0, 0, 0, false), new(0));
    private static HttpResponseMessage Roster() => Json(new SessionPlayerListResponse([new(PlayerId, "Participant", 1)]));
    private static HttpResponseMessage Analytics() => Json(new
    {
        session_id = SessionId, ruleset_id = RulesetId, ruleset_name = "Custom MAHIR", ruleset_version_id = FrozenVersionId,
        summary = new { }, by_player = new[] { new { user_id = PlayerId, cash_in_total = 20, cash_out_total = 12, happiness_points_total = 30 } }
    });
    private static HttpResponseMessage Json(object value) => new(HttpStatusCode.OK) { Content = JsonContent.Create(value) };
    private static T Context<T>(T controller) where T : Controller
    {
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext
        {
            Session = new MemorySession(),
            User = new ClaimsPrincipal(new ClaimsIdentity([new(ClaimTypes.NameIdentifier, PlayerId.ToString()), new(ClaimTypes.Role, "INSTRUCTOR")], "test"))
        }};
        return controller;
    }
    private sealed class Factory(Func<string, HttpResponseMessage> respond) : HttpMessageHandler, IHttpClientFactory
    {
        public ConcurrentBag<string> Paths { get; } = [];
        public HttpClient CreateClient(string name) => new(this) { BaseAddress = new Uri("http://localhost") };
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var path = request.RequestUri!.PathAndQuery;
            Paths.Add(path);
            return Task.FromResult(respond(path));
        }
    }
    private sealed class MemorySession : ISession
    {
        private readonly Dictionary<string, byte[]> values = [];
        public string Id => "integrity";
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => values.Keys;
        public void Clear() => values.Clear();
        public Task CommitAsync(CancellationToken ct = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken ct = default) => Task.CompletedTask;
        public void Remove(string key) => values.Remove(key);
        public void Set(string key, byte[] value) => values[key] = value;
        public bool TryGetValue(string key, out byte[] value) => values.TryGetValue(key, out value!);
    }
    private sealed class EmptyTempData : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }
}
