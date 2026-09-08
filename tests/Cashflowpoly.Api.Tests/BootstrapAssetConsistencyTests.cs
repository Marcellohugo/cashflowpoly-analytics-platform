// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui BootstrapAssetConsistencyTests.
// Mengimpor namespace `System.Text.RegularExpressions` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.RegularExpressions;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `BootstrapAssetConsistencyTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class BootstrapAssetConsistencyTests
// Membuka scope tipe BootstrapAssetConsistencyTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `RepoRoot` menyimpan nilai repo root dengan nilai awal memanggil `ResolveRepositoryRoot` dengan tanpa
    // argumen. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan
    // dibagikan antar instance.
    private static readonly string RepoRoot = ResolveRepositoryRoot();

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed` dengan hasil bertipe `void`; operasi ini
    // menangani bootstrap aset daftar should include versioned migrations dan optional simulation seed.
    public void BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed()
    // Membuka scope metode BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed; pernyataan/deklarasi berikut berada di dalam
    // batas blok ini dalam BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed.
    {
        // Menyiapkan variabel lokal `initializerPath` untuk nilai initializer path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`,
        // `”Cashflowpoly.Api”`, `”Infrastructure”`, `”DatabaseInitialization.cs”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var initializerPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Infrastructure", "DatabaseInitialization.cs");
        // Menyiapkan variabel lokal `apiProjectPath` untuk nilai api project path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`,
        // `”Cashflowpoly.Api”`, `”Cashflowpoly.Api.csproj”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var apiProjectPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Cashflowpoly.Api.csproj");
        // Menyiapkan variabel lokal `apiTestProjectPath` untuk nilai api test project path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”tests”`,
        // `”Cashflowpoly.Api.Tests”`, `”Cashflowpoly.Api.Tests.csproj”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var apiTestProjectPath = Path.Combine(RepoRoot, "tests", "Cashflowpoly.Api.Tests", "Cashflowpoly.Api.Tests.csproj");

        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”02_seed_full_inspection.sql”`,
        // `File.ReadAllText(initializerPath)`, `StringComparison.Ordinal` dalam
        // BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed.
        Assert.DoesNotContain("02_seed_full_inspection.sql", File.ReadAllText(initializerPath), StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”02_seed_full_inspection.sql”`,
        // `File.ReadAllText(apiProjectPath)`, `StringComparison.Ordinal` dalam
        // BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed.
        Assert.DoesNotContain("02_seed_full_inspection.sql", File.ReadAllText(apiProjectPath), StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”02_seed_full_inspection.sql”`,
        // `File.ReadAllText(apiTestProjectPath)`, `StringComparison.Ordinal` dalam
        // BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed.
        Assert.DoesNotContain("02_seed_full_inspection.sql", File.ReadAllText(apiTestProjectPath), StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”03_seed_inspection_matrix.sql”`,
        // `File.ReadAllText(initializerPath)`, `StringComparison.Ordinal` dalam
        // BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed.
        Assert.DoesNotContain("03_seed_inspection_matrix.sql", File.ReadAllText(initializerPath), StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”03_seed_inspection_matrix.sql”`,
        // `File.ReadAllText(apiProjectPath)`, `StringComparison.Ordinal` dalam
        // BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed.
        Assert.DoesNotContain("03_seed_inspection_matrix.sql", File.ReadAllText(apiProjectPath), StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”03_seed_inspection_matrix.sql”`,
        // `File.ReadAllText(apiTestProjectPath)`, `StringComparison.Ordinal` dalam
        // BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed.
        Assert.DoesNotContain("03_seed_inspection_matrix.sql", File.ReadAllText(apiTestProjectPath), StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”03_session_state_seed.sql”`,
        // `File.ReadAllText(initializerPath)`, `StringComparison.Ordinal` dalam
        // BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed.
        Assert.DoesNotContain("03_session_state_seed.sql", File.ReadAllText(initializerPath), StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”03_session_state_seed.sql”`,
        // `File.ReadAllText(apiProjectPath)`, `StringComparison.Ordinal` dalam
        // BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed.
        Assert.DoesNotContain("03_session_state_seed.sql", File.ReadAllText(apiProjectPath), StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”03_session_state_seed.sql”`,
        // `File.ReadAllText(apiTestProjectPath)`, `StringComparison.Ordinal` dalam
        // BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed.
        Assert.DoesNotContain("03_session_state_seed.sql", File.ReadAllText(apiTestProjectPath), StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”02_seed_simulation_sessions_events.sql”`, `File.ReadAllText(initializerPath)`, `StringComparison.Ordinal` dalam
        // BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed.
        Assert.Contains("02_seed_simulation_sessions_events.sql", File.ReadAllText(initializerPath), StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”02_seed_simulation_sessions_events.sql”`, `File.ReadAllText(apiProjectPath)`, `StringComparison.Ordinal` dalam
        // BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed.
        Assert.Contains("02_seed_simulation_sessions_events.sql", File.ReadAllText(apiProjectPath), StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”02_seed_simulation_sessions_events.sql”`, `File.ReadAllText(apiTestProjectPath)`, `StringComparison.Ordinal` dalam
        // BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed.
        Assert.Contains("02_seed_simulation_sessions_events.sql", File.ReadAllText(apiTestProjectPath), StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”database\\migrations”`,
        // `File.ReadAllText(apiProjectPath)`, `StringComparison.Ordinal` dalam
        // BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed.
        Assert.Contains("database\\migrations", File.ReadAllText(apiProjectPath), StringComparison.Ordinal);
    // Menutup scope metode BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed; bagian berikut berada di luar batas blok
    // tersebut dalam BootstrapAssetList_ShouldIncludeVersionedMigrationsAndOptionalSimulationSeed.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Bootstrap_ShouldUseChecksummedMigrationHistory` dengan hasil bertipe `void`; operasi ini menangani bootstrap should use
    // checksummed migration history.
    public void Bootstrap_ShouldUseChecksummedMigrationHistory()
    // Membuka scope metode Bootstrap_ShouldUseChecksummedMigrationHistory; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Bootstrap_ShouldUseChecksummedMigrationHistory.
    {
        // Menyiapkan variabel lokal `initializerPath` untuk nilai initializer path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`,
        // `”Cashflowpoly.Api”`, `”Infrastructure”`, `”DatabaseInitialization.cs”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var initializerPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Infrastructure", "DatabaseInitialization.cs");
        // Menyiapkan variabel lokal `initializerContent` untuk nilai initializer content dengan memanggil `File.ReadAllText` dengan `initializerPath`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var initializerContent = File.ReadAllText(initializerPath);

        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”__EFMigrationsHistory”`,
        // `initializerContent`, `StringComparison.Ordinal` dalam Bootstrap_ShouldUseChecksummedMigrationHistory.
        Assert.DoesNotContain("__EFMigrationsHistory", initializerContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `string.Concat(”Baseline”, ”Le”,
        // ”gacy”, ”SchemaAsync”)`, `initializerContent`, `StringComparison.Ordinal` dalam Bootstrap_ShouldUseChecksummedMigrationHistory.
        Assert.DoesNotContain(string.Concat("Baseline", "Le", "gacy", "SchemaAsync"), initializerContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”InitialSchema”`,
        // `initializerContent`, `StringComparison.Ordinal` dalam Bootstrap_ShouldUseChecksummedMigrationHistory.
        Assert.DoesNotContain("InitialSchema", initializerContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”SchemaParityAndDefaultSeed”`,
        // `initializerContent`, `StringComparison.Ordinal` dalam Bootstrap_ShouldUseChecksummedMigrationHistory.
        Assert.DoesNotContain("SchemaParityAndDefaultSeed", initializerContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”SessionStateSchemaConsolidation”`, `initializerContent`, `StringComparison.Ordinal` dalam Bootstrap_ShouldUseChecksummedMigrationHistory.
        Assert.DoesNotContain("SessionStateSchemaConsolidation", initializerContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”schema_history”`, `initializerContent`,
        // `StringComparison.Ordinal` dalam Bootstrap_ShouldUseChecksummedMigrationHistory.
        Assert.Contains("schema_history", initializerContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”SHA256”`, `initializerContent`,
        // `StringComparison.Ordinal` dalam Bootstrap_ShouldUseChecksummedMigrationHistory.
        Assert.Contains("SHA256", initializerContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”--migrate-only”`,
        // `File.ReadAllText(Path.Combine(RepoRoot, ”src”, ”Cashflowpoly.Api”, ”Program.cs”))`, `StringComparison.Ordinal` dalam
        // Bootstrap_ShouldUseChecksummedMigrationHistory.
        Assert.Contains("--migrate-only", File.ReadAllText(Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Program.cs")), StringComparison.Ordinal);
    // Menutup scope metode Bootstrap_ShouldUseChecksummedMigrationHistory; bagian berikut berada di luar batas blok tersebut dalam
    // Bootstrap_ShouldUseChecksummedMigrationHistory.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots` dengan hasil bertipe `void`; operasi ini menangani aturan
    // contracts should not expose instruktur pemain username slots.
    public void RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots()
    // Membuka scope metode RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
    {
        // Menyiapkan variabel lokal `checkedFiles` untuk nilai checked files dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var checkedFiles = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
        {
            // Melanjutkan pengolahan dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`, `”Cashflowpoly.Api”`, `”Contracts”`,
            // `”RulesetDefinitionDtos.cs”` dalam RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Contracts", "RulesetDefinitionDtos.cs"),
            // Melanjutkan pengolahan dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`, `”Cashflowpoly.Ui”`, `”Contracts”`,
            // `”RulesetDefinitionDtos.cs”` dalam RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Contracts", "RulesetDefinitionDtos.cs"),
            // Melanjutkan pengolahan dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`, `”Cashflowpoly.Api”`, `”Domain”`,
            // `”RulesetDefinitionMapper.cs”` dalam RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Domain", "RulesetDefinitionMapper.cs"),
            // Melanjutkan pengolahan dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`, `”Cashflowpoly.Ui”`, `”Domain”`,
            // `”RulesetDefinitionMapper.cs”` dalam RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Domain", "RulesetDefinitionMapper.cs"),
            // Melanjutkan pengolahan dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`, `”Cashflowpoly.Api”`, `”Data”`, `”RulesetRepository.cs”` dalam
            // RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Data", "RulesetRepository.cs"),
            // Melanjutkan pengolahan dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`, `”Cashflowpoly.Ui”`, `”Infrastructure”`,
            // `”UiTextLexicon.Rulesets.cs”` dalam RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Infrastructure", "UiTextLexicon.Rulesets.cs"),
            // Melanjutkan pengolahan dengan memanggil `Path.Combine` dengan `RepoRoot`, `”postman”`, `”Cashflowpoly.postman_collection.json”` dalam
            // RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
            Path.Combine(RepoRoot, "postman", "Cashflowpoly.postman_collection.json")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
        };

        // Mengulangi setiap elemen `checkedFiles`; elemen saat ini disimpan sebagai `path` bertipe `var` untuk diproses oleh badan loop dalam
        // RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
        foreach (var path in checkedFiles)
        // Membuka scope loop setiap path dari `checkedFiles`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
        {
            // Menyiapkan variabel lokal `content` untuk nilai content dengan memanggil `File.ReadAllText` dengan `path`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal.
            var content = File.ReadAllText(path);
            // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `string.Concat(”instructor”,
            // ”_player”, ”_usernames”)`, `content`, `StringComparison.OrdinalIgnoreCase` dalam RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
            Assert.DoesNotContain(string.Concat("instructor", "_player", "_usernames"), content, StringComparison.OrdinalIgnoreCase);
            // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `string.Concat(”Instructor”,
            // ”Player”, ”Usernames”)`, `content`, `StringComparison.Ordinal` dalam RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
            Assert.DoesNotContain(string.Concat("Instructor", "Player", "Usernames"), content, StringComparison.Ordinal);
            // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `string.Concat(”player”, ”_slots”,
            // ”_title”)`, `content`, `StringComparison.OrdinalIgnoreCase` dalam RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
            Assert.DoesNotContain(string.Concat("player", "_slots", "_title"), content, StringComparison.OrdinalIgnoreCase);
        // Menutup scope loop setiap path dari `checkedFiles`; bagian berikut berada di luar batas blok tersebut dalam
        // RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
        }
    // Menutup scope metode RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots; bagian berikut berada di luar batas blok tersebut dalam
    // RulesetContracts_ShouldNotExposeInstructorPlayerUsernameSlots.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetContracts_ShouldNotExposeAssignmentOrderingAliases` dengan hasil bertipe `void`; operasi ini menangani aturan
    // contracts should not expose assignment ordering aliases.
    public void RulesetContracts_ShouldNotExposeAssignmentOrderingAliases()
    // Membuka scope metode RulesetContracts_ShouldNotExposeAssignmentOrderingAliases; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
    {
        // Menyiapkan variabel lokal `checkedFiles` untuk nilai checked files dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var checkedFiles = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
        {
            // Melanjutkan pengolahan dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`, `”00_create_schema.sql”` dalam
            // RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
            Path.Combine(RepoRoot, "database", "00_create_schema.sql"),
            // Melanjutkan pengolahan dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`, `”Cashflowpoly.Api”`, `”Domain”`, `”RulesetConfig.cs”` dalam
            // RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Domain", "RulesetConfig.cs"),
            // Melanjutkan pengolahan dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`, `”Cashflowpoly.Api”`, `”Domain”`,
            // `”AnalyticsPlayerOrdering.cs”` dalam RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Domain", "AnalyticsPlayerOrdering.cs"),
            // Melanjutkan pengolahan dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`, `”Cashflowpoly.Ui”`, `”Infrastructure”`,
            // `”UiTextLexicon.Rulesets.cs”` dalam RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Infrastructure", "UiTextLexicon.Rulesets.cs"),
            // Melanjutkan pengolahan dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`, `”Cashflowpoly.Ui”`, `”Views”`, `”Rulesets”`,
            // `”Details.cshtml”` dalam RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Views", "Rulesets", "Details.cshtml"),
            // Melanjutkan pengolahan dengan memanggil `Path.Combine` dengan `RepoRoot`, `”tests”`, `”Cashflowpoly.Api.Tests”`,
            // `”EventAnalyticsIntegrationTests.cs”` dalam RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
            Path.Combine(RepoRoot, "tests", "Cashflowpoly.Api.Tests", "EventAnalyticsIntegrationTests.cs")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
        };

        // Mengulangi setiap elemen `checkedFiles`; elemen saat ini disimpan sebagai `path` bertipe `var` untuk diproses oleh badan loop dalam
        // RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
        foreach (var path in checkedFiles)
        // Membuka scope loop setiap path dari `checkedFiles`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
        {
            // Menyiapkan variabel lokal `content` untuk nilai content dengan memanggil `File.ReadAllText` dengan `path`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal.
            var content = File.ReadAllText(path);
            // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `string.Concat(”INSTRUCTOR”,
            // ”_ORDER”)`, `content`, `StringComparison.OrdinalIgnoreCase` dalam RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
            Assert.DoesNotContain(string.Concat("INSTRUCTOR", "_ORDER"), content, StringComparison.OrdinalIgnoreCase);
            // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `string.Concat(”MANUAL”,
            // ”_ORDER”)`, `content`, `StringComparison.OrdinalIgnoreCase` dalam RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
            Assert.DoesNotContain(string.Concat("MANUAL", "_ORDER"), content, StringComparison.OrdinalIgnoreCase);
            // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `string.Concat(”USERNAME”,
            // ”_IDN”)`, `content`, `StringComparison.OrdinalIgnoreCase` dalam RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
            Assert.DoesNotContain(string.Concat("USERNAME", "_IDN"), content, StringComparison.OrdinalIgnoreCase);
            // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `string.Concat(”Instructor”,
            // ”Order”)`, `content`, `StringComparison.Ordinal` dalam RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
            Assert.DoesNotContain(string.Concat("Instructor", "Order"), content, StringComparison.Ordinal);
            // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `string.Concat(”player”,
            // ”_ordering”, ”_instructor”, ”_order”)`, `content`, `StringComparison.OrdinalIgnoreCase` dalam
            // RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
            Assert.DoesNotContain(string.Concat("player", "_ordering", "_instructor", "_order"), content, StringComparison.OrdinalIgnoreCase);
        // Menutup scope loop setiap path dari `checkedFiles`; bagian berikut berada di luar batas blok tersebut dalam
        // RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
        }
    // Menutup scope metode RulesetContracts_ShouldNotExposeAssignmentOrderingAliases; bagian berikut berada di luar batas blok tersebut dalam
    // RulesetContracts_ShouldNotExposeAssignmentOrderingAliases.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetContracts_ShouldNotExposeNeedFamilyTable` dengan hasil bertipe `void`; operasi ini menangani aturan contracts
    // should not expose kebutuhan kelompok table.
    public void RulesetContracts_ShouldNotExposeNeedFamilyTable()
    // Membuka scope metode RulesetContracts_ShouldNotExposeNeedFamilyTable; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // RulesetContracts_ShouldNotExposeNeedFamilyTable.
    {
        // Menyiapkan variabel lokal `checkedFiles` untuk nilai checked files dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var checkedFiles = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // RulesetContracts_ShouldNotExposeNeedFamilyTable.
        {
            // Melanjutkan pengolahan dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`, `”00_create_schema.sql”` dalam
            // RulesetContracts_ShouldNotExposeNeedFamilyTable.
            Path.Combine(RepoRoot, "database", "00_create_schema.sql"),
            // Melanjutkan pengolahan dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`, `”Cashflowpoly.Api”`, `”Data”`, `”RulesetRepository.cs”` dalam
            // RulesetContracts_ShouldNotExposeNeedFamilyTable.
            Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Data", "RulesetRepository.cs")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // RulesetContracts_ShouldNotExposeNeedFamilyTable.
        };

        // Mengulangi setiap elemen `checkedFiles`; elemen saat ini disimpan sebagai `path` bertipe `var` untuk diproses oleh badan loop dalam
        // RulesetContracts_ShouldNotExposeNeedFamilyTable.
        foreach (var path in checkedFiles)
        // Membuka scope loop setiap path dari `checkedFiles`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // RulesetContracts_ShouldNotExposeNeedFamilyTable.
        {
            // Menyiapkan variabel lokal `content` untuk nilai content dengan memanggil `File.ReadAllText` dengan `path`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal.
            var content = File.ReadAllText(path);
            // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `string.Concat(”ruleset”, ”_need”,
            // ”_families”)`, `content`, `StringComparison.OrdinalIgnoreCase` dalam RulesetContracts_ShouldNotExposeNeedFamilyTable.
            Assert.DoesNotContain(string.Concat("ruleset", "_need", "_families"), content, StringComparison.OrdinalIgnoreCase);
        // Menutup scope loop setiap path dari `checkedFiles`; bagian berikut berada di luar batas blok tersebut dalam
        // RulesetContracts_ShouldNotExposeNeedFamilyTable.
        }
    // Menutup scope metode RulesetContracts_ShouldNotExposeNeedFamilyTable; bagian berikut berada di luar batas blok tersebut dalam
    // RulesetContracts_ShouldNotExposeNeedFamilyTable.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables` dengan hasil bertipe `void`; operasi ini
    // menangani canonical schema should use normalized event first aset registry dan projection tables.
    public void CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables()
    // Membuka scope metode CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables; pernyataan/deklarasi berikut berada di dalam
    // batas blok ini dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
    {
        // Menyiapkan variabel lokal `schemaPath` untuk nilai schema path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”00_create_schema.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        // Menyiapkan variabel lokal `schemaContent` untuk nilai schema content dengan memanggil `File.ReadAllText` dengan `schemaPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var schemaContent = File.ReadAllText(schemaPath);
        // Menyiapkan variabel lokal `sessionParticipantsTable` untuk nilai sesi participants table dengan memanggil `ExtractCreateTable` dengan
        // `schemaContent`, `”session_participants”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionParticipantsTable = ExtractCreateTable(schemaContent, "session_participants");
        // Menyiapkan variabel lokal `gameAssetsTable` untuk nilai game aset table dengan memanggil `ExtractCreateTable` dengan `schemaContent`,
        // `”ruleset_game_assets”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var gameAssetsTable = ExtractCreateTable(schemaContent, "ruleset_game_assets");

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create extension if not exists
        // citext”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create extension if not exists citext", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // session_participants”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists session_participants", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player_order_no”`, `schemaContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("player_order_no", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”player_name varchar(80)”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("player_name varchar(80)", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset_version_id uuid not null”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("ruleset_version_id uuid not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // session_ruleset_activations”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("create table if not exists session_ruleset_activations", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”role varchar(20)”`,
        // `sessionParticipantsTable`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("role varchar(20)", sessionParticipantsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // ruleset_game_assets”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists ruleset_game_assets", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset_game_asset_id uuid not null”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("ruleset_game_asset_id uuid not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”asset_type varchar(40) not null”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("asset_type varchar(40) not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”constraint uq_ruleset_game_assets_scope
        // unique (ruleset_version_id, asset_type, asset_code)”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("constraint uq_ruleset_game_assets_scope unique (ruleset_version_id, asset_type, asset_code)", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'QUEST'”`, `gameAssetsTable`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("'QUEST'", gameAssetsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'NARRATIVE'”`, `gameAssetsTable`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("'NARRATIVE'", gameAssetsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'COLLECTION_MISSION'”`,
        // `gameAssetsTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("'COLLECTION_MISSION'", gameAssetsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'FINANCIAL_GOAL'”`,
        // `gameAssetsTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("'FINANCIAL_GOAL'", gameAssetsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'LOAN'”`, `gameAssetsTable`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("'LOAN'", gameAssetsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'INSURANCE'”`, `gameAssetsTable`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("'INSURANCE'", gameAssetsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // event_asset_references”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists event_asset_references", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”references ruleset_game_assets
        // (ruleset_version_id, ruleset_game_asset_id)”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains(
            // Meneruskan nilai literal `”references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id)”` sebagai argumen ke `Assert.Contains`.
            "references ruleset_game_assets (ruleset_version_id, ruleset_game_asset_id)",
            // Meneruskan `schemaContent` (nilai schema content) sebagai argumen ke `Assert.Contains`.
            schemaContent,
            // Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `Assert.Contains`.
            StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset_action_id uuid not null”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("ruleset_action_id uuid not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”foreign key (ruleset_version_id,
        // ruleset_action_id)”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains(
            // Meneruskan nilai literal `”foreign key (ruleset_version_id, ruleset_action_id)”` sebagai argumen ke `Assert.Contains`.
            "foreign key (ruleset_version_id, ruleset_action_id)",
            // Meneruskan `schemaContent` (nilai schema content) sebagai argumen ke `Assert.Contains`.
            schemaContent,
            // Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `Assert.Contains`.
            StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // session_participant_inventory”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists session_participant_inventory", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // session_participant_gold_holdings”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists session_participant_gold_holdings", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // session_participant_loans”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists session_participant_loans", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // session_participant_insurances”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists session_participant_insurances", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan `”create table if not exists ruleset_insurance_products[\\s\\S]*is_active
        // boolean not null default true”`, `schemaContent`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Matches(
            // Meneruskan nilai literal `”create table if not exists ruleset_insurance_products[\\s\\S]*is_active boolean not null default true”` sebagai
            // argumen ke `Assert.Matches`.
            "create table if not exists ruleset_insurance_products[\\s\\S]*is_active boolean not null default true",
            // Meneruskan `schemaContent` (nilai schema content) sebagai argumen ke `Assert.Matches`.
            schemaContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // session_participant_tie_breakers”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists session_participant_tie_breakers", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”uq_ruleset_tie_breakers_ruleset_number”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("uq_ruleset_tie_breakers_ruleset_number", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”uq_session_collection_missions_session_card”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("uq_session_collection_missions_session_card", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // session_final_scores”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists session_final_scores", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // session_final_score_components”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists session_final_score_components", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”foreign key (session_id,
        // source_event_id)”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains(
            // Meneruskan nilai literal `”foreign key (session_id, source_event_id)”` sebagai argumen ke `Assert.Contains`.
            "foreign key (session_id, source_event_id)",
            // Meneruskan `schemaContent` (nilai schema content) sebagai argumen ke `Assert.Contains`.
            schemaContent,
            // Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `Assert.Contains`.
            StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”last_event_id uuid null”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("last_event_id uuid null", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // session_card_positions”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists session_card_positions", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”card_instance_id uuid not null”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("card_instance_id uuid not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”fk_session_card_positions_last_event_id”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("fk_session_card_positions_last_event_id", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // ruleset_need_set_bonuses”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists ruleset_need_set_bonuses", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // ruleset_game_settings”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists ruleset_game_settings", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // ruleset_trigger_conditions”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists ruleset_trigger_conditions", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset_action_id uuid not null”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("ruleset_action_id uuid not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // ruleset_narrative_scenes”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists ruleset_narrative_scenes", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // session_narrative_logs”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists session_narrative_logs", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // ruleset_quests”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("create table if not exists ruleset_quests", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // session_participant_quest_progress”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("create table if not exists session_participant_quest_progress", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // session_projection_checkpoints”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists session_projection_checkpoints", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”fk_session_projection_checkpoints_last_event_id”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("fk_session_projection_checkpoints_last_event_id", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”last_event_pk”`, `schemaContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("last_event_pk", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan memanggil `AssertSqlContains` dengan `”create or replace function project_session_event”`, `schemaContent` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        AssertSqlContains("create or replace function project_session_event", schemaContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”create or replace function project_session_events”`, `schemaContent` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        AssertSqlContains("create or replace function project_session_events", schemaContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”create or replace function ensure_session_card_positions_initialized”`, `schemaContent` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        AssertSqlContains("create or replace function ensure_session_card_positions_initialized", schemaContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”create or replace function project_market_refill”`, `schemaContent` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        AssertSqlContains("create or replace function project_market_refill", schemaContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”existing.ruleset_game_asset_id =
        // rci.ruleset_catalog_item_id”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("existing.ruleset_game_asset_id = rci.ruleset_catalog_item_id", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan memanggil `AssertSqlContains` dengan `”create or replace function rebuild_session_projection”`, `schemaContent` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        AssertSqlContains("create or replace function rebuild_session_projection", schemaContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”SQL-only projection rebuild is disabled
        // to prevent destructive state loss”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("SQL-only projection rebuild is disabled to prevent destructive state loss", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”v_projected :=
        // project_session_events”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("v_projected := project_session_events", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”enforce_event_session_scope”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("enforce_event_session_scope", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”trg_events_session_scope”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("trg_events_session_scope", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan memanggil `AssertSqlContains` dengan `”new.day_index is distinct from v_current_day”`, `schemaContent` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        AssertSqlContains("new.day_index is distinct from v_current_day", schemaContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”new.action_type = 'AkhiriSesi' and v_current_day is distinct from v_finish_day”`,
        // `schemaContent` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        AssertSqlContains("new.action_type = 'AkhiriSesi' and v_current_day is distinct from v_finish_day", schemaContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”is_failed = not is_completed”`, `schemaContent` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        AssertSqlContains("is_failed = not is_completed", schemaContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”when v_event.action_type = 'AkhirGiliran' then v_event.day_index + 1”`, `schemaContent` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        AssertSqlContains("when v_event.action_type = 'AkhirGiliran' then v_event.day_index + 1", schemaContent);
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan
        // `”(?is)new\\.action_type\\s+in\\s*\\([^)]*'CatatTransaksi'[^)]*'AkhirGiliran'”`, `schemaContent`; ketidaksesuaian dengan ekspektasi membuat
        // pengujian gagal dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Matches(
            // Meneruskan nilai literal `”(?is)new\\.action_type\\s+in\\s*\\([^)]*'CatatTransaksi'[^)]*'AkhirGiliran'”` sebagai argumen ke `Assert.Matches`.
            "(?is)new\\.action_type\\s+in\\s*\\([^)]*'CatatTransaksi'[^)]*'AkhirGiliran'",
            // Meneruskan `schemaContent` (nilai schema content) sebagai argumen ke `Assert.Matches`.
            schemaContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `string.Concat(”create table if not
        // exists ruleset”, ”_player”, ”_ordering”, ”_instructor”, ”_users”)`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain(
            // Meneruskan menggabungkan `string` dengan `”create table if not exists ruleset”`, `”_player”`, `”_ordering”`, `”_instructor”`, `”_users”` pada
            // urutan hasil sebagai argumen ke `Assert.DoesNotContain`; Meneruskan nilai literal `”create table if not exists ruleset”` sebagai argumen ke
            // `string.Concat`; Meneruskan nilai literal `”_player”` sebagai argumen ke `string.Concat`; Meneruskan nilai literal `”_ordering”` sebagai argumen
            // ke `string.Concat`; Meneruskan nilai literal `”_instructor”` sebagai argumen ke `string.Concat`; Meneruskan nilai literal `”_users”` sebagai
            // argumen ke `string.Concat`.
            string.Concat("create table if not exists ruleset", "_player", "_ordering", "_instructor", "_users"),
            // Meneruskan `schemaContent` (nilai schema content) sebagai argumen ke `Assert.DoesNotContain`.
            schemaContent,
            // Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `Assert.DoesNotContain`.
            StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // ruleset_orders”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists ruleset_orders", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // ruleset_collection_mission_requirements”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists ruleset_collection_mission_requirements", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”created_by_user_id uuid null”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("created_by_user_id uuid null", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”created_by varchar”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("created_by varchar", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”payload_version varchar(20) not null
        // default '1.0'”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("payload_version varchar(20) not null default '1.0'", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”uq_events_session_client_request”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("uq_events_session_client_request", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”is_archived boolean not null default
        // false”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("is_archived boolean not null default false", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”foreign key (ruleset_version_id,
        // ruleset_game_asset_id)”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("foreign key (ruleset_version_id, ruleset_game_asset_id)", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”foreign key (ruleset_version_id,
        // required_asset_id)”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("foreign key (ruleset_version_id, required_asset_id)", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // session_participant_narrative_logs”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("create table if not exists session_participant_narrative_logs", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // event_cashflow_projections”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("create table if not exists event_cashflow_projections", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”payload jsonb not null”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("payload jsonb not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”raw_payload_json jsonb not null”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("raw_payload_json jsonb not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”metric_payload_json jsonb”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("metric_payload_json jsonb", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”last_event_id uuid”`, `schemaContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("last_event_id uuid", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”source_event_id uuid”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("source_event_id uuid", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”source_json jsonb”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("source_json jsonb", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”status varchar(20) not null
        // default 'NOT_STARTED'”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("status varchar(20) not null default 'NOT_STARTED'", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”enforce_session_ruleset_activation”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("enforce_session_ruleset_activation", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”enforce_session_card_position_catalog”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("enforce_session_card_position_catalog", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”coalesce(new.player_count, 0) <
        // coalesce(v_min_players, 2)”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("coalesce(new.player_count, 0) < coalesce(v_min_players, 2)", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ix_events_session_player_seq”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("ix_events_session_player_seq", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ix_events_received_at”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("ix_events_received_at", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ix_metric_snapshots_computed_at”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("ix_metric_snapshots_computed_at", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ix_validation_logs_created_at”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("ix_validation_logs_created_at", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”ix_event_cashflow_projections_event_pk”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("ix_event_cashflow_projections_event_pk", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”apply_default_log_retention_policies”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.Contains("apply_default_log_retention_policies", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”('events'”`, `schemaContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
        Assert.DoesNotContain("('events'", schemaContent, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables; bagian berikut berada di luar batas blok
    // tersebut dalam CanonicalSchema_ShouldUseNormalizedEventFirstAssetRegistryAndProjectionTables.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings` dengan hasil bertipe `void`; operasi ini menangani
    // canonical schema should use dynamic aksi slot limit dari aturan settings.
    public void CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings()
    // Membuka scope metode CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings.
    {
        // Menyiapkan variabel lokal `schemaPath` untuk nilai schema path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”00_create_schema.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        // Menyiapkan variabel lokal `schemaContent` untuk nilai schema content dengan memanggil `File.ReadAllText` dengan `schemaPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var schemaContent = File.ReadAllText(schemaPath);
        // Menyiapkan variabel lokal `eventsTable` untuk nilai event table dengan memanggil `ExtractCreateTable` dengan `schemaContent`, `”events”`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var eventsTable = ExtractCreateTable(schemaContent, "events");
        // Menyiapkan variabel lokal `sessionStatesTable` untuk nilai sesi states table dengan memanggil `ExtractCreateTable` dengan `schemaContent`,
        // `”session_states”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionStatesTable = ExtractCreateTable(schemaContent, "session_states");
        // Menyiapkan variabel lokal `narrativeLogsTable` untuk nilai narrative logs table dengan memanggil `ExtractCreateTable` dengan `schemaContent`,
        // `”session_narrative_logs”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var narrativeLogsTable = ExtractCreateTable(schemaContent, "session_narrative_logs");

        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”action_slot between 1 and 2”`,
        // `eventsTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings.
        Assert.DoesNotContain("action_slot between 1 and 2", eventsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”action_slot between 1 and 2”`,
        // `sessionStatesTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings.
        Assert.DoesNotContain("action_slot between 1 and 2", sessionStatesTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”action_slot between 1 and 2”`,
        // `narrativeLogsTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings.
        Assert.DoesNotContain("action_slot between 1 and 2", narrativeLogsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ck_events_action_slot check
        // (action_slot >= 0)”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings.
        Assert.Contains("ck_events_action_slot check (action_slot >= 0)", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ck_session_states_action_slot check
        // (action_slot >= 0)”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings.
        Assert.Contains("ck_session_states_action_slot check (action_slot >= 0)", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ck_session_states_current_action_slot
        // check (current_action_slot >= 0)”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings.
        Assert.Contains("ck_session_states_current_action_slot check (current_action_slot >= 0)", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ck_session_narrative_logs_action_slot
        // check (action_slot >= 1)”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings.
        Assert.Contains("ck_session_narrative_logs_action_slot check (action_slot >= 1)", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”enforce_action_slot_within_ruleset_limit”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings.
        Assert.Contains("enforce_action_slot_within_ruleset_limit", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan memanggil `AssertSqlContains` dengan `”tg_table_name = 'events' and new.actor_type = 'SYSTEM'”`, `schemaContent` dalam
        // CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings.
        AssertSqlContains("tg_table_name = 'events' and new.actor_type = 'SYSTEM'", schemaContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”tg_table_name = 'session_states' and coalesce(new.turn_number, 0) = 0”`, `schemaContent` dalam
        // CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings.
        AssertSqlContains("tg_table_name = 'session_states' and coalesce(new.turn_number, 0) = 0", schemaContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rgs.actions_per_turn”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings.
        Assert.Contains("rgs.actions_per_turn", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”trg_events_action_slot_limit”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings.
        Assert.Contains("trg_events_action_slot_limit", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”trg_session_states_action_slot_limit”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings.
        Assert.Contains("trg_session_states_action_slot_limit", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”trg_session_narrative_logs_action_slot_limit”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings.
        Assert.Contains("trg_session_narrative_logs_action_slot_limit", schemaContent, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings; bagian berikut berada di luar batas blok tersebut dalam
    // CanonicalSchema_ShouldUseDynamicActionSlotLimitFromRulesetSettings.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder` dengan hasil bertipe `void`; operasi ini menangani
    // canonical schema should carry giliran number dan validate pemain giliran urutan/pesanan.
    public void CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder()
    // Membuka scope metode CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder.
    {
        // Menyiapkan variabel lokal `schemaPath` untuk nilai schema path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”00_create_schema.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        // Menyiapkan variabel lokal `schemaContent` untuk nilai schema content dengan memanggil `File.ReadAllText` dengan `schemaPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var schemaContent = File.ReadAllText(schemaPath);
        // Menyiapkan variabel lokal `eventsTable` untuk nilai event table dengan memanggil `ExtractCreateTable` dengan `schemaContent`, `”events”`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var eventsTable = ExtractCreateTable(schemaContent, "events");
        // Menyiapkan variabel lokal `sessionStatesTable` untuk nilai sesi states table dengan memanggil `ExtractCreateTable` dengan `schemaContent`,
        // `”session_states”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionStatesTable = ExtractCreateTable(schemaContent, "session_states");

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”turn_number int not null”`,
        // `eventsTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder.
        Assert.Contains("turn_number int not null", eventsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”turn_number int not null”`,
        // `sessionStatesTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder.
        Assert.Contains("turn_number int not null", sessionStatesTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan memanggil `AssertSqlContains` dengan `”ck_events_turn_number check (turn_number between 0 and 4)”`, `eventsTable` dalam
        // CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder.
        AssertSqlContains("ck_events_turn_number check (turn_number between 0 and 4)", eventsTable);
        // Menjalankan memanggil `AssertSqlContains` dengan `”ck_session_states_turn_number check (turn_number between 0 and 4)”`, `sessionStatesTable`
        // dalam CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder.
        AssertSqlContains("ck_session_states_turn_number check (turn_number between 0 and 4)", sessionStatesTable);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ck_events_actor_turn_slot_shape”`,
        // `eventsTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder.
        Assert.Contains("ck_events_actor_turn_slot_shape", eventsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”new.actor_type = 'SYSTEM'”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder.
        Assert.Contains("new.actor_type = 'SYSTEM'", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”new.turn_number <> 0”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder.
        Assert.Contains("new.turn_number <> 0", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”new.action_slot <> 0”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder.
        Assert.Contains("new.action_slot <> 0", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”new.actor_type = 'PLAYER'”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder.
        Assert.Contains("new.actor_type = 'PLAYER'", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”new.turn_number <> v_player_order_no”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder.
        Assert.Contains("new.turn_number <> v_player_order_no", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”new.action_slot < 1”`, `schemaContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder.
        Assert.Contains("new.action_slot < 1", schemaContent, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder; bagian berikut berada di luar batas blok tersebut dalam
    // CanonicalSchema_ShouldCarryTurnNumberAndValidatePlayerTurnOrder.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CanonicalSchema_ShouldBindEventActionTypeToRulesetActionBehavior` dengan hasil bertipe `void`; operasi ini menangani
    // canonical schema should bind event aksi jenis ke aturan aksi behavior.
    public void CanonicalSchema_ShouldBindEventActionTypeToRulesetActionBehavior()
    // Membuka scope metode CanonicalSchema_ShouldBindEventActionTypeToRulesetActionBehavior; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam CanonicalSchema_ShouldBindEventActionTypeToRulesetActionBehavior.
    {
        // Menyiapkan variabel lokal `schemaPath` untuk nilai schema path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”00_create_schema.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        // Menyiapkan variabel lokal `schemaContent` untuk nilai schema content dengan memanggil `File.ReadAllText` dengan `schemaPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var schemaContent = File.ReadAllText(schemaPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”behavior_id varchar(80) not null”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldBindEventActionTypeToRulesetActionBehavior.
        Assert.Contains("behavior_id varchar(80) not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”v_behavior_id”`, `schemaContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldBindEventActionTypeToRulesetActionBehavior.
        Assert.Contains("v_behavior_id", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan memanggil `AssertSqlContains` dengan `”select a.mode, ra.behavior_id”`, `schemaContent` dalam
        // CanonicalSchema_ShouldBindEventActionTypeToRulesetActionBehavior.
        AssertSqlContains("select a.mode, ra.behavior_id", schemaContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”new.action_type is distinct from v_behavior_id”`, `schemaContent` dalam
        // CanonicalSchema_ShouldBindEventActionTypeToRulesetActionBehavior.
        AssertSqlContains("new.action_type is distinct from v_behavior_id", schemaContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Event action_type % must match ruleset
        // action behavior_id %”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldBindEventActionTypeToRulesetActionBehavior.
        Assert.Contains("Event action_type % must match ruleset action behavior_id %", schemaContent, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode CanonicalSchema_ShouldBindEventActionTypeToRulesetActionBehavior; bagian berikut berada di luar batas blok tersebut dalam
    // CanonicalSchema_ShouldBindEventActionTypeToRulesetActionBehavior.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency` dengan hasil bertipe `void`; operasi ini
    // menangani canonical schema should enforce runtime aset types kartu supply dan projection consistency.
    public void CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency()
    // Membuka scope metode CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency; pernyataan/deklarasi berikut berada di
    // dalam batas blok ini dalam CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency.
    {
        // Menyiapkan variabel lokal `schemaPath` untuk nilai schema path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”00_create_schema.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        // Menyiapkan variabel lokal `schemaContent` untuk nilai schema content dengan memanggil `File.ReadAllText` dengan `schemaPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var schemaContent = File.ReadAllText(schemaPath);
        // Menyiapkan variabel lokal `projectionsTable` untuk nilai projections table dengan memanggil `ExtractCreateTable` dengan `schemaContent`,
        // `”event_cashflow_projections”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projectionsTable = ExtractCreateTable(schemaContent, "event_cashflow_projections");

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”allowed_asset_type”`, `schemaContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency.
        Assert.Contains("allowed_asset_type", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session_participant_inventory' then
        // 'INGREDIENT'”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency.
        Assert.Contains("session_participant_inventory' then 'INGREDIENT'", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session_participant_gold_holdings' then
        // 'GOLD'”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency.
        Assert.Contains("session_participant_gold_holdings' then 'GOLD'", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session_participant_tie_breakers' then
        // 'TIE_BREAKER'”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency.
        Assert.Contains("session_participant_tie_breakers' then 'TIE_BREAKER'", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session_card_positions' then
        // 'CARD_POSITION'”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency.
        Assert.Contains("session_card_positions' then 'CARD_POSITION'", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”event_asset_references' then
        // 'EVENT_REFERENCE'”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency.
        Assert.Contains("event_asset_references' then 'EVENT_REFERENCE'", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”copy_number > coalesce(v_card_qty,
        // 0)”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency.
        Assert.Contains("copy_number > coalesce(v_card_qty, 0)", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan memanggil `AssertSqlContains` dengan `”new.zone = 'PLAYER' and new.owner_session_participant_id is null”`, `schemaContent` dalam
        // CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency.
        AssertSqlContains("new.zone = 'PLAYER' and new.owner_session_participant_id is null", schemaContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”new.zone <> 'PLAYER' and new.owner_session_participant_id is not null”`, `schemaContent` dalam
        // CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency.
        AssertSqlContains("new.zone <> 'PLAYER' and new.owner_session_participant_id is not null", schemaContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”projection_order int not null default
        // 1”`, `projectionsTable`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency.
        Assert.Contains("projection_order int not null default 1", projectionsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”unique (session_id, event_id,
        // projection_order)”`, `projectionsTable`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency.
        Assert.Contains("unique (session_id, event_id, projection_order)", projectionsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”enforce_event_cashflow_projection_consistency”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency.
        Assert.Contains("enforce_event_cashflow_projection_consistency", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”trg_event_cashflow_projection_consistency”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency.
        Assert.Contains("trg_event_cashflow_projection_consistency", schemaContent, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency; bagian berikut berada di luar batas blok
    // tersebut dalam CanonicalSchema_ShouldEnforceRuntimeAssetTypesCardSupplyAndProjectionConsistency.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements` dengan hasil bertipe `void`; operasi ini
    // menangani canonical schema should support sesi rule effects dan kebutuhan kelompok misi requirements.
    public void CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements()
    // Membuka scope metode CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements; pernyataan/deklarasi berikut berada di
    // dalam batas blok ini dalam CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
    {
        // Menyiapkan variabel lokal `schemaPath` untuk nilai schema path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”00_create_schema.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        // Menyiapkan variabel lokal `schemaContent` untuk nilai schema content dengan memanggil `File.ReadAllText` dengan `schemaPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var schemaContent = File.ReadAllText(schemaPath);
        // Menyiapkan variabel lokal `lifeRisksTable` untuk nilai life risks table dengan memanggil `ExtractCreateTable` dengan `schemaContent`,
        // `”ruleset_life_risks”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var lifeRisksTable = ExtractCreateTable(schemaContent, "ruleset_life_risks");
        // Menyiapkan variabel lokal `ruleEffectsTable` untuk nilai rule effects table dengan memanggil `ExtractCreateTable` dengan `schemaContent`,
        // `”session_rule_effects”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ruleEffectsTable = ExtractCreateTable(schemaContent, "session_rule_effects");
        // Menyiapkan variabel lokal `missionRequirementsTable` untuk nilai misi requirements table dengan memanggil `ExtractCreateTable` dengan
        // `schemaContent`, `”ruleset_collection_mission_requirements”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var missionRequirementsTable = ExtractCreateTable(schemaContent, "ruleset_collection_mission_requirements");

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”duration_days int not null default 1”`,
        // `lifeRisksTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
        Assert.Contains("duration_days int not null default 1", lifeRisksTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”target_scope varchar(40) not null
        // default 'SELF'”`, `lifeRisksTable`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
        Assert.Contains("target_scope varchar(40) not null default 'SELF'", lifeRisksTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ck_ruleset_life_risks_duration_days”`,
        // `lifeRisksTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
        Assert.Contains("ck_ruleset_life_risks_duration_days", lifeRisksTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ck_ruleset_life_risks_target_scope”`,
        // `lifeRisksTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
        Assert.Contains("ck_ruleset_life_risks_target_scope", lifeRisksTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session_rule_effect_id uuid primary key
        // default gen_random_uuid()”`, `ruleEffectsTable`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
        Assert.Contains("session_rule_effect_id uuid primary key default gen_random_uuid()", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”source_event_id uuid null”`,
        // `ruleEffectsTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
        Assert.Contains("source_event_id uuid null", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”effect_type varchar(60) not null”`,
        // `ruleEffectsTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
        Assert.Contains("effect_type varchar(60) not null", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”target_scope varchar(40) not null”`,
        // `ruleEffectsTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
        Assert.Contains("target_scope varchar(40) not null", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”value_delta int null”`,
        // `ruleEffectsTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
        Assert.Contains("value_delta int null", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”starts_day int not null”`,
        // `ruleEffectsTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
        Assert.Contains("starts_day int not null", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ends_day int null”`,
        // `ruleEffectsTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
        Assert.Contains("ends_day int null", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”metadata_json jsonb not null default
        // '{}' :: jsonb”`, `ruleEffectsTable`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
        Assert.Contains("metadata_json jsonb not null default '{}' :: jsonb", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”fk_session_rule_effects_session_id”`,
        // `ruleEffectsTable`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
        Assert.Contains("fk_session_rule_effects_session_id", ruleEffectsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”required_need_family_code varchar(120)
        // null”`, `missionRequirementsTable`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
        Assert.Contains("required_need_family_code varchar(120) null", missionRequirementsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'NEED_FAMILY'”`,
        // `missionRequirementsTable`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
        Assert.Contains("'NEED_FAMILY'", missionRequirementsTable, StringComparison.OrdinalIgnoreCase);
        // Menjalankan memanggil `AssertSqlContains` dengan `”create or replace function apply_game_event”`, `schemaContent` dalam
        // CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
        AssertSqlContains("create or replace function apply_game_event", schemaContent);
    // Menutup scope metode CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements; bagian berikut berada di luar batas blok
    // tersebut dalam CanonicalSchema_ShouldSupportSessionRuleEffectsAndNeedFamilyMissionRequirements.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CanonicalSchema_ShouldProjectRuntimeStateFromAuthoritativeRulesetData` dengan hasil bertipe `void`; operasi ini menangani
    // canonical schema should project runtime keadaan dari authoritative aturan data.
    public void CanonicalSchema_ShouldProjectRuntimeStateFromAuthoritativeRulesetData()
    // Membuka scope metode CanonicalSchema_ShouldProjectRuntimeStateFromAuthoritativeRulesetData; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam CanonicalSchema_ShouldProjectRuntimeStateFromAuthoritativeRulesetData.
    {
        // Menyiapkan variabel lokal `schemaPath` untuk nilai schema path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”00_create_schema.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        // Menyiapkan variabel lokal `schemaContent` untuk nilai schema content dengan memanggil `File.ReadAllText` dengan `schemaPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var schemaContent = File.ReadAllText(schemaPath);
        // Menyiapkan variabel lokal `projectorBody` untuk nilai projector body dengan memanggil `ExtractFunction` dengan `schemaContent`,
        // `”project_session_event”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projectorBody = ExtractFunction(schemaContent, "project_session_event");

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset_order_requirements”`,
        // `projectorBody`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldProjectRuntimeStateFromAuthoritativeRulesetData.
        Assert.Contains("ruleset_order_requirements", projectorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”jsonb_array_elements_text(coalesce(v_event.payload->'required_ingredient_card_ids'”`, `projectorBody`, `StringComparison.OrdinalIgnoreCase`
        // dalam CanonicalSchema_ShouldProjectRuntimeStateFromAuthoritativeRulesetData.
        Assert.DoesNotContain("jsonb_array_elements_text(coalesce(v_event.payload->'required_ingredient_card_ids'", projectorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”v_event.action_type = 'Menabung'”`,
        // `projectorBody`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldProjectRuntimeStateFromAuthoritativeRulesetData.
        Assert.Contains("v_event.action_type = 'Menabung'", projectorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”v_event.action_type =
        // 'TarikTabungan'”`, `projectorBody`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldProjectRuntimeStateFromAuthoritativeRulesetData.
        Assert.Contains("v_event.action_type = 'TarikTabungan'", projectorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”v_event.action_type =
        // 'TujuanFinansial'”`, `projectorBody`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldProjectRuntimeStateFromAuthoritativeRulesetData.
        Assert.Contains("v_event.action_type = 'TujuanFinansial'", projectorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session_participant_financial_goals”`,
        // `projectorBody`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldProjectRuntimeStateFromAuthoritativeRulesetData.
        Assert.Contains("session_participant_financial_goals", projectorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”happiness =
        // session_participant_balances.happiness +”`, `projectorBody`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldProjectRuntimeStateFromAuthoritativeRulesetData.
        Assert.Contains("happiness = session_participant_balances.happiness +", projectorBody, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode CanonicalSchema_ShouldProjectRuntimeStateFromAuthoritativeRulesetData; bagian berikut berada di luar batas blok tersebut
    // dalam CanonicalSchema_ShouldProjectRuntimeStateFromAuthoritativeRulesetData.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades` dengan hasil bertipe `void`; operasi ini menangani
    // canonical schema should validate emergency options terjual kebutuhan dan emas trades.
    public void CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades()
    // Membuka scope metode CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades.
    {
        // Menyiapkan variabel lokal `schemaPath` untuk nilai schema path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”00_create_schema.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        // Menyiapkan variabel lokal `schemaContent` untuk nilai schema content dengan memanggil `File.ReadAllText` dengan `schemaPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var schemaContent = File.ReadAllText(schemaPath);
        // Menyiapkan variabel lokal `scopeValidatorBody` untuk nilai cakupan validator body dengan memanggil `ExtractFunction` dengan `schemaContent`,
        // `”enforce_event_session_scope”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var scopeValidatorBody = ExtractFunction(schemaContent, "enforce_event_session_scope");
        // Menyiapkan variabel lokal `projectorBody` untuk nilai projector body dengan memanggil `ExtractFunction` dengan `schemaContent`,
        // `”project_session_event”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projectorBody = ExtractFunction(schemaContent, "project_session_event");

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”and not spnp.is_sold”`,
        // `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades.
        Assert.Contains("and not spnp.is_sold", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”v_option_type not in”`,
        // `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades.
        Assert.Contains("v_option_type not in", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'SELL_GOLD'”`, `scopeValidatorBody`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades.
        Assert.Contains("'SELL_GOLD'", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”GunakanOpsiDarurat event payload must
        // contain option_type”`, `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades.
        Assert.Contains("GunakanOpsiDarurat event payload must contain option_type", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”SELL_GOLD emergency option must contain
        // qty, unit_price, and amount”`, `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades.
        Assert.Contains("SELL_GOLD emergency option must contain qty, unit_price, and amount", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”v_active_gold_price”`,
        // `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades.
        Assert.Contains("v_active_gold_price", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”BukaHargaEmas”`, `scopeValidatorBody`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades.
        Assert.Contains("BukaHargaEmas", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Kepemilikan emas tidak cukup”`,
        // `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades.
        Assert.Contains("Kepemilikan emas tidak cukup", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”USE_INSURANCE”`, `projectorBody`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades.
        Assert.DoesNotContain("USE_INSURANCE", projectorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan `@”upper\(v_event\.payload\s*->>\s*'option_type'\)\s*=\s*'SELL_GOLD'”`,
        // `projectorBody`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades.
        Assert.Matches(@"upper\(v_event\.payload\s*->>\s*'option_type'\)\s*=\s*'SELL_GOLD'", projectorBody);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”when v_event.action_type =
        // 'GunakanOpsiDarurat' then upper(nullif(v_event.payload->>'direction”`, `projectorBody`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades.
        Assert.DoesNotContain("when v_event.action_type = 'GunakanOpsiDarurat' then upper(nullif(v_event.payload->>'direction", projectorBody, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades; bagian berikut berada di luar batas blok tersebut
    // dalam CanonicalSchema_ShouldValidateEmergencyOptionsSoldNeedsAndGoldTrades.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CanonicalSchema_ShouldUseLogicalIngredientDeckAndAtomicInsuranceClaims` dengan hasil bertipe `void`; operasi ini menangani
    // canonical schema should use logical bahan deck dan atomic asuransi claims.
    public void CanonicalSchema_ShouldUseLogicalIngredientDeckAndAtomicInsuranceClaims()
    // Membuka scope metode CanonicalSchema_ShouldUseLogicalIngredientDeckAndAtomicInsuranceClaims; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam CanonicalSchema_ShouldUseLogicalIngredientDeckAndAtomicInsuranceClaims.
    {
        // Menyiapkan variabel lokal `schemaPath` untuk nilai schema path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”00_create_schema.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        // Menyiapkan variabel lokal `schemaContent` untuk nilai schema content dengan memanggil `File.ReadAllText` dengan `schemaPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var schemaContent = File.ReadAllText(schemaPath);
        // Menyiapkan variabel lokal `cardPositionValidator` untuk nilai kartu position validator dengan memanggil `ExtractFunction` dengan `schemaContent`,
        // `”enforce_session_card_position_catalog”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var cardPositionValidator = ExtractFunction(schemaContent, "enforce_session_card_position_catalog");
        // Menyiapkan variabel lokal `scopeValidator` untuk nilai cakupan validator dengan memanggil `ExtractFunction` dengan `schemaContent`,
        // `”enforce_event_session_scope”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var scopeValidator = ExtractFunction(schemaContent, "enforce_event_session_scope");
        // Menyiapkan variabel lokal `projector` untuk nilai projector dengan memanggil `ExtractFunction` dengan `schemaContent`, `”project_session_event”`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projector = ExtractFunction(schemaContent, "project_session_event");

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”v_item_type <> 'INGREDIENT'”`,
        // `cardPositionValidator`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseLogicalIngredientDeckAndAtomicInsuranceClaims.
        Assert.Contains("v_item_type <> 'INGREDIENT'", cardPositionValidator, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”new.copy_number > coalesce(v_card_qty,
        // 0)”`, `cardPositionValidator`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseLogicalIngredientDeckAndAtomicInsuranceClaims.
        Assert.Contains("new.copy_number > coalesce(v_card_qty, 0)", cardPositionValidator, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”and zone in ('DECK', 'DISCARD')”`,
        // `scopeValidator`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseLogicalIngredientDeckAndAtomicInsuranceClaims.
        Assert.Contains("and zone in ('DECK', 'DISCARD')", scopeValidator, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”and zone in ('DECK', 'DISCARD',
        // 'MARKET')”`, `scopeValidator`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseLogicalIngredientDeckAndAtomicInsuranceClaims.
        Assert.DoesNotContain("and zone in ('DECK', 'DISCARD', 'MARKET')", scopeValidator, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”candidate.remaining_uses > 0”`,
        // `projector`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseLogicalIngredientDeckAndAtomicInsuranceClaims.
        Assert.Contains("candidate.remaining_uses > 0", projector, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”v_updated_insurances <> 1”`,
        // `projector`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseLogicalIngredientDeckAndAtomicInsuranceClaims.
        Assert.Contains("v_updated_insurances <> 1", projector, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'INSURANCE_OFFSET'”`, `projector`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseLogicalIngredientDeckAndAtomicInsuranceClaims.
        Assert.Contains("'INSURANCE_OFFSET'", projector, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”USE_INSURANCE”`,
        // `scopeValidator`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldUseLogicalIngredientDeckAndAtomicInsuranceClaims.
        Assert.DoesNotContain("USE_INSURANCE", scopeValidator, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode CanonicalSchema_ShouldUseLogicalIngredientDeckAndAtomicInsuranceClaims; bagian berikut berada di luar batas blok tersebut
    // dalam CanonicalSchema_ShouldUseLogicalIngredientDeckAndAtomicInsuranceClaims.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules` dengan hasil bertipe `void`; operasi ini
    // menangani canonical schema should lock rulebook weekday risiko pinjaman asuransi dan tabungan rules.
    public void CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules()
    // Membuka scope metode CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules; pernyataan/deklarasi berikut berada di dalam
    // batas blok ini dalam CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules.
    {
        // Menyiapkan variabel lokal `schemaPath` untuk nilai schema path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”00_create_schema.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        // Menyiapkan variabel lokal `seedPath` untuk nilai seed path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”01_seed_default_rulesets_components.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seedPath = Path.Combine(RepoRoot, "database", "01_seed_default_rulesets_components.sql");
        // Menyiapkan variabel lokal `schemaContent` untuk nilai schema content dengan memanggil `File.ReadAllText` dengan `schemaPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var schemaContent = File.ReadAllText(schemaPath);
        // Menyiapkan variabel lokal `seedContent` untuk nilai seed content dengan memanggil `File.ReadAllText` dengan `seedPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var seedContent = File.ReadAllText(seedPath);
        // Menyiapkan variabel lokal `scopeValidatorBody` untuk nilai cakupan validator body dengan memanggil `ExtractFunction` dengan `schemaContent`,
        // `”enforce_event_session_scope”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var scopeValidatorBody = ExtractFunction(schemaContent, "enforce_event_session_scope");
        // Menyiapkan variabel lokal `projectorBody` untuk nilai projector body dengan memanggil `ExtractFunction` dengan `schemaContent`,
        // `”project_session_event”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var projectorBody = ExtractFunction(schemaContent, "project_session_event");

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Friday player actions are limited to
        // donation”`, `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules.
        Assert.Contains("Friday player actions are limited to donation", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Saturday player actions are limited to
        // gold trades”`, `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules.
        Assert.Contains("Saturday player actions are limited to gold trades", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”All players must finish exactly”`,
        // `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules.
        Assert.Contains("All players must finish exactly", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”All players must submit one Friday
        // donation”`, `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules.
        Assert.Contains("All players must submit one Friday donation", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Saturday requires one gold decision per
        // player”`, `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules.
        Assert.Contains("Saturday requires one gold decision per player", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Physical gold card supply exceeded”`,
        // `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules.
        Assert.Contains("Physical gold card supply exceeded", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Physical financial goal card”`,
        // `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules.
        Assert.Contains("Physical financial goal card", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”v_new_risk_effect_type”`,
        // `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules.
        Assert.DoesNotContain("v_new_risk_effect_type", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”v_new_risk_direction”`,
        // `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules.
        Assert.DoesNotContain("v_new_risk_direction", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”RisikoKehidupan in MAHIR mode must
        // immediately follow JualMasakan for the same player”`, `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules.
        Assert.Contains("RisikoKehidupan in MAHIR mode must immediately follow JualMasakan for the same player", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Emergency loan principal must match
        // catalog principal”`, `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules.
        Assert.Contains("Emergency loan principal must match catalog principal", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Insurance premium must match catalog
        // premium”`, `scopeValidatorBody`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules.
        Assert.Contains("Insurance premium must match catalog premium", scopeValidatorBody, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'TarikTabungan'”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules.
        Assert.DoesNotContain("'TarikTabungan'", seedContent, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules; bagian berikut berada di luar batas blok tersebut
    // dalam CanonicalSchema_ShouldLockRulebookWeekdayRiskLoanInsuranceAndSavingsRules.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetRepository_ShouldPreservePhysicalCountsMissionFamiliesAndLifeRiskRuntimeFields` dengan hasil bertipe `void`;
    // operasi ini menangani aturan repositori should preserve physical counts misi families dan life risiko runtime fields.
    public void RulesetRepository_ShouldPreservePhysicalCountsMissionFamiliesAndLifeRiskRuntimeFields()
    // Membuka scope metode RulesetRepository_ShouldPreservePhysicalCountsMissionFamiliesAndLifeRiskRuntimeFields; pernyataan/deklarasi berikut berada
    // di dalam batas blok ini dalam RulesetRepository_ShouldPreservePhysicalCountsMissionFamiliesAndLifeRiskRuntimeFields.
    {
        // Menyiapkan variabel lokal `repository` untuk nilai repositori dengan memanggil `File.ReadAllText` dengan `Path.Combine( RepoRoot, ”src”,
        // ”Cashflowpoly.Api”, ”Data”, ”RulesetRepository.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var repository = File.ReadAllText(Path.Combine(
            // Meneruskan `RepoRoot` (nilai repo root) sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”src”` sebagai argumen ke `Path.Combine`;
            // Meneruskan nilai literal `”Cashflowpoly.Api”` sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”Data”` sebagai argumen ke
            // `Path.Combine`; Meneruskan nilai literal `”RulesetRepository.cs”` sebagai argumen ke `Path.Combine`.
            RepoRoot, "src", "Cashflowpoly.Api", "Data", "RulesetRepository.cs"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”card_qty as CardQty”`, `repository`,
        // `StringComparison.OrdinalIgnoreCase` dalam RulesetRepository_ShouldPreservePhysicalCountsMissionFamiliesAndLifeRiskRuntimeFields.
        Assert.Contains("card_qty as CardQty", repository, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”need_family_code as Family”`,
        // `repository`, `StringComparison.OrdinalIgnoreCase` dalam RulesetRepository_ShouldPreservePhysicalCountsMissionFamiliesAndLifeRiskRuntimeFields.
        Assert.Contains("need_family_code as Family", repository, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”required_need_family_code”`,
        // `repository`, `StringComparison.OrdinalIgnoreCase` dalam RulesetRepository_ShouldPreservePhysicalCountsMissionFamiliesAndLifeRiskRuntimeFields.
        Assert.Contains("required_need_family_code", repository, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”target_scope as TargetScope”`,
        // `repository`, `StringComparison.OrdinalIgnoreCase` dalam RulesetRepository_ShouldPreservePhysicalCountsMissionFamiliesAndLifeRiskRuntimeFields.
        Assert.Contains("target_scope as TargetScope", repository, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'value_delta', '')::int as
        // ValueDelta”`, `repository`, `StringComparison.OrdinalIgnoreCase` dalam
        // RulesetRepository_ShouldPreservePhysicalCountsMissionFamiliesAndLifeRiskRuntimeFields.
        Assert.Contains("'value_delta', '')::int as ValueDelta", repository, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”jsonb_build_object('value_delta',
        // @ValueDelta)”`, `repository`, `StringComparison.OrdinalIgnoreCase` dalam
        // RulesetRepository_ShouldPreservePhysicalCountsMissionFamiliesAndLifeRiskRuntimeFields.
        Assert.Contains("jsonb_build_object('value_delta', @ValueDelta)", repository, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”@DurationDays”`, `repository`,
        // `StringComparison.OrdinalIgnoreCase` dalam RulesetRepository_ShouldPreservePhysicalCountsMissionFamiliesAndLifeRiskRuntimeFields.
        Assert.Contains("@DurationDays", repository, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”@TargetScope”`, `repository`,
        // `StringComparison.OrdinalIgnoreCase` dalam RulesetRepository_ShouldPreservePhysicalCountsMissionFamiliesAndLifeRiskRuntimeFields.
        Assert.Contains("@TargetScope", repository, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”duration_days as DurationDays”`,
        // `repository`, `StringComparison.OrdinalIgnoreCase` dalam RulesetRepository_ShouldPreservePhysicalCountsMissionFamiliesAndLifeRiskRuntimeFields.
        Assert.Contains("duration_days as DurationDays", repository, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode RulesetRepository_ShouldPreservePhysicalCountsMissionFamiliesAndLifeRiskRuntimeFields; bagian berikut berada di luar batas
    // blok tersebut dalam RulesetRepository_ShouldPreservePhysicalCountsMissionFamiliesAndLifeRiskRuntimeFields.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CanonicalSchema_ShouldTrackSchemaBaselineFingerprint` dengan hasil bertipe `void`; operasi ini menangani canonical schema
    // should track schema baseline fingerprint.
    public void CanonicalSchema_ShouldTrackSchemaBaselineFingerprint()
    // Membuka scope metode CanonicalSchema_ShouldTrackSchemaBaselineFingerprint; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
    {
        // Menyiapkan variabel lokal `schemaPath` untuk nilai schema path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”00_create_schema.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        // Menyiapkan variabel lokal `schemaContent` untuk nilai schema content dengan memanggil `File.ReadAllText` dengan `schemaPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var schemaContent = File.ReadAllText(schemaPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // schema_baseline_versions”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.Contains("create table if not exists schema_baseline_versions", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”baseline_name varchar”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.Contains("baseline_name varchar", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”schema_version varchar”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.Contains("schema_version varchar", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”checksum varchar”`, `schemaContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.Contains("checksum varchar", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”compute_schema_fingerprint”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.Contains("compute_schema_fingerprint", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”assert_schema_baseline”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.Contains("assert_schema_baseline", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”assert_schema_baseline('canonical_relational_baseline', '3.0.13')”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.Contains("assert_schema_baseline('canonical_relational_baseline', '3.0.13')", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”information_schema.columns”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.Contains("information_schema.columns", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”pg_constraint”`, `schemaContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.Contains("pg_constraint", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”pg_indexes”`, `schemaContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.Contains("pg_indexes", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”pg_trigger”`, `schemaContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.Contains("pg_trigger", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”pg_proc”`, `schemaContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.Contains("pg_proc", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”pg_get_functiondef(p.oid)”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.Contains("pg_get_functiondef(p.oid)", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”p.prokind in ('f', 'p')”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.Contains("p.prokind in ('f', 'p')", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”d.deptype = 'e'”`, `schemaContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.Contains("d.deptype = 'e'", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”pg_views”`, `schemaContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.Contains("pg_views", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”E '\\n'”`, `schemaContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
        Assert.DoesNotContain("E '\\n'", schemaContent, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode CanonicalSchema_ShouldTrackSchemaBaselineFingerprint; bagian berikut berada di luar batas blok tersebut dalam
    // CanonicalSchema_ShouldTrackSchemaBaselineFingerprint.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints` dengan hasil bertipe `void`; operasi ini
    // menangani canonical schema should separate misi requirements dan use unified rank poin.
    public void CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints()
    // Membuka scope metode CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints.
    {
        // Menyiapkan variabel lokal `schemaPath` untuk nilai schema path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”00_create_schema.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        // Menyiapkan variabel lokal `schemaContent` untuk nilai schema content dengan memanggil `File.ReadAllText` dengan `schemaPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var schemaContent = File.ReadAllText(schemaPath);
        // Menyiapkan variabel lokal `seedPath` untuk nilai seed path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”01_seed_default_rulesets_components.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seedPath = Path.Combine(RepoRoot, "database", "01_seed_default_rulesets_components.sql");
        // Menyiapkan variabel lokal `seedContent` untuk nilai seed content dengan memanggil `File.ReadAllText` dengan `seedPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var seedContent = File.ReadAllText(seedPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // ruleset_rank_points”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints.
        Assert.Contains("create table if not exists ruleset_rank_points", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rank_type varchar(20) not null”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints.
        Assert.Contains("rank_type varchar(20) not null", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rank_type in ('DONATION', 'PENSION')”`,
        // `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints.
        Assert.Contains("rank_type in ('DONATION', 'PENSION')", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // ruleset_donation_rank_points”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints.
        Assert.DoesNotContain("create table if not exists ruleset_donation_rank_points", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”create table if not exists
        // ruleset_pension_rank_points”`, `schemaContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints.
        Assert.DoesNotContain("create table if not exists ruleset_pension_rank_points", schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan memanggil `AssertSqlContains` dengan `”create or replace view ruleset_catalog_item_requirements”`, `schemaContent` dalam
        // CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints.
        AssertSqlContains("create or replace view ruleset_catalog_item_requirements", schemaContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”create or replace view ruleset_collection_mission_requirement_items”`, `schemaContent` dalam
        // CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints.
        AssertSqlContains("create or replace view ruleset_collection_mission_requirement_items", schemaContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”rcmr.ruleset_collection_mission_id”`, `ExtractView(schemaContent, ”ruleset_catalog_item_requirements”)`, `StringComparison.OrdinalIgnoreCase`
        // dalam CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints.
        Assert.DoesNotContain("rcmr.ruleset_collection_mission_id", ExtractView(schemaContent, "ruleset_catalog_item_requirements"), StringComparison.OrdinalIgnoreCase);
        // Menjalankan memanggil `AssertSqlContains` dengan `”insert into ruleset_rank_points”`, `seedContent` dalam
        // CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints.
        AssertSqlContains("insert into ruleset_rank_points", seedContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into
        // ruleset_donation_rank_points”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints.
        Assert.DoesNotContain("insert into ruleset_donation_rank_points", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into
        // ruleset_pension_rank_points”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints.
        Assert.DoesNotContain("insert into ruleset_pension_rank_points", seedContent, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints; bagian berikut berada di luar batas blok tersebut
    // dalam CanonicalSchema_ShouldSeparateMissionRequirementsAndUseUnifiedRankPoints.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts` dengan hasil bertipe `void`; operasi ini
    // menangani canonical schema should not contain removed catalog pemain atau pemain urutan/pesanan artifacts.
    public void CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts()
    // Membuka scope metode CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts; pernyataan/deklarasi berikut berada di dalam
    // batas blok ini dalam CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
    {
        // Menyiapkan variabel lokal `schemaPath` untuk nilai schema path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”00_create_schema.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        // Menyiapkan variabel lokal `schemaContent` untuk nilai schema content dengan memanggil `File.ReadAllText` dengan `schemaPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var schemaContent = File.ReadAllText(schemaPath);
        // Menyiapkan variabel lokal `appDbContextPath` untuk nilai app basis data context path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`,
        // `”Cashflowpoly.Api”`, `”Data”`, `”AppDbContext.cs”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var appDbContextPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Data", "AppDbContext.cs");
        // Menyiapkan variabel lokal `appDbContextContent` untuk nilai app basis data context content dengan memanggil `File.ReadAllText` dengan
        // `appDbContextPath`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var appDbContextContent = File.ReadAllText(appDbContextPath);

        // Menyiapkan variabel lokal `forbidden` untuk nilai forbidden dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var forbidden = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
        {
            // Menggunakan nilai literal `”create table if not exists ruleset_catalog_items”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists ruleset_catalog_items",
            // Menggunakan nilai literal `”create table if not exists ruleset_catalog_item_requirements”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists ruleset_catalog_item_requirements",
            // Menggunakan nilai literal `”create table if not exists app_menus”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists app_menus",
            // Menggunakan nilai literal `”create table if not exists role_menu_permissions”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists role_menu_permissions",
            // Melanjutkan pengolahan dengan menggabungkan `string` dengan `”create table if not exists ruleset”`, `”_player”`, `”_ordering”`, `”_instructor”`,
            // `”_users”` pada urutan hasil dalam CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            string.Concat("create table if not exists ruleset", "_player", "_ordering", "_instructor", "_users"),
            // Menggunakan nilai literal `”create table if not exists ingredients”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists ingredients",
            // Menggunakan nilai literal `”create table if not exists game_components”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists game_components",
            // Menggunakan nilai literal `”create table if not exists session_players”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists session_players",
            // Menggunakan nilai literal `”create table if not exists session_player_assets”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists session_player_assets",
            // Menggunakan nilai literal `”create table if not exists session_participant_assets”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists session_participant_assets",
            // Menggunakan nilai literal `”create table if not exists session_action_logs”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists session_action_logs",
            // Menggunakan nilai literal `”create table if not exists session_ruleset_activations”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists session_ruleset_activations",
            // Menggunakan nilai literal `”create table if not exists ruleset_quests”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists ruleset_quests",
            // Menggunakan nilai literal `”create table if not exists session_participant_quest_progress”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists session_participant_quest_progress",
            // Menggunakan nilai literal `”create table if not exists session_participant_narrative_logs”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists session_participant_narrative_logs",
            // Menggunakan nilai literal `”create table if not exists interpreter_commands”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists interpreter_commands",
            // Menggunakan nilai literal `”create table if not exists quest_scripts”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists quest_scripts",
            // Menggunakan nilai literal `”create table if not exists narrative_assets”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists narrative_assets",
            // Menggunakan nilai literal `”create table if not exists narrative_scripts”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists narrative_scripts",
            // Menggunakan nilai literal `”create table if not exists session_donation_event_rankings”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists session_donation_event_rankings",
            // Menggunakan nilai literal `”create table if not exists session_pension_rankings”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists session_pension_rankings",
            // Menggunakan nilai literal `”create table if not exists event_inventory_effects”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists event_inventory_effects",
            // Menggunakan nilai literal `”create table if not exists event_need_effects”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists event_need_effects",
            // Menggunakan nilai literal `”create table if not exists event_goal_effects”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists event_goal_effects",
            // Menggunakan nilai literal `”create table if not exists event_asset_effects”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists event_asset_effects",
            // Menggunakan nilai literal `”create table if not exists event_score_effects”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists event_score_effects",
            // Menggunakan nilai literal `”create table if not exists event_turn_effects”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "create table if not exists event_turn_effects",
            // Menggunakan nilai literal `”config_json jsonb”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "config_json jsonb",
            // Menggunakan nilai literal `”ix_ruleset_versions_config_gin”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "ix_ruleset_versions_config_gin",
            // Menggunakan nilai literal `”refresh_ruleset_version_config_json”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "refresh_ruleset_version_config_json",
            // Menggunakan nilai literal `”sync_ruleset_version_relational_content”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "sync_ruleset_version_relational_content",
            // Menggunakan nilai literal `”trg_ruleset_versions_sync_relational_content”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "trg_ruleset_versions_sync_relational_content",
            // Menggunakan nilai literal `”rankings_cache_json”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "rankings_cache_json",
            // Menggunakan nilai literal `”ruleset_game_asset_id uuid not null,\r\n narrative_code”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "ruleset_game_asset_id uuid not null,\r\n  narrative_code",
            // Menggunakan nilai literal `”ruleset_quest_id”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "ruleset_quest_id",
            // Menggunakan nilai literal `”card_ref_id varchar”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "card_ref_id varchar",
            // Menggunakan nilai literal `”ingredient_value varchar”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "ingredient_value varchar",
            // Menggunakan nilai literal `”requirement_value varchar”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "requirement_value varchar",
            // Menggunakan nilai literal `”event_action_type varchar”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "event_action_type varchar",
            // Menggunakan nilai literal `”action_id varchar(80) null”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "action_id varchar(80) null",
            // Menggunakan nilai literal `”details_json jsonb not null default '{}'::jsonb”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "details_json jsonb not null default '{}'::jsonb",
            // Menggunakan nilai literal `”source_json jsonb not null default '{}'::jsonb”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "source_json jsonb not null default '{}'::jsonb",
            // Menggunakan nilai literal `”player_index”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "player_index",
            // Menggunakan nilai literal `”rankings_json jsonb”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "rankings_json jsonb",
            // Menggunakan nilai literal `”session_participants_role”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "session_participants_role"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
        };

        // Mengulangi setiap elemen `forbidden`; elemen saat ini disimpan sebagai `entry` bertipe `var` untuk diproses oleh badan loop dalam
        // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
        foreach (var entry in forbidden)
        // Membuka scope loop setiap entry dari `forbidden`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
        {
            // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `entry`, `schemaContent`,
            // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            Assert.DoesNotContain(entry, schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menutup scope loop setiap entry dari `forbidden`; bagian berikut berada di luar batas blok tersebut dalam
        // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
        }

        // Menyiapkan variabel lokal `forbiddenEfMappings` untuk nilai forbidden ef mappings dengan array baru dengan tipe elemen disimpulkan dari nilai
        // initializer. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var forbiddenEfMappings = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
        {
            // Menggunakan nilai literal `”ConfigJson”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "ConfigJson",
            // Menggunakan nilai literal `”session_players”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "session_players",
            // Menggunakan nilai literal `”session_player_assets”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "session_player_assets",
            // Menggunakan nilai literal `”session_participant_assets”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "session_participant_assets",
            // Menggunakan nilai literal `”SessionParticipantAsset”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "SessionParticipantAsset",
            // Menggunakan nilai literal `”session_ruleset_activations”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "session_ruleset_activations",
            // Menggunakan nilai literal `”ruleset_quests”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "ruleset_quests",
            // Menggunakan nilai literal `”session_participant_quest_progress”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "session_participant_quest_progress",
            // Menggunakan nilai literal `”interpreter_commands”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "interpreter_commands",
            // Menggunakan nilai literal `”narrative_assets”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "narrative_assets",
            // Menggunakan nilai literal `”quest_scripts”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "quest_scripts",
            // Menggunakan nilai literal `”narrative_scripts”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "narrative_scripts",
            // Menggunakan nilai literal `”session_donation_event_rankings”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "session_donation_event_rankings",
            // Menggunakan nilai literal `”session_pension_rankings”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "session_pension_rankings",
            // Menggunakan nilai literal `”session_player_states”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "session_player_states",
            // Menggunakan nilai literal `”session_player_ingredients”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "session_player_ingredients",
            // Menggunakan nilai literal `”session_player_needs”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "session_player_needs",
            // Menggunakan nilai literal `”session_player_financial_goals”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "session_player_financial_goals",
            // Menggunakan nilai literal `”session_player_collection_missions”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "session_player_collection_missions",
            // Menggunakan nilai literal `”session_player_quest_progress”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "session_player_quest_progress",
            // Menggunakan nilai literal `”session_player_action_counters”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            "session_player_action_counters"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
        };

        // Mengulangi setiap elemen `forbiddenEfMappings`; elemen saat ini disimpan sebagai `entry` bertipe `var` untuk diproses oleh badan loop dalam
        // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
        foreach (var entry in forbiddenEfMappings)
        // Membuka scope loop setiap entry dari `forbiddenEfMappings`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
        {
            // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `entry`, `appDbContextContent`,
            // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
            Assert.DoesNotContain(entry, appDbContextContent, StringComparison.OrdinalIgnoreCase);
        // Menutup scope loop setiap entry dari `forbiddenEfMappings`; bagian berikut berada di luar batas blok tersebut dalam
        // CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
        }
    // Menutup scope metode CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts; bagian berikut berada di luar batas blok
    // tersebut dalam CanonicalSchema_ShouldNotContainRemovedCatalogPlayerOrPlayerOrderArtifacts.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes` dengan hasil bertipe `void`; operasi ini menangani canonical
    // schema should not declare known exact duplicate indexes.
    public void CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes()
    // Membuka scope metode CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
    {
        // Menyiapkan variabel lokal `schemaPath` untuk nilai schema path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”00_create_schema.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var schemaPath = Path.Combine(RepoRoot, "database", "00_create_schema.sql");
        // Menyiapkan variabel lokal `schemaContent` untuk nilai schema content dengan memanggil `File.ReadAllText` dengan `schemaPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var schemaContent = File.ReadAllText(schemaPath);

        // Menyiapkan variabel lokal `duplicateIndexes` untuk nilai duplicate indexes dengan array baru dengan tipe elemen disimpulkan dari nilai
        // initializer. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var duplicateIndexes = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
        {
            // Menggunakan nilai literal `”ix_ruleset_versions_ruleset”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
            "ix_ruleset_versions_ruleset",
            // Menggunakan nilai literal `”ix_ruleset_player_ordering_rules_ruleset”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
            "ix_ruleset_player_ordering_rules_ruleset",
            // Melanjutkan pengolahan dengan menggabungkan `string` dengan `”ix_ruleset”`, `”_player”`, `”_ordering”`, `”_instructor”`, `”_users”`, `”_ruleset”`
            // pada urutan hasil dalam CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
            string.Concat("ix_ruleset", "_player", "_ordering", "_instructor", "_users", "_ruleset"),
            // Menggunakan nilai literal `”ix_session_participants_session”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
            "ix_session_participants_session",
            // Menggunakan nilai literal `”ix_session_donation_events_session”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
            "ix_session_donation_events_session",
            // Menggunakan nilai literal `”ix_session_donation_event_rankings_event”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
            "ix_session_donation_event_rankings_event",
            // Menggunakan nilai literal `”ix_events_session_seq”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
            "ix_events_session_seq",
            // Menggunakan nilai literal `”create index if not exists ix_ruleset_collection_mission_requirements_mission”` sebagai bagian ekspresi yang sedang
            // disusun dalam CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
            "create index if not exists ix_ruleset_collection_mission_requirements_mission",
            // Menggunakan nilai literal `”create index if not exists ix_ruleset_narrative_scenes_narrative”` sebagai bagian ekspresi yang sedang disusun dalam
            // CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
            "create index if not exists ix_ruleset_narrative_scenes_narrative"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
        };

        // Mengulangi setiap elemen `duplicateIndexes`; elemen saat ini disimpan sebagai `indexName` bertipe `var` untuk diproses oleh badan loop dalam
        // CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
        foreach (var indexName in duplicateIndexes)
        // Membuka scope loop setiap indexName dari `duplicateIndexes`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
        {
            // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `indexName`, `schemaContent`,
            // `StringComparison.OrdinalIgnoreCase` dalam CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
            Assert.DoesNotContain(indexName, schemaContent, StringComparison.OrdinalIgnoreCase);
        // Menutup scope loop setiap indexName dari `duplicateIndexes`; bagian berikut berada di luar batas blok tersebut dalam
        // CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
        }
    // Menutup scope metode CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes; bagian berikut berada di luar batas blok tersebut dalam
    // CanonicalSchema_ShouldNotDeclareKnownExactDuplicateIndexes.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes` dengan hasil bertipe `void`; operasi ini
    // menangani bawaan aturan seed should populate typed relational aturan data untuk both modes.
    public void DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes()
    // Membuka scope metode DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
    {
        // Menyiapkan variabel lokal `seedPath` untuk nilai seed path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”01_seed_default_rulesets_components.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seedPath = Path.Combine(RepoRoot, "database", "01_seed_default_rulesets_components.sql");
        // Menyiapkan variabel lokal `seedContent` untuk nilai seed content dengan memanggil `File.ReadAllText` dengan `seedPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var seedContent = File.ReadAllText(seedPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”created_by_user_id”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains("created_by_user_id", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”$ json $”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("$ json $", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into app_menus”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("insert into app_menus", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into
        // role_menu_permissions”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("insert into role_menu_permissions", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `string.Concat(”ruleset”,
        // ”_player”, ”_ordering”, ”_instructor”, ”_users”)`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain(
            // Meneruskan menggabungkan `string` dengan `”ruleset”`, `”_player”`, `”_ordering”`, `”_instructor”`, `”_users”` pada urutan hasil sebagai argumen
            // ke `Assert.DoesNotContain`; Meneruskan nilai literal `”ruleset”` sebagai argumen ke `string.Concat`; Meneruskan nilai literal `”_player”` sebagai
            // argumen ke `string.Concat`; Meneruskan nilai literal `”_ordering”` sebagai argumen ke `string.Concat`; Meneruskan nilai literal `”_instructor”`
            // sebagai argumen ke `string.Concat`; Meneruskan nilai literal `”_users”` sebagai argumen ke `string.Concat`.
            string.Concat("ruleset", "_player", "_ordering", "_instructor", "_users"),
            // Meneruskan `seedContent` (nilai seed content) sebagai argumen ke `Assert.DoesNotContain`.
            seedContent,
            // Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `Assert.DoesNotContain`.
            StringComparison.OrdinalIgnoreCase);
        // Menjalankan memanggil `AssertSqlContains` dengan `”insert into ruleset_game_assets”`, `seedContent` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        AssertSqlContains("insert into ruleset_game_assets", seedContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”insert into ruleset_game_settings”`, `seedContent` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        AssertSqlContains("insert into ruleset_game_settings", seedContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”insert into ruleset_orders”`, `seedContent` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        AssertSqlContains("insert into ruleset_orders", seedContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”insert into ruleset_collection_mission_requirements”`, `seedContent` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        AssertSqlContains("insert into ruleset_collection_mission_requirements", seedContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”insert into ruleset_need_set_bonuses”`, `seedContent` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        AssertSqlContains("insert into ruleset_need_set_bonuses", seedContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'THREE_DIFFERENT'”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains("'THREE_DIFFERENT'", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'THREE_SAME'”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains("'THREE_SAME'", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan memanggil `AssertSqlContains` dengan `”insert into ruleset_gold_prices”`, `seedContent` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        AssertSqlContains("insert into ruleset_gold_prices", seedContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”insert into ruleset_tie_breakers”`, `seedContent` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        AssertSqlContains("insert into ruleset_tie_breakers", seedContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”insert into ruleset_sharia_loans”`, `seedContent` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        AssertSqlContains("insert into ruleset_sharia_loans", seedContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”insert into ruleset_insurance_products”`, `seedContent` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        AssertSqlContains("insert into ruleset_insurance_products", seedContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”insert into ruleset_life_risks”`, `seedContent` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        AssertSqlContains("insert into ruleset_life_risks", seedContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”insert into ruleset_narratives”`, `seedContent` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        AssertSqlContains("insert into ruleset_narratives", seedContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”insert into ruleset_narrative_scenes”`, `seedContent` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        AssertSqlContains("insert into ruleset_narrative_scenes", seedContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”delete from ruleset_actions legacy”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains("delete from ruleset_actions legacy", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”delete from actions legacy”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains("delete from actions legacy", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”BagikanEmasAwal”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains("BagikanEmasAwal", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”BagikanMisiKoleksi”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains("BagikanMisiKoleksi", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan hasil dengan `Assert.Single` menggunakan `Regex.Matches(seedContent, ”BagikanEmasAwal”, RegexOptions.IgnoreCase)`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Single(Regex.Matches(seedContent, "BagikanEmasAwal", RegexOptions.IgnoreCase));
        // Menjalankan pemeriksaan hasil dengan `Assert.Single` menggunakan `Regex.Matches(seedContent, ”BagikanMisiKoleksi”, RegexOptions.IgnoreCase)`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Single(Regex.Matches(seedContent, "BagikanMisiKoleksi", RegexOptions.IgnoreCase));
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”KartuDiambilDariPasar”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains("KartuDiambilDariPasar", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan hasil dengan `Assert.Single` menggunakan `Regex.Matches(seedContent, ”KartuDiambilDariPasar”, RegexOptions.IgnoreCase)`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Single(Regex.Matches(seedContent, "KartuDiambilDariPasar", RegexOptions.IgnoreCase));
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”AmbilKartuDariDeck”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("AmbilKartuDariDeck", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”KartuMasukDiscard”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("KartuMasukDiscard", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”IsiUlangPasar”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("IsiUlangPasar", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”LewatiOrder”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("LewatiOrder", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”gold.initial.granted”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("gold.initial.granted", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”card.drawn”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("card.drawn", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”card.taken”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("card.taken", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”card.discarded”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("card.discarded", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”market.refilled”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("market.refilled", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan memanggil `AssertSqlContains` dengan `”insert into ruleset_trigger_conditions”`, `seedContent` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        AssertSqlContains("insert into ruleset_trigger_conditions", seedContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into ruleset_quests”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("insert into ruleset_quests", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””quest”””`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain(@"""quest""", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”asset_type = 'NARRATIVE'”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("asset_type = 'NARRATIVE'", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”asset_type = 'QUEST'”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("asset_type = 'QUEST'", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into
        // interpreter_commands”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("insert into interpreter_commands", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into quest_scripts”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("insert into quest_scripts", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into narrative_scripts”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("insert into narrative_scripts", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”refresh_ruleset_version_config_json”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("refresh_ruleset_version_config_json", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”sync_ruleset_version_relational_content”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain("sync_ruleset_version_relational_content", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””starting_cash””: 20”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains(@"""starting_cash"": 20", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””starting_cash””: 10”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains(@"""starting_cash"": 10", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””actions_per_turn””: 2”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains(@"""actions_per_turn"": 2", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””finishDay””: 25”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains(@"""finishDay"": 25", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””finishDay””: 13”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain(@"""finishDay"": 13", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”min_players,”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains("min_players,", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”gado_gado”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains("gado_gado", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””nama””: ””jam”””`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains(@"""nama"": ""jam""", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”gold_price_4”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains("gold_price_4", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”loan_syariah_10”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains("loan_syariah_10", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””card_supply””:8”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains(@"""card_supply"":8", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”risk_bencana_banjir”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains("risk_bencana_banjir", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””premium””:1”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.Contains(@"""premium"":1", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””scripts””: [”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
        Assert.DoesNotContain(@"""scripts"": [", seedContent, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes; bagian berikut berada di luar batas blok tersebut
    // dalam DefaultRulesetSeed_ShouldPopulateTypedRelationalRulesetDataForBothModes.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules` dengan hasil bertipe `void`; operasi ini
    // menangani bawaan aturan seed should match akhir risiko misi asuransi dan kebutuhan limit rules.
    public void DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules()
    // Membuka scope metode DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
    {
        // Menyiapkan variabel lokal `seedPath` untuk nilai seed path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”01_seed_default_rulesets_components.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seedPath = Path.Combine(RepoRoot, "database", "01_seed_default_rulesets_components.sql");
        // Menyiapkan variabel lokal `seedContent` untuk nilai seed content dengan memanggil `File.ReadAllText` dengan `seedPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var seedContent = File.ReadAllText(seedPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””primary_need_max_per_day””: null”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains(@"""primary_need_max_per_day"": null", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””primary_need_max_per_day””:
        // 1”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.DoesNotContain(@"""primary_need_max_per_day"": 1", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan
        // `@”'risk_beli_peralatan_dapur'[\s\S]*?'COIN_EFFECT'[\s\S]*?'OUT'[\s\S]*?3\s*,”`, `seedContent`; ketidaksesuaian dengan ekspektasi membuat
        // pengujian gagal dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Matches(@"'risk_beli_peralatan_dapur'[\s\S]*?'COIN_EFFECT'[\s\S]*?'OUT'[\s\S]*?3\s*,", seedContent);
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan `@”'risk_sakit_gigi'[\s\S]*?'COIN_EFFECT'[\s\S]*?'OUT'[\s\S]*?4\s*,”`,
        // `seedContent`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Matches(@"'risk_sakit_gigi'[\s\S]*?'COIN_EFFECT'[\s\S]*?'OUT'[\s\S]*?4\s*,", seedContent);
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan `@”'risk_operasi_usus_buntu'[\s\S]*?'COIN_EFFECT'[\s\S]*?'OUT'[\s\S]*?6\s*,”`,
        // `seedContent`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Matches(@"'risk_operasi_usus_buntu'[\s\S]*?'COIN_EFFECT'[\s\S]*?'OUT'[\s\S]*?6\s*,", seedContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'risk_pemadaman_listrik',”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains("'risk_pemadaman_listrik',", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'ALL_PLAYERS_COIN_EFFECT',”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains("'ALL_PLAYERS_COIN_EFFECT',", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””target_scope””:””ALL_PLAYERS”””`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains(@"""target_scope"":""ALL_PLAYERS""", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan `@”'risk_pemadaman_listrik'[\s\S]*?1\s*,\s*'ALL_PLAYERS'\s*,”`, `seedContent`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Matches(@"'risk_pemadaman_listrik'[\s\S]*?1\s*,\s*'ALL_PLAYERS'\s*,", seedContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'risk_bbm_naik',”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains("'risk_bbm_naik',", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'INGREDIENT_PRICE_MODIFIER',”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains("'INGREDIENT_PRICE_MODIFIER',", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””value_delta””:1”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains(@"""value_delta"":1", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan `@”'risk_bbm_naik'[\s\S]*?7\s*,\s*'ALL_PLAYERS'\s*,”`, `seedContent`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Matches(@"'risk_bbm_naik'[\s\S]*?7\s*,\s*'ALL_PLAYERS'\s*,", seedContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'risk_panen_melimpah',”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains("'risk_panen_melimpah',", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””value_delta””:-1”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains(@"""value_delta"":-1", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan `@”'risk_panen_melimpah'[\s\S]*?7\s*,\s*'ALL_PLAYERS'\s*,”`, `seedContent`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Matches(@"'risk_panen_melimpah'[\s\S]*?7\s*,\s*'ALL_PLAYERS'\s*,", seedContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'risk_investasi_emas',”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains("'risk_investasi_emas',", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'GOLD_TRADE',”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains("'GOLD_TRADE',", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'risk_ulang_tahun',”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains("'risk_ulang_tahun',", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'PLAYER_TO_PLAYER_TRANSFER',”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains("'PLAYER_TO_PLAYER_TRANSFER',", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””target_scope””:””OTHER_PLAYERS”””`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains(@"""target_scope"":""OTHER_PLAYERS""", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan `@”'risk_ulang_tahun'[\s\S]*?1\s*,\s*'OTHER_PLAYERS'\s*,”`, `seedContent`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Matches(@"'risk_ulang_tahun'[\s\S]*?1\s*,\s*'OTHER_PLAYERS'\s*,", seedContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””type””: ””FAMILY””, ””value””:
        // ””jam”””`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains(@"""type"": ""FAMILY"", ""value"": ""jam""", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””type””: ””FAMILY””, ””value””:
        // ””boneka”””`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains(@"""type"": ""FAMILY"", ""value"": ""boneka""", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””type””: ””FAMILY””, ””value””:
        // ””gameboy”””`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains(@"""type"": ""FAMILY"", ""value"": ""gameboy""", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””type””: ””FAMILY””, ””value””:
        // ””hiburan”””`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains(@"""type"": ""FAMILY"", ""value"": ""hiburan""", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””value””: ””jam_2”””`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.DoesNotContain(@"""value"": ""jam_2""", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””value””: ””boneka_2”””`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.DoesNotContain(@"""value"": ""boneka_2""", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””value””: ””gameboy_2”””`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.DoesNotContain(@"""value"": ""gameboy_2""", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””value””: ””hiburan_2”””`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.DoesNotContain(@"""value"": ""hiburan_2""", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”required_need_family_code”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Contains("required_need_family_code", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan `@”'multirisk_basic'[\s\S]*?0\s*,\s*'\{\””premium\””:\s*1”`, `seedContent`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
        Assert.Matches(
            // Meneruskan nilai literal `@”'multirisk_basic'[\s\S]*?0\s*,\s*'\{\””premium\””:\s*1”` sebagai argumen ke `Assert.Matches`.
            @"'multirisk_basic'[\s\S]*?0\s*,\s*'\{\""premium\"":\s*1",
            // Meneruskan `seedContent` (nilai seed content) sebagai argumen ke `Assert.Matches`.
            seedContent);
    // Menutup scope metode DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules; bagian berikut berada di luar batas blok tersebut
    // dalam DefaultRulesetSeed_ShouldMatchFinalRiskMissionInsuranceAndNeedLimitRules.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetRepository_ShouldPreserveAnUnlimitedPrimaryNeedSettingAsNull` dengan hasil bertipe `void`; operasi ini menangani
    // aturan repositori should preserve an unlimited primary kebutuhan setting as null.
    public void RulesetRepository_ShouldPreserveAnUnlimitedPrimaryNeedSettingAsNull()
    // Membuka scope metode RulesetRepository_ShouldPreserveAnUnlimitedPrimaryNeedSettingAsNull; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam RulesetRepository_ShouldPreserveAnUnlimitedPrimaryNeedSettingAsNull.
    {
        // Menyiapkan variabel lokal `repositoryPath` untuk nilai repositori path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`,
        // `”Cashflowpoly.Api”`, `”Data”`, `”RulesetRepository.cs”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var repositoryPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Data", "RulesetRepository.cs");
        // Menyiapkan variabel lokal `repository` untuk nilai repositori dengan memanggil `File.ReadAllText` dengan `repositoryPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repository = File.ReadAllText(repositoryPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”public int? PrimaryNeedMaxPerDay”`,
        // `repository`, `StringComparison.Ordinal` dalam RulesetRepository_ShouldPreserveAnUnlimitedPrimaryNeedSettingAsNull.
        Assert.Contains("public int? PrimaryNeedMaxPerDay", repository, StringComparison.Ordinal);
    // Menutup scope metode RulesetRepository_ShouldPreserveAnUnlimitedPrimaryNeedSettingAsNull; bagian berikut berada di luar batas blok tersebut dalam
    // RulesetRepository_ShouldPreserveAnUnlimitedPrimaryNeedSettingAsNull.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `DefaultRulesetSeed_ShouldIncludePemulaTieBreakerAssets` dengan hasil bertipe `void`; operasi ini menangani bawaan aturan
    // seed should include pemula tie breaker aset.
    public void DefaultRulesetSeed_ShouldIncludePemulaTieBreakerAssets()
    // Membuka scope metode DefaultRulesetSeed_ShouldIncludePemulaTieBreakerAssets; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DefaultRulesetSeed_ShouldIncludePemulaTieBreakerAssets.
    {
        // Menyiapkan variabel lokal `seedPath` untuk nilai seed path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”01_seed_default_rulesets_components.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seedPath = Path.Combine(RepoRoot, "database", "01_seed_default_rulesets_components.sql");
        // Menyiapkan variabel lokal `seedContent` untuk nilai seed content dengan memanggil `File.ReadAllText` dengan `seedPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var seedContent = File.ReadAllText(seedPath);

        // Mengulangi setiap elemen `new[] { ”tie_breaker_1”, ”tie_breaker_2”, ”tie_breaker_3”, ”tie_breaker_4” }`; elemen saat ini disimpan sebagai
        // `tieBreakerCode` bertipe `var` untuk diproses oleh badan loop dalam DefaultRulesetSeed_ShouldIncludePemulaTieBreakerAssets.
        foreach (var tieBreakerCode in new[] { "tie_breaker_1", "tie_breaker_2", "tie_breaker_3", "tie_breaker_4" })
        // Membuka scope loop setiap tieBreakerCode dari `new[] { ”tie_breaker_1”, ”tie_breaker_2”, ”tie_breaker_3”, ”tie_breaker_4” }`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DefaultRulesetSeed_ShouldIncludePemulaTieBreakerAssets.
        {
            // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan
            // `$@”\(\s*'f5b4c67b-0825-4970-9f07-3b68e8fcb524'\s*::\s*uuid\s*,\s*'TIE_BREAKER'\s*,\s*'{tieBreakerCode}'”`, `seedContent`; ketidaksesuaian dengan
            // ekspektasi membuat pengujian gagal dalam DefaultRulesetSeed_ShouldIncludePemulaTieBreakerAssets.
            Assert.Matches(
                // Meneruskan teks interpolasi `$@”\(\s*'f5b4c67b-0825-4970-9f07-3b68e8fcb524'\s*::\s*uuid\s*,\s*'TIE_BREAKER'\s*,\s*'{tieBreakerCode}'”`; nilai
                // ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `Assert.Matches`.
                $@"\(\s*'f5b4c67b-0825-4970-9f07-3b68e8fcb524'\s*::\s*uuid\s*,\s*'TIE_BREAKER'\s*,\s*'{tieBreakerCode}'",
                // Meneruskan `seedContent` (nilai seed content) sebagai argumen ke `Assert.Matches`.
                seedContent);
            // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan
            // `$@”\(\s*'f5b4c67b-0825-4970-9f07-3b68e8fcb524'\s*::\s*uuid\s*,\s*'{tieBreakerCode}'\s*,\s*\d+\s*,\s*\d+\s*,\s*1\s*,\s*'\{{\””number\””:”`,
            // `seedContent`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam DefaultRulesetSeed_ShouldIncludePemulaTieBreakerAssets.
            Assert.Matches(
                // Meneruskan teks interpolasi `$@”\(\s*'f5b4c67b-0825-4970-9f07-3b68e8fcb524'\s*::\s*uuid\s*,\s*'{tieBreakerCode}'\s*,\s*\d+\s*,\s*\d+\s*,\s*1\s*,\
                // s*'\{{\””number\””:”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `Assert.Matches`.
                $@"\(\s*'f5b4c67b-0825-4970-9f07-3b68e8fcb524'\s*::\s*uuid\s*,\s*'{tieBreakerCode}'\s*,\s*\d+\s*,\s*\d+\s*,\s*1\s*,\s*'\{{\""number\"":",
                // Meneruskan `seedContent` (nilai seed content) sebagai argumen ke `Assert.Matches`.
                seedContent);
        // Menutup scope loop setiap tieBreakerCode dari `new[] { ”tie_breaker_1”, ”tie_breaker_2”, ”tie_breaker_3”, ”tie_breaker_4” }`; bagian berikut
        // berada di luar batas blok tersebut dalam DefaultRulesetSeed_ShouldIncludePemulaTieBreakerAssets.
        }
    // Menutup scope metode DefaultRulesetSeed_ShouldIncludePemulaTieBreakerAssets; bagian berikut berada di luar batas blok tersebut dalam
    // DefaultRulesetSeed_ShouldIncludePemulaTieBreakerAssets.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook` dengan hasil bertipe `void`; operasi ini menangani
    // bawaan aturan seed should keep aturan baselines aligned dengan rulebook.
    public void DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook()
    // Membuka scope metode DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook.
    {
        // Menyiapkan variabel lokal `seedPath` untuk nilai seed path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”01_seed_default_rulesets_components.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seedPath = Path.Combine(RepoRoot, "database", "01_seed_default_rulesets_components.sql");
        // Menyiapkan variabel lokal `seedContent` untuk nilai seed content dengan memanggil `File.ReadAllText` dengan `seedPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var seedContent = File.ReadAllText(seedPath);

        // Menyiapkan variabel lokal `startingCashValues` untuk nilai starting uang tunai nilai dengan mematerialisasi urutan `Regex.Matches(seedContent,
        // @”””starting_cash””\s*:\s*(\d+)”, RegexOptions.IgnoreCase) .Select(match => int.Parse(match.Groups[1].Value))` menjadi List; enumerasi dijalankan
        // dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var startingCashValues = Regex.Matches(seedContent, @"""starting_cash""\s*:\s*(\d+)", RegexOptions.IgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(match => int.Parse(match.Groups[1].Value)) dalam
            // DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(match => int.Parse(match.Groups[1].Value))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam
            // DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `actionsPerTurnValues` untuk nilai aksi per giliran nilai dengan mematerialisasi urutan `Regex.Matches(seedContent,
        // @”””actions_per_turn””\s*:\s*(\d+)”, RegexOptions.IgnoreCase) .Select(match => int.Parse(match.Groups[1].Value))` menjadi List; enumerasi
        // dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var actionsPerTurnValues = Regex.Matches(seedContent, @"""actions_per_turn""\s*:\s*(\d+)", RegexOptions.IgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(match => int.Parse(match.Groups[1].Value)) dalam
            // DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(match => int.Parse(match.Groups[1].Value))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam
            // DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `finishDayValues` untuk nilai finish hari nilai dengan mematerialisasi urutan `Regex.Matches(seedContent,
        // @”””finishDay””\s*:\s*(\d+)”, RegexOptions.IgnoreCase) .Select(match => int.Parse(match.Groups[1].Value))` menjadi List; enumerasi dijalankan dan
        // hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var finishDayValues = Regex.Matches(seedContent, @"""finishDay""\s*:\s*(\d+)", RegexOptions.IgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(match => int.Parse(match.Groups[1].Value)) dalam
            // DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(match => int.Parse(match.Groups[1].Value))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam
            // DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `donationPointBlocks` untuk nilai donasi point blocks dengan mematerialisasi urutan `Regex.Matches(seedContent,
        // @”””score_source””\s*:\s*””DONATION””\s*,\s*””rank””\s*:\s*(?<rank>\d)\s*,\s*””points””\s*:\s*(?<points>\d+)”, RegexOptions.IgnoreCase)
        // .Select(matc...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var donationPointBlocks = Regex.Matches(seedContent, @"""score_source""\s*:\s*""DONATION""\s*,\s*""rank""\s*:\s*(?<rank>\d)\s*,\s*""points""\s*:\s*(?<points>\d+)", RegexOptions.IgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(match =>
            // $”{match.Groups[”rank”].Value}:{match.Groups[”points”].Value}”) dalam DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(match => $"{match.Groups["rank"].Value}:{match.Groups["points"].Value}")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam
            // DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `pensionPointBlocks` untuk nilai pension point blocks dengan mematerialisasi urutan `Regex.Matches(seedContent,
        // @”””score_source””\s*:\s*””PENSION””\s*,\s*””rank””\s*:\s*(?<rank>\d)\s*,\s*””points””\s*:\s*(?<points>\d+)”, RegexOptions.IgnoreCase)
        // .Select(match...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var pensionPointBlocks = Regex.Matches(seedContent, @"""score_source""\s*:\s*""PENSION""\s*,\s*""rank""\s*:\s*(?<rank>\d)\s*,\s*""points""\s*:\s*(?<points>\d+)", RegexOptions.IgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(match =>
            // $”{match.Groups[”rank”].Value}:{match.Groups[”points”].Value}”) dalam DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(match => $"{match.Groups["rank"].Value}:{match.Groups["points"].Value}")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam
            // DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `missionPenaltyValues` untuk nilai misi penalti nilai dengan mematerialisasi urutan `Regex.Matches(seedContent,
        // @”””success_points””\s*:\s*(?<success>-?\d+)\s*,\s*””failure_points””\s*:\s*(?<failure>-?\d+)”, RegexOptions.IgnoreCase) .Select(match =>
        // $”{match.G...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var missionPenaltyValues = Regex.Matches(seedContent, @"""success_points""\s*:\s*(?<success>-?\d+)\s*,\s*""failure_points""\s*:\s*(?<failure>-?\d+)", RegexOptions.IgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(match =>
            // $”{match.Groups[”success”].Value}:{match.Groups[”failure”].Value}”) dalam DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(match => $"{match.Groups["success"].Value}:{match.Groups["failure"].Value}")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam
            // DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();

        // Menjalankan pemeriksaan hasil dengan `Assert.NotEmpty` menggunakan `startingCashValues`; ketidaksesuaian dengan ekspektasi membuat pengujian
        // gagal dalam DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook.
        Assert.NotEmpty(startingCashValues);
        // Menjalankan pemeriksaan hasil dengan `Assert.NotEmpty` menggunakan `actionsPerTurnValues`; ketidaksesuaian dengan ekspektasi membuat pengujian
        // gagal dalam DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook.
        Assert.NotEmpty(actionsPerTurnValues);
        // Menjalankan pemeriksaan hasil dengan `Assert.NotEmpty` menggunakan `finishDayValues`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal
        // dalam DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook.
        Assert.NotEmpty(finishDayValues);
        // Menjalankan pemeriksaan hasil dengan `Assert.NotEmpty` menggunakan `donationPointBlocks`; ketidaksesuaian dengan ekspektasi membuat pengujian
        // gagal dalam DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook.
        Assert.NotEmpty(donationPointBlocks);
        // Menjalankan pemeriksaan hasil dengan `Assert.NotEmpty` menggunakan `pensionPointBlocks`; ketidaksesuaian dengan ekspektasi membuat pengujian
        // gagal dalam DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook.
        Assert.NotEmpty(pensionPointBlocks);
        // Menjalankan pemeriksaan hasil dengan `Assert.NotEmpty` menggunakan `missionPenaltyValues`; ketidaksesuaian dengan ekspektasi membuat pengujian
        // gagal dalam DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook.
        Assert.NotEmpty(missionPenaltyValues);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `20`, `startingCashValues` dalam
        // DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook.
        Assert.Contains(20, startingCashValues);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `10`, `startingCashValues` dalam
        // DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook.
        Assert.Contains(10, startingCashValues);
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `startingCashValues`, `value => Assert.Contains(value, new[] { 10, 20 })`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook.
        Assert.All(startingCashValues, value => Assert.Contains(value, new[] { 10, 20 }));
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `actionsPerTurnValues`, `value => Assert.Equal(2, value)`; ketidaksesuaian dengan
        // ekspektasi membuat pengujian gagal dalam DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook.
        Assert.All(actionsPerTurnValues, value => Assert.Equal(2, value));
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `finishDayValues`, `value => Assert.Equal(25, value)`; ketidaksesuaian dengan
        // ekspektasi membuat pengujian gagal dalam DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook.
        Assert.All(finishDayValues, value => Assert.Equal(25, value));
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”1:7”, ”2:5”, ”3:2”, ”1:7”, ”2:5”,
        // ”3:2” }`, `donationPointBlocks`); pengujian gagal jika keduanya berbeda dalam DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook.
        Assert.Equal(
            // Meneruskan array baru dengan tipe elemen disimpulkan dari nilai initializer sebagai argumen ke `Assert.Equal`.
            new[] { "1:7", "2:5", "3:2", "1:7", "2:5", "3:2" },
            // Meneruskan `donationPointBlocks` (nilai donasi point blocks) sebagai argumen ke `Assert.Equal`.
            donationPointBlocks);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { ”1:5”, ”2:3”, ”3:1”, ”1:5”, ”2:3”,
        // ”3:1” }`, `pensionPointBlocks`); pengujian gagal jika keduanya berbeda dalam DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook.
        Assert.Equal(
            // Meneruskan array baru dengan tipe elemen disimpulkan dari nilai initializer sebagai argumen ke `Assert.Equal`.
            new[] { "1:5", "2:3", "3:1", "1:5", "2:3", "3:1" },
            // Meneruskan `pensionPointBlocks` (nilai pension point blocks) sebagai argumen ke `Assert.Equal`.
            pensionPointBlocks);
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `missionPenaltyValues`, `value => Assert.Equal(”0:-10”, value)`; ketidaksesuaian
        // dengan ekspektasi membuat pengujian gagal dalam DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook.
        Assert.All(missionPenaltyValues, value => Assert.Equal("0:-10", value));
    // Menutup scope metode DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook; bagian berikut berada di luar batas blok tersebut dalam
    // DefaultRulesetSeed_ShouldKeepRulesetBaselinesAlignedWithRulebook.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ManualSimulationSeed_ShouldExist_AsStandaloneManualSql` dengan hasil bertipe `void`; operasi ini menangani manual
    // simulation seed should exist as standalone manual SQL.
    public void ManualSimulationSeed_ShouldExist_AsStandaloneManualSql()
    // Membuka scope metode ManualSimulationSeed_ShouldExist_AsStandaloneManualSql; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
    {
        // Menyiapkan variabel lokal `seedPath` untuk nilai seed path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”02_seed_simulation_sessions_events.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seedPath = Path.Combine(RepoRoot, "database", "02_seed_simulation_sessions_events.sql");

        // Menjalankan pemeriksaan bahwa `File.Exists(seedPath)`, `$”Seed simulasi manual harus tersedia pada path '{seedPath}'.”` bernilai benar; pengujian
        // gagal jika kondisi tidak terpenuhi dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.True(File.Exists(seedPath), $"Seed simulasi manual harus tersedia pada path '{seedPath}'.");

        // Menyiapkan variabel lokal `seedContent` untuk nilai seed content dengan memanggil `File.ReadAllText` dengan `seedPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var seedContent = File.ReadAllText(seedPath);
        // Menyiapkan variabel lokal `executableSeedContent` untuk nilai executable seed content dengan memanggil `Regex.Replace` dengan
        // `seedContent.TrimStart()`, `@”^(?:--[^\r\n]*(?:\r?\n|$)\s*)+”`, `string.Empty`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var executableSeedContent = Regex.Replace(seedContent.TrimStart(), @"^(?:--[^\r\n]*(?:\r?\n|$)\s*)+", string.Empty);
        // Menjalankan pemeriksaan hasil dengan `Assert.StartsWith` menggunakan `”begin;”`, `executableSeedContent`, `StringComparison.OrdinalIgnoreCase`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.StartsWith("begin;", executableSeedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan hasil dengan `Assert.EndsWith` menggunakan `”commit;”`, `seedContent.TrimEnd()`, `StringComparison.OrdinalIgnoreCase`;
        // ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.EndsWith("commit;", seedContent.TrimEnd(), StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Simulasi Cashflowpoly Kelas XI IPS 2 -
        // Mode Pemula - Kelompok A”`, `seedContent`, `StringComparison.Ordinal` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.Contains("Simulasi Cashflowpoly Kelas XI IPS 2 - Mode Pemula - Kelompok A", seedContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Simulasi Cashflowpoly Kelas XI IPS 2 -
        // Mode Mahir - Kelompok B”`, `seedContent`, `StringComparison.Ordinal` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.Contains("Simulasi Cashflowpoly Kelas XI IPS 2 - Mode Mahir - Kelompok B", seedContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into
        // session_participant_assets”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("insert into session_participant_assets", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into
        // session_participant_gold_holdings”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("insert into session_participant_gold_holdings", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into
        // session_participant_loans”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("insert into session_participant_loans", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into
        // session_participant_insurances”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("insert into session_participant_insurances", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into
        // session_participant_tie_breakers”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("insert into session_participant_tie_breakers", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into
        // session_participant_collection_missions”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("insert into session_participant_collection_missions", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into
        // session_pension_rankings”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("insert into session_pension_rankings", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan memanggil `AssertSqlContains` dengan `”insert into session_final_scores”`, `seedContent` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        AssertSqlContains("insert into session_final_scores", seedContent);
        // Menjalankan memanggil `AssertSqlContains` dengan `”insert into session_final_score_components”`, `seedContent` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        AssertSqlContains("insert into session_final_score_components", seedContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”BagikanTieBreaker”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.Contains("BagikanTieBreaker", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”tie_breaker.assigned”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("tie_breaker.assigned", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”session_participant_collection_missions”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.Contains("session_participant_collection_missions", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”session_participant_quest_progress”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("session_participant_quest_progress", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”session_ruleset_activations”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("session_ruleset_activations", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan memanggil `AssertSqlContains` dengan `”spb.coins + spb.saving + coalesce(ingredients.leftover_qty, 0)”`, `seedContent` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        AssertSqlContains("spb.coins + spb.saving + coalesce(ingredients.leftover_qty, 0)", seedContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”least(latest.day_index,
        // rgs.finish_day)”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("least(latest.day_index, rgs.finish_day)", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `@”””ingredient_name””:””Bumbu”””`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain(@"""ingredient_name"":""Bumbu""", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'donation.rank.awarded',
        // 'donation.rank.awarded', '{\”rank\”:1,\”points\”:5}'”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain(
            // Meneruskan nilai literal `”'donation.rank.awarded', 'donation.rank.awarded', '{\”rank\”:1,\”points\”:5}'”` sebagai argumen ke
            // `Assert.DoesNotContain`.
            "'donation.rank.awarded', 'donation.rank.awarded', '{\"rank\":1,\"points\":5}'",
            // Meneruskan `seedContent` (nilai seed content) sebagai argumen ke `Assert.DoesNotContain`.
            seedContent,
            // Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `Assert.DoesNotContain`.
            StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”set local
        // cashflowpoly.bypass_validation”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("set local cashflowpoly.bypass_validation", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”disable trigger all”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("disable trigger all", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”enable trigger all”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("enable trigger all", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”order_setup_placeholder”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("order_setup_placeholder", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”asset_code\”:\”buku\””`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("\"asset_code\":\"buku\"", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”risk_event_ref\””`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("\"risk_event_ref\"", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Seed simulasi melampaui stok
        // kartu fisik”`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("Seed simulasi melampaui stok kartu fisik", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”risk_id”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.Contains("risk_id", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”\”risk_id\”:\”risk_investasi_emas\””`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.Contains("\"risk_id\":\"risk_investasi_emas\"", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”\”direction\”:\”IN\”,\”amount\””`, `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("\"direction\":\"IN\",\"amount\"", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”{\”goal_id\”:\”tujuan_35\”,\”cost\”:35,\”points\”:35}”`, `seedContent`, `StringComparison.Ordinal` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.Contains("{\"goal_id\":\"tujuan_35\",\"cost\":35,\"points\":35}", seedContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”{\”goal_id\”:\”tujuan_25\”,\”cost\”:25,\”points\”:20}”`, `seedContent`, `StringComparison.Ordinal` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("{\"goal_id\":\"tujuan_25\",\"cost\":25,\"points\":20}", seedContent, StringComparison.Ordinal);

        // Menyiapkan variabel lokal `applyIndex` untuk nilai apply index dengan memanggil `seedContent.IndexOf` dengan `”perform apply_game_event(”`,
        // `StringComparison.OrdinalIgnoreCase`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var applyIndex = seedContent.IndexOf("perform apply_game_event(", StringComparison.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `finalScoreIndex` untuk nilai akhir skor index dengan memanggil `seedContent.IndexOf` dengan `”create temporary table
        // seed_pension_rank_points”`, `StringComparison.OrdinalIgnoreCase`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var finalScoreIndex = seedContent.IndexOf("create temporary table seed_pension_rank_points", StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan bahwa `applyIndex >= 0`, `”Manual simulation seed must insert events through apply_game_event.”` bernilai benar;
        // pengujian gagal jika kondisi tidak terpenuhi dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.True(applyIndex >= 0, "Manual simulation seed must insert events through apply_game_event.");
        // Menjalankan pemeriksaan bahwa `finalScoreIndex > applyIndex`, `”Final score reporting must run after apply_game_event.”` bernilai benar;
        // pengujian gagal jika kondisi tidak terpenuhi dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.True(finalScoreIndex > applyIndex, "Final score reporting must run after apply_game_event.");

        // Menyiapkan variabel lokal `postApplyRuntimeBlock` untuk nilai post apply runtime block dengan memanggil `seedContent.Substring` dengan
        // `applyIndex`, `finalScoreIndex - applyIndex`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var postApplyRuntimeBlock = seedContent.Substring(applyIndex, finalScoreIndex - applyIndex);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”delete from
        // event_cashflow_projections”`, `postApplyRuntimeBlock`, `StringComparison.OrdinalIgnoreCase` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("delete from event_cashflow_projections", postApplyRuntimeBlock, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into
        // event_cashflow_projections”`, `postApplyRuntimeBlock`, `StringComparison.OrdinalIgnoreCase` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("insert into event_cashflow_projections", postApplyRuntimeBlock, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”delete from
        // session_participant_balances”`, `postApplyRuntimeBlock`, `StringComparison.OrdinalIgnoreCase` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("delete from session_participant_balances", postApplyRuntimeBlock, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”insert into
        // session_participant_financial_goals”`, `postApplyRuntimeBlock`, `StringComparison.OrdinalIgnoreCase` dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.DoesNotContain("insert into session_participant_financial_goals", postApplyRuntimeBlock, StringComparison.OrdinalIgnoreCase);

        // Menyiapkan variabel lokal `consumingActionListPattern` untuk nilai consuming aksi daftar pattern dengan nilai literal
        // `@”'BahanMasakan'[\s\S]{0,160}'BuangBahanMasakan'[\s\S]{0,160}'JualMasakan'[\s\S]{0,160}'Kebutuhan'[\s\S]{0,160}'KerjaLepas'[\s\S]{0,160}'Menabun
        // g'[\s\S]{0,160}'TarikTabungan'[...`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var consumingActionListPattern =
            // Menggunakan nilai literal `@”'BahanMasakan'[\s\S]{0,160}'BuangBahanMasakan'[\s\S]{0,160}'JualMasakan'[\s\S]{0,160}'Kebutuhan'[\s\S]{0,160}'KerjaL
            // epas'[\s\S]{0,160}'Menabung'[\s\S]{0,160}'TarikTabungan'[...` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
            @"'BahanMasakan'[\s\S]{0,160}'BuangBahanMasakan'[\s\S]{0,160}'JualMasakan'[\s\S]{0,160}'Kebutuhan'[\s\S]{0,160}'KerjaLepas'[\s\S]{0,160}'Menabung'[\s\S]{0,160}'TarikTabungan'[\s\S]{0,160}'TujuanFinansial'[\s\S]{0,160}'BayarPinjaman'";
        // Menjalankan pemeriksaan bahwa `Regex.Matches(seedContent, consumingActionListPattern, RegexOptions.IgnoreCase).Count >= 3`, `”Manual simulation
        // seed must use the canonical consuming-action list in all action_slot calculations.”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi
        // dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        Assert.True(
            // Meneruskan pemeriksaan lebih besar atau sama antara `Regex.Matches(seedContent, consumingActionListPattern, RegexOptions.IgnoreCase).Count` dan
            // `3` sebagai argumen ke `Assert.True`; Meneruskan `seedContent` (nilai seed content) sebagai argumen ke `Regex.Matches`; Meneruskan
            // `consumingActionListPattern` (nilai consuming aksi daftar pattern) sebagai argumen ke `Regex.Matches`; Meneruskan `RegexOptions.IgnoreCase`
            // (nilai ignore case) sebagai argumen ke `Regex.Matches`.
            Regex.Matches(seedContent, consumingActionListPattern, RegexOptions.IgnoreCase).Count >= 3,
            // Meneruskan nilai literal `”Manual simulation seed must use the canonical consuming-action list in all action_slot calculations.”` sebagai argumen
            // ke `Assert.True`.
            "Manual simulation seed must use the canonical consuming-action list in all action_slot calculations.");

        // Menyiapkan variabel lokal `forbidden` untuk nilai forbidden dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var forbidden = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        {
            // Menggunakan nilai literal `”session_donation_event_rankings”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
            "session_donation_event_rankings",
            // Menggunakan nilai literal `”event_inventory_effects”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
            "event_inventory_effects",
            // Menggunakan nilai literal `”event_need_effects”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
            "event_need_effects",
            // Menggunakan nilai literal `”event_goal_effects”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
            "event_goal_effects",
            // Menggunakan nilai literal `”event_asset_effects”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
            "event_asset_effects",
            // Menggunakan nilai literal `”event_score_effects”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
            "event_score_effects",
            // Menggunakan nilai literal `”event_turn_effects”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
            "event_turn_effects",
            // Menggunakan nilai literal `”ruleset_versions.config_json”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
            "ruleset_versions.config_json"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        };

        // Mengulangi setiap elemen `forbidden`; elemen saat ini disimpan sebagai `entry` bertipe `var` untuk diproses oleh badan loop dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        foreach (var entry in forbidden)
        // Membuka scope loop setiap entry dari `forbidden`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        {
            // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `entry`, `seedContent`,
            // `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
            Assert.DoesNotContain(entry, seedContent, StringComparison.OrdinalIgnoreCase);
        // Menutup scope loop setiap entry dari `forbidden`; bagian berikut berada di luar batas blok tersebut dalam
        // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
        }
    // Menutup scope metode ManualSimulationSeed_ShouldExist_AsStandaloneManualSql; bagian berikut berada di luar batas blok tersebut dalam
    // ManualSimulationSeed_ShouldExist_AsStandaloneManualSql.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence` dengan hasil bertipe `void`; operasi ini
    // menangani manual simulation seed should use runtime emas scoring tie breakers dan mahir risiko sequence.
    public void ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence()
    // Membuka scope metode ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence; pernyataan/deklarasi berikut berada di
    // dalam batas blok ini dalam ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence.
    {
        // Menyiapkan variabel lokal `seedPath` untuk nilai seed path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”database”`,
        // `”02_seed_simulation_sessions_events.sql”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seedPath = Path.Combine(RepoRoot, "database", "02_seed_simulation_sessions_events.sql");
        // Menyiapkan variabel lokal `seedContent` untuk nilai seed content dengan memanggil `File.ReadAllText` dengan `seedPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var seedContent = File.ReadAllText(seedPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”resolve_gold_points(”`, `seedContent`,
        // `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence.
        Assert.Contains("resolve_gold_points(", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”sp.ruleset_version_id”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence.
        Assert.DoesNotContain("sp.ruleset_version_id", seedContent, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rga.quantity = spgh.quantity”`,
        // `seedContent`, `StringComparison.OrdinalIgnoreCase` dalam ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence.
        Assert.DoesNotContain("rga.quantity = spgh.quantity", seedContent, StringComparison.OrdinalIgnoreCase);

        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan mematerialisasi
        // urutan `ParseSimulationSeedEvents(seedContent)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var events = ParseSimulationSeedEvents(seedContent).ToList();
        // Menyiapkan variabel lokal `pemulaTieBreakers` untuk nilai pemula tie breakers dengan mematerialisasi urutan `events .Where(e => e.Mode ==
        // ”PEMULA” && e.ActionType == ”BagikanTieBreaker”)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var pemulaTieBreakers = events
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => e.Mode == ”PEMULA” && e.ActionType == ”BagikanTieBreaker”) dalam
            // ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .Where(e => e.Mode == "PEMULA" && e.ActionType == "BagikanTieBreaker")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam
            // ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .ToList();
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `pemulaTieBreakers.Count`); pengujian
        // gagal jika keduanya berbeda dalam ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence.
        Assert.Equal(4, pemulaTieBreakers.Count);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`new[] { 1, 2, 3, 4 }`,
        // `pemulaTieBreakers.Select(e => e.PlayerNo.GetValueOrDefault()).OrderBy(playerNo => playerNo).ToArray()`); pengujian gagal jika keduanya berbeda
        // dalam ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence.
        Assert.Equal(
            // Meneruskan array baru dengan tipe elemen disimpulkan dari nilai initializer sebagai argumen ke `Assert.Equal`.
            new[] { 1, 2, 3, 4 },
            // Meneruskan mematerialisasi urutan `pemulaTieBreakers.Select(e => e.PlayerNo.GetValueOrDefault()).OrderBy(playerNo => playerNo)` menjadi array
            // dengan elemen hasil saat ini sebagai argumen ke `Assert.Equal`; Meneruskan fungsi lambda `e => e.PlayerNo.GetValueOrDefault()` yang dijalankan
            // oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `pemulaTieBreakers.Select`; Meneruskan fungsi lambda `playerNo =>
            // playerNo` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `pemulaTieBreakers.Select(e =>
            // e.PlayerNo.GetValueOrDefault()).OrderBy`.
            pemulaTieBreakers.Select(e => e.PlayerNo.GetValueOrDefault()).OrderBy(playerNo => playerNo).ToArray());
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `pemulaTieBreakers`, `e => Assert.Equal(0, e.DayIndex)`; ketidaksesuaian dengan
        // ekspektasi membuat pengujian gagal dalam ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence.
        Assert.All(pemulaTieBreakers, e => Assert.Equal(0, e.DayIndex));

        // Menyiapkan variabel lokal `mahirPlayerEvents` untuk nilai mahir pemain event dengan mematerialisasi urutan `events .Where(e => e.Mode == ”MAHIR”
        // && e.ActorType == ”PLAYER”) .OrderBy(e => e.DayIndex) .ThenBy(e => e.EventOrder) .ThenBy(e => e.ActionType, StringComparer.Ordinal) .ThenB...`
        // menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var mahirPlayerEvents = events
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => e.Mode == ”MAHIR” && e.ActorType == ”PLAYER”) dalam
            // ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .Where(e => e.Mode == "MAHIR" && e.ActorType == "PLAYER")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(e => e.DayIndex) dalam
            // ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .OrderBy(e => e.DayIndex)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(e => e.EventOrder) dalam
            // ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .ThenBy(e => e.EventOrder)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(e => e.ActionType, StringComparer.Ordinal) dalam
            // ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .ThenBy(e => e.ActionType, StringComparer.Ordinal)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ThenBy(e => e.PlayerNo ?? 0) dalam
            // ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .ThenBy(e => e.PlayerNo ?? 0)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam
            // ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `invalidRisks` untuk nilai invalid risks dengan mematerialisasi urutan `mahirPlayerEvents .Select((e, index) => new {
        // Event = e, Previous = index > 0 ? mahirPlayerEvents[index - 1] : null }) .Where(pair => pair.Event.ActionType == ”RisikoKehidupan...` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var invalidRisks = mahirPlayerEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select((e, index) => new { Event = e, Previous = index > 0 ?
            // mahirPlayerEvents[index - 1] : null }) dalam ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select((e, index) => new { Event = e, Previous = index > 0 ? mahirPlayerEvents[index - 1] : null })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(pair => pair.Event.ActionType == ”RisikoKehidupan” dalam
            // ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .Where(pair => pair.Event.ActionType == "RisikoKehidupan"
                // Meneruskan fungsi lambda `pair => pair.Event.ActionType == ”RisikoKehidupan” && (pair.Previous is null || pair.Previous.ActionType !=
                // ”JualMasakan” || pair.Previous.PlayerNo != pair.Event.PlayerNo)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                // argumen ke `mahirPlayerEvents .Select((e, index) => new { Event = e, Previous = index > 0 ? mahirPlayerEvents[index - 1] : null }) .Where`.
                && (pair.Previous is null
                    // Meneruskan fungsi lambda `pair => pair.Event.ActionType == ”RisikoKehidupan” && (pair.Previous is null || pair.Previous.ActionType !=
                    // ”JualMasakan” || pair.Previous.PlayerNo != pair.Event.PlayerNo)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                    // argumen ke `mahirPlayerEvents .Select((e, index) => new { Event = e, Previous = index > 0 ? mahirPlayerEvents[index - 1] : null }) .Where`.
                    || pair.Previous.ActionType != "JualMasakan"
                    // Meneruskan fungsi lambda `pair => pair.Event.ActionType == ”RisikoKehidupan” && (pair.Previous is null || pair.Previous.ActionType !=
                    // ”JualMasakan” || pair.Previous.PlayerNo != pair.Event.PlayerNo)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                    // argumen ke `mahirPlayerEvents .Select((e, index) => new { Event = e, Previous = index > 0 ? mahirPlayerEvents[index - 1] : null }) .Where`.
                    || pair.Previous.PlayerNo != pair.Event.PlayerNo))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(pair => $”{pair.Event.RefKey ?? ”<null>”} day={pair.Event.DayIndex}
            // player={pair.Event.PlayerNo}”) dalam ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(pair => $"{pair.Event.RefKey ?? "<null>"} day={pair.Event.DayIndex} player={pair.Event.PlayerNo}")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam
            // ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .ToList();
        // Menjalankan pemeriksaan hasil dengan `Assert.Empty` menggunakan `invalidRisks`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence.
        Assert.Empty(invalidRisks);
    // Menutup scope metode ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence; bagian berikut berada di luar batas blok
    // tersebut dalam ManualSimulationSeed_ShouldUseRuntimeGoldScoringTieBreakersAndMahirRiskSequence.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles` dengan hasil bertipe `void`; operasi ini menangani manual
    // simulation seed should provide distinct human decision profiles.
    public void ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles()
    // Membuka scope metode ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
    {
        // Menyiapkan variabel lokal `seedContent` untuk nilai seed content dengan memanggil `File.ReadAllText` dengan `Path.Combine(RepoRoot, ”database”,
        // ”02_seed_simulation_sessions_events.sql”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var seedContent = File.ReadAllText(Path.Combine(RepoRoot, "database", "02_seed_simulation_sessions_events.sql"));
        // Menyiapkan variabel lokal `consumingActions` untuk nilai consuming aksi dengan objek baru bertipe `HashSet<string>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var consumingActions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
        {
            // Menggunakan nilai literal `”BahanMasakan”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
            "BahanMasakan",
            // Menggunakan nilai literal `”BuangBahanMasakan”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
            "BuangBahanMasakan",
            // Menggunakan nilai literal `”JualMasakan”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
            "JualMasakan",
            // Menggunakan nilai literal `”Kebutuhan”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
            "Kebutuhan",
            // Menggunakan nilai literal `”KerjaLepas”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
            "KerjaLepas",
            // Menggunakan nilai literal `”Menabung”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
            "Menabung",
            // Menggunakan nilai literal `”TarikTabungan”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
            "TarikTabungan",
            // Menggunakan nilai literal `”TujuanFinansial”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
            "TujuanFinansial",
            // Menggunakan nilai literal `”BayarPinjaman”` sebagai bagian ekspresi yang sedang disusun dalam
            // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
            "BayarPinjaman"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
        };
        // Menyiapkan variabel lokal `beginnerActions` untuk nilai beginner aksi dengan mematerialisasi urutan `ParseSimulationSeedEvents(seedContent)
        // .Where(evt => evt.Mode == ”PEMULA” && evt.ActorType == ”PLAYER” && evt.PlayerNo.HasValue && consumingActions.Contains(evt.ActionType))` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var beginnerActions = ParseSimulationSeedEvents(seedContent)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(evt => evt.Mode == ”PEMULA” dalam
            // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(evt => evt.Mode == "PEMULA"
                // Meneruskan fungsi lambda `evt => evt.Mode == ”PEMULA” && evt.ActorType == ”PLAYER” && evt.PlayerNo.HasValue &&
                // consumingActions.Contains(evt.ActionType)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `ParseSimulationSeedEvents(seedContent) .Where`.
                && evt.ActorType == "PLAYER"
                // Meneruskan fungsi lambda `evt => evt.Mode == ”PEMULA” && evt.ActorType == ”PLAYER” && evt.PlayerNo.HasValue &&
                // consumingActions.Contains(evt.ActionType)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `ParseSimulationSeedEvents(seedContent) .Where`.
                && evt.PlayerNo.HasValue
                // Meneruskan `evt.ActionType` (nilai aksi jenis) sebagai argumen ke `consumingActions.Contains`.
                && consumingActions.Contains(evt.ActionType))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam
            // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();

        // Mengulangi setiap elemen `Enumerable.Range(1, 4)`; elemen saat ini disimpan sebagai `playerNo` bertipe `var` untuk diproses oleh badan loop dalam
        // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
        foreach (var playerNo in Enumerable.Range(1, 4))
        // Membuka scope loop setiap playerNo dari `Enumerable.Range(1, 4)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
        {
            // Menyiapkan variabel lokal `playerActions` untuk nilai pemain aksi dengan mematerialisasi urutan `beginnerActions.Where(evt => evt.PlayerNo ==
            // playerNo)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var playerActions = beginnerActions.Where(evt => evt.PlayerNo == playerNo).ToList();
            // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`32`, `playerActions.Count`); pengujian gagal
            // jika keduanya berbeda dalam ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
            Assert.Equal(32, playerActions.Count);
            // Menjalankan pemeriksaan bahwa `playerActions.Count(evt => evt.ActionType == ”JualMasakan”) >= 3` bernilai benar; pengujian gagal jika kondisi
            // tidak terpenuhi dalam ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
            Assert.True(playerActions.Count(evt => evt.ActionType == "JualMasakan") >= 3);
            // Menjalankan pemeriksaan bahwa `playerActions.Count(evt => evt.ActionType == ”KerjaLepas”) < playerActions.Count / 2` bernilai benar; pengujian
            // gagal jika kondisi tidak terpenuhi dalam ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
            Assert.True(playerActions.Count(evt => evt.ActionType == "KerjaLepas") < playerActions.Count / 2);
            // Menjalankan pemeriksaan bahwa `playerActions.Select(evt => evt.ActionType).Distinct(StringComparer.OrdinalIgnoreCase).Count() >= 4` bernilai
            // benar; pengujian gagal jika kondisi tidak terpenuhi dalam ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
            Assert.True(playerActions.Select(evt => evt.ActionType).Distinct(StringComparer.OrdinalIgnoreCase).Count() >= 4);
        // Menutup scope loop setiap playerNo dari `Enumerable.Range(1, 4)`; bagian berikut berada di luar batas blok tersebut dalam
        // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
        }

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `beginnerActions`, `evt => evt.ActionType
        // == ”BuangBahanMasakan”` dalam ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
        Assert.Contains(beginnerActions, evt => evt.ActionType == "BuangBahanMasakan");
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `beginnerActions`, `evt =>
        // evt.ActionType == ”LewatiOrder”` dalam ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
        Assert.DoesNotContain(beginnerActions, evt => evt.ActionType == "LewatiOrder");
    // Menutup scope metode ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles; bagian berikut berada di luar batas blok tersebut dalam
    // ManualSimulationSeed_ShouldProvideDistinctHumanDecisionProfiles.
    }

    // Mendefinisikan metode `ResolveRepositoryRoot` dengan hasil bertipe `string`; operasi ini menangani resolve repositori root.
    private static string ResolveRepositoryRoot()
    // Membuka scope metode ResolveRepositoryRoot; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveRepositoryRoot.
    {
        // Menyiapkan variabel lokal `current` untuk nilai saat ini dengan objek baru bertipe `DirectoryInfo` dengan argumen (AppContext.BaseDirectory).
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        // Mengulangi blok selama hasil pencocokan `current` dengan pola `not null`; kondisi diperiksa lagi sebelum setiap iterasi dalam
        // ResolveRepositoryRoot.
        while (current is not null)
        // Membuka scope loop selama `current is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveRepositoryRoot.
        {
            // Memeriksa memanggil `File.Exists` dengan `Path.Combine(current.FullName, ”Cashflowpoly.sln”)`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam ResolveRepositoryRoot.
            if (File.Exists(Path.Combine(current.FullName, "Cashflowpoly.sln")))
            // Membuka scope cabang if untuk kondisi `File.Exists(Path.Combine(current.FullName, ”Cashflowpoly.sln”))`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam ResolveRepositoryRoot.
            {
                // Mengembalikan `current.FullName` (nilai full nama) kepada pemanggil dalam ResolveRepositoryRoot; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return current.FullName;
            // Menutup scope cabang if untuk kondisi `File.Exists(Path.Combine(current.FullName, ”Cashflowpoly.sln”))`; bagian berikut berada di luar batas blok
            // tersebut dalam ResolveRepositoryRoot.
            }

            // Memperbarui `current` menggunakan `current.Parent` (nilai parent) dalam ResolveRepositoryRoot.
            current = current.Parent;
        // Menutup scope loop selama `current is not null`; bagian berikut berada di luar batas blok tersebut dalam ResolveRepositoryRoot.
        }

        // Menghentikan alur dengan melempar objek baru bertipe `DirectoryNotFoundException` dengan argumen (”Tidak dapat menemukan root repositori
        // (Cashflowpoly.sln).”) dalam ResolveRepositoryRoot; pemanggil atau middleware penanganan error menerima kegagalan ini.
        throw new DirectoryNotFoundException("Tidak dapat menemukan root repositori (Cashflowpoly.sln).");
    // Menutup scope metode ResolveRepositoryRoot; bagian berikut berada di luar batas blok tersebut dalam ResolveRepositoryRoot.
    }

    // Mendefinisikan metode `AssertSqlContains` dengan hasil bertipe `void`; operasi ini menangani assert SQL contains. Masukan: Parameter `expected`
    // bertipe `string` membawa nilai yang diharapkan; Parameter `actual` bertipe `string` membawa nilai aktual.
    private static void AssertSqlContains(string expected, string actual)
    // Membuka scope metode AssertSqlContains; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AssertSqlContains.
    {
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `NormalizeSql(expected)`,
        // `NormalizeSql(actual)`, `StringComparison.OrdinalIgnoreCase` dalam AssertSqlContains.
        Assert.Contains(NormalizeSql(expected), NormalizeSql(actual), StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode AssertSqlContains; bagian berikut berada di luar batas blok tersebut dalam AssertSqlContains.
    }

    // Mendefinisikan metode `NormalizeSql` dengan hasil bertipe `string`; operasi ini menangani normalize SQL. Masukan: Parameter `sql` bertipe
    // `string` membawa nilai SQL.
    private static string NormalizeSql(string sql)
    // Membuka scope metode NormalizeSql; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam NormalizeSql.
    {
        // Menyiapkan variabel lokal `normalized` untuk nilai normalized dengan membersihkan karakter tepi pada `Regex.Replace(sql, @”\s+”, ” ”)` memakai
        // tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var normalized = Regex.Replace(sql, @"\s+", " ").Trim();
        // Memperbarui `normalized` menggunakan memanggil `Regex.Replace` dengan `normalized`, `@”\(\s+”`, `”(”` dalam NormalizeSql.
        normalized = Regex.Replace(normalized, @"\(\s+", "(");
        // Memperbarui `normalized` menggunakan memanggil `Regex.Replace` dengan `normalized`, `@”\s+\)”`, `”)”` dalam NormalizeSql.
        normalized = Regex.Replace(normalized, @"\s+\)", ")");
        // Mengembalikan `normalized` (nilai normalized) kepada pemanggil dalam NormalizeSql; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return normalized;
    // Menutup scope metode NormalizeSql; bagian berikut berada di luar batas blok tersebut dalam NormalizeSql.
    }

    // Mendefinisikan metode `ExtractCreateTable` dengan hasil bertipe `string`; operasi ini menangani extract create table. Masukan: Parameter `sql`
    // bertipe `string` membawa nilai SQL; Parameter `tableName` bertipe `string` membawa nilai table nama.
    private static string ExtractCreateTable(string sql, string tableName)
    // Membuka scope metode ExtractCreateTable; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ExtractCreateTable.
    {
        // Menyiapkan variabel lokal `match` untuk nilai match dengan memanggil `Regex.Match` dengan `sql`, `$@”create table if not exists
        // {Regex.Escape(tableName)}\s*\((?<body>.*?)\n\);”`, `RegexOptions.IgnoreCase | RegexOptions.Singleline`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var match = Regex.Match(
            // Meneruskan `sql` (nilai SQL) sebagai argumen ke `Regex.Match`.
            sql,
            // Meneruskan teks interpolasi `$@”create table if not exists {Regex.Escape(tableName)}\s*\((?<body>.*?)\n\);”`; nilai ekspresi di dalam kurung
            // kurawal disisipkan saat program berjalan sebagai argumen ke `Regex.Match`; Meneruskan `tableName` (nilai table nama) sebagai argumen ke
            // `Regex.Escape`.
            $@"create table if not exists {Regex.Escape(tableName)}\s*\((?<body>.*?)\n\);",
            // Meneruskan penggabungan flag atau operasi OR bit antara `RegexOptions.IgnoreCase` dan `RegexOptions.Singleline` sebagai argumen ke `Regex.Match`.
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        // Menjalankan pemeriksaan bahwa `match.Success`, `$”Table '{tableName}' should exist in canonical schema.”` bernilai benar; pengujian gagal jika
        // kondisi tidak terpenuhi dalam ExtractCreateTable.
        Assert.True(match.Success, $"Table '{tableName}' should exist in canonical schema.");
        // Mengembalikan `match.Value`, yaitu nilai yang dibungkus objek/nullable kepada pemanggil dalam ExtractCreateTable; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return match.Value;
    // Menutup scope metode ExtractCreateTable; bagian berikut berada di luar batas blok tersebut dalam ExtractCreateTable.
    }

    // Mendefinisikan metode `ExtractView` dengan hasil bertipe `string`; operasi ini menangani extract view. Masukan: Parameter `sql` bertipe `string`
    // membawa nilai SQL; Parameter `viewName` bertipe `string` membawa nilai view nama.
    private static string ExtractView(string sql, string viewName)
    // Membuka scope metode ExtractView; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ExtractView.
    {
        // Menyiapkan variabel lokal `match` untuk nilai match dengan memanggil `Regex.Match` dengan `sql`,
        // `$@”create\s+or\s+replace\s+view\s+{Regex.Escape(viewName)}\s+as(?<body>.*?);”`, `RegexOptions.IgnoreCase | RegexOptions.Singleline`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var match = Regex.Match(
            // Meneruskan `sql` (nilai SQL) sebagai argumen ke `Regex.Match`.
            sql,
            // Meneruskan teks interpolasi `$@”create\s+or\s+replace\s+view\s+{Regex.Escape(viewName)}\s+as(?<body>.*?);”`; nilai ekspresi di dalam kurung
            // kurawal disisipkan saat program berjalan sebagai argumen ke `Regex.Match`; Meneruskan `viewName` (nilai view nama) sebagai argumen ke
            // `Regex.Escape`.
            $@"create\s+or\s+replace\s+view\s+{Regex.Escape(viewName)}\s+as(?<body>.*?);",
            // Meneruskan penggabungan flag atau operasi OR bit antara `RegexOptions.IgnoreCase` dan `RegexOptions.Singleline` sebagai argumen ke `Regex.Match`.
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        // Menjalankan pemeriksaan bahwa `match.Success`, `$”View '{viewName}' should exist in canonical schema.”` bernilai benar; pengujian gagal jika
        // kondisi tidak terpenuhi dalam ExtractView.
        Assert.True(match.Success, $"View '{viewName}' should exist in canonical schema.");
        // Mengembalikan `match.Value`, yaitu nilai yang dibungkus objek/nullable kepada pemanggil dalam ExtractView; eksekusi jalur ini selesai setelah
        // nilai hasil ditentukan.
        return match.Value;
    // Menutup scope metode ExtractView; bagian berikut berada di luar batas blok tersebut dalam ExtractView.
    }

    // Mendefinisikan metode `ExtractFunction` dengan hasil bertipe `string`; operasi ini menangani extract function. Masukan: Parameter `sql` bertipe
    // `string` membawa nilai SQL; Parameter `functionName` bertipe `string` membawa nilai function nama.
    private static string ExtractFunction(string sql, string functionName)
    // Membuka scope metode ExtractFunction; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ExtractFunction.
    {
        // Menyiapkan variabel lokal `match` untuk nilai match dengan memanggil `Regex.Match` dengan `sql`,
        // `$@”create\s+or\s+replace\s+function\s+{Regex.Escape(functionName)}\b[\s\S]*?\n\$\$;”`, `RegexOptions.IgnoreCase`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var match = Regex.Match(
            // Meneruskan `sql` (nilai SQL) sebagai argumen ke `Regex.Match`.
            sql,
            // Meneruskan teks interpolasi `$@”create\s+or\s+replace\s+function\s+{Regex.Escape(functionName)}\b[\s\S]*?\n\$\$;”`; nilai ekspresi di dalam
            // kurung kurawal disisipkan saat program berjalan sebagai argumen ke `Regex.Match`; Meneruskan `functionName` (nilai function nama) sebagai argumen
            // ke `Regex.Escape`.
            $@"create\s+or\s+replace\s+function\s+{Regex.Escape(functionName)}\b[\s\S]*?\n\$\$;",
            // Meneruskan `RegexOptions.IgnoreCase` (nilai ignore case) sebagai argumen ke `Regex.Match`.
            RegexOptions.IgnoreCase);

        // Menjalankan pemeriksaan bahwa `match.Success`, `$”Function '{functionName}' should exist in canonical schema.”` bernilai benar; pengujian gagal
        // jika kondisi tidak terpenuhi dalam ExtractFunction.
        Assert.True(match.Success, $"Function '{functionName}' should exist in canonical schema.");
        // Mengembalikan `match.Value`, yaitu nilai yang dibungkus objek/nullable kepada pemanggil dalam ExtractFunction; eksekusi jalur ini selesai setelah
        // nilai hasil ditentukan.
        return match.Value;
    // Menutup scope metode ExtractFunction; bagian berikut berada di luar batas blok tersebut dalam ExtractFunction.
    }

    // Mendefinisikan metode `ParseSimulationSeedEvents` dengan hasil bertipe `IEnumerable<SimulationSeedEvent>`; operasi ini menangani parse simulation
    // seed event. Masukan: Parameter `seedContent` bertipe `string` membawa nilai seed content.
    private static IEnumerable<SimulationSeedEvent> ParseSimulationSeedEvents(string seedContent)
    // Membuka scope metode ParseSimulationSeedEvents; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ParseSimulationSeedEvents.
    {
        // Menyiapkan variabel lokal `pattern` untuk nilai pattern dengan nilai literal
        // `@”(?s)\(\s*'(?<mode>PEMULA|MAHIR)'\s*,\s*(?<ref>null|'[^']*')\s*,\s*(?<day>-?\d+)\s*,\s*(?<eventOrder>-?\d+)\s*,\s*(?<actionSlot>null|\d+)\s*,\s
        // *(?<playerNo>null|\d+)\s*,\s*'(...`. Tipe yang dipakai adalah `string`.
        const string pattern =
            // Menggunakan nilai literal `@”(?s)\(\s*'(?<mode>PEMULA|MAHIR)'\s*,\s*(?<ref>null|'[^']*')\s*,\s*(?<day>-?\d+)\s*,\s*(?<eventOrder>-?\d+)\s*,\s*(?<
            // actionSlot>null|\d+)\s*,\s*(?<playerNo>null|\d+)\s*,\s*'(...` sebagai bagian ekspresi yang sedang disusun dalam ParseSimulationSeedEvents.
            @"(?s)\(\s*'(?<mode>PEMULA|MAHIR)'\s*,\s*(?<ref>null|'[^']*')\s*,\s*(?<day>-?\d+)\s*,\s*(?<eventOrder>-?\d+)\s*,\s*(?<actionSlot>null|\d+)\s*,\s*(?<playerNo>null|\d+)\s*,\s*'(?<actor>[^']+)'\s*,\s*(?<actionId>null|'[^']*')\s*,\s*'(?<action>[^']+)'\s*,\s*'(?<payload>(?:''|[^'])*)' :: jsonb\s*\)";

        // Mengulangi setiap elemen `Regex.Matches(seedContent, pattern, RegexOptions.IgnoreCase)`; elemen saat ini disimpan sebagai `match` bertipe `Match`
        // untuk diproses oleh badan loop dalam ParseSimulationSeedEvents.
        foreach (Match match in Regex.Matches(seedContent, pattern, RegexOptions.IgnoreCase))
        // Membuka scope loop setiap match dari `Regex.Matches(seedContent, pattern, RegexOptions.IgnoreCase)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam ParseSimulationSeedEvents.
        {
            // Melengkapi struktur ekspresi YieldReturnStatement melalui yield return new SimulationSeedEvent( dalam ParseSimulationSeedEvents; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            yield return new SimulationSeedEvent(
                // Meneruskan menormalisasi `match.Groups[”mode”].Value` menjadi huruf besar dengan aturan kultur invariant sebagai argumen ke konstruktor
                // `SimulationSeedEvent`; Meneruskan nilai literal `”mode”` sebagai argumen ke `match.Groups[”mode”].Value.ToUpperInvariant`.
                match.Groups["mode"].Value.ToUpperInvariant(),
                // Meneruskan hasil pemilihan bersyarat: ketika `match.Groups[”ref”].Value.Equals(”null”, StringComparison.OrdinalIgnoreCase)` benar gunakan `null`,
                // jika tidak gunakan `match.Groups[”ref”].Value.Trim('\'')` sebagai argumen ke konstruktor `SimulationSeedEvent`; Meneruskan nilai literal `”ref”`
                // sebagai argumen ke `match.Groups[”ref”].Value.Equals`; Meneruskan nilai literal `”null”` sebagai argumen ke `match.Groups[”ref”].Value.Equals`;
                // Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `match.Groups[”ref”].Value.Equals`.
                match.Groups["ref"].Value.Equals("null", StringComparison.OrdinalIgnoreCase)
                    // Meneruskan hasil pemilihan bersyarat: ketika `match.Groups[”ref”].Value.Equals(”null”, StringComparison.OrdinalIgnoreCase)` benar gunakan `null`,
                    // jika tidak gunakan `match.Groups[”ref”].Value.Trim('\'')` sebagai argumen ke konstruktor `SimulationSeedEvent`.
                    ? null
                    // Meneruskan nilai literal `”ref”` sebagai argumen ke `match.Groups[”ref”].Value.Trim`; Meneruskan nilai literal `'\''` sebagai argumen ke
                    // `match.Groups[”ref”].Value.Trim`.
                    : match.Groups["ref"].Value.Trim('\''),
                // Meneruskan memanggil `int.Parse` dengan `match.Groups[”day”].Value` sebagai argumen ke konstruktor `SimulationSeedEvent`; Meneruskan
                // `match.Groups[”day”].Value`, yaitu nilai yang dibungkus objek/nullable sebagai argumen ke `int.Parse`; Meneruskan nilai literal `”day”` sebagai
                // argumen ke `int.Parse`.
                int.Parse(match.Groups["day"].Value),
                // Meneruskan memanggil `int.Parse` dengan `match.Groups[”eventOrder”].Value` sebagai argumen ke konstruktor `SimulationSeedEvent`; Meneruskan
                // `match.Groups[”eventOrder”].Value`, yaitu nilai yang dibungkus objek/nullable sebagai argumen ke `int.Parse`; Meneruskan nilai literal
                // `”eventOrder”` sebagai argumen ke `int.Parse`.
                int.Parse(match.Groups["eventOrder"].Value),
                // Meneruskan hasil pemilihan bersyarat: ketika `match.Groups[”playerNo”].Value.Equals(”null”, StringComparison.OrdinalIgnoreCase)` benar gunakan
                // `null`, jika tidak gunakan `int.Parse(match.Groups[”playerNo”].Value)` sebagai argumen ke konstruktor `SimulationSeedEvent`; Meneruskan nilai
                // literal `”playerNo”` sebagai argumen ke `match.Groups[”playerNo”].Value.Equals`; Meneruskan nilai literal `”null”` sebagai argumen ke
                // `match.Groups[”playerNo”].Value.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke
                // `match.Groups[”playerNo”].Value.Equals`.
                match.Groups["playerNo"].Value.Equals("null", StringComparison.OrdinalIgnoreCase)
                    // Meneruskan hasil pemilihan bersyarat: ketika `match.Groups[”playerNo”].Value.Equals(”null”, StringComparison.OrdinalIgnoreCase)` benar gunakan
                    // `null`, jika tidak gunakan `int.Parse(match.Groups[”playerNo”].Value)` sebagai argumen ke konstruktor `SimulationSeedEvent`.
                    ? null
                    // Meneruskan `match.Groups[”playerNo”].Value`, yaitu nilai yang dibungkus objek/nullable sebagai argumen ke `int.Parse`; Meneruskan nilai literal
                    // `”playerNo”` sebagai argumen ke `int.Parse`.
                    : int.Parse(match.Groups["playerNo"].Value),
                // Meneruskan menormalisasi `match.Groups[”actor”].Value` menjadi huruf besar dengan aturan kultur invariant sebagai argumen ke konstruktor
                // `SimulationSeedEvent`; Meneruskan nilai literal `”actor”` sebagai argumen ke `match.Groups[”actor”].Value.ToUpperInvariant`.
                match.Groups["actor"].Value.ToUpperInvariant(),
                // Meneruskan `match.Groups[”action”].Value`, yaitu nilai yang dibungkus objek/nullable sebagai argumen ke konstruktor `SimulationSeedEvent`;
                // Meneruskan nilai literal `”action”` sebagai argumen ke konstruktor `SimulationSeedEvent`.
                match.Groups["action"].Value);
        // Menutup scope loop setiap match dari `Regex.Matches(seedContent, pattern, RegexOptions.IgnoreCase)`; bagian berikut berada di luar batas blok
        // tersebut dalam ParseSimulationSeedEvents.
        }
    // Menutup scope metode ParseSimulationSeedEvents; bagian berikut berada di luar batas blok tersebut dalam ParseSimulationSeedEvents.
    }

    // Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `SimulationSeedEvent`; sealed mencegah tipe ini diturunkan lagi.
    private sealed record SimulationSeedEvent(
        // Parameter `Mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan.
        string Mode,
        // Parameter `RefKey` bertipe `string?` membawa nilai ref kunci; nilai null diizinkan ketika data opsional belum tersedia.
        string? RefKey,
        // Parameter `DayIndex` bertipe `int` membawa nilai hari index.
        int DayIndex,
        // Parameter `EventOrder` bertipe `int` membawa nilai event urutan/pesanan.
        int EventOrder,
        // Parameter `PlayerNo` bertipe `int?` membawa nilai pemain no; nilai null diizinkan ketika data opsional belum tersedia.
        int? PlayerNo,
        // Parameter `ActorType` bertipe `string` membawa nilai actor jenis.
        string ActorType,
        // Parameter `ActionType` bertipe `string` membawa nilai aksi jenis.
        string ActionType);
// Menutup scope tipe BootstrapAssetConsistencyTests; bagian berikut berada di luar batas blok tersebut.
}
