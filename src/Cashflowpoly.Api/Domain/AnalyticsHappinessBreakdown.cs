namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Breakdown poin happiness pemain: kebutuhan, bonus set, donasi, emas, pensiun, tabungan, dan penalti.
/// </summary>
public sealed record AnalyticsHappinessBreakdown(
    double Total,
    double NeedPoints,
    double NeedSetBonusPoints,
    double DonationPoints,
    double GoldPoints,
    double PensionPoints,
    double SavingGoalPointsEffective,
    double MissionPenaltyPoints,
    double LoanPenaltyPoints,
    bool HasUnpaidLoan);
