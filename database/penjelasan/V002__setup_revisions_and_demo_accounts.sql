-- Fungsi file: Menambahkan riwayat revisi setup, penanda akun demo, dan indeks cursor.
-- Salinan pembelajaran berkomentar dari database/migrations/V002__setup_revisions_and_demo_accounts.sql; tidak dibaca oleh initializer aplikasi.
-- Berkas sumber migrasi dipertahankan persis karena schema_history memverifikasi checksum seluruh isi file.
-- Penjelasan: Membentuk tabel session_setup_revisions untuk menyimpan riwayat revisi setup sesi serta kunci idempotensi permintaannya. IF NOT EXISTS mempertahankan tabel bila sudah ada. Constraint di dalam definisi berikut menjaga integritas data langsung di PostgreSQL.
create table if not exists session_setup_revisions (
  -- Penjelasan: Mendefinisikan session_setup_revisions.session_id sebagai uuid untuk UUID sesi permainan. NOT NULL mewajibkan nilai tersedia pada setiap rekaman.
  session_id uuid not null,
  -- Penjelasan: Mendefinisikan session_setup_revisions.revision sebagai integer untuk nomor revisi setup. NOT NULL mewajibkan nilai tersedia pada setiap rekaman.
  revision integer not null,
  -- Penjelasan: Mendefinisikan session_setup_revisions.ruleset_version_id sebagai uuid untuk UUID revisi aturan yang menjadi lingkup data. NOT NULL mewajibkan nilai tersedia pada setiap rekaman.
  ruleset_version_id uuid not null,
  -- Penjelasan: Mendefinisikan session_setup_revisions.client_request_id sebagai varchar(120) untuk kunci permintaan klien untuk mencegah pencatatan ganda. NOT NULL mewajibkan nilai tersedia pada setiap rekaman.
  client_request_id varchar(120) not null,
  -- Penjelasan: Mendefinisikan session_setup_revisions.setup_json sebagai jsonb untuk dokumen konfigurasi setup sesi dalam JSONB. NOT NULL mewajibkan nilai tersedia pada setiap rekaman.
  setup_json jsonb not null,
  -- Penjelasan: Mendefinisikan session_setup_revisions.saved_at sebagai timestamptz untuk waktu revisi setup disimpan. NOT NULL mewajibkan nilai tersedia pada setiap rekaman. DEFAULT now() dipakai ketika INSERT tidak menyediakan nilai kolom.
  saved_at timestamptz not null default now(),
  -- Penjelasan: Mendefinisikan session_setup_revisions.locked_at sebagai timestamptz untuk waktu setup dikunci. NULL mengizinkan informasi ini belum tersedia.
  locked_at timestamptz null,
  -- Penjelasan: Mendefinisikan session_setup_revisions.created_by_user_id sebagai uuid untuk UUID akun pembuat rekaman. NOT NULL mewajibkan nilai tersedia pada setiap rekaman.
  created_by_user_id uuid not null,
  -- Penjelasan: Constraint pk_session_setup_revisions pada tabel session_setup_revisions: Menetapkan gabungan kolom sebagai identitas rekaman yang wajib unik dan tidak NULL.
  constraint pk_session_setup_revisions primary key (session_id, revision),
  -- Penjelasan: Constraint uq_session_setup_revisions_request pada tabel session_setup_revisions: Menolak kombinasi nilai kunci yang sama pada lebih dari satu rekaman sesuai semantik NULL PostgreSQL.
  constraint uq_session_setup_revisions_request unique (created_by_user_id, client_request_id),
  -- Penjelasan: Constraint ck_session_setup_revisions_revision pada tabel session_setup_revisions: Memeriksa predikat validitas; PostgreSQL menolak baris bila hasil CHECK bernilai FALSE.
  constraint ck_session_setup_revisions_revision check (revision > 0),
  -- Penjelasan: Constraint ck_session_setup_revisions_request pada tabel session_setup_revisions: Memeriksa predikat validitas; PostgreSQL menolak baris bila hasil CHECK bernilai FALSE.  NULLIF mengubah dua nilai yang sama menjadi NULL sebelum pemeriksaan atau konversi lanjutan.
  constraint ck_session_setup_revisions_request check (nullif(btrim(client_request_id), '') is not null),
  -- Penjelasan: Constraint fk_session_setup_revisions_session pada tabel session_setup_revisions: Mewajibkan nilai relasi menunjuk kunci induk yang benar agar data tidak menjadi referensi yatim. ON DELETE RESTRICT menolak penghapusan induk yang masih dirujuk.
  constraint fk_session_setup_revisions_session foreign key (session_id) references sessions (session_id) on delete restrict,
  -- Penjelasan: Constraint fk_session_setup_revisions_ruleset_version pada tabel session_setup_revisions: Mewajibkan nilai relasi menunjuk kunci induk yang benar agar data tidak menjadi referensi yatim. ON DELETE RESTRICT menolak penghapusan induk yang masih dirujuk.
  constraint fk_session_setup_revisions_ruleset_version foreign key (ruleset_version_id) references ruleset_versions (ruleset_version_id) on delete restrict,
  -- Penjelasan: Constraint fk_session_setup_revisions_created_by pada tabel session_setup_revisions: Mewajibkan nilai relasi menunjuk kunci induk yang benar agar data tidak menjadi referensi yatim. ON DELETE RESTRICT menolak penghapusan induk yang masih dirujuk.
  constraint fk_session_setup_revisions_created_by foreign key (created_by_user_id) references app_users (user_id) on delete restrict
-- Penjelasan: Menutup kelompok yang terkait if not exists session_setup_revisions pada tabel session_setup_revisions. Titik koma menyelesaikan pernyataan SQL atau instruksi prosedural.
);

