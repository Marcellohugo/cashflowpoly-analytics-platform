# Panduan Deployment Produksi
## Cashflowpoly Analytics Platform

### Informasi Dokumen

- Nama dokumen: Panduan Deployment Produksi
- Versi: 2.1
- Tanggal: 13 September 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan dan batasan

Dokumen ini menjelaskan deployment manual Cashflowpoly ke VPS melalui Docker Compose dan Cloudflare Tunnel. Commit yang dipasang selalu commit terbaru `origin/prod`. VPS membangun image dari source, menjalankan migrasi maju beserta Seed 2 sesuai konfigurasi, menghitung ulang analitik, lalu melakukan health check.

Cloudflared dan Nginx memakai jaringan ingress khusus `172.30.254.0/29`. Nginx memakai alamat tetap `172.30.254.3` agar startup lebih dahulu tidak mengambil alamat tunnel. Alamat tunnel `172.30.254.2` harus sama dengan `set_real_ip_from` dan pemetaan proxy tepercaya pada konfigurasi Nginx. Hanya peer ini boleh meneruskan `CF-Connecting-IP` dan skema HTTPS. Nginx mengirim satu IP pengunjung yang sudah diverifikasi ke API/UI, sehingga kuota tidak tercampur antara semua pengguna tunnel. Jangan tambahkan layanan lain dengan kedua alamat tersebut. Jika subnet berbenturan dengan jaringan host, ubah subnet, kedua alamat layanan, dan kedua aturan kepercayaan Nginx bersamaan.

Pembatasan Nginx berlaku per IP: lokasi API umum memakai `50r/s` dengan `burst=100`, sedangkan lokasi khusus `/api/v1/auth/login` memakai `20r/m` dengan `burst=10`. Keduanya memakai `nodelay`: burst yang diizinkan diteruskan tanpa penundaan, kelebihan batas mendapat HTTP `429`; HTTP `503` tetap berarti maintenance. Ini lapisan tambahan di depan kuota API per kelompok/identitas akun (autentikasi 30, ingest 340, lainnya 400 request per *fixed window* satu menit; fallback identitas ke IP dan tanpa antrean). Beberapa akun pada IP yang sama tetap berbagi kuota Nginx. Jalankan `pwsh -File tests/deployment/Test-NginxClientIdentity.ps1` untuk menguji IP tepercaya, penolakan header palsu, dan kuota per pengunjung dalam container sementara tanpa jaringan.

Jalankan `pwsh -File tests/deployment/Test-IngressNetworking.ps1` untuk menguji startup Nginx sebelum tunnel pada jaringan Docker sementara. Tes memakai alamat dari Compose produksi pada subnet uji terpisah, lalu membersihkan container dan jaringan uji. Gerbang verifikasi rilis menjalankannya setelah regresi deployment.

UI menetapkan port HTTPS publik `443` untuk redirect permintaan HTTP. Header skema dari tunnel tepercaya diteruskan Nginx untuk halaman dan aset statis, sehingga permintaan HTTPS tidak diarahkan ulang. Endpoint `/health` tetap dapat diperiksa melalui HTTP internal.

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
| `api` | .NET 10.0.12 | 5041 | Melalui `/api/` |
| `ui` | .NET 10.0.12 | 5203 | Melalui `/` |
| `nginx` | nginx-unprivileged 1.31.3 | 8080 | Melalui tunnel |
| `cloudflared` | 2026.8.1 | - | Koneksi keluar saja |

API, UI, dan Nginx berjalan sebagai pengguna non-root. Semua service memakai `no-new-privileges` dan log production dikirim ke journald.

## 3. Prasyarat

VPS membutuhkan Ubuntu 24.04, Git, Docker Engine, Docker Compose V2, `curl`, `flock`, `logger`, `realpath`, systemd, akses keluar ke GitHub dan Cloudflare, serta minimal ruang untuk dua rilis dan image terkait.

Repository deployment harus dapat menjalankan `git fetch origin prod` tanpa prompt interaktif. Domain produksi adalah `https://narafin.org`.

## 4. Konfigurasi awal VPS

Pada VPS yang sudah berjalan, repository berada di `/root/cashflowpoly-analytics-platform`, environment di `config/env/.env.prod` dalam repository tersebut, dan rilis aktif di `/opt/cashflowpoly/current`. Skrip mengikuti lokasi checkout tempat skrip berada; `REPOSITORY_DIR` dan `ENV_FILE` masih dapat diatur bila lokasi berbeda.

Untuk VPS baru saja, jalankan sekali sebagai `root`:

```bash
install -d /opt/cashflowpoly/releases
git clone <repository-url> /root/cashflowpoly-analytics-platform
cp /root/cashflowpoly-analytics-platform/config/env/.env.prod.example /root/cashflowpoly-analytics-platform/config/env/.env.prod
chmod 600 /root/cashflowpoly-analytics-platform/config/env/.env.prod
```

Isi `/root/cashflowpoly-analytics-platform/config/env/.env.prod`:

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

Perintah tersebut wajib selesai tanpa parameter `-SkipBrowser`, `-SkipPerformance`, atau `-SkipDockerBuild`. Pemeriksaannya meliputi build/test .NET, kontrak OpenAPI, audit dependency, konsistensi dokumentasi, performa, migrasi dan Seed 2 development, E2E Chromium desktop/ponsel, validasi Compose, build image production, dan regresi alur deployment terisolasi. Kegagalan satu langkah menghentikan proses dan berarti commit belum siap dirilis.

Setelah seluruh pemeriksaan lulus:

```powershell
git switch prod
git merge --ff-only codex/project-alignment
git push origin prod
```

