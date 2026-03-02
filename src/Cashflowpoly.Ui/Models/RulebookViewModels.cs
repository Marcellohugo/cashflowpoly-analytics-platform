// Fungsi file: Mendefinisikan ViewModel untuk menampilkan halaman buku aturan (rulebook) permainan, termasuk bagian-bagian aturan dan tabel penilaian.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// ViewModel utama halaman rulebook yang memuat judul, subjudul,
/// daftar bagian aturan, dan daftar item penilaian (scoring).
/// </summary>
public sealed class RulebookPageViewModel
{
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    /// <summary>
    /// Daftar bagian (section) aturan yang ditampilkan pada halaman rulebook.
    /// </summary>
    public List<RulebookSectionViewModel> Sections { get; init; } = new();
    /// <summary>
    /// Daftar item penilaian (scoring) yang menjelaskan kategori dan aturan skor permainan.
    /// </summary>
    public List<RulebookScoreItemViewModel> Scoring { get; init; } = new();
}

/// <summary>
/// ViewModel untuk satu bagian aturan dalam rulebook, berisi judul, deskripsi, dan poin-poin penjelasan.
/// </summary>
public sealed class RulebookSectionViewModel
{
    public string Heading { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    /// <summary>
    /// Daftar poin-poin penjelasan di dalam bagian aturan ini.
    /// </summary>
    public List<string> Points { get; init; } = new();
}

/// <summary>
/// ViewModel untuk satu item penilaian dalam rulebook, memuat kategori dan aturan skor terkait.
/// </summary>
public sealed class RulebookScoreItemViewModel
{
    public string Category { get; init; } = string.Empty;
    public string Rule { get; init; } = string.Empty;
}
