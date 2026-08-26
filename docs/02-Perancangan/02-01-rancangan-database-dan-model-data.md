# Rancangan Model Data dan Basis Data (PostgreSQL)
## Sistem Informasi Dasbor Analitika & Manajemen Ruleset Cashflowpoly

### Dokumen
- Nama dokumen: Rancangan Model Data dan Basis Data
- Versi: 2.1
- Tanggal: 11 Juli 2026
- Penyusun: Marco Marcello Hugo

> Baseline kanonis schema berada pada `database/00_create_schema.sql`, kemudian
> diperbarui oleh migrasi immutable berurutan di `database/migrations`.
> Riwayat, nama, checksum SHA-256, dan waktu penerapan dicatat pada
> `schema_history`. Jika ada perbedaan detail teknis, baseline beserta seluruh
> migrasi yang sudah diterapkan menjadi acuan terakhir.

---

## 1. Tujuan Dokumen
Dokumen ini mendefinisikan model data PostgreSQL untuk:
1. akun pengguna dan peserta sesi,
2. ruleset dan versi ruleset yang dinormalisasi,
3. sesi permainan dan projection state gameplay,
4. event sebagai sumber kebenaran,
5. proyeksi cashflow, metric snapshot, final score, narrative log, audit, dan retensi log.

Sistem tidak memakai ORM sebagai sumber desain schema. Aplikasi memakai EF Core
mapping untuk akses data, tetapi struktur database tetap didefinisikan oleh
skrip SQL baseline.

---

## 2. Prinsip Rancangan
### 2.1 Event-first
Tabel `events` adalah sumber kebenaran gameplay. Projection seperti saldo,
inventory, gold holding, pinjaman, asuransi, final score, narrative log, dan
metric snapshot adalah cache yang dapat dibangun ulang dari event dan ruleset.

### 2.2 Identitas pengguna dan peserta sesi
Identitas akun aplikasi berada pada `app_users.user_id`.

Saat akun role `PLAYER` masuk ke sesi, sistem membuat baris pada
`session_participants`. ID peserta sesi (`session_participant_id`) juga tampil
di DTO state sebagai `session_player_id`.

### 2.3 Ruleset version lock
Sesi menyimpan `ruleset_version_id` langsung pada `sessions`. Tidak ada endpoint
atau tabel aktivasi ruleset per sesi. Session dibuat dengan versi ruleset
berstatus `ACTIVE`, lalu event wajib membawa `ruleset_version_id` yang sama.

### 2.4 Normalisasi ruleset
Request API ruleset memakai field `definition`. API menormalisasi definition ke
tabel `ruleset_*` per domain: settings, action, asset, need, order, mission,
financial goal, narrative, gold, loan, insurance, life risk, dan scoring.

### 2.5 Provenance projection
Projection menyimpan referensi event seperti `last_event_id` atau
`source_event_id` agar setiap perubahan state dapat ditelusuri. Nilai tersebut
bukan histori kedua; histori tetap berasal dari `events`.

---

## 3. Alur Data Inti
Alur relasi inti:

```text
app_users
  -> rulesets
  -> ruleset_versions
  -> sessions
  -> session_participants
  -> events
  -> event_asset_references
  -> event_cashflow_projections
  -> metric_snapshots
  -> session_final_scores
```

Alur operasional:
1. Instruktur memiliki akun pada `app_users`.
2. Instruktur membuat ruleset. Sistem membuat `rulesets` dan `ruleset_versions`.
3. API menormalisasi `definition` ke tabel `ruleset_*`.
4. Instruktur membuat sesi dengan `ruleset_version_id` aktif.
5. Instruktur menambahkan akun Player ke sesi sebagai `session_participants`.
6. Klien Game/IDN mengirim event dengan `session_id`, `user_id`, `action_type`,
   `ruleset_version_id`, dan payload domain.
7. API memvalidasi token, scope, sesi, peserta, urutan, idempotensi, ruleset,
   dan payload.
