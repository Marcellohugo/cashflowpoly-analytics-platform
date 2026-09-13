// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IIngredientInventoryCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

public interface IIngredientInventoryCalculator
{
    AnalyticsIngredientInventory BuildIngredientInventory(List<EventDb> events);
}
