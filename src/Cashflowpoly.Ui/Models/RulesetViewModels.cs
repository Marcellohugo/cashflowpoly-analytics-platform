// Fungsi file: Mendefinisikan model tampilan dan state UI untuk RulesetViewModels.
// Mengimpor namespace `Cashflowpoly.Ui.Contracts` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Contracts;
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Models` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// ViewModel halaman daftar ruleset yang memuat koleksi ruleset dan pesan error.
/// </summary>
// Mendefinisikan tipe class `RulesetListViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetListViewModel
{
    /// <summary>
    /// Daftar item ruleset yang tersedia untuk ditampilkan pada halaman daftar.
    /// </summary>
    // Mendefinisikan properti `Items` bertipe `List<RulesetListItem>` untuk nilai elemen; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<RulesetListItem> Items { get; init; } = new();
    public bool RulesetsAvailable { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// ViewModel formulir pembuatan atau pengeditan ruleset, memuat nama, deskripsi, dan definition JSON editor state.
/// </summary>
// Mendefinisikan tipe class `CreateRulesetViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class CreateRulesetViewModel
{
    public Guid? RulesetId { get; set; }
    public bool IsEditMode { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(120)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DefinitionJson { get; set; } = "{}";
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// ViewModel halaman detail ruleset yang menampilkan data ruleset, komponen terkait, dan status baca-saja.
/// </summary>
// Mendefinisikan tipe class `RulesetDetailViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetDetailViewModel
{
    public RulesetDetailResponse? Ruleset { get; init; }
    public RulesetComponentsResponse? Components { get; init; }
    public JsonElement? CompatibilityDefinitionJson { get; init; }
    public JsonElement? CompatibilityComponentCatalog { get; init; }
    public string? ErrorMessage { get; init; }
    public string? InfoMessage { get; init; }
    public string? ComponentsErrorMessage { get; init; }
    public bool IsReadOnly { get; init; }
    public bool IsDefaultCatalogSource { get; init; }
}
