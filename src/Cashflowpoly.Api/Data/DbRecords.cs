namespace Cashflowpoly.Api.Data;

public sealed class RulesetDb
{
    public Guid RulesetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsArchived { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public sealed class RulesetVersionDb
{
    public Guid RulesetVersionId { get; set; }
    public Guid RulesetId { get; set; }
    public int Version { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ConfigJson { get; set; } = string.Empty;
    public string ConfigHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public sealed class SessionDb
{
    public Guid SessionId { get; set; }
    public string SessionName { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class PlayerDb
{
    public Guid PlayerId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class EventDb
{
    public Guid EventPk { get; set; }
    public Guid EventId { get; set; }
    public Guid SessionId { get; set; }
    public Guid? PlayerId { get; set; }
    public string ActorType { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
    public int DayIndex { get; set; }
    public string Weekday { get; set; } = string.Empty;
    public int TurnNumber { get; set; }
    public long SequenceNumber { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public Guid RulesetVersionId { get; set; }
    public string Payload { get; set; } = string.Empty;
    public DateTimeOffset ReceivedAt { get; set; }
    public string? ClientRequestId { get; set; }
}

public sealed class CashflowProjectionDb
{
    public Guid ProjectionId { get; set; }
    public Guid SessionId { get; set; }
    public Guid PlayerId { get; set; }
    public Guid EventPk { get; set; }
    public Guid EventId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string Direction { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Counterparty { get; set; }
    public string? Reference { get; set; }
    public string? Note { get; set; }
}

public sealed class MetricSnapshotDb
{
    public Guid MetricSnapshotId { get; set; }
    public Guid SessionId { get; set; }
    public Guid? PlayerId { get; set; }
    public DateTimeOffset ComputedAt { get; set; }
    public string MetricName { get; set; } = string.Empty;
    public double? MetricValueNumeric { get; set; }
    public string? MetricValueJson { get; set; }
    public Guid RulesetVersionId { get; set; }
}
