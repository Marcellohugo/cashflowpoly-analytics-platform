// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventRecordMapper.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan tipe class `EventRecordMapper` yang mewarisi atau menerapkan `IEventRecordMapper`; sealed mencegah tipe ini diturunkan lagi.
internal sealed class EventRecordMapper : IEventRecordMapper
// Membuka scope tipe EventRecordMapper; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `ToEventRequest` dengan hasil bertipe `EventRequest`; operasi ini menangani ke event permintaan. Masukan: Parameter
    // `record` bertipe `EventDb` membawa nilai rekaman.
    public EventRequest ToEventRequest(EventDb record)
    // Membuka scope metode ToEventRequest; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ToEventRequest.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `record.Payload`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(record.Payload);
        // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan membuat salinan `document.RootElement` agar hasil dapat
        // digunakan terpisah dari objek sumber. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var payload = document.RootElement.Clone();

        // Mengembalikan objek baru bertipe `EventRequest` dengan argumen ( record.EventId, record.SessionId, record.UserId, record.ActorType,
        // record.Timestamp, record.DayIndex, record.Weekday, record.ActionSlot, record.SequenceNumber... kepada pemanggil dalam ToEventRequest; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return new EventRequest(
            // Meneruskan `record.EventId` (identitas unik event untuk pencatatan dan pemeriksaan duplikasi) sebagai argumen ke konstruktor `EventRequest`.
            record.EventId,
            // Meneruskan `record.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke konstruktor `EventRequest`.
            record.SessionId,
            // Meneruskan `record.UserId` (identitas akun pengguna yang datanya sedang diproses) sebagai argumen ke konstruktor `EventRequest`.
            record.UserId,
            // Meneruskan `record.ActorType` (nilai actor jenis) sebagai argumen ke konstruktor `EventRequest`.
            record.ActorType,
            // Meneruskan `record.Timestamp` (waktu kejadian yang menjaga urutan kronologis data) sebagai argumen ke konstruktor `EventRequest`.
            record.Timestamp,
            // Meneruskan `record.DayIndex` (nilai hari index) sebagai argumen ke konstruktor `EventRequest`.
            record.DayIndex,
            // Meneruskan `record.Weekday` (nilai weekday) sebagai argumen ke konstruktor `EventRequest`.
            record.Weekday,
            // Meneruskan `record.ActionSlot` (nilai aksi slot) sebagai argumen ke konstruktor `EventRequest`.
            record.ActionSlot,
            // Meneruskan `record.SequenceNumber` (nomor urut event yang menentukan urutan pemrosesan riwayat permainan) sebagai argumen ke konstruktor
            // `EventRequest`.
            record.SequenceNumber,
            // Meneruskan `record.ActionType` (nilai aksi jenis) sebagai argumen ke konstruktor `EventRequest`.
            record.ActionType,
            // Meneruskan `record.RulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
            // konstruktor `EventRequest`.
            record.RulesetVersionId,
            // Meneruskan `payload` (muatan detail event dalam format JSON) sebagai argumen ke konstruktor `EventRequest`.
            payload,
            // Meneruskan `record.ClientRequestId` (identitas permintaan dari klien untuk pelacakan atau penanganan permintaan berulang) sebagai argumen ke
            // konstruktor `EventRequest`.
            record.ClientRequestId,
            // Meneruskan `record.TurnNumber` (nilai giliran number) sebagai argumen ke konstruktor `EventRequest`.
            record.TurnNumber);
    // Menutup scope metode ToEventRequest; bagian berikut berada di luar batas blok tersebut dalam ToEventRequest.
    }
// Menutup scope tipe EventRecordMapper; bagian berikut berada di luar batas blok tersebut.
}
