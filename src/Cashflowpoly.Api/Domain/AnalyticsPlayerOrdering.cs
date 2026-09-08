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
// Membuka scope tipe PlayerOrderingService; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `StringComparer`: `UsernameOrderingComparer` menyimpan nilai username ordering comparer dengan nilai awal memanggil
    // `StringComparer.Create` dengan `new CultureInfo(”id-ID”)`, `true`. readonly membatasi penggantian referensi/nilai field pada deklarasi atau
    // konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
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
    // Membuka scope metode OrderPlayers; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam OrderPlayers.
    {
        // Mengembalikan hasil pemetaan `ordering` melalui cabang pola switch yang cocok kepada pemanggil dalam OrderPlayers; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return ordering switch
        // Membuka scope pemetaan switch atas `ordering`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam OrderPlayers.
        {
            // Untuk pola `PlayerOrdering.Username`, menghasilkan mematerialisasi urutan `players .OrderBy(player => HasOrderingUsername(usernamesByPlayer,
            // player.UserId) ? 0 : 1) .ThenBy(player => ResolveOrderingUsername(usernamesByPlayer, player.UserId), Username...` menjadi List; enumerasi
            // dijalankan dan hasilnya disimpan dalam memori sebagai hasil switch.
            PlayerOrdering.Username => players
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(player => HasOrderingUsername(usernamesByPlayer, player.UserId) ? 0 :
                // 1) dalam OrderPlayers; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .OrderBy(player => HasOrderingUsername(usernamesByPlayer, player.UserId) ? 0 : 1)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(player => ResolveOrderingUsername(usernamesByPlayer, player.UserId),
                // UsernameOrderingComparer) dalam OrderPlayers; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .ThenBy(player => ResolveOrderingUsername(usernamesByPlayer, player.UserId), UsernameOrderingComparer)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(player => ResolvePlayerOrder(playerPlayerOrders, player.UserId)) dalam
                // OrderPlayers; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .ThenBy(player => ResolvePlayerOrder(playerPlayerOrders, player.UserId))
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(player => ResolveFirstSequence(firstEventSequenceByPlayer,
                // player.UserId)) dalam OrderPlayers; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .ThenBy(player => ResolveFirstSequence(firstEventSequenceByPlayer, player.UserId))
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(player => player.UserId) dalam OrderPlayers; token pada baris ini
                // menyambungkan bagian kode sebelum dan sesudahnya.
                .ThenBy(player => player.UserId)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(), dalam OrderPlayers; token pada baris ini menyambungkan bagian kode
                // sebelum dan sesudahnya.
                .ToList(),
            // Untuk pola `PlayerOrdering.EventSequence`, menghasilkan mematerialisasi urutan `players .OrderBy(player =>
            // ResolveFirstSequence(firstEventSequenceByPlayer, player.UserId)) .ThenBy(player => ResolvePlayerOrder(playerPlayerOrders, player.UserId))
            // .ThenBy(pl...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori sebagai hasil switch.
            PlayerOrdering.EventSequence => players
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(player => ResolveFirstSequence(firstEventSequenceByPlayer,
                // player.UserId)) dalam OrderPlayers; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .OrderBy(player => ResolveFirstSequence(firstEventSequenceByPlayer, player.UserId))
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(player => ResolvePlayerOrder(playerPlayerOrders, player.UserId)) dalam
                // OrderPlayers; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .ThenBy(player => ResolvePlayerOrder(playerPlayerOrders, player.UserId))
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(player => player.UserId) dalam OrderPlayers; token pada baris ini
                // menyambungkan bagian kode sebelum dan sesudahnya.
                .ThenBy(player => player.UserId)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(), dalam OrderPlayers; token pada baris ini menyambungkan bagian kode
                // sebelum dan sesudahnya.
                .ToList(),
            // Untuk pola `PlayerOrdering.PlayerId`, menghasilkan mematerialisasi urutan `players .OrderBy(player => player.UserId)` menjadi List; enumerasi
            // dijalankan dan hasilnya disimpan dalam memori sebagai hasil switch.
            PlayerOrdering.PlayerId => players
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(player => player.UserId) dalam OrderPlayers; token pada baris ini
                // menyambungkan bagian kode sebelum dan sesudahnya.
                .OrderBy(player => player.UserId)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(), dalam OrderPlayers; token pada baris ini menyambungkan bagian kode
                // sebelum dan sesudahnya.
                .ToList(),
            // Untuk pola `_`, menghasilkan mematerialisasi urutan `players .OrderBy(player => ResolvePlayerOrder(playerPlayerOrders, player.UserId))
            // .ThenBy(player => ResolveFirstSequence(firstEventSequenceByPlayer, player.UserId)) .ThenBy(pl...` menjadi List; enumerasi dijalankan dan hasilnya
            // disimpan dalam memori sebagai hasil switch.
            _ => players
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(player => ResolvePlayerOrder(playerPlayerOrders, player.UserId)) dalam
                // OrderPlayers; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .OrderBy(player => ResolvePlayerOrder(playerPlayerOrders, player.UserId))
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(player => ResolveFirstSequence(firstEventSequenceByPlayer,
                // player.UserId)) dalam OrderPlayers; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .ThenBy(player => ResolveFirstSequence(firstEventSequenceByPlayer, player.UserId))
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(player => player.UserId) dalam OrderPlayers; token pada baris ini
                // menyambungkan bagian kode sebelum dan sesudahnya.
                .ThenBy(player => player.UserId)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList() dalam OrderPlayers; token pada baris ini menyambungkan bagian kode
                // sebelum dan sesudahnya.
                .ToList()
        // Menutup scope pemetaan switch atas `ordering`; bagian berikut berada di luar batas blok tersebut dalam OrderPlayers.
        };
    // Menutup scope metode OrderPlayers; bagian berikut berada di luar batas blok tersebut dalam OrderPlayers.
    }

    // Mendefinisikan metode `ResolvePlayerOrder` dengan hasil bertipe `int`; operasi ini menangani resolve pemain urutan/pesanan. Masukan: Parameter
    // `playerPlayerOrders` bertipe `Dictionary<Guid, int>` membawa nilai pemain pemain pesanan; Parameter `playerId` bertipe `Guid` membawa nilai
    // pemain identitas.
    private int ResolvePlayerOrder(Dictionary<Guid, int> playerPlayerOrders, Guid playerId)
    // Membuka scope metode ResolvePlayerOrder; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolvePlayerOrder.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `playerPlayerOrders.TryGetValue(playerId, out var playerOrder)` benar gunakan `playerOrder`, jika
        // tidak gunakan `int.MaxValue` kepada pemanggil dalam ResolvePlayerOrder; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return playerPlayerOrders.TryGetValue(playerId, out var playerOrder) ? playerOrder : int.MaxValue;
    // Menutup scope metode ResolvePlayerOrder; bagian berikut berada di luar batas blok tersebut dalam ResolvePlayerOrder.
    }

    // Mendefinisikan metode `ResolveFirstSequence` dengan hasil bertipe `long`; operasi ini menangani resolve first sequence. Masukan: Parameter
    // `firstEventSequenceByPlayer` bertipe `Dictionary<Guid, long>` membawa nilai first event sequence berdasarkan pemain; Parameter `playerId` bertipe
    // `Guid` membawa nilai pemain identitas.
    private long ResolveFirstSequence(Dictionary<Guid, long> firstEventSequenceByPlayer, Guid playerId)
    // Membuka scope metode ResolveFirstSequence; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveFirstSequence.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `firstEventSequenceByPlayer.TryGetValue(playerId, out var firstSeq)` benar gunakan `firstSeq`,
        // jika tidak gunakan `long.MaxValue` kepada pemanggil dalam ResolveFirstSequence; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return firstEventSequenceByPlayer.TryGetValue(playerId, out var firstSeq) ? firstSeq : long.MaxValue;
    // Menutup scope metode ResolveFirstSequence; bagian berikut berada di luar batas blok tersebut dalam ResolveFirstSequence.
    }

    // Mendefinisikan metode `HasOrderingUsername` dengan hasil bertipe `bool`; operasi ini menangani memiliki ordering username. Masukan: Parameter
    // `usernamesByPlayer` bertipe `Dictionary<Guid, string>` membawa nilai usernames berdasarkan pemain; Parameter `playerId` bertipe `Guid` membawa
    // nilai pemain identitas.
    private bool HasOrderingUsername(Dictionary<Guid, string> usernamesByPlayer, Guid playerId)
    // Membuka scope metode HasOrderingUsername; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam HasOrderingUsername.
    {
        // Mengembalikan gabungan syarat AND: kedua kondisi wajib benar antara `usernamesByPlayer.TryGetValue(playerId, out var username)` dan
        // `!string.IsNullOrWhiteSpace(username)`; sisi kanan diperiksa hanya jika sisi kiri benar kepada pemanggil dalam HasOrderingUsername; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return usernamesByPlayer.TryGetValue(playerId, out var username) && !string.IsNullOrWhiteSpace(username);
    // Menutup scope metode HasOrderingUsername; bagian berikut berada di luar batas blok tersebut dalam HasOrderingUsername.
    }

    // Mendefinisikan metode `ResolveOrderingUsername` dengan hasil bertipe `string`; operasi ini menangani resolve ordering username. Masukan:
    // Parameter `usernamesByPlayer` bertipe `Dictionary<Guid, string>` membawa nilai usernames berdasarkan pemain; Parameter `playerId` bertipe `Guid`
    // membawa nilai pemain identitas.
    private string ResolveOrderingUsername(Dictionary<Guid, string> usernamesByPlayer, Guid playerId)
    // Membuka scope metode ResolveOrderingUsername; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveOrderingUsername.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `usernamesByPlayer.TryGetValue(playerId, out var username) &&
        // !string.IsNullOrWhiteSpace(username)` benar gunakan `username.Trim()`, jika tidak gunakan `string.Empty` kepada pemanggil dalam
        // ResolveOrderingUsername; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return usernamesByPlayer.TryGetValue(playerId, out var username) && !string.IsNullOrWhiteSpace(username)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: username.Trim() dalam ResolveOrderingUsername.
            ? username.Trim()
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: string.Empty; dalam ResolveOrderingUsername.
            : string.Empty;
    // Menutup scope metode ResolveOrderingUsername; bagian berikut berada di luar batas blok tersebut dalam ResolveOrderingUsername.
    }
// Menutup scope tipe PlayerOrderingService; bagian berikut berada di luar batas blok tersebut.
}
