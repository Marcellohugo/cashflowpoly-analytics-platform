// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui NeedTierClassifier.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan enum untuk membatasi pilihan nilai bernama `NeedTier`.
internal enum NeedTier
// Membuka scope tipe NeedTier; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan pilihan enum `Unknown`; nilai bilangan mengikuti urutan deklarasi enum.
    Unknown,
    // Mendefinisikan pilihan enum `Primary`; nilai bilangan mengikuti urutan deklarasi enum.
    Primary,
    // Mendefinisikan pilihan enum `Secondary`; nilai bilangan mengikuti urutan deklarasi enum.
    Secondary,
    // Mendefinisikan pilihan enum `Tertiary`; nilai bilangan mengikuti urutan deklarasi enum.
    Tertiary
// Menutup scope tipe NeedTier; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `NeedTierClassifier`.
internal static class NeedTierClassifier
// Membuka scope tipe NeedTierClassifier; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `FromPayload` dengan hasil bertipe `NeedTier`; operasi ini menangani dari payload. Masukan: Parameter `payload` bertipe
    // `JsonElement` membawa muatan detail event dalam format JSON; Parameter `cardId` bertipe `string?` membawa nilai kartu identitas; nilai null
    // diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai.
    public static NeedTier FromPayload(JsonElement payload, string? cardId = null)
    // Membuka scope metode FromPayload; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromPayload.
    {
        // Mengulangi setiap elemen `new[] { ”need_tier”, ”tier”, ”need_type” }`; elemen saat ini disimpan sebagai `propertyName` bertipe `var` untuk
        // diproses oleh badan loop dalam FromPayload.
        foreach (var propertyName in new[] { "need_tier", "tier", "need_type" })
        // Membuka scope loop setiap propertyName dari `new[] { ”need_tier”, ”tier”, ”need_type” }`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam FromPayload.
        {
            // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `payload.TryGetProperty(propertyName, out var property)` dan `property.ValueKind
            // == JsonValueKind.String`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // FromPayload.
            if (payload.TryGetProperty(propertyName, out var property) &&
                // Melanjutkan ekspresi dengan perbandingan kesamaan antara `property.ValueKind` dan `JsonValueKind.String` dalam FromPayload.
                property.ValueKind == JsonValueKind.String)
            // Membuka scope cabang if untuk kondisi `payload.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromPayload.
            {
                // Menyiapkan variabel lokal `fromTier` untuk nilai dari tingkat dengan memanggil `FromText` dengan `property.GetString()`. Tipe variabel
                // disimpulkan dari ekspresi nilai awal.
                var fromTier = FromText(property.GetString());
                // Memeriksa perbandingan ketidaksamaan antara `fromTier` dan `NeedTier.Unknown`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
                // FromPayload.
                if (fromTier != NeedTier.Unknown)
                // Membuka scope cabang if untuk kondisi `fromTier != NeedTier.Unknown`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // FromPayload.
                {
                    // Mengembalikan `fromTier` (nilai dari tingkat) kepada pemanggil dalam FromPayload; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                    return fromTier;
                // Menutup scope cabang if untuk kondisi `fromTier != NeedTier.Unknown`; bagian berikut berada di luar batas blok tersebut dalam FromPayload.
                }
            // Menutup scope cabang if untuk kondisi `payload.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String`;
            // bagian berikut berada di luar batas blok tersebut dalam FromPayload.
            }
        // Menutup scope loop setiap propertyName dari `new[] { ”need_tier”, ”tier”, ”need_type” }`; bagian berikut berada di luar batas blok tersebut dalam
        // FromPayload.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `string.IsNullOrWhiteSpace(cardId) && payload.TryGetProperty(”card_id”, out var
        // cardProperty)` dan `cardProperty.ValueKind == JsonValueKind.String`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan
        // ketika kondisi ini bernilai benar dalam FromPayload.
        if (string.IsNullOrWhiteSpace(cardId) &&
            // Melanjutkan pengolahan dengan mencari properti JSON `”card_id”`, `var cardProperty` pada `payload` tanpa menganggap propertinya selalu tersedia
            // dalam FromPayload.
            payload.TryGetProperty("card_id", out var cardProperty) &&
            // Melanjutkan ekspresi dengan perbandingan kesamaan antara `cardProperty.ValueKind` dan `JsonValueKind.String` dalam FromPayload.
            cardProperty.ValueKind == JsonValueKind.String)
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(cardId) && payload.TryGetProperty(”card_id”, out var cardProperty) &&
        // cardProperty.ValueKind == JsonValueKind.String`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromPayload.
        {
            // Memperbarui `cardId` menggunakan membaca nilai string dari `cardProperty` sesuai tipe JSON atau sumber data yang digunakan dalam FromPayload.
            cardId = cardProperty.GetString();
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(cardId) && payload.TryGetProperty(”card_id”, out var cardProperty) &&
        // cardProperty.ValueKind == JsonValueKind.String`; bagian berikut berada di luar batas blok tersebut dalam FromPayload.
        }

        // Mengembalikan memanggil `FromCardId` dengan `cardId` kepada pemanggil dalam FromPayload; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return FromCardId(cardId);
    // Menutup scope metode FromPayload; bagian berikut berada di luar batas blok tersebut dalam FromPayload.
    }

    // Mendefinisikan metode `FromPayloadJson` dengan hasil bertipe `NeedTier`; operasi ini menangani dari payload JSON. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON.
    public static NeedTier FromPayloadJson(string payloadJson)
    // Membuka scope metode FromPayloadJson; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromPayloadJson.
    {
        // Memeriksa memeriksa apakah `payloadJson` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam FromPayloadJson.
        if (string.IsNullOrWhiteSpace(payloadJson))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(payloadJson)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // FromPayloadJson.
        {
            // Mengembalikan `NeedTier.Unknown` (nilai unknown) kepada pemanggil dalam FromPayloadJson; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return NeedTier.Unknown;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(payloadJson)`; bagian berikut berada di luar batas blok tersebut dalam
        // FromPayloadJson.
        }

        // Memulai blok try dalam FromPayloadJson; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromPayloadJson.
        {
            // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan
            // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var document = JsonDocument.Parse(payloadJson);
            // Mengembalikan memanggil `FromPayload` dengan `document.RootElement` kepada pemanggil dalam FromPayloadJson; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return FromPayload(document.RootElement);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam FromPayloadJson.
        }
        // Menangani exception `JsonException` melalui variabel dalam FromPayloadJson.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromPayloadJson.
        {
            // Mengembalikan `NeedTier.Unknown` (nilai unknown) kepada pemanggil dalam FromPayloadJson; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return NeedTier.Unknown;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam FromPayloadJson.
        }
    // Menutup scope metode FromPayloadJson; bagian berikut berada di luar batas blok tersebut dalam FromPayloadJson.
    }

    // Mendefinisikan metode `FromText` dengan hasil bertipe `NeedTier`; operasi ini menangani dari text. Masukan: Parameter `value` bertipe `string?`
    // membawa nilai nilai; nilai null diizinkan ketika data opsional belum tersedia.
    private static NeedTier FromText(string? value)
    // Membuka scope metode FromText; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromText.
    {
        // Mengembalikan hasil pemetaan `(value ?? string.Empty).Trim().ToLowerInvariant()` melalui cabang pola switch yang cocok kepada pemanggil dalam
        // FromText; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return (value ?? string.Empty).Trim().ToLowerInvariant() switch
        // Membuka scope pemetaan switch atas `(value ?? string.Empty).Trim().ToLowerInvariant()`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam FromText.
        {
            // Untuk pola `”primary” or ”primer”`, menghasilkan `NeedTier.Primary` (nilai primary) sebagai hasil switch.
            "primary" or "primer" => NeedTier.Primary,
            // Untuk pola `”secondary” or ”sekunder”`, menghasilkan `NeedTier.Secondary` (nilai secondary) sebagai hasil switch.
            "secondary" or "sekunder" => NeedTier.Secondary,
            // Untuk pola `”tertiary” or ”tersier”`, menghasilkan `NeedTier.Tertiary` (nilai tertiary) sebagai hasil switch.
            "tertiary" or "tersier" => NeedTier.Tertiary,
            // Untuk pola `_`, menghasilkan `NeedTier.Unknown` (nilai unknown) sebagai hasil switch.
            _ => NeedTier.Unknown
        // Menutup scope pemetaan switch atas `(value ?? string.Empty).Trim().ToLowerInvariant()`; bagian berikut berada di luar batas blok tersebut dalam
        // FromText.
        };
    // Menutup scope metode FromText; bagian berikut berada di luar batas blok tersebut dalam FromText.
    }

    // Mendefinisikan metode `FromCardId` dengan hasil bertipe `NeedTier`; operasi ini menangani dari kartu identitas. Masukan: Parameter `cardId`
    // bertipe `string?` membawa nilai kartu identitas; nilai null diizinkan ketika data opsional belum tersedia.
    private static NeedTier FromCardId(string? cardId)
    // Membuka scope metode FromCardId; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromCardId.
    {
        // Memperbarui `cardId` menggunakan memanggil `System.Text.RegularExpressions.Regex.Replace` dengan `cardId ?? string.Empty`, `”_[0-9]+$”`, `””`
        // dalam FromCardId.
        cardId = System.Text.RegularExpressions.Regex.Replace(cardId ?? string.Empty, "_[0-9]+$", "");
        // Menyiapkan variabel lokal `value` untuk nilai nilai dengan menormalisasi `cardId.Trim()` menjadi huruf kecil dengan aturan kultur invariant. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var value = cardId.Trim().ToLowerInvariant();
        // Memeriksa memeriksa apakah `value` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam FromCardId.
        if (string.IsNullOrWhiteSpace(value))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(value)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // FromCardId.
        {
            // Mengembalikan `NeedTier.Unknown` (nilai unknown) kepada pemanggil dalam FromCardId; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return NeedTier.Unknown;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(value)`; bagian berikut berada di luar batas blok tersebut dalam FromCardId.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `value.StartsWith(”primary”, StringComparison.Ordinal) ||
        // value.StartsWith(”rice”, StringComparison.Ordinal) || value.StartsWith(”water”, StringComparison.Ordinal) || value.Con...` dan `value == ”buku”`;
        // sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam FromCardId.
        if (value.StartsWith("primary", StringComparison.Ordinal) ||
            // Melanjutkan pengolahan dengan memanggil `value.StartsWith` dengan `”rice”`, `StringComparison.Ordinal` dalam FromCardId.
            value.StartsWith("rice", StringComparison.Ordinal) ||
            // Melanjutkan pengolahan dengan memanggil `value.StartsWith` dengan `”water”`, `StringComparison.Ordinal` dalam FromCardId.
            value.StartsWith("water", StringComparison.Ordinal) ||
            // Melanjutkan pengolahan dengan memeriksa apakah `value` memuat `”food”`, `StringComparison.Ordinal` dalam FromCardId.
            value.Contains("food", StringComparison.Ordinal) ||
            // Melanjutkan ekspresi dengan perbandingan kesamaan antara `value` dan `”buku”` dalam FromCardId.
            value == "buku")
        // Membuka scope cabang if untuk kondisi `value.StartsWith(”primary”, StringComparison.Ordinal) || value.StartsWith(”rice”,
        // StringComparison.Ordinal) || value.StartsWith(”water”, StringComparison.Ordinal) || value.Con...`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam FromCardId.
        {
            // Mengembalikan `NeedTier.Primary` (nilai primary) kepada pemanggil dalam FromCardId; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return NeedTier.Primary;
        // Menutup scope cabang if untuk kondisi `value.StartsWith(”primary”, StringComparison.Ordinal) || value.StartsWith(”rice”,
        // StringComparison.Ordinal) || value.StartsWith(”water”, StringComparison.Ordinal) || value.Con...`; bagian berikut berada di luar batas blok
        // tersebut dalam FromCardId.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `value.StartsWith(”secondary”, StringComparison.Ordinal) ||
        // value.Contains(”school”, StringComparison.Ordinal) || value == ”book”` dan `value == ”sepatu”`; sisi kanan diperiksa hanya jika sisi kiri salah;
        // blok if hanya dijalankan ketika kondisi ini bernilai benar dalam FromCardId.
        if (value.StartsWith("secondary", StringComparison.Ordinal) ||
            // Melanjutkan pengolahan dengan memeriksa apakah `value` memuat `”school”`, `StringComparison.Ordinal` dalam FromCardId.
            value.Contains("school", StringComparison.Ordinal) ||
            // Melanjutkan ekspresi dengan perbandingan kesamaan antara `value` dan `”book”` dalam FromCardId.
            value == "book" ||
            // Melanjutkan ekspresi dengan perbandingan kesamaan antara `value` dan `”sepatu”` dalam FromCardId.
            value == "sepatu")
        // Membuka scope cabang if untuk kondisi `value.StartsWith(”secondary”, StringComparison.Ordinal) || value.Contains(”school”,
        // StringComparison.Ordinal) || value == ”book” || value == ”sepatu”`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromCardId.
        {
            // Mengembalikan `NeedTier.Secondary` (nilai secondary) kepada pemanggil dalam FromCardId; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return NeedTier.Secondary;
        // Menutup scope cabang if untuk kondisi `value.StartsWith(”secondary”, StringComparison.Ordinal) || value.Contains(”school”,
        // StringComparison.Ordinal) || value == ”book” || value == ”sepatu”`; bagian berikut berada di luar batas blok tersebut dalam FromCardId.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `value.StartsWith(”tertiary”, StringComparison.Ordinal) ||
        // value.Contains(”bike”, StringComparison.Ordinal)` dan `value is ”boneka” or ”gameboy” or ”hiburan”`; sisi kanan diperiksa hanya jika sisi kiri
        // salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam FromCardId.
        if (value.StartsWith("tertiary", StringComparison.Ordinal) ||
            // Melanjutkan pengolahan dengan memeriksa apakah `value` memuat `”bike”`, `StringComparison.Ordinal` dalam FromCardId.
            value.Contains("bike", StringComparison.Ordinal) ||
            // Menggunakan `value` (nilai nilai) sebagai bagian ekspresi yang sedang disusun dalam FromCardId.
            value is "boneka" or "gameboy" or "hiburan")
        // Membuka scope cabang if untuk kondisi `value.StartsWith(”tertiary”, StringComparison.Ordinal) || value.Contains(”bike”, StringComparison.Ordinal)
        // || value is ”boneka” or ”gameboy” or ”hiburan”`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam FromCardId.
        {
            // Mengembalikan `NeedTier.Tertiary` (nilai tertiary) kepada pemanggil dalam FromCardId; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return NeedTier.Tertiary;
        // Menutup scope cabang if untuk kondisi `value.StartsWith(”tertiary”, StringComparison.Ordinal) || value.Contains(”bike”, StringComparison.Ordinal)
        // || value is ”boneka” or ”gameboy” or ”hiburan”`; bagian berikut berada di luar batas blok tersebut dalam FromCardId.
        }

        // Mengembalikan `NeedTier.Tertiary` (nilai tertiary) kepada pemanggil dalam FromCardId; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return NeedTier.Tertiary;
    // Menutup scope metode FromCardId; bagian berikut berada di luar batas blok tersebut dalam FromCardId.
    }
// Menutup scope tipe NeedTierClassifier; bagian berikut berada di luar batas blok tersebut.
}
