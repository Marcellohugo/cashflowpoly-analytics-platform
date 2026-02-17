# Spesifikasi Event dan Kontrak REST API  
## Sistem Informasi Dasbor Analitika & Manajemen Ruleset Cashflowpoly

### Dokumen
- Nama dokumen: Spesifikasi Event dan Kontrak REST API
- Versi: 1.2
- Tanggal: 8 Februari 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan Dokumen
Dokumen ini disusun untuk menetapkan spesifikasi event sebagai format data utama pencatatan permainan serta menetapkan kontrak REST API untuk menerima, memvalidasi, menyimpan, dan menyediakan data analitika serta manajemen ruleset. Dokumen ini menjadi acuan implementasi back-end dan acuan integrasi UI MVC serta pengujian fungsional.

Jika ada konflik detail antara dokumen ini dan dokumen lain, prioritas acuan:
1. `docs/01-Spesifikasi/01-04-kontrak-integrasi-idn-dan-keamanan.md`
2. dokumen ini (`01-02`)
3. dokumen pengujian (`03-01`)

---

## 2. Prinsip Desain Event
### 2.1 Event sebagai sumber kebenaran
Sistem memperlakukan event sebagai sumber utama untuk membentuk histori dan menghitung state serta metrik. Sistem tidak mengandalkan input agregat dari klien.

### 2.2 Idempotensi
Sistem menolak event duplikat berdasarkan kombinasi `session_id` dan `event_id`. Klien dapat mengirim ulang request yang sama tanpa menimbulkan dampak ganda.

### 2.3 Keterurutan
Sistem memproses event sesuai urutan `sequence_number` per sesi. Sistem menolak event dengan `sequence_number` lebih kecil dari event terakhir pada sesi maupun yang melompati urutan (gap) dari event terakhir.

### 2.4 Jejak ruleset
Setiap event wajib menyertakan `ruleset_version_id` agar analisis tetap konsisten walau ruleset berubah.

### 2.5 Catatan implementasi server (tanpa mengubah kontrak payload)
- API menyimpan event dengan `event_pk` sebagai primary key internal.
- API menerapkan idempotensi berdasarkan kombinasi `session_id + event_id`.
- API menolak `sequence_number` duplikat dalam satu sesi.

---

## 3. Model Event Umum
### 3.1 Struktur event
Setiap event dikirim sebagai JSON dengan skema umum berikut:

| Field | Tipe | Wajib | Deskripsi |
|---|---|---:|---|
| event_id | string (UUID) | Ya | ID unik event. |
| session_id | string (UUID) | Ya | ID sesi permainan. |
| player_id | string (UUID) | Ya* | ID pemain. Wajib untuk event aksi pemain. Kosong untuk event sistem. |
| actor_type | string | Ya | Nilai: `PLAYER` atau `SYSTEM`. |
| timestamp | string (ISO 8601) | Ya | Waktu event terjadi. |
| day_index | int | Ya | Indeks hari dalam sesi. |
| weekday | string | Ya | Nilai: `MON,TUE,WED,THU,FRI,SAT,SUN`. |
| turn_number | int | Ya | Nomor giliran pada sesi. |
| action_type | string | Ya | Jenis event. |
| sequence_number | long | Ya | Nomor urut event per sesi. |
| ruleset_version_id | string (UUID) | Ya | Versi ruleset yang aktif saat event terjadi. |
| payload | object | Ya | Detail event sesuai `action_type`. |
| client_request_id | string | Tidak | ID request dari klien untuk tracing. |

Catatan:
- `player_id` wajib saat `actor_type=PLAYER`.
- `timestamp` harus format UTC atau menyertakan offset zona waktu.

### 3.2 Struktur respons error (standar)
Semua error validasi mengikuti format ini:

```json
{
  "error_code": "VALIDATION_ERROR",
  "message": "Field wajib tidak lengkap",
  "details": [
    { "field": "session_id", "issue": "REQUIRED" },
    { "field": "payload.amount", "issue": "OUT_OF_RANGE" }
  ],
  "trace_id": "00-...-..."
}
```

