# Panduan Deployment Production
## Cashflowpoly Analytics Platform

### Dokumen
- Nama dokumen: Panduan Deployment Production
- Versi: 1.0
- Tanggal: 19 Februari 2026
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
   | `AUTH_BOOTSTRAP_SEED_DEFAULT_USERS` | `true` | Set `true` untuk buat user pertama |
   | `AUTH_BOOTSTRAP_INSTRUCTOR_USERNAME` | `admin` | Username instructor |
   | `AUTH_BOOTSTRAP_INSTRUCTOR_PASSWORD` | `Admin@123!` | Password instructor |
   | `AUTH_BOOTSTRAP_PLAYER_USERNAME` | `player1` | Username player |
   | `AUTH_BOOTSTRAP_PLAYER_PASSWORD` | `Player@123!` | Password player |

   > **Penting**: Setelah seed user berhasil (login pertama sukses), ubah `AUTH_BOOTSTRAP_SEED_DEFAULT_USERS` ke `false` agar tidak seed ulang.

---

### 4.2 Deploy Mode Development (Tanpa Nginx)

Mode ini mengekspos port API dan UI langsung. Cocok untuk pengembangan lokal.

```powershell
# Cara 1: Menggunakan script deploy
.\deploy.ps1 -Mode dev

# Cara 2: Manual
docker compose up -d --build
```

Untuk auto-redeploy saat source berubah:

```powershell
.\deploy.ps1 -Mode dev-watch
```

Akses:
- UI: `http://localhost:5203`
- API: `http://localhost:5041`
- Swagger: `http://localhost:5041/swagger`

---

### 4.3 Deploy Mode Production (Dengan Nginx)

Mode ini mengarahkan semua traffic melalui Nginx reverse proxy. Port API dan UI tidak terekspos langsung.

```powershell
# Cara 1: Menggunakan script deploy
.\deploy.ps1 -Mode prod

# Cara 2: Manual
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
```

Akses:
- Dashboard: `http://localhost`
- API: `http://localhost/api/`
- Swagger: `http://localhost/swagger`
- Health: `http://localhost/health/ready`

---

### 4.4 Mengaktifkan Cloudflare Tunnel (Akses Publik)

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
   docker compose -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel up -d --build
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

### 5.1 Menggunakan Script Deploy (`deploy.ps1`)

| Perintah | Fungsi |
|---|---|
| `.\deploy.ps1` | Deploy production (default) |
| `.\deploy.ps1 -Mode dev` | Deploy development |
| `.\deploy.ps1 -Mode dev-watch` | Deploy development + auto-redeploy |
| `.\deploy.ps1 -Mode status` | Cek status container |
| `.\deploy.ps1 -Mode logs` | Lihat log real-time |
| `.\deploy.ps1 -Mode down` | Hentikan semua container |
| `.\deploy.ps1 -Mode restart` | Restart semua container |

### 5.2 Perintah Docker Compose Manual

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

# Rebuild dan restart satu container
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build --no-deps api

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
git clone https://github.com/marcomarcello15/cashflowpoly-analytics-platform.git
cd cashflowpoly-analytics-platform

# 2. Salin file .env (JANGAN commit .env ke Git)
# Copy manual dari mesin lama, atau buat ulang dari template:
Copy-Item .env.example .env
notepad .env    # Isi semua variabel

# 3. Deploy production + tunnel
docker compose -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel up -d --build

# 4. Verifikasi
docker compose -f docker-compose.yml -f docker-compose.prod.yml ps
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

Repository ini menyertakan pipeline CI/CD di `.github/workflows/ci-cd.yml`.

### 7.1 Pipeline Jobs

```
push ke main -> [Build & Test] -> [Docker Build & Push] -> [Deploy]
```

| Job | Fungsi | Trigger |
|---|---|---|
| **Build & Test** | Restore/build per-`csproj`, validasi `docker compose config` (dev+prod), lalu test non-integration + integration | push & PR ke `main` |
| **Docker Build & Push** | Build image API/UI lalu push ke GitHub Container Registry (GHCR) | push ke `main` saja |
| **Deploy** | SSH ke mesin target, `git pull`, validasi compose, lalu `docker compose up -d --build` | push ke `main` saja |

### 7.2 Setup GitHub Secrets

Untuk mengaktifkan CI/CD, tambahkan secrets di GitHub Repository -> Settings -> Secrets and variables -> Actions:

| Secret | Nilai | Keterangan |
|---|---|---|
| `DEPLOY_HOST` | IP publik atau hostname mesin | Mesin target deploy |
| `DEPLOY_USER` | Username SSH | User di mesin target |
| `DEPLOY_SSH_KEY` | Private key SSH | Generate dengan `ssh-keygen` |
| `DEPLOY_PATH` | `/home/user/cashflowpoly-analytics-platform` | Path project di mesin target |
| `DEPLOY_PORT` | `22` | Port SSH mesin target (opsional, default 22) |

### 7.3 Monitoring CI/CD

- Buka tab **Actions** di repository GitHub
- Setiap push ke `main` akan memicu pipeline
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
| `deploy.ps1` | Script deploy untuk Windows (PowerShell) |
| `deploy.sh` | Script deploy untuk Linux/macOS (Bash) |
| `.github/workflows/ci-cd.yml` | Pipeline CI/CD GitHub Actions |
| `src/Cashflowpoly.Api/Dockerfile` | Multi-stage Docker build untuk API |
| `src/Cashflowpoly.Ui/Dockerfile` | Multi-stage Docker build untuk UI |
| `database/00_create_schema.sql` | Skrip inisiasi skema database |

---

## 11. Ringkasan Perintah

### Deploy Cepat (Copy-Paste)

```powershell
# ===== PERTAMA KALI =====
# 1. Buat .env
Copy-Item .env.example .env
notepad .env          # Edit semua variabel

# 2. Deploy production + tunnel
docker compose -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel up -d --build

# 3. Cek status
docker compose -f docker-compose.yml -f docker-compose.prod.yml ps

# 4. Cek tunnel
docker logs cashflowpoly-tunnel --tail 10

# ===== SEHARI-HARI =====
# Start
docker compose -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel up -d

# Stop
docker compose -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel down

# Rebuild setelah perubahan kode
docker compose -f docker-compose.yml -f docker-compose.prod.yml --profile tunnel up -d --build

# Lihat log
docker compose -f docker-compose.yml -f docker-compose.prod.yml logs -f --tail=50
```

---

## 12. Daftar Periksa Deployment

Sistem terdeploy dengan benar jika:

- [ ] File `.env` sudah diisi dengan nilai yang benar (bukan default)
- [ ] Semua container berstatus **healthy**: `db`, `api`, `ui`, `nginx`
- [ ] Container `cloudflared` berstatus **running** (jika menggunakan tunnel)
- [ ] Endpoint `/health/ready` mengembalikan `200 Healthy`
- [ ] Dashboard UI dapat diakses di `/`
- [ ] Swagger UI dapat diakses di `/swagger`
- [ ] Login API (`/api/v1/auth/login`) mengembalikan JWT token
- [ ] Domain `https://tugasakhirmarco.my.id` dapat diakses dari perangkat lain (HP, PC lain)
- [ ] SSL/HTTPS aktif (disediakan otomatis oleh Cloudflare)

