// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventPlayerBalanceCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

internal sealed class EventPlayerBalanceCalculator : IEventPlayerBalanceCalculator
{
    public double Compute(
        // Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
        Guid playerId,
        // Parameter `startingCash` bertipe `int` membawa nilai starting uang tunai.
        int startingCash,
        // Parameter `projections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event
        // permainan.
        IReadOnlyCollection<CashflowProjectionDb> projections)
    {
        var net = projections
            .Where(p => p.UserId == playerId)
            .Sum(p => p.Direction == "IN" ? p.Amount : -p.Amount);

        return startingCash + net;
    }
}
