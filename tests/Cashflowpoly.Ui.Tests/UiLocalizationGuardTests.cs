// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui UiLocalizationGuardTests.
// Mengimpor namespace `System.Text.RegularExpressions` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.RegularExpressions;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

/// <summary>
/// Kelas pengujian unit yang memastikan semua key terjemahan yang digunakan di UI
/// terdaftar pada UiText, dan tidak ada pesan error atau teks literal hardcoded.
/// </summary>
// Mendefinisikan tipe class `UiLocalizationGuardTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class UiLocalizationGuardTests
// Membuka scope tipe UiLocalizationGuardTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Regex untuk mengekstrak key dari dictionary leksikon UiText (pola ["key"] = ...).
    /// </summary>
    // Mendeklarasikan field bertipe `Regex`: `LexiconKeyRegex` menyimpan nilai lexicon kunci regex dengan nilai awal objek baru dengan tipe mengikuti
    // konteks tujuan dan argumen (@”\[””(?<key>[^””]+)””\]\s*=”, RegexOptions.Compiled). readonly membatasi penggantian referensi/nilai field pada
    // deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly Regex LexiconKeyRegex = new(@"\[""(?<key>[^""]+)""\]\s*=", RegexOptions.Compiled);
    /// <summary>
    /// Regex untuk mendeteksi pemanggilan Context.T("key") atau HttpContext.T("key") di file C#/Razor.
    /// </summary>
    // Mendeklarasikan field bertipe `Regex`: `TranslationCallRegex` menyimpan nilai translation call regex dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (@”(?:Context|HttpContext)\.T\(””(?<key>[^””]+)””\)”, RegexOptions.Compiled). readonly membatasi penggantian
    // referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly Regex TranslationCallRegex = new(@"(?:Context|HttpContext)\.T\(""(?<key>[^""]+)""\)", RegexOptions.Compiled);
    /// <summary>
    /// Regex untuk mendeteksi node teks literal di dalam elemen HTML pada Razor view.
    /// </summary>
    // Mendeklarasikan field bertipe `Regex`: `LiteralTextNodeRegex` menyimpan nilai literal text node regex dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen ( @”<[^!/][^>]*>\s*(?<text>[A-Za-z][A-Za-z0-9\s\.,'””/&()\-:]{0,120})\s*</[^>]+>”, RegexOptions.Compiled).
    // readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar
    // instance.
    private static readonly Regex LiteralTextNodeRegex = new(
        // Meneruskan nilai literal `@”<[^!/][^>]*>\s*(?<text>[A-Za-z][A-Za-z0-9\s\.,'””/&()\-:]{0,120})\s*</[^>]+>”` sebagai argumen ke konstruktor dengan
        // tipe mengikuti konteks.
        @"<[^!/][^>]*>\s*(?<text>[A-Za-z][A-Za-z0-9\s\.,'""/&()\-:]{0,120})\s*</[^>]+>",
        // Meneruskan `RegexOptions.Compiled` (nilai compiled) sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        RegexOptions.Compiled);
    /// <summary>
    /// Regex untuk mendeteksi assignment pesan error hardcoded pada variabel controller UI.
    /// </summary>
    // Mendeklarasikan field bertipe `Regex`: `HardcodedErrorAssignmentRegex` menyimpan nilai hardcoded kesalahan assignment regex dengan nilai awal
    // objek baru dengan tipe mengikuti konteks tujuan dan argumen (
    // @”\b(?:ErrorMessage|SessionLookupErrorMessage|RulesetErrorMessage|groupError|gameplayError|timelineErrorMessage|errorMessage)\s*=\s*\$?”””,
    // RegexOptions.Compiled). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik
    // tipe dan dibagikan antar instance.
    private static readonly Regex HardcodedErrorAssignmentRegex = new(
        // Meneruskan nilai literal `@”\b(?:ErrorMessage|SessionLookupErrorMessage|RulesetErrorMessage|groupError|gameplayError|timelineErrorMessage|errorMe
        // ssage)\s*=\s*\$?”””` sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        @"\b(?:ErrorMessage|SessionLookupErrorMessage|RulesetErrorMessage|groupError|gameplayError|timelineErrorMessage|errorMessage)\s*=\s*\$?""",
        // Meneruskan `RegexOptions.Compiled` (nilai compiled) sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        RegexOptions.Compiled);
    /// <summary>
    /// Regex untuk mendeteksi assignment string hardcoded ke TempData pada controller UI.
    /// </summary>
    // Mendeklarasikan field bertipe `Regex`: `HardcodedTempDataAssignmentRegex` menyimpan nilai hardcoded temp data assignment regex dengan nilai awal
    // objek baru dengan tipe mengikuti konteks tujuan dan argumen ( @”TempData\[[^\]]+\]\s*=\s*\$?”””, RegexOptions.Compiled). readonly membatasi
    // penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly Regex HardcodedTempDataAssignmentRegex = new(
        // Meneruskan nilai literal `@”TempData\[[^\]]+\]\s*=\s*\$?”””` sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        @"TempData\[[^\]]+\]\s*=\s*\$?""",
        // Meneruskan `RegexOptions.Compiled` (nilai compiled) sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        RegexOptions.Compiled);

    /// <summary>
    /// Daftar teks literal yang diizinkan muncul langsung di Razor view tanpa terjemahan.
    /// </summary>
    // Mendeklarasikan field bertipe `HashSet<string>`: `AllowedLiteralViewTextNodes` menyimpan nilai allowed literal view text nodes dengan nilai awal
    // objek baru dengan tipe mengikuti konteks tujuan dan argumen (StringComparer.Ordinal). readonly membatasi penggantian referensi/nilai field pada
    // deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly HashSet<string> AllowedLiteralViewTextNodes = new(StringComparer.Ordinal)
    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Menggunakan nilai literal `”Cashflowpoly”` sebagai bagian ekspresi yang sedang disusun.
        "Cashflowpoly",
        // Menggunakan nilai literal `”ID”` sebagai bagian ekspresi yang sedang disusun.
        "ID",
        // Menggunakan nilai literal `”EN”` sebagai bagian ekspresi yang sedang disusun.
        "EN"
    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut.
    };

    /// <summary>
    /// Path root repositori yang ditemukan dengan menelusuri ke atas dari BaseDirectory.
    /// </summary>
    // Mendeklarasikan field bertipe `string`: `RepoRoot` menyimpan nilai repo root dengan nilai awal memanggil `ResolveRepositoryRoot` dengan tanpa
    // argumen. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan
    // dibagikan antar instance.
    private static readonly string RepoRoot = ResolveRepositoryRoot();

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa setiap key yang dipanggil via Context.T() di controller dan view
    /// UI terdaftar dalam dictionary leksikon UiText.cs.
    /// </summary>
    // Mendefinisikan metode `TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon` dengan hasil bertipe `void`; operasi ini menangani
    // translation kunci used berdasarkan ui controllers dan views must exist in ui text lexicon.
    public void TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon()
    // Membuka scope metode TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon.
    {
        // Menyiapkan variabel lokal `uiRoot` untuk nilai ui root dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`, `”Cashflowpoly.Ui”`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var uiRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui");
        // Menyiapkan variabel lokal `infrastructureRoot` untuk nilai infrastructure root dengan memanggil `Path.Combine` dengan `uiRoot`,
        // `”Infrastructure”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var infrastructureRoot = Path.Combine(uiRoot, "Infrastructure");
        // Menyiapkan variabel lokal `lexiconContent` untuk nilai lexicon content dengan memanggil `string.Join` dengan `Environment.NewLine`, `Directory
        // .EnumerateFiles(infrastructureRoot, ”UiText*.cs”, SearchOption.TopDirectoryOnly) .Select(File.ReadAllText)`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var lexiconContent = string.Join(
            // Meneruskan `Environment.NewLine` (nilai new line) sebagai argumen ke `string.Join`.
            Environment.NewLine,
            // Meneruskan memetakan setiap elemen `Directory .EnumerateFiles(infrastructureRoot, ”UiText*.cs”, SearchOption.TopDirectoryOnly)` melalui
            // `File.ReadAllText` menjadi bentuk hasil yang dibutuhkan sebagai argumen ke `string.Join`.
            Directory
                // Meneruskan `infrastructureRoot` (nilai infrastructure root) sebagai argumen ke `Directory .EnumerateFiles`; Meneruskan nilai literal
                // `”UiText*.cs”` sebagai argumen ke `Directory .EnumerateFiles`; Meneruskan `SearchOption.TopDirectoryOnly` (nilai top directory only) sebagai
                // argumen ke `Directory .EnumerateFiles`.
                .EnumerateFiles(infrastructureRoot, "UiText*.cs", SearchOption.TopDirectoryOnly)
                // Meneruskan `File.ReadAllText` (nilai read all text) sebagai argumen ke `Directory .EnumerateFiles(infrastructureRoot, ”UiText*.cs”,
                // SearchOption.TopDirectoryOnly) .Select`.
                .Select(File.ReadAllText));
        // Menyiapkan variabel lokal `lexiconKeys` untuk nilai lexicon kunci dengan membentuk himpunan nilai unik dari `LexiconKeyRegex
        // .Matches(lexiconContent) .Select(match => match.Groups[”key”].Value) .Where(key => !string.IsNullOrWhiteSpace(key))` memakai
        // `StringComparer.OrdinalIgnoreCase`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var lexiconKeys = LexiconKeyRegex
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Matches(lexiconContent) dalam
            // TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Matches(lexiconContent)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(match => match.Groups[”key”].Value) dalam
            // TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(match => match.Groups["key"].Value)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(key => !string.IsNullOrWhiteSpace(key)) dalam
            // TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(key => !string.IsNullOrWhiteSpace(key))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToHashSet(StringComparer.OrdinalIgnoreCase); dalam
            // TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Menyiapkan variabel lokal `uiFiles` untuk nilai ui files dengan menyaring elemen `Directory .EnumerateFiles(uiRoot, ”*.*”,
        // SearchOption.AllDirectories)` dengan predikat `path => path.EndsWith(”.cs”, StringComparison.OrdinalIgnoreCase) || path.EndsWith(”.cshtml”,
        // StringComparison.OrdinalIgnoreCase)`; hanya elemen yang memenuhi kondisi diteruskan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var uiFiles = Directory
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateFiles(uiRoot, ”*.*”, SearchOption.AllDirectories) dalam
            // TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .EnumerateFiles(uiRoot, "*.*", SearchOption.AllDirectories)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(path => path.EndsWith(”.cs”, StringComparison.OrdinalIgnoreCase) dalam
            // TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(path => path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
                // Meneruskan nilai literal `”.cshtml”` sebagai argumen ke `path.EndsWith`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore
                // case) sebagai argumen ke `path.EndsWith`.
                || path.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase));

        // Menyiapkan variabel lokal `missingKeys` untuk nilai missing kunci dengan objek baru bertipe `HashSet<string>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var missingKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Mengulangi setiap elemen `uiFiles`; elemen saat ini disimpan sebagai `filePath` bertipe `var` untuk diproses oleh badan loop dalam
        // TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon.
        foreach (var filePath in uiFiles)
        // Membuka scope loop setiap filePath dari `uiFiles`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon.
        {
            // Menyiapkan variabel lokal `content` untuk nilai content dengan memanggil `File.ReadAllText` dengan `filePath`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal.
            var content = File.ReadAllText(filePath);
            // Mengulangi setiap elemen `TranslationCallRegex.Matches(content)`; elemen saat ini disimpan sebagai `match` bertipe `Match` untuk diproses oleh
            // badan loop dalam TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon.
            foreach (Match match in TranslationCallRegex.Matches(content))
            // Membuka scope loop setiap match dari `TranslationCallRegex.Matches(content)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon.
            {
                // Menyiapkan variabel lokal `key` untuk nilai kunci dengan `match.Groups[”key”].Value`, yaitu nilai yang dibungkus objek/nullable. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var key = match.Groups["key"].Value;
                // Memeriksa kebalikan kondisi `lexiconKeys.Contains(key)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                // TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon.
                if (!lexiconKeys.Contains(key))
                // Membuka scope cabang if untuk kondisi `!lexiconKeys.Contains(key)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon.
                {
                    // Menjalankan menambahkan `key` ke `missingKeys` dalam TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon.
                    missingKeys.Add(key);
                // Menutup scope cabang if untuk kondisi `!lexiconKeys.Contains(key)`; bagian berikut berada di luar batas blok tersebut dalam
                // TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon.
                }
            // Menutup scope loop setiap match dari `TranslationCallRegex.Matches(content)`; bagian berikut berada di luar batas blok tersebut dalam
            // TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon.
            }
        // Menutup scope loop setiap filePath dari `uiFiles`; bagian berikut berada di luar batas blok tersebut dalam
        // TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon.
        }

        // Menjalankan pemeriksaan bahwa `missingKeys.Count == 0`, `$”Ditemukan key terjemahan yang tidak ada di UiText: {string.Join(”, ”,
        // missingKeys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase))}”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon.
        Assert.True(
            // Meneruskan perbandingan kesamaan antara `missingKeys.Count` dan `0` sebagai argumen ke `Assert.True`.
            missingKeys.Count == 0,
            // Meneruskan teks interpolasi `$”Ditemukan key terjemahan yang tidak ada di UiText: {string.Join(”, ”, missingKeys.OrderBy(key => key,
            // StringComparer.OrdinalIgnoreCase))}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `Assert.True`;
            // Meneruskan nilai literal `”, ”` sebagai argumen ke `string.Join`; Meneruskan mengurutkan `missingKeys` secara menaik berdasarkan `key => key`,
            // `StringComparer.OrdinalIgnoreCase` sebagai argumen ke `string.Join`; Meneruskan fungsi lambda `key => key` yang dijalankan oleh operasi pemanggil
            // untuk memproses setiap masukan sebagai argumen ke `missingKeys.OrderBy`; Meneruskan `StringComparer.OrdinalIgnoreCase` (nilai ordinal ignore
            // case) sebagai argumen ke `missingKeys.OrderBy`.
            $"Ditemukan key terjemahan yang tidak ada di UiText: {string.Join(", ", missingKeys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase))}");
    // Menutup scope metode TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon; bagian berikut berada di luar batas blok tersebut
    // dalam TranslationKeys_UsedByUiControllersAndViews_MustExistInUiTextLexicon.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa controller UI tidak mengandung assignment pesan error
    /// hardcoded yang seharusnya menggunakan key terjemahan.
    /// </summary>
    // Mendefinisikan metode `UiControllers_ShouldNotUseHardcodedUserFacingErrorMessages` dengan hasil bertipe `void`; operasi ini menangani ui
    // controllers should not use hardcoded pengguna facing kesalahan pesan.
    public void UiControllers_ShouldNotUseHardcodedUserFacingErrorMessages()
    // Membuka scope metode UiControllers_ShouldNotUseHardcodedUserFacingErrorMessages; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam UiControllers_ShouldNotUseHardcodedUserFacingErrorMessages.
    {
        // Menyiapkan variabel lokal `controllerRoot` untuk nilai controller root dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`,
        // `”Cashflowpoly.Ui”`, `”Controllers”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var controllerRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Controllers");
        // Menyiapkan variabel lokal `controllerFiles` untuk nilai controller files dengan memanggil `Directory.EnumerateFiles` dengan `controllerRoot`,
        // `”*.cs”`, `SearchOption.TopDirectoryOnly`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var controllerFiles = Directory.EnumerateFiles(controllerRoot, "*.cs", SearchOption.TopDirectoryOnly);
        // Menyiapkan variabel lokal `violations` untuk nilai violations dengan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var violations = new List<string>();

        // Mengulangi setiap elemen `controllerFiles`; elemen saat ini disimpan sebagai `filePath` bertipe `var` untuk diproses oleh badan loop dalam
        // UiControllers_ShouldNotUseHardcodedUserFacingErrorMessages.
        foreach (var filePath in controllerFiles)
        // Membuka scope loop setiap filePath dari `controllerFiles`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // UiControllers_ShouldNotUseHardcodedUserFacingErrorMessages.
        {
            // Menyiapkan variabel lokal `content` untuk nilai content dengan memanggil `File.ReadAllText` dengan `filePath`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal.
            var content = File.ReadAllText(filePath);
            // Menjalankan menambahkan seluruh elemen `FindViolations(filePath, content, HardcodedErrorAssignmentRegex)` ke `violations` dalam
            // UiControllers_ShouldNotUseHardcodedUserFacingErrorMessages.
            violations.AddRange(FindViolations(filePath, content, HardcodedErrorAssignmentRegex));
            // Menjalankan menambahkan seluruh elemen `FindViolations(filePath, content, HardcodedTempDataAssignmentRegex)` ke `violations` dalam
            // UiControllers_ShouldNotUseHardcodedUserFacingErrorMessages.
            violations.AddRange(FindViolations(filePath, content, HardcodedTempDataAssignmentRegex));
        // Menutup scope loop setiap filePath dari `controllerFiles`; bagian berikut berada di luar batas blok tersebut dalam
        // UiControllers_ShouldNotUseHardcodedUserFacingErrorMessages.
        }

        // Menjalankan pemeriksaan bahwa `violations.Count == 0`, `$”Ditemukan pesan error hardcoded pada controller
        // UI:{Environment.NewLine}{string.Join(Environment.NewLine, violations)}”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // UiControllers_ShouldNotUseHardcodedUserFacingErrorMessages.
        Assert.True(
            // Meneruskan perbandingan kesamaan antara `violations.Count` dan `0` sebagai argumen ke `Assert.True`.
            violations.Count == 0,
            // Meneruskan teks interpolasi `$”Ditemukan pesan error hardcoded pada controller UI:{Environment.NewLine}{string.Join(Environment.NewLine,
            // violations)}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `Assert.True`; Meneruskan
            // `Environment.NewLine` (nilai new line) sebagai argumen ke `string.Join`; Meneruskan `violations` (nilai violations) sebagai argumen ke
            // `string.Join`.
            $"Ditemukan pesan error hardcoded pada controller UI:{Environment.NewLine}{string.Join(Environment.NewLine, violations)}");
    // Menutup scope metode UiControllers_ShouldNotUseHardcodedUserFacingErrorMessages; bagian berikut berada di luar batas blok tersebut dalam
    // UiControllers_ShouldNotUseHardcodedUserFacingErrorMessages.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    /// <summary>
    /// Memvalidasi bahwa Razor view tidak mengandung node teks literal hardcoded
    /// yang seharusnya menggunakan mekanisme terjemahan, kecuali yang diizinkan.
    /// </summary>
    // Mendefinisikan metode `UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes` dengan hasil bertipe `void`; operasi ini menangani ui views
    // should not contain unexpected hardcoded literal text nodes.
    public void UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes()
    // Membuka scope metode UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
    {
        // Menyiapkan variabel lokal `viewsRoot` untuk nilai views root dengan memanggil `Path.Combine` dengan `RepoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var viewsRoot = Path.Combine(RepoRoot, "src", "Cashflowpoly.Ui", "Views");
        // Menyiapkan variabel lokal `viewFiles` untuk nilai view files dengan memanggil `Directory.EnumerateFiles` dengan `viewsRoot`, `”*.cshtml”`,
        // `SearchOption.AllDirectories`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var viewFiles = Directory.EnumerateFiles(viewsRoot, "*.cshtml", SearchOption.AllDirectories);
        // Menyiapkan variabel lokal `violations` untuk nilai violations dengan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var violations = new List<string>();

        // Mengulangi setiap elemen `viewFiles`; elemen saat ini disimpan sebagai `filePath` bertipe `var` untuk diproses oleh badan loop dalam
        // UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
        foreach (var filePath in viewFiles)
        // Membuka scope loop setiap filePath dari `viewFiles`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
        {
            // Menyiapkan variabel lokal `content` untuk nilai content dengan memanggil `File.ReadAllText` dengan `filePath`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal.
            var content = File.ReadAllText(filePath);
            // Mengulangi setiap elemen `LiteralTextNodeRegex.Matches(content)`; elemen saat ini disimpan sebagai `match` bertipe `Match` untuk diproses oleh
            // badan loop dalam UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
            foreach (Match match in LiteralTextNodeRegex.Matches(content))
            // Membuka scope loop setiap match dari `LiteralTextNodeRegex.Matches(content)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
            {
                // Menyiapkan variabel lokal `text` untuk nilai text dengan membersihkan karakter tepi pada `match.Groups[”text”].Value` memakai tanpa argumen. Tipe
                // variabel disimpulkan dari ekspresi nilai awal.
                var text = match.Groups["text"].Value.Trim();
                // Memeriksa memeriksa apakah `text` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
                // dalam UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
                if (string.IsNullOrWhiteSpace(text))
                // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(text)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam
                    // UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
                    continue;
                // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(text)`; bagian berikut berada di luar batas blok tersebut dalam
                // UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
                }

                // Memeriksa memeriksa apakah `AllowedLiteralViewTextNodes` memuat `text`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                // UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
                if (AllowedLiteralViewTextNodes.Contains(text))
                // Membuka scope cabang if untuk kondisi `AllowedLiteralViewTextNodes.Contains(text)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
                // dalam UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
                {
                    // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam
                    // UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
                    continue;
                // Menutup scope cabang if untuk kondisi `AllowedLiteralViewTextNodes.Contains(text)`; bagian berikut berada di luar batas blok tersebut dalam
                // UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
                }

                // Menyiapkan variabel lokal `line` untuk nilai line dengan penjumlahan/penggabungan antara `content[..match.Index].Count(ch => ch == '\n')` dan
                // `1`. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var line = content[..match.Index].Count(ch => ch == '\n') + 1;
                // Menyiapkan variabel lokal `relativePath` untuk nilai relative path dengan memanggil `Path.GetRelativePath(RepoRoot, filePath).Replace` dengan
                // `'\\'`, `'/'`. Tipe variabel disimpulkan dari ekspresi nilai awal.
                var relativePath = Path.GetRelativePath(RepoRoot, filePath).Replace('\\', '/');
                // Menjalankan menambahkan `$”{relativePath}:{line} -> \”{text}\””` ke `violations` dalam
                // UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
                violations.Add($"{relativePath}:{line} -> \"{text}\"");
            // Menutup scope loop setiap match dari `LiteralTextNodeRegex.Matches(content)`; bagian berikut berada di luar batas blok tersebut dalam
            // UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
            }
        // Menutup scope loop setiap filePath dari `viewFiles`; bagian berikut berada di luar batas blok tersebut dalam
        // UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
        }

        // Menjalankan pemeriksaan bahwa `violations.Count == 0`, `$”Ditemukan literal text node hardcoded pada Razor
        // view:{Environment.NewLine}{string.Join(Environment.NewLine, violations)}”` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
        Assert.True(
            // Meneruskan perbandingan kesamaan antara `violations.Count` dan `0` sebagai argumen ke `Assert.True`.
            violations.Count == 0,
            // Meneruskan teks interpolasi `$”Ditemukan literal text node hardcoded pada Razor view:{Environment.NewLine}{string.Join(Environment.NewLine,
            // violations)}”`; nilai ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `Assert.True`; Meneruskan
            // `Environment.NewLine` (nilai new line) sebagai argumen ke `string.Join`; Meneruskan `violations` (nilai violations) sebagai argumen ke
            // `string.Join`.
            $"Ditemukan literal text node hardcoded pada Razor view:{Environment.NewLine}{string.Join(Environment.NewLine, violations)}");
    // Menutup scope metode UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes; bagian berikut berada di luar batas blok tersebut dalam
    // UiViews_ShouldNotContainUnexpectedHardcodedLiteralTextNodes.
    }

    /// <summary>
    /// Helper yang mencari semua kecocokan regex dalam konten file dan mengembalikan
    /// daftar pelanggaran beserta lokasi baris relatif terhadap root repositori.
    /// </summary>
    // Mendefinisikan metode `FindViolations` dengan hasil bertipe `IEnumerable<string>`. Helper yang mencari semua kecocokan regex dalam konten file
    // dan mengembalikan daftar pelanggaran beserta lokasi baris relatif terhadap root repositori. Masukan: Parameter `filePath` bertipe `string`
    // membawa nilai file path; Parameter `content` bertipe `string` membawa nilai content; Parameter `pattern` bertipe `Regex` membawa nilai pattern.
    private static IEnumerable<string> FindViolations(string filePath, string content, Regex pattern)
    // Membuka scope metode FindViolations; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FindViolations.
    {
        // Mengulangi setiap elemen `pattern.Matches(content)`; elemen saat ini disimpan sebagai `match` bertipe `Match` untuk diproses oleh badan loop
        // dalam FindViolations.
        foreach (Match match in pattern.Matches(content))
        // Membuka scope loop setiap match dari `pattern.Matches(content)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // FindViolations.
        {
            // Menyiapkan variabel lokal `line` untuk nilai line dengan penjumlahan/penggabungan antara `content[..match.Index].Count(ch => ch == '\n')` dan
            // `1`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var line = content[..match.Index].Count(ch => ch == '\n') + 1;
            // Melengkapi struktur ekspresi YieldReturnStatement melalui yield return $”{Path.GetRelativePath(RepoRoot, filePath).Replace('\\', '/')}:{line}”;
            // dalam FindViolations; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            yield return $"{Path.GetRelativePath(RepoRoot, filePath).Replace('\\', '/')}:{line}";
        // Menutup scope loop setiap match dari `pattern.Matches(content)`; bagian berikut berada di luar batas blok tersebut dalam FindViolations.
        }
    // Menutup scope metode FindViolations; bagian berikut berada di luar batas blok tersebut dalam FindViolations.
    }

    /// <summary>
    /// Helper yang menelusuri direktori ke atas dari AppContext.BaseDirectory
    /// untuk menemukan root repositori berdasarkan keberadaan file Cashflowpoly.sln.
    /// </summary>
    // Mendefinisikan metode `ResolveRepositoryRoot` dengan hasil bertipe `string`. Helper yang menelusuri direktori ke atas dari
    // AppContext.BaseDirectory untuk menemukan root repositori berdasarkan keberadaan file Cashflowpoly.sln.
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
// Menutup scope tipe UiLocalizationGuardTests; bagian berikut berada di luar batas blok tersebut.
}
