# Manifest Fungsi File Repositori

Manifest diperbarui: 16 September 2026 (sinkronisasi audit UI, akses data, dan dokumentasi).

Dokumen ini merangkum fungsi file dan family file aktif pada repository, khususnya setelah dilakukan penyederhanaan struktur berkas dokumentasi di bawah folder `docs/`.

## Root, Konfigurasi, dan Infrastruktur
| Path | Kategori | Fungsi |
|---|---|---|
| `README.md` | Root | Entry point dokumentasi repository, setup, endpoint utama, dan tautan dokumen desain. |
| `Cashflowpoly.sln` | Root | Solution .NET untuk API, UI, dan test project. |
| `.gitattributes` | Root | Aturan atribut Git lintas platform. |
| `.gitignore` | Root | Daftar file/folder yang tidak dilacak Git. |
| `config/env/.env.example` | Konfigurasi | Template environment fallback. |
| `config/env/.env.dev.example` | Konfigurasi | Template environment development Docker Compose. |
| `config/env/.env.prod.example` | Konfigurasi | Template environment production. |
| `infra/docker/docker-compose.yml` | Infrastruktur | Definisi service dasar `db`, `api`, dan `ui`. |
| `infra/docker/docker-compose.watch.yml` | Infrastruktur | Override development/watch. |
| `infra/docker/docker-compose.prod.yml` | Infrastruktur | Override production, Nginx, dan Cloudflare Tunnel. |
| `infra/nginx/default.conf` | Infrastruktur | Reverse proxy Nginx production untuk UI, API, health, dan static asset. |
| `infra/cloudflared/config.yml` | Infrastruktur | Konfigurasi Cloudflare Tunnel. |
| `scripts/Invoke-ReleaseVerification.ps1` | Otomasi | Gerbang rilis lokal untuk build, test, audit, dokumentasi, performa, E2E, Compose, migrasi, Seed 2, dan image Docker. |
| `scripts/Test-DocumentationConsistency.ps1` | Otomasi | Memastikan README, database, Postman, endpoint, pagination, serta nama metrik tetap selaras. |
| `scripts/Test-ProductionReadiness.ps1` | Otomasi | Memeriksa environment dan konfigurasi sebelum deployment production. |
| `scripts/deploy-production.sh` | Otomasi | Deployment production ber-lock, migrasi, health/smoke test, dan rollback image aplikasi. |

## Database dan Integrasi

Migrasi `database/migrations/V005__event_consistency.sql` memperbaiki kontrak aktor dan efek harga tanpa menulis ulang migrasi yang telah dirilis. `RulesetDefinitionShapeValidator.cs` menjaga bentuk koleksi ruleset sebelum dipakai; `AnalyticsCollectionMissions.cs` mengevaluasi persyaratan misi dari katalog.

Migrasi `database/migrations/V006__initial_and_mission_happiness.sql` memasukkan poin awal dan hadiah misi ke komponen skor final historis serta menjaga pemberian hadiah misi tepat sekali pada proyeksi state. `ScoringConsistencyIntegrationTests.cs` menguji skor sebelum/sesudah finalisasi, recompute, dan koreksi data lama. `InitialSavingIntegrationTests.cs` menguji penggunaan tabungan awal lintas tujuan; `Test-IngressNetworking.ps1` memeriksa alokasi alamat Nginx dan tunnel pada jaringan baru.

Migrasi `database/migrations/V007__narrative_action_slot_guard.sql` memisahkan akses kolom pada trigger bersama berdasarkan tabel pemanggil. `MealSaleIntegrationTests.cs` menguji penjualan masakan beserta narasi dengan katalog bawaan Pemula dan Mahir.

Migrasi `database/migrations/V008__life_risk_player_resolution.sql` menambahkan penyelesaian biaya risiko massal per pemain, menjaga kompatibilitas proyeksi draw lama, dan menyelaraskan saldo sesi berjalan dengan ledger. `V009__gold_risk_price_refresh.sql` menambahkan pembaruan harga setiap kartu emas. `V010__active_risk_and_gold_guards.sql` memasang validasi risiko dan emas pada trigger aktif tanpa mengubah checksum V008/V009 yang sudah terpasang. `LifeRiskMassIntegrationTests.cs`, `LifeRiskTransferIntegrationTests.cs`, `LifeRiskSqlProjectionIntegrationTests.cs`, dan `GoldRiskPriceRefreshIntegrationTests.cs` menguji pilihan asuransi/bayar, transfer ulang tahun, kompatibilitas/proyeksi ulang SQL, serta pembaruan harga dan transaksi seluruh pemain.

