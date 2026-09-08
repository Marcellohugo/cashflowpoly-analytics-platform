// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IEventStateCalculators.
// Mengimpor namespace `System.Diagnostics.CodeAnalysis` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Diagnostics.CodeAnalysis;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan interface sebagai kontrak operasi `IEventDerivedStateCalculator`.
public interface IEventDerivedStateCalculator
// Membuka scope tipe IEventDerivedStateCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `BuildIngredientInventory` dengan hasil bertipe `EventIngredientInventory`; operasi ini menangani build bahan inventory.
    // Masukan: Parameter `events` bertipe `IEnumerable<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau
    // perhitungan; Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
    EventIngredientInventory BuildIngredientInventory(IEnumerable<EventDb> events, Guid playerId);

    // Mendefinisikan metode `ComputeSavingBalance` dengan hasil bertipe `int`; operasi ini menangani compute tabungan saldo. Masukan: Parameter
    // `events` bertipe `IEnumerable<EventDb>` membawa kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan; Parameter
    // `playerId` bertipe `Guid` membawa nilai pemain identitas; Parameter `goalId` bertipe `string` membawa nilai target identitas.
    int ComputeSavingBalance(IEnumerable<EventDb> events, Guid playerId, string goalId);
// Menutup scope tipe IEventDerivedStateCalculator; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `IEventPlayerBalanceCalculator`.
public interface IEventPlayerBalanceCalculator
// Membuka scope tipe IEventPlayerBalanceCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Compute` dengan hasil bertipe `double`; operasi ini menangani compute. Masukan: Parameter `playerId` bertipe `Guid`
    // membawa nilai pemain identitas; Parameter `startingCash` bertipe `int` membawa nilai starting uang tunai; Parameter `projections` bertipe
    // `IReadOnlyCollection<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan.
    double Compute(
        // Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
        Guid playerId,
        // Parameter `startingCash` bertipe `int` membawa nilai starting uang tunai.
        int startingCash,
        // Parameter `projections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event
        // permainan.
        IReadOnlyCollection<CashflowProjectionDb> projections);
// Menutup scope tipe IEventPlayerBalanceCalculator; bagian berikut berada di luar batas blok tersebut.
}

