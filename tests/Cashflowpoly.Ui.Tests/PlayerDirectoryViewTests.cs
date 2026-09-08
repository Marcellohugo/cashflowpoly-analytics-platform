// Fungsi file: Menguji HTML direktori agar pemain tanpa sesi tetap terlihat bersama kelompok sesi.
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

public sealed class PlayerDirectoryViewTests
{
    [Theory]
    [InlineData("CREATED")]
    [InlineData("STARTED")]
    [InlineData("ENDED")]
    public async Task Index_ShowsPlayersWithoutSessionsAlongsideSessionGroups(string status)
    {
        var joined = new PlayerResponse(Guid.NewGuid(), "Joined Player");
        var unjoined = new PlayerResponse(Guid.NewGuid(), "Unjoined Player");
        var model = new PlayerDirectoryViewModel
        {
            Players = [joined, unjoined],
            SessionGroups =
            [
                new PlayerSessionGroupViewModel
                {
                    SessionId = Guid.NewGuid(), SessionName = "Existing Session", Status = status,
                    Players = [new PlayerSessionEntryViewModel { PlayerId = joined.UserId, DisplayName = joined.DisplayName }]
                }
            ]
        };

        var html = await RenderAsync(model);

        Assert.Contains("Players Without a Session", html);
        Assert.Contains(unjoined.DisplayName, html);
        Assert.Contains("Existing Session", html);
        var ungroupedSection = html[html.IndexOf("Players Without a Session", StringComparison.Ordinal)..
            html.IndexOf("Players by Session", StringComparison.Ordinal)];
        Assert.DoesNotContain(joined.DisplayName, ungroupedSection);
        Assert.DoesNotContain("View Analytics", ungroupedSection);
        Assert.Equal(1, html.Split(unjoined.DisplayName, StringSplitOptions.None).Length - 1);
    }

    [Fact]
    public async Task Index_ShowsPlayersAndEmptyStateWhenThereAreNoSessions()
    {
        var html = await RenderAsync(new PlayerDirectoryViewModel
        {
            Players = [new PlayerResponse(Guid.NewGuid(), "New Player")]
        });
        Assert.Contains("New Player", html);
        Assert.DoesNotContain("Players by Session", html);

        var emptyHtml = await RenderAsync(new PlayerDirectoryViewModel());
        Assert.Contains("Players Without a Session", emptyHtml);
        Assert.Contains("mobile-card-empty", emptyHtml);
    }

    [Fact]
    public async Task Index_DoesNotShowAnEmptyUnjoinedListWhenAllPlayersHaveSessions()
    {
        var player = new PlayerResponse(Guid.NewGuid(), "Joined Player");
        var html = await RenderAsync(new PlayerDirectoryViewModel
        {
            Players = [player],
            SessionGroups =
            [
                new PlayerSessionGroupViewModel
                {
                    SessionName = "Existing Session", Status = "ENDED",
                    Players = [new PlayerSessionEntryViewModel { PlayerId = player.UserId, DisplayName = player.DisplayName }]
                }
            ]
        });

        Assert.Contains(player.DisplayName, html);
        Assert.DoesNotContain("Players Without a Session", html);
        Assert.DoesNotContain("mobile-card-empty", html);
    }

    [Fact]
    public async Task Index_KeepsPlayersVisibleWithoutClaimingTheyHaveNoSessionsWhenGroupingFails()
    {
        var html = await RenderAsync(new PlayerDirectoryViewModel
        {
            Players = [new PlayerResponse(Guid.NewGuid(), "Available Player")],
            ErrorMessage = "Session details unavailable"
        });

        Assert.Contains("Available Player", html);
        Assert.Contains("Session details unavailable", html);
        Assert.Contains("General Player List", html);
        Assert.DoesNotContain("Players Without a Session", html);
    }

    [Theory]
    [InlineData(false, false, "1")]
    [InlineData(true, false, "0")]
    [InlineData(false, true, "—")]
    public async Task InstructorSummary_CountsOnlyDistinctSessionParticipants(bool noSessions, bool failed, string expectedCount)
    {
        var joined = new PlayerResponse(Guid.NewGuid(), "Joined Player");
        var unjoined = new PlayerResponse(Guid.NewGuid(), "General Player");
        var model = new PlayerDirectoryViewModel
        {
            Players = [joined, unjoined],
            ErrorMessage = failed ? "Session details unavailable" : null,
            SessionGroups = noSessions ? [] : new[] { "CREATED", "STARTED", "ENDED" }
                .Select(status => new PlayerSessionGroupViewModel
                {
                    SessionId = Guid.NewGuid(), SessionName = status, Status = status,
                    Players = [new PlayerSessionEntryViewModel { PlayerId = joined.UserId, DisplayName = joined.DisplayName }]
                }).ToList()
        };

        var html = System.Net.WebUtility.HtmlDecode(await RenderAsync(model, instructor: true));
        Assert.Contains("Monitored Players", html);
        Assert.Contains($"<p class=\"ruleset-index-count\">{expectedCount}</p>", html);
        Assert.Contains(unjoined.DisplayName, html);
        Assert.Contains(failed ? "General Player List" : "Players Outside Your Sessions", html);
        if (!failed)
        {
            Assert.Contains("They are not included in Monitored Players", html);
        }
    }

    private static async Task<string> RenderAsync(PlayerDirectoryViewModel model, bool instructor = false)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(PlayerDirectoryController).Assembly.GetName().Name
        });
        builder.Services.AddControllersWithViews();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession();
        await using var app = builder.Build();
        app.MapDefaultControllerRoute();
        using var scope = app.Services.CreateScope();
        var context = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
        context.SetEndpoint(new Endpoint(null, new EndpointMetadataCollection(), "Player directory test"));
        if (instructor)
        {
            context.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Role, "INSTRUCTOR")], "test"));
        }
        context.Session = scope.ServiceProvider.GetRequiredService<ISessionStore>()
            .Create(Guid.NewGuid().ToString(), TimeSpan.FromMinutes(20), TimeSpan.FromSeconds(10), () => true, true);
        context.Session.SetString(AuthConstants.SessionLanguageKey, AuthConstants.LanguageEn);
        var action = new ActionContext(context, new RouteData(), new ActionDescriptor());
        var view = scope.ServiceProvider.GetRequiredService<IRazorViewEngine>()
            .GetView(null, "/Views/Players/Index.cshtml", isMainPage: false);
        Assert.True(view.Success, string.Join(Environment.NewLine, view.SearchedLocations ?? []));
        using var writer = new StringWriter();
        await view.View.RenderAsync(new ViewContext(action, view.View,
            new ViewDataDictionary<PlayerDirectoryViewModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model },
            new TempDataDictionary(context, scope.ServiceProvider.GetRequiredService<ITempDataProvider>()),
            writer, new HtmlHelperOptions()));
        return writer.ToString();
    }
}
