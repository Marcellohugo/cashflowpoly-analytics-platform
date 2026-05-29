# Cashflowpoly Dashboard & Manajemen *Ruleset*

Repositori ini dibangun sebagai sistem informasi yang merekam aktivitas gim papan Cashflowpoly sebagai rangkaian *event*, memvalidasi data masuk, menyimpan data secara konsisten di PostgreSQL, lalu mengolahnya menjadi metrik literasi finansial dan capaian misi yang tampil pada dasbor web. Modul manajemen *ruleset* berbasis konfigurasi dinamis disediakan agar instruktur dapat mengubah parameter permainan tanpa mengubah kode.

## Tujuan
Tujuan utama:
- Menerima *event* permainan dari IDN atau simulator melalui REST API.
- Menjaga kualitas data melalui validasi skema, validasi aturan domain, idempotensi, dan keterurutan *event*.
- Mengelola *ruleset* sebagai konfigurasi variabel permainan (CRUD, *versioning*, aktivasi).
- Menghitung metrik pada level sesi dan pemain dari log *event*.
- Menyajikan analitika melalui UI web berbasis ASP.NET Core MVC (Razor Views).

## Ruang lingkup fitur
### Platform web
- UI berjalan di browser dan mengonsumsi data dari REST API.

### Antarmuka analitik
- Menampilkan performa pembelajaran pemain individu.
- Menampilkan performa pembelajaran agregat.
- Menampilkan performa misi pemain individu.
- Menampilkan performa misi agregat.
- Mengelompokkan data berdasarkan *ruleset* yang aktif pada sesi.

### Antarmuka manajemen aturan
- Membuat *ruleset* baru untuk mengubah variabel permainan (contoh: batas giliran, nilai uang, batas transaksi).
- Menampilkan daftar *ruleset* (termasuk *ruleset* default Cashflowpoly).
- Menghapus *ruleset* dengan pembatasan saat sistem sudah memakai *ruleset* pada sesi.
- Mengatur *ruleset* aktif per sesi (aktivasi berbasis versi).

### API back-end
- Menulis data permainan ke basis data dari *event*.
- Membaca data dari basis data untuk kebutuhan analitika dan manajemen *ruleset*.

### Basis data
- Menyimpan pemain, sesi, *ruleset* dan versi, *event*, proyeksi arus kas, *metric snapshot*, serta log validasi.

## Arsitektur tingkat tinggi
Arsitektur dibagi menjadi tiga komponen:
- **Cashflowpoly.Api**: REST API (ASP.NET Core 10) + Swagger UI.
- **Cashflowpoly.Ui**: MVC (Controller + Razor Views) yang memanggil REST API via `HttpClient`.
- **PostgreSQL**: penyimpanan data dan sumber kebenaran untuk analitika.

UI tidak mengakses database secara langsung. UI membaca data dari REST API agar konsisten dengan kontrak API dan skema data.

Catatan kontrak API:
- Prefix endpoint aktif: `/api/v1/...`.
- Opsi transisi (default nonaktif): set `FeatureFlags__EnableLegacyApiCompatibility=true` bila sementara perlu rewrite `/api/*` ke `/api/v1/*`.

## Teknologi dan alat
| No | Perangkat lunak | Fungsi penggunaan |
|---:|---|---|
| 1 | Windows 11 Home | Sistem operasi untuk pengembangan dan pengujian |
| 2 | Visual Studio Code | IDE untuk menulis kode dan menjalankan *debug* |
| 3 | .NET 10 SDK | Toolchain untuk membangun REST API dan MVC |
| 4 | PostgreSQL | DBMS untuk menyimpan sesi, *event*, *ruleset*, proyeksi, dan metrik |
| 5 | Docker Desktop | Menjalankan seluruh komponen melalui *container* |
| 6 | DBeaver | Mengelola PostgreSQL (koneksi, skema, query, inspeksi data) |
| 7 | Google Chrome | Menguji UI MVC dan mengakses Swagger UI |
| 8 | Swagger UI (Swashbuckle) | Dokumentasi dan uji *endpoint* API dari browser |
| 9 | Postman | Uji fungsional *endpoint* API (black-box) |
| 10 | Tailwind CSS | Styling UI dasbor MVC |

## Struktur repositori

