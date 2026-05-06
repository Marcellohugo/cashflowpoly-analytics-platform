# Architecture Refactor Design
**Date:** 2026-05-06  
**Scope:** Big-bang refactor — all architectural issues resolved in one branch  
**Goals:** Testability, scalability, onboarding clarity

---

## 1. Project Structure

### Before
```
Cashflowpoly.sln
├── src/Cashflowpoly.Api
├── src/Cashflowpoly.Ui
├── tests/Cashflowpoly.Api.Tests
└── tests/Cashflowpoly.Ui.Tests
```

### After
```
Cashflowpoly.sln
├── src/Cashflowpoly.Contracts      ← NEW: shared DTOs, enums, request/response types
├── src/Cashflowpoly.Api
├── src/Cashflowpoly.Ui
├── tests/Cashflowpoly.Api.Tests
└── tests/Cashflowpoly.Ui.Tests
```

### Dependency Rules
- `Cashflowpoly.Api` → references `Cashflowpoly.Contracts`
- `Cashflowpoly.Ui` → references `Cashflowpoly.Contracts`
- `Cashflowpoly.Contracts` → references nothing (pure DTOs and enums only, no business logic)

### What moves to Contracts
- All request/response DTOs currently in `Cashflowpoly.Api/Models/ApiDtos.cs`
- All DTOs currently in `Cashflowpoly.Ui/Models/ApiDtos.cs` (duplicates removed)
- Shared enums (e.g. `PlayerOrdering`)
- `ErrorResponse` model

---

## 2. Domain Layer: Static Classes → DI Services

### Problem
37+ `internal static class` in `Domain/` — cannot be injected, mocked, or tested in isolation.

### Solution
Every static class becomes an interface + sealed implementation:

```csharp
// Before
internal static class AnalyticsHappinessCalculator
{
    public static AnalyticsHappinessBreakdown ComputeByPlayer(...) { }
}

// After
public interface IHappinessCalculator
{
    Dictionary<Guid, AnalyticsHappinessBreakdown> ComputeByPlayer(...);
    AnalyticsHappinessBreakdown ComputeBreakdown(...);
}

internal sealed class HappinessCalculator : IHappinessCalculator { ... }
```

### Folder Structure in Cashflowpoly.Api
```
Domain/
├── Interfaces/      ← IHappinessCalculator, IIngredientInventoryCalculator, etc.
├── Services/        ← concrete implementations (internal sealed)
└── Models/          ← AnalyticsHappinessBreakdown, RulesetConfig, etc.
```

### DI Registration (Program.cs)
All domain services registered as `Scoped`:
```csharp
builder.Services.AddScoped<IHappinessCalculator, HappinessCalculator>();
builder.Services.AddScoped<IIngredientInventoryCalculator, IngredientInventoryCalculator>();
builder.Services.AddScoped<IPrimaryNeedComplianceEvaluator, PrimaryNeedComplianceEvaluator>();
builder.Services.AddScoped<IAnalyticsSnapshotBuilder, AnalyticsSnapshotBuilder>();
builder.Services.AddScoped<ISessionMetricCalculator, SessionMetricCalculator>();
builder.Services.AddScoped<IMetricSnapshotBuilder, MetricSnapshotBuilder>();
builder.Services.AddScoped<IScoreCalculator, ScoreCalculator>();
builder.Services.AddScoped<IPlayerOrdering, PlayerOrderingService>();
builder.Services.AddScoped<IPayloadReader, PayloadReader>();
// ... all 37+ domain services
```

### Payload Readers
`EventPayloadReader` and `AnalyticsPayloadReader` are merged into a single `IPayloadReader` service — they read the same JSON payload format, only differed by calling context.

---

## 3. Application Services: Fat Controllers → Thin Controllers

### Problem
`AnalyticsController` contains ~400 lines of private business logic methods (`BuildByPlayerAsync`, `ComputePlayerMetricsAsync`, `WriteSnapshotsAsync`). Controllers should only handle HTTP concerns.

### Solution

```
Api/
├── Controllers/    ← HTTP only: auth checks, routing, response codes
├── Services/       ← business orchestration
│   ├── IAnalyticsService.cs
│   ├── AnalyticsService.cs
│   ├── IEventIngestionService.cs
│   └── EventIngestionService.cs
└── Domain/         ← pure calculators (Section 2)
```

### Controller Pattern After Refactor
```csharp
public sealed class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analytics;

    public AnalyticsController(IAnalyticsService analytics)
        => _analytics = analytics;

    [HttpGet("sessions/{sessionId:guid}")]
    public async Task<IActionResult> GetSessionAnalytics(Guid sessionId, CancellationToken ct)
    {
        var result = await _analytics.GetSessionAnalyticsAsync(sessionId, User, ct);
        return result.Match<IActionResult>(Ok, NotFound, Forbid);
    }
}
```

Controllers contain: routing attributes, auth attributes, HTTP status mapping. Nothing else.

### Business Logic Extracted
| Controller Method | Moves To |
|---|---|
| `BuildByPlayerAsync` | `AnalyticsService` |
| `ComputePlayerMetricsAsync` | `AnalyticsService` |
| `WriteSnapshotsAsync` | `AnalyticsService` |
| `ResolvePlayerScopeAsync` | `AnalyticsService` |
| `EnsureInstructorSessionAccessAsync` | `AnalyticsService` |
| Event validation logic in `EventsController` | `EventIngestionService` |
| Hardcoded rulebook constants (`RulebookLoanPrincipal = 10`, etc.) | `RulesetConfig` domain model (already exists, just use it) |

