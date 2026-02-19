// Fungsi file: Mendefinisikan ViewModel/DTO UI untuk domain ErrorViewModel.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// Menyatakan peran utama tipe ErrorViewModel pada modul ini.
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    /// <summary>
    /// Menjalankan fungsi IsNullOrEmpty sebagai bagian dari alur file ini.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
