-- Fungsi file: Membatasi isi dan retensi log operasional tanpa menghapus jejak migrasi lama.
-- Salinan pembelajaran berkomentar dari database/migrations/V004__limit_operational_logs.sql; tidak dibaca oleh initializer aplikasi.
-- Berkas sumber migrasi dipertahankan persis karena schema_history memverifikasi checksum seluruh isi file.
-- Penjelasan: Mengubah struktur tabel validation_logs (mencatat kegagalan validasi permintaan beserta identitas pelacakan yang dibatasi); operasi ADD/ALTER berikut diterapkan ke tabel yang sudah ada.
alter table validation_logs
  -- Penjelasan: Menambahkan kolom status_code dengan tipe, aturan NULL, dan nilai default yang tertulis. IF NOT EXISTS, bila ada, memungkinkan migrasi diulang pada skema yang sudah memiliki kolom.
  add column if not exists status_code integer not null default 422,
  -- Penjelasan: Menambahkan kolom trace_id dengan tipe, aturan NULL, dan nilai default yang tertulis. IF NOT EXISTS, bila ada, memungkinkan migrasi diulang pada skema yang sudah memiliki kolom.
  add column if not exists trace_id varchar(64) not null default 'legacy';

-- Penjelasan: Mengubah struktur tabel validation_logs (mencatat kegagalan validasi permintaan beserta identitas pelacakan yang dibatasi); operasi ADD/ALTER berikut diterapkan ke tabel yang sudah ada.
alter table validation_logs
  -- Penjelasan: Mengubah aturan kolom raw_payload_json: set default '{}'::jsonb,. Nilai default memengaruhi INSERT berikut yang mengabaikan kolom tersebut. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
  alter column raw_payload_json set default '{}'::jsonb,
  -- Penjelasan: Mengubah aturan kolom details_json: set default '{}'::jsonb;. Nilai default memengaruhi INSERT berikut yang mengabaikan kolom tersebut. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
  alter column details_json set default '{}'::jsonb;

-- Penjelasan: Memperbarui tabel validation_logs; SET mengatur nilai baru dan WHERE membatasi rekaman sasaran.
update validation_logs
-- Penjelasan: Membuka penetapan nilai raw_payload_json = '{}'::jsonb, dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
set raw_payload_json = '{}'::jsonb,
    -- Penjelasan: Menetapkan atau membandingkan details_json (rincian tambahan log dalam JSONB) terhadap '{}'::jsonb pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
    details_json = '{}'::jsonb
-- Penjelasan: Membatasi baris skrip basis data dengan syarat raw_payload_json <> '{}'::jsonb; hanya baris dengan predikat TRUE masuk hasil atau terkena perubahan data. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
where raw_payload_json <> '{}'::jsonb
   -- Penjelasan: Menambahkan alternatif coalesce(details_json, '{}'::jsonb) <> '{}'::jsonb; pada skrip basis data; OR menerima keadaan ketika setidaknya satu cabang kondisi bernilai TRUE, dengan prioritas mengikuti tanda kurung. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
   or coalesce(details_json, '{}'::jsonb) <> '{}'::jsonb;

-- Penjelasan: Menempatkan log_retention_policies.table_name (nama tabel yang dikenai kebijakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
insert into log_retention_policies (table_name, retention_days)
-- Penjelasan: Menyediakan 2 tuple nilai eksplisit untuk log_retention_policies; setiap tuple membentuk satu rekaman dan mengikuti urutan table_name, retention_days.
values
  -- Penjelasan: Membuka rekaman seed table_name='validation_logs'; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
  ('validation_logs', 30),
  -- Penjelasan: Membuka rekaman seed table_name='security_audit_logs'; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
  ('security_audit_logs', 30)
-- Penjelasan: Menetapkan penanganan konflik kunci unik pada INSERT: on conflict (table_name) do update. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
on conflict (table_name) do update
-- Penjelasan: Membuka penetapan nilai retention_days = excluded.retention_days, dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
set retention_days = excluded.retention_days,
    -- Penjelasan: Menetapkan atau membandingkan updated_at (waktu perubahan terakhir) terhadap now(); pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. NOW mengambil waktu awal transaksi PostgreSQL.
    updated_at = now();
