using Cashflowpoly.Ui.Models;
using System.Collections.Frozen;

namespace Cashflowpoly.Ui.Infrastructure;

public static class UiText
{
    private static readonly FrozenDictionary<string, (string Id, string En)> Lexicon = UiTextLexicon.Build();

    public static string NormalizeLanguage(string? language)
    {
        return string.Equals(language, AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase)
            ? AuthConstants.LanguageEn
            : AuthConstants.LanguageId;
    }

    public static string TranslateSessionStatus(string culture, string? status)
    {
        var normalized = status?.Trim().ToUpperInvariant();
        return normalized switch
        {
            "CREATED" => Translate(culture, "status.session.created"),
            "STARTED" => Translate(culture, "status.session.started"),
            "ENDED" => Translate(culture, "status.session.ended"),
            "CANCELLED" => Translate(culture, "status.session.cancelled"),
            "CANCELED" => Translate(culture, "status.session.cancelled"),
            _ => string.IsNullOrWhiteSpace(status) ? Translate(culture, "status.session.unknown") : status!
        };
    }

    public static string TranslateRulesetStatus(string culture, string? status)
    {
        var normalized = status?.Trim().ToUpperInvariant();
        return normalized switch
        {
            "ACTIVE" => Translate(culture, "status.ruleset.active"),
            "DRAFT" => Translate(culture, "status.ruleset.draft"),
            "RETIRED" => Translate(culture, "status.ruleset.retired"),
            _ => string.IsNullOrWhiteSpace(status) ? Translate(culture, "status.ruleset.unknown") : status!
        };
    }

    public static string TranslateRulesetMode(string culture, string? mode)
    {
        var normalized = mode?.Trim().ToUpperInvariant();
        return normalized switch
        {
            "PEMULA" => Translate(culture, "rulesets.form.mode_beginner"),
            "BEGINNER" => Translate(culture, "rulesets.form.mode_beginner"),
            "MAHIR" => Translate(culture, "rulesets.form.mode_advanced"),
            "ADVANCED" => Translate(culture, "rulesets.form.mode_advanced"),
            _ => string.IsNullOrWhiteSpace(mode) ? "-" : mode!
        };
    }

    public static string Translate(string culture, string key)
    {
        var normalized = NormalizeLanguage(culture);
        if (!Lexicon.TryGetValue(key, out var entry)) return key;
        return string.Equals(normalized, AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase)
            ? entry.En
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

