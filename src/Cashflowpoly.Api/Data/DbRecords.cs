namespace Cashflowpoly.Api.Data;

internal sealed record RulesetDb(
    Guid RulesetId,
    string Name,
    string? Description,
    bool IsArchived,
    DateTimeOffset CreatedAt,
    string? CreatedBy);

internal sealed record RulesetVersionDb(
    Guid RulesetVersionId,
    Guid RulesetId,
    int Version,
    string Status,
    string ConfigJson,
    string ConfigHash,
    DateTimeOffset CreatedAt,
    string? CreatedBy);

internal sealed record SessionDb(
    Guid SessionId,
    string SessionName,
    string Mode,
    string Status,
    DateTimeOffset? StartedAt,
    DateTimeOffset? EndedAt,
    DateTimeOffset CreatedAt);

internal sealed record EventDb(
    Guid EventPk,
    Guid EventId,
    Guid SessionId,
    Guid? PlayerId,
    string ActorType,
    DateTimeOffset Timestamp,
    int DayIndex,
    string Weekday,
    int TurnNumber,
    long SequenceNumber,
    string ActionType,
    Guid RulesetVersionId,
    string Payload,
    DateTimeOffset ReceivedAt,
    string? ClientRequestId);

internal sealed record CashflowProjectionDb(
    Guid ProjectionId,
    Guid SessionId,
    Guid PlayerId,
    Guid EventPk,
    Guid EventId,
    DateTimeOffset Timestamp,
    string Direction,
    int Amount,
    string Category,
    string? Counterparty,
    string? Reference,
    string? Note);

internal sealed record MetricSnapshotDb(
    Guid MetricSnapshotId,
    Guid SessionId,
    Guid? PlayerId,
    DateTimeOffset ComputedAt,
    string MetricName,
    double? MetricValueNumeric,
    string? MetricValueJson,
    Guid RulesetVersionId);
