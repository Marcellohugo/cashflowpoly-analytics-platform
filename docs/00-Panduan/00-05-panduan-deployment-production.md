# Panduan Deployment Production
## Cashflowpoly Analytics Platform

### Dokumen
- Nama dokumen: Panduan Deployment Production
- Versi: 1.2
- Tanggal: 27 Maret 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan
Dokumen ini memandu deployment Cashflowpoly Analytics Platform ke lingkungan production menggunakan Docker Compose, build image lokal dari source, Nginx reverse proxy, dan Cloudflare Tunnel.

Perubahan utama versi ini:
- deployment tidak lagi bergantung pada workflow CI/CD eksternal,
- service `api` dan `ui` dibangun langsung dari source lokal saat deploy,
- auto-redeploy berbasis registry image dan Watchtower tidak dipakai lagi.

---

## 2. Arsitektur Deployment

```
Internet
    |
    v
Cloudflare Edge (SSL termination)
    v https://tugasakhirmarco.my.id
    |
+----------------------------------------------------------+
| Docker Host                                              |
|                                                          |
| cloudflared -> nginx:80 -> api:5041 (REST API)          |
|                      |-> ui:5203 (MVC Web)              |
|                      `-> swagger, health, dll           |
|                                                          |
| db (PostgreSQL 16) <- api                               |
+----------------------------------------------------------+
```

### Komponen Utama

| Komponen | Deskripsi | Port Internal |
|---|---|---|
| **PostgreSQL 16** | Database relasional | 5432 |
| **Cashflowpoly.Api** | REST API (.NET 10) | 5041 |
| **Cashflowpoly.Ui** | Dashboard MVC (.NET 10) | 5203 |
| **Nginx** | Reverse proxy, routing, rate limiting | 80, 443 |
| **Cloudflared** | Tunnel ke Cloudflare edge network | - |

### Routing Nginx

| Path | Diarahkan ke | Keterangan |
|---|---|---|
| `/` | UI (5203) | Dashboard utama |
| `/api/` | API (5041) | Semua endpoint REST API |
| `/api/v1/auth/login` | API (5041) | Login dengan rate limit ketat |
| `/swagger` | API (5041) | Swagger UI |
| `/health`, `/health/ready`, `/health/live` | API (5041) | Health check |
| `/api` atau `/api/` (exact) | API (5041) | Landing page API |
| Static assets (`.css`, `.js`, dll) | UI (5203) | Cache 7 hari |
| `/swagger/*.css`, `/swagger/*.js` | API (5041) | Swagger static assets |

---

## 3. Prasyarat

### 3.1 Perangkat Lunak

| Komponen | Versi Minimum | Cara Cek |
|---|---|---|
| Docker Desktop | 24.x | `docker --version` |
| Docker Compose V2 | 2.20+ | `docker compose version` |
| Git | 2.x | `git --version` |
| PowerShell | 5.1+ / 7+ | `$PSVersionTable.PSVersion` |

### 3.2 Akun dan Layanan

| Layanan | Fungsi | Keterangan |
|---|---|---|
| [Cloudflare](https://dash.cloudflare.com) | DNS dan Tunnel | Free plan |
| Domain `tugasakhirmarco.my.id` | Domain publik permanen | Dibeli di [MyDomaiNesia](https://my.domainesia.com) |

Catatan:
- Remote Git boleh berada di GitHub, GitLab, atau server Git lain.
- Workflow CI/CD eksternal tidak lagi diperlukan untuk build, test, maupun release.

---

## 4. Variabel Environment Penting

Salin template:

```powershell
Copy-Item .env.prod.example .env.prod
notepad .env.prod
```

Variabel yang wajib diisi:

| Variabel | Contoh Nilai | Keterangan |
|---|---|---|
| `POSTGRES_PASSWORD` | `Str0ng!P@ssw0rd` | Password database |
| `JWT_SIGNING_KEY` | (random 48 karakter) | Minimal 32 karakter |
| `AUTH_BOOTSTRAP_SEED_DEFAULT_USERS` | `true` | Hanya untuk bootstrap awal |
| `AUTH_BOOTSTRAP_INSTRUCTOR_USERNAME` | `admin` | Username instructor |
| `AUTH_BOOTSTRAP_INSTRUCTOR_PASSWORD` | `Admin@123!` | Password instructor |
| `AUTH_BOOTSTRAP_PLAYER_USERNAME` | `player1` | Username player |
| `AUTH_BOOTSTRAP_PLAYER_PASSWORD` | `Player@123!` | Password player |
| `CLOUDFLARE_TUNNEL_TOKEN` | `eyJ...` | Isi jika ingin akses publik via tunnel |

Catatan:
- Variabel registry image lama dan variabel Watchtower sudah tidak dipakai lagi.
- Setelah user bootstrap berhasil dibuat dan login pertama sukses, ubah `AUTH_BOOTSTRAP_SEED_DEFAULT_USERS` menjadi `false`.

---

## 5. Deploy Development

Mode ini untuk pengembangan lokal dengan auto-reload:

```powershell
docker context use default
Copy-Item .env.dev.example .env.dev
docker compose --env-file .env.dev -f docker-compose.yml -f docker-compose.watch.yml up --build
```

Akses:
- UI: `http://localhost:5203`
- API: `http://localhost:5041`
- Swagger: `http://localhost:5041/swagger`

Stop:

```powershell
docker compose --env-file .env.dev -f docker-compose.yml -f docker-compose.watch.yml down
```

Verifikasi lokal sebelum merge atau deploy:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/ops/run-local-checks.ps1 -SkipIntegrationTests
```

---

## 6. Deploy Production

### 6.1 Persiapan Source

Masuk ke revision yang ingin dideploy:

```powershell
git pull
```

Atau checkout commit/tag/branch yang diinginkan sebelum menjalankan deploy.

### 6.2 Verifikasi Lokal Sebelum Deploy

Disarankan menjalankan verifikasi penuh:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/ops/run-local-checks.ps1
```

Catatan:
- integration test membutuhkan Docker daemon aktif,
- jika hanya ingin build dan test cepat, gunakan `-SkipIntegrationTests`.

### 6.3 Deploy Production dengan Skrip

Perintah utama:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/ops/redeploy-production.ps1
```

Perilaku skrip:
- membaca `.env.prod` secara default,
- membangun image `api` dan `ui` langsung dari source lokal,
- menjalankan `db`, `api`, `ui`, dan `nginx`,
- otomatis menambahkan `cloudflared` jika `CLOUDFLARE_TUNNEL_TOKEN` terisi,
- menampilkan status container setelah deploy.

Jika ingin mematikan tunnel secara eksplisit:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/ops/redeploy-production.ps1 -WithoutTunnel
```

### 6.4 Deploy Production Manual

Tanpa skrip helper:

```powershell
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel up -d --build db api ui nginx cloudflared
```

Jika tidak memakai Cloudflare Tunnel:

```powershell
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml up -d --build db api ui nginx
```

### 6.5 Verifikasi Setelah Deploy

```powershell
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml ps
curl http://localhost/health/ready
docker logs cashflowpoly-nginx --tail 20
docker logs cashflowpoly-tunnel --tail 20
```

Ekspektasi:
- `db`, `api`, `ui`, dan `nginx` berstatus `healthy`,
- `cloudflared` berstatus `running` jika tunnel diaktifkan,
- endpoint `/health/ready` mengembalikan `200`.

### 6.6 Alur Update Kode Manual

Alur rilis yang disarankan:
1. Ubah source di mesin pengembang.
2. Jalankan `scripts/ops/run-local-checks.ps1`.
3. Commit dan push ke remote Git bila diperlukan.
4. Di mesin server, tarik revision terbaru dengan `git pull`.
5. Jalankan `scripts/ops/redeploy-production.ps1`.
6. Verifikasi health check dan login aplikasi.

Catatan:
- Tidak ada publish image otomatis ke registry.
- Tidak ada auto-update container saat source berubah.
- Setiap perubahan source, Dockerfile, `.env.prod`, atau `nginx/default.conf` perlu redeploy manual.

---

## 7. Cloudflare Tunnel

Cloudflare Tunnel memungkinkan aplikasi diakses publik tanpa membuka port langsung ke internet.

### 7.1 Setup Pertama Kali

1. Tambahkan domain `tugasakhirmarco.my.id` ke Cloudflare.
2. Ganti nameserver domain ke nameserver Cloudflare yang diberikan.
3. Buat Named Tunnel bernama `cashflowpoly`.
4. Tambahkan route ke `http://nginx:80`.
5. Salin token tunnel ke `.env.prod`:

```dotenv
CLOUDFLARE_TUNNEL_TOKEN=eyJhIjoiNGQ0N2......
```

6. Jalankan deploy production dengan skrip atau perintah manual berprofil `tunnel`.

### 7.2 Verifikasi Tunnel

```powershell
docker logs cashflowpoly-tunnel --tail 20
nslookup tugasakhirmarco.my.id 8.8.8.8
```

Jika tunnel aktif, domain publik yang diharapkan:

| Halaman | URL |
|---|---|
| Dashboard UI | `https://tugasakhirmarco.my.id` |
| API Landing | `https://tugasakhirmarco.my.id/api/` |
| Swagger UI | `https://tugasakhirmarco.my.id/swagger` |
| Health Check | `https://tugasakhirmarco.my.id/health/ready` |

---

## 8. Manajemen Container

### 8.1 Perintah Operasional Harian

```powershell
# Status container
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml ps

# Log container
docker logs cashflowpoly-api --tail 50
docker logs cashflowpoly-ui --tail 50
docker logs cashflowpoly-nginx --tail 50

# Restart service tertentu
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml restart api

# Rebuild dan redeploy service aplikasi
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml up -d --build api ui nginx

# Hentikan semua service production
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel down

# Hentikan semua service dan hapus volume database
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel down -v
```

### 8.2 Kapan Harus Rebuild

Jalankan `up -d --build` jika ada perubahan pada:
- source code API/UI,
- Dockerfile,
- dependensi NuGet/npm,
- file konfigurasi yang di-copy saat image build.

Jalankan `up -d` tanpa `--build` biasanya cukup jika hanya mengubah `.env.prod` atau konfigurasi compose yang tidak mempengaruhi hasil image.

---

## 9. Pindah Mesin

### 9.1 Langkah di Mesin Baru

```powershell
git clone <remote-git-anda> cashflowpoly-analytics-platform
cd cashflowpoly-analytics-platform

Copy-Item .env.prod.example .env.prod
notepad .env.prod

powershell -ExecutionPolicy Bypass -File scripts/ops/redeploy-production.ps1
```

### 9.2 Catatan Migrasi

| Aspek | Dampak |
|---|---|
| URL domain publik | Tetap sama selama route dan token tunnel sama |
| Database | Kosong jika volume tidak dipindahkan |
| File `.env.prod` | Harus dipindahkan atau dibuat ulang |
| Source code | Ditarik ulang dari remote Git |

### 9.3 Backup dan Restore Database

```powershell
# Backup di mesin lama
docker exec cashflowpoly-db pg_dump -U cashflowpoly cashflowpoly > backup.sql

# Restore di mesin baru
Get-Content backup.sql | docker exec -i cashflowpoly-db psql -U cashflowpoly cashflowpoly
```

---

## 10. Verifikasi Deployment

### 10.1 Health Check

```powershell
Invoke-WebRequest -Uri "http://localhost/health/ready" -UseBasicParsing
Invoke-WebRequest -Uri "https://tugasakhirmarco.my.id/health/ready" -UseBasicParsing
```

Ekspektasi: status `200`, body `Healthy`.

### 10.2 Dashboard UI

Buka `https://tugasakhirmarco.my.id`.

Ekspektasi: halaman login atau dashboard tampil.

### 10.3 API Landing

Buka `https://tugasakhirmarco.my.id/api/`.

Ekspektasi: halaman landing API tampil.

### 10.4 Swagger UI

Buka `https://tugasakhirmarco.my.id/swagger`.

Ekspektasi: daftar endpoint API tampil lengkap.

### 10.5 Login API

```powershell
$body = @{ username = "admin"; password = "Admin@123!" } | ConvertTo-Json
Invoke-RestMethod -Uri "https://tugasakhirmarco.my.id/api/v1/auth/login" -Method POST -Body $body -ContentType "application/json"
```

Ekspektasi: response berisi JWT token.

---

## 11. Pemecahan Masalah

### 11.1 Container Tidak Healthy

```powershell
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml ps
docker logs cashflowpoly-api --tail 50
docker logs cashflowpoly-nginx --tail 50
```

| Gejala | Kemungkinan Penyebab | Solusi |
|---|---|---|
| API unhealthy | Connection string atau secret JWT salah | Cek `.env.prod` |
| Nginx unhealthy | API/UI belum ready | Tunggu startup lalu cek log |
| Tunnel tidak konek | Token salah atau kosong | Cek `CLOUDFLARE_TUNNEL_TOKEN` |
| Build gagal | Dependensi belum sinkron | Jalankan `scripts/ops/run-local-checks.ps1` |

### 11.2 Domain Tidak Bisa Diakses

| Gejala | Solusi |
|---|---|
| `DNS_PROBE_FINISHED_NXDOMAIN` | Nameserver belum propagasi |
| `502 Bad Gateway` | API/UI belum ready atau Nginx salah route |
| `522 Connection timed out` | Container `cloudflared` mati |

### 11.3 Swagger Blank

Pastikan `nginx/default.conf` memiliki route static asset Swagger ke backend API, lalu redeploy Nginx:

```powershell
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml up -d --build nginx
```

### 11.4 DNS Cache Lokal

```powershell
ipconfig /flushdns
nslookup tugasakhirmarco.my.id 8.8.8.8
```

---

## 12. File Konfigurasi dan Skrip

| File | Fungsi |
|---|---|
| `docker-compose.yml` | Definisi service dasar (`db`, `api`, `ui`) |
| `docker-compose.prod.yml` | Override production, reverse proxy, tunnel |
| `.env.prod.example` | Template environment production |
| `scripts/ops/run-local-checks.ps1` | Verifikasi build, test, dan compose lokal |
| `scripts/ops/redeploy-production.ps1` | Build ulang dan jalankan ulang stack production |
| `nginx/default.conf` | Konfigurasi reverse proxy |
| `src/Cashflowpoly.Api/Dockerfile` | Build image API |
| `src/Cashflowpoly.Ui/Dockerfile` | Build image UI |
| `database/00_create_schema.sql` | Inisiasi skema database |

---

## 13. Ringkasan Perintah

### 13.1 Pertama Kali

```powershell
Copy-Item .env.prod.example .env.prod
notepad .env.prod
powershell -ExecutionPolicy Bypass -File scripts/ops/run-local-checks.ps1
powershell -ExecutionPolicy Bypass -File scripts/ops/redeploy-production.ps1
```

### 13.2 Setelah Ada Perubahan Kode

```powershell
git pull
powershell -ExecutionPolicy Bypass -File scripts/ops/run-local-checks.ps1 -SkipIntegrationTests
powershell -ExecutionPolicy Bypass -File scripts/ops/redeploy-production.ps1
```

### 13.3 Lihat Log

```powershell
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml logs -f --tail=50
```

---

## 14. Daftar Periksa Deployment

Sistem dianggap terdeploy dengan benar jika:

- [ ] File `.env.prod` sudah diisi dengan nilai non-default
- [ ] `POSTGRES_PASSWORD` dan `JWT_SIGNING_KEY` valid
- [ ] Semua container utama (`db`, `api`, `ui`, `nginx`) berstatus sehat
- [ ] `cloudflared` berjalan jika tunnel diaktifkan
- [ ] Endpoint `/health/ready` mengembalikan `200 Healthy`
- [ ] Dashboard UI dapat diakses
- [ ] Swagger UI dapat diakses
- [ ] Login API mengembalikan JWT token
- [ ] Redeploy manual dengan `scripts/ops/redeploy-production.ps1` berhasil dijalankan
