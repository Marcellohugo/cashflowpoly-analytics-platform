# Dokumentasi Database Cashflowpoly

## Baseline
- Database: PostgreSQL.
- Baseline implementasi: 18 Juni 2026.
- Skrip kanonis schema: `database/00_create_schema.sql`.
- Seed ruleset default: `database/01_seed_default_rulesets_components.sql`.
- Seed simulasi session/event: `database/02_seed_simulation_sessions_events.sql`.
- Rancangan detail berada di `docs/02-Perancangan/02-02-rancangan-model-data-dan-basis-data.md`.

Dokumen ini adalah referensi root untuk memahami alur database Cashflowpoly. Jika ada perbedaan detail teknis, `database/00_create_schema.sql` menjadi acuan terakhir.

---

## 1. Prinsip Desain Database
1. Database memakai pendekatan event-first.
2. Tabel `events` adalah sumber kebenaran gameplay.
3. State, balance, inventory, cashflow, metric, final score, dan narrative log adalah projection/cache yang dapat dibangun ulang.
4. Identitas akun berada pada `app_users.user_id`.
5. Peserta sesi berada pada `session_participants.session_participant_id`.
6. Sesi mengunci `ruleset_version_id` saat dibuat.
7. Tidak ada aktivasi ruleset per sesi. Aktivasi terjadi pada versi ruleset, lalu session dibuat dengan versi aktif tersebut.
8. Request API ruleset memakai `definition`, lalu API menormalisasi definition ke tabel `ruleset_*`.
9. Semua perubahan gameplay harus berasal dari event valid.
10. Event invalid masuk ke `validation_logs`, bukan ke `events`.

---

## 2. Alur Data Utama
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
1. Instruktur terdaftar pada `app_users`.
2. Instruktur membuat ruleset pada `rulesets`.
3. Sistem membuat versi ruleset pada `ruleset_versions`.
4. Definition ruleset dinormalisasi ke tabel detail `ruleset_*`.
5. Instruktur membuat sesi pada `sessions` dengan `ruleset_version_id` aktif.
6. Instruktur menambahkan akun Player ke `session_participants`.
7. Klien Game/IDN mengirim event ke API.
8. API memvalidasi event terhadap session, participant, ruleset, sequence, dan payload.
9. Event valid masuk ke `events`.
10. Projector memperbarui state/session projection dan metric.
11. API analytics membaca projection dan snapshot.

---

## 3. Daftar Tabel Baseline
| Kelompok | Tabel |
|---|---|
| Baseline | `schema_baseline_versions` |
| Auth | `app_users` |
| Katalog aksi | `actions` |
| Ruleset core | `rulesets`, `ruleset_versions` |
| Ruleset detail | `ruleset_game_settings`, `ruleset_player_ordering_rules`, `ruleset_actions`, `ruleset_game_assets`, `ruleset_ingredients`, `ruleset_orders`, `ruleset_order_requirements`, `ruleset_needs`, `ruleset_need_set_bonuses`, `ruleset_collection_missions`, `ruleset_collection_mission_requirements`, `ruleset_financial_goals`, `ruleset_narratives`, `ruleset_narrative_scenes`, `ruleset_trigger_conditions`, `ruleset_gold_prices`, `ruleset_gold_assets`, `ruleset_rank_points`, `ruleset_tie_breakers`, `ruleset_sharia_loans`, `ruleset_insurance_products`, `ruleset_life_risks` |
| Session core | `sessions`, `session_participants`, `session_states` |
| Session participant projection | `session_participant_balances`, `session_participant_inventory`, `session_participant_need_purchases`, `session_participant_financial_goals`, `session_participant_collection_missions`, `session_participant_action_counters`, `session_participant_gold_holdings`, `session_participant_loans`, `session_participant_insurances`, `session_participant_tie_breakers` |
| Session projection lain | `session_donation_events`, `session_card_positions` |
| Event | `events`, `event_asset_references` |
| Analitika | `event_cashflow_projections`, `session_projection_checkpoints`, `metric_snapshots` |
| Validasi dan hasil akhir | `validation_logs`, `session_final_scores`, `session_final_score_components`, `session_narrative_logs` |
| Audit dan retensi | `security_audit_logs`, `log_retention_policies` |

---

## 4. Identitas dan Auth
### 4.1 `app_users`
Fungsi:
- menyimpan akun login aplikasi,
- membedakan role `INSTRUCTOR` dan `PLAYER`,
- menjadi sumber `user_id` untuk seluruh API.

