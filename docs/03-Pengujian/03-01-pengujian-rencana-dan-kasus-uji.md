# Rencana Pengujian Fungsional dan Validasi Sistem
## Sistem Informasi Dasbor Analitika & Manajemen *Ruleset* Cashflowpoly

### Dokumen
- Nama dokumen: Rencana Pengujian Fungsional dan Validasi
- Versi: 1.3
- Tanggal: 11 Juli 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan
Dokumen ini disusun untuk menetapkan skenario uji fungsional (*black-box*), uji integrasi alur event, serta validasi konsistensi tampilan dasbor terhadap data basis data. Dokumen ini juga disusun untuk menetapkan format pencatatan hasil uji dan kriteria kelulusan.

---

## 2. Ruang Lingkup Pengujian
Sistem menguji tiga area.

1. **Uji RESTful API (black-box)**  
   Sistem menguji endpoint sesuai kontrak, validasi input, kode status respons, dan format error.

2. **Uji integrasi alur event dan konsistensi *state***  
   Sistem menguji urutan event, idempotensi, keterikatan event ke sesi dan *ruleset*, serta konsistensi proyeksi dan metrik.

3. **Validasi dasbor analitika (UI MVC)**  
   Sistem menguji kesesuaian nilai yang UI tampilkan dengan data pada tabel `metric_snapshots` dan `event_cashflow_projections`.

---

## 3. Lingkungan dan Prasyarat
### 3.1 Lingkungan
- OS: Windows 11 Home
- Runtime: .NET 10
- DBMS: PostgreSQL
- Alat uji: Swagger UI dan Postman
- Peramban: Google Chrome

### 3.2 Konfigurasi dasar
Sistem menyiapkan:
1. database kosong,
2. skrip skema database dijalankan melalui DBeaver,
3. seed data minimal:
   - 1 *ruleset* default + 1 `ruleset_version` default,
   - 1 sesi contoh (opsional).

### 3.3 Data uji minimum
Sistem membutuhkan:
- `session_id`
- `user_id` akun Player (1-3 pemain)
- `session_player_id`/`session_participant_id` bila memvalidasi projection state
- `ruleset_version_id` yang aktif pada sesi
- daftar event contoh dengan `sequence_number` berurutan

---

## 4. Standar Format Error API
Sistem menganggap respons error valid jika sistem mengembalikan struktur berikut:
```json
{
  "error_code": "STRING",
  "message": "STRING",
  "details": [],
  "trace_id": "STRING"
}
```

Sistem memeriksa:
1. `error_code` jelas dan konsisten,
2. `message` membantu diagnosa,
3. `trace_id` ada untuk pelacakan log.

---

## 5. Kriteria Kelulusan Umum
Sistem dinyatakan valid jika:
1. semua skenario uji wajib berstatus **PASS**,
2. sistem tidak menulis data korup pada basis data,
3. sistem menolak input tidak valid dengan kode status yang tepat,
4. UI menampilkan nilai metrik yang sama dengan query basis data yang relevan.

---

## 6. Daftar Modul yang Diuji
Sistem mengelompokkan hasil uji per modul agar laporan mudah dibaca.

| Kode Modul | Modul |
|---|---|
| M1 | Manajemen Sesi |
| M2 | Manajemen *Ruleset* |
| M3 | Ingest Event (REST API) |
| M4 | Proyeksi Arus Kas |
| M5 | Agregasi Metrik |
| M6 | Analitika (Endpoint ringkasan/transaksi) |
| M7 | UI MVC Dasbor |
| M8 | Logging & Error Handling |

Catatan: kalimat “Temuan uji dominan di modul mana?” berarti **modul mana yang paling sering menghasilkan temuan/bug** pada periode uji tertentu. Laporan hasil uji perlu mencatat frekuensi temuan per modul.

---

## 7. Uji RESTful API (Black-box)
Bagian ini merinci skenario uji wajib per endpoint.

### 7.1 Konvensi penulisan skenario
Setiap skenario memuat:
- ID uji
- Modul
- Endpoint
- Input
- Langkah uji
- Ekspektasi respons
- Ekspektasi perubahan data (bila ada)

