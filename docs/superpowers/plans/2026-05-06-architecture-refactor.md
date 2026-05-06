# Architecture Refactor Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Refactor Cashflowpoly platform from procedural static-class architecture to DI-injectable layered architecture with shared Contracts project, EF Core migrations, and OpenTelemetry metrics.

**Architecture:** Add `Cashflowpoly.Contracts` project for shared DTOs; convert all static domain classes to injectable services with interfaces; extract fat controller methods into `AnalyticsService` and `EventIngestionService`; replace custom migration system with EF Core; replace in-memory metrics tracker with OpenTelemetry+Prometheus.

**Tech Stack:** .NET 10, ASP.NET Core MVC/API, Dapper (queries), EF Core 9 + Npgsql (migrations only), xUnit, Testcontainers, OpenTelemetry

---

## File Map

### New files
| Path | Purpose |
|------|---------|
| `src/Cashflowpoly.Contracts/Cashflowpoly.Contracts.csproj` | Shared class library |
| `src/Cashflowpoly.Contracts/Dtos.cs` | All shared request/response DTOs |
| `src/Cashflowpoly.Contracts/ErrorResponse.cs` | Shared error DTO |
| `src/Cashflowpoly.Api/Domain/Interfaces/IHappinessCalculator.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/IIngredientInventoryCalculator.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/IPrimaryNeedComplianceEvaluator.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/ISessionMetricCalculator.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/IMetricSnapshotBuilder.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/IPlayerOrdering.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/IScoreCalculator.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/IAnalyticsPayloadReader.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/IEventPayloadReader.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/IGameplaySnapshotBuilder.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/IEventCashflowProjectionBuilder.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/IEventRecordMapper.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/IEventValidationDetailsSerializer.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/IEventShapeValidator.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/IEventActionValidators.cs` | All 5 event action validator interfaces |
| `src/Cashflowpoly.Api/Domain/Interfaces/IEventDerivedStateCalculator.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/IEventPlayerBalanceCalculator.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/IEventInsuranceOffsetBuilder.cs` | Interface |
| `src/Cashflowpoly.Api/Domain/Interfaces/IGameplayCalculators.cs` | All 9 gameplay calc interfaces |
| `src/Cashflowpoly.Api/Services/IAnalyticsService.cs` | Analytics service interface |
| `src/Cashflowpoly.Api/Services/AnalyticsService.cs` | Analytics service implementation |
| `src/Cashflowpoly.Api/Services/IEventIngestionService.cs` | Event ingestion interface |
| `src/Cashflowpoly.Api/Services/EventIngestionService.cs` | Event ingestion implementation |
| `src/Cashflowpoly.Api/Data/AppDbContext.cs` | EF Core DbContext for migrations |
| `src/Cashflowpoly.Api/Infrastructure/Telemetry/AppMetrics.cs` | OTel metric definitions |

### Modified files
| Path | Change |
|------|--------|
| `Cashflowpoly.sln` | Add Contracts project |
| `src/Cashflowpoly.Api/Cashflowpoly.Api.csproj` | Add EF Core + OTel packages, ref Contracts |
| `src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj` | Ref Contracts |
| `src/Cashflowpoly.Api/Domain/*.cs` (37 files) | Add interface, rename class, remove `static` |
| `src/Cashflowpoly.Api/Controllers/AnalyticsController.cs` | Inject IAnalyticsService, delete private methods |
| `src/Cashflowpoly.Api/Controllers/EventsController.cs` | Inject IEventIngestionService, delete business logic |
| `src/Cashflowpoly.Api/Program.cs` | Register new services, add OTel, remove old registrations |
| `src/Cashflowpoly.Ui/Infrastructure/UiText.cs` | Accept `string culture` instead of `HttpContext` |
| `tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj` | Ref Contracts |

### Deleted files
| Path | Reason |
|------|--------|
| `src/Cashflowpoly.Api/Models/ApiDtos.cs` | Moved to Contracts |
| `src/Cashflowpoly.Api/Models/ErrorResponse.cs` | Moved to Contracts |
| `src/Cashflowpoly.Ui/Models/ApiDtos.cs` | Replaced by Contracts reference |
| `src/Cashflowpoly.Api/Data/DatabaseMigration.cs` | Replaced by EF Core |
| `src/Cashflowpoly.Api/Data/AuthSchemaMigrator.cs` | Replaced by EF Core |
| `src/Cashflowpoly.Api/Data/AuthSchemaMigrations.cs` | Replaced by EF Core |
| `src/Cashflowpoly.Api/Data/AuthSchemaBootstrapper.cs` | Replaced by EF Core |
| `src/Cashflowpoly.Api/Infrastructure/OperationalMetricsTracker.cs` | Replaced by OTel |
| `src/Cashflowpoly.Api/Security/LegacyApiCompatibilityHelper.cs` | Legacy middleware removed |
| `tests/Cashflowpoly.Api.Tests/AuthSchemaMigrationTests.cs` | Tests deleted migration system |
| `tests/Cashflowpoly.Api.Tests/LegacyApiCompatibilityHelperTests.cs` | Tests deleted helper |

---

## Task 1: Create Cashflowpoly.Contracts Project

**Files:**
- Create: `src/Cashflowpoly.Contracts/Cashflowpoly.Contracts.csproj`
- Create: `src/Cashflowpoly.Contracts/Dtos.cs`
- Create: `src/Cashflowpoly.Contracts/ErrorResponse.cs`
- Modify: `Cashflowpoly.sln`

- [ ] **Step 1: Create the .csproj file**

```xml
<!-- src/Cashflowpoly.Contracts/Cashflowpoly.Contracts.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="System.Text.Json" Version="9.0.0" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Create ErrorResponse.cs in Contracts**

```csharp
// src/Cashflowpoly.Contracts/ErrorResponse.cs
using System.Text.Json.Serialization;

namespace Cashflowpoly.Contracts;

public sealed record ErrorDetail(
    [property: JsonPropertyName("field")] string Field,
    [property: JsonPropertyName("issue")] string Issue);

public sealed record ErrorResponse(
    [property: JsonPropertyName("error_code")] string ErrorCode,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("details")] List<ErrorDetail> Details,
    [property: JsonPropertyName("trace_id")] string TraceId);
```

- [ ] **Step 3: Create Dtos.cs — copy all DTOs from Api/Models/ApiDtos.cs, change namespace to `Cashflowpoly.Contracts`, remove XML doc comments**

Copy every `public sealed record` from `src/Cashflowpoly.Api/Models/ApiDtos.cs` into this file, changing the namespace:

```csharp
// src/Cashflowpoly.Contracts/Dtos.cs
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cashflowpoly.Contracts;

public sealed record CreateSessionRequest(
    [property: JsonPropertyName("session_name")] string SessionName,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId);

public sealed record CreateSessionResponse([property: JsonPropertyName("session_id")] Guid SessionId);

public sealed record SessionStatusResponse([property: JsonPropertyName("status")] string Status);

public sealed record CreatePlayerRequest(
    [property: JsonPropertyName("display_name")] string DisplayName,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password);

public sealed record PlayerResponse(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("display_name")] string DisplayName);

public sealed record PlayerListResponse([property: JsonPropertyName("items")] List<PlayerResponse> Items);

public sealed record AddSessionPlayerRequest(
    [property: JsonPropertyName("player_id")] Guid? PlayerId,
    [property: JsonPropertyName("username")] string? Username,
    [property: JsonPropertyName("join_order")] int? JoinOrder,
    [property: JsonPropertyName("role")] string? Role);

public sealed record AddSessionPlayerResponse(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("join_order")] int JoinOrder);

public sealed record SessionListItem(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("session_name")] string SessionName,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("started_at")] DateTimeOffset? StartedAt,
    [property: JsonPropertyName("ended_at")] DateTimeOffset? EndedAt);

public sealed record SessionListResponse([property: JsonPropertyName("items")] List<SessionListItem> Items);

public sealed record ActivateRulesetRequest(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("version")] int Version);

public sealed record ActivateRulesetResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId);

public sealed record CreateRulesetRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("config")] JsonElement Config);

public sealed record UpdateRulesetRequest(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("config")] JsonElement? Config);

public sealed record CreateRulesetResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("version")] int Version);

public sealed record RulesetListItem(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("latest_version")] int LatestVersion,
    [property: JsonPropertyName("status")] string Status);

public sealed record RulesetListResponse([property: JsonPropertyName("items")] List<RulesetListItem> Items);

public sealed record RulesetVersionItem(
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt);

public sealed record RulesetDetailResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("versions")] List<RulesetVersionItem> Versions,
    [property: JsonPropertyName("config_json")] JsonElement? ConfigJson);

public sealed record RulesetComponentsResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("mode")] string? Mode,
    [property: JsonPropertyName("component_catalog")] JsonElement? ComponentCatalog);

public sealed record DefaultRulesetComponentItem(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("version")] int Version,
    [property: JsonPropertyName("mode")] string? Mode,
    [property: JsonPropertyName("component_catalog")] JsonElement? ComponentCatalog);

public sealed record DefaultRulesetComponentsResponse(
    [property: JsonPropertyName("items")] List<DefaultRulesetComponentItem> Items);

public sealed record EventRequest(
    [property: JsonPropertyName("event_id")] Guid EventId,
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("player_id")] Guid? PlayerId,
    [property: JsonPropertyName("actor_type")] string ActorType,
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("day_index")] int DayIndex,
    [property: JsonPropertyName("weekday")] string Weekday,
    [property: JsonPropertyName("turn_number")] int TurnNumber,
    [property: JsonPropertyName("sequence_number")] long SequenceNumber,
    [property: JsonPropertyName("action_type")] string ActionType,
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    [property: JsonPropertyName("payload")] JsonElement Payload,
    [property: JsonPropertyName("client_request_id")] string? ClientRequestId);

public sealed record EventStoredResponse(
    [property: JsonPropertyName("stored")] bool Stored,
    [property: JsonPropertyName("event_id")] Guid EventId);

public sealed record EventBatchRequest([property: JsonPropertyName("events")] List<EventRequest> Events);

public sealed record EventBatchFailed(
    [property: JsonPropertyName("event_id")] Guid EventId,
    [property: JsonPropertyName("error_code")] string ErrorCode);

public sealed record EventBatchResponse(
    [property: JsonPropertyName("stored_count")] int StoredCount,
    [property: JsonPropertyName("failed")] List<EventBatchFailed> Failed);

