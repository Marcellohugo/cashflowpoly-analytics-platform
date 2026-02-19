// Fungsi file: Menyediakan utilitas infrastruktur UI untuk kebutuhan SessionTimelineMapper.
using System.Text.Json;
using System.Globalization;
using Cashflowpoly.Ui.Models;

namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Menyatakan peran utama tipe SessionTimelineMapper pada modul ini.
/// </summary>
public static class SessionTimelineMapper
{
    /// <summary>
    /// Menjalankan fungsi MapTimeline sebagai bagian dari alur file ini.
    /// </summary>
    public static List<SessionTimelineEventViewModel> MapTimeline(List<EventRequestDto>? events, string? language = null)
    {
        if (events is null || events.Count == 0)
        {
            return new List<SessionTimelineEventViewModel>();
        }

        var normalizedLanguage = UiText.NormalizeLanguage(language);

        return events
            .OrderBy(item => item.Timestamp)
            .ThenBy(item => item.SequenceNumber)
            .Select(item => new SessionTimelineEventViewModel
            {
                Timestamp = item.Timestamp,
                SequenceNumber = item.SequenceNumber,
                DayIndex = item.DayIndex,
                Weekday = item.Weekday,
                TurnNumber = item.TurnNumber < 1 ? 1 : item.TurnNumber,
                ActorType = item.ActorType,
                PlayerId = item.PlayerId,
                ActionType = item.ActionType,
                FlowLabel = ResolveFlowLabel(item.ActionType, normalizedLanguage),
                FlowDescription = BuildFlowDescription(item.ActionType, item.Payload, normalizedLanguage)
            })
            .ToList();
    }

    /// <summary>
    /// Menjalankan fungsi ApplyPlayerDisplayNames sebagai bagian dari alur file ini.
    /// </summary>
    public static void ApplyPlayerDisplayNames(
        IEnumerable<SessionTimelineEventViewModel> timeline,
        IReadOnlyDictionary<Guid, string> playerDisplayNames)
    {
        foreach (var item in timeline.Where(item => item.PlayerId.HasValue))
        {
            if (item.PlayerId.HasValue &&
                playerDisplayNames.TryGetValue(item.PlayerId.Value, out var displayName) &&
                !string.IsNullOrWhiteSpace(displayName))
            {
                item.PlayerDisplayName = displayName;
            }
        }
    }

    /// <summary>
    /// Menjalankan fungsi ResolveFlowLabel sebagai bagian dari alur file ini.
    /// </summary>
    private static string ResolveFlowLabel(string actionType, string language)
    {
        if (string.IsNullOrWhiteSpace(actionType))
        {
            return L(language, "Aktivitas", "Activity");
        }

        var lower = actionType.ToLowerInvariant();
        if (lower.StartsWith("setup."))
        {
            return L(language, "Setup", "Setup");
        }

        if (lower.StartsWith("day."))
        {
            return L(language, "Event Harian", "Daily Event");
        }

        if (lower.StartsWith("risk."))
        {
            return L(language, "Risiko", "Risk");
        }

        if (lower.StartsWith("loan."))
        {
            return L(language, "Pembiayaan", "Financing");
        }

        if (lower.StartsWith("saving."))
        {
            return L(language, "Tabungan", "Saving");
        }

        if (lower.StartsWith("mission."))
        {
            return L(language, "Misi", "Mission");
        }

        if (lower.Contains("order"))
        {
            return L(language, "Pesanan", "Order");
        }

        if (lower.Contains("purchased"))
        {
            return L(language, "Pembelian", "Purchase");
        }

        return L(language, "Aktivitas", "Activity");
    }

