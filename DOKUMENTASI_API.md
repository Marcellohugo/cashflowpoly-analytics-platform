# Dokumentasi API Cashflowpoly

## Baseline
- Baseline implementasi: 18 Juni 2026.
- Base URL lokal API: `http://localhost:5041`.
- Format utama: JSON.
- Kontrak teknis detail berada di `docs/01-Spesifikasi/01-02-spesifikasi-event-dan-kontrak-api.md`.
- Kontrak keamanan dan integrasi berada di `docs/01-Spesifikasi/01-04-kontrak-integrasi-idn-dan-keamanan.md`.

Dokumen ini adalah ringkasan lengkap tingkat root untuk integrasi API. Jika ada perbedaan detail teknis, kode controller dan DTO pada `src/Cashflowpoly.Api` menjadi acuan implementasi, lalu dokumen spesifikasi di `docs/` menjadi acuan penjelasan.

---

## 1. Prinsip Umum API
1. API memakai versi path `/api/v1`.
2. Endpoint publik hanya autentikasi: login dan register.
3. Endpoint selain autentikasi membutuhkan header:

```http
Authorization: Bearer <access_token>
Content-Type: application/json
```

4. Gameplay memakai pendekatan event-first. State sesi tidak ditulis langsung oleh klien.
5. Event Player memakai `user_id` akun role `PLAYER`, bukan ID profil pemain lama.
6. API me-resolve `user_id` ke peserta sesi, yaitu `session_participant_id`. Pada DTO state, ID ini tampil sebagai `session_player_id`.
7. Request ruleset memakai field `definition`, bukan `config`.
8. Analytics transaksi memakai query `userId`.
9. Endpoint state write `PUT /api/v1/sessions/{sessionId}/state` selalu mengembalikan `410 STATE_WRITE_DISABLED`.

---

## 2. Istilah Identitas
| Istilah | Lokasi | Makna |
|---|---|---|
| `user_id` | API, JWT, `app_users`, analytics query | ID akun aplikasi. Untuk role `PLAYER`, ini adalah identitas pemain aplikasi. |
| `instructor_user_id` | `sessions`, `rulesets` | Pemilik sesi/ruleset dari sisi instruktur. |
| `session_participant_id` | Database | ID peserta pada sesi tertentu. |
| `session_player_id` | DTO state/projection | Alias kontrak untuk `session_participants.session_participant_id`. |
| `player_order_no` | API, DB, UI | Urutan pemain dalam sesi. |
| `ruleset_id` | Ruleset | ID wadah ruleset. |
| `ruleset_version_id` | Ruleset version, session, event | ID versi ruleset yang dikunci sesi dan dipakai event. |

---

## 3. Role dan Hak Akses
| Role | Kemampuan utama |
|---|---|
| Public | Login dan register. |
| `INSTRUCTOR` | Mengelola ruleset, sesi, Player, event, recompute, analytics, audit, observability. Scope utama adalah data miliknya. |
| `PLAYER` | Membaca sesi yang memuat dirinya, ruleset yang relevan, event/analytics sesuai scope, dan mengirim event untuk dirinya pada sesi yang diikuti. |

Aturan penting:
- Player tidak dapat membuat sesi, mengelola ruleset, menambah peserta, menjalankan recompute, membaca audit log, atau membaca observability API.
- Instruktur hanya dapat mengubah ruleset miliknya yang belum terkunci sesi.
- Ruleset default sistem bersifat read-only.

---

## 4. Format Error Standar
Semua error API memakai format umum:

```json
{
  "error_code": "VALIDATION_ERROR",
  "message": "Field wajib tidak lengkap",
  "details": [
    { "field": "session_id", "issue": "REQUIRED" }
  ],
  "trace_id": "00-...-..."
}
```

Status code umum:

| Status | Makna |
|---:|---|
| 200 | Request berhasil. |
| 201 | Resource berhasil dibuat. |
| 204 | Resource berhasil dihapus tanpa body response. |
| 400 | Request tidak valid. |
| 401 | Token tidak ada atau tidak valid. |
| 403 | Role/scope tidak diizinkan. |
| 404 | Resource tidak ditemukan atau disembunyikan karena scope. |
| 409 | Konflik data, misalnya event duplikat. |
| 410 | Endpoint tidak lagi menerima operasi tersebut. |
| 422 | Aturan domain dilanggar. |
| 429 | Rate limit. |
| 500 | Kesalahan internal. |

---

## 5. Endpoint Auth
### 5.1 Login
```http
POST /api/v1/auth/login
```

Request:

```json
{
  "username": "instructor",
  "password": "your-strong-password"
}
```

Response 200:

```json
{
  "user_id": "uuid",
  "username": "instructor",
  "role": "INSTRUCTOR",
  "display_name": "Ibu Rina",
  "access_token": "jwt",
  "expires_at": "2026-06-18T12:00:00Z"
}
```

### 5.2 Register
```http
POST /api/v1/auth/register
```

Request:

```json
{
  "username": "player_a",
  "password": "your-strong-password",
  "role": "PLAYER",
  "display_name": "Player A"
}
```

Response 201:

```json
{
  "user_id": "uuid",
  "username": "player_a",
  "role": "PLAYER",
  "display_name": "Player A",
  "access_token": "jwt",
  "expires_at": "2026-06-18T12:00:00Z"
}
```

---

## 6. Endpoint Sessions
### 6.1 List Sessions
```http
GET /api/v1/sessions
```

Otorisasi: `INSTRUCTOR`, `PLAYER`.

Scope:
- Instruktur melihat sesi miliknya.
- Player melihat sesi yang memuat `user_id` miliknya.

Response 200:

```json
{
  "items": [
    {
      "session_id": "uuid",
      "session_name": "Kelas A - Pertemuan 1",
      "mode": "PEMULA",
      "status": "CREATED",
      "created_at": "2026-06-18T10:00:00Z",
      "started_at": null,
      "ended_at": null
    }
  ]
}
```

### 6.2 Create Session
```http
POST /api/v1/sessions
```

Otorisasi: `INSTRUCTOR`.

Request:

```json
{
  "session_name": "Kelas A - Pertemuan 1",
  "mode": "PEMULA",
  "ruleset_version_id": "uuid"
}
```

Opsional:

```json
{
  "session_name": "Kelas A - Pertemuan 1",
  "mode": "PEMULA",
  "ruleset_version_id": "uuid",
  "player_names": ["Player A", "Player B"]
}
```

Response 201:

```json
{
  "session_id": "uuid",
  "ruleset_id": "uuid",
  "ruleset_version_id": "uuid",
  "state": {
    "session_id": "uuid",
    "state_version": 1,
    "day": 1,
    "turn": 1,
    "action_slots_left": 2,
    "finish_day": 25,
    "is_game_over": false,
    "players": [],
    "donationEvents": []
  }
}
```

### 6.3 Start Session
```http
POST /api/v1/sessions/{sessionId}/start
```

Otorisasi: `INSTRUCTOR`.

Response 200:

```json
{ "status": "STARTED" }
```

### 6.4 End Session
```http
POST /api/v1/sessions/{sessionId}/end
```

Otorisasi: `INSTRUCTOR`.

Response 200:

```json
{ "status": "ENDED" }
```

### 6.5 Get Session State
```http
GET /api/v1/sessions/{sessionId}/state
```

Otorisasi: `INSTRUCTOR`.

Response 200:

```json
{
  "session_id": "uuid",
  "state_version": 3,
  "day": 1,
  "turn": 1,
  "action_slots_left": 2,
  "finish_day": 25,
  "is_game_over": false,
  "players": [
    {
      "session_player_id": "uuid",
      "user_id": "uuid",
      "player_order_no": 1,
      "name": "Player A",
      "coins": 20,
      "happiness": 0,
      "saving": 0,
      "bahan": [],
      "kebutuhan": [],
      "tujuanFinansial": [],
      "targetKebutuhan": [],
      "actionCounters": [],
      "totalDonasi": 0
    }
  ],
  "donationEvents": []
}
```

### 6.6 Write Session State
```http
PUT /api/v1/sessions/{sessionId}/state
```

Otorisasi: `INSTRUCTOR`.

Status: selalu `410 Gone` bila sesi ditemukan.

Response:

```json
{
  "error_code": "STATE_WRITE_DISABLED",
  "message": "State permainan hanya dapat diubah melalui event ingestion",
  "details": [],
  "trace_id": "00-...-..."
}
```

---

## 7. Endpoint Players
Resource Player merepresentasikan akun `app_users` dengan role `PLAYER`.

### 7.1 Create Player Account
```http
POST /api/v1/players
```

Otorisasi: `INSTRUCTOR`.

Request:

```json
{
  "display_name": "Player A",
  "username": "player_a",
  "password": "your-strong-password"
}
```

Response 201:

```json
{
  "user_id": "uuid",
  "display_name": "Player A"
}
```

### 7.2 List Players
```http
GET /api/v1/players
```

Otorisasi: `INSTRUCTOR`, `PLAYER`.

Response 200:

```json
{
  "items": [
    { "user_id": "uuid", "display_name": "Player A" }
  ]
}
```

### 7.3 Add Player To Session
```http
POST /api/v1/sessions/{sessionId}/players
```

Otorisasi: `INSTRUCTOR`.

Request memakai `user_id`:

```json
{
  "user_id": "uuid",
  "player_order_no": 1
}
```

Alternatif memakai `username`:

```json
{
  "username": "player_a",
  "player_order_no": 1
}
```

Response 200:

```json
{
  "user_id": "uuid",
  "player_order_no": 1
}
```

---

## 8. Endpoint Rulesets
### 8.1 Definition Ruleset
Request create/update ruleset memakai field `definition`.

Struktur utama:

```json
{
  "mode": "PEMULA",
  "settings": {
    "actions_per_turn": 2,
    "starting_cash": 20,
    "initial_coins": 20,
    "initial_happiness": 0,
    "initial_saving": 0,
    "finish_day": 25,
    "min_players": 2,
    "max_players": 4,
    "cash_min": 0,
    "max_ingredient_total": 6,
    "max_same_ingredient": 3,
    "primary_need_max_per_day": 1,
    "require_primary_before_others": true,
    "donation_min_amount": 1,
    "donation_max_amount": 1,
    "gold_trade_allow_buy": true,
    "gold_trade_allow_sell": true,
    "loan_enabled": false,
    "insurance_enabled": false,
    "saving_goal_enabled": false,
    "freelance_income": 1
  },
  "player_ordering": {
    "ordering_code": "PLAYER_ORDER",
    "friday_feature": "DONATION",
    "friday_enabled": true,
    "saturday_feature": "GOLD_TRADE",
    "saturday_enabled": true,
    "sunday_feature": "REST",
    "sunday_enabled": true,
    "instructor_player_usernames": []
  },
  "actions": [{ "action_id": "work.freelance.completed" }],
  "ingredients": [],
  "orders": [],
  "needs": [],
  "need_set_bonuses": [],
  "collection_missions": [],
  "financial_goals": [],
  "narratives": [],
  "donation_rank_points": [],
  "gold_points_by_qty": [],
  "gold_prices": [],
  "pension_rank_points": [],
  "tie_breakers": [],
  "sharia_loans": [],
  "insurance_products": [],
  "life_risks": []
}
```

