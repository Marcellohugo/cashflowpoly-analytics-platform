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
// Membuka scope tipe RulesetListViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Daftar item ruleset yang tersedia untuk ditampilkan pada halaman daftar.
    /// </summary>
    // Mendefinisikan properti `Items` bertipe `List<RulesetListItem>` untuk nilai elemen; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; nilai awalnya objek baru dengan tipe mengikuti konteks tujuan dan argumen ().
    public List<RulesetListItem> Items { get; init; } = new();
    // Mendefinisikan properti `ErrorMessage` bertipe `string?` untuk nilai kesalahan pesan; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public string? ErrorMessage { get; init; }
// Menutup scope tipe RulesetListViewModel; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// ViewModel formulir pembuatan atau pengeditan ruleset, memuat nama, deskripsi, dan definition JSON editor state.
/// </summary>
// Mendefinisikan tipe class `CreateRulesetViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class CreateRulesetViewModel
// Membuka scope tipe CreateRulesetViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `RulesetId` bertipe `Guid?` untuk identitas kumpulan aturan permainan; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public Guid? RulesetId { get; set; }
    // Mendefinisikan properti `IsEditMode` bertipe `bool` untuk nilai berstatus edit mode; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai.
    public bool IsEditMode { get; set; }
    // Mendefinisikan properti `Name` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, set mengizinkan penggantian nilai; nilai
    // awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Name { get; set; } = string.Empty;
    // Mendefinisikan properti `Description` bertipe `string?` untuk nilai description; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; tanda ? mengizinkan nilai null.
    public string? Description { get; set; }
    // Mendefinisikan properti `DefinitionJson` bertipe `string` untuk nilai definisi JSON; get menyediakan pembacaan nilai, set mengizinkan penggantian
    // nilai; nilai awalnya nilai literal `”{}”`.
    public string DefinitionJson { get; set; } = "{}";
    // Mendefinisikan properti `ErrorMessage` bertipe `string?` untuk nilai kesalahan pesan; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public string? ErrorMessage { get; set; }
// Menutup scope tipe CreateRulesetViewModel; bagian berikut berada di luar batas blok tersebut.
}

/// <summary>
/// ViewModel halaman detail ruleset yang menampilkan data ruleset, komponen terkait, dan status baca-saja.
/// </summary>
// Mendefinisikan tipe class `RulesetDetailViewModel`; sealed mencegah tipe ini diturunkan lagi.
public sealed class RulesetDetailViewModel
// Membuka scope tipe RulesetDetailViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `Ruleset` bertipe `RulesetDetailResponse?` untuk nilai aturan; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public RulesetDetailResponse? Ruleset { get; init; }
    // Mendefinisikan properti `Components` bertipe `RulesetComponentsResponse?` untuk nilai komponen; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public RulesetComponentsResponse? Components { get; init; }
    // Mendefinisikan properti `CompatibilityDefinitionJson` bertipe `JsonElement?` untuk nilai compatibility definisi JSON; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public JsonElement? CompatibilityDefinitionJson { get; init; }
    // Mendefinisikan properti `CompatibilityComponentCatalog` bertipe `JsonElement?` untuk nilai compatibility komponen catalog; get menyediakan
    // pembacaan nilai, init membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public JsonElement? CompatibilityComponentCatalog { get; init; }
    // Mendefinisikan properti `ErrorMessage` bertipe `string?` untuk nilai kesalahan pesan; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public string? ErrorMessage { get; init; }
    // Mendefinisikan properti `InfoMessage` bertipe `string?` untuk nilai info pesan; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public string? InfoMessage { get; init; }
    // Mendefinisikan properti `ComponentsErrorMessage` bertipe `string?` untuk nilai komponen kesalahan pesan; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public string? ComponentsErrorMessage { get; init; }
    // Mendefinisikan properti `IsReadOnly` bertipe `bool` untuk nilai berstatus read only; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public bool IsReadOnly { get; init; }
    // Mendefinisikan properti `IsDefaultCatalogSource` bertipe `bool` untuk nilai berstatus bawaan catalog source; get menyediakan pembacaan nilai,
    // init membatasi pengisian saat inisialisasi objek.
    public bool IsDefaultCatalogSource { get; init; }
// Menutup scope tipe RulesetDetailViewModel; bagian berikut berada di luar batas blok tersebut.
}
