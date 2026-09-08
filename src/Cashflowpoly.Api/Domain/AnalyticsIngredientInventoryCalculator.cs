// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsIngredientInventoryCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Kalkulator inventaris bahan analitik dari event pembelian, klaim pesanan, dan discard.
/// </summary>
// Mendefinisikan tipe class `IngredientInventoryCalculator` yang mewarisi atau menerapkan `IIngredientInventoryCalculator`; sealed mencegah tipe
// ini diturunkan lagi.
internal sealed class IngredientInventoryCalculator : IIngredientInventoryCalculator
// Membuka scope tipe IngredientInventoryCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `AnalyticsPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly AnalyticsPayloadReader _payloadReader = new();
    /// <summary>
    /// Membangun inventaris bahan berdasarkan event pembelian, klaim pesanan, dan discard.
    /// </summary>
    // Mendefinisikan metode `BuildIngredientInventory` dengan hasil bertipe `AnalyticsIngredientInventory`. Membangun inventaris bahan berdasarkan
    // event pembelian, klaim pesanan, dan discard. Masukan: Parameter `events` bertipe `List<EventDb>` membawa kumpulan event permainan sebagai sumber
    // riwayat untuk validasi atau perhitungan.
    public AnalyticsIngredientInventory BuildIngredientInventory(List<EventDb> events)
    // Membuka scope metode BuildIngredientInventory; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildIngredientInventory.
    {
        // Menyiapkan variabel lokal `inventory` untuk nilai inventory dengan objek baru bertipe `AnalyticsIngredientInventory` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var inventory = new AnalyticsIngredientInventory();

        // Mengulangi setiap elemen `events`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh badan loop dalam
        // BuildIngredientInventory.
        foreach (var evt in events)
        // Membuka scope loop setiap evt dari `events`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildIngredientInventory.
        {
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `(string.Equals(evt.ActionType, ”BahanMasakan”,
            // StringComparison.OrdinalIgnoreCase) || string.Equals(evt.ActionType, ”SetupBahanAwal”, StringComparison.OrdinalIgnoreCase))` dan
            // `_payloadReader.TryReadIngredientPurchase(evt.Payload, out var cardId, out var amount)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if
            // hanya dijalankan ketika kondisi ini bernilai benar dalam BuildIngredientInventory.
            if ((string.Equals(evt.ActionType, "BahanMasakan", StringComparison.OrdinalIgnoreCase) ||
                 // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `evt.ActionType`, `”SetupBahanAwal”`, `StringComparison.OrdinalIgnoreCase`;
                 // aturan perbandingan mengikuti overload dan comparer yang diberikan dalam BuildIngredientInventory.
                 string.Equals(evt.ActionType, "SetupBahanAwal", StringComparison.OrdinalIgnoreCase)) &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadIngredientPurchase` dengan `evt.Payload`, `var cardId`, `var amount` dalam
                // BuildIngredientInventory.
                _payloadReader.TryReadIngredientPurchase(evt.Payload, out var cardId, out var amount))
            // Membuka scope cabang if untuk kondisi `(string.Equals(evt.ActionType, ”BahanMasakan”, StringComparison.OrdinalIgnoreCase) ||
            // string.Equals(evt.ActionType, ”SetupBahanAwal”, StringComparison.OrdinalIgnoreCase)) && _...`; pernyataan/deklarasi berikut berada di dalam batas
            // blok ini dalam BuildIngredientInventory.
            {
                // Memperbarui `inventory.Total` dengan menambahkan nilai literal `1` dalam BuildIngredientInventory.
                inventory.Total += 1;
                // Memperbarui `inventory.ByCardId[cardId]` menggunakan hasil pemilihan bersyarat: ketika `inventory.ByCardId.TryGetValue(cardId, out var qty)`
                // benar gunakan `qty + 1`, jika tidak gunakan `1` dalam BuildIngredientInventory.
                inventory.ByCardId[cardId] = inventory.ByCardId.TryGetValue(cardId, out var qty) ? qty + 1 : 1;
            // Menutup scope cabang if untuk kondisi `(string.Equals(evt.ActionType, ”BahanMasakan”, StringComparison.OrdinalIgnoreCase) ||
            // string.Equals(evt.ActionType, ”SetupBahanAwal”, StringComparison.OrdinalIgnoreCase)) && _...`; bagian berikut berada di luar batas blok tersebut
            // dalam BuildIngredientInventory.
            }

            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `evt.ActionType == ”JualMasakan”` dan
            // `_payloadReader.TryReadOrderClaim(evt.Payload, out var requiredCards, out _)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya
            // dijalankan ketika kondisi ini bernilai benar dalam BuildIngredientInventory.
            if (evt.ActionType == "JualMasakan" &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadOrderClaim` dengan `evt.Payload`, `var requiredCards`, `_` dalam
                // BuildIngredientInventory.
                _payloadReader.TryReadOrderClaim(evt.Payload, out var requiredCards, out _))
            // Membuka scope cabang if untuk kondisi `evt.ActionType == ”JualMasakan” && _payloadReader.TryReadOrderClaim(evt.Payload, out var requiredCards,
            // out _)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildIngredientInventory.
            {
                // Mengulangi setiap elemen `requiredCards`; elemen saat ini disimpan sebagai `card` bertipe `var` untuk diproses oleh badan loop dalam
                // BuildIngredientInventory.
                foreach (var card in requiredCards)
                // Membuka scope loop setiap card dari `requiredCards`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildIngredientInventory.
                {
                    // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `inventory.ByCardId.TryGetValue(card, out var qty)` dan `qty > 0`; sisi kanan
                    // diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam BuildIngredientInventory.
                    if (inventory.ByCardId.TryGetValue(card, out var qty) && qty > 0)
                    // Membuka scope cabang if untuk kondisi `inventory.ByCardId.TryGetValue(card, out var qty) && qty > 0`; pernyataan/deklarasi berikut berada di
                    // dalam batas blok ini dalam BuildIngredientInventory.
                    {
                        // Memperbarui `inventory.ByCardId[card]` menggunakan selisih antara `qty` dan `1` dalam BuildIngredientInventory.
                        inventory.ByCardId[card] = qty - 1;
                        // Memperbarui `inventory.Total` menggunakan menentukan nilai terbesar dari `0`, `inventory.Total - 1` dalam BuildIngredientInventory.
                        inventory.Total = Math.Max(0, inventory.Total - 1);
                    // Menutup scope cabang if untuk kondisi `inventory.ByCardId.TryGetValue(card, out var qty) && qty > 0`; bagian berikut berada di luar batas blok
                    // tersebut dalam BuildIngredientInventory.
                    }
                // Menutup scope loop setiap card dari `requiredCards`; bagian berikut berada di luar batas blok tersebut dalam BuildIngredientInventory.
                }
            // Menutup scope cabang if untuk kondisi `evt.ActionType == ”JualMasakan” && _payloadReader.TryReadOrderClaim(evt.Payload, out var requiredCards,
            // out _)`; bagian berikut berada di luar batas blok tersebut dalam BuildIngredientInventory.
            }

            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `evt.ActionType == ”BuangBahanMasakan” &&
            // _payloadReader.TryReadIngredientPurchase(evt.Payload, out var discardCardId, out var discardAmount)` dan
            // `inventory.ByCardId.TryGetValue(discardCardId, out var discardQty)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan
            // ketika kondisi ini bernilai benar dalam BuildIngredientInventory.
            if (evt.ActionType == "BuangBahanMasakan" &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadIngredientPurchase` dengan `evt.Payload`, `var discardCardId`, `var discardAmount`
                // dalam BuildIngredientInventory.
                _payloadReader.TryReadIngredientPurchase(evt.Payload, out var discardCardId, out var discardAmount) &&
                // Melanjutkan pengolahan dengan mencari kunci `discardCardId` pada `inventory.ByCardId`; hasil boolean menandakan kunci ditemukan dan argumen out
                // menerima nilainya dalam BuildIngredientInventory.
                inventory.ByCardId.TryGetValue(discardCardId, out var discardQty))
            // Membuka scope cabang if untuk kondisi `evt.ActionType == ”BuangBahanMasakan” && _payloadReader.TryReadIngredientPurchase(evt.Payload, out var
            // discardCardId, out var discardAmount) && inventory.ByCardId.TryGetValue(...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildIngredientInventory.
            {
                // Menyiapkan variabel lokal `newQty` untuk nilai new qty dengan menentukan nilai terbesar dari `0`, `discardQty - discardAmount`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var newQty = Math.Max(0, discardQty - discardAmount);
                // Memperbarui `inventory.ByCardId[discardCardId]` menggunakan `newQty` (nilai new qty) dalam BuildIngredientInventory.
                inventory.ByCardId[discardCardId] = newQty;
                // Memperbarui `inventory.Total` menggunakan menentukan nilai terbesar dari `0`, `inventory.Total - discardAmount` dalam BuildIngredientInventory.
                inventory.Total = Math.Max(0, inventory.Total - discardAmount);
            // Menutup scope cabang if untuk kondisi `evt.ActionType == ”BuangBahanMasakan” && _payloadReader.TryReadIngredientPurchase(evt.Payload, out var
            // discardCardId, out var discardAmount) && inventory.ByCardId.TryGetValue(...`; bagian berikut berada di luar batas blok tersebut dalam
            // BuildIngredientInventory.
            }
        // Menutup scope loop setiap evt dari `events`; bagian berikut berada di luar batas blok tersebut dalam BuildIngredientInventory.
        }

        // Mengembalikan `inventory` (nilai inventory) kepada pemanggil dalam BuildIngredientInventory; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return inventory;
    // Menutup scope metode BuildIngredientInventory; bagian berikut berada di luar batas blok tersebut dalam BuildIngredientInventory.
    }
// Menutup scope tipe IngredientInventoryCalculator; bagian berikut berada di luar batas blok tersebut.
}
