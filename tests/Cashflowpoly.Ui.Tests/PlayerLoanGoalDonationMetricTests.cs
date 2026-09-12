// Fungsi file: Memastikan jumlah pinjaman, persentase pembelian target, dan persentase donasi memakai satuan serta arti yang sesuai.
using System.Globalization;
using Cashflowpoly.Ui.Infrastructure;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class PlayerLoanGoalDonationMetricTests
{
    [Theory]
    [InlineData("id")]
    [InlineData("en")]
    public void LoanCounts_AreCountsOfLoansAndExplainFullRepayment(string locale)
    {
        foreach (var key in new[] { "sharia_loans_taken", "sharia_loans_repaid", "sharia_loans_unpaid_end" })
        {
            var metric = Describe(key, "2", locale);
            Assert.Equal("2", metric.DisplayValue);
            Assert.Equal(locale == "id" ? "pinjaman" : "loans", metric.Unit);
            Assert.DoesNotContain("players.", metric.Explanation);
        }
        Assert.Contains(locale == "id" ? "seluruh pokoknya" : "entire principal", Describe("sharia_loans_repaid", "1", locale).Explanation);
        Assert.Equal("1 + 1 = 2 pinjaman", PlayerMetricCollectionHelper.BuildActualCalculation("debt-discipline",
            [("sharia_loans_taken", "2"), ("sharia_loans_repaid", "1"), ("sharia_loans_unpaid_end", "1")],
            "2", "pinjaman", "N/A", CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData("id", "0", "0%:")]
    [InlineData("id", "50", "Di atas 0% dan di bawah 100%")]
    [InlineData("id", "99.9", "Di atas 0% dan di bawah 100%")]
    [InlineData("id", "100", "100%:")]
    [InlineData("en", "50", "Above 0% and below 100%")]
    [InlineData("en", "100", "100%:")]
    public void GoalPercentage_ExplainsCompletionWithoutConfusingFundingProgress(string locale, string value, string guidance)
    {
        var metric = Describe("financial_goal_completion_percent", value, locale);
        Assert.Equal("%", metric.Unit);
        Assert.StartsWith(guidance, metric.Guidance);
        Assert.DoesNotContain("players.", metric.Label);
        Assert.Equal("1 ÷ 2 × 100% = 50%", PlayerMetricCollectionHelper.BuildActualCalculation("goal-ambition",
            [("financial_goals_completed", "1"), ("financial_goals_attempted", "2")],
            "50", "%", "N/A", CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData("0", "Di bawah 34%")]
    [InlineData("9.76", "Di bawah 34%")]
    [InlineData("33.99", "Di bawah 34%")]
    [InlineData("34", "34% hingga kurang dari 67%")]
    [InlineData("66.99", "34% hingga kurang dari 67%")]
    [InlineData("67", "67% atau lebih")]
    [InlineData("100", "67% atau lebih")]
    public void DonationPercentage_KeepsItsScaleAndThresholds(string value, string guidance)
    {
        var metric = Describe("donation_commitment_score", value, "id");
        Assert.Equal("%", metric.Unit);
        Assert.Equal(double.Parse(value, CultureInfo.InvariantCulture), double.Parse(metric.DisplayValue, CultureInfo.CurrentCulture));
        Assert.StartsWith(guidance, metric.Guidance);
        Assert.Equal("Persentase Komitmen Donasi", metric.Label);
        Assert.Contains("tidak menunjukkan persentase pendapatan", metric.Explanation);
    }

    [Theory]
    [InlineData("financial_goal_completion_percent")]
    [InlineData("donation_commitment_score")]
    public void MissingPercentage_RemainsUnavailableInsteadOfZero(string key)
    {
        var metric = Describe(key, "N/A", "id");
        Assert.Equal("unavailable", metric.State);
        Assert.Equal(string.Empty, metric.Unit);
        Assert.NotEqual("0", metric.DisplayValue);
    }

    private static PlayerMetricPresentation Describe(string key, string value, string locale) =>
        PlayerMetricLabelFormatter.DescribeMetric(key, value, true, true, "N/A", k => UiText.Translate(locale, k));
}
