// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventDerivedStateCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Kalkulator state turunan event yang dipakai validasi domain lanjutan.
/// </summary>
// Mendefinisikan tipe class `EventDerivedStateCalculator` yang mewarisi atau menerapkan `IEventDerivedStateCalculator`; sealed mencegah tipe ini
// diturunkan lagi.
internal sealed class EventDerivedStateCalculator : IEventDerivedStateCalculator
// Membuka scope tipe EventDerivedStateCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `EventPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly EventPayloadReader _payloadReader = new();

    /// <summary>
    /// Membangun inventaris bahan pemain dari event ingredient purchased, order claimed, dan ingredient discarded.
    /// </summary>
    // Mendefinisikan metode `BuildIngredientInventory` dengan hasil bertipe `EventIngredientInventory`. Membangun inventaris bahan pemain dari event
    // ingredient purchased, order claimed, dan ingredient discarded. Masukan: Parameter `events` bertipe `IEnumerable<EventDb>` membawa kumpulan event
    // permainan sebagai sumber riwayat untuk validasi atau perhitungan; Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas.
    public EventIngredientInventory BuildIngredientInventory(IEnumerable<EventDb> events, Guid playerId)
    // Membuka scope metode BuildIngredientInventory; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildIngredientInventory.
    {
        // Menyiapkan variabel lokal `inventory` untuk nilai inventory dengan objek baru bertipe `EventIngredientInventory` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var inventory = new EventIngredientInventory();

        // Mengulangi setiap elemen `events.Where(e => e.UserId == playerId)`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh
        // badan loop dalam BuildIngredientInventory.
        foreach (var evt in events.Where(e => e.UserId == playerId))
        // Membuka scope loop setiap evt dari `events.Where(e => e.UserId == playerId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildIngredientInventory.
        {
            // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan memanggil `_payloadReader.ReadPayload` dengan
            // `evt.Payload`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var payload = _payloadReader.ReadPayload(evt.Payload);
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.BahanMasakan)`
            // dan `_payloadReader.TryReadIngredientPurchase(payload, out var cardId, out _)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya
            // dijalankan ketika kondisi ini bernilai benar dalam BuildIngredientInventory.
            if (GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.BahanMasakan) &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadIngredientPurchase` dengan `payload`, `var cardId`, `_` dalam
                // BuildIngredientInventory.
                _payloadReader.TryReadIngredientPurchase(payload, out var cardId, out _))
            // Membuka scope cabang if untuk kondisi `GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.BahanMasakan) &&
            // _payloadReader.TryReadIngredientPurchase(payload, out var cardId, out _)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildIngredientInventory.
            {
                // Memperbarui `inventory.Total` dengan menambahkan nilai literal `1` dalam BuildIngredientInventory.
                inventory.Total += 1;
                // Memperbarui `inventory.ByCardId[cardId]` menggunakan hasil pemilihan bersyarat: ketika `inventory.ByCardId.TryGetValue(cardId, out var qty)`
                // benar gunakan `qty + 1`, jika tidak gunakan `1` dalam BuildIngredientInventory.
                inventory.ByCardId[cardId] = inventory.ByCardId.TryGetValue(cardId, out var qty) ? qty + 1 : 1;
            // Menutup scope cabang if untuk kondisi `GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.BahanMasakan) &&
            // _payloadReader.TryReadIngredientPurchase(payload, out var cardId, out _)`; bagian berikut berada di luar batas blok tersebut dalam
            // BuildIngredientInventory.
            }

            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.JualMasakan)`
            // dan `_payloadReader.TryReadOrderClaim(payload, out var requiredCards, out _)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya
            // dijalankan ketika kondisi ini bernilai benar dalam BuildIngredientInventory.
            if (GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.JualMasakan) &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadOrderClaim` dengan `payload`, `var requiredCards`, `_` dalam
                // BuildIngredientInventory.
                _payloadReader.TryReadOrderClaim(payload, out var requiredCards, out _))
            // Membuka scope cabang if untuk kondisi `GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.JualMasakan) &&
            // _payloadReader.TryReadOrderClaim(payload, out var requiredCards, out _)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // BuildIngredientInventory.
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
            // Menutup scope cabang if untuk kondisi `GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.JualMasakan) &&
            // _payloadReader.TryReadOrderClaim(payload, out var requiredCards, out _)`; bagian berikut berada di luar batas blok tersebut dalam
            // BuildIngredientInventory.
            }

            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `GameActionCatalog.Is(evt.ActionType, payload,
            // GameActionCatalog.IngredientDiscarded) && _payloadReader.TryReadIngredientPurchase(payload, out var discardCardId, out var discar...` dan
            // `inventory.ByCardId.TryGetValue(discardCardId, out var discardQty)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan
            // ketika kondisi ini bernilai benar dalam BuildIngredientInventory.
            if (GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.IngredientDiscarded) &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadIngredientPurchase` dengan `payload`, `var discardCardId`, `var discardAmount`
                // dalam BuildIngredientInventory.
                _payloadReader.TryReadIngredientPurchase(payload, out var discardCardId, out var discardAmount) &&
                // Melanjutkan pengolahan dengan mencari kunci `discardCardId` pada `inventory.ByCardId`; hasil boolean menandakan kunci ditemukan dan argumen out
                // menerima nilainya dalam BuildIngredientInventory.
                inventory.ByCardId.TryGetValue(discardCardId, out var discardQty))
            // Membuka scope cabang if untuk kondisi `GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.IngredientDiscarded) &&
            // _payloadReader.TryReadIngredientPurchase(payload, out var discardCardId, out var discar...`; pernyataan/deklarasi berikut berada di dalam batas
            // blok ini dalam BuildIngredientInventory.
            {
                // Menyiapkan variabel lokal `newQty` untuk nilai new qty dengan menentukan nilai terbesar dari `0`, `discardQty - discardAmount`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var newQty = Math.Max(0, discardQty - discardAmount);
                // Memperbarui `inventory.ByCardId[discardCardId]` menggunakan `newQty` (nilai new qty) dalam BuildIngredientInventory.
                inventory.ByCardId[discardCardId] = newQty;
                // Memperbarui `inventory.Total` menggunakan menentukan nilai terbesar dari `0`, `inventory.Total - discardAmount` dalam BuildIngredientInventory.
                inventory.Total = Math.Max(0, inventory.Total - discardAmount);
            // Menutup scope cabang if untuk kondisi `GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.IngredientDiscarded) &&
            // _payloadReader.TryReadIngredientPurchase(payload, out var discardCardId, out var discar...`; bagian berikut berada di luar batas blok tersebut
            // dalam BuildIngredientInventory.
            }
        // Menutup scope loop setiap evt dari `events.Where(e => e.UserId == playerId)`; bagian berikut berada di luar batas blok tersebut dalam
        // BuildIngredientInventory.
        }

        // Mengembalikan `inventory` (nilai inventory) kepada pemanggil dalam BuildIngredientInventory; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return inventory;
    // Menutup scope metode BuildIngredientInventory; bagian berikut berada di luar batas blok tersebut dalam BuildIngredientInventory.
    }

    /// <summary>
    /// Menghitung saldo tabungan pemain untuk goal tertentu dari deposit, penarikan, dan pencapaian goal.
    /// </summary>
    // Mendefinisikan metode `ComputeSavingBalance` dengan hasil bertipe `int`. Menghitung saldo tabungan pemain untuk goal tertentu dari deposit,
    // penarikan, dan pencapaian goal. Masukan: Parameter `events` bertipe `IEnumerable<EventDb>` membawa kumpulan event permainan sebagai sumber
    // riwayat untuk validasi atau perhitungan; Parameter `playerId` bertipe `Guid` membawa nilai pemain identitas; Parameter `goalId` bertipe `string`
    // membawa nilai target identitas.
    public int ComputeSavingBalance(IEnumerable<EventDb> events, Guid playerId, string goalId)
    // Membuka scope metode ComputeSavingBalance; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ComputeSavingBalance.
    {
        // Menyiapkan variabel lokal `balance` untuk saldo uang pemain pada keadaan yang sedang diproses dengan nilai literal `0`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var balance = 0;

        // Mengulangi setiap elemen `events.Where(e => e.UserId == playerId)`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh
        // badan loop dalam ComputeSavingBalance.
        foreach (var evt in events.Where(e => e.UserId == playerId))
        // Membuka scope loop setiap evt dari `events.Where(e => e.UserId == playerId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ComputeSavingBalance.
        {
            // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan memanggil `_payloadReader.ReadPayload` dengan
            // `evt.Payload`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var payload = _payloadReader.ReadPayload(evt.Payload);
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.Menabung) &&
            // _payloadReader.TryReadSavingDeposit(payload, out var existingGoalId, out var amount)` dan `string.Equals(existingGoalId, goalId,
            // StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar
            // dalam ComputeSavingBalance.
            if (GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.Menabung) &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadSavingDeposit` dengan `payload`, `var existingGoalId`, `var amount` dalam
                // ComputeSavingBalance.
                _payloadReader.TryReadSavingDeposit(payload, out var existingGoalId, out var amount) &&
                // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `existingGoalId`, `goalId`, `StringComparison.OrdinalIgnoreCase`; aturan
                // perbandingan mengikuti overload dan comparer yang diberikan dalam ComputeSavingBalance.
                string.Equals(existingGoalId, goalId, StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.Menabung) &&
            // _payloadReader.TryReadSavingDeposit(payload, out var existingGoalId, out var amount) && string.Equ...`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam ComputeSavingBalance.
            {
                // Memperbarui `balance` dengan menambahkan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam ComputeSavingBalance.
                balance += amount;
            // Menutup scope cabang if untuk kondisi `GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.Menabung) &&
            // _payloadReader.TryReadSavingDeposit(payload, out var existingGoalId, out var amount) && string.Equ...`; bagian berikut berada di luar batas blok
            // tersebut dalam ComputeSavingBalance.
            }

            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `GameActionCatalog.Is(evt.ActionType, payload,
            // GameActionCatalog.SavingDepositWithdrawn) && _payloadReader.TryReadSavingDeposit(payload, out var withdrawGoalId, out var amountW...` dan
            // `string.Equals(withdrawGoalId, goalId, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya
            // dijalankan ketika kondisi ini bernilai benar dalam ComputeSavingBalance.
            if (GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.SavingDepositWithdrawn) &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadSavingDeposit` dengan `payload`, `var withdrawGoalId`, `var amountWithdraw` dalam
                // ComputeSavingBalance.
                _payloadReader.TryReadSavingDeposit(payload, out var withdrawGoalId, out var amountWithdraw) &&
                // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `withdrawGoalId`, `goalId`, `StringComparison.OrdinalIgnoreCase`; aturan
                // perbandingan mengikuti overload dan comparer yang diberikan dalam ComputeSavingBalance.
                string.Equals(withdrawGoalId, goalId, StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.SavingDepositWithdrawn) &&
            // _payloadReader.TryReadSavingDeposit(payload, out var withdrawGoalId, out var amountW...`; pernyataan/deklarasi berikut berada di dalam batas blok
            // ini dalam ComputeSavingBalance.
            {
                // Memperbarui `balance` dengan mengurangi `amountWithdraw` (nilai nominal withdraw) dalam ComputeSavingBalance.
                balance -= amountWithdraw;
            // Menutup scope cabang if untuk kondisi `GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.SavingDepositWithdrawn) &&
            // _payloadReader.TryReadSavingDeposit(payload, out var withdrawGoalId, out var amountW...`; bagian berikut berada di luar batas blok tersebut dalam
            // ComputeSavingBalance.
            }

            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.TujuanFinansial)
            // && _payloadReader.TryReadSavingGoalAchieved(payload, out var achievedGoalId, out _, out var co...` dan `string.Equals(achievedGoalId, goalId,
            // StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar
            // dalam ComputeSavingBalance.
            if (GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.TujuanFinansial) &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadSavingGoalAchieved` dengan `payload`, `var achievedGoalId`, `_`, `var cost` dalam
                // ComputeSavingBalance.
                _payloadReader.TryReadSavingGoalAchieved(payload, out var achievedGoalId, out _, out var cost) &&
                // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `achievedGoalId`, `goalId`, `StringComparison.OrdinalIgnoreCase`; aturan
                // perbandingan mengikuti overload dan comparer yang diberikan dalam ComputeSavingBalance.
                string.Equals(achievedGoalId, goalId, StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.TujuanFinansial) &&
            // _payloadReader.TryReadSavingGoalAchieved(payload, out var achievedGoalId, out _, out var co...`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam ComputeSavingBalance.
            {
                // Memperbarui `balance` dengan mengurangi `cost` (nilai biaya) dalam ComputeSavingBalance.
                balance -= cost;
            // Menutup scope cabang if untuk kondisi `GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.TujuanFinansial) &&
            // _payloadReader.TryReadSavingGoalAchieved(payload, out var achievedGoalId, out _, out var co...`; bagian berikut berada di luar batas blok
            // tersebut dalam ComputeSavingBalance.
            }
        // Menutup scope loop setiap evt dari `events.Where(e => e.UserId == playerId)`; bagian berikut berada di luar batas blok tersebut dalam
        // ComputeSavingBalance.
        }

        // Mengembalikan `balance` (saldo uang pemain pada keadaan yang sedang diproses) kepada pemanggil dalam ComputeSavingBalance; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return balance;
    // Menutup scope metode ComputeSavingBalance; bagian berikut berada di luar batas blok tersebut dalam ComputeSavingBalance.
    }
// Menutup scope tipe EventDerivedStateCalculator; bagian berikut berada di luar batas blok tersebut.
}
