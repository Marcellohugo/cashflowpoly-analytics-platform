// Fungsi file: Mendefinisikan kontrak data Dtos untuk request dan response API.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `System.Text.Json.Serialization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json.Serialization;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Api.Contracts` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Api.Contracts;

// Mendefinisikan tipe class `CreateSessionRequest`; sealed mencegah tipe ini diturunkan lagi.
public sealed class CreateSessionRequest
// Membuka scope tipe CreateSessionRequest; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan konstruktor CreateSessionRequest yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; konstruktor ini tidak
    // memerlukan argumen.
    public CreateSessionRequest()
    // Membuka scope konstruktor CreateSessionRequest; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateSessionRequest.
    {
    // Menutup scope konstruktor CreateSessionRequest; bagian berikut berada di luar batas blok tersebut dalam CreateSessionRequest.
    }

    // Mendefinisikan konstruktor CreateSessionRequest yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `sessionName` bertipe `string` membawa nilai sesi nama; Parameter `mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan
    // yang digunakan; Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang
    // tepat.
    public CreateSessionRequest(string sessionName, string mode, Guid rulesetVersionId)
    // Membuka scope konstruktor CreateSessionRequest; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateSessionRequest.
    {
        // Memperbarui `SessionName` menggunakan `sessionName` (nilai sesi nama) dalam CreateSessionRequest.
        SessionName = sessionName;
        // Memperbarui `Mode` menggunakan `mode` (mode permainan yang menentukan kelompok aturan yang digunakan) dalam CreateSessionRequest.
        Mode = mode;
        // Memperbarui `RulesetVersionId` menggunakan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat)
        // dalam CreateSessionRequest.
        RulesetVersionId = rulesetVersionId;
    // Menutup scope konstruktor CreateSessionRequest; bagian berikut berada di luar batas blok tersebut dalam CreateSessionRequest.
    }

    // memetakan nama properti JSON menjadi (”session_name”).
    [JsonPropertyName("session_name")]
    // Mendefinisikan properti `SessionName` bertipe `string?` untuk nilai sesi nama; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public string? SessionName { get; init; }

    // memetakan nama properti JSON menjadi (”mode”).
    [JsonPropertyName("mode")]
    // Mendefinisikan properti `Mode` bertipe `string?` untuk mode permainan yang menentukan kelompok aturan yang digunakan; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public string? Mode { get; init; }

    // memetakan nama properti JSON menjadi (”ruleset_version_id”).
    [JsonPropertyName("ruleset_version_id")]
    // Mendefinisikan properti `RulesetVersionId` bertipe `Guid?` untuk identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang
    // tepat; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public Guid? RulesetVersionId { get; init; }

    // memetakan nama properti JSON menjadi (”player_names”).
    [JsonPropertyName("player_names")]
    // Mendefinisikan properti `PlayerNames` bertipe `List<string>?` untuk nilai pemain nama; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public List<string>? PlayerNames { get; init; }
// Menutup scope tipe CreateSessionRequest; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `CreateSessionResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed class CreateSessionResponse
// Membuka scope tipe CreateSessionResponse; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan konstruktor CreateSessionResponse yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; konstruktor ini tidak
    // memerlukan argumen.
    public CreateSessionResponse()
    // Membuka scope konstruktor CreateSessionResponse; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateSessionResponse.
    {
    // Menutup scope konstruktor CreateSessionResponse; bagian berikut berada di luar batas blok tersebut dalam CreateSessionResponse.
    }

    // Mendefinisikan konstruktor CreateSessionResponse yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini.
    public CreateSessionResponse(Guid sessionId)
    // Membuka scope konstruktor CreateSessionResponse; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateSessionResponse.
    {
        // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam CreateSessionResponse.
        SessionId = sessionId;
    // Menutup scope konstruktor CreateSessionResponse; bagian berikut berada di luar batas blok tersebut dalam CreateSessionResponse.
    }

    // Mendefinisikan konstruktor CreateSessionResponse yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `rulesetId` bertipe `Guid`
    // membawa identitas kumpulan aturan permainan; Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan
    // memakai konfigurasi aturan yang tepat.
    public CreateSessionResponse(Guid sessionId, Guid rulesetId, Guid rulesetVersionId)
    // Membuka scope konstruktor CreateSessionResponse; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateSessionResponse.
    {
        // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam CreateSessionResponse.
        SessionId = sessionId;
        // Memperbarui `RulesetId` menggunakan `rulesetId` (identitas kumpulan aturan permainan) dalam CreateSessionResponse.
        RulesetId = rulesetId;
        // Memperbarui `RulesetVersionId` menggunakan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat)
        // dalam CreateSessionResponse.
        RulesetVersionId = rulesetVersionId;
    // Menutup scope konstruktor CreateSessionResponse; bagian berikut berada di luar batas blok tersebut dalam CreateSessionResponse.
    }

    // Mendefinisikan konstruktor CreateSessionResponse yang menyiapkan objek dan menerima dependency/nilai awal dari pemanggil; parameter: Parameter
    // `sessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; Parameter `rulesetId` bertipe `Guid`
    // membawa identitas kumpulan aturan permainan; Parameter `rulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan
    // memakai konfigurasi aturan yang tepat; Parameter `state` bertipe `SessionStateResponse` membawa keadaan permainan yang menjadi sumber atau hasil
    // pembaruan.
    public CreateSessionResponse(Guid sessionId, Guid rulesetId, Guid rulesetVersionId, SessionStateResponse state)
    // Membuka scope konstruktor CreateSessionResponse; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam CreateSessionResponse.
    {
        // Memperbarui `SessionId` menggunakan `sessionId` (identitas unik sesi permainan yang menjadi batas data operasi ini) dalam CreateSessionResponse.
        SessionId = sessionId;
        // Memperbarui `RulesetId` menggunakan `rulesetId` (identitas kumpulan aturan permainan) dalam CreateSessionResponse.
        RulesetId = rulesetId;
        // Memperbarui `RulesetVersionId` menggunakan `rulesetVersionId` (identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat)
        // dalam CreateSessionResponse.
        RulesetVersionId = rulesetVersionId;
        // Memperbarui `State` menggunakan `state` (keadaan permainan yang menjadi sumber atau hasil pembaruan) dalam CreateSessionResponse.
        State = state;
    // Menutup scope konstruktor CreateSessionResponse; bagian berikut berada di luar batas blok tersebut dalam CreateSessionResponse.
    }

    // memetakan nama properti JSON menjadi (”session_id”).
    [JsonPropertyName("session_id")]
    // Mendefinisikan properti `SessionId` bertipe `Guid` untuk identitas unik sesi permainan yang menjadi batas data operasi ini; get menyediakan
    // pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
    public Guid SessionId { get; init; }

    // memetakan nama properti JSON menjadi (”ruleset_id”).
    [JsonPropertyName("ruleset_id")]
    // mengatur pengabaian properti JSON sesuai (Condition = JsonIgnoreCondition.WhenWritingNull) saat serialisasi/deserialisasi.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    // Mendefinisikan properti `RulesetId` bertipe `Guid?` untuk identitas kumpulan aturan permainan; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public Guid? RulesetId { get; init; }

    // memetakan nama properti JSON menjadi (”ruleset_version_id”).
    [JsonPropertyName("ruleset_version_id")]
    // mengatur pengabaian properti JSON sesuai (Condition = JsonIgnoreCondition.WhenWritingNull) saat serialisasi/deserialisasi.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    // Mendefinisikan properti `RulesetVersionId` bertipe `Guid?` untuk identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang
    // tepat; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public Guid? RulesetVersionId { get; init; }

    // memetakan nama properti JSON menjadi (”state”).
    [JsonPropertyName("state")]
    // mengatur pengabaian properti JSON sesuai (Condition = JsonIgnoreCondition.WhenWritingNull) saat serialisasi/deserialisasi.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    // Mendefinisikan properti `State` bertipe `SessionStateResponse?` untuk keadaan permainan yang menjadi sumber atau hasil pembaruan; get menyediakan
    // pembacaan nilai, init membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public SessionStateResponse? State { get; init; }
// Menutup scope tipe CreateSessionResponse; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `SessionStatusResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record SessionStatusResponse([property: JsonPropertyName("status")] string Status);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `SessionPlayerSetupRequest`; sealed mencegah tipe ini diturunkan lagi.
public sealed record SessionPlayerSetupRequest(
    // Parameter `SessionPlayerId` bertipe `Guid` membawa identitas keikutsertaan pemain pada sesi tertentu; memetakan nama properti JSON menjadi
    // (”session_player_id”).
    [property: JsonPropertyName("session_player_id")] Guid SessionPlayerId,
    // Parameter `TieBreakerCode` bertipe `string` membawa kode kartu penentu urutan saat nilai pemain sama; memetakan nama properti JSON menjadi
    // (”tie_breaker_code”).
    [property: JsonPropertyName("tie_breaker_code")] string TieBreakerCode,
    // Parameter `IngredientCardId` bertipe `string` membawa identitas kartu bahan yang diberikan atau digunakan pemain; memetakan nama properti JSON
    // menjadi (”ingredient_card_id”).
    [property: JsonPropertyName("ingredient_card_id")] string IngredientCardId,
    // Parameter `GoldQuantity` bertipe `int` membawa jumlah kartu atau unit emas pemain; memetakan nama properti JSON menjadi (”gold_quantity”).
    [property: JsonPropertyName("gold_quantity")] int GoldQuantity,
    // Parameter `MissionId` bertipe `string?` membawa identitas misi koleksi yang ditugaskan; nilai null diizinkan ketika data opsional belum tersedia;
    // memetakan nama properti JSON menjadi (”mission_id”).
    [property: JsonPropertyName("mission_id")] string? MissionId,
    // Parameter `LoanCode` bertipe `string?` membawa kode produk pinjaman syariah; nilai null diizinkan ketika data opsional belum tersedia; memetakan
    // nama properti JSON menjadi (”loan_code”).
    [property: JsonPropertyName("loan_code")] string? LoanCode,
    // Parameter `InsuranceProductCode` bertipe `string?` membawa kode produk asuransi yang digunakan; nilai null diizinkan ketika data opsional belum
    // tersedia; memetakan nama properti JSON menjadi (”insurance_product_code”).
    [property: JsonPropertyName("insurance_product_code")] string? InsuranceProductCode);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `SessionSetupRequest`; sealed mencegah tipe ini diturunkan lagi.
