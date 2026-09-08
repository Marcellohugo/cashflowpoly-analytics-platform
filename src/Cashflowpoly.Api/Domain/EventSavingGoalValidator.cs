// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventSavingGoalValidator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `EventSavingGoalValidation`; sealed mencegah tipe ini diturunkan lagi.
public sealed record EventSavingGoalValidation(
    // Parameter `Validation` bertipe `EventDomainValidationResult` membawa nilai validasi.
    EventDomainValidationResult Validation,
    // Parameter `OutgoingAmount` bertipe `int?` membawa nilai outgoing nominal; nilai null diizinkan ketika data opsional belum tersedia.
    int? OutgoingAmount);

// Mendefinisikan tipe class `EventSavingGoalValidator` yang mewarisi atau menerapkan `IEventSavingGoalValidator`; sealed mencegah tipe ini
// diturunkan lagi.
internal sealed class EventSavingGoalValidator : IEventSavingGoalValidator
// Membuka scope tipe EventSavingGoalValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `EventPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly EventPayloadReader _payloadReader = new();
    // Mendeklarasikan field bertipe `EventDerivedStateCalculator`: `_derivedState` menyimpan nilai derived keadaan dengan nilai awal objek baru dengan
    // tipe mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static
    // membuat field menjadi milik tipe dan dibagikan antar instance.
    private static readonly EventDerivedStateCalculator _derivedState = new();
    // Mendeklarasikan field bertipe `int`: `RulebookSavingMaxDeposit` menyimpan nilai rulebook tabungan maksimum deposit dengan nilai awal nilai
    // literal `15`.
    private const int RulebookSavingMaxDeposit = 15;

    // Mendefinisikan metode `TryValidate` dengan hasil bertipe `bool`; operasi ini menangani try validate. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `config` bertipe `RulesetConfig`
    // membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe `IEnumerable<EventDb>` membawa
    // nilai history; Parameter `result` bertipe `EventSavingGoalValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryValidate(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history,
        // Parameter `result` bertipe `EventSavingGoalValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out mengembalikan
        // nilai melalui parameter dan harus diisi oleh metode.
        out EventSavingGoalValidation result)
    // Membuka scope metode TryValidate; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
    {
        // Memeriksa membandingkan kesamaan `string` dengan `request.ActionType`, `”TarikTabungan”`, `StringComparison.OrdinalIgnoreCase`; aturan
        // perbandingan mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (string.Equals(request.ActionType, "TarikTabungan", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(request.ActionType, ”TarikTabungan”, StringComparison.OrdinalIgnoreCase)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”TarikTabungan
            // bukan aksi resmi ruleset rulebook”` dalam TryValidate.
            result = Fail(
                // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke `Fail`.
                StatusCodes.Status422UnprocessableEntity,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `Fail`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”TarikTabungan bukan aksi resmi ruleset rulebook”` sebagai argumen ke `Fail`.
                "TarikTabungan bukan aksi resmi ruleset rulebook");
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `string.Equals(request.ActionType, ”TarikTabungan”, StringComparison.OrdinalIgnoreCase)`; bagian berikut
        // berada di luar batas blok tersebut dalam TryValidate.
        }

        // Memeriksa membandingkan kesamaan `string` dengan `request.ActionType`, `”Menabung”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
        // mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (string.Equals(request.ActionType, "Menabung", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(request.ActionType, ”Menabung”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `ValidateDeposit` dengan `request`, `config`, `history` dalam TryValidate.
            result = ValidateDeposit(request, config, history);
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `string.Equals(request.ActionType, ”Menabung”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada
        // di luar batas blok tersebut dalam TryValidate.
        }

        // Memeriksa membandingkan kesamaan `string` dengan `request.ActionType`, `”TujuanFinansial”`, `StringComparison.OrdinalIgnoreCase`; aturan
        // perbandingan mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (string.Equals(request.ActionType, "TujuanFinansial", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(request.ActionType, ”TujuanFinansial”, StringComparison.OrdinalIgnoreCase)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Memeriksa membandingkan kesamaan `string` dengan `request.ActorType`, `”PLAYER”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
            // mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
            if (string.Equals(request.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi
            // berikut berada di dalam batas blok ini dalam TryValidate.
            {
                // Memperbarui `result` menggunakan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”TujuanFinansial
                // bukan aksi pemain terpisah; kartu tujuan diperoleh otomatis saat Menabung mencapai target”` dalam TryValidate.
                result = Fail(
                    // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke `Fail`.
                    StatusCodes.Status422UnprocessableEntity,
                    // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `Fail`.
                    "DOMAIN_RULE_VIOLATION",
                    // Meneruskan nilai literal `”TujuanFinansial bukan aksi pemain terpisah; kartu tujuan diperoleh otomatis saat Menabung mencapai target”` sebagai
                    // argumen ke `Fail`.
                    "TujuanFinansial bukan aksi pemain terpisah; kartu tujuan diperoleh otomatis saat Menabung mencapai target");
                // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return true;
            // Menutup scope cabang if untuk kondisi `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di
            // luar batas blok tersebut dalam TryValidate.
            }

            // Memperbarui `result` menggunakan memanggil `ValidateGoalAchieved` dengan `request`, `config`, `history` dalam TryValidate.
            result = ValidateGoalAchieved(request, config, history);
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `string.Equals(request.ActionType, ”TujuanFinansial”, StringComparison.OrdinalIgnoreCase)`; bagian berikut
        // berada di luar batas blok tersebut dalam TryValidate.
        }

        // Memperbarui `result` menggunakan objek baru bertipe `EventSavingGoalValidation` dengan argumen (EventDomainValidationResult.Valid, null) dalam
        // TryValidate.
        result = new EventSavingGoalValidation(EventDomainValidationResult.Valid, null);
        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return false;
    // Menutup scope metode TryValidate; bagian berikut berada di luar batas blok tersebut dalam TryValidate.
    }

    // Mendefinisikan metode `ValidateDeposit` dengan hasil bertipe `EventSavingGoalValidation`; operasi ini menangani validate deposit. Masukan:
    // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `config`
    // bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe
    // `IEnumerable<EventDb>` membawa nilai history.
    private EventSavingGoalValidation ValidateDeposit(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history)
    // Membuka scope metode ValidateDeposit; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateDeposit.
    {
        // Menyiapkan variabel lokal `featureValidation` untuk nilai feature validasi dengan memanggil `ValidateFeatureAndPlayer` dengan `request`,
        // `config`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var featureValidation = ValidateFeatureAndPlayer(request, config);
        // Memeriksa kebalikan kondisi `featureValidation.IsValid`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateDeposit.
        if (!featureValidation.IsValid)
        // Membuka scope cabang if untuk kondisi `!featureValidation.IsValid`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateDeposit.
        {
            // Mengembalikan objek baru bertipe `EventSavingGoalValidation` dengan argumen (featureValidation, null) kepada pemanggil dalam ValidateDeposit;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return new EventSavingGoalValidation(featureValidation, null);
        // Menutup scope cabang if untuk kondisi `!featureValidation.IsValid`; bagian berikut berada di luar batas blok tersebut dalam ValidateDeposit.
        }

        // Memeriksa kebalikan kondisi `_payloadReader.TryReadSavingDeposit(request.Payload, out var goalId, out var amount)`; blok if hanya dijalankan
        // ketika kondisi ini bernilai benar dalam ValidateDeposit.
        if (!_payloadReader.TryReadSavingDeposit(request.Payload, out var goalId, out var amount))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadSavingDeposit(request.Payload, out var goalId, out var amount)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateDeposit.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Payload tabungan tidak valid”`, `new
            // ErrorDetail(”payload.amount”, ”REQUIRED”)` kepada pemanggil dalam ValidateDeposit; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Payload tabungan tidak valid”` sebagai argumen ke `Fail`.
                "Payload tabungan tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.amount”, ”REQUIRED”) sebagai argumen ke `Fail`; Meneruskan nilai literal
                // `”payload.amount”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke konstruktor
                // `ErrorDetail`.
                new ErrorDetail("payload.amount", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadSavingDeposit(request.Payload, out var goalId, out var amount)`; bagian berikut
        // berada di luar batas blok tersebut dalam ValidateDeposit.
        }

        // Menyiapkan variabel lokal `payloadValidation` untuk nilai payload validasi dengan memanggil `ValidateSavingDepositPayload` dengan `goalId`,
        // `amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var payloadValidation = ValidateSavingDepositPayload(goalId, amount);
        // Memeriksa kebalikan kondisi `payloadValidation.IsValid`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateDeposit.
        if (!payloadValidation.IsValid)
        // Membuka scope cabang if untuk kondisi `!payloadValidation.IsValid`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateDeposit.
        {
            // Mengembalikan objek baru bertipe `EventSavingGoalValidation` dengan argumen (payloadValidation, null) kepada pemanggil dalam ValidateDeposit;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return new EventSavingGoalValidation(payloadValidation, null);
        // Menutup scope cabang if untuk kondisi `!payloadValidation.IsValid`; bagian berikut berada di luar batas blok tersebut dalam ValidateDeposit.
        }

        // Memeriksa memeriksa apakah seluruh elemen `config.FinancialGoals` memenuhi `goal => !string.Equals(goal.Id, goalId,
        // StringComparison.OrdinalIgnoreCase)`; koleksi kosong menghasilkan true; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateDeposit.
        if (config.FinancialGoals.All(goal => !string.Equals(goal.Id, goalId, StringComparison.OrdinalIgnoreCase)))
        // Membuka scope cabang if untuk kondisi `config.FinancialGoals.All(goal => !string.Equals(goal.Id, goalId, StringComparison.OrdinalIgnoreCase))`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateDeposit.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Kartu Tujuan Finansial tidak
            // ditemukan pada ruleset”` kepada pemanggil dalam ValidateDeposit; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Kartu Tujuan Finansial tidak ditemukan pada ruleset”` sebagai argumen ke `Fail`.
                "Kartu Tujuan Finansial tidak ditemukan pada ruleset");
        // Menutup scope cabang if untuk kondisi `config.FinancialGoals.All(goal => !string.Equals(goal.Id, goalId, StringComparison.OrdinalIgnoreCase))`;
        // bagian berikut berada di luar batas blok tersebut dalam ValidateDeposit.
        }

        // Menyiapkan variabel lokal `isCreate` untuk nilai berstatus create dengan membandingkan kesamaan `string` dengan `request.ActionType`,
        // `”Menabung”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan comparer yang diberikan. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var isCreate = string.Equals(request.ActionType, "Menabung", StringComparison.OrdinalIgnoreCase);
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `isCreate` dan `amount > RulebookSavingMaxDeposit`; sisi kanan diperiksa hanya
        // jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateDeposit.
        if (isCreate && amount > RulebookSavingMaxDeposit)
        // Membuka scope cabang if untuk kondisi `isCreate && amount > RulebookSavingMaxDeposit`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ValidateDeposit.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Maksimal tabungan per aksi adalah
            // 15 koin”` kepada pemanggil dalam ValidateDeposit; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Maksimal tabungan per aksi adalah 15 koin");
        // Menutup scope cabang if untuk kondisi `isCreate && amount > RulebookSavingMaxDeposit`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateDeposit.
        }

        // Menyiapkan variabel lokal `balance` untuk saldo uang pemain pada keadaan yang sedang diproses dengan memanggil
        // `_derivedState.ComputeSavingBalance` dengan `history`, `request.UserId!.Value`, `goalId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var balance = _derivedState.ComputeSavingBalance(history, request.UserId!.Value, goalId);
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!isCreate` dan `balance < amount`; sisi kanan diperiksa hanya jika sisi kiri
        // benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateDeposit.
        if (!isCreate && balance < amount)
        // Membuka scope cabang if untuk kondisi `!isCreate && balance < amount`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateDeposit.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Saldo tabungan tidak mencukupi”`
            // kepada pemanggil dalam ValidateDeposit; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Saldo tabungan tidak mencukupi");
        // Menutup scope cabang if untuk kondisi `!isCreate && balance < amount`; bagian berikut berada di luar batas blok tersebut dalam ValidateDeposit.
        }

        // Mengembalikan objek baru bertipe `EventSavingGoalValidation` dengan argumen (EventDomainValidationResult.Valid, isCreate ? amount : null) kepada
        // pemanggil dalam ValidateDeposit; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventSavingGoalValidation(EventDomainValidationResult.Valid, isCreate ? amount : null);
    // Menutup scope metode ValidateDeposit; bagian berikut berada di luar batas blok tersebut dalam ValidateDeposit.
    }

    // Mendefinisikan metode `ValidateGoalAchieved` dengan hasil bertipe `EventSavingGoalValidation`; operasi ini menangani validate target achieved.
    // Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter
    // `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe
    // `IEnumerable<EventDb>` membawa nilai history.
    private EventSavingGoalValidation ValidateGoalAchieved(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history)
    // Membuka scope metode ValidateGoalAchieved; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoalAchieved.
    {
        // Menyiapkan variabel lokal `featureValidation` untuk nilai feature validasi dengan memanggil `ValidateFeatureAndPlayer` dengan `request`,
        // `config`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var featureValidation = ValidateFeatureAndPlayer(request, config);
        // Memeriksa kebalikan kondisi `featureValidation.IsValid`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoalAchieved.
        if (!featureValidation.IsValid)
        // Membuka scope cabang if untuk kondisi `!featureValidation.IsValid`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateGoalAchieved.
        {
            // Mengembalikan objek baru bertipe `EventSavingGoalValidation` dengan argumen (featureValidation, null) kepada pemanggil dalam
            // ValidateGoalAchieved; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return new EventSavingGoalValidation(featureValidation, null);
        // Menutup scope cabang if untuk kondisi `!featureValidation.IsValid`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoalAchieved.
        }

        // Memeriksa kebalikan kondisi `_payloadReader.TryReadSavingGoalAchieved(request.Payload, out var goalId, out var points, out var cost)`; blok if
        // hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoalAchieved.
        if (!_payloadReader.TryReadSavingGoalAchieved(request.Payload, out var goalId, out var points, out var cost))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadSavingGoalAchieved(request.Payload, out var goalId, out var points, out var cost)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoalAchieved.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Payload goal tidak valid”`, `new
            // ErrorDetail(”payload.goal_id”, ”REQUIRED”)` kepada pemanggil dalam ValidateGoalAchieved; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Payload goal tidak valid”` sebagai argumen ke `Fail`.
                "Payload goal tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.goal_id”, ”REQUIRED”) sebagai argumen ke `Fail`; Meneruskan nilai literal
                // `”payload.goal_id”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke konstruktor
                // `ErrorDetail`.
                new ErrorDetail("payload.goal_id", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadSavingGoalAchieved(request.Payload, out var goalId, out var points, out var cost)`;
        // bagian berikut berada di luar batas blok tersebut dalam ValidateGoalAchieved.
        }

        // Memeriksa pemeriksaan lebih kecil antara `points` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoalAchieved.
        if (points < 0)
        // Membuka scope cabang if untuk kondisi `points < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoalAchieved.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Points tidak valid”`, `new
            // ErrorDetail(”payload.points”, ”OUT_OF_RANGE”)` kepada pemanggil dalam ValidateGoalAchieved; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Points tidak valid”` sebagai argumen ke `Fail`.
                "Points tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.points”, ”OUT_OF_RANGE”) sebagai argumen ke `Fail`; Meneruskan nilai literal
                // `”payload.points”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”OUT_OF_RANGE”` sebagai argumen ke konstruktor
                // `ErrorDetail`.
                new ErrorDetail("payload.points", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `points < 0`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoalAchieved.
        }

        // Memeriksa pemeriksaan lebih kecil antara `cost` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoalAchieved.
        if (cost < 0)
        // Membuka scope cabang if untuk kondisi `cost < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoalAchieved.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Cost tidak valid”`, `new
            // ErrorDetail(”payload.cost”, ”OUT_OF_RANGE”)` kepada pemanggil dalam ValidateGoalAchieved; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Cost tidak valid”` sebagai argumen ke `Fail`.
                "Cost tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.cost”, ”OUT_OF_RANGE”) sebagai argumen ke `Fail`; Meneruskan nilai literal
                // `”payload.cost”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”OUT_OF_RANGE”` sebagai argumen ke konstruktor
                // `ErrorDetail`.
                new ErrorDetail("payload.cost", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `cost < 0`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoalAchieved.
        }

        // Menyiapkan variabel lokal `goal` untuk nilai target dengan mengambil elemen pertama `config.FinancialGoals` yang sesuai `item =>
        // string.Equals(item.Id, goalId, StringComparison.OrdinalIgnoreCase)`; jika tidak ada, gunakan nilai default tipe hasil. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var goal = config.FinancialGoals.FirstOrDefault(item =>
            // Meneruskan `item.Id` (nilai identitas) sebagai argumen ke `string.Equals`; Meneruskan `goalId` (nilai target identitas) sebagai argumen ke
            // `string.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke `string.Equals`.
            string.Equals(item.Id, goalId, StringComparison.OrdinalIgnoreCase));
        // Memeriksa hasil pencocokan `goal` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoalAchieved.
        if (goal is null)
        // Membuka scope cabang if untuk kondisi `goal is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateGoalAchieved.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Kartu Tujuan Finansial tidak
            // ditemukan pada ruleset”` kepada pemanggil dalam ValidateGoalAchieved; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Kartu Tujuan Finansial tidak ditemukan pada ruleset”` sebagai argumen ke `Fail`.
                "Kartu Tujuan Finansial tidak ditemukan pada ruleset");
        // Menutup scope cabang if untuk kondisi `goal is null`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoalAchieved.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `cost != goal.HargaBeli` dan `points != goal.PoinKebahagiaan`; sisi
        // kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoalAchieved.
        if (cost != goal.HargaBeli || points != goal.PoinKebahagiaan)
        // Membuka scope cabang if untuk kondisi `cost != goal.HargaBeli || points != goal.PoinKebahagiaan`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam ValidateGoalAchieved.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Biaya atau poin tujuan tidak
            // sesuai katalog ruleset”` kepada pemanggil dalam ValidateGoalAchieved; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Biaya atau poin tujuan tidak sesuai katalog ruleset”` sebagai argumen ke `Fail`.
                "Biaya atau poin tujuan tidak sesuai katalog ruleset");
        // Menutup scope cabang if untuk kondisi `cost != goal.HargaBeli || points != goal.PoinKebahagiaan`; bagian berikut berada di luar batas blok
        // tersebut dalam ValidateGoalAchieved.
        }

        // Menyiapkan variabel lokal `completedCount` untuk nilai selesai jumlah dengan memanggil `history.Count` dengan `evt =>
        // GameActionCatalog.Is(evt.ActionType, _payloadReader.ReadPayload(evt.Payload), GameActionCatalog.TujuanFinansial) &&
        // _payloadReader.TryReadSavingGoalAchieved(_payloadRea...`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var completedCount = history.Count(evt =>
            // Meneruskan `evt.ActionType` (nilai aksi jenis) sebagai argumen ke `GameActionCatalog.Is`; Meneruskan memanggil `_payloadReader.ReadPayload`
            // dengan `evt.Payload` sebagai argumen ke `GameActionCatalog.Is`; Meneruskan `evt.Payload` (muatan detail event dalam format JSON) sebagai argumen
            // ke `_payloadReader.ReadPayload`; Meneruskan `GameActionCatalog.TujuanFinansial` (nilai tujuan finansial) sebagai argumen ke
            // `GameActionCatalog.Is`.
            GameActionCatalog.Is(evt.ActionType, _payloadReader.ReadPayload(evt.Payload), GameActionCatalog.TujuanFinansial) &&
            // Meneruskan memanggil `_payloadReader.ReadPayload` dengan `evt.Payload` sebagai argumen ke `_payloadReader.TryReadSavingGoalAchieved`; Meneruskan
            // `evt.Payload` (muatan detail event dalam format JSON) sebagai argumen ke `_payloadReader.ReadPayload`; Meneruskan `var completedGoalId` sebagai
            // argumen ke `_payloadReader.TryReadSavingGoalAchieved`; Meneruskan `_` (nilai ) sebagai argumen ke `_payloadReader.TryReadSavingGoalAchieved`;
            // Meneruskan `_` (nilai ) sebagai argumen ke `_payloadReader.TryReadSavingGoalAchieved`.
            _payloadReader.TryReadSavingGoalAchieved(_payloadReader.ReadPayload(evt.Payload), out var completedGoalId, out _, out _) &&
            // Meneruskan `completedGoalId` (nilai selesai target identitas) sebagai argumen ke `string.Equals`; Meneruskan `goalId` (nilai target identitas)
            // sebagai argumen ke `string.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke
            // `string.Equals`.
            string.Equals(completedGoalId, goalId, StringComparison.OrdinalIgnoreCase));
        // Memeriksa pemeriksaan lebih besar atau sama antara `completedCount` dan `Math.Max(1, goal.CardQty ?? 1)`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ValidateGoalAchieved.
        if (completedCount >= Math.Max(1, goal.CardQty ?? 1))
        // Membuka scope cabang if untuk kondisi `completedCount >= Math.Max(1, goal.CardQty ?? 1)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ValidateGoalAchieved.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Kartu Tujuan Finansial ini sudah
            // dimiliki pemain lain”` kepada pemanggil dalam ValidateGoalAchieved; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Kartu Tujuan Finansial ini sudah dimiliki pemain lain”` sebagai argumen ke `Fail`.
                "Kartu Tujuan Finansial ini sudah dimiliki pemain lain");
        // Menutup scope cabang if untuk kondisi `completedCount >= Math.Max(1, goal.CardQty ?? 1)`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateGoalAchieved.
        }

        // Menyiapkan variabel lokal `balance` untuk saldo uang pemain pada keadaan yang sedang diproses dengan memanggil
        // `_derivedState.ComputeSavingBalance` dengan `history`, `request.UserId!.Value`, `goalId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var balance = _derivedState.ComputeSavingBalance(history, request.UserId!.Value, goalId);
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `cost > 0` dan `balance < cost`; sisi kanan diperiksa hanya jika sisi kiri benar;
        // blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateGoalAchieved.
        if (cost > 0 && balance < cost)
        // Membuka scope cabang if untuk kondisi `cost > 0 && balance < cost`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateGoalAchieved.
        {
            // Mengembalikan memanggil `Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Saldo tabungan tidak mencukupi
            // untuk goal”` kepada pemanggil dalam ValidateGoalAchieved; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return Fail(StatusCodes.Status422UnprocessableEntity, "DOMAIN_RULE_VIOLATION", "Saldo tabungan tidak mencukupi untuk goal");
        // Menutup scope cabang if untuk kondisi `cost > 0 && balance < cost`; bagian berikut berada di luar batas blok tersebut dalam ValidateGoalAchieved.
        }

        // Mengembalikan objek baru bertipe `EventSavingGoalValidation` dengan argumen (EventDomainValidationResult.Valid, null) kepada pemanggil dalam
        // ValidateGoalAchieved; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventSavingGoalValidation(EventDomainValidationResult.Valid, null);
    // Menutup scope metode ValidateGoalAchieved; bagian berikut berada di luar batas blok tersebut dalam ValidateGoalAchieved.
    }

    // Mendefinisikan metode `ValidateFeatureAndPlayer` dengan hasil bertipe `EventDomainValidationResult`; operasi ini menangani validate feature dan
    // pemain. Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan;
    // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
    private EventDomainValidationResult ValidateFeatureAndPlayer(EventRequest request, RulesetConfig config)
    // Membuka scope metode ValidateFeatureAndPlayer; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateFeatureAndPlayer.
    {
        // Memeriksa kebalikan kondisi `config.SavingGoalEnabled`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateFeatureAndPlayer.
        if (!config.SavingGoalEnabled)
        // Membuka scope cabang if untuk kondisi `!config.SavingGoalEnabled`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateFeatureAndPlayer.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Fitur
            // tabungan tujuan tidak aktif”` kepada pemanggil dalam ValidateFeatureAndPlayer; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke
                // `EventDomainValidationResult.Fail`.
                StatusCodes.Status422UnprocessableEntity,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Fitur tabungan tujuan tidak aktif”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Fitur tabungan tujuan tidak aktif");
        // Menutup scope cabang if untuk kondisi `!config.SavingGoalEnabled`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateFeatureAndPlayer.
        }

        // Memeriksa hasil pencocokan `request.UserId` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateFeatureAndPlayer.
        if (request.UserId is null)
        // Membuka scope cabang if untuk kondisi `request.UserId is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateFeatureAndPlayer.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Player wajib
            // diisi”`, `new ErrorDetail(”user_id”, ”REQUIRED”)` kepada pemanggil dalam ValidateFeatureAndPlayer; eksekusi jalur ini selesai setelah nilai hasil
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
        // Menutup scope cabang if untuk kondisi `request.UserId is null`; bagian berikut berada di luar batas blok tersebut dalam ValidateFeatureAndPlayer.
        }

        // Mengembalikan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) kepada pemanggil dalam
        // ValidateFeatureAndPlayer; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return EventDomainValidationResult.Valid;
    // Menutup scope metode ValidateFeatureAndPlayer; bagian berikut berada di luar batas blok tersebut dalam ValidateFeatureAndPlayer.
    }

    // Mendefinisikan metode `ValidateSavingDepositPayload` dengan hasil bertipe `EventDomainValidationResult`; operasi ini menangani validate tabungan
    // deposit payload. Masukan: Parameter `goalId` bertipe `string` membawa nilai target identitas; Parameter `amount` bertipe `int` membawa nominal
    // uang atau nilai transaksi yang dipakai dalam operasi.
    private EventDomainValidationResult ValidateSavingDepositPayload(string goalId, int amount)
    // Membuka scope metode ValidateSavingDepositPayload; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ValidateSavingDepositPayload.
    {
        // Memeriksa memeriksa apakah `goalId` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam ValidateSavingDepositPayload.
        if (string.IsNullOrWhiteSpace(goalId))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(goalId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateSavingDepositPayload.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Goal ID wajib
            // diisi”`, `new ErrorDetail(”payload.goal_id”, ”REQUIRED”)` kepada pemanggil dalam ValidateSavingDepositPayload; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Goal ID wajib diisi”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Goal ID wajib diisi",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.goal_id”, ”REQUIRED”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”payload.goal_id”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen
                // ke konstruktor `ErrorDetail`.
                new ErrorDetail("payload.goal_id", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(goalId)`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateSavingDepositPayload.
        }

        // Memeriksa pemeriksaan lebih kecil atau sama antara `amount` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateSavingDepositPayload.
        if (amount <= 0)
        // Membuka scope cabang if untuk kondisi `amount <= 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateSavingDepositPayload.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Amount harus > 0”`,
            // `new ErrorDetail(”payload.amount”, ”OUT_OF_RANGE”)` kepada pemanggil dalam ValidateSavingDepositPayload; eksekusi jalur ini selesai setelah nilai
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
        // Menutup scope cabang if untuk kondisi `amount <= 0`; bagian berikut berada di luar batas blok tersebut dalam ValidateSavingDepositPayload.
        }

        // Mengembalikan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) kepada pemanggil dalam
        // ValidateSavingDepositPayload; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return EventDomainValidationResult.Valid;
    // Menutup scope metode ValidateSavingDepositPayload; bagian berikut berada di luar batas blok tersebut dalam ValidateSavingDepositPayload.
    }

    // Mendefinisikan metode `Fail` dengan hasil bertipe `EventSavingGoalValidation`; operasi ini menangani fail. Masukan: Parameter `statusCode`
    // bertipe `int` membawa kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan; Parameter `errorCode` bertipe `string` membawa
    // nilai kesalahan kode; Parameter `message` bertipe `string` membawa nilai pesan; Parameter `details` bertipe `ErrorDetail[]` membawa nilai
    // rincian.
    private EventSavingGoalValidation Fail(
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
        // Mengembalikan objek baru bertipe `EventSavingGoalValidation` dengan argumen ( EventDomainValidationResult.Fail(statusCode, errorCode, message,
        // details), null) kepada pemanggil dalam Fail; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventSavingGoalValidation(
            // Meneruskan memanggil `EventDomainValidationResult.Fail` dengan `statusCode`, `errorCode`, `message`, `details` sebagai argumen ke konstruktor
            // `EventSavingGoalValidation`; Meneruskan `statusCode` (kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan) sebagai argumen
            // ke `EventDomainValidationResult.Fail`; Meneruskan `errorCode` (nilai kesalahan kode) sebagai argumen ke `EventDomainValidationResult.Fail`;
            // Meneruskan `message` (nilai pesan) sebagai argumen ke `EventDomainValidationResult.Fail`; Meneruskan `details` (nilai rincian) sebagai argumen ke
            // `EventDomainValidationResult.Fail`.
            EventDomainValidationResult.Fail(statusCode, errorCode, message, details),
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen ke konstruktor `EventSavingGoalValidation`.
            null);
    // Menutup scope metode Fail; bagian berikut berada di luar batas blok tersebut dalam Fail.
    }
// Menutup scope tipe EventSavingGoalValidator; bagian berikut berada di luar batas blok tersebut.
}
