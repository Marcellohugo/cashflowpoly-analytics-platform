// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventIngredientOrderValidator.
// Mengimpor namespace `System.Linq` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Linq;
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `EventIngredientOrderValidation`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed record EventIngredientOrderValidation(
    // Parameter `Validation` bertipe `EventDomainValidationResult` membawa nilai validasi.
    EventDomainValidationResult Validation,
    // Parameter `OutgoingAmount` bertipe `int?` membawa nilai outgoing nominal; nilai null diizinkan ketika data opsional belum tersedia.
    int? OutgoingAmount);

// Mendefinisikan tipe class `EventIngredientOrderValidator` yang mewarisi atau menerapkan `IEventIngredientOrderValidator`; sealed mencegah tipe
// ini diturunkan lagi.
internal sealed class EventIngredientOrderValidator : IEventIngredientOrderValidator
// Membuka scope tipe EventIngredientOrderValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `EventPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly EventPayloadReader _payloadReader = new();
    // Mendeklarasikan field bertipe `EventDerivedStateCalculator`: `_derivedState` menyimpan nilai derived keadaan dengan nilai awal objek baru dengan
    // tipe mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static
    // membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly EventDerivedStateCalculator _derivedState = new();

    // Mendefinisikan metode `TryValidate` dengan hasil bertipe `bool`; operasi ini menangani try validate. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `config` bertipe `RulesetConfig`
    // membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe `IEnumerable<EventDb>` membawa
    // nilai history; Parameter `result` bertipe `EventIngredientOrderValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap
    // berikutnya; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryValidate(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history,
        // Parameter `result` bertipe `EventIngredientOrderValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out EventIngredientOrderValidation result)
    // Membuka scope metode TryValidate; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
    {
        // Memeriksa memanggil `GameActionCatalog.Is` dengan `request.ActionType`, `request.Payload`, `GameActionCatalog.BahanMasakan`; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.BahanMasakan))
        // Membuka scope cabang if untuk kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.BahanMasakan)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `ValidatePurchase` dengan `request`, `config`, `history` dalam TryValidate.
            result = ValidatePurchase(request, config, history);
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.BahanMasakan)`; bagian berikut
        // berada di luar batas blok tersebut dalam TryValidate.
        }

        // Memeriksa memanggil `GameActionCatalog.Is` dengan `request.ActionType`, `request.Payload`, `GameActionCatalog.IngredientDiscarded`; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.IngredientDiscarded))
        // Membuka scope cabang if untuk kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.IngredientDiscarded)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `ValidateDiscard` dengan `request`, `history` dalam TryValidate.
            result = ValidateDiscard(request, history);
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.IngredientDiscarded)`; bagian
        // berikut berada di luar batas blok tersebut dalam TryValidate.
        }

        // Memeriksa memanggil `GameActionCatalog.Is` dengan `request.ActionType`, `request.Payload`, `GameActionCatalog.JualMasakan`; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.JualMasakan))
        // Membuka scope cabang if untuk kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.JualMasakan)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `ValidateOrderClaim` dengan `request`, `config`, `history` dalam TryValidate.
            result = ValidateOrderClaim(request, config, history);
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.JualMasakan)`; bagian berikut
        // berada di luar batas blok tersebut dalam TryValidate.
        }

        // Memperbarui `result` menggunakan objek baru bertipe `EventIngredientOrderValidation` dengan argumen (EventDomainValidationResult.Valid, null)
        // dalam TryValidate.
        result = new EventIngredientOrderValidation(EventDomainValidationResult.Valid, null);
        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return false;
    // Menutup scope metode TryValidate; bagian berikut berada di luar batas blok tersebut dalam TryValidate.
    }

    // Mendefinisikan metode `ValidatePurchase` dengan hasil bertipe `EventIngredientOrderValidation`; operasi ini menangani validate pembelian.
    // Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter
    // `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe
    // `IEnumerable<EventDb>` membawa nilai history.
    private EventIngredientOrderValidation ValidatePurchase(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history)
    // Membuka scope metode ValidatePurchase; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidatePurchase.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadIngredientPurchase(request.Payload, out var cardId, out var amount)`; blok if hanya dijalankan
        // ketika kondisi ini bernilai benar dalam ValidatePurchase.
        if (!_payloadReader.TryReadIngredientPurchase(request.Payload, out var cardId, out var amount))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadIngredientPurchase(request.Payload, out var cardId, out var amount)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidatePurchase.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Payload ingredient tidak valid”`, `new
            // ErrorDetail(”payload.card_id”, ”REQUIRED”)` kepada pemanggil dalam ValidatePurchase; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Payload ingredient tidak valid”` sebagai argumen ke `Fail`.
                "Payload ingredient tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.card_id”, ”REQUIRED”) sebagai argumen ke `Fail`; Meneruskan nilai literal
                // `”payload.card_id”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke konstruktor
                // `ErrorDetail`.
                new ErrorDetail("payload.card_id", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadIngredientPurchase(request.Payload, out var cardId, out var amount)`; bagian
        // berikut berada di luar batas blok tersebut dalam ValidatePurchase.
        }

        // Menyiapkan variabel lokal `commonValidation` untuk nilai common validasi dengan memanggil `ValidatePositiveAmountAndPlayer` dengan `request`,
        // `amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var commonValidation = ValidatePositiveAmountAndPlayer(request, amount);
        // Memeriksa kebalikan kondisi `commonValidation.IsValid`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidatePurchase.
        if (!commonValidation.IsValid)
        // Membuka scope cabang if untuk kondisi `!commonValidation.IsValid`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidatePurchase.
        {
            // Mengembalikan objek baru bertipe `EventIngredientOrderValidation` dengan argumen (commonValidation, null) kepada pemanggil dalam
            // ValidatePurchase; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return new EventIngredientOrderValidation(commonValidation, null);
        // Menutup scope cabang if untuk kondisi `!commonValidation.IsValid`; bagian berikut berada di luar batas blok tersebut dalam ValidatePurchase.
        }

        // Menyiapkan variabel lokal `matchedIng` untuk nilai matched ing dengan mengambil elemen pertama `config.Ingredients` yang sesuai `i =>
        // string.Equals(i.Id, cardId, StringComparison.OrdinalIgnoreCase)`; jika tidak ada, gunakan nilai default tipe hasil. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var matchedIng = config.Ingredients.FirstOrDefault(i => string.Equals(i.Id, cardId, StringComparison.OrdinalIgnoreCase));
        // Memeriksa hasil pencocokan `matchedIng` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidatePurchase.
        if (matchedIng is null)
        // Membuka scope cabang if untuk kondisi `matchedIng is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidatePurchase.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `$”Bahan {cardId} tidak terdaftar
            // pada katalog ruleset aktif”` kepada pemanggil dalam ValidatePurchase; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke `Fail`.
                StatusCodes.Status422UnprocessableEntity,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `Fail`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan teks interpolasi `$”Bahan {cardId} tidak terdaftar pada katalog ruleset aktif”`; nilai ekspresi di dalam kurung kurawal disisipkan
                // saat program berjalan sebagai argumen ke `Fail`.
                $"Bahan {cardId} tidak terdaftar pada katalog ruleset aktif");
        // Menutup scope cabang if untuk kondisi `matchedIng is null`; bagian berikut berada di luar batas blok tersebut dalam ValidatePurchase.
        }

        // Menyiapkan variabel lokal `modifier` untuk nilai modifier dengan nilai literal `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var modifier = 0;
        // Mengulangi setiap elemen `history`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh badan loop dalam ValidatePurchase.
        foreach (var evt in history)
        // Membuka scope loop setiap evt dari `history`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidatePurchase.
        {
            // Menyiapkan variabel lokal `eventPayload` untuk nilai event payload dengan memanggil `_payloadReader.ReadPayload` dengan `evt.Payload`. Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var eventPayload = _payloadReader.ReadPayload(evt.Payload);
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `GameActionCatalog.Is(evt.ActionType, eventPayload,
            // GameActionCatalog.RisikoKehidupan)` dan `_payloadReader.TryGetString(eventPayload, ”risk_id”, out var riskId)`; sisi kanan diperiksa hanya jika
            // sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidatePurchase.
            if (GameActionCatalog.Is(evt.ActionType, eventPayload, GameActionCatalog.RisikoKehidupan) &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryGetString` dengan `eventPayload`, `”risk_id”`, `var riskId` dalam ValidatePurchase.
                _payloadReader.TryGetString(eventPayload, "risk_id", out var riskId))
            // Membuka scope cabang if untuk kondisi `GameActionCatalog.Is(evt.ActionType, eventPayload, GameActionCatalog.RisikoKehidupan) &&
            // _payloadReader.TryGetString(eventPayload, ”risk_id”, out var riskId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidatePurchase.
            {
                // Menyiapkan variabel lokal `risk` untuk nilai risiko dengan mengambil elemen pertama `config.LifeRisks` yang sesuai `r =>
                // string.Equals(r.RiskCode, riskId, StringComparison.OrdinalIgnoreCase)`; jika tidak ada, gunakan nilai default tipe hasil. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var risk = config.LifeRisks.FirstOrDefault(r => string.Equals(r.RiskCode, riskId, StringComparison.OrdinalIgnoreCase));
                // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `risk is not null` dan `string.Equals(risk.EffectType,
                // ”INGREDIENT_PRICE_MODIFIER”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan
                // ketika kondisi ini bernilai benar dalam ValidatePurchase.
                if (risk is not null && string.Equals(risk.EffectType, "INGREDIENT_PRICE_MODIFIER", StringComparison.OrdinalIgnoreCase))
                // Membuka scope cabang if untuk kondisi `risk is not null && string.Equals(risk.EffectType, ”INGREDIENT_PRICE_MODIFIER”,
                // StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidatePurchase.
                {
                    // Menyiapkan variabel lokal `startDay` untuk nilai start hari dengan `evt.DayIndex` (nilai hari index). Tipe variabel disimpulkan dari ekspresi
                    // nilai awal.
                    var startDay = evt.DayIndex;
                    // Menyiapkan variabel lokal `endDay` untuk nilai end hari dengan selisih antara `startDay + (risk.DurationDays ?? 1)` dan `1`. Tipe variabel
                    // disimpulkan dari ekspresi nilai awal.
                    var endDay = startDay + (risk.DurationDays ?? 1) - 1;
                    // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `request.DayIndex >= startDay` dan `request.DayIndex <= endDay`; sisi kanan
                    // diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidatePurchase.
                    if (request.DayIndex >= startDay && request.DayIndex <= endDay)
                    // Membuka scope cabang if untuk kondisi `request.DayIndex >= startDay && request.DayIndex <= endDay`; pernyataan/deklarasi berikut berada di dalam
                    // batas blok ini dalam ValidatePurchase.
                    {
                        // Memperbarui `modifier` dengan menambahkan hasil pemilihan bersyarat: ketika `string.Equals(risk.Direction, ”IN”,
                        // StringComparison.OrdinalIgnoreCase)` benar gunakan `risk.Amount`, jika tidak gunakan `-risk.Amount` dalam ValidatePurchase.
                        modifier += string.Equals(risk.Direction, "IN", StringComparison.OrdinalIgnoreCase) ? risk.Amount : -risk.Amount;
                    // Menutup scope cabang if untuk kondisi `request.DayIndex >= startDay && request.DayIndex <= endDay`; bagian berikut berada di luar batas blok
                    // tersebut dalam ValidatePurchase.
                    }
                // Menutup scope cabang if untuk kondisi `risk is not null && string.Equals(risk.EffectType, ”INGREDIENT_PRICE_MODIFIER”,
                // StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok tersebut dalam ValidatePurchase.
                }
            // Menutup scope cabang if untuk kondisi `GameActionCatalog.Is(evt.ActionType, eventPayload, GameActionCatalog.RisikoKehidupan) &&
            // _payloadReader.TryGetString(eventPayload, ”risk_id”, out var riskId)`; bagian berikut berada di luar batas blok tersebut dalam ValidatePurchase.
            }
        // Menutup scope loop setiap evt dari `history`; bagian berikut berada di luar batas blok tersebut dalam ValidatePurchase.
        }

        // Menyiapkan variabel lokal `expectedPrice` untuk nilai yang diharapkan harga dengan menentukan nilai terbesar dari `0`, `matchedIng.HargaBeli +
        // modifier`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var expectedPrice = Math.Max(0, matchedIng.HargaBeli + modifier);
        // Memeriksa perbandingan ketidaksamaan antara `amount` dan `expectedPrice`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidatePurchase.
        if (amount != expectedPrice)
        // Membuka scope cabang if untuk kondisi `amount != expectedPrice`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidatePurchase.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `$”Harga pembelian bahan {cardId}
            // ({amount}) tidak sesuai dengan harga katalog modified ({expectedPrice})”` kepada pemanggil dalam ValidatePurchase; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke `Fail`.
                StatusCodes.Status422UnprocessableEntity,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `Fail`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan teks interpolasi `$”Harga pembelian bahan {cardId} ({amount}) tidak sesuai dengan harga katalog modified ({expectedPrice})”`; nilai
                // ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `Fail`.
                $"Harga pembelian bahan {cardId} ({amount}) tidak sesuai dengan harga katalog modified ({expectedPrice})");
        // Menutup scope cabang if untuk kondisi `amount != expectedPrice`; bagian berikut berada di luar batas blok tersebut dalam ValidatePurchase.
        }

        // Menyiapkan variabel lokal `inventory` untuk nilai inventory dengan memanggil `_derivedState.BuildIngredientInventory` dengan `history`,
        // `request.UserId!.Value`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var inventory = _derivedState.BuildIngredientInventory(history, request.UserId!.Value);
        // Memeriksa pemeriksaan lebih besar antara `inventory.Total + 1` dan `config.MaxIngredientTotal`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ValidatePurchase.
        if (inventory.Total + 1 > config.MaxIngredientTotal)
        // Membuka scope cabang if untuk kondisi `inventory.Total + 1 > config.MaxIngredientTotal`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ValidatePurchase.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Total kartu bahan melebihi batas
            // ruleset”` kepada pemanggil dalam ValidatePurchase; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Total kartu bahan melebihi batas ruleset");
        // Menutup scope cabang if untuk kondisi `inventory.Total + 1 > config.MaxIngredientTotal`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidatePurchase.
        }

        // Menyiapkan variabel lokal `currentSame` untuk nilai saat ini same dengan hasil pemilihan bersyarat: ketika
        // `inventory.ByCardId.TryGetValue(cardId, out var currentQty)` benar gunakan `currentQty`, jika tidak gunakan `0`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var currentSame = inventory.ByCardId.TryGetValue(cardId, out var currentQty) ? currentQty : 0;
        // Memeriksa pemeriksaan lebih besar antara `currentSame + 1` dan `config.MaxSameIngredient`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam ValidatePurchase.
        if (currentSame + 1 > config.MaxSameIngredient)
        // Membuka scope cabang if untuk kondisi `currentSame + 1 > config.MaxSameIngredient`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ValidatePurchase.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Jumlah kartu bahan sejenis
            // melebihi batas ruleset”` kepada pemanggil dalam ValidatePurchase; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Jumlah kartu bahan sejenis melebihi batas ruleset");
        // Menutup scope cabang if untuk kondisi `currentSame + 1 > config.MaxSameIngredient`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidatePurchase.
        }

        // Mengembalikan objek baru bertipe `EventIngredientOrderValidation` dengan argumen (EventDomainValidationResult.Valid, amount) kepada pemanggil
        // dalam ValidatePurchase; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventIngredientOrderValidation(EventDomainValidationResult.Valid, amount);
    // Menutup scope metode ValidatePurchase; bagian berikut berada di luar batas blok tersebut dalam ValidatePurchase.
    }

    // Mendefinisikan metode `ValidateDiscard` dengan hasil bertipe `EventIngredientOrderValidation`; operasi ini menangani validate discard. Masukan:
    // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `history`
    // bertipe `IEnumerable<EventDb>` membawa nilai history.
    private EventIngredientOrderValidation ValidateDiscard(EventRequest request, IEnumerable<EventDb> history)
    // Membuka scope metode ValidateDiscard; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateDiscard.
    {
        // Memeriksa hasil pencocokan `request.UserId` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateDiscard.
        if (request.UserId is null)
        // Membuka scope cabang if untuk kondisi `request.UserId is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateDiscard.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Player wajib diisi”`, `new
            // ErrorDetail(”user_id”, ”REQUIRED”)` kepada pemanggil dalam ValidateDiscard; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Player wajib diisi”` sebagai argumen ke `Fail`.
                "Player wajib diisi",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”user_id”, ”REQUIRED”) sebagai argumen ke `Fail`; Meneruskan nilai literal
                // `”user_id”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke konstruktor `ErrorDetail`.
                new ErrorDetail("user_id", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `request.UserId is null`; bagian berikut berada di luar batas blok tersebut dalam ValidateDiscard.
        }

        // Memeriksa kebalikan kondisi `_payloadReader.TryReadIngredientPurchase(request.Payload, out var cardId, out var amount)`; blok if hanya dijalankan
        // ketika kondisi ini bernilai benar dalam ValidateDiscard.
        if (!_payloadReader.TryReadIngredientPurchase(request.Payload, out var cardId, out var amount))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadIngredientPurchase(request.Payload, out var cardId, out var amount)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateDiscard.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Payload discard ingredient tidak valid”`, `new
            // ErrorDetail(”payload.card_id”, ”REQUIRED”)` kepada pemanggil dalam ValidateDiscard; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Payload discard ingredient tidak valid”` sebagai argumen ke `Fail`.
                "Payload discard ingredient tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.card_id”, ”REQUIRED”) sebagai argumen ke `Fail`; Meneruskan nilai literal
                // `”payload.card_id”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke konstruktor
                // `ErrorDetail`.
                new ErrorDetail("payload.card_id", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadIngredientPurchase(request.Payload, out var cardId, out var amount)`; bagian
        // berikut berada di luar batas blok tersebut dalam ValidateDiscard.
        }

        // Menyiapkan variabel lokal `amountValidation` untuk nilai nominal validasi dengan memanggil `ValidatePositiveAmount` dengan `amount`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var amountValidation = ValidatePositiveAmount(amount);
        // Memeriksa kebalikan kondisi `amountValidation.IsValid`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateDiscard.
        if (!amountValidation.IsValid)
        // Membuka scope cabang if untuk kondisi `!amountValidation.IsValid`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateDiscard.
        {
            // Mengembalikan objek baru bertipe `EventIngredientOrderValidation` dengan argumen (amountValidation, null) kepada pemanggil dalam ValidateDiscard;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return new EventIngredientOrderValidation(amountValidation, null);
        // Menutup scope cabang if untuk kondisi `!amountValidation.IsValid`; bagian berikut berada di luar batas blok tersebut dalam ValidateDiscard.
        }

        // Menyiapkan variabel lokal `inventory` untuk nilai inventory dengan memanggil `_derivedState.BuildIngredientInventory` dengan `history`,
        // `request.UserId.Value`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var inventory = _derivedState.BuildIngredientInventory(history, request.UserId.Value);
        // Menyiapkan variabel lokal `currentQty` untuk nilai saat ini qty dengan hasil pemilihan bersyarat: ketika `inventory.ByCardId.TryGetValue(cardId,
        // out var qty)` benar gunakan `qty`, jika tidak gunakan `0`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var currentQty = inventory.ByCardId.TryGetValue(cardId, out var qty) ? qty : 0;
        // Memeriksa pemeriksaan lebih kecil antara `currentQty` dan `amount`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateDiscard.
        if (currentQty < amount)
        // Membuka scope cabang if untuk kondisi `currentQty < amount`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateDiscard.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Jumlah discard melebihi stok
            // bahan”` kepada pemanggil dalam ValidateDiscard; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Jumlah discard melebihi stok bahan");
        // Menutup scope cabang if untuk kondisi `currentQty < amount`; bagian berikut berada di luar batas blok tersebut dalam ValidateDiscard.
        }

        // Mengembalikan objek baru bertipe `EventIngredientOrderValidation` dengan argumen (EventDomainValidationResult.Valid, null) kepada pemanggil dalam
        // ValidateDiscard; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventIngredientOrderValidation(EventDomainValidationResult.Valid, null);
    // Menutup scope metode ValidateDiscard; bagian berikut berada di luar batas blok tersebut dalam ValidateDiscard.
    }

    // Mendefinisikan metode `ValidateOrderClaim` dengan hasil bertipe `EventIngredientOrderValidation`; operasi ini menangani validate urutan/pesanan
    // claim. Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan;
    // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history`
    // bertipe `IEnumerable<EventDb>` membawa nilai history.
    private EventIngredientOrderValidation ValidateOrderClaim(EventRequest request, RulesetConfig config, IEnumerable<EventDb> history)
    // Membuka scope metode ValidateOrderClaim; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateOrderClaim.
    {
        // Memeriksa hasil pencocokan `request.UserId` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateOrderClaim.
        if (request.UserId is null)
        // Membuka scope cabang if untuk kondisi `request.UserId is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateOrderClaim.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Player wajib diisi”`, `new
            // ErrorDetail(”user_id”, ”REQUIRED”)` kepada pemanggil dalam ValidateOrderClaim; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Player wajib diisi”` sebagai argumen ke `Fail`.
                "Player wajib diisi",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”user_id”, ”REQUIRED”) sebagai argumen ke `Fail`; Meneruskan nilai literal
                // `”user_id”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke konstruktor `ErrorDetail`.
                new ErrorDetail("user_id", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `request.UserId is null`; bagian berikut berada di luar batas blok tersebut dalam ValidateOrderClaim.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!request.Payload.TryGetProperty(”order_card_id”, out var
        // orderCardIdProp)` dan `orderCardIdProp.ValueKind != JsonValueKind.String`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam ValidateOrderClaim.
        if (!request.Payload.TryGetProperty("order_card_id", out var orderCardIdProp) ||
            // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `orderCardIdProp.ValueKind` dan `JsonValueKind.String` dalam ValidateOrderClaim.
            orderCardIdProp.ValueKind != JsonValueKind.String)
        // Membuka scope cabang if untuk kondisi `!request.Payload.TryGetProperty(”order_card_id”, out var orderCardIdProp) || orderCardIdProp.ValueKind !=
        // JsonValueKind.String`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateOrderClaim.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Payload order claim tidak valid”`, `new
            // ErrorDetail(”payload.order_card_id”, ”REQUIRED”)` kepada pemanggil dalam ValidateOrderClaim; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Payload order claim tidak valid”` sebagai argumen ke `Fail`.
                "Payload order claim tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.order_card_id”, ”REQUIRED”) sebagai argumen ke `Fail`; Meneruskan nilai
                // literal `”payload.order_card_id”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke
                // konstruktor `ErrorDetail`.
                new ErrorDetail("payload.order_card_id", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `!request.Payload.TryGetProperty(”order_card_id”, out var orderCardIdProp) || orderCardIdProp.ValueKind !=
        // JsonValueKind.String`; bagian berikut berada di luar batas blok tersebut dalam ValidateOrderClaim.
        }

        // Menyiapkan variabel lokal `orderCardId` untuk nilai urutan/pesanan kartu identitas dengan `orderCardIdProp.GetString()` dengan penegasan non-null
        // untuk analisis compiler; operator ! tidak menambah pemeriksaan saat runtime. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var orderCardId = orderCardIdProp.GetString()!;
        // Menyiapkan variabel lokal `order` untuk nilai urutan/pesanan dengan mengambil elemen pertama `config.Orders` yang sesuai `o =>
        // string.Equals(o.Id, orderCardId, StringComparison.OrdinalIgnoreCase)`; jika tidak ada, gunakan nilai default tipe hasil. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var order = config.Orders.FirstOrDefault(o => string.Equals(o.Id, orderCardId, StringComparison.OrdinalIgnoreCase));
        // Memeriksa hasil pencocokan `order` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateOrderClaim.
        if (order is null)
        // Membuka scope cabang if untuk kondisi `order is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateOrderClaim.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `$”Kartu pesanan {orderCardId} tidak
            // terdaftar pada katalog ruleset aktif”` kepada pemanggil dalam ValidateOrderClaim; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke `Fail`.
                StatusCodes.Status422UnprocessableEntity,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `Fail`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan teks interpolasi `$”Kartu pesanan {orderCardId} tidak terdaftar pada katalog ruleset aktif”`; nilai ekspresi di dalam kurung kurawal
                // disisipkan saat program berjalan sebagai argumen ke `Fail`.
                $"Kartu pesanan {orderCardId} tidak terdaftar pada katalog ruleset aktif");
        // Menutup scope cabang if untuk kondisi `order is null`; bagian berikut berada di luar batas blok tersebut dalam ValidateOrderClaim.
        }

        // Memeriksa `order.CardQty.HasValue`, yaitu penanda bahwa nilai nullable tidak kosong; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ValidateOrderClaim.
        if (order.CardQty.HasValue)
        // Membuka scope cabang if untuk kondisi `order.CardQty.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateOrderClaim.
        {
            // Menyiapkan variabel lokal `claimedCount` untuk nilai claimed jumlah dengan memanggil `history.Count` dengan `e => string.Equals(e.ActionType,
            // GameActionCatalog.JualMasakan, StringComparison.OrdinalIgnoreCase) && string.Equals(
            // _payloadReader.ReadPayload(e.Payload).TryGetProperty(”ord...`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var claimedCount = history.Count(e =>
                // Meneruskan `e.ActionType` (nilai aksi jenis) sebagai argumen ke `string.Equals`; Meneruskan `GameActionCatalog.JualMasakan` (nilai jual masakan)
                // sebagai argumen ke `string.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke
                // `string.Equals`.
                string.Equals(e.ActionType, GameActionCatalog.JualMasakan, StringComparison.OrdinalIgnoreCase) &&
                // Meneruskan fungsi lambda `e => string.Equals(e.ActionType, GameActionCatalog.JualMasakan, StringComparison.OrdinalIgnoreCase) && string.Equals(
                // _payloadReader.ReadPayload(e.Payload).TryGetProperty(”ord...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                // argumen ke `history.Count`.
                string.Equals(
                    // Meneruskan hasil pemilihan bersyarat: ketika `_payloadReader.ReadPayload(e.Payload).TryGetProperty(”order_card_id”, out var idProp) &&
                    // idProp.ValueKind == JsonValueKind.String` benar gunakan `idProp.GetString()`, jika tidak gunakan `null` sebagai argumen ke `string.Equals`;
                    // Meneruskan `e.Payload` (muatan detail event dalam format JSON) sebagai argumen ke `_payloadReader.ReadPayload`; Meneruskan nilai literal
                    // `”order_card_id”` sebagai argumen ke `_payloadReader.ReadPayload(e.Payload).TryGetProperty`; Meneruskan `var idProp` sebagai argumen ke
                    // `_payloadReader.ReadPayload(e.Payload).TryGetProperty`.
                    _payloadReader.ReadPayload(e.Payload).TryGetProperty("order_card_id", out var idProp) && idProp.ValueKind == JsonValueKind.String ? idProp.GetString() : null,
                    // Meneruskan `orderCardId` (nilai urutan/pesanan kartu identitas) sebagai argumen ke `string.Equals`.
                    orderCardId,
                    // Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `string.Equals`.
                    StringComparison.OrdinalIgnoreCase
                // Meneruskan fungsi lambda `e => string.Equals(e.ActionType, GameActionCatalog.JualMasakan, StringComparison.OrdinalIgnoreCase) && string.Equals(
                // _payloadReader.ReadPayload(e.Payload).TryGetProperty(”ord...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai
                // argumen ke `history.Count`.
                ));

            // Memeriksa pemeriksaan lebih besar atau sama antara `claimedCount` dan `order.CardQty.Value`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam ValidateOrderClaim.
            if (claimedCount >= order.CardQty.Value)
            // Membuka scope cabang if untuk kondisi `claimedCount >= order.CardQty.Value`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateOrderClaim.
            {
                // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `$”Kartu pesanan {orderCardId} telah
                // mencapai batas kuantitas fisik ({order.CardQty.Value})”` kepada pemanggil dalam ValidateOrderClaim; eksekusi jalur ini selesai setelah nilai
                // hasil ditentukan.
                return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", $"Kartu pesanan {orderCardId} telah mencapai batas kuantitas fisik ({order.CardQty.Value})");
            // Menutup scope cabang if untuk kondisi `claimedCount >= order.CardQty.Value`; bagian berikut berada di luar batas blok tersebut dalam
            // ValidateOrderClaim.
            }
        // Menutup scope cabang if untuk kondisi `order.CardQty.HasValue`; bagian berikut berada di luar batas blok tersebut dalam ValidateOrderClaim.
        }

        // Menyiapkan variabel lokal `requiredCards` untuk nilai required kartu dengan objek baru bertipe `List<string>` dengan nilai awal sesuai
        // konstruktornya. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var requiredCards = new List<string>();
        // Mengulangi setiap elemen `order.Bahan`; elemen saat ini disimpan sebagai `bahanName` bertipe `var` untuk diproses oleh badan loop dalam
        // ValidateOrderClaim.
        foreach (var bahanName in order.Bahan)
        // Membuka scope loop setiap bahanName dari `order.Bahan`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateOrderClaim.
        {
            // Menyiapkan variabel lokal `matchedIng` untuk nilai matched ing dengan mengambil elemen pertama `config.Ingredients` yang sesuai `i =>
            // string.Equals(i.Nama, bahanName, StringComparison.OrdinalIgnoreCase)`; jika tidak ada, gunakan nilai default tipe hasil. Tipe variabel
            // disimpulkan dari ekspresi nilai awal.
            var matchedIng = config.Ingredients.FirstOrDefault(i => string.Equals(i.Nama, bahanName, StringComparison.OrdinalIgnoreCase));
            // Menyiapkan variabel lokal `cardId` untuk nilai kartu identitas dengan `matchedIng?.Id` bila tidak null; jika null gunakan
            // `bahanName.ToLowerInvariant().Replace(” ”, ”_”)` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var cardId = matchedIng?.Id ?? bahanName.ToLowerInvariant().Replace(" ", "_");
            // Menjalankan menambahkan `cardId` ke `requiredCards` dalam ValidateOrderClaim.
            requiredCards.Add(cardId);
        // Menutup scope loop setiap bahanName dari `order.Bahan`; bagian berikut berada di luar batas blok tersebut dalam ValidateOrderClaim.
        }

        // Menyiapkan variabel lokal `inventory` untuk nilai inventory dengan memanggil `_derivedState.BuildIngredientInventory` dengan `history`,
        // `request.UserId.Value`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var inventory = _derivedState.BuildIngredientInventory(history, request.UserId.Value);
        // Mengulangi setiap elemen `requiredCards`; elemen saat ini disimpan sebagai `card` bertipe `var` untuk diproses oleh badan loop dalam
        // ValidateOrderClaim.
        foreach (var card in requiredCards)
        // Membuka scope loop setiap card dari `requiredCards`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateOrderClaim.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!inventory.ByCardId.TryGetValue(card, out var qty)` dan `qty <= 0`;
            // sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateOrderClaim.
            if (!inventory.ByCardId.TryGetValue(card, out var qty) || qty <= 0)
            // Membuka scope cabang if untuk kondisi `!inventory.ByCardId.TryGetValue(card, out var qty) || qty <= 0`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam ValidateOrderClaim.
            {
                // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Bahan tidak mencukupi untuk klaim
                // order”` kepada pemanggil dalam ValidateOrderClaim; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Bahan tidak mencukupi untuk klaim order");
            // Menutup scope cabang if untuk kondisi `!inventory.ByCardId.TryGetValue(card, out var qty) || qty <= 0`; bagian berikut berada di luar batas blok
            // tersebut dalam ValidateOrderClaim.
            }

            // Memperbarui `inventory.ByCardId[card]` menggunakan selisih antara `qty` dan `1` dalam ValidateOrderClaim.
            inventory.ByCardId[card] = qty - 1;
        // Menutup scope loop setiap card dari `requiredCards`; bagian berikut berada di luar batas blok tersebut dalam ValidateOrderClaim.
        }

        // Mengembalikan objek baru bertipe `EventIngredientOrderValidation` dengan argumen (EventDomainValidationResult.Valid, null) kepada pemanggil dalam
        // ValidateOrderClaim; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventIngredientOrderValidation(EventDomainValidationResult.Valid, null);
    // Menutup scope metode ValidateOrderClaim; bagian berikut berada di luar batas blok tersebut dalam ValidateOrderClaim.
    }

    // Mendefinisikan metode `ValidatePositiveAmountAndPlayer` dengan hasil bertipe `EventDomainValidationResult`; operasi ini menangani validate
    // positive nominal dan pemain. Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau
    // diteruskan ke layanan; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi.
    private EventDomainValidationResult ValidatePositiveAmountAndPlayer(EventRequest request, int amount)
    // Membuka scope metode ValidatePositiveAmountAndPlayer; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ValidatePositiveAmountAndPlayer.
    {
        // Menyiapkan variabel lokal `amountValidation` untuk nilai nominal validasi dengan memanggil `ValidatePositiveAmount` dengan `amount`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var amountValidation = ValidatePositiveAmount(amount);
        // Memeriksa kebalikan kondisi `amountValidation.IsValid`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidatePositiveAmountAndPlayer.
        if (!amountValidation.IsValid)
        // Membuka scope cabang if untuk kondisi `!amountValidation.IsValid`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidatePositiveAmountAndPlayer.
        {
            // Mengembalikan `amountValidation` (nilai nominal validasi) kepada pemanggil dalam ValidatePositiveAmountAndPlayer; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return amountValidation;
        // Menutup scope cabang if untuk kondisi `!amountValidation.IsValid`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidatePositiveAmountAndPlayer.
        }

        // Memeriksa hasil pencocokan `request.UserId` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidatePositiveAmountAndPlayer.
        if (request.UserId is null)
        // Membuka scope cabang if untuk kondisi `request.UserId is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidatePositiveAmountAndPlayer.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Player wajib
            // diisi”`, `new ErrorDetail(”user_id”, ”REQUIRED”)` kepada pemanggil dalam ValidatePositiveAmountAndPlayer; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Player wajib diisi”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Player wajib diisi",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”user_id”, ”REQUIRED”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”user_id”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke
                // konstruktor `ErrorDetail`.
                new ErrorDetail("user_id", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `request.UserId is null`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidatePositiveAmountAndPlayer.
        }

        // Mengembalikan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) kepada pemanggil dalam
        // ValidatePositiveAmountAndPlayer; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return EventDomainValidationResult.Valid;
    // Menutup scope metode ValidatePositiveAmountAndPlayer; bagian berikut berada di luar batas blok tersebut dalam ValidatePositiveAmountAndPlayer.
    }

    // Mendefinisikan metode `ValidatePositiveAmount` dengan hasil bertipe `EventDomainValidationResult`; operasi ini menangani validate positive
    // nominal. Masukan: Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi.
    private EventDomainValidationResult ValidatePositiveAmount(int amount)
    // Membuka scope metode ValidatePositiveAmount; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidatePositiveAmount.
    {
        // Memeriksa pemeriksaan lebih besar antara `amount` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidatePositiveAmount.
        if (amount > 0)
        // Membuka scope cabang if untuk kondisi `amount > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidatePositiveAmount.
        {
            // Mengembalikan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) kepada pemanggil dalam ValidatePositiveAmount;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Valid;
        // Menutup scope cabang if untuk kondisi `amount > 0`; bagian berikut berada di luar batas blok tersebut dalam ValidatePositiveAmount.
        }

        // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Amount harus > 0”`,
        // `new ErrorDetail(”payload.amount”, ”OUT_OF_RANGE”)` kepada pemanggil dalam ValidatePositiveAmount; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return EventDomainValidationResult.Fail(
            // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
            StatusCodes.Status400BadRequest,
            // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
            "VALIDATION_ERROR",
            // Meneruskan nilai literal `”Amount harus > 0”` sebagai argumen ke `EventDomainValidationResult.Fail`.
            "Amount harus > 0",
            // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.amount”, ”OUT_OF_RANGE”) sebagai argumen ke
            // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”payload.amount”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai
            // literal `”OUT_OF_RANGE”` sebagai argumen ke konstruktor `ErrorDetail`.
            new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
    // Menutup scope metode ValidatePositiveAmount; bagian berikut berada di luar batas blok tersebut dalam ValidatePositiveAmount.
    }

    // Mendefinisikan metode `Fail` dengan hasil bertipe `EventIngredientOrderValidation`; operasi ini menangani fail. Masukan: Parameter `statusCode`
    // bertipe `int` membawa kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan; Parameter `errorCode` bertipe `string` membawa
    // nilai kesalahan kode; Parameter `message` bertipe `string` membawa nilai pesan; Parameter `details` bertipe `ErrorDetail[]` membawa nilai
    // rincian.
    private EventIngredientOrderValidation Fail(
        // Parameter `statusCode` bertipe `int` membawa kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan.
        int statusCode,
        // Parameter `errorCode` bertipe `string` membawa nilai kesalahan kode.
        string errorCode,
        // Parameter `message` bertipe `string` membawa nilai pesan.
        string message,
        // Parameter `details` bertipe `ErrorDetail[]` membawa nilai rincian.
        params ErrorDetail[] details)
    // Membuka scope metode Fail; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Fail.
    {
        // Mengembalikan objek baru bertipe `EventIngredientOrderValidation` dengan argumen ( EventDomainValidationResult.Fail(statusCode, errorCode,
        // message, details), null) kepada pemanggil dalam Fail; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventIngredientOrderValidation(
            // Meneruskan memanggil `EventDomainValidationResult.Fail` dengan `statusCode`, `errorCode`, `message`, `details` sebagai argumen ke konstruktor
            // `EventIngredientOrderValidation`; Meneruskan `statusCode` (kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan) sebagai
            // argumen ke `EventDomainValidationResult.Fail`; Meneruskan `errorCode` (nilai kesalahan kode) sebagai argumen ke
            // `EventDomainValidationResult.Fail`; Meneruskan `message` (nilai pesan) sebagai argumen ke `EventDomainValidationResult.Fail`; Meneruskan
            // `details` (nilai rincian) sebagai argumen ke `EventDomainValidationResult.Fail`.
            EventDomainValidationResult.Fail(statusCode, errorCode, message, details),
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke konstruktor `EventIngredientOrderValidation`.
            null);
    // Menutup scope metode Fail; bagian berikut berada di luar batas blok tersebut dalam Fail.
    }
// Menutup scope tipe EventIngredientOrderValidator; bagian berikut berada di luar batas blok tersebut.
}
