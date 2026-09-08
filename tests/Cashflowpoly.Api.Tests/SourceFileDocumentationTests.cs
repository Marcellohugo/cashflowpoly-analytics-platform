// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui SourceFileDocumentationTests.
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `SourceFileDocumentationTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SourceFileDocumentationTests
// Membuka scope tipe SourceFileDocumentationTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `HashSet<string>`: `CommentableExtensions` menyimpan nilai commentable extensions dengan nilai awal objek baru
    // dengan tipe mengikuti konteks tujuan dan argumen (StringComparer.OrdinalIgnoreCase). readonly membatasi penggantian referensi/nilai field pada
    // deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly HashSet<string> CommentableExtensions = new(StringComparer.OrdinalIgnoreCase)
    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Menggunakan nilai literal `”.cs”` sebagai bagian ekspresi yang sedang disusun.
        ".cs", ".cshtml", ".js", ".css", ".sql", ".yml", ".yaml", ".ps1", ".csproj", ".conf", ".example", ".http"
    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut.
    };

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `CommentableSourceFiles_MustStartWithFunctionDescription` dengan hasil bertipe `void`; operasi ini menangani commentable
    // source files must start dengan function description.
    public void CommentableSourceFiles_MustStartWithFunctionDescription()
    // Membuka scope metode CommentableSourceFiles_MustStartWithFunctionDescription; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // CommentableSourceFiles_MustStartWithFunctionDescription.
    {
        // Menyiapkan variabel lokal `repositoryRoot` untuk nilai repositori root dengan memanggil `ResolveRepositoryRoot` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var repositoryRoot = ResolveRepositoryRoot();
        // Menyiapkan variabel lokal `roots` untuk nilai roots dengan menyaring elemen `new[] { ”src”, ”tests”, ”database”, ”infra”, ”scripts”, ”config” }
        // .Select(name => Path.Combine(repositoryRoot, name))` dengan predikat `Directory.Exists`; hanya elemen yang memenuhi kondisi diteruskan. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var roots = new[] { "src", "tests", "database", "infra", "scripts", "config" }
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(name => Path.Combine(repositoryRoot, name)) dalam
            // CommentableSourceFiles_MustStartWithFunctionDescription; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(name => Path.Combine(repositoryRoot, name))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(Directory.Exists); dalam
            // CommentableSourceFiles_MustStartWithFunctionDescription; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(Directory.Exists);
        // Menyiapkan variabel lokal `missing` untuk nilai missing dengan mematerialisasi urutan `roots .SelectMany(root => Directory.EnumerateFiles(root,
        // ”*”, SearchOption.AllDirectories)) .Where(IsCommentableSource) .Where(path => File.ReadLines(path).FirstOrDefault() is...` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var missing = roots
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .SelectMany(root => Directory.EnumerateFiles(root, ”*”,
            // SearchOption.AllDirectories)) dalam CommentableSourceFiles_MustStartWithFunctionDescription; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .SelectMany(root => Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(IsCommentableSource) dalam
            // CommentableSourceFiles_MustStartWithFunctionDescription; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(IsCommentableSource)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(path => File.ReadLines(path).FirstOrDefault() is not { } firstLine ||
            // dalam CommentableSourceFiles_MustStartWithFunctionDescription; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(path => File.ReadLines(path).FirstOrDefault() is not { } firstLine ||
                           // Meneruskan nilai literal `”Fungsi file:”` sebagai argumen ke `firstLine.Contains`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal
                           // ignore case) sebagai argumen ke `firstLine.Contains`.
                           !firstLine.Contains("Fungsi file:", StringComparison.OrdinalIgnoreCase))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(path => Path.GetRelativePath(repositoryRoot, path).Replace('\\', '/'))
            // dalam CommentableSourceFiles_MustStartWithFunctionDescription; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(path => Path.GetRelativePath(repositoryRoot, path).Replace('\\', '/'))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderBy(path => path, StringComparer.OrdinalIgnoreCase) dalam
            // CommentableSourceFiles_MustStartWithFunctionDescription; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam CommentableSourceFiles_MustStartWithFunctionDescription; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToList();

        // Menjalankan pemeriksaan bahwa `missing.Count == 0`, `$”Source tanpa komentar fungsi: {string.Join(”, ”, missing)}”` bernilai benar; pengujian
        // gagal jika kondisi tidak terpenuhi dalam CommentableSourceFiles_MustStartWithFunctionDescription.
        Assert.True(missing.Count == 0, $"Source tanpa komentar fungsi: {string.Join(", ", missing)}");
    // Menutup scope metode CommentableSourceFiles_MustStartWithFunctionDescription; bagian berikut berada di luar batas blok tersebut dalam
    // CommentableSourceFiles_MustStartWithFunctionDescription.
    }

    // Mendefinisikan metode `IsCommentableSource` dengan hasil bertipe `bool`; operasi ini menangani berstatus commentable source. Masukan: Parameter
    // `path` bertipe `string` membawa nilai path.
    private static bool IsCommentableSource(string path)
    // Membuka scope metode IsCommentableSource; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IsCommentableSource.
    {
        // Menyiapkan variabel lokal `normalized` untuk nilai normalized dengan memanggil `path.Replace` dengan `'\\'`, `'/'`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var normalized = path.Replace('\\', '/');
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `normalized.Contains(”/bin/”, StringComparison.OrdinalIgnoreCase) ||
        // normalized.Contains(”/obj/”, StringComparison.OrdinalIgnoreCase) || normalized.Contains(”/node_modules/”, S...` dan
        // `normalized.EndsWith(”/wwwroot/css/tailwind.css”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if
        // hanya dijalankan ketika kondisi ini bernilai benar dalam IsCommentableSource.
        if (normalized.Contains("/bin/", StringComparison.OrdinalIgnoreCase) ||
            // Melanjutkan pengolahan dengan memeriksa apakah `normalized` memuat `”/obj/”`, `StringComparison.OrdinalIgnoreCase` dalam IsCommentableSource.
            normalized.Contains("/obj/", StringComparison.OrdinalIgnoreCase) ||
            // Melanjutkan pengolahan dengan memeriksa apakah `normalized` memuat `”/node_modules/”`, `StringComparison.OrdinalIgnoreCase` dalam
            // IsCommentableSource.
            normalized.Contains("/node_modules/", StringComparison.OrdinalIgnoreCase) ||
            // Melanjutkan pengolahan dengan memeriksa apakah `normalized` memuat `”/wwwroot/lib/”`, `StringComparison.OrdinalIgnoreCase` dalam
            // IsCommentableSource.
            normalized.Contains("/wwwroot/lib/", StringComparison.OrdinalIgnoreCase) ||
            // Melanjutkan pengolahan dengan memanggil `normalized.EndsWith` dengan `”/wwwroot/css/tailwind.css”`, `StringComparison.OrdinalIgnoreCase` dalam
            // IsCommentableSource.
            normalized.EndsWith("/wwwroot/css/tailwind.css", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `normalized.Contains(”/bin/”, StringComparison.OrdinalIgnoreCase) || normalized.Contains(”/obj/”,
        // StringComparison.OrdinalIgnoreCase) || normalized.Contains(”/node_modules/”, S...`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam IsCommentableSource.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam IsCommentableSource; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `normalized.Contains(”/bin/”, StringComparison.OrdinalIgnoreCase) || normalized.Contains(”/obj/”,
        // StringComparison.OrdinalIgnoreCase) || normalized.Contains(”/node_modules/”, S...`; bagian berikut berada di luar batas blok tersebut dalam
        // IsCommentableSource.
        }

        // Mengembalikan gabungan syarat OR: setidaknya satu kondisi wajib benar antara `CommentableExtensions.Contains(Path.GetExtension(path))` dan
        // `string.Equals(Path.GetFileName(path), ”Dockerfile”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri salah kepada
        // pemanggil dalam IsCommentableSource; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return CommentableExtensions.Contains(Path.GetExtension(path)) ||
               // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `Path.GetFileName(path)`, `”Dockerfile”`,
               // `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan comparer yang diberikan dalam IsCommentableSource.
               string.Equals(Path.GetFileName(path), "Dockerfile", StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode IsCommentableSource; bagian berikut berada di luar batas blok tersebut dalam IsCommentableSource.
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
// Menutup scope tipe SourceFileDocumentationTests; bagian berikut berada di luar batas blok tersebut.
}
