# Cashflowpoly Analytics Platform

Repositori ini dibangun sebagai sistem informasi yang merekam aktivitas gim papan Cashflowpoly sebagai rangkaian *event*, memvalidasi data masuk, menyimpan data secara konsisten di PostgreSQL, lalu mengolahnya menjadi metrik literasi finansial dan capaian misi yang tampil pada Web Analitik. Setup sesi, penambahan Player, start/end sesi, dan input keputusan Player dilakukan oleh Instruktur melalui Klien Game/IDN yang mengirim data ke API. Pengelolaan *ruleset* dan penguncian `ruleset_version_id` pada sesi tersedia untuk Instruktur melalui Web Analitik MVC dan API.

Baseline dokumentasi ini mengikuti implementasi aktual per 12 Juli 2026 dengan schema `3.0.7`.

## Tujuan
Tujuan utama:
- Menerima *event* permainan dari IDN atau simulator melalui REST API.
- Menjaga kualitas data melalui validasi skema, validasi aturan domain, idempotensi, dan keterurutan *event*.
- Menyediakan kontrak API untuk lifecycle sesi, Player, *ruleset*, dan event permainan yang dipakai Klien Game/IDN.
- Menghitung metrik pada level sesi dan pemain dari log *event*.
- Menyajikan analitika melalui UI web berbasis ASP.NET Core MVC (Razor Views), termasuk manajemen *ruleset* untuk Instruktur.

## Ruang lingkup fitur
### Platform web
- UI berjalan di browser, mengonsumsi data dari REST API, dan difokuskan sebagai Web Analitik.
- UI tidak membuat sesi, menambahkan Player, memulai/mengakhiri sesi, atau mengirim event permainan; operasi gameplay tersebut menjadi tanggung jawab Klien Game/IDN atau integrasi API. UI tetap menyediakan create/edit/delete/activate *ruleset* dan pemilihan `ruleset_version_id` saat setup sesi untuk Instruktur.

### Antarmuka analitik
- Menampilkan performa pembelajaran pemain individu.
- Menampilkan performa pembelajaran agregat.
- Menampilkan performa misi pemain individu.
- Menampilkan performa misi agregat.
- Mengelompokkan data berdasarkan *ruleset* yang aktif pada sesi.

### Referensi aturan di Web Analitik
- Menampilkan daftar *ruleset* yang dapat diakses pengguna.
- Menampilkan detail versi, komponen, mode, dan ringkasan konfigurasi *ruleset*.
- Membantu Instruktur dan Player membaca konteks validasi/metrik tanpa mengubah data permainan dari Web.

### API back-end
- Menyediakan endpoint login/register untuk Instruktur dan Player.
- Menyediakan endpoint lifecycle sesi, penguncian `ruleset_version_id` saat sesi dibuat, dan pengelolaan Player untuk Klien Game/IDN.
- Menulis data permainan ke basis data dari *event*.
- Membaca data dari basis data untuk kebutuhan analitika dan referensi *ruleset*.

### Basis data
- Menyimpan akun aplikasi (`app_users`), peserta sesi (`session_participants`),
  sesi, *ruleset* dan versi, *event*, registry asset, projection session,
  *metric snapshot*, skor akhir, narrative log, audit keamanan, serta log
  validasi.
- `events` adalah satu-satunya sumber kebenaran gameplay. Tabel state,
  inventory, asset, narrative, scoring, dan metrik adalah projection
  yang memiliki provenance event dan dapat dibangun ulang.

## Arsitektur tingkat tinggi
Arsitektur dibagi menjadi empat komponen:
- **Klien Game/IDN**: aplikasi operasional permainan untuk Instruktur dan Player. Klien ini membuat sesi, memilih *ruleset*, menambahkan Player, memulai/mengakhiri sesi, dan mengirim event permainan ke API.
- **Cashflowpoly.Api**: REST API (ASP.NET Core 10) + Swagger UI.
- **Cashflowpoly.Ui**: Web Analitik MVC (Controller + Razor Views) yang membaca data REST API via `HttpClient`.
- **PostgreSQL**: penyimpanan data dan sumber kebenaran untuk analitika.

UI tidak mengakses database secara langsung. UI membaca data dari REST API agar konsisten dengan kontrak API dan skema data.

Catatan kontrak API:
- Prefix endpoint aktif: `/api/v1/...`.

