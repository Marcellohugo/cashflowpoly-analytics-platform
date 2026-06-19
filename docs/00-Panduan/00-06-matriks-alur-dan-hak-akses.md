# Matriks Alur dan Hak Akses
## Cashflowpoly Analytics Platform

### Dokumen
- Nama dokumen: Matriks Alur dan Hak Akses
- Versi: 1.0
- Tanggal: 18 Juni 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan
Dokumen ini menjadi rujukan ringkas untuk alur pengguna, cakupan data, istilah
identitas, route UI, endpoint API, dan aturan hak akses pada baseline
implementasi 18 Juni 2026.

Dokumen ini melengkapi:
- `docs/01-Spesifikasi/01-02-spesifikasi-event-dan-kontrak-api.md`
- `docs/01-Spesifikasi/01-04-kontrak-integrasi-idn-dan-keamanan.md`
- `docs/02-Perancangan/02-02-rancangan-model-data-dan-basis-data.md`
- `docs/02-Perancangan/02-05-rancangan-dashboard-analitika-mvc.md`

---

## 2. Istilah Identitas
| Istilah | Dipakai di | Makna |
|---|---|---|
| `user_id` | API, database, JWT claim, query analytics | ID akun pada `app_users`. Untuk role `PLAYER`, nilai ini adalah identitas pemain aplikasi. |
| `instructor_user_id` | `sessions`, `rulesets`, query scope | Pemilik data instruktur. Instruktur hanya mengelola sesi/ruleset miliknya, kecuali ruleset default. |
| `session_participant_id` | database | ID peserta pada sesi tertentu di `session_participants`. |
| `session_player_id` | DTO state/projection gameplay | Alias kontrak untuk peserta sesi. Nilainya merujuk ke `session_participants.session_participant_id`. |
| `player_order_no` | API/DB/UI | Urutan pemain dalam sesi. |

Catatan penting:
- Tidak ada tabel profil pemain terpisah maupun tabel penghubung akun-pemain lama pada baseline schema aktif.
- Resource API `/api/v1/players` merepresentasikan akun `app_users` dengan role `PLAYER`.
- Event aksi Player mengirim `user_id`; API me-resolve nilai tersebut ke peserta sesi sebelum menyimpan event dan projection.

---

## 3. Alur Utama Pengguna
### 3.1 Public
1. Pengguna membuka Web Analitik atau Klien Game/IDN.
2. Pengguna login melalui `POST /api/v1/auth/login` atau register melalui `POST /api/v1/auth/register`.
3. API mengembalikan `access_token`, `user_id`, `username`, `display_name`, `role`, dan `expires_at`.

### 3.2 Instruktur
1. Instruktur login.
2. Instruktur membuat atau memperbarui ruleset melalui Web Analitik atau API.
3. Instruktur mengaktifkan versi ruleset pada level ruleset.
4. Instruktur membuat sesi melalui Klien Game/IDN atau integrasi API dengan `ruleset_version_id` versi `ACTIVE`.
5. Instruktur menambahkan akun Player ke sesi.
6. Instruktur memulai sesi.
7. Klien Game/IDN mengirim event permainan berurutan.
8. Instruktur membaca dashboard sesi, detail Player, timeline event, ruleset, audit, dan observability.
9. Instruktur mengakhiri sesi; API menghitung skor akhir dan menutup lifecycle sesi.

### 3.3 Player
1. Player login.
2. Player melihat daftar sesi yang berisi akun miliknya.
3. Player membuka detail sesi dan detail performa miliknya.
4. Player membaca ruleset dan rulebook.
5. Player tidak dapat mengubah sesi, ruleset, event, state, audit, atau data akun lain.

---