8. API menyimpan event valid, asset reference, projection, dan metric snapshot.
9. API tidak menyimpan event invalid ke `events`; `validation_logs` hanya menerima metadata penolakan berupa status, kode error, dan `trace_id`, tanpa payload gameplay.

---

## 4. Daftar Tabel Baseline
Tabel pada `database/00_create_schema.sql`:

| Kelompok | Tabel |
|---|---|
| Baseline | `schema_baseline_versions` |
| Migrasi | `schema_history` |
| Auth | `app_users` (`is_demo` menandai akun Seed 2) |
| Katalog aksi | `actions` |
| Ruleset core | `rulesets`, `ruleset_versions` |
| Ruleset detail | `ruleset_game_settings`, `ruleset_player_ordering_rules`, `ruleset_actions`, `ruleset_game_assets`, `ruleset_ingredients`, `ruleset_orders`, `ruleset_order_requirements`, `ruleset_needs`, `ruleset_need_set_bonuses`, `ruleset_collection_missions`, `ruleset_collection_mission_requirements`, `ruleset_financial_goals`, `ruleset_narratives`, `ruleset_narrative_scenes`, `ruleset_trigger_conditions`, `ruleset_gold_prices`, `ruleset_gold_assets`, `ruleset_rank_points`, `ruleset_tie_breakers`, `ruleset_sharia_loans`, `ruleset_insurance_products`, `ruleset_life_risks` |
| Session core | `sessions`, `session_participants`, `session_setup_revisions`, `session_states` |
| Session participant projection | `session_participant_balances`, `session_participant_inventory`, `session_participant_need_purchases`, `session_participant_financial_goals`, `session_participant_collection_missions`, `session_participant_action_counters`, `session_participant_gold_holdings`, `session_participant_loans`, `session_participant_insurances`, `session_participant_tie_breakers` |
| Session projection lain | `session_donation_events`; `session_card_positions` hanya data legacy |
| Event | `events`, `event_asset_references` |
| Analitika | `event_cashflow_projections`, `session_projection_checkpoints`, `metric_snapshots` |
| Validasi dan hasil akhir | `validation_logs`, `session_final_scores`, `session_final_score_components`, `session_narrative_logs` |
| Audit dan retensi | `security_audit_logs`, `log_retention_policies` |

---

## 5. Tabel Identitas dan Auth
### 5.1 `app_users`
Fungsi:
- menyimpan akun login aplikasi,
- menyimpan role `INSTRUCTOR` atau `PLAYER`,
- menyimpan nama tampil dan password hash.

Kolom penting:
- `user_id`
- `username`
- `display_name`
- `password_hash`
- `role`
- `is_active`
- `is_demo`
- `created_at`

Aturan:
- `username` unik.
- Akun Seed 2 ditandai `is_demo=true` tanpa mengubah hak akses berdasarkan role.
- Password tidak pernah dikembalikan API.
- Resource API `/api/v1/players` membuat atau membaca akun role `PLAYER`.

---

## 6. Tabel Ruleset
### 6.1 `rulesets`
Fungsi:
- wadah ruleset,
- pemilik data instruktur,
- status arsip ruleset.

Kolom penting:
- `ruleset_id`
- `name`
- `description`
- `instructor_user_id`
- `created_by_user_id`
- `is_archived`
- `archived_at`
- `created_at`

Aturan:
- `instructor_user_id = null` menandai ruleset default seed/read-only.
- Ruleset default tidak dapat diedit atau dihapus.
- Ruleset yang sudah dipakai sesi dianggap terkunci oleh session.

### 6.2 `ruleset_versions`
Fungsi:
- menyimpan versi ruleset,
- menyimpan status versi,
- menjadi FK utama untuk event, sesi, projection, dan metric.

Kolom penting:
- `ruleset_version_id`
- `ruleset_id`
- `version`
- `status` (`DRAFT`, `ACTIVE`, `ARCHIVED`)
- `mode` (`PEMULA`, `MAHIR`)
- `config_hash`
- `created_by_user_id`
- `created_at`

