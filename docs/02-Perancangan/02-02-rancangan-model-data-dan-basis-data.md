# Rancangan Model Data dan Basis Data (PostgreSQL)  
## Sistem Informasi Dasbor Analitika & Manajemen *Ruleset* Cashflowpoly

### Dokumen
- Nama dokumen: Rancangan Model Data dan Basis Data
- Versi: 1.0
- Tanggal: 28 Januari 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan Dokumen
Dokumen ini disusun untuk mendefinisikan model data dan rancangan skema PostgreSQL untuk:
1. menyimpan *ruleset* beserta versinya,
2. menyimpan akun pengguna aplikasi dan relasi user-player,
3. menyimpan sesi dan pemain,
4. menyimpan event terurut sebagai sumber histori,
5. menyajikan data proyeksi dan agregasi metrik untuk dasbor.

Dokumen ini menjadi acuan implementasi skema PostgreSQL berbasis skrip SQL (tanpa ORM) dan acuan pembuatan indeks untuk kinerja query analitika.

---

## 2. Prinsip Rancangan Data
### 2.1 Event sebagai sumber histori
Sistem menyimpan setiap aksi sebagai event pada tabel `events`. Sistem membangun histori dan metrik dari data event.

### 2.2 Konsistensi urutan
Sistem mengunci urutan event per sesi dengan `sequence_number`. Sistem menolak event yang melanggar aturan urutan pada layer aplikasi.

### 2.3 Idempotensi penyimpanan
Sistem menjaga idempotensi dengan *unique constraint* pada kombinasi `(session_id, event_id)`.

### 2.4 Keterlacakan versi ruleset
Sistem menempelkan `ruleset_version_id` pada `events`. Sistem mempertahankan hasil analisis walau instruktur membuat versi baru.

### 2.5 Query analitika cepat
Sistem melayani dasbor dengan tabel proyeksi dan agregasi (`event_cashflow_projections`, `metric_snapshots`) untuk menghindari *full scan* event pada setiap permintaan.

---

## 3. Konvensi Penamaan dan Tipe Data
### 3.1 Konvensi penamaan
- Nama tabel: `snake_case` dan jamak, contoh `ruleset_versions`.
- Nama kolom: `snake_case`.
- Primary key: `id` atau `<entity>_id` bertipe UUID.
- Timestamp: `timestamptz`.

### 3.2 Tipe data utama
- UUID: `uuid`
- Timestamp: `timestamptz`
- JSON: `jsonb`
- Uang/koin: `integer` (unit koin)

---

## 4. Diagram Entitas dan Relasi (deskripsi)
Skema memuat relasi inti berikut:
1. `app_users` menyimpan akun autentikasi (`INSTRUCTOR`, `PLAYER`).
2. `user_player_links` mengikat akun ke profil pemain secara 1:1.
3. `players`, `sessions`, dan `rulesets` dapat membawa `instructor_user_id` untuk scope data instruktur.
4. `rulesets` memiliki banyak `ruleset_versions`.
5. `sessions` mereferensikan satu `ruleset_version` aktif melalui `session_ruleset_activations`.
6. `sessions` memiliki banyak `session_players`.
7. `events` mereferensikan `sessions`, opsional mereferensikan `players`, dan wajib mereferensikan `ruleset_versions`.
8. `metric_snapshots` mereferensikan `sessions` dan opsional mereferensikan `players`.

---

## 5. Definisi Tabel Inti

## 5.0 Tabel autentikasi (`app_users`)
Sistem menyimpan akun login aplikasi pada tabel ini.

### `app_users`
Kolom utama:
- `user_id` UUID
- `username` unik
- `password_hash`
- `role` (`INSTRUCTOR`, `PLAYER`)
- `is_active`
- `created_at`

SQL:
```sql
create table if not exists app_users (
  user_id uuid primary key,
  username varchar(80) not null unique,
  password_hash text not null,
  role varchar(20) not null check (role in ('INSTRUCTOR','PLAYER')),
  is_active boolean not null default true,
  created_at timestamptz not null default now()
);

create index if not exists ix_app_users_role_active on app_users(role, is_active);
```

## 5.1 Tabel `players`
Sistem menyimpan data identitas pemain pada tabel ini.

Kolom utama:
- `player_id` UUID
- `display_name` nama tampil
- `instructor_user_id` owner instruktur (opsional, FK ke `app_users.user_id`)
- `created_at` waktu pencatatan