| Path | Kategori | Fungsi |
|---|---|---|
| `database/00_create_schema.sql` | Database | DDL kanonis schema PostgreSQL event-first. |
| `database/01_seed_default_rulesets_components.sql` | Database | Seed ruleset default dan katalog komponen gameplay. |
| `database/02_seed_simulation_sessions_events.sql` | Database | Seed simulasi manual untuk sesi dan event contoh. |
| `postman/Cashflowpoly.postman_collection.json` | Integrasi | Collection Postman untuk smoke/API/RBAC flow. |
| `postman/Cashflowpoly.local.postman_environment.json` | Integrasi | Environment lokal Postman. |

## Pengujian Otomatis

Regresi audit menyeluruh berada pada `RulesetRosterAuditTests`, `EventContractRegressionTests`, `AnalyticsAuditRegressionTests`, dan `AnalyticsAuditIntegrationTests`. Regresi browser ada di `tests/e2e/specs/audit-fixes.spec.js`; identitas proxy diuji oleh `tests/deployment/Test-NginxClientIdentity.ps1`. Status dan alasan perubahan tercatat pada [laporan perbaikan audit](03-Pengujian/03-07-perbaikan-audit-menyeluruh.md).

| Path | Kategori | Fungsi |
|---|---|---|
| `tests/Cashflowpoly.Api.Tests/OpenApiContractIntegrationTests.cs` | Contract test | Memastikan OpenAPI dan bentuk error autentikasi sesuai kontrak publik. |
| `tests/Cashflowpoly.Api.Tests/ReleasePerformanceIntegrationTests.cs` | Performance test | Menguji 100 akun, 20 sesi aktif, dan 20 pengguna bersamaan terhadap target p95 API/analitik. |
| `tests/e2e/playwright.config.js` | E2E | Konfigurasi Chromium desktop dan Pixel 7. |
| `tests/e2e/specs/release-gate.spec.js` | E2E | Memeriksa login, UI nonredundan, accordion, mode Pemula, halaman sesi, dan overflow. |

## Dokumentasi (`docs/`)
| Path | Kategori | Fungsi |
|---|---|---|
| `docs/README.md` | Dokumen | Indeks panduan utama navigasi berkas dokumentasi. |
| `docs/00-ringkasan-rulebook-cashflowpoly.md` | Dokumen | Ringkasan rulebook Cashflowpoly untuk referensi cepat. |
| `docs/01-ringkasan-proposal-tugas-akhir.md` | Dokumen | Ringkasan proposal tugas akhir dan konteks akademik. |
| `docs/02-peta-alur-sistem-end-to-end.md` | Dokumen | Peta alur lintas rulebook, ruleset, sesi, event, database, analitika, UI, pengujian, dan deployment. |
| `docs/03-manifest-fungsi-file-repositori.md` | Dokumen | Manifest fungsi file repository (dokumen ini). |
| `docs/00-Panduan/00-01-panduan-setup-lingkungan.md` | Panduan | Setup Windows, .NET, PostgreSQL, JWT, dan pembagian tanggung jawab modul. |
| `docs/00-Panduan/00-02-panduan-manual-pengguna-dashboard.md`| Panduan | Manual navigasi halaman dashboard Web analitik untuk Instruktur/Player. |
| `docs/00-Panduan/00-03-panduan-menjalankan-sistem.md` | Panduan | Cara menjalankan API/UI secara lokal dan troubleshoot awal. |
| `docs/00-Panduan/00-04-panduan-deployment-produksi.md` | Panduan | Deployment produksi menggunakan Docker Compose, Nginx, dan Cloudflare Tunnel. |
| `docs/00-Panduan/00-05-panduan-alur-dan-hak-akses.md` | Panduan | Matriks alur peran pengguna konseptual dan otorisasi RBAC umum. |
| `docs/01-Spesifikasi/01-01-spesifikasi-kebutuhan-sistem.md` | Spesifikasi | Dokumen SRS (Software Requirements Specification) dan aktor pengguna. |
| `docs/01-Spesifikasi/01-02-spesifikasi-ruleset-dan-validasi.md` | Spesifikasi | Definisi parameter ruleset, alur validasi, dan mutabilitas versi. |
| `docs/01-Spesifikasi/01-03-spesifikasi-integrasi-dan-keamanan.md` | Spesifikasi | Integrasi IDN, parameter NFR, logging keamanan, dan audit. |
| `docs/01-Spesifikasi/01-04-spesifikasi-diagram-uml.md` | Spesifikasi | Daftar diagram UML (Use Case, Sequence, Class Diagram). |
| `docs/01-Spesifikasi/01-05-spesifikasi-skenario-simulasi.md` | Spesifikasi | Skenario simulasi pemula/mahir dan skenario operasional daur hidup sesi API. |
| `docs/01-Spesifikasi/01-06-keputusan-produk-dan-arsitektur.md` | Spesifikasi | Alasan keputusan final produk, gameplay, setup, analitik, UI, keamanan, infrastruktur, dan rilis. |
| `docs/02-Perancangan/02-01-arsitektur-database-dan-model-data.md` | Perancangan | Arsitektur PostgreSQL, kamus data, relasi, view, constraint, trigger, migrasi, seed, dan runbook schema. |
| `docs/02-Perancangan/02-02-kontrak-rest-api-dan-event-permainan.md` | Perancangan | Autentikasi, header, error, endpoint API, payload event, ruleset, analitika, OpenAPI, dan Postman. |
| `docs/02-Perancangan/02-03-rancangan-definisi-dan-agregasi-metrik.md` | Perancangan | Formula kalkulasi metrik analitik dashboard dan metrik turunan gameplay fisik. |
| `docs/02-Perancangan/02-04-rancangan-antarmuka-dan-viewmodel-mvc.md` | Perancangan | Desain mock-up visual UI MVC, ViewModel, dan pemetaan call API. |
| `docs/03-Pengujian/03-01-pengujian-rencana-dan-kasus-uji.md` | Pengujian | Rencana pengujian fungsional unit testing, integration, dan smoke test. |
| `docs/03-Pengujian/03-02-pengujian-laporan-hasil-baseline.md` | Pengujian | Laporan hasil eksekusi pengujian baseline sistem. |
| `docs/03-Pengujian/03-03-pengujian-status-kesesuaian-implementasi.md`| Pengujian | Checklist status kesesuaian implementasi kode terhadap spesifikasi. |
| `docs/03-Pengujian/03-04-riwayat-perbaikan-modular-dan-status-rilis.md`| Pengujian | Masalah awal, perbaikan per modul, bukti selesai, kontrak final, status rilis, dan risiko yang diterima. |
| `docs/Img/RuleBook/*.png` | Aset dokumen | Scan/gambar halaman rulebook untuk lampiran dokumen. |

