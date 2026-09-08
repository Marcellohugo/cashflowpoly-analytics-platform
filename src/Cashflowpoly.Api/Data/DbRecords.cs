// Fungsi file: Mengelola pemetaan dan akses PostgreSQL untuk DbRecords.
// Mengimpor namespace `Cashflowpoly.Api.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Api.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Data` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Data;

/// <summary>
/// Representasi baris tabel rulesets — konfigurasi aturan permainan.
/// </summary>
// Mendefinisikan tipe class `RulesetDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetDb
// Membuka scope tipe RulesetDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `RulesetId` bertipe `Guid` untuk identitas kumpulan aturan permainan; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public Guid RulesetId { get; set; }
    // Mendefinisikan properti `Name` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai
    // awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Name { get; set; } = string.Empty;
    // Mendefinisikan properti `Description` bertipe `string?` untuk nilai description; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; tanda ? mengizinkan nilai null.
    public string? Description { get; set; }
    // Mendefinisikan properti `InstructorUserId` bertipe `Guid?` untuk identitas instruktur pemilik sesi atau aturan; get menyediakan pembacaan nilai,
    // set mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public Guid? InstructorUserId { get; set; }
    // Mendefinisikan properti `IsArchived` bertipe `bool` untuk nilai berstatus archived; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public bool IsArchived { get; set; }
    // Mendefinisikan properti `ArchivedAt` bertipe `DateTimeOffset?` untuk nilai archived at; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public DateTimeOffset? ArchivedAt { get; set; }
    // Mendefinisikan properti `CreatedAt` bertipe `DateTimeOffset` untuk nilai created at; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public DateTimeOffset CreatedAt { get; set; }
    // Mendefinisikan properti `CreatedByUserId` bertipe `Guid?` untuk nilai created berdasarkan pengguna identitas; get menyediakan pembacaan nilai,
    // set mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public Guid? CreatedByUserId { get; set; }
// Menutup scope tipe RulesetDb; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Representasi baris tabel ruleset_versions — versi snapshot konfigurasi aturan.
/// </summary>
// Mendefinisikan tipe class `RulesetVersionDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetVersionDb
// Membuka scope tipe RulesetVersionDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `RulesetVersionId` bertipe `Guid` untuk identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang
    // tepat; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public Guid RulesetVersionId { get; set; }
    // Mendefinisikan properti `RulesetId` bertipe `Guid` untuk identitas kumpulan aturan permainan; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public Guid RulesetId { get; set; }
    // Mendefinisikan properti `Version` bertipe `int` untuk nomor versi yang dipakai untuk konsistensi data atau konfigurasi; get menyediakan pembacaan
    // nilai, set mengizinkan penggantian nilai.
    public int Version { get; set; }
    // Mendefinisikan properti `Status` bertipe `string` untuk nilai status; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai
    // awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Status { get; set; } = string.Empty;
    // Mendefinisikan properti `Mode` bertipe `string?` untuk mode permainan yang menentukan kelompok aturan yang digunakan; get menyediakan pembacaan
    // nilai, set mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public string? Mode { get; set; }
    // Mendefinisikan properti `Definition` bertipe `RulesetDefinitionDto?` untuk definisi terstruktur komponen serta parameter aturan permainan; get
    // menyediakan pembacaan nilai, set mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public RulesetDefinitionDto? Definition { get; set; }
    // Mendefinisikan properti `ConfigHash` bertipe `string` untuk nilai konfigurasi hash; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string ConfigHash { get; set; } = string.Empty;
    // Mendefinisikan properti `CreatedAt` bertipe `DateTimeOffset` untuk nilai created at; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public DateTimeOffset CreatedAt { get; set; }
    // Mendefinisikan properti `CreatedByUserId` bertipe `Guid?` untuk nilai created berdasarkan pengguna identitas; get menyediakan pembacaan nilai,
    // set mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public Guid? CreatedByUserId { get; set; }
