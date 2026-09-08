// Fungsi file: Menjalankan aturan dan perhitungan domain permainan melalui IEventValidators.
// Mengimpor namespace `Cashflowpoly.Api.Data` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Data;
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Domain` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Domain;

// Mendefinisikan interface sebagai kontrak operasi `IEventRequestShapeValidator`.
public interface IEventRequestShapeValidator
// Membuka scope tipe IEventRequestShapeValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `Validate` dengan hasil bertipe `EventDomainValidationResult`; operasi ini menangani validate. Masukan: Parameter `request`
    // bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `scopedPlayerId` bertipe
    // `Guid?` membawa nilai scoped pemain identitas; nilai null diizinkan ketika data opsional belum tersedia.
    EventDomainValidationResult Validate(EventRequest request, Guid? scopedPlayerId);
// Menutup scope tipe IEventRequestShapeValidator; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `IEventSimpleActionValidator`.
public interface IEventSimpleActionValidator
// Membuka scope tipe IEventSimpleActionValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `TryValidate` dengan hasil bertipe `bool`; operasi ini menangani try validate. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `config` bertipe `RulesetConfig`
    // membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `result` bertipe `EventDomainValidationResult`
    // membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryValidate(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `result` bertipe `EventDomainValidationResult` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out EventDomainValidationResult result);
// Menutup scope tipe IEventSimpleActionValidator; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `IEventTurnProgressValidator`.
public interface IEventTurnProgressValidator
// Membuka scope tipe IEventTurnProgressValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `RequiresHistory` dengan hasil bertipe `bool`; operasi ini menangani requires history. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `config` bertipe `RulesetConfig`
    // membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
    bool RequiresHistory(EventRequest request, RulesetConfig config);

    // Mendefinisikan metode `TryValidate` dengan hasil bertipe `bool`; operasi ini menangani try validate. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `config` bertipe `RulesetConfig`
    // membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe `IEnumerable<EventDb>` membawa
    // nilai history; Parameter `participantCount` bertipe `int` membawa nilai participant jumlah; Parameter `result` bertipe
    // `EventDomainValidationResult` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out mengembalikan nilai melalui parameter
    // dan harus diisi oleh metode.
    bool TryValidate(
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
        out EventDomainValidationResult result);
// Menutup scope tipe IEventTurnProgressValidator; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `IEventNeedPurchaseValidator`.
public interface IEventNeedPurchaseValidator
// Membuka scope tipe IEventNeedPurchaseValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `TryValidate` dengan hasil bertipe `bool`; operasi ini menangani try validate. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `config` bertipe `RulesetConfig`
    // membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe `IEnumerable<EventDb>` membawa
    // nilai history; Parameter `result` bertipe `EventNeedPurchaseValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryValidate(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history,
        // Parameter `result` bertipe `EventNeedPurchaseValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out EventNeedPurchaseValidation result);
// Menutup scope tipe IEventNeedPurchaseValidator; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `IEventIngredientOrderValidator`.
public interface IEventIngredientOrderValidator
// Membuka scope tipe IEventIngredientOrderValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `TryValidate` dengan hasil bertipe `bool`; operasi ini menangani try validate. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `config` bertipe `RulesetConfig`
    // membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe `IEnumerable<EventDb>` membawa
    // nilai history; Parameter `result` bertipe `EventIngredientOrderValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap
    // berikutnya; out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryValidate(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history,
        // Parameter `result` bertipe `EventIngredientOrderValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out EventIngredientOrderValidation result);
// Menutup scope tipe IEventIngredientOrderValidator; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `IEventSavingGoalValidator`.
public interface IEventSavingGoalValidator
// Membuka scope tipe IEventSavingGoalValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `TryValidate` dengan hasil bertipe `bool`; operasi ini menangani try validate. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `config` bertipe `RulesetConfig`
    // membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe `IEnumerable<EventDb>` membawa
    // nilai history; Parameter `result` bertipe `EventSavingGoalValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out
    // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryValidate(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history,
        // Parameter `result` bertipe `EventSavingGoalValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out mengembalikan
        // nilai melalui parameter dan harus diisi oleh metode.
        out EventSavingGoalValidation result);
// Menutup scope tipe IEventSavingGoalValidator; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `IEventEconomyActionValidator`.
public interface IEventEconomyActionValidator
// Membuka scope tipe IEventEconomyActionValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `TryValidate` dengan hasil bertipe `bool`; operasi ini menangani try validate. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `config` bertipe `RulesetConfig`
    // membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan; Parameter `history` bertipe `IEnumerable<EventDb>` membawa
    // nilai history; Parameter `result` bertipe `EventEconomyActionValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya;
    // out mengembalikan nilai melalui parameter dan harus diisi oleh metode.
    bool TryValidate(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `config` bertipe `RulesetConfig` membawa konfigurasi aturan permainan yang dipakai untuk validasi dan perhitungan.
        RulesetConfig config,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history,
        // Parameter `result` bertipe `EventEconomyActionValidation` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out EventEconomyActionValidation result);
// Menutup scope tipe IEventEconomyActionValidator; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan interface sebagai kontrak operasi `IEventAssignmentValidator`.
public interface IEventAssignmentValidator
// Membuka scope tipe IEventAssignmentValidator; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `TryValidate` dengan hasil bertipe `bool`; operasi ini menangani try validate. Masukan: Parameter `request` bertipe
    // `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan; Parameter `history` bertipe
    // `IEnumerable<EventDb>` membawa nilai history; Parameter `participantCount` bertipe `int` membawa nilai participant jumlah; Parameter `result`
    // bertipe `EventDomainValidationResult` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out mengembalikan nilai melalui
    // parameter dan harus diisi oleh metode.
    bool TryValidate(
        // Parameter `request` bertipe `EventRequest` membawa data masukan permintaan yang akan divalidasi atau diteruskan ke layanan.
        EventRequest request,
        // Parameter `history` bertipe `IEnumerable<EventDb>` membawa nilai history.
        IEnumerable<EventDb> history,
        // Parameter `participantCount` bertipe `int` membawa nilai participant jumlah.
        int participantCount,
        // Parameter `result` bertipe `EventDomainValidationResult` membawa nilai hasil pemrosesan yang akan dipakai pada tahap berikutnya; out
        // mengembalikan nilai melalui parameter dan harus diisi oleh metode.
        out EventDomainValidationResult result);
// Menutup scope tipe IEventAssignmentValidator; bagian berikut berada di luar batas blok tersebut.
}