## 4. Matriks Endpoint API
| Kelompok | Endpoint | Public | INSTRUCTOR | PLAYER | Scope data |
|---|---|---:|---:|---:|---|
| Auth | `POST /api/v1/auth/login` | Ya | Ya | Ya | Login akun. |
| Auth | `POST /api/v1/auth/register` | Ya | Ya | Ya | Register role `INSTRUCTOR` atau `PLAYER`. |
| Sessions | `GET /api/v1/sessions` | Tidak | Ya | Ya | Instruktur melihat sesi miliknya; Player melihat sesi yang memuat `user_id` miliknya. |
| Sessions | `POST /api/v1/sessions` | Tidak | Ya | Tidak | Membuat sesi milik instruktur dengan `ruleset_version_id` aktif. |
| Sessions | `POST /api/v1/sessions/{sessionId}/start` | Tidak | Ya | Tidak | Hanya sesi milik instruktur. |
| Sessions | `POST /api/v1/sessions/{sessionId}/end` | Tidak | Ya | Tidak | Hanya sesi milik instruktur. |
| Sessions | `GET /api/v1/sessions/{sessionId}/state` | Tidak | Ya | Tidak | Projection state sesi milik instruktur. |
| Sessions | `PUT /api/v1/sessions/{sessionId}/state` | Tidak | 410 | Tidak | Selalu `410 STATE_WRITE_DISABLED`; state hanya berubah lewat event ingestion. |
| Players | `GET /api/v1/players` | Tidak | Ya | Ya | Instruktur melihat direktori Player yang dapat dikelola; Player melihat scope dirinya/peer yang diizinkan. |
| Players | `POST /api/v1/players` | Tidak | Ya | Tidak | Membuat akun role `PLAYER`. |
| Players | `POST /api/v1/sessions/{sessionId}/players` | Tidak | Ya | Tidak | Menambahkan akun Player ke sesi milik instruktur. |
| Events | `POST /api/v1/events` | Tidak | Ya | Ya | Instruktur pada sesi miliknya; Player hanya untuk `user_id` miliknya pada sesi terdaftar. |
| Events | `POST /api/v1/events/batch` | Tidak | Ya | Ya | Sama seperti event tunggal. |
| Events | `GET /api/v1/sessions/{sessionId}/events` | Tidak | Ya | Ya | Instruktur sesi miliknya; Player hanya sesi yang memuat dirinya. |
| Rulesets | `GET /api/v1/rulesets` | Tidak | Ya | Ya | Default + ruleset instruktur sendiri; Player melihat default + ruleset sesi yang diikutinya. |
| Rulesets | `GET /api/v1/rulesets/{rulesetId}` | Tidak | Ya | Ya | Read sesuai scope. |
| Rulesets | `GET /api/v1/rulesets/{rulesetId}/components?version=` | Tidak | Ya | Ya | Read sesuai scope. |
| Rulesets | `GET /api/v1/rulesets/components/defaults` | Tidak | Ya | Ya | Read ruleset default. |
| Rulesets | `GET /api/v1/game-components` | Tidak | Ya | Ya | Alias baca komponen default. |
| Rulesets | `GET /api/v1/rulesets/sections` | Tidak | Ya | Ya | Baca section definition sesuai mode/scope. |
| Rulesets | `POST /api/v1/rulesets` | Tidak | Ya | Tidak | Membuat ruleset milik instruktur. |
| Rulesets | `PUT /api/v1/rulesets/{rulesetId}` | Tidak | Ya | Tidak | Membuat versi baru untuk ruleset milik instruktur yang mutable. |
| Rulesets | `POST /api/v1/rulesets/{rulesetId}/versions/{version}/activate` | Tidak | Ya | Tidak | Mengaktifkan versi ruleset milik instruktur. |
| Rulesets | `DELETE /api/v1/rulesets/{rulesetId}/versions/{version}` | Tidak | Ya | Tidak | Tidak boleh versi aktif, versi terakhir, atau versi yang sudah dipakai. |
| Rulesets | `DELETE /api/v1/rulesets/{rulesetId}` | Tidak | Ya | Tidak | Tidak boleh ruleset default atau ruleset yang terkunci sesi. |
| Analytics | `GET /api/v1/analytics/sessions/{sessionId}` | Tidak | Ya | Ya | Instruktur sesi miliknya; Player hanya sesi dirinya. |
| Analytics | `GET /api/v1/analytics/sessions/{sessionId}/transactions?userId=` | Tidak | Ya | Ya | Instruktur boleh filter Player dalam sesi; Player hanya `user_id` miliknya. |
| Analytics | `GET /api/v1/analytics/sessions/{sessionId}/players/{userId}/gameplay` | Tidak | Ya | Ya | Instruktur boleh melihat Player pada sesi miliknya; Player hanya dirinya. |
| Analytics | `POST /api/v1/analytics/sessions/{sessionId}/recompute` | Tidak | Ya | Tidak | Recompute sesi milik instruktur. |
| Analytics | `GET /api/v1/analytics/rulesets/{rulesetId}/summary` | Tidak | Ya | Ya | Ringkasan sesuai ruleset yang dapat dibaca. |
| Observability | `GET /api/v1/observability/metrics/summary` | Tidak | Ya | Tidak | Ringkasan operasional API. |
| Observability | `GET /metrics` | Tidak | Operasional | Operasional | Endpoint Prometheus pada service API. |
| Security | `GET /api/v1/security/audit-logs` | Tidak | Ya | Tidak | Audit keamanan. |
| Health | `GET /health/live`, `GET /health/ready` | Ya | Ya | Ya | Health check service. |

---

