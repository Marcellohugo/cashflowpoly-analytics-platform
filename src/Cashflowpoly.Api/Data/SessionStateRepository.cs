// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk SessionStateRepository.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Data` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Data;

// Mendefinisikan tipe class `SessionStateRepository`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionStateRepository
// Membuka scope tipe SessionStateRepository; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `JsonSerializerOptions`: `JsonOptions` menyimpan nilai JSON options dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (JsonSerializerDefaults.Web). readonly membatasi penggantian referensi/nilai field pada deklarasi atau
    // konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    // Mendeklarasikan field bertipe `NpgsqlDataSource`: `_dataSource` menyimpan sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang
    // koneksi. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly NpgsqlDataSource _dataSource;
    // Mendeklarasikan field bertipe `RulesetRepository`: `_rulesets` menyimpan nilai aturan. readonly membatasi penggantian referensi/nilai field pada
    // deklarasi atau konstruktor.
    private readonly RulesetRepository _rulesets;
    // Mendeklarasikan field bertipe `EventRepository`: `_events` menyimpan kumpulan event permainan sebagai sumber riwayat untuk validasi atau
    // perhitungan. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly EventRepository _events;
    // Mendeklarasikan field bertipe `IHappinessCalculator`: `_happiness` menyimpan nilai kebahagiaan. readonly membatasi penggantian referensi/nilai
    // field pada deklarasi atau konstruktor.
    private readonly IHappinessCalculator _happiness;

    // Mendefinisikan konstruktor SessionStateRepository yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi; Parameter
    // `rulesets` bertipe `RulesetRepository` membawa nilai aturan; Parameter `events` bertipe `EventRepository` membawa kumpulan event permainan
    // sebagai sumber riwayat untuk validasi atau perhitungan; Parameter `happiness` bertipe `IHappinessCalculator` membawa nilai kebahagiaan.
    public SessionStateRepository(
        // Parameter `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
        NpgsqlDataSource dataSource,
        // Parameter `rulesets` bertipe `RulesetRepository` membawa nilai aturan.
        RulesetRepository rulesets,
        // Parameter `events` bertipe `EventRepository` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan.
        EventRepository events,
        // Parameter `happiness` bertipe `IHappinessCalculator` membawa nilai kebahagiaan.
        IHappinessCalculator happiness)
    // Membuka scope konstruktor SessionStateRepository; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SessionStateRepository.
    {
        // Memperbarui `_dataSource` menggunakan `dataSource` (sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi) dalam
        // SessionStateRepository.
        _dataSource = dataSource;
        // Memperbarui `_rulesets` menggunakan `rulesets` (nilai aturan) dalam SessionStateRepository.
        _rulesets = rulesets;
        // Memperbarui `_events` menggunakan `events` (kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan) dalam
        // SessionStateRepository.
        _events = events;
        // Memperbarui `_happiness` menggunakan `happiness` (nilai kebahagiaan) dalam SessionStateRepository.
        _happiness = happiness;
    // Menutup scope konstruktor SessionStateRepository; bagian berikut berada di luar batas blok tersebut dalam SessionStateRepository.
    }

    // Mendefinisikan metode `GetRulesetSectionAsync` dengan hasil bertipe `Task<RulesetSectionDb?>`; operasi ini menangani get aturan section asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `mode` bertipe
    // `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan; Parameter `rulesetId` bertipe `Guid?` membawa identitas kumpulan
    // aturan permainan; nilai null diizinkan ketika data opsional belum tersedia; Parameter `instructorUserId` bertipe `Guid?` membawa identitas
    // instruktur pemilik sesi atau aturan; nilai null diizinkan ketika data opsional belum tersedia; Parameter `ct` bertipe `CancellationToken` membawa
    // sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<RulesetSectionDb?> GetRulesetSectionAsync(
        // Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan.
        string mode,
        // Parameter `rulesetId` bertipe `Guid?` membawa identitas kumpulan aturan permainan; nilai null diizinkan ketika data opsional belum tersedia.
        Guid? rulesetId,
        // Parameter `instructorUserId` bertipe `Guid?` membawa identitas instruktur pemilik sesi atau aturan; nilai null diizinkan ketika data opsional
        // belum tersedia.
        Guid? instructorUserId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode GetRulesetSectionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetRulesetSectionAsync.
    {
        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `requested` untuk nilai yang diminta dengan hasil operasi asinkron memanggil `ResolveRequestedRulesetVersionAsync`
        // dengan `conn`, `mode.ToUpperInvariant()`, `rulesetId`, `instructorUserId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var requested = await ResolveRequestedRulesetVersionAsync(
            // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke
            // `ResolveRequestedRulesetVersionAsync`.
            conn,
            // Meneruskan menormalisasi `mode` menjadi huruf besar dengan aturan kultur invariant sebagai argumen ke `ResolveRequestedRulesetVersionAsync`.
            mode.ToUpperInvariant(),
            // Meneruskan `rulesetId` (identitas kumpulan aturan permainan) sebagai argumen ke `ResolveRequestedRulesetVersionAsync`.
            rulesetId,
            // Meneruskan `instructorUserId` (identitas instruktur pemilik sesi atau aturan) sebagai argumen ke `ResolveRequestedRulesetVersionAsync`.
            instructorUserId,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // ke `ResolveRequestedRulesetVersionAsync`.
            ct);
        // Memeriksa hasil pencocokan `requested` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetRulesetSectionAsync.
        if (requested is null)
        // Membuka scope cabang if untuk kondisi `requested is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRulesetSectionAsync.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam GetRulesetSectionAsync; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `requested is null`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetSectionAsync.
        }

        // Menyiapkan variabel lokal `definition` untuk definisi terstruktur komponen serta parameter aturan permainan dengan hasil operasi asinkron
        // memanggil `_rulesets.GetRulesetDefinitionAsync` dengan `requested.RulesetVersionId`, `ct`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var definition = await _rulesets.GetRulesetDefinitionAsync(requested.RulesetVersionId, ct);
        // Memeriksa hasil pencocokan `definition` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetRulesetSectionAsync.
        if (definition is null)
        // Membuka scope cabang if untuk kondisi `definition is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRulesetSectionAsync.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam GetRulesetSectionAsync; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `definition is null`; bagian berikut berada di luar batas blok tersebut dalam GetRulesetSectionAsync.
        }

        // Mengembalikan objek baru bertipe `RulesetSectionDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam GetRulesetSectionAsync;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new RulesetSectionDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRulesetSectionAsync.
        {
            // Memperbarui `RulesetId` menggunakan `requested.RulesetId` (identitas kumpulan aturan permainan) dalam GetRulesetSectionAsync.
            RulesetId = requested.RulesetId,
            // Memperbarui `RulesetVersionId` menggunakan `requested.RulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan
            // yang tepat) dalam GetRulesetSectionAsync.
            RulesetVersionId = requested.RulesetVersionId,
            // Memperbarui `Mode` menggunakan `requested.Mode` (mode permainan yang menentukan kelompok aturan yang digunakan) dalam GetRulesetSectionAsync.
            Mode = requested.Mode,
            // Memperbarui `Definition` menggunakan `definition` (definisi terstruktur komponen serta parameter aturan permainan) dalam GetRulesetSectionAsync.
            Definition = definition
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam GetRulesetSectionAsync.
        };
    // Menutup scope metode GetRulesetSectionAsync; bagian berikut berada di luar batas blok tersebut dalam GetRulesetSectionAsync.
    }

    // Mendefinisikan metode `GetRulesetSectionByVersionIdAsync` dengan hasil bertipe `Task<RulesetSectionDb?>`; operasi ini menangani get aturan
    // section berdasarkan versi identitas asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui
    // Task. Masukan: Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan; Parameter
    // `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; Parameter
    // `instructorUserId` bertipe `Guid?` membawa identitas instruktur pemilik sesi atau aturan; nilai null diizinkan ketika data opsional belum
    // tersedia; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
    // permintaan atau aplikasi berhenti.
    public async Task<RulesetSectionDb?> GetRulesetSectionByVersionIdAsync(
        // Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan.
        string mode,
        // Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat.
        Guid rulesetVersionId,
        // Parameter `instructorUserId` bertipe `Guid?` membawa identitas instruktur pemilik sesi atau aturan; nilai null diizinkan ketika data opsional
        // belum tersedia.
        Guid? instructorUserId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode GetRulesetSectionByVersionIdAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // GetRulesetSectionByVersionIdAsync.
    {
        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `requested` untuk nilai yang diminta dengan hasil operasi asinkron memanggil `ResolveRulesetVersionByIdAsync` dengan
        // `conn`, `mode.ToUpperInvariant()`, `rulesetVersionId`, `instructorUserId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var requested = await ResolveRulesetVersionByIdAsync(
            // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `ResolveRulesetVersionByIdAsync`.
            conn,
            // Meneruskan menormalisasi `mode` menjadi huruf besar dengan aturan kultur invariant sebagai argumen ke `ResolveRulesetVersionByIdAsync`.
            mode.ToUpperInvariant(),
            // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
            // `ResolveRulesetVersionByIdAsync`.
            rulesetVersionId,
            // Meneruskan `instructorUserId` (identitas instruktur pemilik sesi atau aturan) sebagai argumen ke `ResolveRulesetVersionByIdAsync`.
            instructorUserId,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // ke `ResolveRulesetVersionByIdAsync`.
            ct);
        // Memeriksa hasil pencocokan `requested` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetRulesetSectionByVersionIdAsync.
        if (requested is null)
        // Membuka scope cabang if untuk kondisi `requested is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRulesetSectionByVersionIdAsync.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam GetRulesetSectionByVersionIdAsync; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `requested is null`; bagian berikut berada di luar batas blok tersebut dalam
        // GetRulesetSectionByVersionIdAsync.
        }

        // Menyiapkan variabel lokal `definition` untuk definisi terstruktur komponen serta parameter aturan permainan dengan hasil operasi asinkron
        // memanggil `_rulesets.GetRulesetDefinitionAsync` dengan `requested.RulesetVersionId`, `ct`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var definition = await _rulesets.GetRulesetDefinitionAsync(requested.RulesetVersionId, ct);
        // Memeriksa hasil pencocokan `definition` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetRulesetSectionByVersionIdAsync.
        if (definition is null)
        // Membuka scope cabang if untuk kondisi `definition is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRulesetSectionByVersionIdAsync.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam GetRulesetSectionByVersionIdAsync; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `definition is null`; bagian berikut berada di luar batas blok tersebut dalam
        // GetRulesetSectionByVersionIdAsync.
        }

        // Mengembalikan objek baru bertipe `RulesetSectionDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam
        // GetRulesetSectionByVersionIdAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new RulesetSectionDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetRulesetSectionByVersionIdAsync.
        {
            // Memperbarui `RulesetId` menggunakan `requested.RulesetId` (identitas kumpulan aturan permainan) dalam GetRulesetSectionByVersionIdAsync.
            RulesetId = requested.RulesetId,
            // Memperbarui `RulesetVersionId` menggunakan `requested.RulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan
            // yang tepat) dalam GetRulesetSectionByVersionIdAsync.
            RulesetVersionId = requested.RulesetVersionId,
            // Memperbarui `Mode` menggunakan `requested.Mode` (mode permainan yang menentukan kelompok aturan yang digunakan) dalam
            // GetRulesetSectionByVersionIdAsync.
            Mode = requested.Mode,
            // Memperbarui `Definition` menggunakan `definition` (definisi terstruktur komponen serta parameter aturan permainan) dalam
            // GetRulesetSectionByVersionIdAsync.
            Definition = definition
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // GetRulesetSectionByVersionIdAsync.
        };
    // Menutup scope metode GetRulesetSectionByVersionIdAsync; bagian berikut berada di luar batas blok tersebut dalam
    // GetRulesetSectionByVersionIdAsync.
    }

    // Mendefinisikan metode `InitializeSetupAsync` dengan hasil bertipe `Task<(long NextSequenceNumber, Guid FirstPlayerId)>`; operasi ini menangani
    // initialize setup asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan:
    // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tx`
    // bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi
    // aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; Parameter `mode` bertipe `string` membawa mode permainan yang menentukan
    // kelompok aturan yang digunakan; Parameter `definition` bertipe `RulesetDefinitionDto` membawa definisi terstruktur komponen serta parameter
    // aturan permainan; Parameter `players` bertipe `IReadOnlyList<SetupParticipant>` membawa nilai pemain; Parameter `setupRequest` bertipe
    // `SessionSetupRequest` membawa nilai setup permintaan; Parameter `timestamp` bertipe `DateTimeOffset` membawa waktu kejadian yang menjaga urutan
    // kronologis data; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
    // permintaan atau aplikasi berhenti.
    private static async Task<(long NextSequenceNumber, Guid FirstPlayerId)> InitializeSetupAsync(
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat.
        Guid rulesetVersionId,
        // Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan.
        string mode,
        // Parameter `definition` bertipe `RulesetDefinitionDto` membawa definisi terstruktur komponen serta parameter aturan permainan.
        RulesetDefinitionDto definition,
        // Parameter `players` bertipe `IReadOnlyList<SetupParticipant>` membawa nilai pemain.
        IReadOnlyList<SetupParticipant> players,
        // Parameter `setupRequest` bertipe `SessionSetupRequest` membawa nilai setup permintaan.
        SessionSetupRequest setupRequest,
        // Parameter `timestamp` bertipe `DateTimeOffset` membawa waktu kejadian yang menjaga urutan kronologis data.
        DateTimeOffset timestamp,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode InitializeSetupAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam InitializeSetupAsync.
    {
        // Menyiapkan variabel lokal `orderedPlayers` untuk nilai ordered pemain dengan mematerialisasi urutan `players.OrderBy(item => item.PlayerOrder)`
        // menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var orderedPlayers = players.OrderBy(item => item.PlayerOrder).ToList();
        // Memeriksa perbandingan kesamaan antara `orderedPlayers.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // InitializeSetupAsync.
        if (orderedPlayers.Count == 0)
        // Membuka scope cabang if untuk kondisi `orderedPlayers.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // InitializeSetupAsync.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Session wajib memiliki pemain sebelum setup
            // dijalankan.”) dalam InitializeSetupAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Session wajib memiliki pemain sebelum setup dijalankan.");
        // Menutup scope cabang if untuk kondisi `orderedPlayers.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam InitializeSetupAsync.
        }

        // Menyiapkan variabel lokal `ingredients` untuk nilai bahan dengan mematerialisasi urutan `definition.Ingredients .Where(item =>
        // !string.IsNullOrWhiteSpace(item.Id)) .DistinctBy(item => item.Id, StringComparer.OrdinalIgnoreCase)` menjadi List; enumerasi dijalankan dan
        // hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ingredients = definition.Ingredients
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => !string.IsNullOrWhiteSpace(item.Id)) dalam InitializeSetupAsync;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => !string.IsNullOrWhiteSpace(item.Id))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .DistinctBy(item => item.Id, StringComparer.OrdinalIgnoreCase) dalam
            // InitializeSetupAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .DistinctBy(item => item.Id, StringComparer.OrdinalIgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam InitializeSetupAsync; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `missions` untuk nilai misi dengan membangun kamus dari `definition.CollectionMissions .Where(item =>
        // !string.IsNullOrWhiteSpace(item.Id)) .DistinctBy(item => item.Id, StringComparer.OrdinalIgnoreCase)` dengan pemilihan kunci/nilai `item =>
        // item.Id`, `StringComparer.OrdinalIgnoreCase`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var missions = definition.CollectionMissions
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => !string.IsNullOrWhiteSpace(item.Id)) dalam InitializeSetupAsync;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => !string.IsNullOrWhiteSpace(item.Id))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .DistinctBy(item => item.Id, StringComparer.OrdinalIgnoreCase) dalam
            // InitializeSetupAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .DistinctBy(item => item.Id, StringComparer.OrdinalIgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary(item => item.Id, StringComparer.OrdinalIgnoreCase); dalam
            // InitializeSetupAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(item => item.Id, StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `tieBreakers` untuk nilai tie breakers dengan membangun kamus dari `definition.TieBreakers .Where(item =>
        // !string.IsNullOrWhiteSpace(item.TieBreakerCode) && item.TieNumber >= 1) .DistinctBy(item => item.TieBreakerCode, StringComparer.OrdinalIg...`
        // dengan pemilihan kunci/nilai `item => item.TieBreakerCode`, `StringComparer.OrdinalIgnoreCase`; kunci harus unik agar konversi berhasil. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var tieBreakers = definition.TieBreakers
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => !string.IsNullOrWhiteSpace(item.TieBreakerCode) &&
            // item.TieNumber >= 1) dalam InitializeSetupAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => !string.IsNullOrWhiteSpace(item.TieBreakerCode) && item.TieNumber >= 1)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .DistinctBy(item => item.TieBreakerCode, StringComparer.OrdinalIgnoreCase)
            // dalam InitializeSetupAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .DistinctBy(item => item.TieBreakerCode, StringComparer.OrdinalIgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary(item => item.TieBreakerCode, StringComparer.OrdinalIgnoreCase);
            // dalam InitializeSetupAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(item => item.TieBreakerCode, StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `assignments` untuk nilai assignments dengan membangun kamus dari `setupRequest.Players` dengan pemilihan kunci/nilai
        // `item => item.SessionPlayerId`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var assignments = setupRequest.Players.ToDictionary(item => item.SessionPlayerId);
        // Menyiapkan variabel lokal `tieBreakerByPlayerId` untuk nilai tie breaker berdasarkan pemain identitas dengan membangun kamus dari `assignments`
        // dengan pemilihan kunci/nilai `item => item.Key`, `item => tieBreakers[item.Value.TieBreakerCode]`; kunci harus unik agar konversi berhasil. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var tieBreakerByPlayerId = assignments.ToDictionary(
            // Parameter `item` bertipe `` membawa nilai elemen.
            item => item.Key,
            // Parameter `item` bertipe `` membawa nilai elemen.
            item => tieBreakers[item.Value.TieBreakerCode]);
        // Menyiapkan variabel lokal `firstPlayerId` untuk nilai first pemain identitas dengan `tieBreakerByPlayerId.Single(item => item.Value.TieNumber ==
        // 1).Key` (nilai kunci). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var firstPlayerId = tieBreakerByPlayerId.Single(item => item.Value.TieNumber == 1).Key;
        // Menjalankan hasil operasi asinkron memanggil `ApplyTieBreakerTurnOrderAsync` dengan `conn`, `tx`, `sessionId`, `tieBreakerByPlayerId`, `ct`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam InitializeSetupAsync.
        await ApplyTieBreakerTurnOrderAsync(conn, tx, sessionId, tieBreakerByPlayerId, ct);

        // Menyiapkan variabel lokal `ingredientsById` untuk nilai bahan berdasarkan identitas dengan membangun kamus dari `ingredients` dengan pemilihan
        // kunci/nilai `item => item.Id`, `StringComparer.OrdinalIgnoreCase`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var ingredientsById = ingredients.ToDictionary(item => item.Id, StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `loansByCode` untuk nilai pinjaman berdasarkan kode dengan membangun kamus dari `definition.ShariaLoans` dengan
        // pemilihan kunci/nilai `item => item.LoanCode`, `StringComparer.OrdinalIgnoreCase`; kunci harus unik agar konversi berhasil. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var loansByCode = definition.ShariaLoans.ToDictionary(item => item.LoanCode, StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `insuranceByCode` untuk nilai asuransi berdasarkan kode dengan membangun kamus dari `definition.InsuranceProducts`
        // dengan pemilihan kunci/nilai `item => item.ProductCode`, `StringComparer.OrdinalIgnoreCase`; kunci harus unik agar konversi berhasil. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var insuranceByCode = definition.InsuranceProducts.ToDictionary(item => item.ProductCode, StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `isMahir` untuk nilai berstatus mahir dengan membandingkan kesamaan `string` dengan `mode`, `”MAHIR”`,
        // `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan comparer yang diberikan. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var isMahir = string.Equals(mode, "MAHIR", StringComparison.OrdinalIgnoreCase);

        // Menyiapkan variabel lokal `sequence` untuk nomor urut event yang menentukan urutan pemrosesan riwayat permainan dengan nilai literal `0`. Tipe
        // yang dipakai adalah `long`.
        long sequence = 0;
        // Menjalankan hasil operasi asinkron memanggil `InsertAndProjectSetupEventAsync` dengan `conn`, `tx`, `sessionId`, `rulesetVersionId`, `null`,
        // `null`, `sequence++`, `”MulaiSesi”`, `new { setup = ”INITIAL” }`, `timestamp`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai dalam InitializeSetupAsync.
        await InsertAndProjectSetupEventAsync(
            // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
            // Meneruskan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
            // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
            // `InsertAndProjectSetupEventAsync`; Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`;
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`; Meneruskan `sequence++` sebagai argumen ke
            // `InsertAndProjectSetupEventAsync`; Meneruskan nilai literal `”MulaiSesi”` sebagai argumen ke `InsertAndProjectSetupEventAsync`.
            conn, tx, sessionId, rulesetVersionId, null, null, sequence++, "MulaiSesi",
            // Meneruskan objek anonim yang mengelompokkan setup sebagai satu nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`; Meneruskan `timestamp`
            // (waktu kejadian yang menjaga urutan kronologis data) sebagai argumen ke `InsertAndProjectSetupEventAsync`; Meneruskan `ct` (sinyal pembatalan
            // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen ke
            // `InsertAndProjectSetupEventAsync`.
            new { setup = "INITIAL" }, timestamp, ct);

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < orderedPlayers.Count`, lalu memperbarui pencacah melalui `index++`
        // dalam InitializeSetupAsync.
        for (var index = 0; index < orderedPlayers.Count; index++)
        // Membuka scope loop dengan syarat `index < orderedPlayers.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // InitializeSetupAsync.
        {
            // Menyiapkan variabel lokal `player` untuk nilai pemain dengan `orderedPlayers[index]`, yaitu elemen koleksi yang dipilih melalui indeks atau kunci
            // tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var player = orderedPlayers[index];
            // Menyiapkan variabel lokal `assignment` untuk nilai assignment dengan `assignments[player.SessionParticipantId]`, yaitu elemen koleksi yang
            // dipilih melalui indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var assignment = assignments[player.SessionParticipantId];
            // Menyiapkan variabel lokal `tieBreaker` untuk nilai tie breaker dengan `tieBreakerByPlayerId[player.SessionParticipantId]`, yaitu elemen koleksi
            // yang dipilih melalui indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var tieBreaker = tieBreakerByPlayerId[player.SessionParticipantId];
            // Menyiapkan variabel lokal `ingredient` untuk nilai bahan dengan `ingredientsById[assignment.IngredientCardId]`, yaitu elemen koleksi yang dipilih
            // melalui indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var ingredient = ingredientsById[assignment.IngredientCardId];
            // Menyiapkan variabel lokal `mission` untuk nilai misi dengan `missions[assignment.MissionId!]`, yaitu elemen koleksi yang dipilih melalui indeks
            // atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var mission = missions[assignment.MissionId!];
            // Menyiapkan variabel lokal `targetFamily` untuk nilai target kelompok dengan `mission.KebutuhanTarget.FirstOrDefault(item =>
            // item.Type.Equals(”FAMILY”, StringComparison.OrdinalIgnoreCase) || item.Type.Equals(”NEED_FAMILY”, StringComparison.OrdinalIgnore...` bila tidak
            // null; jika null gunakan `mission.Nama` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var targetFamily = mission.KebutuhanTarget.FirstOrDefault(item =>
                    // Meneruskan nilai literal `”FAMILY”` sebagai argumen ke `item.Type.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore
                    // case) sebagai argumen ke `item.Type.Equals`.
                    item.Type.Equals("FAMILY", StringComparison.OrdinalIgnoreCase) ||
                    // Meneruskan nilai literal `”NEED_FAMILY”` sebagai argumen ke `item.Type.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal
                    // ignore case) sebagai argumen ke `item.Type.Equals`.
                    item.Type.Equals("NEED_FAMILY", StringComparison.OrdinalIgnoreCase))
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: .Value ?? mission.Nama; dalam InitializeSetupAsync.
                ?.Value ?? mission.Nama;
            // Menyiapkan variabel lokal `requirePrimary` untuk nilai require primary dengan memeriksa apakah `mission.KebutuhanTarget` memiliki setidaknya satu
            // elemen yang memenuhi `item => (item.Type.Equals(”TIER”, StringComparison.OrdinalIgnoreCase) || item.Type.Equals(”NEED_TIER”,
            // StringComparison.OrdinalIgnoreCase)) && item.Value.Equals(”primer”, Stri...`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var requirePrimary = mission.KebutuhanTarget.Any(item =>
                // Meneruskan nilai literal `”TIER”` sebagai argumen ke `item.Type.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore
                // case) sebagai argumen ke `item.Type.Equals`.
                (item.Type.Equals("TIER", StringComparison.OrdinalIgnoreCase) ||
                 // Meneruskan nilai literal `”NEED_TIER”` sebagai argumen ke `item.Type.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal
                 // ignore case) sebagai argumen ke `item.Type.Equals`.
                 item.Type.Equals("NEED_TIER", StringComparison.OrdinalIgnoreCase)) &&
                // Meneruskan nilai literal `”primer”` sebagai argumen ke `item.Value.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore
                // case) sebagai argumen ke `item.Value.Equals`.
                item.Value.Equals("primer", StringComparison.OrdinalIgnoreCase));
            // Menyiapkan variabel lokal `requireSecondary` untuk nilai require secondary dengan memeriksa apakah `mission.KebutuhanTarget` memiliki setidaknya
            // satu elemen yang memenuhi `item => (item.Type.Equals(”TIER”, StringComparison.OrdinalIgnoreCase) || item.Type.Equals(”NEED_TIER”,
            // StringComparison.OrdinalIgnoreCase)) && item.Value.Equals(”sekunder”, St...`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var requireSecondary = mission.KebutuhanTarget.Any(item =>
                // Meneruskan nilai literal `”TIER”` sebagai argumen ke `item.Type.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore
                // case) sebagai argumen ke `item.Type.Equals`.
                (item.Type.Equals("TIER", StringComparison.OrdinalIgnoreCase) ||
                 // Meneruskan nilai literal `”NEED_TIER”` sebagai argumen ke `item.Type.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal
                 // ignore case) sebagai argumen ke `item.Type.Equals`.
                 item.Type.Equals("NEED_TIER", StringComparison.OrdinalIgnoreCase)) &&
                // Meneruskan nilai literal `”sekunder”` sebagai argumen ke `item.Value.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal
                // ignore case) sebagai argumen ke `item.Value.Equals`.
                item.Value.Equals("sekunder", StringComparison.OrdinalIgnoreCase));

            // Menjalankan hasil operasi asinkron memanggil `InsertAndProjectSetupEventAsync` dengan `conn`, `tx`, `sessionId`, `rulesetVersionId`,
            // `player.SessionParticipantId`, `player.UserId`, `sequence++`, `”BagikanTieBreaker”`, `new { number = tieBreaker.TieNumber, card_code =
            // tieBreaker.TieBreakerCode }`, `timestamp`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // InitializeSetupAsync.
            await InsertAndProjectSetupEventAsync(
                // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`; Meneruskan `player.SessionParticipantId` (identitas keikutsertaan pemain pada sesi tertentu) sebagai argumen
                // ke `InsertAndProjectSetupEventAsync`; Meneruskan `player.UserId` (identitas akun pengguna yang datanya sedang diproses) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                conn, tx, sessionId, rulesetVersionId, player.SessionParticipantId, player.UserId,
                // Meneruskan `sequence++` sebagai argumen ke `InsertAndProjectSetupEventAsync`; Meneruskan nilai literal `”BagikanTieBreaker”` sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                sequence++, "BagikanTieBreaker",
                // Meneruskan objek anonim yang mengelompokkan number, card_code sebagai satu nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`.
                new { number = tieBreaker.TieNumber, card_code = tieBreaker.TieBreakerCode },
                // Meneruskan `timestamp` (waktu kejadian yang menjaga urutan kronologis data) sebagai argumen ke `InsertAndProjectSetupEventAsync`; Meneruskan `ct`
                // (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                timestamp, ct);
            // Menjalankan hasil operasi asinkron memanggil `InsertAndProjectSetupEventAsync` dengan `conn`, `tx`, `sessionId`, `rulesetVersionId`,
            // `player.SessionParticipantId`, `player.UserId`, `sequence++`, `”SetupBahanAwal”`, `new { card_id = ingredient.Id, ingredient_name =
            // ingredient.Nama, amount = ingredient.HargaBeli, setup = ”INITIAL” }`, `timestamp`, `ct`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai dalam InitializeSetupAsync.
            await InsertAndProjectSetupEventAsync(
                // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`; Meneruskan `player.SessionParticipantId` (identitas keikutsertaan pemain pada sesi tertentu) sebagai argumen
                // ke `InsertAndProjectSetupEventAsync`; Meneruskan `player.UserId` (identitas akun pengguna yang datanya sedang diproses) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                conn, tx, sessionId, rulesetVersionId, player.SessionParticipantId, player.UserId,
                // Meneruskan `sequence++` sebagai argumen ke `InsertAndProjectSetupEventAsync`; Meneruskan nilai literal `”SetupBahanAwal”` sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                sequence++, "SetupBahanAwal",
                // Meneruskan objek anonim yang mengelompokkan card_id, ingredient_name, amount, setup sebagai satu nilai sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                new { card_id = ingredient.Id, ingredient_name = ingredient.Nama, amount = ingredient.HargaBeli, setup = "INITIAL" },
                // Meneruskan `timestamp` (waktu kejadian yang menjaga urutan kronologis data) sebagai argumen ke `InsertAndProjectSetupEventAsync`; Meneruskan `ct`
                // (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                timestamp, ct);
            // Menjalankan hasil operasi asinkron memanggil `InsertAndProjectSetupEventAsync` dengan `conn`, `tx`, `sessionId`, `rulesetVersionId`,
            // `player.SessionParticipantId`, `player.UserId`, `sequence++`, `”SetupEmasAwal”`, `new { qty = assignment.GoldQuantity, asset_code = ”gold_card”,
            // unit_value = 5, setup = ”INITIAL” }`, `timestamp`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // InitializeSetupAsync.
            await InsertAndProjectSetupEventAsync(
                // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`; Meneruskan `player.SessionParticipantId` (identitas keikutsertaan pemain pada sesi tertentu) sebagai argumen
                // ke `InsertAndProjectSetupEventAsync`; Meneruskan `player.UserId` (identitas akun pengguna yang datanya sedang diproses) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                conn, tx, sessionId, rulesetVersionId, player.SessionParticipantId, player.UserId,
                // Meneruskan `sequence++` sebagai argumen ke `InsertAndProjectSetupEventAsync`; Meneruskan nilai literal `”SetupEmasAwal”` sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                sequence++, "SetupEmasAwal",
                // Meneruskan objek anonim yang mengelompokkan qty, asset_code, unit_value, setup sebagai satu nilai sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                new { qty = assignment.GoldQuantity, asset_code = "gold_card", unit_value = 5, setup = "INITIAL" },
                // Meneruskan `timestamp` (waktu kejadian yang menjaga urutan kronologis data) sebagai argumen ke `InsertAndProjectSetupEventAsync`; Meneruskan `ct`
                // (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                timestamp, ct);
            // Menjalankan hasil operasi asinkron memanggil `InsertAndProjectSetupEventAsync` dengan `conn`, `tx`, `sessionId`, `rulesetVersionId`,
            // `player.SessionParticipantId`, `player.UserId`, `sequence++`, `”SetupMisiAwal”`, `new { mission_id = mission.Id, target_tertiary_card_id =
            // targetFamily, penalty_points = mission.PenaltyPoints, require_primary = requirePrimary, require_secondary = requireSec...`, `timestamp`, `ct`;
            // await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam InitializeSetupAsync.
            await InsertAndProjectSetupEventAsync(
                // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`; Meneruskan `player.SessionParticipantId` (identitas keikutsertaan pemain pada sesi tertentu) sebagai argumen
                // ke `InsertAndProjectSetupEventAsync`; Meneruskan `player.UserId` (identitas akun pengguna yang datanya sedang diproses) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                conn, tx, sessionId, rulesetVersionId, player.SessionParticipantId, player.UserId,
                // Meneruskan `sequence++` sebagai argumen ke `InsertAndProjectSetupEventAsync`; Meneruskan nilai literal `”SetupMisiAwal”` sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                sequence++, "SetupMisiAwal",
                // Meneruskan objek anonim yang mengelompokkan mission_id, target_tertiary_card_id, penalty_points, require_primary, require_secondary, setup
                // sebagai satu nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // InitializeSetupAsync.
                {
                    // Meneruskan objek anonim yang mengelompokkan mission_id, target_tertiary_card_id, penalty_points, require_primary, require_secondary, setup
                    // sebagai satu nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`.
                    mission_id = mission.Id,
                    // Meneruskan objek anonim yang mengelompokkan mission_id, target_tertiary_card_id, penalty_points, require_primary, require_secondary, setup
                    // sebagai satu nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`.
                    target_tertiary_card_id = targetFamily,
                    // Meneruskan objek anonim yang mengelompokkan mission_id, target_tertiary_card_id, penalty_points, require_primary, require_secondary, setup
                    // sebagai satu nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`.
                    penalty_points = mission.PenaltyPoints,
                    // Meneruskan objek anonim yang mengelompokkan mission_id, target_tertiary_card_id, penalty_points, require_primary, require_secondary, setup
                    // sebagai satu nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`.
                    require_primary = requirePrimary,
                    // Meneruskan objek anonim yang mengelompokkan mission_id, target_tertiary_card_id, penalty_points, require_primary, require_secondary, setup
                    // sebagai satu nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`.
                    require_secondary = requireSecondary,
                    // Meneruskan objek anonim yang mengelompokkan mission_id, target_tertiary_card_id, penalty_points, require_primary, require_secondary, setup
                    // sebagai satu nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`.
                    setup = "INITIAL"
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam InitializeSetupAsync.
                },
                // Meneruskan `timestamp` (waktu kejadian yang menjaga urutan kronologis data) sebagai argumen ke `InsertAndProjectSetupEventAsync`; Meneruskan `ct`
                // (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                timestamp, ct);

            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!isMahir || string.IsNullOrWhiteSpace(assignment.LoanCode)` dan
            // `string.IsNullOrWhiteSpace(assignment.InsuranceProductCode)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika
            // kondisi ini bernilai benar dalam InitializeSetupAsync.
            if (!isMahir || string.IsNullOrWhiteSpace(assignment.LoanCode) || string.IsNullOrWhiteSpace(assignment.InsuranceProductCode))
            // Membuka scope cabang if untuk kondisi `!isMahir || string.IsNullOrWhiteSpace(assignment.LoanCode) ||
            // string.IsNullOrWhiteSpace(assignment.InsuranceProductCode)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // InitializeSetupAsync.
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam InitializeSetupAsync.
                continue;
            // Menutup scope cabang if untuk kondisi `!isMahir || string.IsNullOrWhiteSpace(assignment.LoanCode) ||
            // string.IsNullOrWhiteSpace(assignment.InsuranceProductCode)`; bagian berikut berada di luar batas blok tersebut dalam InitializeSetupAsync.
            }

            // Menyiapkan variabel lokal `loan` untuk nilai pinjaman dengan `loansByCode[assignment.LoanCode]`, yaitu elemen koleksi yang dipilih melalui indeks
            // atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var loan = loansByCode[assignment.LoanCode];
            // Menyiapkan variabel lokal `insurance` untuk nilai asuransi dengan `insuranceByCode[assignment.InsuranceProductCode]`, yaitu elemen koleksi yang
            // dipilih melalui indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var insurance = insuranceByCode[assignment.InsuranceProductCode];
            // Menyiapkan variabel lokal `loanInstanceId` untuk nilai pinjaman instance identitas dengan teks interpolasi
            // `$”{loan.LoanCode}:setup:{player.SessionParticipantId:N}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan. Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var loanInstanceId = $"{loan.LoanCode}:setup:{player.SessionParticipantId:N}";
            // Menjalankan hasil operasi asinkron memanggil `InsertAndProjectSetupEventAsync` dengan `conn`, `tx`, `sessionId`, `rulesetVersionId`,
            // `player.SessionParticipantId`, `player.UserId`, `sequence++`, `”SetupPinjamanAwal”`, `new { loan_code = loan.LoanCode, loan_id = loanInstanceId,
            // principal = loan.Principal, repayment_amount = loan.RepaymentAmount, duration_days = loan.DurationDays, penalty_poin...`, `timestamp`, `ct`;
            // await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam InitializeSetupAsync.
            await InsertAndProjectSetupEventAsync(
                // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`; Meneruskan `player.SessionParticipantId` (identitas keikutsertaan pemain pada sesi tertentu) sebagai argumen
                // ke `InsertAndProjectSetupEventAsync`; Meneruskan `player.UserId` (identitas akun pengguna yang datanya sedang diproses) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                conn, tx, sessionId, rulesetVersionId, player.SessionParticipantId, player.UserId,
                // Meneruskan `sequence++` sebagai argumen ke `InsertAndProjectSetupEventAsync`; Meneruskan nilai literal `”SetupPinjamanAwal”` sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                sequence++, "SetupPinjamanAwal",
                // Meneruskan objek anonim yang mengelompokkan loan_code, loan_id, principal, repayment_amount, duration_days, penalty_points, setup sebagai satu
                // nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // InitializeSetupAsync.
                {
                    // Meneruskan objek anonim yang mengelompokkan loan_code, loan_id, principal, repayment_amount, duration_days, penalty_points, setup sebagai satu
                    // nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`.
                    loan_code = loan.LoanCode,
                    // Meneruskan objek anonim yang mengelompokkan loan_code, loan_id, principal, repayment_amount, duration_days, penalty_points, setup sebagai satu
                    // nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`.
                    loan_id = loanInstanceId,
                    // Meneruskan objek anonim yang mengelompokkan loan_code, loan_id, principal, repayment_amount, duration_days, penalty_points, setup sebagai satu
                    // nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`.
                    principal = loan.Principal,
                    // Meneruskan objek anonim yang mengelompokkan loan_code, loan_id, principal, repayment_amount, duration_days, penalty_points, setup sebagai satu
                    // nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`.
                    repayment_amount = loan.RepaymentAmount,
                    // Meneruskan objek anonim yang mengelompokkan loan_code, loan_id, principal, repayment_amount, duration_days, penalty_points, setup sebagai satu
                    // nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`.
                    duration_days = loan.DurationDays,
                    // Meneruskan objek anonim yang mengelompokkan loan_code, loan_id, principal, repayment_amount, duration_days, penalty_points, setup sebagai satu
                    // nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`.
                    penalty_points = loan.PenaltyPoints,
                    // Meneruskan objek anonim yang mengelompokkan loan_code, loan_id, principal, repayment_amount, duration_days, penalty_points, setup sebagai satu
                    // nilai sebagai argumen ke `InsertAndProjectSetupEventAsync`.
                    setup = "INITIAL"
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam InitializeSetupAsync.
                },
                // Meneruskan `timestamp` (waktu kejadian yang menjaga urutan kronologis data) sebagai argumen ke `InsertAndProjectSetupEventAsync`; Meneruskan `ct`
                // (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                timestamp, ct);
            // Menjalankan hasil operasi asinkron memanggil `InsertAndProjectSetupEventAsync` dengan `conn`, `tx`, `sessionId`, `rulesetVersionId`,
            // `player.SessionParticipantId`, `player.UserId`, `sequence++`, `”SetupAsuransiAwal”`, `new { product_code = insurance.ProductCode, policy_id =
            // $”{insurance.ProductCode}:setup:{player.SessionParticipantId:N}”, premium = 0, coverage_type = ”MULTIRISK”, setup = ”IN...`, `timestamp`, `ct`;
            // await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam InitializeSetupAsync.
            await InsertAndProjectSetupEventAsync(
                // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke `InsertAndProjectSetupEventAsync`;
                // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`; Meneruskan `player.SessionParticipantId` (identitas keikutsertaan pemain pada sesi tertentu) sebagai argumen
                // ke `InsertAndProjectSetupEventAsync`; Meneruskan `player.UserId` (identitas akun pengguna yang datanya sedang diproses) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                conn, tx, sessionId, rulesetVersionId, player.SessionParticipantId, player.UserId,
                // Meneruskan `sequence++` sebagai argumen ke `InsertAndProjectSetupEventAsync`; Meneruskan nilai literal `”SetupAsuransiAwal”` sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                sequence++, "SetupAsuransiAwal",
                // Meneruskan objek anonim yang mengelompokkan product_code, policy_id, premium, coverage_type, setup sebagai satu nilai sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // InitializeSetupAsync.
                {
                    // Meneruskan objek anonim yang mengelompokkan product_code, policy_id, premium, coverage_type, setup sebagai satu nilai sebagai argumen ke
                    // `InsertAndProjectSetupEventAsync`.
                    product_code = insurance.ProductCode,
                    // Meneruskan objek anonim yang mengelompokkan product_code, policy_id, premium, coverage_type, setup sebagai satu nilai sebagai argumen ke
                    // `InsertAndProjectSetupEventAsync`.
                    policy_id = $"{insurance.ProductCode}:setup:{player.SessionParticipantId:N}",
                    // Meneruskan objek anonim yang mengelompokkan product_code, policy_id, premium, coverage_type, setup sebagai satu nilai sebagai argumen ke
                    // `InsertAndProjectSetupEventAsync`.
                    premium = 0,
                    // Meneruskan objek anonim yang mengelompokkan product_code, policy_id, premium, coverage_type, setup sebagai satu nilai sebagai argumen ke
                    // `InsertAndProjectSetupEventAsync`.
                    coverage_type = "MULTIRISK",
                    // Meneruskan objek anonim yang mengelompokkan product_code, policy_id, premium, coverage_type, setup sebagai satu nilai sebagai argumen ke
                    // `InsertAndProjectSetupEventAsync`.
                    setup = "INITIAL"
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam InitializeSetupAsync.
                },
                // Meneruskan `timestamp` (waktu kejadian yang menjaga urutan kronologis data) sebagai argumen ke `InsertAndProjectSetupEventAsync`; Meneruskan `ct`
                // (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen ke
                // `InsertAndProjectSetupEventAsync`.
                timestamp, ct);
        // Menutup scope loop dengan syarat `index < orderedPlayers.Count`; bagian berikut berada di luar batas blok tersebut dalam InitializeSetupAsync.
        }

        // Mengembalikan tuple yang membawa bagian 1: sequence; bagian 2: firstPlayerId kepada pemanggil dalam InitializeSetupAsync; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return (sequence, firstPlayerId);
    // Menutup scope metode InitializeSetupAsync; bagian berikut berada di luar batas blok tersebut dalam InitializeSetupAsync.
    }

    // Mendefinisikan metode `ApplyTieBreakerTurnOrderAsync` dengan hasil bertipe `Task`; operasi ini menangani apply tie breaker giliran urutan/pesanan
    // asinkron. Masukan: Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data;
    // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `sessionId`
    // bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `tieBreakerByPlayerId` bertipe
    // `IReadOnlyDictionary<Guid, RulesetTieBreakerDto>` membawa nilai tie breaker berdasarkan pemain identitas; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static Task ApplyTieBreakerTurnOrderAsync(
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `tieBreakerByPlayerId` bertipe `IReadOnlyDictionary<Guid, RulesetTieBreakerDto>` membawa nilai tie breaker berdasarkan pemain
        // identitas.
        IReadOnlyDictionary<Guid, RulesetTieBreakerDto> tieBreakerByPlayerId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ApplyTieBreakerTurnOrderAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ApplyTieBreakerTurnOrderAsync.
    {
        // Mengembalikan menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” set constraints
        // uq_session_participants_session_seat deferred; update session_participants participant set player_order_no = assignment.tie_number f...`; nilai
        // hasil menunjukkan jumlah baris yang terpengaruh kepada pemanggil dalam ApplyTieBreakerTurnOrderAsync; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set constraints uq_session_participants_session_seat deferred;`.
            // Baris literal 3: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 4: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update session_participants
            // participant`.
            // Baris literal 5: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set player_order_no = assignment.tie_number`.
            // Baris literal 6: FROM memilih tabel/subquery sumber pembacaan: `from unnest(@participantIds::uuid[], @tieNumbers::int[]) as
            // assignment(session_participant_id, tie_number)`.
            // Baris literal 7: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where participant.session_id = @sessionId`.
            // Baris literal 8: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and participant.session_participant_id =
            // assignment.session_participant_id;`.
            // Baris literal 9: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            set constraints uq_session_participants_session_seat deferred;

            update session_participants participant
            set player_order_no = assignment.tie_number
            from unnest(@participantIds::uuid[], @tieNumbers::int[]) as assignment(session_participant_id, tie_number)
            where participant.session_id = @sessionId
              and participant.session_participant_id = assignment.session_participant_id;
            """,
            // Meneruskan objek anonim yang mengelompokkan sessionId, participantIds, tieNumbers sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ApplyTieBreakerTurnOrderAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantIds, tieNumbers sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                sessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantIds, tieNumbers sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                participantIds = tieBreakerByPlayerId.Keys.ToArray(),
                // Meneruskan fungsi lambda `item => item.TieNumber` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `tieBreakerByPlayerId.Values.Select`.
                tieNumbers = tieBreakerByPlayerId.Values.Select(item => item.TieNumber).ToArray()
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ApplyTieBreakerTurnOrderAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode ApplyTieBreakerTurnOrderAsync; bagian berikut berada di luar batas blok tersebut dalam ApplyTieBreakerTurnOrderAsync.
    }

    // Mendefinisikan metode `InsertAndProjectSetupEventAsync` dengan hasil bertipe `Task`; operasi ini menangani insert dan project setup event
    // asinkron. Masukan: Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data;
    // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `sessionId`
    // bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `rulesetVersionId` bertipe `Guid` membawa
    // identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; Parameter `sessionParticipantId` bertipe `Guid?` membawa
    // identitas keikutsertaan pemain pada sesi tertentu; nilai null diizinkan ketika data opsional belum tersedia; Parameter `userId` bertipe `Guid?`
    // membawa identitas akun pengguna yang datanya sedang diproses; nilai null diizinkan ketika data opsional belum tersedia; Parameter
    // `sequenceNumber` bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan; Parameter `actionType` bertipe
    // `string` membawa nilai aksi jenis; Parameter `payload` bertipe `object` membawa muatan detail event dalam format JSON; Parameter `timestamp`
    // bertipe `DateTimeOffset` membawa waktu kejadian yang menjaga urutan kronologis data; Parameter `ct` bertipe `CancellationToken` membawa sinyal
    // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static Task InsertAndProjectSetupEventAsync(
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat.
        Guid rulesetVersionId,
        // Parameter `sessionParticipantId` bertipe `Guid?` membawa identitas keikutsertaan pemain pada sesi tertentu; nilai null diizinkan ketika data
        // opsional belum tersedia.
        Guid? sessionParticipantId,
        // Parameter `userId` bertipe `Guid?` membawa identitas akun pengguna yang datanya sedang diproses; nilai null diizinkan ketika data opsional belum
        // tersedia.
        Guid? userId,
        // Parameter `sequenceNumber` bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan.
        long sequenceNumber,
        // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
        string actionType,
        // Parameter `payload` bertipe `object` membawa muatan detail event dalam format JSON.
        object payload,
        // Parameter `timestamp` bertipe `DateTimeOffset` membawa waktu kejadian yang menjaga urutan kronologis data.
        DateTimeOffset timestamp,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode InsertAndProjectSetupEventAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // InsertAndProjectSetupEventAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into events (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_pk, event_id,
        // session_id, session_player_id, user_id, actor_type, ”timestamp”,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `day_index, weekday,
        // turn_number, action_slot, sequence_number, ruleset_action_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_type,
        // ruleset_version_id, payload_version, payload, received_at, client_request_id`.
        // Baris literal 6: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 7: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 8: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@eventPk, @eventId, @sessionId, @sessionParticipantId, @userId, 'SYSTEM', @eventTimestamp,`.
        // Baris literal 9: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `0, 'MON', 0, 0, @sequenceNumber, ra.ruleset_action_id,`.
        // Baris literal 10: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@actionType, @rulesetVersionId, '1.0', @payload::jsonb, @eventTimestamp, @clientRequestId`.
        // Baris literal 11: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_actions ra`.
        // Baris literal 12: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ra.ruleset_version_id = @rulesetVersionId`.
        // Baris literal 13: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(ra.action_id) = lower(@actionType)`.
        // Baris literal 14: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ra.is_active`.
        // Baris literal 15: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1;`.
        // Baris literal 16: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 17: SELECT menentukan nilai atau kolom yang dikembalikan query: `select project_session_event(@eventPk);`.
        // Baris literal 18: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            insert into events (
                event_pk, event_id, session_id, session_player_id, user_id, actor_type, "timestamp",
                day_index, weekday, turn_number, action_slot, sequence_number, ruleset_action_id,
                action_type, ruleset_version_id, payload_version, payload, received_at, client_request_id
            )
            select
                @eventPk, @eventId, @sessionId, @sessionParticipantId, @userId, 'SYSTEM', @eventTimestamp,
                0, 'MON', 0, 0, @sequenceNumber, ra.ruleset_action_id,
                @actionType, @rulesetVersionId, '1.0', @payload::jsonb, @eventTimestamp, @clientRequestId
            from ruleset_actions ra
            where ra.ruleset_version_id = @rulesetVersionId
              and lower(ra.action_id) = lower(@actionType)
              and ra.is_active
            limit 1;

            select project_session_event(@eventPk);
            """;

        // Menyiapkan variabel lokal `eventPk` untuk nilai event pk dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var eventPk = Guid.NewGuid();
        // Mengembalikan menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( sql, new { eventPk, eventId = Guid.NewGuid(),
        // sessionId, sessionParticipantId, userId, eventTimestamp = timestamp.AddMilliseconds(sequenceNumber), seque...`; nilai hasil menunjukkan jumlah
        // baris yang terpengaruh kepada pemanggil dalam InsertAndProjectSetupEventAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`.
            sql,
            // Meneruskan objek anonim yang mengelompokkan eventPk, eventId, sessionId, sessionParticipantId, userId, eventTimestamp, sequenceNumber,
            // actionType, rulesetVersionId, payload, clientRequestId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // InsertAndProjectSetupEventAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan eventPk, eventId, sessionId, sessionParticipantId, userId, eventTimestamp, sequenceNumber,
                // actionType, rulesetVersionId, payload, clientRequestId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                eventPk,
                // Meneruskan objek anonim yang mengelompokkan eventPk, eventId, sessionId, sessionParticipantId, userId, eventTimestamp, sequenceNumber,
                // actionType, rulesetVersionId, payload, clientRequestId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                eventId = Guid.NewGuid(),
                // Meneruskan objek anonim yang mengelompokkan eventPk, eventId, sessionId, sessionParticipantId, userId, eventTimestamp, sequenceNumber,
                // actionType, rulesetVersionId, payload, clientRequestId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                sessionId,
                // Meneruskan objek anonim yang mengelompokkan eventPk, eventId, sessionId, sessionParticipantId, userId, eventTimestamp, sequenceNumber,
                // actionType, rulesetVersionId, payload, clientRequestId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                sessionParticipantId,
                // Meneruskan objek anonim yang mengelompokkan eventPk, eventId, sessionId, sessionParticipantId, userId, eventTimestamp, sequenceNumber,
                // actionType, rulesetVersionId, payload, clientRequestId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                userId,
                // Meneruskan `sequenceNumber` (nomor urut event yang menentukan urutan pemrosesan riwayat permainan) sebagai argumen ke
                // `timestamp.AddMilliseconds`.
                eventTimestamp = timestamp.AddMilliseconds(sequenceNumber),
                // Meneruskan objek anonim yang mengelompokkan eventPk, eventId, sessionId, sessionParticipantId, userId, eventTimestamp, sequenceNumber,
                // actionType, rulesetVersionId, payload, clientRequestId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                sequenceNumber,
                // Meneruskan objek anonim yang mengelompokkan eventPk, eventId, sessionId, sessionParticipantId, userId, eventTimestamp, sequenceNumber,
                // actionType, rulesetVersionId, payload, clientRequestId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                actionType,
                // Meneruskan objek anonim yang mengelompokkan eventPk, eventId, sessionId, sessionParticipantId, userId, eventTimestamp, sequenceNumber,
                // actionType, rulesetVersionId, payload, clientRequestId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                rulesetVersionId,
                // Meneruskan `payload` (muatan detail event dalam format JSON) sebagai argumen ke `JsonSerializer.Serialize`.
                payload = JsonSerializer.Serialize(payload),
                // Meneruskan objek anonim yang mengelompokkan eventPk, eventId, sessionId, sessionParticipantId, userId, eventTimestamp, sequenceNumber,
                // actionType, rulesetVersionId, payload, clientRequestId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                clientRequestId = $"setup:{sequenceNumber}:{actionType}"
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // InsertAndProjectSetupEventAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode InsertAndProjectSetupEventAsync; bagian berikut berada di luar batas blok tersebut dalam InsertAndProjectSetupEventAsync.
    }

    // Mendefinisikan metode `GetSessionSetupAsync` dengan hasil bertipe `Task<SessionSetupDb?>`; operasi ini menangani get sesi setup asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan
    // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<SessionSetupDb?> GetSessionSetupAsync(Guid sessionId, CancellationToken ct)
    // Membuka scope metode GetSessionSetupAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetSessionSetupAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id as SessionId,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `revision as Revision,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id as
        // RulesetVersionId,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `client_request_id as
        // ClientRequestId,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `setup_json::text as
        // SetupJson,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `saved_at as SavedAt,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `locked_at as LockedAt,`.
        // Baris literal 10: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_by_user_id as CreatedByUserId`.
        // Baris literal 11: FROM memilih tabel/subquery sumber pembacaan: `from session_setup_revisions`.
        // Baris literal 12: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 13: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by revision desc`.
        // Baris literal 14: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 15: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select
                session_id as SessionId,
                revision as Revision,
                ruleset_version_id as RulesetVersionId,
                client_request_id as ClientRequestId,
                setup_json::text as SetupJson,
                saved_at as SavedAt,
                locked_at as LockedAt,
                created_by_user_id as CreatedByUserId
            from session_setup_revisions
            where session_id = @sessionId
            order by revision desc
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<SessionSetupDb>` dengan `new
        // CommandDefinition(sql, new { sessionId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai kepada pemanggil dalam GetSessionSetupAsync; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return await conn.QuerySingleOrDefaultAsync<SessionSetupDb>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { sessionId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QuerySingleOrDefaultAsync<SessionSetupDb>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan
            // objek anonim yang mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal
            // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
    // Menutup scope metode GetSessionSetupAsync; bagian berikut berada di luar batas blok tersebut dalam GetSessionSetupAsync.
    }

    // Mendefinisikan metode `GetSessionSetupByClientRequestAsync` dengan hasil bertipe `Task<SessionSetupDb?>`; operasi ini menangani get sesi setup
    // berdasarkan client permintaan asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task.
    // Masukan: Parameter `createdByUserId` bertipe `Guid` membawa nilai created berdasarkan pengguna identitas; Parameter `clientRequestId` bertipe
    // `string` membawa identitas permintaan dari klien untuk pelacakan atau penanganan permintaan berulang; Parameter `ct` bertipe `CancellationToken`
    // membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<SessionSetupDb?> GetSessionSetupByClientRequestAsync(
        // Parameter `createdByUserId` bertipe `Guid` membawa nilai created berdasarkan pengguna identitas.
        Guid createdByUserId,
        // Parameter `clientRequestId` bertipe `string` membawa identitas permintaan dari klien untuk pelacakan atau penanganan permintaan berulang.
        string clientRequestId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode GetSessionSetupByClientRequestAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // GetSessionSetupByClientRequestAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id as SessionId,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `revision as Revision,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id as
        // RulesetVersionId,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `client_request_id as
        // ClientRequestId,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `setup_json::text as
        // SetupJson,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `saved_at as SavedAt,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `locked_at as LockedAt,`.
        // Baris literal 10: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_by_user_id as CreatedByUserId`.
        // Baris literal 11: FROM memilih tabel/subquery sumber pembacaan: `from session_setup_revisions`.
        // Baris literal 12: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where created_by_user_id = @createdByUserId`.
        // Baris literal 13: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and client_request_id = @clientRequestId`.
        // Baris literal 14: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 15: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select
                session_id as SessionId,
                revision as Revision,
                ruleset_version_id as RulesetVersionId,
                client_request_id as ClientRequestId,
                setup_json::text as SetupJson,
                saved_at as SavedAt,
                locked_at as LockedAt,
                created_by_user_id as CreatedByUserId
            from session_setup_revisions
            where created_by_user_id = @createdByUserId
              and client_request_id = @clientRequestId
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<SessionSetupDb>` dengan `new
        // CommandDefinition( sql, new { createdByUserId, clientRequestId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada pemanggil dalam GetSessionSetupByClientRequestAsync; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return await conn.QuerySingleOrDefaultAsync<SessionSetupDb>(new CommandDefinition(
            // Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`.
            sql,
            // Meneruskan objek anonim yang mengelompokkan createdByUserId, clientRequestId sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            new { createdByUserId, clientRequestId },
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode GetSessionSetupByClientRequestAsync; bagian berikut berada di luar batas blok tersebut dalam
    // GetSessionSetupByClientRequestAsync.
    }

    // Mendefinisikan metode `HasSessionSetupAsync` dengan hasil bertipe `Task<bool>`; operasi ini menangani memiliki sesi setup asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan
    // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<bool> HasSessionSetupAsync(Guid sessionId, CancellationToken ct)
    // Membuka scope metode HasSessionSetupAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam HasSessionSetupAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan nilai literal `”select exists(select 1 from session_setup_revisions where session_id =
        // @sessionId)”`. Tipe yang dipakai adalah `string`.
        const string sql = "select exists(select 1 from session_setup_revisions where session_id = @sessionId)";
        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Mengembalikan hasil operasi asinkron menjalankan perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { sessionId },
        // cancellationToken: ct)` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada
        // pemanggil dalam HasSessionSetupAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.ExecuteScalarAsync<bool>(new CommandDefinition(sql, new { sessionId }, cancellationToken: ct));
    // Menutup scope metode HasSessionSetupAsync; bagian berikut berada di luar batas blok tersebut dalam HasSessionSetupAsync.
    }

    // Mendefinisikan metode `CreateSessionSetupRevisionAsync` dengan hasil bertipe `Task<SessionSetupDb>`; operasi ini menangani create sesi setup
    // revision asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter
    // `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `rulesetVersionId` bertipe `Guid`
    // membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; Parameter `request` bertipe `SessionSetupRequest`
    // membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `createdByUserId` bertipe `Guid` membawa nilai created
    // berdasarkan pengguna identitas; Parameter `savedAt` bertipe `DateTimeOffset` membawa nilai saved at; Parameter `ct` bertipe `CancellationToken`
    // membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<SessionSetupDb> CreateSessionSetupRevisionAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat.
        Guid rulesetVersionId,
        // Parameter `request` bertipe `SessionSetupRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        SessionSetupRequest request,
        // Parameter `createdByUserId` bertipe `Guid` membawa nilai created berdasarkan pengguna identitas.
        Guid createdByUserId,
        // Parameter `savedAt` bertipe `DateTimeOffset` membawa nilai saved at.
        DateTimeOffset savedAt,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode CreateSessionSetupRevisionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // CreateSessionSetupRevisionAsync.
    {
        // Menyiapkan variabel lokal `lockSql` untuk nilai lock SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang
        // dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string lockSql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select status`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 5: FOR UPDATE mengunci baris hasil selama transaksi agar perubahan bersamaan tidak menimpa keadaan yang dibaca: `for update`.
        // Baris literal 6: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string lockSql = """
            select status
            from sessions
            where session_id = @sessionId
            for update
            """;

        // Menyiapkan variabel lokal `insertSql` untuk nilai insert SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang
        // dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string insertSql =
        // ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_setup_revisions (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `revision,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `client_request_id,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `setup_json,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `saved_at,`.
        // Baris literal 9: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_by_user_id`.
        // Baris literal 10: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 11: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 12: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionId,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(max(revision), 0) +
        // 1,`.
        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@rulesetVersionId,`.
        // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@clientRequestId,`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@setupJson::jsonb,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@savedAt,`.
        // Baris literal 18: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `@createdByUserId`.
        // Baris literal 19: FROM memilih tabel/subquery sumber pembacaan: `from session_setup_revisions`.
        // Baris literal 20: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 21: RETURNING mengembalikan kolom dari baris yang baru ditambahkan/diubah: `returning`.
        // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id as SessionId,`.
        // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `revision as Revision,`.
        // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id as
        // RulesetVersionId,`.
        // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `client_request_id as
        // ClientRequestId,`.
        // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `setup_json::text as
        // SetupJson,`.
        // Baris literal 27: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `saved_at as SavedAt,`.
        // Baris literal 28: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `locked_at as LockedAt,`.
        // Baris literal 29: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_by_user_id as CreatedByUserId`.
        // Baris literal 30: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertSql = """
            insert into session_setup_revisions (
                session_id,
                revision,
                ruleset_version_id,
                client_request_id,
                setup_json,
                saved_at,
                created_by_user_id
            )
            select
                @sessionId,
                coalesce(max(revision), 0) + 1,
                @rulesetVersionId,
                @clientRequestId,
                @setupJson::jsonb,
                @savedAt,
                @createdByUserId
            from session_setup_revisions
            where session_id = @sessionId
            returning
                session_id as SessionId,
                revision as Revision,
                ruleset_version_id as RulesetVersionId,
                client_request_id as ClientRequestId,
                setup_json::text as SetupJson,
                saved_at as SavedAt,
                locked_at as LockedAt,
                created_by_user_id as CreatedByUserId
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `tx` untuk transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan dengan hasil operasi asinkron
        // memulai transaksi pada `conn` menggunakan `ct` agar perubahan terkait dapat diselesaikan bersama; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        await using var tx = await conn.BeginTransactionAsync(ct);
        // Menyiapkan variabel lokal `status` untuk nilai status dengan hasil operasi asinkron membaca satu hasil basis data melalui
        // `conn.QuerySingleOrDefaultAsync<string>` dengan `new CommandDefinition( lockSql, new { sessionId }, tx, cancellationToken: ct)`; nilai default
        // menunjukkan tidak ada baris hasil; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var status = await conn.QuerySingleOrDefaultAsync<string>(new CommandDefinition(
            // Meneruskan `lockSql` (nilai lock SQL) sebagai argumen ke konstruktor `CommandDefinition`.
            lockSql,
            // Meneruskan objek anonim yang mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new { sessionId },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
        // Memeriksa kebalikan kondisi `string.Equals(status, ”CREATED”, StringComparison.OrdinalIgnoreCase)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam CreateSessionSetupRevisionAsync.
        if (!string.Equals(status, "CREATED", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `!string.Equals(status, ”CREATED”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam CreateSessionSetupRevisionAsync.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Pembagian awal tidak dapat diubah setelah sesi
            // dimulai”) dalam CreateSessionSetupRevisionAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Pembagian awal tidak dapat diubah setelah sesi dimulai");
        // Menutup scope cabang if untuk kondisi `!string.Equals(status, ”CREATED”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar
        // batas blok tersebut dalam CreateSessionSetupRevisionAsync.
        }

        // Menyiapkan variabel lokal `created` untuk nilai created dengan hasil operasi asinkron membaca tepat satu baris basis data melalui
        // `conn.QuerySingleAsync<SessionSetupDb>` dengan `new CommandDefinition( insertSql, new { sessionId, rulesetVersionId, clientRequestId =
        // request.ClientRequestId, setupJson = JsonSerializer.Serialize(request, JsonOptions), sav...`; jumlah baris selain satu menyebabkan exception;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var created = await conn.QuerySingleAsync<SessionSetupDb>(new CommandDefinition(
            // Meneruskan `insertSql` (nilai insert SQL) sebagai argumen ke konstruktor `CommandDefinition`.
            insertSql,
            // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, clientRequestId, setupJson, savedAt, createdByUserId sebagai satu nilai
            // sebagai argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // CreateSessionSetupRevisionAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, clientRequestId, setupJson, savedAt, createdByUserId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                sessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, clientRequestId, setupJson, savedAt, createdByUserId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                rulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, clientRequestId, setupJson, savedAt, createdByUserId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                clientRequestId = request.ClientRequestId,
                // Meneruskan `request` (data masukan permintaan yang akan divalidasi atau diteruskan ke layanan) sebagai argumen ke `JsonSerializer.Serialize`;
                // Meneruskan `JsonOptions` (nilai JSON options) sebagai argumen ke `JsonSerializer.Serialize`.
                setupJson = JsonSerializer.Serialize(request, JsonOptions),
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, clientRequestId, setupJson, savedAt, createdByUserId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                savedAt,
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, clientRequestId, setupJson, savedAt, createdByUserId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                createdByUserId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // CreateSessionSetupRevisionAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
        // Menjalankan hasil operasi asinkron mengesahkan transaksi `tx` sehingga perubahan yang terkumpul menjadi permanen; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai dalam CreateSessionSetupRevisionAsync.
        await tx.CommitAsync(ct);
        // Mengembalikan `created` (nilai created) kepada pemanggil dalam CreateSessionSetupRevisionAsync; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return created;
    // Menutup scope metode CreateSessionSetupRevisionAsync; bagian berikut berada di luar batas blok tersebut dalam CreateSessionSetupRevisionAsync.
    }

    // Mendefinisikan metode `DeserializeSetup` dengan hasil bertipe `SessionSetupRequest`; operasi ini menangani deserialize setup. Masukan: Parameter
    // `setup` bertipe `SessionSetupDb` membawa nilai setup.
    public static SessionSetupRequest DeserializeSetup(SessionSetupDb setup)
    // Membuka scope metode DeserializeSetup; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DeserializeSetup.
    {
        // Mengembalikan `JsonSerializer.Deserialize<SessionSetupRequest>(setup.SetupJson, JsonOptions)` bila tidak null; jika null gunakan `throw new
        // InvalidOperationException(”Data setup sesi tidak dapat dibaca.”)` sebagai nilai pengganti kepada pemanggil dalam DeserializeSetup; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return JsonSerializer.Deserialize<SessionSetupRequest>(setup.SetupJson, JsonOptions)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: throw new InvalidOperationException(”Data setup sesi tidak dapat
            // dibaca.”); dalam DeserializeSetup.
            ?? throw new InvalidOperationException("Data setup sesi tidak dapat dibaca.");
    // Menutup scope metode DeserializeSetup; bagian berikut berada di luar batas blok tersebut dalam DeserializeSetup.
    }

    // Mendefinisikan metode `StartSessionWithSetupAsync` dengan hasil bertipe `Task<long>`; operasi ini menangani start sesi dengan setup asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe
    // `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `mode` bertipe `string` membawa mode permainan yang
    // menentukan kelompok aturan yang digunakan; Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan
    // memakai konfigurasi aturan yang tepat; Parameter `definition` bertipe `RulesetDefinitionDto` membawa definisi terstruktur komponen serta
    // parameter aturan permainan; Parameter `setupRequest` bertipe `SessionSetupRequest` membawa nilai setup permintaan; Parameter `setupRevision`
    // bertipe `int` membawa nilai setup revision; Parameter `startedAt` bertipe `DateTimeOffset` membawa nilai started at; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<long> StartSessionWithSetupAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan.
        string mode,
        // Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat.
        Guid rulesetVersionId,
        // Parameter `definition` bertipe `RulesetDefinitionDto` membawa definisi terstruktur komponen serta parameter aturan permainan.
        RulesetDefinitionDto definition,
        // Parameter `setupRequest` bertipe `SessionSetupRequest` membawa nilai setup permintaan.
        SessionSetupRequest setupRequest,
        // Parameter `setupRevision` bertipe `int` membawa nilai setup revision.
        int setupRevision,
        // Parameter `startedAt` bertipe `DateTimeOffset` membawa nilai started at.
        DateTimeOffset startedAt,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode StartSessionWithSetupAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam StartSessionWithSetupAsync.
    {
        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `tx` untuk transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan dengan hasil operasi asinkron
        // memulai transaksi pada `conn` menggunakan `ct` agar perubahan terkait dapat diselesaikan bersama; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        await using var tx = await conn.BeginTransactionAsync(ct);

        // Menyiapkan variabel lokal `status` untuk nilai status dengan hasil operasi asinkron membaca satu hasil basis data melalui
        // `conn.QuerySingleOrDefaultAsync<string>` dengan `new CommandDefinition( ”select status from sessions where session_id = @sessionId for update”,
        // new { sessionId }, tx, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var status = await conn.QuerySingleOrDefaultAsync<string>(new CommandDefinition(
            // Meneruskan nilai literal `”select status from sessions where session_id = @sessionId for update”` sebagai argumen ke konstruktor
            // `CommandDefinition`.
            "select status from sessions where session_id = @sessionId for update",
            // Meneruskan objek anonim yang mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new { sessionId },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
        // Memeriksa kebalikan kondisi `string.Equals(status, ”CREATED”, StringComparison.OrdinalIgnoreCase)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam StartSessionWithSetupAsync.
        if (!string.Equals(status, "CREATED", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `!string.Equals(status, ”CREATED”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam StartSessionWithSetupAsync.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Status sesi tidak valid”) dalam
            // StartSessionWithSetupAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Status sesi tidak valid");
        // Menutup scope cabang if untuk kondisi `!string.Equals(status, ”CREATED”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar
        // batas blok tersebut dalam StartSessionWithSetupAsync.
        }

        // Menyiapkan variabel lokal `latestRevision` untuk nilai latest revision dengan hasil operasi asinkron membaca satu hasil basis data melalui
        // `conn.QuerySingleOrDefaultAsync<int>` dengan `new CommandDefinition( ”select coalesce(max(revision), 0) from session_setup_revisions where
        // session_id = @sessionId”, new { sessionId }, tx, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var latestRevision = await conn.QuerySingleOrDefaultAsync<int>(new CommandDefinition(
            // Meneruskan nilai literal `”select coalesce(max(revision), 0) from session_setup_revisions where session_id = @sessionId”` sebagai argumen ke
            // konstruktor `CommandDefinition`.
            "select coalesce(max(revision), 0) from session_setup_revisions where session_id = @sessionId",
            // Meneruskan objek anonim yang mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new { sessionId },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
        // Memeriksa perbandingan ketidaksamaan antara `latestRevision` dan `setupRevision`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam StartSessionWithSetupAsync.
        if (latestRevision != setupRevision)
        // Membuka scope cabang if untuk kondisi `latestRevision != setupRevision`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // StartSessionWithSetupAsync.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Pembagian awal berubah; baca revisi terbaru
            // sebelum memulai sesi”) dalam StartSessionWithSetupAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Pembagian awal berubah; baca revisi terbaru sebelum memulai sesi");
        // Menutup scope cabang if untuk kondisi `latestRevision != setupRevision`; bagian berikut berada di luar batas blok tersebut dalam
        // StartSessionWithSetupAsync.
        }

        // Menyiapkan variabel lokal `players` untuk nilai pemain dengan mematerialisasi urutan `(await conn.QueryAsync<SetupParticipant>(new
        // CommandDefinition( ””” select session_participant_id as SessionParticipantId, user_id as UserId, player_order_no as PlayerOrder fr...` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var players = (await conn.QueryAsync<SetupParticipant>(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id as
            // SessionParticipantId,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `user_id as UserId,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `player_order_no as
            // PlayerOrder`.
            // Baris literal 6: FROM memilih tabel/subquery sumber pembacaan: `from session_participants`.
            // Baris literal 7: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
            // Baris literal 8: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by player_order_no`.
            // Baris literal 9: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select
                session_participant_id as SessionParticipantId,
                user_id as UserId,
                player_order_no as PlayerOrder
            from session_participants
            where session_id = @sessionId
            order by player_order_no
            """,
            // Meneruskan objek anonim yang mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new { sessionId },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `setup` untuk nilai setup dengan hasil operasi asinkron memanggil `InitializeSetupAsync` dengan `conn`, `tx`,
        // `sessionId`, `rulesetVersionId`, `mode`, `definition`, `players`, `setupRequest`, `startedAt`, `ct`; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var setup = await InitializeSetupAsync(
            // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `InitializeSetupAsync`.
            conn,
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `InitializeSetupAsync`.
            tx,
            // Meneruskan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke `InitializeSetupAsync`.
            sessionId,
            // Meneruskan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
            // `InitializeSetupAsync`.
            rulesetVersionId,
            // Meneruskan `mode` (mode permainan yang menentukan kelompok aturan yang digunakan) sebagai argumen ke `InitializeSetupAsync`.
            mode,
            // Meneruskan `definition` (definisi terstruktur komponen serta parameter aturan permainan) sebagai argumen ke `InitializeSetupAsync`.
            definition,
            // Meneruskan `players` (nilai pemain) sebagai argumen ke `InitializeSetupAsync`.
            players,
            // Meneruskan `setupRequest` (nilai setup permintaan) sebagai argumen ke `InitializeSetupAsync`.
            setupRequest,
            // Meneruskan `startedAt` (nilai started at) sebagai argumen ke `InitializeSetupAsync`.
            startedAt,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // ke `InitializeSetupAsync`.
            ct);

        // Menyiapkan variabel lokal `affected` untuk nilai affected dengan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new
        // CommandDefinition( ””” update sessions set status = 'STARTED', started_at = @startedAt where session_id = @sessionId and status = 'CREATED';
        // update session_setup_revisions...`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var affected = await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update sessions`.
            // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set status = 'STARTED',`.
            // Baris literal 4: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `started_at = @startedAt`.
            // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
            // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and status = 'CREATED';`.
            // Baris literal 7: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 8: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update session_setup_revisions`.
            // Baris literal 9: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set locked_at = @startedAt`.
            // Baris literal 10: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
            // Baris literal 11: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and revision = @setupRevision`.
            // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and locked_at is null;`.
            // Baris literal 13: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 14: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update session_states state`.
            // Baris literal 15: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set day = 1,`.
            // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `weekday = 'MON',`.
            // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `turn_number = 1,`.
            // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_slot = 1,`.
            // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `current_session_player_id = @firstPlayerId,`.
            // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `current_action_slot = 1,`.
            // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_slots_left =
            // settings.actions_per_turn,`.
            // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `phase = 'PLAYER_TURN',`.
            // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_game_over = false,`.
            // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `state_version = 1,`.
            // Baris literal 25: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = @startedAt`.
            // Baris literal 26: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_game_settings settings`.
            // Baris literal 27: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where state.session_id = @sessionId`.
            // Baris literal 28: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and settings.ruleset_version_id = @rulesetVersionId;`.
            // Baris literal 29: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            update sessions
            set status = 'STARTED',
                started_at = @startedAt
            where session_id = @sessionId
              and status = 'CREATED';

            update session_setup_revisions
            set locked_at = @startedAt
            where session_id = @sessionId
              and revision = @setupRevision
              and locked_at is null;

            update session_states state
            set day = 1,
                weekday = 'MON',
                turn_number = 1,
                action_slot = 1,
                current_session_player_id = @firstPlayerId,
                current_action_slot = 1,
                action_slots_left = settings.actions_per_turn,
                phase = 'PLAYER_TURN',
                is_game_over = false,
                state_version = 1,
                updated_at = @startedAt
            from ruleset_game_settings settings
            where state.session_id = @sessionId
              and settings.ruleset_version_id = @rulesetVersionId;
            """,
            // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, setupRevision, firstPlayerId, startedAt sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // StartSessionWithSetupAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, setupRevision, firstPlayerId, startedAt sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                sessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, setupRevision, firstPlayerId, startedAt sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                rulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, setupRevision, firstPlayerId, startedAt sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                setupRevision,
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, setupRevision, firstPlayerId, startedAt sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                firstPlayerId = setup.FirstPlayerId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, setupRevision, firstPlayerId, startedAt sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                startedAt
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // StartSessionWithSetupAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
        // Memeriksa pemeriksaan lebih kecil antara `affected` dan `3`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // StartSessionWithSetupAsync.
        if (affected < 3)
        // Membuka scope cabang if untuk kondisi `affected < 3`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // StartSessionWithSetupAsync.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Sesi tidak dapat dimulai secara atomik”) dalam
            // StartSessionWithSetupAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Sesi tidak dapat dimulai secara atomik");
        // Menutup scope cabang if untuk kondisi `affected < 3`; bagian berikut berada di luar batas blok tersebut dalam StartSessionWithSetupAsync.
        }

        // Menjalankan hasil operasi asinkron mengesahkan transaksi `tx` sehingga perubahan yang terkumpul menjadi permanen; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai dalam StartSessionWithSetupAsync.
        await tx.CommitAsync(ct);
        // Mengembalikan `setup.NextSequenceNumber` (nilai next sequence number) kepada pemanggil dalam StartSessionWithSetupAsync; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return setup.NextSequenceNumber;
    // Menutup scope metode StartSessionWithSetupAsync; bagian berikut berada di luar batas blok tersebut dalam StartSessionWithSetupAsync.
    }

    // Mendefinisikan metode `GetStateAsync` dengan hasil bertipe `Task<SessionStateResponse?>`; operasi ini menangani get keadaan asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan
    // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<SessionStateResponse?> GetStateAsync(Guid sessionId, CancellationToken ct)
    // Membuka scope metode GetStateAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetStateAsync.
    {
        // Menyiapkan variabel lokal `stateSql` untuk nilai keadaan SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang
        // dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string stateSql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `state_version,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `day,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_slot as turn,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_slots_left,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `finish_day,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_game_over,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `(`.
        // Baris literal 11: SELECT menentukan nilai atau kolom yang dikembalikan query: `select coalesce(max(e.sequence_number) + 1, 0)`.
        // Baris literal 12: FROM memilih tabel/subquery sumber pembacaan: `from events e`.
        // Baris literal 13: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where e.session_id = session_states.session_id`.
        // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as next_sequence_number,`.
        // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}'::jsonb::text as
        // ui_state_json`.
        // Baris literal 16: FROM memilih tabel/subquery sumber pembacaan: `from session_states`.
        // Baris literal 17: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 18: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string stateSql = """
            select
                session_id,
                state_version,
                day,
                action_slot as turn,
                action_slots_left,
                finish_day,
                is_game_over,
                (
                    select coalesce(max(e.sequence_number) + 1, 0)
                    from events e
                    where e.session_id = session_states.session_id
                ) as next_sequence_number,
                '{}'::jsonb::text as ui_state_json
            from session_states
            where session_id = @sessionId
            """;

        // Menyiapkan variabel lokal `playersSql` untuk nilai pemain SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe
        // yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string playersSql =
        // ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.session_participant_id as
        // session_player_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.user_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.player_order_no as
        // player_index,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(sp.player_name,
        // u.display_name) as name,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(ps.coins, 0) as
        // coins,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(ps.happiness, 0) as
        // happiness,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(ps.saving, 0) as
        // saving,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(ps.total_donasi, 0)
        // as total_donasi`.
        // Baris literal 11: FROM memilih tabel/subquery sumber pembacaan: `from session_participants sp`.
        // Baris literal 12: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join app_users u on u.user_id = sp.user_id`.
        // Baris literal 13: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join session_participant_balances ps on
        // ps.session_participant_id = sp.session_participant_id`.
        // Baris literal 14: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where sp.session_id = @sessionId`.
        // Baris literal 15: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sp.player_order_no asc, sp.joined_at
        // asc`.
        // Baris literal 16: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string playersSql = """
            select
                sp.session_participant_id as session_player_id,
                sp.user_id,
                sp.player_order_no as player_index,
                coalesce(sp.player_name, u.display_name) as name,
                coalesce(ps.coins, 0) as coins,
                coalesce(ps.happiness, 0) as happiness,
                coalesce(ps.saving, 0) as saving,
                coalesce(ps.total_donasi, 0) as total_donasi
            from session_participants sp
            join app_users u on u.user_id = sp.user_id
            left join session_participant_balances ps on ps.session_participant_id = sp.session_participant_id
            where sp.session_id = @sessionId
            order by sp.player_order_no asc, sp.joined_at asc
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `state` untuk keadaan permainan yang menjadi sumber atau hasil pembaruan dengan hasil operasi asinkron membaca satu
        // hasil basis data melalui `conn.QuerySingleOrDefaultAsync<SessionStateRow>` dengan `new CommandDefinition(stateSql, new { sessionId },
        // cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var state = await conn.QuerySingleOrDefaultAsync<SessionStateRow>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (stateSql, new { sessionId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QuerySingleOrDefaultAsync<SessionStateRow>`; Meneruskan `stateSql` (nilai keadaan SQL) sebagai argumen ke konstruktor `CommandDefinition`;
            // Meneruskan objek anonim yang mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct`
            // (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(stateSql, new { sessionId }, cancellationToken: ct));
        // Memeriksa hasil pencocokan `state` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam GetStateAsync.
        if (state is null)
        // Membuka scope cabang if untuk kondisi `state is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetStateAsync.
        {
            // Mengembalikan null, yaitu penanda tidak ada nilai kepada pemanggil dalam GetStateAsync; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return null;
        // Menutup scope cabang if untuk kondisi `state is null`; bagian berikut berada di luar batas blok tersebut dalam GetStateAsync.
        }

        // Menyiapkan variabel lokal `playerRows` untuk nilai pemain baris dengan mematerialisasi urutan `(await conn.QueryAsync<SessionPlayerStateRow>( new
        // CommandDefinition(playersSql, new { sessionId }, cancellationToken: ct)))` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerRows = (await conn.QueryAsync<SessionPlayerStateRow>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (playersSql, new { sessionId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QueryAsync<SessionPlayerStateRow>`; Meneruskan `playersSql` (nilai pemain SQL) sebagai argumen ke konstruktor `CommandDefinition`;
            // Meneruskan objek anonim yang mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct`
            // (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(playersSql, new { sessionId }, cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `players` untuk nilai pemain dengan mematerialisasi urutan `playerRows.Select(row => new SessionPlayerStateDto {
        // SessionPlayerId = row.SessionPlayerId, UserId = row.UserId, PlayerIndex = row.PlayerIndex, Name = row.Name, Coins = row.Co...` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var players = playerRows.Select(row => new SessionPlayerStateDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetStateAsync.
        {
            // Memperbarui `SessionPlayerId` menggunakan `row.SessionPlayerId` (identitas keikutsertaan pemain pada sesi tertentu) dalam GetStateAsync.
            SessionPlayerId = row.SessionPlayerId,
            // Memperbarui `UserId` menggunakan `row.UserId` (identitas akun pengguna yang datanya sedang diproses) dalam GetStateAsync.
            UserId = row.UserId,
            // Memperbarui `PlayerIndex` menggunakan `row.PlayerIndex` (nilai pemain index) dalam GetStateAsync.
            PlayerIndex = row.PlayerIndex,
            // Memperbarui `Name` menggunakan `row.Name` (nilai nama) dalam GetStateAsync.
            Name = row.Name,
            // Memperbarui `Coins` menggunakan `row.Coins` (nilai coins) dalam GetStateAsync.
            Coins = row.Coins,
            // Memperbarui `Happiness` menggunakan `row.Happiness` (nilai kebahagiaan) dalam GetStateAsync.
            Happiness = row.Happiness,
            // Memperbarui `Saving` menggunakan `row.Saving` (nilai tabungan) dalam GetStateAsync.
            Saving = row.Saving,
            // Memperbarui `TotalDonasi` menggunakan `row.TotalDonasi` (nilai total donasi) dalam GetStateAsync.
            TotalDonasi = row.TotalDonasi
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam GetStateAsync.
        }).ToList();

        // Menyiapkan variabel lokal `bySessionPlayerId` untuk nilai berdasarkan sesi pemain identitas dengan membangun kamus dari `players` dengan
        // pemilihan kunci/nilai `player => player.SessionPlayerId`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var bySessionPlayerId = players.ToDictionary(player => player.SessionPlayerId);
        // Menyiapkan variabel lokal `sessionPlayerIds` untuk nilai sesi pemain identitas dengan mematerialisasi urutan `bySessionPlayerId.Keys` menjadi
        // array dengan elemen hasil saat ini. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionPlayerIds = bySessionPlayerId.Keys.ToArray();

        // Memeriksa pemeriksaan lebih besar antara `sessionPlayerIds.Length` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // GetStateAsync.
        if (sessionPlayerIds.Length > 0)
        // Membuka scope cabang if untuk kondisi `sessionPlayerIds.Length > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // GetStateAsync.
        {
            // Menjalankan hasil operasi asinkron memanggil `LoadPlayerChildrenAsync` dengan `conn`, `sessionPlayerIds`, `bySessionPlayerId`, `ct`; await
            // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam GetStateAsync.
            await LoadPlayerChildrenAsync(conn, sessionPlayerIds, bySessionPlayerId, ct);
        // Menutup scope cabang if untuk kondisi `sessionPlayerIds.Length > 0`; bagian berikut berada di luar batas blok tersebut dalam GetStateAsync.
        }

        // Menyiapkan variabel lokal `donationEvents` untuk nilai donasi event dengan hasil operasi asinkron memanggil `LoadDonationEventsAsync` dengan
        // `conn`, `sessionId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var donationEvents = await LoadDonationEventsAsync(conn, sessionId, ct);

        // Mengembalikan objek baru bertipe `SessionStateResponse` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam GetStateAsync; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return new SessionStateResponse
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetStateAsync.
        {
            // Memperbarui `SessionId` menggunakan `state.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam GetStateAsync.
            SessionId = state.SessionId,
            // Memperbarui `StateVersion` menggunakan `state.StateVersion` (nilai keadaan versi) dalam GetStateAsync.
            StateVersion = state.StateVersion,
            // Memperbarui `NextSequenceNumber` menggunakan `state.NextSequenceNumber` (nilai next sequence number) dalam GetStateAsync.
            NextSequenceNumber = state.NextSequenceNumber,
            // Memperbarui `Day` menggunakan `state.Day` (nomor hari permainan yang menjadi konteks aktivitas) dalam GetStateAsync.
            Day = state.Day,
            // Memperbarui `Turn` menggunakan `state.Turn` (giliran pemain yang sedang berlangsung) dalam GetStateAsync.
            Turn = state.Turn,
            // Memperbarui `ActionSlotsLeft` menggunakan `state.ActionSlotsLeft` (nilai aksi slots left) dalam GetStateAsync.
            ActionSlotsLeft = state.ActionSlotsLeft,
            // Memperbarui `FinishDay` menggunakan `state.FinishDay` (nilai finish hari) dalam GetStateAsync.
            FinishDay = state.FinishDay,
            // Memperbarui `IsGameOver` menggunakan `state.IsGameOver` (nilai berstatus game over) dalam GetStateAsync.
            IsGameOver = state.IsGameOver,
            // Memperbarui `Players` menggunakan `players` (nilai pemain) dalam GetStateAsync.
            Players = players,
            // Memperbarui `DonationEvents` menggunakan `donationEvents` (nilai donasi event) dalam GetStateAsync.
            DonationEvents = donationEvents
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam GetStateAsync.
        };
    // Menutup scope metode GetStateAsync; bagian berikut berada di luar batas blok tersebut dalam GetStateAsync.
    }

    // Mendefinisikan metode `SaveStateAsync` dengan hasil bertipe `Task<SaveSessionStateResult>`; operasi ini menangani save keadaan asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `request` bertipe `SaveSessionStateRequest` membawa data
    // masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar
    // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task<SaveSessionStateResult> SaveStateAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `request` bertipe `SaveSessionStateRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        SaveSessionStateRequest request,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode SaveStateAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SaveStateAsync.
    {
        // Menyiapkan variabel lokal `lockSql` untuk nilai lock SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang
        // dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string lockSql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select state_version`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from session_states`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 5: FOR UPDATE mengunci baris hasil selama transaksi agar perubahan bersamaan tidak menimpa keadaan yang dibaca: `for update`.
        // Baris literal 6: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string lockSql = """
            select state_version
            from session_states
            where session_id = @sessionId
            for update
            """;

        // Menyiapkan variabel lokal `updateSessionStateSql` untuk nilai update sesi keadaan SQL dengan literal multiline yang dirinci pada komentar di
        // dekat deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // updateSessionStateSql = ”””`.
        // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update session_states`.
        // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set day = @day,`.
        // Baris literal 4: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `weekday = @weekday,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `turn_number = coalesce((`.
        // Baris literal 6: SELECT menentukan nilai atau kolom yang dikembalikan query: `select sp.player_order_no`.
        // Baris literal 7: FROM memilih tabel/subquery sumber pembacaan: `from session_participants sp`.
        // Baris literal 8: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where sp.session_id = @sessionId`.
        // Baris literal 9: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and sp.session_participant_id = @currentSessionPlayerId`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `), 0),`.
        // Baris literal 11: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `action_slot = @turn,`.
        // Baris literal 12: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `current_session_player_id = @currentSessionPlayerId,`.
        // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `current_action_slot = @currentActionSlot,`.
        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `action_slots_left = @actionSlotsLeft,`.
        // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `finish_day = @finishDay,`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `phase = @phase,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `is_game_over = @isGameOver,`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `state_version = @newVersion,`.
        // Baris literal 19: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
        // Baris literal 20: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 21: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string updateSessionStateSql = """
            update session_states
            set day = @day,
                weekday = @weekday,
                turn_number = coalesce((
                    select sp.player_order_no
                    from session_participants sp
                    where sp.session_id = @sessionId
                      and sp.session_participant_id = @currentSessionPlayerId
                ), 0),
                action_slot = @turn,
                current_session_player_id = @currentSessionPlayerId,
                current_action_slot = @currentActionSlot,
                action_slots_left = @actionSlotsLeft,
                finish_day = @finishDay,
                phase = @phase,
                is_game_over = @isGameOver,
                state_version = @newVersion,
                updated_at = now()
            where session_id = @sessionId
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `tx` untuk transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan dengan hasil operasi asinkron
        // memulai transaksi pada `conn` menggunakan `ct` agar perubahan terkait dapat diselesaikan bersama; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        await using var tx = await conn.BeginTransactionAsync(ct);

        // Menyiapkan variabel lokal `currentVersion` untuk nilai saat ini versi dengan hasil operasi asinkron membaca satu hasil basis data melalui
        // `conn.QuerySingleOrDefaultAsync<long?>` dengan `new CommandDefinition(lockSql, new { sessionId }, tx, cancellationToken: ct)`; nilai default
        // menunjukkan tidak ada baris hasil; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var currentVersion = await conn.QuerySingleOrDefaultAsync<long?>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (lockSql, new { sessionId }, tx, cancellationToken: ct) sebagai argumen ke
            // `conn.QuerySingleOrDefaultAsync<long?>`; Meneruskan `lockSql` (nilai lock SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan
            // objek anonim yang mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `tx` (transaksi
            // basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal
            // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(lockSql, new { sessionId }, tx, cancellationToken: ct));
        // Memeriksa kebalikan kondisi `currentVersion.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam SaveStateAsync.
        if (!currentVersion.HasValue)
        // Membuka scope cabang if untuk kondisi `!currentVersion.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // SaveStateAsync.
        {
            // Menjalankan hasil operasi asinkron membatalkan perubahan yang belum disahkan pada transaksi `tx`; await menunggu hasil tanpa memblokir thread
            // selama operasi belum selesai dalam SaveStateAsync.
            await tx.RollbackAsync(ct);
            // Mengembalikan membentuk respons HTTP 404 dengan tanpa argumen karena sumber daya tidak ditemukan kepada pemanggil dalam SaveStateAsync; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return SaveSessionStateResult.NotFound();
        // Menutup scope cabang if untuk kondisi `!currentVersion.HasValue`; bagian berikut berada di luar batas blok tersebut dalam SaveStateAsync.
        }

        // Memeriksa perbandingan ketidaksamaan antara `currentVersion.Value` dan `request.StateVersion`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam SaveStateAsync.
        if (currentVersion.Value != request.StateVersion)
        // Membuka scope cabang if untuk kondisi `currentVersion.Value != request.StateVersion`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam SaveStateAsync.
        {
            // Menjalankan hasil operasi asinkron membatalkan perubahan yang belum disahkan pada transaksi `tx`; await menunggu hasil tanpa memblokir thread
            // selama operasi belum selesai dalam SaveStateAsync.
            await tx.RollbackAsync(ct);
            // Mengembalikan memanggil `SaveSessionStateResult.Stale` dengan `currentVersion.Value` kepada pemanggil dalam SaveStateAsync; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return SaveSessionStateResult.Stale(currentVersion.Value);
        // Menutup scope cabang if untuk kondisi `currentVersion.Value != request.StateVersion`; bagian berikut berada di luar batas blok tersebut dalam
        // SaveStateAsync.
        }

        // Menyiapkan variabel lokal `newVersion` untuk nilai new versi dengan penjumlahan/penggabungan antara `currentVersion.Value` dan `1`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var newVersion = currentVersion.Value + 1;
        // Menyiapkan variabel lokal `weekday` untuk nilai weekday dengan memanggil `ResolveWeekdayCode` dengan `request.Day`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var weekday = ResolveWeekdayCode(request.Day);
        // Menyiapkan variabel lokal `currentSessionPlayerId` untuk nilai saat ini sesi pemain identitas dengan `request.Players? .FirstOrDefault(player =>
        // player.PlayerIndex == request.Turn) ?.SessionPlayerId`; akses setelah ?. hanya dilakukan bila penerimanya tidak null. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var currentSessionPlayerId = request.Players?
            // Melanjutkan pengolahan dengan memanggil `.FirstOrDefault` dengan `player => player.PlayerIndex == request.Turn` dalam SaveStateAsync.
            .FirstOrDefault(player => player.PlayerIndex == request.Turn)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: .SessionPlayerId; dalam SaveStateAsync.
            ?.SessionPlayerId;
        // Menjalankan hasil operasi asinkron memanggil `DeleteSnapshotChildrenAsync` dengan `conn`, `tx`, `sessionId`, `ct`; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai dalam SaveStateAsync.
        await DeleteSnapshotChildrenAsync(conn, tx, sessionId, ct);

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( updateSessionStateSql, new {
        // sessionId, day = request.Day, weekday, turn = request.Turn, currentSessionPlayerId, currentActionSlot = ResolveCurrentActio...`; nilai hasil
        // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam SaveStateAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan `updateSessionStateSql` (nilai update sesi keadaan SQL) sebagai argumen ke konstruktor `CommandDefinition`.
            updateSessionStateSql,
            // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turn, currentSessionPlayerId, currentActionSlot, actionSlotsLeft, finishDay,
            // phase, isGameOver, newVersion sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SaveStateAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turn, currentSessionPlayerId, currentActionSlot, actionSlotsLeft, finishDay,
                // phase, isGameOver, newVersion sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                sessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turn, currentSessionPlayerId, currentActionSlot, actionSlotsLeft, finishDay,
                // phase, isGameOver, newVersion sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                day = request.Day,
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turn, currentSessionPlayerId, currentActionSlot, actionSlotsLeft, finishDay,
                // phase, isGameOver, newVersion sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                weekday,
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turn, currentSessionPlayerId, currentActionSlot, actionSlotsLeft, finishDay,
                // phase, isGameOver, newVersion sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                turn = request.Turn,
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turn, currentSessionPlayerId, currentActionSlot, actionSlotsLeft, finishDay,
                // phase, isGameOver, newVersion sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                currentSessionPlayerId,
                // Meneruskan `request.ActionSlotsLeft` (nilai aksi slots left) sebagai argumen ke `ResolveCurrentActionSlot`.
                currentActionSlot = ResolveCurrentActionSlot(request.ActionSlotsLeft),
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turn, currentSessionPlayerId, currentActionSlot, actionSlotsLeft, finishDay,
                // phase, isGameOver, newVersion sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                actionSlotsLeft = request.ActionSlotsLeft,
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turn, currentSessionPlayerId, currentActionSlot, actionSlotsLeft, finishDay,
                // phase, isGameOver, newVersion sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                finishDay = request.FinishDay,
                // Meneruskan `weekday` (nilai weekday) sebagai argumen ke `ResolvePhase`; Meneruskan `request.IsGameOver` (nilai berstatus game over) sebagai
                // argumen ke `ResolvePhase`.
                phase = ResolvePhase(weekday, request.IsGameOver),
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turn, currentSessionPlayerId, currentActionSlot, actionSlotsLeft, finishDay,
                // phase, isGameOver, newVersion sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                isGameOver = request.IsGameOver,
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turn, currentSessionPlayerId, currentActionSlot, actionSlotsLeft, finishDay,
                // phase, isGameOver, newVersion sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                newVersion
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam SaveStateAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));

        // Mengulangi setiap elemen `request.Players ?? []`; elemen saat ini disimpan sebagai `player` bertipe `var` untuk diproses oleh badan loop dalam
        // SaveStateAsync.
        foreach (var player in request.Players ?? [])
        // Membuka scope loop setiap player dari `request.Players ?? []`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SaveStateAsync.
        {
            // Menjalankan hasil operasi asinkron memanggil `SavePlayerSnapshotAsync` dengan `conn`, `tx`, `sessionId`, `player`, `ct`; await menunggu hasil
            // tanpa memblokir thread selama operasi belum selesai dalam SaveStateAsync.
            await SavePlayerSnapshotAsync(conn, tx, sessionId, player, ct);
        // Menutup scope loop setiap player dari `request.Players ?? []`; bagian berikut berada di luar batas blok tersebut dalam SaveStateAsync.
        }

        // Mengulangi setiap elemen `request.DonationEvents ?? []`; elemen saat ini disimpan sebagai `donationEvent` bertipe `var` untuk diproses oleh badan
        // loop dalam SaveStateAsync.
        foreach (var donationEvent in request.DonationEvents ?? [])
        // Membuka scope loop setiap donationEvent dari `request.DonationEvents ?? []`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // SaveStateAsync.
        {
            // Menjalankan hasil operasi asinkron memanggil `SaveDonationEventAsync` dengan `conn`, `tx`, `sessionId`, `donationEvent`, `ct`; await menunggu
            // hasil tanpa memblokir thread selama operasi belum selesai dalam SaveStateAsync.
            await SaveDonationEventAsync(conn, tx, sessionId, donationEvent, ct);
        // Menutup scope loop setiap donationEvent dari `request.DonationEvents ?? []`; bagian berikut berada di luar batas blok tersebut dalam
        // SaveStateAsync.
        }

        // Menjalankan hasil operasi asinkron memanggil `SaveLastActionAsync` dengan `conn`, `tx`, `sessionId`, `newVersion`, `request`, `ct`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam SaveStateAsync.
        await SaveLastActionAsync(conn, tx, sessionId, newVersion, request, ct);

        // Menjalankan hasil operasi asinkron mengesahkan transaksi `tx` sehingga perubahan yang terkumpul menjadi permanen; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai dalam SaveStateAsync.
        await tx.CommitAsync(ct);

        // Menyiapkan variabel lokal `state` untuk keadaan permainan yang menjadi sumber atau hasil pembaruan dengan hasil operasi asinkron memanggil
        // `GetStateAsync` dengan `sessionId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var state = await GetStateAsync(sessionId, ct);
        // Memeriksa hasil pencocokan `state` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam SaveStateAsync.
        if (state is null)
        // Membuka scope cabang if untuk kondisi `state is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SaveStateAsync.
        {
            // Mengembalikan membentuk respons HTTP 404 dengan tanpa argumen karena sumber daya tidak ditemukan kepada pemanggil dalam SaveStateAsync; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return SaveSessionStateResult.NotFound();
        // Menutup scope cabang if untuk kondisi `state is null`; bagian berikut berada di luar batas blok tersebut dalam SaveStateAsync.
        }

        // Mengembalikan memanggil `SaveSessionStateResult.Saved` dengan `state` kepada pemanggil dalam SaveStateAsync; eksekusi jalur ini selesai setelah
        // nilai hasil ditentukan.
        return SaveSessionStateResult.Saved(state);
    // Menutup scope metode SaveStateAsync; bagian berikut berada di luar batas blok tersebut dalam SaveStateAsync.
    }

    // Mendefinisikan metode `ComputeFinalScoresAsync` dengan hasil bertipe `Task`; operasi ini menangani compute akhir skor asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan
    // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task ComputeFinalScoresAsync(Guid sessionId, CancellationToken ct)
    // Membuka scope metode ComputeFinalScoresAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ComputeFinalScoresAsync.
    {
        // Menyiapkan variabel lokal `activeRulesetSql` untuk nilai aktif aturan SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string activeRulesetSql
        // = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_version_id`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from sessions`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 5: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 6: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string activeRulesetSql = """
            select ruleset_version_id
            from sessions
            where session_id = @sessionId
            limit 1
            """;

        // Menyiapkan variabel lokal `conn` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi asinkron
        // membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var conn = await _dataSource.OpenConnectionAsync(ct);
        // Menyiapkan variabel lokal `rulesetVersionId` untuk identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat dengan hasil
        // operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<Guid?>` dengan `new CommandDefinition(activeRulesetSql,
        // new { sessionId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetVersionId = await conn.QuerySingleOrDefaultAsync<Guid?>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (activeRulesetSql, new { sessionId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QuerySingleOrDefaultAsync<Guid?>`; Meneruskan `activeRulesetSql` (nilai aktif aturan SQL) sebagai argumen ke konstruktor
            // `CommandDefinition`; Meneruskan objek anonim yang mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`;
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            new CommandDefinition(activeRulesetSql, new { sessionId }, cancellationToken: ct));
        // Memeriksa kebalikan kondisi `rulesetVersionId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ComputeFinalScoresAsync.
        if (!rulesetVersionId.HasValue)
        // Membuka scope cabang if untuk kondisi `!rulesetVersionId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ComputeFinalScoresAsync.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Session belum memiliki ruleset aktif.”) dalam
            // ComputeFinalScoresAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Session belum memiliki ruleset aktif.");
        // Menutup scope cabang if untuk kondisi `!rulesetVersionId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam
        // ComputeFinalScoresAsync.
        }

        // Menyiapkan variabel lokal `rulesetVersion` untuk nilai aturan versi dengan hasil operasi asinkron memanggil
        // `_rulesets.GetRulesetVersionByIdAsync` dengan `rulesetVersionId.Value`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetVersion = await _rulesets.GetRulesetVersionByIdAsync(rulesetVersionId.Value, ct);
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `rulesetVersion?.Definition is null` dan
        // `!RulesetRuntimeMapper.TryBuildConfig(rulesetVersion.Definition, out var config, out _)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok
        // if hanya dijalankan ketika kondisi ini bernilai benar dalam ComputeFinalScoresAsync.
        if (rulesetVersion?.Definition is null ||
            // Menggunakan kebalikan kondisi `RulesetRuntimeMapper.TryBuildConfig(rulesetVersion.Definition, out var config, out _)` sebagai bagian ekspresi
            // yang sedang disusun dalam ComputeFinalScoresAsync.
            !RulesetRuntimeMapper.TryBuildConfig(rulesetVersion.Definition, out var config, out _))
        // Membuka scope cabang if untuk kondisi `rulesetVersion?.Definition is null || !RulesetRuntimeMapper.TryBuildConfig(rulesetVersion.Definition, out
        // var config, out _)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ComputeFinalScoresAsync.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Definition ruleset aktif tidak valid.”) dalam
            // ComputeFinalScoresAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Definition ruleset aktif tidak valid.");
        // Menutup scope cabang if untuk kondisi `rulesetVersion?.Definition is null || !RulesetRuntimeMapper.TryBuildConfig(rulesetVersion.Definition, out
        // var config, out _)`; bagian berikut berada di luar batas blok tersebut dalam ComputeFinalScoresAsync.
        }

        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan hasil operasi
        // asinkron memanggil `_events.GetAllEventsBySessionAsync` dengan `sessionId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = await _events.GetAllEventsBySessionAsync(sessionId, ct);
        // Menyiapkan variabel lokal `sourceEventId` untuk nilai source event identitas dengan mengambil elemen pertama `events .OrderByDescending(item =>
        // item.SequenceNumber) .Select(item => (Guid?)item.EventId)`; jika tidak ada, gunakan nilai default tipe hasil. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var sourceEventId = events
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderByDescending(item => item.SequenceNumber) dalam ComputeFinalScoresAsync;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderByDescending(item => item.SequenceNumber)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => (Guid?)item.EventId) dalam ComputeFinalScoresAsync; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => (Guid?)item.EventId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .FirstOrDefault(); dalam ComputeFinalScoresAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .FirstOrDefault();
        // Menyiapkan variabel lokal `projections` untuk proyeksi transaksi arus kas yang diturunkan dari event permainan dengan hasil operasi asinkron
        // memanggil `_events.GetCashflowProjectionsAsync` dengan `sessionId`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projections = await _events.GetCashflowProjectionsAsync(sessionId, ct);
        // Menyiapkan variabel lokal `breakdownByUserId` untuk nilai breakdown berdasarkan pengguna identitas dengan memanggil `_happiness.ComputeByPlayer`
        // dengan `events`, `projections`, `config`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var breakdownByUserId = _happiness.ComputeByPlayer(events, projections, config);
        // Menyiapkan variabel lokal `emptyBreakdown` untuk nilai empty breakdown dengan memanggil `_happiness.ComputeBreakdown` dengan `[]`, `0`, `0`, `0`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var emptyBreakdown = _happiness.ComputeBreakdown([], 0, 0, 0);

        // Menyiapkan variabel lokal `participants` untuk nilai participants dengan mematerialisasi urutan `(await
        // conn.QueryAsync<FinalScoreParticipantRow>( new CommandDefinition( ””” select sp.session_participant_id as SessionParticipantId, sp.user_id as
        // UserId, sptb.tie_number as...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var participants = (await conn.QueryAsync<FinalScoreParticipantRow>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select sp.session_participant_id as SessionParticipantId, sp.user_id as
            // UserId, sptb.tie_number as TieBreakerNumber, coalesce(spb.coins, 0) + coalesce(spb... sebagai argumen ke
            // `conn.QueryAsync<FinalScoreParticipantRow>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.session_participant_id as
                // SessionParticipantId,`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sp.user_id as UserId,`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sptb.tie_number as
                // TieBreakerNumber,`.
                // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(spb.coins, 0)`.
                // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `+ coalesce(spb.saving, 0)`.
                // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `+
                // coalesce(ingredients.leftover_qty, 0) as PensionFund`.
                // Baris literal 9: FROM memilih tabel/subquery sumber pembacaan: `from session_participants sp`.
                // Baris literal 10: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join session_participant_balances spb`.
                // Baris literal 11: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on spb.session_id = sp.session_id`.
                // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and spb.session_participant_id =
                // sp.session_participant_id`.
                // Baris literal 13: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join (`.
                // Baris literal 14: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_id, session_participant_id, sum(qty)::int as
                // leftover_qty`.
                // Baris literal 15: FROM memilih tabel/subquery sumber pembacaan: `from session_participant_inventory`.
                // Baris literal 16: GROUP BY mengelompokkan baris sebelum fungsi agregasi dihitung: `group by session_id, session_participant_id`.
                // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) ingredients`.
                // Baris literal 18: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on ingredients.session_id = sp.session_id`.
                // Baris literal 19: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ingredients.session_participant_id =
                // sp.session_participant_id`.
                // Baris literal 20: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join session_participant_tie_breakers sptb`.
                // Baris literal 21: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on sptb.session_id = sp.session_id`.
                // Baris literal 22: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and sptb.session_participant_id =
                // sp.session_participant_id`.
                // Baris literal 23: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where sp.session_id = @sessionId`.
                // Baris literal 24: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sp.player_order_no asc`.
                // Baris literal 25: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select
                    sp.session_participant_id as SessionParticipantId,
                    sp.user_id as UserId,
                    sptb.tie_number as TieBreakerNumber,
                    coalesce(spb.coins, 0)
                        + coalesce(spb.saving, 0)
                        + coalesce(ingredients.leftover_qty, 0) as PensionFund
                from session_participants sp
                left join session_participant_balances spb
                  on spb.session_id = sp.session_id
                 and spb.session_participant_id = sp.session_participant_id
                left join (
                    select session_id, session_participant_id, sum(qty)::int as leftover_qty
                    from session_participant_inventory
                    group by session_id, session_participant_id
                ) ingredients
                  on ingredients.session_id = sp.session_id
                 and ingredients.session_participant_id = sp.session_participant_id
                left join session_participant_tie_breakers sptb
                  on sptb.session_id = sp.session_id
                 and sptb.session_participant_id = sp.session_participant_id
                where sp.session_id = @sessionId
                order by sp.player_order_no asc
                """,
                // Meneruskan objek anonim yang mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { sessionId },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct))).ToList();

        // Menyiapkan variabel lokal `pensionPointsByRank` untuk nilai pension poin berdasarkan rank dengan `config!.Scoring?.PensionRankPoints
        // .ToDictionary(item => item.Rank, item => item.Points)` bila tidak null; jika null gunakan `new Dictionary<int, int>()` sebagai nilai pengganti.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var pensionPointsByRank = config!.Scoring?.PensionRankPoints
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary(item => item.Rank, item => item.Points) dalam
            // ComputeFinalScoresAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(item => item.Rank, item => item.Points)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: new Dictionary<int, int>(); dalam ComputeFinalScoresAsync.
            ?? new Dictionary<int, int>();

        // Menyiapkan variabel lokal `pensionRanking` untuk nilai pension ranking dengan mematerialisasi urutan `participants .Select(participant => {
        // return new FinalScoreParticipantValue( participant, participant.PensionFund, breakdownByUserId.TryGetValue(participant.UserId, out var br...`
        // menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var pensionRanking = participants
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(participant => dalam ComputeFinalScoresAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(participant =>
            // Membuka scope fungsi lambda yang dipasok ke `participants .Select`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ComputeFinalScoresAsync.
            {
                // Mengembalikan objek baru bertipe `FinalScoreParticipantValue` dengan argumen ( participant, participant.PensionFund,
                // breakdownByUserId.TryGetValue(participant.UserId, out var breakdown) ? breakdown : emptyBreakdown) kepada pemanggil dalam
                // ComputeFinalScoresAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return new FinalScoreParticipantValue(
                    // Meneruskan `participant` (nilai participant) sebagai argumen ke konstruktor `FinalScoreParticipantValue`.
                    participant,
                    // Meneruskan `participant.PensionFund` (nilai pension fund) sebagai argumen ke konstruktor `FinalScoreParticipantValue`.
                    participant.PensionFund,
                    // Meneruskan hasil pemilihan bersyarat: ketika `breakdownByUserId.TryGetValue(participant.UserId, out var breakdown)` benar gunakan `breakdown`,
                    // jika tidak gunakan `emptyBreakdown` sebagai argumen ke konstruktor `FinalScoreParticipantValue`; Meneruskan `participant.UserId` (identitas akun
                    // pengguna yang datanya sedang diproses) sebagai argumen ke `breakdownByUserId.TryGetValue`; Meneruskan `var breakdown` sebagai argumen ke
                    // `breakdownByUserId.TryGetValue`.
                    breakdownByUserId.TryGetValue(participant.UserId, out var breakdown)
                        // Meneruskan hasil pemilihan bersyarat: ketika `breakdownByUserId.TryGetValue(participant.UserId, out var breakdown)` benar gunakan `breakdown`,
                        // jika tidak gunakan `emptyBreakdown` sebagai argumen ke konstruktor `FinalScoreParticipantValue`.
                        ? breakdown
                        // Meneruskan hasil pemilihan bersyarat: ketika `breakdownByUserId.TryGetValue(participant.UserId, out var breakdown)` benar gunakan `breakdown`,
                        // jika tidak gunakan `emptyBreakdown` sebagai argumen ke konstruktor `FinalScoreParticipantValue`.
                        : emptyBreakdown);
            // Menutup scope fungsi lambda yang dipasok ke `participants .Select`; bagian berikut berada di luar batas blok tersebut dalam
            // ComputeFinalScoresAsync.
            })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderByDescending(item => item.CashRemaining) dalam ComputeFinalScoresAsync;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderByDescending(item => item.CashRemaining)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenByDescending(item => item.Participant.TieBreakerNumber ?? 0) dalam
            // ComputeFinalScoresAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ThenByDescending(item => item.Participant.TieBreakerNumber ?? 0)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(item => item.Participant.UserId) dalam ComputeFinalScoresAsync; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ThenBy(item => item.Participant.UserId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam ComputeFinalScoresAsync; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .ToList();

        // Menyiapkan variabel lokal `tx` untuk transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan dengan hasil operasi asinkron
        // memulai transaksi pada `conn` menggunakan `ct` agar perubahan terkait dapat diselesaikan bersama; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        await using var tx = await conn.BeginTransactionAsync(ct);
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” update
        // session_participant_collection_missions set is_failed = true, updated_at = now() where session_id = @sessionId and not is_completed; delete
        // f...`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ComputeFinalScoresAsync.
        await conn.ExecuteAsync(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” update session_participant_collection_missions set is_failed = true,
            // updated_at = now() where session_id = @sessionId and not is_completed; delete from se... sebagai argumen ke `conn.ExecuteAsync`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update
                // session_participant_collection_missions`.
                // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set is_failed = true,`.
                // Baris literal 4: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
                // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
                // Baris literal 6: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not is_completed;`.
                // Baris literal 7: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 8: DELETE menghapus baris pada tabel tujuan sesuai pembatas query: `delete from session_final_score_components`.
                // Baris literal 9: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_final_score_id in (`.
                // Baris literal 10: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_final_score_id`.
                // Baris literal 11: FROM memilih tabel/subquery sumber pembacaan: `from session_final_scores`.
                // Baris literal 12: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
                // Baris literal 13: Pembatas literal/penutup `);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 14: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 15: DELETE menghapus baris pada tabel tujuan sesuai pembatas query: `delete from session_final_scores`.
                // Baris literal 16: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId;`.
                // Baris literal 17: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 18: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                update session_participant_collection_missions
                set is_failed = true,
                    updated_at = now()
                where session_id = @sessionId
                  and not is_completed;

                delete from session_final_score_components
                where session_final_score_id in (
                    select session_final_score_id
                    from session_final_scores
                    where session_id = @sessionId
                );

                delete from session_final_scores
                where session_id = @sessionId;

                """,
                // Meneruskan objek anonim yang mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { sessionId },
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                tx,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct));

        // Menyiapkan variabel lokal `finalRanking` untuk nilai akhir ranking dengan mematerialisasi urutan `pensionRanking .Select((value, index) => {
        // pensionPointsByRank.TryGetValue(index + 1, out var pensionPoints); var correctedBreakdown = value.Breakdown with { PensionPoints = p...` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var finalRanking = pensionRanking
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select((value, index) => dalam ComputeFinalScoresAsync; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Select((value, index) =>
            // Membuka scope fungsi lambda yang dipasok ke `pensionRanking .Select`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ComputeFinalScoresAsync.
            {
                // Menjalankan mencari kunci `index + 1` pada `pensionPointsByRank`; hasil boolean menandakan kunci ditemukan dan argumen out menerima nilainya
                // dalam ComputeFinalScoresAsync.
                pensionPointsByRank.TryGetValue(index + 1, out var pensionPoints);
                // Menyiapkan variabel lokal `correctedBreakdown` untuk nilai corrected breakdown dengan `value.Breakdown with { PensionPoints = pensionPoints }`.
                // Tipe variabel disimpulkan dari ekspresi nilai awal.
                var correctedBreakdown = value.Breakdown with
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ComputeFinalScoresAsync.
                {
                    // Memperbarui `PensionPoints` menggunakan `pensionPoints` (nilai pension poin) dalam ComputeFinalScoresAsync.
                    PensionPoints = pensionPoints
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ComputeFinalScoresAsync.
                };
                // Memperbarui `correctedBreakdown` menggunakan `correctedBreakdown with { Total = correctedBreakdown.Total - value.Breakdown.PensionPoints +
                // pensionPoints }` dalam ComputeFinalScoresAsync.
                correctedBreakdown = correctedBreakdown with
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ComputeFinalScoresAsync.
                {
                    // Memperbarui `Total` menggunakan penjumlahan/penggabungan antara `correctedBreakdown.Total - value.Breakdown.PensionPoints` dan `pensionPoints`
                    // dalam ComputeFinalScoresAsync.
                    Total = correctedBreakdown.Total - value.Breakdown.PensionPoints + pensionPoints
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ComputeFinalScoresAsync.
                };

                // Mengembalikan objek anonim yang mengelompokkan Value, CorrectedBreakdown sebagai satu nilai kepada pemanggil dalam ComputeFinalScoresAsync;
                // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ComputeFinalScoresAsync.
                {
                    // Meneruskan fungsi lambda `(value, index) => { pensionPointsByRank.TryGetValue(index + 1, out var pensionPoints); var correctedBreakdown =
                    // value.Breakdown with { PensionPoints = pensionPoints }; correct...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                    // argumen ke `pensionRanking .Select`.
                    Value = value,
                    // Meneruskan fungsi lambda `(value, index) => { pensionPointsByRank.TryGetValue(index + 1, out var pensionPoints); var correctedBreakdown =
                    // value.Breakdown with { PensionPoints = pensionPoints }; correct...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                    // argumen ke `pensionRanking .Select`.
                    CorrectedBreakdown = correctedBreakdown
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ComputeFinalScoresAsync.
                };
            // Menutup scope fungsi lambda yang dipasok ke `pensionRanking .Select`; bagian berikut berada di luar batas blok tersebut dalam
            // ComputeFinalScoresAsync.
            })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderByDescending(item => item.CorrectedBreakdown.Total) dalam
            // ComputeFinalScoresAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderByDescending(item => item.CorrectedBreakdown.Total)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenByDescending(item => item.Value.Participant.TieBreakerNumber ?? 0) dalam
            // ComputeFinalScoresAsync; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ThenByDescending(item => item.Value.Participant.TieBreakerNumber ?? 0)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(item => item.Value.Participant.UserId) dalam ComputeFinalScoresAsync;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ThenBy(item => item.Value.Participant.UserId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam ComputeFinalScoresAsync; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .ToList();

        // Memulai loop dengan inisialisasi `var index = 0`, berjalan selama `index < finalRanking.Count`, lalu memperbarui pencacah melalui `index++` dalam
        // ComputeFinalScoresAsync.
        for (var index = 0; index < finalRanking.Count; index++)
        // Membuka scope loop dengan syarat `index < finalRanking.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ComputeFinalScoresAsync.
        {
            // Menyiapkan variabel lokal `value` untuk nilai nilai dengan `finalRanking[index]`, yaitu elemen koleksi yang dipilih melalui indeks atau kunci
            // tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var value = finalRanking[index];
            // Menyiapkan variabel lokal `correctedBreakdown` untuk nilai corrected breakdown dengan `value.CorrectedBreakdown` (nilai corrected breakdown).
            // Tipe variabel disimpulkan dari ekspresi nilai awal.
            var correctedBreakdown = value.CorrectedBreakdown;
            // Menyiapkan variabel lokal `scoreId` untuk nilai skor identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
            // dari ekspresi nilai awal.
            var scoreId = Guid.NewGuid();
            // Menyiapkan variabel lokal `totalPoints` untuk nilai total poin dengan hasil konversi `Math.Round(correctedBreakdown.Total,
            // MidpointRounding.AwayFromZero)` menjadi tipe `int`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var totalPoints = (int)Math.Round(correctedBreakdown.Total, MidpointRounding.AwayFromZero);

            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” insert into
            // session_final_scores ( session_final_score_id, session_id, session_participant_id, total_points, rank_no, tie_breaker_number, has_unpaid...`;
            // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // ComputeFinalScoresAsync.
            await conn.ExecuteAsync(
                // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” insert into session_final_scores ( session_final_score_id, session_id,
                // session_participant_id, total_points, rank_no, tie_breaker_number, has_unpaid_loan,... sebagai argumen ke `conn.ExecuteAsync`.
                new CommandDefinition(
                    // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                    // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                    // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                    // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_final_scores (`.
                    // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_final_score_id,`.
                    // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
                    // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
                    // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `total_points,`.
                    // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rank_no,`.
                    // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `tie_breaker_number,`.
                    // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `has_unpaid_loan,`.
                    // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `computed_at,`.
                    // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `source_event_id`.
                    // Baris literal 12: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                    // Baris literal 13: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
                    // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
                    // SQL: `@scoreId,`.
                    // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
                    // SQL: `@sessionId,`.
                    // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
                    // SQL: `@sessionParticipantId,`.
                    // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
                    // SQL: `@totalPoints,`.
                    // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
                    // SQL: `@rankNo,`.
                    // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
                    // SQL: `@tieBreakerNumber,`.
                    // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
                    // SQL: `@hasUnpaidLoan,`.
                    // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now(),`.
                    // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
                    // SQL: `@sourceEventId`.
                    // Baris literal 23: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                    // Baris literal 24: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                    """
                    insert into session_final_scores (
                        session_final_score_id,
                        session_id,
                        session_participant_id,
                        total_points,
                        rank_no,
                        tie_breaker_number,
                        has_unpaid_loan,
                        computed_at,
                        source_event_id
                    )
                    values (
                        @scoreId,
                        @sessionId,
                        @sessionParticipantId,
                        @totalPoints,
                        @rankNo,
                        @tieBreakerNumber,
                        @hasUnpaidLoan,
                        now(),
                        @sourceEventId
                    )
                    """,
                    // Meneruskan objek anonim yang mengelompokkan scoreId, sessionId, sessionParticipantId, totalPoints, rankNo,
                    // value.Value.Participant.TieBreakerNumber, correctedBreakdown.HasUnpaidLoan, sourceEventId sebagai satu nilai sebagai argumen ke konstruktor
                    // `CommandDefinition`.
                    new
                    // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // ComputeFinalScoresAsync.
                    {
                        // Meneruskan objek anonim yang mengelompokkan scoreId, sessionId, sessionParticipantId, totalPoints, rankNo,
                        // value.Value.Participant.TieBreakerNumber, correctedBreakdown.HasUnpaidLoan, sourceEventId sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        scoreId,
                        // Meneruskan objek anonim yang mengelompokkan scoreId, sessionId, sessionParticipantId, totalPoints, rankNo,
                        // value.Value.Participant.TieBreakerNumber, correctedBreakdown.HasUnpaidLoan, sourceEventId sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        sessionId,
                        // Meneruskan objek anonim yang mengelompokkan scoreId, sessionId, sessionParticipantId, totalPoints, rankNo,
                        // value.Value.Participant.TieBreakerNumber, correctedBreakdown.HasUnpaidLoan, sourceEventId sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        sessionParticipantId = value.Value.Participant.SessionParticipantId,
                        // Meneruskan objek anonim yang mengelompokkan scoreId, sessionId, sessionParticipantId, totalPoints, rankNo,
                        // value.Value.Participant.TieBreakerNumber, correctedBreakdown.HasUnpaidLoan, sourceEventId sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        totalPoints,
                        // Meneruskan objek anonim yang mengelompokkan scoreId, sessionId, sessionParticipantId, totalPoints, rankNo,
                        // value.Value.Participant.TieBreakerNumber, correctedBreakdown.HasUnpaidLoan, sourceEventId sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        rankNo = index + 1,
                        // Meneruskan objek anonim yang mengelompokkan scoreId, sessionId, sessionParticipantId, totalPoints, rankNo,
                        // value.Value.Participant.TieBreakerNumber, correctedBreakdown.HasUnpaidLoan, sourceEventId sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        value.Value.Participant.TieBreakerNumber,
                        // Meneruskan objek anonim yang mengelompokkan scoreId, sessionId, sessionParticipantId, totalPoints, rankNo,
                        // value.Value.Participant.TieBreakerNumber, correctedBreakdown.HasUnpaidLoan, sourceEventId sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        correctedBreakdown.HasUnpaidLoan,
                        // Meneruskan objek anonim yang mengelompokkan scoreId, sessionId, sessionParticipantId, totalPoints, rankNo,
                        // value.Value.Participant.TieBreakerNumber, correctedBreakdown.HasUnpaidLoan, sourceEventId sebagai satu nilai sebagai argumen ke konstruktor
                        // `CommandDefinition`.
                        sourceEventId
                    // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ComputeFinalScoresAsync.
                    },
                    // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                    tx,
                    // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                    // bernama `cancellationToken`.
                    cancellationToken: ct));

            // Menyiapkan variabel lokal `components` untuk nilai komponen dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var components = new[]
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ComputeFinalScoresAsync.
            {
                // Menggunakan objek baru bertipe `FinalScoreComponentValue` dengan argumen (”NEED_POINTS”, correctedBreakdown.NeedPoints) sebagai bagian ekspresi
                // yang sedang disusun dalam ComputeFinalScoresAsync.
                new FinalScoreComponentValue("NEED_POINTS", correctedBreakdown.NeedPoints),
                // Menggunakan objek baru bertipe `FinalScoreComponentValue` dengan argumen (”NEED_SET_BONUS”, correctedBreakdown.NeedSetBonusPoints) sebagai bagian
                // ekspresi yang sedang disusun dalam ComputeFinalScoresAsync.
                new FinalScoreComponentValue("NEED_SET_BONUS", correctedBreakdown.NeedSetBonusPoints),
                // Menggunakan objek baru bertipe `FinalScoreComponentValue` dengan argumen (”DONATION”, correctedBreakdown.DonationPoints) sebagai bagian ekspresi
                // yang sedang disusun dalam ComputeFinalScoresAsync.
                new FinalScoreComponentValue("DONATION", correctedBreakdown.DonationPoints),
                // Menggunakan objek baru bertipe `FinalScoreComponentValue` dengan argumen (”GOLD”, correctedBreakdown.GoldPoints) sebagai bagian ekspresi yang
                // sedang disusun dalam ComputeFinalScoresAsync.
                new FinalScoreComponentValue("GOLD", correctedBreakdown.GoldPoints),
                // Menggunakan objek baru bertipe `FinalScoreComponentValue` dengan argumen (”PENSION”, correctedBreakdown.PensionPoints) sebagai bagian ekspresi
                // yang sedang disusun dalam ComputeFinalScoresAsync.
                new FinalScoreComponentValue("PENSION", correctedBreakdown.PensionPoints),
                // Menggunakan objek baru bertipe `FinalScoreComponentValue` dengan argumen (”SAVING_GOAL”, correctedBreakdown.SavingGoalPointsEffective) sebagai
                // bagian ekspresi yang sedang disusun dalam ComputeFinalScoresAsync.
                new FinalScoreComponentValue("SAVING_GOAL", correctedBreakdown.SavingGoalPointsEffective),
                // Menggunakan objek baru bertipe `FinalScoreComponentValue` dengan argumen (”MISSION_PENALTY”, -correctedBreakdown.MissionPenaltyPoints) sebagai
                // bagian ekspresi yang sedang disusun dalam ComputeFinalScoresAsync.
                new FinalScoreComponentValue("MISSION_PENALTY", -correctedBreakdown.MissionPenaltyPoints),
                // Menggunakan objek baru bertipe `FinalScoreComponentValue` dengan argumen (”LOAN_PENALTY”, -correctedBreakdown.LoanPenaltyPoints) sebagai bagian
                // ekspresi yang sedang disusun dalam ComputeFinalScoresAsync.
                new FinalScoreComponentValue("LOAN_PENALTY", -correctedBreakdown.LoanPenaltyPoints)
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam ComputeFinalScoresAsync.
            };

            // Mengulangi setiap elemen `components`; elemen saat ini disimpan sebagai `component` bertipe `var` untuk diproses oleh badan loop dalam
            // ComputeFinalScoresAsync.
            foreach (var component in components)
            // Membuka scope loop setiap component dari `components`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ComputeFinalScoresAsync.
            {
                // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” insert into
                // session_final_score_components ( session_final_score_component_id, session_id, session_participant_id, session_final_score_id, component...`;
                // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
                // ComputeFinalScoresAsync.
                await conn.ExecuteAsync(
                    // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” insert into session_final_score_components (
                    // session_final_score_component_id, session_id, session_participant_id, session_final_score_id, component_code,... sebagai argumen ke
                    // `conn.ExecuteAsync`.
                    new CommandDefinition(
                        // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                        // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_final_score_components (`.
                        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
                        // `session_final_score_component_id,`.
                        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
                        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
                        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_final_score_id,`.
                        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `component_code,`.
                        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `points,`.
                        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `source_event_id,`.
                        // Baris literal 10: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
                        // Baris literal 11: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                        // Baris literal 12: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
                        // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
                        // SQL: `@componentId,`.
                        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
                        // SQL: `@sessionId,`.
                        // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
                        // SQL: `@sessionParticipantId,`.
                        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
                        // SQL: `@scoreId,`.
                        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
                        // SQL: `@componentCode,`.
                        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
                        // SQL: `@points,`.
                        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
                        // SQL: `@sourceEventId,`.
                        // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
                        // Baris literal 21: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                        // Baris literal 22: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                        """
                        insert into session_final_score_components (
                            session_final_score_component_id,
                            session_id,
                            session_participant_id,
                            session_final_score_id,
                            component_code,
                            points,
                            source_event_id,
                            created_at
                        )
                        values (
                            @componentId,
                            @sessionId,
                            @sessionParticipantId,
                            @scoreId,
                            @componentCode,
                            @points,
                            @sourceEventId,
                            now()
                        )
                        """,
                        // Meneruskan objek anonim yang mengelompokkan componentId, sessionId, sessionParticipantId, scoreId, componentCode, points, sourceEventId sebagai
                        // satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                        new
                        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                        // ComputeFinalScoresAsync.
                        {
                            // Meneruskan objek anonim yang mengelompokkan componentId, sessionId, sessionParticipantId, scoreId, componentCode, points, sourceEventId sebagai
                            // satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            componentId = Guid.NewGuid(),
                            // Meneruskan objek anonim yang mengelompokkan componentId, sessionId, sessionParticipantId, scoreId, componentCode, points, sourceEventId sebagai
                            // satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            sessionId,
                            // Meneruskan objek anonim yang mengelompokkan componentId, sessionId, sessionParticipantId, scoreId, componentCode, points, sourceEventId sebagai
                            // satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            sessionParticipantId = value.Value.Participant.SessionParticipantId,
                            // Meneruskan objek anonim yang mengelompokkan componentId, sessionId, sessionParticipantId, scoreId, componentCode, points, sourceEventId sebagai
                            // satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            scoreId,
                            // Meneruskan objek anonim yang mengelompokkan componentId, sessionId, sessionParticipantId, scoreId, componentCode, points, sourceEventId sebagai
                            // satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            componentCode = component.Code,
                            // Meneruskan `component.Points` (nilai poin) sebagai argumen ke `Math.Round`; Meneruskan `MidpointRounding.AwayFromZero` (nilai away dari zero)
                            // sebagai argumen ke `Math.Round`.
                            points = (int)Math.Round(component.Points, MidpointRounding.AwayFromZero),
                            // Meneruskan objek anonim yang mengelompokkan componentId, sessionId, sessionParticipantId, scoreId, componentCode, points, sourceEventId sebagai
                            // satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                            sourceEventId
                        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ComputeFinalScoresAsync.
                        },
                        // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                        tx,
                        // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                        // bernama `cancellationToken`.
                        cancellationToken: ct));
            // Menutup scope loop setiap component dari `components`; bagian berikut berada di luar batas blok tersebut dalam ComputeFinalScoresAsync.
            }
        // Menutup scope loop dengan syarat `index < finalRanking.Count`; bagian berikut berada di luar batas blok tersebut dalam ComputeFinalScoresAsync.
        }

        // Menjalankan hasil operasi asinkron mengesahkan transaksi `tx` sehingga perubahan yang terkumpul menjadi permanen; await menunggu hasil tanpa
        // memblokir thread selama operasi belum selesai dalam ComputeFinalScoresAsync.
        await tx.CommitAsync(ct);
    // Menutup scope metode ComputeFinalScoresAsync; bagian berikut berada di luar batas blok tersebut dalam ComputeFinalScoresAsync.
    }

    // Mendefinisikan metode `LoadPlayerChildrenAsync` dengan hasil bertipe `Task`; operasi ini menangani load pemain children asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `conn` bertipe
    // `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `sessionPlayerIds` bertipe `Guid[]`
    // membawa nilai sesi pemain identitas; Parameter `players` bertipe `IReadOnlyDictionary<Guid, SessionPlayerStateDto>` membawa nilai pemain;
    // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
    // aplikasi berhenti.
    private static async Task LoadPlayerChildrenAsync(
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `sessionPlayerIds` bertipe `Guid[]` membawa nilai sesi pemain identitas.
        Guid[] sessionPlayerIds,
        // Parameter `players` bertipe `IReadOnlyDictionary<Guid, SessionPlayerStateDto>` membawa nilai pemain.
        IReadOnlyDictionary<Guid, SessionPlayerStateDto> players,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode LoadPlayerChildrenAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LoadPlayerChildrenAsync.
    {
        // Menyiapkan variabel lokal `bahanSql` untuk nilai bahan SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang
        // dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string bahanSql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select spi.session_participant_id as session_player_id,
        // asset.display_name as nama, spi.qty as jumlah`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from session_participant_inventory spi`.
        // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_game_assets asset on asset.ruleset_game_asset_id
        // = spi.ruleset_game_asset_id`.
        // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where spi.session_participant_id =
        // any(@sessionPlayerIds)`.
        // Baris literal 6: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by asset.display_name asc`.
        // Baris literal 7: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string bahanSql = """
            select spi.session_participant_id as session_player_id, asset.display_name as nama, spi.qty as jumlah
            from session_participant_inventory spi
            join ruleset_game_assets asset on asset.ruleset_game_asset_id = spi.ruleset_game_asset_id
            where spi.session_participant_id = any(@sessionPlayerIds)
            order by asset.display_name asc
            """;

        // Menyiapkan variabel lokal `kebutuhanSql` untuk nilai kebutuhan SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya.
        // Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string kebutuhanSql =
        // ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `spn.session_participant_id as
        // session_player_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rn.item_name as nama,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce(rn.need_tier, '') as
        // tipe`.
        // Baris literal 6: FROM memilih tabel/subquery sumber pembacaan: `from session_participant_need_purchases spn`.
        // Baris literal 7: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_needs rn on rn.ruleset_need_id =
        // spn.ruleset_need_id`.
        // Baris literal 8: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where spn.session_participant_id =
        // any(@sessionPlayerIds)`.
        // Baris literal 9: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by spn.sort_order asc, rn.item_name asc`.
        // Baris literal 10: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string kebutuhanSql = """
            select
                spn.session_participant_id as session_player_id,
                rn.item_name as nama,
                coalesce(rn.need_tier, '') as tipe
            from session_participant_need_purchases spn
            join ruleset_needs rn on rn.ruleset_need_id = spn.ruleset_need_id
            where spn.session_participant_id = any(@sessionPlayerIds)
            order by spn.sort_order asc, rn.item_name asc
            """;

        // Menyiapkan variabel lokal `tujuanSql` untuk nilai tujuan SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang
        // dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string tujuanSql =
        // ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `spfg.session_participant_id
        // as session_player_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rfg.item_name as nama,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `spfg.current_amount,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `spfg.target_amount,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `spfg.status,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `spfg.purchased_at_day`.
        // Baris literal 9: FROM memilih tabel/subquery sumber pembacaan: `from session_participant_financial_goals spfg`.
        // Baris literal 10: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_financial_goals rfg on
        // rfg.ruleset_financial_goal_id = spfg.ruleset_financial_goal_id`.
        // Baris literal 11: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where spfg.session_participant_id =
        // any(@sessionPlayerIds)`.
        // Baris literal 12: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by coalesce(spfg.purchased_at_day,
        // 2147483647) asc, rfg.item_name asc`.
        // Baris literal 13: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string tujuanSql = """
            select
                spfg.session_participant_id as session_player_id,
                rfg.item_name as nama,
                spfg.current_amount,
                spfg.target_amount,
                spfg.status,
                spfg.purchased_at_day
            from session_participant_financial_goals spfg
            join ruleset_financial_goals rfg on rfg.ruleset_financial_goal_id = spfg.ruleset_financial_goal_id
            where spfg.session_participant_id = any(@sessionPlayerIds)
            order by coalesce(spfg.purchased_at_day, 2147483647) asc, rfg.item_name asc
            """;

        // Menyiapkan variabel lokal `targetSql` untuk nilai target SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang
        // dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string targetSql =
        // ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select spcm.session_participant_id as session_player_id,
        // rcm.mission_code as id, spcm.is_completed, spcm.is_failed, spcm.reward_applied`.
        // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from session_participant_collection_missions spcm`.
        // Baris literal 4: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_collection_missions rcm on
        // rcm.ruleset_collection_mission_id = spcm.ruleset_collection_mission_id`.
        // Baris literal 5: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where spcm.session_participant_id =
        // any(@sessionPlayerIds)`.
        // Baris literal 6: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by rcm.mission_code asc`.
        // Baris literal 7: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string targetSql = """
            select spcm.session_participant_id as session_player_id, rcm.mission_code as id, spcm.is_completed, spcm.is_failed, spcm.reward_applied
            from session_participant_collection_missions spcm
            join ruleset_collection_missions rcm on rcm.ruleset_collection_mission_id = spcm.ruleset_collection_mission_id
            where spcm.session_participant_id = any(@sessionPlayerIds)
            order by rcm.mission_code asc
            """;

        // Menyiapkan variabel lokal `counterSql` untuk nilai counter SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe
        // yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string counterSql =
        // ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `spac.session_participant_id
        // as session_player_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ra.action_id as aksi,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `spac.count`.
        // Baris literal 6: FROM memilih tabel/subquery sumber pembacaan: `from session_participant_action_counters spac`.
        // Baris literal 7: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_actions ra`.
        // Baris literal 8: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on ra.ruleset_version_id = spac.ruleset_version_id`.
        // Baris literal 9: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ra.ruleset_action_id = spac.ruleset_action_id`.
        // Baris literal 10: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where spac.session_participant_id =
        // any(@sessionPlayerIds)`.
        // Baris literal 11: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by ra.action_id asc`.
        // Baris literal 12: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string counterSql = """
            select
                spac.session_participant_id as session_player_id,
                ra.action_id as aksi,
                spac.count
            from session_participant_action_counters spac
            join ruleset_actions ra
              on ra.ruleset_version_id = spac.ruleset_version_id
             and ra.ruleset_action_id = spac.ruleset_action_id
            where spac.session_participant_id = any(@sessionPlayerIds)
            order by ra.action_id asc
            """;

        // Mengulangi setiap elemen `await conn.QueryAsync<BahanRow>(new CommandDefinition(bahanSql, new { sessionPlayerIds }, cancellationToken: ct))`;
        // elemen saat ini disimpan sebagai `row` bertipe `var` untuk diproses oleh badan loop dalam LoadPlayerChildrenAsync.
        foreach (var row in await conn.QueryAsync<BahanRow>(new CommandDefinition(bahanSql, new { sessionPlayerIds }, cancellationToken: ct)))
        // Membuka scope loop setiap row dari `await conn.QueryAsync<BahanRow>(new CommandDefinition(bahanSql, new { sessionPlayerIds }, cancellationToken:
        // ct))`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LoadPlayerChildrenAsync.
        {
            // Memeriksa mencari kunci `row.SessionPlayerId` pada `players`; hasil boolean menandakan kunci ditemukan dan argumen out menerima nilainya; blok if
            // hanya dijalankan ketika kondisi ini bernilai benar dalam LoadPlayerChildrenAsync.
            if (players.TryGetValue(row.SessionPlayerId, out var player))
            // Membuka scope cabang if untuk kondisi `players.TryGetValue(row.SessionPlayerId, out var player)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam LoadPlayerChildrenAsync.
            {
                // Menjalankan menambahkan `new BahanItemDto(row.Nama, row.Jumlah)` ke `player.Bahan` dalam LoadPlayerChildrenAsync.
                player.Bahan.Add(new BahanItemDto(row.Nama, row.Jumlah));
            // Menutup scope cabang if untuk kondisi `players.TryGetValue(row.SessionPlayerId, out var player)`; bagian berikut berada di luar batas blok
            // tersebut dalam LoadPlayerChildrenAsync.
            }
        // Menutup scope loop setiap row dari `await conn.QueryAsync<BahanRow>(new CommandDefinition(bahanSql, new { sessionPlayerIds }, cancellationToken:
        // ct))`; bagian berikut berada di luar batas blok tersebut dalam LoadPlayerChildrenAsync.
        }

        // Mengulangi setiap elemen `await conn.QueryAsync<KebutuhanRow>(new CommandDefinition(kebutuhanSql, new { sessionPlayerIds }, cancellationToken:
        // ct))`; elemen saat ini disimpan sebagai `row` bertipe `var` untuk diproses oleh badan loop dalam LoadPlayerChildrenAsync.
        foreach (var row in await conn.QueryAsync<KebutuhanRow>(new CommandDefinition(kebutuhanSql, new { sessionPlayerIds }, cancellationToken: ct)))
        // Membuka scope loop setiap row dari `await conn.QueryAsync<KebutuhanRow>(new CommandDefinition(kebutuhanSql, new { sessionPlayerIds },
        // cancellationToken: ct))`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LoadPlayerChildrenAsync.
        {
            // Memeriksa mencari kunci `row.SessionPlayerId` pada `players`; hasil boolean menandakan kunci ditemukan dan argumen out menerima nilainya; blok if
            // hanya dijalankan ketika kondisi ini bernilai benar dalam LoadPlayerChildrenAsync.
            if (players.TryGetValue(row.SessionPlayerId, out var player))
            // Membuka scope cabang if untuk kondisi `players.TryGetValue(row.SessionPlayerId, out var player)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam LoadPlayerChildrenAsync.
            {
                // Menjalankan menambahkan `new KebutuhanItemDto(row.Nama, row.Tipe)` ke `player.Kebutuhan` dalam LoadPlayerChildrenAsync.
                player.Kebutuhan.Add(new KebutuhanItemDto(row.Nama, row.Tipe));
            // Menutup scope cabang if untuk kondisi `players.TryGetValue(row.SessionPlayerId, out var player)`; bagian berikut berada di luar batas blok
            // tersebut dalam LoadPlayerChildrenAsync.
            }
        // Menutup scope loop setiap row dari `await conn.QueryAsync<KebutuhanRow>(new CommandDefinition(kebutuhanSql, new { sessionPlayerIds },
        // cancellationToken: ct))`; bagian berikut berada di luar batas blok tersebut dalam LoadPlayerChildrenAsync.
        }

        // Mengulangi setiap elemen `await conn.QueryAsync<TujuanFinansialRow>(new CommandDefinition(tujuanSql, new { sessionPlayerIds }, cancellationToken:
        // ct))`; elemen saat ini disimpan sebagai `row` bertipe `var` untuk diproses oleh badan loop dalam LoadPlayerChildrenAsync.
        foreach (var row in await conn.QueryAsync<TujuanFinansialRow>(new CommandDefinition(tujuanSql, new { sessionPlayerIds }, cancellationToken: ct)))
        // Membuka scope loop setiap row dari `await conn.QueryAsync<TujuanFinansialRow>(new CommandDefinition(tujuanSql, new { sessionPlayerIds },
        // cancellationToken: ct))`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LoadPlayerChildrenAsync.
        {
            // Memeriksa mencari kunci `row.SessionPlayerId` pada `players`; hasil boolean menandakan kunci ditemukan dan argumen out menerima nilainya; blok if
            // hanya dijalankan ketika kondisi ini bernilai benar dalam LoadPlayerChildrenAsync.
            if (players.TryGetValue(row.SessionPlayerId, out var player))
            // Membuka scope cabang if untuk kondisi `players.TryGetValue(row.SessionPlayerId, out var player)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam LoadPlayerChildrenAsync.
            {
                // Menjalankan menambahkan `new TujuanFinansialItemDto { Nama = row.Nama, CurrentAmount = row.CurrentAmount, TargetAmount = row.TargetAmount, Status
                // = row.Status, PurchasedAtDay = row.PurchasedAtDay }` ke `player.TujuanFinansial` dalam LoadPlayerChildrenAsync.
                player.TujuanFinansial.Add(new TujuanFinansialItemDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // LoadPlayerChildrenAsync.
                {
                    // Memperbarui `Nama` menggunakan `row.Nama` (nilai nama) dalam LoadPlayerChildrenAsync.
                    Nama = row.Nama,
                    // Memperbarui `CurrentAmount` menggunakan `row.CurrentAmount` (nilai saat ini nominal) dalam LoadPlayerChildrenAsync.
                    CurrentAmount = row.CurrentAmount,
                    // Memperbarui `TargetAmount` menggunakan `row.TargetAmount` (nilai target nominal) dalam LoadPlayerChildrenAsync.
                    TargetAmount = row.TargetAmount,
                    // Memperbarui `Status` menggunakan `row.Status` (nilai status) dalam LoadPlayerChildrenAsync.
                    Status = row.Status,
                    // Memperbarui `PurchasedAtDay` menggunakan `row.PurchasedAtDay` (nilai dibeli at hari) dalam LoadPlayerChildrenAsync.
                    PurchasedAtDay = row.PurchasedAtDay
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam LoadPlayerChildrenAsync.
                });
            // Menutup scope cabang if untuk kondisi `players.TryGetValue(row.SessionPlayerId, out var player)`; bagian berikut berada di luar batas blok
            // tersebut dalam LoadPlayerChildrenAsync.
            }
        // Menutup scope loop setiap row dari `await conn.QueryAsync<TujuanFinansialRow>(new CommandDefinition(tujuanSql, new { sessionPlayerIds },
        // cancellationToken: ct))`; bagian berikut berada di luar batas blok tersebut dalam LoadPlayerChildrenAsync.
        }

        // Mengulangi setiap elemen `await conn.QueryAsync<TargetKebutuhanRow>(new CommandDefinition(targetSql, new { sessionPlayerIds }, cancellationToken:
        // ct))`; elemen saat ini disimpan sebagai `row` bertipe `var` untuk diproses oleh badan loop dalam LoadPlayerChildrenAsync.
        foreach (var row in await conn.QueryAsync<TargetKebutuhanRow>(new CommandDefinition(targetSql, new { sessionPlayerIds }, cancellationToken: ct)))
        // Membuka scope loop setiap row dari `await conn.QueryAsync<TargetKebutuhanRow>(new CommandDefinition(targetSql, new { sessionPlayerIds },
        // cancellationToken: ct))`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LoadPlayerChildrenAsync.
        {
            // Memeriksa mencari kunci `row.SessionPlayerId` pada `players`; hasil boolean menandakan kunci ditemukan dan argumen out menerima nilainya; blok if
            // hanya dijalankan ketika kondisi ini bernilai benar dalam LoadPlayerChildrenAsync.
            if (players.TryGetValue(row.SessionPlayerId, out var player))
            // Membuka scope cabang if untuk kondisi `players.TryGetValue(row.SessionPlayerId, out var player)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam LoadPlayerChildrenAsync.
            {
                // Menjalankan menambahkan `new TargetKebutuhanProgressDto(row.Id, row.IsCompleted, row.IsFailed, row.RewardApplied)` ke `player.TargetKebutuhan`
                // dalam LoadPlayerChildrenAsync.
                player.TargetKebutuhan.Add(new TargetKebutuhanProgressDto(row.Id, row.IsCompleted, row.IsFailed, row.RewardApplied));
            // Menutup scope cabang if untuk kondisi `players.TryGetValue(row.SessionPlayerId, out var player)`; bagian berikut berada di luar batas blok
            // tersebut dalam LoadPlayerChildrenAsync.
            }
        // Menutup scope loop setiap row dari `await conn.QueryAsync<TargetKebutuhanRow>(new CommandDefinition(targetSql, new { sessionPlayerIds },
        // cancellationToken: ct))`; bagian berikut berada di luar batas blok tersebut dalam LoadPlayerChildrenAsync.
        }

        // Mengulangi setiap elemen `await conn.QueryAsync<ActionCounterRow>(new CommandDefinition(counterSql, new { sessionPlayerIds }, cancellationToken:
        // ct))`; elemen saat ini disimpan sebagai `row` bertipe `var` untuk diproses oleh badan loop dalam LoadPlayerChildrenAsync.
        foreach (var row in await conn.QueryAsync<ActionCounterRow>(new CommandDefinition(counterSql, new { sessionPlayerIds }, cancellationToken: ct)))
        // Membuka scope loop setiap row dari `await conn.QueryAsync<ActionCounterRow>(new CommandDefinition(counterSql, new { sessionPlayerIds },
        // cancellationToken: ct))`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LoadPlayerChildrenAsync.
        {
            // Memeriksa mencari kunci `row.SessionPlayerId` pada `players`; hasil boolean menandakan kunci ditemukan dan argumen out menerima nilainya; blok if
            // hanya dijalankan ketika kondisi ini bernilai benar dalam LoadPlayerChildrenAsync.
            if (players.TryGetValue(row.SessionPlayerId, out var player))
            // Membuka scope cabang if untuk kondisi `players.TryGetValue(row.SessionPlayerId, out var player)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam LoadPlayerChildrenAsync.
            {
                // Menjalankan menambahkan `new ActionCounterDto(row.Aksi, row.Count)` ke `player.ActionCounters` dalam LoadPlayerChildrenAsync.
                player.ActionCounters.Add(new ActionCounterDto(row.Aksi, row.Count));
            // Menutup scope cabang if untuk kondisi `players.TryGetValue(row.SessionPlayerId, out var player)`; bagian berikut berada di luar batas blok
            // tersebut dalam LoadPlayerChildrenAsync.
            }
        // Menutup scope loop setiap row dari `await conn.QueryAsync<ActionCounterRow>(new CommandDefinition(counterSql, new { sessionPlayerIds },
        // cancellationToken: ct))`; bagian berikut berada di luar batas blok tersebut dalam LoadPlayerChildrenAsync.
        }
    // Menutup scope metode LoadPlayerChildrenAsync; bagian berikut berada di luar batas blok tersebut dalam LoadPlayerChildrenAsync.
    }

    // Mendefinisikan metode `LoadDonationEventsAsync` dengan hasil bertipe `Task<List<DonationEventDto>>`; operasi ini menangani load donasi event
    // asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `conn`
    // bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan
    // agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task<List<DonationEventDto>> LoadDonationEventsAsync(
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode LoadDonationEventsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LoadDonationEventsAsync.
    {
        // Menyiapkan variabel lokal `eventsSql` untuk nilai event SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang
        // dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string eventsSql =
        // ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `donation.donation_event_id as
        // event_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `donation.event_ke,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `donation.day,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce((`.
        // Baris literal 7: SELECT menentukan nilai atau kolom yang dikembalikan query: `select jsonb_agg(`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `jsonb_build_object(`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'rank',
        // coalesce((event.payload->>'rank')::int, 0),`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'session_player_id',
        // event.session_player_id,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'player_order_no',
        // participant.player_order_no,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'total_donasi',
        // coalesce(balance.total_donasi, 0)`.
        // Baris literal 13: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 14: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by coalesce((event.payload->>'rank')::int,
        // 0)`.
        // Baris literal 15: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 16: FROM memilih tabel/subquery sumber pembacaan: `from events event`.
        // Baris literal 17: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join session_participants participant`.
        // Baris literal 18: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on participant.session_id = event.session_id`.
        // Baris literal 19: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and participant.session_participant_id =
        // event.session_player_id`.
        // Baris literal 20: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join session_participant_balances balance`.
        // Baris literal 21: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on balance.session_id = event.session_id`.
        // Baris literal 22: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and balance.session_participant_id =
        // event.session_player_id`.
        // Baris literal 23: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where event.session_id = donation.session_id`.
        // Baris literal 24: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and event.action_type = 'PoinPeringkatDonasi'`.
        // Baris literal 25: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and greatest(1, ((event.day_index + 3) / 7)) =
        // donation.event_ke`.
        // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `), '[]'::jsonb)::text as
        // rankings_json`.
        // Baris literal 27: FROM memilih tabel/subquery sumber pembacaan: `from session_donation_events donation`.
        // Baris literal 28: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where donation.session_id = @sessionId`.
        // Baris literal 29: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by event_ke asc`.
        // Baris literal 30: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string eventsSql = """
            select
                donation.donation_event_id as event_id,
                donation.event_ke,
                donation.day,
                coalesce((
                    select jsonb_agg(
                        jsonb_build_object(
                            'rank', coalesce((event.payload->>'rank')::int, 0),
                            'session_player_id', event.session_player_id,
                            'player_order_no', participant.player_order_no,
                            'total_donasi', coalesce(balance.total_donasi, 0)
                        )
                        order by coalesce((event.payload->>'rank')::int, 0)
                    )
                    from events event
                    join session_participants participant
                      on participant.session_id = event.session_id
                     and participant.session_participant_id = event.session_player_id
                    left join session_participant_balances balance
                      on balance.session_id = event.session_id
                     and balance.session_participant_id = event.session_player_id
                    where event.session_id = donation.session_id
                      and event.action_type = 'PoinPeringkatDonasi'
                      and greatest(1, ((event.day_index + 3) / 7)) = donation.event_ke
                ), '[]'::jsonb)::text as rankings_json
            from session_donation_events donation
            where donation.session_id = @sessionId
            order by event_ke asc
            """;

        // Menyiapkan variabel lokal `eventRows` untuk nilai event baris dengan mematerialisasi urutan `(await conn.QueryAsync<DonationEventRow>( new
        // CommandDefinition(eventsSql, new { sessionId }, cancellationToken: ct)))` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var eventRows = (await conn.QueryAsync<DonationEventRow>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (eventsSql, new { sessionId }, cancellationToken: ct) sebagai argumen ke
            // `conn.QueryAsync<DonationEventRow>`; Meneruskan `eventsSql` (nilai event SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan
            // objek anonim yang mengelompokkan sessionId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal
            // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama
            // `cancellationToken`.
            new CommandDefinition(eventsSql, new { sessionId }, cancellationToken: ct))).ToList();
        // Menyiapkan variabel lokal `results` untuk nilai results dengan objek baru bertipe `List<DonationEventDto>` dengan argumen (eventRows.Count). Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var results = new List<DonationEventDto>(eventRows.Count);

        // Mengulangi setiap elemen `eventRows`; elemen saat ini disimpan sebagai `row` bertipe `var` untuk diproses oleh badan loop dalam
        // LoadDonationEventsAsync.
        foreach (var row in eventRows)
        // Membuka scope loop setiap row dari `eventRows`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LoadDonationEventsAsync.
        {
            // Menyiapkan variabel lokal `item` untuk nilai elemen dengan objek baru bertipe `DonationEventDto` dengan nilai awal sesuai konstruktornya. Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var item = new DonationEventDto
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // LoadDonationEventsAsync.
            {
                // Memperbarui `EventKe` menggunakan `row.EventKe` (nilai event ke) dalam LoadDonationEventsAsync.
                EventKe = row.EventKe,
                // Memperbarui `Day` menggunakan `row.Day` (nomor hari permainan yang menjadi konteks aktivitas) dalam LoadDonationEventsAsync.
                Day = row.Day
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam LoadDonationEventsAsync.
            };

            // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(row.RankingsJson)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // LoadDonationEventsAsync.
            if (!string.IsNullOrWhiteSpace(row.RankingsJson))
            // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(row.RankingsJson)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
            // dalam LoadDonationEventsAsync.
            {
                // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `row.RankingsJson`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
                using var document = JsonDocument.Parse(row.RankingsJson);
                // Mengulangi setiap elemen `document.RootElement.EnumerateArray()`; elemen saat ini disimpan sebagai `ranking` bertipe `var` untuk diproses oleh
                // badan loop dalam LoadDonationEventsAsync.
                foreach (var ranking in document.RootElement.EnumerateArray())
                // Membuka scope loop setiap ranking dari `document.RootElement.EnumerateArray()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // LoadDonationEventsAsync.
                {
                    // Menjalankan menambahkan `new DonationRankingDto { Rank = ranking.TryGetProperty(”rank”, out var rankProp) ? rankProp.GetInt32() : 0,
                    // SessionPlayerId = ranking.TryGetProperty(”session_player_id”, out v...` ke `item.Rankings` dalam LoadDonationEventsAsync.
                    item.Rankings.Add(new DonationRankingDto
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // LoadDonationEventsAsync.
                    {
                        // Memperbarui `Rank` menggunakan hasil pemilihan bersyarat: ketika `ranking.TryGetProperty(”rank”, out var rankProp)` benar gunakan
                        // `rankProp.GetInt32()`, jika tidak gunakan `0` dalam LoadDonationEventsAsync.
                        Rank = ranking.TryGetProperty("rank", out var rankProp) ? rankProp.GetInt32() : 0,
                        // Memperbarui `SessionPlayerId` menggunakan hasil pemilihan bersyarat: ketika `ranking.TryGetProperty(”session_player_id”, out var
                        // sessionPlayerProp)` benar gunakan `sessionPlayerProp.GetGuid()`, jika tidak gunakan `Guid.Empty` dalam LoadDonationEventsAsync.
                        SessionPlayerId = ranking.TryGetProperty("session_player_id", out var sessionPlayerProp)
                            // Meneruskan objek baru bertipe `DonationRankingDto` dengan nilai awal sesuai konstruktornya sebagai argumen ke `item.Rankings.Add`.
                            ? sessionPlayerProp.GetGuid()
                            // Meneruskan objek baru bertipe `DonationRankingDto` dengan nilai awal sesuai konstruktornya sebagai argumen ke `item.Rankings.Add`.
                            : Guid.Empty,
                        // Memperbarui `PlayerIndex` menggunakan hasil pemilihan bersyarat: ketika `ranking.TryGetProperty(”player_order_no”, out var playerIndexProp) &&
                        // playerIndexProp.ValueKind == JsonValueKind.Number` benar gunakan `playerIndexProp.GetInt32()`, jika tidak gunakan
                        // `ranking.TryGetProperty(”player_index”, out var playerIndexAliasProp) && playerIndexAliasProp.ValueKind == JsonValueKind.Number ?
                        // playerIndexAliasProp.GetInt32() : 0` dalam LoadDonationEventsAsync.
                        PlayerIndex = ranking.TryGetProperty("player_order_no", out var playerIndexProp) &&
                                      // Meneruskan objek baru bertipe `DonationRankingDto` dengan nilai awal sesuai konstruktornya sebagai argumen ke `item.Rankings.Add`.
                                      playerIndexProp.ValueKind == JsonValueKind.Number
                            // Meneruskan objek baru bertipe `DonationRankingDto` dengan nilai awal sesuai konstruktornya sebagai argumen ke `item.Rankings.Add`.
                            ? playerIndexProp.GetInt32()
                            // Meneruskan nilai literal `”player_index”` sebagai argumen ke `ranking.TryGetProperty`; Meneruskan `var playerIndexAliasProp` sebagai argumen ke
                            // `ranking.TryGetProperty`.
                            : ranking.TryGetProperty("player_index", out var playerIndexAliasProp) &&
                              // Meneruskan objek baru bertipe `DonationRankingDto` dengan nilai awal sesuai konstruktornya sebagai argumen ke `item.Rankings.Add`.
                              playerIndexAliasProp.ValueKind == JsonValueKind.Number
                            // Meneruskan objek baru bertipe `DonationRankingDto` dengan nilai awal sesuai konstruktornya sebagai argumen ke `item.Rankings.Add`.
                            ? playerIndexAliasProp.GetInt32()
                            // Meneruskan objek baru bertipe `DonationRankingDto` dengan nilai awal sesuai konstruktornya sebagai argumen ke `item.Rankings.Add`.
                            : 0,
                        // Memperbarui `TotalDonasi` menggunakan hasil pemilihan bersyarat: ketika `ranking.TryGetProperty(”total_donasi”, out var totalDonasiProp) &&
                        // totalDonasiProp.ValueKind == JsonValueKind.Number` benar gunakan `totalDonasiProp.GetInt32()`, jika tidak gunakan `0` dalam
                        // LoadDonationEventsAsync.
                        TotalDonasi = ranking.TryGetProperty("total_donasi", out var totalDonasiProp) &&
                                      // Meneruskan objek baru bertipe `DonationRankingDto` dengan nilai awal sesuai konstruktornya sebagai argumen ke `item.Rankings.Add`.
                                      totalDonasiProp.ValueKind == JsonValueKind.Number
                            // Meneruskan objek baru bertipe `DonationRankingDto` dengan nilai awal sesuai konstruktornya sebagai argumen ke `item.Rankings.Add`.
                            ? totalDonasiProp.GetInt32()
                            // Meneruskan objek baru bertipe `DonationRankingDto` dengan nilai awal sesuai konstruktornya sebagai argumen ke `item.Rankings.Add`.
                            : 0
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam LoadDonationEventsAsync.
                    });
                // Menutup scope loop setiap ranking dari `document.RootElement.EnumerateArray()`; bagian berikut berada di luar batas blok tersebut dalam
                // LoadDonationEventsAsync.
                }
            // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(row.RankingsJson)`; bagian berikut berada di luar batas blok tersebut dalam
            // LoadDonationEventsAsync.
            }

            // Menjalankan menambahkan `item` ke `results` dalam LoadDonationEventsAsync.
            results.Add(item);
        // Menutup scope loop setiap row dari `eventRows`; bagian berikut berada di luar batas blok tersebut dalam LoadDonationEventsAsync.
        }

        // Mengembalikan `results` (nilai results) kepada pemanggil dalam LoadDonationEventsAsync; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return results;
    // Menutup scope metode LoadDonationEventsAsync; bagian berikut berada di luar batas blok tersebut dalam LoadDonationEventsAsync.
    }

    // Mendefinisikan metode `DeleteSnapshotChildrenAsync` dengan hasil bertipe `Task`; operasi ini menangani delete snapshot keadaan children asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `conn` bertipe
    // `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction`
    // membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi
    // permainan yang menjadi batas data operasi ini; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan
    // ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task DeleteSnapshotChildrenAsync(
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode DeleteSnapshotChildrenAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DeleteSnapshotChildrenAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: DELETE menghapus baris pada tabel tujuan sesuai pembatas query: `delete from session_donation_events`.
        // Baris literal 3: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId;`.
        // Baris literal 4: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 5: DELETE menghapus baris pada tabel tujuan sesuai pembatas query: `delete from session_participant_inventory`.
        // Baris literal 6: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_participant_id in (`.
        // Baris literal 7: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_participant_id`.
        // Baris literal 8: FROM memilih tabel/subquery sumber pembacaan: `from session_participants`.
        // Baris literal 9: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 10: Pembatas literal/penutup `);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 11: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 12: DELETE menghapus baris pada tabel tujuan sesuai pembatas query: `delete from session_participant_need_purchases`.
        // Baris literal 13: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_participant_id in (`.
        // Baris literal 14: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_participant_id`.
        // Baris literal 15: FROM memilih tabel/subquery sumber pembacaan: `from session_participants`.
        // Baris literal 16: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 17: Pembatas literal/penutup `);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 18: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 19: DELETE menghapus baris pada tabel tujuan sesuai pembatas query: `delete from session_participant_financial_goals`.
        // Baris literal 20: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_participant_id in (`.
        // Baris literal 21: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_participant_id`.
        // Baris literal 22: FROM memilih tabel/subquery sumber pembacaan: `from session_participants`.
        // Baris literal 23: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 24: Pembatas literal/penutup `);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 25: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 26: DELETE menghapus baris pada tabel tujuan sesuai pembatas query: `delete from session_participant_collection_missions`.
        // Baris literal 27: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_participant_id in (`.
        // Baris literal 28: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_participant_id`.
        // Baris literal 29: FROM memilih tabel/subquery sumber pembacaan: `from session_participants`.
        // Baris literal 30: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 31: Pembatas literal/penutup `);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 32: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 33: DELETE menghapus baris pada tabel tujuan sesuai pembatas query: `delete from session_participant_action_counters`.
        // Baris literal 34: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_participant_id in (`.
        // Baris literal 35: SELECT menentukan nilai atau kolom yang dikembalikan query: `select session_participant_id`.
        // Baris literal 36: FROM memilih tabel/subquery sumber pembacaan: `from session_participants`.
        // Baris literal 37: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 38: Pembatas literal/penutup `);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 39: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            delete from session_donation_events
            where session_id = @sessionId;

            delete from session_participant_inventory
            where session_participant_id in (
                select session_participant_id
                from session_participants
                where session_id = @sessionId
            );

            delete from session_participant_need_purchases
            where session_participant_id in (
                select session_participant_id
                from session_participants
                where session_id = @sessionId
            );

            delete from session_participant_financial_goals
            where session_participant_id in (
                select session_participant_id
                from session_participants
                where session_id = @sessionId
            );

            delete from session_participant_collection_missions
            where session_participant_id in (
                select session_participant_id
                from session_participants
                where session_id = @sessionId
            );

            delete from session_participant_action_counters
            where session_participant_id in (
                select session_participant_id
                from session_participants
                where session_id = @sessionId
            );
            """;

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition(sql, new { sessionId }, tx,
        // cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai dalam DeleteSnapshotChildrenAsync.
        await conn.ExecuteAsync(new CommandDefinition(sql, new { sessionId }, tx, cancellationToken: ct));
    // Menutup scope metode DeleteSnapshotChildrenAsync; bagian berikut berada di luar batas blok tersebut dalam DeleteSnapshotChildrenAsync.
    }

    // Mendefinisikan metode `SavePlayerSnapshotAsync` dengan hasil bertipe `Task`; operasi ini menangani save pemain snapshot keadaan asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `conn` bertipe
    // `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction`
    // membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi
    // permainan yang menjadi batas data operasi ini; Parameter `player` bertipe `SessionPlayerStateDto` membawa nilai pemain; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task SavePlayerSnapshotAsync(
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `player` bertipe `SessionPlayerStateDto` membawa nilai pemain.
        SessionPlayerStateDto player,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode SavePlayerSnapshotAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SavePlayerSnapshotAsync.
    {
        // Menyiapkan variabel lokal `upsertPlayerStateSql` untuk nilai upsert pemain keadaan SQL dengan literal multiline yang dirinci pada komentar di
        // dekat deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // upsertPlayerStateSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_balances (session_id,
        // session_participant_id, coins, happiness, saving, total_donasi, created_at, updated_at)`.
        // Baris literal 3: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (@sessionId,
        // @sessionPlayerId, @coins, @happiness, @saving, @totalDonasi, now(), now())`.
        // Baris literal 4: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict
        // (session_participant_id) do update`.
        // Baris literal 5: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set coins = excluded.coins,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `happiness =
        // excluded.happiness,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `saving = excluded.saving,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `total_donasi =
        // excluded.total_donasi,`.
        // Baris literal 9: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
        // Baris literal 10: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string upsertPlayerStateSql = """
            insert into session_participant_balances (session_id, session_participant_id, coins, happiness, saving, total_donasi, created_at, updated_at)
            values (@sessionId, @sessionPlayerId, @coins, @happiness, @saving, @totalDonasi, now(), now())
            on conflict (session_participant_id) do update
            set coins = excluded.coins,
                happiness = excluded.happiness,
                saving = excluded.saving,
                total_donasi = excluded.total_donasi,
                updated_at = now()
            """;

        // Menyiapkan variabel lokal `updatePlayerNameSql` untuk nilai update pemain nama SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // updatePlayerNameSql = ”””`.
        // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update session_participants`.
        // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set player_name = @name`.
        // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and session_participant_id = @sessionPlayerId`.
        // Baris literal 6: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string updatePlayerNameSql = """
            update session_participants
            set player_name = @name
            where session_id = @sessionId
              and session_participant_id = @sessionPlayerId
            """;

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( upsertPlayerStateSql, new {
        // sessionId, sessionPlayerId = player.SessionPlayerId, coins = player.Coins, happiness = player.Happiness, saving = player.Sav...`; nilai hasil
        // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // SavePlayerSnapshotAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan `upsertPlayerStateSql` (nilai upsert pemain keadaan SQL) sebagai argumen ke konstruktor `CommandDefinition`.
            upsertPlayerStateSql,
            // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, coins, happiness, saving, totalDonasi sebagai satu nilai sebagai argumen
            // ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // SavePlayerSnapshotAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, coins, happiness, saving, totalDonasi sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                sessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, coins, happiness, saving, totalDonasi sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                sessionPlayerId = player.SessionPlayerId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, coins, happiness, saving, totalDonasi sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                coins = player.Coins,
                // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, coins, happiness, saving, totalDonasi sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                happiness = player.Happiness,
                // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, coins, happiness, saving, totalDonasi sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                saving = player.Saving,
                // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, coins, happiness, saving, totalDonasi sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                totalDonasi = player.TotalDonasi
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam SavePlayerSnapshotAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( updatePlayerNameSql, new {
        // sessionId, sessionPlayerId = player.SessionPlayerId, name = player.Name.Trim() }, tx, cancellationToken: ct)`; nilai hasil menunjukkan jumlah
        // baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam SavePlayerSnapshotAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan `updatePlayerNameSql` (nilai update pemain nama SQL) sebagai argumen ke konstruktor `CommandDefinition`.
            updatePlayerNameSql,
            // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, name sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // SavePlayerSnapshotAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, name sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                sessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, name sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                sessionPlayerId = player.SessionPlayerId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, name sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                name = player.Name.Trim()
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam SavePlayerSnapshotAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));

        // Menyiapkan variabel lokal `insertBahanSql` untuk nilai insert bahan SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string insertBahanSql =
        // ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_inventory (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_game_asset_id,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `qty,`.
        // Baris literal 8: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at,`.
        // Baris literal 9: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at`.
        // Baris literal 10: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 11: SELECT menentukan nilai atau kolom yang dikembalikan query: `select @sessionId, @sessionPlayerId, asset.ruleset_version_id,
        // asset.ruleset_game_asset_id, @jumlah, now(), now()`.
        // Baris literal 12: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_game_assets asset`.
        // Baris literal 13: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where asset.ruleset_version_id = (`.
        // Baris literal 14: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_version_id`.
        // Baris literal 15: FROM memilih tabel/subquery sumber pembacaan: `from sessions`.
        // Baris literal 16: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 17: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 18: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 19: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and asset.asset_type = 'INGREDIENT'`.
        // Baris literal 20: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(asset.display_name) = lower(@nama)`.
        // Baris literal 21: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertBahanSql = """
            insert into session_participant_inventory (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                qty,
                created_at,
                updated_at
            )
            select @sessionId, @sessionPlayerId, asset.ruleset_version_id, asset.ruleset_game_asset_id, @jumlah, now(), now()
            from ruleset_game_assets asset
            where asset.ruleset_version_id = (
                select ruleset_version_id
                from sessions
                where session_id = @sessionId
                limit 1
            )
              and asset.asset_type = 'INGREDIENT'
              and lower(asset.display_name) = lower(@nama)
            """;

        // Mengulangi setiap elemen `player.Bahan`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
        // SavePlayerSnapshotAsync.
        foreach (var item in player.Bahan)
        // Membuka scope loop setiap item dari `player.Bahan`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SavePlayerSnapshotAsync.
        {
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertBahanSql, new { sessionId,
            // sessionPlayerId = player.SessionPlayerId, nama = item.Nama, jumlah = item.Jumlah }, tx, cancellationToken: ct)`; nilai hasil menunjukkan jumlah
            // baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam SavePlayerSnapshotAsync.
            await conn.ExecuteAsync(new CommandDefinition(
                // Meneruskan `insertBahanSql` (nilai insert bahan SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                insertBahanSql,
                // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, nama, jumlah sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                new { sessionId, sessionPlayerId = player.SessionPlayerId, nama = item.Nama, jumlah = item.Jumlah },
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                tx,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct));
        // Menutup scope loop setiap item dari `player.Bahan`; bagian berikut berada di luar batas blok tersebut dalam SavePlayerSnapshotAsync.
        }

        // Menyiapkan variabel lokal `insertKebutuhanSql` untuk nilai insert kebutuhan SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string
        // insertKebutuhanSql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_need_purchases (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
        // `session_participant_need_purchase_id, session_id, session_participant_id, ruleset_version_id, ruleset_need_id, sort_order, paid_amount,
        // happiness_delta, purchased_at_day, created_at`.
        // Baris literal 4: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 5: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 6: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@entryId,`.
        // Baris literal 7: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionId,`.
        // Baris literal 8: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionPlayerId,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rn.ruleset_version_id,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rn.ruleset_need_id,`.
        // Baris literal 11: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sortOrder,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rn.purchase_price,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rn.happiness_points,`.
        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@purchasedAtDay,`.
        // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 16: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_needs rn`.
        // Baris literal 17: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rn.ruleset_version_id = (`.
        // Baris literal 18: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_version_id`.
        // Baris literal 19: FROM memilih tabel/subquery sumber pembacaan: `from sessions`.
        // Baris literal 20: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 21: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 22: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 23: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(rn.item_name) = lower(@nama)`.
        // Baris literal 24: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(coalesce(rn.need_tier, '')) = lower(@tipe)`.
        // Baris literal 25: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 26: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertKebutuhanSql = """
            insert into session_participant_need_purchases (
                session_participant_need_purchase_id, session_id, session_participant_id, ruleset_version_id, ruleset_need_id, sort_order, paid_amount, happiness_delta, purchased_at_day, created_at
            )
            select
                @entryId,
                @sessionId,
                @sessionPlayerId,
                rn.ruleset_version_id,
                rn.ruleset_need_id,
                @sortOrder,
                rn.purchase_price,
                rn.happiness_points,
                @purchasedAtDay,
                now()
            from ruleset_needs rn
            where rn.ruleset_version_id = (
                select ruleset_version_id
                from sessions
                where session_id = @sessionId
                limit 1
            )
              and lower(rn.item_name) = lower(@nama)
              and lower(coalesce(rn.need_tier, '')) = lower(@tipe)
            limit 1
            """;

        // Memulai loop dengan inisialisasi `var i = 0`, berjalan selama `i < player.Kebutuhan.Count`, lalu memperbarui pencacah melalui `i++` dalam
        // SavePlayerSnapshotAsync.
        for (var i = 0; i < player.Kebutuhan.Count; i++)
        // Membuka scope loop dengan syarat `i < player.Kebutuhan.Count`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // SavePlayerSnapshotAsync.
        {
            // Menyiapkan variabel lokal `item` untuk nilai elemen dengan `player.Kebutuhan[i]`, yaitu elemen koleksi yang dipilih melalui indeks atau kunci
            // tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var item = player.Kebutuhan[i];
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertKebutuhanSql, new { entryId
            // = Guid.NewGuid(), sessionId, sessionPlayerId = player.SessionPlayerId, nama = item.Nama, tipe = item.Tipe, sortOrder =...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // SavePlayerSnapshotAsync.
            await conn.ExecuteAsync(new CommandDefinition(
                // Meneruskan `insertKebutuhanSql` (nilai insert kebutuhan SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                insertKebutuhanSql,
                // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, sessionPlayerId, nama, tipe, sortOrder, purchasedAtDay sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // SavePlayerSnapshotAsync.
                {
                    // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, sessionPlayerId, nama, tipe, sortOrder, purchasedAtDay sebagai satu nilai sebagai
                    // argumen ke konstruktor `CommandDefinition`.
                    entryId = Guid.NewGuid(),
                    // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, sessionPlayerId, nama, tipe, sortOrder, purchasedAtDay sebagai satu nilai sebagai
                    // argumen ke konstruktor `CommandDefinition`.
                    sessionId,
                    // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, sessionPlayerId, nama, tipe, sortOrder, purchasedAtDay sebagai satu nilai sebagai
                    // argumen ke konstruktor `CommandDefinition`.
                    sessionPlayerId = player.SessionPlayerId,
                    // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, sessionPlayerId, nama, tipe, sortOrder, purchasedAtDay sebagai satu nilai sebagai
                    // argumen ke konstruktor `CommandDefinition`.
                    nama = item.Nama,
                    // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, sessionPlayerId, nama, tipe, sortOrder, purchasedAtDay sebagai satu nilai sebagai
                    // argumen ke konstruktor `CommandDefinition`.
                    tipe = item.Tipe,
                    // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, sessionPlayerId, nama, tipe, sortOrder, purchasedAtDay sebagai satu nilai sebagai
                    // argumen ke konstruktor `CommandDefinition`.
                    sortOrder = i + 1,
                    // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, sessionPlayerId, nama, tipe, sortOrder, purchasedAtDay sebagai satu nilai sebagai
                    // argumen ke konstruktor `CommandDefinition`.
                    purchasedAtDay = 1
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam SavePlayerSnapshotAsync.
                },
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                tx,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct));
        // Menutup scope loop dengan syarat `i < player.Kebutuhan.Count`; bagian berikut berada di luar batas blok tersebut dalam SavePlayerSnapshotAsync.
        }

        // Menyiapkan variabel lokal `insertTujuanSql` untuk nilai insert tujuan SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string insertTujuanSql
        // = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_financial_goals (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_financial_goal_id,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `current_amount,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `target_amount,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `status,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `purchased_at_day,`.
        // Baris literal 11: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at,`.
        // Baris literal 12: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at`.
        // Baris literal 13: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 14: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionId,`.
        // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionPlayerId,`.
        // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rfg.ruleset_version_id,`.
        // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
        // `rfg.ruleset_financial_goal_id,`.
        // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@currentAmount,`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@targetAmount,`.
        // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@status,`.
        // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@purchasedAtDay,`.
        // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now(),`.
        // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 25: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_financial_goals rfg`.
        // Baris literal 26: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rfg.ruleset_version_id = (`.
        // Baris literal 27: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_version_id`.
        // Baris literal 28: FROM memilih tabel/subquery sumber pembacaan: `from sessions`.
        // Baris literal 29: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 30: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 31: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 32: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(rfg.item_name) = lower(@nama)`.
        // Baris literal 33: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 34: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertTujuanSql = """
            insert into session_participant_financial_goals (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_financial_goal_id,
                current_amount,
                target_amount,
                status,
                purchased_at_day,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @sessionPlayerId,
                rfg.ruleset_version_id,
                rfg.ruleset_financial_goal_id,
                @currentAmount,
                @targetAmount,
                @status,
                @purchasedAtDay,
                now(),
                now()
            from ruleset_financial_goals rfg
            where rfg.ruleset_version_id = (
                select ruleset_version_id
                from sessions
                where session_id = @sessionId
                limit 1
            )
              and lower(rfg.item_name) = lower(@nama)
            limit 1
            """;

        // Mengulangi setiap elemen `player.TujuanFinansial`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
        // SavePlayerSnapshotAsync.
        foreach (var item in player.TujuanFinansial)
        // Membuka scope loop setiap item dari `player.TujuanFinansial`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // SavePlayerSnapshotAsync.
        {
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertTujuanSql, new { sessionId,
            // sessionPlayerId = player.SessionPlayerId, nama = item.Nama, currentAmount = item.CurrentAmount, targetAmount = item.Ta...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // SavePlayerSnapshotAsync.
            await conn.ExecuteAsync(new CommandDefinition(
                // Meneruskan `insertTujuanSql` (nilai insert tujuan SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                insertTujuanSql,
                // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, nama, currentAmount, targetAmount, status, purchasedAtDay sebagai satu
                // nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // SavePlayerSnapshotAsync.
                {
                    // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, nama, currentAmount, targetAmount, status, purchasedAtDay sebagai satu
                    // nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    sessionId,
                    // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, nama, currentAmount, targetAmount, status, purchasedAtDay sebagai satu
                    // nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    sessionPlayerId = player.SessionPlayerId,
                    // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, nama, currentAmount, targetAmount, status, purchasedAtDay sebagai satu
                    // nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    nama = item.Nama,
                    // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, nama, currentAmount, targetAmount, status, purchasedAtDay sebagai satu
                    // nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    currentAmount = item.CurrentAmount,
                    // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, nama, currentAmount, targetAmount, status, purchasedAtDay sebagai satu
                    // nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    targetAmount = item.TargetAmount,
                    // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, nama, currentAmount, targetAmount, status, purchasedAtDay sebagai satu
                    // nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    status = item.Status,
                    // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, nama, currentAmount, targetAmount, status, purchasedAtDay sebagai satu
                    // nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    purchasedAtDay = item.PurchasedAtDay
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam SavePlayerSnapshotAsync.
                },
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                tx,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct));
        // Menutup scope loop setiap item dari `player.TujuanFinansial`; bagian berikut berada di luar batas blok tersebut dalam SavePlayerSnapshotAsync.
        }

        // Menyiapkan variabel lokal `insertTargetSql` untuk nilai insert target SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string insertTargetSql
        // = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_collection_missions (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,
        // session_participant_id, ruleset_version_id, ruleset_collection_mission_id, is_completed, is_failed, reward_applied, assigned_at`.
        // Baris literal 4: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 5: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 6: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionId,`.
        // Baris literal 7: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionPlayerId,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rcm.ruleset_version_id,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
        // `rcm.ruleset_collection_mission_id,`.
        // Baris literal 10: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@isCompleted,`.
        // Baris literal 11: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@isFailed,`.
        // Baris literal 12: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@rewardApplied,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 14: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_collection_missions rcm`.
        // Baris literal 15: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rcm.ruleset_version_id = (`.
        // Baris literal 16: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ruleset_version_id`.
        // Baris literal 17: FROM memilih tabel/subquery sumber pembacaan: `from sessions`.
        // Baris literal 18: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
        // Baris literal 19: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 20: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 21: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(rcm.mission_code) = lower(@id)`.
        // Baris literal 22: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 23: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertTargetSql = """
            insert into session_participant_collection_missions (
                session_id, session_participant_id, ruleset_version_id, ruleset_collection_mission_id, is_completed, is_failed, reward_applied, assigned_at
            )
            select
                @sessionId,
                @sessionPlayerId,
                rcm.ruleset_version_id,
                rcm.ruleset_collection_mission_id,
                @isCompleted,
                @isFailed,
                @rewardApplied,
                now()
            from ruleset_collection_missions rcm
            where rcm.ruleset_version_id = (
                select ruleset_version_id
                from sessions
                where session_id = @sessionId
                limit 1
            )
              and lower(rcm.mission_code) = lower(@id)
            limit 1
            """;

        // Mengulangi setiap elemen `player.TargetKebutuhan`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
        // SavePlayerSnapshotAsync.
        foreach (var item in player.TargetKebutuhan)
        // Membuka scope loop setiap item dari `player.TargetKebutuhan`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // SavePlayerSnapshotAsync.
        {
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertTargetSql, new { sessionId,
            // sessionPlayerId = player.SessionPlayerId, id = item.Id, isCompleted = item.IsCompleted, isFailed = item.IsFailed, rewa...`; nilai hasil
            // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // SavePlayerSnapshotAsync.
            await conn.ExecuteAsync(new CommandDefinition(
                // Meneruskan `insertTargetSql` (nilai insert target SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                insertTargetSql,
                // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, id, isCompleted, isFailed, rewardApplied sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // SavePlayerSnapshotAsync.
                {
                    // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, id, isCompleted, isFailed, rewardApplied sebagai satu nilai sebagai
                    // argumen ke konstruktor `CommandDefinition`.
                    sessionId,
                    // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, id, isCompleted, isFailed, rewardApplied sebagai satu nilai sebagai
                    // argumen ke konstruktor `CommandDefinition`.
                    sessionPlayerId = player.SessionPlayerId,
                    // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, id, isCompleted, isFailed, rewardApplied sebagai satu nilai sebagai
                    // argumen ke konstruktor `CommandDefinition`.
                    id = item.Id,
                    // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, id, isCompleted, isFailed, rewardApplied sebagai satu nilai sebagai
                    // argumen ke konstruktor `CommandDefinition`.
                    isCompleted = item.IsCompleted,
                    // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, id, isCompleted, isFailed, rewardApplied sebagai satu nilai sebagai
                    // argumen ke konstruktor `CommandDefinition`.
                    isFailed = item.IsFailed,
                    // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, id, isCompleted, isFailed, rewardApplied sebagai satu nilai sebagai
                    // argumen ke konstruktor `CommandDefinition`.
                    rewardApplied = item.RewardApplied
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam SavePlayerSnapshotAsync.
                },
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                tx,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct));
        // Menutup scope loop setiap item dari `player.TargetKebutuhan`; bagian berikut berada di luar batas blok tersebut dalam SavePlayerSnapshotAsync.
        }

        // Menyiapkan variabel lokal `insertCounterSql` untuk nilai insert counter SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string insertCounterSql
        // = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_action_counters (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_action_id,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count`.
        // Baris literal 8: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 9: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 10: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionId,`.
        // Baris literal 11: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionPlayerId,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `s.ruleset_version_id,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ra.ruleset_action_id,`.
        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@count`.
        // Baris literal 15: FROM memilih tabel/subquery sumber pembacaan: `from sessions s`.
        // Baris literal 16: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_actions ra`.
        // Baris literal 17: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on ra.ruleset_version_id = s.ruleset_version_id`.
        // Baris literal 18: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(ra.action_id) = lower(@aksi)`.
        // Baris literal 19: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and ra.is_active`.
        // Baris literal 20: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where s.session_id = @sessionId`.
        // Baris literal 21: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertCounterSql = """
            insert into session_participant_action_counters (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_action_id,
                count
            )
            select
                @sessionId,
                @sessionPlayerId,
                s.ruleset_version_id,
                ra.ruleset_action_id,
                @count
            from sessions s
            join ruleset_actions ra
              on ra.ruleset_version_id = s.ruleset_version_id
             and lower(ra.action_id) = lower(@aksi)
             and ra.is_active
            where s.session_id = @sessionId
            """;

        // Mengulangi setiap elemen `player.ActionCounters`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
        // SavePlayerSnapshotAsync.
        foreach (var item in player.ActionCounters)
        // Membuka scope loop setiap item dari `player.ActionCounters`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // SavePlayerSnapshotAsync.
        {
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertCounterSql, new { sessionId,
            // sessionPlayerId = player.SessionPlayerId, aksi = item.Aksi, count = item.Count }, tx, cancellationToken: ct)`; nilai hasil menunjukkan jumlah
            // baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam SavePlayerSnapshotAsync.
            await conn.ExecuteAsync(new CommandDefinition(
                // Meneruskan `insertCounterSql` (nilai insert counter SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                insertCounterSql,
                // Meneruskan objek anonim yang mengelompokkan sessionId, sessionPlayerId, aksi, count sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                new { sessionId, sessionPlayerId = player.SessionPlayerId, aksi = item.Aksi, count = item.Count },
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                tx,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct));
        // Menutup scope loop setiap item dari `player.ActionCounters`; bagian berikut berada di luar batas blok tersebut dalam SavePlayerSnapshotAsync.
        }
    // Menutup scope metode SavePlayerSnapshotAsync; bagian berikut berada di luar batas blok tersebut dalam SavePlayerSnapshotAsync.
    }

    // Mendefinisikan metode `SaveDonationEventAsync` dengan hasil bertipe `Task`; operasi ini menangani save donasi event asinkron. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `conn` bertipe `NpgsqlConnection`
    // membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi
    // basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang
    // menjadi batas data operasi ini; Parameter `donationEvent` bertipe `DonationEventDto` membawa nilai donasi event; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task SaveDonationEventAsync(
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `donationEvent` bertipe `DonationEventDto` membawa nilai donasi event.
        DonationEventDto donationEvent,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode SaveDonationEventAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SaveDonationEventAsync.
    {
        // Menyiapkan variabel lokal `eventId` untuk identitas unik event untuk pencatatan dan pemeriksaan duplikasi dengan memanggil `Guid.NewGuid` dengan
        // tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var eventId = Guid.NewGuid();

        // Menyiapkan variabel lokal `insertEventSql` untuk nilai insert event SQL dengan literal multiline yang dirinci pada komentar di dekat
        // deklarasinya. Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string insertEventSql =
        // ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_donation_events (donation_event_id,
        // session_id, event_ke, day, created_at)`.
        // Baris literal 3: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (@eventId,
        // @sessionId, @eventKe, @day, now())`.
        // Baris literal 4: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string insertEventSql = """
            insert into session_donation_events (donation_event_id, session_id, event_ke, day, created_at)
            values (@eventId, @sessionId, @eventKe, @day, now())
            """;

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( insertEventSql, new { eventId,
        // sessionId, eventKe = donationEvent.EventKe, day = donationEvent.Day }, tx, cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang
        // terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam SaveDonationEventAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan `insertEventSql` (nilai insert event SQL) sebagai argumen ke konstruktor `CommandDefinition`.
            insertEventSql,
            // Meneruskan objek anonim yang mengelompokkan eventId, sessionId, eventKe, day sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // SaveDonationEventAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan eventId, sessionId, eventKe, day sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                eventId,
                // Meneruskan objek anonim yang mengelompokkan eventId, sessionId, eventKe, day sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                sessionId,
                // Meneruskan objek anonim yang mengelompokkan eventId, sessionId, eventKe, day sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                eventKe = donationEvent.EventKe,
                // Meneruskan objek anonim yang mengelompokkan eventId, sessionId, eventKe, day sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                day = donationEvent.Day
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam SaveDonationEventAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode SaveDonationEventAsync; bagian berikut berada di luar batas blok tersebut dalam SaveDonationEventAsync.
    }

    // Mendefinisikan metode `SaveLastActionAsync` dengan hasil bertipe `Task`; operasi ini menangani save last aksi asinkron. Masukan: Parameter `conn`
    // bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tx` bertipe
    // `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `sessionId` bertipe `Guid` membawa
    // identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `newVersion` bertipe `long` membawa nilai new versi; Parameter
    // `request` bertipe `SaveSessionStateRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `ct`
    // bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private static Task SaveLastActionAsync(
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `newVersion` bertipe `long` membawa nilai new versi.
        long newVersion,
        // Parameter `request` bertipe `SaveSessionStateRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        SaveSessionStateRequest request,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode SaveLastActionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SaveLastActionAsync.
    {
        // Mengembalikan `Task.CompletedTask` (nilai selesai task) kepada pemanggil dalam SaveLastActionAsync; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return Task.CompletedTask;
    // Menutup scope metode SaveLastActionAsync; bagian berikut berada di luar batas blok tersebut dalam SaveLastActionAsync.
    }

    // Mendefinisikan metode `GetJsonOrEmpty` dengan hasil bertipe `string`; operasi ini menangani get JSON atau empty. Masukan: Parameter `element`
    // bertipe `JsonElement?` membawa nilai element; nilai null diizinkan ketika data opsional belum tersedia.
    private static string GetJsonOrEmpty(JsonElement? element)
    // Membuka scope metode GetJsonOrEmpty; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam GetJsonOrEmpty.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `TryGetJson(element, out var json)` benar gunakan `json`, jika tidak gunakan `”{}”` kepada
        // pemanggil dalam GetJsonOrEmpty; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return TryGetJson(element, out var json) ? json : "{}";
    // Menutup scope metode GetJsonOrEmpty; bagian berikut berada di luar batas blok tersebut dalam GetJsonOrEmpty.
    }

    // Mendefinisikan metode `ResolveWeekdayCode` dengan hasil bertipe `string`; operasi ini menangani resolve weekday kode. Masukan: Parameter `day`
    // bertipe `int` membawa nomor hari permainan yang menjadi konteks aktivitas.
    private static string ResolveWeekdayCode(int day)
    // Membuka scope metode ResolveWeekdayCode; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveWeekdayCode.
    {
        // Menyiapkan variabel lokal `normalized` untuk nilai normalized dengan sisa pembagian antara `((day - 1) % 7 + 7)` dan `7`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var normalized = ((day - 1) % 7 + 7) % 7;
        // Mengembalikan hasil pemetaan `normalized` melalui cabang pola switch yang cocok kepada pemanggil dalam ResolveWeekdayCode; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return normalized switch
        // Membuka scope pemetaan switch atas `normalized`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveWeekdayCode.
        {
            // Untuk pola `0`, menghasilkan nilai literal `”MON”` sebagai hasil switch.
            0 => "MON",
            // Untuk pola `1`, menghasilkan nilai literal `”TUE”` sebagai hasil switch.
            1 => "TUE",
            // Untuk pola `2`, menghasilkan nilai literal `”WED”` sebagai hasil switch.
            2 => "WED",
            // Untuk pola `3`, menghasilkan nilai literal `”THU”` sebagai hasil switch.
            3 => "THU",
            // Untuk pola `4`, menghasilkan nilai literal `”FRI”` sebagai hasil switch.
            4 => "FRI",
            // Untuk pola `5`, menghasilkan nilai literal `”SAT”` sebagai hasil switch.
            5 => "SAT",
            // Untuk pola `_`, menghasilkan nilai literal `”SUN”` sebagai hasil switch.
            _ => "SUN"
        // Menutup scope pemetaan switch atas `normalized`; bagian berikut berada di luar batas blok tersebut dalam ResolveWeekdayCode.
        };
    // Menutup scope metode ResolveWeekdayCode; bagian berikut berada di luar batas blok tersebut dalam ResolveWeekdayCode.
    }

    // Mendefinisikan metode `ResolvePhase` dengan hasil bertipe `string`; operasi ini menangani resolve phase. Masukan: Parameter `weekday` bertipe
    // `string` membawa nilai weekday; Parameter `isGameOver` bertipe `bool` membawa nilai berstatus game over.
    private static string ResolvePhase(string weekday, bool isGameOver)
    // Membuka scope metode ResolvePhase; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolvePhase.
    {
        // Memeriksa `isGameOver` (nilai berstatus game over); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolvePhase.
        if (isGameOver)
        // Membuka scope cabang if untuk kondisi `isGameOver`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolvePhase.
        {
            // Mengembalikan nilai literal `”GAME_END”` kepada pemanggil dalam ResolvePhase; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return "GAME_END";
        // Menutup scope cabang if untuk kondisi `isGameOver`; bagian berikut berada di luar batas blok tersebut dalam ResolvePhase.
        }

        // Mengembalikan hasil pemetaan `weekday` melalui cabang pola switch yang cocok kepada pemanggil dalam ResolvePhase; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return weekday switch
        // Membuka scope pemetaan switch atas `weekday`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolvePhase.
        {
            // Untuk pola `”FRI”`, menghasilkan nilai literal `”DONATION_DAY”` sebagai hasil switch.
            "FRI" => "DONATION_DAY",
            // Untuk pola `”SAT”`, menghasilkan nilai literal `”GOLD_INVESTMENT_DAY”` sebagai hasil switch.
            "SAT" => "GOLD_INVESTMENT_DAY",
            // Untuk pola `”SUN”`, menghasilkan nilai literal `”DAY_END”` sebagai hasil switch.
            "SUN" => "DAY_END",
            // Untuk pola `_`, menghasilkan nilai literal `”PLAYER_TURN”` sebagai hasil switch.
            _ => "PLAYER_TURN"
        // Menutup scope pemetaan switch atas `weekday`; bagian berikut berada di luar batas blok tersebut dalam ResolvePhase.
        };
    // Menutup scope metode ResolvePhase; bagian berikut berada di luar batas blok tersebut dalam ResolvePhase.
    }

    // Mendefinisikan metode `ResolveCurrentActionSlot` dengan hasil bertipe `int`; operasi ini menangani resolve saat ini aksi slot. Masukan: Parameter
    // `actionSlotsLeft` bertipe `int` membawa nilai aksi slots left.
    private static int ResolveCurrentActionSlot(int actionSlotsLeft)
    // Membuka scope metode ResolveCurrentActionSlot; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveCurrentActionSlot.
    {
        // Mengembalikan membatasi `3 - Math.Max(0, actionSlotsLeft)` agar tidak lebih kecil dari `1` dan tidak lebih besar dari `2` kepada pemanggil dalam
        // ResolveCurrentActionSlot; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Math.Clamp(3 - Math.Max(0, actionSlotsLeft), 1, 2);
    // Menutup scope metode ResolveCurrentActionSlot; bagian berikut berada di luar batas blok tersebut dalam ResolveCurrentActionSlot.
    }

    // Mendefinisikan metode `TryGetJson` dengan hasil bertipe `bool`; operasi ini menangani try get JSON. Masukan: Parameter `element` bertipe
    // `JsonElement?` membawa nilai element; nilai null diizinkan ketika data opsional belum tersedia; Parameter `json` bertipe `string` membawa nilai
    // JSON; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    private static bool TryGetJson(JsonElement? element, out string json)
    // Membuka scope metode TryGetJson; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetJson.
    {
        // Memperbarui `json` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryGetJson.
        json = string.Empty;
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!element.HasValue` dan `element.Value.ValueKind is
        // JsonValueKind.Undefined or JsonValueKind.Null`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryGetJson.
        if (!element.HasValue || element.Value.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        // Membuka scope cabang if untuk kondisi `!element.HasValue || element.Value.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetJson.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryGetJson; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!element.HasValue || element.Value.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null`; bagian
        // berikut berada di luar batas blok tersebut dalam TryGetJson.
        }

        // Memperbarui `json` menggunakan mengambil representasi JSON mentah dari `element.Value` untuk disimpan atau diteruskan dalam TryGetJson.
        json = element.Value.GetRawText();
        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryGetJson; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return true;
    // Menutup scope metode TryGetJson; bagian berikut berada di luar batas blok tersebut dalam TryGetJson.
    }

    // Mendefinisikan metode `ParseJsonOrEmpty` dengan hasil bertipe `JsonElement`; operasi ini menangani parse JSON atau empty. Masukan: Parameter
    // `json` bertipe `string?` membawa nilai JSON; nilai null diizinkan ketika data opsional belum tersedia.
    private static JsonElement ParseJsonOrEmpty(string? json)
    // Membuka scope metode ParseJsonOrEmpty; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ParseJsonOrEmpty.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `string.IsNullOrWhiteSpace(json) ? ”{}” :
        // json`. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
        // Mengembalikan membuat salinan `document.RootElement` agar hasil dapat digunakan terpisah dari objek sumber kepada pemanggil dalam
        // ParseJsonOrEmpty; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return document.RootElement.Clone();
    // Menutup scope metode ParseJsonOrEmpty; bagian berikut berada di luar batas blok tersebut dalam ParseJsonOrEmpty.
    }

    // Mendefinisikan metode `ResolveRequestedRulesetVersionAsync` dengan hasil bertipe `Task<RequestedRulesetVersionRow?>`; operasi ini menangani
    // resolve yang diminta aturan versi asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui
    // Task. Masukan: Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data;
    // Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan; Parameter `rulesetId` bertipe `Guid?`
    // membawa identitas kumpulan aturan permainan; nilai null diizinkan ketika data opsional belum tersedia; Parameter `instructorUserId` bertipe
    // `Guid?` membawa identitas instruktur pemilik sesi atau aturan; nilai null diizinkan ketika data opsional belum tersedia; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task<RequestedRulesetVersionRow?> ResolveRequestedRulesetVersionAsync(
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan.
        string mode,
        // Parameter `rulesetId` bertipe `Guid?` membawa identitas kumpulan aturan permainan; nilai null diizinkan ketika data opsional belum tersedia.
        Guid? rulesetId,
        // Parameter `instructorUserId` bertipe `Guid?` membawa identitas instruktur pemilik sesi atau aturan; nilai null diizinkan ketika data opsional
        // belum tersedia.
        Guid? instructorUserId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ResolveRequestedRulesetVersionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ResolveRequestedRulesetVersionAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: WITH menamai hasil query sementara (CTE) yang dapat digunakan oleh bagian SQL berikutnya: `with ranked_versions as (`.
        // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.ruleset_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.ruleset_version_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.mode,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.version,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `row_number() over (`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `partition by rv.ruleset_id`.
        // Baris literal 10: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by`.
        // Baris literal 11: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when rv.status = 'ACTIVE' then 0 else 1 end,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.version desc`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as rn`.
        // Baris literal 14: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions rv`.
        // Baris literal 15: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where upper(rv.mode) = @mode`.
        // Baris literal 16: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 17: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `r.ruleset_id,`.
        // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.ruleset_version_id,`.
        // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `upper(rv.mode) as mode`.
        // Baris literal 21: FROM memilih tabel/subquery sumber pembacaan: `from rulesets r`.
        // Baris literal 22: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ranked_versions rv on rv.ruleset_id = r.ruleset_id and
        // rv.rn = 1`.
        // Baris literal 23: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where (`.
        // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@rulesetId is not null`.
        // Baris literal 25: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and r.ruleset_id = @rulesetId`.
        // Baris literal 26: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not r.is_archived`.
        // Baris literal 27: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (`.
        // Baris literal 28: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `r.instructor_user_id is
        // null`.
        // Baris literal 29: Melanjutkan kondisi SQL dengan OR (alternatif syarat yang dapat terpenuhi): `or (@instructorUserId is not null and
        // r.instructor_user_id = @instructorUserId)`.
        // Baris literal 30: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 31: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 32: Melanjutkan kondisi SQL dengan OR (alternatif syarat yang dapat terpenuhi): `or (`.
        // Baris literal 33: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@rulesetId is null`.
        // Baris literal 34: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and r.instructor_user_id is null`.
        // Baris literal 35: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not r.is_archived`.
        // Baris literal 36: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 37: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by case when r.instructor_user_id is null
        // then 0 else 1 end,`.
        // Baris literal 38: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `r.created_at desc`.
        // Baris literal 39: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 40: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            with ranked_versions as (
                select
                    rv.ruleset_id,
                    rv.ruleset_version_id,
                    rv.mode,
                    rv.version,
                    row_number() over (
                        partition by rv.ruleset_id
                        order by
                            case when rv.status = 'ACTIVE' then 0 else 1 end,
                            rv.version desc
                    ) as rn
                from ruleset_versions rv
                where upper(rv.mode) = @mode
            )
            select
                r.ruleset_id,
                rv.ruleset_version_id,
                upper(rv.mode) as mode
            from rulesets r
            join ranked_versions rv on rv.ruleset_id = r.ruleset_id and rv.rn = 1
            where (
                    @rulesetId is not null
                    and r.ruleset_id = @rulesetId
                    and not r.is_archived
                    and (
                        r.instructor_user_id is null
                        or (@instructorUserId is not null and r.instructor_user_id = @instructorUserId)
                    )
                )
                or (
                    @rulesetId is null
                    and r.instructor_user_id is null
                    and not r.is_archived
                )
            order by case when r.instructor_user_id is null then 0 else 1 end,
                     r.created_at desc
            limit 1
            """;

        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<RequestedRulesetVersionRow>` dengan
        // `new CommandDefinition( sql, new { mode, rulesetId, instructorUserId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris hasil;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada pemanggil dalam ResolveRequestedRulesetVersionAsync; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.QuerySingleOrDefaultAsync<RequestedRulesetVersionRow>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( sql, new { mode, rulesetId, instructorUserId }, cancellationToken: ct) sebagai
            // argumen ke `conn.QuerySingleOrDefaultAsync<RequestedRulesetVersionRow>`.
            new CommandDefinition(
                // Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                sql,
                // Meneruskan objek anonim yang mengelompokkan mode, rulesetId, instructorUserId sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ResolveRequestedRulesetVersionAsync.
                {
                    // Meneruskan objek anonim yang mengelompokkan mode, rulesetId, instructorUserId sebagai satu nilai sebagai argumen ke konstruktor
                    // `CommandDefinition`.
                    mode,
                    // Meneruskan objek anonim yang mengelompokkan mode, rulesetId, instructorUserId sebagai satu nilai sebagai argumen ke konstruktor
                    // `CommandDefinition`.
                    rulesetId,
                    // Meneruskan objek anonim yang mengelompokkan mode, rulesetId, instructorUserId sebagai satu nilai sebagai argumen ke konstruktor
                    // `CommandDefinition`.
                    instructorUserId
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                // ResolveRequestedRulesetVersionAsync.
                },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct));
    // Menutup scope metode ResolveRequestedRulesetVersionAsync; bagian berikut berada di luar batas blok tersebut dalam
    // ResolveRequestedRulesetVersionAsync.
    }

    // Mendefinisikan metode `ResolveDefaultRelationalRulesetVersionIdAsync` dengan hasil bertipe `Task<Guid?>`; operasi ini menangani resolve bawaan
    // relational aturan versi identitas asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui
    // Task. Masukan: Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data;
    // Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task<Guid?> ResolveDefaultRelationalRulesetVersionIdAsync(
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan.
        string mode,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ResolveDefaultRelationalRulesetVersionIdAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ResolveDefaultRelationalRulesetVersionIdAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: WITH menamai hasil query sementara (CTE) yang dapat digunakan oleh bagian SQL berikutnya: `with ranked_defaults as (`.
        // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.ruleset_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.ruleset_version_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.mode,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.version,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `row_number() over (`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `partition by rv.ruleset_id`.
        // Baris literal 10: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by`.
        // Baris literal 11: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when rv.status = 'ACTIVE' then 0 else 1 end,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.version desc`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as rn`.
        // Baris literal 14: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions rv`.
        // Baris literal 15: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where upper(rv.mode) = @mode`.
        // Baris literal 16: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 17: SELECT menentukan nilai atau kolom yang dikembalikan query: `select rv.ruleset_version_id`.
        // Baris literal 18: FROM memilih tabel/subquery sumber pembacaan: `from rulesets r`.
        // Baris literal 19: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ranked_defaults rv on rv.ruleset_id = r.ruleset_id and
        // rv.rn = 1`.
        // Baris literal 20: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where r.instructor_user_id is null`.
        // Baris literal 21: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not r.is_archived`.
        // Baris literal 22: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by rv.version desc, r.created_at desc`.
        // Baris literal 23: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 24: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            with ranked_defaults as (
                select
                    rv.ruleset_id,
                    rv.ruleset_version_id,
                    rv.mode,
                    rv.version,
                    row_number() over (
                        partition by rv.ruleset_id
                        order by
                            case when rv.status = 'ACTIVE' then 0 else 1 end,
                            rv.version desc
                    ) as rn
                from ruleset_versions rv
                where upper(rv.mode) = @mode
            )
            select rv.ruleset_version_id
            from rulesets r
            join ranked_defaults rv on rv.ruleset_id = r.ruleset_id and rv.rn = 1
            where r.instructor_user_id is null
              and not r.is_archived
            order by rv.version desc, r.created_at desc
            limit 1
            """;

        // Mengembalikan hasil operasi asinkron menjalankan perintah basis data melalui `conn` dengan `new CommandDefinition(sql, new { mode },
        // cancellationToken: ct)` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada
        // pemanggil dalam ResolveDefaultRelationalRulesetVersionIdAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.ExecuteScalarAsync<Guid?>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen (sql, new { mode }, cancellationToken: ct) sebagai argumen ke
            // `conn.ExecuteScalarAsync<Guid?>`; Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan objek anonim yang
            // mengelompokkan mode sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`; Meneruskan `ct` (sinyal pembatalan agar operasi dapat
            // dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen bernama `cancellationToken`.
            new CommandDefinition(sql, new { mode }, cancellationToken: ct));
    // Menutup scope metode ResolveDefaultRelationalRulesetVersionIdAsync; bagian berikut berada di luar batas blok tersebut dalam
    // ResolveDefaultRelationalRulesetVersionIdAsync.
    }

    // Mendefinisikan metode `ResolveRulesetVersionByIdAsync` dengan hasil bertipe `Task<RequestedRulesetVersionRow?>`; operasi ini menangani resolve
    // aturan versi berdasarkan identitas asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui
    // Task. Masukan: Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data;
    // Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan; Parameter `rulesetVersionId` bertipe
    // `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; Parameter `instructorUserId` bertipe `Guid?`
    // membawa identitas instruktur pemilik sesi atau aturan; nilai null diizinkan ketika data opsional belum tersedia; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task<RequestedRulesetVersionRow?> ResolveRulesetVersionByIdAsync(
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan.
        string mode,
        // Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat.
        Guid rulesetVersionId,
        // Parameter `instructorUserId` bertipe `Guid?` membawa identitas instruktur pemilik sesi atau aturan; nilai null diizinkan ketika data opsional
        // belum tersedia.
        Guid? instructorUserId,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ResolveRulesetVersionByIdAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ResolveRulesetVersionByIdAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `r.ruleset_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rv.ruleset_version_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `upper(rv.mode) as mode`.
        // Baris literal 6: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_versions rv`.
        // Baris literal 7: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join rulesets r on r.ruleset_id = rv.ruleset_id`.
        // Baris literal 8: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rv.ruleset_version_id = @rulesetVersionId`.
        // Baris literal 9: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and upper(rv.mode) = @mode`.
        // Baris literal 10: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rv.status = 'ACTIVE'`.
        // Baris literal 11: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not r.is_archived`.
        // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `r.instructor_user_id is
        // null`.
        // Baris literal 14: Melanjutkan kondisi SQL dengan OR (alternatif syarat yang dapat terpenuhi): `or (@instructorUserId is not null and
        // r.instructor_user_id = @instructorUserId)`.
        // Baris literal 15: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 16: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 17: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select
                r.ruleset_id,
                rv.ruleset_version_id,
                upper(rv.mode) as mode
            from ruleset_versions rv
            join rulesets r on r.ruleset_id = rv.ruleset_id
            where rv.ruleset_version_id = @rulesetVersionId
              and upper(rv.mode) = @mode
              and rv.status = 'ACTIVE'
              and not r.is_archived
              and (
                  r.instructor_user_id is null
                  or (@instructorUserId is not null and r.instructor_user_id = @instructorUserId)
              )
            limit 1
            """;

        // Mengembalikan hasil operasi asinkron membaca satu hasil basis data melalui `conn.QuerySingleOrDefaultAsync<RequestedRulesetVersionRow>` dengan
        // `new CommandDefinition( sql, new { mode, rulesetVersionId, instructorUserId }, cancellationToken: ct)`; nilai default menunjukkan tidak ada baris
        // hasil; await menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada pemanggil dalam ResolveRulesetVersionByIdAsync; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return await conn.QuerySingleOrDefaultAsync<RequestedRulesetVersionRow>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( sql, new { mode, rulesetVersionId, instructorUserId }, cancellationToken: ct)
            // sebagai argumen ke `conn.QuerySingleOrDefaultAsync<RequestedRulesetVersionRow>`.
            new CommandDefinition(
                // Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                sql,
                // Meneruskan objek anonim yang mengelompokkan mode, rulesetVersionId, instructorUserId sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                new
                // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ResolveRulesetVersionByIdAsync.
                {
                    // Meneruskan objek anonim yang mengelompokkan mode, rulesetVersionId, instructorUserId sebagai satu nilai sebagai argumen ke konstruktor
                    // `CommandDefinition`.
                    mode,
                    // Meneruskan objek anonim yang mengelompokkan mode, rulesetVersionId, instructorUserId sebagai satu nilai sebagai argumen ke konstruktor
                    // `CommandDefinition`.
                    rulesetVersionId,
                    // Meneruskan objek anonim yang mengelompokkan mode, rulesetVersionId, instructorUserId sebagai satu nilai sebagai argumen ke konstruktor
                    // `CommandDefinition`.
                    instructorUserId
                // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
                // ResolveRulesetVersionByIdAsync.
                },
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct));
    // Menutup scope metode ResolveRulesetVersionByIdAsync; bagian berikut berada di luar batas blok tersebut dalam ResolveRulesetVersionByIdAsync.
    }

    // Mendefinisikan tipe class `SessionStateRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class SessionStateRow
    // Membuka scope tipe SessionStateRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionId` bertipe `Guid` untuk identitas unik sesi permainan yang menjadi batas data operasi ini; get menyediakan
        // pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid SessionId { get; init; }
        // Mendefinisikan properti `StateVersion` bertipe `long` untuk nilai keadaan versi; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public long StateVersion { get; init; }
        // Mendefinisikan properti `NextSequenceNumber` bertipe `long` untuk nilai next sequence number; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public long NextSequenceNumber { get; init; }
        // Mendefinisikan properti `Day` bertipe `int` untuk nomor hari permainan yang menjadi konteks aktivitas; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public int Day { get; init; }
        // Mendefinisikan properti `Turn` bertipe `int` untuk giliran pemain yang sedang berlangsung; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public int Turn { get; init; }
        // Mendefinisikan properti `ActionSlotsLeft` bertipe `int` untuk nilai aksi slots left; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek.
        public int ActionSlotsLeft { get; init; }
        // Mendefinisikan properti `FinishDay` bertipe `int` untuk nilai finish hari; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int FinishDay { get; init; }
        // Mendefinisikan properti `IsGameOver` bertipe `bool` untuk nilai berstatus game over; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek.
        public bool IsGameOver { get; init; }
        // Mendefinisikan properti `UiStateJson` bertipe `string` untuk nilai ui keadaan JSON; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek; nilai awalnya nilai literal `”{}”`.
        public string UiStateJson { get; init; } = "{}";
    // Menutup scope tipe SessionStateRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `SessionPlayerStateRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class SessionPlayerStateRow
    // Membuka scope tipe SessionPlayerStateRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionPlayerId` bertipe `Guid` untuk identitas keikutsertaan pemain pada sesi tertentu; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid SessionPlayerId { get; init; }
        // Mendefinisikan properti `UserId` bertipe `Guid` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public Guid UserId { get; init; }
        // Mendefinisikan properti `PlayerIndex` bertipe `int` untuk nilai pemain index; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int PlayerIndex { get; init; }
        // Mendefinisikan properti `Name` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Name { get; init; } = string.Empty;
        // Mendefinisikan properti `Coins` bertipe `int` untuk nilai coins; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek.
        public int Coins { get; init; }
        // Mendefinisikan properti `Happiness` bertipe `int` untuk nilai kebahagiaan; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int Happiness { get; init; }
        // Mendefinisikan properti `Saving` bertipe `int` untuk nilai tabungan; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek.
        public int Saving { get; init; }
        // Mendefinisikan properti `TotalDonasi` bertipe `int` untuk nilai total donasi; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int TotalDonasi { get; init; }
    // Menutup scope tipe SessionPlayerStateRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `BahanRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class BahanRow
    // Membuka scope tipe BahanRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionPlayerId` bertipe `Guid` untuk identitas keikutsertaan pemain pada sesi tertentu; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid SessionPlayerId { get; init; }
        // Mendefinisikan properti `Nama` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Nama { get; init; } = string.Empty;
        // Mendefinisikan properti `Jumlah` bertipe `int` untuk nilai jumlah; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek.
        public int Jumlah { get; init; }
    // Menutup scope tipe BahanRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `KebutuhanRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class KebutuhanRow
    // Membuka scope tipe KebutuhanRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionPlayerId` bertipe `Guid` untuk identitas keikutsertaan pemain pada sesi tertentu; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid SessionPlayerId { get; init; }
        // Mendefinisikan properti `Nama` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Nama { get; init; } = string.Empty;
        // Mendefinisikan properti `Tipe` bertipe `string` untuk nilai tipe; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Tipe { get; init; } = string.Empty;
    // Menutup scope tipe KebutuhanRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `TujuanFinansialRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class TujuanFinansialRow
    // Membuka scope tipe TujuanFinansialRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionPlayerId` bertipe `Guid` untuk identitas keikutsertaan pemain pada sesi tertentu; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid SessionPlayerId { get; init; }
        // Mendefinisikan properti `Nama` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Nama { get; init; } = string.Empty;
        // Mendefinisikan properti `CurrentAmount` bertipe `int` untuk nilai saat ini nominal; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek.
        public int CurrentAmount { get; init; }
        // Mendefinisikan properti `TargetAmount` bertipe `int?` untuk nilai target nominal; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; tanda ? mengizinkan nilai null.
        public int? TargetAmount { get; init; }
        // Mendefinisikan properti `Status` bertipe `string` untuk nilai status; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya nilai literal `”ONGOING”`.
        public string Status { get; init; } = "ONGOING";
        // Mendefinisikan properti `PurchasedAtDay` bertipe `int?` untuk nilai dibeli at hari; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek; tanda ? mengizinkan nilai null.
        public int? PurchasedAtDay { get; init; }
    // Menutup scope tipe TujuanFinansialRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `TargetKebutuhanRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class TargetKebutuhanRow
    // Membuka scope tipe TargetKebutuhanRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionPlayerId` bertipe `Guid` untuk identitas keikutsertaan pemain pada sesi tertentu; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid SessionPlayerId { get; init; }
        // Mendefinisikan properti `Id` bertipe `string` untuk nilai identitas; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Id { get; init; } = string.Empty;
        // Mendefinisikan properti `IsCompleted` bertipe `bool` untuk nilai berstatus selesai; get menyediakan pembacaan nilai, init membatasi pengisian
        // saat inisialisasi objek.
        public bool IsCompleted { get; init; }
        // Mendefinisikan properti `IsFailed` bertipe `bool` untuk nilai berstatus failed; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public bool IsFailed { get; init; }
        // Mendefinisikan properti `RewardApplied` bertipe `bool` untuk nilai reward applied; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public bool RewardApplied { get; init; }
    // Menutup scope tipe TargetKebutuhanRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `ActionCounterRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class ActionCounterRow
    // Membuka scope tipe ActionCounterRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionPlayerId` bertipe `Guid` untuk identitas keikutsertaan pemain pada sesi tertentu; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid SessionPlayerId { get; init; }
        // Mendefinisikan properti `Aksi` bertipe `string` untuk nilai aksi; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Aksi { get; init; } = string.Empty;
        // Mendefinisikan properti `Count` bertipe `int` untuk nilai jumlah; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek.
        public int Count { get; init; }
    // Menutup scope tipe ActionCounterRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `DonationEventRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class DonationEventRow
    // Membuka scope tipe DonationEventRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `EventId` bertipe `Guid` untuk identitas unik event untuk pencatatan dan pemeriksaan duplikasi; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid EventId { get; init; }
        // Mendefinisikan properti `EventKe` bertipe `int` untuk nilai event ke; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
        // objek.
        public int EventKe { get; init; }
        // Mendefinisikan properti `Day` bertipe `int` untuk nomor hari permainan yang menjadi konteks aktivitas; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public int Day { get; init; }
        // Mendefinisikan properti `RankingsJson` bertipe `string` untuk nilai rankings JSON; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek; nilai awalnya nilai literal `”[]”`.
        public string RankingsJson { get; init; } = "[]";
    // Menutup scope tipe DonationEventRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan tipe class `RequestedRulesetVersionRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class RequestedRulesetVersionRow
    // Membuka scope tipe RequestedRulesetVersionRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `RulesetId` bertipe `Guid` untuk identitas kumpulan aturan permainan; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek.
        public Guid RulesetId { get; init; }
        // Mendefinisikan properti `RulesetVersionId` bertipe `Guid` untuk identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang
        // tepat; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid RulesetVersionId { get; init; }
        // Mendefinisikan properti `Mode` bertipe `string` untuk mode permainan yang menentukan kelompok aturan yang digunakan; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
        public string Mode { get; init; } = string.Empty;
    // Menutup scope tipe RequestedRulesetVersionRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `SetupParticipant`; sealed mencegah tipe ini diturunkan lagi.
    private sealed record SetupParticipant(Guid SessionParticipantId, Guid UserId, int PlayerOrder);

    // Mendefinisikan tipe class `FinalScoreParticipantRow`; sealed mencegah tipe ini diturunkan lagi.
    private sealed class FinalScoreParticipantRow
    // Membuka scope tipe FinalScoreParticipantRow; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Mendefinisikan properti `SessionParticipantId` bertipe `Guid` untuk identitas keikutsertaan pemain pada sesi tertentu; get menyediakan pembacaan
        // nilai, init membatasi pengisian saat inisialisasi objek.
        public Guid SessionParticipantId { get; init; }
        // Mendefinisikan properti `UserId` bertipe `Guid` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai, init
        // membatasi pengisian saat inisialisasi objek.
        public Guid UserId { get; init; }
        // Mendefinisikan properti `TieBreakerNumber` bertipe `int?` untuk nilai tie breaker number; get menyediakan pembacaan nilai, init membatasi
        // pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
        public int? TieBreakerNumber { get; init; }
        // Mendefinisikan properti `PensionFund` bertipe `int` untuk nilai pension fund; get menyediakan pembacaan nilai, init membatasi pengisian saat
        // inisialisasi objek.
        public int PensionFund { get; init; }
    // Menutup scope tipe FinalScoreParticipantRow; bagian berikut berada di luar batas blok tersebut.
    }

    // Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `FinalScoreParticipantValue`; sealed mencegah tipe ini diturunkan lagi.
    private sealed record FinalScoreParticipantValue(
        // Parameter `Participant` bertipe `FinalScoreParticipantRow` membawa nilai participant.
        FinalScoreParticipantRow Participant,
        // Parameter `CashRemaining` bertipe `int` membawa nilai uang tunai tersisa.
        int CashRemaining,
        // Parameter `Breakdown` bertipe `AnalyticsHappinessBreakdown` membawa nilai breakdown.
        AnalyticsHappinessBreakdown Breakdown);

    // Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `FinalScoreComponentValue`; sealed mencegah tipe ini diturunkan lagi.
    private sealed record FinalScoreComponentValue(string Code, double Points);

// Menutup scope tipe SessionStateRepository; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetSectionDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetSectionDb
// Membuka scope tipe RulesetSectionDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `RulesetId` bertipe `Guid` untuk identitas kumpulan aturan permainan; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek.
    public Guid RulesetId { get; init; }
    // Mendefinisikan properti `RulesetVersionId` bertipe `Guid` untuk identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang
    // tepat; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
    public Guid RulesetVersionId { get; init; }
    // Mendefinisikan properti `Mode` bertipe `string` untuk mode permainan yang menentukan kelompok aturan yang digunakan; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Mode { get; init; } = string.Empty;
    // Mendefinisikan properti `Definition` bertipe `RulesetDefinitionDto` untuk definisi terstruktur komponen serta parameter aturan permainan; get
    // menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan
    // argumen ().
    public RulesetDefinitionDto Definition { get; init; } = new();
// Menutup scope tipe RulesetSectionDb; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan enum untuk membatasi pilihan nilai bernama `SaveSessionStateStatus`.
public enum SaveSessionStateStatus
// Membuka scope tipe SaveSessionStateStatus; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan pilihan enum `Saved`; nilai bilangan mengikuti urutan deklarasi enum.
    Saved,
    // Mendefinisikan pilihan enum `NotFound`; nilai bilangan mengikuti urutan deklarasi enum.
    NotFound,
    // Mendefinisikan pilihan enum `Stale`; nilai bilangan mengikuti urutan deklarasi enum.
    Stale
// Menutup scope tipe SaveSessionStateStatus; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `SaveSessionStateResult`; sealed mencegah tipe ini diturunkan lagi.
public sealed record SaveSessionStateResult(
    // Parameter `Status` bertipe `SaveSessionStateStatus` membawa nilai status.
    SaveSessionStateStatus Status,
    // Parameter `State` bertipe `SessionStateResponse?` membawa keadaan permainan yang menjadi sumber atau hasil pembaruan; nilai null diizinkan ketika
    // data opsional belum tersedia.
    SessionStateResponse? State,
    // Parameter `CurrentVersion` bertipe `long?` membawa nilai saat ini versi; nilai null diizinkan ketika data opsional belum tersedia.
    long? CurrentVersion)
// Membuka scope tipe SaveSessionStateResult; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Saved` dengan hasil bertipe `SaveSessionStateResult`; operasi ini menangani saved. Masukan: Parameter `state` bertipe
    // `SessionStateResponse` membawa keadaan permainan yang menjadi sumber atau hasil pembaruan. Nilai hasil langsung berasal dari objek baru dengan
    // tipe mengikuti konteks tujuan dan argumen (SaveSessionStateStatus.Saved, state, state.StateVersion).
    public static SaveSessionStateResult Saved(SessionStateResponse state) =>
        // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen (SaveSessionStateStatus.Saved, state, state.StateVersion) sebagai bagian
        // ekspresi yang sedang disusun dalam Saved.
        new(SaveSessionStateStatus.Saved, state, state.StateVersion);

    // Mendefinisikan metode `NotFound` dengan hasil bertipe `SaveSessionStateResult`; operasi ini menangani not found. Nilai hasil langsung berasal
    // dari objek baru dengan tipe mengikuti konteks tujuan dan argumen (SaveSessionStateStatus.NotFound, null, null).
    public static SaveSessionStateResult NotFound() =>
        // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen (SaveSessionStateStatus.NotFound, null, null) sebagai bagian ekspresi
        // yang sedang disusun dalam NotFound.
        new(SaveSessionStateStatus.NotFound, null, null);

    // Mendefinisikan metode `Stale` dengan hasil bertipe `SaveSessionStateResult`; operasi ini menangani stale. Masukan: Parameter `currentVersion`
    // bertipe `long` membawa nilai saat ini versi. Nilai hasil langsung berasal dari objek baru dengan tipe mengikuti konteks tujuan dan argumen
    // (SaveSessionStateStatus.Stale, null, currentVersion).
    public static SaveSessionStateResult Stale(long currentVersion) =>
        // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen (SaveSessionStateStatus.Stale, null, currentVersion) sebagai bagian
        // ekspresi yang sedang disusun dalam Stale.
        new(SaveSessionStateStatus.Stale, null, currentVersion);
// Menutup scope tipe SaveSessionStateResult; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `RulesetSectionCatalog`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetSectionCatalog
// Membuka scope tipe RulesetSectionCatalog; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan konstruktor RulesetSectionCatalog yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `gameConfig` bertipe `JsonElement` membawa nilai game konfigurasi; Parameter `bahan` bertipe `JsonElement` membawa nilai bahan; Parameter `resep`
    // bertipe `JsonElement` membawa nilai resep; Parameter `kebutuhan` bertipe `JsonElement` membawa nilai kebutuhan; Parameter `targetKebutuhan`
    // bertipe `JsonElement` membawa nilai target kebutuhan; Parameter `tujuanFinansial` bertipe `JsonElement` membawa nilai tujuan finansial; Parameter
    // `narasi` bertipe `JsonElement` membawa nilai narasi; Parameter `gameConfigValues` bertipe `RulesetSectionGameConfig` membawa nilai game
    // konfigurasi nilai; Parameter `bahanNames` bertipe `HashSet<string>` membawa nilai bahan nama; Parameter `kebutuhanTypes` bertipe
    // `Dictionary<string, string>` membawa nilai kebutuhan types; Parameter `targetKebutuhanIds` bertipe `HashSet<string>` membawa nilai target
    // kebutuhan identitas; Parameter `tujuanFinansialNames` bertipe `HashSet<string>` membawa nilai tujuan finansial nama; Parameter `actions` bertipe
    // `HashSet<string>` membawa nilai aksi.
    private RulesetSectionCatalog(
        // Parameter `gameConfig` bertipe `JsonElement` membawa nilai game konfigurasi.
        JsonElement gameConfig,
        // Parameter `bahan` bertipe `JsonElement` membawa nilai bahan.
        JsonElement bahan,
        // Parameter `resep` bertipe `JsonElement` membawa nilai resep.
        JsonElement resep,
        // Parameter `kebutuhan` bertipe `JsonElement` membawa nilai kebutuhan.
        JsonElement kebutuhan,
        // Parameter `targetKebutuhan` bertipe `JsonElement` membawa nilai target kebutuhan.
        JsonElement targetKebutuhan,
        // Parameter `tujuanFinansial` bertipe `JsonElement` membawa nilai tujuan finansial.
        JsonElement tujuanFinansial,
        // Parameter `narasi` bertipe `JsonElement` membawa nilai narasi.
        JsonElement narasi,
        // Parameter `gameConfigValues` bertipe `RulesetSectionGameConfig` membawa nilai game konfigurasi nilai.
        RulesetSectionGameConfig gameConfigValues,
        // Parameter `bahanNames` bertipe `HashSet<string>` membawa nilai bahan nama.
        HashSet<string> bahanNames,
        // Parameter `kebutuhanTypes` bertipe `Dictionary<string, string>` membawa nilai kebutuhan types.
        Dictionary<string, string> kebutuhanTypes,
        // Parameter `targetKebutuhanIds` bertipe `HashSet<string>` membawa nilai target kebutuhan identitas.
        HashSet<string> targetKebutuhanIds,
        // Parameter `tujuanFinansialNames` bertipe `HashSet<string>` membawa nilai tujuan finansial nama.
        HashSet<string> tujuanFinansialNames,
        // Parameter `actions` bertipe `HashSet<string>` membawa nilai aksi.
        HashSet<string> actions)
    // Membuka scope konstruktor RulesetSectionCatalog; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RulesetSectionCatalog.
    {
        // Memperbarui `GameConfig` menggunakan `gameConfig` (nilai game konfigurasi) dalam RulesetSectionCatalog.
        GameConfig = gameConfig;
        // Memperbarui `Bahan` menggunakan `bahan` (nilai bahan) dalam RulesetSectionCatalog.
        Bahan = bahan;
        // Memperbarui `Resep` menggunakan `resep` (nilai resep) dalam RulesetSectionCatalog.
        Resep = resep;
        // Memperbarui `Kebutuhan` menggunakan `kebutuhan` (nilai kebutuhan) dalam RulesetSectionCatalog.
        Kebutuhan = kebutuhan;
        // Memperbarui `TargetKebutuhan` menggunakan `targetKebutuhan` (nilai target kebutuhan) dalam RulesetSectionCatalog.
        TargetKebutuhan = targetKebutuhan;
        // Memperbarui `TujuanFinansial` menggunakan `tujuanFinansial` (nilai tujuan finansial) dalam RulesetSectionCatalog.
        TujuanFinansial = tujuanFinansial;
        // Memperbarui `Narasi` menggunakan `narasi` (nilai narasi) dalam RulesetSectionCatalog.
        Narasi = narasi;
        // Memperbarui `GameConfigValues` menggunakan `gameConfigValues` (nilai game konfigurasi nilai) dalam RulesetSectionCatalog.
        GameConfigValues = gameConfigValues;
        // Memperbarui `BahanNames` menggunakan `bahanNames` (nilai bahan nama) dalam RulesetSectionCatalog.
        BahanNames = bahanNames;
        // Memperbarui `KebutuhanTypes` menggunakan `kebutuhanTypes` (nilai kebutuhan types) dalam RulesetSectionCatalog.
        KebutuhanTypes = kebutuhanTypes;
        // Memperbarui `TargetKebutuhanIds` menggunakan `targetKebutuhanIds` (nilai target kebutuhan identitas) dalam RulesetSectionCatalog.
        TargetKebutuhanIds = targetKebutuhanIds;
        // Memperbarui `TujuanFinansialNames` menggunakan `tujuanFinansialNames` (nilai tujuan finansial nama) dalam RulesetSectionCatalog.
        TujuanFinansialNames = tujuanFinansialNames;
        // Memperbarui `Actions` menggunakan `actions` (nilai aksi) dalam RulesetSectionCatalog.
        Actions = actions;
    // Menutup scope konstruktor RulesetSectionCatalog; bagian berikut berada di luar batas blok tersebut dalam RulesetSectionCatalog.
    }

    // Mendefinisikan properti `GameConfig` bertipe `JsonElement` untuk nilai game konfigurasi; get menyediakan pembacaan nilai.
    public JsonElement GameConfig { get; }
    // Mendefinisikan properti `Bahan` bertipe `JsonElement` untuk nilai bahan; get menyediakan pembacaan nilai.
    public JsonElement Bahan { get; }
    // Mendefinisikan properti `Resep` bertipe `JsonElement` untuk nilai resep; get menyediakan pembacaan nilai.
    public JsonElement Resep { get; }
    // Mendefinisikan properti `Kebutuhan` bertipe `JsonElement` untuk nilai kebutuhan; get menyediakan pembacaan nilai.
    public JsonElement Kebutuhan { get; }
    // Mendefinisikan properti `TargetKebutuhan` bertipe `JsonElement` untuk nilai target kebutuhan; get menyediakan pembacaan nilai.
    public JsonElement TargetKebutuhan { get; }
    // Mendefinisikan properti `TujuanFinansial` bertipe `JsonElement` untuk nilai tujuan finansial; get menyediakan pembacaan nilai.
    public JsonElement TujuanFinansial { get; }
    // Mendefinisikan properti `Narasi` bertipe `JsonElement` untuk nilai narasi; get menyediakan pembacaan nilai.
    public JsonElement Narasi { get; }
    // Mendefinisikan properti `GameConfigValues` bertipe `RulesetSectionGameConfig` untuk nilai game konfigurasi nilai; get menyediakan pembacaan
    // nilai.
    public RulesetSectionGameConfig GameConfigValues { get; }
    // Mendefinisikan properti `BahanNames` bertipe `HashSet<string>` untuk nilai bahan nama; get menyediakan pembacaan nilai.
    public HashSet<string> BahanNames { get; }
    // Mendefinisikan properti `KebutuhanTypes` bertipe `Dictionary<string, string>` untuk nilai kebutuhan types; get menyediakan pembacaan nilai.
    public Dictionary<string, string> KebutuhanTypes { get; }
    // Mendefinisikan properti `TargetKebutuhanIds` bertipe `HashSet<string>` untuk nilai target kebutuhan identitas; get menyediakan pembacaan nilai.
    public HashSet<string> TargetKebutuhanIds { get; }
    // Mendefinisikan properti `TujuanFinansialNames` bertipe `HashSet<string>` untuk nilai tujuan finansial nama; get menyediakan pembacaan nilai.
    public HashSet<string> TujuanFinansialNames { get; }
    // Mendefinisikan properti `Actions` bertipe `HashSet<string>` untuk nilai aksi; get menyediakan pembacaan nilai.
    public HashSet<string> Actions { get; }

    // Mendefinisikan metode `FromDefinition` dengan hasil bertipe `RulesetSectionCatalog`; operasi ini menangani dari definisi. Masukan: Parameter
    // `definition` bertipe `RulesetDefinitionDto` membawa definisi terstruktur komponen serta parameter aturan permainan.
    public static RulesetSectionCatalog FromDefinition(RulesetDefinitionDto definition)
    // Membuka scope metode FromDefinition; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromDefinition.
    {
        // Menyiapkan variabel lokal `gameConfig` untuk nilai game konfigurasi dengan memanggil `SerializeElement` dengan `new { initialCoins =
        // definition.Settings.InitialCoins == 0 ? definition.Settings.StartingCash : definition.Settings.InitialCoins, initialHappiness =
        // definition.Settings.Initia...`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var gameConfig = SerializeElement(new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromDefinition.
        {
            // Meneruskan objek anonim yang mengelompokkan initialCoins, initialHappiness, initialSaving, actionsPerTurn, finishDay, minPlayers, maxPlayers
            // sebagai satu nilai sebagai argumen ke `SerializeElement`.
            initialCoins = definition.Settings.InitialCoins == 0
                // Meneruskan objek anonim yang mengelompokkan initialCoins, initialHappiness, initialSaving, actionsPerTurn, finishDay, minPlayers, maxPlayers
                // sebagai satu nilai sebagai argumen ke `SerializeElement`.
                ? definition.Settings.StartingCash
                // Meneruskan objek anonim yang mengelompokkan initialCoins, initialHappiness, initialSaving, actionsPerTurn, finishDay, minPlayers, maxPlayers
                // sebagai satu nilai sebagai argumen ke `SerializeElement`.
                : definition.Settings.InitialCoins,
            // Meneruskan objek anonim yang mengelompokkan initialCoins, initialHappiness, initialSaving, actionsPerTurn, finishDay, minPlayers, maxPlayers
            // sebagai satu nilai sebagai argumen ke `SerializeElement`.
            initialHappiness = definition.Settings.InitialHappiness,
            // Meneruskan objek anonim yang mengelompokkan initialCoins, initialHappiness, initialSaving, actionsPerTurn, finishDay, minPlayers, maxPlayers
            // sebagai satu nilai sebagai argumen ke `SerializeElement`.
            initialSaving = definition.Settings.InitialSaving,
            // Meneruskan objek anonim yang mengelompokkan initialCoins, initialHappiness, initialSaving, actionsPerTurn, finishDay, minPlayers, maxPlayers
            // sebagai satu nilai sebagai argumen ke `SerializeElement`.
            actionsPerTurn = definition.Settings.ActionsPerTurn,
            // Meneruskan objek anonim yang mengelompokkan initialCoins, initialHappiness, initialSaving, actionsPerTurn, finishDay, minPlayers, maxPlayers
            // sebagai satu nilai sebagai argumen ke `SerializeElement`.
            finishDay = definition.Settings.FinishDay,
            // Meneruskan objek anonim yang mengelompokkan initialCoins, initialHappiness, initialSaving, actionsPerTurn, finishDay, minPlayers, maxPlayers
            // sebagai satu nilai sebagai argumen ke `SerializeElement`.
            minPlayers = definition.Settings.MinPlayers,
            // Meneruskan objek anonim yang mengelompokkan initialCoins, initialHappiness, initialSaving, actionsPerTurn, finishDay, minPlayers, maxPlayers
            // sebagai satu nilai sebagai argumen ke `SerializeElement`.
            maxPlayers = definition.Settings.MaxPlayers
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam FromDefinition.
        });
        // Menyiapkan variabel lokal `bahan` untuk nilai bahan dengan memanggil `SerializeElement` dengan `definition.Ingredients`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var bahan = SerializeElement(definition.Ingredients);
        // Menyiapkan variabel lokal `resep` untuk nilai resep dengan memanggil `SerializeElement` dengan `definition.Orders`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var resep = SerializeElement(definition.Orders);
        // Menyiapkan variabel lokal `kebutuhan` untuk nilai kebutuhan dengan memanggil `SerializeElement` dengan `definition.Needs`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var kebutuhan = SerializeElement(definition.Needs);
        // Menyiapkan variabel lokal `targetKebutuhan` untuk nilai target kebutuhan dengan memanggil `SerializeElement` dengan
        // `definition.CollectionMissions`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var targetKebutuhan = SerializeElement(definition.CollectionMissions);
        // Menyiapkan variabel lokal `tujuanFinansial` untuk nilai tujuan finansial dengan memanggil `SerializeElement` dengan `definition.FinancialGoals`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var tujuanFinansial = SerializeElement(definition.FinancialGoals);
        // Menyiapkan variabel lokal `narasi` untuk nilai narasi dengan memanggil `SerializeElement` dengan `definition.Narratives`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var narasi = SerializeElement(definition.Narratives);

        // Menyiapkan variabel lokal `gameConfigValues` untuk nilai game konfigurasi nilai dengan objek baru bertipe `RulesetSectionGameConfig` dengan
        // argumen ( definition.Settings.InitialCoins == 0 ? definition.Settings.StartingCash : definition.Settings.InitialCoins,
        // definition.Settings.InitialHappiness, definition..... Tipe variabel disimpulkan dari ekspresi nilai awal.
        var gameConfigValues = new RulesetSectionGameConfig(
            // Meneruskan hasil pemilihan bersyarat: ketika `definition.Settings.InitialCoins == 0` benar gunakan `definition.Settings.StartingCash`, jika tidak
            // gunakan `definition.Settings.InitialCoins` sebagai argumen ke konstruktor `RulesetSectionGameConfig`.
            definition.Settings.InitialCoins == 0 ? definition.Settings.StartingCash : definition.Settings.InitialCoins,
            // Meneruskan `definition.Settings.InitialHappiness` (nilai awal kebahagiaan) sebagai argumen ke konstruktor `RulesetSectionGameConfig`.
            definition.Settings.InitialHappiness,
            // Meneruskan `definition.Settings.InitialSaving` (nilai awal tabungan) sebagai argumen ke konstruktor `RulesetSectionGameConfig`.
            definition.Settings.InitialSaving,
            // Meneruskan `definition.Settings.ActionsPerTurn` (nilai aksi per giliran) sebagai argumen ke konstruktor `RulesetSectionGameConfig`.
            definition.Settings.ActionsPerTurn,
            // Meneruskan `definition.Settings.FinishDay` (nilai finish hari) sebagai argumen ke konstruktor `RulesetSectionGameConfig`.
            definition.Settings.FinishDay,
            // Meneruskan `definition.Settings.MinPlayers` (nilai minimum pemain) sebagai argumen ke konstruktor `RulesetSectionGameConfig`.
            definition.Settings.MinPlayers,
            // Meneruskan `definition.Settings.MaxPlayers` (nilai maksimum pemain) sebagai argumen ke konstruktor `RulesetSectionGameConfig`.
            definition.Settings.MaxPlayers);

        // Menyiapkan variabel lokal `bahanNames` untuk nilai bahan nama dengan membentuk himpunan nilai unik dari `definition.Ingredients .Select(item =>
        // item.Nama) .Where(item => !string.IsNullOrWhiteSpace(item))` memakai `StringComparer.OrdinalIgnoreCase`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var bahanNames = definition.Ingredients
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => item.Nama) dalam FromDefinition; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => item.Nama)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => !string.IsNullOrWhiteSpace(item)) dalam FromDefinition; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => !string.IsNullOrWhiteSpace(item))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToHashSet(StringComparer.OrdinalIgnoreCase); dalam FromDefinition; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `kebutuhanTypes` untuk nilai kebutuhan types dengan membangun kamus dari `definition.Needs .Where(item =>
        // !string.IsNullOrWhiteSpace(item.Nama) && !string.IsNullOrWhiteSpace(item.Tipe)) .GroupBy(item => item.Nama, StringComparer.OrdinalIgnoreCase)`
        // dengan pemilihan kunci/nilai `group => group.Key`, `group => group.First().Tipe`, `StringComparer.OrdinalIgnoreCase`; kunci harus unik agar
        // konversi berhasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var kebutuhanTypes = definition.Needs
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => !string.IsNullOrWhiteSpace(item.Nama) &&
            // !string.IsNullOrWhiteSpace(item.Tipe)) dalam FromDefinition; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => !string.IsNullOrWhiteSpace(item.Nama) && !string.IsNullOrWhiteSpace(item.Tipe))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(item => item.Nama, StringComparer.OrdinalIgnoreCase) dalam
            // FromDefinition; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(item => item.Nama, StringComparer.OrdinalIgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary(group => group.Key, group => group.First().Tipe,
            // StringComparer.OrdinalIgnoreCase); dalam FromDefinition; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(group => group.Key, group => group.First().Tipe, StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `targetKebutuhanIds` untuk nilai target kebutuhan identitas dengan membentuk himpunan nilai unik dari
        // `definition.CollectionMissions .Select(item => item.Id) .Where(item => !string.IsNullOrWhiteSpace(item))` memakai
        // `StringComparer.OrdinalIgnoreCase`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var targetKebutuhanIds = definition.CollectionMissions
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => item.Id) dalam FromDefinition; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => item.Id)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => !string.IsNullOrWhiteSpace(item)) dalam FromDefinition; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => !string.IsNullOrWhiteSpace(item))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToHashSet(StringComparer.OrdinalIgnoreCase); dalam FromDefinition; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `tujuanFinansialNames` untuk nilai tujuan finansial nama dengan membentuk himpunan nilai unik dari
        // `definition.FinancialGoals .Select(item => item.Nama) .Where(item => !string.IsNullOrWhiteSpace(item))` memakai
        // `StringComparer.OrdinalIgnoreCase`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var tujuanFinansialNames = definition.FinancialGoals
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => item.Nama) dalam FromDefinition; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(item => item.Nama)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => !string.IsNullOrWhiteSpace(item)) dalam FromDefinition; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(item => !string.IsNullOrWhiteSpace(item))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToHashSet(StringComparer.OrdinalIgnoreCase); dalam FromDefinition; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `actions` untuk nilai aksi dengan objek baru bertipe `HashSet<string>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromDefinition.
        {
            // Menggunakan nilai literal `”BahanMasakan”` sebagai bagian ekspresi yang sedang disusun dalam FromDefinition.
            "BahanMasakan",
            // Menggunakan nilai literal `”JualMasakan”` sebagai bagian ekspresi yang sedang disusun dalam FromDefinition.
            "JualMasakan",
            // Menggunakan nilai literal `”Kebutuhan”` sebagai bagian ekspresi yang sedang disusun dalam FromDefinition.
            "Kebutuhan",
            // Menggunakan nilai literal `”KerjaLepas”` sebagai bagian ekspresi yang sedang disusun dalam FromDefinition.
            "KerjaLepas",
            // Menggunakan nilai literal `”TujuanFinansial”` sebagai bagian ekspresi yang sedang disusun dalam FromDefinition.
            "TujuanFinansial",
            // Menggunakan nilai literal `”Menabung”` sebagai bagian ekspresi yang sedang disusun dalam FromDefinition.
            "Menabung",
            // Menggunakan nilai literal `”JumatBerkah”` sebagai bagian ekspresi yang sedang disusun dalam FromDefinition.
            "JumatBerkah"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam FromDefinition.
        };

        // Mengulangi setiap elemen `definition.Actions.Select(item => item.ActionId).Where(item => !string.IsNullOrWhiteSpace(item))`; elemen saat ini
        // disimpan sebagai `actionId` bertipe `var` untuk diproses oleh badan loop dalam FromDefinition.
        foreach (var actionId in definition.Actions.Select(item => item.ActionId).Where(item => !string.IsNullOrWhiteSpace(item)))
        // Membuka scope loop setiap actionId dari `definition.Actions.Select(item => item.ActionId).Where(item => !string.IsNullOrWhiteSpace(item))`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromDefinition.
        {
            // Menjalankan menambahkan `actionId` ke `actions` dalam FromDefinition.
            actions.Add(actionId);
        // Menutup scope loop setiap actionId dari `definition.Actions.Select(item => item.ActionId).Where(item => !string.IsNullOrWhiteSpace(item))`;
        // bagian berikut berada di luar batas blok tersebut dalam FromDefinition.
        }

        // Mengulangi setiap elemen `definition.Narratives .SelectMany(item => item.PrerequisiteAksi) .Select(item => item.Aksi) .Where(item =>
        // !string.IsNullOrWhiteSpace(item))`; elemen saat ini disimpan sebagai `actionId` bertipe `var` untuk diproses oleh badan loop dalam
        // FromDefinition.
        foreach (var actionId in definition.Narratives
                     // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .SelectMany(item => item.PrerequisiteAksi) dalam FromDefinition; token pada
                     // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                     .SelectMany(item => item.PrerequisiteAksi)
                     // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => item.Aksi) dalam FromDefinition; token pada baris ini
                     // menyambungkan bagian kode sebelum dan sesudahnya.
                     .Select(item => item.Aksi)
                     // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(item => !string.IsNullOrWhiteSpace(item))) dalam FromDefinition; token
                     // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                     .Where(item => !string.IsNullOrWhiteSpace(item)))
        // Membuka scope loop setiap actionId dari `definition.Narratives .SelectMany(item => item.PrerequisiteAksi) .Select(item => item.Aksi) .Where(item
        // => !string.IsNullOrWhiteSpace(item))`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromDefinition.
        {
            // Menjalankan menambahkan `actionId` ke `actions` dalam FromDefinition.
            actions.Add(actionId);
        // Menutup scope loop setiap actionId dari `definition.Narratives .SelectMany(item => item.PrerequisiteAksi) .Select(item => item.Aksi) .Where(item
        // => !string.IsNullOrWhiteSpace(item))`; bagian berikut berada di luar batas blok tersebut dalam FromDefinition.
        }

        // Mengembalikan objek baru bertipe `RulesetSectionCatalog` dengan argumen ( gameConfig, bahan, resep, kebutuhan, targetKebutuhan, tujuanFinansial,
        // narasi, gameConfigValues, bahanNames, kebutuhanTypes, targetKebutuhanIds, tujuanFinansi... kepada pemanggil dalam FromDefinition; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return new RulesetSectionCatalog(
            // Meneruskan `gameConfig` (nilai game konfigurasi) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            gameConfig,
            // Meneruskan `bahan` (nilai bahan) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            bahan,
            // Meneruskan `resep` (nilai resep) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            resep,
            // Meneruskan `kebutuhan` (nilai kebutuhan) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            kebutuhan,
            // Meneruskan `targetKebutuhan` (nilai target kebutuhan) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            targetKebutuhan,
            // Meneruskan `tujuanFinansial` (nilai tujuan finansial) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            tujuanFinansial,
            // Meneruskan `narasi` (nilai narasi) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            narasi,
            // Meneruskan `gameConfigValues` (nilai game konfigurasi nilai) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            gameConfigValues,
            // Meneruskan `bahanNames` (nilai bahan nama) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            bahanNames,
            // Meneruskan `kebutuhanTypes` (nilai kebutuhan types) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            kebutuhanTypes,
            // Meneruskan `targetKebutuhanIds` (nilai target kebutuhan identitas) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            targetKebutuhanIds,
            // Meneruskan `tujuanFinansialNames` (nilai tujuan finansial nama) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            tujuanFinansialNames,
            // Meneruskan `actions` (nilai aksi) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            actions);
    // Menutup scope metode FromDefinition; bagian berikut berada di luar batas blok tersebut dalam FromDefinition.
    }

    // Mendefinisikan metode `FromConfig` dengan hasil bertipe `RulesetSectionCatalog`; operasi ini menangani dari konfigurasi. Masukan: Parameter
    // `configJson` bertipe `string` membawa nilai konfigurasi JSON.
    public static RulesetSectionCatalog FromConfig(string configJson)
    // Membuka scope metode FromConfig; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromConfig.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `configJson`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(configJson);
        // Menyiapkan variabel lokal `componentCatalog` untuk nilai komponen catalog dengan memanggil `document.RootElement.GetProperty` dengan
        // `”component_catalog”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var componentCatalog = document.RootElement.GetProperty("component_catalog");
        // Menyiapkan variabel lokal `gameConfig` untuk nilai game konfigurasi dengan membuat salinan `componentCatalog.GetProperty(”gameConfig”)` agar
        // hasil dapat digunakan terpisah dari objek sumber. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var gameConfig = componentCatalog.GetProperty("gameConfig").Clone();
        // Menyiapkan variabel lokal `gameConfigValues` untuk nilai game konfigurasi nilai dengan objek baru bertipe `RulesetSectionGameConfig` dengan
        // argumen ( ReadInt(gameConfig, ”initialCoins”, 20), ReadInt(gameConfig, ”initialHappiness”, 0), ReadInt(gameConfig, ”initialSaving”, 0),
        // ReadInt(gameConfig, ”actionsPerT.... Tipe variabel disimpulkan dari ekspresi nilai awal.
        var gameConfigValues = new RulesetSectionGameConfig(
            // Meneruskan memanggil `ReadInt` dengan `gameConfig`, `”initialCoins”`, `20` sebagai argumen ke konstruktor `RulesetSectionGameConfig`; Meneruskan
            // `gameConfig` (nilai game konfigurasi) sebagai argumen ke `ReadInt`; Meneruskan nilai literal `”initialCoins”` sebagai argumen ke `ReadInt`;
            // Meneruskan nilai literal `20` sebagai argumen ke `ReadInt`.
            ReadInt(gameConfig, "initialCoins", 20),
            // Meneruskan memanggil `ReadInt` dengan `gameConfig`, `”initialHappiness”`, `0` sebagai argumen ke konstruktor `RulesetSectionGameConfig`;
            // Meneruskan `gameConfig` (nilai game konfigurasi) sebagai argumen ke `ReadInt`; Meneruskan nilai literal `”initialHappiness”` sebagai argumen ke
            // `ReadInt`; Meneruskan nilai literal `0` sebagai argumen ke `ReadInt`.
            ReadInt(gameConfig, "initialHappiness", 0),
            // Meneruskan memanggil `ReadInt` dengan `gameConfig`, `”initialSaving”`, `0` sebagai argumen ke konstruktor `RulesetSectionGameConfig`; Meneruskan
            // `gameConfig` (nilai game konfigurasi) sebagai argumen ke `ReadInt`; Meneruskan nilai literal `”initialSaving”` sebagai argumen ke `ReadInt`;
            // Meneruskan nilai literal `0` sebagai argumen ke `ReadInt`.
            ReadInt(gameConfig, "initialSaving", 0),
            // Meneruskan memanggil `ReadInt` dengan `gameConfig`, `”actionsPerTurn”`, `2` sebagai argumen ke konstruktor `RulesetSectionGameConfig`; Meneruskan
            // `gameConfig` (nilai game konfigurasi) sebagai argumen ke `ReadInt`; Meneruskan nilai literal `”actionsPerTurn”` sebagai argumen ke `ReadInt`;
            // Meneruskan nilai literal `2` sebagai argumen ke `ReadInt`.
            ReadInt(gameConfig, "actionsPerTurn", 2),
            // Meneruskan memanggil `ReadInt` dengan `gameConfig`, `”finishDay”`, `25` sebagai argumen ke konstruktor `RulesetSectionGameConfig`; Meneruskan
            // `gameConfig` (nilai game konfigurasi) sebagai argumen ke `ReadInt`; Meneruskan nilai literal `”finishDay”` sebagai argumen ke `ReadInt`;
            // Meneruskan nilai literal `25` sebagai argumen ke `ReadInt`.
            ReadInt(gameConfig, "finishDay", 25),
            // Meneruskan memanggil `ReadInt` dengan `gameConfig`, `”minPlayers”`, `2` sebagai argumen ke konstruktor `RulesetSectionGameConfig`; Meneruskan
            // `gameConfig` (nilai game konfigurasi) sebagai argumen ke `ReadInt`; Meneruskan nilai literal `”minPlayers”` sebagai argumen ke `ReadInt`;
            // Meneruskan nilai literal `2` sebagai argumen ke `ReadInt`.
            ReadInt(gameConfig, "minPlayers", 2),
            // Meneruskan memanggil `ReadInt` dengan `gameConfig`, `”maxPlayers”`, `4` sebagai argumen ke konstruktor `RulesetSectionGameConfig`; Meneruskan
            // `gameConfig` (nilai game konfigurasi) sebagai argumen ke `ReadInt`; Meneruskan nilai literal `”maxPlayers”` sebagai argumen ke `ReadInt`;
            // Meneruskan nilai literal `4` sebagai argumen ke `ReadInt`.
            ReadInt(gameConfig, "maxPlayers", 4));
        // Menyiapkan variabel lokal `bahan` untuk nilai bahan dengan membuat salinan `componentCatalog.GetProperty(”bahan”)` agar hasil dapat digunakan
        // terpisah dari objek sumber. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var bahan = componentCatalog.GetProperty("bahan").Clone();
        // Menyiapkan variabel lokal `resep` untuk nilai resep dengan membuat salinan `componentCatalog.GetProperty(”resep”)` agar hasil dapat digunakan
        // terpisah dari objek sumber. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var resep = componentCatalog.GetProperty("resep").Clone();
        // Menyiapkan variabel lokal `kebutuhan` untuk nilai kebutuhan dengan membuat salinan `componentCatalog.GetProperty(”kebutuhan”)` agar hasil dapat
        // digunakan terpisah dari objek sumber. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var kebutuhan = componentCatalog.GetProperty("kebutuhan").Clone();
        // Menyiapkan variabel lokal `targetKebutuhan` untuk nilai target kebutuhan dengan membuat salinan `componentCatalog.GetProperty(”targetKebutuhan”)`
        // agar hasil dapat digunakan terpisah dari objek sumber. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var targetKebutuhan = componentCatalog.GetProperty("targetKebutuhan").Clone();
        // Menyiapkan variabel lokal `tujuanFinansial` untuk nilai tujuan finansial dengan membuat salinan `componentCatalog.GetProperty(”tujuanFinansial”)`
        // agar hasil dapat digunakan terpisah dari objek sumber. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var tujuanFinansial = componentCatalog.GetProperty("tujuanFinansial").Clone();
        // Menyiapkan variabel lokal `narasi` untuk nilai narasi dengan membuat salinan `componentCatalog.GetProperty(”narasi”)` agar hasil dapat digunakan
        // terpisah dari objek sumber. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var narasi = componentCatalog.GetProperty("narasi").Clone();

        // Menyiapkan variabel lokal `bahanNames` untuk nilai bahan nama dengan memanggil `ReadStringSet` dengan `bahan`, `”nama”`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var bahanNames = ReadStringSet(bahan, "nama");
        // Menyiapkan variabel lokal `kebutuhanTypes` untuk nilai kebutuhan types dengan objek baru bertipe `Dictionary<string, string>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var kebutuhanTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `kebutuhan.EnumerateArray()`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
        // FromConfig.
        foreach (var item in kebutuhan.EnumerateArray())
        // Membuka scope loop setiap item dari `kebutuhan.EnumerateArray()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromConfig.
        {
            // Menyiapkan variabel lokal `nama` untuk nilai nama dengan membaca nilai string dari `item.GetProperty(”nama”)` sesuai tipe JSON atau sumber data
            // yang digunakan. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var nama = item.GetProperty("nama").GetString();
            // Menyiapkan variabel lokal `tipe` untuk nilai tipe dengan membaca nilai string dari `item.GetProperty(”tipe”)` sesuai tipe JSON atau sumber data
            // yang digunakan. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var tipe = item.GetProperty("tipe").GetString();
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!string.IsNullOrWhiteSpace(nama)` dan `!string.IsNullOrWhiteSpace(tipe)`; sisi
            // kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam FromConfig.
            if (!string.IsNullOrWhiteSpace(nama) && !string.IsNullOrWhiteSpace(tipe))
            // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(nama) && !string.IsNullOrWhiteSpace(tipe)`; pernyataan/deklarasi berikut berada
            // di dalam batas blok ini dalam FromConfig.
            {
                // Memperbarui `kebutuhanTypes[nama]` menggunakan `tipe` (nilai tipe) dalam FromConfig.
                kebutuhanTypes[nama] = tipe;
            // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(nama) && !string.IsNullOrWhiteSpace(tipe)`; bagian berikut berada di luar batas
            // blok tersebut dalam FromConfig.
            }
        // Menutup scope loop setiap item dari `kebutuhan.EnumerateArray()`; bagian berikut berada di luar batas blok tersebut dalam FromConfig.
        }

        // Menyiapkan variabel lokal `targetKebutuhanIds` untuk nilai target kebutuhan identitas dengan memanggil `ReadStringSet` dengan `targetKebutuhan`,
        // `”id”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var targetKebutuhanIds = ReadStringSet(targetKebutuhan, "id");
        // Menyiapkan variabel lokal `tujuanFinansialNames` untuk nilai tujuan finansial nama dengan memanggil `ReadStringSet` dengan `tujuanFinansial`,
        // `”nama”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var tujuanFinansialNames = ReadStringSet(tujuanFinansial, "nama");
        // Menyiapkan variabel lokal `actions` untuk nilai aksi dengan objek baru bertipe `HashSet<string>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromConfig.
        {
            // Menggunakan nilai literal `”BahanMasakan”` sebagai bagian ekspresi yang sedang disusun dalam FromConfig.
            "BahanMasakan",
            // Menggunakan nilai literal `”JualMasakan”` sebagai bagian ekspresi yang sedang disusun dalam FromConfig.
            "JualMasakan",
            // Menggunakan nilai literal `”Kebutuhan”` sebagai bagian ekspresi yang sedang disusun dalam FromConfig.
            "Kebutuhan",
            // Menggunakan nilai literal `”KerjaLepas”` sebagai bagian ekspresi yang sedang disusun dalam FromConfig.
            "KerjaLepas",
            // Menggunakan nilai literal `”TujuanFinansial”` sebagai bagian ekspresi yang sedang disusun dalam FromConfig.
            "TujuanFinansial",
            // Menggunakan nilai literal `”Menabung”` sebagai bagian ekspresi yang sedang disusun dalam FromConfig.
            "Menabung",
            // Menggunakan nilai literal `”JumatBerkah”` sebagai bagian ekspresi yang sedang disusun dalam FromConfig.
            "JumatBerkah"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam FromConfig.
        };

        // Mengulangi setiap elemen `narasi.EnumerateArray()`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
        // FromConfig.
        foreach (var item in narasi.EnumerateArray())
        // Membuka scope loop setiap item dari `narasi.EnumerateArray()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromConfig.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!item.TryGetProperty(”prerequisiteAksi”, out var prerequisites)` dan
            // `prerequisites.ValueKind != JsonValueKind.Array`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam FromConfig.
            if (!item.TryGetProperty("prerequisiteAksi", out var prerequisites) ||
                // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `prerequisites.ValueKind` dan `JsonValueKind.Array` dalam FromConfig.
                prerequisites.ValueKind != JsonValueKind.Array)
            // Membuka scope cabang if untuk kondisi `!item.TryGetProperty(”prerequisiteAksi”, out var prerequisites) || prerequisites.ValueKind !=
            // JsonValueKind.Array`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromConfig.
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam FromConfig.
                continue;
            // Menutup scope cabang if untuk kondisi `!item.TryGetProperty(”prerequisiteAksi”, out var prerequisites) || prerequisites.ValueKind !=
            // JsonValueKind.Array`; bagian berikut berada di luar batas blok tersebut dalam FromConfig.
            }

            // Mengulangi setiap elemen `prerequisites.EnumerateArray()`; elemen saat ini disimpan sebagai `prerequisite` bertipe `var` untuk diproses oleh
            // badan loop dalam FromConfig.
            foreach (var prerequisite in prerequisites.EnumerateArray())
            // Membuka scope loop setiap prerequisite dari `prerequisites.EnumerateArray()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // FromConfig.
            {
                // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `prerequisite.TryGetProperty(”aksi”, out var aksiProp)` dan `aksiProp.ValueKind
                // == JsonValueKind.String`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                // FromConfig.
                if (prerequisite.TryGetProperty("aksi", out var aksiProp) && aksiProp.ValueKind == JsonValueKind.String)
                // Membuka scope cabang if untuk kondisi `prerequisite.TryGetProperty(”aksi”, out var aksiProp) && aksiProp.ValueKind == JsonValueKind.String`;
                // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromConfig.
                {
                    // Menyiapkan variabel lokal `aksi` untuk nilai aksi dengan membaca nilai string dari `aksiProp` sesuai tipe JSON atau sumber data yang digunakan.
                    // Tipe variabel disimpulkan dari ekspresi nilai awal.
                    var aksi = aksiProp.GetString();
                    // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(aksi)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam FromConfig.
                    if (!string.IsNullOrWhiteSpace(aksi))
                    // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(aksi)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                    // FromConfig.
                    {
                        // Menjalankan menambahkan `aksi` ke `actions` dalam FromConfig.
                        actions.Add(aksi);
                    // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(aksi)`; bagian berikut berada di luar batas blok tersebut dalam FromConfig.
                    }
                // Menutup scope cabang if untuk kondisi `prerequisite.TryGetProperty(”aksi”, out var aksiProp) && aksiProp.ValueKind == JsonValueKind.String`;
                // bagian berikut berada di luar batas blok tersebut dalam FromConfig.
                }
            // Menutup scope loop setiap prerequisite dari `prerequisites.EnumerateArray()`; bagian berikut berada di luar batas blok tersebut dalam FromConfig.
            }
        // Menutup scope loop setiap item dari `narasi.EnumerateArray()`; bagian berikut berada di luar batas blok tersebut dalam FromConfig.
        }

        // Mengembalikan objek baru bertipe `RulesetSectionCatalog` dengan argumen ( gameConfig, bahan, resep, kebutuhan, targetKebutuhan, tujuanFinansial,
        // narasi, gameConfigValues, bahanNames, kebutuhanTypes, targetKebutuhanIds, tujuanFinansi... kepada pemanggil dalam FromConfig; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return new RulesetSectionCatalog(
            // Meneruskan `gameConfig` (nilai game konfigurasi) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            gameConfig,
            // Meneruskan `bahan` (nilai bahan) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            bahan,
            // Meneruskan `resep` (nilai resep) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            resep,
            // Meneruskan `kebutuhan` (nilai kebutuhan) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            kebutuhan,
            // Meneruskan `targetKebutuhan` (nilai target kebutuhan) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            targetKebutuhan,
            // Meneruskan `tujuanFinansial` (nilai tujuan finansial) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            tujuanFinansial,
            // Meneruskan `narasi` (nilai narasi) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            narasi,
            // Meneruskan `gameConfigValues` (nilai game konfigurasi nilai) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            gameConfigValues,
            // Meneruskan `bahanNames` (nilai bahan nama) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            bahanNames,
            // Meneruskan `kebutuhanTypes` (nilai kebutuhan types) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            kebutuhanTypes,
            // Meneruskan `targetKebutuhanIds` (nilai target kebutuhan identitas) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            targetKebutuhanIds,
            // Meneruskan `tujuanFinansialNames` (nilai tujuan finansial nama) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            tujuanFinansialNames,
            // Meneruskan `actions` (nilai aksi) sebagai argumen ke konstruktor `RulesetSectionCatalog`.
            actions);
    // Menutup scope metode FromConfig; bagian berikut berada di luar batas blok tersebut dalam FromConfig.
    }

    // Mendefinisikan metode `SerializeElement` dengan hasil bertipe `JsonElement`; operasi ini menangani serialize element. Masukan: Parameter `value`
    // bertipe `T` membawa nilai nilai.
    private static JsonElement SerializeElement<T>(T value)
    // Membuka scope metode SerializeElement; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SerializeElement.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `JsonSerializer.Serialize(value)`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(value));
        // Mengembalikan membuat salinan `document.RootElement` agar hasil dapat digunakan terpisah dari objek sumber kepada pemanggil dalam
        // SerializeElement; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return document.RootElement.Clone();
    // Menutup scope metode SerializeElement; bagian berikut berada di luar batas blok tersebut dalam SerializeElement.
    }

    // Mendefinisikan metode `ReadInt` dengan hasil bertipe `int`; operasi ini menangani read int. Masukan: Parameter `root` bertipe `JsonElement`
    // membawa nilai root; Parameter `propertyName` bertipe `string` membawa nilai property nama; Parameter `defaultValue` bertipe `int` membawa nilai
    // bawaan nilai.
    private static int ReadInt(JsonElement root, string propertyName, int defaultValue)
    // Membuka scope metode ReadInt; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadInt.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `root.TryGetProperty(propertyName, out var property) && property.ValueKind ==
        // JsonValueKind.Number && property.TryGetInt32(out var value)` benar gunakan `value`, jika tidak gunakan `defaultValue` kepada pemanggil dalam
        // ReadInt; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return root.TryGetProperty(propertyName, out var property) &&
               // Melanjutkan ekspresi dengan perbandingan kesamaan antara `property.ValueKind` dan `JsonValueKind.Number` dalam ReadInt.
               property.ValueKind == JsonValueKind.Number &&
               // Melanjutkan pengolahan dengan memanggil `property.TryGetInt32` dengan `var value` dalam ReadInt.
               property.TryGetInt32(out var value)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: value dalam ReadInt.
            ? value
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: defaultValue; dalam ReadInt.
            : defaultValue;
    // Menutup scope metode ReadInt; bagian berikut berada di luar batas blok tersebut dalam ReadInt.
    }

    // Mendefinisikan metode `ReadStringSet` dengan hasil bertipe `HashSet<string>`; operasi ini menangani read string set. Masukan: Parameter `array`
    // bertipe `JsonElement` membawa nilai array; Parameter `propertyName` bertipe `string` membawa nilai property nama.
    private static HashSet<string> ReadStringSet(JsonElement array, string propertyName)
    // Membuka scope metode ReadStringSet; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadStringSet.
    {
        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan objek baru bertipe
        // `HashSet<string>` dengan argumen (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `array.EnumerateArray()`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
        // ReadStringSet.
        foreach (var item in array.EnumerateArray())
        // Membuka scope loop setiap item dari `array.EnumerateArray()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadStringSet.
        {
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `item.TryGetProperty(propertyName, out var property)` dan `property.ValueKind ==
            // JsonValueKind.String`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ReadStringSet.
            if (item.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String)
            // Membuka scope cabang if untuk kondisi `item.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadStringSet.
            {
                // Menyiapkan variabel lokal `value` untuk nilai nilai dengan membaca nilai string dari `property` sesuai tipe JSON atau sumber data yang digunakan.
                // Tipe variabel disimpulkan dari ekspresi nilai awal.
                var value = property.GetString();
                // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(value)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ReadStringSet.
                if (!string.IsNullOrWhiteSpace(value))
                // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(value)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ReadStringSet.
                {
                    // Menjalankan menambahkan `value` ke `result` dalam ReadStringSet.
                    result.Add(value);
                // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(value)`; bagian berikut berada di luar batas blok tersebut dalam ReadStringSet.
                }
            // Menutup scope cabang if untuk kondisi `item.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String`; bagian
            // berikut berada di luar batas blok tersebut dalam ReadStringSet.
            }
        // Menutup scope loop setiap item dari `array.EnumerateArray()`; bagian berikut berada di luar batas blok tersebut dalam ReadStringSet.
        }

        // Mengembalikan `result` (nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya) kepada pemanggil dalam ReadStringSet; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return result;
    // Menutup scope metode ReadStringSet; bagian berikut berada di luar batas blok tersebut dalam ReadStringSet.
    }
// Menutup scope tipe RulesetSectionCatalog; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RulesetSectionGameConfig`; sealed mencegah tipe ini diturunkan lagi.
public sealed record RulesetSectionGameConfig(
    // Parameter `InitialCoins` bertipe `int` membawa nilai awal coins.
    int InitialCoins,
    // Parameter `InitialHappiness` bertipe `int` membawa nilai awal kebahagiaan.
    int InitialHappiness,
    // Parameter `InitialSaving` bertipe `int` membawa nilai awal tabungan.
    int InitialSaving,
    // Parameter `ActionsPerTurn` bertipe `int` membawa nilai aksi per giliran.
    int ActionsPerTurn,
    // Parameter `FinishDay` bertipe `int` membawa nilai finish hari.
    int FinishDay,
    // Parameter `MinPlayers` bertipe `int` membawa nilai minimum pemain.
    int MinPlayers,
    // Parameter `MaxPlayers` bertipe `int` membawa nilai maksimum pemain.
    int MaxPlayers);
