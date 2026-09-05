# Riwayat Perbaikan Modular dan Status Rilis

Status: **seluruh modul utama diterapkan**, diperbarui 30 Agustus 2026.

Dokumen ini menjelaskan masalah awal, perubahan yang diterapkan, bukti selesai, serta risiko yang masih diterima. Bagian `Commit` mencatat kelompok perubahan historis dan bukan perintah yang harus dijalankan ulang. Rencana yang belum diterapkan harus ditambahkan sebagai bagian baru dengan status dan kriteria selesai yang jelas.

## Ringkasan keputusan final

Cashflowpoly adalah platform analitik permainan fisik. IDN mencatat pembagian dan keputusan yang terjadi di meja; backend memvalidasi ruleset, saldo, peserta, giliran, proyeksi, serta analitik. Backend tidak mensimulasikan pasar atau deck.

Keputusan lintas modul:

- `LewatiOrder` dihapus. Hari normal wajib dua aksi utama; aksi yang sama boleh diulang.
- Jumat hanya untuk donasi, Sabtu hanya untuk transaksi emas, dan Minggu libur sesuai rulebook.
- Setup pertama mengunci peserta/ruleset, tetapi pembagian dapat direvisi sampai start.
- Start mengunci revisi terbaru dan membentuk event setup secara atomik.
- Data produksi dan event legacy dipertahankan; event aksi lewati dan deck/pasar diabaikan oleh analitik baru.
- API tetap pada `/api/v1` tanpa versi transisi.
- Tidak ada `security_version`, backup database, restore test, atau rollback schema.
- Produksi memakai `https://narafin.org`, Nginx, dan Cloudflare Tunnel.

---

## Modul 0 - Baseline dan perlindungan perubahan

### Sebelumnya

Perubahan besar sulit dibedakan dari regresi dan belum mempunyai baseline terpadu untuk API, Seed 2, analitik, UI, serta RBAC.

### Diperbaiki menjadi

- Perubahan dikerjakan pada branch penyelarasan terpisah sebelum digabungkan ke `prod`.
- Characterization test melindungi login, ruleset, sesi, peserta, setup, start/end, event, transaksi, proyeksi, analitik, dan hak akses.
- Dataset Seed 2 Pemula/Mahir serta kontrak utama dapat dibandingkan otomatis.

### Kriteria selesai

Seluruh test awal lulus dan saldo, event, poin, misi, serta proyeksi dataset acuan konsisten.

### Commit

`test: capture current api analytics and ui behavior`

---

## Modul 1 - Migrasi database dan Seed 2

### Sebelumnya

Schema dibentuk dari bootstrap SQL tanpa riwayat migrasi resmi. Setup hanya mempunyai satu snapshot dan seed dapat bercampur dengan bootstrap.

### Diperbaiki menjadi

- `schema_history` mencatat versi, nama, checksum SHA-256, dan waktu penerapan.
- Migrasi SQL berurutan dijalankan lewat `--migrate-only`; checksum yang berubah menghentikan proses.
- Database kosong menjalankan baseline dan seluruh migrasi; database lama diverifikasi sebelum baseline ditandai.
- `session_setup_revisions`, `app_users.is_demo`, dan indeks cursor ditambahkan tanpa `security_version`.
- Seed 2 idempoten, menandai akun demo, mengikuti rulebook, dan tidak menghapus data produksi.
- Tidak ada backup database sesuai keputusan proyek.

### Kriteria selesai

Database kosong dan baseline lama menghasilkan schema sama; migrasi ulang tidak mengubah hasil; checksum berbeda ditolak.

### Commit

`feat(db): add versioned migrations and setup revisions`

---

## Modul 2 - Setup fisik dan lifecycle sesi

### Sebelumnya

Backend dapat membagi kartu otomatis, setup langsung final, dan keadaan backend dapat berbeda dari meja.

### Diperbaiki menjadi

1. Instruktur membagikan kartu fisik sesuai rulebook.
2. IDN mencatat, memvalidasi tanpa simpan, menampilkan ringkasan, lalu menyimpan revisi.
3. Revisi pertama mengunci peserta/ruleset; revisi berikutnya hanya memperbaiki pembagian peserta/ruleset yang sama.
4. Start mengunci revisi terbaru dan membentuk event setup secara atomik.

Aturan setup:

