using System.Text.Json;
using Cashflowpoly.Api.Data;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class EventActionIdResolverTests
{
    [Theory]
    [InlineData("BahanMasakan")]
    [InlineData("BuangBahanMasakan")]
    [InlineData("JualMasakan")]
    [InlineData("LewatiOrder")]
    [InlineData("Kebutuhan")]
    [InlineData("KerjaLepas")]
    [InlineData("CatatTransaksi")]
    [InlineData("Menabung")]
    [InlineData("TarikTabungan")]
    [InlineData("TujuanFinansial")]
    [InlineData("JumatBerkah")]
    [InlineData("InvestasiEmas")]
    [InlineData("JualEmas")]
    [InlineData("LewatiTransaksiEmas")]
    [InlineData("HariMingguLibur")]
    [InlineData("PinjamanSyariah")]
    [InlineData("BayarPinjaman")]
    [InlineData("Asuransi")]
    [InlineData("RisikoKehidupan")]
    [InlineData("GunakanOpsiDarurat")]
    [InlineData("PoinPeringkatDonasi")]
    [InlineData("UmumkanJuaraDonasi")]
    [InlineData("PoinEmas")]
    [InlineData("PoinPeringkatPensiun")]
    [InlineData("BagikanEmasAwal")]
    [InlineData("BagikanTieBreaker")]
    [InlineData("BagikanMisiKoleksi")]
    [InlineData("AmbilKartuDariDeck")]
    [InlineData("KartuDiambilDariPasar")]
    [InlineData("KartuMasukDiscard")]
    [InlineData("IsiUlangPasar")]
    [InlineData("MulaiSesi")]
    [InlineData("AkhiriSesi")]
    [InlineData("AkhirGiliran")]
    public void Resolve_ReturnsPascalCaseGameActionIdUnchanged(string actionType)
    {
        var actionId = EventActionIdResolver.Resolve(actionType, Parse("""{}"""));

        Assert.Equal(actionType, actionId);
    }

    [Theory]
    [InlineData("ingredient.purchased")]
    [InlineData("ingredient.discarded")]
    [InlineData("order.claimed")]
    [InlineData("order.passed")]
    [InlineData("work.freelance.completed")]
    [InlineData("transaction.recorded")]
    [InlineData("mission.assigned")]
    [InlineData("insurance.multirisk.used")]
    [InlineData("saving.deposit.withdrawn")]
    [InlineData("risk.emergency.used")]
    [InlineData("donation.rank.awarded")]
    [InlineData("donation.winners.announced")]
    [InlineData("gold.points.awarded")]
    [InlineData("pension.rank.awarded")]
    [InlineData("day.saturday.gold_trade")]
    [InlineData("session.started")]
    [InlineData("turn.ended")]
    public void Resolve_ReturnsNull_ForLegacyTechnicalActionTypes(string actionType)
    {
        var resolved = EventActionIdResolver.Resolve(actionType, Parse("""{"trade_type":"SELL"}"""));

        Assert.Null(resolved);
    }

    [Fact]
    public void Resolve_ReturnsNull_ForUnmappedActionTypes()
    {
        var actionId = EventActionIdResolver.Resolve("unknown.action.type", Parse("""{"amount":5}"""));

        Assert.Null(actionId);
    }

    private static JsonElement Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}
