// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventSimpleActionValidator.
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan tipe class `EventSimpleActionValidator` yang mewarisi atau menerapkan `IEventSimpleActionValidator`; sealed mencegah tipe ini
// diturunkan lagi.
internal sealed class EventSimpleActionValidator : IEventSimpleActionValidator
// Membuka scope tipe EventSimpleActionValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `EventPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly EventPayloadReader _payloadReader = new();

    // Mendefinisikan metode `TryValidate` dengan hasil bertipe `bool`; operasi ini menangani try validate. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `config` bertipe `RulesetConfig`
    // membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `result` bertipe `EventDomainValidationResult`
    // membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryValidate(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `result` bertipe `EventDomainValidationResult` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out EventDomainValidationResult result)
    // Membuka scope metode TryValidate; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
    {
        // Menyiapkan variabel lokal `actionType` untuk nilai aksi jenis dengan `request.ActionType` (nilai aksi jenis). Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var actionType = request.ActionType;
        // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan `request.Payload` (muatan detail event dalam format JSON).
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var payload = request.Payload;

        // Memeriksa membandingkan kesamaan `string` dengan `actionType`, `”KerjaLepas”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
        // mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (string.Equals(actionType, "KerjaLepas", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(actionType, ”KerjaLepas”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `ValidateFreelanceCompleted` dengan `request`, `payload`, `config.FreelanceIncome` dalam TryValidate.
            result = ValidateFreelanceCompleted(request, payload, config.FreelanceIncome);
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `string.Equals(actionType, ”KerjaLepas”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di
        // luar batas blok tersebut dalam TryValidate.
        }

        // Memeriksa membandingkan kesamaan `string` dengan `actionType`, `”PoinPeringkatDonasi”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
        // mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (string.Equals(actionType, "PoinPeringkatDonasi", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(actionType, ”PoinPeringkatDonasi”, StringComparison.OrdinalIgnoreCase)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `ValidateRankAwarded` dengan `request`, `payload`, `”Payload donasi tidak valid”` dalam TryValidate.
            result = ValidateRankAwarded(request, payload, "Payload donasi tidak valid");
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `string.Equals(actionType, ”PoinPeringkatDonasi”, StringComparison.OrdinalIgnoreCase)`; bagian berikut
        // berada di luar batas blok tersebut dalam TryValidate.
        }

        // Memeriksa membandingkan kesamaan `string` dengan `actionType`, `”UmumkanJuaraDonasi”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
        // mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (string.Equals(actionType, "UmumkanJuaraDonasi", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(actionType, ”UmumkanJuaraDonasi”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `ValidateDonationWinnersAnnouncement` dengan `request`, `payload` dalam TryValidate.
            result = ValidateDonationWinnersAnnouncement(request, payload);
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `string.Equals(actionType, ”UmumkanJuaraDonasi”, StringComparison.OrdinalIgnoreCase)`; bagian berikut
        // berada di luar batas blok tersebut dalam TryValidate.
        }

        // Memeriksa membandingkan kesamaan `string` dengan `actionType`, `”PoinEmas”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti
        // overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (string.Equals(actionType, "PoinEmas", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(actionType, ”PoinEmas”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `ValidateGoldPointsAwarded` dengan `request`, `payload` dalam TryValidate.
            result = ValidateGoldPointsAwarded(request, payload);
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `string.Equals(actionType, ”PoinEmas”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar
        // batas blok tersebut dalam TryValidate.
        }

        // Memeriksa membandingkan kesamaan `string` dengan `actionType`, `”PoinPeringkatPensiun”`, `StringComparison.OrdinalIgnoreCase`; aturan
        // perbandingan mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (string.Equals(actionType, "PoinPeringkatPensiun", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(actionType, ”PoinPeringkatPensiun”, StringComparison.OrdinalIgnoreCase)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `ValidateRankAwarded` dengan `request`, `payload`, `”Payload pension tidak valid”` dalam TryValidate.
            result = ValidateRankAwarded(request, payload, "Payload pension tidak valid");
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `string.Equals(actionType, ”PoinPeringkatPensiun”, StringComparison.OrdinalIgnoreCase)`; bagian berikut
        // berada di luar batas blok tersebut dalam TryValidate.
        }

        // Memperbarui `result` menggunakan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) dalam TryValidate.
        result = EventDomainValidationResult.Valid;
        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return false;
    // Menutup scope metode TryValidate; bagian berikut berada di luar batas blok tersebut dalam TryValidate.
    }

    // Mendefinisikan metode `ValidateFreelanceCompleted` dengan hasil bertipe `EventDomainValidationResult`; operasi ini menangani validate freelance
    // selesai. Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan;
    // Parameter `payload` bertipe `System.Text.Json.JsonElement` membawa muatan detail event dalam format JSON; Parameter `expectedIncome` bertipe
    // `int` membawa nilai yang diharapkan pemasukan.
    private EventDomainValidationResult ValidateFreelanceCompleted(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `payload` bertipe `System.Text.Json.JsonElement` membawa muatan detail event dalam format JSON.
        System.Text.Json.JsonElement payload,
        // Parameter `expectedIncome` bertipe `int` membawa nilai yang diharapkan pemasukan.
        int expectedIncome)
    // Membuka scope metode ValidateFreelanceCompleted; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateFreelanceCompleted.
    {
        // Menyiapkan variabel lokal `playerCheck` untuk nilai pemain check dengan memanggil `RequirePlayer` dengan `request`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerCheck = RequirePlayer(request);
        // Memeriksa kebalikan kondisi `playerCheck.IsValid`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateFreelanceCompleted.
        if (!playerCheck.IsValid)
        // Membuka scope cabang if untuk kondisi `!playerCheck.IsValid`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateFreelanceCompleted.
        {
            // Mengembalikan `playerCheck` (nilai pemain check) kepada pemanggil dalam ValidateFreelanceCompleted; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return playerCheck;
        // Menutup scope cabang if untuk kondisi `!playerCheck.IsValid`; bagian berikut berada di luar batas blok tersebut dalam ValidateFreelanceCompleted.
        }

        // Memeriksa kebalikan kondisi `_payloadReader.TryReadAmount(payload, out var amount)`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ValidateFreelanceCompleted.
        if (!_payloadReader.TryReadAmount(payload, out var amount))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadAmount(payload, out var amount)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam ValidateFreelanceCompleted.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Payload kerja lepas
            // tidak valid”`, `new ErrorDetail(”payload.amount”, ”REQUIRED”)` kepada pemanggil dalam ValidateFreelanceCompleted; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Payload kerja lepas tidak valid”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Payload kerja lepas tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.amount”, ”REQUIRED”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”payload.amount”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen
                // ke konstruktor `ErrorDetail`.
                new ErrorDetail("payload.amount", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadAmount(payload, out var amount)`; bagian berikut berada di luar batas blok tersebut
        // dalam ValidateFreelanceCompleted.
        }

        // Memeriksa pemeriksaan lebih kecil atau sama antara `amount` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateFreelanceCompleted.
        if (amount <= 0)
        // Membuka scope cabang if untuk kondisi `amount <= 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateFreelanceCompleted.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Amount harus > 0”`,
            // `new ErrorDetail(”payload.amount”, ”OUT_OF_RANGE”)` kepada pemanggil dalam ValidateFreelanceCompleted; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
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
        // Menutup scope cabang if untuk kondisi `amount <= 0`; bagian berikut berada di luar batas blok tersebut dalam ValidateFreelanceCompleted.
        }

        // Menyiapkan variabel lokal `rounded` untuk nilai rounded dengan hasil konversi `Math.Round(amount)` menjadi tipe `int`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var rounded = (int)Math.Round(amount);
        // Memeriksa perbandingan ketidaksamaan antara `rounded` dan `expectedIncome`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateFreelanceCompleted.
        if (rounded != expectedIncome)
        // Membuka scope cabang if untuk kondisi `rounded != expectedIncome`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateFreelanceCompleted.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Amount
            // kerja lepas tidak sesuai ruleset”` kepada pemanggil dalam ValidateFreelanceCompleted; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke
                // `EventDomainValidationResult.Fail`.
                StatusCodes.Status422UnprocessableEntity,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Amount kerja lepas tidak sesuai ruleset”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Amount kerja lepas tidak sesuai ruleset");
        // Menutup scope cabang if untuk kondisi `rounded != expectedIncome`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateFreelanceCompleted.
        }

        // Mengembalikan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) kepada pemanggil dalam
        // ValidateFreelanceCompleted; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return EventDomainValidationResult.Valid;
    // Menutup scope metode ValidateFreelanceCompleted; bagian berikut berada di luar batas blok tersebut dalam ValidateFreelanceCompleted.
    }

    // Mendefinisikan metode `ValidateRankAwarded` dengan hasil bertipe `EventDomainValidationResult`; operasi ini menangani validate rank awarded.
    // Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter
    // `payload` bertipe `System.Text.Json.JsonElement` membawa muatan detail event dalam format JSON; Parameter `invalidPayloadMessage` bertipe
    // `string` membawa nilai invalid payload pesan.
    private EventDomainValidationResult ValidateRankAwarded(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `payload` bertipe `System.Text.Json.JsonElement` membawa muatan detail event dalam format JSON.
        System.Text.Json.JsonElement payload,
        // Parameter `invalidPayloadMessage` bertipe `string` membawa nilai invalid payload pesan.
        string invalidPayloadMessage)
    // Membuka scope metode ValidateRankAwarded; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateRankAwarded.
    {
        // Menyiapkan variabel lokal `playerCheck` untuk nilai pemain check dengan memanggil `RequirePlayer` dengan `request`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerCheck = RequirePlayer(request);
        // Memeriksa kebalikan kondisi `playerCheck.IsValid`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateRankAwarded.
        if (!playerCheck.IsValid)
        // Membuka scope cabang if untuk kondisi `!playerCheck.IsValid`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateRankAwarded.
        {
            // Mengembalikan `playerCheck` (nilai pemain check) kepada pemanggil dalam ValidateRankAwarded; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return playerCheck;
        // Menutup scope cabang if untuk kondisi `!playerCheck.IsValid`; bagian berikut berada di luar batas blok tersebut dalam ValidateRankAwarded.
        }

        // Memeriksa kebalikan kondisi `_payloadReader.TryReadRankAwarded(payload, out var rank, out var points)`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ValidateRankAwarded.
        if (!_payloadReader.TryReadRankAwarded(payload, out var rank, out var points))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadRankAwarded(payload, out var rank, out var points)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam ValidateRankAwarded.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`,
            // `invalidPayloadMessage`, `new ErrorDetail(”payload.rank”, ”REQUIRED”)` kepada pemanggil dalam ValidateRankAwarded; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan `invalidPayloadMessage` (nilai invalid payload pesan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                invalidPayloadMessage,
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.rank”, ”REQUIRED”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”payload.rank”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke
                // konstruktor `ErrorDetail`.
                new ErrorDetail("payload.rank", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadRankAwarded(payload, out var rank, out var points)`; bagian berikut berada di luar
        // batas blok tersebut dalam ValidateRankAwarded.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `rank <= 0` dan `points < 0`; sisi kanan diperiksa hanya jika sisi kiri
        // salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateRankAwarded.
        if (rank <= 0 || points < 0)
        // Membuka scope cabang if untuk kondisi `rank <= 0 || points < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateRankAwarded.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Nilai rank/points
            // tidak valid”`, `new ErrorDetail(”payload.points”, ”OUT_OF_RANGE”)` kepada pemanggil dalam ValidateRankAwarded; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Nilai rank/points tidak valid”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Nilai rank/points tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.points”, ”OUT_OF_RANGE”) sebagai argumen ke
                // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”payload.points”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai
                // literal `”OUT_OF_RANGE”` sebagai argumen ke konstruktor `ErrorDetail`.
                new ErrorDetail("payload.points", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `rank <= 0 || points < 0`; bagian berikut berada di luar batas blok tersebut dalam ValidateRankAwarded.
        }

        // Mengembalikan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) kepada pemanggil dalam ValidateRankAwarded;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return EventDomainValidationResult.Valid;
    // Menutup scope metode ValidateRankAwarded; bagian berikut berada di luar batas blok tersebut dalam ValidateRankAwarded.
    }

    // Mendefinisikan metode `ValidateDonationWinnersAnnouncement` dengan hasil bertipe `EventDomainValidationResult`; operasi ini menangani validate
    // donasi winners announcement. Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau
    // diteruskan ke layanan; Parameter `payload` bertipe `System.Text.Json.JsonElement` membawa muatan detail event dalam format JSON.
    private static EventDomainValidationResult ValidateDonationWinnersAnnouncement(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `payload` bertipe `System.Text.Json.JsonElement` membawa muatan detail event dalam format JSON.
        System.Text.Json.JsonElement payload)
    // Membuka scope metode ValidateDonationWinnersAnnouncement; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ValidateDonationWinnersAnnouncement.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!string.Equals(request.ActorType, ”SYSTEM”,
        // StringComparison.OrdinalIgnoreCase)` dan `request.UserId is not null`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan
        // ketika kondisi ini bernilai benar dalam ValidateDonationWinnersAnnouncement.
        if (!string.Equals(request.ActorType, "SYSTEM", StringComparison.OrdinalIgnoreCase) ||
            // Menggunakan `request` (data masukan permintaan yang akan divalidasi atau diteruskan ke layanan) sebagai bagian ekspresi yang sedang disusun dalam
            // ValidateDonationWinnersAnnouncement.
            request.UserId is not null)
        // Membuka scope cabang if untuk kondisi `!string.Equals(request.ActorType, ”SYSTEM”, StringComparison.OrdinalIgnoreCase) || request.UserId is not
        // null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateDonationWinnersAnnouncement.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Pengumuman juara
            // donasi wajib dibuat oleh sistem”`, `new ErrorDetail(”actor_type”, ”SYSTEM_REQUIRED”)` kepada pemanggil dalam ValidateDonationWinnersAnnouncement;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Pengumuman juara donasi wajib dibuat oleh sistem”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Pengumuman juara donasi wajib dibuat oleh sistem",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”actor_type”, ”SYSTEM_REQUIRED”) sebagai argumen ke
                // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”actor_type”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai
                // literal `”SYSTEM_REQUIRED”` sebagai argumen ke konstruktor `ErrorDetail`.
                new ErrorDetail("actor_type", "SYSTEM_REQUIRED"));
        // Menutup scope cabang if untuk kondisi `!string.Equals(request.ActorType, ”SYSTEM”, StringComparison.OrdinalIgnoreCase) || request.UserId is not
        // null`; bagian berikut berada di luar batas blok tersebut dalam ValidateDonationWinnersAnnouncement.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `payload.ValueKind != System.Text.Json.JsonValueKind.Object ||
        // !payload.TryGetProperty(”winners”, out var winners)` dan `winners.ValueKind != System.Text.Json.JsonValueKind.Array`; sisi kanan diperiksa hanya
        // jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateDonationWinnersAnnouncement.
        if (payload.ValueKind != System.Text.Json.JsonValueKind.Object ||
            // Menggunakan kebalikan kondisi `payload.TryGetProperty(”winners”, out var winners)` sebagai bagian ekspresi yang sedang disusun dalam
            // ValidateDonationWinnersAnnouncement.
            !payload.TryGetProperty("winners", out var winners) ||
            // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `winners.ValueKind` dan `System.Text.Json.JsonValueKind.Array` dalam
            // ValidateDonationWinnersAnnouncement.
            winners.ValueKind != System.Text.Json.JsonValueKind.Array)
        // Membuka scope cabang if untuk kondisi `payload.ValueKind != System.Text.Json.JsonValueKind.Object || !payload.TryGetProperty(”winners”, out var
        // winners) || winners.ValueKind != System.Text.Json.JsonValueKind.Array`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateDonationWinnersAnnouncement.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Payload juara donasi
            // tidak valid”`, `new ErrorDetail(”payload.winners”, ”REQUIRED”)` kepada pemanggil dalam ValidateDonationWinnersAnnouncement; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Payload juara donasi tidak valid”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Payload juara donasi tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.winners”, ”REQUIRED”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”payload.winners”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen
                // ke konstruktor `ErrorDetail`.
                new ErrorDetail("payload.winners", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `payload.ValueKind != System.Text.Json.JsonValueKind.Object || !payload.TryGetProperty(”winners”, out var
        // winners) || winners.ValueKind != System.Text.Json.JsonValueKind.Array`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateDonationWinnersAnnouncement.
        }

        // Menyiapkan variabel lokal `winnerCount` untuk nilai winner jumlah dengan memanggil `winners.GetArrayLength` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var winnerCount = winners.GetArrayLength();
        // Memeriksa hasil pencocokan `winnerCount` dengan pola `< 1 or > 3`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateDonationWinnersAnnouncement.
        if (winnerCount is < 1 or > 3)
        // Membuka scope cabang if untuk kondisi `winnerCount is < 1 or > 3`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateDonationWinnersAnnouncement.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Jumlah juara donasi
            // harus 1 sampai 3”`, `new ErrorDetail(”payload.winners”, ”OUT_OF_RANGE”)` kepada pemanggil dalam ValidateDonationWinnersAnnouncement; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Jumlah juara donasi harus 1 sampai 3”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Jumlah juara donasi harus 1 sampai 3",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.winners”, ”OUT_OF_RANGE”) sebagai argumen ke
                // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”payload.winners”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai
                // literal `”OUT_OF_RANGE”` sebagai argumen ke konstruktor `ErrorDetail`.
                new ErrorDetail("payload.winners", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `winnerCount is < 1 or > 3`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateDonationWinnersAnnouncement.
        }

        // Menyiapkan variabel lokal `expectedRank` untuk nilai yang diharapkan rank dengan nilai literal `1`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var expectedRank = 1;
        // Mengulangi setiap elemen `winners.EnumerateArray()`; elemen saat ini disimpan sebagai `winner` bertipe `var` untuk diproses oleh badan loop dalam
        // ValidateDonationWinnersAnnouncement.
        foreach (var winner in winners.EnumerateArray())
        // Membuka scope loop setiap winner dari `winners.EnumerateArray()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateDonationWinnersAnnouncement.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `winner.ValueKind != System.Text.Json.JsonValueKind.Object ||
            // !winner.TryGetProperty(”rank”, out var rankProperty) || !rankProperty.TryGetInt32(out var rank)` dan `rank != expectedRank`; sisi kanan diperiksa
            // hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateDonationWinnersAnnouncement.
            if (winner.ValueKind != System.Text.Json.JsonValueKind.Object ||
                // Menggunakan kebalikan kondisi `winner.TryGetProperty(”rank”, out var rankProperty)` sebagai bagian ekspresi yang sedang disusun dalam
                // ValidateDonationWinnersAnnouncement.
                !winner.TryGetProperty("rank", out var rankProperty) ||
                // Menggunakan kebalikan kondisi `rankProperty.TryGetInt32(out var rank)` sebagai bagian ekspresi yang sedang disusun dalam
                // ValidateDonationWinnersAnnouncement.
                !rankProperty.TryGetInt32(out var rank) ||
                // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `rank` dan `expectedRank` dalam ValidateDonationWinnersAnnouncement.
                rank != expectedRank)
            // Membuka scope cabang if untuk kondisi `winner.ValueKind != System.Text.Json.JsonValueKind.Object || !winner.TryGetProperty(”rank”, out var
            // rankProperty) || !rankProperty.TryGetInt32(out var rank) || rank != expecte...`; pernyataan/deklarasi berikut berada di dalam batas blok ini
            // dalam ValidateDonationWinnersAnnouncement.
            {
                // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Urutan rank juara
                // donasi tidak valid”`, `new ErrorDetail(”payload.winners”, ”INVALID_RANK_SEQUENCE”)` kepada pemanggil dalam ValidateDonationWinnersAnnouncement;
                // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return EventDomainValidationResult.Fail(
                    // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                    StatusCodes.Status400BadRequest,
                    // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "VALIDATION_ERROR",
                    // Meneruskan nilai literal `”Urutan rank juara donasi tidak valid”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "Urutan rank juara donasi tidak valid",
                    // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.winners”, ”INVALID_RANK_SEQUENCE”) sebagai argumen ke
                    // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”payload.winners”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai
                    // literal `”INVALID_RANK_SEQUENCE”` sebagai argumen ke konstruktor `ErrorDetail`.
                    new ErrorDetail("payload.winners", "INVALID_RANK_SEQUENCE"));
            // Menutup scope cabang if untuk kondisi `winner.ValueKind != System.Text.Json.JsonValueKind.Object || !winner.TryGetProperty(”rank”, out var
            // rankProperty) || !rankProperty.TryGetInt32(out var rank) || rank != expecte...`; bagian berikut berada di luar batas blok tersebut dalam
            // ValidateDonationWinnersAnnouncement.
            }

            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!winner.TryGetProperty(”player_name”, out var playerNameProperty) ||
            // playerNameProperty.ValueKind != System.Text.Json.JsonValueKind.String` dan `string.IsNullOrWhiteSpace(playerNameProperty.GetString())`; sisi
            // kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateDonationWinnersAnnouncement.
            if (!winner.TryGetProperty("player_name", out var playerNameProperty) ||
                // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `playerNameProperty.ValueKind` dan `System.Text.Json.JsonValueKind.String` dalam
                // ValidateDonationWinnersAnnouncement.
                playerNameProperty.ValueKind != System.Text.Json.JsonValueKind.String ||
                // Melanjutkan pengolahan dengan memeriksa apakah `playerNameProperty.GetString()` null, kosong, atau hanya berisi karakter spasi dalam
                // ValidateDonationWinnersAnnouncement.
                string.IsNullOrWhiteSpace(playerNameProperty.GetString()))
            // Membuka scope cabang if untuk kondisi `!winner.TryGetProperty(”player_name”, out var playerNameProperty) || playerNameProperty.ValueKind !=
            // System.Text.Json.JsonValueKind.String || string.IsNullOrWhiteSpace(playerN...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateDonationWinnersAnnouncement.
            {
                // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Nama pemain juara
                // donasi wajib diisi”`, `new ErrorDetail(”payload.winners.player_name”, ”REQUIRED”)` kepada pemanggil dalam ValidateDonationWinnersAnnouncement;
                // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return EventDomainValidationResult.Fail(
                    // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                    StatusCodes.Status400BadRequest,
                    // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "VALIDATION_ERROR",
                    // Meneruskan nilai literal `”Nama pemain juara donasi wajib diisi”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "Nama pemain juara donasi wajib diisi",
                    // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.winners.player_name”, ”REQUIRED”) sebagai argumen ke
                    // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”payload.winners.player_name”` sebagai argumen ke konstruktor `ErrorDetail`;
                    // Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke konstruktor `ErrorDetail`.
                    new ErrorDetail("payload.winners.player_name", "REQUIRED"));
            // Menutup scope cabang if untuk kondisi `!winner.TryGetProperty(”player_name”, out var playerNameProperty) || playerNameProperty.ValueKind !=
            // System.Text.Json.JsonValueKind.String || string.IsNullOrWhiteSpace(playerN...`; bagian berikut berada di luar batas blok tersebut dalam
            // ValidateDonationWinnersAnnouncement.
            }

            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!winner.TryGetProperty(”points”, out var pointsProperty) ||
            // !pointsProperty.TryGetInt32(out var points)` dan `points < 0`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika
            // kondisi ini bernilai benar dalam ValidateDonationWinnersAnnouncement.
            if (!winner.TryGetProperty("points", out var pointsProperty) ||
                // Menggunakan kebalikan kondisi `pointsProperty.TryGetInt32(out var points)` sebagai bagian ekspresi yang sedang disusun dalam
                // ValidateDonationWinnersAnnouncement.
                !pointsProperty.TryGetInt32(out var points) ||
                // Melanjutkan ekspresi dengan pemeriksaan lebih kecil antara `points` dan `0` dalam ValidateDonationWinnersAnnouncement.
                points < 0)
            // Membuka scope cabang if untuk kondisi `!winner.TryGetProperty(”points”, out var pointsProperty) || !pointsProperty.TryGetInt32(out var points) ||
            // points < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateDonationWinnersAnnouncement.
            {
                // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Poin juara donasi
                // tidak valid”`, `new ErrorDetail(”payload.winners.points”, ”OUT_OF_RANGE”)` kepada pemanggil dalam ValidateDonationWinnersAnnouncement; eksekusi
                // jalur ini selesai setelah nilai hasil ditentukan.
                return EventDomainValidationResult.Fail(
                    // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                    StatusCodes.Status400BadRequest,
                    // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "VALIDATION_ERROR",
                    // Meneruskan nilai literal `”Poin juara donasi tidak valid”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "Poin juara donasi tidak valid",
                    // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.winners.points”, ”OUT_OF_RANGE”) sebagai argumen ke
                    // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”payload.winners.points”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan
                    // nilai literal `”OUT_OF_RANGE”` sebagai argumen ke konstruktor `ErrorDetail`.
                    new ErrorDetail("payload.winners.points", "OUT_OF_RANGE"));
            // Menutup scope cabang if untuk kondisi `!winner.TryGetProperty(”points”, out var pointsProperty) || !pointsProperty.TryGetInt32(out var points) ||
            // points < 0`; bagian berikut berada di luar batas blok tersebut dalam ValidateDonationWinnersAnnouncement.
            }

            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `winner.TryGetProperty(”player_order_no”, out var playerOrderProperty)` dan
            // `(!playerOrderProperty.TryGetInt32(out var playerOrderNo) || playerOrderNo <= 0)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya
            // dijalankan ketika kondisi ini bernilai benar dalam ValidateDonationWinnersAnnouncement.
            if (winner.TryGetProperty("player_order_no", out var playerOrderProperty) &&
                // Menggunakan gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!playerOrderProperty.TryGetInt32(out var playerOrderNo)` dan
                // `playerOrderNo <= 0`; sisi kanan diperiksa hanya jika sisi kiri salah sebagai bagian ekspresi yang sedang disusun dalam
                // ValidateDonationWinnersAnnouncement.
                (!playerOrderProperty.TryGetInt32(out var playerOrderNo) || playerOrderNo <= 0))
            // Membuka scope cabang if untuk kondisi `winner.TryGetProperty(”player_order_no”, out var playerOrderProperty) &&
            // (!playerOrderProperty.TryGetInt32(out var playerOrderNo) || playerOrderNo <= 0)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
            // dalam ValidateDonationWinnersAnnouncement.
            {
                // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Urutan pemain juara
                // donasi tidak valid”`, `new ErrorDetail(”payload.winners.player_order_no”, ”OUT_OF_RANGE”)` kepada pemanggil dalam
                // ValidateDonationWinnersAnnouncement; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return EventDomainValidationResult.Fail(
                    // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                    StatusCodes.Status400BadRequest,
                    // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "VALIDATION_ERROR",
                    // Meneruskan nilai literal `”Urutan pemain juara donasi tidak valid”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "Urutan pemain juara donasi tidak valid",
                    // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.winners.player_order_no”, ”OUT_OF_RANGE”) sebagai argumen ke
                    // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”payload.winners.player_order_no”` sebagai argumen ke konstruktor `ErrorDetail`;
                    // Meneruskan nilai literal `”OUT_OF_RANGE”` sebagai argumen ke konstruktor `ErrorDetail`.
                    new ErrorDetail("payload.winners.player_order_no", "OUT_OF_RANGE"));
            // Menutup scope cabang if untuk kondisi `winner.TryGetProperty(”player_order_no”, out var playerOrderProperty) &&
            // (!playerOrderProperty.TryGetInt32(out var playerOrderNo) || playerOrderNo <= 0)`; bagian berikut berada di luar batas blok tersebut dalam
            // ValidateDonationWinnersAnnouncement.
            }

            // Menjalankan `expectedRank++` dalam ValidateDonationWinnersAnnouncement.
            expectedRank++;
        // Menutup scope loop setiap winner dari `winners.EnumerateArray()`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateDonationWinnersAnnouncement.
        }

        // Mengembalikan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) kepada pemanggil dalam
        // ValidateDonationWinnersAnnouncement; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return EventDomainValidationResult.Valid;
    // Menutup scope metode ValidateDonationWinnersAnnouncement; bagian berikut berada di luar batas blok tersebut dalam
    // ValidateDonationWinnersAnnouncement.
    }

    // Mendefinisikan metode `ValidateGoldPointsAwarded` dengan hasil bertipe `EventDomainValidationResult`; operasi ini menangani validate emas poin
    // awarded. Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan;
    // Parameter `payload` bertipe `System.Text.Json.JsonElement` membawa muatan detail event dalam format JSON.
    private EventDomainValidationResult ValidateGoldPointsAwarded(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `payload` bertipe `System.Text.Json.JsonElement` membawa muatan detail event dalam format JSON.
        System.Text.Json.JsonElement payload)
    // Membuka scope metode ValidateGoldPointsAwarded; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldPointsAwarded.
    {
        // Menyiapkan variabel lokal `playerCheck` untuk nilai pemain check dengan memanggil `RequirePlayer` dengan `request`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerCheck = RequirePlayer(request);
        // Memeriksa kebalikan kondisi `playerCheck.IsValid`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoldPointsAwarded.
        if (!playerCheck.IsValid)
        // Membuka scope cabang if untuk kondisi `!playerCheck.IsValid`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateGoldPointsAwarded.
        {
            // Mengembalikan `playerCheck` (nilai pemain check) kepada pemanggil dalam ValidateGoldPointsAwarded; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return playerCheck;
        // Menutup scope cabang if untuk kondisi `!playerCheck.IsValid`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldPointsAwarded.
        }

        // Memeriksa kebalikan kondisi `_payloadReader.TryReadPointsAwarded(payload, out var points)`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam ValidateGoldPointsAwarded.
        if (!_payloadReader.TryReadPointsAwarded(payload, out var points))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadPointsAwarded(payload, out var points)`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam ValidateGoldPointsAwarded.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Payload gold points
            // tidak valid”`, `new ErrorDetail(”payload.points”, ”REQUIRED”)` kepada pemanggil dalam ValidateGoldPointsAwarded; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Payload gold points tidak valid”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Payload gold points tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.points”, ”REQUIRED”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”payload.points”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen
                // ke konstruktor `ErrorDetail`.
                new ErrorDetail("payload.points", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadPointsAwarded(payload, out var points)`; bagian berikut berada di luar batas blok
        // tersebut dalam ValidateGoldPointsAwarded.
        }

        // Memeriksa pemeriksaan lebih kecil antara `points` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateGoldPointsAwarded.
        if (points < 0)
        // Membuka scope cabang if untuk kondisi `points < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoldPointsAwarded.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Points tidak
            // valid”`, `new ErrorDetail(”payload.points”, ”OUT_OF_RANGE”)` kepada pemanggil dalam ValidateGoldPointsAwarded; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
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
        // Menutup scope cabang if untuk kondisi `points < 0`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldPointsAwarded.
        }

        // Mengembalikan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) kepada pemanggil dalam
        // ValidateGoldPointsAwarded; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return EventDomainValidationResult.Valid;
    // Menutup scope metode ValidateGoldPointsAwarded; bagian berikut berada di luar batas blok tersebut dalam ValidateGoldPointsAwarded.
    }

    // Mendefinisikan metode `RequirePlayer` dengan hasil bertipe `EventDomainValidationResult`; operasi ini menangani require pemain. Masukan:
    // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
    private EventDomainValidationResult RequirePlayer(EventRequest request)
    // Membuka scope metode RequirePlayer; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RequirePlayer.
    {
        // Memeriksa hasil pencocokan `request.UserId` dengan pola `not null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // RequirePlayer.
        if (request.UserId is not null)
        // Membuka scope cabang if untuk kondisi `request.UserId is not null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // RequirePlayer.
        {
            // Mengembalikan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) kepada pemanggil dalam RequirePlayer; eksekusi
            // jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Valid;
        // Menutup scope cabang if untuk kondisi `request.UserId is not null`; bagian berikut berada di luar batas blok tersebut dalam RequirePlayer.
        }

        // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Player wajib
        // diisi”`, `new ErrorDetail(”user_id”, ”REQUIRED”)` kepada pemanggil dalam RequirePlayer; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
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
    // Menutup scope metode RequirePlayer; bagian berikut berada di luar batas blok tersebut dalam RequirePlayer.
    }
// Menutup scope tipe EventSimpleActionValidator; bagian berikut berada di luar batas blok tersebut.
}