public sealed record EventsBySessionResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("events")] List<EventRequest> Events);

public sealed record AnalyticsSessionSummary(
    [property: JsonPropertyName("event_count")] int EventCount,
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal,
    [property: JsonPropertyName("cashflow_net_total")] double CashflowNetTotal,
    [property: JsonPropertyName("rules_violations_count")] int RulesViolationsCount);

public sealed record AnalyticsByPlayerItem(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("join_order")] int JoinOrder,
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal,
    [property: JsonPropertyName("donation_total")] double DonationTotal,
    [property: JsonPropertyName("gold_qty")] int GoldQty,
    [property: JsonPropertyName("orders_completed_count")] int OrdersCompletedCount,
    [property: JsonPropertyName("inventory_ingredient_total")] int InventoryIngredientTotal,
    [property: JsonPropertyName("actions_used_total")] int ActionsUsedTotal,
    [property: JsonPropertyName("compliance_primary_need_rate")] double CompliancePrimaryNeedRate,
    [property: JsonPropertyName("rules_violations_count")] int RulesViolationsCount,
    [property: JsonPropertyName("happiness_points_total")] double HappinessPointsTotal,
    [property: JsonPropertyName("need_points_total")] double NeedPointsTotal,
    [property: JsonPropertyName("need_set_bonus_points")] double NeedSetBonusPoints,
    [property: JsonPropertyName("donation_points_total")] double DonationPointsTotal,
    [property: JsonPropertyName("gold_points_total")] double GoldPointsTotal,
    [property: JsonPropertyName("pension_points_total")] double PensionPointsTotal,
    [property: JsonPropertyName("saving_goal_points_total")] double SavingGoalPointsTotal,
    [property: JsonPropertyName("mission_penalty_total")] double MissionPenaltyTotal,
    [property: JsonPropertyName("loan_penalty_total")] double LoanPenaltyTotal,
    [property: JsonPropertyName("has_unpaid_loan")] bool HasUnpaidLoan);

public sealed record AnalyticsSessionResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("summary")] AnalyticsSessionSummary Summary,
    [property: JsonPropertyName("by_player")] List<AnalyticsByPlayerItem> ByPlayer,
    [property: JsonPropertyName("ruleset_id")] Guid? RulesetId,
    [property: JsonPropertyName("ruleset_name")] string? RulesetName);

public sealed record GameplayMetricsResponse(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("computed_at")] DateTimeOffset? ComputedAt,
    [property: JsonPropertyName("raw")] JsonElement? Raw,
    [property: JsonPropertyName("derived")] JsonElement? Derived);

public sealed record TransactionHistoryItem(
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    [property: JsonPropertyName("direction")] string Direction,
    [property: JsonPropertyName("amount")] double Amount,
    [property: JsonPropertyName("category")] string Category);

public sealed record TransactionHistoryResponse(
    [property: JsonPropertyName("items")] List<TransactionHistoryItem> Items);

public sealed record RulesetAnalyticsPlayerItem(
    [property: JsonPropertyName("player_id")] Guid PlayerId,
    [property: JsonPropertyName("learning_performance_individual_score")] double? LearningPerformanceIndividualScore,
    [property: JsonPropertyName("mission_performance_individual_score")] double? MissionPerformanceIndividualScore);

public sealed record RulesetAnalyticsSessionItem(
    [property: JsonPropertyName("session_id")] Guid SessionId,
    [property: JsonPropertyName("session_name")] string SessionName,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("event_count")] int EventCount,
    [property: JsonPropertyName("learning_performance_aggregate_score")] double? LearningPerformanceAggregateScore,
    [property: JsonPropertyName("mission_performance_aggregate_score")] double? MissionPerformanceAggregateScore,
    [property: JsonPropertyName("players")] List<RulesetAnalyticsPlayerItem> Players);

public sealed record RulesetAnalyticsSummaryResponse(
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    [property: JsonPropertyName("ruleset_name")] string RulesetName,
    [property: JsonPropertyName("session_count")] int SessionCount,
    [property: JsonPropertyName("learning_performance_aggregate_score")] double? LearningPerformanceAggregateScore,
    [property: JsonPropertyName("mission_performance_aggregate_score")] double? MissionPerformanceAggregateScore,
    [property: JsonPropertyName("sessions")] List<RulesetAnalyticsSessionItem> Sessions);

public sealed record LoginRequest(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password);

public sealed record LoginResponse(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("display_name")] string DisplayName,
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);

public sealed record RegisterRequest(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("display_name")] string? DisplayName);

public sealed record RegisterResponse(
    [property: JsonPropertyName("user_id")] Guid UserId,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("display_name")] string DisplayName,
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);

public sealed record SecurityAuditLogItem(
    [property: JsonPropertyName("security_audit_log_id")] Guid SecurityAuditLogId,
    [property: JsonPropertyName("occurred_at")] DateTimeOffset OccurredAt,
    [property: JsonPropertyName("trace_id")] string TraceId,
    [property: JsonPropertyName("event_type")] string EventType,
    [property: JsonPropertyName("outcome")] string Outcome,
    [property: JsonPropertyName("user_id")] Guid? UserId,
    [property: JsonPropertyName("username")] string? Username,
    [property: JsonPropertyName("role")] string? Role,
    [property: JsonPropertyName("ip_address")] string? IpAddress,
    [property: JsonPropertyName("user_agent")] string? UserAgent,
    [property: JsonPropertyName("method")] string Method,
    [property: JsonPropertyName("path")] string Path,
    [property: JsonPropertyName("status_code")] int StatusCode,
    [property: JsonPropertyName("detail")] JsonElement? Detail);

public sealed record SecurityAuditLogResponse(
    [property: JsonPropertyName("items")] List<SecurityAuditLogItem> Items);
```

- [ ] **Step 4: Add project to solution**

```bash
cd c:\Users\marco\cashflowpoly-analytics-platform
dotnet sln add src/Cashflowpoly.Contracts/Cashflowpoly.Contracts.csproj
```

Expected: `Project ... added to the solution.`

- [ ] **Step 5: Verify build**

```bash
dotnet build src/Cashflowpoly.Contracts/Cashflowpoly.Contracts.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 6: Commit**

```bash
git add src/Cashflowpoly.Contracts/ Cashflowpoly.sln
git commit -m "feat: add Cashflowpoly.Contracts shared DTOs project"
```

---

## Task 2: Update Api to use Contracts

**Files:**
- Modify: `src/Cashflowpoly.Api/Cashflowpoly.Api.csproj`
- Delete: `src/Cashflowpoly.Api/Models/ApiDtos.cs`
- Delete: `src/Cashflowpoly.Api/Models/ErrorResponse.cs`
- Modify: all `*.cs` files in Api that import `Cashflowpoly.Api.Models`

- [ ] **Step 1: Add Contracts reference to Api.csproj**

In `src/Cashflowpoly.Api/Cashflowpoly.Api.csproj`, add inside `<ItemGroup>`:

```xml
<ItemGroup>
  <ProjectReference Include="..\Cashflowpoly.Contracts\Cashflowpoly.Contracts.csproj" />
</ItemGroup>
```

- [ ] **Step 2: Delete old files**

```bash
del src\Cashflowpoly.Api\Models\ApiDtos.cs
del src\Cashflowpoly.Api\Models\ErrorResponse.cs
```

- [ ] **Step 3: Update all using statements in Api**

Find every file in `src/Cashflowpoly.Api/` that has `using Cashflowpoly.Api.Models;` and replace with `using Cashflowpoly.Contracts;`. Files affected:

- `Controllers/AnalyticsController.cs`
- `Controllers/AuthController.cs`
- `Controllers/EventsController.cs`
- `Controllers/PlayersController.cs`
- `Controllers/RulesetsController.cs`
- `Controllers/SessionsController.cs`
- `Controllers/SecurityAuditController.cs`
- `Controllers/ApiErrorHelper.cs`

For each file, replace:
```csharp
using Cashflowpoly.Api.Models;
```
with:
```csharp
using Cashflowpoly.Contracts;
```

- [ ] **Step 4: Build and fix any remaining namespace errors**

```bash
dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
```

Expected: `Build succeeded.` If errors, the error message will identify the remaining files needing the namespace change.

- [ ] **Step 5: Commit**

```bash
git add src/Cashflowpoly.Api/
git commit -m "refactor: migrate Api DTOs to Cashflowpoly.Contracts"
```

---

## Task 3: Update Ui to use Contracts

**Files:**
- Modify: `src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj`
- Delete: `src/Cashflowpoly.Ui/Models/ApiDtos.cs`
- Modify: all `*.cs` and `*.cshtml` files in Ui that use the old `*Dto` types

- [ ] **Step 1: Add Contracts reference to Ui.csproj**

In `src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="..\Cashflowpoly.Contracts\Cashflowpoly.Contracts.csproj" />
</ItemGroup>
```

- [ ] **Step 2: Delete old Ui ApiDtos.cs**

```bash
del src\Cashflowpoly.Ui\Models\ApiDtos.cs
```

- [ ] **Step 3: Update using statements in Ui**

Find every file in `src/Cashflowpoly.Ui/` with `using Cashflowpoly.Ui.Models;` that referenced the deleted ApiDtos types. Replace `Cashflowpoly.Ui.Models` with `Cashflowpoly.Contracts` for DTO types only (ViewModels stay in `Cashflowpoly.Ui.Models`).

The renamed types to update (Ui used `*Dto` suffix — the Contracts versions have no suffix):

