# Panduan Deployment Produksi
## Cashflowpoly Analytics Platform

### Informasi Dokumen
- **Nama Dokumen**: Panduan Deployment Produksi
- **Versi**: 1.3
- **Tanggal**: 20 Juni 2026
- **Penyusun**: Marco Marcello Hugo

---

## 1. Tujuan
Dokumen ini disusun untuk memandu jalannya deployment Cashflowpoly Analytics Platform ke lingkungan produksi menggunakan Docker Compose, proses kompilasi (*build*) image lokal dari source code, konfigurasi Nginx reverse proxy, dan Cloudflare Tunnel.

Perubahan utama pada versi ini:
- Deployment tidak lagi bergantung pada workflow CI/CD eksternal.
- Service `api` dan `ui` dibangun langsung dari source code lokal saat proses deploy dijalankan.
- Mekanisme auto-redeploy berbasis registry image dan Watchtower tidak digunakan lagi.

---

## 2. Arsitektur Deployment

```
Internet
    |
    v
Cloudflare Edge (SSL Termination)
    v https://narafin.org
    |
+----------------------------------------------------------+
| Docker Host (Server)                                     |
|                                                          |
| cloudflared -> nginx:80 -> api:5041 (REST API)          |
|                      |-> ui:5203 (MVC Web)              |
|                      `-> health dan static asset        |
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
| **Nginx** | Reverse proxy HTTP internal, routing, dan pembatasan rate limit | 80 |
| **Cloudflared** | Tunnel ke jaringan tepi Cloudflare | - |

### Aturan Routing Nginx

| Path | Diarahkan ke | Keterangan |
|---|---|---|
| `/` | UI (5203) | Dashboard utama analitika |
| `/api/` | API (5041) | Semua endpoint REST API |
| `/api/v1/auth/login` | API (5041) | Login dengan pembatasan rate limit ketat |
| `/health` | UI (5203) lalu API/DB | Readiness seluruh aplikasi |
| `/health/ready`, `/health/live` | API (5041) | Readiness/liveness API |
| Static assets (`.css`, `.js`, dll) | UI (5203) | Cache browser selama 10 menit |

---

## 3. Prasyarat Sistem

### 3.1 Perangkat Lunak

| Perangkat Lunak | Versi Minimum | Cara Cek |
|---|---|---|
| Docker Desktop / Engine | 24.x | `docker --version` |
| Docker Compose V2 | 2.20+ | `docker compose version` |
| Git | 2.x | `git --version` |
| PowerShell | 5.1+ / 7+ | `$PSVersionTable.PSVersion` |

### 3.2 Akun dan Layanan
- **Akun Cloudflare**: Digunakan untuk mengelola DNS dan Tunnel.
- **Domain `narafin.org`**: Domain publik permanen (dibeli melalui registrar MyDomaiNesia).

---

## 4. Variabel Lingkungan Penting (Environment Variables)

Salin template berkas environment produksi:

```powershell
Copy-Item config/env/.env.prod.example config/env/.env.prod
notepad config/env/.env.prod
```

Konfigurasi kunci yang wajib diisi:

| Variabel | Contoh Nilai | Keterangan |
|---|---|---|
| `POSTGRES_PASSWORD` | `SandiKuatDB123!` | Password untuk basis data PostgreSQL |
| `JWT_SIGNING_KEY` | (kunci acak 48 karakter) | Minimal 32 karakter untuk pengamanan token |
| `AUTH_BOOTSTRAP_SEED_DEFAULT_USERS` | `true` | Set `true` hanya untuk inisiasi akun awal |
| `AUTH_BOOTSTRAP_INSTRUCTOR_USERNAME` | `rina.kartika` | Username akun instruktur bootstrap |
| `AUTH_BOOTSTRAP_INSTRUCTOR_PASSWORD` | `SandiSeed!2026` | Password akun instruktur bootstrap |
| `AUTH_BOOTSTRAP_PLAYER_USERNAME` | `marco` | Username akun player bootstrap |
| `AUTH_BOOTSTRAP_PLAYER_PASSWORD` | `SandiSeed!2026` | Password akun player bootstrap |
| `CLOUDFLARE_TUNNEL_TOKEN` | `eyJ...` | Token koneksi Cloudflare Tunnel |

*Catatan Keamanan*: Setelah akun bootstrap berhasil dibuat dan masuk pertama kali, segera ubah nilai `AUTH_BOOTSTRAP_SEED_DEFAULT_USERS` menjadi `false`.

---

## 5. Deployment Lingkungan Pengembangan (Development)

Untuk menjalankan sistem secara lokal di lingkungan pengembangan dengan fitur *auto-reload*:

```powershell
docker context use default
Copy-Item config/env/.env.dev.example config/env/.env.dev
docker compose --env-file config/env/.env.dev -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.watch.yml up --build
```

Akses Layanan:
- **Dashboard UI**: `http://localhost:5203`
- **REST API / Swagger**: `http://localhost:5041/swagger`

Untuk menghentikan kontainer pengembangan:

