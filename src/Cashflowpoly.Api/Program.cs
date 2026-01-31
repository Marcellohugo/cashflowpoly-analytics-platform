var builder = WebApplication.CreateBuilder(args);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:Default belum dikonfigurasi.");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(Npgsql.NpgsqlDataSource.Create(connectionString));
builder.Services.AddScoped<Cashflowpoly.Api.Data.RulesetRepository>();
builder.Services.AddScoped<Cashflowpoly.Api.Data.SessionRepository>();
builder.Services.AddScoped<Cashflowpoly.Api.Data.EventRepository>();
builder.Services.AddScoped<Cashflowpoly.Api.Data.MetricsRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
