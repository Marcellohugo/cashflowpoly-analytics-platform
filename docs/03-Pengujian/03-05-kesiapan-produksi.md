# Verifikasi kesiapan produksi

Tanggal pemeriksaan: 13 September 2026. Hasil: pemeriksaan lokal lulus; deployment ke server tidak dijalankan pada pekerjaan ini.

## Perubahan yang diselesaikan

- Seed 2 tetap aktif pada konfigurasi production sesuai permintaan pengguna. Migrasi menjalankan seed, kemudian deployment menjalankan rekalkulasi analitik. Restart runtime production hanya memverifikasi migrasi.
- Batas waktu SQL untuk baseline, migrasi, dan seed dinaikkan menjadi 300 detik. Permintaan API biasa tetap memakai batas waktunya sendiri. Seed 2 yang diperluas sebelumnya melewati batas 30 detik.
- Penandaan akun demo menggunakan UUID yang ditetapkan Seed 2; pembaruan tambahan berdasarkan nama pengguna lama dihapus.
- Image API/UI menggunakan .NET SDK 10.0.401 dan ASP.NET Core 10.0.12. Paket Microsoft terkait diperbarui ke 10.0.12. Build aset UI menggunakan Node 24.21.0 LTS. Sumber versi: [metadata resmi .NET](https://builds.dotnet.microsoft.com/dotnet/release-metadata/10.0/releases.json) dan [rilis resmi Node](https://nodejs.org/dist/index.json).
- Port internal produksi, alamat API yang dipakai UI, dan health check diselaraskan dengan upstream Nginx. API/UI/database tetap tidak memublikasikan port ke host.
- Health check Cloudflare Tunnel memakai pemeriksaan `tunnel ready`, bukan pemeriksaan versi executable. Endpoint metrik hanya mendengarkan loopback container.
- Retensi worktree melindungi direktori rilis aktif dan sebelumnya berdasarkan jalur absolut, termasuk saat rilis lama dipasang ulang. Smoke test HTTPS melalui Nginx meneruskan skema yang benar.
- Pemeriksa konfigurasi mendukung jalur absolut, nilai dotenv berpetik, validasi hostname/token, serta nilai boolean Seed 2. Laporannya membedakan validasi lokal dari konektivitas server.
- Header kolom tabel diratakan ke tengah; kolom Urutan Pemain dihapus dari formulir/ringkasan ruleset. Aturan urutan giliran dalam data permainan tetap berlaku.
- README, panduan pengguna, kontrak registrasi, peta halaman Statistik Pemain, dan koleksi Postman diselaraskan dengan implementasi.

## Bukti pemeriksaan

| Pemeriksaan | Hasil |
|---|---|
| Build Release `/warnaserror` | Lulus, 0 warning dan 0 error |
| Unit, integrasi PostgreSQL, kontrak API dan performa setelah konfigurasi Seed 2 akhir | 369 lulus |
| Pengujian UI .NET | 401 lulus |
| E2E Chromium desktop dan ponsel | 64 lulus |
| Performa 100 akun, 20 sesi, 20 klien bersamaan | Lulus: P95 ingest ≤500 ms dan analitika ≤1.500 ms |
| Audit NuGet termasuk dependensi transitif | Tidak ditemukan paket rentan |
| Audit npm UI dan E2E | 0 kerentanan |
| Validasi konfigurasi produksi lokal dan Compose | Lulus |
| Sintaks Bash, PowerShell, JavaScript frontend | Lulus |
| Image produksi API dan UI | Berhasil dibangun, pengguna runtime `app` |
| Smoke image production pada PostgreSQL kosong sementara | Lulus: migrasi dua kali, startup, UI publik/aset, API terproteksi, Swagger nonaktif, rekalkulasi |
| Hasil Seed 2 pada image production | 8 akun, 16 sesi, 22 ruleset; 10 ruleset per instruktur dan 2 bawaan; tidak mengganda setelah migrasi ulang |
| Snapshot hasil rekalkulasi production | 64 `gameplay.raw.variables` dan 64 `gameplay.derived.metrics` |
| Header tabel dan ringkasan ruleset pada lebar 1081/390 px | 126 header per ukuran layar rata tengah; Urutan Pemain tidak tampil |
| `git diff --check` | Lulus |

Gerbang lengkap dijalankan tanpa parameter skip. Setelah penyesuaian terakhir agar Seed 2 tetap aktif di production, seluruh pengujian .NET, konfigurasi, dan smoke image produksi diulang. Perubahan CSS terakhir juga diperiksa langsung pada desktop/ponsel dan image UI dibangun ulang.

Log lokal tersedia pada `artifacts/production-release-final.log`, `artifacts/production-seed-final-tests.log`, `artifacts/production-seed-image-smoke.log`, dan `artifacts/production-ui-check.log`. Artefak hasil pengujian tidak masuk image Docker.

## Langkah rilis

Jalankan `scripts/Test-ProductionReadiness.ps1` terhadap environment tujuan, lalu gunakan runbook [deployment produksi](../00-Panduan/00-04-panduan-deployment-produksi.md). Seed 2 memakai `DATABASE_MIGRATIONS_SEED_SIMULATION=true`; rekalkulasi wajib mengikuti migrasi. Verifikasi domain, TLS, tunnel aktif, serta login pada server setelah deployment.

Kebijakan proyek tetap memakai deployment manual tanpa GitHub Actions. Sesuai keputusan proyek yang sudah tercatat, rollback hanya memulihkan image aplikasi; backup dan rollback database belum tersedia.
