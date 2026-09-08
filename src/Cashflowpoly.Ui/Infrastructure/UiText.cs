// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui UiText.
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;
// Mengimpor namespace `System.Collections.Frozen` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Collections.Frozen;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

// Mendefinisikan tipe class `UiText`.
public static class UiText
// Membuka scope tipe UiText; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `FrozenDictionary<string, (string Id, string En)>`: `Lexicon` menyimpan nilai lexicon dengan nilai awal memanggil
    // `UiTextLexicon.Build` dengan tanpa argumen. readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly FrozenDictionary<string, (string Id, string En)> Lexicon = UiTextLexicon.Build();

    // Mendefinisikan metode `NormalizeLanguage` dengan hasil bertipe `string`; operasi ini menangani normalize language. Masukan: Parameter `language`
    // bertipe `string?` membawa nilai language; nilai null diizinkan ketika data opsional belum tersedia.
    public static string NormalizeLanguage(string? language)
    // Membuka scope metode NormalizeLanguage; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam NormalizeLanguage.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `string.Equals(language, AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase)` benar
        // gunakan `AuthConstants.LanguageEn`, jika tidak gunakan `AuthConstants.LanguageId` kepada pemanggil dalam NormalizeLanguage; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return string.Equals(language, AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: AuthConstants.LanguageEn dalam NormalizeLanguage.
            ? AuthConstants.LanguageEn
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: AuthConstants.LanguageId; dalam NormalizeLanguage.
            : AuthConstants.LanguageId;
    // Menutup scope metode NormalizeLanguage; bagian berikut berada di luar batas blok tersebut dalam NormalizeLanguage.
    }

    // Mendefinisikan metode `TranslateSessionStatus` dengan hasil bertipe `string`; operasi ini menangani translate sesi status. Masukan: Parameter
    // `culture` bertipe `string` membawa nilai culture; Parameter `status` bertipe `string?` membawa nilai status; nilai null diizinkan ketika data
    // opsional belum tersedia.
    public static string TranslateSessionStatus(string culture, string? status)
    // Membuka scope metode TranslateSessionStatus; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TranslateSessionStatus.
    {
        // Menyiapkan variabel lokal `normalized` untuk nilai normalized dengan `status?.Trim().ToUpperInvariant()`; akses setelah ?. hanya dilakukan bila
        // penerimanya tidak null. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var normalized = status?.Trim().ToUpperInvariant();
        // Mengembalikan hasil pemetaan `normalized` melalui cabang pola switch yang cocok kepada pemanggil dalam TranslateSessionStatus; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return normalized switch
        // Membuka scope pemetaan switch atas `normalized`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TranslateSessionStatus.
        {
            // Untuk pola `”CREATED”`, menghasilkan memanggil `Translate` dengan `culture`, `”status.session.created”` sebagai hasil switch.
            "CREATED" => Translate(culture, "status.session.created"),
            // Untuk pola `”STARTED”`, menghasilkan memanggil `Translate` dengan `culture`, `”status.MulaiSesi”` sebagai hasil switch.
            "STARTED" => Translate(culture, "status.MulaiSesi"),
            // Untuk pola `”ENDED”`, menghasilkan memanggil `Translate` dengan `culture`, `”status.AkhiriSesi”` sebagai hasil switch.
            "ENDED" => Translate(culture, "status.AkhiriSesi"),
            // Untuk pola `”CANCELLED”`, menghasilkan memanggil `Translate` dengan `culture`, `”status.session.cancelled”` sebagai hasil switch.
            "CANCELLED" => Translate(culture, "status.session.cancelled"),
            // Untuk pola `”CANCELED”`, menghasilkan memanggil `Translate` dengan `culture`, `”status.session.cancelled”` sebagai hasil switch.
            "CANCELED" => Translate(culture, "status.session.cancelled"),
            // Untuk pola `_`, menghasilkan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(status)` benar gunakan `Translate(culture,
            // ”status.session.unknown”)`, jika tidak gunakan `status!` sebagai hasil switch.
            _ => string.IsNullOrWhiteSpace(status) ? Translate(culture, "status.session.unknown") : status!
        // Menutup scope pemetaan switch atas `normalized`; bagian berikut berada di luar batas blok tersebut dalam TranslateSessionStatus.
        };
    // Menutup scope metode TranslateSessionStatus; bagian berikut berada di luar batas blok tersebut dalam TranslateSessionStatus.
    }

    // Mendefinisikan metode `TranslateRulesetStatus` dengan hasil bertipe `string`; operasi ini menangani translate aturan status. Masukan: Parameter
    // `culture` bertipe `string` membawa nilai culture; Parameter `status` bertipe `string?` membawa nilai status; nilai null diizinkan ketika data
    // opsional belum tersedia.
    public static string TranslateRulesetStatus(string culture, string? status)
    // Membuka scope metode TranslateRulesetStatus; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TranslateRulesetStatus.
    {
        // Menyiapkan variabel lokal `normalized` untuk nilai normalized dengan `status?.Trim().ToUpperInvariant()`; akses setelah ?. hanya dilakukan bila
        // penerimanya tidak null. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var normalized = status?.Trim().ToUpperInvariant();
        // Mengembalikan hasil pemetaan `normalized` melalui cabang pola switch yang cocok kepada pemanggil dalam TranslateRulesetStatus; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return normalized switch
        // Membuka scope pemetaan switch atas `normalized`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TranslateRulesetStatus.
        {
            // Untuk pola `”ACTIVE”`, menghasilkan memanggil `Translate` dengan `culture`, `”status.ruleset.active”` sebagai hasil switch.
            "ACTIVE" => Translate(culture, "status.ruleset.active"),
            // Untuk pola `”DRAFT”`, menghasilkan memanggil `Translate` dengan `culture`, `”status.ruleset.draft”` sebagai hasil switch.
            "DRAFT" => Translate(culture, "status.ruleset.draft"),
            // Untuk pola `”ARCHIVED”`, menghasilkan memanggil `Translate` dengan `culture`, `”status.ruleset.retired”` sebagai hasil switch.
            "ARCHIVED" => Translate(culture, "status.ruleset.retired"),
            // Untuk pola `_`, menghasilkan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(status)` benar gunakan `Translate(culture,
            // ”status.ruleset.unknown”)`, jika tidak gunakan `status!` sebagai hasil switch.
            _ => string.IsNullOrWhiteSpace(status) ? Translate(culture, "status.ruleset.unknown") : status!
        // Menutup scope pemetaan switch atas `normalized`; bagian berikut berada di luar batas blok tersebut dalam TranslateRulesetStatus.
        };
    // Menutup scope metode TranslateRulesetStatus; bagian berikut berada di luar batas blok tersebut dalam TranslateRulesetStatus.
    }

    // Mendefinisikan metode `TranslateRulesetMode` dengan hasil bertipe `string`; operasi ini menangani translate aturan mode. Masukan: Parameter
    // `culture` bertipe `string` membawa nilai culture; Parameter `mode` bertipe `string?` membawa mode permainan yang menentukan kelompok aturan yang
    // digunakan; nilai null diizinkan ketika data opsional belum tersedia.
    public static string TranslateRulesetMode(string culture, string? mode)
    // Membuka scope metode TranslateRulesetMode; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TranslateRulesetMode.
    {
        // Menyiapkan variabel lokal `normalized` untuk nilai normalized dengan `mode?.Trim().ToUpperInvariant()`; akses setelah ?. hanya dilakukan bila
        // penerimanya tidak null. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var normalized = mode?.Trim().ToUpperInvariant();
        // Mengembalikan hasil pemetaan `normalized` melalui cabang pola switch yang cocok kepada pemanggil dalam TranslateRulesetMode; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return normalized switch
        // Membuka scope pemetaan switch atas `normalized`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TranslateRulesetMode.
        {
            // Untuk pola `”PEMULA”`, menghasilkan memanggil `Translate` dengan `culture`, `”rulesets.form.mode_beginner”` sebagai hasil switch.
            "PEMULA" => Translate(culture, "rulesets.form.mode_beginner"),
            // Untuk pola `”BEGINNER”`, menghasilkan memanggil `Translate` dengan `culture`, `”rulesets.form.mode_beginner”` sebagai hasil switch.
            "BEGINNER" => Translate(culture, "rulesets.form.mode_beginner"),
            // Untuk pola `”MAHIR”`, menghasilkan memanggil `Translate` dengan `culture`, `”rulesets.form.mode_advanced”` sebagai hasil switch.
            "MAHIR" => Translate(culture, "rulesets.form.mode_advanced"),
            // Untuk pola `”ADVANCED”`, menghasilkan memanggil `Translate` dengan `culture`, `”rulesets.form.mode_advanced”` sebagai hasil switch.
            "ADVANCED" => Translate(culture, "rulesets.form.mode_advanced"),
            // Untuk pola `_`, menghasilkan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(mode)` benar gunakan `”-”`, jika tidak gunakan `mode!`
            // sebagai hasil switch.
            _ => string.IsNullOrWhiteSpace(mode) ? "-" : mode!
        // Menutup scope pemetaan switch atas `normalized`; bagian berikut berada di luar batas blok tersebut dalam TranslateRulesetMode.
        };
    // Menutup scope metode TranslateRulesetMode; bagian berikut berada di luar batas blok tersebut dalam TranslateRulesetMode.
    }

    // Mendefinisikan metode `Translate` dengan hasil bertipe `string`; operasi ini menangani translate. Masukan: Parameter `culture` bertipe `string`
    // membawa nilai culture; Parameter `key` bertipe `string` membawa nilai kunci.
    public static string Translate(string culture, string key)
    // Membuka scope metode Translate; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Translate.
    {
        // Menyiapkan variabel lokal `normalized` untuk nilai normalized dengan memanggil `NormalizeLanguage` dengan `culture`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var normalized = NormalizeLanguage(culture);
        // Memeriksa kebalikan kondisi `Lexicon.TryGetValue(key, out var entry)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // Translate.
        if (!Lexicon.TryGetValue(key, out var entry)) return key;
        // Mengembalikan hasil pemilihan bersyarat: ketika `string.Equals(normalized, AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase)` benar
        // gunakan `entry.En`, jika tidak gunakan `entry.Id` kepada pemanggil dalam Translate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return string.Equals(normalized, AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: entry.En dalam Translate.
            ? entry.En
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: entry.Id; dalam Translate.
            : entry.Id;
    // Menutup scope metode Translate; bagian berikut berada di luar batas blok tersebut dalam Translate.
    }
