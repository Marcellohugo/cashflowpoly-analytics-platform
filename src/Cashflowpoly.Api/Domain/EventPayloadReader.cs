// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventPayloadReader.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Helper murni untuk membaca payload event gameplay.
/// </summary>
// Mendefinisikan tipe class `EventPayloadReader` yang mewarisi atau menerapkan `IEventPayloadReader`; sealed mencegah tipe ini diturunkan lagi.
internal sealed class EventPayloadReader : IEventPayloadReader
{
    /// <summary>
    /// Mem-parse string JSON payload event menjadi JsonElement.
    /// </summary>
    // Mendefinisikan metode `ReadPayload` dengan hasil bertipe `JsonElement`. Mem-parse string JSON payload event menjadi JsonElement. Masukan:
    // Parameter `payload` bertipe `string` membawa muatan detail event dalam format JSON.
    public JsonElement ReadPayload(string payload)
    {
        using var document = JsonDocument.Parse(payload);
        return document.RootElement.Clone();
    }

    /// <summary>
    /// Mengekstrak nilai string dari properti JSON payload.
    /// </summary>
    // Mendefinisikan metode `TryGetString` dengan hasil bertipe `bool`. Mengekstrak nilai string dari properti JSON payload. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property
    // nama; Parameter `value` bertipe `string` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
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
    // Mendefinisikan metode `TryGetOptionalString` dengan hasil bertipe `bool`. Mengekstrak nilai string opsional dari payload; mengembalikan true jika
    // properti tidak ada. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName`
    // bertipe `string` membawa nilai property nama; Parameter `value` bertipe `string?` membawa nilai nilai; nilai null diizinkan ketika data opsional
    // belum tersedia; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
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
    // Mendefinisikan metode `TryGetInt32` dengan hasil bertipe `bool`. Mengekstrak nilai integer 32-bit dari properti JSON payload. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property
    // nama; Parameter `value` bertipe `int` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `required` bertipe `bool` membawa nilai required; bila argumen tidak diberikan digunakan true, yaitu kondisi aktif/terpenuhi.
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
    // Mendefinisikan metode `TryGetDouble` dengan hasil bertipe `bool`. Mengekstrak nilai double dari properti JSON payload. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property
    // nama; Parameter `value` bertipe `double` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `required` bertipe `bool` membawa nilai required; bila argumen tidak diberikan digunakan true, yaitu kondisi aktif/terpenuhi.
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
    /// Membaca direction, amount, category, dan counterparty dari payload event CatatTransaksi.
    /// </summary>
    // Mendefinisikan metode `TryReadTransaction` dengan hasil bertipe `bool`. Membaca direction, amount, category, dan counterparty dari payload event
    // CatatTransaksi. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `direction` bertipe
    // `string` membawa nilai direction; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `double`
    // membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode;
    // Parameter `category` bertipe `string` membawa nilai category; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `counterparty` bertipe `string?` membawa nilai counterparty; nilai null diizinkan ketika data opsional belum tersedia; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode.
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
    // Mendefinisikan metode `TryReadAmount` dengan hasil bertipe `bool`. Membaca nilai amount dari payload JSON event. Masukan: Parameter `payload`
    // bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `amount` bertipe `double` membawa nominal uang atau nilai
    // transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadAmount(JsonElement payload, out double amount)
    {
        return TryGetDouble(payload, "amount", out amount);
    }

    /// <summary>
    /// Membaca trade_type, qty, unit_price, dan amount dari payload event InvestasiEmas dan JualEmas.
    /// </summary>
    // Mendefinisikan metode `TryReadGoldTrade` dengan hasil bertipe `bool`. Membaca trade_type, qty, unit_price, dan amount dari payload event
    // InvestasiEmas dan JualEmas. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter
    // `tradeType` bertipe `string` membawa nilai trade jenis; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `qty`
    // bertipe `int` membawa nilai qty; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `unitPrice` bertipe `int`
    // membawa nilai unit harga; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal
    // uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
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
    /// Membaca jumlah aksi terpakai dan sisa dari payload event AkhirGiliran.
    /// </summary>
    // Mendefinisikan metode `TryReadActionUsed` dengan hasil bertipe `bool`. Membaca jumlah aksi terpakai dan sisa dari payload event AkhirGiliran.
    // Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `used` bertipe `int` membawa nilai
    // used; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `remaining` bertipe `int` membawa nilai tersisa; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
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
    /// Membaca card_id dan amount dari payload event BahanMasakan atau BuangBahanMasakan.
    /// </summary>
    // Mendefinisikan metode `TryReadIngredientPurchase` dengan hasil bertipe `bool`. Membaca card_id dan amount dari payload event BahanMasakan atau
    // BuangBahanMasakan. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `cardId` bertipe
    // `string` membawa nilai kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int`
    // membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
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
    /// Membaca daftar kartu bahan yang dibutuhkan dan pendapatan dari payload event JualMasakan.
    /// </summary>
    // Mendefinisikan metode `TryReadOrderClaim` dengan hasil bertipe `bool`. Membaca daftar kartu bahan yang dibutuhkan dan pendapatan dari payload
    // event JualMasakan. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `requiredCards`
    // bertipe `List<string>` membawa nilai required kartu; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `income`
    // bertipe `int` membawa nilai pemasukan; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
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

