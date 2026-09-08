// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui AnalyticsGameplaySnapshotBuilderTests.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `AnalyticsGameplaySnapshotBuilderTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class AnalyticsGameplaySnapshotBuilderTests
// Membuka scope tipe AnalyticsGameplaySnapshotBuilderTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public void Build_DoesNotGuessPensionRankFromPointsAlone(int points)
    {
        var config = BuildAdvancedConfig() with
        {
            Scoring = new RulesetScoringConfig([], [], [new RankPoint(3, points), new RankPoint(4, points)])
        };
        var snapshot = new GameplaySnapshotBuilder().Build([], [], [], config,
            new AnalyticsHappinessBreakdown(points, 0, 0, 0, 0, points, 0, 0, 0, false));
        using var raw = JsonDocument.Parse(snapshot.RawJson);
        Assert.Equal(JsonValueKind.Null, raw.RootElement.GetProperty("pension").GetProperty("pension_fund_rank_per_game").ValueKind);
    }

    [Fact]
    public void Build_SeparatesInitialInsuranceAndClaimsFromPaidActivationTokens()
    {
        var sessionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var events = new List<EventDb>
        {
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "SetupAsuransiAwal", "{}", 0, 0),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "Asuransi", """{"risk_event_id":"11000000-0000-0000-0000-000000000001"}""", 1, 1),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "Asuransi", """{"premium":1}""", 2, 2),
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "Asuransi", """{"risk_event_id":"11000000-0000-0000-0000-000000000002"}""", 3, 3)
        };
        events[0].ActorType = "SYSTEM";
        var projections = new List<CashflowProjectionDb>
        {
            CreateProjection(events[2].EventId, sessionId, playerId, "OUT", 1, "INSURANCE_PREMIUM")
        };
        var snapshot = new GameplaySnapshotBuilder().Build(events, projections, events,
            BuildAdvancedConfig(), new AnalyticsHappinessBreakdown(0, 0, 0, 0, 0, 0, 0, 0, 0, false));
        using var raw = JsonDocument.Parse(snapshot.RawJson);
        using var derived = JsonDocument.Parse(snapshot.DerivedJson);

        Assert.Equal(2, raw.RootElement.GetProperty("life_risk").GetProperty("life_risk_mitigated_with_insurance").GetInt32());
        Assert.Equal(1, raw.RootElement.GetProperty("life_risk").GetProperty("insurance_payments_made").GetInt32());
        Assert.Equal(1, derived.RootElement.GetProperty("long_term_action_share_components").GetProperty("insurance_actions").GetInt32());
    }

    [Theory]
    [InlineData("SYSTEM", 4, 32)]
    [InlineData("PLAYER", 5, 33)]
    public void Build_CountsPurchasedGoalButDoesNotAddTokensForAutomaticCompletion(
        string purchaseActor, int expectedGoalTokens, int expectedTotalTokens)
    {
        var sessionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var events = new List<EventDb>();
        foreach (var amount in new[] { 5, 10, 15, 5 })
        {
            events.Add(CreateEvent(Guid.NewGuid(), sessionId, playerId, "Menabung",
                JsonSerializer.Serialize(new { goal_id = "goal-35", amount }), turn: 1, sequence: events.Count));
        }
        var purchase = CreateEvent(Guid.NewGuid(), sessionId, playerId, "TujuanFinansial",
            """{"goal_id":"goal-35","cost":35,"points":35}""", turn: 1, sequence: events.Count);
        purchase.ActorType = purchaseActor;
        events.Add(purchase);
        events.Add(CreateEvent(Guid.NewGuid(), sessionId, playerId, "Asuransi",
            """{"premium":1}""", turn: 1, sequence: events.Count));
        events.Add(CreateEvent(Guid.NewGuid(), sessionId, playerId, "BayarPinjaman",
            """{"amount":10}""", turn: 1, sequence: events.Count));
        for (var i = 0; i < 26; i++)
        {
            events.Add(CreateEvent(Guid.NewGuid(), sessionId, playerId, "BahanMasakan",
                """{"card_id":"nasi_putih","amount":1}""", turn: 1, sequence: events.Count));
        }
        var happiness = new AnalyticsHappinessBreakdown(35, 0, 0, 0, 0, 0, 35, 0, 0, false);
        var snapshot = new GameplaySnapshotBuilder().Build(events, [], events, BuildAdvancedConfig(), happiness);
        using var raw = JsonDocument.Parse(snapshot.RawJson);
        using var derived = JsonDocument.Parse(snapshot.DerivedJson);
        var goals = raw.RootElement.GetProperty("financial_goals");
        var components = derived.RootElement.GetProperty("long_term_action_share_components");
        Assert.Equal(1, goals.GetProperty("financial_goals_completed").GetInt32());
        Assert.Equal(35, goals.GetProperty("financial_goals_purchase_cost_total").GetInt32());
        Assert.Equal(expectedGoalTokens, components.GetProperty("saving_and_goal_actions").GetInt32());
        Assert.Equal(expectedTotalTokens, components.GetProperty("total_main_actions").GetInt32());
        Assert.Equal((expectedGoalTokens + 2d) / expectedTotalTokens * 100,
            derived.RootElement.GetProperty("long_term_action_share_percent").GetDouble(), 6);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 15)]
    [InlineData(35, 0)]
    [InlineData(35, 5)]
    public void Build_SeparatesGoalPurchaseCostsFromUnfinishedSavings(int purchaseCost, int pendingSavings)
    {
        var sessionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var events = new List<EventDb>();
        if (purchaseCost > 0)
        {
            events.Add(CreateEvent(Guid.NewGuid(), sessionId, playerId, "Menabung",
                JsonSerializer.Serialize(new { goal_id = "purchased", amount = purchaseCost }), turn: 1, sequence: 1));
            events.Add(CreateEvent(Guid.NewGuid(), sessionId, playerId, "TujuanFinansial",
                JsonSerializer.Serialize(new { goal_id = "purchased", cost = purchaseCost, points = 10 }), turn: 1, sequence: 2));
        }
        if (pendingSavings > 0)
        {
            events.Add(CreateEvent(Guid.NewGuid(), sessionId, playerId, "Menabung",
                JsonSerializer.Serialize(new { goal_id = "unfinished", amount = pendingSavings }), turn: 2, sequence: 3));
        }
        var happiness = new AnalyticsHappinessBreakdown(0, 0, 0, 0, 0, 0, 0, 0, 0, false);
        var snapshot = new GameplaySnapshotBuilder().Build(events, [], events, BuildAdvancedConfig(), happiness);
        using var doc = JsonDocument.Parse(snapshot.RawJson);
        var goals = doc.RootElement.GetProperty("financial_goals");
        Assert.Equal(purchaseCost > 0 ? 1 : 0, goals.GetProperty("financial_goals_completed").GetInt32());
        Assert.Equal(purchaseCost, goals.GetProperty("financial_goals_purchase_cost_total").GetInt32());
        Assert.Equal(pendingSavings, goals.GetProperty("financial_goals_incomplete_coins_wasted").GetInt32());
    }

    [Theory]
    [InlineData("MAHIR", 1, 0, 0, 0, 0, 0, 0d)]
    [InlineData("MAHIR", 1, 1, 1, 1, 1, 1, 100d)]
    [InlineData("PEMULA", 1, 1, 1, 1, 1, 99, 100d)]
    [InlineData("MAHIR", 1, 0, 5, 5, 0, 35, 47.637051)]
    [InlineData("MAHIR", 0, 0, 0, 0, 0, 0, null)]
    public void Build_HappinessDiversityUsesPositiveSourcesAndExcludesPenalties(
        string mode, double needs, double bonus, double donation, double gold, double pension, double goals, double? expected)
    {
        var happiness = new AnalyticsHappinessBreakdown(
            needs + bonus + donation + gold + pension + goals - 25,
            needs, bonus, donation, gold, pension, goals, 10, 15, true);
        var snapshot = new GameplaySnapshotBuilder().Build([], [], [], BuildAdvancedConfig() with { Mode = mode }, happiness);
        using var doc = JsonDocument.Parse(snapshot.DerivedJson);
        var diversity = doc.RootElement.GetProperty("happiness_source_diversity_percent");
        if (expected.HasValue) Assert.Equal(expected.Value, diversity.GetDouble(), 5);
        else Assert.Equal(JsonValueKind.Null, diversity.ValueKind);
        Assert.Equal(happiness.Total, doc.RootElement.GetProperty("happiness_points_composition").GetProperty("total_happiness_points").GetDouble());
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Build_CreatesRawAndDerivedGameplayJson` dengan hasil bertipe `void`; operasi ini menangani build creates raw dan derived
    // gameplay JSON.
    public void Build_CreatesRawAndDerivedGameplayJson()
    // Membuka scope metode Build_CreatesRawAndDerivedGameplayJson; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Build_CreatesRawAndDerivedGameplayJson.
    {
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `freelanceEventId` untuk nilai freelance event identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var freelanceEventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `donationEventId` untuk nilai donasi event identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var donationEventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `riskEventId` untuk nilai risiko event identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var riskEventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `playerEvents` untuk nilai pemain event dengan objek baru bertipe `List<EventDb>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playerEvents = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `freelanceEventId`, `sessionId`, `playerId`, `”KerjaLepas”`, `”””{”amount”:10}”””`,
            // `1`, `1` dalam Build_CreatesRawAndDerivedGameplayJson.
            CreateEvent(freelanceEventId, sessionId, playerId, "KerjaLepas", """{"amount":10}""", turn: 1, sequence: 1),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `donationEventId`, `sessionId`, `playerId`, `”JumatBerkah”`, `”””{”amount”:2}”””`,
            // `1`, `2`, `”FRI”` dalam Build_CreatesRawAndDerivedGameplayJson.
            CreateEvent(donationEventId, sessionId, playerId, "JumatBerkah", """{"amount":2}""", turn: 1, sequence: 2, weekday: "FRI"),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `riskEventId`, `sessionId`, `playerId`, `”RisikoKehidupan”`,
            // `”””{”risk_id”:”risk-a”,”direction”:”OUT”,”amount”:3}”””`, `2`, `3` dalam Build_CreatesRawAndDerivedGameplayJson.
            CreateEvent(riskEventId, sessionId, playerId, "RisikoKehidupan", """{"risk_id":"risk-a","direction":"OUT","amount":3}""", turn: 2, sequence: 3)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        };
        // Menyiapkan variabel lokal `allEvents` untuk nilai all event dengan mematerialisasi urutan `playerEvents .Concat(new[] {
        // CreateEvent(Guid.NewGuid(), sessionId, null, ”AkhiriSesi”, ”{}”, turn: 3, sequence: 4) })` menjadi List; enumerasi dijalankan dan hasilnya
        // disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var allEvents = playerEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Concat(new[] { CreateEvent(Guid.NewGuid(), sessionId, null, ”AkhiriSesi”,
            // ”{}”, turn: 3, sequence: 4) }) dalam Build_CreatesRawAndDerivedGameplayJson; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .Concat(new[] { CreateEvent(Guid.NewGuid(), sessionId, null, "AkhiriSesi", "{}", turn: 3, sequence: 4) })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam Build_CreatesRawAndDerivedGameplayJson; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `projections` untuk proyeksi transaksi arus kas yang diturunkan dari event permainan dengan objek baru bertipe
        // `List<CashflowProjectionDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projections = new List<CashflowProjectionDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `freelanceEventId`, `sessionId`, `playerId`, `”IN”`, `10`, `”FREELANCE”` dalam
            // Build_CreatesRawAndDerivedGameplayJson.
            CreateProjection(freelanceEventId, sessionId, playerId, "IN", 10, "FREELANCE"),
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `donationEventId`, `sessionId`, `playerId`, `”OUT”`, `2`, `”DONATION”` dalam
            // Build_CreatesRawAndDerivedGameplayJson.
            CreateProjection(donationEventId, sessionId, playerId, "OUT", 2, "DONATION"),
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `riskEventId`, `sessionId`, `playerId`, `”OUT”`, `3`, `”RISK_LIFE”` dalam
            // Build_CreatesRawAndDerivedGameplayJson.
            CreateProjection(riskEventId, sessionId, playerId, "OUT", 3, "RISK_LIFE")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        };
        // Menyiapkan variabel lokal `happiness` untuk nilai kebahagiaan dengan objek baru bertipe `AnalyticsHappinessBreakdown` dengan argumen ( Total: 4,
        // NeedPoints: 1, NeedSetBonusPoints: 0, DonationPoints: 2, GoldPoints: 0, PensionPoints: 1, SavingGoalPointsEffective: 0, MissionPenaltyPoints: 0,
        // Loa.... Tipe variabel disimpulkan dari ekspresi nilai awal.
        var happiness = new AnalyticsHappinessBreakdown(
            // Meneruskan nilai literal `4` sebagai argumen bernama `Total`.
            Total: 4,
            // Meneruskan nilai literal `1` sebagai argumen bernama `NeedPoints`.
            NeedPoints: 1,
            // Meneruskan nilai literal `0` sebagai argumen bernama `NeedSetBonusPoints`.
            NeedSetBonusPoints: 0,
            // Meneruskan nilai literal `2` sebagai argumen bernama `DonationPoints`.
            DonationPoints: 2,
            // Meneruskan nilai literal `0` sebagai argumen bernama `GoldPoints`.
            GoldPoints: 0,
            // Meneruskan nilai literal `1` sebagai argumen bernama `PensionPoints`.
            PensionPoints: 1,
            // Meneruskan nilai literal `0` sebagai argumen bernama `SavingGoalPointsEffective`.
            SavingGoalPointsEffective: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `MissionPenaltyPoints`.
            MissionPenaltyPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `LoanPenaltyPoints`.
            LoanPenaltyPoints: 0,
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen bernama `HasUnpaidLoan`.
            HasUnpaidLoan: false);

        // Menyiapkan variabel lokal `finalScore` untuk nilai akhir skor dengan objek baru bertipe `SessionFinalScoreDb` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var finalScore = new SessionFinalScoreDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        {
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam Build_CreatesRawAndDerivedGameplayJson.
            UserId = playerId,
            // Memperbarui `PlayerOrder` menggunakan nilai literal `1` dalam Build_CreatesRawAndDerivedGameplayJson.
            PlayerOrder = 1,
            // Memperbarui `Rank` menggunakan nilai literal `2` dalam Build_CreatesRawAndDerivedGameplayJson.
            Rank = 2,
            // Memperbarui `TotalPoints` menggunakan `happiness.Total` (nilai total) dalam Build_CreatesRawAndDerivedGameplayJson.
            TotalPoints = happiness.Total,
            // Memperbarui `PensionPoints` menggunakan `happiness.PensionPoints` (nilai pension poin) dalam Build_CreatesRawAndDerivedGameplayJson.
            PensionPoints = happiness.PensionPoints
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        };
        // Menyiapkan variabel lokal `snapshot` untuk nilai snapshot keadaan dengan memanggil `new GameplaySnapshotBuilder().Build` dengan `playerEvents`,
        // `projections`, `allEvents`, `BuildAdvancedConfig()`, `happiness`, `finalScore`, `4`, `”Marco”`, `true`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var snapshot = new GameplaySnapshotBuilder().Build(
            // Meneruskan `playerEvents` (nilai pemain event) sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            playerEvents,
            // Meneruskan `projections` (proyeksi transaksi arus kas yang diturunkan dari event permainan) sebagai argumen ke `new
            // GameplaySnapshotBuilder().Build`.
            projections,
            // Meneruskan `allEvents` (nilai all event) sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            allEvents,
            // Meneruskan memanggil `BuildAdvancedConfig` dengan tanpa argumen sebagai argumen bernama `config`.
            config: BuildAdvancedConfig(),
            // Meneruskan `happiness` (nilai kebahagiaan) sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            happiness,
            // Meneruskan `finalScore` (nilai akhir skor) sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            finalScore,
            // Meneruskan nilai literal `4` sebagai argumen bernama `pensionRank`.
            pensionRank: 4,
            // Meneruskan nilai literal `”Marco”` sebagai argumen bernama `playerAlias`.
            playerAlias: "Marco",
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `sessionEnded`.
            sessionEnded: true);

        // Menyiapkan variabel lokal `rawDoc` untuk nilai raw doc dengan memanggil `JsonDocument.Parse` dengan `snapshot.RawJson`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var rawDoc = JsonDocument.Parse(snapshot.RawJson);
        // Menyiapkan variabel lokal `derivedDoc` untuk nilai derived doc dengan memanggil `JsonDocument.Parse` dengan `snapshot.DerivedJson`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var derivedDoc = JsonDocument.Parse(snapshot.DerivedJson);
        // Menyiapkan variabel lokal `raw` untuk nilai raw dengan `rawDoc.RootElement` (nilai root element). Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var raw = rawDoc.RootElement;
        // Menyiapkan variabel lokal `derived` untuk nilai derived dengan `derivedDoc.RootElement` (nilai root element). Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var derived = derivedDoc.RootElement;

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`sessionId`,
        // `raw.GetProperty(”metadata”).GetProperty(”session_id”).GetGuid()`); pengujian gagal jika keduanya berbeda dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        Assert.Equal(sessionId, raw.GetProperty("metadata").GetProperty("session_id").GetGuid());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`playerId`,
        // `raw.GetProperty(”metadata”).GetProperty(”user_id”).GetGuid()`); pengujian gagal jika keduanya berbeda dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        Assert.Equal(playerId, raw.GetProperty("metadata").GetProperty("user_id").GetGuid());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Marco”`,
        // `raw.GetProperty(”metadata”).GetProperty(”player_alias”).GetString()`); pengujian gagal jika keduanya berbeda dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        Assert.Equal("Marco", raw.GetProperty("metadata").GetProperty("player_alias").GetString());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`15`,
        // `raw.GetProperty(”coins”).GetProperty(”coins_net_end_game”).GetDouble()`); pengujian gagal jika keduanya berbeda dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        Assert.Equal(15, raw.GetProperty("coins").GetProperty("coins_net_end_game").GetDouble());
        // Menjalankan pemeriksaan bahwa `raw.GetProperty(”outcomes”).GetProperty(”finish_line_reached”).GetBoolean()` bernilai salah; pengujian gagal jika
        // kondisi justru terpenuhi dalam Build_CreatesRawAndDerivedGameplayJson.
        Assert.False(raw.GetProperty("outcomes").GetProperty("finish_line_reached").GetBoolean());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`,
        // `raw.GetProperty(”outcomes”).GetProperty(”final_rank”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        Assert.Equal(2, raw.GetProperty("outcomes").GetProperty("final_rank").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`,
        // `raw.GetProperty(”pension”).GetProperty(”pension_fund_rank_per_game”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        Assert.Equal(4, raw.GetProperty("pension").GetProperty("pension_fund_rank_per_game").GetInt32());
        // Menjalankan pemeriksaan bahwa `raw.GetProperty(”outcomes”).GetProperty(”winner_flag”).GetBoolean()` bernilai salah; pengujian gagal jika kondisi
        // justru terpenuhi dalam Build_CreatesRawAndDerivedGameplayJson.
        Assert.False(raw.GetProperty("outcomes").GetProperty("winner_flag").GetBoolean());
        // Menjalankan pemeriksaan bahwa `raw.GetProperty(”outcomes”).GetProperty(”dnf_flag”).GetBoolean()` bernilai benar; pengujian gagal jika kondisi
        // tidak terpenuhi dalam Build_CreatesRawAndDerivedGameplayJson.
        Assert.True(raw.GetProperty("outcomes").GetProperty("dnf_flag").GetBoolean());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`,
        // `raw.GetProperty(”life_risk”).GetProperty(”life_risk_cards_drawn”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        Assert.Equal(1, raw.GetProperty("life_risk").GetProperty("life_risk_cards_drawn").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`,
        // `raw.GetProperty(”life_risk”).GetProperty(”life_risk_costs_total”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        Assert.Equal(3, raw.GetProperty("life_risk").GetProperty("life_risk_costs_total").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”KerjaLepas”`,
        // `raw.GetProperty(”actions”).GetProperty(”action_sequence”)[0].GetProperty(”actions”)[0].GetString()`); pengujian gagal jika keduanya berbeda
        // dalam Build_CreatesRawAndDerivedGameplayJson.
        Assert.Equal("KerjaLepas", raw.GetProperty("actions").GetProperty("action_sequence")[0].GetProperty("actions")[0].GetString());
        // Menjalankan pemeriksaan hasil dengan `Assert.Empty` menggunakan
        // `raw.GetProperty(”ingredients”).GetProperty(”ingredients_used_per_meal”).EnumerateArray()`; ketidaksesuaian dengan ekspektasi membuat pengujian
        // gagal dalam Build_CreatesRawAndDerivedGameplayJson.
        Assert.Empty(raw.GetProperty("ingredients").GetProperty("ingredients_used_per_meal").EnumerateArray());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`,
        // `raw.GetProperty(”turns”).GetProperty(”day_when_first_risk_hit”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        Assert.Equal(1, raw.GetProperty("turns").GetProperty("day_when_first_risk_hit").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`,
        // `raw.GetProperty(”turns”).GetProperty(”day_game_completion”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        Assert.Equal(1, raw.GetProperty("turns").GetProperty("day_game_completion").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`100`,
        // `derived.GetProperty(”risk_readiness_percent”).GetDouble()`); pengujian gagal jika keduanya berbeda dalam Build_CreatesRawAndDerivedGameplayJson.
        Assert.Equal(100, derived.GetProperty("risk_readiness_percent").GetDouble());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`,
        // `derived.GetProperty(”income_diversification_index”).GetDouble()`); pengujian gagal jika keduanya berbeda dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        Assert.Equal(0, derived.GetProperty("income_diversification_index").GetDouble());

        // Menjalankan memanggil `AssertProperties` dengan `raw.GetProperty(”coins”)`, `”starting_coins”`, `”coins_held_current”`, `”coins_spent_per_turn”`,
        // `”coins_earned_per_turn”`, `”coins_donated”`, `”coins_saved”`, `”coins_net_end_game”` dalam Build_CreatesRawAndDerivedGameplayJson.
        AssertProperties(raw.GetProperty("coins"), "starting_coins", "coins_held_current", "coins_spent_per_turn", "coins_earned_per_turn", "coins_donated", "coins_saved", "coins_net_end_game");
        // Menjalankan memanggil `AssertProperties` dengan `raw.GetProperty(”ingredients”)`, `”ingredients_collected”`, `”ingredients_held_current”`,
        // `”ingredient_types_held”`, `”ingredients_used_per_meal”`, `”ingredients_wasted”`, `”ingredient_investment_coins_total”` dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        AssertProperties(raw.GetProperty("ingredients"), "ingredients_collected", "ingredients_held_current", "ingredient_types_held", "ingredients_used_per_meal", "ingredients_wasted", "ingredient_investment_coins_total");
        // Menjalankan memanggil `AssertProperties` dengan `raw.GetProperty(”meal_orders”)`, `”meal_orders_claimed”`, `”meal_order_income_per_order”`,
        // `”meal_order_income_total”`, `”meal_orders_per_turn_average”` dalam Build_CreatesRawAndDerivedGameplayJson.
        AssertProperties(raw.GetProperty("meal_orders"), "meal_orders_claimed", "meal_order_income_per_order", "meal_order_income_total", "meal_orders_per_turn_average");
        // Menjalankan memanggil `AssertProperties` dengan `raw.GetProperty(”needs”)`, `”need_cards_purchased”`, `”need_cards_owned_current”`,
        // `”primary_needs_owned”`, `”secondary_needs_owned”`, `”tertiary_needs_owned”`, `”specific_tertiary_need”`, `”collection_mission_complete”`,
        // `”need_cards_coins_spent”` dalam Build_CreatesRawAndDerivedGameplayJson.
        AssertProperties(raw.GetProperty("needs"), "need_cards_purchased", "need_cards_owned_current", "primary_needs_owned", "secondary_needs_owned", "tertiary_needs_owned", "specific_tertiary_need", "collection_mission_complete", "need_cards_coins_spent");
        // Menjalankan memanggil `AssertProperties` dengan `raw.GetProperty(”donations”)`, `”donation_amount_per_friday”`, `”donation_rank_per_friday”`,
        // `”donation_total_coins”`, `”donation_champion_cards_earned”`, `”donation_happiness_points”` dalam Build_CreatesRawAndDerivedGameplayJson.
        AssertProperties(raw.GetProperty("donations"), "donation_amount_per_friday", "donation_rank_per_friday", "donation_total_coins", "donation_champion_cards_earned", "donation_happiness_points");
        // Menjalankan memanggil `AssertProperties` dengan `raw.GetProperty(”gold”)`, `”gold_cards_initial”`, `”gold_cards_purchased”`, `”gold_cards_sold”`,
        // `”gold_cards_held_end”`, `”gold_prices_per_purchase”`, `”gold_price_per_sale”`, `”gold_investment_coins_spent”`,
        // `”gold_investment_coins_earned”`, `”gold_investment_net”` dalam Build_CreatesRawAndDerivedGameplayJson.
        AssertProperties(raw.GetProperty("gold"), "gold_cards_initial", "gold_cards_purchased", "gold_cards_sold", "gold_cards_held_end", "gold_prices_per_purchase", "gold_price_per_sale", "gold_investment_coins_spent", "gold_investment_coins_earned", "gold_investment_net");
        // Menjalankan memanggil `AssertProperties` dengan `raw.GetProperty(”pension”)`, `”leftover_coins_end_game”`, `”ingredient_cards_value_end”`,
        // `”coins_in_savings_goal”`, `”pension_fund_total”`, `”pension_fund_rank_per_game”`, `”pension_fund_happiness_points”` dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        AssertProperties(raw.GetProperty("pension"), "leftover_coins_end_game", "ingredient_cards_value_end", "coins_in_savings_goal", "pension_fund_total", "pension_fund_rank_per_game", "pension_fund_happiness_points");
        // Menjalankan memanggil `AssertProperties` dengan `raw.GetProperty(”life_risk”)`, `”life_risk_cards_drawn”`, `”life_risk_costs_per_card”`,
        // `”life_risk_costs_total”`, `”life_risk_mitigated_with_insurance”`, `”insurance_payments_made”`, `”emergency_options_used”` dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        AssertProperties(raw.GetProperty("life_risk"), "life_risk_cards_drawn", "life_risk_costs_per_card", "life_risk_costs_total", "life_risk_mitigated_with_insurance", "insurance_payments_made", "emergency_options_used");
        // Menjalankan memanggil `AssertProperties` dengan `raw.GetProperty(”financial_goals”)`, `”financial_goals_attempted”`,
        // `”financial_goals_completed”`, `”financial_goals_coins_per_goal”`, `”financial_goals_balance_per_goal”`,
        // `”financial_goals_coins_total_invested”`, `”financial_goals_incomplete_coins_wasted”`, `”sharia_loans_taken”`, `”sharia_loans_repaid”`,
        // `”sharia_loans_unpaid_end”`, `”loan_penalty_if_unpaid”` dalam Build_CreatesRawAndDerivedGameplayJson.
        AssertProperties(raw.GetProperty("financial_goals"), "financial_goals_attempted", "financial_goals_completed", "financial_goals_coins_per_goal", "financial_goals_balance_per_goal", "financial_goals_coins_total_invested", "financial_goals_incomplete_coins_wasted", "sharia_loans_taken", "sharia_loans_repaid", "sharia_loans_unpaid_end", "loan_penalty_if_unpaid");
        // Menjalankan memanggil `AssertProperties` dengan `raw.GetProperty(”actions”)`, `”actions_per_turn”`, `”action_repetitions_per_turn”`,
        // `”action_sequence”` dalam Build_CreatesRawAndDerivedGameplayJson.
        AssertProperties(raw.GetProperty("actions"), "actions_per_turn", "action_repetitions_per_turn", "action_sequence");
        // Menjalankan memanggil `AssertProperties` dengan `raw.GetProperty(”turns”)`, `”coins_per_turn_progression”`, `”net_income_per_turn”`,
        // `”day_when_debt_introduced”`, `”day_when_first_risk_hit”`, `”day_game_completion”` dalam Build_CreatesRawAndDerivedGameplayJson.
        AssertProperties(raw.GetProperty("turns"), "coins_per_turn_progression", "net_income_per_turn", "day_when_debt_introduced", "day_when_first_risk_hit", "day_game_completion");
        // Menjalankan memanggil `AssertProperties` dengan `derived`, `”cash_growth_percent”`, `”income_diversification_index”`,
        // `”business_expense_share_percent”`, `”meal_order_profit_margin_percent”`, `”risk_readiness_percent”`, `”loan_burden_percent”`,
        // `”financial_goal_progress_percent”`, `”income_action_focus_percent”`, `”ingredient_utilization_percent”`, `”long_term_action_share_percent”`,
        // `”need_fulfillment_diversity_percent”`, `”donation_commitment_score”`, `”happiness_points_composition”` dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        AssertProperties(derived,
            // Meneruskan nilai literal `”cash_growth_percent”` sebagai argumen ke `AssertProperties`.
            "cash_growth_percent",
            // Meneruskan nilai literal `”income_diversification_index”` sebagai argumen ke `AssertProperties`.
            "income_diversification_index",
            // Meneruskan nilai literal `”business_expense_share_percent”` sebagai argumen ke `AssertProperties`.
            "business_expense_share_percent",
            // Meneruskan nilai literal `”meal_order_profit_margin_percent”` sebagai argumen ke `AssertProperties`.
            "meal_order_profit_margin_percent",
            // Meneruskan nilai literal `”risk_readiness_percent”` sebagai argumen ke `AssertProperties`.
            "risk_readiness_percent",
            // Meneruskan nilai literal `”loan_burden_percent”` sebagai argumen ke `AssertProperties`.
            "loan_burden_percent",
            // Meneruskan nilai literal `”financial_goal_progress_percent”` sebagai argumen ke `AssertProperties`.
            "financial_goal_progress_percent",
            // Meneruskan nilai literal `”income_action_focus_percent”` sebagai argumen ke `AssertProperties`.
            "income_action_focus_percent",
            // Meneruskan nilai literal `”ingredient_utilization_percent”` sebagai argumen ke `AssertProperties`.
            "ingredient_utilization_percent",
            // Meneruskan nilai literal `”long_term_action_share_percent”` sebagai argumen ke `AssertProperties`.
            "long_term_action_share_percent",
            // Meneruskan nilai literal `”need_fulfillment_diversity_percent”` sebagai argumen ke `AssertProperties`.
            "need_fulfillment_diversity_percent",
            // Meneruskan nilai literal `”donation_commitment_score”` sebagai argumen ke `AssertProperties`.
            "donation_commitment_score",
            // Meneruskan nilai literal `”happiness_points_composition”` sebagai argumen ke `AssertProperties`.
            "happiness_points_composition");
        // Menjalankan memanggil `AssertProperties` dengan `derived.GetProperty(”income_action_focus_components”)`, `”income_main_actions”`,
        // `”total_main_actions”` dalam Build_CreatesRawAndDerivedGameplayJson.
        AssertProperties(derived.GetProperty("income_action_focus_components"), "income_main_actions", "total_main_actions");
        // Menjalankan memanggil `AssertProperties` dengan `derived.GetProperty(”long_term_action_share_components”)`, `”saving_actions”`,
        // `”financial_goal_actions”`, `”insurance_actions”`, `”loan_repayment_actions”`, `”total_main_actions”` dalam
        // Build_CreatesRawAndDerivedGameplayJson.
        AssertProperties(derived.GetProperty("long_term_action_share_components"), "saving_actions", "financial_goal_actions", "insurance_actions", "loan_repayment_actions", "total_main_actions");
        // Menjalankan memanggil `AssertProperties` dengan `derived.GetProperty(”donation_commitment_components”)`, `”donation_stability_index”`,
        // `”donated_resource_share”`, `”friday_participation_rate”` dalam Build_CreatesRawAndDerivedGameplayJson.
        AssertProperties(derived.GetProperty("donation_commitment_components"), "donation_stability_index", "donated_resource_share", "friday_participation_rate");
        // Menjalankan memanggil `AssertProperties` dengan `derived.GetProperty(”happiness_points_composition”)`, `”total_happiness_points”`,
        // `”need_card_points”`, `”need_set_bonus_points”`, `”donation_points”`, `”gold_points”`, `”pension_points”`, `”financial_goal_points”`,
        // `”mission_penalty_points”`, `”loan_penalty_points”` dalam Build_CreatesRawAndDerivedGameplayJson.
        AssertProperties(derived.GetProperty("happiness_points_composition"), "total_happiness_points", "need_card_points", "need_set_bonus_points", "donation_points", "gold_points", "pension_points", "financial_goal_points", "mission_penalty_points", "loan_penalty_points");
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`happiness.Total`,
        // `derived.GetProperty(”happiness_points_composition”).GetProperty(”total_happiness_points”).GetDouble()`); pengujian gagal jika keduanya berbeda
        // dalam Build_CreatesRawAndDerivedGameplayJson.
        Assert.Equal(happiness.Total, derived.GetProperty("happiness_points_composition").GetProperty("total_happiness_points").GetDouble());
    // Menutup scope metode Build_CreatesRawAndDerivedGameplayJson; bagian berikut berada di luar batas blok tersebut dalam
    // Build_CreatesRawAndDerivedGameplayJson.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Build_BeginnerModeOmitsAdvancedOnlyDataAndMetrics` dengan hasil bertipe `void`; operasi ini menangani build beginner mode
    // omits advanced only data dan metrics.
    public void Build_BeginnerModeOmitsAdvancedOnlyDataAndMetrics()
    // Membuka scope metode Build_BeginnerModeOmitsAdvancedOnlyDataAndMetrics; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Build_BeginnerModeOmitsAdvancedOnlyDataAndMetrics.
    {
        // Menyiapkan variabel lokal `happiness` untuk nilai kebahagiaan dengan objek baru bertipe `AnalyticsHappinessBreakdown` dengan argumen ( Total: 0,
        // NeedPoints: 0, NeedSetBonusPoints: 0, DonationPoints: 0, GoldPoints: 0, PensionPoints: 0, SavingGoalPointsEffective: 0, MissionPenaltyPoints: 0,
        // Loa.... Tipe variabel disimpulkan dari ekspresi nilai awal.
        var happiness = new AnalyticsHappinessBreakdown(
            // Meneruskan nilai literal `0` sebagai argumen bernama `Total`.
            Total: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `NeedPoints`.
            NeedPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `NeedSetBonusPoints`.
            NeedSetBonusPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `DonationPoints`.
            DonationPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `GoldPoints`.
            GoldPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `PensionPoints`.
            PensionPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `SavingGoalPointsEffective`.
            SavingGoalPointsEffective: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `MissionPenaltyPoints`.
            MissionPenaltyPoints: 0,
            // Meneruskan nilai literal `0` sebagai argumen bernama `LoanPenaltyPoints`.
            LoanPenaltyPoints: 0,
            // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen bernama `HasUnpaidLoan`.
            HasUnpaidLoan: false);
        // Menyiapkan variabel lokal `config` untuk konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan dengan `BuildAdvancedConfig()
        // with { Mode = ”PEMULA”, StartingCash = 20 }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var config = BuildAdvancedConfig() with { Mode = "PEMULA", StartingCash = 20 };

        // Menyiapkan variabel lokal `snapshot` untuk nilai snapshot keadaan dengan memanggil `new GameplaySnapshotBuilder().Build` dengan `[]`, `[]`, `[]`,
        // `config`, `happiness`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var snapshot = new GameplaySnapshotBuilder().Build(
            // Meneruskan koleksi kosong dengan tipe mengikuti konteks tujuan sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            [],
            // Meneruskan koleksi kosong dengan tipe mengikuti konteks tujuan sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            [],
            // Meneruskan koleksi kosong dengan tipe mengikuti konteks tujuan sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            [],
            // Meneruskan `config` (konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan) sebagai argumen ke `new
            // GameplaySnapshotBuilder().Build`.
            config,
            // Meneruskan `happiness` (nilai kebahagiaan) sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            happiness);

        // Menyiapkan variabel lokal `rawDoc` untuk nilai raw doc dengan memanggil `JsonDocument.Parse` dengan `snapshot.RawJson`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var rawDoc = JsonDocument.Parse(snapshot.RawJson);
        // Menyiapkan variabel lokal `derivedDoc` untuk nilai derived doc dengan memanggil `JsonDocument.Parse` dengan `snapshot.DerivedJson`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var derivedDoc = JsonDocument.Parse(snapshot.DerivedJson);
        // Menyiapkan variabel lokal `raw` untuk nilai raw dengan `rawDoc.RootElement` (nilai root element). Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var raw = rawDoc.RootElement;
        // Menyiapkan variabel lokal `derived` untuk nilai derived dengan `derivedDoc.RootElement` (nilai root element). Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var derived = derivedDoc.RootElement;

        // Menjalankan pemeriksaan bahwa `raw.TryGetProperty(”life_risk”, out _)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // Build_BeginnerModeOmitsAdvancedOnlyDataAndMetrics.
        Assert.False(raw.TryGetProperty("life_risk", out _));
        // Menjalankan pemeriksaan bahwa `raw.TryGetProperty(”financial_goals”, out _)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // Build_BeginnerModeOmitsAdvancedOnlyDataAndMetrics.
        Assert.False(raw.TryGetProperty("financial_goals", out _));
        // Menjalankan pemeriksaan bahwa `derived.TryGetProperty(”risk_readiness_percent”, out _)` bernilai salah; pengujian gagal jika kondisi justru
        // terpenuhi dalam Build_BeginnerModeOmitsAdvancedOnlyDataAndMetrics.
        Assert.False(derived.TryGetProperty("risk_readiness_percent", out _));
        // Menjalankan pemeriksaan bahwa `derived.TryGetProperty(”loan_burden_percent”, out _)` bernilai salah; pengujian gagal jika kondisi justru
        // terpenuhi dalam Build_BeginnerModeOmitsAdvancedOnlyDataAndMetrics.
        Assert.False(derived.TryGetProperty("loan_burden_percent", out _));
        // Menjalankan pemeriksaan bahwa `derived.TryGetProperty(”financial_goal_progress_percent”, out _)` bernilai salah; pengujian gagal jika kondisi
        // justru terpenuhi dalam Build_BeginnerModeOmitsAdvancedOnlyDataAndMetrics.
        Assert.False(derived.TryGetProperty("financial_goal_progress_percent", out _));
        // Menjalankan pemeriksaan bahwa `derived.TryGetProperty(”long_term_action_share_percent”, out _)` bernilai salah; pengujian gagal jika kondisi
        // justru terpenuhi dalam Build_BeginnerModeOmitsAdvancedOnlyDataAndMetrics.
        Assert.False(derived.TryGetProperty("long_term_action_share_percent", out _));
        // Menjalankan pemeriksaan bahwa `derived.TryGetProperty(”cash_growth_percent”, out _)` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi
        // dalam Build_BeginnerModeOmitsAdvancedOnlyDataAndMetrics.
        Assert.True(derived.TryGetProperty("cash_growth_percent", out _));
        // Menjalankan pemeriksaan bahwa `derived.TryGetProperty(”happiness_points_composition”, out _)` bernilai benar; pengujian gagal jika kondisi tidak
        // terpenuhi dalam Build_BeginnerModeOmitsAdvancedOnlyDataAndMetrics.
        Assert.True(derived.TryGetProperty("happiness_points_composition", out _));
    // Menutup scope metode Build_BeginnerModeOmitsAdvancedOnlyDataAndMetrics; bagian berikut berada di luar batas blok tersebut dalam
    // Build_BeginnerModeOmitsAdvancedOnlyDataAndMetrics.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost` dengan hasil bertipe `void`; operasi ini menangani build caps
    // keuangan target funding at the attempted target biaya.
    public void Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost()
    // Membuka scope metode Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
    {
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”Menabung”`,
            // `”””{”goal_id”:”goal-a”,”amount”:20}”””`, `1`, `1` dalam Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "Menabung", """{"goal_id":"goal-a","amount":20}""", turn: 1, sequence: 1),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”Menabung”`,
            // `”””{”goal_id”:”goal-a”,”amount”:18}”””`, `2`, `2` dalam Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "Menabung", """{"goal_id":"goal-a","amount":18}""", turn: 2, sequence: 2),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `”TujuanFinansial”`,
            // `”””{”goal_id”:”goal-a”,”points”:10,”cost”:35}”””`, `2`, `3` dalam Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, "TujuanFinansial", """{"goal_id":"goal-a","points":10,"cost":35}""", turn: 2, sequence: 3)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
        };
        // Menyiapkan variabel lokal `config` untuk konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan dengan `BuildAdvancedConfig()
        // with { FinancialGoals = [ new RulesetFinancialGoalDto { Id = ”goal-a”, Nama = ”Goal A”, HargaBeli = 35, PoinKebahagiaan = 10 } ] }`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var config = BuildAdvancedConfig() with
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
        {
            // Memperbarui `FinancialGoals` menggunakan koleksi berisi new RulesetFinancialGoalDto { Id = ”goal-a”, Nama ... dalam
            // Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
            FinancialGoals =
            // Menggunakan koleksi berisi new RulesetFinancialGoalDto { Id = ”goal-a”, Nama ... sebagai bagian ekspresi yang sedang disusun dalam
            // Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
            [
                // Menggunakan objek baru bertipe `RulesetFinancialGoalDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
                new RulesetFinancialGoalDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”goal-a”` dalam Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
                    Id = "goal-a",
                    // Memperbarui `Nama` menggunakan nilai literal `”Goal A”` dalam Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
                    Nama = "Goal A",
                    // Memperbarui `HargaBeli` menggunakan nilai literal `35` dalam Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
                    HargaBeli = 35,
                    // Memperbarui `PoinKebahagiaan` menggunakan nilai literal `10` dalam Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
                    PoinKebahagiaan = 10
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
                // Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
                }
            // Menandai akhir daftar elemen atau indeks koleksi dalam Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost; pasangan kurung siku
            // mengelompokkan nilai sebagai satu struktur.
            ]
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
        };
        // Menyiapkan variabel lokal `happiness` untuk nilai kebahagiaan dengan objek baru bertipe `AnalyticsHappinessBreakdown` dengan argumen (10, 0, 0,
        // 0, 0, 0, 10, 0, 0, false). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var happiness = new AnalyticsHappinessBreakdown(10, 0, 0, 0, 0, 0, 10, 0, 0, false);

        // Menyiapkan variabel lokal `snapshot` untuk nilai snapshot keadaan dengan memanggil `new GameplaySnapshotBuilder().Build` dengan `events`, `[]`,
        // `events`, `config`, `happiness`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var snapshot = new GameplaySnapshotBuilder().Build(
            // Meneruskan `events` (kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan) sebagai argumen ke `new
            // GameplaySnapshotBuilder().Build`.
            events,
            // Meneruskan koleksi kosong dengan tipe mengikuti konteks tujuan sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            [],
            // Meneruskan `events` (kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan) sebagai argumen ke `new
            // GameplaySnapshotBuilder().Build`.
            events,
            // Meneruskan `config` (konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan) sebagai argumen ke `new
            // GameplaySnapshotBuilder().Build`.
            config,
            // Meneruskan `happiness` (nilai kebahagiaan) sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            happiness);

        // Menyiapkan variabel lokal `derivedDoc` untuk nilai derived doc dengan memanggil `JsonDocument.Parse` dengan `snapshot.DerivedJson`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var derivedDoc = JsonDocument.Parse(snapshot.DerivedJson);
        // Menyiapkan variabel lokal `derived` untuk nilai derived dengan `derivedDoc.RootElement` (nilai root element). Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var derived = derivedDoc.RootElement;
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`100`,
        // `derived.GetProperty(”financial_goal_progress_percent”).GetDouble()`); pengujian gagal jika keduanya berbeda dalam
        // Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
        Assert.Equal(100, derived.GetProperty("financial_goal_progress_percent").GetDouble());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`35`,
        // `derived.GetProperty(”financial_goal_progress_components”) .GetProperty(”coins_committed_to_goals”) .GetInt32()`); pengujian gagal jika keduanya
        // berbeda dalam Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
        Assert.Equal(
            // Meneruskan nilai literal `35` sebagai argumen ke `Assert.Equal`.
            35,
            // Meneruskan memanggil `derived.GetProperty(”financial_goal_progress_components”) .GetProperty(”coins_committed_to_goals”) .GetInt32` dengan tanpa
            // argumen sebagai argumen ke `Assert.Equal`; Meneruskan nilai literal `”financial_goal_progress_components”` sebagai argumen ke
            // `derived.GetProperty`.
            derived.GetProperty("financial_goal_progress_components")
                // Meneruskan nilai literal `”coins_committed_to_goals”` sebagai argumen ke `derived.GetProperty(”financial_goal_progress_components”)
                // .GetProperty`.
                .GetProperty("coins_committed_to_goals")
                // Meneruskan memanggil `derived.GetProperty(”financial_goal_progress_components”) .GetProperty(”coins_committed_to_goals”) .GetInt32` dengan tanpa
                // argumen sebagai argumen ke `Assert.Equal`.
                .GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`35`,
        // `derived.GetProperty(”financial_goal_progress_components”) .GetProperty(”attempted_goal_target_total”) .GetInt32()`); pengujian gagal jika
        // keduanya berbeda dalam Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
        Assert.Equal(
            // Meneruskan nilai literal `35` sebagai argumen ke `Assert.Equal`.
            35,
            // Meneruskan memanggil `derived.GetProperty(”financial_goal_progress_components”) .GetProperty(”attempted_goal_target_total”) .GetInt32` dengan
            // tanpa argumen sebagai argumen ke `Assert.Equal`; Meneruskan nilai literal `”financial_goal_progress_components”` sebagai argumen ke
            // `derived.GetProperty`.
            derived.GetProperty("financial_goal_progress_components")
                // Meneruskan nilai literal `”attempted_goal_target_total”` sebagai argumen ke `derived.GetProperty(”financial_goal_progress_components”)
                // .GetProperty`.
                .GetProperty("attempted_goal_target_total")
                // Meneruskan memanggil `derived.GetProperty(”financial_goal_progress_components”) .GetProperty(”attempted_goal_target_total”) .GetInt32` dengan
                // tanpa argumen sebagai argumen ke `Assert.Equal`.
                .GetInt32());
    // Menutup scope metode Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost; bagian berikut berada di luar batas blok tersebut dalam
    // Build_CapsFinancialGoalFundingAtTheAttemptedTargetCost.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”SetupPinjamanAwal”, ”{\”loan_id\”:\”setup-loan\”,\”principal\”:10,\”penalty_points\”:15}”, 0).
    [InlineData("SetupPinjamanAwal", "{\"loan_id\":\"setup-loan\",\"principal\":10,\"penalty_points\":15}", 0)]
    // menyediakan satu kombinasi masukan pengujian (”PinjamanSyariah”, ”{\”loan_id\”:\”regular-loan\”,\”principal\”:10,\”penalty_points\”:15}”, 3).
    [InlineData("PinjamanSyariah", "{\"loan_id\":\"regular-loan\",\"principal\":10,\"penalty_points\":15}", 3)]
    // menyediakan satu kombinasi masukan pengujian (”GunakanOpsiDarurat”,
    // ”{\”option_type\”:\”TAKE_SHARIA_LOAN\”,\”loan_id\”:\”emergency-loan\”,\”principal\”:10,\”penalty_points\”:15}”, 5).
    [InlineData("GunakanOpsiDarurat", "{\"option_type\":\"TAKE_SHARIA_LOAN\",\"loan_id\":\"emergency-loan\",\"principal\":10,\"penalty_points\":15}", 5)]
    // Mendefinisikan metode `Build_ReportsTheFirstDayForEverySupportedLoanSource` dengan hasil bertipe `void`; operasi ini menangani build reports the
    // first hari untuk every supported pinjaman source. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payload`
    // bertipe `string` membawa muatan detail event dalam format JSON; Parameter `dayIndex` bertipe `int` membawa nilai hari index.
    public void Build_ReportsTheFirstDayForEverySupportedLoanSource(
        // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
        string actionType,
        // Parameter `payload` bertipe `string` membawa muatan detail event dalam format JSON.
        string payload,
        // Parameter `dayIndex` bertipe `int` membawa nilai hari index.
        int dayIndex)
    // Membuka scope metode Build_ReportsTheFirstDayForEverySupportedLoanSource; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Build_ReportsTheFirstDayForEverySupportedLoanSource.
    {
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `loanEvent` untuk nilai pinjaman event dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`,
        // `actionType`, `payload`, `dayIndex + 1`, `1`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var loanEvent = CreateEvent(
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke `CreateEvent`.
            Guid.NewGuid(),
            // Meneruskan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke `CreateEvent`.
            sessionId,
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke `CreateEvent`.
            playerId,
            // Meneruskan `actionType` (nilai aksi jenis) sebagai argumen ke `CreateEvent`.
            actionType,
            // Meneruskan `payload` (muatan detail event dalam format JSON) sebagai argumen ke `CreateEvent`.
            payload,
            // Meneruskan penjumlahan/penggabungan antara `dayIndex` dan `1` sebagai argumen bernama `turn`.
            turn: dayIndex + 1,
            // Meneruskan nilai literal `1` sebagai argumen bernama `sequence`.
            sequence: 1);
        // Menyiapkan variabel lokal `happiness` untuk nilai kebahagiaan dengan objek baru bertipe `AnalyticsHappinessBreakdown` dengan argumen (0, 0, 0, 0,
        // 0, 0, 0, 0, 15, true). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var happiness = new AnalyticsHappinessBreakdown(0, 0, 0, 0, 0, 0, 0, 0, 15, true);

        // Menyiapkan variabel lokal `snapshot` untuk nilai snapshot keadaan dengan memanggil `new GameplaySnapshotBuilder().Build` dengan `[loanEvent]`,
        // `[]`, `[loanEvent]`, `BuildAdvancedConfig()`, `happiness`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var snapshot = new GameplaySnapshotBuilder().Build(
            // Meneruskan koleksi berisi loanEvent sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            [loanEvent],
            // Meneruskan koleksi kosong dengan tipe mengikuti konteks tujuan sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            [],
            // Meneruskan koleksi berisi loanEvent sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            [loanEvent],
            // Meneruskan memanggil `BuildAdvancedConfig` dengan tanpa argumen sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            BuildAdvancedConfig(),
            // Meneruskan `happiness` (nilai kebahagiaan) sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            happiness);

        // Menyiapkan variabel lokal `rawDoc` untuk nilai raw doc dengan memanggil `JsonDocument.Parse` dengan `snapshot.RawJson`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var rawDoc = JsonDocument.Parse(snapshot.RawJson);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`dayIndex`,
        // `rawDoc.RootElement.GetProperty(”turns”).GetProperty(”day_when_debt_introduced”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // Build_ReportsTheFirstDayForEverySupportedLoanSource.
        Assert.Equal(
            // Meneruskan `dayIndex` (nilai hari index) sebagai argumen ke `Assert.Equal`.
            dayIndex,
            // Meneruskan memanggil `rawDoc.RootElement.GetProperty(”turns”).GetProperty(”day_when_debt_introduced”).GetInt32` dengan tanpa argumen sebagai
            // argumen ke `Assert.Equal`; Meneruskan nilai literal `”turns”` sebagai argumen ke `rawDoc.RootElement.GetProperty`; Meneruskan nilai literal
            // `”day_when_debt_introduced”` sebagai argumen ke `rawDoc.RootElement.GetProperty(”turns”).GetProperty`.
            rawDoc.RootElement.GetProperty("turns").GetProperty("day_when_debt_introduced").GetInt32());
    // Menutup scope metode Build_ReportsTheFirstDayForEverySupportedLoanSource; bagian berikut berada di luar batas blok tersebut dalam
    // Build_ReportsTheFirstDayForEverySupportedLoanSource.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency` dengan hasil bertipe `void`; operasi ini menangani build
    // risiko readiness counts only hasil resolusi risks tanpa emergency.
    public void Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency()
    // Membuka scope metode Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency.
    {
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `paidRiskId` untuk nilai paid risiko identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var paidRiskId = Guid.NewGuid();
        // Menyiapkan variabel lokal `pendingRiskId` untuk nilai tertunda risiko identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var pendingRiskId = Guid.NewGuid();
        // Menyiapkan variabel lokal `emergencyRiskId` untuk nilai emergency risiko identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var emergencyRiskId = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `paidRiskId`, `sessionId`, `playerId`, `GameActionCatalog.RisikoKehidupan`,
            // `”””{”risk_id”:”paid”}”””`, `1`, `1` dalam Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency.
            CreateEvent(paidRiskId, sessionId, playerId, GameActionCatalog.RisikoKehidupan, """{"risk_id":"paid"}""", 1, 1),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `pendingRiskId`, `sessionId`, `playerId`, `GameActionCatalog.RisikoKehidupan`,
            // `”””{”risk_id”:”pending”}”””`, `2`, `2` dalam Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency.
            CreateEvent(pendingRiskId, sessionId, playerId, GameActionCatalog.RisikoKehidupan, """{"risk_id":"pending"}""", 2, 2),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `emergencyRiskId`, `sessionId`, `playerId`, `GameActionCatalog.RisikoKehidupan`,
            // `”””{”risk_id”:”emergency”}”””`, `3`, `3` dalam Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency.
            CreateEvent(emergencyRiskId, sessionId, playerId, GameActionCatalog.RisikoKehidupan, """{"risk_id":"emergency"}""", 3, 3),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `GameActionCatalog.RiskEmergencyUsed`,
            // `$$”””{”risk_event_id”:”{{emergencyRiskId}}”,”option_type”:”SELL_NEED”}”””`, `3`, `4` dalam
            // Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, GameActionCatalog.RiskEmergencyUsed, $$"""{"risk_event_id":"{{emergencyRiskId}}","option_type":"SELL_NEED"}""", 3, 4)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency.
        };
        // Menyiapkan variabel lokal `projections` untuk proyeksi transaksi arus kas yang diturunkan dari event permainan dengan objek baru bertipe
        // `List<CashflowProjectionDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projections = new List<CashflowProjectionDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateProjection` dengan `paidRiskId`, `sessionId`, `playerId`, `”OUT”`, `3`, `”RISK_LIFE”` dalam
            // Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency.
            CreateProjection(paidRiskId, sessionId, playerId, "OUT", 3, "RISK_LIFE")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency.
        };

        // Menyiapkan variabel lokal `snapshot` untuk nilai snapshot keadaan dengan memanggil `new GameplaySnapshotBuilder().Build` dengan `events`,
        // `projections`, `events`, `BuildAdvancedConfig()`, `new AnalyticsHappinessBreakdown(0, 0, 0, 0, 0, 0, 0, 0, 0, false)`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var snapshot = new GameplaySnapshotBuilder().Build(
            // Meneruskan `events` (kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan) sebagai argumen ke `new
            // GameplaySnapshotBuilder().Build`.
            events,
            // Meneruskan `projections` (proyeksi transaksi arus kas yang diturunkan dari event permainan) sebagai argumen ke `new
            // GameplaySnapshotBuilder().Build`.
            projections,
            // Meneruskan `events` (kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan) sebagai argumen ke `new
            // GameplaySnapshotBuilder().Build`.
            events,
            // Meneruskan memanggil `BuildAdvancedConfig` dengan tanpa argumen sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            BuildAdvancedConfig(),
            // Meneruskan objek baru bertipe `AnalyticsHappinessBreakdown` dengan argumen (0, 0, 0, 0, 0, 0, 0, 0, 0, false) sebagai argumen ke `new
            // GameplaySnapshotBuilder().Build`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor `AnalyticsHappinessBreakdown`; Meneruskan nilai
            // literal `0` sebagai argumen ke konstruktor `AnalyticsHappinessBreakdown`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor
            // `AnalyticsHappinessBreakdown`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor `AnalyticsHappinessBreakdown`; Meneruskan nilai
            // literal `0` sebagai argumen ke konstruktor `AnalyticsHappinessBreakdown`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor
            // `AnalyticsHappinessBreakdown`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor `AnalyticsHappinessBreakdown`; Meneruskan nilai
            // literal `0` sebagai argumen ke konstruktor `AnalyticsHappinessBreakdown`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor
            // `AnalyticsHappinessBreakdown`; Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke konstruktor
            // `AnalyticsHappinessBreakdown`.
            new AnalyticsHappinessBreakdown(0, 0, 0, 0, 0, 0, 0, 0, 0, false));

        // Menyiapkan variabel lokal `derivedDoc` untuk nilai derived doc dengan memanggil `JsonDocument.Parse` dengan `snapshot.DerivedJson`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var derivedDoc = JsonDocument.Parse(snapshot.DerivedJson);
        // Menyiapkan variabel lokal `derived` untuk nilai derived dengan `derivedDoc.RootElement` (nilai root element). Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var derived = derivedDoc.RootElement;
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`,
        // `derived.GetProperty(”risk_readiness_components”).GetProperty(”risks_resolved_without_emergency”).GetInt32()`); pengujian gagal jika keduanya
        // berbeda dalam Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency.
        Assert.Equal(1, derived.GetProperty("risk_readiness_components").GetProperty("risks_resolved_without_emergency").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`100d / 3d`,
        // `derived.GetProperty(”risk_readiness_percent”).GetDouble()`, `8`); pengujian gagal jika keduanya berbeda dalam
        // Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency.
        Assert.Equal(100d / 3d, derived.GetProperty("risk_readiness_percent").GetDouble(), precision: 8);
    // Menutup scope metode Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency; bagian berikut berada di luar batas blok tersebut dalam
    // Build_RiskReadinessCountsOnlyResolvedRisksWithoutEmergency.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Build_LongTermActionShareExcludesFreeInsuranceClaims` dengan hasil bertipe `void`; operasi ini menangani build long term
    // aksi share excludes free asuransi claims.
    public void Build_LongTermActionShareExcludesFreeInsuranceClaims()
    // Membuka scope metode Build_LongTermActionShareExcludesFreeInsuranceClaims; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Build_LongTermActionShareExcludesFreeInsuranceClaims.
    {
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `riskEventId` untuk nilai risiko event identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var riskEventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan objek baru
        // bertipe `List<EventDb>` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = new List<EventDb>
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Build_LongTermActionShareExcludesFreeInsuranceClaims.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `GameActionCatalog.Menabung`,
            // `”””{”amount”:5}”””`, `1`, `1` dalam Build_LongTermActionShareExcludesFreeInsuranceClaims.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, GameActionCatalog.Menabung, """{"amount":5}""", 1, 1),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `GameActionCatalog.Asuransi`,
            // `”””{”premium”:1}”””`, `1`, `2` dalam Build_LongTermActionShareExcludesFreeInsuranceClaims.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, GameActionCatalog.Asuransi, """{"premium":1}""", 1, 2),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `sessionId`, `playerId`, `GameActionCatalog.Asuransi`,
            // `$$”””{”risk_event_id”:”{{riskEventId}}”}”””`, `2`, `3` dalam Build_LongTermActionShareExcludesFreeInsuranceClaims.
            CreateEvent(Guid.NewGuid(), sessionId, playerId, GameActionCatalog.Asuransi, $$"""{"risk_event_id":"{{riskEventId}}"}""", 2, 3)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Build_LongTermActionShareExcludesFreeInsuranceClaims.
        };

        // Menyiapkan variabel lokal `snapshot` untuk nilai snapshot keadaan dengan memanggil `new GameplaySnapshotBuilder().Build` dengan `events`, `[]`,
        // `events`, `BuildAdvancedConfig()`, `new AnalyticsHappinessBreakdown(0, 0, 0, 0, 0, 0, 0, 0, 0, false)`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var snapshot = new GameplaySnapshotBuilder().Build(
            // Meneruskan `events` (kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan) sebagai argumen ke `new
            // GameplaySnapshotBuilder().Build`.
            events,
            // Meneruskan koleksi kosong dengan tipe mengikuti konteks tujuan sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            [],
            // Meneruskan `events` (kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan) sebagai argumen ke `new
            // GameplaySnapshotBuilder().Build`.
            events,
            // Meneruskan memanggil `BuildAdvancedConfig` dengan tanpa argumen sebagai argumen ke `new GameplaySnapshotBuilder().Build`.
            BuildAdvancedConfig(),
            // Meneruskan objek baru bertipe `AnalyticsHappinessBreakdown` dengan argumen (0, 0, 0, 0, 0, 0, 0, 0, 0, false) sebagai argumen ke `new
            // GameplaySnapshotBuilder().Build`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor `AnalyticsHappinessBreakdown`; Meneruskan nilai
            // literal `0` sebagai argumen ke konstruktor `AnalyticsHappinessBreakdown`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor
            // `AnalyticsHappinessBreakdown`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor `AnalyticsHappinessBreakdown`; Meneruskan nilai
            // literal `0` sebagai argumen ke konstruktor `AnalyticsHappinessBreakdown`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor
            // `AnalyticsHappinessBreakdown`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor `AnalyticsHappinessBreakdown`; Meneruskan nilai
            // literal `0` sebagai argumen ke konstruktor `AnalyticsHappinessBreakdown`; Meneruskan nilai literal `0` sebagai argumen ke konstruktor
            // `AnalyticsHappinessBreakdown`; Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen ke konstruktor
            // `AnalyticsHappinessBreakdown`.
            new AnalyticsHappinessBreakdown(0, 0, 0, 0, 0, 0, 0, 0, 0, false));

        // Menyiapkan variabel lokal `derivedDoc` untuk nilai derived doc dengan memanggil `JsonDocument.Parse` dengan `snapshot.DerivedJson`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var derivedDoc = JsonDocument.Parse(snapshot.DerivedJson);
        // Menyiapkan variabel lokal `derived` untuk nilai derived dengan `derivedDoc.RootElement` (nilai root element). Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var derived = derivedDoc.RootElement;
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`100`,
        // `derived.GetProperty(”long_term_action_share_percent”).GetDouble()`); pengujian gagal jika keduanya berbeda dalam
        // Build_LongTermActionShareExcludesFreeInsuranceClaims.
        Assert.Equal(100, derived.GetProperty("long_term_action_share_percent").GetDouble());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`,
        // `derived.GetProperty(”long_term_action_share_components”).GetProperty(”insurance_actions”).GetInt32()`); pengujian gagal jika keduanya berbeda
        // dalam Build_LongTermActionShareExcludesFreeInsuranceClaims.
        Assert.Equal(1, derived.GetProperty("long_term_action_share_components").GetProperty("insurance_actions").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`,
        // `derived.GetProperty(”long_term_action_share_components”).GetProperty(”total_main_actions”).GetInt32()`); pengujian gagal jika keduanya berbeda
        // dalam Build_LongTermActionShareExcludesFreeInsuranceClaims.
        Assert.Equal(2, derived.GetProperty("long_term_action_share_components").GetProperty("total_main_actions").GetInt32());
    // Menutup scope metode Build_LongTermActionShareExcludesFreeInsuranceClaims; bagian berikut berada di luar batas blok tersebut dalam
    // Build_LongTermActionShareExcludesFreeInsuranceClaims.
    }

    // Mendefinisikan metode `AssertProperties` dengan hasil bertipe `void`; operasi ini menangani assert properties. Masukan: Parameter `element`
    // bertipe `JsonElement` membawa nilai element; Parameter `names` bertipe `string[]` membawa nilai nama.
    private static void AssertProperties(JsonElement element, params string[] names)
    // Membuka scope metode AssertProperties; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AssertProperties.
    {
        // Mengulangi setiap elemen `names`; elemen saat ini disimpan sebagai `name` bertipe `var` untuk diproses oleh badan loop dalam AssertProperties.
        foreach (var name in names)
        // Membuka scope loop setiap name dari `names`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AssertProperties.
        {
            // Menjalankan pemeriksaan bahwa `element.TryGetProperty(name, out _)`, `$”Missing gameplay metric: {name}”` bernilai benar; pengujian gagal jika
            // kondisi tidak terpenuhi dalam AssertProperties.
            Assert.True(element.TryGetProperty(name, out _), $"Missing gameplay metric: {name}");
        // Menutup scope loop setiap name dari `names`; bagian berikut berada di luar batas blok tersebut dalam AssertProperties.
        }
    // Menutup scope metode AssertProperties; bagian berikut berada di luar batas blok tersebut dalam AssertProperties.
    }

    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `EventDb`; operasi ini menangani create event. Masukan: Parameter `eventId` bertipe
    // `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi
    // permainan yang menjadi batas data operasi ini; Parameter `playerId` bertipe `Guid?` membawa nilai pemain identitas; nilai null diizinkan ketika
    // data opsional belum tersedia; Parameter `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payload` bertipe `string` membawa
    // muatan detail event dalam format JSON; Parameter `turn` bertipe `int` membawa giliran pemain yang sedang berlangsung; Parameter `sequence`
    // bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan; Parameter `weekday` bertipe `string` membawa nilai
    // weekday; bila argumen tidak diberikan digunakan nilai literal `”MON”`.
    private static EventDb CreateEvent(
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `playerId` bertipe `Guid?` membawa nilai pemain identitas; nilai null diizinkan ketika data opsional belum tersedia.
        Guid? playerId,
        // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
        string actionType,
        // Parameter `payload` bertipe `string` membawa muatan detail event dalam format JSON.
        string payload,
        // Parameter `turn` bertipe `int` membawa giliran pemain yang sedang berlangsung.
        int turn,
        // Parameter `sequence` bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan.
        long sequence,
        // Parameter `weekday` bertipe `string` membawa nilai weekday; bila argumen tidak diberikan digunakan nilai literal `”MON”`.
        string weekday = "MON")
    // Membuka scope metode CreateEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
    {
        // Mengembalikan objek baru bertipe `EventDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam CreateEvent; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new EventDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
        {
            // Memperbarui `EventId` menggunakan `eventId` (identitas unik event untuk pencatatan dan pemeriksaan duplikasi) dalam CreateEvent.
            EventId = eventId,
            // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam CreateEvent.
            SessionId = sessionId,
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam CreateEvent.
            UserId = playerId,
            // Memperbarui `ActorType` menggunakan hasil pemilihan bersyarat: ketika `playerId.HasValue` benar gunakan `”PLAYER”`, jika tidak gunakan `”SYSTEM”`
            // dalam CreateEvent.
            ActorType = playerId.HasValue ? "PLAYER" : "SYSTEM",
            // Memperbarui `Timestamp` menggunakan memanggil `new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero).AddMinutes` dengan `sequence` dalam
            // CreateEvent.
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero).AddMinutes(sequence),
            // Memperbarui `DayIndex` menggunakan selisih antara `turn` dan `1` dalam CreateEvent.
            DayIndex = turn - 1,
            // Memperbarui `Weekday` menggunakan `weekday` (nilai weekday) dalam CreateEvent.
            Weekday = weekday,
            // Memperbarui `ActionSlot` menggunakan `turn` (giliran pemain yang sedang berlangsung) dalam CreateEvent.
            ActionSlot = turn,
            // Memperbarui `SequenceNumber` menggunakan `sequence` (nomor urut event yang menentukan urutan pemrosesan riwayat permainan) dalam CreateEvent.
            SequenceNumber = sequence,
            // Memperbarui `ActionType` menggunakan `actionType` (nilai aksi jenis) dalam CreateEvent.
            ActionType = actionType,
            // Memperbarui `RulesetVersionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
            RulesetVersionId = Guid.NewGuid(),
            // Memperbarui `Payload` menggunakan `payload` (muatan detail event dalam format JSON) dalam CreateEvent.
            Payload = payload
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
        };
    // Menutup scope metode CreateEvent; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
    }

    // Mendefinisikan metode `CreateProjection` dengan hasil bertipe `CashflowProjectionDb`; operasi ini menangani create projection. Masukan: Parameter
    // `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter `sessionId` bertipe `Guid` membawa
    // identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas; Parameter
    // `direction` bertipe `string` membawa nilai direction; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai
    // dalam operasi; Parameter `category` bertipe `string` membawa nilai category.
    private static CashflowProjectionDb CreateProjection(
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
        Guid playerId,
        // Parameter `direction` bertipe `string` membawa nilai direction.
        string direction,
        // Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi.
        int amount,
        // Parameter `category` bertipe `string` membawa nilai category.
        string category)
    // Membuka scope metode CreateProjection; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateProjection.
    {
        // Mengembalikan objek baru bertipe `CashflowProjectionDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam CreateProjection; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return new CashflowProjectionDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateProjection.
        {
            // Memperbarui `ProjectionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateProjection.
            ProjectionId = Guid.NewGuid(),
            // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam CreateProjection.
            SessionId = sessionId,
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam CreateProjection.
            UserId = playerId,
            // Memperbarui `EventPk` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateProjection.
            EventPk = Guid.NewGuid(),
            // Memperbarui `EventId` menggunakan `eventId` (identitas unik event untuk pencatatan dan pemeriksaan duplikasi) dalam CreateProjection.
            EventId = eventId,
            // Memperbarui `Timestamp` menggunakan objek baru bertipe `DateTimeOffset` dengan argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero) dalam
            // CreateProjection.
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            // Memperbarui `Direction` menggunakan `direction` (nilai direction) dalam CreateProjection.
            Direction = direction,
            // Memperbarui `Amount` menggunakan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam CreateProjection.
            Amount = amount,
            // Memperbarui `Category` menggunakan `category` (nilai category) dalam CreateProjection.
            Category = category
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateProjection.
        };
    // Menutup scope metode CreateProjection; bagian berikut berada di luar batas blok tersebut dalam CreateProjection.
    }

    // Mendefinisikan metode `BuildAdvancedConfig` dengan hasil bertipe `RulesetConfig`; operasi ini menangani build advanced konfigurasi. Nilai hasil
    // langsung berasal dari objek baru dengan tipe mengikuti konteks tujuan dan argumen ( ”MAHIR”, 2, 10, PlayerOrdering.PlayerOrder, 0, 6, 3, 1, true,
    // true, true, true, 1, 999, true, true, true, true, true, 1, null).
    private static RulesetConfig BuildAdvancedConfig() => new(
        // Meneruskan nilai literal `”MAHIR”` sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        "MAHIR",
        // Meneruskan nilai literal `2` sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        2,
        // Meneruskan nilai literal `10` sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        10,
        // Meneruskan `PlayerOrdering.PlayerOrder` (nomor urut pemain untuk menentukan urutan tindakan) sebagai argumen ke konstruktor dengan tipe mengikuti
        // konteks.
        PlayerOrdering.PlayerOrder,
        // Meneruskan nilai literal `0` sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        0,
        // Meneruskan nilai literal `6` sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        6,
        // Meneruskan nilai literal `3` sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        3,
        // Meneruskan nilai literal `1` sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        1,
        // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        true,
        // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        true,
        // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        true,
        // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        true,
        // Meneruskan nilai literal `1` sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        1,
        // Meneruskan nilai literal `999` sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        999,
        // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        true,
        // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        true,
        // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        true,
        // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        true,
        // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        true,
        // Meneruskan nilai literal `1` sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        1,
        // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        null);
// Menutup scope tipe AnalyticsGameplaySnapshotBuilderTests; bagian berikut berada di luar batas blok tersebut.
}
