// Fungsi file: Mendefinisikan model tampilan dan state UI untuk AnalyticsViewModels.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Models` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// ViewModel halaman daftar sesi permainan yang memuat koleksi item sesi dan pesan error opsional.
/// </summary>
// Mendefinisikan tipe class `SessionListViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionListViewModel
// Membuka scope tipe SessionListViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Daftar item sesi permainan yang diperoleh dari API untuk ditampilkan pada halaman daftar.
    /// </summary>
    // Mendefinisikan properti `Items` bertipe `List<SessionListItem>` untuk nilai elemen; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<SessionListItem> Items { get; init; } = new();
    // Mendefinisikan properti `ErrorMessage` bertipe `string?` untuk nilai kesalahan pesan; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public string? ErrorMessage { get; init; }
// Menutup scope tipe SessionListViewModel; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// ViewModel halaman detail sesi yang memuat data analitik sesi, peta nama pemain, timeline event, dan status sesi.
/// </summary>
// Mendefinisikan tipe class `SessionDetailViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionDetailViewModel
// Membuka scope tipe SessionDetailViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `SessionId` bertipe `Guid` untuk identitas unik sesi permainan yang menjadi batas data operasi ini; get menyediakan
    // pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
    public Guid SessionId { get; init; }
    // Mendefinisikan properti `Analytics` bertipe `AnalyticsSessionResponse?` untuk nilai analytics; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public AnalyticsSessionResponse? Analytics { get; init; }
    /// <summary>
    /// Kamus pemetaan ID pemain ke nama tampilan untuk resolusi nama pada halaman detail sesi.
    /// </summary>
    // Mendefinisikan properti `PlayerDisplayNames` bertipe `Dictionary<Guid, string>` untuk nilai pemain display nama; get menyediakan pembacaan nilai,
    // init membatasi pengisian saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public Dictionary<Guid, string> PlayerDisplayNames { get; init; } = new();
    /// <summary>
    /// Daftar event timeline sesi yang menampilkan urutan aksi dalam permainan secara kronologis.
    /// </summary>
    // Mendefinisikan properti `Timeline` bertipe `List<SessionTimelineEventViewModel>` untuk nilai timeline; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<SessionTimelineEventViewModel> Timeline { get; init; } = new();
    // Mendefinisikan properti `TimelineErrorMessage` bertipe `string?` untuk nilai timeline kesalahan pesan; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public string? TimelineErrorMessage { get; init; }
    // Mendefinisikan properti `SessionStatus` bertipe `string?` untuk nilai sesi status; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public string? SessionStatus { get; init; }
    // Mendefinisikan properti `ActiveRulesetDetail` bertipe `RulesetDetailViewModel?` untuk nilai aktif aturan detail; get menyediakan pembacaan nilai,
    // init membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public RulesetDetailViewModel? ActiveRulesetDetail { get; init; }
    // Mendefinisikan properti `ErrorMessage` bertipe `string?` untuk nilai kesalahan pesan; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public string? ErrorMessage { get; init; }
// Menutup scope tipe SessionDetailViewModel; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// ViewModel satu event pada timeline sesi, memuat cap waktu, nomor urut, hari, giliran, tipe aktor, tipe aksi, dan deskripsi alur.
/// </summary>
// Mendefinisikan tipe class `SessionTimelineEventViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionTimelineEventViewModel
// Membuka scope tipe SessionTimelineEventViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `Timestamp` bertipe `DateTimeOffset` untuk waktu kejadian yang menjaga urutan kronologis data; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek.
    public DateTimeOffset Timestamp { get; init; }
    // Mendefinisikan properti `SequenceNumber` bertipe `long` untuk nomor urut event yang menentukan urutan pemrosesan riwayat permainan; get
    // menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
    public long SequenceNumber { get; init; }
    // Mendefinisikan properti `DayIndex` bertipe `int` untuk nilai hari index; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int DayIndex { get; init; }
    // Mendefinisikan properti `Weekday` bertipe `string` untuk nilai weekday; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Weekday { get; init; } = string.Empty;
    // Mendefinisikan properti `ActionSlot` bertipe `int` untuk nilai aksi slot; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int ActionSlot { get; init; }
    // Mendefinisikan properti `ActorType` bertipe `string` untuk nilai actor jenis; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string ActorType { get; init; } = string.Empty;
    // Mendefinisikan properti `PlayerId` bertipe `Guid?` untuk nilai pemain identitas; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public Guid? PlayerId { get; init; }
    // Mendefinisikan properti `PlayerDisplayName` bertipe `string?` untuk nilai pemain display nama; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public string? PlayerDisplayName { get; set; }
    // Mendefinisikan properti `ActionType` bertipe `string` untuk nilai aksi jenis; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string ActionType { get; init; } = string.Empty;
    // Mendefinisikan properti `ActionSlotRole` bertipe `string` untuk nilai aksi slot role; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string ActionSlotRole { get; init; } = string.Empty;
    // Mendefinisikan properti `ActionSlotLabel` bertipe `string` untuk nilai aksi slot label; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string ActionSlotLabel { get; init; } = string.Empty;
    // Mendefinisikan properti `FlowLabel` bertipe `string` untuk nilai flow label; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string FlowLabel { get; init; } = string.Empty;
    // Mendefinisikan properti `FlowDescription` bertipe `string` untuk nilai flow description; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string FlowDescription { get; init; } = string.Empty;