Kolom penting:
- `user_id`
- `username`
- `display_name`
- `password_hash`
- `role`
- `is_active`
- `created_at`

Aturan:
- `username` unik.
- Password hanya disimpan sebagai hash.
- API `/api/v1/players` membuat atau membaca akun role `PLAYER`.
- Tidak ada tabel profil pemain terpisah pada baseline aktif.

---

## 5. Ruleset
### 5.1 `rulesets`
Fungsi:
- wadah ruleset,
- menyimpan pemilik instruktur,
- membedakan ruleset default dan ruleset instruktur.

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
- `instructor_user_id = null` berarti ruleset default seed/read-only.
- Ruleset default tidak boleh diedit atau dihapus.
- Ruleset yang sudah dipakai session terkunci dan tidak boleh dimutasi.

### 5.2 `ruleset_versions`
Fungsi:
- menyimpan versi ruleset,
- menyimpan status versi,
- menjadi FK untuk session, event, projection, dan analytics.

Kolom penting:
- `ruleset_version_id`
- `ruleset_id`
- `version`
- `status`
- `mode`
- `config_hash`
- `created_by_user_id`
- `created_at`

Aturan:
- Status utama: `DRAFT`, `ACTIVE`, `ARCHIVED`.
- Versi `ACTIVE` tidak boleh dihapus.
- Versi terakhir tidak boleh dihapus lewat endpoint delete version.
- Versi yang sudah dipakai session/event tidak boleh dihapus.
- `config_hash` mencegah definition identik disimpan sebagai versi baru pada ruleset yang sama.

### 5.3 Tabel Detail Ruleset
| Tabel | Fungsi |
|---|---|
| `ruleset_game_settings` | Pengaturan dasar permainan: jumlah aksi, saldo awal, batas pemain, batas inventory, aturan donasi, emas, pinjaman, asuransi, dan tabungan. |
| `ruleset_player_ordering_rules` | Aturan urutan pemain dan fitur hari Jumat/Sabtu/Minggu. |
| `ruleset_actions` | Daftar action valid pada versi ruleset. |
| `ruleset_game_assets` | Registry asset/card-like untuk ingredient, order, need, mission, gold, risk, loan, insurance, dan tie breaker. |
| `ruleset_ingredients` | Detail bahan, harga beli, dan metadata bahan. |
| `ruleset_orders` | Detail order/resep, harga jual, dan poin kebahagiaan. |
| `ruleset_order_requirements` | Kebutuhan bahan untuk order. |
| `ruleset_needs` | Detail kebutuhan primer/sekunder/tersier, harga, dan poin. |
| `ruleset_need_set_bonuses` | Bonus set kebutuhan. |
| `ruleset_collection_missions` | Misi koleksi kebutuhan. |
| `ruleset_collection_mission_requirements` | Syarat item untuk misi koleksi. |
| `ruleset_financial_goals` | Tujuan finansial mode mahir. |
| `ruleset_narratives` | Narasi ruleset. |
| `ruleset_narrative_scenes` | Scene narasi. |
| `ruleset_trigger_conditions` | Kondisi trigger narasi/ruleset. |
| `ruleset_gold_prices` | Harga transaksi emas. |
| `ruleset_gold_assets` | Asset emas. |
| `ruleset_rank_points` | Poin ranking seperti donasi, emas, dan pensiun. |
| `ruleset_tie_breakers` | Data tie breaker. |
| `ruleset_sharia_loans` | Konfigurasi pinjaman syariah. |
| `ruleset_insurance_products` | Produk asuransi dan limit penggunaan. |
| `ruleset_life_risks` | Risiko kehidupan mode mahir. |

---

## 6. Session
### 6.1 `sessions`
Fungsi:
- menyimpan metadata sesi,
- mengunci versi ruleset,
- menyimpan owner instruktur,
- menyimpan status lifecycle sesi.

Kolom penting:
- `session_id`
- `session_name`
- `mode`
- `status`
- `started_at`
- `ended_at`
- `instructor_user_id`
- `ruleset_version_id`
- `is_archived`
- `created_at`

Aturan:
- `ruleset_version_id` wajib.
- Mode session harus cocok dengan mode ruleset version.
- Status utama: `CREATED`, `STARTED`, `ENDED`.
- Session tidak mengganti ruleset version setelah dibuat.

