# Rancangan Dashboard Analitika Berbasis ASP.NET Core MVC (Razor Views)
## Sistem Informasi Dasbor Analitika Cashflowpoly

### Dokumen
- Nama dokumen: Rancangan Dashboard Analitika (MVC)
- Versi: 1.1
- Tanggal: 8 Februari 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan Dokumen
Dokumen ini mendefinisikan rancangan antarmuka dashboard analitika berbasis ASP.NET Core MVC (Razor Views) untuk scope repository ini:
1. analitika performa pembelajaran dan misi,
2. manajemen ruleset untuk Instruktur dan referensi ruleset untuk Player,
3. integrasi API backend yang sudah disepakati.

Dokumen ini harus dibaca bersama:
- `docs/01-Spesifikasi/01-02-spesifikasi-event-dan-kontrak-api.md`
- `docs/01-Spesifikasi/01-03-spesifikasi-ruleset-dan-validasi.md`
- `docs/01-Spesifikasi/01-04-kontrak-integrasi-idn-dan-keamanan.md`
- `docs/02-Perancangan/02-03-definisi-metrik-dan-agregasi.md`

Jika ada konflik antar dokumen, prioritas kontrak mengikuti `01-04` lalu `01-02`.

---

## 2. Scope UI Dalam Repository Ini
UI yang dibangun pada proyek web ini mencakup:
1. dashboard analitika sesi,
2. detail analitika pemain,
3. analitika agregat per ruleset,
4. daftar, detail, create/edit/delete/activate ruleset untuk Instruktur,
5. visualisasi linimasa event dan performa Player,
6. akses Player ke data yang sesuai hak aksesnya.

UI tidak mencakup:
1. gameplay IDN app,
2. pembuatan sesi, pemilihan ruleset, penambahan Player, start/end sesi, dan input keputusan Player,
3. sistem narasi interaktif,
4. UI mission window di aplikasi seluler.

---

## 3. Peran Pengguna dan Hak Akses
### 3.1 Instruktur
Instruktur dapat:
1. login ke Web Analitik,
2. melihat sesi yang dibuat melalui Klien Game/IDN,
3. melihat semua analitika sesi/pemain,
4. mengelola ruleset dan aktivasi ruleset sesi,
5. melihat referensi dan agregasi analitika per ruleset.

### 3.2 Player
Player dapat:
1. login,
2. melihat ringkasan analitika yang diizinkan aplikasi,
3. melihat detail performa diri (jika endpoint dibatasi per player).

### 3.3 Aturan akses teknis
1. UI menyimpan `access_token` hasil login di session server-side.
2. Semua panggilan API terproteksi mengirim header `Authorization: Bearer <token>`.
3. Endpoint publik tanpa token hanya login/register.
4. Jika API mengembalikan `401`, UI mengarahkan pengguna ke halaman login.
5. Jika API mengembalikan `403`, UI menampilkan pesan "Akses ditolak" tanpa merender aksi terlarang.

---

## 4. Prinsip Desain UI
### 4.1 Struktur halaman
Hierarki utama:
Daftar sesi -> Detail sesi -> Detail pemain.

### 4.2 Konsistensi data
UI hanya membaca data dari endpoint analitika. UI tidak menghitung metrik bisnis di sisi klien.

### 4.3 Filter eksplisit
Filter utama:
1. sesi,
2. pemain,
3. ruleset,
4. rentang waktu/turn (jika endpoint mendukung).

### 4.4 Visualisasi terkontrol
Semua grafik garis harus:
1. dibungkus container dengan tinggi tetap,
2. tidak boleh auto-resize tanpa batas,
3. membatasi jumlah titik yang ditampilkan per grafik.

Tujuan aturan ini adalah mencegah bug grafik memanjang tanpa batas.

---

## 5. Struktur Navigasi dan Rute MVC
| Menu | Controller | Action | Route | Tujuan |
|---|---|---|---|---|
| Home | `HomeController` | `Index` | `/` | Ringkasan akses dan pintasan fitur utama. |
| Sessions | `SessionsController` | `Index` | `/sessions` | Daftar sesi. |
| Session Details | `SessionsController` | `Details` | `/sessions/{sessionId}` | Ringkasan analitika sesi dan daftar pemain. |
| Player Details | `PlayersController` | `Details` | `/sessions/{sessionId}/players/{playerId}` | Analitika detail pemain. |
| Components | `ComponentsController` | `Index` | `/components` | Katalog komponen default Pemula/Mahir. |
| Ruleset | `RulesetsController` | `Index` | `/rulesets` | Daftar ruleset dan aksi manajemen Instruktur. |
| Ruleset Details | `RulesetsController` | `Details` | `/rulesets/{rulesetId}` | Detail ruleset dan versi. |
| Ruleset Create/Edit | `RulesetsController` | `Create`/`Edit` | `/rulesets/create`, `/rulesets/{rulesetId}/edit` | Form create/edit ruleset Instruktur. |
| Session Analytics | `SessionsController` | `Details` | `/sessions/{sessionId}` | Menampilkan ringkasan analitika sesi, pemain, dan ruleset aktif. |
| Session Ruleset Activation | `SessionsController` | `Ruleset` | `/sessions/{sessionId}/ruleset` | Aktivasi ruleset sesi untuk Instruktur. |
| Analytics (Legacy) | `AnalyticsController` | `Index` | `/analytics` | Route kompatibilitas; redirect ke daftar sesi atau detail sesi. |
| Rulebook | `HomeController` | `Rulebook` | `/home/rulebook` | Konten rulebook permainan. |

---

