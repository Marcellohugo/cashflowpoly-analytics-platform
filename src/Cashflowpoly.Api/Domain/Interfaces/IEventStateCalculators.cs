// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IEventStateCalculators.
// Mengimpor namespace `System.Diagnostics.CodeAnalysis` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Diagnostics.CodeAnalysis;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public interface IEventDerivedStateCalculator
{
    EventIngredientInventory BuildIngredientInventory(IEnumerable<EventDb> events, Guid playerId);

    int ComputeSavingBalance(IEnumerable<EventDb> events, Guid playerId, int initialSaving = 0);
}

public interface IEventPlayerBalanceCalculator
{
    double Compute(
        // Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
        Guid playerId,
        // Parameter `startingCash` bertipe `int` membawa nilai starting uang tunai.
        int startingCash,
        // Parameter `projections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event
        // permainan.
        IReadOnlyCollection<CashflowProjectionDb> projections);
}