Section penting:
- `settings`: aturan dasar permainan.
- `actions`: daftar action valid.
- `ingredients`, `orders`, `needs`: komponen utama mode pemula.
- `collection_missions`, `need_set_bonuses`: misi dan skor kebutuhan.
- `financial_goals`, `sharia_loans`, `insurance_products`, `life_risks`: fitur mode mahir.
- `donation_rank_points`, `gold_points_by_qty`, `pension_rank_points`, `tie_breakers`: konfigurasi scoring.

### 8.2 Create Ruleset
```http
POST /api/v1/rulesets
```

Otorisasi: `INSTRUCTOR`.

Request:

```json
{
  "name": "Ruleset Kelas A",
  "description": "Ruleset untuk simulasi kelas A",
  "definition": { "...": "..." }
}
```

Response 201:

```json
{
  "ruleset_id": "uuid",
  "ruleset_version_id": "uuid",
  "version": 1
}
```

### 8.3 Update Ruleset
```http
PUT /api/v1/rulesets/{rulesetId}
```

Otorisasi: `INSTRUCTOR`.

Efek: membuat versi baru.

Request:

```json
{
  "name": "Ruleset Kelas A Revisi",
  "description": "Perubahan kecil",
  "definition": { "...": "..." }
}
```

Response 200:

```json
{
  "ruleset_id": "uuid",
  "ruleset_version_id": "uuid",
  "version": 2
}
```

### 8.4 Activate Ruleset Version
```http
POST /api/v1/rulesets/{rulesetId}/versions/{version}/activate
```

Otorisasi: `INSTRUCTOR`.

Response 200:

```json
{
  "ruleset_id": "uuid",
  "ruleset_version_id": "uuid",
  "version": 2
}
```

### 8.5 Delete Ruleset Version
```http
DELETE /api/v1/rulesets/{rulesetId}/versions/{version}
```

Otorisasi: `INSTRUCTOR`.

Response: `204 No Content`.

Tidak boleh:
- versi aktif,
- versi terakhir,
- versi yang sudah dipakai session/event.

### 8.6 Delete Ruleset
```http
DELETE /api/v1/rulesets/{rulesetId}
```

Otorisasi: `INSTRUCTOR`.

Response: `204 No Content`.

Tidak boleh:
- ruleset default,
- ruleset instruktur yang sudah terkunci sesi.

### 8.7 List Rulesets
```http
GET /api/v1/rulesets
```

Otorisasi: `INSTRUCTOR`, `PLAYER`.

Response 200:

```json
{
  "items": [
    {
      "ruleset_id": "uuid",
      "name": "Ruleset Kelas A",
      "latest_version": 2,
      "status": "ACTIVE",
      "is_default": false,
      "is_locked_by_session": false
    }
  ]
}
```

### 8.8 Get Ruleset Detail
```http
GET /api/v1/rulesets/{rulesetId}
```

Otorisasi: `INSTRUCTOR`, `PLAYER`.

Response 200:

```json
{
  "ruleset_id": "uuid",
  "name": "Ruleset Kelas A",
  "description": "Ruleset untuk simulasi kelas A",
  "versions": [
    {
      "ruleset_version_id": "uuid",
      "version": 1,
      "status": "ACTIVE",
      "created_at": "2026-06-18T10:00:00Z"
    }
  ],
  "ruleset_version_id": "uuid",
  "version": 1,
  "mode": "PEMULA",
  "definition": { "...": "..." },
  "is_default": false,
  "is_locked_by_session": false
}
```

### 8.9 Get Ruleset Components
```http
GET /api/v1/rulesets/{rulesetId}/components?version=1
```

Otorisasi: `INSTRUCTOR`, `PLAYER`.

Response 200:

```json
{
  "ruleset_id": "uuid",
  "ruleset_version_id": "uuid",
  "version": 1,
  "mode": "PEMULA",
  "definition": { "...": "..." }
}
```

### 8.10 Get Default Components
```http
GET /api/v1/rulesets/components/defaults?mode=PEMULA
GET /api/v1/game-components
```

