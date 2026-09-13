// Fungsi file: Mengonfigurasi dependency, middleware, endpoint, dan startup API.
// Mengimpor namespace `System.Security.Claims` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Security.Claims;
// Mengimpor namespace `System.Diagnostics` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Diagnostics;
// Mengimpor namespace `System.Net` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net;
// Mengimpor namespace `System.Threading.RateLimiting` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Threading.RateLimiting;
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Cashflowpoly.Api.Infrastructure` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Infrastructure;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Cashflowpoly.Api.Security` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Security;
// Mengimpor namespace `Microsoft.AspNetCore.Authentication.JwtBearer` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama
// lengkapnya.
using Microsoft.AspNetCore.Authentication.JwtBearer;
// Mengimpor namespace `Microsoft.EntityFrameworkCore` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.EntityFrameworkCore;
// Mengimpor namespace `Microsoft.AspNetCore.Diagnostics` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Diagnostics;
// Mengimpor namespace `Microsoft.AspNetCore.Diagnostics.HealthChecks` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama
// lengkapnya.
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
// Mengimpor namespace `Microsoft.AspNetCore.HttpOverrides` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.HttpOverrides;
// Mengimpor namespace `Microsoft.AspNetCore.RateLimiting` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.RateLimiting;
// Mengimpor namespace `Microsoft.AspNetCore.Mvc` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Mvc;
// Mengimpor namespace `Microsoft.Extensions.Diagnostics.HealthChecks` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama
// lengkapnya.
using Microsoft.Extensions.Diagnostics.HealthChecks;
// Mengimpor namespace `Microsoft.Extensions.Options` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.Extensions.Options;
// Mengimpor namespace `Microsoft.IdentityModel.Tokens` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.IdentityModel.Tokens;
// Mengimpor namespace `Microsoft.OpenApi` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.OpenApi;
// Mengimpor namespace `OpenTelemetry.Metrics` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using OpenTelemetry.Metrics;
// Mengimpor namespace `Cashflowpoly.Api.Infrastructure.Telemetry` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama
// lengkapnya.
using Cashflowpoly.Api.Infrastructure.Telemetry;

