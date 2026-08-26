# Rancangan Kontrak API dan Event
## Sistem Informasi Dasbor Analitika Cashflowpoly

### Dokumen
- Nama dokumen: Rancangan Kontrak API dan Event
- Versi: 2.1
- Tanggal: 11 Juli 2026
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
| action_slot | int | Ya | `0` untuk event sistem/aksi gratis; `1..actions_per_turn` untuk aksi pemain yang memakai token. |
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
- `action_slot` minimal `0`.
- Event `SYSTEM` selalu memakai `turn_number=0` dan `action_slot=0`.
- Aksi gratis pemain memakai `action_slot=0`: `JumatBerkah`, `RisikoKehidupan`, `BayarRisiko`, `GunakanOpsiDarurat`, `InvestasiEmas`, `JualEmas`, dan `LewatiTransaksiEmas`.
- `Asuransi` dan `PinjamanSyariah` memakai `action_slot=0` hanya ketika payload membawa UUID `risk_event_id` yang valid; tanpa referensi risiko keduanya memakai token aksi reguler.
- Aksi pemain reguler memakai slot `1..actions_per_turn`. Kebijakan slot bersumber dari katalog kanonik `GameActionCatalog` dan dijaga kembali oleh database.
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
- Mengunci revisi setup terbaru yang sudah diperiksa Instruktur.
- Membentuk event setup dari Tie Breaker, bahan, emas, misi, dan komponen Mahir yang benar-benar dibagikan secara fisik.
- Menetapkan urutan peserta berdasarkan nomor Tie Breaker dan menginisialisasi projection secara atomik. Backend tidak mengacak deck/pasar atau membagikan kartu.

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
- `actor_type` wajib `SYSTEM`, `turn_number=0`, dan `action_slot=0`.
- Sebelum giliran berakhir, setiap `JualMasakan` mode MAHIR wajib sudah dirujuk tepat satu `RisikoKehidupan` melalui `source_order_event_id`.

Efek data:
- Menandai akhir aksi/giliran pemain.

---

#### 4.2.2 Batas pengetahuan kartu fisik
Instruktur mengelola deck, kartu terbuka, discard, dan refill langsung di meja sesuai rulebook. Backend hanya mengenali katalog ruleset, setup yang dikonfirmasi, serta perubahan kepemilikan yang dilaporkan lewat event sah. Event pengelolaan deck/pasar tidak tersedia pada endpoint ingest. Data legacy terkait deck/pasar tetap tersimpan untuk histori, tetapi diabaikan oleh proyeksi dan analitik baru.

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
- Satu pemain hanya boleh mengirim satu `JumatBerkah` pada `day_index` yang sama (`DONATION_ALREADY_SUBMITTED`).

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
- `weekday` harus `SAT`, kecuali payload membawa `risk_event_id` yang menunjuk risiko `GOLD_TRADE` valid dan masih aktif.
- Jika `trade_type` dikirim, nilainya wajib `BUY`.
- `qty > 0`.
- `amount = unit_price * qty`.
- `unit_price` wajib sama dengan event `BukaHargaEmas` terbaru pada `day_index` yang sama; harga Sabtu sebelumnya tidak berlaku.
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
- `weekday` harus `SAT`, kecuali payload membawa `risk_event_id` yang menunjuk risiko `GOLD_TRADE` valid dan masih aktif.
- Jika `trade_type` dikirim, nilainya wajib `SELL`.
- `qty > 0`.
- `amount = unit_price * qty`.
- `unit_price` wajib sama dengan event `BukaHargaEmas` terbaru pada `day_index` yang sama; harga Sabtu sebelumnya tidak berlaku.
- Sistem menolak jika kepemilikan emas kurang.
- Kepemilikan dibaca dari `session_participant_gold_holdings`, sehingga emas awal, pembelian, penjualan reguler, dan `SELL_GOLD` darurat dihitung dari state yang sama.

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
- Sistem menolak pembelian kebutuhan non-primer jika pemain belum pernah memenuhi kebutuhan primer pada sesi tersebut.
- `amount > 0`.
- `card_id` wajib diisi.
- `points` wajib diisi (nilai poin pada kartu kebutuhan).
- `card_id` wajib berasal dari katalog kebutuhan ruleset; ketersediaan kartu fisik dikonfirmasi IDN saat mengirim event.

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
- `card_id` wajib berasal dari katalog bahan ruleset; ketersediaan kartu fisik dikonfirmasi IDN saat mengirim event.

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
- `order_card_id` wajib berasal dari katalog ruleset; ketersediaan kartu fisik dikonfirmasi oleh IDN saat mengirim event.

Efek data:
- Mengurangi inventori bahan.
- Menambah saldo.

---