### 6.2 `session_participants`
Fungsi:
- menghubungkan akun role `PLAYER` ke session,
- menyimpan urutan pemain,
- menyimpan nama tampil peserta pada sesi.

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
- DTO state API menyebut `session_participant_id` sebagai `session_player_id`.

### 6.3 `session_states`
Fungsi:
- projection state umum session,
- menyimpan hari, giliran, slot aksi, peserta aktif, dan status game over.

Kolom penting:
- `session_id`
- `state_version`
- `day`
- `weekday`
- `turn_number`
- `action_slot`
- `current_session_player_id`
- `current_action_slot`
- `finish_day`
- `is_game_over`
- `last_event_id`

Aturan:
- Ditulis oleh inisialisasi session dan event projector.
- Tidak ditulis langsung melalui API state write.

---

## 7. Projection Peserta Sesi
Projection peserta adalah cache state turunan per `session_participant_id`.

| Tabel | Fungsi | Kolom kunci |
|---|---|---|
| `session_participant_balances` | Saldo koin, happiness, saving, dan total donasi. | `session_id`, `session_participant_id`, `last_event_id` |
| `session_participant_inventory` | Inventory bahan milik peserta. | `session_id`, `session_participant_id`, `ruleset_game_asset_id` |
| `session_participant_need_purchases` | Riwayat/projection pembelian kebutuhan. | `session_id`, `session_participant_id`, `source_event_id` |
| `session_participant_financial_goals` | Progres tujuan finansial. | `session_id`, `session_participant_id`, `ruleset_game_asset_id` |
| `session_participant_collection_missions` | Status misi koleksi peserta. | `session_id`, `session_participant_id`, `ruleset_collection_mission_id` |
| `session_participant_action_counters` | Jumlah penggunaan action per peserta. | `session_id`, `session_participant_id`, `ruleset_action_id` |
| `session_participant_gold_holdings` | Kepemilikan emas. | `session_id`, `session_participant_id` |
| `session_participant_loans` | Pinjaman syariah aktif/lunas. | `session_id`, `session_participant_id`, `source_event_id` |
| `session_participant_insurances` | Asuransi aktif dan usage limit. | `session_id`, `session_participant_id`, `source_event_id` |
| `session_participant_tie_breakers` | Nilai tie breaker peserta. | `session_id`, `session_participant_id`, `source_event_id` |

Aturan umum:
- Projection berubah karena event valid.
- Projection menyimpan provenance event jika kolom tersedia.
- Projection dapat dibangun ulang dari `events` dan ruleset version.

---

## 8. Projection Session Lain
### 8.1 `session_donation_events`
Fungsi:
- menyimpan projection event donasi,
- mendukung ranking donasi dan skor akhir.

Kolom kunci:
- `session_id`
- `session_participant_id`
- `source_event_id`

### 8.2 `session_card_positions`
Fungsi:
- menyimpan posisi kartu/asset pada sesi,
- mendukung kebutuhan UI atau simulator board.

Kolom kunci:
- `session_id`
- `ruleset_game_asset_id`
- `last_event_id`

---

## 9. Event
### 9.1 `events`
Fungsi:
- menyimpan event gameplay terurut,
- menyimpan payload mentah,
- menjadi sumber replay, audit gameplay, dan analytics.

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
- `action_type`
- `ruleset_version_id`
- `ruleset_action_id`
- `payload`
- `received_at`
- `client_request_id`

Aturan:
- `(session_id, event_id)` unik.
- `(session_id, sequence_number)` unik.
- Event Player wajib membawa `user_id` valid.
- API me-resolve `user_id` ke `session_player_id`.
- Event sistem boleh memiliki `user_id = null`.
- `ruleset_version_id` harus sama dengan session.

### 9.2 `event_asset_references`
Fungsi:
- menghubungkan event dengan asset ruleset yang di-resolve dari payload,
- menyimpan role referensi dan path payload.

Kolom penting:
- `event_asset_reference_id`
- `event_pk`
- `session_id`
- `event_id`
- `ruleset_version_id`
- `ruleset_game_asset_id`
- `reference_role`
- `payload_path`

Manfaat:
- Analytics tidak perlu menebak relasi dari string bebas.
- Replay/audit dapat menelusuri asset yang dipakai event.

---

