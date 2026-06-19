# Rancangan Kontrak API dan Event
## Sistem Informasi Dasbor Analitika Cashflowpoly

### Dokumen
- Nama dokumen: Rancangan Kontrak API dan Event
- Versi: 2.0
- Tanggal: 18 Juni 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan Dokumen
Dokumen ini disusun untuk menetapkan spesifikasi event sebagai format data utama pencatatan permainan serta menetapkan kontrak REST API untuk menerima, memvalidasi, menyimpan, dan menyediakan data analitika serta lifecycle sesi/ruleset/Player yang dipakai Klien Game/IDN. Dokumen ini menjadi acuan implementasi back-end, integrasi Klien Game/IDN, Web Analitik MVC, serta pengujian fungsional.

Jika ada konflik detail antara dokumen ini dan dokumen lain, prioritas acuan:
1. `docs/01-Spesifikasi/01-03-spesifikasi-integrasi-dan-keamanan.md`
2. dokumen ini (`02-02`)
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
- API me-resolve `action_type`/kode action ke
  `ruleset_actions.ruleset_action_id` sebelum menyimpan event.
- Kode asset gameplay penting seperti ingredient, order, need, risk, gold,
  gold price, dan tie breaker tetap boleh dikirim sebagai kode yang mudah
  dibaca. API me-resolve kode tersebut ke UUID `ruleset_game_assets` dan
  mencatat relasinya pada `event_asset_references`.
- `payload` tetap disimpan sebagai raw event untuk replay dan audit. State,
  balance, inventory, collection mission, financial goal, narrative, score, dan metric snapshot adalah
  projection/cache yang dapat dibangun ulang.
- API tidak menyimpan action log kedua dan tidak menjalankan script
  interpreter untuk narrative. Quest keluar dari MVP.

---

## 3. Model Event Umum
### 3.1 Struktur event
Setiap event dikirim sebagai JSON dengan skema umum berikut:

| Field | Tipe | Wajib | Deskripsi |
|---|---|---:|---|
| event_id | string (UUID) | Ya | ID unik event. |
| session_id | string (UUID) | Ya | ID sesi permainan. |
| user_id | string (UUID) | Ya* | ID akun Player (`app_users.user_id`). Wajib untuk event aksi Player. Kosong untuk event sistem. |
| actor_type | string | Ya | Nilai: `PLAYER` atau `SYSTEM`. |
| timestamp | string (ISO 8601) | Ya | Waktu event terjadi. |
| day_index | int | Ya | Indeks hari dalam sesi. |
| weekday | string | Ya | Nilai: `MON,TUE,WED,THU,FRI,SAT,SUN`. |
| turn_number | int | Ya | Nomor giliran yang dipakai validator urutan pemain. |
| action_slot | int | Ya | Slot aksi pemain pada hari tersebut, bernilai 1 atau 2. |
| action_type | string | Ya | Jenis event. |
| sequence_number | long | Ya | Nomor urut event per sesi. |
| ruleset_version_id | string (UUID) | Ya | Versi ruleset yang aktif saat event terjadi. |
| payload | object | Ya | Detail event sesuai `action_type`. |
| client_request_id | string | Tidak | ID request dari klien untuk tracing. |

Catatan:
- `user_id` wajib saat `actor_type=PLAYER`.
- API me-resolve `user_id` ke `session_participant_id`/`session_player_id`
  pada sesi sebelum event disimpan. Event sistem memakai `user_id = null`.
- `timestamp` harus format UTC atau menyertakan offset zona waktu.
- `day_index` minimal `0`.
- `turn_number` minimal `0`.
- `action_slot` minimal `1`.
- `sequence_number` minimal `0`.

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
#### 4.1.1 `MulaiSesi`
Payload:
```json
{ "start_note": "Mulai sesi" }
```

Validasi:
- Status sesi harus `CREATED`.

Efek data:
- Mengubah status sesi menjadi `STARTED`.

---

#### 4.1.2 `AkhiriSesi`
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
#### 4.2.1 `AkhirGiliran`
Payload:
```json
{ "note": "Akhir giliran" }
```

