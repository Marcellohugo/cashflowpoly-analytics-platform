// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui DatabaseStartupIntegrationTests.
// Mengimpor namespace `System.Net` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net;
// Mengimpor namespace `System.Net.Http.Headers` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Headers;
// Mengimpor namespace `System.Net.Http.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Json;
// Mengimpor namespace `System.Security.Cryptography` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Cryptography;
// Mengimpor namespace `System.Text` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Cashflowpoly.Api.Tests.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Tests.Infrastructure;
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc.Testing` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc.Testing;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;
// Mengimpor namespace `Testcontainers.PostgreSql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Testcontainers.PostgreSql;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// menempatkan pengujian dalam koleksi fixture (”ApiIntegration”).
[Collection("ApiIntegration")]
// menerapkan metadata `Trait(”Category”, ”Integration”)` pada deklarasi berikut agar framework/compiler dapat mengenali pengaturannya.
[Trait("Category", "Integration")]
// Mendefinisikan tipe class `DatabaseStartupIntegrationTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class DatabaseStartupIntegrationTests
// Membuka scope tipe DatabaseStartupIntegrationTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `JwtSigningKey` menyimpan nilai jwt signing kunci dengan nilai awal nilai literal
    // `”integration-test-signing-key-with-min-32-char”`.
    private const string JwtSigningKey = "integration-test-signing-key-with-min-32-char";

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets` dengan hasil bertipe `Task`; operasi ini
    // menangani api startup on empty database applies schema parity dan seeds bawaan aturan. async memungkinkan metode menunggu operasi I/O dengan
    // await dan mengembalikan penyelesaian melalui Task.
    public async Task ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets()
    // Membuka scope metode ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
    {
        // Menyiapkan variabel lokal `database` untuk nilai database dengan memanggil `new PostgreSqlBuilder(”postgres:16”)
        // .WithDatabase(”cashflowpoly_empty_boot”) .WithUsername(”cashflowpoly”) .WithPassword(”cashflowpoly”) .Build` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var database = new PostgreSqlBuilder("postgres:16")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithDatabase(”cashflowpoly_empty_boot”) dalam
            // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithDatabase("cashflowpoly_empty_boot")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithUsername(”cashflowpoly”) dalam
            // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithUsername("cashflowpoly")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithPassword(”cashflowpoly”) dalam
            // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithPassword("cashflowpoly")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Build(); dalam
            // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Build();

        // Menjalankan hasil operasi asinkron memanggil `database.StartAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        await database.StartAsync();
        // Menjalankan hasil operasi asinkron memanggil `RunWithConnectionStringAsync` dengan `database.GetConnectionString()`, `async () => { await using
        // var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey); using var client = factory.CreateClient(new
        // WebApplication...`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        await RunWithConnectionStringAsync(database.GetConnectionString(), async () =>
        // Membuka scope fungsi lambda yang dipasok ke `RunWithConnectionStringAsync`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        {
            // Menyiapkan variabel lokal `factory` untuk nilai factory dengan objek baru bertipe `ApiWebApplicationFactory` dengan argumen
            // (database.GetConnectionString(), JwtSigningKey). Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas
            // otomatis saat scope berakhir.
            await using var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey);
            // Menyiapkan variabel lokal `client` untuk nilai client dengan memanggil `factory.CreateClient` dengan `new WebApplicationFactoryClientOptions {
            // AllowAutoRedirect = false }`. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
            // berakhir.
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            {
                // Memperbarui `AllowAutoRedirect` menggunakan false, yaitu kondisi nonaktif/tidak terpenuhi dalam
                // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
                AllowAutoRedirect = false
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
            // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            });

            // Menyiapkan variabel lokal `response` untuk hasil respons yang akan dibaca atau dikirim kepada pemanggil dengan hasil operasi asinkron memanggil
            // `client.GetAsync` dengan `”/health/ready”`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan
            // dari ekspresi nilai awal.
            var response = await client.GetAsync("/health/ready");
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `response.StatusCode`);
            // pengujian gagal jika keduanya berbeda dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // Menutup scope fungsi lambda yang dipasok ke `RunWithConnectionStringAsync`; bagian berikut berada di luar batas blok tersebut dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        });

        // Menyiapkan variabel lokal `connection` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan objek baru bertipe
        // `NpgsqlConnection` dengan argumen (database.GetConnectionString()). Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber
        // daya dilepas otomatis saat scope berakhir.
        await using var connection = new NpgsqlConnection(database.GetConnectionString());
        // Menjalankan hasil operasi asinkron memanggil `connection.OpenAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        await connection.OpenAsync();

        // Menyiapkan variabel lokal `hasPgcrypto` untuk nilai memiliki pgcrypto dengan hasil operasi asinkron menjalankan perintah basis data melalui
        // `connection` dengan `”select exists (select 1 from pg_extension where extname = 'pgcrypto');”` dan mengambil nilai skalar hasilnya; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var hasPgcrypto = await connection.ExecuteScalarAsync<bool>("select exists (select 1 from pg_extension where extname = 'pgcrypto');");
        // Menjalankan pemeriksaan bahwa `hasPgcrypto` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(hasPgcrypto);

        // Menyiapkan variabel lokal `seededRulesets` untuk nilai seeded aturan dengan hasil operasi asinkron menjalankan perintah basis data melalui
        // `connection` dengan `”select count(*) from rulesets where instructor_user_id is null and created_by_user_id is null;”` dan mengambil nilai skalar
        // hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seededRulesets = await connection.ExecuteScalarAsync<int>(
            // Meneruskan nilai literal `”select count(*) from rulesets where instructor_user_id is null and created_by_user_id is null;”` sebagai argumen ke
            // `connection.ExecuteScalarAsync<int>`.
            "select count(*) from rulesets where instructor_user_id is null and created_by_user_id is null;");
        // Menjalankan pemeriksaan bahwa `seededRulesets >= 2`, `$”Expected at least 2 default seeded rulesets, found {seededRulesets}.”` bernilai benar;
        // pengujian gagal jika kondisi tidak terpenuhi dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(seededRulesets >= 2, $"Expected at least 2 default seeded rulesets, found {seededRulesets}.");

        // Menyiapkan variabel lokal `canonicalSchemaState` untuk nilai canonical schema keadaan dengan hasil operasi asinkron membaca tepat satu baris
        // basis data melalui `connection.QuerySingleAsync<( bool HasParticipants, bool HasGameAssets, bool HasEventAssetReferences, bool HasInventory, bool
        // HasGoldHoldings, bool HasLoans, bool HasInsurance...` dengan `””” select to_regclass('public.session_participants') is not null as
        // HasParticipants, to_regclass('public.ruleset_game_assets') is not null as HasGameAssets, to_regclass('publ...`; jumlah baris selain satu
        // menyebabkan exception; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var canonicalSchemaState = await connection.QuerySingleAsync<(
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasParticipants,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasGameAssets,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasEventAssetReferences,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasInventory,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasGoldHoldings,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasLoans,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasInsurances,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasTriggerConditions,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasGameSettings,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasDisplayName,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasRemovedParticipantAssets,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasRemovedActionLogs,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasRemovedInterpreterCommands,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasRemovedQuestScripts,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasRemovedNarrativeScripts,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasRemovedNarrativeAssets,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasRemovedDonationRankings,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasRemovedPensionRankings,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasRemovedPlayersTable,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasRemovedCatalogTable,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasConfigJsonColumn,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasEventInventoryEffectsTable,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasRulesetCreatedByUserId,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasRulesetCreatedByVarchar,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasEventPayloadVersion,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasArchivedSessionColumn,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasArchivedRulesetColumn,
            // Menggunakan `bool` sebagai bagian ekspresi yang sedang disusun dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
            bool HasCitextUsername)>(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `connection.QuerySingleAsync<( bool
            // HasParticipants, bool HasGameAssets, bool HasEventAssetReferences, bool HasInventory, bool HasGoldHoldings, bool HasLoans, bool HasInsurance...`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.session_participants') is not null as HasParticipants,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.ruleset_game_assets') is not null as HasGameAssets,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.event_asset_references') is not null as HasEventAssetReferences,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.session_participant_inventory') is not null as HasInventory,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.session_participant_gold_holdings') is not null as HasGoldHoldings,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.session_participant_loans') is not null as HasLoans,`.
            // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.session_participant_insurances') is not null as HasInsurances,`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.ruleset_trigger_conditions') is not null as HasTriggerConditions,`.
            // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.ruleset_game_settings') is not null as HasGameSettings,`.
            // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `exists (`.
            // Baris literal 13: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
            // Baris literal 14: FROM memilih tabel/subquery sumber pembacaan: `from information_schema.columns`.
            // Baris literal 15: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where table_schema = 'public'`.
            // Baris literal 16: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and table_name = 'app_users'`.
            // Baris literal 17: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and column_name = 'display_name'`.
            // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as HasDisplayName,`.
            // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.session_participant_assets') is not null as HasRemovedParticipantAssets,`.
            // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.session_action_logs') is not null as HasRemovedActionLogs,`.
            // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.interpreter_commands') is not null as HasRemovedInterpreterCommands,`.
            // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.quest_scripts') is not null as HasRemovedQuestScripts,`.
            // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.narrative_scripts') is not null as HasRemovedNarrativeScripts,`.
            // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.narrative_assets') is not null as HasRemovedNarrativeAssets,`.
            // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.session_donation_event_rankings') is not null as HasRemovedDonationRankings,`.
            // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `to_regclass('public.session_pension_rankings') is not null as HasRemovedPensionRankings,`.
            // Baris literal 27: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `exists (`.
            // Baris literal 28: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
            // Baris literal 29: FROM memilih tabel/subquery sumber pembacaan: `from information_schema.tables`.
            // Baris literal 30: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where table_schema = 'public'`.
            // Baris literal 31: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and table_name = 'session_players'`.
            // Baris literal 32: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and table_type = 'BASE TABLE'`.
            // Baris literal 33: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as
            // HasRemovedPlayersTable,`.
            // Baris literal 34: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `exists (`.
            // Baris literal 35: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
            // Baris literal 36: FROM memilih tabel/subquery sumber pembacaan: `from information_schema.tables`.
            // Baris literal 37: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where table_schema = 'public'`.
            // Baris literal 38: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and table_name = 'ruleset_catalog_items'`.
            // Baris literal 39: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and table_type = 'BASE TABLE'`.
            // Baris literal 40: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as
            // HasRemovedCatalogTable,`.
            // Baris literal 41: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `exists (`.
            // Baris literal 42: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
            // Baris literal 43: FROM memilih tabel/subquery sumber pembacaan: `from information_schema.columns`.
            // Baris literal 44: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where table_schema = 'public'`.
            // Baris literal 45: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and table_name = 'ruleset_versions'`.
            // Baris literal 46: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and column_name = 'config_json'`.
            // Baris literal 47: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as HasConfigJsonColumn,`.
            // Baris literal 48: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `exists (`.
            // Baris literal 49: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
            // Baris literal 50: FROM memilih tabel/subquery sumber pembacaan: `from information_schema.tables`.
            // Baris literal 51: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where table_schema = 'public'`.
            // Baris literal 52: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and table_name = 'event_inventory_effects'`.
            // Baris literal 53: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and table_type = 'BASE TABLE'`.
            // Baris literal 54: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as
            // HasEventInventoryEffectsTable,`.
            // Baris literal 55: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `exists (`.
            // Baris literal 56: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
            // Baris literal 57: FROM memilih tabel/subquery sumber pembacaan: `from information_schema.columns`.
            // Baris literal 58: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where table_schema = 'public'`.
            // Baris literal 59: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and table_name = 'rulesets'`.
            // Baris literal 60: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and column_name = 'created_by_user_id'`.
            // Baris literal 61: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as
            // HasRulesetCreatedByUserId,`.
            // Baris literal 62: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `exists (`.
            // Baris literal 63: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
            // Baris literal 64: FROM memilih tabel/subquery sumber pembacaan: `from information_schema.columns`.
            // Baris literal 65: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where table_schema = 'public'`.
            // Baris literal 66: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and table_name = 'rulesets'`.
            // Baris literal 67: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and column_name = 'created_by'`.
            // Baris literal 68: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as
            // HasRulesetCreatedByVarchar,`.
            // Baris literal 69: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `exists (`.
            // Baris literal 70: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
            // Baris literal 71: FROM memilih tabel/subquery sumber pembacaan: `from information_schema.columns`.
            // Baris literal 72: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where table_schema = 'public'`.
            // Baris literal 73: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and table_name = 'events'`.
            // Baris literal 74: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and column_name = 'payload_version'`.
            // Baris literal 75: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as
            // HasEventPayloadVersion,`.
            // Baris literal 76: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `exists (`.
            // Baris literal 77: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
            // Baris literal 78: FROM memilih tabel/subquery sumber pembacaan: `from information_schema.columns`.
            // Baris literal 79: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where table_schema = 'public'`.
            // Baris literal 80: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and table_name = 'sessions'`.
            // Baris literal 81: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and column_name = 'is_archived'`.
            // Baris literal 82: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as
            // HasArchivedSessionColumn,`.
            // Baris literal 83: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `exists (`.
            // Baris literal 84: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
            // Baris literal 85: FROM memilih tabel/subquery sumber pembacaan: `from information_schema.columns`.
            // Baris literal 86: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where table_schema = 'public'`.
            // Baris literal 87: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and table_name = 'rulesets'`.
            // Baris literal 88: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and column_name = 'is_archived'`.
            // Baris literal 89: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as
            // HasArchivedRulesetColumn,`.
            // Baris literal 90: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `exists (`.
            // Baris literal 91: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
            // Baris literal 92: FROM memilih tabel/subquery sumber pembacaan: `from information_schema.columns`.
            // Baris literal 93: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where table_schema = 'public'`.
            // Baris literal 94: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and table_name = 'app_users'`.
            // Baris literal 95: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and column_name = 'username'`.
            // Baris literal 96: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and udt_name = 'citext'`.
            // Baris literal 97: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) as HasCitextUsername`.
            // Baris literal 98: Pembatas literal/penutup `”””);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select
                to_regclass('public.session_participants') is not null as HasParticipants,
                to_regclass('public.ruleset_game_assets') is not null as HasGameAssets,
                to_regclass('public.event_asset_references') is not null as HasEventAssetReferences,
                to_regclass('public.session_participant_inventory') is not null as HasInventory,
                to_regclass('public.session_participant_gold_holdings') is not null as HasGoldHoldings,
                to_regclass('public.session_participant_loans') is not null as HasLoans,
                to_regclass('public.session_participant_insurances') is not null as HasInsurances,
                to_regclass('public.ruleset_trigger_conditions') is not null as HasTriggerConditions,
                to_regclass('public.ruleset_game_settings') is not null as HasGameSettings,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'app_users'
                      and column_name = 'display_name'
                ) as HasDisplayName,
                to_regclass('public.session_participant_assets') is not null as HasRemovedParticipantAssets,
                to_regclass('public.session_action_logs') is not null as HasRemovedActionLogs,
                to_regclass('public.interpreter_commands') is not null as HasRemovedInterpreterCommands,
                to_regclass('public.quest_scripts') is not null as HasRemovedQuestScripts,
                to_regclass('public.narrative_scripts') is not null as HasRemovedNarrativeScripts,
                to_regclass('public.narrative_assets') is not null as HasRemovedNarrativeAssets,
                to_regclass('public.session_donation_event_rankings') is not null as HasRemovedDonationRankings,
                to_regclass('public.session_pension_rankings') is not null as HasRemovedPensionRankings,
                exists (
                    select 1
                    from information_schema.tables
                    where table_schema = 'public'
                      and table_name = 'session_players'
                      and table_type = 'BASE TABLE'
                ) as HasRemovedPlayersTable,
                exists (
                    select 1
                    from information_schema.tables
                    where table_schema = 'public'
                      and table_name = 'ruleset_catalog_items'
                      and table_type = 'BASE TABLE'
                ) as HasRemovedCatalogTable,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'ruleset_versions'
                      and column_name = 'config_json'
                ) as HasConfigJsonColumn,
                exists (
                    select 1
                    from information_schema.tables
                    where table_schema = 'public'
                      and table_name = 'event_inventory_effects'
                      and table_type = 'BASE TABLE'
                ) as HasEventInventoryEffectsTable,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'rulesets'
                      and column_name = 'created_by_user_id'
                ) as HasRulesetCreatedByUserId,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'rulesets'
                      and column_name = 'created_by'
                ) as HasRulesetCreatedByVarchar,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'events'
                      and column_name = 'payload_version'
                ) as HasEventPayloadVersion,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'sessions'
                      and column_name = 'is_archived'
                ) as HasArchivedSessionColumn,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'rulesets'
                      and column_name = 'is_archived'
                ) as HasArchivedRulesetColumn,
                exists (
                    select 1
                    from information_schema.columns
                    where table_schema = 'public'
                      and table_name = 'app_users'
                      and column_name = 'username'
                      and udt_name = 'citext'
                ) as HasCitextUsername
            """);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasParticipants` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(canonicalSchemaState.HasParticipants);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasGameAssets` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(canonicalSchemaState.HasGameAssets);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasEventAssetReferences` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(canonicalSchemaState.HasEventAssetReferences);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasInventory` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(canonicalSchemaState.HasInventory);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasGoldHoldings` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(canonicalSchemaState.HasGoldHoldings);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasLoans` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(canonicalSchemaState.HasLoans);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasInsurances` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(canonicalSchemaState.HasInsurances);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasTriggerConditions` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(canonicalSchemaState.HasTriggerConditions);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasGameSettings` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(canonicalSchemaState.HasGameSettings);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasDisplayName` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(canonicalSchemaState.HasDisplayName);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasCitextUsername` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(canonicalSchemaState.HasCitextUsername);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasRemovedParticipantAssets` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.False(canonicalSchemaState.HasRemovedParticipantAssets);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasRemovedActionLogs` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.False(canonicalSchemaState.HasRemovedActionLogs);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasRemovedInterpreterCommands` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.False(canonicalSchemaState.HasRemovedInterpreterCommands);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasRemovedQuestScripts` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.False(canonicalSchemaState.HasRemovedQuestScripts);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasRemovedNarrativeScripts` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.False(canonicalSchemaState.HasRemovedNarrativeScripts);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasRemovedNarrativeAssets` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.False(canonicalSchemaState.HasRemovedNarrativeAssets);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasRemovedDonationRankings` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.False(canonicalSchemaState.HasRemovedDonationRankings);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasRemovedPensionRankings` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.False(canonicalSchemaState.HasRemovedPensionRankings);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasRemovedPlayersTable` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.False(canonicalSchemaState.HasRemovedPlayersTable);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasRemovedCatalogTable` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.False(canonicalSchemaState.HasRemovedCatalogTable);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasConfigJsonColumn` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.False(canonicalSchemaState.HasConfigJsonColumn);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasEventInventoryEffectsTable` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.False(canonicalSchemaState.HasEventInventoryEffectsTable);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasRulesetCreatedByUserId` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(canonicalSchemaState.HasRulesetCreatedByUserId);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasRulesetCreatedByVarchar` bernilai salah; pengujian gagal jika kondisi justru terpenuhi
        // dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.False(canonicalSchemaState.HasRulesetCreatedByVarchar);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasEventPayloadVersion` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(canonicalSchemaState.HasEventPayloadVersion);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasArchivedSessionColumn` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(canonicalSchemaState.HasArchivedSessionColumn);
        // Menjalankan pemeriksaan bahwa `canonicalSchemaState.HasArchivedRulesetColumn` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        Assert.True(canonicalSchemaState.HasArchivedRulesetColumn);

        // Menjalankan hasil operasi asinkron memanggil `AssertHasUniqueIndexAsync` dengan `connection`, `”app_users”`, `[”username”]`; await menunggu hasil
        // tanpa memblokir thread selama operasi belum selesai dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        await AssertHasUniqueIndexAsync(connection, "app_users", ["username"]);
        // Menjalankan hasil operasi asinkron memanggil `AssertHasUniqueIndexAsync` dengan `connection`, `”session_participants”`, `[”session_id”,
        // ”user_id”]`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        await AssertHasUniqueIndexAsync(connection, "session_participants", ["session_id", "user_id"]);
        // Menjalankan hasil operasi asinkron memanggil `AssertHasUniqueIndexAsync` dengan `connection`, `”events”`, `[”session_id”, ”event_id”]`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        await AssertHasUniqueIndexAsync(connection, "events", ["session_id", "event_id"]);
        // Menjalankan hasil operasi asinkron memanggil `AssertHasUniqueIndexAsync` dengan `connection`, `”events”`, `[”session_id”, ”client_request_id”]`;
        // await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
        await AssertHasUniqueIndexAsync(connection, "events", ["session_id", "client_request_id"]);
    // Menutup scope metode ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets; bagian berikut berada di luar batas blok tersebut
    // dalam ApiStartup_OnEmptyDatabase_AppliesSchemaParity_And_SeedsDefaultRulesets.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration` dengan hasil bertipe `Task`; operasi ini menangani api startup on
    // damaged canonical database rejects migration. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui
    // Task.
    public async Task ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration()
    // Membuka scope metode ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration.
    {
        // Menyiapkan variabel lokal `database` untuk nilai database dengan memanggil `new PostgreSqlBuilder(”postgres:16”)
        // .WithDatabase(”cashflowpoly_partial_boot”) .WithUsername(”cashflowpoly”) .WithPassword(”cashflowpoly”) .Build` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var database = new PostgreSqlBuilder("postgres:16")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithDatabase(”cashflowpoly_partial_boot”) dalam
            // ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithDatabase("cashflowpoly_partial_boot")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithUsername(”cashflowpoly”) dalam
            // ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithUsername("cashflowpoly")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithPassword(”cashflowpoly”) dalam
            // ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithPassword("cashflowpoly")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Build(); dalam ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Build();

        // Menjalankan hasil operasi asinkron memanggil `database.StartAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai dalam ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration.
        await database.StartAsync();

        // Membatasi masa pakai `var setupConnection = new NpgsqlConnection(database.GetConnectionString())` pada blok using; sumber daya dilepas ketika
        // blok berakhir melalui DisposeAsync.
        await using (var setupConnection = new NpgsqlConnection(database.GetConnectionString()))
        // Membuka scope scope pemakaian sumber daya using; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration.
        {
            // Menjalankan hasil operasi asinkron memanggil `setupConnection.OpenAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai dalam ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration.
            await setupConnection.OpenAsync();

            // Menyiapkan variabel lokal `schemaPath` untuk nilai schema path dengan memanggil `Path.Combine` dengan `AppContext.BaseDirectory`, `”database”`,
            // `”00_create_schema.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var schemaPath = Path.Combine(AppContext.BaseDirectory, "database", "00_create_schema.sql");
            // Menjalankan pemeriksaan bahwa `File.Exists(schemaPath)`, `$”Schema SQL harus tersedia pada path '{schemaPath}'.”` bernilai benar; pengujian gagal
            // jika kondisi tidak terpenuhi dalam ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration.
            Assert.True(File.Exists(schemaPath), $"Schema SQL harus tersedia pada path '{schemaPath}'.");

            // Menyiapkan variabel lokal `schemaSql` untuk nilai schema SQL dengan hasil operasi asinkron memanggil `File.ReadAllTextAsync` dengan `schemaPath`;
            // await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var schemaSql = await File.ReadAllTextAsync(schemaPath);
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `setupConnection` menggunakan `schemaSql`; nilai hasil menunjukkan jumlah
            // baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration.
            await setupConnection.ExecuteAsync(schemaSql);
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `setupConnection` menggunakan `””” drop table if exists
            // event_asset_references cascade; drop table if exists session_participant_gold_holdings cascade; drop table if exists session_participant_loans
            // cascade...`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai
            // dalam ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration.
            await setupConnection.ExecuteAsync(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke `setupConnection.ExecuteAsync`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `drop table if exists
                // event_asset_references cascade;`.
                // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `drop table if exists
                // session_participant_gold_holdings cascade;`.
                // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `drop table if exists
                // session_participant_loans cascade;`.
                // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `drop table if exists
                // session_participant_insurances cascade;`.
                // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `drop table if exists
                // session_participant_inventory cascade;`.
                // Baris literal 7: Pembatas literal/penutup `”””);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                drop table if exists event_asset_references cascade;
                drop table if exists session_participant_gold_holdings cascade;
                drop table if exists session_participant_loans cascade;
                drop table if exists session_participant_insurances cascade;
                drop table if exists session_participant_inventory cascade;
                """);
        // Menutup scope scope pemakaian sumber daya using; bagian berikut berada di luar batas blok tersebut dalam
        // ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration.
        }

        // Menyiapkan variabel lokal `exception` untuk nilai exception dengan hasil operasi asinkron pemeriksaan bahwa operasi `() =>
        // RunWithConnectionStringAsync(database.GetConnectionString(), async () => { await using var factory = new
        // ApiWebApplicationFactory(database.GetConnectionString(), JwtSign...` melempar jenis exception yang diharapkan; pengujian gagal jika perilaku
        // kesalahan berbeda; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var exception = await Assert.ThrowsAnyAsync<Exception>(() =>
            // Meneruskan memanggil `database.GetConnectionString` dengan tanpa argumen sebagai argumen ke `RunWithConnectionStringAsync`; Meneruskan fungsi
            // lambda `async () => { await using var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey); using var client =
            // factory.CreateClient(new WebApplication...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
            // `RunWithConnectionStringAsync`.
            RunWithConnectionStringAsync(database.GetConnectionString(), async () =>
            // Membuka scope fungsi lambda yang dipasok ke `RunWithConnectionStringAsync`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration.
            {
                // Menyiapkan variabel lokal `factory` untuk nilai factory dengan objek baru bertipe `ApiWebApplicationFactory` dengan argumen
                // (database.GetConnectionString(), JwtSigningKey). Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas
                // otomatis saat scope berakhir.
                await using var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey);
                // Menyiapkan variabel lokal `client` untuk nilai client dengan memanggil `factory.CreateClient` dengan `new WebApplicationFactoryClientOptions {
                // AllowAutoRedirect = false }`. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
                // berakhir.
                using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration.
                {
                    // Memperbarui `AllowAutoRedirect` menggunakan false, yaitu kondisi nonaktif/tidak terpenuhi dalam
                    // ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration.
                    AllowAutoRedirect = false
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
                // ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration.
                });
                // Menjalankan hasil operasi asinkron memanggil `client.GetAsync` dengan `”/health/ready”`; await menunggu hasil tanpa memblokir thread selama
                // operasi belum selesai dalam ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration.
                await client.GetAsync("/health/ready");
            // Menutup scope fungsi lambda yang dipasok ke `RunWithConnectionStringAsync`; bagian berikut berada di luar batas blok tersebut dalam
            // ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration.
            }));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”fingerprint mismatch”`,
        // `exception.ToString()`, `StringComparison.OrdinalIgnoreCase` dalam ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration.
        Assert.Contains("fingerprint mismatch", exception.ToString(), StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration; bagian berikut berada di luar batas blok tersebut dalam
    // ApiStartup_OnDamagedCanonicalDatabase_RejectsMigration.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory` dengan hasil bertipe `Task`; operasi ini menangani api
    // startup when repeated berstatus idempotent dan keeps migration history. async memungkinkan metode menunggu operasi I/O dengan await dan
    // mengembalikan penyelesaian melalui Task.
    public async Task ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory()
    // Membuka scope metode ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory.
    {
        // Menyiapkan variabel lokal `database` untuk nilai database dengan memanggil `new PostgreSqlBuilder(”postgres:16”)
        // .WithDatabase(”cashflowpoly_repeat_boot”) .WithUsername(”cashflowpoly”) .WithPassword(”cashflowpoly”) .Build` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var database = new PostgreSqlBuilder("postgres:16")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithDatabase(”cashflowpoly_repeat_boot”) dalam
            // ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithDatabase("cashflowpoly_repeat_boot")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithUsername(”cashflowpoly”) dalam
            // ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithUsername("cashflowpoly")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithPassword(”cashflowpoly”) dalam
            // ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithPassword("cashflowpoly")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Build(); dalam ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Build();

        // Menjalankan hasil operasi asinkron memanggil `database.StartAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai dalam ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory.
        await database.StartAsync();
        // Menjalankan hasil operasi asinkron memanggil `RunWithConnectionStringAsync` dengan `database.GetConnectionString()`, `async () => { await using
        // (var firstFactory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey)) using (var firstClient =
        // firstFactory.CreateClient()...`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory.
        await RunWithConnectionStringAsync(database.GetConnectionString(), async () =>
        // Membuka scope fungsi lambda yang dipasok ke `RunWithConnectionStringAsync`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory.
        {
            // Membatasi masa pakai `var firstFactory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey)` pada blok using; sumber
            // daya dilepas ketika blok berakhir melalui DisposeAsync.
            await using (var firstFactory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey))
            // Membatasi masa pakai `var firstClient = firstFactory.CreateClient()` pada blok using; sumber daya dilepas ketika blok berakhir melalui Dispose.
            using (var firstClient = firstFactory.CreateClient())
            // Membuka scope scope pemakaian sumber daya using; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `(await
                // firstClient.GetAsync(”/health/ready”)).StatusCode`); pengujian gagal jika keduanya berbeda dalam
                // ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory.
                Assert.Equal(HttpStatusCode.OK, (await firstClient.GetAsync("/health/ready")).StatusCode);
            // Menutup scope scope pemakaian sumber daya using; bagian berikut berada di luar batas blok tersebut dalam
            // ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory.
            }

            // Membatasi masa pakai `var secondFactory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey)` pada blok using; sumber
            // daya dilepas ketika blok berakhir melalui DisposeAsync.
            await using (var secondFactory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey))
            // Membatasi masa pakai `var secondClient = secondFactory.CreateClient()` pada blok using; sumber daya dilepas ketika blok berakhir melalui Dispose.
            using (var secondClient = secondFactory.CreateClient())
            // Membuka scope scope pemakaian sumber daya using; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory.
            {
                // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `(await
                // secondClient.GetAsync(”/health/ready”)).StatusCode`); pengujian gagal jika keduanya berbeda dalam
                // ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory.
                Assert.Equal(HttpStatusCode.OK, (await secondClient.GetAsync("/health/ready")).StatusCode);
            // Menutup scope scope pemakaian sumber daya using; bagian berikut berada di luar batas blok tersebut dalam
            // ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory.
            }
        // Menutup scope fungsi lambda yang dipasok ke `RunWithConnectionStringAsync`; bagian berikut berada di luar batas blok tersebut dalam
        // ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory.
        });

        // Menyiapkan variabel lokal `connection` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan objek baru bertipe
        // `NpgsqlConnection` dengan argumen (database.GetConnectionString()). Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber
        // daya dilepas otomatis saat scope berakhir.
        await using var connection = new NpgsqlConnection(database.GetConnectionString());
        // Menjalankan hasil operasi asinkron memanggil `connection.OpenAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai dalam ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory.
        await connection.OpenAsync();
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `await
        // connection.ExecuteScalarAsync<int>(”select count(*) from schema_history;”)`); pengujian gagal jika keduanya berbeda dalam
        // ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory.
        Assert.Equal(4, await connection.ExecuteScalarAsync<int>("select count(*) from schema_history;"));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`0`, `await connection.ExecuteScalarAsync<int>(
        // ”select count(*) from schema_history where checksum !~ '^[0-9a-f]{64}$';”)`); pengujian gagal jika keduanya berbeda dalam
        // ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory.
        Assert.Equal(0, await connection.ExecuteScalarAsync<int>(
            // Meneruskan nilai literal `”select count(*) from schema_history where checksum !~ '^[0-9a-f]{64}$';”` sebagai argumen ke
            // `connection.ExecuteScalarAsync<int>`.
            "select count(*) from schema_history where checksum !~ '^[0-9a-f]{64}$';"));
    // Menutup scope metode ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory; bagian berikut berada di luar batas blok tersebut dalam
    // ApiStartup_WhenRepeated_IsIdempotentAndKeepsMigrationHistory.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup` dengan hasil bertipe `Task`; operasi ini menangani api startup when
    // applied checksum was changed stops startup. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui
    // Task.
    public async Task ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup()
    // Membuka scope metode ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup.
    {
        // Menyiapkan variabel lokal `database` untuk nilai database dengan memanggil `new PostgreSqlBuilder(”postgres:16”)
        // .WithDatabase(”cashflowpoly_checksum_boot”) .WithUsername(”cashflowpoly”) .WithPassword(”cashflowpoly”) .Build` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var database = new PostgreSqlBuilder("postgres:16")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithDatabase(”cashflowpoly_checksum_boot”) dalam
            // ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithDatabase("cashflowpoly_checksum_boot")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithUsername(”cashflowpoly”) dalam
            // ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithUsername("cashflowpoly")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithPassword(”cashflowpoly”) dalam
            // ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithPassword("cashflowpoly")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Build(); dalam ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Build();

        // Menjalankan hasil operasi asinkron memanggil `database.StartAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai dalam ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup.
        await database.StartAsync();
        // Menjalankan hasil operasi asinkron memanggil `RunWithConnectionStringAsync` dengan `database.GetConnectionString()`, `async () => { await using
        // var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey); using var client = factory.CreateClient();
        // Assert.Equal(Ht...`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup.
        await RunWithConnectionStringAsync(database.GetConnectionString(), async () =>
        // Membuka scope fungsi lambda yang dipasok ke `RunWithConnectionStringAsync`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup.
        {
            // Menyiapkan variabel lokal `factory` untuk nilai factory dengan objek baru bertipe `ApiWebApplicationFactory` dengan argumen
            // (database.GetConnectionString(), JwtSigningKey). Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas
            // otomatis saat scope berakhir.
            await using var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey);
            // Menyiapkan variabel lokal `client` untuk nilai client dengan memanggil `factory.CreateClient` dengan tanpa argumen. Tipe variabel disimpulkan
            // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var client = factory.CreateClient();
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `(await
            // client.GetAsync(”/health/ready”)).StatusCode`); pengujian gagal jika keduanya berbeda dalam
            // ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup.
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health/ready")).StatusCode);
        // Menutup scope fungsi lambda yang dipasok ke `RunWithConnectionStringAsync`; bagian berikut berada di luar batas blok tersebut dalam
        // ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup.
        });

        // Membatasi masa pakai `var connection = new NpgsqlConnection(database.GetConnectionString())` pada blok using; sumber daya dilepas ketika blok
        // berakhir melalui DisposeAsync.
        await using (var connection = new NpgsqlConnection(database.GetConnectionString()))
        // Membuka scope scope pemakaian sumber daya using; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup.
        {
            // Menjalankan hasil operasi asinkron memanggil `connection.OpenAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai dalam ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup.
            await connection.OpenAsync();
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `connection` menggunakan `”update schema_history set checksum = repeat('0',
            // 64) where version = 2;”`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum
            // selesai dalam ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup.
            await connection.ExecuteAsync("update schema_history set checksum = repeat('0', 64) where version = 2;");
        // Menutup scope scope pemakaian sumber daya using; bagian berikut berada di luar batas blok tersebut dalam
        // ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup.
        }

        // Menyiapkan variabel lokal `exception` untuk nilai exception dengan hasil operasi asinkron pemeriksaan bahwa operasi `() =>
        // RunWithConnectionStringAsync(database.GetConnectionString(), async () => { await using var factory = new
        // ApiWebApplicationFactory(database.GetConnectionString(), JwtSign...` melempar jenis exception yang diharapkan; pengujian gagal jika perilaku
        // kesalahan berbeda; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var exception = await Assert.ThrowsAnyAsync<Exception>(() =>
            // Meneruskan memanggil `database.GetConnectionString` dengan tanpa argumen sebagai argumen ke `RunWithConnectionStringAsync`; Meneruskan fungsi
            // lambda `async () => { await using var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey); using var client =
            // factory.CreateClient(); await client.Ge...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
            // `RunWithConnectionStringAsync`.
            RunWithConnectionStringAsync(database.GetConnectionString(), async () =>
            // Membuka scope fungsi lambda yang dipasok ke `RunWithConnectionStringAsync`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup.
            {
                // Menyiapkan variabel lokal `factory` untuk nilai factory dengan objek baru bertipe `ApiWebApplicationFactory` dengan argumen
                // (database.GetConnectionString(), JwtSigningKey). Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas
                // otomatis saat scope berakhir.
                await using var factory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey);
                // Menyiapkan variabel lokal `client` untuk nilai client dengan memanggil `factory.CreateClient` dengan tanpa argumen. Tipe variabel disimpulkan
                // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
                using var client = factory.CreateClient();
                // Menjalankan hasil operasi asinkron memanggil `client.GetAsync` dengan `”/health/ready”`; await menunggu hasil tanpa memblokir thread selama
                // operasi belum selesai dalam ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup.
                await client.GetAsync("/health/ready");
            // Menutup scope fungsi lambda yang dipasok ke `RunWithConnectionStringAsync`; bagian berikut berada di luar batas blok tersebut dalam
            // ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup.
            }));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Checksum migrasi V2 berbeda”`,
        // `exception.ToString()`, `StringComparison.Ordinal` dalam ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup.
        Assert.Contains("Checksum migrasi V2 berbeda", exception.ToString(), StringComparison.Ordinal);
    // Menutup scope metode ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup; bagian berikut berada di luar batas blok tersebut dalam
    // ApiStartup_WhenAppliedChecksumWasChanged_StopsStartup.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts` dengan hasil bertipe `Task`; operasi ini menangani api startup
    // when only migration line endings differ still starts. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian
    // melalui Task.
    public async Task ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts()
    // Membuka scope metode ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts.
    {
        // Menyiapkan variabel lokal `database` untuk nilai database dengan memanggil `new PostgreSqlBuilder(”postgres:16”)
        // .WithDatabase(”cashflowpoly_line_endings_boot”) .WithUsername(”cashflowpoly”) .WithPassword(”cashflowpoly”) .Build` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var database = new PostgreSqlBuilder("postgres:16")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithDatabase(”cashflowpoly_line_endings_boot”) dalam
            // ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithDatabase("cashflowpoly_line_endings_boot")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithUsername(”cashflowpoly”) dalam
            // ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithUsername("cashflowpoly")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .WithPassword(”cashflowpoly”) dalam
            // ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .WithPassword("cashflowpoly")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Build(); dalam ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Build();

        // Menjalankan hasil operasi asinkron memanggil `database.StartAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai dalam ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts.
        await database.StartAsync();
        // Menjalankan hasil operasi asinkron memanggil `RunWithConnectionStringAsync` dengan `database.GetConnectionString()`, `async () => { await using
        // var firstFactory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey); using var firstClient =
        // firstFactory.CreateClient(); ...`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts.
        await RunWithConnectionStringAsync(database.GetConnectionString(), async () =>
        // Membuka scope fungsi lambda yang dipasok ke `RunWithConnectionStringAsync`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts.
        {
            // Menyiapkan variabel lokal `firstFactory` untuk nilai first factory dengan objek baru bertipe `ApiWebApplicationFactory` dengan argumen
            // (database.GetConnectionString(), JwtSigningKey). Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas
            // otomatis saat scope berakhir.
            await using var firstFactory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey);
            // Menyiapkan variabel lokal `firstClient` untuk nilai first client dengan memanggil `firstFactory.CreateClient` dengan tanpa argumen. Tipe variabel
            // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var firstClient = firstFactory.CreateClient();
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `(await
            // firstClient.GetAsync(”/health/ready”)).StatusCode`); pengujian gagal jika keduanya berbeda dalam
            // ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts.
            Assert.Equal(HttpStatusCode.OK, (await firstClient.GetAsync("/health/ready")).StatusCode);
        // Menutup scope fungsi lambda yang dipasok ke `RunWithConnectionStringAsync`; bagian berikut berada di luar batas blok tersebut dalam
        // ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts.
        });

        // Menyiapkan variabel lokal `migrationPath` untuk nilai migration path dengan memanggil `Path.Combine` dengan `AppContext.BaseDirectory`,
        // `”database”`, `”migrations”`, `”V002__setup_revisions_and_demo_accounts.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var migrationPath = Path.Combine(
            // Meneruskan `AppContext.BaseDirectory` (nilai base directory) sebagai argumen ke `Path.Combine`.
            AppContext.BaseDirectory,
            // Meneruskan nilai literal `”database”` sebagai argumen ke `Path.Combine`.
            "database",
            // Meneruskan nilai literal `”migrations”` sebagai argumen ke `Path.Combine`.
            "migrations",
            // Meneruskan nilai literal `”V002__setup_revisions_and_demo_accounts.sql”` sebagai argumen ke `Path.Combine`.
            "V002__setup_revisions_and_demo_accounts.sql");
        // Menyiapkan variabel lokal `migrationSql` untuk nilai migration SQL dengan hasil operasi asinkron memanggil `File.ReadAllTextAsync` dengan
        // `migrationPath`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var migrationSql = await File.ReadAllTextAsync(migrationPath);
        // Menyiapkan variabel lokal `crlfBytes` untuk nilai crlf bytes dengan memanggil `Encoding.UTF8.GetBytes` dengan
        // `migrationSql.ReplaceLineEndings(”\r\n”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var crlfBytes = Encoding.UTF8.GetBytes(migrationSql.ReplaceLineEndings("\r\n"));
        // Menyiapkan variabel lokal `crlfChecksum` untuk nilai crlf checksum dengan menormalisasi `Convert.ToHexString(SHA256.HashData(crlfBytes))` menjadi
        // huruf kecil dengan aturan kultur invariant. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var crlfChecksum = Convert.ToHexString(SHA256.HashData(crlfBytes)).ToLowerInvariant();

        // Membatasi masa pakai `var connection = new NpgsqlConnection(database.GetConnectionString())` pada blok using; sumber daya dilepas ketika blok
        // berakhir melalui DisposeAsync.
        await using (var connection = new NpgsqlConnection(database.GetConnectionString()))
        // Membuka scope scope pemakaian sumber daya using; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts.
        {
            // Menjalankan hasil operasi asinkron memanggil `connection.OpenAsync` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai dalam ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts.
            await connection.OpenAsync();
            // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `connection` menggunakan `”update schema_history set checksum = @checksum
            // where version = 2;”`, `new { checksum = crlfChecksum }`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa
            // memblokir thread selama operasi belum selesai dalam ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts.
            await connection.ExecuteAsync(
                // Meneruskan nilai literal `”update schema_history set checksum = @checksum where version = 2;”` sebagai argumen ke `connection.ExecuteAsync`.
                "update schema_history set checksum = @checksum where version = 2;",
                // Meneruskan objek anonim yang mengelompokkan checksum sebagai satu nilai sebagai argumen ke `connection.ExecuteAsync`.
                new { checksum = crlfChecksum });
        // Menutup scope scope pemakaian sumber daya using; bagian berikut berada di luar batas blok tersebut dalam
        // ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts.
        }

        // Menjalankan hasil operasi asinkron memanggil `RunWithConnectionStringAsync` dengan `database.GetConnectionString()`, `async () => { await using
        // var secondFactory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey); using var secondClient =
        // secondFactory.CreateClient(...`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts.
        await RunWithConnectionStringAsync(database.GetConnectionString(), async () =>
        // Membuka scope fungsi lambda yang dipasok ke `RunWithConnectionStringAsync`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts.
        {
            // Menyiapkan variabel lokal `secondFactory` untuk nilai second factory dengan objek baru bertipe `ApiWebApplicationFactory` dengan argumen
            // (database.GetConnectionString(), JwtSigningKey). Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas
            // otomatis saat scope berakhir.
            await using var secondFactory = new ApiWebApplicationFactory(database.GetConnectionString(), JwtSigningKey);
            // Menyiapkan variabel lokal `secondClient` untuk nilai second client dengan memanggil `secondFactory.CreateClient` dengan tanpa argumen. Tipe
            // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var secondClient = secondFactory.CreateClient();
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`HttpStatusCode.OK`, `(await
            // secondClient.GetAsync(”/health/ready”)).StatusCode`); pengujian gagal jika keduanya berbeda dalam
            // ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts.
            Assert.Equal(HttpStatusCode.OK, (await secondClient.GetAsync("/health/ready")).StatusCode);
        // Menutup scope fungsi lambda yang dipasok ke `RunWithConnectionStringAsync`; bagian berikut berada di luar batas blok tersebut dalam
        // ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts.
        });
    // Menutup scope metode ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts; bagian berikut berada di luar batas blok tersebut dalam
    // ApiStartup_WhenOnlyMigrationLineEndingsDiffer_StillStarts.
    }

    // Mendefinisikan metode `RunWithConnectionStringAsync` dengan hasil bertipe `Task`; operasi ini menangani run dengan connection string asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `connectionString`
    // bertipe `string` membawa nilai connection string; Parameter `action` bertipe `Func<Task>` membawa nilai aksi.
    private static async Task RunWithConnectionStringAsync(string connectionString, Func<Task> action)
    // Membuka scope metode RunWithConnectionStringAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // RunWithConnectionStringAsync.
    {
        // Menyiapkan variabel lokal `previousConnectionString` untuk nilai previous connection string dengan memanggil `Environment.GetEnvironmentVariable`
        // dengan `”ConnectionStrings__Default”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var previousConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
        // Menyiapkan variabel lokal `previousJwtSigningKey` untuk nilai previous jwt signing kunci dengan memanggil `Environment.GetEnvironmentVariable`
        // dengan `”JWT_SIGNING_KEY”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var previousJwtSigningKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY");
        // Menyiapkan variabel lokal `previousJwtSectionSigningKey` untuk nilai previous jwt section signing kunci dengan memanggil
        // `Environment.GetEnvironmentVariable` dengan `”Jwt__SigningKey”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var previousJwtSectionSigningKey = Environment.GetEnvironmentVariable("Jwt__SigningKey");

        // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”ConnectionStrings__Default”`, `connectionString` dalam
        // RunWithConnectionStringAsync.
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", connectionString);
        // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”JWT_SIGNING_KEY”`, `JwtSigningKey` dalam RunWithConnectionStringAsync.
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", JwtSigningKey);
        // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”Jwt__SigningKey”`, `JwtSigningKey` dalam RunWithConnectionStringAsync.
        Environment.SetEnvironmentVariable("Jwt__SigningKey", JwtSigningKey);

        // Memulai blok try dalam RunWithConnectionStringAsync; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap
        // dijalankan saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RunWithConnectionStringAsync.
        {
            // Menjalankan hasil operasi asinkron memanggil `action` dengan tanpa argumen; await menunggu hasil tanpa memblokir thread selama operasi belum
            // selesai dalam RunWithConnectionStringAsync.
            await action();
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam RunWithConnectionStringAsync.
        }
        // Menjalankan blok finally ketika alur meninggalkan try/catch, termasuk saat terjadi exception dalam RunWithConnectionStringAsync; bagian ini
        // dipakai untuk pekerjaan penutup yang harus tetap dilakukan.
        finally
        // Membuka scope pekerjaan penutup finally; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RunWithConnectionStringAsync.
        {
            // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”ConnectionStrings__Default”`, `previousConnectionString` dalam
            // RunWithConnectionStringAsync.
            Environment.SetEnvironmentVariable("ConnectionStrings__Default", previousConnectionString);
            // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”JWT_SIGNING_KEY”`, `previousJwtSigningKey` dalam
            // RunWithConnectionStringAsync.
            Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", previousJwtSigningKey);
            // Menjalankan memanggil `Environment.SetEnvironmentVariable` dengan `”Jwt__SigningKey”`, `previousJwtSectionSigningKey` dalam
            // RunWithConnectionStringAsync.
            Environment.SetEnvironmentVariable("Jwt__SigningKey", previousJwtSectionSigningKey);
        // Menutup scope pekerjaan penutup finally; bagian berikut berada di luar batas blok tersebut dalam RunWithConnectionStringAsync.
        }
    // Menutup scope metode RunWithConnectionStringAsync; bagian berikut berada di luar batas blok tersebut dalam RunWithConnectionStringAsync.
    }

    // Mendefinisikan metode `AssertHasUniqueIndexAsync` dengan hasil bertipe `Task`; operasi ini menangani assert memiliki unique index asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `connection` bertipe
    // `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tableName` bertipe `string`
    // membawa nilai table nama; Parameter `columns` bertipe `string[]` membawa nilai columns.
    private static async Task AssertHasUniqueIndexAsync(NpgsqlConnection connection, string tableName, string[] columns)
    // Membuka scope metode AssertHasUniqueIndexAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AssertHasUniqueIndexAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select exists (`.
        // Baris literal 3: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 4: FROM memilih tabel/subquery sumber pembacaan: `from pg_index i`.
        // Baris literal 5: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join pg_class t on t.oid = i.indrelid`.
        // Baris literal 6: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where t.relname = @tableName`.
        // Baris literal 7: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and i.indisunique`.
        // Baris literal 8: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (`.
        // Baris literal 9: SELECT menentukan nilai atau kolom yang dikembalikan query: `select array_agg(a.attname order by ord.ordinality)`.
        // Baris literal 10: FROM memilih tabel/subquery sumber pembacaan: `from unnest(i.indkey) with ordinality as ord(attnum, ordinality)`.
        // Baris literal 11: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join pg_attribute a on a.attrelid = t.oid and a.attnum =
        // ord.attnum`.
        // Baris literal 12: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ord.attnum > 0`.
        // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `)::text[] = @columns`.
        // Baris literal 14: Pembatas literal/penutup `);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 15: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select exists (
                select 1
                from pg_index i
                join pg_class t on t.oid = i.indrelid
                where t.relname = @tableName
                  and i.indisunique
                  and (
                      select array_agg(a.attname order by ord.ordinality)
                      from unnest(i.indkey) with ordinality as ord(attnum, ordinality)
                      join pg_attribute a on a.attrelid = t.oid and a.attnum = ord.attnum
                      where ord.attnum > 0
                  )::text[] = @columns
            );
            """;

        // Menyiapkan variabel lokal `hasUniqueIndex` untuk nilai memiliki unique index dengan hasil operasi asinkron menjalankan perintah basis data
        // melalui `connection` dengan `sql`, `new { tableName, columns }` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread
        // selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var hasUniqueIndex = await connection.ExecuteScalarAsync<bool>(sql, new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // AssertHasUniqueIndexAsync.
        {
            // Meneruskan objek anonim yang mengelompokkan tableName, columns sebagai satu nilai sebagai argumen ke `connection.ExecuteScalarAsync<bool>`.
            tableName,
            // Meneruskan objek anonim yang mengelompokkan tableName, columns sebagai satu nilai sebagai argumen ke `connection.ExecuteScalarAsync<bool>`.
            columns
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam AssertHasUniqueIndexAsync.
        });

        // Menjalankan pemeriksaan bahwa `hasUniqueIndex`, `$”Expected unique index/constraint on {tableName}({string.Join(”, ”, columns)}), but none was
        // found.”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam AssertHasUniqueIndexAsync.
        Assert.True(
            // Meneruskan `hasUniqueIndex` (nilai memiliki unique index) sebagai argumen ke `Assert.True`.
            hasUniqueIndex,
            // Meneruskan teks interpolasi `$”Expected unique index/constraint on {tableName}({string.Join(”, ”, columns)}), but none was found.”`; nilai
            // ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `Assert.True`; Meneruskan nilai literal `”, ”` sebagai
            // argumen ke `string.Join`; Meneruskan `columns` (nilai columns) sebagai argumen ke `string.Join`.
            $"Expected unique index/constraint on {tableName}({string.Join(", ", columns)}), but none was found.");
    // Menutup scope metode AssertHasUniqueIndexAsync; bagian berikut berada di luar batas blok tersebut dalam AssertHasUniqueIndexAsync.
    }
// Menutup scope tipe DatabaseStartupIntegrationTests; bagian berikut berada di luar batas blok tersebut.
}