// Menutup scope tipe RulesetVersionDb; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Proyeksi gabungan ruleset + versi aktif untuk komponen default seed.
/// </summary>
// Mendefinisikan tipe class `DefaultRulesetComponentDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed class DefaultRulesetComponentDb
// Membuka scope tipe DefaultRulesetComponentDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `RulesetId` bertipe `Guid` untuk identitas kumpulan aturan permainan; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public Guid RulesetId { get; set; }
    // Mendefinisikan properti `Name` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai
    // awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Name { get; set; } = string.Empty;
    // Mendefinisikan properti `Description` bertipe `string?` untuk nilai description; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; tanda ? mengizinkan nilai null.
    public string? Description { get; set; }
    // Mendefinisikan properti `RulesetVersionId` bertipe `Guid` untuk identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang
    // tepat; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public Guid RulesetVersionId { get; set; }
    // Mendefinisikan properti `Version` bertipe `int` untuk nomor versi yang dipakai untuk konsistensi data atau konfigurasi; get menyediakan pembacaan
    // nilai, set mengizinkan penggantian nilai.
    public int Version { get; set; }
    // Mendefinisikan properti `Mode` bertipe `string?` untuk mode permainan yang menentukan kelompok aturan yang digunakan; get menyediakan pembacaan
    // nilai, set mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public string? Mode { get; set; }
    // Mendefinisikan properti `Definition` bertipe `RulesetDefinitionDto?` untuk definisi terstruktur komponen serta parameter aturan permainan; get
    // menyediakan pembacaan nilai, set mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public RulesetDefinitionDto? Definition { get; set; }
// Menutup scope tipe DefaultRulesetComponentDb; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Representasi baris tabel sessions — sesi permainan Cashflowpoly.
/// </summary>
// Mendefinisikan tipe class `SessionDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionDb
// Membuka scope tipe SessionDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `SessionId` bertipe `Guid` untuk identitas unik sesi permainan yang menjadi batas data operasi ini; get menyediakan
    // pembacaan nilai, set mengizinkan penggantian nilai.
    public Guid SessionId { get; set; }
    // Mendefinisikan properti `SessionName` bertipe `string` untuk nilai sesi nama; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai;
    // nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string SessionName { get; set; } = string.Empty;
    // Mendefinisikan properti `Mode` bertipe `string` untuk mode permainan yang menentukan kelompok aturan yang digunakan; get menyediakan pembacaan
    // nilai, set mengizinkan penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Mode { get; set; } = string.Empty;
    // Mendefinisikan properti `Status` bertipe `string` untuk nilai status; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai
    // awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Status { get; set; } = string.Empty;
    // Mendefinisikan properti `StartedAt` bertipe `DateTimeOffset?` untuk nilai started at; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public DateTimeOffset? StartedAt { get; set; }
    // Mendefinisikan properti `EndedAt` bertipe `DateTimeOffset?` untuk nilai ended at; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; tanda ? mengizinkan nilai null.
    public DateTimeOffset? EndedAt { get; set; }
    // Mendefinisikan properti `InstructorUserId` bertipe `Guid?` untuk identitas instruktur pemilik sesi atau aturan; get menyediakan pembacaan nilai,
    // set mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public Guid? InstructorUserId { get; set; }
    // Mendefinisikan properti `RulesetVersionId` bertipe `Guid` untuk identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang
    // tepat; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public Guid RulesetVersionId { get; set; }
    // Mendefinisikan properti `IsArchived` bertipe `bool` untuk nilai berstatus archived; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public bool IsArchived { get; set; }
    // Mendefinisikan properti `ArchivedAt` bertipe `DateTimeOffset?` untuk nilai archived at; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public DateTimeOffset? ArchivedAt { get; set; }
    // Mendefinisikan properti `CreatedAt` bertipe `DateTimeOffset` untuk nilai created at; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public DateTimeOffset CreatedAt { get; set; }