## Alur utama sistem
1. Instruktur dan Player login/sign in ke Web Analitik atau Klien Game/IDN.
2. Instruktur membuat/mengedit ruleset, mengaktifkan versi ruleset, lalu
   membuat sesi dengan `ruleset_version_id` yang dipilih. Setelah itu
   instruktur menambahkan akun Player (`user_id`) ke sesi sebagai
   `session_participant_id`/`session_player_id` dan memulai sesi melalui
   Klien Game/IDN.
3. Selama permainan berjalan, Instruktur memasukkan input keputusan Player melalui Klien Game/IDN.
4. Klien Game/IDN mengirim event permainan ke API.
5. API memvalidasi token, data sesi, data Player, ruleset aktif, urutan event, dan duplikasi event.
6. Event valid disimpan ke PostgreSQL.
7. Web Analitik membaca data permainan dari API.
8. Instruktur memantau jumlah event, cash in, cash out, net cashflow, performa Player, dan pelanggaran validasi.
9. Setelah permainan berakhir, Player melihat data permainan sesuai hak aksesnya melalui Web Analitik.

## Teknologi dan alat
| No | Perangkat lunak | Fungsi penggunaan |
|---:|---|---|
| 1 | Windows 11 Home | Sistem operasi untuk pengembangan dan pengujian |
| 2 | Visual Studio Code | IDE untuk menulis kode dan menjalankan *debug* |
| 3 | .NET 10 SDK | Toolchain untuk membangun REST API dan MVC |
| 4 | PostgreSQL 15+ (direkomendasikan PostgreSQL 16) | DBMS untuk menyimpan sesi, *event*, *ruleset*, proyeksi, dan metrik |
| 5 | Docker Desktop | Menjalankan seluruh komponen melalui *container* |
| 6 | DBeaver | Mengelola PostgreSQL (koneksi, skema, query, inspeksi data) |
| 7 | Google Chrome | Menguji UI MVC dan mengakses Swagger UI |
| 8 | Swagger UI (Swashbuckle) | Dokumentasi dan uji *endpoint* API dari browser |
| 9 | Postman | Uji fungsional *endpoint* API (black-box) |
| 10 | Tailwind CSS | Styling UI dasbor MVC |

## Struktur repositori

```
.
+- .dockerignore
+- .gitattributes
+- .gitignore
+- Cashflowpoly.sln
+- README.md
+- config/
|  +- env/
|     +- .env.example
|     +- .env.dev.example
|     +- .env.prod.example
+- database/
|  +- 00_create_schema.sql
|  +- 01_seed_default_rulesets_components.sql
|  +- 02_seed_simulation_sessions_events.sql
+- infra/
|  +- cloudflared/
|  +- docker/
|  |  +- docker-compose.yml
|  |  +- docker-compose.watch.yml
|  |  +- docker-compose.prod.yml
|  +- nginx/
+- postman/
|  +- Cashflowpoly.postman_collection.json
|  +- Cashflowpoly.local.postman_environment.json
+- docs/
|  +- Img/
|  +- file-function-manifest.md
|  +- 00-ringkasan-rulebook-cashflowpoly.md
|  +- 01-ringkasan-proposal-tugas-akhir.md
|  +- 00-Panduan/
|  |  +- 00-01-panduan-setup-lingkungan.md
|  |  +- 00-02-panduan-manual-pengguna-dashboard.md
|  |  +- 00-03-panduan-menjalankan-sistem.md
|  |  +- 00-04-panduan-deployment-produksi.md
|  |  +- 00-05-panduan-alur-dan-hak-akses.md
|  +- 01-Spesifikasi/
|  |  +- 01-01-spesifikasi-kebutuhan-sistem.md
|  |  +- 01-02-spesifikasi-ruleset-dan-validasi.md
|  |  +- 01-03-spesifikasi-integrasi-dan-keamanan.md
|  |  +- 01-04-spesifikasi-diagram-uml.md
|  |  +- 01-05-spesifikasi-skenario-simulasi.md
|  +- 02-Perancangan/
|  |  +- 02-01-rancangan-database-dan-model-data.md
|  |  +- 02-02-rancangan-kontrak-api-dan-event.md
|  |  +- 02-03-rancangan-definisi-dan-agregasi-metrik.md
|  |  +- 02-04-rancangan-antarmuka-dan-viewmodel-mvc.md
|  +- 03-Pengujian/
|  |  +- 03-01-pengujian-rencana-dan-kasus-uji.md
|  |  +- 03-02-pengujian-laporan-hasil-baseline.md
|  |  +- 03-03-pengujian-status-kesesuaian-implementasi.md
|  |  +- 03-04-pengujian-tahapan-dan-roadmap-implementasi.md
+- src/
+  +- Cashflowpoly.Api/
+  +- Cashflowpoly.Ui/
+- tests/
+  +- Cashflowpoly.Api.Tests/
+  +- Cashflowpoly.Ui.Tests/
```

