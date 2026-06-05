# Kebutuhan Diagram UML
## Cashflowpoly Analytics Platform

### Nama aplikasi
Cashflowpoly Analytics Platform.

Nama sistem pada dokumen proyek: Sistem Informasi Dasbor Analitika Cashflowpoly.

### Jenis UML yang dibutuhkan
Diagram UML yang dibutuhkan: semuanya.

1. Use Case Diagram
   - Menjelaskan aktor Instruktur, Player, Klien Game/IDN, API, dan PostgreSQL.
   - Menegaskan batas: operasi permainan dilakukan melalui Klien Game/IDN, sedangkan Web Analitik membaca data.

2. Activity Diagram
   - Menjelaskan alur login, pembuatan sesi, pemilihan ruleset, penambahan Player, start sesi, input keputusan Player, ingest event, validasi API, penyimpanan data, dan pembacaan analitika.

3. Sequence Diagram
   - Menjelaskan komunikasi Klien Game/IDN -> API -> PostgreSQL untuk setup dan event.
   - Menjelaskan komunikasi Web Analitik -> API -> PostgreSQL untuk dashboard.

4. Class Diagram
   - Menjelaskan model akun, Player, sesi, ruleset, event, validasi, proyeksi cashflow, metrik, audit, dan controller/service/repository utama.

### Aktor pengguna
1. Instruktur
   - Login/sign in ke Web Analitik atau Klien Game/IDN.
   - Membuat sesi, memilih ruleset, menambahkan Player, memulai/mengakhiri sesi, dan memasukkan keputusan Player melalui Klien Game/IDN.
   - Melihat analitika sesi secara real-time melalui Web Analitik.
   - Role API: `INSTRUCTOR`.

2. Player
   - Login/sign in ke Web Analitik atau Klien Game/IDN.
   - Mengikuti sesi permainan.
   - Melihat data permainan sesuai hak akses melalui Web Analitik.
   - Role API: `PLAYER`.

3. Klien Game/IDN
   - Antarmuka operasional permainan.
   - Mengirim setup sesi, aktivasi ruleset, daftar Player, start/end sesi, state permainan, dan event permainan ke API.
   - Wajib memakai token Bearer untuk endpoint terproteksi.

4. API
   - Memvalidasi token autentikasi, data sesi, data Player, ruleset aktif, urutan event, duplikasi event, dan aturan domain.
   - Menyimpan data valid ke PostgreSQL.
   - Menyediakan data analitika untuk Web Analitik.

5. Web Analitik
   - UI MVC berbasis Razor Views.
   - Membaca daftar sesi, detail sesi, ruleset, timeline event, metrik sesi, dan metrik Player dari API.
   - Tidak membuat/mengubah sesi, ruleset, Player, atau event permainan.

6. Database PostgreSQL
   - Menyimpan akun, Player, sesi, ruleset, versi ruleset, event, proyeksi arus kas, metric snapshot, validation log, dan audit keamanan.

### Fitur utama
1. Login/register
   - Instruktur dan Player dapat login/register.
   - API menghasilkan JWT access token.

2. Setup sesi melalui Klien Game/IDN
   - Instruktur membuat sesi.
   - Instruktur memilih ruleset.
   - Instruktur menambahkan Player.
   - Instruktur memulai sesi.

3. Input keputusan permainan melalui Klien Game/IDN
   - Instruktur memasukkan input berdasarkan keputusan Player saat bermain di papan permainan.
   - Klien Game/IDN mengirim event permainan ke API.

4. Validasi event di API
   - Token autentikasi.
   - Data sesi permainan.
   - Data Player.
   - Ruleset aktif.
   - Urutan event.
   - Duplikasi event.
   - Aturan domain permainan.

5. Penyimpanan event valid
   - API menyimpan event valid ke PostgreSQL.
   - API mencatat validation log untuk event valid maupun invalid.

6. Web Analitik real-time
   - Instruktur melihat jumlah event, cash in, cash out, net cashflow, performa Player, dan pelanggaran validasi.
   - Web Analitik membaca data dari API.

7. Akses Player setelah permainan
   - Player melihat sesi dan performa yang sesuai hak aksesnya.
   - API membatasi data berdasarkan role dan cakupan sesi Player.

8. Referensi ruleset
   - Instruktur dan Player melihat daftar/detail ruleset yang diizinkan.
   - Detail ruleset dipakai sebagai konteks pembacaan metrik, bukan sebagai form perubahan data di Web Analitik.

### Alur pengguna
#### Alur utama sistem
1. Instruktur dan Player login/sign in ke Web Analitik atau Klien Game/IDN.
2. Instruktur membuat sesi permainan melalui Klien Game/IDN.
3. Instruktur memilih ruleset yang akan digunakan pada sesi permainan melalui Klien Game/IDN.
4. Instruktur menambahkan Player ke dalam sesi permainan melalui Klien Game/IDN.
5. Instruktur memulai sesi permainan melalui Klien Game/IDN.
6. Selama permainan berjalan, Instruktur memasukkan input Player melalui Klien Game/IDN berdasarkan keputusan Player saat bermain di papan permainan.
7. Klien Game/IDN mengirim event permainan ke API.
8. API memvalidasi event permainan, meliputi token autentikasi, data sesi permainan, data Player, ruleset aktif, urutan event, dan duplikasi event.
9. Jika event valid, API menyimpan event permainan ke PostgreSQL.
10. Web Analitik membaca data permainan dari API.
11. Instruktur melihat data permainan secara real-time melalui Web Analitik, meliputi jumlah event, cash in, cash out, net cashflow, performa Player, dan pelanggaran validasi.
12. Setelah permainan berakhir, Player dapat melihat data permainan yang sesuai dengan hak aksesnya melalui Web Analitik.