---

## 4. Katalog Event dan Spesifikasi Payload
Bagian ini mendefinisikan event yang digunakan sistem. Sistem dapat menambah event baru, namun event baru wajib mengikuti struktur event umum dan aturan validasi.

### 4.1 Event sesi
#### 4.1.1 `session.created`
Tujuan: Membuat sesi permainan.

Payload:
```json
{
  "mode": "PEMULA",
  "session_name": "Kelas A - Pertemuan 1"
}
```

Validasi:
- `payload.mode` bernilai `PEMULA` atau `MAHIR`.
- `payload.session_name` panjang 1–100.

Efek data:
- Membuat record `Session`.

---

#### 4.1.2 `session.started`
Payload:
```json
{ "start_note": "Mulai sesi" }
```

Validasi:
- Status sesi harus `CREATED`.

Efek data:
- Mengubah status sesi menjadi `STARTED`.

---

#### 4.1.3 `session.ended`
Payload:
```json
{ "end_note": "Selesai sesi" }
```

Validasi:
- Status sesi harus `STARTED`.

Efek data:
- Mengubah status sesi menjadi `ENDED`.

---

### 4.2 Event giliran
#### 4.2.1 `turn.started`
Payload:
```json
{
  "action_tokens": 2
}
```

Validasi:
- `payload.action_tokens` sama dengan parameter ruleset `actions_per_turn`.

Efek data:
- Menandai awal giliran.

---

#### 4.2.2 `turn.action.used`
Payload:
```json
{
  "used": 1,
  "remaining": 1
}
```

Validasi:
- `remaining >= 0`.
- Total penggunaan pada giliran tidak melebihi `actions_per_turn`.

Efek data:
- Memperbarui penghitung token aksi.

---

#### 4.2.3 `turn.ended`
Payload:
```json
{ "note": "Akhir giliran" }
```

Validasi:
- Giliran harus sudah dimulai.
- Pada mode MAHIR, jumlah `risk.life.drawn` per pemain harus sama dengan jumlah `order.claimed` pada giliran yang sama.

Efek data:
- Menandai akhir giliran.

---

### 4.3 Event transaksi dan arus kas
#### 4.3.1 `transaction.recorded`
Tujuan: Mencatat transaksi pemasukan/pengeluaran koin.

Payload:
```json
{
  "direction": "OUT",
  "amount": 5,
  "category": "NEED_PRIMARY",
  "counterparty": "BANK",
  "reference": "CARD-NEED-001",
  "note": "Beli kebutuhan primer"
}
```

Validasi:
- `direction` bernilai `IN` atau `OUT`.
- `amount > 0`.
- `category` termasuk enumerasi kategori transaksi.
- `counterparty` termasuk `BANK` atau `PLAYER`.
- Sistem menolak transaksi `OUT` jika saldo tidak cukup (bergantung ruleset).

Efek data:
- Menambah record transaksi atau memproyeksikan dari event.
- Mengubah saldo pada state.

---

### 4.4 Event aturan harian
#### 4.4.1 `day.friday.donation`
Payload:
```json
{
  "amount": 2
}
```

Validasi:
- `weekday` harus `FRI`.
- `amount >= donation_min`.
- `amount > 0`.

Efek data:
- Mengurangi saldo.
- Menambah total donasi pemain.

---

#### 4.4.2 `day.saturday.gold_trade`
Payload:
```json
{
  "trade_type": "BUY",
  "unit_price": 6,
  "qty": 2,
  "amount": 12
}
```

Validasi:
- `weekday` harus `SAT`.
- `trade_type` bernilai `BUY` atau `SELL`.
- `qty > 0`.
- `amount = unit_price * qty`.
- Sistem menolak BUY jika saldo tidak cukup.
- Sistem menolak SELL jika kepemilikan emas kurang.

Efek data:
- Mengubah saldo dan kepemilikan emas.

---