// Menutup scope tipe SessionTimelineEventViewModel; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// ViewModel halaman detail pemain yang memuat ringkasan analitik, metrik gameplay, dan statistik perjalanan arus kas.
/// </summary>
// Mendefinisikan tipe class `PlayerDetailViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerDetailViewModel
// Membuka scope tipe PlayerDetailViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `SessionId` bertipe `Guid` untuk identitas unik sesi permainan yang menjadi batas data operasi ini; get menyediakan
    // pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
    public Guid SessionId { get; init; }
    // Mendefinisikan properti `PlayerId` bertipe `Guid` untuk nilai pemain identitas; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public Guid PlayerId { get; init; }
    // Mendefinisikan properti `PlayerDisplayName` bertipe `string?` untuk nilai pemain display nama; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public string? PlayerDisplayName { get; init; }
    // Mendefinisikan properti `Summary` bertipe `AnalyticsByPlayerItem?` untuk nilai summary; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public AnalyticsByPlayerItem? Summary { get; init; }
    // Mendefinisikan properti `StatSummary` bertipe `PlayerStatSummaryViewModel?` untuk nilai stat summary; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public PlayerStatSummaryViewModel? StatSummary { get; init; }
    // Mendefinisikan properti `GameplayRaw` bertipe `JsonElement?` untuk nilai gameplay raw; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public JsonElement? GameplayRaw { get; init; }
    // Mendefinisikan properti `GameplayDerived` bertipe `JsonElement?` untuk nilai gameplay derived; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public JsonElement? GameplayDerived { get; init; }
    // Mendefinisikan properti `GameplayComputedAt` bertipe `DateTimeOffset?` untuk nilai gameplay computed at; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public DateTimeOffset? GameplayComputedAt { get; init; }
    // Mendefinisikan properti `CashflowJourney` bertipe `PlayerCashflowJourneyStatsViewModel?` untuk nilai arus kas journey; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public PlayerCashflowJourneyStatsViewModel? CashflowJourney { get; init; }
    // Mendefinisikan properti `GameplayErrorMessage` bertipe `string?` untuk nilai gameplay kesalahan pesan; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public string? GameplayErrorMessage { get; init; }
    // Mendefinisikan properti `ErrorMessage` bertipe `string?` untuk nilai kesalahan pesan; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public string? ErrorMessage { get; init; }
// Menutup scope tipe PlayerDetailViewModel; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// ViewModel ringkasan evaluasi statistik pemain untuk tampilan instruktur.
/// </summary>
// Mendefinisikan tipe class `PlayerStatSummaryViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerStatSummaryViewModel
// Membuka scope tipe PlayerStatSummaryViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `CollectionMissionComplete` bertipe `bool?` untuk nilai collection misi complete; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public bool? CollectionMissionComplete { get; init; }
    // Mendefinisikan properti `Insights` bertipe `List<PlayerInstructorInsightViewModel>` untuk nilai insights; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<PlayerInstructorInsightViewModel> Insights { get; init; } = new();
