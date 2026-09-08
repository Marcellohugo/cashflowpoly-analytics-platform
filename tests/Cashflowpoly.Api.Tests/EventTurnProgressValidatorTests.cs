// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui EventTurnProgressValidatorTests.
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

// Mendefinisikan tipe class `EventTurnProgressValidatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventTurnProgressValidatorTests
// Membuka scope tipe EventTurnProgressValidatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RequiresHistory_ReturnsFalseForRemovedActionUsed` dengan hasil bertipe `void`; operasi ini menangani requires history
    // returns false untuk removed aksi used.
    public void RequiresHistory_ReturnsFalseForRemovedActionUsed()
    // Membuka scope metode RequiresHistory_ReturnsFalseForRemovedActionUsed; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // RequiresHistory_ReturnsFalseForRemovedActionUsed.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”turn.action.used”`, `”””{”used”:1,”remaining”:2}”””`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("turn.action.used", """{"used":1,"remaining":2}""");

        // Menyiapkan variabel lokal `requiresHistory` untuk nilai requires history dengan memanggil `new EventTurnProgressValidator().RequiresHistory`
        // dengan `request`, `CreateConfig()`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var requiresHistory = new EventTurnProgressValidator().RequiresHistory(request, CreateConfig());

        // Menjalankan pemeriksaan bahwa `requiresHistory` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // RequiresHistory_ReturnsFalseForRemovedActionUsed.
        Assert.False(requiresHistory);
    // Menutup scope metode RequiresHistory_ReturnsFalseForRemovedActionUsed; bagian berikut berada di luar batas blok tersebut dalam
    // RequiresHistory_ReturnsFalseForRemovedActionUsed.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `RequiresHistory_ReturnsTrueForAkhirGiliranMahir` dengan hasil bertipe `void`; operasi ini menangani requires history
    // returns true untuk akhir giliran mahir.
    public void RequiresHistory_ReturnsTrueForAkhirGiliranMahir()
    // Membuka scope metode RequiresHistory_ReturnsTrueForAkhirGiliranMahir; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // RequiresHistory_ReturnsTrueForAkhirGiliranMahir.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”AkhirGiliran”`, `”{}”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("AkhirGiliran", "{}");

        // Menyiapkan variabel lokal `requiresHistory` untuk nilai requires history dengan memanggil `new EventTurnProgressValidator().RequiresHistory`
        // dengan `request`, `CreateConfig(mode: ”MAHIR”)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var requiresHistory = new EventTurnProgressValidator().RequiresHistory(request, CreateConfig(mode: "MAHIR"));

        // Menjalankan pemeriksaan bahwa `requiresHistory` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // RequiresHistory_ReturnsTrueForAkhirGiliranMahir.
        Assert.True(requiresHistory);
    // Menutup scope metode RequiresHistory_ReturnsTrueForAkhirGiliranMahir; bagian berikut berada di luar batas blok tersebut dalam
    // RequiresHistory_ReturnsTrueForAkhirGiliranMahir.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateTurnEndedMahir_RejectsOrderRiskMismatch` dengan hasil bertipe `void`; operasi ini menangani try validate
    // giliran ended mahir rejects urutan/pesanan risiko mismatch.
    public void TryValidateTurnEndedMahir_RejectsOrderRiskMismatch()
    // Membuka scope metode TryValidateTurnEndedMahir_RejectsOrderRiskMismatch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidateTurnEndedMahir_RejectsOrderRiskMismatch.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”AkhirGiliran”`, `”{}”`, `sessionId`, `playerId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("AkhirGiliran", "{}", sessionId, playerId);
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidateTurnEndedMahir_RejectsOrderRiskMismatch.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”JualMasakan”`, `”””{”required_ingredient_card_ids”:[”A”],”income”:5}”””`,
            // `sessionId`, `playerId`, `1` dalam TryValidateTurnEndedMahir_RejectsOrderRiskMismatch.
            CreateEvent("JualMasakan", """{"required_ingredient_card_ids":["A"],"income":5}""", sessionId, playerId, actionSlot: 1),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”BahanMasakan”`, `”””{”card_id”:”A”,”amount”:1}”””`, `sessionId`, `playerId`, `2`
            // dalam TryValidateTurnEndedMahir_RejectsOrderRiskMismatch.
            CreateEvent("BahanMasakan", """{"card_id":"A","amount":1}""", sessionId, playerId, actionSlot: 2)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidateTurnEndedMahir_RejectsOrderRiskMismatch.
        };

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventTurnProgressValidator().TryValidate` dengan `request`,
        // `CreateConfig(mode: ”MAHIR”)`, `history`, `1`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventTurnProgressValidator().TryValidate(request, CreateConfig(mode: "MAHIR"), history, 1, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateTurnEndedMahir_RejectsOrderRiskMismatch.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidateTurnEndedMahir_RejectsOrderRiskMismatch.
        Assert.False(result.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`StatusCodes.Status422UnprocessableEntity`,
        // `result.StatusCode`); pengujian gagal jika keduanya berbeda dalam TryValidateTurnEndedMahir_RejectsOrderRiskMismatch.
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, result.StatusCode);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Setiap klaim pesanan harus diikuti
        // pengambilan risiko pada mode MAHIR”`, `result.Message`); pengujian gagal jika keduanya berbeda dalam
        // TryValidateTurnEndedMahir_RejectsOrderRiskMismatch.
        Assert.Equal("Setiap klaim pesanan harus diikuti pengambilan risiko pada mode MAHIR", result.Message);
    // Menutup scope metode TryValidateTurnEndedMahir_RejectsOrderRiskMismatch; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidateTurnEndedMahir_RejectsOrderRiskMismatch.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateTurnEndedMahir_AcceptsOrderPairedWithFreeRiskSlot` dengan hasil bertipe `void`; operasi ini menangani try
    // validate giliran ended mahir accepts urutan/pesanan paired dengan free risiko slot.
    public void TryValidateTurnEndedMahir_AcceptsOrderPairedWithFreeRiskSlot()
    // Membuka scope metode TryValidateTurnEndedMahir_AcceptsOrderPairedWithFreeRiskSlot; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam TryValidateTurnEndedMahir_AcceptsOrderPairedWithFreeRiskSlot.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”AkhirGiliran”`, `”{}”`, `sessionId`, `playerId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("AkhirGiliran", "{}", sessionId, playerId);
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidateTurnEndedMahir_AcceptsOrderPairedWithFreeRiskSlot.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”JualMasakan”`, `”””{”required_ingredient_card_ids”:[”A”],”income”:5}”””`,
            // `sessionId`, `playerId`, `1` dalam TryValidateTurnEndedMahir_AcceptsOrderPairedWithFreeRiskSlot.
            CreateEvent("JualMasakan", """{"required_ingredient_card_ids":["A"],"income":5}""", sessionId, playerId, actionSlot: 1),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”BahanMasakan”`, `”””{”card_id”:”A”,”amount”:1}”””`, `sessionId`, `playerId`, `2`
            // dalam TryValidateTurnEndedMahir_AcceptsOrderPairedWithFreeRiskSlot.
            CreateEvent("BahanMasakan", """{"card_id":"A","amount":1}""", sessionId, playerId, actionSlot: 2),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”RisikoKehidupan”`, `”””{”risk_id”:”risk-a”}”””`, `sessionId`, `playerId`, `0`
            // dalam TryValidateTurnEndedMahir_AcceptsOrderPairedWithFreeRiskSlot.
            CreateEvent("RisikoKehidupan", """{"risk_id":"risk-a"}""", sessionId, playerId, actionSlot: 0)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidateTurnEndedMahir_AcceptsOrderPairedWithFreeRiskSlot.
        };

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventTurnProgressValidator().TryValidate` dengan `request`,
        // `CreateConfig(mode: ”MAHIR”)`, `history`, `1`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventTurnProgressValidator().TryValidate(request, CreateConfig(mode: "MAHIR"), history, 1, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateTurnEndedMahir_AcceptsOrderPairedWithFreeRiskSlot.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateTurnEndedMahir_AcceptsOrderPairedWithFreeRiskSlot.
        Assert.True(result.IsValid);
    // Menutup scope metode TryValidateTurnEndedMahir_AcceptsOrderPairedWithFreeRiskSlot; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidateTurnEndedMahir_AcceptsOrderPairedWithFreeRiskSlot.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateTurnEnded_RejectsParticipantWithoutActions` dengan hasil bertipe `void`; operasi ini menangani try validate
    // giliran ended rejects participant tanpa aksi.
    public void TryValidateTurnEnded_RejectsParticipantWithoutActions()
    // Membuka scope metode TryValidateTurnEnded_RejectsParticipantWithoutActions; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidateTurnEnded_RejectsParticipantWithoutActions.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”AkhirGiliran”`, `”{}”`, `sessionId`, `playerId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("AkhirGiliran", "{}", sessionId, playerId);
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidateTurnEnded_RejectsParticipantWithoutActions.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”KerjaLepas”`, `”{}”`, `sessionId`, `playerId`, `1` dalam
            // TryValidateTurnEnded_RejectsParticipantWithoutActions.
            CreateEvent("KerjaLepas", "{}", sessionId, playerId, actionSlot: 1),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”Menabung”`, `”{}”`, `sessionId`, `playerId`, `2` dalam
            // TryValidateTurnEnded_RejectsParticipantWithoutActions.
            CreateEvent("Menabung", "{}", sessionId, playerId, actionSlot: 2)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidateTurnEnded_RejectsParticipantWithoutActions.
        };

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventTurnProgressValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `history`, `2`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventTurnProgressValidator().TryValidate(request, CreateConfig(), history, 2, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateTurnEnded_RejectsParticipantWithoutActions.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidateTurnEnded_RejectsParticipantWithoutActions.
        Assert.False(result.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Setiap pemain harus menyelesaikan seluruh
        // jatah aksi sebelum giliran berakhir”`, `result.Message`); pengujian gagal jika keduanya berbeda dalam
        // TryValidateTurnEnded_RejectsParticipantWithoutActions.
        Assert.Equal("Setiap pemain harus menyelesaikan seluruh jatah aksi sebelum giliran berakhir", result.Message);
    // Menutup scope metode TryValidateTurnEnded_RejectsParticipantWithoutActions; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidateTurnEnded_RejectsParticipantWithoutActions.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateTurnEnded_AcceptsTheSameActionInBothSlots` dengan hasil bertipe `void`; operasi ini menangani try validate
    // giliran ended accepts the same aksi in both slots.
    public void TryValidateTurnEnded_AcceptsTheSameActionInBothSlots()
    // Membuka scope metode TryValidateTurnEnded_AcceptsTheSameActionInBothSlots; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidateTurnEnded_AcceptsTheSameActionInBothSlots.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”AkhirGiliran”`, `”{}”`, `sessionId`, `playerId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("AkhirGiliran", "{}", sessionId, playerId);
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidateTurnEnded_AcceptsTheSameActionInBothSlots.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”KerjaLepas”`, `”{\”amount\”:1}”`, `sessionId`, `playerId`, `1` dalam
            // TryValidateTurnEnded_AcceptsTheSameActionInBothSlots.
            CreateEvent("KerjaLepas", "{\"amount\":1}", sessionId, playerId, actionSlot: 1),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”KerjaLepas”`, `”{\”amount\”:1}”`, `sessionId`, `playerId`, `2` dalam
            // TryValidateTurnEnded_AcceptsTheSameActionInBothSlots.
            CreateEvent("KerjaLepas", "{\"amount\":1}", sessionId, playerId, actionSlot: 2)
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidateTurnEnded_AcceptsTheSameActionInBothSlots.
        };

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventTurnProgressValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `history`, `1`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventTurnProgressValidator().TryValidate(request, CreateConfig(), history, 1, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateTurnEnded_AcceptsTheSameActionInBothSlots.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateTurnEnded_AcceptsTheSameActionInBothSlots.
        Assert.True(result.IsValid);
    // Menutup scope metode TryValidateTurnEnded_AcceptsTheSameActionInBothSlots; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidateTurnEnded_AcceptsTheSameActionInBothSlots.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateTurnEndedFriday_RejectsMissingPlayerDonation` dengan hasil bertipe `void`; operasi ini menangani try validate
    // giliran ended friday rejects missing pemain donasi.
    public void TryValidateTurnEndedFriday_RejectsMissingPlayerDonation()
    // Membuka scope metode TryValidateTurnEndedFriday_RejectsMissingPlayerDonation; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidateTurnEndedFriday_RejectsMissingPlayerDonation.
    {
        // Menyiapkan variabel lokal `firstPlayer` untuk nilai first pemain dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var firstPlayer = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”AkhirGiliran”`, `”{}”`, `sessionId`, `firstPlayer`, `”FRI”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("AkhirGiliran", "{}", sessionId, firstPlayer, "FRI");
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidateTurnEndedFriday_RejectsMissingPlayerDonation.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”JumatBerkah”`, `”””{”amount”:1}”””`, `sessionId`, `firstPlayer`, `0`, `”FRI”`
            // dalam TryValidateTurnEndedFriday_RejectsMissingPlayerDonation.
            CreateEvent("JumatBerkah", """{"amount":1}""", sessionId, firstPlayer, 0, "FRI")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidateTurnEndedFriday_RejectsMissingPlayerDonation.
        };

        // Menjalankan memanggil `new EventTurnProgressValidator().TryValidate` dengan `request`, `CreateConfig()`, `history`, `2`, `var result` dalam
        // TryValidateTurnEndedFriday_RejectsMissingPlayerDonation.
        new EventTurnProgressValidator().TryValidate(request, CreateConfig(), history, 2, out var result);

        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidateTurnEndedFriday_RejectsMissingPlayerDonation.
        Assert.False(result.IsValid);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”setiap pemain”`, `result.Message`,
        // `StringComparison.OrdinalIgnoreCase` dalam TryValidateTurnEndedFriday_RejectsMissingPlayerDonation.
        Assert.Contains("setiap pemain", result.Message, StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode TryValidateTurnEndedFriday_RejectsMissingPlayerDonation; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidateTurnEndedFriday_RejectsMissingPlayerDonation.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateTurnEndedSaturday_AcceptsOneDecisionPerPlayerAfterPriceOpened` dengan hasil bertipe `void`; operasi ini
    // menangani try validate giliran ended saturday accepts one decision per pemain after harga opened.
    public void TryValidateTurnEndedSaturday_AcceptsOneDecisionPerPlayerAfterPriceOpened()
    // Membuka scope metode TryValidateTurnEndedSaturday_AcceptsOneDecisionPerPlayerAfterPriceOpened; pernyataan/deklarasi berikut berada di dalam batas
    // blok ini dalam TryValidateTurnEndedSaturday_AcceptsOneDecisionPerPlayerAfterPriceOpened.
    {
        // Menyiapkan variabel lokal `firstPlayer` untuk nilai first pemain dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var firstPlayer = Guid.NewGuid();
        // Menyiapkan variabel lokal `secondPlayer` untuk nilai second pemain dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var secondPlayer = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”AkhirGiliran”`, `”{}”`, `sessionId`, `firstPlayer`, `”SAT”`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("AkhirGiliran", "{}", sessionId, firstPlayer, "SAT");
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidateTurnEndedSaturday_AcceptsOneDecisionPerPlayerAfterPriceOpened.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”BukaHargaEmas”`, `”””{”gold_price”:5}”””`, `sessionId`, `null`, `0`, `”SAT”`,
            // `”SYSTEM”` dalam TryValidateTurnEndedSaturday_AcceptsOneDecisionPerPlayerAfterPriceOpened.
            CreateEvent("BukaHargaEmas", """{"gold_price":5}""", sessionId, null, 0, "SAT", "SYSTEM"),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”InvestasiEmas”`, `”””{”trade_type”:”BUY”,”qty”:2,”unit_price”:5,”amount”:10}”””`,
            // `sessionId`, `firstPlayer`, `0`, `”SAT”` dalam TryValidateTurnEndedSaturday_AcceptsOneDecisionPerPlayerAfterPriceOpened.
            CreateEvent("InvestasiEmas", """{"trade_type":"BUY","qty":2,"unit_price":5,"amount":10}""", sessionId, firstPlayer, 0, "SAT"),
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `”LewatiTransaksiEmas”`, `”{}”`, `sessionId`, `secondPlayer`, `0`, `”SAT”` dalam
            // TryValidateTurnEndedSaturday_AcceptsOneDecisionPerPlayerAfterPriceOpened.
            CreateEvent("LewatiTransaksiEmas", "{}", sessionId, secondPlayer, 0, "SAT")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidateTurnEndedSaturday_AcceptsOneDecisionPerPlayerAfterPriceOpened.
        };

        // Menjalankan memanggil `new EventTurnProgressValidator().TryValidate` dengan `request`, `CreateConfig()`, `history`, `2`, `var result` dalam
        // TryValidateTurnEndedSaturday_AcceptsOneDecisionPerPlayerAfterPriceOpened.
        new EventTurnProgressValidator().TryValidate(request, CreateConfig(), history, 2, out var result);

        // Menjalankan pemeriksaan bahwa `result.IsValid` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateTurnEndedSaturday_AcceptsOneDecisionPerPlayerAfterPriceOpened.
        Assert.True(result.IsValid);
    // Menutup scope metode TryValidateTurnEndedSaturday_AcceptsOneDecisionPerPlayerAfterPriceOpened; bagian berikut berada di luar batas blok tersebut
    // dalam TryValidateTurnEndedSaturday_AcceptsOneDecisionPerPlayerAfterPriceOpened.
    }

    // Mendefinisikan metode `CreateRequest` dengan hasil bertipe `EventRequest`; operasi ini menangani create permintaan. Masukan: Parameter
    // `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter
    // `sessionId` bertipe `Guid?` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; nilai null diizinkan ketika data opsional
    // belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; Parameter `playerId` bertipe `Guid?` membawa nilai
    // pemain identitas; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada
    // nilai; Parameter `weekday` bertipe `string` membawa nilai weekday; bila argumen tidak diberikan digunakan nilai literal `”MON”`.
    private static EventRequest CreateRequest(
        // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
        string actionType,
        // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON.
        string payloadJson,
        // Parameter `sessionId` bertipe `Guid?` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; nilai null diizinkan ketika data
        // opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
        Guid? sessionId = null,
        // Parameter `playerId` bertipe `Guid?` membawa nilai pemain identitas; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak
        // diberikan digunakan null, yaitu penanda tidak ada nilai.
        Guid? playerId = null,
        // Parameter `weekday` bertipe `string` membawa nilai weekday; bila argumen tidak diberikan digunakan nilai literal `”MON”`.
        string weekday = "MON")
    // Membuka scope metode CreateRequest; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateRequest.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(payloadJson);
        // Mengembalikan objek baru bertipe `EventRequest` dengan argumen ( Guid.NewGuid(), sessionId ?? Guid.NewGuid(), playerId ?? Guid.NewGuid(),
        // ”PLAYER”, new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero), 0, weekday, 1, 0, a... kepada pemanggil dalam CreateRequest; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return new EventRequest(
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan `sessionId` bila tidak null; jika null gunakan `Guid.NewGuid()` sebagai nilai pengganti sebagai argumen ke konstruktor `EventRequest`.
            sessionId ?? Guid.NewGuid(),
            // Meneruskan `playerId` bila tidak null; jika null gunakan `Guid.NewGuid()` sebagai nilai pengganti sebagai argumen ke konstruktor `EventRequest`.
            playerId ?? Guid.NewGuid(),
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
            // Meneruskan `weekday` (nilai weekday) sebagai argumen ke konstruktor `EventRequest`.
            weekday,
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

    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `EventDb`; operasi ini menangani create event. Masukan: Parameter `actionType` bertipe
    // `string` membawa nilai aksi jenis; Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `playerId` bertipe `Guid?` membawa nilai pemain identitas;
    // nilai null diizinkan ketika data opsional belum tersedia; Parameter `actionSlot` bertipe `int` membawa nilai aksi slot; Parameter `weekday`
    // bertipe `string` membawa nilai weekday; bila argumen tidak diberikan digunakan nilai literal `”MON”`; Parameter `actorType` bertipe `string`
    // membawa nilai actor jenis; bila argumen tidak diberikan digunakan nilai literal `”PLAYER”`.
    private static EventDb CreateEvent(
        // Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
        string actionType,
        // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON.
        string payloadJson,
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `playerId` bertipe `Guid?` membawa nilai pemain identitas; nilai null diizinkan ketika data opsional belum tersedia.
        Guid? playerId,
        // Parameter `actionSlot` bertipe `int` membawa nilai aksi slot.
        int actionSlot,
        // Parameter `weekday` bertipe `string` membawa nilai weekday; bila argumen tidak diberikan digunakan nilai literal `”MON”`.
        string weekday = "MON",
        // Parameter `actorType` bertipe `string` membawa nilai actor jenis; bila argumen tidak diberikan digunakan nilai literal `”PLAYER”`.
        string actorType = "PLAYER")
    // Membuka scope metode CreateEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
    {
        // Mengembalikan objek baru bertipe `EventDb` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam CreateEvent; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new EventDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
        {
            // Memperbarui `EventId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
            EventId = Guid.NewGuid(),
            // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam CreateEvent.
            SessionId = sessionId,
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam CreateEvent.
            UserId = playerId,
            // Memperbarui `ActorType` menggunakan `actorType` (nilai actor jenis) dalam CreateEvent.
            ActorType = actorType,
            // Memperbarui `Timestamp` menggunakan objek baru bertipe `DateTimeOffset` dengan argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero) dalam CreateEvent.
            Timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero),
            // Memperbarui `DayIndex` menggunakan nilai literal `0` dalam CreateEvent.
            DayIndex = 0,
            // Memperbarui `Weekday` menggunakan `weekday` (nilai weekday) dalam CreateEvent.
            Weekday = weekday,
            // Memperbarui `ActionSlot` menggunakan `actionSlot` (nilai aksi slot) dalam CreateEvent.
            ActionSlot = actionSlot,
            // Memperbarui `SequenceNumber` menggunakan nilai literal `0` dalam CreateEvent.
            SequenceNumber = 0,
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

    // Mendefinisikan metode `CreateConfig` dengan hasil bertipe `RulesetConfig`; operasi ini menangani create konfigurasi. Masukan: Parameter `mode`
    // bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan; bila argumen tidak diberikan digunakan nilai literal
    // `”PEMULA”`; Parameter `actionsPerTurn` bertipe `int` membawa nilai aksi per giliran; bila argumen tidak diberikan digunakan nilai literal `2`.
    private static RulesetConfig CreateConfig(string mode = "PEMULA", int actionsPerTurn = 2)
    // Membuka scope metode CreateConfig; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateConfig.
    {
        // Mengembalikan objek baru bertipe `RulesetConfig` dengan argumen ( mode, actionsPerTurn, StartingCash: 20, PlayerOrdering.PlayerOrder, CashMin: 0,
        // MaxIngredientTotal: 10, MaxSameIngredient: 5, PrimaryNeedMaxPerDay: 1, Require... kepada pemanggil dalam CreateConfig; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return new RulesetConfig(
            // Meneruskan `mode` (mode permainan yang menentukan kelompok aturan yang digunakan) sebagai argumen ke konstruktor `RulesetConfig`.
            mode,
            // Meneruskan `actionsPerTurn` (nilai aksi per giliran) sebagai argumen ke konstruktor `RulesetConfig`.
            actionsPerTurn,
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
            // Meneruskan perbandingan kesamaan antara `mode` dan `”MAHIR”` sebagai argumen bernama `LoanEnabled`.
            LoanEnabled: mode == "MAHIR",
            // Meneruskan perbandingan kesamaan antara `mode` dan `”MAHIR”` sebagai argumen bernama `InsuranceEnabled`.
            InsuranceEnabled: mode == "MAHIR",
            // Meneruskan perbandingan kesamaan antara `mode` dan `”MAHIR”` sebagai argumen bernama `SavingGoalEnabled`.
            SavingGoalEnabled: mode == "MAHIR",
            // Meneruskan nilai literal `5` sebagai argumen bernama `FreelanceIncome`.
            FreelanceIncome: 5,
            // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `Scoring`.
            Scoring: null);
    // Menutup scope metode CreateConfig; bagian berikut berada di luar batas blok tersebut dalam CreateConfig.
    }
// Menutup scope tipe EventTurnProgressValidatorTests; bagian berikut berada di luar batas blok tersebut.
}