// Menutup scope tipe SessionDb; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Posisi hari aktif sesi yang menjadi acuan validasi event gameplay.
/// </summary>
// Mendefinisikan tipe class `SessionProgressDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionProgressDb
// Membuka scope tipe SessionProgressDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `Day` bertipe `int` untuk nomor hari permainan yang menjadi konteks aktivitas; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai.
    public int Day { get; set; }
    // Mendefinisikan properti `FinishDay` bertipe `int` untuk nilai finish hari; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public int FinishDay { get; set; }
// Menutup scope tipe SessionProgressDb; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Representasi player berbasis tabel app_users — akun role PLAYER.
/// </summary>
// Mendefinisikan tipe class `PlayerDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerDb
// Membuka scope tipe PlayerDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `UserId` bertipe `Guid` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai.
    public Guid UserId { get; set; }
    // Mendefinisikan properti `Username` bertipe `string` untuk nama akun yang dipakai saat autentikasi; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Username { get; set; } = string.Empty;
    // Mendefinisikan properti `DisplayName` bertipe `string` untuk nilai display nama; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string DisplayName { get; set; } = string.Empty;
    // Mendefinisikan properti `InstructorUserId` bertipe `Guid?` untuk identitas instruktur pemilik sesi atau aturan; get menyediakan pembacaan nilai,
    // set mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public Guid? InstructorUserId { get; set; }
    // Mendefinisikan properti `Role` bertipe `string` untuk peran pengguna yang menentukan hak akses; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Role { get; set; } = string.Empty;
    // Mendefinisikan properti `IsActive` bertipe `bool` untuk nilai berstatus aktif; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public bool IsActive { get; set; }
    // Mendefinisikan properti `CreatedAt` bertipe `DateTimeOffset` untuk nilai created at; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public DateTimeOffset CreatedAt { get; set; }
// Menutup scope tipe PlayerDb; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Nilai final pemain yang dibekukan saat sesi selesai; menjadi sumber resmi peringkat dan Poin Kebahagiaan.
/// </summary>
// Mendefinisikan tipe class `SessionFinalScoreDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionFinalScoreDb
// Membuka scope tipe SessionFinalScoreDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `UserId` bertipe `Guid` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai.
    public Guid UserId { get; set; }
    // Mendefinisikan properti `PlayerOrder` bertipe `int` untuk nomor urut pemain untuk menentukan urutan tindakan; get menyediakan pembacaan nilai,
    // set mengizinkan penggantian nilai.
    public int PlayerOrder { get; set; }
    // Mendefinisikan properti `Rank` bertipe `int` untuk nilai rank; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public int Rank { get; set; }
    // Mendefinisikan properti `TotalPoints` bertipe `double` untuk nilai total poin; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public double TotalPoints { get; set; }
    // Mendefinisikan properti `NeedPoints` bertipe `double` untuk nilai kebutuhan poin; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public double NeedPoints { get; set; }
    // Mendefinisikan properti `NeedSetBonusPoints` bertipe `double` untuk nilai kebutuhan set bonus poin; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai.
    public double NeedSetBonusPoints { get; set; }
    // Mendefinisikan properti `DonationPoints` bertipe `double` untuk nilai donasi poin; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public double DonationPoints { get; set; }
    // Mendefinisikan properti `GoldPoints` bertipe `double` untuk nilai emas poin; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public double GoldPoints { get; set; }
    // Mendefinisikan properti `PensionPoints` bertipe `double` untuk nilai pension poin; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public double PensionPoints { get; set; }
    // Mendefinisikan properti `SavingGoalPoints` bertipe `double` untuk nilai tabungan target poin; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public double SavingGoalPoints { get; set; }
    // Mendefinisikan properti `MissionPenaltyPoints` bertipe `double` untuk nilai misi penalti poin; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public double MissionPenaltyPoints { get; set; }
    // Mendefinisikan properti `LoanPenaltyPoints` bertipe `double` untuk nilai pinjaman penalti poin; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public double LoanPenaltyPoints { get; set; }
    // Mendefinisikan properti `HasUnpaidLoan` bertipe `bool` untuk nilai memiliki unpaid pinjaman; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public bool HasUnpaidLoan { get; set; }
