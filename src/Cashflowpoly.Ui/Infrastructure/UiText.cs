using Cashflowpoly.Ui.Models;

namespace Cashflowpoly.Ui.Infrastructure;

public static class UiText
{
    private static readonly Dictionary<string, (string Id, string En)> Lexicon = new(StringComparer.OrdinalIgnoreCase)
    {
        ["brand.chip"] = ("Lembar Petualang", "Adventurer's Ledger"),

        ["nav.home"] = ("Home", "Home"),
        ["nav.sessions"] = ("Sesi", "Sessions"),
        ["nav.players"] = ("Pemain", "Players"),
        ["nav.analytics"] = ("Analitika", "Analytics"),
        ["nav.ruleset"] = ("Ruleset", "Rulesets"),
        ["nav.privacy"] = ("Rulebook", "Rulebook"),

        ["auth.login"] = ("Masuk", "Sign In"),
        ["auth.logout"] = ("Logout", "Logout"),
        ["auth.access"] = ("Akses Dashboard", "Dashboard Access"),
        ["auth.title"] = ("Masuk", "Sign In"),
        ["auth.subtitle"] = ("Masuk dengan akun instruktur atau player untuk melanjutkan.", "Sign in with an instructor or player account to continue."),
        ["auth.username"] = ("Username", "Username"),
        ["auth.password"] = ("Password", "Password"),
        ["auth.submit"] = ("Masuk", "Sign In"),
        ["auth.default_accounts"] = ("Akun default: instructor / instructor123 atau player / player123.", "Default accounts: instructor / instructor123 or player / player123."),
        ["auth.no_account"] = ("Belum punya akun?", "Don't have an account?"),
        ["auth.have_account"] = ("Sudah punya akun?", "Already have an account?"),
        ["auth.register"] = ("Daftar", "Register"),
        ["auth.register_title"] = ("Daftar Akun", "Register Account"),
        ["auth.register_subtitle"] = ("Buat akun baru agar bisa login ke game melalui API.", "Create a new account to sign in to the game through the API."),
        ["auth.confirm_password"] = ("Konfirmasi Password", "Confirm Password"),
        ["auth.role"] = ("Role", "Role"),
        ["auth.role_player"] = ("Player", "Player"),
        ["auth.role_instructor"] = ("Instruktur", "Instructor"),
        ["auth.submit_register"] = ("Buat Akun", "Create Account"),
        ["auth.error.login_required"] = ("Username dan password wajib diisi.", "Username and password are required."),
        ["auth.error.login_failed"] = ("Login gagal.", "Login failed."),
        ["auth.error.login_response_invalid"] = ("Respons login tidak valid.", "Invalid login response."),
        ["auth.error.register_required"] = ("Username, password, dan konfirmasi password wajib diisi.", "Username, password, and confirmation are required."),
        ["auth.error.role_invalid"] = ("Role tidak valid.", "Invalid role."),
        ["auth.error.confirm_mismatch"] = ("Konfirmasi password tidak sama.", "Password confirmation does not match."),
        ["auth.error.register_failed"] = ("Registrasi gagal.", "Registration failed."),
        ["auth.error.register_response_invalid"] = ("Respons registrasi tidak valid.", "Invalid registration response."),

        ["lang.toggle"] = ("EN", "ID"),
        ["lang.label"] = ("Bahasa", "Language"),
        ["lang.option.id"] = ("Bahasa Indonesia (ID)", "Bahasa Indonesia (ID)"),
        ["lang.option.en"] = ("English (EN)", "English (EN)"),

        ["profile.settings"] = ("Pengaturan Web", "Web Settings"),
        ["profile.role"] = ("Peran", "Role"),

        ["home.badge"] = ("Cashflowpoly", "Cashflowpoly"),
        ["home.title"] = ("Dasbor analitika dan manajemen ruleset.", "Analytics dashboard and ruleset management."),
        ["home.subtitle"] = ("Pantau event, proyeksi arus kas, dan ringkasan performa pemain dengan validasi aturan yang rapi.", "Track events, cashflow projections, and player performance summaries with strict rule validation."),
        ["home.cta.sessions"] = ("Mulai dari sesi", "Start from sessions"),
        ["home.cta.analytics"] = ("Lihat analitika", "View analytics"),
        ["home.feature.1"] = ("Validasi rulebook dan skor otomatis.", "Automatic rulebook validation and scoring."),
        ["home.feature.2"] = ("Aktivasi ruleset per sesi dengan cepat.", "Quick per-session ruleset activation."),
        ["home.feature.3"] = ("Snapshot arus kas dan pelanggaran.", "Cashflow and violation snapshots."),
        ["home.feature.4"] = ("Ringkasan pemain siap untuk evaluasi.", "Player summaries ready for evaluation."),
        ["home.active_sessions"] = ("Sesi aktif", "Active sessions"),
        ["home.realtime"] = ("Real-time", "Real-time"),
        ["home.ruleset_active"] = ("Ruleset aktif", "Active ruleset"),
        ["home.players_monitored"] = ("Pemain dipantau", "Players monitored"),
        ["home.snapshot_demo"] = ("Snapshot singkat untuk demo tampilan.", "Quick snapshot for UI demo."),

        ["privacy.title"] = ("Rulebook Permainan", "Game Rulebook"),
        ["privacy.subtitle"] = ("", ""),

        ["analytics.dashboard"] = ("Dashboard Analitika", "Analytics Dashboard"),
        ["analytics.title"] = ("Cari analitika sesi", "Find session analytics"),
        ["analytics.subtitle"] = ("Masukkan ID sesi untuk melihat ringkasan dan performa pemain.", "Enter a session ID to see summary and player performance."),
        ["analytics.button"] = ("Lihat Analitika", "Show Analytics"),

        ["sessions.management"] = ("Manajemen Sesi", "Session Management"),
        ["sessions.title"] = ("Daftar sesi", "Session list"),
        ["sessions.subtitle"] = ("Pantau sesi aktif dan akses detail analitika.", "Monitor active sessions and open analytics details."),
        ["sessions.live_log"] = ("Live session log", "Live session log"),
        ["sessions.detail_title"] = ("Detail sesi", "Session details"),
        ["sessions.analytics_title"] = ("Analitika Sesi", "Session Analytics"),
        ["sessions.activate_ruleset"] = ("Aktivasi Ruleset", "Activate Ruleset"),
        ["sessions.ruleset_title"] = ("Pilih ruleset untuk sesi", "Select a ruleset for session"),
        ["sessions.ruleset_hint"] = ("Isi versi manual (mis. 1 atau versi terbaru).", "Enter version manually (e.g., 1 or latest)."),
        ["sessions.activate"] = ("Aktifkan", "Activate"),

        ["players.management"] = ("Manajemen Pemain", "Player Management"),
        ["players.title"] = ("Daftar pemain", "Player list"),
        ["players.add"] = ("Tambah", "Add"),
        ["players.name_placeholder"] = ("Nama pemain", "Player name"),
        ["players.detail_title"] = ("Detail pemain", "Player details"),
        ["players.analytics_title"] = ("Analitika Pemain", "Player Analytics"),
        ["players.no_transactions"] = ("Belum ada transaksi.", "No transactions yet."),

        ["rulesets.management"] = ("Manajemen Ruleset", "Ruleset Management"),
        ["rulesets.title"] = ("Daftar ruleset", "Ruleset list"),
        ["rulesets.create"] = ("Buat ruleset", "Create ruleset"),
        ["rulesets.create_title"] = ("Buat ruleset baru", "Create new ruleset"),
        ["rulesets.create_subtitle"] = ("Isi konfigurasi JSON sesuai spesifikasi ruleset.", "Fill JSON config according to the ruleset specification."),
        ["rulesets.detail_title"] = ("Detail ruleset", "Ruleset details"),
        ["rulesets.archive"] = ("Arsipkan", "Archive"),
        ["rulesets.delete"] = ("Hapus", "Delete"),
        ["rulesets.save"] = ("Simpan Ruleset", "Save Ruleset"),

        ["common.name"] = ("Nama", "Name"),
        ["common.description"] = ("Deskripsi", "Description"),
        ["common.status"] = ("Status", "Status"),
        ["common.created"] = ("Dibuat", "Created"),
        ["common.mode"] = ("Mode", "Mode"),
        ["common.action"] = ("Aksi", "Action"),
        ["common.detail"] = ("Detail", "Detail"),
        ["common.view"] = ("Lihat", "View"),
        ["common.cancel"] = ("Batal", "Cancel"),
        ["common.version"] = ("Versi", "Version"),
        ["common.latest_version"] = ("Versi Terbaru", "Latest Version"),
        ["common.player_id"] = ("Player ID", "Player ID"),
        ["common.session_id"] = ("Session ID", "Session ID"),
        ["common.ruleset_id"] = ("Ruleset ID", "Ruleset ID"),
        ["common.ruleset"] = ("Ruleset", "Ruleset"),
        ["common.transaction_amount"] = ("Nominal", "Amount"),
        ["common.time"] = ("Waktu", "Time"),
        ["common.direction"] = ("Arah", "Direction"),
        ["common.category"] = ("Kategori", "Category"),
        ["common.computed"] = ("Dihitung", "Computed"),

        ["metric.total_event"] = ("Total Event", "Total Events"),
        ["metric.cash_in"] = ("Cash In", "Cash In"),
        ["metric.cash_out"] = ("Cash Out", "Cash Out"),
        ["metric.net_cashflow"] = ("Net Cashflow", "Net Cashflow"),
        ["metric.violations"] = ("Pelanggaran", "Violations"),
        ["metric.donation"] = ("Donation", "Donation"),
        ["metric.gold_qty"] = ("Gold Qty", "Gold Qty"),
        ["metric.happiness"] = ("Happiness", "Happiness"),
        ["metric.need_points"] = ("Need Points", "Need Points"),
        ["metric.need_bonus"] = ("Need Bonus", "Need Bonus"),
        ["metric.donation_points"] = ("Donation Points", "Donation Points"),
        ["metric.gold_points"] = ("Gold Points", "Gold Points"),
        ["metric.pension_points"] = ("Pension Points", "Pension Points"),
        ["metric.saving_goal"] = ("Saving Goal", "Saving Goal"),
        ["metric.loan_unpaid"] = ("Loan Unpaid", "Loan Unpaid"),
        ["metric.mission_penalty"] = ("Mission Penalty", "Mission Penalty"),
        ["metric.loan_penalty"] = ("Loan Penalty", "Loan Penalty"),

        ["gameplay.title"] = ("Snapshot gameplay & turunan", "Gameplay snapshot & derived"),
        ["gameplay.raw"] = ("Raw Gameplay Variables", "Raw Gameplay Variables"),
        ["gameplay.derived"] = ("Derived Metrics", "Derived Metrics"),

        ["state.no_sessions"] = ("Belum ada sesi.", "No sessions yet."),
        ["state.no_players"] = ("Belum ada pemain.", "No players yet."),
        ["state.no_player_data"] = ("Belum ada data pemain.", "No player data yet."),
        ["state.no_rulesets"] = ("Belum ada ruleset.", "No rulesets yet."),
        ["state.no_versions"] = ("Belum ada versi.", "No versions yet."),
        ["state.true"] = ("Ya", "Yes"),
        ["state.false"] = ("Tidak", "No")
    };

    public static string NormalizeLanguage(string? language)
    {
        return string.Equals(language, AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase)
            ? AuthConstants.LanguageEn
            : AuthConstants.LanguageId;
    }

    public static string Translate(HttpContext context, string key)
    {
        var lang = NormalizeLanguage(context.Session.GetString(AuthConstants.SessionLanguageKey));
        if (!Lexicon.TryGetValue(key, out var term))
        {
            return key;
        }

        return lang == AuthConstants.LanguageEn ? term.En : term.Id;
    }
}

public static class UiTextHttpContextExtensions
{
    public static string T(this HttpContext context, string key) => UiText.Translate(context, key);
}