Validasi:
- `turn_number` harus cocok dengan urutan pemain aktif.
- `action_slot` wajib berada dalam batas `ruleset_game_settings.actions_per_turn`.
- Pada mode MAHIR, jumlah `RisikoKehidupan` per pemain harus sama dengan jumlah `JualMasakan` pada giliran yang sama.

Efek data:
- Menandai akhir aksi/giliran pemain.

---

### 4.3 Event transaksi dan arus kas
#### 4.3.1 `CatatTransaksi`
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
#### 4.4.1 `JumatBerkah`
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

#### 4.4.2 `InvestasiEmas`
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
- Jika `trade_type` dikirim, nilainya wajib `BUY`.
- `qty > 0`.
- `amount = unit_price * qty`.
- Sistem menolak jika saldo tidak cukup.

Efek data:
- Mengurangi saldo dan menambah kepemilikan emas.

---

#### 4.4.3 `JualEmas`
Payload:
```json
{
  "trade_type": "SELL",
  "unit_price": 6,
  "qty": 2,
  "amount": 12
}
```

Validasi:
- `weekday` harus `SAT`.
- Jika `trade_type` dikirim, nilainya wajib `SELL`.
- `qty > 0`.
- `amount = unit_price * qty`.
- Sistem menolak jika kepemilikan emas kurang.

Efek data:
- Menambah saldo dan mengurangi kepemilikan emas.

---

### 4.5 Event kebutuhan dan bahan (contoh minimal)
#### 4.5.1 `Kebutuhan`
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
- Tipe kebutuhan dibaca dari katalog ruleset atau payload `need_tier`.

---

#### 4.5.2 `BahanMasakan`
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

#### 4.5.3 `BuangBahanMasakan`
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

#### 4.5.4 `JualMasakan`
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

#### 4.5.5 `LewatiOrder`
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

#### 4.5.6 `KerjaLepas`
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
#### 4.6.1 `PinjamanSyariah`
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

#### 4.6.2 `BayarPinjaman`
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

#### 4.6.3 `Asuransi`
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

### 4.7 Event Misi dan Skor
#### 4.7.1 `BagikanMisiKoleksi`
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

#### 4.7.2 `PoinPeringkatDonasi`
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

#### 4.7.3 `UmumkanJuaraDonasi`
Payload:
```json
{
  "summary": "Manalu Juara 1, Marcello Juara 2, Marco Juara 3",
  "winners": [
    { "rank": 1, "player_name": "Manalu", "player_order_no": 4, "points": 7 },
    { "rank": 2, "player_name": "Marcello", "player_order_no": 2, "points": 5 },
    { "rank": 3, "player_name": "Marco", "player_order_no": 1, "points": 2 }
  ]
}
```

Validasi:
- `actor_type` wajib `SYSTEM`.
- `user_id` wajib kosong.
- `winners` wajib berisi 1 sampai 3 item.
- `rank` wajib berurutan mulai dari 1.
- `player_name` wajib.
- `points >= 0`.

Efek data:
- Event audit/timeline untuk menampilkan ringkasan juara donasi.
- Tidak menambah poin agar skor tidak terhitung ganda; poin tetap berasal dari `PoinPeringkatDonasi`.

---

#### 4.7.4 `PoinEmas`
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

#### 4.7.5 `PoinPeringkatPensiun`
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

#### 4.7.6 `TujuanFinansial`
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

#### 4.7.7 `BagikanTieBreaker`
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

### 4.8 Event Tabungan dan Risiko
#### 4.8.1 `Menabung`
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

#### 4.8.2 `TarikTabungan`
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

#### 4.8.3 `RisikoKehidupan`
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
- Sistem menolak jika jumlah `RisikoKehidupan` melebihi jumlah `JualMasakan` pemain pada giliran yang sama.

Efek data:
- Menambah/mengurangi saldo sesuai `direction`.

---

#### 4.8.4 `Asuransi`
Payload:
```json
{
  "risk_event_id": "uuid-event"
}
```

Validasi:
- `risk_event_id` wajib.
- `risk_event_id` harus merujuk ke event `RisikoKehidupan` bertipe OUT milik pemain yang sama.

Efek data:
- Menandai penggunaan asuransi terhadap kartu risiko.

---

