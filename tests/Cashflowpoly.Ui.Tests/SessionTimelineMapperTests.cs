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
            CreateEvent("work.freelance.completed", """{"amount":1}""", "MON"),
            CreateEvent("ingredient.purchased", """{"card_id":"sayur","amount":2}""", "TUE", sequenceNumber: 2)
        };

        var indonesianTimeline = SessionTimelineMapper.MapTimeline(events, "id");
        var englishTimeline = SessionTimelineMapper.MapTimeline(events, "en");

        Assert.Equal(new[] { "Senin", "Selasa" }, indonesianTimeline.Select(item => item.Weekday));
        Assert.Equal(new[] { "Monday", "Tuesday" }, englishTimeline.Select(item => item.Weekday));
    }

    [Fact]
    public void MapTimeline_ShouldClassifyInsuranceEventsAsFinancing()
    {
        var events = new List<EventRequest>
        {
            CreateEvent("insurance.multirisk.purchased", """{"policy_id":"INS-SEED-001","premium":1,"coverage_type":"MULTIRISK"}""", "MON")
        };

        var indonesianItem = Assert.Single(SessionTimelineMapper.MapTimeline(events, "id"));
        var englishItem = Assert.Single(SessionTimelineMapper.MapTimeline(events, "en"));

        Assert.Equal("Pembiayaan", indonesianItem.FlowLabel);
        Assert.Equal("Membeli asuransi multirisk dengan premi 1.", indonesianItem.FlowDescription);
        Assert.Equal("Financing", englishItem.FlowLabel);
        Assert.Equal("Purchased multirisk insurance with premium 1.", englishItem.FlowDescription);
    }

    private static EventRequest CreateEvent(
        string actionType,
        string payloadJson,
        string weekday,
        long sequenceNumber = 1)
    {
        using var document = JsonDocument.Parse(payloadJson);
        return new EventRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "PLAYER",
            DateTimeOffset.Parse("2026-02-02T01:00:00Z"),
            0,
            weekday,
            1,
            sequenceNumber,
            actionType,
            Guid.NewGuid(),
            document.RootElement.Clone(),
            null);
    }
}
