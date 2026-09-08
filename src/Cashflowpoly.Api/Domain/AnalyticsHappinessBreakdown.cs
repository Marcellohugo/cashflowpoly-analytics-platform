// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsHappinessBreakdown.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Breakdown poin happiness pemain: kebutuhan, bonus set, donasi, emas, pensiun, tabungan, dan penalti.
/// </summary>
// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsHappinessBreakdown`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed record AnalyticsHappinessBreakdown(
    // Parameter `Total` bertipe `double` membawa nilai total.
    double Total,
    // Parameter `NeedPoints` bertipe `double` membawa nilai kebutuhan poin.
    double NeedPoints,
    // Parameter `NeedSetBonusPoints` bertipe `double` membawa nilai kebutuhan set bonus poin.
    double NeedSetBonusPoints,
    // Parameter `DonationPoints` bertipe `double` membawa nilai donasi poin.
    double DonationPoints,
    // Parameter `GoldPoints` bertipe `double` membawa nilai emas poin.
    double GoldPoints,
    // Parameter `PensionPoints` bertipe `double` membawa nilai pension poin.
    double PensionPoints,
    // Parameter `SavingGoalPointsEffective` bertipe `double` membawa nilai tabungan target poin effective.
    double SavingGoalPointsEffective,
    // Parameter `MissionPenaltyPoints` bertipe `double` membawa nilai misi penalti poin.
    double MissionPenaltyPoints,
    // Parameter `LoanPenaltyPoints` bertipe `double` membawa nilai pinjaman penalti poin.
    double LoanPenaltyPoints,
    // Parameter `HasUnpaidLoan` bertipe `bool` membawa nilai memiliki unpaid pinjaman.
    bool HasUnpaidLoan);
