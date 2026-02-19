// Fungsi file: Mengelola endpoint API untuk domain ApiErrorHelper termasuk validasi request dan respons standar.
using Cashflowpoly.Api.Models;

namespace Cashflowpoly.Api.Controllers;

/// <summary>
/// Menyatakan peran utama tipe ApiErrorHelper pada modul ini.
/// </summary>
internal static class ApiErrorHelper
{
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    private static readonly Dictionary<string, string> IdToEnMessages = new(StringComparer.Ordinal)
    {
        ["Action type wajib diisi"] = "Action type is required",
        ["Actor type tidak valid"] = "Invalid actor type",
        ["Akun PLAYER belum terhubung ke profil pemain"] = "This PLAYER account is not linked to a player profile",
        ["Amount harus > 0"] = "Amount must be greater than 0",
        ["Amount kerja lepas tidak sesuai ruleset"] = "Freelance amount does not match the ruleset",
        ["Amount tidak sesuai unit_price * qty"] = "Amount does not match unit_price * qty",
        ["Asuransi hanya berlaku untuk risiko OUT"] = "Insurance can only be used for OUT risk events",
        ["Bahan tidak mencukupi untuk klaim order"] = "Insufficient ingredients to claim the order",
        ["Card ID wajib diisi"] = "Card ID is required",
        ["Category wajib diisi"] = "Category is required",
        ["Config wajib ada"] = "Config is required",
        ["Cost tidak valid"] = "Invalid cost",
        ["Counterparty tidak valid"] = "Invalid counterparty",
        ["Daftar event batch wajib diisi"] = "Event batch list is required",
        ["Day index minimal 0"] = "Day index must be at least 0",
        ["Direction tidak valid"] = "Invalid direction",
        ["Display name maksimal 80 karakter"] = "Display name must be at most 80 characters",
        ["Emergency option hanya berlaku untuk risiko OUT"] = "Emergency option can only be used for OUT risk events",
        ["Event sudah ada"] = "Event already exists",
        ["Field wajib tidak lengkap"] = "Required fields are incomplete",
        ["Fitur asuransi tidak aktif"] = "Insurance feature is not enabled",
        ["Fitur donasi Jumat tidak aktif"] = "Friday donation feature is not enabled",
        ["Fitur perdagangan emas tidak aktif"] = "Gold trading feature is not enabled",
        ["Fitur pinjaman tidak aktif"] = "Loan feature is not enabled",
        ["Fitur risiko hanya tersedia di mode MAHIR"] = "Risk feature is only available in ADVANCED mode",
        ["Fitur tabungan tujuan tidak aktif"] = "Saving-goal feature is not enabled",
        ["fromSeq tidak boleh negatif"] = "fromSeq cannot be negative",
        ["Gagal sinkronisasi profil player"] = "Failed to synchronize player profile",
        ["Goal ID wajib diisi"] = "Goal ID is required",
        ["Income harus > 0"] = "Income must be greater than 0",
        ["Jumlah discard melebihi stok bahan"] = "Discard quantity exceeds available ingredient stock",
        ["Jumlah donasi di luar batas"] = "Donation amount is outside the allowed range",
        ["Jumlah kartu bahan sejenis melebihi batas ruleset"] = "Quantity of same ingredient cards exceeds ruleset limit",
        ["Jumlah token aksi melebihi batas ruleset"] = "Action token usage exceeds ruleset limit",
        ["Kebutuhan primer harus dibeli terlebih dahulu"] = "Primary needs must be purchased first",
        ["Kepemilikan emas tidak mencukupi"] = "Insufficient gold ownership",
        ["Konfigurasi ruleset tidak valid"] = "Invalid ruleset configuration",
        ["limit harus antara 1 sampai 1000"] = "limit must be between 1 and 1000",
        ["Loan ID sudah dipakai"] = "Loan ID is already in use",
        ["Loan ID tidak ditemukan"] = "Loan ID not found",
        ["Maksimal tabungan per aksi adalah 15 koin"] = "Maximum saving per action is 15 coins",
        ["Misi sudah ditetapkan untuk pemain"] = "Mission has already been assigned to this player",
        ["Mission ID dan target wajib diisi"] = "Mission ID and target are required",
        ["Mode tidak valid"] = "Invalid mode",
        ["Nama pemain wajib diisi"] = "Player name is required",
        ["Nilai pinjaman harus 10 koin"] = "Loan value must be 10 coins",
        ["Nilai pinjaman tidak valid"] = "Invalid loan value",
        ["Nilai qty/unit_price/amount tidak valid"] = "Invalid qty/unit_price/amount values",
        ["Nilai rank/points tidak valid"] = "Invalid rank/points values",
        ["Nilai used/remaining tidak valid"] = "Invalid used/remaining values",
        ["Nomor tie breaker tidak valid"] = "Invalid tie-breaker number",
        ["Option type tidak valid"] = "Invalid option type",
        ["Password minimal 6 karakter"] = "Password must be at least 6 characters",
        ["Password wajib diisi"] = "Password is required",
        ["Payload action used tidak valid"] = "Invalid action-used payload",
        ["Payload discard ingredient tidak valid"] = "Invalid ingredient-discard payload",
        ["Payload donasi tidak valid"] = "Invalid donation payload",
        ["Payload emergency option tidak valid"] = "Invalid emergency-option payload",
        ["Payload goal tidak valid"] = "Invalid goal payload",
        ["Payload gold points tidak valid"] = "Invalid gold-points payload",
        ["Payload gold trade tidak valid"] = "Invalid gold-trade payload",
        ["Payload ingredient tidak valid"] = "Invalid ingredient payload",
        ["Payload insurance tidak valid"] = "Invalid insurance payload",
        ["Payload insurance used tidak valid"] = "Invalid insurance-used payload",
        ["Payload kebutuhan primer tidak valid"] = "Invalid primary-needs payload",
        ["Payload kebutuhan tidak valid"] = "Invalid needs payload",
        ["Payload kerja lepas tidak valid"] = "Invalid freelance payload",
        ["Payload loan repaid tidak valid"] = "Invalid loan-repaid payload",
        ["Payload loan taken tidak valid"] = "Invalid loan-taken payload",
        ["Payload mission tidak valid"] = "Invalid mission payload",
        ["Payload order claim tidak valid"] = "Invalid order-claim payload",
        ["Payload order pass tidak valid"] = "Invalid order-pass payload",
        ["Payload pension tidak valid"] = "Invalid pension payload",
        ["Payload risiko tidak valid"] = "Invalid risk payload",
        ["Payload tabungan tidak valid"] = "Invalid saving payload",
        ["Payload tie breaker tidak valid"] = "Invalid tie-breaker payload",
        ["Payload transaksi tidak valid"] = "Invalid transaction payload",
        ["Pembayaran melebihi sisa pinjaman"] = "Repayment exceeds remaining loan amount",
        ["Pembelian kebutuhan primer melebihi batas harian"] = "Primary-needs purchase exceeds daily limit",
        ["Penalty misi harus 10 poin"] = "Mission penalty must be 10 points",
        ["Penalty pinjaman harus 15 poin"] = "Loan penalty must be 15 points",
        ["Penalty points tidak valid"] = "Invalid penalty points",
        ["Player belum terdaftar pada sesi"] = "Player is not registered in this session",
        ["Player hanya dapat melihat metrik miliknya"] = "Player can only view their own metrics",
        ["Player tidak ditemukan"] = "Player not found",
        ["Player tidak terdaftar di sesi ini"] = "Player is not registered in this session",
        ["Player wajib diisi"] = "Player is required",
        ["Player wajib diisi untuk actor PLAYER"] = "Player is required for PLAYER actor",
        ["Points tidak valid"] = "Invalid points",
        ["Points wajib diisi"] = "Points are required",
        ["Premium asuransi harus 1 koin"] = "Insurance premium must be 1 coin",
        ["Premium harus > 0"] = "Premium must be greater than 0",
        ["Risiko hanya dapat diambil setelah klaim pesanan"] = "Risk can only be drawn after an order claim",
        ["Risk event bukan milik pemain"] = "Risk event does not belong to the player",
        ["Risk event id tidak valid"] = "Invalid risk event id",
        ["Risk event sudah ditangkal asuransi"] = "Risk event has already been mitigated by insurance",
        ["Risk event tidak ditemukan"] = "Risk event not found",
        ["Risk ID wajib diisi"] = "Risk ID is required",
        ["Role tidak diizinkan"] = "Role is not allowed",
        ["Role tidak valid"] = "Invalid role",
        ["Role wajib diisi"] = "Role is required",
        ["Ruleset belum memiliki versi ACTIVE"] = "Ruleset has no ACTIVE version yet",
        ["Ruleset config tidak valid"] = "Invalid ruleset config",
        ["Ruleset melarang BUY emas"] = "Ruleset does not allow BUY gold",
        ["Ruleset melarang pembelian kebutuhan primer"] = "Ruleset does not allow primary-needs purchase",
        ["Ruleset melarang SELL emas"] = "Ruleset does not allow SELL gold",
        ["Ruleset sudah dipakai sesi"] = "Ruleset is already used by a session",
        ["Ruleset tidak ditemukan"] = "Ruleset not found",
        ["Ruleset version tidak aktif"] = "Ruleset version is not active",
        ["Ruleset version tidak ditemukan"] = "Ruleset version not found",
        ["Saldo tabungan tidak mencukupi"] = "Insufficient saving balance",
        ["Saldo tabungan tidak mencukupi untuk goal"] = "Insufficient saving balance for the goal",
        ["Saldo tidak mencukupi"] = "Insufficient balance",
        ["Sequence number lebih kecil dari event terakhir"] = "Sequence number is lower than the latest event",
        ["Sequence number loncat dari event terakhir"] = "Sequence number skips from the latest event",
        ["Sequence number minimal 0"] = "Sequence number must be at least 0",
        ["Sequence number sudah ada"] = "Sequence number already exists",
        ["Session sudah berakhir"] = "Session has already ended",
        ["Session tidak ditemukan"] = "Session not found",
        ["Setiap klaim pesanan harus diikuti pengambilan risiko pada mode MAHIR"] = "Each order claim must be followed by a risk draw in ADVANCED mode",
        ["Status sesi tidak valid"] = "Invalid session status",
        ["Tie breaker sudah ditetapkan untuk pemain"] = "Tie-breaker has already been assigned to this player",
        ["Token user tidak valid"] = "Invalid user token",
        ["Total kartu bahan melebihi batas ruleset"] = "Total ingredient cards exceed ruleset limit",
        ["Trade type tidak valid"] = "Invalid trade type",
        ["Turn number minimal 1"] = "Turn number must be at least 1",
        ["Username atau password salah"] = "Incorrect username or password",
        ["Username dan password wajib diisi"] = "Username and password are required",
        ["Username harus 3-80 karakter"] = "Username must be 3-80 characters",
        ["Username sudah digunakan"] = "Username is already used",
        ["Username wajib diisi"] = "Username is required",
        ["Weekday harus FRI"] = "Weekday must be FRI",
        ["Weekday harus SAT"] = "Weekday must be SAT",
        ["Weekday tidak valid"] = "Invalid weekday",
        ["Terlalu banyak request"] = "Too many requests",
        ["Terjadi kesalahan pada server"] = "An internal server error occurred"
    };

