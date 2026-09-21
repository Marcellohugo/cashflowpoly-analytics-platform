// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsHappinessCalculator.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Kalkulator murni untuk breakdown happiness pemain.
/// </summary>
// Mendefinisikan tipe class `HappinessCalculator` yang mewarisi atau menerapkan `IHappinessCalculator`; sealed mencegah tipe ini diturunkan lagi.
internal sealed class HappinessCalculator : IHappinessCalculator
{
    private static readonly AnalyticsPayloadReader _payloadReader = new();
    /// <summary>
    /// Menghitung breakdown happiness per pemain termasuk donasi, emas, pensiun, dan penalti.
    /// </summary>
    // Mendefinisikan metode `ComputeByPlayer` dengan hasil bertipe `Dictionary<Guid, AnalyticsHappinessBreakdown>`. Menghitung breakdown happiness per
    // pemain termasuk donasi, emas, pensiun, dan penalti. Masukan: Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai
    // sumber riwayat untuk validasi atau perhitungan; Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas
    // yang diturunkan dari event permainan; Parameter `config` bertipe `RulesetConfig?` membawa konfigurasi aturan permainan yang dipakai untuk
    // validasi dan perhitungan; nilai null diizinkan ketika data opsional belum tersedia.
    public Dictionary<Guid, AnalyticsHappinessBreakdown> ComputeByPlayer(
        // Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan.
        List<EventDb> events,
        // Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan.
        List<CashflowProjectionDb> projections,
        // Parameter `config` bertipe `RulesetConfig?` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; nilai null
        // diizinkan ketika data opsional belum tersedia.
        RulesetConfig? config,
        IEnumerable<Guid>? participantIds = null)
    {
        var playerGroups = events.Where(e => e.UserId.HasValue)
            .GroupBy(e => e.UserId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var playerId in participantIds ?? [])
        {
            playerGroups.TryAdd(playerId, []);
        }

        var donationPointsByPlayer = new Dictionary<Guid, double>();
        var goldPointsByPlayer = new Dictionary<Guid, double>();
        var pensionPointsByPlayer = new Dictionary<Guid, double>();

        var hasScoring = config?.Scoring is not null;
        if (hasScoring && config!.Scoring!.DonationRankPoints.Count > 0)
        {
            var tieBreakers = BuildTieBreakerLookup(events);
            donationPointsByPlayer = ComputeDonationPointsFromScoring(events, config.Scoring, tieBreakers);
        }
        else
        {
            foreach (var (playerId, playerEvents) in playerGroups)
            {
                donationPointsByPlayer[playerId] = SumRankAwarded(playerEvents, "PoinPeringkatDonasi");
            }
        }

        if (hasScoring && config!.Scoring!.GoldPointsByQty.Count > 0)
        {
            goldPointsByPlayer = ComputeGoldPointsFromScoring(events, config.Scoring);
        }
        else
        {
            foreach (var (playerId, playerEvents) in playerGroups)
            {
                goldPointsByPlayer[playerId] = SumPointsAwarded(playerEvents, "PoinEmas");
            }
        }

        if (hasScoring && config!.Scoring!.PensionRankPoints.Count > 0)
        {
            pensionPointsByPlayer = ComputePensionPointsFromScoring(events, projections, config, playerGroups.Keys);
        }
        else
        {
            foreach (var (playerId, playerEvents) in playerGroups)
            {
                pensionPointsByPlayer[playerId] = SumRankAwarded(playerEvents, "PoinPeringkatPensiun");
            }
        }

        var result = new Dictionary<Guid, AnalyticsHappinessBreakdown>();
        foreach (var (playerId, playerEvents) in playerGroups)
        {
            donationPointsByPlayer.TryGetValue(playerId, out var donationPoints);
            goldPointsByPlayer.TryGetValue(playerId, out var goldPoints);
            pensionPointsByPlayer.TryGetValue(playerId, out var pensionPoints);

            result[playerId] = ComputeBreakdown(playerEvents, donationPoints, goldPoints, pensionPoints, config);
        }

        return result;
    }

