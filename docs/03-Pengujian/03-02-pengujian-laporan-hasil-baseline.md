# Laporan Hasil Pengujian
## Sistem Informasi Dasbor Analitika dan Manajemen Ruleset Cashflowpoly

### Dokumen
- Nama dokumen: Laporan Hasil Pengujian
- Versi: 2.1
- Tanggal: 13 Juli 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan dan Cakupan
Dokumen ini merekap pengujian lengkap baseline schema `3.0.7` tanggal 12 Juli 2026 dan verifikasi ulang suite otomatis pada clean HEAD tanggal 13 Juli 2026.

Cakupan laporan ini:
- verifikasi teknis otomatis (build, test, docker compose, uji asap, uji beban dasar),
- verifikasi API kotak-hitam utama,
- verifikasi RBAC dan rate limiting,
- verifikasi UI MVC halaman inti,
- verifikasi observability dan security audit endpoint.

---

## 2. Identitas Pengujian
- Lingkungan: Windows 11 Home, VS Code, .NET 10, Docker Desktop, PostgreSQL 16
- Tanggal pengujian lengkap: 12 Juli 2026
- Tanggal verifikasi ulang suite otomatis: 13 Juli 2026
- Branch: `dev`
- Commit aplikasi yang diuji: `7b2c296b0b5c710298bf05ae81e226d1b326527c`
- Status Git saat verifikasi ulang: clean, `HEAD` sama dengan `origin/dev`
- Penguji: Marco (eksekusi teknis melalui sesi Codex)
- DB: `cashflowpoly`
- URL API: `http://localhost:5041`
- URL Web: `http://localhost:5203`

---

## 3. Ringkasan Hasil Uji
| Jenis Uji | Cakupan | Status |
|---|---|---|
| Uji REST API (kotak-hitam) | Alur auth, ruleset, sessions, players, ingest event, analytics | PASS |
| Uji Integrasi | Baseline historis setup, privasi misi, risiko pending, asuransi, opsi darurat, holding emas, pinjaman multi-instance, dan Seed 2 | PASS |
| Validasi UI MVC | Login UI + 6 halaman inti + akses Swagger API | PASS |
| Verifikasi keamanan API | RBAC (401/403), role boundary, fixed-window rate limit (429) | PASS |
| Verifikasi observability + audit keamanan | Endpoint operasional metrics + security audit logs | PASS |
| Uji performa dasar | Skrip load test ingest + analytics sesuai target P95 | PASS |

Kriteria fitur inti tercapai dan seluruh suite otomatis berstatus hijau.

---

## 4. Detail Verifikasi Otomatis
| Pemeriksaan | Perintah | Status | Ringkasan Hasil |
|---|---|---|---|
| Build solusi | `dotnet build Cashflowpoly.sln --no-restore --nologo` | PASS | 0 warning, 0 error |
| Uji API penuh | `dotnet test tests/Cashflowpoly.Api.Tests/Cashflowpoly.Api.Tests.csproj --no-build --no-restore` | PASS | 290/290 test lulus |
| Uji UI penuh | `dotnet test tests/Cashflowpoly.Ui.Tests/Cashflowpoly.Ui.Tests.csproj --no-build --no-restore` | PASS | 103/103 test lulus |
| Uji solution penuh | `dotnet test Cashflowpoly.sln -c Release --nologo` | PASS | total 393/393 test lulus pada clean commit `7b2c296` |
| Uji Seed 2 | filter `ManualSimulationSeedIntegrationTests` | PASS | bootstrap dan replay dua mode lulus |
| Menjalankan compose watch | `docker compose --env-file config/env/.env.dev -f infra/docker/docker-compose.yml -f infra/docker/docker-compose.watch.yml up --build` | PASS | service `db`, `api`, `ui` healthy; API/UI health `200` |
| Uji asap API ujung-ke-ujung | Newman menjalankan Postman collection (alur end-to-end API) | PASS | 44 request dan 45/45 assertion lulus; event kerja, kebutuhan, bahan, pesanan, donasi, dan akhir giliran tersimpan |
| Uji asap RBAC | Postman collection (skenario RBAC) | PASS | 401/403/200/201 sesuai ekspektasi |
| Uji asap rate-limit | Burst request pada endpoint terproteksi (HTTP client) | PASS | respons `429` terdeteksi |
| Uji asap UI Web | Verifikasi browser (login + halaman utama + Swagger) | PASS | login + halaman utama + Swagger terverifikasi |
| Uji beban dasar | Skenario request berulang ke endpoint ingest dan analytics | PASS | Ingest P95 18.72 ms, Analytics P95 867.26 ms, error rate 0% |
| Observability API | `GET /api/v1/observability/metrics/summary` + `GET /metrics` | PASS | respons `200`, endpoint summary dan Prometheus tersedia |
| Security audit API | `GET /api/v1/security/audit-logs` | PASS | respons `200`, jejak event keamanan tersedia |