var builder = WebApplication.CreateBuilder(args);
var bypassOperationalRateLimit = builder.Environment.IsEnvironment("Testing");
var migrateOnly = args.Any(argument => string.Equals(argument, "--migrate-only", StringComparison.OrdinalIgnoreCase));
var recalculateAnalytics = args.Any(argument => string.Equals(argument, "--recalculate-analytics", StringComparison.OrdinalIgnoreCase));

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”ConnectionStrings:Default belum
    // dikonfigurasi.”); pemanggil atau middleware penanganan error menerima kegagalan ini.
    throw new InvalidOperationException("ConnectionStrings:Default belum dikonfigurasi.");
}
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtSection);
builder.Services.Configure<AuthRegistrationOptions>(options =>
{
    options.AllowPublicInstructorRegistration =
        builder.Configuration.GetValue("Auth:AllowPublicInstructorRegistration", true);
});
builder.Services.AddSingleton<JwtSigningKeyProvider>();
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = 1;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();

    var trustedProxies = builder.Configuration.GetSection("Networking:TrustedProxies").Get<string[]>();
    // Mengulangi setiap elemen `trustedProxies ?? []`; elemen saat ini disimpan sebagai `proxy` bertipe `var` untuk diproses oleh badan loop.
    foreach (var proxy in trustedProxies ?? [])
    {
        if (IPAddress.TryParse(proxy, out var ip))
        {
            options.KnownProxies.Add(ip);
        }
    }

    var trustedNetworks = builder.Configuration.GetSection("Networking:TrustedNetworks").Get<string[]>();
    // Mengulangi setiap elemen `trustedNetworks ?? []`; elemen saat ini disimpan sebagai `network` bertipe `var` untuk diproses oleh badan loop.
    foreach (var network in trustedNetworks ?? [])
    {
        if (System.Net.IPNetwork.TryParse(network, out var parsedNetwork))
        {
            options.KnownIPNetworks.Add(parsedNetwork);
        }
    }
});
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var details = context.ModelState
                .SelectMany(entry => entry.Value?.Errors.Select(error => new ErrorDetail(
                    string.IsNullOrWhiteSpace(entry.Key)
                        ? "request"
                        : entry.Key.TrimStart('$', '.'),
                    error.Exception is not null
                        ? "INVALID_FORMAT"
                        : error.ErrorMessage.Contains("required", StringComparison.OrdinalIgnoreCase)
                            ? "REQUIRED"
                            : "INVALID_VALUE")) ?? [])
                .Distinct()
                .ToArray();

            if (details.Length == 0)
            {
                details = [new ErrorDetail("request", "INVALID_VALUE")];
            }

            return new BadRequestObjectResult(ApiErrorHelper.BuildError(
                context.HttpContext,
                "VALIDATION_ERROR",
                "Request tidak valid",
                details));
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<DatabaseHealthCheck>("database", tags: ["ready"]);
builder.Services.AddSwaggerGen(options =>
{
    var bearerScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Masukkan token JWT. Contoh: Bearer {token}"
    };

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cashflowpoly API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", bearerScheme);

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document, null)] = new List<string>()
    });

    options.OperationFilter<StandardResponseOperationFilter>();
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtOptions>, JwtSigningKeyProvider>((options, jwtOptionsAccessor, signingKeyProvider) =>
    {
        var jwtOptions = jwtOptionsAccessor.Value;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeyResolver = (_, _, kid, _) => signingKeyProvider.ResolveValidationKeys(kid),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = async context =>
            {
                context.HttpContext.Items["security_auth_audit_written"] = true;
                var audit = context.HttpContext.RequestServices.GetRequiredService<SecurityAuditService>();
                await audit.LogAsync(
                    context.HttpContext,
                    SecurityAuditEventTypes.AuthFailed,
                    SecurityAuditOutcomes.Failure,
                    StatusCodes.Status401Unauthorized,
                    new
                    {
                        reason = "AUTHENTICATION_FAILED",
                        exception = context.Exception.GetType().Name
                    },
                    context.HttpContext.RequestAborted);
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();
                if (!context.HttpContext.Items.ContainsKey("security_auth_audit_written"))
                {
                    context.HttpContext.Items["security_auth_audit_written"] = true;
                    var audit = context.HttpContext.RequestServices.GetRequiredService<SecurityAuditService>();
                    await audit.LogAsync(
                        context.HttpContext,
                        SecurityAuditEventTypes.AuthChallenge,
                        SecurityAuditOutcomes.Denied,
                        StatusCodes.Status401Unauthorized,
                        new
                        {
                            reason = "MISSING_OR_INVALID_TOKEN"
                        },
                        context.HttpContext.RequestAborted);
                }

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var error = ApiErrorHelper.BuildError(
                    context.HttpContext,
                    "UNAUTHORIZED",
                    "Token user tidak valid");
                await context.Response.WriteAsJsonAsync(error, cancellationToken: context.HttpContext.RequestAborted);
            },
            OnForbidden = async context =>
            {
                if (!context.HttpContext.Items.ContainsKey("security_auth_audit_written"))
                {
                    context.HttpContext.Items["security_auth_audit_written"] = true;
                    var audit = context.HttpContext.RequestServices.GetRequiredService<SecurityAuditService>();
                    await audit.LogAsync(
                        context.HttpContext,
                        SecurityAuditEventTypes.AuthForbidden,
                        SecurityAuditOutcomes.Denied,
                        StatusCodes.Status403Forbidden,
                        new
                        {
                            reason = "ROLE_OR_SCOPE_FORBIDDEN"
                        },
                        context.HttpContext.RequestAborted);
                }

                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var error = ApiErrorHelper.BuildError(
                    context.HttpContext,
                    "FORBIDDEN",
                    "Role tidak diizinkan");
                await context.Response.WriteAsJsonAsync(error, cancellationToken: context.HttpContext.RequestAborted);
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        var securityAudit = context.HttpContext.RequestServices.GetRequiredService<SecurityAuditService>();
        await securityAudit.LogAsync(
            context.HttpContext,
            SecurityAuditEventTypes.RateLimited,
            SecurityAuditOutcomes.Denied,
            StatusCodes.Status429TooManyRequests,
            new
            {
                reason = "RATE_LIMIT_POLICY_TRIGGERED"
            },
            token);

        var error = ApiErrorHelper.BuildError(context.HttpContext, "RATE_LIMITED", "Terlalu banyak request");
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(error, cancellationToken: token);
    };
    options.AddPolicy("api", httpContext =>
    {
        // Pengujian integrasi memvalidasi kontrak endpoint secara paralel dari satu alamat proses.
        // Menyiapkan variabel lokal `permitLimit` untuk nilai permit limit dengan hasil pemilihan bersyarat: ketika `bypassOperationalRateLimit` benar
        // gunakan `10_000`, jika tidak gunakan `RateLimitPolicyHelper.ResolvePermitLimit(httpContext.Request.Path)`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var permitLimit = bypassOperationalRateLimit
            ? 10_000
            : RateLimitPolicyHelper.ResolvePermitLimit(httpContext.Request.Path);
        var partitionKey = RateLimitPolicyHelper.BuildPartitionKey(httpContext);

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = permitLimit,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            AutoReplenishment = true
        });
    });
});
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddMeter(AppMetrics.MeterName)
        .AddPrometheusExporter());
