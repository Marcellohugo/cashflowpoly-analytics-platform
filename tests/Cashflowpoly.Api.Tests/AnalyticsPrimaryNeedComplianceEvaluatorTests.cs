// Fungsi file: Menguji evaluator compliance kebutuhan primer berdasarkan event pemain dan ruleset.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsPrimaryNeedComplianceEvaluatorTests
{
    [Fact]
    public void Evaluate_FlagsSecondaryPurchaseBeforePrimary()
    {
        var events = new List<EventDb>
        {
            BuildEvent("need.secondary.purchased", dayIndex: 1, sequenceNumber: 1),
            BuildEvent("need.primary.purchased", dayIndex: 1, sequenceNumber: 2),
            BuildEvent("need.primary.purchased", dayIndex: 2, sequenceNumber: 3)
        };

        var result = new PrimaryNeedComplianceEvaluator().Evaluate(events, BuildConfig());

        Assert.Equal(0.5, result.Rate);
        Assert.Equal(2, result.EvaluatedDays);
        Assert.Equal(1, result.CompliantDays);
        Assert.False(result.Details[0].compliant);
        Assert.Contains("BOUGHT_OTHER_BEFORE_PRIMARY", result.Details[0].reason);
        Assert.True(result.Details[1].compliant);
    }

    [Fact]
    public void Evaluate_ReturnsZero_WhenConfigMissing()
    {
        var result = new PrimaryNeedComplianceEvaluator().Evaluate(
            [BuildEvent("need.primary.purchased", dayIndex: 1, sequenceNumber: 1)],
            config: null);

        Assert.Equal(0, result.Rate);
        Assert.Equal(0, result.EvaluatedDays);
        Assert.Empty(result.Details);
    }

    private static EventDb BuildEvent(string actionType, int dayIndex, long sequenceNumber)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            ActionType = actionType,
            DayIndex = dayIndex,
            SequenceNumber = sequenceNumber
        };
    }

    private static RulesetConfig BuildConfig()
    {
        return new RulesetConfig(
            "PEMULA",
            2,
            20,
            PlayerOrdering.JoinOrder,
            0,
            6,
            3,
            1,
            true,
            true,
            true,
            true,
            1,
            999,
            true,
            true,
            false,
            false,
            false,
            1,
            null);
    }
}
