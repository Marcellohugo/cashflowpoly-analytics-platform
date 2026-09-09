// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui RulesetDefinitionMapperTests.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `RulesetDefinitionMapperTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetDefinitionMapperTests
// Membuka scope tipe RulesetDefinitionMapperTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    [Fact]
    public void ConfigRoundTrip_PreservesActionsReferencedByNarratives()
    {
        var definition = new RulesetDefinitionDto
        {
            Actions = [new RulesetActionDto { ActionId = "JualMasakan" }],
            Needs = [new RulesetNeedDto { Id = "buku_1", Family = "buku" }],
            Narratives = [new RulesetNarrativeDto
            {
                Id = "first_sale", PrerequisiteAksi = [new RulesetNarrativePrerequisiteDto { Aksi = "JualMasakan", Value = 1 }]
            }]
        };
        var roundTrip = RulesetDefinitionMapper.FromConfigJson(RulesetDefinitionMapper.ToConfigJson(definition));
        Assert.Equal("JualMasakan", Assert.Single(roundTrip.Actions).ActionId);
        Assert.Equal("buku", Assert.Single(roundTrip.Needs).Family);
        Assert.Equal("JualMasakan", Assert.Single(Assert.Single(roundTrip.Narratives).PrerequisiteAksi).Aksi);
    }
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `FromConfigJson_IgnoresRemovedQuestAndScripts_AndKeepsDataDrivenNarrativeConditions` dengan hasil bertipe `void`; operasi
    // ini menangani dari konfigurasi JSON ignores removed quest dan scripts dan keeps data driven narrative conditions.
    public void FromConfigJson_IgnoresRemovedQuestAndScripts_AndKeepsDataDrivenNarrativeConditions()
    // Membuka scope metode FromConfigJson_IgnoresRemovedQuestAndScripts_AndKeepsDataDrivenNarrativeConditions; pernyataan/deklarasi berikut berada di
    // dalam batas blok ini dalam FromConfigJson_IgnoresRemovedQuestAndScripts_AndKeepsDataDrivenNarrativeConditions.
    {
        // Menyiapkan variabel lokal `configJson` untuk nilai konfigurasi JSON dengan literal multiline yang dirinci pada komentar di dekat deklarasinya.
        // Tipe yang dipakai adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string configJson =
        // ”””`.
        // Baris literal 2: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{`.
        // Baris literal 3: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”mode”: ”MAHIR”,`.
        // Baris literal 4: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”actions_per_turn”: 2,`.
        // Baris literal 5: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”starting_cash”: 10,`.
        // Baris literal 6: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”player_ordering”: ”PLAYER_ORDER”,`.
        // Baris literal 7: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”weekday_rules”: {`.
        // Baris literal 8: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”friday”: { ”feature”: ”DONATION”,
        // ”enabled”: true },`.
        // Baris literal 9: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”saturday”: { ”feature”: ”GOLD_TRADE”,
        // ”enabled”: true },`.
        // Baris literal 10: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”sunday”: { ”feature”: ”REST”, ”enabled”:
        // true }`.
        // Baris literal 11: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `},`.
        // Baris literal 12: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”constraints”: {`.
        // Baris literal 13: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”cash_min”: 0,`.
        // Baris literal 14: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”max_ingredient_total”: 6,`.
        // Baris literal 15: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”max_same_ingredient”: 3,`.
        // Baris literal 16: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”primary_need_max_per_day”: 1,`.
        // Baris literal 17: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”require_primary_before_others”: true`.
        // Baris literal 18: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `},`.
        // Baris literal 19: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”donation”: { ”min_amount”: 1,
        // ”max_amount”: 999999 },`.
        // Baris literal 20: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”gold_trade”: { ”allow_buy”: true,
        // ”allow_sell”: true },`.
        // Baris literal 21: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”advanced”: {`.
        // Baris literal 22: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”loan”: { ”enabled”: true },`.
        // Baris literal 23: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”insurance”: { ”enabled”: true },`.
        // Baris literal 24: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”saving_goal”: { ”enabled”: true }`.
        // Baris literal 25: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `},`.
        // Baris literal 26: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”component_catalog”: {`.
        // Baris literal 27: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”gameConfig”: {`.
        // Baris literal 28: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”initialCoins”: 10,`.
        // Baris literal 29: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”initialHappiness”: 0,`.
        // Baris literal 30: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”initialSaving”: 0,`.
        // Baris literal 31: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”finishDay”: 25,`.
        // Baris literal 32: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”minPlayers”: 2,`.
        // Baris literal 33: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”maxPlayers”: 4`.
        // Baris literal 34: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `},`.
        // Baris literal 35: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”bahan”: [],`.
        // Baris literal 36: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”resep”: [],`.
        // Baris literal 37: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”kebutuhan”: [],`.
        // Baris literal 38: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”targetKebutuhan”: [],`.
        // Baris literal 39: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”tujuanFinansial”: [],`.
        // Baris literal 40: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”narasi”: [`.
        // Baris literal 41: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{`.
        // Baris literal 42: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”id”: ”jual_pertama”,`.
        // Baris literal 43: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”nama”: ”Penjualan Pertama”,`.
        // Baris literal 44: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”teks”: [ ”baris 1” ],`.
        // Baris literal 45: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”prerequisiteAksi”: [ { ”aksi”:
        // ”JualMasakan”, ”value”: 1 } ],`.
        // Baris literal 46: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”scripts”: [ { ”command”: ”setbackground” }
        // ]`.
        // Baris literal 47: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `}`.
        // Baris literal 48: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `],`.
        // Baris literal 49: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”quest”: [`.
        // Baris literal 50: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{`.
        // Baris literal 51: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”id”: ”jual_3”,`.
        // Baris literal 52: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”nama”: ”Jual tiga masakan”,`.
        // Baris literal 53: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”aksi”: ”JualMasakan”,`.
        // Baris literal 54: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”target”: 3,`.
        // Baris literal 55: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”rewardCoins”: 5,`.
        // Baris literal 56: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”rewardHappiness”: 2,`.
        // Baris literal 57: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”scripts”: [ { ”command”: ”checkAction” }
        // ]`.
        // Baris literal 58: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `}`.
        // Baris literal 59: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `]`.
        // Baris literal 60: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `}`.
        // Baris literal 61: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `}`.
        // Baris literal 62: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string configJson = """
            {
              "mode": "MAHIR",
              "actions_per_turn": 2,
              "starting_cash": 10,
              "player_ordering": "PLAYER_ORDER",
              "weekday_rules": {
                "friday": { "feature": "DONATION", "enabled": true },
                "saturday": { "feature": "GOLD_TRADE", "enabled": true },
                "sunday": { "feature": "REST", "enabled": true }
              },
              "constraints": {
                "cash_min": 0,
                "max_ingredient_total": 6,
                "max_same_ingredient": 3,
                "primary_need_max_per_day": 1,
                "require_primary_before_others": true
              },
              "donation": { "min_amount": 1, "max_amount": 999999 },
              "gold_trade": { "allow_buy": true, "allow_sell": true },
              "advanced": {
                "loan": { "enabled": true },
                "insurance": { "enabled": true },
                "saving_goal": { "enabled": true }
              },
              "component_catalog": {
                "gameConfig": {
                  "initialCoins": 10,
                  "initialHappiness": 0,
                  "initialSaving": 0,
                  "finishDay": 25,
                  "minPlayers": 2,
                  "maxPlayers": 4
                },
                "bahan": [],
                "resep": [],
                "kebutuhan": [],
                "targetKebutuhan": [],
                "tujuanFinansial": [],
                "narasi": [
                  {
                    "id": "jual_pertama",
                    "nama": "Penjualan Pertama",
                    "teks": [ "baris 1" ],
                    "prerequisiteAksi": [ { "aksi": "JualMasakan", "value": 1 } ],
                    "scripts": [ { "command": "setbackground" } ]
                  }
                ],
                "quest": [
                  {
                    "id": "jual_3",
                    "nama": "Jual tiga masakan",
                    "aksi": "JualMasakan",
                    "target": 3,
                    "rewardCoins": 5,
                    "rewardHappiness": 2,
                    "scripts": [ { "command": "checkAction" } ]
                  }
                ]
              }
            }
            """;

        // Menyiapkan variabel lokal `definition` untuk definisi terstruktur komponen serta parameter aturan permainan dengan memanggil
        // `RulesetDefinitionMapper.FromConfigJson` dengan `configJson`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var definition = RulesetDefinitionMapper.FromConfigJson(configJson);

        // Menyiapkan variabel lokal `narrative` untuk nilai narrative dengan pemeriksaan hasil dengan `Assert.Single` menggunakan `definition.Narratives`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var narrative = Assert.Single(definition.Narratives);
        // Menyiapkan variabel lokal `prerequisite` untuk nilai prerequisite dengan pemeriksaan hasil dengan `Assert.Single` menggunakan
        // `narrative.PrerequisiteAksi`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var prerequisite = Assert.Single(narrative.PrerequisiteAksi);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”JualMasakan”`, `prerequisite.Aksi`);
        // pengujian gagal jika keduanya berbeda dalam FromConfigJson_IgnoresRemovedQuestAndScripts_AndKeepsDataDrivenNarrativeConditions.
        Assert.Equal("JualMasakan", prerequisite.Aksi);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`1`, `prerequisite.Value`); pengujian gagal
        // jika keduanya berbeda dalam FromConfigJson_IgnoresRemovedQuestAndScripts_AndKeepsDataDrivenNarrativeConditions.
        Assert.Equal(1, prerequisite.Value);
    // Menutup scope metode FromConfigJson_IgnoresRemovedQuestAndScripts_AndKeepsDataDrivenNarrativeConditions; bagian berikut berada di luar batas blok
    // tersebut dalam FromConfigJson_IgnoresRemovedQuestAndScripts_AndKeepsDataDrivenNarrativeConditions.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog` dengan hasil bertipe `void`; operasi ini menangani
    // ke konfigurasi JSON emits only data driven narrative fields dan omits quest catalog.
    public void ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog()
    // Membuka scope metode ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
    {
        // Menyiapkan variabel lokal `definition` untuk definisi terstruktur komponen serta parameter aturan permainan dengan objek baru bertipe
        // `RulesetDefinitionDto` dengan nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var definition = new RulesetDefinitionDto
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
        {
            // Memperbarui `Mode` menggunakan nilai literal `”MAHIR”` dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
            Mode = "MAHIR",
            // Memperbarui `Settings` menggunakan objek baru bertipe `RulesetSettingsDto` dengan nilai awal sesuai konstruktornya dalam
            // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
            Settings = new RulesetSettingsDto
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
            {
                // Memperbarui `ActionsPerTurn` menggunakan nilai literal `2` dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                ActionsPerTurn = 2,
                // Memperbarui `StartingCash` menggunakan nilai literal `10` dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                StartingCash = 10,
                // Memperbarui `InitialCoins` menggunakan nilai literal `10` dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                InitialCoins = 10,
                // Memperbarui `FinishDay` menggunakan nilai literal `25` dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                FinishDay = 25,
                // Memperbarui `MinPlayers` menggunakan nilai literal `2` dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                MinPlayers = 2,
                // Memperbarui `MaxPlayers` menggunakan nilai literal `4` dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                MaxPlayers = 4
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
            // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
            },
            // Memperbarui `Narratives` menggunakan koleksi berisi new RulesetNarrativeDto { Id = ”jual_pertama”, Nam... dalam
            // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
            Narratives =
            // Menggunakan koleksi berisi new RulesetNarrativeDto { Id = ”jual_pertama”, Nam... sebagai bagian ekspresi yang sedang disusun dalam
            // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
            [
                // Menggunakan objek baru bertipe `RulesetNarrativeDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                new RulesetNarrativeDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”jual_pertama”` dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                    Id = "jual_pertama",
                    // Memperbarui `Nama` menggunakan nilai literal `”Penjualan Pertama”` dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                    Nama = "Penjualan Pertama",
                    // Memperbarui `Teks` menggunakan koleksi berisi ”baris 1” dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                    Teks = ["baris 1"],
                    // Memperbarui `PrerequisiteAksi` menggunakan koleksi berisi new RulesetNarrativePrerequisiteDto { Aksi = ”Jual... dalam
                    // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                    PrerequisiteAksi =
                    // Menggunakan koleksi berisi new RulesetNarrativePrerequisiteDto { Aksi = ”Jual... sebagai bagian ekspresi yang sedang disusun dalam
                    // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                    [
                        // Menggunakan objek baru bertipe `RulesetNarrativePrerequisiteDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang
                        // disusun dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                        new RulesetNarrativePrerequisiteDto
                        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                        // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                        {
                            // Memperbarui `Aksi` menggunakan nilai literal `”JualMasakan”` dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                            Aksi = "JualMasakan",
                            // Memperbarui `Value` menggunakan nilai literal `1` dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                            Value = 1
                        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
                        // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                        }
                    // Menandai akhir daftar elemen atau indeks koleksi dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog; pasangan kurung siku
                    // mengelompokkan nilai sebagai satu struktur.
                    ]
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
                // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
                }
            // Menandai akhir daftar elemen atau indeks koleksi dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog; pasangan kurung siku
            // mengelompokkan nilai sebagai satu struktur.
            ]
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
        };

        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan
        // `RulesetDefinitionMapper.ToConfigJson(definition)`. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas
        // otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(RulesetDefinitionMapper.ToConfigJson(definition));
        // Menyiapkan variabel lokal `root` untuk nilai root dengan `document.RootElement` (nilai root element). Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var root = document.RootElement;
        // Menyiapkan variabel lokal `catalog` untuk nilai catalog dengan memanggil `root.GetProperty` dengan `”component_catalog”`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var catalog = root.GetProperty("component_catalog");
        // Menyiapkan variabel lokal `narrative` untuk nilai narrative dengan `catalog.GetProperty(”narasi”)[0]`, yaitu elemen koleksi yang dipilih melalui
        // indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var narrative = catalog.GetProperty("narasi")[0];

        // Menjalankan pemeriksaan bahwa `root.TryGetProperty(”interpreter_commands”, out _)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
        Assert.False(root.TryGetProperty("interpreter_commands", out _));
        // Menjalankan pemeriksaan bahwa `root.TryGetProperty(”narrative_assets”, out _)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
        Assert.False(root.TryGetProperty("narrative_assets", out _));
        // Menjalankan pemeriksaan bahwa `narrative.TryGetProperty(”scripts”, out _)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
        Assert.False(narrative.TryGetProperty("scripts", out _));
        // Menjalankan pemeriksaan bahwa `catalog.TryGetProperty(”quest”, out _)` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
        Assert.False(catalog.TryGetProperty("quest", out _));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”JualMasakan”`,
        // `narrative.GetProperty(”prerequisiteAksi”)[0].GetProperty(”aksi”).GetString()`); pengujian gagal jika keduanya berbeda dalam
        // ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
        Assert.Equal("JualMasakan", narrative.GetProperty("prerequisiteAksi")[0].GetProperty("aksi").GetString());
    // Menutup scope metode ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog; bagian berikut berada di luar batas blok tersebut
    // dalam ToConfigJson_EmitsOnlyDataDrivenNarrativeFields_AndOmitsQuestCatalog.
    }
// Menutup scope tipe RulesetDefinitionMapperTests; bagian berikut berada di luar batas blok tersebut.
}
