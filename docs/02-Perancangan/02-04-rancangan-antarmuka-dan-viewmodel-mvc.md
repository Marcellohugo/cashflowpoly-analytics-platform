# Rancangan Antarmuka dan ViewModel MVC
## Web Analitika Cashflowpoly (ASP.NET Core MVC + Razor Views)

### Informasi Dokumen
- **Nama Dokumen**: Rancangan Antarmuka dan ViewModel MVC
- **Versi**: 1.3
- **Tanggal**: 20 Juni 2026
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
- Grafik tren kas, fluktuasi aset, dan pencapaian skor yang real-time.

### 2.2 Fitur yang Diluar Scope UI Web:
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
    PlayerDirectoryController.cs -> Direktori data pemain global
    RulesetsController.cs      -> CRUD, aktivasi, dan detail ruleset
    LanguageController.cs      -> Preferensi bahasa (bilingual)
    AnalyticsController.cs     -> Redirect kompatibilitas rute legacy
  Infrastructure/
    ApiAuthHelper.cs           -> Helper auth API backend
    AuthSessionExtensions.cs    -> Extension session server-side
    BearerTokenHandler.cs      -> Handler JWT Bearer HttpClient
    HttpContentExtensions.cs   -> Ekstraksi respons HTTP
    RulebookContent.cs         -> Penyimpan statis teks buku aturan
    UiText.cs                  -> Lexicon terjemahan bilingual
  Contracts/
    Dtos.cs                    -> Data Transfer Object API backend
    RulesetDefinitionDtos.cs   -> DTO definition ter-normalisasi
    ErrorResponse.cs           -> Format error API standar
  Models/
    AuthViewModels.cs          -> Model data login & register
    AnalyticsViewModels.cs     -> Model data visualisasi analitik & transaksi
    RulesetViewModels.cs       -> Model data form/view ruleset
    RulebookViewModels.cs      -> Model data render buku aturan
  Views/                       -> Razor Views (.cshtml) per modul
  wwwroot/                     -> Aset statis (Tailwind CSS output, Chart.js, images)
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
| Direktori Pemain Global | `/players` | `PlayerDirectoryController`| `Index` | `INSTRUCTOR` |
| Buku Aturan (Rulebook) | `/rulebook` | `HomeController` | `Rulebook` | `INSTRUCTOR` / `PLAYER` |
| Daftar Ruleset | `/rulesets` | `RulesetsController` | `Index` | `INSTRUCTOR` / `PLAYER` |
| Detail Versi Ruleset | `/rulesets/{id}` | `RulesetsController` | `Details` | `INSTRUCTOR` / `PLAYER` |
| Formulir Buat Ruleset | `/rulesets/create` | `RulesetsController` | `Create` | `INSTRUCTOR` |
| Formulir Edit Ruleset | `/rulesets/{id}/edit` | `RulesetsController` | `Edit` | `INSTRUCTOR` |
| Rute Legacy (Kesesuaian) | `/analytics` | `AnalyticsController` | `Index` | Redirect ke `/sessions` |

---

## 5. Prinsip Desain & Presentasi Data

### 5.1 Konsistensi Grafik Visual
- Semua elemen grafik visual (tren arus kas, pertumbuhan aset) dibungkus dalam container dengan tinggi tetap (*fixed height*) untuk mencegah layout melar tanpa batas (*infinite horizontal stretch*).
- Grafik memuat data teragregasi yang dipanggil dari API, bukan melakukan kalkulasi raw event secara langsung di browser client.

### 5.2 Standar Format Data
- **Koin / Uang**: Ditampilkan dengan pemisah ribuan (contoh: `10,000`).
- **Persentase / Rasio**: Ditampilkan dalam skala 0-100 dengan pembulatan dua digit desimal (contoh: `85.50%`).
- **Placeholder Data Kosong**: Jika data bernilai null/empty, wajib digantikan placeholder aman seperti `-` atau `N/A`, bukan dibiarkan kosong.

---

## 6. Kontrak Data (DTO & ViewModel)

### 6.1 DTO API (`Cashflowpoly.Ui/Contracts/`)
- Menerapkan anotasi `JsonPropertyName` snake_case untuk mencocokkan payload JSON API backend.
- Deserialisasi dilakukan menggunakan pustaka bawaan `System.Text.Json` agar performa memori optimal.

### 6.2 ViewModel Tampilan (`Cashflowpoly.Ui/Models/`)
- Menyimpan properti hasil konversi tipe data mentah API ke bentuk siap tampil (seperti format tanggal lokal dan status string berwarna).
- DTO API tidak pernah diumpankan langsung ke Razor View; pemetaan dilakukan di controller menggunakan ViewModel perantara.

---

## 7. Pemetaan Halaman ke Endpoint API

| Layanan Tampilan | Endpoint API Backend | Opsi Parameter / Keterangan |
|---|---|---|
| Autentikasi Masuk | `POST /api/v1/auth/login` | Memperoleh JWT token & data user role |
| Registrasi Akun | `POST /api/v1/auth/register` | Mendaftarkan akun instruktur/pemain baru |
| Halaman Daftar Sesi | `GET /api/v1/sessions` | Membaca daftar sesi sesuai peran pengguna |
| Dasbor Detail Sesi | `GET /api/v1/analytics/sessions/{id}` | Ringkasan analitika lifetime sesi |
| Linimasa Event Sesi | `GET /api/v1/sessions/{id}/events` | Riwayat kronologi event permainan |
| Histori Transaksi Pemain| `GET /api/v1/analytics/sessions/{id}/transactions?userId=...` | Transaksi koin per pemain |
| Gameplay Snapshot | `GET /api/v1/analytics/sessions/{id}/players/{userId}/gameplay` | Kategori metrik detail pemain |
| Manajemen Ruleset | `GET /api/v1/rulesets` | Daftar dan detail aturan ruleset |
| Ringkasan Per Ruleset | `GET /api/v1/analytics/rulesets/{id}/summary` | Agregasi performa lintas sesi |

---

## 8. Otorisasi & Penanganan Error di UI

### 8.1 Manajemen Session Token
1. Token JWT (`access_token`) disimpan di session server-side Web MVC setelah login berhasil.
2. Setiap request yang dipicu oleh `HttpClient` UI akan dilekatkan token Bearer tersebut melalui delegating handler `BearerTokenHandler`.
3. Jika backend API mengembalikan status `401 Unauthorized`, UI otomatis membersihkan session lokal dan mengarahkan pengguna ke halaman login.

### 8.2 Response Handling State
-   **`403 Forbidden`**: UI menyembunyikan tombol mutasi ruleset untuk Player dan merender halaman error ramah pengguna "Akses Ditolak".
-   **`404 Not Found`**: Merender visualisasi *Empty State* atau keterangan data tidak tersedia.
-   **`429 Too Many Requests`**: Menampilkan dialog peringatan rate limit terlampaui.
-   **`500 Internal Server Error`**: Menampilkan pesan kesalahan umum beserta `trace_id` yang didapat dari respons API agar dapat dilaporkan ke tim administrasi.
