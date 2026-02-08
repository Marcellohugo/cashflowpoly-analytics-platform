using System.Security.Claims;
using System.Diagnostics;
using System.Text;
using System.Threading.RateLimiting;
using Cashflowpoly.Api.Controllers;
using Cashflowpoly.Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:Default belum dikonfigurasi.");
}

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey))
{
    throw new InvalidOperationException("Jwt:SigningKey belum dikonfigurasi.");
}

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
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
builder.Services.AddScoped<Cashflowpoly.Api.Data.RulesetRepository>();
builder.Services.AddScoped<Cashflowpoly.Api.Data.SessionRepository>();
builder.Services.AddScoped<Cashflowpoly.Api.Data.EventRepository>();
builder.Services.AddScoped<Cashflowpoly.Api.Data.MetricsRepository>();
builder.Services.AddScoped<Cashflowpoly.Api.Data.PlayerRepository>();
builder.Services.AddScoped<Cashflowpoly.Api.Data.UserRepository>();
builder.Services.AddHostedService<Cashflowpoly.Api.Data.AuthSchemaBootstrapper>();

var app = builder.Build();
var requestLogger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("RequestAudit");
var exceptionLogger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("UnhandledException");

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception is not null)
        {
            exceptionLogger.LogError(
                exception,
                "Unhandled exception. trace_id={TraceId} path={Path}",
                context.TraceIdentifier,
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

app.UseRouting();

app.Use(async (context, next) =>
{
    var start = Stopwatch.GetTimestamp();
    await next();
    var durationMs = (Stopwatch.GetTimestamp() - start) * 1000.0 / Stopwatch.Frequency;
    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
    var role = context.User.FindFirstValue(ClaimTypes.Role) ?? "anonymous";

    requestLogger.LogInformation(
        "request_completed trace_id={TraceId} user_id={UserId} role={Role} method={Method} path={Path} status_code={StatusCode} duration_ms={DurationMs}",
        context.TraceIdentifier,
        userId,
        role,
        context.Request.Method,
        context.Request.Path.Value,
        context.Response.StatusCode,
        Math.Round(durationMs, 2));
});
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers().RequireRateLimiting("api");

app.Run();