- Pemula: kas awal 20, satu bahan dan biaya kartunya, satu emas bernilai 5, satu misi unik.
- Mahir: kas awal 10, komponen Pemula, pinjaman Syariah 10, serta proteksi gratis dari Tie Breaker.
- Tie Breaker unik menentukan urutan pemain.
- Token aksi tetap komponen fisik dan tidak disimpan sebagai setup.
- Nilai tetap berasal dari ruleset, bukan tebakan UI.
- Instruktur melihat semua misi; Player hanya misinya sendiri, termasuk setelah sesi selesai.

### Kriteria selesai

Retry identik idempoten; konflik ID berbeda menghasilkan `409`; roster tidak dapat berubah setelah setup pertama; start ganda hanya satu yang berhasil; event setup sama dengan revisi terkunci.

### Commit

`feat(sessions): align physical setup and lifecycle`

---

## Modul 3 - Gameplay sesuai rulebook

### Sebelumnya

Backend mengelola pasar/deck virtual, aksi lewati dianggap sah, dan event ditolak dapat mencemari statistik.

### Diperbaiki menjadi

- Aksi lewati dihapus dari katalog, validator, Seed 2, proyeksi, UI, Postman, dan dokumentasi.
- Hari normal wajib dua aksi utama; aksi yang sama boleh dilakukan dua kali.
- `AkhirGiliran` ditolak sebelum dua aksi utama sah.
- Jumat hanya donasi, Sabtu hanya emas, Minggu tanpa aksi pemain.
- Backend tidak mengacak, mengisi, atau memproyeksikan pasar/deck.
- Validasi kartu hanya memakai katalog, mode, saldo, kepemilikan yang dilaporkan, urutan, dan status sesi.
- Event ditolak tidak disimpan sebagai event/snapshot/metrik; log hanya berisi status, kode error, dan `trace_id`.
- Event legacy tetap disimpan tetapi tidak memengaruhi hasil baru.

### Kriteria selesai

Aksi lewati baru ditolak; dua aksi sama diterima; satu aksi lalu akhir giliran ditolak; API/UI tidak memiliki state pasar/deck; penolakan tidak menambah statistik domain.

### Commit

`fix(events): enforce rulebook actions and retire virtual market`

---

## Modul 4 - Kontrak API dan pagination

### Sebelumnya

Error belum konsisten, event memakai `fromSeq`, transaksi belum memiliki cursor stabil, dan daftar besar dapat tumbuh tanpa batas.

### Diperbaiki menjadi

- Bentuk error tetap `error_code`, `message`, `details`, `trace_id` dengan status 400/401/403/404/409/422/429/500 yang konsisten.
- Cursor hanya untuk event dan transaksi; default 50, maksimum 100.
- Respons memakai `items`, `next_cursor`, `has_more`.
- Event diurutkan `sequence_number`; transaksi mempunyai `transaction_id` dan diurutkan `timestamp, transaction_id`.
- Cursor Base64URL opaque yang rusak menghasilkan 400.
- Daftar sesi/pemain tetap penuh dan perubahan langsung berlaku pada `/api/v1`.

### Commit

`refactor(api): standardize contracts and cursor pagination`

---

## Modul 5 - Analitik yang dapat dibuktikan

### Sebelumnya

Nama variabel tidak konsisten, ada data tidak teramati, rumus bercampur satuan, dan data tidak tersedia dapat tampil sebagai nol.

### Diperbaiki menjadi

- `raw_json` memuat sebelas kelompok: `coins`, `ingredients`, `meal_orders`, `needs`, `donations`, `gold`, `pension`, `life_risk`, `financial_goals`, `actions`, `turns`.
- `derived_json` memuat tiga belas metrik baku: pertumbuhan kas, diversifikasi pendapatan, porsi biaya usaha, margin pesanan, kesiapan risiko, beban pinjaman, progres target, fokus aksi pendapatan, pemanfaatan bahan, porsi aksi jangka panjang, keragaman kebutuhan, komitmen donasi, dan komposisi poin.
- Setiap rincian menampilkan asal data, variabel, angka aktual, substitusi rumus, dan hasil.
- Pembagi nol/data kurang menghasilkan `null` dan “Belum dapat dihitung”.
- Metrik Mahir tidak dibentuk pada Pemula; histori dihitung ulang dengan event legacy diabaikan.

### Commit

`fix(analytics): derive verifiable player metrics`

---

## Modul 6 - Penyederhanaan UI

### Sebelumnya

Halaman pemain panjang, badge dekoratif menyerupai data, penjelasan berulang, komponen Mahir muncul pada Pemula, accordion tidak konsisten, dan kartu tidak rata.