SQL:
```sql
create table if not exists players (
  player_id uuid primary key,
  display_name varchar(80) not null,
  instructor_user_id uuid null references app_users(user_id) on delete set null,
  created_at timestamptz not null default now()
);

create index if not exists ix_players_instructor_user on players(instructor_user_id, created_at desc);
```

### Relasi akun-pemain (`user_player_links`)
Sistem mengikat akun login ke profil pemain secara 1:1.

Kolom utama:
- `link_id` UUID
- `user_id` (unik, FK ke `app_users.user_id`)
- `player_id` (unik, FK ke `players.player_id`)
- `created_at`

SQL:
```sql
create table if not exists user_player_links (
  link_id uuid primary key,
  user_id uuid not null unique references app_users(user_id) on delete cascade,
  player_id uuid not null unique references players(player_id) on delete cascade,
  created_at timestamptz not null default now()
);

create index if not exists ix_user_player_links_user on user_player_links(user_id);
create index if not exists ix_user_player_links_player on user_player_links(player_id);
```

---

## 5.2 Tabel `sessions`
Sistem menyimpan metadata sesi permainan.

Kolom utama:
- `session_id` UUID
- `session_name` nama sesi
- `mode` mode permainan (`PEMULA`, `MAHIR`)
- `status` status sesi (`CREATED`, `STARTED`, `ENDED`)
- `started_at`, `ended_at`
- `instructor_user_id` owner instruktur (opsional, FK ke `app_users.user_id`)
- `created_at`

SQL:
```sql
create table if not exists sessions (
  session_id uuid primary key,
  session_name varchar(120) not null,
  mode varchar(10) not null check (mode in ('PEMULA','MAHIR')),
  status varchar(10) not null check (status in ('CREATED','STARTED','ENDED')),
  started_at timestamptz null,
  ended_at timestamptz null,
  instructor_user_id uuid null references app_users(user_id) on delete set null,
  created_at timestamptz not null default now()
);

create index if not exists ix_sessions_status on sessions(status);
create index if not exists ix_sessions_created_at on sessions(created_at desc);
create index if not exists ix_sessions_instructor_user on sessions(instructor_user_id, created_at desc);
```

---

## 5.3 Tabel `session_players`
Sistem mengaitkan pemain ke sesi, termasuk urutan pemain bila instruktur menetapkan urutan.

Kolom utama:
- `session_player_id` UUID
- `session_id`
- `player_id`
- `join_order` urutan masuk atau urutan permainan
- `role` peran dalam sesi (`PLAYER`, `INSTRUCTOR_VIEW` bila perlu)

SQL:
```sql
create table if not exists session_players (
  session_player_id uuid primary key,
  session_id uuid not null references sessions(session_id) on delete cascade,
  player_id uuid not null references players(player_id) on delete restrict,
  join_order int not null default 0,
  role varchar(20) not null default 'PLAYER',
  created_at timestamptz not null default now(),
  unique(session_id, player_id)
);

create index if not exists ix_session_players_session on session_players(session_id, join_order);
create index if not exists ix_session_players_player on session_players(player_id);
```

---

## 5.4 Tabel `rulesets`
Sistem menyimpan entitas *ruleset* sebagai “wadah” konfigurasi.

Kolom utama:
- `ruleset_id` UUID
- `name`
- `description`
- `instructor_user_id` owner instruktur (opsional, FK ke `app_users.user_id`)
- `is_archived`
- `created_at`
- `created_by`

SQL:
```sql
create table if not exists rulesets (
  ruleset_id uuid primary key,
  name varchar(120) not null,
  description text null,
  instructor_user_id uuid null references app_users(user_id) on delete set null,
  is_archived boolean not null default false,
  created_at timestamptz not null default now(),
  created_by varchar(80) null
);

create index if not exists ix_rulesets_archived on rulesets(is_archived);
create index if not exists ix_rulesets_created_at on rulesets(created_at desc);
create index if not exists ix_rulesets_instructor_user on rulesets(instructor_user_id, created_at desc);
```

---

## 5.5 Tabel `ruleset_versions`
Sistem menyimpan versi konfigurasi dalam JSONB agar instruktur dapat mengubah parameter tanpa mengubah kode.