*(Bagian file src/ dan tests/ tidak berubah, silakan lihat rincian lengkapnya pada repositori kode).*

## Tambahan audit September 2026
| Path | Fungsi |
|---|---|
| `src/Cashflowpoly.Api/Domain/DonationVisibility.cs` | Menyaring donasi rahasia serta proyeksi yang mendahului pembacaan event dari analitika publik. |
| `tests/Cashflowpoly.Api.Tests/DonationVisibilityTests.cs` | Regresi kerahasiaan jumlah donasi saat pembacaan event dan proyeksi bersamaan. |
| `docs/03-Pengujian/03-05-kesiapan-produksi.md` | Bukti kesiapan dan deployment baseline terdahulu. |
| `docs/03-Pengujian/03-06-perbaikan-audit-ui-data-dan-dokumentasi.md` | Perubahan audit lanjutan, alasan, dan verifikasi lokal. |
| `database/migrations/V011__shared_savings_first_purchase.sql` | Menggabungkan setoran sebagai saldo tabungan per pemain tanpa reservasi kartu; kepemilikan tujuan dibuat saat pembelian penuh dan stok diperiksa dalam transaksi sesi. |
| `database/migrations/V012__session_lifecycle.sql` | Mencatat aktivitas terakhir dan alasan penutupan, serta mengenali sesi tanpa gameplay. |
| `database/migrations/V013__event_undo.sql` | Menyimpan snapshot sebelum event, memulihkan 16 proyeksi secara atomik, dan menjaga audit pembatalan serta reservasi identitas event. |
| `src/Cashflowpoly.Api/Contracts/EventUndoDtos.cs` | Kontrak request, receipt idempoten, dan pagination audit undo. |
| `src/Cashflowpoly.Api/Controllers/EventUndoController.cs` | Endpoint undo event terakhir dan audit khusus instruktur pemilik sesi. |
| `src/Cashflowpoly.Api/Data/EventUndoRepository.cs` | Menjalankan transaksi undo, validasi versi/urutan, pemulihan snapshot, dan invalidasi metrik. |
| `src/Cashflowpoly.Api/Infrastructure/SessionLifecycleOptions.cs` | Default heartbeat 30 menit dan timeout sesi 1 jam. |
| `src/Cashflowpoly.Api/Infrastructure/SessionLifecycleWorker.cs` | Memeriksa timeout, mengakhiri sesi berisi gameplay, dan membersihkan sesi kosong. |
| `tests/Cashflowpoly.Api.Tests/EventUndoIntegrationTests.cs` | Menguji pemulihan seluruh tabel, retry, audit, reservasi identitas, akses, dan rollback. |
| `tests/Cashflowpoly.Api.Tests/EventUndoConcurrencyIntegrationTests.cs` | Memastikan event tunggal/batch yang mengantre setelah undo divalidasi ulang pada state hasil pemulihan. |
| `tests/Cashflowpoly.Api.Tests/SessionLifecycleIntegrationTests.cs` | Menguji heartbeat, timeout, pembersihan sesi kosong, dan perlombaan penutupan. |
