// Fungsi file: Menguji kalkulasi metrik donasi gameplay dari histori event.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsDonationGameplayCalculatorTests
{
    [Fact]
    public void Compute_GroupsDonationAmountAndRankByFriday()
    {
        var playerId = Guid.NewGuid();
        var playerEvents = new List<EventDb>
        {
            CreateEvent(playerId, "day.friday.donation", """{"amount":10}""", dayIndex: 5, weekday: "FRI"),
            CreateEvent(playerId, "day.friday.donation", """{"amount":20}""", dayIndex: 5, weekday: "FRI"),
            CreateEvent(playerId, "day.friday.donation", """{"amount":5}""", dayIndex: 12, weekday: "FRI"),
            CreateEvent(playerId, "donation.rank.awarded", """{"rank":1,"points":15}""", dayIndex: 5, weekday: "FRI"),
            CreateEvent(playerId, "donation.rank.awarded", """{"rank":2,"points":10}""", dayIndex: 12, weekday: "FRI")
        };
        var allEvents = playerEvents.Concat(new[]
        {
            CreateEvent(Guid.NewGuid(), "turn.ended", "{}", dayIndex: 19, weekday: "FRI")
        }).ToList();

        var metrics = new DonationGameplayCalculator().Compute(playerEvents, allEvents, coinsNetEndGame: 100);

        Assert.Equal(35, metrics.DonationTotalCoins);
        Assert.Equal(2, metrics.DonationChampionCardsEarned);
        Assert.Collection(metrics.DonationAmountPerFriday,
            item =>
            {
                Assert.Equal(5, item.DayIndex);
                Assert.Equal(30, item.Amount);
            },
            item =>
            {
                Assert.Equal(12, item.DayIndex);
                Assert.Equal(5, item.Amount);
            });
        Assert.Collection(metrics.DonationRankPerFriday,
            item =>
            {
                Assert.Equal(5, item.DayIndex);
                Assert.Equal(1, item.Rank);
            },
            item =>
            {
                Assert.Equal(12, item.DayIndex);
                Assert.Equal(2, item.Rank);
            });
        Assert.Equal(12.5, metrics.DonationStabilityStdDeviation);
        Assert.Equal(87.5, metrics.DonationStability);
        Assert.Equal(0.35, metrics.DonationRatio);
        Assert.Equal(35, metrics.DonationAggressivenessPercent);
        Assert.Equal(2d / 3d, metrics.FridayParticipationRate);
        Assert.Equal(20.416666666666664, metrics.DonationCommitmentScore);
    }

    [Fact]
    public void Compute_ReturnsNullDerivedMetricsWhenNoDonationAndNoFriday()
    {
        var metrics = new DonationGameplayCalculator().Compute(
            Array.Empty<EventDb>(),
            Array.Empty<EventDb>(),
            coinsNetEndGame: 0);

        Assert.Empty(metrics.DonationAmountPerFriday);
        Assert.Empty(metrics.DonationRankPerFriday);
        Assert.Equal(0, metrics.DonationTotalCoins);
        Assert.Null(metrics.DonationStabilityStdDeviation);
        Assert.Null(metrics.DonationRatio);
        Assert.Null(metrics.DonationAggressivenessPercent);
        Assert.Null(metrics.FridayParticipationRate);
        Assert.Null(metrics.DonationCommitmentScore);
    }

    private static EventDb CreateEvent(
        Guid playerId,
        string actionType,
        string payload,
        int dayIndex,
        string weekday)
    {
        return new EventDb
        {
            EventId = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            PlayerId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = dayIndex,
            Weekday = weekday,
            TurnNumber = dayIndex,
            SequenceNumber = dayIndex,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = payload
        };
    }
}