```
.
+- .env
+- Cashflowpoly.sln
+- README.md
+- docker-compose.yml
+- database/
|  +- 00_create_schema.sql
|  +- 01_seed_default_rulesets_components.sql
|  +- 02_seed_full_inspection.sql
+- docs/
|  +- Img/
|  +- 00-Panduan/
|  |  +- 00-01-panduan-setup-lingkungan.md
|  |  +- 00-02-manual-pengguna-dan-skenario-operasional.md
|  |  +- 00-03-panduan-menjalankan-sistem.md
|  +- 01-Spesifikasi/
|  |  +- 01-01-spesifikasi-kebutuhan-sistem.md
|  |  +- 01-02-spesifikasi-event-dan-kontrak-api.md
|  |  +- 01-03-spesifikasi-ruleset-dan-validasi.md
|  +- 02-Perancangan/
|  |  +- 02-01-rencana-implementasi-dan-struktur-solution-dotnet.md
|  |  +- 02-02-rancangan-model-data-dan-basis-data.md
|  |  +- 02-03-definisi-metrik-dan-agregasi.md
|  |  +- 02-04-metrik-gameplay-fisik-dan-turunan.md
|  |  +- 02-05-rancangan-dashboard-analitika-mvc.md
|  |  +- 02-06-spesifikasi-ui-mvc-dan-rancangan-viewmodel.md
|  +- 03-Pengujian/
|  |  +- 03-01-rencana-pengujian-fungsional-dan-validasi.md
|  |  +- 03-02-laporan-hasil-pengujian.md
+- src/
   +- Cashflowpoly.Api/
   +- Cashflowpoly.Ui/
```

## Menjalankan dengan Docker (Docker Compose)
### PC Kamu (Development)
1. Pastikan konteks Docker lokal:
```bash
docker context use default
```
2. Siapkan env dev (sekali):
```powershell
Copy-Item .env.example .env.dev
```
3. Jalankan dev watch:
```bash
docker compose --env-file .env.dev -f docker-compose.yml -f docker-compose.watch.yml up --build
```
4. Pada startup pertama, API akan menjalankan migration EF lalu memastikan seed `01_seed_default_rulesets_components.sql` dan `02_seed_full_inspection.sql` terpasang.
5. Ngoding seperti biasa, auto-reload jalan.
6. Jika perlu verifikasi lokal sebelum merge/deploy:
```powershell
dotnet restore src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
dotnet restore src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj
dotnet restore tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj
dotnet restore tests/Cashflowpoly.Ui.Tests/Cashflowpoly.Ui.Tests.csproj
dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj -c Release --no-restore /warnaserror
dotnet build src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj -c Release --no-restore /warnaserror
dotnet build tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release --no-restore /warnaserror
dotnet build tests/Cashflowpoly.Ui.Tests/Cashflowpoly.Ui.Tests.csproj -c Release --no-restore /warnaserror
docker compose --env-file .env.dev.example -f docker-compose.yml -f docker-compose.watch.yml config
docker compose --env-file .env.prod.example -f docker-compose.yml -f docker-compose.prod.yml config
dotnet test tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release --filter "Category!=Integration"
dotnet test tests/Cashflowpoly.Ui.Tests/Cashflowpoly.Ui.Tests.csproj -c Release
```
7. Selesai kerja, stop dev:
```bash
docker compose --env-file .env.dev -f docker-compose.yml -f docker-compose.watch.yml down
```

Akses (sesuai env dev):
- API + Swagger (hanya saat `ASPNETCORE_ENVIRONMENT=Development`): `http://localhost:5041/swagger`
- UI MVC: `http://localhost:5203`

