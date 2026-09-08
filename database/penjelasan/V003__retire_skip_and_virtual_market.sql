-- Fungsi file: Menonaktifkan aksi lewati pesanan dan pengelolaan pasar virtual tanpa menghapus histori event lama.
-- Salinan pembelajaran berkomentar dari database/migrations/V003__retire_skip_and_virtual_market.sql; tidak dibaca oleh initializer aplikasi.
-- Berkas sumber migrasi dipertahankan persis karena schema_history memverifikasi checksum seluruh isi file.
-- Penjelasan: Memperbarui tabel ruleset_actions; SET mengatur nilai baru dan WHERE membatasi rekaman sasaran.
update ruleset_actions
-- Penjelasan: Membuka penetapan nilai is_active = false dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
set is_active = false
-- Penjelasan: Membatasi baris skrip basis data dengan syarat action_id in ('LewatiOrder', 'AmbilKartuDariDeck', 'KartuMasukDiscard', 'IsiUlangPasar');; hanya baris dengan predikat TRUE masuk hasil atau terkena perubahan data.
where action_id in ('LewatiOrder', 'AmbilKartuDariDeck', 'KartuMasukDiscard', 'IsiUlangPasar');

-- Penjelasan: Memperbarui tabel actions; SET mengatur nilai baru dan WHERE membatasi rekaman sasaran.
update actions
-- Penjelasan: Membuka penetapan nilai is_active = false dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
set is_active = false
-- Penjelasan: Membatasi baris skrip basis data dengan syarat action_id in ('LewatiOrder', 'AmbilKartuDariDeck', 'KartuMasukDiscard', 'IsiUlangPasar');; hanya baris dengan predikat TRUE masuk hasil atau terkena perubahan data.
where action_id in ('LewatiOrder', 'AmbilKartuDariDeck', 'KartuMasukDiscard', 'IsiUlangPasar');

-- Penjelasan: Mengevaluasi ekspresi create or replace function enforce_event_session_scope_v2() pada fungsi enforce_event_session_scope_v2; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
create or replace function enforce_event_session_scope_v2()
-- Penjelasan: Menetapkan tipe hasil trigger pada fungsi enforce_event_session_scope_v2; RETURNS TRIGGER berarti fungsi mengembalikan baris atau NULL sesuai kontrak pemanggilan trigger.
returns trigger
-- Penjelasan: Memilih bahasa badan fungsi enforce_event_session_scope_v2: plpgsql; PL/pgSQL mendukung SQL bersama variabel, cabang, loop, dan penanganan pengecualian.
language plpgsql
-- Penjelasan: Membuka badan fungsi enforce_event_session_scope_v2 dengan delimiter $$ agar kode prosedural dapat memuat tanda petik SQL tanpa mengakhiri deklarasi fungsi.
as $$
-- Penjelasan: Membuka daftar variabel lokal fungsi enforce_event_session_scope_v2; setiap variabel memiliki tipe PostgreSQL dan dapat diberi nilai awal sebelum blok BEGIN dijalankan.
declare
  -- Penjelasan: Mendeklarasikan v_session_status untuk variabel lokal nilai session status di dalam fungsi enforce_event_session_scope_v2, menggunakan tipe varchar. Tanpa initializer, variabel lokal PL/pgSQL mulai bernilai NULL.
  v_session_status varchar(20);
  -- Penjelasan: Mendeklarasikan v_active_ruleset_version_id untuk variabel lokal identitas relasi active ruleset version di dalam fungsi enforce_event_session_scope_v2, menggunakan tipe uuid. Tanpa initializer, variabel lokal PL/pgSQL mulai bernilai NULL.
  v_active_ruleset_version_id uuid;
  -- Penjelasan: Mendeklarasikan v_participant_user_id untuk variabel lokal identitas relasi participant user di dalam fungsi enforce_event_session_scope_v2, menggunakan tipe uuid. Tanpa initializer, variabel lokal PL/pgSQL mulai bernilai NULL.
  v_participant_user_id uuid;
  -- Penjelasan: Mendeklarasikan v_player_order_no untuk variabel lokal nomor urutan pemain dalam sesi di dalam fungsi enforce_event_session_scope_v2, menggunakan tipe integer. Tanpa initializer, variabel lokal PL/pgSQL mulai bernilai NULL.
  v_player_order_no integer;