        // Mengulangi setiap elemen `cardsProp.EnumerateArray()`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
        // TryReadOrderClaim.
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
    // Mendefinisikan metode `TryReadNeedPurchase` dengan hasil bertipe `bool`. Membaca card_id, amount, dan points dari payload event pembelian
    // kebutuhan. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `cardId` bertipe `string`
    // membawa nilai kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa
    // nominal uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `points` bertipe `int` membawa nilai poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
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
    // Mendefinisikan metode `TryReadMissionAssigned` dengan hasil bertipe `bool`. Membaca mission_id, target_tertiary_card_id, dan penalty_points dari
    // payload event misi. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `missionId`
    // bertipe `string` membawa identitas misi koleksi yang ditugaskan; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `targetCardId` bertipe `string` membawa nilai target kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode;
    // Parameter `penaltyPoints` bertipe `int` membawa nilai penalti poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
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
    /// Membaca nomor tie-breaker dari payload event BagikanTieBreaker.
    /// </summary>
    // Mendefinisikan metode `TryReadTieBreaker` dengan hasil bertipe `bool`. Membaca nomor tie-breaker dari payload event BagikanTieBreaker. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `number` bertipe `int` membawa nilai number;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadTieBreaker(JsonElement payload, out int number)
    {
        return TryGetInt32(payload, "number", out number);
    }

    /// <summary>
    /// Membaca rank dan points dari payload event rank.awarded.
    /// </summary>
    // Mendefinisikan metode `TryReadRankAwarded` dengan hasil bertipe `bool`. Membaca rank dan points dari payload event rank.awarded. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `rank` bertipe `int` membawa nilai rank; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `points` bertipe `int` membawa nilai poin; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode.
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
    // Mendefinisikan metode `TryReadPointsAwarded` dengan hasil bertipe `bool`. Membaca nilai points dari payload event points.awarded. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `points` bertipe `int` membawa nilai poin; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadPointsAwarded(JsonElement payload, out int points)
    {
        return TryGetInt32(payload, "points", out points);
    }

    /// <summary>
    /// Membaca goal_id dan amount dari payload event Menabung/TarikTabungan.
    /// </summary>
    // Membaca amount serta goal_id opsional sebagai label historis, tanpa pemesanan kartu tujuan.
    // Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `goalId` bertipe `string` membawa
    // nilai target identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal
    // uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadSavingDeposit(JsonElement payload, out string goalId, out int amount)
    {
        goalId = string.Empty;
        amount = 0;
        if (!TryGetInt32(payload, "amount", out amount))
        {
            return false;
        }

        if (payload.TryGetProperty("goal_id", out _) && !TryGetString(payload, "goal_id", out goalId))
            return false;
        return true;
    }

