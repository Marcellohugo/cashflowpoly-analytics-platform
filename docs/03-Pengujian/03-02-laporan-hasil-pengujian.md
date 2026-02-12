# Laporan Hasil Pengujian
## Sistem Informasi Dasbor Analitika dan Manajemen Ruleset Cashflowpoly

### Dokumen
- Nama dokumen: Laporan Hasil Pengujian
- Versi: 1.2
- Tanggal: 12 Februari 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan dan Cakupan
Dokumen ini merekap hasil pengujian implementasi terbaru pada tanggal 12 Februari 2026.

Cakupan laporan ini:
- verifikasi teknis otomatis (build, test, compose, smoke),
- verifikasi API black-box utama,
- verifikasi RBAC dan rate limiting,
- verifikasi UI MVC halaman inti.

---

## 2. Identitas Pengujian
- Lingkungan: Windows 11 Home, VS Code, .NET 10, Docker Desktop, PostgreSQL 16
- Tanggal pengujian: 12 Februari 2026
- Versi aplikasi (git commit): `187c89b`
- Penguji: Marco (eksekusi teknis melalui sesi Codex)
- DB: `cashflowpoly`
- URL API: `http://localhost:5041`
- URL Web: `http://localhost:5203`

---

## 3. Ringkasan Hasil Uji
| Jenis Uji | Cakupan | Status |
|---|---|---|
| Uji REST API (black-box) | Alur auth, ruleset, sessions, players, ingest event, analytics | PASS |
| Uji Integrasi | Alur end-to-end: login/register -> ruleset -> session -> player -> event -> analytics | PASS |
| Validasi UI MVC | Login UI + 6 halaman inti + akses Swagger API | PASS |
| Verifikasi keamanan API | RBAC (401/403), role boundary, fixed-window rate limit (429) | PASS |

Kriteria kelulusan tercapai untuk cakupan baseline otomatis: tidak ada kegagalan pada seluruh skrip verifikasi.

---

## 4. Detail Verifikasi Otomatis
| Pemeriksaan | Perintah | Status | Ringkasan Hasil |
|---|---|---|---|
| Build solution | `dotnet build Cashflowpoly.sln -c Debug` | PASS | 0 warning, 0 error |
| Test solution | N/A | N/A | Proyek unit test API dihapus dari solution |
| Compose run | `docker compose up -d --build` | PASS | service `db`, `api`, `ui` aktif |
| Smoke API end-to-end | `powershell -ExecutionPolicy Bypass -File scripts/smoke.ps1` | PASS | ruleset/session/player/event/analytics sukses |
| RBAC smoke | `powershell -ExecutionPolicy Bypass -File scripts/rbac-smoke.ps1` | PASS | 401/403/200/201 sesuai ekspektasi |
| Rate-limit smoke | `powershell -ExecutionPolicy Bypass -File scripts/rbac-smoke.ps1 -CheckRateLimit` | PASS | respons `429` terdeteksi |
| Web UI smoke | `powershell -ExecutionPolicy Bypass -File scripts/web-ui-smoke.ps1` | PASS | login + halaman utama + Swagger terverifikasi |

Tambahan cek endpoint analitika:
- `GET /api/v1/analytics/rulesets/{rulesetId}/summary` -> `200`
- `GET /api/v1/analytics/sessions/{sessionId}/transactions?playerId=...` -> `200`
- `GET /api/v1/analytics/sessions/{sessionId}/players/{playerId}/gameplay` -> `200`
- `POST /api/v1/analytics/sessions/{sessionId}/recompute` -> `200`

---

## 5. Rekap Hasil Uji Per Modul
| Kode Modul | Modul | Status | Bukti Ringkas |
|---|---|---|---|
| M1 | Manajemen Sesi | PASS | create/list/start sesi pada smoke + rbac smoke |
| M2 | Manajemen Ruleset | PASS | create/update/activate/detail ruleset |
| M3 | Ingest Event | PASS | `POST /api/v1/events` sukses (`201`) |
| M4 | Proyeksi Arus Kas | PASS | endpoint transaksi sesi/pemain terbaca (`200`) |
| M5 | Agregasi Metrik | PASS | analitika sesi mengembalikan ringkasan dan by-player |
| M6 | Analitika (Endpoint) | PASS | session analytics, ruleset summary, gameplay, recompute |
| M7 | UI MVC Dasbor | PASS | halaman Home/Sessions/Players/Rulesets/Analytics/Rulebook |
| M8 | Logging dan Error Handling | PASS (baseline) | log `request_completed` + `trace_id` tampil; rate limit `429` tervalidasi |

---

## 6. Temuan dan Risiko Residual
Temuan blocker: **tidak ada**.

Catatan residual:
1. Uji performa beban tinggi (volume event besar) belum dieksekusi pada sesi ini.
2. Bukti screenshot/sql lampiran formal belum dikurasi ke folder `docs/evidence/` pada laporan ini.

---

## 7. Kesimpulan
- Status akhir: **VALID (cakupan baseline otomatis)**
- Ringkasan:
  - build lulus,
  - verifikasi API/UI/RBAC/rate-limit lulus,
  - tidak ditemukan bug blocker pada jalur fitur inti.

Tindak lanjut yang direkomendasikan:
1. Tambahkan pengujian performa terukur (P95 endpoint analitika).
2. Lengkapi artefak bukti formal (screenshot UI, hasil query DB, export Postman) bila dibutuhkan untuk lampiran sidang.