-- Penjelasan: Memulai bagian eksekusi fungsi enforce_event_session_scope_v2; pernyataan setelahnya memakai variabel dan konteks transaksi yang tersedia.
begin
  -- Penjelasan: Mengevaluasi syarat new.action_type in ('LewatiOrder', 'AmbilKartuDariDeck', 'KartuMasukDiscard', 'IsiUlangPasar') then pada fungsi enforce_event_session_scope_v2; bagian THEN dijalankan hanya saat syarat TRUE, sehingga perubahan data atau penolakan event mengikuti keadaan yang sedang diuji. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
  if new.action_type in ('LewatiOrder', 'AmbilKartuDariDeck', 'KartuMasukDiscard', 'IsiUlangPasar') then
    -- Penjelasan: Menolak operasi dengan RAISE EXCEPTION ketika cabang validasi ini tercapai. Pesan raise exception 'Action % is retired and cannot be ingested', new.action_type using errcode = '23514'; menerangkan penyebabnya; placeholder % diisi argumen berikut dan ERRCODE, bila ada, memberi kode kegagalan PostgreSQL. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
    raise exception 'Action % is retired and cannot be ingested', new.action_type using errcode = '23514';
  -- Penjelasan: Menutup cabang IF pada fungsi enforce_event_session_scope_v2; eksekusi melanjutkan pernyataan setelah pemeriksaan tersebut bila tidak dihentikan oleh RETURN atau RAISE EXCEPTION.
  end if;

  -- Penjelasan: Memulai pemilihan hasil pada fungsi enforce_event_session_scope_v2; status, ruleset_version_id menjadi ekspresi hasil yang dievaluasi untuk setiap baris atau kelompok.
  select status, ruleset_version_id
  -- Penjelasan: Mengevaluasi ekspresi into v_session_status, v_active_ruleset_version_id pada fungsi enforce_event_session_scope_v2; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. menampung hasil pada v_session_status (variabel lokal nilai session status).
  into v_session_status, v_active_ruleset_version_id
  -- Penjelasan: Menetapkan sumber baris sessions yang dibaca dalam fungsi enforce_event_session_scope_v2; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
  from sessions
  -- Penjelasan: Membatasi baris fungsi enforce_event_session_scope_v2 dengan syarat session_id = new.session_id;; hanya baris dengan predikat TRUE masuk hasil atau terkena perubahan data. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
  where session_id = new.session_id;

  -- Penjelasan: Mengevaluasi syarat v_active_ruleset_version_id is null then pada fungsi enforce_event_session_scope_v2; bagian THEN dijalankan hanya saat syarat TRUE, sehingga perubahan data atau penolakan event mengikuti keadaan yang sedang diuji.
  if v_active_ruleset_version_id is null then
    -- Penjelasan: Menolak operasi dengan RAISE EXCEPTION ketika cabang validasi ini tercapai. Pesan raise exception 'Session % does not exist', new.session_id using errcode = '23503'; menerangkan penyebabnya; placeholder % diisi argumen berikut dan ERRCODE, bila ada, memberi kode kegagalan PostgreSQL. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
    raise exception 'Session % does not exist', new.session_id using errcode = '23503';
  -- Penjelasan: Menutup cabang IF pada fungsi enforce_event_session_scope_v2; eksekusi melanjutkan pernyataan setelah pemeriksaan tersebut bila tidak dihentikan oleh RETURN atau RAISE EXCEPTION.
  end if;

  -- Penjelasan: Mengevaluasi syarat new.ruleset_version_id is distinct from v_active_ruleset_version_id then pada fungsi enforce_event_session_scope_v2; bagian THEN dijalankan hanya saat syarat TRUE, sehingga perubahan data atau penolakan event mengikuti keadaan yang sedang diuji. IS DISTINCT FROM membandingkan nilai secara aman ketika salah satu atau keduanya NULL; NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
  if new.ruleset_version_id is distinct from v_active_ruleset_version_id then
    -- Penjelasan: Menolak operasi dengan RAISE EXCEPTION ketika cabang validasi ini tercapai. Pesan raise exception 'Event ruleset version does not match session ruleset version' using errcode = '23514'; menerangkan penyebabnya; placeholder % diisi argumen berikut dan ERRCODE, bila ada, memberi kode kegagalan PostgreSQL.
    raise exception 'Event ruleset version does not match session ruleset version' using errcode = '23514';
  -- Penjelasan: Menutup cabang IF pada fungsi enforce_event_session_scope_v2; eksekusi melanjutkan pernyataan setelah pemeriksaan tersebut bila tidak dihentikan oleh RETURN atau RAISE EXCEPTION.
  end if;

  -- Penjelasan: Mengevaluasi syarat not exists ( pada fungsi enforce_event_session_scope_v2; bagian THEN dijalankan hanya saat syarat TRUE, sehingga perubahan data atau penolakan event mengikuti keadaan yang sedang diuji. EXISTS cukup memeriksa keberadaan satu baris dan tidak memerlukan seluruh hasil subkueri.
  if not exists (
    -- Penjelasan: Memulai pemilihan hasil pada fungsi enforce_event_session_scope_v2; 1 menjadi ekspresi hasil yang dievaluasi untuk setiap baris atau kelompok.
    select 1
    -- Penjelasan: Menetapkan sumber baris ruleset_actions ra yang dibaca dalam fungsi enforce_event_session_scope_v2; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
    from ruleset_actions ra
    -- Penjelasan: Menggabungkan sumber join actions a on a.action_id = ra.action_id and a.is_active dalam fungsi enforce_event_session_scope_v2. JOIN memasangkan baris yang memenuhi syarat ON sehingga identitas aturan, sesi, atau aset tetap selaras.
    join actions a on a.action_id = ra.action_id and a.is_active
    -- Penjelasan: Membatasi baris fungsi enforce_event_session_scope_v2 dengan syarat ra.ruleset_version_id = new.ruleset_version_id; hanya baris dengan predikat TRUE masuk hasil atau terkena perubahan data. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
    where ra.ruleset_version_id = new.ruleset_version_id
      -- Penjelasan: Menambahkan syarat wajib ra.ruleset_action_id = new.ruleset_action_id pada fungsi enforce_event_session_scope_v2; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
      and ra.ruleset_action_id = new.ruleset_action_id
      -- Penjelasan: Menambahkan syarat wajib ra.action_id = new.action_type pada fungsi enforce_event_session_scope_v2; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
      and ra.action_id = new.action_type
      -- Penjelasan: Menambahkan syarat wajib ra.is_active pada fungsi enforce_event_session_scope_v2; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos.
      and ra.is_active
  -- Penjelasan: Mengevaluasi ekspresi ) then pada fungsi enforce_event_session_scope_v2; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
  ) then
    -- Penjelasan: Menolak operasi dengan RAISE EXCEPTION ketika cabang validasi ini tercapai. Pesan raise exception 'Action % is not active in the session ruleset', new.action_type using errcode = '23514'; menerangkan penyebabnya; placeholder % diisi argumen berikut dan ERRCODE, bila ada, memberi kode kegagalan PostgreSQL. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
    raise exception 'Action % is not active in the session ruleset', new.action_type using errcode = '23514';
  -- Penjelasan: Menutup cabang IF pada fungsi enforce_event_session_scope_v2; eksekusi melanjutkan pernyataan setelah pemeriksaan tersebut bila tidak dihentikan oleh RETURN atau RAISE EXCEPTION.
  end if;

  -- Penjelasan: Mengevaluasi syarat new.actor_type = 'PLAYER' then pada fungsi enforce_event_session_scope_v2; bagian THEN dijalankan hanya saat syarat TRUE, sehingga perubahan data atau penolakan event mengikuti keadaan yang sedang diuji. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
  if new.actor_type = 'PLAYER' then
    -- Penjelasan: Mengevaluasi syarat v_session_status = 'CREATED' then pada fungsi enforce_event_session_scope_v2; bagian THEN dijalankan hanya saat syarat TRUE, sehingga perubahan data atau penolakan event mengikuti keadaan yang sedang diuji.
    if v_session_status = 'CREATED' then
      -- Penjelasan: Menolak operasi dengan RAISE EXCEPTION ketika cabang validasi ini tercapai. Pesan raise exception 'PLAYER events require a session that has started' using errcode = '23514'; menerangkan penyebabnya; placeholder % diisi argumen berikut dan ERRCODE, bila ada, memberi kode kegagalan PostgreSQL.
      raise exception 'PLAYER events require a session that has started' using errcode = '23514';
    -- Penjelasan: Menutup cabang IF pada fungsi enforce_event_session_scope_v2; eksekusi melanjutkan pernyataan setelah pemeriksaan tersebut bila tidak dihentikan oleh RETURN atau RAISE EXCEPTION.
    end if;

    -- Penjelasan: Mengevaluasi syarat new.session_player_id is null or new.user_id is null then pada fungsi enforce_event_session_scope_v2; bagian THEN dijalankan hanya saat syarat TRUE, sehingga perubahan data atau penolakan event mengikuti keadaan yang sedang diuji. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
    if new.session_player_id is null or new.user_id is null then
      -- Penjelasan: Menolak operasi dengan RAISE EXCEPTION ketika cabang validasi ini tercapai. Pesan raise exception 'PLAYER event requires session_player_id and user_id' using errcode = '23514'; menerangkan penyebabnya; placeholder % diisi argumen berikut dan ERRCODE, bila ada, memberi kode kegagalan PostgreSQL.
      raise exception 'PLAYER event requires session_player_id and user_id' using errcode = '23514';
    -- Penjelasan: Menutup cabang IF pada fungsi enforce_event_session_scope_v2; eksekusi melanjutkan pernyataan setelah pemeriksaan tersebut bila tidak dihentikan oleh RETURN atau RAISE EXCEPTION.
    end if;

    -- Penjelasan: Memulai pemilihan hasil pada fungsi enforce_event_session_scope_v2; user_id, player_order_no menjadi ekspresi hasil yang dievaluasi untuk setiap baris atau kelompok.
    select user_id, player_order_no
    -- Penjelasan: Mengevaluasi ekspresi into v_participant_user_id, v_player_order_no pada fungsi enforce_event_session_scope_v2; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. menampung hasil pada v_participant_user_id (variabel lokal identitas relasi participant user).
    into v_participant_user_id, v_player_order_no
    -- Penjelasan: Menetapkan sumber baris session_participants yang dibaca dalam fungsi enforce_event_session_scope_v2; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
    from session_participants
    -- Penjelasan: Membatasi baris fungsi enforce_event_session_scope_v2 dengan syarat session_id = new.session_id; hanya baris dengan predikat TRUE masuk hasil atau terkena perubahan data. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
    where session_id = new.session_id
      -- Penjelasan: Menambahkan syarat wajib session_participant_id = new.session_player_id; pada fungsi enforce_event_session_scope_v2; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
      and session_participant_id = new.session_player_id;

    -- Penjelasan: Mengevaluasi syarat v_participant_user_id is null or new.user_id is distinct from v_participant_user_id then pada fungsi enforce_event_session_scope_v2; bagian THEN dijalankan hanya saat syarat TRUE, sehingga perubahan data atau penolakan event mengikuti keadaan yang sedang diuji. IS DISTINCT FROM membandingkan nilai secara aman ketika salah satu atau keduanya NULL; NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
    if v_participant_user_id is null or new.user_id is distinct from v_participant_user_id then
      -- Penjelasan: Menolak operasi dengan RAISE EXCEPTION ketika cabang validasi ini tercapai. Pesan raise exception 'Event player does not match the session participant' using errcode = '23514'; menerangkan penyebabnya; placeholder % diisi argumen berikut dan ERRCODE, bila ada, memberi kode kegagalan PostgreSQL.
      raise exception 'Event player does not match the session participant' using errcode = '23514';
    -- Penjelasan: Menutup cabang IF pada fungsi enforce_event_session_scope_v2; eksekusi melanjutkan pernyataan setelah pemeriksaan tersebut bila tidak dihentikan oleh RETURN atau RAISE EXCEPTION.
    end if;

    -- Penjelasan: Mengevaluasi syarat new.turn_number <> v_player_order_no then pada fungsi enforce_event_session_scope_v2; bagian THEN dijalankan hanya saat syarat TRUE, sehingga perubahan data atau penolakan event mengikuti keadaan yang sedang diuji. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
    if new.turn_number <> v_player_order_no then
      -- Penjelasan: Menolak operasi dengan RAISE EXCEPTION ketika cabang validasi ini tercapai. Pesan raise exception 'PLAYER event turn_number must match player_order_no' using errcode = '23514'; menerangkan penyebabnya; placeholder % diisi argumen berikut dan ERRCODE, bila ada, memberi kode kegagalan PostgreSQL.
      raise exception 'PLAYER event turn_number must match player_order_no' using errcode = '23514';
    -- Penjelasan: Menutup cabang IF pada fungsi enforce_event_session_scope_v2; eksekusi melanjutkan pernyataan setelah pemeriksaan tersebut bila tidak dihentikan oleh RETURN atau RAISE EXCEPTION.
    end if;
  -- Penjelasan: Memeriksa alternatif kondisi new.actor_type = 'SYSTEM' then hanya jika cabang IF sebelumnya tidak terpenuhi pada fungsi enforce_event_session_scope_v2. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
  elsif new.actor_type = 'SYSTEM' then
    -- Penjelasan: Mengevaluasi syarat new.action_slot <> 0 then pada fungsi enforce_event_session_scope_v2; bagian THEN dijalankan hanya saat syarat TRUE, sehingga perubahan data atau penolakan event mengikuti keadaan yang sedang diuji. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
    if new.action_slot <> 0 then
      -- Penjelasan: Menolak operasi dengan RAISE EXCEPTION ketika cabang validasi ini tercapai. Pesan raise exception 'SYSTEM event cannot consume an action slot' using errcode = '23514'; menerangkan penyebabnya; placeholder % diisi argumen berikut dan ERRCODE, bila ada, memberi kode kegagalan PostgreSQL.
      raise exception 'SYSTEM event cannot consume an action slot' using errcode = '23514';
    -- Penjelasan: Menutup cabang IF pada fungsi enforce_event_session_scope_v2; eksekusi melanjutkan pernyataan setelah pemeriksaan tersebut bila tidak dihentikan oleh RETURN atau RAISE EXCEPTION.
    end if;

    -- Penjelasan: Mengevaluasi syarat new.session_player_id is not null then pada fungsi enforce_event_session_scope_v2; bagian THEN dijalankan hanya saat syarat TRUE, sehingga perubahan data atau penolakan event mengikuti keadaan yang sedang diuji. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
    if new.session_player_id is not null then
      -- Penjelasan: Memulai pemilihan hasil pada fungsi enforce_event_session_scope_v2; user_id menjadi ekspresi hasil yang dievaluasi untuk setiap baris atau kelompok.
      select user_id
      -- Penjelasan: Mengevaluasi ekspresi into v_participant_user_id pada fungsi enforce_event_session_scope_v2; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. menampung hasil pada v_participant_user_id (variabel lokal identitas relasi participant user).
      into v_participant_user_id
      -- Penjelasan: Menetapkan sumber baris session_participants yang dibaca dalam fungsi enforce_event_session_scope_v2; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
      from session_participants
      -- Penjelasan: Membatasi baris fungsi enforce_event_session_scope_v2 dengan syarat session_id = new.session_id; hanya baris dengan predikat TRUE masuk hasil atau terkena perubahan data. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
      where session_id = new.session_id
        -- Penjelasan: Menambahkan syarat wajib session_participant_id = new.session_player_id; pada fungsi enforce_event_session_scope_v2; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
        and session_participant_id = new.session_player_id;

      -- Penjelasan: Mengevaluasi syarat v_participant_user_id is null or new.user_id is distinct from v_participant_user_id then pada fungsi enforce_event_session_scope_v2; bagian THEN dijalankan hanya saat syarat TRUE, sehingga perubahan data atau penolakan event mengikuti keadaan yang sedang diuji. IS DISTINCT FROM membandingkan nilai secara aman ketika salah satu atau keduanya NULL; NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
      if v_participant_user_id is null or new.user_id is distinct from v_participant_user_id then
        -- Penjelasan: Menolak operasi dengan RAISE EXCEPTION ketika cabang validasi ini tercapai. Pesan raise exception 'SYSTEM event player does not match the session participant' using errcode = '23514'; menerangkan penyebabnya; placeholder % diisi argumen berikut dan ERRCODE, bila ada, memberi kode kegagalan PostgreSQL.
        raise exception 'SYSTEM event player does not match the session participant' using errcode = '23514';
      -- Penjelasan: Menutup cabang IF pada fungsi enforce_event_session_scope_v2; eksekusi melanjutkan pernyataan setelah pemeriksaan tersebut bila tidak dihentikan oleh RETURN atau RAISE EXCEPTION.
      end if;
    -- Penjelasan: Memeriksa alternatif kondisi new.user_id is not null then hanya jika cabang IF sebelumnya tidak terpenuhi pada fungsi enforce_event_session_scope_v2. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
    elsif new.user_id is not null then
      -- Penjelasan: Menolak operasi dengan RAISE EXCEPTION ketika cabang validasi ini tercapai. Pesan raise exception 'SYSTEM event user_id requires session_player_id' using errcode = '23514'; menerangkan penyebabnya; placeholder % diisi argumen berikut dan ERRCODE, bila ada, memberi kode kegagalan PostgreSQL.
      raise exception 'SYSTEM event user_id requires session_player_id' using errcode = '23514';
    -- Penjelasan: Menutup cabang IF pada fungsi enforce_event_session_scope_v2; eksekusi melanjutkan pernyataan setelah pemeriksaan tersebut bila tidak dihentikan oleh RETURN atau RAISE EXCEPTION.
    end if;
  -- Penjelasan: Menggunakan jalur atau nilai cadangan pada blok berikut ketika cabang sebelumnya tidak cocok dalam fungsi enforce_event_session_scope_v2.
  else
    -- Penjelasan: Menolak operasi dengan RAISE EXCEPTION ketika cabang validasi ini tercapai. Pesan raise exception 'Unsupported actor type %', new.actor_type using errcode = '23514'; menerangkan penyebabnya; placeholder % diisi argumen berikut dan ERRCODE, bila ada, memberi kode kegagalan PostgreSQL. NEW merujuk keadaan calon baris yang sedang divalidasi oleh trigger.
    raise exception 'Unsupported actor type %', new.actor_type using errcode = '23514';
  -- Penjelasan: Menutup cabang IF pada fungsi enforce_event_session_scope_v2; eksekusi melanjutkan pernyataan setelah pemeriksaan tersebut bila tidak dihentikan oleh RETURN atau RAISE EXCEPTION.
  end if;

  -- Penjelasan: Mengakhiri pemanggilan fungsi enforce_event_session_scope_v2 dengan new. RETURN NEW mengizinkan calon baris yang telah diperiksa diteruskan oleh trigger.
  return new;
