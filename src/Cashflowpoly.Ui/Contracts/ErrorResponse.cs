// Fungsi file: Mendefinisikan kontrak pertukaran data UI dengan API untuk ErrorResponse.
// Mengimpor namespace `System.Text.Json.Serialization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json.Serialization;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Contracts` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Contracts;

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `ErrorDetail`; sealed mencegah tipe ini diturunkan lagi.
public sealed record ErrorDetail(
    // Parameter `Field` bertipe `string` membawa nilai field; memetakan nama properti JSON menjadi (”field”).
    [property: JsonPropertyName("field")] string Field,
    // Parameter `Issue` bertipe `string` membawa nilai issue; memetakan nama properti JSON menjadi (”issue”).
    [property: JsonPropertyName("issue")] string Issue);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `ErrorResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record ErrorResponse(
    // Parameter `ErrorCode` bertipe `string` membawa nilai kesalahan kode; memetakan nama properti JSON menjadi (”error_code”).
    [property: JsonPropertyName("error_code")] string ErrorCode,
    // Parameter `Message` bertipe `string` membawa nilai pesan; memetakan nama properti JSON menjadi (”message”).
    [property: JsonPropertyName("message")] string Message,
    // Parameter `Details` bertipe `List<ErrorDetail>` membawa nilai rincian; memetakan nama properti JSON menjadi (”details”).
    [property: JsonPropertyName("details")] List<ErrorDetail> Details,
    // Parameter `TraceId` bertipe `string` membawa identitas penelusuran yang menghubungkan respons, log, dan permintaan yang sama; memetakan nama
    // properti JSON menjadi (”trace_id”).
    [property: JsonPropertyName("trace_id")] string TraceId);
