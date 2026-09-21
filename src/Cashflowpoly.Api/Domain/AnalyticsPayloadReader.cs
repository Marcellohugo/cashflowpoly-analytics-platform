// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsPayloadReader.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Parser payload event gameplay yang dipakai oleh pipeline analitik.
/// </summary>
// Mendefinisikan tipe class `AnalyticsPayloadReader` yang mewarisi atau menerapkan `IAnalyticsPayloadReader`; sealed mencegah tipe ini diturunkan
// lagi.
internal sealed class AnalyticsPayloadReader : IAnalyticsPayloadReader
{
    /// <summary>
    /// Membaca direction, amount, dan category dari payload JSON transaksi.
    /// </summary>
    // Mendefinisikan metode `TryReadTransaction` dengan hasil bertipe `bool`. Membaca direction, amount, dan category dari payload JSON transaksi.
    // Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `direction` bertipe `string` membawa nilai direction; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `double` membawa nominal uang atau nilai transaksi
    // yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `category` bertipe `string` membawa
    // nilai category; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadTransaction(string payloadJson, out string direction, out double amount, out string category)
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadTransaction.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Membaca field amount dari payload JSON.
    /// </summary>
    // Mendefinisikan metode `TryReadAmount` dengan hasil bertipe `bool`. Membaca field amount dari payload JSON. Masukan: Parameter `payloadJson`
    // bertipe `string` membawa nilai payload JSON; Parameter `amount` bertipe `double` membawa nominal uang atau nilai transaksi yang dipakai dalam
    // operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadAmount(string payloadJson, out double amount)
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadAmount.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Membaca trade_type dan qty dari payload JSON perdagangan emas.
    /// </summary>
    // Mendefinisikan metode `TryReadGoldTrade` dengan hasil bertipe `bool`. Membaca trade_type dan qty dari payload JSON perdagangan emas. Masukan:
    // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `tradeType` bertipe `string` membawa nilai trade jenis; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `qty` bertipe `int` membawa nilai qty; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode.
    public bool TryReadGoldTrade(string payloadJson, out string tradeType, out int qty)
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadGoldTrade.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Memeriksa apakah tipe aksi termasuk event gameplay substantif (bukan meta-event seperti awarded/assigned).
    /// </summary>
    // Mendefinisikan metode `IsActionEvent` dengan hasil bertipe `bool`. Memeriksa apakah tipe aksi termasuk event gameplay substantif (bukan
    // meta-event seperti awarded/assigned). Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
    public bool IsActionEvent(string actionType)
    {
        if (string.IsNullOrWhiteSpace(actionType))
        {
            return false;
        }

        return GameActionCatalog.GetPlayerActionSlotPolicy(actionType, default) == PlayerActionSlotPolicy.Consumes;
    }