#### 4.8.5 `GunakanOpsiDarurat`
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
- `risk_event_id` wajib dan harus merujuk ke event `RisikoKehidupan` bertipe OUT milik pemain yang sama.
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
- Retry/idempotency klien mengikuti dokumen `01-03` (bagian retry/backoff/timeouts).

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
  "display_name": "Ibu Rina",
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
  "role": "PLAYER",
  "display_name": "Player A"
}
```
- Response 201:
```json
{
  "user_id": "uuid",
  "username": "player_a",
  "role": "PLAYER",
  "display_name": "Player A",
  "access_token": "jwt",
  "expires_at": "2026-02-08T12:00:00Z"
}
```
- Catatan kebijakan:
  - Registrasi publik role `PLAYER` diperbolehkan.
  - Registrasi publik role `INSTRUCTOR` diperbolehkan.

---

## 6. Endpoint Session
Catatan akses:
- Semua endpoint session mensyaratkan token Bearer.
- `GET /api/v1/sessions` dapat dipakai Instruktur dan Player dengan scope data
  berbeda.
- Endpoint mutasi session hanya untuk role `INSTRUCTOR`.

### 6.1 Ambil daftar sesi
- Method: `GET`
- Path: `/api/v1/sessions`
- Otorisasi: `INSTRUCTOR` atau `PLAYER`
- Response 200:
```json
{
  "items": [
    {
      "session_id": "uuid",
      "session_name": "Kelas A - Pertemuan 1",
      "mode": "PEMULA",
      "status": "CREATED",
      "created_at": "2026-02-08T10:00:00Z",
      "started_at": null,
      "ended_at": null
    }
  ]
}
```

Scope:
- Instruktur melihat sesi miliknya.
- Player melihat sesi yang memiliki peserta dengan `user_id` miliknya.

---

### 6.2 Buat sesi
- Method: `POST`
- Path: `/api/v1/sessions`
- Otorisasi: `INSTRUCTOR`
- Request:
```json
{
  "session_name": "Kelas A - Pertemuan 1",
  "mode": "PEMULA",
  "ruleset_version_id": "uuid"
}
```
- Response 201:
```json
{
  "session_id": "uuid",
  "ruleset_id": "uuid",
  "ruleset_version_id": "uuid"
}
```

Status code:
- 201 Created
- 400 Validation Error
- 404 Ruleset tidak ditemukan

---

### 6.3 Mulai sesi
- Method: `POST`
- Path: `/api/v1/sessions/{sessionId}/start`
- Otorisasi: `INSTRUCTOR`
- Response 200:
```json
{ "status": "STARTED" }
```

---

### 6.4 Akhiri sesi
- Method: `POST`
- Path: `/api/v1/sessions/{sessionId}/end`
- Otorisasi: `INSTRUCTOR`
- Response 200:
```json
{ "status": "ENDED" }
```

---

### 6.5 Ambil state sesi
- Method: `GET`
- Path: `/api/v1/sessions/{sessionId}/state`
- Otorisasi: `INSTRUCTOR`
- Response 200 (ringkas):
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
      "saving": 0
    }
  ],
  "donationEvents": []
}
```

---

### 6.6 Tulis state sesi
- Method: `PUT`
- Path: `/api/v1/sessions/{sessionId}/state`
- Otorisasi: `INSTRUCTOR`
- Status: selalu `410 Gone` bila sesi ditemukan.
- Response:
```json
{
  "error_code": "STATE_WRITE_DISABLED",
  "message": "State permainan hanya dapat diubah melalui event ingestion",
  "details": [],
  "trace_id": "00-...-..."
}
```

Catatan: state gameplay tidak ditulis langsung. Semua perubahan state berasal
dari event valid pada `POST /api/v1/events` atau `POST /api/v1/events/batch`.

---

## 7. Endpoint Player
Catatan akses:
- Semua endpoint Player mensyaratkan token Bearer.
- Mutasi Player hanya untuk role `INSTRUCTOR`.
- Resource Player merepresentasikan akun `app_users` role `PLAYER`.

### 7.1 Buat akun Player
- Method: `POST`
- Path: `/api/v1/players`
- Otorisasi: `INSTRUCTOR`
- Request:
```json
{
  "display_name": "Player A",
  "username": "player_a",
  "password": "your-strong-password"
}
```
- Response 201:
```json
{
  "user_id": "uuid",
  "display_name": "Player A"
}
```

