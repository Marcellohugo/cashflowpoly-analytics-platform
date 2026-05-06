// Fungsi file: Menyediakan parser payload JSON event gameplay untuk komputasi analitik.
using System.Text.Json;

namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Parser payload event gameplay yang dipakai oleh pipeline analitik.
/// </summary>
internal static class AnalyticsPayloadReader
{
    /// <summary>
    /// Membaca direction, amount, dan category dari payload JSON transaksi.
    /// </summary>
    internal static bool TryReadTransaction(string payloadJson, out string direction, out double amount, out string category)
    {
        direction = string.Empty;
        category = string.Empty;
        amount = 0;

        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("direction", out var directionProp) ||
                !root.TryGetProperty("amount", out var amountProp) ||
                !root.TryGetProperty("category", out var categoryProp))
            {
                return false;
            }

            direction = directionProp.GetString() ?? string.Empty;
            category = categoryProp.GetString() ?? string.Empty;
            amount = amountProp.GetDouble();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Membaca field amount dari payload JSON.
    /// </summary>
    internal static bool TryReadAmount(string payloadJson, out double amount)
    {
        amount = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("amount", out var amountProp))
            {
                return false;
            }

            amount = amountProp.GetDouble();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Membaca trade_type dan qty dari payload JSON perdagangan emas.
    /// </summary>
    internal static bool TryReadGoldTrade(string payloadJson, out string tradeType, out int qty)
    {
        tradeType = string.Empty;
        qty = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("trade_type", out var tradeTypeProp) ||
                !root.TryGetProperty("qty", out var qtyProp))
            {
                return false;
            }

