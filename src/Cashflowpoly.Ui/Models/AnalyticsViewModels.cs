// Fungsi file: Mendefinisikan ViewModel/DTO UI untuk domain AnalyticsViewModels.
using System.Text.Json;

namespace Cashflowpoly.Ui.Models;

/// <summary>
/// Menyatakan peran utama tipe AnalyticsSearchViewModel pada modul ini.
/// </summary>
public sealed class AnalyticsSearchViewModel
{
    public string SessionId { get; set; } = string.Empty;
    public bool IsInstructor { get; set; } = true;
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    public List<SessionListItemDto> Sessions { get; set; } = new();
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    public Dictionary<Guid, string> PlayerDisplayNames { get; set; } = new();
    public AnalyticsSessionResponseDto? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SessionLookupErrorMessage { get; set; }
}

/// <summary>
/// Menyatakan peran utama tipe SessionListViewModel pada modul ini.
/// </summary>
public sealed class SessionListViewModel
{
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    public List<SessionListItemDto> Items { get; init; } = new();
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Menyatakan peran utama tipe SessionDetailViewModel pada modul ini.
/// </summary>
public sealed class SessionDetailViewModel
{
    public Guid SessionId { get; init; }
    public AnalyticsSessionResponseDto? Analytics { get; init; }
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    public Dictionary<Guid, string> PlayerDisplayNames { get; init; } = new();
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    public List<SessionTimelineEventViewModel> Timeline { get; init; } = new();
    public string? TimelineErrorMessage { get; init; }
    public string? SessionStatus { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Menyatakan peran utama tipe SessionTimelineEventViewModel pada modul ini.
/// </summary>
public sealed class SessionTimelineEventViewModel
{
    public DateTimeOffset Timestamp { get; init; }
    public long SequenceNumber { get; init; }
    public int DayIndex { get; init; }
    public string Weekday { get; init; } = string.Empty;
    public int TurnNumber { get; init; }
    public string ActorType { get; init; } = string.Empty;
    public Guid? PlayerId { get; init; }
    public string? PlayerDisplayName { get; set; }
    public string ActionType { get; init; } = string.Empty;
    public string FlowLabel { get; init; } = string.Empty;
    public string FlowDescription { get; init; } = string.Empty;
}

/// <summary>
/// Menyatakan peran utama tipe PlayerDetailViewModel pada modul ini.
/// </summary>
public sealed class PlayerDetailViewModel
{
    public Guid SessionId { get; init; }
    public Guid PlayerId { get; init; }
    public string? PlayerDisplayName { get; init; }
    public AnalyticsByPlayerItemDto? Summary { get; init; }
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    public List<TransactionHistoryItemDto> Transactions { get; init; } = new();
    public JsonElement? GameplayRaw { get; init; }
    public JsonElement? GameplayDerived { get; init; }
    public DateTimeOffset? GameplayComputedAt { get; init; }
    public string? GameplayErrorMessage { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Menyatakan peran utama tipe SessionRulesetViewModel pada modul ini.
/// </summary>
public sealed class SessionRulesetViewModel
{
    public Guid SessionId { get; set; }
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    public List<RulesetListItemDto> Rulesets { get; set; } = new();
    public Guid? SelectedRulesetId { get; set; }
    public int? SelectedVersion { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Menyatakan peran utama tipe PlayerDirectoryViewModel pada modul ini.
/// </summary>
public sealed class PlayerDirectoryViewModel
{
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    public List<PlayerResponseDto> Players { get; init; } = new();
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    public List<PlayerSessionGroupViewModel> SessionGroups { get; init; } = new();
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Menyatakan peran utama tipe PlayerSessionGroupViewModel pada modul ini.
/// </summary>
public sealed class PlayerSessionGroupViewModel
{
    public Guid SessionId { get; init; }
    public string SessionName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? EndedAt { get; init; }
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    public List<PlayerSessionEntryViewModel> Players { get; init; } = new();
}

/// <summary>
/// Menyatakan peran utama tipe PlayerSessionEntryViewModel pada modul ini.
/// </summary>
public sealed class PlayerSessionEntryViewModel
{
    public Guid PlayerId { get; init; }
    public int JoinOrder { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public double CashInTotal { get; init; }
    public double CashOutTotal { get; init; }
    public double DonationTotal { get; init; }
    public double DonationPointsTotal { get; init; }
    public double PensionPointsTotal { get; init; }
    public int GoldQty { get; init; }
    public double HappinessPointsTotal { get; init; }
}
