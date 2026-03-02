// Fungsi file: Mendefinisikan ViewModel untuk halaman beranda (dashboard) yang menampilkan ringkasan statistik sesi, pemain, dan ruleset.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// ViewModel halaman beranda yang memuat jumlah sesi aktif, total sesi,
/// total pemain, total ruleset, waktu sinkronisasi terakhir, dan pesan error opsional.
/// </summary>
public sealed class HomeIndexViewModel
{
    public int ActiveSessions { get; init; }
    public int TotalSessions { get; init; }
    public int TotalPlayers { get; init; }
    public int TotalRulesets { get; init; }
    public DateTimeOffset LastSyncedAt { get; init; } = DateTimeOffset.UtcNow;
    public string? ErrorMessage { get; init; }
}