### 4.5 Event kebutuhan dan bahan (contoh minimal)
#### 4.5.1 `need.primary.purchased`
Payload:
```json
{
  "card_id": "NEED-001",
  "amount": 5,
  "points": 2
}
```

Validasi:
- `weekday` tidak membatasi, namun sistem membatasi maksimal 1 pembelian kebutuhan primer per hari.
- Sistem menolak pembelian kebutuhan non-primer jika kebutuhan primer belum terpenuhi pada hari itu.
- `amount > 0`.
- `card_id` wajib diisi.
- `points` wajib diisi (nilai poin pada kartu kebutuhan).

Efek data:
- Mengurangi saldo.
- Menambah kepemilikan kartu kebutuhan.

Catatan:
- Struktur payload `need.secondary.purchased` dan `need.tertiary.purchased` sama dengan `need.primary.purchased`.

---

#### 4.5.2 `ingredient.purchased`
Payload:
```json
{
  "card_id": "ING-012",
  "ingredient_name": "Beras",
  "amount": 1
}
```

Validasi:
- Total kartu bahan tidak melebihi 6.
- Kartu bahan yang sama tidak melebihi 3.
- `amount > 0`.

Efek data:
- Mengurangi saldo.
- Menambah inventori bahan.

---

#### 4.5.3 `ingredient.discarded`
Payload:
```json
{
  "card_id": "ING-012",
  "amount": 1,
  "reason": "HAND_LIMIT"
}
```

Validasi:
- `amount > 0`.
- `card_id` wajib diisi.
- Pemain harus memiliki stok bahan yang cukup untuk didiscard.

Efek data:
- Mengurangi inventori bahan.

---

#### 4.5.4 `order.claimed`
Payload:
```json
{
  "order_card_id": "ORD-005",
  "required_ingredient_card_ids": ["ING-001", "ING-012"],
  "income": 15
}
```

Validasi:
- Pemain memiliki semua bahan pada daftar.
- `income > 0`.

Efek data:
- Mengurangi inventori bahan.
- Menambah saldo.

---

#### 4.5.5 `order.passed`
Payload:
```json
{
  "order_card_id": "ORD-006",
  "required_ingredient_card_ids": ["ING-003", "ING-010"],
  "income": 13
}
```

Validasi:
- `income > 0`.
- `required_ingredient_card_ids` wajib diisi.

Efek data:
- Tidak mengubah saldo.
- Menambah catatan order yang dilewati untuk analitika.

---

#### 4.5.6 `work.freelance.completed`
Payload:
```json
{
  "amount": 1
}
```

Validasi:
- `amount > 0`.
- `amount` harus sama dengan `freelance.income` pada ruleset aktif.

Efek data:
- Menambah saldo.

---

### 4.6 Event mode mahir (minimum)
#### 4.6.1 `loan.syariah.taken`
Payload:
```json
{
  "loan_id": "LOAN-001",
  "principal": 10,
  "installment": 10,
  "duration_turn": 1,
  "penalty_points": 15
}
```

Validasi:
- `principal > 0`.
- `installment > 0`.
- `duration_turn > 0`.
- `penalty_points >= 0`.
- `principal` harus **10** koin (sesuai rulebook).
- `penalty_points` harus **15** poin (sesuai rulebook).
- Sistem menolak jika aturan ruleset melarang pinjaman pada kondisi tertentu.

Efek data:
- Menambah saldo.
- Mencatat kewajiban cicilan.

---

#### 4.6.2 `loan.syariah.repaid`
Payload:
```json
{
  "loan_id": "LOAN-001",
  "amount": 5
}
```

Validasi:
- `amount > 0`.
- Pemain memiliki saldo cukup.
- Loan masih aktif.
- Sistem menolak pembayaran melebihi sisa pinjaman.
- `loan_id` harus terdaftar pada loan yang masih aktif.

Efek data:
- Mengurangi saldo.
- Mengurangi sisa kewajiban.

---