var postgresDataSource = Npgsql.NpgsqlDataSource.Create(connectionString);
builder.Services.AddSingleton(postgresDataSource);
builder.Services.AddHostedService<LogRetentionWorker>();
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
    options.UseNpgsql(serviceProvider.GetRequiredService<Npgsql.NpgsqlDataSource>()));
builder.Services.AddScoped<RulesetRepository>();
builder.Services.AddScoped<SessionRepository>();
builder.Services.AddScoped<EventRepository>();
builder.Services.AddScoped<MetricsRepository>();
builder.Services.AddScoped<PlayerRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<SessionStateRepository>();
builder.Services.AddScoped<SessionEventProjector>();
builder.Services.AddScoped<SecurityAuditRepository>();
builder.Services.AddScoped<SecurityAuditService>();
// Domain calculators
// Menjalankan mendaftarkan layanan `builder.Services.AddScoped<IHappinessCalculator, HappinessCalculator>` dengan masa hidup satu instance per
// scope/request.
builder.Services.AddScoped<IHappinessCalculator, HappinessCalculator>();
builder.Services.AddScoped<IIngredientInventoryCalculator, IngredientInventoryCalculator>();
builder.Services.AddScoped<ISessionMetricCalculator, SessionMetricCalculator>();
builder.Services.AddScoped<IMetricSnapshotBuilder, MetricSnapshotBuilder>();
builder.Services.AddScoped<IPlayerOrdering, PlayerOrderingService>();
builder.Services.AddScoped<IScoreCalculator, ScoreCalculator>();
builder.Services.AddScoped<IAnalyticsPayloadReader, AnalyticsPayloadReader>();
builder.Services.AddScoped<IEventPayloadReader, EventPayloadReader>();
builder.Services.AddScoped<IGameplaySnapshotBuilder, GameplaySnapshotBuilder>();
builder.Services.AddScoped<IEventCashflowProjectionBuilder, EventCashflowProjectionBuilder>();
builder.Services.AddScoped<IEventRecordMapper, EventRecordMapper>();
builder.Services.AddScoped<IEventRequestShapeValidator, EventRequestShapeValidator>();
builder.Services.AddScoped<IEventSimpleActionValidator, EventSimpleActionValidator>();
builder.Services.AddScoped<IEventTurnProgressValidator, EventTurnProgressValidator>();
builder.Services.AddScoped<IEventNeedPurchaseValidator, EventNeedPurchaseValidator>();
builder.Services.AddScoped<IEventIngredientOrderValidator, EventIngredientOrderValidator>();
builder.Services.AddScoped<IEventSavingGoalValidator, EventSavingGoalValidator>();
builder.Services.AddScoped<IEventEconomyActionValidator, EventEconomyActionValidator>();
builder.Services.AddScoped<IEventAssignmentValidator, EventAssignmentValidator>();
builder.Services.AddScoped<IEventDerivedStateCalculator, EventDerivedStateCalculator>();
builder.Services.AddScoped<IEventPlayerBalanceCalculator, EventPlayerBalanceCalculator>();
builder.Services.AddScoped<ICashTimelineCalculator, CashTimelineCalculator>();
builder.Services.AddScoped<IDonationGameplayCalculator, DonationGameplayCalculator>();
builder.Services.AddScoped<ISavingGoalCalculator, SavingGoalCalculator>();
builder.Services.AddScoped<IIngredientMealCalculator, IngredientMealCalculator>();
builder.Services.AddScoped<IGoldGameplayCalculator, GoldGameplayCalculator>();
builder.Services.AddScoped<INeedMissionCalculator, NeedMissionCalculator>();
builder.Services.AddScoped<IRiskLoanCalculator, RiskLoanCalculator>();
builder.Services.AddScoped<IActionUsageCalculator, ActionUsageCalculator>();
builder.Services.AddScoped<IIncomeDiversificationCalculator, IncomeDiversificationCalculator>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Cashflowpoly.Api.Services.IAnalyticsService, Cashflowpoly.Api.Services.AnalyticsService>();
builder.Services.AddScoped<Cashflowpoly.Api.Services.IEventIngestionService, Cashflowpoly.Api.Services.EventIngestionService>();