Aturan:
- Satu ruleset memiliki banyak versi.
- Versi `ACTIVE` tidak boleh dihapus.
- Versi yang sudah dipakai sesi/event tidak boleh dihapus.
- Version hash mencegah duplikasi definition yang identik pada ruleset sama.

### 6.3 Tabel detail ruleset
Tabel detail menyimpan hasil normalisasi `definition`.

| Tabel | Fungsi |
|---|---|
| `ruleset_game_settings` | Pengaturan dasar, saldo awal, jumlah aksi, batas pemain. |
| `ruleset_player_ordering_rules` | Aturan urutan pemain dan mode pengurutan. |
| `ruleset_actions` | Action yang valid pada versi ruleset. |
| `ruleset_game_assets` | Registry asset/card-like seperti ingredient, order, need, risk, gold, gold price, tie breaker. |
| `ruleset_ingredients` | Detail asset bahan. |
| `ruleset_orders`, `ruleset_order_requirements` | Detail order dan bahan yang dibutuhkan. |
| `ruleset_needs`, `ruleset_need_set_bonuses` | Detail kebutuhan dan bonus set kebutuhan. |
| `ruleset_collection_missions`, `ruleset_collection_mission_requirements` | Misi koleksi dan syarat kebutuhan. |
| `ruleset_financial_goals` | Tujuan keuangan mode mahir. |
| `ruleset_narratives`, `ruleset_narrative_scenes`, `ruleset_trigger_conditions` | Narasi dan kondisi trigger. |
| `ruleset_gold_prices`, `ruleset_gold_assets` | Harga dan aset emas. |
| `ruleset_rank_points` | Poin ranking donasi, emas, dana pensiun, atau domain skor lain. |
| `ruleset_tie_breakers` | Kartu/angka tie breaker. |
| `ruleset_sharia_loans` | Parameter pinjaman syariah. |
| `ruleset_insurance_products` | Produk asuransi dan premi. |
| `ruleset_life_risks` | Risiko kehidupan mode mahir. |

---

## 7. Tabel Session
### 7.1 `sessions`
Fungsi:
- menyimpan metadata sesi,
- mengunci ruleset version,
- menyimpan owner instruktur dan status sesi.

Kolom penting:
- `session_id`
- `session_name`
- `ruleset_version_id`
- `mode`
- `status` (`CREATED`, `STARTED`, `ENDED`)
- `player_count`
- `started_at`
- `ended_at`
- `instructor_user_id`
- `is_archived`
- `archived_at`
- `created_at`

Aturan:
- `ruleset_version_id` wajib.
- Mode session harus sama dengan mode `ruleset_versions`.
- Session hanya dapat dimulai jika jumlah peserta sesuai batas ruleset.

### 7.2 `session_participants`
Fungsi:
- menghubungkan akun role `PLAYER` ke sesi,
- menyimpan urutan pemain dan nama tampil pada sesi.

Kolom penting:
- `session_participant_id`
- `session_id`
- `user_id`
- `player_order_no`
- `player_name`
- `joined_at`

Aturan:
- Kombinasi `(session_id, user_id)` unik.
- Kombinasi `(session_id, player_order_no)` unik.
- DTO state menyebut `session_participant_id` sebagai `session_player_id`.

### 7.3 `session_setup_revisions`
Fungsi:
- menyimpan riwayat revisi pembagian kartu fisik yang dikirim IDN;
- menyediakan revisi terbaru sebagai sumber event setup ketika sesi dimulai;
- menjaga retry setup tetap idempoten.

Kolom penting:
- `session_id`
- `revision`
- `ruleset_version_id`
- `client_request_id`
- `setup_json`
- `saved_at`
- `locked_at`
- `created_by_user_id`

Aturan:
- Satu sesi dapat memiliki beberapa revisi selama status masih `CREATED`.
- Revisi pertama mengunci peserta dan ruleset; revisi berikutnya hanya dapat memperbaiki pembagian untuk peserta dan ruleset yang sama.
- Start mengunci revisi terbaru dengan mengisi `locked_at`; setelah itu setup tidak dapat diubah.
- `client_request_id` unik per Instruktur.
- Retry dengan `client_request_id` dan payload identik mengembalikan revisi yang sama; payload berbeda menghasilkan konflik.
- Payload pemain wajib berupa array yang tidak kosong.
- Validasi peserta, kode ruleset, mode, dan stok kartu dilakukan API sebelum insert.

