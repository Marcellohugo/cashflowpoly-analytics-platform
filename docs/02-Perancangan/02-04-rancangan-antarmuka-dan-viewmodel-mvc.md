# Rancangan Antarmuka dan ViewModel MVC
## Web Analitika Cashflowpoly (ASP.NET Core MVC + Razor Views)

### Informasi Dokumen
- **Nama Dokumen**: Rancangan Antarmuka dan ViewModel MVC
- **Versi**: 1.5
- **Tanggal**: 13 September 2026
- **Penyusun**: Marco Marcello Hugo

---

## 1. Tujuan Dokumen
Dokumen ini mendefinisikan rancangan antarmuka pengguna, struktur navigasi rute MVC, pemetaan DTO/ViewModel, serta pemetaan rute API backend ke frontend Web Analitika MVC (Razor Views). Dokumen ini menjadi acuan tunggal pengerjaan frontend dasbor analitika.

---

## 2. Cakupan Antarmuka (Scope UI)

Web Analitika MVC difokuskan sebagai **Dasbor Analitik Pemantauan Pembelajaran**.

### 2.1 Fitur yang Masuk Scope:
- Halaman masuk (*login*) dan daftar (*register*).
- Halaman dasbor daftar sesi, detail metrik sesi agregat, dan timeline event sesi.
- Halaman performa individual pemain beserta histori transaksi lengkapnya.
- Halaman katalog ruleset, detail aturan komponen ruleset, dan form create/edit ruleset (khusus Instruktur).
- Halaman panduan buku aturan (*rulebook*) bilingual.
- Statistik lintas sesi satu pemain dalam satu mode, memakai grafik SVG dan tabel sumber. Timeline memeriksa event baru berkala; halaman analitika memperoleh angka terbaru saat dimuat ulang.

### 2.2 Fitur di Luar Scope UI Web:
- Antarmuka permainan seluler Klien Game/IDN.
- Fitur operasional gameplay (membuat sesi baru, menambahkan pemain ke sesi, memulai/mengakhiri sesi, pengiriman event permainan). Operasi ini dilakukan eksklusif oleh Game Client/IDN atau melalui integrasi API eksternal.

---

## 3. Struktur Folder Proyek UI MVC
Struktur pengorganisasian kode pada proyek `src/Cashflowpoly.Ui/` adalah sebagai berikut:

```text
Cashflowpoly.Ui/
  Controllers/
    AuthController.cs          -> Kontrol masuk/daftar pengguna
    HomeController.cs          -> Kontrol halaman utama, privasi, dan rulebook
    SessionsController.cs      -> Kontrol daftar sesi dan detail analitika sesi
    PlayersController.cs       -> Kontrol detail analitika individual pemain
    PlayerStatisticsController.cs -> Statistik peserta sesuai scope akun dan mode
    RulesetsController.cs      -> CRUD, aktivasi, dan detail ruleset
    LanguageController.cs      -> Preferensi bahasa (bilingual)
    AnalyticsController.cs     -> Redirect rute analytics
  Infrastructure/
    ApiAuthHelper.cs           -> Helper auth API backend
    AuthSessionExtensions.cs   -> AuthContextExtensions membaca role principal cookie
    BearerTokenHandler.cs      -> Handler JWT Bearer HttpClient
    HttpContentExtensions.cs   -> Ekstraksi respons HTTP
    RulebookContent.cs         -> Penyimpan statis teks buku aturan
    UiText.cs                  -> Lexicon terjemahan bilingual
    PlayerStatisticsChartBuilder.cs -> Pemetaan metrik antarsesi untuk grafik
    SessionRosterLoader.cs     -> Pembacaan roster sesi melalui endpoint gabungan
  Contracts/
    Dtos.cs                    -> Data Transfer Object API backend
    RulesetDefinitionDtos.cs   -> DTO definition ter-normalisasi
    ErrorResponse.cs           -> Format error API standar
  Models/
    AuthViewModels.cs          -> Model data login & register
    AnalyticsViewModels.cs     -> Model data visualisasi analitik & transaksi
    PlayerStatisticsViewModels.cs -> Pilihan pemain, sesi, mode, dan grafik statistik
    RulesetViewModels.cs       -> Model data form/view ruleset
    RulebookViewModels.cs      -> Model data render buku aturan
  Views/                       -> Razor Views (.cshtml) per modul
  wwwroot/                     -> Tailwind CSS, renderer SVG JavaScript, gambar
```

---

## 4. Struktur Navigasi dan Rute MVC

Dasbor memetakan rute URL antarmuka pengguna sebagai berikut:

