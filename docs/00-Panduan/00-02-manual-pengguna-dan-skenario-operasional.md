# Manual Pengguna dan Skenario Operasional
## Sistem Informasi Dasbor Analitika Cashflowpoly

### Dokumen
- Nama dokumen: Manual Pengguna dan Skenario Operasional
- Versi: 1.2
- Tanggal: 18 Juni 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan
Dokumen ini menjelaskan cara penggunaan sistem dari sudut pandang instruktur sebagai operator. Dokumen ini juga mendokumentasikan skenario operasional standar dari pembuatan sesi sampai interpretasi metrik pada dasbor.

---

## 2. Peran Pengguna
### 2.1 Instruktur
Instruktur mengelola sesi, menambahkan Player, dan memasukkan input keputusan Player melalui Klien Game/IDN. Instruktur memakai Web Analitik untuk memantau performa pembelajaran, performa misi, tren hasil pembelajaran, serta mengelola dan mengaktifkan *ruleset*.

### 2.2 Pemain
Pemain melihat performa diri dan histori transaksi.

Identitas yang dipakai sistem:
- akun Instruktur dan Player disimpan pada `app_users` dan diidentifikasi dengan `user_id`;
- peserta sesi disimpan pada `session_participants` dan diidentifikasi sebagai `session_participant_id` pada database atau `session_player_id` pada beberapa DTO state;
- tidak ada tabel `players` terpisah; resource API `/api/v1/players` merepresentasikan akun role `PLAYER`.

Matriks lengkap alur dan hak akses tersedia pada `docs/00-Panduan/00-06-matriks-alur-dan-hak-akses.md`.

---

## 3. Akses Sistem
Instruktur menjalankan dua komponen berikut:
1. REST API (server) untuk menerima event dan menghasilkan analitika,
2. Web MVC (dasbor) untuk tampilan analitika,
3. Klien Game/IDN untuk setup sesi dan input permainan.

Instruktur atau Player mengakses Web Analitik melalui browser:
- Home: `http://localhost:5203/`
- Daftar sesi: `http://localhost:5203/sessions`
- Ruleset: `http://localhost:5203/rulesets`
- Rulebook: `http://localhost:5203/rulebook`

Instruktur menguji API melalui Swagger:
- URL Swagger: `http://localhost:5041/swagger`

Catatan:
- URL dapat berbeda tergantung konfigurasi `launchSettings.json`.
- UI menggunakan Tailwind CSS. Jika baru menyiapkan lingkungan atau ada perubahan tampilan, jalankan build CSS Tailwind sesuai panduan setup.
- Endpoint API selain login/register mensyaratkan token Bearer.

### 3.1 Alur autentikasi API (ringkas)
1. Login via `POST /api/v1/auth/login`.
2. Simpan `access_token` dari respons login.
3. Kirim header `Authorization: Bearer <access_token>` pada endpoint terproteksi.
4. Jika token kedaluwarsa atau salah, API merespons `401`.

Pada Web Analitik, token API disimpan pada session server-side UI dan dikirim
ulang sebagai Bearer token oleh `HttpClient`. Browser pengguna tidak perlu
memasang header Bearer secara manual saat memakai UI MVC.

---

## 4. Alur Operasional Standar (Sebagai Instruktur)
Bagian ini mendefinisikan langkah kerja yang instruktur lakukan pada setiap sesi.

### 4.1 Menyiapkan sistem
Instruktur memastikan:
1. PostgreSQL berjalan,
2. REST API berjalan,
3. Web MVC berjalan,
4. Swagger dapat diakses.

---

## 5. Skenario 1 - Membuat Sesi Baru melalui Klien Game/IDN
### Tujuan Skenario 1
Instruktur membuat sesi permainan untuk menerima event dan menyimpan histori.

### Langkah Skenario 1
1. Instruktur login ke Klien Game/IDN.
2. Instruktur membuat sesi baru.
3. Instruktur mengisi nama sesi, mode, dan `ruleset_version_id` versi ruleset
   berstatus `ACTIVE`.
4. Klien Game/IDN mengirim request ke API.

Catatan pengujian developer:
- Skenario yang sama dapat diuji melalui Swagger/Postman dengan endpoint `POST /api/v1/sessions`.

### Hasil Skenario 1
Sistem mengembalikan `session_id`.

### Verifikasi cepat (opsional)
Instruktur membuka halaman `/sessions` pada Web dan memastikan sesi muncul pada daftar.

---

## 6. Skenario 2 - Membuat Sesi dengan *Ruleset*
### Tujuan Skenario 2
Instruktur menetapkan konfigurasi aturan yang berlaku saat sesi dibuat.

