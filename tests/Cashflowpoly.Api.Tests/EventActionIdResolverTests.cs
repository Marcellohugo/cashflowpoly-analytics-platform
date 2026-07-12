using System.Text.Json;
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
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
    [InlineData("BayarRisiko")]
    [InlineData("GunakanOpsiDarurat")]
    [InlineData("PoinPeringkatDonasi")]
    [InlineData("UmumkanJuaraDonasi")]
    [InlineData("PoinEmas")]
    [InlineData("PoinPeringkatPensiun")]
    [InlineData("BagikanTieBreaker")]
    [InlineData("AmbilKartuDariDeck")]
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
    [InlineData("BagikanEmasAwal")]
    [InlineData("BagikanMisiKoleksi")]
    [InlineData("KartuDiambilDariPasar")]
    public void Resolve_ReturnsNull_ForRemovedTechnicalActionTypes(string actionType)
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

    [Theory]
    [InlineData("BahanMasakan")]
    [InlineData("JualMasakan")]
    [InlineData("Kebutuhan")]
    [InlineData("KerjaLepas")]
    [InlineData("Menabung")]
    [InlineData("BayarPinjaman")]
    public void SlotPolicy_ConsumesRegularPlayerActions(string actionType)
    {
        Assert.Equal(PlayerActionSlotPolicy.Consumes, GameActionCatalog.GetPlayerActionSlotPolicy(actionType, Parse("{}")));
    }

    [Theory]
    [InlineData("JumatBerkah")]
    [InlineData("RisikoKehidupan")]
    [InlineData("BayarRisiko")]
    [InlineData("GunakanOpsiDarurat")]
    [InlineData("InvestasiEmas")]
    [InlineData("JualEmas")]
    [InlineData("LewatiTransaksiEmas")]
    public void SlotPolicy_DoesNotConsumeRulebookFreeActions(string actionType)
    {
        Assert.Equal(PlayerActionSlotPolicy.Free, GameActionCatalog.GetPlayerActionSlotPolicy(actionType, Parse("{}")));
    }

    [Theory]
    [InlineData("Asuransi")]
    [InlineData("PinjamanSyariah")]
    public void SlotPolicy_DoesNotConsumeRiskResponses(string actionType)
    {
        Assert.Equal(
            PlayerActionSlotPolicy.Free,
            GameActionCatalog.GetPlayerActionSlotPolicy(actionType, Parse("{\"risk_event_id\":\"95000000-0000-0000-0000-000000000123\"}")));
        Assert.Equal(
            PlayerActionSlotPolicy.Consumes,
            GameActionCatalog.GetPlayerActionSlotPolicy(actionType, Parse("{}")));
        Assert.Equal(
            PlayerActionSlotPolicy.Consumes,
            GameActionCatalog.GetPlayerActionSlotPolicy(actionType, Parse("{\"risk_event_id\":\"fake\"}")));
    }

    private static JsonElement Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}
