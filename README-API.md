# Dokumentasi REST API Cashflowpoly

Dokumen ini menjelaskan autentikasi, header, error contract, seluruh endpoint, payload event, ruleset, analitika, dan penggunaan Postman untuk Cashflowpoly Analytics Platform.

Kontrak aktif menggunakan prefix **`/api/v1`**.

Navigasi:

- [README utama](README.md)
- [Dokumentasi database](README-DATABASE.md)
- [Kontrak API dan event terperinci](docs/02-Perancangan/02-02-rancangan-kontrak-api-dan-event.md)

## Daftar isi

- [Base URL dan OpenAPI](#base-url-dan-openapi)
- [Autentikasi](#autentikasi)
- [Header, bahasa, tracing, dan rate limit](#header-bahasa-tracing-dan-rate-limit)
- [Format error](#format-error)
- [Konvensi request dan response](#konvensi-request-dan-response)
- [Ringkasan seluruh endpoint](#ringkasan-seluruh-endpoint)
- [Kontrak event permainan](#kontrak-event-permainan)
- [Kontrak ruleset](#kontrak-ruleset)
- [Kontrak analitika](#kontrak-analitika)
- [Postman](#postman)

## Base URL dan OpenAPI

- lokal langsung ke API: `http://localhost:5041/api/v1`;
- production melalui reverse proxy: `https://<DOMAIN>/api/v1`;
- Swagger UI: `/swagger` hanya pada `Development`;
- dokumen OpenAPI: `/swagger/v1/swagger.json` hanya pada `Development`.

Kontrak runtime pada Swagger dan source controller adalah acuan endpoint. Dokumen payload domain yang lebih panjang berada pada [rancangan kontrak API dan event](docs/02-Perancangan/02-02-rancangan-kontrak-api-dan-event.md).

## Autentikasi

Semua endpoint selain login, register, health, metrics, dan landing API membutuhkan JWT:

```http
Authorization: Bearer <access_token>
```

Login PowerShell:

```powershell
$baseUrl = 'http://localhost:5041/api/v1'
$login = Invoke-RestMethod `
  -Method Post `
  -Uri "$baseUrl/auth/login" `
  -ContentType 'application/json' `
  -Body (@{
    username = 'rina.kartika'
    password = 'SeedLocal!2026'
  } | ConvertTo-Json)

$token = $login.access_token
$headers = @{ Authorization = "Bearer $token" }
```

Respons login:

```json
{
  "user_id": "00000000-0000-0000-0000-000000000000",
  "username": "rina.kartika",
  "role": "INSTRUCTOR",
  "display_name": "Rina Kartika",
  "access_token": "eyJ...",
  "expires_at": "2026-08-20T12:00:00Z"
}
```

## Header, bahasa, tracing, dan rate limit

| Header | Arah | Fungsi |
|---|---|---|
| `Authorization` | Request | JWT Bearer |
| `Content-Type: application/json` | Request | Wajib untuk body JSON |
| `Accept-Language: id` atau `en` | Request | Bahasa pesan; default Indonesia |
| `X-Client-Request-Id` | Request | Korelasi request dari klien |
| `X-Trace-Id` | Response | ID penelusuran server; simpan ketika melaporkan error |

Rate limit API per identitas pengguna atau IP:

- auth: 10 request/menit;
- event: 240 request/menit;
- endpoint lain: 300 request/menit.

Production Nginx menambahkan batas jaringan tersendiri, terutama untuk login. Saat menerima `429`, gunakan retry dengan *backoff* dan jangan mengirim batch yang sama secara paralel.

## Format error

```json
{
  "error_code": "VALIDATION_ERROR",
  "message": "Permintaan tidak valid.",
  "details": [
    {
      "field": "sequence_number",
      "issue": "Harus mengikuti sequence terakhir sesi."
    }
  ],
  "trace_id": "00-..."
}
```

Status yang perlu ditangani klien:

| Status | Arti umum |
|---:|---|
| `200` | Berhasil membaca/memutasi resource |
| `201` | Resource/event berhasil dibuat |
| `204` | Berhasil dihapus tanpa body |
| `400` | JSON/parameter/validasi dasar tidak valid |
| `401` | Token hilang, tidak valid, atau kedaluwarsa |
| `403` | Role atau scope resource tidak diizinkan |
| `404` | Resource tidak ditemukan dalam scope pengguna |
| `409` | Duplikasi atau konflik state/versi/urutan |
| `410` | Endpoint state write sengaja dinonaktifkan |
| `422` | Aturan domain permainan dilanggar |
| `429` | Rate limit terlampaui |
| `500` | Error internal; korelasikan menggunakan `trace_id` |

## Konvensi request dan response

- JSON API memakai UTF-8 dan umumnya `snake_case`.
- Definisi ruleset mempertahankan beberapa nama domain/kompatibilitas seperti `nama`, `hargaBeli`, dan `poinKebahagiaan`; selalu ambil template dari endpoint defaults agar casing dan koleksi lengkap tidak ditebak manual.
- UUID dikirim sebagai string canonical; timestamp dikirim sebagai ISO-8601 dengan offset, direkomendasikan UTC (`Z`).
- Response sukses dikembalikan langsung sebagai DTO, tanpa envelope global `data`.
- Daftar utama memakai bentuk `{ "items": [...] }`; histori event memakai `{ "session_id": "...", "events": [...] }`.
- Response delete `204` tidak memiliki body.
- Field nullable dapat bernilai `null`; beberapa field response create yang opsional dapat tidak diserialisasi.
- Endpoint event adalah satu-satunya daftar yang memiliki window eksplisit (`fromSeq`, `limit`). Endpoint daftar lain pada kontrak saat ini belum memakai pagination.
- Nilai enum/status canonical memakai huruf besar, misalnya `INSTRUCTOR`, `PLAYER`, `PEMULA`, `MAHIR`, `CREATED`, `STARTED`, dan `ENDED`.

Ringkasan bentuk response utama:

| DTO | Field utama |
|---|---|
| `LoginResponse` / `RegisterResponse` | `user_id`, `username`, `role`, `display_name`, `access_token`, `expires_at` |
| `PlayerListResponse` | `items[]` berisi `user_id`, `display_name` |
| `SessionPlayerListResponse` | `items[]` berisi `user_id`, `display_name`, `player_order_no` |
| `SessionListResponse` | `items[]` berisi ID, nama, mode, status, dan timestamp lifecycle |
| `CreateSessionResponse` | `session_id`, `ruleset_id`, `ruleset_version_id`, dan `state` bila tersedia |
| `SessionStateResponse` | versi/sequence berikutnya, hari, turn, slot, game-over, Player, dan donation events |
| `RulesetListResponse` | `items[]` berisi ID, nama, versi terakhir, status, mode, serta flag default/locked |
| `RulesetDetailResponse` | metadata, daftar versi, versi terpilih, mode, dan `definition` |
| `EventStoredResponse` | `stored`, `event_id` |
| `EventBatchResponse` | `stored_count`, `failed[]` (`event_id`, `error_code`) |
| `AnalyticsSessionResponse` | `session_id`, `summary`, `by_player`, ruleset, dan `leaderboard` |
| `GameplayMetricsResponse` | economy, progress, score, needs, raw/derived provenance, dan waktu hitung |
| `SecurityAuditLogResponse` | `items[]` berisi trace, identitas, request, outcome, status, dan detail audit |

Definisi field paling presisi tersedia pada Swagger Development dan [`src/Cashflowpoly.Api/Contracts`](src/Cashflowpoly.Api/Contracts). Hindari menggandakan model response dari teks UI karena label tampilan dapat dilokalkan tanpa mengubah kontrak JSON.

## Ringkasan seluruh endpoint

Legenda akses: **Publik**, **Login** (Player/Instruktur sesuai scope), dan **Instruktur**.

### Auth

| Method | Path | Akses | Request | Hasil |
|---|---|---|---|---|
| `POST` | `/api/v1/auth/login` | Publik | `LoginRequest` | `200 LoginResponse` |
| `POST` | `/api/v1/auth/register` | Publik | `RegisterRequest` | `201 RegisterResponse` |

`LoginRequest`:

```json
{
  "username": "marco",
  "password": "SeedLocal!2026"
}
```

`RegisterRequest`:

```json
{
  "username": "player.baru",
  "password": "password-kuat-minimal-12-karakter",
  "role": "PLAYER",
  "display_name": "Player Baru"
}
```

Password wajib minimal 12 karakter dan maksimal 72 byte UTF-8. Username harus unik. Public register menolak role selain `PLAYER`.

### Player dan peserta sesi

| Method | Path | Akses | Request/query | Hasil |
|---|---|---|---|---|
| `POST` | `/api/v1/players` | Instruktur | `CreatePlayerRequest` | `201 PlayerResponse` |
| `GET` | `/api/v1/players` | Login | — | daftar Player sesuai scope |
| `GET` | `/api/v1/sessions/{sessionId}/players` | Login | — | peserta sesi sesuai scope |
| `POST` | `/api/v1/sessions/{sessionId}/players` | Instruktur | `AddSessionPlayerRequest` | `200 AddSessionPlayerResponse` |

Membuat akun Player:

```json
{
  "display_name": "Player Baru",
  "username": "player.baru",
  "password": "password-kuat-minimal-12-karakter"
}
```

Menambahkan akun yang ada ke sesi—isi `user_id` **atau** `username`:

```json
{
  "username": "player.baru",
  "player_order_no": 4
}
```

### Sesi

| Method | Path | Akses | Request | Hasil/catatan |
|---|---|---|---|---|
| `GET` | `/api/v1/sessions` | Login | — | sesi milik Instruktur / sesi yang diikuti Player |
| `POST` | `/api/v1/sessions` | Instruktur | `CreateSessionRequest` | `201 CreateSessionResponse` |
| `POST` | `/api/v1/sessions/{sessionId}/start` | Instruktur | — | start sesi dan setup awal |
| `POST` | `/api/v1/sessions/{sessionId}/end` | Instruktur | — | end sesi dan skor final |
| `GET` | `/api/v1/sessions/{sessionId}/state` | Instruktur | — | state/proyeksi sesi |
| `PUT` | `/api/v1/sessions/{sessionId}/state` | Instruktur | body legacy | selalu `410 STATE_WRITE_DISABLED` |

Membuat sesi:

```json
{
  "session_name": "Kelas Literasi Keuangan A",
  "mode": "PEMULA",
  "ruleset_version_id": "11111111-1111-1111-1111-111111111111"
}
```

Syarat:

- ruleset version harus `ACTIVE`;
- mode ruleset dan mode sesi harus sama;
- peserta ditambahkan sebelum start;
- jumlah peserta saat start harus berada pada `min_players`–`max_players` ruleset;
- hanya sesi `CREATED` yang dapat di-start dan hanya sesi `STARTED` yang dapat di-end;
- field bootstrap `player_names` hanya tersedia untuk development/testing dan tidak boleh dipakai integrasi production.

Contoh panggilan lifecycle:

```powershell
$session = Invoke-RestMethod -Method Post -Uri "$baseUrl/sessions" -Headers $headers -ContentType 'application/json' -Body (@{
  session_name = 'Kelas A'
  mode = 'PEMULA'
  ruleset_version_id = '<RULESET_VERSION_UUID>'
} | ConvertTo-Json)

$sessionId = $session.session_id

Invoke-RestMethod -Method Post -Uri "$baseUrl/sessions/$sessionId/start" -Headers $headers
# ... kirim event ...
Invoke-RestMethod -Method Post -Uri "$baseUrl/sessions/$sessionId/end" -Headers $headers
```

### Ruleset dan komponen permainan

| Method | Path | Akses | Request/query | Hasil/catatan |
|---|---|---|---|---|
| `GET` | `/api/v1/rulesets/sections` | Login | `mode`, opsional `rulesetId` | katalog section/form ruleset |
| `POST` | `/api/v1/rulesets` | Instruktur | `CreateRulesetRequest` | `201`, membuat versi awal |
| `PUT` | `/api/v1/rulesets/{rulesetId}` | Instruktur | `UpdateRulesetRequest` | membuat versi immutable baru |
| `POST` | `/api/v1/rulesets/{rulesetId}/versions/{version}/activate` | Instruktur | — | mengaktifkan versi |
| `DELETE` | `/api/v1/rulesets/{rulesetId}/versions/{version}` | Instruktur | — | `204` bila aman dihapus |
| `GET` | `/api/v1/rulesets` | Login | — | daftar sesuai scope + default |
| `GET` | `/api/v1/rulesets/components/defaults` | Login | `mode=PEMULA|MAHIR` | template komponen default |
| `GET` | `/api/v1/game-components` | Login | `mode=PEMULA|MAHIR` | alias kompatibilitas endpoint di atas |
| `GET` | `/api/v1/rulesets/{rulesetId}` | Login | — | metadata dan versi ruleset |
| `GET` | `/api/v1/rulesets/{rulesetId}/components` | Login | opsional `version` | komponen versi terpilih |
| `DELETE` | `/api/v1/rulesets/{rulesetId}` | Instruktur | — | `204` bila belum dipakai dan bukan default |

Pola membuat ruleset:

1. Ambil template dari `/rulesets/components/defaults?mode=PEMULA`.
2. Sesuaikan `definition` tanpa mengubah key/jenis data.
3. `POST /rulesets`.
4. Aktifkan versi menggunakan endpoint activate.
5. Gunakan `ruleset_version_id` hasilnya saat membuat sesi.

Body create/update:

```json
{
  "name": "Ruleset Kelas A",
  "description": "Konfigurasi latihan kelas A",
  "definition": {
    "mode": "PEMULA",
    "settings": {
      "actions_per_turn": 2,
      "starting_cash": 10,
      "finish_day": 30,
      "min_players": 2,
      "max_players": 4
    },
    "actions": [],
    "ingredients": [],
    "orders": [],
    "needs": []
  }
}
```

Contoh tersebut hanya memperlihatkan bentuk, bukan definition lengkap. Selalu mulai dari endpoint defaults agar seluruh koleksi wajib dan batas domain ikut terbawa.

### Event

| Method | Path | Akses | Request/query | Hasil |
|---|---|---|---|---|
| `POST` | `/api/v1/events` | Login | `EventRequest` | `201 EventStoredResponse` |
| `POST` | `/api/v1/events/batch` | Login | `EventBatchRequest` | `200`, jumlah berhasil/gagal |
| `GET` | `/api/v1/sessions/{sessionId}/events` | Login | `fromSeq=0`, `limit=200` | event sesuai scope |

`limit` query event harus 1–1000. Batch menerima paling banyak 500 event per request. Lihat [Kontrak event permainan](#kontrak-event-permainan) untuk aturan lengkap.

### Analitika

| Method | Path | Akses | Request/query | Hasil |
|---|---|---|---|---|
| `POST` | `/api/v1/analytics/sessions/{sessionId}/recompute` | Instruktur | — | rebuild projection/metrik sesi |
| `GET` | `/api/v1/analytics/sessions/{sessionId}` | Login | — | ringkasan sesi + leaderboard |
| `GET` | `/api/v1/analytics/sessions/{sessionId}/transactions` | Login | opsional `userId` | transaksi sesuai scope |
| `GET` | `/api/v1/analytics/sessions/{sessionId}/players/{userId}/gameplay` | Login | — | metrik gameplay pemain |
| `GET` | `/api/v1/analytics/rulesets/{rulesetId}/summary` | Login | — | agregasi sesi ruleset |

Instruktur dapat meminta peserta dalam sesi miliknya. Player selalu dibatasi ke `user_id` miliknya meskipun query/path diubah.

### Operasional dan keamanan

| Method | Path | Akses | Query | Hasil |
|---|---|---|---|---|
| `GET` | `/api/v1/observability/metrics/summary` | Instruktur | — | pesan penunjuk bahwa metrics tersedia di `/metrics` |
| `GET` | `/api/v1/security/audit-logs` | Instruktur | `limit`, `eventType`, `userId` | audit keamanan; `limit` dijepit ke 1–500 |
| `GET` | `/health/live` | Publik | — | proses API hidup |
| `GET` | `/health/ready` | Publik | — | API dan database siap |
| `GET` | `/metrics` | Publik langsung ke API | — | format Prometheus |

Endpoint `/metrics` sebaiknya hanya dapat dijangkau jaringan monitoring tepercaya pada production.

## Kontrak event permainan

### Bentuk EventRequest

```json
{
  "event_id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
  "session_id": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
  "user_id": "cccccccc-cccc-cccc-cccc-cccccccccccc",
  "actor_type": "PLAYER",
  "timestamp": "2026-08-20T08:30:00Z",
  "day_index": 1,
  "weekday": "MON",
  "turn_number": 1,
  "action_slot": 1,
  "sequence_number": 17,
  "action_type": "KerjaLepas",
  "ruleset_version_id": "dddddddd-dddd-dddd-dddd-dddddddddddd",
  "payload": {
    "amount": 1
  },
  "client_request_id": "game-client-01:session-b:17"
}
```

| Field | Aturan |
|---|---|
| `event_id` | UUID unik global; event duplikat tidak diterapkan dua kali |
| `session_id` | Sesi harus ada, dapat diakses, dan berstatus `STARTED` |
| `user_id` | Wajib untuk aktor `PLAYER`; harus peserta sesi |
| `actor_type` | `PLAYER` atau `SYSTEM` |
| `timestamp` | ISO-8601/UTC direkomendasikan |
| `day_index` | Bilangan bulat ≥ 0 dan konsisten dengan progres sesi |
| `weekday` | `MON`, `TUE`, `WED`, `THU`, `FRI`, `SAT`, atau `SUN` |
| `turn_number` | `0` untuk system/setup; Player wajib `1..4` pada kontrak saat ini dan harus sama dengan `player_order_no` |
| `action_slot` | `0` untuk system/aksi bebas; `1..actions_per_turn` untuk aksi utama |
| `sequence_number` | Kontigu per sesi; tidak boleh duplikat atau melompati nomor berikutnya |
| `action_type` | Nama aksi canonical; pencocokan server case-insensitive, tetapi klien sebaiknya memakai ejaan katalog |
| `ruleset_version_id` | Harus sama dengan versi yang dikunci sesi |
| `payload` | Object JSON spesifik action type |
| `client_request_id` | Opsional; ID korelasi stabil dari klien |

### Urutan, idempotensi, dan retry

- Ambil sequence terakhir bila klien kehilangan sinkronisasi menggunakan `GET /sessions/{sessionId}/events`.
- Start sesi otomatis menulis event setup mulai dari sequence `0`. Setelah start, Instruktur dapat membaca `state.next_sequence_number` melalui `GET /sessions/{sessionId}/state`; jangan mengasumsikan event gameplay pertama bernomor `0` atau `1`.
- Kirim event berikutnya hanya setelah event sebelumnya berhasil.
- Jangan memakai UUID baru untuk me-retry keputusan yang sama; pertahankan `event_id` dan `client_request_id` agar duplikasi terdeteksi.
- Duplicate/sequence conflict mengembalikan `409`; klien harus rekonsiliasi, bukan terus menambah sequence secara buta.
- Pelanggaran aturan domain mengembalikan `422`; perbaiki keputusan/payload sebelum retry.
- Batch mempertahankan urutan array. Jangan mengurutkan ulang event yang sudah diberi sequence.

### Batch request

```json
{
  "events": [
    {
      "event_id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1",
      "session_id": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
      "user_id": "cccccccc-cccc-cccc-cccc-cccccccccccc",
      "actor_type": "PLAYER",
      "timestamp": "2026-08-20T08:30:00Z",
      "day_index": 1,
      "weekday": "MON",
      "turn_number": 1,
      "action_slot": 1,
      "sequence_number": 17,
      "action_type": "KerjaLepas",
      "ruleset_version_id": "dddddddd-dddd-dddd-dddd-dddddddddddd",
      "payload": {
        "amount": 1
      },
      "client_request_id": "client:17"
    }
  ]
}
```

### Katalog action type

| Kelompok | Action type canonical |
|---|---|
| Lifecycle/setup system | `MulaiSesi`, `AkhiriSesi`, `AkhirGiliran`, `SetupModalAwal`, `SetupBahanAwal`, `SetupEmasAwal`, `SetupMisiAwal`, `SetupPinjamanAwal`, `SetupAsuransiAwal` |
| Deck/pasar system | `AmbilKartuDariDeck`, `KartuMasukDiscard`, `IsiUlangPasar`, `BukaHargaEmas`, `HariMingguLibur` |
| Arus kas | `CatatTransaksi`, `KerjaLepas`, `Menabung` (`TarikTabungan` hanya dikenali untuk data lama dan ditolak saat ingestion) |
| Bahan dan pesanan | `BahanMasakan`, `BuangBahanMasakan`, `JualMasakan`, `LewatiOrder` |
| Kebutuhan dan tujuan | `Kebutuhan`; `TujuanFinansial` dicatat otomatis oleh aktor `SYSTEM` |
| Donasi dan peringkat | `JumatBerkah`, `PoinPeringkatDonasi`, `UmumkanJuaraDonasi`, `PoinPeringkatPensiun`, `BagikanTieBreaker` |
| Emas | `InvestasiEmas`, `JualEmas`, `LewatiTransaksiEmas`, `PoinEmas` |
| Pinjaman/asuransi/risiko | `PinjamanSyariah`, `BayarPinjaman`, `Asuransi`, `RisikoKehidupan`, `BayarRisiko`, `GunakanOpsiDarurat` |

Aturan aktor dan slot:

- aksi utama Player umumnya memakai slot `1..actions_per_turn`;
- `JumatBerkah`, aksi risiko tertentu, serta transaksi emas tertentu merupakan aksi bebas dengan slot `0`;
- `Asuransi` dan `PinjamanSyariah` dapat menjadi aksi bebas hanya ketika menyelesaikan konteks risiko yang sah melalui `risk_event_id`;
- action type setup, lifecycle, deck, pasar, dan scoring hanya boleh dikirim aktor `SYSTEM`;
- `AmbilKartuDariDeck` dan `IsiUlangPasar` adalah operasi internal/setup dan ingestion manual saat runtime dapat ditolak.
- Senin–Kamis ditutup setelah setiap pemain memakai tepat dua aksi. Jumat memerlukan tepat satu donasi per pemain. Sabtu memerlukan satu harga emas dan tepat satu pilihan beli/jual/lewati per pemain; jumlah kartu dalam transaksi boleh lebih dari satu.
- Harga/poin kebutuhan dan tujuan harus sama dengan katalog. Total Kartu Emas yang sedang dimiliki seluruh pemain dibatasi 20 dan setiap Kartu Tujuan Finansial hanya tersedia sesuai `cardQty` (default satu).

Field payload setiap action—termasuk contoh, tipe data, dan invariannya—didokumentasikan pada [bagian 4 kontrak event](docs/02-Perancangan/02-02-rancangan-kontrak-api-dan-event.md#4-katalog-event-dan-spesifikasi-payload). Jangan menebak key payload dari teks tampilan UI.

## Kontrak ruleset

`RulesetDefinitionDto` berisi seluruh aturan yang dibekukan ke satu versi:

| Bagian | Isi |
|---|---|
| `mode` | `PEMULA` atau `MAHIR` |
| `settings` | aksi/giliran, modal, hari akhir, batas Player, limit inventory, fitur, dan nilai global |
| `player_ordering` | aturan urutan pemain |
| `actions` | aksi yang tersedia dan parameternya |
| `ingredients` | katalog bahan dan jumlah fisik `cardQty` (default rulebook: 5 per jenis) |
| `orders` | katalog pesanan |
| `needs` | kebutuhan primer/sekunder/tersier beserta jumlah fisik `cardQty` |
| `need_set_bonuses` | bonus set kebutuhan |
| `collection_missions` | misi koleksi |
| `financial_goals` | tujuan finansial beserta jumlah fisik `cardQty` |
| `narratives` | teks/aturan naratif |
| `donation_rank_points` | skor peringkat donasi |
| `gold_points_by_qty` | skor jumlah emas |
| `gold_prices` | harga emas per hari/kondisi |
| `pension_rank_points` | skor peringkat dana pensiun |
| `tie_breakers` | aturan pemecah seri |
| `sharia_loans` | produk pinjaman syariah |
| `insurance_products` | produk asuransi |
| `life_risks` | risiko kehidupan dan opsi penyelesaian |

`settings` antara lain memuat `actions_per_turn`, `starting_cash`, `initial_coins`, `initial_happiness`, `initial_saving`, `finish_day`, `min_players`, `max_players`, `cash_min`, batas bahan, aturan kebutuhan primer, batas donasi, aturan emas, fitur pinjaman/asuransi/tujuan menabung, dan pendapatan kerja lepas.

Prinsip versioning:

1. create menghasilkan ruleset dan versi pertama;
2. update tidak menimpa versi lama—API membuat versi baru;
3. versi harus diaktifkan sebelum dapat dipakai sesi production;
4. sesi menyimpan `ruleset_version_id`, bukan hanya `ruleset_id`;
5. versi aktif, versi yang dipakai sesi, versi terakhir, dan ruleset default memiliki guard penghapusan;
6. mode versi ruleset harus sama dengan mode sesi.

Untuk integrasi yang tahan perubahan, bangun form/config dari `/rulesets/sections` dan `/rulesets/components/defaults`, bukan dari daftar hard-coded pada klien.

## Kontrak analitika

### Ringkasan sesi

`GET /api/v1/analytics/sessions/{sessionId}` menghasilkan:

- `session_id`;
- `summary`: `event_count`, `cash_in_total`, `cash_out_total`, `cashflow_net_total`, dan `rules_violations_count`;
- `by_player`: agregasi per Player;
- metadata `ruleset_id` dan nama ruleset;
- `leaderboard`.

`rules_violations_count` merepresentasikan log validasi/audit yang relevan, bukan event gameplay ilegal yang lolos. Backend tetap menolak mutasi yang melanggar aturan.

### Metrik gameplay Player

`GET /api/v1/analytics/sessions/{sessionId}/players/{userId}/gameplay` menghasilkan:

- `session_id`, `user_id`, dan `computed_at`;
- kelompok `economy`;
- kelompok `progress`;
- kelompok `score`;
- kelompok `needs`;
- `rules_violations_count`;
- `raw_json` untuk angka sumber;
- `derived_json` untuk nilai turunan dan penjelasan perhitungan.

Field terstruktur pada kelompok metrik:

| Kelompok | Field |
|---|---|
| `economy` | `starting_cash`, `cash_in_total`, `cash_out_total`, `cashflow_net_total`, `donation_total` |
| `progress` | `gold_qty`, `orders_completed_count`, `inventory_ingredient_total`, `actions_used_total` |
| `score` | `happiness_points_total`, `need_points_total`, `need_set_bonus_points`, `donation_points_total`, `gold_points_total`, `pension_points_total`, `saving_goal_points_total`, `mission_penalty_total`, `loan_penalty_total`, `has_unpaid_loan` |
| `needs` | `fulfillment_diversity` |

Item `by_player` pada ringkasan sesi memadukan identitas (`user_id`, `player_order_no`), arus kas, donasi, emas, pesanan, inventory, pemakaian aksi, keberagaman kebutuhan, pelanggaran tervalidasi, seluruh komponen skor, penalti, dan status pinjaman. `leaderboard` berisi `user_id`, `player_order_no`, `rank`, dan `happiness_points_total`, dan bersifat final setelah sesi `ENDED`.

Tampilan **Lihat angka pembentuk dan rumus/Lihat cara menghitung** harus membaca variabel dari response/provenance tersebut. Nama variabel, angka aktual, substitusi rumus, hasil, dan asal data harus konsisten dengan parent metric.

### Transaksi

`GET /api/v1/analytics/sessions/{sessionId}/transactions?userId=<UUID>` mengembalikan item dengan timestamp, arah (`IN`/`OUT`), jumlah, dan kategori. Untuk Player, API mengabaikan upaya memilih user lain dan tetap menerapkan scope diri sendiri.

### Recompute

Gunakan recompute bila:

- proyeksi lama perlu dibangun ulang dari event;
- algoritme metrik berubah dan kompatibel dengan data event yang tersimpan;
- verifikasi operasional menemukan projection drift.

Recompute bukan cara memperbaiki event salah. Event salah harus ditangani sesuai kebijakan koreksi/audit, bukan dengan menulis state langsung.

## Postman

File:

- [`postman/Cashflowpoly.postman_collection.json`](postman/Cashflowpoly.postman_collection.json);
- [`postman/Cashflowpoly.local.postman_environment.json`](postman/Cashflowpoly.local.postman_environment.json).

Cara memakai:

1. Import collection dan environment ke Postman.
2. Pilih environment **Cashflowpoly Local**.
3. Pastikan `baseUrl` bernilai `http://localhost:5041`.
4. Isi `authUsername` dan `authPassword` dengan akun Instruktur development.
5. Jalankan folder **Auth**; test script menyimpan `accessToken`.
6. Jalankan **Rulesets** untuk membaca default/membuat/mengaktifkan versi.
7. Jalankan **Players** dan **Sessions** untuk menyiapkan peserta dan sesi.
8. Jalankan **Events** secara berurutan.
9. Jalankan **Session Close** lalu **Analytics**.
10. Jalankan **Observability and Security** dan **Health** untuk pemeriksaan operasional.

Variabel collection utama mencakup:

- `baseUrl`, `authUsername`, `authPassword`, `accessToken`;
- `rulesetId`, `rulesetVersionNumber`, `rulesetVersionId`;
- `sessionId`;
- ID akun/peserta Player;
- ID event dan nilai sequence.

Request yang namanya diawali **Reject** serta folder **Destructive Guards** sengaja menguji respons gagal seperti `409`/`422`; hasil tersebut bukan kegagalan sistem bila assertion Postman lulus. Jangan menjalankan operasi delete terhadap data production tanpa meninjau ID dan prasyaratnya.