### PC Server (Production)
1. Install Docker, Docker Compose, Git.
2. Clone repo atau checkout revision yang ingin dideploy.
3. Siapkan env prod (isi secret benar): `.env.prod`.
4. Verifikasi lokal sebelum deploy (disarankan):
```powershell
dotnet restore src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
dotnet restore src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj
dotnet restore tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj
dotnet restore tests/Cashflowpoly.Ui.Tests/Cashflowpoly.Ui.Tests.csproj
dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj -c Release --no-restore /warnaserror
dotnet build src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj -c Release --no-restore /warnaserror
dotnet build tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release --no-restore /warnaserror
dotnet build tests/Cashflowpoly.Ui.Tests/Cashflowpoly.Ui.Tests.csproj -c Release --no-restore /warnaserror
docker compose --env-file .env.prod.example -f docker-compose.yml -f docker-compose.prod.yml config
dotnet test tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release
dotnet test tests/Cashflowpoly.Ui.Tests/Cashflowpoly.Ui.Tests.csproj -c Release
```
5. Deploy awal atau redeploy:
```powershell
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel up -d --build db api ui nginx cloudflared
```
6. Verifikasi:
```bash
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml ps
curl http://localhost/health/ready
docker logs cashflowpoly-tunnel --tail 20
```
Jika `CLOUDFLARE_TUNNEL_TOKEN` kosong, skrip deploy otomatis menjalankan production tanpa `cloudflared`.

Catatan keamanan lokal:
- Set `JWT_SIGNING_KEY` di `.env.dev` (dev) dan `.env.prod` (production), minimal 32 karakter.
- Untuk rotasi key JWT, bisa pakai:
  - `JWT_SIGNING_KEYS_JSON` (array JSON key + `kid` + window aktivasi), atau
  - `Jwt:SigningKeysFile`/`Jwt:SigningKeyFile` (secret file, cocok untuk mount dari secret manager).
- Registrasi publik untuk semua role (`INSTRUCTOR` dan `PLAYER`) tersedia melalui endpoint `POST /api/v1/auth/register`.
- Untuk bootstrap user awal via environment, aktifkan `AUTH_BOOTSTRAP_SEED_DEFAULT_USERS=true` dan isi username/password bootstrap.
- Dataset inspeksi penuh ikut dipasang otomatis saat startup API melalui seed gabungan `database/02_seed_full_inspection.sql`.

Rute UI utama:
- Analitika sesi: `/Analytics`
- Manajemen ruleset: `/Rulesets`

### Akun inspeksi
Akun aktif:
- `mira.hartanto` / `MiraAudit!2026` (`INSTRUCTOR`)
- `bayu.prakasa` / `BayuAudit!2026` (`INSTRUCTOR`)
- `sindy.lestari` / `SindyAudit!2026` (`INSTRUCTOR`)
- `nadia.putri` / `NadiaAudit!2026` (`PLAYER`)
- `rangga.maulana` / `RanggaAudit!2026` (`PLAYER`)
- `safira.anindya` / `SafiraAudit!2026` (`PLAYER`)
- `teo.prasetyo` / `TeoAudit!2026` (`PLAYER`)
- `ulfa.ramadhani` / `UlfaAudit!2026` (`PLAYER`)
- `vina.anggraini` / `VinaAudit!2026` (`PLAYER`)
- `wahyu.firmansyah` / `WahyuAudit!2026` (`PLAYER`)
- `xenia.kusuma` / `XeniaAudit!2026` (`PLAYER`)
- `yudha.permana` / `YudhaAudit!2026` (`PLAYER`)
- `zara.nuraini` / `ZaraAudit!2026` (`PLAYER`)
- `adit.suryana` / `AditAudit!2026` (`PLAYER`)
- `chandra.gunawan` / `ChandraAudit!2026` (`PLAYER`)
- `dinda.ayu` / `DindaAudit!2026` (`PLAYER`)
- `elang.nugroho` / `ElangAudit!2026` (`PLAYER`)
- `fiona.melati` / `FionaAudit!2026` (`PLAYER`)

Akun nonaktif untuk inspeksi kasus gagal login:
- `arman.wijaya` / `ArmanAudit!2026` (`INSTRUCTOR`, `is_active=false`)
- `bella.kartika` / `BellaAudit!2026` (`PLAYER`, `is_active=false`)

