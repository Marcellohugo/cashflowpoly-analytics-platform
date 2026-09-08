// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui RulesetIndexLayoutTests.
// Mengimpor namespace `System.Text.RegularExpressions` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.RegularExpressions;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `RulesetIndexLayoutTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetIndexLayoutTests
// Membuka scope tipe RulesetIndexLayoutTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell` dengan hasil bertipe `void`; operasi ini menangani aturan
    // index view should use tips panel tanpa separate guide shell.
    public void RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell()
    // Membuka scope metode RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `viewPath` untuk nilai view path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Rulesets”`, `”Index.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Rulesets", "Index.cshtml");
        // Menyiapkan variabel lokal `viewContent` untuk nilai view content dengan memanggil `File.ReadAllText` dengan `viewPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var viewContent = File.ReadAllText(viewPath);

        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset-index-guide-shell”`,
        // `viewContent` dalam RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell.
        Assert.DoesNotContain("ruleset-index-guide-shell", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset-section-title”`, `viewContent`
        // dalam RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell.
        Assert.Contains("ruleset-section-title", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset-section-subtitle”`,
        // `viewContent` dalam RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell.
        Assert.Contains("ruleset-section-subtitle", viewContent);
        // Menjalankan pemeriksaan hasil dengan `Assert.Matches` menggunakan `new Regex(@”<details\s+class=””mt-3 data-toggle””\s*>”,
        // RegexOptions.CultureInvariant)`, `viewContent`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell.
        Assert.Matches(
            // Meneruskan objek baru bertipe `Regex` dengan argumen (@”<details\s+class=””mt-3 data-toggle””\s*>”, RegexOptions.CultureInvariant) sebagai
            // argumen ke `Assert.Matches`; Meneruskan nilai literal `@”<details\s+class=””mt-3 data-toggle””\s*>”` sebagai argumen ke konstruktor `Regex`;
            // Meneruskan `RegexOptions.CultureInvariant` (nilai culture invariant) sebagai argumen ke konstruktor `Regex`.
            new Regex(@"<details\s+class=""mt-3 data-toggle""\s*>", RegexOptions.CultureInvariant),
            // Meneruskan `viewContent` (nilai view content) sebagai argumen ke `Assert.Matches`.
            viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulesets.tips.title”`, `viewContent`
        // dalam RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell.
        Assert.Contains("rulesets.tips.title", viewContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulesets.index.tips_subtitle”`,
        // `viewContent` dalam RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell.
        Assert.DoesNotContain("rulesets.index.tips_subtitle", viewContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ux-legend”`, `viewContent` dalam
        // RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell.
        Assert.DoesNotContain("ux-legend", viewContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulesets.legend.title”`,
        // `viewContent` dalam RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell.
        Assert.DoesNotContain("rulesets.legend.title", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulesets.tip.create”`, `viewContent`
        // dalam RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell.
        Assert.Contains("rulesets.tip.create", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulesets.tip.versioning”`,
        // `viewContent` dalam RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell.
        Assert.Contains("rulesets.tip.versioning", viewContent);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”rulesets.tip.safety”`, `viewContent`
        // dalam RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell.
        Assert.Contains("rulesets.tip.safety", viewContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”rulesets.default_components.title”`, `viewContent` dalam RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell.
        Assert.DoesNotContain("rulesets.default_components.title", viewContent);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”DefaultComponentItems”`,
        // `viewContent` dalam RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell.
        Assert.DoesNotContain("DefaultComponentItems", viewContent);
    // Menutup scope metode RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell; bagian berikut berada di luar batas blok tersebut dalam
    // RulesetIndexView_ShouldUseTipsPanelWithoutSeparateGuideShell.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetIndexView_ShouldRenderDefaultRowsAsReadonlyInMainTable` dengan hasil bertipe `void`; operasi ini menangani aturan
    // index view should render bawaan baris as readonly in main table.
    public void RulesetIndexView_ShouldRenderDefaultRowsAsReadonlyInMainTable()
    // Membuka scope metode RulesetIndexView_ShouldRenderDefaultRowsAsReadonlyInMainTable; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam RulesetIndexView_ShouldRenderDefaultRowsAsReadonlyInMainTable.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `viewPath` untuk nilai view path dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`, `”Rulesets”`, `”Index.cshtml”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var viewPath = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Rulesets", "Index.cshtml");
        // Menyiapkan variabel lokal `viewContent` untuk nilai view content dengan memanggil `File.ReadAllText` dengan `viewPath`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var viewContent = File.ReadAllText(viewPath);

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”item.IsDefault”`, `viewContent`,
        // `StringComparison.Ordinal` dalam RulesetIndexView_ShouldRenderDefaultRowsAsReadonlyInMainTable.
        Assert.Contains("item.IsDefault", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”item.IsLockedBySession”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulesetIndexView_ShouldRenderDefaultRowsAsReadonlyInMainTable.
        Assert.Contains("item.IsLockedBySession", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”source = item.IsDefault ?
        // DefaultCatalogSource : null”`, `viewContent`, `StringComparison.Ordinal` dalam RulesetIndexView_ShouldRenderDefaultRowsAsReadonlyInMainTable.
        Assert.Contains("source = item.IsDefault ? DefaultCatalogSource : null", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”!item.IsDefault &&
        // !item.IsLockedBySession”`, `viewContent`, `StringComparison.Ordinal` dalam RulesetIndexView_ShouldRenderDefaultRowsAsReadonlyInMainTable.
        Assert.Contains("!item.IsDefault && !item.IsLockedBySession", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”asp-route-rulesetVersionId”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulesetIndexView_ShouldRenderDefaultRowsAsReadonlyInMainTable.
        Assert.DoesNotContain("asp-route-rulesetVersionId", viewContent, StringComparison.Ordinal);
    // Menutup scope metode RulesetIndexView_ShouldRenderDefaultRowsAsReadonlyInMainTable; bagian berikut berada di luar batas blok tersebut dalam
    // RulesetIndexView_ShouldRenderDefaultRowsAsReadonlyInMainTable.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetIndexView_ShouldHideTechnicalRulesetIdColumn` dengan hasil bertipe `void`; operasi ini menangani aturan index view
    // should hide technical aturan identitas column.
    public void RulesetIndexView_ShouldHideTechnicalRulesetIdColumn()
    // Membuka scope metode RulesetIndexView_ShouldHideTechnicalRulesetIdColumn; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // RulesetIndexView_ShouldHideTechnicalRulesetIdColumn.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `viewContent` untuk nilai view content dengan memanggil `File.ReadAllText` dengan `Path.Combine(repoRoot, ”src”,
        // ”Cashflowpoly.Ui”, ”Views”, ”Rulesets”, ”Index.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var viewContent = File.ReadAllText(Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Rulesets", "Index.cshtml"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”showMutationActions ? 7 : 5”`,
        // `viewContent`, `StringComparison.Ordinal` dalam RulesetIndexView_ShouldHideTechnicalRulesetIdColumn.
        Assert.Contains("showMutationActions ? 7 : 5", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”<th class=\”px-4
        // py-3\”>@Context.T(\”common.ruleset_id\”)</th>”`, `viewContent`, `StringComparison.Ordinal` dalam
        // RulesetIndexView_ShouldHideTechnicalRulesetIdColumn.
        Assert.DoesNotContain("<th class=\"px-4 py-3\">@Context.T(\"common.ruleset_id\")</th>", viewContent, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”data-label=\”@Context.T(\”common.ruleset_id\”)\””`, `viewContent`, `StringComparison.Ordinal` dalam
        // RulesetIndexView_ShouldHideTechnicalRulesetIdColumn.
        Assert.DoesNotContain("data-label=\"@Context.T(\"common.ruleset_id\")\"", viewContent, StringComparison.Ordinal);
    // Menutup scope metode RulesetIndexView_ShouldHideTechnicalRulesetIdColumn; bagian berikut berada di luar batas blok tersebut dalam
    // RulesetIndexView_ShouldHideTechnicalRulesetIdColumn.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes` dengan hasil bertipe `void`; operasi ini menangani aturan
    // views should visually distinguish beginner dan advanced modes.
    public void RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes()
    // Membuka scope metode RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `rulesetViewsRoot` untuk nilai aturan views root dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`,
        // `”Cashflowpoly.Ui”`, `”Views”`, `”Rulesets”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetViewsRoot = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Rulesets");
        // Menyiapkan variabel lokal `indexView` untuk nilai index view dengan memanggil `File.ReadAllText` dengan `Path.Combine(rulesetViewsRoot,
        // ”Index.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var indexView = File.ReadAllText(Path.Combine(rulesetViewsRoot, "Index.cshtml"));
        // Menyiapkan variabel lokal `createView` untuk nilai create view dengan memanggil `File.ReadAllText` dengan `Path.Combine(rulesetViewsRoot,
        // ”Create.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createView = File.ReadAllText(Path.Combine(rulesetViewsRoot, "Create.cshtml"));
        // Menyiapkan variabel lokal `detailView` untuk nilai detail view dengan memanggil `File.ReadAllText` dengan `Path.Combine(repoRoot, ”src”,
        // ”Cashflowpoly.Ui”, ”Views”, ”Shared”, ”_RulesetDetailContent.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var detailView = File.ReadAllText(Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Shared", "_RulesetDetailContent.cshtml"));
        // Menyiapkan variabel lokal `css` untuk nilai css dengan memanggil `File.ReadAllText` dengan `Path.Combine(repoRoot, ”src”, ”Cashflowpoly.Ui”,
        // ”wwwroot”, ”css”, ”tailwind.input.css”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var css = File.ReadAllText(Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "wwwroot", "css", "tailwind.input.css"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”item.Mode”`, `indexView`,
        // `StringComparison.Ordinal` dalam RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes.
        Assert.Contains("item.Mode", indexView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset-mode-row--@modeCss”`,
        // `indexView`, `StringComparison.Ordinal` dalam RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes.
        Assert.Contains("ruleset-mode-row--@modeCss", indexView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset-default-badge”`, `indexView`,
        // `StringComparison.Ordinal` dalam RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes.
        Assert.Contains("ruleset-default-badge", indexView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”name=\”cfg-mode\””`, `createView`,
        // `StringComparison.Ordinal` dalam RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes.
        Assert.Contains("name=\"cfg-mode\"", createView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”data-ruleset-mode=\”pemula\””`,
        // `createView`, `StringComparison.Ordinal` dalam RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes.
        Assert.Contains("data-ruleset-mode=\"pemula\"", createView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan DoesNotContain untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”<select id=\”cfg-mode\””`,
        // `createView`, `StringComparison.Ordinal` dalam RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes.
        Assert.DoesNotContain("<select id=\"cfg-mode\"", createView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”ruleset-mode-surface--@modeCss”`,
        // `detailView`, `StringComparison.Ordinal` dalam RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes.
        Assert.Contains("ruleset-mode-surface--@modeCss", detailView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.ruleset-mode-badge--beginner”`, `css`,
        // `StringComparison.Ordinal` dalam RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes.
        Assert.Contains(".ruleset-mode-badge--beginner", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”.ruleset-mode-badge--advanced”`, `css`,
        // `StringComparison.Ordinal` dalam RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes.
        Assert.Contains(".ruleset-mode-badge--advanced", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”background: #dbeafe;”`, `css`,
        // `StringComparison.Ordinal` dalam RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes.
        Assert.Contains("background: #dbeafe;", css, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”color: #1d4ed8;”`, `css`,
        // `StringComparison.Ordinal` dalam RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes.
        Assert.Contains("color: #1d4ed8;", css, StringComparison.Ordinal);
    // Menutup scope metode RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes; bagian berikut berada di luar batas blok tersebut dalam
    // RulesetViews_ShouldVisuallyDistinguishBeginnerAndAdvancedModes.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RulesetViews_ShouldExposeAdvancedFeaturesOnlyForAdvancedMode` dengan hasil bertipe `void`; operasi ini menangani aturan
    // views should expose advanced features only untuk advanced mode.
    public void RulesetViews_ShouldExposeAdvancedFeaturesOnlyForAdvancedMode()
    // Membuka scope metode RulesetViews_ShouldExposeAdvancedFeaturesOnlyForAdvancedMode; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam RulesetViews_ShouldExposeAdvancedFeaturesOnlyForAdvancedMode.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `rulesetViewsRoot` untuk nilai aturan views root dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`,
        // `”Cashflowpoly.Ui”`, `”Views”`, `”Rulesets”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetViewsRoot = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Rulesets");
        // Menyiapkan variabel lokal `createView` untuk nilai create view dengan memanggil `File.ReadAllText` dengan `Path.Combine(rulesetViewsRoot,
        // ”Create.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var createView = File.ReadAllText(Path.Combine(rulesetViewsRoot, "Create.cshtml"));
        // Menyiapkan variabel lokal `detailView` untuk nilai detail view dengan memanggil `File.ReadAllText` dengan `Path.Combine(repoRoot, ”src”,
        // ”Cashflowpoly.Ui”, ”Views”, ”Shared”, ”_RulesetDetailContent.cshtml”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var detailView = File.ReadAllText(Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views", "Shared", "_RulesetDetailContent.cshtml"));

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”data-ruleset-advanced-features
        // hidden”`, `createView`, `StringComparison.Ordinal` dalam RulesetViews_ShouldExposeAdvancedFeaturesOnlyForAdvancedMode.
        Assert.Contains("data-ruleset-advanced-features hidden", createView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”setAdvancedFeatureAvailability”`,
        // `createView`, `StringComparison.Ordinal` dalam RulesetViews_ShouldExposeAdvancedFeaturesOnlyForAdvancedMode.
        Assert.Contains("setAdvancedFeatureAvailability", createView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”input.disabled = !isAdvanced”`,
        // `createView`, `StringComparison.Ordinal` dalam RulesetViews_ShouldExposeAdvancedFeaturesOnlyForAdvancedMode.
        Assert.Contains("input.disabled = !isAdvanced", createView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”getMode() === \”MAHIR\” &&
        // toBool(\”cfg-adv-loan\”)”`, `createView`, `StringComparison.Ordinal` dalam RulesetViews_ShouldExposeAdvancedFeaturesOnlyForAdvancedMode.
        Assert.Contains("getMode() === \"MAHIR\" && toBool(\"cfg-adv-loan\")", createView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”@if (isAdvancedMode)”`, `detailView`,
        // `StringComparison.Ordinal` dalam RulesetViews_ShouldExposeAdvancedFeaturesOnlyForAdvancedMode.
        Assert.Contains("@if (isAdvancedMode)", detailView, StringComparison.Ordinal);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan:
        // `”ruleset-advanced-feature-grid--active”`, `detailView`, `StringComparison.Ordinal` dalam
        // RulesetViews_ShouldExposeAdvancedFeaturesOnlyForAdvancedMode.
        Assert.Contains("ruleset-advanced-feature-grid--active", detailView, StringComparison.Ordinal);
    // Menutup scope metode RulesetViews_ShouldExposeAdvancedFeaturesOnlyForAdvancedMode; bagian berikut berada di luar batas blok tersebut dalam
    // RulesetViews_ShouldExposeAdvancedFeaturesOnlyForAdvancedMode.
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
// Menutup scope tipe RulesetIndexLayoutTests; bagian berikut berada di luar batas blok tersebut.
}