#### 4.6.3 `insurance.multirisk.purchased`
Payload:
```json
{
  "policy_id": "INS-001",
  "premium": 1,
  "coverage_type": "MULTIRISK"
}
```

Validasi:
- `premium > 0`.
- `premium` harus **1** koin (sesuai rulebook).

Efek data:
- Mengurangi saldo.
- Menambah status proteksi.

---

## 4.7 Event Misi dan Skor
#### 4.7.1 `mission.assigned`
Payload:
```json
{
  "mission_id": "MIS-001",
  "target_tertiary_card_id": "NEED-T-003",
  "penalty_points": 10,
  "require_primary": true,
  "require_secondary": true
}
```

Validasi:
- `mission_id` wajib.
- `target_tertiary_card_id` wajib.
- `penalty_points >= 0`.
- `penalty_points` harus **10** poin (sesuai rulebook).

Efek data:
- Menetapkan misi koleksi pemain untuk evaluasi di akhir sesi.

---

#### 4.7.2 `donation.rank.awarded`
Payload:
```json
{
  "rank": 1,
  "points": 7
}
```

Validasi:
- `rank > 0`.
- `points >= 0`.

Efek data:
- Menambah poin kebahagiaan kategori donasi.

---

#### 4.7.3 `gold.points.awarded`
Payload:
```json
{
  "points": 5
}
```

Validasi:
- `points >= 0`.

Efek data:
- Menambah poin kebahagiaan dari investasi emas.

---

#### 4.7.4 `pension.rank.awarded`
Payload:
```json
{
  "rank": 2,
  "points": 3
}
```

Validasi:
- `rank > 0`.
- `points >= 0`.

Efek data:
- Menambah poin kebahagiaan kategori dana pensiun.

---

#### 4.7.5 `saving.goal.achieved`
Payload:
```json
{
  "goal_id": "GOAL-001",
  "points": 5,
  "cost": 15
}
```

Validasi:
- `goal_id` wajib.
- `points >= 0`.
- `cost >= 0` (opsional, dipakai untuk validasi saldo tabungan).

Efek data:
- Menambah poin kebahagiaan dari tujuan keuangan (hanya berlaku jika pinjaman lunas).

---

#### 4.7.6 `tie_breaker.assigned`
Payload:
```json
{
  "number": 7
}
```

Validasi:
- `number > 0`.

Efek data:
- Menyimpan nomor tie breaker untuk pemecah seri.

---

## 4.8 Event Tabungan dan Risiko
#### 4.8.1 `saving.deposit.created`
Payload:
```json
{
  "goal_id": "GOAL-001",
  "amount": 5
}
```

Validasi:
- `goal_id` wajib.
- `amount > 0`.
- `amount` maksimal **15** koin per aksi (sesuai rulebook).
- Fitur tabungan tujuan aktif.

Efek data:
- Mengurangi saldo.
- Menambah saldo tabungan tujuan.

---

#### 4.8.2 `saving.deposit.withdrawn`
Payload:
```json
{
  "goal_id": "GOAL-001",
  "amount": 5
}
```

Validasi:
- `goal_id` wajib.
- `amount > 0`.
- Saldo tabungan mencukupi.

Efek data:
- Menambah saldo.
- Mengurangi saldo tabungan tujuan.

---

#### 4.8.3 `risk.life.drawn`
Payload:
```json
{
  "risk_id": "RISK-012",
  "direction": "OUT",
  "amount": 3,
  "note": "Biaya kesehatan"
}
```

Validasi:
- `risk_id` wajib.
- `direction` bernilai `IN` atau `OUT`.
- `amount > 0`.
- Hanya tersedia pada mode mahir.
- Sistem menolak jika jumlah `risk.life.drawn` melebihi jumlah `order.claimed` pemain pada giliran yang sama.

Efek data:
- Menambah/mengurangi saldo sesuai `direction`.

---

#### 4.8.4 `insurance.multirisk.used`
Payload:
```json
{
  "risk_event_id": "uuid-event"
}
```