    /// <summary>
    /// Menghitung detail breakdown happiness satu pemain dari event kebutuhan, donasi, emas, tabungan, misi, dan pinjaman.
    /// </summary>
    // Mendefinisikan metode `ComputeBreakdown` dengan hasil bertipe `AnalyticsHappinessBreakdown`. Menghitung detail breakdown happiness satu pemain
    // dari event kebutuhan, donasi, emas, tabungan, misi, dan pinjaman. Masukan: Parameter `playerEvents` bertipe `List<EventDb>` membawa nilai pemain
    // event; Parameter `donationPoints` bertipe `double` membawa nilai donasi poin; Parameter `goldPoints` bertipe `double` membawa nilai emas poin;
    // Parameter `pensionPoints` bertipe `double` membawa nilai pension poin.
    public AnalyticsHappinessBreakdown ComputeBreakdown(
        // Parameter `playerEvents` bertipe `List<EventDb>` membawa nilai pemain event.
        List<EventDb> playerEvents,
        // Parameter `donationPoints` bertipe `double` membawa nilai donasi poin.
        double donationPoints,
        // Parameter `goldPoints` bertipe `double` membawa nilai emas poin.
        double goldPoints,
        // Parameter `pensionPoints` bertipe `double` membawa nilai pension poin.
        double pensionPoints)
    {
        return ComputeBreakdown(playerEvents, donationPoints, goldPoints, pensionPoints, null);
    }

