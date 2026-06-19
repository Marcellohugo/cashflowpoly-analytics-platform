# Indeks Dokumentasi Cashflowpoly Analytics Platform

Selamat datang di direktori dokumentasi proyek. Seluruh dokumen disusun berdasarkan kategori numerik terurut untuk mempermudah pemahaman arsitektur sistem informasi dari aspek panduan, spesifikasi, rancangan, hingga pengujian.

---

## 📂 Struktur Kategori Dokumentasi

### 00. Panduan Penggunaan (`00-Panduan/`)
*Kumpulan dokumen instruksi instalasi, operasional, dan pengamanan sistem.*
- [00-01-panduan-setup-lingkungan.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/00-Panduan/00-01-panduan-setup-lingkungan.md): Setup prasyarat perkakas, basis data lokal, konfigurasi JWT, dan pembagian tanggung jawab modul.
- [00-02-panduan-manual-pengguna-dashboard.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/00-Panduan/00-02-panduan-manual-pengguna-dashboard.md): Manual penggunaan antarmuka web dashboard analitik bagi Instruktur dan Player.
- [00-03-panduan-menjalankan-sistem.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/00-Panduan/00-03-panduan-menjalankan-sistem.md): Cara menjalankan server API dan UI MVC secara lokal maupun troubleshoot awal.
- [00-04-panduan-deployment-produksi.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/00-Panduan/00-04-panduan-deployment-produksi.md): Langkah deployment production menggunakan Docker Compose, Nginx, dan Cloudflare Tunnel.
- [00-05-panduan-alur-dan-hak-akses.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/00-Panduan/00-05-panduan-alur-dan-hak-akses.md): Alur peran pengguna konseptual dan batasan otorisasi RBAC sistem.

### 01. Spesifikasi Sistem (`01-Spesifikasi/`)
*Definisi kebutuhan perangkat lunak, rancangan kontrak API, aturan ruleset, dan skenario permainan.*
- [01-01-spesifikasi-kebutuhan-sistem.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/01-Spesifikasi/01-01-spesifikasi-kebutuhan-sistem.md): Kebutuhan SRS (Software Requirements Specification), aktor, dan batasan fungsional.
- [01-02-spesifikasi-ruleset-dan-validasi.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/01-Spesifikasi/01-02-spesifikasi-ruleset-dan-validasi.md): Parameter aturan ruleset, alur validasi backend, dan mutabilitas versi ruleset.
- [01-03-spesifikasi-integrasi-dan-keamanan.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/01-Spesifikasi/01-03-spesifikasi-integrasi-dan-keamanan.md): Mekanisme integrasi Klien Game/IDN, non-fungsional requirement (NFR), audit trail, dan log keamanan.
- [01-04-spesifikasi-diagram-uml.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/01-Spesifikasi/01-04-spesifikasi-diagram-uml.md): Kebutuhan Diagram UML sistem (Use Case, Sequence, Class Diagram).
- [01-05-spesifikasi-skenario-simulasi.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/01-Spesifikasi/01-05-spesifikasi-skenario-simulasi.md): Skenario simulasi permainan pemula/mahir serta skenario operasional daur hidup sesi API.

### 02. Perancangan Sistem (`02-Perancangan/`)
*Desain basis data, formula kalkulasi metrik, dan desain mockup antarmuka MVC.*
- [02-01-rancangan-database-dan-model-data.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/02-Perancangan/02-01-rancangan-database-dan-model-data.md): Kamus data PostgreSQL, skema relasional, indeks, dan riwayat keputusan normalisasi.
- [02-02-rancangan-kontrak-api-dan-event.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/02-Perancangan/02-02-rancangan-kontrak-api-dan-event.md): Kontrak payload API endpoint `/api/v1/...` dan skema event.
- [02-03-rancangan-definisi-dan-agregasi-metrik.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/02-Perancangan/02-03-rancangan-definisi-dan-agregasi-metrik.md): Formula metrik analitik dasar dashboard, variabel fisik permainan, dan rumus metrik turunan.
- [02-04-rancangan-antarmuka-dan-viewmodel-mvc.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/02-Perancangan/02-04-rancangan-antarmuka-dan-viewmodel-mvc.md): Desain mockup layout UI MVC, ViewModel C#, dan mapping call API ke UI.

### 03. Pengujian & Progress (`03-Pengujian/`)
*Rencana kasus uji, laporan pengujian baseline, checklist status, dan roadmap pengerjaan.*
- [03-01-pengujian-rencana-dan-kasus-uji.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/03-Pengujian/03-01-pengujian-rencana-dan-kasus-uji.md): Rencana pengujian fungsional unit test, integrasi, dan RBAC.
- [03-02-pengujian-laporan-hasil-baseline.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/03-Pengujian/03-02-pengujian-laporan-hasil-baseline.md): Laporan hasil pengujian baseline sistem.
- [03-03-pengujian-status-kesesuaian-implementasi.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/03-Pengujian/03-03-pengujian-status-kesesuaian-implementasi.md): Checklist pelacakan implementasi terhadap target spesifikasi.
- [03-04-pengujian-tahapan-dan-roadmap-implementasi.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/03-Pengujian/03-04-pengujian-tahapan-dan-roadmap-implementasi.md): Milestone pengerjaan dan checklist Definition of Done (DoD).

---

## 📚 Dokumen Root Lainnya
- [00-ringkasan-rulebook-cashflowpoly.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/00-ringkasan-rulebook-cashflowpoly.md): Ringkasan rulebook Cashflowpoly untuk referensi cepat.
- [01-ringkasan-proposal-tugas-akhir.md](file:///c:/Users/marco/cashflowpoly-analytics-platform/docs/01-ringkasan-proposal-tugas-akhir.md): Ringkasan proposal tugas akhir dan konteks akademik.