| Old Ui type | New Contracts type |
|-------------|-------------------|
| `RulesetListItemDto` | `RulesetListItem` |
| `RulesetListResponseDto` | `RulesetListResponse` |
| `SessionListItemDto` | `SessionListItem` |
| `SessionListResponseDto` | `SessionListResponse` |
| `RulesetVersionItemDto` | `RulesetVersionItem` |
| `RulesetDetailResponseDto` | `RulesetDetailResponse` |
| `RulesetComponentsResponseDto` | `RulesetComponentsResponse` |
| `DefaultRulesetComponentItemDto` | `DefaultRulesetComponentItem` |
| `DefaultRulesetComponentsResponseDto` | `DefaultRulesetComponentsResponse` |
| `AnalyticsSessionSummaryDto` | `AnalyticsSessionSummary` |
| `AnalyticsByPlayerItemDto` | `AnalyticsByPlayerItem` |
| `AnalyticsSessionResponseDto` | `AnalyticsSessionResponse` |
| `GameplayMetricsResponseDto` | `GameplayMetricsResponse` |
| `TransactionHistoryItemDto` | `TransactionHistoryItem` |
| `TransactionHistoryResponseDto` | `TransactionHistoryResponse` |
| `RulesetAnalyticsPlayerItemDto` | `RulesetAnalyticsPlayerItem` |
| `RulesetAnalyticsSessionItemDto` | `RulesetAnalyticsSessionItem` |
| `RulesetAnalyticsSummaryResponseDto` | `RulesetAnalyticsSummaryResponse` |
| `PlayerResponseDto` | `PlayerResponse` |
| `PlayerListResponseDto` | `PlayerListResponse` |
| `EventRequestDto` | `EventRequest` |
| `EventsBySessionResponseDto` | `EventsBySessionResponse` |
| `ApiErrorDetailDto` | `ErrorDetail` |
| `ApiErrorResponseDto` | `ErrorResponse` |
| `LoginRequestDto` | `LoginRequest` |
| `LoginResponseDto` | `LoginResponse` |
| `RegisterRequestDto` | `RegisterRequest` |
| `RegisterResponseDto` | `RegisterResponse` |

Use find-and-replace in each file:
- `src/Cashflowpoly.Ui/Controllers/AuthController.cs`
- `src/Cashflowpoly.Ui/Controllers/RulesetsController.cs`
- `src/Cashflowpoly.Ui/Controllers/PlayersController.cs`
- `src/Cashflowpoly.Ui/Controllers/SessionsController.cs`
- `src/Cashflowpoly.Ui/Controllers/AnalyticsController.cs`
- `src/Cashflowpoly.Ui/Infrastructure/SessionTimelineMapper.cs`
- `src/Cashflowpoly.Ui/Infrastructure/PlayerMetricJsonMapper.cs`
- `src/Cashflowpoly.Ui/Infrastructure/PlayerMetricChartPayloadBuilder.cs`
- Any `.cshtml` views that reference Dto types

- [ ] **Step 4: Build Ui**

```bash
dotnet build src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 5: Update Tests project to reference Contracts**

In `tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj`, add:
```xml
<ItemGroup>
  <ProjectReference Include="..\..\src\Cashflowpoly.Contracts\Cashflowpoly.Contracts.csproj" />
</ItemGroup>
```

Run tests:
```bash
dotnet test tests/Cashflowpoly.Api.Tests/
```

Expected: All tests pass.

- [ ] **Step 6: Commit**

```bash
git add src/Cashflowpoly.Ui/ tests/Cashflowpoly.Api.Tests/
git commit -m "refactor: migrate Ui to use Cashflowpoly.Contracts, remove Dto suffix"
```

---

## Task 4: Create Domain Interfaces

**Files:**
- Create: `src/Cashflowpoly.Api/Domain/Interfaces/IHappinessCalculator.cs`
- Create: `src/Cashflowpoly.Api/Domain/Interfaces/IIngredientInventoryCalculator.cs`
- Create: `src/Cashflowpoly.Api/Domain/Interfaces/IPrimaryNeedComplianceEvaluator.cs`
- Create: `src/Cashflowpoly.Api/Domain/Interfaces/ISessionMetricCalculator.cs`
- Create: `src/Cashflowpoly.Api/Domain/Interfaces/IMetricSnapshotBuilder.cs`
- Create: `src/Cashflowpoly.Api/Domain/Interfaces/IPlayerOrdering.cs`
- Create: `src/Cashflowpoly.Api/Domain/Interfaces/IScoreCalculator.cs`
- Create: `src/Cashflowpoly.Api/Domain/Interfaces/IPayloadReaders.cs`
- Create: `src/Cashflowpoly.Api/Domain/Interfaces/IGameplaySnapshotBuilder.cs`
- Create: `src/Cashflowpoly.Api/Domain/Interfaces/IEventCashflowProjectionBuilder.cs`
- Create: `src/Cashflowpoly.Api/Domain/Interfaces/IEventRecordMapper.cs`
- Create: `src/Cashflowpoly.Api/Domain/Interfaces/IEventValidationDetailsSerializer.cs`
- Create: `src/Cashflowpoly.Api/Domain/Interfaces/IEventValidators.cs`
- Create: `src/Cashflowpoly.Api/Domain/Interfaces/IEventStateCalculators.cs`
- Create: `src/Cashflowpoly.Api/Domain/Interfaces/IGameplayCalculators.cs`

- [ ] **Step 1: Create IHappinessCalculator.cs**

```csharp
// src/Cashflowpoly.Api/Domain/Interfaces/IHappinessCalculator.cs
namespace Cashflowpoly.Api.Domain;

public interface IHappinessCalculator
{
    Dictionary<Guid, AnalyticsHappinessBreakdown> ComputeByPlayer(
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        RulesetConfig? config);

    AnalyticsHappinessBreakdown ComputeBreakdown(
        List<EventDb> playerEvents,
        double donationPoints,
        double goldPoints,
        double pensionPoints);

    double SumRankAwarded(IEnumerable<EventDb> events, string actionType);
    double SumPointsAwarded(IEnumerable<EventDb> events, string actionType);
}
```

- [ ] **Step 2: Create IIngredientInventoryCalculator.cs**

```csharp
// src/Cashflowpoly.Api/Domain/Interfaces/IIngredientInventoryCalculator.cs
namespace Cashflowpoly.Api.Domain;

public interface IIngredientInventoryCalculator
{
    AnalyticsIngredientInventory BuildIngredientInventory(List<EventDb> playerEvents);
}
```

- [ ] **Step 3: Create IPrimaryNeedComplianceEvaluator.cs**

```csharp
// src/Cashflowpoly.Api/Domain/Interfaces/IPrimaryNeedComplianceEvaluator.cs
namespace Cashflowpoly.Api.Domain;

public interface IPrimaryNeedComplianceEvaluator
{
    PrimaryNeedComplianceResult Evaluate(List<EventDb> playerEvents, RulesetConfig? config);
}
```

- [ ] **Step 4: Create ISessionMetricCalculator.cs**

```csharp
// src/Cashflowpoly.Api/Domain/Interfaces/ISessionMetricCalculator.cs
namespace Cashflowpoly.Api.Domain;

public interface ISessionMetricCalculator
{
    Dictionary<string, (double? Numeric, string? Json)> ComputeSessionMetrics(
        List<EventDb> events,
        List<CashflowProjectionDb> projections,
        Dictionary<Guid, AnalyticsHappinessBreakdown> happinessByPlayer);
}
```

- [ ] **Step 5: Create IMetricSnapshotBuilder.cs**

```csharp
// src/Cashflowpoly.Api/Domain/Interfaces/IMetricSnapshotBuilder.cs
namespace Cashflowpoly.Api.Domain;

public interface IMetricSnapshotBuilder
{
    List<MetricSnapshotDb> BuildMetricSnapshots(
        Guid sessionId,
        Guid? playerId,
        Guid rulesetVersionId,
        DateTimeOffset computedAt,
        Dictionary<string, (double? Numeric, string? Json)> metrics);
}
```

- [ ] **Step 6: Create IPlayerOrdering.cs**

```csharp
// src/Cashflowpoly.Api/Domain/Interfaces/IPlayerOrdering.cs
namespace Cashflowpoly.Api.Domain;

public interface IPlayerOrdering
{
    List<Cashflowpoly.Contracts.AnalyticsByPlayerItem> OrderPlayers(
        List<Cashflowpoly.Contracts.AnalyticsByPlayerItem> players,
        PlayerOrdering ordering,
        Dictionary<Guid, int> joinOrders,
        Dictionary<Guid, long> firstEventSequences,
        Dictionary<Guid, string> usernamesByPlayer);
}
```

- [ ] **Step 7: Create IScoreCalculator.cs**

```csharp
// src/Cashflowpoly.Api/Domain/Interfaces/IScoreCalculator.cs
namespace Cashflowpoly.Api.Domain;

public interface IScoreCalculator
{
    double? ComputeLearningPerformanceScore(
        double cashIn,
        double cashOut,
        double happinessPoints,
        double? complianceRate);

    double? ComputeMissionPerformanceScore(
        double missionPenalty,
        double loanPenalty);

    double? AverageNullable(IEnumerable<double?> values);
}
```

- [ ] **Step 8: Create IPayloadReaders.cs**

```csharp
// src/Cashflowpoly.Api/Domain/Interfaces/IPayloadReaders.cs
using System.Text.Json;

namespace Cashflowpoly.Api.Domain;

public interface IAnalyticsPayloadReader
{
    bool TryReadAmount(string payload, out int amount);
    bool TryReadGoldTrade(string payload, out string tradeType, out int qty);
    bool TryReadActionUsed(string payload, out int actionsUsed, out string? actionId);
    bool TryReadNeedPurchase(string payload, out string? needType, out string? cardId, out double points);
    bool TryReadMissionAssigned(string payload, out string missionId, out string targetCardId, out int penaltyPoints, out bool requirePrimary, out bool requireSecondary);
    bool TryReadSavingGoalAchieved(string payload, out double points);
    bool TryReadLoanTaken(string payload, out string loanId, out int principal, out int penaltyPoints);
    bool TryReadLoanRepay(string payload, out string loanId, out double amount);
    bool TryReadRankAwarded(string payload, out int rank, out double points);
    bool TryReadPointsAwarded(string payload, out double points);
    bool TryReadTieBreaker(string payload, out int number);
    JsonElement? ParseJsonElement(string? json);
}

public interface IEventPayloadReader
{
    bool TryReadAmount(string payload, out int amount);
    bool TryReadGoldTrade(string payload, out string tradeType, out int qty);
    bool TryReadActionUsed(string payload, out int used, out string? actionId);
}
```

- [ ] **Step 9: Create IGameplaySnapshotBuilder.cs**

Read `src/Cashflowpoly.Api/Domain/AnalyticsGameplaySnapshotBuilder.cs` to get the exact method signature, then create:

```csharp
// src/Cashflowpoly.Api/Domain/Interfaces/IGameplaySnapshotBuilder.cs
namespace Cashflowpoly.Api.Domain;

