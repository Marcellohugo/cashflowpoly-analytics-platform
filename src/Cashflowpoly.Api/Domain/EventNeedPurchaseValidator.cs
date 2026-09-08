// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventNeedPurchaseValidator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `EventNeedPurchaseValidation`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed record EventNeedPurchaseValidation(
    // Parameter `Validation` bertipe `EventDomainValidationResult` membawa nilai validasi.
    EventDomainValidationResult Validation,
    // Parameter `OutgoingAmount` bertipe `int?` membawa nilai outgoing nominal; nilai null diizinkan ketika data opsional belum tersedia.
    int? OutgoingAmount);

// Mendefinisikan tipe class `EventNeedPurchaseValidator` yang mewarisi atau menerapkan `IEventNeedPurchaseValidator`; sealed mencegah tipe ini
// diturunkan lagi.
internal sealed class EventNeedPurchaseValidator : IEventNeedPurchaseValidator
// Membuka scope tipe EventNeedPurchaseValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `EventPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly EventPayloadReader _payloadReader = new();

    // Mendefinisikan metode `TryValidate` dengan hasil bertipe `bool`; operasi ini menangani try validate. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `config` bertipe `RulesetConfig`
    // membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe `IEnumerable<EventDb>` membawa
    // nilai history; Parameter `result` bertipe `EventNeedPurchaseValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryValidate(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history,
        // Parameter `result` bertipe `EventNeedPurchaseValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out EventNeedPurchaseValidation result)
    // Membuka scope metode TryValidate; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
    {
        // Memeriksa kebalikan kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.Kebutuhan)`; blok if hanya dijalankan
        // ketika kondisi ini bernilai benar dalam TryValidate.
        if (!GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.Kebutuhan))
        // Membuka scope cabang if untuk kondisi `!GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.Kebutuhan)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan objek baru bertipe `EventNeedPurchaseValidation` dengan argumen (EventDomainValidationResult.Valid, null) dalam
            // TryValidate.
            result = new EventNeedPurchaseValidation(EventDomainValidationResult.Valid, null);
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.Kebutuhan)`; bagian berikut
        // berada di luar batas blok tersebut dalam TryValidate.
        }

        // Memeriksa kebalikan kondisi `_payloadReader.TryReadNeedPurchase(request.Payload, out var cardId, out var amount, out var points)`; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (!_payloadReader.TryReadNeedPurchase(request.Payload, out var cardId, out var amount, out var points))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadNeedPurchase(request.Payload, out var cardId, out var amount, out var points)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Menyiapkan variabel lokal `payloadValidation` untuk nilai payload validasi dengan memanggil `ValidatePayload` dengan `request`, `false`, `_`,
            // `_`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var payloadValidation = ValidatePayload(request, primary: false, out _, out _);
            // Memperbarui `result` menggunakan objek baru bertipe `EventNeedPurchaseValidation` dengan argumen (payloadValidation, null) dalam TryValidate.
            result = new EventNeedPurchaseValidation(payloadValidation, null);
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadNeedPurchase(request.Payload, out var cardId, out var amount, out var points)`;
        // bagian berikut berada di luar batas blok tersebut dalam TryValidate.
        }

        // Menyiapkan variabel lokal `needTier` untuk nilai kebutuhan tingkat dengan memanggil `NeedTierClassifier.FromPayload` dengan `request.Payload`,
        // `cardId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var needTier = NeedTierClassifier.FromPayload(request.Payload, cardId);
        // Memeriksa pemeriksaan lebih besar antara `config.Needs.Count` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryValidate.
        if (config.Needs.Count > 0)
        // Membuka scope cabang if untuk kondisi `config.Needs.Count > 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Menyiapkan variabel lokal `catalogItem` untuk nilai catalog elemen dengan mengambil elemen pertama `config.Needs` yang sesuai `item =>
            // string.Equals(item.Id, cardId, StringComparison.OrdinalIgnoreCase)`; jika tidak ada, gunakan nilai default tipe hasil. Tipe variabel disimpulkan
            // dari ekspresi nilai awal.
            var catalogItem = config.Needs.FirstOrDefault(item =>
                // Meneruskan `item.Id` (nilai identitas) sebagai argumen ke `string.Equals`; Meneruskan `cardId` (nilai kartu identitas) sebagai argumen ke
                // `string.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `string.Equals`.
                string.Equals(item.Id, cardId, StringComparison.OrdinalIgnoreCase));
            // Memeriksa hasil pencocokan `catalogItem` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
            if (catalogItem is null)
            // Membuka scope cabang if untuk kondisi `catalogItem is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
            {
                // Memperbarui `result` menggunakan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Kartu kebutuhan
                // tidak ditemukan pada ruleset”` dalam TryValidate.
                result = Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    // Meneruskan nilai literal `”Kartu kebutuhan tidak ditemukan pada ruleset”` sebagai argumen ke `Fail`.
                    "Kartu kebutuhan tidak ditemukan pada ruleset");
                // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return true;
            // Menutup scope cabang if untuk kondisi `catalogItem is null`; bagian berikut berada di luar batas blok tersebut dalam TryValidate.
            }

            // Menyiapkan variabel lokal `catalogTier` untuk nilai catalog tingkat dengan hasil pemetaan `catalogItem.Tipe.Trim().ToLowerInvariant()` melalui
            // cabang pola switch yang cocok. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var catalogTier = catalogItem.Tipe.Trim().ToLowerInvariant() switch
            // Membuka scope pemetaan switch atas `catalogItem.Tipe.Trim().ToLowerInvariant()`; pernyataan/deklarasi berikut berada di dalam batas blok ini
            // dalam TryValidate.
            {
                // Untuk pola `”primer” or ”primary”`, menghasilkan `NeedTier.Primary` (nilai primary) sebagai hasil switch.
                "primer" or "primary" => NeedTier.Primary,
                // Untuk pola `”sekunder” or ”secondary”`, menghasilkan `NeedTier.Secondary` (nilai secondary) sebagai hasil switch.
                "sekunder" or "secondary" => NeedTier.Secondary,
                // Untuk pola `”tersier” or ”tertiary”`, menghasilkan `NeedTier.Tertiary` (nilai tertiary) sebagai hasil switch.
                "tersier" or "tertiary" => NeedTier.Tertiary,
                // Untuk pola `_`, menghasilkan `NeedTier.Unknown` (nilai unknown) sebagai hasil switch.
                _ => NeedTier.Unknown
            // Menutup scope pemetaan switch atas `catalogItem.Tipe.Trim().ToLowerInvariant()`; bagian berikut berada di luar batas blok tersebut dalam
            // TryValidate.
            };
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `amount != catalogItem.HargaBeli || points !=
            // catalogItem.PoinKebahagiaan` dan `needTier != catalogTier`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika
            // kondisi ini bernilai benar dalam TryValidate.
            if (amount != catalogItem.HargaBeli || points != catalogItem.PoinKebahagiaan || needTier != catalogTier)
            // Membuka scope cabang if untuk kondisi `amount != catalogItem.HargaBeli || points != catalogItem.PoinKebahagiaan || needTier != catalogTier`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
            {
                // Memperbarui `result` menggunakan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Harga, poin,
                // atau jenis kebutuhan tidak sesuai katalog ruleset”` dalam TryValidate.
                result = Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                    // Meneruskan nilai literal `”Harga, poin, atau jenis kebutuhan tidak sesuai katalog ruleset”` sebagai argumen ke `Fail`.
                    "Harga, poin, atau jenis kebutuhan tidak sesuai katalog ruleset");
                // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return true;
            // Menutup scope cabang if untuk kondisi `amount != catalogItem.HargaBeli || points != catalogItem.PoinKebahagiaan || needTier != catalogTier`;
            // bagian berikut berada di luar batas blok tersebut dalam TryValidate.
            }
        // Menutup scope cabang if untuk kondisi `config.Needs.Count > 0`; bagian berikut berada di luar batas blok tersebut dalam TryValidate.
        }

        // Memperbarui `result` menggunakan hasil pemilihan bersyarat: ketika `needTier == NeedTier.Primary` benar gunakan `ValidatePrimary(request, config,
        // history)`, jika tidak gunakan `ValidateSecondaryOrTertiary(request, config, history)` dalam TryValidate.
        result = needTier == NeedTier.Primary
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ValidatePrimary(request, config, history) dalam TryValidate.
            ? ValidatePrimary(request, config, history)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: ValidateSecondaryOrTertiary(request, config, history); dalam
            // TryValidate.
            : ValidateSecondaryOrTertiary(request, config, history);
        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return true;
    // Menutup scope metode TryValidate; bagian berikut berada di luar batas blok tersebut dalam TryValidate.
    }

    // Mendefinisikan metode `ValidatePrimary` dengan hasil bertipe `EventNeedPurchaseValidation`; operasi ini menangani validate primary. Masukan:
    // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `config`
    // bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe
    // `IEnumerable<EventDb>` membawa nilai history.
    private EventNeedPurchaseValidation ValidatePrimary(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history)
    // Membuka scope metode ValidatePrimary; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidatePrimary.
    {
        // Menyiapkan variabel lokal `payloadValidation` untuk nilai payload validasi dengan memanggil `ValidatePayload` dengan `request`, `true`, `_`, `var
        // amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var payloadValidation = ValidatePayload(request, primary: true, out _, out var amount);
        // Memeriksa kebalikan kondisi `payloadValidation.IsValid`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidatePrimary.
        if (!payloadValidation.IsValid)
        // Membuka scope cabang if untuk kondisi `!payloadValidation.IsValid`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidatePrimary.
        {
            // Mengembalikan objek baru bertipe `EventNeedPurchaseValidation` dengan argumen (payloadValidation, null) kepada pemanggil dalam ValidatePrimary;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return new EventNeedPurchaseValidation(payloadValidation, null);
        // Menutup scope cabang if untuk kondisi `!payloadValidation.IsValid`; bagian berikut berada di luar batas blok tersebut dalam ValidatePrimary.
        }

        // Memeriksa hasil pencocokan `request.UserId` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidatePrimary.
        if (request.UserId is null)
        // Membuka scope cabang if untuk kondisi `request.UserId is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidatePrimary.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Player wajib diisi”`, `new
            // ErrorDetail(”user_id”, ”REQUIRED”)` kepada pemanggil dalam ValidatePrimary; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
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
        // Menutup scope cabang if untuk kondisi `request.UserId is null`; bagian berikut berada di luar batas blok tersebut dalam ValidatePrimary.
        }

        // Menyiapkan variabel lokal `primaryCount` untuk nilai primary jumlah dengan memanggil `history.Count` dengan `e => e.UserId == request.UserId &&
        // e.DayIndex == request.DayIndex && GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.Kebutuhan) && ...`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var primaryCount = history.Count(e =>
            // Meneruskan fungsi lambda `e => e.UserId == request.UserId && e.DayIndex == request.DayIndex && GameActionCatalog.Is(e.ActionType,
            // _payloadReader.ReadPayload(e.Payload), GameActionCatalog.Kebutuhan) && ...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan
            // sebagai argumen ke `history.Count`.
            e.UserId == request.UserId &&
            // Meneruskan fungsi lambda `e => e.UserId == request.UserId && e.DayIndex == request.DayIndex && GameActionCatalog.Is(e.ActionType,
            // _payloadReader.ReadPayload(e.Payload), GameActionCatalog.Kebutuhan) && ...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan
            // sebagai argumen ke `history.Count`.
            e.DayIndex == request.DayIndex &&
            // Meneruskan `e.ActionType` (nilai aksi jenis) sebagai argumen ke `GameActionCatalog.Is`; Meneruskan memanggil `_payloadReader.ReadPayload` dengan
            // `e.Payload` sebagai argumen ke `GameActionCatalog.Is`; Meneruskan `e.Payload` (muatan detail event dalam format JSON) sebagai argumen ke
            // `_payloadReader.ReadPayload`; Meneruskan `GameActionCatalog.Kebutuhan` (nilai kebutuhan) sebagai argumen ke `GameActionCatalog.Is`.
            GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.Kebutuhan) &&
            // Meneruskan `e.Payload` (muatan detail event dalam format JSON) sebagai argumen ke `NeedTierClassifier.FromPayloadJson`.
            NeedTierClassifier.FromPayloadJson(e.Payload) == NeedTier.Primary);

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `config.PrimaryNeedMaxPerDay is > 0` dan `primaryCount >=
        // config.PrimaryNeedMaxPerDay.Value`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ValidatePrimary.
        if (config.PrimaryNeedMaxPerDay is > 0 && primaryCount >= config.PrimaryNeedMaxPerDay.Value)
        // Membuka scope cabang if untuk kondisi `config.PrimaryNeedMaxPerDay is > 0 && primaryCount >= config.PrimaryNeedMaxPerDay.Value`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidatePrimary.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Pembelian kebutuhan primer
            // melebihi batas harian”` kepada pemanggil dalam ValidatePrimary; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Pembelian kebutuhan primer melebihi batas harian");
        // Menutup scope cabang if untuk kondisi `config.PrimaryNeedMaxPerDay is > 0 && primaryCount >= config.PrimaryNeedMaxPerDay.Value`; bagian berikut
        // berada di luar batas blok tersebut dalam ValidatePrimary.
        }

        // Mengembalikan objek baru bertipe `EventNeedPurchaseValidation` dengan argumen (EventDomainValidationResult.Valid, amount) kepada pemanggil dalam
        // ValidatePrimary; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventNeedPurchaseValidation(EventDomainValidationResult.Valid, amount);
    // Menutup scope metode ValidatePrimary; bagian berikut berada di luar batas blok tersebut dalam ValidatePrimary.
    }

    // Mendefinisikan metode `ValidateSecondaryOrTertiary` dengan hasil bertipe `EventNeedPurchaseValidation`; operasi ini menangani validate secondary
    // atau tertiary. Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke
    // layanan; Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter
    // `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
    private EventNeedPurchaseValidation ValidateSecondaryOrTertiary(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history)
    // Membuka scope metode ValidateSecondaryOrTertiary; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateSecondaryOrTertiary.
    {
        // Menyiapkan variabel lokal `payloadValidation` untuk nilai payload validasi dengan memanggil `ValidatePayload` dengan `request`, `false`, `_`,
        // `var amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var payloadValidation = ValidatePayload(request, primary: false, out _, out var amount);
        // Memeriksa kebalikan kondisi `payloadValidation.IsValid`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateSecondaryOrTertiary.
        if (!payloadValidation.IsValid)
        // Membuka scope cabang if untuk kondisi `!payloadValidation.IsValid`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateSecondaryOrTertiary.
        {
            // Mengembalikan objek baru bertipe `EventNeedPurchaseValidation` dengan argumen (payloadValidation, null) kepada pemanggil dalam
            // ValidateSecondaryOrTertiary; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return new EventNeedPurchaseValidation(payloadValidation, null);
        // Menutup scope cabang if untuk kondisi `!payloadValidation.IsValid`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateSecondaryOrTertiary.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `config.RequirePrimaryBeforeOthers` dan `request.UserId is not null`; sisi kanan
        // diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateSecondaryOrTertiary.
        if (config.RequirePrimaryBeforeOthers && request.UserId is not null)
        // Membuka scope cabang if untuk kondisi `config.RequirePrimaryBeforeOthers && request.UserId is not null`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam ValidateSecondaryOrTertiary.
        {
            // Menyiapkan variabel lokal `hasPrimary` untuk nilai memiliki primary dengan memeriksa apakah `history` memiliki setidaknya satu elemen yang
            // memenuhi `e => e.UserId == request.UserId && GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload),
            // GameActionCatalog.Kebutuhan) && NeedTierClassifier.FromPayloadJson...`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var hasPrimary = history.Any(e =>
                // Meneruskan fungsi lambda `e => e.UserId == request.UserId && GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload),
                // GameActionCatalog.Kebutuhan) && NeedTierClassifier.FromPayloadJson...` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan
                // sebagai argumen ke `history.Any`.
                e.UserId == request.UserId &&
                // Meneruskan `e.ActionType` (nilai aksi jenis) sebagai argumen ke `GameActionCatalog.Is`; Meneruskan memanggil `_payloadReader.ReadPayload` dengan
                // `e.Payload` sebagai argumen ke `GameActionCatalog.Is`; Meneruskan `e.Payload` (muatan detail event dalam format JSON) sebagai argumen ke
                // `_payloadReader.ReadPayload`; Meneruskan `GameActionCatalog.Kebutuhan` (nilai kebutuhan) sebagai argumen ke `GameActionCatalog.Is`.
                GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.Kebutuhan) &&
                // Meneruskan `e.Payload` (muatan detail event dalam format JSON) sebagai argumen ke `NeedTierClassifier.FromPayloadJson`.
                NeedTierClassifier.FromPayloadJson(e.Payload) == NeedTier.Primary);

            // Memeriksa kebalikan kondisi `hasPrimary`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateSecondaryOrTertiary.
            if (!hasPrimary)
            // Membuka scope cabang if untuk kondisi `!hasPrimary`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateSecondaryOrTertiary.
            {
                // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Kebutuhan primer harus dibeli
                // terlebih dahulu”` kepada pemanggil dalam ValidateSecondaryOrTertiary; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Kebutuhan primer harus dibeli terlebih dahulu");
            // Menutup scope cabang if untuk kondisi `!hasPrimary`; bagian berikut berada di luar batas blok tersebut dalam ValidateSecondaryOrTertiary.
            }
        // Menutup scope cabang if untuk kondisi `config.RequirePrimaryBeforeOthers && request.UserId is not null`; bagian berikut berada di luar batas blok
        // tersebut dalam ValidateSecondaryOrTertiary.
        }

        // Mengembalikan objek baru bertipe `EventNeedPurchaseValidation` dengan argumen (EventDomainValidationResult.Valid, request.UserId is null ? null :
        // amount) kepada pemanggil dalam ValidateSecondaryOrTertiary; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventNeedPurchaseValidation(EventDomainValidationResult.Valid, request.UserId is null ? null : amount);
    // Menutup scope metode ValidateSecondaryOrTertiary; bagian berikut berada di luar batas blok tersebut dalam ValidateSecondaryOrTertiary.
    }

    // Mendefinisikan metode `ValidatePayload` dengan hasil bertipe `EventDomainValidationResult`; operasi ini menangani validate payload. Masukan:
    // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `primary`
    // bertipe `bool` membawa nilai primary; Parameter `cardId` bertipe `string` membawa nilai kartu identitas; out mengembalikan nilai melalui
    // parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    private EventDomainValidationResult ValidatePayload(EventRequest request, bool primary, out string cardId, out int amount)
    // Membuka scope metode ValidatePayload; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidatePayload.
    {
        // Memperbarui `cardId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam ValidatePayload.
        cardId = string.Empty;
        // Memperbarui `amount` menggunakan nilai literal `0` dalam ValidatePayload.
        amount = 0;
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadNeedPurchase(request.Payload, out cardId, out amount, out var points)`; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam ValidatePayload.
        if (!_payloadReader.TryReadNeedPurchase(request.Payload, out cardId, out amount, out var points))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadNeedPurchase(request.Payload, out cardId, out amount, out var points)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidatePayload.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `primary ? ”Payload
            // kebutuhan primer tidak valid” : ”Payload kebutuhan tidak valid”`, `new ErrorDetail(”payload.card_id”, ”REQUIRED”)` kepada pemanggil dalam
            // ValidatePayload; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan hasil pemilihan bersyarat: ketika `primary` benar gunakan `”Payload kebutuhan primer tidak valid”`, jika tidak gunakan `”Payload
                // kebutuhan tidak valid”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                primary ? "Payload kebutuhan primer tidak valid" : "Payload kebutuhan tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.card_id”, ”REQUIRED”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”payload.card_id”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen
                // ke konstruktor `ErrorDetail`.
                new ErrorDetail("payload.card_id", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadNeedPurchase(request.Payload, out cardId, out amount, out var points)`; bagian
        // berikut berada di luar batas blok tersebut dalam ValidatePayload.
        }

        // Memeriksa pemeriksaan lebih kecil atau sama antara `amount` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidatePayload.
        if (amount <= 0)
        // Membuka scope cabang if untuk kondisi `amount <= 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidatePayload.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Amount harus > 0”`,
            // `new ErrorDetail(”payload.amount”, ”OUT_OF_RANGE”)` kepada pemanggil dalam ValidatePayload; eksekusi jalur ini selesai setelah nilai hasil
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
        // Menutup scope cabang if untuk kondisi `amount <= 0`; bagian berikut berada di luar batas blok tersebut dalam ValidatePayload.
        }

        // Memeriksa memeriksa apakah `cardId` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ValidatePayload.
        if (string.IsNullOrWhiteSpace(cardId))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(cardId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidatePayload.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Card ID wajib
            // diisi”`, `new ErrorDetail(”payload.card_id”, ”REQUIRED”)` kepada pemanggil dalam ValidatePayload; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Card ID wajib diisi”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Card ID wajib diisi",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.card_id”, ”REQUIRED”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”payload.card_id”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen
                // ke konstruktor `ErrorDetail`.
                new ErrorDetail("payload.card_id", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(cardId)`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidatePayload.
        }

        // Memeriksa pemeriksaan lebih kecil antara `points` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidatePayload.
        if (points < 0)
        // Membuka scope cabang if untuk kondisi `points < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidatePayload.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Points tidak
            // valid”`, `new ErrorDetail(”payload.points”, ”OUT_OF_RANGE”)` kepada pemanggil dalam ValidatePayload; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Points tidak valid”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Points tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.points”, ”OUT_OF_RANGE”) sebagai argumen ke
                // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”payload.points”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai
                // literal `”OUT_OF_RANGE”` sebagai argumen ke konstruktor `ErrorDetail`.
                new ErrorDetail("payload.points", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `points < 0`; bagian berikut berada di luar batas blok tersebut dalam ValidatePayload.
        }

        // Memeriksa kebalikan kondisi `request.Payload.TryGetProperty(”points”, out _)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidatePayload.
        if (!request.Payload.TryGetProperty("points", out _))
        // Membuka scope cabang if untuk kondisi `!request.Payload.TryGetProperty(”points”, out _)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ValidatePayload.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Points wajib
            // diisi”`, `new ErrorDetail(”payload.points”, ”REQUIRED”)` kepada pemanggil dalam ValidatePayload; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Points wajib diisi”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Points wajib diisi",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.points”, ”REQUIRED”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”payload.points”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen
                // ke konstruktor `ErrorDetail`.
                new ErrorDetail("payload.points", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `!request.Payload.TryGetProperty(”points”, out _)`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidatePayload.
        }

        // Memeriksa perbandingan kesamaan antara `NeedTierClassifier.FromPayload(request.Payload, cardId)` dan `NeedTier.Unknown`; blok if hanya dijalankan
        // ketika kondisi ini bernilai benar dalam ValidatePayload.
        if (NeedTierClassifier.FromPayload(request.Payload, cardId) == NeedTier.Unknown)
        // Membuka scope cabang if untuk kondisi `NeedTierClassifier.FromPayload(request.Payload, cardId) == NeedTier.Unknown`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam ValidatePayload.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Tipe kebutuhan tidak
            // valid”`, `new ErrorDetail(”payload.need_tier”, ”INVALID_ENUM”)` kepada pemanggil dalam ValidatePayload; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Tipe kebutuhan tidak valid”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Tipe kebutuhan tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.need_tier”, ”INVALID_ENUM”) sebagai argumen ke
                // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”payload.need_tier”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai
                // literal `”INVALID_ENUM”` sebagai argumen ke konstruktor `ErrorDetail`.
                new ErrorDetail("payload.need_tier", "INVALID_ENUM"));
        // Menutup scope cabang if untuk kondisi `NeedTierClassifier.FromPayload(request.Payload, cardId) == NeedTier.Unknown`; bagian berikut berada di
        // luar batas blok tersebut dalam ValidatePayload.
        }

        // Mengembalikan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) kepada pemanggil dalam ValidatePayload;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return EventDomainValidationResult.Valid;
    // Menutup scope metode ValidatePayload; bagian berikut berada di luar batas blok tersebut dalam ValidatePayload.
    }

    // Mendefinisikan metode `Fail` dengan hasil bertipe `EventNeedPurchaseValidation`; operasi ini menangani fail. Masukan: Parameter `statusCode`
    // bertipe `int` membawa kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan; Parameter `errorCode` bertipe `string` membawa
    // nilai kesalahan kode; Parameter `message` bertipe `string` membawa nilai pesan; Parameter `details` bertipe `ErrorDetail[]` membawa nilai
    // rincian.
    private EventNeedPurchaseValidation Fail(
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
        // Mengembalikan objek baru bertipe `EventNeedPurchaseValidation` dengan argumen ( EventDomainValidationResult.Fail(statusCode, errorCode, message,
        // details), null) kepada pemanggil dalam Fail; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventNeedPurchaseValidation(
            // Meneruskan memanggil `EventDomainValidationResult.Fail` dengan `statusCode`, `errorCode`, `message`, `details` sebagai argumen ke konstruktor
            // `EventNeedPurchaseValidation`; Meneruskan `statusCode` (kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan) sebagai argumen
            // ke `EventDomainValidationResult.Fail`; Meneruskan `errorCode` (nilai kesalahan kode) sebagai argumen ke `EventDomainValidationResult.Fail`;
            // Meneruskan `message` (nilai pesan) sebagai argumen ke `EventDomainValidationResult.Fail`; Meneruskan `details` (nilai rincian) sebagai argumen ke
            // `EventDomainValidationResult.Fail`.
            EventDomainValidationResult.Fail(statusCode, errorCode, message, details),
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke konstruktor `EventNeedPurchaseValidation`.
            null);
    // Menutup scope metode Fail; bagian berikut berada di luar batas blok tersebut dalam Fail.
    }
// Menutup scope tipe EventNeedPurchaseValidator; bagian berikut berada di luar batas blok tersebut.
}