Otorisasi: `INSTRUCTOR`, `PLAYER`.

Response 200:

```json
{
  "items": [
    {
      "ruleset_id": "uuid",
      "name": "Default Pemula",
      "description": "Komponen default",
      "ruleset_version_id": "uuid",
      "version": 1,
      "mode": "PEMULA",
      "definition": { "...": "..." }
    }
  ]
}
```

### 8.11 Get Ruleset Sections
```http
GET /api/v1/rulesets/sections?mode=PEMULA&rulesetId=uuid
```

Otorisasi: `INSTRUCTOR`, `PLAYER`.

Response 200:

```json
{
  "ruleset_id": "uuid",
  "ruleset_version_id": "uuid",
  "mode": "PEMULA",
  "gameConfig": {},
  "bahan": {},
  "resep": {},
  "kebutuhan": {},
  "targetKebutuhan": {},
  "tujuanFinansial": {},
  "narasi": {}
}
```

---

## 9. Endpoint Events
### 9.1 Struktur Event Umum
```json
{
  "event_id": "uuid",
  "session_id": "uuid",
  "user_id": "uuid",
  "actor_type": "PLAYER",
  "timestamp": "2026-06-18T10:00:00Z",
  "day_index": 1,
  "weekday": "MON",
  "turn_number": 1,
  "action_slot": 1,
  "sequence_number": 1,
  "action_type": "work.freelance.completed",
  "ruleset_version_id": "uuid",
  "payload": {},
  "client_request_id": "optional-client-id"
}
```

Field wajib:
- `event_id`
- `session_id`
- `actor_type`
- `timestamp`
- `day_index`
- `weekday`
- `turn_number`
- `action_slot`
- `sequence_number`
- `action_type`
- `ruleset_version_id`
- `payload`

Khusus event Player:
- `user_id` wajib.
- API memastikan `user_id` terdaftar pada session.

Khusus event sistem:
- `actor_type` bernilai `SYSTEM`.
- `user_id` boleh `null`.

### 9.2 Create Event
```http
POST /api/v1/events
```

Otorisasi: `INSTRUCTOR`, `PLAYER`.

Response 201:

```json
{
  "stored": true,
  "event_id": "uuid"
}
```

Validasi utama:
- session ada dan status valid,
- `ruleset_version_id` cocok dengan session,
- event tidak duplikat berdasarkan `(session_id, event_id)`,
- `sequence_number` berurutan,
- `action_type` valid pada ruleset,
- payload valid untuk domain action,
- Player hanya boleh mengirim event untuk `user_id` miliknya.

### 9.3 Create Events Batch
```http
POST /api/v1/events/batch
```

Otorisasi: `INSTRUCTOR`, `PLAYER`.

Request:

```json
{
  "events": [
    { "...": "event-1" },
    { "...": "event-2" }
  ]
}
```

Response 200:

```json
{
  "stored_count": 2,
  "failed": []
}
```

Jika sebagian gagal:

```json
{
  "stored_count": 1,
  "failed": [
    { "event_id": "uuid", "error_code": "VALIDATION_ERROR" }
  ]
}
```

### 9.4 Get Events By Session
```http
GET /api/v1/sessions/{sessionId}/events?fromSeq=0&limit=200
```

Otorisasi: `INSTRUCTOR`, `PLAYER`.

Response 200:

```json
{
  "session_id": "uuid",
  "events": [
    {
      "event_id": "uuid",
      "session_id": "uuid",
      "user_id": "uuid",
      "actor_type": "PLAYER",
      "timestamp": "2026-06-18T10:00:00Z",
      "day_index": 1,
      "weekday": "MON",
      "turn_number": 1,
      "action_slot": 1,
      "sequence_number": 1,
      "action_type": "work.freelance.completed",
      "ruleset_version_id": "uuid",
      "payload": {},
      "client_request_id": "optional-client-id"
    }
  ]
}
```

