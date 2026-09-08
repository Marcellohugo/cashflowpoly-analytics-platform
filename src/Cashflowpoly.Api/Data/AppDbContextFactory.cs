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
// Membuka scope tipe AppDbContextFactory; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `CreateDbContext` dengan hasil bertipe `AppDbContext`; operasi ini menangani create basis data context. Masukan: Parameter
    // `args` bertipe `string[]` membawa nilai args.
    public AppDbContext CreateDbContext(string[] args)
    // Membuka scope metode CreateDbContext; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateDbContext.
    {
        // Menyiapkan variabel lokal `options` untuk kumpulan pengaturan yang mengendalikan perilaku komponen dengan `new
        // DbContextOptionsBuilder<AppDbContext>() .UseNpgsql(”Host=localhost;Database=cashflowpoly;Username=postgres;Password=postgres”) .Options`
        // (kumpulan pengaturan yang mengendalikan perilaku komponen). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui
            // .UseNpgsql(”Host=localhost;Database=cashflowpoly;Username=postgres;Password=postgres”) dalam CreateDbContext; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .UseNpgsql("Host=localhost;Database=cashflowpoly;Username=postgres;Password=postgres")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Options; dalam CreateDbContext; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .Options;
        // Mengembalikan objek baru bertipe `AppDbContext` dengan argumen (options) kepada pemanggil dalam CreateDbContext; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new AppDbContext(options);
    // Menutup scope metode CreateDbContext; bagian berikut berada di luar batas blok tersebut dalam CreateDbContext.
    }
// Menutup scope tipe AppDbContextFactory; bagian berikut berada di luar batas blok tersebut.
}