    /// <summary>
    /// Menjalankan fungsi BuildFlowDescription sebagai bagian dari alur file ini.
    /// </summary>
    private static string BuildFlowDescription(string actionType, JsonElement payload, string language)
    {
        var text = actionType switch
        {
            "transaction.recorded" => DescribeTransaction(payload, language),
            "day.friday.donation" => DescribeFridayDonation(payload, language),
            "day.saturday.gold_trade" => DescribeGoldTrade(payload, language),
            "ingredient.purchased" => DescribeIngredientPurchase(payload, language),
            "ingredient.discarded" => DescribeIngredientDiscard(payload, language),
            "order.claimed" => DescribeOrderClaim(payload, language),
            "order.passed" => DescribeOrderPassed(payload, language),
            "work.freelance.completed" => DescribeFreelance(payload, language),
            "need.primary.purchased" => DescribeNeedPurchase(payload, language, "kebutuhan primer", "primary need"),
            "need.secondary.purchased" => DescribeNeedPurchase(payload, language, "kebutuhan sekunder", "secondary need"),
            "need.tertiary.purchased" => DescribeNeedPurchase(payload, language, "kebutuhan tersier", "tertiary need"),
            "saving.deposit.created" => DescribeSavingDeposit(payload, language, isWithdrawn: false),
            "saving.deposit.withdrawn" => DescribeSavingDeposit(payload, language, isWithdrawn: true),
            "saving.goal.achieved" => DescribeSavingGoalAchieved(payload, language),
            "loan.syariah.taken" => DescribeLoanTaken(payload, language),
            "loan.syariah.repaid" => DescribeLoanRepaid(payload, language),
            "risk.life.drawn" => DescribeRiskLife(payload, language),
            "risk.emergency.used" => DescribeRiskEmergency(payload, language),
            "insurance.multirisk.purchased" => DescribeInsurancePurchase(payload, language),
            "insurance.multirisk.used" => DescribeInsuranceUse(payload, language),
            "donation.rank.awarded" => DescribeRankAward(payload, language, "donasi", "donation"),
            "pension.rank.awarded" => DescribeRankAward(payload, language, "dana pensiun", "pension"),
            "gold.points.awarded" => DescribePointsAward(payload, language, "emas", "gold"),
            "turn.action.used" => DescribeTurnAction(payload, language),
            _ => BuildGenericDescription(actionType, payload, language)
        };

        return text;
    }

    /// <summary>
    /// Menjalankan fungsi DescribeTransaction sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeTransaction(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "direction", out var direction))
        {
            return BuildGenericDescription("transaction.recorded", payload, language);
        }

        var directionLabel = direction.Equals("IN", StringComparison.OrdinalIgnoreCase)
            ? L(language, "Kas masuk", "Cash in")
            : direction.Equals("OUT", StringComparison.OrdinalIgnoreCase)
                ? L(language, "Kas keluar", "Cash out")
                : direction.ToUpperInvariant();

        var amountText = TryGetNumber(payload, "amount", out var amount)
            ? FormatNumber(amount)
            : L(language, "nominal tidak diketahui", "unknown amount");

        var categoryText = TryGetString(payload, "category", out var category)
            ? category
            : L(language, "kategori tidak diketahui", "unknown category");

        var counterpartyText = TryGetString(payload, "counterparty", out var counterparty)
            ? L(language, $" dengan pihak {counterparty}", $" with counterparty {counterparty}")
            : string.Empty;

        return L(
            language,
            $"{directionLabel} sebesar {amountText} pada kategori {categoryText}{counterpartyText}.",
            $"{directionLabel} {amountText} in category {categoryText}{counterpartyText}.");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeFridayDonation sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeFridayDonation(JsonElement payload, string language)
    {
        if (!TryGetNumber(payload, "amount", out var amount))
        {
            return BuildGenericDescription("day.friday.donation", payload, language);
        }

        return L(
            language,
            $"Melakukan donasi Jumat sebesar {FormatNumber(amount)}.",
            $"Made a Friday donation of {FormatNumber(amount)}.");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeGoldTrade sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeGoldTrade(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "trade_type", out var tradeType) ||
            !TryGetInt(payload, "qty", out var qty) ||
            !TryGetInt(payload, "unit_price", out var unitPrice))
        {
            return BuildGenericDescription("day.saturday.gold_trade", payload, language);
        }

        var amount = TryGetInt(payload, "amount", out var parsedAmount)
            ? parsedAmount
            : qty * unitPrice;
        var action = tradeType.Equals("BUY", StringComparison.OrdinalIgnoreCase)
            ? L(language, "Membeli", "Bought")
            : tradeType.Equals("SELL", StringComparison.OrdinalIgnoreCase)
                ? L(language, "Menjual", "Sold")
                : L(language, "Melakukan transaksi", "Executed trade");

