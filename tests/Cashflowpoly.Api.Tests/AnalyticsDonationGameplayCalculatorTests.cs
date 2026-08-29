// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsDonationGameplayCalculatorTests.
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
            CreateEvent(playerId, "JumatBerkah", """{"amount":10}""", dayIndex: 5, weekday: "FRI"),
            CreateEvent(playerId, "JumatBerkah", """{"amount":20}""", dayIndex: 5, weekday: "FRI"),
            CreateEvent(playerId, "JumatBerkah", """{"amount":5}""", dayIndex: 12, weekday: "FRI"),
            CreateEvent(playerId, "PoinPeringkatDonasi", """{"rank":1,"points":15}""", dayIndex: 5, weekday: "FRI"),
            CreateEvent(playerId, "PoinPeringkatDonasi", """{"rank":2,"points":10}""", dayIndex: 12, weekday: "FRI")
        };
        var allEvents = playerEvents.Concat(new[]
        {
            CreateEvent(Guid.NewGuid(), "AkhirGiliran", "{}", dayIndex: 19, weekday: "FRI")
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
        Assert.Equal(87.5, metrics.DonationStability!.Value, precision: 12);
        Assert.Equal(28.57142857142857, metrics.DonationStabilityIndex!.Value, precision: 12);
        Assert.Equal(0.35, metrics.DonationRatio);
        Assert.Equal(35, metrics.DonationAggressivenessPercent);
        Assert.Equal(2d / 3d, metrics.FridayParticipationRate);
        Assert.Equal(6.666666666666666, metrics.DonationCommitmentScore!.Value, precision: 12);
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

    [Fact]
    public void Compute_RanksEveryFridayUsingDonationThenHighestTieBreaker()
    {
        var playerId = Guid.NewGuid();
        var secondPlayerId = Guid.NewGuid();
        var thirdPlayerId = Guid.NewGuid();
        var playerEvents = new[]
        {
            CreateEvent(playerId, "JumatBerkah", """{"amount":1}""", dayIndex: 5, weekday: "FRI"),
            CreateEvent(playerId, "JumatBerkah", """{"amount":4}""", dayIndex: 12, weekday: "FRI")
        };
        var allEvents = playerEvents.Concat(new[]
        {
            CreateEvent(secondPlayerId, "JumatBerkah", """{"amount":3}""", dayIndex: 5, weekday: "FRI"),
            CreateEvent(thirdPlayerId, "JumatBerkah", """{"amount":3}""", dayIndex: 5, weekday: "FRI"),
            CreateEvent(secondPlayerId, "JumatBerkah", """{"amount":4}""", dayIndex: 12, weekday: "FRI"),
            CreateEvent(playerId, "BagikanTieBreaker", """{"number":4}""", dayIndex: 0, weekday: "SUN"),
            CreateEvent(secondPlayerId, "BagikanTieBreaker", """{"number":2}""", dayIndex: 0, weekday: "SUN"),
            CreateEvent(thirdPlayerId, "BagikanTieBreaker", """{"number":3}""", dayIndex: 0, weekday: "SUN")
        }).ToList();

        var metrics = new DonationGameplayCalculator().Compute(playerEvents, allEvents, coinsNetEndGame: 20);

        Assert.Collection(metrics.DonationRankPerFriday,
            item =>
            {
                Assert.Equal(5, item.DayIndex);
                Assert.Equal(3, item.Rank);
            },
            item =>
            {
                Assert.Equal(12, item.DayIndex);
                Assert.Equal(1, item.Rank);
            });
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
            UserId = playerId,
            ActorType = "PLAYER",
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            DayIndex = dayIndex,
            Weekday = weekday,
            ActionSlot = dayIndex,
            SequenceNumber = dayIndex,
            ActionType = actionType,
            RulesetVersionId = Guid.NewGuid(),
            Payload = payload
        };
    }
}