public sealed record SessionSetupRequest(
    // Parameter `ClientRequestId` bertipe `string` membawa identitas permintaan dari klien untuk pelacakan atau penanganan permintaan berulang;
    // memetakan nama properti JSON menjadi (”client_request_id”).
    [property: JsonPropertyName("client_request_id")] string ClientRequestId,
    // Parameter `Players` bertipe `List<SessionPlayerSetupRequest>` membawa nilai pemain; memetakan nama properti JSON menjadi (”players”).
    [property: JsonPropertyName("players")] List<SessionPlayerSetupRequest> Players);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `SessionSetupValidationResponse`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed record SessionSetupValidationResponse(
    // Parameter `Valid` bertipe `bool` membawa penanda apakah validasi telah memenuhi syarat; memetakan nama properti JSON menjadi (”valid”).
    [property: JsonPropertyName("valid")] bool Valid);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `SessionSetupResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record SessionSetupResponse(
    // Parameter `SessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; memetakan nama properti JSON
    // menjadi (”session_id”).
    [property: JsonPropertyName("session_id")] Guid SessionId,
    // Parameter `RulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; memetakan
    // nama properti JSON menjadi (”ruleset_version_id”).
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    // Parameter `Revision` bertipe `int` membawa nomor revisi data untuk membedakan versi penyimpanan; memetakan nama properti JSON menjadi
    // (”revision”).
    [property: JsonPropertyName("revision")] int Revision,
    // Parameter `SetupStatus` bertipe `string` membawa nilai setup status; memetakan nama properti JSON menjadi (”setup_status”).
    [property: JsonPropertyName("setup_status")] string SetupStatus,
    // Parameter `ClientRequestId` bertipe `string` membawa identitas permintaan dari klien untuk pelacakan atau penanganan permintaan berulang;
    // memetakan nama properti JSON menjadi (”client_request_id”).
    [property: JsonPropertyName("client_request_id")] string ClientRequestId,
    // Parameter `Players` bertipe `List<SessionPlayerSetupRequest>` membawa nilai pemain; memetakan nama properti JSON menjadi (”players”).
    [property: JsonPropertyName("players")] List<SessionPlayerSetupRequest> Players,
    // Parameter `SavedAt` bertipe `DateTimeOffset` membawa nilai saved at; memetakan nama properti JSON menjadi (”saved_at”).
    [property: JsonPropertyName("saved_at")] DateTimeOffset SavedAt,
    // Parameter `LockedAt` bertipe `DateTimeOffset?` membawa nilai locked at; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama
    // properti JSON menjadi (”locked_at”).
    [property: JsonPropertyName("locked_at")] DateTimeOffset? LockedAt);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `CreatePlayerRequest`; sealed mencegah tipe ini diturunkan lagi.
public sealed record CreatePlayerRequest(
    // Parameter `DisplayName` bertipe `string` membawa nilai display nama; memetakan nama properti JSON menjadi (”display_name”).
    [property: JsonPropertyName("display_name")] string DisplayName,
    // Parameter `Username` bertipe `string` membawa nama akun yang dipakai saat autentikasi; memetakan nama properti JSON menjadi (”username”).
    [property: JsonPropertyName("username")] string Username,
    // Parameter `Password` bertipe `string` membawa kata sandi masukan yang diperiksa sesuai kebijakan autentikasi; memetakan nama properti JSON
    // menjadi (”password”).
    [property: JsonPropertyName("password")] string Password);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `PlayerResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record PlayerResponse(
    // Parameter `UserId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; memetakan nama properti JSON menjadi (”user_id”).
    [property: JsonPropertyName("user_id")] Guid UserId,
    // Parameter `DisplayName` bertipe `string` membawa nilai display nama; memetakan nama properti JSON menjadi (”display_name”).
    [property: JsonPropertyName("display_name")] string DisplayName);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `PlayerListResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record PlayerListResponse([property: JsonPropertyName("items")] List<PlayerResponse> Items);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `SessionPlayerResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record SessionPlayerResponse(
    // Parameter `SessionPlayerId` bertipe `Guid` membawa identitas keikutsertaan pemain pada sesi tertentu; memetakan nama properti JSON menjadi
    // (”session_player_id”).
    [property: JsonPropertyName("session_player_id")] Guid SessionPlayerId,
    // Parameter `UserId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; memetakan nama properti JSON menjadi (”user_id”).
    [property: JsonPropertyName("user_id")] Guid UserId,
    // Parameter `DisplayName` bertipe `string` membawa nilai display nama; memetakan nama properti JSON menjadi (”display_name”).
    [property: JsonPropertyName("display_name")] string DisplayName,
    // Parameter `PlayerOrder` bertipe `int` membawa nomor urut pemain untuk menentukan urutan tindakan; memetakan nama properti JSON menjadi
    // (”player_order_no”).
    [property: JsonPropertyName("player_order_no")] int PlayerOrder);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `SessionPlayerListResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record SessionPlayerListResponse(
    // Parameter `Items` bertipe `List<SessionPlayerResponse>` membawa nilai elemen; memetakan nama properti JSON menjadi (”items”).
    [property: JsonPropertyName("items")] List<SessionPlayerResponse> Items);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AddSessionPlayerRequest`; sealed mencegah tipe ini diturunkan lagi.
public sealed record AddSessionPlayerRequest(
    // Parameter `UserId` bertipe `Guid?` membawa identitas akun pengguna yang datanya sedang diproses; nilai null diizinkan ketika data opsional belum
    // tersedia; memetakan nama properti JSON menjadi (”user_id”).
    [property: JsonPropertyName("user_id")] Guid? UserId,
    // Parameter `Username` bertipe `string?` membawa nama akun yang dipakai saat autentikasi; nilai null diizinkan ketika data opsional belum tersedia;
    // memetakan nama properti JSON menjadi (”username”).
    [property: JsonPropertyName("username")] string? Username,
    // Parameter `PlayerOrder` bertipe `int?` membawa nomor urut pemain untuk menentukan urutan tindakan; nilai null diizinkan ketika data opsional
    // belum tersedia; memetakan nama properti JSON menjadi (”player_order_no”).
    [property: JsonPropertyName("player_order_no")] int? PlayerOrder);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AddSessionPlayerResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record AddSessionPlayerResponse(
    // Parameter `UserId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; memetakan nama properti JSON menjadi (”user_id”).
    [property: JsonPropertyName("user_id")] Guid UserId,
    // Parameter `PlayerOrder` bertipe `int` membawa nomor urut pemain untuk menentukan urutan tindakan; memetakan nama properti JSON menjadi
    // (”player_order_no”).
    [property: JsonPropertyName("player_order_no")] int PlayerOrder);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `SessionListItem`; sealed mencegah tipe ini diturunkan lagi.
