// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui RulesetManagementActionTests.
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;
// Mengimpor namespace `System.Text.RegularExpressions` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.RegularExpressions;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `RulesetManagementActionTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetManagementActionTests
// Membuka scope tipe RulesetManagementActionTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `string`: `RepoRoot` menyimpan nilai repo root dengan nilai awal memanggil `ResolveRepositoryRoot` dengan tanpa
    // argumen. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan
    // dibagikan antar instance.
    private static readonly string RepoRoot = ResolveRepositoryRoot();
    // Mendeklarasikan field bertipe `string`: `UiRoot` menyimpan nilai ui root dengan nilai awal memanggil `Path.Combine` dengan `RepoRoot`, `”src”`,
    // `”Cashflowpoly.Ui”`. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik
    // tipe dan dibagikan antar instance.
    private static readonly string UiRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui");

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetViews_ShouldRenderInstructorMutationActions` dengan hasil bertipe `void`; operasi ini menangani aturan views should
    // render instruktur mutation aksi.
    public void RulesetViews_ShouldRenderInstructorMutationActions()
    // Membuka scope metode RulesetViews_ShouldRenderInstructorMutationActions; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // RulesetViews_ShouldRenderInstructorMutationActions.
    {
        // Menyiapkan variabel lokal `indexView` untuk nilai index view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”,
        // ”Rulesets”, ”Index.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var indexView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Rulesets", "Index.cshtml"));
        // Menyiapkan variabel lokal `detailView` untuk nilai detail view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”,
        // ”Shared”, ”_RulesetDetailContent.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var detailView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Shared", "_RulesetDetailContent.cshtml"));
        // Menyiapkan variabel lokal `sessionDetailView` untuk nilai sesi detail view dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot,
        // ”Views”, ”Sessions”, ”Details.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionDetailView = File.ReadAllText(Path.Combine(UiRoot, "Views", "Sessions", "Details.cshtml"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”asp-action=\”Create\””`, `indexView`,
        // `StringComparison.Ordinal` dalam RulesetViews_ShouldRenderInstructorMutationActions.
        Assert.Contains("asp-action=\"Create\"", indexView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”asp-action=\”BulkDelete\””`,
        // `indexView`, `StringComparison.Ordinal` dalam RulesetViews_ShouldRenderInstructorMutationActions.
        Assert.Contains("asp-action=\"BulkDelete\"", indexView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”asp-action=\”Edit\””`, `indexView`,
        // `StringComparison.Ordinal` dalam RulesetViews_ShouldRenderInstructorMutationActions.
        Assert.Contains("asp-action=\"Edit\"", indexView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”asp-action=\”Delete\””`, `indexView`,
        // `StringComparison.Ordinal` dalam RulesetViews_ShouldRenderInstructorMutationActions.
        Assert.Contains("asp-action=\"Delete\"", indexView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”asp-action=\”ActivateVersion\””`,
        // `detailView`, `StringComparison.Ordinal` dalam RulesetViews_ShouldRenderInstructorMutationActions.
        Assert.Contains("asp-action=\"ActivateVersion\"", detailView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”asp-action=\”DeleteVersion\””`,
        // `detailView`, `StringComparison.Ordinal` dalam RulesetViews_ShouldRenderInstructorMutationActions.
        Assert.Contains("asp-action=\"DeleteVersion\"", detailView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”asp-action=\”Ruleset\””`,
        // `sessionDetailView`, `StringComparison.Ordinal` dalam RulesetViews_ShouldRenderInstructorMutationActions.
        Assert.DoesNotContain("asp-action=\"Ruleset\"", sessionDetailView, StringComparison.Ordinal);
    // Menutup scope metode RulesetViews_ShouldRenderInstructorMutationActions; bagian berikut berada di luar batas blok tersebut dalam
    // RulesetViews_ShouldRenderInstructorMutationActions.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetControllers_ShouldCallMutationApiEndpoints` dengan hasil bertipe `void`; operasi ini menangani aturan controllers
    // should call mutation api endpoints.
    public void RulesetControllers_ShouldCallMutationApiEndpoints()
    // Membuka scope metode RulesetControllers_ShouldCallMutationApiEndpoints; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // RulesetControllers_ShouldCallMutationApiEndpoints.
    {
        // Menyiapkan variabel lokal `rulesetsController` untuk nilai aturan controller dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot,
        // ”Controllers”, ”RulesetsController.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetsController = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "RulesetsController.cs"));
        // Menyiapkan variabel lokal `sessionsController` untuk nilai sessions controller dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot,
        // ”Controllers”, ”SessionsController.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionsController = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "SessionsController.cs"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”PostAsJsonAsync(\”api/v1/rulesets\””`,
        // `rulesetsController`, `StringComparison.Ordinal` dalam RulesetControllers_ShouldCallMutationApiEndpoints.
        Assert.Contains("PostAsJsonAsync(\"api/v1/rulesets\"", rulesetsController, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”PutAsJsonAsync($\”api/v1/rulesets/{rulesetId}\””`, `rulesetsController`, `StringComparison.Ordinal` dalam
        // RulesetControllers_ShouldCallMutationApiEndpoints.
        Assert.Contains("PutAsJsonAsync($\"api/v1/rulesets/{rulesetId}\"", rulesetsController, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”PostAsync($\”api/v1/rulesets/{rulesetId}/versions/{version}/activate\””`, `rulesetsController`, `StringComparison.Ordinal` dalam
        // RulesetControllers_ShouldCallMutationApiEndpoints.
        Assert.Contains("PostAsync($\"api/v1/rulesets/{rulesetId}/versions/{version}/activate\"", rulesetsController, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”DeleteAsync($\”api/v1/rulesets/{rulesetId}/versions/{version}\””`, `rulesetsController`, `StringComparison.Ordinal` dalam
        // RulesetControllers_ShouldCallMutationApiEndpoints.
        Assert.Contains("DeleteAsync($\"api/v1/rulesets/{rulesetId}/versions/{version}\"", rulesetsController, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”DeleteAsync($\”api/v1/rulesets/{rulesetId}\””`, `rulesetsController`, `StringComparison.Ordinal` dalam
        // RulesetControllers_ShouldCallMutationApiEndpoints.
        Assert.Contains("DeleteAsync($\"api/v1/rulesets/{rulesetId}\"", rulesetsController, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset/activate”`,
        // `sessionsController`, `StringComparison.Ordinal` dalam RulesetControllers_ShouldCallMutationApiEndpoints.
        Assert.DoesNotContain("ruleset/activate", sessionsController, StringComparison.Ordinal);
    // Menutup scope metode RulesetControllers_ShouldCallMutationApiEndpoints; bagian berikut berada di luar batas blok tersebut dalam
    // RulesetControllers_ShouldCallMutationApiEndpoints.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `PlayerRulesetAccess_ShouldBeRestrictedToSessionDetails` dengan hasil bertipe `void`; operasi ini menangani pemain aturan
    // akses should be restricted ke sesi rincian.
    public void PlayerRulesetAccess_ShouldBeRestrictedToSessionDetails()
    // Membuka scope metode PlayerRulesetAccess_ShouldBeRestrictedToSessionDetails; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // PlayerRulesetAccess_ShouldBeRestrictedToSessionDetails.
    {
        // Menyiapkan variabel lokal `rulesetsController` untuk nilai aturan controller dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot,
        // ”Controllers”, ”RulesetsController.cs”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetsController = File.ReadAllText(Path.Combine(UiRoot, "Controllers", "RulesetsController.cs"));
        // Menyiapkan variabel lokal `layout` untuk nilai layout dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Shared”,
        // ”_Layout.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var layout = File.ReadAllText(Path.Combine(UiRoot, "Views", "Shared", "_Layout.cshtml"));
        // Menyiapkan variabel lokal `home` untuk nilai home dengan memanggil `File.ReadAllText` dengan `Path.Combine(UiRoot, ”Views”, ”Home”,
        // ”Index.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var home = File.ReadAllText(Path.Combine(UiRoot, "Views", "Home", "Index.cshtml"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”public override void
        // OnActionExecuting(ActionExecutingContext context)”`, `rulesetsController`, `StringComparison.Ordinal` dalam
        // PlayerRulesetAccess_ShouldBeRestrictedToSessionDetails.
        Assert.Contains("public override void OnActionExecuting(ActionExecutingContext context)", rulesetsController, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”context.Result =
        // RedirectToAction(\”Index\”, \”Sessions\”)”`, `rulesetsController`, `StringComparison.Ordinal` dalam
        // PlayerRulesetAccess_ShouldBeRestrictedToSessionDetails.
        Assert.Contains("context.Result = RedirectToAction(\"Index\", \"Sessions\")", rulesetsController, StringComparison.Ordinal);
        // Menjalankan pemeriksaan hasil dengan `Assert.Single` menggunakan `Regex.Matches(layout, ”Controller = \\\”Rulesets\\\””)`; ketidaksesuaian dengan
        // ekspektasi membuat pengujian gagal dalam PlayerRulesetAccess_ShouldBeRestrictedToSessionDetails.
        Assert.Single(Regex.Matches(layout, "Controller = \\\"Rulesets\\\""));
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”secondaryCtaController = isInstructor ?
        // \”Rulesets\” : \”Sessions\””`, `home`, `StringComparison.Ordinal` dalam PlayerRulesetAccess_ShouldBeRestrictedToSessionDetails.
        Assert.Contains("secondaryCtaController = isInstructor ? \"Rulesets\" : \"Sessions\"", home, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”<a asp-controller=\”Sessions\”
        // asp-action=\”Index\”>@Context.T(\”home.player_guide.ruleset.link\”)</a>”`, `home`, `StringComparison.Ordinal` dalam
        // PlayerRulesetAccess_ShouldBeRestrictedToSessionDetails.
        Assert.Contains("<a asp-controller=\"Sessions\" asp-action=\"Index\">@Context.T(\"home.player_guide.ruleset.link\")</a>", home, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”home-stat-grid-player”`, `home`,
        // `StringComparison.Ordinal` dalam PlayerRulesetAccess_ShouldBeRestrictedToSessionDetails.
        Assert.Contains("home-stat-grid-player", home, StringComparison.Ordinal);
    // Menutup scope metode PlayerRulesetAccess_ShouldBeRestrictedToSessionDetails; bagian berikut berada di luar batas blok tersebut dalam
    // PlayerRulesetAccess_ShouldBeRestrictedToSessionDetails.
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
// Menutup scope tipe RulesetManagementActionTests; bagian berikut berada di luar batas blok tersebut.
}
