// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui ApiErrorLocalizationCoverageTests.
// Mengimpor namespace `System.Text.RegularExpressions` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.RegularExpressions;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `ApiErrorLocalizationCoverageTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class ApiErrorLocalizationCoverageTests
// Membuka scope tipe ApiErrorLocalizationCoverageTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `Regex`: `TranslationKeyRegex` menyimpan nilai translation kunci regex dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (@”\[””(?<message>[^””\r\n]+)””\]\s*=”, RegexOptions.Compiled). readonly membatasi penggantian
    // referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly Regex TranslationKeyRegex = new(@"\[""(?<message>[^""\r\n]+)""\]\s*=", RegexOptions.Compiled);
    // Mendeklarasikan field bertipe `Regex`: `DirectBuildErrorRegex` menyimpan nilai direct build kesalahan regex dengan nilai awal objek baru dengan
    // tipe mengikuti konteks tujuan dan argumen ( @”ApiErrorHelper\.BuildError\s*\(\s*[^,]+,\s*””[^””\r\n]+””\s*,\s*””(?<message>[^””\r\n]+)”,
    // RegexOptions.Compiled | RegexOptions.Singleline). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static
    // membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly Regex DirectBuildErrorRegex = new(
        // Meneruskan nilai literal `@”ApiErrorHelper\.BuildError\s*\(\s*[^,]+,\s*””[^””\r\n]+””\s*,\s*””(?<message>[^””\r\n]+)”` sebagai argumen ke
        // konstruktor dengan tipe mengikuti konteks.
        @"ApiErrorHelper\.BuildError\s*\(\s*[^,]+,\s*""[^""\r\n]+""\s*,\s*""(?<message>[^""\r\n]+)",
        // Meneruskan penggabungan flag atau operasi OR bit antara `RegexOptions.Compiled` dan `RegexOptions.Singleline` sebagai argumen ke konstruktor
        // dengan tipe mengikuti konteks.
        RegexOptions.Compiled | RegexOptions.Singleline);
    // Mendeklarasikan field bertipe `Regex`: `ServiceBuildErrorRegex` menyimpan nilai layanan build kesalahan regex dengan nilai awal objek baru dengan
    // tipe mengikuti konteks tujuan dan argumen ( @”(?<!ApiErrorHelper\.)\bBuildError\s*\(\s*””[^””\r\n]+””\s*,\s*””(?<message>[^””\r\n]+)”,
    // RegexOptions.Compiled | RegexOptions.Singleline). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static
    // membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly Regex ServiceBuildErrorRegex = new(
        // Meneruskan nilai literal `@”(?<!ApiErrorHelper\.)\bBuildError\s*\(\s*””[^””\r\n]+””\s*,\s*””(?<message>[^””\r\n]+)”` sebagai argumen ke
        // konstruktor dengan tipe mengikuti konteks.
        @"(?<!ApiErrorHelper\.)\bBuildError\s*\(\s*""[^""\r\n]+""\s*,\s*""(?<message>[^""\r\n]+)",
        // Meneruskan penggabungan flag atau operasi OR bit antara `RegexOptions.Compiled` dan `RegexOptions.Singleline` sebagai argumen ke konstruktor
        // dengan tipe mengikuti konteks.
        RegexOptions.Compiled | RegexOptions.Singleline);

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `LiteralApiErrors_MustHaveEnglishTranslations` dengan hasil bertipe `void`; operasi ini menangani literal api kesalahan
    // must have english translations.
    public void LiteralApiErrors_MustHaveEnglishTranslations()
    // Membuka scope metode LiteralApiErrors_MustHaveEnglishTranslations; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // LiteralApiErrors_MustHaveEnglishTranslations.
    {
        // Menyiapkan variabel lokal `repositoryRoot` untuk nilai repositori root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var repositoryRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `apiRoot` untuk nilai api root dengan memanggil `Path.Combine` dengan `repositoryRoot`, `”src”`, `”Cashflowpoly.Api”`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var apiRoot = Path.Combine(repositoryRoot, "src", "Cashflowpoly.Api");
        // Menyiapkan variabel lokal `helperPath` untuk nilai helper path dengan memanggil `Path.Combine` dengan `apiRoot`, `”Infrastructure”`,
        // `”ApiErrorHelper.cs”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var helperPath = Path.Combine(apiRoot, "Infrastructure", "ApiErrorHelper.cs");
        // Menyiapkan variabel lokal `translatedMessages` untuk nilai translated pesan dengan membentuk himpunan nilai unik dari `TranslationKeyRegex
        // .Matches(File.ReadAllText(helperPath)) .Select(match => match.Groups[”message”].Value)` memakai `StringComparer.Ordinal`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var translatedMessages = TranslationKeyRegex
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Matches(File.ReadAllText(helperPath)) dalam
            // LiteralApiErrors_MustHaveEnglishTranslations; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Matches(File.ReadAllText(helperPath))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(match => match.Groups[”message”].Value) dalam
            // LiteralApiErrors_MustHaveEnglishTranslations; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(match => match.Groups["message"].Value)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToHashSet(StringComparer.Ordinal); dalam
            // LiteralApiErrors_MustHaveEnglishTranslations; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToHashSet(StringComparer.Ordinal);

        // Menyiapkan variabel lokal `literalMessages` untuk nilai literal pesan dengan mematerialisasi urutan `Directory .EnumerateFiles(apiRoot, ”*.cs”,
        // SearchOption.AllDirectories) .SelectMany(path => { var content = File.ReadAllText(path); return DirectBuildErrorRegex.Matches(conten...` menjadi
        // List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var literalMessages = Directory
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .EnumerateFiles(apiRoot, ”*.cs”, SearchOption.AllDirectories) dalam
            // LiteralApiErrors_MustHaveEnglishTranslations; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .EnumerateFiles(apiRoot, "*.cs", SearchOption.AllDirectories)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .SelectMany(path => dalam LiteralApiErrors_MustHaveEnglishTranslations; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .SelectMany(path =>
            // Membuka scope fungsi lambda yang dipasok ke `Directory .EnumerateFiles(apiRoot, ”*.cs”, SearchOption.AllDirectories) .SelectMany`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam LiteralApiErrors_MustHaveEnglishTranslations.
            {
                // Menyiapkan variabel lokal `content` untuk nilai content dengan memanggil `File.ReadAllText` dengan `path`. Tipe variabel disimpulkan dari
                // ekspresi nilai awal.
                var content = File.ReadAllText(path);
                // Mengembalikan memetakan setiap elemen `DirectBuildErrorRegex.Matches(content) .Concat(ServiceBuildErrorRegex.Matches(content))` melalui `match =>
                // match.Groups[”message”].Value` menjadi bentuk hasil yang dibutuhkan kepada pemanggil dalam LiteralApiErrors_MustHaveEnglishTranslations; eksekusi
                // jalur ini selesai setelah nilai hasil ditentukan.
                return DirectBuildErrorRegex.Matches(content)
                    // Meneruskan memanggil `ServiceBuildErrorRegex.Matches` dengan `content` sebagai argumen ke `DirectBuildErrorRegex.Matches(content) .Concat`;
                    // Meneruskan `content` (nilai content) sebagai argumen ke `ServiceBuildErrorRegex.Matches`.
                    .Concat(ServiceBuildErrorRegex.Matches(content))
                    // Meneruskan fungsi lambda `match => match.Groups[”message”].Value` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                    // argumen ke `DirectBuildErrorRegex.Matches(content) .Concat(ServiceBuildErrorRegex.Matches(content)) .Select`; Meneruskan nilai literal
                    // `”message”` sebagai argumen ke `DirectBuildErrorRegex.Matches(content) .Concat(ServiceBuildErrorRegex.Matches(content)) .Select`.
                    .Select(match => match.Groups["message"].Value);
            // Menutup scope fungsi lambda yang dipasok ke `Directory .EnumerateFiles(apiRoot, ”*.cs”, SearchOption.AllDirectories) .SelectMany`; bagian berikut
            // berada di luar batas blok tersebut dalam LiteralApiErrors_MustHaveEnglishTranslations.
            })
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Distinct(StringComparer.Ordinal) dalam
            // LiteralApiErrors_MustHaveEnglishTranslations; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Distinct(StringComparer.Ordinal)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam LiteralApiErrors_MustHaveEnglishTranslations; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();

        // Menyiapkan variabel lokal `missing` untuk nilai missing dengan mematerialisasi urutan `literalMessages .Where(message =>
        // !translatedMessages.Contains(message)) .OrderBy(message => message, StringComparer.Ordinal)` menjadi List; enumerasi dijalankan dan hasilnya
        // disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var missing = literalMessages
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(message => !translatedMessages.Contains(message)) dalam
            // LiteralApiErrors_MustHaveEnglishTranslations; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(message => !translatedMessages.Contains(message))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(message => message, StringComparer.Ordinal) dalam
            // LiteralApiErrors_MustHaveEnglishTranslations; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderBy(message => message, StringComparer.Ordinal)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam LiteralApiErrors_MustHaveEnglishTranslations; token pada baris
            // ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();

        // Menjalankan pemeriksaan bahwa `missing.Count == 0`, `$”Pesan API tanpa terjemahan EN: {string.Join(” | ”, missing)}”` bernilai benar; pengujian
        // gagal jika kondisi tidak terpenuhi dalam LiteralApiErrors_MustHaveEnglishTranslations.
        Assert.True(missing.Count == 0, $"Pesan API tanpa terjemahan EN: {string.Join(" | ", missing)}");
    // Menutup scope metode LiteralApiErrors_MustHaveEnglishTranslations; bagian berikut berada di luar batas blok tersebut dalam
    // LiteralApiErrors_MustHaveEnglishTranslations.
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

        // Menghentikan alur dengan melempar objek baru bertipe `DirectoryNotFoundException` dengan argumen (”Root repositori tidak ditemukan.”) dalam
        // ResolveRepositoryRoot; pemanggil atau middleware penanganan error menerima kegagalan ini.
        throw new DirectoryNotFoundException("Root repositori tidak ditemukan.");
    // Menutup scope metode ResolveRepositoryRoot; bagian berikut berada di luar batas blok tersebut dalam ResolveRepositoryRoot.
    }
// Menutup scope tipe ApiErrorLocalizationCoverageTests; bagian berikut berada di luar batas blok tersebut.
}