### Matriks inspeksi dataset
- `mira.hartanto` memiliki sesi `810...001`, `810...002`, dan `810...007` untuk memeriksa mode `PEMULA` dan `MAHIR`, ruleset dengan versi `RETIRED/ACTIVE`, sesi `CREATED` dan `ENDED`, serta kombinasi kebutuhan, donasi, emas, pinjaman, asuransi, tabungan, dan risiko.
- `bayu.prakasa` memiliki sesi `810...003`, `810...004`, dan `810...008` untuk memeriksa ruleset Sabtu nonaktif, sesi `STARTED`, sesi `ENDED`, username ordering, validation log, dan pemain aktif/nonaktif.
- `sindy.lestari` memiliki sesi `810...005` dan `810...006` untuk memeriksa sesi live mode `MAHIR`, ruleset draft, penolakan aktivasi, dan skenario tabungan/risiko.
- `arman.wijaya` dan `bella.kartika` sengaja nonaktif agar kasus gagal login dan filtering akun aktif dapat diperiksa.
- Dataset inspeksi sekarang mencakup `20` akun, `8` sesi, lebih dari `190` event, `120` proyeksi arus kas, `220` metric snapshot, serta audit log dan validation log untuk memeriksa analytics, compliance, observability, keamanan, dan variabel metrik fisik/turunan dari dokumen metrik.
- Action type yang tercakup meliputi `session.created`, `session.started`, `session.ended`, `turn.action.used`, `mission.assigned`, seluruh pembelian kebutuhan `primary/secondary/tertiary`, `ingredient.purchased`, `ingredient.discarded`, `order.claimed`, `order.passed`, `work.freelance.completed`, `day.friday.donation`, `day.saturday.gold_trade`, `gold.points.awarded`, `saving.deposit.created`, `saving.deposit.withdrawn`, `saving.goal.achieved`, `loan.syariah.taken`, `loan.syariah.repaid`, `insurance.multirisk.purchased`, `insurance.multirisk.used`, `risk.life.drawn`, `risk.emergency.used`, `pension.rank.awarded`, `donation.rank.awarded`, dan `tie_breaker.assigned`.

### 2) Sambungkan DBeaver ke PostgreSQL
Gunakan konfigurasi berikut:
- Host: `localhost`
- Port: `5432`
- Database: `cashflowpoly`
- User: `cashflowpoly`
- Password: `cashflowpoly`

DBeaver menampilkan tabel pada schema `public` setelah service API selesai startup. Jalur startup yang benar sekarang adalah:
- migration EF membuat atau menyelaraskan schema database
- API memastikan seed `database/01_seed_default_rulesets_components.sql`
- API memastikan seed `database/02_seed_full_inspection.sql`

### 3) Hentikan layanan
Untuk development:
```bash
docker compose --env-file .env.dev -f docker-compose.yml -f docker-compose.watch.yml down
```

Untuk production:
```bash
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel down
```

Jika perlu menghapus data database/volume:
```bash
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel down -v
```

## Menjalankan lokal tanpa Docker
### 1) Siapkan PostgreSQL
Buat database dan user. Jalur yang direkomendasikan adalah langsung menjalankan API karena migration dan seed akan dipasang otomatis saat startup.

Jika ingin memuat SQL secara manual tanpa menunggu startup API, jalankan:
1. `database/00_create_schema.sql`
2. `database/01_seed_default_rulesets_components.sql`
3. `database/02_seed_full_inspection.sql`

### 2) Atur konfigurasi API dan UI
API memakai koneksi database dari `ConnectionStrings:Default`.
UI memakai base URL API dari `ApiBaseUrl`.
JWT API dibaca dari `Jwt:SigningKey` dengan fallback ke environment variable `JWT_SIGNING_KEY`.
Untuk production hardening, API juga mendukung multi-key rotation via `Jwt:SigningKeys` / `JWT_SIGNING_KEYS_JSON` serta secret file (`Jwt:SigningKeysFile` / `Jwt:SigningKeyFile`).

Contoh lokal (sesuai `.env` dan launch settings):
- API: `http://localhost:5041` atau `https://localhost:7041`
- UI: `http://localhost:5203` atau `https://localhost:7203`

### 3) Jalankan API
```bash
dotnet run --project src/Cashflowpoly.Api
```

### 4) Jalankan UI
Buka terminal baru:
```bash
dotnet run --project src/Cashflowpoly.Ui
```

