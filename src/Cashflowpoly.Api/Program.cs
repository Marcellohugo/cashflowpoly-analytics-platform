// Fungsi file: Mengonfigurasi bootstrap service, middleware, endpoint, dan keamanan untuk aplikasi API.
using System.Security.Claims;
using System.Diagnostics;
using System.Net;
using System.Threading.RateLimiting;
using Cashflowpoly.Api.Controllers;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Infrastructure;
using Cashflowpoly.Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:Default belum dikonfigurasi.");
}
var enableLegacyApiCompatibility = builder.Configuration.GetValue<bool>("FeatureFlags:EnableLegacyApiCompatibility");

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtOptions>(jwtSection);
builder.Services.Configure<AuthBootstrapOptions>(builder.Configuration.GetSection("AuthBootstrap"));
builder.Services.AddSingleton<JwtSigningKeyProvider>();
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddSingleton<OperationalMetricsTracker>();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = 1;

    var trustedProxies = builder.Configuration.GetSection("Networking:TrustedProxies").Get<string[]>();
    if (trustedProxies is null || trustedProxies.Length == 0)
    {
        return;
    }

    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
    foreach (var proxy in trustedProxies)
    {
        if (IPAddress.TryParse(proxy, out var ip))
        {
            options.KnownProxies.Add(ip);
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
        var permitLimit = RateLimitPolicyHelper.ResolvePermitLimit(httpContext.Request.Path);
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
builder.Services.AddSingleton(Npgsql.NpgsqlDataSource.Create(connectionString));
builder.Services.AddScoped<RulesetRepository>();
builder.Services.AddScoped<SessionRepository>();
builder.Services.AddScoped<EventRepository>();
builder.Services.AddScoped<MetricsRepository>();
builder.Services.AddScoped<PlayerRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<SecurityAuditRepository>();
builder.Services.AddScoped<SecurityAuditService>();
builder.Services.AddHostedService<AuthSchemaBootstrapper>();

var app = builder.Build();
var requestLogger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("RequestAudit");
var exceptionLogger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("UnhandledException");
var metricsTracker = app.Services.GetRequiredService<OperationalMetricsTracker>();
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

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cashflowpoly API v1");
});

app.UseForwardedHeaders();
app.UseDefaultFiles();
app.UseStaticFiles();
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
app.UseRouting();

app.Use(async (context, next) =>
{
    var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
    context.TraceIdentifier = traceId;
    context.Response.Headers["X-Trace-Id"] = traceId;

    var start = Stopwatch.GetTimestamp();
    await next();
    var durationMs = (Stopwatch.GetTimestamp() - start) * 1000.0 / Stopwatch.Frequency;

    metricsTracker.Record(context, durationMs);

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
        context.Response.StatusCode,
        Math.Round(durationMs, 2),
        string.IsNullOrWhiteSpace(clientRequestId) ? "-" : clientRequestId);
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

app.Run();

/// <summary>
/// Menyatakan peran utama tipe Program pada modul ini.
/// </summary>
public partial class Program { }