Catatan:
- Suite otomatis 393/393 diverifikasi ulang pada clean commit `7b2c296` tanggal 13 Juli 2026. Hasil compose, Postman, RBAC, rate limit, observability, dan performa merupakan bagian pengujian lengkap tanggal 12 Juli 2026.
- Verifikasi pada tabel di atas dijalankan secara lokal berbasis CLI, koleksi Postman, dan browser.
- Verifikasi build/test/compose dapat dijalankan ulang secara lokal melalui rangkaian perintah `dotnet restore`, `dotnet build`, `dotnet test`, dan `docker compose ... config`.

Tambahan cek endpoint analitika:
- `GET /api/v1/analytics/rulesets/{rulesetId}/summary` -> `200`
- `GET /api/v1/analytics/sessions/{sessionId}/transactions?userId=...` -> `200`
- `GET /api/v1/analytics/sessions/{sessionId}/players/{userId}/gameplay` -> `200`
- `POST /api/v1/analytics/sessions/{sessionId}/recompute` -> `200`

Tambahan cek endpoint observability & security:
- `GET /api/v1/observability/metrics/summary` -> `200`
- `GET /metrics` -> `200`
- `GET /api/v1/security/audit-logs?limit=20` -> `200`

---

## 5. Rekap Hasil Uji Per Modul
| Kode Modul | Modul | Status | Bukti Ringkas |
|---|---|---|---|
| M1 | Manajemen Sesi | PASS | create/list/start sesi pada uji asap + uji asap RBAC |
| M2 | Manajemen Ruleset | PASS | create/update/activate/detail ruleset |
| M3 | Ingest Event | PASS | `POST /api/v1/events` sukses (`201`) |
| M4 | Proyeksi Arus Kas | PASS | endpoint transaksi sesi/pemain terbaca (`200`) |
| M5 | Agregasi Metrik | PASS | analitika sesi mengembalikan ringkasan dan by-player |
| M6 | Analitika (Endpoint) | PASS | analitika sesi, ringkasan ruleset, gameplay, recompute |
| M7 | UI MVC Dasbor | PASS | halaman Home/Sessions/Players/Rulesets/Analytics/Rulebook |
| M8 | Logging dan Error Handling | PASS (dasar) | log `request_completed` + `trace_id` tampil; rate limit `429` tervalidasi |

---

## 6. Temuan dan Risiko Residual
Temuan blocker: **tidak ada**.

Catatan residual:
1. Uji beban jangka panjang (durasi > 30 menit, concurrency tinggi) belum dieksekusi pada sesi ini.
2. Evidence screenshot UI khusus sidang belum ditambahkan; artefak teknis masih perlu diarsipkan bersama laporan pengujian.

---

## 7. Kesimpulan
- Status akhir: **VALID; build, suite otomatis, Seed 2, dan Compose Watch lulus**
- Ringkasan:
  - build lulus,
  - verifikasi API/UI/RBAC/rate-limit lulus,
  - risiko pending, penyelesaian tunai/asuransi/darurat, holding emas, pinjaman multi-instance, dan Seed 2 lulus,
  - tidak ditemukan bug blocker pada jalur fitur inti.

Tindak lanjut yang direkomendasikan:
1. Tambahkan skenario stress test paralel berdurasi panjang untuk validasi stabilitas.
2. Lengkapi screenshot UI terkurasi bila dibutuhkan untuk lampiran sidang.
