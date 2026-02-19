// Fungsi file: Mendefinisikan ViewModel/DTO UI untuk domain HomeViewModels.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// Menyatakan peran utama tipe HomeIndexViewModel pada modul ini.
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

