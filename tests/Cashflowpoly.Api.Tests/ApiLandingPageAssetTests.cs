// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui ApiLandingPageAssetTests.
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `ApiLandingPageAssetTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class ApiLandingPageAssetTests
// Membuka scope tipe ApiLandingPageAssetTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `RepoRoot` menyimpan nilai repo root dengan nilai awal memanggil `ResolveRepositoryRoot` dengan tanpa
    // argumen. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan
    // dibagikan antar instance.
    private static readonly string RepoRoot = ResolveRepositoryRoot();

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ApiLandingPage_DoesNotAdvertiseSwaggerLinks` dengan hasil bertipe `void`; operasi ini menangani api landing page does not
    // advertise swagger links.
    public void ApiLandingPage_DoesNotAdvertiseSwaggerLinks()
    // Membuka scope metode ApiLandingPage_DoesNotAdvertiseSwaggerLinks; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ApiLandingPage_DoesNotAdvertiseSwaggerLinks.
    {
        // Menyiapkan variabel lokal `indexPath` untuk nilai index path dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`, `”Cashflowpoly.Api”`,
        // `”wwwroot”`, `”index.html”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var indexPath = Path.Combine(RepoRoot, "src", "Cashflowpoly.Api", "wwwroot", "index.html");

        // Menjalankan pemeriksaan bahwa `File.Exists(indexPath)`, `”Landing page API harus tersedia di wwwroot/index.html.”` bernilai benar; pengujian
        // gagal jika kondisi tidak terpenuhi dalam ApiLandingPage_DoesNotAdvertiseSwaggerLinks.
        Assert.True(File.Exists(indexPath), "Landing page API harus tersedia di wwwroot/index.html.");

        // Menyiapkan variabel lokal `content` untuk nilai content dengan memanggil `File.ReadAllText` dengan `indexPath`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var content = File.ReadAllText(indexPath);

        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”href=\”/swagger\””`, `content`,
        // `StringComparison.Ordinal` dalam ApiLandingPage_DoesNotAdvertiseSwaggerLinks.
        Assert.DoesNotContain("href=\"/swagger\"", content, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”href=\”/swagger/v1/swagger.json\””`, `content`, `StringComparison.Ordinal` dalam ApiLandingPage_DoesNotAdvertiseSwaggerLinks.
        Assert.DoesNotContain("href=\"/swagger/v1/swagger.json\"", content, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”cta_swagger”`, `content`,
        // `StringComparison.Ordinal` dalam ApiLandingPage_DoesNotAdvertiseSwaggerLinks.
        Assert.DoesNotContain("cta_swagger", content, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”cta_openapi”`, `content`,
        // `StringComparison.Ordinal` dalam ApiLandingPage_DoesNotAdvertiseSwaggerLinks.
        Assert.DoesNotContain("cta_openapi", content, StringComparison.Ordinal);
    // Menutup scope metode ApiLandingPage_DoesNotAdvertiseSwaggerLinks; bagian berikut berada di luar batas blok tersebut dalam
    // ApiLandingPage_DoesNotAdvertiseSwaggerLinks.
    }

    // Mendefinisikan metode `ResolveRepositoryRoot` dengan hasil bertipe `string`; operasi ini menangani resolve repositori root.
    private static string ResolveRepositoryRoot()
    // Membuka scope metode ResolveRepositoryRoot; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveRepositoryRoot.
    {
        // Menyiapkan variabel lokal `current` untuk nilai saat ini dengan objek baru bertipe `DirectoryInfo` dengan argumen (AppContext.BaseDirectory).
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        // Mengulangi blok selama hasil pencocokan `current` dengan pola `not null`; kondisi diperiksa lagi sebelum setiap iterasi dalam
        // ResolveRepositoryRoot.
        while (current is not null)
        // Membuka scope loop selama `current is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveRepositoryRoot.
        {
            // Memeriksa memanggil `File.Exists` dengan `Path.Combine(current.FullName, ”Cashflowpoly.sln”)`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam ResolveRepositoryRoot.
            if (File.Exists(Path.Combine(current.FullName, "Cashflowpoly.sln")))
            // Membuka scope cabang if untuk kondisi `File.Exists(Path.Combine(current.FullName, ”Cashflowpoly.sln”))`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam ResolveRepositoryRoot.
            {
                // Mengembalikan `current.FullName` (nilai full nama) kepada pemanggil dalam ResolveRepositoryRoot; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return current.FullName;
            // Menutup scope cabang if untuk kondisi `File.Exists(Path.Combine(current.FullName, ”Cashflowpoly.sln”))`; bagian berikut berada di luar batas blok
            // tersebut dalam ResolveRepositoryRoot.
            }

            // Memperbarui `current` menggunakan `current.Parent` (nilai parent) dalam ResolveRepositoryRoot.
            current = current.Parent;
        // Menutup scope loop selama `current is not null`; bagian berikut berada di luar batas blok tersebut dalam ResolveRepositoryRoot.
        }

        // Menghentikan alur dengan melempar objek baru bertipe `DirectoryNotFoundException` dengan argumen (”Tidak dapat menemukan root repositori
        // (Cashflowpoly.sln).”) dalam ResolveRepositoryRoot; pemanggil atau middleware penanganan error menerima kegagalan ini.
        throw new DirectoryNotFoundException("Tidak dapat menemukan root repositori (Cashflowpoly.sln).");
    // Menutup scope metode ResolveRepositoryRoot; bagian berikut berada di luar batas blok tersebut dalam ResolveRepositoryRoot.
    }
// Menutup scope tipe ApiLandingPageAssetTests; bagian berikut berada di luar batas blok tersebut.
}