    private AnalyticsHappinessBreakdown ComputeBreakdown(
        // Parameter `playerEvents` bertipe `List<EventDb>` membawa nilai pemain event.
        List<EventDb> playerEvents,
        // Parameter `donationPoints` bertipe `double` membawa nilai donasi poin.
        double donationPoints,
        // Parameter `goldPoints` bertipe `double` membawa nilai emas poin.
        double goldPoints,
        // Parameter `pensionPoints` bertipe `double` membawa nilai pension poin.
        double pensionPoints,
        // Parameter `config` bertipe `RulesetConfig?` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; nilai null
        // diizinkan ketika data opsional belum tersedia.
        RulesetConfig? config)
    {
        var activeNeeds = new List<NeedCard>();
        var loans = new Dictionary<string, LoanState>(StringComparer.OrdinalIgnoreCase);
        double savingGoalPoints = 0;

        // Mengulangi setiap elemen `playerEvents.OrderBy(e => e.SequenceNumber)`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh
        // badan loop dalam ComputeBreakdown.
        foreach (var evt in playerEvents.OrderBy(e => e.SequenceNumber))
        {
            if (evt.ActionType == "Kebutuhan" &&
                _payloadReader.TryReadNeedPurchase(evt.Payload, out _, out var cardId, out var points))
            {
                var purchasedNeed = new NeedCard(cardId, NeedTierClassifier.FromPayloadJson(evt.Payload), points);
                activeNeeds.Add(purchasedNeed);
            }

            if (evt.ActionType == "GunakanOpsiDarurat" &&
                _payloadReader.TryReadSoldNeed(evt.Payload, out var soldNeedCardId))
            {
                var soldIndex = activeNeeds.FindIndex(need =>
                    string.Equals(need.CardId, soldNeedCardId, StringComparison.OrdinalIgnoreCase));
                if (soldIndex >= 0)
                {
                    activeNeeds.RemoveAt(soldIndex);
                }
            }


            if (evt.ActionType == "TujuanFinansial" && _payloadReader.TryReadSavingGoalAchieved(evt.Payload, out var savingPoints))
            {
                savingGoalPoints += savingPoints;
            }

            if ((evt.ActionType == GameActionCatalog.PinjamanSyariah ||
                 evt.ActionType == GameActionCatalog.SetupPinjamanAwal ||
                 IsEmergencyOption(evt.Payload, "TAKE_SHARIA_LOAN")) &&
                _payloadReader.TryReadLoanTaken(evt.Payload, out var loanId, out var principal, out var penaltyPointsValue))
            {
                loans[loanId] = new LoanState(loanId, principal, penaltyPointsValue, 0);
            }

            if (evt.ActionType == "BayarPinjaman" && _payloadReader.TryReadLoanRepay(evt.Payload, out var repayLoanId, out var repayAmount))
            {
                if (loans.TryGetValue(repayLoanId, out var state))
                {
                    loans[repayLoanId] = state with { RepaidAmount = state.RepaidAmount + repayAmount };
                }
            }
        }

        var needPoints = activeNeeds.Sum(need => need.Points);
        var primaryCount = activeNeeds.Count(need => need.Tier == NeedTier.Primary);
        var secondaryCount = activeNeeds.Count(need => need.Tier == NeedTier.Secondary);
        var tertiaryCount = activeNeeds.Count(need => need.Tier == NeedTier.Tertiary);
        var differentBonus = ResolveNeedSetBonus(config, "THREE_DIFFERENT", requiredCount: 3, points: 4);
        var sameBonus = ResolveNeedSetBonus(config, "THREE_SAME", requiredCount: 3, points: 2);
        var mixedSets = differentBonus.RequiredCount == 3
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: Math.Min(primaryCount, Math.Min(secondaryCount, tertiaryCount)) dalam
            // ComputeBreakdown.
            ? Math.Min(primaryCount, Math.Min(secondaryCount, tertiaryCount))
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: 0; dalam ComputeBreakdown.
            : 0;
        // Kedua pola dihitung mandiri. Contoh rulebook: 3 primer, 2 sekunder,
        // dan 1 tersier memperoleh bonus 4 + 2, bukan hanya bonus campuran 4.
        // Menyiapkan variabel lokal `sameSets` untuk nilai same sets dengan penjumlahan/penggabungan antara `(primaryCount / sameBonus.RequiredCount) +
        // (secondaryCount / sameBonus.RequiredCount)` dan `(tertiaryCount / sameBonus.RequiredCount)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sameSets = (primaryCount / sameBonus.RequiredCount) +
                       (secondaryCount / sameBonus.RequiredCount) +
                       (tertiaryCount / sameBonus.RequiredCount);
        var needSetBonusPoints = mixedSets * differentBonus.Points + sameSets * sameBonus.Points;

        var missions = AnalyticsCollectionMissions.Evaluate(playerEvents, config);
        var missionPenaltyPoints = missions.Where(mission => !mission.Complete).Sum(mission => (double)mission.PenaltyPoints);
        var missionRewardPoints = missions.Where(mission => mission.Complete).Sum(mission => (double)mission.RewardPoints);
        var initialHappinessPoints = config?.InitialHappiness ?? 0;

        var loanPenaltyPoints = 0d;
        var hasUnpaidLoan = false;
        // Mengulangi setiap elemen `loans.Values`; elemen saat ini disimpan sebagai `loan` bertipe `var` untuk diproses oleh badan loop dalam
        // ComputeBreakdown.
        foreach (var loan in loans.Values)
        {
            if (loan.RepaidAmount < loan.Principal)
            {
                hasUnpaidLoan = true;
                loanPenaltyPoints += loan.PenaltyPoints;
            }
        }

        var savingGoalPointsEffective = hasUnpaidLoan ? 0 : savingGoalPoints;

        var total = initialHappinessPoints + missionRewardPoints + needPoints +
                    needSetBonusPoints +
                    donationPoints +
                    goldPoints +
                    pensionPoints +
                    savingGoalPointsEffective -
                    missionPenaltyPoints -
                    loanPenaltyPoints;

        return new AnalyticsHappinessBreakdown(
            total,
            needPoints,
            needSetBonusPoints,
            donationPoints,
            goldPoints,
            pensionPoints,
            savingGoalPointsEffective,
            missionPenaltyPoints,
            loanPenaltyPoints,
            hasUnpaidLoan,
            initialHappinessPoints,
            missionRewardPoints);
    }

    private static NeedSetBonus ResolveNeedSetBonus(
        // Parameter `config` bertipe `RulesetConfig?` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; nilai null
        // diizinkan ketika data opsional belum tersedia.
        RulesetConfig? config,
        // Parameter `patternCode` bertipe `string` membawa nilai pattern kode.
        string patternCode,
        // Parameter `requiredCount` bertipe `int` membawa nilai required jumlah.
        int requiredCount,
        // Parameter `points` bertipe `int` membawa nilai poin.
        int points)
    {
        var configured = config?.NeedSetBonuses.FirstOrDefault(item =>
            string.Equals(item.PatternCode, patternCode, StringComparison.OrdinalIgnoreCase));
        return configured is null
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: new NeedSetBonus(requiredCount, points) dalam ResolveNeedSetBonus.
            ? new NeedSetBonus(requiredCount, points)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: new NeedSetBonus(configured.RequiredCount, configured.Points); dalam
            // ResolveNeedSetBonus.
            : new NeedSetBonus(configured.RequiredCount, configured.Points);
    }

