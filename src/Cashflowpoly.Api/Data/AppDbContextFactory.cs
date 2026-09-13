// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk AppDbContextFactory.
// Mengimpor namespace `Microsoft.EntityFrameworkCore` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.EntityFrameworkCore;
// Mengimpor namespace `Microsoft.EntityFrameworkCore.Design` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.EntityFrameworkCore.Design;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Data` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Data;

/// <summary>
/// Digunakan oleh dotnet-ef CLI untuk membuat AppDbContext saat generate migrasi tanpa menjalankan aplikasi penuh.
/// </summary>
// Mendefinisikan tipe class `AppDbContextFactory` yang mewarisi atau menerapkan `IDesignTimeDbContextFactory<AppDbContext>`; sealed mencegah tipe
// ini diturunkan lagi.
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
