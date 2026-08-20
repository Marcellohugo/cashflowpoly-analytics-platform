// Fungsi file: Memverifikasi pembagian deck fisik dan pasar awal sesi.
using Cashflowpoly.Api.Contracts;
using Cashflowpoly.Api.Data;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class SessionStateSetupTests
{
    [Fact]
    public void DrawIngredientMarket_LimitsEveryIngredientToTwoCards()
    {
        var deck = new[]
        {
            Card("nasi"), Card("nasi"), Card("nasi"),
            Card("telur"), Card("telur"),
            Card("sayur"), Card("sayur")
        };

        var market = SessionStateRepository.DrawIngredientMarket(deck, dealtCards: 0, marketSize: 5);

        Assert.Equal(5, market.Count);
        Assert.All(market.GroupBy(item => item.Id), group => Assert.InRange(group.Count(), 1, 2));
    }

    private static RulesetIngredientDto Card(string id) => new()
    {
        Id = id,
        Nama = id,
        HargaBeli = 1,
        CardQty = 5
    };
}