    private Dictionary<Guid, int> BuildTieBreakerLookup(IEnumerable<EventDb> events)
    {
        return events.Where(e => e.UserId.HasValue && e.ActionType == "BagikanTieBreaker")
            .OrderBy(e => e.SequenceNumber)
            .GroupBy(e => e.UserId!.Value)
            .ToDictionary(
                // Parameter `g` bertipe `` membawa nilai g.
                g => g.Key,
                // Parameter `g` bertipe `` membawa nilai g.
                g =>
                {
                    var last = g.Last();
                    return _payloadReader.TryReadTieBreaker(last.Payload, out var number) ? number : 0;
                });
    }

    private Dictionary<Guid, double> ComputeDonationPointsFromScoring(
        // Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan.
        List<EventDb> events,
        // Parameter `scoring` bertipe `RulesetScoringConfig` membawa nilai scoring.
        RulesetScoringConfig scoring,
        // Parameter `tieBreakers` bertipe `Dictionary<Guid, int>` membawa nilai tie breakers.
        Dictionary<Guid, int> tieBreakers)
    {
        var pointsByRank = scoring.DonationRankPoints.ToDictionary(item => item.Rank, item => item.Points);
        var result = new Dictionary<Guid, double>();

        var fridayGroups = events.Where(e => e.ActionType == "JumatBerkah" && e.UserId.HasValue)
            .GroupBy(e => e.DayIndex);

        // Mengulangi setiap elemen `fridayGroups`; elemen saat ini disimpan sebagai `dayGroup` bertipe `var` untuk diproses oleh badan loop dalam
        // ComputeDonationPointsFromScoring.
        foreach (var dayGroup in fridayGroups)
        {
            var totals = dayGroup
                .GroupBy(e => e.UserId!.Value)
                .Select(g =>
                {
                    var total = g.Sum(e => _payloadReader.TryReadAmount(e.Payload, out var amount) ? amount : 0);
                    tieBreakers.TryGetValue(g.Key, out var tieNumber);
                    return new { UserId = g.Key, Amount = total, Tie = tieNumber };
                })
                .Where(item => item.Amount > 0)
                .OrderByDescending(item => item.Amount)
                .ThenByDescending(item => item.Tie)
                .ThenBy(item => item.UserId)
                .ToList();

            var rank = 1;
            // Mengulangi setiap elemen `totals`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
            // ComputeDonationPointsFromScoring.
            foreach (var item in totals)
            {
                if (pointsByRank.TryGetValue(rank, out var points) && points > 0)
                {
                    result[item.UserId] = result.TryGetValue(item.UserId, out var existing) ? existing + points : points;
                }

                rank += 1;
            }
        }

        return result;
    }

    private Dictionary<Guid, double> ComputeGoldPointsFromScoring(
        // Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan.
        List<EventDb> events,
        // Parameter `scoring` bertipe `RulesetScoringConfig` membawa nilai scoring.
        RulesetScoringConfig scoring)
    {
        var table = scoring.GoldPointsByQty
            .OrderBy(item => item.Qty)
            .ToList();

        var goldQtyByPlayer = events.Where(e =>
                (e.ActionType == GameActionCatalog.InvestasiEmas ||
                 e.ActionType == GameActionCatalog.JualEmas ||
                 e.ActionType == GameActionCatalog.SetupEmasAwal ||
                 IsEmergencyOption(e.Payload, "SELL_GOLD")) &&
                e.UserId.HasValue)
            .GroupBy(e => e.UserId!.Value)
            .ToDictionary(
                // Parameter `g` bertipe `` membawa nilai g.
                g => g.Key,
                // Parameter `g` bertipe `` membawa nilai g.
                g => g.Sum(e =>
                {
                    if (e.ActionType == GameActionCatalog.SetupEmasAwal)
                    {
                        return TryReadInt32(e.Payload, "qty", out var initialQty) ? initialQty : 1;
                    }

                    if (IsEmergencyOption(e.Payload, "SELL_GOLD"))
                    {
                        return TryReadInt32(e.Payload, "qty", out var emergencyQty) ? -emergencyQty : 0;
                    }

                    if (!_payloadReader.TryReadGoldTrade(e.Payload, out var tradeType, out var qty))
                    {
                        return 0;
                    }

                    return e.ActionType == GameActionCatalog.JualEmas ||
                           string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase)
                        ? -qty
                        : qty;
                }));