## Endpoint tambahan
Endpoint tambahan yang tersedia:
- `DELETE /api/v1/rulesets/{rulesetId}` hapus ruleset (jika belum dipakai sesi)
- `GET /api/v1/sessions` daftar sesi
- `POST /api/v1/players` buat pemain
- `GET /api/v1/players` daftar pemain
- `POST /api/v1/sessions/{sessionId}/players` tambah pemain ke sesi
- `GET /api/v1/rulesets/{rulesetId}` detail ruleset + versi
- `GET /api/v1/rulesets/components/defaults` daftar ruleset default komponen (mode pemula + mahir)
- `GET /api/v1/rulesets/{rulesetId}/components` detail komponen ruleset dari `component_catalog` (opsional `?version=`).
- `POST /api/v1/analytics/sessions/{sessionId}/recompute` hitung ulang metrik
- `GET /api/v1/observability/metrics/summary` ringkasan singkat observability yang menunjuk ke endpoint Prometheus API
- `GET /metrics` metrik operasional API dalam format Prometheus (akses langsung ke service API)
- `GET /api/v1/security/audit-logs` ringkasan audit log keamanan
- `GET /health/live` liveness API/UI
- `GET /health/ready` readiness API/UI

## Catatan pengembangan
- Gunakan Swagger untuk uji cepat *endpoint* dan gunakan Postman untuk skenario uji *black-box* yang terdokumentasi.

## Dokumen desain dan spesifikasi
Seluruh dokumen TA disimpan pada folder `docs/` agar repositori memuat artefak desain dan artefak implementasi pada satu tempat.

Dokumen kunci:
- Spesifikasi kebutuhan sistem (SRS): `docs/01-Spesifikasi/01-01-spesifikasi-kebutuhan-sistem.md`
- Kontrak API: `docs/01-Spesifikasi/01-02-spesifikasi-event-dan-kontrak-api.md`
- Spesifikasi *ruleset*: `docs/01-Spesifikasi/01-03-spesifikasi-ruleset-dan-validasi.md`
- Rencana implementasi dan struktur solusi: `docs/02-Perancangan/02-01-rencana-implementasi-dan-struktur-solution-dotnet.md`
- Model data dan basis data: `docs/02-Perancangan/02-02-rancangan-model-data-dan-basis-data.md`
- Definisi metrik dan agregasi: `docs/02-Perancangan/02-03-definisi-metrik-dan-agregasi.md`
- Variabel gameplay fisik dan metrik turunan: `docs/02-Perancangan/02-04-metrik-gameplay-fisik-dan-turunan.md`
- Rancangan dasbor MVC: `docs/02-Perancangan/02-05-rancangan-dashboard-analitika-mvc.md`
- Spesifikasi UI dan ViewModel: `docs/02-Perancangan/02-06-spesifikasi-ui-mvc-dan-rancangan-viewmodel.md`

## Pengujian
- Build verifikasi:
  - `dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj -c Release`
  - `dotnet build src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj -c Release`
  - `dotnet build tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release`
- Jalankan seluruh test API: `dotnet test tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release`.
- Integration test API (auth + RBAC + ruleset + analytics flow) dijalankan via xUnit + Testcontainers, jadi Docker daemon wajib aktif saat `dotnet test`.
- Jalankan hanya integration test: `dotnet test tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release --filter "Category=Integration"`.
- Jalankan test non-integration (lebih cepat): `dotnet test tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release --filter "Category!=Integration"`.
- Jalankan load test baseline dengan tool HTTP pilihan tim menggunakan skenario request berulang ke endpoint ingest event dan analytics sesi.
- Koleksi Postman: `postman/Cashflowpoly.postman_collection.json`.
- Uji *endpoint* melalui Swagger UI untuk verifikasi cepat.
- Jalankan skenario pengujian fungsional melalui Postman sesuai dokumen rencana pengujian.
- Validasi dasbor dengan membandingkan metrik UI vs data pada tabel `metric_snapshots` dan proyeksi transaksi.
- Verifikasi end-to-end API, RBAC, dan Web UI dilakukan mengikuti checklist pada `docs/03-Pengujian/03-01-rencana-pengujian-fungsional-dan-validasi.md`.
- Artefak bukti formal disimpan pada media dokumentasi pengujian yang dipakai tim atau penguji.
- Verifikasi lokal dilakukan dengan rangkaian perintah `dotnet restore`, `dotnet build`, `dotnet test`, dan `docker compose ... config`.

## Operasional DB (Backup/Restore)
- Backup database dilakukan dengan mekanisme native PostgreSQL sesuai environment deployment.
- Uji restore tetap wajib dilakukan berkala untuk memastikan backup dapat dipulihkan.

## Lisensi
Lisensi akan ditentukan untuk repositori ini.





