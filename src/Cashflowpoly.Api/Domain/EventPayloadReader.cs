// Fungsi file: Parser payload JSON event gameplay untuk validasi domain dan proyeksi cashflow.
using System.Text.Json;

namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Helper murni untuk membaca payload event gameplay.
/// </summary>
internal sealed class EventPayloadReader : IEventPayloadReader
{
    /// <summary>
    /// Mem-parse string JSON payload event menjadi JsonElement.
    /// </summary>
    public JsonElement ReadPayload(string payload)
    {
        using var document = JsonDocument.Parse(payload);
        return document.RootElement.Clone();
    }

    /// <summary>
    /// Mengekstrak nilai string dari properti JSON payload.
    /// </summary>
    public bool TryGetString(JsonElement payload, string propertyName, out string value)
    {
        value = string.Empty;
        if (!payload.TryGetProperty(propertyName, out var property))
        {
            return false;
        }

        if (property.ValueKind is JsonValueKind.String or JsonValueKind.Null)
        {
            value = property.GetString() ?? string.Empty;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Mengekstrak nilai string opsional dari payload; mengembalikan true jika properti tidak ada.
    /// </summary>
    public bool TryGetOptionalString(JsonElement payload, string propertyName, out string? value)
    {
        value = null;
        if (!payload.TryGetProperty(propertyName, out var property))
        {
            return true;
        }

        if (property.ValueKind is JsonValueKind.String or JsonValueKind.Null)
        {
            value = property.GetString();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Mengekstrak nilai integer 32-bit dari properti JSON payload.
    /// </summary>
    public bool TryGetInt32(JsonElement payload, string propertyName, out int value, bool required = true)
    {
        value = 0;
        if (!payload.TryGetProperty(propertyName, out var property))
        {
            return !required;
        }

        if (property.ValueKind != JsonValueKind.Number || !property.TryGetInt32(out value))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Mengekstrak nilai double dari properti JSON payload.
    /// </summary>
    public bool TryGetDouble(JsonElement payload, string propertyName, out double value, bool required = true)
    {
        value = 0;
        if (!payload.TryGetProperty(propertyName, out var property))
        {
            return !required;
        }

        if (property.ValueKind != JsonValueKind.Number || !property.TryGetDouble(out value))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Membaca direction, amount, category, dan counterparty dari payload event transaction.recorded.
    /// </summary>
    public bool TryReadTransaction(JsonElement payload, out string direction, out double amount, out string category, out string? counterparty)
    {
        direction = string.Empty;
        category = string.Empty;
        counterparty = null;
        amount = 0;

        if (!TryGetString(payload, "direction", out direction) ||
            !TryGetDouble(payload, "amount", out amount) ||
            !TryGetString(payload, "category", out category))
        {
            return false;
        }

        return TryGetOptionalString(payload, "counterparty", out counterparty);
    }

    /// <summary>
    /// Membaca nilai amount dari payload JSON event.
    /// </summary>
    public bool TryReadAmount(JsonElement payload, out double amount)
    {
        return TryGetDouble(payload, "amount", out amount);
    }

    /// <summary>
    /// Membaca trade_type, qty, unit_price, dan amount dari payload event gold_trade.
    /// </summary>
    public bool TryReadGoldTrade(JsonElement payload, out string tradeType, out int qty, out int unitPrice, out int amount)
    {
        tradeType = string.Empty;
        qty = 0;
        unitPrice = 0;
        amount = 0;

        if (!TryGetString(payload, "trade_type", out tradeType) ||
            !TryGetInt32(payload, "qty", out qty) ||
            !TryGetInt32(payload, "unit_price", out unitPrice) ||
            !TryGetInt32(payload, "amount", out amount))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Membaca jumlah aksi terpakai dan sisa dari payload event turn.action.used.
    /// </summary>
    public bool TryReadActionUsed(JsonElement payload, out int used, out int remaining)
    {
        used = 0;
        remaining = 0;
        if (!TryGetInt32(payload, "used", out used) ||
            !TryGetInt32(payload, "remaining", out remaining))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Membaca card_id dan amount dari payload event ingredient.purchased atau ingredient.discarded.
    /// </summary>
    public bool TryReadIngredientPurchase(JsonElement payload, out string cardId, out int amount)
    {
        cardId = string.Empty;
        amount = 0;

        if (!TryGetString(payload, "card_id", out cardId) ||
            !TryGetInt32(payload, "amount", out amount))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(cardId);
    }

    /// <summary>
    /// Membaca daftar kartu bahan yang dibutuhkan dan pendapatan dari payload event order.claimed.
    /// </summary>
    public bool TryReadOrderClaim(JsonElement payload, out List<string> requiredCards, out int income)
    {
        requiredCards = new List<string>();
        income = 0;

        if (!payload.TryGetProperty("required_ingredient_card_ids", out var cardsProp) ||
            cardsProp.ValueKind != JsonValueKind.Array ||
            !TryGetInt32(payload, "income", out income))
        {
            return false;
        }

        foreach (var item in cardsProp.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.String)
            {
                requiredCards.Clear();
                return false;
            }

            var cardId = item.GetString();
            if (!string.IsNullOrWhiteSpace(cardId))
            {
                requiredCards.Add(cardId);
            }
        }

        return requiredCards.Count > 0;
    }

    /// <summary>
    /// Membaca card_id, amount, dan points dari payload event pembelian kebutuhan.
    /// </summary>
    public bool TryReadNeedPurchase(JsonElement payload, out string cardId, out int amount, out int points)
    {
        cardId = string.Empty;
        amount = 0;
        points = 0;

        if (!TryGetString(payload, "card_id", out cardId) ||
            !TryGetInt32(payload, "amount", out amount) ||
            !TryGetInt32(payload, "points", out points, required: false))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(cardId))
        {
            cardId = string.Empty;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Membaca mission_id, target_tertiary_card_id, dan penalty_points dari payload event misi.
    /// </summary>
    public bool TryReadMissionAssigned(JsonElement payload, out string missionId, out string targetCardId, out int penaltyPoints)
    {
        missionId = string.Empty;
        targetCardId = string.Empty;
        penaltyPoints = 0;

        if (!TryGetString(payload, "mission_id", out missionId) ||
            !TryGetString(payload, "target_tertiary_card_id", out targetCardId) ||
            !TryGetInt32(payload, "penalty_points", out penaltyPoints))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(missionId);
    }

    /// <summary>
    /// Membaca nomor tie-breaker dari payload event tie_breaker.assigned.
    /// </summary>
    public bool TryReadTieBreaker(JsonElement payload, out int number)
    {
        return TryGetInt32(payload, "number", out number);
    }

    /// <summary>
    /// Membaca rank dan points dari payload event rank.awarded.
    /// </summary>
    public bool TryReadRankAwarded(JsonElement payload, out int rank, out int points)
    {
        rank = 0;
        points = 0;
        if (!TryGetInt32(payload, "rank", out rank) ||
            !TryGetInt32(payload, "points", out points))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Membaca nilai points dari payload event points.awarded.
    /// </summary>
    public bool TryReadPointsAwarded(JsonElement payload, out int points)
    {
        return TryGetInt32(payload, "points", out points);
    }

    /// <summary>
    /// Membaca goal_id dan amount dari payload event saving.deposit.
    /// </summary>
    public bool TryReadSavingDeposit(JsonElement payload, out string goalId, out int amount)
    {
        goalId = string.Empty;
        amount = 0;
        if (!TryGetString(payload, "goal_id", out goalId) ||
            !TryGetInt32(payload, "amount", out amount))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Membaca goal_id, points, dan cost dari payload event saving.goal.achieved.
    /// </summary>
    public bool TryReadSavingGoalAchieved(JsonElement payload, out string goalId, out int points, out int cost)
    {
        goalId = string.Empty;
        points = 0;
        cost = 0;
        if (!TryGetString(payload, "goal_id", out goalId) ||
            !TryGetInt32(payload, "points", out points) ||
            !TryGetInt32(payload, "cost", out cost, required: false))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(goalId);
    }

    /// <summary>
    /// Membaca risk_id, direction, dan amount dari payload event risk.life.drawn.
    /// </summary>
    public bool TryReadRiskLife(JsonElement payload, out string riskId, out string direction, out int amount)
    {
        riskId = string.Empty;
        direction = string.Empty;
        amount = 0;
        if (!TryGetString(payload, "risk_id", out riskId) ||
            !TryGetString(payload, "direction", out direction) ||
            !TryGetInt32(payload, "amount", out amount))
        {
            return false;
        }

        return direction.Equals("IN", StringComparison.OrdinalIgnoreCase) ||
               direction.Equals("OUT", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Membaca risk_event_id dari payload event insurance.multirisk.used.
    /// </summary>
    public bool TryReadInsuranceUsed(JsonElement payload, out string riskEventId)
    {
        riskEventId = string.Empty;
        if (!TryGetString(payload, "risk_event_id", out riskEventId))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(riskEventId);
    }

    /// <summary>
    /// Membaca risk_event_id, option_type, direction, dan amount dari payload event risk.emergency.used.
    /// </summary>
    public bool TryReadEmergencyOption(
        JsonElement payload,
        out string riskEventId,
        out string optionType,
        out string direction,
        out int amount)
    {
        riskEventId = string.Empty;
        optionType = string.Empty;
        direction = string.Empty;
        amount = 0;

        if (!TryGetString(payload, "risk_event_id", out riskEventId) ||
            !TryGetString(payload, "option_type", out optionType) ||
            !TryGetString(payload, "direction", out direction) ||
            !TryGetInt32(payload, "amount", out amount))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(riskEventId);
    }

    /// <summary>
    /// Membaca loan_id, principal, installment, duration, dan penalty_points dari payload event pinjaman.
    /// </summary>
    public bool TryReadLoanTaken(
        JsonElement payload,
        out string loanId,
        out int principal,
        out int installment,
        out int duration,
        out int penaltyPoints)
    {
        loanId = string.Empty;
        principal = 0;
        installment = 0;
        duration = 0;
        penaltyPoints = 0;

        if (!TryGetString(payload, "loan_id", out loanId) ||
            !TryGetInt32(payload, "principal", out principal) ||
            !TryGetInt32(payload, "installment", out installment) ||
            !TryGetInt32(payload, "duration_turn", out duration) ||
            !TryGetInt32(payload, "penalty_points", out penaltyPoints))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(loanId);
    }

    /// <summary>
    /// Membaca loan_id dan amount dari payload event loan.syariah.repaid.
    /// </summary>
    public bool TryReadLoanRepay(JsonElement payload, out string loanId, out int amount)
    {
        loanId = string.Empty;
        amount = 0;
        if (!TryGetString(payload, "loan_id", out loanId) ||
            !TryGetInt32(payload, "amount", out amount))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(loanId);
    }

    /// <summary>
    /// Membaca nilai premium dari payload event insurance.multirisk.purchased.
    /// </summary>
    public bool TryReadInsurance(JsonElement payload, out int premium)
    {
        return TryGetInt32(payload, "premium", out premium);
    }
}