var app = builder.Build();

var applyDatabaseChanges = migrateOnly || app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing");
await DatabaseInitialization.InitializeAsync(app.Services, applyDatabaseChanges, CancellationToken.None);

if (migrateOnly)
{
    return;
}

using (var scope = app.Services.CreateScope())
{
    var bootstrapOptions = app.Configuration.GetSection("AuthBootstrap").Get<AuthBootstrapOptions>();
    if (bootstrapOptions is { SeedDefaultUsers: true })
    {
        var dataSource = scope.ServiceProvider.GetRequiredService<Npgsql.NpgsqlDataSource>();
        await using var seedConn = await dataSource.OpenConnectionAsync();
        await SeedBootstrapUserAsync(seedConn, bootstrapOptions.InstructorUsername, bootstrapOptions.InstructorPassword, "INSTRUCTOR", CancellationToken.None);
        await SeedBootstrapUserAsync(seedConn, bootstrapOptions.PlayerUsername, bootstrapOptions.PlayerPassword, "PLAYER", CancellationToken.None);
    }
}

if (recalculateAnalytics)
{
    using var scope = app.Services.CreateScope();
    var sessions = await scope.ServiceProvider.GetRequiredService<SessionRepository>()
        .ListAllSessionsForMaintenanceAsync(CancellationToken.None);
    var analytics = scope.ServiceProvider.GetRequiredService<Cashflowpoly.Api.Services.IAnalyticsService>();
    var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
    var recalculationLogger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
        .CreateLogger("AnalyticsRecalculation");

    var recalculatedCount = 0;
    // Mengulangi setiap elemen `sessions.Where(item => item.InstructorUserId.HasValue)`; elemen saat ini disimpan sebagai `session` bertipe `var` untuk
    // diproses oleh badan loop.
    foreach (var session in sessions.Where(item => item.InstructorUserId.HasValue))
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, session.InstructorUserId!.Value.ToString()),
            new Claim(ClaimTypes.Role, "INSTRUCTOR")
        ], "maintenance"));
        httpContextAccessor.HttpContext = new DefaultHttpContext
        {
            User = principal,
            TraceIdentifier = $"recalculate-{session.SessionId:N}"
        };

        var (_, statusCode, error) = await analytics.RecomputeAsync(session.SessionId, principal, CancellationToken.None);
        if (statusCode != StatusCodes.Status200OK)
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( $”Rekalkulasi analitik sesi {session.SessionId}
            // gagal: {error?.ErrorCode ?? statusCode.ToString()}.”); pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException(
                $"Rekalkulasi analitik sesi {session.SessionId} gagal: {error?.ErrorCode ?? statusCode.ToString()}.");
        }

        recalculatedCount++;
    }

    recalculationLogger.LogInformation("Recalculated analytics for {SessionCount} sessions", recalculatedCount);
    return;
}