### Service Registration
```csharp
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IEventIngestionService, EventIngestionService>();
```

---

## 4. EF Core Migrations

### Problem
Custom `DatabaseMigration` + `AuthSchemaMigrator` + `AuthSchemaBootstrapper` with SHA256 checksums. Reinvents what EF Core already provides.

### Solution
Add EF Core with Npgsql provider (Npgsql already in project).

### New Files
```
Api/Data/
├── AppDbContext.cs          ← EF Core DbContext with all entity DbSets
├── Migrations/              ← generated by `dotnet ef migrations add`
└── (existing repositories kept as-is — Dapper still used for queries)
```

### DbContext Pattern
```csharp
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserDb> Users { get; set; }
    public DbSet<SessionDb> Sessions { get; set; }
    public DbSet<EventDb> Events { get; set; }
    public DbSet<MetricSnapshotDb> MetricSnapshots { get; set; }
    // ...

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

### Hybrid Dapper + EF Core
Repositories keep Dapper for reads and writes (performant, existing, tested). EF Core is used **only for schema migrations** — not for queries. This is a valid and common pattern.

### Migration Workflow
```bash
dotnet ef migrations add InitialSchema --project src/Cashflowpoly.Api
dotnet ef database update --project src/Cashflowpoly.Api
```

### Files Deleted
- `Data/DatabaseMigration.cs`
- `Data/AuthSchemaMigrator.cs`
- `Data/AuthSchemaMigrations.cs`
- `Data/AuthSchemaBootstrapper.cs` (IHostedService removed)

`dotnet ef database update` replaces `AuthSchemaBootstrapper`. Run in CI/CD pipeline or startup script, not as a HostedService.

---

## 5. OpenTelemetry Metrics

### Problem
`OperationalMetricsTracker` uses `ConcurrentDictionary` in-memory — data lost on restart, no dashboarding, not scrape-compatible.

### Solution
Replace with OpenTelemetry Metrics + Prometheus exporter.

### New Packages
```xml
<PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.x" />
<PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.x" />
<PackageReference Include="OpenTelemetry.Exporter.Prometheus.AspNetCore" Version="1.x" />
```

### New File: Infrastructure/Telemetry/AppMetrics.cs
```csharp
public static class AppMetrics
{
    private static readonly Meter Meter = new("Cashflowpoly.Api", "1.0");

    public static readonly Counter<long> RequestsTotal =
        Meter.CreateCounter<long>("http_requests_total", "requests", "Total HTTP requests");

    public static readonly Histogram<double> RequestDurationMs =
        Meter.CreateHistogram<double>("http_request_duration_ms", "ms", "HTTP request duration");
}
```

### Program.cs Changes
```csharp
// Add
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddMeter("Cashflowpoly.Api")
        .AddPrometheusExporter());

// Add endpoint
app.MapPrometheusScrapingEndpoint("/metrics");

// Remove
// builder.Services.AddSingleton<OperationalMetricsTracker>();
// var metricsTracker = app.Services.GetRequiredService<OperationalMetricsTracker>();
// metricsTracker.Record(context, durationMs);  ← remove from middleware
```

### Files Deleted
- `Infrastructure/OperationalMetricsTracker.cs`

Request logging middleware stays — only the `metricsTracker.Record()` call is removed.

---

## 6. Cleanup

| # | Item | Action |
|---|------|--------|
| 1 | `LegacyApiCompatibilityHelper` + `EnableLegacyApiCompatibility` feature flag | Confirm no active clients on `/api/` (without `/v1/`). If confirmed, delete helper + middleware block + feature flag config. If still needed, extract to proper `IMiddleware` class. |
| 2 | `ApiErrorHelper.cs` in `Controllers/` folder | Move to `Infrastructure/ApiErrorHelper.cs` |
| 3 | `UiText.Translate(HttpContext, key)` passes full HttpContext | Refactor to accept `string culture` instead of `HttpContext` — extract culture from HttpContext at controller level, pass string down |
| 4 | `using static Domain.AnalyticsPayloadReader` in controllers | Removed automatically after Section 2 & 3 — payload reading becomes domain service |
| 5 | Verbose XML doc comments on all controller methods | Remove — method names and routes are self-documenting |
| 6 | Indonesian `// Fungsi file: ...` comments at top of every file | Remove — not informative for self-evident code |

---

## Out of Scope
- UI/UX changes
- New analytics features
- Database schema changes
- Authentication flow changes
- Test coverage additions (existing tests should continue passing after refactor)

---

## Success Criteria
1. All existing tests pass after refactor
2. Controllers contain no business logic (no loops, no DB calls, no calculations)
3. All domain calculators injectable via DI (no `static` calls in controllers or services)
4. Single source of truth for DTOs — `Cashflowpoly.Contracts` only
5. `dotnet ef migrations` generates valid schema matching current DB
6. `/metrics` endpoint returns Prometheus-compatible output
7. No `OperationalMetricsTracker`, `AuthSchemaMigrator`, or `ApiDtos.cs` (duplicated) remain
