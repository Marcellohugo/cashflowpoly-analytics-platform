# Kontrak Integrasi IDN, Keamanan, dan NFR Operasional
## Sistem Informasi Dasbor Analitika & Manajemen Ruleset Cashflowpoly

### Dokumen
- Nama dokumen: Kontrak Integrasi IDN, Keamanan, dan NFR Operasional
- Versi: 1.0
- Tanggal: 8 Februari 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan
Dokumen ini menetapkan kontrak integrasi API antara aplikasi IDN dan backend Cashflowpoly agar implementasi lintas tim tidak ambigu. Dokumen ini melengkapi:
- `docs/01-Spesifikasi/01-02-spesifikasi-event-dan-kontrak-api.md`
- `docs/01-Spesifikasi/01-03-spesifikasi-ruleset-dan-validasi.md`

---

## 2. Versi API dan Kompatibilitas
### 2.1 Versi kontrak aktif
- Versi kontrak API aktif: `v1`.
- Prefix route aktif: `/api/...` (tanpa prefix `/v1`).

### 2.2 Kebijakan kompatibilitas
1. Perubahan non-breaking boleh menambah field response baru.
2. Perubahan breaking wajib diumumkan sebagai kontrak mayor baru dan dicatat di dokumen ini.
3. Klien IDN wajib mengabaikan field response yang tidak dikenal.

---

## 3. Autentikasi dan Otorisasi
### 3.1 Endpoint autentikasi
1. `POST /api/auth/login`
2. `POST /api/auth/register`

Response login minimal:
```json
{
  "user_id": "uuid",
  "username": "string",
  "role": "INSTRUCTOR",
  "access_token": "jwt",
  "expires_at": "2026-02-08T12:00:00Z"
}
```

### 3.2 Header autentikasi
- Header wajib untuk endpoint terproteksi:
  - `Authorization: Bearer <access_token>`

### 3.3 Role matrix endpoint
| Endpoint | INSTRUCTOR | PLAYER |
|---|---:|---:|
| `POST /api/sessions` | Ya | Tidak |
| `POST /api/sessions/{id}/start` | Ya | Tidak |
| `POST /api/sessions/{id}/end` | Ya | Tidak |
| `POST /api/sessions/{id}/ruleset/activate` | Ya | Tidak |
| `POST /api/rulesets` | Ya | Tidak |
| `PUT /api/rulesets/{id}` | Ya | Tidak |
| `POST /api/rulesets/{id}/archive` | Ya | Tidak |
| `DELETE /api/rulesets/{id}` | Ya | Tidak |
| `POST /api/players` | Ya | Tidak |
| `POST /api/sessions/{id}/players` | Ya | Tidak |
| `GET /api/sessions` | Ya | Ya |
| `GET /api/analytics/...` | Ya | Ya |
| `POST /api/events` | Ya | Ya |
| `POST /api/events/batch` | Ya | Ya |

Catatan:
- Peran PLAYER hanya boleh mengakses data yang diizinkan oleh UI dan kebijakan bisnis.
- Endpoint publik tanpa token hanya `login` dan `register`.

---

## 4. Idempotency, Retry, dan Timeout
### 4.1 Idempotency event
1. Kunci idempotensi event adalah kombinasi `session_id + event_id`.
2. `event_id` harus UUID stabil untuk event yang sama pada retry.
3. `client_request_id` opsional untuk tracing request lintas sistem.

### 4.2 Aturan retry klien IDN
1. Boleh retry pada:
   - timeout jaringan,
   - HTTP `429`, `500`, `502`, `503`, `504`.
2. Jangan retry pada:
   - HTTP `400`, `401`, `403`, `404`, `409`, `422`.
3. Strategi retry:
   - exponential backoff: 1s, 2s, 4s,
   - maksimum 3 kali retry.

### 4.3 Timeout
1. Timeout HTTP klien IDN ke API: 10 detik/request.
2. Jika timeout, klien wajib mengirim ulang request dengan `event_id` yang sama.

---

## 5. Standar Error
Semua error menggunakan format:
```json
{
  "error_code": "STRING",
  "message": "STRING",
  "details": [
    { "field": "payload.amount", "issue": "OUT_OF_RANGE" }
  ],
  "trace_id": "STRING"
}
```

Kode status domain utama:
- `409`: duplikasi idempotensi.
- `422`: pelanggaran aturan domain.

---

## 6. Lifecycle Ruleset
### 6.1 Lifecycle level ruleset
1. `ACTIVE_RECORD` (ruleset dapat dipakai)
2. `ARCHIVED` (`is_archived = true`)
3. `DELETED` (hard delete, hanya jika belum pernah dipakai sesi)

### 6.2 Lifecycle level ruleset version
1. `DRAFT` (opsional, belum dipakai)
2. `ACTIVE` (dipakai saat ini)
3. `RETIRED` (versi lama, tidak aktif)

### 6.3 Aturan penghapusan
1. Ruleset tidak boleh dihapus jika sudah muncul pada `session_ruleset_activations`.
2. Ruleset yang sudah dipakai hanya boleh diarsipkan.

---

## 7. Kontrak Data Log dari IDN
### 7.1 Event wajib
Klien IDN wajib mengirim:
1. metadata urutan event (`event_id`, `session_id`, `sequence_number`, `turn_number`, `timestamp`)
2. identitas aktor (`actor_type`, `player_id`)
3. konteks ruleset (`ruleset_version_id`)
4. payload domain event sesuai `action_type`.

### 7.2 Domain learning
Domain learning finansial/komputasional/ekologis disimpan sebagai:
1. metrik turunan di snapshot `gameplay.derived.metrics`, atau
2. payload event domain khusus jika modul IDN sudah mengirim.

Catatan:
- Kontrak minimum backend saat ini sudah siap menyimpan payload mentah event (`jsonb`) untuk ekspansi domain selanjutnya.

---

## 8. NFR Operasional
### 8.1 Performa
1. P95 ingest event tunggal <= 500 ms.
2. P95 analitika sesi <= 1500 ms (<= 2000 event per sesi).

### 8.2 Reliabilitas
1. Tidak ada efek ganda pada retry event duplikat.
2. Konsistensi data dijaga oleh transaksi pada operasi multi-tabel.

### 8.3 Observability
1. Log terstruktur minimum: `trace_id`, `path`, `status_code`, `duration_ms`.
2. Audit validasi event masuk ke `validation_logs`.

### 8.4 Backup dan restore
1. Backup database harian.
2. Uji restore minimal bulanan.

---

## 9. Acceptance Criteria Integrasi IDN
Sistem dianggap siap integrasi IDN jika:
1. klien IDN dapat login, mendapatkan token, dan mengakses endpoint terproteksi sesuai role,
2. retry event dengan `event_id` sama tidak menggandakan data,
3. seluruh endpoint utama mengembalikan format error standar saat gagal,
4. data analitika sesi dan pemain dapat diambil konsisten setelah ingest event.