| Nama Tampilan | URL Dasbor | Controller | Action | Akses Peran |
|---|---|---|---|---|
| Halaman Utama (Home) | `/` | `HomeController` | `Index` | `INSTRUCTOR` / `PLAYER` |
| Daftar Sesi | `/sessions` | `SessionsController` | `Index` | `INSTRUCTOR` / `PLAYER` |
| Detail Sesi & Analitika | `/sessions/{id}` | `SessionsController` | `Details` | `INSTRUCTOR` / `PLAYER` |
| Performa Detil Pemain | `/sessions/{id}/players/{userId}` | `PlayersController` | `Details` | `INSTRUCTOR` / `PLAYER` |
| Statistik Pemain | `/statistics` | `PlayerStatisticsController` | `Index` | Player sendiri / Instruktur pemilik sesi |
| Buku Aturan (Rulebook) | `/rulebook` | `HomeController` | `Rulebook` | Publik |
| Daftar Ruleset | `/rulesets` | `RulesetsController` | `Index` | `INSTRUCTOR`; Player diarahkan ke `/sessions` |
| Detail Versi Ruleset | `/rulesets/{id}` | `RulesetsController` | `Details` | `INSTRUCTOR`; Player diarahkan ke `/sessions` |
| Formulir Buat Ruleset | `/rulesets/create` | `RulesetsController` | `Create` | `INSTRUCTOR` |
| Formulir Edit Ruleset | `/rulesets/{id}/edit` | `RulesetsController` | `Edit` | `INSTRUCTOR` |
| Rute Analytics (Redirect) | `/analytics` | `AnalyticsController` | `Index` | Redirect ke `/sessions` |

---

## 5. Prinsip Desain & Presentasi Data

### 5.1 Konsistensi Grafik Visual
- Grafik memakai renderer SVG `player-detail-charts.js` dalam container responsif. Rincian titik tampil sementara di bawah grafik saat hover atau fokus keyboard yang terlihat (`:focus-visible`), lalu menutup pada pointer keluar, blur, atau Escape. Klik/ketukan tidak mengunci rincian; tabel nilai dan sumber menyediakan akses data untuk layar sentuh.
- Grafik memuat data teragregasi yang dipanggil dari API, bukan melakukan kalkulasi raw event secara langsung di browser client.

### 5.2 Standar Format Data
- **Koin / Uang**: Ditampilkan dengan pemisah ribuan (contoh: `10,000`).
- **Persentase / Rasio**: Ditampilkan dalam skala 0-100 dengan pembulatan dua digit desimal (contoh: `85.50%`).
- **Status Data Kosong**: Jika data bernilai `null` atau tidak tersedia, UI wajib menampilkan keterangan eksplisit seperti **Belum ada data** atau **Belum dapat dihitung**. Nilai tersebut tidak boleh diubah menjadi angka nol atau status `false` karena dapat memicu kesimpulan yang salah.

### 5.3 Detail Pemain
- Urutan utama halaman adalah **Ringkasan Statistik Pemain**, **Cerita di Balik Hasil Pemain**, lalu **Data Permainan Lengkap**.
- Ketiga bagian memakai elemen native `<details>/<summary>` agar dapat dilipat tanpa state JavaScript tambahan.
- Setiap rincian **Lihat angka pembentuk dan rumus** harus memakai nama variabel yang sama dengan metrik induknya serta menampilkan sumber data, rumus, substitusi angka aktual, dan hasil perhitungan.
- Metrik khusus mode Mahir hanya dirender ketika sesi memakai ruleset mode `MAHIR`.
- Indikator buka/tutup ditempatkan di sisi kanan judul dan tetap memiliki target interaksi serta status aksesibel bawaan browser.

### 5.4 Jejak Perjalanan Sesi
- `day_index=0` merepresentasikan fase setup dan ditampilkan sebagai **GO**.
- Hari permainan memakai indeks `1..25`; formatter tidak menambah atau memaksa indeks 0 menjadi Hari 1.
- `action_slot=0` dipertahankan untuk event sistem dan aksi gratis.

### 5.5 Statistik lintas sesi
- Instruktur mencari nama peserta dari sesi miliknya; nama ambigu dibedakan dengan UUID. Player hanya dapat melihat dirinya sendiri.
- Filter selalu memilih satu mode (`PEMULA`/`MAHIR`) dan status sesi. Sesi `CREATED` memiliki gameplay `null` dan tidak digambar sebagai nol.
- Tombol kelompok metrik menampilkan satu kelompok grafik. Tabel nilai dan sumber di bawah setiap grafik memakai paginasi **5 baris per halaman**.
- Data sesi, roster, dan gameplay diambil dari endpoint gabungan; jumlah permintaan API maksimal dua untuk Player dan tiga untuk Instruktur.
- Agregasi lintas sesi per ruleset tersedia melalui API; belum ada tampilan UI yang memanggil endpoint tersebut.