-- Penjelasan: Memulai blok anonim PL/pgSQL yang dijalankan sekali oleh skrip. Pembatas $$ membungkus badan prosedural untuk validasi awal atau pengolahan seed; DECLARE, jika ada, mendefinisikan variabel lokal.
do $$
-- Penjelasan: Memulai bagian eksekusi skrip basis data; pernyataan setelahnya memakai variabel dan konteks transaksi yang tersedia.
begin
  -- Penjelasan: Mengevaluasi syarat to_regclass('public.session_setups') is not null then pada skrip basis data; bagian THEN dijalankan hanya saat syarat TRUE, sehingga perubahan data atau penolakan event mengikuti keadaan yang sedang diuji.
  if to_regclass('public.session_setups') is not null then
    -- Penjelasan: Mengevaluasi syarat exists ( pada skrip basis data; bagian THEN dijalankan hanya saat syarat TRUE, sehingga perubahan data atau penolakan event mengikuti keadaan yang sedang diuji. EXISTS cukup memeriksa keberadaan satu baris dan tidak memerlukan seluruh hasil subkueri.
    if exists (
      -- Penjelasan: Memulai pemilihan hasil pada skrip basis data; 1 menjadi ekspresi hasil yang dievaluasi untuk setiap baris atau kelompok.
      select 1
      -- Penjelasan: Menetapkan sumber baris information_schema.columns yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
      from information_schema.columns
      -- Penjelasan: Membatasi baris skrip basis data dengan syarat table_schema = 'public'; hanya baris dengan predikat TRUE masuk hasil atau terkena perubahan data.
      where table_schema = 'public'
        -- Penjelasan: Menambahkan syarat wajib table_name = 'session_setups' pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos.
        and table_name = 'session_setups'
        -- Penjelasan: Menambahkan syarat wajib column_name = 'setup_json' pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos.
        and column_name = 'setup_json'
    -- Penjelasan: Mengevaluasi ekspresi ) then pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
    ) then
      -- Penjelasan: Penjelasan literal SQL/teks multiline: indeks +0 adalah baris pembuka literal langsung di bawah rangkaian komentar ini. Penjelasan diletakkan di luar literal agar isi data dan checksum tetap identik.
      -- Penjelasan: Literal +0: Menjalankan SQL dinamis dari $migration$ pada skrip basis data. USING, bila ada, memasok parameter terikat; teks dollar-quoted dibiarkan utuh agar isi perintah yang dieksekusi tidak berubah.
      -- Penjelasan: Literal +1: Mengevaluasi ekspresi insert into session_setup_revisions ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. menampung hasil pada session_setup_revisions (nilai session setup revisions).
      -- Penjelasan: Literal +2: Memakai session_id (UUID sesi permainan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +3: Memakai revision (nomor revisi setup) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +4: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +5: Memakai client_request_id (kunci permintaan klien untuk mencegah pencatatan ganda) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +6: Memakai setup_json (dokumen konfigurasi setup sesi dalam JSONB) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +7: Memakai saved_at (waktu revisi setup disimpan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +8: Memakai locked_at (waktu setup dikunci) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +9: Memakai created_by_user_id (UUID akun pembuat rekaman) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +10: Menutup kelompok ekspresi atau daftar pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
      -- Penjelasan: Literal +11: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
      -- Penjelasan: Literal +12: Memakai session_id (UUID sesi permainan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +13: Memakai 1 (nilai 1) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +14: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +15: Memakai client_request_id (kunci permintaan klien untuk mencegah pencatatan ganda) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +16: Memakai setup_json (dokumen konfigurasi setup sesi dalam JSONB) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +17: Memakai locked_at (waktu setup dikunci) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +18: Memakai locked_at (waktu setup dikunci) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +19: Memakai created_by_user_id (UUID akun pembuat rekaman) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +20: Menetapkan sumber baris session_setups yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
      -- Penjelasan: Literal +21: Menetapkan penanganan konflik kunci unik pada INSERT: on conflict do nothing. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
      -- Penjelasan: Literal +22: Mengevaluasi ekspresi $migration$; pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
      execute $migration$
      insert into session_setup_revisions (
        session_id,
        revision,
        ruleset_version_id,
        client_request_id,
        setup_json,
        saved_at,
        locked_at,
        created_by_user_id
      )
      select
        session_id,
        1,
        ruleset_version_id,
        client_request_id,
        setup_json,
        locked_at,
        locked_at,
        created_by_user_id
      from session_setups
      on conflict do nothing
      $migration$;
    -- Penjelasan: Memeriksa alternatif kondisi exists ( hanya jika cabang IF sebelumnya tidak terpenuhi pada skrip basis data. EXISTS cukup memeriksa keberadaan satu baris dan tidak memerlukan seluruh hasil subkueri.
    elsif exists (
      -- Penjelasan: Memulai pemilihan hasil pada skrip basis data; 1 menjadi ekspresi hasil yang dievaluasi untuk setiap baris atau kelompok.
      select 1
      -- Penjelasan: Menetapkan sumber baris information_schema.columns yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
      from information_schema.columns
      -- Penjelasan: Membatasi baris skrip basis data dengan syarat table_schema = 'public'; hanya baris dengan predikat TRUE masuk hasil atau terkena perubahan data.
      where table_schema = 'public'
        -- Penjelasan: Menambahkan syarat wajib table_name = 'session_setups' pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos.
        and table_name = 'session_setups'
        -- Penjelasan: Menambahkan syarat wajib column_name = 'setup_payload' pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos.
        and column_name = 'setup_payload'
    -- Penjelasan: Mengevaluasi ekspresi ) then pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
    ) then
      -- Penjelasan: Penjelasan literal SQL/teks multiline: indeks +0 adalah baris pembuka literal langsung di bawah rangkaian komentar ini. Penjelasan diletakkan di luar literal agar isi data dan checksum tetap identik.
      -- Penjelasan: Literal +0: Menjalankan SQL dinamis dari $migration$ pada skrip basis data. USING, bila ada, memasok parameter terikat; teks dollar-quoted dibiarkan utuh agar isi perintah yang dieksekusi tidak berubah.
      -- Penjelasan: Literal +1: Mengevaluasi ekspresi insert into session_setup_revisions ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. menampung hasil pada session_setup_revisions (nilai session setup revisions).
      -- Penjelasan: Literal +2: Memakai session_id (UUID sesi permainan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +3: Memakai revision (nomor revisi setup) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +4: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +5: Memakai client_request_id (kunci permintaan klien untuk mencegah pencatatan ganda) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +6: Memakai setup_json (dokumen konfigurasi setup sesi dalam JSONB) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +7: Memakai saved_at (waktu revisi setup disimpan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +8: Memakai locked_at (waktu setup dikunci) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +9: Memakai created_by_user_id (UUID akun pembuat rekaman) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +10: Menutup kelompok ekspresi atau daftar pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
      -- Penjelasan: Literal +11: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
      -- Penjelasan: Literal +12: Memakai session_id (UUID sesi permainan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +13: Memakai 1 (nilai 1) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +14: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +15: Memakai client_request_id (kunci permintaan klien untuk mencegah pencatatan ganda) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +16: Memakai setup_payload (nilai setup payload) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +17: Memakai locked_at (waktu setup dikunci) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +18: Memakai locked_at (waktu setup dikunci) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +19: Memakai created_by_user_id (UUID akun pembuat rekaman) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
      -- Penjelasan: Literal +20: Menetapkan sumber baris session_setups yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
      -- Penjelasan: Literal +21: Menetapkan penanganan konflik kunci unik pada INSERT: on conflict do nothing. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
      -- Penjelasan: Literal +22: Mengevaluasi ekspresi $migration$; pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
      execute $migration$
        insert into session_setup_revisions (
          session_id,
          revision,
          ruleset_version_id,
          client_request_id,
          setup_json,
          saved_at,
          locked_at,
          created_by_user_id
        )
        select
          session_id,
          1,
          ruleset_version_id,
          client_request_id,
          setup_payload,
          locked_at,
          locked_at,
          created_by_user_id
        from session_setups
        on conflict do nothing
      $migration$;
    -- Penjelasan: Menutup cabang IF pada skrip basis data; eksekusi melanjutkan pernyataan setelah pemeriksaan tersebut bila tidak dihentikan oleh RETURN atau RAISE EXCEPTION.
    end if;
  -- Penjelasan: Menutup cabang IF pada skrip basis data; eksekusi melanjutkan pernyataan setelah pemeriksaan tersebut bila tidak dihentikan oleh RETURN atau RAISE EXCEPTION.
  end if;
-- Penjelasan: Menutup ekspresi CASE atau blok prosedural pada skrip basis data; akhiran AS, koma, atau titik koma menentukan apakah hasil diberi nama, dipakai dalam daftar, atau pernyataan diakhiri.
end
-- Penjelasan: Menutup badan prosedural skrip basis data pada delimiter $$; titik koma menyelesaikan deklarasi atau blok anonim yang dikirim ke PostgreSQL.
$$;

-- Penjelasan: Mengubah struktur tabel app_users (menyimpan akun autentikasi instruktur dan pemain beserta hash kata sandinya); operasi ADD/ALTER berikut diterapkan ke tabel yang sudah ada.
alter table app_users
  -- Penjelasan: Menambahkan kolom is_demo dengan tipe, aturan NULL, dan nilai default yang tertulis. IF NOT EXISTS, bila ada, memungkinkan migrasi diulang pada skema yang sudah memiliki kolom.
  add column if not exists is_demo boolean not null default false;

-- Penjelasan: Memperbarui tabel app_users; SET mengatur nilai baru dan WHERE membatasi rekaman sasaran.
update app_users
-- Penjelasan: Membuka penetapan nilai is_demo = true dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
set is_demo = true
-- Penjelasan: Membatasi baris skrip basis data dengan syarat lower(username::text) in ('rina.kartika', 'marco', 'marcello', 'hugo', 'manalu');; hanya baris dengan predikat TRUE masuk hasil atau terkena perubahan data. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai; LOWER menormalkan huruf untuk pencocokan kode/nama tanpa perbedaan kapital.
where lower(username::text) in ('rina.kartika', 'marco', 'marcello', 'hugo', 'manalu');

-- Penjelasan: Membentuk indeks ix_session_setup_revisions_latest untuk kolom dan urutan yang dinyatakan. Indeks mempercepat pencarian atau pengurutan dengan pola kolom tersebut. IF NOT EXISTS mempertahankan indeks bernama sama; WHERE, bila dilanjutkan, membatasi indeks menjadi parsial.
create index if not exists ix_session_setup_revisions_latest
  -- Penjelasan: Menetapkan kondisi pemasangan atau sasaran trigger session_setup_revisions (session_id, revision desc); pada skrip basis data; relasi kolom menjaga pasangan baris sesuai lingkup yang dinyatakan.
  on session_setup_revisions (session_id, revision desc);

-- Penjelasan: Membentuk indeks ix_events_session_received_cursor untuk kolom dan urutan yang dinyatakan. Indeks mempercepat pencarian atau pengurutan dengan pola kolom tersebut. IF NOT EXISTS mempertahankan indeks bernama sama; WHERE, bila dilanjutkan, membatasi indeks menjadi parsial.
create index if not exists ix_events_session_received_cursor
  -- Penjelasan: Menetapkan kondisi pemasangan atau sasaran trigger events (session_id, received_at, event_pk); pada skrip basis data; relasi kolom menjaga pasangan baris sesuai lingkup yang dinyatakan.
  on events (session_id, received_at, event_pk);