---

### 7.2 Ambil daftar Player
- Method: `GET`
- Path: `/api/v1/players`
- Otorisasi: `INSTRUCTOR` atau `PLAYER`
- Response 200:
```json
{
  "items": [
    { "user_id": "uuid", "display_name": "Player A" }
  ]
}
```

Scope:
- Instruktur melihat Player yang dapat dikelola.
- Player melihat data sesuai scope sesi yang diizinkan.

---

### 7.3 Tambah Player ke sesi
- Method: `POST`
- Path: `/api/v1/sessions/{sessionId}/players`
- Otorisasi: `INSTRUCTOR`
- Request:
```json
{
  "user_id": "uuid",
  "player_order_no": 1
}
```

Alternatif lookup:
```json
{
  "username": "player_a",
  "player_order_no": 1
}
```

- Response 200:
```json
{
  "user_id": "uuid",
  "player_order_no": 1
}
```

Efek data:
- Menambah atau memperbarui baris `session_participants`.
- Menginisialisasi projection awal peserta sesi.

---

## 8. Endpoint Event
### 8.1 Kirim event tunggal
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

### 8.2 Kirim event batch
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

### 8.3 Ambil event per sesi
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

## 9. Endpoint Ruleset
Catatan akses:
- Endpoint mutasi ruleset dan aktivasi ruleset mensyaratkan role `INSTRUCTOR` melalui token Bearer. Endpoint ini dapat dipakai oleh Web Analitik MVC, Klien Game/IDN, atau integrasi API untuk kebutuhan manajemen ruleset Instruktur.

### 9.1 Buat ruleset
- Method: `POST`
- Path: `/api/v1/rulesets`
- Request:
```json
{
  "name": "Ruleset Default",
  "description": "Konfigurasi awal",
  "definition": { "..." }
}
```

Isi field `definition` mengikuti struktur JSON pada `docs/01-Spesifikasi/01-02-spesifikasi-ruleset-dan-validasi.md` bagian 4.1. Saat disimpan, API menormalisasi definisi tersebut ke tabel `ruleset_*`. Contoh lengkap mode pemula dan mahir tersedia pada bagian 9 dokumen yang sama.

- Response 201:
```json
{ "ruleset_id": "uuid", "ruleset_version_id": "uuid", "version": 1 }
```

---

### 9.2 Update ruleset (menciptakan versi baru)
- Method: `PUT`
- Path: `/api/v1/rulesets/{rulesetId}`
- Request:
```json
{
  "name": "Ruleset Default Revisi",
  "description": "Konfigurasi revisi",
  "definition": { "..." }
}
```
- Response 200:
```json
{ "ruleset_id": "uuid", "ruleset_version_id": "uuid", "version": 2 }
```

---

### 9.3 Aktivasi versi ruleset
- Method: `POST`
- Path: `/api/v1/rulesets/{rulesetId}/versions/{version}/activate`
- Response 200:
```json
{ "ruleset_id": "uuid", "ruleset_version_id": "uuid", "version": 2 }
```

Catatan: session tidak memiliki endpoint aktivasi ruleset. Sesi memilih
`ruleset_version_id` langsung saat dibuat.

---

### 9.4 Hapus versi ruleset
- Method: `DELETE`
- Path: `/api/v1/rulesets/{rulesetId}/versions/{version}`
- Otorisasi: `INSTRUCTOR`
- Response: `204 No Content`

Aturan:
- Versi `ACTIVE` tidak boleh dihapus.
- Versi terakhir tidak boleh dihapus via delete version.
- Versi yang sudah dipakai sesi/event tidak boleh dihapus.

---

### 9.5 Hapus ruleset
- Method: `DELETE`
- Path: `/api/v1/rulesets/{rulesetId}`
- Otorisasi: `INSTRUCTOR`
- Response: `204 No Content`

Aturan:
- Ruleset default read-only dan tidak boleh dihapus.
- Ruleset milik instruktur tidak boleh dihapus bila sudah terkunci sesi.

---

