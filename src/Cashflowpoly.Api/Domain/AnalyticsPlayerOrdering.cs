// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsPlayerOrdering.
// Mengimpor namespace `System.Globalization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Globalization;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Helper pengurutan pemain analitik untuk menjaga controller tetap tipis.
/// </summary>
// Mendefinisikan tipe class `PlayerOrderingService` yang mewarisi atau menerapkan `IPlayerOrdering`; sealed mencegah tipe ini diturunkan lagi.
internal sealed class PlayerOrderingService : IPlayerOrdering
{
    private static readonly StringComparer UsernameOrderingComparer = StringComparer.Create(new CultureInfo("id-ID"), true);

    /// <summary>
    /// Mengurutkan daftar pemain sesuai konfigurasi PlayerOrdering pada ruleset.
    /// </summary>
    // Mendefinisikan metode `OrderPlayers` dengan hasil bertipe `List<AnalyticsByPlayerItem>`. Mengurutkan daftar pemain sesuai konfigurasi
    // PlayerOrdering pada ruleset. Masukan: Parameter `players` bertipe `List<AnalyticsByPlayerItem>` membawa nilai pemain; Parameter `ordering`
    // bertipe `PlayerOrdering` membawa nilai ordering; Parameter `playerPlayerOrders` bertipe `Dictionary<Guid, int>` membawa nilai pemain pemain
    // pesanan; Parameter `firstEventSequenceByPlayer` bertipe `Dictionary<Guid, long>` membawa nilai first event sequence berdasarkan pemain; Parameter
    // `usernamesByPlayer` bertipe `Dictionary<Guid, string>` membawa nilai usernames berdasarkan pemain.
    public List<AnalyticsByPlayerItem> OrderPlayers(
        // Parameter `players` bertipe `List<AnalyticsByPlayerItem>` membawa nilai pemain.
        List<AnalyticsByPlayerItem> players,
        // Parameter `ordering` bertipe `PlayerOrdering` membawa nilai ordering.
        PlayerOrdering ordering,
        // Parameter `playerPlayerOrders` bertipe `Dictionary<Guid, int>` membawa nilai pemain pemain pesanan.
        Dictionary<Guid, int> playerPlayerOrders,
        // Parameter `firstEventSequenceByPlayer` bertipe `Dictionary<Guid, long>` membawa nilai first event sequence berdasarkan pemain.
        Dictionary<Guid, long> firstEventSequenceByPlayer,
        // Parameter `usernamesByPlayer` bertipe `Dictionary<Guid, string>` membawa nilai usernames berdasarkan pemain.
        Dictionary<Guid, string> usernamesByPlayer)
    {
        return ordering switch
        {
            // Untuk pola `PlayerOrdering.Username`, menghasilkan mematerialisasi urutan `players .OrderBy(player => HasOrderingUsername(usernamesByPlayer,
            // player.UserId) ? 0 : 1) .ThenBy(player => ResolveOrderingUsername(usernamesByPlayer, player.UserId), Username...` menjadi List; enumerasi
            // dijalankan dan hasilnya disimpan dalam memori sebagai hasil switch.
            PlayerOrdering.Username => players
                .OrderBy(player => HasOrderingUsername(usernamesByPlayer, player.UserId) ? 0 : 1)
                .ThenBy(player => ResolveOrderingUsername(usernamesByPlayer, player.UserId), UsernameOrderingComparer)
                .ThenBy(player => ResolvePlayerOrder(playerPlayerOrders, player.UserId))
                .ThenBy(player => ResolveFirstSequence(firstEventSequenceByPlayer, player.UserId))
                .ThenBy(player => player.UserId)
                .ToList(),
            // Untuk pola `PlayerOrdering.EventSequence`, menghasilkan mematerialisasi urutan `players .OrderBy(player =>
            // ResolveFirstSequence(firstEventSequenceByPlayer, player.UserId)) .ThenBy(player => ResolvePlayerOrder(playerPlayerOrders, player.UserId))
            // .ThenBy(pl...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori sebagai hasil switch.
            PlayerOrdering.EventSequence => players
                .OrderBy(player => ResolveFirstSequence(firstEventSequenceByPlayer, player.UserId))
                .ThenBy(player => ResolvePlayerOrder(playerPlayerOrders, player.UserId))
                .ThenBy(player => player.UserId)
                .ToList(),
            // Untuk pola `PlayerOrdering.PlayerId`, menghasilkan mematerialisasi urutan `players .OrderBy(player => player.UserId)` menjadi List; enumerasi
            // dijalankan dan hasilnya disimpan dalam memori sebagai hasil switch.
            PlayerOrdering.PlayerId => players
                .OrderBy(player => player.UserId)
                .ToList(),
            // Untuk pola `_`, menghasilkan mematerialisasi urutan `players .OrderBy(player => ResolvePlayerOrder(playerPlayerOrders, player.UserId))
            // .ThenBy(player => ResolveFirstSequence(firstEventSequenceByPlayer, player.UserId)) .ThenBy(pl...` menjadi List; enumerasi dijalankan dan hasilnya
            // disimpan dalam memori sebagai hasil switch.
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
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: username.Trim() dalam ResolveOrderingUsername.
            ? username.Trim()
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: string.Empty; dalam ResolveOrderingUsername.
            : string.Empty;
    }
}
