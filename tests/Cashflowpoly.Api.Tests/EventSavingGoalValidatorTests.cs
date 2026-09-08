// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui EventSavingGoalValidatorTests.
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

// Mendefinisikan tipe class `EventSavingGoalValidatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventSavingGoalValidatorTests
// Membuka scope tipe EventSavingGoalValidatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_ReturnsFalseForUnhandledAction` dengan hasil bertipe `void`; operasi ini menangani try validate returns false
    // untuk unhandled aksi.
    public void TryValidate_ReturnsFalseForUnhandledAction()
    // Membuka scope metode TryValidate_ReturnsFalseForUnhandledAction; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidate_ReturnsFalseForUnhandledAction.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”CatatTransaksi”`, `”””{”amount”:1}”””`, `Guid.NewGuid()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("CatatTransaksi", """{"amount":1}""", Guid.NewGuid());

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventSavingGoalValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `Array.Empty<EventDb>()`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventSavingGoalValidator().TryValidate(request, CreateConfig(), Array.Empty<EventDb>(), out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidate_ReturnsFalseForUnhandledAction.
        Assert.False(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidate_ReturnsFalseForUnhandledAction.
        Assert.True(result.Validation.IsValid);
    // Menutup scope metode TryValidate_ReturnsFalseForUnhandledAction; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_ReturnsFalseForUnhandledAction.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateDeposit_RejectsDisabledFeature` dengan hasil bertipe `void`; operasi ini menangani try validate deposit rejects
    // disabled feature.
    public void TryValidateDeposit_RejectsDisabledFeature()
    // Membuka scope metode TryValidateDeposit_RejectsDisabledFeature; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidateDeposit_RejectsDisabledFeature.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”Menabung”`, `”””{”goal_id”:”goal-a”,”amount”:5}”””`, `Guid.NewGuid()`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var request = CreateRequest("Menabung", """{"goal_id":"goal-a","amount":5}""", Guid.NewGuid());

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventSavingGoalValidator().TryValidate` dengan `request`,
        // `CreateConfig(enabled: false)`, `Array.Empty<EventDb>()`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventSavingGoalValidator().TryValidate(request, CreateConfig(enabled: false), Array.Empty<EventDb>(), out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateDeposit_RejectsDisabledFeature.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidateDeposit_RejectsDisabledFeature.
        Assert.False(result.Validation.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`StatusCodes.Status422UnprocessableEntity`,
        // `result.Validation.StatusCode`); pengujian gagal jika keduanya berbeda dalam TryValidateDeposit_RejectsDisabledFeature.
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.Validation.StatusCode);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Fitur tabungan tujuan tidak aktif”`,
        // `result.Validation.Message`); pengujian gagal jika keduanya berbeda dalam TryValidateDeposit_RejectsDisabledFeature.
        Assert.Equal("Fitur tabungan tujuan tidak aktif", result.Validation.Message);
    // Menutup scope metode TryValidateDeposit_RejectsDisabledFeature; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidateDeposit_RejectsDisabledFeature.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateDeposit_ReturnsOutgoingAmountWhenValid` dengan hasil bertipe `void`; operasi ini menangani try validate deposit
    // returns outgoing nominal when valid.
    public void TryValidateDeposit_ReturnsOutgoingAmountWhenValid()
    // Membuka scope metode TryValidateDeposit_ReturnsOutgoingAmountWhenValid; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidateDeposit_ReturnsOutgoingAmountWhenValid.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”Menabung”`, `”””{”goal_id”:”goal-a”,”amount”:15}”””`, `Guid.NewGuid()`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var request = CreateRequest("Menabung", """{"goal_id":"goal-a","amount":15}""", Guid.NewGuid());

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventSavingGoalValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `Array.Empty<EventDb>()`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventSavingGoalValidator().TryValidate(request, CreateConfig(), Array.Empty<EventDb>(), out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateDeposit_ReturnsOutgoingAmountWhenValid.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateDeposit_ReturnsOutgoingAmountWhenValid.
        Assert.True(result.Validation.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`15`, `result.OutgoingAmount`); pengujian gagal
        // jika keduanya berbeda dalam TryValidateDeposit_ReturnsOutgoingAmountWhenValid.
        Assert.Equal(15, result.OutgoingAmount);
    // Menutup scope metode TryValidateDeposit_ReturnsOutgoingAmountWhenValid; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidateDeposit_ReturnsOutgoingAmountWhenValid.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateWithdraw_RejectsWithdrawBecauseRulebookHasNoWithdrawAction` dengan hasil bertipe `void`; operasi ini menangani
    // try validate withdraw rejects withdraw because rulebook memiliki no withdraw aksi.
    public void TryValidateWithdraw_RejectsWithdrawBecauseRulebookHasNoWithdrawAction()
    // Membuka scope metode TryValidateWithdraw_RejectsWithdrawBecauseRulebookHasNoWithdrawAction; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam TryValidateWithdraw_RejectsWithdrawBecauseRulebookHasNoWithdrawAction.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”TarikTabungan”`, `”””{”goal_id”:”goal-a”,”amount”:5}”””`, `playerId`. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var request = CreateRequest("TarikTabungan", """{"goal_id":"goal-a","amount":5}""", playerId);
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidateWithdraw_RejectsWithdrawBecauseRulebookHasNoWithdrawAction.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”Menabung”`, `”””{”goal_id”:”goal-a”,”amount”:5}”””`, `playerId` dalam
            // TryValidateWithdraw_RejectsWithdrawBecauseRulebookHasNoWithdrawAction.
            CreateEvent("Menabung", """{"goal_id":"goal-a","amount":5}""", playerId)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidateWithdraw_RejectsWithdrawBecauseRulebookHasNoWithdrawAction.
        };

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventSavingGoalValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `history`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventSavingGoalValidator().TryValidate(request, CreateConfig(), history, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateWithdraw_RejectsWithdrawBecauseRulebookHasNoWithdrawAction.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidateWithdraw_RejectsWithdrawBecauseRulebookHasNoWithdrawAction.
        Assert.False(result.Validation.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”TarikTabungan bukan aksi resmi ruleset
        // rulebook”`, `result.Validation.Message`); pengujian gagal jika keduanya berbeda dalam
        // TryValidateWithdraw_RejectsWithdrawBecauseRulebookHasNoWithdrawAction.
        Assert.Equal("TarikTabungan bukan aksi resmi ruleset rulebook", result.Validation.Message);
    // Menutup scope metode TryValidateWithdraw_RejectsWithdrawBecauseRulebookHasNoWithdrawAction; bagian berikut berada di luar batas blok tersebut
    // dalam TryValidateWithdraw_RejectsWithdrawBecauseRulebookHasNoWithdrawAction.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateGoalAchieved_RejectsCostAboveSavingBalance` dengan hasil bertipe `void`; operasi ini menangani try validate
    // target achieved rejects biaya above tabungan saldo.
    public void TryValidateGoalAchieved_RejectsCostAboveSavingBalance()
    // Membuka scope metode TryValidateGoalAchieved_RejectsCostAboveSavingBalance; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidateGoalAchieved_RejectsCostAboveSavingBalance.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan
        // `CreateRequest(”TujuanFinansial”, ”””{”goal_id”:”goal-a”,”points”:10,”cost”:8}”””, playerId) with { ActorType = ”SYSTEM”, ActionSlot = 0,
        // TurnNumber = 0 }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("TujuanFinansial", """{"goal_id":"goal-a","points":10,"cost":8}""", playerId) with
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidateGoalAchieved_RejectsCostAboveSavingBalance.
        {
            // Memperbarui `ActorType` menggunakan nilai literal `”SYSTEM”` dalam TryValidateGoalAchieved_RejectsCostAboveSavingBalance.
            ActorType = "SYSTEM",
            // Memperbarui `ActionSlot` menggunakan nilai literal `0` dalam TryValidateGoalAchieved_RejectsCostAboveSavingBalance.
            ActionSlot = 0,
            // Memperbarui `TurnNumber` menggunakan nilai literal `0` dalam TryValidateGoalAchieved_RejectsCostAboveSavingBalance.
            TurnNumber = 0
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidateGoalAchieved_RejectsCostAboveSavingBalance.
        };
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidateGoalAchieved_RejectsCostAboveSavingBalance.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”Menabung”`, `”””{”goal_id”:”goal-a”,”amount”:5}”””`, `playerId` dalam
            // TryValidateGoalAchieved_RejectsCostAboveSavingBalance.
            CreateEvent("Menabung", """{"goal_id":"goal-a","amount":5}""", playerId)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidateGoalAchieved_RejectsCostAboveSavingBalance.
        };

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventSavingGoalValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `history`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventSavingGoalValidator().TryValidate(request, CreateConfig(), history, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateGoalAchieved_RejectsCostAboveSavingBalance.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidateGoalAchieved_RejectsCostAboveSavingBalance.
        Assert.False(result.Validation.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Saldo tabungan tidak mencukupi untuk goal”`,
        // `result.Validation.Message`); pengujian gagal jika keduanya berbeda dalam TryValidateGoalAchieved_RejectsCostAboveSavingBalance.
        Assert.Equal("Saldo tabungan tidak mencukupi untuk goal", result.Validation.Message);
    // Menutup scope metode TryValidateGoalAchieved_RejectsCostAboveSavingBalance; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidateGoalAchieved_RejectsCostAboveSavingBalance.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateGoalAchieved_RejectsSeparatePlayerAction` dengan hasil bertipe `void`; operasi ini menangani try validate
    // target achieved rejects separate pemain aksi.
    public void TryValidateGoalAchieved_RejectsSeparatePlayerAction()
    // Membuka scope metode TryValidateGoalAchieved_RejectsSeparatePlayerAction; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidateGoalAchieved_RejectsSeparatePlayerAction.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”TujuanFinansial”`, `”””{”goal_id”:”goal-a”,”points”:20,”cost”:25}”””`, `playerId`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var request = CreateRequest("TujuanFinansial", """{"goal_id":"goal-a","points":20,"cost":25}""", playerId);

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventSavingGoalValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `Array.Empty<EventDb>()`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventSavingGoalValidator().TryValidate(request, CreateConfig(), Array.Empty<EventDb>(), out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateGoalAchieved_RejectsSeparatePlayerAction.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidateGoalAchieved_RejectsSeparatePlayerAction.
        Assert.False(result.Validation.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”TujuanFinansial bukan aksi pemain terpisah;
        // kartu tujuan diperoleh otomatis saat Menabung mencapai target”`, `result.Validation.Message`); pengujian gagal jika keduanya berbeda dalam
        // TryValidateGoalAchieved_RejectsSeparatePlayerAction.
        Assert.Equal("TujuanFinansial bukan aksi pemain terpisah; kartu tujuan diperoleh otomatis saat Menabung mencapai target", result.Validation.Message);
    // Menutup scope metode TryValidateGoalAchieved_RejectsSeparatePlayerAction; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidateGoalAchieved_RejectsSeparatePlayerAction.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken` dengan hasil bertipe `void`; operasi ini menangani try validate
    // target achieved rejects target kartu already taken.
    public void TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken()
    // Membuka scope metode TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan
        // `CreateRequest(”TujuanFinansial”, ”””{”goal_id”:”goal-a”,”points”:10,”cost”:8}”””, playerId) with { ActorType = ”SYSTEM”, ActionSlot = 0,
        // TurnNumber = 0 }`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("TujuanFinansial", """{"goal_id":"goal-a","points":10,"cost":8}""", playerId) with
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken.
        {
            // Memperbarui `ActorType` menggunakan nilai literal `”SYSTEM”` dalam TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken.
            ActorType = "SYSTEM",
            // Memperbarui `ActionSlot` menggunakan nilai literal `0` dalam TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken.
            ActionSlot = 0,
            // Memperbarui `TurnNumber` menggunakan nilai literal `0` dalam TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken.
            TurnNumber = 0
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken.
        };
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”Menabung”`, `”””{”goal_id”:”goal-a”,”amount”:8}”””`, `playerId` dalam
            // TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken.
            CreateEvent("Menabung", """{"goal_id":"goal-a","amount":8}""", playerId),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”TujuanFinansial”`, `”””{”goal_id”:”goal-a”,”points”:10,”cost”:8}”””`,
            // `Guid.NewGuid()` dalam TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken.
            CreateEvent("TujuanFinansial", """{"goal_id":"goal-a","points":10,"cost":8}""", Guid.NewGuid())
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken.
        };

        // Menjalankan memanggil `new EventSavingGoalValidator().TryValidate` dengan `request`, `CreateConfig()`, `history`, `var result` dalam
        // TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken.
        new EventSavingGoalValidator().TryValidate(request, CreateConfig(), history, out var result);

        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken.
        Assert.False(result.Validation.IsValid);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”pemain lain”`,
        // `result.Validation.Message` dalam TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken.
        Assert.Contains("pemain lain", result.Validation.Message);
    // Menutup scope metode TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidateGoalAchieved_RejectsGoalCardAlreadyTaken.
    }

    // Mendefinisikan metode `CreateRequest` dengan hasil bertipe `EventRequest`; operasi ini menangani create permintaan. Masukan: Parameter
    // `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `playerId`
    // bertipe `Guid?` membawa nilai pemain identitas; nilai null diizinkan ketika data opsional belum tersedia.
    private static EventRequest CreateRequest(string actionType, string payloadJson, Guid? playerId)
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
            "client-123",
            // Meneruskan nilai literal `1` sebagai argumen ke konstruktor `EventRequest`.
            1);
    // Menutup scope metode CreateRequest; bagian berikut berada di luar batas blok tersebut dalam CreateRequest.
    }

    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `EventDb`; operasi ini menangani create event. Masukan: Parameter `actionType` bertipe
    // `string` membawa nilai aksi jenis; Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `playerId` bertipe `Guid`
    // membawa nilai pemain identitas.
    private static EventDb CreateEvent(string actionType, string payloadJson, Guid playerId)
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
            // Memperbarui `Payload` menggunakan `payloadJson` (nilai payload JSON) dalam CreateEvent.
            Payload = payloadJson
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
        };
    // Menutup scope metode CreateEvent; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
    }

    // Mendefinisikan metode `CreateConfig` dengan hasil bertipe `RulesetConfig`; operasi ini menangani create konfigurasi. Masukan: Parameter `enabled`
    // bertipe `bool` membawa nilai enabled; bila argumen tidak diberikan digunakan true, yaitu kondisi aktif/terpenuhi.
    private static RulesetConfig CreateConfig(bool enabled = true)
    // Membuka scope metode CreateConfig; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateConfig.
    {
        // Mengembalikan objek baru bertipe `RulesetConfig` dengan argumen ( ”MAHIR”, ActionsPerTurn: 3, StartingCash: 20, PlayerOrdering.PlayerOrder,
        // CashMin: 0, MaxIngredientTotal: 10, MaxSameIngredient: 5, PrimaryNeedMaxPerDay: 1, R... kepada pemanggil dalam CreateConfig; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return new RulesetConfig(
            // Meneruskan nilai literal `”MAHIR”` sebagai argumen ke konstruktor `RulesetConfig`.
            "MAHIR",
            // Meneruskan nilai literal `3` sebagai argumen bernama `ActionsPerTurn`.
            ActionsPerTurn: 3,
            // Meneruskan nilai literal `20` sebagai argumen bernama `StartingCash`.
            StartingCash: 20,
            // Meneruskan `PlayerOrdering.PlayerOrder` (nomor urut pemain untuk menentukan urutan tindakan) sebagai argumen ke konstruktor `RulesetConfig`.
            PlayerOrdering.PlayerOrder,
            // Meneruskan nilai literal `0` sebagai argumen bernama `CashMin`.
            CashMin: 0,
            // Meneruskan nilai literal `10` sebagai argumen bernama `MaxIngredientTotal`.
            MaxIngredientTotal: 10,
            // Meneruskan nilai literal `5` sebagai argumen bernama `MaxSameIngredient`.
            MaxSameIngredient: 5,
            // Meneruskan nilai literal `1` sebagai argumen bernama `PrimaryNeedMaxPerDay`.
            PrimaryNeedMaxPerDay: 1,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `RequirePrimaryBeforeOthers`.
            RequirePrimaryBeforeOthers: true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `FridayEnabled`.
            FridayEnabled: true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `SaturdayEnabled`.
            SaturdayEnabled: true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `SundayEnabled`.
            SundayEnabled: true,
            // Meneruskan nilai literal `1` sebagai argumen bernama `DonationMin`.
            DonationMin: 1,
            // Meneruskan nilai literal `10` sebagai argumen bernama `DonationMax`.
            DonationMax: 10,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `GoldAllowBuy`.
            GoldAllowBuy: true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `GoldAllowSell`.
            GoldAllowSell: true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `LoanEnabled`.
            LoanEnabled: true,
            // Meneruskan true, yaitu kondisi aktif/terpenuhi sebagai argumen bernama `InsuranceEnabled`.
            InsuranceEnabled: true,
            // Meneruskan `enabled` (nilai enabled) sebagai argumen ke konstruktor `RulesetConfig`.
            enabled,
            // Meneruskan nilai literal `5` sebagai argumen bernama `FreelanceIncome`.
            FreelanceIncome: 5,
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `Scoring`.
            Scoring: null)
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateConfig.
        {
            // Memperbarui `FinancialGoals` menggunakan koleksi berisi new RulesetFinancialGoalDto { Id = ”goal-a”, Nama ... dalam CreateConfig.
            FinancialGoals =
            // Menggunakan koleksi berisi new RulesetFinancialGoalDto { Id = ”goal-a”, Nama ... sebagai bagian ekspresi yang sedang disusun dalam CreateConfig.
            [
                // Menggunakan objek baru bertipe `RulesetFinancialGoalDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun
                // dalam CreateConfig.
                new RulesetFinancialGoalDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateConfig.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”goal-a”` dalam CreateConfig.
                    Id = "goal-a",
                    // Memperbarui `Nama` menggunakan nilai literal `”Tujuan A”` dalam CreateConfig.
                    Nama = "Tujuan A",
                    // Memperbarui `HargaBeli` menggunakan nilai literal `8` dalam CreateConfig.
                    HargaBeli = 8,
                    // Memperbarui `PoinKebahagiaan` menggunakan nilai literal `10` dalam CreateConfig.
                    PoinKebahagiaan = 10,
                    // Memperbarui `CardQty` menggunakan nilai literal `1` dalam CreateConfig.
                    CardQty = 1
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateConfig.
                }
            // Menandai akhir daftar elemen atau indeks koleksi dalam CreateConfig; pasangan kurung siku mengelompokkan nilai sebagai satu struktur.
            ]
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateConfig.
        };
    // Menutup scope metode CreateConfig; bagian berikut berada di luar batas blok tersebut dalam CreateConfig.
    }
// Menutup scope tipe EventSavingGoalValidatorTests; bagian berikut berada di luar batas blok tersebut.
}
