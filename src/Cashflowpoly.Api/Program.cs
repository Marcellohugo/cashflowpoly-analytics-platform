// Fungsi file: Mengonfigurasi dependency, middleware, endpoint, dan startup API.
using System.Security.Claims;
using System.Diagnostics;
using System.Net;
using System.Threading.RateLimiting;
using Dapper;
using Cashflowpoly.Api.Infrastructure;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Cashflowpoly.Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using OpenTelemetry.Metrics;
using Cashflowpoly.Api.Infrastructure.Telemetry;

var builder = WebApplication.CreateBuilder(args);
var bypassOperationalRateLimit = builder.Environment.IsEnvironment("Testing");

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:Default belum dikonfigurasi.");
}
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtSection);
builder.Services.Configure<AuthRegistrationOptions>(options =>
{
    options.AllowPublicInstructorRegistration =
        builder.Configuration.GetValue<bool>("Auth:AllowPublicInstructorRegistration");
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
    foreach (var proxy in trustedProxies ?? [])
    {
        if (IPAddress.TryParse(proxy, out var ip))
        {
            options.KnownProxies.Add(ip);
        }
    }

    var trustedNetworks = builder.Configuration.GetSection("Networking:TrustedNetworks").Get<string[]>();
    foreach (var network in trustedNetworks ?? [])
    {
        if (System.Net.IPNetwork.TryParse(network, out var parsedNetwork))
        {
            options.KnownIPNetworks.Add(parsedNetwork);
        }
    }
});
builder.Services.AddControllers();
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
                if (context.HttpContext.Items.ContainsKey("security_auth_audit_written"))
                {
                    return;
                }

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
            },
            OnForbidden = async context =>
            {
                if (context.HttpContext.Items.ContainsKey("security_auth_audit_written"))
                {
                    return;
                }

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
builder.Services.AddScoped<IEventValidationDetailsSerializer, EventValidationDetailsSerializer>();
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
builder.Services.AddScoped<IDerivedRatioCalculator, DerivedRatioCalculator>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Cashflowpoly.Api.Services.IAnalyticsService, Cashflowpoly.Api.Services.AnalyticsService>();
builder.Services.AddScoped<Cashflowpoly.Api.Services.IEventIngestionService, Cashflowpoly.Api.Services.EventIngestionService>();

var app = builder.Build();

await DatabaseInitialization.InitializeAsync(app.Services, CancellationToken.None);

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

if (app.Environment.IsDevelopment())
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
    catch
    {
        failed = true;
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

static async Task SeedBootstrapUserAsync(
    Npgsql.NpgsqlConnection conn,
    string? username,
    string? password,
    string role,
    CancellationToken cancellationToken)
{
    if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
    {
        return;
    }

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
        throw new InvalidOperationException($"AuthBootstrap untuk role {role} harus mengisi username dan password.");
    }

    if (password.Length < Cashflowpoly.Api.Security.PasswordPolicy.MinPasswordLength)
    {
        throw new InvalidOperationException(
            $"AuthBootstrap password role {role} minimal {Cashflowpoly.Api.Security.PasswordPolicy.MinPasswordLength} karakter.");
    }

    if (!Cashflowpoly.Api.Security.PasswordPolicy.IsWithinBcryptLimit(password))
    {
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
