// Fungsi file: Menyiapkan payload pembagian awal deterministik untuk pengujian integrasi.
// Mengimpor namespace `System.Net.Http.Headers` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Headers;
// Mengimpor namespace `System.Net.Http.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Json;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests.Infrastructure;

// Mendefinisikan tipe class `SessionSetupTestHelper`.
internal static class SessionSetupTestHelper
// Membuka scope tipe SessionSetupTestHelper; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `SaveAsync` dengan hasil bertipe `Task<HttpResponseMessage>`; operasi ini menangani save asinkron. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `client` bertipe `HttpClient` membawa
    // nilai client; Parameter `accessToken` bertipe `string` membawa nilai akses token; Parameter `sessionId` bertipe `Guid` membawa identitas unik
    // sesi permainan yang menjadi batas data operasi ini; Parameter `definition` bertipe `RulesetDefinitionDto` membawa definisi terstruktur komponen
    // serta parameter aturan permainan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika
    // pemanggil membatalkan permintaan atau aplikasi berhenti.
    public static async Task<HttpResponseMessage> SaveAsync(
        // Parameter `client` bertipe `HttpClient` membawa nilai client.
        HttpClient client,
        // Parameter `accessToken` bertipe `string` membawa nilai akses token.
        string accessToken,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `definition` bertipe `RulesetDefinitionDto` membawa definisi terstruktur komponen serta parameter aturan permainan.
        RulesetDefinitionDto definition,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode SaveAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SaveAsync.
    {
        // Menyiapkan variabel lokal `listRequest` untuk nilai daftar permintaan dengan objek baru bertipe `HttpRequestMessage` dengan argumen (
        // HttpMethod.Get, $”/api/v1/sessions/{sessionId}/players”). Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya
        // dilepas otomatis saat scope berakhir.
        using var listRequest = new HttpRequestMessage(
            // Meneruskan `HttpMethod.Get` (nilai get) sebagai argumen ke konstruktor `HttpRequestMessage`.
            HttpMethod.Get,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{sessionId}/players”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
            // sebagai argumen ke konstruktor `HttpRequestMessage`.
            $"/api/v1/sessions/{sessionId}/players");
        // Memperbarui `listRequest.Headers.Authorization` menggunakan objek baru bertipe `AuthenticationHeaderValue` dengan argumen (”Bearer”, accessToken)
        // dalam SaveAsync.
        listRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        // Menyiapkan variabel lokal `listResponse` untuk nilai daftar respons dengan hasil operasi asinkron memanggil `client.SendAsync` dengan
        // `listRequest`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var listResponse = await client.SendAsync(listRequest, ct);
        // Menjalankan memanggil `listResponse.EnsureSuccessStatusCode` dengan tanpa argumen dalam SaveAsync.
        listResponse.EnsureSuccessStatusCode();
        // Menyiapkan variabel lokal `playerList` untuk nilai pemain daftar dengan `await
        // listResponse.Content.ReadFromJsonAsync<SessionPlayerListResponse>(cancellationToken: ct)` bila tidak null; jika null gunakan `throw new
        // InvalidOperationException(”Daftar peserta sesi tidak dapat dibaca oleh test.”)` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var playerList = await listResponse.Content.ReadFromJsonAsync<SessionPlayerListResponse>(cancellationToken: ct)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: throw new InvalidOperationException(”Daftar peserta sesi tidak dapat
            // dibaca oleh test.”); dalam SaveAsync.
            ?? throw new InvalidOperationException("Daftar peserta sesi tidak dapat dibaca oleh test.");
        // Menyiapkan variabel lokal `players` untuk nilai pemain dengan mematerialisasi urutan `playerList.Items.OrderBy(item => item.PlayerOrder)` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var players = playerList.Items.OrderBy(item => item.PlayerOrder).ToList();

        // Menyiapkan variabel lokal `tieBreakers` untuk nilai tie breakers dengan mematerialisasi urutan `definition.TieBreakers.OrderBy(item =>
        // item.TieNumber).Take(players.Count)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var tieBreakers = definition.TieBreakers.OrderBy(item => item.TieNumber).Take(players.Count).ToList();
        // Menyiapkan variabel lokal `ingredients` untuk nilai bahan dengan memanggil `Expand` dengan `definition.Ingredients`, `item => item.CardQty ?? 5`,
        // `players.Count`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ingredients = Expand(
            // Meneruskan `definition.Ingredients` (nilai bahan) sebagai argumen ke `Expand`.
            definition.Ingredients,
            // Parameter `item` bertipe `` membawa nilai elemen.
            item => item.CardQty ?? 5,
            // Meneruskan `players.Count`, yaitu jumlah elemen atau panjang data sebagai argumen ke `Expand`.
            players.Count);
        // Menyiapkan variabel lokal `missions` untuk nilai misi dengan mematerialisasi urutan `definition.CollectionMissions.Take(players.Count)` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var missions = definition.CollectionMissions.Take(players.Count).ToList();
        // Menyiapkan variabel lokal `loans` untuk nilai pinjaman dengan hasil pemilihan bersyarat: ketika `definition.Settings.LoanEnabled` benar gunakan
        // `Expand( definition.ShariaLoans, item => item.CardQty ?? 1, players.Count, $”pinjaman (catalog={definition.ShariaLoans.Count};
        // quantities={string.Join(',', definition.ShariaLoa...`, jika tidak gunakan `[]`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var loans = definition.Settings.LoanEnabled
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: Expand( dalam SaveAsync.
            ? Expand(
                // Meneruskan `definition.ShariaLoans` (nilai sharia pinjaman) sebagai argumen ke `Expand`.
                definition.ShariaLoans,
                // Parameter `item` bertipe `` membawa nilai elemen.
                item => item.CardQty ?? 1,
                // Meneruskan `players.Count`, yaitu jumlah elemen atau panjang data sebagai argumen ke `Expand`.
                players.Count,
                // Meneruskan teks interpolasi `$”pinjaman (catalog={definition.ShariaLoans.Count}; quantities={string.Join(',', definition.ShariaLoans.Select(item
                // => item.CardQty))})”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `Expand`; Meneruskan nilai
                // literal `','` sebagai argumen ke `string.Join`; Meneruskan memetakan setiap elemen `definition.ShariaLoans` melalui `item => item.CardQty`
                // menjadi bentuk hasil yang dibutuhkan sebagai argumen ke `string.Join`; Meneruskan fungsi lambda `item => item.CardQty` yang dijalankan oleh
                // operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `definition.ShariaLoans.Select`.
                $"pinjaman (catalog={definition.ShariaLoans.Count}; quantities={string.Join(',', definition.ShariaLoans.Select(item => item.CardQty))})")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: []; dalam SaveAsync.
            : [];
        // Menyiapkan variabel lokal `insurance` untuk nilai asuransi dengan hasil pemilihan bersyarat: ketika `definition.Settings.InsuranceEnabled` benar
        // gunakan `Enumerable.Repeat( definition.InsuranceProducts.FirstOrDefault() ?? throw new InvalidOperationException(”Ruleset test tidak memiliki
        // produk asuransi awal.”), players.Count) .T...`, jika tidak gunakan `[]`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var insurance = definition.Settings.InsuranceEnabled
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: Enumerable.Repeat( dalam SaveAsync.
            ? Enumerable.Repeat(
                    // Meneruskan `definition.InsuranceProducts.FirstOrDefault()` bila tidak null; jika null gunakan `throw new InvalidOperationException(”Ruleset test
                    // tidak memiliki produk asuransi awal.”)` sebagai nilai pengganti sebagai argumen ke `Enumerable.Repeat`.
                    definition.InsuranceProducts.FirstOrDefault()
                        // Meneruskan nilai literal `”Ruleset test tidak memiliki produk asuransi awal.”` sebagai argumen ke konstruktor `InvalidOperationException`.
                        ?? throw new InvalidOperationException("Ruleset test tidak memiliki produk asuransi awal."),
                    // Meneruskan `players.Count`, yaitu jumlah elemen atau panjang data sebagai argumen ke `Enumerable.Repeat`.
                    players.Count)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList() dalam SaveAsync; token pada baris ini menyambungkan bagian kode
                // sebelum dan sesudahnya.
                .ToList()
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: []; dalam SaveAsync.
            : [];

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `tieBreakers.Count != players.Count` dan `missions.Count !=
        // players.Count`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam SaveAsync.
        if (tieBreakers.Count != players.Count || missions.Count != players.Count)
        // Membuka scope cabang if untuk kondisi `tieBreakers.Count != players.Count || missions.Count != players.Count`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam SaveAsync.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Ruleset test tidak memiliki cukup kartu
            // pembagian awal.”) dalam SaveAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Ruleset test tidak memiliki cukup kartu pembagian awal.");
        // Menutup scope cabang if untuk kondisi `tieBreakers.Count != players.Count || missions.Count != players.Count`; bagian berikut berada di luar
        // batas blok tersebut dalam SaveAsync.
        }

        // Menyiapkan variabel lokal `assignments` untuk nilai assignments dengan mematerialisasi urutan `players.Select((player, index) => new
        // SessionPlayerSetupRequest( player.SessionPlayerId, tieBreakers[index].TieBreakerCode, ingredients[index].Id, 1, missions[index].Id, defin...`
        // menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var assignments = players.Select((player, index) => new SessionPlayerSetupRequest(
            // Meneruskan `player.SessionPlayerId` (identitas keikutsertaan pemain pada sesi tertentu) sebagai argumen ke konstruktor
            // `SessionPlayerSetupRequest`.
            player.SessionPlayerId,
            // Meneruskan `tieBreakers[index].TieBreakerCode` (kode kartu penentu urutan saat nilai pemain sama) sebagai argumen ke konstruktor
            // `SessionPlayerSetupRequest`; Meneruskan `index` (nilai index) sebagai argumen ke konstruktor `SessionPlayerSetupRequest`.
            tieBreakers[index].TieBreakerCode,
            // Meneruskan `ingredients[index].Id` (nilai identitas) sebagai argumen ke konstruktor `SessionPlayerSetupRequest`; Meneruskan `index` (nilai index)
            // sebagai argumen ke konstruktor `SessionPlayerSetupRequest`.
            ingredients[index].Id,
            // Meneruskan nilai literal `1` sebagai argumen ke konstruktor `SessionPlayerSetupRequest`.
            1,
            // Meneruskan `missions[index].Id` (nilai identitas) sebagai argumen ke konstruktor `SessionPlayerSetupRequest`; Meneruskan `index` (nilai index)
            // sebagai argumen ke konstruktor `SessionPlayerSetupRequest`.
            missions[index].Id,
            // Meneruskan hasil pemilihan bersyarat: ketika `definition.Settings.LoanEnabled` benar gunakan `loans[index].LoanCode`, jika tidak gunakan `null`
            // sebagai argumen ke konstruktor `SessionPlayerSetupRequest`; Meneruskan `index` (nilai index) sebagai argumen ke konstruktor
            // `SessionPlayerSetupRequest`.
            definition.Settings.LoanEnabled ? loans[index].LoanCode : null,
            // Meneruskan hasil pemilihan bersyarat: ketika `definition.Settings.InsuranceEnabled` benar gunakan `insurance[index].ProductCode`, jika tidak
            // gunakan `null` sebagai argumen ke konstruktor `SessionPlayerSetupRequest`; Meneruskan `index` (nilai index) sebagai argumen ke konstruktor
            // `SessionPlayerSetupRequest`.
            definition.Settings.InsuranceEnabled ? insurance[index].ProductCode : null)).ToList();
        // Menyiapkan variabel lokal `setup` untuk nilai setup dengan objek baru bertipe `SessionSetupRequest` dengan argumen ($”test-setup-{sessionId:N}”,
        // assignments). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var setup = new SessionSetupRequest($"test-setup-{sessionId:N}", assignments);

        // Menyiapkan variabel lokal `saveRequest` untuk nilai save permintaan dengan objek baru bertipe `HttpRequestMessage` dengan argumen (
        // HttpMethod.Post, $”/api/v1/sessions/{sessionId}/setup”). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var saveRequest = new HttpRequestMessage(
            // Meneruskan `HttpMethod.Post` (nilai post) sebagai argumen ke konstruktor `HttpRequestMessage`.
            HttpMethod.Post,
            // Meneruskan teks interpolasi `$”/api/v1/sessions/{sessionId}/setup”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan
            // sebagai argumen ke konstruktor `HttpRequestMessage`.
            $"/api/v1/sessions/{sessionId}/setup");
        // Memperbarui `saveRequest.Headers.Authorization` menggunakan objek baru bertipe `AuthenticationHeaderValue` dengan argumen (”Bearer”, accessToken)
        // dalam SaveAsync.
        saveRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        // Memperbarui `saveRequest.Content` menggunakan memanggil `JsonContent.Create` dengan `setup` dalam SaveAsync.
        saveRequest.Content = JsonContent.Create(setup);
        // Mengembalikan hasil operasi asinkron memanggil `client.SendAsync` dengan `saveRequest`, `ct`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai kepada pemanggil dalam SaveAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await client.SendAsync(saveRequest, ct);
    // Menutup scope metode SaveAsync; bagian berikut berada di luar batas blok tersebut dalam SaveAsync.
    }

    // Mendefinisikan metode `Expand` dengan hasil bertipe `List<T>`; operasi ini menangani expand. Masukan: Parameter `source` bertipe `IEnumerable<T>`
    // membawa nilai source; Parameter `getQuantity` bertipe `Func<T, int>` membawa nilai get jumlah; Parameter `required` bertipe `int` membawa nilai
    // required; Parameter `cardName` bertipe `string` membawa nilai kartu nama; bila argumen tidak diberikan digunakan nilai literal `”kartu”`.
    private static List<T> Expand<T>(
        // Parameter `source` bertipe `IEnumerable<T>` membawa nilai source.
        IEnumerable<T> source,
        // Parameter `getQuantity` bertipe `Func<T, int>` membawa nilai get jumlah.
        Func<T, int> getQuantity,
        // Parameter `required` bertipe `int` membawa nilai required.
        int required,
        // Parameter `cardName` bertipe `string` membawa nilai kartu nama; bila argumen tidak diberikan digunakan nilai literal `”kartu”`.
        string cardName = "kartu")
    // Membuka scope metode Expand; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Expand.
    {
        // Menyiapkan variabel lokal `cards` untuk nilai kartu dengan mematerialisasi urutan `source .SelectMany(item => Enumerable.Repeat(item, Math.Max(0,
        // getQuantity(item)))) .Take(required)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var cards = source
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .SelectMany(item => Enumerable.Repeat(item, Math.Max(0, getQuantity(item))))
            // dalam Expand; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .SelectMany(item => Enumerable.Repeat(item, Math.Max(0, getQuantity(item))))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Take(required) dalam Expand; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .Take(required)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam Expand; token pada baris ini menyambungkan bagian kode sebelum
            // dan sesudahnya.
            .ToList();
        // Memeriksa perbandingan ketidaksamaan antara `cards.Count` dan `required`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // Expand.
        if (cards.Count != required)
        // Membuka scope cabang if untuk kondisi `cards.Count != required`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Expand.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( $”Ruleset test hanya memiliki {cards.Count}
            // {cardName} pembagian awal; dibutuhkan {required}.”) dalam Expand; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException(
                // Meneruskan teks interpolasi `$”Ruleset test hanya memiliki {cards.Count} {cardName} pembagian awal; dibutuhkan {required}.”`; nilai ekspresi di
                // dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke konstruktor `InvalidOperationException`.
                $"Ruleset test hanya memiliki {cards.Count} {cardName} pembagian awal; dibutuhkan {required}.");
        // Menutup scope cabang if untuk kondisi `cards.Count != required`; bagian berikut berada di luar batas blok tersebut dalam Expand.
        }

        // Mengembalikan `cards` (nilai kartu) kepada pemanggil dalam Expand; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return cards;
    // Menutup scope metode Expand; bagian berikut berada di luar batas blok tersebut dalam Expand.
    }
// Menutup scope tipe SessionSetupTestHelper; bagian berikut berada di luar batas blok tersebut.
}