// Menutup scope tipe SessionFinalScoreDb; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `SessionPlayerDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionPlayerDb
// Membuka scope tipe SessionPlayerDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `SessionPlayerId` bertipe `Guid` untuk identitas keikutsertaan pemain pada sesi tertentu; get menyediakan pembacaan
    // nilai, set mengizinkan penggantian nilai.
    public Guid SessionPlayerId { get; set; }
    // Mendefinisikan properti `UserId` bertipe `Guid` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai.
    public Guid UserId { get; set; }
    // Mendefinisikan properti `DisplayName` bertipe `string` untuk nilai display nama; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string DisplayName { get; set; } = string.Empty;
    // Mendefinisikan properti `PlayerOrder` bertipe `int` untuk nomor urut pemain untuk menentukan urutan tindakan; get menyediakan pembacaan nilai,
    // set mengizinkan penggantian nilai.
    public int PlayerOrder { get; set; }
// Menutup scope tipe SessionPlayerDb; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `SessionSetupDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionSetupDb
// Membuka scope tipe SessionSetupDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `SessionId` bertipe `Guid` untuk identitas unik sesi permainan yang menjadi batas data operasi ini; get menyediakan
    // pembacaan nilai, set mengizinkan penggantian nilai.
    public Guid SessionId { get; set; }
    // Mendefinisikan properti `Revision` bertipe `int` untuk nomor revisi data untuk membedakan versi penyimpanan; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai.
    public int Revision { get; set; }
    // Mendefinisikan properti `RulesetVersionId` bertipe `Guid` untuk identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang
    // tepat; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public Guid RulesetVersionId { get; set; }
    // Mendefinisikan properti `ClientRequestId` bertipe `string` untuk identitas permintaan dari klien untuk pelacakan atau penanganan permintaan
    // berulang; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe
    // terkait.
    public string ClientRequestId { get; set; } = string.Empty;
    // Mendefinisikan properti `SetupJson` bertipe `string` untuk nilai setup JSON; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai;
    // nilai awalnya nilai literal `”{}”`.
    public string SetupJson { get; set; } = "{}";
    // Mendefinisikan properti `SavedAt` bertipe `DateTimeOffset` untuk nilai saved at; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public DateTimeOffset SavedAt { get; set; }
    // Mendefinisikan properti `LockedAt` bertipe `DateTimeOffset?` untuk nilai locked at; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; tanda ? mengizinkan nilai null.
    public DateTimeOffset? LockedAt { get; set; }
    // Mendefinisikan properti `CreatedByUserId` bertipe `Guid` untuk nilai created berdasarkan pengguna identitas; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai.
    public Guid CreatedByUserId { get; set; }