Catatan: `.dockerignore`, `.gitattributes`, `.gitignore`, `Cashflowpoly.sln`, dan `README.md` tetap di root karena menjadi entry point standar untuk Docker, Git, .NET solution, dan dokumentasi utama.

## Menjalankan dengan Docker (Docker Compose)
### PC Kamu (Development)
1. Pastikan konteks Docker lokal:
```bash
docker context use default
```
2. Siapkan env dev (sekali):
```powershell
Copy-Item config/env/.env.dev.example config/env/.env.dev
```
3. Jalankan dev watch:
```bash
docker compose --env-file config/env/.env.dev -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.watch.yml up --build
```
4. Pada startup pertama, API akan menjalankan bootstrap schema SQL kanonik dari `database/00_create_schema.sql`, lalu memastikan seed `01_seed_default_rulesets_components.sql` terpasang.
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
docker compose --env-file config/env/.env.dev.example -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.watch.yml config
docker compose --env-file config/env/.env.prod.example -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.prod.yml config
dotnet test tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release --filter "Category!=Integration"
dotnet test tests/Cashflowpoly.Ui.Tests/Cashflowpoly.Ui.Tests.csproj -c Release
```
7. Selesai kerja, stop dev:
```bash
docker compose --env-file config/env/.env.dev -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.watch.yml down
```

Akses (sesuai env dev):
- API + Swagger (hanya saat `ASPNETCORE_ENVIRONMENT=Development`): `http://localhost:5041/swagger`
- UI MVC: `http://localhost:5203`

### PC Server (Production)
1. Install Docker, Docker Compose, Git.
2. Clone repo atau checkout revision yang ingin dideploy.
3. Siapkan env prod (isi secret benar): `config/env/.env.prod`.
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
docker compose --env-file config/env/.env.prod.example -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.prod.yml config
dotnet test tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release
dotnet test tests/Cashflowpoly.Ui.Tests/Cashflowpoly.Ui.Tests.csproj -c Release
```
5. Deploy awal atau redeploy:
```powershell
docker compose --env-file config/env/.env.prod -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.prod.yml --profile tunnel up -d --build db api ui nginx cloudflared
```
6. Verifikasi:
```bash
docker compose --env-file config/env/.env.prod -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.prod.yml ps
curl http://localhost/health/ready
docker logs cashflowpoly-tunnel --tail 20
```
Jika `CLOUDFLARE_TUNNEL_TOKEN` kosong, skrip deploy otomatis menjalankan production tanpa `cloudflared`.

Catatan keamanan lokal:
- Set `JWT_SIGNING_KEY` di `config/env/.env.dev` (dev) dan `config/env/.env.prod` (production), minimal 32 karakter.
- Untuk rotasi key JWT, bisa pakai:
  - `JWT_SIGNING_KEYS_JSON` (array JSON key + `kid` + window aktivasi), atau
  - `Jwt:SigningKeysFile`/`Jwt:SigningKeyFile` (secret file, cocok untuk mount dari secret manager).
- Registrasi publik untuk semua role (`INSTRUCTOR` dan `PLAYER`) tersedia melalui endpoint `POST /api/v1/auth/register`.
- Setiap kartu pinjaman memakai `loan_instance_id`; beberapa instance produk yang sama dapat aktif selama stok fisik `card_qty` sesi tersedia, dan pelunasan menutup satu instance penuh.
- Aksi gratis dan event sistem memakai `action_slot=0`; aksi reguler pemain memakai `1..actions_per_turn` sesuai `GameActionCatalog`.
- Risiko `OUT` Mode Mahir disimpan pending dan diselesaikan melalui `BayarRisiko`, asuransi aktif, atau `GunakanOpsiDarurat`. Nominal opsi darurat dihitung server.
- Penggunaan asuransi hanya memakai event `Asuransi` dengan `risk_event_id`; satu klaim mengurangi tepat satu `remaining_uses` polis aktif dan membuat pasangan `INSURANCE_OFFSET IN` serta `RISK_LIFE OUT` secara atomik.
- `GunakanOpsiDarurat` hanya menerima `SELL_NEED`, `SELL_GOLD`, atau `TAKE_SHARIA_LOAN`; `direction` dan `amount` tidak dipercaya dari klien.
- Saat sesi dimulai, server mengacak pembagian Tie Breaker `#1..#N`; pemilik `#1` menjadi pemain pertama dan `player_order_no` seluruh peserta mengikuti nomor kartu. Bahan awal dan misi unik juga diacak. Pasar awal selalu berisi lima bahan, lima pesanan, dan lima kebutuhan Primer; sesi ditolak jika katalog tidak mencukupi.
- Payload `SetupMisiAwal` hanya terlihat penuh oleh pemain pemilik dan Instruktur selama sesi aktif. Pemain lain menerima status `HIDDEN`; seluruh misi baru terbuka setelah sesi `ENDED`.
- Pembelian bahan/kebutuhan dan klaim pesanan harus merujuk kartu yang sedang berada di pasar. Setelah aksi reguler terakhir pemain, server mengisi seluruh slot kosong secara atomik dan menyimpan hasilnya pada `payload.market_refills`; klien tidak boleh mengirim `AmbilKartuDariDeck` atau `IsiUlangPasar` saat runtime.
- Refill non-bahan memakai kartu `DECK` lalu `DISCARD`. Khusus bahan masakan, jumlah kartu deck tidak dihitung dan posisi logis baru dapat dibuat saat deck/discard kosong. Kartu pesanan yang berhasil diklaim tetap menjadi kartu milik pemain.
- Harga transaksi emas wajib berasal dari `BukaHargaEmas` pada hari yang sama. Syarat Primer bersifat historis, tie breaker dibatasi `1..jumlah pemain`, dan skor emas di atas empat kartu berhenti pada tier tertinggi ruleset.
- Donasi Jumat dibatasi satu kali per pemain per hari serta tetap `SEALED` sampai semua pemain mengirim; validasi jual emas memakai `session_participant_gold_holdings`.
- Untuk bootstrap user awal via environment, aktifkan `AUTH_BOOTSTRAP_SEED_DEFAULT_USERS=true` dan isi username/password bootstrap.
Rute UI utama:
- Home: `/`
- Daftar sesi: `/sessions`
- Detail sesi: `/sessions/{id}`
- Direktori Player: `/players`
- Detail Player dalam sesi: `/sessions/{id}/players/{userId}`
- Ruleset: `/rulesets`
- Rulebook: `/rulebook`
- Analytics redirect: `/Analytics` atau `/analytics`

