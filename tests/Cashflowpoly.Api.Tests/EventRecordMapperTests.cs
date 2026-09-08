// Fungsi file: Memverifikasi perilaku API, database, atau domain melalui EventRecordMapperTests.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `EventRecordMapperTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventRecordMapperTests
// Membuka scope tipe EventRecordMapperTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `ToEventRequest_CopiesStoredFieldsAndClonesPayload` dengan hasil bertipe `void`; operasi ini menangani ke event permintaan
    // copies stored fields dan clones payload.
    public void ToEventRequest_CopiesStoredFieldsAndClonesPayload()
    // Membuka scope metode ToEventRequest_CopiesStoredFieldsAndClonesPayload; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ToEventRequest_CopiesStoredFieldsAndClonesPayload.
    {
        // Menyiapkan variabel lokal `eventId` untuk identitas unik event untuk pencatatan dan pemeriksaan duplikasi dengan memanggil `Guid.NewGuid` dengan
        // tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var eventId = Guid.NewGuid();
        // Menyiapkan variabel lokal `sessionId` untuk identitas unik sesi permainan yang menjadi batas data operasi ini dengan memanggil `Guid.NewGuid`
        // dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `rulesetVersionId` untuk identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat dengan
        // memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var rulesetVersionId = Guid.NewGuid();
        // Menyiapkan variabel lokal `timestamp` untuk waktu kejadian yang menjaga urutan kronologis data dengan objek baru bertipe `DateTimeOffset` dengan
        // argumen (2026, 1, 2, 3, 4, 5, TimeSpan.Zero). Tipe variabel disimpulkan dari ekspresi nilai awal.
        var timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero);
        // Menyiapkan variabel lokal `record` untuk nilai rekaman dengan objek baru bertipe `EventDb` dengan nilai awal sesuai konstruktornya. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var record = new EventDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        {
            // Memperbarui `EventId` menggunakan `eventId` (identitas unik event untuk pencatatan dan pemeriksaan duplikasi) dalam
            // ToEventRequest_CopiesStoredFieldsAndClonesPayload.
            EventId = eventId,
            // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam
            // ToEventRequest_CopiesStoredFieldsAndClonesPayload.
            SessionId = sessionId,
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
            UserId = playerId,
            // Memperbarui `ActorType` menggunakan nilai literal `”player”` dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
            ActorType = "player",
            // Memperbarui `Timestamp` menggunakan `timestamp` (waktu kejadian yang menjaga urutan kronologis data) dalam
            // ToEventRequest_CopiesStoredFieldsAndClonesPayload.
            Timestamp = timestamp,
            // Memperbarui `DayIndex` menggunakan nilai literal `7` dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
            DayIndex = 7,
            // Memperbarui `Weekday` menggunakan nilai literal `”Friday”` dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
            Weekday = "Friday",
            // Memperbarui `TurnNumber` menggunakan nilai literal `2` dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
            TurnNumber = 2,
            // Memperbarui `ActionSlot` menggunakan nilai literal `3` dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
            ActionSlot = 3,
            // Memperbarui `SequenceNumber` menggunakan nilai literal `12` dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
            SequenceNumber = 12,
            // Memperbarui `ActionType` menggunakan nilai literal `”CatatTransaksi”` dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
            ActionType = "CatatTransaksi",
            // Memperbarui `RulesetVersionId` menggunakan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat)
            // dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
            RulesetVersionId = rulesetVersionId,
            // Memperbarui `Payload` menggunakan nilai literal `”””{”amount”:5,”category”:”PAYCHECK”}”””` dalam
            // ToEventRequest_CopiesStoredFieldsAndClonesPayload.
            Payload = """{"amount":5,"category":"PAYCHECK"}""",
            // Memperbarui `ClientRequestId` menggunakan nilai literal `”client-123”` dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
            ClientRequestId = "client-123"
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        };

        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil `new
        // EventRecordMapper().ToEventRequest` dengan `record`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = new EventRecordMapper().ToEventRequest(record);

        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`eventId`, `request.EventId`); pengujian gagal
        // jika keduanya berbeda dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        Assert.Equal(eventId, request.EventId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`sessionId`, `request.SessionId`); pengujian
        // gagal jika keduanya berbeda dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        Assert.Equal(sessionId, request.SessionId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`playerId`, `request.UserId`); pengujian gagal
        // jika keduanya berbeda dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        Assert.Equal(playerId, request.UserId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”player”`, `request.ActorType`); pengujian
        // gagal jika keduanya berbeda dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        Assert.Equal("player", request.ActorType);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`timestamp`, `request.Timestamp`); pengujian
        // gagal jika keduanya berbeda dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        Assert.Equal(timestamp, request.Timestamp);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`7`, `request.DayIndex`); pengujian gagal jika
        // keduanya berbeda dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        Assert.Equal(7, request.DayIndex);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”Friday”`, `request.Weekday`); pengujian gagal
        // jika keduanya berbeda dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        Assert.Equal("Friday", request.Weekday);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`2`, `request.TurnNumber`); pengujian gagal
        // jika keduanya berbeda dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        Assert.Equal(2, request.TurnNumber);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`3`, `request.ActionSlot`); pengujian gagal
        // jika keduanya berbeda dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        Assert.Equal(3, request.ActionSlot);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`12`, `request.SequenceNumber`); pengujian
        // gagal jika keduanya berbeda dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        Assert.Equal(12, request.SequenceNumber);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”CatatTransaksi”`, `request.ActionType`);
        // pengujian gagal jika keduanya berbeda dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        Assert.Equal("CatatTransaksi", request.ActionType);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`rulesetVersionId`,
        // `request.RulesetVersionId`); pengujian gagal jika keduanya berbeda dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        Assert.Equal(rulesetVersionId, request.RulesetVersionId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”client-123”`, `request.ClientRequestId`);
        // pengujian gagal jika keduanya berbeda dalam ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        Assert.Equal("client-123", request.ClientRequestId);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`5`,
        // `request.Payload.GetProperty(”amount”).GetInt32()`); pengujian gagal jika keduanya berbeda dalam
        // ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        Assert.Equal(5, request.Payload.GetProperty("amount").GetInt32());
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”PAYCHECK”`,
        // `request.Payload.GetProperty(”category”).GetString()`); pengujian gagal jika keduanya berbeda dalam
        // ToEventRequest_CopiesStoredFieldsAndClonesPayload.
        Assert.Equal("PAYCHECK", request.Payload.GetProperty("category").GetString());
    // Menutup scope metode ToEventRequest_CopiesStoredFieldsAndClonesPayload; bagian berikut berada di luar batas blok tersebut dalam
    // ToEventRequest_CopiesStoredFieldsAndClonesPayload.
    }
// Menutup scope tipe EventRecordMapperTests; bagian berikut berada di luar batas blok tersebut.
}
