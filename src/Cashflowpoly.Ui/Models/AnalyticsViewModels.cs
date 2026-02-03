using System.Text.Json;

namespace Cashflowpoly.Ui.Models;

public sealed class AnalyticsSearchViewModel
{
    public string SessionId { get; set; } = string.Empty;
    public AnalyticsSessionResponseDto? Result { get; set; }
    public string? ErrorMessage { get; set; }
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
    public string? ErrorMessage { get; init; }
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
    public Guid SessionId { get; init; }
    public List<RulesetListItemDto> Rulesets { get; init; } = new();
    public Guid? SelectedRulesetId { get; set; }
    public int? SelectedVersion { get; set; }
    public string? ErrorMessage { get; set; }
}

public sealed class PlayerDirectoryViewModel
{
    public List<PlayerResponseDto> Players { get; init; } = new();
    public string DisplayName { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}