### 2) Sambungkan DBeaver ke PostgreSQL
Gunakan konfigurasi berikut:
- Host: `localhost`
- Port: `5432`
- Database: `cashflowpoly`
- User: `cashflowpoly`
- Password: `cashflowpoly`

DBeaver menampilkan tabel pada schema `public` setelah service API selesai startup. Jalur startup yang benar sekarang adalah:
- bootstrap schema SQL kanonik dari `database/00_create_schema.sql` membuat atau menyelaraskan schema database
- API memastikan seed `database/01_seed_default_rulesets_components.sql`

### 3) Hentikan layanan
Untuk development:
```bash
docker compose --env-file config/env/.env.dev -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.watch.yml down
```

Untuk production:
```bash
docker compose --env-file config/env/.env.prod -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.prod.yml --profile tunnel down
```

Jika perlu menghapus data database/volume:
```bash
docker compose --env-file config/env/.env.prod -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.prod.yml --profile tunnel down -v
```

## Menjalankan lokal tanpa Docker
### 1) Siapkan PostgreSQL
Buat database dan user. Jalur yang direkomendasikan adalah langsung menjalankan API karena bootstrap schema SQL kanonik dan seed akan dipasang otomatis saat startup.

Jika ingin memuat SQL secara manual tanpa menunggu startup API, jalankan:
1. `database/00_create_schema.sql`
2. `database/01_seed_default_rulesets_components.sql`
3. Opsional untuk data simulasi manual dual-mode: `database/02_seed_simulation_sessions_events.sql`
   File ini tidak di-bootstrap otomatis saat startup API; kredensial demo lokal hanya dicantumkan pada header komentar file SQL tersebut.

### 2) Atur konfigurasi API dan UI
API memakai koneksi database dari `ConnectionStrings:Default`.
UI memakai base URL API dari `ApiBaseUrl`.
JWT API dibaca dari `Jwt:SigningKey` dengan fallback ke environment variable `JWT_SIGNING_KEY`.
Untuk production hardening, API juga mendukung multi-key rotation via `Jwt:SigningKeys` / `JWT_SIGNING_KEYS_JSON` serta secret file (`Jwt:SigningKeysFile` / `Jwt:SigningKeyFile`).

