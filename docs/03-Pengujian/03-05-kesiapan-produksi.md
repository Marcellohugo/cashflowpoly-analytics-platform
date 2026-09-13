# Verifikasi kesiapan produksi

Tanggal pemeriksaan: 13 September 2026. Dokumen ini mencatat verifikasi perbaikan audit setelah rilis `59552ec`. Status rilis aktif diperiksa melalui log deployment dan symlink `current` di server.

## Perbaikan audit

- Deployment menyalakan API/UI/Nginx terlebih dahulu, membuka maintenance setelah API/UI siap, lalu menyalakan Cloudflare Tunnel. Mengulang SHA yang sama tidak terjebak pada health check Nginx dan tetap menyimpan rilis rollback. Kegagalan pembersihan setelah aktivasi tidak membatalkan rilis sehat.
- Statistik memakai endpoint gabungan untuk roster dan gameplay. Halaman pemain membutuhkan paling banyak dua permintaan API dan halaman instruktur tiga, terlepas dari jumlah sesi. Perhitungan menggunakan kalkulator yang sama dengan analitika per sesi; mode, kepemilikan sesi, dan identitas pemain tetap diperiksa di server.
- Pilihan nama pemain membedakan nama identik atau yang hanya berbeda huruf besar/kecil. Masukan ambigu tidak diam-diam memilih UUID pertama.
- Waktu statis dan aktivitas langsung mengikuti zona waktu browser, dengan label zona. HTML menyimpan waktu ISO dan menyediakan teks UTC sebelum JavaScript berjalan.
- Seed 2 tetap aktif di production. Data berisi 24 sesi: 16 selesai, 4 persiapan, dan 4 berjalan. Masing-masing instruktur memiliki empat sesi selesai per mode, satu persiapan per mode, dan satu berjalan per mode.
- Hadziq dan Pratama masing-masing memiliki 10 konfigurasi ruleset berbeda yang terpakai. Versi baru mengubah modal awal dan pendapatan kerja lepas; nilai kejadian mengikuti versinya. Versi lama yang digunakan sesi lain dipertahankan. Waktu berakhir sesi mencakup seluruh kejadian.
- Komentar otomatis yang mengulang setiap baris kode dibersihkan. CSS produksi diminifikasi dengan tool yang sudah tersedia. `site.css` turun dari 1.244.905 menjadi 135.884 byte, sekitar 89%, tanpa mengubah aturan tampilannya.
- README, kontrak endpoint gabungan, Postman, dan runbook memakai implementasi serta lokasi repository VPS yang aktual. Pesan API baru memiliki terjemahan Inggris.

## Bukti pemeriksaan

| Pemeriksaan | Hasil |
|---|---|
| Build Release `/warnaserror` | Lulus, 0 warning dan 0 error |
| Unit, integrasi PostgreSQL, kontrak API | 369 lulus |
| Pengujian UI .NET | 406 lulus |
| Performa 100 akun, 20 sesi uji aktif, 20 klien bersamaan, 2.000 kejadian per sesi | 1 lulus; P95 ingest ≤500 ms dan analitika ≤1.500 ms |
| Statistik 160 sesi yang dimainkan ditambah skenario batas | Cakupan akun, pemisahan mode, null untuk persiapan, dan kesamaan dengan endpoint per sesi lulus |
| E2E Chromium desktop dan ponsel | 70 skenario lulus: 66 pada run lengkap dan 4 pada pengulangan setelah asumsi tes diperbarui |
| Audit NuGet termasuk dependensi transitif | Tidak ditemukan paket rentan |
| Audit npm UI dan E2E | 0 kerentanan |
| Konfigurasi produksi lokal, Compose, dan konsistensi dokumentasi | Lulus |
| Sintaks Bash, PowerShell, JavaScript frontend | Lulus |
| Regresi deployment terisolasi | 4 skenario lulus; pengujian Compose/Nginx aktual juga lulus |
| Image produksi pada PostgreSQL kosong sementara | Migrasi dua kali, startup, UI/aset, API terproteksi, Swagger nonaktif, dan rekalkulasi lulus |
| Seed 2 pada image produksi | 8 akun, 24 sesi, 22 ruleset; 10 per instruktur dan 2 bawaan; tidak mengganda |
| Snapshot hasil rekalkulasi produksi | 80 `gameplay.raw.variables` dan 80 `gameplay.derived.metrics` |
| Perlindungan data di luar seed | Uji terisolasi memastikan sesi di luar 24 UUID dan versi aturan yang dipakainya tidak berubah |
| `git diff --check` | Lulus |

Run lengkap menemukan dua asumsi tes browser yang masih memakai data lama: delapan sesi instruktur dan ketiadaan semua tautan pada sesi persiapan. Asumsi diperbaiki menjadi 12 sesi serta larangan khusus tautan analitika; keempat kasus desktop/ponsel kemudian lulus pada pengulangan. Build image dan regresi deployment dijalankan terpisah setelah tahap browser. Tidak ada tahap pemeriksaan yang dilewati.

Seed diuji melalui replay legal seluruh kejadian, pemeriksaan saldo terhadap kalkulator, serta penerapan ulang deterministik. Dua sesi awal tetap menjadi acuan angka regresi. Tes beban menghitung 20 sesi khusus pengukuran, terpisah dari empat sesi demo yang sedang berjalan.

Log lokal: `artifacts/audit-fixes-release.log`, `artifacts/audit-fixes-browser-recheck.log`, `artifacts/audit-fixes-deployment-tests.log`, `artifacts/audit-fixes-production-smoke.log`, `artifacts/audit-production-readiness.log`, `artifacts/batch-api-tests.log`, `artifacts/seed-diversity-test.log`, dan `artifacts/deployment-nginx-smoke.log`. Artefak pengujian tidak masuk image Docker.

## Pelaksanaan rilis

Gunakan [runbook deployment produksi](../00-Panduan/00-04-panduan-deployment-produksi.md). Repository VPS berada di `/root/cashflowpoly-analytics-platform`, environment di `config/env/.env.prod` dalam repository itu, dan worktree rilis di `/opt/cashflowpoly/releases`. Skrip memperoleh path repository dari lokasi skrip; penggantian environment/path tetap tersedia melalui variabel yang didokumentasikan.

`DATABASE_MIGRATIONS_SEED_SIMULATION=true` tetap berlaku dan rekalkulasi wajib mengikuti migrasi. Setelah deploy, periksa domain/TLS, health lima service, login kedua peran, jumlah seed, serta data sesi/partisipan/kejadian di luar seed.

Kebijakan proyek tetap memakai deployment manual tanpa GitHub Actions. Sesuai keputusan proyek yang sudah tercatat, rollback memulihkan image aplikasi; backup dan rollback database belum tersedia.