#### 4.5.5 `KerjaLepas`
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
  "loan_id": "loan_syariah_10",
  "loan_code": "loan_syariah_10",
  "principal": 10,
  "repayment_amount": 10,
  "duration_days": 1,
  "penalty_points": 15
}
```

Validasi:
- Detail principal, nilai pelunasan, durasi, dan penalti wajib sama dengan produk pada katalog ruleset aktif.
- Setiap kartu memakai `loan_id`/`loan_instance_id` unik. Beberapa instance produk yang sama dapat `ACTIVE` selama total kartu aktif pada sesi tidak melewati `card_qty` katalog.
- Jika memakai slot 0, payload wajib membawa `risk_event_id` yang menunjuk risiko `OUT` berstatus pending milik pemain dan saldo pemain memang tidak cukup.
- Satu risiko tidak dapat dipakai untuk mengambil pinjaman berulang.

Efek data:
- Menambah saldo.
- Membuat/mengaktifkan holding pada `session_participant_loans` dan mencatat penalti produk.

---

#### 4.6.2 `BayarPinjaman`
Payload:
```json
{
  "loan_id": "loan_syariah_10",
  "amount": 10
}
```

Validasi:
- `amount > 0`.
- Pemain memiliki saldo cukup.
- Loan masih aktif.
- `amount` wajib sama dengan seluruh `outstanding_amount`; cicilan parsial tidak didukung.
- `loan_id`/kode produk harus terdaftar pada loan yang masih aktif.

Efek data:
- Mengurangi saldo.
- Mengubah `outstanding_amount` menjadi 0 dan status menjadi `PAID`.

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
#### 4.7.1 `SetupMisiAwal`
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
- Selama sesi aktif, respons event untuk role `PLAYER` hanya membuka payload misi miliknya sendiri; payload misi pemain lain menjadi `{ "status": "HIDDEN" }`. Instruktur selalu dapat membaca payload lengkap dan seluruh misi terbuka setelah sesi berakhir.

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
  "number": 3
}
```

