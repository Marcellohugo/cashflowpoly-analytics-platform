# Panduan Deployment Produksi
## Cashflowpoly Analytics Platform

### Informasi Dokumen

- Nama dokumen: Panduan Deployment Produksi
- Versi: 2.0
- Tanggal: 26 Agustus 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan dan batasan

Dokumen ini menjelaskan deployment manual Cashflowpoly ke VPS melalui Docker Compose dan Cloudflare Tunnel. Commit yang dipasang selalu commit terbaru `origin/prod`. VPS membangun image dari source, menjalankan migrasi maju, menyelaraskan Seed 2 secara idempoten, menghitung ulang analitik, lalu melakukan health check.

Keputusan operasional proyek:

- deployment dijalankan sebagai `root` melalui SSH;
- tidak ada GitHub Actions atau staging terpisah;
- jeda pemeliharaan singkat diperbolehkan;
- rollback hanya mengembalikan image aplikasi, bukan schema database;
- tidak ada backup database, backup sebelum migrasi, atau restore test;
- hanya rilis aktif dan satu rilis sebelumnya yang dipertahankan.

## 2. Arsitektur produksi

```text
Internet
  -> Cloudflare Edge (TLS)
  -> Cloudflare Tunnel
  -> nginx:8080
       -> ui:5203
       -> api:5041
            -> db:5432
```

PostgreSQL, API, UI, dan `/metrics` tidak diekspos langsung. Hanya Nginx yang dipetakan ke `127.0.0.1:80`; Cloudflare Tunnel meneruskan trafik publik ke `nginx:8080`. Nginx mengembalikan `404` untuk `/metrics`.

| Service | Image/runtime | Port internal | Akses publik |
|---|---|---:|---|
| `db` | PostgreSQL 16.15 | 5432 | Tidak |
| `api` | .NET 10.0.4 | 5041 | Melalui `/api/` |
| `ui` | .NET 10.0.4 | 5203 | Melalui `/` |
| `nginx` | nginx-unprivileged 1.31.3 | 8080 | Melalui tunnel |
| `cloudflared` | 2026.8.1 | - | Koneksi keluar saja |

API, UI, dan Nginx berjalan sebagai pengguna non-root. Semua service memakai `no-new-privileges` dan log production dikirim ke journald.

## 3. Prasyarat

VPS membutuhkan Ubuntu 24.04, Git, Docker Engine, Docker Compose V2, `curl`, `flock`, `logger`, `realpath`, systemd, akses keluar ke GitHub dan Cloudflare, serta minimal ruang untuk dua rilis dan image terkait.

Repository deployment harus dapat menjalankan `git fetch origin prod` tanpa prompt interaktif. Domain produksi adalah `https://narafin.org`.

## 4. Konfigurasi awal VPS

Jalankan sekali sebagai `root`:

```bash
install -d /opt/cashflowpoly/repository /opt/cashflowpoly/releases /opt/cashflowpoly/shared
git clone <repository-url> /opt/cashflowpoly/repository
cp /opt/cashflowpoly/repository/config/env/.env.prod.example /opt/cashflowpoly/shared/.env.prod
chmod 600 /opt/cashflowpoly/shared/.env.prod
```

Isi `/opt/cashflowpoly/shared/.env.prod`:

- `POSTGRES_DB` dan `POSTGRES_USER`;
- `POSTGRES_PASSWORD` minimal 16 karakter dan bukan placeholder;
- `JWT_SIGNING_KEY` minimal 32 karakter dan bukan placeholder;
- `DOMAIN=narafin.org`;
- `CLOUDFLARE_TUNNEL_TOKEN`;
- kredensial bootstrap hanya bila benar-benar diperlukan.

Validasi file environment dari checkout lokal:

```powershell
./scripts/Test-ProductionReadiness.ps1 -EnvironmentFile config/env/.env.prod
```

Jangan mencatat nilai rahasia di repository, command history, tiket, atau log deployment.

## 5. Konfigurasi Cloudflare

1. Hubungkan `narafin.org` ke Cloudflare DNS.
2. Buat Named Tunnel untuk aplikasi.
3. Arahkan hostname publik ke `http://nginx:8080`.
4. Simpan token pada `CLOUDFLARE_TUNNEL_TOKEN` di environment production.
5. Jangan membuka port PostgreSQL, API, atau UI pada firewall VPS.

## 6. Gerbang sebelum rilis

Di komputer pengembang, jalankan gerbang verifikasi lengkap:

```powershell
./scripts/Invoke-ReleaseVerification.ps1
```

Setelah seluruh pemeriksaan lulus:

```powershell
git switch prod
git merge --ff-only codex/project-alignment
git push origin prod
```

Jangan melakukan deployment dari perubahan lokal yang belum ada pada `origin/prod`.

## 7. Menjalankan deployment

Masuk ke VPS sebagai `root`, lalu:

```bash
git -C /opt/cashflowpoly/repository fetch origin prod
git -C /opt/cashflowpoly/repository checkout --force origin/prod
/opt/cashflowpoly/repository/scripts/deploy-production.sh
```

Skrip deployment melakukan langkah berikut:

1. Mengambil lock `/var/lock/cashflowpoly-deploy.lock` agar tidak ada dua deployment bersamaan.
2. Mengambil SHA terbaru `origin/prod` dan membuat worktree rilis per SHA.
3. Memvalidasi Docker Compose dan environment.
4. Memasang retensi journald 30 hari dengan batas disk.
5. Menarik image eksternal yang dipin dan membangun API/UI berurutan.
6. Menampilkan halaman pemeliharaan singkat bila ada rilis sebelumnya.
7. Menyalakan PostgreSQL dan menunggu status sehat.
8. Menjalankan API `--migrate-only`.
9. Menjalankan Seed 2 idempoten melalui konfigurasi migrasi/seed.
10. Menjalankan `--recalculate-analytics` untuk seluruh sesi.
11. Menyalakan API, UI, Nginx, dan tunnel baru.
12. Memeriksa health container, `/health`, serta `/privacy`.
13. Mengubah symlink `current`, menyimpan dua rilis terakhir, dan membersihkan image lama.

## 8. Verifikasi setelah deployment

```bash
docker compose --project-name cashflowpoly \
  --env-file /opt/cashflowpoly/shared/.env.prod \
  -f /opt/cashflowpoly/current/infra/docker/docker-compose.yml \
  -f /opt/cashflowpoly/current/infra/docker/docker-compose.prod.yml \
  --profile tunnel ps

curl --fail https://narafin.org/health
curl --fail https://narafin.org/privacy
curl --fail https://narafin.org/terms
```

Lakukan smoke test login Instruktur dan Player Seed 2, daftar sesi, setup, event, analitik pemain, buku aturan, Privasi, dan Ketentuan. Permintaan publik `https://narafin.org/metrics` harus menghasilkan `404`.

## 9. Rollback dan kegagalan

Jika build, migrasi, container health, atau smoke test gagal, trap pada skrip menjalankan kembali image SHA sebelumnya. Database tidak diturunkan. Karena itu seluruh migrasi wajib memakai pola expand/contract sehingga aplikasi versi sebelumnya tetap dapat membaca schema yang sudah maju.

Periksa kegagalan dengan:

```bash
journalctl -t cashflowpoly-deploy --since today
journalctl CONTAINER_NAME=cashflowpoly-api --since "30 minutes ago"
docker inspect cashflowpoly-api cashflowpoly-ui cashflowpoly-nginx cashflowpoly-tunnel
```

Jangan mengatasi kegagalan dengan `down -v`, `DROP DATABASE`, reset Seed, atau penghapusan direktori rilis secara manual.

## 10. Keputusan tanpa backup

Deployment tidak membuat backup database, tidak melakukan restore test, dan tidak membuat backup sebelum migrasi. Konsekuensinya, kerusakan VPS, kesalahan operator, atau migrasi destruktif dapat menyebabkan kehilangan data permanen. Rollback image tidak memulihkan schema atau data.

Karena risiko tersebut diterima oleh keputusan proyek, setiap migrasi wajib:

- mempunyai versi baru dan checksum yang tidak berubah setelah diterapkan;
- lulus pada database kosong serta database baseline lama;
- idempoten ketika mode migrasi dijalankan ulang;
- bersifat expand/contract dan tidak bergantung pada downgrade schema.

## 11. Operasi rutin

```bash
# Deploy commit prod terbaru
/opt/cashflowpoly/repository/scripts/deploy-production.sh

# Status service
docker compose --project-name cashflowpoly ps

# Log aplikasi
journalctl CONTAINER_NAME=cashflowpoly-api --since today

# Rilis aktif
readlink -f /opt/cashflowpoly/current
```

Swagger hanya aktif pada environment Development. Gunakan `postman/Cashflowpoly.postman_collection.json` atau Swagger lokal untuk inspeksi kontrak API.
