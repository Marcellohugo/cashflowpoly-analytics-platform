using System.Text.Json;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventSimpleActionValidatorTests
{
    [Fact]
    public void TryValidate_ReturnsFalseForUnhandledAction()
    {
        var request = CreateRequest("transaction.recorded", """{"direction":"IN","amount":1,"category":"PAYCHECK"}""");

        var handled = new EventSimpleActionValidator().TryValidate(request, CreateConfig(), out var result);

        Assert.False(handled);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void TryValidateOrderPassed_RequiresPlayer()
    {
        var request = CreateRequest("order.passed", """{"required_ingredient_card_ids":["A"],"income":5}""") with
        {
            PlayerId = null
        };

        var handled = new EventSimpleActionValidator().TryValidate(request, CreateConfig(), out var result);

        Assert.True(handled);
        Assert.False(result.IsValid);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("VALIDATION_ERROR", result.ErrorCode);
        Assert.Contains(result.Details, detail => detail.Field == "player_id" && detail.Issue == "REQUIRED");
    }

    [Fact]
    public void TryValidateFreelance_RejectsIncomeOutsideRuleset()
    {
        var request = CreateRequest("work.freelance.completed", """{"amount":7}""");

        var handled = new EventSimpleActionValidator().TryValidate(request, CreateConfig(freelanceIncome: 5), out var result);

        Assert.True(handled);
        Assert.False(result.IsValid);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);
        Assert.Equal("DOMAIN_RULE_VIOLATION", result.ErrorCode);
        Assert.Equal("Amount kerja lepas tidak sesuai ruleset", result.Message);
    }

    [Fact]
    public void TryValidateGoldPoints_RejectsNegativePoints()
    {
        var request = CreateRequest("gold.points.awarded", """{"points":-1}""");

        var handled = new EventSimpleActionValidator().TryValidate(request, CreateConfig(), out var result);

        Assert.True(handled);
        Assert.False(result.IsValid);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Contains(result.Details, detail => detail.Field == "payload.points" && detail.Issue == "OUT_OF_RANGE");
    }

    [Fact]
    public void TryValidatePensionRank_AcceptsValidPayload()
    {
        var request = CreateRequest("pension.rank.awarded", """{"rank":2,"points":10}""");

        var handled = new EventSimpleActionValidator().TryValidate(request, CreateConfig(), out var result);

        Assert.True(handled);
        Assert.True(result.IsValid);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    private static EventRequest CreateRequest(string actionType, string payloadJson)
    {
        using var document = JsonDocument.Parse(payloadJson);
        return new EventRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "PLAYER",
            new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            0,
            "MON",
            1,
            0,
            actionType,
            Guid.NewGuid(),
            document.RootElement.Clone(),
            "client-123");
    }

    private static RulesetConfig CreateConfig(int freelanceIncome = 5)
    {
        return new RulesetConfig(
            "PEMULA",
            ActionsPerTurn: 3,
            StartingCash: 20,
            PlayerOrdering.JoinOrder,
            CashMin: 0,
            MaxIngredientTotal: 10,
            MaxSameIngredient: 5,
            PrimaryNeedMaxPerDay: 1,
            RequirePrimaryBeforeOthers: true,
            FridayEnabled: true,
            SaturdayEnabled: true,
            SundayEnabled: true,
            DonationMin: 1,
            DonationMax: 10,
            GoldAllowBuy: true,
            GoldAllowSell: true,
            LoanEnabled: false,
            InsuranceEnabled: false,
            SavingGoalEnabled: false,
            freelanceIncome,
            Scoring: null);
    }
}