## 5. Matriks Route UI
| Route UI | Akses | Data/API utama | Catatan |
|---|---|---|---|
| `/auth/login` | Public | `POST /api/v1/auth/login` | Membuat session UI server-side. |
| `/auth/register` | Public | `POST /api/v1/auth/register` | Register Instruktur atau Player sesuai kebijakan API. |
| `/` | Login | Ringkasan UI | Home dan pintasan fitur. |
| `/sessions` | Login | `GET /api/v1/sessions` | Daftar sesi sesuai role/scope. |
| `/sessions/{id}` | Login | analytics, events, ruleset summary | Detail sesi, metrik, timeline, daftar Player. |
| `/sessions/{id}/players/{userId}` | Login | transactions + gameplay metrics | Detail performa akun Player pada sesi. |
| `/players` | Login | `GET /api/v1/players` | Direktori Player; aksi mutasi hanya untuk Instruktur. |
| `/rulesets` | Login | `GET /api/v1/rulesets` | Daftar ruleset sesuai scope. |
| `/rulesets/create` | Instruktur | `POST /api/v1/rulesets` | Form ruleset baru. |
| `/rulesets/{rulesetId}/edit` | Instruktur | `PUT /api/v1/rulesets/{rulesetId}` | Membuat versi baru. |
| `/rulesets/{rulesetId}` | Login | detail + components | Detail versi, definition, komponen, status lock/default. |
| `/rulebook` | Login | Konten lokal rulebook | Referensi aturan permainan. |
| `/Analytics` atau `/analytics` | Login | Redirect | Route legacy menuju halaman sesi/detail yang relevan. |

Catatan UI:
- UI menyimpan `access_token`, `role`, `username`, dan `display_name` pada session server-side.
- UI tidak mengirim event gameplay dan tidak menulis state gameplay.
- UI hanya merender aksi ruleset untuk role `INSTRUCTOR`.

---

## 6. Aturan Ruleset
1. Ruleset default adalah read-only. Ruleset default dapat dibaca oleh Instruktur
   dan Player, tetapi tidak dapat diedit atau dihapus.
2. Ruleset milik instruktur dapat dibuat, diperbarui menjadi versi baru,
   diaktifkan per versi, dan dihapus selama belum terkunci sesi.
3. Sesi mengunci satu `ruleset_version_id` saat dibuat. Setelah sesi dibuat,
   versi ruleset sesi tidak diganti melalui endpoint aktivasi sesi.
4. Ruleset yang sudah dipakai oleh sesi ditandai `is_locked_by_session` pada
   respons list/detail dan tidak boleh dihapus.
5. Versi ruleset `ACTIVE` tidak boleh dihapus. Instruktur harus mengaktifkan
   versi lain terlebih dahulu.
6. Versi terakhir pada sebuah ruleset tidak boleh dihapus melalui endpoint
   delete version; bila ruleset belum terpakai, hapus ruleset induknya.
7. Versi ruleset yang sudah dipakai oleh sesi atau event tidak boleh dihapus.

---

## 7. Aturan Scope Player
1. Player hanya melihat sesi yang memiliki baris `session_participants` untuk
   `user_id` miliknya.
2. Player hanya melihat transaksi, metrik gameplay, dan performa untuk
   `user_id` miliknya.
3. Player tidak dapat membuat sesi, menambahkan Player ke sesi, mengubah status
   sesi, mengelola ruleset, menjalankan recompute, membaca audit keamanan, atau
   membaca observability API.
4. Jika Player mengirim event, API memastikan `user_id` pada payload adalah
   akun Player tersebut dan akun tersebut terdaftar pada sesi.

---

## 8. Alur Data Database
Alur data utama:

```text
app_users
  -> rulesets
  -> ruleset_versions
  -> sessions
  -> session_participants
  -> events
  -> event_asset_references
  -> session projections
  -> event_cashflow_projections
  -> metric_snapshots
  -> session_final_scores
```

Log pendukung:
- `validation_logs` menyimpan event/request invalid yang ditolak.
- `session_narrative_logs` menyimpan hasil trigger narasi.
- `security_audit_logs` menyimpan jejak auth/forbidden/rate-limit/security event.
- `log_retention_policies` menyimpan kebijakan retensi log.

---

## 9. Checklist Konsistensi Implementasi
Dokumentasi dan implementasi dianggap sinkron jika:
1. Payload event memakai `user_id`, bukan ID profil pemain lama.
2. Endpoint analytics transaksi memakai query `userId`.
3. Endpoint gameplay memakai path `/players/{userId}/gameplay`.
4. Request create/update ruleset memakai field `definition`.
5. State write memakai event ingestion; `PUT /state` mengembalikan `410`.
6. Observability API memakai `/api/v1/observability/metrics/summary` dan
   Prometheus memakai `/metrics`.
7. UI utama memakai `/`, `/sessions`, `/sessions/{id}`, `/players`,
   `/sessions/{id}/players/{userId}`, `/rulesets`, `/rulebook`, dan
   `/Analytics` sebagai legacy redirect.
