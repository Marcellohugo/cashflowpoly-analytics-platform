// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IPlayerOrdering.
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public interface IPlayerOrdering
{
    List<AnalyticsByPlayerItem> OrderPlayers(
        // Parameter `players` bertipe `List<AnalyticsByPlayerItem>` membawa nilai pemain.
        List<AnalyticsByPlayerItem> players,
        // Parameter `ordering` bertipe `PlayerOrdering` membawa nilai ordering.
        PlayerOrdering ordering,
        // Parameter `playerPlayerOrders` bertipe `Dictionary<Guid, int>` membawa nilai pemain pemain pesanan.
        Dictionary<Guid, int> playerPlayerOrders,
        // Parameter `firstEventSequenceByPlayer` bertipe `Dictionary<Guid, long>` membawa nilai first event sequence berdasarkan pemain.
        Dictionary<Guid, long> firstEventSequenceByPlayer,
        // Parameter `usernamesByPlayer` bertipe `Dictionary<Guid, string>` membawa nilai usernames berdasarkan pemain.
        Dictionary<Guid, string> usernamesByPlayer);
}