### 9.6 Ambil daftar ruleset
- Method: `GET`
- Path: `/api/v1/rulesets`
- Response 200:
```json
{
  "items": [
    {
      "ruleset_id": "uuid",
      "name": "Ruleset Default",
      "latest_version": 2,
      "status": "ACTIVE",
      "is_default": true,
      "is_locked_by_session": false
    }
  ]
}
```

Keterangan:
- Field `status` merepresentasikan status pada **versi terbaru** ruleset (`DRAFT`/`ACTIVE`/`ARCHIVED`).

---

### 9.7 Ambil detail ruleset
- Method: `GET`
- Path: `/api/v1/rulesets/{rulesetId}`
- Response 200 (ringkas):
```json
{
  "ruleset_id": "uuid",
  "name": "Ruleset Default",
  "description": "Konfigurasi awal",
  "versions": [
    { "ruleset_version_id": "uuid", "version": 1, "status": "ACTIVE", "created_at": "2026-02-08T10:00:00Z" }
  ],
  "ruleset_version_id": "uuid",
  "version": 1,
  "mode": "PEMULA",
  "definition": { "...": "..." },
  "is_default": true,
  "is_locked_by_session": false
}
```

---

### 9.8 Ambil komponen ruleset
- Method: `GET`
- Path: `/api/v1/rulesets/{rulesetId}/components?version=1`
- Response 200:
```json
{
  "ruleset_id": "uuid",
  "ruleset_version_id": "uuid",
  "version": 1,
  "mode": "PEMULA",
  "definition": { "...": "..." }
}
```

---

### 9.9 Ambil komponen default
- Method: `GET`
- Path: `/api/v1/rulesets/components/defaults?mode=PEMULA`
- Alias: `GET /api/v1/game-components`
- Response 200:
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

---

### 9.10 Ambil section ruleset
- Method: `GET`
- Path: `/api/v1/rulesets/sections?mode=PEMULA&rulesetId=uuid`
- Response 200:
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

## 10. Endpoint Metrics dan Dashboard
### 10.1 Ambil metrik sesi
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
  ]
}
```

---

### 10.2 Ambil histori transaksi
- Method: `GET`
- Path: `/api/v1/analytics/sessions/{sessionId}/transactions?userId=uuid`
- Response 200:
```json
{
  "items": [
    { "timestamp":"...", "direction":"OUT", "amount":5, "category":"NEED_PRIMARY" }
  ]
}
```

---

### 10.3 Ambil snapshot metrik gameplay
- Method: `GET`
- Path: `/api/v1/analytics/sessions/{sessionId}/players/{userId}/gameplay`
- Response 200:
```json
{
  "session_id": "uuid",
  "user_id": "uuid",
  "computed_at": "2026-02-03T11:20:00Z",
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

Catatan:
- Response gameplay API saat ini disajikan dalam empat kelompok:
  `economy`, `progress`, `score`, dan `compliance`.
- Snapshot JSON mentah/turunan tetap dapat disimpan pada `metric_snapshots`
  dengan nama `gameplay.raw.variables` dan `gameplay.derived.metrics` sebagai
  sumber perhitungan.
- Struktur lengkap mengikuti dokumen `docs/02-Perancangan/02-03-rancangan-definisi-dan-agregasi-metrik.md`.

---

### 10.4 Ambil ringkasan analitika per ruleset
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

### 10.5 Ambil metrik operasional observability
- Method: `GET`
- Path: `/api/v1/observability/metrics/summary`
- Otorisasi: `INSTRUCTOR`
- Response 200 (ringkas):
```json
{
  "message": "Metrics available at /metrics (Prometheus format)"
}
```

Catatan: metrik operasional detail tersedia pada `GET /metrics` dalam format
Prometheus pada service API.

---

### 10.6 Ambil audit log keamanan
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

## 11. Status Code dan Makna
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

## 12. Checklist Konsistensi (Event-API-Data)
Dokumen ini konsisten jika:
1. Setiap event memiliki definisi payload dan validasi.
2. Setiap endpoint memiliki request/response dan status code.
3. Setiap validasi domain dapat ditelusuri ke aturan ruleset atau aturan permainan.
4. Setiap endpoint yang dipakai UI memiliki kebutuhan data yang tersedia.