// Menutup scope tipe SessionSetupDb; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Representasi baris tabel events — event gameplay dari klien.
/// </summary>
// Mendefinisikan tipe class `EventDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed class EventDb
// Membuka scope tipe EventDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `EventPk` bertipe `Guid` untuk nilai event pk; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public Guid EventPk { get; set; }
    // Mendefinisikan properti `EventId` bertipe `Guid` untuk identitas unik event untuk pencatatan dan pemeriksaan duplikasi; get menyediakan pembacaan
    // nilai, set mengizinkan penggantian nilai.
    public Guid EventId { get; set; }
    // Mendefinisikan properti `SessionId` bertipe `Guid` untuk identitas unik sesi permainan yang menjadi batas data operasi ini; get menyediakan
    // pembacaan nilai, set mengizinkan penggantian nilai.
    public Guid SessionId { get; set; }
    // Mendefinisikan properti `SessionPlayerId` bertipe `Guid?` untuk identitas keikutsertaan pemain pada sesi tertentu; get menyediakan pembacaan
    // nilai, set mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public Guid? SessionPlayerId { get; set; }
    // Mendefinisikan properti `UserId` bertipe `Guid?` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public Guid? UserId { get; set; }
    // Mendefinisikan properti `ActorType` bertipe `string` untuk nilai actor jenis; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai;
    // nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string ActorType { get; set; } = string.Empty;
    // Mendefinisikan properti `Timestamp` bertipe `DateTimeOffset` untuk waktu kejadian yang menjaga urutan kronologis data; get menyediakan pembacaan
    // nilai, set mengizinkan penggantian nilai.
    public DateTimeOffset Timestamp { get; set; }
    // Mendefinisikan properti `DayIndex` bertipe `int` untuk nilai hari index; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public int DayIndex { get; set; }
    // Mendefinisikan properti `Weekday` bertipe `string` untuk nilai weekday; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai
    // awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Weekday { get; set; } = string.Empty;
    // Mendefinisikan properti `TurnNumber` bertipe `int` untuk nilai giliran number; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public int TurnNumber { get; set; }
    // Mendefinisikan properti `ActionSlot` bertipe `int` untuk nilai aksi slot; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public int ActionSlot { get; set; }
    // Mendefinisikan properti `SequenceNumber` bertipe `long` untuk nomor urut event yang menentukan urutan pemrosesan riwayat permainan; get
    // menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public long SequenceNumber { get; set; }
    // Mendefinisikan properti `RulesetActionId` bertipe `Guid` untuk nilai aturan aksi identitas; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public Guid RulesetActionId { get; set; }
    // Mendefinisikan properti `ActionId` bertipe `string?` untuk kode aksi yang dipetakan terhadap katalog aturan; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public string? ActionId { get; set; }
    // Mendefinisikan properti `ActionType` bertipe `string` untuk nilai aksi jenis; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai;
    // nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string ActionType { get; set; } = string.Empty;
    // Mendefinisikan properti `RulesetVersionId` bertipe `Guid` untuk identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang
    // tepat; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public Guid RulesetVersionId { get; set; }
    // Mendefinisikan properti `PayloadVersion` bertipe `string` untuk nilai payload versi; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; nilai awalnya nilai literal `”1.0”`.
    public string PayloadVersion { get; set; } = "1.0";
    // Mendefinisikan properti `Payload` bertipe `string` untuk muatan detail event dalam format JSON; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Payload { get; set; } = string.Empty;
    // Mendefinisikan properti `ReceivedAt` bertipe `DateTimeOffset` untuk nilai received at; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public DateTimeOffset ReceivedAt { get; set; }
    // Mendefinisikan properti `ClientRequestId` bertipe `string?` untuk identitas permintaan dari klien untuk pelacakan atau penanganan permintaan
    // berulang; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public string? ClientRequestId { get; set; }
