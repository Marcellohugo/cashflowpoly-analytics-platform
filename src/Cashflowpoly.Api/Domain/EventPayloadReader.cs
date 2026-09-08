// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventPayloadReader.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Helper murni untuk membaca payload event gameplay.
/// </summary>
// Mendefinisikan tipe class `EventPayloadReader` yang mewarisi atau menerapkan `IEventPayloadReader`; sealed mencegah tipe ini diturunkan lagi.
internal sealed class EventPayloadReader : IEventPayloadReader
// Membuka scope tipe EventPayloadReader; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Mem-parse string JSON payload event menjadi JsonElement.
    /// </summary>
    // Mendefinisikan metode `ReadPayload` dengan hasil bertipe `JsonElement`. Mem-parse string JSON payload event menjadi JsonElement. Masukan:
    // Parameter `payload` bertipe `string` membawa muatan detail event dalam format JSON.
    public JsonElement ReadPayload(string payload)
    // Membuka scope metode ReadPayload; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadPayload.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `payload`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(payload);
        // Mengembalikan membuat salinan `document.RootElement` agar hasil dapat digunakan terpisah dari objek sumber kepada pemanggil dalam ReadPayload;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return document.RootElement.Clone();
    // Menutup scope metode ReadPayload; bagian berikut berada di luar batas blok tersebut dalam ReadPayload.
    }

    /// <summary>
    /// Mengekstrak nilai string dari properti JSON payload.
    /// </summary>
    // Mendefinisikan metode `TryGetString` dengan hasil bertipe `bool`. Mengekstrak nilai string dari properti JSON payload. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property
    // nama; Parameter `value` bertipe `string` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryGetString(JsonElement payload, string propertyName, out string value)
    // Membuka scope metode TryGetString; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetString.
    {
        // Memperbarui `value` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryGetString.
        value = string.Empty;
        // Memeriksa kebalikan kondisi `payload.TryGetProperty(propertyName, out var property)`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam TryGetString.
        if (!payload.TryGetProperty(propertyName, out var property))
        // Membuka scope cabang if untuk kondisi `!payload.TryGetProperty(propertyName, out var property)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam TryGetString.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryGetString; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!payload.TryGetProperty(propertyName, out var property)`; bagian berikut berada di luar batas blok
        // tersebut dalam TryGetString.
        }

        // Memeriksa hasil pencocokan `property.ValueKind` dengan pola `JsonValueKind.String or JsonValueKind.Null`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam TryGetString.
        if (property.ValueKind is JsonValueKind.String or JsonValueKind.Null)
        // Membuka scope cabang if untuk kondisi `property.ValueKind is JsonValueKind.String or JsonValueKind.Null`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam TryGetString.
        {
            // Memperbarui `value` menggunakan `property.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
            // TryGetString.
            value = property.GetString() ?? string.Empty;
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryGetString; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `property.ValueKind is JsonValueKind.String or JsonValueKind.Null`; bagian berikut berada di luar batas
        // blok tersebut dalam TryGetString.
        }

        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryGetString; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return false;
    // Menutup scope metode TryGetString; bagian berikut berada di luar batas blok tersebut dalam TryGetString.
    }

    /// <summary>
    /// Mengekstrak nilai string opsional dari payload; mengembalikan true jika properti tidak ada.
    /// </summary>
    // Mendefinisikan metode `TryGetOptionalString` dengan hasil bertipe `bool`. Mengekstrak nilai string opsional dari payload; mengembalikan true jika
    // properti tidak ada. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName`
    // bertipe `string` membawa nilai property nama; Parameter `value` bertipe `string?` membawa nilai nilai; nilai null diizinkan ketika data opsional
    // belum tersedia; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryGetOptionalString(JsonElement payload, string propertyName, out string? value)
    // Membuka scope metode TryGetOptionalString; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetOptionalString.
    {
        // Memperbarui `value` menggunakan null, yaitu penanda tidak ada nilai dalam TryGetOptionalString.
        value = null;
        // Memeriksa kebalikan kondisi `payload.TryGetProperty(propertyName, out var property)`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam TryGetOptionalString.
        if (!payload.TryGetProperty(propertyName, out var property))
        // Membuka scope cabang if untuk kondisi `!payload.TryGetProperty(propertyName, out var property)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam TryGetOptionalString.
        {
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryGetOptionalString; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `!payload.TryGetProperty(propertyName, out var property)`; bagian berikut berada di luar batas blok
        // tersebut dalam TryGetOptionalString.
        }

        // Memeriksa hasil pencocokan `property.ValueKind` dengan pola `JsonValueKind.String or JsonValueKind.Null`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam TryGetOptionalString.
        if (property.ValueKind is JsonValueKind.String or JsonValueKind.Null)
        // Membuka scope cabang if untuk kondisi `property.ValueKind is JsonValueKind.String or JsonValueKind.Null`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam TryGetOptionalString.
        {
            // Memperbarui `value` menggunakan membaca nilai string dari `property` sesuai tipe JSON atau sumber data yang digunakan dalam TryGetOptionalString.
            value = property.GetString();
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryGetOptionalString; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `property.ValueKind is JsonValueKind.String or JsonValueKind.Null`; bagian berikut berada di luar batas
        // blok tersebut dalam TryGetOptionalString.
        }

        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryGetOptionalString; eksekusi jalur ini selesai setelah nilai
        // hasil ditentukan.
        return false;
    // Menutup scope metode TryGetOptionalString; bagian berikut berada di luar batas blok tersebut dalam TryGetOptionalString.
    }

    /// <summary>
    /// Mengekstrak nilai integer 32-bit dari properti JSON payload.
    /// </summary>
    // Mendefinisikan metode `TryGetInt32` dengan hasil bertipe `bool`. Mengekstrak nilai integer 32-bit dari properti JSON payload. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property
    // nama; Parameter `value` bertipe `int` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `required` bertipe `bool` membawa nilai required; bila argumen tidak diberikan digunakan true, yaitu kondisi aktif/terpenuhi.
    public bool TryGetInt32(JsonElement payload, string propertyName, out int value, bool required = true)
    // Membuka scope metode TryGetInt32; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetInt32.
    {
        // Memperbarui `value` menggunakan nilai literal `0` dalam TryGetInt32.
        value = 0;
        // Memeriksa kebalikan kondisi `payload.TryGetProperty(propertyName, out var property)`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam TryGetInt32.
        if (!payload.TryGetProperty(propertyName, out var property))
        // Membuka scope cabang if untuk kondisi `!payload.TryGetProperty(propertyName, out var property)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam TryGetInt32.
        {
            // Mengembalikan kebalikan kondisi `required` kepada pemanggil dalam TryGetInt32; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return !required;
        // Menutup scope cabang if untuk kondisi `!payload.TryGetProperty(propertyName, out var property)`; bagian berikut berada di luar batas blok
        // tersebut dalam TryGetInt32.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `property.ValueKind != JsonValueKind.Number` dan
        // `!property.TryGetInt32(out value)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam TryGetInt32.
        if (property.ValueKind != JsonValueKind.Number || !property.TryGetInt32(out value))
        // Membuka scope cabang if untuk kondisi `property.ValueKind != JsonValueKind.Number || !property.TryGetInt32(out value)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam TryGetInt32.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryGetInt32; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `property.ValueKind != JsonValueKind.Number || !property.TryGetInt32(out value)`; bagian berikut berada di
        // luar batas blok tersebut dalam TryGetInt32.
        }

        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryGetInt32; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return true;
    // Menutup scope metode TryGetInt32; bagian berikut berada di luar batas blok tersebut dalam TryGetInt32.
    }

    /// <summary>
    /// Mengekstrak nilai double dari properti JSON payload.
    /// </summary>
    // Mendefinisikan metode `TryGetDouble` dengan hasil bertipe `bool`. Mengekstrak nilai double dari properti JSON payload. Masukan: Parameter
    // `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property
    // nama; Parameter `value` bertipe `double` membawa nilai nilai; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `required` bertipe `bool` membawa nilai required; bila argumen tidak diberikan digunakan true, yaitu kondisi aktif/terpenuhi.
    public bool TryGetDouble(JsonElement payload, string propertyName, out double value, bool required = true)
    // Membuka scope metode TryGetDouble; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryGetDouble.
    {
        // Memperbarui `value` menggunakan nilai literal `0` dalam TryGetDouble.
        value = 0;
        // Memeriksa kebalikan kondisi `payload.TryGetProperty(propertyName, out var property)`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam TryGetDouble.
        if (!payload.TryGetProperty(propertyName, out var property))
        // Membuka scope cabang if untuk kondisi `!payload.TryGetProperty(propertyName, out var property)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam TryGetDouble.
        {
            // Mengembalikan kebalikan kondisi `required` kepada pemanggil dalam TryGetDouble; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return !required;
        // Menutup scope cabang if untuk kondisi `!payload.TryGetProperty(propertyName, out var property)`; bagian berikut berada di luar batas blok
        // tersebut dalam TryGetDouble.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `property.ValueKind != JsonValueKind.Number` dan
        // `!property.TryGetDouble(out value)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam TryGetDouble.
        if (property.ValueKind != JsonValueKind.Number || !property.TryGetDouble(out value))
        // Membuka scope cabang if untuk kondisi `property.ValueKind != JsonValueKind.Number || !property.TryGetDouble(out value)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam TryGetDouble.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryGetDouble; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `property.ValueKind != JsonValueKind.Number || !property.TryGetDouble(out value)`; bagian berikut berada di
        // luar batas blok tersebut dalam TryGetDouble.
        }

        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryGetDouble; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return true;
    // Menutup scope metode TryGetDouble; bagian berikut berada di luar batas blok tersebut dalam TryGetDouble.
    }

    /// <summary>
    /// Membaca direction, amount, category, dan counterparty dari payload event CatatTransaksi.
    /// </summary>
    // Mendefinisikan metode `TryReadTransaction` dengan hasil bertipe `bool`. Membaca direction, amount, category, dan counterparty dari payload event
    // CatatTransaksi. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `direction` bertipe
    // `string` membawa nilai direction; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `double`
    // membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode;
    // Parameter `category` bertipe `string` membawa nilai category; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `counterparty` bertipe `string?` membawa nilai counterparty; nilai null diizinkan ketika data opsional belum tersedia; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode.
    public bool TryReadTransaction(JsonElement payload, out string direction, out double amount, out string category, out string? counterparty)
    // Membuka scope metode TryReadTransaction; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadTransaction.
    {
        // Memperbarui `direction` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadTransaction.
        direction = string.Empty;
        // Memperbarui `category` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadTransaction.
        category = string.Empty;
        // Memperbarui `counterparty` menggunakan null, yaitu penanda tidak ada nilai dalam TryReadTransaction.
        counterparty = null;
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadTransaction.
        amount = 0;

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(payload, ”direction”, out direction) ||
        // !TryGetDouble(payload, ”amount”, out amount)` dan `!TryGetString(payload, ”category”, out category)`; sisi kanan diperiksa hanya jika sisi kiri
        // salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadTransaction.
        if (!TryGetString(payload, "direction", out direction) ||
            // Menggunakan kebalikan kondisi `TryGetDouble(payload, ”amount”, out amount)` sebagai bagian ekspresi yang sedang disusun dalam TryReadTransaction.
            !TryGetDouble(payload, "amount", out amount) ||
            // Menggunakan kebalikan kondisi `TryGetString(payload, ”category”, out category)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryReadTransaction.
            !TryGetString(payload, "category", out category))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”direction”, out direction) || !TryGetDouble(payload, ”amount”, out amount) ||
        // !TryGetString(payload, ”category”, out category)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadTransaction.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadTransaction; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”direction”, out direction) || !TryGetDouble(payload, ”amount”, out amount) ||
        // !TryGetString(payload, ”category”, out category)`; bagian berikut berada di luar batas blok tersebut dalam TryReadTransaction.
        }

        // Mengembalikan memanggil `TryGetOptionalString` dengan `payload`, `”counterparty”`, `counterparty` kepada pemanggil dalam TryReadTransaction;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return TryGetOptionalString(payload, "counterparty", out counterparty);
    // Menutup scope metode TryReadTransaction; bagian berikut berada di luar batas blok tersebut dalam TryReadTransaction.
    }

    /// <summary>
    /// Membaca nilai amount dari payload JSON event.
    /// </summary>
    // Mendefinisikan metode `TryReadAmount` dengan hasil bertipe `bool`. Membaca nilai amount dari payload JSON event. Masukan: Parameter `payload`
    // bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `amount` bertipe `double` membawa nominal uang atau nilai
    // transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadAmount(JsonElement payload, out double amount)
    // Membuka scope metode TryReadAmount; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadAmount.
    {
        // Mengembalikan memanggil `TryGetDouble` dengan `payload`, `”amount”`, `amount` kepada pemanggil dalam TryReadAmount; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return TryGetDouble(payload, "amount", out amount);
    // Menutup scope metode TryReadAmount; bagian berikut berada di luar batas blok tersebut dalam TryReadAmount.
    }

    /// <summary>
    /// Membaca trade_type, qty, unit_price, dan amount dari payload event InvestasiEmas dan JualEmas.
    /// </summary>
    // Mendefinisikan metode `TryReadGoldTrade` dengan hasil bertipe `bool`. Membaca trade_type, qty, unit_price, dan amount dari payload event
    // InvestasiEmas dan JualEmas. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter
    // `tradeType` bertipe `string` membawa nilai trade jenis; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `qty`
    // bertipe `int` membawa nilai qty; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `unitPrice` bertipe `int`
    // membawa nilai unit harga; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal
    // uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadGoldTrade(JsonElement payload, out string tradeType, out int qty, out int unitPrice, out int amount)
    // Membuka scope metode TryReadGoldTrade; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadGoldTrade.
    {
        // Memperbarui `tradeType` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadGoldTrade.
        tradeType = string.Empty;
        // Memperbarui `qty` menggunakan nilai literal `0` dalam TryReadGoldTrade.
        qty = 0;
        // Memperbarui `unitPrice` menggunakan nilai literal `0` dalam TryReadGoldTrade.
        unitPrice = 0;
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadGoldTrade.
        amount = 0;

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(payload, ”trade_type”, out tradeType) ||
        // !TryGetInt32(payload, ”qty”, out qty) || !TryGetInt32(payload, ”unit_price”, out unitPrice)` dan `!TryGetInt32(payload, ”amount”, out amount)`;
        // sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadGoldTrade.
        if (!TryGetString(payload, "trade_type", out tradeType) ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”qty”, out qty)` sebagai bagian ekspresi yang sedang disusun dalam TryReadGoldTrade.
            !TryGetInt32(payload, "qty", out qty) ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”unit_price”, out unitPrice)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryReadGoldTrade.
            !TryGetInt32(payload, "unit_price", out unitPrice) ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”amount”, out amount)` sebagai bagian ekspresi yang sedang disusun dalam TryReadGoldTrade.
            !TryGetInt32(payload, "amount", out amount))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”trade_type”, out tradeType) || !TryGetInt32(payload, ”qty”, out qty) ||
        // !TryGetInt32(payload, ”unit_price”, out unitPrice) || !TryGetInt32(payload, ”am...`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam TryReadGoldTrade.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadGoldTrade; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”trade_type”, out tradeType) || !TryGetInt32(payload, ”qty”, out qty) ||
        // !TryGetInt32(payload, ”unit_price”, out unitPrice) || !TryGetInt32(payload, ”am...`; bagian berikut berada di luar batas blok tersebut dalam
        // TryReadGoldTrade.
        }

        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryReadGoldTrade; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return true;
    // Menutup scope metode TryReadGoldTrade; bagian berikut berada di luar batas blok tersebut dalam TryReadGoldTrade.
    }

    /// <summary>
    /// Membaca jumlah aksi terpakai dan sisa dari payload event AkhirGiliran.
    /// </summary>
    // Mendefinisikan metode `TryReadActionUsed` dengan hasil bertipe `bool`. Membaca jumlah aksi terpakai dan sisa dari payload event AkhirGiliran.
    // Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `used` bertipe `int` membawa nilai
    // used; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `remaining` bertipe `int` membawa nilai tersisa; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadActionUsed(JsonElement payload, out int used, out int remaining)
    // Membuka scope metode TryReadActionUsed; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadActionUsed.
    {
        // Memperbarui `used` menggunakan nilai literal `0` dalam TryReadActionUsed.
        used = 0;
        // Memperbarui `remaining` menggunakan nilai literal `0` dalam TryReadActionUsed.
        remaining = 0;
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetInt32(payload, ”used”, out used)` dan `!TryGetInt32(payload,
        // ”remaining”, out remaining)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryReadActionUsed.
        if (!TryGetInt32(payload, "used", out used) ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”remaining”, out remaining)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryReadActionUsed.
            !TryGetInt32(payload, "remaining", out remaining))
        // Membuka scope cabang if untuk kondisi `!TryGetInt32(payload, ”used”, out used) || !TryGetInt32(payload, ”remaining”, out remaining)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadActionUsed.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadActionUsed; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!TryGetInt32(payload, ”used”, out used) || !TryGetInt32(payload, ”remaining”, out remaining)`; bagian
        // berikut berada di luar batas blok tersebut dalam TryReadActionUsed.
        }

        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryReadActionUsed; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return true;
    // Menutup scope metode TryReadActionUsed; bagian berikut berada di luar batas blok tersebut dalam TryReadActionUsed.
    }

    /// <summary>
    /// Membaca card_id dan amount dari payload event BahanMasakan atau BuangBahanMasakan.
    /// </summary>
    // Mendefinisikan metode `TryReadIngredientPurchase` dengan hasil bertipe `bool`. Membaca card_id dan amount dari payload event BahanMasakan atau
    // BuangBahanMasakan. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `cardId` bertipe
    // `string` membawa nilai kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int`
    // membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadIngredientPurchase(JsonElement payload, out string cardId, out int amount)
    // Membuka scope metode TryReadIngredientPurchase; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadIngredientPurchase.
    {
        // Memperbarui `cardId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadIngredientPurchase.
        cardId = string.Empty;
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadIngredientPurchase.
        amount = 0;

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(payload, ”card_id”, out cardId)` dan
        // `!TryGetInt32(payload, ”amount”, out amount)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryReadIngredientPurchase.
        if (!TryGetString(payload, "card_id", out cardId) ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”amount”, out amount)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryReadIngredientPurchase.
            !TryGetInt32(payload, "amount", out amount))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”card_id”, out cardId) || !TryGetInt32(payload, ”amount”, out amount)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadIngredientPurchase.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadIngredientPurchase; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”card_id”, out cardId) || !TryGetInt32(payload, ”amount”, out amount)`; bagian
        // berikut berada di luar batas blok tersebut dalam TryReadIngredientPurchase.
        }

        // Mengembalikan kebalikan kondisi `string.IsNullOrWhiteSpace(cardId)` kepada pemanggil dalam TryReadIngredientPurchase; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return !string.IsNullOrWhiteSpace(cardId);
    // Menutup scope metode TryReadIngredientPurchase; bagian berikut berada di luar batas blok tersebut dalam TryReadIngredientPurchase.
    }

    /// <summary>
    /// Membaca daftar kartu bahan yang dibutuhkan dan pendapatan dari payload event JualMasakan.
    /// </summary>
    // Mendefinisikan metode `TryReadOrderClaim` dengan hasil bertipe `bool`. Membaca daftar kartu bahan yang dibutuhkan dan pendapatan dari payload
    // event JualMasakan. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `requiredCards`
    // bertipe `List<string>` membawa nilai required kartu; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `income`
    // bertipe `int` membawa nilai pemasukan; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadOrderClaim(JsonElement payload, out List<string> requiredCards, out int income)
    // Membuka scope metode TryReadOrderClaim; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadOrderClaim.
    {
        // Memperbarui `requiredCards` menggunakan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya dalam TryReadOrderClaim.
        requiredCards = new List<string>();
        // Memperbarui `income` menggunakan nilai literal `0` dalam TryReadOrderClaim.
        income = 0;

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!payload.TryGetProperty(”required_ingredient_card_ids”, out var
        // cardsProp) || cardsProp.ValueKind != JsonValueKind.Array` dan `!TryGetInt32(payload, ”income”, out income)`; sisi kanan diperiksa hanya jika sisi
        // kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadOrderClaim.
        if (!payload.TryGetProperty("required_ingredient_card_ids", out var cardsProp) ||
            // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `cardsProp.ValueKind` dan `JsonValueKind.Array` dalam TryReadOrderClaim.
            cardsProp.ValueKind != JsonValueKind.Array ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”income”, out income)` sebagai bagian ekspresi yang sedang disusun dalam TryReadOrderClaim.
            !TryGetInt32(payload, "income", out income))
        // Membuka scope cabang if untuk kondisi `!payload.TryGetProperty(”required_ingredient_card_ids”, out var cardsProp) || cardsProp.ValueKind !=
        // JsonValueKind.Array || !TryGetInt32(payload, ”income”, out income)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryReadOrderClaim.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadOrderClaim; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!payload.TryGetProperty(”required_ingredient_card_ids”, out var cardsProp) || cardsProp.ValueKind !=
        // JsonValueKind.Array || !TryGetInt32(payload, ”income”, out income)`; bagian berikut berada di luar batas blok tersebut dalam TryReadOrderClaim.
        }

        // Mengulangi setiap elemen `cardsProp.EnumerateArray()`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
        // TryReadOrderClaim.
        foreach (var item in cardsProp.EnumerateArray())
        // Membuka scope loop setiap item dari `cardsProp.EnumerateArray()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryReadOrderClaim.
        {
            // Memeriksa perbandingan ketidaksamaan antara `item.ValueKind` dan `JsonValueKind.String`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam TryReadOrderClaim.
            if (item.ValueKind != JsonValueKind.String)
            // Membuka scope cabang if untuk kondisi `item.ValueKind != JsonValueKind.String`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // TryReadOrderClaim.
            {
                // Menjalankan mengosongkan seluruh elemen `requiredCards` dalam TryReadOrderClaim.
                requiredCards.Clear();
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadOrderClaim; eksekusi jalur ini selesai setelah nilai
                // hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `item.ValueKind != JsonValueKind.String`; bagian berikut berada di luar batas blok tersebut dalam
            // TryReadOrderClaim.
            }

            // Menyiapkan variabel lokal `cardId` untuk nilai kartu identitas dengan membaca nilai string dari `item` sesuai tipe JSON atau sumber data yang
            // digunakan. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var cardId = item.GetString();
            // Memeriksa kebalikan kondisi `string.IsNullOrWhiteSpace(cardId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // TryReadOrderClaim.
            if (!string.IsNullOrWhiteSpace(cardId))
            // Membuka scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(cardId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // TryReadOrderClaim.
            {
                // Menjalankan menambahkan `cardId` ke `requiredCards` dalam TryReadOrderClaim.
                requiredCards.Add(cardId);
            // Menutup scope cabang if untuk kondisi `!string.IsNullOrWhiteSpace(cardId)`; bagian berikut berada di luar batas blok tersebut dalam
            // TryReadOrderClaim.
            }
        // Menutup scope loop setiap item dari `cardsProp.EnumerateArray()`; bagian berikut berada di luar batas blok tersebut dalam TryReadOrderClaim.
        }

        // Mengembalikan pemeriksaan lebih besar antara `requiredCards.Count` dan `0` kepada pemanggil dalam TryReadOrderClaim; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return requiredCards.Count > 0;
    // Menutup scope metode TryReadOrderClaim; bagian berikut berada di luar batas blok tersebut dalam TryReadOrderClaim.
    }

    /// <summary>
    /// Membaca card_id, amount, dan points dari payload event pembelian kebutuhan.
    /// </summary>
    // Mendefinisikan metode `TryReadNeedPurchase` dengan hasil bertipe `bool`. Membaca card_id, amount, dan points dari payload event pembelian
    // kebutuhan. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `cardId` bertipe `string`
    // membawa nilai kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa
    // nominal uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `points` bertipe `int` membawa nilai poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadNeedPurchase(JsonElement payload, out string cardId, out int amount, out int points)
    // Membuka scope metode TryReadNeedPurchase; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadNeedPurchase.
    {
        // Memperbarui `cardId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadNeedPurchase.
        cardId = string.Empty;
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadNeedPurchase.
        amount = 0;
        // Memperbarui `points` menggunakan nilai literal `0` dalam TryReadNeedPurchase.
        points = 0;

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(payload, ”card_id”, out cardId) || !TryGetInt32(payload,
        // ”amount”, out amount)` dan `!TryGetInt32(payload, ”points”, out points, required: false)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok
        // if hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadNeedPurchase.
        if (!TryGetString(payload, "card_id", out cardId) ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”amount”, out amount)` sebagai bagian ekspresi yang sedang disusun dalam TryReadNeedPurchase.
            !TryGetInt32(payload, "amount", out amount) ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”points”, out points, required: false)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryReadNeedPurchase.
            !TryGetInt32(payload, "points", out points, required: false))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”card_id”, out cardId) || !TryGetInt32(payload, ”amount”, out amount) ||
        // !TryGetInt32(payload, ”points”, out points, required: false)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryReadNeedPurchase.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadNeedPurchase; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”card_id”, out cardId) || !TryGetInt32(payload, ”amount”, out amount) ||
        // !TryGetInt32(payload, ”points”, out points, required: false)`; bagian berikut berada di luar batas blok tersebut dalam TryReadNeedPurchase.
        }

        // Memeriksa memeriksa apakah `cardId` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam TryReadNeedPurchase.
        if (string.IsNullOrWhiteSpace(cardId))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(cardId)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryReadNeedPurchase.
        {
            // Memperbarui `cardId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadNeedPurchase.
            cardId = string.Empty;
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadNeedPurchase; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(cardId)`; bagian berikut berada di luar batas blok tersebut dalam
        // TryReadNeedPurchase.
        }

        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryReadNeedPurchase; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return true;
    // Menutup scope metode TryReadNeedPurchase; bagian berikut berada di luar batas blok tersebut dalam TryReadNeedPurchase.
    }

    /// <summary>
    /// Membaca mission_id, target_tertiary_card_id, dan penalty_points dari payload event misi.
    /// </summary>
    // Mendefinisikan metode `TryReadMissionAssigned` dengan hasil bertipe `bool`. Membaca mission_id, target_tertiary_card_id, dan penalty_points dari
    // payload event misi. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `missionId`
    // bertipe `string` membawa identitas misi koleksi yang ditugaskan; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `targetCardId` bertipe `string` membawa nilai target kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode;
    // Parameter `penaltyPoints` bertipe `int` membawa nilai penalti poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadMissionAssigned(JsonElement payload, out string missionId, out string targetCardId, out int penaltyPoints)
    // Membuka scope metode TryReadMissionAssigned; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadMissionAssigned.
    {
        // Memperbarui `missionId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadMissionAssigned.
        missionId = string.Empty;
        // Memperbarui `targetCardId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadMissionAssigned.
        targetCardId = string.Empty;
        // Memperbarui `penaltyPoints` menggunakan nilai literal `0` dalam TryReadMissionAssigned.
        penaltyPoints = 0;

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(payload, ”mission_id”, out missionId) ||
        // !TryGetString(payload, ”target_tertiary_card_id”, out targetCardId)` dan `!TryGetInt32(payload, ”penalty_points”, out penaltyPoints)`; sisi kanan
        // diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadMissionAssigned.
        if (!TryGetString(payload, "mission_id", out missionId) ||
            // Menggunakan kebalikan kondisi `TryGetString(payload, ”target_tertiary_card_id”, out targetCardId)` sebagai bagian ekspresi yang sedang disusun
            // dalam TryReadMissionAssigned.
            !TryGetString(payload, "target_tertiary_card_id", out targetCardId) ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”penalty_points”, out penaltyPoints)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryReadMissionAssigned.
            !TryGetInt32(payload, "penalty_points", out penaltyPoints))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”mission_id”, out missionId) || !TryGetString(payload, ”target_tertiary_card_id”,
        // out targetCardId) || !TryGetInt32(payload, ”penalty_points”, out penal...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryReadMissionAssigned.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadMissionAssigned; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”mission_id”, out missionId) || !TryGetString(payload, ”target_tertiary_card_id”,
        // out targetCardId) || !TryGetInt32(payload, ”penalty_points”, out penal...`; bagian berikut berada di luar batas blok tersebut dalam
        // TryReadMissionAssigned.
        }

        // Mengembalikan kebalikan kondisi `string.IsNullOrWhiteSpace(missionId)` kepada pemanggil dalam TryReadMissionAssigned; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return !string.IsNullOrWhiteSpace(missionId);
    // Menutup scope metode TryReadMissionAssigned; bagian berikut berada di luar batas blok tersebut dalam TryReadMissionAssigned.
    }

    /// <summary>
    /// Membaca nomor tie-breaker dari payload event BagikanTieBreaker.
    /// </summary>
    // Mendefinisikan metode `TryReadTieBreaker` dengan hasil bertipe `bool`. Membaca nomor tie-breaker dari payload event BagikanTieBreaker. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `number` bertipe `int` membawa nilai number;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadTieBreaker(JsonElement payload, out int number)
    // Membuka scope metode TryReadTieBreaker; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadTieBreaker.
    {
        // Mengembalikan memanggil `TryGetInt32` dengan `payload`, `”number”`, `number` kepada pemanggil dalam TryReadTieBreaker; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return TryGetInt32(payload, "number", out number);
    // Menutup scope metode TryReadTieBreaker; bagian berikut berada di luar batas blok tersebut dalam TryReadTieBreaker.
    }

    /// <summary>
    /// Membaca rank dan points dari payload event rank.awarded.
    /// </summary>
    // Mendefinisikan metode `TryReadRankAwarded` dengan hasil bertipe `bool`. Membaca rank dan points dari payload event rank.awarded. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `rank` bertipe `int` membawa nilai rank; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `points` bertipe `int` membawa nilai poin; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode.
    public bool TryReadRankAwarded(JsonElement payload, out int rank, out int points)
    // Membuka scope metode TryReadRankAwarded; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadRankAwarded.
    {
        // Memperbarui `rank` menggunakan nilai literal `0` dalam TryReadRankAwarded.
        rank = 0;
        // Memperbarui `points` menggunakan nilai literal `0` dalam TryReadRankAwarded.
        points = 0;
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetInt32(payload, ”rank”, out rank)` dan `!TryGetInt32(payload,
        // ”points”, out points)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryReadRankAwarded.
        if (!TryGetInt32(payload, "rank", out rank) ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”points”, out points)` sebagai bagian ekspresi yang sedang disusun dalam TryReadRankAwarded.
            !TryGetInt32(payload, "points", out points))
        // Membuka scope cabang if untuk kondisi `!TryGetInt32(payload, ”rank”, out rank) || !TryGetInt32(payload, ”points”, out points)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadRankAwarded.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadRankAwarded; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!TryGetInt32(payload, ”rank”, out rank) || !TryGetInt32(payload, ”points”, out points)`; bagian berikut
        // berada di luar batas blok tersebut dalam TryReadRankAwarded.
        }

        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryReadRankAwarded; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return true;
    // Menutup scope metode TryReadRankAwarded; bagian berikut berada di luar batas blok tersebut dalam TryReadRankAwarded.
    }

    /// <summary>
    /// Membaca nilai points dari payload event points.awarded.
    /// </summary>
    // Mendefinisikan metode `TryReadPointsAwarded` dengan hasil bertipe `bool`. Membaca nilai points dari payload event points.awarded. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `points` bertipe `int` membawa nilai poin; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadPointsAwarded(JsonElement payload, out int points)
    // Membuka scope metode TryReadPointsAwarded; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadPointsAwarded.
    {
        // Mengembalikan memanggil `TryGetInt32` dengan `payload`, `”points”`, `points` kepada pemanggil dalam TryReadPointsAwarded; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return TryGetInt32(payload, "points", out points);
    // Menutup scope metode TryReadPointsAwarded; bagian berikut berada di luar batas blok tersebut dalam TryReadPointsAwarded.
    }

    /// <summary>
    /// Membaca goal_id dan amount dari payload event Menabung/TarikTabungan.
    /// </summary>
    // Mendefinisikan metode `TryReadSavingDeposit` dengan hasil bertipe `bool`. Membaca goal_id dan amount dari payload event Menabung/TarikTabungan.
    // Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `goalId` bertipe `string` membawa
    // nilai target identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal
    // uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadSavingDeposit(JsonElement payload, out string goalId, out int amount)
    // Membuka scope metode TryReadSavingDeposit; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadSavingDeposit.
    {
        // Memperbarui `goalId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadSavingDeposit.
        goalId = string.Empty;
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadSavingDeposit.
        amount = 0;
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(payload, ”goal_id”, out goalId)` dan
        // `!TryGetInt32(payload, ”amount”, out amount)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryReadSavingDeposit.
        if (!TryGetString(payload, "goal_id", out goalId) ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”amount”, out amount)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryReadSavingDeposit.
            !TryGetInt32(payload, "amount", out amount))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”goal_id”, out goalId) || !TryGetInt32(payload, ”amount”, out amount)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadSavingDeposit.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadSavingDeposit; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”goal_id”, out goalId) || !TryGetInt32(payload, ”amount”, out amount)`; bagian
        // berikut berada di luar batas blok tersebut dalam TryReadSavingDeposit.
        }

        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryReadSavingDeposit; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return true;
    // Menutup scope metode TryReadSavingDeposit; bagian berikut berada di luar batas blok tersebut dalam TryReadSavingDeposit.
    }

    /// <summary>
    /// Membaca goal_id, points, dan cost dari payload event TujuanFinansial.
    /// </summary>
    // Mendefinisikan metode `TryReadSavingGoalAchieved` dengan hasil bertipe `bool`. Membaca goal_id, points, dan cost dari payload event
    // TujuanFinansial. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `goalId` bertipe
    // `string` membawa nilai target identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `points` bertipe `int`
    // membawa nilai poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `cost` bertipe `int` membawa nilai biaya;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadSavingGoalAchieved(JsonElement payload, out string goalId, out int points, out int cost)
    // Membuka scope metode TryReadSavingGoalAchieved; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadSavingGoalAchieved.
    {
        // Memperbarui `goalId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadSavingGoalAchieved.
        goalId = string.Empty;
        // Memperbarui `points` menggunakan nilai literal `0` dalam TryReadSavingGoalAchieved.
        points = 0;
        // Memperbarui `cost` menggunakan nilai literal `0` dalam TryReadSavingGoalAchieved.
        cost = 0;
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(payload, ”goal_id”, out goalId) || !TryGetInt32(payload,
        // ”points”, out points)` dan `!TryGetInt32(payload, ”cost”, out cost, required: false)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if
        // hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadSavingGoalAchieved.
        if (!TryGetString(payload, "goal_id", out goalId) ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”points”, out points)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryReadSavingGoalAchieved.
            !TryGetInt32(payload, "points", out points) ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”cost”, out cost, required: false)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryReadSavingGoalAchieved.
            !TryGetInt32(payload, "cost", out cost, required: false))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”goal_id”, out goalId) || !TryGetInt32(payload, ”points”, out points) ||
        // !TryGetInt32(payload, ”cost”, out cost, required: false)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryReadSavingGoalAchieved.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadSavingGoalAchieved; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”goal_id”, out goalId) || !TryGetInt32(payload, ”points”, out points) ||
        // !TryGetInt32(payload, ”cost”, out cost, required: false)`; bagian berikut berada di luar batas blok tersebut dalam TryReadSavingGoalAchieved.
        }

        // Mengembalikan kebalikan kondisi `string.IsNullOrWhiteSpace(goalId)` kepada pemanggil dalam TryReadSavingGoalAchieved; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return !string.IsNullOrWhiteSpace(goalId);
    // Menutup scope metode TryReadSavingGoalAchieved; bagian berikut berada di luar batas blok tersebut dalam TryReadSavingGoalAchieved.
    }

    /// <summary>
    /// Membaca risk_id, direction, dan amount dari payload event RisikoKehidupan.
    /// </summary>
    // Mendefinisikan metode `TryReadRiskLife` dengan hasil bertipe `bool`. Membaca risk_id, direction, dan amount dari payload event RisikoKehidupan.
    // Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `riskId` bertipe `string` membawa
    // nilai risiko identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `direction` bertipe `string` membawa
    // nilai direction; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang
    // atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadRiskLife(JsonElement payload, out string riskId, out string direction, out int amount)
    // Membuka scope metode TryReadRiskLife; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadRiskLife.
    {
        // Memperbarui `riskId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadRiskLife.
        riskId = string.Empty;
        // Memperbarui `direction` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadRiskLife.
        direction = string.Empty;
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadRiskLife.
        amount = 0;
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(payload, ”risk_id”, out riskId) || !TryGetString(payload,
        // ”direction”, out direction)` dan `!TryGetInt32(payload, ”amount”, out amount)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam TryReadRiskLife.
        if (!TryGetString(payload, "risk_id", out riskId) ||
            // Menggunakan kebalikan kondisi `TryGetString(payload, ”direction”, out direction)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryReadRiskLife.
            !TryGetString(payload, "direction", out direction) ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”amount”, out amount)` sebagai bagian ekspresi yang sedang disusun dalam TryReadRiskLife.
            !TryGetInt32(payload, "amount", out amount))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”risk_id”, out riskId) || !TryGetString(payload, ”direction”, out direction) ||
        // !TryGetInt32(payload, ”amount”, out amount)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadRiskLife.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadRiskLife; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”risk_id”, out riskId) || !TryGetString(payload, ”direction”, out direction) ||
        // !TryGetInt32(payload, ”amount”, out amount)`; bagian berikut berada di luar batas blok tersebut dalam TryReadRiskLife.
        }

        // Mengembalikan gabungan syarat OR: setidaknya satu kondisi wajib benar antara `direction.Equals(”IN”, StringComparison.OrdinalIgnoreCase)` dan
        // `direction.Equals(”OUT”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri salah kepada pemanggil dalam
        // TryReadRiskLife; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return direction.Equals("IN", StringComparison.OrdinalIgnoreCase) ||
               // Melanjutkan pengolahan dengan membandingkan kesamaan `direction` dengan `”OUT”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
               // mengikuti overload dan comparer yang diberikan dalam TryReadRiskLife.
               direction.Equals("OUT", StringComparison.OrdinalIgnoreCase);
    // Menutup scope metode TryReadRiskLife; bagian berikut berada di luar batas blok tersebut dalam TryReadRiskLife.
    }

    /// <summary>
    /// Membaca risk_event_id dari payload event klaim Asuransi.
    /// </summary>
    // Mendefinisikan metode `TryReadInsuranceUsed` dengan hasil bertipe `bool`. Membaca risk_event_id dari payload event klaim Asuransi. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `riskEventId` bertipe `string` membawa nilai
    // risiko event identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadInsuranceUsed(JsonElement payload, out string riskEventId)
    // Membuka scope metode TryReadInsuranceUsed; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadInsuranceUsed.
    {
        // Memperbarui `riskEventId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadInsuranceUsed.
        riskEventId = string.Empty;
        // Memeriksa kebalikan kondisi `TryGetString(payload, ”risk_event_id”, out riskEventId)`; blok if hanya dijalankan ketika kondisi ini bernilai benar
        // dalam TryReadInsuranceUsed.
        if (!TryGetString(payload, "risk_event_id", out riskEventId))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”risk_event_id”, out riskEventId)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam TryReadInsuranceUsed.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadInsuranceUsed; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”risk_event_id”, out riskEventId)`; bagian berikut berada di luar batas blok
        // tersebut dalam TryReadInsuranceUsed.
        }

        // Mengembalikan kebalikan kondisi `string.IsNullOrWhiteSpace(riskEventId)` kepada pemanggil dalam TryReadInsuranceUsed; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return !string.IsNullOrWhiteSpace(riskEventId);
    // Menutup scope metode TryReadInsuranceUsed; bagian berikut berada di luar batas blok tersebut dalam TryReadInsuranceUsed.
    }

    /// <summary>
    /// Membaca opsi darurat yang nominal dan arahnya sudah dihitung server.
    /// </summary>
    // Mendefinisikan metode `TryReadEmergencyOption` dengan hasil bertipe `bool`. Membaca opsi darurat yang nominal dan arahnya sudah dihitung server.
    // Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `riskEventId` bertipe `string`
    // membawa nilai risiko event identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `optionType` bertipe
    // `string` membawa nilai option jenis; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `direction` bertipe
    // `string` membawa nilai direction; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa
    // nominal uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadEmergencyOption(
        // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON.
        JsonElement payload,
        // Parameter `riskEventId` bertipe `string` membawa nilai risiko event identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh
        // metode.
        out string riskEventId,
        // Parameter `optionType` bertipe `string` membawa nilai option jenis; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string optionType,
        // Parameter `direction` bertipe `string` membawa nilai direction; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string direction,
        // Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter
        // dan harus diisi oleh metode.
        out int amount)
    // Membuka scope metode TryReadEmergencyOption; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadEmergencyOption.
    {
        // Memperbarui `riskEventId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadEmergencyOption.
        riskEventId = string.Empty;
        // Memperbarui `optionType` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadEmergencyOption.
        optionType = string.Empty;
        // Memperbarui `direction` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadEmergencyOption.
        direction = string.Empty;
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadEmergencyOption.
        amount = 0;

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(payload, ”risk_event_id”, out riskEventId)` dan
        // `!TryGetString(payload, ”option_type”, out optionType)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam TryReadEmergencyOption.
        if (!TryGetString(payload, "risk_event_id", out riskEventId) ||
            // Menggunakan kebalikan kondisi `TryGetString(payload, ”option_type”, out optionType)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryReadEmergencyOption.
            !TryGetString(payload, "option_type", out optionType))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”risk_event_id”, out riskEventId) || !TryGetString(payload, ”option_type”, out
        // optionType)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadEmergencyOption.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadEmergencyOption; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”risk_event_id”, out riskEventId) || !TryGetString(payload, ”option_type”, out
        // optionType)`; bagian berikut berada di luar batas blok tersebut dalam TryReadEmergencyOption.
        }

        // Memperbarui `direction` menggunakan nilai literal `”IN”` dalam TryReadEmergencyOption.
        direction = "IN";
        // Memeriksa membandingkan kesamaan `string` dengan `optionType`, `”TAKE_SHARIA_LOAN”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
        // mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadEmergencyOption.
        if (string.Equals(optionType, "TAKE_SHARIA_LOAN", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(optionType, ”TAKE_SHARIA_LOAN”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam TryReadEmergencyOption.
        {
            // Menjalankan memanggil `TryGetInt32` dengan `payload`, `”principal”`, `amount` dalam TryReadEmergencyOption.
            TryGetInt32(payload, "principal", out amount);
        // Menutup scope cabang if untuk kondisi `string.Equals(optionType, ”TAKE_SHARIA_LOAN”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada
        // di luar batas blok tersebut dalam TryReadEmergencyOption.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam TryReadEmergencyOption.
        else
        // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadEmergencyOption.
        {
            // Menjalankan memanggil `TryGetInt32` dengan `payload`, `”amount”`, `amount` dalam TryReadEmergencyOption.
            TryGetInt32(payload, "amount", out amount);
        // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam TryReadEmergencyOption.
        }

        // Mengembalikan gabungan syarat AND: kedua kondisi wajib benar antara `!string.IsNullOrWhiteSpace(riskEventId)` dan `amount > 0`; sisi kanan
        // diperiksa hanya jika sisi kiri benar kepada pemanggil dalam TryReadEmergencyOption; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return !string.IsNullOrWhiteSpace(riskEventId) && amount > 0;
    // Menutup scope metode TryReadEmergencyOption; bagian berikut berada di luar batas blok tersebut dalam TryReadEmergencyOption.
    }

    /// <summary>
    /// Membaca loan_id, principal, repayment_amount opsional, duration opsional, dan penalty_points dari payload event pinjaman.
    /// </summary>
    // Mendefinisikan metode `TryReadLoanTaken` dengan hasil bertipe `bool`. Membaca loan_id, principal, repayment_amount opsional, duration opsional,
    // dan penalty_points dari payload event pinjaman. Masukan: Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON;
    // Parameter `loanId` bertipe `string` membawa nilai pinjaman identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode;
    // Parameter `principal` bertipe `int` membawa nilai principal; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `repaymentAmount` bertipe `int` membawa nilai repayment nominal; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `duration` bertipe `int` membawa nilai duration; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `penaltyPoints`
    // bertipe `int` membawa nilai penalti poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadLoanTaken(
        // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON.
        JsonElement payload,
        // Parameter `loanId` bertipe `string` membawa nilai pinjaman identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string loanId,
        // Parameter `principal` bertipe `int` membawa nilai principal; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int principal,
        // Parameter `repaymentAmount` bertipe `int` membawa nilai repayment nominal; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int repaymentAmount,
        // Parameter `duration` bertipe `int` membawa nilai duration; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int duration,
        // Parameter `penaltyPoints` bertipe `int` membawa nilai penalti poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int penaltyPoints)
    // Membuka scope metode TryReadLoanTaken; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadLoanTaken.
    {
        // Memperbarui `loanId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadLoanTaken.
        loanId = string.Empty;
        // Memperbarui `principal` menggunakan nilai literal `0` dalam TryReadLoanTaken.
        principal = 0;
        // Memperbarui `repaymentAmount` menggunakan nilai literal `0` dalam TryReadLoanTaken.
        repaymentAmount = 0;
        // Memperbarui `duration` menggunakan nilai literal `0` dalam TryReadLoanTaken.
        duration = 0;
        // Memperbarui `penaltyPoints` menggunakan nilai literal `0` dalam TryReadLoanTaken.
        penaltyPoints = 0;

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(payload, ”loan_id”, out loanId) || !TryGetInt32(payload,
        // ”principal”, out principal)` dan `!TryGetInt32(payload, ”penalty_points”, out penaltyPoints)`; sisi kanan diperiksa hanya jika sisi kiri salah;
        // blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadLoanTaken.
        if (!TryGetString(payload, "loan_id", out loanId) ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”principal”, out principal)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryReadLoanTaken.
            !TryGetInt32(payload, "principal", out principal) ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”penalty_points”, out penaltyPoints)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryReadLoanTaken.
            !TryGetInt32(payload, "penalty_points", out penaltyPoints))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”loan_id”, out loanId) || !TryGetInt32(payload, ”principal”, out principal) ||
        // !TryGetInt32(payload, ”penalty_points”, out penaltyPoints)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadLoanTaken.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadLoanTaken; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”loan_id”, out loanId) || !TryGetInt32(payload, ”principal”, out principal) ||
        // !TryGetInt32(payload, ”penalty_points”, out penaltyPoints)`; bagian berikut berada di luar batas blok tersebut dalam TryReadLoanTaken.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!TryGetInt32(payload, ”repayment_amount”, out repaymentAmount, required: false)`
        // dan `!TryGetInt32(payload, ”installment”, out repaymentAmount, required: false)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam TryReadLoanTaken.
        if (!TryGetInt32(payload, "repayment_amount", out repaymentAmount, required: false) &&
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”installment”, out repaymentAmount, required: false)` sebagai bagian ekspresi yang sedang
            // disusun dalam TryReadLoanTaken.
            !TryGetInt32(payload, "installment", out repaymentAmount, required: false))
        // Membuka scope cabang if untuk kondisi `!TryGetInt32(payload, ”repayment_amount”, out repaymentAmount, required: false) && !TryGetInt32(payload,
        // ”installment”, out repaymentAmount, required: false)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadLoanTaken.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadLoanTaken; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!TryGetInt32(payload, ”repayment_amount”, out repaymentAmount, required: false) && !TryGetInt32(payload,
        // ”installment”, out repaymentAmount, required: false)`; bagian berikut berada di luar batas blok tersebut dalam TryReadLoanTaken.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetInt32(payload, ”duration_turn”, out duration, required: false)`
        // dan `(duration == 0 && !TryGetInt32(payload, ”duration_turns”, out duration, required: false))`; sisi kanan diperiksa hanya jika sisi kiri salah;
        // blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadLoanTaken.
        if (!TryGetInt32(payload, "duration_turn", out duration, required: false) ||
            // Menggunakan gabungan syarat AND: kedua kondisi wajib benar antara `duration == 0` dan `!TryGetInt32(payload, ”duration_turns”, out duration,
            // required: false)`; sisi kanan diperiksa hanya jika sisi kiri benar sebagai bagian ekspresi yang sedang disusun dalam TryReadLoanTaken.
            (duration == 0 && !TryGetInt32(payload, "duration_turns", out duration, required: false)))
        // Membuka scope cabang if untuk kondisi `!TryGetInt32(payload, ”duration_turn”, out duration, required: false) || (duration == 0 &&
        // !TryGetInt32(payload, ”duration_turns”, out duration, required: false))`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryReadLoanTaken.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadLoanTaken; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!TryGetInt32(payload, ”duration_turn”, out duration, required: false) || (duration == 0 &&
        // !TryGetInt32(payload, ”duration_turns”, out duration, required: false))`; bagian berikut berada di luar batas blok tersebut dalam
        // TryReadLoanTaken.
        }

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `duration == 0` dan `!TryGetInt32(payload, ”duration_days”, out duration,
        // required: false)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TryReadLoanTaken.
        if (duration == 0 &&
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”duration_days”, out duration, required: false)` sebagai bagian ekspresi yang sedang disusun
            // dalam TryReadLoanTaken.
            !TryGetInt32(payload, "duration_days", out duration, required: false))
        // Membuka scope cabang if untuk kondisi `duration == 0 && !TryGetInt32(payload, ”duration_days”, out duration, required: false)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadLoanTaken.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadLoanTaken; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `duration == 0 && !TryGetInt32(payload, ”duration_days”, out duration, required: false)`; bagian berikut
        // berada di luar batas blok tersebut dalam TryReadLoanTaken.
        }

        // Memeriksa perbandingan kesamaan antara `duration` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadLoanTaken.
        if (duration == 0)
        // Membuka scope cabang if untuk kondisi `duration == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadLoanTaken.
        {
            // Memperbarui `duration` menggunakan nilai literal `1` dalam TryReadLoanTaken.
            duration = 1;
        // Menutup scope cabang if untuk kondisi `duration == 0`; bagian berikut berada di luar batas blok tersebut dalam TryReadLoanTaken.
        }

        // Mengembalikan kebalikan kondisi `string.IsNullOrWhiteSpace(loanId)` kepada pemanggil dalam TryReadLoanTaken; eksekusi jalur ini selesai setelah
        // nilai hasil ditentukan.
        return !string.IsNullOrWhiteSpace(loanId);
    // Menutup scope metode TryReadLoanTaken; bagian berikut berada di luar batas blok tersebut dalam TryReadLoanTaken.
    }

    /// <summary>
    /// Membaca loan_id dan amount dari payload event BayarPinjaman.
    /// </summary>
    // Mendefinisikan metode `TryReadLoanRepay` dengan hasil bertipe `bool`. Membaca loan_id dan amount dari payload event BayarPinjaman. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `loanId` bertipe `string` membawa nilai
    // pinjaman identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang
    // atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadLoanRepay(JsonElement payload, out string loanId, out int amount)
    // Membuka scope metode TryReadLoanRepay; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadLoanRepay.
    {
        // Memperbarui `loanId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadLoanRepay.
        loanId = string.Empty;
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadLoanRepay.
        amount = 0;
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!TryGetString(payload, ”loan_id”, out loanId)` dan
        // `!TryGetInt32(payload, ”amount”, out amount)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryReadLoanRepay.
        if (!TryGetString(payload, "loan_id", out loanId) ||
            // Menggunakan kebalikan kondisi `TryGetInt32(payload, ”amount”, out amount)` sebagai bagian ekspresi yang sedang disusun dalam TryReadLoanRepay.
            !TryGetInt32(payload, "amount", out amount))
        // Membuka scope cabang if untuk kondisi `!TryGetString(payload, ”loan_id”, out loanId) || !TryGetInt32(payload, ”amount”, out amount)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadLoanRepay.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadLoanRepay; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!TryGetString(payload, ”loan_id”, out loanId) || !TryGetInt32(payload, ”amount”, out amount)`; bagian
        // berikut berada di luar batas blok tersebut dalam TryReadLoanRepay.
        }

        // Mengembalikan kebalikan kondisi `string.IsNullOrWhiteSpace(loanId)` kepada pemanggil dalam TryReadLoanRepay; eksekusi jalur ini selesai setelah
        // nilai hasil ditentukan.
        return !string.IsNullOrWhiteSpace(loanId);
    // Menutup scope metode TryReadLoanRepay; bagian berikut berada di luar batas blok tersebut dalam TryReadLoanRepay.
    }

    /// <summary>
    /// Membaca nilai premium dari payload event pembelian Asuransi.
    /// </summary>
    // Mendefinisikan metode `TryReadInsurance` dengan hasil bertipe `bool`. Membaca nilai premium dari payload event pembelian Asuransi. Masukan:
    // Parameter `payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `premium` bertipe `int` membawa nilai premium;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadInsurance(JsonElement payload, out int premium)
    // Membuka scope metode TryReadInsurance; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadInsurance.
    {
        // Mengembalikan memanggil `TryGetInt32` dengan `payload`, `”premium”`, `premium` kepada pemanggil dalam TryReadInsurance; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return TryGetInt32(payload, "premium", out premium);
    // Menutup scope metode TryReadInsurance; bagian berikut berada di luar batas blok tersebut dalam TryReadInsurance.
    }
// Menutup scope tipe EventPayloadReader; bagian berikut berada di luar batas blok tersebut.
}
