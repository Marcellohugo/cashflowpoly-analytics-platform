// Fungsi file: Mendefinisikan model tampilan dan state UI untuk RulebookViewModels.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Models` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// ViewModel utama halaman rulebook yang memuat judul, subjudul,
/// daftar bagian aturan, dan daftar item penilaian (scoring).
/// </summary>
// Mendefinisikan tipe class `RulebookPageViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulebookPageViewModel
{
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    /// <summary>
    /// Daftar bagian (section) aturan yang ditampilkan pada halaman rulebook.
    /// </summary>
    // Mendefinisikan properti `Sections` bertipe `List<RulebookSectionViewModel>` untuk nilai sections; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<RulebookSectionViewModel> Sections { get; init; } = new();
    /// <summary>
    /// Daftar item penilaian (scoring) yang menjelaskan kategori dan aturan skor permainan.
    /// </summary>
    // Mendefinisikan properti `Scoring` bertipe `List<RulebookScoreItemViewModel>` untuk nilai scoring; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<RulebookScoreItemViewModel> Scoring { get; init; } = new();
}

/// <summary>
/// ViewModel untuk satu bagian aturan dalam rulebook, berisi judul, deskripsi, dan poin-poin penjelasan.
/// </summary>
// Mendefinisikan tipe class `RulebookSectionViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulebookSectionViewModel
{
    public string Heading { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    /// <summary>
    /// Daftar poin-poin penjelasan di dalam bagian aturan ini.
    /// </summary>
    // Mendefinisikan properti `Points` bertipe `List<string>` untuk nilai poin; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<string> Points { get; init; } = new();
}

/// <summary>
/// ViewModel untuk satu item penilaian dalam rulebook, memuat kategori dan aturan skor terkait.
/// </summary>
// Mendefinisikan tipe class `RulebookScoreItemViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulebookScoreItemViewModel
{
    public string Category { get; init; } = string.Empty;
    public string Rule { get; init; } = string.Empty;
}
