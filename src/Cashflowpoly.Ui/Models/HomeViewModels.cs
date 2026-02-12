namespace Cashflowpoly.Ui.Models;

public sealed class HomeIndexViewModel
{
    public int ActiveSessions { get; init; }
    public int TotalSessions { get; init; }
    public int TotalPlayers { get; init; }
    public int TotalRulesets { get; init; }
    public DateTimeOffset LastSyncedAt { get; init; } = DateTimeOffset.UtcNow;
    public string? ErrorMessage { get; init; }
}

