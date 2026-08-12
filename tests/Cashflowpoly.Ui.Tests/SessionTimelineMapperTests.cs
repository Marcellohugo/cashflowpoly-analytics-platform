// Fungsi file: Memverifikasi perilaku, lokalisasi, atau tata letak UI melalui SessionTimelineMapperTests.
using System.Text.Json;
using Cashflowpoly.Ui.Contracts;
using Cashflowpoly.Ui.Infrastructure;
using Xunit;

namespace Cashflowpoly.Ui.Tests;

public sealed class SessionTimelineMapperTests
{
    [Fact]
    public void MapTimeline_ShouldLocalizeWeekdayNamesForActiveLanguage()
    {
        var events = new List<EventRequest>
        {
            CreateEvent("KerjaLepas", """{"amount":1}""", "MON"),
            CreateEvent("BahanMasakan", """{"card_id":"sayur","amount":2}""", "TUE", sequenceNumber: 2)
        };

        var indonesianTimeline = SessionTimelineMapper.MapTimeline(events, "id");
        var englishTimeline = SessionTimelineMapper.MapTimeline(events, "en");

        Assert.Equal(new[] { "Senin", "Selasa" }, indonesianTimeline.Select(item => item.Weekday));
        Assert.Equal(new[] { "Monday", "Tuesday" }, englishTimeline.Select(item => item.Weekday));
    }

    [Fact]
    public void MapTimeline_ShouldPlaceInitialSetupOnGoWithoutMovingGameplayDayOne()
    {
        var events = new List<EventRequest>
        {
            CreateEvent("MulaiSesi", """{"start_note":"Mulai sesi"}""", "MON", actionSlot: 0, actorType: "SYSTEM", dayIndex: 1),
            CreateEvent("SetupBahanAwal", """{"card_id":"sayur","setup":"INITIAL"}""", "MON", sequenceNumber: 2, actionSlot: 0, actorType: "SYSTEM", dayIndex: 1),
            CreateEvent("KerjaLepas", """{"amount":1}""", "MON", sequenceNumber: 3, dayIndex: 1)
        };

        var timeline = SessionTimelineMapper.MapTimeline(events, "id");

        Assert.Equal(new[] { 0, 0, 1 }, timeline.Select(item => item.DayIndex));
        Assert.Equal(new[] { 0, 0, 1 }, timeline.Select(item => item.ActionSlot));
    }

    [Fact]
    public void MapTimeline_ShouldClassifyInsuranceEventsAsFinancing()
    {
        var events = new List<EventRequest>
        {
            CreateEvent("Asuransi", """{"policy_id":"INS-SEED-001","premium":1,"coverage_type":"MULTIRISK"}""", "MON")
        };

        var indonesianItem = Assert.Single(SessionTimelineMapper.MapTimeline(events, "id"));
        var englishItem = Assert.Single(SessionTimelineMapper.MapTimeline(events, "en"));

        Assert.Equal("Pembiayaan", indonesianItem.FlowLabel);
        Assert.Equal("Membeli asuransi multirisk dengan premi 1.", indonesianItem.FlowDescription);
        Assert.Equal("action", indonesianItem.ActionSlotRole);
        Assert.Equal("Aksi 1", indonesianItem.ActionSlotLabel);
        Assert.Equal("Financing", englishItem.FlowLabel);
        Assert.Equal("Purchased multirisk insurance with premium 1.", englishItem.FlowDescription);
        Assert.Equal("action", englishItem.ActionSlotRole);
        Assert.Equal("Action 1", englishItem.ActionSlotLabel);
    }

    [Fact]
    public void MapTimeline_ShouldClassifyRiskLifeAsActionEffect()
    {
        var events = new List<EventRequest>
        {
            CreateEvent("RisikoKehidupan", """{"risk_id":"risk_bonus_tunjangan"}""", "TUE", actionSlot: 2)
        };

        var indonesianItem = Assert.Single(SessionTimelineMapper.MapTimeline(events, "id"));
        var englishItem = Assert.Single(SessionTimelineMapper.MapTimeline(events, "en"));

        Assert.Equal("effect", indonesianItem.ActionSlotRole);
        Assert.Equal("Efek Aksi 2", indonesianItem.ActionSlotLabel);
        Assert.Equal("effect", englishItem.ActionSlotRole);
        Assert.Equal("Action Effect 2", englishItem.ActionSlotLabel);
    }

    [Fact]
    public void MapTimeline_ShouldClassifyInsuranceUseAsActionResponse()
    {
        var events = new List<EventRequest>
        {
            CreateEvent("Asuransi", """{"risk_event_id":"risk-event-001"}""", "TUE", actionSlot: 2)
        };

        var indonesianItem = Assert.Single(SessionTimelineMapper.MapTimeline(events, "id"));
        var englishItem = Assert.Single(SessionTimelineMapper.MapTimeline(events, "en"));

        Assert.Equal("response", indonesianItem.ActionSlotRole);
        Assert.Equal("Respons Aksi 2", indonesianItem.ActionSlotLabel);
        Assert.Equal("response", englishItem.ActionSlotRole);
        Assert.Equal("Action Response 2", englishItem.ActionSlotLabel);
    }

    [Fact]
    public void MapTimeline_ShouldClassifyEmergencyOptionAsActionResponse()
    {
        var events = new List<EventRequest>
        {
            CreateEvent("GunakanOpsiDarurat", """{"option_type":"loan","direction":"IN","amount":5}""", "TUE", actionSlot: 2)
        };

        var item = Assert.Single(SessionTimelineMapper.MapTimeline(events, "id"));

        Assert.Equal("response", item.ActionSlotRole);
        Assert.Equal("Respons Aksi 2", item.ActionSlotLabel);
    }

    [Fact]
    public void MapTimeline_ShouldDescribeDonationWinnerAnnouncementAsSystemSummary()
    {
        var events = new List<EventRequest>
        {
            CreateEvent(
                "UmumkanJuaraDonasi",
                """{"summary":"Manalu Juara 1, Marcello Juara 2, Marco Juara 3","winners":[{"rank":1,"player_name":"Manalu","points":7},{"rank":2,"player_name":"Marcello","points":5},{"rank":3,"player_name":"Marco","points":2}]}""",
                "FRI",
                actorType: "SYSTEM",
                userId: null)
        };

        var item = Assert.Single(SessionTimelineMapper.MapTimeline(events, "id"));

        Assert.Equal("Peduli Donasi", item.FlowLabel);
        Assert.Equal("Sistem menentukan Juara Donasi: Manalu Juara 1, Marcello Juara 2, Marco Juara 3.", item.FlowDescription);
        Assert.Equal("SYSTEM", item.ActorType);
        Assert.Null(item.PlayerId);
    }

    private static EventRequest CreateEvent(
        string actionType,
        string payloadJson,
        string weekday,
        long sequenceNumber = 1,
        int actionSlot = 1,
        string actorType = "PLAYER",
        Guid? userId = null,
        int dayIndex = 0)
    {
        using var document = JsonDocument.Parse(payloadJson);
        return new EventRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            userId,
            actorType,
            DateTimeOffset.Parse("2026-02-02T01:00:00Z"),
            dayIndex,
            weekday,
            actionSlot,
            sequenceNumber,
            actionType,
            Guid.NewGuid(),
            document.RootElement.Clone(),
            null);
    }
}
