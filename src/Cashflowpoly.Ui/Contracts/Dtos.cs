// Fungsi file: Mendefinisikan kontrak pertukaran data UI dengan API untuk Dtos.
// Mengimpor namespace `System.Text.Json` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json;
// Mengimpor namespace `System.Text.Json.Serialization` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using System.Text.Json.Serialization;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Contracts` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Contracts;

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `PlayerResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record PlayerResponse(
    // Parameter `UserId` bertipe `Guid` membawa identitas akun pengguna yang datanya sedang diproses; memetakan nama properti JSON menjadi (”user_id”).
    [property: JsonPropertyName("user_id")] Guid UserId,
    // Parameter `DisplayName` bertipe `string` membawa nilai display nama; memetakan nama properti JSON menjadi (”display_name”).
    [property: JsonPropertyName("display_name")] string DisplayName);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `PlayerListResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record PlayerListResponse(
    // Parameter `Items` bertipe `List<PlayerResponse>` membawa nilai elemen; memetakan nama properti JSON menjadi (”items”).
    [property: JsonPropertyName("items")] List<PlayerResponse> Items);

// Mendefinisikan record untuk membawa data dengan kesetaraan berbasis nilai `SessionPlayerResponse`; sealed mencegah tipe ini diturunkan lagi.
public sealed record SessionPlayerResponse(
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
public sealed record SessionListResponse(
    // Parameter `Items` bertipe `List<SessionListItem>` membawa nilai elemen; memetakan nama properti JSON menjadi (”items”).
    [property: JsonPropertyName("items")] List<SessionListItem> Items);

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
public sealed record RulesetListResponse(
    // Parameter `Items` bertipe `List<RulesetListItem>` membawa nilai elemen; memetakan nama properti JSON menjadi (”items”).
    [property: JsonPropertyName("items")] List<RulesetListItem> Items);

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
