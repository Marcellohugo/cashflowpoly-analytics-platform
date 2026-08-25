// Fungsi file: Memverifikasi cursor pagination dapat dibaca ulang dan menolak nilai yang rusak.
using Cashflowpoly.Api.Infrastructure;
using Xunit;

namespace Cashflowpoly.Api.Tests;

public sealed class OpaqueCursorTests
{
    [Fact]
    public void EventCursor_RoundTripsSequenceNumber()
    {
        var cursor = OpaqueCursor.EncodeEvent(42);

        Assert.True(OpaqueCursor.TryDecodeEvent(cursor, out var sequenceNumber));
        Assert.Equal(42, sequenceNumber);
        Assert.DoesNotContain("42", cursor, StringComparison.Ordinal);
    }

    [Fact]
    public void TransactionCursor_RoundTripsStableOrderingFields()
    {
        var timestamp = new DateTimeOffset(2026, 8, 26, 10, 20, 30, TimeSpan.Zero);
        var transactionId = Guid.NewGuid();

        var cursor = OpaqueCursor.EncodeTransaction(timestamp, transactionId);

        Assert.True(OpaqueCursor.TryDecodeTransaction(cursor, out var decodedTimestamp, out var decodedId));
        Assert.Equal(timestamp, decodedTimestamp);
        Assert.Equal(transactionId, decodedId);
    }

    [Theory]
    [InlineData("%%")]
    [InlineData("not-a-valid-cursor")]
    [InlineData("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
    public void InvalidCursor_IsRejected(string cursor)
    {
        Assert.False(OpaqueCursor.TryDecodeEvent(cursor, out _));
        Assert.False(OpaqueCursor.TryDecodeTransaction(cursor, out _, out _));
    }
}