        var result = new Dictionary<Guid, double>();
        foreach (var (playerId, qty) in goldQtyByPlayer)
        {
            var points = ResolvePointsByQty(qty, table);
            if (points > 0)
            {
                result[playerId] = points;
            }
        }

        return result;
    }

    private Dictionary<Guid, double> ComputePensionPointsFromScoring(
        // Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan.
        List<EventDb> events,
        // Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan.
        List<CashflowProjectionDb> projections,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        IEnumerable<Guid> participantIds)
    {
        var pointsByRank = config.Scoring?.PensionRankPoints.ToDictionary(item => item.Rank, item => item.Points)
                           // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: new Dictionary<int, int>(); dalam ComputePensionPointsFromScoring.
                           ?? new Dictionary<int, int>();

        var result = new Dictionary<Guid, double>();
        foreach (var (playerId, rank) in ComputePensionRanks(events, projections, config, participantIds))
        {
            if (pointsByRank.TryGetValue(rank, out var points) && points > 0)
            {
                result[playerId] = points;
            }
        }

        return result;
    }

    public Dictionary<Guid, int> ComputePensionRanks(
        // Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan.
        List<EventDb> events,
        // Parameter `projections` bertipe `List<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan.
        List<CashflowProjectionDb> projections,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        IEnumerable<Guid>? participantIds = null)
    {
        var tieBreakers = BuildTieBreakerLookup(events);

        var cashByPlayer = projections
            .GroupBy(p => p.UserId)
            .ToDictionary(
                // Parameter `g` bertipe `` membawa nilai g.
                g => g.Key,
                // Parameter `g` bertipe `` membawa nilai g.
                g => g.Sum(p => p.Direction == "IN" ? p.Amount : -p.Amount));

        var savingByPlayer = events.Where(evt => evt.UserId.HasValue).GroupBy(evt => evt.UserId!.Value)
            .ToDictionary(group => group.Key, group => new SavingGoalCalculator().Compute(group, initialSaving: config.InitialSaving).CoinsSaved);
        var ingredientValueByPlayer = BuildIngredientValueLookup(events);
        var players = events.Where(e => e.UserId.HasValue).Select(e => e.UserId!.Value)
            .Union(projections.Select(projection => projection.UserId)).Union(participantIds ?? []).ToList();
        var ranking = players.Select(playerId =>
            {
                cashByPlayer.TryGetValue(playerId, out var cash);
                var saving = savingByPlayer.GetValueOrDefault(playerId, config.InitialSaving);
                ingredientValueByPlayer.TryGetValue(playerId, out var ingredientValue);
                tieBreakers.TryGetValue(playerId, out var tieNumber);
                return new { UserId = playerId, PensionFund = config.StartingCash + cash + saving + ingredientValue, Tie = tieNumber };
            })
            .OrderByDescending(item => item.PensionFund)
            .ThenByDescending(item => item.Tie)
            .ThenBy(item => item.UserId)
            .ToList();

        return ranking
            .Select((item, index) => new { item.UserId, Rank = index + 1 })
            .ToDictionary(item => item.UserId, item => item.Rank);
    }

    private Dictionary<Guid, int> BuildIngredientValueLookup(IEnumerable<EventDb> events)
    {
        var inventoryByPlayer = new Dictionary<Guid, Dictionary<string, int>>();
        // Mengulangi setiap elemen `events.Where(e => e.UserId.HasValue)`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh badan
        // loop dalam BuildIngredientValueLookup.
        foreach (var evt in events.Where(e => e.UserId.HasValue))
        {
            var playerId = evt.UserId!.Value;
            if (!inventoryByPlayer.TryGetValue(playerId, out var inventory))
            {
                inventory = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                inventoryByPlayer[playerId] = inventory;
            }

            if ((evt.ActionType == "BahanMasakan" || evt.ActionType == "SetupBahanAwal") &&
                _payloadReader.TryReadIngredientPurchase(evt.Payload, out var cardId, out var setupAmount))
            {
                inventory[cardId] = inventory.TryGetValue(cardId, out var current) ? current + 1 : 1;
            }
            else if (evt.ActionType == "BuangBahanMasakan" &&
                     TryReadString(evt.Payload, "card_id", out var discardedCardId))
            {
                var qty = TryReadInt32(evt.Payload, "amount", out var amount) ? Math.Max(1, amount) : 1;
                inventory[discardedCardId] = Math.Max(0, inventory.TryGetValue(discardedCardId, out var current) ? current - qty : 0);
            }
            else if (evt.ActionType == "JualMasakan" &&
                     _payloadReader.TryReadOrderClaim(evt.Payload, out var requiredCards, out _))
            {
                // Mengulangi setiap elemen `requiredCards`; elemen saat ini disimpan sebagai `requiredCard` bertipe `var` untuk diproses oleh badan loop dalam
                // BuildIngredientValueLookup.
                foreach (var requiredCard in requiredCards)
                {
                    inventory[requiredCard] = Math.Max(0, inventory.TryGetValue(requiredCard, out var current) ? current - 1 : 0);
                }
            }
        }

        return inventoryByPlayer.ToDictionary(
            // Parameter `pair` bertipe `` membawa nilai pair.
            pair => pair.Key,
            // Parameter `pair` bertipe `` membawa nilai pair.
            pair => pair.Value.Values.Sum());
    }

    private static bool TryReadString(string payloadJson, string propertyName, out string value)
    {
        value = string.Empty;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty(propertyName, out var property) ||
                property.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            value = property.GetString() ?? string.Empty;
            return !string.IsNullOrWhiteSpace(value);
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadString.
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool IsEmergencyOption(string payloadJson, string optionType)
        => TryReadString(payloadJson, "option_type", out var value) &&
           value.Equals(optionType, StringComparison.OrdinalIgnoreCase);

    private static bool TryReadInt32(string payloadJson, string propertyName, out int value)
    {
        value = 0;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            return doc.RootElement.TryGetProperty(propertyName, out var property) &&
                   property.ValueKind == JsonValueKind.Number &&
                   property.TryGetInt32(out value);
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadInt32.
        catch (JsonException)
        {
            return false;
        }
    }

    private int ResolvePointsByQty(int qty, IReadOnlyList<QtyPoint> table)
    {
        if (qty <= 0 || table.Count == 0)
        {
            return 0;
        }

        var maxTableQty = table.Max(x => x.Qty);
        if (qty <= maxTableQty)
        {
            var bestPoints = 0;
            // Mengulangi setiap elemen `table`; elemen saat ini disimpan sebagai `entry` bertipe `var` untuk diproses oleh badan loop dalam ResolvePointsByQty.
            foreach (var entry in table)
            {
                if (entry.Qty <= qty && entry.Points > bestPoints)
                {
                    bestPoints = entry.Points;
                }
            }
            return bestPoints;
        }
        return table.First(x => x.Qty == maxTableQty).Points;
    }

    public double SumRankAwarded(IEnumerable<EventDb> events, string actionType)
    {
        return events.Where(e => e.ActionType == actionType)
            .Sum(e => _payloadReader.TryReadRankAwarded(e.Payload, out _, out var points) ? points : 0);
    }

    public double SumPointsAwarded(IEnumerable<EventDb> events, string actionType)
    {
        return events.Where(e => e.ActionType == actionType)
            .Sum(e => _payloadReader.TryReadPointsAwarded(e.Payload, out var points) ? points : 0);
    }

    private sealed record LoanState(
        // Parameter `LoanId` bertipe `string` membawa nilai pinjaman identitas.
        string LoanId,
        // Parameter `Principal` bertipe `int` membawa nilai principal.
        int Principal,
        // Parameter `PenaltyPoints` bertipe `int` membawa nilai penalti poin.
        int PenaltyPoints,
        // Parameter `RepaidAmount` bertipe `double` membawa nilai dilunasi nominal.
        double RepaidAmount);

    private sealed record NeedSetBonus(int RequiredCount, int Points);
    private sealed record NeedCard(string CardId, NeedTier Tier, double Points);
}
