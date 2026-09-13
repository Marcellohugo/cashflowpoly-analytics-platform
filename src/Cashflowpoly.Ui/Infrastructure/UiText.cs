// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui UiText.
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;
// Mengimpor namespace `System.Collections.Frozen` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Collections.Frozen;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

public static class UiText
{
    private static readonly FrozenDictionary<string, (string Id, string En)> Lexicon = UiTextLexicon.Build();

    public static string NormalizeLanguage(string? language)
    {
        return string.Equals(language, AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: AuthConstants.LanguageEn dalam NormalizeLanguage.
            ? AuthConstants.LanguageEn
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: AuthConstants.LanguageId; dalam NormalizeLanguage.
            : AuthConstants.LanguageId;
    }

    public static string TranslateSessionStatus(string culture, string? status)
    {
        var normalized = status?.Trim().ToUpperInvariant();
        return normalized switch
        {
            // Untuk pola `”CREATED”`, menghasilkan memanggil `Translate` dengan `culture`, `”status.session.created”` sebagai hasil switch.
            "CREATED" => Translate(culture, "status.session.created"),
            // Untuk pola `”STARTED”`, menghasilkan memanggil `Translate` dengan `culture`, `”status.MulaiSesi”` sebagai hasil switch.
            "STARTED" => Translate(culture, "status.MulaiSesi"),
            // Untuk pola `”ENDED”`, menghasilkan memanggil `Translate` dengan `culture`, `”status.AkhiriSesi”` sebagai hasil switch.
            "ENDED" => Translate(culture, "status.AkhiriSesi"),
            // Untuk pola `”CANCELLED”`, menghasilkan memanggil `Translate` dengan `culture`, `”status.session.cancelled”` sebagai hasil switch.
            "CANCELLED" => Translate(culture, "status.session.cancelled"),
            // Untuk pola `”CANCELED”`, menghasilkan memanggil `Translate` dengan `culture`, `”status.session.cancelled”` sebagai hasil switch.
            "CANCELED" => Translate(culture, "status.session.cancelled"),
            // Untuk pola `_`, menghasilkan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(status)` benar gunakan `Translate(culture,
            // ”status.session.unknown”)`, jika tidak gunakan `status!` sebagai hasil switch.
            _ => string.IsNullOrWhiteSpace(status) ? Translate(culture, "status.session.unknown") : status!
        };
    }

    public static string TranslateRulesetStatus(string culture, string? status)
    {
        var normalized = status?.Trim().ToUpperInvariant();
        return normalized switch
        {
            // Untuk pola `”ACTIVE”`, menghasilkan memanggil `Translate` dengan `culture`, `”status.ruleset.active”` sebagai hasil switch.
            "ACTIVE" => Translate(culture, "status.ruleset.active"),
            // Untuk pola `”DRAFT”`, menghasilkan memanggil `Translate` dengan `culture`, `”status.ruleset.draft”` sebagai hasil switch.
            "DRAFT" => Translate(culture, "status.ruleset.draft"),
            // Untuk pola `”ARCHIVED”`, menghasilkan memanggil `Translate` dengan `culture`, `”status.ruleset.retired”` sebagai hasil switch.
            "ARCHIVED" => Translate(culture, "status.ruleset.retired"),
            // Untuk pola `_`, menghasilkan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(status)` benar gunakan `Translate(culture,
            // ”status.ruleset.unknown”)`, jika tidak gunakan `status!` sebagai hasil switch.
            _ => string.IsNullOrWhiteSpace(status) ? Translate(culture, "status.ruleset.unknown") : status!
        };
    }

    public static string TranslateRulesetMode(string culture, string? mode)
    {
        var normalized = mode?.Trim().ToUpperInvariant();
        return normalized switch
        {
            // Untuk pola `”PEMULA”`, menghasilkan memanggil `Translate` dengan `culture`, `”rulesets.form.mode_beginner”` sebagai hasil switch.
            "PEMULA" => Translate(culture, "rulesets.form.mode_beginner"),
            // Untuk pola `”BEGINNER”`, menghasilkan memanggil `Translate` dengan `culture`, `”rulesets.form.mode_beginner”` sebagai hasil switch.
            "BEGINNER" => Translate(culture, "rulesets.form.mode_beginner"),
            // Untuk pola `”MAHIR”`, menghasilkan memanggil `Translate` dengan `culture`, `”rulesets.form.mode_advanced”` sebagai hasil switch.
            "MAHIR" => Translate(culture, "rulesets.form.mode_advanced"),
            // Untuk pola `”ADVANCED”`, menghasilkan memanggil `Translate` dengan `culture`, `”rulesets.form.mode_advanced”` sebagai hasil switch.
            "ADVANCED" => Translate(culture, "rulesets.form.mode_advanced"),
            // Untuk pola `_`, menghasilkan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(mode)` benar gunakan `”-”`, jika tidak gunakan `mode!`
            // sebagai hasil switch.
            _ => string.IsNullOrWhiteSpace(mode) ? "-" : mode!
        };
    }

    public static string Translate(string culture, string key)
    {
        var normalized = NormalizeLanguage(culture);
        if (!Lexicon.TryGetValue(key, out var entry)) return key;
        return string.Equals(normalized, AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: entry.En dalam Translate.
            ? entry.En
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: entry.Id; dalam Translate.
            : entry.Id;
    }
}

public static class UiTextHttpContextExtensions
{
    public static string T(this HttpContext context, string key)
    {
        var culture = context.Session.GetString(AuthConstants.SessionLanguageKey) ?? AuthConstants.LanguageId;
        return UiText.Translate(culture, key);
    }

    public static string TSessionStatus(this HttpContext context, string? status)
    {
        var culture = context.Session.GetString(AuthConstants.SessionLanguageKey) ?? AuthConstants.LanguageId;
        return UiText.TranslateSessionStatus(culture, status);
    }

    public static string TRulesetStatus(this HttpContext context, string? status)
    {
        var culture = context.Session.GetString(AuthConstants.SessionLanguageKey) ?? AuthConstants.LanguageId;
        return UiText.TranslateRulesetStatus(culture, status);
    }

    public static string TRulesetMode(this HttpContext context, string? mode)
    {
        var culture = context.Session.GetString(AuthConstants.SessionLanguageKey) ?? AuthConstants.LanguageId;
        return UiText.TranslateRulesetMode(culture, mode);
    }
}

