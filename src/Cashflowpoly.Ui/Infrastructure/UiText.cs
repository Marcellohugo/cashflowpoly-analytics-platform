// Fungsi file: Menyimpan seluruh kamus teks dwibahasa (ID/EN) untuk antarmuka pengguna dan menyediakan metode penerjemahan berdasarkan preferensi bahasa sesi.
using Cashflowpoly.Ui.Models;
using System.Collections.Frozen;

namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Kelas statis yang menyimpan kamus teks dwibahasa (Indonesia dan Inggris)
/// serta menyediakan metode penerjemahan untuk seluruh label, pesan, dan status UI.
/// </summary>
public static class UiText
{
    /// <summary>
    /// Kamus utama berisi pasangan kunci teks dan terjemahannya dalam Bahasa Indonesia (Id) dan Inggris (En),
    /// digunakan sebagai sumber tunggal untuk semua teks antarmuka pengguna.
    /// </summary>
    private static readonly FrozenDictionary<string, (string Id, string En)> Lexicon = UiTextLexicon.Build();

    /// <summary>
    /// Menormalisasi kode bahasa yang diberikan menjadi salah satu dari dua nilai valid: "en" atau "id".
    /// Jika input bukan "en", maka default ke Bahasa Indonesia ("id").
    /// </summary>
    /// <param name="language">Kode bahasa dari sesi pengguna, boleh null.</param>
    /// <returns>Kode bahasa yang sudah dinormalisasi ("en" atau "id").</returns>
    public static string NormalizeLanguage(string? language)
    {
        return string.Equals(language, AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase)
            ? AuthConstants.LanguageEn
            : AuthConstants.LanguageId;
    }

    /// <summary>
    /// Menerjemahkan status sesi permainan (CREATED, STARTED, ENDED, CANCELLED)
    /// ke teks lokal sesuai bahasa aktif pengguna.
    /// </summary>
    /// <param name="context">HttpContext yang berisi informasi sesi dan preferensi bahasa.</param>
    /// <param name="status">Status sesi mentah dari API, misalnya "CREATED" atau "STARTED".</param>
    /// <returns>Teks status sesi yang sudah diterjemahkan.</returns>
    public static string TranslateSessionStatus(HttpContext context, string? status)
    {
        var normalized = status?.Trim().ToUpperInvariant();
        return normalized switch
        {
            "CREATED" => Translate(context, "status.session.created"),
            "STARTED" => Translate(context, "status.session.started"),
            "ENDED" => Translate(context, "status.session.ended"),
            "CANCELLED" => Translate(context, "status.session.cancelled"),
            "CANCELED" => Translate(context, "status.session.cancelled"),
            _ => string.IsNullOrWhiteSpace(status) ? Translate(context, "status.session.unknown") : status!
        };
    }

    /// <summary>
    /// Menerjemahkan status set aturan (ACTIVE, DRAFT, RETIRED)
    /// ke teks lokal sesuai bahasa aktif pengguna.
    /// </summary>
    /// <param name="context">HttpContext yang berisi informasi sesi dan preferensi bahasa.</param>
    /// <param name="status">Status set aturan mentah dari API.</param>
    /// <returns>Teks status set aturan yang sudah diterjemahkan.</returns>
    public static string TranslateRulesetStatus(HttpContext context, string? status)
    {
        var normalized = status?.Trim().ToUpperInvariant();
        return normalized switch
        {
            "ACTIVE" => Translate(context, "status.ruleset.active"),
            "DRAFT" => Translate(context, "status.ruleset.draft"),
            "RETIRED" => Translate(context, "status.ruleset.retired"),
            _ => string.IsNullOrWhiteSpace(status) ? Translate(context, "status.ruleset.unknown") : status!
        };
    }