public sealed record SessionListItem(
    // Parameter `SessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; memetakan nama properti JSON
    // menjadi (”session_id”).
    [property: JsonPropertyName("session_id")] Guid SessionId,
    // Parameter `SessionName` bertipe `string` membawa nilai sesi nama; memetakan nama properti JSON menjadi (”session_name”).
    [property: JsonPropertyName("session_name")] string SessionName,
    // Parameter `Mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan; memetakan nama properti JSON menjadi
    // (”mode”).
    [property: JsonPropertyName("mode")] string Mode,
    // Parameter `Status` bertipe `string` membawa nilai status; memetakan nama properti JSON menjadi (”status”).
    [property: JsonPropertyName("status")] string Status,
    // Parameter `CreatedAt` bertipe `DateTimeOffset` membawa nilai created at; memetakan nama properti JSON menjadi (”created_at”).
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    // Parameter `StartedAt` bertipe `DateTimeOffset?` membawa nilai started at; nilai null diizinkan ketika data opsional belum tersedia; memetakan
    // nama properti JSON menjadi (”started_at”).
    [property: JsonPropertyName("started_at")] DateTimeOffset? StartedAt,
    // Parameter `EndedAt` bertipe `DateTimeOffset?` membawa nilai ended at; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama
    // properti JSON menjadi (”ended_at”).
    [property: JsonPropertyName("ended_at")] DateTimeOffset? EndedAt);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `SessionListResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record SessionListResponse([property: JsonPropertyName("items")] List<SessionListItem> Items);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `CreateRulesetRequest`; sealed mencegah tipe ini diturunkan lagi.
public sealed record CreateRulesetRequest(
    // Parameter `Name` bertipe `string` membawa nilai nama; memetakan nama properti JSON menjadi (”name”).
    [property: JsonPropertyName("name")] string Name,
    // Parameter `Description` bertipe `string?` membawa nilai description; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama
    // properti JSON menjadi (”description”).
    [property: JsonPropertyName("description")] string? Description,
    // Parameter `Definition` bertipe `RulesetDefinitionDto?` membawa definisi terstruktur komponen serta parameter aturan permainan; nilai null
    // diizinkan ketika data opsional belum tersedia; memetakan nama properti JSON menjadi (”definition”).
    [property: JsonPropertyName("definition")] RulesetDefinitionDto? Definition);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `UpdateRulesetRequest`; sealed mencegah tipe ini diturunkan lagi.
public sealed record UpdateRulesetRequest(
    // Parameter `Name` bertipe `string?` membawa nilai nama; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama properti JSON
    // menjadi (”name”).
    [property: JsonPropertyName("name")] string? Name,
    // Parameter `Description` bertipe `string?` membawa nilai description; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama
    // properti JSON menjadi (”description”).
    [property: JsonPropertyName("description")] string? Description,
    // Parameter `Definition` bertipe `RulesetDefinitionDto?` membawa definisi terstruktur komponen serta parameter aturan permainan; nilai null
    // diizinkan ketika data opsional belum tersedia; memetakan nama properti JSON menjadi (”definition”).
    [property: JsonPropertyName("definition")] RulesetDefinitionDto? Definition);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `CreateRulesetResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record CreateRulesetResponse(
    // Parameter `RulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan; memetakan nama properti JSON menjadi (”ruleset_id”).
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    // Parameter `RulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; memetakan
    // nama properti JSON menjadi (”ruleset_version_id”).
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    // Parameter `Version` bertipe `int` membawa nomor versi yang dipakai untuk konsistensi data atau konfigurasi; memetakan nama properti JSON menjadi
    // (”version”).
    [property: JsonPropertyName("version")] int Version);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RulesetListItem`; sealed mencegah tipe ini diturunkan lagi.
public sealed record RulesetListItem(
    // Parameter `RulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan; memetakan nama properti JSON menjadi (”ruleset_id”).
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    // Parameter `Name` bertipe `string` membawa nilai nama; memetakan nama properti JSON menjadi (”name”).
    [property: JsonPropertyName("name")] string Name,
    // Parameter `LatestVersion` bertipe `int` membawa nilai latest versi; memetakan nama properti JSON menjadi (”latest_version”).
    [property: JsonPropertyName("latest_version")] int LatestVersion,
    // Parameter `Status` bertipe `string` membawa nilai status; memetakan nama properti JSON menjadi (”status”).
    [property: JsonPropertyName("status")] string Status,
    // Parameter `IsDefault` bertipe `bool` membawa nilai berstatus bawaan; bila argumen tidak diberikan digunakan false, yaitu kondisi nonaktif/tidak
    // terpenuhi; memetakan nama properti JSON menjadi (”is_default”).
    [property: JsonPropertyName("is_default")] bool IsDefault = false,
    // Parameter `IsLockedBySession` bertipe `bool` membawa nilai berstatus locked berdasarkan sesi; bila argumen tidak diberikan digunakan false, yaitu
    // kondisi nonaktif/tidak terpenuhi; memetakan nama properti JSON menjadi (”is_locked_by_session”).
    [property: JsonPropertyName("is_locked_by_session")] bool IsLockedBySession = false,
    // Parameter `Mode` bertipe `string?` membawa mode permainan yang menentukan kelompok aturan yang digunakan; nilai null diizinkan ketika data
    // opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; memetakan nama properti JSON menjadi
    // (”mode”).
    [property: JsonPropertyName("mode")] string? Mode = null);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RulesetListResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record RulesetListResponse([property: JsonPropertyName("items")] List<RulesetListItem> Items);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RulesetVersionItem`; sealed mencegah tipe ini diturunkan lagi.
public sealed record RulesetVersionItem(
    // Parameter `RulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; memetakan
    // nama properti JSON menjadi (”ruleset_version_id”).
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    // Parameter `Version` bertipe `int` membawa nomor versi yang dipakai untuk konsistensi data atau konfigurasi; memetakan nama properti JSON menjadi
    // (”version”).
    [property: JsonPropertyName("version")] int Version,
    // Parameter `Status` bertipe `string` membawa nilai status; memetakan nama properti JSON menjadi (”status”).
    [property: JsonPropertyName("status")] string Status,
    // Parameter `CreatedAt` bertipe `DateTimeOffset` membawa nilai created at; memetakan nama properti JSON menjadi (”created_at”).
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RulesetDetailResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record RulesetDetailResponse(
    // Parameter `RulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan; memetakan nama properti JSON menjadi (”ruleset_id”).
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    // Parameter `Name` bertipe `string` membawa nilai nama; memetakan nama properti JSON menjadi (”name”).
    [property: JsonPropertyName("name")] string Name,
    // Parameter `Description` bertipe `string?` membawa nilai description; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama
    // properti JSON menjadi (”description”).
    [property: JsonPropertyName("description")] string? Description,
    // Parameter `Versions` bertipe `List<RulesetVersionItem>` membawa nilai versions; memetakan nama properti JSON menjadi (”versions”).
    [property: JsonPropertyName("versions")] List<RulesetVersionItem> Versions,
    // Parameter `RulesetVersionId` bertipe `Guid?` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; nilai
    // null diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; memetakan nama
    // properti JSON menjadi (”ruleset_version_id”).
    [property: JsonPropertyName("ruleset_version_id")] Guid? RulesetVersionId = null,
    // Parameter `Version` bertipe `int?` membawa nomor versi yang dipakai untuk konsistensi data atau konfigurasi; nilai null diizinkan ketika data
    // opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; memetakan nama properti JSON menjadi
    // (”version”).
    [property: JsonPropertyName("version")] int? Version = null,
    // Parameter `Mode` bertipe `string?` membawa mode permainan yang menentukan kelompok aturan yang digunakan; nilai null diizinkan ketika data
    // opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; memetakan nama properti JSON menjadi
    // (”mode”).
    [property: JsonPropertyName("mode")] string? Mode = null,
    // Parameter `Definition` bertipe `RulesetDefinitionDto?` membawa definisi terstruktur komponen serta parameter aturan permainan; nilai null
    // diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; memetakan nama
    // properti JSON menjadi (”definition”).
    [property: JsonPropertyName("definition")] RulesetDefinitionDto? Definition = null,
    // Parameter `IsDefault` bertipe `bool` membawa nilai berstatus bawaan; bila argumen tidak diberikan digunakan false, yaitu kondisi nonaktif/tidak
    // terpenuhi; memetakan nama properti JSON menjadi (”is_default”).
    [property: JsonPropertyName("is_default")] bool IsDefault = false,
    // Parameter `IsLockedBySession` bertipe `bool` membawa nilai berstatus locked berdasarkan sesi; bila argumen tidak diberikan digunakan false, yaitu
    // kondisi nonaktif/tidak terpenuhi; memetakan nama properti JSON menjadi (”is_locked_by_session”).
    [property: JsonPropertyName("is_locked_by_session")] bool IsLockedBySession = false);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RulesetComponentsResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record RulesetComponentsResponse(
    // Parameter `RulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan; memetakan nama properti JSON menjadi (”ruleset_id”).
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    // Parameter `RulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; memetakan
    // nama properti JSON menjadi (”ruleset_version_id”).
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    // Parameter `Version` bertipe `int` membawa nomor versi yang dipakai untuk konsistensi data atau konfigurasi; memetakan nama properti JSON menjadi
    // (”version”).
    [property: JsonPropertyName("version")] int Version,
    // Parameter `Mode` bertipe `string?` membawa mode permainan yang menentukan kelompok aturan yang digunakan; nilai null diizinkan ketika data
    // opsional belum tersedia; memetakan nama properti JSON menjadi (”mode”).
    [property: JsonPropertyName("mode")] string? Mode,
    // Parameter `Definition` bertipe `RulesetDefinitionDto?` membawa definisi terstruktur komponen serta parameter aturan permainan; nilai null
    // diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; memetakan nama
    // properti JSON menjadi (”definition”).
    [property: JsonPropertyName("definition")] RulesetDefinitionDto? Definition = null);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `DefaultRulesetComponentItem`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed record DefaultRulesetComponentItem(
    // Parameter `RulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan; memetakan nama properti JSON menjadi (”ruleset_id”).
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    // Parameter `Name` bertipe `string` membawa nilai nama; memetakan nama properti JSON menjadi (”name”).
    [property: JsonPropertyName("name")] string Name,
    // Parameter `Description` bertipe `string?` membawa nilai description; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama
    // properti JSON menjadi (”description”).
    [property: JsonPropertyName("description")] string? Description,
    // Parameter `RulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; memetakan
    // nama properti JSON menjadi (”ruleset_version_id”).
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    // Parameter `Version` bertipe `int` membawa nomor versi yang dipakai untuk konsistensi data atau konfigurasi; memetakan nama properti JSON menjadi
    // (”version”).
    [property: JsonPropertyName("version")] int Version,
    // Parameter `Mode` bertipe `string?` membawa mode permainan yang menentukan kelompok aturan yang digunakan; nilai null diizinkan ketika data
    // opsional belum tersedia; memetakan nama properti JSON menjadi (”mode”).
    [property: JsonPropertyName("mode")] string? Mode,
    // Parameter `Definition` bertipe `RulesetDefinitionDto?` membawa definisi terstruktur komponen serta parameter aturan permainan; nilai null
    // diizinkan ketika data opsional belum tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; memetakan nama
    // properti JSON menjadi (”definition”).
    [property: JsonPropertyName("definition")] RulesetDefinitionDto? Definition = null);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `DefaultRulesetComponentsResponse`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed record DefaultRulesetComponentsResponse(
    // Parameter `Items` bertipe `List<DefaultRulesetComponentItem>` membawa nilai elemen; memetakan nama properti JSON menjadi (”items”).
    [property: JsonPropertyName("items")] List<DefaultRulesetComponentItem> Items);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `EventRequest`; sealed mencegah tipe ini diturunkan lagi.
public sealed record EventRequest(
    // Parameter `EventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; memetakan nama properti JSON menjadi
    // (”event_id”).
    [property: JsonPropertyName("event_id")] Guid EventId,
    // Parameter `SessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; memetakan nama properti JSON
    // menjadi (”session_id”).
    [property: JsonPropertyName("session_id")] Guid SessionId,
    // Parameter `UserId` bertipe `Guid?` membawa identitas akun pengguna yang datanya sedang diproses; nilai null diizinkan ketika data opsional belum
    // tersedia; memetakan nama properti JSON menjadi (”user_id”).
    [property: JsonPropertyName("user_id")] Guid? UserId,
    // Parameter `ActorType` bertipe `string` membawa nilai actor jenis; memetakan nama properti JSON menjadi (”actor_type”).
    [property: JsonPropertyName("actor_type")] string ActorType,
    // Parameter `Timestamp` bertipe `DateTimeOffset` membawa waktu kejadian yang menjaga urutan kronologis data; memetakan nama properti JSON menjadi
    // (”timestamp”).
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    // Parameter `DayIndex` bertipe `int` membawa nilai hari index; memetakan nama properti JSON menjadi (”day_index”).
    [property: JsonPropertyName("day_index")] int DayIndex,
    // Parameter `Weekday` bertipe `string` membawa nilai weekday; memetakan nama properti JSON menjadi (”weekday”).
    [property: JsonPropertyName("weekday")] string Weekday,
    // Parameter `ActionSlot` bertipe `int` membawa nilai aksi slot; memetakan nama properti JSON menjadi (”action_slot”).
    [property: JsonPropertyName("action_slot")] int ActionSlot,
    // Parameter `SequenceNumber` bertipe `long` membawa nomor urut event yang menentukan urutan pemrosesan riwayat permainan; memetakan nama properti
    // JSON menjadi (”sequence_number”).
    [property: JsonPropertyName("sequence_number")] long SequenceNumber,
    // Parameter `ActionType` bertipe `string` membawa nilai aksi jenis; memetakan nama properti JSON menjadi (”action_type”).
    [property: JsonPropertyName("action_type")] string ActionType,
    // Parameter `RulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; memetakan
    // nama properti JSON menjadi (”ruleset_version_id”).
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    // Parameter `Payload` bertipe `JsonElement` membawa muatan detail event dalam format JSON; memetakan nama properti JSON menjadi (”payload”).
    [property: JsonPropertyName("payload")] JsonElement Payload,
    // Parameter `ClientRequestId` bertipe `string?` membawa identitas permintaan dari klien untuk pelacakan atau penanganan permintaan berulang; nilai
    // null diizinkan ketika data opsional belum tersedia; memetakan nama properti JSON menjadi (”client_request_id”).
    [property: JsonPropertyName("client_request_id")] string? ClientRequestId,
    // Parameter `TurnNumber` bertipe `int` membawa nilai giliran number; bila argumen tidak diberikan digunakan nilai literal `0`; memetakan nama
    // properti JSON menjadi (”turn_number”).
    [property: JsonPropertyName("turn_number")] int TurnNumber = 0);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `EventStoredResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record EventStoredResponse(
    // Parameter `Stored` bertipe `bool` membawa nilai stored; memetakan nama properti JSON menjadi (”stored”).
    [property: JsonPropertyName("stored")] bool Stored,
    // Parameter `EventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; memetakan nama properti JSON menjadi
    // (”event_id”).
    [property: JsonPropertyName("event_id")] Guid EventId);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `EventBatchRequest`; sealed mencegah tipe ini diturunkan lagi.
