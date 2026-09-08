// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventCashflowProjectionBuilder.
// Mengimpor namespace `System.Diagnostics.CodeAnalysis` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Diagnostics.CodeAnalysis;
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

/// <summary>
/// Builder proyeksi cashflow dari event gameplay.
/// </summary>
// Mendefinisikan tipe class `EventCashflowProjectionBuilder` yang mewarisi atau menerapkan `IEventCashflowProjectionBuilder`; sealed mencegah tipe
// ini diturunkan lagi.
internal sealed class EventCashflowProjectionBuilder : IEventCashflowProjectionBuilder
// Membuka scope tipe EventCashflowProjectionBuilder; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `EventPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly EventPayloadReader _payloadReader = new();

    /// <summary>
    /// Mencoba membangun proyeksi arus kas dari event yang berdampak pada saldo pemain.
    /// </summary>
    // Mendefinisikan metode `TryBuild` dengan hasil bertipe `bool`. Mencoba membangun proyeksi arus kas dari event yang berdampak pada saldo pemain.
    // Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter
    // `timestamp` bertipe `DateTimeOffset` membawa waktu kejadian yang menjaga urutan kronologis data; Parameter `eventPk` bertipe `Guid` membawa nilai
    // event pk; Parameter `projection` bertipe `CashflowProjectionDb?` membawa nilai projection; nilai null diizinkan ketika data opsional belum
    // tersedia; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; menerapkan metadata `NotNullWhen(true)` pada deklarasi berikut
    // agar framework/compiler dapat mengenali pengaturannya.
    public bool TryBuild(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `timestamp` bertipe `DateTimeOffset` membawa waktu kejadian yang menjaga urutan kronologis data.
        DateTimeOffset timestamp,
        // Parameter `eventPk` bertipe `Guid` membawa nilai event pk.
        Guid eventPk,
        // Parameter `projection` bertipe `CashflowProjectionDb?` membawa nilai projection; nilai null diizinkan ketika data opsional belum tersedia; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode; menerapkan metadata `NotNullWhen(true)` pada deklarasi berikut agar
        // framework/compiler dapat mengenali pengaturannya.
        [NotNullWhen(true)] out CashflowProjectionDb? projection)
    // Membuka scope metode TryBuild; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryBuild.
    {
        // Memperbarui `projection` menggunakan null, yaitu penanda tidak ada nilai dalam TryBuild.
        projection = null;
        // Memeriksa hasil pencocokan `request.UserId` dengan pola `null`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryBuild.
        if (request.UserId is null)
        // Membuka scope cabang if untuk kondisi `request.UserId is null`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryBuild.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryBuild; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `request.UserId is null`; bagian berikut berada di luar batas blok tersebut dalam TryBuild.
        }

        // Menyiapkan variabel lokal `sourceAction` untuk nilai source aksi dengan membersihkan karakter tepi pada `request.ActionType` memakai tanpa
        // argumen. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sourceAction = request.ActionType.Trim();
        // Menyiapkan variabel lokal `action` untuk nilai aksi dengan `GameActionCatalog.ResolveGameActionId(sourceAction, request.Payload)` bila tidak
        // null; jika null gunakan `sourceAction` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var action = GameActionCatalog.ResolveGameActionId(sourceAction, request.Payload) ?? sourceAction;
        // Menyiapkan variabel lokal `playerId` untuk nilai pemain identitas dengan `request.UserId.Value`, yaitu nilai yang dibungkus objek/nullable. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var playerId = request.UserId.Value;
        // Menyiapkan variabel lokal `direction` untuk nilai direction dengan `string.Empty`, yaitu nilai kosong bawaan tipe terkait. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var direction = string.Empty;
        // Menyiapkan variabel lokal `amount` untuk nominal uang atau nilai transaksi yang dipakai dalam operasi dengan nilai literal `0`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var amount = 0;
        // Menyiapkan variabel lokal `category` untuk nilai category dengan `string.Empty`, yaitu nilai kosong bawaan tipe terkait. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var category = string.Empty;
        // Menyiapkan variabel lokal `counterparty` untuk nilai counterparty dengan null, yaitu penanda tidak ada nilai. Tipe yang dipakai adalah `string?`.
        string? counterparty = null;
        // Menyiapkan variabel lokal `reference` untuk nilai reference dengan null, yaitu penanda tidak ada nilai. Tipe yang dipakai adalah `string?`.
        string? reference = null;
        // Menyiapkan variabel lokal `note` untuk nilai note dengan null, yaitu penanda tidak ada nilai. Tipe yang dipakai adalah `string?`.
        string? note = null;

        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `string.Equals(action, ”CatatTransaksi”, StringComparison.OrdinalIgnoreCase)` dan
        // `_payloadReader.TryReadTransaction(request.Payload, out var dir, out var amt, out var cat, out var cp)`; sisi kanan diperiksa hanya jika sisi
        // kiri benar; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryBuild.
        if (string.Equals(action, "CatatTransaksi", StringComparison.OrdinalIgnoreCase) &&
            // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadTransaction` dengan `request.Payload`, `var dir`, `var amt`, `var cat`, `var cp`
            // dalam TryBuild.
            _payloadReader.TryReadTransaction(request.Payload, out var dir, out var amt, out var cat, out var cp))
        // Membuka scope cabang if untuk kondisi `string.Equals(action, ”CatatTransaksi”, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadTransaction(request.Payload, out var dir, out var amt, out var cat, out va...`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam TryBuild.
        {
            // Memperbarui `direction` menggunakan menormalisasi `dir` menjadi huruf besar dengan aturan kultur invariant dalam TryBuild.
            direction = dir.ToUpperInvariant();
            // Memperbarui `amount` menggunakan hasil konversi `Math.Round(amt)` menjadi tipe `int` dalam TryBuild.
            amount = (int)Math.Round(amt);
            // Memperbarui `category` menggunakan `cat` (nilai cat) dalam TryBuild.
            category = cat;
            // Memperbarui `counterparty` menggunakan `cp` (nilai cp) dalam TryBuild.
            counterparty = cp;
        // Menutup scope cabang if untuk kondisi `string.Equals(action, ”CatatTransaksi”, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadTransaction(request.Payload, out var dir, out var amt, out var cat, out va...`; bagian berikut berada di luar batas blok
        // tersebut dalam TryBuild.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam TryBuild.
        else if (string.Equals(action, GameActionCatalog.JumatBerkah, StringComparison.OrdinalIgnoreCase) &&
                 // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadAmount` dengan `request.Payload`, `var donationAmount` dalam TryBuild.
                 _payloadReader.TryReadAmount(request.Payload, out var donationAmount))
        // Membuka scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.JumatBerkah, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadAmount(request.Payload, out var donationAmount)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryBuild.
        {
            // Memperbarui `direction` menggunakan nilai literal `”OUT”` dalam TryBuild.
            direction = "OUT";
            // Memperbarui `amount` menggunakan hasil konversi `Math.Round(donationAmount)` menjadi tipe `int` dalam TryBuild.
            amount = (int)Math.Round(donationAmount);
            // Memperbarui `category` menggunakan nilai literal `”DONATION”` dalam TryBuild.
            category = "DONATION";
        // Menutup scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.JumatBerkah, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadAmount(request.Payload, out var donationAmount)`; bagian berikut berada di luar batas blok tersebut dalam TryBuild.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam TryBuild.
        else if ((string.Equals(action, GameActionCatalog.InvestasiEmas, StringComparison.OrdinalIgnoreCase) ||
                  // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `action`, `GameActionCatalog.JualEmas`,
                  // `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan comparer yang diberikan dalam TryBuild.
                  string.Equals(action, GameActionCatalog.JualEmas, StringComparison.OrdinalIgnoreCase)) &&
                 // Melanjutkan pengolahan dengan memanggil `TryReadGoldAmount` dengan `request.Payload`, `var tradeAmount` dalam TryBuild.
                 TryReadGoldAmount(request.Payload, out var tradeAmount))
        // Membuka scope cabang if untuk kondisi `(string.Equals(action, GameActionCatalog.InvestasiEmas, StringComparison.OrdinalIgnoreCase) ||
        // string.Equals(action, GameActionCatalog.JualEmas, StringComparison.OrdinalIgnore...`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam TryBuild.
        {
            // Memperbarui `direction` menggunakan hasil pemilihan bersyarat: ketika `string.Equals(action, GameActionCatalog.JualEmas,
            // StringComparison.OrdinalIgnoreCase)` benar gunakan `”IN”`, jika tidak gunakan `”OUT”` dalam TryBuild.
            direction = string.Equals(action, GameActionCatalog.JualEmas, StringComparison.OrdinalIgnoreCase) ? "IN" : "OUT";
            // Memperbarui `amount` menggunakan `tradeAmount` (nilai trade nominal) dalam TryBuild.
            amount = tradeAmount;
            // Memperbarui `category` menggunakan nilai literal `”GOLD_TRADE”` dalam TryBuild.
            category = "GOLD_TRADE";
        // Menutup scope cabang if untuk kondisi `(string.Equals(action, GameActionCatalog.InvestasiEmas, StringComparison.OrdinalIgnoreCase) ||
        // string.Equals(action, GameActionCatalog.JualEmas, StringComparison.OrdinalIgnore...`; bagian berikut berada di luar batas blok tersebut dalam
        // TryBuild.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam TryBuild.
        else if (string.Equals(action, GameActionCatalog.BahanMasakan, StringComparison.OrdinalIgnoreCase) &&
                 // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadIngredientPurchase` dengan `request.Payload`, `_`, `var ingredientAmount` dalam
                 // TryBuild.
                 _payloadReader.TryReadIngredientPurchase(request.Payload, out _, out var ingredientAmount))
        // Membuka scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.BahanMasakan, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadIngredientPurchase(request.Payload, out _, out var ingredien...`; pernyataan/deklarasi berikut berada di dalam batas blok
        // ini dalam TryBuild.
        {
            // Memperbarui `direction` menggunakan nilai literal `”OUT”` dalam TryBuild.
            direction = "OUT";
            // Memperbarui `amount` menggunakan `ingredientAmount` (nilai bahan nominal) dalam TryBuild.
            amount = ingredientAmount;
            // Memperbarui `category` menggunakan nilai literal `”INGREDIENT”` dalam TryBuild.
            category = "INGREDIENT";
        // Menutup scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.BahanMasakan, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadIngredientPurchase(request.Payload, out _, out var ingredien...`; bagian berikut berada di luar batas blok tersebut dalam
        // TryBuild.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam TryBuild.
        else if (string.Equals(action, GameActionCatalog.JualMasakan, StringComparison.OrdinalIgnoreCase) &&
                 // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadOrderClaim` dengan `request.Payload`, `_`, `var income` dalam TryBuild.
                 _payloadReader.TryReadOrderClaim(request.Payload, out _, out var income))
        // Membuka scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.JualMasakan, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadOrderClaim(request.Payload, out _, out var income)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryBuild.
        {
            // Memperbarui `direction` menggunakan nilai literal `”IN”` dalam TryBuild.
            direction = "IN";
            // Memperbarui `amount` menggunakan `income` (nilai pemasukan) dalam TryBuild.
            amount = income;
            // Memperbarui `category` menggunakan nilai literal `”ORDER”` dalam TryBuild.
            category = "ORDER";
        // Menutup scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.JualMasakan, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadOrderClaim(request.Payload, out _, out var income)`; bagian berikut berada di luar batas blok tersebut dalam TryBuild.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam TryBuild.
        else if (string.Equals(action, GameActionCatalog.KerjaLepas, StringComparison.OrdinalIgnoreCase) &&
                 // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadAmount` dengan `request.Payload`, `var freelanceAmount` dalam TryBuild.
                 _payloadReader.TryReadAmount(request.Payload, out var freelanceAmount))
        // Membuka scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.KerjaLepas, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadAmount(request.Payload, out var freelanceAmount)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryBuild.
        {
            // Memperbarui `direction` menggunakan nilai literal `”IN”` dalam TryBuild.
            direction = "IN";
            // Memperbarui `amount` menggunakan hasil konversi `Math.Round(freelanceAmount)` menjadi tipe `int` dalam TryBuild.
            amount = (int)Math.Round(freelanceAmount);
            // Memperbarui `category` menggunakan nilai literal `”FREELANCE”` dalam TryBuild.
            category = "FREELANCE";
        // Menutup scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.KerjaLepas, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadAmount(request.Payload, out var freelanceAmount)`; bagian berikut berada di luar batas blok tersebut dalam TryBuild.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam TryBuild.
        else if (string.Equals(action, GameActionCatalog.Kebutuhan, StringComparison.OrdinalIgnoreCase) &&
                 // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadNeedPurchase` dengan `request.Payload`, `var needCardId`, `var needAmount`, `_`
                 // dalam TryBuild.
                 _payloadReader.TryReadNeedPurchase(request.Payload, out var needCardId, out var needAmount, out _))
        // Membuka scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.Kebutuhan, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadNeedPurchase(request.Payload, out var needCardId, out var needA...`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam TryBuild.
        {
            // Memperbarui `direction` menggunakan nilai literal `”OUT”` dalam TryBuild.
            direction = "OUT";
            // Memperbarui `amount` menggunakan `needAmount` (nilai kebutuhan nominal) dalam TryBuild.
            amount = needAmount;
            // Memperbarui `category` menggunakan hasil pemetaan `NeedTierClassifier.FromPayload(request.Payload, needCardId)` melalui cabang pola switch yang
            // cocok dalam TryBuild.
            category = NeedTierClassifier.FromPayload(request.Payload, needCardId) switch
            // Membuka scope pemetaan switch atas `NeedTierClassifier.FromPayload(request.Payload, needCardId)`; pernyataan/deklarasi berikut berada di dalam
            // batas blok ini dalam TryBuild.
            {
                // Untuk pola `NeedTier.Primary`, menghasilkan nilai literal `”NEED_PRIMARY”` sebagai hasil switch.
                NeedTier.Primary => "NEED_PRIMARY",
                // Untuk pola `NeedTier.Secondary`, menghasilkan nilai literal `”NEED_SECONDARY”` sebagai hasil switch.
                NeedTier.Secondary => "NEED_SECONDARY",
                // Untuk pola `NeedTier.Tertiary`, menghasilkan nilai literal `”NEED_TERTIARY”` sebagai hasil switch.
                NeedTier.Tertiary => "NEED_TERTIARY",
                // Untuk pola `_`, menghasilkan nilai literal `”NEED”` sebagai hasil switch.
                _ => "NEED"
            // Menutup scope pemetaan switch atas `NeedTierClassifier.FromPayload(request.Payload, needCardId)`; bagian berikut berada di luar batas blok
            // tersebut dalam TryBuild.
            };
        // Menutup scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.Kebutuhan, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadNeedPurchase(request.Payload, out var needCardId, out var needA...`; bagian berikut berada di luar batas blok tersebut
        // dalam TryBuild.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam TryBuild.
        else if (string.Equals(action, GameActionCatalog.Menabung, StringComparison.OrdinalIgnoreCase) &&
                 // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadSavingDeposit` dengan `request.Payload`, `_`, `var savingAmount` dalam TryBuild.
                 _payloadReader.TryReadSavingDeposit(request.Payload, out _, out var savingAmount))
        // Membuka scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.Menabung, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadSavingDeposit(request.Payload, out _, out var savingAmount)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam TryBuild.
        {
            // Memperbarui `direction` menggunakan nilai literal `”OUT”` dalam TryBuild.
            direction = "OUT";
            // Memperbarui `amount` menggunakan `savingAmount` (nilai tabungan nominal) dalam TryBuild.
            amount = savingAmount;
            // Memperbarui `category` menggunakan nilai literal `”SAVING_DEPOSIT”` dalam TryBuild.
            category = "SAVING_DEPOSIT";
        // Menutup scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.Menabung, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadSavingDeposit(request.Payload, out _, out var savingAmount)`; bagian berikut berada di luar batas blok tersebut dalam
        // TryBuild.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam TryBuild.
        else if (string.Equals(action, GameActionCatalog.SavingDepositWithdrawn, StringComparison.OrdinalIgnoreCase) &&
                 // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadSavingDeposit` dengan `request.Payload`, `_`, `var savingWithdrawAmount` dalam
                 // TryBuild.
                 _payloadReader.TryReadSavingDeposit(request.Payload, out _, out var savingWithdrawAmount))
        // Membuka scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.SavingDepositWithdrawn, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadSavingDeposit(request.Payload, out _, out var savi...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryBuild.
        {
            // Memperbarui `direction` menggunakan nilai literal `”IN”` dalam TryBuild.
            direction = "IN";
            // Memperbarui `amount` menggunakan `savingWithdrawAmount` (nilai tabungan withdraw nominal) dalam TryBuild.
            amount = savingWithdrawAmount;
            // Memperbarui `category` menggunakan nilai literal `”SAVING_WITHDRAW”` dalam TryBuild.
            category = "SAVING_WITHDRAW";
        // Menutup scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.SavingDepositWithdrawn, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadSavingDeposit(request.Payload, out _, out var savi...`; bagian berikut berada di luar batas blok tersebut dalam TryBuild.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam TryBuild.
        else if (string.Equals(action, GameActionCatalog.RisikoKehidupan, StringComparison.OrdinalIgnoreCase) &&
                 // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadRiskLife` dengan `request.Payload`, `_`, `var riskDirection`, `var riskAmount`
                 // dalam TryBuild.
                 _payloadReader.TryReadRiskLife(request.Payload, out _, out var riskDirection, out var riskAmount))
        // Membuka scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.RisikoKehidupan, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadRiskLife(request.Payload, out _, out var riskDirection, o...`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam TryBuild.
        {
            // Memperbarui `direction` menggunakan menormalisasi `riskDirection` menjadi huruf besar dengan aturan kultur invariant dalam TryBuild.
            direction = riskDirection.ToUpperInvariant();
            // Memperbarui `amount` menggunakan `riskAmount` (nilai risiko nominal) dalam TryBuild.
            amount = riskAmount;
            // Memperbarui `category` menggunakan nilai literal `”RISK_LIFE”` dalam TryBuild.
            category = "RISK_LIFE";
        // Menutup scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.RisikoKehidupan, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadRiskLife(request.Payload, out _, out var riskDirection, o...`; bagian berikut berada di luar batas blok tersebut dalam
        // TryBuild.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam TryBuild.
        else if ((string.Equals(action, GameActionCatalog.PinjamanSyariah, StringComparison.OrdinalIgnoreCase) ||
                  // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `action`, `GameActionCatalog.SetupPinjamanAwal`,
                  // `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan comparer yang diberikan dalam TryBuild.
                  string.Equals(action, GameActionCatalog.SetupPinjamanAwal, StringComparison.OrdinalIgnoreCase)) &&
                 // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadLoanTaken` dengan `request.Payload`, `_`, `var principal`, `_`, `_`, `_` dalam
                 // TryBuild.
                 _payloadReader.TryReadLoanTaken(request.Payload, out _, out var principal, out _, out _, out _))
        // Membuka scope cabang if untuk kondisi `(string.Equals(action, GameActionCatalog.PinjamanSyariah, StringComparison.OrdinalIgnoreCase) ||
        // string.Equals(action, GameActionCatalog.SetupPinjamanAwal, StringComparison.Or...`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam TryBuild.
        {
            // Memperbarui `direction` menggunakan nilai literal `”IN”` dalam TryBuild.
            direction = "IN";
            // Memperbarui `amount` menggunakan `principal` (nilai principal) dalam TryBuild.
            amount = principal;
            // Memperbarui `category` menggunakan nilai literal `”LOAN_TAKEN”` dalam TryBuild.
            category = "LOAN_TAKEN";
        // Menutup scope cabang if untuk kondisi `(string.Equals(action, GameActionCatalog.PinjamanSyariah, StringComparison.OrdinalIgnoreCase) ||
        // string.Equals(action, GameActionCatalog.SetupPinjamanAwal, StringComparison.Or...`; bagian berikut berada di luar batas blok tersebut dalam
        // TryBuild.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam TryBuild.
        else if (string.Equals(action, GameActionCatalog.BayarPinjaman, StringComparison.OrdinalIgnoreCase) &&
                 // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadLoanRepay` dengan `request.Payload`, `_`, `var repayAmount` dalam TryBuild.
                 _payloadReader.TryReadLoanRepay(request.Payload, out _, out var repayAmount))
        // Membuka scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.BayarPinjaman, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadLoanRepay(request.Payload, out _, out var repayAmount)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // TryBuild.
        {
            // Memperbarui `direction` menggunakan nilai literal `”OUT”` dalam TryBuild.
            direction = "OUT";
            // Memperbarui `amount` menggunakan `repayAmount` (nilai repay nominal) dalam TryBuild.
            amount = repayAmount;
            // Memperbarui `category` menggunakan nilai literal `”LOAN_REPAID”` dalam TryBuild.
            category = "LOAN_REPAID";
        // Menutup scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.BayarPinjaman, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadLoanRepay(request.Payload, out _, out var repayAmount)`; bagian berikut berada di luar batas blok tersebut dalam TryBuild.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam TryBuild.
        else if (string.Equals(action, GameActionCatalog.Asuransi, StringComparison.OrdinalIgnoreCase) &&
                 // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadInsurance` dengan `request.Payload`, `var premium` dalam TryBuild.
                 _payloadReader.TryReadInsurance(request.Payload, out var premium))
        // Membuka scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.Asuransi, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadInsurance(request.Payload, out var premium)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryBuild.
        {
            // Memperbarui `direction` menggunakan nilai literal `”OUT”` dalam TryBuild.
            direction = "OUT";
            // Memperbarui `amount` menggunakan `premium` (nilai premium) dalam TryBuild.
            amount = premium;
            // Memperbarui `category` menggunakan nilai literal `”INSURANCE_PREMIUM”` dalam TryBuild.
            category = "INSURANCE_PREMIUM";
        // Menutup scope cabang if untuk kondisi `string.Equals(action, GameActionCatalog.Asuransi, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadInsurance(request.Payload, out var premium)`; bagian berikut berada di luar batas blok tersebut dalam TryBuild.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam TryBuild.
        else if (string.Equals(action, "GunakanOpsiDarurat", StringComparison.OrdinalIgnoreCase) &&
                 // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryReadEmergencyOption` dengan `request.Payload`, `_`, `var emergencyOption`, `var
                 // emergencyDirection`, `var emergencyAmount` dalam TryBuild.
                 _payloadReader.TryReadEmergencyOption(request.Payload, out _, out var emergencyOption, out var emergencyDirection, out var emergencyAmount) &&
                 // Melanjutkan pengolahan dengan menormalisasi `emergencyOption` menjadi huruf besar dengan aturan kultur invariant dalam TryBuild.
                 emergencyOption.ToUpperInvariant() is "SELL_NEED" or "SELL_GOLD" or "TAKE_SHARIA_LOAN")
        // Membuka scope cabang if untuk kondisi `string.Equals(action, ”GunakanOpsiDarurat”, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadEmergencyOption(request.Payload, out _, out var emergencyOption, out v...`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam TryBuild.
        {
            // Memperbarui `direction` menggunakan menormalisasi `emergencyDirection` menjadi huruf besar dengan aturan kultur invariant dalam TryBuild.
            direction = emergencyDirection.ToUpperInvariant();
            // Memperbarui `amount` menggunakan `emergencyAmount` (nilai emergency nominal) dalam TryBuild.
            amount = emergencyAmount;
            // Memperbarui `category` menggunakan nilai literal `”EMERGENCY_OPTION”` dalam TryBuild.
            category = "EMERGENCY_OPTION";
        // Menutup scope cabang if untuk kondisi `string.Equals(action, ”GunakanOpsiDarurat”, StringComparison.OrdinalIgnoreCase) &&
        // _payloadReader.TryReadEmergencyOption(request.Payload, out _, out var emergencyOption, out v...`; bagian berikut berada di luar batas blok
        // tersebut dalam TryBuild.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam TryBuild.
        else
        // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryBuild.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryBuild; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam TryBuild.
        }

        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `amount <= 0` dan `string.IsNullOrWhiteSpace(direction)`; sisi kanan
        // diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam TryBuild.
        if (amount <= 0 || string.IsNullOrWhiteSpace(direction))
        // Membuka scope cabang if untuk kondisi `amount <= 0 || string.IsNullOrWhiteSpace(direction)`; pernyataan/deklarasi berikut berada di dalam batas
        // blok ini dalam TryBuild.
        {
            // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryBuild; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return false;
        // Menutup scope cabang if untuk kondisi `amount <= 0 || string.IsNullOrWhiteSpace(direction)`; bagian berikut berada di luar batas blok tersebut
        // dalam TryBuild.
        }

        // Memperbarui `projection` menggunakan objek baru bertipe `CashflowProjectionDb` dengan nilai awal sesuai konstruktornya dalam TryBuild.
        projection = new CashflowProjectionDb
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryBuild.
        {
            // Memperbarui `ProjectionId` menggunakan memanggil `Guid.NewGuid` dengan tanpa argumen dalam TryBuild.
            ProjectionId = Guid.NewGuid(),
            // Memperbarui `SessionId` menggunakan `request.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam TryBuild.
            SessionId = request.SessionId,
            // Memperbarui `UserId` menggunakan `playerId` (nilai pemain identitas) dalam TryBuild.
            UserId = playerId,
            // Memperbarui `EventPk` menggunakan `eventPk` (nilai event pk) dalam TryBuild.
            EventPk = eventPk,
            // Memperbarui `EventId` menggunakan `request.EventId` (identitas unik event untuk pencatatan dan pemeriksaan duplikasi) dalam TryBuild.
            EventId = request.EventId,
            // Memperbarui `Timestamp` menggunakan `timestamp` (waktu kejadian yang menjaga urutan kronologis data) dalam TryBuild.
            Timestamp = timestamp,
            // Memperbarui `Direction` menggunakan `direction` (nilai direction) dalam TryBuild.
            Direction = direction,
            // Memperbarui `Amount` menggunakan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) dalam TryBuild.
            Amount = amount,
            // Memperbarui `Category` menggunakan `category` (nilai category) dalam TryBuild.
            Category = category,
            // Memperbarui `Counterparty` menggunakan `counterparty` (nilai counterparty) dalam TryBuild.
            Counterparty = counterparty,
            // Memperbarui `Reference` menggunakan `reference` (nilai reference) dalam TryBuild.
            Reference = reference,
            // Memperbarui `Note` menggunakan `note` (nilai note) dalam TryBuild.
            Note = note
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam TryBuild.
        };

        // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryBuild; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return true;
    // Menutup scope metode TryBuild; bagian berikut berada di luar batas blok tersebut dalam TryBuild.
    }

    // Mendefinisikan metode `TryReadGoldAmount` dengan hasil bertipe `bool`; operasi ini menangani try read emas nominal. Masukan: Parameter `payload`
    // bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `amount` bertipe `int` membawa nominal uang atau nilai transaksi
    // yang dipakai dalam operasi; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    private static bool TryReadGoldAmount(JsonElement payload, out int amount)
    // Membuka scope metode TryReadGoldAmount; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadGoldAmount.
    {
        // Memperbarui `amount` menggunakan nilai literal `0` dalam TryReadGoldAmount.
        amount = 0;
        // Memeriksa memanggil `_payloadReader.TryReadGoldTrade` dengan `payload`, `_`, `_`, `_`, `amount`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam TryReadGoldAmount.
        if (_payloadReader.TryReadGoldTrade(payload, out _, out _, out _, out amount))
        // Membuka scope cabang if untuk kondisi `_payloadReader.TryReadGoldTrade(payload, out _, out _, out _, out amount)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam TryReadGoldAmount.
        {
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryReadGoldAmount; eksekusi jalur ini selesai setelah nilai hasil
            // ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `_payloadReader.TryReadGoldTrade(payload, out _, out _, out _, out amount)`; bagian berikut berada di luar
        // batas blok tersebut dalam TryReadGoldAmount.
        }

        // Mengembalikan gabungan syarat AND: kedua kondisi wajib benar antara `_payloadReader.TryGetInt32(payload, ”qty”, out var qty) &&
        // _payloadReader.TryGetInt32(payload, ”unit_price”, out var unitPrice) && _payloadReader.TryGetInt32(payload, ”amount”...` dan `unitPrice > 0`;
        // sisi kanan diperiksa hanya jika sisi kiri benar kepada pemanggil dalam TryReadGoldAmount; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return _payloadReader.TryGetInt32(payload, "qty", out var qty) &&
               // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryGetInt32` dengan `payload`, `”unit_price”`, `var unitPrice` dalam TryReadGoldAmount.
               _payloadReader.TryGetInt32(payload, "unit_price", out var unitPrice) &&
               // Melanjutkan pengolahan dengan memanggil `_payloadReader.TryGetInt32` dengan `payload`, `”amount”`, `amount` dalam TryReadGoldAmount.
               _payloadReader.TryGetInt32(payload, "amount", out amount) &&
               // Melanjutkan ekspresi dengan pemeriksaan lebih besar antara `qty` dan `0` dalam TryReadGoldAmount.
               qty > 0 &&
               // Melanjutkan ekspresi dengan pemeriksaan lebih besar antara `unitPrice` dan `0` dalam TryReadGoldAmount.
               unitPrice > 0;
    // Menutup scope metode TryReadGoldAmount; bagian berikut berada di luar batas blok tersebut dalam TryReadGoldAmount.
    }
// Menutup scope tipe EventCashflowProjectionBuilder; bagian berikut berada di luar batas blok tersebut.
}
