// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui SessionTimelineMapper.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `System.Globalization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Globalization;
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Kelas statis untuk memetakan data event transaksi gameplay menjadi
/// deskripsi timeline yang mudah dibaca pengguna dalam dua bahasa (Indonesia/Inggris).
/// </summary>
// Mendefinisikan tipe class `SessionTimelineMapper`.
public static class SessionTimelineMapper
// Membuka scope tipe SessionTimelineMapper; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Mengonversi daftar event mentah menjadi item timeline terurut berdasarkan
    /// waktu dan nomor urut, dengan label dan deskripsi bilingual.
    /// </summary>
    /// <param name="events">Daftar event dari API, boleh null.</param>
    /// <param name="language">Kode bahasa ("id" atau "en"); default bahasa Indonesia.</param>
    /// <returns>Daftar item timeline yang siap ditampilkan di UI.</returns>
    // Mendefinisikan metode `MapTimeline` dengan hasil bertipe `List<SessionTimelineEventViewModel>`. Mengonversi daftar event mentah menjadi item
    // timeline terurut berdasarkan waktu dan nomor urut, dengan label dan deskripsi bilingual. Masukan: Parameter `events` bertipe
    // `List<EventRequest>?` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan; nilai null diizinkan ketika data
    // opsional belum tersedia; Parameter `language` bertipe `string?` membawa nilai language; nilai null diizinkan ketika data opsional belum tersedia;
    // bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
    public static List<SessionTimelineEventViewModel> MapTimeline(List<EventRequest>? events, string? language = null)
    // Membuka scope metode MapTimeline; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MapTimeline.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `events is null` dan `events.Count == 0`; sisi kanan diperiksa hanya
        // jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam MapTimeline.
        if (events is null || events.Count == 0)
        // Membuka scope cabang if untuk kondisi `events is null || events.Count == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MapTimeline.
        {
            // Mengembalikan objek baru bertipe `List<SessionTimelineEventViewModel>` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam
            // MapTimeline; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return new List<SessionTimelineEventViewModel>();
        // Menutup scope cabang if untuk kondisi `events is null || events.Count == 0`; bagian berikut berada di luar batas blok tersebut dalam MapTimeline.
        }

        // Menyiapkan variabel lokal `normalizedLanguage` untuk nilai normalized language dengan memanggil `UiText.NormalizeLanguage` dengan `language`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var normalizedLanguage = UiText.NormalizeLanguage(language);

        // Mengembalikan mematerialisasi urutan `events .OrderBy(item => item.Timestamp) .ThenBy(item => item.SequenceNumber) .Select(item => { var
        // actionSlot = Math.Max(0, item.ActionSlot); var actionSlotRole = ResolveActio...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam
        // memori kepada pemanggil dalam MapTimeline; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return events
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(item => item.Timestamp) dalam MapTimeline; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderBy(item => item.Timestamp)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(item => item.SequenceNumber) dalam MapTimeline; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .ThenBy(item => item.SequenceNumber)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(item => dalam MapTimeline; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .Select(item =>
            // Membuka scope fungsi lambda yang dipasok ke `events .OrderBy(item => item.Timestamp) .ThenBy(item => item.SequenceNumber) .Select`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MapTimeline.
            {
                // Menyiapkan variabel lokal `actionSlot` untuk nilai aksi slot dengan menentukan nilai terbesar dari `0`, `item.ActionSlot`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var actionSlot = Math.Max(0, item.ActionSlot);
                // Menyiapkan variabel lokal `actionSlotRole` untuk nilai aksi slot role dengan memanggil `ResolveActionSlotRole` dengan `item.ActionType`,
                // `item.Payload`. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var actionSlotRole = ResolveActionSlotRole(item.ActionType, item.Payload);

                // Mengembalikan objek baru bertipe `SessionTimelineEventViewModel` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam MapTimeline;
                // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return new SessionTimelineEventViewModel
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam MapTimeline.
                {
                    // Memperbarui `Timestamp` menggunakan `item.Timestamp` (waktu kejadian yang menjaga urutan kronologis data) dalam MapTimeline.
                    Timestamp = item.Timestamp,
                    // Memperbarui `SequenceNumber` menggunakan `item.SequenceNumber` (nomor urut event yang menentukan urutan pemrosesan riwayat permainan) dalam
                    // MapTimeline.
                    SequenceNumber = item.SequenceNumber,
                    // Memperbarui `DayIndex` menggunakan memanggil `ResolveJourneyDayIndex` dengan `item` dalam MapTimeline.
                    DayIndex = ResolveJourneyDayIndex(item),
                    // Memperbarui `Weekday` menggunakan memanggil `ResolveWeekdayLabel` dengan `item.Weekday`, `normalizedLanguage` dalam MapTimeline.
                    Weekday = ResolveWeekdayLabel(item.Weekday, normalizedLanguage),
                    // Memperbarui `ActionSlot` menggunakan `actionSlot` (nilai aksi slot) dalam MapTimeline.
                    ActionSlot = actionSlot,
                    // Memperbarui `ActorType` menggunakan `item.ActorType` (nilai actor jenis) dalam MapTimeline.
                    ActorType = item.ActorType,
                    // Memperbarui `PlayerId` menggunakan `item.UserId` (identitas akun pengguna yang datanya sedang diproses) dalam MapTimeline.
                    PlayerId = item.UserId,
                    // Memperbarui `ActionType` menggunakan `item.ActionType` (nilai aksi jenis) dalam MapTimeline.
                    ActionType = item.ActionType,
                    // Memperbarui `ActionSlotRole` menggunakan `actionSlotRole` (nilai aksi slot role) dalam MapTimeline.
                    ActionSlotRole = actionSlotRole,
                    // Memperbarui `ActionSlotLabel` menggunakan memanggil `BuildActionSlotLabel` dengan `actionSlotRole`, `actionSlot`, `normalizedLanguage` dalam
                    // MapTimeline.
                    ActionSlotLabel = BuildActionSlotLabel(actionSlotRole, actionSlot, normalizedLanguage),
                    // Memperbarui `FlowLabel` menggunakan memanggil `ResolveFlowLabel` dengan `item.ActionType`, `normalizedLanguage` dalam MapTimeline.
                    FlowLabel = ResolveFlowLabel(item.ActionType, normalizedLanguage),
                    // Memperbarui `FlowDescription` menggunakan memanggil `BuildFlowDescription` dengan `item.ActionType`, `item.Payload`, `normalizedLanguage` dalam
                    // MapTimeline.
                    FlowDescription = BuildFlowDescription(item.ActionType, item.Payload, normalizedLanguage)
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam MapTimeline.
                };
            // Menutup scope fungsi lambda yang dipasok ke `events .OrderBy(item => item.Timestamp) .ThenBy(item => item.SequenceNumber) .Select`; bagian
            // berikut berada di luar batas blok tersebut dalam MapTimeline.
            })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam MapTimeline; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .ToList();
    // Menutup scope metode MapTimeline; bagian berikut berada di luar batas blok tersebut dalam MapTimeline.
    }

    // Mendefinisikan metode `ResolveJourneyDayIndex` dengan hasil bertipe `int`; operasi ini menangani resolve journey hari index. Masukan: Parameter
    // `item` bertipe `EventRequest` membawa nilai elemen.
    private static int ResolveJourneyDayIndex(EventRequest item)
    // Membuka scope metode ResolveJourneyDayIndex; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveJourneyDayIndex.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `string.Equals(item.ActionType, ”MulaiSesi”,
        // StringComparison.OrdinalIgnoreCase)` dan `item.ActionType.StartsWith(”Setup”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya
        // jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveJourneyDayIndex.
        if (string.Equals(item.ActionType, "MulaiSesi", StringComparison.OrdinalIgnoreCase) ||
            // Melanjutkan pengolahan dengan memanggil `item.ActionType.StartsWith` dengan `”Setup”`, `StringComparison.OrdinalIgnoreCase` dalam
            // ResolveJourneyDayIndex.
            item.ActionType.StartsWith("Setup", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(item.ActionType, ”MulaiSesi”, StringComparison.OrdinalIgnoreCase) ||
        // item.ActionType.StartsWith(”Setup”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveJourneyDayIndex.
        {
            // Mengembalikan nilai literal `0` kepada pemanggil dalam ResolveJourneyDayIndex; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return 0;
        // Menutup scope cabang if untuk kondisi `string.Equals(item.ActionType, ”MulaiSesi”, StringComparison.OrdinalIgnoreCase) ||
        // item.ActionType.StartsWith(”Setup”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveJourneyDayIndex.
        }

        // Mengembalikan hasil pemilihan bersyarat: ketika `item.Payload.ValueKind == JsonValueKind.Object && item.Payload.TryGetProperty(”setup”, out var
        // setup) && setup.ValueKind == JsonValueKind.String && string.Equals(setup.GetStri...` benar gunakan `0`, jika tidak gunakan `item.DayIndex` kepada
        // pemanggil dalam ResolveJourneyDayIndex; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return item.Payload.ValueKind == JsonValueKind.Object &&
               // Melanjutkan pengolahan dengan mencari properti JSON `”setup”`, `var setup` pada `item.Payload` tanpa menganggap propertinya selalu tersedia dalam
               // ResolveJourneyDayIndex.
               item.Payload.TryGetProperty("setup", out var setup) &&
               // Melanjutkan ekspresi dengan perbandingan kesamaan antara `setup.ValueKind` dan `JsonValueKind.String` dalam ResolveJourneyDayIndex.
               setup.ValueKind == JsonValueKind.String &&
               // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `setup.GetString()`, `”INITIAL”`, `StringComparison.OrdinalIgnoreCase`;
               // aturan perbandingan mengikuti overload dan comparer yang diberikan dalam ResolveJourneyDayIndex.
               string.Equals(setup.GetString(), "INITIAL", StringComparison.OrdinalIgnoreCase)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: 0 dalam ResolveJourneyDayIndex.
            ? 0
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: item.DayIndex; dalam ResolveJourneyDayIndex.
            : item.DayIndex;
    // Menutup scope metode ResolveJourneyDayIndex; bagian berikut berada di luar batas blok tersebut dalam ResolveJourneyDayIndex.
    }

    /// <summary>
    /// Menetapkan nama tampilan pemain pada setiap item timeline berdasarkan
    /// pemetaan PlayerId ke display name yang diberikan.
    /// </summary>
    /// <param name="timeline">Koleksi item timeline yang akan diperbarui.</param>
    /// <param name="playerDisplayNames">Pemetaan GUID pemain ke nama tampilan.</param>
    // Mendefinisikan metode `ApplyPlayerDisplayNames` dengan hasil bertipe `void`. Menetapkan nama tampilan pemain pada setiap item timeline
    // berdasarkan pemetaan PlayerId ke display name yang diberikan. Masukan: Parameter `timeline` bertipe `IEnumerable<SessionTimelineEventViewModel>`
    // membawa nilai timeline; Parameter `playerDisplayNames` bertipe `IReadOnlyDictionary<Guid, string>` membawa nilai pemain display nama.
    public static void ApplyPlayerDisplayNames(
        // Parameter `timeline` bertipe `IEnumerable<SessionTimelineEventViewModel>` membawa nilai timeline.
        IEnumerable<SessionTimelineEventViewModel> timeline,
        // Parameter `playerDisplayNames` bertipe `IReadOnlyDictionary<Guid, string>` membawa nilai pemain display nama.
        IReadOnlyDictionary<Guid, string> playerDisplayNames)
    // Membuka scope metode ApplyPlayerDisplayNames; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyPlayerDisplayNames.
    {
        // Mengulangi setiap elemen `timeline.Where(item => item.PlayerId.HasValue)`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses
        // oleh badan loop dalam ApplyPlayerDisplayNames.
        foreach (var item in timeline.Where(item => item.PlayerId.HasValue))
        // Membuka scope loop setiap item dari `timeline.Where(item => item.PlayerId.HasValue)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ApplyPlayerDisplayNames.
        {
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `item.PlayerId.HasValue && playerDisplayNames.TryGetValue(item.PlayerId.Value,
            // out var displayName)` dan `!string.IsNullOrWhiteSpace(displayName)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan
            // ketika kondisi ini bernilai benar dalam ApplyPlayerDisplayNames.
            if (item.PlayerId.HasValue &&
                // Melanjutkan pengolahan dengan mencari kunci `item.PlayerId.Value` pada `playerDisplayNames`; hasil boolean menandakan kunci ditemukan dan argumen
                // out menerima nilainya dalam ApplyPlayerDisplayNames.
                playerDisplayNames.TryGetValue(item.PlayerId.Value, out var displayName) &&
                // Menggunakan kebalikan kondisi `string.IsNullOrWhiteSpace(displayName)` sebagai bagian ekspresi yang sedang disusun dalam ApplyPlayerDisplayNames.
                !string.IsNullOrWhiteSpace(displayName))
            // Membuka scope cabang if untuk kondisi `item.PlayerId.HasValue && playerDisplayNames.TryGetValue(item.PlayerId.Value, out var displayName) &&
            // !string.IsNullOrWhiteSpace(displayName)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyPlayerDisplayNames.
            {
                // Memperbarui `item.PlayerDisplayName` menggunakan `displayName` (nilai display nama) dalam ApplyPlayerDisplayNames.
                item.PlayerDisplayName = displayName;
            // Menutup scope cabang if untuk kondisi `item.PlayerId.HasValue && playerDisplayNames.TryGetValue(item.PlayerId.Value, out var displayName) &&
            // !string.IsNullOrWhiteSpace(displayName)`; bagian berikut berada di luar batas blok tersebut dalam ApplyPlayerDisplayNames.
            }
        // Menutup scope loop setiap item dari `timeline.Where(item => item.PlayerId.HasValue)`; bagian berikut berada di luar batas blok tersebut dalam
        // ApplyPlayerDisplayNames.
        }
    // Menutup scope metode ApplyPlayerDisplayNames; bagian berikut berada di luar batas blok tersebut dalam ApplyPlayerDisplayNames.
    }

    /// <summary>
    /// Menentukan label kategori alur (flow label) berdasarkan prefiks actionType,
    /// misalnya "Setup", "Event Harian", "Risiko", dll.
    /// </summary>
    /// <param name="actionType">Tipe aksi event dari payload API.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Label kategori alur dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `ResolveFlowLabel` dengan hasil bertipe `string`. Menentukan label kategori alur (flow label) berdasarkan prefiks
    // actionType, misalnya ”Setup”, ”Event Harian”, ”Risiko”, dll. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis; Parameter
    // `language` bertipe `string` membawa nilai language.
    private static string ResolveFlowLabel(string actionType, string language)
    // Membuka scope metode ResolveFlowLabel; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveFlowLabel.
    {
        // Memeriksa memeriksa apakah `actionType` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam ResolveFlowLabel.
        if (string.IsNullOrWhiteSpace(actionType))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(actionType)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveFlowLabel.
        {
            // Mengembalikan memanggil `L` dengan `language`, `”Aktivitas”`, `”Activity”` kepada pemanggil dalam ResolveFlowLabel; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return L(language, "Aktivitas", "Activity");
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(actionType)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveFlowLabel.
        }

        // Memeriksa hasil pencocokan `actionType` dengan pola `”MulaiSesi” or ”AkhiriSesi” or ”BagikanTieBreaker” or ”AmbilKartuDariDeck” or
        // ”KartuMasukDiscard” or ”IsiUlangPasar”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveFlowLabel.
        if (actionType is "MulaiSesi" or "AkhiriSesi" or "BagikanTieBreaker" or
            // Menggunakan nilai literal `”AmbilKartuDariDeck”` sebagai bagian ekspresi yang sedang disusun dalam ResolveFlowLabel.
            "AmbilKartuDariDeck" or "KartuMasukDiscard" or "IsiUlangPasar")
        // Membuka scope cabang if untuk kondisi `actionType is ”MulaiSesi” or ”AkhiriSesi” or ”BagikanTieBreaker” or ”AmbilKartuDariDeck” or
        // ”KartuMasukDiscard” or ”IsiUlangPasar”`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveFlowLabel.
        {
            // Mengembalikan memanggil `L` dengan `language`, `”Setup”`, `”Setup”` kepada pemanggil dalam ResolveFlowLabel; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return L(language, "Setup", "Setup");
        // Menutup scope cabang if untuk kondisi `actionType is ”MulaiSesi” or ”AkhiriSesi” or ”BagikanTieBreaker” or ”AmbilKartuDariDeck” or
        // ”KartuMasukDiscard” or ”IsiUlangPasar”`; bagian berikut berada di luar batas blok tersebut dalam ResolveFlowLabel.
        }

        // Memeriksa hasil pencocokan `actionType` dengan pola `”JumatBerkah” or ”InvestasiEmas” or ”JualEmas” or ”LewatiTransaksiEmas” or
        // ”HariMingguLibur”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveFlowLabel.
        if (actionType is "JumatBerkah" or "InvestasiEmas" or "JualEmas" or "LewatiTransaksiEmas" or "HariMingguLibur")
        // Membuka scope cabang if untuk kondisi `actionType is ”JumatBerkah” or ”InvestasiEmas” or ”JualEmas” or ”LewatiTransaksiEmas” or
        // ”HariMingguLibur”`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveFlowLabel.
        {
            // Mengembalikan memanggil `L` dengan `language`, `”Event Harian”`, `”Daily Event”` kepada pemanggil dalam ResolveFlowLabel; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return L(language, "Event Harian", "Daily Event");
        // Menutup scope cabang if untuk kondisi `actionType is ”JumatBerkah” or ”InvestasiEmas” or ”JualEmas” or ”LewatiTransaksiEmas” or
        // ”HariMingguLibur”`; bagian berikut berada di luar batas blok tersebut dalam ResolveFlowLabel.
        }

        // Memeriksa hasil pencocokan `actionType` dengan pola `”PoinPeringkatDonasi” or ”UmumkanJuaraDonasi”`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ResolveFlowLabel.
        if (actionType is "PoinPeringkatDonasi" or "UmumkanJuaraDonasi")
        // Membuka scope cabang if untuk kondisi `actionType is ”PoinPeringkatDonasi” or ”UmumkanJuaraDonasi”`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam ResolveFlowLabel.
        {
            // Mengembalikan memanggil `L` dengan `language`, `”Peduli Donasi”`, `”Donation Care”` kepada pemanggil dalam ResolveFlowLabel; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return L(language, "Peduli Donasi", "Donation Care");
        // Menutup scope cabang if untuk kondisi `actionType is ”PoinPeringkatDonasi” or ”UmumkanJuaraDonasi”`; bagian berikut berada di luar batas blok
        // tersebut dalam ResolveFlowLabel.
        }

        // Memeriksa hasil pencocokan `actionType` dengan pola `”RisikoKehidupan” or ”BayarRisiko” or ”GunakanOpsiDarurat”`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam ResolveFlowLabel.
        if (actionType is "RisikoKehidupan" or "BayarRisiko" or "GunakanOpsiDarurat")
        // Membuka scope cabang if untuk kondisi `actionType is ”RisikoKehidupan” or ”BayarRisiko” or ”GunakanOpsiDarurat”`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam ResolveFlowLabel.
        {
            // Mengembalikan memanggil `L` dengan `language`, `”Risiko”`, `”Risk”` kepada pemanggil dalam ResolveFlowLabel; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return L(language, "Risiko", "Risk");
        // Menutup scope cabang if untuk kondisi `actionType is ”RisikoKehidupan” or ”BayarRisiko” or ”GunakanOpsiDarurat”`; bagian berikut berada di luar
        // batas blok tersebut dalam ResolveFlowLabel.
        }

        // Memeriksa hasil pencocokan `actionType` dengan pola `”PinjamanSyariah” or ”BayarPinjaman” or ”Asuransi”`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ResolveFlowLabel.
        if (actionType is "PinjamanSyariah" or "BayarPinjaman" or "Asuransi")
        // Membuka scope cabang if untuk kondisi `actionType is ”PinjamanSyariah” or ”BayarPinjaman” or ”Asuransi”`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam ResolveFlowLabel.
        {
            // Mengembalikan memanggil `L` dengan `language`, `”Pembiayaan”`, `”Financing”` kepada pemanggil dalam ResolveFlowLabel; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return L(language, "Pembiayaan", "Financing");
        // Menutup scope cabang if untuk kondisi `actionType is ”PinjamanSyariah” or ”BayarPinjaman” or ”Asuransi”`; bagian berikut berada di luar batas
        // blok tersebut dalam ResolveFlowLabel.
        }

        // Memeriksa hasil pencocokan `actionType` dengan pola `”Menabung” or ”TarikTabungan” or ”TujuanFinansial”`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ResolveFlowLabel.
        if (actionType is "Menabung" or "TarikTabungan" or "TujuanFinansial")
        // Membuka scope cabang if untuk kondisi `actionType is ”Menabung” or ”TarikTabungan” or ”TujuanFinansial”`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam ResolveFlowLabel.
        {
            // Mengembalikan memanggil `L` dengan `language`, `”Tabungan”`, `”Saving”` kepada pemanggil dalam ResolveFlowLabel; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return L(language, "Tabungan", "Saving");
        // Menutup scope cabang if untuk kondisi `actionType is ”Menabung” or ”TarikTabungan” or ”TujuanFinansial”`; bagian berikut berada di luar batas
        // blok tersebut dalam ResolveFlowLabel.
        }

        // Memeriksa hasil pencocokan `actionType` dengan pola `”SetupMisiAwal”`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveFlowLabel.
        if (actionType is "SetupMisiAwal")
        // Membuka scope cabang if untuk kondisi `actionType is ”SetupMisiAwal”`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveFlowLabel.
        {
            // Mengembalikan memanggil `L` dengan `language`, `”Misi”`, `”Mission”` kepada pemanggil dalam ResolveFlowLabel; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return L(language, "Misi", "Mission");
        // Menutup scope cabang if untuk kondisi `actionType is ”SetupMisiAwal”`; bagian berikut berada di luar batas blok tersebut dalam ResolveFlowLabel.
        }

        // Memeriksa hasil pencocokan `actionType` dengan pola `”JualMasakan” or ”LewatiOrder”`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ResolveFlowLabel.
        if (actionType is "JualMasakan" or "LewatiOrder")
        // Membuka scope cabang if untuk kondisi `actionType is ”JualMasakan” or ”LewatiOrder”`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ResolveFlowLabel.
        {
            // Mengembalikan memanggil `L` dengan `language`, `”Pesanan”`, `”Order”` kepada pemanggil dalam ResolveFlowLabel; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return L(language, "Pesanan", "Order");
        // Menutup scope cabang if untuk kondisi `actionType is ”JualMasakan” or ”LewatiOrder”`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveFlowLabel.
        }

        // Memeriksa hasil pencocokan `actionType` dengan pola `”BahanMasakan” or ”Kebutuhan”`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ResolveFlowLabel.
        if (actionType is "BahanMasakan" or "Kebutuhan")
        // Membuka scope cabang if untuk kondisi `actionType is ”BahanMasakan” or ”Kebutuhan”`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ResolveFlowLabel.
        {
            // Mengembalikan memanggil `L` dengan `language`, `”Pembelian”`, `”Purchase”` kepada pemanggil dalam ResolveFlowLabel; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return L(language, "Pembelian", "Purchase");
        // Menutup scope cabang if untuk kondisi `actionType is ”BahanMasakan” or ”Kebutuhan”`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveFlowLabel.
        }

        // Mengembalikan memanggil `L` dengan `language`, `”Aktivitas”`, `”Activity”` kepada pemanggil dalam ResolveFlowLabel; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return L(language, "Aktivitas", "Activity");
    // Menutup scope metode ResolveFlowLabel; bagian berikut berada di luar batas blok tersebut dalam ResolveFlowLabel.
    }

    // Mendefinisikan metode `ResolveActionSlotRole` dengan hasil bertipe `string`; operasi ini menangani resolve aksi slot role. Masukan: Parameter
    // `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON.
    private static string ResolveActionSlotRole(string actionType, JsonElement payload)
    // Membuka scope metode ResolveActionSlotRole; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveActionSlotRole.
    {
        // Mengembalikan hasil pemetaan `actionType` melalui cabang pola switch yang cocok kepada pemanggil dalam ResolveActionSlotRole; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return actionType switch
        // Membuka scope pemetaan switch atas `actionType`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveActionSlotRole.
        {
            // Untuk pola `”RisikoKehidupan”`, menghasilkan nilai literal `”effect”` sebagai hasil switch.
            "RisikoKehidupan" => "effect",
            // Untuk pola `”BayarRisiko”`, menghasilkan nilai literal `”response”` sebagai hasil switch.
            "BayarRisiko" => "response",
            // Untuk pola `”GunakanOpsiDarurat”`, menghasilkan nilai literal `”response”` sebagai hasil switch.
            "GunakanOpsiDarurat" => "response",
            // Untuk pola `”Asuransi”` dengan syarat tambahan `IsInsuranceUse(payload)`, menghasilkan nilai literal `”response”` sebagai hasil switch.
            "Asuransi" when IsInsuranceUse(payload) => "response",
            // Untuk pola `_`, menghasilkan nilai literal `”action”` sebagai hasil switch.
            _ => "action"
        // Menutup scope pemetaan switch atas `actionType`; bagian berikut berada di luar batas blok tersebut dalam ResolveActionSlotRole.
        };
    // Menutup scope metode ResolveActionSlotRole; bagian berikut berada di luar batas blok tersebut dalam ResolveActionSlotRole.
    }

    // Mendefinisikan metode `BuildActionSlotLabel` dengan hasil bertipe `string`; operasi ini menangani build aksi slot label. Masukan: Parameter
    // `actionSlotRole` bertipe `string` membawa nilai aksi slot role; Parameter `actionSlot` bertipe `int` membawa nilai aksi slot; Parameter
    // `language` bertipe `string` membawa nilai language.
    private static string BuildActionSlotLabel(string actionSlotRole, int actionSlot, string language)
    // Membuka scope metode BuildActionSlotLabel; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildActionSlotLabel.
    {
        // Mengembalikan hasil pemetaan `actionSlotRole` melalui cabang pola switch yang cocok kepada pemanggil dalam BuildActionSlotLabel; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return actionSlotRole switch
        // Membuka scope pemetaan switch atas `actionSlotRole`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildActionSlotLabel.
        {
            // Untuk pola `”effect”`, menghasilkan memanggil `L` dengan `language`, `$”Efek Aksi {actionSlot}”`, `$”Action Effect {actionSlot}”` sebagai hasil
            // switch.
            "effect" => L(language, $"Efek Aksi {actionSlot}", $"Action Effect {actionSlot}"),
            // Untuk pola `”response”`, menghasilkan memanggil `L` dengan `language`, `$”Respons Aksi {actionSlot}”`, `$”Action Response {actionSlot}”` sebagai
            // hasil switch.
            "response" => L(language, $"Respons Aksi {actionSlot}", $"Action Response {actionSlot}"),
            // Untuk pola `_`, menghasilkan memanggil `L` dengan `language`, `$”Aksi {actionSlot}”`, `$”Action {actionSlot}”` sebagai hasil switch.
            _ => L(language, $"Aksi {actionSlot}", $"Action {actionSlot}")
        // Menutup scope pemetaan switch atas `actionSlotRole`; bagian berikut berada di luar batas blok tersebut dalam BuildActionSlotLabel.
        };
    // Menutup scope metode BuildActionSlotLabel; bagian berikut berada di luar batas blok tersebut dalam BuildActionSlotLabel.
    }

    /// <summary>
    /// Membangun deskripsi naratif untuk setiap event berdasarkan actionType,
    /// mendelegasikan ke fungsi deskripsi spesifik sesuai jenis aksi.
    /// </summary>
    /// <param name="actionType">Tipe aksi event (misal "CatatTransaksi").</param>
    /// <param name="payload">Data payload JSON dari event.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi naratif event dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `BuildFlowDescription` dengan hasil bertipe `string`. Membangun deskripsi naratif untuk setiap event berdasarkan
    // actionType, mendelegasikan ke fungsi deskripsi spesifik sesuai jenis aksi. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi
    // jenis; Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe `string` membawa
    // nilai language.
    private static string BuildFlowDescription(string actionType, JsonElement payload, string language)
    // Membuka scope metode BuildFlowDescription; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildFlowDescription.
    {
        // Menyiapkan variabel lokal `text` untuk nilai text dengan hasil pemetaan `actionType` melalui cabang pola switch yang cocok. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var text = actionType switch
        // Membuka scope pemetaan switch atas `actionType`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildFlowDescription.
        {
            // Untuk pola `”CatatTransaksi”`, menghasilkan memanggil `DescribeTransaction` dengan `payload`, `language` sebagai hasil switch.
            "CatatTransaksi" => DescribeTransaction(payload, language),
            // Untuk pola `”JumatBerkah”`, menghasilkan memanggil `DescribeFridayDonation` dengan `payload`, `language` sebagai hasil switch.
            "JumatBerkah" => DescribeFridayDonation(payload, language),
            // Untuk pola `”InvestasiEmas” or ”JualEmas”`, menghasilkan memanggil `DescribeGoldTrade` dengan `payload`, `language` sebagai hasil switch.
            "InvestasiEmas" or "JualEmas" => DescribeGoldTrade(payload, language),
            // Untuk pola `”BahanMasakan”`, menghasilkan memanggil `DescribeIngredientPurchase` dengan `payload`, `language` sebagai hasil switch.
            "BahanMasakan" => DescribeIngredientPurchase(payload, language),
            // Untuk pola `”BuangBahanMasakan”`, menghasilkan memanggil `DescribeIngredientDiscard` dengan `payload`, `language` sebagai hasil switch.
            "BuangBahanMasakan" => DescribeIngredientDiscard(payload, language),
            // Untuk pola `”JualMasakan”`, menghasilkan memanggil `DescribeOrderClaim` dengan `payload`, `language` sebagai hasil switch.
            "JualMasakan" => DescribeOrderClaim(payload, language),
            // Untuk pola `”LewatiOrder”`, menghasilkan memanggil `DescribeOrderPassed` dengan `payload`, `language` sebagai hasil switch.
            "LewatiOrder" => DescribeOrderPassed(payload, language),
            // Untuk pola `”KerjaLepas”`, menghasilkan memanggil `DescribeFreelance` dengan `payload`, `language` sebagai hasil switch.
            "KerjaLepas" => DescribeFreelance(payload, language),
            // Untuk pola `”Kebutuhan”`, menghasilkan memanggil `DescribeNeedPurchase` dengan `payload`, `language`, `ResolveNeedLabel(payload, language)`,
            // `ResolveNeedLabel(payload, language)` sebagai hasil switch.
            "Kebutuhan" => DescribeNeedPurchase(payload, language, ResolveNeedLabel(payload, language), ResolveNeedLabel(payload, language)),
            // Untuk pola `”Menabung”`, menghasilkan memanggil `DescribeSavingDeposit` dengan `payload`, `language`, `false` sebagai hasil switch.
            "Menabung" => DescribeSavingDeposit(payload, language, isWithdrawn: false),
            // Untuk pola `”TarikTabungan”`, menghasilkan memanggil `DescribeSavingDeposit` dengan `payload`, `language`, `true` sebagai hasil switch.
            "TarikTabungan" => DescribeSavingDeposit(payload, language, isWithdrawn: true),
            // Untuk pola `”TujuanFinansial”`, menghasilkan memanggil `DescribeSavingGoalAchieved` dengan `payload`, `language` sebagai hasil switch.
            "TujuanFinansial" => DescribeSavingGoalAchieved(payload, language),
            // Untuk pola `”PinjamanSyariah”`, menghasilkan memanggil `DescribeLoanTaken` dengan `payload`, `language` sebagai hasil switch.
            "PinjamanSyariah" => DescribeLoanTaken(payload, language),
            // Untuk pola `”BayarPinjaman”`, menghasilkan memanggil `DescribeLoanRepaid` dengan `payload`, `language` sebagai hasil switch.
            "BayarPinjaman" => DescribeLoanRepaid(payload, language),
            // Untuk pola `”RisikoKehidupan”`, menghasilkan memanggil `DescribeRiskLife` dengan `payload`, `language` sebagai hasil switch.
            "RisikoKehidupan" => DescribeRiskLife(payload, language),
            // Untuk pola `”GunakanOpsiDarurat”`, menghasilkan memanggil `DescribeRiskEmergency` dengan `payload`, `language` sebagai hasil switch.
            "GunakanOpsiDarurat" => DescribeRiskEmergency(payload, language),
            // Untuk pola `”Asuransi”`, menghasilkan hasil pemilihan bersyarat: ketika `IsInsuranceUse(payload)` benar gunakan `DescribeInsuranceUse(payload,
            // language)`, jika tidak gunakan `DescribeInsurancePurchase(payload, language)` sebagai hasil switch.
            "Asuransi" => IsInsuranceUse(payload) ? DescribeInsuranceUse(payload, language) : DescribeInsurancePurchase(payload, language),
            // Untuk pola `”PoinPeringkatDonasi”`, menghasilkan memanggil `DescribeRankAward` dengan `payload`, `language`, `”donasi”`, `”donation”` sebagai
            // hasil switch.
            "PoinPeringkatDonasi" => DescribeRankAward(payload, language, "donasi", "donation"),
            // Untuk pola `”UmumkanJuaraDonasi”`, menghasilkan memanggil `DescribeDonationWinnersAnnouncement` dengan `payload`, `language` sebagai hasil
            // switch.
            "UmumkanJuaraDonasi" => DescribeDonationWinnersAnnouncement(payload, language),
            // Untuk pola `”PoinPeringkatPensiun”`, menghasilkan memanggil `DescribeRankAward` dengan `payload`, `language`, `”dana pensiun”`, `”pension”`
            // sebagai hasil switch.
            "PoinPeringkatPensiun" => DescribeRankAward(payload, language, "dana pensiun", "pension"),
            // Untuk pola `”PoinEmas”`, menghasilkan memanggil `DescribePointsAward` dengan `payload`, `language`, `”emas”`, `”gold”` sebagai hasil switch.
            "PoinEmas" => DescribePointsAward(payload, language, "emas", "gold"),
            // Untuk pola `”AkhirGiliran”`, menghasilkan memanggil `DescribeTurnAction` dengan `payload`, `language` sebagai hasil switch.
            "AkhirGiliran" => DescribeTurnAction(payload, language),
            // Untuk pola `_`, menghasilkan memanggil `BuildGenericDescription` dengan `actionType`, `payload`, `language` sebagai hasil switch.
            _ => BuildGenericDescription(actionType, payload, language)
        // Menutup scope pemetaan switch atas `actionType`; bagian berikut berada di luar batas blok tersebut dalam BuildFlowDescription.
        };

        // Mengembalikan `text` (nilai text) kepada pemanggil dalam BuildFlowDescription; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return text;
    // Menutup scope metode BuildFlowDescription; bagian berikut berada di luar batas blok tersebut dalam BuildFlowDescription.
    }

    /// <summary>
    /// Mendeskripsikan event transaksi kas masuk/keluar, termasuk nominal, kategori, dan pihak terkait.
    /// </summary>
    /// <param name="payload">Data payload JSON event transaksi.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi transaksi dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeTransaction` dengan hasil bertipe `string`. Mendeskripsikan event transaksi kas masuk/keluar, termasuk nominal,
    // kategori, dan pihak terkait. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter
    // `language` bertipe `string` membawa nilai language.
    private static string DescribeTransaction(JsonElement payload, string language)
    // Membuka scope metode DescribeTransaction; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeTransaction.
    {
        // Memeriksa kebalikan kondisi `TryGetString(payload, ”direction”, out var direction)`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam DescribeTransaction.
        if (!TryGetString(payload, "direction", out var direction))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”direction”, out var direction)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam DescribeTransaction.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”CatatTransaksi”`, `payload`, `language` kepada pemanggil dalam DescribeTransaction;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("CatatTransaksi", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”direction”, out var direction)`; bagian berikut berada di luar batas blok tersebut
        // dalam DescribeTransaction.
        }

        // Menyiapkan variabel lokal `directionLabel` untuk nilai direction label dengan hasil pemilihan bersyarat: ketika `direction.Equals(”IN”,
        // StringComparison.OrdinalIgnoreCase)` benar gunakan `L(language, ”Kas masuk”, ”Cash in”)`, jika tidak gunakan `direction.Equals(”OUT”,
        // StringComparison.OrdinalIgnoreCase) ? L(language, ”Kas keluar”, ”Cash out”) : direction.ToUpperInvariant()`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var directionLabel = direction.Equals("IN", StringComparison.OrdinalIgnoreCase)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: L(language, ”Kas masuk”, ”Cash in”) dalam DescribeTransaction.
            ? L(language, "Kas masuk", "Cash in")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: direction.Equals(”OUT”, StringComparison.OrdinalIgnoreCase) dalam
            // DescribeTransaction.
            : direction.Equals("OUT", StringComparison.OrdinalIgnoreCase)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: L(language, ”Kas keluar”, ”Cash out”) dalam DescribeTransaction.
                ? L(language, "Kas keluar", "Cash out")
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: direction.ToUpperInvariant(); dalam DescribeTransaction.
                : direction.ToUpperInvariant();

        // Menyiapkan variabel lokal `amountText` untuk nilai nominal text dengan hasil pemilihan bersyarat: ketika `TryGetNumber(payload, ”amount”, out var
        // amount)` benar gunakan `FormatNumber(amount)`, jika tidak gunakan `L(language, ”nominal tidak diketahui”, ”unknown amount”)`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var amountText = TryGetNumber(payload, "amount", out var amount)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(amount) dalam DescribeTransaction.
            ? FormatNumber(amount)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”nominal tidak diketahui”, ”unknown amount”); dalam
            // DescribeTransaction.
            : L(language, "nominal tidak diketahui", "unknown amount");

        // Menyiapkan variabel lokal `categoryText` untuk nilai category text dengan hasil pemilihan bersyarat: ketika `TryGetString(payload, ”category”,
        // out var category)` benar gunakan `category`, jika tidak gunakan `L(language, ”kategori tidak diketahui”, ”unknown category”)`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var categoryText = TryGetString(payload, "category", out var category)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: category dalam DescribeTransaction.
            ? category
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”kategori tidak diketahui”, ”unknown category”); dalam
            // DescribeTransaction.
            : L(language, "kategori tidak diketahui", "unknown category");

        // Menyiapkan variabel lokal `counterpartyText` untuk nilai counterparty text dengan hasil pemilihan bersyarat: ketika `TryGetString(payload,
        // ”counterparty”, out var counterparty)` benar gunakan `L(language, $” dengan pihak {counterparty}”, $” with counterparty {counterparty}”)`, jika
        // tidak gunakan `string.Empty`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var counterpartyText = TryGetString(payload, "counterparty", out var counterparty)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: L(language, $” dengan pihak {counterparty}”, $” with counterparty
            // {counterparty}”) dalam DescribeTransaction.
            ? L(language, $" dengan pihak {counterparty}", $" with counterparty {counterparty}")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: string.Empty; dalam DescribeTransaction.
            : string.Empty;

        // Mengembalikan memanggil `L` dengan `language`, `$”{directionLabel} sebesar {amountText} pada kategori {categoryText}{counterpartyText}.”`,
        // `$”{directionLabel} {amountText} in category {categoryText}{counterpartyText}.”` kepada pemanggil dalam DescribeTransaction; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”{directionLabel} sebesar {amountText} pada kategori {categoryText}{counterpartyText}.”`; nilai ekspresi di dalam
            // kurung kurawal disisipkan saat program berjalan sebagai argumen ke `L`.
            $"{directionLabel} sebesar {amountText} pada kategori {categoryText}{counterpartyText}.",
            // Meneruskan teks interpolasi `$”{directionLabel} {amountText} in category {categoryText}{counterpartyText}.”`; nilai ekspresi di dalam kurung
            // kurawal disisipkan saat program berjalan sebagai argumen ke `L`.
            $"{directionLabel} {amountText} in category {categoryText}{counterpartyText}.");
    // Menutup scope metode DescribeTransaction; bagian berikut berada di luar batas blok tersebut dalam DescribeTransaction.
    }

    /// <summary>
    /// Mendeskripsikan event donasi Jumat, termasuk nominal yang didonasikan.
    /// </summary>
    /// <param name="payload">Data payload JSON event donasi.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi donasi Jumat dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeFridayDonation` dengan hasil bertipe `string`. Mendeskripsikan event donasi Jumat, termasuk nominal yang
    // didonasikan. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe
    // `string` membawa nilai language.
    private static string DescribeFridayDonation(JsonElement payload, string language)
    // Membuka scope metode DescribeFridayDonation; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeFridayDonation.
    {
        // Memeriksa kebalikan kondisi `TryGetNumber(payload, ”amount”, out var amount)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DescribeFridayDonation.
        if (!TryGetNumber(payload, "amount", out var amount))
        // Membuka scope cabang if untuk kondisi `!TryGetNumber(payload, ”amount”, out var amount)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam DescribeFridayDonation.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”JumatBerkah”`, `payload`, `language` kepada pemanggil dalam DescribeFridayDonation;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("JumatBerkah", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetNumber(payload, ”amount”, out var amount)`; bagian berikut berada di luar batas blok tersebut dalam
        // DescribeFridayDonation.
        }

        // Mengembalikan memanggil `L` dengan `language`, `$”Melakukan donasi Jumat sebesar {FormatNumber(amount)}.”`, `$”Made a Friday donation of
        // {FormatNumber(amount)}.”` kepada pemanggil dalam DescribeFridayDonation; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Melakukan donasi Jumat sebesar {FormatNumber(amount)}.”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `L`; Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai argumen ke
            // `FormatNumber`.
            $"Melakukan donasi Jumat sebesar {FormatNumber(amount)}.",
            // Meneruskan teks interpolasi `$”Made a Friday donation of {FormatNumber(amount)}.”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `L`; Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai argumen ke
            // `FormatNumber`.
            $"Made a Friday donation of {FormatNumber(amount)}.");
    // Menutup scope metode DescribeFridayDonation; bagian berikut berada di luar batas blok tersebut dalam DescribeFridayDonation.
    }

    /// <summary>
    /// Mendeskripsikan event perdagangan emas hari Sabtu, termasuk jenis transaksi (beli/jual),
    /// jumlah unit, harga per unit, dan total nominal.
    /// </summary>
    /// <param name="payload">Data payload JSON event perdagangan emas.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi perdagangan emas dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeGoldTrade` dengan hasil bertipe `string`. Mendeskripsikan event perdagangan emas hari Sabtu, termasuk jenis
    // transaksi (beli/jual), jumlah unit, harga per unit, dan total nominal. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail
    // event dalam format JSON; Parameter `language` bertipe `string` membawa nilai language.
    private static string DescribeGoldTrade(JsonElement payload, string language)
    // Membuka scope metode DescribeGoldTrade; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeGoldTrade.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(payload, ”trade_type”, out var tradeType) ||
        // !TryGetInt(payload, ”qty”, out var qty)` dan `!TryGetInt(payload, ”unit_price”, out var unitPrice)`; sisi kanan diperiksa hanya jika sisi kiri
        // salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam DescribeGoldTrade.
        if (!TryGetString(payload, "trade_type", out var tradeType) ||
            // Menggunakan kebalikan kondisi `TryGetInt(payload, ”qty”, out var qty)` sebagai bagian ekspresi yang sedang disusun dalam DescribeGoldTrade.
            !TryGetInt(payload, "qty", out var qty) ||
            // Menggunakan kebalikan kondisi `TryGetInt(payload, ”unit_price”, out var unitPrice)` sebagai bagian ekspresi yang sedang disusun dalam
            // DescribeGoldTrade.
            !TryGetInt(payload, "unit_price", out var unitPrice))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”trade_type”, out var tradeType) || !TryGetInt(payload, ”qty”, out var qty) ||
        // !TryGetInt(payload, ”unit_price”, out var unitPrice)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeGoldTrade.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”TransaksiEmas”`, `payload`, `language` kepada pemanggil dalam DescribeGoldTrade;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("TransaksiEmas", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”trade_type”, out var tradeType) || !TryGetInt(payload, ”qty”, out var qty) ||
        // !TryGetInt(payload, ”unit_price”, out var unitPrice)`; bagian berikut berada di luar batas blok tersebut dalam DescribeGoldTrade.
        }

        // Menyiapkan variabel lokal `amount` untuk nominal uang atau nilai transaksi yang dipakai dalam operasi dengan hasil pemilihan bersyarat: ketika
        // `TryGetInt(payload, ”amount”, out var parsedAmount)` benar gunakan `parsedAmount`, jika tidak gunakan `qty * unitPrice`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var amount = TryGetInt(payload, "amount", out var parsedAmount)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: parsedAmount dalam DescribeGoldTrade.
            ? parsedAmount
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: qty * unitPrice; dalam DescribeGoldTrade.
            : qty * unitPrice;
        // Menyiapkan variabel lokal `action` untuk nilai aksi dengan hasil pemilihan bersyarat: ketika `tradeType.Equals(”BUY”,
        // StringComparison.OrdinalIgnoreCase)` benar gunakan `L(language, ”Membeli”, ”Bought”)`, jika tidak gunakan `tradeType.Equals(”SELL”,
        // StringComparison.OrdinalIgnoreCase) ? L(language, ”Menjual”, ”Sold”) : L(language, ”Melakukan transaksi”, ”Executed trade”)`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var action = tradeType.Equals("BUY", StringComparison.OrdinalIgnoreCase)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: L(language, ”Membeli”, ”Bought”) dalam DescribeGoldTrade.
            ? L(language, "Membeli", "Bought")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: tradeType.Equals(”SELL”, StringComparison.OrdinalIgnoreCase) dalam
            // DescribeGoldTrade.
            : tradeType.Equals("SELL", StringComparison.OrdinalIgnoreCase)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: L(language, ”Menjual”, ”Sold”) dalam DescribeGoldTrade.
                ? L(language, "Menjual", "Sold")
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”Melakukan transaksi”, ”Executed trade”); dalam
                // DescribeGoldTrade.
                : L(language, "Melakukan transaksi", "Executed trade");

        // Mengembalikan memanggil `L` dengan `language`, `$”{action} emas {qty} unit x {FormatNumber(unitPrice)} (total {FormatNumber(amount)}).”`,
        // `$”{action} {qty} gold unit(s) x {FormatNumber(unitPrice)} (total {FormatNumber(amount)}).”` kepada pemanggil dalam DescribeGoldTrade; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”{action} emas {qty} unit x {FormatNumber(unitPrice)} (total {FormatNumber(amount)}).”`; nilai ekspresi di dalam
            // kurung kurawal disisipkan saat program berjalan sebagai argumen ke `L`; Meneruskan `unitPrice` (nilai unit harga) sebagai argumen ke
            // `FormatNumber`; Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai argumen ke `FormatNumber`.
            $"{action} emas {qty} unit x {FormatNumber(unitPrice)} (total {FormatNumber(amount)}).",
            // Meneruskan teks interpolasi `$”{action} {qty} gold unit(s) x {FormatNumber(unitPrice)} (total {FormatNumber(amount)}).”`; nilai ekspresi di dalam
            // kurung kurawal disisipkan saat program berjalan sebagai argumen ke `L`; Meneruskan `unitPrice` (nilai unit harga) sebagai argumen ke
            // `FormatNumber`; Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai argumen ke `FormatNumber`.
            $"{action} {qty} gold unit(s) x {FormatNumber(unitPrice)} (total {FormatNumber(amount)}).");
    // Menutup scope metode DescribeGoldTrade; bagian berikut berada di luar batas blok tersebut dalam DescribeGoldTrade.
    }

    /// <summary>
    /// Mendeskripsikan event pembelian kartu bahan, termasuk ID kartu dan biaya.
    /// </summary>
    /// <param name="payload">Data payload JSON event pembelian bahan.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi pembelian bahan dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeIngredientPurchase` dengan hasil bertipe `string`. Mendeskripsikan event pembelian kartu bahan, termasuk ID kartu
    // dan biaya. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe
    // `string` membawa nilai language.
    private static string DescribeIngredientPurchase(JsonElement payload, string language)
    // Membuka scope metode DescribeIngredientPurchase; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeIngredientPurchase.
    {
        // Memeriksa kebalikan kondisi `TryGetString(payload, ”card_id”, out var cardId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DescribeIngredientPurchase.
        if (!TryGetString(payload, "card_id", out var cardId))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”card_id”, out var cardId)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam DescribeIngredientPurchase.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”BahanMasakan”`, `payload`, `language` kepada pemanggil dalam
            // DescribeIngredientPurchase; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("BahanMasakan", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”card_id”, out var cardId)`; bagian berikut berada di luar batas blok tersebut
        // dalam DescribeIngredientPurchase.
        }

        // Menyiapkan variabel lokal `amountText` untuk nilai nominal text dengan hasil pemilihan bersyarat: ketika `TryGetNumber(payload, ”amount”, out var
        // amount)` benar gunakan `FormatNumber(amount)`, jika tidak gunakan `L(language, ”nominal tidak diketahui”, ”unknown amount”)`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var amountText = TryGetNumber(payload, "amount", out var amount)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(amount) dalam DescribeIngredientPurchase.
            ? FormatNumber(amount)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”nominal tidak diketahui”, ”unknown amount”); dalam
            // DescribeIngredientPurchase.
            : L(language, "nominal tidak diketahui", "unknown amount");

        // Mengembalikan memanggil `L` dengan `language`, `$”Membeli bahan {cardId} dengan biaya {amountText}.”`, `$”Purchased ingredient {cardId} with cost
        // {amountText}.”` kepada pemanggil dalam DescribeIngredientPurchase; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Membeli bahan {cardId} dengan biaya {amountText}.”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `L`.
            $"Membeli bahan {cardId} dengan biaya {amountText}.",
            // Meneruskan teks interpolasi `$”Purchased ingredient {cardId} with cost {amountText}.”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `L`.
            $"Purchased ingredient {cardId} with cost {amountText}.");
    // Menutup scope metode DescribeIngredientPurchase; bagian berikut berada di luar batas blok tersebut dalam DescribeIngredientPurchase.
    }

    /// <summary>
    /// Mendeskripsikan event pembuangan kartu bahan saat slot penuh, termasuk ID kartu dan jumlah.
    /// </summary>
    /// <param name="payload">Data payload JSON event pembuangan bahan.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi pembuangan bahan dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeIngredientDiscard` dengan hasil bertipe `string`. Mendeskripsikan event pembuangan kartu bahan saat slot penuh,
    // termasuk ID kartu dan jumlah. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter
    // `language` bertipe `string` membawa nilai language.
    private static string DescribeIngredientDiscard(JsonElement payload, string language)
    // Membuka scope metode DescribeIngredientDiscard; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeIngredientDiscard.
    {
        // Memeriksa kebalikan kondisi `TryGetString(payload, ”card_id”, out var cardId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DescribeIngredientDiscard.
        if (!TryGetString(payload, "card_id", out var cardId))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”card_id”, out var cardId)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam DescribeIngredientDiscard.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”BuangBahanMasakan”`, `payload`, `language` kepada pemanggil dalam
            // DescribeIngredientDiscard; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("BuangBahanMasakan", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”card_id”, out var cardId)`; bagian berikut berada di luar batas blok tersebut
        // dalam DescribeIngredientDiscard.
        }

        // Menyiapkan variabel lokal `amountText` untuk nilai nominal text dengan hasil pemilihan bersyarat: ketika `TryGetNumber(payload, ”amount”, out var
        // amount)` benar gunakan `FormatNumber(amount)`, jika tidak gunakan `L(language, ”jumlah tidak diketahui”, ”unknown quantity”)`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var amountText = TryGetNumber(payload, "amount", out var amount)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(amount) dalam DescribeIngredientDiscard.
            ? FormatNumber(amount)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”jumlah tidak diketahui”, ”unknown quantity”); dalam
            // DescribeIngredientDiscard.
            : L(language, "jumlah tidak diketahui", "unknown quantity");

        // Mengembalikan memanggil `L` dengan `language`, `$”Membuang bahan {cardId} sebanyak {amountText}.”`, `$”Discarded ingredient {cardId} with
        // quantity {amountText}.”` kepada pemanggil dalam DescribeIngredientDiscard; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Membuang bahan {cardId} sebanyak {amountText}.”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `L`.
            $"Membuang bahan {cardId} sebanyak {amountText}.",
            // Meneruskan teks interpolasi `$”Discarded ingredient {cardId} with quantity {amountText}.”`; nilai ekspresi di dalam kurung kurawal disisipkan
            // saat program berjalan sebagai argumen ke `L`.
            $"Discarded ingredient {cardId} with quantity {amountText}.");
    // Menutup scope metode DescribeIngredientDiscard; bagian berikut berada di luar batas blok tersebut dalam DescribeIngredientDiscard.
    }

    /// <summary>
    /// Mendeskripsikan event klaim pesanan, termasuk jumlah bahan yang digunakan dan pemasukan yang diterima.
    /// </summary>
    /// <param name="payload">Data payload JSON event klaim pesanan.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi klaim pesanan dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeOrderClaim` dengan hasil bertipe `string`. Mendeskripsikan event klaim pesanan, termasuk jumlah bahan yang
    // digunakan dan pemasukan yang diterima. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON;
    // Parameter `language` bertipe `string` membawa nilai language.
    private static string DescribeOrderClaim(JsonElement payload, string language)
    // Membuka scope metode DescribeOrderClaim; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeOrderClaim.
    {
        // Menyiapkan variabel lokal `incomeText` untuk nilai pemasukan text dengan hasil pemilihan bersyarat: ketika `TryGetNumber(payload, ”income”, out
        // var income)` benar gunakan `FormatNumber(income)`, jika tidak gunakan `L(language, ”nominal tidak diketahui”, ”unknown amount”)`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var incomeText = TryGetNumber(payload, "income", out var income)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(income) dalam DescribeOrderClaim.
            ? FormatNumber(income)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”nominal tidak diketahui”, ”unknown amount”); dalam
            // DescribeOrderClaim.
            : L(language, "nominal tidak diketahui", "unknown amount");
        // Menyiapkan variabel lokal `ingredientCountText` untuk nilai bahan jumlah text dengan hasil pemilihan bersyarat: ketika `TryGetArrayCount(payload,
        // ”required_ingredient_card_ids”, out var count)` benar gunakan `count.ToString(CultureInfo.InvariantCulture)`, jika tidak gunakan `L(language,
        // ”?”, ”?”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ingredientCountText = TryGetArrayCount(payload, "required_ingredient_card_ids", out var count)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: count.ToString(CultureInfo.InvariantCulture) dalam
            // DescribeOrderClaim.
            ? count.ToString(CultureInfo.InvariantCulture)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”?”, ”?”); dalam DescribeOrderClaim.
            : L(language, "?", "?");

        // Mengembalikan memanggil `L` dengan `language`, `$”Menyelesaikan order dengan {ingredientCountText} bahan dan menerima pemasukan {incomeText}.”`,
        // `$”Claimed an order using {ingredientCountText} ingredient(s) and received {incomeText} income.”` kepada pemanggil dalam DescribeOrderClaim;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Menyelesaikan order dengan {ingredientCountText} bahan dan menerima pemasukan {incomeText}.”`; nilai ekspresi di
            // dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `L`.
            $"Menyelesaikan order dengan {ingredientCountText} bahan dan menerima pemasukan {incomeText}.",
            // Meneruskan teks interpolasi `$”Claimed an order using {ingredientCountText} ingredient(s) and received {incomeText} income.”`; nilai ekspresi di
            // dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `L`.
            $"Claimed an order using {ingredientCountText} ingredient(s) and received {incomeText} income.");
    // Menutup scope metode DescribeOrderClaim; bagian berikut berada di luar batas blok tersebut dalam DescribeOrderClaim.
    }

    /// <summary>
    /// Mendeskripsikan event pesanan yang dilewati (tidak diklaim), termasuk potensi pemasukan.
    /// </summary>
    /// <param name="payload">Data payload JSON event pesanan dilewati.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi pesanan yang dilewati dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeOrderPassed` dengan hasil bertipe `string`. Mendeskripsikan event pesanan yang dilewati (tidak diklaim), termasuk
    // potensi pemasukan. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe
    // `string` membawa nilai language.
    private static string DescribeOrderPassed(JsonElement payload, string language)
    // Membuka scope metode DescribeOrderPassed; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeOrderPassed.
    {
        // Menyiapkan variabel lokal `ingredientCountText` untuk nilai bahan jumlah text dengan hasil pemilihan bersyarat: ketika `TryGetArrayCount(payload,
        // ”required_ingredient_card_ids”, out var count)` benar gunakan `count.ToString(CultureInfo.InvariantCulture)`, jika tidak gunakan `L(language,
        // ”?”, ”?”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ingredientCountText = TryGetArrayCount(payload, "required_ingredient_card_ids", out var count)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: count.ToString(CultureInfo.InvariantCulture) dalam
            // DescribeOrderPassed.
            ? count.ToString(CultureInfo.InvariantCulture)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”?”, ”?”); dalam DescribeOrderPassed.
            : L(language, "?", "?");
        // Menyiapkan variabel lokal `incomeText` untuk nilai pemasukan text dengan hasil pemilihan bersyarat: ketika `TryGetNumber(payload, ”income”, out
        // var income)` benar gunakan `FormatNumber(income)`, jika tidak gunakan `L(language, ”nominal tidak diketahui”, ”unknown amount”)`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var incomeText = TryGetNumber(payload, "income", out var income)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(income) dalam DescribeOrderPassed.
            ? FormatNumber(income)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”nominal tidak diketahui”, ”unknown amount”); dalam
            // DescribeOrderPassed.
            : L(language, "nominal tidak diketahui", "unknown amount");

        // Mengembalikan memanggil `L` dengan `language`, `$”Melewati order yang membutuhkan {ingredientCountText} bahan (potensi pemasukan
        // {incomeText}).”`, `$”Passed an order requiring {ingredientCountText} ingredient(s) (potential income {incomeText}).”` kepada pemanggil dalam
        // DescribeOrderPassed; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Melewati order yang membutuhkan {ingredientCountText} bahan (potensi pemasukan {incomeText}).”`; nilai ekspresi di
            // dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `L`.
            $"Melewati order yang membutuhkan {ingredientCountText} bahan (potensi pemasukan {incomeText}).",
            // Meneruskan teks interpolasi `$”Passed an order requiring {ingredientCountText} ingredient(s) (potential income {incomeText}).”`; nilai ekspresi
            // di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `L`.
            $"Passed an order requiring {ingredientCountText} ingredient(s) (potential income {incomeText}).");
    // Menutup scope metode DescribeOrderPassed; bagian berikut berada di luar batas blok tersebut dalam DescribeOrderPassed.
    }

    /// <summary>
    /// Mendeskripsikan event pekerjaan freelance, termasuk nominal pemasukan yang diterima.
    /// </summary>
    /// <param name="payload">Data payload JSON event freelance.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi pekerjaan freelance dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeFreelance` dengan hasil bertipe `string`. Mendeskripsikan event pekerjaan freelance, termasuk nominal pemasukan
    // yang diterima. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe
    // `string` membawa nilai language.
    private static string DescribeFreelance(JsonElement payload, string language)
    // Membuka scope metode DescribeFreelance; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeFreelance.
    {
        // Memeriksa kebalikan kondisi `TryGetNumber(payload, ”amount”, out var amount)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DescribeFreelance.
        if (!TryGetNumber(payload, "amount", out var amount))
        // Membuka scope cabang if untuk kondisi `!TryGetNumber(payload, ”amount”, out var amount)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam DescribeFreelance.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”KerjaLepas”`, `payload`, `language` kepada pemanggil dalam DescribeFreelance; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("KerjaLepas", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetNumber(payload, ”amount”, out var amount)`; bagian berikut berada di luar batas blok tersebut dalam
        // DescribeFreelance.
        }

        // Mengembalikan memanggil `L` dengan `language`, `$”Menyelesaikan pekerjaan freelance dengan pemasukan {FormatNumber(amount)}.”`, `$”Completed
        // freelance work and earned {FormatNumber(amount)}.”` kepada pemanggil dalam DescribeFreelance; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Menyelesaikan pekerjaan freelance dengan pemasukan {FormatNumber(amount)}.”`; nilai ekspresi di dalam kurung
            // kurawal disisipkan saat program berjalan sebagai argumen ke `L`; Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam
            // operasi) sebagai argumen ke `FormatNumber`.
            $"Menyelesaikan pekerjaan freelance dengan pemasukan {FormatNumber(amount)}.",
            // Meneruskan teks interpolasi `$”Completed freelance work and earned {FormatNumber(amount)}.”`; nilai ekspresi di dalam kurung kurawal disisipkan
            // saat program berjalan sebagai argumen ke `L`; Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai argumen
            // ke `FormatNumber`.
            $"Completed freelance work and earned {FormatNumber(amount)}.");
    // Menutup scope metode DescribeFreelance; bagian berikut berada di luar batas blok tersebut dalam DescribeFreelance.
    }

    /// <summary>
    /// Mendeskripsikan event pembelian kartu kebutuhan (primer/sekunder/tersier),
    /// termasuk ID kartu, biaya, dan poin yang diperoleh.
    /// </summary>
    /// <param name="payload">Data payload JSON event pembelian kebutuhan.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <param name="needTypeId">Label jenis kebutuhan dalam bahasa Indonesia.</param>
    /// <param name="needTypeEn">Label jenis kebutuhan dalam bahasa Inggris.</param>
    /// <returns>Deskripsi pembelian kebutuhan dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeNeedPurchase` dengan hasil bertipe `string`. Mendeskripsikan event pembelian kartu kebutuhan
    // (primer/sekunder/tersier), termasuk ID kartu, biaya, dan poin yang diperoleh. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan
    // detail event dalam format JSON; Parameter `language` bertipe `string` membawa nilai language; Parameter `needTypeId` bertipe `string` membawa
    // nilai kebutuhan jenis identitas; Parameter `needTypeEn` bertipe `string` membawa nilai kebutuhan jenis en.
    private static string DescribeNeedPurchase(JsonElement payload, string language, string needTypeId, string needTypeEn)
    // Membuka scope metode DescribeNeedPurchase; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeNeedPurchase.
    {
        // Memeriksa kebalikan kondisi `TryGetString(payload, ”card_id”, out var cardId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DescribeNeedPurchase.
        if (!TryGetString(payload, "card_id", out var cardId))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”card_id”, out var cardId)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam DescribeNeedPurchase.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”Kebutuhan”`, `payload`, `language` kepada pemanggil dalam DescribeNeedPurchase;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("Kebutuhan", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”card_id”, out var cardId)`; bagian berikut berada di luar batas blok tersebut
        // dalam DescribeNeedPurchase.
        }

        // Menyiapkan variabel lokal `amountText` untuk nilai nominal text dengan hasil pemilihan bersyarat: ketika `TryGetNumber(payload, ”amount”, out var
        // amount)` benar gunakan `FormatNumber(amount)`, jika tidak gunakan `L(language, ”nominal tidak diketahui”, ”unknown amount”)`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var amountText = TryGetNumber(payload, "amount", out var amount)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(amount) dalam DescribeNeedPurchase.
            ? FormatNumber(amount)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”nominal tidak diketahui”, ”unknown amount”); dalam
            // DescribeNeedPurchase.
            : L(language, "nominal tidak diketahui", "unknown amount");
        // Menyiapkan variabel lokal `pointsText` untuk nilai poin text dengan hasil pemilihan bersyarat: ketika `TryGetNumber(payload, ”points”, out var
        // points)` benar gunakan `FormatNumber(points)`, jika tidak gunakan `L(language, ”poin kebahagiaan tidak diketahui”, ”unknown happiness points”)`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var pointsText = TryGetNumber(payload, "points", out var points)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(points) dalam DescribeNeedPurchase.
            ? FormatNumber(points)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”poin kebahagiaan tidak diketahui”, ”unknown happiness
            // points”); dalam DescribeNeedPurchase.
            : L(language, "poin kebahagiaan tidak diketahui", "unknown happiness points");

        // Mengembalikan memanggil `L` dengan `language`, `$”Membeli {needTypeId} {cardId} (biaya {amountText}, poin kebahagiaan {pointsText}).”`,
        // `$”Purchased {needTypeEn} card {cardId} (cost {amountText}, happiness points {pointsText}).”` kepada pemanggil dalam DescribeNeedPurchase;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Membeli {needTypeId} {cardId} (biaya {amountText}, poin kebahagiaan {pointsText}).”`; nilai ekspresi di dalam
            // kurung kurawal disisipkan saat program berjalan sebagai argumen ke `L`.
            $"Membeli {needTypeId} {cardId} (biaya {amountText}, poin kebahagiaan {pointsText}).",
            // Meneruskan teks interpolasi `$”Purchased {needTypeEn} card {cardId} (cost {amountText}, happiness points {pointsText}).”`; nilai ekspresi di
            // dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `L`.
            $"Purchased {needTypeEn} card {cardId} (cost {amountText}, happiness points {pointsText}).");
    // Menutup scope metode DescribeNeedPurchase; bagian berikut berada di luar batas blok tersebut dalam DescribeNeedPurchase.
    }

    // Mendefinisikan metode `ResolveNeedLabel` dengan hasil bertipe `string`; operasi ini menangani resolve kebutuhan label. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe `string` membawa nilai language.
    private static string ResolveNeedLabel(JsonElement payload, string language)
    // Membuka scope metode ResolveNeedLabel; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveNeedLabel.
    {
        // Menyiapkan variabel lokal `fallback` untuk nilai fallback dengan memanggil `L` dengan `language`, `”kebutuhan”`, `”need”`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var fallback = L(language, "kebutuhan", "need");
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!TryGetString(payload, ”need_tier”, out var tier) && !TryGetString(payload,
        // ”tier”, out tier)` dan `!TryGetString(payload, ”need_type”, out tier)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan
        // ketika kondisi ini bernilai benar dalam ResolveNeedLabel.
        if (!TryGetString(payload, "need_tier", out var tier) &&
            // Menggunakan kebalikan kondisi `TryGetString(payload, ”tier”, out tier)` sebagai bagian ekspresi yang sedang disusun dalam ResolveNeedLabel.
            !TryGetString(payload, "tier", out tier) &&
            // Menggunakan kebalikan kondisi `TryGetString(payload, ”need_type”, out tier)` sebagai bagian ekspresi yang sedang disusun dalam ResolveNeedLabel.
            !TryGetString(payload, "need_type", out tier))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”need_tier”, out var tier) && !TryGetString(payload, ”tier”, out tier) &&
        // !TryGetString(payload, ”need_type”, out tier)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveNeedLabel.
        {
            // Mengembalikan `fallback` (nilai fallback) kepada pemanggil dalam ResolveNeedLabel; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return fallback;
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”need_tier”, out var tier) && !TryGetString(payload, ”tier”, out tier) &&
        // !TryGetString(payload, ”need_type”, out tier)`; bagian berikut berada di luar batas blok tersebut dalam ResolveNeedLabel.
        }

        // Mengembalikan hasil pemetaan `tier.ToUpperInvariant()` melalui cabang pola switch yang cocok kepada pemanggil dalam ResolveNeedLabel; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return tier.ToUpperInvariant() switch
        // Membuka scope pemetaan switch atas `tier.ToUpperInvariant()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveNeedLabel.
        {
            // Untuk pola `”PRIMARY” or ”PRIMER”`, menghasilkan memanggil `L` dengan `language`, `”kebutuhan primer”`, `”primary need”` sebagai hasil switch.
            "PRIMARY" or "PRIMER" => L(language, "kebutuhan primer", "primary need"),
            // Untuk pola `”SECONDARY” or ”SEKUNDER”`, menghasilkan memanggil `L` dengan `language`, `”kebutuhan sekunder”`, `”secondary need”` sebagai hasil
            // switch.
            "SECONDARY" or "SEKUNDER" => L(language, "kebutuhan sekunder", "secondary need"),
            // Untuk pola `”TERTIARY” or ”TERSIER”`, menghasilkan memanggil `L` dengan `language`, `”kebutuhan tersier”`, `”tertiary need”` sebagai hasil
            // switch.
            "TERTIARY" or "TERSIER" => L(language, "kebutuhan tersier", "tertiary need"),
            // Untuk pola `_`, menghasilkan `fallback` (nilai fallback) sebagai hasil switch.
            _ => fallback
        // Menutup scope pemetaan switch atas `tier.ToUpperInvariant()`; bagian berikut berada di luar batas blok tersebut dalam ResolveNeedLabel.
        };
    // Menutup scope metode ResolveNeedLabel; bagian berikut berada di luar batas blok tersebut dalam ResolveNeedLabel.
    }

    /// <summary>
    /// Mendeskripsikan event setoran atau penarikan tabungan tujuan keuangan,
    /// termasuk ID tujuan dan nominal.
    /// </summary>
    /// <param name="payload">Data payload JSON event tabungan.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <param name="isWithdrawn">True jika penarikan, false jika setoran.</param>
    /// <returns>Deskripsi setoran/penarikan tabungan dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeSavingDeposit` dengan hasil bertipe `string`. Mendeskripsikan event setoran atau penarikan tabungan tujuan
    // keuangan, termasuk ID tujuan dan nominal. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON;
    // Parameter `language` bertipe `string` membawa nilai language; Parameter `isWithdrawn` bertipe `bool` membawa nilai berstatus withdrawn.
    private static string DescribeSavingDeposit(JsonElement payload, string language, bool isWithdrawn)
    // Membuka scope metode DescribeSavingDeposit; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeSavingDeposit.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(payload, ”goal_id”, out var goalId)` dan
        // `!TryGetNumber(payload, ”amount”, out var amount)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam DescribeSavingDeposit.
        if (!TryGetString(payload, "goal_id", out var goalId) || !TryGetNumber(payload, "amount", out var amount))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”goal_id”, out var goalId) || !TryGetNumber(payload, ”amount”, out var amount)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeSavingDeposit.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `isWithdrawn ? ”TarikTabungan” : ”Menabung”`, `payload`, `language` kepada pemanggil
            // dalam DescribeSavingDeposit; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription(isWithdrawn ? "TarikTabungan" : "Menabung", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”goal_id”, out var goalId) || !TryGetNumber(payload, ”amount”, out var amount)`;
        // bagian berikut berada di luar batas blok tersebut dalam DescribeSavingDeposit.
        }

        // Mengembalikan hasil pemilihan bersyarat: ketika `isWithdrawn` benar gunakan `L( language, $”Menarik {FormatNumber(amount)} dari tabungan tujuan
        // {goalId}.”, $”Withdrew {FormatNumber(amount)} from saving goal {goalId}.”)`, jika tidak gunakan `L( language, $”Menyetor {FormatNumber(amount)}
        // ke tabungan tujuan {goalId}.”, $”Deposited {FormatNumber(amount)} to saving goal {goalId}.”)` kepada pemanggil dalam DescribeSavingDeposit;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return isWithdrawn
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: L( dalam DescribeSavingDeposit.
            ? L(
                // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
                language,
                // Meneruskan teks interpolasi `$”Menarik {FormatNumber(amount)} dari tabungan tujuan {goalId}.”`; nilai ekspresi di dalam kurung kurawal disisipkan
                // saat program berjalan sebagai argumen ke `L`; Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai argumen
                // ke `FormatNumber`.
                $"Menarik {FormatNumber(amount)} dari tabungan tujuan {goalId}.",
                // Meneruskan teks interpolasi `$”Withdrew {FormatNumber(amount)} from saving goal {goalId}.”`; nilai ekspresi di dalam kurung kurawal disisipkan
                // saat program berjalan sebagai argumen ke `L`; Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai argumen
                // ke `FormatNumber`.
                $"Withdrew {FormatNumber(amount)} from saving goal {goalId}.")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L( dalam DescribeSavingDeposit.
            : L(
                // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
                language,
                // Meneruskan teks interpolasi `$”Menyetor {FormatNumber(amount)} ke tabungan tujuan {goalId}.”`; nilai ekspresi di dalam kurung kurawal disisipkan
                // saat program berjalan sebagai argumen ke `L`; Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai argumen
                // ke `FormatNumber`.
                $"Menyetor {FormatNumber(amount)} ke tabungan tujuan {goalId}.",
                // Meneruskan teks interpolasi `$”Deposited {FormatNumber(amount)} to saving goal {goalId}.”`; nilai ekspresi di dalam kurung kurawal disisipkan
                // saat program berjalan sebagai argumen ke `L`; Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai argumen
                // ke `FormatNumber`.
                $"Deposited {FormatNumber(amount)} to saving goal {goalId}.");
    // Menutup scope metode DescribeSavingDeposit; bagian berikut berada di luar batas blok tersebut dalam DescribeSavingDeposit.
    }

    /// <summary>
    /// Mendeskripsikan event tercapainya target tabungan tujuan keuangan,
    /// termasuk ID tujuan, poin, dan biaya.
    /// </summary>
    /// <param name="payload">Data payload JSON event pencapaian target tabungan.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi pencapaian target tabungan dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeSavingGoalAchieved` dengan hasil bertipe `string`. Mendeskripsikan event tercapainya target tabungan tujuan
    // keuangan, termasuk ID tujuan, poin, dan biaya. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON;
    // Parameter `language` bertipe `string` membawa nilai language.
    private static string DescribeSavingGoalAchieved(JsonElement payload, string language)
    // Membuka scope metode DescribeSavingGoalAchieved; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeSavingGoalAchieved.
    {
        // Memeriksa kebalikan kondisi `TryGetString(payload, ”goal_id”, out var goalId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DescribeSavingGoalAchieved.
        if (!TryGetString(payload, "goal_id", out var goalId))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”goal_id”, out var goalId)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam DescribeSavingGoalAchieved.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”TujuanFinansial”`, `payload`, `language` kepada pemanggil dalam
            // DescribeSavingGoalAchieved; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("TujuanFinansial", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”goal_id”, out var goalId)`; bagian berikut berada di luar batas blok tersebut
        // dalam DescribeSavingGoalAchieved.
        }

        // Menyiapkan variabel lokal `pointsText` untuk nilai poin text dengan hasil pemilihan bersyarat: ketika `TryGetNumber(payload, ”points”, out var
        // points)` benar gunakan `FormatNumber(points)`, jika tidak gunakan `L(language, ”poin kebahagiaan tidak diketahui”, ”unknown happiness points”)`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var pointsText = TryGetNumber(payload, "points", out var points)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(points) dalam DescribeSavingGoalAchieved.
            ? FormatNumber(points)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”poin kebahagiaan tidak diketahui”, ”unknown happiness
            // points”); dalam DescribeSavingGoalAchieved.
            : L(language, "poin kebahagiaan tidak diketahui", "unknown happiness points");
        // Menyiapkan variabel lokal `costText` untuk nilai biaya text dengan hasil pemilihan bersyarat: ketika `TryGetNumber(payload, ”cost”, out var
        // cost)` benar gunakan `FormatNumber(cost)`, jika tidak gunakan `L(language, ”biaya tidak diketahui”, ”unknown cost”)`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var costText = TryGetNumber(payload, "cost", out var cost)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(cost) dalam DescribeSavingGoalAchieved.
            ? FormatNumber(cost)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”biaya tidak diketahui”, ”unknown cost”); dalam
            // DescribeSavingGoalAchieved.
            : L(language, "biaya tidak diketahui", "unknown cost");

        // Mengembalikan memanggil `L` dengan `language`, `$”Target tabungan {goalId} tercapai (poin kebahagiaan {pointsText}, biaya {costText}).”`,
        // `$”Saving goal {goalId} was achieved (happiness points {pointsText}, cost {costText}).”` kepada pemanggil dalam DescribeSavingGoalAchieved;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Target tabungan {goalId} tercapai (poin kebahagiaan {pointsText}, biaya {costText}).”`; nilai ekspresi di dalam
            // kurung kurawal disisipkan saat program berjalan sebagai argumen ke `L`.
            $"Target tabungan {goalId} tercapai (poin kebahagiaan {pointsText}, biaya {costText}).",
            // Meneruskan teks interpolasi `$”Saving goal {goalId} was achieved (happiness points {pointsText}, cost {costText}).”`; nilai ekspresi di dalam
            // kurung kurawal disisipkan saat program berjalan sebagai argumen ke `L`.
            $"Saving goal {goalId} was achieved (happiness points {pointsText}, cost {costText}).");
    // Menutup scope metode DescribeSavingGoalAchieved; bagian berikut berada di luar batas blok tersebut dalam DescribeSavingGoalAchieved.
    }

    /// <summary>
    /// Mendeskripsikan event pengambilan pinjaman syariah,
    /// termasuk ID pinjaman, pokok, cicilan, dan durasi.
    /// </summary>
    /// <param name="payload">Data payload JSON event pengambilan pinjaman.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi pengambilan pinjaman dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeLoanTaken` dengan hasil bertipe `string`. Mendeskripsikan event pengambilan pinjaman syariah, termasuk ID
    // pinjaman, pokok, cicilan, dan durasi. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter
    // `language` bertipe `string` membawa nilai language.
    private static string DescribeLoanTaken(JsonElement payload, string language)
    // Membuka scope metode DescribeLoanTaken; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeLoanTaken.
    {
        // Memeriksa kebalikan kondisi `TryGetString(payload, ”loan_id”, out var loanId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DescribeLoanTaken.
        if (!TryGetString(payload, "loan_id", out var loanId))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”loan_id”, out var loanId)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam DescribeLoanTaken.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”PinjamanSyariah”`, `payload`, `language` kepada pemanggil dalam DescribeLoanTaken;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("PinjamanSyariah", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”loan_id”, out var loanId)`; bagian berikut berada di luar batas blok tersebut
        // dalam DescribeLoanTaken.
        }

        // Menyiapkan variabel lokal `principalText` untuk nilai principal text dengan hasil pemilihan bersyarat: ketika `TryGetNumber(payload, ”principal”,
        // out var principal)` benar gunakan `FormatNumber(principal)`, jika tidak gunakan `L(language, ”nominal tidak diketahui”, ”unknown amount”)`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var principalText = TryGetNumber(payload, "principal", out var principal)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(principal) dalam DescribeLoanTaken.
            ? FormatNumber(principal)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”nominal tidak diketahui”, ”unknown amount”); dalam
            // DescribeLoanTaken.
            : L(language, "nominal tidak diketahui", "unknown amount");
        // Menyiapkan variabel lokal `repaymentText` untuk nilai repayment text dengan hasil pemilihan bersyarat: ketika `TryGetNumber(payload,
        // ”repayment_amount”, out var repaymentAmount)` benar gunakan `FormatNumber(repaymentAmount)`, jika tidak gunakan `TryGetNumber(payload,
        // ”installment”, out var installment) ? FormatNumber(installment) : L(language, ”nominal pelunasan tidak diketahui”, ”unknown repayment amount”)`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var repaymentText = TryGetNumber(payload, "repayment_amount", out var repaymentAmount)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(repaymentAmount) dalam DescribeLoanTaken.
            ? FormatNumber(repaymentAmount)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: TryGetNumber(payload, ”installment”, out var installment) dalam
            // DescribeLoanTaken.
            : TryGetNumber(payload, "installment", out var installment)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: FormatNumber(installment) dalam DescribeLoanTaken.
                ? FormatNumber(installment)
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”nominal pelunasan tidak diketahui”, ”unknown repayment
                // amount”); dalam DescribeLoanTaken.
                : L(language, "nominal pelunasan tidak diketahui", "unknown repayment amount");
        // Menyiapkan variabel lokal `durationText` untuk nilai duration text dengan hasil pemilihan bersyarat: ketika `TryGetInt(payload, ”duration_turn”,
        // out var durationTurn)` benar gunakan `durationTurn.ToString(CultureInfo.InvariantCulture)`, jika tidak gunakan `L(language, ”?”, ”?”)`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var durationText = TryGetInt(payload, "duration_turn", out var durationTurn)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: durationTurn.ToString(CultureInfo.InvariantCulture) dalam
            // DescribeLoanTaken.
            ? durationTurn.ToString(CultureInfo.InvariantCulture)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: L(language, ”?”, ”?”); dalam DescribeLoanTaken.
            : L(language, "?", "?");

        // Mengembalikan memanggil `L` dengan `language`, `$”Mengambil pinjaman {loanId} (pokok {principalText}, pelunasan {repaymentText}, durasi
        // {durationText} turn).”`, `$”Took loan {loanId} (principal {principalText}, repayment {repaymentText}, duration {durationText} turns).”` kepada
        // pemanggil dalam DescribeLoanTaken; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Mengambil pinjaman {loanId} (pokok {principalText}, pelunasan {repaymentText}, durasi {durationText} turn).”`;
            // nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `L`.
            $"Mengambil pinjaman {loanId} (pokok {principalText}, pelunasan {repaymentText}, durasi {durationText} turn).",
            // Meneruskan teks interpolasi `$”Took loan {loanId} (principal {principalText}, repayment {repaymentText}, duration {durationText} turns).”`; nilai
            // ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `L`.
            $"Took loan {loanId} (principal {principalText}, repayment {repaymentText}, duration {durationText} turns).");
    // Menutup scope metode DescribeLoanTaken; bagian berikut berada di luar batas blok tersebut dalam DescribeLoanTaken.
    }

    /// <summary>
    /// Mendeskripsikan event pelunasan pinjaman syariah, termasuk ID pinjaman dan nominal pembayaran.
    /// </summary>
    /// <param name="payload">Data payload JSON event pelunasan pinjaman.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi pelunasan pinjaman dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeLoanRepaid` dengan hasil bertipe `string`. Mendeskripsikan event pelunasan pinjaman syariah, termasuk ID pinjaman
    // dan nominal pembayaran. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language`
    // bertipe `string` membawa nilai language.
    private static string DescribeLoanRepaid(JsonElement payload, string language)
    // Membuka scope metode DescribeLoanRepaid; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeLoanRepaid.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(payload, ”loan_id”, out var loanId)` dan
        // `!TryGetNumber(payload, ”amount”, out var amount)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam DescribeLoanRepaid.
        if (!TryGetString(payload, "loan_id", out var loanId) || !TryGetNumber(payload, "amount", out var amount))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”loan_id”, out var loanId) || !TryGetNumber(payload, ”amount”, out var amount)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeLoanRepaid.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”BayarPinjaman”`, `payload`, `language` kepada pemanggil dalam DescribeLoanRepaid;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("BayarPinjaman", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”loan_id”, out var loanId) || !TryGetNumber(payload, ”amount”, out var amount)`;
        // bagian berikut berada di luar batas blok tersebut dalam DescribeLoanRepaid.
        }

        // Mengembalikan memanggil `L` dengan `language`, `$”Membayar pinjaman {loanId} sebesar {FormatNumber(amount)}.”`, `$”Repaid loan {loanId} by
        // {FormatNumber(amount)}.”` kepada pemanggil dalam DescribeLoanRepaid; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Membayar pinjaman {loanId} sebesar {FormatNumber(amount)}.”`; nilai ekspresi di dalam kurung kurawal disisipkan
            // saat program berjalan sebagai argumen ke `L`; Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai argumen
            // ke `FormatNumber`.
            $"Membayar pinjaman {loanId} sebesar {FormatNumber(amount)}.",
            // Meneruskan teks interpolasi `$”Repaid loan {loanId} by {FormatNumber(amount)}.”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `L`; Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai argumen ke
            // `FormatNumber`.
            $"Repaid loan {loanId} by {FormatNumber(amount)}.");
    // Menutup scope metode DescribeLoanRepaid; bagian berikut berada di luar batas blok tersebut dalam DescribeLoanRepaid.
    }

    /// <summary>
    /// Mendeskripsikan event kartu risiko kehidupan yang ditarik.
    /// </summary>
    /// <param name="payload">Data payload JSON event risiko kehidupan.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi event risiko kehidupan dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeRiskLife` dengan hasil bertipe `string`. Mendeskripsikan event kartu risiko kehidupan yang ditarik. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe `string` membawa nilai
    // language.
    private static string DescribeRiskLife(JsonElement payload, string language)
    // Membuka scope metode DescribeRiskLife; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeRiskLife.
    {
        // Memeriksa kebalikan kondisi `TryGetString(payload, ”risk_id”, out var riskId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DescribeRiskLife.
        if (!TryGetString(payload, "risk_id", out var riskId))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”risk_id”, out var riskId)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam DescribeRiskLife.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”RisikoKehidupan”`, `payload`, `language` kepada pemanggil dalam DescribeRiskLife;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("RisikoKehidupan", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”risk_id”, out var riskId)`; bagian berikut berada di luar batas blok tersebut
        // dalam DescribeRiskLife.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(payload, ”direction”, out var direction)` dan
        // `!TryGetNumber(payload, ”amount”, out var amount)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam DescribeRiskLife.
        if (!TryGetString(payload, "direction", out var direction) ||
            // Menggunakan kebalikan kondisi `TryGetNumber(payload, ”amount”, out var amount)` sebagai bagian ekspresi yang sedang disusun dalam
            // DescribeRiskLife.
            !TryGetNumber(payload, "amount", out var amount))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”direction”, out var direction) || !TryGetNumber(payload, ”amount”, out var
        // amount)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeRiskLife.
        {
            // Mengembalikan memanggil `L` dengan `language`, `$”Kartu risiko {riskId} aktif.”`, `$”Risk card {riskId} triggered.”` kepada pemanggil dalam
            // DescribeRiskLife; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return L(
                // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
                language,
                // Meneruskan teks interpolasi `$”Kartu risiko {riskId} aktif.”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai
                // argumen ke `L`.
                $"Kartu risiko {riskId} aktif.",
                // Meneruskan teks interpolasi `$”Risk card {riskId} triggered.”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai
                // argumen ke `L`.
                $"Risk card {riskId} triggered.");
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”direction”, out var direction) || !TryGetNumber(payload, ”amount”, out var
        // amount)`; bagian berikut berada di luar batas blok tersebut dalam DescribeRiskLife.
        }

        // Menyiapkan variabel lokal `directionText` untuk nilai direction text dengan hasil pemilihan bersyarat: ketika `direction.Equals(”IN”,
        // StringComparison.OrdinalIgnoreCase)` benar gunakan `L(language, ”dampak positif”, ”positive impact”)`, jika tidak gunakan
        // `direction.Equals(”OUT”, StringComparison.OrdinalIgnoreCase) ? L(language, ”dampak negatif”, ”negative impact”) : direction.ToUpperInvariant()`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var directionText = direction.Equals("IN", StringComparison.OrdinalIgnoreCase)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: L(language, ”dampak positif”, ”positive impact”) dalam
            // DescribeRiskLife.
            ? L(language, "dampak positif", "positive impact")
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: direction.Equals(”OUT”, StringComparison.OrdinalIgnoreCase) dalam
            // DescribeRiskLife.
            : direction.Equals("OUT", StringComparison.OrdinalIgnoreCase)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: L(language, ”dampak negatif”, ”negative impact”) dalam
                // DescribeRiskLife.
                ? L(language, "dampak negatif", "negative impact")
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: direction.ToUpperInvariant(); dalam DescribeRiskLife.
                : direction.ToUpperInvariant();

        // Mengembalikan memanggil `L` dengan `language`, `$”Kartu risiko {riskId} aktif dengan {directionText} sebesar {FormatNumber(amount)}.”`, `$”Risk
        // card {riskId} triggered with {directionText} of {FormatNumber(amount)}.”` kepada pemanggil dalam DescribeRiskLife; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Kartu risiko {riskId} aktif dengan {directionText} sebesar {FormatNumber(amount)}.”`; nilai ekspresi di dalam
            // kurung kurawal disisipkan saat program berjalan sebagai argumen ke `L`; Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam
            // operasi) sebagai argumen ke `FormatNumber`.
            $"Kartu risiko {riskId} aktif dengan {directionText} sebesar {FormatNumber(amount)}.",
            // Meneruskan teks interpolasi `$”Risk card {riskId} triggered with {directionText} of {FormatNumber(amount)}.”`; nilai ekspresi di dalam kurung
            // kurawal disisipkan saat program berjalan sebagai argumen ke `L`; Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam
            // operasi) sebagai argumen ke `FormatNumber`.
            $"Risk card {riskId} triggered with {directionText} of {FormatNumber(amount)}.");
    // Menutup scope metode DescribeRiskLife; bagian berikut berada di luar batas blok tersebut dalam DescribeRiskLife.
    }

    /// <summary>
    /// Mendeskripsikan event penggunaan opsi darurat akibat risiko,
    /// termasuk jenis opsi, arah dampak, dan nominal.
    /// </summary>
    /// <param name="payload">Data payload JSON event opsi darurat.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi penggunaan opsi darurat dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeRiskEmergency` dengan hasil bertipe `string`. Mendeskripsikan event penggunaan opsi darurat akibat risiko,
    // termasuk jenis opsi, arah dampak, dan nominal. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON;
    // Parameter `language` bertipe `string` membawa nilai language.
    private static string DescribeRiskEmergency(JsonElement payload, string language)
    // Membuka scope metode DescribeRiskEmergency; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeRiskEmergency.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(payload, ”option_type”, out var optionType)` dan
        // `!TryGetNumber(payload, ”amount”, out var amount)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam DescribeRiskEmergency.
        if (!TryGetString(payload, "option_type", out var optionType) ||
            // Menggunakan kebalikan kondisi `TryGetNumber(payload, ”amount”, out var amount)` sebagai bagian ekspresi yang sedang disusun dalam
            // DescribeRiskEmergency.
            !TryGetNumber(payload, "amount", out var amount))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”option_type”, out var optionType) || !TryGetNumber(payload, ”amount”, out var
        // amount)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeRiskEmergency.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”GunakanOpsiDarurat”`, `payload`, `language` kepada pemanggil dalam
            // DescribeRiskEmergency; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("GunakanOpsiDarurat", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”option_type”, out var optionType) || !TryGetNumber(payload, ”amount”, out var
        // amount)`; bagian berikut berada di luar batas blok tersebut dalam DescribeRiskEmergency.
        }

        // Menyiapkan variabel lokal `directionText` untuk nilai direction text dengan memanggil `L` dengan `language`, `”menambah saldo”`, `”adds
        // balance”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var directionText = L(language, "menambah saldo", "adds balance");

        // Mengembalikan memanggil `L` dengan `language`, `$”Menggunakan opsi darurat {optionType} ({directionText} {FormatNumber(amount)}).”`, `$”Used
        // emergency option {optionType} ({directionText} {FormatNumber(amount)}).”` kepada pemanggil dalam DescribeRiskEmergency; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Menggunakan opsi darurat {optionType} ({directionText} {FormatNumber(amount)}).”`; nilai ekspresi di dalam kurung
            // kurawal disisipkan saat program berjalan sebagai argumen ke `L`; Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam
            // operasi) sebagai argumen ke `FormatNumber`.
            $"Menggunakan opsi darurat {optionType} ({directionText} {FormatNumber(amount)}).",
            // Meneruskan teks interpolasi `$”Used emergency option {optionType} ({directionText} {FormatNumber(amount)}).”`; nilai ekspresi di dalam kurung
            // kurawal disisipkan saat program berjalan sebagai argumen ke `L`; Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam
            // operasi) sebagai argumen ke `FormatNumber`.
            $"Used emergency option {optionType} ({directionText} {FormatNumber(amount)}).");
    // Menutup scope metode DescribeRiskEmergency; bagian berikut berada di luar batas blok tersebut dalam DescribeRiskEmergency.
    }

    /// <summary>
    /// Mendeskripsikan event pembelian asuransi multirisk, termasuk nominal premi.
    /// </summary>
    /// <param name="payload">Data payload JSON event pembelian asuransi.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi pembelian asuransi dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeInsurancePurchase` dengan hasil bertipe `string`. Mendeskripsikan event pembelian asuransi multirisk, termasuk
    // nominal premi. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe
    // `string` membawa nilai language.
    private static string DescribeInsurancePurchase(JsonElement payload, string language)
    // Membuka scope metode DescribeInsurancePurchase; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeInsurancePurchase.
    {
        // Memeriksa kebalikan kondisi `TryGetNumber(payload, ”premium”, out var premium)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DescribeInsurancePurchase.
        if (!TryGetNumber(payload, "premium", out var premium))
        // Membuka scope cabang if untuk kondisi `!TryGetNumber(payload, ”premium”, out var premium)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam DescribeInsurancePurchase.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”Asuransi”`, `payload`, `language` kepada pemanggil dalam DescribeInsurancePurchase;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("Asuransi", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetNumber(payload, ”premium”, out var premium)`; bagian berikut berada di luar batas blok tersebut
        // dalam DescribeInsurancePurchase.
        }

        // Mengembalikan memanggil `L` dengan `language`, `$”Membeli asuransi multirisk dengan premi {FormatNumber(premium)}.”`, `$”Purchased multirisk
        // insurance with premium {FormatNumber(premium)}.”` kepada pemanggil dalam DescribeInsurancePurchase; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Membeli asuransi multirisk dengan premi {FormatNumber(premium)}.”`; nilai ekspresi di dalam kurung kurawal
            // disisipkan saat program berjalan sebagai argumen ke `L`; Meneruskan `premium` (nilai premium) sebagai argumen ke `FormatNumber`.
            $"Membeli asuransi multirisk dengan premi {FormatNumber(premium)}.",
            // Meneruskan teks interpolasi `$”Purchased multirisk insurance with premium {FormatNumber(premium)}.”`; nilai ekspresi di dalam kurung kurawal
            // disisipkan saat program berjalan sebagai argumen ke `L`; Meneruskan `premium` (nilai premium) sebagai argumen ke `FormatNumber`.
            $"Purchased multirisk insurance with premium {FormatNumber(premium)}.");
    // Menutup scope metode DescribeInsurancePurchase; bagian berikut berada di luar batas blok tersebut dalam DescribeInsurancePurchase.
    }

    /// <summary>
    /// Mendeskripsikan event penggunaan klaim asuransi multirisk untuk event risiko tertentu.
    /// </summary>
    /// <param name="payload">Data payload JSON event klaim asuransi.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi penggunaan asuransi dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeInsuranceUse` dengan hasil bertipe `string`. Mendeskripsikan event penggunaan klaim asuransi multirisk untuk event
    // risiko tertentu. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe
    // `string` membawa nilai language.
    private static string DescribeInsuranceUse(JsonElement payload, string language)
    // Membuka scope metode DescribeInsuranceUse; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeInsuranceUse.
    {
        // Memeriksa kebalikan kondisi `TryGetString(payload, ”risk_event_id”, out var riskEventId)`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam DescribeInsuranceUse.
        if (!TryGetString(payload, "risk_event_id", out var riskEventId))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”risk_event_id”, out var riskEventId)`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam DescribeInsuranceUse.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”Asuransi”`, `payload`, `language` kepada pemanggil dalam DescribeInsuranceUse;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("Asuransi", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”risk_event_id”, out var riskEventId)`; bagian berikut berada di luar batas blok
        // tersebut dalam DescribeInsuranceUse.
        }

        // Mengembalikan memanggil `L` dengan `language`, `$”Mengaktifkan perlindungan asuransi untuk event risiko {riskEventId}.”`, `$”Activated insurance
        // protection for risk event {riskEventId}.”` kepada pemanggil dalam DescribeInsuranceUse; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Mengaktifkan perlindungan asuransi untuk event risiko {riskEventId}.”`; nilai ekspresi di dalam kurung kurawal
            // disisipkan saat program berjalan sebagai argumen ke `L`.
            $"Mengaktifkan perlindungan asuransi untuk event risiko {riskEventId}.",
            // Meneruskan teks interpolasi `$”Activated insurance protection for risk event {riskEventId}.”`; nilai ekspresi di dalam kurung kurawal disisipkan
            // saat program berjalan sebagai argumen ke `L`.
            $"Activated insurance protection for risk event {riskEventId}.");
    // Menutup scope metode DescribeInsuranceUse; bagian berikut berada di luar batas blok tersebut dalam DescribeInsuranceUse.
    }

    // Mendefinisikan metode `IsInsuranceUse` dengan hasil bertipe `bool`; operasi ini menangani berstatus asuransi use. Masukan: Parameter `payload`
    // bertipe `JsonElement` membawa muatan detail event dalam format JSON.
    private static bool IsInsuranceUse(JsonElement payload)
    // Membuka scope metode IsInsuranceUse; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IsInsuranceUse.
    {
        // Mengembalikan memanggil `TryGetString` dengan `payload`, `”risk_event_id”`, `_` kepada pemanggil dalam IsInsuranceUse; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return TryGetString(payload, "risk_event_id", out _);
    // Menutup scope metode IsInsuranceUse; bagian berikut berada di luar batas blok tersebut dalam IsInsuranceUse.
    }

    /// <summary>
    /// Mendeskripsikan event pemberian peringkat (donasi/pensiun), termasuk peringkat dan poin.
    /// </summary>
    /// <param name="payload">Data payload JSON event peringkat.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <param name="topicId">Label topik peringkat dalam bahasa Indonesia.</param>
    /// <param name="topicEn">Label topik peringkat dalam bahasa Inggris.</param>
    /// <returns>Deskripsi pemberian peringkat dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeRankAward` dengan hasil bertipe `string`. Mendeskripsikan event pemberian peringkat (donasi/pensiun), termasuk
    // peringkat dan poin. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language`
    // bertipe `string` membawa nilai language; Parameter `topicId` bertipe `string` membawa nilai topic identitas; Parameter `topicEn` bertipe `string`
    // membawa nilai topic en.
    private static string DescribeRankAward(JsonElement payload, string language, string topicId, string topicEn)
    // Membuka scope metode DescribeRankAward; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeRankAward.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetInt(payload, ”rank”, out var rank)` dan `!TryGetNumber(payload,
        // ”points”, out var points)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DescribeRankAward.
        if (!TryGetInt(payload, "rank", out var rank) || !TryGetNumber(payload, "points", out var points))
        // Membuka scope cabang if untuk kondisi `!TryGetInt(payload, ”rank”, out var rank) || !TryGetNumber(payload, ”points”, out var points)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeRankAward.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”rank.awarded”`, `payload`, `language` kepada pemanggil dalam DescribeRankAward;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("rank.awarded", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetInt(payload, ”rank”, out var rank) || !TryGetNumber(payload, ”points”, out var points)`; bagian
        // berikut berada di luar batas blok tersebut dalam DescribeRankAward.
        }

        // Mengembalikan memanggil `L` dengan `language`, `$”Mendapat peringkat {rank} pada kategori {topicId} (poin kebahagiaan {FormatNumber(points)}).”`,
        // `$”Received rank {rank} in {topicEn} category (happiness points {FormatNumber(points)}).”` kepada pemanggil dalam DescribeRankAward; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Mendapat peringkat {rank} pada kategori {topicId} (poin kebahagiaan {FormatNumber(points)}).”`; nilai ekspresi di
            // dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `L`; Meneruskan `points` (nilai poin) sebagai argumen ke `FormatNumber`.
            $"Mendapat peringkat {rank} pada kategori {topicId} (poin kebahagiaan {FormatNumber(points)}).",
            // Meneruskan teks interpolasi `$”Received rank {rank} in {topicEn} category (happiness points {FormatNumber(points)}).”`; nilai ekspresi di dalam
            // kurung kurawal disisipkan saat program berjalan sebagai argumen ke `L`; Meneruskan `points` (nilai poin) sebagai argumen ke `FormatNumber`.
            $"Received rank {rank} in {topicEn} category (happiness points {FormatNumber(points)}).");
    // Menutup scope metode DescribeRankAward; bagian berikut berada di luar batas blok tersebut dalam DescribeRankAward.
    }

    // Mendefinisikan metode `DescribeDonationWinnersAnnouncement` dengan hasil bertipe `string`; operasi ini menangani describe donasi winners
    // announcement. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe
    // `string` membawa nilai language.
    private static string DescribeDonationWinnersAnnouncement(JsonElement payload, string language)
    // Membuka scope metode DescribeDonationWinnersAnnouncement; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DescribeDonationWinnersAnnouncement.
    {
        // Menyiapkan variabel lokal `summary` untuk nilai summary dengan hasil pemilihan bersyarat: ketika `TryGetString(payload, ”summary”, out var
        // summaryText)` benar gunakan `summaryText.Trim().TrimEnd('.')`, jika tidak gunakan `BuildDonationWinnerSummary(payload, language)`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var summary = TryGetString(payload, "summary", out var summaryText)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: summaryText.Trim().TrimEnd('.') dalam
            // DescribeDonationWinnersAnnouncement.
            ? summaryText.Trim().TrimEnd('.')
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: BuildDonationWinnerSummary(payload, language); dalam
            // DescribeDonationWinnersAnnouncement.
            : BuildDonationWinnerSummary(payload, language);

        // Memeriksa memeriksa apakah `summary` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam DescribeDonationWinnersAnnouncement.
        if (string.IsNullOrWhiteSpace(summary))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(summary)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // DescribeDonationWinnersAnnouncement.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”UmumkanJuaraDonasi”`, `payload`, `language` kepada pemanggil dalam
            // DescribeDonationWinnersAnnouncement; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("UmumkanJuaraDonasi", payload, language);
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(summary)`; bagian berikut berada di luar batas blok tersebut dalam
        // DescribeDonationWinnersAnnouncement.
        }

        // Mengembalikan memanggil `L` dengan `language`, `$”Sistem menentukan Juara Donasi: {summary}.”`, `$”System determined Donation Winners:
        // {summary}.”` kepada pemanggil dalam DescribeDonationWinnersAnnouncement; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Sistem menentukan Juara Donasi: {summary}.”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `L`.
            $"Sistem menentukan Juara Donasi: {summary}.",
            // Meneruskan teks interpolasi `$”System determined Donation Winners: {summary}.”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `L`.
            $"System determined Donation Winners: {summary}.");
    // Menutup scope metode DescribeDonationWinnersAnnouncement; bagian berikut berada di luar batas blok tersebut dalam
    // DescribeDonationWinnersAnnouncement.
    }

    // Mendefinisikan metode `BuildDonationWinnerSummary` dengan hasil bertipe `string`; operasi ini menangani build donasi winner summary. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe `string` membawa nilai
    // language.
    private static string BuildDonationWinnerSummary(JsonElement payload, string language)
    // Membuka scope metode BuildDonationWinnerSummary; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildDonationWinnerSummary.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `payload.ValueKind != JsonValueKind.Object ||
        // !payload.TryGetProperty(”winners”, out var winners)` dan `winners.ValueKind != JsonValueKind.Array`; sisi kanan diperiksa hanya jika sisi kiri
        // salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildDonationWinnerSummary.
        if (payload.ValueKind != JsonValueKind.Object ||
            // Menggunakan kebalikan kondisi `payload.TryGetProperty(”winners”, out var winners)` sebagai bagian ekspresi yang sedang disusun dalam
            // BuildDonationWinnerSummary.
            !payload.TryGetProperty("winners", out var winners) ||
            // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `winners.ValueKind` dan `JsonValueKind.Array` dalam BuildDonationWinnerSummary.
            winners.ValueKind != JsonValueKind.Array)
        // Membuka scope cabang if untuk kondisi `payload.ValueKind != JsonValueKind.Object || !payload.TryGetProperty(”winners”, out var winners) ||
        // winners.ValueKind != JsonValueKind.Array`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildDonationWinnerSummary.
        {
            // Mengembalikan `string.Empty`, yaitu nilai kosong bawaan tipe terkait kepada pemanggil dalam BuildDonationWinnerSummary; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return string.Empty;
        // Menutup scope cabang if untuk kondisi `payload.ValueKind != JsonValueKind.Object || !payload.TryGetProperty(”winners”, out var winners) ||
        // winners.ValueKind != JsonValueKind.Array`; bagian berikut berada di luar batas blok tersebut dalam BuildDonationWinnerSummary.
        }

        // Menyiapkan variabel lokal `parts` untuk nilai parts dengan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var parts = new List<string>();
        // Mengulangi setiap elemen `winners.EnumerateArray()`; elemen saat ini disimpan sebagai `winner` bertipe `var` untuk diproses oleh badan loop dalam
        // BuildDonationWinnerSummary.
        foreach (var winner in winners.EnumerateArray())
        // Membuka scope loop setiap winner dari `winners.EnumerateArray()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildDonationWinnerSummary.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(winner, ”player_name”, out var playerName)` dan
            // `!TryGetInt(winner, ”rank”, out var rank)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam BuildDonationWinnerSummary.
            if (!TryGetString(winner, "player_name", out var playerName) ||
                // Menggunakan kebalikan kondisi `TryGetInt(winner, ”rank”, out var rank)` sebagai bagian ekspresi yang sedang disusun dalam
                // BuildDonationWinnerSummary.
                !TryGetInt(winner, "rank", out var rank))
            // Membuka scope cabang if untuk kondisi `!TryGetString(winner, ”player_name”, out var playerName) || !TryGetInt(winner, ”rank”, out var rank)`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildDonationWinnerSummary.
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildDonationWinnerSummary.
                continue;
            // Menutup scope cabang if untuk kondisi `!TryGetString(winner, ”player_name”, out var playerName) || !TryGetInt(winner, ”rank”, out var rank)`;
            // bagian berikut berada di luar batas blok tersebut dalam BuildDonationWinnerSummary.
            }

            // Menjalankan menambahkan `L(language, $”{playerName} Juara {rank}”, $”{playerName} Rank {rank}”)` ke `parts` dalam BuildDonationWinnerSummary.
            parts.Add(L(language, $"{playerName} Juara {rank}", $"{playerName} Rank {rank}"));
        // Menutup scope loop setiap winner dari `winners.EnumerateArray()`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildDonationWinnerSummary.
        }

        // Mengembalikan memanggil `string.Join` dengan `”, ”`, `parts` kepada pemanggil dalam BuildDonationWinnerSummary; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return string.Join(", ", parts);
    // Menutup scope metode BuildDonationWinnerSummary; bagian berikut berada di luar batas blok tersebut dalam BuildDonationWinnerSummary.
    }

    /// <summary>
    /// Mendeskripsikan event pemberian bonus poin (misalnya poin emas), termasuk jumlah poin.
    /// </summary>
    /// <param name="payload">Data payload JSON event bonus poin.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <param name="topicId">Label topik poin dalam bahasa Indonesia.</param>
    /// <param name="topicEn">Label topik poin dalam bahasa Inggris.</param>
    /// <returns>Deskripsi bonus poin dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribePointsAward` dengan hasil bertipe `string`. Mendeskripsikan event pemberian bonus poin (misalnya poin emas),
    // termasuk jumlah poin. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language`
    // bertipe `string` membawa nilai language; Parameter `topicId` bertipe `string` membawa nilai topic identitas; Parameter `topicEn` bertipe `string`
    // membawa nilai topic en.
    private static string DescribePointsAward(JsonElement payload, string language, string topicId, string topicEn)
    // Membuka scope metode DescribePointsAward; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribePointsAward.
    {
        // Memeriksa kebalikan kondisi `TryGetNumber(payload, ”points”, out var points)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // DescribePointsAward.
        if (!TryGetNumber(payload, "points", out var points))
        // Membuka scope cabang if untuk kondisi `!TryGetNumber(payload, ”points”, out var points)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam DescribePointsAward.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”points.awarded”`, `payload`, `language` kepada pemanggil dalam DescribePointsAward;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("points.awarded", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetNumber(payload, ”points”, out var points)`; bagian berikut berada di luar batas blok tersebut dalam
        // DescribePointsAward.
        }

        // Mengembalikan memanggil `L` dengan `language`, `$”Mendapat bonus poin kebahagiaan {topicId} sebesar {FormatNumber(points)}.”`, `$”Received
        // {topicEn} bonus happiness points of {FormatNumber(points)}.”` kepada pemanggil dalam DescribePointsAward; eksekusi jalur ini selesai setelah
        // nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Mendapat bonus poin kebahagiaan {topicId} sebesar {FormatNumber(points)}.”`; nilai ekspresi di dalam kurung
            // kurawal disisipkan saat program berjalan sebagai argumen ke `L`; Meneruskan `points` (nilai poin) sebagai argumen ke `FormatNumber`.
            $"Mendapat bonus poin kebahagiaan {topicId} sebesar {FormatNumber(points)}.",
            // Meneruskan teks interpolasi `$”Received {topicEn} bonus happiness points of {FormatNumber(points)}.”`; nilai ekspresi di dalam kurung kurawal
            // disisipkan saat program berjalan sebagai argumen ke `L`; Meneruskan `points` (nilai poin) sebagai argumen ke `FormatNumber`.
            $"Received {topicEn} bonus happiness points of {FormatNumber(points)}.");
    // Menutup scope metode DescribePointsAward; bagian berikut berada di luar batas blok tersebut dalam DescribePointsAward.
    }

    /// <summary>
    /// Mendeskripsikan event penggunaan aksi turn, termasuk jumlah aksi terpakai dan sisa.
    /// </summary>
    /// <param name="payload">Data payload JSON event aksi turn.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi penggunaan aksi turn dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `DescribeTurnAction` dengan hasil bertipe `string`. Mendeskripsikan event penggunaan aksi turn, termasuk jumlah aksi
    // terpakai dan sisa. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe
    // `string` membawa nilai language.
    private static string DescribeTurnAction(JsonElement payload, string language)
    // Membuka scope metode DescribeTurnAction; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeTurnAction.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetInt(payload, ”used”, out var used)` dan `!TryGetInt(payload,
        // ”remaining”, out var remaining)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam DescribeTurnAction.
        if (!TryGetInt(payload, "used", out var used) || !TryGetInt(payload, "remaining", out var remaining))
        // Membuka scope cabang if untuk kondisi `!TryGetInt(payload, ”used”, out var used) || !TryGetInt(payload, ”remaining”, out var remaining)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DescribeTurnAction.
        {
            // Mengembalikan memanggil `BuildGenericDescription` dengan `”AkhirGiliran”`, `payload`, `language` kepada pemanggil dalam DescribeTurnAction;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return BuildGenericDescription("AkhirGiliran", payload, language);
        // Menutup scope cabang if untuk kondisi `!TryGetInt(payload, ”used”, out var used) || !TryGetInt(payload, ”remaining”, out var remaining)`; bagian
        // berikut berada di luar batas blok tersebut dalam DescribeTurnAction.
        }

        // Mengembalikan memanggil `L` dengan `language`, `$”Menggunakan {used} aksi, sisa aksi turn ini {remaining}.”`, `$”Used {used} action(s), remaining
        // actions this turn: {remaining}.”` kepada pemanggil dalam DescribeTurnAction; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Menggunakan {used} aksi, sisa aksi turn ini {remaining}.”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `L`.
            $"Menggunakan {used} aksi, sisa aksi turn ini {remaining}.",
            // Meneruskan teks interpolasi `$”Used {used} action(s), remaining actions this turn: {remaining}.”`; nilai ekspresi di dalam kurung kurawal
            // disisipkan saat program berjalan sebagai argumen ke `L`.
            $"Used {used} action(s), remaining actions this turn: {remaining}.");
    // Menutup scope metode DescribeTurnAction; bagian berikut berada di luar batas blok tersebut dalam DescribeTurnAction.
    }

    /// <summary>
    /// Membangun deskripsi generik untuk actionType yang tidak memiliki handler spesifik,
    /// dengan menyertakan ringkasan field utama dari payload.
    /// </summary>
    /// <param name="actionType">Tipe aksi event.</param>
    /// <param name="payload">Data payload JSON event.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Deskripsi generik event dalam bahasa yang sesuai.</returns>
    // Mendefinisikan metode `BuildGenericDescription` dengan hasil bertipe `string`. Membangun deskripsi generik untuk actionType yang tidak memiliki
    // handler spesifik, dengan menyertakan ringkasan field utama dari payload. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi
    // jenis; Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `language` bertipe `string` membawa
    // nilai language.
    private static string BuildGenericDescription(string actionType, JsonElement payload, string language)
    // Membuka scope metode BuildGenericDescription; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildGenericDescription.
    {
        // Menyiapkan variabel lokal `actionLabel` untuk nilai aksi label dengan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(actionType)`
        // benar gunakan `L(language, ”aktivitas”, ”activity”)`, jika tidak gunakan `actionType`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actionLabel = string.IsNullOrWhiteSpace(actionType) ? L(language, "aktivitas", "activity") : actionType;
        // Menyiapkan variabel lokal `baseText` untuk nilai base text dengan memanggil `L` dengan `language`, `$”Aksi {actionLabel} dieksekusi pada sesi.”`,
        // `$”Action {actionLabel} was executed in this session.”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var baseText = L(
            // Meneruskan `language` (nilai language) sebagai argumen ke `L`.
            language,
            // Meneruskan teks interpolasi `$”Aksi {actionLabel} dieksekusi pada sesi.”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program
            // berjalan sebagai argumen ke `L`.
            $"Aksi {actionLabel} dieksekusi pada sesi.",
            // Meneruskan teks interpolasi `$”Action {actionLabel} was executed in this session.”`; nilai ekspresi di dalam kurung kurawal disisipkan saat
            // program berjalan sebagai argumen ke `L`.
            $"Action {actionLabel} was executed in this session.");
        // Menyiapkan variabel lokal `payloadSummary` untuk nilai payload summary dengan memanggil `BuildPayloadSummary` dengan `payload`, `language`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var payloadSummary = BuildPayloadSummary(payload, language);
        // Memeriksa memeriksa apakah `payloadSummary` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam BuildGenericDescription.
        if (string.IsNullOrWhiteSpace(payloadSummary))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(payloadSummary)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam BuildGenericDescription.
        {
            // Mengembalikan `baseText` (nilai base text) kepada pemanggil dalam BuildGenericDescription; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return baseText;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(payloadSummary)`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildGenericDescription.
        }

        // Mengembalikan memanggil `L` dengan `language`, `$”{baseText} Detail: {payloadSummary}”`, `$”{baseText} Details: {payloadSummary}”` kepada
        // pemanggil dalam BuildGenericDescription; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return L(language, $"{baseText} Detail: {payloadSummary}", $"{baseText} Details: {payloadSummary}");
    // Menutup scope metode BuildGenericDescription; bagian berikut berada di luar batas blok tersebut dalam BuildGenericDescription.
    }

    /// <summary>
    /// Merangkum hingga 5 field utama dari payload JSON menjadi string ringkas
    /// dengan label yang diterjemahkan sesuai bahasa aktif.
    /// </summary>
    /// <param name="payload">Data payload JSON event.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>String ringkasan payload, atau kosong jika payload bukan objek.</returns>
    // Mendefinisikan metode `BuildPayloadSummary` dengan hasil bertipe `string`. Merangkum hingga 5 field utama dari payload JSON menjadi string
    // ringkas dengan label yang diterjemahkan sesuai bahasa aktif. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam
    // format JSON; Parameter `language` bertipe `string` membawa nilai language.
    private static string BuildPayloadSummary(JsonElement payload, string language)
    // Membuka scope metode BuildPayloadSummary; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildPayloadSummary.
    {
        // Memeriksa perbandingan ketidaksamaan antara `payload.ValueKind` dan `JsonValueKind.Object`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam BuildPayloadSummary.
        if (payload.ValueKind != JsonValueKind.Object)
        // Membuka scope cabang if untuk kondisi `payload.ValueKind != JsonValueKind.Object`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam BuildPayloadSummary.
        {
            // Mengembalikan `string.Empty`, yaitu nilai kosong bawaan tipe terkait kepada pemanggil dalam BuildPayloadSummary; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return string.Empty;
        // Menutup scope cabang if untuk kondisi `payload.ValueKind != JsonValueKind.Object`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildPayloadSummary.
        }

        // Menyiapkan variabel lokal `keyOrder` untuk nilai kunci urutan/pesanan dengan array baru dengan tipe elemen disimpulkan dari nilai initializer.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var keyOrder = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildPayloadSummary.
        {
            // Menggunakan nilai literal `”amount”` sebagai bagian ekspresi yang sedang disusun dalam BuildPayloadSummary.
            "amount",
            // Menggunakan nilai literal `”direction”` sebagai bagian ekspresi yang sedang disusun dalam BuildPayloadSummary.
            "direction",
            // Menggunakan nilai literal `”category”` sebagai bagian ekspresi yang sedang disusun dalam BuildPayloadSummary.
            "category",
            // Menggunakan nilai literal `”trade_type”` sebagai bagian ekspresi yang sedang disusun dalam BuildPayloadSummary.
            "trade_type",
            // Menggunakan nilai literal `”qty”` sebagai bagian ekspresi yang sedang disusun dalam BuildPayloadSummary.
            "qty",
            // Menggunakan nilai literal `”unit_price”` sebagai bagian ekspresi yang sedang disusun dalam BuildPayloadSummary.
            "unit_price",
            // Menggunakan nilai literal `”card_id”` sebagai bagian ekspresi yang sedang disusun dalam BuildPayloadSummary.
            "card_id",
            // Menggunakan nilai literal `”goal_id”` sebagai bagian ekspresi yang sedang disusun dalam BuildPayloadSummary.
            "goal_id",
            // Menggunakan nilai literal `”loan_id”` sebagai bagian ekspresi yang sedang disusun dalam BuildPayloadSummary.
            "loan_id",
            // Menggunakan nilai literal `”risk_id”` sebagai bagian ekspresi yang sedang disusun dalam BuildPayloadSummary.
            "risk_id",
            // Menggunakan nilai literal `”option_type”` sebagai bagian ekspresi yang sedang disusun dalam BuildPayloadSummary.
            "option_type",
            // Menggunakan nilai literal `”premium”` sebagai bagian ekspresi yang sedang disusun dalam BuildPayloadSummary.
            "premium",
            // Menggunakan nilai literal `”rank”` sebagai bagian ekspresi yang sedang disusun dalam BuildPayloadSummary.
            "rank",
            // Menggunakan nilai literal `”points”` sebagai bagian ekspresi yang sedang disusun dalam BuildPayloadSummary.
            "points"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildPayloadSummary.
        };

        // Menyiapkan variabel lokal `parts` untuk nilai parts dengan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var parts = new List<string>();
        // Mengulangi setiap elemen `keyOrder`; elemen saat ini disimpan sebagai `key` bertipe `var` untuk diproses oleh badan loop dalam
        // BuildPayloadSummary.
        foreach (var key in keyOrder)
        // Membuka scope loop setiap key dari `keyOrder`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildPayloadSummary.
        {
            // Memeriksa kebalikan kondisi `payload.TryGetProperty(key, out var value)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // BuildPayloadSummary.
            if (!payload.TryGetProperty(key, out var value))
            // Membuka scope cabang if untuk kondisi `!payload.TryGetProperty(key, out var value)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
            // dalam BuildPayloadSummary.
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam BuildPayloadSummary.
                continue;
            // Menutup scope cabang if untuk kondisi `!payload.TryGetProperty(key, out var value)`; bagian berikut berada di luar batas blok tersebut dalam
            // BuildPayloadSummary.
            }

            // Menjalankan menambahkan `$”{ResolvePayloadKeyLabel(key, language)}: {JsonElementToInlineText(value, key, language)}”` ke `parts` dalam
            // BuildPayloadSummary.
            parts.Add($"{ResolvePayloadKeyLabel(key, language)}: {JsonElementToInlineText(value, key, language)}");
            // Memeriksa perbandingan kesamaan antara `parts.Count` dan `5`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // BuildPayloadSummary.
            if (parts.Count == 5)
            // Membuka scope cabang if untuk kondisi `parts.Count == 5`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildPayloadSummary.
            {
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam BuildPayloadSummary.
                break;
            // Menutup scope cabang if untuk kondisi `parts.Count == 5`; bagian berikut berada di luar batas blok tersebut dalam BuildPayloadSummary.
            }
        // Menutup scope loop setiap key dari `keyOrder`; bagian berikut berada di luar batas blok tersebut dalam BuildPayloadSummary.
        }

        // Mengembalikan memanggil `string.Join` dengan `”, ”`, `parts` kepada pemanggil dalam BuildPayloadSummary; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return string.Join(", ", parts);
    // Menutup scope metode BuildPayloadSummary; bagian berikut berada di luar batas blok tersebut dalam BuildPayloadSummary.
    }

    /// <summary>
    /// Menerjemahkan nama field payload JSON (misalnya "amount", "direction")
    /// menjadi label yang mudah dibaca dalam bahasa yang sesuai.
    /// </summary>
    /// <param name="key">Nama field payload.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Label field yang diterjemahkan.</returns>
    // Mendefinisikan metode `ResolvePayloadKeyLabel` dengan hasil bertipe `string`. Menerjemahkan nama field payload JSON (misalnya ”amount”,
    // ”direction”) menjadi label yang mudah dibaca dalam bahasa yang sesuai. Masukan: Parameter `key` bertipe `string` membawa nilai kunci; Parameter
    // `language` bertipe `string` membawa nilai language.
    private static string ResolvePayloadKeyLabel(string key, string language)
    // Membuka scope metode ResolvePayloadKeyLabel; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolvePayloadKeyLabel.
    {
        // Mengembalikan hasil pemetaan `key` melalui cabang pola switch yang cocok kepada pemanggil dalam ResolvePayloadKeyLabel; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return key switch
        // Membuka scope pemetaan switch atas `key`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolvePayloadKeyLabel.
        {
            // Untuk pola `”amount”`, menghasilkan memanggil `L` dengan `language`, `”nominal”`, `”amount”` sebagai hasil switch.
            "amount" => L(language, "nominal", "amount"),
            // Untuk pola `”direction”`, menghasilkan memanggil `L` dengan `language`, `”arah”`, `”direction”` sebagai hasil switch.
            "direction" => L(language, "arah", "direction"),
            // Untuk pola `”category”`, menghasilkan memanggil `L` dengan `language`, `”kategori”`, `”category”` sebagai hasil switch.
            "category" => L(language, "kategori", "category"),
            // Untuk pola `”trade_type”`, menghasilkan memanggil `L` dengan `language`, `”jenis transaksi”`, `”trade type”` sebagai hasil switch.
            "trade_type" => L(language, "jenis transaksi", "trade type"),
            // Untuk pola `”qty”`, menghasilkan memanggil `L` dengan `language`, `”jumlah”`, `”qty”` sebagai hasil switch.
            "qty" => L(language, "jumlah", "qty"),
            // Untuk pola `”unit_price”`, menghasilkan memanggil `L` dengan `language`, `”harga per unit”`, `”unit price”` sebagai hasil switch.
            "unit_price" => L(language, "harga per unit", "unit price"),
            // Untuk pola `”card_id”`, menghasilkan memanggil `L` dengan `language`, `”kartu”`, `”card id”` sebagai hasil switch.
            "card_id" => L(language, "kartu", "card id"),
            // Untuk pola `”goal_id”`, menghasilkan memanggil `L` dengan `language`, `”tujuan tabungan”`, `”saving goal”` sebagai hasil switch.
            "goal_id" => L(language, "tujuan tabungan", "saving goal"),
            // Untuk pola `”loan_id”`, menghasilkan memanggil `L` dengan `language`, `”pinjaman”`, `”loan id”` sebagai hasil switch.
            "loan_id" => L(language, "pinjaman", "loan id"),
            // Untuk pola `”risk_id”`, menghasilkan memanggil `L` dengan `language`, `”risiko”`, `”risk id”` sebagai hasil switch.
            "risk_id" => L(language, "risiko", "risk id"),
            // Untuk pola `”option_type”`, menghasilkan memanggil `L` dengan `language`, `”opsi”`, `”option”` sebagai hasil switch.
            "option_type" => L(language, "opsi", "option"),
            // Untuk pola `”premium”`, menghasilkan memanggil `L` dengan `language`, `”premi”`, `”premium”` sebagai hasil switch.
            "premium" => L(language, "premi", "premium"),
            // Untuk pola `”rank”`, menghasilkan memanggil `L` dengan `language`, `”peringkat”`, `”rank”` sebagai hasil switch.
            "rank" => L(language, "peringkat", "rank"),
            // Untuk pola `”points”`, menghasilkan memanggil `L` dengan `language`, `”poin kebahagiaan”`, `”happiness points”` sebagai hasil switch.
            "points" => L(language, "poin kebahagiaan", "happiness points"),
            // Untuk pola `_`, menghasilkan `key` (nilai kunci) sebagai hasil switch.
            _ => key
        // Menutup scope pemetaan switch atas `key`; bagian berikut berada di luar batas blok tersebut dalam ResolvePayloadKeyLabel.
        };
    // Menutup scope metode ResolvePayloadKeyLabel; bagian berikut berada di luar batas blok tersebut dalam ResolvePayloadKeyLabel.
    }

    /// <summary>
    /// Mengonversi nilai JsonElement menjadi teks inline yang mudah dibaca,
    /// dengan penanganan khusus untuk field "direction" dan "trade_type".
    /// </summary>
    /// <param name="value">Elemen JSON yang akan dikonversi.</param>
    /// <param name="key">Nama field asal untuk konteks penerjemahan.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Representasi teks inline dari nilai JSON.</returns>
    // Mendefinisikan metode `JsonElementToInlineText` dengan hasil bertipe `string`. Mengonversi nilai JsonElement menjadi teks inline yang mudah
    // dibaca, dengan penanganan khusus untuk field ”direction” dan ”trade_type”. Masukan: Parameter `value` bertipe `JsonElement` membawa nilai nilai;
    // Parameter `key` bertipe `string` membawa nilai kunci; Parameter `language` bertipe `string` membawa nilai language.
    private static string JsonElementToInlineText(JsonElement value, string key, string language)
    // Membuka scope metode JsonElementToInlineText; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam JsonElementToInlineText.
    {
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `key == ”direction”` dan `value.ValueKind == JsonValueKind.String`; sisi kanan
        // diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam JsonElementToInlineText.
        if (key == "direction" && value.ValueKind == JsonValueKind.String)
        // Membuka scope cabang if untuk kondisi `key == ”direction” && value.ValueKind == JsonValueKind.String`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam JsonElementToInlineText.
        {
            // Menyiapkan variabel lokal `direction` untuk nilai direction dengan `value.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai
            // nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var direction = value.GetString() ?? string.Empty;
            // Memeriksa membandingkan kesamaan `direction` dengan `”IN”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan
            // comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam JsonElementToInlineText.
            if (direction.Equals("IN", StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `direction.Equals(”IN”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam JsonElementToInlineText.
            {
                // Mengembalikan memanggil `L` dengan `language`, `”IN (masuk)”`, `”IN”` kepada pemanggil dalam JsonElementToInlineText; eksekusi jalur ini selesai
                // setelah nilai hasil ditentukan.
                return L(language, "IN (masuk)", "IN");
            // Menutup scope cabang if untuk kondisi `direction.Equals(”IN”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok
            // tersebut dalam JsonElementToInlineText.
            }

            // Memeriksa membandingkan kesamaan `direction` dengan `”OUT”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan
            // comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam JsonElementToInlineText.
            if (direction.Equals("OUT", StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `direction.Equals(”OUT”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam JsonElementToInlineText.
            {
                // Mengembalikan memanggil `L` dengan `language`, `”OUT (keluar)”`, `”OUT”` kepada pemanggil dalam JsonElementToInlineText; eksekusi jalur ini
                // selesai setelah nilai hasil ditentukan.
                return L(language, "OUT (keluar)", "OUT");
            // Menutup scope cabang if untuk kondisi `direction.Equals(”OUT”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok
            // tersebut dalam JsonElementToInlineText.
            }
        // Menutup scope cabang if untuk kondisi `key == ”direction” && value.ValueKind == JsonValueKind.String`; bagian berikut berada di luar batas blok
        // tersebut dalam JsonElementToInlineText.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `key == ”trade_type”` dan `value.ValueKind == JsonValueKind.String`; sisi kanan
        // diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam JsonElementToInlineText.
        if (key == "trade_type" && value.ValueKind == JsonValueKind.String)
        // Membuka scope cabang if untuk kondisi `key == ”trade_type” && value.ValueKind == JsonValueKind.String`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam JsonElementToInlineText.
        {
            // Menyiapkan variabel lokal `tradeType` untuk nilai trade jenis dengan `value.GetString()` bila tidak null; jika null gunakan `string.Empty`
            // sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var tradeType = value.GetString() ?? string.Empty;
            // Memeriksa membandingkan kesamaan `tradeType` dengan `”BUY”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan
            // comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam JsonElementToInlineText.
            if (tradeType.Equals("BUY", StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `tradeType.Equals(”BUY”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam JsonElementToInlineText.
            {
                // Mengembalikan memanggil `L` dengan `language`, `”BUY (beli)”`, `”BUY”` kepada pemanggil dalam JsonElementToInlineText; eksekusi jalur ini selesai
                // setelah nilai hasil ditentukan.
                return L(language, "BUY (beli)", "BUY");
            // Menutup scope cabang if untuk kondisi `tradeType.Equals(”BUY”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok
            // tersebut dalam JsonElementToInlineText.
            }

            // Memeriksa membandingkan kesamaan `tradeType` dengan `”SELL”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan
            // comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam JsonElementToInlineText.
            if (tradeType.Equals("SELL", StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `tradeType.Equals(”SELL”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam JsonElementToInlineText.
            {
                // Mengembalikan memanggil `L` dengan `language`, `”SELL (jual)”`, `”SELL”` kepada pemanggil dalam JsonElementToInlineText; eksekusi jalur ini
                // selesai setelah nilai hasil ditentukan.
                return L(language, "SELL (jual)", "SELL");
            // Menutup scope cabang if untuk kondisi `tradeType.Equals(”SELL”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok
            // tersebut dalam JsonElementToInlineText.
            }
        // Menutup scope cabang if untuk kondisi `key == ”trade_type” && value.ValueKind == JsonValueKind.String`; bagian berikut berada di luar batas blok
        // tersebut dalam JsonElementToInlineText.
        }

        // Mengembalikan hasil pemetaan `value.ValueKind` melalui cabang pola switch yang cocok kepada pemanggil dalam JsonElementToInlineText; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return value.ValueKind switch
        // Membuka scope pemetaan switch atas `value.ValueKind`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam JsonElementToInlineText.
        {
            // Untuk pola `JsonValueKind.String`, menghasilkan `value.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti
            // sebagai hasil switch.
            JsonValueKind.String => value.GetString() ?? string.Empty,
            // Untuk pola `JsonValueKind.Number`, menghasilkan memanggil `FormatNumber` dengan `value.GetDouble()` sebagai hasil switch.
            JsonValueKind.Number => FormatNumber(value.GetDouble()),
            // Untuk pola `JsonValueKind.True`, menghasilkan memanggil `L` dengan `language`, `”ya”`, `”true”` sebagai hasil switch.
            JsonValueKind.True => L(language, "ya", "true"),
            // Untuk pola `JsonValueKind.False`, menghasilkan memanggil `L` dengan `language`, `”tidak”`, `”false”` sebagai hasil switch.
            JsonValueKind.False => L(language, "tidak", "false"),
            // Untuk pola `JsonValueKind.Array`, menghasilkan memanggil `L` dengan `language`, `$”[{value.GetArrayLength()} item]”`,
            // `$”[{value.GetArrayLength()} item(s)]”` sebagai hasil switch.
            JsonValueKind.Array => L(language, $"[{value.GetArrayLength()} item]", $"[{value.GetArrayLength()} item(s)]"),
            // Untuk pola `JsonValueKind.Object`, menghasilkan nilai literal `”{...}”` sebagai hasil switch.
            JsonValueKind.Object => "{...}",
            // Untuk pola `_`, menghasilkan `string.Empty`, yaitu nilai kosong bawaan tipe terkait sebagai hasil switch.
            _ => string.Empty
        // Menutup scope pemetaan switch atas `value.ValueKind`; bagian berikut berada di luar batas blok tersebut dalam JsonElementToInlineText.
        };
    // Menutup scope metode JsonElementToInlineText; bagian berikut berada di luar batas blok tersebut dalam JsonElementToInlineText.
    }

    /// <summary>
    /// Mencoba mengambil nilai string dari properti payload JSON.
    /// </summary>
    /// <param name="payload">Elemen JSON sumber.</param>
    /// <param name="propertyName">Nama properti yang dicari.</param>
    /// <param name="value">Nilai string hasil ekstraksi.</param>
    /// <returns>True jika properti ditemukan dan bernilai string tidak kosong.</returns>
    // Mendefinisikan metode `TryGetString` dengan hasil bertipe `bool`. Mencoba mengambil nilai string dari properti payload JSON. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property
    // nama; Parameter `value` bertipe `string` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    private static bool TryGetString(JsonElement payload, string propertyName, out string value)
    // Membuka scope metode TryGetString; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetString.
    {
        // Memperbarui `value` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryGetString.
        value = string.Empty;
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `payload.ValueKind != JsonValueKind.Object ||
        // !payload.TryGetProperty(propertyName, out var property)` dan `property.ValueKind != JsonValueKind.String`; sisi kanan diperiksa hanya jika sisi
        // kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryGetString.
        if (payload.ValueKind != JsonValueKind.Object ||
            // Menggunakan kebalikan kondisi `payload.TryGetProperty(propertyName, out var property)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryGetString.
            !payload.TryGetProperty(propertyName, out var property) ||
            // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `property.ValueKind` dan `JsonValueKind.String` dalam TryGetString.
            property.ValueKind != JsonValueKind.String)
        // Membuka scope cabang if untuk kondisi `payload.ValueKind != JsonValueKind.Object || !payload.TryGetProperty(propertyName, out var property) ||
        // property.ValueKind != JsonValueKind.String`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetString.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryGetString; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `payload.ValueKind != JsonValueKind.Object || !payload.TryGetProperty(propertyName, out var property) ||
        // property.ValueKind != JsonValueKind.String`; bagian berikut berada di luar batas blok tersebut dalam TryGetString.
        }

        // Memperbarui `value` menggunakan `property.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
        // TryGetString.
        value = property.GetString() ?? string.Empty;
        // Mengembalikan kebalikan kondisi `string.IsNullOrWhiteSpace(value)` kepada pemanggil dalam TryGetString; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return !string.IsNullOrWhiteSpace(value);
    // Menutup scope metode TryGetString; bagian berikut berada di luar batas blok tersebut dalam TryGetString.
    }

    /// <summary>
    /// Mencoba mengambil nilai integer dari properti payload JSON.
    /// </summary>
    /// <param name="payload">Elemen JSON sumber.</param>
    /// <param name="propertyName">Nama properti yang dicari.</param>
    /// <param name="value">Nilai integer hasil ekstraksi.</param>
    /// <returns>True jika properti ditemukan dan bernilai numerik.</returns>
    // Mendefinisikan metode `TryGetInt` dengan hasil bertipe `bool`. Mencoba mengambil nilai integer dari properti payload JSON. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property
    // nama; Parameter `value` bertipe `int` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    private static bool TryGetInt(JsonElement payload, string propertyName, out int value)
    // Membuka scope metode TryGetInt; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetInt.
    {
        // Memperbarui `value` menggunakan nilai literal `0` dalam TryGetInt.
        value = 0;
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `payload.ValueKind != JsonValueKind.Object ||
        // !payload.TryGetProperty(propertyName, out var property)` dan `property.ValueKind != JsonValueKind.Number`; sisi kanan diperiksa hanya jika sisi
        // kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryGetInt.
        if (payload.ValueKind != JsonValueKind.Object ||
            // Menggunakan kebalikan kondisi `payload.TryGetProperty(propertyName, out var property)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryGetInt.
            !payload.TryGetProperty(propertyName, out var property) ||
            // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `property.ValueKind` dan `JsonValueKind.Number` dalam TryGetInt.
            property.ValueKind != JsonValueKind.Number)
        // Membuka scope cabang if untuk kondisi `payload.ValueKind != JsonValueKind.Object || !payload.TryGetProperty(propertyName, out var property) ||
        // property.ValueKind != JsonValueKind.Number`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetInt.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryGetInt; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `payload.ValueKind != JsonValueKind.Object || !payload.TryGetProperty(propertyName, out var property) ||
        // property.ValueKind != JsonValueKind.Number`; bagian berikut berada di luar batas blok tersebut dalam TryGetInt.
        }

        // Memeriksa memanggil `property.TryGetInt32` dengan `value`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryGetInt.
        if (property.TryGetInt32(out value))
        // Membuka scope cabang if untuk kondisi `property.TryGetInt32(out value)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryGetInt.
        {
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryGetInt; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `property.TryGetInt32(out value)`; bagian berikut berada di luar batas blok tersebut dalam TryGetInt.
        }

        // Memperbarui `value` menggunakan hasil konversi `Math.Round(property.GetDouble())` menjadi tipe `int` dalam TryGetInt.
        value = (int)Math.Round(property.GetDouble());
        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryGetInt; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return true;
    // Menutup scope metode TryGetInt; bagian berikut berada di luar batas blok tersebut dalam TryGetInt.
    }

    /// <summary>
    /// Mencoba mengambil nilai numerik (double) dari properti payload JSON.
    /// </summary>
    /// <param name="payload">Elemen JSON sumber.</param>
    /// <param name="propertyName">Nama properti yang dicari.</param>
    /// <param name="value">Nilai double hasil ekstraksi.</param>
    /// <returns>True jika properti ditemukan dan bernilai numerik.</returns>
    // Mendefinisikan metode `TryGetNumber` dengan hasil bertipe `bool`. Mencoba mengambil nilai numerik (double) dari properti payload JSON. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai
    // property nama; Parameter `value` bertipe `double` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    private static bool TryGetNumber(JsonElement payload, string propertyName, out double value)
    // Membuka scope metode TryGetNumber; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetNumber.
    {
        // Memperbarui `value` menggunakan nilai literal `0` dalam TryGetNumber.
        value = 0;
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `payload.ValueKind != JsonValueKind.Object ||
        // !payload.TryGetProperty(propertyName, out var property)` dan `property.ValueKind != JsonValueKind.Number`; sisi kanan diperiksa hanya jika sisi
        // kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryGetNumber.
        if (payload.ValueKind != JsonValueKind.Object ||
            // Menggunakan kebalikan kondisi `payload.TryGetProperty(propertyName, out var property)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryGetNumber.
            !payload.TryGetProperty(propertyName, out var property) ||
            // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `property.ValueKind` dan `JsonValueKind.Number` dalam TryGetNumber.
            property.ValueKind != JsonValueKind.Number)
        // Membuka scope cabang if untuk kondisi `payload.ValueKind != JsonValueKind.Object || !payload.TryGetProperty(propertyName, out var property) ||
        // property.ValueKind != JsonValueKind.Number`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetNumber.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryGetNumber; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `payload.ValueKind != JsonValueKind.Object || !payload.TryGetProperty(propertyName, out var property) ||
        // property.ValueKind != JsonValueKind.Number`; bagian berikut berada di luar batas blok tersebut dalam TryGetNumber.
        }

        // Memperbarui `value` menggunakan memanggil `property.GetDouble` dengan tanpa argumen dalam TryGetNumber.
        value = property.GetDouble();
        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryGetNumber; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return true;
    // Menutup scope metode TryGetNumber; bagian berikut berada di luar batas blok tersebut dalam TryGetNumber.
    }

    /// <summary>
    /// Mencoba mengambil jumlah elemen dari properti array dalam payload JSON.
    /// </summary>
    /// <param name="payload">Elemen JSON sumber.</param>
    /// <param name="propertyName">Nama properti array yang dicari.</param>
    /// <param name="count">Jumlah elemen dalam array.</param>
    /// <returns>True jika properti ditemukan dan bertipe array.</returns>
    // Mendefinisikan metode `TryGetArrayCount` dengan hasil bertipe `bool`. Mencoba mengambil jumlah elemen dari properti array dalam payload JSON.
    // Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string`
    // membawa nilai property nama; Parameter `count` bertipe `int` membawa nilai jumlah; out mengembalikan nilai melalui parameter dan harus diisi oleh
    // metode.
    private static bool TryGetArrayCount(JsonElement payload, string propertyName, out int count)
    // Membuka scope metode TryGetArrayCount; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetArrayCount.
    {
        // Memperbarui `count` menggunakan nilai literal `0` dalam TryGetArrayCount.
        count = 0;
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `payload.ValueKind != JsonValueKind.Object ||
        // !payload.TryGetProperty(propertyName, out var property)` dan `property.ValueKind != JsonValueKind.Array`; sisi kanan diperiksa hanya jika sisi
        // kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryGetArrayCount.
        if (payload.ValueKind != JsonValueKind.Object ||
            // Menggunakan kebalikan kondisi `payload.TryGetProperty(propertyName, out var property)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryGetArrayCount.
            !payload.TryGetProperty(propertyName, out var property) ||
            // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `property.ValueKind` dan `JsonValueKind.Array` dalam TryGetArrayCount.
            property.ValueKind != JsonValueKind.Array)
        // Membuka scope cabang if untuk kondisi `payload.ValueKind != JsonValueKind.Object || !payload.TryGetProperty(propertyName, out var property) ||
        // property.ValueKind != JsonValueKind.Array`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetArrayCount.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryGetArrayCount; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `payload.ValueKind != JsonValueKind.Object || !payload.TryGetProperty(propertyName, out var property) ||
        // property.ValueKind != JsonValueKind.Array`; bagian berikut berada di luar batas blok tersebut dalam TryGetArrayCount.
        }

        // Memperbarui `count` menggunakan memanggil `property.GetArrayLength` dengan tanpa argumen dalam TryGetArrayCount.
        count = property.GetArrayLength();
        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryGetArrayCount; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return true;
    // Menutup scope metode TryGetArrayCount; bagian berikut berada di luar batas blok tersebut dalam TryGetArrayCount.
    }

    /// <summary>
    /// Memformat angka double menjadi string dengan maksimal 2 desimal menggunakan kultur invariant.
    /// </summary>
    /// <param name="value">Nilai numerik yang akan diformat.</param>
    /// <returns>String angka yang diformat.</returns>
    // Mendefinisikan metode `FormatNumber` dengan hasil bertipe `string`. Memformat angka double menjadi string dengan maksimal 2 desimal menggunakan
    // kultur invariant. Masukan: Parameter `value` bertipe `double` membawa nilai nilai. Nilai hasil langsung berasal dari mengubah `value` menjadi
    // teks memakai format `”0.##”`, `CultureInfo.InvariantCulture`.
    private static string FormatNumber(double value)
        // Melengkapi struktur ekspresi ArrowExpressionClause melalui => value.ToString(”0.##”, CultureInfo.InvariantCulture); dalam FormatNumber; token
        // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
        => value.ToString("0.##", CultureInfo.InvariantCulture);

    /// <summary>
    /// Menerjemahkan kode weekday event menjadi nama hari sesuai bahasa aktif.
    /// </summary>
    /// <param name="weekday">Kode hari dari API, misalnya MON atau TUE.</param>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <returns>Nama hari terlokalisasi atau nilai asal jika kode tidak dikenal.</returns>
    // Mendefinisikan metode `ResolveWeekdayLabel` dengan hasil bertipe `string`. Menerjemahkan kode weekday event menjadi nama hari sesuai bahasa
    // aktif. Masukan: Parameter `weekday` bertipe `string` membawa nilai weekday; Parameter `language` bertipe `string` membawa nilai language.
    private static string ResolveWeekdayLabel(string weekday, string language)
    // Membuka scope metode ResolveWeekdayLabel; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveWeekdayLabel.
    {
        // Mengembalikan hasil pemetaan `(weekday ?? string.Empty).Trim().ToUpperInvariant()` melalui cabang pola switch yang cocok kepada pemanggil dalam
        // ResolveWeekdayLabel; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return (weekday ?? string.Empty).Trim().ToUpperInvariant() switch
        // Membuka scope pemetaan switch atas `(weekday ?? string.Empty).Trim().ToUpperInvariant()`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ResolveWeekdayLabel.
        {
            // Untuk pola `”MON”`, menghasilkan memanggil `L` dengan `language`, `”Senin”`, `”Monday”` sebagai hasil switch.
            "MON" => L(language, "Senin", "Monday"),
            // Untuk pola `”TUE”`, menghasilkan memanggil `L` dengan `language`, `”Selasa”`, `”Tuesday”` sebagai hasil switch.
            "TUE" => L(language, "Selasa", "Tuesday"),
            // Untuk pola `”WED”`, menghasilkan memanggil `L` dengan `language`, `”Rabu”`, `”Wednesday”` sebagai hasil switch.
            "WED" => L(language, "Rabu", "Wednesday"),
            // Untuk pola `”THU”`, menghasilkan memanggil `L` dengan `language`, `”Kamis”`, `”Thursday”` sebagai hasil switch.
            "THU" => L(language, "Kamis", "Thursday"),
            // Untuk pola `”FRI”`, menghasilkan memanggil `L` dengan `language`, `”Jumat”`, `”Friday”` sebagai hasil switch.
            "FRI" => L(language, "Jumat", "Friday"),
            // Untuk pola `”SAT”`, menghasilkan memanggil `L` dengan `language`, `”Sabtu”`, `”Saturday”` sebagai hasil switch.
            "SAT" => L(language, "Sabtu", "Saturday"),
            // Untuk pola `”SUN”`, menghasilkan memanggil `L` dengan `language`, `”Minggu”`, `”Sunday”` sebagai hasil switch.
            "SUN" => L(language, "Minggu", "Sunday"),
            // Untuk pola `_`, menghasilkan `weekday` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti sebagai hasil switch.
            _ => weekday ?? string.Empty
        // Menutup scope pemetaan switch atas `(weekday ?? string.Empty).Trim().ToUpperInvariant()`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveWeekdayLabel.
        };
    // Menutup scope metode ResolveWeekdayLabel; bagian berikut berada di luar batas blok tersebut dalam ResolveWeekdayLabel.
    }

    /// <summary>
    /// Helper bilingual: mengembalikan teks bahasa Indonesia atau Inggris sesuai kode bahasa aktif.
    /// </summary>
    /// <param name="language">Kode bahasa aktif.</param>
    /// <param name="id">Teks dalam bahasa Indonesia.</param>
    /// <param name="en">Teks dalam bahasa Inggris.</param>
    /// <returns>Teks sesuai bahasa yang dipilih.</returns>
    // Mendefinisikan metode `L` dengan hasil bertipe `string`. Helper bilingual: mengembalikan teks bahasa Indonesia atau Inggris sesuai kode bahasa
    // aktif. Masukan: Parameter `language` bertipe `string` membawa nilai language; Parameter `id` bertipe `string` membawa nilai identitas; Parameter
    // `en` bertipe `string` membawa nilai en. Nilai hasil langsung berasal dari hasil pemilihan bersyarat: ketika `string.Equals(language,
    // AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase)` benar gunakan `en`, jika tidak gunakan `id`.
    private static string L(string language, string id, string en)
        // Melengkapi struktur ekspresi ArrowExpressionClause melalui => string.Equals(language, AuthConstants.LanguageEn,
        // StringComparison.OrdinalIgnoreCase) ? en : id; dalam L; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
        => string.Equals(language, AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase) ? en : id;
// Menutup scope tipe SessionTimelineMapper; bagian berikut berada di luar batas blok tersebut.
}
