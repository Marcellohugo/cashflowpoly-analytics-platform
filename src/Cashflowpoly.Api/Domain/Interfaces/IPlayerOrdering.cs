using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

public interface IPlayerOrdering
{
    List<AnalyticsByPlayerItem> OrderPlayers(
        List<AnalyticsByPlayerItem> players,
        PlayerOrdering ordering,
        Dictionary<Guid, int> playerJoinOrders,
        Dictionary<Guid, long> firstEventSequenceByPlayer,
        Dictionary<Guid, string> usernamesByPlayer);
}
