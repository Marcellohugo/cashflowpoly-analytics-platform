// Fungsi file: Mengurutkan item analitik pemain berdasarkan konfigurasi ruleset.
using System.Globalization;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Helper pengurutan pemain analitik untuk menjaga controller tetap tipis.
/// </summary>
internal static class AnalyticsPlayerOrdering
{
    private static readonly StringComparer UsernameOrderingComparer = StringComparer.Create(new CultureInfo("id-ID"), true);

    /// <summary>
    /// Mengurutkan daftar pemain sesuai konfigurasi PlayerOrdering pada ruleset.
    /// </summary>
    internal static List<AnalyticsByPlayerItem> OrderPlayers(
        List<AnalyticsByPlayerItem> players,
        PlayerOrdering ordering,
        Dictionary<Guid, int> playerJoinOrders,
        Dictionary<Guid, long> firstEventSequenceByPlayer,
        Dictionary<Guid, string> usernamesByPlayer)
    {
        return ordering switch
        {
            PlayerOrdering.InstructorOrder => players
                .OrderBy(player => ResolveJoinOrder(playerJoinOrders, player.PlayerId))
                .ThenBy(player => ResolveFirstSequence(firstEventSequenceByPlayer, player.PlayerId))
                .ThenBy(player => player.PlayerId)
                .ToList(),
            PlayerOrdering.Username => players
                .OrderBy(player => HasOrderingUsername(usernamesByPlayer, player.PlayerId) ? 0 : 1)
                .ThenBy(player => ResolveOrderingUsername(usernamesByPlayer, player.PlayerId), UsernameOrderingComparer)
                .ThenBy(player => ResolveJoinOrder(playerJoinOrders, player.PlayerId))
                .ThenBy(player => ResolveFirstSequence(firstEventSequenceByPlayer, player.PlayerId))
                .ThenBy(player => player.PlayerId)
                .ToList(),
            PlayerOrdering.EventSequence => players
                .OrderBy(player => ResolveFirstSequence(firstEventSequenceByPlayer, player.PlayerId))
                .ThenBy(player => ResolveJoinOrder(playerJoinOrders, player.PlayerId))
                .ThenBy(player => player.PlayerId)
                .ToList(),
            PlayerOrdering.PlayerId => players
                .OrderBy(player => player.PlayerId)
                .ToList(),
            _ => players
                .OrderBy(player => ResolveJoinOrder(playerJoinOrders, player.PlayerId))
                .ThenBy(player => ResolveFirstSequence(firstEventSequenceByPlayer, player.PlayerId))
                .ThenBy(player => player.PlayerId)
                .ToList()
        };
    }

    private static int ResolveJoinOrder(Dictionary<Guid, int> playerJoinOrders, Guid playerId)
    {
        return playerJoinOrders.TryGetValue(playerId, out var joinOrder) ? joinOrder : int.MaxValue;
    }

    private static long ResolveFirstSequence(Dictionary<Guid, long> firstEventSequenceByPlayer, Guid playerId)
    {
        return firstEventSequenceByPlayer.TryGetValue(playerId, out var firstSeq) ? firstSeq : long.MaxValue;
    }

    private static bool HasOrderingUsername(Dictionary<Guid, string> usernamesByPlayer, Guid playerId)
    {
        return usernamesByPlayer.TryGetValue(playerId, out var username) && !string.IsNullOrWhiteSpace(username);
    }

    private static string ResolveOrderingUsername(Dictionary<Guid, string> usernamesByPlayer, Guid playerId)
    {
        return usernamesByPlayer.TryGetValue(playerId, out var username) && !string.IsNullOrWhiteSpace(username)
            ? username.Trim()
            : string.Empty;
    }
}