#### Alur login/register
1. User membuka Web Analitik atau Klien Game/IDN.
2. User login/register dengan username, password, dan role.
3. API memvalidasi kredensial.
4. Jika berhasil, API mengembalikan access token dan data user.
5. Jika gagal, sistem menampilkan pesan error.

#### Alur ingest event dari Klien Game/IDN
1. Klien Game/IDN mengirim `POST /api/v1/events` atau `POST /api/v1/events/batch`.
2. API memvalidasi Bearer token dan role.
3. API memvalidasi session, Player, ruleset version aktif, payload, sequence number, dan idempotency key `session_id + event_id`.
4. Jika sequence number melompat, duplikat, atau ruleset tidak aktif, API menolak event.
5. Jika valid, API menyimpan event dan proyeksi cashflow dalam transaksi database.
6. API mengembalikan respons sukses atau error standar.

#### Alur Web Analitik
1. User login ke Web Analitik.
2. UI MVC mengambil daftar sesi dari API.
3. User membuka detail sesi.
4. UI MVC mengambil analitika sesi, daftar event, ruleset aktif, dan data Player dari API.
5. API mengambil data dari PostgreSQL sesuai hak akses.
6. UI MVC menampilkan ringkasan, tabel Player, timeline, grafik, dan detail ruleset.

### Role dan hak akses
#### Instruktur (`INSTRUCTOR`)
- Login/register.
- Mengakses Web Analitik.
- Melihat sesi miliknya.
- Melihat analitika sesi, Player, ruleset, timeline event, dan validation violations.
- Melihat direktori Player.
- Melihat audit keamanan dan observability bila endpoint diizinkan.
- Melalui Klien Game/IDN atau integrasi API:
  - membuat sesi,
  - memilih/mengaktifkan ruleset pada sesi,
  - menambahkan Player ke sesi,
  - memulai/mengakhiri sesi,
  - mengirim event permainan,
  - menyimpan state sesi.

#### Player (`PLAYER`)
- Login/register.
- Mengakses Web Analitik.
- Melihat daftar sesi yang terkait dengannya.
- Melihat detail sesi dan performa yang diizinkan.
- Melihat ruleset dan rulebook.
- Tidak dapat mengelola sesi, Player, ruleset, audit, atau data di luar cakupan sesi.

#### Klien Game/IDN
- Mengakses endpoint operasional API dengan token Bearer.
- Mengirim request setup sesi dan event permainan.
- Menjaga `event_id` stabil saat retry.
- Menjaga `sequence_number` berurutan per sesi.

### Catatan khusus
#### Ada approval?
Tidak ada modul approval formal. Kontrol dilakukan melalui role `INSTRUCTOR`, status sesi, ruleset aktif, idempotensi event, dan guard data yang sudah dipakai.

#### Ada pembayaran?
Tidak ada pembayaran aplikasi. Transaksi pada sistem adalah transaksi permainan/cashflow simulasi.

#### Ada notifikasi?
Tidak ada modul notifikasi khusus. UI menampilkan error, empty state, dan status sinkronisasi data.

#### Ada dashboard?
Ada. Web Analitik menampilkan dashboard utama, daftar sesi, detail sesi, performa Player, timeline event, detail ruleset, dan rulebook.

#### Ada integrasi API?
Ada. API menyediakan autentikasi, lifecycle sesi, ruleset, Player, event ingestion, analitika, observability, audit, dan health check.

### Catatan untuk pembuatan diagram
#### Use Case Diagram
Use case utama:
- Login/register
- Buat sesi melalui Klien Game/IDN
- Pilih ruleset melalui Klien Game/IDN
- Tambah Player melalui Klien Game/IDN
- Mulai/akhiri sesi melalui Klien Game/IDN
- Input keputusan Player melalui Klien Game/IDN
- Kirim event permainan
- Validasi event
- Simpan event
- Lihat Web Analitik
- Lihat data Player sesuai hak akses

#### Activity Diagram
Activity diagram yang disarankan:
- Login user
- Setup sesi dari Klien Game/IDN
- Ingest event dan validasi API
- Web Analitik membaca data sesi
- Player melihat hasil sesuai hak akses

#### Sequence Diagram
Sequence diagram yang disarankan:
- Login: User -> Web Analitik/Klien Game -> API Auth -> PostgreSQL -> User
- Setup sesi: Instruktur -> Klien Game/IDN -> API Sessions/Rulesets/Players -> PostgreSQL -> Klien Game/IDN
- Ingest event: Klien Game/IDN -> API Events -> EventIngestionService -> PostgreSQL -> Klien Game/IDN
- Dashboard: User -> Web Analitik MVC -> API Analytics/Sessions/Rulesets -> PostgreSQL -> Web Analitik

#### Class Diagram
Entitas utama:
- AppUser
- UserPlayerLink
- Player
- Session
- SessionPlayer
- Ruleset
- RulesetVersion
- SessionRulesetActivation
- EventLog
- CashflowProjection
- MetricSnapshot
- ValidationLog
- SecurityAuditLog

Komponen aplikasi:
- UI Controllers: AuthController, HomeController, SessionsController, PlayersController, PlayerDirectoryController, RulesetsController
- API Controllers: AuthController, SessionsController, PlayersController, RulesetsController, EventsController, AnalyticsController
- Services: JwtTokenService, EventIngestionService, AnalyticsService, SecurityAuditService
- Repositories: UserRepository, SessionRepository, PlayerRepository, RulesetRepository, EventRepository, MetricsRepository