---

## 10. Endpoint Analytics
### 10.1 Recompute Session Analytics
```http
POST /api/v1/analytics/sessions/{sessionId}/recompute
```

Otorisasi: `INSTRUCTOR`.

Response 200: ringkasan hasil recompute sesi.

### 10.2 Get Session Analytics
```http
GET /api/v1/analytics/sessions/{sessionId}
```

Otorisasi: `INSTRUCTOR`, `PLAYER`.

Response 200:

```json
{
  "session_id": "uuid",
  "summary": {
    "event_count": 120,
    "cash_in_total": 200,
    "cash_out_total": 150,
    "cashflow_net_total": 50,
    "rules_violations_count": 2
  },
  "by_player": [
    {
      "user_id": "uuid",
      "player_order_no": 1,
      "cash_in_total": 120,
      "cash_out_total": 90,
      "donation_total": 10,
      "gold_qty": 2,
      "orders_completed_count": 3,
      "inventory_ingredient_total": 4,
      "actions_used_total": 12,
      "compliance_primary_need_rate": 0.8,
      "rules_violations_count": 0,
      "happiness_points_total": 14,
      "need_points_total": 6,
      "need_set_bonus_points": 4,
      "donation_points_total": 5,
      "gold_points_total": 3,
      "pension_points_total": 3,
      "saving_goal_points_total": 3,
      "mission_penalty_total": 10,
      "loan_penalty_total": 0,
      "has_unpaid_loan": false
    }
  ],
  "ruleset_id": "uuid",
  "ruleset_name": "Ruleset Kelas A"
}
```

### 10.3 Get Transaction History
```http
GET /api/v1/analytics/sessions/{sessionId}/transactions?userId=uuid
```

Otorisasi: `INSTRUCTOR`, `PLAYER`.

Response 200:

```json
{
  "items": [
    {
      "timestamp": "2026-06-18T10:00:00Z",
      "direction": "OUT",
      "amount": 5,
      "category": "NEED_PRIMARY"
    }
  ]
}
```

### 10.4 Get Gameplay Metrics
```http
GET /api/v1/analytics/sessions/{sessionId}/players/{userId}/gameplay
```

Otorisasi: `INSTRUCTOR`, `PLAYER`.

Response 200:

```json
{
  "session_id": "uuid",
  "user_id": "uuid",
  "computed_at": "2026-06-18T10:00:00Z",
  "economy": {
    "starting_cash": 20,
    "cash_in_total": 120,
    "cash_out_total": 90,
    "cashflow_net_total": 30,
    "donation_total": 10
  },
  "progress": {
    "gold_qty": 2,
    "orders_completed_count": 3,
    "inventory_ingredient_total": 4,
    "actions_used_total": 12
  },
  "score": {
    "happiness_points_total": 14,
    "need_points_total": 6,
    "need_set_bonus_points": 4,
    "donation_points_total": 5,
    "gold_points_total": 3,
    "pension_points_total": 3,
    "saving_goal_points_total": 3,
    "mission_penalty_total": 10,
    "loan_penalty_total": 0,
    "has_unpaid_loan": false
  },
  "compliance": {
    "primary_need_rate": 0.8,
    "rules_violations_count": 0
  }
}
```

### 10.5 Get Ruleset Analytics Summary
```http
GET /api/v1/analytics/rulesets/{rulesetId}/summary
```

Otorisasi: `INSTRUCTOR`, `PLAYER`.

Response 200:

```json
{
  "ruleset_id": "uuid",
  "ruleset_name": "Ruleset Kelas A",
  "session_count": 4,
  "learning_performance_aggregate_score": 72.5,
  "mission_performance_aggregate_score": 65.0,
  "sessions": [
    {
      "session_id": "uuid",
      "session_name": "Kelas A - Pertemuan 1",
      "status": "ENDED",
      "event_count": 120,
      "learning_performance_aggregate_score": 70.0,
      "mission_performance_aggregate_score": 66.0,
      "players": [
        {
          "user_id": "uuid",
          "learning_performance_individual_score": 74.0,
          "mission_performance_individual_score": 68.0
        }
      ]
    }
  ]
}
```

