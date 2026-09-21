# API undo event

Status: **diimplementasikan**, memerlukan migrasi `V013__event_undo.sql`. Kontrak API umum ada di [kontrak REST](02-02-kontrak-rest-api-dan-event-permainan.md).

## Perilaku

Batasi undo pada **aksi gameplay terakhir dalam satu sesi** yang masih `STARTED`. Satu aksi berarti event utama beserta semua efek turunannya (saldo, inventori, misi, risiko, proyeksi, dan metrik). Event setup otomatis tidak boleh di-undo. Untuk kesalahan setup, gunakan revisi setup sebelum `/start`.

Backend menyimpan snapshot sebelum event diterapkan. Undo memulihkan saldo, inventori, kebutuhan, emas, tabungan/tujuan finansial, misi beserta bonusnya, pinjaman, asuransi, donasi, tie-breaker, posisi kartu, slot aksi, risiko, narasi, efek aturan, giliran/hari, dan checkpoint proyeksi. Undo berikutnya dapat membatalkan event efektif sebelumnya. Data gameplay kembali ke kondisi sebelumnya; versi, sequence, audit, dan waktu aktivitas server tetap maju.

## Kontrak

`POST /api/v1/sessions/{sessionId}/events/{eventId}/undo`

JWT wajib; hanya instruktur pemilik sesi. IDN mengirim:

```json
{
  "client_request_id": "undo-uuid-dari-idn",
  "expected_state_version": 42,
  "reason": "Kartu yang dipilih keliru"
}
```

Respons pertama `201 Created`; retry identik `200 OK` dengan hasil operasi yang sama:

```json
{
  "undo_id": "uuid-operasi-undo",
  "event_id": "uuid-event-asli",
  "status": "UNDONE",
  "state_version": 43,
  "next_sequence_number": 18
}
```

`state_version` selalu naik, termasuk ketika undo. Sequence, ID event, dan `client_request_id` event yang dibatalkan tetap dipesan dalam lingkup sesi; event pengganti memakai identitas baru dan nomor berikutnya dari `/state`. Kunci idempotensi undo berlaku per instruktur. Retry dengan `client_request_id` sama tetapi target/payload berbeda menghasilkan `409 CLIENT_REQUEST_ID_CONFLICT`. Retry identik tetap mengembalikan receipt semula setelah sesi kosong terhapus.

Respons lain: `400` untuk body tidak valid, `403` untuk role selain INSTRUCTOR, `404` untuk sesi/event tidak ditemukan atau bukan milik instruktur, `409 STATE_VERSION_CONFLICT` jika state berubah, `409 EVENT_NOT_LAST` jika ada aksi sesudah target, `422 SESSION_NOT_STARTED` untuk sesi bukan STARTED, dan `422 SETUP_UNDO_NOT_ALLOWED` untuk target setup. Jika target sudah dibatalkan memakai request ID lain, respons `409 EVENT_ALREADY_UNDONE`. `client_request_id` dan `reason` wajib nonkosong, maksimal 120 dan 500 karakter.

## Transaksi dan pemulihan state

1. Ambil advisory lock sesi yang sama dengan ingest dan `/end`, lalu row lock sesi. Periksa idempotensi lebih dahulu untuk retry; setelah itu periksa status, versi, dan posisi target dalam urutan aksi efektif.
2. Arsipkan event asli, arus kas, dan referensi aset ke `event_undos`, lalu keluarkan target dari tabel event efektif. Pulihkan 16 tabel proyeksi dari snapshot sebelum aktivitas. Seluruh perubahan, peningkatan `state_version`, dan penyegaran aktivitas berada dalam satu transaksi; kegagalan pemulihan membatalkan semuanya.
3. Batalkan/invalidate snapshot metrik yang terdampak; pastikan saldo, inventori, checkpoint, misi, pinjaman, asuransi, risiko, dan timeline memakai hasil yang sama. Audit tetap menampilkan event asli beserta informasi pembatalannya.
4. IDN membaca `/state` setelah undo untuk memperoleh state terbaru. Retry undo yang sama tetap mengembalikan hasil operasi semula; `/state` menunjukkan state saat ini apabila sudah ada aksi berikutnya.

Snapshot penuh mengembalikan hasil yang persis sama tanpa mengulang keputusan acak atau menggunakan `rebuild_session_projection()` yang dinonaktifkan. Recompute metrik memakai lock sesi yang sama sehingga tidak dapat menyimpan hasil sebelum undo setelah transaksi undo selesai. Ukuran snapshot bertambah sesuai jumlah event dan state sesi; beralih ke before-image per baris bila volume memerlukannya. Tabel proyeksi baru harus ditambahkan ke whitelist capture/restore dan pengujian kesetaraan state.

**Batas kompatibilitas:** event yang diterima sebelum fitur snapshot aktif ditolak dengan `422 UNDO_SNAPSHOT_UNAVAILABLE`; backend tidak menebak state historis. Event baru pada sesi lama tetap dapat di-undo.

Audit permanen bersifat append-only dan dapat dibaca instruktur pemilik melalui `GET /api/v1/sessions/{sessionId}/event-undos?limit=50&cursor=...`. Respons berisi `items`, `next_cursor`, `has_more`; setiap item mencatat `undo_id`, `event_id`, instruktur, request ID, alasan, waktu server, `original_event`, `original_cashflows`, dan `original_asset_references`. Limit 1–100. Audit dan retry tetap tersedia setelah sesi kosong dihapus.

IDN harus menghentikan antrean lokal saat undo, membaca `/state` terbaru, mengirim request, lalu mengganti state lokal dengan `/state` hasil server. Segarkan timeline dan statistik. `GET /sessions/{sessionId}/events` mengembalikan `undone_sequence_numbers` agar klien membuang event tersebut dari cache; dashboard repository ini menerapkannya saat polling. Jika semua gameplay dibatalkan, sesi terhapus ketika `/end` atau heartbeat timeout, sedangkan audit tetap disimpan.

Undo terhadap event lama yang sudah memiliki aksi bergantung memerlukan pembatalan berantai atau replay dengan validasi seluruh aksi sesudahnya; tunda sampai ada kebutuhan produk yang jelas. Event kompensasi khusus bisa dipakai untuk koreksi administratif dengan aturan eksplisit, tetapi bukan pengganti undo umum.

## Cakupan verifikasi

- Retry identik, konflik request ID, instruktur lain, sesi berakhir, dan target setup.
- Dua undo bersamaan; undo melawan event baru, heartbeat timeout, dan `/end`.
- State sebelum aksi sama dengan state setelah aksi lalu undo, kecuali versi/audit/sequence yang tetap maju.
- Efek lintas pemain, pinjaman, risiko, inventori, misi, dan metrik pulih bersamaan; kegagalan pemulihan melakukan rollback seluruh transaksi.
- Event terakhir yang sudah dibatalkan diabaikan saat mencari aksi efektif terakhir berikutnya; riwayat audit tetap utuh.