---

## 6. Kontrak Data (DTO & ViewModel)

### 6.1 DTO API (`Cashflowpoly.Ui/Contracts/`)
- Menerapkan anotasi `JsonPropertyName` snake_case untuk mencocokkan payload JSON API backend.
- Deserialisasi dilakukan menggunakan pustaka bawaan `System.Text.Json` agar performa memori optimal.

### 6.2 ViewModel Tampilan (`Cashflowpoly.Ui/Models/`)
- Menyimpan properti hasil konversi tipe data mentah API ke bentuk siap tampil (seperti format tanggal lokal dan status string berwarna).
- Controller membentuk ViewModel per halaman; beberapa ViewModel menyertakan DTO sesi atau gameplay sebagai bagian dari data tampilan.

---

## 7. Pemetaan Halaman ke Endpoint API

| Layanan Tampilan | Endpoint API Backend | Opsi Parameter / Keterangan |
|---|---|---|
| Autentikasi Masuk | `POST /api/v1/auth/login` | Memperoleh JWT token & data user role |
| Registrasi Akun | `POST /api/v1/auth/register` | Player atau Instruktur; `Auth:AllowPublicInstructorRegistration=true` secara default |
| Halaman Daftar Sesi | `GET /api/v1/sessions` | Membaca daftar sesi sesuai peran pengguna |
| Daftar Player Sesi | `GET /api/v1/sessions/{id}/players` | Membaca peserta sesi sesuai scope pengguna |
| Dasbor Detail Sesi | `GET /api/v1/analytics/sessions/{id}` | Ringkasan analitika lifetime sesi |
| Linimasa Event Sesi | `GET /api/v1/sessions/{id}/events` | Riwayat kronologi event permainan |
| Histori Transaksi Pemain| `GET /api/v1/analytics/sessions/{id}/players/{userId}/gameplay` | Tabel transaksi dibentuk dari histori koin pada snapshot gameplay yang sama dengan ringkasan pemain; endpoint `/transactions` tersedia untuk integrasi API, tetapi tidak dipanggil halaman ini. |
| Gameplay Snapshot | `GET /api/v1/analytics/sessions/{id}/players/{userId}/gameplay` | Kategori metrik detail pemain |
| Peserta Sesi dan Pilihan Pemain | `GET /api/v1/analytics/session-rosters` | Roster seluruh sesi sesuai scope dalam satu permintaan |
| Statistik Pemain | `GET /api/v1/analytics/players/{playerId}/gameplay?mode=...&status=...` | Riwayat gameplay pemain dalam satu mode |
| Manajemen Ruleset | `GET /api/v1/rulesets` | Daftar dan detail aturan ruleset |

---

## 8. Otorisasi & Penanganan Error di UI

### 8.1 Tiket cookie autentikasi
1. Token JWT (`access_token`) disimpan sebagai claim dalam tiket cookie autentikasi yang dilindungi ASP.NET Core Data Protection (terenkripsi), dengan `HttpOnly`, `SameSite=Lax`, dan `Secure` pada production. Penyimpanan ini tidak menggunakan session server-side.
2. Setiap request yang dipicu oleh `HttpClient` UI akan dilekatkan token Bearer tersebut melalui delegating handler `BearerTokenHandler`.
3. Jika backend API mengembalikan status `401 Unauthorized`, UI menghapus cookie autentikasi dan mengarahkan pengguna ke halaman login.

### 8.2 Response Handling State
-   **`403 Forbidden`**: UI menyembunyikan tombol mutasi ruleset untuk Player dan merender halaman error ramah pengguna "Akses Ditolak".
-   **`404 Not Found`**: Merender visualisasi *Empty State* atau keterangan data tidak tersedia.
-   **`429 Too Many Requests`**: Menampilkan dialog peringatan rate limit terlampaui.
-   **`500 Internal Server Error`**: Menampilkan pesan API atau pesan kesalahan umum. `trace_id` API belum diteruskan ke tampilan; ID permintaan pada halaman galat UI adalah ID milik permintaan UI tersebut.

### 8.3 Pembaruan timeline dan analitika sementara
Cursor opaque terakhir dari pemuatan awal disimpan dalam ViewModel. Polling melanjutkan dari cursor itu; entri SEALED diminta ulang melalui `refreshSequences` (maksimal 100 per permintaan) dan diganti berdasarkan sequence number. Respons `refreshed_items` tidak mengubah cursor halaman. Permintaan polling tidak tumpang tindih. Analitika publik mengecualikan donasi yang belum lengkap dan menyertakan `has_sealed_donations`; UI menampilkan keterangan sampai seluruh pemain menyetor.
