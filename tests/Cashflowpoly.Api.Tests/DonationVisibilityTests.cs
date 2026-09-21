// Fungsi file: Menguji bahwa proyeksi yang lebih baru tidak membocorkan donasi sebelum semua peserta menyetor.
using Cashflowpoly.Api.Data;
using Cashflowpoly.Api.Domain;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class DonationVisibilityTests
{
    [Fact]
    public void RemoveSealed_ExcludesAmountsIncludingProjectionsAheadOfEventRead()
    {
        var known = Guid.NewGuid();
        var hidden = Guid.NewGuid();
        var events = new List<EventDb>
        {
            new() { EventId = known, ActionType = "KerjaLepas", UserId = Guid.NewGuid() },
            new() { EventId = hidden, ActionType = "JumatBerkah", UserId = Guid.NewGuid(), DayIndex = 5 }
        };
        var projections = new List<CashflowProjectionDb>
        {
            new() { EventId = known, Amount = 1 },
            new() { EventId = hidden, Amount = 7 },
            new() { EventId = Guid.NewGuid(), Amount = 9 }
        };
        Assert.True(DonationVisibility.RemoveSealed(events, projections, 3));
        Assert.Equal(known, Assert.Single(events).EventId);
        Assert.Equal(1, Assert.Single(projections).Amount);
    }
}