```powershell
docker compose --env-file config/env/.env.dev -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.watch.yml down
```

---

## 6. Deployment Lingkungan Produksi (Production)

### 6.1 Sinkronisasi Source Code
Pastikan Anda berada di commit/tag/cabang yang ingin dideploy:

```powershell
git pull
```

### 6.2 Verifikasi Lokal Sebelum Deploy
Disarankan untuk melakukan verifikasi penuh atas build dan unit testing secara lokal:

```powershell
dotnet restore src/Cashflowpoly.Api/Cashflowpoly.Api.csproj
dotnet restore src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj
dotnet build src/Cashflowpoly.Api/Cashflowpoly.Api.csproj -c Release /warnaserror
dotnet build src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj -c Release /warnaserror
dotnet test tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj -c Release --filter "Category!=Integration"
./scripts/Test-ProductionReadiness.ps1
```

### 6.3 Menjalankan Deploy Produksi
Jalankan perintah berikut untuk mengompilasi image lokal dan memulai seluruh service:

```powershell
docker compose --env-file config/env/.env.prod -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.prod.yml --profile tunnel up -d --build db api ui nginx cloudflared
```

Template production ini menggunakan Cloudflare Tunnel sebagai terminasi HTTPS. Deployment dihentikan oleh skrip kesiapan apabila token tunnel atau rahasia production belum valid.

### 6.4 Verifikasi Status Setelah Deploy
Pastikan semua kontainer berjalan dengan normal dan sehat:

```powershell
docker compose --env-file config/env/.env.prod -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.prod.yml ps
curl http://localhost/health
docker logs cashflowpoly-nginx --tail 20
```

Ekspektasi:
- Kontainer `db`, `api`, `ui`, dan `nginx` berstatus `healthy`.
- Endpoint `/health` mengembalikan respons `200 OK` hanya jika UI, API, dan database siap.

---

## 7. Pengaturan Cloudflare Tunnel

Cloudflare Tunnel digunakan agar aplikasi dapat diakses secara publik melalui SSL/HTTPS tanpa harus membuka port router server secara langsung.

1. Hubungkan domain `narafin.org` ke DNS Cloudflare.
2. Pada dasbor Cloudflare, buat Named Tunnel baru dengan nama `cashflowpoly`.
3. Tambahkan rute publik yang mengarah ke kontainer Nginx internal: `http://nginx:80`.
4. Salin token tunnel yang diberikan Cloudflare ke file konfigurasi `config/env/.env.prod` pada variabel `CLOUDFLARE_TUNNEL_TOKEN`.
5. Mulai kontainer dengan menyertakan profil `tunnel`.

Setelah tunnel aktif, Anda dapat mengakses:
- **Dashboard UI**: `https://narafin.org`
- **Readiness aplikasi**: `https://narafin.org/health`
- **Readiness API**: `https://narafin.org/health/ready`

Swagger UI sengaja tidak dipublikasikan pada environment Production. Gunakan koleksi Postman atau jalankan API pada environment Development untuk melihat OpenAPI/Swagger.

---

## 8. Manajemen Kontainer Harian

Berikut daftar perintah yang sering digunakan untuk operasional:

```powershell
# Memantau log service API secara real-time
docker logs cashflowpoly-api -f --tail 50

# Merestart service backend API
docker compose -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.prod.yml restart api

# Melakukan kompilasi ulang (rebuild) setelah ada perubahan kode
docker compose --env-file config/env/.env.prod -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.prod.yml up -d --build db api ui nginx

# Menghentikan seluruh service produksi
docker compose --env-file config/env/.env.prod -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.prod.yml --profile tunnel down

# Menghentikan service dan HAPUS data database (Tindakan Destruktif)
docker compose --env-file config/env/.env.prod -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.prod.yml --profile tunnel down -v
```

---

## 9. Pemecahan Masalah (Troubleshooting)

### 9.1 Kontainer Gagal Mencapai Status Healthy
*   **Penyebab**: Konfigurasi connection string PostgreSQL atau JWT signing key bermasalah.
*   **Solusi**: Periksa log API menggunakan perintah `docker logs cashflowpoly-api`. Cek kebenaran isian password DB di file `.env.prod`.

### 9.2 Swagger Tidak Bisa Diakses di Produksi
*   **Penyebab**: Swagger hanya diaktifkan pada environment Development sebagai kebijakan pengurangan permukaan informasi publik.
*   **Solusi**: Gunakan koleksi `postman/Cashflowpoly.postman_collection.json`. Untuk inspeksi Swagger, jalankan API secara lokal dengan `ASPNETCORE_ENVIRONMENT=Development` dan buka `http://localhost:5041/swagger`.

### 9.3 Backup dan Restore Database PostgreSQL (Kontainer)
Untuk melakukan ekspor data (backup):
```powershell
docker exec cashflowpoly-db pg_dump -U cashflowpoly cashflowpoly > backup.sql
```
Untuk mengimpor kembali data (restore):
```powershell
Get-Content backup.sql | docker exec -i cashflowpoly-db psql -U cashflowpoly cashflowpoly
```