## 6. Rancangan Halaman Utama
### 6.1 Daftar sesi (`/sessions`)
Menampilkan:
1. nama sesi,
2. mode,
3. status,
4. waktu dibuat/mulai/selesai,
5. aksi ke detail sesi.

### 6.2 Detail sesi (`/sessions/{sessionId}`)
Menampilkan:
1. metrik sesi (event, cash in, cash out, net, violations),
2. daftar pemain dengan indikator learning/mission,
3. grafik garis ringkas (tren kas dan performa),
4. tautan ke detail pemain.

### 6.3 Detail pemain (`/sessions/{sessionId}/players/{playerId}`)
Menampilkan:
1. metrik pemain,
2. histori transaksi,
3. snapshot gameplay raw dan derived,
4. grafik garis tren performa pemain.

### 6.4 Ruleset list/details
Menampilkan:
1. daftar ruleset dan versi terbaru,
2. detail versi ruleset,
3. komponen dan ringkasan konfigurasi ruleset,
4. aksi create/edit/delete/activate untuk role Instruktur.

### 6.5 Ruleset analytics (embedded di `/sessions/{sessionId}`)
Menampilkan:
1. agregasi performa learning per ruleset,
2. agregasi performa misi per ruleset,
3. pemecahan berdasarkan sesi dan pemain.

Catatan implementasi:
- UI tidak memiliki route terpisah `/analytics/rulesets/{rulesetId}`.
- Route `/analytics` hanya dipertahankan sebagai kompatibilitas dan melakukan redirect ke daftar/detail sesi.
- Ringkasan ruleset dipanggil dari endpoint `GET /api/v1/analytics/rulesets/{rulesetId}/summary` setelah analitika sesi pada detail sesi berhasil dimuat.

---

## 7. Pemetaan Endpoint API ke UI
| Kebutuhan UI | Endpoint | Otorisasi |
|---|---|---|
| Login | `POST /api/v1/auth/login` | publik |
| Register | `POST /api/v1/auth/register` | publik (PLAYER dan INSTRUCTOR) |
| Daftar sesi | `GET /api/v1/sessions` | Bearer |
| Daftar ruleset | `GET /api/v1/rulesets` | Bearer |
| Detail ruleset | `GET /api/v1/rulesets/{rulesetId}` | Bearer |
| Komponen ruleset | `GET /api/v1/rulesets/{rulesetId}/components` | Bearer |
| Komponen default | `GET /api/v1/rulesets/components/defaults` | Bearer |
| Analitika sesi | `GET /api/v1/analytics/sessions/{sessionId}` | Bearer |
| Linimasa event sesi | `GET /api/v1/sessions/{sessionId}/events` | Bearer |
| Histori transaksi | `GET /api/v1/analytics/sessions/{sessionId}/transactions?playerId=...` | Bearer |
| Gameplay snapshot | `GET /api/v1/analytics/sessions/{sessionId}/players/{playerId}/gameplay` | Bearer |
| Analitika per ruleset | `GET /api/v1/analytics/rulesets/{rulesetId}/summary` | Bearer |

Endpoint operasional berikut tetap tersedia di API, tetapi dipakai oleh Klien Game/IDN atau integrasi API, bukan oleh Web Analitik MVC:
- `POST /api/v1/sessions`
- `POST /api/v1/sessions/{sessionId}/players`
- `POST /api/v1/sessions/{sessionId}/start`
- `POST /api/v1/sessions/{sessionId}/end`
- `POST /api/v1/events`
- `POST /api/v1/events/batch`

Endpoint ruleset berikut juga dipanggil oleh Web Analitik MVC untuk role Instruktur:
- `POST /api/v1/rulesets`
- `PUT /api/v1/rulesets/{rulesetId}`
- `POST /api/v1/rulesets/{rulesetId}/versions/{version}/activate`
- `DELETE /api/v1/rulesets/{rulesetId}`
- `DELETE /api/v1/rulesets/{rulesetId}/versions/{version}`
- `POST /api/v1/sessions/{sessionId}/ruleset/activate`

---

## 8. Kontrak Data UI (ViewModel)
ViewModel UI mengikuti implementasi nyata di proyek `Cashflowpoly.Ui/Models`.

Minimal model yang dipakai:
1. session list + session detail,
2. player detail + transaction rows,
3. ruleset list/detail,
4. analytics request/response untuk sesi,
5. analytics summary berbasis ruleset.

Prinsip:
1. DTO API terpisah dari ViewModel tampilan,
2. format angka/tanggal dilakukan di UI,
3. fallback null menjadi teks aman (`-`).

---

## 9. Penanganan Error UI
UI wajib menangani:
1. `400/422`: tampilkan pesan validasi,
2. `401`: redirect login,
3. `403`: tampilkan akses ditolak,
4. `404`: tampilkan data tidak ditemukan,
5. `429`: tampilkan permintaan terlalu sering,
6. `500`: tampilkan pesan umum + `trace_id` jika tersedia.

---

## 10. Kinerja dan Optimasi
1. Gunakan endpoint ringkasan untuk halaman utama.
2. Histori transaksi memakai pagination/limit.
3. Grafik garis memuat data teragregasi, bukan seluruh event mentah.
4. Hindari render ulang grafik terus-menerus saat resize.

---

## 11. Checklist Kesiapan Implementasi UI
UI dinyatakan siap jika:
1. semua route utama dapat diakses sesuai role,
2. semua panggilan API terproteksi memakai token Bearer,
3. halaman detail sesi (analitika) dan per-ruleset berjalan,
4. grafik garis tidak mengalami stretch horizontal tak terbatas,
5. error state 401/403/422/429 tampil sesuai standar.
