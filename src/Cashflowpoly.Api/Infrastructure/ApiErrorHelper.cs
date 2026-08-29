// Fungsi file: Menyediakan dukungan infrastruktur API melalui ApiErrorHelper.
using Cashflowpoly.Api.Contracts;
using System.Collections.Frozen;

namespace Cashflowpoly.Api.Infrastructure;

/// <summary>
/// Helper statis untuk membangun respons error terstandar dengan terjemahan otomatis Indonesia↔Inggris berdasarkan header Accept-Language.
/// </summary>
internal static class ApiErrorHelper
{
    /// <summary>
    /// Kamus pemetaan pesan error dari Bahasa Indonesia ke Bahasa Inggris.
    /// </summary>
    private static readonly FrozenDictionary<string, string> IdToEnMessages = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["Action type wajib diisi"] = "Action type is required",
        ["Aktivitas ditolak karena melanggar aturan permainan"] = "The activity was rejected because it violates the game rules",
        ["Actor type tidak valid"] = "Invalid actor type",
        ["Akun PLAYER belum terhubung ke profil pemain"] = "This PLAYER account is not linked to a player profile",
        ["Akun instruktur hanya dapat dibuat oleh administrator"] = "Instructor accounts can only be created by an administrator",
        ["Amount harus > 0"] = "Amount must be greater than 0",
        ["Amount kerja lepas tidak sesuai ruleset"] = "Freelance amount does not match the ruleset",
        ["Amount tidak sesuai unit_price * qty"] = "Amount does not match unit_price * qty",
        ["Asuransi hanya berlaku untuk risiko OUT"] = "Insurance can only be used for OUT risk events",
        ["Bahan tidak mencukupi untuk klaim order"] = "Insufficient ingredients to claim the order",
        ["Batch maksimal 500 event"] = "Batch cannot exceed 500 events",
        ["Card ID wajib diisi"] = "Card ID is required",
        ["Category wajib diisi"] = "Category is required",
        ["Config wajib ada"] = "Config is required",
        ["Cost tidak valid"] = "Invalid cost",
        ["Cursor event tidak valid"] = "Invalid event cursor",
        ["Cursor transaksi tidak valid"] = "Invalid transaction cursor",
        ["Counterparty tidak valid"] = "Invalid counterparty",
        ["Daftar event batch wajib diisi"] = "Event batch list is required",
        ["Daftar pemain sudah dikunci sejak pembagian awal disimpan"] = "The player roster is locked after the initial setup is saved",
        ["Day index minimal 0"] = "Day index must be at least 0",
        ["Definition ruleset tidak ditemukan"] = "Ruleset definition not found",
        ["Definition ruleset tidak valid"] = "Invalid ruleset definition",
        ["Definition ruleset wajib ada"] = "Ruleset definition is required",
        ["Direction tidak valid"] = "Invalid direction",
        ["Display name maksimal 80 karakter"] = "Display name must be at most 80 characters",
        ["Emergency option hanya berlaku untuk risiko OUT"] = "Emergency option can only be used for OUT risk events",
        ["Event sudah ada"] = "Event already exists",
        ["Field wajib tidak lengkap"] = "Required fields are incomplete",
        ["Fitur asuransi tidak aktif"] = "Insurance feature is not enabled",
        ["Fitur darurat hanya tersedia di mode MAHIR"] = "Emergency feature is only available in ADVANCED mode",
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
        ["Kepemilikan emas tidak mencukupi"] = "Insufficient gold ownership",
        ["Konfigurasi ruleset tidak valid"] = "Invalid ruleset configuration",
        ["Konfigurasi ruleset tersebut sudah pernah dibuat sebagai versi ruleset ini"] = "This ruleset configuration already exists as a version of this ruleset",
        ["limit harus antara 1 sampai 1000"] = "limit must be between 1 and 1000",
        ["limit harus antara 1 sampai 100"] = "limit must be between 1 and 100",
        ["Loan ID sudah dipakai"] = "Loan ID is already in use",
        ["Loan ID tidak ditemukan"] = "Loan ID not found",
        ["Maksimal tabungan per aksi adalah 15 koin"] = "Maximum saving per action is 15 coins",
        ["Misi sudah ditetapkan untuk pemain"] = "Mission has already been assigned to this player",
        ["Mission ID dan target wajib diisi"] = "Mission ID and target are required",
        ["Mode tidak valid"] = "Invalid mode",
        ["Mode session harus sama dengan mode ruleset version"] = "Session mode must match the ruleset version mode",
        ["Nama pemain wajib diisi"] = "Player name is required",
        ["Nama pemain maksimal 80 karakter"] = "Player name must be at most 80 characters",
        ["Nilai pinjaman harus 10 koin"] = "Loan value must be 10 coins",
        ["Nilai pinjaman tidak valid"] = "Invalid loan value",
        ["Nilai qty/unit_price/amount tidak valid"] = "Invalid qty/unit_price/amount values",
        ["Nilai rank/points tidak valid"] = "Invalid rank/points values",
        ["Nilai used/remaining tidak valid"] = "Invalid used/remaining values",
        ["Nomor tie breaker tidak valid"] = "Invalid tie-breaker number",
        ["Option type tidak valid"] = "Invalid option type",
        ["Password minimal 6 karakter"] = "Password must be at least 6 characters",
        ["Password minimal 12 karakter"] = "Password must be at least 12 characters",
        ["Password maksimal 72 byte UTF-8"] = "Password must be at most 72 UTF-8 bytes",
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
        ["Path version tidak valid"] = "Invalid version path",
        ["Pemain belum membeli asuransi"] = "Player has not purchased insurance",
        ["Pemain tidak terdaftar pada sesi ini"] = "Player is not registered in this session",
        ["Pembayaran melebihi sisa pinjaman"] = "Repayment exceeds remaining loan amount",
        ["Penalty misi harus 10 poin"] = "Mission penalty must be 10 points",
        ["Penalty pinjaman harus 15 poin"] = "Loan penalty must be 15 points",
        ["Penalty points tidak valid"] = "Invalid penalty points",
        ["Player belum terdaftar pada sesi"] = "Player is not registered in this session",
        ["Player hanya dapat melihat metrik miliknya"] = "Player can only view their own metrics",
        ["Player tidak ditemukan"] = "Player not found",
        ["Player tidak terdaftar di sesi ini"] = "Player is not registered in this session",
        ["Player hanya dapat mengirim event actor PLAYER"] = "Player can only submit PLAYER actor events",
        ["Player hanya dapat mengirim event miliknya"] = "Player can only submit their own events",
        ["Player wajib diisi"] = "Player is required",
        ["Player wajib diisi untuk actor PLAYER"] = "Player is required for PLAYER actor",
        ["player_names hanya tersedia untuk environment dev/test"] = "player_names is only available in dev/test environments",
        ["Points tidak valid"] = "Invalid points",
        ["Points wajib diisi"] = "Points are required",
        ["Premium asuransi harus 1 koin"] = "Insurance premium must be 1 coin",
        ["Premium harus > 0"] = "Premium must be greater than 0",
        ["Query mode tidak valid"] = "Invalid mode query",
        ["Query version tidak valid"] = "Invalid version query",
        ["Request tidak valid"] = "Invalid request",
        ["Risiko hanya dapat diambil setelah klaim pesanan"] = "Risk can only be drawn after an order claim",
        ["Risk event bukan milik pemain"] = "Risk event does not belong to the player",
        ["Risk event id tidak valid"] = "Invalid risk event id",
        ["Risk event sudah ditangkal asuransi"] = "Risk event has already been mitigated by insurance",
        ["Risk event tidak ditemukan"] = "Risk event not found",
        ["Risk ID wajib diisi"] = "Risk ID is required",
        ["Role tidak diizinkan"] = "Role is not allowed",
        ["Role tidak dikenali"] = "Unrecognized role",
        ["Role tidak valid"] = "Invalid role",
        ["Role wajib diisi"] = "Role is required",
        ["Data pembagian awal tidak dapat dibaca"] = "The initial setup data cannot be read",
        ["Kirim dan kunci pembagian awal dari IDN sebelum memulai sesi"] = "Submit and lock the initial setup from IDN before starting the session",
        ["Pembagian awal belum dikirim oleh IDN"] = "The initial setup has not been submitted by IDN",
        ["Pembagian awal hanya dapat diperiksa sebelum sesi dimulai"] = "The initial setup can only be validated before the session starts",
        ["Pembagian awal sudah dikunci dan tidak dapat diganti"] = "The initial setup is locked and cannot be replaced",
        ["Pembagian awal tidak dapat diubah setelah sesi dimulai"] = "The initial setup cannot be changed after the session starts",
        ["Pembagian awal tidak lagi sesuai dengan pemain dan set aturan sesi"] = "The initial setup no longer matches the session players and ruleset",
        ["Pembagian awal tidak menggunakan set aturan sesi yang aktif"] = "The initial setup does not use the active session ruleset",
        ["Pembagian awal tidak sesuai dengan pemain dan set aturan sesi"] = "The initial setup does not match the session players and ruleset",
        ["client_request_id sudah dipakai untuk pembagian awal lain"] = "The client_request_id is already used by another initial setup",
        ["player_names tidak lagi didukung. Buat sesi, tambahkan pemain, lalu kirim pembagian awal dari IDN."] = "player_names is no longer supported. Create the session, add players, then submit the initial setup from IDN.",
        ["Ruleset belum memiliki versi ACTIVE"] = "Ruleset has no ACTIVE version yet",
        ["Ruleset aktif session tidak valid"] = "The session active ruleset is invalid",
        ["Ruleset config tidak valid"] = "Invalid ruleset config",
        ["Ruleset melarang BUY emas"] = "Ruleset does not allow BUY gold",
        ["Ruleset melarang pembelian kebutuhan primer"] = "Ruleset does not allow primary-needs purchase",
        ["Ruleset melarang SELL emas"] = "Ruleset does not allow SELL gold",
        ["Ruleset sudah dipakai sesi"] = "Ruleset is already used by a session",
        ["Ruleset default sistem hanya dapat dilihat atau dijadikan dasar membuat ruleset baru."] = "The system default ruleset can only be viewed or used as the basis for a new ruleset.",
        ["Ruleset sudah dipakai pada sesi yang berjalan atau selesai sehingga hanya dapat dilihat."] = "The ruleset is already used by an active or completed session and can only be viewed.",
        ["Ruleset tidak memiliki definisi relasional yang lengkap"] = "Ruleset does not have a complete relational definition",
        ["Ruleset tidak ditemukan"] = "Ruleset not found",
        ["Ruleset version tidak aktif"] = "Ruleset version is not active",
        ["Ruleset version tidak ditemukan"] = "Ruleset version not found",
        ["Ruleset version harus ACTIVE sebelum dipakai sesi"] = "Ruleset version must be ACTIVE before session use",
        ["Saldo tabungan tidak mencukupi"] = "Insufficient saving balance",
        ["Saldo tabungan tidak mencukupi untuk goal"] = "Insufficient saving balance for the goal",
        ["Saldo tidak mencukupi"] = "Insufficient balance",
        ["Sequence number lebih kecil dari event terakhir"] = "Sequence number is lower than the latest event",
        ["Sequence number loncat dari event terakhir"] = "Sequence number skips from the latest event",
        ["Sequence number minimal 0"] = "Sequence number must be at least 0",
        ["Sequence number sudah ada"] = "Sequence number already exists",
        ["Seat number minimal 1"] = "Seat number must be at least 1",
        ["Sesi maksimal 4 pemain"] = "A session can have at most 4 players",
        ["Session harus berstatus STARTED untuk menerima event"] = "Session must be STARTED to accept events",
        ["Session belum memiliki ruleset ACTIVE yang valid"] = "Session does not have a valid ACTIVE ruleset",
        ["Session belum memiliki ruleset aktif"] = "Session does not have an active ruleset",
        ["Session sudah berakhir"] = "Session has already ended",
        ["Session tidak ditemukan"] = "Session not found",
        ["Setiap klaim pesanan harus diikuti pengambilan risiko pada mode MAHIR"] = "Each order claim must be followed by a risk draw in ADVANCED mode",
        ["Status sesi tidak valid"] = "Invalid session status",
        ["State permainan hanya dapat diubah melalui event ingestion"] = "Game state can only be changed through event ingestion",
        ["State session tidak ditemukan"] = "Session state not found",
        ["Terlalu banyak request"] = "Too many requests",
        ["Terjadi kesalahan pada server"] = "An internal server error occurred",
        ["Tie breaker sudah ditetapkan untuk pemain"] = "Tie-breaker has already been assigned to this player",
        ["Token user tidak valid"] = "Invalid user token",
        ["Total kartu bahan melebihi batas ruleset"] = "Total ingredient cards exceed ruleset limit",
        ["Trade type tidak valid"] = "Invalid trade type",
        ["Action slot harus bernilai 1 atau 2"] = "Action slot must be 1 or 2",
        ["Username atau password salah"] = "Incorrect username or password",
        ["Username dan password wajib diisi"] = "Username and password are required",
        ["Username harus 3-80 karakter"] = "Username must be 3-80 characters",
        ["Username sudah digunakan"] = "Username is already used",
        ["Username wajib diisi"] = "Username is required",
        ["User ID atau username wajib diisi"] = "User ID or username is required",
        ["Versi aktif tidak dapat dihapus. Aktifkan versi lain terlebih dahulu."] = "The active version cannot be deleted. Activate another version first.",
        ["Versi ruleset sudah dipakai pada sesi/event sehingga tidak dapat dihapus."] = "The ruleset version is already used by a session or event and cannot be deleted.",
        ["Versi terakhir tidak dapat dihapus. Hapus ruleset jika tidak lagi diperlukan."] = "The last version cannot be deleted. Delete the ruleset if it is no longer needed.",
        ["Weekday harus FRI"] = "Weekday must be FRI",
        ["Weekday harus SAT"] = "Weekday must be SAT",
        ["Weekday tidak valid"] = "Invalid weekday"
    }.ToFrozenDictionary(StringComparer.Ordinal);

    /// <summary>
    /// Membangun objek <see cref="ErrorResponse"/> dengan pesan yang dilokalisasi sesuai preferensi bahasa klien.
    /// </summary>
    /// <param name="httpContext">Konteks HTTP untuk membaca Accept-Language dan trace identifier.</param>
    /// <param name="code">Kode error (misal VALIDATION_ERROR, DUPLICATE).</param>
    /// <param name="message">Pesan error dalam Bahasa Indonesia.</param>
    /// <param name="details">Detail error tambahan.</param>
    /// <returns>Objek ErrorResponse siap dikembalikan ke klien.</returns>
    internal static ErrorResponse BuildError(HttpContext httpContext, string code, string message, params ErrorDetail[] details)
    {
        var localizedMessage = ResolveMessage(httpContext, message);
        return new ErrorResponse(code, localizedMessage, details.ToList(), httpContext.TraceIdentifier);
    }

    /// <summary>
    /// Menerjemahkan pesan ke Bahasa Inggris jika klien memilih Accept-Language: en.
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
    /// Memeriksa apakah klien lebih memilih respons dalam Bahasa Inggris berdasarkan header Accept-Language.
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
