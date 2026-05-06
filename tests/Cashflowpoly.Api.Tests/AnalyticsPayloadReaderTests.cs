// Fungsi file: Menguji parser payload dan helper matematika analitik yang dipakai komputasi metrik.
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

/// <summary>
/// Kelas pengujian unit untuk helper analitik murni yang sebelumnya berada di AnalyticsController.
/// </summary>
public sealed class AnalyticsPayloadReaderTests
{
    [Fact]
    /// <summary>
    /// Memvalidasi parser turn.action.used membaca used dan remaining dari payload valid.
    /// </summary>
    public void TryReadActionUsed_ReturnsUsedAndRemaining_WhenPayloadIsValid()
    {
        var ok = AnalyticsPayloadReader.TryReadActionUsed("""{"used":2,"remaining":1}""", out var used, out var remaining);

        Assert.True(ok);
        Assert.Equal(2, used);
        Assert.Equal(1, remaining);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi parser kebutuhan mengizinkan card_id dan points opsional.
    /// </summary>
    public void TryReadNeedPurchase_ReturnsDefaults_WhenOptionalFieldsAreMissing()
    {
        var ok = AnalyticsPayloadReader.TryReadNeedPurchase("""{"amount":4}""", out var amount, out var cardId, out var points);

        Assert.True(ok);
        Assert.Equal(4, amount);
        Assert.Equal(string.Empty, cardId);
        Assert.Equal(0, points);
    }

    [Fact]
    /// <summary>
    /// Memvalidasi parser order menolak daftar kartu bahan kosong setelah normalisasi.
    /// </summary>
    public void TryReadOrderClaim_ReturnsFalse_WhenRequiredCardsAreBlank()
    {
        var ok = AnalyticsPayloadReader.TryReadOrderClaim(
            """{"required_ingredient_card_ids":[""," "],"income":10}""",
            out var requiredCards,
            out var income);

        Assert.False(ok);
        Assert.Empty(requiredCards);
        Assert.Equal(10, income);
    }

    [Theory]
    [InlineData("ingredient.purchased", true)]
    [InlineData("turn.action.used", false)]
    [InlineData("rank.awarded", false)]
    [InlineData("mission.assigned", false)]
    /// <summary>
    /// Memvalidasi klasifikasi event gameplay substantif.
    /// </summary>
    public void IsActionEvent_ClassifiesMetaEvents(string actionType, bool expected)
    {
        Assert.Equal(expected, AnalyticsPayloadReader.IsActionEvent(actionType));
    }

    [Fact]
    /// <summary>
    /// Memvalidasi SafeRatio mengembalikan null untuk denominator nol dan persen saat diminta.
    /// </summary>
    public void SafeRatio_HandlesZeroAndPercent()
    {
        Assert.Null(AnalyticsMath.SafeRatio(10, 0));
        Assert.Equal(25, AnalyticsMath.SafeRatio(1, 4, percent: true));
    }

    [Fact]
    /// <summary>
    /// Memvalidasi standar deviasi populasi untuk kumpulan nilai.
    /// </summary>
    public void StdDev_ComputesPopulationStandardDeviation()
    {
        var value = AnalyticsMath.StdDev(new[] { 2.0, 4.0, 4.0, 4.0, 5.0, 5.0, 7.0, 9.0 });

        Assert.Equal(2, value);
    }
}