Validasi:
- `risk_event_id` wajib.
- `risk_event_id` harus merujuk ke event `risk.life.drawn` bertipe OUT milik pemain yang sama.

Efek data:
- Menandai penggunaan asuransi terhadap kartu risiko.

---

#### 4.8.5 `risk.emergency.used`
Payload:
```json
{
  "risk_event_id": "uuid-event",
  "option_type": "SELL_NEED",
  "direction": "IN",
  "amount": 3,
  "note": "Menjual kartu kebutuhan untuk menutup risiko"
}
```

Validasi:
- `risk_event_id` wajib dan harus merujuk ke event `risk.life.drawn` bertipe OUT milik pemain yang sama.
- `option_type` bernilai `SELL_NEED`, `SELL_GOLD`, `SELL_GOAL`, atau `OTHER`.
- `direction` bernilai `IN` atau `OUT`.
- `amount > 0`.

Efek data:
- Menambah catatan penggunaan opsi darurat.
- Jika `direction = IN/OUT`, sistem memproyeksikan arus kas sesuai nilai `amount`.

---

## 5. Kontrak REST API
Kontrak berikut menjadi acuan Swagger dan pengujian.

### 5.1 Prinsip umum endpoint
- Semua endpoint mengirim dan menerima JSON.
- Sistem mengembalikan error sesuai format standar bagian 3.2.
- Sistem mengembalikan `trace_id` untuk pelacakan log.
- Endpoint terproteksi wajib mengirim `Authorization: Bearer <token>`.
- Endpoint publik tanpa token hanya endpoint autentikasi (`/api/v1/auth/login`, `/api/v1/auth/register`).
- Retry/idempotency klien mengikuti dokumen `01-04` (bagian retry/backoff/timeouts).

### 5.2 Endpoint autentikasi
#### 5.2.1 Login
- Method: `POST`
- Path: `/api/v1/auth/login`
- Request:
```json
{ "username": "instructor", "password": "your-strong-password" }
```
- Response 200 (minimum):
```json
{
  "user_id": "uuid",
  "username": "instructor",
  "role": "INSTRUCTOR",
  "access_token": "jwt",
  "expires_at": "2026-02-08T12:00:00Z"
}
```

#### 5.2.2 Register
- Method: `POST`
- Path: `/api/v1/auth/register`
- Request:
```json
{
  "username": "player_a",
  "password": "your-strong-password",
  "role": "PLAYER"
}
```
- Response 201:
```json
{
  "user_id": "uuid",
  "username": "player_a",
  "role": "PLAYER",
  "access_token": "jwt",
  "expires_at": "2026-02-08T12:00:00Z"
}
```
- Catatan kebijakan:
  - Registrasi publik role `PLAYER` diperbolehkan.
  - Registrasi publik role `INSTRUCTOR` ditentukan langsung oleh `Auth:AllowPublicInstructorRegistration` (`true` = diizinkan, `false` = ditolak `403`).

---

## 6. Endpoint Session
### 6.1 Buat sesi
- Method: `POST`
- Path: `/api/v1/sessions`
- Request:
```json
{
  "session_name": "Kelas A - Pertemuan 1",
  "mode": "PEMULA",
  "ruleset_id": "uuid"
}
```
- Response 201:
```json
{ "session_id": "uuid" }
```

Status code:
- 201 Created
- 400 Validation Error
- 404 Ruleset tidak ditemukan

---

### 6.2 Mulai sesi
- Method: `POST`
- Path: `/api/v1/sessions/{sessionId}/start`
- Response 200:
```json
{ "status": "STARTED" }
```

---

### 6.3 Akhiri sesi
- Method: `POST`
- Path: `/api/v1/sessions/{sessionId}/end`
- Response 200:
```json
{ "status": "ENDED" }
```

---

## 7. Endpoint Event
### 7.1 Kirim event tunggal
- Method: `POST`
- Path: `/api/v1/events`
- Request: sesuai struktur event umum bagian 3.1
- Response 201:
```json
{ "stored": true, "event_id": "uuid" }
```

