// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IPlayerOrdering.
using Cashflowpoly.Api.Contracts;

namespace Cashflowpoly.Api.Domain;

public interface IPlayerOrdering
{
    List<AnalyticsByPlayerItem> OrderPlayers(
        List<AnalyticsByPlayerItem> players,
        PlayerOrdering ordering,
        Dictionary<Guid, int> playerPlayerOrders,
        Dictionary<Guid, long> firstEventSequenceByPlayer,
        Dictionary<Guid, string> usernamesByPlayer);
}