    /// <summary>
    /// Membaca goal_id, points, dan cost dari payload event TujuanFinansial.
    /// </summary>
    // Mendefinisikan metode `TryReadSavingGoalAchieved` dengan hasil bertipe `bool`. Membaca goal_id, points, dan cost dari payload event
    // TujuanFinansial. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `goalId` bertipe
    // `string` membawa nilai target identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `points` bertipe `int`
    // membawa nilai poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `cost` bertipe `int` membawa nilai biaya;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
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
    /// Membaca risk_id, direction, dan amount dari payload event RisikoKehidupan.
    /// </summary>
    // Mendefinisikan metode `TryReadRiskLife` dengan hasil bertipe `bool`. Membaca risk_id, direction, dan amount dari payload event RisikoKehidupan.
    // Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `riskId` bertipe `string` membawa
    // nilai risiko identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `direction` bertipe `string` membawa
    // nilai direction; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang
    // atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
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
    /// Membaca risk_event_id dari payload event klaim Asuransi.
    /// </summary>
    // Mendefinisikan metode `TryReadInsuranceUsed` dengan hasil bertipe `bool`. Membaca risk_event_id dari payload event klaim Asuransi. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `riskEventId` bertipe `string` membawa nilai
    // risiko event identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
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
    /// Membaca opsi darurat yang nominal dan arahnya sudah dihitung server.
    /// </summary>
    // Mendefinisikan metode `TryReadEmergencyOption` dengan hasil bertipe `bool`. Membaca opsi darurat yang nominal dan arahnya sudah dihitung server.
    // Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `riskEventId` bertipe `string`
    // membawa nilai risiko event identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `optionType` bertipe
    // `string` membawa nilai option jenis; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `direction` bertipe
    // `string` membawa nilai direction; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa
    // nominal uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadEmergencyOption(
        // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON.
        JsonElement payload,
        // Parameter `riskEventId` bertipe `string` membawa nilai risiko event identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh
        // metode.
        out string riskEventId,
        // Parameter `optionType` bertipe `string` membawa nilai option jenis; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string optionType,
        // Parameter `direction` bertipe `string` membawa nilai direction; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string direction,
        // Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter
        // dan harus diisi oleh metode.
        out int amount)
    {
        riskEventId = string.Empty;
        optionType = string.Empty;
        direction = string.Empty;
        amount = 0;

        if (!TryGetString(payload, "risk_event_id", out riskEventId) ||
            !TryGetString(payload, "option_type", out optionType))
        {
            return false;
        }

        direction = "IN";
        if (string.Equals(optionType, "TAKE_SHARIA_LOAN", StringComparison.OrdinalIgnoreCase))
        {
            TryGetInt32(payload, "principal", out amount);
        }
        else
        {
            TryGetInt32(payload, "amount", out amount);
        }

        return !string.IsNullOrWhiteSpace(riskEventId) && amount > 0;
    }

    /// <summary>
    /// Membaca loan_id, principal, repayment_amount opsional, duration opsional, dan penalty_points dari payload event pinjaman.
    /// </summary>
    // Mendefinisikan metode `TryReadLoanTaken` dengan hasil bertipe `bool`. Membaca loan_id, principal, repayment_amount opsional, duration opsional,
    // dan penalty_points dari payload event pinjaman. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON;
    // Parameter `loanId` bertipe `string` membawa nilai pinjaman identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode;
    // Parameter `principal` bertipe `int` membawa nilai principal; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `repaymentAmount` bertipe `int` membawa nilai repayment nominal; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `duration` bertipe `int` membawa nilai duration; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `penaltyPoints`
    // bertipe `int` membawa nilai penalti poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadLoanTaken(
        // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON.
        JsonElement payload,
        // Parameter `loanId` bertipe `string` membawa nilai pinjaman identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string loanId,
        // Parameter `principal` bertipe `int` membawa nilai principal; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int principal,
        // Parameter `repaymentAmount` bertipe `int` membawa nilai repayment nominal; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int repaymentAmount,
        // Parameter `duration` bertipe `int` membawa nilai duration; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int duration,
        // Parameter `penaltyPoints` bertipe `int` membawa nilai penalti poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int penaltyPoints)
    {
        loanId = string.Empty;
        principal = 0;
        repaymentAmount = 0;
        duration = 0;
        penaltyPoints = 0;

        if (!TryGetString(payload, "loan_id", out loanId) ||
            !TryGetInt32(payload, "principal", out principal) ||
            !TryGetInt32(payload, "penalty_points", out penaltyPoints))
        {
            return false;
        }

        if (!TryGetInt32(payload, "repayment_amount", out repaymentAmount, required: false) &&
            !TryGetInt32(payload, "installment", out repaymentAmount, required: false))
        {
            return false;
        }

        if (!TryGetInt32(payload, "duration_turn", out duration, required: false) ||
            (duration == 0 && !TryGetInt32(payload, "duration_turns", out duration, required: false)))
        {
            return false;
        }

        if (duration == 0 &&
            !TryGetInt32(payload, "duration_days", out duration, required: false))
        {
            return false;
        }

        if (duration == 0)
        {
            duration = 1;
        }

        return !string.IsNullOrWhiteSpace(loanId);
    }

    /// <summary>
    /// Membaca loan_id dan amount dari payload event BayarPinjaman.
    /// </summary>
    // Mendefinisikan metode `TryReadLoanRepay` dengan hasil bertipe `bool`. Membaca loan_id dan amount dari payload event BayarPinjaman. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `loanId` bertipe `string` membawa nilai
    // pinjaman identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang
    // atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
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
    /// Membaca nilai premium dari payload event pembelian Asuransi.
    /// </summary>
    // Mendefinisikan metode `TryReadInsurance` dengan hasil bertipe `bool`. Membaca nilai premium dari payload event pembelian Asuransi. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `premium` bertipe `int` membawa nilai premium;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadInsurance(JsonElement payload, out int premium)
    {
        return TryGetInt32(payload, "premium", out premium);
    }
}