// Menutup scope tipe EventDb; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Representasi baris tabel event_cashflow_projections — proyeksi arus kas per event.
/// </summary>
// Mendefinisikan tipe class `CashflowProjectionDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed class CashflowProjectionDb
// Membuka scope tipe CashflowProjectionDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `ProjectionId` bertipe `Guid` untuk nilai projection identitas; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public Guid ProjectionId { get; set; }
    // Mendefinisikan properti `SessionId` bertipe `Guid` untuk identitas unik sesi permainan yang menjadi batas data operasi ini; get menyediakan
    // pembacaan nilai, set mengizinkan penggantian nilai.
    public Guid SessionId { get; set; }
    // Mendefinisikan properti `UserId` bertipe `Guid` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai.
    public Guid UserId { get; set; }
    // Mendefinisikan properti `EventPk` bertipe `Guid` untuk nilai event pk; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public Guid EventPk { get; set; }
    // Mendefinisikan properti `EventId` bertipe `Guid` untuk identitas unik event untuk pencatatan dan pemeriksaan duplikasi; get menyediakan pembacaan
    // nilai, set mengizinkan penggantian nilai.
    public Guid EventId { get; set; }
    // Mendefinisikan properti `ProjectionOrder` bertipe `int` untuk nilai projection urutan/pesanan; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; nilai awalnya nilai literal `1`.
    public int ProjectionOrder { get; set; } = 1;
    // Mendefinisikan properti `Timestamp` bertipe `DateTimeOffset` untuk waktu kejadian yang menjaga urutan kronologis data; get menyediakan pembacaan
    // nilai, set mengizinkan penggantian nilai.
    public DateTimeOffset Timestamp { get; set; }
    // Mendefinisikan properti `Direction` bertipe `string` untuk nilai direction; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai;
    // nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Direction { get; set; } = string.Empty;
    // Mendefinisikan properti `Amount` bertipe `int` untuk nominal uang atau nilai transaksi yang dipakai dalam operasi; get menyediakan pembacaan
    // nilai, set mengizinkan penggantian nilai.
    public int Amount { get; set; }
    // Mendefinisikan properti `Category` bertipe `string` untuk nilai category; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai;
    // nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Category { get; set; } = string.Empty;
    // Mendefinisikan properti `Counterparty` bertipe `string?` untuk nilai counterparty; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; tanda ? mengizinkan nilai null.
    public string? Counterparty { get; set; }
    // Mendefinisikan properti `Reference` bertipe `string?` untuk nilai reference; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai;
    // tanda ? mengizinkan nilai null.
    public string? Reference { get; set; }
    // Mendefinisikan properti `Note` bertipe `string?` untuk nilai note; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; tanda ?
    // mengizinkan nilai null.
    public string? Note { get; set; }
// Menutup scope tipe CashflowProjectionDb; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Representasi baris tabel metric_snapshots — snapshot metrik numerik.
/// </summary>
// Mendefinisikan tipe class `MetricSnapshotDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed class MetricSnapshotDb
// Membuka scope tipe MetricSnapshotDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `MetricSnapshotId` bertipe `Guid` untuk nilai metric snapshot keadaan identitas; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai.
    public Guid MetricSnapshotId { get; set; }
    // Mendefinisikan properti `SessionId` bertipe `Guid` untuk identitas unik sesi permainan yang menjadi batas data operasi ini; get menyediakan
    // pembacaan nilai, set mengizinkan penggantian nilai.
    public Guid SessionId { get; set; }
    // Mendefinisikan properti `UserId` bertipe `Guid?` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public Guid? UserId { get; set; }
    // Mendefinisikan properti `SessionPlayerId` bertipe `Guid?` untuk identitas keikutsertaan pemain pada sesi tertentu; get menyediakan pembacaan
    // nilai, set mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public Guid? SessionPlayerId { get; set; }
    // Mendefinisikan properti `ComputedAt` bertipe `DateTimeOffset` untuk nilai computed at; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public DateTimeOffset ComputedAt { get; set; }
    // Mendefinisikan properti `MetricName` bertipe `string` untuk nilai metric nama; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string MetricName { get; set; } = string.Empty;
    // Mendefinisikan properti `MetricValueNumeric` bertipe `double?` untuk nilai metric nilai numerik; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public double? MetricValueNumeric { get; set; }
    // Mendefinisikan properti `MetricValueJson` bertipe `string?` untuk nilai metric nilai JSON; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public string? MetricValueJson { get; set; }
    // Mendefinisikan properti `RulesetVersionId` bertipe `Guid` untuk identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang
    // tepat; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public Guid RulesetVersionId { get; set; }
    // Mendefinisikan properti `LastEventId` bertipe `Guid?` untuk nilai last event identitas; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public Guid? LastEventId { get; set; }
// Menutup scope tipe MetricSnapshotDb; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Proyeksi ringan metric snapshot yang hanya berisi kolom JSON (untuk agregasi lanjut).
/// </summary>
// Mendefinisikan tipe class `MetricSnapshotJsonDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed class MetricSnapshotJsonDb
// Membuka scope tipe MetricSnapshotJsonDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `MetricName` bertipe `string` untuk nilai metric nama; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string MetricName { get; set; } = string.Empty;
    // Mendefinisikan properti `MetricValueJson` bertipe `string?` untuk nilai metric nilai JSON; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public string? MetricValueJson { get; set; }
    // Mendefinisikan properti `ComputedAt` bertipe `DateTimeOffset` untuk nilai computed at; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public DateTimeOffset ComputedAt { get; set; }