// Menutup scope tipe UiText; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `UiTextHttpContextExtensions`.
public static class UiTextHttpContextExtensions
// Membuka scope tipe UiTextHttpContextExtensions; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `T` dengan hasil bertipe `string`; operasi ini menangani t. Masukan: Parameter `context` bertipe `HttpContext` membawa
    // konteks operasi yang menyediakan data lingkungan pemrosesan saat ini; Parameter `key` bertipe `string` membawa nilai kunci.
    public static string T(this HttpContext context, string key)
    // Membuka scope metode T; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam T.
    {
        // Menyiapkan variabel lokal `culture` untuk nilai culture dengan `context.Session.GetString(AuthConstants.SessionLanguageKey)` bila tidak null;
        // jika null gunakan `AuthConstants.LanguageId` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var culture = context.Session.GetString(AuthConstants.SessionLanguageKey) ?? AuthConstants.LanguageId;
        // Mengembalikan memanggil `UiText.Translate` dengan `culture`, `key` kepada pemanggil dalam T; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return UiText.Translate(culture, key);
    // Menutup scope metode T; bagian berikut berada di luar batas blok tersebut dalam T.
    }

    // Mendefinisikan metode `TSessionStatus` dengan hasil bertipe `string`; operasi ini menangani t sesi status. Masukan: Parameter `context` bertipe
    // `HttpContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini; Parameter `status` bertipe `string?` membawa nilai
    // status; nilai null diizinkan ketika data opsional belum tersedia.
    public static string TSessionStatus(this HttpContext context, string? status)
    // Membuka scope metode TSessionStatus; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TSessionStatus.
    {
        // Menyiapkan variabel lokal `culture` untuk nilai culture dengan `context.Session.GetString(AuthConstants.SessionLanguageKey)` bila tidak null;
        // jika null gunakan `AuthConstants.LanguageId` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var culture = context.Session.GetString(AuthConstants.SessionLanguageKey) ?? AuthConstants.LanguageId;
        // Mengembalikan memanggil `UiText.TranslateSessionStatus` dengan `culture`, `status` kepada pemanggil dalam TSessionStatus; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return UiText.TranslateSessionStatus(culture, status);
    // Menutup scope metode TSessionStatus; bagian berikut berada di luar batas blok tersebut dalam TSessionStatus.
    }

    // Mendefinisikan metode `TRulesetStatus` dengan hasil bertipe `string`; operasi ini menangani t aturan status. Masukan: Parameter `context` bertipe
    // `HttpContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini; Parameter `status` bertipe `string?` membawa nilai
    // status; nilai null diizinkan ketika data opsional belum tersedia.
    public static string TRulesetStatus(this HttpContext context, string? status)
    // Membuka scope metode TRulesetStatus; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TRulesetStatus.
    {
        // Menyiapkan variabel lokal `culture` untuk nilai culture dengan `context.Session.GetString(AuthConstants.SessionLanguageKey)` bila tidak null;
        // jika null gunakan `AuthConstants.LanguageId` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var culture = context.Session.GetString(AuthConstants.SessionLanguageKey) ?? AuthConstants.LanguageId;
        // Mengembalikan memanggil `UiText.TranslateRulesetStatus` dengan `culture`, `status` kepada pemanggil dalam TRulesetStatus; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return UiText.TranslateRulesetStatus(culture, status);
    // Menutup scope metode TRulesetStatus; bagian berikut berada di luar batas blok tersebut dalam TRulesetStatus.
    }

    // Mendefinisikan metode `TRulesetMode` dengan hasil bertipe `string`; operasi ini menangani t aturan mode. Masukan: Parameter `context` bertipe
    // `HttpContext` membawa konteks operasi yang menyediakan data lingkungan pemrosesan saat ini; Parameter `mode` bertipe `string?` membawa mode
    // permainan yang menentukan kelompok aturan yang digunakan; nilai null diizinkan ketika data opsional belum tersedia.
    public static string TRulesetMode(this HttpContext context, string? mode)
    // Membuka scope metode TRulesetMode; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TRulesetMode.
    {
        // Menyiapkan variabel lokal `culture` untuk nilai culture dengan `context.Session.GetString(AuthConstants.SessionLanguageKey)` bila tidak null;
        // jika null gunakan `AuthConstants.LanguageId` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var culture = context.Session.GetString(AuthConstants.SessionLanguageKey) ?? AuthConstants.LanguageId;
        // Mengembalikan memanggil `UiText.TranslateRulesetMode` dengan `culture`, `mode` kepada pemanggil dalam TRulesetMode; eksekusi jalur ini selesai
        // setelah nilai hasil ditentukan.
        return UiText.TranslateRulesetMode(culture, mode);
    // Menutup scope metode TRulesetMode; bagian berikut berada di luar batas blok tersebut dalam TRulesetMode.
    }
// Menutup scope tipe UiTextHttpContextExtensions; bagian berikut berada di luar batas blok tersebut.
}

