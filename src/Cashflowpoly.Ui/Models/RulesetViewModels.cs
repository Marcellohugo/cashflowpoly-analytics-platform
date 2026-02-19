// Fungsi file: Mendefinisikan ViewModel/DTO UI untuk domain RulesetViewModels.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// Menyatakan peran utama tipe RulesetListViewModel pada modul ini.
/// </summary>
public sealed class RulesetListViewModel
{
    /// <summary>
    /// Menjalankan fungsi new sebagai bagian dari alur file ini.
    /// </summary>
    public List<RulesetListItemDto> Items { get; init; } = new();
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Menyatakan peran utama tipe CreateRulesetViewModel pada modul ini.
/// </summary>
public sealed class CreateRulesetViewModel
{
    public Guid? RulesetId { get; set; }
    public bool IsEditMode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ConfigJson { get; set; } = "{}";
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Menyatakan peran utama tipe RulesetDetailViewModel pada modul ini.
/// </summary>
public sealed class RulesetDetailViewModel
{
    public RulesetDetailResponseDto? Ruleset { get; init; }
    public string? ErrorMessage { get; init; }
}