public sealed record EventBatchRequest([property: JsonPropertyName("events")] List<EventRequest> Events);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `EventBatchFailed`; sealed mencegah tipe ini diturunkan lagi.
public sealed record EventBatchFailed(
    // Parameter `EventId` bertipe `Guid` membawa identitas unik event untuk pencatatan dan pemeriksaan duplikasi; memetakan nama properti JSON menjadi
    // (”event_id”).
    [property: JsonPropertyName("event_id")] Guid EventId,
    // Parameter `ErrorCode` bertipe `string` membawa nilai kesalahan kode; memetakan nama properti JSON menjadi (”error_code”).
    [property: JsonPropertyName("error_code")] string ErrorCode);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `EventBatchResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record EventBatchResponse(
    // Parameter `StoredCount` bertipe `int` membawa nilai stored jumlah; memetakan nama properti JSON menjadi (”stored_count”).
    [property: JsonPropertyName("stored_count")] int StoredCount,
    // Parameter `Failed` bertipe `List<EventBatchFailed>` membawa nilai failed; memetakan nama properti JSON menjadi (”failed”).
    [property: JsonPropertyName("failed")] List<EventBatchFailed> Failed);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `EventsBySessionResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record EventsBySessionResponse(
    // Parameter `SessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; memetakan nama properti JSON
    // menjadi (”session_id”).
    [property: JsonPropertyName("session_id")] Guid SessionId,
    // Parameter `Items` bertipe `List<EventRequest>` membawa nilai elemen; memetakan nama properti JSON menjadi (”items”).
    [property: JsonPropertyName("items")] List<EventRequest> Items,
    // Parameter `NextCursor` bertipe `string?` membawa nilai next cursor; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama
    // properti JSON menjadi (”next_cursor”).
    [property: JsonPropertyName("next_cursor")] string? NextCursor,
    // Parameter `HasMore` bertipe `bool` membawa nilai memiliki more; memetakan nama properti JSON menjadi (”has_more”).
    [property: JsonPropertyName("has_more")] bool HasMore);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsSessionSummary`; sealed mencegah tipe ini diturunkan lagi.
public sealed record AnalyticsSessionSummary(
    // Parameter `EventCount` bertipe `int` membawa nilai event jumlah; memetakan nama properti JSON menjadi (”event_count”).
    [property: JsonPropertyName("event_count")] int EventCount,
    // Parameter `CashInTotal` bertipe `double` membawa jumlah seluruh pemasukan arus kas; memetakan nama properti JSON menjadi (”cash_in_total”).
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    // Parameter `CashOutTotal` bertipe `double` membawa jumlah seluruh pengeluaran arus kas; memetakan nama properti JSON menjadi (”cash_out_total”).
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal,
    // Parameter `CashflowNetTotal` bertipe `double` membawa selisih pemasukan terhadap pengeluaran arus kas; memetakan nama properti JSON menjadi
    // (”cashflow_net_total”).
    [property: JsonPropertyName("cashflow_net_total")] double CashflowNetTotal);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsByPlayerItem`; sealed mencegah tipe ini diturunkan lagi.
public sealed record AnalyticsByPlayerItem(
    // Parameter `UserId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; memetakan nama properti JSON menjadi (”user_id”).
    [property: JsonPropertyName("user_id")] Guid UserId,
    // Parameter `PlayerOrder` bertipe `int` membawa nomor urut pemain untuk menentukan urutan tindakan; memetakan nama properti JSON menjadi
    // (”player_order_no”).
    [property: JsonPropertyName("player_order_no")] int PlayerOrder,
    // Parameter `CashInTotal` bertipe `double` membawa jumlah seluruh pemasukan arus kas; memetakan nama properti JSON menjadi (”cash_in_total”).
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    // Parameter `CashOutTotal` bertipe `double` membawa jumlah seluruh pengeluaran arus kas; memetakan nama properti JSON menjadi (”cash_out_total”).
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal,
    // Parameter `DonationTotal` bertipe `double` membawa nilai donasi total; memetakan nama properti JSON menjadi (”donation_total”).
    [property: JsonPropertyName("donation_total")] double DonationTotal,
    // Parameter `GoldQty` bertipe `int` membawa nilai emas qty; memetakan nama properti JSON menjadi (”gold_qty”).
    [property: JsonPropertyName("gold_qty")] int GoldQty,
    // Parameter `OrdersCompletedCount` bertipe `int` membawa nilai pesanan selesai jumlah; memetakan nama properti JSON menjadi
    // (”orders_completed_count”).
    [property: JsonPropertyName("orders_completed_count")] int OrdersCompletedCount,
    // Parameter `InventoryIngredientTotal` bertipe `int` membawa nilai inventory bahan total; memetakan nama properti JSON menjadi
    // (”inventory_ingredient_total”).
    [property: JsonPropertyName("inventory_ingredient_total")] int InventoryIngredientTotal,
    // Parameter `ActionsUsedTotal` bertipe `int` membawa nilai aksi used total; memetakan nama properti JSON menjadi (”actions_used_total”).
    [property: JsonPropertyName("actions_used_total")] int ActionsUsedTotal,
    // Parameter `FulfillmentDiversity` bertipe `double` membawa tingkat keberagaman kategori kebutuhan yang telah dipenuhi; memetakan nama properti
    // JSON menjadi (”fulfillment_diversity”).
    [property: JsonPropertyName("fulfillment_diversity")] double FulfillmentDiversity,
    // Parameter `HappinessPointsTotal` bertipe `double` membawa akumulasi poin kebahagiaan pemain; memetakan nama properti JSON menjadi
    // (”happiness_points_total”).
    [property: JsonPropertyName("happiness_points_total")] double HappinessPointsTotal,
    // Parameter `NeedPointsTotal` bertipe `double` membawa nilai kebutuhan poin total; memetakan nama properti JSON menjadi (”need_points_total”).
    [property: JsonPropertyName("need_points_total")] double NeedPointsTotal,
    // Parameter `NeedSetBonusPoints` bertipe `double` membawa nilai kebutuhan set bonus poin; memetakan nama properti JSON menjadi
    // (”need_set_bonus_points”).
    [property: JsonPropertyName("need_set_bonus_points")] double NeedSetBonusPoints,
    // Parameter `DonationPointsTotal` bertipe `double` membawa nilai donasi poin total; memetakan nama properti JSON menjadi (”donation_points_total”).
    [property: JsonPropertyName("donation_points_total")] double DonationPointsTotal,
    // Parameter `GoldPointsTotal` bertipe `double` membawa nilai emas poin total; memetakan nama properti JSON menjadi (”gold_points_total”).
    [property: JsonPropertyName("gold_points_total")] double GoldPointsTotal,
    // Parameter `PensionPointsTotal` bertipe `double` membawa nilai pension poin total; memetakan nama properti JSON menjadi (”pension_points_total”).
    [property: JsonPropertyName("pension_points_total")] double PensionPointsTotal,
    // Parameter `SavingGoalPointsTotal` bertipe `double` membawa nilai tabungan target poin total; memetakan nama properti JSON menjadi
    // (”saving_goal_points_total”).
    [property: JsonPropertyName("saving_goal_points_total")] double SavingGoalPointsTotal,
    // Parameter `MissionPenaltyTotal` bertipe `double` membawa nilai misi penalti total; memetakan nama properti JSON menjadi
    // (”mission_penalty_total”).
    [property: JsonPropertyName("mission_penalty_total")] double MissionPenaltyTotal,
    // Parameter `LoanPenaltyTotal` bertipe `double` membawa nilai pinjaman penalti total; memetakan nama properti JSON menjadi (”loan_penalty_total”).
    [property: JsonPropertyName("loan_penalty_total")] double LoanPenaltyTotal,
    // Parameter `HasUnpaidLoan` bertipe `bool` membawa nilai memiliki unpaid pinjaman; memetakan nama properti JSON menjadi (”has_unpaid_loan”).
    [property: JsonPropertyName("has_unpaid_loan")] bool HasUnpaidLoan);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsLeaderboardItem`; sealed mencegah tipe ini diturunkan lagi.
public sealed record AnalyticsLeaderboardItem(
    // Parameter `UserId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; memetakan nama properti JSON menjadi (”user_id”).
    [property: JsonPropertyName("user_id")] Guid UserId,
    // Parameter `PlayerOrder` bertipe `int` membawa nomor urut pemain untuk menentukan urutan tindakan; memetakan nama properti JSON menjadi
    // (”player_order_no”).
    [property: JsonPropertyName("player_order_no")] int PlayerOrder,
    // Parameter `Rank` bertipe `int` membawa nilai rank; memetakan nama properti JSON menjadi (”rank”).
    [property: JsonPropertyName("rank")] int Rank,
    // Parameter `HappinessPointsTotal` bertipe `double` membawa akumulasi poin kebahagiaan pemain; memetakan nama properti JSON menjadi
    // (”happiness_points_total”).
    [property: JsonPropertyName("happiness_points_total")] double HappinessPointsTotal);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `AnalyticsSessionResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record AnalyticsSessionResponse(
    // Parameter `SessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; memetakan nama properti JSON
    // menjadi (”session_id”).
    [property: JsonPropertyName("session_id")] Guid SessionId,
    // Parameter `Summary` bertipe `AnalyticsSessionSummary` membawa nilai summary; memetakan nama properti JSON menjadi (”summary”).
    [property: JsonPropertyName("summary")] AnalyticsSessionSummary Summary,
    // Parameter `ByPlayer` bertipe `List<AnalyticsByPlayerItem>` membawa nilai berdasarkan pemain; memetakan nama properti JSON menjadi (”by_player”).
    [property: JsonPropertyName("by_player")] List<AnalyticsByPlayerItem> ByPlayer,
    // Parameter `RulesetId` bertipe `Guid?` membawa identitas kumpulan aturan permainan; nilai null diizinkan ketika data opsional belum tersedia;
    // memetakan nama properti JSON menjadi (”ruleset_id”).
    [property: JsonPropertyName("ruleset_id")] Guid? RulesetId,
    // Parameter `RulesetName` bertipe `string?` membawa nilai aturan nama; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama
    // properti JSON menjadi (”ruleset_name”).
    [property: JsonPropertyName("ruleset_name")] string? RulesetName,
    // Parameter `Leaderboard` bertipe `List<AnalyticsLeaderboardItem>?` membawa nilai leaderboard; nilai null diizinkan ketika data opsional belum
    // tersedia; bila argumen tidak diberikan digunakan null, yaitu penanda tidak ada nilai; memetakan nama properti JSON menjadi (”leaderboard”).
    [property: JsonPropertyName("leaderboard")] List<AnalyticsLeaderboardItem>? Leaderboard = null);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `GameplayMetricsResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record GameplayMetricsResponse(
    // Parameter `SessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; memetakan nama properti JSON
    // menjadi (”session_id”).
    [property: JsonPropertyName("session_id")] Guid SessionId,
    // Parameter `UserId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; memetakan nama properti JSON menjadi (”user_id”).
    [property: JsonPropertyName("user_id")] Guid UserId,
    // Parameter `ComputedAt` bertipe `DateTimeOffset?` membawa nilai computed at; nilai null diizinkan ketika data opsional belum tersedia; memetakan
    // nama properti JSON menjadi (”computed_at”).
    [property: JsonPropertyName("computed_at")] DateTimeOffset? ComputedAt,
    // Parameter `Economy` bertipe `GameplayEconomyMetrics` membawa nilai economy; memetakan nama properti JSON menjadi (”economy”).
    [property: JsonPropertyName("economy")] GameplayEconomyMetrics Economy,
    // Parameter `Progress` bertipe `GameplayProgressMetrics` membawa nilai progress; memetakan nama properti JSON menjadi (”progress”).
    [property: JsonPropertyName("progress")] GameplayProgressMetrics Progress,
    // Parameter `Score` bertipe `GameplayScoreMetrics` membawa nilai skor; memetakan nama properti JSON menjadi (”score”).
    [property: JsonPropertyName("score")] GameplayScoreMetrics Score,
    // Parameter `Needs` bertipe `GameplayNeedMetrics` membawa nilai kebutuhan; memetakan nama properti JSON menjadi (”needs”).
    [property: JsonPropertyName("needs")] GameplayNeedMetrics Needs,
    // Parameter `RawJson` bertipe `JsonElement?` membawa nilai raw JSON; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak
    // diberikan digunakan null, yaitu penanda tidak ada nilai; memetakan nama properti JSON menjadi (”raw_json”).
    [property: JsonPropertyName("raw_json")] JsonElement? RawJson = null,
    // Parameter `DerivedJson` bertipe `JsonElement?` membawa nilai derived JSON; nilai null diizinkan ketika data opsional belum tersedia; bila argumen
    // tidak diberikan digunakan null, yaitu penanda tidak ada nilai; memetakan nama properti JSON menjadi (”derived_json”).
    [property: JsonPropertyName("derived_json")] JsonElement? DerivedJson = null);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `GameplayEconomyMetrics`; sealed mencegah tipe ini diturunkan lagi.
