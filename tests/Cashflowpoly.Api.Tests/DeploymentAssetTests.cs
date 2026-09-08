// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui DeploymentAssetTests.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `DeploymentAssetTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class DeploymentAssetTests
// Membuka scope tipe DeploymentAssetTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `RepoRoot` menyimpan nilai repo root dengan nilai awal memanggil `ResolveRepositoryRoot` dengan tanpa
    // argumen. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan
    // dibagikan antar instance.
    private static readonly string RepoRoot = ResolveRepositoryRoot();

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `DockerCompose_DoesNotMountDatabaseInitScripts` dengan hasil bertipe `void`; operasi ini menangani docker compose does not
    // mount database init scripts.
    public void DockerCompose_DoesNotMountDatabaseInitScripts()
    // Membuka scope metode DockerCompose_DoesNotMountDatabaseInitScripts; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DockerCompose_DoesNotMountDatabaseInitScripts.
    {
        // Menyiapkan variabel lokal `composePath` untuk nilai compose path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”infra”`, `”docker”`,
        // `”docker-compose.yml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var composePath = Path.Combine(RepoRoot, "infra", "docker", "docker-compose.yml");

        // Menjalankan pemeriksaan bahwa `File.Exists(composePath)`, `”docker-compose.yml harus tersedia pada infra/docker.”` bernilai benar; pengujian
        // gagal jika kondisi tidak terpenuhi dalam DockerCompose_DoesNotMountDatabaseInitScripts.
        Assert.True(File.Exists(composePath), "docker-compose.yml harus tersedia pada infra/docker.");

        // Menyiapkan variabel lokal `content` untuk nilai content dengan memanggil `File.ReadAllText` dengan `composePath`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var content = File.ReadAllText(composePath);

        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”/docker-entrypoint-initdb.d”`,
        // `content`, `StringComparison.Ordinal` dalam DockerCompose_DoesNotMountDatabaseInitScripts.
        Assert.DoesNotContain("/docker-entrypoint-initdb.d", content, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”./database:/docker-entrypoint-initdb.d:ro”`, `content`, `StringComparison.Ordinal` dalam DockerCompose_DoesNotMountDatabaseInitScripts.
        Assert.DoesNotContain("./database:/docker-entrypoint-initdb.d:ro", content, StringComparison.Ordinal);
    // Menutup scope metode DockerCompose_DoesNotMountDatabaseInitScripts; bagian berikut berada di luar batas blok tersebut dalam
    // DockerCompose_DoesNotMountDatabaseInitScripts.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `DockerComposeWatch_UsesInlineUiWatcherCommand` dengan hasil bertipe `void`; operasi ini menangani docker compose watch
    // uses inline ui watcher command.
    public void DockerComposeWatch_UsesInlineUiWatcherCommand()
    // Membuka scope metode DockerComposeWatch_UsesInlineUiWatcherCommand; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DockerComposeWatch_UsesInlineUiWatcherCommand.
    {
        // Menyiapkan variabel lokal `composePath` untuk nilai compose path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”infra”`, `”docker”`,
        // `”docker-compose.watch.yml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var composePath = Path.Combine(RepoRoot, "infra", "docker", "docker-compose.watch.yml");

        // Menjalankan pemeriksaan bahwa `File.Exists(composePath)`, `”docker-compose.watch.yml harus tersedia pada infra/docker.”` bernilai benar;
        // pengujian gagal jika kondisi tidak terpenuhi dalam DockerComposeWatch_UsesInlineUiWatcherCommand.
        Assert.True(File.Exists(composePath), "docker-compose.watch.yml harus tersedia pada infra/docker.");

        // Menyiapkan variabel lokal `content` untuk nilai content dengan memanggil `File.ReadAllText` dengan `composePath`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var content = File.ReadAllText(composePath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”npm ci --no-audit --no-fund”`,
        // `content`, `StringComparison.Ordinal` dalam DockerComposeWatch_UsesInlineUiWatcherCommand.
        Assert.Contains("npm ci --no-audit --no-fund", content, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”npm run tailwind:watch”`, `content`,
        // `StringComparison.Ordinal` dalam DockerComposeWatch_UsesInlineUiWatcherCommand.
        Assert.Contains("npm run tailwind:watch", content, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”dotnet watch --non-interactive
        // --project src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj run --no-launch-profile”`, `content`, `StringComparison.Ordinal` dalam
        // DockerComposeWatch_UsesInlineUiWatcherCommand.
        Assert.Contains(
            // Meneruskan nilai literal `”dotnet watch --non-interactive --project src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj run --no-launch-profile”` sebagai
            // argumen ke `Assert.Contains`.
            "dotnet watch --non-interactive --project src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj run --no-launch-profile",
            // Meneruskan `content` (nilai content) sebagai argumen ke `Assert.Contains`.
            content,
            // Meneruskan `StringComparison.Ordinal` (nilai ordinal) sebagai argumen ke `Assert.Contains`.
            StringComparison.Ordinal);
    // Menutup scope metode DockerComposeWatch_UsesInlineUiWatcherCommand; bagian berikut berada di luar batas blok tersebut dalam
    // DockerComposeWatch_UsesInlineUiWatcherCommand.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ApiAndUiProjects_DoNotReferenceSharedContractsProject` dengan hasil bertipe `void`; operasi ini menangani api dan ui
    // projects do not reference shared contracts project.
    public void ApiAndUiProjects_DoNotReferenceSharedContractsProject()
    // Membuka scope metode ApiAndUiProjects_DoNotReferenceSharedContractsProject; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ApiAndUiProjects_DoNotReferenceSharedContractsProject.
    {
        // Menyiapkan variabel lokal `apiProjectPath` untuk nilai api project path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`,
        // `”Cashflowpoly.Api”`, `”Cashflowpoly.Api.csproj”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var apiProjectPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Cashflowpoly.Api.csproj");
        // Menyiapkan variabel lokal `uiProjectPath` untuk nilai ui project path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`,
        // `”Cashflowpoly.Ui”`, `”Cashflowpoly.Ui.csproj”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var uiProjectPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Cashflowpoly.Ui.csproj");
        // Menyiapkan variabel lokal `apiDockerfilePath` untuk nilai api dockerfile path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`,
        // `”Cashflowpoly.Api”`, `”Dockerfile”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var apiDockerfilePath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Dockerfile");
        // Menyiapkan variabel lokal `uiDockerfilePath` untuk nilai ui dockerfile path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`,
        // `”Cashflowpoly.Ui”`, `”Dockerfile”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var uiDockerfilePath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Dockerfile");
        // Menyiapkan variabel lokal `contractsProjectPath` untuk nilai contracts project path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`,
        // `”Cashflowpoly.Contracts”`, `”Cashflowpoly.Contracts.csproj”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var contractsProjectPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Contracts", "Cashflowpoly.Contracts.csproj");

        // Menjalankan pemeriksaan bahwa `File.Exists(apiProjectPath)`, `”Project API harus tersedia.”` bernilai benar; pengujian gagal jika kondisi tidak
        // terpenuhi dalam ApiAndUiProjects_DoNotReferenceSharedContractsProject.
        Assert.True(File.Exists(apiProjectPath), "Project API harus tersedia.");
        // Menjalankan pemeriksaan bahwa `File.Exists(uiProjectPath)`, `”Project UI harus tersedia.”` bernilai benar; pengujian gagal jika kondisi tidak
        // terpenuhi dalam ApiAndUiProjects_DoNotReferenceSharedContractsProject.
        Assert.True(File.Exists(uiProjectPath), "Project UI harus tersedia.");
        // Menjalankan pemeriksaan bahwa `File.Exists(apiDockerfilePath)`, `”Dockerfile API harus tersedia.”` bernilai benar; pengujian gagal jika kondisi
        // tidak terpenuhi dalam ApiAndUiProjects_DoNotReferenceSharedContractsProject.
        Assert.True(File.Exists(apiDockerfilePath), "Dockerfile API harus tersedia.");
        // Menjalankan pemeriksaan bahwa `File.Exists(uiDockerfilePath)`, `”Dockerfile UI harus tersedia.”` bernilai benar; pengujian gagal jika kondisi
        // tidak terpenuhi dalam ApiAndUiProjects_DoNotReferenceSharedContractsProject.
        Assert.True(File.Exists(uiDockerfilePath), "Dockerfile UI harus tersedia.");

        // Menyiapkan variabel lokal `apiProject` untuk nilai api project dengan memanggil `File.ReadAllText` dengan `apiProjectPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var apiProject = File.ReadAllText(apiProjectPath);
        // Menyiapkan variabel lokal `uiProject` untuk nilai ui project dengan memanggil `File.ReadAllText` dengan `uiProjectPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var uiProject = File.ReadAllText(uiProjectPath);
        // Menyiapkan variabel lokal `apiDockerfile` untuk nilai api dockerfile dengan memanggil `File.ReadAllText` dengan `apiDockerfilePath`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var apiDockerfile = File.ReadAllText(apiDockerfilePath);
        // Menyiapkan variabel lokal `uiDockerfile` untuk nilai ui dockerfile dengan memanggil `File.ReadAllText` dengan `uiDockerfilePath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var uiDockerfile = File.ReadAllText(uiDockerfilePath);

        // Menjalankan pemeriksaan bahwa `File.Exists(contractsProjectPath)`, `”Project Contracts terpisah tidak digunakan lagi.”` bernilai salah; pengujian
        // gagal jika kondisi justru terpenuhi dalam ApiAndUiProjects_DoNotReferenceSharedContractsProject.
        Assert.False(File.Exists(contractsProjectPath), "Project Contracts terpisah tidak digunakan lagi.");
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Cashflowpoly.Contracts”`,
        // `apiProject`, `StringComparison.Ordinal` dalam ApiAndUiProjects_DoNotReferenceSharedContractsProject.
        Assert.DoesNotContain("Cashflowpoly.Contracts", apiProject, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Cashflowpoly.Contracts”`,
        // `uiProject`, `StringComparison.Ordinal` dalam ApiAndUiProjects_DoNotReferenceSharedContractsProject.
        Assert.DoesNotContain("Cashflowpoly.Contracts", uiProject, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Cashflowpoly.Contracts”`,
        // `apiDockerfile`, `StringComparison.Ordinal` dalam ApiAndUiProjects_DoNotReferenceSharedContractsProject.
        Assert.DoesNotContain("Cashflowpoly.Contracts", apiDockerfile, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Cashflowpoly.Contracts”`,
        // `uiDockerfile`, `StringComparison.Ordinal` dalam ApiAndUiProjects_DoNotReferenceSharedContractsProject.
        Assert.DoesNotContain("Cashflowpoly.Contracts", uiDockerfile, StringComparison.Ordinal);
    // Menutup scope metode ApiAndUiProjects_DoNotReferenceSharedContractsProject; bagian berikut berada di luar batas blok tersebut dalam
    // ApiAndUiProjects_DoNotReferenceSharedContractsProject.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Readme_DoesNotReferenceRemovedInspectionSeedOrDemoCredentials` dengan hasil bertipe `void`; operasi ini menangani readme
    // does not reference removed inspection seed atau demo credentials.
    public void Readme_DoesNotReferenceRemovedInspectionSeedOrDemoCredentials()
    // Membuka scope metode Readme_DoesNotReferenceRemovedInspectionSeedOrDemoCredentials; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam Readme_DoesNotReferenceRemovedInspectionSeedOrDemoCredentials.
    {
        // Menyiapkan variabel lokal `readmePath` untuk nilai readme path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”README.md”`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var readmePath = Path.Combine(RepoRoot, "README.md");

        // Menjalankan pemeriksaan bahwa `File.Exists(readmePath)`, `”README.md harus tersedia pada root repositori.”` bernilai benar; pengujian gagal jika
        // kondisi tidak terpenuhi dalam Readme_DoesNotReferenceRemovedInspectionSeedOrDemoCredentials.
        Assert.True(File.Exists(readmePath), "README.md harus tersedia pada root repositori.");

        // Menyiapkan variabel lokal `readme` untuk nilai readme dengan memanggil `File.ReadAllText` dengan `readmePath`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var readme = File.ReadAllText(readmePath);

        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”02_seed_full_inspection.sql”`,
        // `readme`, `StringComparison.Ordinal` dalam Readme_DoesNotReferenceRemovedInspectionSeedOrDemoCredentials.
        Assert.DoesNotContain("02_seed_full_inspection.sql", readme, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”mira.hartanto”`, `readme`,
        // `StringComparison.Ordinal` dalam Readme_DoesNotReferenceRemovedInspectionSeedOrDemoCredentials.
        Assert.DoesNotContain("mira.hartanto", readme, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”MiraAudit!2026”`, `readme`,
        // `StringComparison.Ordinal` dalam Readme_DoesNotReferenceRemovedInspectionSeedOrDemoCredentials.
        Assert.DoesNotContain("MiraAudit!2026", readme, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ulfa.ramadhani”`, `readme`,
        // `StringComparison.Ordinal` dalam Readme_DoesNotReferenceRemovedInspectionSeedOrDemoCredentials.
        Assert.DoesNotContain("ulfa.ramadhani", readme, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”UlfaAudit!2026”`, `readme`,
        // `StringComparison.Ordinal` dalam Readme_DoesNotReferenceRemovedInspectionSeedOrDemoCredentials.
        Assert.DoesNotContain("UlfaAudit!2026", readme, StringComparison.Ordinal);
    // Menutup scope metode Readme_DoesNotReferenceRemovedInspectionSeedOrDemoCredentials; bagian berikut berada di luar batas blok tersebut dalam
    // Readme_DoesNotReferenceRemovedInspectionSeedOrDemoCredentials.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Readme_DescribesVersionedSqlMigrations_InsteadOfEfMigrationStartup` dengan hasil bertipe `void`; operasi ini menangani
    // readme describes versioned SQL migrations instead of ef migration startup.
    public void Readme_DescribesVersionedSqlMigrations_InsteadOfEfMigrationStartup()
    // Membuka scope metode Readme_DescribesVersionedSqlMigrations_InsteadOfEfMigrationStartup; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam Readme_DescribesVersionedSqlMigrations_InsteadOfEfMigrationStartup.
    {
        // Menyiapkan variabel lokal `readmePath` untuk nilai readme path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”README.md”`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var readmePath = Path.Combine(RepoRoot, "README.md");

        // Menjalankan pemeriksaan bahwa `File.Exists(readmePath)`, `”README.md harus tersedia pada root repositori.”` bernilai benar; pengujian gagal jika
        // kondisi tidak terpenuhi dalam Readme_DescribesVersionedSqlMigrations_InsteadOfEfMigrationStartup.
        Assert.True(File.Exists(readmePath), "README.md harus tersedia pada root repositori.");

        // Menyiapkan variabel lokal `readme` untuk nilai readme dengan memanggil `File.ReadAllText` dengan `readmePath`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var readme = File.ReadAllText(readmePath);

        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”migration EF”`, `readme`,
        // `StringComparison.OrdinalIgnoreCase` dalam Readme_DescribesVersionedSqlMigrations_InsteadOfEfMigrationStartup.
        Assert.DoesNotContain("migration EF", readme, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”baseline SQL lalu migrasi berurutan”`,
        // `readme`, `StringComparison.OrdinalIgnoreCase` dalam Readme_DescribesVersionedSqlMigrations_InsteadOfEfMigrationStartup.
        Assert.Contains("baseline SQL lalu migrasi berurutan", readme, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”schema_history”`, `readme`,
        // `StringComparison.Ordinal` dalam Readme_DescribesVersionedSqlMigrations_InsteadOfEfMigrationStartup.
        Assert.Contains("schema_history", readme, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”--migrate-only”`, `readme`,
        // `StringComparison.Ordinal` dalam Readme_DescribesVersionedSqlMigrations_InsteadOfEfMigrationStartup.
        Assert.Contains("--migrate-only", readme, StringComparison.Ordinal);
    // Menutup scope metode Readme_DescribesVersionedSqlMigrations_InsteadOfEfMigrationStartup; bagian berikut berada di luar batas blok tersebut dalam
    // Readme_DescribesVersionedSqlMigrations_InsteadOfEfMigrationStartup.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract` dengan hasil bertipe `void`; operasi ini menangani postman
    // dan documentation should match saat ini auth dan api contract.
    public void PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract()
    // Membuka scope metode PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract.
    {
        // Menyiapkan variabel lokal `collection` untuk nilai collection dengan memanggil `File.ReadAllText` dengan `Path.Combine(RepoRoot, ”postman”,
        // ”Cashflowpoly.postman_collection.json”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var collection = File.ReadAllText(Path.Combine(RepoRoot, "postman", "Cashflowpoly.postman_collection.json"));
        // Menyiapkan variabel lokal `environment` untuk nilai environment dengan memanggil `File.ReadAllText` dengan `Path.Combine(RepoRoot, ”postman”,
        // ”Cashflowpoly.local.postman_environment.json”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var environment = File.ReadAllText(Path.Combine(RepoRoot, "postman", "Cashflowpoly.local.postman_environment.json"));
        // Menyiapkan variabel lokal `readme` untuk nilai readme dengan memanggil `File.ReadAllText` dengan `Path.Combine(RepoRoot, ”README.md”)`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var readme = File.ReadAllText(Path.Combine(RepoRoot, "README.md"));
        // Menyiapkan variabel lokal `docsIndex` untuk nilai docs index dengan memanggil `File.ReadAllText` dengan `Path.Combine(RepoRoot, ”docs”,
        // ”README.md”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var docsIndex = File.ReadAllText(Path.Combine(RepoRoot, "docs", "README.md"));

        // Menyiapkan variabel lokal `collectionJson` untuk nilai collection JSON dengan memanggil `JsonDocument.Parse` dengan `collection`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var collectionJson = JsonDocument.Parse(collection);
        // Menyiapkan variabel lokal `environmentJson` untuk nilai environment JSON dengan memanggil `JsonDocument.Parse` dengan `environment`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var environmentJson = JsonDocument.Parse(environment);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”26 Agustus 2026”`, `collection`,
        // `StringComparison.Ordinal` dalam PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract.
        Assert.Contains("26 Agustus 2026", collection, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Retry Session Setup”`, `collection`,
        // `StringComparison.Ordinal` dalam PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract.
        Assert.Contains("Retry Session Setup", collection, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”?limit=50”`, `collection`,
        // `StringComparison.Ordinal` dalam PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract.
        Assert.Contains("?limit=50", collection, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”fromSeq”`, `collection`,
        // `StringComparison.Ordinal` dalam PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract.
        Assert.DoesNotContain("fromSeq", collection, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Verify Public Instructor Registration
        // Is Rejected”`, `collection`, `StringComparison.Ordinal` dalam PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract.
        Assert.Contains("Verify Public Instructor Registration Is Rejected", collection, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'Login': [200]”`, `collection`,
        // `StringComparison.Ordinal` dalam PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract.
        Assert.Contains("'Login': [200]", collection, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'Login': [200, 401]”`,
        // `collection`, `StringComparison.Ordinal` dalam PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract.
        Assert.DoesNotContain("'Login': [200, 401]", collection, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”protectedRequests”`,
        // `collection`, `StringComparison.Ordinal` dalam PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract.
        Assert.DoesNotContain("protectedRequests", collection, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”List Session Players”`, `collection`,
        // `StringComparison.Ordinal` dalam PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract.
        Assert.Contains("List Session Players", collection, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”/api/v1/sessions/{{sessionId}}/players”`, `collection`, `StringComparison.Ordinal` dalam
        // PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract.
        Assert.Contains("/api/v1/sessions/{{sessionId}}/players", collection, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”deniedInstructorUsername”`,
        // `environment`, `StringComparison.Ordinal` dalam PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract.
        Assert.Contains("deniedInstructorUsername", environment, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”skema database **`3.0.13`**”`,
        // `readme`, `StringComparison.Ordinal` dalam PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract.
        Assert.Contains("skema database **`3.0.13`**", readme, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”file:///”`, `docsIndex`,
        // `StringComparison.OrdinalIgnoreCase` dalam PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract.
        Assert.DoesNotContain("file:///", docsIndex, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract; bagian berikut berada di luar batas blok tersebut dalam
    // PostmanAndDocumentation_ShouldMatchCurrentAuthAndApiContract.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ProductionAssets_ShouldValidateBootstrapAndNotPublishSwagger` dengan hasil bertipe `void`; operasi ini menangani
    // production aset should validate bootstrap dan not publish swagger.
    public void ProductionAssets_ShouldValidateBootstrapAndNotPublishSwagger()
    // Membuka scope metode ProductionAssets_ShouldValidateBootstrapAndNotPublishSwagger; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam ProductionAssets_ShouldValidateBootstrapAndNotPublishSwagger.
    {
        // Menyiapkan variabel lokal `prodEnvironment` untuk nilai prod environment dengan memanggil `File.ReadAllText` dengan `Path.Combine(RepoRoot,
        // ”config”, ”env”, ”.env.prod.example”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var prodEnvironment = File.ReadAllText(Path.Combine(RepoRoot, "config", "env", ".env.prod.example"));
        // Menyiapkan variabel lokal `readinessScript` untuk nilai readiness script dengan memanggil `File.ReadAllText` dengan `Path.Combine(RepoRoot,
        // ”scripts”, ”Test-ProductionReadiness.ps1”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var readinessScript = File.ReadAllText(Path.Combine(RepoRoot, "scripts", "Test-ProductionReadiness.ps1"));
        // Menyiapkan variabel lokal `nginx` untuk nilai nginx dengan memanggil `File.ReadAllText` dengan `Path.Combine(RepoRoot, ”infra”, ”nginx”,
        // ”default.conf”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var nginx = File.ReadAllText(Path.Combine(RepoRoot, "infra", "nginx", "default.conf"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”AUTH_BOOTSTRAP_SEED_DEFAULT_USERS=false”`, `prodEnvironment`, `StringComparison.Ordinal` dalam
        // ProductionAssets_ShouldValidateBootstrapAndNotPublishSwagger.
        Assert.Contains("AUTH_BOOTSTRAP_SEED_DEFAULT_USERS=false", prodEnvironment, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”AUTH_BOOTSTRAP_INSTRUCTOR_USERNAME=”`,
        // `prodEnvironment`, `StringComparison.Ordinal` dalam ProductionAssets_ShouldValidateBootstrapAndNotPublishSwagger.
        Assert.Contains("AUTH_BOOTSTRAP_INSTRUCTOR_USERNAME=", prodEnvironment, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Test-BootstrapCredentialPair”`,
        // `readinessScript`, `StringComparison.Ordinal` dalam ProductionAssets_ShouldValidateBootstrapAndNotPublishSwagger.
        Assert.Contains("Test-BootstrapCredentialPair", readinessScript, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”UTF8.GetByteCount”`, `readinessScript`,
        // `StringComparison.Ordinal` dalam ProductionAssets_ShouldValidateBootstrapAndNotPublishSwagger.
        Assert.Contains("UTF8.GetByteCount", readinessScript, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”location /swagger”`, `nginx`,
        // `StringComparison.OrdinalIgnoreCase` dalam ProductionAssets_ShouldValidateBootstrapAndNotPublishSwagger.
        Assert.DoesNotContain("location /swagger", nginx, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”^/swagger/”`, `nginx`,
        // `StringComparison.OrdinalIgnoreCase` dalam ProductionAssets_ShouldValidateBootstrapAndNotPublishSwagger.
        Assert.DoesNotContain("^/swagger/", nginx, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode ProductionAssets_ShouldValidateBootstrapAndNotPublishSwagger; bagian berikut berada di luar batas blok tersebut dalam
    // ProductionAssets_ShouldValidateBootstrapAndNotPublishSwagger.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate` dengan hasil bertipe `void`; operasi ini menangani
    // production containers should be pinned non root dan keep metrics private.
    public void ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate()
    // Membuka scope metode ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
    {
        // Menyiapkan variabel lokal `compose` untuk nilai compose dengan memanggil `File.ReadAllText` dengan `Path.Combine(RepoRoot, ”infra”, ”docker”,
        // ”docker-compose.yml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var compose = File.ReadAllText(Path.Combine(RepoRoot, "infra", "docker", "docker-compose.yml"));
        // Menyiapkan variabel lokal `production` untuk nilai production dengan memanggil `File.ReadAllText` dengan `Path.Combine(RepoRoot, ”infra”,
        // ”docker”, ”docker-compose.prod.yml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var production = File.ReadAllText(Path.Combine(RepoRoot, "infra", "docker", "docker-compose.prod.yml"));
        // Menyiapkan variabel lokal `apiDockerfile` untuk nilai api dockerfile dengan memanggil `File.ReadAllText` dengan `Path.Combine(RepoRoot, ”src”,
        // ”Cashflowpoly.Api”, ”Dockerfile”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var apiDockerfile = File.ReadAllText(Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Dockerfile"));
        // Menyiapkan variabel lokal `uiDockerfile` untuk nilai ui dockerfile dengan memanggil `File.ReadAllText` dengan `Path.Combine(RepoRoot, ”src”,
        // ”Cashflowpoly.Ui”, ”Dockerfile”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var uiDockerfile = File.ReadAllText(Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Dockerfile"));
        // Menyiapkan variabel lokal `nginx` untuk nilai nginx dengan memanggil `File.ReadAllText` dengan `Path.Combine(RepoRoot, ”infra”, ”nginx”,
        // ”default.conf”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var nginx = File.ReadAllText(Path.Combine(RepoRoot, "infra", "nginx", "default.conf"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”postgres:16.15”`, `compose`,
        // `StringComparison.Ordinal` dalam ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("postgres:16.15", compose, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”nginxinc/nginx-unprivileged:1.31.3-alpine”`, `production`, `StringComparison.Ordinal` dalam
        // ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("nginxinc/nginx-unprivileged:1.31.3-alpine", production, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”cloudflare/cloudflared:2026.8.1”`,
        // `production`, `StringComparison.Ordinal` dalam ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("cloudflare/cloudflared:2026.8.1", production, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”:latest”`, `production`,
        // `StringComparison.Ordinal` dalam ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.DoesNotContain(":latest", production, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”USER app”`, `apiDockerfile`,
        // `StringComparison.Ordinal` dalam ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("USER app", apiDockerfile, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”USER app”`, `uiDockerfile`,
        // `StringComparison.Ordinal` dalam ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("USER app", uiDockerfile, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”chown -R app:app /home/app/.aspnet”`,
        // `apiDockerfile`, `StringComparison.Ordinal` dalam ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("chown -R app:app /home/app/.aspnet", apiDockerfile, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”chown -R app:app /home/app/.aspnet”`,
        // `uiDockerfile`, `StringComparison.Ordinal` dalam ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("chown -R app:app /home/app/.aspnet", uiDockerfile, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”mcr.microsoft.com/dotnet/aspnet:10.0.4”`, `apiDockerfile`, `StringComparison.Ordinal` dalam
        // ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("mcr.microsoft.com/dotnet/aspnet:10.0.4", apiDockerfile, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”mcr.microsoft.com/dotnet/aspnet:10.0.4”`, `uiDockerfile`, `StringComparison.Ordinal` dalam
        // ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("mcr.microsoft.com/dotnet/aspnet:10.0.4", uiDockerfile, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”driver: ${LOG_DRIVER:-journald}”`,
        // `production`, `StringComparison.Ordinal` dalam ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("driver: ${LOG_DRIVER:-journald}", production, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”no-new-privileges:true”`, `production`,
        // `StringComparison.Ordinal` dalam ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("no-new-privileges:true", production, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”location = /metrics”`, `nginx`,
        // `StringComparison.Ordinal` dalam ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("location = /metrics", nginx, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”return 404;”`, `nginx`,
        // `StringComparison.Ordinal` dalam ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("return 404;", nginx, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”listen 8080;”`, `nginx`,
        // `StringComparison.Ordinal` dalam ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("listen 8080;", nginx, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”proxy_set_header Host ui;”`, `nginx`,
        // `StringComparison.Ordinal` dalam ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("proxy_set_header Host ui;", nginx, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”127.0.0.1:80:8080”`, `production`,
        // `StringComparison.Ordinal` dalam ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("127.0.0.1:80:8080", production, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”http://127.0.0.1:8080/health”`,
        // `production`, `StringComparison.Ordinal` dalam ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
        Assert.Contains("http://127.0.0.1:8080/health", production, StringComparison.Ordinal);
    // Menutup scope metode ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate; bagian berikut berada di luar batas blok tersebut dalam
    // ProductionContainers_ShouldBePinnedNonRootAndKeepMetricsPrivate.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly` dengan hasil bertipe `void`; operasi ini menangani
    // deployment script should lock migrate recalculate dan rollback images only.
    public void DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly()
    // Membuka scope metode DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly.
    {
        // Menyiapkan variabel lokal `script` untuk nilai script dengan memanggil `File.ReadAllText` dengan `Path.Combine(RepoRoot, ”scripts”,
        // ”deploy-production.sh”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var script = File.ReadAllText(Path.Combine(RepoRoot, "scripts", "deploy-production.sh"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”flock -n”`, `script`,
        // `StringComparison.Ordinal` dalam DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly.
        Assert.Contains("flock -n", script, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”--migrate-only”`, `script`,
        // `StringComparison.Ordinal` dalam DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly.
        Assert.Contains("--migrate-only", script, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”--recalculate-analytics”`, `script`,
        // `StringComparison.Ordinal` dalam DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly.
        Assert.Contains("--recalculate-analytics", script, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”Host: ${DOMAIN_HOST}”`, `script`,
        // `StringComparison.Ordinal` dalam DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly.
        Assert.Contains("Host: ${DOMAIN_HOST}", script, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rollback_images”`, `script`,
        // `StringComparison.Ordinal` dalam DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly.
        Assert.Contains("rollback_images", script, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”PREVIOUS_SHA”`, `script`,
        // `StringComparison.Ordinal` dalam DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly.
        Assert.Contains("PREVIOUS_SHA", script, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”COMPOSE_PROJECT_NAME=${COMPOSE_PROJECT_NAME:-cashflowpoly-analytics-platform}”`, `script`, `StringComparison.Ordinal` dalam
        // DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly.
        Assert.Contains("COMPOSE_PROJECT_NAME=${COMPOSE_PROJECT_NAME:-cashflowpoly-analytics-platform}", script, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”--project-name cashflowpoly\n”`,
        // `script`, `StringComparison.Ordinal` dalam DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly.
        Assert.DoesNotContain("--project-name cashflowpoly\n", script, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”up -d --no-recreate db”`, `script`,
        // `StringComparison.Ordinal` dalam DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly.
        Assert.Contains("up -d --no-recreate db", script, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”up -d --no-build --force-recreate api
        // ui nginx cloudflared”`, `script`, `StringComparison.Ordinal` dalam DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly.
        Assert.Contains("up -d --no-build --force-recreate api ui nginx cloudflared", script, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”MaxRetentionSec=30day”`, `script`,
        // `StringComparison.Ordinal` dalam DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly.
        Assert.Contains("MaxRetentionSec=30day", script, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”pg_dump”`, `script`,
        // `StringComparison.OrdinalIgnoreCase` dalam DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly.
        Assert.DoesNotContain("pg_dump", script, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”pg_restore”`, `script`,
        // `StringComparison.OrdinalIgnoreCase` dalam DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly.
        Assert.DoesNotContain("pg_restore", script, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly; bagian berikut berada di luar batas blok tersebut dalam
    // DeploymentScript_ShouldLockMigrateRecalculateAndRollbackImagesOnly.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `OperationalLogMigration_ShouldRemoveRejectedPayloadAndUseThirtyDayRetention` dengan hasil bertipe `void`; operasi ini
    // menangani operational log migration should remove rejected payload dan use thirty hari retention.
    public void OperationalLogMigration_ShouldRemoveRejectedPayloadAndUseThirtyDayRetention()
    // Membuka scope metode OperationalLogMigration_ShouldRemoveRejectedPayloadAndUseThirtyDayRetention; pernyataan/deklarasi berikut berada di dalam
    // batas blok ini dalam OperationalLogMigration_ShouldRemoveRejectedPayloadAndUseThirtyDayRetention.
    {
        // Menyiapkan variabel lokal `migration` untuk nilai migration dengan memanggil `File.ReadAllText` dengan `Path.Combine( RepoRoot, ”database”,
        // ”migrations”, ”V004__limit_operational_logs.sql”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var migration = File.ReadAllText(Path.Combine(
            // Meneruskan `RepoRoot` (nilai repo root) sebagai argumen ke `Path.Combine`.
            RepoRoot,
            // Meneruskan nilai literal `”database”` sebagai argumen ke `Path.Combine`.
            "database",
            // Meneruskan nilai literal `”migrations”` sebagai argumen ke `Path.Combine`.
            "migrations",
            // Meneruskan nilai literal `”V004__limit_operational_logs.sql”` sebagai argumen ke `Path.Combine`.
            "V004__limit_operational_logs.sql"));
        // Menyiapkan variabel lokal `repository` untuk nilai repositori dengan memanggil `File.ReadAllText` dengan `Path.Combine(RepoRoot, ”src”,
        // ”Cashflowpoly.Api”, ”Data”, ”EventRepository.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var repository = File.ReadAllText(Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "Data", "EventRepository.cs"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”status_code integer”`, `migration`,
        // `StringComparison.OrdinalIgnoreCase` dalam OperationalLogMigration_ShouldRemoveRejectedPayloadAndUseThirtyDayRetention.
        Assert.Contains("status_code integer", migration, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”trace_id varchar(64)”`, `migration`,
        // `StringComparison.OrdinalIgnoreCase` dalam OperationalLogMigration_ShouldRemoveRejectedPayloadAndUseThirtyDayRetention.
        Assert.Contains("trace_id varchar(64)", migration, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”raw_payload_json = '{}'::jsonb”`,
        // `migration`, `StringComparison.OrdinalIgnoreCase` dalam OperationalLogMigration_ShouldRemoveRejectedPayloadAndUseThirtyDayRetention.
        Assert.Contains("raw_payload_json = '{}'::jsonb", migration, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”('validation_logs', 30)”`, `migration`,
        // `StringComparison.OrdinalIgnoreCase` dalam OperationalLogMigration_ShouldRemoveRejectedPayloadAndUseThirtyDayRetention.
        Assert.Contains("('validation_logs', 30)", migration, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”('security_audit_logs', 30)”`,
        // `migration`, `StringComparison.OrdinalIgnoreCase` dalam OperationalLogMigration_ShouldRemoveRejectedPayloadAndUseThirtyDayRetention.
        Assert.Contains("('security_audit_logs', 30)", migration, StringComparison.OrdinalIgnoreCase);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”'{}'::jsonb”`, `repository`,
        // `StringComparison.Ordinal` dalam OperationalLogMigration_ShouldRemoveRejectedPayloadAndUseThirtyDayRetention.
        Assert.Contains("'{}'::jsonb", repository, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rawPayloadJson”`, `repository`,
        // `StringComparison.Ordinal` dalam OperationalLogMigration_ShouldRemoveRejectedPayloadAndUseThirtyDayRetention.
        Assert.DoesNotContain("rawPayloadJson", repository, StringComparison.Ordinal);

        // Menyiapkan variabel lokal `retentionWorker` untuk nilai retention worker dengan memanggil `File.ReadAllText` dengan `Path.Combine( RepoRoot,
        // ”src”, ”Cashflowpoly.Api”, ”Infrastructure”, ”LogRetentionWorker.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var retentionWorker = File.ReadAllText(Path.Combine(
            // Meneruskan `RepoRoot` (nilai repo root) sebagai argumen ke `Path.Combine`.
            RepoRoot,
            // Meneruskan nilai literal `”src”` sebagai argumen ke `Path.Combine`.
            "src",
            // Meneruskan nilai literal `”Cashflowpoly.Api”` sebagai argumen ke `Path.Combine`.
            "Cashflowpoly.Api",
            // Meneruskan nilai literal `”Infrastructure”` sebagai argumen ke `Path.Combine`.
            "Infrastructure",
            // Meneruskan nilai literal `”LogRetentionWorker.cs”` sebagai argumen ke `Path.Combine`.
            "LogRetentionWorker.cs"));
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”PeriodicTimer(TimeSpan.FromHours(24))”`, `retentionWorker`, `StringComparison.Ordinal` dalam
        // OperationalLogMigration_ShouldRemoveRejectedPayloadAndUseThirtyDayRetention.
        Assert.Contains("PeriodicTimer(TimeSpan.FromHours(24))", retentionWorker, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”purge_logs_by_retention”`,
        // `retentionWorker`, `StringComparison.Ordinal` dalam OperationalLogMigration_ShouldRemoveRejectedPayloadAndUseThirtyDayRetention.
        Assert.Contains("purge_logs_by_retention", retentionWorker, StringComparison.Ordinal);
    // Menutup scope metode OperationalLogMigration_ShouldRemoveRejectedPayloadAndUseThirtyDayRetention; bagian berikut berada di luar batas blok
    // tersebut dalam OperationalLogMigration_ShouldRemoveRejectedPayloadAndUseThirtyDayRetention.
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
// Menutup scope tipe DeploymentAssetTests; bagian berikut berada di luar batas blok tersebut.
}