public interface IGameplaySnapshotBuilder
{
    GameplaySnapshot Build(
        List<EventDb> playerEvents,
        List<CashflowProjectionDb> playerProjections,
        List<EventDb> allEvents,
        RulesetConfig? config,
        AnalyticsHappinessBreakdown happiness);
}
```

(Where `GameplaySnapshot` is the existing return type — check the actual type name in `AnalyticsGameplaySnapshotBuilder.cs` and use it.)

- [ ] **Step 10: Create IEventCashflowProjectionBuilder.cs**

Read `src/Cashflowpoly.Api/Domain/EventCashflowProjectionBuilder.cs` for the exact signature:

```csharp
// src/Cashflowpoly.Api/Domain/Interfaces/IEventCashflowProjectionBuilder.cs
namespace Cashflowpoly.Api.Domain;

public interface IEventCashflowProjectionBuilder
{
    List<CashflowProjectionDb> Build(EventDb eventRecord, RulesetConfig? config);
}
```

- [ ] **Step 11: Create IEventRecordMapper.cs**

Read `src/Cashflowpoly.Api/Domain/EventRecordMapper.cs` for the exact signature:

```csharp
// src/Cashflowpoly.Api/Domain/Interfaces/IEventRecordMapper.cs
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

public interface IEventRecordMapper
{
    EventDb ToRecord(EventRequest request, Guid sessionId);
}
```

- [ ] **Step 12: Create IEventValidationDetailsSerializer.cs**

```csharp
// src/Cashflowpoly.Api/Domain/Interfaces/IEventValidationDetailsSerializer.cs
namespace Cashflowpoly.Api.Domain;

public interface IEventValidationDetailsSerializer
{
    string? Serialize(EventDomainValidationResult result);
}
```

- [ ] **Step 13: Create IEventValidators.cs — all 7 event validator interfaces**

Read each validator file to get the exact method signatures, then create:

```csharp
// src/Cashflowpoly.Api/Domain/Interfaces/IEventValidators.cs
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Domain;

public interface IEventShapeValidator
{
    EventDomainValidationResult Validate(EventRequest request);
}

public interface IEventSimpleActionValidator
{
    EventDomainValidationResult Validate(EventRequest request, RulesetConfig? config);
}

public interface IEventTurnProgressValidator
{
    EventDomainValidationResult Validate(EventRequest request, RulesetConfig? config);
}

public interface IEventNeedPurchaseValidator
{
    EventDomainValidationResult Validate(EventRequest request, RulesetConfig? config);
}

public interface IEventIngredientOrderValidator
{
    EventDomainValidationResult Validate(EventRequest request, RulesetConfig? config);
}

public interface IEventSavingGoalValidator
{
    EventDomainValidationResult Validate(EventRequest request, RulesetConfig? config);
}

public interface IEventEconomyActionValidator
{
    EventDomainValidationResult Validate(EventRequest request, RulesetConfig? config);
}

public interface IEventAssignmentValidator
{
    EventDomainValidationResult Validate(EventRequest request, RulesetConfig? config);
}
```

- [ ] **Step 14: Create IEventStateCalculators.cs**

Read `EventDerivedStateCalculator.cs`, `EventPlayerBalanceCalculator.cs`, `EventInsuranceOffsetBuilder.cs` for exact signatures:

```csharp
// src/Cashflowpoly.Api/Domain/Interfaces/IEventStateCalculators.cs
namespace Cashflowpoly.Api.Domain;

public interface IEventDerivedStateCalculator
{
    // copy exact public method signature from EventDerivedStateCalculator.cs
}

public interface IEventPlayerBalanceCalculator
{
    // copy exact public method signature from EventPlayerBalanceCalculator.cs
}

public interface IEventInsuranceOffsetBuilder
{
    // copy exact public method signature from EventInsuranceOffsetBuilder.cs
}
```

- [ ] **Step 15: Create IGameplayCalculators.cs — all 9 gameplay calculators**

Read each file for exact signatures:
- `AnalyticsCashTimelineCalculator.cs`
- `AnalyticsDonationGameplayCalculator.cs`
- `AnalyticsSavingGoalCalculator.cs`
- `AnalyticsIngredientMealCalculator.cs`
- `AnalyticsGoldGameplayCalculator.cs`
- `AnalyticsNeedMissionCalculator.cs`
- `AnalyticsRiskLoanCalculator.cs`
- `AnalyticsActionUsageCalculator.cs`
- `AnalyticsIncomeDiversificationCalculator.cs`
- `AnalyticsDerivedRatioCalculator.cs`

```csharp
// src/Cashflowpoly.Api/Domain/Interfaces/IGameplayCalculators.cs
namespace Cashflowpoly.Api.Domain;

public interface ICashTimelineCalculator { /* copy exact public method signatures */ }
public interface IDonationGameplayCalculator { /* copy exact public method signatures */ }
public interface ISavingGoalCalculator { /* copy exact public method signatures */ }
public interface IIngredientMealCalculator { /* copy exact public method signatures */ }
public interface IGoldGameplayCalculator { /* copy exact public method signatures */ }
public interface INeedMissionCalculator { /* copy exact public method signatures */ }
public interface IRiskLoanCalculator { /* copy exact public method signatures */ }
public interface IActionUsageCalculator { /* copy exact public method signatures */ }
public interface IIncomeDiversificationCalculator { /* copy exact public method signatures */ }
public interface IDerivedRatioCalculator { /* copy exact public method signatures */ }
```

- [ ] **Step 16: Build to verify interfaces compile**

```bash
dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
```

Expected: `Build succeeded.` (Interfaces exist, implementations still have original signatures — no errors yet.)

- [ ] **Step 17: Commit**

```bash
git add src/Cashflowpoly.Api/Domain/Interfaces/
git commit -m "refactor: add domain service interfaces"
```

---

## Task 5: Convert Domain Static Classes to Injectable Services

**Files:**
- Modify: all 37 `*.cs` files in `src/Cashflowpoly.Api/Domain/`

**Pattern to apply to every file:**

```csharp
// BEFORE
internal static class AnalyticsHappinessCalculator
{
    internal static Dictionary<Guid, AnalyticsHappinessBreakdown> ComputeByPlayer(...) { ... }
    internal static AnalyticsHappinessBreakdown ComputeBreakdown(...) { ... }
    internal static double SumRankAwarded(...) { ... }
    internal static double SumPointsAwarded(...) { ... }
}

// AFTER
internal sealed class HappinessCalculator : IHappinessCalculator
{
    public Dictionary<Guid, AnalyticsHappinessBreakdown> ComputeByPlayer(...) { ... }
    public AnalyticsHappinessBreakdown ComputeBreakdown(...) { ... }
    public double SumRankAwarded(...) { ... }
    public double SumPointsAwarded(...) { ... }
}
```

Changes per file:
1. Remove `static` from class declaration
2. Add `: I<InterfaceName>` to class declaration
3. Change `internal static` to `public` on all methods
4. Rename class (drop `Analytics` prefix where it creates redundancy, e.g. `AnalyticsHappinessCalculator` → `HappinessCalculator`)

**Full rename map:**

| Original class | New class name | Interface |
|---|---|---|
| `AnalyticsHappinessCalculator` | `HappinessCalculator` | `IHappinessCalculator` |
| `AnalyticsIngredientInventoryCalculator` | `IngredientInventoryCalculator` | `IIngredientInventoryCalculator` |
| `AnalyticsPrimaryNeedComplianceEvaluator` | `PrimaryNeedComplianceEvaluator` | `IPrimaryNeedComplianceEvaluator` |
| `AnalyticsSessionMetricCalculator` | `SessionMetricCalculator` | `ISessionMetricCalculator` |
| `AnalyticsMetricSnapshotBuilder` | `MetricSnapshotBuilder` | `IMetricSnapshotBuilder` |
| `AnalyticsPlayerOrdering` | `PlayerOrderingService` | `IPlayerOrdering` |
| `AnalyticsScoreCalculator` | `ScoreCalculator` | `IScoreCalculator` |
| `AnalyticsPayloadReader` | `AnalyticsPayloadReader` | `IAnalyticsPayloadReader` |
| `EventPayloadReader` | `EventPayloadReader` | `IEventPayloadReader` |
| `AnalyticsGameplaySnapshotBuilder` | `GameplaySnapshotBuilder` | `IGameplaySnapshotBuilder` |
| `EventCashflowProjectionBuilder` | `EventCashflowProjectionBuilder` | `IEventCashflowProjectionBuilder` |
| `EventRecordMapper` | `EventRecordMapper` | `IEventRecordMapper` |
| `EventValidationDetailsSerializer` | `EventValidationDetailsSerializer` | `IEventValidationDetailsSerializer` |
| `EventRequestShapeValidator` | `EventShapeValidator` | `IEventShapeValidator` |
| `EventSimpleActionValidator` | `EventSimpleActionValidator` | `IEventSimpleActionValidator` |
| `EventTurnProgressValidator` | `EventTurnProgressValidator` | `IEventTurnProgressValidator` |
| `EventNeedPurchaseValidator` | `EventNeedPurchaseValidator` | `IEventNeedPurchaseValidator` |
| `EventIngredientOrderValidator` | `EventIngredientOrderValidator` | `IEventIngredientOrderValidator` |
| `EventSavingGoalValidator` | `EventSavingGoalValidator` | `IEventSavingGoalValidator` |
| `EventEconomyActionValidator` | `EventEconomyActionValidator` | `IEventEconomyActionValidator` |
| `EventAssignmentValidator` | `EventAssignmentValidator` | `IEventAssignmentValidator` |
| `EventDerivedStateCalculator` | `EventDerivedStateCalculator` | `IEventDerivedStateCalculator` |
| `EventPlayerBalanceCalculator` | `EventPlayerBalanceCalculator` | `IEventPlayerBalanceCalculator` |
| `EventInsuranceOffsetBuilder` | `EventInsuranceOffsetBuilder` | `IEventInsuranceOffsetBuilder` |
| `AnalyticsCashTimelineCalculator` | `CashTimelineCalculator` | `ICashTimelineCalculator` |
| `AnalyticsDonationGameplayCalculator` | `DonationGameplayCalculator` | `IDonationGameplayCalculator` |
| `AnalyticsSavingGoalCalculator` | `SavingGoalCalculator` | `ISavingGoalCalculator` |
| `AnalyticsIngredientMealCalculator` | `IngredientMealCalculator` | `IIngredientMealCalculator` |
| `AnalyticsGoldGameplayCalculator` | `GoldGameplayCalculator` | `IGoldGameplayCalculator` |
| `AnalyticsNeedMissionCalculator` | `NeedMissionCalculator` | `INeedMissionCalculator` |
| `AnalyticsRiskLoanCalculator` | `RiskLoanCalculator` | `IRiskLoanCalculator` |
| `AnalyticsActionUsageCalculator` | `ActionUsageCalculator` | `IActionUsageCalculator` |
| `AnalyticsIncomeDiversificationCalculator` | `IncomeDiversificationCalculator` | `IIncomeDiversificationCalculator` |
| `AnalyticsDerivedRatioCalculator` | `DerivedRatioCalculator` | `IDerivedRatioCalculator` |

**Note:** `AnalyticsMath` stays static — pure math helpers with no DI benefit. `RulesetConfig`, `SessionRules`, model records stay unchanged.

- [ ] **Step 1: For each file in the rename map, apply the pattern**

Start with `AnalyticsHappinessCalculator.cs` as the first example:
- Open `src/Cashflowpoly.Api/Domain/AnalyticsHappinessCalculator.cs`
- Change `internal static class AnalyticsHappinessCalculator` → `internal sealed class HappinessCalculator : IHappinessCalculator`
- Change every `internal static` method to `public`
- Remove `static` from private helper methods too

Repeat for all 34 other files in the rename map.

- [ ] **Step 2: Build**

```bash
dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
```

If there are errors about static calls within the domain classes (e.g., `AnalyticsHappinessCalculator.ComputeBreakdown(...)` called from another domain class), update those calls to use `this.ComputeBreakdown(...)` or pass the dependency through the constructor.

- [ ] **Step 3: Update unit tests to instantiate classes instead of calling static methods**

For every test file in `tests/Cashflowpoly.Api.Tests/` that calls a now-non-static domain class, update the test to instantiate:

```csharp
// BEFORE
var result = AnalyticsHappinessCalculator.ComputeBreakdown(events, 0, 0, 0);

