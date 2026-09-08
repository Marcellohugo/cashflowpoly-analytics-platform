// Fungsi file: Memverifikasi tampilan target finansial dan pengelompokan aksi pada halaman analitika pemain.
using System.Net;
using System.Text.Json;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Controllers;
using Cashflowpoly.Ui.Infrastructure;
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

public sealed class PlayerFinancialGoalViewTests
{
    [Theory]
    [InlineData("id", 1, 35, 0)]
    [InlineData("id", 1, 35, 5)]
    [InlineData("id", 0, 0, 15)]
    [InlineData("id", 0, 0, 0)]
    [InlineData("en", 1, 35, 5)]
    public async Task Details_ShowsPurchasedGoalsAndKeepsUnfinishedSavingsSeparate(
        string language, int count, int cost, int savings)
    {
        var html = await RenderAsync(language, "advanced", count, cost, savings);
        var cardStart = html.IndexOf("<article class=\"player-analysis-card player-analysis-card--goal-ambition\"", StringComparison.Ordinal);
        Assert.True(cardStart >= 0);
        var card = html[cardStart..html.IndexOf("</article>", cardStart, StringComparison.Ordinal)];
        Assert.Contains(UiText.Translate(language, "players.raw.financial_goals_completed"), card);
        Assert.Contains($"<strong>{count}<small>{UiText.Translate(language, "players.support.unit.goals")}</small></strong>", card);
        Assert.Contains(string.Format(UiText.Translate(language, "players.analysis.goal_purchases.result"), cost), card);
        Assert.Contains(UiText.Translate(language, "players.raw.financial_goals_incomplete_coins_wasted"), card);
        Assert.Contains($"<strong>{savings}</strong>", card);
        Assert.DoesNotContain("%", card);
        Assert.DoesNotContain(UiText.Translate(language, "players.metric.financial_goal_progress_percent"), card);
    }

    [Fact]
    public async Task Details_BeginnerModeOmitsGoalPurchases()
    {
        var html = await RenderAsync("id", "beginner", 1, 35, 0);
        Assert.DoesNotContain("player-analysis-card--goal-ambition", html);
    }

    [Theory]
    [InlineData("id")]
    [InlineData("en")]
    public async Task Details_GroupsSavingsAndGoalActionsInsteadOfShowingZeroBesideAPurchasedGoal(string language)
    {
        var derived = JsonSerializer.SerializeToElement(new
        {
            long_term_action_share_percent = 18.75,
            long_term_action_share_components = new
            {
                saving_actions = 4,
                financial_goal_actions = 0,
                saving_and_goal_actions = 4,
                insurance_actions = 1,
                loan_repayment_actions = 1,
                total_main_actions = 32
            }
        });
        var html = await RenderAsync(language, "advanced", 1, 35, 0, derived);
        var cardStart = html.IndexOf("<article class=\"player-analysis-card player-analysis-card--planning-horizon\"", StringComparison.Ordinal);
        Assert.True(cardStart >= 0);
        var card = html[cardStart..html.IndexOf("</article>", cardStart, StringComparison.Ordinal)];
        Assert.Contains($"<dt>{UiText.Translate(language, "players.metric.saving_and_goal_actions")}</dt>", card);
        Assert.Contains("<strong>4</strong>", card);
        Assert.Contains("(4 + 1 + 1) ÷ 32 × 100%", card);
        Assert.DoesNotContain($"<dt>{UiText.Translate(language, "players.metric.financial_goal_actions")}</dt>", card);
        Assert.DoesNotContain($"<dt>{UiText.Translate(language, "players.metric.saving_actions")}</dt>", card);
        Assert.DoesNotContain("<strong>0</strong>", card);
        Assert.Contains(UiText.Translate(language, "players.raw.financial_goals_completed"), html);
        Assert.Contains(string.Format(UiText.Translate(language, "players.analysis.goal_purchases.result"), 35), html);
    }

    [Theory]
    [InlineData("id", 1, 10, true)]
    [InlineData("en", 1, 10, true)]
    [InlineData("id", 1, 0, false)]
    [InlineData("id", 0, 10, false)]
    public async Task Details_ExplainsWhyPurchasedGoalPointsDoNotCountWhileALoanIsUnpaid(
        string language, int count, int outstandingLoan, bool showNote)
    {
        var html = await RenderAsync(language, "advanced", count, count * 35, 0, outstandingLoan: outstandingLoan);
        var note = UiText.Translate(language, "players.analysis.goal_purchases.unpaid_loan");
        Assert.Equal(showNote, html.Contains(note, StringComparison.Ordinal));
        Assert.Contains(string.Format(UiText.Translate(language, "players.analysis.goal_purchases.result"), count * 35), html);
    }

    private static async Task<string> RenderAsync(string language, string mode, int count, int cost, int savings,
        JsonElement? derived = null, int outstandingLoan = 0)
    {
        var model = new PlayerDetailViewModel
        {
            SessionId = Guid.NewGuid(), PlayerId = Guid.NewGuid(), PlayerDisplayName = "Marco",
            GameplayRaw = JsonSerializer.SerializeToElement(new
            {
                metadata = new { game_mode = mode },
                financial_goals = new
                {
                    financial_goals_completed = count,
                    financial_goals_purchase_cost_total = cost,
                    financial_goals_incomplete_coins_wasted = savings,
                    sharia_loans_outstanding_coins = outstandingLoan
                }
            }),
            GameplayDerived = derived ?? JsonSerializer.SerializeToElement(new { financial_goal_progress_percent = 100 })
        };
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(PlayersController).Assembly.GetName().Name
        });
        builder.Services.AddControllersWithViews();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession();
        await using var app = builder.Build();
        app.MapDefaultControllerRoute();
        using var scope = app.Services.CreateScope();
        var context = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
        context.SetEndpoint(new Endpoint(null, new EndpointMetadataCollection(), "Player details test"));
        context.Session = scope.ServiceProvider.GetRequiredService<ISessionStore>()
            .Create(Guid.NewGuid().ToString(), TimeSpan.FromMinutes(20), TimeSpan.FromSeconds(10), () => true, true);
        context.Session.SetString(AuthConstants.SessionLanguageKey, language);
        var action = new ActionContext(context, new RouteData(), new ActionDescriptor());
        var view = scope.ServiceProvider.GetRequiredService<IRazorViewEngine>()
            .GetView(null, "/Views/Players/Details.cshtml", isMainPage: false);
        Assert.True(view.Success, string.Join(Environment.NewLine, view.SearchedLocations ?? []));
        using var writer = new StringWriter();
        await view.View.RenderAsync(new ViewContext(action, view.View,
            new ViewDataDictionary<PlayerDetailViewModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model },
            new TempDataDictionary(context, scope.ServiceProvider.GetRequiredService<ITempDataProvider>()),
            writer, new HtmlHelperOptions()));
        return WebUtility.HtmlDecode(writer.ToString());
    }
}