Kolom utama:
- `ruleset_version_id` UUID
- `ruleset_id`
- `version` nomor versi mulai dari 1
- `status` (`DRAFT`, `ACTIVE`, `RETIRED`)
- `config_json` JSONB konfigurasi
- `config_hash` string untuk audit
- `created_at`, `created_by`

SQL:
```sql
create table if not exists ruleset_versions (
  ruleset_version_id uuid primary key,
  ruleset_id uuid not null references rulesets(ruleset_id) on delete cascade,
  version int not null check (version >= 1),
  status varchar(10) not null check (status in ('DRAFT','ACTIVE','RETIRED')),
  config_json jsonb not null,
  config_hash varchar(128) not null,
  created_at timestamptz not null default now(),
  created_by varchar(80) null,
  unique(ruleset_id, version),
  unique(ruleset_id, config_hash)
);

create index if not exists ix_ruleset_versions_ruleset on ruleset_versions(ruleset_id, version desc);
create index if not exists ix_ruleset_versions_status on ruleset_versions(status);
create index if not exists ix_ruleset_versions_config_gin on ruleset_versions using gin (config_json);
```

Catatan:
- Sistem memakai indeks GIN pada `config_json` untuk pencarian parameter saat debug dan audit.

---

## 5.6 Tabel `session_ruleset_activations`
Sistem mencatat aktivasi versi *ruleset* pada sesi.

Kolom utama:
- `activation_id` UUID
- `session_id`
- `ruleset_version_id`
- `activated_at`
- `activated_by`

SQL:
```sql
create table if not exists session_ruleset_activations (
  activation_id uuid primary key,
  session_id uuid not null references sessions(session_id) on delete cascade,
  ruleset_version_id uuid not null references ruleset_versions(ruleset_version_id) on delete restrict,
  activated_at timestamptz not null default now(),
  activated_by varchar(80) null
);

create index if not exists ix_sra_session on session_ruleset_activations(session_id, activated_at desc);
create index if not exists ix_sra_ruleset_version on session_ruleset_activations(ruleset_version_id);
```

Aturan:
- Sistem menganggap record aktivasi terakhir sebagai versi aktif untuk sesi.
- Sistem menolak aktivasi bila sesi berstatus `ENDED`.

---

## 5.7 Tabel `events`
Sistem menyimpan event terurut sebagai log sesi.

Kolom utama:
- `event_pk` UUID (PK internal)
- `event_id` UUID (ID event dari klien)
- `session_id`
- `player_id` (opsional; boleh null untuk event sistem)
- `actor_type` (`PLAYER`, `SYSTEM`)
- `timestamp` waktu event
- `day_index`, `weekday`, `turn_number`
- `sequence_number` urutan event per sesi
- `action_type` tipe event
- `ruleset_version_id`
- `payload` JSONB
- `received_at` waktu server menerima event
- `client_request_id` (opsional)

SQL:
```sql
create table if not exists events (
  event_pk uuid primary key,
  event_id uuid not null,
  session_id uuid not null references sessions(session_id) on delete cascade,
  player_id uuid null references players(player_id) on delete restrict,
  actor_type varchar(10) not null check (actor_type in ('PLAYER','SYSTEM')),
  timestamp timestamptz not null,
  day_index int not null check (day_index >= 0),
  weekday varchar(3) not null check (weekday in ('MON','TUE','WED','THU','FRI','SAT','SUN')),
  turn_number int not null check (turn_number >= 0),
  sequence_number bigint not null check (sequence_number >= 0),
  action_type varchar(64) not null,
  ruleset_version_id uuid not null references ruleset_versions(ruleset_version_id) on delete restrict,
  payload jsonb not null,
  received_at timestamptz not null default now(),
  client_request_id varchar(120) null,
  unique(session_id, event_id),
  unique(session_id, sequence_number)
);

create index if not exists ix_events_session_seq on events(session_id, sequence_number);
create index if not exists ix_events_session_time on events(session_id, timestamp);
create index if not exists ix_events_session_action on events(session_id, action_type);
create index if not exists ix_events_player_time on events(player_id, timestamp);
create index if not exists ix_events_payload_gin on events using gin (payload);
```

---

## 6. Tabel Proyeksi dan Agregasi (untuk Dasbor)

## 6.1 Tabel `event_cashflow_projections`
Sistem membuat proyeksi transaksi arus kas dari event yang memengaruhi saldo.