var requestLogger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("RequestAudit");
var exceptionLogger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("UnhandledException");
app.Services.GetRequiredService<JwtSigningKeyProvider>().ValidateConfiguration();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        context.TraceIdentifier = traceId;
        context.Response.Headers["X-Trace-Id"] = traceId;

        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception is not null)
        {
            exceptionLogger.LogError(
                exception,
                "Unhandled exception. trace_id={TraceId} span_id={SpanId} path={Path}",
                traceId,
                Activity.Current?.SpanId.ToString(),
                context.Request.Path.Value);
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        var error = ApiErrorHelper.BuildError(context, "INTERNAL_ERROR", "Terjadi kesalahan pada server");
        await context.Response.WriteAsJsonAsync(error);
    });
});

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cashflowpoly API v1");
    });
}

app.UseForwardedHeaders();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.Use(async (context, next) =>
{
    var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
    context.TraceIdentifier = traceId;
    context.Response.Headers["X-Trace-Id"] = traceId;

    var start = Stopwatch.GetTimestamp();
    var failed = false;
    try
    {
        await next();
    }
    // Menangani exception yang muncul dari blok try sebelumnya.
    catch
    {
        failed = true;
        // Melempar ulang exception yang sedang ditangani sambil mempertahankan jejak asal kegagalannya.
        throw;
    }
    finally
    {
        var durationMs = (Stopwatch.GetTimestamp() - start) * 1000.0 / Stopwatch.Frequency;
        var statusCode = failed ? StatusCodes.Status500InternalServerError : context.Response.StatusCode;

        AppMetrics.RequestsTotal.Add(1);
        AppMetrics.RequestDurationMs.Record(durationMs);
        if (statusCode >= 400)
        {
            AppMetrics.RequestErrorsTotal.Add(1);
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        var role = context.User.FindFirstValue(ClaimTypes.Role) ?? "anonymous";
        var clientRequestId = context.Request.Headers["X-Client-Request-Id"].ToString();
        var endpoint = context.GetEndpoint()?.DisplayName ?? context.Request.Path.Value;

        requestLogger.LogInformation(
            "request_completed trace_id={TraceId} span_id={SpanId} user_id={UserId} role={Role} method={Method} path={Path} endpoint={Endpoint} status_code={StatusCode} duration_ms={DurationMs} client_request_id={ClientRequestId}",
            traceId,
            Activity.Current?.SpanId.ToString(),
            userId,
            role,
            context.Request.Method,
            context.Request.Path.Value,
            endpoint,
            statusCode,
            Math.Round(durationMs, 2),
            string.IsNullOrWhiteSpace(clientRequestId) ? "-" : clientRequestId);
    }
});
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode is < 200 or >= 300)
    {
        return;
    }

    var isDemo = string.Equals(
        context.User.FindFirstValue(JwtTokenService.DemoAccountClaim),
        "true",
        StringComparison.OrdinalIgnoreCase);
    var eventType = ResolveOperationalAuditEvent(context.Request.Method, context.Request.Path, isDemo);
    if (eventType is null)
    {
        return;
    }

    var audit = context.RequestServices.GetRequiredService<SecurityAuditService>();
    await audit.LogAsync(
        context,
        eventType,
        SecurityAuditOutcomes.Success,
        context.Response.StatusCode,
        new { is_demo = isDemo },
        CancellationToken.None);
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapControllers().RequireRateLimiting("api");
app.MapPrometheusScrapingEndpoint("/metrics");

app.Run();

