// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventEconomyActionValidator.
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

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `EventEconomyActionValidation`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed record EventEconomyActionValidation(
    // Parameter `Validation` bertipe `EventDomainValidationResult` membawa nilai validasi.
    EventDomainValidationResult Validation,
    // Parameter `OutgoingAmount` bertipe `double?` membawa nilai outgoing nominal; nilai null diizinkan ketika data opsional belum tersedia.
    double? OutgoingAmount);

// Mendefinisikan tipe class `EventEconomyActionValidator` yang mewarisi atau menerapkan `IEventEconomyActionValidator`; sealed mencegah tipe ini
// diturunkan lagi.
internal sealed class EventEconomyActionValidator : IEventEconomyActionValidator
// Membuka scope tipe EventEconomyActionValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `EventPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly EventPayloadReader _payloadReader = new();
    // Mendeklarasikan field bertipe `int`: `RulebookGoldCardSupply` menyimpan nilai rulebook emas kartu supply dengan nilai awal nilai literal `20`.
    private const int RulebookGoldCardSupply = 20;

    // Mendefinisikan metode `TryValidate` dengan hasil bertipe `bool`; operasi ini menangani try validate. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `config` bertipe `RulesetConfig`
    // membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe `IEnumerable<EventDb>` membawa
    // nilai history; Parameter `result` bertipe `EventEconomyActionValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryValidate(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history,
        // Parameter `result` bertipe `EventEconomyActionValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out EventEconomyActionValidation result)
    // Membuka scope metode TryValidate; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
    {
        // Memeriksa membandingkan kesamaan `string` dengan `request.ActionType`, `”CatatTransaksi”`, `StringComparison.OrdinalIgnoreCase`; aturan
        // perbandingan mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (string.Equals(request.ActionType, "CatatTransaksi", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(request.ActionType, ”CatatTransaksi”, StringComparison.OrdinalIgnoreCase)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `ValidateTransaction` dengan `request` dalam TryValidate.
            result = ValidateTransaction(request);
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `string.Equals(request.ActionType, ”CatatTransaksi”, StringComparison.OrdinalIgnoreCase)`; bagian berikut
        // berada di luar batas blok tersebut dalam TryValidate.
        }

        // Memeriksa memanggil `GameActionCatalog.Is` dengan `request.ActionType`, `request.Payload`, `GameActionCatalog.JumatBerkah`; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.JumatBerkah))
        // Membuka scope cabang if untuk kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.JumatBerkah)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `ValidateFridayDonation` dengan `request`, `config`, `history` dalam TryValidate.
            result = ValidateFridayDonation(request, config, history);
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.JumatBerkah)`; bagian berikut
        // berada di luar batas blok tersebut dalam TryValidate.
        }

        // Memeriksa memanggil `GameActionCatalog.Is` dengan `request.ActionType`, `request.Payload`, `GameActionCatalog.GoldPriceOpened`; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.GoldPriceOpened))
        // Membuka scope cabang if untuk kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.GoldPriceOpened)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `ValidateGoldPrice` dengan `request`, `config`, `history` dalam TryValidate.
            result = ValidateGoldPrice(request, config, history);
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.GoldPriceOpened)`; bagian
        // berikut berada di luar batas blok tersebut dalam TryValidate.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `GameActionCatalog.Is(request.ActionType, request.Payload,
        // GameActionCatalog.InvestasiEmas)` dan `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.JualEmas)`; sisi kanan
        // diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.InvestasiEmas) ||
            // Melanjutkan pengolahan dengan memanggil `GameActionCatalog.Is` dengan `request.ActionType`, `request.Payload`, `GameActionCatalog.JualEmas` dalam
            // TryValidate.
            GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.JualEmas))
        // Membuka scope cabang if untuk kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.InvestasiEmas) ||
        // GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.JualE...`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `ValidateGoldTrade` dengan `request`, `config`, `history` dalam TryValidate.
            result = ValidateGoldTrade(request, config, history);
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.InvestasiEmas) ||
        // GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.JualE...`; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidate.
        }

        // Memperbarui `result` menggunakan objek baru bertipe `EventEconomyActionValidation` dengan argumen (EventDomainValidationResult.Valid, null) dalam
        // TryValidate.
        result = new EventEconomyActionValidation(EventDomainValidationResult.Valid, null);
        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return false;
    // Menutup scope metode TryValidate; bagian berikut berada di luar batas blok tersebut dalam TryValidate.
    }

    // Mendefinisikan metode `ValidateGoldPrice` dengan hasil bertipe `EventEconomyActionValidation`; operasi ini menangani validate emas harga.
    // Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter
    // `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe
    // `IEnumerable<EventDb>` membawa nilai history.
    private EventEconomyActionValidation ValidateGoldPrice(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history)
    // Membuka scope metode ValidateGoldPrice; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldPrice.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!_payloadReader.TryGetInt32(request.Payload, ”gold_price”, out var
        // price)` dan `price <= 0`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateGoldPrice.
        if (!_payloadReader.TryGetInt32(request.Payload, "gold_price", out var price) || price <= 0)
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryGetInt32(request.Payload, ”gold_price”, out var price) || price <= 0`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldPrice.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Harga emas tidak valid”`, `new
            // ErrorDetail(”payload.gold_price”, ”OUT_OF_RANGE”)` kepada pemanggil dalam ValidateGoldPrice; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Fail(StatusCodes.Status400BadRequest, "VALIDATION_ERROR", "Harga emas tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.gold_price”, ”OUT_OF_RANGE”) sebagai argumen ke `Fail`; Meneruskan nilai
                // literal `”payload.gold_price”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”OUT_OF_RANGE”` sebagai argumen ke
                // konstruktor `ErrorDetail`.
                new ErrorDetail("payload.gold_price", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryGetInt32(request.Payload, ”gold_price”, out var price) || price <= 0`; bagian berikut
        // berada di luar batas blok tersebut dalam ValidateGoldPrice.
        }

        // Memeriksa memeriksa apakah seluruh elemen `config.GoldPrices` memenuhi `item => item.UnitPrice != price`; koleksi kosong menghasilkan true; blok
        // if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoldPrice.
        if (config.GoldPrices.All(item => item.UnitPrice != price))
        // Membuka scope cabang if untuk kondisi `config.GoldPrices.All(item => item.UnitPrice != price)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam ValidateGoldPrice.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Harga emas harus berasal dari
            // Kartu Harga Emas ruleset”` kepada pemanggil dalam ValidateGoldPrice; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Harga emas harus berasal dari Kartu Harga Emas ruleset”` sebagai argumen ke `Fail`.
                "Harga emas harus berasal dari Kartu Harga Emas ruleset");
        // Menutup scope cabang if untuk kondisi `config.GoldPrices.All(item => item.UnitPrice != price)`; bagian berikut berada di luar batas blok tersebut
        // dalam ValidateGoldPrice.
        }

        // Menyiapkan variabel lokal `events` untuk kumpulan event permainan sebagai sumber riwayat untuk validasi atau perhitungan dengan mematerialisasi
        // urutan `history` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var events = history.ToList();
        // Memeriksa memeriksa apakah `events` memiliki setidaknya satu elemen yang memenuhi `e => e.DayIndex == request.DayIndex &&
        // GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.GoldPriceOpened)`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam ValidateGoldPrice.
        if (events.Any(e => e.DayIndex == request.DayIndex &&
            // Meneruskan `e.ActionType` (nilai aksi jenis) sebagai argumen ke `GameActionCatalog.Is`; Meneruskan memanggil `_payloadReader.ReadPayload` dengan
            // `e.Payload` sebagai argumen ke `GameActionCatalog.Is`; Meneruskan `e.Payload` (muatan detail event dalam format JSON) sebagai argumen ke
            // `_payloadReader.ReadPayload`; Meneruskan `GameActionCatalog.GoldPriceOpened` (nilai emas harga opened) sebagai argumen ke `GameActionCatalog.Is`.
            GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.GoldPriceOpened)))
        // Membuka scope cabang if untuk kondisi `events.Any(e => e.DayIndex == request.DayIndex && GameActionCatalog.Is(e.ActionType,
        // _payloadReader.ReadPayload(e.Payload), GameActionCatalog.GoldPriceOpened))`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateGoldPrice.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Harga emas hari ini sudah dibuka”`
            // kepada pemanggil dalam ValidateGoldPrice; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Harga emas hari ini sudah dibuka");
        // Menutup scope cabang if untuk kondisi `events.Any(e => e.DayIndex == request.DayIndex && GameActionCatalog.Is(e.ActionType,
        // _payloadReader.ReadPayload(e.Payload), GameActionCatalog.GoldPriceOpened))`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateGoldPrice.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!request.Weekday.Equals(”SAT”, StringComparison.OrdinalIgnoreCase)` dan
        // `!HasActiveGoldRiskOnDay(request.DayIndex, config, events)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam ValidateGoldPrice.
        if (!request.Weekday.Equals("SAT", StringComparison.OrdinalIgnoreCase) &&
            // Menggunakan kebalikan kondisi `HasActiveGoldRiskOnDay(request.DayIndex, config, events)` sebagai bagian ekspresi yang sedang disusun dalam
            // ValidateGoldPrice.
            !HasActiveGoldRiskOnDay(request.DayIndex, config, events))
        // Membuka scope cabang if untuk kondisi `!request.Weekday.Equals(”SAT”, StringComparison.OrdinalIgnoreCase) &&
        // !HasActiveGoldRiskOnDay(request.DayIndex, config, events)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldPrice.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Harga emas hanya dibuka pada Sabtu
            // atau saat efek Risiko Kehidupan emas aktif”` kepada pemanggil dalam ValidateGoldPrice; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Harga emas hanya dibuka pada Sabtu atau saat efek Risiko Kehidupan emas aktif”` sebagai argumen ke `Fail`.
                "Harga emas hanya dibuka pada Sabtu atau saat efek Risiko Kehidupan emas aktif");
        // Menutup scope cabang if untuk kondisi `!request.Weekday.Equals(”SAT”, StringComparison.OrdinalIgnoreCase) &&
        // !HasActiveGoldRiskOnDay(request.DayIndex, config, events)`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldPrice.
        }

        // Mengembalikan objek baru bertipe `EventEconomyActionValidation` dengan argumen (EventDomainValidationResult.Valid, null) kepada pemanggil dalam
        // ValidateGoldPrice; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventEconomyActionValidation(EventDomainValidationResult.Valid, null);
    // Menutup scope metode ValidateGoldPrice; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldPrice.
    }

    // Mendefinisikan metode `ValidateTransaction` dengan hasil bertipe `EventEconomyActionValidation`; operasi ini menangani validate transaction.
    // Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
    private EventEconomyActionValidation ValidateTransaction(EventRequest request)
    // Membuka scope metode ValidateTransaction; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateTransaction.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadTransaction(request.Payload, out var direction, out var amount, out var category, out var
        // counterparty)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateTransaction.
        if (!_payloadReader.TryReadTransaction(request.Payload, out var direction, out var amount, out var category, out var counterparty))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadTransaction(request.Payload, out var direction, out var amount, out var category,
        // out var counterparty)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateTransaction.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Payload transaksi tidak valid”`, `new
            // ErrorDetail(”payload”, ”INVALID_STRUCTURE”)` kepada pemanggil dalam ValidateTransaction; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Payload transaksi tidak valid”` sebagai argumen ke `Fail`.
                "Payload transaksi tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload”, ”INVALID_STRUCTURE”) sebagai argumen ke `Fail`; Meneruskan nilai literal
                // `”payload”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”INVALID_STRUCTURE”` sebagai argumen ke konstruktor
                // `ErrorDetail`.
                new ErrorDetail("payload", "INVALID_STRUCTURE"));
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadTransaction(request.Payload, out var direction, out var amount, out var category,
        // out var counterparty)`; bagian berikut berada di luar batas blok tersebut dalam ValidateTransaction.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!string.Equals(direction, ”IN”, StringComparison.OrdinalIgnoreCase)` dan
        // `!string.Equals(direction, ”OUT”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan
        // ketika kondisi ini bernilai benar dalam ValidateTransaction.
        if (!string.Equals(direction, "IN", StringComparison.OrdinalIgnoreCase) &&
            // Menggunakan kebalikan kondisi `string.Equals(direction, ”OUT”, StringComparison.OrdinalIgnoreCase)` sebagai bagian ekspresi yang sedang disusun
            // dalam ValidateTransaction.
            !string.Equals(direction, "OUT", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `!string.Equals(direction, ”IN”, StringComparison.OrdinalIgnoreCase) && !string.Equals(direction, ”OUT”,
        // StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateTransaction.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Direction tidak valid”`, `new
            // ErrorDetail(”payload.direction”, ”INVALID_ENUM”)` kepada pemanggil dalam ValidateTransaction; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Direction tidak valid”` sebagai argumen ke `Fail`.
                "Direction tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.direction”, ”INVALID_ENUM”) sebagai argumen ke `Fail`; Meneruskan nilai
                // literal `”payload.direction”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”INVALID_ENUM”` sebagai argumen ke
                // konstruktor `ErrorDetail`.
                new ErrorDetail("payload.direction", "INVALID_ENUM"));
        // Menutup scope cabang if untuk kondisi `!string.Equals(direction, ”IN”, StringComparison.OrdinalIgnoreCase) && !string.Equals(direction, ”OUT”,
        // StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok tersebut dalam ValidateTransaction.
        }

        // Memeriksa pemeriksaan lebih kecil atau sama antara `amount` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateTransaction.
        if (amount <= 0)
        // Membuka scope cabang if untuk kondisi `amount <= 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateTransaction.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Amount harus > 0”`, `new
            // ErrorDetail(”payload.amount”, ”OUT_OF_RANGE”)` kepada pemanggil dalam ValidateTransaction; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Amount harus > 0”` sebagai argumen ke `Fail`.
                "Amount harus > 0",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.amount”, ”OUT_OF_RANGE”) sebagai argumen ke `Fail`; Meneruskan nilai literal
                // `”payload.amount”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”OUT_OF_RANGE”` sebagai argumen ke konstruktor
                // `ErrorDetail`.
                new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `amount <= 0`; bagian berikut berada di luar batas blok tersebut dalam ValidateTransaction.
        }

        // Memeriksa memeriksa apakah `category` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ValidateTransaction.
        if (string.IsNullOrWhiteSpace(category))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(category)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateTransaction.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Category wajib diisi”`, `new
            // ErrorDetail(”payload.category”, ”REQUIRED”)` kepada pemanggil dalam ValidateTransaction; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Category wajib diisi”` sebagai argumen ke `Fail`.
                "Category wajib diisi",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.category”, ”REQUIRED”) sebagai argumen ke `Fail`; Meneruskan nilai literal
                // `”payload.category”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke konstruktor
                // `ErrorDetail`.
                new ErrorDetail("payload.category", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(category)`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateTransaction.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!string.IsNullOrWhiteSpace(counterparty) && !string.Equals(counterparty, ”BANK”,
        // StringComparison.OrdinalIgnoreCase)` dan `!string.Equals(counterparty, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya
        // jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateTransaction.
        if (!string.IsNullOrWhiteSpace(counterparty) &&
            // Menggunakan kebalikan kondisi `string.Equals(counterparty, ”BANK”, StringComparison.OrdinalIgnoreCase)` sebagai bagian ekspresi yang sedang
            // disusun dalam ValidateTransaction.
            !string.Equals(counterparty, "BANK", StringComparison.OrdinalIgnoreCase) &&
            // Menggunakan kebalikan kondisi `string.Equals(counterparty, ”PLAYER”, StringComparison.OrdinalIgnoreCase)` sebagai bagian ekspresi yang sedang
            // disusun dalam ValidateTransaction.
            !string.Equals(counterparty, "PLAYER", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(counterparty) && !string.Equals(counterparty, ”BANK”,
        // StringComparison.OrdinalIgnoreCase) && !string.Equals(counterparty, ”PLAYER”, StringComparison...`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam ValidateTransaction.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Counterparty tidak valid”`, `new
            // ErrorDetail(”payload.counterparty”, ”INVALID_ENUM”)` kepada pemanggil dalam ValidateTransaction; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Counterparty tidak valid”` sebagai argumen ke `Fail`.
                "Counterparty tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.counterparty”, ”INVALID_ENUM”) sebagai argumen ke `Fail`; Meneruskan nilai
                // literal `”payload.counterparty”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”INVALID_ENUM”` sebagai argumen ke
                // konstruktor `ErrorDetail`.
                new ErrorDetail("payload.counterparty", "INVALID_ENUM"));
        // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(counterparty) && !string.Equals(counterparty, ”BANK”,
        // StringComparison.OrdinalIgnoreCase) && !string.Equals(counterparty, ”PLAYER”, StringComparison...`; bagian berikut berada di luar batas blok
        // tersebut dalam ValidateTransaction.
        }

        // Menyiapkan variabel lokal `outgoing` untuk nilai outgoing dengan hasil pemilihan bersyarat: ketika `string.Equals(direction, ”OUT”,
        // StringComparison.OrdinalIgnoreCase) && request.UserId is not null` benar gunakan `amount`, jika tidak gunakan `(double?)null`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var outgoing = string.Equals(direction, "OUT", StringComparison.OrdinalIgnoreCase) && request.UserId is not null
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: amount dalam ValidateTransaction.
            ? amount
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (double?)null; dalam ValidateTransaction.
            : (double?)null;
        // Mengembalikan objek baru bertipe `EventEconomyActionValidation` dengan argumen (EventDomainValidationResult.Valid, outgoing) kepada pemanggil
        // dalam ValidateTransaction; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventEconomyActionValidation(EventDomainValidationResult.Valid, outgoing);
    // Menutup scope metode ValidateTransaction; bagian berikut berada di luar batas blok tersebut dalam ValidateTransaction.
    }

    // Mendefinisikan metode `ValidateFridayDonation` dengan hasil bertipe `EventEconomyActionValidation`; operasi ini menangani validate friday donasi.
    // Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter
    // `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe
    // `IEnumerable<EventDb>` membawa nilai history.
    private EventEconomyActionValidation ValidateFridayDonation(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history)
    // Membuka scope metode ValidateFridayDonation; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateFridayDonation.
    {
        // Memeriksa kebalikan kondisi `config.FridayEnabled`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateFridayDonation.
        if (!config.FridayEnabled)
        // Membuka scope cabang if untuk kondisi `!config.FridayEnabled`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateFridayDonation.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Fitur donasi Jumat tidak aktif”`
            // kepada pemanggil dalam ValidateFridayDonation; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur donasi Jumat tidak aktif");
        // Menutup scope cabang if untuk kondisi `!config.FridayEnabled`; bagian berikut berada di luar batas blok tersebut dalam ValidateFridayDonation.
        }

        // Memeriksa kebalikan kondisi `string.Equals(request.Weekday, ”FRI”, StringComparison.OrdinalIgnoreCase)`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ValidateFridayDonation.
        if (!string.Equals(request.Weekday, "FRI", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `!string.Equals(request.Weekday, ”FRI”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam ValidateFridayDonation.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Weekday harus FRI”`, `new ErrorDetail(”weekday”,
            // ”INVALID_VALUE”)` kepada pemanggil dalam ValidateFridayDonation; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Weekday harus FRI”` sebagai argumen ke `Fail`.
                "Weekday harus FRI",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”weekday”, ”INVALID_VALUE”) sebagai argumen ke `Fail`; Meneruskan nilai literal
                // `”weekday”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”INVALID_VALUE”` sebagai argumen ke konstruktor
                // `ErrorDetail`.
                new ErrorDetail("weekday", "INVALID_VALUE"));
        // Menutup scope cabang if untuk kondisi `!string.Equals(request.Weekday, ”FRI”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar
        // batas blok tersebut dalam ValidateFridayDonation.
        }

        // Memeriksa kebalikan kondisi `_payloadReader.TryReadAmount(request.Payload, out var amount)`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam ValidateFridayDonation.
        if (!_payloadReader.TryReadAmount(request.Payload, out var amount))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadAmount(request.Payload, out var amount)`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam ValidateFridayDonation.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Payload donasi tidak valid”`, `new
            // ErrorDetail(”payload.amount”, ”REQUIRED”)` kepada pemanggil dalam ValidateFridayDonation; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Payload donasi tidak valid”` sebagai argumen ke `Fail`.
                "Payload donasi tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.amount”, ”REQUIRED”) sebagai argumen ke `Fail`; Meneruskan nilai literal
                // `”payload.amount”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke konstruktor
                // `ErrorDetail`.
                new ErrorDetail("payload.amount", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadAmount(request.Payload, out var amount)`; bagian berikut berada di luar batas blok
        // tersebut dalam ValidateFridayDonation.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `amount < config.DonationMin` dan `amount > config.DonationMax`; sisi
        // kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateFridayDonation.
        if (amount < config.DonationMin || amount > config.DonationMax)
        // Membuka scope cabang if untuk kondisi `amount < config.DonationMin || amount > config.DonationMax`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam ValidateFridayDonation.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Jumlah donasi di luar batas”`
            // kepada pemanggil dalam ValidateFridayDonation; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Jumlah donasi di luar batas");
        // Menutup scope cabang if untuk kondisi `amount < config.DonationMin || amount > config.DonationMax`; bagian berikut berada di luar batas blok
        // tersebut dalam ValidateFridayDonation.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `request.UserId.HasValue` dan `history.Any(e => e.UserId == request.UserId &&
        // e.DayIndex == request.DayIndex && GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.Ju...`; sisi kanan
        // diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateFridayDonation.
        if (request.UserId.HasValue && history.Any(e =>
                // Meneruskan fungsi lambda `e => e.UserId == request.UserId && e.DayIndex == request.DayIndex && GameActionCatalog.Is(e.ActionType,
                // _payloadReader.ReadPayload(e.Payload), GameActionCatalog.JumatBerkah)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan
                // sebagai argumen ke `history.Any`.
                e.UserId == request.UserId &&
                // Meneruskan fungsi lambda `e => e.UserId == request.UserId && e.DayIndex == request.DayIndex && GameActionCatalog.Is(e.ActionType,
                // _payloadReader.ReadPayload(e.Payload), GameActionCatalog.JumatBerkah)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan
                // sebagai argumen ke `history.Any`.
                e.DayIndex == request.DayIndex &&
                // Meneruskan `e.ActionType` (nilai aksi jenis) sebagai argumen ke `GameActionCatalog.Is`; Meneruskan memanggil `_payloadReader.ReadPayload` dengan
                // `e.Payload` sebagai argumen ke `GameActionCatalog.Is`; Meneruskan `e.Payload` (muatan detail event dalam format JSON) sebagai argumen ke
                // `_payloadReader.ReadPayload`; Meneruskan `GameActionCatalog.JumatBerkah` (nilai jumat berkah) sebagai argumen ke `GameActionCatalog.Is`.
                GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.JumatBerkah)))
        // Membuka scope cabang if untuk kondisi `request.UserId.HasValue && history.Any(e => e.UserId == request.UserId && e.DayIndex == request.DayIndex
        // && GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Pay...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateFridayDonation.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DONATION_ALREADY_SUBMITTED”`, `”Donasi Jumat sudah dikirim
            // pemain pada hari ini”` kepada pemanggil dalam ValidateFridayDonation; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DONATION_ALREADY_SUBMITTED", "Donasi Jumat sudah dikirim pemain pada hari ini");
        // Menutup scope cabang if untuk kondisi `request.UserId.HasValue && history.Any(e => e.UserId == request.UserId && e.DayIndex == request.DayIndex
        // && GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Pay...`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateFridayDonation.
        }

        // Mengembalikan objek baru bertipe `EventEconomyActionValidation` dengan argumen (EventDomainValidationResult.Valid, request.UserId is null ? null
        // : amount) kepada pemanggil dalam ValidateFridayDonation; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventEconomyActionValidation(EventDomainValidationResult.Valid, request.UserId is null ? null : amount);
    // Menutup scope metode ValidateFridayDonation; bagian berikut berada di luar batas blok tersebut dalam ValidateFridayDonation.
    }

    // Mendefinisikan metode `ValidateGoldTrade` dengan hasil bertipe `EventEconomyActionValidation`; operasi ini menangani validate emas trade.
    // Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter
    // `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe
    // `IEnumerable<EventDb>` membawa nilai history.
    private EventEconomyActionValidation ValidateGoldTrade(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history)
    // Membuka scope metode ValidateGoldTrade; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldTrade.
    {
        // Memeriksa kebalikan kondisi `config.SaturdayEnabled`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoldTrade.
        if (!config.SaturdayEnabled)
        // Membuka scope cabang if untuk kondisi `!config.SaturdayEnabled`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateGoldTrade.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Fitur perdagangan emas tidak
            // aktif”` kepada pemanggil dalam ValidateGoldTrade; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Fitur perdagangan emas tidak aktif");
        // Menutup scope cabang if untuk kondisi `!config.SaturdayEnabled`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldTrade.
        }

        // Menyiapkan variabel lokal `referencedGoldRiskIsActive` untuk nilai referenced emas risiko berstatus aktif dengan memanggil
        // `HasActiveReferencedGoldRisk` dengan `request`, `config`, `history`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var referencedGoldRiskIsActive = HasActiveReferencedGoldRisk(request, config, history);
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!string.Equals(request.Weekday, ”SAT”, StringComparison.OrdinalIgnoreCase)` dan
        // `!referencedGoldRiskIsActive`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateGoldTrade.
        if (!string.Equals(request.Weekday, "SAT", StringComparison.OrdinalIgnoreCase) &&
            // Menggunakan kebalikan kondisi `referencedGoldRiskIsActive` sebagai bagian ekspresi yang sedang disusun dalam ValidateGoldTrade.
            !referencedGoldRiskIsActive)
        // Membuka scope cabang if untuk kondisi `!string.Equals(request.Weekday, ”SAT”, StringComparison.OrdinalIgnoreCase) &&
        // !referencedGoldRiskIsActive`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldTrade.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Weekday harus SAT”`, `new ErrorDetail(”weekday”,
            // ”INVALID_VALUE”)` kepada pemanggil dalam ValidateGoldTrade; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Weekday harus SAT”` sebagai argumen ke `Fail`.
                "Weekday harus SAT",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”weekday”, ”INVALID_VALUE”) sebagai argumen ke `Fail`; Meneruskan nilai literal
                // `”weekday”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”INVALID_VALUE”` sebagai argumen ke konstruktor
                // `ErrorDetail`.
                new ErrorDetail("weekday", "INVALID_VALUE"));
        // Menutup scope cabang if untuk kondisi `!string.Equals(request.Weekday, ”SAT”, StringComparison.OrdinalIgnoreCase) &&
        // !referencedGoldRiskIsActive`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldTrade.
        }

        // Menyiapkan variabel lokal `canonicalAction` untuk nilai canonical aksi dengan memanggil `GameActionCatalog.ResolveGameActionId` dengan
        // `request.ActionType`, `request.Payload`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var canonicalAction = GameActionCatalog.ResolveGameActionId(request.ActionType, request.Payload);
        // Menyiapkan variabel lokal `hasFullGoldPayload` untuk nilai memiliki full emas payload dengan memanggil `_payloadReader.TryReadGoldTrade` dengan
        // `request.Payload`, `var tradeType`, `var qty`, `var unitPrice`, `var amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var hasFullGoldPayload = _payloadReader.TryReadGoldTrade(request.Payload, out var tradeType, out var qty, out var unitPrice, out var amount);
        // Memeriksa kebalikan kondisi `hasFullGoldPayload`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoldTrade.
        if (!hasFullGoldPayload)
        // Membuka scope cabang if untuk kondisi `!hasFullGoldPayload`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldTrade.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!_payloadReader.TryGetInt32(request.Payload, ”qty”, out qty) ||
            // !_payloadReader.TryGetInt32(request.Payload, ”unit_price”, out unitPrice)` dan `!_payloadReader.TryGetInt32(request.Payload, ”amount”, out
            // amount)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoldTrade.
            if (!_payloadReader.TryGetInt32(request.Payload, "qty", out qty) ||
                // Menggunakan kebalikan kondisi `_payloadReader.TryGetInt32(request.Payload, ”unit_price”, out unitPrice)` sebagai bagian ekspresi yang sedang
                // disusun dalam ValidateGoldTrade.
                !_payloadReader.TryGetInt32(request.Payload, "unit_price", out unitPrice) ||
                // Menggunakan kebalikan kondisi `_payloadReader.TryGetInt32(request.Payload, ”amount”, out amount)` sebagai bagian ekspresi yang sedang disusun
                // dalam ValidateGoldTrade.
                !_payloadReader.TryGetInt32(request.Payload, "amount", out amount))
            // Membuka scope cabang if untuk kondisi `!_payloadReader.TryGetInt32(request.Payload, ”qty”, out qty) ||
            // !_payloadReader.TryGetInt32(request.Payload, ”unit_price”, out unitPrice) || !_payloadReader.TryGetInt32(reques...`; pernyataan/deklarasi berikut
            // berada di dalam batas blok ini dalam ValidateGoldTrade.
            {
                // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Payload gold trade tidak valid”`, `new
                // ErrorDetail(”payload”, ”INVALID_STRUCTURE”)` kepada pemanggil dalam ValidateGoldTrade; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return Fail(
                    // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                    StatusCodes.Status400BadRequest,
                    // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                    "VALIDATION_ERROR",
                    // Meneruskan nilai literal `”Payload gold trade tidak valid”` sebagai argumen ke `Fail`.
                    "Payload gold trade tidak valid",
                    // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload”, ”INVALID_STRUCTURE”) sebagai argumen ke `Fail`; Meneruskan nilai literal
                    // `”payload”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”INVALID_STRUCTURE”` sebagai argumen ke konstruktor
                    // `ErrorDetail`.
                    new ErrorDetail("payload", "INVALID_STRUCTURE"));
            // Menutup scope cabang if untuk kondisi `!_payloadReader.TryGetInt32(request.Payload, ”qty”, out qty) ||
            // !_payloadReader.TryGetInt32(request.Payload, ”unit_price”, out unitPrice) || !_payloadReader.TryGetInt32(reques...`; bagian berikut berada di
            // luar batas blok tersebut dalam ValidateGoldTrade.
            }

            // Memperbarui `tradeType` menggunakan hasil pemilihan bersyarat: ketika `string.Equals(canonicalAction, GameActionCatalog.JualEmas,
            // StringComparison.OrdinalIgnoreCase)` benar gunakan `”SELL”`, jika tidak gunakan `”BUY”` dalam ValidateGoldTrade.
            tradeType = string.Equals(canonicalAction, GameActionCatalog.JualEmas, StringComparison.OrdinalIgnoreCase)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”SELL” dalam ValidateGoldTrade.
                ? "SELL"
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: ”BUY”; dalam ValidateGoldTrade.
                : "BUY";
        // Menutup scope cabang if untuk kondisi `!hasFullGoldPayload`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldTrade.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!string.Equals(tradeType, ”BUY”, StringComparison.OrdinalIgnoreCase)` dan
        // `!string.Equals(tradeType, ”SELL”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam ValidateGoldTrade.
        if (!string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) &&
            // Menggunakan kebalikan kondisi `string.Equals(tradeType, ”SELL”, StringComparison.OrdinalIgnoreCase)` sebagai bagian ekspresi yang sedang disusun
            // dalam ValidateGoldTrade.
            !string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `!string.Equals(tradeType, ”BUY”, StringComparison.OrdinalIgnoreCase) && !string.Equals(tradeType, ”SELL”,
        // StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldTrade.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Trade type tidak valid”`, `new
            // ErrorDetail(”payload.trade_type”, ”INVALID_ENUM”)` kepada pemanggil dalam ValidateGoldTrade; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Trade type tidak valid”` sebagai argumen ke `Fail`.
                "Trade type tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.trade_type”, ”INVALID_ENUM”) sebagai argumen ke `Fail`; Meneruskan nilai
                // literal `”payload.trade_type”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”INVALID_ENUM”` sebagai argumen ke
                // konstruktor `ErrorDetail`.
                new ErrorDetail("payload.trade_type", "INVALID_ENUM"));
        // Menutup scope cabang if untuk kondisi `!string.Equals(tradeType, ”BUY”, StringComparison.OrdinalIgnoreCase) && !string.Equals(tradeType, ”SELL”,
        // StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldTrade.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `string.Equals(canonicalAction, GameActionCatalog.InvestasiEmas,
        // StringComparison.OrdinalIgnoreCase)` dan `!string.Equals(tradeType, ”BUY”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika
        // sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoldTrade.
        if (string.Equals(canonicalAction, GameActionCatalog.InvestasiEmas, StringComparison.OrdinalIgnoreCase) &&
            // Menggunakan kebalikan kondisi `string.Equals(tradeType, ”BUY”, StringComparison.OrdinalIgnoreCase)` sebagai bagian ekspresi yang sedang disusun
            // dalam ValidateGoldTrade.
            !string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(canonicalAction, GameActionCatalog.InvestasiEmas, StringComparison.OrdinalIgnoreCase) &&
        // !string.Equals(tradeType, ”BUY”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateGoldTrade.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”InvestasiEmas wajib memakai trade_type BUY”`,
            // `new ErrorDetail(”payload.trade_type”, ”INVALID_ENUM”)` kepada pemanggil dalam ValidateGoldTrade; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”InvestasiEmas wajib memakai trade_type BUY”` sebagai argumen ke `Fail`.
                "InvestasiEmas wajib memakai trade_type BUY",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.trade_type”, ”INVALID_ENUM”) sebagai argumen ke `Fail`; Meneruskan nilai
                // literal `”payload.trade_type”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”INVALID_ENUM”` sebagai argumen ke
                // konstruktor `ErrorDetail`.
                new ErrorDetail("payload.trade_type", "INVALID_ENUM"));
        // Menutup scope cabang if untuk kondisi `string.Equals(canonicalAction, GameActionCatalog.InvestasiEmas, StringComparison.OrdinalIgnoreCase) &&
        // !string.Equals(tradeType, ”BUY”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldTrade.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `string.Equals(canonicalAction, GameActionCatalog.JualEmas,
        // StringComparison.OrdinalIgnoreCase)` dan `!string.Equals(tradeType, ”SELL”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika
        // sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoldTrade.
        if (string.Equals(canonicalAction, GameActionCatalog.JualEmas, StringComparison.OrdinalIgnoreCase) &&
            // Menggunakan kebalikan kondisi `string.Equals(tradeType, ”SELL”, StringComparison.OrdinalIgnoreCase)` sebagai bagian ekspresi yang sedang disusun
            // dalam ValidateGoldTrade.
            !string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(canonicalAction, GameActionCatalog.JualEmas, StringComparison.OrdinalIgnoreCase) &&
        // !string.Equals(tradeType, ”SELL”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateGoldTrade.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”JualEmas wajib memakai trade_type SELL”`, `new
            // ErrorDetail(”payload.trade_type”, ”INVALID_ENUM”)` kepada pemanggil dalam ValidateGoldTrade; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”JualEmas wajib memakai trade_type SELL”` sebagai argumen ke `Fail`.
                "JualEmas wajib memakai trade_type SELL",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.trade_type”, ”INVALID_ENUM”) sebagai argumen ke `Fail`; Meneruskan nilai
                // literal `”payload.trade_type”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”INVALID_ENUM”` sebagai argumen ke
                // konstruktor `ErrorDetail`.
                new ErrorDetail("payload.trade_type", "INVALID_ENUM"));
        // Menutup scope cabang if untuk kondisi `string.Equals(canonicalAction, GameActionCatalog.JualEmas, StringComparison.OrdinalIgnoreCase) &&
        // !string.Equals(tradeType, ”SELL”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateGoldTrade.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `qty <= 0 || unitPrice <= 0` dan `amount <= 0`; sisi kanan diperiksa
        // hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoldTrade.
        if (qty <= 0 || unitPrice <= 0 || amount <= 0)
        // Membuka scope cabang if untuk kondisi `qty <= 0 || unitPrice <= 0 || amount <= 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ValidateGoldTrade.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Nilai qty/unit_price/amount tidak valid”`, `new
            // ErrorDetail(”payload.amount”, ”OUT_OF_RANGE”)` kepada pemanggil dalam ValidateGoldTrade; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Nilai qty/unit_price/amount tidak valid”` sebagai argumen ke `Fail`.
                "Nilai qty/unit_price/amount tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.amount”, ”OUT_OF_RANGE”) sebagai argumen ke `Fail`; Meneruskan nilai literal
                // `”payload.amount”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”OUT_OF_RANGE”` sebagai argumen ke konstruktor
                // `ErrorDetail`.
                new ErrorDetail("payload.amount", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `qty <= 0 || unitPrice <= 0 || amount <= 0`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateGoldTrade.
        }

        // Memeriksa perbandingan ketidaksamaan antara `amount` dan `unitPrice * qty`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateGoldTrade.
        if (amount != unitPrice * qty)
        // Membuka scope cabang if untuk kondisi `amount != unitPrice * qty`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateGoldTrade.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Amount tidak sesuai unit_price *
            // qty”` kepada pemanggil dalam ValidateGoldTrade; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Amount tidak sesuai unit_price * qty");
        // Menutup scope cabang if untuk kondisi `amount != unitPrice * qty`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldTrade.
        }

        // Menyiapkan variabel lokal `activePriceEvent` untuk nilai aktif harga event dengan mengambil elemen pertama `history .Where(e =>
        // GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.GoldPriceOpened) && e.DayIndex == request.DayIndex)
        // .OrderByDes...`; jika tidak ada, gunakan nilai default tipe hasil. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var activePriceEvent = history
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => GameActionCatalog.Is(e.ActionType,
            // _payloadReader.ReadPayload(e.Payload), GameActionCatalog.GoldPriceOpened) && dalam ValidateGoldTrade; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .Where(e => GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.GoldPriceOpened) &&
                        // Meneruskan fungsi lambda `e => GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.GoldPriceOpened) &&
                        // e.DayIndex == request.DayIndex` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `history .Where`.
                        e.DayIndex == request.DayIndex)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .OrderByDescending(e => e.SequenceNumber) dalam ValidateGoldTrade; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .OrderByDescending(e => e.SequenceNumber)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .FirstOrDefault(); dalam ValidateGoldTrade; token pada baris ini menyambungkan
            // bagian kode sebelum dan sesudahnya.
            .FirstOrDefault();
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `activePriceEvent is null ||
        // !_payloadReader.TryGetInt32(_payloadReader.ReadPayload(activePriceEvent.Payload), ”gold_price”, out var activePrice)` dan `activePrice !=
        // unitPrice`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoldTrade.
        if (activePriceEvent is null ||
            // Menggunakan kebalikan kondisi `_payloadReader.TryGetInt32(_payloadReader.ReadPayload(activePriceEvent.Payload), ”gold_price”, out var
            // activePrice)` sebagai bagian ekspresi yang sedang disusun dalam ValidateGoldTrade.
            !_payloadReader.TryGetInt32(_payloadReader.ReadPayload(activePriceEvent.Payload), "gold_price", out var activePrice) ||
            // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `activePrice` dan `unitPrice` dalam ValidateGoldTrade.
            activePrice != unitPrice)
        // Membuka scope cabang if untuk kondisi `activePriceEvent is null ||
        // !_payloadReader.TryGetInt32(_payloadReader.ReadPayload(activePriceEvent.Payload), ”gold_price”, out var activePrice) || activePrice !=
        // unitPrice`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldTrade.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Harga emas harus berasal dari
            // BukaHargaEmas pada hari transaksi”` kepada pemanggil dalam ValidateGoldTrade; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke `Fail`.
                StatusCodes.Status422UnprocessableEntity,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `Fail`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Harga emas harus berasal dari BukaHargaEmas pada hari transaksi”` sebagai argumen ke `Fail`.
                "Harga emas harus berasal dari BukaHargaEmas pada hari transaksi");
        // Menutup scope cabang if untuk kondisi `activePriceEvent is null ||
        // !_payloadReader.TryGetInt32(_payloadReader.ReadPayload(activePriceEvent.Payload), ”gold_price”, out var activePrice) || activePrice !=
        // unitPrice`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldTrade.
        }

        // Menyiapkan variabel lokal `goldTradeOpened` untuk nilai emas trade opened dengan false, yaitu kondisi nonaktif/tidak terpenuhi. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var goldTradeOpened = false;
        // Mengulangi setiap elemen `history`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh badan loop dalam ValidateGoldTrade.
        foreach (var evt in history)
        // Membuka scope loop setiap evt dari `history`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldTrade.
        {
            // Menyiapkan variabel lokal `eventPayload` untuk nilai event payload dengan memanggil `_payloadReader.ReadPayload` dengan `evt.Payload`. Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var eventPayload = _payloadReader.ReadPayload(evt.Payload);
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `GameActionCatalog.Is(evt.ActionType, eventPayload,
            // GameActionCatalog.RisikoKehidupan)` dan `_payloadReader.TryGetString(eventPayload, ”risk_id”, out var riskId)`; sisi kanan diperiksa hanya jika
            // sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoldTrade.
            if (GameActionCatalog.Is(evt.ActionType, eventPayload, GameActionCatalog.RisikoKehidupan) &&
                // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryGetString` dengan `eventPayload`, `”risk_id”`, `var riskId` dalam ValidateGoldTrade.
                _payloadReader.TryGetString(eventPayload, "risk_id", out var riskId))
            // Membuka scope cabang if untuk kondisi `GameActionCatalog.Is(evt.ActionType, eventPayload, GameActionCatalog.RisikoKehidupan) &&
            // _payloadReader.TryGetString(eventPayload, ”risk_id”, out var riskId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateGoldTrade.
            {
                // Menyiapkan variabel lokal `risk` untuk nilai risiko dengan mengambil elemen pertama `config.LifeRisks` yang sesuai `r =>
                // string.Equals(r.RiskCode, riskId, StringComparison.OrdinalIgnoreCase)`; jika tidak ada, gunakan nilai default tipe hasil. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var risk = config.LifeRisks.FirstOrDefault(r => string.Equals(r.RiskCode, riskId, StringComparison.OrdinalIgnoreCase));
                // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `risk is not null` dan `string.Equals(risk.EffectType, ”GOLD_TRADE”,
                // StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar
                // dalam ValidateGoldTrade.
                if (risk is not null && string.Equals(risk.EffectType, "GOLD_TRADE", StringComparison.OrdinalIgnoreCase))
                // Membuka scope cabang if untuk kondisi `risk is not null && string.Equals(risk.EffectType, ”GOLD_TRADE”, StringComparison.OrdinalIgnoreCase)`;
                // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldTrade.
                {
                    // Menyiapkan variabel lokal `startDay` untuk nilai start hari dengan `evt.DayIndex` (nilai hari index). Tipe variabel disimpulkan dari ekspresi
                    // nilai awal.
                    var startDay = evt.DayIndex;
                    // Menyiapkan variabel lokal `endDay` untuk nilai end hari dengan selisih antara `startDay + (risk.DurationDays ?? 1)` dan `1`. Tipe variabel
                    // disimpulkan dari ekspresi nilai awal.
                    var endDay = startDay + (risk.DurationDays ?? 1) - 1;
                    // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `request.DayIndex >= startDay` dan `request.DayIndex <= endDay`; sisi kanan
                    // diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoldTrade.
                    if (request.DayIndex >= startDay && request.DayIndex <= endDay)
                    // Membuka scope cabang if untuk kondisi `request.DayIndex >= startDay && request.DayIndex <= endDay`; pernyataan/deklarasi berikut berada di dalam
                    // batas blok ini dalam ValidateGoldTrade.
                    {
                        // Memperbarui `goldTradeOpened` menggunakan true, yaitu kondisi aktif/terpenuhi dalam ValidateGoldTrade.
                        goldTradeOpened = true;
                        // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ValidateGoldTrade.
                        break;
                    // Menutup scope cabang if untuk kondisi `request.DayIndex >= startDay && request.DayIndex <= endDay`; bagian berikut berada di luar batas blok
                    // tersebut dalam ValidateGoldTrade.
                    }
                // Menutup scope cabang if untuk kondisi `risk is not null && string.Equals(risk.EffectType, ”GOLD_TRADE”, StringComparison.OrdinalIgnoreCase)`;
                // bagian berikut berada di luar batas blok tersebut dalam ValidateGoldTrade.
                }
            // Menutup scope cabang if untuk kondisi `GameActionCatalog.Is(evt.ActionType, eventPayload, GameActionCatalog.RisikoKehidupan) &&
            // _payloadReader.TryGetString(eventPayload, ”risk_id”, out var riskId)`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldTrade.
            }
        // Menutup scope loop setiap evt dari `history`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldTrade.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `string.Equals(tradeType, ”BUY”, StringComparison.OrdinalIgnoreCase) &&
        // !config.GoldAllowBuy` dan `!goldTradeOpened`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ValidateGoldTrade.
        if (string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) && !config.GoldAllowBuy && !goldTradeOpened)
        // Membuka scope cabang if untuk kondisi `string.Equals(tradeType, ”BUY”, StringComparison.OrdinalIgnoreCase) && !config.GoldAllowBuy &&
        // !goldTradeOpened`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldTrade.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Ruleset melarang BUY emas”` kepada
            // pemanggil dalam ValidateGoldTrade; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Ruleset melarang BUY emas");
        // Menutup scope cabang if untuk kondisi `string.Equals(tradeType, ”BUY”, StringComparison.OrdinalIgnoreCase) && !config.GoldAllowBuy &&
        // !goldTradeOpened`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldTrade.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `string.Equals(tradeType, ”SELL”, StringComparison.OrdinalIgnoreCase) &&
        // !config.GoldAllowSell` dan `!goldTradeOpened`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ValidateGoldTrade.
        if (string.Equals(tradeType, "SELL", StringComparison.OrdinalIgnoreCase) && !config.GoldAllowSell && !goldTradeOpened)
        // Membuka scope cabang if untuk kondisi `string.Equals(tradeType, ”SELL”, StringComparison.OrdinalIgnoreCase) && !config.GoldAllowSell &&
        // !goldTradeOpened`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldTrade.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Ruleset melarang SELL emas”`
            // kepada pemanggil dalam ValidateGoldTrade; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Ruleset melarang SELL emas");
        // Menutup scope cabang if untuk kondisi `string.Equals(tradeType, ”SELL”, StringComparison.OrdinalIgnoreCase) && !config.GoldAllowSell &&
        // !goldTradeOpened`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldTrade.
        }

        // Memeriksa membandingkan kesamaan `string` dengan `tradeType`, `”BUY”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti
        // overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoldTrade.
        if (string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(tradeType, ”BUY”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada
        // di dalam batas blok ini dalam ValidateGoldTrade.
        {
            // Menyiapkan variabel lokal `cardsHeld` untuk nilai kartu held dengan `new GoldGameplayCalculator().Compute(history).GoldHeldEnd` (nilai emas held
            // end). Tipe variabel disimpulkan dari ekspresi nilai awal.
            var cardsHeld = new GoldGameplayCalculator().Compute(history).GoldHeldEnd;
            // Memeriksa pemeriksaan lebih besar antara `cardsHeld + qty` dan `RulebookGoldCardSupply`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam ValidateGoldTrade.
            if (cardsHeld + qty > RulebookGoldCardSupply)
            // Membuka scope cabang if untuk kondisi `cardsHeld + qty > RulebookGoldCardSupply`; pernyataan/deklarasi berikut berada di dalam batas blok ini
            // dalam ValidateGoldTrade.
            {
                // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `$”Stok fisik Kartu Emas tidak
                // cukup; tersedia {Math.Max(0, RulebookGoldCardSupply - cardsHeld)} kartu”` kepada pemanggil dalam ValidateGoldTrade; eksekusi jalur ini selesai
                // setelah nilai hasil ditentukan.
                return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    // Meneruskan teks interpolasi `$”Stok fisik Kartu Emas tidak cukup; tersedia {Math.Max(0, RulebookGoldCardSupply - cardsHeld)} kartu”`; nilai
                    // ekspresi di dalam kurung kurawal disisipkan saat program berjalan sebagai argumen ke `Fail`; Meneruskan nilai literal `0` sebagai argumen ke
                    // `Math.Max`; Meneruskan selisih antara `RulebookGoldCardSupply` dan `cardsHeld` sebagai argumen ke `Math.Max`.
                    $"Stok fisik Kartu Emas tidak cukup; tersedia {Math.Max(0, RulebookGoldCardSupply - cardsHeld)} kartu");
            // Menutup scope cabang if untuk kondisi `cardsHeld + qty > RulebookGoldCardSupply`; bagian berikut berada di luar batas blok tersebut dalam
            // ValidateGoldTrade.
            }
        // Menutup scope cabang if untuk kondisi `string.Equals(tradeType, ”BUY”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas
        // blok tersebut dalam ValidateGoldTrade.
        }

        // Menyiapkan variabel lokal `outgoing` untuk nilai outgoing dengan hasil pemilihan bersyarat: ketika `string.Equals(tradeType, ”BUY”,
        // StringComparison.OrdinalIgnoreCase) && request.UserId is not null` benar gunakan `amount`, jika tidak gunakan `(double?)null`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var outgoing = string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase) && request.UserId is not null
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: amount dalam ValidateGoldTrade.
            ? amount
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: (double?)null; dalam ValidateGoldTrade.
            : (double?)null;
        // Mengembalikan objek baru bertipe `EventEconomyActionValidation` dengan argumen (EventDomainValidationResult.Valid, outgoing) kepada pemanggil
        // dalam ValidateGoldTrade; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventEconomyActionValidation(EventDomainValidationResult.Valid, outgoing);
    // Menutup scope metode ValidateGoldTrade; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldTrade.
    }

    // Mendefinisikan metode `Fail` dengan hasil bertipe `EventEconomyActionValidation`; operasi ini menangani fail. Masukan: Parameter `statusCode`
    // bertipe `int` membawa kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan; Parameter `errorCode` bertipe `string` membawa
    // nilai kesalahan kode; Parameter `message` bertipe `string` membawa nilai pesan; Parameter `details` bertipe `ErrorDetail[]` membawa nilai
    // rincian.
    private EventEconomyActionValidation Fail(
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
        // Mengembalikan objek baru bertipe `EventEconomyActionValidation` dengan argumen ( EventDomainValidationResult.Fail(statusCode, errorCode, message,
        // details), null) kepada pemanggil dalam Fail; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventEconomyActionValidation(
            // Meneruskan memanggil `EventDomainValidationResult.Fail` dengan `statusCode`, `errorCode`, `message`, `details` sebagai argumen ke konstruktor
            // `EventEconomyActionValidation`; Meneruskan `statusCode` (kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan) sebagai
            // argumen ke `EventDomainValidationResult.Fail`; Meneruskan `errorCode` (nilai kesalahan kode) sebagai argumen ke
            // `EventDomainValidationResult.Fail`; Meneruskan `message` (nilai pesan) sebagai argumen ke `EventDomainValidationResult.Fail`; Meneruskan
            // `details` (nilai rincian) sebagai argumen ke `EventDomainValidationResult.Fail`.
            EventDomainValidationResult.Fail(statusCode, errorCode, message, details),
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke konstruktor `EventEconomyActionValidation`.
            null);
    // Menutup scope metode Fail; bagian berikut berada di luar batas blok tersebut dalam Fail.
    }

    // Mendefinisikan metode `HasActiveReferencedGoldRisk` dengan hasil bertipe `bool`; operasi ini menangani memiliki aktif referenced emas risiko.
    // Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter
    // `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe
    // `IEnumerable<EventDb>` membawa nilai history.
    private bool HasActiveReferencedGoldRisk(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history)
    // Membuka scope metode HasActiveReferencedGoldRisk; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam HasActiveReferencedGoldRisk.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!_payloadReader.TryGetString(request.Payload, ”risk_event_id”, out var
        // riskEventIdText)` dan `!Guid.TryParse(riskEventIdText, out var riskEventId)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam HasActiveReferencedGoldRisk.
        if (!_payloadReader.TryGetString(request.Payload, "risk_event_id", out var riskEventIdText) ||
            // Menggunakan kebalikan kondisi `Guid.TryParse(riskEventIdText, out var riskEventId)` sebagai bagian ekspresi yang sedang disusun dalam
            // HasActiveReferencedGoldRisk.
            !Guid.TryParse(riskEventIdText, out var riskEventId))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryGetString(request.Payload, ”risk_event_id”, out var riskEventIdText) ||
        // !Guid.TryParse(riskEventIdText, out var riskEventId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // HasActiveReferencedGoldRisk.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam HasActiveReferencedGoldRisk; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryGetString(request.Payload, ”risk_event_id”, out var riskEventIdText) ||
        // !Guid.TryParse(riskEventIdText, out var riskEventId)`; bagian berikut berada di luar batas blok tersebut dalam HasActiveReferencedGoldRisk.
        }

        // Menyiapkan variabel lokal `riskEvent` untuk nilai risiko event dengan mengambil elemen pertama `history` yang sesuai `e => e.EventId ==
        // riskEventId && e.SessionId == request.SessionId`; jika tidak ada, gunakan nilai default tipe hasil. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var riskEvent = history.FirstOrDefault(e => e.EventId == riskEventId && e.SessionId == request.SessionId);
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `riskEvent is null` dan `!GameActionCatalog.Is(riskEvent.ActionType,
        // _payloadReader.ReadPayload(riskEvent.Payload), GameActionCatalog.RisikoKehidupan)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if
        // hanya dijalankan ketika kondisi ini bernilai benar dalam HasActiveReferencedGoldRisk.
        if (riskEvent is null ||
            // Menggunakan kebalikan kondisi `GameActionCatalog.Is(riskEvent.ActionType, _payloadReader.ReadPayload(riskEvent.Payload),
            // GameActionCatalog.RisikoKehidupan)` sebagai bagian ekspresi yang sedang disusun dalam HasActiveReferencedGoldRisk.
            !GameActionCatalog.Is(riskEvent.ActionType, _payloadReader.ReadPayload(riskEvent.Payload), GameActionCatalog.RisikoKehidupan))
        // Membuka scope cabang if untuk kondisi `riskEvent is null || !GameActionCatalog.Is(riskEvent.ActionType,
        // _payloadReader.ReadPayload(riskEvent.Payload), GameActionCatalog.RisikoKehidupan)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam HasActiveReferencedGoldRisk.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam HasActiveReferencedGoldRisk; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `riskEvent is null || !GameActionCatalog.Is(riskEvent.ActionType,
        // _payloadReader.ReadPayload(riskEvent.Payload), GameActionCatalog.RisikoKehidupan)`; bagian berikut berada di luar batas blok tersebut dalam
        // HasActiveReferencedGoldRisk.
        }

        // Menyiapkan variabel lokal `riskPayload` untuk nilai risiko payload dengan memanggil `_payloadReader.ReadPayload` dengan `riskEvent.Payload`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var riskPayload = _payloadReader.ReadPayload(riskEvent.Payload);
        // Memeriksa kebalikan kondisi `_payloadReader.TryGetString(riskPayload, ”risk_id”, out var riskId)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam HasActiveReferencedGoldRisk.
        if (!_payloadReader.TryGetString(riskPayload, "risk_id", out var riskId))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryGetString(riskPayload, ”risk_id”, out var riskId)`; pernyataan/deklarasi berikut berada
        // di dalam batas blok ini dalam HasActiveReferencedGoldRisk.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam HasActiveReferencedGoldRisk; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryGetString(riskPayload, ”risk_id”, out var riskId)`; bagian berikut berada di luar batas
        // blok tersebut dalam HasActiveReferencedGoldRisk.
        }

        // Menyiapkan variabel lokal `risk` untuk nilai risiko dengan mengambil elemen pertama `config.LifeRisks` yang sesuai `item =>
        // string.Equals(item.RiskCode, riskId, StringComparison.OrdinalIgnoreCase)`; jika tidak ada, gunakan nilai default tipe hasil. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var risk = config.LifeRisks.FirstOrDefault(item =>
            // Meneruskan `item.RiskCode` (nilai risiko kode) sebagai argumen ke `string.Equals`; Meneruskan `riskId` (nilai risiko identitas) sebagai argumen
            // ke `string.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `string.Equals`.
            string.Equals(item.RiskCode, riskId, StringComparison.OrdinalIgnoreCase));
        // Menyiapkan variabel lokal `endDay` untuk nilai end hari dengan selisih antara `riskEvent.DayIndex + (risk?.DurationDays ?? 1)` dan `1`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var endDay = riskEvent.DayIndex + (risk?.DurationDays ?? 1) - 1;
        // Mengembalikan gabungan syarat AND: kedua kondisi wajib benar antara `risk is not null && string.Equals(risk.EffectType, ”GOLD_TRADE”,
        // StringComparison.OrdinalIgnoreCase) && request.DayIndex >= riskEvent.DayIndex` dan `request.DayIndex <= endDay`; sisi kanan diperiksa hanya jika
        // sisi kiri benar kepada pemanggil dalam HasActiveReferencedGoldRisk; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return risk is not null &&
               // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `risk.EffectType`, `”GOLD_TRADE”`, `StringComparison.OrdinalIgnoreCase`;
               // aturan perbandingan mengikuti overload dan comparer yang diberikan dalam HasActiveReferencedGoldRisk.
               string.Equals(risk.EffectType, "GOLD_TRADE", StringComparison.OrdinalIgnoreCase) &&
               // Melanjutkan ekspresi dengan pemeriksaan lebih besar atau sama antara `request.DayIndex` dan `riskEvent.DayIndex` dalam
               // HasActiveReferencedGoldRisk.
               request.DayIndex >= riskEvent.DayIndex &&
               // Melanjutkan ekspresi dengan pemeriksaan lebih kecil atau sama antara `request.DayIndex` dan `endDay` dalam HasActiveReferencedGoldRisk.
               request.DayIndex <= endDay;
    // Menutup scope metode HasActiveReferencedGoldRisk; bagian berikut berada di luar batas blok tersebut dalam HasActiveReferencedGoldRisk.
    }

    // Mendefinisikan metode `HasActiveGoldRiskOnDay` dengan hasil bertipe `bool`; operasi ini menangani memiliki aktif emas risiko on hari. Masukan:
    // Parameter `dayIndex` bertipe `int` membawa nilai hari index; Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang
    // dipakai untuk validasi dan perhitungan; Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
    private static bool HasActiveGoldRiskOnDay(
        // Parameter `dayIndex` bertipe `int` membawa nilai hari index.
        int dayIndex,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history)
    // Membuka scope metode HasActiveGoldRiskOnDay; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam HasActiveGoldRiskOnDay.
    {
        // Mengulangi setiap elemen `history`; elemen saat ini disimpan sebagai `evt` bertipe `var` untuk diproses oleh badan loop dalam
        // HasActiveGoldRiskOnDay.
        foreach (var evt in history)
        // Membuka scope loop setiap evt dari `history`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam HasActiveGoldRiskOnDay.
        {
            // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan memanggil `_payloadReader.ReadPayload` dengan
            // `evt.Payload`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var payload = _payloadReader.ReadPayload(evt.Payload);
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!GameActionCatalog.Is(evt.ActionType, payload,
            // GameActionCatalog.RisikoKehidupan)` dan `!_payloadReader.TryGetString(payload, ”risk_id”, out var riskId)`; sisi kanan diperiksa hanya jika sisi
            // kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam HasActiveGoldRiskOnDay.
            if (!GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.RisikoKehidupan) ||
                // Menggunakan kebalikan kondisi `_payloadReader.TryGetString(payload, ”risk_id”, out var riskId)` sebagai bagian ekspresi yang sedang disusun dalam
                // HasActiveGoldRiskOnDay.
                !_payloadReader.TryGetString(payload, "risk_id", out var riskId))
            // Membuka scope cabang if untuk kondisi `!GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.RisikoKehidupan) ||
            // !_payloadReader.TryGetString(payload, ”risk_id”, out var riskId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // HasActiveGoldRiskOnDay.
            {
                // Melewati sisa pernyataan pada iterasi saat ini dan melanjutkan ke elemen/iterasi berikutnya dalam HasActiveGoldRiskOnDay.
                continue;
            // Menutup scope cabang if untuk kondisi `!GameActionCatalog.Is(evt.ActionType, payload, GameActionCatalog.RisikoKehidupan) ||
            // !_payloadReader.TryGetString(payload, ”risk_id”, out var riskId)`; bagian berikut berada di luar batas blok tersebut dalam
            // HasActiveGoldRiskOnDay.
            }

            // Menyiapkan variabel lokal `risk` untuk nilai risiko dengan mengambil elemen pertama `config.LifeRisks` yang sesuai `item =>
            // string.Equals(item.RiskCode, riskId, StringComparison.OrdinalIgnoreCase)`; jika tidak ada, gunakan nilai default tipe hasil. Tipe variabel
            // disimpulkan dari ekspresi nilai awal.
            var risk = config.LifeRisks.FirstOrDefault(item =>
                // Meneruskan `item.RiskCode` (nilai risiko kode) sebagai argumen ke `string.Equals`; Meneruskan `riskId` (nilai risiko identitas) sebagai argumen
                // ke `string.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `string.Equals`.
                string.Equals(item.RiskCode, riskId, StringComparison.OrdinalIgnoreCase));
            // Menyiapkan variabel lokal `endDay` untuk nilai end hari dengan selisih antara `evt.DayIndex + (risk?.DurationDays ?? 1)` dan `1`. Tipe variabel
            // disimpulkan dari ekspresi nilai awal.
            var endDay = evt.DayIndex + (risk?.DurationDays ?? 1) - 1;
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `risk is not null && string.Equals(risk.EffectType, ”GOLD_TRADE”,
            // StringComparison.OrdinalIgnoreCase) && dayIndex >= evt.DayIndex` dan `dayIndex <= endDay`; sisi kanan diperiksa hanya jika sisi kiri benar; blok
            // if hanya dijalankan ketika kondisi ini bernilai benar dalam HasActiveGoldRiskOnDay.
            if (risk is not null &&
                // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `risk.EffectType`, `”GOLD_TRADE”`, `StringComparison.OrdinalIgnoreCase`;
                // aturan perbandingan mengikuti overload dan comparer yang diberikan dalam HasActiveGoldRiskOnDay.
                string.Equals(risk.EffectType, "GOLD_TRADE", StringComparison.OrdinalIgnoreCase) &&
                // Melanjutkan ekspresi dengan pemeriksaan lebih besar atau sama antara `dayIndex` dan `evt.DayIndex` dalam HasActiveGoldRiskOnDay.
                dayIndex >= evt.DayIndex && dayIndex <= endDay)
            // Membuka scope cabang if untuk kondisi `risk is not null && string.Equals(risk.EffectType, ”GOLD_TRADE”, StringComparison.OrdinalIgnoreCase) &&
            // dayIndex >= evt.DayIndex && dayIndex <= endDay`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam HasActiveGoldRiskOnDay.
            {
                // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam HasActiveGoldRiskOnDay; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return true;
            // Menutup scope cabang if untuk kondisi `risk is not null && string.Equals(risk.EffectType, ”GOLD_TRADE”, StringComparison.OrdinalIgnoreCase) &&
            // dayIndex >= evt.DayIndex && dayIndex <= endDay`; bagian berikut berada di luar batas blok tersebut dalam HasActiveGoldRiskOnDay.
            }
        // Menutup scope loop setiap evt dari `history`; bagian berikut berada di luar batas blok tersebut dalam HasActiveGoldRiskOnDay.
        }

        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam HasActiveGoldRiskOnDay; eksekusi jalur ini selesai setelah
        // nilai hasil ditentukan.
        return false;
    // Menutup scope metode HasActiveGoldRiskOnDay; bagian berikut berada di luar batas blok tersebut dalam HasActiveGoldRiskOnDay.
    }
// Menutup scope tipe EventEconomyActionValidator; bagian berikut berada di luar batas blok tersebut.
}