Tujuan:
- Sistem mempercepat query histori transaksi.

Kolom utama:
- `projection_id` UUID
- `session_id`
- `player_id`
- `event_pk` UUID (FK ke `events.event_pk`)
- `event_id` UUID (atribut audit/idempotensi dari klien)
- `timestamp`
- `direction` (`IN`, `OUT`)
- `amount` int
- `category` string
- `counterparty` string
- `reference` string
- `note` string

SQL:
```sql
create table if not exists event_cashflow_projections (
  projection_id uuid primary key,
  session_id uuid not null references sessions(session_id) on delete cascade,
  player_id uuid not null references players(player_id) on delete restrict,
  event_pk uuid not null references events(event_pk) on delete restrict,
  event_id uuid not null,
  timestamp timestamptz not null,
  direction varchar(3) not null check (direction in ('IN','OUT')),
  amount int not null check (amount > 0),
  category varchar(40) not null,
  counterparty varchar(20) null,
  reference varchar(80) null,
  note varchar(160) null,
  unique(session_id, event_id)
);

create index if not exists ix_ecp_session_time on event_cashflow_projections(session_id, timestamp desc);
create index if not exists ix_ecp_session_player_time on event_cashflow_projections(session_id, player_id, timestamp desc);
create index if not exists ix_ecp_category on event_cashflow_projections(category);
```

Aturan proyeksi:
- Sistem menulis baris proyeksi saat sistem menerima event valid.
- Sistem menjaga keterlacakan proyeksi ke log event melalui FK `event_pk -> events(event_pk)`.
- Sistem menjaga idempotensi proyeksi berbasis `event_id` klien melalui `unique(session_id, event_id)`.

---

## 6.2 Tabel `metric_snapshots`
Sistem menyimpan hasil agregasi metrik agar dasbor menampilkan data cepat dan konsisten.

Kolom utama:
- `metric_snapshot_id` UUID
- `session_id`
- `player_id` (opsional: null untuk snapshot/agregat level sesi)
- `computed_at` waktu hitung
- `metric_name` nama metrik
- `metric_value_numeric` nilai numerik
- `metric_value_json` nilai kompleks (opsional)
- `ruleset_version_id` versi ruleset konteks hitung

SQL:
```sql
create table if not exists metric_snapshots (
  metric_snapshot_id uuid primary key,
  session_id uuid not null references sessions(session_id) on delete cascade,
  player_id uuid null references players(player_id) on delete restrict,
  computed_at timestamptz not null default now(),
  metric_name varchar(80) not null,
  metric_value_numeric double precision null,
  metric_value_json jsonb null,
  ruleset_version_id uuid not null references ruleset_versions(ruleset_version_id) on delete restrict
);

create index if not exists ix_metrics_session_name_time on metric_snapshots(session_id, metric_name, computed_at desc);
create index if not exists ix_metrics_session_player_name_time on metric_snapshots(session_id, player_id, metric_name, computed_at desc);
create index if not exists ix_metrics_session_player_time on metric_snapshots(session_id, player_id, computed_at desc);
create index if not exists ix_metrics_ruleset_version on metric_snapshots(ruleset_version_id);
```

Kebijakan:
- Sistem menyimpan beberapa snapshot bila instruktur meminta pembandingan waktu.
- Sistem dapat menghapus snapshot lama bila kebutuhan ruang meningkat.

---

## 6.3 Tabel `validation_logs` (opsional, berguna saat pengujian)
Sistem menyimpan hasil validasi event agar instruktur dapat melacak kegagalan validasi.

Kolom utama:
- `validation_log_id` UUID
- `session_id`
- `event_pk` (FK ke `events.event_pk`, opsional sesuai kebutuhan log)
- `event_id` (atribut audit/idempotensi dari klien)
- `is_valid`
- `error_code`
- `error_message`
- `details_json`
- `created_at`

SQL:
```sql
create table if not exists validation_logs (
  validation_log_id uuid primary key,
  session_id uuid not null references sessions(session_id) on delete cascade,
  event_pk uuid null references events(event_pk) on delete restrict,
  event_id uuid not null,
  is_valid boolean not null,
  error_code varchar(40) null,
  error_message varchar(200) null,
  details_json jsonb null,
  created_at timestamptz not null default now(),
  unique(session_id, event_id)
);

create index if not exists ix_validation_session_time on validation_logs(session_id, created_at desc);
create index if not exists ix_validation_valid on validation_logs(is_valid);
```

