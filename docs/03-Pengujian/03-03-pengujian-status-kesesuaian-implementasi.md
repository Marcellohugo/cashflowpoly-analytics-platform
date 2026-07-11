# Status Kesesuaian Implementasi
## Sistem Informasi Dasbor Analitika Cashflowpoly

### Dokumen
- Nama dokumen: Status Kesesuaian Implementasi
- Versi: 2.1
- Tanggal: 11 Juli 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan
Dokumen ini memetakan kesesuaian implementasi terhadap dokumen spesifikasi aktif, sekaligus menjadi daftar gap prioritas yang harus ditutup.

Acuan utama:
- `docs/01-Spesifikasi/01-01-spesifikasi-kebutuhan-sistem.md`
- `docs/02-Perancangan/02-02-rancangan-kontrak-api-dan-event.md`
- `docs/01-Spesifikasi/01-02-spesifikasi-ruleset-dan-validasi.md`
- `docs/01-Spesifikasi/01-03-spesifikasi-integrasi-dan-keamanan.md`

---

## 2. Ringkasan Kesesuaian per Area
| Area | Status | Catatan |
|---|---|---|
| Ingest event + validasi domain | Sesuai | Kebijakan slot terpusat, pasangan pesanan-risiko berbasis UUID, risiko pending, penyelesaian tunai/asuransi/darurat, donasi tunggal, holding emas, dan batas pinjaman aktif sudah dijaga API serta database. |
| Snapshot metrik dan analitika sesi/pemain | Sesuai | Endpoint analitika sesi, transaksi, gameplay snapshot tersedia; endpoint GET analitika bersifat read-only. |
| API lifecycle sesi/ruleset/player | Sesuai | Endpoint operasional tersedia untuk Klien Game/IDN: session lifecycle, aktivasi versi ruleset, player assignment, state read/write-disabled guard, dan guard ruleset terpakai. |
| UI dashboard (home/sessions/players/rulesets/rulebook/analytics) | Sesuai | Halaman inti tersedia dan terhubung API; Web Analitik bersifat baca-saja untuk gameplay event, tetapi Instruktur dapat mengelola ruleset dan aktivasi versi ruleset. Analitika utama ditampilkan pada detail sesi (`/sessions/{sessionId}`), sementara `/analytics` atau `/Analytics` dipertahankan sebagai route redirect. |
| Kontrak auth Bearer + RBAC | Sesuai | API Bearer-only untuk endpoint terproteksi, role check `INSTRUCTOR/PLAYER` ditegakkan server-side, registrasi publik tersedia untuk seluruh role. |
| Analitika agregasi grouped-by-ruleset | Sesuai | Endpoint `GET /api/v1/analytics/rulesets/{rulesetId}/summary` tersedia dan hasilnya ditampilkan pada halaman detail sesi (`/sessions/{sessionId}`). |
| NFR keamanan (rate limiting) | Sesuai | Rate limiting fixed-window diterapkan pada API dengan respons `429`; identitas klien tidak lagi mempercayai header spoofing secara langsung. |
| Dokumen uji + smoke + postman sinkron Bearer | Sesuai | Langkah smoke berbasis CLI dan Postman collection sudah menggunakan login + token Bearer. |
| Observability operasional | Sesuai | Endpoint ringkas `GET /api/v1/observability/metrics/summary` tersedia untuk role `INSTRUCTOR` dan menunjuk ke endpoint Prometheus `GET /metrics`; trace ID diseragamkan pada header/log. |
| Hardening keamanan produksi (baseline) | Sesuai | Rotasi JWT multi-key berbasis `kid` + window aktivasi/retire, dukungan secret env/file untuk integrasi vault/secret manager, dan audit log keamanan persisten tersedia. |
| Baseline uji performa | Sesuai | Baseline performa dapat diulang memakai skenario request berulang ke endpoint ingest event dan analytics sesi, lalu dicatat pada laporan pengujian. |

---

## 3. Daftar Gap Prioritas
Gap prioritas Mode Mahir sebelumnya telah ditutup pada baseline schema `3.0.4`.

Pekerjaan lanjutan yang masih direkomendasikan (non-blocker):
1. Selaraskan fixture integrasi `buku.cardQty=0` agar suite API penuh 279/279 hijau.
2. Integrasikan exporter tracing/metrics ke platform observability eksternal (Grafana/OTel collector) untuk environment produksi.
3. Aktifkan rotasi secret terjadwal melalui secret manager yang dipakai environment deploy (misalnya KV/Secrets Manager) dengan SOP operasional.
4. Tambahkan uji performa skenario beban paralel jangka panjang (durasi > 30 menit) untuk uji stabilitas.

---

## 4. Kriteria Siap Sidang
Implementasi dianggap siap ketika:
1. semua endpoint terproteksi sudah Bearer-only,
2. role check instruktur/player tervalidasi API (bukan UI saja),
3. endpoint grouped-by-ruleset tersedia dan tervalidasi,
4. docs, smoke, postman, dan implementasi konsisten,
5. hasil build/test/smoke/compose/load-test baseline lulus tanpa bug blocker,
6. observability operasional + audit log keamanan aktif dan dapat diverifikasi.



