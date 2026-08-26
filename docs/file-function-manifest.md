# Manifest Fungsi File

Baseline manifest: 26 Agustus 2026 (setelah penyelarasan rulebook dan gerbang rilis).

Dokumen ini merangkum fungsi file dan family file aktif pada repository, khususnya setelah dilakukan penyederhanaan struktur berkas dokumentasi di bawah folder `docs/`.

## Root, Konfigurasi, dan Infrastruktur
| Path | Kategori | Fungsi |
|---|---|---|
| `README.md` | Root | Entry point dokumentasi repository, setup, endpoint utama, dan tautan dokumen desain. |
| `README-API.md` | Root | Dokumentasi kontrak API v1, autentikasi, setup, event, pagination, error, dan Postman. |
| `README-DATABASE.md` | Root | Dokumentasi schema, relasi, migrasi berurutan, seed, operasi, dan batas rollback database. |
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
| Path | Kategori | Fungsi |
|---|---|---|
| `database/00_create_schema.sql` | Database | DDL kanonis schema PostgreSQL event-first. |
| `database/01_seed_default_rulesets_components.sql` | Database | Seed ruleset default dan katalog komponen gameplay. |
| `database/02_seed_simulation_sessions_events.sql` | Database | Seed simulasi manual untuk sesi dan event contoh. |
| `postman/Cashflowpoly.postman_collection.json` | Integrasi | Collection Postman untuk smoke/API/RBAC flow. |
| `postman/Cashflowpoly.local.postman_environment.json` | Integrasi | Environment lokal Postman. |

## Pengujian Otomatis

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
| `docs/file-function-manifest.md` | Dokumen | Manifest fungsi file repository (dokumen ini). |
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
| `docs/02-Perancangan/02-01-rancangan-database-dan-model-data.md` | Perancangan | Kamus data PostgreSQL, skema relasional, dan catatan keputusan normalisasi. |
| `docs/02-Perancangan/02-02-rancangan-kontrak-api-dan-event.md` | Perancangan | Kontrak event, endpoint API, request/response, dan status code. |
| `docs/02-Perancangan/02-03-rancangan-definisi-dan-agregasi-metrik.md` | Perancangan | Formula kalkulasi metrik analitik dashboard dan metrik turunan gameplay fisik. |
| `docs/02-Perancangan/02-04-rancangan-antarmuka-dan-viewmodel-mvc.md` | Perancangan | Desain mock-up visual UI MVC, ViewModel, dan pemetaan call API. |
| `docs/03-Pengujian/03-01-pengujian-rencana-dan-kasus-uji.md` | Pengujian | Rencana pengujian fungsional unit testing, integration, dan smoke test. |
| `docs/03-Pengujian/03-02-pengujian-laporan-hasil-baseline.md` | Pengujian | Laporan hasil eksekusi pengujian baseline sistem. |
| `docs/03-Pengujian/03-03-pengujian-status-kesesuaian-implementasi.md`| Pengujian | Checklist status kesesuaian implementasi kode terhadap spesifikasi. |
| `docs/03-Pengujian/03-04-pengujian-tahapan-dan-roadmap-implementasi.md`| Pengujian | Tahapan pentahapan pengerjaan (roadmap) dan checklist Definition of Done. |
| `docs/Img/RuleBook/*.png` | Aset dokumen | Scan/gambar halaman rulebook untuk lampiran dokumen. |

*(Bagian file src/ dan tests/ tidak berubah, silakan lihat rincian lengkapnya pada repositori kode).*
