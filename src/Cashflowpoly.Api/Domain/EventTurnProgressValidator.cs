// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui EventTurnProgressValidator.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;
// Mengimpor namespace `Microsoft.AspNetCore.Http` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Microsoft.AspNetCore.Http;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan tipe class `EventTurnProgressValidator` yang mewarisi atau menerapkan `IEventTurnProgressValidator`; sealed mencegah tipe ini
// diturunkan lagi.
internal sealed class EventTurnProgressValidator : IEventTurnProgressValidator
// Membuka scope tipe EventTurnProgressValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendeklarasikan field bertipe `EventPayloadReader`: `_payloadReader` menyimpan nilai payload pembaca dengan nilai awal objek baru dengan tipe
    // mengikuti konteks tujuan dan argumen (). readonly membatasi penggantian referensi/nilai field pada deklarasi atau konstruktor. static membuat
    // field menjadi milik tipe dan dibagikan antar instance.
    private static readonly EventPayloadReader _payloadReader = new();

    // Mendefinisikan metode `RequiresHistory` dengan hasil bertipe `bool`; operasi ini menangani requires history. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `config` bertipe `RulesetConfig`
    // membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
    public bool RequiresHistory(EventRequest request, RulesetConfig config)
    // Membuka scope metode RequiresHistory; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam RequiresHistory.
    {
        // Mengembalikan memanggil `GameActionCatalog.Is` dengan `request.ActionType`, `request.Payload`, `GameActionCatalog.AkhirGiliran` kepada pemanggil
        // dalam RequiresHistory; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.AkhirGiliran);
    // Menutup scope metode RequiresHistory; bagian berikut berada di luar batas blok tersebut dalam RequiresHistory.
    }

    // Mendefinisikan metode `TryValidate` dengan hasil bertipe `bool`; operasi ini menangani try validate. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `config` bertipe `RulesetConfig`
    // membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe `IEnumerable<EventDb>` membawa
    // nilai history; Parameter `participantCount` bertipe `int` membawa nilai participant jumlah; Parameter `result` bertipe
    // `EventDomainValidationResult` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out mengembalikan nilai melalui parameter
    // dan harus diisi oleh metode.
    public bool TryValidate(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history,
        // Parameter `participantCount` bertipe `int` membawa nilai participant jumlah.
        int participantCount,
        // Parameter `result` bertipe `EventDomainValidationResult` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out EventDomainValidationResult result)
    // Membuka scope metode TryValidate; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
    {
        // Memeriksa memanggil `GameActionCatalog.Is` dengan `request.ActionType`, `request.Payload`, `GameActionCatalog.AkhirGiliran`; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam TryValidate.
        if (GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.AkhirGiliran))
        // Membuka scope cabang if untuk kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.AkhirGiliran)`;
        // pernyataan/deklarasi berikut berada di dalam batas blok ini dalam TryValidate.
        {
            // Memperbarui `result` menggunakan memanggil `ValidateTurnEnded` dengan `request`, `config`, `history`, `participantCount` dalam TryValidate.
            result = ValidateTurnEnded(request, config, history, participantCount);
            // Mengembalikan true, yaitu kondisi aktif/terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return true;
        // Menutup scope cabang if untuk kondisi `GameActionCatalog.Is(request.ActionType, request.Payload, GameActionCatalog.AkhirGiliran)`; bagian berikut
        // berada di luar batas blok tersebut dalam TryValidate.
        }

        // Memperbarui `result` menggunakan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) dalam TryValidate.
        result = EventDomainValidationResult.Valid;
        // Mengembalikan false, yaitu kondisi nonaktif/tidak terpenuhi kepada pemanggil dalam TryValidate; eksekusi jalur ini selesai setelah nilai hasil
        // ditentukan.
        return false;
    // Menutup scope metode TryValidate; bagian berikut berada di luar batas blok tersebut dalam TryValidate.
    }

    // Mendefinisikan metode `ValidateTurnEnded` dengan hasil bertipe `EventDomainValidationResult`; operasi ini menangani validate giliran ended.
    // Masukan: Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter
    // `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe
    // `IEnumerable<EventDb>` membawa nilai history; Parameter `participantCount` bertipe `int` membawa nilai participant jumlah.
    private EventDomainValidationResult ValidateTurnEnded(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history,
        // Parameter `participantCount` bertipe `int` membawa nilai participant jumlah.
        int participantCount)
    // Membuka scope metode ValidateTurnEnded; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateTurnEnded.
    {
        // Menyiapkan variabel lokal `sessionEvents` untuk nilai sesi event dengan mematerialisasi urutan `history .Where(e => e.SessionId ==
        // request.SessionId)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var sessionEvents = history
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => e.SessionId == request.SessionId) dalam ValidateTurnEnded; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(e => e.SessionId == request.SessionId)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam ValidateTurnEnded; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `turnEvents` untuk nilai giliran event dengan mematerialisasi urutan `sessionEvents .Where(e => e.SessionId ==
        // request.SessionId && e.DayIndex == request.DayIndex && e.UserId.HasValue)` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var turnEvents = sessionEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => e.SessionId == request.SessionId && dalam ValidateTurnEnded; token
            // pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(e => e.SessionId == request.SessionId &&
                        // Meneruskan fungsi lambda `e => e.SessionId == request.SessionId && e.DayIndex == request.DayIndex && e.UserId.HasValue` yang dijalankan oleh
                        // operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `sessionEvents .Where`.
                        e.DayIndex == request.DayIndex &&
                        // Meneruskan fungsi lambda `e => e.SessionId == request.SessionId && e.DayIndex == request.DayIndex && e.UserId.HasValue` yang dijalankan oleh
                        // operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `sessionEvents .Where`.
                        e.UserId.HasValue)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam ValidateTurnEnded; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .ToList();
        // Menyiapkan variabel lokal `consumingActions` untuk nilai consuming aksi dengan mematerialisasi urutan `turnEvents .Where(e =>
        // string.Equals(e.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase) && GameActionCatalog.GetPlayerActionSlotPolicy( e.ActionType,
        // _payloadReader.Re...` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var consumingActions = turnEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => string.Equals(e.ActorType, ”PLAYER”,
            // StringComparison.OrdinalIgnoreCase) && dalam ValidateTurnEnded; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .Where(e => string.Equals(e.ActorType, "PLAYER", StringComparison.OrdinalIgnoreCase) &&
                        // Meneruskan fungsi lambda `e => string.Equals(e.ActorType, ”PLAYER”, StringComparison.OrdinalIgnoreCase) &&
                        // GameActionCatalog.GetPlayerActionSlotPolicy( e.ActionType, _payloadReader.ReadPayload(string.I...` yang dijalankan oleh operasi pemanggil untuk
                        // memproses setiap masukan sebagai argumen ke `turnEvents .Where`.
                        GameActionCatalog.GetPlayerActionSlotPolicy(
                            // Meneruskan `e.ActionType` (nilai aksi jenis) sebagai argumen ke `GameActionCatalog.GetPlayerActionSlotPolicy`.
                            e.ActionType,
                            // Meneruskan memanggil `_payloadReader.ReadPayload` dengan `string.IsNullOrWhiteSpace(e.Payload) ? ”{}” : e.Payload` sebagai argumen ke
                            // `GameActionCatalog.GetPlayerActionSlotPolicy`; Meneruskan hasil pemilihan bersyarat: ketika `string.IsNullOrWhiteSpace(e.Payload)` benar gunakan
                            // `”{}”`, jika tidak gunakan `e.Payload` sebagai argumen ke `_payloadReader.ReadPayload`; Meneruskan `e.Payload` (muatan detail event dalam format
                            // JSON) sebagai argumen ke `string.IsNullOrWhiteSpace`.
                            _payloadReader.ReadPayload(string.IsNullOrWhiteSpace(e.Payload) ? "{}" : e.Payload)) == PlayerActionSlotPolicy.Consumes)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam ValidateTurnEnded; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .ToList();

        // Memeriksa memanggil `IsRegularActionWeekday` dengan `request.Weekday`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
        // ValidateTurnEnded.
        if (IsRegularActionWeekday(request.Weekday))
        // Membuka scope cabang if untuk kondisi `IsRegularActionWeekday(request.Weekday)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ValidateTurnEnded.
        {
            // Menyiapkan variabel lokal `participantIds` untuk nilai participant identitas dengan mematerialisasi urutan `consumingActions .Where(e =>
            // e.UserId.HasValue) .Select(e => e.UserId!.Value) .Distinct()` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam memori. Tipe
            // variabel disimpulkan dari ekspresi nilai awal.
            var participantIds = consumingActions
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => e.UserId.HasValue) dalam ValidateTurnEnded; token pada baris ini
                // menyambungkan bagian kode sebelum dan sesudahnya.
                .Where(e => e.UserId.HasValue)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Select(e => e.UserId!.Value) dalam ValidateTurnEnded; token pada baris ini
                // menyambungkan bagian kode sebelum dan sesudahnya.
                .Select(e => e.UserId!.Value)
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Distinct() dalam ValidateTurnEnded; token pada baris ini menyambungkan bagian
                // kode sebelum dan sesudahnya.
                .Distinct()
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam ValidateTurnEnded; token pada baris ini menyambungkan bagian
                // kode sebelum dan sesudahnya.
                .ToList();
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `participantIds.Count != participantCount` dan
            // `participantIds.Any(playerId => consumingActions .Where(e => e.UserId == playerId) .Select(e => e.ActionSlot) .Distinct() .Count() !=
            // config.ActionsPerTurn)`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ValidateTurnEnded.
            if (participantIds.Count != participantCount || participantIds.Any(playerId => consumingActions
                    // Meneruskan fungsi lambda `e => e.UserId == playerId` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                    // `consumingActions .Where`.
                    .Where(e => e.UserId == playerId)
                    // Meneruskan fungsi lambda `e => e.ActionSlot` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke
                    // `consumingActions .Where(e => e.UserId == playerId) .Select`.
                    .Select(e => e.ActionSlot)
                    // Meneruskan fungsi lambda `playerId => consumingActions .Where(e => e.UserId == playerId) .Select(e => e.ActionSlot) .Distinct() .Count() !=
                    // config.ActionsPerTurn` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `participantIds.Any`.
                    .Distinct()
                    // Meneruskan fungsi lambda `playerId => consumingActions .Where(e => e.UserId == playerId) .Select(e => e.ActionSlot) .Distinct() .Count() !=
                    // config.ActionsPerTurn` yang dijalankan oleh operasi pemanggil untuk memproses setiap masukan sebagai argumen ke `participantIds.Any`.
                    .Count() != config.ActionsPerTurn))
            // Membuka scope cabang if untuk kondisi `participantIds.Count != participantCount || participantIds.Any(playerId => consumingActions .Where(e =>
            // e.UserId == playerId) .Select(e => e.ActionSlot) .Distinct() .Count() !...`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateTurnEnded.
            {
                // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Setiap
                // pemain harus menyelesaikan seluruh jatah aksi sebelum giliran berakhir”` kepada pemanggil dalam ValidateTurnEnded; eksekusi jalur ini selesai
                // setelah nilai hasil ditentukan.
                return EventDomainValidationResult.Fail(
                    // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke
                    // `EventDomainValidationResult.Fail`.
                    StatusCodes.Status422UnprocessableEntity,
                    // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "DOMAIN_RULE_VIOLATION",
                    // Meneruskan nilai literal `”Setiap pemain harus menyelesaikan seluruh jatah aksi sebelum giliran berakhir”` sebagai argumen ke
                    // `EventDomainValidationResult.Fail`.
                    "Setiap pemain harus menyelesaikan seluruh jatah aksi sebelum giliran berakhir");
            // Menutup scope cabang if untuk kondisi `participantIds.Count != participantCount || participantIds.Any(playerId => consumingActions .Where(e =>
            // e.UserId == playerId) .Select(e => e.ActionSlot) .Distinct() .Count() !...`; bagian berikut berada di luar batas blok tersebut dalam
            // ValidateTurnEnded.
            }
        // Menutup scope cabang if untuk kondisi `IsRegularActionWeekday(request.Weekday)`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateTurnEnded.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam ValidateTurnEnded.
        else if (request.Weekday.Equals("FRI", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `request.Weekday.Equals(”FRI”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam ValidateTurnEnded.
        {
            // Menyiapkan variabel lokal `donations` untuk nilai donations dengan mematerialisasi urutan `turnEvents .Where(e =>
            // GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.JumatBerkah))` menjadi List; enumerasi dijalankan dan
            // hasilnya disimpan dalam memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var donations = turnEvents
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => GameActionCatalog.Is(e.ActionType,
                // _payloadReader.ReadPayload(e.Payload), GameActionCatalog.JumatBerkah)) dalam ValidateTurnEnded; token pada baris ini menyambungkan bagian kode
                // sebelum dan sesudahnya.
                .Where(e => GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.JumatBerkah))
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam ValidateTurnEnded; token pada baris ini menyambungkan bagian
                // kode sebelum dan sesudahnya.
                .ToList();
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `donations.Select(e => e.UserId!.Value).Distinct().Count() !=
            // participantCount` dan `donations.GroupBy(e => e.UserId!.Value).Any(group => group.Count() != 1)`; sisi kanan diperiksa hanya jika sisi kiri
            // salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateTurnEnded.
            if (donations.Select(e => e.UserId!.Value).Distinct().Count() != participantCount ||
                // Melanjutkan pengolahan dengan mengelompokkan `donations` memakai kunci `e => e.UserId!.Value` agar perhitungan dapat dilakukan per kelompok dalam
                // ValidateTurnEnded.
                donations.GroupBy(e => e.UserId!.Value).Any(group => group.Count() != 1))
            // Membuka scope cabang if untuk kondisi `donations.Select(e => e.UserId!.Value).Distinct().Count() != participantCount || donations.GroupBy(e =>
            // e.UserId!.Value).Any(group => group.Count() != 1)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateTurnEnded.
            {
                // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Setiap
                // pemain harus menyelesaikan tepat satu donasi Jumat sebelum hari berakhir”` kepada pemanggil dalam ValidateTurnEnded; eksekusi jalur ini selesai
                // setelah nilai hasil ditentukan.
                return EventDomainValidationResult.Fail(
                    // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke
                    // `EventDomainValidationResult.Fail`.
                    StatusCodes.Status422UnprocessableEntity,
                    // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "DOMAIN_RULE_VIOLATION",
                    // Meneruskan nilai literal `”Setiap pemain harus menyelesaikan tepat satu donasi Jumat sebelum hari berakhir”` sebagai argumen ke
                    // `EventDomainValidationResult.Fail`.
                    "Setiap pemain harus menyelesaikan tepat satu donasi Jumat sebelum hari berakhir");
            // Menutup scope cabang if untuk kondisi `donations.Select(e => e.UserId!.Value).Distinct().Count() != participantCount || donations.GroupBy(e =>
            // e.UserId!.Value).Any(group => group.Count() != 1)`; bagian berikut berada di luar batas blok tersebut dalam ValidateTurnEnded.
            }
        // Menutup scope cabang if untuk kondisi `request.Weekday.Equals(”FRI”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas
        // blok tersebut dalam ValidateTurnEnded.
        }
        // Menjalankan cabang alternatif ketika kondisi if sebelumnya tidak terpenuhi dalam ValidateTurnEnded.
        else if (request.Weekday.Equals("SAT", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `request.Weekday.Equals(”SAT”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut berada di
        // dalam batas blok ini dalam ValidateTurnEnded.
        {
            // Menyiapkan variabel lokal `hasGoldPrice` untuk nilai memiliki emas harga dengan memeriksa apakah `sessionEvents` memiliki setidaknya satu elemen
            // yang memenuhi `e => e.DayIndex == request.DayIndex && GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload),
            // GameActionCatalog.GoldPriceOpened)`. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var hasGoldPrice = sessionEvents.Any(e => e.DayIndex == request.DayIndex &&
                // Meneruskan `e.ActionType` (nilai aksi jenis) sebagai argumen ke `GameActionCatalog.Is`; Meneruskan memanggil `_payloadReader.ReadPayload` dengan
                // `e.Payload` sebagai argumen ke `GameActionCatalog.Is`; Meneruskan `e.Payload` (muatan detail event dalam format JSON) sebagai argumen ke
                // `_payloadReader.ReadPayload`; Meneruskan `GameActionCatalog.GoldPriceOpened` (nilai emas harga opened) sebagai argumen ke `GameActionCatalog.Is`.
                GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.GoldPriceOpened));
            // Menyiapkan variabel lokal `decisions` untuk nilai decisions dengan mematerialisasi urutan `turnEvents .Where(e =>
            // IsScheduledGoldDecision(e.ActionType, _payloadReader.ReadPayload(e.Payload)))` menjadi List; enumerasi dijalankan dan hasilnya disimpan dalam
            // memori. Tipe variabel disimpulkan dari ekspresi nilai awal.
            var decisions = turnEvents
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => IsScheduledGoldDecision(e.ActionType,
                // _payloadReader.ReadPayload(e.Payload))) dalam ValidateTurnEnded; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
                .Where(e => IsScheduledGoldDecision(e.ActionType, _payloadReader.ReadPayload(e.Payload)))
                // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToList(); dalam ValidateTurnEnded; token pada baris ini menyambungkan bagian
                // kode sebelum dan sesudahnya.
                .ToList();
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!hasGoldPrice || decisions.Select(e =>
            // e.UserId!.Value).Distinct().Count() != participantCount` dan `decisions.GroupBy(e => e.UserId!.Value).Any(group => group.Count() != 1)`; sisi
            // kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateTurnEnded.
            if (!hasGoldPrice ||
                // Melanjutkan pengolahan dengan memetakan setiap elemen `decisions` melalui `e => e.UserId!.Value` menjadi bentuk hasil yang dibutuhkan dalam
                // ValidateTurnEnded.
                decisions.Select(e => e.UserId!.Value).Distinct().Count() != participantCount ||
                // Melanjutkan pengolahan dengan mengelompokkan `decisions` memakai kunci `e => e.UserId!.Value` agar perhitungan dapat dilakukan per kelompok dalam
                // ValidateTurnEnded.
                decisions.GroupBy(e => e.UserId!.Value).Any(group => group.Count() != 1))
            // Membuka scope cabang if untuk kondisi `!hasGoldPrice || decisions.Select(e => e.UserId!.Value).Distinct().Count() != participantCount ||
            // decisions.GroupBy(e => e.UserId!.Value).Any(group => group.Count() != 1)`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateTurnEnded.
            {
                // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Harga
                // emas harus dibuka dan setiap pemain harus memilih beli, jual, atau lewati tepat satu kali pada Sabtu”` kepada pemanggil dalam ValidateTurnEnded;
                // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
                return EventDomainValidationResult.Fail(
                    // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke
                    // `EventDomainValidationResult.Fail`.
                    StatusCodes.Status422UnprocessableEntity,
                    // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "DOMAIN_RULE_VIOLATION",
                    // Meneruskan nilai literal `”Harga emas harus dibuka dan setiap pemain harus memilih beli, jual, atau lewati tepat satu kali pada Sabtu”` sebagai
                    // argumen ke `EventDomainValidationResult.Fail`.
                    "Harga emas harus dibuka dan setiap pemain harus memilih beli, jual, atau lewati tepat satu kali pada Sabtu");
            // Menutup scope cabang if untuk kondisi `!hasGoldPrice || decisions.Select(e => e.UserId!.Value).Distinct().Count() != participantCount ||
            // decisions.GroupBy(e => e.UserId!.Value).Any(group => group.Count() != 1)`; bagian berikut berada di luar batas blok tersebut dalam
            // ValidateTurnEnded.
            }
        // Menutup scope cabang if untuk kondisi `request.Weekday.Equals(”SAT”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar batas
        // blok tersebut dalam ValidateTurnEnded.
        }

        // Menyiapkan variabel lokal `hasUsed` untuk nilai memiliki used dengan mencari properti JSON `”used”`, `_` pada `request.Payload` tanpa menganggap
        // propertinya selalu tersedia. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var hasUsed = request.Payload.TryGetProperty("used", out _);
        // Menyiapkan variabel lokal `hasRemaining` untuk nilai memiliki tersisa dengan mencari properti JSON `”remaining”`, `_` pada `request.Payload`
        // tanpa menganggap propertinya selalu tersedia. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var hasRemaining = request.Payload.TryGetProperty("remaining", out _);
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `hasUsed` dan `hasRemaining`; sisi kanan diperiksa hanya jika sisi kiri
        // salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateTurnEnded.
        if (hasUsed || hasRemaining)
        // Membuka scope cabang if untuk kondisi `hasUsed || hasRemaining`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateTurnEnded.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!hasUsed || !hasRemaining ||
            // !_payloadReader.TryReadActionUsed(request.Payload, out var used, out var remaining) || used != consumingActions.Count` dan `remaining != 0`; sisi
            // kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateTurnEnded.
            if (!hasUsed || !hasRemaining ||
                // Menggunakan kebalikan kondisi `_payloadReader.TryReadActionUsed(request.Payload, out var used, out var remaining)` sebagai bagian ekspresi yang
                // sedang disusun dalam ValidateTurnEnded.
                !_payloadReader.TryReadActionUsed(request.Payload, out var used, out var remaining) ||
                // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `used` dan `consumingActions.Count` dalam ValidateTurnEnded.
                used != consumingActions.Count ||
                // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `remaining` dan `0` dalam ValidateTurnEnded.
                remaining != 0)
            // Membuka scope cabang if untuk kondisi `!hasUsed || !hasRemaining || !_payloadReader.TryReadActionUsed(request.Payload, out var used, out var
            // remaining) || used != consumingActions.Count || remaining != 0`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
            // ValidateTurnEnded.
            {
                // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Nilai
                // used/remaining tidak sesuai dengan riwayat aksi”` kepada pemanggil dalam ValidateTurnEnded; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return EventDomainValidationResult.Fail(
                    // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke
                    // `EventDomainValidationResult.Fail`.
                    StatusCodes.Status422UnprocessableEntity,
                    // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "DOMAIN_RULE_VIOLATION",
                    // Meneruskan nilai literal `”Nilai used/remaining tidak sesuai dengan riwayat aksi”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "Nilai used/remaining tidak sesuai dengan riwayat aksi");
            // Menutup scope cabang if untuk kondisi `!hasUsed || !hasRemaining || !_payloadReader.TryReadActionUsed(request.Payload, out var used, out var
            // remaining) || used != consumingActions.Count || remaining != 0`; bagian berikut berada di luar batas blok tersebut dalam ValidateTurnEnded.
            }
        // Menutup scope cabang if untuk kondisi `hasUsed || hasRemaining`; bagian berikut berada di luar batas blok tersebut dalam ValidateTurnEnded.
        }

        // Menyiapkan variabel lokal `hasFromDay` untuk nilai memiliki dari hari dengan mencari properti JSON `”from_day”`, `_` pada `request.Payload` tanpa
        // menganggap propertinya selalu tersedia. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var hasFromDay = request.Payload.TryGetProperty("from_day", out _);
        // Menyiapkan variabel lokal `hasToDay` untuk nilai memiliki ke hari dengan mencari properti JSON `”to_day”`, `_` pada `request.Payload` tanpa
        // menganggap propertinya selalu tersedia. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var hasToDay = request.Payload.TryGetProperty("to_day", out _);
        // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `hasFromDay` dan `hasToDay`; sisi kanan diperiksa hanya jika sisi kiri
        // salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateTurnEnded.
        if (hasFromDay || hasToDay)
        // Membuka scope cabang if untuk kondisi `hasFromDay || hasToDay`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam
        // ValidateTurnEnded.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!_payloadReader.TryGetInt32(request.Payload, ”from_day”, out var
            // fromDay) || !_payloadReader.TryGetInt32(request.Payload, ”to_day”, out var toDay) || fromDay != request.DayInd...` dan `toDay != request.DayIndex
            // + 1`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam ValidateTurnEnded.
            if (!_payloadReader.TryGetInt32(request.Payload, "from_day", out var fromDay) ||
                // Menggunakan kebalikan kondisi `_payloadReader.TryGetInt32(request.Payload, ”to_day”, out var toDay)` sebagai bagian ekspresi yang sedang disusun
                // dalam ValidateTurnEnded.
                !_payloadReader.TryGetInt32(request.Payload, "to_day", out var toDay) ||
                // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `fromDay` dan `request.DayIndex` dalam ValidateTurnEnded.
                fromDay != request.DayIndex ||
                // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `toDay` dan `request.DayIndex + 1` dalam ValidateTurnEnded.
                toDay != request.DayIndex + 1)
            // Membuka scope cabang if untuk kondisi `!_payloadReader.TryGetInt32(request.Payload, ”from_day”, out var fromDay) ||
            // !_payloadReader.TryGetInt32(request.Payload, ”to_day”, out var toDay) || fromDay != request.DayInd...`; pernyataan/deklarasi berikut berada di
            // dalam batas blok ini dalam ValidateTurnEnded.
            {
                // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`,
                // `”Transisi hari pada AkhirGiliran tidak sesuai”` kepada pemanggil dalam ValidateTurnEnded; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return EventDomainValidationResult.Fail(
                    // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke
                    // `EventDomainValidationResult.Fail`.
                    StatusCodes.Status422UnprocessableEntity,
                    // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "DOMAIN_RULE_VIOLATION",
                    // Meneruskan nilai literal `”Transisi hari pada AkhirGiliran tidak sesuai”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "Transisi hari pada AkhirGiliran tidak sesuai");
            // Menutup scope cabang if untuk kondisi `!_payloadReader.TryGetInt32(request.Payload, ”from_day”, out var fromDay) ||
            // !_payloadReader.TryGetInt32(request.Payload, ”to_day”, out var toDay) || fromDay != request.DayInd...`; bagian berikut berada di luar batas blok
            // tersebut dalam ValidateTurnEnded.
            }
        // Menutup scope cabang if untuk kondisi `hasFromDay || hasToDay`; bagian berikut berada di luar batas blok tersebut dalam ValidateTurnEnded.
        }

        // Memeriksa mencari properti JSON `”completed_players”`, `_` pada `request.Payload` tanpa menganggap propertinya selalu tersedia; blok if hanya
        // dijalankan ketika kondisi ini bernilai benar dalam ValidateTurnEnded.
        if (request.Payload.TryGetProperty("completed_players", out _))
        // Membuka scope cabang if untuk kondisi `request.Payload.TryGetProperty(”completed_players”, out _)`; pernyataan/deklarasi berikut berada di dalam
        // batas blok ini dalam ValidateTurnEnded.
        {
            // Memeriksa gabungan syarat OR: setidaknya satu kondisi wajib benar antara `!_payloadReader.TryGetInt32(request.Payload, ”completed_players”, out
            // var completedPlayers)` dan `completedPlayers != participantCount`; sisi kanan diperiksa hanya jika sisi kiri salah; blok if hanya dijalankan
            // ketika kondisi ini bernilai benar dalam ValidateTurnEnded.
            if (!_payloadReader.TryGetInt32(request.Payload, "completed_players", out var completedPlayers) ||
                // Melanjutkan ekspresi dengan perbandingan ketidaksamaan antara `completedPlayers` dan `participantCount` dalam ValidateTurnEnded.
                completedPlayers != participantCount)
            // Membuka scope cabang if untuk kondisi `!_payloadReader.TryGetInt32(request.Payload, ”completed_players”, out var completedPlayers) ||
            // completedPlayers != participantCount`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateTurnEnded.
            {
                // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Jumlah
                // completed_players tidak sesuai peserta sesi”` kepada pemanggil dalam ValidateTurnEnded; eksekusi jalur ini selesai setelah nilai hasil
                // ditentukan.
                return EventDomainValidationResult.Fail(
                    // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke
                    // `EventDomainValidationResult.Fail`.
                    StatusCodes.Status422UnprocessableEntity,
                    // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "DOMAIN_RULE_VIOLATION",
                    // Meneruskan nilai literal `”Jumlah completed_players tidak sesuai peserta sesi”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "Jumlah completed_players tidak sesuai peserta sesi");
            // Menutup scope cabang if untuk kondisi `!_payloadReader.TryGetInt32(request.Payload, ”completed_players”, out var completedPlayers) ||
            // completedPlayers != participantCount`; bagian berikut berada di luar batas blok tersebut dalam ValidateTurnEnded.
            }
        // Menutup scope cabang if untuk kondisi `request.Payload.TryGetProperty(”completed_players”, out _)`; bagian berikut berada di luar batas blok
        // tersebut dalam ValidateTurnEnded.
        }

        // Memeriksa kebalikan kondisi `string.Equals(config.Mode, ”MAHIR”, StringComparison.OrdinalIgnoreCase)`; blok if hanya dijalankan ketika kondisi
        // ini bernilai benar dalam ValidateTurnEnded.
        if (!string.Equals(config.Mode, "MAHIR", StringComparison.OrdinalIgnoreCase))
        // Membuka scope cabang if untuk kondisi `!string.Equals(config.Mode, ”MAHIR”, StringComparison.OrdinalIgnoreCase)`; pernyataan/deklarasi berikut
        // berada di dalam batas blok ini dalam ValidateTurnEnded.
        {
            // Mengembalikan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) kepada pemanggil dalam ValidateTurnEnded;
            // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
            return EventDomainValidationResult.Valid;
        // Menutup scope cabang if untuk kondisi `!string.Equals(config.Mode, ”MAHIR”, StringComparison.OrdinalIgnoreCase)`; bagian berikut berada di luar
        // batas blok tersebut dalam ValidateTurnEnded.
        }

        // Menyiapkan variabel lokal `orderCounts` untuk nilai urutan/pesanan counts dengan membangun kamus dari `turnEvents .Where(e =>
        // GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.JualMasakan)) .GroupBy(e => e.UserId!.Value)` dengan
        // pemilihan kunci/nilai `g => g.Key`, `g => g.Count()`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari ekspresi nilai
        // awal.
        var orderCounts = turnEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => GameActionCatalog.Is(e.ActionType,
            // _payloadReader.ReadPayload(e.Payload), GameActionCatalog.JualMasakan)) dalam ValidateTurnEnded; token pada baris ini menyambungkan bagian kode
            // sebelum dan sesudahnya.
            .Where(e => GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.JualMasakan))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(e => e.UserId!.Value) dalam ValidateTurnEnded; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(e => e.UserId!.Value)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary(g => g.Key, g => g.Count()); dalam ValidateTurnEnded; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(g => g.Key, g => g.Count());

        // Menyiapkan variabel lokal `riskCounts` untuk nilai risiko counts dengan membangun kamus dari `turnEvents .Where(e =>
        // GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.RisikoKehidupan)) .GroupBy(e => e.UserId!.Value)`
        // dengan pemilihan kunci/nilai `g => g.Key`, `g => g.Count()`; kunci harus unik agar konversi berhasil. Tipe variabel disimpulkan dari ekspresi
        // nilai awal.
        var riskCounts = turnEvents
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .Where(e => GameActionCatalog.Is(e.ActionType,
            // _payloadReader.ReadPayload(e.Payload), GameActionCatalog.RisikoKehidupan)) dalam ValidateTurnEnded; token pada baris ini menyambungkan bagian
            // kode sebelum dan sesudahnya.
            .Where(e => GameActionCatalog.Is(e.ActionType, _payloadReader.ReadPayload(e.Payload), GameActionCatalog.RisikoKehidupan))
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .GroupBy(e => e.UserId!.Value) dalam ValidateTurnEnded; token pada baris ini
            // menyambungkan bagian kode sebelum dan sesudahnya.
            .GroupBy(e => e.UserId!.Value)
            // Melengkapi struktur ekspresi SimpleMemberAccessExpression melalui .ToDictionary(g => g.Key, g => g.Count()); dalam ValidateTurnEnded; token pada
            // baris ini menyambungkan bagian kode sebelum dan sesudahnya.
            .ToDictionary(g => g.Key, g => g.Count());

        // Mengulangi setiap elemen `orderCounts.Keys.Union(riskCounts.Keys)`; elemen saat ini disimpan sebagai `playerId` bertipe `var` untuk diproses oleh
        // badan loop dalam ValidateTurnEnded.
        foreach (var playerId in orderCounts.Keys.Union(riskCounts.Keys))
        // Membuka scope loop setiap playerId dari `orderCounts.Keys.Union(riskCounts.Keys)`; pernyataan/deklarasi berikut berada di dalam batas blok ini
        // dalam ValidateTurnEnded.
        {
            // Menjalankan mencari kunci `playerId` pada `orderCounts`; hasil boolean menandakan kunci ditemukan dan argumen out menerima nilainya dalam
            // ValidateTurnEnded.
            orderCounts.TryGetValue(playerId, out var orders);
            // Menjalankan mencari kunci `playerId` pada `riskCounts`; hasil boolean menandakan kunci ditemukan dan argumen out menerima nilainya dalam
            // ValidateTurnEnded.
            riskCounts.TryGetValue(playerId, out var risks);
            // Memeriksa perbandingan ketidaksamaan antara `orders` dan `risks`; blok if hanya dijalankan ketika kondisi ini bernilai benar dalam
            // ValidateTurnEnded.
            if (orders != risks)
            // Membuka scope cabang if untuk kondisi `orders != risks`; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam ValidateTurnEnded.
            {
                // Mengembalikan memanggil `EventDomainValidationResult.Fail` dengan `StatusCodes.Status422UnprocessableEntity`, `”DOMAIN_RULE_VIOLATION”`, `”Setiap
                // klaim pesanan harus diikuti pengambilan risiko pada mode MAHIR”` kepada pemanggil dalam ValidateTurnEnded; eksekusi jalur ini selesai setelah
                // nilai hasil ditentukan.
                return EventDomainValidationResult.Fail(
                    // Meneruskan `StatusCodes.Status422UnprocessableEntity` (nilai status 422 unprocessable entity) sebagai argumen ke
                    // `EventDomainValidationResult.Fail`.
                    StatusCodes.Status422UnprocessableEntity,
                    // Meneruskan nilai literal `”DOMAIN_RULE_VIOLATION”` sebagai argumen ke `EventDomainValidationResult.Fail`.
                    "DOMAIN_RULE_VIOLATION",
                    // Meneruskan nilai literal `”Setiap klaim pesanan harus diikuti pengambilan risiko pada mode MAHIR”` sebagai argumen ke
                    // `EventDomainValidationResult.Fail`.
                    "Setiap klaim pesanan harus diikuti pengambilan risiko pada mode MAHIR");
            // Menutup scope cabang if untuk kondisi `orders != risks`; bagian berikut berada di luar batas blok tersebut dalam ValidateTurnEnded.
            }
        // Menutup scope loop setiap playerId dari `orderCounts.Keys.Union(riskCounts.Keys)`; bagian berikut berada di luar batas blok tersebut dalam
        // ValidateTurnEnded.
        }

        // Mengembalikan `EventDomainValidationResult.Valid` (penanda apakah validasi telah memenuhi syarat) kepada pemanggil dalam ValidateTurnEnded;
        // eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return EventDomainValidationResult.Valid;
    // Menutup scope metode ValidateTurnEnded; bagian berikut berada di luar batas blok tersebut dalam ValidateTurnEnded.
    }

    // Mendefinisikan metode `IsRegularActionWeekday` dengan hasil bertipe `bool`; operasi ini menangani berstatus regular aksi weekday. Masukan:
    // Parameter `weekday` bertipe `string` membawa nilai weekday. Nilai hasil langsung berasal dari gabungan syarat OR: setidaknya satu kondisi wajib
    // benar antara `weekday.Equals(”MON”, StringComparison.OrdinalIgnoreCase) || weekday.Equals(”TUE”, StringComparison.OrdinalIgnoreCase) ||
    // weekday.Equals(”WED”, StringComparison.OrdinalIgnoreC...` dan `weekday.Equals(”THU”, StringComparison.OrdinalIgnoreCase)`; sisi kanan diperiksa
    // hanya jika sisi kiri salah.
    private static bool IsRegularActionWeekday(string weekday)
        // Melengkapi struktur ekspresi ArrowExpressionClause melalui => weekday.Equals(”MON”, StringComparison.OrdinalIgnoreCase) || dalam
        // IsRegularActionWeekday; token pada baris ini menyambungkan bagian kode sebelum dan sesudahnya.
        => weekday.Equals("MON", StringComparison.OrdinalIgnoreCase) ||
           // Melanjutkan pengolahan dengan membandingkan kesamaan `weekday` dengan `”TUE”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
           // mengikuti overload dan comparer yang diberikan dalam IsRegularActionWeekday.
           weekday.Equals("TUE", StringComparison.OrdinalIgnoreCase) ||
           // Melanjutkan pengolahan dengan membandingkan kesamaan `weekday` dengan `”WED”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
           // mengikuti overload dan comparer yang diberikan dalam IsRegularActionWeekday.
           weekday.Equals("WED", StringComparison.OrdinalIgnoreCase) ||
           // Melanjutkan pengolahan dengan membandingkan kesamaan `weekday` dengan `”THU”`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan
           // mengikuti overload dan comparer yang diberikan dalam IsRegularActionWeekday.
           weekday.Equals("THU", StringComparison.OrdinalIgnoreCase);

    // Mendefinisikan metode `IsScheduledGoldDecision` dengan hasil bertipe `bool`; operasi ini menangani berstatus scheduled emas decision. Masukan:
    // Parameter `actionType` bertipe `string` membawa nilai aksi jenis; Parameter `payload` bertipe `System.Text.Json.JsonElement` membawa muatan
    // detail event dalam format JSON.
    private static bool IsScheduledGoldDecision(string actionType, System.Text.Json.JsonElement payload)
    // Membuka scope metode IsScheduledGoldDecision; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam IsScheduledGoldDecision.
    {
        // Menyiapkan variabel lokal `isDecision` untuk nilai berstatus decision dengan gabungan syarat OR: setidaknya satu kondisi wajib benar antara
        // `GameActionCatalog.Is(actionType, payload, GameActionCatalog.InvestasiEmas) || GameActionCatalog.Is(actionType, payload,
        // GameActionCatalog.JualEmas)` dan `GameActionCatalog.Is(actionType, payload, GameActionCatalog.GoldSkipped)`; sisi kanan diperiksa hanya jika sisi
        // kiri salah. Tipe variabel disimpulkan dari ekspresi nilai awal.
        var isDecision = GameActionCatalog.Is(actionType, payload, GameActionCatalog.InvestasiEmas) ||
                         // Melanjutkan pengolahan dengan memanggil `GameActionCatalog.Is` dengan `actionType`, `payload`, `GameActionCatalog.JualEmas` dalam
                         // IsScheduledGoldDecision.
                         GameActionCatalog.Is(actionType, payload, GameActionCatalog.JualEmas) ||
                         // Melanjutkan pengolahan dengan memanggil `GameActionCatalog.Is` dengan `actionType`, `payload`, `GameActionCatalog.GoldSkipped` dalam
                         // IsScheduledGoldDecision.
                         GameActionCatalog.Is(actionType, payload, GameActionCatalog.GoldSkipped);
        // Mengembalikan gabungan syarat AND: kedua kondisi wajib benar antara `isDecision` dan `!(payload.TryGetProperty(”risk_event_id”, out var
        // riskEventId) && riskEventId.ValueKind == System.Text.Json.JsonValueKind.String && Guid.TryParse(riskEventId.GetString(), out ...`; sisi kanan
        // diperiksa hanya jika sisi kiri benar kepada pemanggil dalam IsScheduledGoldDecision; eksekusi jalur ini selesai setelah nilai hasil ditentukan.
        return isDecision && !(payload.TryGetProperty("risk_event_id", out var riskEventId) &&
                               // Melanjutkan ekspresi dengan perbandingan kesamaan antara `riskEventId.ValueKind` dan `System.Text.Json.JsonValueKind.String` dalam
                               // IsScheduledGoldDecision.
                               riskEventId.ValueKind == System.Text.Json.JsonValueKind.String &&
                               // Melanjutkan pengolahan dengan mencoba mengonversi `riskEventId.GetString()`, `_` melalui `Guid.TryParse`; keberhasilan dilaporkan sebagai boolean
                               // dan hasil ditempatkan pada argumen out dalam IsScheduledGoldDecision.
                               Guid.TryParse(riskEventId.GetString(), out _));
    // Menutup scope metode IsScheduledGoldDecision; bagian berikut berada di luar batas blok tersebut dalam IsScheduledGoldDecision.
    }

// Menutup scope tipe EventTurnProgressValidator; bagian berikut berada di luar batas blok tersebut.
}
