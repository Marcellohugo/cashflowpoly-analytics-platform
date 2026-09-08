// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui HttpContentExtensions.
// Mengimpor namespace `System.Net.Http.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Net.Http.Json;
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Extension method untuk <see cref="HttpContent"/> yang menyediakan deserialisasi JSON secara aman
/// dengan penanganan otomatis terhadap <see cref="JsonException"/> dan <see cref="NotSupportedException"/>.
/// </summary>
// Mendefinisikan tipe class `HttpContentExtensions`.
internal static class HttpContentExtensions
// Membuka scope tipe HttpContentExtensions; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Membaca dan mendeserialisasi konten HTTP ke tipe <typeparamref name="T"/> secara aman.
    /// Mengembalikan <c>default</c> jika terjadi kesalahan deserialisasi atau content-type tidak didukung.
    /// </summary>
    /// <typeparam name="T">Tipe target deserialisasi.</typeparam>
    /// <param name="content">Konten HTTP yang akan dideserialisasi.</param>
    /// <param name="cancellationToken">Token pembatalan untuk membatalkan operasi.</param>
    /// <returns>Objek hasil deserialisasi, atau <c>default</c> jika gagal.</returns>
    // Mendefinisikan metode `TryReadFromJsonAsync` dengan hasil bertipe `Task<T?>`. Membaca dan mendeserialisasi konten HTTP ke tipe secara aman.
    // Mengembalikan default jika terjadi kesalahan deserialisasi atau content-type tidak didukung. async memungkinkan metode menunggu operasi I/O
    // dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `content` bertipe `HttpContent` membawa nilai content; Parameter
    // `cancellationToken` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan
    // atau aplikasi berhenti; bila argumen tidak diberikan digunakan nilai literal `default`.
    internal static async Task<T?> TryReadFromJsonAsync<T>(this HttpContent content, CancellationToken cancellationToken = default)
    // Membuka scope metode TryReadFromJsonAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadFromJsonAsync.
    {
        // Memulai blok try dalam TryReadFromJsonAsync; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadFromJsonAsync.
        {
            // Mengembalikan hasil operasi asinkron membaca `cancellationToken` menjadi objek bertipe sesuai kontrak JSON melalui
            // `content.ReadFromJsonAsync<T>`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai kepada pemanggil dalam
            // TryReadFromJsonAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return await content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadFromJsonAsync.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadFromJsonAsync.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadFromJsonAsync.
        {
            // Mengembalikan nilai literal `default` kepada pemanggil dalam TryReadFromJsonAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return default;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadFromJsonAsync.
        }
        // Menangani exception `NotSupportedException` melalui variabel dalam TryReadFromJsonAsync.
        catch (NotSupportedException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadFromJsonAsync.
        {
            // Mengembalikan nilai literal `default` kepada pemanggil dalam TryReadFromJsonAsync; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return default;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadFromJsonAsync.
        }
    // Menutup scope metode TryReadFromJsonAsync; bagian berikut berada di luar batas blok tersebut dalam TryReadFromJsonAsync.
    }
// Menutup scope tipe HttpContentExtensions; bagian berikut berada di luar batas blok tersebut.
}