---

## 6.4 Tabel `security_audit_logs`
Sistem menyimpan jejak audit keamanan aplikasi (auth challenge/forbidden/login/register/rate-limit).

Kolom utama:
- `security_audit_log_id` UUID
- `occurred_at`
- `trace_id`
- `event_type`
- `outcome` (`SUCCESS`/`FAILURE`/`DENIED`)
- `user_id` (opsional)
- `username` (opsional)
- `role` (opsional)
- `ip_address`
- `user_agent`
- `method`, `path`, `status_code`
- `detail_json`

SQL:
```sql
create table if not exists security_audit_logs (
  security_audit_log_id uuid primary key,
  occurred_at timestamptz not null default now(),
  trace_id varchar(64) not null,
  event_type varchar(80) not null,
  outcome varchar(20) not null check (outcome in ('SUCCESS','FAILURE','DENIED')),
  user_id uuid null references app_users(user_id) on delete set null,
  username varchar(80) null,
  role varchar(20) null,
  ip_address varchar(64) null,
  user_agent varchar(300) null,
  method varchar(16) not null,
  path varchar(240) not null,
  status_code int not null check (status_code >= 100 and status_code <= 599),
  detail_json jsonb null
);

create index if not exists ix_security_audit_logs_occurred on security_audit_logs(occurred_at desc);
create index if not exists ix_security_audit_logs_event on security_audit_logs(event_type, occurred_at desc);
create index if not exists ix_security_audit_logs_user on security_audit_logs(user_id, occurred_at desc);
```

---

## 7. Aturan Integritas Data
Sistem menerapkan aturan integritas berikut:
1. Sistem menjaga urutan event dalam sesi melalui `unique(session_id, sequence_number)` pada `events`.
2. Sistem menjaga idempotensi ingest event melalui `unique(session_id, event_id)` pada `events`.
3. Sistem menjaga keunikan pemain dalam sesi melalui `unique(session_id, player_id)` pada `session_players`.
4. Sistem menjaga keunikan versi *ruleset* melalui `unique(ruleset_id, version)` pada `ruleset_versions`.
5. Sistem melarang *orphan record* dengan foreign key pada sesi, pemain, versi *ruleset*, dan referensi event (`event_pk`).
6. Sistem melarang penghapusan `ruleset_versions` yang sudah dipakai event melalui `on delete restrict`.
7. Sistem menghapus data turunan sesi saat sistem menghapus sesi melalui `on delete cascade` pada tabel proyeksi dan metrik.

### 7.1 Aturan nullable yang disengaja
- `events.player_id` boleh `null` untuk event sistem (`actor_type='SYSTEM'`).
- `metric_snapshots.player_id` boleh `null` untuk snapshot/agregat level sesi.

---

## 8. Strategi Query untuk Dasbor
Sistem melayani kebutuhan dasbor lewat query berikut:
1. Ringkasan sesi: sistem ambil metrik terakhir pada `metric_snapshots` untuk `player_id null`.
2. Ringkasan per pemain: sistem ambil metrik terakhir per `player_id`.
3. Histori transaksi: sistem query `event_cashflow_projections` per sesi dan pemain.
4. Audit event: sistem query `events` per sesi dan `sequence_number`.

Sistem dapat menambah *materialized view* bila query agregat tumbuh kompleks.

---

## 9. Strategi Perubahan Skema (Tanpa ORM)
### 9.1 Konvensi perubahan skema
Sistem mengelola perubahan skema dengan skrip SQL terpisah:
1. Sistem tambah tabel inti terlebih dahulu.
2. Sistem tambah indeks setelah tabel ada.
3. Sistem tambah constraint setelah data aman.

### 9.2 Seed data minimum
Sistem dapat menanam data awal berikut pada lingkungan pengembangan:
- satu ruleset default,
- satu ruleset_version default,
- satu sesi contoh untuk uji endpoint.

---

## 10. Checklist Kesiapan Implementasi
Sistem siap masuk implementasi basis data jika:
1. skema tabel inti berjalan pada PostgreSQL tanpa error,
2. indeks utama terbentuk (`events`, `metric_snapshots`, `event_cashflow_projections`),
3. constraint idempotensi dan urutan event aktif,
4. skrip SQL menghasilkan struktur identik dengan dokumen ini.



