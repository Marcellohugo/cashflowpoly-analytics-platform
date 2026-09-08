// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui UiTextLexiconStructureTests.
// Mengimpor namespace `System.Text.RegularExpressions` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.RegularExpressions;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Tests;

// Mendefinisikan tipe class `UiTextLexiconStructureTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class UiTextLexiconStructureTests
// Membuka scope tipe UiTextLexiconStructureTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `Regex`: `LexiconEntryRegex` menyimpan nilai lexicon entry regex dengan nilai awal objek baru dengan tipe mengikuti
    // konteks tujuan dan argumen ( ”””terms\[\”(?<key>[^\”]+)\”\]\s*=\s*\(\”(?<id>(?:\\.|[^\”])*)\”,\s*\”(?<en>(?:\\.|[^\”])*)\”\);”””,
    // RegexOptions.Compiled). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik
    // tipe dan dibagikan antar instance.
    private static readonly Regex LexiconEntryRegex = new(
        // Meneruskan nilai literal `”””terms\[\”(?<key>[^\”]+)\”\]\s*=\s*\(\”(?<id>(?:\\.|[^\”])*)\”,\s*\”(?<en>(?:\\.|[^\”])*)\”\);”””` sebagai argumen ke
        // konstruktor dengan tipe mengikuti konteks.
        """terms\[\"(?<key>[^\"]+)\"\]\s*=\s*\(\"(?<id>(?:\\.|[^\"])*)\",\s*\"(?<en>(?:\\.|[^\"])*)\"\);""",
        // Meneruskan `RegexOptions.Compiled` (nilai compiled) sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        RegexOptions.Compiled);

    // Mendeklarasikan field bertipe `Regex`: `PlaceholderRegex` menyimpan nilai placeholder regex dengan nilai awal objek baru dengan tipe mengikuti
    // konteks tujuan dan argumen (@”\{[^{}]+\}”, RegexOptions.Compiled). readonly membatasi penggantian referensi/nilai field pada deklarasi atau
    // konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly Regex PlaceholderRegex = new(@"\{[^{}]+\}", RegexOptions.Compiled);

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `UiTextLexicon_ShouldBeSplitAcrossDomainFiles` dengan hasil bertipe `void`; operasi ini menangani ui text lexicon should be
    // split across domain files.
    public void UiTextLexicon_ShouldBeSplitAcrossDomainFiles()
    // Membuka scope metode UiTextLexicon_ShouldBeSplitAcrossDomainFiles; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // UiTextLexicon_ShouldBeSplitAcrossDomainFiles.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `infrastructureRoot` untuk nilai infrastructure root dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`,
        // `”Cashflowpoly.Ui”`, `”Infrastructure”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var infrastructureRoot = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Infrastructure");
        // Menyiapkan variabel lokal `uiTextPath` untuk nilai ui text path dengan memanggil `Path.Combine` dengan `infrastructureRoot`, `”UiText.cs”`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var uiTextPath = Path.Combine(infrastructureRoot, "UiText.cs");
        // Menyiapkan variabel lokal `uiTextLineCount` untuk nilai ui text line jumlah dengan memanggil `File.ReadLines(uiTextPath).Count` dengan tanpa
        // argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var uiTextLineCount = File.ReadLines(uiTextPath).Count();
        // Menyiapkan variabel lokal `lexiconFiles` untuk nilai lexicon files dengan membentuk himpunan nilai unik dari `Directory
        // .EnumerateFiles(infrastructureRoot, ”UiTextLexicon*.cs”, SearchOption.TopDirectoryOnly) .Select(Path.GetFileName)` memakai
        // `StringComparer.OrdinalIgnoreCase`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var lexiconFiles = Directory
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateFiles(infrastructureRoot, ”UiTextLexicon*.cs”,
            // SearchOption.TopDirectoryOnly) dalam UiTextLexicon_ShouldBeSplitAcrossDomainFiles; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .EnumerateFiles(infrastructureRoot, "UiTextLexicon*.cs", SearchOption.TopDirectoryOnly)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(Path.GetFileName) dalam UiTextLexicon_ShouldBeSplitAcrossDomainFiles;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(Path.GetFileName)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToHashSet(StringComparer.OrdinalIgnoreCase); dalam
            // UiTextLexicon_ShouldBeSplitAcrossDomainFiles; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Menjalankan pemeriksaan bahwa `uiTextLineCount < 250`, `$”UiText.cs masih terlalu besar: {uiTextLineCount} baris.”` bernilai benar; pengujian
        // gagal jika kondisi tidak terpenuhi dalam UiTextLexicon_ShouldBeSplitAcrossDomainFiles.
        Assert.True(uiTextLineCount < 250, $"UiText.cs masih terlalu besar: {uiTextLineCount} baris.");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”UiTextLexicon.Core.cs”`, `lexiconFiles`
        // dalam UiTextLexicon_ShouldBeSplitAcrossDomainFiles.
        Assert.Contains("UiTextLexicon.Core.cs", lexiconFiles);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”UiTextLexicon.Sessions.cs”`,
        // `lexiconFiles` dalam UiTextLexicon_ShouldBeSplitAcrossDomainFiles.
        Assert.Contains("UiTextLexicon.Sessions.cs", lexiconFiles);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”UiTextLexicon.Players.cs”`,
        // `lexiconFiles` dalam UiTextLexicon_ShouldBeSplitAcrossDomainFiles.
        Assert.Contains("UiTextLexicon.Players.cs", lexiconFiles);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”UiTextLexicon.Rulesets.cs”`,
        // `lexiconFiles` dalam UiTextLexicon_ShouldBeSplitAcrossDomainFiles.
        Assert.Contains("UiTextLexicon.Rulesets.cs", lexiconFiles);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”UiTextLexicon.Rulebook.cs”`,
        // `lexiconFiles` dalam UiTextLexicon_ShouldBeSplitAcrossDomainFiles.
        Assert.Contains("UiTextLexicon.Rulebook.cs", lexiconFiles);
    // Menutup scope metode UiTextLexicon_ShouldBeSplitAcrossDomainFiles; bagian berikut berada di luar batas blok tersebut dalam
    // UiTextLexicon_ShouldBeSplitAcrossDomainFiles.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `BackActions_ShouldRenderArrowPrefix` dengan hasil bertipe `void`; operasi ini menangani back aksi should render arrow
    // prefix.
    public void BackActions_ShouldRenderArrowPrefix()
    // Membuka scope metode BackActions_ShouldRenderArrowPrefix; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // BackActions_ShouldRenderArrowPrefix.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `viewsRoot` untuk nilai views root dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`, `”Cashflowpoly.Ui”`,
        // `”Views”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var viewsRoot = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Views");
        // Menyiapkan variabel lokal `backActionLines` untuk nilai back aksi lines dengan mematerialisasi urutan `Directory .EnumerateFiles(viewsRoot,
        // ”*.cshtml”, SearchOption.AllDirectories) .SelectMany(File.ReadLines) .Where(line => line.Contains(”back_to”, StringComparison.Ordinal) || ...`
        // menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var backActionLines = Directory
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateFiles(viewsRoot, ”*.cshtml”, SearchOption.AllDirectories) dalam
            // BackActions_ShouldRenderArrowPrefix; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .EnumerateFiles(viewsRoot, "*.cshtml", SearchOption.AllDirectories)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .SelectMany(File.ReadLines) dalam BackActions_ShouldRenderArrowPrefix; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .SelectMany(File.ReadLines)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(line => line.Contains(”back_to”, StringComparison.Ordinal) ||
            // line.Contains(”nav_back_”, StringComparison.Ordinal)) dalam BackActions_ShouldRenderArrowPrefix; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .Where(line => line.Contains("back_to", StringComparison.Ordinal) || line.Contains("nav_back_", StringComparison.Ordinal))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam BackActions_ShouldRenderArrowPrefix; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`10`, `backActionLines.Count`); pengujian gagal
        // jika keduanya berbeda dalam BackActions_ShouldRenderArrowPrefix.
        Assert.Equal(10, backActionLines.Count);
        // Menjalankan pemeriksaan hasil dengan `Assert.All` menggunakan `backActionLines`, `line => { var expectedPrefix =
        // line.Contains(”privacy.back_to_top”, StringComparison.Ordinal) ? ”@(\”↑ \” + Context.T(” : ”@(\”<- \” + Context.T(”;
        // Assert.Contains(expectedPre...`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam BackActions_ShouldRenderArrowPrefix.
        Assert.All(backActionLines, line =>
        // Membuka scope fungsi lambda yang dipasok ke `Assert.All`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BackActions_ShouldRenderArrowPrefix.
        {
            // Menyiapkan variabel lokal `expectedPrefix` untuk nilai yang diharapkan prefix dengan hasil pemilihan bersyarat: ketika
            // `line.Contains(”privacy.back_to_top”, StringComparison.Ordinal)` benar gunakan `”@(\”↑ \” + Context.T(”`, jika tidak gunakan `”@(\”<- \” +
            // Context.T(”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var expectedPrefix = line.Contains("privacy.back_to_top", StringComparison.Ordinal)
                // Meneruskan fungsi lambda `line => { var expectedPrefix = line.Contains(”privacy.back_to_top”, StringComparison.Ordinal) ? ”@(\”↑ \” + Context.T(”
                // : ”@(\”<- \” + Context.T(”; Assert.Contains(expectedPre...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen
                // ke `Assert.All`.
                ? "@(\"↑ \" + Context.T("
                // Meneruskan fungsi lambda `line => { var expectedPrefix = line.Contains(”privacy.back_to_top”, StringComparison.Ordinal) ? ”@(\”↑ \” + Context.T(”
                // : ”@(\”<- \” + Context.T(”; Assert.Contains(expectedPre...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen
                // ke `Assert.All`.
                : "@(\"<- \" + Context.T(";
            // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `expectedPrefix`, `line`,
            // `StringComparison.Ordinal` dalam BackActions_ShouldRenderArrowPrefix.
            Assert.Contains(expectedPrefix, line, StringComparison.Ordinal);
        // Menutup scope fungsi lambda yang dipasok ke `Assert.All`; bagian berikut berada di luar batas blok tersebut dalam
        // BackActions_ShouldRenderArrowPrefix.
        });
    // Menutup scope metode BackActions_ShouldRenderArrowPrefix; bagian berikut berada di luar batas blok tersebut dalam
    // BackActions_ShouldRenderArrowPrefix.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible` dengan hasil bertipe `void`; operasi ini menangani ui
    // text lexicon entries must be unique complete dan placeholder compatible.
    public void UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible()
    // Membuka scope metode UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; pernyataan/deklarasi berikut berada di dalam batas blok
    // ini dalam UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible.
    {
        // Menyiapkan variabel lokal `repoRoot` untuk nilai repo root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var repoRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `infrastructureRoot` untuk nilai infrastructure root dengan memanggil `Path.Combine` dengan `repoRoot`, `”src”`,
        // `”Cashflowpoly.Ui”`, `”Infrastructure”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var infrastructureRoot = Path.Combine(repoRoot, "src", "Cashflowpoly.Ui", "Infrastructure");
        // Menyiapkan variabel lokal `entries` untuk nilai entries dengan mematerialisasi urutan `Directory .EnumerateFiles(infrastructureRoot,
        // ”UiTextLexicon*.cs”, SearchOption.TopDirectoryOnly) .SelectMany(path => LexiconEntryRegex.Matches(File.ReadAllText(path))) .Selec...` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var entries = Directory
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateFiles(infrastructureRoot, ”UiTextLexicon*.cs”,
            // SearchOption.TopDirectoryOnly) dalam UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .EnumerateFiles(infrastructureRoot, "UiTextLexicon*.cs", SearchOption.TopDirectoryOnly)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .SelectMany(path => LexiconEntryRegex.Matches(File.ReadAllText(path))) dalam
            // UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .SelectMany(path => LexiconEntryRegex.Matches(File.ReadAllText(path)))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(match => new dalam
            // UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(match => new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible.
            {
                // Meneruskan nilai literal `”key”` sebagai argumen ke `Directory .EnumerateFiles(infrastructureRoot, ”UiTextLexicon*.cs”,
                // SearchOption.TopDirectoryOnly) .SelectMany(path => LexiconEntryRegex.Matches(File.ReadAllText(path))) .Selec...`.
                Key = match.Groups["key"].Value,
                // Meneruskan nilai literal `”id”` sebagai argumen ke `Directory .EnumerateFiles(infrastructureRoot, ”UiTextLexicon*.cs”,
                // SearchOption.TopDirectoryOnly) .SelectMany(path => LexiconEntryRegex.Matches(File.ReadAllText(path))) .Selec...`.
                Id = match.Groups["id"].Value,
                // Meneruskan nilai literal `”en”` sebagai argumen ke `Directory .EnumerateFiles(infrastructureRoot, ”UiTextLexicon*.cs”,
                // SearchOption.TopDirectoryOnly) .SelectMany(path => LexiconEntryRegex.Matches(File.ReadAllText(path))) .Selec...`.
                En = match.Groups["en"].Value
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible.
            })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam
            // UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();

        // Menyiapkan variabel lokal `duplicateKeys` untuk nilai duplicate kunci dengan mematerialisasi urutan `entries .GroupBy(entry => entry.Key,
        // StringComparer.OrdinalIgnoreCase) .Where(group => group.Count() > 1) .Select(group => group.Key) .OrderBy(key => key, StringComparer.Ordin...`
        // menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var duplicateKeys = entries
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(entry => entry.Key, StringComparer.OrdinalIgnoreCase) dalam
            // UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(entry => entry.Key, StringComparer.OrdinalIgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(group => group.Count() > 1) dalam
            // UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(group => group.Count() > 1)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(group => group.Key) dalam
            // UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(group => group.Key)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(key => key, StringComparer.OrdinalIgnoreCase) dalam
            // UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderBy(key => key, StringComparer.OrdinalIgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam
            // UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `incompleteKeys` untuk nilai incomplete kunci dengan mematerialisasi urutan `entries .Where(entry =>
        // string.IsNullOrWhiteSpace(entry.Id) || string.IsNullOrWhiteSpace(entry.En)) .Select(entry => entry.Key)` menjadi List; enumerasi dijalankan dan
        // hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var incompleteKeys = entries
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(entry => string.IsNullOrWhiteSpace(entry.Id) ||
            // string.IsNullOrWhiteSpace(entry.En)) dalam UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .Where(entry => string.IsNullOrWhiteSpace(entry.Id) || string.IsNullOrWhiteSpace(entry.En))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(entry => entry.Key) dalam
            // UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(entry => entry.Key)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam
            // UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `placeholderMismatches` untuk nilai placeholder mismatches dengan mematerialisasi urutan `entries .Where(entry =>
        // !PlaceholderRegex.Matches(entry.Id).Select(match => match.Value) .OrderBy(value => value, StringComparer.Ordinal) .SequenceEqual(
        // PlaceholderRegex.Matc...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var placeholderMismatches = entries
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(entry => !PlaceholderRegex.Matches(entry.Id).Select(match =>
            // match.Value) dalam UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; token pada baris ini menyambungkan bagian kode sebelum dan
            // sesudahnya.
            .Where(entry => !PlaceholderRegex.Matches(entry.Id).Select(match => match.Value)
                // Meneruskan fungsi lambda `value => value` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                // `PlaceholderRegex.Matches(entry.Id).Select(match => match.Value) .OrderBy`; Meneruskan `StringComparer.Ordinal` (nilai ordinal) sebagai argumen
                // ke `PlaceholderRegex.Matches(entry.Id).Select(match => match.Value) .OrderBy`.
                .OrderBy(value => value, StringComparer.Ordinal)
                // Meneruskan fungsi lambda `entry => !PlaceholderRegex.Matches(entry.Id).Select(match => match.Value) .OrderBy(value => value,
                // StringComparer.Ordinal) .SequenceEqual( PlaceholderRegex.Matches(entry.En).S...` yang dijalankan oleh operasi pemanggil untuk memproses setiap
                // masukan sebagai argumen ke `entries .Where`.
                .SequenceEqual(
                    // Meneruskan mengurutkan `PlaceholderRegex.Matches(entry.En).Select(match => match.Value)` secara menaik berdasarkan `value => value`,
                    // `StringComparer.Ordinal` sebagai argumen ke `PlaceholderRegex.Matches(entry.Id).Select(match => match.Value) .OrderBy(value => value,
                    // StringComparer.Ordinal) .SequenceEqual`; Meneruskan `entry.En` (nilai en) sebagai argumen ke `PlaceholderRegex.Matches`; Meneruskan fungsi lambda
                    // `match => match.Value` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                    // `PlaceholderRegex.Matches(entry.En).Select`.
                    PlaceholderRegex.Matches(entry.En).Select(match => match.Value)
                        // Meneruskan fungsi lambda `value => value` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                        // `PlaceholderRegex.Matches(entry.En).Select(match => match.Value) .OrderBy`; Meneruskan `StringComparer.Ordinal` (nilai ordinal) sebagai argumen
                        // ke `PlaceholderRegex.Matches(entry.En).Select(match => match.Value) .OrderBy`.
                        .OrderBy(value => value, StringComparer.Ordinal),
                    // Meneruskan `StringComparer.Ordinal` (nilai ordinal) sebagai argumen ke `PlaceholderRegex.Matches(entry.Id).Select(match => match.Value)
                    // .OrderBy(value => value, StringComparer.Ordinal) .SequenceEqual`.
                    StringComparer.Ordinal))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(entry => entry.Key) dalam
            // UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(entry => entry.Key)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam
            // UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();

        // Menjalankan pemeriksaan bahwa `duplicateKeys.Count == 0`, `$”Key terjemahan ganda: {string.Join(”, ”, duplicateKeys)}”` bernilai benar; pengujian
        // gagal jika kondisi tidak terpenuhi dalam UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible.
        Assert.True(duplicateKeys.Count == 0, $"Key terjemahan ganda: {string.Join(", ", duplicateKeys)}");
        // Menjalankan pemeriksaan bahwa `incompleteKeys.Count == 0`, `$”Terjemahan kosong: {string.Join(”, ”, incompleteKeys)}”` bernilai benar; pengujian
        // gagal jika kondisi tidak terpenuhi dalam UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible.
        Assert.True(incompleteKeys.Count == 0, $"Terjemahan kosong: {string.Join(", ", incompleteKeys)}");
        // Menjalankan pemeriksaan bahwa `placeholderMismatches.Count == 0`, `$”Placeholder ID/EN tidak sama: {string.Join(”, ”, placeholderMismatches)}”`
        // bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible.
        Assert.True(placeholderMismatches.Count == 0, $"Placeholder ID/EN tidak sama: {string.Join(", ", placeholderMismatches)}");
    // Menutup scope metode UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible; bagian berikut berada di luar batas blok tersebut dalam
    // UiTextLexicon_EntriesMustBeUniqueCompleteAndPlaceholderCompatible.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName` dengan hasil bertipe `void`; operasi ini menangani scoring terms
    // use kebahagiaan poin tanpa duplicating the nama.
    public void ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName()
    // Membuka scope metode ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName.
    {
        // Menyiapkan variabel lokal `infrastructureRoot` untuk nilai infrastructure root dengan memanggil `Path.Combine` dengan `ResolveRepositoryRoot()`,
        // `”src”`, `”Cashflowpoly.Ui”`, `”Infrastructure”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var infrastructureRoot = Path.Combine(ResolveRepositoryRoot(), "src", "Cashflowpoly.Ui", "Infrastructure");
        // Menyiapkan variabel lokal `entries` untuk nilai entries dengan meratakan hasil koleksi bertingkat dari `Directory
        // .EnumerateFiles(infrastructureRoot, ”UiTextLexicon*.cs”, SearchOption.TopDirectoryOnly)` melalui `path =>
        // LexiconEntryRegex.Matches(File.ReadAllText(path))` menjadi satu urutan. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var entries = Directory
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateFiles(infrastructureRoot, ”UiTextLexicon*.cs”,
            // SearchOption.TopDirectoryOnly) dalam ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .EnumerateFiles(infrastructureRoot, "UiTextLexicon*.cs", SearchOption.TopDirectoryOnly)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .SelectMany(path => LexiconEntryRegex.Matches(File.ReadAllText(path))); dalam
            // ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .SelectMany(path => LexiconEntryRegex.Matches(File.ReadAllText(path)));

        // Mengulangi setiap elemen `entries`; elemen saat ini disimpan sebagai `entry` bertipe `var` untuk diproses oleh badan loop dalam
        // ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName.
        foreach (var entry in entries)
        // Membuka scope loop setiap entry dari `entries`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName.
        {
            // Menyiapkan variabel lokal `id` untuk nilai identitas dengan `entry.Groups[”id”].Value`, yaitu nilai yang dibungkus objek/nullable. Tipe variabel
            // disimpulkan dari ekspresi nilai awal.
            var id = entry.Groups["id"].Value;
            // Menyiapkan variabel lokal `en` untuk nilai en dengan `entry.Groups[”en”].Value`, yaitu nilai yang dibungkus objek/nullable. Tipe variabel
            // disimpulkan dari ekspresi nilai awal.
            var en = entry.Groups["en"].Value;
            // Menjalankan pemeriksaan hasil dengan `Assert.DoesNotMatch` menggunakan `@”(?i)\bpoin\b(?!\s+kebahagiaan\b)”`, `id`; ketidaksesuaian dengan
            // ekspektasi membuat pengujian gagal dalam ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName.
            Assert.DoesNotMatch(@"(?i)\bpoin\b(?!\s+kebahagiaan\b)", id);
            // Menjalankan pemeriksaan hasil dengan `Assert.DoesNotMatch` menggunakan `@”(?i)\bpoin\s+kebahagiaan\s+kebahagiaan\b”`, `id`; ketidaksesuaian
            // dengan ekspektasi membuat pengujian gagal dalam ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName.
            Assert.DoesNotMatch(@"(?i)\bpoin\s+kebahagiaan\s+kebahagiaan\b", id);
            // Menjalankan pemeriksaan hasil dengan `Assert.DoesNotMatch` menggunakan `@”(?i)\bhappiness\s+happiness\s+points?\b”`, `en`; ketidaksesuaian dengan
            // ekspektasi membuat pengujian gagal dalam ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName.
            Assert.DoesNotMatch(@"(?i)\bhappiness\s+happiness\s+points?\b", en);
            // Memeriksa memeriksa apakah `id` memuat `”poin”`, `StringComparison.OrdinalIgnoreCase`; blok if hanya dijalankan ketika kondisi ini bernilai benar
            // dalam ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName.
            if (id.Contains("poin", StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `id.Contains(”poin”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName.
            {
                // Menjalankan pemeriksaan hasil dengan `Assert.DoesNotMatch` menggunakan `@”(?i)(?<!happiness )\bpoints?\b”`, `en`; ketidaksesuaian dengan
                // ekspektasi membuat pengujian gagal dalam ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName.
                Assert.DoesNotMatch(@"(?i)(?<!happiness )\bpoints?\b", en);
            // Menutup scope cabang if untuk kondisi `id.Contains(”poin”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok
            // tersebut dalam ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName.
            }
        // Menutup scope loop setiap entry dari `entries`; bagian berikut berada di luar batas blok tersebut dalam
        // ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName.
        }
    // Menutup scope metode ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName; bagian berikut berada di luar batas blok tersebut dalam
    // ScoringTerms_UseHappinessPointsWithoutDuplicatingTheName.
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
// Menutup scope tipe UiTextLexiconStructureTests; bagian berikut berada di luar batas blok tersebut.
}