Status code:
- 201 Created
- 400 Validation Error
- 404 Session tidak ditemukan
- 409 Duplicate event_id
- 422 Domain rule violated

---

### 7.2 Kirim event batch
- Method: `POST`
- Path: `/api/v1/events/batch`
- Request:
```json
{ "events": [ { ... }, { ... } ] }
```
- Response 200:
```json
{
  "stored_count": 10,
  "failed": [
    { "event_id": "uuid", "error_code": "VALIDATION_ERROR" }
  ]
}
```

---

### 7.3 Ambil event per sesi
- Method: `GET`
- Path: `/api/v1/sessions/{sessionId}/events?fromSeq=0&limit=200`
- Response 200:
```json
{
  "session_id": "uuid",
  "events": [ { ... }, { ... } ]
}
```

---

## 8. Endpoint Ruleset
Catatan akses:
- Endpoint manajemen ruleset dan aktivasi ruleset mensyaratkan role `INSTRUCTOR` melalui token Bearer.

### 8.1 Buat ruleset
- Method: `POST`
- Path: `/api/v1/rulesets`
- Request:
```json
{
  "name": "Ruleset Default",
  "description": "Konfigurasi awal",
  "config": {
    "mode": "PEMULA",
    "actions_per_turn": 2,
    "starting_cash": 20,
    "weekday_rules": {
      "friday": { "feature": "DONATION", "enabled": true },
      "saturday": { "feature": "GOLD_TRADE", "enabled": true },
      "sunday": { "feature": "REST", "enabled": true }
    },
    "constraints": {
      "cash_min": 0,
      "max_ingredient_total": 6,
      "max_same_ingredient": 3,
      "primary_need_max_per_day": 1,
      "require_primary_before_others": true
    },
    "donation": { "min_amount": 1, "max_amount": 999999 },
    "gold_trade": { "allow_buy": true, "allow_sell": true },
    "advanced": {
      "loan": { "enabled": false },
      "insurance": { "enabled": false },
      "saving_goal": { "enabled": false }
    },
    "freelance": { "income": 1 },
    "scoring": {
      "donation_rank_points": [
        { "rank": 1, "points": 7 },
        { "rank": 2, "points": 5 },
        { "rank": 3, "points": 2 }
      ],
      "gold_points_by_qty": [
        { "qty": 1, "points": 3 },
        { "qty": 2, "points": 5 },
        { "qty": 3, "points": 8 },
        { "qty": 4, "points": 12 }
      ],
      "pension_rank_points": [
        { "rank": 1, "points": 5 },
        { "rank": 2, "points": 3 },
        { "rank": 3, "points": 1 }
      ]
    }
  }
}
```
- Response 201:
```json
{ "ruleset_id": "uuid", "version": 1 }
```

---

### 8.2 Update ruleset (menciptakan versi baru)
- Method: `PUT`
- Path: `/api/v1/rulesets/{rulesetId}`
- Response 200:
```json
{ "ruleset_id": "uuid", "version": 2 }
```

---

### 8.3 Aktivasi ruleset untuk sesi
- Method: `POST`
- Path: `/api/v1/sessions/{sessionId}/ruleset/activate`
- Request:
```json
{ "ruleset_id": "uuid", "version": 2 }
```
- Response 200:
```json
{ "session_id": "uuid", "ruleset_version_id": "uuid" }
```

---

### 8.4 Ambil daftar ruleset
- Method: `GET`
- Path: `/api/v1/rulesets`
- Response 200:
```json
{ "items": [ { "ruleset_id":"uuid", "name":"Ruleset Default", "latest_version": 2 } ] }
```

---