    /// <summary>
    /// Membaca jumlah aksi terpakai dan tersisa dari payload JSON event AkhirGiliran.
    /// </summary>
    // Mendefinisikan metode `TryReadActionUsed` dengan hasil bertipe `bool`. Membaca jumlah aksi terpakai dan tersisa dari payload JSON event
    // AkhirGiliran. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `used` bertipe `int` membawa nilai used;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `remaining` bertipe `int` membawa nilai tersisa; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadActionUsed(string payloadJson, out int used, out int remaining)
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadActionUsed.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Membaca detail lengkap perdagangan emas (tipe, kuantitas, harga satuan, jumlah) dari payload JSON.
    /// </summary>
    // Mendefinisikan metode `TryReadGoldTradeDetailed` dengan hasil bertipe `bool`. Membaca detail lengkap perdagangan emas (tipe, kuantitas, harga
    // satuan, jumlah) dari payload JSON. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `tradeType` bertipe
    // `string` membawa nilai trade jenis; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `qty` bertipe `int` membawa
    // nilai qty; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `unitPrice` bertipe `int` membawa nilai unit harga;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi
    // yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadGoldTradeDetailed(
        // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON.
        string payloadJson,
        // Parameter `tradeType` bertipe `string` membawa nilai trade jenis; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string tradeType,
        // Parameter `qty` bertipe `int` membawa nilai qty; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int qty,
        // Parameter `unitPrice` bertipe `int` membawa nilai unit harga; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int unitPrice,
        // Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter
        // dan harus diisi oleh metode.
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadGoldTradeDetailed.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload pembelian bahan baku detail: card_id, ingredient_name, amount.
    /// </summary>
    // Mendefinisikan metode `TryReadIngredientPurchaseDetailed` dengan hasil bertipe `bool`. Mem-parsing payload pembelian bahan baku detail: card_id,
    // ingredient_name, amount. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `cardId` bertipe `string`
    // membawa nilai kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `ingredientName` bertipe `string`
    // membawa nilai bahan nama; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal
    // uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadIngredientPurchaseDetailed(
        // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON.
        string payloadJson,
        // Parameter `cardId` bertipe `string` membawa nilai kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string cardId,
        // Parameter `ingredientName` bertipe `string` membawa nilai bahan nama; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string ingredientName,
        // Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter
        // dan harus diisi oleh metode.
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadIngredientPurchaseDetailed.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload setoran tabungan: goal_id dan amount.
    /// </summary>
    // Membaca setoran tabungan bersama: amount wajib dan goal_id opsional sebagai label historis. Masukan:
    // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `goalId` bertipe `string` membawa nilai target identitas; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi
    // yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadSavingDeposit(string payloadJson, out string goalId, out int amount)
    {
        goalId = string.Empty;
        amount = 0;

        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;
            if (!root.TryGetProperty("amount", out var amountProp) ||
                !amountProp.TryGetInt32(out amount))
            {
                return false;
            }

            if (root.TryGetProperty("goal_id", out var goalProp) && goalProp.ValueKind == JsonValueKind.String)
                goalId = goalProp.GetString() ?? string.Empty;
            return true;
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadSavingDeposit.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload pembelian bahan baku ringkas: card_id dan amount.
    /// </summary>
    // Mendefinisikan metode `TryReadIngredientPurchase` dengan hasil bertipe `bool`. Mem-parsing payload pembelian bahan baku ringkas: card_id dan
    // amount. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `cardId` bertipe `string` membawa nilai kartu
    // identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau
    // nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadIngredientPurchase(string payloadJson, out string cardId, out int amount)
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadIngredientPurchase.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload pembelian kebutuhan: amount, card_id opsional, dan points opsional.
    /// </summary>
    // Mendefinisikan metode `TryReadNeedPurchase` dengan hasil bertipe `bool`. Mem-parsing payload pembelian kebutuhan: amount, card_id opsional, dan
    // points opsional. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `amount` bertipe `int` membawa nominal
    // uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `cardId`
    // bertipe `string` membawa nilai kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `points` bertipe
    // `int` membawa nilai poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadNeedPurchase(string payloadJson, out int amount, out string cardId, out int points)
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadNeedPurchase.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload penugasan misi: mission_id, target kartu tersier, penalti, dan flag kebutuhan primer/sekunder.
    /// </summary>
    // Mendefinisikan metode `TryReadMissionAssigned` dengan hasil bertipe `bool`. Mem-parsing payload penugasan misi: mission_id, target kartu tersier,
    // penalti, dan flag kebutuhan primer/sekunder. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `missionId`
    // bertipe `string` membawa identitas misi koleksi yang ditugaskan; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `targetTertiaryCardId` bertipe `string` membawa nilai target tertiary kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi
    // oleh metode; Parameter `penaltyPoints` bertipe `int` membawa nilai penalti poin; out mengembalikan nilai melalui parameter dan harus diisi oleh
    // metode; Parameter `requirePrimary` bertipe `bool` membawa nilai require primary; out mengembalikan nilai melalui parameter dan harus diisi oleh
    // metode; Parameter `requireSecondary` bertipe `bool` membawa nilai require secondary; out mengembalikan nilai melalui parameter dan harus diisi
    // oleh metode.
    public bool TryReadMissionAssigned(
        // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON.
        string payloadJson,
        // Parameter `missionId` bertipe `string` membawa identitas misi koleksi yang ditugaskan; out mengembalikan nilai melalui parameter dan harus diisi
        // oleh metode.
        out string missionId,
        // Parameter `targetTertiaryCardId` bertipe `string` membawa nilai target tertiary kartu identitas; out mengembalikan nilai melalui parameter dan
        // harus diisi oleh metode.
        out string targetTertiaryCardId,
        // Parameter `penaltyPoints` bertipe `int` membawa nilai penalti poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int penaltyPoints,
        // Parameter `requirePrimary` bertipe `bool` membawa nilai require primary; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out bool requirePrimary,
        // Parameter `requireSecondary` bertipe `bool` membawa nilai require secondary; out mengembalikan nilai melalui parameter dan harus diisi oleh
        // metode.
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadMissionAssigned.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload tie breaker: nomor undian.
    /// </summary>
    // Mendefinisikan metode `TryReadTieBreaker` dengan hasil bertipe `bool`. Mem-parsing payload tie breaker: nomor undian. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `number` bertipe `int` membawa nilai number; out mengembalikan nilai melalui
    // parameter dan harus diisi oleh metode.
    public bool TryReadTieBreaker(string payloadJson, out int number)
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadTieBreaker.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload penghargaan peringkat: rank dan points.
    /// </summary>
    // Mendefinisikan metode `TryReadRankAwarded` dengan hasil bertipe `bool`. Mem-parsing payload penghargaan peringkat: rank dan points. Masukan:
    // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `rank` bertipe `int` membawa nilai rank; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode; Parameter `points` bertipe `int` membawa nilai poin; out mengembalikan nilai melalui parameter dan
    // harus diisi oleh metode.
    public bool TryReadRankAwarded(string payloadJson, out int rank, out int points)
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadRankAwarded.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload pemberian poin: jumlah points.
    /// </summary>
    // Mendefinisikan metode `TryReadPointsAwarded` dengan hasil bertipe `bool`. Mem-parsing payload pemberian poin: jumlah points. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `points` bertipe `int` membawa nilai poin; out mengembalikan nilai melalui
    // parameter dan harus diisi oleh metode.
    public bool TryReadPointsAwarded(string payloadJson, out int points)
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadPointsAwarded.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload pencapaian target tabungan detail: goal_id, points, cost.
    /// </summary>
    // Mendefinisikan metode `TryReadSavingGoalAchievedDetailed` dengan hasil bertipe `bool`. Mem-parsing payload pencapaian target tabungan detail:
    // goal_id, points, cost. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `goalId` bertipe `string` membawa
    // nilai target identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `points` bertipe `int` membawa nilai
    // poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `cost` bertipe `int` membawa nilai biaya; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadSavingGoalAchievedDetailed(
        // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON.
        string payloadJson,
        // Parameter `goalId` bertipe `string` membawa nilai target identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string goalId,
        // Parameter `points` bertipe `int` membawa nilai poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int points,
        // Parameter `cost` bertipe `int` membawa nilai biaya; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadSavingGoalAchievedDetailed.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload pencapaian target tabungan ringkas: points.
    /// </summary>
    // Mendefinisikan metode `TryReadSavingGoalAchieved` dengan hasil bertipe `bool`. Mem-parsing payload pencapaian target tabungan ringkas: points.
    // Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `points` bertipe `int` membawa nilai poin; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadSavingGoalAchieved(string payloadJson, out int points)
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadSavingGoalAchieved.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload pengambilan pinjaman: loan_id, principal, penalty_points.
    /// </summary>
    // Mendefinisikan metode `TryReadLoanTaken` dengan hasil bertipe `bool`. Mem-parsing payload pengambilan pinjaman: loan_id, principal,
    // penalty_points. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `loanId` bertipe `string` membawa nilai
    // pinjaman identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `principal` bertipe `int` membawa nilai
    // principal; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `penaltyPoints` bertipe `int` membawa nilai penalti
    // poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadLoanTaken(string payloadJson, out string loanId, out int principal, out int penaltyPoints)
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadLoanTaken.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload pembayaran pinjaman: loan_id dan amount.
    /// </summary>
    // Mendefinisikan metode `TryReadLoanRepay` dengan hasil bertipe `bool`. Mem-parsing payload pembayaran pinjaman: loan_id dan amount. Masukan:
    // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `loanId` bertipe `string` membawa nilai pinjaman identitas; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi
    // yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadLoanRepay(string payloadJson, out string loanId, out int amount)
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadLoanRepay.
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Mem-parsing payload klaim pesanan: daftar kartu bahan baku yang diperlukan dan income.
    /// </summary>
    // Mendefinisikan metode `TryReadOrderClaim` dengan hasil bertipe `bool`. Mem-parsing payload klaim pesanan: daftar kartu bahan baku yang diperlukan
    // dan income. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `requiredCards` bertipe `List<string>`
    // membawa nilai required kartu; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `income` bertipe `int` membawa
    // nilai pemasukan; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadOrderClaim(string payloadJson, out List<string> requiredCards, out int income)
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
            // Mengulangi setiap elemen `cardsProp.EnumerateArray()`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
            // TryReadOrderClaim.
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
        // Menangani exception `JsonException` melalui variabel dalam TryReadOrderClaim.
        catch (JsonException)
        {
            return false;
        }
    }

    public bool TryReadSoldNeed(string payloadJson, out string cardId)
    {
        cardId = string.Empty;
        if (string.IsNullOrWhiteSpace(payloadJson))
        {
            return false;
        }

        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("option_type", out var typeProp) ||
                !string.Equals(typeProp.GetString(), "SELL_NEED", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!doc.RootElement.TryGetProperty("card_id", out var cardProp))
            {
                return false;
            }

            cardId = cardProp.GetString() ?? string.Empty;
            return !string.IsNullOrWhiteSpace(cardId);
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadSoldNeed.
        catch (JsonException)
        {
            return false;
        }
    }
}