// AFTER
var calculator = new HappinessCalculator();
var result = calculator.ComputeBreakdown(events, 0, 0, 0);
```

Files to update: all `*CalculatorTests.cs`, `*ValidatorTests.cs`, `*BuilderTests.cs`, `*MapperTests.cs` files.

- [ ] **Step 4: Run tests**

```bash
dotnet test tests/Cashflowpoly.Api.Tests/
```

Expected: All tests pass.

- [ ] **Step 5: Commit**

```bash
git add src/Cashflowpoly.Api/Domain/ tests/Cashflowpoly.Api.Tests/
git commit -m "refactor: convert static domain classes to injectable services"
```

---

## Task 6: Register Domain Services in DI

**Files:**
- Modify: `src/Cashflowpoly.Api/Program.cs`

- [ ] **Step 1: Add all domain service registrations to Program.cs**

After the existing repository registrations, add:

```csharp
// Domain calculators
builder.Services.AddScoped<IHappinessCalculator, HappinessCalculator>();
builder.Services.AddScoped<IIngredientInventoryCalculator, IngredientInventoryCalculator>();
builder.Services.AddScoped<IPrimaryNeedComplianceEvaluator, PrimaryNeedComplianceEvaluator>();
builder.Services.AddScoped<ISessionMetricCalculator, SessionMetricCalculator>();
builder.Services.AddScoped<IMetricSnapshotBuilder, MetricSnapshotBuilder>();
builder.Services.AddScoped<IPlayerOrdering, PlayerOrderingService>();
builder.Services.AddScoped<IScoreCalculator, ScoreCalculator>();
builder.Services.AddScoped<IAnalyticsPayloadReader, AnalyticsPayloadReader>();
builder.Services.AddScoped<IEventPayloadReader, EventPayloadReader>();
builder.Services.AddScoped<IGameplaySnapshotBuilder, GameplaySnapshotBuilder>();
builder.Services.AddScoped<IEventCashflowProjectionBuilder, EventCashflowProjectionBuilder>();
builder.Services.AddScoped<IEventRecordMapper, EventRecordMapper>();
builder.Services.AddScoped<IEventValidationDetailsSerializer, EventValidationDetailsSerializer>();
builder.Services.AddScoped<IEventShapeValidator, EventShapeValidator>();
builder.Services.AddScoped<IEventSimpleActionValidator, EventSimpleActionValidator>();
builder.Services.AddScoped<IEventTurnProgressValidator, EventTurnProgressValidator>();
builder.Services.AddScoped<IEventNeedPurchaseValidator, EventNeedPurchaseValidator>();
builder.Services.AddScoped<IEventIngredientOrderValidator, EventIngredientOrderValidator>();
builder.Services.AddScoped<IEventSavingGoalValidator, EventSavingGoalValidator>();
builder.Services.AddScoped<IEventEconomyActionValidator, EventEconomyActionValidator>();
builder.Services.AddScoped<IEventAssignmentValidator, EventAssignmentValidator>();
builder.Services.AddScoped<IEventDerivedStateCalculator, EventDerivedStateCalculator>();
builder.Services.AddScoped<IEventPlayerBalanceCalculator, EventPlayerBalanceCalculator>();
builder.Services.AddScoped<IEventInsuranceOffsetBuilder, EventInsuranceOffsetBuilder>();
builder.Services.AddScoped<ICashTimelineCalculator, CashTimelineCalculator>();
builder.Services.AddScoped<IDonationGameplayCalculator, DonationGameplayCalculator>();
builder.Services.AddScoped<ISavingGoalCalculator, SavingGoalCalculator>();
builder.Services.AddScoped<IIngredientMealCalculator, IngredientMealCalculator>();
builder.Services.AddScoped<IGoldGameplayCalculator, GoldGameplayCalculator>();
builder.Services.AddScoped<INeedMissionCalculator, NeedMissionCalculator>();
builder.Services.AddScoped<IRiskLoanCalculator, RiskLoanCalculator>();
builder.Services.AddScoped<IActionUsageCalculator, ActionUsageCalculator>();
builder.Services.AddScoped<IIncomeDiversificationCalculator, IncomeDiversificationCalculator>();
builder.Services.AddScoped<IDerivedRatioCalculator, DerivedRatioCalculator>();
```

- [ ] **Step 2: Build**

```bash
dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 3: Commit**

```bash
git add src/Cashflowpoly.Api/Program.cs
git commit -m "refactor: register domain services in DI container"
```

---

## Task 7: Create AnalyticsService

**Files:**
- Create: `src/Cashflowpoly.Api/Services/IAnalyticsService.cs`
- Create: `src/Cashflowpoly.Api/Services/AnalyticsService.cs`

- [ ] **Step 1: Create IAnalyticsService.cs**

```csharp
// src/Cashflowpoly.Api/Services/IAnalyticsService.cs
using System.Security.Claims;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Services;

public interface IAnalyticsService
{
    Task<(AnalyticsSessionResponse? Result, int StatusCode, ErrorResponse? Error)> GetSessionAnalyticsAsync(
        Guid sessionId, ClaimsPrincipal user, CancellationToken ct);

    Task<(AnalyticsSessionResponse? Result, int StatusCode, ErrorResponse? Error)> RecomputeAsync(
        Guid sessionId, ClaimsPrincipal user, CancellationToken ct);

    Task<(TransactionHistoryResponse? Result, int StatusCode, ErrorResponse? Error)> GetTransactionsAsync(
        Guid sessionId, Guid? playerId, ClaimsPrincipal user, CancellationToken ct);

    Task<(GameplayMetricsResponse? Result, int StatusCode, ErrorResponse? Error)> GetGameplayMetricsAsync(
        Guid sessionId, Guid playerId, ClaimsPrincipal user, CancellationToken ct);

    Task<(RulesetAnalyticsSummaryResponse? Result, int StatusCode, ErrorResponse? Error)> GetRulesetAnalyticsSummaryAsync(
        Guid rulesetId, ClaimsPrincipal user, CancellationToken ct);
}
```

- [ ] **Step 2: Create AnalyticsService.cs — move all private methods from AnalyticsController**

Create `src/Cashflowpoly.Api/Services/AnalyticsService.cs`. Move the following private methods from `AnalyticsController` into this class:
- `BuildByPlayerAsync`
- `ComputePlayerMetricsAsync`
- `WriteSnapshotsAsync`
- `ResolvePlayerScopeAsync`
- `EnsureInstructorSessionAccessAsync`
- `TryGetCurrentUserId`
- `ParseJsonElement` (move to `AnalyticsPayloadReader` or keep as private static in service)
- `BuildSummary` (if it exists — check controller)
- `AverageNullable` (move to `ScoreCalculator`)
- `ComputePrimaryNeedComplianceAsync`

The service constructor takes all dependencies currently injected into the controller, plus the domain interfaces:

```csharp
// src/Cashflowpoly.Api/Services/AnalyticsService.cs
using System.Security.Claims;
using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Http;

namespace Cashflowpoly.Api.Services;

internal sealed class AnalyticsService : IAnalyticsService
{
    private readonly SessionRepository _sessions;
    private readonly EventRepository _events;
    private readonly RulesetRepository _rulesets;
    private readonly MetricsRepository _metrics;
    private readonly PlayerRepository _players;
    private readonly UserRepository _users;
    private readonly IHappinessCalculator _happiness;
    private readonly IIngredientInventoryCalculator _inventory;
    private readonly IPrimaryNeedComplianceEvaluator _compliance;
    private readonly ISessionMetricCalculator _sessionMetrics;
    private readonly IMetricSnapshotBuilder _snapshotBuilder;
    private readonly IPlayerOrdering _playerOrdering;
    private readonly IScoreCalculator _scores;
    private readonly IAnalyticsPayloadReader _payloadReader;
    private readonly IGameplaySnapshotBuilder _gameplaySnapshots;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AnalyticsService(
        SessionRepository sessions,
        EventRepository events,
        RulesetRepository rulesets,
        MetricsRepository metrics,
        PlayerRepository players,
        UserRepository users,
        IHappinessCalculator happiness,
        IIngredientInventoryCalculator inventory,
        IPrimaryNeedComplianceEvaluator compliance,
        ISessionMetricCalculator sessionMetrics,
        IMetricSnapshotBuilder snapshotBuilder,
        IPlayerOrdering playerOrdering,
        IScoreCalculator scores,
        IAnalyticsPayloadReader payloadReader,
        IGameplaySnapshotBuilder gameplaySnapshots,
        IHttpContextAccessor httpContextAccessor)
    {
        _sessions = sessions;
        _events = events;
        _rulesets = rulesets;
        _metrics = metrics;
        _players = players;
        _users = users;
        _happiness = happiness;
        _inventory = inventory;
        _compliance = compliance;
        _sessionMetrics = sessionMetrics;
        _snapshotBuilder = snapshotBuilder;
        _playerOrdering = playerOrdering;
        _scores = scores;
        _payloadReader = payloadReader;
        _gameplaySnapshots = gameplaySnapshots;
        _httpContextAccessor = httpContextAccessor;
    }

    // Move all private methods from AnalyticsController here.
    // Convert them to private instance methods (not static, not async unless they were).
    // Replace HttpContext references with _httpContextAccessor.HttpContext!
    // Replace direct static domain calls with the injected interface fields.
    // Implement IAnalyticsService methods that call these private methods.
}
```

