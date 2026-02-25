# Panduan Deployment Production
## Cashflowpoly Analytics Platform

### Dokumen
- Nama dokumen: Panduan Deployment Production
- Versi: 1.1
- Tanggal: 23 Februari 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan
Dokumen ini disusun untuk memandu proses deployment Cashflowpoly Analytics Platform ke lingkungan production menggunakan Docker Compose, Nginx reverse proxy, dan Cloudflare Tunnel agar sistem dapat diakses dari mana saja melalui domain `tugasakhirmarco.my.id`.

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

### 3.2 Akun & Layanan

| Layanan | Fungsi | Keterangan |
|---|---|---|
| [Cloudflare](https://dash.cloudflare.com) | DNS & Tunnel | Free plan |
| Domain `tugasakhirmarco.my.id` | Domain publik permanen | Dibeli di [MyDomaiNesia](https://my.domainesia.com) |
| [GitHub](https://github.com) | Repository & CI/CD | Free plan |

---

## 4. Langkah Deployment

### 4.1 Persiapan File Environment

1. **Salin template** `.env.example` ke `.env`:
   ```powershell
   Copy-Item .env.example .env
   ```

2. **Edit `.env`** dan ganti semua nilai default:
   ```powershell
   notepad .env
   ```

3. **Variabel yang wajib diubah:**

   | Variabel | Contoh Nilai | Keterangan |
   |---|---|---|
   | `POSTGRES_PASSWORD` | `Str0ng!P@ssw0rd` | Password database, jangan gunakan default |
   | `JWT_SIGNING_KEY` | (random 48 karakter) | Generate: `openssl rand -base64 48` |
   | `IMAGE_TAG` | `prod-latest` | Wajib untuk server production agar tidak menarik image branch `dev` |
   | `AUTH_BOOTSTRAP_SEED_DEFAULT_USERS` | `true` | Set `true` untuk buat user pertama |
   | `AUTH_BOOTSTRAP_INSTRUCTOR_USERNAME` | `admin` | Username instructor |
   | `AUTH_BOOTSTRAP_INSTRUCTOR_PASSWORD` | `Admin@123!` | Password instructor |
   | `AUTH_BOOTSTRAP_PLAYER_USERNAME` | `player1` | Username player |
   | `AUTH_BOOTSTRAP_PLAYER_PASSWORD` | `Player@123!` | Password player |
   | `GHCR_USERNAME` | `username-github` | Isi jika image GHCR private |
   | `GHCR_TOKEN` | `ghp_xxx...` | Personal access token `read:packages` untuk pull image private |

   > **Penting**: Setelah seed user berhasil (login pertama sukses), ubah `AUTH_BOOTSTRAP_SEED_DEFAULT_USERS` ke `false` agar tidak seed ulang.

---

### 4.2 Deploy Mode Development (Tanpa Nginx)

Mode ini mengekspos port API dan UI langsung. Cocok untuk pengembangan lokal.

```powershell
docker context use default
Copy-Item .env.example .env.dev
docker compose --env-file .env.dev -f docker-compose.yml -f docker-compose.watch.yml up --build
```

Akses:
- UI: `http://localhost:5203`
- API: `http://localhost:5041`
- Swagger: `http://localhost:5041/swagger`

---

### 4.3 Deploy Mode Production (Dengan Nginx)

Mode ini mengarahkan semua traffic melalui Nginx reverse proxy. Port API dan UI tidak terekspos langsung.

```powershell
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel pull api ui db nginx cloudflared watchtower
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel up -d db api ui nginx watchtower cloudflared
```

Akses:
- Dashboard: `http://localhost`
- API: `http://localhost/api/`
- Swagger: `http://localhost/swagger`
- Health: `http://localhost/health/ready`

Catatan:
- `docker-compose.prod.yml` memakai image dari GHCR (bukan build lokal) untuk service `api` dan `ui`.
- Default fallback tag production adalah `prod-latest`.

---

### 4.4 Alur End-to-End (PC Developer dan PC Target Deploy)

Alur ini memastikan deployment bisa dipicu dari mana saja, tetapi tetap aman untuk production.

#### Alur dari PC Developer

1. Ubah kode di branch `dev`, lalu `commit` dan `push` ke `origin/dev`.
2. Workflow `CI - Build & Test` berjalan.
3. Jika CI sukses, workflow `CD - Build & Push Images` mem-publish image:
   - branch `dev`: tag `dev-latest` dan `SHA`.
   - branch `prod`: tag `prod-latest`, `latest`, dan `SHA`.
4. Saat siap rilis, dorong perubahan ke branch `prod`.

#### Alur di PC Target Deploy (Docker Desktop)

1. Jalankan deploy awal:
   ```powershell
   docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel pull api ui db nginx cloudflared watchtower
   docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel up -d db api ui nginx watchtower cloudflared
   ```
2. Pastikan `IMAGE_TAG=prod-latest` pada `.env`.
3. Service `watchtower` akan cek image baru sesuai `WATCHTOWER_POLL_INTERVAL` (default 30 detik).
4. Ketika image `prod-latest` baru tersedia, container `api` dan `ui` akan auto pull dan rolling restart.

#### Batasan Auto-Deploy

- Auto-update berlaku untuk update image container (`api` dan `ui`).
- Perubahan file konfigurasi (contoh: `.env`, `docker-compose*.yml`, `nginx/default.conf`) tetap butuh deploy manual ulang (`docker compose ... up -d`).

---

### 4.5 Mengaktifkan Cloudflare Tunnel (Akses Publik)

Cloudflare Tunnel memungkinkan sistem diakses dari mana saja melalui internet tanpa membuka port di firewall atau memerlukan IP publik statis.

#### Named Tunnel dengan Domain `tugasakhirmarco.my.id`

Named Tunnel memberikan URL permanen di domain sendiri. URL tetap sama walaupun container di-restart atau pindah mesin.

> **Catatan**: Jika belum memiliki domain, dapat menggunakan Quick Tunnel sebagai alternatif sementara. Quick Tunnel memberikan URL random `*.trycloudflare.com` yang berubah setiap restart. Caranya: ganti `command` di `docker-compose.prod.yml` menjadi `tunnel --url http://nginx:80`, lalu cek URL di `docker logs cashflowpoly-tunnel`.

**Langkah setup pertama kali:**

1. **Domain `tugasakhirmarco.my.id`** dibeli di [MyDomaiNesia](https://my.domainesia.com).

2. **Tambahkan domain ke Cloudflare**:
   - Login ke [Cloudflare Dashboard](https://dash.cloudflare.com)
   - Klik **"Add a site"** -> masukkan `tugasakhirmarco.my.id`
   - Pilih plan **Free**
   - Cloudflare memberikan 2 nameserver:
     ```
     liv.ns.cloudflare.com
     quinton.ns.cloudflare.com
     ```

3. **Ganti nameserver di MyDomaiNesia**:
   - Login ke [MyDomaiNesia](https://my.domainesia.com) -> **Domains** -> `tugasakhirmarco.my.id` -> **Nameservers**
   - Ganti nameserver default ke:
     - Nameserver 1: `liv.ns.cloudflare.com`
     - Nameserver 2: `quinton.ns.cloudflare.com`
   - Klik **"Change Nameservers"**
   - Tunggu propagasi DNS (5 menit - 24 jam, tergantung registry `.my.id`/PANDI)
   - Status di Cloudflare Overview akan berubah ke **"Active"** setelah propagasi selesai

4. **Buat tunnel di Cloudflare**:
   - Cloudflare Dashboard -> **Networking** -> **Tunnels**
   - Klik **"Create a tunnel"** -> pilih **Cloudflared** -> beri nama: `cashflowpoly`
   - Cloudflare akan menampilkan **Tunnel Token** (string panjang `eyJ...`)
   - Copy token tersebut

5. **Tambahkan route**:
   - Di halaman tunnel -> tab **Routes** -> **"+ Add route"**
   - Pilih **"Published application"**
   - Isi:
     - **Subdomain**: *(kosongkan)*
     - **Domain**: pilih `tugasakhirmarco.my.id` dari dropdown
     - **Path**: *(kosongkan)*
     - **Service URL**: `http://nginx:80`
   - Klik **"Add route"**

6. **Simpan token ke `.env`**:
   ```dotenv
   CLOUDFLARE_TUNNEL_TOKEN=eyJhIjoiNGQ0N2......
   ```

7. **Pastikan `docker-compose.prod.yml`** menggunakan Named Tunnel:
   ```yaml
   cloudflared:
     image: cloudflare/cloudflared:latest
     container_name: cashflowpoly-tunnel
     restart: unless-stopped
     profiles: ["tunnel"]
     command: tunnel --no-autoupdate --protocol http2 run --token ${CLOUDFLARE_TUNNEL_TOKEN}
     depends_on:
       nginx:
         condition: service_healthy
     networks:
       - cashflowpoly-net
   ```

8. **Jalankan**:
   ```powershell
   docker compose -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel up -d
   ```

9. **Verifikasi** tunnel berjalan:
   ```powershell
   docker logs cashflowpoly-tunnel --tail 10
   ```
   Pastikan muncul pesan:
   ```
   INF Registered tunnel connection connIndex=0 ...
   INF Updated to new configuration config="..." ...
   ```

10. **Cek DNS propagasi**:
    ```powershell
    nslookup tugasakhirmarco.my.id 8.8.8.8
    ```
    Jika sudah resolve, output akan menampilkan IP Cloudflare (104.x.x.x atau 172.x.x.x).
    Jika belum, jalankan `ipconfig /flushdns` dan tunggu beberapa menit.

11. **Akses sistem** melalui domain permanen:

    | Halaman | URL |
    |---|---|
    | Dashboard UI | `https://tugasakhirmarco.my.id` |
    | API Landing | `https://tugasakhirmarco.my.id/api/` |
    | Swagger UI | `https://tugasakhirmarco.my.id/swagger` |
    | Health Check | `https://tugasakhirmarco.my.id/health/ready` |
    | Login API | `POST https://tugasakhirmarco.my.id/api/v1/auth/login` |

---

## 5. Manajemen Container

### 5.1 Perintah Docker Compose Manual

```powershell
# Lihat status semua container
docker compose -f docker-compose.yml -f docker-compose.prod.yml ps

# Lihat log container tertentu
docker logs cashflowpoly-api --tail 50
docker logs cashflowpoly-ui --tail 50
docker logs cashflowpoly-nginx --tail 50
docker logs cashflowpoly-tunnel --tail 50

# Restart satu container
docker compose -f docker-compose.yml -f docker-compose.prod.yml restart api

# Redeploy image terbaru untuk API
docker compose -f docker-compose.yml -f docker-compose.prod.yml pull api
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --no-build --force-recreate --no-deps api

# Hentikan semua (data database tetap aman)
docker compose -f docker-compose.yml -f docker-compose.prod.yml down

# Hentikan semua DAN hapus volume database (perhatian: data hilang!)
docker compose -f docker-compose.yml -f docker-compose.prod.yml down -v
```

---

## 6. Pindah Mesin (Migrasi)

Jika perlu memindahkan deployment dari PC pribadi ke PC lab atau mesin lain:

### 6.1 Langkah di Mesin Baru

```powershell
# 1. Clone repository
git clone https://github.com/Marcellohugo/cashflowpoly-analytics-platform.git
cd cashflowpoly-analytics-platform
git checkout prod

# 2. Salin file .env (JANGAN commit .env ke Git)
# Copy manual dari mesin lama, atau buat ulang dari template:
Copy-Item .env.example .env.prod
notepad .env.prod    # Isi semua variabel

# 3. Deploy production (script akan pull image dan recreate service)
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel pull api ui db nginx cloudflared watchtower
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel up -d db api ui nginx watchtower cloudflared

# 4. Verifikasi
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml ps
```

### 6.2 Catatan Penting Saat Pindah Mesin

| Aspek | Dampak |
|---|---|
| **URL `tugasakhirmarco.my.id`** | Tetap sama - tidak berubah, selama tunnel token dan route sama |
| **URL Quick Tunnel (jika dipakai)** | Berubah - URL random baru setiap restart |
| **Data database** | Database dibuat ulang (kosong). Jika perlu data lama, backup dulu dengan `pg_dump` |
| **File `.env`** | Harus di-copy manual (tidak ada di Git) |
| **Konfigurasi lainnya** | Semua ikut di Git |

### 6.3 Backup & Restore Database

```powershell
# Backup di mesin lama
docker exec cashflowpoly-db pg_dump -U cashflowpoly cashflowpoly > backup.sql

# Restore di mesin baru (setelah container jalan)
Get-Content backup.sql | docker exec -i cashflowpoly-db psql -U cashflowpoly cashflowpoly
```

---

## 7. CI/CD dengan GitHub Actions

Repository ini menyertakan pipeline CI/CD pada dua workflow:
- `.github/workflows/ci.yml` untuk build dan test.
- `.github/workflows/cd.yml` untuk build dan push image ke GHCR.

### 7.1 Pipeline Jobs

```
push ke dev/prod -> [Build & Test] -> [Docker Build & Push] -> [Watchtower auto-update di server target]
```

| Job | Fungsi | Trigger |
|---|---|---|
| **Build & Test** | Restore/build per-`csproj`, validasi `docker compose config` (dev+prod), lalu test non-integration + integration | push & PR di semua branch |
| **Docker Build & Push** | Build image API/UI lalu push ke GHCR dengan tag sesuai branch dan SHA commit | push ke `dev` atau `prod` (setelah CI sukses), atau manual via `workflow_dispatch` |
| **Auto-Redeploy (Watchtower)** | Menarik image terbaru untuk service berlabel (`api`, `ui`) lalu restart container | polling berkala di server target |

Aturan tag image:
- Push ke `dev`: publish `dev-latest` dan `<SHA>`.
- Push ke `prod`: publish `prod-latest`, `latest`, dan `<SHA>`.
- Server production disarankan tetap memakai `IMAGE_TAG=prod-latest`.

### 7.2 Secrets yang Dibutuhkan

Untuk workflow CI/CD saat ini:
- `ci.yml` tidak butuh secret khusus.
- `cd.yml` memakai `GITHUB_TOKEN` bawaan GitHub Actions untuk push image ke GHCR.

Untuk deploy manual di server target (opsional, jika image private), siapkan variabel environment berikut di mesin server:

| Secret | Nilai | Keterangan |
|---|---|---|
| `GHCR_USERNAME` | Username GitHub | Untuk login registry GHCR saat deploy manual |
| `GHCR_TOKEN` | Personal Access Token (`read:packages`) | Token pull image dari GHCR (jika image private) |

### 7.3 Monitoring CI/CD

- Buka tab **Actions** di repository GitHub
- Setiap push ke `dev` atau `prod` akan memicu pipeline
- Status pipeline: sukses atau gagal
- Klik job yang gagal untuk melihat log error

---

## 8. Verifikasi Deployment

Setelah deploy selesai, verifikasi seluruh endpoint:

### 8.1 Health Check

```powershell
# Via localhost (dari mesin yang sama)
Invoke-WebRequest -Uri "http://localhost/health/ready" -UseBasicParsing

# Via domain publik
Invoke-WebRequest -Uri "https://tugasakhirmarco.my.id/health/ready" -UseBasicParsing
```

Ekspektasi: Status `200`, body `Healthy`.

### 8.2 Dashboard UI

Buka browser: `https://tugasakhirmarco.my.id`

Ekspektasi: Halaman login atau dashboard tampil.

### 8.3 API Landing Page

Buka browser: `https://tugasakhirmarco.my.id/api/`

Ekspektasi: Halaman landing API dengan link ke Swagger.

### 8.4 Swagger UI

Buka browser: `https://tugasakhirmarco.my.id/swagger`

Ekspektasi: Swagger UI menampilkan daftar endpoint API.

### 8.5 Login API

```powershell
$body = @{ username = "admin"; password = "Admin@123!" } | ConvertTo-Json
Invoke-RestMethod -Uri "https://tugasakhirmarco.my.id/api/v1/auth/login" -Method POST -Body $body -ContentType "application/json"
```

Ekspektasi: Response berisi JWT token.

---

## 9. Pemecahan Masalah

### 9.1 Container Tidak Healthy

```powershell
# Cek status
docker compose -f docker-compose.yml -f docker-compose.prod.yml ps

# Cek log container yang bermasalah
docker logs cashflowpoly-api --tail 50
docker logs cashflowpoly-nginx --tail 50
```

| Gejala | Kemungkinan Penyebab | Solusi |
|---|---|---|
| API unhealthy | Connection string salah | Cek `POSTGRES_PASSWORD` di `.env` |
| Nginx unhealthy | API/UI belum ready | Tunggu startup (15-30 detik) |
| Tunnel tidak konek | Token salah | Cek `CLOUDFLARE_TUNNEL_TOKEN` di `.env` |
| Database error | Skema belum diinisiasi | Hapus volume: `docker compose down -v` lalu `up` ulang |

### 9.2 Domain Tidak Bisa Diakses

| Gejala | Solusi |
|---|---|
| `DNS_PROBE_FINISHED_NXDOMAIN` | Nameserver belum propagasi. Tunggu 1-24 jam |
| `502 Bad Gateway` | Nginx sudah jalan tapi API/UI belum ready. Cek log API |
| `522 Connection timed out` | Cloudflared container mati. Restart: `docker restart cashflowpoly-tunnel` |
| Halaman Cloudflare error | Cek SSL/TLS mode di Cloudflare -> set ke **Flexible** atau **Full** |

### 9.3 Swagger Halaman Kosong (Blank)

Jika Swagger UI blank (putih):
1. Buka Developer Tools (F12) -> tab Network
2. Cek apakah file `.js` dan `.css` Swagger return 200 dengan ukuran besar (>100KB)
3. Jika ukurannya kecil (~5KB), artinya static assets diarahkan ke UI, bukan API
4. Pastikan `nginx/default.conf` memiliki rule khusus untuk Swagger static assets:
   ```nginx
   location ~* ^/swagger/.*\.(css|js|png|ico|map|json)$ {
       proxy_pass http://api_backend;
   }
   ```

### 9.4 DNS Cache Lokal

Jika domain sudah aktif tapi PC sendiri belum bisa akses:
```powershell
# Flush DNS cache
ipconfig /flushdns

# Coba akses lagi
nslookup tugasakhirmarco.my.id 8.8.8.8
```

---

## 10. File Konfigurasi Deployment

Berikut daftar file yang berkaitan dengan deployment:

| File | Fungsi |
|---|---|
| `docker-compose.yml` | Definisi service dasar (db, api, ui) |
| `docker-compose.prod.yml` | Override production (Nginx, Cloudflare Tunnel) |
| `.env` | Variabel environment (tidak di-commit ke Git) |
| `.env.example` | Template variabel environment |
| `nginx/default.conf` | Konfigurasi Nginx reverse proxy |
| `.github/workflows/ci.yml` | Pipeline CI build & test |
| `.github/workflows/cd.yml` | Pipeline CD build & push image |
| `src/Cashflowpoly.Api/Dockerfile` | Multi-stage Docker build untuk API |
| `src/Cashflowpoly.Ui/Dockerfile` | Multi-stage Docker build untuk UI |
| `database/00_create_schema.sql` | Skrip inisiasi skema database |

---

## 11. Ringkasan Perintah

### Deploy Cepat (Copy-Paste)

```powershell
# ===== PERTAMA KALI =====
# 1. Buat .env.prod
Copy-Item .env.example .env.prod
notepad .env.prod     # Edit semua variabel secret + IMAGE_TAG=prod-latest

# 2. Deploy production
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel pull api ui db nginx cloudflared watchtower
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel up -d db api ui nginx watchtower cloudflared

# 3. Cek status
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml ps

# 4. Cek tunnel
docker logs cashflowpoly-tunnel --tail 10

# ===== SEHARI-HARI =====
# Rilis perubahan kode:
# a) push perubahan ke branch prod dari PC developer
# b) tunggu CI/CD sukses
# c) watchtower di server target auto-update container

# Jika perlu paksa redeploy manual
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel pull api ui db nginx cloudflared watchtower
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel up -d db api ui nginx watchtower cloudflared

# Lihat log
docker compose --env-file .env.prod -f docker-compose.yml -f docker-compose.prod.yml logs -f --tail=50
```

---

## 12. Daftar Periksa Deployment

Sistem terdeploy dengan benar jika:

- [ ] File `.env` sudah diisi dengan nilai yang benar (bukan default)
- [ ] `IMAGE_TAG` pada `.env` production bernilai `prod-latest`
- [ ] Semua container berstatus **healthy**: `db`, `api`, `ui`, `nginx`
- [ ] Container `cloudflared` berstatus **running** (jika menggunakan tunnel)
- [ ] Container `watchtower` berstatus **running** untuk auto-redeploy
- [ ] Endpoint `/health/ready` mengembalikan `200 Healthy`
- [ ] Dashboard UI dapat diakses di `/`
- [ ] Swagger UI dapat diakses di `/swagger`
- [ ] Login API (`/api/v1/auth/login`) mengembalikan JWT token
- [ ] Domain `https://tugasakhirmarco.my.id` dapat diakses dari perangkat lain (HP, PC lain)
- [ ] SSL/HTTPS aktif (disediakan otomatis oleh Cloudflare)

