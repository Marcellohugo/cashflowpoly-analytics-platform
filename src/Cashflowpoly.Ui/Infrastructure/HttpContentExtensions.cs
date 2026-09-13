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
    {
        try
        {
            return await content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadFromJsonAsync.
        catch (JsonException)
        {
            return default;
        }
        // Menangani exception `NotSupportedException` melalui variabel dalam TryReadFromJsonAsync.
        catch (NotSupportedException)
        {
            return default;
        }
    }
}
