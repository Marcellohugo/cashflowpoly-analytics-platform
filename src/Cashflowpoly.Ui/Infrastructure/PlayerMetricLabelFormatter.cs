// Fungsi file: Memformat label metrik dan detail transaksi pemain untuk halaman detail.
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Formatter label metrik pemain yang tidak bergantung pada Razor runtime.
/// </summary>
public static class PlayerMetricLabelFormatter
{
    /// <summary>
    /// Mengubah key metrik menjadi label manusiawi dengan dukungan lokalisasi.
    /// </summary>
    public static string HumanizeMetricKey(string key, Func<string, string> translate)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return translate("players.details.metric_fallback");
        }

        if (string.Equals(key, "value", StringComparison.OrdinalIgnoreCase))
        {
            return translate("common.value");
        }

        if (string.Equals(key, "series", StringComparison.OrdinalIgnoreCase))
        {
            return translate("players.details.series");
        }

        var normalized = key.Trim().ToLowerInvariant();
        var aliasLexiconKey = normalized switch
        {
            "coins_net_end" => "players.raw.coins_net_end_game",
            "coins_net_end_game" => "players.raw.coins_net_end_game",
            "meal_orders_available_passed" => "players.raw.meal_orders_passed",
            _ => string.Empty
        };

        var localizedLabel =
            TryTranslateMetricLexicon(aliasLexiconKey, translate) ??
            TryTranslateMetricLexicon($"players.raw.{normalized}", translate) ??
            TryTranslateMetricLexicon($"players.group.{normalized}", translate) ??
            TryTranslateMetricLexicon($"players.metric.{normalized}", translate);
        if (!string.IsNullOrWhiteSpace(localizedLabel))
        {
            return localizedLabel;
        }

        var text = key.Replace('_', ' ').Trim();
        if (text.Length == 0)
        {
            return translate("players.details.metric_fallback");
        }

        return char.ToUpperInvariant(text[0]) + text[1..];
    }

    /// <summary>
    /// Mengubah path metrik bertingkat menjadi label bersegmen.
    /// </summary>
    public static string FormatMetricPathLabel(string rawPath, Func<string, string> translate)
    {
        if (string.IsNullOrWhiteSpace(rawPath))
        {
            return translate("players.details.metric_fallback");
        }

        var tokens = new List<string>();
        var buffer = new StringBuilder();

        void FlushBuffer()
        {
            if (buffer.Length == 0)
            {
                return;
            }

            tokens.Add(HumanizeMetricKey(buffer.ToString(), translate));
            buffer.Clear();
        }

        for (var index = 0; index < rawPath.Length; index++)
        {
            var current = rawPath[index];
            if (current == '.')
            {
                FlushBuffer();
                continue;
            }

            if (current == '[')
            {
                FlushBuffer();

                var closeIndex = rawPath.IndexOf(']', index);
                if (closeIndex > index)
                {
                    var indexToken = rawPath.Substring(index + 1, closeIndex - index - 1);
                    if (int.TryParse(indexToken, out var parsedIndex))
                    {
                        tokens.Add($"{translate("players.details.item")} {parsedIndex + 1}");
                    }
                    else if (!string.IsNullOrWhiteSpace(indexToken))
                    {
                        tokens.Add($"{translate("players.details.item")} {indexToken}");
                    }

                    index = closeIndex;
                    continue;
                }
            }

            buffer.Append(current);
        }

        FlushBuffer();

        var displayTokens = tokens
            .Where(token => !string.IsNullOrWhiteSpace(token))
            .ToList();

        if (displayTokens.Count == 0)
        {
            return HumanizeMetricKey(rawPath, translate);
        }

        return string.Join(" - ", displayTokens);
    }

    /// <summary>
    /// Mengubah kategori transaksi menjadi label lokal.
    /// </summary>
    public static string HumanizeTransactionCategory(string category, Func<string, string> translate)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return translate("players.details.transaction_label");
        }

        var normalized = category.Trim().ToUpperInvariant();
        var mapped = normalized switch
        {
            "DONATION" => translate("players.details.transaction.category.donation"),
            "GOLD_TRADE" => translate("players.details.transaction.category.gold_trade"),
            "INGREDIENT" => translate("players.details.transaction.category.ingredient"),
            "ORDER" => translate("players.details.transaction.category.order"),
            "FREELANCE" => translate("players.details.transaction.category.freelance"),
            "NEED_PRIMARY" => translate("players.details.transaction.category.need_primary"),
            "NEED_SECONDARY" => translate("players.details.transaction.category.need_secondary"),
            "NEED_TERTIARY" => translate("players.details.transaction.category.need_tertiary"),
            "SAVING_DEPOSIT" => translate("players.details.transaction.category.saving_deposit"),
            "SAVING_WITHDRAW" => translate("players.details.transaction.category.saving_withdraw"),
            "RISK_LIFE" => translate("players.details.transaction.category.risk_life"),
            "LOAN_TAKEN" => translate("players.details.transaction.category.loan_taken"),
            "LOAN_REPAID" => translate("players.details.transaction.category.loan_repaid"),
            "INSURANCE_PREMIUM" => translate("players.details.transaction.category.insurance_premium"),
            "INSURANCE_CLAIM" => translate("players.details.transaction.category.insurance_claim"),
            "EMERGENCY_OPTION" => translate("players.details.transaction.category.emergency_option"),
            _ => string.Empty
        };

        if (!string.IsNullOrWhiteSpace(mapped))
        {
            return mapped;
        }

        return HumanizeMetricKey(category.Trim().ToLowerInvariant(), translate);
    }

    /// <summary>
    /// Melokalkan detail transaksi berformat "DIRECTION - CATEGORY (amount)".
    /// </summary>
    public static string LocalizeTransactionDetail(string rawDetail, Func<string, string> translate)
    {
        if (string.IsNullOrWhiteSpace(rawDetail))
        {
            return translate("players.details.transaction_label");
        }

        var text = rawDetail.Trim();
        var match = Regex.Match(text, @"^(?<dir>[A-Za-z_]+)\s*-\s*(?<cat>.+?)(?:\s*\((?<amt>[^)]+)\))?$");
        if (!match.Success)
        {
            return text;
        }

        var rawDirection = match.Groups["dir"].Value.Trim();
        var rawCategory = match.Groups["cat"].Value.Trim();
        var amountText = match.Groups["amt"].Success ? match.Groups["amt"].Value.Trim() : string.Empty;

        if (rawDirection.Equals("START", StringComparison.OrdinalIgnoreCase))
        {
            return string.IsNullOrWhiteSpace(amountText)
                ? translate("players.details.transaction.opening_cash")
                : string.Format(
                    CultureInfo.CurrentCulture,
                    translate("players.details.transaction.opening_cash_with_amount"),
                    amountText);
        }

        var directionText = rawDirection.Equals("IN", StringComparison.OrdinalIgnoreCase)
            ? translate("players.details.transaction.cash_in")
            : rawDirection.Equals("OUT", StringComparison.OrdinalIgnoreCase)
                ? translate("players.details.transaction.cash_out")
                : HumanizeMetricKey(rawDirection.ToLowerInvariant(), translate);
        var categoryText = HumanizeTransactionCategory(rawCategory, translate);

        return string.IsNullOrWhiteSpace(amountText)
            ? $"{directionText} - {categoryText}"
            : $"{directionText} - {categoryText} ({amountText})";
    }

    /// <summary>
    /// Mencoba membaca teks metrik menjadi angka untuk chart.
    /// </summary>
    public static bool TryParseMetricNumber(string rawValue, out double numericValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            numericValue = 0;
            return false;
        }

        if (string.Equals(rawValue, "true", StringComparison.OrdinalIgnoreCase))
        {
            numericValue = 1;
            return true;
        }

        if (string.Equals(rawValue, "false", StringComparison.OrdinalIgnoreCase))
        {
            numericValue = 0;
            return true;
        }

        if (double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out numericValue))
        {
            return true;
        }

        if (double.TryParse(rawValue, NumberStyles.Float, CultureInfo.CurrentCulture, out numericValue))
        {
            return true;
        }

        numericValue = 0;
        return false;
    }

    /// <summary>
    /// Mengklasifikasikan path scalar compact untuk ringkasan gabungan utama.
    /// </summary>
    public static bool IsPreferredCombinedSummaryPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        return !path.Contains('[') && !path.Contains('.');
    }

    /// <summary>
    /// Mengklasifikasikan path fallback ringkasan gabungan yang masih menolak array.
    /// </summary>
    public static bool IsFallbackCombinedSummaryPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        return !path.Contains('[');
    }

    private static string? TryTranslateMetricLexicon(string lexiconKey, Func<string, string> translate)
    {
        if (string.IsNullOrWhiteSpace(lexiconKey))
        {
            return null;
        }

        var translated = translate(lexiconKey);
        if (string.Equals(translated, lexiconKey, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return translated;
    }
}
