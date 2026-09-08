// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk SessionEventProjector.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Cashflowpoly.Api.Domain` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Domain;
// Mengimpor namespace `Dapper` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Dapper;
// Mengimpor namespace `Npgsql` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Npgsql;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Data` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Data;

// Mendefinisikan tipe class `SessionEventProjector`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionEventProjector
// Membuka scope tipe SessionEventProjector; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `IEventPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca. readonly membatasi penggantian
    // referensi/nilai field pada deklarasi atau konstruktor.
    private readonly IEventPayloadReader _payloadReader;

    // Mendefinisikan konstruktor SessionEventProjector yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `payloadReader` bertipe `IEventPayloadReader` membawa nilai payload pembaca.
    public SessionEventProjector(IEventPayloadReader payloadReader)
    // Membuka scope konstruktor SessionEventProjector; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam SessionEventProjector.
    {
        // Memperbarui `_payloadReader` menggunakan `payloadReader` (nilai payload pembaca) dalam SessionEventProjector.
        _payloadReader = payloadReader;
    // Menutup scope konstruktor SessionEventProjector; bagian berikut berada di luar batas blok tersebut dalam SessionEventProjector.
    }

    // Mendefinisikan metode `ProjectAsync` dengan hasil bertipe `Task`; operasi ini menangani project asinkron. async memungkinkan metode menunggu
    // operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan
    // permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `storedEvent` bertipe `EventDb` membawa nilai stored event; Parameter
    // `cashflowProjections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai arus kas projections; Parameter `conn` bertipe
    // `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction`
    // membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `ct` bertipe `CancellationToken` membawa sinyal
    // pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    public async Task ProjectAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `storedEvent` bertipe `EventDb` membawa nilai stored event.
        EventDb storedEvent,
        // Parameter `cashflowProjections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa nilai arus kas projections.
        IReadOnlyCollection<CashflowProjectionDb> cashflowProjections,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectAsync.
    {
        // Menjalankan hasil operasi asinkron memanggil `UpdateSessionStateAsync` dengan `request`, `storedEvent.SessionPlayerId`, `conn`, `tx`, `ct`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
        await UpdateSessionStateAsync(request, storedEvent.SessionPlayerId, conn, tx, ct);
        // Menyiapkan variabel lokal `canonicalAction` untuk nilai canonical aksi dengan `GameActionCatalog.ResolveGameActionId(request.ActionType,
        // request.Payload)` bila tidak null; jika null gunakan `request.ActionType.Trim()` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var canonicalAction = GameActionCatalog.ResolveGameActionId(request.ActionType, request.Payload) ?? request.ActionType.Trim();

        // Memeriksa kebalikan kondisi `storedEvent.SessionPlayerId.HasValue`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ProjectAsync.
        if (!storedEvent.SessionPlayerId.HasValue)
        // Membuka scope cabang if untuk kondisi `!storedEvent.SessionPlayerId.HasValue`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ProjectAsync.
        {
            // Memeriksa memanggil `GameActionCatalog.Is` dengan `request.ActionType`, `request.Payload`, `GameActionCatalog.SessionEnded`; blok if hanya
            // dijalankan ketika kondisi ini bernilai benar dalam ProjectAsync.
            if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.SessionEnded))
            // Membuka scope cabang if untuk kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.SessionEnded)`;
            // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectAsync.
            {
                // Menjalankan hasil operasi asinkron memanggil `MarkIncompleteMissionsFailedAsync` dengan `request.SessionId`, `storedEvent.EventId`, `conn`, `tx`,
                // `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await MarkIncompleteMissionsFailedAsync(request.SessionId, storedEvent.EventId, conn, tx, ct);
            // Menutup scope cabang if untuk kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.SessionEnded)`; bagian berikut
            // berada di luar batas blok tersebut dalam ProjectAsync.
            }

            // Menjalankan hasil operasi asinkron memanggil `UpdateProjectionCheckpointAsync` dengan `request.SessionId`, `storedEvent.SequenceNumber`,
            // `storedEvent.EventId`, `conn`, `tx`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
            await UpdateProjectionCheckpointAsync(request.SessionId, storedEvent.SequenceNumber, storedEvent.EventId, conn, tx, ct);
            // Mengakhiri eksekusi lebih awal dalam ProjectAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!storedEvent.SessionPlayerId.HasValue`; bagian berikut berada di luar batas blok tersebut dalam
        // ProjectAsync.
        }

        // Menyiapkan variabel lokal `participantId` untuk nilai participant identitas dengan `storedEvent.SessionPlayerId.Value`, yaitu nilai yang
        // dibungkus objek/nullable. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var participantId = storedEvent.SessionPlayerId.Value;
        // Menjalankan hasil operasi asinkron memanggil `EnsureParticipantBalanceAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
        await EnsureParticipantBalanceAsync(request, participantId, conn, tx, ct);
        // Menjalankan hasil operasi asinkron memanggil `ApplyCashflowAsync` dengan `participantId`, `storedEvent.EventId`, `cashflowProjections`, `conn`,
        // `tx`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
        await ApplyCashflowAsync(participantId, storedEvent.EventId, cashflowProjections, conn, tx, ct);
        // Memeriksa membandingkan kesamaan `string` dengan `storedEvent.ActorType`, `”PLAYER”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
        // mengikuti overload dan comparer yang diberikan; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ProjectAsync.
        if (string.Equals(storedEvent.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `string.Equals(storedEvent.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam ProjectAsync.
        {
            // Menjalankan hasil operasi asinkron memanggil `IncrementActionCounterAsync` dengan `request.SessionId`, `participantId`,
            // `request.RulesetVersionId`, `storedEvent.RulesetActionId`, `storedEvent.EventId`, `conn`, `tx`, `ct`; await menunggu hasil tanpa memblokir thread
            // selama operasi belum selesai dalam ProjectAsync.
            await IncrementActionCounterAsync(
                // Meneruskan `request.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke
                // `IncrementActionCounterAsync`.
                request.SessionId,
                // Meneruskan `participantId` (nilai participant identitas) sebagai argumen ke `IncrementActionCounterAsync`.
                participantId,
                // Meneruskan `request.RulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
                // `IncrementActionCounterAsync`.
                request.RulesetVersionId,
                // Meneruskan `storedEvent.RulesetActionId` (nilai aturan aksi identitas) sebagai argumen ke `IncrementActionCounterAsync`.
                storedEvent.RulesetActionId,
                // Meneruskan `storedEvent.EventId` (identitas unik event untuk pencatatan dan pemeriksaan duplikasi) sebagai argumen ke
                // `IncrementActionCounterAsync`.
                storedEvent.EventId,
                // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `IncrementActionCounterAsync`.
                conn,
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `IncrementActionCounterAsync`.
                tx,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `IncrementActionCounterAsync`.
                ct);
            // Menjalankan hasil operasi asinkron memanggil `ProjectNarrativeTriggersAsync` dengan `request`, `participantId`, `storedEvent.RulesetActionId`,
            // `storedEvent.EventId`, `conn`, `tx`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
            await ProjectNarrativeTriggersAsync(
                // Meneruskan `request` (data masukan permintaan yang akan divalidasi atau diteruskan ke layanan) sebagai argumen ke
                // `ProjectNarrativeTriggersAsync`.
                request,
                // Meneruskan `participantId` (nilai participant identitas) sebagai argumen ke `ProjectNarrativeTriggersAsync`.
                participantId,
                // Meneruskan `storedEvent.RulesetActionId` (nilai aturan aksi identitas) sebagai argumen ke `ProjectNarrativeTriggersAsync`.
                storedEvent.RulesetActionId,
                // Meneruskan `storedEvent.EventId` (identitas unik event untuk pencatatan dan pemeriksaan duplikasi) sebagai argumen ke
                // `ProjectNarrativeTriggersAsync`.
                storedEvent.EventId,
                // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `ProjectNarrativeTriggersAsync`.
                conn,
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `ProjectNarrativeTriggersAsync`.
                tx,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `ProjectNarrativeTriggersAsync`.
                ct);
        // Menutup scope cabang if untuk kondisi `string.Equals(storedEvent.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada
        // di luar batas blok tersebut dalam ProjectAsync.
        }

        // Memilih cabang berdasarkan `canonicalAction` (nilai canonical aksi); label case menentukan perlakuan untuk setiap nilai yang dikenali dalam
        // ProjectAsync.
        switch (canonicalAction)
        // Membuka scope pemilihan switch atas `canonicalAction`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectAsync.
        {
            // Menetapkan label cabang `case GameActionCatalog.BahanMasakan:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.BahanMasakan:
                // Menjalankan hasil operasi asinkron memanggil `ProjectIngredientPurchaseAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await
                // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectIngredientPurchaseAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.IngredientDiscarded:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.IngredientDiscarded:
                // Menjalankan hasil operasi asinkron memanggil `ProjectIngredientDiscardAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await
                // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectIngredientDiscardAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.JualMasakan:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.JualMasakan:
                // Menjalankan hasil operasi asinkron memanggil `ProjectOrderClaimAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await menunggu hasil
                // tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectOrderClaimAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.Kebutuhan:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.Kebutuhan:
                // Menjalankan hasil operasi asinkron memanggil `ProjectNeedPurchaseAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await menunggu
                // hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectNeedPurchaseAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.SetupMisiAwal:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.SetupMisiAwal:
                // Menjalankan hasil operasi asinkron memanggil `ProjectMissionAssignmentAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await
                // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectMissionAssignmentAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.JumatBerkah:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.JumatBerkah:
                // Menjalankan hasil operasi asinkron memanggil `ProjectDonationAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await menunggu hasil
                // tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectDonationAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.DonationRankAwarded:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.DonationRankAwarded:
                // Menjalankan hasil operasi asinkron memanggil `ProjectDonationRankingAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await menunggu
                // hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectDonationRankingAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.InvestasiEmas:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.InvestasiEmas:
            // Menetapkan label cabang `case GameActionCatalog.JualEmas:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.JualEmas:
                // Menjalankan hasil operasi asinkron memanggil `ProjectGoldTradeAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await menunggu hasil
                // tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectGoldTradeAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.SetupEmasAwal:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.SetupEmasAwal:
                // Menjalankan hasil operasi asinkron memanggil `ProjectInitialGoldAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await menunggu
                // hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectInitialGoldAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.GoldPointsAwarded:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.GoldPointsAwarded:
                // Menjalankan hasil operasi asinkron memanggil `ProjectAwardedPointsAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await menunggu
                // hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectAwardedPointsAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.Menabung:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.Menabung:
                // Menjalankan hasil operasi asinkron memanggil `ProjectSavingDepositAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await menunggu
                // hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectSavingDepositAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.SavingDepositWithdrawn:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch
            // ini.
            case GameActionCatalog.SavingDepositWithdrawn:
                // Menjalankan hasil operasi asinkron memanggil `ProjectSavingWithdrawalAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await menunggu
                // hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectSavingWithdrawalAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.TujuanFinansial:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.TujuanFinansial:
                // Menjalankan hasil operasi asinkron memanggil `ProjectSavingGoalAchievedAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await
                // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectSavingGoalAchievedAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.PinjamanSyariah:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.PinjamanSyariah:
                // Menjalankan hasil operasi asinkron memanggil `ProjectLoanTakenAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await menunggu hasil
                // tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectLoanTakenAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.BayarPinjaman:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.BayarPinjaman:
                // Menjalankan hasil operasi asinkron memanggil `ProjectLoanRepaidAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await menunggu hasil
                // tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectLoanRepaidAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.RiskEmergencyUsed:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.RiskEmergencyUsed:
                // Menjalankan hasil operasi asinkron memanggil `ProjectEmergencyOptionAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await menunggu
                // hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectEmergencyOptionAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.Asuransi:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.Asuransi:
                // Memeriksa memanggil `_payloadReader.TryReadInsuranceUsed` dengan `request.Payload`, `_`; blok if hanya dijalankan ketika kondisi ini bernilai
                // benar dalam ProjectAsync.
                if (_payloadReader.TryReadInsuranceUsed(request.Payload, out _))
                // Membuka scope cabang if untuk kondisi `_payloadReader.TryReadInsuranceUsed(request.Payload, out _)`; pernyataan/deklarasi berikut berada di dalam
                // batas blok ini dalam ProjectAsync.
                {
                    // Menjalankan hasil operasi asinkron memanggil `ProjectInsuranceUsedAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await menunggu
                    // hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                    await ProjectInsuranceUsedAsync(request, participantId, conn, tx, ct);
                // Menutup scope cabang if untuk kondisi `_payloadReader.TryReadInsuranceUsed(request.Payload, out _)`; bagian berikut berada di luar batas blok
                // tersebut dalam ProjectAsync.
                }
                // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam ProjectAsync.
                else
                // Membuka scope cabang else; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectAsync.
                {
                    // Menjalankan hasil operasi asinkron memanggil `ProjectInsurancePurchasedAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await
                    // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                    await ProjectInsurancePurchasedAsync(request, participantId, conn, tx, ct);
                // Menutup scope cabang else; bagian berikut berada di luar batas blok tersebut dalam ProjectAsync.
                }
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.TieBreakerAssigned:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.TieBreakerAssigned:
                // Menjalankan hasil operasi asinkron memanggil `ProjectTieBreakerAsync` dengan `request`, `participantId`, `storedEvent.EventId`, `conn`, `tx`,
                // `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectTieBreakerAsync(request, participantId, storedEvent.EventId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.PensionRankAwarded:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.PensionRankAwarded:
                // Menjalankan hasil operasi asinkron memanggil `ProjectPensionRankingAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await menunggu
                // hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await ProjectPensionRankingAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
            // Menetapkan label cabang `case GameActionCatalog.SessionEnded:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case GameActionCatalog.SessionEnded:
                // Menjalankan hasil operasi asinkron memanggil `MarkIncompleteMissionsFailedAsync` dengan `request.SessionId`, `storedEvent.EventId`, `conn`, `tx`,
                // `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
                await MarkIncompleteMissionsFailedAsync(request.SessionId, storedEvent.EventId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectAsync.
                break;
        // Menutup scope pemilihan switch atas `canonicalAction`; bagian berikut berada di luar batas blok tersebut dalam ProjectAsync.
        }

        // Menjalankan hasil operasi asinkron memanggil `UpdateProjectionCheckpointAsync` dengan `request.SessionId`, `storedEvent.SequenceNumber`,
        // `storedEvent.EventId`, `conn`, `tx`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAsync.
        await UpdateProjectionCheckpointAsync(request.SessionId, storedEvent.SequenceNumber, storedEvent.EventId, conn, tx, ct);
    // Menutup scope metode ProjectAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectAsync.
    }

    // Mendefinisikan metode `ProjectMarketRefillAsync` dengan hasil bertipe `Task`; operasi ini menangani project pasar refill asinkron. Masukan:
    // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `eventId`
    // bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter `conn` bertipe `NpgsqlConnection` membawa
    // koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data
    // yang menggabungkan perubahan sebagai satu kesatuan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat
    // dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static Task ProjectMarketRefillAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectMarketRefillAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectMarketRefillAsync.
    {
        // Memeriksa kebalikan kondisi `TryReadMarketReference(request.Payload, out var slotGroup, out var slotCode, out var assetType, out var assetCode)`;
        // blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ProjectMarketRefillAsync.
        if (!TryReadMarketReference(request.Payload, out var slotGroup, out var slotCode, out var assetType, out var assetCode))
        // Membuka scope cabang if untuk kondisi `!TryReadMarketReference(request.Payload, out var slotGroup, out var slotCode, out var assetType, out var
        // assetCode)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectMarketRefillAsync.
        {
            // Mengembalikan `Task.CompletedTask` (nilai selesai task) kepada pemanggil dalam ProjectMarketRefillAsync; eksekusi jalur ini selesai setelah nilai
            // hasil ditentukan.
            return Task.CompletedTask;
        // Menutup scope cabang if untuk kondisi `!TryReadMarketReference(request.Payload, out var slotGroup, out var slotCode, out var assetType, out var
        // assetCode)`; bagian berikut berada di luar batas blok tersebut dalam ProjectMarketRefillAsync.
        }

        // Mengembalikan memanggil `ProjectMarketRefillAsync` dengan `request.SessionId`, `request.RulesetVersionId`, `eventId`, `slotGroup`, `slotCode`,
        // `assetType`, `assetCode`, `conn`, `tx`, `ct` kepada pemanggil dalam ProjectMarketRefillAsync; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return ProjectMarketRefillAsync(
            // Meneruskan `request.SessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) sebagai argumen ke `ProjectMarketRefillAsync`.
            request.SessionId,
            // Meneruskan `request.RulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat) sebagai argumen ke
            // `ProjectMarketRefillAsync`.
            request.RulesetVersionId,
            // Meneruskan `eventId` (identitas unik event untuk pencatatan dan pemeriksaan duplikasi) sebagai argumen ke `ProjectMarketRefillAsync`.
            eventId,
            // Meneruskan `slotGroup` (nilai slot group) sebagai argumen ke `ProjectMarketRefillAsync`.
            slotGroup,
            // Meneruskan `slotCode` (nilai slot kode) sebagai argumen ke `ProjectMarketRefillAsync`.
            slotCode,
            // Meneruskan `assetType` (nilai aset jenis) sebagai argumen ke `ProjectMarketRefillAsync`.
            assetType,
            // Meneruskan `assetCode` (nilai aset kode) sebagai argumen ke `ProjectMarketRefillAsync`.
            assetCode,
            // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `ProjectMarketRefillAsync`.
            conn,
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `ProjectMarketRefillAsync`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // ke `ProjectMarketRefillAsync`.
            ct);
    // Menutup scope metode ProjectMarketRefillAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectMarketRefillAsync.
    }

    // Mendefinisikan metode `ProjectMarketRefillAsync` dengan hasil bertipe `Task`; operasi ini menangani project pasar refill asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi
    // aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk
    // pencatatan dan pemeriksaan duplikasi; Parameter `slotGroup` bertipe `string` membawa nilai slot group; Parameter `slotCode` bertipe `string`
    // membawa nilai slot kode; Parameter `assetType` bertipe `string` membawa nilai aset jenis; Parameter `assetCode` bertipe `string` membawa nilai
    // aset kode; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter
    // `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    internal static async Task ProjectMarketRefillAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat.
        Guid rulesetVersionId,
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `slotGroup` bertipe `string` membawa nilai slot group.
        string slotGroup,
        // Parameter `slotCode` bertipe `string` membawa nilai slot kode.
        string slotCode,
        // Parameter `assetType` bertipe `string` membawa nilai aset jenis.
        string assetType,
        // Parameter `assetCode` bertipe `string` membawa nilai aset kode.
        string assetCode,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectMarketRefillAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectMarketRefillAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select ensure_session_card_positions_initialized(@sessionId);`.
        // Baris literal 3: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 4: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update session_card_positions
        // position`.
        // Baris literal 5: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set zone = 'MARKET',`.
        // Baris literal 6: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `slot_group = @slotGroup,`.
        // Baris literal 7: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `slot_code = @slotCode,`.
        // Baris literal 8: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `position_order = nullif(regexp_replace(@slotCode, '\D', '', 'g'), '')::int,`.
        // Baris literal 9: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `last_event_id = @eventId,`.
        // Baris literal 10: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
        // Baris literal 11: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where position.card_position_id = (`.
        // Baris literal 12: SELECT menentukan nilai atau kolom yang dikembalikan query: `select candidate.card_position_id`.
        // Baris literal 13: FROM memilih tabel/subquery sumber pembacaan: `from session_card_positions candidate`.
        // Baris literal 14: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_game_assets asset`.
        // Baris literal 15: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on asset.ruleset_game_asset_id = candidate.ruleset_game_asset_id`.
        // Baris literal 16: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and asset.ruleset_version_id =
        // candidate.ruleset_version_id`.
        // Baris literal 17: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where candidate.session_id = @sessionId`.
        // Baris literal 18: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and candidate.status = 'ACTIVE'`.
        // Baris literal 19: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and candidate.zone in ('DECK', 'DISCARD')`.
        // Baris literal 20: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and asset.asset_type = upper(@assetType)`.
        // Baris literal 21: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(asset.asset_code) = lower(@assetCode)`.
        // Baris literal 22: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by case candidate.zone when 'DECK' then 1
        // else 2 end,`.
        // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `candidate.copy_number`.
        // Baris literal 24: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 25: Pembatas literal/penutup `);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 26: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 27: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_card_positions (`.
        // Baris literal 28: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `card_instance_id,
        // session_id, ruleset_version_id, ruleset_game_asset_id,`.
        // Baris literal 29: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `copy_number, zone,
        // position_order, slot_code, slot_group, status, last_event_id`.
        // Baris literal 30: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 31: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 32: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `gen_random_uuid(), @sessionId, asset.ruleset_version_id, asset.ruleset_game_asset_id,`.
        // Baris literal 33: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce((`.
        // Baris literal 34: SELECT menentukan nilai atau kolom yang dikembalikan query: `select max(existing.copy_number) + 1`.
        // Baris literal 35: FROM memilih tabel/subquery sumber pembacaan: `from session_card_positions existing`.
        // Baris literal 36: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where existing.session_id = @sessionId`.
        // Baris literal 37: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and existing.ruleset_game_asset_id =
        // asset.ruleset_game_asset_id`.
        // Baris literal 38: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `), 1),`.
        // Baris literal 39: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `'MARKET', nullif(regexp_replace(@slotCode, '\D', '', 'g'), '')::int,`.
        // Baris literal 40: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@slotCode, @slotGroup, 'ACTIVE', @eventId`.
        // Baris literal 41: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_game_assets asset`.
        // Baris literal 42: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where asset.ruleset_version_id = @rulesetVersionId`.
        // Baris literal 43: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and asset.asset_type = 'INGREDIENT'`.
        // Baris literal 44: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(asset.asset_code) = lower(@assetCode)`.
        // Baris literal 45: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and asset.is_active`.
        // Baris literal 46: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not exists (`.
        // Baris literal 47: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
        // Baris literal 48: FROM memilih tabel/subquery sumber pembacaan: `from session_card_positions existing`.
        // Baris literal 49: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where existing.session_id = @sessionId`.
        // Baris literal 50: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and existing.zone = 'MARKET'`.
        // Baris literal 51: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and existing.status = 'ACTIVE'`.
        // Baris literal 52: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and existing.slot_group = @slotGroup`.
        // Baris literal 53: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and existing.slot_code = @slotCode`.
        // Baris literal 54: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 55: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1;`.
        // Baris literal 56: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            select ensure_session_card_positions_initialized(@sessionId);

            update session_card_positions position
            set zone = 'MARKET',
                slot_group = @slotGroup,
                slot_code = @slotCode,
                position_order = nullif(regexp_replace(@slotCode, '\D', '', 'g'), '')::int,
                last_event_id = @eventId,
                updated_at = now()
            where position.card_position_id = (
                select candidate.card_position_id
                from session_card_positions candidate
                join ruleset_game_assets asset
                  on asset.ruleset_game_asset_id = candidate.ruleset_game_asset_id
                 and asset.ruleset_version_id = candidate.ruleset_version_id
                where candidate.session_id = @sessionId
                  and candidate.status = 'ACTIVE'
                  and candidate.zone in ('DECK', 'DISCARD')
                  and asset.asset_type = upper(@assetType)
                  and lower(asset.asset_code) = lower(@assetCode)
                order by case candidate.zone when 'DECK' then 1 else 2 end,
                         candidate.copy_number
                limit 1
            );

            insert into session_card_positions (
                card_instance_id, session_id, ruleset_version_id, ruleset_game_asset_id,
                copy_number, zone, position_order, slot_code, slot_group, status, last_event_id
            )
            select
                gen_random_uuid(), @sessionId, asset.ruleset_version_id, asset.ruleset_game_asset_id,
                coalesce((
                    select max(existing.copy_number) + 1
                    from session_card_positions existing
                    where existing.session_id = @sessionId
                      and existing.ruleset_game_asset_id = asset.ruleset_game_asset_id
                ), 1),
                'MARKET', nullif(regexp_replace(@slotCode, '\D', '', 'g'), '')::int,
                @slotCode, @slotGroup, 'ACTIVE', @eventId
            from ruleset_game_assets asset
            where asset.ruleset_version_id = @rulesetVersionId
              and asset.asset_type = 'INGREDIENT'
              and lower(asset.asset_code) = lower(@assetCode)
              and asset.is_active
              and not exists (
                  select 1
                  from session_card_positions existing
                  where existing.session_id = @sessionId
                    and existing.zone = 'MARKET'
                    and existing.status = 'ACTIVE'
                    and existing.slot_group = @slotGroup
                    and existing.slot_code = @slotCode
              )
            limit 1;
            """;

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( sql, new { sessionId,
        // rulesetVersionId, slotGroup, slotCode, assetType, assetCode, eventId }, tx, cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang
        // terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectMarketRefillAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`.
            sql,
            // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, slotGroup, slotCode, assetType, assetCode, eventId sebagai satu nilai
            // sebagai argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectMarketRefillAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, slotGroup, slotCode, assetType, assetCode, eventId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                sessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, slotGroup, slotCode, assetType, assetCode, eventId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                rulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, slotGroup, slotCode, assetType, assetCode, eventId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                slotGroup,
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, slotGroup, slotCode, assetType, assetCode, eventId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                slotCode,
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, slotGroup, slotCode, assetType, assetCode, eventId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                assetType,
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, slotGroup, slotCode, assetType, assetCode, eventId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                assetCode,
                // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, slotGroup, slotCode, assetType, assetCode, eventId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                eventId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ProjectMarketRefillAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode ProjectMarketRefillAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectMarketRefillAsync.
    }

    // Mendefinisikan metode `ProjectCardDiscardAsync` dengan hasil bertipe `Task`; operasi ini menangani project kartu discard asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `eventId` bertipe `Guid` membawa
    // identitas unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk
    // mengirim perintah dan membaca hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan
    // perubahan sebagai satu kesatuan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika
    // pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task ProjectCardDiscardAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectCardDiscardAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectCardDiscardAsync.
    {
        // Menyiapkan variabel lokal `slotGroup` untuk nilai slot group dengan hasil pemilihan bersyarat: ketika
        // `request.Payload.TryGetProperty(”slot_group”, out var groupValue)` benar gunakan `groupValue.GetString()`, jika tidak gunakan `null`. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var slotGroup = request.Payload.TryGetProperty("slot_group", out var groupValue) ? groupValue.GetString() : null;
        // Menyiapkan variabel lokal `slotCode` untuk nilai slot kode dengan hasil pemilihan bersyarat: ketika `request.Payload.TryGetProperty(”slot_code”,
        // out var slotValue)` benar gunakan `slotValue.GetString()`, jika tidak gunakan `null`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var slotCode = request.Payload.TryGetProperty("slot_code", out var slotValue) ? slotValue.GetString() : null;
        // Menyiapkan variabel lokal `assetType` untuk nilai aset jenis dengan hasil pemilihan bersyarat: ketika
        // `request.Payload.TryGetProperty(”asset_type”, out var typeValue)` benar gunakan `typeValue.GetString()`, jika tidak gunakan `null`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var assetType = request.Payload.TryGetProperty("asset_type", out var typeValue) ? typeValue.GetString() : null;
        // Menyiapkan variabel lokal `assetCode` untuk nilai aset kode dengan hasil pemilihan bersyarat: ketika
        // `request.Payload.TryGetProperty(”asset_code”, out var codeValue)` benar gunakan `codeValue.GetString()`, jika tidak gunakan `null`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var assetCode = request.Payload.TryGetProperty("asset_code", out var codeValue) ? codeValue.GetString() : null;
        // Menyiapkan variabel lokal `hasSlot` untuk nilai memiliki slot dengan gabungan syarat AND: kedua kondisi wajib benar antara
        // `!string.IsNullOrWhiteSpace(slotGroup)` dan `!string.IsNullOrWhiteSpace(slotCode)`; sisi kanan diperiksa hanya jika sisi kiri benar. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var hasSlot = !string.IsNullOrWhiteSpace(slotGroup) && !string.IsNullOrWhiteSpace(slotCode);
        // Menyiapkan variabel lokal `hasAsset` untuk nilai memiliki aset dengan gabungan syarat AND: kedua kondisi wajib benar antara
        // `!string.IsNullOrWhiteSpace(assetType)` dan `!string.IsNullOrWhiteSpace(assetCode)`; sisi kanan diperiksa hanya jika sisi kiri benar. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var hasAsset = !string.IsNullOrWhiteSpace(assetType) && !string.IsNullOrWhiteSpace(assetCode);
        // Memeriksa gabungan syarat AND: kedua kondisi wajib benar antara `!hasSlot` dan `!hasAsset`; sisi kanan diperiksa hanya jika sisi kiri benar; blok
        // if hanya dijalankan ketika kondisi ini bernilai benar dalam ProjectCardDiscardAsync.
        if (!hasSlot && !hasAsset)
        // Membuka scope cabang if untuk kondisi `!hasSlot && !hasAsset`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ProjectCardDiscardAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectCardDiscardAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!hasSlot && !hasAsset`; bagian berikut berada di luar batas blok tersebut dalam ProjectCardDiscardAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” update session_card_positions
        // position set zone = 'DISCARD', owner_session_participant_id = null, slot_group = null, slot_code = null, last_event_id...`; nilai hasil
        // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ProjectCardDiscardAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update session_card_positions
            // position`.
            // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set zone = 'DISCARD',`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `owner_session_participant_id
            // = null,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `slot_group = null,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `slot_code = null,`.
            // Baris literal 7: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 8: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 9: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where position.card_position_id = (`.
            // Baris literal 10: SELECT menentukan nilai atau kolom yang dikembalikan query: `select candidate.card_position_id`.
            // Baris literal 11: FROM memilih tabel/subquery sumber pembacaan: `from session_card_positions candidate`.
            // Baris literal 12: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_game_assets asset`.
            // Baris literal 13: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on asset.ruleset_game_asset_id = candidate.ruleset_game_asset_id`.
            // Baris literal 14: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and asset.ruleset_version_id =
            // candidate.ruleset_version_id`.
            // Baris literal 15: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where candidate.session_id = @sessionId`.
            // Baris literal 16: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and candidate.zone <> 'DISCARD'`.
            // Baris literal 17: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and candidate.status = 'ACTIVE'`.
            // Baris literal 18: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (`.
            // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `(@hasSlot and candidate.zone = 'MARKET' and candidate.slot_group = @slotGroup and candidate.slot_code = @slotCode)`.
            // Baris literal 20: Melanjutkan kondisi SQL dengan OR (alternatif syarat yang dapat terpenuhi): `or (not @hasSlot and asset.asset_type =
            // upper(@assetType) and lower(asset.asset_code) = lower(@assetCode))`.
            // Baris literal 21: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 22: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by case candidate.zone when 'MARKET' then 1
            // when 'PLAYER' then 2 else 3 end,`.
            // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `candidate.copy_number`.
            // Baris literal 24: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
            // Baris literal 25: Pembatas literal/penutup `);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 26: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            update session_card_positions position
            set zone = 'DISCARD',
                owner_session_participant_id = null,
                slot_group = null,
                slot_code = null,
                last_event_id = @eventId,
                updated_at = now()
            where position.card_position_id = (
                select candidate.card_position_id
                from session_card_positions candidate
                join ruleset_game_assets asset
                  on asset.ruleset_game_asset_id = candidate.ruleset_game_asset_id
                 and asset.ruleset_version_id = candidate.ruleset_version_id
                where candidate.session_id = @sessionId
                  and candidate.zone <> 'DISCARD'
                  and candidate.status = 'ACTIVE'
                  and (
                    (@hasSlot and candidate.zone = 'MARKET' and candidate.slot_group = @slotGroup and candidate.slot_code = @slotCode)
                    or (not @hasSlot and asset.asset_type = upper(@assetType) and lower(asset.asset_code) = lower(@assetCode))
                  )
                order by case candidate.zone when 'MARKET' then 1 when 'PLAYER' then 2 else 3 end,
                         candidate.copy_number
                limit 1
            );
            """,
            // Meneruskan objek anonim yang mengelompokkan sessionId, hasSlot, slotGroup, slotCode, assetType, assetCode, eventId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectCardDiscardAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, hasSlot, slotGroup, slotCode, assetType, assetCode, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                sessionId = request.SessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, hasSlot, slotGroup, slotCode, assetType, assetCode, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                hasSlot,
                // Meneruskan objek anonim yang mengelompokkan sessionId, hasSlot, slotGroup, slotCode, assetType, assetCode, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                slotGroup,
                // Meneruskan objek anonim yang mengelompokkan sessionId, hasSlot, slotGroup, slotCode, assetType, assetCode, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                slotCode,
                // Meneruskan objek anonim yang mengelompokkan sessionId, hasSlot, slotGroup, slotCode, assetType, assetCode, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                assetType,
                // Meneruskan objek anonim yang mengelompokkan sessionId, hasSlot, slotGroup, slotCode, assetType, assetCode, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                assetCode,
                // Meneruskan objek anonim yang mengelompokkan sessionId, hasSlot, slotGroup, slotCode, assetType, assetCode, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                eventId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ProjectCardDiscardAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode ProjectCardDiscardAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectCardDiscardAsync.
    }

    // Mendefinisikan metode `TakeMarketCardAsync` dengan hasil bertipe `Task`; operasi ini menangani take pasar kartu asinkron. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe `EventRequest` membawa
    // data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa nilai participant
    // identitas; Parameter `assetType` bertipe `string` membawa nilai aset jenis; Parameter `payloadProperty` bertipe `string` membawa nilai payload
    // property; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter
    // `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task TakeMarketCardAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `assetType` bertipe `string` membawa nilai aset jenis.
        string assetType,
        // Parameter `payloadProperty` bertipe `string` membawa nilai payload property.
        string payloadProperty,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode TakeMarketCardAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TakeMarketCardAsync.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!request.Payload.TryGetProperty(payloadProperty, out var codeValue)`
        // dan `string.IsNullOrWhiteSpace(codeValue.GetString())`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam TakeMarketCardAsync.
        if (!request.Payload.TryGetProperty(payloadProperty, out var codeValue) ||
            // Melanjutkan pengolahan dengan memeriksa apakah `codeValue.GetString()` null, kosong, atau hanya berisi karakter spasi dalam TakeMarketCardAsync.
            string.IsNullOrWhiteSpace(codeValue.GetString()))
        // Membuka scope cabang if untuk kondisi `!request.Payload.TryGetProperty(payloadProperty, out var codeValue) ||
        // string.IsNullOrWhiteSpace(codeValue.GetString())`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TakeMarketCardAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam TakeMarketCardAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!request.Payload.TryGetProperty(payloadProperty, out var codeValue) ||
        // string.IsNullOrWhiteSpace(codeValue.GetString())`; bagian berikut berada di luar batas blok tersebut dalam TakeMarketCardAsync.
        }

        // Menyiapkan variabel lokal `affected` untuk nilai affected dengan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new
        // CommandDefinition( ””” update session_card_positions position set zone = 'PLAYER', owner_session_participant_id = @participantId, slot_group =
        // null, slot_code = null, last...`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var affected = await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update session_card_positions
            // position`.
            // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set zone = 'PLAYER',`.
            // Baris literal 4: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `owner_session_participant_id = @participantId,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `slot_group = null,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `slot_code = null,`.
            // Baris literal 7: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 8: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 9: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where position.card_position_id = (`.
            // Baris literal 10: SELECT menentukan nilai atau kolom yang dikembalikan query: `select candidate.card_position_id`.
            // Baris literal 11: FROM memilih tabel/subquery sumber pembacaan: `from session_card_positions candidate`.
            // Baris literal 12: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_game_assets asset`.
            // Baris literal 13: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on asset.ruleset_game_asset_id = candidate.ruleset_game_asset_id`.
            // Baris literal 14: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and asset.ruleset_version_id =
            // candidate.ruleset_version_id`.
            // Baris literal 15: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where candidate.session_id = @sessionId`.
            // Baris literal 16: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and candidate.zone = 'MARKET'`.
            // Baris literal 17: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and candidate.status = 'ACTIVE'`.
            // Baris literal 18: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and asset.asset_type = @assetType`.
            // Baris literal 19: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(asset.asset_code) = lower(@assetCode)`.
            // Baris literal 20: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by candidate.slot_code`.
            // Baris literal 21: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
            // Baris literal 22: Pembatas literal/penutup `);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 23: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            update session_card_positions position
            set zone = 'PLAYER',
                owner_session_participant_id = @participantId,
                slot_group = null,
                slot_code = null,
                last_event_id = @eventId,
                updated_at = now()
            where position.card_position_id = (
                select candidate.card_position_id
                from session_card_positions candidate
                join ruleset_game_assets asset
                  on asset.ruleset_game_asset_id = candidate.ruleset_game_asset_id
                 and asset.ruleset_version_id = candidate.ruleset_version_id
                where candidate.session_id = @sessionId
                  and candidate.zone = 'MARKET'
                  and candidate.status = 'ACTIVE'
                  and asset.asset_type = @assetType
                  and lower(asset.asset_code) = lower(@assetCode)
                order by candidate.slot_code
                limit 1
            );
            """,
            // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, assetType, assetCode, eventId sebagai satu nilai sebagai argumen ke
            // konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // TakeMarketCardAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, assetType, assetCode, eventId sebagai satu nilai sebagai argumen ke
                // konstruktor `CommandDefinition`.
                sessionId = request.SessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, assetType, assetCode, eventId sebagai satu nilai sebagai argumen ke
                // konstruktor `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, assetType, assetCode, eventId sebagai satu nilai sebagai argumen ke
                // konstruktor `CommandDefinition`.
                assetType,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, assetType, assetCode, eventId sebagai satu nilai sebagai argumen ke
                // konstruktor `CommandDefinition`.
                assetCode = codeValue.GetString(),
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, assetType, assetCode, eventId sebagai satu nilai sebagai argumen ke
                // konstruktor `CommandDefinition`.
                eventId = request.EventId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam TakeMarketCardAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));

        // Memeriksa perbandingan ketidaksamaan antara `affected` dan `1`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // TakeMarketCardAsync.
        if (affected != 1)
        // Membuka scope cabang if untuk kondisi `affected != 1`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TakeMarketCardAsync.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen ($”Kartu {assetType}/{codeValue.GetString()}
            // tidak tersedia di pasar.”) dalam TakeMarketCardAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException($"Kartu {assetType}/{codeValue.GetString()} tidak tersedia di pasar.");
        // Menutup scope cabang if untuk kondisi `affected != 1`; bagian berikut berada di luar batas blok tersebut dalam TakeMarketCardAsync.
        }
    // Menutup scope metode TakeMarketCardAsync; bagian berikut berada di luar batas blok tersebut dalam TakeMarketCardAsync.
    }

    // Mendefinisikan metode `DiscardOwnedCardAsync` dengan hasil bertipe `Task`; operasi ini menangani discard dimiliki kartu asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `assetType` bertipe `string` membawa nilai aset jenis; Parameter `payloadProperty` bertipe `string`
    // membawa nilai payload property; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private static async Task DiscardOwnedCardAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `assetType` bertipe `string` membawa nilai aset jenis.
        string assetType,
        // Parameter `payloadProperty` bertipe `string` membawa nilai payload property.
        string payloadProperty,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode DiscardOwnedCardAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DiscardOwnedCardAsync.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!request.Payload.TryGetProperty(payloadProperty, out var codeValue)`
        // dan `string.IsNullOrWhiteSpace(codeValue.GetString())`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam DiscardOwnedCardAsync.
        if (!request.Payload.TryGetProperty(payloadProperty, out var codeValue) ||
            // Melanjutkan pengolahan dengan memeriksa apakah `codeValue.GetString()` null, kosong, atau hanya berisi karakter spasi dalam
            // DiscardOwnedCardAsync.
            string.IsNullOrWhiteSpace(codeValue.GetString()))
        // Membuka scope cabang if untuk kondisi `!request.Payload.TryGetProperty(payloadProperty, out var codeValue) ||
        // string.IsNullOrWhiteSpace(codeValue.GetString())`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam DiscardOwnedCardAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam DiscardOwnedCardAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!request.Payload.TryGetProperty(payloadProperty, out var codeValue) ||
        // string.IsNullOrWhiteSpace(codeValue.GetString())`; bagian berikut berada di luar batas blok tersebut dalam DiscardOwnedCardAsync.
        }

        // Menyiapkan variabel lokal `quantity` untuk nilai jumlah dengan hasil pemilihan bersyarat: ketika `request.Payload.TryGetProperty(”amount”, out
        // var amountValue) && amountValue.TryGetInt32(out var amount)` benar gunakan `Math.Max(1, amount)`, jika tidak gunakan `1`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var quantity = request.Payload.TryGetProperty("amount", out var amountValue) && amountValue.TryGetInt32(out var amount)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: Math.Max(1, amount) dalam DiscardOwnedCardAsync.
            ? Math.Max(1, amount)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: 1; dalam DiscardOwnedCardAsync.
            : 1;
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” update session_card_positions
        // position set zone = 'DISCARD', owner_session_participant_id = null, last_event_id = @eventId, updated_at = now() where...`; nilai hasil
        // menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam DiscardOwnedCardAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update session_card_positions
            // position`.
            // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set zone = 'DISCARD',`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `owner_session_participant_id
            // = null,`.
            // Baris literal 5: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 6: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 7: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where position.card_position_id in (`.
            // Baris literal 8: SELECT menentukan nilai atau kolom yang dikembalikan query: `select candidate.card_position_id`.
            // Baris literal 9: FROM memilih tabel/subquery sumber pembacaan: `from session_card_positions candidate`.
            // Baris literal 10: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_game_assets asset`.
            // Baris literal 11: ON menetapkan syarat pencocokan baris pada relasi JOIN: `on asset.ruleset_game_asset_id = candidate.ruleset_game_asset_id`.
            // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and asset.ruleset_version_id =
            // candidate.ruleset_version_id`.
            // Baris literal 13: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where candidate.session_id = @sessionId`.
            // Baris literal 14: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and candidate.owner_session_participant_id =
            // @participantId`.
            // Baris literal 15: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and candidate.zone = 'PLAYER'`.
            // Baris literal 16: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and candidate.status = 'ACTIVE'`.
            // Baris literal 17: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and asset.asset_type = @assetType`.
            // Baris literal 18: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(asset.asset_code) = lower(@assetCode)`.
            // Baris literal 19: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by candidate.copy_number`.
            // Baris literal 20: LIMIT membatasi jumlah baris yang dikembalikan query: `limit @quantity`.
            // Baris literal 21: Pembatas literal/penutup `);`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 22: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            update session_card_positions position
            set zone = 'DISCARD',
                owner_session_participant_id = null,
                last_event_id = @eventId,
                updated_at = now()
            where position.card_position_id in (
                select candidate.card_position_id
                from session_card_positions candidate
                join ruleset_game_assets asset
                  on asset.ruleset_game_asset_id = candidate.ruleset_game_asset_id
                 and asset.ruleset_version_id = candidate.ruleset_version_id
                where candidate.session_id = @sessionId
                  and candidate.owner_session_participant_id = @participantId
                  and candidate.zone = 'PLAYER'
                  and candidate.status = 'ACTIVE'
                  and asset.asset_type = @assetType
                  and lower(asset.asset_code) = lower(@assetCode)
                order by candidate.copy_number
                limit @quantity
            );
            """,
            // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, assetType, assetCode, quantity, eventId sebagai satu nilai sebagai argumen
            // ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // DiscardOwnedCardAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, assetType, assetCode, quantity, eventId sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                sessionId = request.SessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, assetType, assetCode, quantity, eventId sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, assetType, assetCode, quantity, eventId sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                assetType,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, assetType, assetCode, quantity, eventId sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                assetCode = codeValue.GetString(),
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, assetType, assetCode, quantity, eventId sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                quantity,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, assetType, assetCode, quantity, eventId sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                eventId = request.EventId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam DiscardOwnedCardAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode DiscardOwnedCardAsync; bagian berikut berada di luar batas blok tersebut dalam DiscardOwnedCardAsync.
    }

    // Mendefinisikan metode `DiscardOrderIngredientsAsync` dengan hasil bertipe `Task`; operasi ini menangani discard urutan/pesanan bahan asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private async Task DiscardOrderIngredientsAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode DiscardOrderIngredientsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // DiscardOrderIngredientsAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadOrderClaim(request.Payload, out var requiredCards, out _)`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam DiscardOrderIngredientsAsync.
        if (!_payloadReader.TryReadOrderClaim(request.Payload, out var requiredCards, out _))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadOrderClaim(request.Payload, out var requiredCards, out _)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam DiscardOrderIngredientsAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam DiscardOrderIngredientsAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadOrderClaim(request.Payload, out var requiredCards, out _)`; bagian berikut berada
        // di luar batas blok tersebut dalam DiscardOrderIngredientsAsync.
        }

        // Mengulangi setiap elemen `requiredCards`; elemen saat ini disimpan sebagai `cardId` bertipe `var` untuk diproses oleh badan loop dalam
        // DiscardOrderIngredientsAsync.
        foreach (var cardId in requiredCards)
        // Membuka scope loop setiap cardId dari `requiredCards`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // DiscardOrderIngredientsAsync.
        {
            // Menyiapkan variabel lokal `payload` untuk muatan detail event dalam format JSON dengan memanggil `System.Text.Json.JsonDocument.Parse` dengan
            // `$$”””{”card_id”:”{{cardId}}”,”amount”:1}”””`. Tipe variabel disimpulkan dari ekspresi nilai awal; using memastikan sumber daya dilepas otomatis
            // saat scope berakhir.
            using var payload = System.Text.Json.JsonDocument.Parse($$"""{"card_id":"{{cardId}}","amount":1}""");
            // Menjalankan hasil operasi asinkron memanggil `DiscardOwnedCardAsync` dengan `request with { Payload = payload.RootElement.Clone() }`,
            // `participantId`, `”INGREDIENT”`, `”card_id”`, `conn`, `tx`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
            // DiscardOrderIngredientsAsync.
            await DiscardOwnedCardAsync(
                // Meneruskan `request with { Payload = payload.RootElement.Clone() }` sebagai argumen ke `DiscardOwnedCardAsync`.
                request with { Payload = payload.RootElement.Clone() },
                // Meneruskan `participantId` (nilai participant identitas) sebagai argumen ke `DiscardOwnedCardAsync`.
                participantId,
                // Meneruskan nilai literal `”INGREDIENT”` sebagai argumen ke `DiscardOwnedCardAsync`.
                "INGREDIENT",
                // Meneruskan nilai literal `”card_id”` sebagai argumen ke `DiscardOwnedCardAsync`.
                "card_id",
                // Meneruskan `conn` (koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data) sebagai argumen ke `DiscardOwnedCardAsync`.
                conn,
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke `DiscardOwnedCardAsync`.
                tx,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // ke `DiscardOwnedCardAsync`.
                ct);
        // Menutup scope loop setiap cardId dari `requiredCards`; bagian berikut berada di luar batas blok tersebut dalam DiscardOrderIngredientsAsync.
        }
    // Menutup scope metode DiscardOrderIngredientsAsync; bagian berikut berada di luar batas blok tersebut dalam DiscardOrderIngredientsAsync.
    }

    // Mendefinisikan metode `TryReadMarketReference` dengan hasil bertipe `bool`; operasi ini menangani try read pasar reference. Masukan: Parameter
    // `payload` bertipe `System.Text.Json.JsonElement` membawa muatan detail event dalam format JSON; Parameter `slotGroup` bertipe `string` membawa
    // nilai slot group; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `slotCode` bertipe `string` membawa nilai slot
    // kode; out mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `assetType` bertipe `string` membawa nilai aset jenis; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode; Parameter `assetCode` bertipe `string` membawa nilai aset kode; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    private static bool TryReadMarketReference(
        // Parameter `payload` bertipe `System.Text.Json.JsonElement` membawa muatan detail event dalam format JSON.
        System.Text.Json.JsonElement payload,
        // Parameter `slotGroup` bertipe `string` membawa nilai slot group; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string slotGroup,
        // Parameter `slotCode` bertipe `string` membawa nilai slot kode; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string slotCode,
        // Parameter `assetType` bertipe `string` membawa nilai aset jenis; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string assetType,
        // Parameter `assetCode` bertipe `string` membawa nilai aset kode; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out string assetCode)
    // Membuka scope metode TryReadMarketReference; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryReadMarketReference.
    {
        // Memperbarui `slotGroup` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadMarketReference.
        slotGroup = string.Empty;
        // Memperbarui `slotCode` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadMarketReference.
        slotCode = string.Empty;
        // Memperbarui `assetType` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadMarketReference.
        assetType = string.Empty;
        // Memperbarui `assetCode` menggunakan `string.Empty`, yaitu nilai kosong bawaan tipe terkait dalam TryReadMarketReference.
        assetCode = string.Empty;
        // Mengembalikan gabungan syarat AND: kedua kondisi wajib benar antara `payload.TryGetProperty(”slot_group”, out var group) &&
        // payload.TryGetProperty(”slot_code”, out var slot) && payload.TryGetProperty(”asset_type”, out var type) && payload.TryGe...` dan
        // `!string.IsNullOrWhiteSpace(assetCode = code.GetString() ?? string.Empty)`; sisi kanan diperiksa hanya jika sisi kiri benar kepada pemanggil
        // dalam TryReadMarketReference; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return payload.TryGetProperty("slot_group", out var group) &&
               // Melanjutkan pengolahan dengan mencari properti JSON `”slot_code”`, `var slot` pada `payload` tanpa menganggap propertinya selalu tersedia dalam
               // TryReadMarketReference.
               payload.TryGetProperty("slot_code", out var slot) &&
               // Melanjutkan pengolahan dengan mencari properti JSON `”asset_type”`, `var type` pada `payload` tanpa menganggap propertinya selalu tersedia dalam
               // TryReadMarketReference.
               payload.TryGetProperty("asset_type", out var type) &&
               // Melanjutkan pengolahan dengan mencari properti JSON `”asset_code”`, `var code` pada `payload` tanpa menganggap propertinya selalu tersedia dalam
               // TryReadMarketReference.
               payload.TryGetProperty("asset_code", out var code) &&
               // Menggunakan kebalikan kondisi `string.IsNullOrWhiteSpace(slotGroup = group.GetString() ?? string.Empty)` sebagai bagian ekspresi yang sedang
               // disusun dalam TryReadMarketReference.
               !string.IsNullOrWhiteSpace(slotGroup = group.GetString() ?? string.Empty) &&
               // Menggunakan kebalikan kondisi `string.IsNullOrWhiteSpace(slotCode = slot.GetString() ?? string.Empty)` sebagai bagian ekspresi yang sedang
               // disusun dalam TryReadMarketReference.
               !string.IsNullOrWhiteSpace(slotCode = slot.GetString() ?? string.Empty) &&
               // Menggunakan kebalikan kondisi `string.IsNullOrWhiteSpace(assetType = type.GetString() ?? string.Empty)` sebagai bagian ekspresi yang sedang
               // disusun dalam TryReadMarketReference.
               !string.IsNullOrWhiteSpace(assetType = type.GetString() ?? string.Empty) &&
               // Menggunakan kebalikan kondisi `string.IsNullOrWhiteSpace(assetCode = code.GetString() ?? string.Empty)` sebagai bagian ekspresi yang sedang
               // disusun dalam TryReadMarketReference.
               !string.IsNullOrWhiteSpace(assetCode = code.GetString() ?? string.Empty);
    // Menutup scope metode TryReadMarketReference; bagian berikut berada di luar batas blok tersebut dalam TryReadMarketReference.
    }

    // Mendefinisikan metode `ProjectEmergencyOptionAsync` dengan hasil bertipe `Task`; operasi ini menangani project emergency option asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private async Task ProjectEmergencyOptionAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectEmergencyOptionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectEmergencyOptionAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryGetString(request.Payload, ”option_type”, out var optionType)`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam ProjectEmergencyOptionAsync.
        if (!_payloadReader.TryGetString(request.Payload, "option_type", out var optionType))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryGetString(request.Payload, ”option_type”, out var optionType)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam ProjectEmergencyOptionAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectEmergencyOptionAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryGetString(request.Payload, ”option_type”, out var optionType)`; bagian berikut berada
        // di luar batas blok tersebut dalam ProjectEmergencyOptionAsync.
        }

        // Memilih cabang berdasarkan menormalisasi `optionType` menjadi huruf besar dengan aturan kultur invariant; label case menentukan perlakuan untuk
        // setiap nilai yang dikenali dalam ProjectEmergencyOptionAsync.
        switch (optionType.ToUpperInvariant())
        // Membuka scope pemilihan switch atas `optionType.ToUpperInvariant()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ProjectEmergencyOptionAsync.
        {
            // Menetapkan label cabang `case ”SELL_NEED”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "SELL_NEED":
                // Menjalankan hasil operasi asinkron memanggil `ProjectEmergencyNeedSaleAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await
                // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectEmergencyOptionAsync.
                await ProjectEmergencyNeedSaleAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectEmergencyOptionAsync.
                break;
            // Menetapkan label cabang `case ”SELL_GOLD”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "SELL_GOLD":
                // Menjalankan hasil operasi asinkron memanggil `ProjectEmergencyGoldSaleAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await
                // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectEmergencyOptionAsync.
                await ProjectEmergencyGoldSaleAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectEmergencyOptionAsync.
                break;
            // Menetapkan label cabang `case ”TAKE_SHARIA_LOAN”:` agar nilai/pola yang cocok menjalankan pernyataan pada bagian switch ini.
            case "TAKE_SHARIA_LOAN":
                // Menjalankan hasil operasi asinkron memanggil `ProjectLoanTakenAsync` dengan `request`, `participantId`, `conn`, `tx`, `ct`; await menunggu hasil
                // tanpa memblokir thread selama operasi belum selesai dalam ProjectEmergencyOptionAsync.
                await ProjectLoanTakenAsync(request, participantId, conn, tx, ct);
                // Mengakhiri loop atau cabang switch terdekat, kemudian melanjutkan setelah blok tersebut dalam ProjectEmergencyOptionAsync.
                break;
        // Menutup scope pemilihan switch atas `optionType.ToUpperInvariant()`; bagian berikut berada di luar batas blok tersebut dalam
        // ProjectEmergencyOptionAsync.
        }
    // Menutup scope metode ProjectEmergencyOptionAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectEmergencyOptionAsync.
    }

    // Mendefinisikan metode `ProjectEmergencyNeedSaleAsync` dengan hasil bertipe `Task`; operasi ini menangani project emergency kebutuhan penjualan
    // asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request`
    // bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid`
    // membawa nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca
    // hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan;
    // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
    // aplikasi berhenti.
    private async Task ProjectEmergencyNeedSaleAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectEmergencyNeedSaleAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ProjectEmergencyNeedSaleAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryGetString(request.Payload, ”card_id”, out var cardId)`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ProjectEmergencyNeedSaleAsync.
        if (!_payloadReader.TryGetString(request.Payload, "card_id", out var cardId))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryGetString(request.Payload, ”card_id”, out var cardId)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam ProjectEmergencyNeedSaleAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectEmergencyNeedSaleAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryGetString(request.Payload, ”card_id”, out var cardId)`; bagian berikut berada di luar
        // batas blok tersebut dalam ProjectEmergencyNeedSaleAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” update
        // session_participant_need_purchases purchase set is_sold = true, sold_at_day_index = @dayIndex, sold_event_id = @eventId where
        // purchase.sessio...`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai dalam ProjectEmergencyNeedSaleAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update
            // session_participant_need_purchases purchase`.
            // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set is_sold = true,`.
            // Baris literal 4: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `sold_at_day_index = @dayIndex,`.
            // Baris literal 5: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `sold_event_id = @eventId`.
            // Baris literal 6: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where purchase.session_participant_need_purchase_id = (`.
            // Baris literal 7: SELECT menentukan nilai atau kolom yang dikembalikan query: `select candidate.session_participant_need_purchase_id`.
            // Baris literal 8: FROM memilih tabel/subquery sumber pembacaan: `from session_participant_need_purchases candidate`.
            // Baris literal 9: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_needs rn on rn.ruleset_need_id =
            // candidate.ruleset_need_id`.
            // Baris literal 10: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where candidate.session_participant_id = @participantId`.
            // Baris literal 11: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(rn.need_code) = lower(@cardId)`.
            // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not candidate.is_sold`.
            // Baris literal 13: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by candidate.purchased_at_day,
            // candidate.sort_order`.
            // Baris literal 14: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
            // Baris literal 15: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 16: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            update session_participant_need_purchases purchase
            set is_sold = true,
                sold_at_day_index = @dayIndex,
                sold_event_id = @eventId
            where purchase.session_participant_need_purchase_id = (
                select candidate.session_participant_need_purchase_id
                from session_participant_need_purchases candidate
                join ruleset_needs rn on rn.ruleset_need_id = candidate.ruleset_need_id
                where candidate.session_participant_id = @participantId
                  and lower(rn.need_code) = lower(@cardId)
                  and not candidate.is_sold
                order by candidate.purchased_at_day, candidate.sort_order
                limit 1
            )
            """,
            // Meneruskan objek anonim yang mengelompokkan participantId, cardId, dayIndex, eventId sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            new { participantId, cardId, dayIndex = request.DayIndex, eventId = request.EventId },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));

        // Menyiapkan variabel lokal `soldCount` untuk nilai terjual jumlah dengan hasil operasi asinkron menjalankan perintah basis data melalui `conn`
        // dengan `new CommandDefinition( ””” select count(*)::int from session_participant_need_purchases where session_participant_id = @participantId and
        // sold_event_id = @eventId ”””, new { p...` dan mengambil nilai skalar hasilnya; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var soldCount = await conn.ExecuteScalarAsync<int>(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select count(*)::int`.
            // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from session_participant_need_purchases`.
            // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_participant_id = @participantId`.
            // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and sold_event_id = @eventId`.
            // Baris literal 6: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            select count(*)::int
            from session_participant_need_purchases
            where session_participant_id = @participantId
              and sold_event_id = @eventId
            """,
            // Meneruskan objek anonim yang mengelompokkan participantId, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new { participantId, eventId = request.EventId },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
        // Memeriksa perbandingan ketidaksamaan antara `soldCount` dan `1`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ProjectEmergencyNeedSaleAsync.
        if (soldCount != 1)
        // Membuka scope cabang if untuk kondisi `soldCount != 1`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ProjectEmergencyNeedSaleAsync.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Penjualan kebutuhan darurat harus menghapus
            // tepat satu kartu.”) dalam ProjectEmergencyNeedSaleAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Penjualan kebutuhan darurat harus menghapus tepat satu kartu.");
        // Menutup scope cabang if untuk kondisi `soldCount != 1`; bagian berikut berada di luar batas blok tersebut dalam ProjectEmergencyNeedSaleAsync.
        }

        // Menjalankan hasil operasi asinkron memanggil `RefreshMissionCompletionAsync` dengan `request.SessionId`, `participantId`, `request.EventId`,
        // `conn`, `tx`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectEmergencyNeedSaleAsync.
        await RefreshMissionCompletionAsync(request.SessionId, participantId, request.EventId, conn, tx, ct);
    // Menutup scope metode ProjectEmergencyNeedSaleAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectEmergencyNeedSaleAsync.
    }

    // Mendefinisikan metode `ProjectEmergencyGoldSaleAsync` dengan hasil bertipe `Task`; operasi ini menangani project emergency emas penjualan
    // asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request`
    // bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid`
    // membawa nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca
    // hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan;
    // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
    // aplikasi berhenti.
    private async Task ProjectEmergencyGoldSaleAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectEmergencyGoldSaleAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ProjectEmergencyGoldSaleAsync.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!_payloadReader.TryGetInt32(request.Payload, ”qty”, out var qty)` dan
        // `qty <= 0`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ProjectEmergencyGoldSaleAsync.
        if (!_payloadReader.TryGetInt32(request.Payload, "qty", out var qty) || qty <= 0)
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryGetInt32(request.Payload, ”qty”, out var qty) || qty <= 0`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam ProjectEmergencyGoldSaleAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectEmergencyGoldSaleAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryGetInt32(request.Payload, ”qty”, out var qty) || qty <= 0`; bagian berikut berada di
        // luar batas blok tersebut dalam ProjectEmergencyGoldSaleAsync.
        }

        // Menyiapkan variabel lokal `affected` untuk nilai affected dengan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new
        // CommandDefinition( ””” update session_participant_gold_holdings holding set quantity = holding.quantity - @qty, last_event_id = @eventId,
        // updated_at = now() where holding....`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var affected = await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update
            // session_participant_gold_holdings holding`.
            // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set quantity = holding.quantity - @qty,`.
            // Baris literal 4: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 5: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 6: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where holding.session_participant_id = @participantId`.
            // Baris literal 7: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and holding.ruleset_game_asset_id = (`.
            // Baris literal 8: SELECT menentukan nilai atau kolom yang dikembalikan query: `select rga.ruleset_game_asset_id`.
            // Baris literal 9: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_game_assets rga`.
            // Baris literal 10: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rga.ruleset_version_id = @rulesetVersionId`.
            // Baris literal 11: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rga.asset_type = 'GOLD'`.
            // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rga.is_active`.
            // Baris literal 13: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by rga.sort_order`.
            // Baris literal 14: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
            // Baris literal 15: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 16: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and holding.quantity >= @qty`.
            // Baris literal 17: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            update session_participant_gold_holdings holding
            set quantity = holding.quantity - @qty,
                last_event_id = @eventId,
                updated_at = now()
            where holding.session_participant_id = @participantId
              and holding.ruleset_game_asset_id = (
                  select rga.ruleset_game_asset_id
                  from ruleset_game_assets rga
                  where rga.ruleset_version_id = @rulesetVersionId
                    and rga.asset_type = 'GOLD'
                    and rga.is_active
                  order by rga.sort_order
                  limit 1
              )
              and holding.quantity >= @qty
            """,
            // Meneruskan objek anonim yang mengelompokkan participantId, rulesetVersionId, qty, eventId sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectEmergencyGoldSaleAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan participantId, rulesetVersionId, qty, eventId sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan participantId, rulesetVersionId, qty, eventId sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                rulesetVersionId = request.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan participantId, rulesetVersionId, qty, eventId sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                qty,
                // Meneruskan objek anonim yang mengelompokkan participantId, rulesetVersionId, qty, eventId sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                eventId = request.EventId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ProjectEmergencyGoldSaleAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
        // Memeriksa perbandingan ketidaksamaan antara `affected` dan `1`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ProjectEmergencyGoldSaleAsync.
        if (affected != 1)
        // Membuka scope cabang if untuk kondisi `affected != 1`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ProjectEmergencyGoldSaleAsync.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Penjualan emas darurat harus mengurangi tepat
            // satu holding.”) dalam ProjectEmergencyGoldSaleAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Penjualan emas darurat harus mengurangi tepat satu holding.");
        // Menutup scope cabang if untuk kondisi `affected != 1`; bagian berikut berada di luar batas blok tersebut dalam ProjectEmergencyGoldSaleAsync.
        }
    // Menutup scope metode ProjectEmergencyGoldSaleAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectEmergencyGoldSaleAsync.
    }

    // Mendefinisikan metode `UpdateSessionStateAsync` dengan hasil bertipe `Task`; operasi ini menangani update sesi keadaan asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid?` membawa
    // nilai participant identitas; nilai null diizinkan ketika data opsional belum tersedia; Parameter `conn` bertipe `NpgsqlConnection` membawa
    // koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data
    // yang menggabungkan perubahan sebagai satu kesatuan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat
    // dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private async Task UpdateSessionStateAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid?` membawa nilai participant identitas; nilai null diizinkan ketika data opsional belum tersedia.
        Guid? participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode UpdateSessionStateAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam UpdateSessionStateAsync.
    {
        // Menyiapkan variabel lokal `resetsActionSlots` untuk nilai resets aksi slots dengan memanggil `GameActionCatalog.Is` dengan `request.ActionType`,
        // `request.Payload`, `GameActionCatalog.AkhirGiliran`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var resetsActionSlots = GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.AkhirGiliran);
        // Menyiapkan variabel lokal `consumesAction` untuk nilai consumes aksi dengan gabungan syarat AND: kedua kondisi wajib benar antara
        // `string.Equals(request.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase)` dan
        // `GameActionCatalog.GetPlayerActionSlotPolicy(request.ActionType, request.Payload) == PlayerActionSlotPolicy.Consumes`; sisi kanan diperiksa hanya
        // jika sisi kiri benar. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var consumesAction = string.Equals(request.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
                             // Melanjutkan pengolahan dengan memanggil `GameActionCatalog.GetPlayerActionSlotPolicy` dengan `request.ActionType`, `request.Payload` dalam
                             // UpdateSessionStateAsync.
                             GameActionCatalog.GetPlayerActionSlotPolicy(request.ActionType, request.Payload) == PlayerActionSlotPolicy.Consumes;
        // Menyiapkan variabel lokal `day` untuk nomor hari permainan yang menjadi konteks aktivitas dengan menentukan nilai terbesar dari `1`,
        // `resetsActionSlots ? request.DayIndex + 1 : request.DayIndex`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var day = Math.Max(1, resetsActionSlots ? request.DayIndex + 1 : request.DayIndex);
        // Menyiapkan variabel lokal `weekday` untuk nilai weekday dengan memanggil `ResolveWeekday` dengan `day`. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var weekday = ResolveWeekday(day);
        // Menyiapkan variabel lokal `isGameOver` untuk nilai berstatus game over dengan memanggil `GameActionCatalog.Is` dengan `request.ActionType`,
        // `request.Payload`, `GameActionCatalog.SessionEnded`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var isGameOver = GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.SessionEnded);
        // Menyiapkan variabel lokal `phase` untuk nilai phase dengan memanggil `ResolvePhase` dengan `weekday`, `isGameOver`. Tipe variabel disimpulkan
        // dari ekspresi nilai awal.
        var phase = ResolvePhase(weekday, isGameOver);

        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_states (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `day,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `weekday,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `turn_number,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_slot,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `current_session_player_id,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `current_action_slot,`.
        // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_slots_left,`.
        // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `finish_day,`.
        // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `phase,`.
        // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_game_over,`.
        // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `state_version,`.
        // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id,`.
        // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ui_state_json,`.
        // Baris literal 17: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at,`.
        // Baris literal 18: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at`.
        // Baris literal 19: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 20: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionId,`.
        // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@day,`.
        // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@weekday,`.
        // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@turnNumber,`.
        // Baris literal 25: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@actionSlot,`.
        // Baris literal 26: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@participantId,`.
        // Baris literal 27: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when @resetsActionSlots then 1 when @consumesAction then
        // least(@actionSlot + 1, rgs.actions_per_turn) else 1 end,`.
        // Baris literal 28: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `case when @resetsActionSlots then rgs.actions_per_turn when
        // @consumesAction then greatest(rgs.actions_per_turn - @actionSlot, 0) else rgs.actions_per_turn end,`.
        // Baris literal 29: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rgs.finish_day,`.
        // Baris literal 30: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@phase,`.
        // Baris literal 31: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@isGameOver,`.
        // Baris literal 32: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `1,`.
        // Baris literal 33: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@eventId,`.
        // Baris literal 34: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}'::jsonb,`.
        // Baris literal 35: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now(),`.
        // Baris literal 36: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 37: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_game_settings rgs`.
        // Baris literal 38: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rgs.ruleset_version_id = @rulesetVersionId`.
        // Baris literal 39: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict (session_id) do
        // update`.
        // Baris literal 40: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set day = excluded.day,`.
        // Baris literal 41: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `weekday =
        // excluded.weekday,`.
        // Baris literal 42: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `turn_number =
        // excluded.turn_number,`.
        // Baris literal 43: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_slot =
        // excluded.action_slot,`.
        // Baris literal 44: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `current_session_player_id =
        // coalesce(excluded.current_session_player_id, session_states.current_session_player_id),`.
        // Baris literal 45: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `current_action_slot = case when @resetsActionSlots or @consumesAction then excluded.current_action_slot else
        // session_states.current_action_slot end,`.
        // Baris literal 46: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `action_slots_left = case when @resetsActionSlots or @consumesAction then excluded.action_slots_left else session_states.action_slots_left
        // end,`.
        // Baris literal 47: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `finish_day =
        // excluded.finish_day,`.
        // Baris literal 48: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `phase = excluded.phase,`.
        // Baris literal 49: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_game_over =
        // excluded.is_game_over,`.
        // Baris literal 50: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `state_version =
        // session_states.state_version + 1,`.
        // Baris literal 51: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id =
        // excluded.last_event_id,`.
        // Baris literal 52: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
        // Baris literal 53: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            insert into session_states (
                session_id,
                day,
                weekday,
                turn_number,
                action_slot,
                current_session_player_id,
                current_action_slot,
                action_slots_left,
                finish_day,
                phase,
                is_game_over,
                state_version,
                last_event_id,
                ui_state_json,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @day,
                @weekday,
                @turnNumber,
                @actionSlot,
                @participantId,
                case when @resetsActionSlots then 1 when @consumesAction then least(@actionSlot + 1, rgs.actions_per_turn) else 1 end,
                case when @resetsActionSlots then rgs.actions_per_turn when @consumesAction then greatest(rgs.actions_per_turn - @actionSlot, 0) else rgs.actions_per_turn end,
                rgs.finish_day,
                @phase,
                @isGameOver,
                1,
                @eventId,
                '{}'::jsonb,
                now(),
                now()
            from ruleset_game_settings rgs
            where rgs.ruleset_version_id = @rulesetVersionId
            on conflict (session_id) do update
            set day = excluded.day,
                weekday = excluded.weekday,
                turn_number = excluded.turn_number,
                action_slot = excluded.action_slot,
                current_session_player_id = coalesce(excluded.current_session_player_id, session_states.current_session_player_id),
                current_action_slot = case when @resetsActionSlots or @consumesAction then excluded.current_action_slot else session_states.current_action_slot end,
                action_slots_left = case when @resetsActionSlots or @consumesAction then excluded.action_slots_left else session_states.action_slots_left end,
                finish_day = excluded.finish_day,
                phase = excluded.phase,
                is_game_over = excluded.is_game_over,
                state_version = session_states.state_version + 1,
                last_event_id = excluded.last_event_id,
                updated_at = now()
            """;

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( sql, new { sessionId =
        // request.SessionId, day, weekday, turnNumber = request.ActorType.Equals(”SYSTEM”, StringComparison.OrdinalIgnoreCase) ? 0 : reques...`; nilai
        // hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // UpdateSessionStateAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`.
            sql,
            // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turnNumber, actionSlot, participantId, resetsActionSlots, consumesAction,
            // rulesetVersionId, phase, isGameOver, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // UpdateSessionStateAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turnNumber, actionSlot, participantId, resetsActionSlots, consumesAction,
                // rulesetVersionId, phase, isGameOver, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                sessionId = request.SessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turnNumber, actionSlot, participantId, resetsActionSlots, consumesAction,
                // rulesetVersionId, phase, isGameOver, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                day,
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turnNumber, actionSlot, participantId, resetsActionSlots, consumesAction,
                // rulesetVersionId, phase, isGameOver, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                weekday,
                // Meneruskan nilai literal `”SYSTEM”` sebagai argumen ke `request.ActorType.Equals`; Meneruskan `StringComparison.OrdinalIgnoreCase` (nilai ordinal
                // ignore case) sebagai argumen ke `request.ActorType.Equals`.
                turnNumber = request.ActorType.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase) ? 0 : request.TurnNumber,
                // Meneruskan nilai literal `1` sebagai argumen ke `Math.Max`; Meneruskan `request.ActionSlot` (nilai aksi slot) sebagai argumen ke `Math.Max`.
                actionSlot = Math.Max(1, request.ActionSlot),
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turnNumber, actionSlot, participantId, resetsActionSlots, consumesAction,
                // rulesetVersionId, phase, isGameOver, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turnNumber, actionSlot, participantId, resetsActionSlots, consumesAction,
                // rulesetVersionId, phase, isGameOver, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                resetsActionSlots,
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turnNumber, actionSlot, participantId, resetsActionSlots, consumesAction,
                // rulesetVersionId, phase, isGameOver, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                consumesAction,
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turnNumber, actionSlot, participantId, resetsActionSlots, consumesAction,
                // rulesetVersionId, phase, isGameOver, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                rulesetVersionId = request.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turnNumber, actionSlot, participantId, resetsActionSlots, consumesAction,
                // rulesetVersionId, phase, isGameOver, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                phase,
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turnNumber, actionSlot, participantId, resetsActionSlots, consumesAction,
                // rulesetVersionId, phase, isGameOver, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                isGameOver,
                // Meneruskan objek anonim yang mengelompokkan sessionId, day, weekday, turnNumber, actionSlot, participantId, resetsActionSlots, consumesAction,
                // rulesetVersionId, phase, isGameOver, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                eventId = request.EventId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam UpdateSessionStateAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode UpdateSessionStateAsync; bagian berikut berada di luar batas blok tersebut dalam UpdateSessionStateAsync.
    }

    // Mendefinisikan metode `ResolveWeekday` dengan hasil bertipe `string`; operasi ini menangani resolve weekday. Masukan: Parameter `day` bertipe
    // `int` membawa nomor hari permainan yang menjadi konteks aktivitas. Nilai hasil langsung berasal dari hasil pemetaan `(((day - 1) % 7 + 7) % 7)`
    // melalui cabang pola switch yang cocok.
    private static string ResolveWeekday(int day)
        // Melengkapi struktur ekspresi ArrowExpressionClause melalui => (((day - 1) % 7 + 7) % 7) switch dalam ResolveWeekday; token pada baris ini
        // menyambungkan bagian kode sebelum dan sesudahnya.
        => (((day - 1) % 7 + 7) % 7) switch
        // Membuka scope pemetaan switch atas `(((day - 1) % 7 + 7) % 7)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolveWeekday.
        {
            // Untuk pola `0`, menghasilkan nilai literal `”MON”` sebagai hasil switch.
            0 => "MON",
            // Untuk pola `1`, menghasilkan nilai literal `”TUE”` sebagai hasil switch.
            1 => "TUE",
            // Untuk pola `2`, menghasilkan nilai literal `”WED”` sebagai hasil switch.
            2 => "WED",
            // Untuk pola `3`, menghasilkan nilai literal `”THU”` sebagai hasil switch.
            3 => "THU",
            // Untuk pola `4`, menghasilkan nilai literal `”FRI”` sebagai hasil switch.
            4 => "FRI",
            // Untuk pola `5`, menghasilkan nilai literal `”SAT”` sebagai hasil switch.
            5 => "SAT",
            // Untuk pola `_`, menghasilkan nilai literal `”SUN”` sebagai hasil switch.
            _ => "SUN"
        // Menutup scope pemetaan switch atas `(((day - 1) % 7 + 7) % 7)`; bagian berikut berada di luar batas blok tersebut dalam ResolveWeekday.
        };

    // Mendefinisikan metode `EnsureParticipantBalanceAsync` dengan hasil bertipe `Task`; operasi ini menangani ensure participant saldo asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private static async Task EnsureParticipantBalanceAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode EnsureParticipantBalanceAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // EnsureParticipantBalanceAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_balances (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coins,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `happiness,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `saving,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `total_donasi,`.
        // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id,`.
        // Baris literal 10: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at,`.
        // Baris literal 11: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at`.
        // Baris literal 12: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 13: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionId,`.
        // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@participantId,`.
        // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rgs.starting_cash,`.
        // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rgs.starting_happiness,`.
        // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rgs.starting_saving,`.
        // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `0,`.
        // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@eventId,`.
        // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now(),`.
        // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 23: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_game_settings rgs`.
        // Baris literal 24: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rgs.ruleset_version_id = @rulesetVersionId`.
        // Baris literal 25: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict
        // (session_participant_id) do nothing`.
        // Baris literal 26: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            insert into session_participant_balances (
                session_id,
                session_participant_id,
                coins,
                happiness,
                saving,
                total_donasi,
                last_event_id,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @participantId,
                rgs.starting_cash,
                rgs.starting_happiness,
                rgs.starting_saving,
                0,
                @eventId,
                now(),
                now()
            from ruleset_game_settings rgs
            where rgs.ruleset_version_id = @rulesetVersionId
            on conflict (session_participant_id) do nothing
            """;

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( sql, new { sessionId =
        // request.SessionId, participantId, rulesetVersionId = request.RulesetVersionId, eventId = request.EventId }, tx, cancellationToken...`; nilai
        // hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // EnsureParticipantBalanceAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`.
            sql,
            // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, eventId sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // EnsureParticipantBalanceAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, eventId sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                sessionId = request.SessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, eventId sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, eventId sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                rulesetVersionId = request.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, eventId sebagai satu nilai sebagai argumen ke konstruktor
                // `CommandDefinition`.
                eventId = request.EventId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // EnsureParticipantBalanceAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode EnsureParticipantBalanceAsync; bagian berikut berada di luar batas blok tersebut dalam EnsureParticipantBalanceAsync.
    }

    // Mendefinisikan metode `ApplyCashflowAsync` dengan hasil bertipe `Task`; operasi ini menangani apply arus kas asinkron. async memungkinkan metode
    // menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `participantId` bertipe `Guid` membawa nilai
    // participant identitas; Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter
    // `projections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event permainan;
    // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tx`
    // bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task ApplyCashflowAsync(
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `projections` bertipe `IReadOnlyCollection<CashflowProjectionDb>` membawa proyeksi transaksi arus kas yang diturunkan dari event
        // permainan.
        IReadOnlyCollection<CashflowProjectionDb> projections,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ApplyCashflowAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyCashflowAsync.
    {
        // Menyiapkan variabel lokal `delta` untuk nilai delta dengan menjumlahkan nilai `projections` berdasarkan `item => string.Equals(item.Direction,
        // ”IN”, StringComparison.OrdinalIgnoreCase) ? item.Amount : -item.Amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var delta = projections.Sum(item => string.Equals(item.Direction, "IN", StringComparison.OrdinalIgnoreCase)
            // Meneruskan fungsi lambda `item => string.Equals(item.Direction, ”IN”, StringComparison.OrdinalIgnoreCase) ? item.Amount : -item.Amount` yang
            // dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `projections.Sum`.
            ? item.Amount
            // Meneruskan fungsi lambda `item => string.Equals(item.Direction, ”IN”, StringComparison.OrdinalIgnoreCase) ? item.Amount : -item.Amount` yang
            // dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `projections.Sum`.
            : -item.Amount);
        // Memeriksa perbandingan kesamaan antara `delta` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ApplyCashflowAsync.
        if (delta == 0)
        // Membuka scope cabang if untuk kondisi `delta == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ApplyCashflowAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ApplyCashflowAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `delta == 0`; bagian berikut berada di luar batas blok tersebut dalam ApplyCashflowAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” update
        // session_participant_balances set coins = coins + @delta, last_event_id = @eventId, updated_at = now() where session_participant_id =
        // @partici...`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai
        // dalam ApplyCashflowAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update
            // session_participant_balances`.
            // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set coins = coins + @delta,`.
            // Baris literal 4: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 5: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 6: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_participant_id = @participantId`.
            // Baris literal 7: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            update session_participant_balances
            set coins = coins + @delta,
                last_event_id = @eventId,
                updated_at = now()
            where session_participant_id = @participantId
            """,
            // Meneruskan objek anonim yang mengelompokkan participantId, delta, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new { participantId, delta, eventId },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode ApplyCashflowAsync; bagian berikut berada di luar batas blok tersebut dalam ApplyCashflowAsync.
    }

    // Mendefinisikan metode `IncrementActionCounterAsync` dengan hasil bertipe `Task`; operasi ini menangani increment aksi counter asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `participantId` bertipe `Guid` membawa nilai participant
    // identitas; Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat;
    // Parameter `rulesetActionId` bertipe `Guid` membawa nilai aturan aksi identitas; Parameter `eventId` bertipe `Guid` membawa identitas unik event
    // untuk pencatatan dan pemeriksaan duplikasi; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan
    // membaca hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu
    // kesatuan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan
    // permintaan atau aplikasi berhenti.
    private static async Task IncrementActionCounterAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat.
        Guid rulesetVersionId,
        // Parameter `rulesetActionId` bertipe `Guid` membawa nilai aturan aksi identitas.
        Guid rulesetActionId,
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode IncrementActionCounterAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IncrementActionCounterAsync.
    {
        // Memeriksa perbandingan kesamaan antara `rulesetActionId` dan `Guid.Empty`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // IncrementActionCounterAsync.
        if (rulesetActionId == Guid.Empty)
        // Membuka scope cabang if untuk kondisi `rulesetActionId == Guid.Empty`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // IncrementActionCounterAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam IncrementActionCounterAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `rulesetActionId == Guid.Empty`; bagian berikut berada di luar batas blok tersebut dalam
        // IncrementActionCounterAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” insert into
        // session_participant_action_counters ( session_id, session_participant_id, ruleset_version_id, ruleset_action_id, count, last_event_id ) ...`;
        // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // IncrementActionCounterAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_action_counters (`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_action_id,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `count,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id`.
            // Baris literal 9: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 10: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values
            // (@sessionId, @participantId, @rulesetVersionId, @rulesetActionId, 1, @eventId)`.
            // Baris literal 11: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict
            // (session_participant_id, ruleset_action_id) do update`.
            // Baris literal 12: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set count = session_participant_action_counters.count + 1,`.
            // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id =
            // excluded.last_event_id`.
            // Baris literal 14: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            insert into session_participant_action_counters (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_action_id,
                count,
                last_event_id
            )
            values (@sessionId, @participantId, @rulesetVersionId, @rulesetActionId, 1, @eventId)
            on conflict (session_participant_id, ruleset_action_id) do update
            set count = session_participant_action_counters.count + 1,
                last_event_id = excluded.last_event_id
            """,
            // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, rulesetActionId, eventId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            new { sessionId, participantId, rulesetVersionId, rulesetActionId, eventId },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode IncrementActionCounterAsync; bagian berikut berada di luar batas blok tersebut dalam IncrementActionCounterAsync.
    }

    // Mendefinisikan metode `ProjectNarrativeTriggersAsync` dengan hasil bertipe `Task`; operasi ini menangani project narrative triggers asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `rulesetActionId` bertipe `Guid` membawa nilai aturan aksi identitas; Parameter `eventId` bertipe `Guid`
    // membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL
    // untuk mengirim perintah dan membaca hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan
    // perubahan sebagai satu kesatuan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika
    // pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task ProjectNarrativeTriggersAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `rulesetActionId` bertipe `Guid` membawa nilai aturan aksi identitas.
        Guid rulesetActionId,
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectNarrativeTriggersAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ProjectNarrativeTriggersAsync.
    {
        // Memeriksa perbandingan kesamaan antara `rulesetActionId` dan `Guid.Empty`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ProjectNarrativeTriggersAsync.
        if (rulesetActionId == Guid.Empty)
        // Membuka scope cabang if untuk kondisi `rulesetActionId == Guid.Empty`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ProjectNarrativeTriggersAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectNarrativeTriggersAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `rulesetActionId == Guid.Empty`; bagian berikut berada di luar batas blok tersebut dalam
        // ProjectNarrativeTriggersAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” insert into
        // session_narrative_logs ( narrative_log_id, session_id, session_participant_id, ruleset_version_id, ruleset_narrative_id, ruleset_narrati...`;
        // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ProjectNarrativeTriggersAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_narrative_logs (`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `narrative_log_id,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_narrative_id,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_narrative_scene_id,`.
            // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `source_event_id,`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `shown_at,`.
            // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `day,`.
            // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `action_slot,`.
            // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `payload_json`.
            // Baris literal 14: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 15: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `gen_random_uuid(),`.
            // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@sessionId,`.
            // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@participantId,`.
            // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@rulesetVersionId,`.
            // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rtc.ruleset_narrative_id,`.
            // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `scene.ruleset_narrative_scene_id,`.
            // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@eventId,`.
            // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@eventTimestamp,`.
            // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@day,`.
            // Baris literal 25: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@actionSlot,`.
            // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `jsonb_build_object(`.
            // Baris literal 27: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'trigger_condition_id',
            // rtc.ruleset_trigger_condition_id,`.
            // Baris literal 28: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'ruleset_action_id',
            // rtc.ruleset_action_id`.
            // Baris literal 29: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 30: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_trigger_conditions rtc`.
            // Baris literal 31: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `left join lateral (`.
            // Baris literal 32: SELECT menentukan nilai atau kolom yang dikembalikan query: `select rns.ruleset_narrative_scene_id`.
            // Baris literal 33: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_narrative_scenes rns`.
            // Baris literal 34: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rns.ruleset_version_id = rtc.ruleset_version_id`.
            // Baris literal 35: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rns.ruleset_narrative_id =
            // rtc.ruleset_narrative_id`.
            // Baris literal 36: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by rns.scene_order asc`.
            // Baris literal 37: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
            // Baris literal 38: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) scene on true`.
            // Baris literal 39: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rtc.ruleset_version_id = @rulesetVersionId`.
            // Baris literal 40: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rtc.trigger_owner_type = 'NARRATIVE'`.
            // Baris literal 41: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rtc.ruleset_action_id = @rulesetActionId`.
            // Baris literal 42: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rtc.is_active`.
            // Baris literal 43: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rtc.threshold_numeric <= (`.
            // Baris literal 44: SELECT menentukan nilai atau kolom yang dikembalikan query: `select count(*)`.
            // Baris literal 45: FROM memilih tabel/subquery sumber pembacaan: `from events e`.
            // Baris literal 46: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where e.session_id = @sessionId`.
            // Baris literal 47: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.session_player_id = @participantId`.
            // Baris literal 48: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and e.ruleset_action_id = @rulesetActionId`.
            // Baris literal 49: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 50: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict (`.
            // Baris literal 51: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 52: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
            // Baris literal 53: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_narrative_id,`.
            // Baris literal 54: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `source_event_id`.
            // Baris literal 55: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `) do nothing`.
            // Baris literal 56: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            insert into session_narrative_logs (
                narrative_log_id,
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_narrative_id,
                ruleset_narrative_scene_id,
                source_event_id,
                shown_at,
                day,
                action_slot,
                payload_json
            )
            select
                gen_random_uuid(),
                @sessionId,
                @participantId,
                @rulesetVersionId,
                rtc.ruleset_narrative_id,
                scene.ruleset_narrative_scene_id,
                @eventId,
                @eventTimestamp,
                @day,
                @actionSlot,
                jsonb_build_object(
                    'trigger_condition_id', rtc.ruleset_trigger_condition_id,
                    'ruleset_action_id', rtc.ruleset_action_id
                )
            from ruleset_trigger_conditions rtc
            left join lateral (
                select rns.ruleset_narrative_scene_id
                from ruleset_narrative_scenes rns
                where rns.ruleset_version_id = rtc.ruleset_version_id
                  and rns.ruleset_narrative_id = rtc.ruleset_narrative_id
                order by rns.scene_order asc
                limit 1
            ) scene on true
            where rtc.ruleset_version_id = @rulesetVersionId
              and rtc.trigger_owner_type = 'NARRATIVE'
              and rtc.ruleset_action_id = @rulesetActionId
              and rtc.is_active
              and rtc.threshold_numeric <= (
                  select count(*)
                  from events e
                  where e.session_id = @sessionId
                    and e.session_player_id = @participantId
                    and e.ruleset_action_id = @rulesetActionId
              )
            on conflict (
                session_id,
                session_participant_id,
                ruleset_narrative_id,
                source_event_id
            ) do nothing
            """,
            // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, rulesetActionId, eventId, eventTimestamp, day, actionSlot
            // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectNarrativeTriggersAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, rulesetActionId, eventId, eventTimestamp, day, actionSlot
                // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                sessionId = request.SessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, rulesetActionId, eventId, eventTimestamp, day, actionSlot
                // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, rulesetActionId, eventId, eventTimestamp, day, actionSlot
                // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                rulesetVersionId = request.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, rulesetActionId, eventId, eventTimestamp, day, actionSlot
                // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                rulesetActionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, rulesetActionId, eventId, eventTimestamp, day, actionSlot
                // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                eventId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, rulesetActionId, eventId, eventTimestamp, day, actionSlot
                // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                eventTimestamp = request.Timestamp.ToUniversalTime(),
                // Meneruskan nilai literal `1` sebagai argumen ke `Math.Max`; Meneruskan penjumlahan/penggabungan antara `request.DayIndex` dan `1` sebagai argumen
                // ke `Math.Max`.
                day = Math.Max(1, request.DayIndex + 1),
                // Meneruskan nilai literal `1` sebagai argumen ke `Math.Max`; Meneruskan `request.ActionSlot` (nilai aksi slot) sebagai argumen ke `Math.Max`.
                actionSlot = Math.Max(1, request.ActionSlot)
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ProjectNarrativeTriggersAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode ProjectNarrativeTriggersAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectNarrativeTriggersAsync.
    }

    // Mendefinisikan metode `ProjectIngredientPurchaseAsync` dengan hasil bertipe `Task`; operasi ini menangani project bahan pembelian asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private async Task ProjectIngredientPurchaseAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectIngredientPurchaseAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ProjectIngredientPurchaseAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadIngredientPurchase(request.Payload, out var cardId, out _)`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam ProjectIngredientPurchaseAsync.
        if (!_payloadReader.TryReadIngredientPurchase(request.Payload, out var cardId, out _))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadIngredientPurchase(request.Payload, out var cardId, out _)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam ProjectIngredientPurchaseAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectIngredientPurchaseAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadIngredientPurchase(request.Payload, out var cardId, out _)`; bagian berikut berada
        // di luar batas blok tersebut dalam ProjectIngredientPurchaseAsync.
        }

        // Menjalankan hasil operasi asinkron memanggil `ChangeIngredientQuantityAsync` dengan `request.SessionId`, `request.RulesetVersionId`,
        // `participantId`, `cardId`, `1`, `request.EventId`, `conn`, `tx`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai
        // dalam ProjectIngredientPurchaseAsync.
        await ChangeIngredientQuantityAsync(request.SessionId, request.RulesetVersionId, participantId, cardId, 1, request.EventId, conn, tx, ct);
    // Menutup scope metode ProjectIngredientPurchaseAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectIngredientPurchaseAsync.
    }

    // Mendefinisikan metode `ProjectIngredientDiscardAsync` dengan hasil bertipe `Task`; operasi ini menangani project bahan discard asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private async Task ProjectIngredientDiscardAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectIngredientDiscardAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ProjectIngredientDiscardAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryGetString(request.Payload, ”card_id”, out var cardId)`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ProjectIngredientDiscardAsync.
        if (!_payloadReader.TryGetString(request.Payload, "card_id", out var cardId))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryGetString(request.Payload, ”card_id”, out var cardId)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam ProjectIngredientDiscardAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectIngredientDiscardAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryGetString(request.Payload, ”card_id”, out var cardId)`; bagian berikut berada di luar
        // batas blok tersebut dalam ProjectIngredientDiscardAsync.
        }

        // Menyiapkan variabel lokal `quantity` untuk nilai jumlah dengan hasil pemilihan bersyarat: ketika `_payloadReader.TryGetInt32(request.Payload,
        // ”amount”, out var amount)` benar gunakan `amount`, jika tidak gunakan `1`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var quantity = _payloadReader.TryGetInt32(request.Payload, "amount", out var amount) ? amount : 1;
        // Menjalankan hasil operasi asinkron memanggil `ChangeIngredientQuantityAsync` dengan `request.SessionId`, `request.RulesetVersionId`,
        // `participantId`, `cardId`, `-Math.Max(1, quantity)`, `request.EventId`, `conn`, `tx`, `ct`; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai dalam ProjectIngredientDiscardAsync.
        await ChangeIngredientQuantityAsync(request.SessionId, request.RulesetVersionId, participantId, cardId, -Math.Max(1, quantity), request.EventId, conn, tx, ct);
    // Menutup scope metode ProjectIngredientDiscardAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectIngredientDiscardAsync.
    }

    // Mendefinisikan metode `ProjectOrderClaimAsync` dengan hasil bertipe `Task`; operasi ini menangani project urutan/pesanan claim asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private async Task ProjectOrderClaimAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectOrderClaimAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectOrderClaimAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadOrderClaim(request.Payload, out var requiredCards, out _)`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam ProjectOrderClaimAsync.
        if (!_payloadReader.TryReadOrderClaim(request.Payload, out var requiredCards, out _))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadOrderClaim(request.Payload, out var requiredCards, out _)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam ProjectOrderClaimAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectOrderClaimAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadOrderClaim(request.Payload, out var requiredCards, out _)`; bagian berikut berada
        // di luar batas blok tersebut dalam ProjectOrderClaimAsync.
        }

        // Mengulangi setiap elemen `requiredCards`; elemen saat ini disimpan sebagai `cardId` bertipe `var` untuk diproses oleh badan loop dalam
        // ProjectOrderClaimAsync.
        foreach (var cardId in requiredCards)
        // Membuka scope loop setiap cardId dari `requiredCards`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectOrderClaimAsync.
        {
            // Menjalankan hasil operasi asinkron memanggil `ChangeIngredientQuantityAsync` dengan `request.SessionId`, `request.RulesetVersionId`,
            // `participantId`, `cardId`, `-1`, `request.EventId`, `conn`, `tx`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai
            // dalam ProjectOrderClaimAsync.
            await ChangeIngredientQuantityAsync(request.SessionId, request.RulesetVersionId, participantId, cardId, -1, request.EventId, conn, tx, ct);
        // Menutup scope loop setiap cardId dari `requiredCards`; bagian berikut berada di luar batas blok tersebut dalam ProjectOrderClaimAsync.
        }
    // Menutup scope metode ProjectOrderClaimAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectOrderClaimAsync.
    }

    // Mendefinisikan metode `ChangeIngredientQuantityAsync` dengan hasil bertipe `Task`; operasi ini menangani change bahan jumlah asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe `Guid`
    // membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi
    // aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; Parameter `participantId` bertipe `Guid` membawa nilai participant identitas;
    // Parameter `ingredientId` bertipe `string` membawa nilai bahan identitas; Parameter `delta` bertipe `int` membawa nilai delta; Parameter `eventId`
    // bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter `conn` bertipe `NpgsqlConnection` membawa
    // koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data
    // yang menggabungkan perubahan sebagai satu kesatuan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat
    // dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task ChangeIngredientQuantityAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat.
        Guid rulesetVersionId,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `ingredientId` bertipe `string` membawa nilai bahan identitas.
        string ingredientId,
        // Parameter `delta` bertipe `int` membawa nilai delta.
        int delta,
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ChangeIngredientQuantityAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ChangeIngredientQuantityAsync.
    {
        // Menyiapkan variabel lokal `sql` untuk nilai SQL dengan literal multiline yang dirinci pada komentar di dekat deklarasinya. Tipe yang dipakai
        // adalah `string`.
        // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
        // Baris literal 1: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `const string sql = ”””`.
        // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_inventory (`.
        // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
        // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
        // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
        // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_game_asset_id,`.
        // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `qty,`.
        // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id,`.
        // Baris literal 9: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at,`.
        // Baris literal 10: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at`.
        // Baris literal 11: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 12: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
        // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@sessionId,`.
        // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@participantId,`.
        // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rga.ruleset_version_id,`.
        // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rga.ruleset_game_asset_id,`.
        // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `greatest(0, @delta),`.
        // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `@eventId,`.
        // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now(),`.
        // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
        // Baris literal 21: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_game_assets rga`.
        // Baris literal 22: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rga.ruleset_version_id = @rulesetVersionId`.
        // Baris literal 23: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rga.asset_type = 'INGREDIENT'`.
        // Baris literal 24: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (`.
        // Baris literal 25: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `lower(rga.asset_code) = lower(@ingredientId)`.
        // Baris literal 26: Melanjutkan kondisi SQL dengan OR (alternatif syarat yang dapat terpenuhi): `or lower(rga.display_name) =
        // lower(@ingredientId)`.
        // Baris literal 27: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        // Baris literal 28: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rga.is_active`.
        // Baris literal 29: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
        // Baris literal 30: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict
        // (session_participant_id, ruleset_game_asset_id) do update`.
        // Baris literal 31: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set qty = greatest(0, session_participant_inventory.qty + @delta),`.
        // Baris literal 32: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
        // SQL: `last_event_id = @eventId,`.
        // Baris literal 33: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
        // Baris literal 34: Pembatas literal/penutup `”””;`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
        const string sql = """
            insert into session_participant_inventory (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                qty,
                last_event_id,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @participantId,
                rga.ruleset_version_id,
                rga.ruleset_game_asset_id,
                greatest(0, @delta),
                @eventId,
                now(),
                now()
            from ruleset_game_assets rga
            where rga.ruleset_version_id = @rulesetVersionId
              and rga.asset_type = 'INGREDIENT'
              and (
                lower(rga.asset_code) = lower(@ingredientId)
                or lower(rga.display_name) = lower(@ingredientId)
              )
              and rga.is_active
            limit 1
            on conflict (session_participant_id, ruleset_game_asset_id) do update
            set qty = greatest(0, session_participant_inventory.qty + @delta),
                last_event_id = @eventId,
                updated_at = now()
            """;

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( sql, new { sessionId,
        // rulesetVersionId, participantId, ingredientId, delta, eventId }, tx, cancellationToken: ct)`; nilai hasil menunjukkan jumlah baris yang
        // terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ChangeIngredientQuantityAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan `sql` (nilai SQL) sebagai argumen ke konstruktor `CommandDefinition`.
            sql,
            // Meneruskan objek anonim yang mengelompokkan sessionId, rulesetVersionId, participantId, ingredientId, delta, eventId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            new { sessionId, rulesetVersionId, participantId, ingredientId, delta, eventId },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode ChangeIngredientQuantityAsync; bagian berikut berada di luar batas blok tersebut dalam ChangeIngredientQuantityAsync.
    }

    // Mendefinisikan metode `ProjectNeedPurchaseAsync` dengan hasil bertipe `Task`; operasi ini menangani project kebutuhan pembelian asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private async Task ProjectNeedPurchaseAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectNeedPurchaseAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectNeedPurchaseAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadNeedPurchase(request.Payload, out var cardId, out var amount, out var points)`; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam ProjectNeedPurchaseAsync.
        if (!_payloadReader.TryReadNeedPurchase(request.Payload, out var cardId, out var amount, out var points))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadNeedPurchase(request.Payload, out var cardId, out var amount, out var points)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectNeedPurchaseAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectNeedPurchaseAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadNeedPurchase(request.Payload, out var cardId, out var amount, out var points)`;
        // bagian berikut berada di luar batas blok tersebut dalam ProjectNeedPurchaseAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” insert into
        // session_participant_need_purchases ( session_participant_need_purchase_id, session_id, session_participant_id, ruleset_version_id, rules...`;
        // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ProjectNeedPurchaseAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_need_purchases (`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `session_participant_need_purchase_id,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_need_id,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `sort_order,`.
            // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `paid_amount,`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `happiness_delta,`.
            // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `purchased_at_day,`.
            // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `source_event_id,`.
            // Baris literal 13: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
            // Baris literal 14: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 15: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@entryId,`.
            // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@sessionId,`.
            // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@participantId,`.
            // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rn.ruleset_version_id,`.
            // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rn.ruleset_need_id,`.
            // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `coalesce((`.
            // Baris literal 22: SELECT menentukan nilai atau kolom yang dikembalikan query: `select max(existing.sort_order) + 1`.
            // Baris literal 23: FROM memilih tabel/subquery sumber pembacaan: `from session_participant_need_purchases existing`.
            // Baris literal 24: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where existing.session_participant_id = @participantId`.
            // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `), 1),`.
            // Baris literal 26: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@amount,`.
            // Baris literal 27: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@points,`.
            // Baris literal 28: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@day,`.
            // Baris literal 29: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@eventId,`.
            // Baris literal 30: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
            // Baris literal 31: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_needs rn`.
            // Baris literal 32: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rn.ruleset_version_id = @rulesetVersionId`.
            // Baris literal 33: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(rn.need_code) = lower(@cardId)`.
            // Baris literal 34: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rn.is_active`.
            // Baris literal 35: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
            // Baris literal 36: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            insert into session_participant_need_purchases (
                session_participant_need_purchase_id,
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_need_id,
                sort_order,
                paid_amount,
                happiness_delta,
                purchased_at_day,
                source_event_id,
                created_at
            )
            select
                @entryId,
                @sessionId,
                @participantId,
                rn.ruleset_version_id,
                rn.ruleset_need_id,
                coalesce((
                    select max(existing.sort_order) + 1
                    from session_participant_need_purchases existing
                    where existing.session_participant_id = @participantId
                ), 1),
                @amount,
                @points,
                @day,
                @eventId,
                now()
            from ruleset_needs rn
            where rn.ruleset_version_id = @rulesetVersionId
              and lower(rn.need_code) = lower(@cardId)
              and rn.is_active
            limit 1
            """,
            // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, participantId, rulesetVersionId, cardId, amount, points, day, eventId sebagai
            // satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectNeedPurchaseAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, participantId, rulesetVersionId, cardId, amount, points, day, eventId sebagai
                // satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                entryId = Guid.NewGuid(),
                // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, participantId, rulesetVersionId, cardId, amount, points, day, eventId sebagai
                // satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                sessionId = request.SessionId,
                // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, participantId, rulesetVersionId, cardId, amount, points, day, eventId sebagai
                // satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, participantId, rulesetVersionId, cardId, amount, points, day, eventId sebagai
                // satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                rulesetVersionId = request.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, participantId, rulesetVersionId, cardId, amount, points, day, eventId sebagai
                // satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                cardId,
                // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, participantId, rulesetVersionId, cardId, amount, points, day, eventId sebagai
                // satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                amount,
                // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, participantId, rulesetVersionId, cardId, amount, points, day, eventId sebagai
                // satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                points,
                // Meneruskan nilai literal `1` sebagai argumen ke `Math.Max`; Meneruskan penjumlahan/penggabungan antara `request.DayIndex` dan `1` sebagai argumen
                // ke `Math.Max`.
                day = Math.Max(1, request.DayIndex + 1),
                // Meneruskan objek anonim yang mengelompokkan entryId, sessionId, participantId, rulesetVersionId, cardId, amount, points, day, eventId sebagai
                // satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                eventId = request.EventId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ProjectNeedPurchaseAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));

        // Menjalankan hasil operasi asinkron memanggil `AddHappinessAsync` dengan `participantId`, `points`, `request.EventId`, `conn`, `tx`, `ct`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectNeedPurchaseAsync.
        await AddHappinessAsync(participantId, points, request.EventId, conn, tx, ct);
        // Menjalankan hasil operasi asinkron memanggil `RefreshMissionCompletionAsync` dengan `request.SessionId`, `participantId`, `request.EventId`,
        // `conn`, `tx`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectNeedPurchaseAsync.
        await RefreshMissionCompletionAsync(request.SessionId, participantId, request.EventId, conn, tx, ct);
    // Menutup scope metode ProjectNeedPurchaseAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectNeedPurchaseAsync.
    }

    // Mendefinisikan metode `ProjectMissionAssignmentAsync` dengan hasil bertipe `Task`; operasi ini menangani project misi assignment asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private async Task ProjectMissionAssignmentAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectMissionAssignmentAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ProjectMissionAssignmentAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadMissionAssigned(request.Payload, out var missionId, out _, out _)`; blok if hanya dijalankan
        // ketika kondisi ini bernilai benar dalam ProjectMissionAssignmentAsync.
        if (!_payloadReader.TryReadMissionAssigned(request.Payload, out var missionId, out _, out _))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadMissionAssigned(request.Payload, out var missionId, out _, out _)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectMissionAssignmentAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectMissionAssignmentAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadMissionAssigned(request.Payload, out var missionId, out _, out _)`; bagian berikut
        // berada di luar batas blok tersebut dalam ProjectMissionAssignmentAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” insert into
        // session_participant_collection_missions ( session_id, session_participant_id, ruleset_version_id, ruleset_collection_mission_id, is_comp...`;
        // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ProjectMissionAssignmentAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_collection_missions (`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `ruleset_collection_mission_id,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_completed,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_failed,`.
            // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `reward_applied,`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id,`.
            // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `assigned_at`.
            // Baris literal 12: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 13: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@sessionId,`.
            // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@participantId,`.
            // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rcm.ruleset_version_id,`.
            // Baris literal 17: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `rcm.ruleset_collection_mission_id,`.
            // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `false,`.
            // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `false,`.
            // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `false,`.
            // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@eventId,`.
            // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@assignedAt`.
            // Baris literal 23: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_collection_missions rcm`.
            // Baris literal 24: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rcm.ruleset_version_id = @rulesetVersionId`.
            // Baris literal 25: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(rcm.mission_code) = lower(@missionId)`.
            // Baris literal 26: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rcm.is_active`.
            // Baris literal 27: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
            // Baris literal 28: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict
            // (session_participant_id, ruleset_collection_mission_id) do update`.
            // Baris literal 29: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set assigned_at = excluded.assigned_at,`.
            // Baris literal 30: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_completed = false,`.
            // Baris literal 31: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_failed = false,`.
            // Baris literal 32: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `reward_applied = false,`.
            // Baris literal 33: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id =
            // excluded.last_event_id`.
            // Baris literal 34: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            insert into session_participant_collection_missions (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_collection_mission_id,
                is_completed,
                is_failed,
                reward_applied,
                last_event_id,
                assigned_at
            )
            select
                @sessionId,
                @participantId,
                rcm.ruleset_version_id,
                rcm.ruleset_collection_mission_id,
                false,
                false,
                false,
                @eventId,
                @assignedAt
            from ruleset_collection_missions rcm
            where rcm.ruleset_version_id = @rulesetVersionId
              and lower(rcm.mission_code) = lower(@missionId)
              and rcm.is_active
            limit 1
            on conflict (session_participant_id, ruleset_collection_mission_id) do update
            set assigned_at = excluded.assigned_at,
                is_completed = false,
                is_failed = false,
                reward_applied = false,
                last_event_id = excluded.last_event_id
            """,
            // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, missionId, eventId, assignedAt sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectMissionAssignmentAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, missionId, eventId, assignedAt sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                sessionId = request.SessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, missionId, eventId, assignedAt sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, missionId, eventId, assignedAt sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                rulesetVersionId = request.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, missionId, eventId, assignedAt sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                missionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, missionId, eventId, assignedAt sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                eventId = request.EventId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, missionId, eventId, assignedAt sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                assignedAt = request.Timestamp.ToUniversalTime()
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ProjectMissionAssignmentAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));

        // Menjalankan hasil operasi asinkron memanggil `RefreshMissionCompletionAsync` dengan `request.SessionId`, `participantId`, `request.EventId`,
        // `conn`, `tx`, `ct`; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectMissionAssignmentAsync.
        await RefreshMissionCompletionAsync(request.SessionId, participantId, request.EventId, conn, tx, ct);
    // Menutup scope metode ProjectMissionAssignmentAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectMissionAssignmentAsync.
    }

    // Mendefinisikan metode `RefreshMissionCompletionAsync` dengan hasil bertipe `Task`; operasi ini menangani refresh misi penyelesaian asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe
    // `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `participantId` bertipe `Guid` membawa nilai
    // participant identitas; Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter
    // `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tx` bertipe
    // `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `ct` bertipe `CancellationToken`
    // membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private static async Task RefreshMissionCompletionAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode RefreshMissionCompletionAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // RefreshMissionCompletionAsync.
    {
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” update
        // session_participant_collection_missions spcm set is_completed = not exists ( select 1 from ruleset_collection_mission_requirements
        // requiremen...`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai
        // dalam RefreshMissionCompletionAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update
            // session_participant_collection_missions spcm`.
            // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set is_completed = not exists (`.
            // Baris literal 4: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
            // Baris literal 5: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_collection_mission_requirements requirement`.
            // Baris literal 6: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where requirement.ruleset_collection_mission_id =
            // spcm.ruleset_collection_mission_id`.
            // Baris literal 7: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not exists (`.
            // Baris literal 8: SELECT menentukan nilai atau kolom yang dikembalikan query: `select 1`.
            // Baris literal 9: FROM memilih tabel/subquery sumber pembacaan: `from session_participant_need_purchases purchase`.
            // Baris literal 10: JOIN menghubungkan data antartabel berdasarkan relasi/kondisi ON: `join ruleset_needs need on need.ruleset_need_id =
            // purchase.ruleset_need_id`.
            // Baris literal 11: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where purchase.session_id = @sessionId`.
            // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and purchase.session_participant_id = @participantId`.
            // Baris literal 13: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (`.
            // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `(`.
            // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `upper(requirement.requirement_type) = 'NEED_TIER'`.
            // Baris literal 16: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(need.need_tier) =
            // lower(requirement.required_need_tier)`.
            // Baris literal 17: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 18: Melanjutkan kondisi SQL dengan OR (alternatif syarat yang dapat terpenuhi): `or (`.
            // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `upper(requirement.requirement_type) = 'ASSET'`.
            // Baris literal 20: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and need.ruleset_game_asset_id =
            // requirement.required_asset_id`.
            // Baris literal 21: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 22: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 23: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 24: Pembatas literal/penutup `),`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `is_failed = false,`.
            // Baris literal 26: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId`.
            // Baris literal 27: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where spcm.session_id = @sessionId`.
            // Baris literal 28: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and spcm.session_participant_id = @participantId`.
            // Baris literal 29: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            update session_participant_collection_missions spcm
            set is_completed = not exists (
                    select 1
                    from ruleset_collection_mission_requirements requirement
                    where requirement.ruleset_collection_mission_id = spcm.ruleset_collection_mission_id
                      and not exists (
                          select 1
                          from session_participant_need_purchases purchase
                          join ruleset_needs need on need.ruleset_need_id = purchase.ruleset_need_id
                          where purchase.session_id = @sessionId
                            and purchase.session_participant_id = @participantId
                            and (
                                (
                                    upper(requirement.requirement_type) = 'NEED_TIER'
                                    and lower(need.need_tier) = lower(requirement.required_need_tier)
                                )
                                or (
                                    upper(requirement.requirement_type) = 'ASSET'
                                    and need.ruleset_game_asset_id = requirement.required_asset_id
                                )
                            )
                      )
                ),
                is_failed = false,
                last_event_id = @eventId
            where spcm.session_id = @sessionId
              and spcm.session_participant_id = @participantId
            """,
            // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, eventId sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            new { sessionId, participantId, eventId },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode RefreshMissionCompletionAsync; bagian berikut berada di luar batas blok tersebut dalam RefreshMissionCompletionAsync.
    }

    // Mendefinisikan metode `ProjectDonationAsync` dengan hasil bertipe `Task`; operasi ini menangani project donasi asinkron. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe `EventRequest` membawa
    // data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa nilai participant
    // identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter
    // `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private async Task ProjectDonationAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectDonationAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectDonationAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadAmount(request.Payload, out var amount)`; blok if hanya dijalankan ketika kondisi ini bernilai
        // benar dalam ProjectDonationAsync.
        if (!_payloadReader.TryReadAmount(request.Payload, out var amount))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadAmount(request.Payload, out var amount)`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam ProjectDonationAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectDonationAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadAmount(request.Payload, out var amount)`; bagian berikut berada di luar batas blok
        // tersebut dalam ProjectDonationAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” update
        // session_participant_balances set total_donasi = total_donasi + @amount, last_event_id = @eventId, updated_at = now() where
        // session_participan...`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai dalam ProjectDonationAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update
            // session_participant_balances`.
            // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set total_donasi = total_donasi + @amount,`.
            // Baris literal 4: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 5: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 6: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_participant_id = @participantId`.
            // Baris literal 7: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            update session_participant_balances
            set total_donasi = total_donasi + @amount,
                last_event_id = @eventId,
                updated_at = now()
            where session_participant_id = @participantId
            """,
            // Meneruskan objek anonim yang mengelompokkan participantId, amount, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectDonationAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan participantId, amount, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                participantId,
                // Meneruskan `amount` (nominal uang atau nilai transaksi yang dipakai dalam operasi) sebagai argumen ke `Math.Round`; Meneruskan
                // `MidpointRounding.AwayFromZero` (nilai away dari zero) sebagai argumen ke `Math.Round`.
                amount = (int)Math.Round(amount, MidpointRounding.AwayFromZero),
                // Meneruskan objek anonim yang mengelompokkan participantId, amount, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                eventId = request.EventId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ProjectDonationAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode ProjectDonationAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectDonationAsync.
    }

    // Mendefinisikan metode `ProjectDonationRankingAsync` dengan hasil bertipe `Task`; operasi ini menangani project donasi ranking asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private async Task ProjectDonationRankingAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectDonationRankingAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectDonationRankingAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadRankAwarded(request.Payload, out _, out var points)`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ProjectDonationRankingAsync.
        if (!_payloadReader.TryReadRankAwarded(request.Payload, out _, out var points))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadRankAwarded(request.Payload, out _, out var points)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam ProjectDonationRankingAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectDonationRankingAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadRankAwarded(request.Payload, out _, out var points)`; bagian berikut berada di luar
        // batas blok tersebut dalam ProjectDonationRankingAsync.
        }

        // Menyiapkan variabel lokal `day` untuk nomor hari permainan yang menjadi konteks aktivitas dengan menentukan nilai terbesar dari `1`,
        // `request.DayIndex + 1`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var day = Math.Max(1, request.DayIndex + 1);
        // Menyiapkan variabel lokal `eventNumber` untuk nilai event number dengan menentukan nilai terbesar dari `1`, `(day + 2) / 7`. Tipe variabel
        // disimpulkan dari ekspresi nilai awal.
        var eventNumber = Math.Max(1, (day + 2) / 7);
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” insert into
        // session_donation_events ( donation_event_id, session_id, event_ke, day, source_event_id, created_at ) values ( @donationEventId, @sessio...`;
        // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ProjectDonationRankingAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_donation_events (`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `donation_event_id,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `event_ke,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `day,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `source_event_id,`.
            // Baris literal 8: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at`.
            // Baris literal 9: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 10: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
            // Baris literal 11: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@donationEventId,`.
            // Baris literal 12: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@sessionId,`.
            // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@eventNumber,`.
            // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@day,`.
            // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@eventId,`.
            // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
            // Baris literal 17: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 18: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict (session_id, event_ke)
            // do update`.
            // Baris literal 19: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set day = excluded.day,`.
            // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `source_event_id =
            // excluded.source_event_id`.
            // Baris literal 21: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            insert into session_donation_events (
                donation_event_id,
                session_id,
                event_ke,
                day,
                source_event_id,
                created_at
            )
            values (
                @donationEventId,
                @sessionId,
                @eventNumber,
                @day,
                @eventId,
                now()
            )
            on conflict (session_id, event_ke) do update
            set day = excluded.day,
                source_event_id = excluded.source_event_id
            """,
            // Meneruskan objek anonim yang mengelompokkan donationEventId, sessionId, eventNumber, day, eventId sebagai satu nilai sebagai argumen ke
            // konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectDonationRankingAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan donationEventId, sessionId, eventNumber, day, eventId sebagai satu nilai sebagai argumen ke
                // konstruktor `CommandDefinition`.
                donationEventId = Guid.NewGuid(),
                // Meneruskan objek anonim yang mengelompokkan donationEventId, sessionId, eventNumber, day, eventId sebagai satu nilai sebagai argumen ke
                // konstruktor `CommandDefinition`.
                sessionId = request.SessionId,
                // Meneruskan objek anonim yang mengelompokkan donationEventId, sessionId, eventNumber, day, eventId sebagai satu nilai sebagai argumen ke
                // konstruktor `CommandDefinition`.
                eventNumber,
                // Meneruskan objek anonim yang mengelompokkan donationEventId, sessionId, eventNumber, day, eventId sebagai satu nilai sebagai argumen ke
                // konstruktor `CommandDefinition`.
                day,
                // Meneruskan objek anonim yang mengelompokkan donationEventId, sessionId, eventNumber, day, eventId sebagai satu nilai sebagai argumen ke
                // konstruktor `CommandDefinition`.
                eventId = request.EventId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ProjectDonationRankingAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));

        // Menjalankan hasil operasi asinkron memanggil `AddHappinessAsync` dengan `participantId`, `points`, `request.EventId`, `conn`, `tx`, `ct`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectDonationRankingAsync.
        await AddHappinessAsync(participantId, points, request.EventId, conn, tx, ct);
    // Menutup scope metode ProjectDonationRankingAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectDonationRankingAsync.
    }

    // Mendefinisikan metode `ProjectGoldTradeAsync` dengan hasil bertipe `Task`; operasi ini menangani project emas trade asinkron. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe `EventRequest` membawa
    // data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa nilai participant
    // identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter
    // `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private async Task ProjectGoldTradeAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectGoldTradeAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectGoldTradeAsync.
    {
        // Menyiapkan variabel lokal `canonicalAction` untuk nilai canonical aksi dengan memanggil `GameActionCatalog.ResolveGameActionId` dengan
        // `request.ActionType`, `request.Payload`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var canonicalAction = GameActionCatalog.ResolveGameActionId(request.ActionType, request.Payload);
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadGoldTrade(request.Payload, out var tradeType, out var qty, out var unitPrice, out var
        // amount)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ProjectGoldTradeAsync.
        if (!_payloadReader.TryReadGoldTrade(request.Payload, out var tradeType, out var qty, out var unitPrice, out var amount))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadGoldTrade(request.Payload, out var tradeType, out var qty, out var unitPrice, out
        // var amount)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectGoldTradeAsync.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!_payloadReader.TryGetInt32(request.Payload, ”qty”, out qty) ||
            // !_payloadReader.TryGetInt32(request.Payload, ”unit_price”, out unitPrice)` dan `!_payloadReader.TryGetInt32(request.Payload, ”amount”, out
            // amount)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ProjectGoldTradeAsync.
            if (!_payloadReader.TryGetInt32(request.Payload, "qty", out qty) ||
                // Menggunakan kebalikan kondisi `_payloadReader.TryGetInt32(request.Payload, ”unit_price”, out unitPrice)` sebagai bagian ekspresi yang sedang
                // disusun dalam ProjectGoldTradeAsync.
                !_payloadReader.TryGetInt32(request.Payload, "unit_price", out unitPrice) ||
                // Menggunakan kebalikan kondisi `_payloadReader.TryGetInt32(request.Payload, ”amount”, out amount)` sebagai bagian ekspresi yang sedang disusun
                // dalam ProjectGoldTradeAsync.
                !_payloadReader.TryGetInt32(request.Payload, "amount", out amount))
            // Membuka scope cabang if untuk kondisi `!_payloadReader.TryGetInt32(request.Payload, ”qty”, out qty) ||
            // !_payloadReader.TryGetInt32(request.Payload, ”unit_price”, out unitPrice) || !_payloadReader.TryGetInt32(reques...`; pernyataan/deklarasi berikut
            // berada di dalam batas blok ini dalam ProjectGoldTradeAsync.
            {
                // Mengakhiri eksekusi lebih awal dalam ProjectGoldTradeAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
                return;
            // Menutup scope cabang if untuk kondisi `!_payloadReader.TryGetInt32(request.Payload, ”qty”, out qty) ||
            // !_payloadReader.TryGetInt32(request.Payload, ”unit_price”, out unitPrice) || !_payloadReader.TryGetInt32(reques...`; bagian berikut berada di
            // luar batas blok tersebut dalam ProjectGoldTradeAsync.
            }

            // Memperbarui `tradeType` menggunakan hasil pemilihan bersyarat: ketika `string.Equals(canonicalAction, GameActionCatalog.JualEmas,
            // StringComparison.OrdinalIgnoreCase)` benar gunakan `”SELL”`, jika tidak gunakan `”BUY”` dalam ProjectGoldTradeAsync.
            tradeType = string.Equals(canonicalAction, GameActionCatalog.JualEmas, StringComparison.OrdinalIgnoreCase)
                // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”SELL” dalam ProjectGoldTradeAsync.
                ? "SELL"
                // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: ”BUY”; dalam ProjectGoldTradeAsync.
                : "BUY";
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadGoldTrade(request.Payload, out var tradeType, out var qty, out var unitPrice, out
        // var amount)`; bagian berikut berada di luar batas blok tersebut dalam ProjectGoldTradeAsync.
        }

        // Menyiapkan variabel lokal `isBuy` untuk nilai berstatus buy dengan gabungan syarat OR: setidaknya satu kondisi wajib benar antara
        // `string.Equals(canonicalAction, GameActionCatalog.InvestasiEmas, StringComparison.OrdinalIgnoreCase)` dan `(!string.Equals(canonicalAction,
        // GameActionCatalog.JualEmas, StringComparison.OrdinalIgnoreCase) && string.Equals(tradeType, ”BUY”, StringComparison.OrdinalIgnoreCase))`; sisi
        // kanan diperiksa hanya jika sisi kiri salah. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var isBuy = string.Equals(canonicalAction, GameActionCatalog.InvestasiEmas, StringComparison.OrdinalIgnoreCase) ||
                    // Menggunakan gabungan syarat AND: kedua kondisi wajib benar antara `!string.Equals(canonicalAction, GameActionCatalog.JualEmas,
                    // StringComparison.OrdinalIgnoreCase)` dan `string.Equals(tradeType, ”BUY”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa hanya jika
                    // sisi kiri benar sebagai bagian ekspresi yang sedang disusun dalam ProjectGoldTradeAsync.
                    (!string.Equals(canonicalAction, GameActionCatalog.JualEmas, StringComparison.OrdinalIgnoreCase) &&
                     // Melanjutkan pengolahan dengan membandingkan kesamaan `string` dengan `tradeType`, `”BUY”`, `StringComparison.OrdinalIgnoreCase`; aturan
                     // perbandingan mengikuti overload dan comparer yang diberikan dalam ProjectGoldTradeAsync.
                     string.Equals(tradeType, "BUY", StringComparison.OrdinalIgnoreCase));
        // Menyiapkan variabel lokal `quantityDelta` untuk nilai jumlah delta dengan hasil pemilihan bersyarat: ketika `isBuy` benar gunakan `qty`, jika
        // tidak gunakan `-qty`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var quantityDelta = isBuy ? qty : -qty;
        // Menyiapkan variabel lokal `amountDelta` untuk nilai nominal delta dengan hasil pemilihan bersyarat: ketika `isBuy` benar gunakan `amount`, jika
        // tidak gunakan `-amount`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var amountDelta = isBuy ? amount : -amount;
        // Menyiapkan variabel lokal `assetCode` untuk nilai aset kode dengan `ReadOptionalCode(request.Payload, ”asset_code”)` bila tidak null; jika null
        // gunakan `”gold_card”` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var assetCode = ReadOptionalCode(request.Payload, "asset_code") ?? "gold_card";
        // Menyiapkan variabel lokal `metadataJson` untuk nilai metadata JSON dengan menserialisasi `new { last_trade_type = tradeType.ToUpperInvariant(),
        // unit_price = unitPrice, last_event_id = request.EventId }` menjadi JSON melalui `JsonSerializer.Serialize`. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var metadataJson = JsonSerializer.Serialize(new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ProjectGoldTradeAsync.
        {
            // Meneruskan objek anonim yang mengelompokkan last_trade_type, unit_price, last_event_id sebagai satu nilai sebagai argumen ke
            // `JsonSerializer.Serialize`.
            last_trade_type = tradeType.ToUpperInvariant(),
            // Meneruskan objek anonim yang mengelompokkan last_trade_type, unit_price, last_event_id sebagai satu nilai sebagai argumen ke
            // `JsonSerializer.Serialize`.
            unit_price = unitPrice,
            // Meneruskan objek anonim yang mengelompokkan last_trade_type, unit_price, last_event_id sebagai satu nilai sebagai argumen ke
            // `JsonSerializer.Serialize`.
            last_event_id = request.EventId
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ProjectGoldTradeAsync.
        });

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” insert into
        // session_participant_gold_holdings ( session_id, session_participant_id, ruleset_version_id, ruleset_game_asset_id, quantity, last_event_...`;
        // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ProjectGoldTradeAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_gold_holdings (`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_game_asset_id,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `quantity,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id,`.
            // Baris literal 9: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at,`.
            // Baris literal 10: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at`.
            // Baris literal 11: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 12: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@sessionId,`.
            // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@participantId,`.
            // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rga.ruleset_version_id,`.
            // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rga.ruleset_game_asset_id,`.
            // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `greatest(0, @quantityDelta),`.
            // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@eventId,`.
            // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now(),`.
            // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
            // Baris literal 21: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_game_assets rga`.
            // Baris literal 22: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rga.ruleset_version_id = @rulesetVersionId`.
            // Baris literal 23: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rga.asset_type = 'GOLD'`.
            // Baris literal 24: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rga.asset_code = @assetCode`.
            // Baris literal 25: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
            // Baris literal 26: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict
            // (session_participant_id, ruleset_game_asset_id) do update`.
            // Baris literal 27: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set quantity = greatest(0,
            // coalesce(session_participant_gold_holdings.quantity, 0) + @quantityDelta),`.
            // Baris literal 28: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 29: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 30: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            insert into session_participant_gold_holdings (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                quantity,
                last_event_id,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @participantId,
                rga.ruleset_version_id,
                rga.ruleset_game_asset_id,
                greatest(0, @quantityDelta),
                @eventId,
                now(),
                now()
            from ruleset_game_assets rga
            where rga.ruleset_version_id = @rulesetVersionId
              and rga.asset_type = 'GOLD'
              and rga.asset_code = @assetCode
            limit 1
            on conflict (session_participant_id, ruleset_game_asset_id) do update
            set quantity = greatest(0, coalesce(session_participant_gold_holdings.quantity, 0) + @quantityDelta),
                last_event_id = @eventId,
                updated_at = now()
            """,
            // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, assetCode, quantityDelta, eventId sebagai satu nilai
            // sebagai argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectGoldTradeAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, assetCode, quantityDelta, eventId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                sessionId = request.SessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, assetCode, quantityDelta, eventId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, assetCode, quantityDelta, eventId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                rulesetVersionId = request.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, assetCode, quantityDelta, eventId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                assetCode,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, assetCode, quantityDelta, eventId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                quantityDelta,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, assetCode, quantityDelta, eventId sebagai satu nilai
                // sebagai argumen ke konstruktor `CommandDefinition`.
                eventId = request.EventId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ProjectGoldTradeAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode ProjectGoldTradeAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectGoldTradeAsync.
    }

    // Mendefinisikan metode `ProjectInitialGoldAsync` dengan hasil bertipe `Task`; operasi ini menangani project awal emas asinkron. async memungkinkan
    // metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe `EventRequest` membawa
    // data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa nilai participant
    // identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter
    // `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private async Task ProjectInitialGoldAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectInitialGoldAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectInitialGoldAsync.
    {
        // Menyiapkan variabel lokal `quantity` untuk nilai jumlah dengan hasil pemilihan bersyarat: ketika `_payloadReader.TryGetInt32(request.Payload,
        // ”qty”, out var qty)` benar gunakan `Math.Max(1, qty)`, jika tidak gunakan `1`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var quantity = _payloadReader.TryGetInt32(request.Payload, "qty", out var qty)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: Math.Max(1, qty) dalam ProjectInitialGoldAsync.
            ? Math.Max(1, qty)
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: 1; dalam ProjectInitialGoldAsync.
            : 1;
        // Menyiapkan variabel lokal `assetCode` untuk nilai aset kode dengan `ReadOptionalCode(request.Payload, ”asset_code”)` bila tidak null; jika null
        // gunakan `”gold_card”` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var assetCode = ReadOptionalCode(request.Payload, "asset_code") ?? "gold_card";

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” insert into
        // session_participant_gold_holdings ( session_id, session_participant_id, ruleset_version_id, ruleset_game_asset_id, quantity, last_event_...`;
        // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ProjectInitialGoldAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_gold_holdings (`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_game_asset_id,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `quantity,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id,`.
            // Baris literal 9: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at,`.
            // Baris literal 10: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at`.
            // Baris literal 11: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 12: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@sessionId,`.
            // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@participantId,`.
            // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rga.ruleset_version_id,`.
            // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rga.ruleset_game_asset_id,`.
            // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@quantity,`.
            // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@eventId,`.
            // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now(),`.
            // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
            // Baris literal 21: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_game_assets rga`.
            // Baris literal 22: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rga.ruleset_version_id = @rulesetVersionId`.
            // Baris literal 23: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rga.asset_type = 'GOLD'`.
            // Baris literal 24: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rga.asset_code = @assetCode`.
            // Baris literal 25: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
            // Baris literal 26: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict
            // (session_participant_id, ruleset_game_asset_id) do update`.
            // Baris literal 27: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set quantity = coalesce(session_participant_gold_holdings.quantity, 0)
            // + @quantity,`.
            // Baris literal 28: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 29: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 30: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            insert into session_participant_gold_holdings (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                quantity,
                last_event_id,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @participantId,
                rga.ruleset_version_id,
                rga.ruleset_game_asset_id,
                @quantity,
                @eventId,
                now(),
                now()
            from ruleset_game_assets rga
            where rga.ruleset_version_id = @rulesetVersionId
              and rga.asset_type = 'GOLD'
              and rga.asset_code = @assetCode
            limit 1
            on conflict (session_participant_id, ruleset_game_asset_id) do update
            set quantity = coalesce(session_participant_gold_holdings.quantity, 0) + @quantity,
                last_event_id = @eventId,
                updated_at = now()
            """,
            // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, assetCode, quantity, eventId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectInitialGoldAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, assetCode, quantity, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                sessionId = request.SessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, assetCode, quantity, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, assetCode, quantity, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                rulesetVersionId = request.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, assetCode, quantity, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                assetCode,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, assetCode, quantity, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                quantity,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, assetCode, quantity, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                eventId = request.EventId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ProjectInitialGoldAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode ProjectInitialGoldAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectInitialGoldAsync.
    }

    // Mendefinisikan metode `ProjectAwardedPointsAsync` dengan hasil bertipe `Task`; operasi ini menangani project awarded poin asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private async Task ProjectAwardedPointsAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectAwardedPointsAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectAwardedPointsAsync.
    {
        // Memeriksa memanggil `_payloadReader.TryReadPointsAwarded` dengan `request.Payload`, `var points`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ProjectAwardedPointsAsync.
        if (_payloadReader.TryReadPointsAwarded(request.Payload, out var points))
        // Membuka scope cabang if untuk kondisi `_payloadReader.TryReadPointsAwarded(request.Payload, out var points)`; pernyataan/deklarasi berikut berada
        // di dalam batas blok ini dalam ProjectAwardedPointsAsync.
        {
            // Menjalankan hasil operasi asinkron memanggil `AddHappinessAsync` dengan `participantId`, `points`, `request.EventId`, `conn`, `tx`, `ct`; await
            // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectAwardedPointsAsync.
            await AddHappinessAsync(participantId, points, request.EventId, conn, tx, ct);
        // Menutup scope cabang if untuk kondisi `_payloadReader.TryReadPointsAwarded(request.Payload, out var points)`; bagian berikut berada di luar batas
        // blok tersebut dalam ProjectAwardedPointsAsync.
        }
    // Menutup scope metode ProjectAwardedPointsAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectAwardedPointsAsync.
    }

    // Mendefinisikan metode `ProjectSavingDepositAsync` dengan hasil bertipe `Task`; operasi ini menangani project tabungan deposit asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private async Task ProjectSavingDepositAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectSavingDepositAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectSavingDepositAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadSavingDeposit(request.Payload, out var goalId, out var amount)`; blok if hanya dijalankan
        // ketika kondisi ini bernilai benar dalam ProjectSavingDepositAsync.
        if (!_payloadReader.TryReadSavingDeposit(request.Payload, out var goalId, out var amount))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadSavingDeposit(request.Payload, out var goalId, out var amount)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectSavingDepositAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectSavingDepositAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadSavingDeposit(request.Payload, out var goalId, out var amount)`; bagian berikut
        // berada di luar batas blok tersebut dalam ProjectSavingDepositAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” insert into
        // session_participant_financial_goals ( session_id, session_participant_id, ruleset_version_id, ruleset_financial_goal_id, current_amount,...`;
        // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ProjectSavingDepositAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_financial_goals (`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_financial_goal_id,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `current_amount,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `target_amount,`.
            // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `status,`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `purchased_at_day,`.
            // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id,`.
            // Baris literal 12: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at,`.
            // Baris literal 13: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at`.
            // Baris literal 14: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 15: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@sessionId,`.
            // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@participantId,`.
            // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rfg.ruleset_version_id,`.
            // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `rfg.ruleset_financial_goal_id,`.
            // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `least(@amount, rfg.purchase_price),`.
            // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rfg.purchase_price,`.
            // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'ONGOING',`.
            // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `null,`.
            // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@eventId,`.
            // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now(),`.
            // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
            // Baris literal 27: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_financial_goals rfg`.
            // Baris literal 28: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rfg.ruleset_version_id = @rulesetVersionId`.
            // Baris literal 29: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(rfg.goal_code) = lower(@goalId)`.
            // Baris literal 30: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rfg.is_active`.
            // Baris literal 31: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
            // Baris literal 32: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict
            // (session_participant_id, ruleset_financial_goal_id) do update`.
            // Baris literal 33: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set current_amount = least(`.
            // Baris literal 34: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `session_participant_financial_goals.target_amount,`.
            // Baris literal 35: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `session_participant_financial_goals.current_amount + @amount`.
            // Baris literal 36: Pembatas literal/penutup `),`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 37: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 38: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 39: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            insert into session_participant_financial_goals (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_financial_goal_id,
                current_amount,
                target_amount,
                status,
                purchased_at_day,
                last_event_id,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @participantId,
                rfg.ruleset_version_id,
                rfg.ruleset_financial_goal_id,
                least(@amount, rfg.purchase_price),
                rfg.purchase_price,
                'ONGOING',
                null,
                @eventId,
                now(),
                now()
            from ruleset_financial_goals rfg
            where rfg.ruleset_version_id = @rulesetVersionId
              and lower(rfg.goal_code) = lower(@goalId)
              and rfg.is_active
            limit 1
            on conflict (session_participant_id, ruleset_financial_goal_id) do update
            set current_amount = least(
                    session_participant_financial_goals.target_amount,
                    session_participant_financial_goals.current_amount + @amount
                ),
                last_event_id = @eventId,
                updated_at = now()
            """,
            // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, goalId, amount, eventId sebagai satu nilai sebagai
            // argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectSavingDepositAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, goalId, amount, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                sessionId = request.SessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, goalId, amount, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, goalId, amount, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                rulesetVersionId = request.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, goalId, amount, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                goalId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, goalId, amount, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                amount,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, goalId, amount, eventId sebagai satu nilai sebagai
                // argumen ke konstruktor `CommandDefinition`.
                eventId = request.EventId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ProjectSavingDepositAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” update
        // session_participant_balances set saving = saving + @amount, last_event_id = @eventId, updated_at = now() where session_participant_id =
        // @part...`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ProjectSavingDepositAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update
            // session_participant_balances`.
            // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set saving = saving + @amount,`.
            // Baris literal 4: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 5: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 6: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_participant_id = @participantId`.
            // Baris literal 7: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            update session_participant_balances
            set saving = saving + @amount,
                last_event_id = @eventId,
                updated_at = now()
            where session_participant_id = @participantId
            """,
            // Meneruskan objek anonim yang mengelompokkan participantId, amount, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new { participantId, amount, eventId = request.EventId },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode ProjectSavingDepositAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectSavingDepositAsync.
    }

    // Mendefinisikan metode `ProjectSavingWithdrawalAsync` dengan hasil bertipe `Task`; operasi ini menangani project tabungan withdrawal asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private async Task ProjectSavingWithdrawalAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectSavingWithdrawalAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ProjectSavingWithdrawalAsync.
    {
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!_payloadReader.TryGetString(request.Payload, ”goal_id”, out var
        // goalId)` dan `!_payloadReader.TryGetInt32(request.Payload, ”amount”, out var amount)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if
        // hanya dijalankan ketika kondisi ini bernilai benar dalam ProjectSavingWithdrawalAsync.
        if (!_payloadReader.TryGetString(request.Payload, "goal_id", out var goalId) ||
            // Menggunakan kebalikan kondisi `_payloadReader.TryGetInt32(request.Payload, ”amount”, out var amount)` sebagai bagian ekspresi yang sedang disusun
            // dalam ProjectSavingWithdrawalAsync.
            !_payloadReader.TryGetInt32(request.Payload, "amount", out var amount))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryGetString(request.Payload, ”goal_id”, out var goalId) ||
        // !_payloadReader.TryGetInt32(request.Payload, ”amount”, out var amount)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ProjectSavingWithdrawalAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectSavingWithdrawalAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryGetString(request.Payload, ”goal_id”, out var goalId) ||
        // !_payloadReader.TryGetInt32(request.Payload, ”amount”, out var amount)`; bagian berikut berada di luar batas blok tersebut dalam
        // ProjectSavingWithdrawalAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” update
        // session_participant_financial_goals goal set current_amount = greatest(0, goal.current_amount - @amount), last_event_id = @eventId,
        // updated_a...`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai
        // dalam ProjectSavingWithdrawalAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update
            // session_participant_financial_goals goal`.
            // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set current_amount = greatest(0, goal.current_amount - @amount),`.
            // Baris literal 4: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 5: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 6: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_financial_goals rfg`.
            // Baris literal 7: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where goal.session_participant_id = @participantId`.
            // Baris literal 8: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and goal.ruleset_financial_goal_id =
            // rfg.ruleset_financial_goal_id`.
            // Baris literal 9: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(rfg.goal_code) = lower(@goalId);`.
            // Baris literal 10: Pembatas literal/penutup ``; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 11: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update
            // session_participant_balances`.
            // Baris literal 12: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set saving = greatest(0, saving - @amount),`.
            // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 14: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 15: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_participant_id = @participantId;`.
            // Baris literal 16: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            update session_participant_financial_goals goal
            set current_amount = greatest(0, goal.current_amount - @amount),
                last_event_id = @eventId,
                updated_at = now()
            from ruleset_financial_goals rfg
            where goal.session_participant_id = @participantId
              and goal.ruleset_financial_goal_id = rfg.ruleset_financial_goal_id
              and lower(rfg.goal_code) = lower(@goalId);

            update session_participant_balances
            set saving = greatest(0, saving - @amount),
                last_event_id = @eventId,
                updated_at = now()
            where session_participant_id = @participantId;
            """,
            // Meneruskan objek anonim yang mengelompokkan participantId, goalId, amount, eventId sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            new { participantId, goalId, amount, eventId = request.EventId },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode ProjectSavingWithdrawalAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectSavingWithdrawalAsync.
    }

    // Mendefinisikan metode `ProjectSavingGoalAchievedAsync` dengan hasil bertipe `Task`; operasi ini menangani project tabungan target achieved
    // asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request`
    // bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid`
    // membawa nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca
    // hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan;
    // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
    // aplikasi berhenti.
    private async Task ProjectSavingGoalAchievedAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectSavingGoalAchievedAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ProjectSavingGoalAchievedAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadSavingGoalAchieved(request.Payload, out var goalId, out var points, out var cost)`; blok if
        // hanya dijalankan ketika kondisi ini bernilai benar dalam ProjectSavingGoalAchievedAsync.
        if (!_payloadReader.TryReadSavingGoalAchieved(request.Payload, out var goalId, out var points, out var cost))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadSavingGoalAchieved(request.Payload, out var goalId, out var points, out var cost)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectSavingGoalAchievedAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectSavingGoalAchievedAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadSavingGoalAchieved(request.Payload, out var goalId, out var points, out var cost)`;
        // bagian berikut berada di luar batas blok tersebut dalam ProjectSavingGoalAchievedAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” insert into
        // session_participant_financial_goals ( session_id, session_participant_id, ruleset_version_id, ruleset_financial_goal_id, current_amount,...`;
        // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ProjectSavingGoalAchievedAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_financial_goals (`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_financial_goal_id,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `current_amount,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `target_amount,`.
            // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `status,`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `purchased_at_day,`.
            // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id,`.
            // Baris literal 12: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at,`.
            // Baris literal 13: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at`.
            // Baris literal 14: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 15: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@sessionId,`.
            // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@participantId,`.
            // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rfg.ruleset_version_id,`.
            // Baris literal 19: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `rfg.ruleset_financial_goal_id,`.
            // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rfg.purchase_price,`.
            // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rfg.purchase_price,`.
            // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'COMPLETED',`.
            // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@day,`.
            // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@eventId,`.
            // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now(),`.
            // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
            // Baris literal 27: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_financial_goals rfg`.
            // Baris literal 28: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rfg.ruleset_version_id = @rulesetVersionId`.
            // Baris literal 29: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(rfg.goal_code) = lower(@goalId)`.
            // Baris literal 30: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rfg.is_active`.
            // Baris literal 31: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
            // Baris literal 32: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict
            // (session_participant_id, ruleset_financial_goal_id) do update`.
            // Baris literal 33: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set current_amount = excluded.current_amount,`.
            // Baris literal 34: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `target_amount =
            // excluded.target_amount,`.
            // Baris literal 35: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `status = 'COMPLETED',`.
            // Baris literal 36: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `purchased_at_day =
            // excluded.purchased_at_day,`.
            // Baris literal 37: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 38: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 39: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            insert into session_participant_financial_goals (
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_financial_goal_id,
                current_amount,
                target_amount,
                status,
                purchased_at_day,
                last_event_id,
                created_at,
                updated_at
            )
            select
                @sessionId,
                @participantId,
                rfg.ruleset_version_id,
                rfg.ruleset_financial_goal_id,
                rfg.purchase_price,
                rfg.purchase_price,
                'COMPLETED',
                @day,
                @eventId,
                now(),
                now()
            from ruleset_financial_goals rfg
            where rfg.ruleset_version_id = @rulesetVersionId
              and lower(rfg.goal_code) = lower(@goalId)
              and rfg.is_active
            limit 1
            on conflict (session_participant_id, ruleset_financial_goal_id) do update
            set current_amount = excluded.current_amount,
                target_amount = excluded.target_amount,
                status = 'COMPLETED',
                purchased_at_day = excluded.purchased_at_day,
                last_event_id = @eventId,
                updated_at = now()
            """,
            // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, goalId, eventId, day sebagai satu nilai sebagai argumen
            // ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectSavingGoalAchievedAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, goalId, eventId, day sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                sessionId = request.SessionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, goalId, eventId, day sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, goalId, eventId, day sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                rulesetVersionId = request.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, goalId, eventId, day sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                goalId,
                // Meneruskan objek anonim yang mengelompokkan sessionId, participantId, rulesetVersionId, goalId, eventId, day sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                eventId = request.EventId,
                // Meneruskan nilai literal `1` sebagai argumen ke `Math.Max`; Meneruskan penjumlahan/penggabungan antara `request.DayIndex` dan `1` sebagai argumen
                // ke `Math.Max`.
                day = Math.Max(1, request.DayIndex + 1)
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ProjectSavingGoalAchievedAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” update
        // session_participant_balances set saving = greatest(0, saving - @cost), happiness = happiness + @points, last_event_id = @eventId, updated_at
        // ...`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ProjectSavingGoalAchievedAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update
            // session_participant_balances`.
            // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set saving = greatest(0, saving - @cost),`.
            // Baris literal 4: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `happiness = happiness + @points,`.
            // Baris literal 5: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 6: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 7: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_participant_id = @participantId`.
            // Baris literal 8: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            update session_participant_balances
            set saving = greatest(0, saving - @cost),
                happiness = happiness + @points,
                last_event_id = @eventId,
                updated_at = now()
            where session_participant_id = @participantId
            """,
            // Meneruskan objek anonim yang mengelompokkan participantId, cost, points, eventId sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            new { participantId, cost, points, eventId = request.EventId },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode ProjectSavingGoalAchievedAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectSavingGoalAchievedAsync.
    }

    // Mendefinisikan metode `ProjectLoanTakenAsync` dengan hasil bertipe `Task`; operasi ini menangani project pinjaman taken asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private async Task ProjectLoanTakenAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectLoanTakenAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectLoanTakenAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadLoanTaken( request.Payload, out var loanId, out var principal, out var repaymentAmount, out
        // var duration, out var penaltyPoints)`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ProjectLoanTakenAsync.
        if (!_payloadReader.TryReadLoanTaken(
                // Meneruskan `request.Payload` (muatan detail event dalam format JSON) sebagai argumen ke `_payloadReader.TryReadLoanTaken`.
                request.Payload,
                // Meneruskan `var loanId` sebagai argumen ke `_payloadReader.TryReadLoanTaken`.
                out var loanId,
                // Meneruskan `var principal` sebagai argumen ke `_payloadReader.TryReadLoanTaken`.
                out var principal,
                // Meneruskan `var repaymentAmount` sebagai argumen ke `_payloadReader.TryReadLoanTaken`.
                out var repaymentAmount,
                // Meneruskan `var duration` sebagai argumen ke `_payloadReader.TryReadLoanTaken`.
                out var duration,
                // Meneruskan `var penaltyPoints` sebagai argumen ke `_payloadReader.TryReadLoanTaken`.
                out var penaltyPoints))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadLoanTaken( request.Payload, out var loanId, out var principal, out var
        // repaymentAmount, out var duration, out var penaltyPoints)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ProjectLoanTakenAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectLoanTakenAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadLoanTaken( request.Payload, out var loanId, out var principal, out var
        // repaymentAmount, out var duration, out var penaltyPoints)`; bagian berikut berada di luar batas blok tersebut dalam ProjectLoanTakenAsync.
        }

        // Menyiapkan variabel lokal `metadataJson` untuk nilai metadata JSON dengan menserialisasi `new { loan_id = loanId, principal, repayment_amount =
        // repaymentAmount, duration_days = duration, penalty_points = penaltyPoints, repaid_amount = 0 }` menjadi JSON melalui `JsonSerializer.Serialize`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metadataJson = JsonSerializer.Serialize(new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ProjectLoanTakenAsync.
        {
            // Meneruskan objek anonim yang mengelompokkan loan_id, principal, repayment_amount, duration_days, penalty_points, repaid_amount sebagai satu nilai
            // sebagai argumen ke `JsonSerializer.Serialize`.
            loan_id = loanId,
            // Meneruskan objek anonim yang mengelompokkan loan_id, principal, repayment_amount, duration_days, penalty_points, repaid_amount sebagai satu nilai
            // sebagai argumen ke `JsonSerializer.Serialize`.
            principal,
            // Meneruskan objek anonim yang mengelompokkan loan_id, principal, repayment_amount, duration_days, penalty_points, repaid_amount sebagai satu nilai
            // sebagai argumen ke `JsonSerializer.Serialize`.
            repayment_amount = repaymentAmount,
            // Meneruskan objek anonim yang mengelompokkan loan_id, principal, repayment_amount, duration_days, penalty_points, repaid_amount sebagai satu nilai
            // sebagai argumen ke `JsonSerializer.Serialize`.
            duration_days = duration,
            // Meneruskan objek anonim yang mengelompokkan loan_id, principal, repayment_amount, duration_days, penalty_points, repaid_amount sebagai satu nilai
            // sebagai argumen ke `JsonSerializer.Serialize`.
            penalty_points = penaltyPoints,
            // Meneruskan objek anonim yang mengelompokkan loan_id, principal, repayment_amount, duration_days, penalty_points, repaid_amount sebagai satu nilai
            // sebagai argumen ke `JsonSerializer.Serialize`.
            repaid_amount = 0
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ProjectLoanTakenAsync.
        });
        // Menyiapkan variabel lokal `loanCode` untuk kode produk pinjaman syariah dengan hasil pemilihan bersyarat: ketika
        // `_payloadReader.TryGetString(request.Payload, ”loan_code”, out var requestedLoanCode)` benar gunakan `requestedLoanCode`, jika tidak gunakan
        // `loanId`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var loanCode = _payloadReader.TryGetString(request.Payload, "loan_code", out var requestedLoanCode)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: requestedLoanCode dalam ProjectLoanTakenAsync.
            ? requestedLoanCode
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: loanId; dalam ProjectLoanTakenAsync.
            : loanId;

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” insert into
        // session_participant_loans ( session_participant_loan_id, session_id, session_participant_id, ruleset_version_id, ruleset_sharia_loan_id,...`;
        // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ProjectLoanTakenAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_loans (`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `session_participant_loan_id,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_sharia_loan_id,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `loan_instance_id,`.
            // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `principal_amount,`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `outstanding_amount,`.
            // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `repayment_amount,`.
            // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `status,`.
            // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `source_event_id,`.
            // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id,`.
            // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metadata_json,`.
            // Baris literal 16: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at,`.
            // Baris literal 17: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at`.
            // Baris literal 18: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 19: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@loanHoldingId,`.
            // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@sessionId,`.
            // Baris literal 22: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@participantId,`.
            // Baris literal 23: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rsl.ruleset_version_id,`.
            // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `rsl.ruleset_sharia_loan_id,`.
            // Baris literal 25: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@loanId,`.
            // Baris literal 26: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@principal,`.
            // Baris literal 27: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@principal,`.
            // Baris literal 28: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@repaymentAmount,`.
            // Baris literal 29: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'ACTIVE',`.
            // Baris literal 30: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@eventId,`.
            // Baris literal 31: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@eventId,`.
            // Baris literal 32: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@metadataJson::jsonb,`.
            // Baris literal 33: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now(),`.
            // Baris literal 34: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
            // Baris literal 35: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_sharia_loans rsl`.
            // Baris literal 36: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rsl.ruleset_version_id = @rulesetVersionId`.
            // Baris literal 37: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(rsl.loan_code) = lower(@loanCode)`.
            // Baris literal 38: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
            // Baris literal 39: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict
            // (session_participant_id, loan_instance_id) do update`.
            // Baris literal 40: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set principal_amount = excluded.principal_amount,`.
            // Baris literal 41: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `outstanding_amount =
            // excluded.outstanding_amount,`.
            // Baris literal 42: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `repayment_amount =
            // excluded.repayment_amount,`.
            // Baris literal 43: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `status = excluded.status,`.
            // Baris literal 44: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id =
            // excluded.last_event_id,`.
            // Baris literal 45: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metadata_json =
            // session_participant_loans.metadata_json || excluded.metadata_json,`.
            // Baris literal 46: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 47: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            insert into session_participant_loans (
                session_participant_loan_id,
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_sharia_loan_id,
                loan_instance_id,
                principal_amount,
                outstanding_amount,
                repayment_amount,
                status,
                source_event_id,
                last_event_id,
                metadata_json,
                created_at,
                updated_at
            )
            select
                @loanHoldingId,
                @sessionId,
                @participantId,
                rsl.ruleset_version_id,
                rsl.ruleset_sharia_loan_id,
                @loanId,
                @principal,
                @principal,
                @repaymentAmount,
                'ACTIVE',
                @eventId,
                @eventId,
                @metadataJson::jsonb,
                now(),
                now()
            from ruleset_sharia_loans rsl
            where rsl.ruleset_version_id = @rulesetVersionId
              and lower(rsl.loan_code) = lower(@loanCode)
            limit 1
            on conflict (session_participant_id, loan_instance_id) do update
            set principal_amount = excluded.principal_amount,
                outstanding_amount = excluded.outstanding_amount,
                repayment_amount = excluded.repayment_amount,
                status = excluded.status,
                last_event_id = excluded.last_event_id,
                metadata_json = session_participant_loans.metadata_json || excluded.metadata_json,
                updated_at = now()
            """,
            // Meneruskan objek anonim yang mengelompokkan loanHoldingId, sessionId, participantId, rulesetVersionId, loanId, loanCode, principal,
            // repaymentAmount, metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectLoanTakenAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan loanHoldingId, sessionId, participantId, rulesetVersionId, loanId, loanCode, principal,
                // repaymentAmount, metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                loanHoldingId = Guid.NewGuid(),
                // Meneruskan objek anonim yang mengelompokkan loanHoldingId, sessionId, participantId, rulesetVersionId, loanId, loanCode, principal,
                // repaymentAmount, metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                sessionId = request.SessionId,
                // Meneruskan objek anonim yang mengelompokkan loanHoldingId, sessionId, participantId, rulesetVersionId, loanId, loanCode, principal,
                // repaymentAmount, metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan loanHoldingId, sessionId, participantId, rulesetVersionId, loanId, loanCode, principal,
                // repaymentAmount, metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                rulesetVersionId = request.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan loanHoldingId, sessionId, participantId, rulesetVersionId, loanId, loanCode, principal,
                // repaymentAmount, metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                loanId,
                // Meneruskan objek anonim yang mengelompokkan loanHoldingId, sessionId, participantId, rulesetVersionId, loanId, loanCode, principal,
                // repaymentAmount, metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                loanCode,
                // Meneruskan objek anonim yang mengelompokkan loanHoldingId, sessionId, participantId, rulesetVersionId, loanId, loanCode, principal,
                // repaymentAmount, metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                principal,
                // Meneruskan objek anonim yang mengelompokkan loanHoldingId, sessionId, participantId, rulesetVersionId, loanId, loanCode, principal,
                // repaymentAmount, metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                repaymentAmount,
                // Meneruskan objek anonim yang mengelompokkan loanHoldingId, sessionId, participantId, rulesetVersionId, loanId, loanCode, principal,
                // repaymentAmount, metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                metadataJson,
                // Meneruskan objek anonim yang mengelompokkan loanHoldingId, sessionId, participantId, rulesetVersionId, loanId, loanCode, principal,
                // repaymentAmount, metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                eventId = request.EventId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ProjectLoanTakenAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode ProjectLoanTakenAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectLoanTakenAsync.
    }

    // Mendefinisikan metode `ProjectLoanRepaidAsync` dengan hasil bertipe `Task`; operasi ini menangani project pinjaman dilunasi asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private async Task ProjectLoanRepaidAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectLoanRepaidAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectLoanRepaidAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadLoanRepay(request.Payload, out var loanId, out var amount)`; blok if hanya dijalankan ketika
        // kondisi ini bernilai benar dalam ProjectLoanRepaidAsync.
        if (!_payloadReader.TryReadLoanRepay(request.Payload, out var loanId, out var amount))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadLoanRepay(request.Payload, out var loanId, out var amount)`; pernyataan/deklarasi
        // berikut berada di dalam batas blok ini dalam ProjectLoanRepaidAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectLoanRepaidAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadLoanRepay(request.Payload, out var loanId, out var amount)`; bagian berikut berada
        // di luar batas blok tersebut dalam ProjectLoanRepaidAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” update
        // session_participant_loans set outstanding_amount = greatest(0, coalesce(outstanding_amount, 0) - @amount), status = case when
        // coalesce(outsta...`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum
        // selesai dalam ProjectLoanRepaidAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update session_participant_loans`.
            // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set outstanding_amount = greatest(0, coalesce(outstanding_amount, 0) -
            // @amount),`.
            // Baris literal 4: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `status = case when coalesce(outstanding_amount, 0) - @amount <= 0 then 'PAID' else 'ACTIVE' end,`.
            // Baris literal 5: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metadata_json = metadata_json
            // || jsonb_build_object(`.
            // Baris literal 7: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `'last_repayment', @amount,`.
            // Baris literal 8: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `'last_repaid_at', @repaidAt`.
            // Baris literal 9: Pembatas literal/penutup `),`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 10: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 11: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_participant_id = @participantId`.
            // Baris literal 12: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(loan_instance_id) = lower(@loanId)`.
            // Baris literal 13: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            update session_participant_loans
            set outstanding_amount = greatest(0, coalesce(outstanding_amount, 0) - @amount),
                status = case when coalesce(outstanding_amount, 0) - @amount <= 0 then 'PAID' else 'ACTIVE' end,
                last_event_id = @eventId,
                metadata_json = metadata_json || jsonb_build_object(
                    'last_repayment', @amount,
                    'last_repaid_at', @repaidAt
                ),
                updated_at = now()
            where session_participant_id = @participantId
              and lower(loan_instance_id) = lower(@loanId)
            """,
            // Meneruskan objek anonim yang mengelompokkan participantId, rulesetVersionId, loanId, amount, eventId, repaidAt sebagai satu nilai sebagai argumen
            // ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectLoanRepaidAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan participantId, rulesetVersionId, loanId, amount, eventId, repaidAt sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan participantId, rulesetVersionId, loanId, amount, eventId, repaidAt sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                rulesetVersionId = request.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan participantId, rulesetVersionId, loanId, amount, eventId, repaidAt sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                loanId,
                // Meneruskan objek anonim yang mengelompokkan participantId, rulesetVersionId, loanId, amount, eventId, repaidAt sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                amount,
                // Meneruskan objek anonim yang mengelompokkan participantId, rulesetVersionId, loanId, amount, eventId, repaidAt sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                eventId = request.EventId,
                // Meneruskan objek anonim yang mengelompokkan participantId, rulesetVersionId, loanId, amount, eventId, repaidAt sebagai satu nilai sebagai argumen
                // ke konstruktor `CommandDefinition`.
                repaidAt = request.Timestamp.ToUniversalTime()
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ProjectLoanRepaidAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode ProjectLoanRepaidAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectLoanRepaidAsync.
    }

    // Mendefinisikan metode `ProjectInsurancePurchasedAsync` dengan hasil bertipe `Task`; operasi ini menangani project asuransi dibeli asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private async Task ProjectInsurancePurchasedAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectInsurancePurchasedAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // ProjectInsurancePurchasedAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadInsurance(request.Payload, out var premium)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ProjectInsurancePurchasedAsync.
        if (!_payloadReader.TryReadInsurance(request.Payload, out var premium))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadInsurance(request.Payload, out var premium)`; pernyataan/deklarasi berikut berada
        // di dalam batas blok ini dalam ProjectInsurancePurchasedAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectInsurancePurchasedAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadInsurance(request.Payload, out var premium)`; bagian berikut berada di luar batas
        // blok tersebut dalam ProjectInsurancePurchasedAsync.
        }

        // Menyiapkan variabel lokal `productCode` untuk nilai product kode dengan `ReadOptionalCode(request.Payload, ”policy_id”)` bila tidak null; jika
        // null gunakan `ReadOptionalCode(request.Payload, ”product_code”) ?? ”multirisk_basic”` sebagai nilai pengganti. Tipe variabel disimpulkan dari
        // ekspresi nilai awal.
        var productCode = ReadOptionalCode(request.Payload, "policy_id")
                          // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ReadOptionalCode(request.Payload, ”product_code”) dalam
                          // ProjectInsurancePurchasedAsync.
                          ?? ReadOptionalCode(request.Payload, "product_code")
                          // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ”multirisk_basic”; dalam ProjectInsurancePurchasedAsync.
                          ?? "multirisk_basic";
        // Menyiapkan variabel lokal `usageLimit` untuk nilai usage limit dengan `await conn.QuerySingleOrDefaultAsync<int?>( new CommandDefinition( ”””
        // select usage_limit from ruleset_insurance_products where ruleset_version_id = @rulesetVersionId and lowe...` bila tidak null; jika null gunakan
        // `1` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var usageLimit = await conn.QuerySingleOrDefaultAsync<int?>(
            // Meneruskan objek baru bertipe `CommandDefinition` dengan argumen ( ””” select usage_limit from ruleset_insurance_products where
            // ruleset_version_id = @rulesetVersionId and lower(product_code) = lower(@productCode) order by sor... sebagai argumen ke
            // `conn.QuerySingleOrDefaultAsync<int?>`.
            new CommandDefinition(
                // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
                // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
                // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                // Baris literal 2: SELECT menentukan nilai atau kolom yang dikembalikan query: `select usage_limit`.
                // Baris literal 3: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_insurance_products`.
                // Baris literal 4: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where ruleset_version_id = @rulesetVersionId`.
                // Baris literal 5: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(product_code) = lower(@productCode)`.
                // Baris literal 6: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by sort_order asc`.
                // Baris literal 7: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
                // Baris literal 8: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
                """
                select usage_limit
                from ruleset_insurance_products
                where ruleset_version_id = @rulesetVersionId
                  and lower(product_code) = lower(@productCode)
                order by sort_order asc
                limit 1
                """,
                // Meneruskan objek anonim yang mengelompokkan rulesetVersionId, productCode sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                new { rulesetVersionId = request.RulesetVersionId, productCode },
                // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
                tx,
                // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
                // bernama `cancellationToken`.
                cancellationToken: ct)) ?? 1;
        // Menyiapkan variabel lokal `metadataJson` untuk nilai metadata JSON dengan menserialisasi `new { premium, usage_limit = usageLimit, used_count = 0
        // }` menjadi JSON melalui `JsonSerializer.Serialize`. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var metadataJson = JsonSerializer.Serialize(new
        // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ProjectInsurancePurchasedAsync.
        {
            // Meneruskan objek anonim yang mengelompokkan premium, usage_limit, used_count sebagai satu nilai sebagai argumen ke `JsonSerializer.Serialize`.
            premium,
            // Meneruskan objek anonim yang mengelompokkan premium, usage_limit, used_count sebagai satu nilai sebagai argumen ke `JsonSerializer.Serialize`.
            usage_limit = usageLimit,
            // Meneruskan objek anonim yang mengelompokkan premium, usage_limit, used_count sebagai satu nilai sebagai argumen ke `JsonSerializer.Serialize`.
            used_count = 0
        // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
        // ProjectInsurancePurchasedAsync.
        });

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” insert into
        // session_participant_insurances ( session_participant_insurance_id, session_id, session_participant_id, ruleset_version_id, ruleset_insur...`;
        // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ProjectInsurancePurchasedAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_insurances (`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `session_participant_insurance_id,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `ruleset_insurance_product_id,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `premium_paid,`.
            // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `remaining_uses,`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `status,`.
            // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `source_event_id,`.
            // Baris literal 12: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id,`.
            // Baris literal 13: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metadata_json,`.
            // Baris literal 14: CREATE mendefinisikan objek basis data yang diperlukan proses ini: `created_at,`.
            // Baris literal 15: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at`.
            // Baris literal 16: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 17: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 18: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@insuranceHoldingId,`.
            // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@sessionId,`.
            // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@participantId,`.
            // Baris literal 21: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rip.ruleset_version_id,`.
            // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `rip.ruleset_insurance_product_id,`.
            // Baris literal 23: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@premium,`.
            // Baris literal 24: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `greatest(@usageLimit, rip.usage_limit),`.
            // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'ACTIVE',`.
            // Baris literal 26: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@eventId,`.
            // Baris literal 27: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@eventId,`.
            // Baris literal 28: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@metadataJson::jsonb,`.
            // Baris literal 29: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now(),`.
            // Baris literal 30: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now()`.
            // Baris literal 31: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_insurance_products rip`.
            // Baris literal 32: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rip.ruleset_version_id = @rulesetVersionId`.
            // Baris literal 33: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and lower(rip.product_code) = lower(@productCode)`.
            // Baris literal 34: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
            // Baris literal 35: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict
            // (session_participant_id, ruleset_insurance_product_id) do update`.
            // Baris literal 36: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set premium_paid = excluded.premium_paid,`.
            // Baris literal 37: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `remaining_uses =
            // excluded.remaining_uses,`.
            // Baris literal 38: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `status = excluded.status,`.
            // Baris literal 39: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id =
            // excluded.last_event_id,`.
            // Baris literal 40: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metadata_json =
            // session_participant_insurances.metadata_json || excluded.metadata_json,`.
            // Baris literal 41: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 42: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            insert into session_participant_insurances (
                session_participant_insurance_id,
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_insurance_product_id,
                premium_paid,
                remaining_uses,
                status,
                source_event_id,
                last_event_id,
                metadata_json,
                created_at,
                updated_at
            )
            select
                @insuranceHoldingId,
                @sessionId,
                @participantId,
                rip.ruleset_version_id,
                rip.ruleset_insurance_product_id,
                @premium,
                greatest(@usageLimit, rip.usage_limit),
                'ACTIVE',
                @eventId,
                @eventId,
                @metadataJson::jsonb,
                now(),
                now()
            from ruleset_insurance_products rip
            where rip.ruleset_version_id = @rulesetVersionId
              and lower(rip.product_code) = lower(@productCode)
            limit 1
            on conflict (session_participant_id, ruleset_insurance_product_id) do update
            set premium_paid = excluded.premium_paid,
                remaining_uses = excluded.remaining_uses,
                status = excluded.status,
                last_event_id = excluded.last_event_id,
                metadata_json = session_participant_insurances.metadata_json || excluded.metadata_json,
                updated_at = now()
            """,
            // Meneruskan objek anonim yang mengelompokkan insuranceHoldingId, sessionId, participantId, rulesetVersionId, productCode, premium, usageLimit,
            // metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectInsurancePurchasedAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan insuranceHoldingId, sessionId, participantId, rulesetVersionId, productCode, premium, usageLimit,
                // metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                insuranceHoldingId = Guid.NewGuid(),
                // Meneruskan objek anonim yang mengelompokkan insuranceHoldingId, sessionId, participantId, rulesetVersionId, productCode, premium, usageLimit,
                // metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                sessionId = request.SessionId,
                // Meneruskan objek anonim yang mengelompokkan insuranceHoldingId, sessionId, participantId, rulesetVersionId, productCode, premium, usageLimit,
                // metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan insuranceHoldingId, sessionId, participantId, rulesetVersionId, productCode, premium, usageLimit,
                // metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                rulesetVersionId = request.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan insuranceHoldingId, sessionId, participantId, rulesetVersionId, productCode, premium, usageLimit,
                // metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                productCode,
                // Meneruskan objek anonim yang mengelompokkan insuranceHoldingId, sessionId, participantId, rulesetVersionId, productCode, premium, usageLimit,
                // metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                premium,
                // Meneruskan objek anonim yang mengelompokkan insuranceHoldingId, sessionId, participantId, rulesetVersionId, productCode, premium, usageLimit,
                // metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                usageLimit,
                // Meneruskan objek anonim yang mengelompokkan insuranceHoldingId, sessionId, participantId, rulesetVersionId, productCode, premium, usageLimit,
                // metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                metadataJson,
                // Meneruskan objek anonim yang mengelompokkan insuranceHoldingId, sessionId, participantId, rulesetVersionId, productCode, premium, usageLimit,
                // metadataJson, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                eventId = request.EventId
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam
            // ProjectInsurancePurchasedAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode ProjectInsurancePurchasedAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectInsurancePurchasedAsync.
    }

    // Mendefinisikan metode `ProjectInsuranceUsedAsync` dengan hasil bertipe `Task`; operasi ini menangani project asuransi used asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private static async Task ProjectInsuranceUsedAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectInsuranceUsedAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectInsuranceUsedAsync.
    {
        // Menyiapkan variabel lokal `affected` untuk nilai affected dengan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new
        // CommandDefinition( ””” update session_participant_insurances asset set remaining_uses = greatest(remaining_uses - 1, 0), last_event_id =
        // @eventId, metadata_json = asset.me...`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama
        // operasi belum selesai. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var affected = await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update
            // session_participant_insurances asset`.
            // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set remaining_uses = greatest(remaining_uses - 1, 0),`.
            // Baris literal 4: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metadata_json =
            // asset.metadata_json || jsonb_build_object(`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'used_count',
            // coalesce((asset.metadata_json->>'used_count')::int, 0) + 1,`.
            // Baris literal 7: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `'last_used_event_id', @eventId,`.
            // Baris literal 8: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `'last_used_at', @usedAt`.
            // Baris literal 9: Pembatas literal/penutup `),`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `status = case`.
            // Baris literal 11: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when greatest(remaining_uses - 1, 0) = 0 then 'INACTIVE'`.
            // Baris literal 12: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `else 'ACTIVE'`.
            // Baris literal 13: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `end,`.
            // Baris literal 14: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 15: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where asset.session_participant_insurance_id = (`.
            // Baris literal 16: SELECT menentukan nilai atau kolom yang dikembalikan query: `select candidate.session_participant_insurance_id`.
            // Baris literal 17: FROM memilih tabel/subquery sumber pembacaan: `from session_participant_insurances candidate`.
            // Baris literal 18: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where candidate.session_participant_id = @participantId`.
            // Baris literal 19: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and candidate.status = 'ACTIVE'`.
            // Baris literal 20: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and candidate.remaining_uses > 0`.
            // Baris literal 21: ORDER BY mengatur urutan hasil SQL agar pembacaan atau pagination konsisten: `order by candidate.created_at asc`.
            // Baris literal 22: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
            // Baris literal 23: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 24: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            update session_participant_insurances asset
            set remaining_uses = greatest(remaining_uses - 1, 0),
                last_event_id = @eventId,
                metadata_json = asset.metadata_json || jsonb_build_object(
                    'used_count', coalesce((asset.metadata_json->>'used_count')::int, 0) + 1,
                    'last_used_event_id', @eventId,
                    'last_used_at', @usedAt
                ),
                status = case
                    when greatest(remaining_uses - 1, 0) = 0 then 'INACTIVE'
                    else 'ACTIVE'
                end,
                updated_at = now()
            where asset.session_participant_insurance_id = (
                select candidate.session_participant_insurance_id
                from session_participant_insurances candidate
                where candidate.session_participant_id = @participantId
                  and candidate.status = 'ACTIVE'
                  and candidate.remaining_uses > 0
                order by candidate.created_at asc
                limit 1
            )
            """,
            // Meneruskan objek anonim yang mengelompokkan participantId, eventId, usedAt sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectInsuranceUsedAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan participantId, eventId, usedAt sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan participantId, eventId, usedAt sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                eventId = request.EventId,
                // Meneruskan objek anonim yang mengelompokkan participantId, eventId, usedAt sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                usedAt = request.Timestamp.ToUniversalTime()
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ProjectInsuranceUsedAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));

        // Memeriksa perbandingan ketidaksamaan antara `affected` dan `1`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ProjectInsuranceUsedAsync.
        if (affected != 1)
        // Membuka scope cabang if untuk kondisi `affected != 1`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ProjectInsuranceUsedAsync.
        {
            // Menghentikan alur dengan melempar objek baru bertipe `InvalidOperationException` dengan argumen (”Penggunaan asuransi harus mengurangi tepat satu
            // polis aktif.”) dalam ProjectInsuranceUsedAsync; pemanggil atau middleware penanganan error menerima kegagalan ini.
            throw new InvalidOperationException("Penggunaan asuransi harus mengurangi tepat satu polis aktif.");
        // Menutup scope cabang if untuk kondisi `affected != 1`; bagian berikut berada di luar batas blok tersebut dalam ProjectInsuranceUsedAsync.
        }
    // Menutup scope metode ProjectInsuranceUsedAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectInsuranceUsedAsync.
    }

    // Mendefinisikan metode `ProjectTieBreakerAsync` dengan hasil bertipe `Task`; operasi ini menangani project tie breaker asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi;
    // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data; Parameter `tx`
    // bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `ct` bertipe
    // `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti.
    private async Task ProjectTieBreakerAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectTieBreakerAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectTieBreakerAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadTieBreaker(request.Payload, out var tieNumber)`; blok if hanya dijalankan ketika kondisi ini
        // bernilai benar dalam ProjectTieBreakerAsync.
        if (!_payloadReader.TryReadTieBreaker(request.Payload, out var tieNumber))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadTieBreaker(request.Payload, out var tieNumber)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam ProjectTieBreakerAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectTieBreakerAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadTieBreaker(request.Payload, out var tieNumber)`; bagian berikut berada di luar
        // batas blok tersebut dalam ProjectTieBreakerAsync.
        }

        // Menyiapkan variabel lokal `cardCode` untuk nilai kartu kode dengan `ReadOptionalCode(request.Payload, ”card_code”)` bila tidak null; jika null
        // gunakan `ReadOptionalCode(request.Payload, ”tie_breaker_code”)` sebagai nilai pengganti. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var cardCode = ReadOptionalCode(request.Payload, "card_code")
                       // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: ReadOptionalCode(request.Payload, ”tie_breaker_code”); dalam
                       // ProjectTieBreakerAsync.
                       ?? ReadOptionalCode(request.Payload, "tie_breaker_code");
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” insert into
        // session_participant_tie_breakers ( session_participant_tie_breaker_id, session_id, session_participant_id, ruleset_version_id, ruleset_g...`;
        // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // ProjectTieBreakerAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_participant_tie_breakers (`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun:
            // `session_participant_tie_breaker_id,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_participant_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_version_id,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_game_asset_id,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `tie_number,`.
            // Baris literal 9: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `source_event_id,`.
            // Baris literal 10: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `assigned_at,`.
            // Baris literal 11: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metadata_json`.
            // Baris literal 12: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 13: SELECT menentukan nilai atau kolom yang dikembalikan query: `select`.
            // Baris literal 14: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@tieBreakerId,`.
            // Baris literal 15: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@sessionId,`.
            // Baris literal 16: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@participantId,`.
            // Baris literal 17: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@rulesetVersionId,`.
            // Baris literal 18: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `rga.ruleset_game_asset_id,`.
            // Baris literal 19: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@tieNumber,`.
            // Baris literal 20: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@eventId,`.
            // Baris literal 21: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@assignedAt,`.
            // Baris literal 22: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'{}'::jsonb`.
            // Baris literal 23: FROM memilih tabel/subquery sumber pembacaan: `from ruleset_game_assets rga`.
            // Baris literal 24: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where rga.ruleset_version_id = @rulesetVersionId`.
            // Baris literal 25: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and rga.asset_type = 'TIE_BREAKER'`.
            // Baris literal 26: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (`.
            // Baris literal 27: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `lower(rga.asset_code) = lower(@cardCode)`.
            // Baris literal 28: Melanjutkan kondisi SQL dengan OR (alternatif syarat yang dapat terpenuhi): `or (`.
            // Baris literal 29: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@cardCode is null`.
            // Baris literal 30: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and (rga.metadata_json->>'tie_number')::int =
            // @tieNumber`.
            // Baris literal 31: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 32: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 33: LIMIT membatasi jumlah baris yang dikembalikan query: `limit 1`.
            // Baris literal 34: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict
            // (session_participant_id) do update`.
            // Baris literal 35: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set tie_number = excluded.tie_number,`.
            // Baris literal 36: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `ruleset_game_asset_id =
            // excluded.ruleset_game_asset_id,`.
            // Baris literal 37: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `source_event_id =
            // excluded.source_event_id,`.
            // Baris literal 38: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `assigned_at =
            // excluded.assigned_at`.
            // Baris literal 39: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            insert into session_participant_tie_breakers (
                session_participant_tie_breaker_id,
                session_id,
                session_participant_id,
                ruleset_version_id,
                ruleset_game_asset_id,
                tie_number,
                source_event_id,
                assigned_at,
                metadata_json
            )
            select
                @tieBreakerId,
                @sessionId,
                @participantId,
                @rulesetVersionId,
                rga.ruleset_game_asset_id,
                @tieNumber,
                @eventId,
                @assignedAt,
                '{}'::jsonb
            from ruleset_game_assets rga
            where rga.ruleset_version_id = @rulesetVersionId
              and rga.asset_type = 'TIE_BREAKER'
              and (
                  lower(rga.asset_code) = lower(@cardCode)
                  or (
                      @cardCode is null
                      and (rga.metadata_json->>'tie_number')::int = @tieNumber
                  )
              )
            limit 1
            on conflict (session_participant_id) do update
            set tie_number = excluded.tie_number,
                ruleset_game_asset_id = excluded.ruleset_game_asset_id,
                source_event_id = excluded.source_event_id,
                assigned_at = excluded.assigned_at
            """,
            // Meneruskan objek anonim yang mengelompokkan tieBreakerId, sessionId, participantId, rulesetVersionId, tieNumber, cardCode, eventId, assignedAt
            // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new
            // Membuka scope objek anonim yang mengelompokkan beberapa nilai; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ProjectTieBreakerAsync.
            {
                // Meneruskan objek anonim yang mengelompokkan tieBreakerId, sessionId, participantId, rulesetVersionId, tieNumber, cardCode, eventId, assignedAt
                // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                tieBreakerId = Guid.NewGuid(),
                // Meneruskan objek anonim yang mengelompokkan tieBreakerId, sessionId, participantId, rulesetVersionId, tieNumber, cardCode, eventId, assignedAt
                // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                sessionId = request.SessionId,
                // Meneruskan objek anonim yang mengelompokkan tieBreakerId, sessionId, participantId, rulesetVersionId, tieNumber, cardCode, eventId, assignedAt
                // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                participantId,
                // Meneruskan objek anonim yang mengelompokkan tieBreakerId, sessionId, participantId, rulesetVersionId, tieNumber, cardCode, eventId, assignedAt
                // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                rulesetVersionId = request.RulesetVersionId,
                // Meneruskan objek anonim yang mengelompokkan tieBreakerId, sessionId, participantId, rulesetVersionId, tieNumber, cardCode, eventId, assignedAt
                // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                tieNumber,
                // Meneruskan objek anonim yang mengelompokkan tieBreakerId, sessionId, participantId, rulesetVersionId, tieNumber, cardCode, eventId, assignedAt
                // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                cardCode,
                // Meneruskan objek anonim yang mengelompokkan tieBreakerId, sessionId, participantId, rulesetVersionId, tieNumber, cardCode, eventId, assignedAt
                // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                eventId,
                // Meneruskan objek anonim yang mengelompokkan tieBreakerId, sessionId, participantId, rulesetVersionId, tieNumber, cardCode, eventId, assignedAt
                // sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
                assignedAt = request.Timestamp.ToUniversalTime()
            // Menutup scope objek anonim yang mengelompokkan beberapa nilai; bagian berikut berada di luar batas blok tersebut dalam ProjectTieBreakerAsync.
            },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode ProjectTieBreakerAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectTieBreakerAsync.
    }

    // Mendefinisikan metode `ProjectPensionRankingAsync` dengan hasil bertipe `Task`; operasi ini menangani project pension ranking asinkron. async
    // memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `participantId` bertipe `Guid` membawa
    // nilai participant identitas; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil
    // basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter
    // `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private async Task ProjectPensionRankingAsync(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode ProjectPensionRankingAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ProjectPensionRankingAsync.
    {
        // Memeriksa kebalikan kondisi `_payloadReader.TryReadRankAwarded(request.Payload, out _, out var points)`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ProjectPensionRankingAsync.
        if (!_payloadReader.TryReadRankAwarded(request.Payload, out _, out var points))
        // Membuka scope cabang if untuk kondisi `!_payloadReader.TryReadRankAwarded(request.Payload, out _, out var points)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam ProjectPensionRankingAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam ProjectPensionRankingAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak
            // dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `!_payloadReader.TryReadRankAwarded(request.Payload, out _, out var points)`; bagian berikut berada di luar
        // batas blok tersebut dalam ProjectPensionRankingAsync.
        }

        // Menjalankan hasil operasi asinkron memanggil `AddHappinessAsync` dengan `participantId`, `points`, `request.EventId`, `conn`, `tx`, `ct`; await
        // menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam ProjectPensionRankingAsync.
        await AddHappinessAsync(participantId, points, request.EventId, conn, tx, ct);
    // Menutup scope metode ProjectPensionRankingAsync; bagian berikut berada di luar batas blok tersebut dalam ProjectPensionRankingAsync.
    }

    // Mendefinisikan metode `AddHappinessAsync` dengan hasil bertipe `Task`; operasi ini menangani add kebahagiaan asinkron. async memungkinkan metode
    // menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `participantId` bertipe `Guid` membawa nilai
    // participant identitas; Parameter `points` bertipe `int` membawa nilai poin; Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk
    // pencatatan dan pemeriksaan duplikasi; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca
    // hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan;
    // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
    // aplikasi berhenti.
    private static async Task AddHappinessAsync(
        // Parameter `participantId` bertipe `Guid` membawa nilai participant identitas.
        Guid participantId,
        // Parameter `points` bertipe `int` membawa nilai poin.
        int points,
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode AddHappinessAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AddHappinessAsync.
    {
        // Memeriksa perbandingan kesamaan antara `points` dan `0`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam AddHappinessAsync.
        if (points == 0)
        // Membuka scope cabang if untuk kondisi `points == 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AddHappinessAsync.
        {
            // Mengakhiri eksekusi lebih awal dalam AddHappinessAsync tanpa mengembalikan nilai; pernyataan sesudah return pada jalur ini tidak dijalankan.
            return;
        // Menutup scope cabang if untuk kondisi `points == 0`; bagian berikut berada di luar batas blok tersebut dalam AddHappinessAsync.
        }

        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” update
        // session_participant_balances set happiness = greatest(0, happiness + @points), last_event_id = @eventId, updated_at = now() where
        // session_par...`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai
        // dalam AddHappinessAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update
            // session_participant_balances`.
            // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set happiness = greatest(0, happiness + @points),`.
            // Baris literal 4: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId,`.
            // Baris literal 5: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `updated_at = now()`.
            // Baris literal 6: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_participant_id = @participantId`.
            // Baris literal 7: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            update session_participant_balances
            set happiness = greatest(0, happiness + @points),
                last_event_id = @eventId,
                updated_at = now()
            where session_participant_id = @participantId
            """,
            // Meneruskan objek anonim yang mengelompokkan participantId, points, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new { participantId, points, eventId },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode AddHappinessAsync; bagian berikut berada di luar batas blok tersebut dalam AddHappinessAsync.
    }

    // Mendefinisikan metode `MarkIncompleteMissionsFailedAsync` dengan hasil bertipe `Task`; operasi ini menangani mark incomplete misi failed
    // asinkron. async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId`
    // bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `eventId` bertipe `Guid` membawa identitas
    // unik event untuk pencatatan dan pemeriksaan duplikasi; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim
    // perintah dan membaca hasil basis data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan
    // sebagai satu kesatuan; Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil
    // membatalkan permintaan atau aplikasi berhenti.
    private static async Task MarkIncompleteMissionsFailedAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode MarkIncompleteMissionsFailedAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // MarkIncompleteMissionsFailedAsync.
    {
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” update
        // session_participant_collection_missions set is_failed = true, reward_applied = false, last_event_id = @eventId where session_id = @sessionId
        // ...`; nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // MarkIncompleteMissionsFailedAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: UPDATE memilih tabel yang akan diperbarui; kolom dan batas baris ditentukan oleh SET/WHERE: `update
            // session_participant_collection_missions`.
            // Baris literal 3: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set is_failed = true,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `reward_applied = false,`.
            // Baris literal 5: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `last_event_id = @eventId`.
            // Baris literal 6: WHERE menyaring baris agar hanya data yang memenuhi syarat diproses: `where session_id = @sessionId`.
            // Baris literal 7: Melanjutkan kondisi SQL dengan AND (syarat tambahan wajib terpenuhi): `and not is_completed`.
            // Baris literal 8: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            update session_participant_collection_missions
            set is_failed = true,
                reward_applied = false,
                last_event_id = @eventId
            where session_id = @sessionId
              and not is_completed
            """,
            // Meneruskan objek anonim yang mengelompokkan sessionId, eventId sebagai satu nilai sebagai argumen ke konstruktor `CommandDefinition`.
            new { sessionId, eventId },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode MarkIncompleteMissionsFailedAsync; bagian berikut berada di luar batas blok tersebut dalam
    // MarkIncompleteMissionsFailedAsync.
    }

    // Mendefinisikan metode `UpdateProjectionCheckpointAsync` dengan hasil bertipe `Task`; operasi ini menangani update projection checkpoint asinkron.
    // async memungkinkan metode menunggu operasi I/O dengan await dan mengembalikan penyelesaian melalui Task. Masukan: Parameter `sessionId` bertipe
    // `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `sequenceNumber` bertipe `long` membawa nomor urut
    // event yang menentukan urutan pemrosesan riwayat permainan; Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan
    // pemeriksaan duplikasi; Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis
    // data; Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan; Parameter `ct`
    // bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi
    // berhenti.
    private static async Task UpdateProjectionCheckpointAsync(
        // Parameter `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
        Guid sessionId,
        // Parameter `sequenceNumber` bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan.
        long sequenceNumber,
        // Parameter `eventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi.
        Guid eventId,
        // Parameter `conn` bertipe `NpgsqlConnection` membawa koneksi PostgreSQL untuk mengirim perintah dan membaca hasil basis data.
        NpgsqlConnection conn,
        // Parameter `tx` bertipe `NpgsqlTransaction` membawa transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan.
        NpgsqlTransaction tx,
        // Parameter `ct` bertipe `CancellationToken` membawa sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau
        // aplikasi berhenti.
        CancellationToken ct)
    // Membuka scope metode UpdateProjectionCheckpointAsync; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
    // UpdateProjectionCheckpointAsync.
    {
        // Menjalankan hasil operasi asinkron menjalankan perintah SQL melalui `conn` menggunakan `new CommandDefinition( ””” insert into
        // session_projection_checkpoints ( session_id, last_sequence_number, last_event_id, projected_at, status, metadata_json ) values ( @sessio...`;
        // nilai hasil menunjukkan jumlah baris yang terpengaruh; await menunggu hasil tanpa memblokir thread selama operasi belum selesai dalam
        // UpdateProjectionCheckpointAsync.
        await conn.ExecuteAsync(new CommandDefinition(
            // Meneruskan literal multiline yang dirinci pada komentar di dekat deklarasinya sebagai argumen ke konstruktor `CommandDefinition`.
            // Penjelasan literal multiline berikut diletakkan di luar tanda kutip agar nilai SQL/JSON/teks yang digunakan program tetap persis sama.
            // Baris literal 1: Pembatas literal/penutup `”””`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 2: INSERT INTO menetapkan tabel dan kolom tujuan penambahan rekaman: `insert into session_projection_checkpoints (`.
            // Baris literal 3: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `session_id,`.
            // Baris literal 4: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_sequence_number,`.
            // Baris literal 5: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id,`.
            // Baris literal 6: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `projected_at,`.
            // Baris literal 7: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `status,`.
            // Baris literal 8: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metadata_json`.
            // Baris literal 9: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 10: VALUES menyediakan nilai baris baru sesuai urutan kolom INSERT; placeholder @ diikat ke parameter perintah: `values (`.
            // Baris literal 11: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@sessionId,`.
            // Baris literal 12: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@sequenceNumber,`.
            // Baris literal 13: Meneruskan kolom/ekspresi SQL dengan placeholder @ yang nilainya diikat dari parameter perintah, bukan digabung sebagai teks
            // SQL: `@eventId,`.
            // Baris literal 14: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `now(),`.
            // Baris literal 15: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `'IDLE',`.
            // Baris literal 16: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `jsonb_build_object('source',
            // 'SessionEventProjector')`.
            // Baris literal 17: Pembatas literal/penutup `)`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            // Baris literal 18: ON CONFLICT menentukan penanganan saat INSERT bertabrakan dengan kunci unik yang sudah ada: `on conflict (session_id) do
            // update`.
            // Baris literal 19: SET menetapkan nilai kolom yang diperbarui oleh UPDATE: `set last_sequence_number =
            // greatest(session_projection_checkpoints.last_sequence_number, excluded.last_sequence_number),`.
            // Baris literal 20: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `last_event_id = case`.
            // Baris literal 21: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `when excluded.last_sequence_number >=
            // session_projection_checkpoints.last_sequence_number then excluded.last_event_id`.
            // Baris literal 22: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `else session_projection_checkpoints.last_event_id`.
            // Baris literal 23: Menyusun pilihan nilai bersyarat di dalam ekspresi SQL: `end,`.
            // Baris literal 24: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `projected_at = now(),`.
            // Baris literal 25: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `status = 'IDLE',`.
            // Baris literal 26: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `error_message = null,`.
            // Baris literal 27: Meneruskan daftar kolom, ekspresi, atau struktur teks literal untuk perintah yang sedang disusun: `metadata_json =
            // session_projection_checkpoints.metadata_json || excluded.metadata_json`.
            // Baris literal 28: Pembatas literal/penutup `”””,`; menandai batas teks dan tidak menambahkan komentar ke nilai string.
            """
            insert into session_projection_checkpoints (
                session_id,
                last_sequence_number,
                last_event_id,
                projected_at,
                status,
                metadata_json
            )
            values (
                @sessionId,
                @sequenceNumber,
                @eventId,
                now(),
                'IDLE',
                jsonb_build_object('source', 'SessionEventProjector')
            )
            on conflict (session_id) do update
            set last_sequence_number = greatest(session_projection_checkpoints.last_sequence_number, excluded.last_sequence_number),
                last_event_id = case
                    when excluded.last_sequence_number >= session_projection_checkpoints.last_sequence_number then excluded.last_event_id
                    else session_projection_checkpoints.last_event_id
                end,
                projected_at = now(),
                status = 'IDLE',
                error_message = null,
                metadata_json = session_projection_checkpoints.metadata_json || excluded.metadata_json
            """,
            // Meneruskan objek anonim yang mengelompokkan sessionId, sequenceNumber, eventId sebagai satu nilai sebagai argumen ke konstruktor
            // `CommandDefinition`.
            new { sessionId, sequenceNumber, eventId },
            // Meneruskan `tx` (transaksi basis data yang menggabungkan perubahan sebagai satu kesatuan) sebagai argumen ke konstruktor `CommandDefinition`.
            tx,
            // Meneruskan `ct` (sinyal pembatalan agar operasi dapat dihentikan ketika pemanggil membatalkan permintaan atau aplikasi berhenti) sebagai argumen
            // bernama `cancellationToken`.
            cancellationToken: ct));
    // Menutup scope metode UpdateProjectionCheckpointAsync; bagian berikut berada di luar batas blok tersebut dalam UpdateProjectionCheckpointAsync.
    }

    // Mendefinisikan metode `ReadOptionalCode` dengan hasil bertipe `string?`; operasi ini menangani read optional kode. Masukan: Parameter `payload`
    // bertipe `JsonElement` membawa muatan detail event dalam format JSON; Parameter `propertyName` bertipe `string` membawa nilai property nama.
    private string? ReadOptionalCode(JsonElement payload, string propertyName)
    // Membuka scope metode ReadOptionalCode; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ReadOptionalCode.
    {
        // Mengembalikan hasil pemilihan bersyarat: ketika `_payloadReader.TryGetOptionalString(payload, propertyName, out var value) &&
        // !string.IsNullOrWhiteSpace(value)` benar gunakan `value.Trim()`, jika tidak gunakan `null` kepada pemanggil dalam ReadOptionalCode; eksekusi
        // jalur ini selesai setelah nilai hasil ditentukan.
        return _payloadReader.TryGetOptionalString(payload, propertyName, out var value) &&
               // Menggunakan kebalikan kondisi `string.IsNullOrWhiteSpace(value)` sebagai bagian ekspresi yang sedang disusun dalam ReadOptionalCode.
               !string.IsNullOrWhiteSpace(value)
            // Menentukan hasil yang dipakai saat kondisi operator ternary bernilai benar: value.Trim() dalam ReadOptionalCode.
            ? value.Trim()
            // Menentukan hasil alternatif saat kondisi operator ternary bernilai salah: null; dalam ReadOptionalCode.
            : null;
    // Menutup scope metode ReadOptionalCode; bagian berikut berada di luar batas blok tersebut dalam ReadOptionalCode.
    }

    // Mendefinisikan metode `ResolvePhase` dengan hasil bertipe `string`; operasi ini menangani resolve phase. Masukan: Parameter `weekday` bertipe
    // `string` membawa nilai weekday; Parameter `isGameOver` bertipe `bool` membawa nilai berstatus game over.
    private static string ResolvePhase(string weekday, bool isGameOver)
    // Membuka scope metode ResolvePhase; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolvePhase.
    {
        // Memeriksa `isGameOver` (nilai berstatus game over); blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ResolvePhase.
        if (isGameOver)
        // Membuka scope cabang if untuk kondisi `isGameOver`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ResolvePhase.
        {
            // Mengembalikan nilai literal `”GAME_END”` kepada pemanggil dalam ResolvePhase; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return "GAME_END";
        // Menutup scope cabang if untuk kondisi `isGameOver`; bagian berikut berada di luar batas blok tersebut dalam ResolvePhase.
        }

        // Mengembalikan hasil pemetaan `weekday.Trim().ToUpperInvariant()` melalui cabang pola switch yang cocok kepada pemanggil dalam ResolvePhase;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return weekday.Trim().ToUpperInvariant() switch
        // Membuka scope pemetaan switch atas `weekday.Trim().ToUpperInvariant()`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ResolvePhase.
        {
            // Untuk pola `”FRI”`, menghasilkan nilai literal `”DONATION_DAY”` sebagai hasil switch.
            "FRI" => "DONATION_DAY",
            // Untuk pola `”SAT”`, menghasilkan nilai literal `”GOLD_INVESTMENT_DAY”` sebagai hasil switch.
            "SAT" => "GOLD_INVESTMENT_DAY",
            // Untuk pola `”SUN”`, menghasilkan nilai literal `”DAY_END”` sebagai hasil switch.
            "SUN" => "DAY_END",
            // Untuk pola `_`, menghasilkan nilai literal `”PLAYER_TURN”` sebagai hasil switch.
            _ => "PLAYER_TURN"
        // Menutup scope pemetaan switch atas `weekday.Trim().ToUpperInvariant()`; bagian berikut berada di luar batas blok tersebut dalam ResolvePhase.
        };
    // Menutup scope metode ResolvePhase; bagian berikut berada di luar batas blok tersebut dalam ResolvePhase.
    }
// Menutup scope tipe SessionEventProjector; bagian berikut berada di luar batas blok tersebut.
}
