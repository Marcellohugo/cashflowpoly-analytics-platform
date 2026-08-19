# Panduan Alur dan Hak Akses
## Cashflowpoly Analytics Platform

### Informasi Dokumen
- **Nama Dokumen**: Panduan Alur dan Hak Akses
- **Versi**: 1.1
- **Tanggal**: 20 Juni 2026
- **Penyusun**: Marco Marcello Hugo

---

## 1. Tujuan
Dokumen ini disusun sebagai rujukan logis untuk memahami alur kerja pengguna (Instruktur & Player), istilah identitas yang digunakan sistem, aturan validasi *ruleset*, batasan hak akses data pemain (*Player scope*), serta hubungan otorisasi di dalam sistem.

Dokumen ini menjadi penghubung antara:
- Kontrak Teknis API: [02-02-rancangan-kontrak-api-dan-event.md](../02-Perancangan/02-02-rancangan-kontrak-api-dan-event.md)
- Struktur Skema DB: [02-01-rancangan-database-dan-model-data.md](../02-Perancangan/02-01-rancangan-database-dan-model-data.md)
- Desain Antarmuka MVC: [02-04-rancangan-antarmuka-dan-viewmodel-mvc.md](../02-Perancangan/02-04-rancangan-antarmuka-dan-viewmodel-mvc.md)

---

## 2. Istilah Identitas Sistem

Sistem menggunakan terminologi identitas berikut untuk membedakan pengguna aplikasi dan peserta sesi permainan:

| Istilah | Dipakai di | Penjelasan |
|---|---|---|
| `user_id` | API, Database, JWT Claim | ID akun unik pada tabel `app_users`. Berlaku untuk akun dengan role `INSTRUCTOR` maupun `PLAYER`. |
| `instructor_user_id` | Tabel `sessions`, `rulesets` | Merujuk pada `user_id` milik Instruktur yang mengelola dan memiliki sesi atau ruleset tersebut. |
| `session_participant_id` | Database | ID unik peserta sesi pada tabel `session_participants` yang menghubungkan sesi dengan akun `PLAYER`. |
| `session_player_id` | DTO API, Proyeksi State | Alias kontrak untuk peserta sesi, nilainya merujuk langsung ke `session_participants.session_participant_id`. |
| `player_order_no` | API, DB, UI | Nomor urut giliran pemain di dalam satu sesi permainan. |

> [!NOTE]
> Sistem tidak memiliki tabel profil pemain (`players`) terpisah. Semua data pemain direpresentasikan oleh akun `app_users` dengan peran (*role*) `PLAYER`.

---

## 3. Alur Utama Pengguna (User Journeys)

### 3.1 Pengguna Publik (Public / Guest)
1. Pengguna membuka Web Analitika dasbor atau Game Client.
2. Player dapat mendaftarkan akun baru melalui `/auth/register`; akun Instruktur dibuat melalui bootstrap/admin. Keduanya masuk melalui `/auth/login`.
3. Setelah masuk, sistem menerbitkan JWT token yang berisi informasi `user_id`, `role`, dan data sesi login.

### 3.2 Alur Kerja Instruktur (`INSTRUCTOR`)
1. **Masuk**: Instruktur masuk ke sistem dasbor atau Game Client.
2. **Setup Aturan**: Instruktur membuat atau memperbarui ruleset, lalu mengaktifkan versi ruleset tertentu ke status `ACTIVE`.
3. **Setup Sesi**: Instruktur membuat sesi permainan baru menggunakan Klien Game/IDN dengan mengunci `ruleset_version_id` yang sedang aktif.
4. **Pendaftaran Peserta**: Instruktur mendaftarkan akun-akun `PLAYER` (`user_id`) ke dalam sesi permainan.
5. **Jalankan Sesi**: Instruktur memulai sesi (`STARTED`), memasukkan data keputusan giliran Player, dan mengakhiri sesi (`ENDED`) setelah selesai melalui Klien Game/IDN.
6. **Evaluasi**: Instruktur memantau dasbor analitika sesi, data audit, dan statistik kesehatan API.

### 3.3 Alur Kerja Pemain (`PLAYER`)
1. **Masuk**: Player masuk ke sistem dasbor Web Analitika.
2. **Lihat Sesi**: Player melihat daftar sesi permainan aktif maupun historis yang mencantumkan nama akun mereka.
3. **Analisis Performa**: Player membuka detail sesi dan performa grafik individu mereka sendiri untuk melihat histori transaksi dan capaian misi finansial.
4. Player **tidak memiliki izin** untuk melakukan modifikasi data sesi, memicu kalkulasi ulang metrik, ataupun membaca log audit sistem.

---

## 4. Kebijakan Otorisasi & Hak Akses (Role-Based Access Control)

Berikut ringkasan otorisasi akses layanan berdasarkan peran pengguna:

*   **Autentikasi & Registrasi**: Login dan registrasi Player dapat diakses publik; registrasi publik Instruktur ditolak.
*   **Modifikasi Sesi & Ruleset (Create/Update/Delete/Activate)**: Hanya diizinkan untuk peran `INSTRUCTOR`. Player hanya memiliki hak baca (*read-only*).
*   **Pengiriman Event Permainan**:
    *   `INSTRUCTOR` dapat mengirim event untuk sesi mana pun yang mereka buat.
    *   `PLAYER` hanya dapat mengirim event yang merujuk pada `user_id` milik dirinya sendiri pada sesi permainan aktif di mana ia terdaftar sebagai peserta.
*   **Pembacaan Metrik Analitika**:
    *   `INSTRUCTOR` dapat membaca analitika seluruh peserta sesi.
    *   `PLAYER` dibatasi hanya dapat membaca grafik performa dan histori transaksi miliknya sendiri (*Player Data Scope*).
*   **Audit Keamanan & Metrik Observabilitas**: Hanya dapat diakses oleh peran `INSTRUCTOR` atau administrator sistem.

> [!TIP]
> Detail pemetaan endpoint API secara teknis beserta kode respons HTTP yang dikembalikan dapat dilihat pada dokumen [02-02-rancangan-kontrak-api-dan-event.md](../02-Perancangan/02-02-rancangan-kontrak-api-dan-event.md).

---

## 5. Aturan Mutasi Ruleset & Validasi Sesi

1. **Aturan Mutability**: Ruleset bawaan (default) berstatus *read-only*. Ruleset milik Instruktur dapat diubah gilirannya (versi baru) selama ruleset tersebut belum dikunci oleh sesi permainan yang berjalan.
2. **Aturan Penguncian Sesi**: Begitu sesi dibuat, sesi akan mengunci satu `ruleset_version_id` secara permanen. Versi ruleset pada sesi tersebut tidak dapat diubah lagi untuk menjamin keabsahan data permainan.
3. **Aturan Penghapusan**:
   - Versi ruleset dengan status `ACTIVE` tidak boleh dihapus.
   - Ruleset yang sudah pernah digunakan oleh sesi permainan tidak dapat dihapus (`is_locked_by_session = true`).
   - Versi ruleset yang memiliki korelasi data dengan tabel event tidak boleh dihapus.