### Langkah Skenario 2
1. Instruktur memilih ruleset version berstatus `ACTIVE`.
2. Klien Game/IDN membuat sesi dengan `POST /api/v1/sessions`.
3. Payload create session menyertakan `ruleset_version_id`.
4. API memvalidasi role `INSTRUCTOR`, mode sesi, ruleset, dan versi aktif.

### Hasil Skenario 2
Sistem menampilkan ruleset aktif dan versi yang terpilih.

### Aturan penting
Sistem tidak menyediakan aktivasi ruleset terpisah setelah session dibuat.
Session mengunci `ruleset_version_id` saat dibuat. Versi ruleset dapat
diaktifkan pada level ruleset melalui
`POST /api/v1/rulesets/{rulesetId}/versions/{version}/activate` sebelum
session baru dibuat.

Catatan akses:
- Untuk pengujian developer, endpoint yang digunakan adalah `POST /api/v1/sessions` dengan token Bearer milik role `INSTRUCTOR`.
- Web Analitik menampilkan ruleset aktif pada detail sesi secara read-only.

---

## 7. Skenario 3 - Memulai Sesi melalui Klien Game/IDN
### Tujuan Skenario 3
Instruktur menandai sesi siap menerima event permainan.

### Langkah Skenario 3
1. Instruktur memilih aksi mulai sesi pada Klien Game/IDN.
2. Klien Game/IDN memanggil `POST /api/v1/sessions/{sessionId}/start`.

### Hasil Skenario 3
Sistem mengubah status menjadi `STARTED`.

---

## 8. Skenario 4 - Mengirim Event Permainan (Klien/Simulator)
### Tujuan Skenario 4
Sistem menerima event permainan dan menyimpannya sebagai histori.

### Cara mengirim event
Sistem menerima event dari Klien Game/IDN atau simulator. Pengujian manual dapat dilakukan via Postman/Swagger.

### Langkah uji manual (Postman/Swagger)
1. Buka endpoint `POST /api/v1/events`.
2. Isi payload event sesuai kontrak.
3. Kirim event berurutan berdasarkan `sequence_number`.

### Hal yang instruktur perhatikan
1. Sistem menerima event valid dengan status berhasil.
2. Sistem menolak event yang:
   - urutan `sequence_number` loncat,
   - `sequence_number` duplikat dalam satu sesi,
   - `ruleset_version_id` tidak sesuai sesi,
   - tipe data payload salah.

Catatan server:
- Sistem menyimpan event dengan `event_pk` sebagai PK internal.
- Sistem menerapkan idempotensi pada kombinasi `session_id + event_id`.
- Event aksi Player memakai `user_id` akun Player. API me-resolve akun tersebut
  ke peserta sesi (`session_player_id`) sebelum menulis event/projection.
- Endpoint `PUT /api/v1/sessions/{sessionId}/state` tidak dipakai untuk menulis
  gameplay dan selalu mengembalikan `410 STATE_WRITE_DISABLED`; state berubah
  dari event valid.

Catatan operasional:
- Aksi kerja lepas dicatat dengan event `KerjaLepas` dengan `amount` mengikuti ruleset.
- Pada mode mahir, setiap `JualMasakan` harus diikuti `RisikoKehidupan` pada giliran yang sama.

### Bukti yang instruktur simpan
Instruktur menyimpan:
- respons API,
- `trace_id` jika sistem menolak event.

---

## 9. Skenario 5 - Memantau Analitika pada Detail Sesi
### Tujuan Skenario 5
Instruktur menilai performa pembelajaran dan performa misi pada level sesi (agregat) dan per pemain.

### Langkah Skenario 5
1. Buka Web MVC.
2. Buka `/sessions`.
3. Pilih sesi dan buka `Detail`.
4. Lihat kartu metrik ringkasan:
   - total pemasukan,
   - total pengeluaran,
   - net cashflow,
   - jumlah event,
   - jumlah pelanggaran aturan (jika ada).
5. Lihat indikator performa pembelajaran dan performa misi (agregat).
6. Lihat tabel pemain untuk ringkasan per pemain.
   - total poin kebahagiaan,
   - total donasi dan emas,
   - indikator pinjaman belum lunas (jika ada).
7. Gunakan filter/pengelompokan berdasarkan *ruleset* bila tersedia pada UI.

### Interpretasi cepat
- Pemasukan tinggi dan pengeluaran tinggi perlu dilihat bersama net cashflow.
- Donasi menunjukkan keputusan sosial pemain pada hari Jumat.
- Kepatuhan kebutuhan primer menunjukkan kedisiplinan pemain terhadap aturan pembelian.

---

## 10. Skenario 6 - Membaca Detail Pemain dan Histori Transaksi
### Tujuan Skenario 6
Instruktur atau pemain menilai performa pembelajaran, performa misi, dan perilaku pemain berdasarkan transaksi dan metrik.