public sealed record GameplayEconomyMetrics(
    // Parameter `StartingCash` bertipe `double` membawa nilai starting uang tunai; memetakan nama properti JSON menjadi (”starting_cash”).
    [property: JsonPropertyName("starting_cash")] double StartingCash,
    // Parameter `CashInTotal` bertipe `double` membawa jumlah seluruh pemasukan arus kas; memetakan nama properti JSON menjadi (”cash_in_total”).
    [property: JsonPropertyName("cash_in_total")] double CashInTotal,
    // Parameter `CashOutTotal` bertipe `double` membawa jumlah seluruh pengeluaran arus kas; memetakan nama properti JSON menjadi (”cash_out_total”).
    [property: JsonPropertyName("cash_out_total")] double CashOutTotal,
    // Parameter `CashflowNetTotal` bertipe `double` membawa selisih pemasukan terhadap pengeluaran arus kas; memetakan nama properti JSON menjadi
    // (”cashflow_net_total”).
    [property: JsonPropertyName("cashflow_net_total")] double CashflowNetTotal,
    // Parameter `DonationTotal` bertipe `double` membawa nilai donasi total; memetakan nama properti JSON menjadi (”donation_total”).
    [property: JsonPropertyName("donation_total")] double DonationTotal);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `GameplayProgressMetrics`; sealed mencegah tipe ini diturunkan lagi.
public sealed record GameplayProgressMetrics(
    // Parameter `GoldQty` bertipe `int` membawa nilai emas qty; memetakan nama properti JSON menjadi (”gold_qty”).
    [property: JsonPropertyName("gold_qty")] int GoldQty,
    // Parameter `OrdersCompletedCount` bertipe `int` membawa nilai pesanan selesai jumlah; memetakan nama properti JSON menjadi
    // (”orders_completed_count”).
    [property: JsonPropertyName("orders_completed_count")] int OrdersCompletedCount,
    // Parameter `InventoryIngredientTotal` bertipe `int` membawa nilai inventory bahan total; memetakan nama properti JSON menjadi
    // (”inventory_ingredient_total”).
    [property: JsonPropertyName("inventory_ingredient_total")] int InventoryIngredientTotal,
    // Parameter `ActionsUsedTotal` bertipe `int` membawa nilai aksi used total; memetakan nama properti JSON menjadi (”actions_used_total”).
    [property: JsonPropertyName("actions_used_total")] int ActionsUsedTotal);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `GameplayScoreMetrics`; sealed mencegah tipe ini diturunkan lagi.
public sealed record GameplayScoreMetrics(
    // Parameter `HappinessPointsTotal` bertipe `double` membawa akumulasi poin kebahagiaan pemain; memetakan nama properti JSON menjadi
    // (”happiness_points_total”).
    [property: JsonPropertyName("happiness_points_total")] double HappinessPointsTotal,
    // Parameter `NeedPointsTotal` bertipe `double` membawa nilai kebutuhan poin total; memetakan nama properti JSON menjadi (”need_points_total”).
    [property: JsonPropertyName("need_points_total")] double NeedPointsTotal,
    // Parameter `NeedSetBonusPoints` bertipe `double` membawa nilai kebutuhan set bonus poin; memetakan nama properti JSON menjadi
    // (”need_set_bonus_points”).
    [property: JsonPropertyName("need_set_bonus_points")] double NeedSetBonusPoints,
    // Parameter `DonationPointsTotal` bertipe `double` membawa nilai donasi poin total; memetakan nama properti JSON menjadi (”donation_points_total”).
    [property: JsonPropertyName("donation_points_total")] double DonationPointsTotal,
    // Parameter `GoldPointsTotal` bertipe `double` membawa nilai emas poin total; memetakan nama properti JSON menjadi (”gold_points_total”).
    [property: JsonPropertyName("gold_points_total")] double GoldPointsTotal,
    // Parameter `PensionPointsTotal` bertipe `double` membawa nilai pension poin total; memetakan nama properti JSON menjadi (”pension_points_total”).
    [property: JsonPropertyName("pension_points_total")] double PensionPointsTotal,
    // Parameter `SavingGoalPointsTotal` bertipe `double` membawa nilai tabungan target poin total; memetakan nama properti JSON menjadi
    // (”saving_goal_points_total”).
    [property: JsonPropertyName("saving_goal_points_total")] double SavingGoalPointsTotal,
    // Parameter `MissionPenaltyTotal` bertipe `double` membawa nilai misi penalti total; memetakan nama properti JSON menjadi
    // (”mission_penalty_total”).
    [property: JsonPropertyName("mission_penalty_total")] double MissionPenaltyTotal,
    // Parameter `LoanPenaltyTotal` bertipe `double` membawa nilai pinjaman penalti total; memetakan nama properti JSON menjadi (”loan_penalty_total”).
    [property: JsonPropertyName("loan_penalty_total")] double LoanPenaltyTotal,
    // Parameter `HasUnpaidLoan` bertipe `bool` membawa nilai memiliki unpaid pinjaman; memetakan nama properti JSON menjadi (”has_unpaid_loan”).
    [property: JsonPropertyName("has_unpaid_loan")] bool HasUnpaidLoan);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `GameplayNeedMetrics`; sealed mencegah tipe ini diturunkan lagi.
public sealed record GameplayNeedMetrics(
    // Parameter `FulfillmentDiversity` bertipe `double` membawa tingkat keberagaman kategori kebutuhan yang telah dipenuhi; memetakan nama properti
    // JSON menjadi (”fulfillment_diversity”).
    [property: JsonPropertyName("fulfillment_diversity")] double FulfillmentDiversity);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `TransactionHistoryItem`; sealed mencegah tipe ini diturunkan lagi.
public sealed record TransactionHistoryItem(
    // Parameter `TransactionId` bertipe `Guid` membawa nilai transaction identitas; memetakan nama properti JSON menjadi (”transaction_id”).
    [property: JsonPropertyName("transaction_id")] Guid TransactionId,
    // Parameter `Timestamp` bertipe `DateTimeOffset` membawa waktu kejadian yang menjaga urutan kronologis data; memetakan nama properti JSON menjadi
    // (”timestamp”).
    [property: JsonPropertyName("timestamp")] DateTimeOffset Timestamp,
    // Parameter `Direction` bertipe `string` membawa nilai direction; memetakan nama properti JSON menjadi (”direction”).
    [property: JsonPropertyName("direction")] string Direction,
    // Parameter `Amount` bertipe `double` membawa nominal uang atau nilai transaksi yang dipakai dalam operasi; memetakan nama properti JSON menjadi
    // (”amount”).
    [property: JsonPropertyName("amount")] double Amount,
    // Parameter `Category` bertipe `string` membawa nilai category; memetakan nama properti JSON menjadi (”category”).
    [property: JsonPropertyName("category")] string Category);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `TransactionHistoryResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record TransactionHistoryResponse(
    // Parameter `Items` bertipe `List<TransactionHistoryItem>` membawa nilai elemen; memetakan nama properti JSON menjadi (”items”).
    [property: JsonPropertyName("items")] List<TransactionHistoryItem> Items,
    // Parameter `NextCursor` bertipe `string?` membawa nilai next cursor; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama
    // properti JSON menjadi (”next_cursor”).
    [property: JsonPropertyName("next_cursor")] string? NextCursor,
    // Parameter `HasMore` bertipe `bool` membawa nilai memiliki more; memetakan nama properti JSON menjadi (”has_more”).
    [property: JsonPropertyName("has_more")] bool HasMore);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RulesetAnalyticsPlayerItem`; sealed mencegah tipe ini diturunkan lagi.