        return L(
            language,
            $"{action} emas {qty} unit x {FormatNumber(unitPrice)} (total {FormatNumber(amount)}).",
            $"{action} {qty} gold unit(s) x {FormatNumber(unitPrice)} (total {FormatNumber(amount)}).");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeIngredientPurchase sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeIngredientPurchase(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "card_id", out var cardId))
        {
            return BuildGenericDescription("ingredient.purchased", payload, language);
        }

        var amountText = TryGetNumber(payload, "amount", out var amount)
            ? FormatNumber(amount)
            : L(language, "nominal tidak diketahui", "unknown amount");

        return L(
            language,
            $"Membeli bahan {cardId} dengan biaya {amountText}.",
            $"Purchased ingredient {cardId} with cost {amountText}.");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeIngredientDiscard sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeIngredientDiscard(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "card_id", out var cardId))
        {
            return BuildGenericDescription("ingredient.discarded", payload, language);
        }

        var amountText = TryGetNumber(payload, "amount", out var amount)
            ? FormatNumber(amount)
            : L(language, "jumlah tidak diketahui", "unknown quantity");

        return L(
            language,
            $"Membuang bahan {cardId} sebanyak {amountText}.",
            $"Discarded ingredient {cardId} with quantity {amountText}.");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeOrderClaim sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeOrderClaim(JsonElement payload, string language)
    {
        var incomeText = TryGetNumber(payload, "income", out var income)
            ? FormatNumber(income)
            : L(language, "nominal tidak diketahui", "unknown amount");
        var ingredientCountText = TryGetArrayCount(payload, "required_ingredient_card_ids", out var count)
            ? count.ToString(CultureInfo.InvariantCulture)
            : L(language, "?", "?");

        return L(
            language,
            $"Menyelesaikan order dengan {ingredientCountText} bahan dan menerima pemasukan {incomeText}.",
            $"Claimed an order using {ingredientCountText} ingredient(s) and received {incomeText} income.");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeOrderPassed sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeOrderPassed(JsonElement payload, string language)
    {
        var ingredientCountText = TryGetArrayCount(payload, "required_ingredient_card_ids", out var count)
            ? count.ToString(CultureInfo.InvariantCulture)
            : L(language, "?", "?");
        var incomeText = TryGetNumber(payload, "income", out var income)
            ? FormatNumber(income)
            : L(language, "nominal tidak diketahui", "unknown amount");

        return L(
            language,
            $"Melewati order yang membutuhkan {ingredientCountText} bahan (potensi pemasukan {incomeText}).",
            $"Passed an order requiring {ingredientCountText} ingredient(s) (potential income {incomeText}).");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeFreelance sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeFreelance(JsonElement payload, string language)
    {
        if (!TryGetNumber(payload, "amount", out var amount))
        {
            return BuildGenericDescription("work.freelance.completed", payload, language);
        }

        return L(
            language,
            $"Menyelesaikan pekerjaan freelance dengan pemasukan {FormatNumber(amount)}.",
            $"Completed freelance work and earned {FormatNumber(amount)}.");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeNeedPurchase sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeNeedPurchase(JsonElement payload, string language, string needTypeId, string needTypeEn)
    {
        if (!TryGetString(payload, "card_id", out var cardId))
        {
            return BuildGenericDescription("need.purchased", payload, language);
        }

        var amountText = TryGetNumber(payload, "amount", out var amount)
            ? FormatNumber(amount)
            : L(language, "nominal tidak diketahui", "unknown amount");
        var pointsText = TryGetNumber(payload, "points", out var points)
            ? FormatNumber(points)
            : L(language, "poin tidak diketahui", "unknown points");

        return L(
            language,
            $"Membeli {needTypeId} {cardId} (biaya {amountText}, poin {pointsText}).",
            $"Purchased {needTypeEn} card {cardId} (cost {amountText}, points {pointsText}).");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeSavingDeposit sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeSavingDeposit(JsonElement payload, string language, bool isWithdrawn)
    {
        if (!TryGetString(payload, "goal_id", out var goalId) || !TryGetNumber(payload, "amount", out var amount))
        {
            return BuildGenericDescription(isWithdrawn ? "saving.deposit.withdrawn" : "saving.deposit.created", payload, language);
        }

        return isWithdrawn
            ? L(
                language,
                $"Menarik {FormatNumber(amount)} dari tabungan tujuan {goalId}.",
                $"Withdrew {FormatNumber(amount)} from saving goal {goalId}.")
            : L(
                language,
                $"Menyetor {FormatNumber(amount)} ke tabungan tujuan {goalId}.",
                $"Deposited {FormatNumber(amount)} to saving goal {goalId}.");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeSavingGoalAchieved sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeSavingGoalAchieved(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "goal_id", out var goalId))
        {
            return BuildGenericDescription("saving.goal.achieved", payload, language);
        }

        var pointsText = TryGetNumber(payload, "points", out var points)
            ? FormatNumber(points)
            : L(language, "poin tidak diketahui", "unknown points");
        var costText = TryGetNumber(payload, "cost", out var cost)
            ? FormatNumber(cost)
            : L(language, "biaya tidak diketahui", "unknown cost");

        return L(
            language,
            $"Target tabungan {goalId} tercapai (poin {pointsText}, biaya {costText}).",
            $"Saving goal {goalId} was achieved (points {pointsText}, cost {costText}).");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeLoanTaken sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeLoanTaken(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "loan_id", out var loanId))
        {
            return BuildGenericDescription("loan.syariah.taken", payload, language);
        }

        var principalText = TryGetNumber(payload, "principal", out var principal)
            ? FormatNumber(principal)
            : L(language, "nominal tidak diketahui", "unknown amount");
        var installmentText = TryGetNumber(payload, "installment", out var installment)
            ? FormatNumber(installment)
            : L(language, "cicilan tidak diketahui", "unknown installment");
        var durationText = TryGetInt(payload, "duration_turn", out var durationTurn)
            ? durationTurn.ToString(CultureInfo.InvariantCulture)
            : L(language, "?", "?");

        return L(
            language,
            $"Mengambil pinjaman {loanId} (pokok {principalText}, cicilan {installmentText}, durasi {durationText} turn).",
            $"Took loan {loanId} (principal {principalText}, installment {installmentText}, duration {durationText} turns).");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeLoanRepaid sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeLoanRepaid(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "loan_id", out var loanId) || !TryGetNumber(payload, "amount", out var amount))
        {
            return BuildGenericDescription("loan.syariah.repaid", payload, language);
        }

        return L(
            language,
            $"Membayar pinjaman {loanId} sebesar {FormatNumber(amount)}.",
            $"Repaid loan {loanId} by {FormatNumber(amount)}.");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeRiskLife sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeRiskLife(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "risk_id", out var riskId) ||
            !TryGetString(payload, "direction", out var direction) ||
            !TryGetNumber(payload, "amount", out var amount))
        {
            return BuildGenericDescription("risk.life.drawn", payload, language);
        }

        var directionText = direction.Equals("IN", StringComparison.OrdinalIgnoreCase)
            ? L(language, "dampak positif", "positive impact")
            : direction.Equals("OUT", StringComparison.OrdinalIgnoreCase)
                ? L(language, "dampak negatif", "negative impact")
                : direction.ToUpperInvariant();

        return L(
            language,
            $"Kartu risiko {riskId} aktif dengan {directionText} sebesar {FormatNumber(amount)}.",
            $"Risk card {riskId} triggered with {directionText} of {FormatNumber(amount)}.");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeRiskEmergency sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeRiskEmergency(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "option_type", out var optionType) ||
            !TryGetString(payload, "direction", out var direction) ||
            !TryGetNumber(payload, "amount", out var amount))
        {
            return BuildGenericDescription("risk.emergency.used", payload, language);
        }

        var directionText = direction.Equals("IN", StringComparison.OrdinalIgnoreCase)
            ? L(language, "menambah saldo", "adds balance")
            : direction.Equals("OUT", StringComparison.OrdinalIgnoreCase)
                ? L(language, "mengurangi saldo", "reduces balance")
                : direction.ToUpperInvariant();

        return L(
            language,
            $"Menggunakan opsi darurat {optionType} ({directionText} {FormatNumber(amount)}).",
            $"Used emergency option {optionType} ({directionText} {FormatNumber(amount)}).");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeInsurancePurchase sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeInsurancePurchase(JsonElement payload, string language)
    {
        if (!TryGetNumber(payload, "premium", out var premium))
        {
            return BuildGenericDescription("insurance.multirisk.purchased", payload, language);
        }

        return L(
            language,
            $"Membeli asuransi multirisk dengan premi {FormatNumber(premium)}.",
            $"Purchased multirisk insurance with premium {FormatNumber(premium)}.");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeInsuranceUse sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeInsuranceUse(JsonElement payload, string language)
    {
        if (!TryGetString(payload, "risk_event_id", out var riskEventId))
        {
            return BuildGenericDescription("insurance.multirisk.used", payload, language);
        }

        return L(
            language,
            $"Mengaktifkan perlindungan asuransi untuk event risiko {riskEventId}.",
            $"Activated insurance protection for risk event {riskEventId}.");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeRankAward sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeRankAward(JsonElement payload, string language, string topicId, string topicEn)
    {
        if (!TryGetInt(payload, "rank", out var rank) || !TryGetNumber(payload, "points", out var points))
        {
            return BuildGenericDescription("rank.awarded", payload, language);
        }

        return L(
            language,
            $"Mendapat peringkat {rank} pada kategori {topicId} (poin {FormatNumber(points)}).",
            $"Received rank {rank} in {topicEn} category (points {FormatNumber(points)}).");
    }

    /// <summary>
    /// Menjalankan fungsi DescribePointsAward sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribePointsAward(JsonElement payload, string language, string topicId, string topicEn)
    {
        if (!TryGetNumber(payload, "points", out var points))
        {
            return BuildGenericDescription("points.awarded", payload, language);
        }

        return L(
            language,
            $"Mendapat bonus poin {topicId} sebesar {FormatNumber(points)}.",
            $"Received {topicEn} bonus points of {FormatNumber(points)}.");
    }

    /// <summary>
    /// Menjalankan fungsi DescribeTurnAction sebagai bagian dari alur file ini.
    /// </summary>
    private static string DescribeTurnAction(JsonElement payload, string language)
    {
        if (!TryGetInt(payload, "used", out var used) || !TryGetInt(payload, "remaining", out var remaining))
        {
            return BuildGenericDescription("turn.action.used", payload, language);
        }

        return L(
            language,
            $"Menggunakan {used} aksi, sisa aksi turn ini {remaining}.",
            $"Used {used} action(s), remaining actions this turn: {remaining}.");
    }

    /// <summary>
    /// Menjalankan fungsi BuildGenericDescription sebagai bagian dari alur file ini.
    /// </summary>
    private static string BuildGenericDescription(string actionType, JsonElement payload, string language)
    {
        var actionLabel = string.IsNullOrWhiteSpace(actionType) ? L(language, "aktivitas", "activity") : actionType;
        var baseText = L(
            language,
            $"Aksi {actionLabel} dieksekusi pada sesi.",
            $"Action {actionLabel} was executed in this session.");
        var payloadSummary = BuildPayloadSummary(payload, language);
        if (string.IsNullOrWhiteSpace(payloadSummary))
        {
            return baseText;
        }

        return L(language, $"{baseText} Detail: {payloadSummary}", $"{baseText} Details: {payloadSummary}");
    }

    /// <summary>
    /// Menjalankan fungsi BuildPayloadSummary sebagai bagian dari alur file ini.
    /// </summary>
    private static string BuildPayloadSummary(JsonElement payload, string language)
    {
        if (payload.ValueKind != JsonValueKind.Object)
        {
            return string.Empty;
        }

        var keyOrder = new[]
        {
            "amount",
            "direction",
            "category",
            "trade_type",
            "qty",
            "unit_price",
            "card_id",
            "goal_id",
            "loan_id",
            "risk_id",
            "option_type",
            "premium",
            "rank",
            "points"
        };

        var parts = new List<string>();
        foreach (var key in keyOrder)
        {
            if (!payload.TryGetProperty(key, out var value))
            {
                continue;
            }

            parts.Add($"{ResolvePayloadKeyLabel(key, language)}: {JsonElementToInlineText(value, key, language)}");
            if (parts.Count == 5)
            {
                break;
            }
        }

        return string.Join(", ", parts);
    }

    /// <summary>
    /// Menjalankan fungsi ResolvePayloadKeyLabel sebagai bagian dari alur file ini.
    /// </summary>
    private static string ResolvePayloadKeyLabel(string key, string language)
    {
        return key switch
        {
            "amount" => L(language, "nominal", "amount"),
            "direction" => L(language, "arah", "direction"),
            "category" => L(language, "kategori", "category"),
            "trade_type" => L(language, "jenis transaksi", "trade type"),
            "qty" => L(language, "jumlah", "qty"),
            "unit_price" => L(language, "harga per unit", "unit price"),
            "card_id" => L(language, "kartu", "card id"),
            "goal_id" => L(language, "tujuan tabungan", "saving goal"),
            "loan_id" => L(language, "pinjaman", "loan id"),
            "risk_id" => L(language, "risiko", "risk id"),
            "option_type" => L(language, "opsi", "option"),
            "premium" => L(language, "premi", "premium"),
            "rank" => L(language, "peringkat", "rank"),
            "points" => L(language, "poin", "points"),
            _ => key
        };
    }

    /// <summary>
    /// Menjalankan fungsi JsonElementToInlineText sebagai bagian dari alur file ini.
    /// </summary>
    private static string JsonElementToInlineText(JsonElement value, string key, string language)
    {
        if (key == "direction" && value.ValueKind == JsonValueKind.String)
        {
            var direction = value.GetString() ?? string.Empty;
            if (direction.Equals("IN", StringComparison.OrdinalIgnoreCase))
            {
                return L(language, "IN (masuk)", "IN");
            }

            if (direction.Equals("OUT", StringComparison.OrdinalIgnoreCase))
            {
                return L(language, "OUT (keluar)", "OUT");
            }
        }

        if (key == "trade_type" && value.ValueKind == JsonValueKind.String)
        {
            var tradeType = value.GetString() ?? string.Empty;
            if (tradeType.Equals("BUY", StringComparison.OrdinalIgnoreCase))
            {
                return L(language, "BUY (beli)", "BUY");
            }

            if (tradeType.Equals("SELL", StringComparison.OrdinalIgnoreCase))
            {
                return L(language, "SELL (jual)", "SELL");
            }
        }

        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString() ?? string.Empty,
            JsonValueKind.Number => FormatNumber(value.GetDouble()),
            JsonValueKind.True => L(language, "ya", "true"),
            JsonValueKind.False => L(language, "tidak", "false"),
            JsonValueKind.Array => L(language, $"[{value.GetArrayLength()} item]", $"[{value.GetArrayLength()} item(s)]"),
            JsonValueKind.Object => "{...}",
            _ => string.Empty
        };
    }

    /// <summary>
    /// Menjalankan fungsi TryGetString sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryGetString(JsonElement payload, string propertyName, out string value)
    {
        value = string.Empty;
        if (payload.ValueKind != JsonValueKind.Object ||
            !payload.TryGetProperty(propertyName, out var property) ||
            property.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        value = property.GetString() ?? string.Empty;
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Menjalankan fungsi TryGetInt sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryGetInt(JsonElement payload, string propertyName, out int value)
    {
        value = 0;
        if (payload.ValueKind != JsonValueKind.Object ||
            !payload.TryGetProperty(propertyName, out var property) ||
            property.ValueKind != JsonValueKind.Number)
        {
            return false;
        }

        if (property.TryGetInt32(out value))
        {
            return true;
        }

        value = (int)Math.Round(property.GetDouble());
        return true;
    }

    /// <summary>
    /// Menjalankan fungsi TryGetNumber sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryGetNumber(JsonElement payload, string propertyName, out double value)
    {
        value = 0;
        if (payload.ValueKind != JsonValueKind.Object ||
            !payload.TryGetProperty(propertyName, out var property) ||
            property.ValueKind != JsonValueKind.Number)
        {
            return false;
        }

        value = property.GetDouble();
        return true;
    }

    /// <summary>
    /// Menjalankan fungsi TryGetArrayCount sebagai bagian dari alur file ini.
    /// </summary>
    private static bool TryGetArrayCount(JsonElement payload, string propertyName, out int count)
    {
        count = 0;
        if (payload.ValueKind != JsonValueKind.Object ||
            !payload.TryGetProperty(propertyName, out var property) ||
            property.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        count = property.GetArrayLength();
        return true;
    }

    /// <summary>
    /// Menjalankan fungsi FormatNumber sebagai bagian dari alur file ini.
    /// </summary>
    private static string FormatNumber(double value)
        => value.ToString("0.##", CultureInfo.InvariantCulture);

    /// <summary>
    /// Menjalankan fungsi L sebagai bagian dari alur file ini.
    /// </summary>
    private static string L(string language, string id, string en)
        => string.Equals(language, AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase) ? en : id;
}