### 7.4 `session_states`
Fungsi:
- projection state umum sesi,
- menyimpan hari, giliran, slot aksi, dan peserta aktif.

Kolom penting:
- `session_id`
- `day`
- `weekday`
- `turn_number`
- `action_slot`
- `current_session_player_id`
- `current_action_slot`
- `action_slots_left`
- `finish_day`
- `phase`
- `is_game_over`
- `state_version`
- `last_event_id`
- `ui_state_json`

Aturan:
- Ditulis oleh projector event atau inisialisasi session.
- `PUT /api/v1/sessions/{sessionId}/state` tidak mengubah state dan selalu
  mengembalikan `410 STATE_WRITE_DISABLED`.

---

## 8. Projection Peserta Sesi
Projection peserta menyimpan state turunan per peserta:

| Tabel | Fungsi |
|---|---|
| `session_participant_balances` | Koin, happiness, saving, total donasi. |
| `session_participant_inventory` | Inventory bahan berbasis asset registry. |
| `session_participant_need_purchases` | Pembelian kebutuhan dan poin kebutuhan. |
| `session_participant_financial_goals` | Progres tujuan keuangan. |
| `session_participant_collection_missions` | Status misi koleksi. |
| `session_participant_action_counters` | Penggunaan aksi per ruleset action. |
| `session_participant_gold_holdings` | Kepemilikan emas hasil setup, beli/jual reguler, dan jual darurat; menjadi sumber validasi kuantitas. |
| `session_participant_loans` | Pinjaman syariah aktif/lunas; setiap kartu memakai `loan_instance_id` unik. |
| `session_participant_insurances` | Polis, status `ACTIVE/INACTIVE`, dan `remaining_uses`. |
| `session_participant_tie_breakers` | Nilai tie breaker peserta. |

Aturan umum:
- Semua tabel membawa `session_id` dan `session_participant_id`.
- Perubahan projection harus berasal dari event valid.
- Projection menyimpan provenance event jika kolom tersedia.
- Seorang peserta dapat memiliki beberapa instance produk pinjaman yang sama selama setiap `loan_instance_id` unik dan total instance `ACTIVE` pada sesi tidak melewati `card_qty` katalog.
- Update penggunaan asuransi dan pembuatan offset dilakukan dalam transaksi event yang sama; penggunaan hanya valid saat polis aktif dan sisa penggunaan positif.

---

## 9. Event dan Asset Reference
### 9.1 `events`
Fungsi:
- menyimpan log event terurut,
- menyimpan payload mentah,
- menjadi sumber replay dan audit gameplay.

Kolom penting:
- `event_pk`
- `event_id`
- `session_id`
- `session_player_id`
- `user_id`
- `actor_type`
- `timestamp`
- `day_index`
- `weekday`
- `turn_number`
- `action_slot`
- `sequence_number`
- `ruleset_action_id`
- `action_type`
- `ruleset_version_id`
- `payload_version`
- `payload`
- `received_at`
- `client_request_id`

Aturan:
- `(session_id, event_id)` unik untuk idempotensi.
- `(session_id, sequence_number)` unik untuk keterurutan.
- Event Player wajib memiliki `user_id` dan peserta sesi yang valid.
- Event sistem memakai `user_id = null` dan dapat memakai `session_player_id = null`.
- `ruleset_version_id` harus cocok dengan session.

### 9.2 `event_asset_references`
Fungsi:
- menghubungkan event dengan asset ruleset yang di-resolve dari payload,
- menyimpan role referensi seperti target, requirement, price, risk, atau source.

Kolom penting:
- `event_asset_reference_id`
- `session_id`
- `event_id`
- `ruleset_version_id`
- `ruleset_game_asset_id`
- `reference_role`
- `payload_path`
- `created_at`

