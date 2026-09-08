// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui RulesetFormHelper.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `System.Text.Json.Nodes` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json.Nodes;
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

// Mendefinisikan tipe class `RulesetFormHelper`.
public static class RulesetFormHelper
// Membuka scope tipe RulesetFormHelper; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `BuildDefaultCreateViewModel` dengan hasil bertipe `CreateRulesetViewModel`; operasi ini menangani build bawaan create view
    // model.
    public static CreateRulesetViewModel BuildDefaultCreateViewModel()
    // Membuka scope metode BuildDefaultCreateViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildDefaultCreateViewModel.
    {
        // Mengembalikan objek baru bertipe `CreateRulesetViewModel` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam
        // BuildDefaultCreateViewModel; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return new CreateRulesetViewModel
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // BuildDefaultCreateViewModel.
        {
            // Memperbarui `IsEditMode` menggunakan false, yaitu kondisi nonaktif/tidak terpenuhi dalam BuildDefaultCreateViewModel.
            IsEditMode = false,
            // Memperbarui `DefinitionJson` menggunakan literal multiline yang dirinci pada komentar di dekat deklarasinya dalam BuildDefaultCreateViewModel.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `DefinitionJson = ”””`.
            // Baris literal 2: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{`.
            // Baris literal 3: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”mode”: ”PEMULA”,`.
            // Baris literal 4: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”actions_per_turn”: 2,`.
            // Baris literal 5: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”starting_cash”: 20,`.
            // Baris literal 6: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”weekday_rules”: {`.
            // Baris literal 7: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”friday”: { ”feature”: ”DONATION”,
            // ”enabled”: true },`.
            // Baris literal 8: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”saturday”: { ”feature”: ”GOLD_TRADE”,
            // ”enabled”: true },`.
            // Baris literal 9: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”sunday”: { ”feature”: ”REST”, ”enabled”:
            // true }`.
            // Baris literal 10: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `},`.
            // Baris literal 11: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”constraints”: {`.
            // Baris literal 12: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”cash_min”: 0,`.
            // Baris literal 13: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”max_ingredient_total”: 6,`.
            // Baris literal 14: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”max_same_ingredient”: 3,`.
            // Baris literal 15: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”primary_need_max_per_day”: null,`.
            // Baris literal 16: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”require_primary_before_others”: true`.
            // Baris literal 17: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `},`.
            // Baris literal 18: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”donation”: { ”min_amount”: 1,
            // ”max_amount”: 999999 },`.
            // Baris literal 19: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”gold_trade”: { ”allow_buy”: true,
            // ”allow_sell”: true },`.
            // Baris literal 20: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”advanced”: {`.
            // Baris literal 21: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”loan”: { ”enabled”: false },`.
            // Baris literal 22: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”insurance”: { ”enabled”: false },`.
            // Baris literal 23: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”saving_goal”: { ”enabled”: false }`.
            // Baris literal 24: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `},`.
            // Baris literal 25: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”freelance”: { ”income”: 1 },`.
            // Baris literal 26: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”scoring”: {`.
            // Baris literal 27: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”donation_rank_points”: [`.
            // Baris literal 28: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{ ”rank”: 1, ”points”: 7 },`.
            // Baris literal 29: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{ ”rank”: 2, ”points”: 5 },`.
            // Baris literal 30: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{ ”rank”: 3, ”points”: 2 }`.
            // Baris literal 31: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `],`.
            // Baris literal 32: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”gold_points_by_qty”: [`.
            // Baris literal 33: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{ ”qty”: 1, ”points”: 3 },`.
            // Baris literal 34: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{ ”qty”: 2, ”points”: 5 },`.
            // Baris literal 35: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{ ”qty”: 3, ”points”: 8 },`.
            // Baris literal 36: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{ ”qty”: 4, ”points”: 12 }`.
            // Baris literal 37: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `],`.
            // Baris literal 38: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `”pension_rank_points”: [`.
            // Baris literal 39: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{ ”rank”: 1, ”points”: 5 },`.
            // Baris literal 40: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{ ”rank”: 2, ”points”: 3 },`.
            // Baris literal 41: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `{ ”rank”: 3, ”points”: 1 }`.
            // Baris literal 42: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `]`.
            // Baris literal 43: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `}`.
            // Baris literal 44: Bagian struktur atau nilai JSON/teks literal yang dipakai persis seperti tertulis: `}`.
            // Baris literal 45: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            DefinitionJson = """
            {
              "mode": "PEMULA",
              "actions_per_turn": 2,
              "starting_cash": 20,
              "weekday_rules": {
                "friday": { "feature": "DONATION", "enabled": true },
                "saturday": { "feature": "GOLD_TRADE", "enabled": true },
                "sunday": { "feature": "REST", "enabled": true }
              },
              "constraints": {
                "cash_min": 0,
                "max_ingredient_total": 6,
                "max_same_ingredient": 3,
                "primary_need_max_per_day": null,
                "require_primary_before_others": true
              },
              "donation": { "min_amount": 1, "max_amount": 999999 },
              "gold_trade": { "allow_buy": true, "allow_sell": true },
              "advanced": {
                "loan": { "enabled": false },
                "insurance": { "enabled": false },
                "saving_goal": { "enabled": false }
              },
              "freelance": { "income": 1 },
              "scoring": {
                "donation_rank_points": [
                  { "rank": 1, "points": 7 },
                  { "rank": 2, "points": 5 },
                  { "rank": 3, "points": 2 }
                ],
                "gold_points_by_qty": [
                  { "qty": 1, "points": 3 },
                  { "qty": 2, "points": 5 },
                  { "qty": 3, "points": 8 },
                  { "qty": 4, "points": 12 }
                ],
                "pension_rank_points": [
                  { "rank": 1, "points": 5 },
                  { "rank": 2, "points": 3 },
                  { "rank": 3, "points": 1 }
                ]
              }
            }
            """
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam BuildDefaultCreateViewModel.
        };
    // Menutup scope metode BuildDefaultCreateViewModel; bagian berikut berada di luar batas blok tersebut dalam BuildDefaultCreateViewModel.
    }

    // Mendefinisikan metode `TryResolveMode` dengan hasil bertipe `bool`; operasi ini menangani try resolve mode. Masukan: Parameter `configObject`
    // bertipe `JsonObject` membawa nilai konfigurasi object; Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan
    // yang digunakan; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    public static bool TryResolveMode(JsonObject configObject, out string mode)
    // Membuka scope metode TryResolveMode; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryResolveMode.
    {
        // Memperbarui `mode` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryResolveMode.
        mode = string.Empty;
        // Memeriksa kebalikan kondisi `configObject.TryGetPropertyValue(”mode”, out var modeNode)`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam TryResolveMode.
        if (!configObject.TryGetPropertyValue("mode", out var modeNode))
        // Membuka scope cabang if untuk kondisi `!configObject.TryGetPropertyValue(”mode”, out var modeNode)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam TryResolveMode.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryResolveMode; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!configObject.TryGetPropertyValue(”mode”, out var modeNode)`; bagian berikut berada di luar batas blok
        // tersebut dalam TryResolveMode.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `modeNode is not JsonValue modeValue` dan
        // `!modeValue.TryGetValue<string>(out var rawMode)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryResolveMode.
        if (modeNode is not JsonValue modeValue || !modeValue.TryGetValue<string>(out var rawMode))
        // Membuka scope cabang if untuk kondisi `modeNode is not JsonValue modeValue || !modeValue.TryGetValue<string>(out var rawMode)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryResolveMode.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryResolveMode; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `modeNode is not JsonValue modeValue || !modeValue.TryGetValue<string>(out var rawMode)`; bagian berikut
        // berada di luar batas blok tersebut dalam TryResolveMode.
        }

        // Menyiapkan variabel lokal `modeText` untuk nilai mode text dengan `rawMode?.Trim().ToUpperInvariant()`; akses setelah ?. hanya dilakukan bila
        // penerimanya tidak null. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var modeText = rawMode?.Trim().ToUpperInvariant();
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!string.Equals(modeText, ”PEMULA”, StringComparison.Ordinal)` dan
        // `!string.Equals(modeText, ”MAHIR”, StringComparison.Ordinal)`; sisi kanan diperiksa hanya jika sisi kiri benar; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam TryResolveMode.
        if (!string.Equals(modeText, "PEMULA", StringComparison.Ordinal) &&
            // Menggunakan kebalikan kondisi `string.Equals(modeText, ”MAHIR”, StringComparison.Ordinal)` sebagai bagian ekspresi yang sedang disusun dalam
            // TryResolveMode.
            !string.Equals(modeText, "MAHIR", StringComparison.Ordinal))
        // Membuka scope cabang if untuk kondisi `!string.Equals(modeText, ”PEMULA”, StringComparison.Ordinal) && !string.Equals(modeText, ”MAHIR”,
        // StringComparison.Ordinal)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryResolveMode.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryResolveMode; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `!string.Equals(modeText, ”PEMULA”, StringComparison.Ordinal) && !string.Equals(modeText, ”MAHIR”,
        // StringComparison.Ordinal)`; bagian berikut berada di luar batas blok tersebut dalam TryResolveMode.
        }

        // Memperbarui `mode` menggunakan `modeText` dengan penegasan non-null untuk analisis compiler; operator ! tidak menambah pemeriksaan saat runtime
        // dalam TryResolveMode.
        mode = modeText!;
        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryResolveMode; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return true;
    // Menutup scope metode TryResolveMode; bagian berikut berada di luar batas blok tersebut dalam TryResolveMode.
    }

    // Mendefinisikan metode `BuildRulesetApiErrorMessage` dengan hasil bertipe `Task<string>`; operasi ini menangani build aturan api kesalahan pesan.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `response` bertipe
    // `HttpResponseMessage` membawa hasil respons yang akan dibaca atau dikirim kepada pemanggil; Parameter `prefix` bertipe `string` membawa nilai
    // prefix; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
    // permintaan atau aplikasi berhenti.
    public static async Task<string> BuildRulesetApiErrorMessage(HttpResponseMessage response, string prefix, CancellationToken ct)
    // Membuka scope metode BuildRulesetApiErrorMessage; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam BuildRulesetApiErrorMessage.
    {
        // Menyiapkan variabel lokal `error` untuk informasi kesalahan yang dikembalikan atau dicatat ketika operasi tidak berhasil dengan hasil operasi
        // asinkron memanggil `response.Content.TryReadFromJsonAsync<ErrorResponse>` dengan `ct`; await menunggu hasil tanpa memblokir thread selama operasi
        // belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var error = await response.Content.TryReadFromJsonAsync<ErrorResponse>(ct);
        // Mengembalikan `error?.Message` bila tidak null; jika null gunakan `$”{prefix}. Status: {(int)response.StatusCode}”` sebagai nilai pengganti
        // kepada pemanggil dalam BuildRulesetApiErrorMessage; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return error?.Message ?? $"{prefix}. Status: {(int)response.StatusCode}";
    // Menutup scope metode BuildRulesetApiErrorMessage; bagian berikut berada di luar batas blok tersebut dalam BuildRulesetApiErrorMessage.
    }

    // Mendefinisikan metode `SerializeIndentedJson` dengan hasil bertipe `string`; operasi ini menangani serialize indented JSON. Masukan: Parameter
    // `definitionJson` bertipe `JsonElement?` membawa nilai definisi JSON; nilai null diizinkan ketika data opsional belum tersedia.
    public static string SerializeIndentedJson(JsonElement? definitionJson)
    // Membuka scope metode SerializeIndentedJson; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SerializeIndentedJson.
    {
        // Memeriksa kebalikan kondisi `definitionJson.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam SerializeIndentedJson.
        if (!definitionJson.HasValue)
        // Membuka scope cabang if untuk kondisi `!definitionJson.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // SerializeIndentedJson.
        {
            // Mengembalikan nilai literal `”{}”` kepada pemanggil dalam SerializeIndentedJson; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return "{}";
        // Menutup scope cabang if untuk kondisi `!definitionJson.HasValue`; bagian berikut berada di luar batas blok tersebut dalam SerializeIndentedJson.
        }

        // Memulai blok try dalam SerializeIndentedJson; exception dari blok ini dapat dialihkan ke catch, sedangkan finally (jika ada) tetap dijalankan
        // saat keluar.
        try
        // Membuka scope penanganan operasi try; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SerializeIndentedJson.
        {
            // Mengembalikan menserialisasi `definitionJson.Value`, `new JsonSerializerOptions { WriteIndented = true }` menjadi JSON melalui
            // `JsonSerializer.Serialize` kepada pemanggil dalam SerializeIndentedJson; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return JsonSerializer.Serialize(definitionJson.Value, new JsonSerializerOptions
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // SerializeIndentedJson.
            {
                // Memperbarui `WriteIndented` menggunakan true, yaitu kondisi aktif/terpenuhi dalam SerializeIndentedJson.
                WriteIndented = true
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam SerializeIndentedJson.
            });
        // Menutup scope penanganan operasi try; bagian berikut berada di luar batas blok tersebut dalam SerializeIndentedJson.
        }
        // Menangani exception `JsonException` melalui variabel dalam SerializeIndentedJson.
        catch (JsonException)
        // Membuka scope penanganan exception catch; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SerializeIndentedJson.
        {
            // Mengembalikan nilai literal `”{}”` kepada pemanggil dalam SerializeIndentedJson; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return "{}";
        // Menutup scope penanganan exception catch; bagian berikut berada di luar batas blok tersebut dalam SerializeIndentedJson.
        }
    // Menutup scope metode SerializeIndentedJson; bagian berikut berada di luar batas blok tersebut dalam SerializeIndentedJson.
    }
// Menutup scope tipe RulesetFormHelper; bagian berikut berada di luar batas blok tersebut.
}
