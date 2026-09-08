// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui SessionTimelineMapperTests.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;
// Mengimpor namespace `Cashflowpoly.Ui.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Infrastructure;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `SessionTimelineMapperTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionTimelineMapperTests
// Membuka scope tipe SessionTimelineMapperTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `MapTimeline_ShouldLocalizeWeekdayNamesForActiveLanguage` dengan hasil bertipe `void`; operasi ini menangani pemetaan
    // timeline should localize weekday nama untuk aktif language.
    public void MapTimeline_ShouldLocalizeWeekdayNamesForActiveLanguage()
    // Membuka scope metode MapTimeline_ShouldLocalizeWeekdayNamesForActiveLanguage; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // MapTimeline_ShouldLocalizeWeekdayNamesForActiveLanguage.
    {
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventRequest>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventRequest>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MapTimeline_ShouldLocalizeWeekdayNamesForActiveLanguage.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”KerjaLepas”`, `”””{”amount”:1}”””`, `”MON”` dalam
            // MapTimeline_ShouldLocalizeWeekdayNamesForActiveLanguage.
            CreateEvent("KerjaLepas", """{"amount":1}""", "MON"),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”BahanMasakan”`, `”””{”card_id”:”sayur”,”amount”:2}”””`, `”TUE”`, `2` dalam
            // MapTimeline_ShouldLocalizeWeekdayNamesForActiveLanguage.
            CreateEvent("BahanMasakan", """{"card_id":"sayur","amount":2}""", "TUE", sequenceNumber: 2)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // MapTimeline_ShouldLocalizeWeekdayNamesForActiveLanguage.
        };

        // Menyiapkan variabel lokal `indonesianTimeline` untuk nilai indonesian timeline dengan memanggil `SessionTimelineMapper.MapTimeline` dengan
        // `events`, `”id”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var indonesianTimeline = SessionTimelineMapper.MapTimeline(events, "id");
        // Menyiapkan variabel lokal `englishTimeline` untuk nilai english timeline dengan memanggil `SessionTimelineMapper.MapTimeline` dengan `events`,
        // `”en”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var englishTimeline = SessionTimelineMapper.MapTimeline(events, "en");

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”Senin”, ”Selasa” }`,
        // `indonesianTimeline.Select(item => item.Weekday)`); pengujian gagal jika keduanya berbeda dalam
        // MapTimeline_ShouldLocalizeWeekdayNamesForActiveLanguage.
        Assert.Equal(new[] { "Senin", "Selasa" }, indonesianTimeline.Select(item => item.Weekday));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”Monday”, ”Tuesday” }`,
        // `englishTimeline.Select(item => item.Weekday)`); pengujian gagal jika keduanya berbeda dalam
        // MapTimeline_ShouldLocalizeWeekdayNamesForActiveLanguage.
        Assert.Equal(new[] { "Monday", "Tuesday" }, englishTimeline.Select(item => item.Weekday));
    // Menutup scope metode MapTimeline_ShouldLocalizeWeekdayNamesForActiveLanguage; bagian berikut berada di luar batas blok tersebut dalam
    // MapTimeline_ShouldLocalizeWeekdayNamesForActiveLanguage.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `MapTimeline_ShouldPlaceInitialSetupOnGoWithoutMovingGameplayDayOne` dengan hasil bertipe `void`; operasi ini menangani
    // pemetaan timeline should place awal setup on go tanpa moving gameplay hari one.
    public void MapTimeline_ShouldPlaceInitialSetupOnGoWithoutMovingGameplayDayOne()
    // Membuka scope metode MapTimeline_ShouldPlaceInitialSetupOnGoWithoutMovingGameplayDayOne; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam MapTimeline_ShouldPlaceInitialSetupOnGoWithoutMovingGameplayDayOne.
    {
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventRequest>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventRequest>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MapTimeline_ShouldPlaceInitialSetupOnGoWithoutMovingGameplayDayOne.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”MulaiSesi”`, `”””{”start_note”:”Mulai sesi”}”””`, `”MON”`, `0`, `”SYSTEM”`, `1`
            // dalam MapTimeline_ShouldPlaceInitialSetupOnGoWithoutMovingGameplayDayOne.
            CreateEvent("MulaiSesi", """{"start_note":"Mulai sesi"}""", "MON", actionSlot: 0, actorType: "SYSTEM", dayIndex: 1),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”SetupBahanAwal”`, `”””{”card_id”:”sayur”,”setup”:”INITIAL”}”””`, `”MON”`, `2`,
            // `0`, `”SYSTEM”`, `1` dalam MapTimeline_ShouldPlaceInitialSetupOnGoWithoutMovingGameplayDayOne.
            CreateEvent("SetupBahanAwal", """{"card_id":"sayur","setup":"INITIAL"}""", "MON", sequenceNumber: 2, actionSlot: 0, actorType: "SYSTEM", dayIndex: 1),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”KerjaLepas”`, `”””{”amount”:1}”””`, `”MON”`, `3`, `1` dalam
            // MapTimeline_ShouldPlaceInitialSetupOnGoWithoutMovingGameplayDayOne.
            CreateEvent("KerjaLepas", """{"amount":1}""", "MON", sequenceNumber: 3, dayIndex: 1)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // MapTimeline_ShouldPlaceInitialSetupOnGoWithoutMovingGameplayDayOne.
        };

        // Menyiapkan variabel lokal `timeline` untuk nilai timeline dengan memanggil `SessionTimelineMapper.MapTimeline` dengan `events`, `”id”`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var timeline = SessionTimelineMapper.MapTimeline(events, "id");

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { 0, 0, 1 }`, `timeline.Select(item =>
        // item.DayIndex)`); pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldPlaceInitialSetupOnGoWithoutMovingGameplayDayOne.
        Assert.Equal(new[] { 0, 0, 1 }, timeline.Select(item => item.DayIndex));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { 0, 0, 1 }`, `timeline.Select(item =>
        // item.ActionSlot)`); pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldPlaceInitialSetupOnGoWithoutMovingGameplayDayOne.
        Assert.Equal(new[] { 0, 0, 1 }, timeline.Select(item => item.ActionSlot));
    // Menutup scope metode MapTimeline_ShouldPlaceInitialSetupOnGoWithoutMovingGameplayDayOne; bagian berikut berada di luar batas blok tersebut dalam
    // MapTimeline_ShouldPlaceInitialSetupOnGoWithoutMovingGameplayDayOne.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `MapTimeline_ShouldClassifyInsuranceEventsAsFinancing` dengan hasil bertipe `void`; operasi ini menangani pemetaan timeline
    // should classify asuransi event as financing.
    public void MapTimeline_ShouldClassifyInsuranceEventsAsFinancing()
    // Membuka scope metode MapTimeline_ShouldClassifyInsuranceEventsAsFinancing; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // MapTimeline_ShouldClassifyInsuranceEventsAsFinancing.
    {
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventRequest>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventRequest>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MapTimeline_ShouldClassifyInsuranceEventsAsFinancing.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”Asuransi”`,
            // `”””{”policy_id”:”INS-SEED-001”,”premium”:1,”coverage_type”:”MULTIRISK”}”””`, `”MON”` dalam MapTimeline_ShouldClassifyInsuranceEventsAsFinancing.
            CreateEvent("Asuransi", """{"policy_id":"INS-SEED-001","premium":1,"coverage_type":"MULTIRISK"}""", "MON")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // MapTimeline_ShouldClassifyInsuranceEventsAsFinancing.
        };

        // Menyiapkan variabel lokal `indonesianItem` untuk nilai indonesian elemen dengan pemeriksaan hasil dengan `Assert.Single` menggunakan
        // `SessionTimelineMapper.MapTimeline(events, ”id”)`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var indonesianItem = Assert.Single(SessionTimelineMapper.MapTimeline(events, "id"));
        // Menyiapkan variabel lokal `englishItem` untuk nilai english elemen dengan pemeriksaan hasil dengan `Assert.Single` menggunakan
        // `SessionTimelineMapper.MapTimeline(events, ”en”)`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var englishItem = Assert.Single(SessionTimelineMapper.MapTimeline(events, "en"));

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Pembiayaan”`, `indonesianItem.FlowLabel`);
        // pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyInsuranceEventsAsFinancing.
        Assert.Equal("Pembiayaan", indonesianItem.FlowLabel);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Membeli asuransi multirisk dengan premi 1.”`,
        // `indonesianItem.FlowDescription`); pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyInsuranceEventsAsFinancing.
        Assert.Equal("Membeli asuransi multirisk dengan premi 1.", indonesianItem.FlowDescription);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”action”`, `indonesianItem.ActionSlotRole`);
        // pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyInsuranceEventsAsFinancing.
        Assert.Equal("action", indonesianItem.ActionSlotRole);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Aksi 1”`, `indonesianItem.ActionSlotLabel`);
        // pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyInsuranceEventsAsFinancing.
        Assert.Equal("Aksi 1", indonesianItem.ActionSlotLabel);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Financing”`, `englishItem.FlowLabel`);
        // pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyInsuranceEventsAsFinancing.
        Assert.Equal("Financing", englishItem.FlowLabel);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Purchased multirisk insurance with premium
        // 1.”`, `englishItem.FlowDescription`); pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyInsuranceEventsAsFinancing.
        Assert.Equal("Purchased multirisk insurance with premium 1.", englishItem.FlowDescription);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”action”`, `englishItem.ActionSlotRole`);
        // pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyInsuranceEventsAsFinancing.
        Assert.Equal("action", englishItem.ActionSlotRole);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Action 1”`, `englishItem.ActionSlotLabel`);
        // pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyInsuranceEventsAsFinancing.
        Assert.Equal("Action 1", englishItem.ActionSlotLabel);
    // Menutup scope metode MapTimeline_ShouldClassifyInsuranceEventsAsFinancing; bagian berikut berada di luar batas blok tersebut dalam
    // MapTimeline_ShouldClassifyInsuranceEventsAsFinancing.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `MapTimeline_ShouldClassifyRiskLifeAsActionEffect` dengan hasil bertipe `void`; operasi ini menangani pemetaan timeline
    // should classify risiko life as aksi effect.
    public void MapTimeline_ShouldClassifyRiskLifeAsActionEffect()
    // Membuka scope metode MapTimeline_ShouldClassifyRiskLifeAsActionEffect; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // MapTimeline_ShouldClassifyRiskLifeAsActionEffect.
    {
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventRequest>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventRequest>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MapTimeline_ShouldClassifyRiskLifeAsActionEffect.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”RisikoKehidupan”`, `”””{”risk_id”:”risk_bonus_tunjangan”}”””`, `”TUE”`, `2` dalam
            // MapTimeline_ShouldClassifyRiskLifeAsActionEffect.
            CreateEvent("RisikoKehidupan", """{"risk_id":"risk_bonus_tunjangan"}""", "TUE", actionSlot: 2)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // MapTimeline_ShouldClassifyRiskLifeAsActionEffect.
        };

        // Menyiapkan variabel lokal `indonesianItem` untuk nilai indonesian elemen dengan pemeriksaan hasil dengan `Assert.Single` menggunakan
        // `SessionTimelineMapper.MapTimeline(events, ”id”)`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var indonesianItem = Assert.Single(SessionTimelineMapper.MapTimeline(events, "id"));
        // Menyiapkan variabel lokal `englishItem` untuk nilai english elemen dengan pemeriksaan hasil dengan `Assert.Single` menggunakan
        // `SessionTimelineMapper.MapTimeline(events, ”en”)`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var englishItem = Assert.Single(SessionTimelineMapper.MapTimeline(events, "en"));

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”effect”`, `indonesianItem.ActionSlotRole`);
        // pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyRiskLifeAsActionEffect.
        Assert.Equal("effect", indonesianItem.ActionSlotRole);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Efek Aksi 2”`,
        // `indonesianItem.ActionSlotLabel`); pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyRiskLifeAsActionEffect.
        Assert.Equal("Efek Aksi 2", indonesianItem.ActionSlotLabel);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”effect”`, `englishItem.ActionSlotRole`);
        // pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyRiskLifeAsActionEffect.
        Assert.Equal("effect", englishItem.ActionSlotRole);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Action Effect 2”`,
        // `englishItem.ActionSlotLabel`); pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyRiskLifeAsActionEffect.
        Assert.Equal("Action Effect 2", englishItem.ActionSlotLabel);
    // Menutup scope metode MapTimeline_ShouldClassifyRiskLifeAsActionEffect; bagian berikut berada di luar batas blok tersebut dalam
    // MapTimeline_ShouldClassifyRiskLifeAsActionEffect.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `MapTimeline_ShouldClassifyInsuranceUseAsActionResponse` dengan hasil bertipe `void`; operasi ini menangani pemetaan
    // timeline should classify asuransi use as aksi respons.
    public void MapTimeline_ShouldClassifyInsuranceUseAsActionResponse()
    // Membuka scope metode MapTimeline_ShouldClassifyInsuranceUseAsActionResponse; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // MapTimeline_ShouldClassifyInsuranceUseAsActionResponse.
    {
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventRequest>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventRequest>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MapTimeline_ShouldClassifyInsuranceUseAsActionResponse.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”Asuransi”`, `”””{”risk_event_id”:”risk-event-001”}”””`, `”TUE”`, `2` dalam
            // MapTimeline_ShouldClassifyInsuranceUseAsActionResponse.
            CreateEvent("Asuransi", """{"risk_event_id":"risk-event-001"}""", "TUE", actionSlot: 2)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // MapTimeline_ShouldClassifyInsuranceUseAsActionResponse.
        };

        // Menyiapkan variabel lokal `indonesianItem` untuk nilai indonesian elemen dengan pemeriksaan hasil dengan `Assert.Single` menggunakan
        // `SessionTimelineMapper.MapTimeline(events, ”id”)`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var indonesianItem = Assert.Single(SessionTimelineMapper.MapTimeline(events, "id"));
        // Menyiapkan variabel lokal `englishItem` untuk nilai english elemen dengan pemeriksaan hasil dengan `Assert.Single` menggunakan
        // `SessionTimelineMapper.MapTimeline(events, ”en”)`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var englishItem = Assert.Single(SessionTimelineMapper.MapTimeline(events, "en"));

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”response”`, `indonesianItem.ActionSlotRole`);
        // pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyInsuranceUseAsActionResponse.
        Assert.Equal("response", indonesianItem.ActionSlotRole);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Respons Aksi 2”`,
        // `indonesianItem.ActionSlotLabel`); pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyInsuranceUseAsActionResponse.
        Assert.Equal("Respons Aksi 2", indonesianItem.ActionSlotLabel);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”response”`, `englishItem.ActionSlotRole`);
        // pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyInsuranceUseAsActionResponse.
        Assert.Equal("response", englishItem.ActionSlotRole);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Action Response 2”`,
        // `englishItem.ActionSlotLabel`); pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyInsuranceUseAsActionResponse.
        Assert.Equal("Action Response 2", englishItem.ActionSlotLabel);
    // Menutup scope metode MapTimeline_ShouldClassifyInsuranceUseAsActionResponse; bagian berikut berada di luar batas blok tersebut dalam
    // MapTimeline_ShouldClassifyInsuranceUseAsActionResponse.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `MapTimeline_ShouldClassifyEmergencyOptionAsActionResponse` dengan hasil bertipe `void`; operasi ini menangani pemetaan
    // timeline should classify emergency option as aksi respons.
    public void MapTimeline_ShouldClassifyEmergencyOptionAsActionResponse()
    // Membuka scope metode MapTimeline_ShouldClassifyEmergencyOptionAsActionResponse; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // MapTimeline_ShouldClassifyEmergencyOptionAsActionResponse.
    {
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventRequest>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventRequest>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MapTimeline_ShouldClassifyEmergencyOptionAsActionResponse.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”GunakanOpsiDarurat”`, `”””{”option_type”:”loan”,”direction”:”IN”,”amount”:5}”””`,
            // `”TUE”`, `2` dalam MapTimeline_ShouldClassifyEmergencyOptionAsActionResponse.
            CreateEvent("GunakanOpsiDarurat", """{"option_type":"loan","direction":"IN","amount":5}""", "TUE", actionSlot: 2)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // MapTimeline_ShouldClassifyEmergencyOptionAsActionResponse.
        };

        // Menyiapkan variabel lokal `item` untuk nilai elemen dengan pemeriksaan hasil dengan `Assert.Single` menggunakan
        // `SessionTimelineMapper.MapTimeline(events, ”id”)`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var item = Assert.Single(SessionTimelineMapper.MapTimeline(events, "id"));

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”response”`, `item.ActionSlotRole`); pengujian
        // gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyEmergencyOptionAsActionResponse.
        Assert.Equal("response", item.ActionSlotRole);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Respons Aksi 2”`, `item.ActionSlotLabel`);
        // pengujian gagal jika keduanya berbeda dalam MapTimeline_ShouldClassifyEmergencyOptionAsActionResponse.
        Assert.Equal("Respons Aksi 2", item.ActionSlotLabel);
    // Menutup scope metode MapTimeline_ShouldClassifyEmergencyOptionAsActionResponse; bagian berikut berada di luar batas blok tersebut dalam
    // MapTimeline_ShouldClassifyEmergencyOptionAsActionResponse.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `MapTimeline_ShouldDescribeDonationWinnerAnnouncementAsSystemSummary` dengan hasil bertipe `void`; operasi ini menangani
    // pemetaan timeline should describe donasi winner announcement as system summary.
    public void MapTimeline_ShouldDescribeDonationWinnerAnnouncementAsSystemSummary()
    // Membuka scope metode MapTimeline_ShouldDescribeDonationWinnerAnnouncementAsSystemSummary; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam MapTimeline_ShouldDescribeDonationWinnerAnnouncementAsSystemSummary.
    {
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventRequest>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventRequest>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MapTimeline_ShouldDescribeDonationWinnerAnnouncementAsSystemSummary.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”UmumkanJuaraDonasi”`, `”””{”summary”:”Manalu Juara 1, Marcello Juara 2, Marco
            // Juara 3”,”winners”:[{”rank”:1,”player_name”:”Manalu”,”points”:7},{”rank”:2,”player_name”:”Marcello”,”points”:5},{”rank”:...`, `”FRI”`,
            // `”SYSTEM”`, `null` dalam MapTimeline_ShouldDescribeDonationWinnerAnnouncementAsSystemSummary.
            CreateEvent(
                // Meneruskan nilai literal `”UmumkanJuaraDonasi”` sebagai argumen ke `CreateEvent`.
                "UmumkanJuaraDonasi",
                // Meneruskan nilai literal `”””{”summary”:”Manalu Juara 1, Marcello Juara 2, Marco Juara
                // 3”,”winners”:[{”rank”:1,”player_name”:”Manalu”,”points”:7},{”rank”:2,”player_name”:”Marcello”,”points”:5},{”rank”:...` sebagai argumen ke
                // `CreateEvent`.
                """{"summary":"Manalu Juara 1, Marcello Juara 2, Marco Juara 3","winners":[{"rank":1,"player_name":"Manalu","points":7},{"rank":2,"player_name":"Marcello","points":5},{"rank":3,"player_name":"Marco","points":2}]}""",
                // Meneruskan nilai literal `”FRI”` sebagai argumen ke `CreateEvent`.
                "FRI",
                // Meneruskan nilai literal `”SYSTEM”` sebagai argumen bernama `actorType`.
                actorType: "SYSTEM",
                // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `userId`.
                userId: null)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // MapTimeline_ShouldDescribeDonationWinnerAnnouncementAsSystemSummary.
        };

        // Menyiapkan variabel lokal `item` untuk nilai elemen dengan pemeriksaan hasil dengan `Assert.Single` menggunakan
        // `SessionTimelineMapper.MapTimeline(events, ”id”)`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var item = Assert.Single(SessionTimelineMapper.MapTimeline(events, "id"));

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Peduli Donasi”`, `item.FlowLabel`); pengujian
        // gagal jika keduanya berbeda dalam MapTimeline_ShouldDescribeDonationWinnerAnnouncementAsSystemSummary.
        Assert.Equal("Peduli Donasi", item.FlowLabel);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Sistem menentukan Juara Donasi: Manalu Juara
        // 1, Marcello Juara 2, Marco Juara 3.”`, `item.FlowDescription`); pengujian gagal jika keduanya berbeda dalam
        // MapTimeline_ShouldDescribeDonationWinnerAnnouncementAsSystemSummary.
        Assert.Equal("Sistem menentukan Juara Donasi: Manalu Juara 1, Marcello Juara 2, Marco Juara 3.", item.FlowDescription);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”SYSTEM”`, `item.ActorType`); pengujian gagal
        // jika keduanya berbeda dalam MapTimeline_ShouldDescribeDonationWinnerAnnouncementAsSystemSummary.
        Assert.Equal("SYSTEM", item.ActorType);
        // Menjalankan pemeriksaan Null atas `item.PlayerId` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // MapTimeline_ShouldDescribeDonationWinnerAnnouncementAsSystemSummary.
        Assert.Null(item.PlayerId);
    // Menutup scope metode MapTimeline_ShouldDescribeDonationWinnerAnnouncementAsSystemSummary; bagian berikut berada di luar batas blok tersebut dalam
    // MapTimeline_ShouldDescribeDonationWinnerAnnouncementAsSystemSummary.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”id”, ”poin kebahagiaan”).
    [InlineData("id", "poin kebahagiaan")]
    // menyediakan satu kombinasi masukan pengujian (”en”, ”happiness points”).
    [InlineData("en", "happiness points")]
    // Mendefinisikan metode `MapTimeline_UsesHappinessPointsForScoringDescriptions` dengan hasil bertipe `void`; operasi ini menangani pemetaan
    // timeline uses kebahagiaan poin untuk scoring descriptions. Masukan: Parameter `language` bertipe `string` membawa nilai language; Parameter
    // `expected` bertipe `string` membawa nilai yang diharapkan.
    public void MapTimeline_UsesHappinessPointsForScoringDescriptions(string language, string expected)
    // Membuka scope metode MapTimeline_UsesHappinessPointsForScoringDescriptions; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // MapTimeline_UsesHappinessPointsForScoringDescriptions.
    {
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventRequest>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventRequest>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // MapTimeline_UsesHappinessPointsForScoringDescriptions.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”Kebutuhan”`, `”””{”card_id”:”buku_1”,”amount”:2,”points”:1}”””`, `”MON”` dalam
            // MapTimeline_UsesHappinessPointsForScoringDescriptions.
            CreateEvent("Kebutuhan", """{"card_id":"buku_1","amount":2,"points":1}""", "MON"),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”TujuanFinansial”`, `”””{”goal_id”:”goal_35”,”cost”:35,”points”:35}”””`, `”MON”`,
            // `2` dalam MapTimeline_UsesHappinessPointsForScoringDescriptions.
            CreateEvent("TujuanFinansial", """{"goal_id":"goal_35","cost":35,"points":35}""", "MON", sequenceNumber: 2),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”PoinPeringkatDonasi”`, `”””{”rank”:2,”points”:5}”””`, `”FRI”`, `3` dalam
            // MapTimeline_UsesHappinessPointsForScoringDescriptions.
            CreateEvent("PoinPeringkatDonasi", """{"rank":2,"points":5}""", "FRI", sequenceNumber: 3),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”PoinPeringkatPensiun”`, `”””{”rank”:1,”points”:5}”””`, `”MON”`, `4` dalam
            // MapTimeline_UsesHappinessPointsForScoringDescriptions.
            CreateEvent("PoinPeringkatPensiun", """{"rank":1,"points":5}""", "MON", sequenceNumber: 4),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”PoinEmas”`, `”””{”points”:5}”””`, `”MON”`, `5` dalam
            // MapTimeline_UsesHappinessPointsForScoringDescriptions.
            CreateEvent("PoinEmas", """{"points":5}""", "MON", sequenceNumber: 5)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // MapTimeline_UsesHappinessPointsForScoringDescriptions.
        };

        // Menyiapkan variabel lokal `timeline` untuk nilai timeline dengan memanggil `SessionTimelineMapper.MapTimeline` dengan `events`, `language`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var timeline = SessionTimelineMapper.MapTimeline(events, language);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`events.Count`, `timeline.Count`); pengujian
        // gagal jika keduanya berbeda dalam MapTimeline_UsesHappinessPointsForScoringDescriptions.
        Assert.Equal(events.Count, timeline.Count);
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `timeline`, `item => Assert.Contains(expected, item.FlowDescription,
        // StringComparison.OrdinalIgnoreCase)`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // MapTimeline_UsesHappinessPointsForScoringDescriptions.
        Assert.All(timeline, item => Assert.Contains(expected, item.FlowDescription, StringComparison.OrdinalIgnoreCase));
    // Menutup scope metode MapTimeline_UsesHappinessPointsForScoringDescriptions; bagian berikut berada di luar batas blok tersebut dalam
    // MapTimeline_UsesHappinessPointsForScoringDescriptions.
    }

    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `EventRequest`; operasi ini menangani create event. Masukan: Parameter `actionType`
    // bertipe `string` membawa nilai aksi jenis; Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `weekday` bertipe
    // `string` membawa nilai weekday; Parameter `sequenceNumber` bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat
    // permainan; bila argumen tidak diberikan digunakan nilai literal `1`; Parameter `actionSlot` bertipe `int` membawa nilai aksi slot; bila argumen
    // tidak diberikan digunakan nilai literal `1`; Parameter `actorType` bertipe `string` membawa nilai actor jenis; bila argumen tidak diberikan
    // digunakan nilai literal `”PLAYER”`; Parameter `userId` bertipe `Guid?` membawa identitas akun pengguna yang datanya sedang diproses; nilai null
    // diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; Parameter `dayIndex`
    // bertipe `int` membawa nilai hari index; bila argumen tidak diberikan digunakan nilai literal `0`.
    private static EventRequest CreateEvent(
        // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
        string actionType,
        // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON.
        string payloadJson,
        // Parameter `weekday` bertipe `string` membawa nilai weekday.
        string weekday,
        // Parameter `sequenceNumber` bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan; bila argumen tidak
        // diberikan digunakan nilai literal `1`.
        long sequenceNumber = 1,
        // Parameter `actionSlot` bertipe `int` membawa nilai aksi slot; bila argumen tidak diberikan digunakan nilai literal `1`.
        int actionSlot = 1,
        // Parameter `actorType` bertipe `string` membawa nilai actor jenis; bila argumen tidak diberikan digunakan nilai literal `”PLAYER”`.
        string actorType = "PLAYER",
        // Parameter `userId` bertipe `Guid?` membawa identitas akun pengguna yang datanya sedang diproses; nilai null diizinkan ketika data opsional belum
        // tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
        Guid? userId = null,
        // Parameter `dayIndex` bertipe `int` membawa nilai hari index; bila argumen tidak diberikan digunakan nilai literal `0`.
        int dayIndex = 0)
    // Membuka scope metode CreateEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(payloadJson);
        // Mengembalikan objek baru bertipe `EventRequest` dengan argumen ( Guid.NewGuid(), Guid.NewGuid(), userId, actorType,
        // DateTimeOffset.Parse(”2026-02-02T01:00:00Z”), dayIndex, weekday, actionSlot, sequenceNumber, actionType, Gu... kepada pemanggil dalam
        // CreateEvent; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventRequest(
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan `userId` (identitas akun pengguna yang datanya sedang diproses) sebagai argumen ke konstruktor `EventRequest`.
            userId,
            // Meneruskan `actorType` (nilai actor jenis) sebagai argumen ke konstruktor `EventRequest`.
            actorType,
            // Meneruskan memanggil `DateTimeOffset.Parse` dengan `”2026-02-02T01:00:00Z”` sebagai argumen ke konstruktor `EventRequest`; Meneruskan nilai
            // literal `”2026-02-02T01:00:00Z”` sebagai argumen ke `DateTimeOffset.Parse`.
            DateTimeOffset.Parse("2026-02-02T01:00:00Z"),
            // Meneruskan `dayIndex` (nilai hari index) sebagai argumen ke konstruktor `EventRequest`.
            dayIndex,
            // Meneruskan `weekday` (nilai weekday) sebagai argumen ke konstruktor `EventRequest`.
            weekday,
            // Meneruskan `actionSlot` (nilai aksi slot) sebagai argumen ke konstruktor `EventRequest`.
            actionSlot,
            // Meneruskan `sequenceNumber` (nomor urut event yang menentukan urutan pemrosesan riwayat permainan) sebagai argumen ke konstruktor `EventRequest`.
            sequenceNumber,
            // Meneruskan `actionType` (nilai aksi jenis) sebagai argumen ke konstruktor `EventRequest`.
            actionType,
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan membuat salinan `document.RootElement` agar hasil dapat digunakan terpisah dari objek sumber sebagai argumen ke konstruktor
            // `EventRequest`.
            document.RootElement.Clone(),
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke konstruktor `EventRequest`.
            null);
    // Menutup scope metode CreateEvent; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
    }
// Menutup scope tipe SessionTimelineMapperTests; bagian berikut berada di luar batas blok tersebut.
}