## 9. Endpoint Metrics dan Dashboard
### 9.1 Ambil metrik sesi
- Method: `GET`
- Path: `/api/v1/analytics/sessions/{sessionId}`
- Response 200:
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
      "player_id": "uuid",
      "cash_in_total": 120,
      "cash_out_total": 90,
      "donation_total": 10,
      "gold_qty": 2,
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
  ]
}
```

---

### 9.2 Ambil histori transaksi
- Method: `GET`
- Path: `/api/v1/analytics/sessions/{sessionId}/transactions?playerId=uuid`
- Response 200:
```json
{
  "items": [
    { "timestamp":"...", "direction":"OUT", "amount":5, "category":"NEED_PRIMARY" }
  ]
}
```

---

### 9.3 Ambil snapshot metrik gameplay (raw + derived)
- Method: `GET`
- Path: `/api/v1/analytics/sessions/{sessionId}/players/{playerId}/gameplay`
- Response 200:
```json
{
  "session_id": "uuid",
  "player_id": "uuid",
  "computed_at": "2026-02-03T11:20:00Z",
  "raw": { "...": "..." },
  "derived": { "...": "..." }
}
```

Catatan:
- `raw` berisi variabel gameplay fisik.
- `derived` berisi metrik turunan dari variabel fisik.
- Struktur lengkap mengikuti dokumen `docs/02-Perancangan/02-06-metrik-gameplay-fisik-dan-turunan.md`.

---

### 9.4 Ambil ringkasan analitika per ruleset
- Method: `GET`
- Path: `/api/v1/analytics/rulesets/{rulesetId}/summary`
- Response 200:
```json
{
  "ruleset_id": "uuid",
  "ruleset_name": "Ruleset Default",
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
          "player_id": "uuid",
          "learning_performance_individual_score": 74.0,
          "mission_performance_individual_score": 68.0
        }
      ]
    }
  ]
}
```

---

### 9.5 Ambil metrik operasional observability
- Method: `GET`
- Path: `/api/v1/observability/metrics?top=20`
- Otorisasi: `INSTRUCTOR`
- Response 200 (ringkas):
```json
{
  "generated_at": "2026-02-17T10:30:00Z",
  "total_requests": 1200,
  "total_errors": 12,
  "error_rate_percent": 1.0,
  "endpoints": [
    {
      "method": "GET",
      "route_pattern": "api/v1/analytics/sessions/{sessionId:guid}",
      "request_count": 320,
      "error_count": 0,
      "error_rate_percent": 0,
      "average_duration_ms": 210.6,
      "p95_duration_ms": 842.1
    }
  ]
}
```

---

### 9.6 Ambil audit log keamanan
- Method: `GET`
- Path: `/api/v1/security/audit-logs?limit=100&eventType=AUTH_FORBIDDEN`
- Otorisasi: `INSTRUCTOR`
- Response 200:
```json
{
  "items": [
    {
      "security_audit_log_id": "uuid",
      "occurred_at": "2026-02-17T10:31:00Z",
      "trace_id": "trace-id",
      "event_type": "AUTH_FORBIDDEN",
      "outcome": "DENIED",
      "status_code": 403,
      "path": "/api/v1/rulesets",
      "method": "POST"
    }
  ]
}
```

---

## 10. Status Code dan Makna
| Status | Makna |
|---:|---|
| 200 | Request berhasil. |
| 201 | Data berhasil dibuat dan disimpan. |
| 400 | Struktur request tidak valid atau field wajib kosong. |
| 401 | Tidak terautentikasi. |
| 403 | Tidak berhak akses. |
| 404 | Resource tidak ditemukan. |
| 409 | Duplikasi data, terutama event idempotensi. |
| 422 | Aturan domain permainan dilanggar. |
| 429 | Request melebihi batas rate limit. |
| 500 | Kesalahan internal server. |

---

## 11. Checklist Konsistensi (Event–API–Data)
Dokumen ini konsisten jika:
1. Setiap event memiliki definisi payload dan validasi.
2. Setiap endpoint memiliki request/response dan status code.
3. Setiap validasi domain dapat ditelusuri ke aturan ruleset atau aturan permainan.
4. Setiap endpoint yang dipakai UI memiliki kebutuhan data yang tersedia.