Manfaat:
- Analitika tidak perlu menebak relasi dari string bebas pada JSON.
- Replay dan audit dapat menelusuri asset yang dipakai event.

### 9.3 `session_card_positions` (legacy)
Tabel ini dipertahankan agar data historis lama tetap dapat dibaca. Backend baru tidak membentuk, mengisi, atau memvalidasi deck/pasar virtual dari tabel ini. Kepemilikan pemain yang dapat divalidasi berasal dari setup fisik yang dikonfirmasi dan event gameplay sah. Event legacy deck/pasar tidak memengaruhi proyeksi atau analitik baru.

Nomor Tie Breaker tetap unik per versi ruleset dan satu kartu Misi Koleksi tidak dapat diberikan kepada dua peserta dalam sesi yang sama.

---

## 10. Analitika dan Snapshot
### 10.1 `event_cashflow_projections`
Fungsi:
- menyimpan transaksi arus kas hasil projection event,
- mempercepat query histori transaksi.

Kolom penting:
- `projection_id`
- `session_id`
- `user_id`
- `event_pk`
- `event_id`
- `projection_order`
- `timestamp`
- `direction`
- `amount`
- `category`
- `counterparty`
- `reference`
- `note`

Aturan:
- Baris projection wajib merujuk event valid.
- Query endpoint transaksi memakai `userId` sebagai filter opsional.
- Risiko kehidupan pemain berarah `OUT` dianggap pending selama belum ada projection kategori `RISK_LIFE` yang merujuk `risk_event_id` tersebut.
- `BayarRisiko` membuat `RISK_LIFE OUT`. Penyelesaian asuransi membuat pasangan `INSURANCE_OFFSET IN` dan `RISK_LIFE OUT` dengan nominal sama.
- Penggunaan asuransi hanya direkam sebagai event `Asuransi` dengan `risk_event_id`; proyeksi mengunci dan mengurangi tepat satu polis `ACTIVE` dengan `remaining_uses > 0` pada transaksi yang sama.
- `GunakanOpsiDarurat` hanya memproyeksikan `SELL_NEED`, `SELL_GOLD`, atau `TAKE_SHARIA_LOAN` berdasarkan nominal yang dihitung server.

### 10.2 `session_projection_checkpoints`
Fungsi:
- menyimpan checkpoint replay/projection,
- membantu rebuild projection secara terukur.

Kolom penting:
- `session_id` (PK)
- `last_sequence_number`
- `last_event_id`
- `projected_at`
- `status`
- `rebuild_started_at`
- `rebuild_completed_at`
- `error_message`
- `metadata_json`

### 10.3 `metric_snapshots`
Fungsi:
- menyimpan hasil metrik numeric atau JSON,
- menjadi sumber utama dashboard.

Kolom penting:
- `metric_snapshot_id`
- `session_id`
- `user_id`
- `session_player_id`
- `computed_at`
- `metric_name`
- `metric_value_numeric`
- `metric_value_text`
- `metric_value_boolean`
- `metric_payload_json`
- `ruleset_version_id`
- `last_event_id`

Aturan:
- Snapshot level sesi memakai `user_id = null` dan `session_player_id = null`.
- Snapshot level Player memakai `user_id` akun Player dan dapat membawa
  `session_player_id`.
- Gameplay JSON memakai nama `gameplay.raw.variables` dan
  `gameplay.derived.metrics`; response API gameplay menyajikannya sebagai
  kelompok `economy`, `progress`, `score`, dan `compliance`.

---

## 11. Validasi, Skor, Narrative, dan Audit
### 11.1 `validation_logs`
Fungsi:
- menyimpan metadata operasional event yang ditolak tanpa payload gameplay,
- membantu korelasi masalah melalui kode error dan `trace_id`; data ini bukan metrik domain atau pelanggaran permainan.

Kolom penting:
- `validation_log_id`
- `session_id`
- `ruleset_version_id`
- `event_id`
- `status_code`
- `trace_id`
- `error_code`
- `error_message`
- `created_at`