    /// <summary>
    /// Menerjemahkan mode set aturan (PEMULA/BEGINNER, MAHIR/ADVANCED)
    /// ke teks lokal sesuai bahasa aktif pengguna.
    /// </summary>
    /// <param name="context">HttpContext yang berisi informasi sesi dan preferensi bahasa.</param>
    /// <param name="mode">Mode set aturan mentah dari API, misalnya "PEMULA" atau "ADVANCED".</param>
    /// <returns>Teks mode set aturan yang sudah diterjemahkan.</returns>
    public static string TranslateRulesetMode(HttpContext context, string? mode)
    {
        var normalized = mode?.Trim().ToUpperInvariant();
        return normalized switch
        {
            "PEMULA" => Translate(context, "rulesets.form.mode_beginner"),
            "BEGINNER" => Translate(context, "rulesets.form.mode_beginner"),
            "MAHIR" => Translate(context, "rulesets.form.mode_advanced"),
            "ADVANCED" => Translate(context, "rulesets.form.mode_advanced"),
            _ => string.IsNullOrWhiteSpace(mode) ? "-" : mode!
        };
    }

    /// <summary>
    /// Menerjemahkan kunci teks ke bahasa yang aktif pada sesi pengguna saat ini.
    /// Jika kunci tidak ditemukan di Lexicon, mengembalikan kunci itu sendiri sebagai fallback.
    /// </summary>
    /// <param name="context">HttpContext yang berisi informasi sesi dan preferensi bahasa.</param>
    /// <param name="key">Kunci teks yang akan diterjemahkan, misalnya "nav.home" atau "auth.login".</param>
    /// <returns>Teks terjemahan sesuai bahasa aktif, atau kunci asli jika tidak ditemukan.</returns>
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

/// <summary>
/// Kelas ekstensi untuk HttpContext agar View dan Controller dapat memanggil
/// fungsi penerjemahan UiText secara singkat melalui metode ekstensi.
/// </summary>
public static class UiTextHttpContextExtensions
{
    /// <summary>
    /// Pintasan (shortcut) untuk menerjemahkan kunci teks ke bahasa aktif sesi pengguna.
    /// Memanggil <see cref="UiText.Translate"/> secara internal.
    /// </summary>
    /// <param name="context">HttpContext saat ini.</param>
    /// <param name="key">Kunci teks di kamus Lexicon, misalnya "nav.home".</param>
    /// <returns>Teks terjemahan sesuai bahasa aktif.</returns>
    public static string T(this HttpContext context, string key) => UiText.Translate(context, key);

    /// <summary>
    /// Pintasan untuk menerjemahkan status sesi permainan ke bahasa aktif.
    /// Memanggil <see cref="UiText.TranslateSessionStatus"/> secara internal.
    /// </summary>
    /// <param name="context">HttpContext saat ini.</param>
    /// <param name="status">Status sesi mentah dari API.</param>
    /// <returns>Teks status sesi yang sudah diterjemahkan.</returns>
    public static string TSessionStatus(this HttpContext context, string? status) => UiText.TranslateSessionStatus(context, status);

    /// <summary>
    /// Pintasan untuk menerjemahkan status set aturan ke bahasa aktif.
    /// Memanggil <see cref="UiText.TranslateRulesetStatus"/> secara internal.
    /// </summary>
    /// <param name="context">HttpContext saat ini.</param>
    /// <param name="status">Status set aturan mentah dari API.</param>
    /// <returns>Teks status set aturan yang sudah diterjemahkan.</returns>
    public static string TRulesetStatus(this HttpContext context, string? status) => UiText.TranslateRulesetStatus(context, status);

    /// <summary>
    /// Pintasan untuk menerjemahkan mode set aturan ke bahasa aktif.
    /// Memanggil <see cref="UiText.TranslateRulesetMode"/> secara internal.
    /// </summary>
    /// <param name="context">HttpContext saat ini.</param>
    /// <param name="mode">Mode set aturan mentah dari API.</param>
    /// <returns>Teks mode set aturan yang sudah diterjemahkan.</returns>
    public static string TRulesetMode(this HttpContext context, string? mode) => UiText.TranslateRulesetMode(context, mode);
}

