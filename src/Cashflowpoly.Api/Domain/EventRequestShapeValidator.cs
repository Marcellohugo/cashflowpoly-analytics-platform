// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventRequestShapeValidator.
// Mengimpor namespace `System.Collections.Frozen` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Collections.Frozen;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan tipe class `EventRequestShapeValidator` yang mewarisi atau menerapkan `IEventRequestShapeValidator`; sealed mencegah tipe ini
// diturunkan lagi.
internal sealed class EventRequestShapeValidator : IEventRequestShapeValidator
// Membuka scope tipe EventRequestShapeValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `FrozenSet<string>`: `AllowedActorTypes` menyimpan nilai allowed actor types dengan nilai awal membentuk himpunan
    // nilai unik dari `new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ”PLAYER”, ”SYSTEM” }` memakai `StringComparer.OrdinalIgnoreCase`.
    // readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field menjadi milik tipe dan dibagikan antar
    // instance.
    private static readonly FrozenSet<string> AllowedActorTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Menggunakan nilai literal `”PLAYER”` sebagai bagian ekspresi yang sedang disusun.
        "PLAYER",
        // Menggunakan nilai literal `”SYSTEM”` sebagai bagian ekspresi yang sedang disusun.
        "SYSTEM"
    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut.
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    // Mendeklarasikan field bertipe `FrozenSet<string>`: `AllowedWeekdays` menyimpan nilai allowed weekdays dengan nilai awal membentuk himpunan nilai
    // unik dari `new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ”MON”, ”TUE”, ”WED”, ”THU”, ”FRI”, ”SAT”, ”SUN” }` memakai
    // `StringComparer.OrdinalIgnoreCase`. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat field
    // menjadi milik tipe dan dibagikan antar instance.
    private static readonly FrozenSet<string> AllowedWeekdays = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini.
    {
        // Menggunakan nilai literal `”MON”` sebagai bagian ekspresi yang sedang disusun.
        "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN"
    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut.
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    // Mendefinisikan metode `Validate` dengan hasil bertipe `EventDomainValidationResult`; operasi ini menangani validate. Masukan: Parameter `request`
    // bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `scopedPlayerId` bertipe
    // `Guid?` membawa nilai scoped pemain identitas; nilai null diizinkan ketika data opsional belum tersedia.
    public EventDomainValidationResult Validate(EventRequest request, Guid? scopedPlayerId)
    // Membuka scope metode Validate; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
    {
        // Memeriksa kebalikan kondisi `AllowedActorTypes.Contains(request.ActorType)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // Validate.
        if (!AllowedActorTypes.Contains(request.ActorType))
        // Membuka scope cabang if untuk kondisi `!AllowedActorTypes.Contains(request.ActorType)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam Validate.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Actor type tidak
            // valid”`, `new ErrorDetail(”actor_type”, ”INVALID_ENUM”)` kepada pemanggil dalam Validate; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Actor type tidak valid”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Actor type tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”actor_type”, ”INVALID_ENUM”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”actor_type”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”INVALID_ENUM”` sebagai argumen
                // ke konstruktor `ErrorDetail`.
                new ErrorDetail("actor_type", "INVALID_ENUM"));
        // Menutup scope cabang if untuk kondisi `!AllowedActorTypes.Contains(request.ActorType)`; bagian berikut berada di luar batas blok tersebut dalam
        // Validate.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `scopedPlayerId.HasValue` dan `!string.Equals(request.ActorType, ”PLAYER”,
        // StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam Validate.
        if (scopedPlayerId.HasValue &&
            // Menggunakan kebalikan kondisi `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase)` sebagai bagian ekspresi yang
            // sedang disusun dalam Validate.
            !string.Equals(request.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `scopedPlayerId.HasValue && !string.Equals(request.ActorType, ”PLAYER”,
        // StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status403Forbidden`, `”FORBIDDEN”`, `”Player hanya dapat mengirim
            // event actor PLAYER”` kepada pemanggil dalam Validate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status403Forbidden` (nilai status 403 forbidden) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status403Forbidden,
                // Meneruskan nilai literal `”FORBIDDEN”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "FORBIDDEN",
                // Meneruskan nilai literal `”Player hanya dapat mengirim event actor PLAYER”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Player hanya dapat mengirim event actor PLAYER");
        // Menutup scope cabang if untuk kondisi `scopedPlayerId.HasValue && !string.Equals(request.ActorType, ”PLAYER”,
        // StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok tersebut dalam Validate.
        }

        // Memeriksa kebalikan kondisi `AllowedWeekdays.Contains(request.Weekday)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // Validate.
        if (!AllowedWeekdays.Contains(request.Weekday))
        // Membuka scope cabang if untuk kondisi `!AllowedWeekdays.Contains(request.Weekday)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam Validate.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Weekday tidak
            // valid”`, `new ErrorDetail(”weekday”, ”INVALID_ENUM”)` kepada pemanggil dalam Validate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Weekday tidak valid”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Weekday tidak valid",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”weekday”, ”INVALID_ENUM”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”weekday”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”INVALID_ENUM”` sebagai argumen ke
                // konstruktor `ErrorDetail`.
                new ErrorDetail("weekday", "INVALID_ENUM"));
        // Menutup scope cabang if untuk kondisi `!AllowedWeekdays.Contains(request.Weekday)`; bagian berikut berada di luar batas blok tersebut dalam
        // Validate.
        }

        // Memeriksa pemeriksaan lebih kecil antara `request.ActionSlot` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Validate.
        if (request.ActionSlot < 0)
        // Membuka scope cabang if untuk kondisi `request.ActionSlot < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Action slot minimal
            // 0”`, `new ErrorDetail(”action_slot”, ”OUT_OF_RANGE”)` kepada pemanggil dalam Validate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Action slot minimal 0”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Action slot minimal 0",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”action_slot”, ”OUT_OF_RANGE”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”action_slot”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”OUT_OF_RANGE”` sebagai argumen
                // ke konstruktor `ErrorDetail`.
                new ErrorDetail("action_slot", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `request.ActionSlot < 0`; bagian berikut berada di luar batas blok tersebut dalam Validate.
        }

        // Memeriksa pemeriksaan lebih kecil antara `request.TurnNumber` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Validate.
        if (request.TurnNumber < 0)
        // Membuka scope cabang if untuk kondisi `request.TurnNumber < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Turn number minimal
            // 0”`, `new ErrorDetail(”turn_number”, ”OUT_OF_RANGE”)` kepada pemanggil dalam Validate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Turn number minimal 0”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Turn number minimal 0",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”turn_number”, ”OUT_OF_RANGE”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”turn_number”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”OUT_OF_RANGE”` sebagai argumen
                // ke konstruktor `ErrorDetail`.
                new ErrorDetail("turn_number", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `request.TurnNumber < 0`; bagian berikut berada di luar batas blok tersebut dalam Validate.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `string.Equals(request.ActorType, ”SYSTEM”, StringComparison.OrdinalIgnoreCase)`
        // dan `request.TurnNumber != 0`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // Validate.
        if (string.Equals(request.ActorType, "SYSTEM", StringComparison.OrdinalIgnoreCase) &&
            // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `request.TurnNumber` dan `0` dalam Validate.
            request.TurnNumber != 0)
        // Membuka scope cabang if untuk kondisi `string.Equals(request.ActorType, ”SYSTEM”, StringComparison.OrdinalIgnoreCase) && request.TurnNumber !=
        // 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”System event harus
            // memakai turn number 0”`, `new ErrorDetail(”turn_number”, ”INVALID_FOR_ACTOR”)` kepada pemanggil dalam Validate; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”System event harus memakai turn number 0”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "System event harus memakai turn number 0",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”turn_number”, ”INVALID_FOR_ACTOR”) sebagai argumen ke
                // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”turn_number”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai
                // literal `”INVALID_FOR_ACTOR”` sebagai argumen ke konstruktor `ErrorDetail`.
                new ErrorDetail("turn_number", "INVALID_FOR_ACTOR"));
        // Menutup scope cabang if untuk kondisi `string.Equals(request.ActorType, ”SYSTEM”, StringComparison.OrdinalIgnoreCase) && request.TurnNumber !=
        // 0`; bagian berikut berada di luar batas blok tersebut dalam Validate.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `string.Equals(request.ActorType, ”SYSTEM”, StringComparison.OrdinalIgnoreCase)`
        // dan `request.ActionSlot != 0`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // Validate.
        if (string.Equals(request.ActorType, "SYSTEM", StringComparison.OrdinalIgnoreCase) &&
            // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `request.ActionSlot` dan `0` dalam Validate.
            request.ActionSlot != 0)
        // Membuka scope cabang if untuk kondisi `string.Equals(request.ActorType, ”SYSTEM”, StringComparison.OrdinalIgnoreCase) && request.ActionSlot !=
        // 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”System event harus
            // memakai action slot 0”`, `new ErrorDetail(”action_slot”, ”INVALID_FOR_ACTOR”)` kepada pemanggil dalam Validate; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”System event harus memakai action slot 0”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "System event harus memakai action slot 0",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”action_slot”, ”INVALID_FOR_ACTOR”) sebagai argumen ke
                // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”action_slot”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai
                // literal `”INVALID_FOR_ACTOR”` sebagai argumen ke konstruktor `ErrorDetail`.
                new ErrorDetail("action_slot", "INVALID_FOR_ACTOR"));
        // Menutup scope cabang if untuk kondisi `string.Equals(request.ActorType, ”SYSTEM”, StringComparison.OrdinalIgnoreCase) && request.ActionSlot !=
        // 0`; bagian berikut berada di luar batas blok tersebut dalam Validate.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`
        // dan `request.TurnNumber is < 1 or > 4`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam Validate.
        if (string.Equals(request.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
            // Menggunakan `request` (data masukan permintaan yang akan divalidasi atau diteruskan ke layanan) sebagai bagian ekspresi yang sedang disusun dalam
            // Validate.
            request.TurnNumber is < 1 or > 4)
        // Membuka scope cabang if untuk kondisi `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase) && request.TurnNumber is <
        // 1 or > 4`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Player event harus
            // memakai turn number 1 sampai 4”`, `new ErrorDetail(”turn_number”, ”OUT_OF_RANGE”)` kepada pemanggil dalam Validate; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Player event harus memakai turn number 1 sampai 4”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Player event harus memakai turn number 1 sampai 4",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”turn_number”, ”OUT_OF_RANGE”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”turn_number”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”OUT_OF_RANGE”` sebagai argumen
                // ke konstruktor `ErrorDetail`.
                new ErrorDetail("turn_number", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase) && request.TurnNumber is <
        // 1 or > 4`; bagian berikut berada di luar batas blok tersebut dalam Validate.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`
        // dan `GameActionCatalog.RequiresSystemActor(request.ActionType, request.Payload)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam Validate.
        if (string.Equals(request.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
            // Melanjutkan pengolahan dengan memanggil `GameActionCatalog.RequiresSystemActor` dengan `request.ActionType`, `request.Payload` dalam Validate.
            GameActionCatalog.RequiresSystemActor(request.ActionType, request.Payload))
        // Membuka scope cabang if untuk kondisi `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase) &&
        // GameActionCatalog.RequiresSystemActor(request.ActionType, request.Payload)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Validate.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Action type ini
            // harus dibuat oleh sistem”`, `new ErrorDetail(”actor_type”, ”SYSTEM_REQUIRED”)` kepada pemanggil dalam Validate; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Action type ini harus dibuat oleh sistem”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Action type ini harus dibuat oleh sistem",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”actor_type”, ”SYSTEM_REQUIRED”) sebagai argumen ke
                // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”actor_type”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai
                // literal `”SYSTEM_REQUIRED”` sebagai argumen ke konstruktor `ErrorDetail`.
                new ErrorDetail("actor_type", "SYSTEM_REQUIRED"));
        // Menutup scope cabang if untuk kondisi `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase) &&
        // GameActionCatalog.RequiresSystemActor(request.ActionType, request.Payload)`; bagian berikut berada di luar batas blok tersebut dalam Validate.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase)
        // && request.ActionSlot < 1` dan `GameActionCatalog.GetPlayerActionSlotPolicy(request.ActionType, request.Payload) != PlayerActionSlotPolicy.Free`;
        // sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Validate.
        if (string.Equals(request.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
            // Melanjutkan ekspresi dengan pemeriksaan lebih kecil antara `request.ActionSlot` dan `1` dalam Validate.
            request.ActionSlot < 1 &&
            // Melanjutkan pengolahan dengan memanggil `GameActionCatalog.GetPlayerActionSlotPolicy` dengan `request.ActionType`, `request.Payload` dalam
            // Validate.
            GameActionCatalog.GetPlayerActionSlotPolicy(request.ActionType, request.Payload) != PlayerActionSlotPolicy.Free)
        // Membuka scope cabang if untuk kondisi `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase) && request.ActionSlot < 1
        // && GameActionCatalog.GetPlayerActionSlotPolicy(request.ActionType, req...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Validate.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Player event harus
            // memakai action slot minimal 1”`, `new ErrorDetail(”action_slot”, ”OUT_OF_RANGE”)` kepada pemanggil dalam Validate; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Player event harus memakai action slot minimal 1”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Player event harus memakai action slot minimal 1",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”action_slot”, ”OUT_OF_RANGE”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”action_slot”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”OUT_OF_RANGE”` sebagai argumen
                // ke konstruktor `ErrorDetail`.
                new ErrorDetail("action_slot", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase) && request.ActionSlot < 1
        // && GameActionCatalog.GetPlayerActionSlotPolicy(request.ActionType, req...`; bagian berikut berada di luar batas blok tersebut dalam Validate.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase)
        // && request.ActionSlot != 0` dan `GameActionCatalog.GetPlayerActionSlotPolicy(request.ActionType, request.Payload) ==
        // PlayerActionSlotPolicy.Free`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // Validate.
        if (string.Equals(request.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
            // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `request.ActionSlot` dan `0` dalam Validate.
            request.ActionSlot != 0 &&
            // Melanjutkan pengolahan dengan memanggil `GameActionCatalog.GetPlayerActionSlotPolicy` dengan `request.ActionType`, `request.Payload` dalam
            // Validate.
            GameActionCatalog.GetPlayerActionSlotPolicy(request.ActionType, request.Payload) == PlayerActionSlotPolicy.Free)
        // Membuka scope cabang if untuk kondisi `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase) && request.ActionSlot != 0
        // && GameActionCatalog.GetPlayerActionSlotPolicy(request.ActionType, re...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Validate.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Aksi gratis pemain
            // harus memakai action slot 0”`, `new ErrorDetail(”action_slot”, ”INVALID_FOR_ACTION”)` kepada pemanggil dalam Validate; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Aksi gratis pemain harus memakai action slot 0”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Aksi gratis pemain harus memakai action slot 0",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”action_slot”, ”INVALID_FOR_ACTION”) sebagai argumen ke
                // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”action_slot”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai
                // literal `”INVALID_FOR_ACTION”` sebagai argumen ke konstruktor `ErrorDetail`.
                new ErrorDetail("action_slot", "INVALID_FOR_ACTION"));
        // Menutup scope cabang if untuk kondisi `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase) && request.ActionSlot != 0
        // && GameActionCatalog.GetPlayerActionSlotPolicy(request.ActionType, re...`; bagian berikut berada di luar batas blok tersebut dalam Validate.
        }

        // Memeriksa pemeriksaan lebih kecil antara `request.DayIndex` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Validate.
        if (request.DayIndex < 0)
        // Membuka scope cabang if untuk kondisi `request.DayIndex < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Day index minimal
            // 0”`, `new ErrorDetail(”day_index”, ”OUT_OF_RANGE”)` kepada pemanggil dalam Validate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Day index minimal 0”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Day index minimal 0",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”day_index”, ”OUT_OF_RANGE”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”day_index”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”OUT_OF_RANGE”` sebagai argumen ke
                // konstruktor `ErrorDetail`.
                new ErrorDetail("day_index", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `request.DayIndex < 0`; bagian berikut berada di luar batas blok tersebut dalam Validate.
        }

        // Memeriksa pemeriksaan lebih kecil antara `request.SequenceNumber` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // Validate.
        if (request.SequenceNumber < 0)
        // Membuka scope cabang if untuk kondisi `request.SequenceNumber < 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Sequence number
            // minimal 0”`, `new ErrorDetail(”sequence_number”, ”OUT_OF_RANGE”)` kepada pemanggil dalam Validate; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Sequence number minimal 0”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Sequence number minimal 0",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”sequence_number”, ”OUT_OF_RANGE”) sebagai argumen ke
                // `EventDomainValidationResult.Fail`; Meneruskan nilai literal `”sequence_number”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai
                // literal `”OUT_OF_RANGE”` sebagai argumen ke konstruktor `ErrorDetail`.
                new ErrorDetail("sequence_number", "OUT_OF_RANGE"));
        // Menutup scope cabang if untuk kondisi `request.SequenceNumber < 0`; bagian berikut berada di luar batas blok tersebut dalam Validate.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`
        // dan `request.UserId is null`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // Validate.
        if (string.Equals(request.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
            // Menggunakan `request` (data masukan permintaan yang akan divalidasi atau diteruskan ke layanan) sebagai bagian ekspresi yang sedang disusun dalam
            // Validate.
            request.UserId is null)
        // Membuka scope cabang if untuk kondisi `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase) && request.UserId is null`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Validate.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Player wajib diisi
            // untuk actor PLAYER”`, `new ErrorDetail(”user_id”, ”REQUIRED”)` kepada pemanggil dalam Validate; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Player wajib diisi untuk actor PLAYER”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Player wajib diisi untuk actor PLAYER",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”user_id”, ”REQUIRED”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”user_id”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke
                // konstruktor `ErrorDetail`.
                new ErrorDetail("user_id", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase) && request.UserId is null`;
        // bagian berikut berada di luar batas blok tersebut dalam Validate.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `scopedPlayerId.HasValue` dan `request.UserId != scopedPlayerId.Value`; sisi
        // kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam Validate.
        if (scopedPlayerId.HasValue && request.UserId != scopedPlayerId.Value)
        // Membuka scope cabang if untuk kondisi `scopedPlayerId.HasValue && request.UserId != scopedPlayerId.Value`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam Validate.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status403Forbidden`, `”FORBIDDEN”`, `”Player hanya dapat mengirim
            // event miliknya”` kepada pemanggil dalam Validate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status403Forbidden` (nilai status 403 forbidden) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status403Forbidden,
                // Meneruskan nilai literal `”FORBIDDEN”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "FORBIDDEN",
                // Meneruskan nilai literal `”Player hanya dapat mengirim event miliknya”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Player hanya dapat mengirim event miliknya");
        // Menutup scope cabang if untuk kondisi `scopedPlayerId.HasValue && request.UserId != scopedPlayerId.Value`; bagian berikut berada di luar batas
        // blok tersebut dalam Validate.
        }

        // Memeriksa memeriksa apakah `request.ActionType` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam Validate.
        if (string.IsNullOrWhiteSpace(request.ActionType))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(request.ActionType)`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam Validate.
        {
            // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status400BadRequest`, `”VALIDATION_ERROR”`, `”Action type wajib
            // diisi”`, `new ErrorDetail(”action_type”, ”REQUIRED”)` kepada pemanggil dalam Validate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Fail(
                // Meneruskan `StatusCodes.Status400BadRequest` (nilai status 400 bad permintaan) sebagai argumen ke `EventDomainValidationResult.Fail`.
                StatusCodes.Status400BadRequest,
                // Meneruskan nilai literal `”VALIDATION_ERROR”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "VALIDATION_ERROR",
                // Meneruskan nilai literal `”Action type wajib diisi”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                "Action type wajib diisi",
                // Meneruskan objek baru bertipe `ErrorDetail` dengan argumen (”action_type”, ”REQUIRED”) sebagai argumen ke `EventDomainValidationResult.Fail`;
                // Meneruskan nilai literal `”action_type”` sebagai argumen ke konstruktor `ErrorDetail`; Meneruskan nilai literal `”REQUIRED”` sebagai argumen ke
                // konstruktor `ErrorDetail`.
                new ErrorDetail("action_type", "REQUIRED"));
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(request.ActionType)`; bagian berikut berada di luar batas blok tersebut dalam
        // Validate.
        }

        // Mengembalikan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) kepada pemanggil dalam Validate; eksekusi jalur
        // ini selesai setelah nilai hasil ditentukan.
        return EventDomainValidationResult.Valid;
    // Menutup scope metode Validate; bagian berikut berada di luar batas blok tersebut dalam Validate.
    }
// Menutup scope tipe EventRequestShapeValidator; bagian berikut berada di luar batas blok tersebut.
}