// Menutup scope tipe MetricSnapshotJsonDb; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `MetricSnapshotValueDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed class MetricSnapshotValueDb
// Membuka scope tipe MetricSnapshotValueDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `MetricName` bertipe `string` untuk nilai metric nama; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string MetricName { get; set; } = string.Empty;
    // Mendefinisikan properti `MetricValueNumeric` bertipe `double?` untuk nilai metric nilai numerik; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public double? MetricValueNumeric { get; set; }
    // Mendefinisikan properti `ComputedAt` bertipe `DateTimeOffset` untuk nilai computed at; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public DateTimeOffset ComputedAt { get; set; }
// Menutup scope tipe MetricSnapshotValueDb; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// Representasi baris tabel security_audit_logs — catatan audit keamanan.
/// </summary>
// Mendefinisikan tipe class `SecurityAuditLogDb`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SecurityAuditLogDb
// Membuka scope tipe SecurityAuditLogDb; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `SecurityAuditLogId` bertipe `Guid` untuk nilai security audit log identitas; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai.
    public Guid SecurityAuditLogId { get; set; }
    // Mendefinisikan properti `OccurredAt` bertipe `DateTimeOffset` untuk nilai occurred at; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public DateTimeOffset OccurredAt { get; set; }
    // Mendefinisikan properti `TraceId` bertipe `string` untuk identitas penelusuran yang menghubungkan respons, log, dan permintaan yang sama; get
    // menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string TraceId { get; set; } = string.Empty;
    // Mendefinisikan properti `EventType` bertipe `string` untuk jenis aktivitas yang menentukan aturan validasi dan proyeksi event; get menyediakan
    // pembacaan nilai, set mengizinkan penggantian nilai; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string EventType { get; set; } = string.Empty;
    // Mendefinisikan properti `Outcome` bertipe `string` untuk nilai hasil; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai
    // awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Outcome { get; set; } = string.Empty;
    // Mendefinisikan properti `UserId` bertipe `Guid?` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public Guid? UserId { get; set; }
    // Mendefinisikan properti `Username` bertipe `string?` untuk nama akun yang dipakai saat autentikasi; get menyediakan pembacaan nilai, set
    // mengizinkan penggantian nilai; tanda ? mengizinkan nilai null.
    public string? Username { get; set; }
    // Mendefinisikan properti `Role` bertipe `string?` untuk peran pengguna yang menentukan hak akses; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public string? Role { get; set; }
    // Mendefinisikan properti `IpAddress` bertipe `string?` untuk nilai ip address; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai;
    // tanda ? mengizinkan nilai null.
    public string? IpAddress { get; set; }
    // Mendefinisikan properti `UserAgent` bertipe `string?` untuk nilai pengguna agent; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; tanda ? mengizinkan nilai null.
    public string? UserAgent { get; set; }
    // Mendefinisikan properti `Method` bertipe `string` untuk nilai method; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai
    // awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Method { get; set; } = string.Empty;
    // Mendefinisikan properti `Path` bertipe `string` untuk nilai path; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai
    // awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Path { get; set; } = string.Empty;
    // Mendefinisikan properti `StatusCode` bertipe `int` untuk kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan; get
    // menyediakan pembacaan nilai, set mengizinkan penggantian nilai.
    public int StatusCode { get; set; }
    // Mendefinisikan properti `DetailJson` bertipe `string?` untuk nilai detail JSON; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; tanda ? mengizinkan nilai null.
    public string? DetailJson { get; set; }
// Menutup scope tipe SecurityAuditLogDb; bagian berikut berada di luar batas blok tersebut.
}