    /// <summary>
    /// Menjalankan fungsi BuildError sebagai bagian dari alur file ini.
    /// </summary>
    internal static ErrorResponse BuildError(HttpContext httpContext, string code, string message, params ErrorDetail[] details)
    {
        var localizedMessage = ResolveMessage(httpContext, message);
        return new ErrorResponse(code, localizedMessage, details.ToList(), httpContext.TraceIdentifier);
    }

    /// <summary>
    /// Menjalankan fungsi ResolveMessage sebagai bagian dari alur file ini.
    /// </summary>
    private static string ResolveMessage(HttpContext context, string message)
    {
        if (!PrefersEnglish(context))
        {
            return message;
        }

        return IdToEnMessages.TryGetValue(message, out var translated)
            ? translated
            : message;
    }

    /// <summary>
    /// Menjalankan fungsi PrefersEnglish sebagai bagian dari alur file ini.
    /// </summary>
    private static bool PrefersEnglish(HttpContext context)
    {
        var acceptLanguage = context.Request.Headers.AcceptLanguage.ToString();
        if (string.IsNullOrWhiteSpace(acceptLanguage))
        {
            return false;
        }

        return acceptLanguage
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(segment => segment.Split(';', 2, StringSplitOptions.TrimEntries)[0])
            .Any(code => code.StartsWith("en", StringComparison.OrdinalIgnoreCase));
    }
}