## 10. Analitika
### 10.1 `event_cashflow_projections`
Fungsi:
- menyimpan transaksi arus kas hasil projection event,
- mempercepat query histori transaksi.

Kolom penting:
- `projection_id`
- `session_id`
- `session_player_id`
- `user_id`
- `event_pk`
- `event_id`
- `timestamp`
- `direction`
- `amount`
- `category`

### 10.2 `session_projection_checkpoints`
Fungsi:
- menyimpan checkpoint proses projection,
- membantu audit dan recovery recompute.

Kolom penting:
- `checkpoint_id`
- `session_id`
- `last_sequence_number`
- `status`
- `projected_at`

### 10.3 `metric_snapshots`
Fungsi:
- menyimpan hasil metrik terhitung,
- mendukung dashboard session, player, dan ruleset analytics.

Kolom penting:
- `metric_snapshot_id`
- `session_id`
- `session_player_id`
- `user_id`
- `ruleset_version_id`
- `metric_name`
- `metric_value`
- `metric_payload`
- `computed_at`

Contoh metrik:
- `cashflow.in.total`
- `cashflow.out.total`
- `cashflow.net.total`
- `donation.total`
- `gold.qty.current`
- `orders.completed.count`
- `inventory.ingredient.total`
- `compliance.primary_need.rate`
- `actions.used.total`
- `rules.violations.count`
- `happiness.points.total`
- `gameplay.raw.variables`
- `gameplay.derived.metrics`

---

## 11. Validasi, Final Score, dan Narrative
### 11.1 `validation_logs`
Fungsi:
- menyimpan request/event invalid,
- menyimpan error code, message, detail, dan trace.

Kolom penting:
- `validation_log_id`
- `session_id`
- `event_id`
- `user_id`
- `error_code`
- `message`
- `details`
- `trace_id`
- `created_at`

Aturan:
- Event invalid tidak masuk ke `events`.
- Log ini dipakai untuk audit validasi dan debugging.

### 11.2 `session_final_scores`
Fungsi:
- menyimpan skor akhir per peserta.

Kolom penting:
- `session_final_score_id`
- `session_id`
- `session_participant_id`
- `user_id`
- `total_score`
- `rank`
- `computed_at`

### 11.3 `session_final_score_components`
Fungsi:
- menyimpan breakdown skor akhir.

Contoh komponen:
- need points,
- need set bonus,
- donation points,
- gold points,
- pension points,
- saving goal points,
- mission penalty,
- loan penalty.

### 11.4 `session_narrative_logs`
Fungsi:
- menyimpan narasi yang muncul selama sesi,
- menyimpan event/condition yang memicu narasi.

Kolom penting:
- `session_narrative_log_id`
- `session_id`
- `session_participant_id`
- `ruleset_narrative_id`
- `ruleset_narrative_scene_id`
- `shown_at`

---

## 12. Audit dan Retensi
### 12.1 `security_audit_logs`
Fungsi:
- menyimpan jejak keamanan,
- mencatat auth failure, forbidden access, rate limit, dan event operasional keamanan lain.

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
- `detail`

### 12.2 `log_retention_policies`
Fungsi:
- menyimpan aturan retensi log,
- menjadi acuan cleanup operasional.

Kolom penting:
- `policy_id`
- `log_type`
- `retention_days`
- `is_enabled`
- `created_at`
- `updated_at`

---

## 13. Relasi Penting
| Relasi | Makna |
|---|---|
| `rulesets.ruleset_id -> ruleset_versions.ruleset_id` | Satu ruleset punya banyak versi. |
| `ruleset_versions.ruleset_version_id -> sessions.ruleset_version_id` | Session mengunci versi ruleset. |
| `sessions.session_id -> session_participants.session_id` | Satu session punya banyak peserta. |
| `app_users.user_id -> session_participants.user_id` | Peserta session adalah akun Player. |
| `sessions.session_id -> events.session_id` | Event berada dalam session. |
| `session_participants.session_participant_id -> events.session_player_id` | Event Player mengacu peserta session. |
| `app_users.user_id -> events.user_id` | Event Player mengacu akun Player. |
| `ruleset_versions.ruleset_version_id -> events.ruleset_version_id` | Event membawa versi ruleset yang dipakai. |
| `events.event_pk -> event_asset_references.event_pk` | Event dapat mengacu banyak asset ruleset. |
| `events.event_pk -> event_cashflow_projections.event_pk` | Projection cashflow berasal dari event. |
| `events.event_id -> projection.last_event_id/source_event_id` | Projection menyimpan provenance event. |
| `sessions.session_id -> metric_snapshots.session_id` | Metric dihitung per session. |

