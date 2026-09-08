// Fungsi file: Menyediakan dukungan infrastruktur API melalui ApiErrorHelper.
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `System.Collections.Frozen` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Collections.Frozen;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Infrastructure;

/// <summary>
/// Helper statis untuk membangun respons error terstandar dengan terjemahan otomatis Indonesia↔Inggris berdasarkan header Accept-Language.
/// </summary>
// Mendefinisikan tipe class `ApiErrorHelper`.
internal static class ApiErrorHelper
// Membuka scope tipe ApiErrorHelper; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Kamus pemetaan pesan error dari Bahasa Indonesia ke Bahasa Inggris.
    /// </summary>
    // Mendeklarasikan field bertipe `FrozenDictionary<string, string>`: `IdToEnMessages` menyimpan nilai identitas ke en pesan dengan nilai awal
    // membentuk kamus yang tidak dapat diubah dari `new Dictionary<string, string>(StringComparer.Ordinal) { [”Action type wajib diisi”] = ”Action type
    // is required”, [”Aktivitas ditolak karena melanggar aturan permainan”] = ”Th...` untuk pencarian berulang. readonly membatasi penggantian
    // referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly FrozenDictionary<string, string> IdToEnMessages = new Dictionary<string, string>(StringComparer.Ordinal)
    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Memperbarui `[”Action type wajib diisi”]` menggunakan nilai literal `”Action type is required”`.
        ["Action type wajib diisi"] = "Action type is required",
        // Memperbarui `[”Aktivitas ditolak karena melanggar aturan permainan”]` menggunakan nilai literal `”The activity was rejected because it violates
        // the game rules”`.
        ["Aktivitas ditolak karena melanggar aturan permainan"] = "The activity was rejected because it violates the game rules",
        // Memperbarui `[”Actor type tidak valid”]` menggunakan nilai literal `”Invalid actor type”`.
        ["Actor type tidak valid"] = "Invalid actor type",
        // Memperbarui `[”Akun PLAYER belum terhubung ke profil pemain”]` menggunakan nilai literal `”This PLAYER account is not linked to a player
        // profile”`.
        ["Akun PLAYER belum terhubung ke profil pemain"] = "This PLAYER account is not linked to a player profile",
        // Memperbarui `[”Akun instruktur hanya dapat dibuat oleh administrator”]` menggunakan nilai literal `”Instructor accounts can only be created by an
        // administrator”`.
        ["Akun instruktur hanya dapat dibuat oleh administrator"] = "Instructor accounts can only be created by an administrator",
        // Memperbarui `[”Amount harus > 0”]` menggunakan nilai literal `”Amount must be greater than 0”`.
        ["Amount harus > 0"] = "Amount must be greater than 0",
        // Memperbarui `[”Amount kerja lepas tidak sesuai ruleset”]` menggunakan nilai literal `”Freelance amount does not match the ruleset”`.
        ["Amount kerja lepas tidak sesuai ruleset"] = "Freelance amount does not match the ruleset",
        // Memperbarui `[”Amount tidak sesuai unit_price * qty”]` menggunakan nilai literal `”Amount does not match unit_price * qty”`.
        ["Amount tidak sesuai unit_price * qty"] = "Amount does not match unit_price * qty",
        // Memperbarui `[”Asuransi hanya berlaku untuk risiko OUT”]` menggunakan nilai literal `”Insurance can only be used for OUT risk events”`.
        ["Asuransi hanya berlaku untuk risiko OUT"] = "Insurance can only be used for OUT risk events",
        // Memperbarui `[”Bahan tidak mencukupi untuk klaim order”]` menggunakan nilai literal `”Insufficient ingredients to claim the order”`.
        ["Bahan tidak mencukupi untuk klaim order"] = "Insufficient ingredients to claim the order",
        // Memperbarui `[”Batch maksimal 500 event”]` menggunakan nilai literal `”Batch cannot exceed 500 events”`.
        ["Batch maksimal 500 event"] = "Batch cannot exceed 500 events",
        // Memperbarui `[”Card ID wajib diisi”]` menggunakan nilai literal `”Card ID is required”`.
        ["Card ID wajib diisi"] = "Card ID is required",
        // Memperbarui `[”Category wajib diisi”]` menggunakan nilai literal `”Category is required”`.
        ["Category wajib diisi"] = "Category is required",
        // Memperbarui `[”Config wajib ada”]` menggunakan nilai literal `”Config is required”`.
        ["Config wajib ada"] = "Config is required",
        // Memperbarui `[”Cost tidak valid”]` menggunakan nilai literal `”Invalid cost”`.
        ["Cost tidak valid"] = "Invalid cost",
        // Memperbarui `[”Cursor event tidak valid”]` menggunakan nilai literal `”Invalid event cursor”`.
        ["Cursor event tidak valid"] = "Invalid event cursor",
        // Memperbarui `[”Cursor transaksi tidak valid”]` menggunakan nilai literal `”Invalid transaction cursor”`.
        ["Cursor transaksi tidak valid"] = "Invalid transaction cursor",
        // Memperbarui `[”Counterparty tidak valid”]` menggunakan nilai literal `”Invalid counterparty”`.
        ["Counterparty tidak valid"] = "Invalid counterparty",
        // Memperbarui `[”Daftar event batch wajib diisi”]` menggunakan nilai literal `”Event batch list is required”`.
        ["Daftar event batch wajib diisi"] = "Event batch list is required",
        // Memperbarui `[”Daftar pemain sudah dikunci sejak pembagian awal disimpan”]` menggunakan nilai literal `”The player roster is locked after the
        // initial setup is saved”`.
        ["Daftar pemain sudah dikunci sejak pembagian awal disimpan"] = "The player roster is locked after the initial setup is saved",
        // Memperbarui `[”Day index minimal 0”]` menggunakan nilai literal `”Day index must be at least 0”`.
        ["Day index minimal 0"] = "Day index must be at least 0",
        // Memperbarui `[”Definition ruleset tidak ditemukan”]` menggunakan nilai literal `”Ruleset definition not found”`.
        ["Definition ruleset tidak ditemukan"] = "Ruleset definition not found",
        // Memperbarui `[”Definition ruleset tidak valid”]` menggunakan nilai literal `”Invalid ruleset definition”`.
        ["Definition ruleset tidak valid"] = "Invalid ruleset definition",
        // Memperbarui `[”Definition ruleset wajib ada”]` menggunakan nilai literal `”Ruleset definition is required”`.
        ["Definition ruleset wajib ada"] = "Ruleset definition is required",
        // Memperbarui `[”Direction tidak valid”]` menggunakan nilai literal `”Invalid direction”`.
        ["Direction tidak valid"] = "Invalid direction",
        // Memperbarui `[”Display name maksimal 80 karakter”]` menggunakan nilai literal `”Display name must be at most 80 characters”`.
        ["Display name maksimal 80 karakter"] = "Display name must be at most 80 characters",
        // Memperbarui `[”Emergency option hanya berlaku untuk risiko OUT”]` menggunakan nilai literal `”Emergency option can only be used for OUT risk
        // events”`.
        ["Emergency option hanya berlaku untuk risiko OUT"] = "Emergency option can only be used for OUT risk events",
        // Memperbarui `[”Event sudah ada”]` menggunakan nilai literal `”Event already exists”`.
        ["Event sudah ada"] = "Event already exists",
        // Memperbarui `[”Field wajib tidak lengkap”]` menggunakan nilai literal `”Required fields are incomplete”`.
        ["Field wajib tidak lengkap"] = "Required fields are incomplete",
        // Memperbarui `[”Fitur asuransi tidak aktif”]` menggunakan nilai literal `”Insurance feature is not enabled”`.
        ["Fitur asuransi tidak aktif"] = "Insurance feature is not enabled",
        // Memperbarui `[”Fitur darurat hanya tersedia di mode MAHIR”]` menggunakan nilai literal `”Emergency feature is only available in ADVANCED mode”`.
        ["Fitur darurat hanya tersedia di mode MAHIR"] = "Emergency feature is only available in ADVANCED mode",
        // Memperbarui `[”Fitur donasi Jumat tidak aktif”]` menggunakan nilai literal `”Friday donation feature is not enabled”`.
        ["Fitur donasi Jumat tidak aktif"] = "Friday donation feature is not enabled",
        // Memperbarui `[”Fitur perdagangan emas tidak aktif”]` menggunakan nilai literal `”Gold trading feature is not enabled”`.
        ["Fitur perdagangan emas tidak aktif"] = "Gold trading feature is not enabled",
        // Memperbarui `[”Fitur pinjaman tidak aktif”]` menggunakan nilai literal `”Loan feature is not enabled”`.
        ["Fitur pinjaman tidak aktif"] = "Loan feature is not enabled",
        // Memperbarui `[”Fitur risiko hanya tersedia di mode MAHIR”]` menggunakan nilai literal `”Risk feature is only available in ADVANCED mode”`.
        ["Fitur risiko hanya tersedia di mode MAHIR"] = "Risk feature is only available in ADVANCED mode",
        // Memperbarui `[”Fitur tabungan tujuan tidak aktif”]` menggunakan nilai literal `”Saving-goal feature is not enabled”`.
        ["Fitur tabungan tujuan tidak aktif"] = "Saving-goal feature is not enabled",
        // Memperbarui `[”fromSeq tidak boleh negatif”]` menggunakan nilai literal `”fromSeq cannot be negative”`.
        ["fromSeq tidak boleh negatif"] = "fromSeq cannot be negative",
        // Memperbarui `[”Gagal sinkronisasi profil player”]` menggunakan nilai literal `”Failed to synchronize player profile”`.
        ["Gagal sinkronisasi profil player"] = "Failed to synchronize player profile",
        // Memperbarui `[”Goal ID wajib diisi”]` menggunakan nilai literal `”Goal ID is required”`.
        ["Goal ID wajib diisi"] = "Goal ID is required",
        // Memperbarui `[”Income harus > 0”]` menggunakan nilai literal `”Income must be greater than 0”`.
        ["Income harus > 0"] = "Income must be greater than 0",
        // Memperbarui `[”Jumlah discard melebihi stok bahan”]` menggunakan nilai literal `”Discard quantity exceeds available ingredient stock”`.
        ["Jumlah discard melebihi stok bahan"] = "Discard quantity exceeds available ingredient stock",
        // Memperbarui `[”Jumlah donasi di luar batas”]` menggunakan nilai literal `”Donation amount is outside the allowed range”`.
        ["Jumlah donasi di luar batas"] = "Donation amount is outside the allowed range",
        // Memperbarui `[”Jumlah kartu bahan sejenis melebihi batas ruleset”]` menggunakan nilai literal `”Quantity of same ingredient cards exceeds ruleset
        // limit”`.
        ["Jumlah kartu bahan sejenis melebihi batas ruleset"] = "Quantity of same ingredient cards exceeds ruleset limit",
        // Memperbarui `[”Jumlah token aksi melebihi batas ruleset”]` menggunakan nilai literal `”Action token usage exceeds ruleset limit”`.
        ["Jumlah token aksi melebihi batas ruleset"] = "Action token usage exceeds ruleset limit",
        // Memperbarui `[”Kepemilikan emas tidak mencukupi”]` menggunakan nilai literal `”Insufficient gold ownership”`.
        ["Kepemilikan emas tidak mencukupi"] = "Insufficient gold ownership",
        // Memperbarui `[”Konfigurasi ruleset tidak valid”]` menggunakan nilai literal `”Invalid ruleset configuration”`.
        ["Konfigurasi ruleset tidak valid"] = "Invalid ruleset configuration",
        // Memperbarui `[”Konfigurasi ruleset tersebut sudah pernah dibuat sebagai versi ruleset ini”]` menggunakan nilai literal `”This ruleset
        // configuration already exists as a version of this ruleset”`.
        ["Konfigurasi ruleset tersebut sudah pernah dibuat sebagai versi ruleset ini"] = "This ruleset configuration already exists as a version of this ruleset",
        // Memperbarui `[”limit harus antara 1 sampai 1000”]` menggunakan nilai literal `”limit must be between 1 and 1000”`.
        ["limit harus antara 1 sampai 1000"] = "limit must be between 1 and 1000",
        // Memperbarui `[”limit harus antara 1 sampai 100”]` menggunakan nilai literal `”limit must be between 1 and 100”`.
        ["limit harus antara 1 sampai 100"] = "limit must be between 1 and 100",
        // Memperbarui `[”Loan ID sudah dipakai”]` menggunakan nilai literal `”Loan ID is already in use”`.
        ["Loan ID sudah dipakai"] = "Loan ID is already in use",
        // Memperbarui `[”Loan ID tidak ditemukan”]` menggunakan nilai literal `”Loan ID not found”`.
        ["Loan ID tidak ditemukan"] = "Loan ID not found",
        // Memperbarui `[”Maksimal tabungan per aksi adalah 15 koin”]` menggunakan nilai literal `”Maximum saving per action is 15 coins”`.
        ["Maksimal tabungan per aksi adalah 15 koin"] = "Maximum saving per action is 15 coins",
        // Memperbarui `[”Misi sudah ditetapkan untuk pemain”]` menggunakan nilai literal `”Mission has already been assigned to this player”`.
        ["Misi sudah ditetapkan untuk pemain"] = "Mission has already been assigned to this player",
        // Memperbarui `[”Mission ID dan target wajib diisi”]` menggunakan nilai literal `”Mission ID and target are required”`.
        ["Mission ID dan target wajib diisi"] = "Mission ID and target are required",
        // Memperbarui `[”Mode tidak valid”]` menggunakan nilai literal `”Invalid mode”`.
        ["Mode tidak valid"] = "Invalid mode",
        // Memperbarui `[”Mode session harus sama dengan mode ruleset version”]` menggunakan nilai literal `”Session mode must match the ruleset version
        // mode”`.
        ["Mode session harus sama dengan mode ruleset version"] = "Session mode must match the ruleset version mode",
        // Memperbarui `[”Nama pemain wajib diisi”]` menggunakan nilai literal `”Player name is required”`.
        ["Nama pemain wajib diisi"] = "Player name is required",
        // Memperbarui `[”Nama pemain maksimal 80 karakter”]` menggunakan nilai literal `”Player name must be at most 80 characters”`.
        ["Nama pemain maksimal 80 karakter"] = "Player name must be at most 80 characters",
        // Memperbarui `[”Nilai pinjaman harus 10 koin”]` menggunakan nilai literal `”Loan value must be 10 coins”`.
        ["Nilai pinjaman harus 10 koin"] = "Loan value must be 10 coins",
        // Memperbarui `[”Nilai pinjaman tidak valid”]` menggunakan nilai literal `”Invalid loan value”`.
        ["Nilai pinjaman tidak valid"] = "Invalid loan value",
        // Memperbarui `[”Nilai qty/unit_price/amount tidak valid”]` menggunakan nilai literal `”Invalid qty/unit_price/amount values”`.
        ["Nilai qty/unit_price/amount tidak valid"] = "Invalid qty/unit_price/amount values",
        // Memperbarui `[”Nilai rank/points tidak valid”]` menggunakan nilai literal `”Invalid rank/points values”`.
        ["Nilai rank/points tidak valid"] = "Invalid rank/points values",
        // Memperbarui `[”Nilai used/remaining tidak valid”]` menggunakan nilai literal `”Invalid used/remaining values”`.
        ["Nilai used/remaining tidak valid"] = "Invalid used/remaining values",
        // Memperbarui `[”Nomor tie breaker tidak valid”]` menggunakan nilai literal `”Invalid tie-breaker number”`.
        ["Nomor tie breaker tidak valid"] = "Invalid tie-breaker number",
        // Memperbarui `[”Option type tidak valid”]` menggunakan nilai literal `”Invalid option type”`.
        ["Option type tidak valid"] = "Invalid option type",
        // Memperbarui `[”Password minimal 6 karakter”]` menggunakan nilai literal `”Password must be at least 6 characters”`.
        ["Password minimal 6 karakter"] = "Password must be at least 6 characters",
        // Memperbarui `[”Password minimal 12 karakter”]` menggunakan nilai literal `”Password must be at least 12 characters”`.
        ["Password minimal 12 karakter"] = "Password must be at least 12 characters",
        // Memperbarui `[”Password maksimal 72 byte UTF-8”]` menggunakan nilai literal `”Password must be at most 72 UTF-8 bytes”`.
        ["Password maksimal 72 byte UTF-8"] = "Password must be at most 72 UTF-8 bytes",
        // Memperbarui `[”Password wajib diisi”]` menggunakan nilai literal `”Password is required”`.
        ["Password wajib diisi"] = "Password is required",
        // Memperbarui `[”Payload action used tidak valid”]` menggunakan nilai literal `”Invalid action-used payload”`.
        ["Payload action used tidak valid"] = "Invalid action-used payload",
        // Memperbarui `[”Payload discard ingredient tidak valid”]` menggunakan nilai literal `”Invalid ingredient-discard payload”`.
        ["Payload discard ingredient tidak valid"] = "Invalid ingredient-discard payload",
        // Memperbarui `[”Payload donasi tidak valid”]` menggunakan nilai literal `”Invalid donation payload”`.
        ["Payload donasi tidak valid"] = "Invalid donation payload",
        // Memperbarui `[”Payload emergency option tidak valid”]` menggunakan nilai literal `”Invalid emergency-option payload”`.
        ["Payload emergency option tidak valid"] = "Invalid emergency-option payload",
        // Memperbarui `[”Payload goal tidak valid”]` menggunakan nilai literal `”Invalid goal payload”`.
        ["Payload goal tidak valid"] = "Invalid goal payload",
        // Memperbarui `[”Payload gold points tidak valid”]` menggunakan nilai literal `”Invalid gold-points payload”`.
        ["Payload gold points tidak valid"] = "Invalid gold-points payload",
        // Memperbarui `[”Payload gold trade tidak valid”]` menggunakan nilai literal `”Invalid gold-trade payload”`.
        ["Payload gold trade tidak valid"] = "Invalid gold-trade payload",
        // Memperbarui `[”Payload ingredient tidak valid”]` menggunakan nilai literal `”Invalid ingredient payload”`.
        ["Payload ingredient tidak valid"] = "Invalid ingredient payload",
        // Memperbarui `[”Payload insurance tidak valid”]` menggunakan nilai literal `”Invalid insurance payload”`.
        ["Payload insurance tidak valid"] = "Invalid insurance payload",
        // Memperbarui `[”Payload insurance used tidak valid”]` menggunakan nilai literal `”Invalid insurance-used payload”`.
        ["Payload insurance used tidak valid"] = "Invalid insurance-used payload",
        // Memperbarui `[”Payload kebutuhan primer tidak valid”]` menggunakan nilai literal `”Invalid primary-needs payload”`.
        ["Payload kebutuhan primer tidak valid"] = "Invalid primary-needs payload",
        // Memperbarui `[”Payload kebutuhan tidak valid”]` menggunakan nilai literal `”Invalid needs payload”`.
        ["Payload kebutuhan tidak valid"] = "Invalid needs payload",
        // Memperbarui `[”Payload kerja lepas tidak valid”]` menggunakan nilai literal `”Invalid freelance payload”`.
        ["Payload kerja lepas tidak valid"] = "Invalid freelance payload",
        // Memperbarui `[”Payload loan repaid tidak valid”]` menggunakan nilai literal `”Invalid loan-repaid payload”`.
        ["Payload loan repaid tidak valid"] = "Invalid loan-repaid payload",
        // Memperbarui `[”Payload loan taken tidak valid”]` menggunakan nilai literal `”Invalid loan-taken payload”`.
        ["Payload loan taken tidak valid"] = "Invalid loan-taken payload",
        // Memperbarui `[”Payload mission tidak valid”]` menggunakan nilai literal `”Invalid mission payload”`.
        ["Payload mission tidak valid"] = "Invalid mission payload",
        // Memperbarui `[”Payload order claim tidak valid”]` menggunakan nilai literal `”Invalid order-claim payload”`.
        ["Payload order claim tidak valid"] = "Invalid order-claim payload",
        // Memperbarui `[”Payload order pass tidak valid”]` menggunakan nilai literal `”Invalid order-pass payload”`.
        ["Payload order pass tidak valid"] = "Invalid order-pass payload",
        // Memperbarui `[”Payload pension tidak valid”]` menggunakan nilai literal `”Invalid pension payload”`.
        ["Payload pension tidak valid"] = "Invalid pension payload",
        // Memperbarui `[”Payload risiko tidak valid”]` menggunakan nilai literal `”Invalid risk payload”`.
        ["Payload risiko tidak valid"] = "Invalid risk payload",
        // Memperbarui `[”Payload tabungan tidak valid”]` menggunakan nilai literal `”Invalid saving payload”`.
        ["Payload tabungan tidak valid"] = "Invalid saving payload",
        // Memperbarui `[”Payload tie breaker tidak valid”]` menggunakan nilai literal `”Invalid tie-breaker payload”`.
        ["Payload tie breaker tidak valid"] = "Invalid tie-breaker payload",
        // Memperbarui `[”Payload transaksi tidak valid”]` menggunakan nilai literal `”Invalid transaction payload”`.
        ["Payload transaksi tidak valid"] = "Invalid transaction payload",
        // Memperbarui `[”Path version tidak valid”]` menggunakan nilai literal `”Invalid version path”`.
        ["Path version tidak valid"] = "Invalid version path",
        // Memperbarui `[”Pemain belum membeli asuransi”]` menggunakan nilai literal `”Player has not purchased insurance”`.
        ["Pemain belum membeli asuransi"] = "Player has not purchased insurance",
        // Memperbarui `[”Pemain tidak terdaftar pada sesi ini”]` menggunakan nilai literal `”Player is not registered in this session”`.
        ["Pemain tidak terdaftar pada sesi ini"] = "Player is not registered in this session",
        // Memperbarui `[”Pembayaran melebihi sisa pinjaman”]` menggunakan nilai literal `”Repayment exceeds remaining loan amount”`.
        ["Pembayaran melebihi sisa pinjaman"] = "Repayment exceeds remaining loan amount",
        // Memperbarui `[”Penalty misi harus 10 poin”]` menggunakan nilai literal `”Mission penalty must be 10 points”`.
        ["Penalty misi harus 10 poin"] = "Mission penalty must be 10 points",
        // Memperbarui `[”Penalty pinjaman harus 15 poin”]` menggunakan nilai literal `”Loan penalty must be 15 points”`.
        ["Penalty pinjaman harus 15 poin"] = "Loan penalty must be 15 points",
        // Memperbarui `[”Penalty points tidak valid”]` menggunakan nilai literal `”Invalid penalty points”`.
        ["Penalty points tidak valid"] = "Invalid penalty points",
        // Memperbarui `[”Player belum terdaftar pada sesi”]` menggunakan nilai literal `”Player is not registered in this session”`.
        ["Player belum terdaftar pada sesi"] = "Player is not registered in this session",
        // Memperbarui `[”Player hanya dapat melihat metrik miliknya”]` menggunakan nilai literal `”Player can only view their own metrics”`.
        ["Player hanya dapat melihat metrik miliknya"] = "Player can only view their own metrics",
        // Memperbarui `[”Player tidak ditemukan”]` menggunakan nilai literal `”Player not found”`.
        ["Player tidak ditemukan"] = "Player not found",
        // Memperbarui `[”Player tidak terdaftar di sesi ini”]` menggunakan nilai literal `”Player is not registered in this session”`.
        ["Player tidak terdaftar di sesi ini"] = "Player is not registered in this session",
        // Memperbarui `[”Player hanya dapat mengirim event actor PLAYER”]` menggunakan nilai literal `”Player can only submit PLAYER actor events”`.
        ["Player hanya dapat mengirim event actor PLAYER"] = "Player can only submit PLAYER actor events",
        // Memperbarui `[”Player hanya dapat mengirim event miliknya”]` menggunakan nilai literal `”Player can only submit their own events”`.
        ["Player hanya dapat mengirim event miliknya"] = "Player can only submit their own events",
        // Memperbarui `[”Player wajib diisi”]` menggunakan nilai literal `”Player is required”`.
        ["Player wajib diisi"] = "Player is required",
        // Memperbarui `[”Player wajib diisi untuk actor PLAYER”]` menggunakan nilai literal `”Player is required for PLAYER actor”`.
        ["Player wajib diisi untuk actor PLAYER"] = "Player is required for PLAYER actor",
        // Memperbarui `[”player_names hanya tersedia untuk environment dev/test”]` menggunakan nilai literal `”player_names is only available in dev/test
        // environments”`.
        ["player_names hanya tersedia untuk environment dev/test"] = "player_names is only available in dev/test environments",
        // Memperbarui `[”Points tidak valid”]` menggunakan nilai literal `”Invalid points”`.
        ["Points tidak valid"] = "Invalid points",
        // Memperbarui `[”Points wajib diisi”]` menggunakan nilai literal `”Points are required”`.
        ["Points wajib diisi"] = "Points are required",
        // Memperbarui `[”Premium asuransi harus 1 koin”]` menggunakan nilai literal `”Insurance premium must be 1 coin”`.
        ["Premium asuransi harus 1 koin"] = "Insurance premium must be 1 coin",
        // Memperbarui `[”Premium harus > 0”]` menggunakan nilai literal `”Premium must be greater than 0”`.
        ["Premium harus > 0"] = "Premium must be greater than 0",
        // Memperbarui `[”Query mode tidak valid”]` menggunakan nilai literal `”Invalid mode query”`.
        ["Query mode tidak valid"] = "Invalid mode query",
        // Memperbarui `[”Query version tidak valid”]` menggunakan nilai literal `”Invalid version query”`.
        ["Query version tidak valid"] = "Invalid version query",
        // Memperbarui `[”Request tidak valid”]` menggunakan nilai literal `”Invalid request”`.
        ["Request tidak valid"] = "Invalid request",
        // Memperbarui `[”Risiko hanya dapat diambil setelah klaim pesanan”]` menggunakan nilai literal `”Risk can only be drawn after an order claim”`.
        ["Risiko hanya dapat diambil setelah klaim pesanan"] = "Risk can only be drawn after an order claim",
        // Memperbarui `[”Risk event bukan milik pemain”]` menggunakan nilai literal `”Risk event does not belong to the player”`.
        ["Risk event bukan milik pemain"] = "Risk event does not belong to the player",
        // Memperbarui `[”Risk event id tidak valid”]` menggunakan nilai literal `”Invalid risk event id”`.
        ["Risk event id tidak valid"] = "Invalid risk event id",
        // Memperbarui `[”Risk event sudah ditangkal asuransi”]` menggunakan nilai literal `”Risk event has already been mitigated by insurance”`.
        ["Risk event sudah ditangkal asuransi"] = "Risk event has already been mitigated by insurance",
        // Memperbarui `[”Risk event tidak ditemukan”]` menggunakan nilai literal `”Risk event not found”`.
        ["Risk event tidak ditemukan"] = "Risk event not found",
        // Memperbarui `[”Risk ID wajib diisi”]` menggunakan nilai literal `”Risk ID is required”`.
        ["Risk ID wajib diisi"] = "Risk ID is required",
        // Memperbarui `[”Role tidak diizinkan”]` menggunakan nilai literal `”Role is not allowed”`.
        ["Role tidak diizinkan"] = "Role is not allowed",
        // Memperbarui `[”Role tidak dikenali”]` menggunakan nilai literal `”Unrecognized role”`.
        ["Role tidak dikenali"] = "Unrecognized role",
        // Memperbarui `[”Role tidak valid”]` menggunakan nilai literal `”Invalid role”`.
        ["Role tidak valid"] = "Invalid role",
        // Memperbarui `[”Role wajib diisi”]` menggunakan nilai literal `”Role is required”`.
        ["Role wajib diisi"] = "Role is required",
        // Memperbarui `[”Data pembagian awal tidak dapat dibaca”]` menggunakan nilai literal `”The initial setup data cannot be read”`.
        ["Data pembagian awal tidak dapat dibaca"] = "The initial setup data cannot be read",
        // Memperbarui `[”Kirim dan kunci pembagian awal dari IDN sebelum memulai sesi”]` menggunakan nilai literal `”Submit and lock the initial setup from
        // IDN before starting the session”`.
        ["Kirim dan kunci pembagian awal dari IDN sebelum memulai sesi"] = "Submit and lock the initial setup from IDN before starting the session",
        // Memperbarui `[”Pembagian awal belum dikirim oleh IDN”]` menggunakan nilai literal `”The initial setup has not been submitted by IDN”`.
        ["Pembagian awal belum dikirim oleh IDN"] = "The initial setup has not been submitted by IDN",
        // Memperbarui `[”Pembagian awal hanya dapat diperiksa sebelum sesi dimulai”]` menggunakan nilai literal `”The initial setup can only be validated
        // before the session starts”`.
        ["Pembagian awal hanya dapat diperiksa sebelum sesi dimulai"] = "The initial setup can only be validated before the session starts",
        // Memperbarui `[”Pembagian awal sudah dikunci dan tidak dapat diganti”]` menggunakan nilai literal `”The initial setup is locked and cannot be
        // replaced”`.
        ["Pembagian awal sudah dikunci dan tidak dapat diganti"] = "The initial setup is locked and cannot be replaced",
        // Memperbarui `[”Pembagian awal tidak dapat diubah setelah sesi dimulai”]` menggunakan nilai literal `”The initial setup cannot be changed after
        // the session starts”`.
        ["Pembagian awal tidak dapat diubah setelah sesi dimulai"] = "The initial setup cannot be changed after the session starts",
        // Memperbarui `[”Pembagian awal tidak lagi sesuai dengan pemain dan set aturan sesi”]` menggunakan nilai literal `”The initial setup no longer
        // matches the session players and ruleset”`.
        ["Pembagian awal tidak lagi sesuai dengan pemain dan set aturan sesi"] = "The initial setup no longer matches the session players and ruleset",
        // Memperbarui `[”Pembagian awal tidak menggunakan set aturan sesi yang aktif”]` menggunakan nilai literal `”The initial setup does not use the
        // active session ruleset”`.
        ["Pembagian awal tidak menggunakan set aturan sesi yang aktif"] = "The initial setup does not use the active session ruleset",
        // Memperbarui `[”Pembagian awal tidak sesuai dengan pemain dan set aturan sesi”]` menggunakan nilai literal `”The initial setup does not match the
        // session players and ruleset”`.
        ["Pembagian awal tidak sesuai dengan pemain dan set aturan sesi"] = "The initial setup does not match the session players and ruleset",
        // Memperbarui `[”client_request_id sudah dipakai untuk pembagian awal lain”]` menggunakan nilai literal `”The client_request_id is already used by
        // another initial setup”`.
        ["client_request_id sudah dipakai untuk pembagian awal lain"] = "The client_request_id is already used by another initial setup",
        // Memperbarui `[”player_names tidak lagi didukung. Buat sesi, tambahkan pemain, lalu kirim pembagian awal dari IDN.”]` menggunakan nilai literal
        // `”player_names is no longer supported. Create the session, add players, then submit the initial setup from IDN.”`.
        ["player_names tidak lagi didukung. Buat sesi, tambahkan pemain, lalu kirim pembagian awal dari IDN."] = "player_names is no longer supported. Create the session, add players, then submit the initial setup from IDN.",
        // Memperbarui `[”Ruleset belum memiliki versi ACTIVE”]` menggunakan nilai literal `”Ruleset has no ACTIVE version yet”`.
        ["Ruleset belum memiliki versi ACTIVE"] = "Ruleset has no ACTIVE version yet",
        // Memperbarui `[”Ruleset aktif session tidak valid”]` menggunakan nilai literal `”The session active ruleset is invalid”`.
        ["Ruleset aktif session tidak valid"] = "The session active ruleset is invalid",
        // Memperbarui `[”Ruleset config tidak valid”]` menggunakan nilai literal `”Invalid ruleset config”`.
        ["Ruleset config tidak valid"] = "Invalid ruleset config",
        // Memperbarui `[”Ruleset melarang BUY emas”]` menggunakan nilai literal `”Ruleset does not allow BUY gold”`.
        ["Ruleset melarang BUY emas"] = "Ruleset does not allow BUY gold",
        // Memperbarui `[”Ruleset melarang pembelian kebutuhan primer”]` menggunakan nilai literal `”Ruleset does not allow primary-needs purchase”`.
        ["Ruleset melarang pembelian kebutuhan primer"] = "Ruleset does not allow primary-needs purchase",
        // Memperbarui `[”Ruleset melarang SELL emas”]` menggunakan nilai literal `”Ruleset does not allow SELL gold”`.
        ["Ruleset melarang SELL emas"] = "Ruleset does not allow SELL gold",
        // Memperbarui `[”Ruleset sudah dipakai sesi”]` menggunakan nilai literal `”Ruleset is already used by a session”`.
        ["Ruleset sudah dipakai sesi"] = "Ruleset is already used by a session",
        // Memperbarui `[”Ruleset default sistem hanya dapat dilihat atau dijadikan dasar membuat ruleset baru.”]` menggunakan nilai literal `”The system
        // default ruleset can only be viewed or used as the basis for a new ruleset.”`.
        ["Ruleset default sistem hanya dapat dilihat atau dijadikan dasar membuat ruleset baru."] = "The system default ruleset can only be viewed or used as the basis for a new ruleset.",
        // Memperbarui `[”Ruleset sudah dipakai pada sesi yang berjalan atau selesai sehingga hanya dapat dilihat.”]` menggunakan nilai literal `”The
        // ruleset is already used by an active or completed session and can only be viewed.”`.
        ["Ruleset sudah dipakai pada sesi yang berjalan atau selesai sehingga hanya dapat dilihat."] = "The ruleset is already used by an active or completed session and can only be viewed.",
        // Memperbarui `[”Ruleset tidak memiliki definisi relasional yang lengkap”]` menggunakan nilai literal `”Ruleset does not have a complete relational
        // definition”`.
        ["Ruleset tidak memiliki definisi relasional yang lengkap"] = "Ruleset does not have a complete relational definition",
        // Memperbarui `[”Ruleset tidak ditemukan”]` menggunakan nilai literal `”Ruleset not found”`.
        ["Ruleset tidak ditemukan"] = "Ruleset not found",
        // Memperbarui `[”Ruleset version tidak aktif”]` menggunakan nilai literal `”Ruleset version is not active”`.
        ["Ruleset version tidak aktif"] = "Ruleset version is not active",
        // Memperbarui `[”Ruleset version tidak ditemukan”]` menggunakan nilai literal `”Ruleset version not found”`.
        ["Ruleset version tidak ditemukan"] = "Ruleset version not found",
        // Memperbarui `[”Ruleset version harus ACTIVE sebelum dipakai sesi”]` menggunakan nilai literal `”Ruleset version must be ACTIVE before session
        // use”`.
        ["Ruleset version harus ACTIVE sebelum dipakai sesi"] = "Ruleset version must be ACTIVE before session use",
        // Memperbarui `[”Saldo tabungan tidak mencukupi”]` menggunakan nilai literal `”Insufficient saving balance”`.
        ["Saldo tabungan tidak mencukupi"] = "Insufficient saving balance",
        // Memperbarui `[”Saldo tabungan tidak mencukupi untuk goal”]` menggunakan nilai literal `”Insufficient saving balance for the goal”`.
        ["Saldo tabungan tidak mencukupi untuk goal"] = "Insufficient saving balance for the goal",
        // Memperbarui `[”Saldo tidak mencukupi”]` menggunakan nilai literal `”Insufficient balance”`.
        ["Saldo tidak mencukupi"] = "Insufficient balance",
        // Memperbarui `[”Sequence number lebih kecil dari event terakhir”]` menggunakan nilai literal `”Sequence number is lower than the latest event”`.
        ["Sequence number lebih kecil dari event terakhir"] = "Sequence number is lower than the latest event",
        // Memperbarui `[”Sequence number loncat dari event terakhir”]` menggunakan nilai literal `”Sequence number skips from the latest event”`.
        ["Sequence number loncat dari event terakhir"] = "Sequence number skips from the latest event",
        // Memperbarui `[”Sequence number minimal 0”]` menggunakan nilai literal `”Sequence number must be at least 0”`.
        ["Sequence number minimal 0"] = "Sequence number must be at least 0",
        // Memperbarui `[”Sequence number sudah ada”]` menggunakan nilai literal `”Sequence number already exists”`.
        ["Sequence number sudah ada"] = "Sequence number already exists",
        // Memperbarui `[”Seat number minimal 1”]` menggunakan nilai literal `”Seat number must be at least 1”`.
        ["Seat number minimal 1"] = "Seat number must be at least 1",
        // Memperbarui `[”Sesi maksimal 4 pemain”]` menggunakan nilai literal `”A session can have at most 4 players”`.
        ["Sesi maksimal 4 pemain"] = "A session can have at most 4 players",
        // Memperbarui `[”Session harus berstatus STARTED untuk menerima event”]` menggunakan nilai literal `”Session must be STARTED to accept events”`.
        ["Session harus berstatus STARTED untuk menerima event"] = "Session must be STARTED to accept events",
        // Memperbarui `[”Session belum memiliki ruleset ACTIVE yang valid”]` menggunakan nilai literal `”Session does not have a valid ACTIVE ruleset”`.
        ["Session belum memiliki ruleset ACTIVE yang valid"] = "Session does not have a valid ACTIVE ruleset",
        // Memperbarui `[”Session belum memiliki ruleset aktif”]` menggunakan nilai literal `”Session does not have an active ruleset”`.
        ["Session belum memiliki ruleset aktif"] = "Session does not have an active ruleset",
        // Memperbarui `[”Session sudah berakhir”]` menggunakan nilai literal `”Session has already ended”`.
        ["Session sudah berakhir"] = "Session has already ended",
        // Memperbarui `[”Session tidak ditemukan”]` menggunakan nilai literal `”Session not found”`.
        ["Session tidak ditemukan"] = "Session not found",
        // Memperbarui `[”Setiap klaim pesanan harus diikuti pengambilan risiko pada mode MAHIR”]` menggunakan nilai literal `”Each order claim must be
        // followed by a risk draw in ADVANCED mode”`.
        ["Setiap klaim pesanan harus diikuti pengambilan risiko pada mode MAHIR"] = "Each order claim must be followed by a risk draw in ADVANCED mode",
        // Memperbarui `[”Status sesi tidak valid”]` menggunakan nilai literal `”Invalid session status”`.
        ["Status sesi tidak valid"] = "Invalid session status",
        // Memperbarui `[”State permainan hanya dapat diubah melalui event ingestion”]` menggunakan nilai literal `”Game state can only be changed through
        // event ingestion”`.
        ["State permainan hanya dapat diubah melalui event ingestion"] = "Game state can only be changed through event ingestion",
        // Memperbarui `[”State session tidak ditemukan”]` menggunakan nilai literal `”Session state not found”`.
        ["State session tidak ditemukan"] = "Session state not found",
        // Memperbarui `[”Terlalu banyak request”]` menggunakan nilai literal `”Too many requests”`.
        ["Terlalu banyak request"] = "Too many requests",
        // Memperbarui `[”Terjadi kesalahan pada server”]` menggunakan nilai literal `”An internal server error occurred”`.
        ["Terjadi kesalahan pada server"] = "An internal server error occurred",
        // Memperbarui `[”Tie breaker sudah ditetapkan untuk pemain”]` menggunakan nilai literal `”Tie-breaker has already been assigned to this player”`.
        ["Tie breaker sudah ditetapkan untuk pemain"] = "Tie-breaker has already been assigned to this player",
        // Memperbarui `[”Token user tidak valid”]` menggunakan nilai literal `”Invalid user token”`.
        ["Token user tidak valid"] = "Invalid user token",
        // Memperbarui `[”Total kartu bahan melebihi batas ruleset”]` menggunakan nilai literal `”Total ingredient cards exceed ruleset limit”`.
        ["Total kartu bahan melebihi batas ruleset"] = "Total ingredient cards exceed ruleset limit",
        // Memperbarui `[”Trade type tidak valid”]` menggunakan nilai literal `”Invalid trade type”`.
        ["Trade type tidak valid"] = "Invalid trade type",
        // Memperbarui `[”Action slot harus bernilai 1 atau 2”]` menggunakan nilai literal `”Action slot must be 1 or 2”`.
        ["Action slot harus bernilai 1 atau 2"] = "Action slot must be 1 or 2",
        // Memperbarui `[”Username atau password salah”]` menggunakan nilai literal `”Incorrect username or password”`.
        ["Username atau password salah"] = "Incorrect username or password",
        // Memperbarui `[”Username dan password wajib diisi”]` menggunakan nilai literal `”Username and password are required”`.
        ["Username dan password wajib diisi"] = "Username and password are required",
        // Memperbarui `[”Username harus 3-80 karakter”]` menggunakan nilai literal `”Username must be 3-80 characters”`.
        ["Username harus 3-80 karakter"] = "Username must be 3-80 characters",
        // Memperbarui `[”Username sudah digunakan”]` menggunakan nilai literal `”Username is already used”`.
        ["Username sudah digunakan"] = "Username is already used",
        // Memperbarui `[”Username wajib diisi”]` menggunakan nilai literal `”Username is required”`.
        ["Username wajib diisi"] = "Username is required",
        // Memperbarui `[”User ID atau username wajib diisi”]` menggunakan nilai literal `”User ID or username is required”`.
        ["User ID atau username wajib diisi"] = "User ID or username is required",
        // Memperbarui `[”Versi aktif tidak dapat dihapus. Aktifkan versi lain terlebih dahulu.”]` menggunakan nilai literal `”The active version cannot be
        // deleted. Activate another version first.”`.
        ["Versi aktif tidak dapat dihapus. Aktifkan versi lain terlebih dahulu."] = "The active version cannot be deleted. Activate another version first.",
        // Memperbarui `[”Versi ruleset sudah dipakai pada sesi/event sehingga tidak dapat dihapus.”]` menggunakan nilai literal `”The ruleset version is
        // already used by a session or event and cannot be deleted.”`.
        ["Versi ruleset sudah dipakai pada sesi/event sehingga tidak dapat dihapus."] = "The ruleset version is already used by a session or event and cannot be deleted.",
        // Memperbarui `[”Versi terakhir tidak dapat dihapus. Hapus ruleset jika tidak lagi diperlukan.”]` menggunakan nilai literal `”The last version
        // cannot be deleted. Delete the ruleset if it is no longer needed.”`.
        ["Versi terakhir tidak dapat dihapus. Hapus ruleset jika tidak lagi diperlukan."] = "The last version cannot be deleted. Delete the ruleset if it is no longer needed.",
        // Memperbarui `[”Weekday harus FRI”]` menggunakan nilai literal `”Weekday must be FRI”`.
        ["Weekday harus FRI"] = "Weekday must be FRI",
        // Memperbarui `[”Weekday harus SAT”]` menggunakan nilai literal `”Weekday must be SAT”`.
        ["Weekday harus SAT"] = "Weekday must be SAT",
        // Memperbarui `[”Weekday tidak valid”]` menggunakan nilai literal `”Invalid weekday”`.
        ["Weekday tidak valid"] = "Invalid weekday"
    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut.
    }.ToFrozenDictionary(StringComparer.Ordinal);

    /// <summary>
    /// Membangun objek <see cref="ErrorResponse"/> dengan pesan yang dilokalisasi sesuai preferensi bahasa klien.
    /// </summary>
    /// <param name="httpContext">Konteks HTTP untuk membaca Accept-Language dan trace identifier.</param>
    /// <param name="code">Kode error (misal VALIDATION_ERROR, DUPLICATE).</param>
    /// <param name="message">Pesan error dalam Bahasa Indonesia.</param>
    /// <param name="details">Detail error tambahan.</param>
    /// <returns>Objek ErrorResponse siap dikembalikan ke klien.</returns>
    // Mendefinisikan metode `BuildError` dengan hasil bertipe `ErrorResponse`. Membangun objek dengan pesan yang dilokalisasi sesuai preferensi bahasa
    // klien. Masukan: Parameter `httpContext` bertipe `HttpContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini;
    // Parameter `code` bertipe `string` membawa nilai kode; Parameter `message` bertipe `string` membawa nilai pesan; Parameter `details` bertipe
    // `ErrorDetail[]` membawa nilai rincian.
    internal static ErrorResponse BuildError(HttpContext httpContext, string code, string message, params ErrorDetail[] details)
    // Membuka scope metode BuildError; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildError.
    {
        // Menyiapkan variabel lokal `localizedMessage` untuk nilai localized pesan dengan memanggil `ResolveMessage` dengan `httpContext`, `message`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var localizedMessage = ResolveMessage(httpContext, message);
        // Mengembalikan objek baru bertipe `ErrorResponse` dengan argumen (code, localizedMessage, details.ToList(), httpContext.TraceIdentifier) kepada
        // pemanggil dalam BuildError; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new ErrorResponse(code, localizedMessage, details.ToList(), httpContext.TraceIdentifier);
    // Menutup scope metode BuildError; bagian berikut berada di luar batas blok tersebut dalam BuildError.
    }

    /// <summary>
    /// Menerjemahkan pesan ke Bahasa Inggris jika klien memilih Accept-Language: en.
    /// </summary>
    // Mendefinisikan metode `ResolveMessage` dengan hasil bertipe `string`. Menerjemahkan pesan ke Bahasa Inggris jika klien memilih Accept-Language:
    // en. Masukan: Parameter `context` bertipe `HttpContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini; Parameter
    // `message` bertipe `string` membawa nilai pesan.
    private static string ResolveMessage(HttpContext context, string message)
    // Membuka scope metode ResolveMessage; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveMessage.
    {
        // Memeriksa kebalikan kondisi `PrefersEnglish(context)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolveMessage.
        if (!PrefersEnglish(context))
        // Membuka scope cabang if untuk kondisi `!PrefersEnglish(context)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolveMessage.
        {
            // Mengembalikan `message` (nilai pesan) kepada pemanggil dalam ResolveMessage; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return message;
        // Menutup scope cabang if untuk kondisi `!PrefersEnglish(context)`; bagian berikut berada di luar batas blok tersebut dalam ResolveMessage.
        }

        // Mengembalikan hasil pemilihan bersyarat: ketika `IdToEnMessages.TryGetValue(message, out var translated)` benar gunakan `translated`, jika tidak
        // gunakan `message` kepada pemanggil dalam ResolveMessage; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return IdToEnMessages.TryGetValue(message, out var translated)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: translated dalam ResolveMessage.
            ? translated
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: message; dalam ResolveMessage.
            : message;
    // Menutup scope metode ResolveMessage; bagian berikut berada di luar batas blok tersebut dalam ResolveMessage.
    }

    /// <summary>
    /// Memeriksa apakah klien lebih memilih respons dalam Bahasa Inggris berdasarkan header Accept-Language.
    /// </summary>
    // Mendefinisikan metode `PrefersEnglish` dengan hasil bertipe `bool`. Memeriksa apakah klien lebih memilih respons dalam Bahasa Inggris berdasarkan
    // header Accept-Language. Masukan: Parameter `context` bertipe `HttpContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan
    // saat ini.
    private static bool PrefersEnglish(HttpContext context)
    // Membuka scope metode PrefersEnglish; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam PrefersEnglish.
    {
        // Menyiapkan variabel lokal `acceptLanguage` untuk nilai accept language dengan mengubah `context.Request.Headers.AcceptLanguage` menjadi teks.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var acceptLanguage = context.Request.Headers.AcceptLanguage.ToString();
        // Memeriksa memeriksa apakah `acceptLanguage` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam PrefersEnglish.
        if (string.IsNullOrWhiteSpace(acceptLanguage))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(acceptLanguage)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam PrefersEnglish.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam PrefersEnglish; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(acceptLanguage)`; bagian berikut berada di luar batas blok tersebut dalam
        // PrefersEnglish.
        }

        // Mengembalikan memeriksa apakah `acceptLanguage .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
        // .Select(segment => segment.Split(';', 2, StringSplitOptions.TrimEntries)[0])` memiliki setidaknya satu elemen yang memenuhi `code =>
        // code.StartsWith(”en”, StringComparison.OrdinalIgnoreCase)` kepada pemanggil dalam PrefersEnglish; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return acceptLanguage
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Split(',', StringSplitOptions.TrimEntries |
            // StringSplitOptions.RemoveEmptyEntries) dalam PrefersEnglish; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(segment => segment.Split(';', 2, StringSplitOptions.TrimEntries)[0])
            // dalam PrefersEnglish; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(segment => segment.Split(';', 2, StringSplitOptions.TrimEntries)[0])
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Any(code => code.StartsWith(”en”, StringComparison.OrdinalIgnoreCase)); dalam
            // PrefersEnglish; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Any(code => code.StartsWith("en", StringComparison.OrdinalIgnoreCase));
    // Menutup scope metode PrefersEnglish; bagian berikut berada di luar batas blok tersebut dalam PrefersEnglish.
    }
// Menutup scope tipe ApiErrorHelper; bagian berikut berada di luar batas blok tersebut.
}