- [ ] **Step 3: Implement each IAnalyticsService method**

For `GetSessionAnalyticsAsync`, copy the body of `AnalyticsController.GetSessionAnalytics` (minus the HTTP return statements), replacing:
- `return Ok(...)` → `return (result, 200, null)`
- `return NotFound(...)` → `return (null, 404, error)`
- `return StatusCode(403, ...)` → `return (null, 403, error)`
- `return Unauthorized(...)` → `return (null, 401, error)`

Repeat for all 5 action methods.

- [ ] **Step 4: Register AnalyticsService in Program.cs**

```csharp
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddHttpContextAccessor();
```

- [ ] **Step 5: Build**

```bash
dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 6: Commit**

```bash
git add src/Cashflowpoly.Api/Services/ src/Cashflowpoly.Api/Program.cs
git commit -m "feat: extract AnalyticsService from AnalyticsController"
```

---

## Task 8: Slim Down AnalyticsController

**Files:**
- Modify: `src/Cashflowpoly.Api/Controllers/AnalyticsController.cs`

- [ ] **Step 1: Rewrite AnalyticsController to inject IAnalyticsService**

Replace the entire file content:

```csharp
using Cashflowpoly.Api.Controllers;
using Cashflowpoly.Api.Services;
using Cashflowpoly.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cashflowpoly.Api.Controllers;

