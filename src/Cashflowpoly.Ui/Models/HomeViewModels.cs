// Fungsi file: Mendefinisikan model tampilan dan state UI untuk HomeViewModels.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Models` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// ViewModel halaman beranda yang memuat jumlah sesi aktif, total sesi,
/// total pemain, total ruleset, waktu sinkronisasi terakhir, dan pesan error opsional.
/// </summary>
// Mendefinisikan tipe class `HomeIndexViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class HomeIndexViewModel
// Membuka scope tipe HomeIndexViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `ActiveSessions` bertipe `int` untuk nilai aktif sessions; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int ActiveSessions { get; init; }
    // Mendefinisikan properti `TotalSessions` bertipe `int` untuk nilai total sessions; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int TotalSessions { get; init; }
    // Mendefinisikan properti `TotalPlayers` bertipe `int` untuk nilai total pemain; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int TotalPlayers { get; init; }
    // Mendefinisikan properti `TotalRulesets` bertipe `int` untuk nilai total aturan; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int TotalRulesets { get; init; }
    // Mendefinisikan properti `LastSyncedAt` bertipe `DateTimeOffset` untuk nilai last synced at; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya `DateTimeOffset.UtcNow`, yaitu waktu UTC saat operasi dilakukan.
    public DateTimeOffset LastSyncedAt { get; init; } = DateTimeOffset.UtcNow;
    // Mendefinisikan properti `ErrorMessage` bertipe `string?` untuk nilai kesalahan pesan; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public string? ErrorMessage { get; init; }
// Menutup scope tipe HomeIndexViewModel; bagian berikut berada di luar batas blok tersebut.
}