### Diperbaiki menjadi

- Ringkasan Statistik, Cerita di Balik Hasil, dan Data Permainan Lengkap menjadi accordion dengan kontrol plus/minus seragam.
- Ringkasan terbuka pada kunjungan pertama; pilihan terakhir disimpan di `localStorage`.
- Semua badge jumlah, label item, kotak “Di luar jatah aksi”, watermark angka beranda, persentase/label Total Sesi pada kartu berjalan, dan kartu Pelanggaran Aturan dihapus.
- Kelompok Mahir tidak dirender pada Pemula.
- Kartu memakai grid responsif dengan tinggi/lebar konsisten; tabel panjang scroll horizontal.
- Komponen skor sesi menjadi accordion per pemain dan kalender memakai `finish_day`/fitur ruleset.
- Bahasa Indonesia default dan istilah ID/EN konsisten.

### Kriteria selesai

Desktop, ponsel, keyboard, screen reader, overflow, mode, dan persistensi accordion lulus.

### Commit

`fix(ui): remove redundant elements and simplify analytics`

---

## Modul 7 - Akun, privasi, dan metadata

### Sebelumnya

Metadata domain tersebar, tautan Privasi salah, Ketentuan belum ada, dan identitas akademik belum jelas.

### Diperbaiki menjadi

- JWT delapan jam, logout perangkat saat ini, dan rate limit login dipertahankan.
- Tidak ada profil, ganti password, logout-all, admin akun, atau token demo khusus.
- Registrasi publik hanya Player; Instruktur publik ditolak.
- Akun Seed 2 bertanda demo tetapi tetap mengikuti hak akses role.
- Halaman Kebijakan Privasi dan Ketentuan Penggunaan menjelaskan proyek akademik, retensi selama proyek, serta tidak adanya fitur salinan/penghapusan mandiri.
- Canonical, Open Graph, robots, dan sitemap berasal dari `DOMAIN`; production memakai `https://narafin.org`; halaman privat `noindex`.

### Commit

`feat(ui): add academic policies and configurable metadata`

---

## Modul 8 - Container, log, metrics, dan deployment

### Sebelumnya

Container belum seluruhnya non-root, image belum dipin, `/metrics` dapat diteruskan, log tidak konsisten, dan deployment tidak mempunyai rollback image terarah.

### Diperbaiki menjadi

- API/UI/Nginx non-root, image eksternal dipin, dan `no-new-privileges` diterapkan.
- PostgreSQL, API, UI, dan `/metrics` tidak diekspos langsung; Nginx menolak `/metrics`; Cloudflare Tunnel tetap digunakan.
- Health check tersedia pada seluruh service.
- Audit login, setup, lifecycle, ruleset, deployment, dan demo disimpan 30 hari; payload gameplay ditolak tidak disimpan.
- Journald memakai retensi 30 hari dan batas disk.
- `scripts/deploy-production.sh` memakai lock, release SHA, build berurutan, maintenance, migrasi, Seed 2, rekalkulasi, health/smoke, rollback image otomatis, serta retensi dua rilis.
- Database tidak diturunkan saat rollback dan tidak dibuatkan backup.

### Commit

`chore(infra): harden containers logs and manual deployment`

---

## Modul 9 - Dokumentasi, OpenAPI, dan Postman

### Sebelumnya

README, desain database, Postman, dan dokumen implementasi masih menyebut setup/pasar otomatis, aksi lewati, satu setup final, rumus lama, serta backup yang tidak dipilih.

### Diperbaiki menjadi

- README utama memuat orientasi dan alur penggunaan; kontrak API serta arsitektur database menjadi dokumen kanonis pada `docs/02-Perancangan`.
- OpenAPI runtime, DTO, Postman, migrasi, dan seluruh `docs` diselaraskan.
- Alur IDN didokumentasikan: login, ruleset, sesi, peserta, validasi setup, simpan/revisi, start, event, end.
- Dokumentasi meliputi Pemula/Mahir, pagination, error, privasi misi, setup revision, data legacy, sebelas kelompok variabel, tiga belas metrik, deployment manual, tunnel, tanpa backup, dan risiko rollback schema.
- Dokumentasi tidak lagi menjadikan pasar/deck sebagai state backend atau aksi lewati sebagai gameplay sah.

### Commit

`docs: synchronize api database postman and rulebook`

---

## Modul 10 - Pengujian, commit, push, dan rilis

### Sebelumnya

