# Spesifikasi UI MVC dan Rancangan ViewModel
## Dasbor Analitika dan Manajemen Ruleset Cashflowpoly (ASP.NET Core MVC + Razor Views)

### Dokumen
- Nama dokumen: Spesifikasi UI MVC dan Rancangan ViewModel
- Versi: 1.1
- Tanggal: 8 Februari 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan
Dokumen ini menetapkan:
1. struktur halaman UI MVC,
2. kontrak ViewModel dan DTO,
3. pemetaan API ke tampilan,
4. standar perilaku error dan otorisasi.

Dokumen ini mengikuti:
- `docs/01-Spesifikasi/01-02-spesifikasi-event-dan-kontrak-api.md`
- `docs/01-Spesifikasi/01-04-kontrak-integrasi-idn-dan-keamanan.md`
- `docs/02-Perancangan/02-03-rancangan-dashboard-analitika-mvc.md`

---

## 2. Prinsip Implementasi UI
1. UI tidak mengakses database secara langsung.
2. UI memanggil API backend lewat `HttpClient`.
3. Endpoint terproteksi wajib memakai token Bearer dari session.
4. UI tidak menghitung rumus metrik domain; UI hanya presentasi data API.
5. UI menampilkan state error API secara eksplisit (`401`, `403`, `422`, `429`, `500`).

---

## 3. Struktur Folder Aktual Proyek UI
```
Cashflowpoly.Ui/
  Controllers/
    AnalyticsController.cs
    AuthController.cs
    HomeController.cs
    LanguageController.cs
    PlayerDirectoryController.cs
    PlayersController.cs
    RulesetsController.cs
    SessionsController.cs
  Infrastructure/
    AuthSessionExtensions.cs
    RoleHeaderHandler.cs
    RulebookContent.cs
    UiText.cs
  Models/
    AnalyticsViewModels.cs
    ApiDtos.cs
    AuthViewModels.cs
    RulebookViewModels.cs
    RulesetViewModels.cs
  Views/
    Analytics/
    Auth/
    Home/
    Players/
    Rulesets/
    Sessions/
    Shared/
  wwwroot/
    css/
    js/
    images/
```

---

## 4. Halaman dan Route Utama
| Halaman | Route | Controller | Akses |
|---|---|---|---|
| Login | `/auth/login` | `AuthController` | Publik |
| Register | `/auth/register` | `AuthController` | Publik |
| Home | `/` | `HomeController` | Login |
| Sessions | `/sessions` | `SessionsController` | Login |
| Session details | `/sessions/{sessionId}` | `SessionsController` | Login |
| Session ruleset | `/sessions/{sessionId}/ruleset` | `SessionsController` | Instruktur |
| Players | `/players` | `PlayerDirectoryController` | Login |
| Player details | `/sessions/{sessionId}/players/{playerId}` | `PlayersController` | Login |
| Rulesets | `/rulesets` | `RulesetsController` | Login |
| Ruleset details | `/rulesets/{rulesetId}` | `RulesetsController` | Login |
| Analytics | `/analytics` | `AnalyticsController` | Login |
| Rulebook | `/rulebook` | `HomeController` | Login |

---

## 5. DTO API yang Dipakai UI
DTO ada di `Cashflowpoly.Ui/Models/ApiDtos.cs`.

Kelompok utama:
1. autentikasi: login/register,
2. sesi,
3. ruleset,
4. analitika sesi dan pemain,
5. histori transaksi,
6. error response standar.

Aturan:
1. field JSON mengikuti snake_case dari API,
2. deserialisasi menggunakan `System.Net.Http.Json`.

---

## 6. ViewModel UI yang Dipakai
ViewModel ada di:
- `Cashflowpoly.Ui/Models/AnalyticsViewModels.cs`
- `Cashflowpoly.Ui/Models/AuthViewModels.cs`
- `Cashflowpoly.Ui/Models/RulesetViewModels.cs`
- `Cashflowpoly.Ui/Models/RulebookViewModels.cs`

Entitas tampilan minimum:
1. daftar sesi,
2. detail sesi + summary + by-player,
3. detail pemain + gameplay + transaksi,
4. list/detail/create ruleset,
5. login/register form,
6. rulebook sections.

---

## 7. Pemetaan API ke Halaman
| Halaman | Endpoint API |
|---|---|
| Login | `POST /api/auth/login` |
| Register | `POST /api/auth/register` |
| Sessions list | `GET /api/sessions` |
| Session details | `GET /api/analytics/sessions/{sessionId}` |
| Player details | `GET /api/analytics/sessions/{sessionId}` + `GET /api/analytics/sessions/{sessionId}/transactions?playerId=...` + `GET /api/analytics/sessions/{sessionId}/players/{playerId}/gameplay` |
| Rulesets list | `GET /api/rulesets` |
| Ruleset create | `POST /api/rulesets` |
| Ruleset archive | `POST /api/rulesets/{rulesetId}/archive` |
| Ruleset delete | `DELETE /api/rulesets/{rulesetId}` |
| Session ruleset activation | `POST /api/sessions/{sessionId}/ruleset/activate` |

---

## 8. Aturan Otorisasi di UI
1. Middleware UI memaksa login untuk semua route non publik.
2. Session menyimpan `role`, `username`, dan `access_token`.
3. Aksi instruktur-only (create/archive/delete/activate ruleset) harus diverifikasi role di UI dan API.
4. Jika API membalas `403`, UI tetap menampilkan halaman read-only bila memungkinkan.

---

## 9. Standar Error UI
1. `401`: redirect ke login dan hapus session auth.
2. `403`: tampilkan pesan akses ditolak.
3. `404`: tampilkan data tidak ditemukan.
4. `422`: tampilkan detail validasi dari API.
5. `429`: tampilkan notifikasi rate limit.
6. `500`: tampilkan pesan umum dan `trace_id` jika tersedia.

---

## 10. Standar Presentasi Data
1. Angka uang: format pemisah ribuan.
2. Persentase: tampilkan 0-100 dengan dua digit desimal jika perlu.
3. Waktu: tampilkan lokal pengguna dengan format konsisten.
4. Data kosong: render placeholder aman (`-` atau pesan empty state).

---

## 11. Checklist Kesiapan UI
1. Semua halaman utama render tanpa error.
2. Semua call API terproteksi memakai token Bearer.
3. Alur login/register/logout berjalan.
4. Aksi instruktur-only tidak bisa diakses player.
5. Halaman analitika dan ruleset menampilkan error state dengan benar.