public sealed record RulesetAnalyticsPlayerItem(
    // Parameter `UserId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; memetakan nama properti JSON menjadi (”user_id”).
    [property: JsonPropertyName("user_id")] Guid UserId,
    // Parameter `LearningPerformanceIndividualScore` bertipe `double?` membawa nilai pembelajaran performa individual skor; nilai null diizinkan ketika
    // data opsional belum tersedia; memetakan nama properti JSON menjadi (”learning_performance_individual_score”).
    [property: JsonPropertyName("learning_performance_individual_score")] double? LearningPerformanceIndividualScore,
    // Parameter `MissionPerformanceIndividualScore` bertipe `double?` membawa nilai misi performa individual skor; nilai null diizinkan ketika data
    // opsional belum tersedia; memetakan nama properti JSON menjadi (”mission_performance_individual_score”).
    [property: JsonPropertyName("mission_performance_individual_score")] double? MissionPerformanceIndividualScore);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RulesetAnalyticsSessionItem`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed record RulesetAnalyticsSessionItem(
    // Parameter `SessionId` bertipe `Guid` membawa identitas unik sesi permainan yang menjadi batas data operasi ini; memetakan nama properti JSON
    // menjadi (”session_id”).
    [property: JsonPropertyName("session_id")] Guid SessionId,
    // Parameter `SessionName` bertipe `string` membawa nilai sesi nama; memetakan nama properti JSON menjadi (”session_name”).
    [property: JsonPropertyName("session_name")] string SessionName,
    // Parameter `Status` bertipe `string` membawa nilai status; memetakan nama properti JSON menjadi (”status”).
    [property: JsonPropertyName("status")] string Status,
    // Parameter `EventCount` bertipe `int` membawa nilai event jumlah; memetakan nama properti JSON menjadi (”event_count”).
    [property: JsonPropertyName("event_count")] int EventCount,
    // Parameter `LearningPerformanceAggregateScore` bertipe `double?` membawa nilai pembelajaran performa aggregate skor; nilai null diizinkan ketika
    // data opsional belum tersedia; memetakan nama properti JSON menjadi (”learning_performance_aggregate_score”).
    [property: JsonPropertyName("learning_performance_aggregate_score")] double? LearningPerformanceAggregateScore,
    // Parameter `MissionPerformanceAggregateScore` bertipe `double?` membawa nilai misi performa aggregate skor; nilai null diizinkan ketika data
    // opsional belum tersedia; memetakan nama properti JSON menjadi (”mission_performance_aggregate_score”).
    [property: JsonPropertyName("mission_performance_aggregate_score")] double? MissionPerformanceAggregateScore,
    // Parameter `Players` bertipe `List<RulesetAnalyticsPlayerItem>` membawa nilai pemain; memetakan nama properti JSON menjadi (”players”).
    [property: JsonPropertyName("players")] List<RulesetAnalyticsPlayerItem> Players);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RulesetAnalyticsSummaryResponse`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed record RulesetAnalyticsSummaryResponse(
    // Parameter `RulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan; memetakan nama properti JSON menjadi (”ruleset_id”).
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    // Parameter `RulesetName` bertipe `string` membawa nilai aturan nama; memetakan nama properti JSON menjadi (”ruleset_name”).
    [property: JsonPropertyName("ruleset_name")] string RulesetName,
    // Parameter `SessionCount` bertipe `int` membawa nilai sesi jumlah; memetakan nama properti JSON menjadi (”session_count”).
    [property: JsonPropertyName("session_count")] int SessionCount,
    // Parameter `LearningPerformanceAggregateScore` bertipe `double?` membawa nilai pembelajaran performa aggregate skor; nilai null diizinkan ketika
    // data opsional belum tersedia; memetakan nama properti JSON menjadi (”learning_performance_aggregate_score”).
    [property: JsonPropertyName("learning_performance_aggregate_score")] double? LearningPerformanceAggregateScore,
    // Parameter `MissionPerformanceAggregateScore` bertipe `double?` membawa nilai misi performa aggregate skor; nilai null diizinkan ketika data
    // opsional belum tersedia; memetakan nama properti JSON menjadi (”mission_performance_aggregate_score”).
    [property: JsonPropertyName("mission_performance_aggregate_score")] double? MissionPerformanceAggregateScore,
    // Parameter `Sessions` bertipe `List<RulesetAnalyticsSessionItem>` membawa nilai sessions; memetakan nama properti JSON menjadi (”sessions”).
    [property: JsonPropertyName("sessions")] List<RulesetAnalyticsSessionItem> Sessions);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `LoginRequest`; sealed mencegah tipe ini diturunkan lagi.
public sealed record LoginRequest(
    // Parameter `Username` bertipe `string` membawa nama akun yang dipakai saat autentikasi; memetakan nama properti JSON menjadi (”username”).
    [property: JsonPropertyName("username")] string Username,
    // Parameter `Password` bertipe `string` membawa kata sandi masukan yang diperiksa sesuai kebijakan autentikasi; memetakan nama properti JSON
    // menjadi (”password”).
    [property: JsonPropertyName("password")] string Password);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `LoginResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record LoginResponse(
    // Parameter `UserId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; memetakan nama properti JSON menjadi (”user_id”).
    [property: JsonPropertyName("user_id")] Guid UserId,
    // Parameter `Username` bertipe `string` membawa nama akun yang dipakai saat autentikasi; memetakan nama properti JSON menjadi (”username”).
    [property: JsonPropertyName("username")] string Username,
    // Parameter `Role` bertipe `string` membawa peran pengguna yang menentukan hak akses; memetakan nama properti JSON menjadi (”role”).
    [property: JsonPropertyName("role")] string Role,
    // Parameter `DisplayName` bertipe `string?` membawa nilai display nama; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama
    // properti JSON menjadi (”display_name”).
    [property: JsonPropertyName("display_name")] string? DisplayName,
    // Parameter `AccessToken` bertipe `string` membawa nilai akses token; memetakan nama properti JSON menjadi (”access_token”).
    [property: JsonPropertyName("access_token")] string AccessToken,
    // Parameter `ExpiresAt` bertipe `DateTimeOffset` membawa nilai expires at; memetakan nama properti JSON menjadi (”expires_at”).
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RegisterRequest`; sealed mencegah tipe ini diturunkan lagi.
public sealed record RegisterRequest(
    // Parameter `Username` bertipe `string` membawa nama akun yang dipakai saat autentikasi; memetakan nama properti JSON menjadi (”username”).
    [property: JsonPropertyName("username")] string Username,
    // Parameter `Password` bertipe `string` membawa kata sandi masukan yang diperiksa sesuai kebijakan autentikasi; memetakan nama properti JSON
    // menjadi (”password”).
    [property: JsonPropertyName("password")] string Password,
    // Parameter `Role` bertipe `string` membawa peran pengguna yang menentukan hak akses; memetakan nama properti JSON menjadi (”role”).
    [property: JsonPropertyName("role")] string Role,
    // Parameter `DisplayName` bertipe `string?` membawa nilai display nama; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama
    // properti JSON menjadi (”display_name”).
    [property: JsonPropertyName("display_name")] string? DisplayName);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RegisterResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record RegisterResponse(
    // Parameter `UserId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; memetakan nama properti JSON menjadi (”user_id”).
    [property: JsonPropertyName("user_id")] Guid UserId,
    // Parameter `Username` bertipe `string` membawa nama akun yang dipakai saat autentikasi; memetakan nama properti JSON menjadi (”username”).
    [property: JsonPropertyName("username")] string Username,
    // Parameter `Role` bertipe `string` membawa peran pengguna yang menentukan hak akses; memetakan nama properti JSON menjadi (”role”).
    [property: JsonPropertyName("role")] string Role,
    // Parameter `DisplayName` bertipe `string?` membawa nilai display nama; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama
    // properti JSON menjadi (”display_name”).
    [property: JsonPropertyName("display_name")] string? DisplayName,
    // Parameter `AccessToken` bertipe `string` membawa nilai akses token; memetakan nama properti JSON menjadi (”access_token”).
    [property: JsonPropertyName("access_token")] string AccessToken,
    // Parameter `ExpiresAt` bertipe `DateTimeOffset` membawa nilai expires at; memetakan nama properti JSON menjadi (”expires_at”).
    [property: JsonPropertyName("expires_at")] DateTimeOffset ExpiresAt);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `SecurityAuditLogItem`; sealed mencegah tipe ini diturunkan lagi.
public sealed record SecurityAuditLogItem(
    // Parameter `SecurityAuditLogId` bertipe `Guid` membawa nilai security audit log identitas; memetakan nama properti JSON menjadi
    // (”security_audit_log_id”).
    [property: JsonPropertyName("security_audit_log_id")] Guid SecurityAuditLogId,
    // Parameter `OccurredAt` bertipe `DateTimeOffset` membawa nilai occurred at; memetakan nama properti JSON menjadi (”occurred_at”).
    [property: JsonPropertyName("occurred_at")] DateTimeOffset OccurredAt,
    // Parameter `TraceId` bertipe `string` membawa identitas penelusuran yang menghubungkan respons, log, dan permintaan yang sama; memetakan nama
    // properti JSON menjadi (”trace_id”).
    [property: JsonPropertyName("trace_id")] string TraceId,
    // Parameter `EventType` bertipe `string` membawa jenis aktivitas yang menentukan aturan validasi dan proyeksi event; memetakan nama properti JSON
    // menjadi (”event_type”).
    [property: JsonPropertyName("event_type")] string EventType,
    // Parameter `Outcome` bertipe `string` membawa nilai hasil; memetakan nama properti JSON menjadi (”outcome”).
    [property: JsonPropertyName("outcome")] string Outcome,
    // Parameter `UserId` bertipe `Guid?` membawa identitas akun pengguna yang datanya sedang diproses; nilai null diizinkan ketika data opsional belum
    // tersedia; memetakan nama properti JSON menjadi (”user_id”).
    [property: JsonPropertyName("user_id")] Guid? UserId,
    // Parameter `Username` bertipe `string?` membawa nama akun yang dipakai saat autentikasi; nilai null diizinkan ketika data opsional belum tersedia;
    // memetakan nama properti JSON menjadi (”username”).
    [property: JsonPropertyName("username")] string? Username,
    // Parameter `Role` bertipe `string?` membawa peran pengguna yang menentukan hak akses; nilai null diizinkan ketika data opsional belum tersedia;
    // memetakan nama properti JSON menjadi (”role”).
    [property: JsonPropertyName("role")] string? Role,
    // Parameter `IpAddress` bertipe `string?` membawa nilai ip address; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama
    // properti JSON menjadi (”ip_address”).
    [property: JsonPropertyName("ip_address")] string? IpAddress,
    // Parameter `UserAgent` bertipe `string?` membawa nilai pengguna agent; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama
    // properti JSON menjadi (”user_agent”).
    [property: JsonPropertyName("user_agent")] string? UserAgent,
    // Parameter `Method` bertipe `string` membawa nilai method; memetakan nama properti JSON menjadi (”method”).
    [property: JsonPropertyName("method")] string Method,
    // Parameter `Path` bertipe `string` membawa nilai path; memetakan nama properti JSON menjadi (”path”).
    [property: JsonPropertyName("path")] string Path,
    // Parameter `StatusCode` bertipe `int` membawa kode status hasil HTTP yang mengomunikasikan keberhasilan atau kegagalan; memetakan nama properti
    // JSON menjadi (”status_code”).
    [property: JsonPropertyName("status_code")] int StatusCode,
    // Parameter `Detail` bertipe `JsonElement?` membawa nilai detail; nilai null diizinkan ketika data opsional belum tersedia; memetakan nama properti
    // JSON menjadi (”detail”).
    [property: JsonPropertyName("detail")] JsonElement? Detail);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `SecurityAuditLogResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record SecurityAuditLogResponse(
    // Parameter `Items` bertipe `List<SecurityAuditLogItem>` membawa nilai elemen; memetakan nama properti JSON menjadi (”items”).
    [property: JsonPropertyName("items")] List<SecurityAuditLogItem> Items);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RulesetSectionCatalogResponse`; sealed mencegah tipe ini diturunkan
