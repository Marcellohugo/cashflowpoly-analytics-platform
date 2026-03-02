// Fungsi file: Mendefinisikan ViewModel untuk halaman analitik, termasuk daftar sesi, detail sesi, detail pemain, perjalanan arus kas, dan direktori pemain.
using System.Text.Json;

namespace Cashflowpoly.Ui.Models;

/// <summary>
/// ViewModel halaman daftar sesi permainan yang memuat koleksi item sesi dan pesan error opsional.
/// </summary>
public sealed class SessionListViewModel
{
    /// <summary>
    /// Daftar item sesi permainan yang diperoleh dari API untuk ditampilkan pada halaman daftar.
    /// </summary>
    public List<SessionListItemDto> Items { get; init; } = new();
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// ViewModel halaman detail sesi yang memuat data analitik sesi, peta nama pemain, timeline event, dan status sesi.
/// </summary>
public sealed class SessionDetailViewModel
{
    public Guid SessionId { get; init; }
    public AnalyticsSessionResponseDto? Analytics { get; init; }
    /// <summary>
    /// Kamus pemetaan ID pemain ke nama tampilan untuk resolusi nama pada halaman detail sesi.
    /// </summary>
    public Dictionary<Guid, string> PlayerDisplayNames { get; init; } = new();
    /// <summary>
    /// Daftar event timeline sesi yang menampilkan urutan aksi dalam permainan secara kronologis.
    /// </summary>
    public List<SessionTimelineEventViewModel> Timeline { get; init; } = new();
    public string? TimelineErrorMessage { get; init; }
    public string? SessionStatus { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// ViewModel satu event pada timeline sesi, memuat cap waktu, nomor urut, hari, giliran, tipe aktor, tipe aksi, dan deskripsi alur.
/// </summary>
public sealed class SessionTimelineEventViewModel
{
    public DateTimeOffset Timestamp { get; init; }
    public long SequenceNumber { get; init; }
    public int DayIndex { get; init; }
    public string Weekday { get; init; } = string.Empty;
    public int TurnNumber { get; init; }
    public string ActorType { get; init; } = string.Empty;
    public Guid? PlayerId { get; init; }
    public string? PlayerDisplayName { get; set; }
    public string ActionType { get; init; } = string.Empty;
    public string FlowLabel { get; init; } = string.Empty;
    public string FlowDescription { get; init; } = string.Empty;
}

/// <summary>
/// ViewModel halaman detail pemain yang memuat ringkasan analitik, riwayat transaksi, metrik gameplay mentah/turunan, dan statistik perjalanan arus kas.
/// </summary>
public sealed class PlayerDetailViewModel
{
    public Guid SessionId { get; init; }
    public Guid PlayerId { get; init; }
    public string? PlayerDisplayName { get; init; }
    public AnalyticsByPlayerItemDto? Summary { get; init; }
    /// <summary>
    /// Daftar riwayat transaksi keuangan pemain dalam sesi ini.
    /// </summary>
    public List<TransactionHistoryItemDto> Transactions { get; init; } = new();
    public JsonElement? GameplayRaw { get; init; }
    public JsonElement? GameplayDerived { get; init; }
    public DateTimeOffset? GameplayComputedAt { get; init; }
    public PlayerCashflowJourneyStatsViewModel? CashflowJourney { get; init; }
    public string? GameplayErrorMessage { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// ViewModel statistik perjalanan arus kas pemain, memuat saldo awal/akhir, jumlah transaksi,
/// total arus kas masuk/keluar, arus kas bersih, puncak/terendah, serta data seri untuk grafik.
/// </summary>
public sealed class PlayerCashflowJourneyStatsViewModel
{
    public double StartingCash { get; init; }
    public double EndingCash { get; init; }
    public int TransactionCount { get; init; }
    public int CashInCount { get; init; }
    public int CashOutCount { get; init; }
    public double TotalCashIn { get; init; }
    public double TotalCashOut { get; init; }
    public double NetCashflow { get; init; }
    public double PeakRunningNet { get; init; }
    public double LowestRunningNet { get; init; }
    public DateTimeOffset? FirstTransactionAt { get; init; }
    public DateTimeOffset? LastTransactionAt { get; init; }
    /// <summary>
    /// Label sumbu waktu untuk grafik perjalanan arus kas pemain.
    /// </summary>
    public List<string> TimelineLabels { get; init; } = new();
    /// <summary>
    /// Seri data arus kas bersih berjalan (running net) untuk ditampilkan pada grafik.
    /// </summary>
    public List<double> RunningNetSeries { get; init; } = new();
    /// <summary>
    /// Daftar detail transaksi dalam format teks untuk tooltip atau legenda grafik.
    /// </summary>
    public List<string> TransactionDetails { get; init; } = new();
}

/// <summary>
/// ViewModel halaman pemilihan ruleset untuk sesi, memuat daftar ruleset yang tersedia dan pilihan yang dipilih.
/// </summary>
public sealed class SessionRulesetViewModel
{
    public Guid SessionId { get; set; }
    /// <summary>
    /// Daftar ruleset yang tersedia untuk dipilih dan dikaitkan dengan sesi permainan.
    /// </summary>
    public List<RulesetListItemDto> Rulesets { get; set; } = new();
    public Guid? SelectedRulesetId { get; set; }
    public int? SelectedVersion { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// ViewModel halaman direktori pemain yang memuat daftar semua pemain dan pengelompokan pemain berdasarkan sesi.
/// </summary>
public sealed class PlayerDirectoryViewModel
{
    /// <summary>
    /// Daftar seluruh pemain yang terdaftar dalam sistem.
    /// </summary>
    public List<PlayerResponseDto> Players { get; init; } = new();
    /// <summary>
    /// Daftar grup sesi yang masing-masing berisi data pemain peserta sesi tersebut.
    /// </summary>
    public List<PlayerSessionGroupViewModel> SessionGroups { get; init; } = new();
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// ViewModel satu grup sesi dalam direktori pemain, memuat metadata sesi dan daftar pemain peserta.
/// </summary>
public sealed class PlayerSessionGroupViewModel
{
    public Guid SessionId { get; init; }
    public string SessionName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? EndedAt { get; init; }
    /// <summary>
    /// Daftar pemain peserta dalam grup sesi ini beserta metrik singkat mereka.
    /// </summary>
    public List<PlayerSessionEntryViewModel> Players { get; init; } = new();
}

/// <summary>
/// ViewModel satu entri pemain dalam grup sesi, memuat urutan bergabung, nama tampilan, dan metrik keuangan ringkas.
/// </summary>
public sealed class PlayerSessionEntryViewModel
{
    public Guid PlayerId { get; init; }
    public int JoinOrder { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public double CashInTotal { get; init; }
    public double CashOutTotal { get; init; }
    public double DonationTotal { get; init; }
    public double DonationPointsTotal { get; init; }
    public double PensionPointsTotal { get; init; }
    public int GoldQty { get; init; }
    public double HappinessPointsTotal { get; init; }
}
