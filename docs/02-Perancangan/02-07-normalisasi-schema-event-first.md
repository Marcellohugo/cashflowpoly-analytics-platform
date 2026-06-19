# Normalisasi Schema Event-First Cashflowpoly

Dokumen ini menjelaskan baseline schema yang berlaku. DDL kanonis berada di
`database/00_create_schema.sql`; seed ruleset berada di
`database/01_seed_default_rulesets_components.sql`.

## Prinsip Arsitektur

1. `events` adalah satu-satunya sumber kebenaran aksi gameplay.
2. State sesi, balance, inventory, asset peserta, narrative, score, dan
   metric snapshot adalah projection/cache yang dapat dibangun ulang.
3. Kode yang mudah dibaca tetap diterima pada kontrak API, tetapi server
   me-resolve kode tersebut menjadi UUID FK sebelum menyimpan event.
4. Tidak ada action log kedua. Timeline dibaca dari `events`.
5. JSON dipakai untuk payload mentah event dan metadata tambahan, bukan untuk
   menggantikan relasi inti yang dapat dinormalisasi.

## Identitas Akun dan Peserta

- Akun aplikasi berada pada `app_users.user_id`.
- Role `PLAYER` adalah akun pada `app_users`, bukan profil pemain pada tabel
  terpisah.
- Peserta sesi berada pada `session_participants.session_participant_id`.
- DTO state menyebut peserta sesi sebagai `session_player_id`.
- Event aksi Player membawa `user_id`; API me-resolve nilai tersebut ke
  `session_player_id` sebelum event dan projection disimpan.

## Alur Event ke Projection

1. Session dibuat dengan satu `ruleset_version_id` yang dikunci langsung pada
   tabel `sessions`.
2. Klien mengirim `action_type`, `ruleset_version_id`, dan payload.
3. API memvalidasi sesi, peserta, urutan, idempotensi, dan kesesuaian ruleset
   session.
4. API me-resolve action ke `ruleset_actions.ruleset_action_id`.
5. Kode asset penting pada payload di-resolve ke
   `ruleset_game_assets.ruleset_game_asset_id`.
6. Event valid dan `event_asset_references` disimpan dalam transaksi yang sama.
   Event invalid tidak masuk `events`; payload mentahnya dicatat di
   `validation_logs.raw_payload_json`.
7. `SessionEventProjector` memperbarui projection session secara sinkron.
8. Dashboard membaca projection dan `metric_snapshots`; replay tetap dimulai
   dari `events`.

Semua projection mutable menyimpan `last_event_id` atau `source_event_id`.
Nilai tersebut adalah watermark provenance, bukan pengganti histori event.

## Gameplay Asset Registry

`ruleset_game_assets` hanya menyimpan identitas gameplay asset/card-like:

- `ruleset_version_id`
- `asset_type`
- `asset_code`
- `display_name`
- status dan urutan tampilan

Tipe asset yang masuk registry adalah `INGREDIENT`, `ORDER`, `NEED`, `RISK`,
`GOLD`, `GOLD_PRICE`, dan `TIE_BREAKER`. Collection mission, financial goal,
loan, insurance, quest, dan narrative tidak masuk registry. Collection mission,
financial goal, loan, dan insurance memiliki tabel ruleset sendiri dengan PK/FK
langsung.

`event_asset_references` menghubungkan payload event dengan asset UUID dan
menyimpan peran referensi, misalnya `TARGET`, `REQUIREMENT`, atau `PRICE`.
Dengan ini analitik tidak perlu menebak relasi dari string bebas di JSON.

## Session Projection

Projection utama:

- `session_states`
- `session_participant_balances`
- `session_participant_inventory`
- `session_participant_gold_holdings`
- `session_participant_loans`
- `session_participant_insurances`
- `session_participant_collection_missions`
- `session_participant_financial_goals`
- `session_narrative_logs`
- `session_final_scores`
- `session_final_score_components`
- `metric_snapshots`

Gold, loan, dan insurance dipisah karena lifecycle, constraint, dan atributnya
berbeda. Desain ini menghindari polymorphic row dengan kolom nullable dan
discriminator yang sulit divalidasi.

## Quest dan Narrative

Quest keluar dari MVP. Capaian pemain direpresentasikan oleh collection mission
dan financial goal.

Narrative berdiri sendiri melalui `ruleset_narratives` dan
`ruleset_narrative_scenes`, bukan sebagai asset registry. Kondisi narrative
disimpan pada `ruleset_trigger_conditions` dengan owner `NARRATIVE` dan
dievaluasi dari event/action yang sudah terstruktur. Hasil trigger dicatat pada
`session_narrative_logs`.

Script engine dihapus karena:

- menduplikasi aturan yang sudah dapat dinyatakan sebagai kondisi event;
- memperbesar permukaan validasi dan risiko eksekusi;
- menyulitkan replay deterministik dan audit analitik;
- tidak diperlukan untuk platform analitik board game ini.

## Scoring

`session_final_scores` menyimpan hasil akhir per peserta.
`session_final_score_components` menyimpan komponen poin terstruktur dan
provenance event. Ranking donasi/pensiun dihitung dari event atau score
component, bukan disimpan sebagai tabel core atau snapshot JSON terpisah.

## Ringkasan Reduksi Tabel

Tabel yang dihapus:

- `session_action_logs`
- `session_participant_assets`
- `session_player_assets`
- `session_ruleset_activations`
- `ruleset_quests`
- `session_participant_quest_progress`
- master global `ingredients` dan `game_components`
- `interpreter_commands`
- `quest_scripts`
- `narrative_scripts`
- `narrative_assets`
- `session_donation_event_rankings`
- `session_pension_rankings`
- tabel effect per domain seperti `event_inventory_effects`,
  `event_need_effects`, `event_goal_effects`, `event_asset_effects`,
  `event_score_effects`, dan `event_turn_effects`

Konsep yang digabung atau diganti:

- identitas gameplay asset/card-like digabung ke `ruleset_game_assets`;
- katalog ruleset menjadi view `ruleset_catalog_items` atas registry dan tabel
  detail;
- inventory ingredient lama diganti projection asset generik
  `session_participant_inventory`;
- asset peserta polymorphic dipecah menjadi tabel gold, loan, dan insurance;
- action code pada event di-resolve ke `ruleset_action_id`.

Tabel baru utama:

- `ruleset_game_assets`
- `event_asset_references`
- `ruleset_trigger_conditions`
- `session_participant_inventory`
- `session_participant_gold_holdings`
- `session_participant_loans`
- `session_participant_insurances`
- `ruleset_narrative_scenes`
- `session_narrative_logs`

## Aturan Integritas

- Semua tabel `ruleset_*` yang bergantung versi membawa
  `ruleset_version_id`.
- Semua projection peserta membawa `session_id` dan
  `session_participant_id`.
- FK event menggunakan scope `(session_id, event_id)` karena `event_id` unik
  per sesi.
- Asset detail menggunakan FK komposit
  `(ruleset_version_id, ruleset_game_asset_id)` untuk mencegah referensi silang
  antarversi.
- Ranking dashboard adalah query/view turunan, bukan sumber data baru.
