// Fungsi file: Menghapus log yang melewati kebijakan retensi database secara berkala.
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Infrastructure;

// Mendefinisikan tipe class `LogRetentionWorker` yang mewarisi atau menerapkan `BackgroundService`; sealed mencegah tipe ini diturunkan lagi.
internal sealed class LogRetentionWorker : BackgroundService
// Membuka scope tipe LogRetentionWorker; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `NpgsqlDataSource`: `_dataSource` menyimpan sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang
    // koneksi. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly NpgsqlDataSource _dataSource;
    // Mendeklarasikan field bertipe `ILogger<LogRetentionWorker>`: `_logger` menyimpan pencatat log terstruktur untuk memantau proses dan mendiagnosis
    // kegagalan. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor.
    private readonly ILogger<LogRetentionWorker> _logger;

    // Mendefinisikan konstruktor LogRetentionWorker yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `dataSource` bertipe `NpgsqlDataSource` membawa sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi; Parameter
    // `logger` bertipe `ILogger<LogRetentionWorker>` membawa pencatat log terstruktur untuk memantau proses dan mendiagnosis kegagalan.
    public LogRetentionWorker(NpgsqlDataSource dataSource, ILogger<LogRetentionWorker> logger)
    // Membuka scope konstruktor LogRetentionWorker; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LogRetentionWorker.
    {
        // Memperbarui `_dataSource` menggunakan `dataSource` (sumber koneksi PostgreSQL yang mengelola pembuatan dan penggunaan ulang koneksi) dalam
        // LogRetentionWorker.
        _dataSource = dataSource;
        // Memperbarui `_logger` menggunakan `logger` (pencatat log terstruktur untuk memantau proses dan mendiagnosis kegagalan) dalam LogRetentionWorker.
        _logger = logger;
    // Menutup scope konstruktor LogRetentionWorker; bagian berikut berada di luar batas blok tersebut dalam LogRetentionWorker.
    }

    // Mendefinisikan metode `ExecuteAsync` dengan hasil bertipe `Task`; operasi ini menangani execute asinkron. async memungkinkan metode menunggu
    // operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `stoppingToken` bertipe `CancellationToken` membawa
    // sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    // Membuka scope metode ExecuteAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ExecuteAsync.
    {
        // Menjalankan hasil operasi asinkron memanggil `PurgeAsync` dengan `stoppingToken`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai dalam ExecuteAsync.
        await PurgeAsync(stoppingToken);
        // Menyiapkan variabel lokal `timer` untuk nilai timer dengan objek baru bertipe `PeriodicTimer` dengan argumen (TimeSpan.FromHours(24)). Tipe
        // variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var timer = new PeriodicTimer(TimeSpan.FromHours(24));
        // Mengulangi blok selama hasil operasi asinkron memanggil `timer.WaitForNextTickAsync` dengan `stoppingToken`; await menunggu hasil tanpa memblokir
        // thread selama operasi belum selesai; kondisi diperiksa lagi sebelum setiap iterasi dalam ExecuteAsync.
        while (await timer.WaitForNextTickAsync(stoppingToken))
        // Membuka scope loop selama `await timer.WaitForNextTickAsync(stoppingToken)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ExecuteAsync.
        {
            // Menjalankan hasil operasi asinkron memanggil `PurgeAsync` dengan `stoppingToken`; await menunggu hasil tanpa memblokir thread selama operasi
            // belum selesai dalam ExecuteAsync.
            await PurgeAsync(stoppingToken);
        // Menutup scope loop selama `await timer.WaitForNextTickAsync(stoppingToken)`; bagian berikut berada di luar batas blok tersebut dalam
        // ExecuteAsync.
        }
    // Menutup scope metode ExecuteAsync; bagian berikut berada di luar batas blok tersebut dalam ExecuteAsync.
    }

    // Mendefinisikan metode `PurgeAsync` dengan hasil bertipe `Task`; operasi ini menangani purge asinkron. async memungkinkan metode menunggu operasi
    // I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal
    // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private async Task PurgeAsync(CancellationToken cancellationToken)
    // Membuka scope metode PurgeAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam PurgeAsync.
    {
        // Memulai blok try dalam PurgeAsync; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam PurgeAsync.
        {
            // Menyiapkan variabel lokal `connection` untuk koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data dengan hasil operasi
            // asinkron membuka koneksi PostgreSQL melalui `_dataSource` menggunakan `cancellationToken`; await menunggu hasil tanpa memblokir thread selama
            // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            // Menyiapkan variabel lokal `deleted` untuk nilai deleted dengan hasil operasi asinkron menjalankan perintah basis data melalui `connection` dengan
            // `new CommandDefinition( ”select coalesce(sum(purge_logs_by_retention(table_name)), 0)::int from log_retention_policies;”, cancellationToken:
            // cancellationToken)` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum selesai. Tipe variabel
            // disimpulkan dari ekspresi nilai awal.
            var deleted = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                // Meneruskan nilai literal `”select coalesce(sum(purge_logs_by_retention(table_name)), 0)::int from log_retention_policies;”` sebagai argumen ke
                // konstruktor `CommandDefinition`.
                "select coalesce(sum(purge_logs_by_retention(table_name)), 0)::int from log_retention_policies;",
                // Meneruskan `cancellationToken` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti)
                // sebagai argumen bernama `cancellationToken`.
                cancellationToken: cancellationToken));
            // Memeriksa pemeriksaan lebih besar antara `deleted` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam PurgeAsync.
            if (deleted > 0)
            // Membuka scope cabang if untuk kondisi `deleted > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam PurgeAsync.
            {
                // Menjalankan mencatat log tingkat Information melalui `_logger` dengan pesan dan data `”Purged {DeletedRows} expired operational log rows”`,
                // `deleted` dalam PurgeAsync.
                _logger.LogInformation("Purged {DeletedRows} expired operational log rows", deleted);
            // Menutup scope cabang if untuk kondisi `deleted > 0`; bagian berikut berada di luar batas blok tersebut dalam PurgeAsync.
            }
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam PurgeAsync.
        }
        // Menangani exception `OperationCanceledException` melalui variabel hanya jika filter `cancellationToken.IsCancellationRequested` terpenuhi dalam
        // PurgeAsync.
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam PurgeAsync.
        {
            // Shutdown normal aplikasi.
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam PurgeAsync.
        }
        // Menangani exception `Exception` melalui variabel exception dalam PurgeAsync.
        catch (Exception exception)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam PurgeAsync.
        {
            // Menjalankan mencatat log tingkat Error melalui `_logger` dengan pesan dan data `exception`, `”Failed to purge expired operational logs”` dalam
            // PurgeAsync.
            _logger.LogError(exception, "Failed to purge expired operational logs");
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam PurgeAsync.
        }
    // Menutup scope metode PurgeAsync; bagian berikut berada di luar batas blok tersebut dalam PurgeAsync.
    }
// Menutup scope tipe LogRetentionWorker; bagian berikut berada di luar batas blok tersebut.
}