[ApiController]
[Route("api/v1/analytics")]
[Authorize]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status429TooManyRequests)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
public sealed class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analytics;

    public AnalyticsController(IAnalyticsService analytics) => _analytics = analytics;

    [HttpPost("sessions/{sessionId:guid}/recompute")]
    [Authorize(Roles = "INSTRUCTOR")]
    [ProducesResponseType(typeof(AnalyticsSessionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Recompute(Guid sessionId, CancellationToken ct)
    {
        var (result, status, error) = await _analytics.RecomputeAsync(sessionId, User, ct);
        return status == 200 ? Ok(result) : StatusCode(status, error);
    }

    [HttpGet("sessions/{sessionId:guid}")]
    [ProducesResponseType(typeof(AnalyticsSessionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSessionAnalytics(Guid sessionId, CancellationToken ct)
    {
        var (result, status, error) = await _analytics.GetSessionAnalyticsAsync(sessionId, User, ct);
        return status == 200 ? Ok(result) : StatusCode(status, error);
    }

    [HttpGet("sessions/{sessionId:guid}/transactions")]
    [ProducesResponseType(typeof(TransactionHistoryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactions(Guid sessionId, [FromQuery] Guid? playerId = null, CancellationToken ct = default)
    {
        var (result, status, error) = await _analytics.GetTransactionsAsync(sessionId, playerId, User, ct);
        return status == 200 ? Ok(result) : StatusCode(status, error);
    }

    [HttpGet("sessions/{sessionId:guid}/players/{playerId:guid}/gameplay")]
    [ProducesResponseType(typeof(GameplayMetricsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGameplayMetrics(Guid sessionId, Guid playerId, CancellationToken ct)
    {
        var (result, status, error) = await _analytics.GetGameplayMetricsAsync(sessionId, playerId, User, ct);
        return status == 200 ? Ok(result) : StatusCode(status, error);
    }

    [HttpGet("rulesets/{rulesetId:guid}/summary")]
    [ProducesResponseType(typeof(RulesetAnalyticsSummaryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRulesetAnalyticsSummary(Guid rulesetId, CancellationToken ct)
    {
        var (result, status, error) = await _analytics.GetRulesetAnalyticsSummaryAsync(rulesetId, User, ct);
        return status == 200 ? Ok(result) : StatusCode(status, error);
    }
}
```

- [ ] **Step 2: Build**

```bash
dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 3: Run integration tests**

```bash
dotnet test tests/Cashflowpoly.Api.Tests/ --filter "EventAnalytics"
```

Expected: All EventAnalytics integration tests pass.

- [ ] **Step 4: Commit**

```bash
git add src/Cashflowpoly.Api/Controllers/AnalyticsController.cs
git commit -m "refactor: slim down AnalyticsController to delegate to IAnalyticsService"
```

---

## Task 9: Create EventIngestionService and Slim EventsController

**Files:**
- Create: `src/Cashflowpoly.Api/Services/IEventIngestionService.cs`
- Create: `src/Cashflowpoly.Api/Services/EventIngestionService.cs`
- Modify: `src/Cashflowpoly.Api/Controllers/EventsController.cs`

- [ ] **Step 1: Create IEventIngestionService.cs**

```csharp
// src/Cashflowpoly.Api/Services/IEventIngestionService.cs
using System.Security.Claims;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Services;

public interface IEventIngestionService
{
    Task<(EventStoredResponse? Result, int StatusCode, ErrorResponse? Error)> IngestEventAsync(
        EventRequest request, ClaimsPrincipal user, CancellationToken ct);

    Task<(EventBatchResponse? Result, int StatusCode, ErrorResponse? Error)> IngestBatchAsync(
        EventBatchRequest request, ClaimsPrincipal user, CancellationToken ct);

    Task<(EventsBySessionResponse? Result, int StatusCode, ErrorResponse? Error)> GetEventsBySessionAsync(
        Guid sessionId, ClaimsPrincipal user, CancellationToken ct);
}
```

- [ ] **Step 2: Create EventIngestionService.cs**

Move all business logic from `EventsController` into this service. The constructor should inject all domain validator interfaces and repositories:

```csharp
// src/Cashflowpoly.Api/Services/EventIngestionService.cs
using System.Security.Claims;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Contracts;

namespace Cashflowpoly.Api.Services;

internal sealed class EventIngestionService : IEventIngestionService
{
    private readonly SessionRepository _sessions;
    private readonly RulesetRepository _rulesets;
    private readonly EventRepository _events;
    private readonly PlayerRepository _players;
    private readonly UserRepository _users;
    private readonly IEventShapeValidator _shapeValidator;
    private readonly IEventSimpleActionValidator _simpleActionValidator;
    private readonly IEventTurnProgressValidator _turnProgressValidator;
    private readonly IEventNeedPurchaseValidator _needPurchaseValidator;
    private readonly IEventIngredientOrderValidator _ingredientOrderValidator;
    private readonly IEventSavingGoalValidator _savingGoalValidator;
    private readonly IEventEconomyActionValidator _economyActionValidator;
    private readonly IEventAssignmentValidator _assignmentValidator;
    private readonly IEventCashflowProjectionBuilder _projectionBuilder;
    private readonly IEventRecordMapper _recordMapper;
    private readonly IEventValidationDetailsSerializer _validationSerializer;

    public EventIngestionService(
        SessionRepository sessions,
        RulesetRepository rulesets,
        EventRepository events,
        PlayerRepository players,
        UserRepository users,
        IEventShapeValidator shapeValidator,
        IEventSimpleActionValidator simpleActionValidator,
        IEventTurnProgressValidator turnProgressValidator,
        IEventNeedPurchaseValidator needPurchaseValidator,
        IEventIngredientOrderValidator ingredientOrderValidator,
        IEventSavingGoalValidator savingGoalValidator,
        IEventEconomyActionValidator economyActionValidator,
        IEventAssignmentValidator assignmentValidator,
        IEventCashflowProjectionBuilder projectionBuilder,
        IEventRecordMapper recordMapper,
        IEventValidationDetailsSerializer validationSerializer)
    {
        _sessions = sessions;
        _rulesets = rulesets;
        _events = events;
        _players = players;
        _users = users;
        _shapeValidator = shapeValidator;
        _simpleActionValidator = simpleActionValidator;
        _turnProgressValidator = turnProgressValidator;
        _needPurchaseValidator = needPurchaseValidator;
        _ingredientOrderValidator = ingredientOrderValidator;
        _savingGoalValidator = savingGoalValidator;
        _economyActionValidator = economyActionValidator;
        _assignmentValidator = assignmentValidator;
        _projectionBuilder = projectionBuilder;
        _recordMapper = recordMapper;
        _validationSerializer = validationSerializer;
    }

    // Implement all methods by moving business logic from EventsController private methods here
}
```

- [ ] **Step 3: Register EventIngestionService in Program.cs**

```csharp
builder.Services.AddScoped<IEventIngestionService, EventIngestionService>();
```

- [ ] **Step 4: Rewrite EventsController to delegate to IEventIngestionService**

Apply same thin-controller pattern as AnalyticsController — controller only has routing attributes and calls service.

- [ ] **Step 5: Build and run tests**

```bash
dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
dotnet test tests/Cashflowpoly.Api.Tests/
```

Expected: All tests pass.

- [ ] **Step 6: Commit**

```bash
git add src/Cashflowpoly.Api/Services/ src/Cashflowpoly.Api/Controllers/EventsController.cs src/Cashflowpoly.Api/Program.cs
git commit -m "refactor: extract EventIngestionService, slim EventsController"
```

---

## Task 10: Add EF Core and Create AppDbContext

**Files:**
- Modify: `src/Cashflowpoly.Api/Cashflowpoly.Api.csproj`
- Create: `src/Cashflowpoly.Api/Data/AppDbContext.cs`

- [ ] **Step 1: Add EF Core packages**

In `src/Cashflowpoly.Api/Cashflowpoly.Api.csproj`:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.4" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.4" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.4">
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  <PrivateAssets>all</PrivateAssets>
</PackageReference>
```

Restore:
```bash
dotnet restore src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
```

- [ ] **Step 2: Create AppDbContext.cs**

```csharp
// src/Cashflowpoly.Api/Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace Cashflowpoly.Api.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<RulesetDb> Rulesets { get; set; }
    public DbSet<RulesetVersionDb> RulesetVersions { get; set; }
    public DbSet<SessionDb> Sessions { get; set; }
    public DbSet<PlayerDb> Players { get; set; }
    public DbSet<EventDb> Events { get; set; }
    public DbSet<CashflowProjectionDb> CashflowProjections { get; set; }
    public DbSet<MetricSnapshotDb> MetricSnapshots { get; set; }
    public DbSet<SecurityAuditLogDb> SecurityAuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RulesetDb>(entity =>
        {
            entity.ToTable("rulesets");
            entity.HasKey(e => e.RulesetId);
            entity.Property(e => e.RulesetId).HasColumnName("ruleset_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.InstructorUserId).HasColumnName("instructor_user_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
        });

        modelBuilder.Entity<RulesetVersionDb>(entity =>
        {
            entity.ToTable("ruleset_versions");
            entity.HasKey(e => e.RulesetVersionId);
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            entity.Property(e => e.RulesetId).HasColumnName("ruleset_id");
            entity.Property(e => e.Version).HasColumnName("version");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.ConfigJson).HasColumnName("config_json");
            entity.Property(e => e.ConfigHash).HasColumnName("config_hash");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
        });

        modelBuilder.Entity<SessionDb>(entity =>
        {
            entity.ToTable("sessions");
            entity.HasKey(e => e.SessionId);
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.SessionName).HasColumnName("session_name");
            entity.Property(e => e.Mode).HasColumnName("mode");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StartedAt).HasColumnName("started_at");
            entity.Property(e => e.EndedAt).HasColumnName("ended_at");
            entity.Property(e => e.InstructorUserId).HasColumnName("instructor_user_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<PlayerDb>(entity =>
        {
            entity.ToTable("players");
            entity.HasKey(e => e.PlayerId);
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.DisplayName).HasColumnName("display_name");
            entity.Property(e => e.InstructorUserId).HasColumnName("instructor_user_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<EventDb>(entity =>
        {
            entity.ToTable("events");
            entity.HasKey(e => e.EventPk);
            entity.Property(e => e.EventPk).HasColumnName("event_pk");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.ActorType).HasColumnName("actor_type");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.DayIndex).HasColumnName("day_index");
            entity.Property(e => e.Weekday).HasColumnName("weekday");
            entity.Property(e => e.TurnNumber).HasColumnName("turn_number");
            entity.Property(e => e.SequenceNumber).HasColumnName("sequence_number");
            entity.Property(e => e.ActionType).HasColumnName("action_type");
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
            entity.Property(e => e.Payload).HasColumnName("payload");
            entity.Property(e => e.ReceivedAt).HasColumnName("received_at");
            entity.Property(e => e.ClientRequestId).HasColumnName("client_request_id");
        });

        modelBuilder.Entity<CashflowProjectionDb>(entity =>
        {
            entity.ToTable("event_cashflow_projections");
            entity.HasKey(e => e.ProjectionId);
            entity.Property(e => e.ProjectionId).HasColumnName("projection_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.EventPk).HasColumnName("event_pk");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.Direction).HasColumnName("direction");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Counterparty).HasColumnName("counterparty");
            entity.Property(e => e.Reference).HasColumnName("reference");
            entity.Property(e => e.Note).HasColumnName("note");
        });

        modelBuilder.Entity<MetricSnapshotDb>(entity =>
        {
            entity.ToTable("metric_snapshots");
            entity.HasKey(e => e.MetricSnapshotId);
            entity.Property(e => e.MetricSnapshotId).HasColumnName("metric_snapshot_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.ComputedAt).HasColumnName("computed_at");
            entity.Property(e => e.MetricName).HasColumnName("metric_name");
            entity.Property(e => e.MetricValueNumeric).HasColumnName("metric_value_numeric");
            entity.Property(e => e.MetricValueJson).HasColumnName("metric_value_json");
            entity.Property(e => e.RulesetVersionId).HasColumnName("ruleset_version_id");
        });

        modelBuilder.Entity<SecurityAuditLogDb>(entity =>
        {
            entity.ToTable("security_audit_logs");
            entity.HasKey(e => e.SecurityAuditLogId);
            entity.Property(e => e.SecurityAuditLogId).HasColumnName("security_audit_log_id");
            entity.Property(e => e.OccurredAt).HasColumnName("occurred_at");
            entity.Property(e => e.TraceId).HasColumnName("trace_id");
            entity.Property(e => e.EventType).HasColumnName("event_type");
            entity.Property(e => e.Outcome).HasColumnName("outcome");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Username).HasColumnName("username");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.IpAddress).HasColumnName("ip_address");
            entity.Property(e => e.UserAgent).HasColumnName("user_agent");
            entity.Property(e => e.Method).HasColumnName("method");
            entity.Property(e => e.Path).HasColumnName("path");
            entity.Property(e => e.StatusCode).HasColumnName("status_code");
            entity.Property(e => e.DetailJson).HasColumnName("detail_json");
        });
    }
}
```

- [ ] **Step 3: Register AppDbContext in Program.cs**

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
```

Add `using Microsoft.EntityFrameworkCore;` at the top of Program.cs.

- [ ] **Step 4: Build**

```bash
dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 5: Commit**

```bash
git add src/Cashflowpoly.Api/
git commit -m "feat: add EF Core AppDbContext for schema migrations"
```

---

## Task 11: Generate EF Core Initial Migration

**Files:**
- Create: `src/Cashflowpoly.Api/Data/Migrations/` (generated)

- [ ] **Step 1: Install EF Core tools if not present**

```bash
dotnet tool install --global dotnet-ef
```

If already installed:
```bash
dotnet tool update --global dotnet-ef
```

- [ ] **Step 2: Read the existing schema SQL**

Open `database/00_create_schema.sql` and review all CREATE TABLE statements. The EF Core migration must produce equivalent SQL.

- [ ] **Step 3: Generate initial migration**

```bash
dotnet ef migrations add InitialSchema --project src/Cashflowpoly.Api --output-dir Data/Migrations
```

Expected output: `Done. To undo this action, use 'ef migrations remove'`

This generates:
- `src/Cashflowpoly.Api/Data/Migrations/<timestamp>_InitialSchema.cs`
- `src/Cashflowpoly.Api/Data/Migrations/<timestamp>_InitialSchema.Designer.cs`
- `src/Cashflowpoly.Api/Data/Migrations/AppDbContextModelSnapshot.cs`

- [ ] **Step 4: Review generated migration**

Open the generated `<timestamp>_InitialSchema.cs` and verify the `Up()` method creates tables matching `database/00_create_schema.sql`. Check:
- Table names match exactly
- Column names match exactly (snake_case)
- Primary keys match
- Nullable columns match

If there are discrepancies, add raw SQL in the migration:
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // EF Core generated code, then override with raw SQL if needed:
    migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pgcrypto;");
    // etc.
}
```

- [ ] **Step 5: Add migration run to Program.cs startup**

```csharp
// After var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}
```

This replaces `AuthSchemaBootstrapper` as the migration runner. Keep the seed user logic by moving it into a separate `SeedService` called right after:

```csharp
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedDefaultUsersAsync();
}
```

Create `src/Cashflowpoly.Api/Data/DatabaseSeeder.cs` with the seed user logic moved from `AuthSchemaBootstrapper.SeedUserAsync`.

- [ ] **Step 6: Build**

```bash
dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 7: Commit**

```bash
git add src/Cashflowpoly.Api/Data/Migrations/ src/Cashflowpoly.Api/Data/AppDbContext.cs src/Cashflowpoly.Api/Data/DatabaseSeeder.cs src/Cashflowpoly.Api/Program.cs
git commit -m "feat: add EF Core InitialSchema migration, replace AuthSchemaBootstrapper"
```

---

## Task 12: Remove Old Migration System

**Files:**
- Delete: `src/Cashflowpoly.Api/Data/DatabaseMigration.cs`
- Delete: `src/Cashflowpoly.Api/Data/AuthSchemaMigrator.cs`
- Delete: `src/Cashflowpoly.Api/Data/AuthSchemaMigrations.cs`
- Delete: `src/Cashflowpoly.Api/Data/AuthSchemaBootstrapper.cs`
- Delete: `tests/Cashflowpoly.Api.Tests/AuthSchemaMigrationTests.cs`
- Modify: `src/Cashflowpoly.Api/Program.cs` (remove `AddHostedService<AuthSchemaBootstrapper>`)

- [ ] **Step 1: Delete migration files**

```bash
del src\Cashflowpoly.Api\Data\DatabaseMigration.cs
del src\Cashflowpoly.Api\Data\AuthSchemaMigrator.cs
del src\Cashflowpoly.Api\Data\AuthSchemaMigrations.cs
del src\Cashflowpoly.Api\Data\AuthSchemaBootstrapper.cs
```

- [ ] **Step 2: Remove hosted service registration from Program.cs**

Find and remove:
```csharp
builder.Services.AddHostedService<AuthSchemaBootstrapper>();
```

Also remove:
```csharp
builder.Services.Configure<AuthBootstrapOptions>(builder.Configuration.GetSection("AuthBootstrap"));
```

Remove `using Cashflowpoly.Api.Security;` for `AuthBootstrapOptions` if it's no longer needed elsewhere.

- [ ] **Step 3: Delete migration test file**

```bash
del tests\Cashflowpoly.Api.Tests\AuthSchemaMigrationTests.cs
```

- [ ] **Step 4: Delete legacy helper test**

```bash
del tests\Cashflowpoly.Api.Tests\LegacyApiCompatibilityHelperTests.cs
```

- [ ] **Step 5: Build and test**

```bash
dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
dotnet test tests/Cashflowpoly.Api.Tests/
```

Expected: Build and all remaining tests pass.

- [ ] **Step 6: Commit**

```bash
git add -A
git commit -m "refactor: remove custom migration system, replaced by EF Core"
```

---

## Task 13: Add OpenTelemetry Metrics

**Files:**
- Modify: `src/Cashflowpoly.Api/Cashflowpoly.Api.csproj`
- Create: `src/Cashflowpoly.Api/Infrastructure/Telemetry/AppMetrics.cs`
- Modify: `src/Cashflowpoly.Api/Program.cs`

- [ ] **Step 1: Add OTel packages to Api.csproj**

```xml
<PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.11.2" />
<PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.11.1" />
<PackageReference Include="OpenTelemetry.Exporter.Prometheus.AspNetCore" Version="1.11.0-alpha.1" />
```

Restore:
```bash
dotnet restore src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
```

- [ ] **Step 2: Create AppMetrics.cs**

```csharp
// src/Cashflowpoly.Api/Infrastructure/Telemetry/AppMetrics.cs
using System.Diagnostics.Metrics;

namespace Cashflowpoly.Api.Infrastructure.Telemetry;

public static class AppMetrics
{
    public const string MeterName = "Cashflowpoly.Api";

    private static readonly Meter Meter = new(MeterName, "1.0");

    public static readonly Counter<long> RequestsTotal =
        Meter.CreateCounter<long>("http_requests_total", "requests", "Total HTTP requests processed");

    public static readonly Histogram<double> RequestDurationMs =
        Meter.CreateHistogram<double>("http_request_duration_ms", "ms", "HTTP request duration in milliseconds");

    public static readonly Counter<long> RequestErrorsTotal =
        Meter.CreateCounter<long>("http_request_errors_total", "requests", "Total HTTP requests that returned 4xx or 5xx");
}
```

- [ ] **Step 3: Register OTel in Program.cs**

Add after existing service registrations:

```csharp
using OpenTelemetry.Metrics;

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddMeter(AppMetrics.MeterName)
        .AddPrometheusExporter());
```

Add `using Cashflowpoly.Api.Infrastructure.Telemetry;` at the top.

- [ ] **Step 4: Add Prometheus scraping endpoint**

In the app middleware section, after `app.MapControllers()`:

```csharp
app.MapPrometheusScrapingEndpoint("/metrics");
```

- [ ] **Step 5: Update request logging middleware to use OTel**

Find the existing request logging middleware in Program.cs:

```csharp
app.Use(async (context, next) =>
{
    var start = Stopwatch.GetTimestamp();
    await next();
    var durationMs = (Stopwatch.GetTimestamp() - start) * 1000.0 / Stopwatch.Frequency;

    metricsTracker.Record(context, durationMs);  // REMOVE this line

    // Add these lines instead:
    AppMetrics.RequestsTotal.Add(1);
    AppMetrics.RequestDurationMs.Record(durationMs);
    if (context.Response.StatusCode >= 400)
    {
        AppMetrics.RequestErrorsTotal.Add(1);
    }

    // keep the requestLogger.LogInformation(...) call unchanged
});
```

- [ ] **Step 6: Build**

```bash
dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 7: Commit**

```bash
git add src/Cashflowpoly.Api/
git commit -m "feat: replace OperationalMetricsTracker with OpenTelemetry + Prometheus"
```

---

## Task 14: Remove OperationalMetricsTracker

**Files:**
- Delete: `src/Cashflowpoly.Api/Infrastructure/OperationalMetricsTracker.cs`
- Modify: `src/Cashflowpoly.Api/Program.cs`
- Modify: `src/Cashflowpoly.Api/Controllers/ObservabilityController.cs`

- [ ] **Step 1: Delete OperationalMetricsTracker.cs**

```bash
del src\Cashflowpoly.Api\Infrastructure\OperationalMetricsTracker.cs
```

- [ ] **Step 2: Remove tracker from Program.cs**

Remove:
```csharp
builder.Services.AddSingleton<OperationalMetricsTracker>();
```

Remove:
```csharp
var metricsTracker = app.Services.GetRequiredService<OperationalMetricsTracker>();
```

- [ ] **Step 3: Update ObservabilityController**

Open `src/Cashflowpoly.Api/Controllers/ObservabilityController.cs`. It likely uses `OperationalMetricsTracker`. Replace that usage with a note pointing to the `/metrics` Prometheus endpoint, or remove the endpoint if it's now superseded:

```csharp
[HttpGet("metrics/summary")]
public IActionResult GetMetricsSummary()
{
    return Ok(new { message = "Metrics available at /metrics (Prometheus format)" });
}
```

- [ ] **Step 4: Build and run all tests**

```bash
dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
dotnet test tests/Cashflowpoly.Api.Tests/
```

Expected: Build and all tests pass.

- [ ] **Step 5: Commit**

```bash
git add src/Cashflowpoly.Api/
git commit -m "refactor: remove OperationalMetricsTracker, metrics now at /metrics"
```

---

## Task 15: Cleanup

**Files:**
- Move: `src/Cashflowpoly.Api/Controllers/ApiErrorHelper.cs` → `src/Cashflowpoly.Api/Infrastructure/ApiErrorHelper.cs`
- Modify: `src/Cashflowpoly.Ui/Infrastructure/UiText.cs`
- Delete: `src/Cashflowpoly.Api/Security/LegacyApiCompatibilityHelper.cs`
- Modify: `src/Cashflowpoly.Api/Program.cs`
- Modify: all domain files (remove Indonesian file comments and verbose XML docs)

- [ ] **Step 1: Move ApiErrorHelper to Infrastructure**

Create `src/Cashflowpoly.Api/Infrastructure/ApiErrorHelper.cs` with same content as `Controllers/ApiErrorHelper.cs` but change namespace from `Cashflowpoly.Api.Controllers` to `Cashflowpoly.Api.Infrastructure`.

Delete `src/Cashflowpoly.Api/Controllers/ApiErrorHelper.cs`.

Update all `using Cashflowpoly.Api.Controllers;` references that were only there for `ApiErrorHelper` — change to `using Cashflowpoly.Api.Infrastructure;` in:
- `Program.cs`
- Any controller that uses `ApiErrorHelper` directly

- [ ] **Step 2: Fix UiText to accept string culture instead of HttpContext**

Open `src/Cashflowpoly.Ui/Infrastructure/UiText.cs`. The `Translate(HttpContext context, string key)` method extracts the language from the session. Refactor to:

```csharp
// Before
public static string Translate(HttpContext context, string key) { ... }
public static string TranslateSessionStatus(HttpContext context, string? status) { ... }

// After
public static string Translate(string culture, string key)
{
    var normalized = NormalizeLanguage(culture);
    if (!Lexicon.TryGetValue(key, out var entry)) return key;
    return string.Equals(normalized, AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase)
        ? entry.En
        : entry.Id;
}

public static string TranslateSessionStatus(string culture, string? status) { ... }
```

Update all callers in Ui controllers and views to extract culture from session first:
```csharp
// In controller action
var culture = HttpContext.Session.GetString(AuthConstants.SessionLanguageKey) ?? AuthConstants.LanguageId;
var label = UiText.Translate(culture, "some.key");
```

- [ ] **Step 3: Remove legacy middleware**

Delete `src/Cashflowpoly.Api/Security/LegacyApiCompatibilityHelper.cs`.

In `Program.cs`, remove:
```csharp
var enableLegacyApiCompatibility = builder.Configuration.GetValue<bool>("FeatureFlags:EnableLegacyApiCompatibility");
```

And remove the middleware block:
```csharp
if (enableLegacyApiCompatibility)
{
    app.Use(async (context, next) =>
    {
        if (LegacyApiCompatibilityHelper.TryRewritePath(context.Request.Path, out var rewrittenPath))
        {
            context.Request.Path = rewrittenPath;
        }
        await next();
    });
}
```

- [ ] **Step 4: Remove Indonesian file-level comments**

For every `.cs` file that starts with `// Fungsi file: ...`, remove that first line. Files in scope:
- All files in `src/Cashflowpoly.Api/`
- All files in `src/Cashflowpoly.Ui/`

This can be done with a find-and-replace: search for lines matching `^// Fungsi file:.*$` (regex, line by line) and delete them.

PowerShell one-liner to preview affected files:
```powershell
Get-ChildItem -Recurse -Filter "*.cs" src/ | Select-String "^// Fungsi file:" | Select-Object -ExpandProperty Path | Sort-Object -Unique
```

Edit each file to remove the first line.

- [ ] **Step 5: Remove verbose XML doc comments from controllers**

For every `/// <summary>` block on controller methods and class declarations, delete them. This applies to all files in `src/Cashflowpoly.Api/Controllers/` and `src/Cashflowpoly.Ui/Controllers/`.

Keep XML docs only where they genuinely add non-obvious information.

- [ ] **Step 6: Build and run all tests**

```bash
dotnet build
dotnet test tests/Cashflowpoly.Api.Tests/
dotnet test tests/Cashflowpoly.Ui.Tests/
```

Expected: Build and all tests pass.

- [ ] **Step 7: Commit**

```bash
git add -A
git commit -m "refactor: cleanup - move ApiErrorHelper, fix UiText, remove legacy middleware and noise comments"
```

---

## Task 16: Final Verification

- [ ] **Step 1: Full solution build**

```bash
dotnet build Cashflowpoly.sln
```

Expected: `Build succeeded. 0 Warning(s) 0 Error(s)` (TreatWarningsAsErrors is on — zero warnings required)

- [ ] **Step 2: Run all test suites**

```bash
dotnet test Cashflowpoly.sln
```

Expected: All tests pass. Note test count — it should be equal to or greater than the count before refactor.

- [ ] **Step 3: Verify no leftover static domain calls in controllers or services**

```bash
# Should return no matches
grep -r "AnalyticsHappinessCalculator\." src/Cashflowpoly.Api/Controllers/
grep -r "AnalyticsHappinessCalculator\." src/Cashflowpoly.Api/Services/
grep -r "static class" src/Cashflowpoly.Api/Domain/
```

Expected: No results (only `AnalyticsMath` should still be static).

- [ ] **Step 4: Verify no duplicate DTOs remain**

```bash
grep -r "namespace Cashflowpoly.Api.Models" src/
grep -r "namespace Cashflowpoly.Ui.Models" src/Cashflowpoly.Ui/Models/ApiDtos.cs
```

Expected: No results — both ApiDtos files are deleted.

- [ ] **Step 5: Verify OTel endpoint responds**

Start the API locally and verify `/metrics` returns Prometheus-format output:

```bash
dotnet run --project src/Cashflowpoly.Api
curl http://localhost:5000/metrics
```

Expected: Response containing `# HELP http_requests_total` and `# TYPE http_requests_total counter`.

- [ ] **Step 6: Final commit**

```bash
git add -A
git commit -m "chore: final verification pass - architecture refactor complete"
```
