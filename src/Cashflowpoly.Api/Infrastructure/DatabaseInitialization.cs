// Fungsi file: Menerapkan migrasi SQL berurutan dan seed idempoten secara aman.
// Mengimpor namespace `System.Security.Cryptography` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Cryptography;
// Mengimpor namespace `System.Text` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text;
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Infrastructure;

// Mendefinisikan tipe class `DatabaseInitialization`.
internal static class DatabaseInitialization
// Membuka scope tipe DatabaseInitialization; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `BaselineName` menyimpan nilai baseline nama dengan nilai awal nilai literal
    // `”canonical_relational_baseline”`.
    private const string BaselineName = "canonical_relational_baseline";
    // Mendeklarasikan field bertipe `string`: `BaselineVersion` menyimpan nilai baseline versi dengan nilai awal nilai literal `”3.0.13”`.
    private const string BaselineVersion = "3.0.13";
    // Mendeklarasikan field bertipe `long`: `MigrationLockKey` menyimpan nilai migration lock kunci dengan nilai awal nilai literal `43465348504`.
    private const long MigrationLockKey = 43465348504;

    // Mendefinisikan metode `InitializeAsync` dengan hasil bertipe `Task`; operasi ini menangani initialize asinkron. async memungkinkan metode
    // menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `services` bertipe `IServiceProvider` membawa
    // nilai services; Parameter `applyChanges` bertipe `bool` membawa nilai apply changes; Parameter `cancellationToken` bertipe `CancellationToken`
    // membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public static async Task InitializeAsync(
        // Parameter `services` bertipe `IServiceProvider` membawa nilai services.
        IServiceProvider services,
        // Parameter `applyChanges` bertipe `bool` membawa nilai apply changes.
        bool applyChanges,
        // Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
        // permintaan atau aplikasi berhenti.
        CancellationToken cancellationToken)
    // Membuka scope metode InitializeAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam InitializeAsync.
    {
        // Menyiapkan variabel lokal `scope` untuk nilai cakupan dengan membuka scope dependency injection baru agar layanan scoped memiliki masa hidup yang
        // terikat pada blok penggunaan. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope
        // berakhir.
        using var scope = services.CreateScope();
        // Menyiapkan variabel lokal `logger` untuk pencatat log terstruktur untuk memantau proses dan mendiagnosis kegagalan dengan memanggil
        // `scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger` dengan `”DatabaseInitialization”`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitialization");
        // Menyiapkan variabel lokal `configuration` untuk konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar dengan mengambil
        // dependency wajib melalui `scope.ServiceProvider.GetRequiredService<IConfiguration>`; registrasi layanan yang tidak tersedia menyebabkan
        // exception. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        // Menyiapkan variabel lokal `dataSource` untuk sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi dengan mengambil
        // dependency wajib melalui `scope.ServiceProvider.GetRequiredService<NpgsqlDataSource>`; registrasi layanan yang tidak tersedia menyebabkan
        // exception. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var dataSource = scope.ServiceProvider.GetRequiredService<NpgsqlDataSource>();
        // Menyiapkan variabel lokal `migrations` untuk nilai migrations dengan memanggil `ResolveMigrations` dengan `configuration`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var migrations = ResolveMigrations(configuration);

        // Memeriksa kebalikan kondisi `applyChanges`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam InitializeAsync.
        if (!applyChanges)
        // Membuka scope cabang if untuk kondisi `!applyChanges`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam InitializeAsync.
        {
            // Menjalankan hasil operasi asinkron memanggil `VerifyAppliedMigrationsAsync` dengan `dataSource`, `migrations`, `cancellationToken`; await
            // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam InitializeAsync.
            await VerifyAppliedMigrationsAsync(dataSource, migrations, cancellationToken);
            // Menjalankan mencatat log tingkat Information melalui `logger` dengan pesan dan data `”Database migration history verified; no schema changes were
            // applied”` dalam InitializeAsync.
            logger.LogInformation("Database migration history verified; no schema changes were applied");
            // Mengakhiri eksekusi lebih awal dalam InitializeAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!applyChanges`; bagian berikut berada di luar batas blok tersebut dalam InitializeAsync.
        }

        // Menjalankan hasil operasi asinkron memanggil `ApplyMigrationsAsync` dengan `dataSource`, `migrations`, `logger`, `configuration`,
        // `cancellationToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam InitializeAsync.
        await ApplyMigrationsAsync(dataSource, migrations, logger, configuration, cancellationToken);
        // Menjalankan hasil operasi asinkron memanggil `dataSource.ReloadTypesAsync` dengan `cancellationToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai dalam InitializeAsync.
        await dataSource.ReloadTypesAsync(cancellationToken);
    // Menutup scope metode InitializeAsync; bagian berikut berada di luar batas blok tersebut dalam InitializeAsync.
    }

    // Mendefinisikan metode `ApplyMigrationsAsync` dengan hasil bertipe `Task`; operasi ini menangani apply migrations asinkron. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `dataSource` bertipe `NpgsqlDataSource`
    // membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi; Parameter `migrations` bertipe
    // `IReadOnlyList<SqlMigration>` membawa nilai migrations; Parameter `logger` bertipe `ILogger` membawa pencatat log terstruktur untuk memantau
    // proses dan mendiagnosis kegagalan; Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai
    // pengaturan dari sumber terdaftar; Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat
    // dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task ApplyMigrationsAsync(
        // Parameter `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
        NpgsqlDataSource dataSource,
        // Parameter `migrations` bertipe `IReadOnlyList<SqlMigration>` membawa nilai migrations.
        IReadOnlyList<SqlMigration> migrations,
        // Parameter `logger` bertipe `ILogger` membawa pencatat log terstruktur untuk memantau proses dan mendiagnosis kegagalan.
        ILogger logger,
        // Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar.
        IConfiguration configuration,
        // Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
        // permintaan atau aplikasi berhenti.
        CancellationToken cancellationToken)
    // Membuka scope metode ApplyMigrationsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyMigrationsAsync.
    {
        // Menyiapkan variabel lokal `connection` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi
        // asinkron membuka koneksi PostgreSQL melalui `dataSource` menggunakan `cancellationToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `connection` menggunakan `new CommandDefinition( ”select
        // pg_advisory_lock(@lockKey);”, new { lockKey = MigrationLockKey }, cancellationToken: cancellationToken)`; nilai hasil menunjukkan jumlah baris
        // yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ApplyMigrationsAsync.
        await connection.ExecuteAsync(new CommandDefinition(
            // Meneruskan nilai literal `”select pg_advisory_lock(@lockKey);”` sebagai argumen ke konstruktor `CommandDefinition`.
            "select pg_advisory_lock(@lockKey);",
            // Meneruskan objek anonim yang mengelompokkan lockKey sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new { lockKey = MigrationLockKey },
            // Meneruskan `cancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti)
            // sebagai argumen bernama `cancellationToken`.
            cancellationToken: cancellationToken));

        // Memulai blok try dalam ApplyMigrationsAsync; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyMigrationsAsync.
        {
            // Menyiapkan variabel lokal `databaseState` untuk nilai database keadaan dengan hasil operasi asinkron membaca tepat satu baris basis data melalui
            // `connection.QuerySingleAsync<DatabaseState>` dengan `new CommandDefinition( ”select to_regclass('public.app_users') is not null as
            // HasApplicationSchema, to_regclass('public.schema_history') is not null as HasHistory;”, cancellat...`; jumlah baris selain satu menyebabkan
            // exception; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var databaseState = await connection.QuerySingleAsync<DatabaseState>(new CommandDefinition(
                // Meneruskan nilai literal `”select to_regclass('public.app_users') is not null as HasApplicationSchema, to_regclass('public.schema_history') is
                // not null as HasHistory;”` sebagai argumen ke konstruktor `CommandDefinition`.
                "select to_regclass('public.app_users') is not null as HasApplicationSchema, to_regclass('public.schema_history') is not null as HasHistory;",
                // Meneruskan `cancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti)
                // sebagai argumen bernama `cancellationToken`.
                cancellationToken: cancellationToken));

            // Memeriksa kebalikan kondisi `databaseState.HasApplicationSchema`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ApplyMigrationsAsync.
            if (!databaseState.HasApplicationSchema)
            // Membuka scope cabang if untuk kondisi `!databaseState.HasApplicationSchema`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ApplyMigrationsAsync.
            {
                // Menyiapkan variabel lokal `baselinePath` untuk nilai baseline path dengan memanggil `ResolveSqlFilePath` dengan `”00_create_schema.sql”`,
                // `configuration`. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var baselinePath = ResolveSqlFilePath("00_create_schema.sql", configuration);
                // Menyiapkan variabel lokal `baselineSql` untuk nilai baseline SQL dengan hasil operasi asinkron memanggil `File.ReadAllTextAsync` dengan
                // `baselinePath`, `cancellationToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
                // ekspresi nilai awal.
                var baselineSql = await File.ReadAllTextAsync(baselinePath, cancellationToken);
                // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `connection` menggunakan `new CommandDefinition(baselineSql,
                // cancellationToken: cancellationToken)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama
                // operasi belum selesai dalam ApplyMigrationsAsync.
                await connection.ExecuteAsync(new CommandDefinition(baselineSql, cancellationToken: cancellationToken));
                // Menjalankan mencatat log tingkat Information melalui `logger` dengan pesan dan data `”Applied canonical database baseline from {BaselinePath}”`,
                // `baselinePath` dalam ApplyMigrationsAsync.
                logger.LogInformation("Applied canonical database baseline from {BaselinePath}", baselinePath);
            // Menutup scope cabang if untuk kondisi `!databaseState.HasApplicationSchema`; bagian berikut berada di luar batas blok tersebut dalam
            // ApplyMigrationsAsync.
            }
            // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam ApplyMigrationsAsync.
            else if (!databaseState.HasHistory)
            // Membuka scope cabang if untuk kondisi `!databaseState.HasHistory`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ApplyMigrationsAsync.
            {
                // Menjalankan hasil operasi asinkron memanggil `VerifyCanonicalBaselineAsync` dengan `connection`, `cancellationToken`; await menunggu hasil tanpa
                // memblokir thread selama operasi belum selesai dalam ApplyMigrationsAsync.
                await VerifyCanonicalBaselineAsync(connection, cancellationToken);
            // Menutup scope cabang if untuk kondisi `!databaseState.HasHistory`; bagian berikut berada di luar batas blok tersebut dalam ApplyMigrationsAsync.
            }

            // Menjalankan hasil operasi asinkron memanggil `EnsureHistoryTableAsync` dengan `connection`, `cancellationToken`; await menunggu hasil tanpa
            // memblokir thread selama operasi belum selesai dalam ApplyMigrationsAsync.
            await EnsureHistoryTableAsync(connection, cancellationToken);
            // Menjalankan hasil operasi asinkron memanggil `RecordOrVerifyBaselineAsync` dengan `connection`, `configuration`, `cancellationToken`; await
            // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ApplyMigrationsAsync.
            await RecordOrVerifyBaselineAsync(connection, configuration, cancellationToken);

            // Mengulangi setiap elemen `migrations`; elemen saat ini disimpan sebagai `migration` bertipe `var` untuk diproses oleh badan loop dalam
            // ApplyMigrationsAsync.
            foreach (var migration in migrations)
            // Membuka scope loop setiap migration dari `migrations`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyMigrationsAsync.
            {
                // Menyiapkan variabel lokal `applied` untuk nilai applied dengan hasil operasi asinkron membaca satu hasil basis data melalui
                // `connection.QuerySingleOrDefaultAsync<AppliedMigration>` dengan `new CommandDefinition( ”select version, name, checksum from schema_history where
                // version = @version;”, new { migration.Version }, cancellationToken: cancellationToken)`; nilai default menunjukkan tidak ada baris hasil; await
                // menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var applied = await connection.QuerySingleOrDefaultAsync<AppliedMigration>(new CommandDefinition(
                    // Meneruskan nilai literal `”select version, name, checksum from schema_history where version = @version;”` sebagai argumen ke konstruktor
                    // `CommandDefinition`.
                    "select version, name, checksum from schema_history where version = @version;",
                    // Meneruskan objek anonim yang mengelompokkan migration.Version sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    new { migration.Version },
                    // Meneruskan `cancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti)
                    // sebagai argumen bernama `cancellationToken`.
                    cancellationToken: cancellationToken));

                // Memeriksa hasil pencocokan `applied` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                // ApplyMigrationsAsync.
                if (applied is not null)
                // Membuka scope cabang if untuk kondisi `applied is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ApplyMigrationsAsync.
                {
                    // Menjalankan memanggil `EnsureChecksumMatches` dengan `applied`, `migration` dalam ApplyMigrationsAsync.
                    EnsureChecksumMatches(applied, migration);
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam ApplyMigrationsAsync.
                    continue;
                // Menutup scope cabang if untuk kondisi `applied is not null`; bagian berikut berada di luar batas blok tersebut dalam ApplyMigrationsAsync.
                }

                // Menyiapkan variabel lokal `transaction` untuk transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan dengan hasil operasi
                // asinkron memulai transaksi pada `connection` menggunakan `cancellationToken` agar perubahan terkait dapat diselesaikan bersama; await menunggu
                // hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya
                // dilepas otomatis saat scope berakhir.
                await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
                // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `connection` menggunakan `new CommandDefinition( migration.Sql, transaction:
                // transaction, cancellationToken: cancellationToken)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir
                // thread selama operasi belum selesai dalam ApplyMigrationsAsync.
                await connection.ExecuteAsync(new CommandDefinition(
                    // Meneruskan `migration.Sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`.
                    migration.Sql,
                    // Meneruskan `transaction` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen bernama `transaction`.
                    transaction: transaction,
                    // Meneruskan `cancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti)
                    // sebagai argumen bernama `cancellationToken`.
                    cancellationToken: cancellationToken));
                // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `connection` menggunakan `new CommandDefinition( ”insert into schema_history
                // (version, name, checksum) values (@Version, @Name, @Checksum);”, new { migration.Version, migration.Name, migration.Checksum...`; nilai hasil
                // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ApplyMigrationsAsync.
                await connection.ExecuteAsync(new CommandDefinition(
                    // Meneruskan nilai literal `”insert into schema_history (version, name, checksum) values (@Version, @Name, @Checksum);”` sebagai argumen ke
                    // konstruktor `CommandDefinition`.
                    "insert into schema_history (version, name, checksum) values (@Version, @Name, @Checksum);",
                    // Meneruskan objek anonim yang mengelompokkan migration.Version, migration.Name, migration.Checksum sebagai satu nilai sebagai argumen ke
                    // konstruktor `CommandDefinition`.
                    new { migration.Version, migration.Name, migration.Checksum },
                    // Meneruskan `transaction` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen bernama `transaction`.
                    transaction: transaction,
                    // Meneruskan `cancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti)
                    // sebagai argumen bernama `cancellationToken`.
                    cancellationToken: cancellationToken));
                // Menjalankan hasil operasi asinkron mengesahkan transaksi `transaction` sehingga perubahan yang terkumpul menjadi permanen; await menunggu hasil
                // tanpa memblokir thread selama operasi belum selesai dalam ApplyMigrationsAsync.
                await transaction.CommitAsync(cancellationToken);
                // Menjalankan mencatat log tingkat Information melalui `logger` dengan pesan dan data `”Applied database migration V{Version}: {Name}”`,
                // `migration.Version`, `migration.Name` dalam ApplyMigrationsAsync.
                logger.LogInformation("Applied database migration V{Version}: {Name}", migration.Version, migration.Name);
            // Menutup scope loop setiap migration dari `migrations`; bagian berikut berada di luar batas blok tersebut dalam ApplyMigrationsAsync.
            }

            // Menjalankan hasil operasi asinkron memanggil `SeedSqlFileAsync` dengan `connection`, `logger`, `”01_seed_default_rulesets_components.sql”`,
            // `configuration`, `cancellationToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ApplyMigrationsAsync.
            await SeedSqlFileAsync(connection, logger, "01_seed_default_rulesets_components.sql", configuration, cancellationToken);
            // Memeriksa memanggil `configuration.GetValue<bool>` dengan `”DatabaseMigrations:SeedSimulation”`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam ApplyMigrationsAsync.
            if (configuration.GetValue<bool>("DatabaseMigrations:SeedSimulation"))
            // Membuka scope cabang if untuk kondisi `configuration.GetValue<bool>(”DatabaseMigrations:SeedSimulation”)`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam ApplyMigrationsAsync.
            {
                // Menjalankan hasil operasi asinkron memanggil `SeedSqlFileAsync` dengan `connection`, `logger`, `”02_seed_simulation_sessions_events.sql”`,
                // `configuration`, `cancellationToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ApplyMigrationsAsync.
                await SeedSqlFileAsync(connection, logger, "02_seed_simulation_sessions_events.sql", configuration, cancellationToken);
                // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `connection` menggunakan `new CommandDefinition( ”update app_users set
                // is_demo = true where lower(username::text) in ('rina.kartika', 'marco', 'marcello', 'hugo', 'manalu');”, cancellationToken: cancel...`; nilai
                // hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
                // ApplyMigrationsAsync.
                await connection.ExecuteAsync(new CommandDefinition(
                    // Meneruskan nilai literal `”update app_users set is_demo = true where lower(username::text) in ('rina.kartika', 'marco', 'marcello', 'hugo',
                    // 'manalu');”` sebagai argumen ke konstruktor `CommandDefinition`.
                    "update app_users set is_demo = true where lower(username::text) in ('rina.kartika', 'marco', 'marcello', 'hugo', 'manalu');",
                    // Meneruskan `cancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti)
                    // sebagai argumen bernama `cancellationToken`.
                    cancellationToken: cancellationToken));
            // Menutup scope cabang if untuk kondisi `configuration.GetValue<bool>(”DatabaseMigrations:SeedSimulation”)`; bagian berikut berada di luar batas
            // blok tersebut dalam ApplyMigrationsAsync.
            }
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam ApplyMigrationsAsync.
        }
        // Menangani exception yang muncul dari blok try sebelumnya dalam ApplyMigrationsAsync.
        catch
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyMigrationsAsync.
        {
            // Memulai blok try dalam ApplyMigrationsAsync; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
            // keluar.
            try
            // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyMigrationsAsync.
            {
                // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `connection` menggunakan `new CommandDefinition(”rollback;”,
                // cancellationToken: cancellationToken)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama
                // operasi belum selesai dalam ApplyMigrationsAsync.
                await connection.ExecuteAsync(new CommandDefinition("rollback;", cancellationToken: cancellationToken));
            // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam ApplyMigrationsAsync.
            }
            // Menangani exception `PostgresException` melalui variabel dalam ApplyMigrationsAsync.
            catch (PostgresException)
            // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyMigrationsAsync.
            {
                // Menutup koneksi tetap melepaskan advisory lock jika transaksi gagal sebelum rollback.
            // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam ApplyMigrationsAsync.
            }

            // Melempar ulang exception yang sedang ditangani sambil mempertahankan jejak asal kegagalannya.
            throw;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam ApplyMigrationsAsync.
        }
        // Menjalankan blok finally ketika alur meninggalkan try/catch, termasuk saat terjadi exception dalam ApplyMigrationsAsync; bagian ini dipakai untuk
        // pekerjaan penutup yang harus tetap dilakukan.
        finally
        // Membuka scope pekerjaan penutup finally; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyMigrationsAsync.
        {
            // Memeriksa perbandingan kesamaan antara `connection.FullState` dan `System.Data.ConnectionState.Open`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam ApplyMigrationsAsync.
            if (connection.FullState == System.Data.ConnectionState.Open)
            // Membuka scope cabang if untuk kondisi `connection.FullState == System.Data.ConnectionState.Open`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam ApplyMigrationsAsync.
            {
                // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `connection` menggunakan `new CommandDefinition( ”select
                // pg_advisory_unlock(@lockKey);”, new { lockKey = MigrationLockKey }, cancellationToken: cancellationToken)`; nilai hasil menunjukkan jumlah baris
                // yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ApplyMigrationsAsync.
                await connection.ExecuteAsync(new CommandDefinition(
                    // Meneruskan nilai literal `”select pg_advisory_unlock(@lockKey);”` sebagai argumen ke konstruktor `CommandDefinition`.
                    "select pg_advisory_unlock(@lockKey);",
                    // Meneruskan objek anonim yang mengelompokkan lockKey sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                    new { lockKey = MigrationLockKey },
                    // Meneruskan `cancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti)
                    // sebagai argumen bernama `cancellationToken`.
                    cancellationToken: cancellationToken));
            // Menutup scope cabang if untuk kondisi `connection.FullState == System.Data.ConnectionState.Open`; bagian berikut berada di luar batas blok
            // tersebut dalam ApplyMigrationsAsync.
            }
        // Menutup scope pekerjaan penutup finally; bagian berikut berada di luar batas blok tersebut dalam ApplyMigrationsAsync.
        }
    // Menutup scope metode ApplyMigrationsAsync; bagian berikut berada di luar batas blok tersebut dalam ApplyMigrationsAsync.
    }

    // Mendefinisikan metode `VerifyAppliedMigrationsAsync` dengan hasil bertipe `Task`; operasi ini menangani verify applied migrations asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `dataSource` bertipe
    // `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi; Parameter `migrations` bertipe
    // `IReadOnlyList<SqlMigration>` membawa nilai migrations; Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar
    // operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task VerifyAppliedMigrationsAsync(
        // Parameter `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi.
        NpgsqlDataSource dataSource,
        // Parameter `migrations` bertipe `IReadOnlyList<SqlMigration>` membawa nilai migrations.
        IReadOnlyList<SqlMigration> migrations,
        // Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
        // permintaan atau aplikasi berhenti.
        CancellationToken cancellationToken)
    // Membuka scope metode VerifyAppliedMigrationsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // VerifyAppliedMigrationsAsync.
    {
        // Menyiapkan variabel lokal `connection` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi
        // asinkron membuka koneksi PostgreSQL melalui `dataSource` menggunakan `cancellationToken`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        // Menyiapkan variabel lokal `hasHistory` untuk nilai memiliki history dengan hasil operasi asinkron menjalankan perintah basis data melalui
        // `connection` dengan `new CommandDefinition( ”select to_regclass('public.schema_history') is not null;”, cancellationToken: cancellationToken)`
        // dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var hasHistory = await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
            // Meneruskan nilai literal `”select to_regclass('public.schema_history') is not null;”` sebagai argumen ke konstruktor `CommandDefinition`.
            "select to_regclass('public.schema_history') is not null;",
            // Meneruskan `cancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti)
            // sebagai argumen bernama `cancellationToken`.
            cancellationToken: cancellationToken));
        // Memeriksa kebalikan kondisi `hasHistory`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam VerifyAppliedMigrationsAsync.
        if (!hasHistory)
        // Membuka scope cabang if untuk kondisi `!hasHistory`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // VerifyAppliedMigrationsAsync.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Database belum dimigrasikan. Jalankan aplikasi
            // dengan --migrate-only sebelum menyalakan API.”) dalam VerifyAppliedMigrationsAsync; pemanggil atau middleware penanganan error menerima kegagalan
            // ini.
            throw new InvalidOperationException("Database belum dimigrasikan. Jalankan aplikasi dengan --migrate-only sebelum menyalakan API.");
        // Menutup scope cabang if untuk kondisi `!hasHistory`; bagian berikut berada di luar batas blok tersebut dalam VerifyAppliedMigrationsAsync.
        }

        // Menyiapkan variabel lokal `applied` untuk nilai applied dengan membangun kamus dari `(await connection.QueryAsync<AppliedMigration>(new
        // CommandDefinition( ”select version, name, checksum from schema_history order by version;”, cancellationToken: cancellationTo...` dengan pemilihan
        // kunci/nilai `item => item.Version`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var applied = (await connection.QueryAsync<AppliedMigration>(new CommandDefinition(
            // Meneruskan nilai literal `”select version, name, checksum from schema_history order by version;”` sebagai argumen ke konstruktor
            // `CommandDefinition`.
            "select version, name, checksum from schema_history order by version;",
            // Meneruskan `cancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti)
            // sebagai argumen bernama `cancellationToken`; Meneruskan fungsi lambda `item => item.Version` yang dijalankan oleh operasi pemanggil untuk
            // memproses setiap masukan sebagai argumen ke `(await connection.QueryAsync<AppliedMigration>(new CommandDefinition( ”select version, name,
            // checksum from schema_history order by version;”, cancellationToken: cancellationTo...`.
            cancellationToken: cancellationToken))).ToDictionary(item => item.Version);

        // Mengulangi setiap elemen `migrations`; elemen saat ini disimpan sebagai `migration` bertipe `var` untuk diproses oleh badan loop dalam
        // VerifyAppliedMigrationsAsync.
        foreach (var migration in migrations)
        // Membuka scope loop setiap migration dari `migrations`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // VerifyAppliedMigrationsAsync.
        {
            // Memeriksa kebalikan kondisi `applied.TryGetValue(migration.Version, out var existing)`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam VerifyAppliedMigrationsAsync.
            if (!applied.TryGetValue(migration.Version, out var existing))
            // Membuka scope cabang if untuk kondisi `!applied.TryGetValue(migration.Version, out var existing)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam VerifyAppliedMigrationsAsync.
            {
                // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Migrasi V{migration.Version} belum diterapkan.
                // Jalankan --migrate-only.”) dalam VerifyAppliedMigrationsAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
                throw new InvalidOperationException($"Migrasi V{migration.Version} belum diterapkan. Jalankan --migrate-only.");
            // Menutup scope cabang if untuk kondisi `!applied.TryGetValue(migration.Version, out var existing)`; bagian berikut berada di luar batas blok
            // tersebut dalam VerifyAppliedMigrationsAsync.
            }

            // Menjalankan memanggil `EnsureChecksumMatches` dengan `existing`, `migration` dalam VerifyAppliedMigrationsAsync.
            EnsureChecksumMatches(existing, migration);
        // Menutup scope loop setiap migration dari `migrations`; bagian berikut berada di luar batas blok tersebut dalam VerifyAppliedMigrationsAsync.
        }
    // Menutup scope metode VerifyAppliedMigrationsAsync; bagian berikut berada di luar batas blok tersebut dalam VerifyAppliedMigrationsAsync.
    }

    // Mendefinisikan metode `VerifyCanonicalBaselineAsync` dengan hasil bertipe `Task`; operasi ini menangani verify canonical baseline asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `connection` bertipe
    // `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `cancellationToken` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task VerifyCanonicalBaselineAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    // Membuka scope metode VerifyCanonicalBaselineAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // VerifyCanonicalBaselineAsync.
    {
        // Menyiapkan variabel lokal `hasBaselineRegistry` untuk nilai memiliki baseline registry dengan hasil operasi asinkron menjalankan perintah basis
        // data melalui `connection` dengan `new CommandDefinition( ”select to_regclass('public.schema_baseline_versions') is not null;”, cancellationToken:
        // cancellationToken)` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var hasBaselineRegistry = await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
            // Meneruskan nilai literal `”select to_regclass('public.schema_baseline_versions') is not null;”` sebagai argumen ke konstruktor
            // `CommandDefinition`.
            "select to_regclass('public.schema_baseline_versions') is not null;",
            // Meneruskan `cancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti)
            // sebagai argumen bernama `cancellationToken`.
            cancellationToken: cancellationToken));
        // Memeriksa kebalikan kondisi `hasBaselineRegistry`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam VerifyCanonicalBaselineAsync.
        if (!hasBaselineRegistry)
        // Membuka scope cabang if untuk kondisi `!hasBaselineRegistry`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // VerifyCanonicalBaselineAsync.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Database lama tidak mempunyai penanda baseline
            // resmi; migrasi dihentikan agar schema tidak ditebak.”) dalam VerifyCanonicalBaselineAsync; pemanggil atau middleware penanganan error menerima
            // kegagalan ini.
            throw new InvalidOperationException("Database lama tidak mempunyai penanda baseline resmi; migrasi dihentikan agar schema tidak ditebak.");
        // Menutup scope cabang if untuk kondisi `!hasBaselineRegistry`; bagian berikut berada di luar batas blok tersebut dalam
        // VerifyCanonicalBaselineAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `connection` menggunakan `new CommandDefinition( ”select
        // assert_schema_baseline(@baselineName, @baselineVersion);”, new { baselineName = BaselineName, baselineVersion = BaselineVersion },
        // cancellationT...`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai dalam VerifyCanonicalBaselineAsync.
        await connection.ExecuteAsync(new CommandDefinition(
            // Meneruskan nilai literal `”select assert_schema_baseline(@baselineName, @baselineVersion);”` sebagai argumen ke konstruktor `CommandDefinition`.
            "select assert_schema_baseline(@baselineName, @baselineVersion);",
            // Meneruskan objek anonim yang mengelompokkan baselineName, baselineVersion sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new { baselineName = BaselineName, baselineVersion = BaselineVersion },
            // Meneruskan `cancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti)
            // sebagai argumen bernama `cancellationToken`.
            cancellationToken: cancellationToken));
    // Menutup scope metode VerifyCanonicalBaselineAsync; bagian berikut berada di luar batas blok tersebut dalam VerifyCanonicalBaselineAsync.
    }

    // Mendefinisikan metode `EnsureHistoryTableAsync` dengan hasil bertipe `Task`; operasi ini menangani ensure history table asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `connection` bertipe
    // `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `cancellationToken` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task EnsureHistoryTableAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    // Membuka scope metode EnsureHistoryTableAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam EnsureHistoryTableAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `create table if not exists schema_history (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `version integer not null,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `name varchar(160) not null,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `checksum char(64) not null,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `applied_at timestamptz not
        // null default now(),`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `constraint pk_schema_history
        // primary key (version),`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `constraint
        // ck_schema_history_version check (version > 0),`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `constraint
        // ck_schema_history_name check (nullif(btrim(name), '') is not null),`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `constraint
        // ck_schema_history_checksum check (checksum ~ '^[0-9a-f]{64}$')`.
        // Baris literal 11: Pembatas literal/penutup `);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 12: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            create table if not exists schema_history (
              version integer not null,
              name varchar(160) not null,
              checksum char(64) not null,
              applied_at timestamptz not null default now(),
              constraint pk_schema_history primary key (version),
              constraint ck_schema_history_version check (version > 0),
              constraint ck_schema_history_name check (nullif(btrim(name), '') is not null),
              constraint ck_schema_history_checksum check (checksum ~ '^[0-9a-f]{64}$')
            );
            """;
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `connection` menggunakan `new CommandDefinition(sql, cancellationToken:
        // cancellationToken)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai dalam EnsureHistoryTableAsync.
        await connection.ExecuteAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));
    // Menutup scope metode EnsureHistoryTableAsync; bagian berikut berada di luar batas blok tersebut dalam EnsureHistoryTableAsync.
    }

    // Mendefinisikan metode `RecordOrVerifyBaselineAsync` dengan hasil bertipe `Task`; operasi ini menangani rekaman atau verify baseline asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `connection` bertipe
    // `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `configuration` bertipe
    // `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar; Parameter `cancellationToken` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task RecordOrVerifyBaselineAsync(
        // Parameter `connection` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection connection,
        // Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar.
        IConfiguration configuration,
        // Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
        // permintaan atau aplikasi berhenti.
        CancellationToken cancellationToken)
    // Membuka scope metode RecordOrVerifyBaselineAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RecordOrVerifyBaselineAsync.
    {
        // Menyiapkan variabel lokal `baselinePath` untuk nilai baseline path dengan memanggil `ResolveSqlFilePath` dengan `”00_create_schema.sql”`,
        // `configuration`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var baselinePath = ResolveSqlFilePath("00_create_schema.sql", configuration);
        // Menyiapkan variabel lokal `baseline` untuk nilai baseline dengan memanggil `CreateMigrationDescriptor` dengan `1`, `BaselineName`,
        // `baselinePath`, `false`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var baseline = CreateMigrationDescriptor(1, BaselineName, baselinePath, includeSql: false);
        // Menyiapkan variabel lokal `applied` untuk nilai applied dengan hasil operasi asinkron membaca satu hasil basis data melalui
        // `connection.QuerySingleOrDefaultAsync<AppliedMigration>` dengan `new CommandDefinition( ”select version, name, checksum from schema_history where
        // version = 1;”, cancellationToken: cancellationToken)`; nilai default menunjukkan tidak ada baris hasil; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var applied = await connection.QuerySingleOrDefaultAsync<AppliedMigration>(new CommandDefinition(
            // Meneruskan nilai literal `”select version, name, checksum from schema_history where version = 1;”` sebagai argumen ke konstruktor
            // `CommandDefinition`.
            "select version, name, checksum from schema_history where version = 1;",
            // Meneruskan `cancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti)
            // sebagai argumen bernama `cancellationToken`.
            cancellationToken: cancellationToken));

        // Memeriksa hasil pencocokan `applied` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // RecordOrVerifyBaselineAsync.
        if (applied is not null)
        // Membuka scope cabang if untuk kondisi `applied is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // RecordOrVerifyBaselineAsync.
        {
            // Menjalankan memanggil `EnsureChecksumMatches` dengan `applied`, `baseline` dalam RecordOrVerifyBaselineAsync.
            EnsureChecksumMatches(applied, baseline);
            // Mengakhiri eksekusi lebih awal dalam RecordOrVerifyBaselineAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `applied is not null`; bagian berikut berada di luar batas blok tersebut dalam RecordOrVerifyBaselineAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `connection` menggunakan `new CommandDefinition( ”insert into schema_history
        // (version, name, checksum) values (1, @name, @checksum);”, new { name = BaselineName, checksum = baseline.Checksum }, cancell...`; nilai hasil
        // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // RecordOrVerifyBaselineAsync.
        await connection.ExecuteAsync(new CommandDefinition(
            // Meneruskan nilai literal `”insert into schema_history (version, name, checksum) values (1, @name, @checksum);”` sebagai argumen ke konstruktor
            // `CommandDefinition`.
            "insert into schema_history (version, name, checksum) values (1, @name, @checksum);",
            // Meneruskan objek anonim yang mengelompokkan name, checksum sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new { name = BaselineName, checksum = baseline.Checksum },
            // Meneruskan `cancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti)
            // sebagai argumen bernama `cancellationToken`.
            cancellationToken: cancellationToken));
    // Menutup scope metode RecordOrVerifyBaselineAsync; bagian berikut berada di luar batas blok tersebut dalam RecordOrVerifyBaselineAsync.
    }

    // Mendefinisikan metode `SeedSqlFileAsync` dengan hasil bertipe `Task`; operasi ini menangani seed SQL file asinkron. async memungkinkan metode
    // menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `connection` bertipe `NpgsqlConnection` membawa
    // koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `logger` bertipe `ILogger` membawa pencatat log terstruktur
    // untuk memantau proses dan mendiagnosis kegagalan; Parameter `fileName` bertipe `string` membawa nilai file nama; Parameter `configuration`
    // bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar; Parameter `cancellationToken`
    // bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private static async Task SeedSqlFileAsync(
        // Parameter `connection` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection connection,
        // Parameter `logger` bertipe `ILogger` membawa pencatat log terstruktur untuk memantau proses dan mendiagnosis kegagalan.
        ILogger logger,
        // Parameter `fileName` bertipe `string` membawa nilai file nama.
        string fileName,
        // Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar.
        IConfiguration configuration,
        // Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
        // permintaan atau aplikasi berhenti.
        CancellationToken cancellationToken)
    // Membuka scope metode SeedSqlFileAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SeedSqlFileAsync.
    {
        // Menyiapkan variabel lokal `seedPath` untuk nilai seed path dengan memanggil `ResolveSqlFilePath` dengan `fileName`, `configuration`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var seedPath = ResolveSqlFilePath(fileName, configuration);
        // Menyiapkan variabel lokal `seedSql` untuk nilai seed SQL dengan hasil operasi asinkron memanggil `File.ReadAllTextAsync` dengan `seedPath`,
        // `cancellationToken`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var seedSql = await File.ReadAllTextAsync(seedPath, cancellationToken);
        // Memperbarui `seedSql` menggunakan memanggil `seedSql.Replace` dengan `”create extension if not exists pgcrypto;”`, `string.Empty`,
        // `StringComparison.OrdinalIgnoreCase` dalam SeedSqlFileAsync.
        seedSql = seedSql.Replace("create extension if not exists pgcrypto;", string.Empty, StringComparison.OrdinalIgnoreCase);
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `connection` menggunakan `new CommandDefinition(seedSql, cancellationToken:
        // cancellationToken)`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai dalam SeedSqlFileAsync.
        await connection.ExecuteAsync(new CommandDefinition(seedSql, cancellationToken: cancellationToken));
        // Menjalankan mencatat log tingkat Information melalui `logger` dengan pesan dan data `”Applied idempotent seed from {SeedPath}”`, `seedPath` dalam
        // SeedSqlFileAsync.
        logger.LogInformation("Applied idempotent seed from {SeedPath}", seedPath);
    // Menutup scope metode SeedSqlFileAsync; bagian berikut berada di luar batas blok tersebut dalam SeedSqlFileAsync.
    }

    // Mendefinisikan metode `ResolveMigrations` dengan hasil bertipe `IReadOnlyList<SqlMigration>`; operasi ini menangani resolve migrations. Masukan:
    // Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar.
    private static IReadOnlyList<SqlMigration> ResolveMigrations(IConfiguration configuration)
    // Membuka scope metode ResolveMigrations; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveMigrations.
    {
        // Menyiapkan variabel lokal `directory` untuk nilai directory dengan memanggil `ResolveSqlDirectory` dengan `”migrations”`, `configuration`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var directory = ResolveSqlDirectory("migrations", configuration);
        // Mengembalikan mematerialisasi urutan `Directory.GetFiles(directory, ”V*__*.sql”) .Select(path => { var fileName = Path.GetFileName(path); var
        // separator = fileName.IndexOf(”__”, StringComparison.Ordinal); if (separ...` menjadi array dengan elemen hasil saat ini kepada pemanggil dalam
        // ResolveMigrations; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return Directory.GetFiles(directory, "V*__*.sql")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(path => dalam ResolveMigrations; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .Select(path =>
            // Membuka scope fungsi lambda yang dipasok ke `Directory.GetFiles(directory, ”V*__*.sql”) .Select`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam ResolveMigrations.
            {
                // Menyiapkan variabel lokal `fileName` untuk nilai file nama dengan memanggil `Path.GetFileName` dengan `path`. Tipe variabel disimpulkan dari
                // ekspresi nilai awal.
                var fileName = Path.GetFileName(path);
                // Menyiapkan variabel lokal `separator` untuk nilai separator dengan memanggil `fileName.IndexOf` dengan `”__”`, `StringComparison.Ordinal`. Tipe
                // variabel disimpulkan dari ekspresi nilai awal.
                var separator = fileName.IndexOf("__", StringComparison.Ordinal);
                // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `separator < 2 || !int.TryParse(fileName.AsSpan(1, separator - 1), out
                // var version)` dan `version <= 1`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar
                // dalam ResolveMigrations.
                if (separator < 2 || !int.TryParse(fileName.AsSpan(1, separator - 1), out var version) || version <= 1)
                // Membuka scope cabang if untuk kondisi `separator < 2 || !int.TryParse(fileName.AsSpan(1, separator - 1), out var version) || version <= 1`;
                // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveMigrations.
                {
                    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Nama migrasi '{fileName}' tidak valid. Gunakan
                    // VNNN__nama.sql dengan versi di atas 1.”) dalam ResolveMigrations; pemanggil atau middleware penanganan error menerima kegagalan ini.
                    throw new InvalidOperationException($"Nama migrasi '{fileName}' tidak valid. Gunakan VNNN__nama.sql dengan versi di atas 1.");
                // Menutup scope cabang if untuk kondisi `separator < 2 || !int.TryParse(fileName.AsSpan(1, separator - 1), out var version) || version <= 1`;
                // bagian berikut berada di luar batas blok tersebut dalam ResolveMigrations.
                }

                // Menyiapkan variabel lokal `name` untuk nilai nama dengan memanggil `Path.GetFileNameWithoutExtension(fileName)[(separator + 2)..].Replace` dengan
                // `'_'`, `' '`. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var name = Path.GetFileNameWithoutExtension(fileName)[(separator + 2)..].Replace('_', ' ');
                // Mengembalikan memanggil `CreateMigrationDescriptor` dengan `version`, `name`, `path`, `true` kepada pemanggil dalam ResolveMigrations; eksekusi
                // jalur ini selesai setelah nilai hasil ditentukan.
                return CreateMigrationDescriptor(version, name, path, includeSql: true);
            // Menutup scope fungsi lambda yang dipasok ke `Directory.GetFiles(directory, ”V*__*.sql”) .Select`; bagian berikut berada di luar batas blok
            // tersebut dalam ResolveMigrations.
            })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(migration => migration.Version) dalam ResolveMigrations; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderBy(migration => migration.Version)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToArray(); dalam ResolveMigrations; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .ToArray();
    // Menutup scope metode ResolveMigrations; bagian berikut berada di luar batas blok tersebut dalam ResolveMigrations.
    }

    // Mendefinisikan metode `ResolveSqlDirectory` dengan hasil bertipe `string`; operasi ini menangani resolve SQL directory. Masukan: Parameter
    // `directoryName` bertipe `string` membawa nilai directory nama; Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi
    // yang menyediakan nilai pengaturan dari sumber terdaftar.
    private static string ResolveSqlDirectory(string directoryName, IConfiguration configuration)
    // Membuka scope metode ResolveSqlDirectory; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveSqlDirectory.
    {
        // Menyiapkan variabel lokal `markerPath` untuk nilai marker path dengan memanggil `ResolveSqlFilePath` dengan `Path.Combine(directoryName,
        // ”.keep”)`, `configuration`, `false`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var markerPath = ResolveSqlFilePath(Path.Combine(directoryName, ".keep"), configuration, requireFile: false);
        // Menyiapkan variabel lokal `directory` untuk nilai directory dengan `Path.GetDirectoryName(markerPath)` dengan penegasan non-null untuk analisis
        // compiler; operator ! tidak menambah pemeriksaan saat runtime. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var directory = Path.GetDirectoryName(markerPath)!;
        // Memeriksa kebalikan kondisi `Directory.Exists(directory)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveSqlDirectory.
        if (!Directory.Exists(directory))
        // Membuka scope cabang if untuk kondisi `!Directory.Exists(directory)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveSqlDirectory.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `DirectoryNotFoundException` dengan argumen ($”Direktori migrasi SQL '{directoryName}' tidak
            // ditemukan.”) dalam ResolveSqlDirectory; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new DirectoryNotFoundException($"Direktori migrasi SQL '{directoryName}' tidak ditemukan.");
        // Menutup scope cabang if untuk kondisi `!Directory.Exists(directory)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveSqlDirectory.
        }

        // Mengembalikan `directory` (nilai directory) kepada pemanggil dalam ResolveSqlDirectory; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return directory;
    // Menutup scope metode ResolveSqlDirectory; bagian berikut berada di luar batas blok tersebut dalam ResolveSqlDirectory.
    }

    // Mendefinisikan metode `ResolveSqlFilePath` dengan hasil bertipe `string`; operasi ini menangani resolve SQL file path. Masukan: Parameter
    // `fileName` bertipe `string` membawa nilai file nama; Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi yang
    // menyediakan nilai pengaturan dari sumber terdaftar; Parameter `requireFile` bertipe `bool` membawa nilai require file; bila argumen tidak
    // diberikan digunakan true, yaitu kondisi aktif/terpenuhi.
    private static string ResolveSqlFilePath(
        // Parameter `fileName` bertipe `string` membawa nilai file nama.
        string fileName,
        // Parameter `configuration` bertipe `IConfiguration` membawa konfigurasi aplikasi yang menyediakan nilai pengaturan dari sumber terdaftar.
        IConfiguration configuration,
        // Parameter `requireFile` bertipe `bool` membawa nilai require file; bila argumen tidak diberikan digunakan true, yaitu kondisi aktif/terpenuhi.
        bool requireFile = true)
    // Membuka scope metode ResolveSqlFilePath; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveSqlFilePath.
    {
        // Menyiapkan variabel lokal `candidateRoots` untuk nilai candidate roots dengan objek baru bertipe `List<string>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var candidateRoots = new List<string>();
        // Menyiapkan variabel lokal `configuredDirectory` untuk nilai configured directory dengan `configuration[”DatabaseBootstrap:SqlDirectory”]`, yaitu
        // elemen koleksi yang dipilih melalui indeks atau kunci tersebut. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var configuredDirectory = configuration["DatabaseBootstrap:SqlDirectory"];
        // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(configuredDirectory)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ResolveSqlFilePath.
        if (!string.IsNullOrWhiteSpace(configuredDirectory))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(configuredDirectory)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ResolveSqlFilePath.
        {
            // Menjalankan menambahkan `configuredDirectory` ke `candidateRoots` dalam ResolveSqlFilePath.
            candidateRoots.Add(configuredDirectory);
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(configuredDirectory)`; bagian berikut berada di luar batas blok tersebut dalam
        // ResolveSqlFilePath.
        }

        // Menjalankan menambahkan seluruh elemen `[ Path.Combine(AppContext.BaseDirectory, ”artifacts”, ”runtime-sql”),
        // Path.Combine(Directory.GetCurrentDirectory(), ”artifacts”, ”runtime-sql”), Path.Combine(AppContext.BaseDi...` ke `candidateRoots` dalam
        // ResolveSqlFilePath.
        candidateRoots.AddRange(
        // Meneruskan koleksi berisi Path.Combine(AppContext.BaseDirectory, ”artifacts”..., Path.Combine(Directory.GetCurrentDirectory(), ”art...,
        // Path.Combine(AppContext.BaseDirectory, ”..”, ”..”,..., Path.Combine(Directory.GetCurrentDirectory(), ”..”...,
        // Path.Combine(Directory.GetCurrentDirectory(), ”..”..., Path.Combine(AppContext.BaseDirectory, ”database”),
        // Path.Combine(Directory.GetCurrentDirectory(), ”dat..., Path.Combine(AppContext.BaseDirectory, ”..”, ”..”,...,
        // Path.Combine(Directory.GetCurrentDirectory(), ”..”..., Path.Combine(Directory.GetCurrentDirectory(), ”..”... sebagai argumen ke
        // `candidateRoots.AddRange`.
        [
            // Meneruskan `AppContext.BaseDirectory` (nilai base directory) sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”artifacts”` sebagai
            // argumen ke `Path.Combine`; Meneruskan nilai literal `”runtime-sql”` sebagai argumen ke `Path.Combine`.
            Path.Combine(AppContext.BaseDirectory, "artifacts", "runtime-sql"),
            // Meneruskan memanggil `Directory.GetCurrentDirectory` dengan tanpa argumen sebagai argumen ke `Path.Combine`; Meneruskan nilai literal
            // `”artifacts”` sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”runtime-sql”` sebagai argumen ke `Path.Combine`.
            Path.Combine(Directory.GetCurrentDirectory(), "artifacts", "runtime-sql"),
            // Meneruskan `AppContext.BaseDirectory` (nilai base directory) sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”..”` sebagai argumen
            // ke `Path.Combine`; Meneruskan nilai literal `”..”` sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”..”` sebagai argumen ke
            // `Path.Combine`; Meneruskan nilai literal `”..”` sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”..”` sebagai argumen ke
            // `Path.Combine`; Meneruskan nilai literal `”artifacts”` sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”runtime-sql”` sebagai
            // argumen ke `Path.Combine`.
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "artifacts", "runtime-sql"),
            // Meneruskan memanggil `Directory.GetCurrentDirectory` dengan tanpa argumen sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”..”`
            // sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”artifacts”` sebagai argumen ke `Path.Combine`; Meneruskan nilai literal
            // `”runtime-sql”` sebagai argumen ke `Path.Combine`.
            Path.Combine(Directory.GetCurrentDirectory(), "..", "artifacts", "runtime-sql"),
            // Meneruskan memanggil `Directory.GetCurrentDirectory` dengan tanpa argumen sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”..”`
            // sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”..”` sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”artifacts”`
            // sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”runtime-sql”` sebagai argumen ke `Path.Combine`.
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "artifacts", "runtime-sql"),
            // Meneruskan `AppContext.BaseDirectory` (nilai base directory) sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”database”` sebagai
            // argumen ke `Path.Combine`.
            Path.Combine(AppContext.BaseDirectory, "database"),
            // Meneruskan memanggil `Directory.GetCurrentDirectory` dengan tanpa argumen sebagai argumen ke `Path.Combine`; Meneruskan nilai literal
            // `”database”` sebagai argumen ke `Path.Combine`.
            Path.Combine(Directory.GetCurrentDirectory(), "database"),
            // Meneruskan `AppContext.BaseDirectory` (nilai base directory) sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”..”` sebagai argumen
            // ke `Path.Combine`; Meneruskan nilai literal `”..”` sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”..”` sebagai argumen ke
            // `Path.Combine`; Meneruskan nilai literal `”..”` sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”..”` sebagai argumen ke
            // `Path.Combine`; Meneruskan nilai literal `”database”` sebagai argumen ke `Path.Combine`.
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "database"),
            // Meneruskan memanggil `Directory.GetCurrentDirectory` dengan tanpa argumen sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”..”`
            // sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”database”` sebagai argumen ke `Path.Combine`.
            Path.Combine(Directory.GetCurrentDirectory(), "..", "database"),
            // Meneruskan memanggil `Directory.GetCurrentDirectory` dengan tanpa argumen sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”..”`
            // sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”..”` sebagai argumen ke `Path.Combine`; Meneruskan nilai literal `”database”`
            // sebagai argumen ke `Path.Combine`.
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "database")
        // Meneruskan koleksi berisi Path.Combine(AppContext.BaseDirectory, ”artifacts”..., Path.Combine(Directory.GetCurrentDirectory(), ”art...,
        // Path.Combine(AppContext.BaseDirectory, ”..”, ”..”,..., Path.Combine(Directory.GetCurrentDirectory(), ”..”...,
        // Path.Combine(Directory.GetCurrentDirectory(), ”..”..., Path.Combine(AppContext.BaseDirectory, ”database”),
        // Path.Combine(Directory.GetCurrentDirectory(), ”dat..., Path.Combine(AppContext.BaseDirectory, ”..”, ”..”,...,
        // Path.Combine(Directory.GetCurrentDirectory(), ”..”..., Path.Combine(Directory.GetCurrentDirectory(), ”..”... sebagai argumen ke
        // `candidateRoots.AddRange`.
        ]);

        // Mengulangi setiap elemen `candidateRoots`; elemen saat ini disimpan sebagai `root` bertipe `var` untuk diproses oleh badan loop dalam
        // ResolveSqlFilePath.
        foreach (var root in candidateRoots)
        // Membuka scope loop setiap root dari `candidateRoots`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveSqlFilePath.
        {
            // Menyiapkan variabel lokal `fullPath` untuk nilai full path dengan memanggil `Path.GetFullPath` dengan `Path.Combine(root, fileName)`. Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var fullPath = Path.GetFullPath(Path.Combine(root, fileName));
            // Memeriksa hasil pemilihan bersyarat: ketika `requireFile` benar gunakan `File.Exists(fullPath)`, jika tidak gunakan
            // `Directory.Exists(Path.GetDirectoryName(fullPath))`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveSqlFilePath.
            if (requireFile ? File.Exists(fullPath) : Directory.Exists(Path.GetDirectoryName(fullPath)))
            // Membuka scope cabang if untuk kondisi `requireFile ? File.Exists(fullPath) : Directory.Exists(Path.GetDirectoryName(fullPath))`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveSqlFilePath.
            {
                // Mengembalikan `fullPath` (nilai full path) kepada pemanggil dalam ResolveSqlFilePath; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return fullPath;
            // Menutup scope cabang if untuk kondisi `requireFile ? File.Exists(fullPath) : Directory.Exists(Path.GetDirectoryName(fullPath))`; bagian berikut
            // berada di luar batas blok tersebut dalam ResolveSqlFilePath.
            }
        // Menutup scope loop setiap root dari `candidateRoots`; bagian berikut berada di luar batas blok tersebut dalam ResolveSqlFilePath.
        }

        // Menghentikan alur dengan melempar objek baru bertipe `FileNotFoundException` dengan argumen ($”Aset SQL '{fileName}' tidak ditemukan.”) dalam
        // ResolveSqlFilePath; pemanggil atau middleware penanganan error menerima kegagalan ini.
        throw new FileNotFoundException($"Aset SQL '{fileName}' tidak ditemukan.");
    // Menutup scope metode ResolveSqlFilePath; bagian berikut berada di luar batas blok tersebut dalam ResolveSqlFilePath.
    }

    // Mendefinisikan metode `ComputeChecksum` dengan hasil bertipe `string`; operasi ini menangani compute checksum. Masukan: Parameter `bytes` bertipe
    // `byte[]` membawa nilai bytes. Nilai hasil langsung berasal dari menormalisasi `Convert.ToHexString(SHA256.HashData(bytes))` menjadi huruf kecil
    // dengan aturan kultur invariant.
    private static string ComputeChecksum(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

    // Mendefinisikan metode `CreateMigrationDescriptor` dengan hasil bertipe `SqlMigration`; operasi ini menangani create migration descriptor.
    // Masukan: Parameter `version` bertipe `int` membawa nomor versi yang dipakai untuk konsistensi data atau konfigurasi; Parameter `name` bertipe
    // `string` membawa nilai nama; Parameter `path` bertipe `string` membawa nilai path; Parameter `includeSql` bertipe `bool` membawa nilai include
    // SQL.
    private static SqlMigration CreateMigrationDescriptor(
        // Parameter `version` bertipe `int` membawa nomor versi yang dipakai untuk konsistensi data atau konfigurasi.
        int version,
        // Parameter `name` bertipe `string` membawa nilai nama.
        string name,
        // Parameter `path` bertipe `string` membawa nilai path.
        string path,
        // Parameter `includeSql` bertipe `bool` membawa nilai include SQL.
        bool includeSql)
    // Membuka scope metode CreateMigrationDescriptor; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateMigrationDescriptor.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan memanggil `File.ReadAllText` dengan `path`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var sql = File.ReadAllText(path);
        // Menyiapkan variabel lokal `normalized` untuk nilai normalized dengan memanggil `sql.ReplaceLineEndings` dengan `”\n”`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var normalized = sql.ReplaceLineEndings("\n");
        // Menyiapkan variabel lokal `checksums` untuk nilai checksums dengan objek baru bertipe `HashSet<string>` dengan argumen (StringComparer.Ordinal).
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var checksums = new HashSet<string>(StringComparer.Ordinal)
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // CreateMigrationDescriptor.
        {
            // Melanjutkan pengolahan dengan memanggil `ComputeChecksum` dengan `File.ReadAllBytes(path)` dalam CreateMigrationDescriptor.
            ComputeChecksum(File.ReadAllBytes(path)),
            // Melanjutkan pengolahan dengan memanggil `ComputeChecksum` dengan `Encoding.UTF8.GetBytes(normalized)` dalam CreateMigrationDescriptor.
            ComputeChecksum(Encoding.UTF8.GetBytes(normalized)),
            // Melanjutkan pengolahan dengan memanggil `ComputeChecksum` dengan `Encoding.UTF8.GetBytes(normalized.Replace(”\n”, ”\r\n”,
            // StringComparison.Ordinal))` dalam CreateMigrationDescriptor.
            ComputeChecksum(Encoding.UTF8.GetBytes(normalized.Replace("\n", "\r\n", StringComparison.Ordinal)))
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateMigrationDescriptor.
        };

        // Menyiapkan variabel lokal `canonicalChecksum` untuk nilai canonical checksum dengan memanggil `ComputeChecksum` dengan
        // `Encoding.UTF8.GetBytes(normalized)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var canonicalChecksum = ComputeChecksum(Encoding.UTF8.GetBytes(normalized));
        // Mengembalikan objek baru bertipe `SqlMigration` dengan argumen ( version, name, canonicalChecksum, includeSql ? sql : string.Empty, checksums)
        // kepada pemanggil dalam CreateMigrationDescriptor; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new SqlMigration(
            // Meneruskan `version` (nomor versi yang dipakai untuk konsistensi data atau konfigurasi) sebagai argumen ke konstruktor `SqlMigration`.
            version,
            // Meneruskan `name` (nilai nama) sebagai argumen ke konstruktor `SqlMigration`.
            name,
            // Meneruskan `canonicalChecksum` (nilai canonical checksum) sebagai argumen ke konstruktor `SqlMigration`.
            canonicalChecksum,
            // Meneruskan hasil pemilihan bersyarat: ketika `includeSql` benar gunakan `sql`, jika tidak gunakan `string.Empty` sebagai argumen ke konstruktor
            // `SqlMigration`.
            includeSql ? sql : string.Empty,
            // Meneruskan `checksums` (nilai checksums) sebagai argumen ke konstruktor `SqlMigration`.
            checksums);
    // Menutup scope metode CreateMigrationDescriptor; bagian berikut berada di luar batas blok tersebut dalam CreateMigrationDescriptor.
    }

    // Mendefinisikan metode `EnsureChecksumMatches` dengan hasil bertipe `void`; operasi ini menangani ensure checksum matches. Masukan: Parameter
    // `applied` bertipe `AppliedMigration` membawa nilai applied; Parameter `expected` bertipe `SqlMigration` membawa nilai yang diharapkan.
    private static void EnsureChecksumMatches(AppliedMigration applied, SqlMigration expected)
    // Membuka scope metode EnsureChecksumMatches; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam EnsureChecksumMatches.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!string.Equals(applied.Name, expected.Name, StringComparison.Ordinal)`
        // dan `!expected.CompatibleChecksums.Contains(applied.Checksum)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam EnsureChecksumMatches.
        if (!string.Equals(applied.Name, expected.Name, StringComparison.Ordinal) ||
            // Menggunakan kebalikan kondisi `expected.CompatibleChecksums.Contains(applied.Checksum)` sebagai bagian ekspresi yang sedang disusun dalam
            // EnsureChecksumMatches.
            !expected.CompatibleChecksums.Contains(applied.Checksum))
        // Membuka scope cabang if untuk kondisi `!string.Equals(applied.Name, expected.Name, StringComparison.Ordinal) ||
        // !expected.CompatibleChecksums.Contains(applied.Checksum)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // EnsureChecksumMatches.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( $”Checksum migrasi V{expected.Version} berbeda.
            // Migrasi yang sudah diterapkan tidak boleh diedit.”) dalam EnsureChecksumMatches; pemanggil atau middleware penanganan error menerima kegagalan
            // ini.
            throw new InvalidOperationException(
                // Meneruskan teks interpolasi `$”Checksum migrasi V{expected.Version} berbeda. Migrasi yang sudah diterapkan tidak boleh diedit.”`; nilai ekspresi
                // di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke konstruktor `InvalidOperationException`.
                $"Checksum migrasi V{expected.Version} berbeda. Migrasi yang sudah diterapkan tidak boleh diedit.");
        // Menutup scope cabang if untuk kondisi `!string.Equals(applied.Name, expected.Name, StringComparison.Ordinal) ||
        // !expected.CompatibleChecksums.Contains(applied.Checksum)`; bagian berikut berada di luar batas blok tersebut dalam EnsureChecksumMatches.
        }
    // Menutup scope metode EnsureChecksumMatches; bagian berikut berada di luar batas blok tersebut dalam EnsureChecksumMatches.
    }

    // Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `SqlMigration`; sealed mencegah tipe ini diturunkan lagi.
    private sealed record SqlMigration(
        // Parameter `Version` bertipe `int` membawa nomor versi yang dipakai untuk konsistensi data atau konfigurasi.
        int Version,
        // Parameter `Name` bertipe `string` membawa nilai nama.
        string Name,
        // Parameter `Checksum` bertipe `string` membawa nilai checksum.
        string Checksum,
        // Parameter `Sql` bertipe `string` membawa nilai SQL.
        string Sql,
        // Parameter `CompatibleChecksums` bertipe `IReadOnlySet<string>` membawa nilai compatible checksums.
        IReadOnlySet<string> CompatibleChecksums);
    // Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AppliedMigration`; sealed mencegah tipe ini diturunkan lagi.
    private sealed record AppliedMigration(int Version, string Name, string Checksum);
    // Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `DatabaseState`; sealed mencegah tipe ini diturunkan lagi.
    private sealed record DatabaseState(bool HasApplicationSchema, bool HasHistory);
// Menutup scope tipe DatabaseInitialization; bagian berikut berada di luar batas blok tersebut.
}
