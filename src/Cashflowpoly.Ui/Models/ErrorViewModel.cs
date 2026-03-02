// Fungsi file: Mendefinisikan model data untuk menampilkan halaman error dengan kode permintaan dan indikator tampilan.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// ViewModel yang membawa data error seperti RequestId
/// untuk ditampilkan pada halaman error standar aplikasi.
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    /// <summary>
    /// Menentukan apakah RequestId perlu ditampilkan; bernilai true jika RequestId tidak null atau kosong.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