// Menutup scope tipe PlayerStatSummaryViewModel; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// ViewModel satu insight deterministik untuk instruktur.
/// </summary>
// Mendefinisikan tipe class `PlayerInstructorInsightViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerInstructorInsightViewModel
// Membuka scope tipe PlayerInstructorInsightViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `Key` bertipe `string` untuk nilai kunci; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Key { get; init; } = string.Empty;
    // Mendefinisikan properti `Title` bertipe `string` untuk nilai title; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Title { get; init; } = string.Empty;
    // Mendefinisikan properti `Description` bertipe `string` untuk nilai description; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Description { get; init; } = string.Empty;
    // Mendefinisikan properti `Tone` bertipe `string` untuk nilai tone; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya nilai literal `”neutral”`.
    public string Tone { get; init; } = "neutral";
// Menutup scope tipe PlayerInstructorInsightViewModel; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// ViewModel statistik perjalanan arus kas pemain, memuat saldo awal/akhir, jumlah transaksi,
/// total arus kas masuk/keluar, arus kas bersih, puncak/terendah, serta data seri untuk grafik.
/// </summary>
// Mendefinisikan tipe class `PlayerCashflowJourneyStatsViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerCashflowJourneyStatsViewModel
// Membuka scope tipe PlayerCashflowJourneyStatsViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `StartingCash` bertipe `double` untuk nilai starting uang tunai; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek.
    public double StartingCash { get; init; }
    // Mendefinisikan properti `EndingCash` bertipe `double` untuk nilai ending uang tunai; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public double EndingCash { get; init; }
    // Mendefinisikan properti `TransactionCount` bertipe `int` untuk nilai transaction jumlah; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek.
    public int TransactionCount { get; init; }
    // Mendefinisikan properti `CashInCount` bertipe `int` untuk nilai uang tunai in jumlah; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public int CashInCount { get; init; }
    // Mendefinisikan properti `CashOutCount` bertipe `int` untuk nilai uang tunai out jumlah; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public int CashOutCount { get; init; }
    // Mendefinisikan properti `TotalCashIn` bertipe `double` untuk nilai total uang tunai in; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public double TotalCashIn { get; init; }
    // Mendefinisikan properti `TotalCashOut` bertipe `double` untuk nilai total uang tunai out; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek.
    public double TotalCashOut { get; init; }
    // Mendefinisikan properti `NetCashflow` bertipe `double` untuk nilai net arus kas; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public double NetCashflow { get; init; }
    // Mendefinisikan properti `PeakRunningNet` bertipe `double` untuk nilai peak running net; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public double PeakRunningNet { get; init; }
    // Mendefinisikan properti `LowestRunningNet` bertipe `double` untuk nilai lowest running net; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek.
    public double LowestRunningNet { get; init; }
    // Mendefinisikan properti `FirstTransactionAt` bertipe `DateTimeOffset?` untuk nilai first transaction at; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public DateTimeOffset? FirstTransactionAt { get; init; }
    // Mendefinisikan properti `LastTransactionAt` bertipe `DateTimeOffset?` untuk nilai last transaction at; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public DateTimeOffset? LastTransactionAt { get; init; }
    /// <summary>
    /// Label sumbu waktu untuk grafik perjalanan arus kas pemain.
    /// </summary>
    // Mendefinisikan properti `TimelineLabels` bertipe `List<string>` untuk nilai timeline labels; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<string> TimelineLabels { get; init; } = new();
    /// <summary>
    /// Seri data arus kas bersih berjalan (running net) untuk ditampilkan pada grafik.
    /// </summary>
    // Mendefinisikan properti `RunningNetSeries` bertipe `List<double>` untuk nilai running net series; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<double> RunningNetSeries { get; init; } = new();
    /// <summary>
    /// Daftar detail transaksi dalam format teks untuk tooltip atau legenda grafik.
    /// </summary>
    // Mendefinisikan properti `TransactionDetails` bertipe `List<string>` untuk nilai transaction rincian; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<string> TransactionDetails { get; init; } = new();
// Menutup scope tipe PlayerCashflowJourneyStatsViewModel; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// ViewModel halaman direktori pemain yang memuat daftar semua pemain dan pengelompokan pemain berdasarkan sesi.
/// </summary>
// Mendefinisikan tipe class `PlayerDirectoryViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerDirectoryViewModel
// Membuka scope tipe PlayerDirectoryViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Daftar seluruh pemain yang terdaftar dalam sistem.
    /// </summary>
    // Mendefinisikan properti `Players` bertipe `List<PlayerResponse>` untuk nilai pemain; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<PlayerResponse> Players { get; init; } = new();
    /// <summary>
    /// Daftar grup sesi yang masing-masing berisi data pemain peserta sesi tersebut.
    /// </summary>
    // Mendefinisikan properti `SessionGroups` bertipe `List<PlayerSessionGroupViewModel>` untuk nilai sesi groups; get menyediakan pembacaan nilai,
    // init membatasi pengisian saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<PlayerSessionGroupViewModel> SessionGroups { get; init; } = new();
    // Mendefinisikan properti `ErrorMessage` bertipe `string?` untuk nilai kesalahan pesan; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public string? ErrorMessage { get; set; }
