// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui EventRequestShapeValidatorTests.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
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

// Mendefinisikan tipe class `EventRequestShapeValidatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventRequestShapeValidatorTests
// Membuka scope tipe EventRequestShapeValidatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Validate_RejectsInvalidActorType` dengan hasil bertipe `void`; operasi ini menangani validate rejects invalid actor jenis.
    public void Validate_RejectsInvalidActorType()
    // Membuka scope metode Validate_RejectsInvalidActorType; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Validate_RejectsInvalidActorType.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan `CreateRequest() with {
        // ActorType = ”BANK” }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest() with { ActorType = "BANK" };

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil `new
        // EventRequestShapeValidator().Validate` dengan `request`, `null`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // Validate_RejectsInvalidActorType.
        Assert.False(result.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`StatusCodes.Status400BadRequest`,
        // `result.StatusCode`); pengujian gagal jika keduanya berbeda dalam Validate_RejectsInvalidActorType.
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”VALIDATION_ERROR”`, `result.ErrorCode`);
        // pengujian gagal jika keduanya berbeda dalam Validate_RejectsInvalidActorType.
        Assert.Equal("VALIDATION_ERROR", result.ErrorCode);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Actor type tidak valid”`, `result.Message`);
        // pengujian gagal jika keduanya berbeda dalam Validate_RejectsInvalidActorType.
        Assert.Equal("Actor type tidak valid", result.Message);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `result.Details`, `detail => detail.Field
        // == ”actor_type” && detail.Issue == ”INVALID_ENUM”` dalam Validate_RejectsInvalidActorType.
        Assert.Contains(result.Details, detail => detail.Field == "actor_type" && detail.Issue == "INVALID_ENUM");
    // Menutup scope metode Validate_RejectsInvalidActorType; bagian berikut berada di luar batas blok tersebut dalam Validate_RejectsInvalidActorType.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Validate_RejectsSystemActorFromScopedPlayer` dengan hasil bertipe `void`; operasi ini menangani validate rejects system
    // actor dari scoped pemain.
    public void Validate_RejectsSystemActorFromScopedPlayer()
    // Membuka scope metode Validate_RejectsSystemActorFromScopedPlayer; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Validate_RejectsSystemActorFromScopedPlayer.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan `CreateRequest() with {
        // ActorType = ”SYSTEM”, UserId = playerId }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest() with { ActorType = "SYSTEM", UserId = playerId };

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil `new
        // EventRequestShapeValidator().Validate` dengan `request`, `playerId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: playerId);

        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // Validate_RejectsSystemActorFromScopedPlayer.
        Assert.False(result.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`StatusCodes.Status403Forbidden`,
        // `result.StatusCode`); pengujian gagal jika keduanya berbeda dalam Validate_RejectsSystemActorFromScopedPlayer.
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”FORBIDDEN”`, `result.ErrorCode`); pengujian
        // gagal jika keduanya berbeda dalam Validate_RejectsSystemActorFromScopedPlayer.
        Assert.Equal("FORBIDDEN", result.ErrorCode);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Player hanya dapat mengirim event actor
        // PLAYER”`, `result.Message`); pengujian gagal jika keduanya berbeda dalam Validate_RejectsSystemActorFromScopedPlayer.
        Assert.Equal("Player hanya dapat mengirim event actor PLAYER", result.Message);
        // Menjalankan pemeriksaan hasil dengan `Assert.Empty` menggunakan `result.Details`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // Validate_RejectsSystemActorFromScopedPlayer.
        Assert.Empty(result.Details);
    // Menutup scope metode Validate_RejectsSystemActorFromScopedPlayer; bagian berikut berada di luar batas blok tersebut dalam
    // Validate_RejectsSystemActorFromScopedPlayer.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Validate_RejectsMismatchedScopedPlayer` dengan hasil bertipe `void`; operasi ini menangani validate rejects mismatched
    // scoped pemain.
    public void Validate_RejectsMismatchedScopedPlayer()
    // Membuka scope metode Validate_RejectsMismatchedScopedPlayer; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Validate_RejectsMismatchedScopedPlayer.
    {
        // Menyiapkan variabel lokal `scopedPlayerId` untuk nilai scoped pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var scopedPlayerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan `CreateRequest() with {
        // UserId = Guid.NewGuid() }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest() with { UserId = Guid.NewGuid() };

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil `new
        // EventRequestShapeValidator().Validate` dengan `request`, `scopedPlayerId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId);

        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // Validate_RejectsMismatchedScopedPlayer.
        Assert.False(result.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`StatusCodes.Status403Forbidden`,
        // `result.StatusCode`); pengujian gagal jika keduanya berbeda dalam Validate_RejectsMismatchedScopedPlayer.
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”FORBIDDEN”`, `result.ErrorCode`); pengujian
        // gagal jika keduanya berbeda dalam Validate_RejectsMismatchedScopedPlayer.
        Assert.Equal("FORBIDDEN", result.ErrorCode);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Player hanya dapat mengirim event miliknya”`,
        // `result.Message`); pengujian gagal jika keduanya berbeda dalam Validate_RejectsMismatchedScopedPlayer.
        Assert.Equal("Player hanya dapat mengirim event miliknya", result.Message);
    // Menutup scope metode Validate_RejectsMismatchedScopedPlayer; bagian berikut berada di luar batas blok tersebut dalam
    // Validate_RejectsMismatchedScopedPlayer.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Validate_AcceptsValidPlayerEvent` dengan hasil bertipe `void`; operasi ini menangani validate accepts valid pemain event.
    public void Validate_AcceptsValidPlayerEvent()
    // Membuka scope metode Validate_AcceptsValidPlayerEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Validate_AcceptsValidPlayerEvent.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan `CreateRequest() with {
        // UserId = playerId }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest() with { UserId = playerId };

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil `new
        // EventRequestShapeValidator().Validate` dengan `request`, `playerId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: playerId);

        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // Validate_AcceptsValidPlayerEvent.
        Assert.True(result.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`StatusCodes.Status200OK`,
        // `result.StatusCode`); pengujian gagal jika keduanya berbeda dalam Validate_AcceptsValidPlayerEvent.
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        // Menjalankan pemeriksaan Null atas `result.ErrorCode` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // Validate_AcceptsValidPlayerEvent.
        Assert.Null(result.ErrorCode);
        // Menjalankan pemeriksaan hasil dengan `Assert.Empty` menggunakan `result.Details`; ketidaksesuaian dengan ekspektasi membuat pengujian gagal dalam
        // Validate_AcceptsValidPlayerEvent.
        Assert.Empty(result.Details);
    // Menutup scope metode Validate_AcceptsValidPlayerEvent; bagian berikut berada di luar batas blok tersebut dalam Validate_AcceptsValidPlayerEvent.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Validate_AcceptsValidSystemEventWithZeroTurnAndActionSlot` dengan hasil bertipe `void`; operasi ini menangani validate
    // accepts valid system event dengan zero giliran dan aksi slot.
    public void Validate_AcceptsValidSystemEventWithZeroTurnAndActionSlot()
    // Membuka scope metode Validate_AcceptsValidSystemEventWithZeroTurnAndActionSlot; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Validate_AcceptsValidSystemEventWithZeroTurnAndActionSlot.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan `CreateRequest() with {
        // ActorType = ”SYSTEM”, UserId = null, ActionSlot = 0, TurnNumber = 0 }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest() with
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Validate_AcceptsValidSystemEventWithZeroTurnAndActionSlot.
        {
            // Memperbarui `ActorType` menggunakan nilai literal `”SYSTEM”` dalam Validate_AcceptsValidSystemEventWithZeroTurnAndActionSlot.
            ActorType = "SYSTEM",
            // Memperbarui `UserId` menggunakan null, yaitu penanda tidak ada nilai dalam Validate_AcceptsValidSystemEventWithZeroTurnAndActionSlot.
            UserId = null,
            // Memperbarui `ActionSlot` menggunakan nilai literal `0` dalam Validate_AcceptsValidSystemEventWithZeroTurnAndActionSlot.
            ActionSlot = 0,
            // Memperbarui `TurnNumber` menggunakan nilai literal `0` dalam Validate_AcceptsValidSystemEventWithZeroTurnAndActionSlot.
            TurnNumber = 0
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Validate_AcceptsValidSystemEventWithZeroTurnAndActionSlot.
        };

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil `new
        // EventRequestShapeValidator().Validate` dengan `request`, `null`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // Validate_AcceptsValidSystemEventWithZeroTurnAndActionSlot.
        Assert.True(result.IsValid);
    // Menutup scope metode Validate_AcceptsValidSystemEventWithZeroTurnAndActionSlot; bagian berikut berada di luar batas blok tersebut dalam
    // Validate_AcceptsValidSystemEventWithZeroTurnAndActionSlot.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Validate_AcceptsActionSlotAboveTwoForRulesetDrivenLimit` dengan hasil bertipe `void`; operasi ini menangani validate
    // accepts aksi slot above two untuk aturan driven limit.
    public void Validate_AcceptsActionSlotAboveTwoForRulesetDrivenLimit()
    // Membuka scope metode Validate_AcceptsActionSlotAboveTwoForRulesetDrivenLimit; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Validate_AcceptsActionSlotAboveTwoForRulesetDrivenLimit.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan `CreateRequest() with {
        // UserId = playerId, ActionSlot = 3 }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest() with { UserId = playerId, ActionSlot = 3 };

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil `new
        // EventRequestShapeValidator().Validate` dengan `request`, `playerId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: playerId);

        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // Validate_AcceptsActionSlotAboveTwoForRulesetDrivenLimit.
        Assert.True(result.IsValid);
    // Menutup scope metode Validate_AcceptsActionSlotAboveTwoForRulesetDrivenLimit; bagian berikut berada di luar batas blok tersebut dalam
    // Validate_AcceptsActionSlotAboveTwoForRulesetDrivenLimit.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Validate_RejectsSystemActionSlotAboveZero` dengan hasil bertipe `void`; operasi ini menangani validate rejects system aksi
    // slot above zero.
    public void Validate_RejectsSystemActionSlotAboveZero()
    // Membuka scope metode Validate_RejectsSystemActionSlotAboveZero; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Validate_RejectsSystemActionSlotAboveZero.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan `CreateRequest() with {
        // ActorType = ”SYSTEM”, UserId = null, ActionSlot = 1, TurnNumber = 0 }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest() with
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Validate_RejectsSystemActionSlotAboveZero.
        {
            // Memperbarui `ActorType` menggunakan nilai literal `”SYSTEM”` dalam Validate_RejectsSystemActionSlotAboveZero.
            ActorType = "SYSTEM",
            // Memperbarui `UserId` menggunakan null, yaitu penanda tidak ada nilai dalam Validate_RejectsSystemActionSlotAboveZero.
            UserId = null,
            // Memperbarui `ActionSlot` menggunakan nilai literal `1` dalam Validate_RejectsSystemActionSlotAboveZero.
            ActionSlot = 1,
            // Memperbarui `TurnNumber` menggunakan nilai literal `0` dalam Validate_RejectsSystemActionSlotAboveZero.
            TurnNumber = 0
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Validate_RejectsSystemActionSlotAboveZero.
        };

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil `new
        // EventRequestShapeValidator().Validate` dengan `request`, `null`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // Validate_RejectsSystemActionSlotAboveZero.
        Assert.False(result.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`StatusCodes.Status400BadRequest`,
        // `result.StatusCode`); pengujian gagal jika keduanya berbeda dalam Validate_RejectsSystemActionSlotAboveZero.
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `result.Details`, `detail => detail.Field
        // == ”action_slot” && detail.Issue == ”INVALID_FOR_ACTOR”` dalam Validate_RejectsSystemActionSlotAboveZero.
        Assert.Contains(result.Details, detail => detail.Field == "action_slot" && detail.Issue == "INVALID_FOR_ACTOR");
    // Menutup scope metode Validate_RejectsSystemActionSlotAboveZero; bagian berikut berada di luar batas blok tersebut dalam
    // Validate_RejectsSystemActionSlotAboveZero.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Validate_RejectsPlayerActionSlotZero` dengan hasil bertipe `void`; operasi ini menangani validate rejects pemain aksi slot
    // zero.
    public void Validate_RejectsPlayerActionSlotZero()
    // Membuka scope metode Validate_RejectsPlayerActionSlotZero; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Validate_RejectsPlayerActionSlotZero.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan `CreateRequest() with {
        // ActionSlot = 0 }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest() with { ActionSlot = 0 };

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil `new
        // EventRequestShapeValidator().Validate` dengan `request`, `null`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // Validate_RejectsPlayerActionSlotZero.
        Assert.False(result.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`StatusCodes.Status400BadRequest`,
        // `result.StatusCode`); pengujian gagal jika keduanya berbeda dalam Validate_RejectsPlayerActionSlotZero.
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `result.Details`, `detail => detail.Field
        // == ”action_slot” && detail.Issue == ”OUT_OF_RANGE”` dalam Validate_RejectsPlayerActionSlotZero.
        Assert.Contains(result.Details, detail => detail.Field == "action_slot" && detail.Issue == "OUT_OF_RANGE");
    // Menutup scope metode Validate_RejectsPlayerActionSlotZero; bagian berikut berada di luar batas blok tersebut dalam
    // Validate_RejectsPlayerActionSlotZero.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”JumatBerkah”).
    [InlineData("JumatBerkah")]
    // menyediakan satu kombinasi masukan pengujian (”RisikoKehidupan”).
    [InlineData("RisikoKehidupan")]
    // menyediakan satu kombinasi masukan pengujian (”BayarRisiko”).
    [InlineData("BayarRisiko")]
    // menyediakan satu kombinasi masukan pengujian (”GunakanOpsiDarurat”).
    [InlineData("GunakanOpsiDarurat")]
    // menyediakan satu kombinasi masukan pengujian (”InvestasiEmas”).
    [InlineData("InvestasiEmas")]
    // menyediakan satu kombinasi masukan pengujian (”JualEmas”).
    [InlineData("JualEmas")]
    // menyediakan satu kombinasi masukan pengujian (”LewatiTransaksiEmas”).
    [InlineData("LewatiTransaksiEmas")]
    // Mendefinisikan metode `Validate_AcceptsPlayerActionSlotZeroForRulebookFreeActions` dengan hasil bertipe `void`; operasi ini menangani validate
    // accepts pemain aksi slot zero untuk rulebook free aksi. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
    public void Validate_AcceptsPlayerActionSlotZeroForRulebookFreeActions(string actionType)
    // Membuka scope metode Validate_AcceptsPlayerActionSlotZeroForRulebookFreeActions; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam Validate_AcceptsPlayerActionSlotZeroForRulebookFreeActions.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan `CreateRequest() with {
        // ActionType = actionType, ActionSlot = 0 }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest() with { ActionType = actionType, ActionSlot = 0 };

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil `new
        // EventRequestShapeValidator().Validate` dengan `request`, `null`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        // Menjalankan pemeriksaan bahwa `result.IsValid`, `result.Message` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // Validate_AcceptsPlayerActionSlotZeroForRulebookFreeActions.
        Assert.True(result.IsValid, result.Message);
    // Menutup scope metode Validate_AcceptsPlayerActionSlotZeroForRulebookFreeActions; bagian berikut berada di luar batas blok tersebut dalam
    // Validate_AcceptsPlayerActionSlotZeroForRulebookFreeActions.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”JumatBerkah”).
    [InlineData("JumatBerkah")]
    // menyediakan satu kombinasi masukan pengujian (”RisikoKehidupan”).
    [InlineData("RisikoKehidupan")]
    // menyediakan satu kombinasi masukan pengujian (”GunakanOpsiDarurat”).
    [InlineData("GunakanOpsiDarurat")]
    // menyediakan satu kombinasi masukan pengujian (”InvestasiEmas”).
    [InlineData("InvestasiEmas")]
    // menyediakan satu kombinasi masukan pengujian (”JualEmas”).
    [InlineData("JualEmas")]
    // menyediakan satu kombinasi masukan pengujian (”LewatiTransaksiEmas”).
    [InlineData("LewatiTransaksiEmas")]
    // Mendefinisikan metode `Validate_RejectsNonZeroSlotForRulebookFreeActions` dengan hasil bertipe `void`; operasi ini menangani validate rejects non
    // zero slot untuk rulebook free aksi. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
    public void Validate_RejectsNonZeroSlotForRulebookFreeActions(string actionType)
    // Membuka scope metode Validate_RejectsNonZeroSlotForRulebookFreeActions; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Validate_RejectsNonZeroSlotForRulebookFreeActions.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan `CreateRequest() with {
        // ActionType = actionType, ActionSlot = 1 }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest() with { ActionType = actionType, ActionSlot = 1 };

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil `new
        // EventRequestShapeValidator().Validate` dengan `request`, `null`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // Validate_RejectsNonZeroSlotForRulebookFreeActions.
        Assert.False(result.IsValid);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `result.Details`, `detail => detail.Field
        // == ”action_slot” && detail.Issue == ”INVALID_FOR_ACTION”` dalam Validate_RejectsNonZeroSlotForRulebookFreeActions.
        Assert.Contains(result.Details, detail => detail.Field == "action_slot" && detail.Issue == "INVALID_FOR_ACTION");
    // Menutup scope metode Validate_RejectsNonZeroSlotForRulebookFreeActions; bagian berikut berada di luar batas blok tersebut dalam
    // Validate_RejectsNonZeroSlotForRulebookFreeActions.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”CatatTransaksi”).
    [InlineData("CatatTransaksi")]
    // menyediakan satu kombinasi masukan pengujian (”HariMingguLibur”).
    [InlineData("HariMingguLibur")]
    // menyediakan satu kombinasi masukan pengujian (”AkhirGiliran”).
    [InlineData("AkhirGiliran")]
    // Mendefinisikan metode `Validate_RejectsSystemOnlyActionsFromPlayer` dengan hasil bertipe `void`; operasi ini menangani validate rejects system
    // only aksi dari pemain. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
    public void Validate_RejectsSystemOnlyActionsFromPlayer(string actionType)
    // Membuka scope metode Validate_RejectsSystemOnlyActionsFromPlayer; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Validate_RejectsSystemOnlyActionsFromPlayer.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan `CreateRequest() with {
        // ActionType = actionType, ActionSlot = 0 }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest() with { ActionType = actionType, ActionSlot = 0 };

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil `new
        // EventRequestShapeValidator().Validate` dengan `request`, `null`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // Validate_RejectsSystemOnlyActionsFromPlayer.
        Assert.False(result.IsValid);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `result.Details`, `detail => detail.Field
        // == ”actor_type” && detail.Issue == ”SYSTEM_REQUIRED”` dalam Validate_RejectsSystemOnlyActionsFromPlayer.
        Assert.Contains(result.Details, detail => detail.Field == "actor_type" && detail.Issue == "SYSTEM_REQUIRED");
    // Menutup scope metode Validate_RejectsSystemOnlyActionsFromPlayer; bagian berikut berada di luar batas blok tersebut dalam
    // Validate_RejectsSystemOnlyActionsFromPlayer.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”CatatTransaksi”).
    [InlineData("CatatTransaksi")]
    // menyediakan satu kombinasi masukan pengujian (”HariMingguLibur”).
    [InlineData("HariMingguLibur")]
    // menyediakan satu kombinasi masukan pengujian (”AkhirGiliran”).
    [InlineData("AkhirGiliran")]
    // Mendefinisikan metode `Validate_AcceptsSystemOnlyActionsWithZeroTurnAndSlot` dengan hasil bertipe `void`; operasi ini menangani validate accepts
    // system only aksi dengan zero giliran dan slot. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
    public void Validate_AcceptsSystemOnlyActionsWithZeroTurnAndSlot(string actionType)
    // Membuka scope metode Validate_AcceptsSystemOnlyActionsWithZeroTurnAndSlot; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Validate_AcceptsSystemOnlyActionsWithZeroTurnAndSlot.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan `CreateRequest() with {
        // ActionType = actionType, ActorType = ”SYSTEM”, UserId = null, TurnNumber = 0, ActionSlot = 0 }`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var request = CreateRequest() with
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Validate_AcceptsSystemOnlyActionsWithZeroTurnAndSlot.
        {
            // Memperbarui `ActionType` menggunakan `actionType` (nilai aksi jenis) dalam Validate_AcceptsSystemOnlyActionsWithZeroTurnAndSlot.
            ActionType = actionType,
            // Memperbarui `ActorType` menggunakan nilai literal `”SYSTEM”` dalam Validate_AcceptsSystemOnlyActionsWithZeroTurnAndSlot.
            ActorType = "SYSTEM",
            // Memperbarui `UserId` menggunakan null, yaitu penanda tidak ada nilai dalam Validate_AcceptsSystemOnlyActionsWithZeroTurnAndSlot.
            UserId = null,
            // Memperbarui `TurnNumber` menggunakan nilai literal `0` dalam Validate_AcceptsSystemOnlyActionsWithZeroTurnAndSlot.
            TurnNumber = 0,
            // Memperbarui `ActionSlot` menggunakan nilai literal `0` dalam Validate_AcceptsSystemOnlyActionsWithZeroTurnAndSlot.
            ActionSlot = 0
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Validate_AcceptsSystemOnlyActionsWithZeroTurnAndSlot.
        };

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil `new
        // EventRequestShapeValidator().Validate` dengan `request`, `null`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        // Menjalankan pemeriksaan bahwa `result.IsValid`, `result.Message` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // Validate_AcceptsSystemOnlyActionsWithZeroTurnAndSlot.
        Assert.True(result.IsValid, result.Message);
    // Menutup scope metode Validate_AcceptsSystemOnlyActionsWithZeroTurnAndSlot; bagian berikut berada di luar batas blok tersebut dalam
    // Validate_AcceptsSystemOnlyActionsWithZeroTurnAndSlot.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”Asuransi”).
    [InlineData("Asuransi")]
    // menyediakan satu kombinasi masukan pengujian (”PinjamanSyariah”).
    [InlineData("PinjamanSyariah")]
    // Mendefinisikan metode `Validate_AcceptsPlayerActionSlotZeroForRiskResponses` dengan hasil bertipe `void`; operasi ini menangani validate accepts
    // pemain aksi slot zero untuk risiko responses. Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
    public void Validate_AcceptsPlayerActionSlotZeroForRiskResponses(string actionType)
    // Membuka scope metode Validate_AcceptsPlayerActionSlotZeroForRiskResponses; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Validate_AcceptsPlayerActionSlotZeroForRiskResponses.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan
        // `”””{”risk_event_id”:”95000000-0000-0000-0000-000000000123”}”””`. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber
        // daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse("""{"risk_event_id":"95000000-0000-0000-0000-000000000123"}""");
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan `CreateRequest() with {
        // ActionType = actionType, ActionSlot = 0, Payload = document.RootElement.Clone() }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest() with
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Validate_AcceptsPlayerActionSlotZeroForRiskResponses.
        {
            // Memperbarui `ActionType` menggunakan `actionType` (nilai aksi jenis) dalam Validate_AcceptsPlayerActionSlotZeroForRiskResponses.
            ActionType = actionType,
            // Memperbarui `ActionSlot` menggunakan nilai literal `0` dalam Validate_AcceptsPlayerActionSlotZeroForRiskResponses.
            ActionSlot = 0,
            // Memperbarui `Payload` menggunakan membuat salinan `document.RootElement` agar hasil dapat digunakan terpisah dari objek sumber dalam
            // Validate_AcceptsPlayerActionSlotZeroForRiskResponses.
            Payload = document.RootElement.Clone()
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // Validate_AcceptsPlayerActionSlotZeroForRiskResponses.
        };

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil `new
        // EventRequestShapeValidator().Validate` dengan `request`, `null`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        // Menjalankan pemeriksaan bahwa `result.IsValid`, `result.Message` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // Validate_AcceptsPlayerActionSlotZeroForRiskResponses.
        Assert.True(result.IsValid, result.Message);
    // Menutup scope metode Validate_AcceptsPlayerActionSlotZeroForRiskResponses; bagian berikut berada di luar batas blok tersebut dalam
    // Validate_AcceptsPlayerActionSlotZeroForRiskResponses.
    }

    // menandai metode sebagai pengujian xUnit yang dijalankan untuk setiap kombinasi data.
    [Theory]
    // menyediakan satu kombinasi masukan pengujian (”Asuransi”).
    [InlineData("Asuransi")]
    // menyediakan satu kombinasi masukan pengujian (”PinjamanSyariah”).
    [InlineData("PinjamanSyariah")]
    // Mendefinisikan metode `Validate_RejectsPlayerActionSlotZeroForRiskActionsWithoutRiskReference` dengan hasil bertipe `void`; operasi ini menangani
    // validate rejects pemain aksi slot zero untuk risiko aksi tanpa risiko reference. Masukan: Parameter `actionType` bertipe `string` membawa nilai
    // aksi jenis.
    public void Validate_RejectsPlayerActionSlotZeroForRiskActionsWithoutRiskReference(string actionType)
    // Membuka scope metode Validate_RejectsPlayerActionSlotZeroForRiskActionsWithoutRiskReference; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam Validate_RejectsPlayerActionSlotZeroForRiskActionsWithoutRiskReference.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan `CreateRequest() with {
        // ActionType = actionType, ActionSlot = 0 }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest() with { ActionType = actionType, ActionSlot = 0 };

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil `new
        // EventRequestShapeValidator().Validate` dengan `request`, `null`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // Validate_RejectsPlayerActionSlotZeroForRiskActionsWithoutRiskReference.
        Assert.False(result.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`StatusCodes.Status400BadRequest`,
        // `result.StatusCode`); pengujian gagal jika keduanya berbeda dalam Validate_RejectsPlayerActionSlotZeroForRiskActionsWithoutRiskReference.
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `result.Details`, `detail => detail.Field
        // == ”action_slot” && detail.Issue == ”OUT_OF_RANGE”` dalam Validate_RejectsPlayerActionSlotZeroForRiskActionsWithoutRiskReference.
        Assert.Contains(result.Details, detail => detail.Field == "action_slot" && detail.Issue == "OUT_OF_RANGE");
    // Menutup scope metode Validate_RejectsPlayerActionSlotZeroForRiskActionsWithoutRiskReference; bagian berikut berada di luar batas blok tersebut
    // dalam Validate_RejectsPlayerActionSlotZeroForRiskActionsWithoutRiskReference.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `Validate_RejectsNegativeTurnNumber` dengan hasil bertipe `void`; operasi ini menangani validate rejects negative giliran
    // number.
    public void Validate_RejectsNegativeTurnNumber()
    // Membuka scope metode Validate_RejectsNegativeTurnNumber; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // Validate_RejectsNegativeTurnNumber.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan `CreateRequest() with {
        // TurnNumber = -1 }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest() with { TurnNumber = -1 };

        // Menyiapkan variabel lokal `result` untuk nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya dengan memanggil `new
        // EventRequestShapeValidator().Validate` dengan `request`, `null`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var result = new EventRequestShapeValidator().Validate(request, scopedPlayerId: null);

        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // Validate_RejectsNegativeTurnNumber.
        Assert.False(result.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`StatusCodes.Status400BadRequest`,
        // `result.StatusCode`); pengujian gagal jika keduanya berbeda dalam Validate_RejectsNegativeTurnNumber.
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `result.Details`, `detail => detail.Field
        // == ”turn_number” && detail.Issue == ”OUT_OF_RANGE”` dalam Validate_RejectsNegativeTurnNumber.
        Assert.Contains(result.Details, detail => detail.Field == "turn_number" && detail.Issue == "OUT_OF_RANGE");
    // Menutup scope metode Validate_RejectsNegativeTurnNumber; bagian berikut berada di luar batas blok tersebut dalam
    // Validate_RejectsNegativeTurnNumber.
    }

    // Mendefinisikan metode `CreateRequest` dengan hasil bertipe `EventRequest`; operasi ini menangani create permintaan.
    private static EventRequest CreateRequest()
    // Membuka scope metode CreateRequest; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateRequest.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `”{}”`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse("{}");
        // Mengembalikan objek baru bertipe `EventRequest` dengan argumen ( Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), ”PLAYER”, new
        // DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero), 0, ”MON”, 1, 0, ”KerjaLepas”, Guid.NewGuid()... kepada pemanggil dalam CreateRequest;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new EventRequest(
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
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
            // Meneruskan nilai literal `”KerjaLepas”` sebagai argumen ke konstruktor `EventRequest`.
            "KerjaLepas",
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan membuat salinan `document.RootElement` agar hasil dapat digunakan terpisah dari objek sumber sebagai argumen ke konstruktor
            // `EventRequest`.
            document.RootElement.Clone(),
            // Meneruskan nilai literal `”client-123”` sebagai argumen ke konstruktor `EventRequest`.
            "client-123",
            // Meneruskan nilai literal `1` sebagai argumen ke konstruktor `EventRequest`.
            1);
    // Menutup scope metode CreateRequest; bagian berikut berada di luar batas blok tersebut dalam CreateRequest.
    }
// Menutup scope tipe EventRequestShapeValidatorTests; bagian berikut berada di luar batas blok tersebut.
}
