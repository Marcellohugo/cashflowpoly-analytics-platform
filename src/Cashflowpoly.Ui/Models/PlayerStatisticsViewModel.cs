// Fungsi file: Menyimpan histori dan titik grafik pemain dari analitika setiap sesi yang diikuti.
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Infrastructure;

namespace Cashflowpoly.Ui.Models;

public sealed class PlayerStatisticsViewModel
{
    public Guid PlayerId { get; init; }
    public string? PlayerName { get; init; }
    public List<PlayerResponse> AvailablePlayers { get; init; } = [];
    public string Mode { get; init; } = "PEMULA";
    public string Status { get; init; } = "ALL";
    public int? TotalSessions { get; init; }
    public List<PlayerStatisticsSession> Sessions { get; init; } = [];
    public List<PlayerStatisticsChart> Charts { get; init; } = [];
    public string? ErrorMessage { get; init; }
}

public sealed record PlayerStatisticsSession(SessionListItem Session, GameplayMetricsResponse? Gameplay);
public sealed record PlayerStatisticsPoint(string Label, double? Value, PlayerMetricPresentation Presentation);
public sealed record PlayerStatisticsChart(
    string Key, string Group, string Title, string Unit, string Source, string Json,
    IReadOnlyList<PlayerStatisticsPoint> Points);