// Menutup scope tipe PlayerDirectoryViewModel; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// ViewModel satu grup sesi dalam direktori pemain, memuat metadata sesi dan daftar pemain peserta.
/// </summary>
// Mendefinisikan tipe class `PlayerSessionGroupViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerSessionGroupViewModel
// Membuka scope tipe PlayerSessionGroupViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `SessionId` bertipe `Guid` untuk identitas unik sesi permainan yang menjadi batas data operasi ini; get menyediakan
    // pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
    public Guid SessionId { get; init; }
    // Mendefinisikan properti `SessionName` bertipe `string` untuk nilai sesi nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string SessionName { get; init; } = string.Empty;
    // Mendefinisikan properti `Status` bertipe `string` untuk nilai status; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Status { get; init; } = string.Empty;
    // Mendefinisikan properti `StartedAt` bertipe `DateTimeOffset?` untuk nilai started at; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public DateTimeOffset? StartedAt { get; init; }
    // Mendefinisikan properti `EndedAt` bertipe `DateTimeOffset?` untuk nilai ended at; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public DateTimeOffset? EndedAt { get; init; }
    /// <summary>
    /// Daftar pemain peserta dalam grup sesi ini beserta metrik singkat mereka.
    /// </summary>
    // Mendefinisikan properti `Players` bertipe `List<PlayerSessionEntryViewModel>` untuk nilai pemain; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<PlayerSessionEntryViewModel> Players { get; init; } = new();
// Menutup scope tipe PlayerSessionGroupViewModel; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// ViewModel satu entri pemain dalam grup sesi, memuat urutan bergabung, nama tampilan, dan metrik keuangan ringkas.
/// </summary>
// Mendefinisikan tipe class `PlayerSessionEntryViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerSessionEntryViewModel
// Membuka scope tipe PlayerSessionEntryViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `PlayerId` bertipe `Guid` untuk nilai pemain identitas; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public Guid PlayerId { get; init; }
    // Mendefinisikan properti `PlayerOrder` bertipe `int` untuk nomor urut pemain untuk menentukan urutan tindakan; get menyediakan pembacaan nilai,
    // init membatasi pengisian saat inisialisasi objek.
    public int PlayerOrder { get; init; }
    // Mendefinisikan properti `FinalRank` bertipe `int` untuk nilai akhir rank; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int FinalRank { get; init; }
    // Mendefinisikan properti `DisplayName` bertipe `string` untuk nilai display nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string DisplayName { get; init; } = string.Empty;
    // Mendefinisikan properti `CashInTotal` bertipe `double` untuk jumlah seluruh pemasukan arus kas; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek.
    public double CashInTotal { get; init; }
    // Mendefinisikan properti `CashOutTotal` bertipe `double` untuk jumlah seluruh pengeluaran arus kas; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek.
    public double CashOutTotal { get; init; }
    // Mendefinisikan properti `DonationTotal` bertipe `double` untuk nilai donasi total; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public double DonationTotal { get; init; }
    // Mendefinisikan properti `DonationPointsTotal` bertipe `double` untuk nilai donasi poin total; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek.
    public double DonationPointsTotal { get; init; }
    // Mendefinisikan properti `PensionPointsTotal` bertipe `double` untuk nilai pension poin total; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek.
    public double PensionPointsTotal { get; init; }
    // Mendefinisikan properti `GoldQty` bertipe `int` untuk nilai emas qty; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek.
    public int GoldQty { get; init; }
    // Mendefinisikan properti `HappinessPointsTotal` bertipe `double` untuk akumulasi poin kebahagiaan pemain; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek.
    public double HappinessPointsTotal { get; init; }
// Menutup scope tipe PlayerSessionEntryViewModel; bagian berikut berada di luar batas blok tersebut.
}