Jangan melakukan deployment dari perubahan lokal yang belum ada pada `origin/prod`.

Uji regresi alur deployment memakai container disposable tanpa jaringan; Git, Docker, HTTP, dan systemd di dalamnya diganti stub. Uji ini tidak mengakses daemon Docker atau server produksi:

```powershell
docker run --rm --network none --user 0 --entrypoint bash `
  --mount "type=bind,source=$($PWD.Path),target=/repo,readonly" `
  cashflowpoly-api:release-gate /repo/tests/deployment/deploy-production.test.sh
```

Gerbang verifikasi membangun image `cashflowpoly-api:release-gate` lalu menjalankan uji ini otomatis. Cakupannya: deployment ulang SHA aktif beserta retensi rilis rollback, dependensi health Nginx/tunnel, rilis baru dengan marker usang, kegagalan cleanup, dan rollback ketika smoke test gagal.

## 7. Menjalankan deployment

Masuk ke VPS sebagai `root`, lalu:

```bash
git -C /root/cashflowpoly-analytics-platform fetch origin prod
git -C /root/cashflowpoly-analytics-platform status --short
git -C /root/cashflowpoly-analytics-platform checkout --detach origin/prod
/root/cashflowpoly-analytics-platform/scripts/deploy-production.sh
```

Pastikan `git status --short` bersih sebelum checkout. Simpan perubahan lokal yang belum dicatat terlebih dahulu; jangan memakai `checkout --force`. File `.env.prod` diabaikan Git dan tetap tersedia antar-rilis.

Skrip deployment melakukan langkah berikut:

1. Mengambil lock `/var/lock/cashflowpoly-deploy.lock` agar tidak ada dua deployment bersamaan.
2. Mengambil SHA terbaru `origin/prod` dan membuat worktree rilis per SHA.
3. Memvalidasi Docker Compose dan environment.
4. Memasang retensi journald 30 hari dengan batas disk.
5. Menarik image eksternal yang dipin dan membangun API/UI berurutan.
6. Menampilkan halaman pemeliharaan singkat bila ada rilis sebelumnya.
7. Menyalakan PostgreSQL dan menunggu status sehat.
8. Menjalankan API `--migrate-only`.
9. Menjalankan Seed 2 idempoten dengan `DATABASE_MIGRATIONS_SEED_SIMULATION=true` (default produksi), lalu melanjutkan rekalkulasi.
10. Menjalankan `--recalculate-analytics` untuk seluruh sesi.
11. Menyalakan API, UI, dan Nginx baru.
12. Menunggu API/UI sehat, menghapus penanda maintenance pada rilis kandidat (termasuk saat SHA yang sama di-deploy ulang), dan menunggu Nginx sehat. Setelah itu tunnel dinyalakan; skrip memeriksa tunnel, `/health`, dan `/privacy`. Pemisahan ini mencegah Compose menunggu Nginx saat maintenance masih aktif.
13. Mengubah symlink `current` ke rilis yang sudah sehat.
14. Mempertahankan rilis aktif dan rilis sebelumnya; deployment ulang SHA aktif melewati cleanup agar rilis rollback yang tersimpan tetap tersedia. Kegagalan pembersihan hanya memberi peringatan dan tidak mengembalikan image lama.

## 8. Verifikasi setelah deployment

```bash
export RELEASE_SHA=$(basename "$(readlink -f /opt/cashflowpoly/current)")
docker compose --project-name cashflowpoly-analytics-platform \
  --env-file /root/cashflowpoly-analytics-platform/config/env/.env.prod \
  -f /opt/cashflowpoly/current/infra/docker/docker-compose.yml \
  -f /opt/cashflowpoly/current/infra/docker/docker-compose.prod.yml \
  --profile tunnel ps

curl --fail https://narafin.org/health
curl --fail https://narafin.org/privacy
curl --fail https://narafin.org/terms
```

Lakukan smoke test login Instruktur dan Player, termasuk akun demo Seed 2, daftar sesi, setup, event, analitik pemain, buku aturan, Privasi, dan Ketentuan. Permintaan publik `https://narafin.org/metrics` harus menghasilkan `404`.

## 9. Rollback dan kegagalan

Jika build, migrasi, container health, atau smoke test gagal, trap pada skrip menjalankan kembali image SHA sebelumnya dan mengarahkan `current` ke rilis tersebut setelah container berhasil dinyalakan. Database tidak diturunkan. Kegagalan cleanup setelah aktivasi tidak memicu rollback; periksa peringatan dan ulangi cleanup pada deployment berikutnya. Karena itu seluruh migrasi wajib memakai pola expand/contract sehingga aplikasi versi sebelumnya tetap dapat membaca schema yang sudah maju.

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
/root/cashflowpoly-analytics-platform/scripts/deploy-production.sh

# Status service
export RELEASE_SHA=$(basename "$(readlink -f /opt/cashflowpoly/current)")
docker compose --project-name cashflowpoly-analytics-platform \
  --env-file /root/cashflowpoly-analytics-platform/config/env/.env.prod \
  -f /opt/cashflowpoly/current/infra/docker/docker-compose.yml \
  -f /opt/cashflowpoly/current/infra/docker/docker-compose.prod.yml \
  --profile tunnel ps

# Log aplikasi
journalctl CONTAINER_NAME=cashflowpoly-api --since today

# Rilis aktif
readlink -f /opt/cashflowpoly/current
```

Swagger hanya aktif pada environment Development. Gunakan `postman/Cashflowpoly.postman_collection.json` atau Swagger lokal untuk inspeksi kontrak API.
