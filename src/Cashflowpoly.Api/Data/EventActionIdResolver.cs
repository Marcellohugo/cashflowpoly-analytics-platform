// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk EventActionIdResolver.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Data` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Data;

// Mendefinisikan tipe class `EventActionIdResolver`.
public static class EventActionIdResolver
// Membuka scope tipe EventActionIdResolver; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Resolve` dengan hasil bertipe `string?`; operasi ini menangani resolve. Masukan: Parameter `actionType` bertipe `string`
    // membawa nilai aksi jenis; Parameter `payloadJson` bertipe `string` membawa nilai payload JSON.
    public static string? Resolve(string actionType, string payloadJson)
    // Membuka scope metode Resolve; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Resolve.
    {
        // Memeriksa memeriksa apakah `payloadJson` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam Resolve.
        if (string.IsNullOrWhiteSpace(payloadJson))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(payloadJson)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // Resolve.
        {
            // Menyiapkan variabel lokal `emptyDocument` untuk nilai empty document dengan memanggil `JsonDocument.Parse` dengan `”{}”`. Tipe variabel
            // disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var emptyDocument = JsonDocument.Parse("{}");
            // Mengembalikan memanggil `Resolve` dengan `actionType`, `emptyDocument.RootElement` kepada pemanggil dalam Resolve; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return Resolve(actionType, emptyDocument.RootElement);
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(payloadJson)`; bagian berikut berada di luar batas blok tersebut dalam Resolve.
        }

        // Memulai blok try dalam Resolve; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Resolve.
        {
            // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan
            // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var document = JsonDocument.Parse(payloadJson);
            // Mengembalikan memanggil `Resolve` dengan `actionType`, `document.RootElement` kepada pemanggil dalam Resolve; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return Resolve(actionType, document.RootElement);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam Resolve.
        }
        // Menangani exception `JsonException` melalui variabel dalam Resolve.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Resolve.
        {
            // Mengembalikan memanggil `Resolve` dengan `actionType`, `default(JsonElement)` kepada pemanggil dalam Resolve; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return Resolve(actionType, default(JsonElement));
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam Resolve.
        }
    // Menutup scope metode Resolve; bagian berikut berada di luar batas blok tersebut dalam Resolve.
    }

    // Mendefinisikan metode `Resolve` dengan hasil bertipe `string?`; operasi ini menangani resolve. Masukan: Parameter `actionType` bertipe `string`
    // membawa nilai aksi jenis; Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON.
    public static string? Resolve(string actionType, JsonElement payload)
    // Membuka scope metode Resolve; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Resolve.
    {
        // Mengembalikan memanggil `GameActionCatalog.ResolveGameActionId` dengan `actionType`, `payload` kepada pemanggil dalam Resolve; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return GameActionCatalog.ResolveGameActionId(actionType, payload);
    // Menutup scope metode Resolve; bagian berikut berada di luar batas blok tersebut dalam Resolve.
    }
// Menutup scope tipe EventActionIdResolver; bagian berikut berada di luar batas blok tersebut.
}