Kolom legacy `raw_payload_json` dan `details_json` tetap ada untuk kompatibilitas schema, tetapi selalu dikosongkan menjadi objek JSON dan tidak boleh diisi payload gameplay.

### 11.2 `session_final_scores`
Fungsi:
- menyimpan skor akhir per peserta.

Kolom penting:
- `session_final_score_id`
- `session_id`
- `session_participant_id`
- `total_points`
- `rank_no`
- `tie_breaker_number`
- `has_unpaid_loan`
- `computed_at`
- `source_event_id`

### 11.2b `session_final_score_components`
Fungsi:
- menyimpan komponen skor agar hasil akhir dapat diaudit.

Kolom penting:
- `session_final_score_component_id`
- `session_id`
- `session_participant_id`
- `session_final_score_id`
- `component_code`
- `points`
- `source_event_id`
- `created_at`

Aturan:
- Skor akhir dihitung saat session diakhiri atau saat recompute.
- Komponen skor menyimpan sumber/provenance yang relevan.

### 11.3 `session_narrative_logs`
Fungsi:
- menyimpan hasil trigger narrative dari `ruleset_narratives` dan
  `ruleset_narrative_scenes`.

Kolom penting:
- `narrative_log_id`
- `session_id`
- `session_participant_id`
- `ruleset_version_id`
- `ruleset_narrative_id`
- `ruleset_narrative_scene_id`
- `source_event_id`
- `shown_at`
- `day`
- `action_slot`
- `payload_json`

Catatan:
- Narrative bukan script engine. Trigger narrative dievaluasi dari event/action
  terstruktur.

### 11.4 `security_audit_logs`
Fungsi:
- menyimpan audit login/register, forbidden, challenge, rate-limit, dan event
  keamanan lain.

Kolom penting:
- `security_audit_log_id`
- `occurred_at`
- `trace_id`
- `event_type`
- `outcome`
- `user_id`
- `username`
- `role`
- `ip_address`
- `user_agent`
- `method`
- `path`
- `status_code`
- `detail_json`

### 11.5 `log_retention_policies`
Fungsi:
- menyimpan kebijakan retensi log per kategori,
- menjadi dasar housekeeping log pada environment production.

Kolom penting:
- `table_name` (PK)
- `retention_days`
- `created_at`
- `updated_at`

---

## 12. Strategi Query Dashboard
| Kebutuhan | Sumber utama |
|---|---|
| Daftar sesi | `sessions` dengan scope `instructor_user_id` atau `session_participants.user_id`. |
| Detail sesi | `sessions`, `metric_snapshots`, `events`, `ruleset_versions`. |
| Daftar Player sesi | `session_participants`, `app_users`, `metric_snapshots`. |
| Histori event | `events` berdasarkan `session_id` dan `sequence_number`. |
| Histori transaksi | `event_cashflow_projections` berdasarkan `session_id` dan opsional `user_id`. |
| Gameplay Player | `metric_snapshots` berdasarkan `session_id`, `user_id`, dan metric gameplay. |
| Ruleset detail | `rulesets`, `ruleset_versions`, dan tabel `ruleset_*`. |
| Audit keamanan | `security_audit_logs`. |

---

## 13. Aturan Integritas
1. Event idempotent melalui unique `(session_id, event_id)`.
2. Event terurut melalui unique `(session_id, sequence_number)`.
3. Peserta sesi unik melalui `(session_id, user_id)`.
4. Urutan peserta unik melalui `(session_id, player_order_no)`.
5. Semua tabel `ruleset_*` membawa `ruleset_version_id`.
6. Semua projection peserta membawa `session_id` dan `session_participant_id`.
7. Session, event, projection, dan metric tidak boleh merujuk ruleset version yang hilang.
8. Ruleset default dan ruleset yang terkunci sesi tidak boleh dihapus melalui API.

---

## 14. Seed dan Bootstrap
Startup API memastikan:
1. `database/00_create_schema.sql` terpasang,
2. `database/01_seed_default_rulesets_components.sql` terpasang.

Data simulasi manual berada pada:
- `database/02_seed_simulation_sessions_events.sql`

File simulasi tidak di-bootstrap otomatis saat startup API.

