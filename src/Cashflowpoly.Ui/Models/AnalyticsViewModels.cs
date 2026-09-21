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
{
    /// <summary>
    /// Daftar item sesi permainan yang diperoleh dari API untuk ditampilkan pada halaman daftar.
    /// </summary>
    // Mendefinisikan properti `Items` bertipe `List<SessionListItem>` untuk nilai elemen; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<SessionListItem> Items { get; init; } = new();
    public List<PlayerSessionGroupViewModel> SessionGroups { get; init; } = [];
    public int? MonitoredPlayers { get; init; }
    public bool SessionsAvailable { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// ViewModel halaman detail sesi yang memuat data analitik sesi, peta nama pemain, timeline event, dan status sesi.
/// </summary>
// Mendefinisikan tipe class `SessionDetailViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionDetailViewModel
{
    public Guid SessionId { get; init; }
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
    public string? TimelineCursor { get; init; }
    public string? TimelineErrorMessage { get; init; }
    public string? SessionStatus { get; init; }
    public RulesetDetailViewModel? ActiveRulesetDetail { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// ViewModel satu event pada timeline sesi, memuat cap waktu, nomor urut, hari, giliran, tipe aktor, tipe aksi, dan deskripsi alur.
/// </summary>
// Mendefinisikan tipe class `SessionTimelineEventViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionTimelineEventViewModel
{
    public DateTimeOffset Timestamp { get; init; }
    public bool IsSealed { get; init; }
    public long SequenceNumber { get; init; }
    public int DayIndex { get; init; }
    public string Weekday { get; init; } = string.Empty;
    public int ActionSlot { get; init; }
    public string ActorType { get; init; } = string.Empty;
    public Guid? PlayerId { get; init; }
    public string? PlayerDisplayName { get; set; }
    public string ActionType { get; init; } = string.Empty;
    public string ActionSlotRole { get; init; } = string.Empty;
    public string ActionSlotLabel { get; init; } = string.Empty;
    public string FlowLabel { get; init; } = string.Empty;
    public string FlowDescription { get; init; } = string.Empty;
}

/// <summary>
/// ViewModel halaman detail pemain yang memuat ringkasan analitik, metrik gameplay, dan statistik perjalanan arus kas.
/// </summary>
// Mendefinisikan tipe class `PlayerDetailViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerDetailViewModel
{
    public Guid SessionId { get; init; }
    public Guid PlayerId { get; init; }
    public string? PlayerDisplayName { get; init; }
    public int? PlayerOrder { get; init; }
    public AnalyticsByPlayerItem? Summary { get; init; }
    public PlayerStatSummaryViewModel? StatSummary { get; init; }
    public JsonElement? GameplayRaw { get; init; }
    public JsonElement? GameplayDerived { get; init; }
    public DateTimeOffset? GameplayComputedAt { get; init; }
    public GameplayMetricsResponse? Gameplay { get; init; }
    public string? GameplayErrorMessage { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// ViewModel ringkasan evaluasi statistik pemain untuk tampilan instruktur.
/// </summary>
// Mendefinisikan tipe class `PlayerStatSummaryViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerStatSummaryViewModel
{
    public double? CashInTotal { get; init; }
    public double? CashOutTotal { get; init; }
    public double? NetCashflow { get; init; }
    public double? HappinessPoints { get; init; }
    public double? FulfillmentDiversity { get; init; }
    public bool? HasUnpaidLoan { get; init; }
    public bool? CollectionMissionComplete { get; init; }
    public List<PlayerInstructorInsightViewModel> Insights { get; init; } = new();
}

/// <summary>
/// ViewModel satu insight deterministik untuk instruktur.
/// </summary>
// Mendefinisikan tipe class `PlayerInstructorInsightViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class PlayerInstructorInsightViewModel
{
    public string Key { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Tone { get; init; } = "neutral";
}

/// <summary>Peserta satu sesi beserta hasil akhir yang sudah tersedia.</summary>
public sealed class PlayerSessionGroupViewModel
{
    public Guid SessionId { get; init; }
    public string SessionName { get; init; } = string.Empty;
    public string Mode { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? EndedAt { get; init; }
    public bool ParticipantsAvailable { get; init; }
    public bool ResultsAvailable { get; init; }
    public bool Unauthorized { get; init; }
    public List<PlayerSessionEntryViewModel> Players { get; init; } = [];
}

public sealed class PlayerSessionEntryViewModel
{
    public Guid PlayerId { get; init; }
    public int PlayerOrder { get; init; }
    public int FinalRank { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public double? HappinessPointsTotal { get; init; }
}