### Langkah Skenario 6
1. Dari detail sesi, pilih pemain.
2. Buka halaman detail pemain.
3. Lihat:
   - metrik pemain (kartu/daftar),
   - indikator performa pembelajaran dan performa misi pemain,
   - tabel histori transaksi.
   - rincian poin kebahagiaan (kebutuhan, bonus set, donasi, emas, pensiun, tujuan keuangan, penalti).

Catatan:
- Jika ruleset memuat tabel scoring, poin donasi/emas/pensiun dihitung otomatis tanpa event awarding.

### Cara membaca tabel transaksi
Instruktur menilai:
- kategori transaksi dominan,
- frekuensi transaksi per hari,
- transaksi besar yang memengaruhi saldo,
- hubungan transaksi dengan event tertentu (bila sistem menyediakan audit event).

---

## 11. Skenario 7 - Mengakhiri Sesi dan Membekukan Data
### Tujuan Skenario 7
Instruktur menandai sesi selesai agar sistem tidak menerima perubahan konfigurasi.

### Langkah Skenario 7
1. Buka Klien Game/IDN atau Swagger dengan token Instruktur.
2. Panggil `POST /api/v1/sessions/{sessionId}/end`.

### Hasil Skenario 7
Sistem mengubah status menjadi `ENDED`.

### Dampak
Sistem menolak:
- perubahan `ruleset_version_id` pada sesi ini,
- perubahan status lain yang tidak relevan.

---

## 12. Skenario 8 - Menghitung Ulang Metrik (opsional)
Jika sistem menyediakan endpoint `recompute`, instruktur menjalankan fitur ini saat:
- instruktur memperbaiki data seed,
- instruktur menemukan inkonsistensi snapshot,
- instruktur ingin menghasilkan snapshot akhir sesi.

### Langkah Skenario 8
1. Buka Swagger.
2. Panggil `POST /api/v1/analytics/sessions/{sessionId}/recompute` (jika ada).
3. Buka ulang detail sesi pada Web.

---

## 13. Skenario 9 - Pengambilan Bukti untuk Laporan Tugas Akhir
Instruktur menyiapkan bukti uji dan bukti tampilan untuk laporan.

### Bukti yang instruktur siapkan
1. Tangkapan layar Swagger untuk uji endpoint utama:
   - create session,
   - activate ruleset version,
   - ingest event,
   - analytics summary.
2. Tangkapan layar Postman untuk skenario validasi input.
3. Tangkapan layar halaman Web:
   - daftar sesi,
   - detail sesi,
   - detail pemain,
   - detail ruleset.
4. Hasil query PostgreSQL untuk validasi:
   - `metric_snapshots`
   - `event_cashflow_projections`
   - `events`
   - integritas referensi event (`event_pk`) pada proyeksi/log

### Format penyimpanan bukti
Instruktur menyimpan bukti pada folder kerja atau media arsip pengujian yang disepakati, dengan pemisahan minimal untuk artefak `api`, `ui`, dan `db`.

---

## 14. Masalah Umum dan Solusi Cepat
### 14.1 Web menampilkan kesalahan `API tidak dapat diakses`
Instruktur melakukan:
1. cek API berjalan pada URL yang benar,
2. cek `ApiBaseUrl` pada `Cashflowpoly.Ui/appsettings.Development.json`,
3. cek sertifikat https jika memakai https.

### 14.2 Analitika kosong padahal event sudah terkirim
Instruktur melakukan:
1. cek event tersimpan pada tabel `events`,
2. cek proyeksi transaksi pada `event_cashflow_projections`,
3. pastikan proyeksi punya referensi event yang valid via `event_pk`,
4. cek snapshot pada `metric_snapshots`,
5. jalankan `recompute` jika endpoint tersedia.

### 14.3 Event ditolak terus
Instruktur melakukan:
1. cek urutan `sequence_number`,
2. cek `ruleset_version_id` sesuai sesi aktif,
3. cek struktur payload sesuai kontrak,
4. lihat `error_code` dan `trace_id`.

---

## 15. Ringkasan Alur Operasional
Instruktur menjalankan urutan ini pada setiap sesi:
1. buat sesi melalui Klien Game/IDN,
2. pilih ruleset melalui Klien Game/IDN,
3. tambahkan Player melalui Klien Game/IDN,
4. mulai sesi melalui Klien Game/IDN,
5. masukkan input keputusan Player dan kirim event berurutan,
6. buka Web Analitik untuk memantau metrik,
7. evaluasi detail pemain,
8. akhiri sesi,
9. simpan bukti uji dan tampilan.







