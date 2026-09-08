// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui MenuComponentConsistencyTests.
// Mengimpor namespace `System.Text.RegularExpressions` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.RegularExpressions;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `MenuComponentConsistencyTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class MenuComponentConsistencyTests
// Membuka scope tipe MenuComponentConsistencyTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”Sessions”, ”Details.cshtml”, ”sessions.back_to_list”).
    [InlineData("Sessions", "Details.cshtml", "sessions.back_to_list")]
    // menyediakan satu kombinasi masukan pengujian (”Players”, ”Details.cshtml”, ”players.detail.nav_back_players”).
    [InlineData("Players", "Details.cshtml", "players.detail.nav_back_players")]
    // menyediakan satu kombinasi masukan pengujian (”Players”, ”Details.cshtml”, ”players.detail.nav_back_session”).
    [InlineData("Players", "Details.cshtml", "players.detail.nav_back_session")]
    // menyediakan satu kombinasi masukan pengujian (”Rulesets”, ”Details.cshtml”, ”rulesets.back_to_list”).
    [InlineData("Rulesets", "Details.cshtml", "rulesets.back_to_list")]
    // menyediakan satu kombinasi masukan pengujian (”Rulesets”, ”Create.cshtml”, ”rulesets.back_to_list”).
    [InlineData("Rulesets", "Create.cshtml", "rulesets.back_to_list")]
    // Mendefinisikan metode `PageBackLinks_ShouldPrecedeTitleInResponsiveToolbar` dengan hasil bertipe `void`; operasi ini menangani page back links
    // should precede title in responsive toolbar. Masukan: Parameter `folder` bertipe `string` membawa nilai folder; Parameter `fileName` bertipe
    // `string` membawa nilai file nama; Parameter `labelKey` bertipe `string` membawa nilai label kunci.
    public void PageBackLinks_ShouldPrecedeTitleInResponsiveToolbar(string folder, string fileName, string labelKey)
    // Membuka scope metode PageBackLinks_ShouldPrecedeTitleInResponsiveToolbar; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // PageBackLinks_ShouldPrecedeTitleInResponsiveToolbar.
    {
        // Menyiapkan variabel lokal `view` untuk nilai view dengan memanggil `File.ReadAllText` dengan `Path.Combine(ResolveRepositoryRoot(), ”src”,
        // ”Cashflowpoly.Ui”, ”Views”, folder, fileName)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var view = File.ReadAllText(Path.Combine(ResolveRepositoryRoot(), "src", "Cashflowpoly.Ui", "Views", folder, fileName));
        // Menyiapkan variabel lokal `titlePosition` untuk nilai title position dengan memanggil `view.IndexOf` dengan `”<h1”`, `StringComparison.Ordinal`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var titlePosition = view.IndexOf("<h1", StringComparison.Ordinal);
        // Menyiapkan variabel lokal `toolbarPosition` untuk nilai toolbar position dengan memanggil `view.IndexOf` dengan `”class=\”action-toolbar mb-4”`,
        // `StringComparison.Ordinal`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var toolbarPosition = view.IndexOf("class=\"action-toolbar mb-4", StringComparison.Ordinal);
        // Menyiapkan variabel lokal `backLinks` untuk nilai back links dengan memanggil `Regex.Matches` dengan `view`,
        // `Regex.Escape($”Context.T(\”{labelKey}\”)”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var backLinks = Regex.Matches(view, Regex.Escape($"Context.T(\"{labelKey}\")"));

        // Menjalankan pemeriksaan bahwa `toolbarPosition >= 0 && toolbarPosition < titlePosition` bernilai benar; pengujian gagal jika kondisi tidak
        // terpenuhi dalam PageBackLinks_ShouldPrecedeTitleInResponsiveToolbar.
        Assert.True(toolbarPosition >= 0 && toolbarPosition < titlePosition);
        // Menjalankan pemeriksaan hasil dengan `Assert.NotEmpty` menggunakan `backLinks`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // PageBackLinks_ShouldPrecedeTitleInResponsiveToolbar.
        Assert.NotEmpty(backLinks);
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `backLinks.Cast<Match>()`, `link => Assert.InRange(link.Index, toolbarPosition,
        // titlePosition - 1)`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam PageBackLinks_ShouldPrecedeTitleInResponsiveToolbar.
        Assert.All(backLinks.Cast<Match>(), link => Assert.InRange(link.Index, toolbarPosition, titlePosition - 1));
    // Menutup scope metode PageBackLinks_ShouldPrecedeTitleInResponsiveToolbar; bagian berikut berada di luar batas blok tersebut dalam
    // PageBackLinks_ShouldPrecedeTitleInResponsiveToolbar.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SharedRulesetContent_ShouldNotDuplicatePageBackLinkInsideModalOrDetails` dengan hasil bertipe `void`; operasi ini
    // menangani shared aturan content should not duplicate page back link inside modal atau rincian.
    public void SharedRulesetContent_ShouldNotDuplicatePageBackLinkInsideModalOrDetails()
    // Membuka scope metode SharedRulesetContent_ShouldNotDuplicatePageBackLinkInsideModalOrDetails; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam SharedRulesetContent_ShouldNotDuplicatePageBackLinkInsideModalOrDetails.
    {
        // Menyiapkan variabel lokal `content` untuk nilai content dengan memanggil `File.ReadAllText` dengan `Path.Combine(ResolveRepositoryRoot(), ”src”,
        // ”Cashflowpoly.Ui”, ”Views”, ”Shared”, ”_RulesetDetailContent.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var content = File.ReadAllText(Path.Combine(ResolveRepositoryRoot(), "src", "Cashflowpoly.Ui", "Views", "Shared", "_RulesetDetailContent.cshtml"));

        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulesets.back_to_list”`,
        // `content`, `StringComparison.Ordinal` dalam SharedRulesetContent_ShouldNotDuplicatePageBackLinkInsideModalOrDetails.
        Assert.DoesNotContain("rulesets.back_to_list", content, StringComparison.Ordinal);
    // Menutup scope metode SharedRulesetContent_ShouldNotDuplicatePageBackLinkInsideModalOrDetails; bagian berikut berada di luar batas blok tersebut
    // dalam SharedRulesetContent_ShouldNotDuplicatePageBackLinkInsideModalOrDetails.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `QuickstartToggle_ShouldReferenceItsControlledPanel` dengan hasil bertipe `void`; operasi ini menangani quickstart toggle
    // should reference its controlled panel.
    public void QuickstartToggle_ShouldReferenceItsControlledPanel()
    // Membuka scope metode QuickstartToggle_ShouldReferenceItsControlledPanel; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // QuickstartToggle_ShouldReferenceItsControlledPanel.
    {
        // Menyiapkan variabel lokal `layoutContent` untuk nilai layout content dengan memanggil `File.ReadAllText` dengan
        // `Path.Combine(ResolveRepositoryRoot(), ”src”, ”Cashflowpoly.Ui”, ”Views”, ”Shared”, ”_Layout.cshtml”)`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var layoutContent = File.ReadAllText(Path.Combine(ResolveRepositoryRoot(), "src", "Cashflowpoly.Ui", "Views", "Shared", "_Layout.cshtml"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”aria-controls=\”quickstart-guide-body\””`, `layoutContent` dalam QuickstartToggle_ShouldReferenceItsControlledPanel.
        Assert.Contains("aria-controls=\"quickstart-guide-body\"", layoutContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”id=\”quickstart-guide-body\””`,
        // `layoutContent` dalam QuickstartToggle_ShouldReferenceItsControlledPanel.
        Assert.Contains("id=\"quickstart-guide-body\"", layoutContent);
    // Menutup scope metode QuickstartToggle_ShouldReferenceItsControlledPanel; bagian berikut berada di luar batas blok tersebut dalam
    // QuickstartToggle_ShouldReferenceItsControlledPanel.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”Home”, ”Index.cshtml”, 2).
    [InlineData("Home", "Index.cshtml", 2)]
    // menyediakan satu kombinasi masukan pengujian (”Sessions”, ”Index.cshtml”, 2).
    [InlineData("Sessions", "Index.cshtml", 2)]
    // menyediakan satu kombinasi masukan pengujian (”Sessions”, ”Details.cshtml”, 3).
    [InlineData("Sessions", "Details.cshtml", 3)]
    // menyediakan satu kombinasi masukan pengujian (”Players”, ”Index.cshtml”, 3).
    [InlineData("Players", "Index.cshtml", 3)]
    // menyediakan satu kombinasi masukan pengujian (”Players”, ”Details.cshtml”, 2).
    [InlineData("Players", "Details.cshtml", 2)]
    // menyediakan satu kombinasi masukan pengujian (”Rulesets”, ”Index.cshtml”, 3).
    [InlineData("Rulesets", "Index.cshtml", 3)]
    // menyediakan satu kombinasi masukan pengujian (”Rulesets”, ”Create.cshtml”, 5).
    [InlineData("Rulesets", "Create.cshtml", 5)]
    // menyediakan satu kombinasi masukan pengujian (”Rulesets”, ”Details.cshtml”, 6).
    [InlineData("Rulesets", "Details.cshtml", 6)]
    // Mendefinisikan metode `PrimaryMenuViews_ShouldUseSharedSectionHeaders` dengan hasil bertipe `void`; operasi ini menangani primary menu views
    // should use shared section headers. Masukan: Parameter `folder` bertipe `string` membawa nilai folder; Parameter `fileName` bertipe `string`
    // membawa nilai file nama; Parameter `minimumSectionTitles` bertipe `int` membawa nilai minimum section titles.
    public void PrimaryMenuViews_ShouldUseSharedSectionHeaders(string folder, string fileName, int minimumSectionTitles)
    // Membuka scope metode PrimaryMenuViews_ShouldUseSharedSectionHeaders; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // PrimaryMenuViews_ShouldUseSharedSectionHeaders.
    {
        // Menyiapkan variabel lokal `viewContent` untuk nilai view content dengan memanggil `File.ReadAllText` dengan
        // `Path.Combine(ResolveRepositoryRoot(), ”src”, ”Cashflowpoly.Ui”, ”Views”, folder, fileName)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var viewContent = File.ReadAllText(Path.Combine(ResolveRepositoryRoot(), "src", "Cashflowpoly.Ui", "Views", folder, fileName));
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `folder == ”Rulesets”` dan `fileName == ”Details.cshtml”`; sisi kanan diperiksa
        // hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam PrimaryMenuViews_ShouldUseSharedSectionHeaders.
        if (folder == "Rulesets" && fileName == "Details.cshtml")
        // Membuka scope cabang if untuk kondisi `folder == ”Rulesets” && fileName == ”Details.cshtml”`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam PrimaryMenuViews_ShouldUseSharedSectionHeaders.
        {
            // Memperbarui `viewContent` dengan menambahkan memanggil `File.ReadAllText` dengan `Path.Combine(ResolveRepositoryRoot(), ”src”, ”Cashflowpoly.Ui”,
            // ”Views”, ”Shared”, ”_RulesetDetailContent.cshtml”)` dalam PrimaryMenuViews_ShouldUseSharedSectionHeaders.
            viewContent += File.ReadAllText(Path.Combine(ResolveRepositoryRoot(), "src", "Cashflowpoly.Ui", "Views", "Shared", "_RulesetDetailContent.cshtml"));
        // Menutup scope cabang if untuk kondisi `folder == ”Rulesets” && fileName == ”Details.cshtml”`; bagian berikut berada di luar batas blok tersebut
        // dalam PrimaryMenuViews_ShouldUseSharedSectionHeaders.
        }

        // Menyiapkan variabel lokal `sectionHeadCount` untuk nilai section head jumlah dengan memanggil `Count` dengan `viewContent`,
        // `”ruleset-section-head”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sectionHeadCount = Count(viewContent, "ruleset-section-head");
        // Menyiapkan variabel lokal `sectionTitleCount` untuk nilai section title jumlah dengan memanggil `Count` dengan `viewContent`,
        // `”ruleset-section-title”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sectionTitleCount = Count(viewContent, "ruleset-section-title");
        // Menyiapkan variabel lokal `sectionSubtitleCount` untuk nilai section subtitle jumlah dengan memanggil `Count` dengan `viewContent`,
        // `”ruleset-section-subtitle”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sectionSubtitleCount = Count(viewContent, "ruleset-section-subtitle");

        // Menjalankan pemeriksaan bahwa `sectionTitleCount >= minimumSectionTitles`, `$”{folder}/{fileName} should expose at least {minimumSectionTitles}
        // shared section titles but had {sectionTitleCount}.”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // PrimaryMenuViews_ShouldUseSharedSectionHeaders.
        Assert.True(sectionTitleCount >= minimumSectionTitles, $"{folder}/{fileName} should expose at least {minimumSectionTitles} shared section titles but had {sectionTitleCount}.");
        // Menjalankan pemeriksaan bahwa `sectionHeadCount >= minimumSectionTitles`, `$”{folder}/{fileName} should expose at least {minimumSectionTitles}
        // shared section heads but had {sectionHeadCount}.”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // PrimaryMenuViews_ShouldUseSharedSectionHeaders.
        Assert.True(sectionHeadCount >= minimumSectionTitles, $"{folder}/{fileName} should expose at least {minimumSectionTitles} shared section heads but had {sectionHeadCount}.");
        // Menjalankan pemeriksaan bahwa `sectionSubtitleCount >= Math.Min(minimumSectionTitles, sectionTitleCount)`, `$”{folder}/{fileName} should keep
        // shared section subtitles aligned with section titles.”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // PrimaryMenuViews_ShouldUseSharedSectionHeaders.
        Assert.True(sectionSubtitleCount >= Math.Min(minimumSectionTitles, sectionTitleCount), $"{folder}/{fileName} should keep shared section subtitles aligned with section titles.");
    // Menutup scope metode PrimaryMenuViews_ShouldUseSharedSectionHeaders; bagian berikut berada di luar batas blok tersebut dalam
    // PrimaryMenuViews_ShouldUseSharedSectionHeaders.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `SessionJourneyPartial_ShouldUseSharedSectionHeader` dengan hasil bertipe `void`; operasi ini menangani sesi journey
    // partial should use shared section header.
    public void SessionJourneyPartial_ShouldUseSharedSectionHeader()
    // Membuka scope metode SessionJourneyPartial_ShouldUseSharedSectionHeader; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // SessionJourneyPartial_ShouldUseSharedSectionHeader.
    {
        // Menyiapkan variabel lokal `viewContent` untuk nilai view content dengan memanggil `File.ReadAllText` dengan
        // `Path.Combine(ResolveRepositoryRoot(), ”src”, ”Cashflowpoly.Ui”, ”Views”, ”Sessions”, ”_SessionJourneySection.cshtml”)`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var viewContent = File.ReadAllText(Path.Combine(ResolveRepositoryRoot(), "src", "Cashflowpoly.Ui", "Views", "Sessions", "_SessionJourneySection.cshtml"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset-section-head”`, `viewContent`,
        // `StringComparison.Ordinal` dalam SessionJourneyPartial_ShouldUseSharedSectionHeader.
        Assert.Contains("ruleset-section-head", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset-section-title”`, `viewContent`,
        // `StringComparison.Ordinal` dalam SessionJourneyPartial_ShouldUseSharedSectionHeader.
        Assert.Contains("ruleset-section-title", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset-section-subtitle”`,
        // `viewContent`, `StringComparison.Ordinal` dalam SessionJourneyPartial_ShouldUseSharedSectionHeader.
        Assert.Contains("ruleset-section-subtitle", viewContent, StringComparison.Ordinal);
    // Menutup scope metode SessionJourneyPartial_ShouldUseSharedSectionHeader; bagian berikut berada di luar batas blok tersebut dalam
    // SessionJourneyPartial_ShouldUseSharedSectionHeader.
    }

    // Mendefinisikan metode `Count` dengan hasil bertipe `int`; operasi ini menangani jumlah. Masukan: Parameter `content` bertipe `string` membawa
    // nilai content; Parameter `value` bertipe `string` membawa nilai nilai.
    private static int Count(string content, string value)
    // Membuka scope metode Count; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Count.
    {
        // Mengembalikan `Regex.Matches(content, Regex.Escape(value)).Count`, yaitu jumlah elemen atau panjang data kepada pemanggil dalam Count; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return Regex.Matches(content, Regex.Escape(value)).Count;
    // Menutup scope metode Count; bagian berikut berada di luar batas blok tersebut dalam Count.
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

        // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Unable to locate repository root.”) dalam
        // ResolveRepositoryRoot; pemanggil atau middleware penanganan error menerima kegagalan ini.
        throw new InvalidOperationException("Unable to locate repository root.");
    // Menutup scope metode ResolveRepositoryRoot; bagian berikut berada di luar batas blok tersebut dalam ResolveRepositoryRoot.
    }
// Menutup scope tipe MenuComponentConsistencyTests; bagian berikut berada di luar batas blok tersebut.
}
