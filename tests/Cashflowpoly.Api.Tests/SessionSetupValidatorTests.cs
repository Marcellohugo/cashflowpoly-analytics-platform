// Fungsi file: Menguji pembagian awal pemain terhadap ruleset dan mode sesi.
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class SessionSetupValidatorTests
{
    private static readonly Guid FirstPlayerId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    private static readonly Guid SecondPlayerId = Guid.Parse("10000000-0000-0000-0000-000000000002");

    [Fact]
    public void Validate_AcceptsCompleteMahirSetupFromRuleset()
    {
        var errors = SessionSetupValidator.Validate(
            BuildRequest(),
            BuildDefinition(advancedEnabled: true),
            "MAHIR",
            BuildPlayers());

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_RejectsDuplicatePhysicalAssignmentsAndMissingPlayer()
    {
        var request = new SessionSetupRequest(
            "setup-duplicate",
            [
                BuildAssignment(FirstPlayerId, "tie-1", "ingredient-meat", "mission-1"),
                BuildAssignment(FirstPlayerId, "tie-1", "ingredient-meat", "mission-1")
            ]);

        var errors = SessionSetupValidator.Validate(
            request,
            BuildDefinition(advancedEnabled: true),
            "MAHIR",
            BuildPlayers());

        Assert.Contains(errors, error => error.Field.EndsWith("session_player_id", StringComparison.Ordinal) && error.Issue == "DUPLICATE");
        Assert.Contains(errors, error => error.Field == "players" && error.Issue == "MISSING_PARTICIPANT");
        Assert.Contains(errors, error => error.Field.EndsWith("tie_breaker_code", StringComparison.Ordinal) && error.Issue == "DUPLICATE");
        Assert.Contains(errors, error => error.Field.EndsWith("mission_id", StringComparison.Ordinal) && error.Issue == "DUPLICATE");
    }

    [Fact]
    public void Validate_RejectsAdvancedCardsInPemulaMode()
    {
        var errors = SessionSetupValidator.Validate(
            BuildRequest(),
            BuildDefinition(advancedEnabled: false),
            "PEMULA",
            BuildPlayers());

        Assert.Contains(errors, error => error.Field.EndsWith("loan_code", StringComparison.Ordinal) && error.Issue == "DISALLOWED_FOR_MODE");
        Assert.Contains(errors, error => error.Field.EndsWith("insurance_product_code", StringComparison.Ordinal) && error.Issue == "DISALLOWED_FOR_MODE");
    }

    [Fact]
    public void Validate_RequiresExactlyOneGoldCardAndKnownRulesetCards()
    {
        var request = new SessionSetupRequest(
            "setup-invalid-cards",
            [
                BuildAssignment(FirstPlayerId, "tie-1", "ingredient-unknown", "mission-1", goldQuantity: 2),
                BuildAssignment(SecondPlayerId, "tie-2", "ingredient-meat", "mission-unknown")
            ]);

        var errors = SessionSetupValidator.Validate(
            request,
            BuildDefinition(advancedEnabled: true),
            "MAHIR",
            BuildPlayers());

        Assert.Contains(errors, error => error.Field.EndsWith("ingredient_card_id", StringComparison.Ordinal) && error.Issue == "UNKNOWN_REFERENCE");
        Assert.Contains(errors, error => error.Field.EndsWith("gold_quantity", StringComparison.Ordinal) && error.Issue == "MUST_EQUAL_ONE");
        Assert.Contains(errors, error => error.Field.EndsWith("mission_id", StringComparison.Ordinal) && error.Issue == "UNKNOWN_REFERENCE");
    }

    private static SessionSetupRequest BuildRequest() =>
        new(
            "setup-valid",
            [
                BuildAssignment(FirstPlayerId, "tie-1", "ingredient-meat", "mission-1"),
                BuildAssignment(SecondPlayerId, "tie-2", "ingredient-flour", "mission-2")
            ]);

    private static SessionPlayerSetupRequest BuildAssignment(
        Guid playerId,
        string tieBreakerCode,
        string ingredientCardId,
        string missionId,
        int goldQuantity = 1) =>
        new(playerId, tieBreakerCode, ingredientCardId, goldQuantity, missionId, "loan-10", "multi-risk");

    private static List<SessionPlayerDb> BuildPlayers() =>
    [
        new() { SessionPlayerId = FirstPlayerId, UserId = Guid.NewGuid(), PlayerOrder = 1 },
        new() { SessionPlayerId = SecondPlayerId, UserId = Guid.NewGuid(), PlayerOrder = 2 }
    ];

    private static RulesetDefinitionDto BuildDefinition(bool advancedEnabled) =>
        new()
        {
            Mode = advancedEnabled ? "MAHIR" : "PEMULA",
            Settings = new RulesetSettingsDto
            {
                LoanEnabled = advancedEnabled,
                InsuranceEnabled = advancedEnabled
            },
            TieBreakers =
            [
                new RulesetTieBreakerDto { TieBreakerCode = "tie-1", TieNumber = 1, CardQty = 1 },
                new RulesetTieBreakerDto { TieBreakerCode = "tie-2", TieNumber = 2, CardQty = 1 }
            ],
            Ingredients =
            [
                new RulesetIngredientDto { Id = "ingredient-meat", Nama = "Daging", HargaBeli = 5, CardQty = 2 },
                new RulesetIngredientDto { Id = "ingredient-flour", Nama = "Tepung", HargaBeli = 2, CardQty = 2 }
            ],
            CollectionMissions =
            [
                new RulesetCollectionMissionDto { Id = "mission-1", Nama = "Misi 1" },
                new RulesetCollectionMissionDto { Id = "mission-2", Nama = "Misi 2" }
            ],
            ShariaLoans =
            [
                new RulesetShariaLoanDto { LoanCode = "loan-10", Principal = 10, CardQty = 2 }
            ],
            InsuranceProducts =
            [
                // Sesuai rulebook: asuransi berada di belakang kartu Tie Breaker,
                // bukan stok kartu asuransi terpisah.
                new RulesetInsuranceProductDto { ProductCode = "multi-risk", UsageLimit = 1, CardQty = 0 }
            ]
        };
}
