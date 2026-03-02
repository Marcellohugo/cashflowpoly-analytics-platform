// Fungsi file: Menyediakan extension method aman untuk deserialisasi JSON dari HttpContent, menangkap JsonException dan NotSupportedException.
using System.Net.Http.Json;
using System.Text.Json;

namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Extension method untuk <see cref="HttpContent"/> yang menyediakan deserialisasi JSON secara aman
/// dengan penanganan otomatis terhadap <see cref="JsonException"/> dan <see cref="NotSupportedException"/>.
/// </summary>
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
    internal static async Task<T?> TryReadFromJsonAsync<T>(this HttpContent content, CancellationToken cancellationToken = default)
    {
        try
        {
            return await content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }
        catch (JsonException)
        {
            return default;
        }
        catch (NotSupportedException)
        {
            return default;
        }
    }
}