            tradeType = tradeTypeProp.GetString() ?? string.Empty;
            qty = qtyProp.GetInt32();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Memeriksa apakah tipe aksi termasuk event gameplay substantif (bukan meta-event seperti awarded/assigned).
    /// </summary>
    internal static bool IsActionEvent(string actionType)
    {
        if (string.IsNullOrWhiteSpace(actionType))
        {
            return false;
        }

        return !actionType.Equals("turn.action.used", StringComparison.OrdinalIgnoreCase) &&
               !actionType.EndsWith(".awarded", StringComparison.OrdinalIgnoreCase) &&
               !actionType.Equals("order.passed", StringComparison.OrdinalIgnoreCase) &&
               !actionType.Equals("ingredient.discarded", StringComparison.OrdinalIgnoreCase) &&
               !actionType.Equals("risk.emergency.used", StringComparison.OrdinalIgnoreCase) &&
               !actionType.Equals("tie_breaker.assigned", StringComparison.OrdinalIgnoreCase) &&
               !actionType.Equals("mission.assigned", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Membaca jumlah aksi terpakai dan tersisa dari payload JSON event turn.action.used.
    /// </summary>
    internal static bool TryReadActionUsed(string payloadJson, out int used, out int remaining)
    {
        used = 0;
        remaining = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("used", out var usedProp) ||
                !doc.RootElement.TryGetProperty("remaining", out var remainingProp))
            {
                return false;
            }

            used = usedProp.GetInt32();
            remaining = remainingProp.GetInt32();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Membaca detail lengkap perdagangan emas (tipe, kuantitas, harga satuan, jumlah) dari payload JSON.
    /// </summary>
    internal static bool TryReadGoldTradeDetailed(
        string payloadJson,
        out string tradeType,
        out int qty,
        out int unitPrice,
        out int amount)
    {
        tradeType = string.Empty;
        qty = 0;
        unitPrice = 0;
        amount = 0;

        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("trade_type", out var tradeTypeProp) ||
                !root.TryGetProperty("qty", out var qtyProp) ||
                !root.TryGetProperty("unit_price", out var unitPriceProp) ||
                !root.TryGetProperty("amount", out var amountProp))
            {
                return false;
            }

            tradeType = tradeTypeProp.GetString() ?? string.Empty;
            qty = qtyProp.GetInt32();
            unitPrice = unitPriceProp.GetInt32();
            amount = amountProp.GetInt32();
            return qty > 0;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload pembelian bahan baku detail: card_id, ingredient_name, amount.
    /// </summary>
    internal static bool TryReadIngredientPurchaseDetailed(
        string payloadJson,
        out string cardId,
        out string ingredientName,
        out int amount)
    {
        cardId = string.Empty;
        ingredientName = string.Empty;
        amount = 0;

        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("card_id", out var cardIdProp) ||
                !root.TryGetProperty("ingredient_name", out var nameProp) ||
                !root.TryGetProperty("amount", out var amountProp))
            {
                return false;
            }

            cardId = cardIdProp.GetString() ?? string.Empty;
            ingredientName = nameProp.GetString() ?? string.Empty;
            amount = amountProp.GetInt32();
            return !string.IsNullOrWhiteSpace(cardId);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload setoran tabungan: goal_id dan amount.
    /// </summary>
    internal static bool TryReadSavingDeposit(string payloadJson, out string goalId, out int amount)
    {
        goalId = string.Empty;
        amount = 0;

        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("goal_id", out var goalProp) ||
                !root.TryGetProperty("amount", out var amountProp))
            {
                return false;
            }

            goalId = goalProp.GetString() ?? string.Empty;
            amount = amountProp.GetInt32();
            return !string.IsNullOrWhiteSpace(goalId);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload pembelian bahan baku ringkas: card_id dan amount.
    /// </summary>
    internal static bool TryReadIngredientPurchase(string payloadJson, out string cardId, out int amount)
    {
        cardId = string.Empty;
        amount = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("card_id", out var cardIdProp) ||
                !doc.RootElement.TryGetProperty("amount", out var amountProp))
            {
                return false;
            }

            cardId = cardIdProp.GetString() ?? string.Empty;
            amount = amountProp.GetInt32();
            return !string.IsNullOrWhiteSpace(cardId);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload pembelian kebutuhan: amount, card_id opsional, dan points opsional.
    /// </summary>
    internal static bool TryReadNeedPurchase(string payloadJson, out int amount, out string cardId, out int points)
    {
        amount = 0;
        cardId = string.Empty;
        points = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("amount", out var amountProp))
            {
                return false;
            }

            amount = amountProp.GetInt32();
            if (doc.RootElement.TryGetProperty("card_id", out var cardIdProp))
            {
                cardId = cardIdProp.GetString() ?? string.Empty;
            }

            if (doc.RootElement.TryGetProperty("points", out var pointsProp))
            {
                points = pointsProp.GetInt32();
            }

            return amount > 0;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload penugasan misi: mission_id, target kartu tersier, penalti, dan flag kebutuhan primer/sekunder.
    /// </summary>
    internal static bool TryReadMissionAssigned(
        string payloadJson,
        out string missionId,
        out string targetTertiaryCardId,
        out int penaltyPoints,
        out bool requirePrimary,
        out bool requireSecondary)
    {
        missionId = string.Empty;
        targetTertiaryCardId = string.Empty;
        penaltyPoints = 0;
        requirePrimary = true;
        requireSecondary = true;

        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("mission_id", out var missionIdProp) ||
                !root.TryGetProperty("target_tertiary_card_id", out var targetProp) ||
                !root.TryGetProperty("penalty_points", out var penaltyProp))
            {
                return false;
            }

            missionId = missionIdProp.GetString() ?? string.Empty;
            targetTertiaryCardId = targetProp.GetString() ?? string.Empty;
            penaltyPoints = penaltyProp.GetInt32();

            if (root.TryGetProperty("require_primary", out var requirePrimaryProp))
            {
                requirePrimary = requirePrimaryProp.GetBoolean();
            }

            if (root.TryGetProperty("require_secondary", out var requireSecondaryProp))
            {
                requireSecondary = requireSecondaryProp.GetBoolean();
            }

            return !string.IsNullOrWhiteSpace(missionId);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload tie breaker: nomor undian.
    /// </summary>
    internal static bool TryReadTieBreaker(string payloadJson, out int number)
    {
        number = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("number", out var numberProp))
            {
                return false;
            }

            number = numberProp.GetInt32();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload penghargaan peringkat: rank dan points.
    /// </summary>
    internal static bool TryReadRankAwarded(string payloadJson, out int rank, out int points)
    {
        rank = 0;
        points = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("rank", out var rankProp) ||
                !doc.RootElement.TryGetProperty("points", out var pointsProp))
            {
                return false;
            }

            rank = rankProp.GetInt32();
            points = pointsProp.GetInt32();
            return rank > 0;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload pemberian poin: jumlah points.
    /// </summary>
    internal static bool TryReadPointsAwarded(string payloadJson, out int points)
    {
        points = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("points", out var pointsProp))
            {
                return false;
            }

            points = pointsProp.GetInt32();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload pencapaian target tabungan detail: goal_id, points, cost.
    /// </summary>
    internal static bool TryReadSavingGoalAchievedDetailed(
        string payloadJson,
        out string goalId,
        out int points,
        out int cost)
    {
        goalId = string.Empty;
        points = 0;
        cost = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("goal_id", out var goalProp))
            {
                return false;
            }

            goalId = goalProp.GetString() ?? string.Empty;

            if (doc.RootElement.TryGetProperty("points", out var pointsProp))
            {
                points = pointsProp.GetInt32();
            }

            if (doc.RootElement.TryGetProperty("cost", out var costProp))
            {
                cost = costProp.GetInt32();
            }

            return !string.IsNullOrWhiteSpace(goalId);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload pencapaian target tabungan ringkas: points.
    /// </summary>
    internal static bool TryReadSavingGoalAchieved(string payloadJson, out int points)
    {
        points = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("points", out var pointsProp))
            {
                return false;
            }

            points = pointsProp.GetInt32();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload pengambilan pinjaman: loan_id, principal, penalty_points.
    /// </summary>
    internal static bool TryReadLoanTaken(string payloadJson, out string loanId, out int principal, out int penaltyPoints)
    {
        loanId = string.Empty;
        principal = 0;
        penaltyPoints = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("loan_id", out var loanIdProp) ||
                !doc.RootElement.TryGetProperty("principal", out var principalProp) ||
                !doc.RootElement.TryGetProperty("penalty_points", out var penaltyProp))
            {
                return false;
            }

            loanId = loanIdProp.GetString() ?? string.Empty;
            principal = principalProp.GetInt32();
            penaltyPoints = penaltyProp.GetInt32();
            return !string.IsNullOrWhiteSpace(loanId);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload pembayaran pinjaman: loan_id dan amount.
    /// </summary>
    internal static bool TryReadLoanRepay(string payloadJson, out string loanId, out int amount)
    {
        loanId = string.Empty;
        amount = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("loan_id", out var loanIdProp) ||
                !doc.RootElement.TryGetProperty("amount", out var amountProp))
            {
                return false;
            }

            loanId = loanIdProp.GetString() ?? string.Empty;
            amount = amountProp.GetInt32();
            return !string.IsNullOrWhiteSpace(loanId);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload klaim pesanan: daftar kartu bahan baku yang diperlukan dan income.
    /// </summary>
    internal static bool TryReadOrderClaim(string payloadJson, out List<string> requiredCards, out int income)
    {
        requiredCards = new List<string>();
        income = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("required_ingredient_card_ids", out var cardsProp) ||
                cardsProp.ValueKind != JsonValueKind.Array ||
                !doc.RootElement.TryGetProperty("income", out var incomeProp))
            {
                return false;
            }

            income = incomeProp.GetInt32();
            foreach (var item in cardsProp.EnumerateArray())
            {
                var cardId = item.GetString();
                if (!string.IsNullOrWhiteSpace(cardId))
                {
                    requiredCards.Add(cardId);
                }
            }

            return requiredCards.Count > 0;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