### 7.2 Daftar skenario uji inti
#### A. Manajemen Sesi (M1)
**TC-API-01 — Buat sesi**
- Endpoint: `POST /api/v1/sessions`
- Input contoh:
```json
{ "session_name": "Sesi Uji 01", "mode": "PEMULA", "ruleset_version_id": "UUID" }
```
- Langkah:
1. Kirim permintaan.
2. Catat `session_id`.
- Ekspektasi:
  - Status: `201`
  - Body memuat `session_id`
  - DB: tabel `sessions` menambah 1 baris.

**TC-API-02 — Mulai sesi**
- Endpoint: `POST /api/v1/sessions/{sessionId}/start`
- Langkah:
1. Tambahkan peserta, validasi dan kunci pembagian awal, lalu kirim permintaan pada sesi berstatus `CREATED`.
- Ekspektasi:
  - Status: `200`
  - DB: `status=STARTED`, `started_at` terisi.
  - Event setup memakai kartu yang dikirim IDN, bukan hasil pembagian acak backend.

**TC-API-03 — Akhiri sesi**
- Endpoint: `POST /api/v1/sessions/{sessionId}/end`
- Langkah:
1. Kirim permintaan pada sesi berstatus `STARTED`.
- Ekspektasi:
  - Status: `200`
  - DB: `status=ENDED`, `ended_at` terisi.

**TC-API-04 — Tolak start sesi yang sudah END**
- Endpoint: `POST /api/v1/sessions/{sessionId}/start`
- Kondisi: sesi `ENDED`
- Ekspektasi:
  - Status: `422`
  - Body error sesuai standar.

---

#### B. Manajemen *Ruleset* (M2)
Catatan:
- Semua endpoint mutasi ruleset dan aktivasi ruleset mensyaratkan token Bearer milik role `INSTRUCTOR`.

