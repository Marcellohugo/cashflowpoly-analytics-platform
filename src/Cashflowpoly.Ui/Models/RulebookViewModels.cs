// Fungsi file: Mendefinisikan ViewModel/DTO UI untuk domain RulebookViewModels.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// Menyatakan peran utama tipe RulebookPageViewModel pada modul ini.
/// </summary>
public sealed class RulebookPageViewModel
{
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    public List<RulebookSectionViewModel> Sections { get; init; } = new();
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    public List<RulebookScoreItemViewModel> Scoring { get; init; } = new();
}

/// <summary>
/// Menyatakan peran utama tipe RulebookSectionViewModel pada modul ini.
/// </summary>
public sealed class RulebookSectionViewModel
{
    public string Heading { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    public List<string> Points { get; init; } = new();
}

/// <summary>
/// Menyatakan peran utama tipe RulebookScoreItemViewModel pada modul ini.
/// </summary>
public sealed class RulebookScoreItemViewModel
{
    public string Category { get; init; } = string.Empty;
    public string Rule { get; init; } = string.Empty;
}
