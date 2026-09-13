// Fungsi file: Menguji kesesuaian angka histori, data yang tidak tersedia, filter sesi, dan identitas pemain.
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Controllers;
using Cashflowpoly.Ui.Infrastructure;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class PlayerStatisticsTests
{
    private static readonly Guid Player = Guid.NewGuid();
    private static readonly DateTimeOffset Date = DateTimeOffset.Parse("2026-02-01T00:00:00Z");
    private static string T(string key) => UiText.Translate("id", key);

    [Theory]
    [InlineData(120, 20, "+20")]
    [InlineData(80, -20, "-20")]
    [InlineData(100, 0, "0")]
    public void Charts_UseSignedCoinChangeLoanCountsAndGoalCompletionPercentage(double stored, double plotted, string display)
    {
        var session = Session("MAHIR");
        var gameplay = Gameplay(session) with
        {
            DerivedJson = JsonSerializer.SerializeToElement(new { cash_growth_percent = stored, financial_goal_completion_percent = 50, donation_commitment_score = 9.76 }),
            RawJson = JsonSerializer.SerializeToElement(new { financial_goals = new { financial_goals_completed = 1, sharia_loans_taken = 2, sharia_loans_repaid = 1 } })
        };
        var charts = PlayerStatisticsChartBuilder.Build([new(session, gameplay)], T);
        var growth = Assert.Single(charts, c => c.Key == "cash_growth_percent");
        var point = Assert.Single(growth.Points);
        Assert.Equal(plotted, point.Value);
        Assert.Equal(display, point.Presentation.DisplayValue);
        using var json = JsonDocument.Parse(growth.Json);
        Assert.Equal(plotted, json.RootElement.GetProperty("series")[0].GetProperty("values")[0].GetDouble());
        var detail = json.RootElement.GetProperty("pointDetails")[0];
        Assert.Equal(session.SessionName, detail.GetProperty("sessionName").GetString());
        Assert.Equal(display, detail.GetProperty("displayValue").GetString());
        Assert.Equal("%", detail.GetProperty("unit").GetString());
        Assert.Equal(point.Presentation.Guidance, detail.GetProperty("guidance").GetString());
        Assert.Equal(50, Assert.Single(charts.Single(c => c.Key == "financial_goal_completion_percent").Points).Value);
        Assert.Equal(2, Assert.Single(charts.Single(c => c.Key == "sharia_loans_taken").Points).Value);
        Assert.Equal(1, Assert.Single(charts.Single(c => c.Key == "sharia_loans_repaid").Points).Value);
        Assert.Equal("pinjaman", charts.Single(c => c.Key == "sharia_loans_taken").Unit);
        Assert.Equal(9.76, Assert.Single(charts.Single(c => c.Key == "donation_commitment_score").Points).Value);
        Assert.Equal("%", charts.Single(c => c.Key == "donation_commitment_score").Unit);
        Assert.Equal(12, Assert.Single(charts.Single(c => c.Key == "coins_net_end_game").Points).Value);
        Assert.Equal(36, Assert.Single(charts.Single(c => c.Key == "total_happiness_points").Points).Value);
        Assert.Equal("Total Poin Kebahagiaan", charts.Single(c => c.Key == "total_happiness_points").Title);
        Assert.Equal("Pesanan Selesai", charts.Single(c => c.Key == "meal_orders_claimed").Title);
    }

    [Fact]
    public void Charts_KeepMissingAndInapplicableDataSeparateFromZero()
    {
        var first = Session("PEMULA");
        var second = Session("MAHIR");
        var third = Session("MAHIR", "CREATED");
        var beginner = Gameplay(first) with { RawJson = JsonSerializer.SerializeToElement(new { financial_goals = new { sharia_loans_taken = 0 } }) };
        var advanced = Gameplay(second) with { RawJson = beginner.RawJson };
        var charts = PlayerStatisticsChartBuilder.Build([new(second, advanced), new(third, null)], T);
        var debt = charts.Single(c => c.Key == "sharia_loans_taken");
        Assert.Equal(new double?[] { 0, null }, debt.Points.Select(p => p.Value));
        using var json = JsonDocument.Parse(debt.Json);
        Assert.Equal("[0,null]", json.RootElement.GetProperty("series")[0].GetProperty("values").GetRawText());
        Assert.All(charts.Single(c => c.Key == "meal_order_profit_margin_percent").Points, p => Assert.Null(p.Value));
        Assert.DoesNotContain(PlayerStatisticsChartBuilder.Build([new(first, beginner)], T), c => c.Group == "future");
    }

    [Fact]
    public async Task Controller_UsesSignedInIdentityAndPreservesSessionsWhenAnalyticsFail()
    {
        var good = Session("MAHIR");
        var wrongOwner = Session("MAHIR");
        var failed = Session("MAHIR");
        var preparing = Session("PEMULA", "CREATED");
        var handler = new ResponseHandler(path => path == "/api/v1/sessions"
            ? Json(new SessionListResponse([preparing, good, wrongOwner, failed]))
            : Json(new PlayerGameplayHistoryResponse([
                new(good.SessionId, Gameplay(good)), new(wrongOwner.SessionId, Gameplay(wrongOwner) with { UserId = Guid.NewGuid() }),
                new(failed.SessionId, null)])));
        var result = Assert.IsType<ViewResult>(await Controller(handler).Index("MAHIR", null, TestContext.Current.CancellationToken));
        var model = Assert.IsType<PlayerStatisticsViewModel>(result.Model);
        Assert.Equal(4, model.TotalSessions);
        Assert.Equal(3, model.Sessions.Count);
        Assert.Equal("MAHIR", model.Mode);
        Assert.Single(model.Sessions, s => s.Gameplay is not null);
        Assert.Equal(Player, model.PlayerId);
        Assert.Equal(T("statistics.error.partial"), model.ErrorMessage);
        var analyticsRequests = handler.Paths.Where(p => p.Contains("/analytics/")).ToList();
        Assert.Single(analyticsRequests);
        Assert.EndsWith($"/players/{Player}/gameplay?mode=MAHIR&status=ALL", analyticsRequests[0]);
        Assert.DoesNotContain(analyticsRequests, p => p.Contains(preparing.SessionId.ToString()));
    }

    [Fact]
    public async Task Controller_FiltersBeforeFetchingAndOrdersSessionsChronologically()
    {
        var first = Session("MAHIR") with { CreatedAt = Date, StartedAt = Date };
        var last = Session("MAHIR") with { CreatedAt = Date.AddDays(2), StartedAt = Date.AddDays(2) };
        var otherMode = Session("PEMULA");
        var ongoing = Session("MAHIR", "STARTED");
        var handler = new ResponseHandler(path => path == "/api/v1/sessions"
            ? Json(new SessionListResponse([last, ongoing, otherMode, first]))
            : Json(new PlayerGameplayHistoryResponse([new(first.SessionId, Gameplay(first)), new(last.SessionId, Gameplay(last))])));
        var view = Assert.IsType<ViewResult>(await Controller(handler).Index("mahir", "ended", TestContext.Current.CancellationToken));
        var model = Assert.IsType<PlayerStatisticsViewModel>(view.Model);
        Assert.Equal(4, model.TotalSessions);
        Assert.Equal(new[] { first.SessionId, last.SessionId }, model.Sessions.Select(s => s.Session.SessionId));
        Assert.Equal(2, handler.Paths.Count);
        Assert.Contains(handler.Paths, path => path.EndsWith("?mode=MAHIR&status=ENDED", StringComparison.Ordinal));
        Assert.Null(model.ErrorMessage);
    }

    [Fact]
    public void Charts_RejectCombinedModes()
    {
        Assert.Throws<ArgumentException>(() => PlayerStatisticsChartBuilder.Build([new(Session("PEMULA"), null), new(Session("MAHIR"), null)], T));
    }

    [Fact]
    public async Task Player_CannotRequestSomeoneElse()
    {
        var handler = new ResponseHandler(_ => throw new InvalidOperationException("No API call expected"));
        var result = Assert.IsType<StatusCodeResult>(await Controller(handler).Index(null, null, TestContext.Current.CancellationToken, Guid.NewGuid()));
        Assert.Equal(403, result.StatusCode);
        Assert.Empty(handler.Paths);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("ALL")]
    [InlineData("invalid")]
    public async Task InvalidOrMissingMode_ChoosesLatestSingleMode(string? mode)
    {
        var first = Session("PEMULA");
        var last = Session("MAHIR") with { StartedAt = Date.AddDays(2) };
        var handler = new ResponseHandler(path => path == "/api/v1/sessions" ? Json(new SessionListResponse([first, last])) : Json(new PlayerGameplayHistoryResponse([new(last.SessionId, Gameplay(last))])));
        var result = Assert.IsType<ViewResult>(await Controller(handler).Index(mode, null, TestContext.Current.CancellationToken));
        var model = Assert.IsType<PlayerStatisticsViewModel>(result.Model);
        Assert.Equal("MAHIR", model.Mode);
        Assert.Equal(last.SessionId, Assert.Single(model.Sessions).Session.SessionId);
        Assert.DoesNotContain(handler.Paths, path => path.Contains(first.SessionId.ToString()));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Instructor_OnlyLoadsSelectedParticipantFromOwnedSessions(bool foreignPlayer)
    {
        var shared = Session("MAHIR");
        var notJoined = Session("MAHIR");
        var other = Guid.NewGuid();
        var handler = new ResponseHandler(path => path == "/api/v1/sessions" ? Json(new SessionListResponse([shared, notJoined]))
            : path.Contains("/session-rosters") ? Json(new SessionRostersResponse([
                new(shared.SessionId, [new(Player, "Selected Player", 1, null, null)], false),
                new(notJoined.SessionId, [new(other, "Other Participant", 1, null, null)], false)]))
            : Json(new PlayerGameplayHistoryResponse([new(shared.SessionId, Gameplay(shared))])));
        var result = await Controller(handler, "INSTRUCTOR").Index("MAHIR", null, TestContext.Current.CancellationToken, foreignPlayer ? Guid.NewGuid() : Player);
        if (foreignPlayer)
        {
            Assert.IsType<NotFoundResult>(result);
            Assert.DoesNotContain(handler.Paths, p => p.Contains("/gameplay"));
        }
        else
        {
            var model = Assert.IsType<PlayerStatisticsViewModel>(Assert.IsType<ViewResult>(result).Model);
            Assert.Equal(2, model.AvailablePlayers.Count);
            Assert.Equal(1, model.TotalSessions);
            Assert.Equal(shared.SessionId, Assert.Single(model.Sessions).Session.SessionId);
            Assert.Equal($"/api/v1/analytics/players/{Player}/gameplay?mode=MAHIR&status=ALL", Assert.Single(handler.Paths, p => p.Contains("/gameplay")));
        }
    }

    [Fact]
    public async Task Roster_ResultFailureDoesNotRemoveParticipantsOrEmptySessions()
    {
        var ended = Session("MAHIR");
        var empty = Session("PEMULA", "CREATED");
        var handler = new ResponseHandler(_ => Json(new SessionRostersResponse([
            new(ended.SessionId, [new(Player, "Known Player", 1, null, null)], false),
            new(empty.SessionId, [], false)])));
        var groups = await SessionRosterLoader.LoadAsync(new ClientFactory(handler).CreateClient("Api"), [ended, empty], true, TestContext.Current.CancellationToken);
        Assert.Equal(2, groups.Count);
        Assert.All(groups, g => Assert.True(g.ParticipantsAvailable));
        Assert.Null(Assert.Single(groups[0].Players).HappinessPointsTotal);
        Assert.False(groups[0].ResultsAvailable);
        Assert.Empty(groups[1].Players);
        Assert.Single(handler.Paths, path => path.Contains("/analytics/"));
    }

    [Theory]
    [InlineData("PLAYER", 2)]
    [InlineData("INSTRUCTOR", 3)]
    public async Task Controller_PreservesAll160SessionsWithConstantHttpRequests(string role, int expectedRequests)
    {
        var sessions = Enumerable.Range(0, 160).Select(i => Session("MAHIR") with
            { StartedAt = Date.AddDays(i), CreatedAt = Date.AddDays(i) }).ToList();
        var handler = new ResponseHandler(path => path == "/api/v1/sessions" ? Json(new SessionListResponse(sessions))
            : path.Contains("/session-rosters") ? Json(new SessionRostersResponse(sessions.Select(s =>
                new SessionRosterResponse(s.SessionId, [new(Player, "Known Player", 1, 1, 36)], true)).ToList()))
            : Json(new PlayerGameplayHistoryResponse(sessions.Select(s => new SessionGameplayItem(s.SessionId, Gameplay(s))).ToList())));
        var result = Assert.IsType<ViewResult>(await Controller(handler, role).Index("MAHIR", null, TestContext.Current.CancellationToken, Player));
        var model = Assert.IsType<PlayerStatisticsViewModel>(result.Model);
        Assert.Equal(160, model.Sessions.Count);
        Assert.Equal(160, model.TotalSessions);
        Assert.All(model.Sessions, s => Assert.NotNull(s.Gameplay));
        Assert.All(model.Charts, chart => Assert.Equal(160, chart.Points.Count));
        Assert.Null(model.ErrorMessage);
        Assert.Equal(expectedRequests, handler.Paths.Count);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task BatchFailure_PreservesSessionRowsAsMissing(bool networkFailure)
    {
        var sessions = new[] { Session("MAHIR"), Session("MAHIR", "CREATED") };
        var handler = new ResponseHandler(path => path == "/api/v1/sessions" ? Json(new SessionListResponse(sessions.ToList()))
            : networkFailure ? throw new HttpRequestException("offline") : new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
        var result = Assert.IsType<ViewResult>(await Controller(handler).Index("MAHIR", null, TestContext.Current.CancellationToken));
        var model = Assert.IsType<PlayerStatisticsViewModel>(result.Model);
        Assert.Equal(2, model.Sessions.Count);
        Assert.All(model.Sessions, s => Assert.Null(s.Gameplay));
        Assert.Equal(T("statistics.error.partial"), model.ErrorMessage);
    }

    [Fact]
    public async Task SessionList_Preserves160RostersInOneBatchRequest()
    {
        var sessions = Enumerable.Range(0, 160).Select(_ => Session("MAHIR")).ToList();
        var handler = new ResponseHandler(_ => Json(new SessionRostersResponse(sessions.Select(s =>
            new SessionRosterResponse(s.SessionId, [new(Player, "Known Player", 1, 1, 36)], true)).ToList())));
        var groups = await SessionRosterLoader.LoadAsync(new ClientFactory(handler).CreateClient("Api"), sessions, true, TestContext.Current.CancellationToken);
        Assert.Equal(160, groups.Count);
        Assert.All(groups, g => { Assert.True(g.ParticipantsAvailable); Assert.True(g.ResultsAvailable); Assert.Single(g.Players); });
        Assert.Single(handler.Paths);
    }

    private static SessionListItem Session(string mode, string status = "ENDED") =>
        new(Guid.NewGuid(), "Sesi uji", mode, status, Date, Date, Date.AddDays(1));
    private static GameplayMetricsResponse Gameplay(SessionListItem session) =>
        new(session.SessionId, Player, Date.AddDays(1), new(10, 121, 119, 2, 6), new(1, 5, 2, 32),
            new(36, 1, 0, 5, 5, 0, 35, -10, 0, false), new(0));
    private static HttpResponseMessage Json(object value) => new(HttpStatusCode.OK) { Content = JsonContent.Create(value) };
    private static PlayerStatisticsController Controller(ResponseHandler handler, string role = "PLAYER") => new(new ClientFactory(handler))
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Session = new MemorySession(),
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, Player.ToString()), new Claim(ClaimTypes.Role, role)], "test"))
            }
        }
    };
    private sealed class ClientFactory(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(handler) { BaseAddress = new Uri("http://localhost") };
    }
    private sealed class ResponseHandler(Func<string, HttpResponseMessage> respond) : HttpMessageHandler
    {
        public ConcurrentBag<string> Paths { get; } = [];
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
        public string Id => "statistics-test";
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => values.Keys;
        public void Clear() => values.Clear();
        public Task CommitAsync(CancellationToken ct = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken ct = default) => Task.CompletedTask;
        public void Remove(string key) => values.Remove(key);
        public void Set(string key, byte[] value) => values[key] = value;
        public bool TryGetValue(string key, out byte[] value) => values.TryGetValue(key, out value!);
    }
}
