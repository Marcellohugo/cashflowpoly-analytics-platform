// Fungsi file: Memverifikasi validasi pembelian kebutuhan dan aturan urutan kebutuhan primer.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Xunit` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Xunit;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Tests` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Tests;

// Mendefinisikan tipe class `EventNeedPurchaseValidatorTests`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventNeedPurchaseValidatorTests
// Membuka scope tipe EventNeedPurchaseValidatorTests; pernyataan/deklarasi berikut berada di dalam batas blok ini.
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
        // `CreateRequest` dengan `”CatatTransaksi”`, `”””{”amount”:1}”””`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest("CatatTransaksi", """{"amount":1}""");

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventNeedPurchaseValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `[]`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventNeedPurchaseValidator().TryValidate(request, CreateConfig(), [], out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidate_ReturnsFalseForUnhandledAction.
        Assert.False(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidate_ReturnsFalseForUnhandledAction.
        Assert.True(result.Validation.IsValid);
        // Menjalankan pemeriksaan Null atas `result.OutgoingAmount` untuk memastikan keberadaan nilai sesuai kontrak pengujian dalam
        // TryValidate_ReturnsFalseForUnhandledAction.
        Assert.Null(result.OutgoingAmount);
    // Menutup scope metode TryValidate_ReturnsFalseForUnhandledAction; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_ReturnsFalseForUnhandledAction.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidatePrimary_ReturnsOutgoingAmount` dengan hasil bertipe `void`; operasi ini menangani try validate primary returns
    // outgoing nominal.
    public void TryValidatePrimary_ReturnsOutgoingAmount()
    // Membuka scope metode TryValidatePrimary_ReturnsOutgoingAmount; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidatePrimary_ReturnsOutgoingAmount.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”Kebutuhan”`, `”””{”card_id”:”rice”,”amount”:5,”points”:2,”need_tier”:”primer”}”””`, `playerId`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest(
            // Meneruskan nilai literal `”Kebutuhan”` sebagai argumen ke `CreateRequest`.
            "Kebutuhan",
            // Meneruskan nilai literal `”””{”card_id”:”rice”,”amount”:5,”points”:2,”need_tier”:”primer”}”””` sebagai argumen ke `CreateRequest`.
            """{"card_id":"rice","amount":5,"points":2,"need_tier":"primer"}""",
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke `CreateRequest`.
            playerId);

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventNeedPurchaseValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `[]`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventNeedPurchaseValidator().TryValidate(request, CreateConfig(), [], out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidatePrimary_ReturnsOutgoingAmount.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidatePrimary_ReturnsOutgoingAmount.
        Assert.True(result.Validation.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`5`, `result.OutgoingAmount`); pengujian gagal
        // jika keduanya berbeda dalam TryValidatePrimary_ReturnsOutgoingAmount.
        Assert.Equal(5, result.OutgoingAmount);
    // Menutup scope metode TryValidatePrimary_ReturnsOutgoingAmount; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidatePrimary_ReturnsOutgoingAmount.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidatePrimary_RejectsPurchasePastDailyLimit` dengan hasil bertipe `void`; operasi ini menangani try validate primary
    // rejects pembelian past daily limit.
    public void TryValidatePrimary_RejectsPurchasePastDailyLimit()
    // Membuka scope metode TryValidatePrimary_RejectsPurchasePastDailyLimit; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidatePrimary_RejectsPurchasePastDailyLimit.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”Kebutuhan”`, `”””{”card_id”:”rice-2”,”amount”:5,”points”:2,”need_tier”:”primer”}”””`, `playerId`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest(
            // Meneruskan nilai literal `”Kebutuhan”` sebagai argumen ke `CreateRequest`.
            "Kebutuhan",
            // Meneruskan nilai literal `”””{”card_id”:”rice-2”,”amount”:5,”points”:2,”need_tier”:”primer”}”””` sebagai argumen ke `CreateRequest`.
            """{"card_id":"rice-2","amount":5,"points":2,"need_tier":"primer"}""",
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke `CreateRequest`.
            playerId);
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidatePrimary_RejectsPurchasePastDailyLimit.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `request`, `”””{”card_id”:”rice-1”,”amount”:3,”points”:1,”need_tier”:”primer”}”””`
            // dalam TryValidatePrimary_RejectsPurchasePastDailyLimit.
            CreateEvent(request, """{"card_id":"rice-1","amount":3,"points":1,"need_tier":"primer"}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidatePrimary_RejectsPurchasePastDailyLimit.
        };

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventNeedPurchaseValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `history`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventNeedPurchaseValidator().TryValidate(request, CreateConfig(), history, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidatePrimary_RejectsPurchasePastDailyLimit.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidatePrimary_RejectsPurchasePastDailyLimit.
        Assert.False(result.Validation.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”DOMAIN_RULE_VIOLATION”`,
        // `result.Validation.ErrorCode`); pengujian gagal jika keduanya berbeda dalam TryValidatePrimary_RejectsPurchasePastDailyLimit.
        Assert.Equal("DOMAIN_RULE_VIOLATION", result.Validation.ErrorCode);
    // Menutup scope metode TryValidatePrimary_RejectsPurchasePastDailyLimit; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidatePrimary_RejectsPurchasePastDailyLimit.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateSecondary_RejectsPurchaseWithoutPrimaryWhenRequired` dengan hasil bertipe `void`; operasi ini menangani try
    // validate secondary rejects pembelian tanpa primary when required.
    public void TryValidateSecondary_RejectsPurchaseWithoutPrimaryWhenRequired()
    // Membuka scope metode TryValidateSecondary_RejectsPurchaseWithoutPrimaryWhenRequired; pernyataan/deklarasi berikut berada di dalam batas blok ini
    // dalam TryValidateSecondary_RejectsPurchaseWithoutPrimaryWhenRequired.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”Kebutuhan”`, `”””{”card_id”:”book”,”amount”:4,”points”:1,”need_tier”:”sekunder”}”””`, `playerId`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest(
            // Meneruskan nilai literal `”Kebutuhan”` sebagai argumen ke `CreateRequest`.
            "Kebutuhan",
            // Meneruskan nilai literal `”””{”card_id”:”book”,”amount”:4,”points”:1,”need_tier”:”sekunder”}”””` sebagai argumen ke `CreateRequest`.
            """{"card_id":"book","amount":4,"points":1,"need_tier":"sekunder"}""",
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke `CreateRequest`.
            playerId);

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventNeedPurchaseValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `[]`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventNeedPurchaseValidator().TryValidate(request, CreateConfig(), [], out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateSecondary_RejectsPurchaseWithoutPrimaryWhenRequired.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidateSecondary_RejectsPurchaseWithoutPrimaryWhenRequired.
        Assert.False(result.Validation.IsValid);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`”DOMAIN_RULE_VIOLATION”`,
        // `result.Validation.ErrorCode`); pengujian gagal jika keduanya berbeda dalam TryValidateSecondary_RejectsPurchaseWithoutPrimaryWhenRequired.
        Assert.Equal("DOMAIN_RULE_VIOLATION", result.Validation.ErrorCode);
    // Menutup scope metode TryValidateSecondary_RejectsPurchaseWithoutPrimaryWhenRequired; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidateSecondary_RejectsPurchaseWithoutPrimaryWhenRequired.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateSecondary_AllowsPurchaseAfterPrimary` dengan hasil bertipe `void`; operasi ini menangani try validate secondary
    // allows pembelian after primary.
    public void TryValidateSecondary_AllowsPurchaseAfterPrimary()
    // Membuka scope metode TryValidateSecondary_AllowsPurchaseAfterPrimary; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidateSecondary_AllowsPurchaseAfterPrimary.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”Kebutuhan”`, `”””{”card_id”:”book”,”amount”:4,”points”:1,”need_tier”:”sekunder”}”””`, `playerId`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest(
            // Meneruskan nilai literal `”Kebutuhan”` sebagai argumen ke `CreateRequest`.
            "Kebutuhan",
            // Meneruskan nilai literal `”””{”card_id”:”book”,”amount”:4,”points”:1,”need_tier”:”sekunder”}”””` sebagai argumen ke `CreateRequest`.
            """{"card_id":"book","amount":4,"points":1,"need_tier":"sekunder"}""",
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke `CreateRequest`.
            playerId);
        // Menyiapkan variabel lokal `history` untuk nilai history dengan array baru dengan tipe elemen disimpulkan dari nilai initializer. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var history = new[]
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidateSecondary_AllowsPurchaseAfterPrimary.
        {
            // Melanjutkan pengolahan dengan memanggil `CreateEvent` dengan `request`, `”””{”card_id”:”rice”,”amount”:3,”points”:1,”need_tier”:”primer”}”””`
            // dalam TryValidateSecondary_AllowsPurchaseAfterPrimary.
            CreateEvent(request, """{"card_id":"rice","amount":3,"points":1,"need_tier":"primer"}""")
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidateSecondary_AllowsPurchaseAfterPrimary.
        };

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventNeedPurchaseValidator().TryValidate` dengan `request`,
        // `CreateConfig()`, `history`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventNeedPurchaseValidator().TryValidate(request, CreateConfig(), history, out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateSecondary_AllowsPurchaseAfterPrimary.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid`, `result.Validation.Message` bernilai benar; pengujian gagal jika kondisi tidak
        // terpenuhi dalam TryValidateSecondary_AllowsPurchaseAfterPrimary.
        Assert.True(result.Validation.IsValid, result.Validation.Message);
        // Menjalankan pemeriksaan bahwa nilai aktual sama dengan nilai yang diharapkan melalui Assert.Equal(`4`, `result.OutgoingAmount`); pengujian gagal
        // jika keduanya berbeda dalam TryValidateSecondary_AllowsPurchaseAfterPrimary.
        Assert.Equal(4, result.OutgoingAmount);
    // Menutup scope metode TryValidateSecondary_AllowsPurchaseAfterPrimary; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidateSecondary_AllowsPurchaseAfterPrimary.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidateSecondary_AllowsPurchaseWhenRuleIsDisabled` dengan hasil bertipe `void`; operasi ini menangani try validate
    // secondary allows pembelian when rule berstatus disabled.
    public void TryValidateSecondary_AllowsPurchaseWhenRuleIsDisabled()
    // Membuka scope metode TryValidateSecondary_AllowsPurchaseWhenRuleIsDisabled; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidateSecondary_AllowsPurchaseWhenRuleIsDisabled.
    {
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan memanggil `Guid.NewGuid` dengan tanpa argumen. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var playerId = Guid.NewGuid();
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”Kebutuhan”`, `”””{”card_id”:”book”,”amount”:4,”points”:1,”need_tier”:”sekunder”}”””`, `playerId`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest(
            // Meneruskan nilai literal `”Kebutuhan”` sebagai argumen ke `CreateRequest`.
            "Kebutuhan",
            // Meneruskan nilai literal `”””{”card_id”:”book”,”amount”:4,”points”:1,”need_tier”:”sekunder”}”””` sebagai argumen ke `CreateRequest`.
            """{"card_id":"book","amount":4,"points":1,"need_tier":"sekunder"}""",
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke `CreateRequest`.
            playerId);

        // Menyiapkan variabel lokal `handled` untuk nilai handled dengan memanggil `new EventNeedPurchaseValidator().TryValidate` dengan `request`,
        // `CreateConfig() with { RequirePrimaryBeforeOthers = false }`, `[]`, `var result`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var handled = new EventNeedPurchaseValidator().TryValidate(
            // Meneruskan `request` (data masukan permintaan yang akan divalidasi atau diteruskan ke layanan) sebagai argumen ke `new
            // EventNeedPurchaseValidator().TryValidate`.
            request,
            // Meneruskan `CreateConfig() with { RequirePrimaryBeforeOthers = false }` sebagai argumen ke `new EventNeedPurchaseValidator().TryValidate`.
            CreateConfig() with { RequirePrimaryBeforeOthers = false },
            // Meneruskan koleksi kosong dengan tipe mengikuti konteks tujuan sebagai argumen ke `new EventNeedPurchaseValidator().TryValidate`.
            [],
            // Meneruskan `var result` sebagai argumen ke `new EventNeedPurchaseValidator().TryValidate`.
            out var result);

        // Menjalankan pemeriksaan bahwa `handled` bernilai benar; pengujian gagal jika kondisi tidak terpenuhi dalam
        // TryValidateSecondary_AllowsPurchaseWhenRuleIsDisabled.
        Assert.True(handled);
        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid`, `result.Validation.Message` bernilai benar; pengujian gagal jika kondisi tidak
        // terpenuhi dalam TryValidateSecondary_AllowsPurchaseWhenRuleIsDisabled.
        Assert.True(result.Validation.IsValid, result.Validation.Message);
    // Menutup scope metode TryValidateSecondary_AllowsPurchaseWhenRuleIsDisabled; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidateSecondary_AllowsPurchaseWhenRuleIsDisabled.
    }

    // menandai metode sebagai satu kasus uji xUnit tanpa parameter data.
    [Fact]
    // Mendefinisikan metode `TryValidate_RejectsClientValuesThatDifferFromNeedCatalog` dengan hasil bertipe `void`; operasi ini menangani try validate
    // rejects client nilai that differ dari kebutuhan catalog.
    public void TryValidate_RejectsClientValuesThatDifferFromNeedCatalog()
    // Membuka scope metode TryValidate_RejectsClientValuesThatDifferFromNeedCatalog; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
    {
        // Menyiapkan variabel lokal `request` untuk data masukan permintaan yang akan divalidasi atau diteruskan ke layanan dengan memanggil
        // `CreateRequest` dengan `”Kebutuhan”`, `”””{”card_id”:”buku_1”,”amount”:99,”points”:99,”need_tier”:”primer”}”””`, `Guid.NewGuid()`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var request = CreateRequest(
            // Meneruskan nilai literal `”Kebutuhan”` sebagai argumen ke `CreateRequest`.
            "Kebutuhan",
            // Meneruskan nilai literal `”””{”card_id”:”buku_1”,”amount”:99,”points”:99,”need_tier”:”primer”}”””` sebagai argumen ke `CreateRequest`.
            """{"card_id":"buku_1","amount":99,"points":99,"need_tier":"primer"}""",
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke `CreateRequest`.
            Guid.NewGuid());
        // Menyiapkan variabel lokal `config` untuk konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan dengan `CreateConfig() with {
        // Needs = [ new RulesetNeedDto { Id = ”buku_1”, Nama = ”Buku”, Tipe = ”primer”, HargaBeli = 3, PoinKebahagiaan = 1, CardQty = 1 } ] }`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var config = CreateConfig() with
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
        {
            // Memperbarui `Needs` menggunakan koleksi berisi new RulesetNeedDto { Id = ”buku_1”, Nama = ”Buku”,... dalam
            // TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
            Needs =
            // Menggunakan koleksi berisi new RulesetNeedDto { Id = ”buku_1”, Nama = ”Buku”,... sebagai bagian ekspresi yang sedang disusun dalam
            // TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
            [
                // Menggunakan objek baru bertipe `RulesetNeedDto` dengan nilai awal sesuai konstruktornya sebagai bagian ekspresi yang sedang disusun dalam
                // TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
                new RulesetNeedDto
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
                // TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
                {
                    // Memperbarui `Id` menggunakan nilai literal `”buku_1”` dalam TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
                    Id = "buku_1",
                    // Memperbarui `Nama` menggunakan nilai literal `”Buku”` dalam TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
                    Nama = "Buku",
                    // Memperbarui `Tipe` menggunakan nilai literal `”primer”` dalam TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
                    Tipe = "primer",
                    // Memperbarui `HargaBeli` menggunakan nilai literal `3` dalam TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
                    HargaBeli = 3,
                    // Memperbarui `PoinKebahagiaan` menggunakan nilai literal `1` dalam TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
                    PoinKebahagiaan = 1,
                    // Memperbarui `CardQty` menggunakan nilai literal `1` dalam TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
                    CardQty = 1
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
                // TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
                }
            // Menandai akhir daftar elemen atau indeks koleksi dalam TryValidate_RejectsClientValuesThatDifferFromNeedCatalog; pasangan kurung siku
            // mengelompokkan nilai sebagai satu struktur.
            ]
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam
        // TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
        };

        // Menjalankan memanggil `new EventNeedPurchaseValidator().TryValidate` dengan `request`, `config`, `[]`, `var result` dalam
        // TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
        new EventNeedPurchaseValidator().TryValidate(request, config, [], out var result);

        // Menjalankan pemeriksaan bahwa `result.Validation.IsValid` bernilai salah; pengujian gagal jika kondisi justru terpenuhi dalam
        // TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
        Assert.False(result.Validation.IsValid);
        // Menjalankan pemeriksaan Contains untuk memastikan keanggotaan elemen atau potongan teks sesuai harapan: `”katalog”`, `result.Validation.Message`
        // dalam TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
        Assert.Contains("katalog", result.Validation.Message);
    // Menutup scope metode TryValidate_RejectsClientValuesThatDifferFromNeedCatalog; bagian berikut berada di luar batas blok tersebut dalam
    // TryValidate_RejectsClientValuesThatDifferFromNeedCatalog.
    }

    // Mendefinisikan metode `CreateRequest` dengan hasil bertipe `EventRequest`; operasi ini menangani create permintaan. Masukan: Parameter
    // `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payloadJson` bertipe `string` membawa nilai payload JSON; Parameter `playerId`
    // bertipe `Guid?` membawa nilai pemain identitas; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan
    // null, yaitu penanda tidak ada nilai.
    private static EventRequest CreateRequest(string actionType, string payloadJson, Guid? playerId = null)
    // Membuka scope metode CreateRequest; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateRequest.
    {
        // Menyiapkan variabel lokal `document` untuk nilai document dengan memanggil `JsonDocument.Parse` dengan `payloadJson`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis saat scope berakhir.
        using var document = JsonDocument.Parse(payloadJson);
        // Mengembalikan objek baru bertipe `EventRequest` dengan argumen ( Guid.NewGuid(), Guid.NewGuid(), playerId, playerId.HasValue ? ”PLAYER” :
        // ”SYSTEM”, new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero), 0, ”MON”, 1, 0, act... kepada pemanggil dalam CreateRequest; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return new EventRequest(
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan memanggil `Guid.NewGuid` dengan tanpa argumen sebagai argumen ke konstruktor `EventRequest`.
            Guid.NewGuid(),
            // Meneruskan `playerId` (nilai pemain identitas) sebagai argumen ke konstruktor `EventRequest`.
            playerId,
            // Meneruskan hasil pemilihan bersyarat: ketika `playerId.HasValue` benar gunakan `”PLAYER”`, jika tidak gunakan `”SYSTEM”` sebagai argumen ke
            // konstruktor `EventRequest`.
            playerId.HasValue ? "PLAYER" : "SYSTEM",
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
            "client-123");
    // Menutup scope metode CreateRequest; bagian berikut berada di luar batas blok tersebut dalam CreateRequest.
    }

    // Mendefinisikan metode `CreateEvent` dengan hasil bertipe `EventDb`; operasi ini menangani create event. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `payload` bertipe `string` membawa
    // muatan detail event dalam format JSON. Nilai hasil langsung berasal dari objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    private static EventDb CreateEvent(EventRequest request, string payload) => new()
    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateEvent.
    {
        // Memperbarui `EventId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam CreateEvent.
        EventId = Guid.NewGuid(),
        // Memperbarui `SessionId` menggunakan `request.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam CreateEvent.
        SessionId = request.SessionId,
        // Memperbarui `UserId` menggunakan `request.UserId` (identitas akun pengguna yang datanya sedang diproses) dalam CreateEvent.
        UserId = request.UserId,
        // Memperbarui `ActorType` menggunakan nilai literal `”PLAYER”` dalam CreateEvent.
        ActorType = "PLAYER",
        // Memperbarui `Timestamp` menggunakan `request.Timestamp` (waktu kejadian yang menjaga urutan kronologis data) dalam CreateEvent.
        Timestamp = request.Timestamp,
        // Memperbarui `DayIndex` menggunakan `request.DayIndex` (nilai hari index) dalam CreateEvent.
        DayIndex = request.DayIndex,
        // Memperbarui `Weekday` menggunakan `request.Weekday` (nilai weekday) dalam CreateEvent.
        Weekday = request.Weekday,
        // Memperbarui `ActionSlot` menggunakan nilai literal `1` dalam CreateEvent.
        ActionSlot = 1,
        // Memperbarui `SequenceNumber` menggunakan nilai literal `1` dalam CreateEvent.
        SequenceNumber = 1,
        // Memperbarui `ActionType` menggunakan nilai literal `”Kebutuhan”` dalam CreateEvent.
        ActionType = "Kebutuhan",
        // Memperbarui `RulesetVersionId` menggunakan `request.RulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan
        // yang tepat) dalam CreateEvent.
        RulesetVersionId = request.RulesetVersionId,
        // Memperbarui `Payload` menggunakan `payload` (muatan detail event dalam format JSON) dalam CreateEvent.
        Payload = payload
    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam CreateEvent.
    };

    // Mendefinisikan metode `CreateConfig` dengan hasil bertipe `RulesetConfig`; operasi ini menangani create konfigurasi. Nilai hasil langsung berasal
    // dari objek baru dengan tipe mengikuti konteks tujuan dan argumen ( ”PEMULA”, ActionsPerTurn: 2, StartingCash: 20, PlayerOrdering.PlayerOrder,
    // CashMin: 0, MaxIngredientTotal: 10, MaxSameIngredient: 5, PrimaryNeedMaxPerDay: 1, RequirePrimaryB....
    private static RulesetConfig CreateConfig() => new(
        // Meneruskan nilai literal `”PEMULA”` sebagai argumen ke konstruktor dengan tipe mengikuti konteks.
        "PEMULA",
        // Meneruskan nilai literal `2` sebagai argumen bernama `ActionsPerTurn`.
        ActionsPerTurn: 2,
        // Meneruskan nilai literal `20` sebagai argumen bernama `StartingCash`.
        StartingCash: 20,
        // Meneruskan `PlayerOrdering.PlayerOrder` (nomor urut pemain untuk menentukan urutan tindakan) sebagai argumen ke konstruktor dengan tipe mengikuti
        // konteks.
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
        // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen bernama `LoanEnabled`.
        LoanEnabled: false,
        // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen bernama `InsuranceEnabled`.
        InsuranceEnabled: false,
        // Meneruskan false, yaitu kondisi nonaktif/tidak terpenuhi sebagai argumen bernama `SavingGoalEnabled`.
        SavingGoalEnabled: false,
        // Meneruskan nilai literal `5` sebagai argumen bernama `FreelanceIncome`.
        FreelanceIncome: 5,
        // Meneruskan null, yaitu penanda tidak ada nilai sebagai argumen bernama `Scoring`.
        Scoring: null);
// Menutup scope tipe EventNeedPurchaseValidatorTests; bagian berikut berada di luar batas blok tersebut.
}
