# Pengujian Tahapan dan Roadmap Implementasi
## Cashflowpoly Analytics Platform

### Informasi Dokumen
- **Nama Dokumen**: Pengujian Tahapan dan Roadmap Implementasi
- **Versi**: 1.4
- **Tanggal**: 20 Juni 2026
- **Penyusun**: Marco Marcello Hugo

---

## 1. Tujuan Dokumen
Dokumen ini disusun untuk menetapkan pentahapan kerja (*roadmap*), urutan pengerjaan fitur, serta kriteria kualitatif *Definition of Done* (DoD) untuk memastikan seluruh fungsionalitas sistem informasi analitika Cashflowpoly selesai diuji dan siap rilis.

---

## 2. Urutan dan Tahapan Implementasi

Roadmap pengerjaan dibagi menjadi 10 tahap pengerjaan terukur:

### Tahap A - Inisiasi Proyek (Bootstrap)
1. Menyiapkan repositori git dan menginisiasi dua proyek utama (`Cashflowpoly.Api` dan `Cashflowpoly.Ui`).
2. Menghubungkan solution `.NET` (`Cashflowpoly.sln`) dan melakukan verifikasi build awal.
3. Mengaktifkan Swagger UI pada environment development backend API.

### Tahap B - Integrasi Basis Data
1. Menjalankan skrip inisiasi DDL `database/00_create_schema.sql` pada PostgreSQL.
2. Memverifikasi pembuatan tabel inti (sessions, rulesets, events, metric_snapshots, logs).
3. Melakukan seeding ruleset default (mode pemula dan mahir) menggunakan skrip `database/01_seed_default_rulesets_components.sql`.

### Tahap C - Autentikasi dan Otorisasi (RBAC)
1. Mengembangkan endpoint login dan register akun pada API.
2. Menerapkan pengamanan JWT Bearer token pada endpoint API yang terproteksi.
3. Menerapkan otorisasi peran (`INSTRUCTOR` dan `PLAYER`) di tingkat backend dan frontend UI.
4. Menyimpan token login secara aman di session server-side Web MVC.

### Tahap D - Sesi dan Manajemen Ruleset
1. Menyediakan endpoint API untuk alur hidup sesi (`create`, `list`, `start`, `end`).
2. Menyediakan endpoint CRUD ruleset beserta aktivasi versi ruleset di tingkat ruleset.
3. Menambahkan penguncian ruleset (`is_locked_by_session`) jika ruleset sedang berjalan di suatu sesi.

### Tahap E - Ingest Event & Validasi Domain
1. Mengembangkan pipeline ingestion event permainan (`POST /api/v1/events` dan `/events/batch`).
2. Menerapkan validasi urutan event (`sequence_number`) dan idempotensi request `(session_id, event_id)`.
3. Menulis event yang tidak valid ke dalam tabel `validation_logs`.

### Tahap F - Pembuatan Proyeksi dan Snapshot
1. Memproses event valid untuk memperbarui saldo koin, inventaris bahan, emas, asuransi, dan pinjaman.
2. Membentuk baris proyeksi transaksi di tabel `event_cashflow_projections`.
3. Menghitung snapshot metrik berkala dan menyimpannya di `metric_snapshots`.

### Tahap G - Penyediaan Endpoint Analitika API
1. Endpoint `GET /api/v1/analytics/sessions/{id}` (Ringkasan analitika sesi).
2. Endpoint `GET /api/v1/analytics/sessions/{id}/transactions` (Histori transaksi pemain).
3. Endpoint `GET /api/v1/analytics/sessions/{id}/players/{userId}/gameplay` (Metrik detail pemain).
4. Endpoint `GET /api/v1/analytics/rulesets/{id}/summary` (Agregasi performa ruleset).

### Tahap H - Antarmuka Dashboard MVC
1. Mengintegrasikan rute navigasi dasbor, daftar sesi, dan detail sesi.
2. Membuat tampilan performa grafis pemain individu beserta histori transaksinya.
3. Mengintegrasikan formulir pembuatan, pengeditan, dan aktivasi ruleset untuk Instruktur.
4. Menyediakan redirect rute `/analytics` dan halaman buku aturan.

### Tahap I - Pengujian Integrasi & Deployment
1. Menjalankan seluruh rangkaian pengujian fungsional unit testing dan integration testing.
2. Menguji seluruh endpoint API menggunakan Postman collection.
3. Mengemas seluruh layanan ke dalam Docker Compose (dev dan prod) serta memverifikasi status kesehatannya.

### Tahap J - Observabilitas & Hardening Keamanan
1. Mengaktifkan endpoint Prometheus `/metrics` untuk metrik operasional.
2. Menambahkan trace context (`trace_id`, `span_id`) terstruktur pada logging dan header respons.
3. Menerapkan rotasi kunci JWT (*multi-key rotation*) berbasis `kid`.
4. Mengimplementasikan logging audit keamanan ke tabel `security_audit_logs`.

---

## 3. Checklist Kriteria Definition of Done (DoD)
Fitur dinyatakan selesai dan siap rilis jika memenuhi checklist berikut:

- [ ] Lulus pengujian fungsional kasus uji pada dokumen rencana pengujian.
- [ ] Kontrak payload JSON API backend sesuai spesifikasi dan tidak melanggar kesepakatan integrasi.
- [ ] Dokumentasi desain dan status implementasi sinkron dengan fungsionalitas aktual.
- [ ] Build solution .NET sukses 100% tanpa peringatan error (`/warnaserror`).
- [ ] Seluruh unit testing dan integration testing (xUnit + Testcontainers) lulus 100%.
- [ ] Endpoint observabilitas, pemeriksaan kesehatan, dan log audit keamanan terverifikasi aktif.
- [ ] Target performa load-test baseline terpenuhi (P95 ingest event <= 500 ms, P95 analitika <= 1500 ms).
- [ ] Tidak ada bug blocker kritis yang masih terbuka di modul repositori utama.
