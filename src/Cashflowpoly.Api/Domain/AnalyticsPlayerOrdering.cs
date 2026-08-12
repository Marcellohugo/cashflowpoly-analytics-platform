// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsPlayerOrdering.
using System.Globalization;
using Cashflowpoly.Api.Contracts;

namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Helper pengurutan pemain analitik untuk menjaga controller tetap tipis.
/// </summary>
internal sealed class PlayerOrderingService : IPlayerOrdering
{
    private static readonly StringComparer UsernameOrderingComparer = StringComparer.Create(new CultureInfo("id-ID"), true);

    /// <summary>
    /// Mengurutkan daftar pemain sesuai konfigurasi PlayerOrdering pada ruleset.
    /// </summary>
    public List<AnalyticsByPlayerItem> OrderPlayers(
        List<AnalyticsByPlayerItem> players,
        PlayerOrdering ordering,
        Dictionary<Guid, int> playerPlayerOrders,
        Dictionary<Guid, long> firstEventSequenceByPlayer,
        Dictionary<Guid, string> usernamesByPlayer)
    {
        return ordering switch
        {
            PlayerOrdering.Username => players
                .OrderBy(player => HasOrderingUsername(usernamesByPlayer, player.UserId) ? 0 : 1)
                .ThenBy(player => ResolveOrderingUsername(usernamesByPlayer, player.UserId), UsernameOrderingComparer)
                .ThenBy(player => ResolvePlayerOrder(playerPlayerOrders, player.UserId))
                .ThenBy(player => ResolveFirstSequence(firstEventSequenceByPlayer, player.UserId))
                .ThenBy(player => player.UserId)
                .ToList(),
            PlayerOrdering.EventSequence => players
                .OrderBy(player => ResolveFirstSequence(firstEventSequenceByPlayer, player.UserId))
                .ThenBy(player => ResolvePlayerOrder(playerPlayerOrders, player.UserId))
                .ThenBy(player => player.UserId)
                .ToList(),
            PlayerOrdering.PlayerId => players
                .OrderBy(player => player.UserId)
                .ToList(),
            _ => players
                .OrderBy(player => ResolvePlayerOrder(playerPlayerOrders, player.UserId))
                .ThenBy(player => ResolveFirstSequence(firstEventSequenceByPlayer, player.UserId))
                .ThenBy(player => player.UserId)
                .ToList()
        };
    }

    private int ResolvePlayerOrder(Dictionary<Guid, int> playerPlayerOrders, Guid playerId)
    {
        return playerPlayerOrders.TryGetValue(playerId, out var playerOrder) ? playerOrder : int.MaxValue;
    }

    private long ResolveFirstSequence(Dictionary<Guid, long> firstEventSequenceByPlayer, Guid playerId)
    {
        return firstEventSequenceByPlayer.TryGetValue(playerId, out var firstSeq) ? firstSeq : long.MaxValue;
    }

    private bool HasOrderingUsername(Dictionary<Guid, string> usernamesByPlayer, Guid playerId)
    {
        return usernamesByPlayer.TryGetValue(playerId, out var username) && !string.IsNullOrWhiteSpace(username);
    }

    private string ResolveOrderingUsername(Dictionary<Guid, string> usernamesByPlayer, Guid playerId)
    {
        return usernamesByPlayer.TryGetValue(playerId, out var username) && !string.IsNullOrWhiteSpace(username)
            ? username.Trim()
            : string.Empty;
    }
}
