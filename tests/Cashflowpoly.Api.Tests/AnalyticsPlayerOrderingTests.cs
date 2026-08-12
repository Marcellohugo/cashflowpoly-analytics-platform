// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsPlayerOrderingTests.
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Services;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsPlayerOrderingTests
{
    [Fact]
    public void OrderPlayers_UsesPlayerOrderByDefaultThenEventSequence()
    {
        var first = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var second = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var third = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var players = new List<AnalyticsByPlayerItem>
        {
            BuildPlayer(third),
            BuildPlayer(first),
            BuildPlayer(second)
        };

        var ordered = new PlayerOrderingService().OrderPlayers(
            players,
            PlayerOrdering.PlayerOrder,
            new Dictionary<Guid, int> { [first] = 2, [second] = 1 },
            new Dictionary<Guid, long> { [first] = 5, [second] = 9, [third] = 1 },
            new Dictionary<Guid, string>());

        Assert.Equal([second, first, third], ordered.Select(item => item.UserId));
    }

    [Fact]
    public void OrderPlayers_UsesLocalizedUsernameAndTrimsWhitespace()
    {
        var first = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var second = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var missingUsername = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var players = new List<AnalyticsByPlayerItem>
        {
            BuildPlayer(missingUsername),
            BuildPlayer(second),
            BuildPlayer(first)
        };

        var ordered = new PlayerOrderingService().OrderPlayers(
            players,
            PlayerOrdering.Username,
            new Dictionary<Guid, int>(),
            new Dictionary<Guid, long>(),
            new Dictionary<Guid, string>
            {
                [first] = "  Budi ",
                [second] = "Andi"
            });

        Assert.Equal([second, first, missingUsername], ordered.Select(item => item.UserId));
    }

    [Fact]
    public void OrderPlayers_UsesEventSequenceWhenConfigured()
    {
        var first = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var second = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var players = new List<AnalyticsByPlayerItem>
        {
            BuildPlayer(first),
            BuildPlayer(second)
        };

        var ordered = new PlayerOrderingService().OrderPlayers(
            players,
            PlayerOrdering.EventSequence,
            new Dictionary<Guid, int> { [first] = 1, [second] = 2 },
            new Dictionary<Guid, long> { [first] = 20, [second] = 10 },
            new Dictionary<Guid, string>());

        Assert.Equal([second, first], ordered.Select(item => item.UserId));
    }

    [Fact]
    public void BuildFinalLeaderboard_RanksByHappinessThenNetCashflow()
    {
        var marco = Guid.Parse("90000000-0000-0000-0000-000000000011");
        var marcello = Guid.Parse("90000000-0000-0000-0000-000000000012");
        var hugo = Guid.Parse("90000000-0000-0000-0000-000000000013");
        var manalu = Guid.Parse("90000000-0000-0000-0000-000000000014");

        var leaderboard = AnalyticsService.BuildFinalLeaderboard([
            BuildPlayer(marco, 1, 20, 118, 113),
            BuildPlayer(marcello, 2, 8, 135, 106),
            BuildPlayer(hugo, 3, 9, 143, 121),
            BuildPlayer(manalu, 4, 17, 115, 99)
        ]);

        Assert.Equal([marco, manalu, hugo, marcello], leaderboard.Select(item => item.UserId));
        Assert.Equal([1, 2, 3, 4], leaderboard.Select(item => item.Rank));
        Assert.Equal([20d, 17d, 9d, 8d], leaderboard.Select(item => item.HappinessPointsTotal));
    }

    [Fact]
    public void FinalizedSession_UsesPersistedComponentsAndRank()
    {
        var player = Guid.NewGuid();
        var computed = new Dictionary<Guid, AnalyticsHappinessBreakdown>
        {
            [player] = new(10, 1, 1, 1, 1, 1, 5, 0, 0, false)
        };
        var finalScores = new List<SessionFinalScoreDb>
        {
            new()
            {
                UserId = player,
                PlayerOrder = 3,
                Rank = 1,
                TotalPoints = 30,
                NeedPoints = 7,
                DonationPoints = 8,
                GoldPoints = 5,
                PensionPoints = 10
            }
        };

        var authoritative = AnalyticsService.ApplyFinalScores(computed, finalScores);
        var leaderboard = AnalyticsService.BuildFinalLeaderboard([BuildPlayer(player, 3, 10)], finalScores);

        Assert.Equal(30, authoritative[player].Total);
        Assert.Equal(7, authoritative[player].NeedPoints);
        Assert.Equal(1, leaderboard.Single().Rank);
        Assert.Equal(30, leaderboard.Single().HappinessPointsTotal);
    }

    private static AnalyticsByPlayerItem BuildPlayer(
        Guid playerId,
        int playerOrder = 0,
        double happinessPoints = 0,
        double cashIn = 0,
        double cashOut = 0)
    {
        return new AnalyticsByPlayerItem(
            playerId,
            playerOrder,
            cashIn,
            cashOut,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            happinessPoints,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            false);
    }
}
