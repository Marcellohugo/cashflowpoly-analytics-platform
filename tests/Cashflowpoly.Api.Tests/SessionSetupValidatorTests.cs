// Fungsi file: Menguji pembagian awal pemain terhadap ruleset dan mode sesi.
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `SessionSetupValidatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionSetupValidatorTests
// Membuka scope tipe SessionSetupValidatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `Guid`: `FirstPlayerId` menyimpan nilai first pemain identitas dengan nilai awal memanggil `Guid.Parse` dengan
    // `”10000000-0000-0000-0000-000000000001”`. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly Guid FirstPlayerId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    // Mendeklarasikan field bertipe `Guid`: `SecondPlayerId` menyimpan nilai second pemain identitas dengan nilai awal memanggil `Guid.Parse` dengan
    // `”10000000-0000-0000-0000-000000000002”`. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly Guid SecondPlayerId = Guid.Parse("10000000-0000-0000-0000-000000000002");

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Validate_AcceptsCompleteMahirSetupFromRuleset` dengan hasil bertipe `void`; operasi ini menangani validate accepts
    // complete mahir setup dari aturan.
    public void Validate_AcceptsCompleteMahirSetupFromRuleset()
    // Membuka scope metode Validate_AcceptsCompleteMahirSetupFromRuleset; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Validate_AcceptsCompleteMahirSetupFromRuleset.
    {
        // Menyiapkan variabel lokal `errors` untuk nilai kesalahan dengan memanggil `SessionSetupValidator.Validate` dengan `BuildRequest()`,
        // `BuildDefinition(advancedEnabled: true)`, `”MAHIR”`, `BuildPlayers()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var errors = SessionSetupValidator.Validate(
            // Meneruskan memanggil `BuildRequest` dengan tanpa argumen sebagai argumen ke `SessionSetupValidator.Validate`.
            BuildRequest(),
            // Meneruskan memanggil `BuildDefinition` dengan `true` sebagai argumen ke `SessionSetupValidator.Validate`; Meneruskan true, yaitu kondisi
            // aktif/terpenuhi sebagai argumen bernama `advancedEnabled`.
            BuildDefinition(advancedEnabled: true),
            // Meneruskan nilai literal `”MAHIR”` sebagai argumen ke `SessionSetupValidator.Validate`.
            "MAHIR",
            // Meneruskan memanggil `BuildPlayers` dengan tanpa argumen sebagai argumen ke `SessionSetupValidator.Validate`.
            BuildPlayers());

        // Menjalankan pemeriksaan hasil dengan `Assert.Empty` menggunakan `errors`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // Validate_AcceptsCompleteMahirSetupFromRuleset.
        Assert.Empty(errors);
    // Menutup scope metode Validate_AcceptsCompleteMahirSetupFromRuleset; bagian berikut berada di luar batas blok tersebut dalam
    // Validate_AcceptsCompleteMahirSetupFromRuleset.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Validate_RejectsDuplicatePhysicalAssignmentsAndMissingPlayer` dengan hasil bertipe `void`; operasi ini menangani validate
    // rejects duplicate physical assignments dan missing pemain.
    public void Validate_RejectsDuplicatePhysicalAssignmentsAndMissingPlayer()
    // Membuka scope metode Validate_RejectsDuplicatePhysicalAssignmentsAndMissingPlayer; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam Validate_RejectsDuplicatePhysicalAssignmentsAndMissingPlayer.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan objek baru bertipe
        // `SessionSetupRequest` dengan argumen ( ”setup-duplicate”, [ BuildAssignment(FirstPlayerId, ”tie-1”, ”ingredient-meat”, ”mission-1”),
        // BuildAssignment(FirstPlayerId, ”tie-1”, ”ingredient-meat”, ”miss.... Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = new SessionSetupRequest(
            // Meneruskan nilai literal `”setup-duplicate”` sebagai argumen ke konstruktor `SessionSetupRequest`.
            "setup-duplicate",
            // Meneruskan koleksi berisi BuildAssignment(FirstPlayerId, ”tie-1”, ”ingredien..., BuildAssignment(FirstPlayerId, ”tie-1”, ”ingredien... sebagai
            // argumen ke konstruktor `SessionSetupRequest`.
            [
                // Meneruskan `FirstPlayerId` (nilai first pemain identitas) sebagai argumen ke `BuildAssignment`; Meneruskan nilai literal `”tie-1”` sebagai
                // argumen ke `BuildAssignment`; Meneruskan nilai literal `”ingredient-meat”` sebagai argumen ke `BuildAssignment`; Meneruskan nilai literal
                // `”mission-1”` sebagai argumen ke `BuildAssignment`.
                BuildAssignment(FirstPlayerId, "tie-1", "ingredient-meat", "mission-1"),
                // Meneruskan `FirstPlayerId` (nilai first pemain identitas) sebagai argumen ke `BuildAssignment`; Meneruskan nilai literal `”tie-1”` sebagai
                // argumen ke `BuildAssignment`; Meneruskan nilai literal `”ingredient-meat”` sebagai argumen ke `BuildAssignment`; Meneruskan nilai literal
                // `”mission-1”` sebagai argumen ke `BuildAssignment`.
                BuildAssignment(FirstPlayerId, "tie-1", "ingredient-meat", "mission-1")
            // Meneruskan koleksi berisi BuildAssignment(FirstPlayerId, ”tie-1”, ”ingredien..., BuildAssignment(FirstPlayerId, ”tie-1”, ”ingredien... sebagai
            // argumen ke konstruktor `SessionSetupRequest`.
            ]);

        // Menyiapkan variabel lokal `errors` untuk nilai kesalahan dengan memanggil `SessionSetupValidator.Validate` dengan `request`,
        // `BuildDefinition(advancedEnabled: true)`, `”MAHIR”`, `BuildPlayers()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var errors = SessionSetupValidator.Validate(
            // Meneruskan `request` (data masukan permintaan yang akan divalidasi atau diteruskan ke layanan) sebagai argumen ke
            // `SessionSetupValidator.Validate`.
            request,
            // Meneruskan memanggil `BuildDefinition` dengan `true` sebagai argumen ke `SessionSetupValidator.Validate`; Meneruskan true, yaitu kondisi
            // aktif/terpenuhi sebagai argumen bernama `advancedEnabled`.
            BuildDefinition(advancedEnabled: true),
            // Meneruskan nilai literal `”MAHIR”` sebagai argumen ke `SessionSetupValidator.Validate`.
            "MAHIR",
            // Meneruskan memanggil `BuildPlayers` dengan tanpa argumen sebagai argumen ke `SessionSetupValidator.Validate`.
            BuildPlayers());

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `errors`, `error =>
        // error.Field.EndsWith(”session_player_id”, StringComparison.Ordinal) && error.Issue == ”DUPLICATE”` dalam
        // Validate_RejectsDuplicatePhysicalAssignmentsAndMissingPlayer.
        Assert.Contains(errors, error => error.Field.EndsWith("session_player_id", StringComparison.Ordinal) && error.Issue == "DUPLICATE");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `errors`, `error => error.Field ==
        // ”players” && error.Issue == ”MISSING_PARTICIPANT”` dalam Validate_RejectsDuplicatePhysicalAssignmentsAndMissingPlayer.
        Assert.Contains(errors, error => error.Field == "players" && error.Issue == "MISSING_PARTICIPANT");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `errors`, `error =>
        // error.Field.EndsWith(”tie_breaker_code”, StringComparison.Ordinal) && error.Issue == ”DUPLICATE”` dalam
        // Validate_RejectsDuplicatePhysicalAssignmentsAndMissingPlayer.
        Assert.Contains(errors, error => error.Field.EndsWith("tie_breaker_code", StringComparison.Ordinal) && error.Issue == "DUPLICATE");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `errors`, `error =>
        // error.Field.EndsWith(”mission_id”, StringComparison.Ordinal) && error.Issue == ”DUPLICATE”` dalam
        // Validate_RejectsDuplicatePhysicalAssignmentsAndMissingPlayer.
        Assert.Contains(errors, error => error.Field.EndsWith("mission_id", StringComparison.Ordinal) && error.Issue == "DUPLICATE");
    // Menutup scope metode Validate_RejectsDuplicatePhysicalAssignmentsAndMissingPlayer; bagian berikut berada di luar batas blok tersebut dalam
    // Validate_RejectsDuplicatePhysicalAssignmentsAndMissingPlayer.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Validate_RejectsAdvancedCardsInPemulaMode` dengan hasil bertipe `void`; operasi ini menangani validate rejects advanced
    // kartu in pemula mode.
    public void Validate_RejectsAdvancedCardsInPemulaMode()
    // Membuka scope metode Validate_RejectsAdvancedCardsInPemulaMode; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Validate_RejectsAdvancedCardsInPemulaMode.
    {
        // Menyiapkan variabel lokal `errors` untuk nilai kesalahan dengan memanggil `SessionSetupValidator.Validate` dengan `BuildRequest()`,
        // `BuildDefinition(advancedEnabled: false)`, `”PEMULA”`, `BuildPlayers()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var errors = SessionSetupValidator.Validate(
            // Meneruskan memanggil `BuildRequest` dengan tanpa argumen sebagai argumen ke `SessionSetupValidator.Validate`.
            BuildRequest(),
            // Meneruskan memanggil `BuildDefinition` dengan `false` sebagai argumen ke `SessionSetupValidator.Validate`; Meneruskan false, yaitu kondisi
            // nonaktif/tidak terpenuhi sebagai argumen bernama `advancedEnabled`.
            BuildDefinition(advancedEnabled: false),
            // Meneruskan nilai literal `”PEMULA”` sebagai argumen ke `SessionSetupValidator.Validate`.
            "PEMULA",
            // Meneruskan memanggil `BuildPlayers` dengan tanpa argumen sebagai argumen ke `SessionSetupValidator.Validate`.
            BuildPlayers());

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `errors`, `error =>
        // error.Field.EndsWith(”loan_code”, StringComparison.Ordinal) && error.Issue == ”DISALLOWED_FOR_MODE”` dalam
        // Validate_RejectsAdvancedCardsInPemulaMode.
        Assert.Contains(errors, error => error.Field.EndsWith("loan_code", StringComparison.Ordinal) && error.Issue == "DISALLOWED_FOR_MODE");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `errors`, `error =>
        // error.Field.EndsWith(”insurance_product_code”, StringComparison.Ordinal) && error.Issue == ”DISALLOWED_FOR_MODE”` dalam
        // Validate_RejectsAdvancedCardsInPemulaMode.
        Assert.Contains(errors, error => error.Field.EndsWith("insurance_product_code", StringComparison.Ordinal) && error.Issue == "DISALLOWED_FOR_MODE");
    // Menutup scope metode Validate_RejectsAdvancedCardsInPemulaMode; bagian berikut berada di luar batas blok tersebut dalam
    // Validate_RejectsAdvancedCardsInPemulaMode.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Validate_RequiresExactlyOneGoldCardAndKnownRulesetCards` dengan hasil bertipe `void`; operasi ini menangani validate
    // requires exactly one emas kartu dan known aturan kartu.
    public void Validate_RequiresExactlyOneGoldCardAndKnownRulesetCards()
    // Membuka scope metode Validate_RequiresExactlyOneGoldCardAndKnownRulesetCards; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Validate_RequiresExactlyOneGoldCardAndKnownRulesetCards.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan objek baru bertipe
        // `SessionSetupRequest` dengan argumen ( ”setup-invalid-cards”, [ BuildAssignment(FirstPlayerId, ”tie-1”, ”ingredient-unknown”, ”mission-1”,
        // goldQuantity: 2), BuildAssignment(SecondPlayerId, ”tie-2”,.... Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = new SessionSetupRequest(
            // Meneruskan nilai literal `”setup-invalid-cards”` sebagai argumen ke konstruktor `SessionSetupRequest`.
            "setup-invalid-cards",
            // Meneruskan koleksi berisi BuildAssignment(FirstPlayerId, ”tie-1”, ”ingredien..., BuildAssignment(SecondPlayerId, ”tie-2”, ”ingredie... sebagai
            // argumen ke konstruktor `SessionSetupRequest`.
            [
                // Meneruskan `FirstPlayerId` (nilai first pemain identitas) sebagai argumen ke `BuildAssignment`; Meneruskan nilai literal `”tie-1”` sebagai
                // argumen ke `BuildAssignment`; Meneruskan nilai literal `”ingredient-unknown”` sebagai argumen ke `BuildAssignment`; Meneruskan nilai literal
                // `”mission-1”` sebagai argumen ke `BuildAssignment`; Meneruskan nilai literal `2` sebagai argumen bernama `goldQuantity`.
                BuildAssignment(FirstPlayerId, "tie-1", "ingredient-unknown", "mission-1", goldQuantity: 2),
                // Meneruskan `SecondPlayerId` (nilai second pemain identitas) sebagai argumen ke `BuildAssignment`; Meneruskan nilai literal `”tie-2”` sebagai
                // argumen ke `BuildAssignment`; Meneruskan nilai literal `”ingredient-meat”` sebagai argumen ke `BuildAssignment`; Meneruskan nilai literal
                // `”mission-unknown”` sebagai argumen ke `BuildAssignment`.
                BuildAssignment(SecondPlayerId, "tie-2", "ingredient-meat", "mission-unknown")
            // Meneruskan koleksi berisi BuildAssignment(FirstPlayerId, ”tie-1”, ”ingredien..., BuildAssignment(SecondPlayerId, ”tie-2”, ”ingredie... sebagai
            // argumen ke konstruktor `SessionSetupRequest`.
            ]);

        // Menyiapkan variabel lokal `errors` untuk nilai kesalahan dengan memanggil `SessionSetupValidator.Validate` dengan `request`,
        // `BuildDefinition(advancedEnabled: true)`, `”MAHIR”`, `BuildPlayers()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var errors = SessionSetupValidator.Validate(
            // Meneruskan `request` (data masukan permintaan yang akan divalidasi atau diteruskan ke layanan) sebagai argumen ke
            // `SessionSetupValidator.Validate`.
            request,
            // Meneruskan memanggil `BuildDefinition` dengan `true` sebagai argumen ke `SessionSetupValidator.Validate`; Meneruskan true, yaitu kondisi
            // aktif/terpenuhi sebagai argumen bernama `advancedEnabled`.
            BuildDefinition(advancedEnabled: true),
            // Meneruskan nilai literal `”MAHIR”` sebagai argumen ke `SessionSetupValidator.Validate`.
            "MAHIR",
            // Meneruskan memanggil `BuildPlayers` dengan tanpa argumen sebagai argumen ke `SessionSetupValidator.Validate`.
            BuildPlayers());

        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `errors`, `error =>
        // error.Field.EndsWith(”ingredient_card_id”, StringComparison.Ordinal) && error.Issue == ”UNKNOWN_REFERENCE”` dalam
        // Validate_RequiresExactlyOneGoldCardAndKnownRulesetCards.
        Assert.Contains(errors, error => error.Field.EndsWith("ingredient_card_id", StringComparison.Ordinal) && error.Issue == "UNKNOWN_REFERENCE");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `errors`, `error =>
        // error.Field.EndsWith(”gold_quantity”, StringComparison.Ordinal) && error.Issue == ”MUST_EQUAL_ONE”` dalam
        // Validate_RequiresExactlyOneGoldCardAndKnownRulesetCards.
        Assert.Contains(errors, error => error.Field.EndsWith("gold_quantity", StringComparison.Ordinal) && error.Issue == "MUST_EQUAL_ONE");
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `errors`, `error =>
        // error.Field.EndsWith(”mission_id”, StringComparison.Ordinal) && error.Issue == ”UNKNOWN_REFERENCE”` dalam
        // Validate_RequiresExactlyOneGoldCardAndKnownRulesetCards.
        Assert.Contains(errors, error => error.Field.EndsWith("mission_id", StringComparison.Ordinal) && error.Issue == "UNKNOWN_REFERENCE");
    // Menutup scope metode Validate_RequiresExactlyOneGoldCardAndKnownRulesetCards; bagian berikut berada di luar batas blok tersebut dalam
    // Validate_RequiresExactlyOneGoldCardAndKnownRulesetCards.
    }

    // Mendefinisikan metode `BuildRequest` dengan hasil bertipe `SessionSetupRequest`; operasi ini menangani build permintaan. Nilai hasil langsung
    // berasal dari objek baru dengan tipe mengikuti konteks tujuan dan argumen ( ”setup-valid”, [ BuildAssignment(FirstPlayerId, ”tie-1”,
    // ”ingredient-meat”, ”mission-1”), BuildAssignment(SecondPlayerId, ”tie-2”, ”ingredient-flour”, ”mission-2”) ]).
    private static SessionSetupRequest BuildRequest() =>
        // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen ( ”setup-valid”, [ BuildAssignment(FirstPlayerId, ”tie-1”,
        // ”ingredient-meat”, ”mission-1”), BuildAssignment(SecondPlayerId, ”tie-2”, ”ingredient-flour”, ”mission-2”) ]) sebagai bagian ekspresi yang sedang
        // disusun dalam BuildRequest.
        new(
            // Meneruskan nilai literal `”setup-valid”` sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
            "setup-valid",
            // Meneruskan koleksi berisi BuildAssignment(FirstPlayerId, ”tie-1”, ”ingredien..., BuildAssignment(SecondPlayerId, ”tie-2”, ”ingredie... sebagai
            // argumen ke konstruktor dengan tipe mengikuti konteks.
            [
                // Meneruskan `FirstPlayerId` (nilai first pemain identitas) sebagai argumen ke `BuildAssignment`; Meneruskan nilai literal `”tie-1”` sebagai
                // argumen ke `BuildAssignment`; Meneruskan nilai literal `”ingredient-meat”` sebagai argumen ke `BuildAssignment`; Meneruskan nilai literal
                // `”mission-1”` sebagai argumen ke `BuildAssignment`.
                BuildAssignment(FirstPlayerId, "tie-1", "ingredient-meat", "mission-1"),
                // Meneruskan `SecondPlayerId` (nilai second pemain identitas) sebagai argumen ke `BuildAssignment`; Meneruskan nilai literal `”tie-2”` sebagai
                // argumen ke `BuildAssignment`; Meneruskan nilai literal `”ingredient-flour”` sebagai argumen ke `BuildAssignment`; Meneruskan nilai literal
                // `”mission-2”` sebagai argumen ke `BuildAssignment`.
                BuildAssignment(SecondPlayerId, "tie-2", "ingredient-flour", "mission-2")
            // Meneruskan koleksi berisi BuildAssignment(FirstPlayerId, ”tie-1”, ”ingredien..., BuildAssignment(SecondPlayerId, ”tie-2”, ”ingredie... sebagai
            // argumen ke konstruktor dengan tipe mengikuti konteks.
            ]);

    // Mendefinisikan metode `BuildAssignment` dengan hasil bertipe `SessionPlayerSetupRequest`; operasi ini menangani build assignment. Masukan:
    // Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas; Parameter `tieBreakerCode` bertipe `string` membawa kode kartu penentu urutan
    // saat nilai pemain sama; Parameter `ingredientCardId` bertipe `string` membawa identitas kartu bahan yang diberikan atau digunakan pemain;
    // Parameter `missionId` bertipe `string` membawa identitas misi koleksi yang ditugaskan; Parameter `goldQuantity` bertipe `int` membawa jumlah
    // kartu atau unit emas pemain; bila argumen tidak diberikan digunakan nilai literal `1`. Nilai hasil langsung berasal dari objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (playerId, tieBreakerCode, ingredientCardId, goldQuantity, missionId, ”loan-10”, ”multi-risk”).
    private static SessionPlayerSetupRequest BuildAssignment(
        // Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
        Guid playerId,
        // Parameter `tieBreakerCode` bertipe `string` membawa kode kartu penentu urutan saat nilai pemain sama.
        string tieBreakerCode,
        // Parameter `ingredientCardId` bertipe `string` membawa identitas kartu bahan yang diberikan atau digunakan pemain.
        string ingredientCardId,
        // Parameter `missionId` bertipe `string` membawa identitas misi koleksi yang ditugaskan.
        string missionId,
        // Parameter `goldQuantity` bertipe `int` membawa jumlah kartu atau unit emas pemain; bila argumen tidak diberikan digunakan nilai literal `1`.
        int goldQuantity = 1) =>
        // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen (playerId, tieBreakerCode, ingredientCardId, goldQuantity, missionId,
        // ”loan-10”, ”multi-risk”) sebagai bagian ekspresi yang sedang disusun dalam BuildAssignment.
        new(playerId, tieBreakerCode, ingredientCardId, goldQuantity, missionId, "loan-10", "multi-risk");

    // Mendefinisikan metode `BuildPlayers` dengan hasil bertipe `List<SessionPlayerDb>`; operasi ini menangani build pemain. Nilai hasil langsung
    // berasal dari koleksi berisi new() { SessionPlayerId = FirstPlayerId, UserId = ..., new() { SessionPlayerId = SecondPlayerId, UserId =....
    private static List<SessionPlayerDb> BuildPlayers() =>
    // Menggunakan koleksi berisi new() { SessionPlayerId = FirstPlayerId, UserId = ..., new() { SessionPlayerId = SecondPlayerId, UserId =... sebagai
    // bagian ekspresi yang sedang disusun dalam BuildPlayers.
    [
        // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam BuildPlayers.
        new() { SessionPlayerId = FirstPlayerId, UserId = Guid.NewGuid(), PlayerOrder = 1 },
        // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam BuildPlayers.
        new() { SessionPlayerId = SecondPlayerId, UserId = Guid.NewGuid(), PlayerOrder = 2 }
    // Menandai akhir daftar elemen atau indeks koleksi dalam BuildPlayers; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
    ];

    // Mendefinisikan metode `BuildDefinition` dengan hasil bertipe `RulesetDefinitionDto`; operasi ini menangani build definisi. Masukan: Parameter
    // `advancedEnabled` bertipe `bool` membawa nilai advanced enabled. Nilai hasil langsung berasal dari objek baru dengan tipe mengikuti konteks
    // tujuan dan argumen ().
    private static RulesetDefinitionDto BuildDefinition(bool advancedEnabled) =>
        // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam BuildDefinition.
        new()
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildDefinition.
        {
            // Memperbarui `Mode` menggunakan hasil pemilihan bersyarat: ketika `advancedEnabled` benar gunakan `”MAHIR”`, jika tidak gunakan `”PEMULA”` dalam
            // BuildDefinition.
            Mode = advancedEnabled ? "MAHIR" : "PEMULA",
            // Memperbarui `Settings` menggunakan objek baru bertipe `RulesetSettingsDto` dengan nilai awal sesuai konstruktornya dalam BuildDefinition.
            Settings = new RulesetSettingsDto
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildDefinition.
            {
                // Memperbarui `LoanEnabled` menggunakan `advancedEnabled` (nilai advanced enabled) dalam BuildDefinition.
                LoanEnabled = advancedEnabled,
                // Memperbarui `InsuranceEnabled` menggunakan `advancedEnabled` (nilai advanced enabled) dalam BuildDefinition.
                InsuranceEnabled = advancedEnabled
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildDefinition.
            },
            // Memperbarui `TieBreakers` menggunakan koleksi berisi new RulesetTieBreakerDto { TieBreakerCode = ”tie-1..., new RulesetTieBreakerDto {
            // TieBreakerCode = ”tie-2... dalam BuildDefinition.
            TieBreakers =
            // Menggunakan koleksi berisi new RulesetTieBreakerDto { TieBreakerCode = ”tie-1..., new RulesetTieBreakerDto { TieBreakerCode = ”tie-2... sebagai
            // bagian ekspresi yang sedang disusun dalam BuildDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetTieBreakerDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildDefinition.
                new RulesetTieBreakerDto { TieBreakerCode = "tie-1", TieNumber = 1, CardQty = 1 },
                // Menggunakan objek baru bertipe `RulesetTieBreakerDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildDefinition.
                new RulesetTieBreakerDto { TieBreakerCode = "tie-2", TieNumber = 2, CardQty = 1 }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `Ingredients` menggunakan koleksi berisi new RulesetIngredientDto { Id = ”ingredient-meat”,..., new RulesetIngredientDto { Id =
            // ”ingredient-flour”... dalam BuildDefinition.
            Ingredients =
            // Menggunakan koleksi berisi new RulesetIngredientDto { Id = ”ingredient-meat”,..., new RulesetIngredientDto { Id = ”ingredient-flour”... sebagai
            // bagian ekspresi yang sedang disusun dalam BuildDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetIngredientDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildDefinition.
                new RulesetIngredientDto { Id = "ingredient-meat", Nama = "Daging", HargaBeli = 5, CardQty = 2 },
                // Menggunakan objek baru bertipe `RulesetIngredientDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildDefinition.
                new RulesetIngredientDto { Id = "ingredient-flour", Nama = "Tepung", HargaBeli = 2, CardQty = 2 }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `CollectionMissions` menggunakan koleksi berisi new RulesetCollectionMissionDto { Id = ”mission-1”..., new
            // RulesetCollectionMissionDto { Id = ”mission-2”... dalam BuildDefinition.
            CollectionMissions =
            // Menggunakan koleksi berisi new RulesetCollectionMissionDto { Id = ”mission-1”..., new RulesetCollectionMissionDto { Id = ”mission-2”... sebagai
            // bagian ekspresi yang sedang disusun dalam BuildDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetCollectionMissionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam BuildDefinition.
                new RulesetCollectionMissionDto { Id = "mission-1", Nama = "Misi 1" },
                // Menggunakan objek baru bertipe `RulesetCollectionMissionDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam BuildDefinition.
                new RulesetCollectionMissionDto { Id = "mission-2", Nama = "Misi 2" }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `ShariaLoans` menggunakan koleksi berisi new RulesetShariaLoanDto { LoanCode = ”loan-10”, P... dalam BuildDefinition.
            ShariaLoans =
            // Menggunakan koleksi berisi new RulesetShariaLoanDto { LoanCode = ”loan-10”, P... sebagai bagian ekspresi yang sedang disusun dalam
            // BuildDefinition.
            [
                // Menggunakan objek baru bertipe `RulesetShariaLoanDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // BuildDefinition.
                new RulesetShariaLoanDto { LoanCode = "loan-10", Principal = 10, CardQty = 2 }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ],
            // Memperbarui `InsuranceProducts` menggunakan koleksi berisi new RulesetInsuranceProductDto { ProductCode = ”mu... dalam BuildDefinition.
            InsuranceProducts =
            // Menggunakan koleksi berisi new RulesetInsuranceProductDto { ProductCode = ”mu... sebagai bagian ekspresi yang sedang disusun dalam
            // BuildDefinition.
            [
                // Sesuai rulebook: asuransi berada di belakang kartu Tie Breaker,
                // bukan stok kartu asuransi terpisah.
                // Menggunakan objek baru bertipe `RulesetInsuranceProductDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam BuildDefinition.
                new RulesetInsuranceProductDto { ProductCode = "multi-risk", UsageLimit = 1, CardQty = 0 }
            // Menandai akhir daftar elemen atau indeks koleksi dalam BuildDefinition; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ]
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildDefinition.
        };
// Menutup scope tipe SessionSetupValidatorTests; bagian berikut berada di luar batas blok tersebut.
}