**TC-API-05 — Buat ruleset + versi 1**
- Endpoint: `POST /api/v1/rulesets`
- Input contoh:
```json
{
  "name": "Ruleset Default",
  "description": "Default pemula",
  "definition": {
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
- Ekspektasi:
  - Status: `201`
  - Body memuat `ruleset_id` dan `version`
  - DB: `rulesets` dan `ruleset_versions` bertambah.

**TC-API-06 — Update ruleset membuat versi baru**
- Endpoint: `PUT /api/v1/rulesets/{rulesetId}`
- Input: perubahan kecil pada `definition`
- Ekspektasi:
  - Status: `200`
  - DB: `ruleset_versions` menambah versi baru berstatus awal `DRAFT`.

**TC-API-07 — Tolak ruleset invalid (rentang salah)**
- Endpoint: `POST /api/v1/rulesets`
- Input: `donation.min_amount > donation.max_amount`
- Ekspektasi:
  - Status: `400`
  - `error_code` menyebut validasi ruleset
  - DB: tidak menambah versi.

**TC-API-07B — Aktivasi versi ruleset global**
- Endpoint: `POST /api/v1/rulesets/{rulesetId}/versions/{version}/activate`
- Prasyarat: versi target tersedia dan masih `DRAFT`.
- Ekspektasi:
  - Status: `200`
  - DB: versi target menjadi `ACTIVE` dan versi `ACTIVE` lama menjadi `ARCHIVED`.

**TC-API-08 — Buat sesi dengan ruleset_version_id**
- Endpoint: `POST /api/v1/sessions`
- Input contoh:
```json
{
  "session_name": "Kelas A - Pertemuan 1",
  "mode": "PEMULA",
  "ruleset_version_id": "UUID"
}
```
- Ekspektasi:
  - Status: `201`
  - DB: `sessions.ruleset_version_id` terisi sesuai input.

**TC-API-09 — Tolak create session tanpa ruleset_version_id**
- Endpoint: `POST /api/v1/sessions`
- Kondisi: payload tidak memiliki `ruleset_version_id`
- Ekspektasi:
  - Status: `400`
  - DB: tidak membuat session.

---

#### C. Ingest Event (M3)
**TC-API-10 — Terima event valid**
- Endpoint: `POST /api/v1/events`
- Input contoh:
```json
{
  "event_id": "UUID",
  "session_id": "UUID",
  "user_id": "UUID",
  "actor_type": "PLAYER",
  "timestamp": "2026-01-27T10:00:00Z",
  "day_index": 0,
  "weekday": "MON",
  "turn_number": 1,
  "action_slot": 1,
  "sequence_number": 1,
  "action_type": "CatatTransaksi",
  "ruleset_version_id": "UUID",
  "payload": { "direction": "IN", "amount": 1, "category": "TEST" }
}
```
- Ekspektasi:
  - Status: `201`
  - DB: `events` bertambah 1 baris.

**TC-API-11 — Idempotensi event (event_id sama)**
- Kirim input TC-API-10 dua kali.
- Ekspektasi:
  - Respons kedua: `409`
  - DB: tidak menambah baris kedua pada `events`.

**TC-API-12 — Tolak ruleset_version_id tidak cocok dengan sesi**
- Kondisi: sesi aktif memakai ruleset A, event mengirim ruleset B.
- Ekspektasi:
  - Status: `422`
  - DB: tidak menambah event
  - DB: `validation_logs` (jika dipakai) mencatat kegagalan.

**TC-API-13 — Tolak sequence_number loncat**
- Kirim event `sequence_number=2` saat sistem baru menerima `0`.
- Ekspektasi:
  - Status: `422`
  - `error_code` menyebut urutan event.

**TC-API-14 — Tolak payload tidak sesuai tipe**
- Kirim `payload.amount = "satu"` pada event `CatatTransaksi`.
- Ekspektasi:
  - Status: `400` atau `422`
  - Body error sesuai standar.

---

#### D. Analitika (M6)
**TC-API-15 — Ringkasan analitika sesi**
- Endpoint: `GET /api/v1/analytics/sessions/{sessionId}`
- Prasyarat: sistem sudah menerima minimal 5 event valid.
- Ekspektasi:
  - Status: `200`
  - Body memuat:
    - `summary` level sesi
    - `by_player` list per pemain
    - `ruleset_version_id` konteks

**TC-API-16 — Histori transaksi pemain**
- Endpoint: `GET /api/v1/analytics/sessions/{sessionId}/transactions?userId={userId}`
- Ekspektasi:
  - Status: `200`
  - Body list transaksi terurut waktu desc/asc (pilih satu dan konsisten).

**TC-API-17 — Histori transaksi tanpa filter userId**
- Endpoint: transaksi tanpa `userId`
- Ekspektasi:
  - Status: `200`
  - Instruktur menerima semua transaksi sesi yang sesuai scope.
  - Player tetap hanya menerima transaksi miliknya.

---

#### E. Autentikasi dan Otorisasi (M8)
**TC-API-18 — Login valid**
- Endpoint: `POST /api/v1/auth/login`
- Input: username/password valid.
- Ekspektasi:
  - Status: `200`
  - Respons memuat `access_token`, `expires_at`, dan `role`.

**TC-API-19 — Login kredensial salah**
- Endpoint: `POST /api/v1/auth/login`
- Ekspektasi:
  - Status: `401`
  - Error mengikuti format standar.

**TC-API-20 — Register valid (PLAYER)**
- Endpoint: `POST /api/v1/auth/register`
- Input: role `PLAYER`.
- Ekspektasi:
  - Status: `201`
  - User baru tersimpan.

**TC-API-20B — Register INSTRUCTOR sesuai kebijakan**
- Endpoint: `POST /api/v1/auth/register`
- Input: role `INSTRUCTOR`.
- Ekspektasi:
  - Status production: `403`.

**TC-API-21 — Endpoint terproteksi tanpa token**
- Contoh endpoint: `POST /api/v1/rulesets`.
- Ekspektasi:
  - Status: `401`.

**TC-API-22 — Endpoint instruktur dipanggil token player**
- Contoh endpoint: `DELETE /api/v1/rulesets/{rulesetId}`.
- Ekspektasi:
  - Status: `403`.

---

## 8. Uji Integrasi Alur Event dan Konsistensi *State*
Sistem menjalankan uji integrasi memakai rangkaian event yang menyerupai permainan nyata.

### 8.1 Skenario integrasi minimum (IT-01)
**Tujuan:** sistem membangun proyeksi arus kas dan metrik tanpa inkonsistensi.

**Langkah:**
1. Buat sesi (TC-API-01).
2. Aktivasi ruleset (TC-API-08).
3. Daftarkan 2 pemain (jika endpoint tersedia).
4. Kirim rangkaian event berurutan:
   - `AkhirGiliran`
   - `Kebutuhan`
   - `BahanMasakan`
   - `JualMasakan`
   - `JumatBerkah`
5. Panggil `GET /api/v1/analytics/sessions/{sessionId}`.

**Ekspektasi:**
- `events` berisi event sesuai urutan `sequence_number`.
- `event_cashflow_projections` berisi transaksi hasil proyeksi.
- `metric_snapshots` berisi metrik minimum.
- Sistem tidak menghasilkan nilai negatif untuk kepemilikan (bahan/emas).
- Setiap baris proyeksi memiliki referensi event yang valid melalui `event_pk`.

Validasi DB integritas referensi event:
```sql
select count(*) as orphan_projections
from event_cashflow_projections ecp
left join events e on e.event_pk = ecp.event_pk
where e.event_pk is null;
```
Ekspektasi: `orphan_projections = 0`.

### 8.2 Skenario integrasi pelanggaran aturan (IT-02)
**Tujuan:** sistem menolak event yang melanggar aturan ruleset.

**Langkah:**
1. Buat sesi dengan `ruleset_version_id` yang memiliki `constraints.require_primary_before_others=true`.
2. Kirim event `Kebutuhan` sebelum `Kebutuhan` pada hari yang sama.
3. Ambil respons error.

**Ekspektasi:**
- Sistem mengembalikan `422`.
- Sistem tidak menyimpan event pada `events`.
- Sistem mencatat kegagalan pada `validation_logs` jika modul ini aktif.

Validasi payload invalid tersimpan tanpa event utama:
```sql
select count(*) as invalid_logs
from validation_logs vl
where vl.raw_payload_json is not null;
```
Ekspektasi: `invalid_logs > 0`.

### 8.3 Skenario integrasi perubahan ruleset (IT-03)
**Tujuan:** sistem menempelkan event ke versi ruleset yang benar.

**Langkah:**
1. Buat sesi dengan ruleset versi 1.
2. Kirim 3 event valid.
3. Buat/update ruleset hingga terbentuk versi 2 berstatus `DRAFT`.
4. Aktifkan versi 2 secara global melalui endpoint aktivasi versi ruleset.
5. Buat sesi baru dengan `ruleset_version_id` versi 2 untuk skenario perbandingan.
6. Kirim 3 event valid berikutnya.
7. Ambil ringkasan analitika dan audit event.

**Ekspektasi:**
- Event awal mereferensikan `ruleset_version_id` versi 1.
- Event berikutnya mereferensikan versi 2.
- Snapshot metrik menyertakan `ruleset_version_id` konteks saat hitung.

---

## 9. Validasi Dasbor Analitika (UI MVC)
Sistem memvalidasi UI dengan cara membandingkan nilai UI terhadap data DB.

### 9.1 Prinsip validasi
UI hanya menampilkan nilai dari API. UI tidak menghitung ulang metrik di klien.

### 9.2 Skenario validasi UI (UI-01)
**Tujuan:** halaman detail sesi menampilkan ringkasan yang sama dengan DB.

**Langkah:**
1. Buka `/sessions/{sessionId}`.
2. Catat nilai kartu metrik:
   - total pemasukan
   - total pengeluaran
   - *net cashflow*
3. Jalankan query DB:
```sql
select metric_name, metric_value_numeric
from metric_snapshots
where session_id = '<SESSION_ID>' and user_id is null and session_player_id is null
order by computed_at desc;
```
4. Bandingkan nilai UI dan nilai DB untuk metrik yang sama.

**Ekspektasi:**
- UI menampilkan nilai yang sama (toleransi 0 untuk integer).

### 9.3 Skenario validasi UI transaksi (UI-02)
**Tujuan:** tabel transaksi pemain cocok dengan tabel proyeksi.

**Langkah:**
1. Buka `/sessions/{sessionId}/players/{userId}`.
2. Catat 5 baris transaksi teratas.
3. Jalankan query DB:
```sql
select timestamp, direction, amount, category
from event_cashflow_projections
where session_id = '<SESSION_ID>' and user_id = '<USER_ID>'
order by timestamp desc
limit 5;
```
4. Bandingkan nilai.

**Ekspektasi:**
- waktu, arah, nominal, kategori cocok.

### 9.4 Validasi error UI (UI-03)
**Tujuan:** UI menampilkan error terstruktur.
- Paksa kondisi 404 (akses sesi tidak ada).
- Ekspektasi: UI menampilkan pesan “Sesi tidak ditemukan” dan menampilkan `trace_id` bila API menyertakannya.

---

## 10. Kriteria *Pass/Fail* per Skenario
Sistem menetapkan:
- **PASS** jika respons API sesuai kontrak dan perubahan data sesuai ekspektasi.
- **FAIL** jika salah satu kondisi berikut terjadi:
  1. status code salah,
  2. format body salah,
  3. sistem menyimpan event tidak valid,
  4. UI menampilkan nilai tidak sama dengan data basis data.

---

## 11. Pencatatan Hasil Uji
### 11.1 Template tabel hasil uji
Sistem mencatat hasil uji pada tabel berikut.

| Tanggal | ID Uji | Modul | Status | Bukti (URL/SS) | Catatan | *Trace ID* |
|---|---|---|---|---|---|---|

### 11.2 Template temuan (*bug report*)
Sistem mencatat temuan dengan format berikut:
- ID temuan: BUG-XXX
- Modul: M1–M8
- Ringkasan: (1 kalimat)
- Langkah reproduksi:
  1. ...
  2. ...
- Ekspektasi:
- Aktual:
- Dampak:
- Bukti:
- *Trace ID*:
- Status: OPEN / FIXED / RE-TEST

### 11.3 Rekap “temuan dominan”
Sistem membuat rekap mingguan:
- total temuan,
- jumlah temuan per modul,
- modul dengan temuan terbanyak (temuan dominan).

---

## 12. Checklist Kelulusan Akhir
Sistem lulus tahap pengujian dan validasi jika:
1. sistem menyelesaikan semua TC-API-01 s.d. TC-API-33 (termasuk TC-API-07B) dengan PASS,
2. sistem menyelesaikan IT-01 s.d. IT-03 dengan PASS,
3. sistem menyelesaikan UI-01 s.d. UI-03 dengan PASS,
4. sistem tidak meninggalkan temuan berstatus OPEN pada modul inti (M1–M6).




## 13. Tambahan TC Event Skor & Risiko
Tambahan pengujian untuk event baru:
1. TC-API-23 — `SetupMisiAwal` valid (penetapan misi)
2. TC-API-24 — `PoinPeringkatDonasi` valid (award poin)
3. TC-API-25 — `PoinEmas` valid (award poin emas)
4. TC-API-26 — `PoinPeringkatPensiun` valid (award poin pensiun)
5. TC-API-27 — `Menabung` valid sampai maksimal 15 koin/aksi dan `TarikTabungan` ditolak
6. TC-API-28 — event sistem `TujuanFinansial` valid hanya saat saldo, biaya, poin, dan stok kartu sesuai
7. TC-API-29 — `RisikoKehidupan` valid untuk mode MAHIR
8. TC-API-30 — `BayarPinjaman` menolak pembayaran yang tidak sama dengan seluruh outstanding
9. TC-API-31 — `KerjaLepas` valid sesuai `freelance.income`
10. TC-API-32 — `AkhirGiliran` menolak jika `JualMasakan` tanpa `RisikoKehidupan` (mode MAHIR)
11. TC-API-33 — ruleset scoring menghitung poin donasi/emas/pensiun pada analitika
12. TC-API-34 — `RisikoKehidupan` wajib merujuk `JualMasakan` valid melalui `source_order_event_id`
13. TC-API-35 — seluruh risiko `OUT` tetap pending sampai menerima event penyelesaian
14. TC-API-36 — `BayarRisiko` hanya menerima risiko pending milik pemain dengan saldo cukup
15. TC-API-37 — `Asuransi` dengan `risk_event_id` memerlukan polis aktif, mengurangi tepat satu penggunaan, dan hanya membuat satu offset
16. TC-API-38 — `GunakanOpsiDarurat` menghitung nominal server-side dan mengubah kebutuhan, emas, atau pinjaman secara atomik
17. TC-API-39 — donasi Jumat kedua pemain pada hari yang sama ditolak dengan `DONATION_ALREADY_SUBMITTED`
18. TC-API-40 — `JualEmas` membaca holding relasional, termasuk emas setup dan penjualan darurat
19. TC-API-41 — beberapa `loan_instance_id` dari produk yang sama dapat aktif sampai batas `card_qty`; ID instance duplikat dan pengambilan melebihi stok ditolak

Catatan tambahan:
- `SetupMisiAwal.penalty_points` harus 10 (rulebook).
- `PinjamanSyariah.principal` harus 10 dan `penalty_points` harus 15.
- `loan_id`/`loan_instance_id` mengidentifikasi satu kartu pinjaman, bukan produk. Pelunasan menutup seluruh outstanding satu instance.
- `Asuransi.premium` harus sesuai katalog; penggunaan polis harus merujuk risiko `OUT` pending milik pemain serta memeriksa `ACTIVE` dan `remaining_uses`.

Catatan:
- TC ini mengikuti format skenario uji pada bagian TC-API sebelumnya.
- Bila sistem belum memakai otomatisasi, pengujian dilakukan melalui Postman.

---

## 14. Smoke Test dan Kriteria Fitur Selesai
Checklist ini wajib dipenuhi sebagai *acceptance criteria* teknis sebelum fitur dinyatakan selesai.

### 14.0 Gerbang rilis tunggal

Sebelum merge dan deployment, jalankan dari root repository:

```powershell
.\scripts\Invoke-ReleaseVerification.ps1
```

Gerbang akhir wajib dijalankan tanpa parameter skip. Parameter `-SkipBrowser`, `-SkipPerformance`, dan `-SkipDockerBuild` hanya untuk mempercepat iterasi pengembangan, bukan bukti kelulusan rilis.

### 14.1 Smoke test otomatis
1. Jalankan `docker compose up --build -d`.
2. Jalankan build per proyek:
   - `dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj`
   - `dotnet build src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj`
   - `dotnet build tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj`
3. Jalankan test per proyek:
   - `dotnet test tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj --filter "Category!=Integration"`
   - `dotnet test tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj --filter "Category=Integration"`
4. Jalankan skenario end-to-end API (auth -> ruleset -> session -> player -> event -> analytics) melalui Postman collection.
5. Verifikasi semua langkah smoke berstatus sukses tanpa exception.

### 14.2 Verifikasi API collection
1. Impor `postman/Cashflowpoly.postman_collection.json` dan `postman/Cashflowpoly.local.postman_environment.json`.
2. Isi `authUsername` dan `authPassword` dengan akun Instruktur bootstrap yang valid, lalu jalankan koleksi dari request pertama.
3. Pastikan Login menghasilkan `200`; respons `401/403` pada request positif harus menggagalkan pengujian.
4. Verifikasi endpoint kritikal:
   - autentikasi (`login/register`),
   - ruleset (create/update/activate/delete),
   - sesi (create/start/end dengan `ruleset_version_id`),
   - daftar pemain sesi (`GET /api/v1/sessions/{sessionId}/players`),
   - ingest event dan analytics.

### 14.2A RBAC smoke test
1. Jalankan skenario RBAC pada Postman collection atau HTTP client setara.
2. Verifikasi skenario minimum:
   - endpoint terproteksi tanpa token => `401`,
   - endpoint instruktur dipanggil token player => `403`,
   - endpoint read yang diizinkan player => `200`,
   - endpoint instruktur dipanggil token instruktur => sukses.

### 14.2B Web UI smoke test
1. Login ke UI menggunakan akun valid.
2. Verifikasi halaman utama (`/`, `/sessions`, `/players`, `/rulesets`, `/rulebook`) dapat diakses tanpa error, dan route kompatibilitas `/analytics` atau `/Analytics` melakukan redirect yang benar ke halaman sesi.
3. Pada environment Development, verifikasi Swagger API (`/swagger`) dapat diakses. Swagger tidak diekspos pada Production.

### 14.3 Definition of Done (DoD)
Fitur dinyatakan selesai jika:
1. Build API dan UI lulus tanpa error.
2. Verifikasi end-to-end API lulus.
3. Verifikasi RBAC lulus.
4. Verifikasi Web UI lulus.
5. Skenario Postman kritikal lulus.
6. Tidak ada bug blocker (`S1`) pada modul terdampak.

### 14.4 Baseline uji performa dan evidence formal
1. Jalankan `ReleasePerformanceIntegrationTests` yang menyiapkan 100 akun, 20 sesi aktif, 20 pengguna bersamaan, serta request berulang ke endpoint umum dan analitik.
2. Verifikasi target minimum:
   - P95 API umum < 500 ms,
   - P95 analitik < 2 detik,
   - error rate 0% pada skenario baseline.
3. Simpan artefak pada media dokumentasi pengujian yang dipakai:
   - output build/test,
   - status compose/health,
   - ringkasan load test,
   - sampel SQL/security audit/observability (`/api/v1/observability/metrics/summary` dan `/metrics`).