---

## 11. Endpoint Observability, Audit, dan Health
### 11.1 Observability Summary
```http
GET /api/v1/observability/metrics/summary
```

Otorisasi: `INSTRUCTOR`.

Response 200:

```json
{
  "message": "Metrics available at /metrics (Prometheus format)"
}
```

### 11.2 Prometheus Metrics
```http
GET /metrics
```

Format: Prometheus text exposition.

Catatan deployment:
- Endpoint ini tersedia pada service API.
- Pada production melalui Nginx publik, akses langsung dapat dibatasi. Verifikasi biasanya lewat container API.

### 11.3 Security Audit Logs
```http
GET /api/v1/security/audit-logs?limit=100&eventType=AUTH_FORBIDDEN&userId=uuid
```

Otorisasi: `INSTRUCTOR`.

Response 200:

```json
{
  "items": [
    {
      "security_audit_log_id": "uuid",
      "occurred_at": "2026-06-18T10:00:00Z",
      "trace_id": "trace-id",
      "event_type": "AUTH_FORBIDDEN",
      "outcome": "DENIED",
      "user_id": "uuid",
      "username": "player_a",
      "role": "PLAYER",
      "ip_address": "127.0.0.1",
      "user_agent": "PostmanRuntime",
      "method": "POST",
      "path": "/api/v1/rulesets",
      "status_code": 403,
      "detail": {}
    }
  ]
}
```

### 11.4 Health Checks
```http
GET /health/live
GET /health/ready
```

Otorisasi: publik atau operasional, sesuai konfigurasi hosting.

---

## 12. Matriks Endpoint Ringkas
| Endpoint | Method | Public | Instructor | Player | Catatan |
|---|---|---:|---:|---:|---|
| `/api/v1/auth/login` | POST | Ya | Ya | Ya | Login. |
| `/api/v1/auth/register` | POST | Ya | Ya | Ya | Register. |
| `/api/v1/sessions` | GET | Tidak | Ya | Ya | Scope role. |
| `/api/v1/sessions` | POST | Tidak | Ya | Tidak | Pakai `ruleset_version_id`. |
| `/api/v1/sessions/{id}/start` | POST | Tidak | Ya | Tidak | Start. |
| `/api/v1/sessions/{id}/end` | POST | Tidak | Ya | Tidak | End. |
| `/api/v1/sessions/{id}/state` | GET | Tidak | Ya | Tidak | Read projection. |
| `/api/v1/sessions/{id}/state` | PUT | Tidak | 410 | Tidak | Write disabled. |
| `/api/v1/players` | GET | Tidak | Ya | Ya | List Player scope. |
| `/api/v1/players` | POST | Tidak | Ya | Tidak | Create akun Player. |
| `/api/v1/sessions/{id}/players` | POST | Tidak | Ya | Tidak | Add Player by `user_id`/`username`. |
| `/api/v1/events` | POST | Tidak | Ya | Ya | Ingest event. |
| `/api/v1/events/batch` | POST | Tidak | Ya | Ya | Batch ingest. |
| `/api/v1/sessions/{id}/events` | GET | Tidak | Ya | Ya | Read event. |
| `/api/v1/rulesets` | GET | Tidak | Ya | Ya | List. |
| `/api/v1/rulesets` | POST | Tidak | Ya | Tidak | Create. |
| `/api/v1/rulesets/{id}` | GET | Tidak | Ya | Ya | Detail. |
| `/api/v1/rulesets/{id}` | PUT | Tidak | Ya | Tidak | New version. |
| `/api/v1/rulesets/{id}` | DELETE | Tidak | Ya | Tidak | Delete mutable ruleset. |
| `/api/v1/rulesets/{id}/versions/{version}/activate` | POST | Tidak | Ya | Tidak | Activate version. |
| `/api/v1/rulesets/{id}/versions/{version}` | DELETE | Tidak | Ya | Tidak | Delete version with guard. |
| `/api/v1/rulesets/{id}/components` | GET | Tidak | Ya | Ya | Component definition. |
| `/api/v1/rulesets/components/defaults` | GET | Tidak | Ya | Ya | Default components. |
| `/api/v1/game-components` | GET | Tidak | Ya | Ya | Alias default components. |
| `/api/v1/rulesets/sections` | GET | Tidak | Ya | Ya | Section format for UI/game. |
| `/api/v1/analytics/sessions/{id}/recompute` | POST | Tidak | Ya | Tidak | Recompute. |
| `/api/v1/analytics/sessions/{id}` | GET | Tidak | Ya | Ya | Session analytics. |
| `/api/v1/analytics/sessions/{id}/transactions` | GET | Tidak | Ya | Ya | Query `userId`. |
| `/api/v1/analytics/sessions/{id}/players/{userId}/gameplay` | GET | Tidak | Ya | Ya | Gameplay metrics. |
| `/api/v1/analytics/rulesets/{id}/summary` | GET | Tidak | Ya | Ya | Ruleset analytics. |
| `/api/v1/observability/metrics/summary` | GET | Tidak | Ya | Tidak | Operational summary. |
| `/api/v1/security/audit-logs` | GET | Tidak | Ya | Tidak | Security audit. |
| `/metrics` | GET | Tidak | Operasional | Operasional | Prometheus. |
| `/health/live`, `/health/ready` | GET | Ya | Ya | Ya | Health. |

