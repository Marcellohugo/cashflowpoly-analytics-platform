# SQL Seed Consolidation Design

## Tujuan

Menyatukan kepemilikan DDL dan seed database agar bootstrap database lebih rapi, tidak ada definisi schema ganda, dan ruleset default yang dipakai API konsisten antara kebutuhan rulebook, endpoint ruleset, dan session state.

## Masalah Saat Ini

- `database/00_create_schema.sql` sudah berisi tabel session state dan tabel pendukungnya.
- `database/03_session_state_seed.sql` masih mendefinisikan ulang DDL yang sama, sehingga ownership schema terpecah.
- `database/01_seed_default_rulesets_components.sql` berisi ruleset default berbasis komponen fisik, tetapi belum memuat katalog digital lengkap yang dibutuhkan `SessionStateRepository`.
- `database/03_session_state_seed.sql` menambahkan ruleset digital khusus dengan struktur `component_catalog` yang berbeda dari ruleset default utama.
- `DatabaseInitialization` masih menjalankan `03_session_state_seed.sql`, sehingga startup bergantung pada file yang mencampur schema dan seed.

## Hasil Desain

### Kepemilikan File

`database/00_create_schema.sql`
- Menjadi satu-satunya sumber DDL.
- Seluruh `create table`, `create index`, dan constraint session state tetap berada di file ini.

`database/01_seed_default_rulesets_components.sql`
- Menjadi satu-satunya sumber seed ruleset default aplikasi.
- Ruleset default `PEMULA` dan `MAHIR` tetap dipertahankan sebagai ruleset publik utama.
- Kedua ruleset default diperkaya agar `component_catalog` memuat:
  - `gameConfig`
  - `bahan`
  - `resep`
  - `kebutuhan`
  - `targetKebutuhan`
  - `tujuanFinansial`
  - `narasi`
  - `quest`
- Struktur komponen fisik yang sudah ada tetap dipertahankan, sehingga satu ruleset default bisa melayani kebutuhan rulebook dan kebutuhan session state sekaligus.

`database/03_session_state_seed.sql`
- Tidak lagi memiliki tanggung jawab DDL.
- Jika setelah konsolidasi tidak ada seed khusus yang masih relevan, file ini dihapus dari bootstrap dan aset project.

### Aturan Seed Ruleset Default

- Mode `PEMULA` dan `MAHIR` tetap menjadi sumber default untuk endpoint `GET /api/v1/rulesets`, `GET /api/v1/rulesets/{id}`, `GET /api/v1/rulesets/components/defaults`, `GET /api/v1/rulesets/sections`, dan pembuatan session state.
- Nilai baseline utama harus konsisten dengan rulebook dan test yang sudah ada:
  - `actions_per_turn = 2`
  - `starting_cash = 20` untuk `PEMULA`
  - `starting_cash = 10` untuk `MAHIR`
- `component_catalog.gameConfig` akan menjadi translasi digital dari baseline ruleset:
  - `initialCoins` mengikuti `starting_cash`
  - `actionsPerTurn` mengikuti `actions_per_turn`
  - `finishDay = 13`
  - `minPlayers = 3`
  - `maxPlayers = 4`
- Seed `PEMULA` dan `MAHIR` harus memakai bentuk data yang sama agar repository dan controller tidak perlu mencari ruleset khusus lain.

### Aturan Bootstrap

`DatabaseInitialization` sesudah konsolidasi:
- tetap melakukan baseline legacy schema
- tetap menjalankan migration EF
- menjalankan `01_seed_default_rulesets_components.sql`
- menjalankan `02_seed_full_inspection.sql`
- tidak lagi menjalankan `03_session_state_seed.sql` bila file tersebut tidak lagi memiliki tanggung jawab

Urutan ini memisahkan tanggung jawab dengan jelas:
- schema dari `00`
- default ruleset dari `01`
- dataset inspeksi dari `02`

## Dampak ke Aplikasi

### SessionStateRepository

- Query pemilihan ruleset tetap mencari `component_catalog` yang memiliki `gameConfig`, `bahan`, `resep`, `kebutuhan`, `targetKebutuhan`, `tujuanFinansial`, `narasi`, dan `quest`.
- Bedanya, ruleset yang ditemukan sekarang berasal dari default seed `01`, bukan lagi ruleset digital terpisah dari `03`.
- Ini menghilangkan ketergantungan terhadap ruleset `93000000-0000-0000-0000-000000000001`.

### RulesetsController

- Kontrak respons `RulesetSectionsResponse` tidak berubah.
- `GET /api/v1/rulesets/sections` tetap mengambil struktur yang sama, tetapi sumber data menjadi ruleset default yang sudah dikonsolidasikan.

### Ruleset Detail dan Components

- `GET /api/v1/rulesets/{id}` dan `GET /api/v1/rulesets/{id}/components` akan menampilkan satu konfigurasi ruleset default yang lebih lengkap.
- Tidak ada lagi pemisahan antara ruleset “komponen fisik” dan ruleset “digital session state” untuk default system seed.

## Dampak ke Test

Test perlu disesuaikan untuk memverifikasi:

- `03_session_state_seed.sql` tidak lagi memuat DDL session state.
- `DatabaseInitialization` tidak lagi memasang `03_session_state_seed.sql` bila sudah tidak dipakai.
- `01_seed_default_rulesets_components.sql` memuat katalog lengkap yang kompatibel dengan `SessionStateRepository`.
- integration test session state tetap lolos dengan ruleset default hasil konsolidasi.
- test bootstrap database tetap lolos dan tetap memverifikasi minimal dua default ruleset tersedia.

## Langkah Implementasi

1. Tambahkan failing test yang memverifikasi bootstrap tidak lagi merujuk `03_session_state_seed.sql` dan bahwa `01` memuat katalog session state.
2. Pindahkan atau satukan data katalog digital dari `03` ke ruleset default di `01`.
3. Hapus DDL duplikat dari `03`, lalu keluarkan file tersebut dari startup dan project include bila sudah kosong per tanggung jawab.
4. Perbarui test integration yang sebelumnya mengasumsikan ruleset digital khusus.
5. Jalankan test SQL/bootstrap/session state untuk memastikan perilaku tetap benar.

## Risiko dan Mitigasi

- Risiko: format katalog `PEMULA` dan `MAHIR` menjadi terlalu besar dan sulit dipelihara.
  Mitigasi: pertahankan struktur JSON yang konsisten dan verifikasi dengan test tekstual/integration.

- Risiko: perubahan ID ruleset default mematahkan test atau referensi seed lain.
  Mitigasi: pertahankan ID ruleset default existing di `01`; hapus ketergantungan ke ID khusus `03`.

- Risiko: inspeksi dataset `02` bergantung pada asumsi struktur ruleset tertentu.
  Mitigasi: biarkan `02` tetap berdiri sendiri, hanya pastikan tidak ada konflik dengan default seed yang diperluas.

## Non-Goal

- Tidak mengubah kontrak API session state.
- Tidak mengubah struktur tabel session state yang sudah ada di `00_create_schema.sql`.
- Tidak merombak seed inspeksi `02_seed_full_inspection.sql` di luar penyesuaian minimum bila ada test yang perlu disejajarkan.
