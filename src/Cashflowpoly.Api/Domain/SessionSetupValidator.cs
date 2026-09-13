// Fungsi file: Memvalidasi pembagian awal fisik pemain terhadap ruleset sesi.
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public static class SessionSetupValidator
{
    public static SessionSetupRequest Normalize(SessionSetupRequest request)
    {
        var players = (request.Players ?? [])
            .Select(player => new SessionPlayerSetupRequest(
                player.SessionPlayerId,
                (player.TieBreakerCode ?? string.Empty).Trim(),
                (player.IngredientCardId ?? string.Empty).Trim(),
                player.GoldQuantity,
                NormalizeOptional(player.MissionId),
                NormalizeOptional(player.LoanCode),
                NormalizeOptional(player.InsuranceProductCode)))
            .OrderBy(player => player.SessionPlayerId)
            .ToList();

        return new SessionSetupRequest((request.ClientRequestId ?? string.Empty).Trim(), players);
    }

    public static List<ErrorDetail> Validate(
        // Parameter `request` bertipe `SessionSetupRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        SessionSetupRequest request,
        // Parameter `definition` bertipe `RulesetDefinitionDto` membawa definisi terstruktur komponen serta parameter aturan permainan.
        RulesetDefinitionDto definition,
        // Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan.
        string mode,
        // Parameter `sessionPlayers` bertipe `IReadOnlyCollection<SessionPlayerDb>` membawa nilai sesi pemain.
        IReadOnlyCollection<SessionPlayerDb> sessionPlayers)
    {
        var normalized = Normalize(request);
        var errors = new List<ErrorDetail>();

        if (string.IsNullOrWhiteSpace(normalized.ClientRequestId))
        {
            errors.Add(new ErrorDetail("client_request_id", "REQUIRED"));
        }
        else if (normalized.ClientRequestId.Length > 120)
        {
            errors.Add(new ErrorDetail("client_request_id", "MAX_LENGTH"));
        }

        if (normalized.Players.Count != sessionPlayers.Count)
        {
            errors.Add(new ErrorDetail("players", "COUNT_MISMATCH"));
        }

        var expectedPlayerIds = sessionPlayers.Select(player => player.SessionPlayerId).ToHashSet();
        var assignedPlayerIds = new HashSet<Guid>();
        var assignedTieCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var assignedTieNumbers = new HashSet<int>();
        var assignedMissionIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var ingredientCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var loanCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var insuranceCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        var tieBreakers = definition.TieBreakers
            .Where(item => !string.IsNullOrWhiteSpace(item.TieBreakerCode))
            .GroupBy(item => item.TieBreakerCode, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var ingredients = definition.Ingredients
            .Where(item => !string.IsNullOrWhiteSpace(item.Id))
            .GroupBy(item => item.Id, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var missions = definition.CollectionMissions
            .Where(item => !string.IsNullOrWhiteSpace(item.Id))
            .GroupBy(item => item.Id, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var loans = definition.ShariaLoans
            .Where(item => !string.IsNullOrWhiteSpace(item.LoanCode))
            .GroupBy(item => item.LoanCode, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var insuranceProducts = definition.InsuranceProducts
            .Where(item => !string.IsNullOrWhiteSpace(item.ProductCode))
            .GroupBy(item => item.ProductCode, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        var isMahir = string.Equals(mode, "MAHIR", StringComparison.OrdinalIgnoreCase);
        var loanRequired = isMahir && definition.Settings.LoanEnabled;
        var insuranceRequired = isMahir && definition.Settings.InsuranceEnabled;

        for (var index = 0; index < normalized.Players.Count; index++)
        {
            var player = normalized.Players[index];
            var field = $"players[{index}]";

            if (player.SessionPlayerId == Guid.Empty || !expectedPlayerIds.Contains(player.SessionPlayerId))
            {
                errors.Add(new ErrorDetail($"{field}.session_player_id", "UNKNOWN_REFERENCE"));
            }
            else if (!assignedPlayerIds.Add(player.SessionPlayerId))
            {
                errors.Add(new ErrorDetail($"{field}.session_player_id", "DUPLICATE"));
            }

            if (!tieBreakers.TryGetValue(player.TieBreakerCode, out var tieBreaker))
            {
                errors.Add(new ErrorDetail($"{field}.tie_breaker_code", "UNKNOWN_REFERENCE"));
            }
            else
            {
                if (!assignedTieCodes.Add(tieBreaker.TieBreakerCode) || !assignedTieNumbers.Add(tieBreaker.TieNumber))
                {
                    errors.Add(new ErrorDetail($"{field}.tie_breaker_code", "DUPLICATE"));
                }
            }

            if (!ingredients.TryGetValue(player.IngredientCardId, out var ingredient))
            {
                errors.Add(new ErrorDetail($"{field}.ingredient_card_id", "UNKNOWN_REFERENCE"));
            }
            else
            {
                IncrementCount(ingredientCounts, ingredient.Id);
            }

            if (player.GoldQuantity != 1)
            {
                errors.Add(new ErrorDetail($"{field}.gold_quantity", "MUST_EQUAL_ONE"));
            }

            if (string.IsNullOrWhiteSpace(player.MissionId) || !missions.ContainsKey(player.MissionId))
            {
                errors.Add(new ErrorDetail($"{field}.mission_id", string.IsNullOrWhiteSpace(player.MissionId) ? "REQUIRED" : "UNKNOWN_REFERENCE"));
            }
            else if (!assignedMissionIds.Add(player.MissionId))
            {
                errors.Add(new ErrorDetail($"{field}.mission_id", "DUPLICATE"));
            }

            ValidateOptionalCard(
                player.LoanCode,
                loanRequired,
                loans,
                loanCounts,
                $"{field}.loan_code",
                // Parameter `item` bertipe `` membawa nilai elemen.
                item => item.LoanCode,
                errors);
            ValidateOptionalCard(
                player.InsuranceProductCode,
                insuranceRequired,
                insuranceProducts,
                insuranceCounts,
                $"{field}.insurance_product_code",
                // Parameter `item` bertipe `` membawa nilai elemen.
                item => item.ProductCode,
                errors);
        }

        if (!expectedPlayerIds.SetEquals(assignedPlayerIds))
        {
            errors.Add(new ErrorDetail("players", "MISSING_PARTICIPANT"));
        }

        if (!Enumerable.Range(1, sessionPlayers.Count).All(assignedTieNumbers.Contains))
        {
            errors.Add(new ErrorDetail("players.tie_breaker_code", "ORDER_SEQUENCE_INCOMPLETE"));
        }

        ValidatePhysicalCardQuantity(ingredientCounts, ingredients, item => item.CardQty ?? 5, "players.ingredient_card_id", errors);
        ValidatePhysicalCardQuantity(loanCounts, loans, item => item.CardQty ?? 1, "players.loan_code", errors);
        // Asuransi mode mahir berada di sisi belakang kartu Tie Breaker, sehingga
        // card_qty produk asuransi memang 0 dan tidak mewakili stok kartu terpisah.

        return errors.Distinct().ToList();
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static void IncrementCount(Dictionary<string, int> counts, string code)
    {
        counts.TryGetValue(code, out var count);
        counts[code] = count + 1;
    }

    private static void ValidateOptionalCard<T>(
        // Parameter `code` bertipe `string?` membawa nilai kode; nilai null diizinkan ketika data opsional belum tersedia.
        string? code,
        // Parameter `required` bertipe `bool` membawa nilai required.
        bool required,
        // Parameter `catalog` bertipe `IReadOnlyDictionary<string, T>` membawa nilai catalog.
        IReadOnlyDictionary<string, T> catalog,
        // Parameter `counts` bertipe `Dictionary<string, int>` membawa nilai counts.
        Dictionary<string, int> counts,
        // Parameter `field` bertipe `string` membawa nilai field.
        string field,
        // Parameter `getCode` bertipe `Func<T, string>` membawa nilai get kode.
        Func<T, string> getCode,
        // Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
        List<ErrorDetail> errors)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            if (required)
            {
                errors.Add(new ErrorDetail(field, "REQUIRED"));
            }

            return;
        }

        if (!required)
        {
            errors.Add(new ErrorDetail(field, "DISALLOWED_FOR_MODE"));
            return;
        }

        if (!catalog.TryGetValue(code, out var item))
        {
            errors.Add(new ErrorDetail(field, "UNKNOWN_REFERENCE"));
            return;
        }

        IncrementCount(counts, getCode(item));
    }

    private static void ValidatePhysicalCardQuantity<T>(
        // Parameter `assignedCounts` bertipe `IReadOnlyDictionary<string, int>` membawa nilai assigned counts.
        IReadOnlyDictionary<string, int> assignedCounts,
        // Parameter `catalog` bertipe `IReadOnlyDictionary<string, T>` membawa nilai catalog.
        IReadOnlyDictionary<string, T> catalog,
        // Parameter `getAvailableQuantity` bertipe `Func<T, int>` membawa nilai get tersedia jumlah.
        Func<T, int> getAvailableQuantity,
        // Parameter `field` bertipe `string` membawa nilai field.
        string field,
        // Parameter `errors` bertipe `List<ErrorDetail>` membawa nilai kesalahan.
        List<ErrorDetail> errors)
    {
        foreach (var (code, assignedCount) in assignedCounts)
        {
            if (catalog.TryGetValue(code, out var item) && assignedCount > Math.Max(0, getAvailableQuantity(item)))
            {
                errors.Add(new ErrorDetail(field, "CARD_QUANTITY_EXCEEDED"));
            }
        }
    }
}