---

## 15. Checklist Konsistensi
Model data dianggap sinkron dengan implementasi jika:
1. akun Player memakai `app_users.user_id`,
2. peserta sesi memakai `session_participants.session_participant_id`,
3. event memakai `user_id` dan `session_player_id`,
4. sesi menyimpan `ruleset_version_id`,
5. payload ruleset API memakai `definition`,
6. event invalid masuk `validation_logs`,
7. asset reference event masuk `event_asset_references`,
8. dashboard membaca projection dan `metric_snapshots`, bukan menghitung ulang dari UI.

---

## 16. Riwayat Normalisasi & Keputusan Desain (Design History)

Bagian ini mencatat latar belakang perancangan normalisasi database dan pemangkasan kompleksitas menuju skema *event-first* aktif.

### 16.1 Mengapa Desain Event-First Dipilih?
- **Events sebagai Single Source of Truth**: Seluruh keadaan (*state*) sesi permainan, saldo koin, kepemilikan aset, pinjaman, dan metrik analitik dapat dibangun ulang sewaktu-waktu dari tabel `events`.
- **Auditability**: Mencegah ketidakpastian manipulasi state secara langsung. State mutasi langsung via `PUT /state` dinonaktifkan (`410 STATE_WRITE_DISABLED`).

### 16.2 Konsolidasi dan Reduksi Tabel
Untuk merapikan model data dan meminimalkan permukaan validasi, beberapa konsep digabung atau dipecah:
1. **Pemisahan Aset Peserta Polymorphic**: Aset emas, pinjaman, dan asuransi dipisah menjadi tabel mandiri:
   - `session_participant_gold_holdings`
   - `session_participant_loans`
   - `session_participant_insurances`
   Langkah ini menghindari penggunaan kolom *polymorphic* dengan nilai-nilai nullable yang menyulitkan integritas basis data.
2. **Gameplay Asset Registry**: Seluruh aset permainan kartu/fisik (bahan masakan, pesanan, kebutuhan dasar/sekunder/tersier) didaftarkan di bawah tabel terpusat `ruleset_game_assets` untuk mempermudah referensi silang.
3. **Penyederhanaan Konsep Inventory**: Konsep inventarisasi bahan makanan yang terpecah-pecah digabung menjadi tabel proyeksi inventaris tunggal `session_participant_inventory`.
4. **Penyelesaian Aksi (Action Resolution)**: Seluruh aksi permainan dirujuk melalui `ruleset_actions.ruleset_action_id` sehingga penamaan string bebas pada payload JSON event dapat divalidasi ke katalog DDL.
5. **Status Risiko tanpa Tabel Tambahan**: Risiko `OUT` pending direpresentasikan oleh event `RisikoKehidupan` yang belum memiliki projection penyelesaian `RISK_LIFE`. Pendekatan ini menjaga replay deterministik tanpa tabel status kedua.

### 16.3 Fitur yang Dihilangkan untuk Efisiensi
1. **Penghapusan Quest**: Konsep Quest dipangkas keluar dari MVP. Target pencapaian pemain direpresentasikan secara penuh oleh Misi Koleksi (*Collection Mission*) dan Tujuan Keuangan (*Financial Goal*).
2. **Penghapusan Script Engine**: Mesin eksekusi skrip dinonaktifkan karena:
   - Menduplikasi aturan yang sudah dapat dinyatakan sebagai kondisi event.
   - Memperbesar celah keamanan eksekusi kode dinamis.
   - Menyulitkan replay event yang bersifat deterministik dan audit analitis.

### 16.4 Aturan Integritas Tambahan
- Seluruh tabel `ruleset_*` wajib memuat `ruleset_version_id` untuk mencegah percampuran definisi aturan antarversi.
- Foreign Key (FK) event menggunakan gabungan scope komposit `(session_id, event_id)` karena nilai ID event unik per sesi permainan.
- Seluruh data pemeringkatan (*ranking*) dashboard dihitung secara dinamis melalui query analitik, bukan disimpan dalam tabel status baru yang redundan.