static string? ResolveOperationalAuditEvent(string method, PathString path, bool isDemo)
{
    var pathValue = path.Value ?? string.Empty;
    var modifiesState = HttpMethods.IsPost(method) || HttpMethods.IsPut(method) || HttpMethods.IsDelete(method);
    if (!modifiesState)
    {
        return null;
    }

    if (HttpMethods.IsPost(method) &&
        pathValue.EndsWith("/setup", StringComparison.OrdinalIgnoreCase))
    {
        return SecurityAuditEventTypes.SetupSaved;
    }

    if (HttpMethods.IsPost(method) &&
        pathValue.EndsWith("/start", StringComparison.OrdinalIgnoreCase))
    {
        return SecurityAuditEventTypes.SessionStarted;
    }

    if (HttpMethods.IsPost(method) &&
        pathValue.EndsWith("/end", StringComparison.OrdinalIgnoreCase))
    {
        return SecurityAuditEventTypes.SessionEnded;
    }

    if (path.StartsWithSegments("/api/v1/rulesets", StringComparison.OrdinalIgnoreCase))
    {
        return SecurityAuditEventTypes.RulesetChanged;
    }

    return isDemo ? SecurityAuditEventTypes.DemoActivity : null;
}

static async Task SeedBootstrapUserAsync(
    // Parameter `conn` bertipe `Npgsql.NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
    Npgsql.NpgsqlConnection conn,
    // Parameter `username` bertipe `string?` membawa nama akun yang dipakai saat autentikasi; nilai null diizinkan ketika data opsional belum tersedia.
    string? username,
    // Parameter `password` bertipe `string?` membawa kata sandi masukan yang diperiksa sesuai kebijakan autentikasi; nilai null diizinkan ketika data
    // opsional belum tersedia.
    string? password,
    // Parameter `role` bertipe `string` membawa peran pengguna yang menentukan hak akses.
    string role,
    // Parameter `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
    // permintaan atau aplikasi berhenti.
    CancellationToken cancellationToken)
{
    if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
    {
        return;
    }

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
        // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”AuthBootstrap untuk role {role} harus mengisi
        // username dan password.”) dalam SeedBootstrapUserAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
        throw new InvalidOperationException($"AuthBootstrap untuk role {role} harus mengisi username dan password.");
    }

    if (password.Length < Cashflowpoly.Api.Security.PasswordPolicy.MinPasswordLength)
    {
        // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( $”AuthBootstrap password role {role} minimal
        // {Cashflowpoly.Api.Security.PasswordPolicy.MinPasswordLength} karakter.”) dalam SeedBootstrapUserAsync; pemanggil atau middleware penanganan error
        // menerima kegagalan ini.
        throw new InvalidOperationException(
            $"AuthBootstrap password role {role} minimal {Cashflowpoly.Api.Security.PasswordPolicy.MinPasswordLength} karakter.");
    }

    if (!Cashflowpoly.Api.Security.PasswordPolicy.IsWithinBcryptLimit(password))
    {
        // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ( $”AuthBootstrap password role {role} maksimal
        // {Cashflowpoly.Api.Security.PasswordPolicy.MaxPasswordUtf8Bytes} byte UTF-8.”) dalam SeedBootstrapUserAsync; pemanggil atau middleware penanganan
        // error menerima kegagalan ini.
        throw new InvalidOperationException(
            $"AuthBootstrap password role {role} maksimal {Cashflowpoly.Api.Security.PasswordPolicy.MaxPasswordUtf8Bytes} byte UTF-8.");
    }

    const string insertSql = """
        insert into app_users (user_id, username, display_name, password_hash, role, is_active)
        select gen_random_uuid(), @username, @displayName, crypt(@password, gen_salt('bf', 10)), @role, true
        where not exists (select 1 from app_users where lower(username) = lower(@username));
        """;

    await conn.ExecuteAsync(
        new Dapper.CommandDefinition(
            insertSql,
            new { username, displayName = username, password, role },
            cancellationToken: cancellationToken));
}
