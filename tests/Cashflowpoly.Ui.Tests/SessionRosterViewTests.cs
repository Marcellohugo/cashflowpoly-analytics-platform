// Fungsi file: Menguji direktori pemain dan pembatasan tampilan instruktur pada peserta sesinya.
using Cashflowpoly.Ui.Contracts;
using System.Security.Claims;
using Cashflowpoly.Ui.Controllers;
using Cashflowpoly.Ui.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class SessionRosterViewTests
{
    [Theory]
    [InlineData("CREATED", false)]
    [InlineData("STARTED", false)]
    [InlineData("ENDED", false)]
    [InlineData("ENDED", true)]
    public async Task SessionCards_ShowActualParticipantsAndOnlyPermittedAnalytics(string status, bool instructor)
    {
        var own = Guid.NewGuid();
        var other = Guid.NewGuid();
        var session = new SessionListItem(Guid.NewGuid(), "Shared Session", "MAHIR", status, DateTimeOffset.UtcNow, null, null);
        var model = new SessionListViewModel
        {
            Items = [session], MonitoredPlayers = 2,
            SessionGroups = [new PlayerSessionGroupViewModel
            {
                SessionId = session.SessionId, SessionName = session.SessionName, Mode = session.Mode, Status = status,
                ParticipantsAvailable = true, ResultsAvailable = false,
                Players = [new() { PlayerId = own, DisplayName = "Own Player", PlayerOrder = 1 },
                           new() { PlayerId = other, DisplayName = "Other Player", PlayerOrder = 2 }]
            }]
        };
        var html = await RenderAsync(model, instructor, own);
        Assert.Contains("Own Player", html);
        Assert.Contains("Other Player", html);
        Assert.Contains("players-session-meta", html);
        Assert.Contains("players-session-detail", html);
        Assert.Contains("Monitored Players", html);
        var expectedLinks = status == "CREATED" ? 0 : instructor ? 2 : 1;
        Assert.Equal(expectedLinks, System.Text.RegularExpressions.Regex.Matches(html, "class=\"table-link\"").Count);
        var otherRow = html[html.IndexOf("Other Player", StringComparison.Ordinal)..];
        Assert.Equal(instructor && status != "CREATED", otherRow.Contains("class=\"table-link\"", StringComparison.Ordinal));
        Assert.DoesNotContain("players-session-score\">0", html);
    }

    [Fact]
    public async Task EmptySession_KeepsItsDetailsLinkAndDoesNotInventParticipants()
    {
        var model = new SessionListViewModel { SessionGroups = [new() { SessionName = "Empty Session", Mode = "PEMULA", Status = "CREATED", ParticipantsAvailable = true }] };
        var html = await RenderAsync(model, true);
        Assert.Contains("Empty Session", html);
        Assert.Contains("players-session-detail", html);
        Assert.DoesNotContain("players-session-player", html);
    }

    private static async Task<string> RenderAsync(SessionListViewModel model, bool instructor = false, Guid? playerId = null)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(SessionsController).Assembly.GetName().Name
        });
        builder.Services.AddControllersWithViews();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession();
        await using var app = builder.Build();
        app.MapDefaultControllerRoute();
        using var scope = app.Services.CreateScope();
        var context = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
        context.SetEndpoint(new Endpoint(null, new EndpointMetadataCollection(), "Session roster test"));
        context.User = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.Role, instructor ? "INSTRUCTOR" : "PLAYER"),
            new Claim(ClaimTypes.NameIdentifier, (playerId ?? Guid.NewGuid()).ToString())], "test"));
        context.Session = scope.ServiceProvider.GetRequiredService<ISessionStore>()
            .Create(Guid.NewGuid().ToString(), TimeSpan.FromMinutes(20), TimeSpan.FromSeconds(10), () => true, true);
        context.Session.SetString(AuthConstants.SessionLanguageKey, AuthConstants.LanguageEn);
        var action = new ActionContext(context, new RouteData(), new ActionDescriptor());
        var view = scope.ServiceProvider.GetRequiredService<IRazorViewEngine>()
            .GetView(null, "/Views/Sessions/Index.cshtml", isMainPage: false);
        Assert.True(view.Success, string.Join(Environment.NewLine, view.SearchedLocations ?? []));
        using var writer = new StringWriter();
        await view.View.RenderAsync(new ViewContext(action, view.View,
            new ViewDataDictionary<SessionListViewModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model },
            new TempDataDictionary(context, scope.ServiceProvider.GetRequiredService<ITempDataProvider>()),
            writer, new HtmlHelperOptions()));
        return writer.ToString();
    }
}
