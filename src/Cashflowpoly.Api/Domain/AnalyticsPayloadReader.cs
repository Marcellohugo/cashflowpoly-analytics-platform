// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui AnalyticsPayloadReader.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Parser payload event gameplay yang dipakai oleh pipeline analitik.
/// </summary>
// Mendefinisikan tipe class `AnalyticsPayloadReader` yang mewarisi atau menerapkan `IAnalyticsPayloadReader`; sealed mencegah tipe ini diturunkan
// lagi.
internal sealed class AnalyticsPayloadReader : IAnalyticsPayloadReader
// Membuka scope tipe AnalyticsPayloadReader; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Membaca direction, amount, dan category dari payload JSON transaksi.
    /// </summary>
    // Mendefinisikan metode `TryReadTransaction` dengan hasil bertipe `bool`. Membaca direction, amount, dan category dari payload JSON transaksi.
    // Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `direction` bertipe `string` membawa nilai direction; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `double` membawa nominal uang atau nilai transaksi
    // yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `category` bertipe `string` membawa
    // nilai category; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadTransaction(string payloadJson, out string direction, out double amount, out string category)
    // Membuka scope metode TryReadTransaction; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadTransaction.
    {
        // Memperbarui `direction` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadTransaction.
        direction = string.Empty;
        // Memperbarui `category` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadTransaction.
        category = string.Empty;
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadTransaction.
        amount = 0;

        // Memulai blok try dalam TryReadTransaction; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadTransaction.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Menyiapkan variabel lokal `root` untuk nilai root dengan `doc.RootElement` (nilai root element). Tipe variabel disimpulkan dari ekspresi nilai
            // awal.
            var root = doc.RootElement;
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!root.TryGetProperty(”direction”, out var directionProp) ||
            // !root.TryGetProperty(”amount”, out var amountProp)` dan `!root.TryGetProperty(”category”, out var categoryProp)`; sisi kanan diperiksa hanya jika
            // sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadTransaction.
            if (!root.TryGetProperty("direction", out var directionProp) ||
                // Menggunakan kebalikan kondisi `root.TryGetProperty(”amount”, out var amountProp)` sebagai bagian ekspresi yang sedang disusun dalam
                // TryReadTransaction.
                !root.TryGetProperty("amount", out var amountProp) ||
                // Menggunakan kebalikan kondisi `root.TryGetProperty(”category”, out var categoryProp)` sebagai bagian ekspresi yang sedang disusun dalam
                // TryReadTransaction.
                !root.TryGetProperty("category", out var categoryProp))
            // Membuka scope cabang if untuk kondisi `!root.TryGetProperty(”direction”, out var directionProp) || !root.TryGetProperty(”amount”, out var
            // amountProp) || !root.TryGetProperty(”category”, out var categoryProp)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // TryReadTransaction.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadTransaction; eksekusi jalur ini selesai setelah nilai
                // hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!root.TryGetProperty(”direction”, out var directionProp) || !root.TryGetProperty(”amount”, out var
            // amountProp) || !root.TryGetProperty(”category”, out var categoryProp)`; bagian berikut berada di luar batas blok tersebut dalam
            // TryReadTransaction.
            }

            // Memperbarui `direction` menggunakan `directionProp.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
            // TryReadTransaction.
            direction = directionProp.GetString() ?? string.Empty;
            // Memperbarui `category` menggunakan `categoryProp.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
            // TryReadTransaction.
            category = categoryProp.GetString() ?? string.Empty;
            // Memperbarui `amount` menggunakan memanggil `amountProp.GetDouble` dengan tanpa argumen dalam TryReadTransaction.
            amount = amountProp.GetDouble();
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryReadTransaction; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return true;
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadTransaction.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadTransaction.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadTransaction.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadTransaction; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadTransaction.
        }
    // Menutup scope metode TryReadTransaction; bagian berikut berada di luar batas blok tersebut dalam TryReadTransaction.
    }

    /// <summary>
    /// Membaca field amount dari payload JSON.
    /// </summary>
    // Mendefinisikan metode `TryReadAmount` dengan hasil bertipe `bool`. Membaca field amount dari payload JSON. Masukan: Parameter `payloadJson`
    // bertipe `string` membawa nilai payload JSON; Parameter `amount` bertipe `double` membawa nominal uang atau nilai transaksi yang dipakai dalam
    // operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadAmount(string payloadJson, out double amount)
    // Membuka scope metode TryReadAmount; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadAmount.
    {
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadAmount.
        amount = 0;
        // Memulai blok try dalam TryReadAmount; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadAmount.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Memeriksa kebalikan kondisi `doc.RootElement.TryGetProperty(”amount”, out var amountProp)`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam TryReadAmount.
            if (!doc.RootElement.TryGetProperty("amount", out var amountProp))
            // Membuka scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”amount”, out var amountProp)`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam TryReadAmount.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadAmount; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”amount”, out var amountProp)`; bagian berikut berada di luar batas blok
            // tersebut dalam TryReadAmount.
            }

            // Memperbarui `amount` menggunakan memanggil `amountProp.GetDouble` dengan tanpa argumen dalam TryReadAmount.
            amount = amountProp.GetDouble();
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryReadAmount; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return true;
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadAmount.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadAmount.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadAmount.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadAmount; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadAmount.
        }
    // Menutup scope metode TryReadAmount; bagian berikut berada di luar batas blok tersebut dalam TryReadAmount.
    }

    /// <summary>
    /// Membaca trade_type dan qty dari payload JSON perdagangan emas.
    /// </summary>
    // Mendefinisikan metode `TryReadGoldTrade` dengan hasil bertipe `bool`. Membaca trade_type dan qty dari payload JSON perdagangan emas. Masukan:
    // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `tradeType` bertipe `string` membawa nilai trade jenis; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `qty` bertipe `int` membawa nilai qty; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode.
    public bool TryReadGoldTrade(string payloadJson, out string tradeType, out int qty)
    // Membuka scope metode TryReadGoldTrade; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadGoldTrade.
    {
        // Memperbarui `tradeType` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadGoldTrade.
        tradeType = string.Empty;
        // Memperbarui `qty` menggunakan nilai literal `0` dalam TryReadGoldTrade.
        qty = 0;
        // Memulai blok try dalam TryReadGoldTrade; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadGoldTrade.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Menyiapkan variabel lokal `root` untuk nilai root dengan `doc.RootElement` (nilai root element). Tipe variabel disimpulkan dari ekspresi nilai
            // awal.
            var root = doc.RootElement;
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!root.TryGetProperty(”trade_type”, out var tradeTypeProp)` dan
            // `!root.TryGetProperty(”qty”, out var qtyProp)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
            // bernilai benar dalam TryReadGoldTrade.
            if (!root.TryGetProperty("trade_type", out var tradeTypeProp) ||
                // Menggunakan kebalikan kondisi `root.TryGetProperty(”qty”, out var qtyProp)` sebagai bagian ekspresi yang sedang disusun dalam TryReadGoldTrade.
                !root.TryGetProperty("qty", out var qtyProp))
            // Membuka scope cabang if untuk kondisi `!root.TryGetProperty(”trade_type”, out var tradeTypeProp) || !root.TryGetProperty(”qty”, out var
            // qtyProp)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadGoldTrade.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadGoldTrade; eksekusi jalur ini selesai setelah nilai
                // hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!root.TryGetProperty(”trade_type”, out var tradeTypeProp) || !root.TryGetProperty(”qty”, out var
            // qtyProp)`; bagian berikut berada di luar batas blok tersebut dalam TryReadGoldTrade.
            }

            // Memperbarui `tradeType` menggunakan `tradeTypeProp.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
            // TryReadGoldTrade.
            tradeType = tradeTypeProp.GetString() ?? string.Empty;
            // Memperbarui `qty` menggunakan memanggil `qtyProp.GetInt32` dengan tanpa argumen dalam TryReadGoldTrade.
            qty = qtyProp.GetInt32();
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryReadGoldTrade; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return true;
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadGoldTrade.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadGoldTrade.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadGoldTrade.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadGoldTrade; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadGoldTrade.
        }
    // Menutup scope metode TryReadGoldTrade; bagian berikut berada di luar batas blok tersebut dalam TryReadGoldTrade.
    }

    /// <summary>
    /// Memeriksa apakah tipe aksi termasuk event gameplay substantif (bukan meta-event seperti awarded/assigned).
    /// </summary>
    // Mendefinisikan metode `IsActionEvent` dengan hasil bertipe `bool`. Memeriksa apakah tipe aksi termasuk event gameplay substantif (bukan
    // meta-event seperti awarded/assigned). Masukan: Parameter `actionType` bertipe `string` membawa nilai aksi jenis.
    public bool IsActionEvent(string actionType)
    // Membuka scope metode IsActionEvent; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IsActionEvent.
    {
        // Memeriksa memeriksa apakah `actionType` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam IsActionEvent.
        if (string.IsNullOrWhiteSpace(actionType))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(actionType)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IsActionEvent.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam IsActionEvent; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(actionType)`; bagian berikut berada di luar batas blok tersebut dalam
        // IsActionEvent.
        }

        // Mengembalikan perbandingan kesamaan antara `GameActionCatalog.GetPlayerActionSlotPolicy(actionType, default)` dan
        // `PlayerActionSlotPolicy.Consumes` kepada pemanggil dalam IsActionEvent; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return GameActionCatalog.GetPlayerActionSlotPolicy(actionType, default) == PlayerActionSlotPolicy.Consumes;
    // Menutup scope metode IsActionEvent; bagian berikut berada di luar batas blok tersebut dalam IsActionEvent.
    }

    /// <summary>
    /// Membaca jumlah aksi terpakai dan tersisa dari payload JSON event AkhirGiliran.
    /// </summary>
    // Mendefinisikan metode `TryReadActionUsed` dengan hasil bertipe `bool`. Membaca jumlah aksi terpakai dan tersisa dari payload JSON event
    // AkhirGiliran. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `used` bertipe `int` membawa nilai used;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `remaining` bertipe `int` membawa nilai tersisa; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadActionUsed(string payloadJson, out int used, out int remaining)
    // Membuka scope metode TryReadActionUsed; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadActionUsed.
    {
        // Memperbarui `used` menggunakan nilai literal `0` dalam TryReadActionUsed.
        used = 0;
        // Memperbarui `remaining` menggunakan nilai literal `0` dalam TryReadActionUsed.
        remaining = 0;
        // Memulai blok try dalam TryReadActionUsed; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadActionUsed.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!doc.RootElement.TryGetProperty(”used”, out var usedProp)` dan
            // `!doc.RootElement.TryGetProperty(”remaining”, out var remainingProp)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan
            // ketika kondisi ini bernilai benar dalam TryReadActionUsed.
            if (!doc.RootElement.TryGetProperty("used", out var usedProp) ||
                // Menggunakan kebalikan kondisi `doc.RootElement.TryGetProperty(”remaining”, out var remainingProp)` sebagai bagian ekspresi yang sedang disusun
                // dalam TryReadActionUsed.
                !doc.RootElement.TryGetProperty("remaining", out var remainingProp))
            // Membuka scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”used”, out var usedProp) || !doc.RootElement.TryGetProperty(”remaining”,
            // out var remainingProp)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadActionUsed.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadActionUsed; eksekusi jalur ini selesai setelah nilai
                // hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”used”, out var usedProp) || !doc.RootElement.TryGetProperty(”remaining”,
            // out var remainingProp)`; bagian berikut berada di luar batas blok tersebut dalam TryReadActionUsed.
            }

            // Memperbarui `used` menggunakan memanggil `usedProp.GetInt32` dengan tanpa argumen dalam TryReadActionUsed.
            used = usedProp.GetInt32();
            // Memperbarui `remaining` menggunakan memanggil `remainingProp.GetInt32` dengan tanpa argumen dalam TryReadActionUsed.
            remaining = remainingProp.GetInt32();
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryReadActionUsed; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return true;
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadActionUsed.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadActionUsed.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadActionUsed.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadActionUsed; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadActionUsed.
        }
    // Menutup scope metode TryReadActionUsed; bagian berikut berada di luar batas blok tersebut dalam TryReadActionUsed.
    }

    /// <summary>
    /// Membaca detail lengkap perdagangan emas (tipe, kuantitas, harga satuan, jumlah) dari payload JSON.
    /// </summary>
    // Mendefinisikan metode `TryReadGoldTradeDetailed` dengan hasil bertipe `bool`. Membaca detail lengkap perdagangan emas (tipe, kuantitas, harga
    // satuan, jumlah) dari payload JSON. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `tradeType` bertipe
    // `string` membawa nilai trade jenis; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `qty` bertipe `int` membawa
    // nilai qty; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `unitPrice` bertipe `int` membawa nilai unit harga;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi
    // yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadGoldTradeDetailed(
        // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON.
        string payloadJson,
        // Parameter `tradeType` bertipe `string` membawa nilai trade jenis; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string tradeType,
        // Parameter `qty` bertipe `int` membawa nilai qty; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int qty,
        // Parameter `unitPrice` bertipe `int` membawa nilai unit harga; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int unitPrice,
        // Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter
        // dan harus diisi oleh metode.
        out int amount)
    // Membuka scope metode TryReadGoldTradeDetailed; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadGoldTradeDetailed.
    {
        // Memperbarui `tradeType` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadGoldTradeDetailed.
        tradeType = string.Empty;
        // Memperbarui `qty` menggunakan nilai literal `0` dalam TryReadGoldTradeDetailed.
        qty = 0;
        // Memperbarui `unitPrice` menggunakan nilai literal `0` dalam TryReadGoldTradeDetailed.
        unitPrice = 0;
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadGoldTradeDetailed.
        amount = 0;

        // Memulai blok try dalam TryReadGoldTradeDetailed; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan
        // saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadGoldTradeDetailed.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Menyiapkan variabel lokal `root` untuk nilai root dengan `doc.RootElement` (nilai root element). Tipe variabel disimpulkan dari ekspresi nilai
            // awal.
            var root = doc.RootElement;
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!root.TryGetProperty(”trade_type”, out var tradeTypeProp) ||
            // !root.TryGetProperty(”qty”, out var qtyProp) || !root.TryGetProperty(”unit_price”, out var unitPriceProp)` dan `!root.TryGetProperty(”amount”,
            // out var amountProp)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // TryReadGoldTradeDetailed.
            if (!root.TryGetProperty("trade_type", out var tradeTypeProp) ||
                // Menggunakan kebalikan kondisi `root.TryGetProperty(”qty”, out var qtyProp)` sebagai bagian ekspresi yang sedang disusun dalam
                // TryReadGoldTradeDetailed.
                !root.TryGetProperty("qty", out var qtyProp) ||
                // Menggunakan kebalikan kondisi `root.TryGetProperty(”unit_price”, out var unitPriceProp)` sebagai bagian ekspresi yang sedang disusun dalam
                // TryReadGoldTradeDetailed.
                !root.TryGetProperty("unit_price", out var unitPriceProp) ||
                // Menggunakan kebalikan kondisi `root.TryGetProperty(”amount”, out var amountProp)` sebagai bagian ekspresi yang sedang disusun dalam
                // TryReadGoldTradeDetailed.
                !root.TryGetProperty("amount", out var amountProp))
            // Membuka scope cabang if untuk kondisi `!root.TryGetProperty(”trade_type”, out var tradeTypeProp) || !root.TryGetProperty(”qty”, out var qtyProp)
            // || !root.TryGetProperty(”unit_price”, out var unitPriceProp) || !root...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // TryReadGoldTradeDetailed.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadGoldTradeDetailed; eksekusi jalur ini selesai setelah
                // nilai hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!root.TryGetProperty(”trade_type”, out var tradeTypeProp) || !root.TryGetProperty(”qty”, out var qtyProp)
            // || !root.TryGetProperty(”unit_price”, out var unitPriceProp) || !root...`; bagian berikut berada di luar batas blok tersebut dalam
            // TryReadGoldTradeDetailed.
            }

            // Memperbarui `tradeType` menggunakan `tradeTypeProp.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
            // TryReadGoldTradeDetailed.
            tradeType = tradeTypeProp.GetString() ?? string.Empty;
            // Memperbarui `qty` menggunakan memanggil `qtyProp.GetInt32` dengan tanpa argumen dalam TryReadGoldTradeDetailed.
            qty = qtyProp.GetInt32();
            // Memperbarui `unitPrice` menggunakan memanggil `unitPriceProp.GetInt32` dengan tanpa argumen dalam TryReadGoldTradeDetailed.
            unitPrice = unitPriceProp.GetInt32();
            // Memperbarui `amount` menggunakan memanggil `amountProp.GetInt32` dengan tanpa argumen dalam TryReadGoldTradeDetailed.
            amount = amountProp.GetInt32();
            // Mengembalikan pemeriksaan lebih besar antara `qty` dan `0` kepada pemanggil dalam TryReadGoldTradeDetailed; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return qty > 0;
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadGoldTradeDetailed.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadGoldTradeDetailed.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadGoldTradeDetailed.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadGoldTradeDetailed; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadGoldTradeDetailed.
        }
    // Menutup scope metode TryReadGoldTradeDetailed; bagian berikut berada di luar batas blok tersebut dalam TryReadGoldTradeDetailed.
    }

    /// <summary>
    /// Mem-parsing payload pembelian bahan baku detail: card_id, ingredient_name, amount.
    /// </summary>
    // Mendefinisikan metode `TryReadIngredientPurchaseDetailed` dengan hasil bertipe `bool`. Mem-parsing payload pembelian bahan baku detail: card_id,
    // ingredient_name, amount. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `cardId` bertipe `string`
    // membawa nilai kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `ingredientName` bertipe `string`
    // membawa nilai bahan nama; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal
    // uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadIngredientPurchaseDetailed(
        // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON.
        string payloadJson,
        // Parameter `cardId` bertipe `string` membawa nilai kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string cardId,
        // Parameter `ingredientName` bertipe `string` membawa nilai bahan nama; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string ingredientName,
        // Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter
        // dan harus diisi oleh metode.
        out int amount)
    // Membuka scope metode TryReadIngredientPurchaseDetailed; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryReadIngredientPurchaseDetailed.
    {
        // Memperbarui `cardId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadIngredientPurchaseDetailed.
        cardId = string.Empty;
        // Memperbarui `ingredientName` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadIngredientPurchaseDetailed.
        ingredientName = string.Empty;
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadIngredientPurchaseDetailed.
        amount = 0;

        // Memulai blok try dalam TryReadIngredientPurchaseDetailed; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap
        // dijalankan saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadIngredientPurchaseDetailed.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Menyiapkan variabel lokal `root` untuk nilai root dengan `doc.RootElement` (nilai root element). Tipe variabel disimpulkan dari ekspresi nilai
            // awal.
            var root = doc.RootElement;
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!root.TryGetProperty(”card_id”, out var cardIdProp) ||
            // !root.TryGetProperty(”ingredient_name”, out var nameProp)` dan `!root.TryGetProperty(”amount”, out var amountProp)`; sisi kanan diperiksa hanya
            // jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadIngredientPurchaseDetailed.
            if (!root.TryGetProperty("card_id", out var cardIdProp) ||
                // Menggunakan kebalikan kondisi `root.TryGetProperty(”ingredient_name”, out var nameProp)` sebagai bagian ekspresi yang sedang disusun dalam
                // TryReadIngredientPurchaseDetailed.
                !root.TryGetProperty("ingredient_name", out var nameProp) ||
                // Menggunakan kebalikan kondisi `root.TryGetProperty(”amount”, out var amountProp)` sebagai bagian ekspresi yang sedang disusun dalam
                // TryReadIngredientPurchaseDetailed.
                !root.TryGetProperty("amount", out var amountProp))
            // Membuka scope cabang if untuk kondisi `!root.TryGetProperty(”card_id”, out var cardIdProp) || !root.TryGetProperty(”ingredient_name”, out var
            // nameProp) || !root.TryGetProperty(”amount”, out var amountProp)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // TryReadIngredientPurchaseDetailed.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadIngredientPurchaseDetailed; eksekusi jalur ini selesai
                // setelah nilai hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!root.TryGetProperty(”card_id”, out var cardIdProp) || !root.TryGetProperty(”ingredient_name”, out var
            // nameProp) || !root.TryGetProperty(”amount”, out var amountProp)`; bagian berikut berada di luar batas blok tersebut dalam
            // TryReadIngredientPurchaseDetailed.
            }

            // Memperbarui `cardId` menggunakan `cardIdProp.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
            // TryReadIngredientPurchaseDetailed.
            cardId = cardIdProp.GetString() ?? string.Empty;
            // Memperbarui `ingredientName` menggunakan `nameProp.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
            // TryReadIngredientPurchaseDetailed.
            ingredientName = nameProp.GetString() ?? string.Empty;
            // Memperbarui `amount` menggunakan memanggil `amountProp.GetInt32` dengan tanpa argumen dalam TryReadIngredientPurchaseDetailed.
            amount = amountProp.GetInt32();
            // Mengembalikan kebalikan kondisi `string.IsNullOrWhiteSpace(cardId)` kepada pemanggil dalam TryReadIngredientPurchaseDetailed; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return !string.IsNullOrWhiteSpace(cardId);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadIngredientPurchaseDetailed.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadIngredientPurchaseDetailed.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadIngredientPurchaseDetailed.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadIngredientPurchaseDetailed; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadIngredientPurchaseDetailed.
        }
    // Menutup scope metode TryReadIngredientPurchaseDetailed; bagian berikut berada di luar batas blok tersebut dalam
    // TryReadIngredientPurchaseDetailed.
    }

    /// <summary>
    /// Mem-parsing payload setoran tabungan: goal_id dan amount.
    /// </summary>
    // Mendefinisikan metode `TryReadSavingDeposit` dengan hasil bertipe `bool`. Mem-parsing payload setoran tabungan: goal_id dan amount. Masukan:
    // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `goalId` bertipe `string` membawa nilai target identitas; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi
    // yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadSavingDeposit(string payloadJson, out string goalId, out int amount)
    // Membuka scope metode TryReadSavingDeposit; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadSavingDeposit.
    {
        // Memperbarui `goalId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadSavingDeposit.
        goalId = string.Empty;
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadSavingDeposit.
        amount = 0;

        // Memulai blok try dalam TryReadSavingDeposit; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadSavingDeposit.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Menyiapkan variabel lokal `root` untuk nilai root dengan `doc.RootElement` (nilai root element). Tipe variabel disimpulkan dari ekspresi nilai
            // awal.
            var root = doc.RootElement;
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!root.TryGetProperty(”goal_id”, out var goalProp)` dan
            // `!root.TryGetProperty(”amount”, out var amountProp)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi
            // ini bernilai benar dalam TryReadSavingDeposit.
            if (!root.TryGetProperty("goal_id", out var goalProp) ||
                // Menggunakan kebalikan kondisi `root.TryGetProperty(”amount”, out var amountProp)` sebagai bagian ekspresi yang sedang disusun dalam
                // TryReadSavingDeposit.
                !root.TryGetProperty("amount", out var amountProp))
            // Membuka scope cabang if untuk kondisi `!root.TryGetProperty(”goal_id”, out var goalProp) || !root.TryGetProperty(”amount”, out var amountProp)`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadSavingDeposit.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadSavingDeposit; eksekusi jalur ini selesai setelah nilai
                // hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!root.TryGetProperty(”goal_id”, out var goalProp) || !root.TryGetProperty(”amount”, out var amountProp)`;
            // bagian berikut berada di luar batas blok tersebut dalam TryReadSavingDeposit.
            }

            // Memperbarui `goalId` menggunakan `goalProp.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
            // TryReadSavingDeposit.
            goalId = goalProp.GetString() ?? string.Empty;
            // Memperbarui `amount` menggunakan memanggil `amountProp.GetInt32` dengan tanpa argumen dalam TryReadSavingDeposit.
            amount = amountProp.GetInt32();
            // Mengembalikan kebalikan kondisi `string.IsNullOrWhiteSpace(goalId)` kepada pemanggil dalam TryReadSavingDeposit; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return !string.IsNullOrWhiteSpace(goalId);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadSavingDeposit.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadSavingDeposit.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadSavingDeposit.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadSavingDeposit; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadSavingDeposit.
        }
    // Menutup scope metode TryReadSavingDeposit; bagian berikut berada di luar batas blok tersebut dalam TryReadSavingDeposit.
    }

    /// <summary>
    /// Mem-parsing payload pembelian bahan baku ringkas: card_id dan amount.
    /// </summary>
    // Mendefinisikan metode `TryReadIngredientPurchase` dengan hasil bertipe `bool`. Mem-parsing payload pembelian bahan baku ringkas: card_id dan
    // amount. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `cardId` bertipe `string` membawa nilai kartu
    // identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau
    // nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadIngredientPurchase(string payloadJson, out string cardId, out int amount)
    // Membuka scope metode TryReadIngredientPurchase; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadIngredientPurchase.
    {
        // Memperbarui `cardId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadIngredientPurchase.
        cardId = string.Empty;
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadIngredientPurchase.
        amount = 0;
        // Memulai blok try dalam TryReadIngredientPurchase; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan
        // saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadIngredientPurchase.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!doc.RootElement.TryGetProperty(”card_id”, out var cardIdProp)` dan
            // `!doc.RootElement.TryGetProperty(”amount”, out var amountProp)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika
            // kondisi ini bernilai benar dalam TryReadIngredientPurchase.
            if (!doc.RootElement.TryGetProperty("card_id", out var cardIdProp) ||
                // Menggunakan kebalikan kondisi `doc.RootElement.TryGetProperty(”amount”, out var amountProp)` sebagai bagian ekspresi yang sedang disusun dalam
                // TryReadIngredientPurchase.
                !doc.RootElement.TryGetProperty("amount", out var amountProp))
            // Membuka scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”card_id”, out var cardIdProp) ||
            // !doc.RootElement.TryGetProperty(”amount”, out var amountProp)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // TryReadIngredientPurchase.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadIngredientPurchase; eksekusi jalur ini selesai setelah
                // nilai hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”card_id”, out var cardIdProp) ||
            // !doc.RootElement.TryGetProperty(”amount”, out var amountProp)`; bagian berikut berada di luar batas blok tersebut dalam
            // TryReadIngredientPurchase.
            }

            // Memperbarui `cardId` menggunakan `cardIdProp.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
            // TryReadIngredientPurchase.
            cardId = cardIdProp.GetString() ?? string.Empty;
            // Memperbarui `amount` menggunakan memanggil `amountProp.GetInt32` dengan tanpa argumen dalam TryReadIngredientPurchase.
            amount = amountProp.GetInt32();
            // Mengembalikan kebalikan kondisi `string.IsNullOrWhiteSpace(cardId)` kepada pemanggil dalam TryReadIngredientPurchase; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return !string.IsNullOrWhiteSpace(cardId);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadIngredientPurchase.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadIngredientPurchase.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadIngredientPurchase.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadIngredientPurchase; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadIngredientPurchase.
        }
    // Menutup scope metode TryReadIngredientPurchase; bagian berikut berada di luar batas blok tersebut dalam TryReadIngredientPurchase.
    }

    /// <summary>
    /// Mem-parsing payload pembelian kebutuhan: amount, card_id opsional, dan points opsional.
    /// </summary>
    // Mendefinisikan metode `TryReadNeedPurchase` dengan hasil bertipe `bool`. Mem-parsing payload pembelian kebutuhan: amount, card_id opsional, dan
    // points opsional. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `amount` bertipe `int` membawa nominal
    // uang atau nilai transaksi yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `cardId`
    // bertipe `string` membawa nilai kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `points` bertipe
    // `int` membawa nilai poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadNeedPurchase(string payloadJson, out int amount, out string cardId, out int points)
    // Membuka scope metode TryReadNeedPurchase; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadNeedPurchase.
    {
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadNeedPurchase.
        amount = 0;
        // Memperbarui `cardId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadNeedPurchase.
        cardId = string.Empty;
        // Memperbarui `points` menggunakan nilai literal `0` dalam TryReadNeedPurchase.
        points = 0;
        // Memulai blok try dalam TryReadNeedPurchase; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadNeedPurchase.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Memeriksa kebalikan kondisi `doc.RootElement.TryGetProperty(”amount”, out var amountProp)`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam TryReadNeedPurchase.
            if (!doc.RootElement.TryGetProperty("amount", out var amountProp))
            // Membuka scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”amount”, out var amountProp)`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam TryReadNeedPurchase.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadNeedPurchase; eksekusi jalur ini selesai setelah nilai
                // hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”amount”, out var amountProp)`; bagian berikut berada di luar batas blok
            // tersebut dalam TryReadNeedPurchase.
            }

            // Memperbarui `amount` menggunakan memanggil `amountProp.GetInt32` dengan tanpa argumen dalam TryReadNeedPurchase.
            amount = amountProp.GetInt32();
            // Memeriksa mencari properti JSON `”card_id”`, `var cardIdProp` pada `doc.RootElement` tanpa menganggap propertinya selalu tersedia; blok if hanya
            // dijalankan ketika kondisi ini bernilai benar dalam TryReadNeedPurchase.
            if (doc.RootElement.TryGetProperty("card_id", out var cardIdProp))
            // Membuka scope cabang if untuk kondisi `doc.RootElement.TryGetProperty(”card_id”, out var cardIdProp)`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam TryReadNeedPurchase.
            {
                // Memperbarui `cardId` menggunakan `cardIdProp.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
                // TryReadNeedPurchase.
                cardId = cardIdProp.GetString() ?? string.Empty;
            // Menutup scope cabang if untuk kondisi `doc.RootElement.TryGetProperty(”card_id”, out var cardIdProp)`; bagian berikut berada di luar batas blok
            // tersebut dalam TryReadNeedPurchase.
            }

            // Memeriksa mencari properti JSON `”points”`, `var pointsProp` pada `doc.RootElement` tanpa menganggap propertinya selalu tersedia; blok if hanya
            // dijalankan ketika kondisi ini bernilai benar dalam TryReadNeedPurchase.
            if (doc.RootElement.TryGetProperty("points", out var pointsProp))
            // Membuka scope cabang if untuk kondisi `doc.RootElement.TryGetProperty(”points”, out var pointsProp)`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam TryReadNeedPurchase.
            {
                // Memperbarui `points` menggunakan memanggil `pointsProp.GetInt32` dengan tanpa argumen dalam TryReadNeedPurchase.
                points = pointsProp.GetInt32();
            // Menutup scope cabang if untuk kondisi `doc.RootElement.TryGetProperty(”points”, out var pointsProp)`; bagian berikut berada di luar batas blok
            // tersebut dalam TryReadNeedPurchase.
            }

            // Mengembalikan pemeriksaan lebih besar antara `amount` dan `0` kepada pemanggil dalam TryReadNeedPurchase; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return amount > 0;
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadNeedPurchase.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadNeedPurchase.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadNeedPurchase.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadNeedPurchase; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadNeedPurchase.
        }
    // Menutup scope metode TryReadNeedPurchase; bagian berikut berada di luar batas blok tersebut dalam TryReadNeedPurchase.
    }

    /// <summary>
    /// Mem-parsing payload penugasan misi: mission_id, target kartu tersier, penalti, dan flag kebutuhan primer/sekunder.
    /// </summary>
    // Mendefinisikan metode `TryReadMissionAssigned` dengan hasil bertipe `bool`. Mem-parsing payload penugasan misi: mission_id, target kartu tersier,
    // penalti, dan flag kebutuhan primer/sekunder. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `missionId`
    // bertipe `string` membawa identitas misi koleksi yang ditugaskan; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter
    // `targetTertiaryCardId` bertipe `string` membawa nilai target tertiary kartu identitas; out mengembalikan nilai melalui parameter dan harus diisi
    // oleh metode; Parameter `penaltyPoints` bertipe `int` membawa nilai penalti poin; out mengembalikan nilai melalui parameter dan harus diisi oleh
    // metode; Parameter `requirePrimary` bertipe `bool` membawa nilai require primary; out mengembalikan nilai melalui parameter dan harus diisi oleh
    // metode; Parameter `requireSecondary` bertipe `bool` membawa nilai require secondary; out mengembalikan nilai melalui parameter dan harus diisi
    // oleh metode.
    public bool TryReadMissionAssigned(
        // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON.
        string payloadJson,
        // Parameter `missionId` bertipe `string` membawa identitas misi koleksi yang ditugaskan; out mengembalikan nilai melalui parameter dan harus diisi
        // oleh metode.
        out string missionId,
        // Parameter `targetTertiaryCardId` bertipe `string` membawa nilai target tertiary kartu identitas; out mengembalikan nilai melalui parameter dan
        // harus diisi oleh metode.
        out string targetTertiaryCardId,
        // Parameter `penaltyPoints` bertipe `int` membawa nilai penalti poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int penaltyPoints,
        // Parameter `requirePrimary` bertipe `bool` membawa nilai require primary; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out bool requirePrimary,
        // Parameter `requireSecondary` bertipe `bool` membawa nilai require secondary; out mengembalikan nilai melalui parameter dan harus diisi oleh
        // metode.
        out bool requireSecondary)
    // Membuka scope metode TryReadMissionAssigned; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadMissionAssigned.
    {
        // Memperbarui `missionId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadMissionAssigned.
        missionId = string.Empty;
        // Memperbarui `targetTertiaryCardId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadMissionAssigned.
        targetTertiaryCardId = string.Empty;
        // Memperbarui `penaltyPoints` menggunakan nilai literal `0` dalam TryReadMissionAssigned.
        penaltyPoints = 0;
        // Memperbarui `requirePrimary` menggunakan true, yaitu kondisi aktif/terpenuhi dalam TryReadMissionAssigned.
        requirePrimary = true;
        // Memperbarui `requireSecondary` menggunakan true, yaitu kondisi aktif/terpenuhi dalam TryReadMissionAssigned.
        requireSecondary = true;

        // Memulai blok try dalam TryReadMissionAssigned; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan
        // saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadMissionAssigned.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Menyiapkan variabel lokal `root` untuk nilai root dengan `doc.RootElement` (nilai root element). Tipe variabel disimpulkan dari ekspresi nilai
            // awal.
            var root = doc.RootElement;
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!root.TryGetProperty(”mission_id”, out var missionIdProp) ||
            // !root.TryGetProperty(”target_tertiary_card_id”, out var targetProp)` dan `!root.TryGetProperty(”penalty_points”, out var penaltyProp)`; sisi
            // kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadMissionAssigned.
            if (!root.TryGetProperty("mission_id", out var missionIdProp) ||
                // Menggunakan kebalikan kondisi `root.TryGetProperty(”target_tertiary_card_id”, out var targetProp)` sebagai bagian ekspresi yang sedang disusun
                // dalam TryReadMissionAssigned.
                !root.TryGetProperty("target_tertiary_card_id", out var targetProp) ||
                // Menggunakan kebalikan kondisi `root.TryGetProperty(”penalty_points”, out var penaltyProp)` sebagai bagian ekspresi yang sedang disusun dalam
                // TryReadMissionAssigned.
                !root.TryGetProperty("penalty_points", out var penaltyProp))
            // Membuka scope cabang if untuk kondisi `!root.TryGetProperty(”mission_id”, out var missionIdProp) ||
            // !root.TryGetProperty(”target_tertiary_card_id”, out var targetProp) || !root.TryGetProperty(”penalty_points”, out ...`; pernyataan/deklarasi
            // berikut berada di dalam batas blok ini dalam TryReadMissionAssigned.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadMissionAssigned; eksekusi jalur ini selesai setelah
                // nilai hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!root.TryGetProperty(”mission_id”, out var missionIdProp) ||
            // !root.TryGetProperty(”target_tertiary_card_id”, out var targetProp) || !root.TryGetProperty(”penalty_points”, out ...`; bagian berikut berada di
            // luar batas blok tersebut dalam TryReadMissionAssigned.
            }

            // Memperbarui `missionId` menggunakan `missionIdProp.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
            // TryReadMissionAssigned.
            missionId = missionIdProp.GetString() ?? string.Empty;
            // Memperbarui `targetTertiaryCardId` menggunakan `targetProp.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti
            // dalam TryReadMissionAssigned.
            targetTertiaryCardId = targetProp.GetString() ?? string.Empty;
            // Memperbarui `penaltyPoints` menggunakan memanggil `penaltyProp.GetInt32` dengan tanpa argumen dalam TryReadMissionAssigned.
            penaltyPoints = penaltyProp.GetInt32();

            // Memeriksa mencari properti JSON `”require_primary”`, `var requirePrimaryProp` pada `root` tanpa menganggap propertinya selalu tersedia; blok if
            // hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadMissionAssigned.
            if (root.TryGetProperty("require_primary", out var requirePrimaryProp))
            // Membuka scope cabang if untuk kondisi `root.TryGetProperty(”require_primary”, out var requirePrimaryProp)`; pernyataan/deklarasi berikut berada
            // di dalam batas blok ini dalam TryReadMissionAssigned.
            {
                // Memperbarui `requirePrimary` menggunakan memanggil `requirePrimaryProp.GetBoolean` dengan tanpa argumen dalam TryReadMissionAssigned.
                requirePrimary = requirePrimaryProp.GetBoolean();
            // Menutup scope cabang if untuk kondisi `root.TryGetProperty(”require_primary”, out var requirePrimaryProp)`; bagian berikut berada di luar batas
            // blok tersebut dalam TryReadMissionAssigned.
            }

            // Memeriksa mencari properti JSON `”require_secondary”`, `var requireSecondaryProp` pada `root` tanpa menganggap propertinya selalu tersedia; blok
            // if hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadMissionAssigned.
            if (root.TryGetProperty("require_secondary", out var requireSecondaryProp))
            // Membuka scope cabang if untuk kondisi `root.TryGetProperty(”require_secondary”, out var requireSecondaryProp)`; pernyataan/deklarasi berikut
            // berada di dalam batas blok ini dalam TryReadMissionAssigned.
            {
                // Memperbarui `requireSecondary` menggunakan memanggil `requireSecondaryProp.GetBoolean` dengan tanpa argumen dalam TryReadMissionAssigned.
                requireSecondary = requireSecondaryProp.GetBoolean();
            // Menutup scope cabang if untuk kondisi `root.TryGetProperty(”require_secondary”, out var requireSecondaryProp)`; bagian berikut berada di luar
            // batas blok tersebut dalam TryReadMissionAssigned.
            }

            // Mengembalikan kebalikan kondisi `string.IsNullOrWhiteSpace(missionId)` kepada pemanggil dalam TryReadMissionAssigned; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return !string.IsNullOrWhiteSpace(missionId);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadMissionAssigned.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadMissionAssigned.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadMissionAssigned.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadMissionAssigned; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadMissionAssigned.
        }
    // Menutup scope metode TryReadMissionAssigned; bagian berikut berada di luar batas blok tersebut dalam TryReadMissionAssigned.
    }

    /// <summary>
    /// Mem-parsing payload tie breaker: nomor undian.
    /// </summary>
    // Mendefinisikan metode `TryReadTieBreaker` dengan hasil bertipe `bool`. Mem-parsing payload tie breaker: nomor undian. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `number` bertipe `int` membawa nilai number; out mengembalikan nilai melalui
    // parameter dan harus diisi oleh metode.
    public bool TryReadTieBreaker(string payloadJson, out int number)
    // Membuka scope metode TryReadTieBreaker; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadTieBreaker.
    {
        // Memperbarui `number` menggunakan nilai literal `0` dalam TryReadTieBreaker.
        number = 0;
        // Memulai blok try dalam TryReadTieBreaker; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadTieBreaker.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Memeriksa kebalikan kondisi `doc.RootElement.TryGetProperty(”number”, out var numberProp)`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam TryReadTieBreaker.
            if (!doc.RootElement.TryGetProperty("number", out var numberProp))
            // Membuka scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”number”, out var numberProp)`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam TryReadTieBreaker.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadTieBreaker; eksekusi jalur ini selesai setelah nilai
                // hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”number”, out var numberProp)`; bagian berikut berada di luar batas blok
            // tersebut dalam TryReadTieBreaker.
            }

            // Memperbarui `number` menggunakan memanggil `numberProp.GetInt32` dengan tanpa argumen dalam TryReadTieBreaker.
            number = numberProp.GetInt32();
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryReadTieBreaker; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return true;
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadTieBreaker.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadTieBreaker.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadTieBreaker.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadTieBreaker; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadTieBreaker.
        }
    // Menutup scope metode TryReadTieBreaker; bagian berikut berada di luar batas blok tersebut dalam TryReadTieBreaker.
    }

    /// <summary>
    /// Mem-parsing payload penghargaan peringkat: rank dan points.
    /// </summary>
    // Mendefinisikan metode `TryReadRankAwarded` dengan hasil bertipe `bool`. Mem-parsing payload penghargaan peringkat: rank dan points. Masukan:
    // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `rank` bertipe `int` membawa nilai rank; out mengembalikan nilai
    // melalui parameter dan harus diisi oleh metode; Parameter `points` bertipe `int` membawa nilai poin; out mengembalikan nilai melalui parameter dan
    // harus diisi oleh metode.
    public bool TryReadRankAwarded(string payloadJson, out int rank, out int points)
    // Membuka scope metode TryReadRankAwarded; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadRankAwarded.
    {
        // Memperbarui `rank` menggunakan nilai literal `0` dalam TryReadRankAwarded.
        rank = 0;
        // Memperbarui `points` menggunakan nilai literal `0` dalam TryReadRankAwarded.
        points = 0;
        // Memulai blok try dalam TryReadRankAwarded; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadRankAwarded.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!doc.RootElement.TryGetProperty(”rank”, out var rankProp)` dan
            // `!doc.RootElement.TryGetProperty(”points”, out var pointsProp)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika
            // kondisi ini bernilai benar dalam TryReadRankAwarded.
            if (!doc.RootElement.TryGetProperty("rank", out var rankProp) ||
                // Menggunakan kebalikan kondisi `doc.RootElement.TryGetProperty(”points”, out var pointsProp)` sebagai bagian ekspresi yang sedang disusun dalam
                // TryReadRankAwarded.
                !doc.RootElement.TryGetProperty("points", out var pointsProp))
            // Membuka scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”rank”, out var rankProp) || !doc.RootElement.TryGetProperty(”points”, out
            // var pointsProp)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadRankAwarded.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadRankAwarded; eksekusi jalur ini selesai setelah nilai
                // hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”rank”, out var rankProp) || !doc.RootElement.TryGetProperty(”points”, out
            // var pointsProp)`; bagian berikut berada di luar batas blok tersebut dalam TryReadRankAwarded.
            }

            // Memperbarui `rank` menggunakan memanggil `rankProp.GetInt32` dengan tanpa argumen dalam TryReadRankAwarded.
            rank = rankProp.GetInt32();
            // Memperbarui `points` menggunakan memanggil `pointsProp.GetInt32` dengan tanpa argumen dalam TryReadRankAwarded.
            points = pointsProp.GetInt32();
            // Mengembalikan pemeriksaan lebih besar antara `rank` dan `0` kepada pemanggil dalam TryReadRankAwarded; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return rank > 0;
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadRankAwarded.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadRankAwarded.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadRankAwarded.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadRankAwarded; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadRankAwarded.
        }
    // Menutup scope metode TryReadRankAwarded; bagian berikut berada di luar batas blok tersebut dalam TryReadRankAwarded.
    }

    /// <summary>
    /// Mem-parsing payload pemberian poin: jumlah points.
    /// </summary>
    // Mendefinisikan metode `TryReadPointsAwarded` dengan hasil bertipe `bool`. Mem-parsing payload pemberian poin: jumlah points. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `points` bertipe `int` membawa nilai poin; out mengembalikan nilai melalui
    // parameter dan harus diisi oleh metode.
    public bool TryReadPointsAwarded(string payloadJson, out int points)
    // Membuka scope metode TryReadPointsAwarded; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadPointsAwarded.
    {
        // Memperbarui `points` menggunakan nilai literal `0` dalam TryReadPointsAwarded.
        points = 0;
        // Memulai blok try dalam TryReadPointsAwarded; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadPointsAwarded.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Memeriksa kebalikan kondisi `doc.RootElement.TryGetProperty(”points”, out var pointsProp)`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam TryReadPointsAwarded.
            if (!doc.RootElement.TryGetProperty("points", out var pointsProp))
            // Membuka scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”points”, out var pointsProp)`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam TryReadPointsAwarded.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadPointsAwarded; eksekusi jalur ini selesai setelah nilai
                // hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”points”, out var pointsProp)`; bagian berikut berada di luar batas blok
            // tersebut dalam TryReadPointsAwarded.
            }

            // Memperbarui `points` menggunakan memanggil `pointsProp.GetInt32` dengan tanpa argumen dalam TryReadPointsAwarded.
            points = pointsProp.GetInt32();
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryReadPointsAwarded; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return true;
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadPointsAwarded.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadPointsAwarded.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadPointsAwarded.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadPointsAwarded; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadPointsAwarded.
        }
    // Menutup scope metode TryReadPointsAwarded; bagian berikut berada di luar batas blok tersebut dalam TryReadPointsAwarded.
    }

    /// <summary>
    /// Mem-parsing payload pencapaian target tabungan detail: goal_id, points, cost.
    /// </summary>
    // Mendefinisikan metode `TryReadSavingGoalAchievedDetailed` dengan hasil bertipe `bool`. Mem-parsing payload pencapaian target tabungan detail:
    // goal_id, points, cost. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `goalId` bertipe `string` membawa
    // nilai target identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `points` bertipe `int` membawa nilai
    // poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `cost` bertipe `int` membawa nilai biaya; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadSavingGoalAchievedDetailed(
        // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON.
        string payloadJson,
        // Parameter `goalId` bertipe `string` membawa nilai target identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string goalId,
        // Parameter `points` bertipe `int` membawa nilai poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int points,
        // Parameter `cost` bertipe `int` membawa nilai biaya; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out int cost)
    // Membuka scope metode TryReadSavingGoalAchievedDetailed; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryReadSavingGoalAchievedDetailed.
    {
        // Memperbarui `goalId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadSavingGoalAchievedDetailed.
        goalId = string.Empty;
        // Memperbarui `points` menggunakan nilai literal `0` dalam TryReadSavingGoalAchievedDetailed.
        points = 0;
        // Memperbarui `cost` menggunakan nilai literal `0` dalam TryReadSavingGoalAchievedDetailed.
        cost = 0;
        // Memulai blok try dalam TryReadSavingGoalAchievedDetailed; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap
        // dijalankan saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadSavingGoalAchievedDetailed.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Memeriksa kebalikan kondisi `doc.RootElement.TryGetProperty(”goal_id”, out var goalProp)`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam TryReadSavingGoalAchievedDetailed.
            if (!doc.RootElement.TryGetProperty("goal_id", out var goalProp))
            // Membuka scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”goal_id”, out var goalProp)`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam TryReadSavingGoalAchievedDetailed.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadSavingGoalAchievedDetailed; eksekusi jalur ini selesai
                // setelah nilai hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”goal_id”, out var goalProp)`; bagian berikut berada di luar batas blok
            // tersebut dalam TryReadSavingGoalAchievedDetailed.
            }

            // Memperbarui `goalId` menggunakan `goalProp.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
            // TryReadSavingGoalAchievedDetailed.
            goalId = goalProp.GetString() ?? string.Empty;

            // Memeriksa mencari properti JSON `”points”`, `var pointsProp` pada `doc.RootElement` tanpa menganggap propertinya selalu tersedia; blok if hanya
            // dijalankan ketika kondisi ini bernilai benar dalam TryReadSavingGoalAchievedDetailed.
            if (doc.RootElement.TryGetProperty("points", out var pointsProp))
            // Membuka scope cabang if untuk kondisi `doc.RootElement.TryGetProperty(”points”, out var pointsProp)`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam TryReadSavingGoalAchievedDetailed.
            {
                // Memperbarui `points` menggunakan memanggil `pointsProp.GetInt32` dengan tanpa argumen dalam TryReadSavingGoalAchievedDetailed.
                points = pointsProp.GetInt32();
            // Menutup scope cabang if untuk kondisi `doc.RootElement.TryGetProperty(”points”, out var pointsProp)`; bagian berikut berada di luar batas blok
            // tersebut dalam TryReadSavingGoalAchievedDetailed.
            }

            // Memeriksa mencari properti JSON `”cost”`, `var costProp` pada `doc.RootElement` tanpa menganggap propertinya selalu tersedia; blok if hanya
            // dijalankan ketika kondisi ini bernilai benar dalam TryReadSavingGoalAchievedDetailed.
            if (doc.RootElement.TryGetProperty("cost", out var costProp))
            // Membuka scope cabang if untuk kondisi `doc.RootElement.TryGetProperty(”cost”, out var costProp)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam TryReadSavingGoalAchievedDetailed.
            {
                // Memperbarui `cost` menggunakan memanggil `costProp.GetInt32` dengan tanpa argumen dalam TryReadSavingGoalAchievedDetailed.
                cost = costProp.GetInt32();
            // Menutup scope cabang if untuk kondisi `doc.RootElement.TryGetProperty(”cost”, out var costProp)`; bagian berikut berada di luar batas blok
            // tersebut dalam TryReadSavingGoalAchievedDetailed.
            }

            // Mengembalikan kebalikan kondisi `string.IsNullOrWhiteSpace(goalId)` kepada pemanggil dalam TryReadSavingGoalAchievedDetailed; eksekusi jalur ini
            // selesai setelah nilai hasil ditentukan.
            return !string.IsNullOrWhiteSpace(goalId);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadSavingGoalAchievedDetailed.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadSavingGoalAchievedDetailed.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadSavingGoalAchievedDetailed.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadSavingGoalAchievedDetailed; eksekusi jalur ini selesai
            // setelah nilai hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadSavingGoalAchievedDetailed.
        }
    // Menutup scope metode TryReadSavingGoalAchievedDetailed; bagian berikut berada di luar batas blok tersebut dalam
    // TryReadSavingGoalAchievedDetailed.
    }

    /// <summary>
    /// Mem-parsing payload pencapaian target tabungan ringkas: points.
    /// </summary>
    // Mendefinisikan metode `TryReadSavingGoalAchieved` dengan hasil bertipe `bool`. Mem-parsing payload pencapaian target tabungan ringkas: points.
    // Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `points` bertipe `int` membawa nilai poin; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadSavingGoalAchieved(string payloadJson, out int points)
    // Membuka scope metode TryReadSavingGoalAchieved; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadSavingGoalAchieved.
    {
        // Memperbarui `points` menggunakan nilai literal `0` dalam TryReadSavingGoalAchieved.
        points = 0;
        // Memulai blok try dalam TryReadSavingGoalAchieved; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan
        // saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadSavingGoalAchieved.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Memeriksa kebalikan kondisi `doc.RootElement.TryGetProperty(”points”, out var pointsProp)`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam TryReadSavingGoalAchieved.
            if (!doc.RootElement.TryGetProperty("points", out var pointsProp))
            // Membuka scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”points”, out var pointsProp)`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam TryReadSavingGoalAchieved.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadSavingGoalAchieved; eksekusi jalur ini selesai setelah
                // nilai hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”points”, out var pointsProp)`; bagian berikut berada di luar batas blok
            // tersebut dalam TryReadSavingGoalAchieved.
            }

            // Memperbarui `points` menggunakan memanggil `pointsProp.GetInt32` dengan tanpa argumen dalam TryReadSavingGoalAchieved.
            points = pointsProp.GetInt32();
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryReadSavingGoalAchieved; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return true;
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadSavingGoalAchieved.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadSavingGoalAchieved.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadSavingGoalAchieved.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadSavingGoalAchieved; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadSavingGoalAchieved.
        }
    // Menutup scope metode TryReadSavingGoalAchieved; bagian berikut berada di luar batas blok tersebut dalam TryReadSavingGoalAchieved.
    }

    /// <summary>
    /// Mem-parsing payload pengambilan pinjaman: loan_id, principal, penalty_points.
    /// </summary>
    // Mendefinisikan metode `TryReadLoanTaken` dengan hasil bertipe `bool`. Mem-parsing payload pengambilan pinjaman: loan_id, principal,
    // penalty_points. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `loanId` bertipe `string` membawa nilai
    // pinjaman identitas; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `principal` bertipe `int` membawa nilai
    // principal; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `penaltyPoints` bertipe `int` membawa nilai penalti
    // poin; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadLoanTaken(string payloadJson, out string loanId, out int principal, out int penaltyPoints)
    // Membuka scope metode TryReadLoanTaken; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadLoanTaken.
    {
        // Memperbarui `loanId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadLoanTaken.
        loanId = string.Empty;
        // Memperbarui `principal` menggunakan nilai literal `0` dalam TryReadLoanTaken.
        principal = 0;
        // Memperbarui `penaltyPoints` menggunakan nilai literal `0` dalam TryReadLoanTaken.
        penaltyPoints = 0;
        // Memulai blok try dalam TryReadLoanTaken; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadLoanTaken.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!doc.RootElement.TryGetProperty(”loan_id”, out var loanIdProp) ||
            // !doc.RootElement.TryGetProperty(”principal”, out var principalProp)` dan `!doc.RootElement.TryGetProperty(”penalty_points”, out var
            // penaltyProp)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // TryReadLoanTaken.
            if (!doc.RootElement.TryGetProperty("loan_id", out var loanIdProp) ||
                // Menggunakan kebalikan kondisi `doc.RootElement.TryGetProperty(”principal”, out var principalProp)` sebagai bagian ekspresi yang sedang disusun
                // dalam TryReadLoanTaken.
                !doc.RootElement.TryGetProperty("principal", out var principalProp) ||
                // Menggunakan kebalikan kondisi `doc.RootElement.TryGetProperty(”penalty_points”, out var penaltyProp)` sebagai bagian ekspresi yang sedang disusun
                // dalam TryReadLoanTaken.
                !doc.RootElement.TryGetProperty("penalty_points", out var penaltyProp))
            // Membuka scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”loan_id”, out var loanIdProp) ||
            // !doc.RootElement.TryGetProperty(”principal”, out var principalProp) || !doc.RootElement.TryGetProperty(”penal...`; pernyataan/deklarasi berikut
            // berada di dalam batas blok ini dalam TryReadLoanTaken.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadLoanTaken; eksekusi jalur ini selesai setelah nilai
                // hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”loan_id”, out var loanIdProp) ||
            // !doc.RootElement.TryGetProperty(”principal”, out var principalProp) || !doc.RootElement.TryGetProperty(”penal...`; bagian berikut berada di luar
            // batas blok tersebut dalam TryReadLoanTaken.
            }

            // Memperbarui `loanId` menggunakan `loanIdProp.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
            // TryReadLoanTaken.
            loanId = loanIdProp.GetString() ?? string.Empty;
            // Memperbarui `principal` menggunakan memanggil `principalProp.GetInt32` dengan tanpa argumen dalam TryReadLoanTaken.
            principal = principalProp.GetInt32();
            // Memperbarui `penaltyPoints` menggunakan memanggil `penaltyProp.GetInt32` dengan tanpa argumen dalam TryReadLoanTaken.
            penaltyPoints = penaltyProp.GetInt32();
            // Mengembalikan kebalikan kondisi `string.IsNullOrWhiteSpace(loanId)` kepada pemanggil dalam TryReadLoanTaken; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return !string.IsNullOrWhiteSpace(loanId);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadLoanTaken.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadLoanTaken.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadLoanTaken.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadLoanTaken; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadLoanTaken.
        }
    // Menutup scope metode TryReadLoanTaken; bagian berikut berada di luar batas blok tersebut dalam TryReadLoanTaken.
    }

    /// <summary>
    /// Mem-parsing payload pembayaran pinjaman: loan_id dan amount.
    /// </summary>
    // Mendefinisikan metode `TryReadLoanRepay` dengan hasil bertipe `bool`. Mem-parsing payload pembayaran pinjaman: loan_id dan amount. Masukan:
    // Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `loanId` bertipe `string` membawa nilai pinjaman identitas; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi
    // yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadLoanRepay(string payloadJson, out string loanId, out int amount)
    // Membuka scope metode TryReadLoanRepay; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadLoanRepay.
    {
        // Memperbarui `loanId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadLoanRepay.
        loanId = string.Empty;
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadLoanRepay.
        amount = 0;
        // Memulai blok try dalam TryReadLoanRepay; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadLoanRepay.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!doc.RootElement.TryGetProperty(”loan_id”, out var loanIdProp)` dan
            // `!doc.RootElement.TryGetProperty(”amount”, out var amountProp)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika
            // kondisi ini bernilai benar dalam TryReadLoanRepay.
            if (!doc.RootElement.TryGetProperty("loan_id", out var loanIdProp) ||
                // Menggunakan kebalikan kondisi `doc.RootElement.TryGetProperty(”amount”, out var amountProp)` sebagai bagian ekspresi yang sedang disusun dalam
                // TryReadLoanRepay.
                !doc.RootElement.TryGetProperty("amount", out var amountProp))
            // Membuka scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”loan_id”, out var loanIdProp) ||
            // !doc.RootElement.TryGetProperty(”amount”, out var amountProp)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // TryReadLoanRepay.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadLoanRepay; eksekusi jalur ini selesai setelah nilai
                // hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”loan_id”, out var loanIdProp) ||
            // !doc.RootElement.TryGetProperty(”amount”, out var amountProp)`; bagian berikut berada di luar batas blok tersebut dalam TryReadLoanRepay.
            }

            // Memperbarui `loanId` menggunakan `loanIdProp.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
            // TryReadLoanRepay.
            loanId = loanIdProp.GetString() ?? string.Empty;
            // Memperbarui `amount` menggunakan memanggil `amountProp.GetInt32` dengan tanpa argumen dalam TryReadLoanRepay.
            amount = amountProp.GetInt32();
            // Mengembalikan kebalikan kondisi `string.IsNullOrWhiteSpace(loanId)` kepada pemanggil dalam TryReadLoanRepay; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return !string.IsNullOrWhiteSpace(loanId);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadLoanRepay.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadLoanRepay.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadLoanRepay.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadLoanRepay; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadLoanRepay.
        }
    // Menutup scope metode TryReadLoanRepay; bagian berikut berada di luar batas blok tersebut dalam TryReadLoanRepay.
    }

    /// <summary>
    /// Mem-parsing payload klaim pesanan: daftar kartu bahan baku yang diperlukan dan income.
    /// </summary>
    // Mendefinisikan metode `TryReadOrderClaim` dengan hasil bertipe `bool`. Mem-parsing payload klaim pesanan: daftar kartu bahan baku yang diperlukan
    // dan income. Masukan: Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `requiredCards` bertipe `List<string>`
    // membawa nilai required kartu; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `income` bertipe `int` membawa
    // nilai pemasukan; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadOrderClaim(string payloadJson, out List<string> requiredCards, out int income)
    // Membuka scope metode TryReadOrderClaim; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadOrderClaim.
    {
        // Memperbarui `requiredCards` menggunakan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya dalam TryReadOrderClaim.
        requiredCards = new List<string>();
        // Memperbarui `income` menggunakan nilai literal `0` dalam TryReadOrderClaim.
        income = 0;
        // Memulai blok try dalam TryReadOrderClaim; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadOrderClaim.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!doc.RootElement.TryGetProperty(”required_ingredient_card_ids”, out var
            // cardsProp) || cardsProp.ValueKind != JsonValueKind.Array` dan `!doc.RootElement.TryGetProperty(”income”, out var incomeProp)`; sisi kanan
            // diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadOrderClaim.
            if (!doc.RootElement.TryGetProperty("required_ingredient_card_ids", out var cardsProp) ||
                // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `cardsProp.ValueKind` dan `JsonValueKind.Array` dalam TryReadOrderClaim.
                cardsProp.ValueKind != JsonValueKind.Array ||
                // Menggunakan kebalikan kondisi `doc.RootElement.TryGetProperty(”income”, out var incomeProp)` sebagai bagian ekspresi yang sedang disusun dalam
                // TryReadOrderClaim.
                !doc.RootElement.TryGetProperty("income", out var incomeProp))
            // Membuka scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”required_ingredient_card_ids”, out var cardsProp) || cardsProp.ValueKind
            // != JsonValueKind.Array || !doc.RootElement.TryGetProperty(”income”, o...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // TryReadOrderClaim.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadOrderClaim; eksekusi jalur ini selesai setelah nilai
                // hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”required_ingredient_card_ids”, out var cardsProp) || cardsProp.ValueKind
            // != JsonValueKind.Array || !doc.RootElement.TryGetProperty(”income”, o...`; bagian berikut berada di luar batas blok tersebut dalam
            // TryReadOrderClaim.
            }

            // Memperbarui `income` menggunakan memanggil `incomeProp.GetInt32` dengan tanpa argumen dalam TryReadOrderClaim.
            income = incomeProp.GetInt32();
            // Mengulangi setiap elemen `cardsProp.EnumerateArray()`; elemen saat ini disimpan sebagai `item` bertipe `var` untuk diproses oleh badan loop dalam
            // TryReadOrderClaim.
            foreach (var item in cardsProp.EnumerateArray())
            // Membuka scope loop setiap item dari `cardsProp.EnumerateArray()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // TryReadOrderClaim.
            {
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
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadOrderClaim.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadOrderClaim.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadOrderClaim.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadOrderClaim; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadOrderClaim.
        }
    // Menutup scope metode TryReadOrderClaim; bagian berikut berada di luar batas blok tersebut dalam TryReadOrderClaim.
    }

    // Mendefinisikan metode `TryReadSoldNeed` dengan hasil bertipe `bool`; operasi ini menangani try read terjual kebutuhan. Masukan: Parameter
    // `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `cardId` bertipe `string` membawa nilai kartu identitas; out mengembalikan
    // nilai melalui parameter dan harus diisi oleh metode.
    public bool TryReadSoldNeed(string payloadJson, out string cardId)
    // Membuka scope metode TryReadSoldNeed; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadSoldNeed.
    {
        // Memperbarui `cardId` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadSoldNeed.
        cardId = string.Empty;
        // Memeriksa memeriksa apakah `payloadJson` null, kosong, atau hanya berisi karakter spasi; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam TryReadSoldNeed.
        if (string.IsNullOrWhiteSpace(payloadJson))
        // Membuka scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(payloadJson)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryReadSoldNeed.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadSoldNeed; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `string.IsNullOrWhiteSpace(payloadJson)`; bagian berikut berada di luar batas blok tersebut dalam
        // TryReadSoldNeed.
        }

        // Memulai blok try dalam TryReadSoldNeed; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan saat
        // keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadSoldNeed.
        {
            // Menyiapkan variabel lokal `doc` untuk nilai doc dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan dari
            // ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
            using var doc = JsonDocument.Parse(payloadJson);
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!doc.RootElement.TryGetProperty(”option_type”, out var typeProp)` dan
            // `!string.Equals(typeProp.GetString(), ”SELL_NEED”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if
            // hanya dijalankan ketika kondisi ini bernilai benar dalam TryReadSoldNeed.
            if (!doc.RootElement.TryGetProperty("option_type", out var typeProp) ||
                // Menggunakan kebalikan kondisi `string.Equals(typeProp.GetString(), ”SELL_NEED”, StringComparison.OrdinalIgnoreCase)` sebagai bagian ekspresi yang
                // sedang disusun dalam TryReadSoldNeed.
                !string.Equals(typeProp.GetString(), "SELL_NEED", StringComparison.OrdinalIgnoreCase))
            // Membuka scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”option_type”, out var typeProp) || !string.Equals(typeProp.GetString(),
            // ”SELL_NEED”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadSoldNeed.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadSoldNeed; eksekusi jalur ini selesai setelah nilai
                // hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”option_type”, out var typeProp) || !string.Equals(typeProp.GetString(),
            // ”SELL_NEED”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas blok tersebut dalam TryReadSoldNeed.
            }

            // Memeriksa kebalikan kondisi `doc.RootElement.TryGetProperty(”card_id”, out var cardProp)`; blok if hanya dijalankan ketika kondisi ini bernilai
            // benar dalam TryReadSoldNeed.
            if (!doc.RootElement.TryGetProperty("card_id", out var cardProp))
            // Membuka scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”card_id”, out var cardProp)`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam TryReadSoldNeed.
            {
                // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadSoldNeed; eksekusi jalur ini selesai setelah nilai
                // hasil ditentukan.
                return false;
            // Menutup scope cabang if untuk kondisi `!doc.RootElement.TryGetProperty(”card_id”, out var cardProp)`; bagian berikut berada di luar batas blok
            // tersebut dalam TryReadSoldNeed.
            }

            // Memperbarui `cardId` menggunakan `cardProp.GetString()` bila tidak null; jika null gunakan `string.Empty` sebagai nilai pengganti dalam
            // TryReadSoldNeed.
            cardId = cardProp.GetString() ?? string.Empty;
            // Mengembalikan kebalikan kondisi `string.IsNullOrWhiteSpace(cardId)` kepada pemanggil dalam TryReadSoldNeed; eksekusi jalur ini selesai setelah
            // nilai hasil ditentukan.
            return !string.IsNullOrWhiteSpace(cardId);
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam TryReadSoldNeed.
        }
        // Menangani exception `JsonException` melalui variabel dalam TryReadSoldNeed.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadSoldNeed.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryReadSoldNeed; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return false;
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam TryReadSoldNeed.
        }
    // Menutup scope metode TryReadSoldNeed; bagian berikut berada di luar batas blok tersebut dalam TryReadSoldNeed.
    }
// Menutup scope tipe AnalyticsPayloadReader; bagian berikut berada di luar batas blok tersebut.
}
