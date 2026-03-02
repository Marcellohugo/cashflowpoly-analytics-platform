// Fungsi file: Mendefinisikan ViewModel untuk halaman daftar, pembuatan, dan detail ruleset permainan.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// ViewModel halaman daftar ruleset yang memuat koleksi ruleset beserta komponen default dan pesan error.
/// </summary>
public sealed class RulesetListViewModel
{
    /// <summary>
    /// Daftar item ruleset yang tersedia untuk ditampilkan pada halaman daftar.
    /// </summary>
    public List<RulesetListItemDto> Items { get; init; } = new();
    /// <summary>
    /// Daftar komponen default bawaan dari katalog ruleset untuk referensi pengguna.
    /// </summary>
    public List<DefaultRulesetComponentItemDto> DefaultComponentItems { get; init; } = new();
    public string? ErrorMessage { get; init; }
    public string? DefaultComponentsErrorMessage { get; init; }
}

/// <summary>
/// ViewModel formulir pembuatan atau pengeditan ruleset, memuat nama, deskripsi, dan konfigurasi JSON.
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
/// ViewModel halaman detail ruleset yang menampilkan data ruleset, komponen terkait, dan status baca-saja.
/// </summary>
public sealed class RulesetDetailViewModel
{
    public RulesetDetailResponseDto? Ruleset { get; init; }
    public RulesetComponentsResponseDto? Components { get; init; }
    public string? ErrorMessage { get; init; }
    public string? InfoMessage { get; init; }
    public string? ComponentsErrorMessage { get; init; }
    public bool IsReadOnly { get; init; }
    public bool IsDefaultCatalogSource { get; init; }
}
