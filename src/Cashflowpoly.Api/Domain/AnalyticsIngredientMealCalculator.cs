// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsIngredientMealCalculator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsIngredientMealMetrics`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed record AnalyticsIngredientMealMetrics(
    // Parameter `Inventory` bertipe `AnalyticsIngredientInventory` membawa nilai inventory.
    AnalyticsIngredientInventory Inventory,
    // Parameter `IngredientTypesHeld` bertipe `IReadOnlyDictionary<string, int>` membawa nilai bahan types held.
    IReadOnlyDictionary<string, int> IngredientTypesHeld,
    // Parameter `IngredientsCollected` bertipe `int` membawa nilai bahan collected.
    int IngredientsCollected,
    // Parameter `IngredientsUsedPerMeal` bertipe `IReadOnlyList<int>` membawa nilai bahan used per meal.
    IReadOnlyList<int> IngredientsUsedPerMeal,
    // Parameter `IngredientsUsedTotal` bertipe `int` membawa nilai bahan used total.
    int IngredientsUsedTotal,
    // Parameter `IngredientsWasted` bertipe `int` membawa nilai bahan wasted.
    int IngredientsWasted,
    // Parameter `IngredientInvestmentTotal` bertipe `int` membawa nilai bahan investment total.
    int IngredientInvestmentTotal,
    // Parameter `MealOrderIncomeValues` bertipe `IReadOnlyList<int>` membawa nilai meal urutan/pesanan pemasukan nilai.
    IReadOnlyList<int> MealOrderIncomeValues,
    // Parameter `MealOrdersClaimed` bertipe `int` membawa nilai meal pesanan claimed.
    int MealOrdersClaimed,
    // Parameter `MealOrderIncomeTotal` bertipe `int` membawa nilai meal urutan/pesanan pemasukan total.
    int MealOrderIncomeTotal,
    // Parameter `LatestDayIndex` bertipe `int` membawa nilai latest hari index.
    int LatestDayIndex,
    // Parameter `MealOrdersPerTurnAverage` bertipe `double` membawa nilai meal pesanan per giliran rata-rata.
    double MealOrdersPerTurnAverage,
    // Parameter `EssentialIngredientExpenses` bertipe `double` membawa nilai essential bahan expenses.
    double EssentialIngredientExpenses);