Contoh lokal (sesuai `config/env/.env.dev`, `appsettings.Development.json`, dan launch settings):
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
- `GET /api/v1/sessions` daftar sesi
- `POST /api/v1/sessions` buat sesi dengan `ruleset_version_id` melalui Klien Game/IDN atau integrasi API
- `POST /api/v1/sessions/{sessionId}/start` mulai sesi melalui Klien Game/IDN atau integrasi API
- `POST /api/v1/sessions/{sessionId}/end` akhiri sesi melalui Klien Game/IDN atau integrasi API
- `GET /api/v1/sessions/{sessionId}/state` baca projection state sesi untuk Instruktur
- `PUT /api/v1/sessions/{sessionId}/state` selalu mengembalikan `410 STATE_WRITE_DISABLED`; state hanya berubah lewat event ingestion
- `POST /api/v1/players` buat pemain
- `GET /api/v1/players` daftar pemain
- `POST /api/v1/sessions/{sessionId}/players` tambah pemain ke sesi
- `POST /api/v1/events` ingest satu event permainan
- `POST /api/v1/events/batch` ingest batch event permainan
- `GET /api/v1/sessions/{sessionId}/events` ambil event sesi berurutan (`fromSeq`, `limit`)
- `GET /api/v1/rulesets` daftar ruleset sesuai scope pengguna
- `DELETE /api/v1/rulesets/{rulesetId}` hapus ruleset (jika belum dipakai sesi)
- `GET /api/v1/rulesets/{rulesetId}` detail ruleset + versi
- `GET /api/v1/rulesets/sections` struktur section ruleset untuk form/preview
- `GET /api/v1/rulesets/components/defaults` daftar ruleset default komponen (mode pemula + mahir)
- `GET /api/v1/rulesets/{rulesetId}/components` detail komponen ruleset dari `definition` ter-normalisasi (opsional `?version=`).
- `POST /api/v1/analytics/sessions/{sessionId}/recompute` hitung ulang metrik
- `GET /api/v1/analytics/sessions/{sessionId}` ringkasan analitika sesi
- `GET /api/v1/analytics/sessions/{sessionId}/transactions?userId=...` histori transaksi, dapat dibatasi ke akun Player tertentu
- `GET /api/v1/analytics/sessions/{sessionId}/players/{userId}/gameplay` metrik gameplay per akun Player
- `GET /api/v1/analytics/rulesets/{rulesetId}/summary` ringkasan performa lintas sesi pada ruleset
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
- Matriks alur dan hak akses: `docs/00-Panduan/00-05-panduan-alur-dan-hak-akses.md`
- Kontrak API: `docs/02-Perancangan/02-02-rancangan-kontrak-api-dan-event.md`
- Spesifikasi *ruleset*: `docs/01-Spesifikasi/01-02-spesifikasi-ruleset-dan-validasi.md`
- Rencana implementasi dan struktur solusi: `docs/03-Pengujian/03-04-pengujian-tahapan-dan-roadmap-implementasi.md`
- Model data dan basis data: `docs/02-Perancangan/02-01-rancangan-database-dan-model-data.md`
- Definisi metrik dan agregasi: `docs/02-Perancangan/02-03-rancangan-definisi-dan-agregasi-metrik.md`
- Variabel gameplay fisik dan metrik turunan: `docs/02-Perancangan/02-03-rancangan-definisi-dan-agregasi-metrik.md`
- Rancangan dasbor MVC: `docs/02-Perancangan/02-04-rancangan-antarmuka-dan-viewmodel-mvc.md`
- Spesifikasi UI dan ViewModel: `docs/02-Perancangan/02-04-rancangan-antarmuka-dan-viewmodel-mvc.md`

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
- Verifikasi end-to-end API, RBAC, dan Web UI dilakukan mengikuti checklist pada `docs/03-Pengujian/03-01-pengujian-rencana-dan-kasus-uji.md`.
- Artefak bukti formal disimpan pada media dokumentasi pengujian yang dipakai tim atau penguji.
- Verifikasi lokal dilakukan dengan rangkaian perintah `dotnet restore`, `dotnet build`, `dotnet test`, dan `docker compose ... config`.

## Operasional DB (Backup/Restore)
- Backup database dilakukan dengan mekanisme native PostgreSQL sesuai environment deployment.
- Uji restore tetap wajib dilakukan berkala untuk memastikan backup dapat dipulihkan.

## Lisensi
Lisensi akan ditentukan untuk repositori ini.







