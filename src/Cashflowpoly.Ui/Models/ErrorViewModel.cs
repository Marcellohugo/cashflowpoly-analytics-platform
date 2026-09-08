// Fungsi file: Mendefinisikan model tampilan dan state UI untuk ErrorViewModel.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Models` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Models;

/// <summary>
/// ViewModel yang membawa data error seperti RequestId
/// untuk ditampilkan pada halaman error standar aplikasi.
/// </summary>
// Mendefinisikan tipe class `ErrorViewModel`.
public class ErrorViewModel
// Membuka scope tipe ErrorViewModel; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan properti `RequestId` bertipe `string?` untuk nilai permintaan identitas; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai; tanda ? mengizinkan nilai null.
    public string? RequestId { get; set; }

    /// <summary>
    /// Menentukan apakah RequestId perlu ditampilkan; bernilai true jika RequestId tidak null atau kosong.
    /// </summary>
    // Mendefinisikan properti `ShowRequestId` bertipe `bool` untuk nilai show permintaan identitas; nilainya dihitung dari kebalikan kondisi
    // `string.IsNullOrEmpty(RequestId)`.
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
// Menutup scope tipe ErrorViewModel; bagian berikut berada di luar batas blok tersebut.
}
