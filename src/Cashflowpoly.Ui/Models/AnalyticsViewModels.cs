using System.Text.Json;

namespace Cashflowpoly.Ui.Models;

public sealed class AnalyticsSearchViewModel
{
    public string SessionId { get; set; } = string.Empty;
    public bool IsInstructor { get; set; } = true;
    public List<SessionListItemDto> Sessions { get; set; } = new();
    public AnalyticsSessionResponseDto? Result { get; set; }
    public RulesetAnalyticsSummaryResponseDto? RulesetResult { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RulesetErrorMessage { get; set; }
    public string? SessionLookupErrorMessage { get; set; }
}

public sealed class SessionListViewModel
{
    public List<SessionListItemDto> Items { get; init; } = new();
    public string? ErrorMessage { get; init; }
}

public sealed class SessionDetailViewModel
{
    public Guid SessionId { get; init; }
    public AnalyticsSessionResponseDto? Analytics { get; init; }
    public List<SessionTimelineEventViewModel> Timeline { get; init; } = new();
    public string? TimelineErrorMessage { get; init; }
    public string? SessionStatus { get; init; }
    public bool IsDevelopment { get; init; }
    public string? ErrorMessage { get; init; }
}

public sealed class SessionTimelineEventViewModel
{
    public DateTimeOffset Timestamp { get; init; }
    public long SequenceNumber { get; init; }
    public int DayIndex { get; init; }
    public string Weekday { get; init; } = string.Empty;
    public int TurnNumber { get; init; }
    public string ActorType { get; init; } = string.Empty;
    public Guid? PlayerId { get; init; }
    public string ActionType { get; init; } = string.Empty;
    public string FlowLabel { get; init; } = string.Empty;
    public string FlowDescription { get; init; } = string.Empty;
}

public sealed class PlayerDetailViewModel
{
    public Guid SessionId { get; init; }
    public Guid PlayerId { get; init; }
    public AnalyticsByPlayerItemDto? Summary { get; init; }
    public List<TransactionHistoryItemDto> Transactions { get; init; } = new();
    public JsonElement? GameplayRaw { get; init; }
    public JsonElement? GameplayDerived { get; init; }
    public DateTimeOffset? GameplayComputedAt { get; init; }
    public string? GameplayErrorMessage { get; init; }
    public string? ErrorMessage { get; init; }
}

public sealed class SessionRulesetViewModel
{
    public Guid SessionId { get; set; }
    public List<RulesetListItemDto> Rulesets { get; set; } = new();
    public Guid? SelectedRulesetId { get; set; }
    public int? SelectedVersion { get; set; }
    public string? ErrorMessage { get; set; }
}

public sealed class PlayerDirectoryViewModel
{
    public List<PlayerResponseDto> Players { get; init; } = new();
    public List<PlayerSessionGroupViewModel> SessionGroups { get; init; } = new();
    public string? ErrorMessage { get; set; }
}

public sealed class PlayerSessionGroupViewModel
{
    public Guid SessionId { get; init; }
    public string SessionName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? EndedAt { get; init; }
    public List<PlayerSessionEntryViewModel> Players { get; init; } = new();
}

public sealed class PlayerSessionEntryViewModel
{
    public Guid PlayerId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public double CashInTotal { get; init; }
    public double CashOutTotal { get; init; }
    public double DonationTotal { get; init; }
    public int GoldQty { get; init; }
    public double HappinessPointsTotal { get; init; }
}
