// Fungsi file: Memastikan kegagalan sumber, cakupan sesi, dan versi aturan tidak menghasilkan data UI yang menyesatkan.
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Controllers;
using Cashflowpoly.Ui.Infrastructure;
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
    public async Task TimelinePolling_ForwardsUndoneSequencesEvenWithoutNewEvents()
    {
        var factory = new Factory(path => path.EndsWith("/players") ? Roster()
            : Json(new EventsBySessionResponse(SessionId, [], null, false, [], [17, 19])));
        var result = Assert.IsType<JsonResult>(await Context(new SessionsController(factory))
            .Timeline(SessionId, "last-seen", 100, TestContext.Current.CancellationToken));
        var payload = System.Text.Json.JsonSerializer.SerializeToElement(result.Value);
        Assert.Empty(payload.GetProperty("timeline").EnumerateArray());
        Assert.Equal(new long[] { 17, 19 }, payload.GetProperty("undoneSequenceNumbers").EnumerateArray().Select(item => item.GetInt64()));
        Assert.Equal(System.Text.Json.JsonValueKind.Null, payload.GetProperty("errorMessage").ValueKind);
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

    [Theory]
    [InlineData("http")]
    [InlineData("network")]
    [InlineData("timeout")]
    [InlineData("json")]
    [InlineData("empty")]
    public async Task RulesetList_DistinguishesUnavailableFromKnownEmpty(string failure)
    {
        var factory = new Factory(_ => failure switch
        {
            "network" => throw new HttpRequestException("offline"),
            "timeout" => throw new TaskCanceledException("timed out"),
            "json" => Json(new { items = (object?)null }),
            "empty" => Json(new RulesetListResponse([])),
            _ => new(HttpStatusCode.ServiceUnavailable)
        });
        var controller = Context(new RulesetsController(factory));
        controller.TempData = new TempDataDictionary(controller.HttpContext, new EmptyTempData());
        var model = Assert.IsType<RulesetListViewModel>(Assert.IsType<ViewResult>(await controller.Index(TestContext.Current.CancellationToken)).Model);
        Assert.Empty(model.Items);
        Assert.Equal(failure == "empty", model.RulesetsAvailable);
        Assert.Equal(failure != "empty", model.ErrorMessage is not null);
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, false, true)]
    [InlineData(false, true, false)]
    [InlineData(false, true, true)]
    [InlineData(true, false, false)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    [InlineData(true, true, true)]
    public async Task RulesetSave_TransportFailurePreservesEnteredForm(bool edit, bool timeout, bool catalogFailure)
    {
        var factory = new Factory(path => path.Contains("components/defaults") && !catalogFailure
            ? Json(new DefaultRulesetComponentsResponse([]))
            : timeout ? throw new TaskCanceledException("timed out") : throw new HttpRequestException("offline"));
        var controller = Context(new RulesetsController(factory));
        var entered = new CreateRulesetViewModel
        {
            Name = "Percobaan tersimpan di formulir", Description = "Jangan hilangkan masukan",
            DefinitionJson = """{"mode":"MAHIR","starting_cash":37,"freelance":{"income":3},"component_catalog":{"gameConfig":{}},"weekday_rules":{},"constraints":{},"donation":{},"gold_trade":{},"advanced":{}}"""
        };
        var definition = entered.DefinitionJson;
        var result = edit
            ? await controller.Edit(RulesetId, entered, TestContext.Current.CancellationToken)
            : await controller.Create(entered, TestContext.Current.CancellationToken);
        var model = Assert.IsType<CreateRulesetViewModel>(Assert.IsType<ViewResult>(result).Model);
        Assert.Same(entered, model);
        Assert.Equal(definition, model.DefinitionJson);
        Assert.Equal("Percobaan tersimpan di formulir", model.Name);
        Assert.Equal("Jangan hilangkan masukan", model.Description);
        Assert.Equal(edit, model.IsEditMode);
        if (edit) Assert.Equal(RulesetId, model.RulesetId);
        Assert.Equal("Layanan data belum dapat diakses. Silakan coba lagi.", model.ErrorMessage);
        Assert.Equal(catalogFailure ? 1 : 2, factory.Paths.Count);
    }

    [Theory]
    [InlineData(false, "{}")]
    [InlineData(true, "{}")]
    [InlineData(false, "[]")]
    [InlineData(true, "[]")]
    public async Task RulesetSave_MalformedConfigPreservesForm(bool edit, string definition)
    {
        var factory = new Factory(_ => Json(new DefaultRulesetComponentsResponse([])));
        var controller = Context(new RulesetsController(factory));
        var entered = new CreateRulesetViewModel { Name = "Masukan tetap ada", DefinitionJson = definition };
        var result = edit
            ? await controller.Edit(RulesetId, entered, TestContext.Current.CancellationToken)
            : await controller.Create(entered, TestContext.Current.CancellationToken);
        var model = Assert.IsType<CreateRulesetViewModel>(Assert.IsType<ViewResult>(result).Model);
        Assert.Same(entered, model);
        Assert.Equal(definition, model.DefinitionJson);
        Assert.False(string.IsNullOrWhiteSpace(model.ErrorMessage));
        Assert.Empty(factory.Paths);
    }

    [Theory]
    [InlineData("http")]
    [InlineData("network")]
    [InlineData("timeout")]
    public async Task Session_OptionalRulesetFailureKeepsLoadedSessionData(string failure)
    {
        var item = new EventRequest(Guid.NewGuid(), SessionId, PlayerId, "PLAYER", Date, 1, "MON", 1, 1,
            "KerjaLepas", FrozenVersionId, System.Text.Json.JsonSerializer.SerializeToElement(new { amount = 1 }), null);
        var factory = new Factory(path => path.Contains("/rulesets/") ? failure switch
        {
            "network" => throw new HttpRequestException("offline"),
            "timeout" => throw new TaskCanceledException("timed out"),
            _ => new(HttpStatusCode.ServiceUnavailable)
        } : path.EndsWith("/players") ? Roster()
            : path == "/api/v1/sessions" ? Json(new SessionListResponse([new(SessionId, "Session", "MAHIR", "ENDED", Date, Date, Date)]))
            : path.Contains("/events?") ? Json(new EventsBySessionResponse(SessionId, [item], "latest", false))
            : Analytics());
        var model = Assert.IsType<SessionDetailViewModel>(Assert.IsType<ViewResult>(await Context(new SessionsController(factory)).Details(SessionId, TestContext.Current.CancellationToken)).Model);
        Assert.Null(model.ActiveRulesetDetail);
        Assert.NotNull(model.Analytics);
        Assert.Equal("ENDED", model.SessionStatus);
        Assert.Equal("Participant", model.PlayerDisplayNames[PlayerId]);
        Assert.Single(model.Timeline);
        Assert.Equal("latest", model.TimelineCursor);
        Assert.Null(model.ErrorMessage);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Player_SummaryFailureUsesVerifiedRosterAndSharedGameplayFallback(bool rosterAvailable)
    {
        var factory = new Factory(path => path.EndsWith("/players")
            ? rosterAvailable ? Json(new SessionPlayerListResponse([new(PlayerId, "Third player", 3)])) : new(HttpStatusCode.ServiceUnavailable)
            : path.EndsWith("/gameplay") ? Json(Gameplay()) : new(HttpStatusCode.ServiceUnavailable));
        var model = Assert.IsType<PlayerDetailViewModel>(Assert.IsType<ViewResult>(await Context(new PlayersController(factory)).Details(SessionId, PlayerId, TestContext.Current.CancellationToken)).Model);
        Assert.Equal(rosterAvailable ? 3 : (int?)null, model.PlayerOrder);
        Assert.Equal(rosterAvailable ? "Third player" : null, model.PlayerDisplayName);
        Assert.Null(model.Summary);
        Assert.NotNull(model.Gameplay);
        Assert.Equal(20, model.StatSummary!.CashInTotal);
        Assert.Equal(12, model.StatSummary.CashOutTotal);
        Assert.Equal(8, model.StatSummary.NetCashflow);
        Assert.Equal(30, model.StatSummary.HappinessPoints);
        Assert.Equal(0, model.StatSummary.FulfillmentDiversity);
        Assert.False(model.StatSummary.HasUnpaidLoan);
        Assert.DoesNotContain(model.StatSummary.Insights, insight => insight.Key == "data_unavailable");
    }

    [Theory]
    [InlineData("status", false)]
    [InlineData("status", true)]
    [InlineData("roster", false)]
    [InlineData("roster", true)]
    [InlineData("timeline", false)]
    [InlineData("timeline", true)]
    public async Task Session_SecondaryTransportFailureKeepsAnalyticsAndOtherSources(string source, bool timeout)
    {
        var item = new EventRequest(Guid.NewGuid(), SessionId, PlayerId, "PLAYER", Date, 1, "MON", 1, 1,
            "KerjaLepas", FrozenVersionId, JsonSerializer.SerializeToElement(new { amount = 1 }), null);
        var factory = new Factory(path =>
        {
            if ((source == "status" && path == "/api/v1/sessions") ||
                (source == "roster" && path.EndsWith("/players")) ||
                (source == "timeline" && path.Contains("/events?")))
                throw timeout ? new TaskCanceledException("timed out") : new HttpRequestException("offline");
            return path == "/api/v1/sessions" ? Json(new SessionListResponse([new(SessionId, "Session", "MAHIR", "ENDED", Date, Date, Date)]))
                : path.EndsWith("/players") ? Roster()
                : path.Contains("/events?") ? Json(new EventsBySessionResponse(SessionId, [item], "latest", false))
                : path.Contains("/rulesets/") ? new(HttpStatusCode.ServiceUnavailable) : Analytics();
        });
        var model = Assert.IsType<SessionDetailViewModel>(Assert.IsType<ViewResult>(await Context(new SessionsController(factory)).Details(SessionId, TestContext.Current.CancellationToken)).Model);
        Assert.NotNull(model.Analytics);
        Assert.Equal(30, Assert.Single(model.Analytics.ByPlayer).HappinessPointsTotal);
        Assert.Equal(source == "status" ? null : "ENDED", model.SessionStatus);
        Assert.Equal(source == "roster" ? 0 : 1, model.PlayerDisplayNames.Count);
        Assert.Equal(source == "timeline" ? 0 : 1, model.Timeline.Count);
        Assert.Equal(source == "timeline", model.TimelineErrorMessage is not null);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task Ruleset_ComponentTransportFailurePreservesMainDetailWithoutInventingOlderDefinition(bool timeout, bool olderVersion)
    {
        var factory = new Factory(path => path.Contains("/components?")
            ? throw (timeout ? new TaskCanceledException("timed out") : new HttpRequestException("offline"))
            : Json(new RulesetDetailResponse(RulesetId, "Verified ruleset", null, [], LatestVersionId, 2, "MAHIR", new RulesetDefinitionDto())));
        var controller = Context(new RulesetsController(factory));
        controller.TempData = new TempDataDictionary(controller.HttpContext, new EmptyTempData());
        var model = Assert.IsType<RulesetDetailViewModel>(Assert.IsType<ViewResult>(await controller.Details(RulesetId, olderVersion ? 1 : null, null, null, TestContext.Current.CancellationToken)).Model);
        Assert.Equal("Verified ruleset", model.Ruleset!.Name);
        Assert.Null(model.Components);
        Assert.NotNull(model.ComponentsErrorMessage);
        Assert.Equal(!olderVersion, model.CompatibilityDefinitionJson.HasValue);
    }

    [Theory]
    [InlineData("PEMULA", false)]
    [InlineData("MAHIR", false)]
    [InlineData("MAHIR", true)]
    public async Task RulesetEdit_FillsMissingTargetModeCatalogAndPreservesCommonEdits(string mode, bool existingAdvancedCatalog)
    {
        var config = JsonNode.Parse(RulesetFormHelper.BuildDefaultCreateViewModel().DefinitionJson)!.AsObject();
        config["mode"] = mode;
        config["starting_cash"] = 37;
        config["freelance"]!["income"] = 3;
        config["component_catalog"] = JsonNode.Parse("""{"gameConfig":{"initialCoins":37},"bahan":[{"id":"custom-ingredient","nama":"Custom","hargaBeli":8}],"tujuanFinansial":[]}""");
        config["actions"] = JsonNode.Parse("""[{"action_id":"CustomAction"}]""");
        config["sharia_loans"] = JsonNode.Parse(existingAdvancedCatalog ? """[{"loan_code":"custom-loan","principal":9}]""" : "[]");
        config["insurance_products"] = JsonNode.Parse(existingAdvancedCatalog ? """[{"product_code":"custom-policy","premium":4}]""" : "[]");
        config["life_risks"] = new JsonArray();
        var paths = new List<string>();
        RulesetDefinitionDto? saved = null;
        var defaults = new RulesetDefinitionDto
        {
            Mode = "MAHIR", Actions = [new() { ActionId = "CustomAction" }, new() { ActionId = "Menabung" }],
            ShariaLoans = [new() { LoanCode = "default-loan", Principal = 10 }],
            InsuranceProducts = [new() { ProductCode = "default-policy", Premium = 1 }],
            FinancialGoals = [new() { Id = "default-goal", HargaBeli = 20 }],
            LifeRisks = [new() { RiskCode = "default-risk" }]
        };
        var factory = new RequestFactory(async request =>
        {
            paths.Add(request.RequestUri!.PathAndQuery);
            if (request.Method == HttpMethod.Get)
                return Json(new DefaultRulesetComponentsResponse([new(RulesetId, "Mahir", null, LatestVersionId, 1, "MAHIR", defaults)]));
            using var body = JsonDocument.Parse(await request.Content!.ReadAsStringAsync());
            saved = body.RootElement.GetProperty("definition").Deserialize<RulesetDefinitionDto>();
            return new(HttpStatusCode.OK);
        });
        var model = new CreateRulesetViewModel { Name = "Retained name", Description = "Retained description", DefinitionJson = config.ToJsonString() };
        Assert.IsType<RedirectToActionResult>(await Context(new RulesetsController(factory)).Edit(RulesetId, model, TestContext.Current.CancellationToken));
        Assert.NotNull(saved);
        Assert.Equal(37, saved.Settings.StartingCash);
        Assert.Equal(3, saved.Settings.FreelanceIncome);
        Assert.Equal("custom-ingredient", Assert.Single(saved.Ingredients).Id);
        if (mode == "MAHIR" && !existingAdvancedCatalog)
        {
            Assert.Equal(2, paths.Count);
            Assert.EndsWith("/defaults?mode=MAHIR", paths[0]);
            Assert.Equal("default-loan", Assert.Single(saved.ShariaLoans).LoanCode);
            Assert.Equal("default-policy", Assert.Single(saved.InsuranceProducts).ProductCode);
            Assert.Equal("default-goal", Assert.Single(saved.FinancialGoals).Id);
            Assert.Equal("default-risk", Assert.Single(saved.LifeRisks).RiskCode);
            Assert.Equal(new[] { "CustomAction", "Menabung" }, saved.Actions.Select(a => a.ActionId));
        }
        else
        {
            Assert.Single(paths);
            Assert.Single(saved.Actions);
            Assert.Empty(saved.FinancialGoals);
            if (existingAdvancedCatalog)
            {
                Assert.Equal("custom-loan", Assert.Single(saved.ShariaLoans).LoanCode);
                Assert.Equal("custom-policy", Assert.Single(saved.InsuranceProducts).ProductCode);
            }
            else Assert.Empty(saved.ShariaLoans);
        }
    }

    [Fact]
    public void Scores_ReadOpeningAndMissionPointsFromApiWithoutDroppingThem()
    {
        const string json = """{"happiness_points_total":33,"need_points_total":3,"initial_happiness_points":20,"mission_reward_total":10}""";
        var summary = JsonSerializer.Deserialize<AnalyticsByPlayerItem>(json)!;
        var gameplay = JsonSerializer.Deserialize<GameplayScoreMetrics>(json)!;
        Assert.Equal(20, summary.InitialHappinessPoints);
        Assert.Equal(10, summary.MissionRewardTotal);
        Assert.Equal(summary.HappinessPointsTotal,
            summary.NeedPointsTotal + summary.InitialHappinessPoints + summary.MissionRewardTotal);
        Assert.Equal(20, gameplay.InitialHappinessPoints);
        Assert.Equal(10, gameplay.MissionRewardTotal);
    }

    [Theory]
    [InlineData(0, 0, 5)]
    [InlineData(20, 0, 6)]
    [InlineData(20, 10, 7)]
    public void HappinessFormula_IncludesPositiveCustomSourcesOnly(int initial, int mission, int sourceCount)
    {
        var rows = new (string Path, string Value)[]
        {
            ("need_card_points", "5"), ("need_set_bonus_points", "0"), ("donation_points", "0"),
            ("gold_points", "0"), ("pension_points", "0"),
            ("initial_happiness_points", initial.ToString()), ("mission_reward_points", mission.ToString())
        };
        var formula = PlayerMetricCollectionHelper.BuildActualCalculation("happiness-portfolio-beginner", rows,
            "0", "%", "N/A", System.Globalization.CultureInfo.InvariantCulture);
        var total = 5 + initial + mission;
        Assert.Contains($"(5 ÷ {total})²", formula);
        Assert.Contains($"(1 − 1/{sourceCount})", formula);
        if (initial > 0) Assert.Contains($"({initial} ÷ {total})²", formula);
        if (mission > 0) Assert.Contains($"({mission} ÷ {total})²", formula);
    }

    private sealed class RequestFactory(Func<HttpRequestMessage, Task<HttpResponseMessage>> respond) : HttpMessageHandler, IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(this) { BaseAddress = new Uri("http://localhost") };
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) => respond(request);
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
