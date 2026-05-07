// Fungsi file: Menguji pengurutan pemain analitik berdasarkan konfigurasi ruleset.
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Contracts;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class AnalyticsPlayerOrderingTests
{
    [Fact]
    public void OrderPlayers_UsesJoinOrderByDefaultThenEventSequence()
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
            PlayerOrdering.JoinOrder,
            new Dictionary<Guid, int> { [first] = 2, [second] = 1 },
            new Dictionary<Guid, long> { [first] = 5, [second] = 9, [third] = 1 },
            new Dictionary<Guid, string>());

        Assert.Equal([second, first, third], ordered.Select(item => item.PlayerId));
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

        Assert.Equal([second, first, missingUsername], ordered.Select(item => item.PlayerId));
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

        Assert.Equal([second, first], ordered.Select(item => item.PlayerId));
    }

    private static AnalyticsByPlayerItem BuildPlayer(Guid playerId)
    {
        return new AnalyticsByPlayerItem(
            playerId,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
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
