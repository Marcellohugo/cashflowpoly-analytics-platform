// Fungsi file: Memvalidasi pembagian awal fisik pemain terhadap ruleset sesi.
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan tipe class `SessionSetupValidator`.
public static class SessionSetupValidator
// Membuka scope tipe SessionSetupValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Normalize` dengan hasil bertipe `SessionSetupRequest`; operasi ini menangani normalize. Masukan: Parameter `request`
    // bertipe `SessionSetupRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
    public static SessionSetupRequest Normalize(SessionSetupRequest request)
    // Membuka scope metode Normalize; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Normalize.
    {
        // Menyiapkan variabel lokal `players` untuk nilai pemain dengan mematerialisasi urutan `(request.Players ?? []) .Select(player => new
        // SessionPlayerSetupRequest( player.SessionPlayerId, (player.TieBreakerCode ?? string.Empty).Trim(), (player.IngredientCardId ?? st...` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var players = (request.Players ?? [])
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(player => new SessionPlayerSetupRequest( dalam Normalize; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(player => new SessionPlayerSetupRequest(
                // Meneruskan `player.SessionPlayerId` (identitas keikutsertaan pemain pada sesi tertentu) sebagai argumen ke konstruktor
                // `SessionPlayerSetupRequest`.
                player.SessionPlayerId,
                // Meneruskan membersihkan karakter tepi pada `(player.TieBreakerCode ?? string.Empty)` memakai tanpa argumen sebagai argumen ke konstruktor
                // `SessionPlayerSetupRequest`.
                (player.TieBreakerCode ?? string.Empty).Trim(),
                // Meneruskan membersihkan karakter tepi pada `(player.IngredientCardId ?? string.Empty)` memakai tanpa argumen sebagai argumen ke konstruktor
                // `SessionPlayerSetupRequest`.
                (player.IngredientCardId ?? string.Empty).Trim(),
                // Meneruskan `player.GoldQuantity` (jumlah kartu atau unit emas pemain) sebagai argumen ke konstruktor `SessionPlayerSetupRequest`.
                player.GoldQuantity,
                // Meneruskan memanggil `NormalizeOptional` dengan `player.MissionId` sebagai argumen ke konstruktor `SessionPlayerSetupRequest`; Meneruskan
                // `player.MissionId` (identitas misi koleksi yang ditugaskan) sebagai argumen ke `NormalizeOptional`.
                NormalizeOptional(player.MissionId),
                // Meneruskan memanggil `NormalizeOptional` dengan `player.LoanCode` sebagai argumen ke konstruktor `SessionPlayerSetupRequest`; Meneruskan
                // `player.LoanCode` (kode produk pinjaman syariah) sebagai argumen ke `NormalizeOptional`.
                NormalizeOptional(player.LoanCode),
                // Meneruskan memanggil `NormalizeOptional` dengan `player.InsuranceProductCode` sebagai argumen ke konstruktor `SessionPlayerSetupRequest`;
                // Meneruskan `player.InsuranceProductCode` (kode produk asuransi yang digunakan) sebagai argumen ke `NormalizeOptional`.
                NormalizeOptional(player.InsuranceProductCode)))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(player => player.SessionPlayerId) dalam Normalize; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderBy(player => player.SessionPlayerId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam Normalize; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .ToList();

        // Mengembalikan objek baru bertipe `SessionSetupRequest` dengan argumen ((request.ClientRequestId ?? string.Empty).Trim(), players) kepada
        // pemanggil dalam Normalize; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new SessionSetupRequest((request.ClientRequestId ?? string.Empty).Trim(), players);
    // Menutup scope metode Normalize; bagian berikut berada di luar batas blok tersebut dalam Normalize.
    }

    // Mendefinisikan metode `Validate` dengan hasil bertipe `List<ErrorDetail>`; operasi ini menangani validate. Masukan: Parameter `request` bertipe
    // `SessionSetupRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `definition` bertipe
    // `RulesetDefinitionDto` membawa definisi terstruktur komponen serta parameter aturan permainan; Parameter `mode` bertipe `string` membawa mode
    // permainan yang menentukan kelompok aturan yang digunakan; Parameter `sessionPlayers` bertipe `IReadOnlyCollection<SessionPlayerDb>` membawa nilai
    // sesi pemain.
    public static List<ErrorDetail> Validate(
        // Parameter `request` bertipe `SessionSetupRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        SessionSetupRequest request,
        // Parameter `definition` bertipe `RulesetDefinitionDto` membawa definisi terstruktur komponen serta parameter aturan permainan.
        RulesetDefinitionDto definition,
        // Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan.
        string mode,
        // Parameter `sessionPlayers` bertipe `IReadOnlyCollection<SessionPlayerDb>` membawa nilai sesi pemain.
        IReadOnlyCollection<SessionPlayerDb> sessionPlayers)
    // Membuka scope metode Validate; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
    {
        // Menyiapkan variabel lokal `normalized` untuk nilai normalized dengan memanggil `Normalize` dengan `request`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var normalized = Normalize(request);
        // Menyiapkan variabel lokal `errors` untuk nilai kesalahan dengan objek baru bertipe `List<ErrorDetail>` dengan nilai awal sesuai konstruktornya.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var errors = new List<ErrorDetail>();

        // Memeriksa memeriksa apakah `normalized.ClientRequestId` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam Validate.
        if (string.IsNullOrWhiteSpace(normalized.ClientRequestId))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(normalized.ClientRequestId)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam Validate.
        {
            // Menjalankan menambahkan `new ErrorDetail(”client_request_id”, ”REQUIRED”)` ke `errors` dalam Validate.
            errors.Add(new ErrorDetail("client_request_id", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(normalized.ClientRequestId)`; bagian berikut berada di luar batas blok tersebut
        // dalam Validate.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam Validate.
        else if (normalized.ClientRequestId.Length > 120)
        // Membuka scope cabang if untuk kondisi `normalized.ClientRequestId.Length > 120`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam Validate.
        {
            // Menjalankan menambahkan `new ErrorDetail(”client_request_id”, ”MAX_LENGTH”)` ke `errors` dalam Validate.
            errors.Add(new ErrorDetail("client_request_id", "MAX_LENGTH"));
        // Menutup scope cabang if untuk kondisi `normalized.ClientRequestId.Length > 120`; bagian berikut berada di luar batas blok tersebut dalam
        // Validate.
        }

        // Memeriksa perbandingan ketidaksamaan antara `normalized.Players.Count` dan `sessionPlayers.Count`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam Validate.
        if (normalized.Players.Count != sessionPlayers.Count)
        // Membuka scope cabang if untuk kondisi `normalized.Players.Count != sessionPlayers.Count`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam Validate.
        {
            // Menjalankan menambahkan `new ErrorDetail(”players”, ”COUNT_MISMATCH”)` ke `errors` dalam Validate.
            errors.Add(new ErrorDetail("players", "COUNT_MISMATCH"));
        // Menutup scope cabang if untuk kondisi `normalized.Players.Count != sessionPlayers.Count`; bagian berikut berada di luar batas blok tersebut dalam
        // Validate.
        }

        // Menyiapkan variabel lokal `expectedPlayerIds` untuk nilai yang diharapkan pemain identitas dengan membentuk himpunan nilai unik dari
        // `sessionPlayers.Select(player => player.SessionPlayerId)` memakai tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var expectedPlayerIds = sessionPlayers.Select(player => player.SessionPlayerId).ToHashSet();
        // Menyiapkan variabel lokal `assignedPlayerIds` untuk nilai assigned pemain identitas dengan objek baru bertipe `HashSet<Guid>` dengan nilai awal
        // sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var assignedPlayerIds = new HashSet<Guid>();
        // Menyiapkan variabel lokal `assignedTieCodes` untuk nilai assigned tie kode dengan objek baru bertipe `HashSet<string>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var assignedTieCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `assignedTieNumbers` untuk nilai assigned tie numbers dengan objek baru bertipe `HashSet<int>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var assignedTieNumbers = new HashSet<int>();
        // Menyiapkan variabel lokal `assignedMissionIds` untuk nilai assigned misi identitas dengan objek baru bertipe `HashSet<string>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var assignedMissionIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `ingredientCounts` untuk nilai bahan counts dengan objek baru bertipe `Dictionary<string, int>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ingredientCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `loanCounts` untuk nilai pinjaman counts dengan objek baru bertipe `Dictionary<string, int>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var loanCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `insuranceCounts` untuk nilai asuransi counts dengan objek baru bertipe `Dictionary<string, int>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var insuranceCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        // Menyiapkan variabel lokal `tieBreakers` untuk nilai tie breakers dengan membangun kamus dari `definition.TieBreakers .Where(item =>
        // !string.IsNullOrWhiteSpace(item.TieBreakerCode)) .GroupBy(item => item.TieBreakerCode, StringComparer.OrdinalIgnoreCase)` dengan pemilihan
        // kunci/nilai `group => group.Key`, `group => group.First()`, `StringComparer.OrdinalIgnoreCase`; kunci harus unik agar konversi berhasil. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var tieBreakers = definition.TieBreakers
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => !string.IsNullOrWhiteSpace(item.TieBreakerCode)) dalam Validate;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => !string.IsNullOrWhiteSpace(item.TieBreakerCode))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(item => item.TieBreakerCode, StringComparer.OrdinalIgnoreCase) dalam
            // Validate; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(item => item.TieBreakerCode, StringComparer.OrdinalIgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary(group => group.Key, group => group.First(),
            // StringComparer.OrdinalIgnoreCase); dalam Validate; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `ingredients` untuk nilai bahan dengan membangun kamus dari `definition.Ingredients .Where(item =>
        // !string.IsNullOrWhiteSpace(item.Id)) .GroupBy(item => item.Id, StringComparer.OrdinalIgnoreCase)` dengan pemilihan kunci/nilai `group =>
        // group.Key`, `group => group.First()`, `StringComparer.OrdinalIgnoreCase`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var ingredients = definition.Ingredients
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => !string.IsNullOrWhiteSpace(item.Id)) dalam Validate; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => !string.IsNullOrWhiteSpace(item.Id))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(item => item.Id, StringComparer.OrdinalIgnoreCase) dalam Validate;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(item => item.Id, StringComparer.OrdinalIgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary(group => group.Key, group => group.First(),
            // StringComparer.OrdinalIgnoreCase); dalam Validate; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `missions` untuk nilai misi dengan membangun kamus dari `definition.CollectionMissions .Where(item =>
        // !string.IsNullOrWhiteSpace(item.Id)) .GroupBy(item => item.Id, StringComparer.OrdinalIgnoreCase)` dengan pemilihan kunci/nilai `group =>
        // group.Key`, `group => group.First()`, `StringComparer.OrdinalIgnoreCase`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var missions = definition.CollectionMissions
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => !string.IsNullOrWhiteSpace(item.Id)) dalam Validate; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => !string.IsNullOrWhiteSpace(item.Id))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(item => item.Id, StringComparer.OrdinalIgnoreCase) dalam Validate;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(item => item.Id, StringComparer.OrdinalIgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary(group => group.Key, group => group.First(),
            // StringComparer.OrdinalIgnoreCase); dalam Validate; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `loans` untuk nilai pinjaman dengan membangun kamus dari `definition.ShariaLoans .Where(item =>
        // !string.IsNullOrWhiteSpace(item.LoanCode)) .GroupBy(item => item.LoanCode, StringComparer.OrdinalIgnoreCase)` dengan pemilihan kunci/nilai `group
        // => group.Key`, `group => group.First()`, `StringComparer.OrdinalIgnoreCase`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var loans = definition.ShariaLoans
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => !string.IsNullOrWhiteSpace(item.LoanCode)) dalam Validate; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => !string.IsNullOrWhiteSpace(item.LoanCode))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(item => item.LoanCode, StringComparer.OrdinalIgnoreCase) dalam
            // Validate; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(item => item.LoanCode, StringComparer.OrdinalIgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary(group => group.Key, group => group.First(),
            // StringComparer.OrdinalIgnoreCase); dalam Validate; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `insuranceProducts` untuk nilai asuransi products dengan membangun kamus dari `definition.InsuranceProducts .Where(item
        // => !string.IsNullOrWhiteSpace(item.ProductCode)) .GroupBy(item => item.ProductCode, StringComparer.OrdinalIgnoreCase)` dengan pemilihan
        // kunci/nilai `group => group.Key`, `group => group.First()`, `StringComparer.OrdinalIgnoreCase`; kunci harus unik agar konversi berhasil. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var insuranceProducts = definition.InsuranceProducts
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => !string.IsNullOrWhiteSpace(item.ProductCode)) dalam Validate;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => !string.IsNullOrWhiteSpace(item.ProductCode))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(item => item.ProductCode, StringComparer.OrdinalIgnoreCase) dalam
            // Validate; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(item => item.ProductCode, StringComparer.OrdinalIgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary(group => group.Key, group => group.First(),
            // StringComparer.OrdinalIgnoreCase); dalam Validate; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        // Menyiapkan variabel lokal `isMahir` untuk nilai berstatus mahir dengan membandingkan kesamaan `string` dengan `mode`, `”MAHIR”`,
        // `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan comparer yang diberikan. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var isMahir = string.Equals(mode, "MAHIR", StringComparison.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `loanRequired` untuk nilai pinjaman required dengan gabungan syarat AND: kedua kondisi wajib benar antara `isMahir` dan
        // `definition.Settings.LoanEnabled`; sisi kanan diperiksa hanya jika sisi kiri benar. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var loanRequired = isMahir && definition.Settings.LoanEnabled;
        // Menyiapkan variabel lokal `insuranceRequired` untuk nilai asuransi required dengan gabungan syarat AND: kedua kondisi wajib benar antara
        // `isMahir` dan `definition.Settings.InsuranceEnabled`; sisi kanan diperiksa hanya jika sisi kiri benar. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var insuranceRequired = isMahir && definition.Settings.InsuranceEnabled;

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < normalized.Players.Count`, lalu memperbarui pencacah melalui `index++`
        // dalam Validate.
        for (var index = 0; index < normalized.Players.Count; index++)
        // Membuka scope loop dengan syarat `index < normalized.Players.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
        {
            // Menyiapkan variabel lokal `player` untuk nilai pemain dengan `normalized.Players[index]`, yaitu elemen koleksi yang dipilih melalui indeks atau
            // kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var player = normalized.Players[index];
            // Menyiapkan variabel lokal `field` untuk nilai field dengan teks interpolasi `$”players[{index}]”`; nilai ekspresi di dalam kurung kurawal
            // disisipkan saat program berjalan. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var field = $"players[{index}]";

            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `player.SessionPlayerId == Guid.Empty` dan
            // `!expectedPlayerIds.Contains(player.SessionPlayerId)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi
            // ini bernilai benar dalam Validate.
            if (player.SessionPlayerId == Guid.Empty || !expectedPlayerIds.Contains(player.SessionPlayerId))
            // Membuka scope cabang if untuk kondisi `player.SessionPlayerId == Guid.Empty || !expectedPlayerIds.Contains(player.SessionPlayerId)`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
            {
                // Menjalankan menambahkan `new ErrorDetail($”{field}.session_player_id”, ”UNKNOWN_REFERENCE”)` ke `errors` dalam Validate.
                errors.Add(new ErrorDetail($"{field}.session_player_id", "UNKNOWN_REFERENCE"));
            // Menutup scope cabang if untuk kondisi `player.SessionPlayerId == Guid.Empty || !expectedPlayerIds.Contains(player.SessionPlayerId)`; bagian
            // berikut berada di luar batas blok tersebut dalam Validate.
            }
            // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam Validate.
            else if (!assignedPlayerIds.Add(player.SessionPlayerId))
            // Membuka scope cabang if untuk kondisi `!assignedPlayerIds.Add(player.SessionPlayerId)`; pernyataan/deklarasi berikut berada di dalam batas blok
            // ini dalam Validate.
            {
                // Menjalankan menambahkan `new ErrorDetail($”{field}.session_player_id”, ”DUPLICATE”)` ke `errors` dalam Validate.
                errors.Add(new ErrorDetail($"{field}.session_player_id", "DUPLICATE"));
            // Menutup scope cabang if untuk kondisi `!assignedPlayerIds.Add(player.SessionPlayerId)`; bagian berikut berada di luar batas blok tersebut dalam
            // Validate.
            }

            // Memeriksa kebalikan kondisi `tieBreakers.TryGetValue(player.TieBreakerCode, out var tieBreaker)`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam Validate.
            if (!tieBreakers.TryGetValue(player.TieBreakerCode, out var tieBreaker))
            // Membuka scope cabang if untuk kondisi `!tieBreakers.TryGetValue(player.TieBreakerCode, out var tieBreaker)`; pernyataan/deklarasi berikut berada
            // di dalam batas blok ini dalam Validate.
            {
                // Menjalankan menambahkan `new ErrorDetail($”{field}.tie_breaker_code”, ”UNKNOWN_REFERENCE”)` ke `errors` dalam Validate.
                errors.Add(new ErrorDetail($"{field}.tie_breaker_code", "UNKNOWN_REFERENCE"));
            // Menutup scope cabang if untuk kondisi `!tieBreakers.TryGetValue(player.TieBreakerCode, out var tieBreaker)`; bagian berikut berada di luar batas
            // blok tersebut dalam Validate.
            }
            // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam Validate.
            else
            // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
            {
                // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!assignedTieCodes.Add(tieBreaker.TieBreakerCode)` dan
                // `!assignedTieNumbers.Add(tieBreaker.TieNumber)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
                // bernilai benar dalam Validate.
                if (!assignedTieCodes.Add(tieBreaker.TieBreakerCode) || !assignedTieNumbers.Add(tieBreaker.TieNumber))
                // Membuka scope cabang if untuk kondisi `!assignedTieCodes.Add(tieBreaker.TieBreakerCode) || !assignedTieNumbers.Add(tieBreaker.TieNumber)`;
                // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
                {
                    // Menjalankan menambahkan `new ErrorDetail($”{field}.tie_breaker_code”, ”DUPLICATE”)` ke `errors` dalam Validate.
                    errors.Add(new ErrorDetail($"{field}.tie_breaker_code", "DUPLICATE"));
                // Menutup scope cabang if untuk kondisi `!assignedTieCodes.Add(tieBreaker.TieBreakerCode) || !assignedTieNumbers.Add(tieBreaker.TieNumber)`; bagian
                // berikut berada di luar batas blok tersebut dalam Validate.
                }
            // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam Validate.
            }

            // Memeriksa kebalikan kondisi `ingredients.TryGetValue(player.IngredientCardId, out var ingredient)`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam Validate.
            if (!ingredients.TryGetValue(player.IngredientCardId, out var ingredient))
            // Membuka scope cabang if untuk kondisi `!ingredients.TryGetValue(player.IngredientCardId, out var ingredient)`; pernyataan/deklarasi berikut
            // berada di dalam batas blok ini dalam Validate.
            {
                // Menjalankan menambahkan `new ErrorDetail($”{field}.ingredient_card_id”, ”UNKNOWN_REFERENCE”)` ke `errors` dalam Validate.
                errors.Add(new ErrorDetail($"{field}.ingredient_card_id", "UNKNOWN_REFERENCE"));
            // Menutup scope cabang if untuk kondisi `!ingredients.TryGetValue(player.IngredientCardId, out var ingredient)`; bagian berikut berada di luar
            // batas blok tersebut dalam Validate.
            }
            // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam Validate.
            else
            // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
            {
                // Menjalankan memanggil `IncrementCount` dengan `ingredientCounts`, `ingredient.Id` dalam Validate.
                IncrementCount(ingredientCounts, ingredient.Id);
            // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam Validate.
            }

            // Memeriksa perbandingan ketidaksamaan antara `player.GoldQuantity` dan `1`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // Validate.
            if (player.GoldQuantity != 1)
            // Membuka scope cabang if untuk kondisi `player.GoldQuantity != 1`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
            {
                // Menjalankan menambahkan `new ErrorDetail($”{field}.gold_quantity”, ”MUST_EQUAL_ONE”)` ke `errors` dalam Validate.
                errors.Add(new ErrorDetail($"{field}.gold_quantity", "MUST_EQUAL_ONE"));
            // Menutup scope cabang if untuk kondisi `player.GoldQuantity != 1`; bagian berikut berada di luar batas blok tersebut dalam Validate.
            }

            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `string.IsNullOrWhiteSpace(player.MissionId)` dan
            // `!missions.ContainsKey(player.MissionId)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam Validate.
            if (string.IsNullOrWhiteSpace(player.MissionId) || !missions.ContainsKey(player.MissionId))
            // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(player.MissionId) || !missions.ContainsKey(player.MissionId)`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
            {
                // Menjalankan menambahkan `new ErrorDetail($”{field}.mission_id”, string.IsNullOrWhiteSpace(player.MissionId) ? ”REQUIRED” : ”UNKNOWN_REFERENCE”)`
                // ke `errors` dalam Validate.
                errors.Add(new ErrorDetail($"{field}.mission_id", string.IsNullOrWhiteSpace(player.MissionId) ? "REQUIRED" : "UNKNOWN_REFERENCE"));
            // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(player.MissionId) || !missions.ContainsKey(player.MissionId)`; bagian berikut
            // berada di luar batas blok tersebut dalam Validate.
            }
            // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam Validate.
            else if (!assignedMissionIds.Add(player.MissionId))
            // Membuka scope cabang if untuk kondisi `!assignedMissionIds.Add(player.MissionId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
            // dalam Validate.
            {
                // Menjalankan menambahkan `new ErrorDetail($”{field}.mission_id”, ”DUPLICATE”)` ke `errors` dalam Validate.
                errors.Add(new ErrorDetail($"{field}.mission_id", "DUPLICATE"));
            // Menutup scope cabang if untuk kondisi `!assignedMissionIds.Add(player.MissionId)`; bagian berikut berada di luar batas blok tersebut dalam
            // Validate.
            }

            // Menjalankan memanggil `ValidateOptionalCard` dengan `player.LoanCode`, `loanRequired`, `loans`, `loanCounts`, `$”{field}.loan_code”`, `item =>
            // item.LoanCode`, `errors` dalam Validate.
            ValidateOptionalCard(
                // Meneruskan `player.LoanCode` (kode produk pinjaman syariah) sebagai argumen ke `ValidateOptionalCard`.
                player.LoanCode,
                // Meneruskan `loanRequired` (nilai pinjaman required) sebagai argumen ke `ValidateOptionalCard`.
                loanRequired,
                // Meneruskan `loans` (nilai pinjaman) sebagai argumen ke `ValidateOptionalCard`.
                loans,
                // Meneruskan `loanCounts` (nilai pinjaman counts) sebagai argumen ke `ValidateOptionalCard`.
                loanCounts,
                // Meneruskan teks interpolasi `$”{field}.loan_code”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke
                // `ValidateOptionalCard`.
                $"{field}.loan_code",
                // Parameter `item` bertipe `` membawa nilai elemen.
                item => item.LoanCode,
                // Meneruskan `errors` (nilai kesalahan) sebagai argumen ke `ValidateOptionalCard`.
                errors);
            // Menjalankan memanggil `ValidateOptionalCard` dengan `player.InsuranceProductCode`, `insuranceRequired`, `insuranceProducts`, `insuranceCounts`,
            // `$”{field}.insurance_product_code”`, `item => item.ProductCode`, `errors` dalam Validate.
            ValidateOptionalCard(
                // Meneruskan `player.InsuranceProductCode` (kode produk asuransi yang digunakan) sebagai argumen ke `ValidateOptionalCard`.
                player.InsuranceProductCode,
                // Meneruskan `insuranceRequired` (nilai asuransi required) sebagai argumen ke `ValidateOptionalCard`.
                insuranceRequired,
                // Meneruskan `insuranceProducts` (nilai asuransi products) sebagai argumen ke `ValidateOptionalCard`.
                insuranceProducts,
                // Meneruskan `insuranceCounts` (nilai asuransi counts) sebagai argumen ke `ValidateOptionalCard`.
                insuranceCounts,
                // Meneruskan teks interpolasi `$”{field}.insurance_product_code”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai
                // argumen ke `ValidateOptionalCard`.
                $"{field}.insurance_product_code",
                // Parameter `item` bertipe `` membawa nilai elemen.
                item => item.ProductCode,
                // Meneruskan `errors` (nilai kesalahan) sebagai argumen ke `ValidateOptionalCard`.
                errors);
        // Menutup scope loop dengan syarat `index < normalized.Players.Count`; bagian berikut berada di luar batas blok tersebut dalam Validate.
        }

        // Memeriksa kebalikan kondisi `expectedPlayerIds.SetEquals(assignedPlayerIds)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // Validate.
        if (!expectedPlayerIds.SetEquals(assignedPlayerIds))
        // Membuka scope cabang if untuk kondisi `!expectedPlayerIds.SetEquals(assignedPlayerIds)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam Validate.
        {
            // Menjalankan menambahkan `new ErrorDetail(”players”, ”MISSING_PARTICIPANT”)` ke `errors` dalam Validate.
            errors.Add(new ErrorDetail("players", "MISSING_PARTICIPANT"));
        // Menutup scope cabang if untuk kondisi `!expectedPlayerIds.SetEquals(assignedPlayerIds)`; bagian berikut berada di luar batas blok tersebut dalam
        // Validate.
        }

        // Memeriksa kebalikan kondisi `Enumerable.Range(1, sessionPlayers.Count).All(assignedTieNumbers.Contains)`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam Validate.
        if (!Enumerable.Range(1, sessionPlayers.Count).All(assignedTieNumbers.Contains))
        // Membuka scope cabang if untuk kondisi `!Enumerable.Range(1, sessionPlayers.Count).All(assignedTieNumbers.Contains)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam Validate.
        {
            // Menjalankan menambahkan `new ErrorDetail(”players.tie_breaker_code”, ”ORDER_SEQUENCE_INCOMPLETE”)` ke `errors` dalam Validate.
            errors.Add(new ErrorDetail("players.tie_breaker_code", "ORDER_SEQUENCE_INCOMPLETE"));
        // Menutup scope cabang if untuk kondisi `!Enumerable.Range(1, sessionPlayers.Count).All(assignedTieNumbers.Contains)`; bagian berikut berada di
        // luar batas blok tersebut dalam Validate.
        }

        // Menjalankan memanggil `ValidatePhysicalCardQuantity` dengan `ingredientCounts`, `ingredients`, `item => item.CardQty ?? 5`,
        // `”players.ingredient_card_id”`, `errors` dalam Validate.
        ValidatePhysicalCardQuantity(ingredientCounts, ingredients, item => item.CardQty ?? 5, "players.ingredient_card_id", errors);
        // Menjalankan memanggil `ValidatePhysicalCardQuantity` dengan `loanCounts`, `loans`, `item => item.CardQty ?? 1`, `”players.loan_code”`, `errors`
        // dalam Validate.
        ValidatePhysicalCardQuantity(loanCounts, loans, item => item.CardQty ?? 1, "players.loan_code", errors);
        // Asuransi mode mahir berada di sisi belakang kartu Tie Breaker, sehingga
        // card_qty produk asuransi memang 0 dan tidak mewakili stok kartu terpisah.

        // Mengembalikan mematerialisasi urutan `errors.Distinct()` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori kepada pemanggil
        // dalam Validate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return errors.Distinct().ToList();
    // Menutup scope metode Validate; bagian berikut berada di luar batas blok tersebut dalam Validate.
    }

    // Mendefinisikan metode `NormalizeOptional` dengan hasil bertipe `string?`; operasi ini menangani normalize optional. Masukan: Parameter `value`
    // bertipe `string?` membawa nilai nilai; nilai null diizinkan ketika data opsional belum tersedia. Nilai hasil langsung berasal dari hasil
    // pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(value)` benar gunakan `null`, jika tidak gunakan `value.Trim()`.
    private static string? NormalizeOptional(string? value) =>
        // Melanjutkan pengolahan dengan memeriksa apakah `value` null, kosong, atau hanya berisi karakter spasi dalam NormalizeOptional.
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    // Mendefinisikan metode `IncrementCount` dengan hasil bertipe `void`; operasi ini menangani increment jumlah. Masukan: Parameter `counts` bertipe
    // `Dictionary<string, int>` membawa nilai counts; Parameter `code` bertipe `string` membawa nilai kode.
    private static void IncrementCount(Dictionary<string, int> counts, string code)
    // Membuka scope metode IncrementCount; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IncrementCount.
    {
        // Menjalankan mencari kunci `code` pada `counts`; hasil boolean menandakan kunci ditemukan dan argumen out menerima nilainya dalam IncrementCount.
        counts.TryGetValue(code, out var count);
        // Memperbarui `counts[code]` menggunakan penjumlahan/penggabungan antara `count` dan `1` dalam IncrementCount.
        counts[code] = count + 1;
    // Menutup scope metode IncrementCount; bagian berikut berada di luar batas blok tersebut dalam IncrementCount.
    }

    // Mendefinisikan metode `ValidateOptionalCard` dengan hasil bertipe `void`; operasi ini menangani validate optional kartu. Masukan: Parameter
    // `code` bertipe `string?` membawa nilai kode; nilai null diizinkan ketika data opsional belum tersedia; Parameter `required` bertipe `bool`
    // membawa nilai required; Parameter `catalog` bertipe `IReadOnlyDictionary<string, T>` membawa nilai catalog; Parameter `counts` bertipe
    // `Dictionary<string, int>` membawa nilai counts; Parameter `field` bertipe `string` membawa nilai field; Parameter `getCode` bertipe `Func<T,
    // string>` membawa nilai get kode; Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
    private static void ValidateOptionalCard<T>(
        // Parameter `code` bertipe `string?` membawa nilai kode; nilai null diizinkan ketika data opsional belum tersedia.
        string? code,
        // Parameter `required` bertipe `bool` membawa nilai required.
        bool required,
        // Parameter `catalog` bertipe `IReadOnlyDictionary<string, T>` membawa nilai catalog.
        IReadOnlyDictionary<string, T> catalog,
        // Parameter `counts` bertipe `Dictionary<string, int>` membawa nilai counts.
        Dictionary<string, int> counts,
        // Parameter `field` bertipe `string` membawa nilai field.
        string field,
        // Parameter `getCode` bertipe `Func<T, string>` membawa nilai get kode.
        Func<T, string> getCode,
        // Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
        List<ErrorDetail> errors)
    // Membuka scope metode ValidateOptionalCard; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateOptionalCard.
    {
        // Memeriksa memeriksa apakah `code` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ValidateOptionalCard.
        if (string.IsNullOrWhiteSpace(code))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(code)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateOptionalCard.
        {
            // Memeriksa `required` (nilai required); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateOptionalCard.
            if (required)
            // Membuka scope cabang if untuk kondisi `required`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateOptionalCard.
            {
                // Menjalankan menambahkan `new ErrorDetail(field, ”REQUIRED”)` ke `errors` dalam ValidateOptionalCard.
                errors.Add(new ErrorDetail(field, "REQUIRED"));
            // Menutup scope cabang if untuk kondisi `required`; bagian berikut berada di luar batas blok tersebut dalam ValidateOptionalCard.
            }

            // Mengakhiri eksekusi lebih awal dalam ValidateOptionalCard tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(code)`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateOptionalCard.
        }

        // Memeriksa kebalikan kondisi `required`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateOptionalCard.
        if (!required)
        // Membuka scope cabang if untuk kondisi `!required`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateOptionalCard.
        {
            // Menjalankan menambahkan `new ErrorDetail(field, ”DISALLOWED_FOR_MODE”)` ke `errors` dalam ValidateOptionalCard.
            errors.Add(new ErrorDetail(field, "DISALLOWED_FOR_MODE"));
            // Mengakhiri eksekusi lebih awal dalam ValidateOptionalCard tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!required`; bagian berikut berada di luar batas blok tersebut dalam ValidateOptionalCard.
        }

        // Memeriksa kebalikan kondisi `catalog.TryGetValue(code, out var item)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateOptionalCard.
        if (!catalog.TryGetValue(code, out var item))
        // Membuka scope cabang if untuk kondisi `!catalog.TryGetValue(code, out var item)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ValidateOptionalCard.
        {
            // Menjalankan menambahkan `new ErrorDetail(field, ”UNKNOWN_REFERENCE”)` ke `errors` dalam ValidateOptionalCard.
            errors.Add(new ErrorDetail(field, "UNKNOWN_REFERENCE"));
            // Mengakhiri eksekusi lebih awal dalam ValidateOptionalCard tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!catalog.TryGetValue(code, out var item)`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateOptionalCard.
        }

        // Menjalankan memanggil `IncrementCount` dengan `counts`, `getCode(item)` dalam ValidateOptionalCard.
        IncrementCount(counts, getCode(item));
    // Menutup scope metode ValidateOptionalCard; bagian berikut berada di luar batas blok tersebut dalam ValidateOptionalCard.
    }

    // Mendefinisikan metode `ValidatePhysicalCardQuantity` dengan hasil bertipe `void`; operasi ini menangani validate physical kartu jumlah. Masukan:
    // Parameter `assignedCounts` bertipe `IReadOnlyDictionary<string, int>` membawa nilai assigned counts; Parameter `catalog` bertipe
    // `IReadOnlyDictionary<string, T>` membawa nilai catalog; Parameter `getAvailableQuantity` bertipe `Func<T, int>` membawa nilai get tersedia
    // jumlah; Parameter `field` bertipe `string` membawa nilai field; Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
    private static void ValidatePhysicalCardQuantity<T>(
        // Parameter `assignedCounts` bertipe `IReadOnlyDictionary<string, int>` membawa nilai assigned counts.
        IReadOnlyDictionary<string, int> assignedCounts,
        // Parameter `catalog` bertipe `IReadOnlyDictionary<string, T>` membawa nilai catalog.
        IReadOnlyDictionary<string, T> catalog,
        // Parameter `getAvailableQuantity` bertipe `Func<T, int>` membawa nilai get tersedia jumlah.
        Func<T, int> getAvailableQuantity,
        // Parameter `field` bertipe `string` membawa nilai field.
        string field,
        // Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
        List<ErrorDetail> errors)
    // Membuka scope metode ValidatePhysicalCardQuantity; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ValidatePhysicalCardQuantity.
    {
        // Melengkapi struktur ekspresi ForEachVariableStatement melalui foreach (var (code, assignedCount) in assignedCounts) dalam
        // ValidatePhysicalCardQuantity; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
        foreach (var (code, assignedCount) in assignedCounts)
        // Membuka scope blok ForEachVariableStatement; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidatePhysicalCardQuantity.
        {
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `catalog.TryGetValue(code, out var item)` dan `assignedCount > Math.Max(0,
            // getAvailableQuantity(item))`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ValidatePhysicalCardQuantity.
            if (catalog.TryGetValue(code, out var item) && assignedCount > Math.Max(0, getAvailableQuantity(item)))
            // Membuka scope cabang if untuk kondisi `catalog.TryGetValue(code, out var item) && assignedCount > Math.Max(0, getAvailableQuantity(item))`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidatePhysicalCardQuantity.
            {
                // Menjalankan menambahkan `new ErrorDetail(field, ”CARD_QUANTITY_EXCEEDED”)` ke `errors` dalam ValidatePhysicalCardQuantity.
                errors.Add(new ErrorDetail(field, "CARD_QUANTITY_EXCEEDED"));
            // Menutup scope cabang if untuk kondisi `catalog.TryGetValue(code, out var item) && assignedCount > Math.Max(0, getAvailableQuantity(item))`;
            // bagian berikut berada di luar batas blok tersebut dalam ValidatePhysicalCardQuantity.
            }
        // Menutup scope blok ForEachVariableStatement; bagian berikut berada di luar batas blok tersebut dalam ValidatePhysicalCardQuantity.
        }
    // Menutup scope metode ValidatePhysicalCardQuantity; bagian berikut berada di luar batas blok tersebut dalam ValidatePhysicalCardQuantity.
    }
// Menutup scope tipe SessionSetupValidator; bagian berikut berada di luar batas blok tersebut.
}