// Mendefinisikan tipe class `IngredientMealCalculator` yang mewarisi atau menerapkan `IIngredientMealCalculator`; sealed mencegah tipe ini
// diturunkan lagi.
internal sealed class IngredientMealCalculator : IIngredientMealCalculator
// Membuka scope tipe IngredientMealCalculator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `HashSet<string>`: `_retiredActionTypes` menyimpan nilai retired aksi types dengan nilai awal objek baru dengan
    // tipe mengikuti konteks tujuan dan argumen (StringComparer.OrdinalIgnoreCase). readonly membatasi penggantian referensi/nilai field pada deklarasi
    // atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly HashSet<string> _retiredActionTypes = new(StringComparer.OrdinalIgnoreCase)
    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Menggunakan nilai literal `”LewatiOrder”` sebagai bagian ekspresi yang sedang disusun.
        "LewatiOrder",
        // Menggunakan nilai literal `”AmbilKartuDariDeck”` sebagai bagian ekspresi yang sedang disusun.
        "AmbilKartuDariDeck",
        // Menggunakan nilai literal `”KartuMasukDiscard”` sebagai bagian ekspresi yang sedang disusun.
        "KartuMasukDiscard",
        // Menggunakan nilai literal `”IsiUlangPasar”` sebagai bagian ekspresi yang sedang disusun.
        "IsiUlangPasar"
    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut.
    };
    // Mendeklarasikan field bertipe `AnalyticsPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly AnalyticsPayloadReader _payloadReader = new();
    // Mendeklarasikan field bertipe `IngredientInventoryCalculator`: `_inventoryCalculator` menyimpan nilai inventory kalkulator dengan nilai awal
    // objek baru dengan tipe mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau
    // konstruktor. static membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly IngredientInventoryCalculator _inventoryCalculator = new();

    // Mendefinisikan metode `Compute` dengan hasil bertipe `AnalyticsIngredientMealMetrics`; operasi ini menangani compute. Masukan: Parameter
    // `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event; Parameter `playerProjections` bertipe
    // `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections.
    public AnalyticsIngredientMealMetrics Compute(
        // Parameter `playerEvents` bertipe `IReadOnlyCollection<EventDb>` membawa nilai pemain event.
        IReadOnlyCollection<EventDb> playerEvents,
        // Parameter `playerProjections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai pemain projections.
        IReadOnlyCollection<CashflowProjectionDb> playerProjections)
    // Membuka scope metode Compute; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
    {
        // Memperbarui `playerEvents` menggunakan mematerialisasi urutan `playerEvents.Where(e => !_retiredActionTypes.Contains(e.ActionType))` menjadi
        // array dengan elemen hasil saat ini dalam Compute.
        playerEvents = playerEvents.Where(e => !_retiredActionTypes.Contains(e.ActionType)).ToArray();

        // Menyiapkan variabel lokal `ingredientPurchaseMap` untuk nilai bahan pembelian pemetaan dengan objek baru bertipe `Dictionary<string, string>`
        // dengan argumen (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ingredientPurchaseMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `ingredientsCollected` untuk nilai bahan collected dengan nilai literal `0`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var ingredientsCollected = 0;
        // Mengulangi setiap elemen `playerEvents.Where(e => string.Equals(e.ActionType, ”BahanMasakan”, StringComparison.OrdinalIgnoreCase) ||
        // string.Equals(e.ActionType, ”SetupBahanAwal”, StringComparison.Ordin...`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses
        // oleh badan loop dalam Compute.
        foreach (var evt in playerEvents.Where(e => string.Equals(e.ActionType, "BahanMasakan", StringComparison.OrdinalIgnoreCase) || string.Equals(e.ActionType, "SetupBahanAwal", StringComparison.OrdinalIgnoreCase)))
        // Membuka scope loop setiap evt dari `playerEvents.Where(e => string.Equals(e.ActionType, ”BahanMasakan”, StringComparison.OrdinalIgnoreCase) ||
        // string.Equals(e.ActionType, ”SetupBahanAwal”, StringComparison.Ordin...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Compute.
        {
            // Memeriksa memanggil `_payloadReader.TryReadIngredientPurchaseDetailed` dengan `evt.Payload`, `var cardId`, `var ingredientName`, `var amount`;
            // blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Compute.
            if (_payloadReader.TryReadIngredientPurchaseDetailed(evt.Payload, out var cardId, out var ingredientName, out var amount))
            // Membuka scope cabang if untuk kondisi `_payloadReader.TryReadIngredientPurchaseDetailed(evt.Payload, out var cardId, out var ingredientName, out
            // var amount)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Memperbarui `ingredientsCollected` dengan menambahkan nilai literal `1` dalam Compute.
                ingredientsCollected += 1;
                // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!string.IsNullOrWhiteSpace(cardId)` dan
                // `!string.IsNullOrWhiteSpace(ingredientName)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini
                // bernilai benar dalam Compute.
                if (!string.IsNullOrWhiteSpace(cardId) && !string.IsNullOrWhiteSpace(ingredientName))
                // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(cardId) && !string.IsNullOrWhiteSpace(ingredientName)`; pernyataan/deklarasi
                // berikut berada di dalam batas blok ini dalam Compute.
                {
                    // Memperbarui `ingredientPurchaseMap[cardId]` menggunakan `ingredientName` (nilai bahan nama) dalam Compute.
                    ingredientPurchaseMap[cardId] = ingredientName;
                // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(cardId) && !string.IsNullOrWhiteSpace(ingredientName)`; bagian berikut berada
                // di luar batas blok tersebut dalam Compute.
                }
            // Menutup scope cabang if untuk kondisi `_payloadReader.TryReadIngredientPurchaseDetailed(evt.Payload, out var cardId, out var ingredientName, out
            // var amount)`; bagian berikut berada di luar batas blok tersebut dalam Compute.
            }
            // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam Compute.
            else if (_payloadReader.TryReadIngredientPurchase(evt.Payload, out var fallbackCardId, out var fallbackAmount))
            // Membuka scope cabang if untuk kondisi `_payloadReader.TryReadIngredientPurchase(evt.Payload, out var fallbackCardId, out var fallbackAmount)`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Memperbarui `ingredientsCollected` dengan menambahkan nilai literal `1` dalam Compute.
                ingredientsCollected += 1;
                // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!string.IsNullOrWhiteSpace(fallbackCardId)` dan
                // `!ingredientPurchaseMap.ContainsKey(fallbackCardId)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi
                // ini bernilai benar dalam Compute.
                if (!string.IsNullOrWhiteSpace(fallbackCardId) && !ingredientPurchaseMap.ContainsKey(fallbackCardId))
                // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(fallbackCardId) && !ingredientPurchaseMap.ContainsKey(fallbackCardId)`;
                // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
                {
                    // Memperbarui `ingredientPurchaseMap[fallbackCardId]` menggunakan `fallbackCardId` (nilai fallback kartu identitas) dalam Compute.
                    ingredientPurchaseMap[fallbackCardId] = fallbackCardId;
                // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(fallbackCardId) && !ingredientPurchaseMap.ContainsKey(fallbackCardId)`; bagian
                // berikut berada di luar batas blok tersebut dalam Compute.
                }
            // Menutup scope cabang if untuk kondisi `_payloadReader.TryReadIngredientPurchase(evt.Payload, out var fallbackCardId, out var fallbackAmount)`;
            // bagian berikut berada di luar batas blok tersebut dalam Compute.
            }
        // Menutup scope loop setiap evt dari `playerEvents.Where(e => string.Equals(e.ActionType, ”BahanMasakan”, StringComparison.OrdinalIgnoreCase) ||
        // string.Equals(e.ActionType, ”SetupBahanAwal”, StringComparison.Ordin...`; bagian berikut berada di luar batas blok tersebut dalam Compute.
        }

        // Menyiapkan variabel lokal `inventory` untuk nilai inventory dengan memanggil `_inventoryCalculator.BuildIngredientInventory` dengan
        // `playerEvents.ToList()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var inventory = _inventoryCalculator.BuildIngredientInventory(playerEvents.ToList());
        // Menyiapkan variabel lokal `ingredientTypesHeld` untuk nilai bahan types held dengan objek baru bertipe `Dictionary<string, int>` dengan argumen
        // (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ingredientTypesHeld = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        // Melengkapi struktur ekspresi ForEachVariableStatement melalui foreach (var (cardId, qty) in inventory.ByCardId) dalam Compute; token pada baris
        // ini menyambungkan bagian kode sebelum dan sesudahnya.
        foreach (var (cardId, qty) in inventory.ByCardId)
        // Membuka scope blok ForEachVariableStatement; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
        {
            // Memeriksa pemeriksaan lebih kecil atau sama antara `qty` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Compute.
            if (qty <= 0)
            // Membuka scope cabang if untuk kondisi `qty <= 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Compute.
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam Compute.
                continue;
            // Menutup scope cabang if untuk kondisi `qty <= 0`; bagian berikut berada di luar batas blok tersebut dalam Compute.
            }

            // Menyiapkan variabel lokal `name` untuk nilai nama dengan hasil pemilihan bersyarat: ketika `ingredientPurchaseMap.TryGetValue(cardId, out var
            // ingredientName)` benar gunakan `ingredientName`, jika tidak gunakan `cardId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var name = ingredientPurchaseMap.TryGetValue(cardId, out var ingredientName) ? ingredientName : cardId;
            // Memperbarui `ingredientTypesHeld[name]` menggunakan hasil pemilihan bersyarat: ketika `ingredientTypesHeld.TryGetValue(name, out var existing)`
            // benar gunakan `existing + qty`, jika tidak gunakan `qty` dalam Compute.
            ingredientTypesHeld[name] = ingredientTypesHeld.TryGetValue(name, out var existing) ? existing + qty : qty;
        // Menutup scope blok ForEachVariableStatement; bagian berikut berada di luar batas blok tersebut dalam Compute.
        }

        // Menyiapkan variabel lokal `ingredientsUsedPerMeal` untuk nilai bahan used per meal dengan mematerialisasi urutan `playerEvents .Where(e =>
        // e.ActionType == ”JualMasakan”) .Select(e => _payloadReader.TryReadOrderClaim(e.Payload, out var cards, out _) ? cards.Count : 0)` menjadi List;
        // enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ingredientsUsedPerMeal = playerEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => e.ActionType == ”JualMasakan”) dalam Compute; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(e => e.ActionType == "JualMasakan")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(e => _payloadReader.TryReadOrderClaim(e.Payload, out var cards, out _)
            // ? cards.Count : 0) dalam Compute; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(e => _payloadReader.TryReadOrderClaim(e.Payload, out var cards, out _) ? cards.Count : 0)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam Compute; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `ingredientsUsedTotal` untuk nilai bahan used total dengan menjumlahkan nilai `ingredientsUsedPerMeal`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var ingredientsUsedTotal = ingredientsUsedPerMeal.Sum();

        // Menyiapkan variabel lokal `ingredientsWasted` untuk nilai bahan wasted dengan menjumlahkan nilai `playerEvents .Where(e => e.ActionType ==
        // ”BuangBahanMasakan”) .Select(e => _payloadReader.TryReadIngredientPurchase(e.Payload, out _, out var amount) ? amount : 0)`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var ingredientsWasted = playerEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => e.ActionType == ”BuangBahanMasakan”) dalam Compute; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(e => e.ActionType == "BuangBahanMasakan")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(e => _payloadReader.TryReadIngredientPurchase(e.Payload, out _, out var
            // amount) ? amount : 0) dalam Compute; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Select(e => _payloadReader.TryReadIngredientPurchase(e.Payload, out _, out var amount) ? amount : 0)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Sum(); dalam Compute; token pada baris ini menyambungkan bagian kode sebelum
            // dan sesudahnya.
            .Sum();

        // Menyiapkan variabel lokal `ingredientInvestmentTotal` untuk nilai bahan investment total dengan menjumlahkan nilai `playerProjections .Where(p =>
        // p.Category == ”INGREDIENT” && p.Direction == ”OUT”)` berdasarkan `p => p.Amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var ingredientInvestmentTotal = playerProjections
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(p => p.Category == ”INGREDIENT” && p.Direction == ”OUT”) dalam Compute;
            // token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(p => p.Category == "INGREDIENT" && p.Direction == "OUT")
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Sum(p => p.Amount); dalam Compute; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .Sum(p => p.Amount);

        // Menyiapkan variabel lokal `mealOrderIncomeValues` untuk nilai meal urutan/pesanan pemasukan nilai dengan objek baru bertipe `List<int>` dengan
        // nilai awal sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var mealOrderIncomeValues = new List<int>();
        // Mengulangi setiap elemen `playerEvents.Where(e => e.ActionType == ”JualMasakan”)`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk
        // diproses oleh badan loop dalam Compute.
        foreach (var evt in playerEvents.Where(e => e.ActionType == "JualMasakan"))
        // Membuka scope loop setiap evt dari `playerEvents.Where(e => e.ActionType == ”JualMasakan”)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam Compute.
        {
            // Memeriksa memanggil `_payloadReader.TryReadOrderClaim` dengan `evt.Payload`, `_`, `var income`; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam Compute.
            if (_payloadReader.TryReadOrderClaim(evt.Payload, out _, out var income))
            // Membuka scope cabang if untuk kondisi `_payloadReader.TryReadOrderClaim(evt.Payload, out _, out var income)`; pernyataan/deklarasi berikut berada
            // di dalam batas blok ini dalam Compute.
            {
                // Menjalankan menambahkan `income` ke `mealOrderIncomeValues` dalam Compute.
                mealOrderIncomeValues.Add(income);
            // Menutup scope cabang if untuk kondisi `_payloadReader.TryReadOrderClaim(evt.Payload, out _, out var income)`; bagian berikut berada di luar batas
            // blok tersebut dalam Compute.
            }
        // Menutup scope loop setiap evt dari `playerEvents.Where(e => e.ActionType == ”JualMasakan”)`; bagian berikut berada di luar batas blok tersebut
        // dalam Compute.
        }

        // Menyiapkan variabel lokal `mealOrdersClaimed` untuk nilai meal pesanan claimed dengan `mealOrderIncomeValues.Count`, yaitu jumlah elemen atau
        // panjang data. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var mealOrdersClaimed = mealOrderIncomeValues.Count;
        // Menyiapkan variabel lokal `mealOrderIncomeTotal` untuk nilai meal urutan/pesanan pemasukan total dengan menjumlahkan nilai
        // `mealOrderIncomeValues`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var mealOrderIncomeTotal = mealOrderIncomeValues.Sum();
        // Menyiapkan variabel lokal `latestDayIndex` untuk nilai latest hari index dengan hasil pemilihan bersyarat: ketika `playerEvents.Count == 0` benar
        // gunakan `-1`, jika tidak gunakan `playerEvents.Max(e => e.DayIndex)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var latestDayIndex = playerEvents.Count == 0 ? -1 : playerEvents.Max(e => e.DayIndex);
        // Menyiapkan variabel lokal `eventPayloadReader` untuk nilai event payload pembaca dengan objek baru bertipe `EventPayloadReader` dengan nilai awal
        // sesuai konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var eventPayloadReader = new EventPayloadReader();
        // Menyiapkan variabel lokal `playedTurnCount` untuk nilai played giliran jumlah dengan memanggil `playerEvents .Where(e =>
        // string.Equals(e.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase) && GameActionCatalog.GetPlayerActionSlotPolicy( e.ActionType,
        // eventPayloadRea...` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var playedTurnCount = playerEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => string.Equals(e.ActorType, ”PLAYER”,
            // StringComparison.OrdinalIgnoreCase) && dalam Compute; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(e => string.Equals(e.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
                        // Meneruskan fungsi lambda `e => string.Equals(e.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase) &&
                        // GameActionCatalog.GetPlayerActionSlotPolicy( e.ActionType, eventPayloadReader.ReadPayload(stri...` yang dijalankan oleh operasi pemanggil untuk
                        // memproses setiap masukan sebagai argumen ke `playerEvents .Where`.
                        GameActionCatalog.GetPlayerActionSlotPolicy(
                            // Meneruskan `e.ActionType` (nilai aksi jenis) sebagai argumen ke `GameActionCatalog.GetPlayerActionSlotPolicy`.
                            e.ActionType,
                            // Meneruskan memanggil `eventPayloadReader.ReadPayload` dengan `string.IsNullOrWhiteSpace(e.Payload) ? ”{}” : e.Payload` sebagai argumen ke
                            // `GameActionCatalog.GetPlayerActionSlotPolicy`; Meneruskan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(e.Payload)` benar gunakan
                            // `”{}”`, jika tidak gunakan `e.Payload` sebagai argumen ke `eventPayloadReader.ReadPayload`; Meneruskan `e.Payload` (muatan detail event dalam
                            // format JSON) sebagai argumen ke `string.IsNullOrWhiteSpace`.
                            eventPayloadReader.ReadPayload(string.IsNullOrWhiteSpace(e.Payload) ? "{}" : e.Payload)) == PlayerActionSlotPolicy.Consumes)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(e => e.DayIndex) dalam Compute; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .Select(e => e.DayIndex)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Distinct() dalam Compute; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .Distinct()
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Count(); dalam Compute; token pada baris ini menyambungkan bagian kode sebelum
            // dan sesudahnya.
            .Count();
        // Menyiapkan variabel lokal `mealOrdersPerTurnAverage` untuk nilai meal pesanan per giliran rata-rata dengan hasil pemilihan bersyarat: ketika
        // `playedTurnCount > 0` benar gunakan `(double)mealOrdersClaimed / playedTurnCount`, jika tidak gunakan `0`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var mealOrdersPerTurnAverage = playedTurnCount > 0 ? (double)mealOrdersClaimed / playedTurnCount : 0;
        // Menyiapkan variabel lokal `essentialIngredientExpenses` untuk nilai essential bahan expenses dengan memanggil
        // `ComputeEssentialIngredientExpenses` dengan `playerEvents`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var essentialIngredientExpenses = ComputeEssentialIngredientExpenses(playerEvents);

        // Mengembalikan objek baru bertipe `AnalyticsIngredientMealMetrics` dengan argumen ( inventory, ingredientTypesHeld, ingredientsCollected,
        // ingredientsUsedPerMeal, ingredientsUsedTotal, ingredientsWasted, ingredientInvestmentTotal, mealOrderInc... kepada pemanggil dalam Compute;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new AnalyticsIngredientMealMetrics(
            // Meneruskan `inventory` (nilai inventory) sebagai argumen ke konstruktor `AnalyticsIngredientMealMetrics`.
            inventory,
            // Meneruskan `ingredientTypesHeld` (nilai bahan types held) sebagai argumen ke konstruktor `AnalyticsIngredientMealMetrics`.
            ingredientTypesHeld,
            // Meneruskan `ingredientsCollected` (nilai bahan collected) sebagai argumen ke konstruktor `AnalyticsIngredientMealMetrics`.
            ingredientsCollected,
            // Meneruskan `ingredientsUsedPerMeal` (nilai bahan used per meal) sebagai argumen ke konstruktor `AnalyticsIngredientMealMetrics`.
            ingredientsUsedPerMeal,
            // Meneruskan `ingredientsUsedTotal` (nilai bahan used total) sebagai argumen ke konstruktor `AnalyticsIngredientMealMetrics`.
            ingredientsUsedTotal,
            // Meneruskan `ingredientsWasted` (nilai bahan wasted) sebagai argumen ke konstruktor `AnalyticsIngredientMealMetrics`.
            ingredientsWasted,
            // Meneruskan `ingredientInvestmentTotal` (nilai bahan investment total) sebagai argumen ke konstruktor `AnalyticsIngredientMealMetrics`.
            ingredientInvestmentTotal,
            // Meneruskan `mealOrderIncomeValues` (nilai meal urutan/pesanan pemasukan nilai) sebagai argumen ke konstruktor `AnalyticsIngredientMealMetrics`.
            mealOrderIncomeValues,
            // Meneruskan `mealOrdersClaimed` (nilai meal pesanan claimed) sebagai argumen ke konstruktor `AnalyticsIngredientMealMetrics`.
            mealOrdersClaimed,
            // Meneruskan `mealOrderIncomeTotal` (nilai meal urutan/pesanan pemasukan total) sebagai argumen ke konstruktor `AnalyticsIngredientMealMetrics`.
            mealOrderIncomeTotal,
            // Meneruskan `latestDayIndex` (nilai latest hari index) sebagai argumen ke konstruktor `AnalyticsIngredientMealMetrics`.
            latestDayIndex,
            // Meneruskan `mealOrdersPerTurnAverage` (nilai meal pesanan per giliran rata-rata) sebagai argumen ke konstruktor `AnalyticsIngredientMealMetrics`.
            mealOrdersPerTurnAverage,
            // Meneruskan `essentialIngredientExpenses` (nilai essential bahan expenses) sebagai argumen ke konstruktor `AnalyticsIngredientMealMetrics`.
            essentialIngredientExpenses);
    // Menutup scope metode Compute; bagian berikut berada di luar batas blok tersebut dalam Compute.
    }

    // Mendefinisikan metode `ComputeEssentialIngredientExpenses` dengan hasil bertipe `double`; operasi ini menangani compute essential bahan expenses.
    // Masukan: Parameter `playerEvents` bertipe `IEnumerable<EventDb>` membawa nilai pemain event.
    private double ComputeEssentialIngredientExpenses(IEnumerable<EventDb> playerEvents)
    // Membuka scope metode ComputeEssentialIngredientExpenses; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ComputeEssentialIngredientExpenses.
    {
        // Menyiapkan variabel lokal `purchaseCostByCardId` untuk nilai pembelian biaya berdasarkan kartu identitas dengan objek baru bertipe
        // `Dictionary<string, Queue<double>>` dengan argumen (StringComparer.OrdinalIgnoreCase). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var purchaseCostByCardId = new Dictionary<string, Queue<double>>(StringComparer.OrdinalIgnoreCase);
        // Menyiapkan variabel lokal `essentialIngredientExpenses` untuk nilai essential bahan expenses dengan nilai literal `0d`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var essentialIngredientExpenses = 0d;

        // Mengulangi setiap elemen `playerEvents.OrderBy(e => e.SequenceNumber)`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh
        // badan loop dalam ComputeEssentialIngredientExpenses.
        foreach (var evt in playerEvents.OrderBy(e => e.SequenceNumber))
        // Membuka scope loop setiap evt dari `playerEvents.OrderBy(e => e.SequenceNumber)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ComputeEssentialIngredientExpenses.
        {
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `(string.Equals(evt.ActionType, ”BahanMasakan”,
            // StringComparison.OrdinalIgnoreCase) || string.Equals(evt.ActionType, ”SetupBahanAwal”, StringComparison.OrdinalIgnoreCase)) && _...` dan
            // `!string.IsNullOrWhiteSpace(purchasedCardId)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam ComputeEssentialIngredientExpenses.
            if ((string.Equals(evt.ActionType, "BahanMasakan", StringComparison.OrdinalIgnoreCase) ||
                 // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `evt.ActionType`, `”SetupBahanAwal”`, `StringComparison.OrdinalIgnoreCase`;
                 // aturan perbandingan mengikuti overload dan comparer yang diberikan dalam ComputeEssentialIngredientExpenses.
                 string.Equals(evt.ActionType, "SetupBahanAwal", StringComparison.OrdinalIgnoreCase)) &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadIngredientPurchase` dengan `evt.Payload`, `var purchasedCardId`, `var
                // purchaseAmount` dalam ComputeEssentialIngredientExpenses.
                _payloadReader.TryReadIngredientPurchase(evt.Payload, out var purchasedCardId, out var purchaseAmount) &&
                // Menggunakan kebalikan kondisi `string.IsNullOrWhiteSpace(purchasedCardId)` sebagai bagian ekspresi yang sedang disusun dalam
                // ComputeEssentialIngredientExpenses.
                !string.IsNullOrWhiteSpace(purchasedCardId))
            // Membuka scope cabang if untuk kondisi `(string.Equals(evt.ActionType, ”BahanMasakan”, StringComparison.OrdinalIgnoreCase) ||
            // string.Equals(evt.ActionType, ”SetupBahanAwal”, StringComparison.OrdinalIgnoreCase)) && _...`; pernyataan/deklarasi berikut berada di dalam batas
            // blok ini dalam ComputeEssentialIngredientExpenses.
            {
                // Memeriksa kebalikan kondisi `purchaseCostByCardId.TryGetValue(purchasedCardId, out var queue)`; blok if hanya dijalankan ketika kondisi ini
                // bernilai benar dalam ComputeEssentialIngredientExpenses.
                if (!purchaseCostByCardId.TryGetValue(purchasedCardId, out var queue))
                // Membuka scope cabang if untuk kondisi `!purchaseCostByCardId.TryGetValue(purchasedCardId, out var queue)`; pernyataan/deklarasi berikut berada di
                // dalam batas blok ini dalam ComputeEssentialIngredientExpenses.
                {
                    // Memperbarui `queue` menggunakan objek baru bertipe `Queue<double>` dengan nilai awal sesuai konstruktornya dalam
                    // ComputeEssentialIngredientExpenses.
                    queue = new Queue<double>();
                    // Memperbarui `purchaseCostByCardId[purchasedCardId]` menggunakan `queue` (nilai queue) dalam ComputeEssentialIngredientExpenses.
                    purchaseCostByCardId[purchasedCardId] = queue;
                // Menutup scope cabang if untuk kondisi `!purchaseCostByCardId.TryGetValue(purchasedCardId, out var queue)`; bagian berikut berada di luar batas
                // blok tersebut dalam ComputeEssentialIngredientExpenses.
                }

                // Menjalankan memanggil `queue.Enqueue` dengan `Math.Max(0, purchaseAmount)` dalam ComputeEssentialIngredientExpenses.
                queue.Enqueue(Math.Max(0, purchaseAmount));
            // Menutup scope cabang if untuk kondisi `(string.Equals(evt.ActionType, ”BahanMasakan”, StringComparison.OrdinalIgnoreCase) ||
            // string.Equals(evt.ActionType, ”SetupBahanAwal”, StringComparison.OrdinalIgnoreCase)) && _...`; bagian berikut berada di luar batas blok tersebut
            // dalam ComputeEssentialIngredientExpenses.
            }

            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `evt.ActionType == ”JualMasakan”` dan
            // `_payloadReader.TryReadOrderClaim(evt.Payload, out var requiredCards, out _)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya
            // dijalankan ketika kondisi ini bernilai benar dalam ComputeEssentialIngredientExpenses.
            if (evt.ActionType == "JualMasakan" &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadOrderClaim` dengan `evt.Payload`, `var requiredCards`, `_` dalam
                // ComputeEssentialIngredientExpenses.
                _payloadReader.TryReadOrderClaim(evt.Payload, out var requiredCards, out _))
            // Membuka scope cabang if untuk kondisi `evt.ActionType == ”JualMasakan” && _payloadReader.TryReadOrderClaim(evt.Payload, out var requiredCards,
            // out _)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ComputeEssentialIngredientExpenses.
            {
                // Mengulangi setiap elemen `requiredCards`; elemen saat ini disimpan sebagai `requiredCard` bertipe `var` untuk diproses oleh badan loop dalam
                // ComputeEssentialIngredientExpenses.
                foreach (var requiredCard in requiredCards)
                // Membuka scope loop setiap requiredCard dari `requiredCards`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // ComputeEssentialIngredientExpenses.
                {
                    // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!purchaseCostByCardId.TryGetValue(requiredCard, out var queue)` dan
                    // `queue.Count == 0`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                    // ComputeEssentialIngredientExpenses.
                    if (!purchaseCostByCardId.TryGetValue(requiredCard, out var queue) || queue.Count == 0)
                    // Membuka scope cabang if untuk kondisi `!purchaseCostByCardId.TryGetValue(requiredCard, out var queue) || queue.Count == 0`; pernyataan/deklarasi
                    // berikut berada di dalam batas blok ini dalam ComputeEssentialIngredientExpenses.
                    {
                        // Memperbarui `essentialIngredientExpenses` dengan menambahkan nilai literal `1` dalam ComputeEssentialIngredientExpenses.
                        essentialIngredientExpenses += 1;
                        // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam ComputeEssentialIngredientExpenses.
                        continue;
                    // Menutup scope cabang if untuk kondisi `!purchaseCostByCardId.TryGetValue(requiredCard, out var queue) || queue.Count == 0`; bagian berikut berada
                    // di luar batas blok tersebut dalam ComputeEssentialIngredientExpenses.
                    }

                    // Memperbarui `essentialIngredientExpenses` dengan menambahkan memanggil `queue.Dequeue` dengan tanpa argumen dalam
                    // ComputeEssentialIngredientExpenses.
                    essentialIngredientExpenses += queue.Dequeue();
                // Menutup scope loop setiap requiredCard dari `requiredCards`; bagian berikut berada di luar batas blok tersebut dalam
                // ComputeEssentialIngredientExpenses.
                }
            // Menutup scope cabang if untuk kondisi `evt.ActionType == ”JualMasakan” && _payloadReader.TryReadOrderClaim(evt.Payload, out var requiredCards,
            // out _)`; bagian berikut berada di luar batas blok tersebut dalam ComputeEssentialIngredientExpenses.
            }
        // Menutup scope loop setiap evt dari `playerEvents.OrderBy(e => e.SequenceNumber)`; bagian berikut berada di luar batas blok tersebut dalam
        // ComputeEssentialIngredientExpenses.
        }

        // Mengembalikan `essentialIngredientExpenses` (nilai essential bahan expenses) kepada pemanggil dalam ComputeEssentialIngredientExpenses; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return essentialIngredientExpenses;
    // Menutup scope metode ComputeEssentialIngredientExpenses; bagian berikut berada di luar batas blok tersebut dalam
    // ComputeEssentialIngredientExpenses.
    }
// Menutup scope tipe IngredientMealCalculator; bagian berikut berada di luar batas blok tersebut.
}
