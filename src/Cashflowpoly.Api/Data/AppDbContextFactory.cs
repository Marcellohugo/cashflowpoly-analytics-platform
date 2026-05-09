// Fungsi file: Design-time factory untuk dotnet-ef migrations — menghindari kebutuhan koneksi database saat generate migration.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cashflowpoly.Api.Data;

/// <summary>
/// Digunakan oleh dotnet-ef CLI untuk membuat AppDbContext saat generate migrasi tanpa menjalankan aplikasi penuh.
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=cashflowpoly;Username=postgres;Password=postgres")
            .Options;
        return new AppDbContext(options);
    }
}