Validasi:
- `number` wajib berada pada rentang `1..jumlah pemain` dan unik dalam sesi.

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
  "risk_id": "risk_cost_4",
  "source_order_event_id": "uuid-event-jual-masakan",
  "note": "Biaya kesehatan"
}
```

Validasi:
- `risk_id` wajib.
- Nilai `effect_type`, `direction`, `amount`, target, dan durasi diambil dari katalog `ruleset_life_risks`, bukan dari klien.
- Hanya tersedia pada mode mahir.
- `source_order_event_id` wajib menunjuk `JualMasakan` pada sesi, pemain, dan hari yang sama.
- Satu event pesanan hanya boleh dipasangkan dengan satu event risiko.

Efek data:
- Risiko `OUT` pemain disimpan sebagai pending tanpa langsung mengurangi saldo.
- Risiko `OUT` menjadi selesai saat ada `BayarRisiko`, penggunaan asuransi, atau opsi darurat yang menyediakan dana cukup.
- Risiko bonus/non-pembayaran tetap diproyeksikan sesuai efek katalog.

---

#### 4.8.4 `BayarRisiko`
Payload:
```json
{
  "risk_event_id": "uuid-event"
}
```

Validasi:
- `risk_event_id` wajib.
- Referensi harus menunjuk risiko `OUT` pending milik pemain pada sesi yang sama.
- Saldo pemain harus cukup untuk membayar nilai risiko dari katalog.

Efek data:
- Membuat proyeksi `RISK_LIFE OUT` dan menandai risiko selesai.

---

#### 4.8.5 `Asuransi` untuk penyelesaian risiko
Payload:
```json
{
  "risk_event_id": "uuid-event"
}
```

Validasi:
- Polis pemain wajib berstatus `ACTIVE` dan `remaining_uses > 0`.
- `risk_event_id` harus menunjuk risiko `OUT` pending milik pemain yang sama.
- Asuransi dapat dipilih walaupun saldo pemain cukup.

Efek data:
- Membuat `INSURANCE_OFFSET IN` dan `RISK_LIFE OUT` dengan nominal yang sama.
- Mengurangi `remaining_uses`; polis menjadi `INACTIVE` ketika penggunaan habis.

---

#### 4.8.6 `GunakanOpsiDarurat`
Payload dasar:
```json
{
  "risk_event_id": "uuid-event",
  "option_type": "SELL_NEED",
  "need_card_id": "buku_1"
}
```

Validasi:
- `risk_event_id` wajib dan harus merujuk ke event `RisikoKehidupan` bertipe OUT milik pemain yang sama.
- `option_type` hanya bernilai `SELL_NEED`, `SELL_GOLD`, atau `TAKE_SHARIA_LOAN`.
- Klien tidak menentukan `direction` atau `amount`; server menghapus nilai tersebut dari request dan menghitung ulang berdasarkan katalog/state.
- `SELL_NEED` membutuhkan `need_card_id`/`card_id` yang dimiliki pemain; nilai jual adalah pembulatan ke bawah dari setengah harga beli dan kartu ditandai terjual.
- `SELL_GOLD` membutuhkan `qty` dan `gold_price_event_id`; server memeriksa holding serta event harga pada hari yang sama, lalu mengurangi holding.
- `TAKE_SHARIA_LOAN` membutuhkan `loan_code`; server memakai detail katalog dan membuat satu instance pinjaman baru selama stok kartu tersedia.
- Seluruh opsi darurat hanya tersedia ketika saldo sebelum penyelesaian tidak cukup untuk membayar risiko.

Efek data:
- `SELL_NEED`, `SELL_GOLD`, dan `TAKE_SHARIA_LOAN` membuat `EMERGENCY_OPTION IN` sesuai nilai hasil perhitungan server dan memperbarui aset/utang terkait.
- Risiko ditandai selesai ketika saldo setelah dana masuk cukup untuk membayar nilai risiko.
- Penggunaan polis tidak memakai `GunakanOpsiDarurat`; kontrak tunggalnya adalah event `Asuransi` pada bagian 4.8.5.

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
  - Registrasi publik role `INSTRUCTOR` ditolak; akun instruktur dibuat melalui bootstrap/admin.

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

### 6.3 Validasi, simpan, dan revisi pembagian awal
- Method: `POST`
- Path validasi tanpa penyimpanan: `/api/v1/sessions/{sessionId}/setup/validate`
- Path penyimpanan/revisi: `/api/v1/sessions/{sessionId}/setup`
- Otorisasi: `INSTRUCTOR`
- Request:
```json
{
  "client_request_id": "idn-setup-001",
  "players": [
    {
      "session_player_id": "uuid",
      "tie_breaker_code": "TB-1",
      "ingredient_card_id": "ING-001",
      "gold_quantity": 1,
      "mission_id": "MIS-001",
      "loan_code": null,
      "insurance_product_code": null
    }
  ]
}
```
- Setup harus mencakup setiap peserta tepat satu kali dan seluruh kode harus berasal dari ruleset sesi.
- Penyimpanan pertama mengunci daftar peserta dan ruleset. Selama status sesi masih `CREATED`, payload baru dengan `client_request_id` baru membuat revisi berikutnya untuk peserta/ruleset yang sama.
- Respons revisi baru berstatus `201` dan memuat `revision`, `setup_status=EDITABLE`, `saved_at`, `locked_at=null`, serta `players`.
- Retry dengan `client_request_id` dan payload identik mengembalikan revisi yang sama dengan status `200`. Pemakaian ID yang sama untuk payload berbeda menghasilkan `409 CLIENT_REQUEST_ID_CONFLICT`.
- `GET /api/v1/sessions/{sessionId}/setup` membaca revisi terbaru. Start mengubah status revisi tersebut menjadi `LOCKED` dan mengisi `locked_at`; setelah start tidak ada endpoint untuk membuka kembali setup.

---

### 6.4 Mulai sesi
- Method: `POST`
- Path: `/api/v1/sessions/{sessionId}/start`
- Otorisasi: `INSTRUCTOR`
- Prasyarat: revisi pembagian awal terbaru dari IDN tersedia dan masih valid terhadap peserta/ruleset yang terkunci sejak revisi pertama.
- Response 200:
```json
{ "status": "STARTED" }
```

---

### 6.5 Akhiri sesi
- Method: `POST`
- Path: `/api/v1/sessions/{sessionId}/end`
- Otorisasi: `INSTRUCTOR`
- Response 200:
```json
{ "status": "ENDED" }
```

---

### 6.6 Ambil state sesi
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

### 6.7 Tulis state sesi
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

### 7.4 Ambil daftar Player pada sesi
- Method: `GET`
- Path: `/api/v1/sessions/{sessionId}/players`
- Otorisasi: `INSTRUCTOR` atau `PLAYER` yang menjadi peserta sesi tersebut
- Response 200:
```json
{
  "items": [
    {
      "user_id": "uuid",
      "display_name": "Player A",
      "player_order_no": 1
    }
  ]
}
```

Scope:
- Instruktur dapat membaca peserta sesi yang dapat diaksesnya.
- Player hanya dapat membaca peserta sesi tempat akunnya terdaftar.

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
- Path: `/api/v1/sessions/{sessionId}/events?cursor=opaque&limit=50`
- Response 200:
```json
{
  "items": [ { ... }, { ... } ],
  "next_cursor": "opaque-or-null",
  "has_more": true
}
```

Urutan stabil memakai `sequence_number`. `limit` default 50 dan maksimum 100; cursor Base64URL yang rusak menghasilkan `400 VALIDATION_ERROR`.

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
    "cashflow_net_total": 50
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
      "fulfillment_diversity": 0.8,
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
- Path: `/api/v1/analytics/sessions/{sessionId}/transactions?userId=uuid&cursor=opaque&limit=50`
- Response 200:
```json
{
  "items": [
    { "transaction_id":"uuid", "timestamp":"...", "direction":"OUT", "amount":5, "category":"NEED_PRIMARY" }
  ],
  "next_cursor": "opaque-or-null",
  "has_more": false
}
```

Urutan stabil memakai `timestamp, transaction_id`. `limit` default 50 dan maksimum 100.

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
  "needs": {
    "fulfillment_diversity": 0.8
  }
}
```

Catatan:
- Response gameplay API saat ini disajikan dalam empat kelompok:
  `economy`, `progress`, `score`, dan `needs`, ditambah `raw_json` serta `derived_json` agar asal data dan substitusi rumus dapat ditelusuri.
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
Prometheus hanya di jaringan internal service API. Nginx mengembalikan `404`
untuk permintaan publik ke `/metrics`.

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
