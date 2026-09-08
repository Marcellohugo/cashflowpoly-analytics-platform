// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui RulesetDetailsControllerTests.
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `RulesetDetailsControllerTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetDetailsControllerTests
// Membuka scope tipe RulesetDetailsControllerTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetDetailsController_ShouldMakeReadonlyFromApiMetadata` dengan hasil bertipe `void`; operasi ini menangani aturan
    // rincian controller should make readonly dari api metadata.
    public void RulesetDetailsController_ShouldMakeReadonlyFromApiMetadata()
    // Membuka scope metode RulesetDetailsController_ShouldMakeReadonlyFromApiMetadata; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam RulesetDetailsController_ShouldMakeReadonlyFromApiMetadata.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `controllerPath` untuk nilai controller path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`,
        // `”Cashflowpoly.Ui”`, `”Controllers”`, `”RulesetsController.cs”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var controllerPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Controllers", "RulesetsController.cs");
        // Menyiapkan variabel lokal `controllerContent` untuk nilai controller content dengan memanggil `File.ReadAllText` dengan `controllerPath`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var controllerContent = File.ReadAllText(controllerPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”data.IsDefault”`, `controllerContent`,
        // `StringComparison.Ordinal` dalam RulesetDetailsController_ShouldMakeReadonlyFromApiMetadata.
        Assert.Contains("data.IsDefault", controllerContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”data.IsLockedBySession”`,
        // `controllerContent`, `StringComparison.Ordinal` dalam RulesetDetailsController_ShouldMakeReadonlyFromApiMetadata.
        Assert.Contains("data.IsLockedBySession", controllerContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”fromDefaultCatalog || data.IsDefault ||
        // data.IsLockedBySession”`, `controllerContent`, `StringComparison.Ordinal` dalam RulesetDetailsController_ShouldMakeReadonlyFromApiMetadata.
        Assert.Contains("fromDefaultCatalog || data.IsDefault || data.IsLockedBySession", controllerContent, StringComparison.Ordinal);
    // Menutup scope metode RulesetDetailsController_ShouldMakeReadonlyFromApiMetadata; bagian berikut berada di luar batas blok tersebut dalam
    // RulesetDetailsController_ShouldMakeReadonlyFromApiMetadata.
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

        // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Repository root tidak ditemukan.”) dalam
        // ResolveRepositoryRoot; pemanggil atau middleware penanganan error menerima kegagalan ini.
        throw new InvalidOperationException("Repository root tidak ditemukan.");
    // Menutup scope metode ResolveRepositoryRoot; bagian berikut berada di luar batas blok tersebut dalam ResolveRepositoryRoot.
    }
// Menutup scope tipe RulesetDetailsControllerTests; bagian berikut berada di luar batas blok tersebut.
}
