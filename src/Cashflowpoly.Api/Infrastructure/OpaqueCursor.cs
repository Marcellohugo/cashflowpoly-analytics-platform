// Fungsi file: Mengodekan dan memvalidasi cursor pagination API yang tidak membuka struktur internal.
// Mengimpor namespace `System.Globalization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Globalization;
// Mengimpor namespace `System.Text` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Infrastructure;

internal static class OpaqueCursor
{
    public static string EncodeEvent(long sequenceNumber) => Encode(sequenceNumber.ToString(CultureInfo.InvariantCulture));

    public static bool TryDecodeEvent(string? cursor, out long sequenceNumber)
    {
        sequenceNumber = -1;
        return string.IsNullOrWhiteSpace(cursor) ||
               (TryDecode(cursor, out var value) &&
                long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out sequenceNumber) &&
                sequenceNumber >= 0);
    }

    public static string EncodeTransaction(DateTimeOffset timestamp, Guid transactionId) =>
        Encode($"{timestamp.UtcTicks.ToString(CultureInfo.InvariantCulture)}|{transactionId:N}");

    public static bool TryDecodeTransaction(string? cursor, out DateTimeOffset timestamp, out Guid transactionId)
    {
        timestamp = default;
        transactionId = default;
        if (string.IsNullOrWhiteSpace(cursor))
        {
            return true;
        }

        if (!TryDecode(cursor, out var value))
        {
            return false;
        }

        var parts = value.Split('|');
        if (parts.Length != 2 ||
            !long.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var ticks) ||
            ticks < DateTimeOffset.MinValue.UtcTicks || ticks > DateTimeOffset.MaxValue.UtcTicks ||
            !Guid.TryParseExact(parts[1], "N", out transactionId))
        {
            return false;
        }

        timestamp = new DateTimeOffset(ticks, TimeSpan.Zero);
        return true;
    }

    private static string Encode(string value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value))
        .TrimEnd('=')
        .Replace('+', '-')
        .Replace('/', '_');

    private static bool TryDecode(string value, out string decoded)
    {
        decoded = string.Empty;
        if (value.Length is 0 or > 256 || value.Any(ch => !char.IsAsciiLetterOrDigit(ch) && ch is not '-' and not '_'))
        {
            return false;
        }

        try
        {
            var padded = value.Replace('-', '+').Replace('_', '/');
            padded += new string('=', (4 - padded.Length % 4) % 4);
            decoded = Encoding.UTF8.GetString(Convert.FromBase64String(padded));
            return true;
        }
        // Menangani exception `FormatException` melalui variabel dalam TryDecode.
        catch (FormatException)
        {
            return false;
        }
    }
}