// lagi.
public sealed record RulesetSectionCatalogResponse(
    // Parameter `GameConfig` bertipe `JsonElement` membawa nilai game konfigurasi; memetakan nama properti JSON menjadi (”gameConfig”).
    [property: JsonPropertyName("gameConfig")] JsonElement GameConfig,
    // Parameter `Bahan` bertipe `JsonElement` membawa nilai bahan; memetakan nama properti JSON menjadi (”bahan”).
    [property: JsonPropertyName("bahan")] JsonElement Bahan,
    // Parameter `Resep` bertipe `JsonElement` membawa nilai resep; memetakan nama properti JSON menjadi (”resep”).
    [property: JsonPropertyName("resep")] JsonElement Resep,
    // Parameter `Kebutuhan` bertipe `JsonElement` membawa nilai kebutuhan; memetakan nama properti JSON menjadi (”kebutuhan”).
    [property: JsonPropertyName("kebutuhan")] JsonElement Kebutuhan,
    // Parameter `TargetKebutuhan` bertipe `JsonElement` membawa nilai target kebutuhan; memetakan nama properti JSON menjadi (”targetKebutuhan”).
    [property: JsonPropertyName("targetKebutuhan")] JsonElement TargetKebutuhan,
    // Parameter `TujuanFinansial` bertipe `JsonElement` membawa nilai tujuan finansial; memetakan nama properti JSON menjadi (”tujuanFinansial”).
    [property: JsonPropertyName("tujuanFinansial")] JsonElement TujuanFinansial,
    // Parameter `Narasi` bertipe `JsonElement` membawa nilai narasi; memetakan nama properti JSON menjadi (”narasi”).
    [property: JsonPropertyName("narasi")] JsonElement Narasi);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `RulesetSectionsResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record RulesetSectionsResponse(
    // Parameter `RulesetId` bertipe `Guid` membawa identitas kumpulan aturan permainan; memetakan nama properti JSON menjadi (”ruleset_id”).
    [property: JsonPropertyName("ruleset_id")] Guid RulesetId,
    // Parameter `RulesetVersionId` bertipe `Guid` membawa identitas versi aturan sehingga perhitungan memakai konfigurasi aturan yang tepat; memetakan
    // nama properti JSON menjadi (”ruleset_version_id”).
    [property: JsonPropertyName("ruleset_version_id")] Guid RulesetVersionId,
    // Parameter `Mode` bertipe `string` membawa mode permainan yang menentukan kelompok aturan yang digunakan; memetakan nama properti JSON menjadi
    // (”mode”).
    [property: JsonPropertyName("mode")] string Mode,
    // Parameter `GameConfig` bertipe `JsonElement` membawa nilai game konfigurasi; memetakan nama properti JSON menjadi (”gameConfig”).
    [property: JsonPropertyName("gameConfig")] JsonElement GameConfig,
    // Parameter `Bahan` bertipe `JsonElement` membawa nilai bahan; memetakan nama properti JSON menjadi (”bahan”).
    [property: JsonPropertyName("bahan")] JsonElement Bahan,
    // Parameter `Resep` bertipe `JsonElement` membawa nilai resep; memetakan nama properti JSON menjadi (”resep”).
    [property: JsonPropertyName("resep")] JsonElement Resep,
    // Parameter `Kebutuhan` bertipe `JsonElement` membawa nilai kebutuhan; memetakan nama properti JSON menjadi (”kebutuhan”).
    [property: JsonPropertyName("kebutuhan")] JsonElement Kebutuhan,
    // Parameter `TargetKebutuhan` bertipe `JsonElement` membawa nilai target kebutuhan; memetakan nama properti JSON menjadi (”targetKebutuhan”).
    [property: JsonPropertyName("targetKebutuhan")] JsonElement TargetKebutuhan,
    // Parameter `TujuanFinansial` bertipe `JsonElement` membawa nilai tujuan finansial; memetakan nama properti JSON menjadi (”tujuanFinansial”).
    [property: JsonPropertyName("tujuanFinansial")] JsonElement TujuanFinansial,
    // Parameter `Narasi` bertipe `JsonElement` membawa nilai narasi; memetakan nama properti JSON menjadi (”narasi”).
    [property: JsonPropertyName("narasi")] JsonElement Narasi);

// Mendefinisikan tipe class `SaveSessionStateRequest`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SaveSessionStateRequest
// Membuka scope tipe SaveSessionStateRequest; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”state_version”).
    [JsonPropertyName("state_version")]
    // Mendefinisikan properti `StateVersion` bertipe `long` untuk nilai keadaan versi; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public long StateVersion { get; init; }

    // memetakan nama properti JSON menjadi (”day”).
    [JsonPropertyName("day")]
    // Mendefinisikan properti `Day` bertipe `int` untuk nomor hari permainan yang menjadi konteks aktivitas; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek.
    public int Day { get; init; }

    // memetakan nama properti JSON menjadi (”turn”).
    [JsonPropertyName("turn")]
    // Mendefinisikan properti `Turn` bertipe `int` untuk giliran pemain yang sedang berlangsung; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek.
    public int Turn { get; init; }

    // memetakan nama properti JSON menjadi (”action_slots_left”).
    [JsonPropertyName("action_slots_left")]
    // Mendefinisikan properti `ActionSlotsLeft` bertipe `int` untuk nilai aksi slots left; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public int ActionSlotsLeft { get; init; }

    // memetakan nama properti JSON menjadi (”finish_day”).
    [JsonPropertyName("finish_day")]
    // Mendefinisikan properti `FinishDay` bertipe `int` untuk nilai finish hari; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int FinishDay { get; init; }

    // memetakan nama properti JSON menjadi (”is_game_over”).
    [JsonPropertyName("is_game_over")]
    // Mendefinisikan properti `IsGameOver` bertipe `bool` untuk nilai berstatus game over; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public bool IsGameOver { get; init; }

    // memetakan nama properti JSON menjadi (”players”).
    [JsonPropertyName("players")]
    // Mendefinisikan properti `Players` bertipe `List<SessionPlayerStateDto>?` untuk nilai pemain; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public List<SessionPlayerStateDto>? Players { get; init; }

    // memetakan nama properti JSON menjadi (”donationEvents”).
    [JsonPropertyName("donationEvents")]
    // Mendefinisikan properti `DonationEvents` bertipe `List<DonationEventDto>?` untuk nilai donasi event; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public List<DonationEventDto>? DonationEvents { get; init; }

    // memetakan nama properti JSON menjadi (”last_action”).
    [JsonPropertyName("last_action")]
    // Mendefinisikan properti `LastAction` bertipe `JsonElement?` untuk nilai last aksi; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public JsonElement? LastAction { get; init; }
// Menutup scope tipe SaveSessionStateRequest; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `SessionStateResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionStateResponse
// Membuka scope tipe SessionStateResponse; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”session_id”).
    [JsonPropertyName("session_id")]
    // Mendefinisikan properti `SessionId` bertipe `Guid` untuk identitas unik sesi permainan yang menjadi batas data operasi ini; get menyediakan
    // pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
    public Guid SessionId { get; init; }

    // memetakan nama properti JSON menjadi (”state_version”).
    [JsonPropertyName("state_version")]
    // Mendefinisikan properti `StateVersion` bertipe `long` untuk nilai keadaan versi; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public long StateVersion { get; init; }

    // memetakan nama properti JSON menjadi (”next_sequence_number”).
    [JsonPropertyName("next_sequence_number")]
    // Mendefinisikan properti `NextSequenceNumber` bertipe `long` untuk nilai next sequence number; get menyediakan pembacaan nilai, set mengizinkan
    // penggantian nilai.
    public long NextSequenceNumber { get; set; }

    // memetakan nama properti JSON menjadi (”day”).
    [JsonPropertyName("day")]
    // Mendefinisikan properti `Day` bertipe `int` untuk nomor hari permainan yang menjadi konteks aktivitas; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek.
    public int Day { get; init; }

    // memetakan nama properti JSON menjadi (”turn”).
    [JsonPropertyName("turn")]
    // Mendefinisikan properti `Turn` bertipe `int` untuk giliran pemain yang sedang berlangsung; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek.
    public int Turn { get; init; }

    // memetakan nama properti JSON menjadi (”action_slots_left”).
    [JsonPropertyName("action_slots_left")]
    // Mendefinisikan properti `ActionSlotsLeft` bertipe `int` untuk nilai aksi slots left; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public int ActionSlotsLeft { get; init; }

    // memetakan nama properti JSON menjadi (”finish_day”).
    [JsonPropertyName("finish_day")]
    // Mendefinisikan properti `FinishDay` bertipe `int` untuk nilai finish hari; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int FinishDay { get; init; }

    // memetakan nama properti JSON menjadi (”is_game_over”).
    [JsonPropertyName("is_game_over")]
    // Mendefinisikan properti `IsGameOver` bertipe `bool` untuk nilai berstatus game over; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public bool IsGameOver { get; init; }

    // memetakan nama properti JSON menjadi (”players”).
    [JsonPropertyName("players")]
    // Mendefinisikan properti `Players` bertipe `List<SessionPlayerStateDto>` untuk nilai pemain; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<SessionPlayerStateDto> Players { get; init; } = [];

    // memetakan nama properti JSON menjadi (”donationEvents”).
    [JsonPropertyName("donationEvents")]
    // Mendefinisikan properti `DonationEvents` bertipe `List<DonationEventDto>` untuk nilai donasi event; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<DonationEventDto> DonationEvents { get; init; } = [];
