// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui EventAssignmentValidatorTests.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `EventAssignmentValidatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventAssignmentValidatorTests
// Membuka scope tipe EventAssignmentValidatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_MissionRejectsDuplicateAssignmentForPlayer` dengan hasil bertipe `void`; operasi ini menangani try validate
    // misi rejects duplicate assignment untuk pemain.
    public void TryValidate_MissionRejectsDuplicateAssignmentForPlayer()
    // Membuka scope metode TryValidate_MissionRejectsDuplicateAssignmentForPlayer; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidate_MissionRejectsDuplicateAssignmentForPlayer.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `playerId`, `”SetupMisiAwal”`, `”””{”mission_id”:”m-1”,”target_tertiary_card_id”:”bike”,”penalty_points”:10}”””`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest(
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke `CreateRequest`.
            playerId,
            // Meneruskan nilai literal `”SetupMisiAwal”` sebagai argumen ke `CreateRequest`.
            "SetupMisiAwal",
            // Meneruskan nilai literal `”””{”mission_id”:”m-1”,”target_tertiary_card_id”:”bike”,”penalty_points”:10}”””` sebagai argumen ke `CreateRequest`.
            """{"mission_id":"m-1","target_tertiary_card_id":"bike","penalty_points":10}""");
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidate_MissionRejectsDuplicateAssignmentForPlayer.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `playerId`, `”SetupMisiAwal”`,
            // `”””{”mission_id”:”m-old”,”target_tertiary_card_id”:”phone”,”penalty_points”:10}”””` dalam
            // TryValidate_MissionRejectsDuplicateAssignmentForPlayer.
            CreateEvent(playerId, "SetupMisiAwal", """{"mission_id":"m-old","target_tertiary_card_id":"phone","penalty_points":10}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidate_MissionRejectsDuplicateAssignmentForPlayer.
        };

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventAssignmentValidator().TryValidate` dengan `request`,
        // `history`, `4`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventAssignmentValidator().TryValidate(request, history, 4, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidate_MissionRejectsDuplicateAssignmentForPlayer.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidate_MissionRejectsDuplicateAssignmentForPlayer.
        Assert.False(result.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`StatusCodes.Status422UnprocessableEntity`,
        // `result.StatusCode`); pengujian gagal jika keduanya berbeda dalam TryValidate_MissionRejectsDuplicateAssignmentForPlayer.
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Misi sudah ditetapkan untuk pemain”`,
        // `result.Message`); pengujian gagal jika keduanya berbeda dalam TryValidate_MissionRejectsDuplicateAssignmentForPlayer.
        Assert.Equal("Misi sudah ditetapkan untuk pemain", result.Message);
    // Menutup scope metode TryValidate_MissionRejectsDuplicateAssignmentForPlayer; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_MissionRejectsDuplicateAssignmentForPlayer.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_TieBreakerAcceptsPositiveNumber` dengan hasil bertipe `void`; operasi ini menangani try validate tie breaker
    // accepts positive number.
    public void TryValidate_TieBreakerAcceptsPositiveNumber()
    // Membuka scope metode TryValidate_TieBreakerAcceptsPositiveNumber; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidate_TieBreakerAcceptsPositiveNumber.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `Guid.NewGuid()`, `”BagikanTieBreaker”`, `”””{”number”:3}”””`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest(Guid.NewGuid(), "BagikanTieBreaker", """{"number":3}""");

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventAssignmentValidator().TryValidate` dengan `request`,
        // `Array.Empty<EventDb>()`, `4`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventAssignmentValidator().TryValidate(request, Array.Empty<EventDb>(), 4, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidate_TieBreakerAcceptsPositiveNumber.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidate_TieBreakerAcceptsPositiveNumber.
        Assert.True(result.IsValid);
    // Menutup scope metode TryValidate_TieBreakerAcceptsPositiveNumber; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_TieBreakerAcceptsPositiveNumber.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_MissionRejectsCardAssignedToAnotherPlayer` dengan hasil bertipe `void`; operasi ini menangani try validate
    // misi rejects kartu assigned ke another pemain.
    public void TryValidate_MissionRejectsCardAssignedToAnotherPlayer()
    // Membuka scope metode TryValidate_MissionRejectsCardAssignedToAnotherPlayer; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidate_MissionRejectsCardAssignedToAnotherPlayer.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `Guid.NewGuid()`, `”SetupMisiAwal”`, `”””{”mission_id”:”m-1”,”target_tertiary_card_id”:”bike”,”penalty_points”:10}”””`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest(
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke `CreateRequest`.
            Guid.NewGuid(),
            // Meneruskan nilai literal `”SetupMisiAwal”` sebagai argumen ke `CreateRequest`.
            "SetupMisiAwal",
            // Meneruskan nilai literal `”””{”mission_id”:”m-1”,”target_tertiary_card_id”:”bike”,”penalty_points”:10}”””` sebagai argumen ke `CreateRequest`.
            """{"mission_id":"m-1","target_tertiary_card_id":"bike","penalty_points":10}""");
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidate_MissionRejectsCardAssignedToAnotherPlayer.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `”SetupMisiAwal”`,
            // `”””{”mission_id”:”m-1”,”target_tertiary_card_id”:”bike”,”penalty_points”:10}”””` dalam TryValidate_MissionRejectsCardAssignedToAnotherPlayer.
            CreateEvent(
                // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke `CreateEvent`.
                Guid.NewGuid(),
                // Meneruskan nilai literal `”SetupMisiAwal”` sebagai argumen ke `CreateEvent`.
                "SetupMisiAwal",
                // Meneruskan nilai literal `”””{”mission_id”:”m-1”,”target_tertiary_card_id”:”bike”,”penalty_points”:10}”””` sebagai argumen ke `CreateEvent`.
                """{"mission_id":"m-1","target_tertiary_card_id":"bike","penalty_points":10}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidate_MissionRejectsCardAssignedToAnotherPlayer.
        };

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventAssignmentValidator().TryValidate` dengan `request`,
        // `history`, `4`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventAssignmentValidator().TryValidate(request, history, 4, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidate_MissionRejectsCardAssignedToAnotherPlayer.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidate_MissionRejectsCardAssignedToAnotherPlayer.
        Assert.False(result.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Kartu Misi Koleksi sudah ditetapkan untuk
        // pemain lain”`, `result.Message`); pengujian gagal jika keduanya berbeda dalam TryValidate_MissionRejectsCardAssignedToAnotherPlayer.
        Assert.Equal("Kartu Misi Koleksi sudah ditetapkan untuk pemain lain", result.Message);
    // Menutup scope metode TryValidate_MissionRejectsCardAssignedToAnotherPlayer; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_MissionRejectsCardAssignedToAnotherPlayer.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_TieBreakerRejectsNumberAssignedToAnotherPlayer` dengan hasil bertipe `void`; operasi ini menangani try
    // validate tie breaker rejects number assigned ke another pemain.
    public void TryValidate_TieBreakerRejectsNumberAssignedToAnotherPlayer()
    // Membuka scope metode TryValidate_TieBreakerRejectsNumberAssignedToAnotherPlayer; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam TryValidate_TieBreakerRejectsNumberAssignedToAnotherPlayer.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `Guid.NewGuid()`, `”BagikanTieBreaker”`, `”””{”number”:3}”””`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest(Guid.NewGuid(), "BagikanTieBreaker", """{"number":3}""");
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidate_TieBreakerRejectsNumberAssignedToAnotherPlayer.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `Guid.NewGuid()`, `”BagikanTieBreaker”`, `”””{”number”:3}”””` dalam
            // TryValidate_TieBreakerRejectsNumberAssignedToAnotherPlayer.
            CreateEvent(Guid.NewGuid(), "BagikanTieBreaker", """{"number":3}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidate_TieBreakerRejectsNumberAssignedToAnotherPlayer.
        };

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventAssignmentValidator().TryValidate` dengan `request`,
        // `history`, `4`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventAssignmentValidator().TryValidate(request, history, 4, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidate_TieBreakerRejectsNumberAssignedToAnotherPlayer.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidate_TieBreakerRejectsNumberAssignedToAnotherPlayer.
        Assert.False(result.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Nomor tie breaker sudah ditetapkan untuk
        // pemain lain”`, `result.Message`); pengujian gagal jika keduanya berbeda dalam TryValidate_TieBreakerRejectsNumberAssignedToAnotherPlayer.
        Assert.Equal("Nomor tie breaker sudah ditetapkan untuk pemain lain", result.Message);
    // Menutup scope metode TryValidate_TieBreakerRejectsNumberAssignedToAnotherPlayer; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_TieBreakerRejectsNumberAssignedToAnotherPlayer.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_TieBreakerRejectsNumberOutsideParticipantRange` dengan hasil bertipe `void`; operasi ini menangani try
    // validate tie breaker rejects number outside participant range.
    public void TryValidate_TieBreakerRejectsNumberOutsideParticipantRange()
    // Membuka scope metode TryValidate_TieBreakerRejectsNumberOutsideParticipantRange; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam TryValidate_TieBreakerRejectsNumberOutsideParticipantRange.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `Guid.NewGuid()`, `”BagikanTieBreaker”`, `”””{”number”:5}”””`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest(Guid.NewGuid(), "BagikanTieBreaker", """{"number":5}""");

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventAssignmentValidator().TryValidate` dengan `request`,
        // `Array.Empty<EventDb>()`, `4`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventAssignmentValidator().TryValidate(request, Array.Empty<EventDb>(), 4, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidate_TieBreakerRejectsNumberOutsideParticipantRange.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidate_TieBreakerRejectsNumberOutsideParticipantRange.
        Assert.False(result.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`StatusCodes.Status422UnprocessableEntity`,
        // `result.StatusCode`); pengujian gagal jika keduanya berbeda dalam TryValidate_TieBreakerRejectsNumberOutsideParticipantRange.
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”payload.number”`,
        // `Assert.Single(result.Details).Field`); pengujian gagal jika keduanya berbeda dalam TryValidate_TieBreakerRejectsNumberOutsideParticipantRange.
        Assert.Equal("payload.number", Assert.Single(result.Details).Field);
    // Menutup scope metode TryValidate_TieBreakerRejectsNumberOutsideParticipantRange; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_TieBreakerRejectsNumberOutsideParticipantRange.
    }

    // Mendefinisikan metode `CreateRequest` dengan hasil bertipe `EventRequest`; operasi ini menangani create permintaan. Masukan: Parameter `playerId`
    // bertipe `Guid` membawa nilai pemain identitas; Parameter `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payloadJson` bertipe
    // `string` membawa nilai payload JSON.
    private static EventRequest CreateRequest(Guid playerId, string actionType, string payloadJson)
    // Membuka scope metode CreateRequest; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateRequest.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(payloadJson);
        // Mengembalikan objek baru bertipe `EventRequest` dengan argumen ( Guid.NewGuid(), Guid.NewGuid(), playerId, ”PLAYER”, new DateTimeOffset(2026, 1,
        // 2, 3, 4, 5, TimeSpan.Zero), 0, ”MON”, 1, 0, actionType, Guid.NewGuid(), docume... kepada pemanggil dalam CreateRequest; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return new EventRequest(
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke konstruktor `EventRequest`.
            playerId,
            // Meneruskan nilai literal `”PLAYER”` sebagai argumen ke konstruktor `EventRequest`.
            "PLAYER",
            // Meneruskan objek baru bertipe `DateTimeOffset` dengan argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero) sebagai argumen ke konstruktor `EventRequest`;
            // Meneruskan nilai literal `2026` sebagai argumen ke konstruktor `DateTimeOffset`; Meneruskan nilai literal `1` sebagai argumen ke konstruktor
            // `DateTimeOffset`; Meneruskan nilai literal `2` sebagai argumen ke konstruktor `DateTimeOffset`; Meneruskan nilai literal `3` sebagai argumen ke
            // konstruktor `DateTimeOffset`; Meneruskan nilai literal `4` sebagai argumen ke konstruktor `DateTimeOffset`; Meneruskan nilai literal `5` sebagai
            // argumen ke konstruktor `DateTimeOffset`; Meneruskan `TimeSpan.Zero` (nilai zero) sebagai argumen ke konstruktor `DateTimeOffset`.
            new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            // Meneruskan nilai literal `0` sebagai argumen ke konstruktor `EventRequest`.
            0,
            // Meneruskan nilai literal `”MON”` sebagai argumen ke konstruktor `EventRequest`.
            "MON",
            // Meneruskan nilai literal `1` sebagai argumen ke konstruktor `EventRequest`.
            1,
            // Meneruskan nilai literal `0` sebagai argumen ke konstruktor `EventRequest`.
            0,
            // Meneruskan `actionType` (nilai aksi jenis) sebagai argumen ke konstruktor `EventRequest`.
            actionType,
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan membuat salinan `document.RootElement` agar hasil dapat digunakan terpisah dari objek sumber sebagai argumen ke konstruktor
            // `EventRequest`.
            document.RootElement.Clone(),
            // Meneruskan nilai literal `”client-123”` sebagai argumen ke konstruktor `EventRequest`.
            "client-123");
    // Menutup scope metode CreateRequest; bagian berikut berada di luar batas blok tersebut dalam CreateRequest.
    }

    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `EventDb`; operasi ini menangani create event. Masukan: Parameter `playerId` bertipe
    // `Guid` membawa nilai pemain identitas; Parameter `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payload` bertipe `string`
    // membawa muatan detail event dalam format JSON.
    private static EventDb CreateEvent(Guid playerId, string actionType, string payload)
    // Membuka scope metode CreateEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
    {
        // Mengembalikan objek baru bertipe `EventDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam CreateEvent; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new EventDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
        {
            // Memperbarui `EventId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
            EventId = Guid.NewGuid(),
            // Memperbarui `SessionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
            SessionId = Guid.NewGuid(),
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam CreateEvent.
            UserId = playerId,
            // Memperbarui `ActorType` menggunakan nilai literal `”PLAYER”` dalam CreateEvent.
            ActorType = "PLAYER",
            // Memperbarui `Timestamp` menggunakan objek baru bertipe `DateTimeOffset` dengan argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero) dalam CreateEvent.
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            // Memperbarui `DayIndex` menggunakan nilai literal `0` dalam CreateEvent.
            DayIndex = 0,
            // Memperbarui `Weekday` menggunakan nilai literal `”MON”` dalam CreateEvent.
            Weekday = "MON",
            // Memperbarui `ActionSlot` menggunakan nilai literal `1` dalam CreateEvent.
            ActionSlot = 1,
            // Memperbarui `SequenceNumber` menggunakan nilai literal `1` dalam CreateEvent.
            SequenceNumber = 1,
            // Memperbarui `ActionType` menggunakan `actionType` (nilai aksi jenis) dalam CreateEvent.
            ActionType = actionType,
            // Memperbarui `RulesetVersionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
            RulesetVersionId = Guid.NewGuid(),
            // Memperbarui `Payload` menggunakan `payload` (muatan detail event dalam format JSON) dalam CreateEvent.
            Payload = payload
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
        };
    // Menutup scope metode CreateEvent; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
    }
// Menutup scope tipe EventAssignmentValidatorTests; bagian berikut berada di luar batas blok tersebut.
}
