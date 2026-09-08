// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventAssignmentValidator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan tipe class `EventAssignmentValidator` yang mewarisi atau menerapkan `IEventAssignmentValidator`; sealed mencegah tipe ini
// diturunkan lagi.
internal sealed class EventAssignmentValidator : IEventAssignmentValidator
// Membuka scope tipe EventAssignmentValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `EventPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly EventPayloadReader _payloadReader = new();
    // Mendeklarasikan field bertipe `int`: `RulebookMissionPenaltyPoints` menyimpan nilai rulebook misi penalti poin dengan nilai awal nilai literal
    // `10`.
    private const int RulebookMissionPenaltyPoints = 10;

    // Mendefinisikan metode `TryValidate` dengan hasil bertipe `bool`; operasi ini menangani try validate. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `history` bertipe
    // `IEnumerable<EventDb>` membawa nilai history; Parameter `participantCount` bertipe `int` membawa nilai participant jumlah; Parameter `result`
    // bertipe `EventDomainValidationResult` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out mengembalikan nilai melalui
    // parameter dan harus diisi oleh metode.
    public bool TryValidate(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history,
        // Parameter `participantCount` bertipe `int` membawa nilai participant jumlah.
        int participantCount,
        // Parameter `result` bertipe `EventDomainValidationResult` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out EventDomainValidationResult result)
    // Membuka scope metode TryValidate; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
    {
        // Memeriksa membandingkan kesamaan `string` dengan `request.ActionType`, `GameActionCatalog.SetupMisiAwal`, `StringComparison.OrdinalIgnoreCase`;
        // aturan perbandingan mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (string.Equals(request.ActionType, GameActionCatalog.SetupMisiAwal, StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(request.ActionType, GameActionCatalog.SetupMisiAwal, StringComparison.OrdinalIgnoreCase)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `ValidateMission` dengan `request`, `history` dalam TryValidate.
            result = ValidateMission(request, history);
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `string.Equals(request.ActionType, GameActionCatalog.SetupMisiAwal, StringComparison.OrdinalIgnoreCase)`;
        // bagian berikut berada di luar batas blok tersebut dalam TryValidate.
        }

        // Memeriksa membandingkan kesamaan `string` dengan `request.ActionType`, `”BagikanTieBreaker”`, `StringComparison.OrdinalIgnoreCase`; aturan
        // perbandingan mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (string.Equals(request.ActionType, "BagikanTieBreaker", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(request.ActionType, ”BagikanTieBreaker”, StringComparison.OrdinalIgnoreCase)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `ValidateTieBreaker` dengan `request`, `history`, `participantCount` dalam TryValidate.
            result = ValidateTieBreaker(request, history, participantCount);
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `string.Equals(request.ActionType, ”BagikanTieBreaker”, StringComparison.OrdinalIgnoreCase)`; bagian
        // berikut berada di luar batas blok tersebut dalam TryValidate.
        }

        // Memperbarui `result` menggunakan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) dalam TryValidate.
        result = EventDomainValidationResult.Valid;
        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return false;
    // Menutup scope metode TryValidate; bagian berikut berada di luar batas blok tersebut dalam TryValidate.
    }

    // Mendefinisikan metode `ValidateMission` dengan hasil bertipe `EventDomainValidationResult`; operasi ini menangani validate misi. Masukan:
    // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `history`
    // bertipe `IEnumerable<EventDb>` membawa nilai history.
    private EventDomainValidationResult ValidateMission(EventRequest request, IEnumerable<EventDb> history)
    // Membuka scope metode ValidateMission; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateMission.
    {
        // Menyiapkan variabel lokal `playerCheck` untuk nilai pemain check dengan memanggil `RequirePlayer` dengan `request`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerCheck = RequirePlayer(request);
        // Memeriksa kebalikan kondisi `playerCheck.IsValid`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateMission.
        if (!playerCheck.IsValid)
        // Membuka scope cabang if untuk kondisi `!playerCheck.IsValid`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateMission.
        {
            // Mengembalikan `playerCheck` (nilai pemain check) kepada pemanggil dalam ValidateMission; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return playerCheck;
        // Menutup scope cabang if untuk kondisi `!playerCheck.IsValid`; bagian berikut berada di luar batas blok tersebut dalam ValidateMission.
        }

        // Memeriksa kebalikan kondisi `_payloadReader.TryReadMissionAssigned(request.Payload, out var missionId, out var targetCardId, out var
        // penaltyPoints)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateMission.
        if (!_payloadReader.TryReadMissionAssigned(request.Payload, out var missionId, out var targetCardId, out var penaltyPoints))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadMissionAssigned(request.Payload, out var missionId, out var targetCardId, out var
        // penaltyPoints)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateMission.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Payload mission
            // tidak valid”`, `new ErrorDetail(”payload.mission_id”, ”REQUIRED”)` kepada pemanggil dalam ValidateMission; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Payload mission tidak valid”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Payload mission tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.mission_id”, ”REQUIRED”) sebagai argumen ke
                // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”payload.mission_id”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan
                // nilai literal `”REQUIRED”` sebagai argumen ke konstruktor `ErrorDetail`.
                new ErrorDetail("payload.mission_id", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadMissionAssigned(request.Payload, out var missionId, out var targetCardId, out var
        // penaltyPoints)`; bagian berikut berada di luar batas blok tersebut dalam ValidateMission.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `string.IsNullOrWhiteSpace(missionId)` dan
        // `string.IsNullOrWhiteSpace(targetCardId)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam ValidateMission.
        if (string.IsNullOrWhiteSpace(missionId) || string.IsNullOrWhiteSpace(targetCardId))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(missionId) || string.IsNullOrWhiteSpace(targetCardId)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam ValidateMission.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Mission ID dan
            // target wajib diisi”`, `new ErrorDetail(”payload.target_tertiary_card_id”, ”REQUIRED”)` kepada pemanggil dalam ValidateMission; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Mission ID dan target wajib diisi”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Mission ID dan target wajib diisi",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.target_tertiary_card_id”, ”REQUIRED”) sebagai argumen ke
                // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”payload.target_tertiary_card_id”` sebagai argumen ke konstruktor `ErrorDetail`;
                // Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke konstruktor `ErrorDetail`.
                new ErrorDetail("payload.target_tertiary_card_id", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(missionId) || string.IsNullOrWhiteSpace(targetCardId)`; bagian berikut berada di
        // luar batas blok tersebut dalam ValidateMission.
        }

        // Memeriksa pemeriksaan lebih kecil antara `penaltyPoints` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateMission.
        if (penaltyPoints < 0)
        // Membuka scope cabang if untuk kondisi `penaltyPoints < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateMission.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Penalty points tidak
            // valid”`, `new ErrorDetail(”payload.penalty_points”, ”OUT_OF_RANGE”)` kepada pemanggil dalam ValidateMission; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Penalty points tidak valid”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Penalty points tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.penalty_points”, ”OUT_OF_RANGE”) sebagai argumen ke
                // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”payload.penalty_points”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan
                // nilai literal `”OUT_OF_RANGE”` sebagai argumen ke konstruktor `ErrorDetail`.
                new ErrorDetail("payload.penalty_points", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `penaltyPoints < 0`; bagian berikut berada di luar batas blok tersebut dalam ValidateMission.
        }

        // Memeriksa perbandingan ketidaksamaan antara `penaltyPoints` dan `RulebookMissionPenaltyPoints`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ValidateMission.
        if (penaltyPoints != RulebookMissionPenaltyPoints)
        // Membuka scope cabang if untuk kondisi `penaltyPoints != RulebookMissionPenaltyPoints`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam ValidateMission.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`,
            // `”Penalty misi harus 10 poin”` kepada pemanggil dalam ValidateMission; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke
                // `EventDomainValidationResult.Fail`.
                StatusCodes.Status422UnprocessableEntity,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Penalty misi harus 10 poin”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Penalty misi harus 10 poin");
        // Menutup scope cabang if untuk kondisi `penaltyPoints != RulebookMissionPenaltyPoints`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateMission.
        }

        // Menyiapkan variabel lokal `alreadyAssigned` untuk nilai already assigned dengan memeriksa apakah `history` memiliki setidaknya satu elemen yang
        // memenuhi `e => e.UserId == request.UserId && string.Equals(e.ActionType, GameActionCatalog.SetupMisiAwal, StringComparison.OrdinalIgnoreCase)`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var alreadyAssigned = history.Any(e =>
            // Meneruskan fungsi lambda `e => e.UserId == request.UserId && string.Equals(e.ActionType, GameActionCatalog.SetupMisiAwal,
            // StringComparison.OrdinalIgnoreCase)` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `history.Any`.
            e.UserId == request.UserId &&
            // Meneruskan `e.ActionType` (nilai aksi jenis) sebagai argumen ke `string.Equals`; Meneruskan `GameActionCatalog.SetupMisiAwal` (nilai setup misi
            // awal) sebagai argumen ke `string.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke
            // `string.Equals`.
            string.Equals(e.ActionType, GameActionCatalog.SetupMisiAwal, StringComparison.OrdinalIgnoreCase));
        // Memeriksa `alreadyAssigned` (nilai already assigned); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateMission.
        if (alreadyAssigned)
        // Membuka scope cabang if untuk kondisi `alreadyAssigned`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateMission.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Misi
            // sudah ditetapkan untuk pemain”` kepada pemanggil dalam ValidateMission; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke
                // `EventDomainValidationResult.Fail`.
                StatusCodes.Status422UnprocessableEntity,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Misi sudah ditetapkan untuk pemain”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Misi sudah ditetapkan untuk pemain");
        // Menutup scope cabang if untuk kondisi `alreadyAssigned`; bagian berikut berada di luar batas blok tersebut dalam ValidateMission.
        }

        // Menyiapkan variabel lokal `missionAlreadyAssigned` untuk nilai misi already assigned dengan memeriksa apakah `history` memiliki setidaknya satu
        // elemen yang memenuhi `e => e.UserId != request.UserId && string.Equals(e.ActionType, GameActionCatalog.SetupMisiAwal,
        // StringComparison.OrdinalIgnoreCase) && _payloadReader.TryReadMissionAssigned( _...`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var missionAlreadyAssigned = history.Any(e =>
            // Meneruskan fungsi lambda `e => e.UserId != request.UserId && string.Equals(e.ActionType, GameActionCatalog.SetupMisiAwal,
            // StringComparison.OrdinalIgnoreCase) && _payloadReader.TryReadMissionAssigned( _...` yang dijalankan oleh operasi pemanggil untuk memproses setiap
            // masukan sebagai argumen ke `history.Any`.
            e.UserId != request.UserId &&
            // Meneruskan `e.ActionType` (nilai aksi jenis) sebagai argumen ke `string.Equals`; Meneruskan `GameActionCatalog.SetupMisiAwal` (nilai setup misi
            // awal) sebagai argumen ke `string.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen ke
            // `string.Equals`.
            string.Equals(e.ActionType, GameActionCatalog.SetupMisiAwal, StringComparison.OrdinalIgnoreCase) &&
            // Meneruskan fungsi lambda `e => e.UserId != request.UserId && string.Equals(e.ActionType, GameActionCatalog.SetupMisiAwal,
            // StringComparison.OrdinalIgnoreCase) && _payloadReader.TryReadMissionAssigned( _...` yang dijalankan oleh operasi pemanggil untuk memproses setiap
            // masukan sebagai argumen ke `history.Any`.
            _payloadReader.TryReadMissionAssigned(
                // Meneruskan memanggil `_payloadReader.ReadPayload` dengan `e.Payload` sebagai argumen ke `_payloadReader.TryReadMissionAssigned`; Meneruskan
                // `e.Payload` (muatan detail event dalam format JSON) sebagai argumen ke `_payloadReader.ReadPayload`.
                _payloadReader.ReadPayload(e.Payload),
                // Meneruskan `var assignedMissionId` sebagai argumen ke `_payloadReader.TryReadMissionAssigned`.
                out var assignedMissionId,
                // Meneruskan `_` (nilai ) sebagai argumen ke `_payloadReader.TryReadMissionAssigned`.
                out _,
                // Meneruskan `_` (nilai ) sebagai argumen ke `_payloadReader.TryReadMissionAssigned`.
                out _) &&
            // Meneruskan `assignedMissionId` (nilai assigned misi identitas) sebagai argumen ke `string.Equals`; Meneruskan `missionId` (identitas misi koleksi
            // yang ditugaskan) sebagai argumen ke `string.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen
            // ke `string.Equals`.
            string.Equals(assignedMissionId, missionId, StringComparison.OrdinalIgnoreCase));
        // Memeriksa `missionAlreadyAssigned` (nilai misi already assigned); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateMission.
        if (missionAlreadyAssigned)
        // Membuka scope cabang if untuk kondisi `missionAlreadyAssigned`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateMission.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Kartu
            // Misi Koleksi sudah ditetapkan untuk pemain lain”` kepada pemanggil dalam ValidateMission; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke
                // `EventDomainValidationResult.Fail`.
                StatusCodes.Status422UnprocessableEntity,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Kartu Misi Koleksi sudah ditetapkan untuk pemain lain”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Kartu Misi Koleksi sudah ditetapkan untuk pemain lain");
        // Menutup scope cabang if untuk kondisi `missionAlreadyAssigned`; bagian berikut berada di luar batas blok tersebut dalam ValidateMission.
        }

        // Mengembalikan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) kepada pemanggil dalam ValidateMission;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return EventDomainValidationResult.Valid;
    // Menutup scope metode ValidateMission; bagian berikut berada di luar batas blok tersebut dalam ValidateMission.
    }

    // Mendefinisikan metode `ValidateTieBreaker` dengan hasil bertipe `EventDomainValidationResult`; operasi ini menangani validate tie breaker.
    // Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter
    // `history` bertipe `IEnumerable<EventDb>` membawa nilai history; Parameter `participantCount` bertipe `int` membawa nilai participant jumlah.
    private EventDomainValidationResult ValidateTieBreaker(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history,
        // Parameter `participantCount` bertipe `int` membawa nilai participant jumlah.
        int participantCount)
    // Membuka scope metode ValidateTieBreaker; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateTieBreaker.
    {
        // Menyiapkan variabel lokal `playerCheck` untuk nilai pemain check dengan memanggil `RequirePlayer` dengan `request`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerCheck = RequirePlayer(request);
        // Memeriksa kebalikan kondisi `playerCheck.IsValid`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateTieBreaker.
        if (!playerCheck.IsValid)
        // Membuka scope cabang if untuk kondisi `!playerCheck.IsValid`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateTieBreaker.
        {
            // Mengembalikan `playerCheck` (nilai pemain check) kepada pemanggil dalam ValidateTieBreaker; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return playerCheck;
        // Menutup scope cabang if untuk kondisi `!playerCheck.IsValid`; bagian berikut berada di luar batas blok tersebut dalam ValidateTieBreaker.
        }

        // Memeriksa kebalikan kondisi `_payloadReader.TryReadTieBreaker(request.Payload, out var number)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ValidateTieBreaker.
        if (!_payloadReader.TryReadTieBreaker(request.Payload, out var number))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadTieBreaker(request.Payload, out var number)`; pernyataan/deklarasi berikut berada
        // di dalam batas blok ini dalam ValidateTieBreaker.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Payload tie breaker
            // tidak valid”`, `new ErrorDetail(”payload.number”, ”REQUIRED”)` kepada pemanggil dalam ValidateTieBreaker; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Payload tie breaker tidak valid”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Payload tie breaker tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.number”, ”REQUIRED”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”payload.number”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen
                // ke konstruktor `ErrorDetail`.
                new ErrorDetail("payload.number", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadTieBreaker(request.Payload, out var number)`; bagian berikut berada di luar batas
        // blok tersebut dalam ValidateTieBreaker.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `number < 1` dan `number > participantCount`; sisi kanan diperiksa hanya
        // jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateTieBreaker.
        if (number < 1 || number > participantCount)
        // Membuka scope cabang if untuk kondisi `number < 1 || number > participantCount`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ValidateTieBreaker.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `$”Nomor
            // tie breaker harus berada pada rentang 1 sampai {participantCount}”`, `new ErrorDetail(”payload.number”, ”OUT_OF_RANGE”)` kepada pemanggil dalam
            // ValidateTieBreaker; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke
                // `EventDomainValidationResult.Fail`.
                StatusCodes.Status422UnprocessableEntity,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan teks interpolasi `$”Nomor tie breaker harus berada pada rentang 1 sampai {participantCount}”`; nilai ekspresi di dalam kurung kurawal
                // disisipkan saat program berjalan sebagai argumen ke `EventDomainValidationResult.Fail`.
                $"Nomor tie breaker harus berada pada rentang 1 sampai {participantCount}",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”payload.number”, ”OUT_OF_RANGE”) sebagai argumen ke
                // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”payload.number”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai
                // literal `”OUT_OF_RANGE”` sebagai argumen ke konstruktor `ErrorDetail`.
                new ErrorDetail("payload.number", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `number < 1 || number > participantCount`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateTieBreaker.
        }

        // Menyiapkan variabel lokal `alreadyAssigned` untuk nilai already assigned dengan memeriksa apakah `history` memiliki setidaknya satu elemen yang
        // memenuhi `e => e.UserId == request.UserId && e.ActionType == ”BagikanTieBreaker”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var alreadyAssigned = history.Any(e =>
            // Meneruskan fungsi lambda `e => e.UserId == request.UserId && e.ActionType == ”BagikanTieBreaker”` yang dijalankan oleh operasi pemanggil untuk
            // memproses setiap masukan sebagai argumen ke `history.Any`.
            e.UserId == request.UserId &&
            // Meneruskan fungsi lambda `e => e.UserId == request.UserId && e.ActionType == ”BagikanTieBreaker”` yang dijalankan oleh operasi pemanggil untuk
            // memproses setiap masukan sebagai argumen ke `history.Any`.
            e.ActionType == "BagikanTieBreaker");
        // Memeriksa `alreadyAssigned` (nilai already assigned); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateTieBreaker.
        if (alreadyAssigned)
        // Membuka scope cabang if untuk kondisi `alreadyAssigned`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateTieBreaker.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Tie
            // breaker sudah ditetapkan untuk pemain”` kepada pemanggil dalam ValidateTieBreaker; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke
                // `EventDomainValidationResult.Fail`.
                StatusCodes.Status422UnprocessableEntity,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Tie breaker sudah ditetapkan untuk pemain”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Tie breaker sudah ditetapkan untuk pemain");
        // Menutup scope cabang if untuk kondisi `alreadyAssigned`; bagian berikut berada di luar batas blok tersebut dalam ValidateTieBreaker.
        }

        // Menyiapkan variabel lokal `numberAlreadyAssigned` untuk nilai number already assigned dengan memeriksa apakah `history` memiliki setidaknya satu
        // elemen yang memenuhi `e => e.UserId != request.UserId && string.Equals(e.ActionType, GameActionCatalog.TieBreakerAssigned,
        // StringComparison.OrdinalIgnoreCase) && _payloadReader.TryReadTieBreaker(_p...`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var numberAlreadyAssigned = history.Any(e =>
            // Meneruskan fungsi lambda `e => e.UserId != request.UserId && string.Equals(e.ActionType, GameActionCatalog.TieBreakerAssigned,
            // StringComparison.OrdinalIgnoreCase) && _payloadReader.TryReadTieBreaker(_p...` yang dijalankan oleh operasi pemanggil untuk memproses setiap
            // masukan sebagai argumen ke `history.Any`.
            e.UserId != request.UserId &&
            // Meneruskan `e.ActionType` (nilai aksi jenis) sebagai argumen ke `string.Equals`; Meneruskan `GameActionCatalog.TieBreakerAssigned` (nilai tie
            // breaker assigned) sebagai argumen ke `string.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal ignore case) sebagai argumen
            // ke `string.Equals`.
            string.Equals(e.ActionType, GameActionCatalog.TieBreakerAssigned, StringComparison.OrdinalIgnoreCase) &&
            // Meneruskan memanggil `_payloadReader.ReadPayload` dengan `e.Payload` sebagai argumen ke `_payloadReader.TryReadTieBreaker`; Meneruskan
            // `e.Payload` (muatan detail event dalam format JSON) sebagai argumen ke `_payloadReader.ReadPayload`; Meneruskan `var assignedNumber` sebagai
            // argumen ke `_payloadReader.TryReadTieBreaker`.
            _payloadReader.TryReadTieBreaker(_payloadReader.ReadPayload(e.Payload), out var assignedNumber) &&
            // Meneruskan fungsi lambda `e => e.UserId != request.UserId && string.Equals(e.ActionType, GameActionCatalog.TieBreakerAssigned,
            // StringComparison.OrdinalIgnoreCase) && _payloadReader.TryReadTieBreaker(_p...` yang dijalankan oleh operasi pemanggil untuk memproses setiap
            // masukan sebagai argumen ke `history.Any`.
            assignedNumber == number);
        // Memeriksa `numberAlreadyAssigned` (nilai number already assigned); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateTieBreaker.
        if (numberAlreadyAssigned)
        // Membuka scope cabang if untuk kondisi `numberAlreadyAssigned`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateTieBreaker.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Nomor
            // tie breaker sudah ditetapkan untuk pemain lain”` kepada pemanggil dalam ValidateTieBreaker; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke
                // `EventDomainValidationResult.Fail`.
                StatusCodes.Status422UnprocessableEntity,
                // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "DOMAIN_RULE_VIOLATION",
                // Meneruskan nilai literal `”Nomor tie breaker sudah ditetapkan untuk pemain lain”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Nomor tie breaker sudah ditetapkan untuk pemain lain");
        // Menutup scope cabang if untuk kondisi `numberAlreadyAssigned`; bagian berikut berada di luar batas blok tersebut dalam ValidateTieBreaker.
        }

        // Mengembalikan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) kepada pemanggil dalam ValidateTieBreaker;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return EventDomainValidationResult.Valid;
    // Menutup scope metode ValidateTieBreaker; bagian berikut berada di luar batas blok tersebut dalam ValidateTieBreaker.
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
// Menutup scope tipe EventAssignmentValidator; bagian berikut berada di luar batas blok tersebut.
}