---

## 13. Status Koleksi Postman Saat Ini
Koleksi `postman/Cashflowpoly.postman_collection.json` sudah disesuaikan dengan kontrak API baseline 18 Juni 2026.

Penyesuaian penting:
1. Request create/update ruleset memakai field `definition`.
2. Request create session memakai `ruleset_version_id`.
3. Request add Player to session memakai `user_id` dan `player_order_no`.
4. Request event memakai `user_id`, `action_slot`, `turn_number`, dan action type kanonis seperti `KerjaLepas`, `Kebutuhan`, `BahanMasakan`, `JualMasakan`, `JumatBerkah`, `InvestasiEmas`, dan `AkhirGiliran`.
5. Query transaksi memakai `userId={{playerUserId}}`.
6. Script Postman menyimpan `rulesetId`, `rulesetVersionId`, `rulesetVersionNumber`, `sessionId`, `playerUserId`, dan `secondPlayerUserId` dari response API.
7. Environment lokal memakai variabel `playerUserId` dan `secondPlayerUserId`, bukan variabel pemain lama.
8. Koleksi mencakup endpoint aktual:
   - `GET /api/v1/rulesets/sections`
   - `GET /api/v1/rulesets/components/defaults`
   - `GET /api/v1/game-components`
   - `POST /api/v1/rulesets/{rulesetId}/versions/{version}/activate`
   - `DELETE /api/v1/rulesets/{rulesetId}/versions/{version}`
   - `DELETE /api/v1/rulesets/{rulesetId}`
   - `GET /api/v1/observability/metrics/summary`
   - `GET /api/v1/security/audit-logs`
   - `GET /metrics`
   - `GET /api/v1/sessions/{sessionId}/state`
   - `PUT /api/v1/sessions/{sessionId}/state` untuk memverifikasi `410 STATE_WRITE_DISABLED`

Catatan operasional: koleksi ini disusun sebagai smoke test kontrak. Untuk skenario simulasi penuh, jalankan event berurutan sesuai dokumen `docs/01-Spesifikasi/01-06-skenario-simulasi-permainan.md` atau seed simulasi database.
