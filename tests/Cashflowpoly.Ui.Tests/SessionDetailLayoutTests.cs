// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui SessionDetailLayoutTests.
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `SessionDetailLayoutTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionDetailLayoutTests
// Membuka scope tipe SessionDetailLayoutTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `LayoutAndSessionDetailView_ShouldKeepPageStyleOutOfMainFlow` dengan hasil bertipe `void`; operasi ini menangani layout dan
    // sesi detail view should keep page style out of main flow.
    public void LayoutAndSessionDetailView_ShouldKeepPageStyleOutOfMainFlow()
    // Membuka scope metode LayoutAndSessionDetailView_ShouldKeepPageStyleOutOfMainFlow; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam LayoutAndSessionDetailView_ShouldKeepPageStyleOutOfMainFlow.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `layoutPath` untuk nilai layout path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Shared”`, `”_Layout.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var layoutPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Shared", "_Layout.cshtml");
        // Menyiapkan variabel lokal `sessionDetailPath` untuk nilai sesi detail path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`,
        // `”Cashflowpoly.Ui”`, `”Views”`, `”Sessions”`, `”Details.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionDetailPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Sessions", "Details.cshtml");

        // Menyiapkan variabel lokal `layoutContent` untuk nilai layout content dengan memanggil `File.ReadAllText` dengan `layoutPath`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var layoutContent = File.ReadAllText(layoutPath);
        // Menyiapkan variabel lokal `sessionDetailContent` untuk nilai sesi detail content dengan memanggil `File.ReadAllText` dengan `sessionDetailPath`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionDetailContent = File.ReadAllText(sessionDetailPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”””@await RenderSectionAsync(”Head”,
        // required: false)”””`, `layoutContent` dalam LayoutAndSessionDetailView_ShouldKeepPageStyleOutOfMainFlow.
        Assert.Contains("""@await RenderSectionAsync("Head", required: false)""", layoutContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”@section Head”`, `sessionDetailContent`
        // dalam LayoutAndSessionDetailView_ShouldKeepPageStyleOutOfMainFlow.
        Assert.Contains("@section Head", sessionDetailContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `Environment.NewLine + ”<style>”`,
        // `sessionDetailContent`, `StringComparison.Ordinal` dalam LayoutAndSessionDetailView_ShouldKeepPageStyleOutOfMainFlow.
        Assert.DoesNotContain(Environment.NewLine + "<style>", sessionDetailContent, StringComparison.Ordinal);
    // Menutup scope metode LayoutAndSessionDetailView_ShouldKeepPageStyleOutOfMainFlow; bagian berikut berada di luar batas blok tersebut dalam
    // LayoutAndSessionDetailView_ShouldKeepPageStyleOutOfMainFlow.
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
// Menutup scope tipe SessionDetailLayoutTests; bagian berikut berada di luar batas blok tersebut.
}
