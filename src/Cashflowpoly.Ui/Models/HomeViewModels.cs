// Fungsi file: Mendefinisikan model tampilan dan state UI untuk HomeViewModels.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Models` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// ViewModel halaman beranda yang memuat jumlah sesi aktif, total sesi,
/// total pemain, total ruleset, waktu sinkronisasi terakhir, dan pesan error opsional.
/// </summary>
// Mendefinisikan tipe class `HomeIndexViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class HomeIndexViewModel
{
    public int? ActiveSessions { get; init; }
    public int? TotalSessions { get; init; }
    public int? TotalPlayers { get; init; }
    public int? TotalRulesets { get; init; }
    public DateTimeOffset LastSyncedAt { get; init; } = DateTimeOffset.UtcNow;
    public string? ErrorMessage { get; init; }
}