-- Penjelasan: Menutup ekspresi CASE atau blok prosedural pada fungsi enforce_event_session_scope_v2; akhiran AS, koma, atau titik koma menentukan apakah hasil diberi nama, dipakai dalam daftar, atau pernyataan diakhiri.
end
-- Penjelasan: Menutup badan prosedural fungsi enforce_event_session_scope_v2 pada delimiter $$; titik koma menyelesaikan deklarasi atau blok anonim yang dikirim ke PostgreSQL.
$$;

-- Penjelasan: Menghapus definisi trigger yang disebut (drop trigger if exists trg_events_session_scope on events;). IF EXISTS membuat langkah tetap berhasil bila objek sudah tidak ada; penghapusan trigger atau indeks diikuti pemasangan definisi kanonik bila dinyatakan sesudahnya.
drop trigger if exists trg_events_session_scope on events;

-- Penjelasan: Memasang trigger trg_events_session_scope; deklarasi waktu BEFORE/AFTER, peristiwa INSERT/UPDATE/DELETE, dan tabel sasaran menentukan kapan fungsi validasi atau proyeksi dipanggil otomatis.
create trigger trg_events_session_scope
-- Penjelasan: Melengkapi kontrak trigger melalui before insert or update of; waktu eksekusi, jenis perubahan, cakupan setiap baris/pernyataan, dan penundaan validasi menentukan kapan fungsi penjaga integritas berjalan.
before insert or update of
  -- Penjelasan: Memakai session_id (UUID sesi permainan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
  session_id,
  -- Penjelasan: Memakai session_player_id (UUID peserta yang melakukan atau menerima efek event) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
  session_player_id,
  -- Penjelasan: Memakai user_id (UUID akun pengguna) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
  user_id,
  -- Penjelasan: Memakai ruleset_action_id (identitas relasi ruleset action) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
  ruleset_action_id,
  -- Penjelasan: Memakai action_type (jenis kejadian atau aksi yang direkam) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
  action_type,
  -- Penjelasan: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
  ruleset_version_id,
  -- Penjelasan: Memakai actor_type (pelaku event PLAYER atau SYSTEM) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
  actor_type,
  -- Penjelasan: Memakai turn_number (nomor urutan pemain yang terkait giliran) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
  turn_number,
  -- Penjelasan: Memakai action_slot (slot aksi dalam giliran; slot nol dipakai event tanpa konsumsi slot) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
  action_slot
-- Penjelasan: Menetapkan kondisi pemasangan atau sasaran trigger events pada skrip basis data; relasi kolom menjaga pasangan baris sesuai lingkup yang dinyatakan.
on events
-- Penjelasan: Memulai perulangan for each row execute function enforce_event_session_scope_v2(); pada skrip basis data; hasil kueri menjadi sumber rekaman dan badan LOOP menerapkan langkah yang sama secara berurutan pada tiap rekaman.
for each row execute function enforce_event_session_scope_v2();
