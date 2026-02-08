# Rencana Implementasi dan Struktur Solution .NET
## REST API + ASP.NET Core MVC (Razor Views) untuk Cashflowpoly

### Dokumen
- Nama dokumen: Rencana Implementasi dan Struktur Solution .NET
- Versi: 1.1
- Tanggal: 8 Februari 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan
Dokumen ini menetapkan struktur repository, pembagian proyek API/UI, urutan implementasi, dan artefak validasi agar pengerjaan terukur dan sesuai scope:
1. web analytics,
2. ruleset management,
3. backend API,
4. database.

---

## 2. Struktur Repository
```
cashflowpoly-analytics-platform/
  database/
    00_create_schema.sql
  docs/
    00-Panduan/
    01-Spesifikasi/
    02-Perancangan/
    03-Pengujian/
  postman/
  scripts/
    smoke.ps1
  src/
    Cashflowpoly.Api/
    Cashflowpoly.Ui/
  tests/
    Cashflowpoly.Api.Tests/
  docker-compose.yml
```

---

## 3. Struktur Solution dan Tanggung Jawab
### 3.1 `Cashflowpoly.Api`
Tanggung jawab:
1. endpoint autentikasi,
2. endpoint sesi, player, ruleset, event, analitika,
3. validasi domain ruleset/event,
4. idempotensi ingest event,
5. akses database PostgreSQL.

### 3.2 `Cashflowpoly.Ui`
Tanggung jawab:
1. MVC Controller + Razor Views,
2. alur login/register/logout,
3. dashboard analitika,
4. manajemen ruleset,
5. integrasi API via `HttpClient`.

---

## 4. Prinsip Arsitektur
1. UI tidak query DB langsung.
2. API menjadi satu-satunya pintu masuk data.
3. Endpoint terproteksi memakai token Bearer.
4. Otorisasi berbasis role (`INSTRUCTOR`, `PLAYER`).
5. Kontrak API versi `v1` dijaga kompatibel.

---

## 5. Urutan Implementasi
### Tahap A - Bootstrap
1. siapkan dua project (`Api`, `Ui`),
2. pastikan build dan run dasar berhasil,
3. aktifkan Swagger pada environment development.

### Tahap B - Database
1. jalankan `database/00_create_schema.sql`,
2. verifikasi tabel inti:
   - `sessions`,
   - `rulesets`, `ruleset_versions`,
   - `events`, `event_cashflow_projections`,
   - `metric_snapshots`,
   - `validation_logs`,
   - `app_users`.

### Tahap C - Auth dan RBAC
1. implementasi login/register,
2. keluarkan token Bearer,
3. proteksi endpoint sensitif berdasarkan role,
4. pastikan UI menyimpan token di session server-side.

### Tahap D - Sesi dan Ruleset
1. endpoint create/list/start/end session,
2. endpoint create/list/detail/archive/delete ruleset,
3. endpoint activate ruleset ke sesi,
4. guard domain: ruleset dipakai sesi tidak boleh dihapus.

### Tahap E - Ingest Event dan Validasi Domain
1. ingest `POST /api/events`,
2. validasi payload + urutan `sequence_number`,
3. idempotensi `(session_id, event_id)`,
4. tulis validation log untuk event invalid.

### Tahap F - Proyeksi dan Snapshot Metrik
1. bentuk proyeksi cashflow dari event,
2. hitung snapshot metrik session/player,
3. simpan ke `metric_snapshots`.

### Tahap G - Endpoint Analitika
1. analitika sesi: `GET /api/analytics/sessions/{sessionId}`,
2. histori transaksi: `GET /api/analytics/sessions/{sessionId}/transactions`,
3. gameplay snapshot: `GET /api/analytics/sessions/{sessionId}/players/{playerId}/gameplay`,
4. agregasi per ruleset: `GET /api/analytics/rulesets/{rulesetId}/summary`.

### Tahap H - UI Dashboard
1. login/register,
2. sessions list/details,
3. players details,
4. ruleset list/details/create,
5. activation ruleset,
6. halaman analytics dan rulebook.

### Tahap I - Pengujian dan Operasional
1. `dotnet build` API dan UI,
2. `dotnet test` API tests,
3. jalankan smoke script,
4. jalankan Postman collection,
5. jalankan `docker compose` untuk verifikasi integrasi.

---

## 6. Checklist Definition of Done
1. fitur lulus test fungsional pada dokumen pengujian,
2. kontrak API sesuai spesifikasi,
3. docs dan implementasi sinkron,
4. build/test lulus,
5. smoke test lulus,
6. tidak ada bug blocker terbuka untuk modul inti.