---

## 14. Index Penting
Index pada schema mendukung pola query utama:
- filter akun berdasarkan role dan status aktif,
- list ruleset milik instruktur,
- list ruleset version berdasarkan status/mode,
- list session berdasarkan owner/status/created date,
- lookup participant berdasarkan `user_id`,
- event replay berdasarkan session dan sequence,
- event timeline berdasarkan session dan timestamp,
- event filter berdasarkan action type,
- transaction history berdasarkan session, user, dan timestamp,
- metric snapshot terbaru berdasarkan session, user, dan metric name,
- audit log berdasarkan event type, user, dan waktu.

---

## 15. Alur Lifecycle Data
### 15.1 Membuat Ruleset
1. API menerima `POST /api/v1/rulesets`.
2. API memvalidasi `definition`.
3. API menulis `rulesets`.
4. API menulis `ruleset_versions`.
5. API menormalisasi definition ke tabel detail `ruleset_*`.

### 15.2 Membuat Session
1. API menerima `POST /api/v1/sessions`.
2. API memastikan `ruleset_version_id` ada dan aktif.
3. API menulis `sessions`.
4. API menginisialisasi `session_states`.
5. Jika peserta awal dikirim, API menulis `session_participants` dan projection awal.

### 15.3 Menambahkan Player
1. API menerima `POST /api/v1/sessions/{sessionId}/players`.
2. API mencari akun Player lewat `user_id` atau `username`.
3. API menulis `session_participants`.
4. API menginisialisasi projection peserta.

### 15.4 Mengirim Event
1. API menerima event.
2. API memvalidasi auth, scope, session, sequence, idempotency, participant, ruleset, action, dan payload.
3. API menulis event ke `events`.
4. API menulis asset reference jika payload mengacu asset ruleset.
5. Projector memperbarui session state dan participant projection.
6. API memperbarui cashflow projection dan metric snapshot.

### 15.5 Event Invalid
1. API menolak event.
2. API menulis alasan ke `validation_logs`.
3. Tidak ada perubahan pada `events` atau projection gameplay.

### 15.6 Mengakhiri Session
1. API menerima `POST /api/v1/sessions/{sessionId}/end`.
2. API mengubah status session menjadi `ENDED`.
3. API menghitung snapshot/final score yang relevan.
4. Data session menjadi basis laporan dan analitika akhir.

---

## 16. Query Operasional Contoh
List session milik instruktur:

```sql
select session_id, session_name, mode, status, created_at
from sessions
where instructor_user_id = :instructor_user_id
  and is_archived = false
order by created_at desc;
```

Replay event session:

```sql
select sequence_number, action_type, user_id, payload, received_at
from events
where session_id = :session_id
order by sequence_number;
```

Transaction history satu Player:

```sql
select "timestamp", direction, amount, category
from event_cashflow_projections
where session_id = :session_id
  and user_id = :user_id
order by "timestamp" desc;
```

Metric snapshot terbaru:

```sql
select metric_name, metric_value, metric_payload, computed_at
from metric_snapshots
where session_id = :session_id
  and user_id = :user_id
order by computed_at desc;
```

Audit security terbaru:

```sql
select occurred_at, event_type, outcome, username, role, method, path, status_code
from security_audit_logs
order by occurred_at desc
limit 100;
```

---

## 17. Checklist Konsistensi Database
Database dianggap sinkron dengan API jika:
1. Tidak ada tabel profil pemain lama pada schema aktif.
2. Resource Player API mengarah ke `app_users` role `PLAYER`.
3. Event memakai `user_id` dan disimpan dengan hasil resolusi `session_player_id`.
4. Session menyimpan `ruleset_version_id`.
5. Ruleset API memakai `definition` dan hasilnya masuk ke tabel `ruleset_*`.
6. State gameplay berubah melalui event projector, bukan PUT state manual.
7. Cashflow dan metric analytics membaca projection/snapshot, bukan menghitung ulang seluruh event pada setiap request normal.
8. Event invalid masuk ke `validation_logs`.
9. Security event masuk ke `security_audit_logs`.
10. Retensi log dikendalikan oleh `log_retention_policies`.