// Menutup scope tipe SessionStateResponse; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `SessionPlayerStateDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class SessionPlayerStateDto
// Membuka scope tipe SessionPlayerStateDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”session_player_id”).
    [JsonPropertyName("session_player_id")]
    // Mendefinisikan properti `SessionPlayerId` bertipe `Guid` untuk identitas keikutsertaan pemain pada sesi tertentu; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek.
    public Guid SessionPlayerId { get; init; }

    // memetakan nama properti JSON menjadi (”user_id”).
    [JsonPropertyName("user_id")]
    // Mendefinisikan properti `UserId` bertipe `Guid?` untuk identitas akun pengguna yang datanya sedang diproses; get menyediakan pembacaan nilai,
    // init membatasi pengisian saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public Guid? UserId { get; init; }

    // memetakan nama properti JSON menjadi (”player_order_no”).
    [JsonPropertyName("player_order_no")]
    // Mendefinisikan properti `PlayerIndex` bertipe `int` untuk nilai pemain index; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int PlayerIndex { get; init; }

    // memetakan nama properti JSON menjadi (”name”).
    [JsonPropertyName("name")]
    // Mendefinisikan properti `Name` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Name { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”coins”).
    [JsonPropertyName("coins")]
    // Mendefinisikan properti `Coins` bertipe `int` untuk nilai coins; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek.
    public int Coins { get; init; }

    // memetakan nama properti JSON menjadi (”happiness”).
    [JsonPropertyName("happiness")]
    // Mendefinisikan properti `Happiness` bertipe `int` untuk nilai kebahagiaan; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int Happiness { get; init; }

    // memetakan nama properti JSON menjadi (”saving”).
    [JsonPropertyName("saving")]
    // Mendefinisikan properti `Saving` bertipe `int` untuk nilai tabungan; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek.
    public int Saving { get; init; }

    // memetakan nama properti JSON menjadi (”bahan”).
    [JsonPropertyName("bahan")]
    // Mendefinisikan properti `Bahan` bertipe `List<BahanItemDto>` untuk nilai bahan; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<BahanItemDto> Bahan { get; init; } = [];

    // memetakan nama properti JSON menjadi (”kebutuhan”).
    [JsonPropertyName("kebutuhan")]
    // Mendefinisikan properti `Kebutuhan` bertipe `List<KebutuhanItemDto>` untuk nilai kebutuhan; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<KebutuhanItemDto> Kebutuhan { get; init; } = [];

    // memetakan nama properti JSON menjadi (”tujuanFinansial”).
    [JsonPropertyName("tujuanFinansial")]
    // Mendefinisikan properti `TujuanFinansial` bertipe `List<TujuanFinansialItemDto>` untuk nilai tujuan finansial; get menyediakan pembacaan nilai,
    // init membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<TujuanFinansialItemDto> TujuanFinansial { get; init; } = [];

    // memetakan nama properti JSON menjadi (”targetKebutuhan”).
    [JsonPropertyName("targetKebutuhan")]
    // Mendefinisikan properti `TargetKebutuhan` bertipe `List<TargetKebutuhanProgressDto>` untuk nilai target kebutuhan; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<TargetKebutuhanProgressDto> TargetKebutuhan { get; init; } = [];

    // memetakan nama properti JSON menjadi (”actionCounters”).
    [JsonPropertyName("actionCounters")]
    // Mendefinisikan properti `ActionCounters` bertipe `List<ActionCounterDto>` untuk nilai aksi counters; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<ActionCounterDto> ActionCounters { get; init; } = [];

    // memetakan nama properti JSON menjadi (”totalDonasi”).
    [JsonPropertyName("totalDonasi")]
    // Mendefinisikan properti `TotalDonasi` bertipe `int` untuk nilai total donasi; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int TotalDonasi { get; init; }
// Menutup scope tipe SessionPlayerStateDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `BahanItemDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed record BahanItemDto(
    // Parameter `Nama` bertipe `string` membawa nilai nama; memetakan nama properti JSON menjadi (”nama”).
    [property: JsonPropertyName("nama")] string Nama,
    // Parameter `Jumlah` bertipe `int` membawa nilai jumlah; memetakan nama properti JSON menjadi (”jumlah”).
    [property: JsonPropertyName("jumlah")] int Jumlah);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `KebutuhanItemDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed record KebutuhanItemDto(
    // Parameter `Nama` bertipe `string` membawa nilai nama; memetakan nama properti JSON menjadi (”nama”).
    [property: JsonPropertyName("nama")] string Nama,
    // Parameter `Tipe` bertipe `string` membawa nilai tipe; memetakan nama properti JSON menjadi (”tipe”).
    [property: JsonPropertyName("tipe")] string Tipe);

// Mendefinisikan tipe class `TujuanFinansialItemDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class TujuanFinansialItemDto
// Membuka scope tipe TujuanFinansialItemDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”nama”).
    [JsonPropertyName("nama")]
    // Mendefinisikan properti `Nama` bertipe `string` untuk nilai nama; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya `string.Empty`, yaitu nilai kosong bawaan tipe terkait.
    public string Nama { get; init; } = string.Empty;

    // memetakan nama properti JSON menjadi (”current_amount”).
    [JsonPropertyName("current_amount")]
    // Mendefinisikan properti `CurrentAmount` bertipe `int` untuk nilai saat ini nominal; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek.
    public int CurrentAmount { get; init; }

    // memetakan nama properti JSON menjadi (”target_amount”).
    [JsonPropertyName("target_amount")]
    // Mendefinisikan properti `TargetAmount` bertipe `int?` untuk nilai target nominal; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public int? TargetAmount { get; init; }

    // memetakan nama properti JSON menjadi (”status”).
    [JsonPropertyName("status")]
    // Mendefinisikan properti `Status` bertipe `string` untuk nilai status; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek; nilai awalnya nilai literal `”ONGOING”`.
    public string Status { get; init; } = "ONGOING";

    // memetakan nama properti JSON menjadi (”purchased_at_day”).
    [JsonPropertyName("purchased_at_day")]
    // Mendefinisikan properti `PurchasedAtDay` bertipe `int?` untuk nilai dibeli at hari; get menyediakan pembacaan nilai, init membatasi pengisian
    // saat inisialisasi objek; tanda ? mengizinkan nilai null.
    public int? PurchasedAtDay { get; init; }
// Menutup scope tipe TujuanFinansialItemDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `TargetKebutuhanProgressDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed record TargetKebutuhanProgressDto(
    // Parameter `Id` bertipe `string` membawa nilai identitas; memetakan nama properti JSON menjadi (”id”).
    [property: JsonPropertyName("id")] string Id,
    // Parameter `IsCompleted` bertipe `bool` membawa nilai berstatus selesai; memetakan nama properti JSON menjadi (”is_completed”).
    [property: JsonPropertyName("is_completed")] bool IsCompleted,
    // Parameter `IsFailed` bertipe `bool` membawa nilai berstatus failed; memetakan nama properti JSON menjadi (”is_failed”).
    [property: JsonPropertyName("is_failed")] bool IsFailed,
    // Parameter `RewardApplied` bertipe `bool` membawa nilai reward applied; memetakan nama properti JSON menjadi (”reward_applied”).
    [property: JsonPropertyName("reward_applied")] bool RewardApplied);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `ActionCounterDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed record ActionCounterDto(
    // Parameter `Aksi` bertipe `string` membawa nilai aksi; memetakan nama properti JSON menjadi (”aksi”).
    [property: JsonPropertyName("aksi")] string Aksi,
    // Parameter `Count` bertipe `int` membawa nilai jumlah; memetakan nama properti JSON menjadi (”count”).
    [property: JsonPropertyName("count")] int Count);

// Mendefinisikan tipe class `DonationEventDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class DonationEventDto
// Membuka scope tipe DonationEventDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”event_ke”).
    [JsonPropertyName("event_ke")]
    // Mendefinisikan properti `EventKe` bertipe `int` untuk nilai event ke; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi
    // objek.
    public int EventKe { get; init; }

    // memetakan nama properti JSON menjadi (”day”).
    [JsonPropertyName("day")]
    // Mendefinisikan properti `Day` bertipe `int` untuk nomor hari permainan yang menjadi konteks aktivitas; get menyediakan pembacaan nilai, init
    // membatasi pengisian saat inisialisasi objek.
    public int Day { get; init; }

    // memetakan nama properti JSON menjadi (”rankings”).
    [JsonPropertyName("rankings")]
    // Mendefinisikan properti `Rankings` bertipe `List<DonationRankingDto>` untuk nilai rankings; get menyediakan pembacaan nilai, init membatasi
    // pengisian saat inisialisasi objek; nilai awalnya koleksi kosong dengan tipe mengikuti konteks tujuan.
    public List<DonationRankingDto> Rankings { get; init; } = [];
// Menutup scope tipe DonationEventDto; bagian berikut berada di luar batas blok tersebut.
}

// Mendefinisikan tipe class `DonationRankingDto`; sealed mencegah tipe ini diturunkan lagi.
public sealed class DonationRankingDto
// Membuka scope tipe DonationRankingDto; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // memetakan nama properti JSON menjadi (”rank”).
    [JsonPropertyName("rank")]
    // Mendefinisikan properti `Rank` bertipe `int` untuk nilai rank; get menyediakan pembacaan nilai, init membatasi pengisian saat inisialisasi objek.
    public int Rank { get; init; }

    // memetakan nama properti JSON menjadi (”session_player_id”).
    [JsonPropertyName("session_player_id")]
    // Mendefinisikan properti `SessionPlayerId` bertipe `Guid` untuk identitas keikutsertaan pemain pada sesi tertentu; get menyediakan pembacaan
    // nilai, init membatasi pengisian saat inisialisasi objek.
    public Guid SessionPlayerId { get; init; }

    // memetakan nama properti JSON menjadi (”player_order_no”).
    [JsonPropertyName("player_order_no")]
    // Mendefinisikan properti `PlayerIndex` bertipe `int?` untuk nilai pemain index; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek; tanda ? mengizinkan nilai null.
    public int? PlayerIndex { get; init; }

    // memetakan nama properti JSON menjadi (”total_donasi”).
    [JsonPropertyName("total_donasi")]
    // Mendefinisikan properti `TotalDonasi` bertipe `int` untuk nilai total donasi; get menyediakan pembacaan nilai, init membatasi pengisian saat
    // inisialisasi objek.
    public int TotalDonasi { get; init; }
// Menutup scope tipe DonationRankingDto; bagian berikut berada di luar batas blok tersebut.
}
