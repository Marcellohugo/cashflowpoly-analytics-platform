// Fungsi file: Menyembunyikan donasi yang belum dibuka dari seluruh perhitungan analitika publik.
using Cashflowpoly.Api.Data;

namespace Cashflowpoly.Api.Domain;

internal static class DonationVisibility
{
    // Public analytics must not reveal a sealed amount through cash totals, rankings, or raw metrics.
    internal static bool RemoveSealed(List<EventDb> events, List<CashflowProjectionDb> projections, int participantCount)
    {
        var sealedDays = events.Where(e => e.ActionType == GameActionCatalog.JumatBerkah && e.UserId.HasValue)
            .GroupBy(e => e.DayIndex)
            .Where(g => g.Select(e => e.UserId).Distinct().Count() < participantCount)
            .Select(g => g.Key).ToHashSet();
        var hiddenIds = events.Where(e => e.ActionType == GameActionCatalog.JumatBerkah && sealedDays.Contains(e.DayIndex))
            .Select(e => e.EventId).ToHashSet();
        events.RemoveAll(e => hiddenIds.Contains(e.EventId));
        var visibleIds = events.Select(e => e.EventId).ToHashSet();
        projections.RemoveAll(p => !visibleIds.Contains(p.EventId));
        return hiddenIds.Count > 0;
    }
}