Belum ada satu gerbang verifikasi lokal yang mencakup build, keamanan dependency, kontrak, UI lintas viewport, Docker, dokumentasi, dan performa.

### Diperbaiki menjadi

Satu skrip lokal menjalankan:

- restore/build .NET dengan warning sebagai error;
- unit dan integration test PostgreSQL;
- contract/OpenAPI/Postman test;
- audit kerentanan NuGet/npm;
- E2E Chromium desktop dan ponsel;
- konsistensi dokumentasi;
- build/config Docker;
- performa kebutuhan resmi: 100 akun, 20 sesi aktif dengan 2.000 event per sesi, 20 pengguna bersamaan, 200 permintaan per endpoint, P95 ingest event 251,9 ms (batas <=500 ms), dan P95 analitika sesi 458,4 ms (batas <=1.500 ms). Beban sintetis tiap sesi mencakup 64 aksi kerja lepas/pembelian bahan dan 1.936 transaksi sistem terkait dua pemain beserta proyeksi; POST terukur memakai CatatTransaksi.

Setiap modul diuji dan dibuatkan Conventional Commit. Setelah semua gerbang lulus, perubahan digabungkan ke `prod`, didorong ke origin, lalu deployment manual memasang commit terbaru dan memverifikasi login, Seed 2, sesi, setup, analitik, ruleset, legal, health, dan `narafin.org`.

### Commit

`test: add release e2e contract and performance gates`

---

## Modul 11 - Audit Source Code dan Konsolidasi Dokumentasi

### Sebelumnya

Respons kegagalan model binding otomatis masih memakai format bawaan ASP.NET dan berbeda dari kontrak error publik. Dokumentasi API/database juga dipelihara ganda pada root dan `docs`, sedangkan rancangan Bab IV serta dokumen biner ikut dilacak Git.

### Diperbaiki menjadi

- respons model binding memakai `error_code`, `message`, `details`, dan `trace_id`, termasuk terjemahan Inggris;
- audit independen berbasis source code memeriksa logika domain, seluruh operasi OpenAPI, database, RBAC, keamanan, UI desktop/mobile, dan runtime Docker;
- kontrak API, arsitektur database, keputusan, serta riwayat perbaikan masing-masing memiliki satu dokumen kanonis di `docs`;
- rancangan Bab IV, Word, draw.io, screenshot, PDF hasil render, dan bukti visual ditempatkan pada `artifacts` lokal yang tidak dilacak Git.

### Bukti selesai

- build Release bersih tanpa warning/error;
- 142 dari 142 pemeriksaan logika source lulus;
- 132 dari 132 pemeriksaan HTTP lulus untuk 36 operasi Swagger;
- constraint dan relasi database tervalidasi tanpa data yatim;
- UI desktop/mobile serta bahasa Indonesia/Inggris tidak menghasilkan error console atau overflow halaman;
- pemeriksaan konsistensi dokumentasi dan tautan lokal lulus.

---

## Kontrak publik final

### Setup

- `POST /api/v1/sessions/{sessionId}/setup/validate`
- `POST /api/v1/sessions/{sessionId}/setup`
- `GET /api/v1/sessions/{sessionId}/setup`

Respons memuat `revision`, `setup_status` (`EDITABLE`/`LOCKED`), `saved_at`, dan `locked_at`. Revisi baru menghasilkan 201; retry identik menghasilkan 200.

### Pagination

Event dan transaksi menerima `cursor` serta `limit`, lalu mengembalikan `items`, `next_cursor`, dan `has_more`. Transaksi mempunyai `transaction_id`.

### Kontrak yang dihapus

- aksi lewati;
- variabel pesanan dilewati;
- metrik pelanggaran aturan;
- state/proyeksi pasar dan deck virtual;
- statistik dari event gameplay ditolak.

## Risiko yang diterima

- IDN merupakan proyek terpisah; repository ini hanya menyediakan kontrak API.
- Production memasang commit terbaru `prod`, bukan tag SemVer.
- Tidak ada GitHub Actions, staging, atau pemaksaan gerbang test oleh GitHub.
- Deployment memakai akun VPS `root` dan boleh memiliki jeda pemeliharaan singkat.
- Akun demo memakai password tetap dan akses penuh sesuai role.
- Tidak ada backup; kegagalan VPS/operator/migrasi dapat menyebabkan kehilangan permanen.
- Rollback hanya aplikasi, bukan schema.
- Data produksi dipertahankan, tetapi analitik historis dapat berubah setelah koreksi formula dan pengabaian event legacy.
