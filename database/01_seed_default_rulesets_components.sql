-- Fungsi file: Mengisi dua set aturan bawaan beserta seluruh komponen relasional mode Pemula dan Mahir secara idempoten.
-- Penjelasan per baris sumber (nomor dihitung pada kode asli, sebelum blok ini ditambahkan).
-- Uraian sumber 2: Mengaktifkan pgcrypto bila belum tersedia untuk kebutuhan fungsi kriptografi dan identitas acak yang digunakan skema/seed; IF NOT EXISTS menjaga langkah inisialisasi tetap dapat diulang.
-- Uraian sumber 4: Memulai transaksi eksplisit sehingga perubahan skema atau seed sesudah titik ini disahkan bersama oleh COMMIT; kegagalan sebelum commit dapat menggagalkan keseluruhan unit perubahan.
-- Uraian sumber 6: Membentuk tabel seed_legacy_action_ids untuk menampung daftar aksi lama yang akan dibersihkan bila tidak dirujuk histori. TEMPORARY membatasi tabel pembantu pada sesi koneksi dan ON COMMIT DROP, bila dicantumkan, menghapusnya ketika transaksi disahkan.
-- Uraian sumber 7: Mendefinisikan seed_legacy_action_ids.action_id sebagai text untuk kode aksi pada katalog. Kolom dapat bernilai NULL jika tidak dibatasi constraint lain. PRIMARY KEY menjadikan kolom kunci unik dan tidak boleh NULL.
-- Uraian sumber 8: Mengevaluasi ekspresi ) on commit drop; pada tabel seed_legacy_action_ids; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 10: Menempatkan seed_legacy_action_ids.action_id (kode aksi pada katalog) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 11: Menyediakan 3 tuple nilai eksplisit untuk seed_legacy_action_ids; setiap tuple membentuk satu rekaman dan mengikuti urutan action_id.
-- Uraian sumber 12: Membuka rekaman seed action_id=’aksi pembagian emas awal’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 13: Membuka rekaman seed action_id=’aksi pembagian misi koleksi’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 14: Membuka rekaman seed action_id=’aksi pengambilan kartu dari pasar’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 15: Mengakhiri kelompok ekspresi atau daftar pada skrip basis data. Titik koma menyelesaikan pernyataan SQL atau instruksi prosedural.
-- Uraian sumber 17: Memulai penghapusan baris pada ruleset_actions legacy; klausa WHERE/USING berikut menentukan lingkup rekaman yang boleh dihapus agar relasi atau data di luar sasaran tetap terlindungi.
-- Uraian sumber 18: Menentukan sumber tambahan atau parameter terikat melalui using seed_legacy_action_ids legacy_id; arti USING mengikuti pernyataan DELETE, EXECUTE, atau RAISE yang sedang dibentuk.
-- Uraian sumber 19: Membatasi baris skrip basis data dengan syarat legacy.action_id = legacy_id.action_id; hanya baris dengan predikat TRUE masuk hasil atau terkena perubahan data.
-- Uraian sumber 20: Menambahkan syarat wajib not exists ( pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos. EXISTS cukup memeriksa keberadaan satu baris dan tidak memerlukan seluruh hasil subkueri.
-- Uraian sumber 21: Memulai pemilihan hasil pada skrip basis data; 1 menjadi ekspresi hasil yang dievaluasi untuk setiap baris atau kelompok.
-- Uraian sumber 22: Menetapkan sumber baris events event_log yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 23: Membatasi baris skrip basis data dengan syarat event_log.ruleset_action_id = legacy.ruleset_action_id; hanya baris dengan predikat TRUE masuk hasil atau terkena perubahan data.
-- Uraian sumber 24: Menutup kelompok yang terkait action_id and not exists pada skrip basis data. Titik koma menyelesaikan pernyataan SQL atau instruksi prosedural.
-- Uraian sumber 26: Memulai penghapusan baris pada actions legacy; klausa WHERE/USING berikut menentukan lingkup rekaman yang boleh dihapus agar relasi atau data di luar sasaran tetap terlindungi.
-- Uraian sumber 27: Menentukan sumber tambahan atau parameter terikat melalui using seed_legacy_action_ids legacy_id; arti USING mengikuti pernyataan DELETE, EXECUTE, atau RAISE yang sedang dibentuk.
-- Uraian sumber 28: Membatasi baris skrip basis data dengan syarat legacy.action_id = legacy_id.action_id; hanya baris dengan predikat TRUE masuk hasil atau terkena perubahan data.
-- Uraian sumber 29: Menambahkan syarat wajib not exists ( pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos. EXISTS cukup memeriksa keberadaan satu baris dan tidak memerlukan seluruh hasil subkueri.
-- Uraian sumber 30: Memulai pemilihan hasil pada skrip basis data; 1 menjadi ekspresi hasil yang dievaluasi untuk setiap baris atau kelompok.
-- Uraian sumber 31: Menetapkan sumber baris ruleset_actions ruleset_action yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 32: Membatasi baris skrip basis data dengan syarat ruleset_action.action_id = legacy.action_id; hanya baris dengan predikat TRUE masuk hasil atau terkena perubahan data.
-- Uraian sumber 33: Menutup kelompok yang terkait action_id and not exists pada skrip basis data. Titik koma menyelesaikan pernyataan SQL atau instruksi prosedural.
-- Uraian sumber 36: Memulai penyisipan rekaman ke actions, tabel yang menyimpan katalog aksi dan jenis perubahan keadaan permainan yang diizinkan; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 37: Mengevaluasi ekspresi actions ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 38: Menempatkan actions.action_id (kode aksi pada katalog) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 39: Menempatkan actions.action_name (nama aksi untuk tampilan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 40: Menempatkan actions.behavior_id (kode perilaku domain yang menjalankan efek aksi) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 41: Menempatkan actions.mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 42: Menempatkan actions.cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 43: Menempatkan actions.affects_coin (penanda aksi dapat memengaruhi koin) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 44: Menempatkan actions.affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 45: Menempatkan actions.affects_saving (penanda aksi dapat memengaruhi tabungan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 46: Menempatkan actions.affects_inventory (penanda aksi dapat memengaruhi inventaris) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 47: Menempatkan actions.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 48: Menutup kelompok yang terkait ; insert into actions pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 49: Menyediakan 34 tuple nilai eksplisit untuk actions; setiap tuple membentuk satu rekaman dan mengikuti urutan action_id, action_name, behavior_id, mode, cashflow_direction, affects_coin, affects_happiness, affects_saving, affects_inventory, is_active.
-- Uraian sumber 50: Membuka rekaman seed action_id=’BahanMasakan’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 51: Mengisi action_id (kode aksi pada katalog) dengan ’BahanMasakan’ untuk action_id=’BahanMasakan’, mode=’BOTH’.
-- Uraian sumber 52: Mengisi action_name (nama aksi untuk tampilan) dengan ’Beli Bahan Masakan’ untuk action_id=’BahanMasakan’, mode=’BOTH’.
-- Uraian sumber 53: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’BahanMasakan’ untuk action_id=’BahanMasakan’, mode=’BOTH’.
-- Uraian sumber 54: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’BahanMasakan’, mode=’BOTH’.
-- Uraian sumber 55: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan ’OUT’ untuk action_id=’BahanMasakan’, mode=’BOTH’.
-- Uraian sumber 56: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’BahanMasakan’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 57: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’BahanMasakan’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 58: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’BahanMasakan’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 59: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan true untuk action_id=’BahanMasakan’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 60: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’BahanMasakan’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 61: Menutup rekaman seed action_id=’BahanMasakan’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 62: Membuka rekaman seed action_id=’BuangBahanMasakan’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 63: Mengisi action_id (kode aksi pada katalog) dengan ’BuangBahanMasakan’ untuk action_id=’BuangBahanMasakan’, mode=’BOTH’.
-- Uraian sumber 64: Mengisi action_name (nama aksi untuk tampilan) dengan ’Buang Bahan Masakan’ untuk action_id=’BuangBahanMasakan’, mode=’BOTH’.
-- Uraian sumber 65: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’BuangBahanMasakan’ untuk action_id=’BuangBahanMasakan’, mode=’BOTH’.
-- Uraian sumber 66: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’BuangBahanMasakan’, mode=’BOTH’.
-- Uraian sumber 67: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’BuangBahanMasakan’, mode=’BOTH’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 68: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan false untuk action_id=’BuangBahanMasakan’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 69: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’BuangBahanMasakan’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 70: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’BuangBahanMasakan’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 71: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan true untuk action_id=’BuangBahanMasakan’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 72: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’BuangBahanMasakan’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 73: Menutup rekaman seed action_id=’BuangBahanMasakan’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 74: Membuka rekaman seed action_id=’JualMasakan’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 75: Mengisi action_id (kode aksi pada katalog) dengan ’JualMasakan’ untuk action_id=’JualMasakan’, mode=’BOTH’.
-- Uraian sumber 76: Mengisi action_name (nama aksi untuk tampilan) dengan ’Jual Masakan’ untuk action_id=’JualMasakan’, mode=’BOTH’.
-- Uraian sumber 77: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’JualMasakan’ untuk action_id=’JualMasakan’, mode=’BOTH’.
-- Uraian sumber 78: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’JualMasakan’, mode=’BOTH’.
-- Uraian sumber 79: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan ’IN’ untuk action_id=’JualMasakan’, mode=’BOTH’.
-- Uraian sumber 80: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’JualMasakan’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 81: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’JualMasakan’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 82: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’JualMasakan’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 83: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan true untuk action_id=’JualMasakan’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 84: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’JualMasakan’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 85: Menutup rekaman seed action_id=’JualMasakan’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 86: Membuka rekaman seed action_id=’Kebutuhan’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 87: Mengisi action_id (kode aksi pada katalog) dengan ’Kebutuhan’ untuk action_id=’Kebutuhan’, mode=’BOTH’.
-- Uraian sumber 88: Mengisi action_name (nama aksi untuk tampilan) dengan ’Beli Kebutuhan’ untuk action_id=’Kebutuhan’, mode=’BOTH’.
-- Uraian sumber 89: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’Kebutuhan’ untuk action_id=’Kebutuhan’, mode=’BOTH’.
-- Uraian sumber 90: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’Kebutuhan’, mode=’BOTH’.
-- Uraian sumber 91: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan ’OUT’ untuk action_id=’Kebutuhan’, mode=’BOTH’.
-- Uraian sumber 92: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’Kebutuhan’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 93: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan true untuk action_id=’Kebutuhan’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 94: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’Kebutuhan’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 95: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’Kebutuhan’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 96: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’Kebutuhan’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 97: Menutup rekaman seed action_id=’Kebutuhan’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 98: Membuka rekaman seed action_id=’KerjaLepas’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 99: Mengisi action_id (kode aksi pada katalog) dengan ’KerjaLepas’ untuk action_id=’KerjaLepas’, mode=’BOTH’.
-- Uraian sumber 100: Mengisi action_name (nama aksi untuk tampilan) dengan ’Kerja Lepas’ untuk action_id=’KerjaLepas’, mode=’BOTH’.
-- Uraian sumber 101: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’KerjaLepas’ untuk action_id=’KerjaLepas’, mode=’BOTH’.
-- Uraian sumber 102: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’KerjaLepas’, mode=’BOTH’.
-- Uraian sumber 103: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan ’IN’ untuk action_id=’KerjaLepas’, mode=’BOTH’.
-- Uraian sumber 104: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’KerjaLepas’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 105: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’KerjaLepas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 106: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’KerjaLepas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 107: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’KerjaLepas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 108: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’KerjaLepas’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 109: Menutup rekaman seed action_id=’KerjaLepas’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 110: Membuka rekaman seed action_id=’CatatTransaksi’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 111: Mengisi action_id (kode aksi pada katalog) dengan ’CatatTransaksi’ untuk action_id=’CatatTransaksi’, mode=’BOTH’.
-- Uraian sumber 112: Mengisi action_name (nama aksi untuk tampilan) dengan ’Catat Transaksi’ untuk action_id=’CatatTransaksi’, mode=’BOTH’.
-- Uraian sumber 113: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’CatatTransaksi’ untuk action_id=’CatatTransaksi’, mode=’BOTH’.
-- Uraian sumber 114: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’CatatTransaksi’, mode=’BOTH’.
-- Uraian sumber 115: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’CatatTransaksi’, mode=’BOTH’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 116: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’CatatTransaksi’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 117: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’CatatTransaksi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 118: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan true untuk action_id=’CatatTransaksi’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 119: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’CatatTransaksi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 120: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’CatatTransaksi’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 121: Menutup rekaman seed action_id=’CatatTransaksi’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 122: Membuka rekaman seed action_id=’Menabung’, mode=’MAHIR’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 123: Mengisi action_id (kode aksi pada katalog) dengan ’Menabung’ untuk action_id=’Menabung’, mode=’MAHIR’.
-- Uraian sumber 124: Mengisi action_name (nama aksi untuk tampilan) dengan ’Menabung’ untuk action_id=’Menabung’, mode=’MAHIR’.
-- Uraian sumber 125: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’Menabung’ untuk action_id=’Menabung’, mode=’MAHIR’.
-- Uraian sumber 126: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’MAHIR’ untuk action_id=’Menabung’, mode=’MAHIR’.
-- Uraian sumber 127: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan ’OUT’ untuk action_id=’Menabung’, mode=’MAHIR’.
-- Uraian sumber 128: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’Menabung’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 129: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’Menabung’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 130: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan true untuk action_id=’Menabung’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 131: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’Menabung’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 132: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’Menabung’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 133: Menutup rekaman seed action_id=’Menabung’, mode=’MAHIR’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 134: Membuka rekaman seed action_id=’TujuanFinansial’, mode=’MAHIR’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 135: Mengisi action_id (kode aksi pada katalog) dengan ’TujuanFinansial’ untuk action_id=’TujuanFinansial’, mode=’MAHIR’.
-- Uraian sumber 136: Mengisi action_name (nama aksi untuk tampilan) dengan ’Tujuan Finansial’ untuk action_id=’TujuanFinansial’, mode=’MAHIR’.
-- Uraian sumber 137: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’TujuanFinansial’ untuk action_id=’TujuanFinansial’, mode=’MAHIR’.
-- Uraian sumber 138: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’MAHIR’ untuk action_id=’TujuanFinansial’, mode=’MAHIR’.
-- Uraian sumber 139: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan ’OUT’ untuk action_id=’TujuanFinansial’, mode=’MAHIR’.
-- Uraian sumber 140: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’TujuanFinansial’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 141: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan true untuk action_id=’TujuanFinansial’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 142: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan true untuk action_id=’TujuanFinansial’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 143: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’TujuanFinansial’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 144: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’TujuanFinansial’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 145: Menutup rekaman seed action_id=’TujuanFinansial’, mode=’MAHIR’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 146: Membuka rekaman seed action_id=’JumatBerkah’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 147: Mengisi action_id (kode aksi pada katalog) dengan ’JumatBerkah’ untuk action_id=’JumatBerkah’, mode=’BOTH’.
-- Uraian sumber 148: Mengisi action_name (nama aksi untuk tampilan) dengan ’Peduli Donasi’ untuk action_id=’JumatBerkah’, mode=’BOTH’.
-- Uraian sumber 149: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’JumatBerkah’ untuk action_id=’JumatBerkah’, mode=’BOTH’.
-- Uraian sumber 150: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’JumatBerkah’, mode=’BOTH’.
-- Uraian sumber 151: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan ’OUT’ untuk action_id=’JumatBerkah’, mode=’BOTH’.
-- Uraian sumber 152: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’JumatBerkah’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 153: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’JumatBerkah’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 154: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’JumatBerkah’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 155: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’JumatBerkah’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 156: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’JumatBerkah’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 157: Menutup rekaman seed action_id=’JumatBerkah’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 158: Membuka rekaman seed action_id=’InvestasiEmas’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 159: Mengisi action_id (kode aksi pada katalog) dengan ’InvestasiEmas’ untuk action_id=’InvestasiEmas’, mode=’BOTH’.
-- Uraian sumber 160: Mengisi action_name (nama aksi untuk tampilan) dengan ’Investasi Emas’ untuk action_id=’InvestasiEmas’, mode=’BOTH’.
-- Uraian sumber 161: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’InvestasiEmas’ untuk action_id=’InvestasiEmas’, mode=’BOTH’.
-- Uraian sumber 162: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’InvestasiEmas’, mode=’BOTH’.
-- Uraian sumber 163: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan ’OUT’ untuk action_id=’InvestasiEmas’, mode=’BOTH’.
-- Uraian sumber 164: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’InvestasiEmas’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 165: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’InvestasiEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 166: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’InvestasiEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 167: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’InvestasiEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 168: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’InvestasiEmas’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 169: Menutup rekaman seed action_id=’InvestasiEmas’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 170: Membuka rekaman seed action_id=’JualEmas’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 171: Mengisi action_id (kode aksi pada katalog) dengan ’JualEmas’ untuk action_id=’JualEmas’, mode=’BOTH’.
-- Uraian sumber 172: Mengisi action_name (nama aksi untuk tampilan) dengan ’Jual Emas’ untuk action_id=’JualEmas’, mode=’BOTH’.
-- Uraian sumber 173: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’JualEmas’ untuk action_id=’JualEmas’, mode=’BOTH’.
-- Uraian sumber 174: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’JualEmas’, mode=’BOTH’.
-- Uraian sumber 175: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan ’IN’ untuk action_id=’JualEmas’, mode=’BOTH’.
-- Uraian sumber 176: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’JualEmas’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 177: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’JualEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 178: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’JualEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 179: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’JualEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 180: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’JualEmas’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 181: Menutup rekaman seed action_id=’JualEmas’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 182: Membuka rekaman seed action_id=’LewatiTransaksiEmas’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 183: Mengisi action_id (kode aksi pada katalog) dengan ’LewatiTransaksiEmas’ untuk action_id=’LewatiTransaksiEmas’, mode=’BOTH’.
-- Uraian sumber 184: Mengisi action_name (nama aksi untuk tampilan) dengan ’Lewati Transaksi Emas’ untuk action_id=’LewatiTransaksiEmas’, mode=’BOTH’.
-- Uraian sumber 185: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’LewatiTransaksiEmas’ untuk action_id=’LewatiTransaksiEmas’, mode=’BOTH’.
-- Uraian sumber 186: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’LewatiTransaksiEmas’, mode=’BOTH’.
-- Uraian sumber 187: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’LewatiTransaksiEmas’, mode=’BOTH’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 188: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan false untuk action_id=’LewatiTransaksiEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 189: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’LewatiTransaksiEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 190: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’LewatiTransaksiEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 191: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’LewatiTransaksiEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 192: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’LewatiTransaksiEmas’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 193: Menutup rekaman seed action_id=’LewatiTransaksiEmas’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 194: Membuka rekaman seed action_id=’HariMingguLibur’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 195: Mengisi action_id (kode aksi pada katalog) dengan ’HariMingguLibur’ untuk action_id=’HariMingguLibur’, mode=’BOTH’.
-- Uraian sumber 196: Mengisi action_name (nama aksi untuk tampilan) dengan ’Hari Minggu Libur’ untuk action_id=’HariMingguLibur’, mode=’BOTH’.
-- Uraian sumber 197: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’HariMingguLibur’ untuk action_id=’HariMingguLibur’, mode=’BOTH’.
-- Uraian sumber 198: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’HariMingguLibur’, mode=’BOTH’.
-- Uraian sumber 199: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’HariMingguLibur’, mode=’BOTH’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 200: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan false untuk action_id=’HariMingguLibur’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 201: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’HariMingguLibur’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 202: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’HariMingguLibur’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 203: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’HariMingguLibur’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 204: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’HariMingguLibur’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 205: Menutup rekaman seed action_id=’HariMingguLibur’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 206: Membuka rekaman seed action_id=’PinjamanSyariah’, mode=’MAHIR’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 207: Mengisi action_id (kode aksi pada katalog) dengan ’PinjamanSyariah’ untuk action_id=’PinjamanSyariah’, mode=’MAHIR’.
-- Uraian sumber 208: Mengisi action_name (nama aksi untuk tampilan) dengan ’Pinjaman Syariah’ untuk action_id=’PinjamanSyariah’, mode=’MAHIR’.
-- Uraian sumber 209: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’PinjamanSyariah’ untuk action_id=’PinjamanSyariah’, mode=’MAHIR’.
-- Uraian sumber 210: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’MAHIR’ untuk action_id=’PinjamanSyariah’, mode=’MAHIR’.
-- Uraian sumber 211: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan ’IN’ untuk action_id=’PinjamanSyariah’, mode=’MAHIR’.
-- Uraian sumber 212: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’PinjamanSyariah’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 213: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’PinjamanSyariah’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 214: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’PinjamanSyariah’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 215: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’PinjamanSyariah’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 216: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’PinjamanSyariah’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 217: Menutup rekaman seed action_id=’PinjamanSyariah’, mode=’MAHIR’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 218: Membuka rekaman seed action_id=’BayarPinjaman’, mode=’MAHIR’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 219: Mengisi action_id (kode aksi pada katalog) dengan ’BayarPinjaman’ untuk action_id=’BayarPinjaman’, mode=’MAHIR’.
-- Uraian sumber 220: Mengisi action_name (nama aksi untuk tampilan) dengan ’Bayar Pinjaman’ untuk action_id=’BayarPinjaman’, mode=’MAHIR’.
-- Uraian sumber 221: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’BayarPinjaman’ untuk action_id=’BayarPinjaman’, mode=’MAHIR’.
-- Uraian sumber 222: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’MAHIR’ untuk action_id=’BayarPinjaman’, mode=’MAHIR’.
-- Uraian sumber 223: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan ’OUT’ untuk action_id=’BayarPinjaman’, mode=’MAHIR’.
-- Uraian sumber 224: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’BayarPinjaman’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 225: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’BayarPinjaman’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 226: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’BayarPinjaman’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 227: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’BayarPinjaman’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 228: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’BayarPinjaman’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 229: Menutup rekaman seed action_id=’BayarPinjaman’, mode=’MAHIR’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 230: Membuka rekaman seed action_id=’Asuransi’, mode=’MAHIR’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 231: Mengisi action_id (kode aksi pada katalog) dengan ’Asuransi’ untuk action_id=’Asuransi’, mode=’MAHIR’.
-- Uraian sumber 232: Mengisi action_name (nama aksi untuk tampilan) dengan ’Asuransi’ untuk action_id=’Asuransi’, mode=’MAHIR’.
-- Uraian sumber 233: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’Asuransi’ untuk action_id=’Asuransi’, mode=’MAHIR’.
-- Uraian sumber 234: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’MAHIR’ untuk action_id=’Asuransi’, mode=’MAHIR’.
-- Uraian sumber 235: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan ’OUT’ untuk action_id=’Asuransi’, mode=’MAHIR’.
-- Uraian sumber 236: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’Asuransi’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 237: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’Asuransi’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 238: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’Asuransi’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 239: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’Asuransi’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 240: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’Asuransi’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 241: Menutup rekaman seed action_id=’Asuransi’, mode=’MAHIR’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 242: Membuka rekaman seed action_id=’RisikoKehidupan’, mode=’MAHIR’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 243: Mengisi action_id (kode aksi pada katalog) dengan ’RisikoKehidupan’ untuk action_id=’RisikoKehidupan’, mode=’MAHIR’.
-- Uraian sumber 244: Mengisi action_name (nama aksi untuk tampilan) dengan ’Risiko Kehidupan’ untuk action_id=’RisikoKehidupan’, mode=’MAHIR’.
-- Uraian sumber 245: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’RisikoKehidupan’ untuk action_id=’RisikoKehidupan’, mode=’MAHIR’.
-- Uraian sumber 246: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’MAHIR’ untuk action_id=’RisikoKehidupan’, mode=’MAHIR’.
-- Uraian sumber 247: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’RisikoKehidupan’, mode=’MAHIR’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 248: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’RisikoKehidupan’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 249: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’RisikoKehidupan’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 250: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’RisikoKehidupan’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 251: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’RisikoKehidupan’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 252: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’RisikoKehidupan’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 253: Menutup rekaman seed action_id=’RisikoKehidupan’, mode=’MAHIR’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 254: Membuka rekaman seed action_id=’BayarRisiko’, mode=’MAHIR’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 255: Mengisi action_id (kode aksi pada katalog) dengan ’BayarRisiko’ untuk action_id=’BayarRisiko’, mode=’MAHIR’.
-- Uraian sumber 256: Mengisi action_name (nama aksi untuk tampilan) dengan ’Bayar Risiko’ untuk action_id=’BayarRisiko’, mode=’MAHIR’.
-- Uraian sumber 257: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’BayarRisiko’ untuk action_id=’BayarRisiko’, mode=’MAHIR’.
-- Uraian sumber 258: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’MAHIR’ untuk action_id=’BayarRisiko’, mode=’MAHIR’.
-- Uraian sumber 259: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan ’OUT’ untuk action_id=’BayarRisiko’, mode=’MAHIR’.
-- Uraian sumber 260: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’BayarRisiko’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 261: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’BayarRisiko’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 262: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’BayarRisiko’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 263: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’BayarRisiko’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 264: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’BayarRisiko’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 265: Menutup rekaman seed action_id=’BayarRisiko’, mode=’MAHIR’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 266: Membuka rekaman seed action_id=’GunakanOpsiDarurat’, mode=’MAHIR’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 267: Mengisi action_id (kode aksi pada katalog) dengan ’GunakanOpsiDarurat’ untuk action_id=’GunakanOpsiDarurat’, mode=’MAHIR’.
-- Uraian sumber 268: Mengisi action_name (nama aksi untuk tampilan) dengan ’Gunakan Opsi Darurat’ untuk action_id=’GunakanOpsiDarurat’, mode=’MAHIR’.
-- Uraian sumber 269: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’GunakanOpsiDarurat’ untuk action_id=’GunakanOpsiDarurat’, mode=’MAHIR’.
-- Uraian sumber 270: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’MAHIR’ untuk action_id=’GunakanOpsiDarurat’, mode=’MAHIR’.
-- Uraian sumber 271: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’GunakanOpsiDarurat’, mode=’MAHIR’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 272: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’GunakanOpsiDarurat’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 273: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’GunakanOpsiDarurat’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 274: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’GunakanOpsiDarurat’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 275: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’GunakanOpsiDarurat’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 276: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’GunakanOpsiDarurat’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 277: Menutup rekaman seed action_id=’GunakanOpsiDarurat’, mode=’MAHIR’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 278: Membuka rekaman seed action_id=’PoinPeringkatDonasi’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 279: Mengisi action_id (kode aksi pada katalog) dengan ’PoinPeringkatDonasi’ untuk action_id=’PoinPeringkatDonasi’, mode=’BOTH’.
-- Uraian sumber 280: Mengisi action_name (nama aksi untuk tampilan) dengan ’Poin Peringkat Donasi’ untuk action_id=’PoinPeringkatDonasi’, mode=’BOTH’.
-- Uraian sumber 281: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’PoinPeringkatDonasi’ untuk action_id=’PoinPeringkatDonasi’, mode=’BOTH’.
-- Uraian sumber 282: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’PoinPeringkatDonasi’, mode=’BOTH’.
-- Uraian sumber 283: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’PoinPeringkatDonasi’, mode=’BOTH’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 284: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan false untuk action_id=’PoinPeringkatDonasi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 285: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan true untuk action_id=’PoinPeringkatDonasi’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 286: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’PoinPeringkatDonasi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 287: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’PoinPeringkatDonasi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 288: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’PoinPeringkatDonasi’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 289: Menutup rekaman seed action_id=’PoinPeringkatDonasi’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 290: Membuka rekaman seed action_id=’UmumkanJuaraDonasi’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 291: Mengisi action_id (kode aksi pada katalog) dengan ’UmumkanJuaraDonasi’ untuk action_id=’UmumkanJuaraDonasi’, mode=’BOTH’.
-- Uraian sumber 292: Mengisi action_name (nama aksi untuk tampilan) dengan ’Sistem: Umumkan Juara Donasi’ untuk action_id=’UmumkanJuaraDonasi’, mode=’BOTH’.
-- Uraian sumber 293: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’UmumkanJuaraDonasi’ untuk action_id=’UmumkanJuaraDonasi’, mode=’BOTH’.
-- Uraian sumber 294: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’UmumkanJuaraDonasi’, mode=’BOTH’.
-- Uraian sumber 295: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’UmumkanJuaraDonasi’, mode=’BOTH’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 296: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan false untuk action_id=’UmumkanJuaraDonasi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 297: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’UmumkanJuaraDonasi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 298: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’UmumkanJuaraDonasi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 299: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’UmumkanJuaraDonasi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 300: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’UmumkanJuaraDonasi’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 301: Menutup rekaman seed action_id=’UmumkanJuaraDonasi’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 302: Membuka rekaman seed action_id=’PoinEmas’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 303: Mengisi action_id (kode aksi pada katalog) dengan ’PoinEmas’ untuk action_id=’PoinEmas’, mode=’BOTH’.
-- Uraian sumber 304: Mengisi action_name (nama aksi untuk tampilan) dengan ’Poin Emas’ untuk action_id=’PoinEmas’, mode=’BOTH’.
-- Uraian sumber 305: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’PoinEmas’ untuk action_id=’PoinEmas’, mode=’BOTH’.
-- Uraian sumber 306: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’PoinEmas’, mode=’BOTH’.
-- Uraian sumber 307: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’PoinEmas’, mode=’BOTH’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 308: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan false untuk action_id=’PoinEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 309: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan true untuk action_id=’PoinEmas’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 310: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’PoinEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 311: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’PoinEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 312: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’PoinEmas’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 313: Menutup rekaman seed action_id=’PoinEmas’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 314: Membuka rekaman seed action_id=’PoinPeringkatPensiun’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 315: Mengisi action_id (kode aksi pada katalog) dengan ’PoinPeringkatPensiun’ untuk action_id=’PoinPeringkatPensiun’, mode=’BOTH’.
-- Uraian sumber 316: Mengisi action_name (nama aksi untuk tampilan) dengan ’Poin Peringkat Pensiun’ untuk action_id=’PoinPeringkatPensiun’, mode=’BOTH’.
-- Uraian sumber 317: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’PoinPeringkatPensiun’ untuk action_id=’PoinPeringkatPensiun’, mode=’BOTH’.
-- Uraian sumber 318: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’PoinPeringkatPensiun’, mode=’BOTH’.
-- Uraian sumber 319: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’PoinPeringkatPensiun’, mode=’BOTH’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 320: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan false untuk action_id=’PoinPeringkatPensiun’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 321: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan true untuk action_id=’PoinPeringkatPensiun’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 322: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’PoinPeringkatPensiun’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 323: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’PoinPeringkatPensiun’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 324: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’PoinPeringkatPensiun’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 325: Menutup rekaman seed action_id=’PoinPeringkatPensiun’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 326: Membuka rekaman seed action_id=’BagikanTieBreaker’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 327: Mengisi action_id (kode aksi pada katalog) dengan ’BagikanTieBreaker’ untuk action_id=’BagikanTieBreaker’, mode=’BOTH’.
-- Uraian sumber 328: Mengisi action_name (nama aksi untuk tampilan) dengan ’Sistem: Bagikan Tie Breaker’ untuk action_id=’BagikanTieBreaker’, mode=’BOTH’.
-- Uraian sumber 329: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’BagikanTieBreaker’ untuk action_id=’BagikanTieBreaker’, mode=’BOTH’.
-- Uraian sumber 330: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’BagikanTieBreaker’, mode=’BOTH’.
-- Uraian sumber 331: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’BagikanTieBreaker’, mode=’BOTH’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 332: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan false untuk action_id=’BagikanTieBreaker’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 333: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’BagikanTieBreaker’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 334: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’BagikanTieBreaker’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 335: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’BagikanTieBreaker’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 336: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’BagikanTieBreaker’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 337: Menutup rekaman seed action_id=’BagikanTieBreaker’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 338: Membuka rekaman seed action_id=’MulaiSesi’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 339: Mengisi action_id (kode aksi pada katalog) dengan ’MulaiSesi’ untuk action_id=’MulaiSesi’, mode=’BOTH’.
-- Uraian sumber 340: Mengisi action_name (nama aksi untuk tampilan) dengan ’Sistem: Mulai Sesi’ untuk action_id=’MulaiSesi’, mode=’BOTH’.
-- Uraian sumber 341: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’MulaiSesi’ untuk action_id=’MulaiSesi’, mode=’BOTH’.
-- Uraian sumber 342: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’MulaiSesi’, mode=’BOTH’.
-- Uraian sumber 343: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’MulaiSesi’, mode=’BOTH’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 344: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan false untuk action_id=’MulaiSesi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 345: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’MulaiSesi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 346: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’MulaiSesi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 347: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’MulaiSesi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 348: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’MulaiSesi’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 349: Menutup rekaman seed action_id=’MulaiSesi’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 350: Membuka rekaman seed action_id=’AkhiriSesi’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 351: Mengisi action_id (kode aksi pada katalog) dengan ’AkhiriSesi’ untuk action_id=’AkhiriSesi’, mode=’BOTH’.
-- Uraian sumber 352: Mengisi action_name (nama aksi untuk tampilan) dengan ’Sistem: Akhiri Sesi’ untuk action_id=’AkhiriSesi’, mode=’BOTH’.
-- Uraian sumber 353: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’AkhiriSesi’ untuk action_id=’AkhiriSesi’, mode=’BOTH’.
-- Uraian sumber 354: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’AkhiriSesi’, mode=’BOTH’.
-- Uraian sumber 355: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’AkhiriSesi’, mode=’BOTH’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 356: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan false untuk action_id=’AkhiriSesi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 357: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’AkhiriSesi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 358: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’AkhiriSesi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 359: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’AkhiriSesi’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 360: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’AkhiriSesi’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 361: Menutup rekaman seed action_id=’AkhiriSesi’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 362: Membuka rekaman seed action_id=’AkhirGiliran’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 363: Mengisi action_id (kode aksi pada katalog) dengan ’AkhirGiliran’ untuk action_id=’AkhirGiliran’, mode=’BOTH’.
-- Uraian sumber 364: Mengisi action_name (nama aksi untuk tampilan) dengan ’Akhir Giliran’ untuk action_id=’AkhirGiliran’, mode=’BOTH’.
-- Uraian sumber 365: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’AkhirGiliran’ untuk action_id=’AkhirGiliran’, mode=’BOTH’.
-- Uraian sumber 366: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’AkhirGiliran’, mode=’BOTH’.
-- Uraian sumber 367: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’AkhirGiliran’, mode=’BOTH’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 368: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan false untuk action_id=’AkhirGiliran’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 369: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’AkhirGiliran’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 370: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’AkhirGiliran’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 371: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’AkhirGiliran’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 372: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’AkhirGiliran’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 373: Menutup rekaman seed action_id=’AkhirGiliran’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 374: Membuka rekaman seed action_id=’BukaHargaEmas’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 375: Mengisi action_id (kode aksi pada katalog) dengan ’BukaHargaEmas’ untuk action_id=’BukaHargaEmas’, mode=’BOTH’.
-- Uraian sumber 376: Mengisi action_name (nama aksi untuk tampilan) dengan ’Buka Harga Emas’ untuk action_id=’BukaHargaEmas’, mode=’BOTH’.
-- Uraian sumber 377: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’BukaHargaEmas’ untuk action_id=’BukaHargaEmas’, mode=’BOTH’.
-- Uraian sumber 378: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’BukaHargaEmas’, mode=’BOTH’.
-- Uraian sumber 379: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’BukaHargaEmas’, mode=’BOTH’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 380: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan false untuk action_id=’BukaHargaEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 381: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’BukaHargaEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 382: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’BukaHargaEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 383: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’BukaHargaEmas’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 384: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’BukaHargaEmas’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 385: Menutup rekaman seed action_id=’BukaHargaEmas’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 386: Membuka rekaman seed action_id=’SetupModalAwal’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 387: Mengisi action_id (kode aksi pada katalog) dengan ’SetupModalAwal’ untuk action_id=’SetupModalAwal’, mode=’BOTH’.
-- Uraian sumber 388: Mengisi action_name (nama aksi untuk tampilan) dengan ’Setup Modal Awal’ untuk action_id=’SetupModalAwal’, mode=’BOTH’.
-- Uraian sumber 389: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’SetupModalAwal’ untuk action_id=’SetupModalAwal’, mode=’BOTH’.
-- Uraian sumber 390: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’SetupModalAwal’, mode=’BOTH’.
-- Uraian sumber 391: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’SetupModalAwal’, mode=’BOTH’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 392: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan false untuk action_id=’SetupModalAwal’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 393: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’SetupModalAwal’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 394: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’SetupModalAwal’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 395: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’SetupModalAwal’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 396: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’SetupModalAwal’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 397: Menutup rekaman seed action_id=’SetupModalAwal’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 398: Membuka rekaman seed action_id=’SetupBahanAwal’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 399: Mengisi action_id (kode aksi pada katalog) dengan ’SetupBahanAwal’ untuk action_id=’SetupBahanAwal’, mode=’BOTH’.
-- Uraian sumber 400: Mengisi action_name (nama aksi untuk tampilan) dengan ’Setup Bahan Awal’ untuk action_id=’SetupBahanAwal’, mode=’BOTH’.
-- Uraian sumber 401: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’SetupBahanAwal’ untuk action_id=’SetupBahanAwal’, mode=’BOTH’.
-- Uraian sumber 402: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’SetupBahanAwal’, mode=’BOTH’.
-- Uraian sumber 403: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan ’OUT’ untuk action_id=’SetupBahanAwal’, mode=’BOTH’.
-- Uraian sumber 404: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’SetupBahanAwal’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 405: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’SetupBahanAwal’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 406: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’SetupBahanAwal’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 407: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan true untuk action_id=’SetupBahanAwal’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 408: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’SetupBahanAwal’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 409: Menutup rekaman seed action_id=’SetupBahanAwal’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 410: Membuka rekaman seed action_id=’SetupEmasAwal’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 411: Mengisi action_id (kode aksi pada katalog) dengan ’SetupEmasAwal’ untuk action_id=’SetupEmasAwal’, mode=’BOTH’.
-- Uraian sumber 412: Mengisi action_name (nama aksi untuk tampilan) dengan ’Setup Emas Awal’ untuk action_id=’SetupEmasAwal’, mode=’BOTH’.
-- Uraian sumber 413: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’SetupEmasAwal’ untuk action_id=’SetupEmasAwal’, mode=’BOTH’.
-- Uraian sumber 414: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’SetupEmasAwal’, mode=’BOTH’.
-- Uraian sumber 415: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’SetupEmasAwal’, mode=’BOTH’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 416: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan false untuk action_id=’SetupEmasAwal’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 417: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’SetupEmasAwal’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 418: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’SetupEmasAwal’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 419: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’SetupEmasAwal’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 420: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’SetupEmasAwal’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 421: Menutup rekaman seed action_id=’SetupEmasAwal’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 422: Membuka rekaman seed action_id=’SetupMisiAwal’, mode=’BOTH’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 423: Mengisi action_id (kode aksi pada katalog) dengan ’SetupMisiAwal’ untuk action_id=’SetupMisiAwal’, mode=’BOTH’.
-- Uraian sumber 424: Mengisi action_name (nama aksi untuk tampilan) dengan ’Setup Misi Awal’ untuk action_id=’SetupMisiAwal’, mode=’BOTH’.
-- Uraian sumber 425: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’SetupMisiAwal’ untuk action_id=’SetupMisiAwal’, mode=’BOTH’.
-- Uraian sumber 426: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’BOTH’ untuk action_id=’SetupMisiAwal’, mode=’BOTH’.
-- Uraian sumber 427: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’SetupMisiAwal’, mode=’BOTH’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 428: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan false untuk action_id=’SetupMisiAwal’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 429: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’SetupMisiAwal’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 430: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’SetupMisiAwal’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 431: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’SetupMisiAwal’, mode=’BOTH’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 432: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’SetupMisiAwal’, mode=’BOTH’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 433: Menutup rekaman seed action_id=’SetupMisiAwal’, mode=’BOTH’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 434: Membuka rekaman seed action_id=’SetupPinjamanAwal’, mode=’MAHIR’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 435: Mengisi action_id (kode aksi pada katalog) dengan ’SetupPinjamanAwal’ untuk action_id=’SetupPinjamanAwal’, mode=’MAHIR’.
-- Uraian sumber 436: Mengisi action_name (nama aksi untuk tampilan) dengan ’Setup Pinjaman Awal’ untuk action_id=’SetupPinjamanAwal’, mode=’MAHIR’.
-- Uraian sumber 437: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’SetupPinjamanAwal’ untuk action_id=’SetupPinjamanAwal’, mode=’MAHIR’.
-- Uraian sumber 438: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’MAHIR’ untuk action_id=’SetupPinjamanAwal’, mode=’MAHIR’.
-- Uraian sumber 439: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan ’IN’ untuk action_id=’SetupPinjamanAwal’, mode=’MAHIR’.
-- Uraian sumber 440: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan true untuk action_id=’SetupPinjamanAwal’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 441: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’SetupPinjamanAwal’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 442: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’SetupPinjamanAwal’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 443: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’SetupPinjamanAwal’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 444: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’SetupPinjamanAwal’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 445: Menutup rekaman seed action_id=’SetupPinjamanAwal’, mode=’MAHIR’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 446: Membuka rekaman seed action_id=’SetupAsuransiAwal’, mode=’MAHIR’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 447: Mengisi action_id (kode aksi pada katalog) dengan ’SetupAsuransiAwal’ untuk action_id=’SetupAsuransiAwal’, mode=’MAHIR’.
-- Uraian sumber 448: Mengisi action_name (nama aksi untuk tampilan) dengan ’Setup Asuransi Awal’ untuk action_id=’SetupAsuransiAwal’, mode=’MAHIR’.
-- Uraian sumber 449: Mengisi behavior_id (kode perilaku domain yang menjalankan efek aksi) dengan ’SetupAsuransiAwal’ untuk action_id=’SetupAsuransiAwal’, mode=’MAHIR’.
-- Uraian sumber 450: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’MAHIR’ untuk action_id=’SetupAsuransiAwal’, mode=’MAHIR’.
-- Uraian sumber 451: Mengisi cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) dengan null untuk action_id=’SetupAsuransiAwal’, mode=’MAHIR’. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 452: Mengisi affects_coin (penanda aksi dapat memengaruhi koin) dengan false untuk action_id=’SetupAsuransiAwal’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 453: Mengisi affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) dengan false untuk action_id=’SetupAsuransiAwal’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 454: Mengisi affects_saving (penanda aksi dapat memengaruhi tabungan) dengan false untuk action_id=’SetupAsuransiAwal’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 455: Mengisi affects_inventory (penanda aksi dapat memengaruhi inventaris) dengan false untuk action_id=’SetupAsuransiAwal’, mode=’MAHIR’. Nilai boolean FALSE menonaktifkan penanda tersebut.
-- Uraian sumber 456: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk action_id=’SetupAsuransiAwal’, mode=’MAHIR’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 457: Menutup rekaman seed action_id=’SetupAsuransiAwal’, mode=’MAHIR’; kueri melanjutkan pemrosesan kumpulan nilai yang telah lengkap.
-- Uraian sumber 458: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 459: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 460: Menetapkan atau membandingkan action_name (nama aksi untuk tampilan) terhadap excluded.action_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 461: Menetapkan atau membandingkan behavior_id (kode perilaku domain yang menjalankan efek aksi) terhadap excluded.behavior_id, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 462: Menetapkan atau membandingkan mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) terhadap excluded.mode, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 463: Menetapkan atau membandingkan cashflow_direction (arah arus kas IN untuk pemasukan atau OUT untuk pengeluaran) terhadap excluded.cashflow_direction, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 464: Menetapkan atau membandingkan affects_coin (penanda aksi dapat memengaruhi koin) terhadap excluded.affects_coin, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 465: Menetapkan atau membandingkan affects_happiness (penanda aksi dapat memengaruhi kebahagiaan) terhadap excluded.affects_happiness, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 466: Menetapkan atau membandingkan affects_saving (penanda aksi dapat memengaruhi tabungan) terhadap excluded.affects_saving, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 467: Menetapkan atau membandingkan affects_inventory (penanda aksi dapat memengaruhi inventaris) terhadap excluded.affects_inventory, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 468: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 471: Membentuk tabel seed_rulesets untuk menampung definisi JSON bawaan Pemula dan Mahir sebelum dipecah ke tabel relasional. TEMPORARY membatasi tabel pembantu pada sesi koneksi dan ON COMMIT DROP, bila dicantumkan, menghapusnya ketika transaksi disahkan.
-- Uraian sumber 472: Mendefinisikan seed_rulesets.ruleset_id sebagai uuid untuk UUID paket aturan induk. NOT NULL mewajibkan nilai tersedia pada setiap rekaman.
-- Uraian sumber 473: Mendefinisikan seed_rulesets.ruleset_version_id sebagai uuid untuk UUID revisi aturan yang menjadi lingkup data. NOT NULL mewajibkan nilai tersedia pada setiap rekaman.
-- Uraian sumber 474: Mendefinisikan seed_rulesets.mode sebagai varchar(10) untuk mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya. NOT NULL mewajibkan nilai tersedia pada setiap rekaman.
-- Uraian sumber 475: Mendefinisikan seed_rulesets.ruleset_name sebagai varchar(160) untuk nama paket aturan bawaan. NOT NULL mewajibkan nilai tersedia pada setiap rekaman.
-- Uraian sumber 476: Mendefinisikan seed_rulesets.ruleset_description sebagai text untuk deskripsi paket aturan bawaan. NOT NULL mewajibkan nilai tersedia pada setiap rekaman.
-- Uraian sumber 477: Mendefinisikan seed_rulesets.definition_json sebagai jsonb untuk dokumen sumber konfigurasi yang dipecah menjadi komponen relasional. NOT NULL mewajibkan nilai tersedia pada setiap rekaman.
-- Uraian sumber 478: Mengevaluasi ekspresi ) on commit drop; pada tabel seed_rulesets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 480: Memulai penyisipan rekaman ke seed_rulesets, tabel yang menampung definisi JSON bawaan Pemula dan Mahir sebelum dipecah ke tabel relasional; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 481: Mengevaluasi ekspresi seed_rulesets ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 482: Menempatkan seed_rulesets.ruleset_id (UUID paket aturan induk) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 483: Menempatkan seed_rulesets.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 484: Menempatkan seed_rulesets.mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 485: Menempatkan seed_rulesets.ruleset_name (nama paket aturan bawaan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 486: Menempatkan seed_rulesets.ruleset_description (deskripsi paket aturan bawaan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 487: Menempatkan seed_rulesets.definition_json (dokumen sumber konfigurasi yang dipecah menjadi komponen relasional) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 488: Menutup kelompok yang terkait ; insert into seed_rulesets pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 489: Menyediakan 2 tuple nilai eksplisit untuk seed_rulesets; setiap tuple membentuk satu rekaman dan mengikuti urutan ruleset_id, ruleset_version_id, mode, ruleset_name, ruleset_description, definition_json.
-- Uraian sumber 490: Membuka rekaman seed mode=’PEMULA’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 491: Mengisi ruleset_id (UUID paket aturan induk) dengan ’2f4d94db-2a9f-4d4d-9a8a-53b58c598f71’ untuk mode=’PEMULA’.
-- Uraian sumber 492: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ untuk mode=’PEMULA’.
-- Uraian sumber 493: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’PEMULA’ untuk mode=’PEMULA’.
-- Uraian sumber 494: Mengisi ruleset_name (nama paket aturan bawaan) dengan ’Cashflowpoly Default - Mode Pemula’ untuk mode=’PEMULA’.
-- Uraian sumber 495: Mengisi ruleset_description (deskripsi paket aturan bawaan) dengan ’Seed ruleset mode pemula dalam definition_json terpadu dan katalog generik.’ untuk mode=’PEMULA’.
-- Uraian sumber 496: Penjelasan literal JSON konfigurasi: indeks +0 adalah baris pembuka literal langsung di bawah rangkaian komentar ini. Penjelasan diletakkan di luar literal agar isi data dan checksum tetap identik.
-- Uraian sumber 496: Literal +0: mode=”PEMULA” (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya)
-- Uraian sumber 496: Literal +1: actions_per_turn=2 (jumlah slot aksi per giliran)
-- Uraian sumber 496: Literal +2: starting_cash=20 (modal koin awal peserta)
-- Uraian sumber 496: Literal +3: player_ordering=”PLAYER_ORDER” (nilai player ordering)
-- Uraian sumber 496: Literal +4: feature=”DONATION” (fitur yang berlaku pada hari tersebut)
-- Uraian sumber 496: Literal +5: enabled=true (penanda aktivasi fitur)
-- Uraian sumber 496: Literal +6: feature=”GOLD_TRADE” (fitur yang berlaku pada hari tersebut)
-- Uraian sumber 496: Literal +7: enabled=true (penanda aktivasi fitur)
-- Uraian sumber 496: Literal +8: feature=”REST” (fitur yang berlaku pada hari tersebut)
-- Uraian sumber 496: Literal +9: enabled=true (penanda aktivasi fitur)
-- Uraian sumber 496: Literal +10: cash_min=0 (batas saldo koin terendah)
-- Uraian sumber 496: Literal +11: max_ingredient_total=6 (batas total bahan yang boleh disimpan)
-- Uraian sumber 496: Literal +12: max_same_ingredient=3 (batas kepemilikan bahan sejenis)
-- Uraian sumber 496: Literal +13: primary_need_max_per_day=null (batas pembelian kebutuhan primer per hari bila ditetapkan)
-- Uraian sumber 496: Literal +14: require_primary_before_others=true (kewajiban membeli kebutuhan primer sebelum tingkat lain)
-- Uraian sumber 496: Literal +15: min_amount=1 (nilai min amount)
-- Uraian sumber 496: Literal +16: max_amount=999999 (nilai max amount)
-- Uraian sumber 496: Literal +17: allow_buy=true (izin membeli emas)
-- Uraian sumber 496: Literal +18: allow_sell=true (izin menjual emas)
-- Uraian sumber 496: Literal +19: enabled=false (penanda aktivasi fitur)
-- Uraian sumber 496: Literal +20: enabled=false (penanda aktivasi fitur)
-- Uraian sumber 496: Literal +21: enabled=false (penanda aktivasi fitur)
-- Uraian sumber 496: Literal +22: income=1 (pemasukan kerja lepas)
-- Uraian sumber 496: Literal +23: rank=1 (posisi peringkat); points=7 (nilai poin); rank=2 (posisi peringkat); points=5 (nilai poin); rank=3 (posisi peringkat); points=2 (nilai poin)
-- Uraian sumber 496: Literal +24: qty=1 (kuantitas aset); points=3 (nilai poin); qty=2 (kuantitas aset); points=5 (nilai poin); qty=3 (kuantitas aset); points=8 (nilai poin); qty=4 (kuantitas aset); points=12 (nilai poin)
-- Uraian sumber 496: Literal +25: rank=1 (posisi peringkat); points=5 (nilai poin); rank=2 (posisi peringkat); points=3 (nilai poin); rank=3 (posisi peringkat); points=1 (nilai poin)
-- Uraian sumber 496: Literal +26: score_source=”DONATION” (kategori sumber skor); rank=1 (posisi peringkat); points=7 (nilai poin); score_source=”DONATION” (kategori sumber skor); rank=2 (posisi peringkat); points=5 (nilai poin); score_source=”DONATION” (kategori sumber skor); rank=3 (posisi peringkat); points=2 (nilai poin); score_source=”PENSION” (kategori sumber skor); rank=1 (posisi peringkat); points=5 (nilai poin); score_source=”PENSION” (kategori sumber skor); rank=2 (posisi peringkat); entri sejenis berikutnya melengkapi katalog pada baris yang sama
-- Uraian sumber 496: Literal +27: initialCoins=20 (modal koin awal)
-- Uraian sumber 496: Literal +28: initialHappiness=0 (poin kebahagiaan awal)
-- Uraian sumber 496: Literal +29: initialSaving=0 (tabungan awal)
-- Uraian sumber 496: Literal +30: actionsPerTurn=2 (batas aksi per giliran)
-- Uraian sumber 496: Literal +31: finishDay=25 (hari selesai permainan)
-- Uraian sumber 496: Literal +32: minPlayers=2 (minimum jumlah pemain)
-- Uraian sumber 496: Literal +33: maxPlayers=4 (maksimum jumlah pemain)
-- Uraian sumber 496: Literal +34: id=”nasi_putih” (kode identitas komponen); nama=”Nasi Putih” (nama tampilan komponen); hargaBeli=1 (harga beli kartu dalam koin); cardQty=5 (jumlah salinan fisik kartu); id=”sayur” (kode identitas komponen); nama=”Sayur” (nama tampilan komponen); hargaBeli=2 (harga beli kartu dalam koin); cardQty=5 (jumlah salinan fisik kartu); id=”tahu_tempe” (kode identitas komponen); nama=”Tahu Tempe” (nama tampilan komponen); hargaBeli=3 (harga beli kartu dalam koin); cardQty=5 (jumlah salinan fisik kartu); id=”telur” (kode identitas komponen); nama=”Telur” (nama tampilan komponen); entri sejenis berikutnya melengkapi katalog pada baris yang sama
-- Uraian sumber 496: Literal +35: resep: katalog pesanan beserta bahan wajib. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 496: Literal +36: id=”lontong_balap” (kode identitas komponen); nama=”lontong balap” (nama tampilan komponen); hargaJual=13 (harga jual pesanan dalam koin); poinKebahagiaan=0 (poin kebahagiaan kartu); cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +37: id=”nasi_goreng” (kode identitas komponen)
-- Uraian sumber 496: Literal +38: nama=”nasi goreng” (nama tampilan komponen)
-- Uraian sumber 496: Literal +39: hargaJual=15 (harga jual pesanan dalam koin)
-- Uraian sumber 496: Literal +40: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 496: Literal +41: cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +42: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 496: Literal +43: id=”tahu_campur” (kode identitas komponen)
-- Uraian sumber 496: Literal +44: nama=”tahu campur” (nama tampilan komponen)
-- Uraian sumber 496: Literal +45: hargaJual=16 (harga jual pesanan dalam koin)
-- Uraian sumber 496: Literal +46: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 496: Literal +47: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +48: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 496: Literal +49: id=”rawon” (kode identitas komponen)
-- Uraian sumber 496: Literal +50: nama=”rawon” (nama tampilan komponen)
-- Uraian sumber 496: Literal +51: hargaJual=24 (harga jual pesanan dalam koin)
-- Uraian sumber 496: Literal +52: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 496: Literal +53: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +54: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 496: Literal +55: id=”semanggi_surabaya” (kode identitas komponen)
-- Uraian sumber 496: Literal +56: nama=”semanggi surabaya” (nama tampilan komponen)
-- Uraian sumber 496: Literal +57: hargaJual=14 (harga jual pesanan dalam koin)
-- Uraian sumber 496: Literal +58: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 496: Literal +59: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +60: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 496: Literal +61: id=”soto_daging” (kode identitas komponen)
-- Uraian sumber 496: Literal +62: nama=”soto daging” (nama tampilan komponen)
-- Uraian sumber 496: Literal +63: hargaJual=17 (harga jual pesanan dalam koin)
-- Uraian sumber 496: Literal +64: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 496: Literal +65: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +66: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 496: Literal +67: id=”nasi_pecel” (kode identitas komponen)
-- Uraian sumber 496: Literal +68: nama=”nasi pecel” (nama tampilan komponen)
-- Uraian sumber 496: Literal +69: hargaJual=20 (harga jual pesanan dalam koin)
-- Uraian sumber 496: Literal +70: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 496: Literal +71: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +72: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 496: Literal +73: id=”sego_penyet” (kode identitas komponen)
-- Uraian sumber 496: Literal +74: nama=”sego penyet” (nama tampilan komponen)
-- Uraian sumber 496: Literal +75: hargaJual=22 (harga jual pesanan dalam koin)
-- Uraian sumber 496: Literal +76: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 496: Literal +77: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +78: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 496: Literal +79: id=”tahu_telur” (kode identitas komponen)
-- Uraian sumber 496: Literal +80: nama=”tahu telur” (nama tampilan komponen)
-- Uraian sumber 496: Literal +81: hargaJual=25 (harga jual pesanan dalam koin)
-- Uraian sumber 496: Literal +82: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 496: Literal +83: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +84: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 496: Literal +85: id=”sate_klopo” (kode identitas komponen)
-- Uraian sumber 496: Literal +86: nama=”sate klopo” (nama tampilan komponen)
-- Uraian sumber 496: Literal +87: hargaJual=26 (harga jual pesanan dalam koin)
-- Uraian sumber 496: Literal +88: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 496: Literal +89: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +90: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 496: Literal +91: id=”rujak_cingur” (kode identitas komponen)
-- Uraian sumber 496: Literal +92: nama=”rujak cingur” (nama tampilan komponen)
-- Uraian sumber 496: Literal +93: hargaJual=28 (harga jual pesanan dalam koin)
-- Uraian sumber 496: Literal +94: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 496: Literal +95: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +96: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 496: Literal +97: id=”gado_gado” (kode identitas komponen)
-- Uraian sumber 496: Literal +98: nama=”gado gado” (nama tampilan komponen)
-- Uraian sumber 496: Literal +99: hargaJual=26 (harga jual pesanan dalam koin)
-- Uraian sumber 496: Literal +100: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 496: Literal +101: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +102: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 496: Literal +103: id=”nasi_campur” (kode identitas komponen)
-- Uraian sumber 496: Literal +104: nama=”nasi campur” (nama tampilan komponen)
-- Uraian sumber 496: Literal +105: hargaJual=27 (harga jual pesanan dalam koin)
-- Uraian sumber 496: Literal +106: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 496: Literal +107: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +108: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 496: Literal +109: kebutuhan: katalog kartu kebutuhan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 496: Literal +110: id=”buku_1” (kode identitas komponen); nama=”Buku” (nama tampilan komponen); tipe=”primer” (nilai tipe); hargaBeli=2 (harga beli kartu dalam koin); poinKebahagiaan=1 (poin kebahagiaan kartu); cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +111: id=”buku_2” (kode identitas komponen); nama=”Buku” (nama tampilan komponen); tipe=”primer” (nilai tipe); hargaBeli=3 (harga beli kartu dalam koin); poinKebahagiaan=2 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +112: id=”baju_1” (kode identitas komponen); nama=”Baju” (nama tampilan komponen); tipe=”primer” (nilai tipe); hargaBeli=2 (harga beli kartu dalam koin); poinKebahagiaan=1 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +113: id=”baju_2” (kode identitas komponen); nama=”Baju” (nama tampilan komponen); tipe=”primer” (nilai tipe); hargaBeli=3 (harga beli kartu dalam koin); poinKebahagiaan=2 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +114: id=”tempat_makan_1” (kode identitas komponen); nama=”Tempat Makan” (nama tampilan komponen); tipe=”primer” (nilai tipe); hargaBeli=2 (harga beli kartu dalam koin); poinKebahagiaan=1 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +115: id=”tempat_makan_2” (kode identitas komponen); nama=”Tempat Makan” (nama tampilan komponen); tipe=”primer” (nilai tipe); hargaBeli=3 (harga beli kartu dalam koin); poinKebahagiaan=2 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +116: id=”sepatu_1” (kode identitas komponen); nama=”Sepatu” (nama tampilan komponen); tipe=”primer” (nilai tipe); hargaBeli=2 (harga beli kartu dalam koin); poinKebahagiaan=1 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +117: id=”sepatu_2” (kode identitas komponen); nama=”Sepatu” (nama tampilan komponen); tipe=”primer” (nilai tipe); hargaBeli=3 (harga beli kartu dalam koin); poinKebahagiaan=2 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +118: id=”tas_1” (kode identitas komponen); nama=”Tas” (nama tampilan komponen); tipe=”sekunder” (nilai tipe); hargaBeli=4 (harga beli kartu dalam koin); poinKebahagiaan=3 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +119: id=”tas_2” (kode identitas komponen); nama=”Tas” (nama tampilan komponen); tipe=”sekunder” (nilai tipe); hargaBeli=5 (harga beli kartu dalam koin); poinKebahagiaan=4 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +120: id=”sepeda_1” (kode identitas komponen); nama=”Sepeda” (nama tampilan komponen); tipe=”sekunder” (nilai tipe); hargaBeli=4 (harga beli kartu dalam koin); poinKebahagiaan=3 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +121: id=”sepeda_2” (kode identitas komponen); nama=”Sepeda” (nama tampilan komponen); tipe=”sekunder” (nilai tipe); hargaBeli=5 (harga beli kartu dalam koin); poinKebahagiaan=4 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +122: id=”gadget_1” (kode identitas komponen); nama=”Gadget” (nama tampilan komponen); tipe=”sekunder” (nilai tipe); hargaBeli=4 (harga beli kartu dalam koin); poinKebahagiaan=3 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +123: id=”gadget_2” (kode identitas komponen); nama=”Gadget” (nama tampilan komponen); tipe=”sekunder” (nilai tipe); hargaBeli=5 (harga beli kartu dalam koin); poinKebahagiaan=4 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +124: id=”tempat_pensil_1” (kode identitas komponen); nama=”Tempat Pensil” (nama tampilan komponen); tipe=”sekunder” (nilai tipe); hargaBeli=4 (harga beli kartu dalam koin); poinKebahagiaan=3 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +125: id=”tempat_pensil_2” (kode identitas komponen); nama=”Tempat Pensil” (nama tampilan komponen); tipe=”sekunder” (nilai tipe); hargaBeli=5 (harga beli kartu dalam koin); poinKebahagiaan=4 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +126: id=”boneka_1” (kode identitas komponen); nama=”Boneka” (nama tampilan komponen); tipe=”tersier” (nilai tipe); hargaBeli=6 (harga beli kartu dalam koin); poinKebahagiaan=5 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +127: id=”boneka_2” (kode identitas komponen); nama=”Boneka” (nama tampilan komponen); tipe=”tersier” (nilai tipe); hargaBeli=7 (harga beli kartu dalam koin); poinKebahagiaan=6 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +128: id=”gameboy_1” (kode identitas komponen); nama=”Gameboy” (nama tampilan komponen); tipe=”tersier” (nilai tipe); hargaBeli=6 (harga beli kartu dalam koin); poinKebahagiaan=5 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +129: id=”gameboy_2” (kode identitas komponen); nama=”Gameboy” (nama tampilan komponen); tipe=”tersier” (nilai tipe); hargaBeli=7 (harga beli kartu dalam koin); poinKebahagiaan=6 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +130: id=”jam_1” (kode identitas komponen); nama=”Jam” (nama tampilan komponen); tipe=”tersier” (nilai tipe); hargaBeli=6 (harga beli kartu dalam koin); poinKebahagiaan=5 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +131: id=”jam_2” (kode identitas komponen); nama=”Jam” (nama tampilan komponen); tipe=”tersier” (nilai tipe); hargaBeli=7 (harga beli kartu dalam koin); poinKebahagiaan=6 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +132: id=”hiburan_1” (kode identitas komponen); nama=”Hiburan” (nama tampilan komponen); tipe=”tersier” (nilai tipe); hargaBeli=6 (harga beli kartu dalam koin); poinKebahagiaan=5 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +133: id=”hiburan_2” (kode identitas komponen); nama=”Hiburan” (nama tampilan komponen); tipe=”tersier” (nilai tipe); hargaBeli=7 (harga beli kartu dalam koin); poinKebahagiaan=6 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 496: Literal +134: Menutup susunan objek atau daftar JSON yang sedang dibentuk; pemisah koma mempertahankan batas antaranggota dokumen konfigurasi
-- Uraian sumber 496: Literal +135: targetKebutuhan: misi koleksi dan syarat kartu kebutuhan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 496: Literal +136: id=”misi_jam” (kode identitas komponen); nama=”jam” (nama tampilan komponen); success_points=0 (poin ketika syarat berhasil dipenuhi); failure_points=-10 (poin ketika syarat gagal dipenuhi); penaltyPoints=10 (nilai penalti poin); order=1 (urutan syarat); type=”TIER” (jenis syarat atau kategori); value=”primer” (nilai syarat atau data konfigurasi); order=2 (urutan syarat); type=”TIER” (jenis syarat atau kategori); value=”sekunder” (nilai syarat atau data konfigurasi); order=3 (urutan syarat); type=”FAMILY” (jenis syarat atau kategori); value=”jam” (nilai syarat atau data konfigurasi)
-- Uraian sumber 496: Literal +137: id=”misi_boneka” (kode identitas komponen)
-- Uraian sumber 496: Literal +138: nama=”boneka” (nama tampilan komponen)
-- Uraian sumber 496: Literal +139: success_points=0 (poin ketika syarat berhasil dipenuhi)
-- Uraian sumber 496: Literal +140: failure_points=-10 (poin ketika syarat gagal dipenuhi)
-- Uraian sumber 496: Literal +141: penaltyPoints=10 (nilai penalti poin)
-- Uraian sumber 496: Literal +142: order=1 (urutan syarat); type=”TIER” (jenis syarat atau kategori); value=”primer” (nilai syarat atau data konfigurasi); order=2 (urutan syarat); type=”TIER” (jenis syarat atau kategori); value=”sekunder” (nilai syarat atau data konfigurasi); order=3 (urutan syarat); type=”FAMILY” (jenis syarat atau kategori); value=”boneka” (nilai syarat atau data konfigurasi)
-- Uraian sumber 496: Literal +143: id=”misi_gameboy” (kode identitas komponen)
-- Uraian sumber 496: Literal +144: nama=”gameboy” (nama tampilan komponen)
-- Uraian sumber 496: Literal +145: success_points=0 (poin ketika syarat berhasil dipenuhi)
-- Uraian sumber 496: Literal +146: failure_points=-10 (poin ketika syarat gagal dipenuhi)
-- Uraian sumber 496: Literal +147: penaltyPoints=10 (nilai penalti poin)
-- Uraian sumber 496: Literal +148: order=1 (urutan syarat); type=”TIER” (jenis syarat atau kategori); value=”primer” (nilai syarat atau data konfigurasi); order=2 (urutan syarat); type=”TIER” (jenis syarat atau kategori); value=”sekunder” (nilai syarat atau data konfigurasi); order=3 (urutan syarat); type=”FAMILY” (jenis syarat atau kategori); value=”gameboy” (nilai syarat atau data konfigurasi)
-- Uraian sumber 496: Literal +149: id=”misi_hiburan” (kode identitas komponen)
-- Uraian sumber 496: Literal +150: nama=”hiburan” (nama tampilan komponen)
-- Uraian sumber 496: Literal +151: success_points=0 (poin ketika syarat berhasil dipenuhi)
-- Uraian sumber 496: Literal +152: failure_points=-10 (poin ketika syarat gagal dipenuhi)
-- Uraian sumber 496: Literal +153: penaltyPoints=10 (nilai penalti poin)
-- Uraian sumber 496: Literal +154: order=1 (urutan syarat); type=”TIER” (jenis syarat atau kategori); value=”primer” (nilai syarat atau data konfigurasi); order=2 (urutan syarat); type=”TIER” (jenis syarat atau kategori); value=”sekunder” (nilai syarat atau data konfigurasi); order=3 (urutan syarat); type=”FAMILY” (jenis syarat atau kategori); value=”hiburan” (nilai syarat atau data konfigurasi)
-- Uraian sumber 496: Literal +155: id=”tujuan_25” (kode identitas komponen); nama=”Kumpul Keluarga” (nama tampilan komponen); hargaBeli=25 (harga beli kartu dalam koin); poinKebahagiaan=20 (poin kebahagiaan kartu); id=”tujuan_28” (kode identitas komponen); nama=”Tamasya” (nama tampilan komponen); hargaBeli=28 (harga beli kartu dalam koin); poinKebahagiaan=25 (poin kebahagiaan kartu); id=”tujuan_30” (kode identitas komponen); nama=”Keluar Kota” (nama tampilan komponen); hargaBeli=30 (harga beli kartu dalam koin); poinKebahagiaan=28 (poin kebahagiaan kartu); id=”tujuan_32” (kode identitas komponen); nama=”Beli Mobil Baru” (nama tampilan komponen); entri sejenis berikutnya melengkapi katalog pada baris yang sama
-- Uraian sumber 496: Literal +156: id=”jual_pertama” (kode identitas komponen); nama=”jual_pertama” (nama tampilan komponen)
-- Uraian sumber 496: Literal +157: aksi=”JualMasakan” (nilai aksi); value=1 (nilai syarat atau data konfigurasi)
-- Uraian sumber 654: Menutup rekaman seed mode=’PEMULA’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 655: Membuka rekaman seed mode=’MAHIR’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 656: Mengisi ruleset_id (UUID paket aturan induk) dengan ’a68f53f9-92a2-446f-9f62-5a4f502a0199’ untuk mode=’MAHIR’.
-- Uraian sumber 657: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ untuk mode=’MAHIR’.
-- Uraian sumber 658: Mengisi mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) dengan ’MAHIR’ untuk mode=’MAHIR’.
-- Uraian sumber 659: Mengisi ruleset_name (nama paket aturan bawaan) dengan ’Cashflowpoly Default - Mode Mahir’ untuk mode=’MAHIR’.
-- Uraian sumber 660: Mengisi ruleset_description (deskripsi paket aturan bawaan) dengan ’Seed ruleset mode mahir dalam definition_json terpadu dan katalog generik.’ untuk mode=’MAHIR’.
-- Uraian sumber 661: Penjelasan literal JSON konfigurasi: indeks +0 adalah baris pembuka literal langsung di bawah rangkaian komentar ini. Penjelasan diletakkan di luar literal agar isi data dan checksum tetap identik.
-- Uraian sumber 661: Literal +0: mode=”MAHIR” (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya)
-- Uraian sumber 661: Literal +1: actions_per_turn=2 (jumlah slot aksi per giliran)
-- Uraian sumber 661: Literal +2: starting_cash=10 (modal koin awal peserta)
-- Uraian sumber 661: Literal +3: player_ordering=”PLAYER_ORDER” (nilai player ordering)
-- Uraian sumber 661: Literal +4: feature=”DONATION” (fitur yang berlaku pada hari tersebut)
-- Uraian sumber 661: Literal +5: enabled=true (penanda aktivasi fitur)
-- Uraian sumber 661: Literal +6: feature=”GOLD_TRADE” (fitur yang berlaku pada hari tersebut)
-- Uraian sumber 661: Literal +7: enabled=true (penanda aktivasi fitur)
-- Uraian sumber 661: Literal +8: feature=”REST” (fitur yang berlaku pada hari tersebut)
-- Uraian sumber 661: Literal +9: enabled=true (penanda aktivasi fitur)
-- Uraian sumber 661: Literal +10: cash_min=0 (batas saldo koin terendah)
-- Uraian sumber 661: Literal +11: max_ingredient_total=6 (batas total bahan yang boleh disimpan)
-- Uraian sumber 661: Literal +12: max_same_ingredient=3 (batas kepemilikan bahan sejenis)
-- Uraian sumber 661: Literal +13: primary_need_max_per_day=null (batas pembelian kebutuhan primer per hari bila ditetapkan)
-- Uraian sumber 661: Literal +14: require_primary_before_others=true (kewajiban membeli kebutuhan primer sebelum tingkat lain)
-- Uraian sumber 661: Literal +15: min_amount=1 (nilai min amount)
-- Uraian sumber 661: Literal +16: max_amount=999999 (nilai max amount)
-- Uraian sumber 661: Literal +17: allow_buy=true (izin membeli emas)
-- Uraian sumber 661: Literal +18: allow_sell=true (izin menjual emas)
-- Uraian sumber 661: Literal +19: enabled=true (penanda aktivasi fitur)
-- Uraian sumber 661: Literal +20: enabled=true (penanda aktivasi fitur)
-- Uraian sumber 661: Literal +21: enabled=true (penanda aktivasi fitur)
-- Uraian sumber 661: Literal +22: income=1 (pemasukan kerja lepas)
-- Uraian sumber 661: Literal +23: rank=1 (posisi peringkat); points=7 (nilai poin); rank=2 (posisi peringkat); points=5 (nilai poin); rank=3 (posisi peringkat); points=2 (nilai poin)
-- Uraian sumber 661: Literal +24: qty=1 (kuantitas aset); points=3 (nilai poin); qty=2 (kuantitas aset); points=5 (nilai poin); qty=3 (kuantitas aset); points=8 (nilai poin); qty=4 (kuantitas aset); points=12 (nilai poin)
-- Uraian sumber 661: Literal +25: rank=1 (posisi peringkat); points=5 (nilai poin); rank=2 (posisi peringkat); points=3 (nilai poin); rank=3 (posisi peringkat); points=1 (nilai poin)
-- Uraian sumber 661: Literal +26: score_source=”DONATION” (kategori sumber skor); rank=1 (posisi peringkat); points=7 (nilai poin); score_source=”DONATION” (kategori sumber skor); rank=2 (posisi peringkat); points=5 (nilai poin); score_source=”DONATION” (kategori sumber skor); rank=3 (posisi peringkat); points=2 (nilai poin); score_source=”PENSION” (kategori sumber skor); rank=1 (posisi peringkat); points=5 (nilai poin); score_source=”PENSION” (kategori sumber skor); rank=2 (posisi peringkat); entri sejenis berikutnya melengkapi katalog pada baris yang sama
-- Uraian sumber 661: Literal +27: initialCoins=10 (modal koin awal)
-- Uraian sumber 661: Literal +28: initialHappiness=0 (poin kebahagiaan awal)
-- Uraian sumber 661: Literal +29: initialSaving=0 (tabungan awal)
-- Uraian sumber 661: Literal +30: actionsPerTurn=2 (batas aksi per giliran)
-- Uraian sumber 661: Literal +31: finishDay=25 (hari selesai permainan)
-- Uraian sumber 661: Literal +32: minPlayers=2 (minimum jumlah pemain)
-- Uraian sumber 661: Literal +33: maxPlayers=4 (maksimum jumlah pemain)
-- Uraian sumber 661: Literal +34: id=”nasi_putih” (kode identitas komponen); nama=”Nasi Putih” (nama tampilan komponen); hargaBeli=1 (harga beli kartu dalam koin); cardQty=5 (jumlah salinan fisik kartu); id=”sayur” (kode identitas komponen); nama=”Sayur” (nama tampilan komponen); hargaBeli=2 (harga beli kartu dalam koin); cardQty=5 (jumlah salinan fisik kartu); id=”tahu_tempe” (kode identitas komponen); nama=”Tahu Tempe” (nama tampilan komponen); hargaBeli=3 (harga beli kartu dalam koin); cardQty=5 (jumlah salinan fisik kartu); id=”telur” (kode identitas komponen); nama=”Telur” (nama tampilan komponen); entri sejenis berikutnya melengkapi katalog pada baris yang sama
-- Uraian sumber 661: Literal +35: resep: katalog pesanan beserta bahan wajib. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 661: Literal +36: id=”lontong_balap” (kode identitas komponen); nama=”lontong balap” (nama tampilan komponen); hargaJual=13 (harga jual pesanan dalam koin); poinKebahagiaan=0 (poin kebahagiaan kartu); cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +37: id=”nasi_goreng” (kode identitas komponen)
-- Uraian sumber 661: Literal +38: nama=”nasi goreng” (nama tampilan komponen)
-- Uraian sumber 661: Literal +39: hargaJual=15 (harga jual pesanan dalam koin)
-- Uraian sumber 661: Literal +40: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +41: cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +42: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 661: Literal +43: id=”tahu_campur” (kode identitas komponen)
-- Uraian sumber 661: Literal +44: nama=”tahu campur” (nama tampilan komponen)
-- Uraian sumber 661: Literal +45: hargaJual=16 (harga jual pesanan dalam koin)
-- Uraian sumber 661: Literal +46: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +47: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +48: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 661: Literal +49: id=”rawon” (kode identitas komponen)
-- Uraian sumber 661: Literal +50: nama=”rawon” (nama tampilan komponen)
-- Uraian sumber 661: Literal +51: hargaJual=24 (harga jual pesanan dalam koin)
-- Uraian sumber 661: Literal +52: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +53: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +54: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 661: Literal +55: id=”semanggi_surabaya” (kode identitas komponen)
-- Uraian sumber 661: Literal +56: nama=”semanggi surabaya” (nama tampilan komponen)
-- Uraian sumber 661: Literal +57: hargaJual=14 (harga jual pesanan dalam koin)
-- Uraian sumber 661: Literal +58: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +59: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +60: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 661: Literal +61: id=”soto_daging” (kode identitas komponen)
-- Uraian sumber 661: Literal +62: nama=”soto daging” (nama tampilan komponen)
-- Uraian sumber 661: Literal +63: hargaJual=17 (harga jual pesanan dalam koin)
-- Uraian sumber 661: Literal +64: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +65: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +66: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 661: Literal +67: id=”nasi_pecel” (kode identitas komponen)
-- Uraian sumber 661: Literal +68: nama=”nasi pecel” (nama tampilan komponen)
-- Uraian sumber 661: Literal +69: hargaJual=20 (harga jual pesanan dalam koin)
-- Uraian sumber 661: Literal +70: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +71: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +72: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 661: Literal +73: id=”sego_penyet” (kode identitas komponen)
-- Uraian sumber 661: Literal +74: nama=”sego penyet” (nama tampilan komponen)
-- Uraian sumber 661: Literal +75: hargaJual=22 (harga jual pesanan dalam koin)
-- Uraian sumber 661: Literal +76: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +77: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +78: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 661: Literal +79: id=”tahu_telur” (kode identitas komponen)
-- Uraian sumber 661: Literal +80: nama=”tahu telur” (nama tampilan komponen)
-- Uraian sumber 661: Literal +81: hargaJual=25 (harga jual pesanan dalam koin)
-- Uraian sumber 661: Literal +82: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +83: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +84: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 661: Literal +85: id=”sate_klopo” (kode identitas komponen)
-- Uraian sumber 661: Literal +86: nama=”sate klopo” (nama tampilan komponen)
-- Uraian sumber 661: Literal +87: hargaJual=26 (harga jual pesanan dalam koin)
-- Uraian sumber 661: Literal +88: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +89: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +90: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 661: Literal +91: id=”rujak_cingur” (kode identitas komponen)
-- Uraian sumber 661: Literal +92: nama=”rujak cingur” (nama tampilan komponen)
-- Uraian sumber 661: Literal +93: hargaJual=28 (harga jual pesanan dalam koin)
-- Uraian sumber 661: Literal +94: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +95: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +96: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 661: Literal +97: id=”gado_gado” (kode identitas komponen)
-- Uraian sumber 661: Literal +98: nama=”gado gado” (nama tampilan komponen)
-- Uraian sumber 661: Literal +99: hargaJual=26 (harga jual pesanan dalam koin)
-- Uraian sumber 661: Literal +100: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +101: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +102: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 661: Literal +103: id=”nasi_campur” (kode identitas komponen)
-- Uraian sumber 661: Literal +104: nama=”nasi campur” (nama tampilan komponen)
-- Uraian sumber 661: Literal +105: hargaJual=27 (harga jual pesanan dalam koin)
-- Uraian sumber 661: Literal +106: poinKebahagiaan=0 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +107: cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +108: bahan: daftar bahan resep atau katalog bahan masakan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 661: Literal +109: kebutuhan: katalog kartu kebutuhan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 661: Literal +110: id=”buku_1” (kode identitas komponen); nama=”Buku” (nama tampilan komponen); tipe=”primer” (nilai tipe); hargaBeli=2 (harga beli kartu dalam koin); poinKebahagiaan=1 (poin kebahagiaan kartu); cardQty=2 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +111: id=”buku_2” (kode identitas komponen); nama=”Buku” (nama tampilan komponen); tipe=”primer” (nilai tipe); hargaBeli=3 (harga beli kartu dalam koin); poinKebahagiaan=2 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +112: id=”baju_1” (kode identitas komponen); nama=”Baju” (nama tampilan komponen); tipe=”primer” (nilai tipe); hargaBeli=2 (harga beli kartu dalam koin); poinKebahagiaan=1 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +113: id=”baju_2” (kode identitas komponen); nama=”Baju” (nama tampilan komponen); tipe=”primer” (nilai tipe); hargaBeli=3 (harga beli kartu dalam koin); poinKebahagiaan=2 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +114: id=”tempat_makan_1” (kode identitas komponen); nama=”Tempat Makan” (nama tampilan komponen); tipe=”primer” (nilai tipe); hargaBeli=2 (harga beli kartu dalam koin); poinKebahagiaan=1 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +115: id=”tempat_makan_2” (kode identitas komponen); nama=”Tempat Makan” (nama tampilan komponen); tipe=”primer” (nilai tipe); hargaBeli=3 (harga beli kartu dalam koin); poinKebahagiaan=2 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +116: id=”sepatu_1” (kode identitas komponen); nama=”Sepatu” (nama tampilan komponen); tipe=”primer” (nilai tipe); hargaBeli=2 (harga beli kartu dalam koin); poinKebahagiaan=1 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +117: id=”sepatu_2” (kode identitas komponen); nama=”Sepatu” (nama tampilan komponen); tipe=”primer” (nilai tipe); hargaBeli=3 (harga beli kartu dalam koin); poinKebahagiaan=2 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +118: id=”tas_1” (kode identitas komponen); nama=”Tas” (nama tampilan komponen); tipe=”sekunder” (nilai tipe); hargaBeli=4 (harga beli kartu dalam koin); poinKebahagiaan=3 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +119: id=”tas_2” (kode identitas komponen); nama=”Tas” (nama tampilan komponen); tipe=”sekunder” (nilai tipe); hargaBeli=5 (harga beli kartu dalam koin); poinKebahagiaan=4 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +120: id=”sepeda_1” (kode identitas komponen); nama=”Sepeda” (nama tampilan komponen); tipe=”sekunder” (nilai tipe); hargaBeli=4 (harga beli kartu dalam koin); poinKebahagiaan=3 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +121: id=”sepeda_2” (kode identitas komponen); nama=”Sepeda” (nama tampilan komponen); tipe=”sekunder” (nilai tipe); hargaBeli=5 (harga beli kartu dalam koin); poinKebahagiaan=4 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +122: id=”gadget_1” (kode identitas komponen); nama=”Gadget” (nama tampilan komponen); tipe=”sekunder” (nilai tipe); hargaBeli=4 (harga beli kartu dalam koin); poinKebahagiaan=3 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +123: id=”gadget_2” (kode identitas komponen); nama=”Gadget” (nama tampilan komponen); tipe=”sekunder” (nilai tipe); hargaBeli=5 (harga beli kartu dalam koin); poinKebahagiaan=4 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +124: id=”tempat_pensil_1” (kode identitas komponen); nama=”Tempat Pensil” (nama tampilan komponen); tipe=”sekunder” (nilai tipe); hargaBeli=4 (harga beli kartu dalam koin); poinKebahagiaan=3 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +125: id=”tempat_pensil_2” (kode identitas komponen); nama=”Tempat Pensil” (nama tampilan komponen); tipe=”sekunder” (nilai tipe); hargaBeli=5 (harga beli kartu dalam koin); poinKebahagiaan=4 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +126: id=”boneka_1” (kode identitas komponen); nama=”Boneka” (nama tampilan komponen); tipe=”tersier” (nilai tipe); hargaBeli=6 (harga beli kartu dalam koin); poinKebahagiaan=5 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +127: id=”boneka_2” (kode identitas komponen); nama=”Boneka” (nama tampilan komponen); tipe=”tersier” (nilai tipe); hargaBeli=7 (harga beli kartu dalam koin); poinKebahagiaan=6 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +128: id=”gameboy_1” (kode identitas komponen); nama=”Gameboy” (nama tampilan komponen); tipe=”tersier” (nilai tipe); hargaBeli=6 (harga beli kartu dalam koin); poinKebahagiaan=5 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +129: id=”gameboy_2” (kode identitas komponen); nama=”Gameboy” (nama tampilan komponen); tipe=”tersier” (nilai tipe); hargaBeli=7 (harga beli kartu dalam koin); poinKebahagiaan=6 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +130: id=”jam_1” (kode identitas komponen); nama=”Jam” (nama tampilan komponen); tipe=”tersier” (nilai tipe); hargaBeli=6 (harga beli kartu dalam koin); poinKebahagiaan=5 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +131: id=”jam_2” (kode identitas komponen); nama=”Jam” (nama tampilan komponen); tipe=”tersier” (nilai tipe); hargaBeli=7 (harga beli kartu dalam koin); poinKebahagiaan=6 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +132: id=”hiburan_1” (kode identitas komponen); nama=”Hiburan” (nama tampilan komponen); tipe=”tersier” (nilai tipe); hargaBeli=6 (harga beli kartu dalam koin); poinKebahagiaan=5 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +133: id=”hiburan_2” (kode identitas komponen); nama=”Hiburan” (nama tampilan komponen); tipe=”tersier” (nilai tipe); hargaBeli=7 (harga beli kartu dalam koin); poinKebahagiaan=6 (poin kebahagiaan kartu); cardQty=1 (jumlah salinan fisik kartu)
-- Uraian sumber 661: Literal +134: Menutup susunan objek atau daftar JSON yang sedang dibentuk; pemisah koma mempertahankan batas antaranggota dokumen konfigurasi
-- Uraian sumber 661: Literal +135: targetKebutuhan: misi koleksi dan syarat kartu kebutuhan. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 661: Literal +136: id=”misi_jam” (kode identitas komponen); nama=”jam” (nama tampilan komponen); success_points=0 (poin ketika syarat berhasil dipenuhi); failure_points=-10 (poin ketika syarat gagal dipenuhi); penaltyPoints=10 (nilai penalti poin); order=1 (urutan syarat); type=”TIER” (jenis syarat atau kategori); value=”primer” (nilai syarat atau data konfigurasi); order=2 (urutan syarat); type=”TIER” (jenis syarat atau kategori); value=”sekunder” (nilai syarat atau data konfigurasi); order=3 (urutan syarat); type=”FAMILY” (jenis syarat atau kategori); value=”jam” (nilai syarat atau data konfigurasi)
-- Uraian sumber 661: Literal +137: id=”misi_boneka” (kode identitas komponen)
-- Uraian sumber 661: Literal +138: nama=”boneka” (nama tampilan komponen)
-- Uraian sumber 661: Literal +139: success_points=0 (poin ketika syarat berhasil dipenuhi)
-- Uraian sumber 661: Literal +140: failure_points=-10 (poin ketika syarat gagal dipenuhi)
-- Uraian sumber 661: Literal +141: penaltyPoints=10 (nilai penalti poin)
-- Uraian sumber 661: Literal +142: order=1 (urutan syarat); type=”TIER” (jenis syarat atau kategori); value=”primer” (nilai syarat atau data konfigurasi); order=2 (urutan syarat); type=”TIER” (jenis syarat atau kategori); value=”sekunder” (nilai syarat atau data konfigurasi); order=3 (urutan syarat); type=”FAMILY” (jenis syarat atau kategori); value=”boneka” (nilai syarat atau data konfigurasi)
-- Uraian sumber 661: Literal +143: id=”misi_gameboy” (kode identitas komponen)
-- Uraian sumber 661: Literal +144: nama=”gameboy” (nama tampilan komponen)
-- Uraian sumber 661: Literal +145: success_points=0 (poin ketika syarat berhasil dipenuhi)
-- Uraian sumber 661: Literal +146: failure_points=-10 (poin ketika syarat gagal dipenuhi)
-- Uraian sumber 661: Literal +147: penaltyPoints=10 (nilai penalti poin)
-- Uraian sumber 661: Literal +148: order=1 (urutan syarat); type=”TIER” (jenis syarat atau kategori); value=”primer” (nilai syarat atau data konfigurasi); order=2 (urutan syarat); type=”TIER” (jenis syarat atau kategori); value=”sekunder” (nilai syarat atau data konfigurasi); order=3 (urutan syarat); type=”FAMILY” (jenis syarat atau kategori); value=”gameboy” (nilai syarat atau data konfigurasi)
-- Uraian sumber 661: Literal +149: id=”misi_hiburan” (kode identitas komponen)
-- Uraian sumber 661: Literal +150: nama=”hiburan” (nama tampilan komponen)
-- Uraian sumber 661: Literal +151: success_points=0 (poin ketika syarat berhasil dipenuhi)
-- Uraian sumber 661: Literal +152: failure_points=-10 (poin ketika syarat gagal dipenuhi)
-- Uraian sumber 661: Literal +153: penaltyPoints=10 (nilai penalti poin)
-- Uraian sumber 661: Literal +154: order=1 (urutan syarat); type=”TIER” (jenis syarat atau kategori); value=”primer” (nilai syarat atau data konfigurasi); order=2 (urutan syarat); type=”TIER” (jenis syarat atau kategori); value=”sekunder” (nilai syarat atau data konfigurasi); order=3 (urutan syarat); type=”FAMILY” (jenis syarat atau kategori); value=”hiburan” (nilai syarat atau data konfigurasi)
-- Uraian sumber 661: Literal +155: tujuanFinansial: katalog tujuan finansial. Tanda kurung mengelompokkan objek/array sesuai hierarki konfigurasi
-- Uraian sumber 661: Literal +156: id=”tujuan_25” (kode identitas komponen); nama=”Kumpul Keluarga” (nama tampilan komponen); hargaBeli=25 (harga beli kartu dalam koin); poinKebahagiaan=20 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +157: id=”tujuan_28” (kode identitas komponen); nama=”Tamasya” (nama tampilan komponen); hargaBeli=28 (harga beli kartu dalam koin); poinKebahagiaan=25 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +158: id=”tujuan_30” (kode identitas komponen); nama=”Keluar Kota” (nama tampilan komponen); hargaBeli=30 (harga beli kartu dalam koin); poinKebahagiaan=28 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +159: id=”tujuan_32” (kode identitas komponen); nama=”Beli Mobil Baru” (nama tampilan komponen); hargaBeli=32 (harga beli kartu dalam koin); poinKebahagiaan=30 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +160: id=”tujuan_35” (kode identitas komponen); nama=”Beli Rumah Baru” (nama tampilan komponen); hargaBeli=35 (harga beli kartu dalam koin); poinKebahagiaan=35 (poin kebahagiaan kartu)
-- Uraian sumber 661: Literal +161: Menutup susunan objek atau daftar JSON yang sedang dibentuk; pemisah koma mempertahankan batas antaranggota dokumen konfigurasi
-- Uraian sumber 661: Literal +162: id=”jual_pertama” (kode identitas komponen); nama=”jual_pertama” (nama tampilan komponen)
-- Uraian sumber 661: Literal +163: aksi=”JualMasakan” (nilai aksi); value=1 (nilai syarat atau data konfigurasi)
-- Uraian sumber 825: Menutup rekaman seed mode=’MAHIR’; kueri melanjutkan pemrosesan kumpulan nilai yang telah lengkap.
-- Uraian sumber 827: Memulai penyisipan rekaman ke rulesets, tabel yang menyimpan identitas paket aturan milik instruktur dan status pengarsipannya; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 828: Mengevaluasi ekspresi rulesets ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 829: Menempatkan rulesets.ruleset_id (UUID paket aturan induk) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 830: Menempatkan rulesets.name (nama entitas) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 831: Menempatkan rulesets.description (uraian entitas) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 832: Menempatkan rulesets.instructor_user_id (UUID instruktur pemilik atau pengelola) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 833: Menempatkan rulesets.created_at (waktu pembuatan rekaman) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 834: Menempatkan rulesets.created_by_user_id (UUID akun pembuat rekaman) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 835: Menutup kelompok yang terkait ; insert into rulesets pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 836: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 837: Memakai ruleset_id (UUID paket aturan induk) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 838: Memakai ruleset_name (nama paket aturan bawaan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 839: Memakai ruleset_description (deskripsi paket aturan bawaan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 840: Memakai null (nilai null) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 841: Mengevaluasi ekspresi now(), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. NOW mengambil waktu awal transaksi PostgreSQL.
-- Uraian sumber 842: Mengevaluasi ekspresi null :: uuid pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 843: Menetapkan sumber baris seed_rulesets on conflict (ruleset_id) do update set name = excluded.name, description = excluded.description; yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 844: Menetapkan penanganan konflik kunci unik pada INSERT: seed_rulesets on conflict (ruleset_id) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 845: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 846: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 847: Menetapkan atau membandingkan name (nama entitas) terhadap excluded.name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 848: Menetapkan atau membandingkan description (uraian entitas) terhadap excluded.description; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 850: Memulai penyisipan rekaman ke ruleset_versions, tabel yang memisahkan revisi aturan sehingga sesi memakai konfigurasi versi yang tetap; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 851: Mengevaluasi ekspresi ruleset_versions ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 852: Menempatkan ruleset_versions.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 853: Menempatkan ruleset_versions.ruleset_id (UUID paket aturan induk) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 854: Menempatkan ruleset_versions.version (nomor revisi paket aturan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 855: Menempatkan ruleset_versions.status (status siklus hidup sesuai pilihan yang divalidasi) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 856: Menempatkan ruleset_versions.mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 857: Menempatkan ruleset_versions.schema_version (versi kontrak skema) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 858: Menempatkan ruleset_versions.config_hash (hash konfigurasi untuk identitas isi aturan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 859: Menempatkan ruleset_versions.change_note (catatan perubahan versi aturan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 860: Menempatkan ruleset_versions.published_at (waktu versi aturan dipublikasikan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 861: Menempatkan ruleset_versions.created_at (waktu pembuatan rekaman) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 862: Menempatkan ruleset_versions.created_by_user_id (UUID akun pembuat rekaman) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 863: Menutup kelompok yang terkait ; insert into ruleset_versions pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 864: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 865: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 866: Memakai ruleset_id (UUID paket aturan induk) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 867: Memakai 1 (nilai 1) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 868: Menyediakan nilai literal ’ACTIVE’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 869: Memakai mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 870: Menyediakan nilai literal ’3.0.0’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 871: Mengevaluasi ekspresi encode(digest(definition_json :: text, ’sha256’), ’hex’), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 872: Menyediakan nilai literal ’Canonical relational catalog seed’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 873: Mengevaluasi ekspresi now(), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. NOW mengambil waktu awal transaksi PostgreSQL.
-- Uraian sumber 874: Mengevaluasi ekspresi now(), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. NOW mengambil waktu awal transaksi PostgreSQL.
-- Uraian sumber 875: Mengevaluasi ekspresi null :: uuid pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 876: Menetapkan sumber baris seed_rulesets on conflict (ruleset_id, version) do update set status = excluded.status, mode = excluded.mode, schema_version = excluded.schema_version, yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 877: Menetapkan penanganan konflik kunci unik pada INSERT: seed_rulesets on conflict (ruleset_id, version) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 878: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 879: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 880: Menetapkan atau membandingkan status (status siklus hidup sesuai pilihan yang divalidasi) terhadap excluded.status, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 881: Menetapkan atau membandingkan mode (mode PEMULA atau MAHIR; BOTH berarti berlaku pada kedua mode jika kolom mengizinkannya) terhadap excluded.mode, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 882: Menetapkan atau membandingkan schema_version (versi kontrak skema) terhadap excluded.schema_version, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 883: Menetapkan atau membandingkan config_hash (hash konfigurasi untuk identitas isi aturan) terhadap excluded.config_hash, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 884: Menetapkan atau membandingkan change_note (catatan perubahan versi aturan) terhadap excluded.change_note, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 885: Menetapkan atau membandingkan published_at (waktu versi aturan dipublikasikan) terhadap excluded.published_at; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 887: Memulai penyisipan rekaman ke ruleset_game_settings, tabel yang menetapkan modal awal, batas pemain, giliran, inventaris, dan fitur setiap versi aturan; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 888: Mengevaluasi ekspresi ruleset_game_settings ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 889: Menempatkan ruleset_game_settings.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 890: Menempatkan ruleset_game_settings.starting_cash (modal koin awal peserta) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 891: Menempatkan ruleset_game_settings.starting_happiness (poin kebahagiaan awal) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 892: Menempatkan ruleset_game_settings.starting_saving (saldo tabungan awal) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 893: Menempatkan ruleset_game_settings.actions_per_turn (jumlah slot aksi per giliran) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 894: Menempatkan ruleset_game_settings.finish_day (hari batas selesai permainan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 895: Menempatkan ruleset_game_settings.min_players (jumlah peserta minimum) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 896: Menempatkan ruleset_game_settings.max_players (jumlah peserta maksimum) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 897: Menempatkan ruleset_game_settings.cash_min (batas saldo koin terendah) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 898: Menempatkan ruleset_game_settings.max_ingredient_total (batas total bahan yang boleh disimpan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 899: Menempatkan ruleset_game_settings.max_same_ingredient (batas kepemilikan bahan sejenis) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 900: Menempatkan ruleset_game_settings.primary_need_max_per_day (batas pembelian kebutuhan primer per hari bila ditetapkan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 901: Menempatkan ruleset_game_settings.require_primary_before_others (kewajiban membeli kebutuhan primer sebelum tingkat lain) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 902: Menempatkan ruleset_game_settings.donation_min_amount (nominal donasi minimum) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 903: Menempatkan ruleset_game_settings.donation_max_amount (nominal donasi maksimum) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 904: Menempatkan ruleset_game_settings.gold_trade_allow_buy (izin pembelian emas) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 905: Menempatkan ruleset_game_settings.gold_trade_allow_sell (izin penjualan emas) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 906: Menempatkan ruleset_game_settings.loan_enabled (status aktivasi fitur pinjaman) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 907: Menempatkan ruleset_game_settings.insurance_enabled (status aktivasi fitur asuransi) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 908: Menempatkan ruleset_game_settings.saving_goal_enabled (status aktivasi tujuan tabungan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 909: Menempatkan ruleset_game_settings.freelance_income (pemasukan dari aksi kerja lepas) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 910: Menutup kelompok yang terkait ; insert into ruleset_game_settings pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 911: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 912: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 913: Mengevaluasi ekspresi coalesce( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 914: Mengevaluasi ekspresi (sr.definition_json ->> ’starting_cash’) :: int, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 915: Memakai 0 (nilai 0) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 916: Menutup kelompok yang terkait . ruleset_version_id , coalesce pada skrip basis data. Koma memisahkan unsur ini dari unsur berikutnya. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 917: Mengevaluasi ekspresi coalesce( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 918: Membuka kelompok yang terkait ) , coalesce ( pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 919: Menetapkan penanganan konflik kunci unik pada INSERT: sr.definition_json #>>’{component_catalog,gameConfig,initialHappiness}’)::int, 0),coalesce((sr.definition_json#>>’{component_catalog,gameConfig,initialSaving}’)::int, 0),coalesce((sr.definition_json->>’actions_per_turn’)::int, 2),coalesc.... DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai; NOW mengambil waktu awal transaksi PostgreSQL; EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 920: Memulai penyisipan rekaman ke ruleset_player_ordering_rules, tabel yang menetapkan prioritas urutan pemain serta aturan khusus hari dalam satu versi aturan; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 921: Mengevaluasi ekspresi ruleset_player_ordering_rules ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 922: Menempatkan ruleset_player_ordering_rules.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 923: Menempatkan ruleset_player_ordering_rules.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 924: Menempatkan ruleset_player_ordering_rules.ordering_code (metode pengurutan pemain) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 925: Menempatkan ruleset_player_ordering_rules.weekday_code (kode hari khusus) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 926: Menempatkan ruleset_player_ordering_rules.feature_code (kode fitur pada hari tersebut) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 927: Menempatkan ruleset_player_ordering_rules.is_enabled (penanda fitur diaktifkan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 928: Menutup kelompok yang terkait ; insert into ruleset_player_ordering_rules pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 929: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 930: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 931: Memakai 1 (nilai 1) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 932: Mengevaluasi ekspresi coalesce( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 933: Mengevaluasi ekspresi sr.definition_json ->> ’player_ordering’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 934: Menyediakan nilai literal ’PLAYER_ORDER’ dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 935: Menutup kelompok yang terkait , 1 , coalesce pada skrip basis data. Koma memisahkan unsur ini dari unsur berikutnya. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 936: Memakai null (nilai null) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 937: Memakai null (nilai null) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 938: Memakai true (nilai true) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 939: Menetapkan sumber baris seed_rulesets sr on conflict (ruleset_version_id, sort_order) do update set ordering_code = excluded.ordering_code, weekday_code = excluded.weekday_code, fea... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 940: Menetapkan penanganan konflik kunci unik pada INSERT: seed_rulesets sr on conflict (ruleset_version_id, sort_order) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 941: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 942: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 943: Menetapkan atau membandingkan ordering_code (metode pengurutan pemain) terhadap excluded.ordering_code, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 944: Menetapkan atau membandingkan weekday_code (kode hari khusus) terhadap excluded.weekday_code, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 945: Menetapkan atau membandingkan feature_code (kode fitur pada hari tersebut) terhadap excluded.feature_code, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 946: Menetapkan atau membandingkan is_enabled (penanda fitur diaktifkan) terhadap excluded.is_enabled; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 948: Memulai penyisipan rekaman ke ruleset_player_ordering_rules, tabel yang menetapkan prioritas urutan pemain serta aturan khusus hari dalam satu versi aturan; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 949: Mengevaluasi ekspresi ruleset_player_ordering_rules ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 950: Menempatkan ruleset_player_ordering_rules.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 951: Menempatkan ruleset_player_ordering_rules.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 952: Menempatkan ruleset_player_ordering_rules.ordering_code (metode pengurutan pemain) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 953: Menempatkan ruleset_player_ordering_rules.weekday_code (kode hari khusus) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 954: Menempatkan ruleset_player_ordering_rules.feature_code (kode fitur pada hari tersebut) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 955: Menempatkan ruleset_player_ordering_rules.is_enabled (penanda fitur diaktifkan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 956: Menutup kelompok yang terkait ; insert into ruleset_player_ordering_rules pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 957: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 958: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 959: Memakai mapped.sort_order (urutan tampilan atau evaluasi komponen) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 960: Memakai null (nilai null) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 961: Memakai mapped.weekday_code (kode hari khusus) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 962: Memakai mapped.feature_code (kode fitur pada hari tersebut) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 963: Memakai mapped.is_enabled (penanda fitur diaktifkan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 964: Menetapkan sumber baris seed_rulesets sr cross join lateral ( values ( 10, ’FRI’ :: varchar(8), yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 965: Mengevaluasi ekspresi seed_rulesets sr pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 966: Menggabungkan sumber cross join lateral ( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog.
-- Uraian sumber 967: Menyediakan 3 tuple nilai eksplisit untuk mapped; setiap tuple membentuk satu rekaman dan mengikuti urutan sort_order, weekday_code, feature_code, is_enabled.
-- Uraian sumber 968: Membuka rekaman seed mapped, rekaman ke-1; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 969: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 10 untuk mapped, rekaman ke-1.
-- Uraian sumber 970: Mengisi weekday_code (kode hari khusus) dengan ’FRI’ :: varchar(8) untuk mapped, rekaman ke-1.
-- Uraian sumber 971: Mengisi feature_code (kode fitur pada hari tersebut) dengan coalesce( sr.definition_json #>>’{weekday_rules,friday,feature}’, sr.definition_json#>>’{weekday_rules,FRI,feature}’, ’DONATION’)::varchar(40) untuk mapped, rekaman ke-1.
-- Uraian sumber 972: Mengisi feature_code (kode fitur pada hari tersebut) dengan coalesce( sr.definition_json #>>’{weekday_rules,friday,feature}’, sr.definition_json#>>’{weekday_rules,FRI,feature}’, ’DONATION’)::varchar(40) untuk mapped, rekaman ke-1.
-- Uraian sumber 973: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 974: Memakai sumber atau sasaran ruleset_game_assets, yang menjadi katalog induk seluruh aset atau kartu yang dirujuk aturan dan posisi kartu sesi, dalam skrip basis data; posisi klausa menentukan apakah tabel dibaca atau diubah.
-- Uraian sumber 975: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 976: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap false pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan.
-- Uraian sumber 977: Membatasi baris skrip basis data dengan syarat ruleset_version_id = ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid and asset_type in (’RISK’, ’TIE_BREAKER’); insert into ruleset_game_assets ( ruleset_vers...; hanya baris dengan predikat TRUE masuk hasil atau terkena perubahan data.
-- Uraian sumber 978: Menetapkan atau membandingkan ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) terhadap ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 979: Menambahkan syarat wajib asset_type in (’RISK’, ’TIE_BREAKER’); pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos.
-- Uraian sumber 981: Memulai penyisipan rekaman ke ruleset_game_assets, tabel yang menjadi katalog induk seluruh aset atau kartu yang dirujuk aturan dan posisi kartu sesi; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 982: Mengevaluasi ekspresi ruleset_game_assets ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 983: Menempatkan ruleset_game_assets.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 984: Menempatkan ruleset_game_assets.asset_type (kategori aset atau kartu) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 985: Menempatkan ruleset_game_assets.asset_code (kode aset dalam versi aturan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 986: Menempatkan ruleset_game_assets.display_name (nama yang ditampilkan kepada pengguna) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 987: Menempatkan ruleset_game_assets.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 988: Menempatkan ruleset_game_assets.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 989: Menempatkan ruleset_game_assets.metadata_json (metadata tambahan aset dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 990: Membuka CTE json_assets, yaitu hasil kueri sementara dalam pernyataan ini, untuk mengolah data json assets.
-- Uraian sumber 991: Memulai pemilihan hasil pada CTE json_assets; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 992: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada CTE json_assets; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 993: Menyediakan nilai literal ’INGREDIENT’ :: varchar(40) as asset_type, dalam CTE json_assets; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis. memberi nama hasil asset_type (kategori aset atau kartu); operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 994: Mengevaluasi ekspresi coalesce( pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 995: Mengevaluasi ekspresi item ->> ’id’, pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 996: Mengevaluasi ekspresi regexp_replace(lower(item ->> ’nama’), ’\s+’, ’’, ’g’) pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; LOWER menormalkan huruf untuk pencocokan kode/nama tanpa perbedaan kapital.
-- Uraian sumber 997: Mengevaluasi ekspresi ) as asset_code, pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil asset_code (kode aset dalam versi aturan).
-- Uraian sumber 998: Mengevaluasi ekspresi item ->> ’nama’ as display_name, pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil display_name (nama yang ditampilkan kepada pengguna); operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 999: Mengevaluasi ekspresi ord :: int as sort_order, pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil sort_order (urutan tampilan atau evaluasi komponen); operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1000: Mengevaluasi ekspresi true as is_active, pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil is_active (penanda apakah entitas masih boleh digunakan).
-- Uraian sumber 1001: Mengevaluasi ekspresi jsonb_build_object(’source’, ’component_catalog.bahan’) as metadata_json pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil metadata_json (metadata tambahan aset dalam JSONB); jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 1002: Menetapkan sumber baris seed_rulesets sr cross join lateral jsonb_array_elements( coalesce( sr.definition_json -> ’component_catalog’ -> ’bahan’, ’[]’ :: jsonb ) yang dibaca dalam CTE json_assets; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1003: Mengevaluasi ekspresi seed_rulesets sr pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1004: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam CTE json_assets. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 1005: Mengevaluasi ekspresi coalesce( pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 1006: Mengevaluasi ekspresi sr.definition_json -> ’component_catalog’ -> ’bahan’, pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 1007: Menyediakan parameter JSONB bagi CTE json_assets: Menutup susunan objek atau daftar JSON yang sedang dibentuk; pemisah koma mempertahankan batas antaranggota dokumen konfigurasi. Cast ::jsonb memvalidasi struktur JSON sebelum nilai dipakai aturan atau event.
-- Uraian sumber 1008: Menutup kelompok yang terkait lateral jsonb_array_elements ( coalesce pada CTE json_assets. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1009: Mengevaluasi ekspresi ) with ordinality as x(item, ord) pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil x (nilai x).
-- Uraian sumber 1010: Menggabungkan hasil kueri dalam CTE json_assets; UNION ALL mempertahankan seluruh baris termasuk duplikat, sedangkan UNION tanpa ALL menghilangkan baris hasil yang sama.
-- Uraian sumber 1011: Menggabungkan hasil kueri dalam CTE json_assets; UNION ALL mempertahankan seluruh baris termasuk duplikat, sedangkan UNION tanpa ALL menghilangkan baris hasil yang sama.
-- Uraian sumber 1012: Memulai pemilihan hasil pada CTE json_assets; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1013: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada CTE json_assets; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1014: Menyediakan nilai literal ’ORDER’ :: varchar(40), dalam CTE json_assets; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1015: Mengevaluasi ekspresi item ->> ’id’, pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1016: Mengevaluasi ekspresi item ->> ’nama’, pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1017: Mengevaluasi ekspresi (100 + ord) :: int, pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1018: Memakai true (nilai true) sebagai unsur ekspresi atau daftar pada CTE json_assets; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1019: Mengevaluasi ekspresi jsonb_build_object(’source’, ’component_catalog.resep’) pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 1020: Menetapkan sumber baris seed_rulesets sr cross join lateral jsonb_array_elements( coalesce( sr.definition_json -> ’component_catalog’ -> ’resep’, ’[]’ :: jsonb ) yang dibaca dalam CTE json_assets; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1021: Mengevaluasi ekspresi seed_rulesets sr pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1022: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam CTE json_assets. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 1023: Mengevaluasi ekspresi coalesce( pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 1024: Mengevaluasi ekspresi sr.definition_json -> ’component_catalog’ -> ’resep’, pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 1025: Menyediakan parameter JSONB bagi CTE json_assets: Menutup susunan objek atau daftar JSON yang sedang dibentuk; pemisah koma mempertahankan batas antaranggota dokumen konfigurasi. Cast ::jsonb memvalidasi struktur JSON sebelum nilai dipakai aturan atau event.
-- Uraian sumber 1026: Menutup kelompok yang terkait lateral jsonb_array_elements ( coalesce pada CTE json_assets. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1027: Mengevaluasi ekspresi ) with ordinality as x(item, ord) pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil x (nilai x).
-- Uraian sumber 1028: Menggabungkan hasil kueri dalam CTE json_assets; UNION ALL mempertahankan seluruh baris termasuk duplikat, sedangkan UNION tanpa ALL menghilangkan baris hasil yang sama.
-- Uraian sumber 1029: Menggabungkan hasil kueri dalam CTE json_assets; UNION ALL mempertahankan seluruh baris termasuk duplikat, sedangkan UNION tanpa ALL menghilangkan baris hasil yang sama.
-- Uraian sumber 1030: Memulai pemilihan hasil pada CTE json_assets; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1031: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada CTE json_assets; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1032: Menyediakan nilai literal ’NEED’ :: varchar(40), dalam CTE json_assets; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1033: Mengevaluasi ekspresi item ->> ’id’, pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1034: Mengevaluasi ekspresi item ->> ’nama’, pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1035: Mengevaluasi ekspresi (200 + ord) :: int, pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1036: Memakai true (nilai true) sebagai unsur ekspresi atau daftar pada CTE json_assets; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1037: Mengevaluasi ekspresi jsonb_build_object(’source’, ’component_catalog.kebutuhan’) pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 1038: Menetapkan sumber baris seed_rulesets sr cross join lateral jsonb_array_elements( coalesce( sr.definition_json -> ’component_catalog’ -> ’kebutuhan’, ’[]’ :: jsonb ) yang dibaca dalam CTE json_assets; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1039: Mengevaluasi ekspresi seed_rulesets sr pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1040: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam CTE json_assets. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 1041: Mengevaluasi ekspresi coalesce( pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 1042: Mengevaluasi ekspresi sr.definition_json -> ’component_catalog’ -> ’kebutuhan’, pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 1043: Menyediakan parameter JSONB bagi CTE json_assets: Menutup susunan objek atau daftar JSON yang sedang dibentuk; pemisah koma mempertahankan batas antaranggota dokumen konfigurasi. Cast ::jsonb memvalidasi struktur JSON sebelum nilai dipakai aturan atau event.
-- Uraian sumber 1044: Menutup kelompok yang terkait lateral jsonb_array_elements ( coalesce pada CTE json_assets. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1045: Mengevaluasi ekspresi ) with ordinality as x(item, ord) pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil x (nilai x).
-- Uraian sumber 1046: Menggabungkan hasil kueri dalam CTE json_assets; UNION ALL mempertahankan seluruh baris termasuk duplikat, sedangkan UNION tanpa ALL menghilangkan baris hasil yang sama.
-- Uraian sumber 1047: Menggabungkan hasil kueri dalam CTE json_assets; UNION ALL mempertahankan seluruh baris termasuk duplikat, sedangkan UNION tanpa ALL menghilangkan baris hasil yang sama.
-- Uraian sumber 1048: Memulai pemilihan hasil pada CTE json_assets; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1049: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada CTE json_assets; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1050: Menyediakan nilai literal ’GOLD’ :: varchar(40), dalam CTE json_assets; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1051: Menyediakan nilai literal ’gold_card’ :: varchar(120), dalam CTE json_assets; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1052: Menyediakan nilai literal ’Kartu Emas’ :: varchar(160), dalam CTE json_assets; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1053: Menyediakan nilai literal 701 :: int, dalam CTE json_assets; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1054: Memakai true (nilai true) sebagai unsur ekspresi atau daftar pada CTE json_assets; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1055: Mengevaluasi ekspresi jsonb_build_object(’source’, ’ruleset_gold_assets’, ’card_qty’, 20) pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 1056: Menetapkan sumber baris seed_rulesets sr ), static_assets as ( select * from yang dibaca dalam CTE json_assets; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1057: Mengevaluasi ekspresi seed_rulesets sr pada CTE json_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1058: Menutup kelompok yang terkait ) with json_assets as pada CTE json_assets. Koma memisahkan unsur ini dari unsur berikutnya. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1059: Membuka CTE static_assets, yaitu hasil kueri sementara dalam pernyataan ini, untuk mengolah data static assets.
-- Uraian sumber 1060: Memulai pemilihan hasil pada CTE static_assets; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1061: Mengambil semua kolom sumber pada CTE static_assets; bentuk rekaman mengikuti hasil sumber kueri.
-- Uraian sumber 1062: Menetapkan sumber baris ( values ( ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid, ’GOLD_PRICE’ :: varchar(40), ’gold_price_1’ :: varchar(120), yang dibaca dalam CTE static_assets; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1063: Membuka kelompok yang terkait ( select * from pada CTE static_assets. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1064: Menyediakan 39 tuple nilai eksplisit untuk CTE static_assets; setiap tuple membentuk satu rekaman dan mengikuti urutan ruleset_version_id, asset_type, asset_code, display_name, sort_order, is_active, metadata_json.
-- Uraian sumber 1065: Membuka rekaman seed asset_code=’gold_price_1’ :: varchar(120); posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1066: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk asset_code=’gold_price_1’ :: varchar(120).
-- Uraian sumber 1067: Mengisi asset_type (kategori aset atau kartu) dengan ’GOLD_PRICE’ :: varchar(40) untuk asset_code=’gold_price_1’ :: varchar(120).
-- Uraian sumber 1068: Mengisi asset_code (kode aset dalam versi aturan) dengan ’gold_price_1’ :: varchar(120) untuk asset_code=’gold_price_1’ :: varchar(120).
-- Uraian sumber 1069: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Harga Emas 1’ :: varchar(160) untuk asset_code=’gold_price_1’ :: varchar(120).
-- Uraian sumber 1070: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 801 untuk asset_code=’gold_price_1’ :: varchar(120).
-- Uraian sumber 1071: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’gold_price_1’ :: varchar(120). Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1072: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.gold_price”}’ :: jsonb untuk asset_code=’gold_price_1’ :: varchar(120). Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.gold_price” (asal definisi komponen).
-- Uraian sumber 1073: Menutup rekaman seed asset_code=’gold_price_1’ :: varchar(120); koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1074: Membuka rekaman seed asset_code=’gold_price_2’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1075: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk asset_code=’gold_price_2’.
-- Uraian sumber 1076: Mengisi asset_type (kategori aset atau kartu) dengan ’GOLD_PRICE’ untuk asset_code=’gold_price_2’.
-- Uraian sumber 1077: Mengisi asset_code (kode aset dalam versi aturan) dengan ’gold_price_2’ untuk asset_code=’gold_price_2’.
-- Uraian sumber 1078: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Harga Emas 2’ untuk asset_code=’gold_price_2’.
-- Uraian sumber 1079: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 802 untuk asset_code=’gold_price_2’.
-- Uraian sumber 1080: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’gold_price_2’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1081: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.gold_price”}’ :: jsonb untuk asset_code=’gold_price_2’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.gold_price” (asal definisi komponen).
-- Uraian sumber 1082: Menutup rekaman seed asset_code=’gold_price_2’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1083: Membuka rekaman seed asset_code=’gold_price_3’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1084: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk asset_code=’gold_price_3’.
-- Uraian sumber 1085: Mengisi asset_type (kategori aset atau kartu) dengan ’GOLD_PRICE’ untuk asset_code=’gold_price_3’.
-- Uraian sumber 1086: Mengisi asset_code (kode aset dalam versi aturan) dengan ’gold_price_3’ untuk asset_code=’gold_price_3’.
-- Uraian sumber 1087: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Harga Emas 3’ untuk asset_code=’gold_price_3’.
-- Uraian sumber 1088: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 803 untuk asset_code=’gold_price_3’.
-- Uraian sumber 1089: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’gold_price_3’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1090: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.gold_price”}’ :: jsonb untuk asset_code=’gold_price_3’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.gold_price” (asal definisi komponen).
-- Uraian sumber 1091: Menutup rekaman seed asset_code=’gold_price_3’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1092: Membuka rekaman seed asset_code=’gold_price_4’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1093: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk asset_code=’gold_price_4’.
-- Uraian sumber 1094: Mengisi asset_type (kategori aset atau kartu) dengan ’GOLD_PRICE’ untuk asset_code=’gold_price_4’.
-- Uraian sumber 1095: Mengisi asset_code (kode aset dalam versi aturan) dengan ’gold_price_4’ untuk asset_code=’gold_price_4’.
-- Uraian sumber 1096: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Harga Emas 4’ untuk asset_code=’gold_price_4’.
-- Uraian sumber 1097: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 804 untuk asset_code=’gold_price_4’.
-- Uraian sumber 1098: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’gold_price_4’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1099: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.gold_price”}’ :: jsonb untuk asset_code=’gold_price_4’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.gold_price” (asal definisi komponen).
-- Uraian sumber 1100: Menutup rekaman seed asset_code=’gold_price_4’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1101: Membuka rekaman seed asset_code=’tie_breaker_1’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1102: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk asset_code=’tie_breaker_1’.
-- Uraian sumber 1103: Mengisi asset_type (kategori aset atau kartu) dengan ’TIE_BREAKER’ untuk asset_code=’tie_breaker_1’.
-- Uraian sumber 1104: Mengisi asset_code (kode aset dalam versi aturan) dengan ’tie_breaker_1’ untuk asset_code=’tie_breaker_1’.
-- Uraian sumber 1105: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Tie Breaker 1’ untuk asset_code=’tie_breaker_1’.
-- Uraian sumber 1106: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 901 untuk asset_code=’tie_breaker_1’.
-- Uraian sumber 1107: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’tie_breaker_1’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1108: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.tie_breaker”}’ :: jsonb untuk asset_code=’tie_breaker_1’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.tie_breaker” (asal definisi komponen).
-- Uraian sumber 1109: Menutup rekaman seed asset_code=’tie_breaker_1’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1110: Membuka rekaman seed asset_code=’tie_breaker_2’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1111: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk asset_code=’tie_breaker_2’.
-- Uraian sumber 1112: Mengisi asset_type (kategori aset atau kartu) dengan ’TIE_BREAKER’ untuk asset_code=’tie_breaker_2’.
-- Uraian sumber 1113: Mengisi asset_code (kode aset dalam versi aturan) dengan ’tie_breaker_2’ untuk asset_code=’tie_breaker_2’.
-- Uraian sumber 1114: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Tie Breaker 2’ untuk asset_code=’tie_breaker_2’.
-- Uraian sumber 1115: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 902 untuk asset_code=’tie_breaker_2’.
-- Uraian sumber 1116: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’tie_breaker_2’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1117: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.tie_breaker”}’ :: jsonb untuk asset_code=’tie_breaker_2’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.tie_breaker” (asal definisi komponen).
-- Uraian sumber 1118: Menutup rekaman seed asset_code=’tie_breaker_2’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1119: Membuka rekaman seed asset_code=’tie_breaker_3’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1120: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk asset_code=’tie_breaker_3’.
-- Uraian sumber 1121: Mengisi asset_type (kategori aset atau kartu) dengan ’TIE_BREAKER’ untuk asset_code=’tie_breaker_3’.
-- Uraian sumber 1122: Mengisi asset_code (kode aset dalam versi aturan) dengan ’tie_breaker_3’ untuk asset_code=’tie_breaker_3’.
-- Uraian sumber 1123: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Tie Breaker 3’ untuk asset_code=’tie_breaker_3’.
-- Uraian sumber 1124: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 903 untuk asset_code=’tie_breaker_3’.
-- Uraian sumber 1125: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’tie_breaker_3’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1126: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.tie_breaker”}’ :: jsonb untuk asset_code=’tie_breaker_3’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.tie_breaker” (asal definisi komponen).
-- Uraian sumber 1127: Menutup rekaman seed asset_code=’tie_breaker_3’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1128: Membuka rekaman seed asset_code=’tie_breaker_4’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1129: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk asset_code=’tie_breaker_4’.
-- Uraian sumber 1130: Mengisi asset_type (kategori aset atau kartu) dengan ’TIE_BREAKER’ untuk asset_code=’tie_breaker_4’.
-- Uraian sumber 1131: Mengisi asset_code (kode aset dalam versi aturan) dengan ’tie_breaker_4’ untuk asset_code=’tie_breaker_4’.
-- Uraian sumber 1132: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Tie Breaker 4’ untuk asset_code=’tie_breaker_4’.
-- Uraian sumber 1133: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 904 untuk asset_code=’tie_breaker_4’.
-- Uraian sumber 1134: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’tie_breaker_4’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1135: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.tie_breaker”}’ :: jsonb untuk asset_code=’tie_breaker_4’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.tie_breaker” (asal definisi komponen).
-- Uraian sumber 1136: Menutup rekaman seed asset_code=’tie_breaker_4’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1137: Membuka rekaman seed asset_code=’gold_price_1’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1138: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’gold_price_1’.
-- Uraian sumber 1139: Mengisi asset_type (kategori aset atau kartu) dengan ’GOLD_PRICE’ untuk asset_code=’gold_price_1’.
-- Uraian sumber 1140: Mengisi asset_code (kode aset dalam versi aturan) dengan ’gold_price_1’ untuk asset_code=’gold_price_1’.
-- Uraian sumber 1141: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Harga Emas 1’ untuk asset_code=’gold_price_1’.
-- Uraian sumber 1142: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 801 untuk asset_code=’gold_price_1’.
-- Uraian sumber 1143: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’gold_price_1’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1144: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.gold_price”}’ :: jsonb untuk asset_code=’gold_price_1’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.gold_price” (asal definisi komponen).
-- Uraian sumber 1145: Menutup rekaman seed asset_code=’gold_price_1’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1146: Membuka rekaman seed asset_code=’gold_price_2’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1147: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’gold_price_2’.
-- Uraian sumber 1148: Mengisi asset_type (kategori aset atau kartu) dengan ’GOLD_PRICE’ untuk asset_code=’gold_price_2’.
-- Uraian sumber 1149: Mengisi asset_code (kode aset dalam versi aturan) dengan ’gold_price_2’ untuk asset_code=’gold_price_2’.
-- Uraian sumber 1150: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Harga Emas 2’ untuk asset_code=’gold_price_2’.
-- Uraian sumber 1151: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 802 untuk asset_code=’gold_price_2’.
-- Uraian sumber 1152: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’gold_price_2’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1153: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.gold_price”}’ :: jsonb untuk asset_code=’gold_price_2’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.gold_price” (asal definisi komponen).
-- Uraian sumber 1154: Menutup rekaman seed asset_code=’gold_price_2’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1155: Membuka rekaman seed asset_code=’gold_price_3’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1156: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’gold_price_3’.
-- Uraian sumber 1157: Mengisi asset_type (kategori aset atau kartu) dengan ’GOLD_PRICE’ untuk asset_code=’gold_price_3’.
-- Uraian sumber 1158: Mengisi asset_code (kode aset dalam versi aturan) dengan ’gold_price_3’ untuk asset_code=’gold_price_3’.
-- Uraian sumber 1159: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Harga Emas 3’ untuk asset_code=’gold_price_3’.
-- Uraian sumber 1160: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 803 untuk asset_code=’gold_price_3’.
-- Uraian sumber 1161: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’gold_price_3’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1162: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.gold_price”}’ :: jsonb untuk asset_code=’gold_price_3’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.gold_price” (asal definisi komponen).
-- Uraian sumber 1163: Menutup rekaman seed asset_code=’gold_price_3’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1164: Membuka rekaman seed asset_code=’gold_price_4’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1165: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’gold_price_4’.
-- Uraian sumber 1166: Mengisi asset_type (kategori aset atau kartu) dengan ’GOLD_PRICE’ untuk asset_code=’gold_price_4’.
-- Uraian sumber 1167: Mengisi asset_code (kode aset dalam versi aturan) dengan ’gold_price_4’ untuk asset_code=’gold_price_4’.
-- Uraian sumber 1168: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Harga Emas 4’ untuk asset_code=’gold_price_4’.
-- Uraian sumber 1169: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 804 untuk asset_code=’gold_price_4’.
-- Uraian sumber 1170: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’gold_price_4’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1171: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.gold_price”}’ :: jsonb untuk asset_code=’gold_price_4’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.gold_price” (asal definisi komponen).
-- Uraian sumber 1172: Menutup rekaman seed asset_code=’gold_price_4’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1173: Membuka rekaman seed asset_code=’tie_breaker_1’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1174: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’tie_breaker_1’.
-- Uraian sumber 1175: Mengisi asset_type (kategori aset atau kartu) dengan ’TIE_BREAKER’ untuk asset_code=’tie_breaker_1’.
-- Uraian sumber 1176: Mengisi asset_code (kode aset dalam versi aturan) dengan ’tie_breaker_1’ untuk asset_code=’tie_breaker_1’.
-- Uraian sumber 1177: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Tie Breaker 1’ untuk asset_code=’tie_breaker_1’.
-- Uraian sumber 1178: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 901 untuk asset_code=’tie_breaker_1’.
-- Uraian sumber 1179: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’tie_breaker_1’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1180: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.tie_breaker”}’ :: jsonb untuk asset_code=’tie_breaker_1’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.tie_breaker” (asal definisi komponen).
-- Uraian sumber 1181: Menutup rekaman seed asset_code=’tie_breaker_1’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1182: Membuka rekaman seed asset_code=’tie_breaker_2’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1183: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’tie_breaker_2’.
-- Uraian sumber 1184: Mengisi asset_type (kategori aset atau kartu) dengan ’TIE_BREAKER’ untuk asset_code=’tie_breaker_2’.
-- Uraian sumber 1185: Mengisi asset_code (kode aset dalam versi aturan) dengan ’tie_breaker_2’ untuk asset_code=’tie_breaker_2’.
-- Uraian sumber 1186: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Tie Breaker 2’ untuk asset_code=’tie_breaker_2’.
-- Uraian sumber 1187: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 902 untuk asset_code=’tie_breaker_2’.
-- Uraian sumber 1188: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’tie_breaker_2’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1189: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.tie_breaker”}’ :: jsonb untuk asset_code=’tie_breaker_2’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.tie_breaker” (asal definisi komponen).
-- Uraian sumber 1190: Menutup rekaman seed asset_code=’tie_breaker_2’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1191: Membuka rekaman seed asset_code=’tie_breaker_3’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1192: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’tie_breaker_3’.
-- Uraian sumber 1193: Mengisi asset_type (kategori aset atau kartu) dengan ’TIE_BREAKER’ untuk asset_code=’tie_breaker_3’.
-- Uraian sumber 1194: Mengisi asset_code (kode aset dalam versi aturan) dengan ’tie_breaker_3’ untuk asset_code=’tie_breaker_3’.
-- Uraian sumber 1195: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Tie Breaker 3’ untuk asset_code=’tie_breaker_3’.
-- Uraian sumber 1196: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 903 untuk asset_code=’tie_breaker_3’.
-- Uraian sumber 1197: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’tie_breaker_3’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1198: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.tie_breaker”}’ :: jsonb untuk asset_code=’tie_breaker_3’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.tie_breaker” (asal definisi komponen).
-- Uraian sumber 1199: Menutup rekaman seed asset_code=’tie_breaker_3’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1200: Membuka rekaman seed asset_code=’tie_breaker_4’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1201: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’tie_breaker_4’.
-- Uraian sumber 1202: Mengisi asset_type (kategori aset atau kartu) dengan ’TIE_BREAKER’ untuk asset_code=’tie_breaker_4’.
-- Uraian sumber 1203: Mengisi asset_code (kode aset dalam versi aturan) dengan ’tie_breaker_4’ untuk asset_code=’tie_breaker_4’.
-- Uraian sumber 1204: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Tie Breaker 4’ untuk asset_code=’tie_breaker_4’.
-- Uraian sumber 1205: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 904 untuk asset_code=’tie_breaker_4’.
-- Uraian sumber 1206: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’tie_breaker_4’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1207: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.tie_breaker”}’ :: jsonb untuk asset_code=’tie_breaker_4’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.tie_breaker” (asal definisi komponen).
-- Uraian sumber 1208: Menutup rekaman seed asset_code=’tie_breaker_4’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1209: Membuka rekaman seed asset_code=’risk_beli_peralatan_dapur’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1210: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_beli_peralatan_dapur’.
-- Uraian sumber 1211: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_beli_peralatan_dapur’.
-- Uraian sumber 1212: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_beli_peralatan_dapur’ untuk asset_code=’risk_beli_peralatan_dapur’.
-- Uraian sumber 1213: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Beli Peralatan Dapur’ untuk asset_code=’risk_beli_peralatan_dapur’.
-- Uraian sumber 1214: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1202 untuk asset_code=’risk_beli_peralatan_dapur’.
-- Uraian sumber 1215: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_beli_peralatan_dapur’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1216: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_beli_peralatan_dapur’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1217: Menutup rekaman seed asset_code=’risk_beli_peralatan_dapur’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1218: Membuka rekaman seed asset_code=’risk_menang_undian’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1219: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_menang_undian’.
-- Uraian sumber 1220: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_menang_undian’.
-- Uraian sumber 1221: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_menang_undian’ untuk asset_code=’risk_menang_undian’.
-- Uraian sumber 1222: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Menang Undian’ untuk asset_code=’risk_menang_undian’.
-- Uraian sumber 1223: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1203 untuk asset_code=’risk_menang_undian’.
-- Uraian sumber 1224: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_menang_undian’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1225: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_menang_undian’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1226: Menutup rekaman seed asset_code=’risk_menang_undian’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1227: Membuka rekaman seed asset_code=’risk_study_tour’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1228: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_study_tour’.
-- Uraian sumber 1229: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_study_tour’.
-- Uraian sumber 1230: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_study_tour’ untuk asset_code=’risk_study_tour’.
-- Uraian sumber 1231: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Study Tour’ untuk asset_code=’risk_study_tour’.
-- Uraian sumber 1232: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1204 untuk asset_code=’risk_study_tour’.
-- Uraian sumber 1233: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_study_tour’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1234: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_study_tour’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1235: Menutup rekaman seed asset_code=’risk_study_tour’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1236: Membuka rekaman seed asset_code=’risk_depresi’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1237: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_depresi’.
-- Uraian sumber 1238: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_depresi’.
-- Uraian sumber 1239: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_depresi’ untuk asset_code=’risk_depresi’.
-- Uraian sumber 1240: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Depresi’ untuk asset_code=’risk_depresi’.
-- Uraian sumber 1241: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1205 untuk asset_code=’risk_depresi’.
-- Uraian sumber 1242: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_depresi’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1243: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_depresi’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1244: Menutup rekaman seed asset_code=’risk_depresi’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1245: Membuka rekaman seed asset_code=’risk_panen_melimpah’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1246: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_panen_melimpah’.
-- Uraian sumber 1247: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_panen_melimpah’.
-- Uraian sumber 1248: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_panen_melimpah’ untuk asset_code=’risk_panen_melimpah’.
-- Uraian sumber 1249: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Panen Melimpah’ untuk asset_code=’risk_panen_melimpah’.
-- Uraian sumber 1250: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1206 untuk asset_code=’risk_panen_melimpah’.
-- Uraian sumber 1251: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_panen_melimpah’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1252: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_panen_melimpah’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1253: Menutup rekaman seed asset_code=’risk_panen_melimpah’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1254: Membuka rekaman seed asset_code=’risk_ekstrakurikuler_anak’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1255: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_ekstrakurikuler_anak’.
-- Uraian sumber 1256: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_ekstrakurikuler_anak’.
-- Uraian sumber 1257: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_ekstrakurikuler_anak’ untuk asset_code=’risk_ekstrakurikuler_anak’.
-- Uraian sumber 1258: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Ekstrakurikuler Anak’ untuk asset_code=’risk_ekstrakurikuler_anak’.
-- Uraian sumber 1259: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1207 untuk asset_code=’risk_ekstrakurikuler_anak’.
-- Uraian sumber 1260: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_ekstrakurikuler_anak’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1261: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_ekstrakurikuler_anak’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1262: Menutup rekaman seed asset_code=’risk_ekstrakurikuler_anak’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1263: Membuka rekaman seed asset_code=’risk_ban_bocor’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1264: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_ban_bocor’.
-- Uraian sumber 1265: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_ban_bocor’.
-- Uraian sumber 1266: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_ban_bocor’ untuk asset_code=’risk_ban_bocor’.
-- Uraian sumber 1267: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Ban Bocor’ untuk asset_code=’risk_ban_bocor’.
-- Uraian sumber 1268: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1208 untuk asset_code=’risk_ban_bocor’.
-- Uraian sumber 1269: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_ban_bocor’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1270: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_ban_bocor’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1271: Menutup rekaman seed asset_code=’risk_ban_bocor’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1272: Membuka rekaman seed asset_code=’risk_sakit_gigi’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1273: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_sakit_gigi’.
-- Uraian sumber 1274: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_sakit_gigi’.
-- Uraian sumber 1275: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_sakit_gigi’ untuk asset_code=’risk_sakit_gigi’.
-- Uraian sumber 1276: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Sakit Gigi’ untuk asset_code=’risk_sakit_gigi’.
-- Uraian sumber 1277: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1209 untuk asset_code=’risk_sakit_gigi’.
-- Uraian sumber 1278: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_sakit_gigi’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1279: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_sakit_gigi’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1280: Menutup rekaman seed asset_code=’risk_sakit_gigi’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1281: Membuka rekaman seed asset_code=’risk_operasi_usus_buntu’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1282: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_operasi_usus_buntu’.
-- Uraian sumber 1283: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_operasi_usus_buntu’.
-- Uraian sumber 1284: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_operasi_usus_buntu’ untuk asset_code=’risk_operasi_usus_buntu’.
-- Uraian sumber 1285: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Operasi Usus Buntu’ untuk asset_code=’risk_operasi_usus_buntu’.
-- Uraian sumber 1286: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1210 untuk asset_code=’risk_operasi_usus_buntu’.
-- Uraian sumber 1287: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_operasi_usus_buntu’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1288: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_operasi_usus_buntu’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1289: Menutup rekaman seed asset_code=’risk_operasi_usus_buntu’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1290: Membuka rekaman seed asset_code=’risk_ganti_oli’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1291: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_ganti_oli’.
-- Uraian sumber 1292: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_ganti_oli’.
-- Uraian sumber 1293: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_ganti_oli’ untuk asset_code=’risk_ganti_oli’.
-- Uraian sumber 1294: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Ganti Oli’ untuk asset_code=’risk_ganti_oli’.
-- Uraian sumber 1295: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1211 untuk asset_code=’risk_ganti_oli’.
-- Uraian sumber 1296: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_ganti_oli’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1297: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_ganti_oli’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1298: Menutup rekaman seed asset_code=’risk_ganti_oli’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1299: Membuka rekaman seed asset_code=’risk_mobil_tabrakan’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1300: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_mobil_tabrakan’.
-- Uraian sumber 1301: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_mobil_tabrakan’.
-- Uraian sumber 1302: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_mobil_tabrakan’ untuk asset_code=’risk_mobil_tabrakan’.
-- Uraian sumber 1303: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Mobil Tabrakan’ untuk asset_code=’risk_mobil_tabrakan’.
-- Uraian sumber 1304: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1212 untuk asset_code=’risk_mobil_tabrakan’.
-- Uraian sumber 1305: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_mobil_tabrakan’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1306: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_mobil_tabrakan’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1307: Menutup rekaman seed asset_code=’risk_mobil_tabrakan’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1308: Membuka rekaman seed asset_code=’risk_pemadaman_listrik’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1309: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_pemadaman_listrik’.
-- Uraian sumber 1310: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_pemadaman_listrik’.
-- Uraian sumber 1311: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_pemadaman_listrik’ untuk asset_code=’risk_pemadaman_listrik’.
-- Uraian sumber 1312: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Pemadaman Listrik’ untuk asset_code=’risk_pemadaman_listrik’.
-- Uraian sumber 1313: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1213 untuk asset_code=’risk_pemadaman_listrik’.
-- Uraian sumber 1314: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_pemadaman_listrik’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1315: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_pemadaman_listrik’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1316: Menutup rekaman seed asset_code=’risk_pemadaman_listrik’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1317: Membuka rekaman seed asset_code=’risk_ganti_aki’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1318: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_ganti_aki’.
-- Uraian sumber 1319: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_ganti_aki’.
-- Uraian sumber 1320: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_ganti_aki’ untuk asset_code=’risk_ganti_aki’.
-- Uraian sumber 1321: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Ganti Aki’ untuk asset_code=’risk_ganti_aki’.
-- Uraian sumber 1322: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1214 untuk asset_code=’risk_ganti_aki’.
-- Uraian sumber 1323: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_ganti_aki’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1324: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_ganti_aki’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1325: Menutup rekaman seed asset_code=’risk_ganti_aki’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1326: Membuka rekaman seed asset_code=’risk_bbm_naik’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1327: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_bbm_naik’.
-- Uraian sumber 1328: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_bbm_naik’.
-- Uraian sumber 1329: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_bbm_naik’ untuk asset_code=’risk_bbm_naik’.
-- Uraian sumber 1330: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’BBM Naik’ untuk asset_code=’risk_bbm_naik’.
-- Uraian sumber 1331: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1215 untuk asset_code=’risk_bbm_naik’.
-- Uraian sumber 1332: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_bbm_naik’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1333: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_bbm_naik’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1334: Menutup rekaman seed asset_code=’risk_bbm_naik’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1335: Membuka rekaman seed asset_code=’risk_kecelakaan’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1336: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_kecelakaan’.
-- Uraian sumber 1337: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_kecelakaan’.
-- Uraian sumber 1338: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_kecelakaan’ untuk asset_code=’risk_kecelakaan’.
-- Uraian sumber 1339: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Kecelakaan’ untuk asset_code=’risk_kecelakaan’.
-- Uraian sumber 1340: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1216 untuk asset_code=’risk_kecelakaan’.
-- Uraian sumber 1341: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_kecelakaan’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1342: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_kecelakaan’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1343: Menutup rekaman seed asset_code=’risk_kecelakaan’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1344: Membuka rekaman seed asset_code=’risk_sakit_perut’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1345: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_sakit_perut’.
-- Uraian sumber 1346: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_sakit_perut’.
-- Uraian sumber 1347: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_sakit_perut’ untuk asset_code=’risk_sakit_perut’.
-- Uraian sumber 1348: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Sakit Perut’ untuk asset_code=’risk_sakit_perut’.
-- Uraian sumber 1349: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1217 untuk asset_code=’risk_sakit_perut’.
-- Uraian sumber 1350: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_sakit_perut’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1351: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_sakit_perut’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1352: Menutup rekaman seed asset_code=’risk_sakit_perut’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1353: Membuka rekaman seed asset_code=’risk_sakit_asam_lambung’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1354: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_sakit_asam_lambung’.
-- Uraian sumber 1355: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_sakit_asam_lambung’.
-- Uraian sumber 1356: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_sakit_asam_lambung’ untuk asset_code=’risk_sakit_asam_lambung’.
-- Uraian sumber 1357: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Sakit Asam Lambung’ untuk asset_code=’risk_sakit_asam_lambung’.
-- Uraian sumber 1358: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1218 untuk asset_code=’risk_sakit_asam_lambung’.
-- Uraian sumber 1359: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_sakit_asam_lambung’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1360: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_sakit_asam_lambung’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1361: Menutup rekaman seed asset_code=’risk_sakit_asam_lambung’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1362: Membuka rekaman seed asset_code=’risk_wisuda_kelulusan’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1363: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_wisuda_kelulusan’.
-- Uraian sumber 1364: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_wisuda_kelulusan’.
-- Uraian sumber 1365: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_wisuda_kelulusan’ untuk asset_code=’risk_wisuda_kelulusan’.
-- Uraian sumber 1366: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Wisuda Kelulusan’ untuk asset_code=’risk_wisuda_kelulusan’.
-- Uraian sumber 1367: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1219 untuk asset_code=’risk_wisuda_kelulusan’.
-- Uraian sumber 1368: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_wisuda_kelulusan’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1369: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_wisuda_kelulusan’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1370: Menutup rekaman seed asset_code=’risk_wisuda_kelulusan’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1371: Membuka rekaman seed asset_code=’risk_bencana_banjir’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1372: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_bencana_banjir’.
-- Uraian sumber 1373: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_bencana_banjir’.
-- Uraian sumber 1374: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_bencana_banjir’ untuk asset_code=’risk_bencana_banjir’.
-- Uraian sumber 1375: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Bencana Banjir’ untuk asset_code=’risk_bencana_banjir’.
-- Uraian sumber 1376: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1220 untuk asset_code=’risk_bencana_banjir’.
-- Uraian sumber 1377: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_bencana_banjir’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1378: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_bencana_banjir’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1379: Menutup rekaman seed asset_code=’risk_bencana_banjir’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1380: Membuka rekaman seed asset_code=’risk_gudang_terbakar’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1381: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_gudang_terbakar’.
-- Uraian sumber 1382: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_gudang_terbakar’.
-- Uraian sumber 1383: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_gudang_terbakar’ untuk asset_code=’risk_gudang_terbakar’.
-- Uraian sumber 1384: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Gudang Terbakar’ untuk asset_code=’risk_gudang_terbakar’.
-- Uraian sumber 1385: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1221 untuk asset_code=’risk_gudang_terbakar’.
-- Uraian sumber 1386: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_gudang_terbakar’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1387: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_gudang_terbakar’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1388: Menutup rekaman seed asset_code=’risk_gudang_terbakar’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1389: Membuka rekaman seed asset_code=’risk_investasi_emas’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1390: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_investasi_emas’.
-- Uraian sumber 1391: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_investasi_emas’.
-- Uraian sumber 1392: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_investasi_emas’ untuk asset_code=’risk_investasi_emas’.
-- Uraian sumber 1393: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Investasi Emas’ untuk asset_code=’risk_investasi_emas’.
-- Uraian sumber 1394: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1222 untuk asset_code=’risk_investasi_emas’.
-- Uraian sumber 1395: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_investasi_emas’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1396: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_investasi_emas’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1397: Menutup rekaman seed asset_code=’risk_investasi_emas’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1398: Membuka rekaman seed asset_code=’risk_tahun_ajaran_baru’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1399: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_tahun_ajaran_baru’.
-- Uraian sumber 1400: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_tahun_ajaran_baru’.
-- Uraian sumber 1401: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_tahun_ajaran_baru’ untuk asset_code=’risk_tahun_ajaran_baru’.
-- Uraian sumber 1402: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Tahun Ajaran Baru’ untuk asset_code=’risk_tahun_ajaran_baru’.
-- Uraian sumber 1403: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1223 untuk asset_code=’risk_tahun_ajaran_baru’.
-- Uraian sumber 1404: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_tahun_ajaran_baru’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1405: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_tahun_ajaran_baru’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1406: Menutup rekaman seed asset_code=’risk_tahun_ajaran_baru’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 1407: Membuka rekaman seed asset_code=’risk_ulang_tahun’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1408: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’risk_ulang_tahun’.
-- Uraian sumber 1409: Mengisi asset_type (kategori aset atau kartu) dengan ’RISK’ untuk asset_code=’risk_ulang_tahun’.
-- Uraian sumber 1410: Mengisi asset_code (kode aset dalam versi aturan) dengan ’risk_ulang_tahun’ untuk asset_code=’risk_ulang_tahun’.
-- Uraian sumber 1411: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Ulang Tahun’ untuk asset_code=’risk_ulang_tahun’.
-- Uraian sumber 1412: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1224 untuk asset_code=’risk_ulang_tahun’.
-- Uraian sumber 1413: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’risk_ulang_tahun’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 1414: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”source”:”static.risk”}’ :: jsonb untuk asset_code=’risk_ulang_tahun’. Isi JSON dipakai sebagai parameter aksi/metadata: source=”static.risk” (asal definisi komponen).
-- Uraian sumber 1415: Menutup rekaman seed asset_code=’risk_ulang_tahun’; kueri melanjutkan pemrosesan kumpulan nilai yang telah lengkap.
-- Uraian sumber 1416: Mengevaluasi ekspresi ) as v( pada CTE static_assets; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil v (nilai v).
-- Uraian sumber 1417: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada CTE static_assets; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1418: Memakai asset_type (kategori aset atau kartu) sebagai unsur ekspresi atau daftar pada CTE static_assets; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1419: Memakai asset_code (kode aset dalam versi aturan) sebagai unsur ekspresi atau daftar pada CTE static_assets; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1420: Memakai display_name (nama yang ditampilkan kepada pengguna) sebagai unsur ekspresi atau daftar pada CTE static_assets; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1421: Memakai sort_order (urutan tampilan atau evaluasi komponen) sebagai unsur ekspresi atau daftar pada CTE static_assets; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1422: Memakai is_active (penanda apakah entitas masih boleh digunakan) sebagai unsur ekspresi atau daftar pada CTE static_assets; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1423: Memakai metadata_json (metadata tambahan aset dalam JSONB) sebagai unsur ekspresi atau daftar pada CTE static_assets; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1424: Menutup kelompok yang terkait ) ) as v pada CTE static_assets. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1425: Menutup kelompok yang terkait ) , static_assets as pada CTE static_assets. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1426: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1427: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1428: Memakai asset_type (kategori aset atau kartu) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1429: Memakai asset_code (kode aset dalam versi aturan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1430: Memakai display_name (nama yang ditampilkan kepada pengguna) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1431: Memakai sort_order (urutan tampilan atau evaluasi komponen) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1432: Memakai is_active (penanda apakah entitas masih boleh digunakan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1433: Memakai metadata_json (metadata tambahan aset dalam JSONB) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1434: Menetapkan sumber baris json_assets union all select distinct on ( sr.ruleset_version_id, yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1435: Memakai json_assets (nilai json assets) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1436: Menggabungkan hasil kueri dalam skrip basis data; UNION ALL mempertahankan seluruh baris termasuk duplikat, sedangkan UNION tanpa ALL menghilangkan baris hasil yang sama.
-- Uraian sumber 1437: Menggabungkan hasil kueri dalam skrip basis data; UNION ALL mempertahankan seluruh baris termasuk duplikat, sedangkan UNION tanpa ALL menghilangkan baris hasil yang sama.
-- Uraian sumber 1438: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1439: Mengevaluasi ekspresi distinct on ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1440: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1441: Memakai sa.asset_type (kategori aset atau kartu) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1442: Memakai sa.asset_code (kode aset dalam versi aturan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1443: Mengevaluasi ekspresi ) sr.ruleset_version_id, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1444: Memakai sa.asset_type (kategori aset atau kartu) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1445: Memakai sa.asset_code (kode aset dalam versi aturan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1446: Memakai sa.display_name (nama yang ditampilkan kepada pengguna) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1447: Memakai sa.sort_order (urutan tampilan atau evaluasi komponen) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1448: Memakai sa.is_active (penanda apakah entitas masih boleh digunakan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1449: Memakai sa.metadata_json (metadata tambahan aset dalam JSONB) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1450: Menetapkan sumber baris static_assets sa join seed_rulesets sr on sr.ruleset_version_id = sa.ruleset_version_id on conflict (ruleset_version_id, asset_type, asset_code) do update se... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1451: Mengevaluasi ekspresi static_assets sa pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1452: Menetapkan penanganan konflik kunci unik pada INSERT: join seed_rulesets sr on sr.ruleset_version_id = sa.ruleset_version_id on conflict (ruleset_version_id, asset_type, asset_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 1453: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 1454: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 1455: Menetapkan atau membandingkan display_name (nama yang ditampilkan kepada pengguna) terhadap excluded.display_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1456: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1457: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1458: Menetapkan atau membandingkan metadata_json (metadata tambahan aset dalam JSONB) terhadap excluded.metadata_json, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1459: Menetapkan atau membandingkan updated_at (waktu perubahan terakhir) terhadap now(); pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. NOW mengambil waktu awal transaksi PostgreSQL.
-- Uraian sumber 1461: Memulai penyisipan rekaman ke ruleset_orders, tabel yang menyimpan pesanan masakan, harga jual, poin kebahagiaan, dan jumlah kartu; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 1462: Mengevaluasi ekspresi ruleset_orders ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1463: Menempatkan ruleset_orders.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1464: Menempatkan ruleset_orders.ruleset_game_asset_id (UUID aset pada katalog induk) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1465: Menempatkan ruleset_orders.order_code (kode pesanan masakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1466: Menempatkan ruleset_orders.item_name (nama item dalam katalog) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1467: Menempatkan ruleset_orders.sell_price (harga penjualan atau pesanan dalam koin) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1468: Menempatkan ruleset_orders.happiness_points (poin kebahagiaan yang diberikan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1469: Menempatkan ruleset_orders.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1470: Menempatkan ruleset_orders.card_qty (jumlah salinan kartu fisik yang tersedia) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1471: Menempatkan ruleset_orders.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1472: Menempatkan ruleset_orders.payload_json (rincian domain tambahan dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1473: Menutup kelompok yang terkait ; insert into ruleset_orders pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1474: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1475: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1476: Memakai rga.ruleset_game_asset_id (UUID aset pada katalog induk) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1477: Mengevaluasi ekspresi item ->> ’id’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1478: Mengevaluasi ekspresi item ->> ’nama’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1479: Mengevaluasi ekspresi coalesce((item ->> ’hargaJual’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1480: Mengevaluasi ekspresi coalesce((item ->> ’poinKebahagiaan’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1481: Mengevaluasi ekspresi ord :: int, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1482: Mengevaluasi ekspresi coalesce((item ->> ’cardQty’) :: int, 1), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1483: Memakai true (nilai true) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1484: Mengevaluasi ekspresi jsonb_build_object( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 1485: Menyediakan nilai literal ’hargaJual’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1486: Mengevaluasi ekspresi coalesce((item ->> ’hargaJual’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1487: Menyediakan nilai literal ’poinKebahagiaan’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1488: Mengevaluasi ekspresi coalesce((item ->> ’poinKebahagiaan’) :: int, 0) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1489: Menutup kelompok yang terkait , true , jsonb_build_object pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1490: Menetapkan sumber baris seed_rulesets sr cross join lateral jsonb_array_elements( sr.definition_json -> ’component_catalog’ -> ’resep’ ) with ordinality as x(item, ord) join ruleset... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1491: Mengevaluasi ekspresi seed_rulesets sr pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1492: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 1493: Mengevaluasi ekspresi sr.definition_json -> ’component_catalog’ -> ’resep’ pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 1494: Mengevaluasi ekspresi ) with ordinality as x(item, ord) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil x (nilai x).
-- Uraian sumber 1495: Menggabungkan sumber join ruleset_game_assets rga on rga.ruleset_version_id = sr.ruleset_version_id dalam skrip basis data. JOIN memasangkan baris yang memenuhi syarat ON sehingga identitas aturan, sesi, atau aset tetap selaras.
-- Uraian sumber 1496: Menambahkan syarat wajib rga.asset_type = ’ORDER’ pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos.
-- Uraian sumber 1497: Menetapkan penanganan konflik kunci unik pada INSERT: and rga.asset_code = item ->> ’id’ on conflict (ruleset_version_id, order_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1498: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 1499: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 1500: Menetapkan atau membandingkan item_name (nama item dalam katalog) terhadap excluded.item_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1501: Menetapkan atau membandingkan sell_price (harga penjualan atau pesanan dalam koin) terhadap excluded.sell_price, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1502: Menetapkan atau membandingkan happiness_points (poin kebahagiaan yang diberikan) terhadap excluded.happiness_points, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1503: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1504: Menetapkan atau membandingkan card_qty (jumlah salinan kartu fisik yang tersedia) terhadap excluded.card_qty, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1505: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1506: Menetapkan atau membandingkan payload_json (rincian domain tambahan dalam JSONB) terhadap excluded.payload_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1508: Memulai penyisipan rekaman ke ruleset_actions, tabel yang menghubungkan aksi aktif dengan versi aturan dan urutan tampilnya; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 1509: Mengevaluasi ekspresi ruleset_actions ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1510: Menempatkan ruleset_actions.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1511: Menempatkan ruleset_actions.action_id (kode aksi pada katalog) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1512: Menempatkan ruleset_actions.behavior_id (kode perilaku domain yang menjalankan efek aksi) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1513: Menempatkan ruleset_actions.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1514: Menempatkan ruleset_actions.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1515: Menutup kelompok yang terkait ; insert into ruleset_actions pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1516: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1517: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1518: Memakai a.action_id (kode aksi pada katalog) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1519: Memakai a.behavior_id (kode perilaku domain yang menjalankan efek aksi) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1520: Mengevaluasi ekspresi row_number() over ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. ROW_NUMBER memberi nomor urut per partisi menurut ORDER BY sehingga identitas atau urutan hasil dapat direproduksi.
-- Uraian sumber 1521: Memisahkan window skrip basis data berdasarkan sr.ruleset_version_id; penomoran, peringkat, atau agregat window dimulai ulang untuk tiap kelompok kunci tersebut.
-- Uraian sumber 1522: Mengurutkan hasil skrip basis data menurut a.action_id ) :: int, a.is_active from seed_rulesets sr join actions a on a.is_active; ASC memakai urutan naik, DESC urutan turun, dan urutan kolom menjadi prioritas pembanding.
-- Uraian sumber 1523: Memakai a.action_id (kode aksi pada katalog) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1524: Mengevaluasi ekspresi ) :: int, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1525: Memakai a.is_active (penanda apakah entitas masih boleh digunakan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1526: Menetapkan sumber baris seed_rulesets sr join actions a on a.is_active and a.mode in (’BOTH’, sr.mode) on conflict (ruleset_version_id, action_id) do update set sort_order = exclude... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1527: Mengevaluasi ekspresi seed_rulesets sr pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1528: Menggabungkan sumber join actions a on a.is_active dalam skrip basis data. JOIN memasangkan baris yang memenuhi syarat ON sehingga identitas aturan, sesi, atau aset tetap selaras.
-- Uraian sumber 1529: Menetapkan penanganan konflik kunci unik pada INSERT: and a.mode in (’BOTH’, sr.mode) on conflict (ruleset_version_id, action_id) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 1530: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 1531: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 1532: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1533: Menetapkan atau membandingkan behavior_id (kode perilaku domain yang menjalankan efek aksi) terhadap excluded.behavior_id, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1534: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1536: Memulai penyisipan rekaman ke ruleset_ingredients, tabel yang menyimpan katalog bahan masakan, harga beli, serta jumlah kartu fisik; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 1537: Mengevaluasi ekspresi ruleset_ingredients ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1538: Menempatkan ruleset_ingredients.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1539: Menempatkan ruleset_ingredients.ruleset_game_asset_id (UUID aset pada katalog induk) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1540: Menempatkan ruleset_ingredients.ingredient_code (kode bahan masakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1541: Menempatkan ruleset_ingredients.item_name (nama item dalam katalog) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1542: Menempatkan ruleset_ingredients.display_name (nama yang ditampilkan kepada pengguna) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1543: Menempatkan ruleset_ingredients.purchase_price (harga beli dalam koin) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1544: Menempatkan ruleset_ingredients.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1545: Menempatkan ruleset_ingredients.card_qty (jumlah salinan kartu fisik yang tersedia) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1546: Menempatkan ruleset_ingredients.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1547: Menempatkan ruleset_ingredients.payload_json (rincian domain tambahan dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1548: Menutup kelompok yang terkait ; insert into ruleset_ingredients pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1549: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1550: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1551: Memakai rga.ruleset_game_asset_id (UUID aset pada katalog induk) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1552: Mengevaluasi ekspresi coalesce( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 1553: Mengevaluasi ekspresi item ->> ’id’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1554: Mengevaluasi ekspresi regexp_replace(lower(item ->> ’nama’), ’\s+’, ’’, ’g’) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; LOWER menormalkan huruf untuk pencocokan kode/nama tanpa perbedaan kapital.
-- Uraian sumber 1555: Menutup kelompok yang terkait . ruleset_game_asset_id , coalesce pada skrip basis data. Koma memisahkan unsur ini dari unsur berikutnya. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1556: Mengevaluasi ekspresi item ->> ’nama’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1557: Mengevaluasi ekspresi item ->> ’nama’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1558: Mengevaluasi ekspresi coalesce((item ->> ’hargaBeli’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1559: Mengevaluasi ekspresi ord :: int, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1560: Mengevaluasi ekspresi coalesce((item ->> ’cardQty’) :: int, 5), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1561: Memakai true (nilai true) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1562: Mengevaluasi ekspresi jsonb_build_object( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 1563: Menyediakan nilai literal ’asset_code’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1564: Mengevaluasi ekspresi coalesce( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 1565: Mengevaluasi ekspresi item ->> ’id’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1566: Mengevaluasi ekspresi regexp_replace(lower(item ->> ’nama’), ’\s+’, ’’, ’g’) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; LOWER menormalkan huruf untuk pencocokan kode/nama tanpa perbedaan kapital.
-- Uraian sumber 1567: Menutup kelompok yang terkait ( ’asset_code’ , coalesce pada skrip basis data. Koma memisahkan unsur ini dari unsur berikutnya. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1568: Menyediakan nilai literal ’display_name’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1569: Mengevaluasi ekspresi item ->> ’nama’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1570: Menyediakan nilai literal ’hargaBeli’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1571: Mengevaluasi ekspresi coalesce((item ->> ’hargaBeli’) :: int, 0) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1572: Menutup kelompok yang terkait , true , jsonb_build_object pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1573: Menetapkan sumber baris seed_rulesets sr cross join lateral jsonb_array_elements( sr.definition_json -> ’component_catalog’ -> ’bahan’ ) with ordinality as x(item, ord) join ruleset... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1574: Mengevaluasi ekspresi seed_rulesets sr pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1575: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 1576: Mengevaluasi ekspresi sr.definition_json -> ’component_catalog’ -> ’bahan’ pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 1577: Mengevaluasi ekspresi ) with ordinality as x(item, ord) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil x (nilai x).
-- Uraian sumber 1578: Menggabungkan sumber join ruleset_game_assets rga on rga.ruleset_version_id = sr.ruleset_version_id dalam skrip basis data. JOIN memasangkan baris yang memenuhi syarat ON sehingga identitas aturan, sesi, atau aset tetap selaras.
-- Uraian sumber 1579: Menambahkan syarat wajib rga.asset_type = ’INGREDIENT’ pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos.
-- Uraian sumber 1580: Menambahkan syarat wajib rga.asset_code = coalesce( pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 1581: Mengevaluasi ekspresi item ->> ’id’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1582: Mengevaluasi ekspresi regexp_replace(lower(item ->> ’nama’), ’\s+’, ’_’, ’g’) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; LOWER menormalkan huruf untuk pencocokan kode/nama tanpa perbedaan kapital.
-- Uraian sumber 1583: Menetapkan penanganan konflik kunci unik pada INSERT: ) on conflict (ruleset_version_id, ingredient_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 1584: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 1585: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 1586: Menetapkan atau membandingkan item_name (nama item dalam katalog) terhadap excluded.item_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1587: Menetapkan atau membandingkan display_name (nama yang ditampilkan kepada pengguna) terhadap excluded.display_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1588: Menetapkan atau membandingkan purchase_price (harga beli dalam koin) terhadap excluded.purchase_price, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1589: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1590: Menetapkan atau membandingkan card_qty (jumlah salinan kartu fisik yang tersedia) terhadap excluded.card_qty, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1591: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1592: Menetapkan atau membandingkan payload_json (rincian domain tambahan dalam JSONB) terhadap excluded.payload_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1594: Memulai penyisipan rekaman ke ruleset_order_requirements, tabel yang merinci bahan dan jumlah yang harus dipenuhi untuk menjual satu pesanan; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 1595: Mengevaluasi ekspresi ruleset_order_requirements ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1596: Menempatkan ruleset_order_requirements.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1597: Menempatkan ruleset_order_requirements.ruleset_order_id (identitas relasi ruleset order) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1598: Menempatkan ruleset_order_requirements.requirement_order (urutan syarat dalam komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1599: Menempatkan ruleset_order_requirements.required_asset_id (UUID aset yang diwajibkan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1600: Menempatkan ruleset_order_requirements.qty_required (jumlah unit yang diwajibkan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1601: Menempatkan ruleset_order_requirements.payload_json (rincian domain tambahan dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1602: Menutup kelompok yang terkait ; insert into ruleset_order_requirements pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1603: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1604: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1605: Memakai ro.ruleset_order_id (identitas relasi ruleset order) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1606: Mengevaluasi ekspresi row_number() over ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. ROW_NUMBER memberi nomor urut per partisi menurut ORDER BY sehingga identitas atau urutan hasil dapat direproduksi.
-- Uraian sumber 1607: Memisahkan window skrip basis data berdasarkan sr.ruleset_version_id,; penomoran, peringkat, atau agregat window dimulai ulang untuk tiap kelompok kunci tersebut.
-- Uraian sumber 1608: Mengevaluasi ekspresi recipe.recipe ->> ’id’ pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1609: Mengurutkan hasil skrip basis data menurut req.ingredient_name ) :: int, ri.ruleset_game_asset_id, req.ingredient_qty, jsonb_build_object(’ingredient_name’, req.ingredient_name) from; ASC memakai urutan naik, DESC urutan turun, dan urutan kolom menjadi prioritas pembanding.
-- Uraian sumber 1610: Memakai req.ingredient_name (nama bahan untuk pencocokan katalog) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1611: Mengevaluasi ekspresi ) :: int, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1612: Memakai ri.ruleset_game_asset_id (UUID aset pada katalog induk) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1613: Memakai req.ingredient_qty (nilai ingredient qty) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1614: Mengevaluasi ekspresi jsonb_build_object(’ingredient_name’, req.ingredient_name) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 1615: Menetapkan sumber baris seed_rulesets sr cross join lateral jsonb_array_elements( sr.definition_json -> ’component_catalog’ -> ’resep’ ) as recipe(recipe) cross join lateral ( select yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1616: Mengevaluasi ekspresi seed_rulesets sr pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1617: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 1618: Mengevaluasi ekspresi sr.definition_json -> ’component_catalog’ -> ’resep’ pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 1619: Mengevaluasi ekspresi ) as recipe(recipe) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil recipe (nilai recipe).
-- Uraian sumber 1620: Menggabungkan sumber cross join lateral ( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog.
-- Uraian sumber 1621: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1622: Memakai ingredient_name (nama bahan untuk pencocokan katalog) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1623: Mengevaluasi ekspresi count(*) :: int as ingredient_qty pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil ingredient_qty (nilai ingredient qty); operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai; COUNT menghitung baris atau nilai non-NULL untuk validasi jumlah dan statistik.
-- Uraian sumber 1624: Menetapkan sumber baris jsonb_array_elements_text(recipe.recipe -> ’bahan’) as z(ingredient_name) group by ingredient_name ) req join ruleset_orders ro on ro.ruleset_version_id = sr... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1625: Mengevaluasi ekspresi jsonb_array_elements_text(recipe.recipe -> ’bahan’) as z(ingredient_name) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil z (nilai z); jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional; operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 1626: Mengelompokkan baris skrip basis data menurut ingredient_name ) req join ruleset_orders ro on ro.ruleset_version_id = sr.ruleset_version_id and ro.order_code = recipe.recipe ->> ’id’ join ruleset_ingredi... sebelum agregat dihitung, sehingga hasil terpisah untuk setiap kombinasi kunci kelompok.
-- Uraian sumber 1627: Memakai ingredient_name (nama bahan untuk pencocokan katalog) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1628: Mengevaluasi ekspresi ) req pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1629: Menggabungkan sumber join ruleset_orders ro on ro.ruleset_version_id = sr.ruleset_version_id dalam skrip basis data. JOIN memasangkan baris yang memenuhi syarat ON sehingga identitas aturan, sesi, atau aset tetap selaras.
-- Uraian sumber 1630: Menambahkan syarat wajib ro.order_code = recipe.recipe ->> ’id’ pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1631: Menggabungkan sumber join ruleset_ingredients ri on ri.ruleset_version_id = sr.ruleset_version_id dalam skrip basis data. JOIN memasangkan baris yang memenuhi syarat ON sehingga identitas aturan, sesi, atau aset tetap selaras.
-- Uraian sumber 1632: Menetapkan penanganan konflik kunci unik pada INSERT: and lower(ri.display_name) = lower(req.ingredient_name) on conflict (. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed. LOWER menormalkan huruf untuk pencocokan kode/nama tanpa perbedaan kapital.
-- Uraian sumber 1633: Memakai ruleset_order_id (identitas relasi ruleset order) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1634: Memakai requirement_order (urutan syarat dalam komponen) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1635: Memakai required_asset_id (UUID aset yang diwajibkan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1636: Mengevaluasi ekspresi ) do pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1637: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 1638: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 1639: Menetapkan atau membandingkan qty_required (jumlah unit yang diwajibkan) terhadap excluded.qty_required, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1640: Menetapkan atau membandingkan payload_json (rincian domain tambahan dalam JSONB) terhadap excluded.payload_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1642: Memulai penyisipan rekaman ke ruleset_needs, tabel yang menyimpan kartu kebutuhan primer, sekunder, dan tersier beserta harga dan poinnya; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 1643: Mengevaluasi ekspresi ruleset_needs ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1644: Menempatkan ruleset_needs.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1645: Menempatkan ruleset_needs.ruleset_game_asset_id (UUID aset pada katalog induk) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1646: Menempatkan ruleset_needs.need_code (kode kartu kebutuhan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1647: Menempatkan ruleset_needs.item_name (nama item dalam katalog) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1648: Menempatkan ruleset_needs.need_tier (tingkat kebutuhan primer, sekunder, atau tersier) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1649: Menempatkan ruleset_needs.purchase_price (harga beli dalam koin) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1650: Menempatkan ruleset_needs.happiness_points (poin kebahagiaan yang diberikan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1651: Menempatkan ruleset_needs.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1652: Menempatkan ruleset_needs.card_qty (jumlah salinan kartu fisik yang tersedia) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1653: Menempatkan ruleset_needs.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1654: Menempatkan ruleset_needs.need_family_code (kelompok jenis kebutuhan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1655: Menempatkan ruleset_needs.payload_json (rincian domain tambahan dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1656: Menutup kelompok yang terkait ; insert into ruleset_needs pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1657: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1658: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1659: Memakai rga.ruleset_game_asset_id (UUID aset pada katalog induk) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1660: Mengevaluasi ekspresi item ->> ’id’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1661: Mengevaluasi ekspresi item ->> ’nama’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1662: Mengevaluasi ekspresi coalesce(item ->> ’tipe’, ’primer’), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1663: Mengevaluasi ekspresi coalesce((item ->> ’hargaBeli’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1664: Mengevaluasi ekspresi coalesce((item ->> ’poinKebahagiaan’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1665: Mengevaluasi ekspresi ord :: int, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1666: Mengevaluasi ekspresi coalesce((item ->> ’cardQty’) :: int, 1), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1667: Memakai true (nilai true) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1668: Mengevaluasi ekspresi coalesce( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 1669: Mengevaluasi ekspresi nullif(item ->> ’family’, ’’), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. NULLIF mengubah dua nilai yang sama menjadi NULL sebelum pemeriksaan atau konversi lanjutan; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1670: Mengevaluasi ekspresi regexp_replace(item ->> ’id’, ’_[0-9]+$’, ’’) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1671: Menutup kelompok yang terkait , true , coalesce pada skrip basis data. Koma memisahkan unsur ini dari unsur berikutnya. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1672: Mengevaluasi ekspresi jsonb_build_object( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 1673: Menyediakan nilai literal ’tipe’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1674: Mengevaluasi ekspresi coalesce(item ->> ’tipe’, ’primer’), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1675: Menyediakan nilai literal ’family’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1676: Mengevaluasi ekspresi coalesce( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 1677: Mengevaluasi ekspresi nullif(item ->> ’family’, ’’), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. NULLIF mengubah dua nilai yang sama menjadi NULL sebelum pemeriksaan atau konversi lanjutan; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1678: Mengevaluasi ekspresi regexp_replace(item ->> ’id’, ’_[0-9]+$’, ’’) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1679: Menutup kelompok yang terkait , ’family’ , coalesce pada skrip basis data. Koma memisahkan unsur ini dari unsur berikutnya. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1680: Menyediakan nilai literal ’hargaBeli’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1681: Mengevaluasi ekspresi coalesce((item ->> ’hargaBeli’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1682: Menyediakan nilai literal ’poinKebahagiaan’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1683: Mengevaluasi ekspresi coalesce((item ->> ’poinKebahagiaan’) :: int, 0) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1684: Menutup kelompok yang terkait ) ) , jsonb_build_object pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1685: Menetapkan sumber baris seed_rulesets sr cross join lateral jsonb_array_elements( sr.definition_json -> ’component_catalog’ -> ’kebutuhan’ ) with ordinality as x(item, ord) join rul... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1686: Mengevaluasi ekspresi seed_rulesets sr pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1687: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 1688: Mengevaluasi ekspresi sr.definition_json -> ’component_catalog’ -> ’kebutuhan’ pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 1689: Mengevaluasi ekspresi ) with ordinality as x(item, ord) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil x (nilai x).
-- Uraian sumber 1690: Menggabungkan sumber join ruleset_game_assets rga on rga.ruleset_version_id = sr.ruleset_version_id dalam skrip basis data. JOIN memasangkan baris yang memenuhi syarat ON sehingga identitas aturan, sesi, atau aset tetap selaras.
-- Uraian sumber 1691: Menambahkan syarat wajib rga.asset_type = ’NEED’ pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos.
-- Uraian sumber 1692: Menetapkan penanganan konflik kunci unik pada INSERT: and rga.asset_code = item ->> ’id’ on conflict (ruleset_version_id, need_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1693: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 1694: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 1695: Menetapkan atau membandingkan item_name (nama item dalam katalog) terhadap excluded.item_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1696: Menetapkan atau membandingkan need_tier (tingkat kebutuhan primer, sekunder, atau tersier) terhadap excluded.need_tier, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1697: Menetapkan atau membandingkan purchase_price (harga beli dalam koin) terhadap excluded.purchase_price, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1698: Menetapkan atau membandingkan happiness_points (poin kebahagiaan yang diberikan) terhadap excluded.happiness_points, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1699: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1700: Menetapkan atau membandingkan card_qty (jumlah salinan kartu fisik yang tersedia) terhadap excluded.card_qty, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1701: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1702: Menetapkan atau membandingkan need_family_code (kelompok jenis kebutuhan) terhadap excluded.need_family_code, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1703: Menetapkan atau membandingkan payload_json (rincian domain tambahan dalam JSONB) terhadap excluded.payload_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1705: Memulai penyisipan rekaman ke ruleset_need_set_bonuses, tabel yang menyimpan bonus pencapaian pola kombinasi kebutuhan; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 1706: Mengevaluasi ekspresi ruleset_need_set_bonuses ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1707: Menempatkan ruleset_need_set_bonuses.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1708: Menempatkan ruleset_need_set_bonuses.pattern_code (kode pola kombinasi kebutuhan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1709: Menempatkan ruleset_need_set_bonuses.required_count (jumlah required) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1710: Menempatkan ruleset_need_set_bonuses.points (nilai poin) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1711: Menempatkan ruleset_need_set_bonuses.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1712: Menempatkan ruleset_need_set_bonuses.payload_json (rincian domain tambahan dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1713: Menutup kelompok yang terkait ; insert into ruleset_need_set_bonuses pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1714: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1715: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1716: Memakai bonus.pattern_code (kode pola kombinasi kebutuhan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1717: Memakai bonus.required_count (jumlah required) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1718: Memakai bonus.points (nilai poin) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1719: Memakai bonus.sort_order (urutan tampilan atau evaluasi komponen) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1720: Mengevaluasi ekspresi jsonb_build_object( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 1721: Menyediakan nilai literal ’pattern’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1722: Memakai bonus.pattern_code (kode pola kombinasi kebutuhan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1723: Menyediakan nilai literal ’required_count’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1724: Memakai bonus.required_count (jumlah required) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1725: Menutup kelompok yang terkait . sort_order , jsonb_build_object pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1726: Menetapkan sumber baris seed_rulesets sr cross join ( values (’THREE_DIFFERENT’ :: varchar(40), 3, 4, 1), (’THREE_SAME’ :: varchar(40), 3, 2, 2) ) as bonus(pattern_code, required_co... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1727: Mengevaluasi ekspresi seed_rulesets sr pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1728: Menggabungkan sumber cross join ( dalam skrip basis data. CROSS JOIN membentuk semua kombinasi antarbaris sumber.
-- Uraian sumber 1729: Menyediakan 2 tuple nilai eksplisit untuk bonus; setiap tuple membentuk satu rekaman dan mengikuti urutan pattern_code, required_count, points, sort_order.
-- Uraian sumber 1730: Membuka rekaman seed bonus, rekaman ke-1; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1731: Membuka rekaman seed bonus, rekaman ke-2; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 1732: Menetapkan penanganan konflik kunci unik pada INSERT: ) as bonus(pattern_code, required_count, points, sort_order) on conflict (ruleset_version_id, pattern_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed. memberi nama hasil bonus (nilai bonus).
-- Uraian sumber 1733: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 1734: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 1735: Menetapkan atau membandingkan required_count (jumlah required) terhadap excluded.required_count, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1736: Menetapkan atau membandingkan points (nilai poin) terhadap excluded.points, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1737: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1738: Menetapkan atau membandingkan payload_json (rincian domain tambahan dalam JSONB) terhadap excluded.payload_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1740: Memulai penyisipan rekaman ke ruleset_collection_missions, tabel yang menyimpan misi koleksi kebutuhan serta poin keberhasilan atau kegagalannya; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 1741: Mengevaluasi ekspresi ruleset_collection_missions ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1742: Menempatkan ruleset_collection_missions.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1743: Menempatkan ruleset_collection_missions.mission_code (kode misi koleksi) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1744: Menempatkan ruleset_collection_missions.item_name (nama item dalam katalog) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1745: Menempatkan ruleset_collection_missions.success_points (poin ketika syarat berhasil dipenuhi) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1746: Menempatkan ruleset_collection_missions.failure_points (poin ketika syarat gagal dipenuhi) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1747: Menempatkan ruleset_collection_missions.penalty_points (pengurang poin karena kewajiban atau misi gagal) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1748: Menempatkan ruleset_collection_missions.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1749: Menempatkan ruleset_collection_missions.card_qty (jumlah salinan kartu fisik yang tersedia) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1750: Menempatkan ruleset_collection_missions.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1751: Menempatkan ruleset_collection_missions.payload_json (rincian domain tambahan dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1752: Menutup kelompok yang terkait ; insert into ruleset_collection_missions pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1753: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1754: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1755: Mengevaluasi ekspresi item ->> ’id’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1756: Mengevaluasi ekspresi item ->> ’nama’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1757: Mengevaluasi ekspresi coalesce((item ->> ’success_points’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1758: Mengevaluasi ekspresi coalesce((item ->> ’failure_points’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1759: Mengevaluasi ekspresi coalesce((item ->> ’penaltyPoints’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1760: Mengevaluasi ekspresi ord :: int, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1761: Memakai 1 (nilai 1) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1762: Memakai true (nilai true) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1763: Mengevaluasi ekspresi jsonb_build_object( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 1764: Menyediakan nilai literal ’success_points’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1765: Mengevaluasi ekspresi coalesce((item ->> ’success_points’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1766: Menyediakan nilai literal ’failure_points’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1767: Mengevaluasi ekspresi coalesce((item ->> ’failure_points’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1768: Menyediakan nilai literal ’penaltyPoints’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1769: Mengevaluasi ekspresi coalesce((item ->> ’penaltyPoints’) :: int, 0) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1770: Menutup kelompok yang terkait , true , jsonb_build_object pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1771: Menetapkan sumber baris seed_rulesets sr cross join lateral jsonb_array_elements( sr.definition_json -> ’component_catalog’ -> ’targetKebutuhan’ ) with ordinality as x(item, ord) on... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1772: Mengevaluasi ekspresi seed_rulesets sr pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1773: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 1774: Mengevaluasi ekspresi sr.definition_json -> ’component_catalog’ -> ’targetKebutuhan’ pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 1775: Menetapkan penanganan konflik kunci unik pada INSERT: ) with ordinality as x(item, ord) on conflict (ruleset_version_id, mission_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed. memberi nama hasil x (nilai x).
-- Uraian sumber 1776: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 1777: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 1778: Menetapkan atau membandingkan item_name (nama item dalam katalog) terhadap excluded.item_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1779: Menetapkan atau membandingkan success_points (poin ketika syarat berhasil dipenuhi) terhadap excluded.success_points, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1780: Menetapkan atau membandingkan failure_points (poin ketika syarat gagal dipenuhi) terhadap excluded.failure_points, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1781: Menetapkan atau membandingkan penalty_points (pengurang poin karena kewajiban atau misi gagal) terhadap excluded.penalty_points, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1782: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1783: Menetapkan atau membandingkan card_qty (jumlah salinan kartu fisik yang tersedia) terhadap excluded.card_qty, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1784: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1785: Menetapkan atau membandingkan payload_json (rincian domain tambahan dalam JSONB) terhadap excluded.payload_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1787: Memulai penyisipan rekaman ke ruleset_collection_mission_requirements, tabel yang merinci syarat aset, tingkat kebutuhan, atau keluarga kebutuhan untuk misi koleksi; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 1788: Mengevaluasi ekspresi ruleset_collection_mission_requirements ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1789: Menempatkan ruleset_collection_mission_requirements.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1790: Menempatkan ruleset_collection_mission_requirements.ruleset_collection_mission_id (identitas relasi ruleset collection mission) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1791: Menempatkan ruleset_collection_mission_requirements.requirement_order (urutan syarat dalam komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1792: Menempatkan ruleset_collection_mission_requirements.requirement_type (jenis pemeriksaan syarat) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1793: Menempatkan ruleset_collection_mission_requirements.required_asset_id (UUID aset yang diwajibkan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1794: Menempatkan ruleset_collection_mission_requirements.required_need_tier (tingkat kebutuhan yang disyaratkan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1795: Menempatkan ruleset_collection_mission_requirements.required_need_family_code (keluarga kebutuhan yang disyaratkan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1796: Menempatkan ruleset_collection_mission_requirements.qty_required (jumlah unit yang diwajibkan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1797: Menempatkan ruleset_collection_mission_requirements.payload_json (rincian domain tambahan dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1798: Menutup kelompok yang terkait ; insert into ruleset_collection_mission_requirements pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1799: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1800: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1801: Memakai rcm.ruleset_collection_mission_id (identitas relasi ruleset collection mission) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1802: Mengevaluasi ekspresi coalesce((rule ->> ’order’) :: int, ord :: int), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1803: Membuka pemilihan nilai CASE pada skrip basis data; cabang WHEN pertama yang cocok menentukan hasil, kemudian ELSE memberi nilai cadangan bila disediakan.
-- Uraian sumber 1804: Menetapkan cabang CASE when upper(rule ->> ’type’) in (’TIER’, ’NEED_TIER’) then ’NEED_TIER’; syarat atau nilai setelah WHEN memilih hasil setelah THEN pada skrip basis data. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1805: Menetapkan cabang CASE when upper(rule ->> ’type’) in (’FAMILY’, ’NEED_FAMILY’) then ’NEED_FAMILY’; syarat atau nilai setelah WHEN memilih hasil setelah THEN pada skrip basis data. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1806: Menggunakan jalur atau nilai cadangan ’ASSET’ ketika cabang sebelumnya tidak cocok dalam skrip basis data.
-- Uraian sumber 1807: Menutup ekspresi CASE atau blok prosedural pada skrip basis data; akhiran AS, koma, atau titik koma menentukan apakah hasil diberi nama, dipakai dalam daftar, atau pernyataan diakhiri.
-- Uraian sumber 1808: Membuka pemilihan nilai CASE pada skrip basis data; cabang WHEN pertama yang cocok menentukan hasil, kemudian ELSE memberi nilai cadangan bila disediakan.
-- Uraian sumber 1809: Menetapkan cabang CASE when upper(rule ->> ’type’) in (’TIER’, ’NEED_TIER’, ’FAMILY’, ’NEED_FAMILY’) then null; syarat atau nilai setelah WHEN memilih hasil setelah THEN pada skrip basis data. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1810: Menggunakan jalur atau nilai cadangan required_asset.ruleset_game_asset_id ketika cabang sebelumnya tidak cocok dalam skrip basis data.
-- Uraian sumber 1811: Menutup ekspresi CASE atau blok prosedural pada skrip basis data; akhiran AS, koma, atau titik koma menentukan apakah hasil diberi nama, dipakai dalam daftar, atau pernyataan diakhiri.
-- Uraian sumber 1812: Membuka pemilihan nilai CASE pada skrip basis data; cabang WHEN pertama yang cocok menentukan hasil, kemudian ELSE memberi nilai cadangan bila disediakan.
-- Uraian sumber 1813: Menetapkan cabang CASE when upper(rule ->> ’type’) in (’TIER’, ’NEED_TIER’) then lower(rule ->> ’value’); syarat atau nilai setelah WHEN memilih hasil setelah THEN pada skrip basis data. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; LOWER menormalkan huruf untuk pencocokan kode/nama tanpa perbedaan kapital.
-- Uraian sumber 1814: Menggunakan jalur atau nilai cadangan null ketika cabang sebelumnya tidak cocok dalam skrip basis data.
-- Uraian sumber 1815: Menutup ekspresi CASE atau blok prosedural pada skrip basis data; akhiran AS, koma, atau titik koma menentukan apakah hasil diberi nama, dipakai dalam daftar, atau pernyataan diakhiri.
-- Uraian sumber 1816: Membuka pemilihan nilai CASE pada skrip basis data; cabang WHEN pertama yang cocok menentukan hasil, kemudian ELSE memberi nilai cadangan bila disediakan.
-- Uraian sumber 1817: Menetapkan cabang CASE when upper(rule ->> ’type’) in (’FAMILY’, ’NEED_FAMILY’) then lower(rule ->> ’value’); syarat atau nilai setelah WHEN memilih hasil setelah THEN pada skrip basis data. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; LOWER menormalkan huruf untuk pencocokan kode/nama tanpa perbedaan kapital.
-- Uraian sumber 1818: Menggunakan jalur atau nilai cadangan null ketika cabang sebelumnya tidak cocok dalam skrip basis data.
-- Uraian sumber 1819: Menutup ekspresi CASE atau blok prosedural pada skrip basis data; akhiran AS, koma, atau titik koma menentukan apakah hasil diberi nama, dipakai dalam daftar, atau pernyataan diakhiri.
-- Uraian sumber 1820: Memakai null (nilai null) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1821: Mengevaluasi ekspresi jsonb_build_object( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 1822: Menyediakan nilai literal ’type’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1823: Mengevaluasi ekspresi rule ->> ’type’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1824: Menyediakan nilai literal ’value’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1825: Mengevaluasi ekspresi rule ->> ’value’ pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1826: Menutup kelompok yang terkait , null , jsonb_build_object pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1827: Menetapkan sumber baris seed_rulesets sr cross join lateral jsonb_array_elements( sr.definition_json -> ’component_catalog’ -> ’targetKebutuhan’ ) as mission(mission) cross join lat... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1828: Mengevaluasi ekspresi seed_rulesets sr pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1829: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 1830: Mengevaluasi ekspresi sr.definition_json -> ’component_catalog’ -> ’targetKebutuhan’ pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 1831: Mengevaluasi ekspresi ) as mission(mission) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil mission (nilai mission).
-- Uraian sumber 1832: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 1833: Mengevaluasi ekspresi coalesce( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 1834: Mengevaluasi ekspresi mission.mission -> ’kebutuhanTarget’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 1835: Menyediakan parameter JSONB bagi skrip basis data: Menutup susunan objek atau daftar JSON yang sedang dibentuk; pemisah koma mempertahankan batas antaranggota dokumen konfigurasi. Cast ::jsonb memvalidasi struktur JSON sebelum nilai dipakai aturan atau event.
-- Uraian sumber 1836: Menutup kelompok yang terkait lateral jsonb_array_elements ( coalesce pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1837: Mengevaluasi ekspresi ) with ordinality as x(rule, ord) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil x (nilai x).
-- Uraian sumber 1838: Menggabungkan sumber join ruleset_collection_missions rcm on rcm.ruleset_version_id = sr.ruleset_version_id dalam skrip basis data. JOIN memasangkan baris yang memenuhi syarat ON sehingga identitas aturan, sesi, atau aset tetap selaras.
-- Uraian sumber 1839: Menambahkan syarat wajib rcm.mission_code = mission.mission ->> ’id’ pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1840: Menggabungkan sumber left join ruleset_game_assets required_asset on required_asset.ruleset_version_id = sr.ruleset_version_id dalam skrip basis data. LEFT JOIN mempertahankan baris kiri walau pasangan tidak ada; kolom pasangan menjadi NULL.
-- Uraian sumber 1841: Menambahkan syarat wajib required_asset.asset_type = ’NEED’ pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos.
-- Uraian sumber 1842: Menambahkan syarat wajib upper(rule ->> ’type’) not in (’TIER’, ’NEED_TIER’, ’FAMILY’, ’NEED_FAMILY’) pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1843: Menambahkan syarat wajib ( pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos.
-- Uraian sumber 1844: Mengevaluasi ekspresi lower(required_asset.asset_code) = lower(rule ->> ’value’) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; LOWER menormalkan huruf untuk pencocokan kode/nama tanpa perbedaan kapital.
-- Uraian sumber 1845: Menambahkan alternatif lower(required_asset.display_name) = lower(rule ->> ’value’) pada skrip basis data; OR menerima keadaan ketika setidaknya satu cabang kondisi bernilai TRUE, dengan prioritas mengikuti tanda kurung. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; LOWER menormalkan huruf untuk pencocokan kode/nama tanpa perbedaan kapital.
-- Uraian sumber 1846: Menetapkan penanganan konflik kunci unik pada INSERT: ) on conflict (ruleset_collection_mission_id, requirement_order) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 1847: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 1848: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 1849: Menetapkan atau membandingkan requirement_type (jenis pemeriksaan syarat) terhadap excluded.requirement_type, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1850: Menetapkan atau membandingkan required_asset_id (UUID aset yang diwajibkan) terhadap excluded.required_asset_id, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1851: Menetapkan atau membandingkan required_need_tier (tingkat kebutuhan yang disyaratkan) terhadap excluded.required_need_tier, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1852: Menetapkan atau membandingkan required_need_family_code (keluarga kebutuhan yang disyaratkan) terhadap excluded.required_need_family_code, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1853: Menetapkan atau membandingkan qty_required (jumlah unit yang diwajibkan) terhadap excluded.qty_required, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1854: Menetapkan atau membandingkan payload_json (rincian domain tambahan dalam JSONB) terhadap excluded.payload_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1856: Memulai penyisipan rekaman ke ruleset_financial_goals, tabel yang menyimpan tujuan finansial beserta biaya dan penghargaan poinnya; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 1857: Mengevaluasi ekspresi ruleset_financial_goals ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1858: Menempatkan ruleset_financial_goals.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1859: Menempatkan ruleset_financial_goals.goal_code (kode tujuan finansial) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1860: Menempatkan ruleset_financial_goals.item_name (nama item dalam katalog) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1861: Menempatkan ruleset_financial_goals.purchase_price (harga beli dalam koin) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1862: Menempatkan ruleset_financial_goals.happiness_points (poin kebahagiaan yang diberikan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1863: Menempatkan ruleset_financial_goals.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1864: Menempatkan ruleset_financial_goals.card_qty (jumlah salinan kartu fisik yang tersedia) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1865: Menempatkan ruleset_financial_goals.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1866: Menempatkan ruleset_financial_goals.payload_json (rincian domain tambahan dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1867: Menutup kelompok yang terkait ; insert into ruleset_financial_goals pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1868: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1869: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1870: Mengevaluasi ekspresi item ->> ’id’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1871: Mengevaluasi ekspresi item ->> ’nama’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1872: Mengevaluasi ekspresi coalesce((item ->> ’hargaBeli’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1873: Mengevaluasi ekspresi coalesce((item ->> ’poinKebahagiaan’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1874: Mengevaluasi ekspresi ord :: int, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1875: Memakai 1 (nilai 1) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1876: Memakai true (nilai true) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1877: Mengevaluasi ekspresi jsonb_build_object( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 1878: Menyediakan nilai literal ’hargaBeli’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1879: Mengevaluasi ekspresi coalesce((item ->> ’hargaBeli’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1880: Menyediakan nilai literal ’poinKebahagiaan’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1881: Mengevaluasi ekspresi coalesce((item ->> ’poinKebahagiaan’) :: int, 0) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1882: Menutup kelompok yang terkait , true , jsonb_build_object pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1883: Menetapkan sumber baris seed_rulesets sr cross join lateral jsonb_array_elements( sr.definition_json -> ’component_catalog’ -> ’tujuanFinansial’ ) with ordinality as x(item, ord) wh... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1884: Mengevaluasi ekspresi seed_rulesets sr pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1885: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 1886: Mengevaluasi ekspresi sr.definition_json -> ’component_catalog’ -> ’tujuanFinansial’ pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 1887: Mengevaluasi ekspresi ) with ordinality as x(item, ord) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil x (nilai x).
-- Uraian sumber 1888: Membatasi baris skrip basis data dengan syarat sr.mode = ’MAHIR’ on conflict (ruleset_version_id, goal_code) do update set item_name = excluded.item_name, purchase_price = excluded.purchase_price, happine...; hanya baris dengan predikat TRUE masuk hasil atau terkena perubahan data.
-- Uraian sumber 1889: Menetapkan penanganan konflik kunci unik pada INSERT: sr.mode = ’MAHIR’ on conflict (ruleset_version_id, goal_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 1890: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 1891: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 1892: Menetapkan atau membandingkan item_name (nama item dalam katalog) terhadap excluded.item_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1893: Menetapkan atau membandingkan purchase_price (harga beli dalam koin) terhadap excluded.purchase_price, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1894: Menetapkan atau membandingkan happiness_points (poin kebahagiaan yang diberikan) terhadap excluded.happiness_points, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1895: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1896: Menetapkan atau membandingkan card_qty (jumlah salinan kartu fisik yang tersedia) terhadap excluded.card_qty, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1897: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1898: Menetapkan atau membandingkan payload_json (rincian domain tambahan dalam JSONB) terhadap excluded.payload_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1900: Memulai penyisipan rekaman ke ruleset_narratives, tabel yang menyimpan definisi narasi pendidikan keuangan yang dipicu aksi permainan; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 1901: Mengevaluasi ekspresi ruleset_narratives ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1902: Menempatkan ruleset_narratives.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1903: Menempatkan ruleset_narratives.narrative_code (kode narasi) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1904: Menempatkan ruleset_narratives.item_name (nama item dalam katalog) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1905: Menempatkan ruleset_narratives.repeatable (nilai repeatable) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1906: Menempatkan ruleset_narratives.cooldown_turns (nilai cooldown turns) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1907: Menempatkan ruleset_narratives.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1908: Menempatkan ruleset_narratives.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1909: Menempatkan ruleset_narratives.payload_json (rincian domain tambahan dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1910: Menutup kelompok yang terkait ; insert into ruleset_narratives pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1911: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1912: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1913: Mengevaluasi ekspresi item ->> ’id’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1914: Mengevaluasi ekspresi item ->> ’nama’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1915: Mengevaluasi ekspresi coalesce((item ->> ’repeatable’) :: boolean, false), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1916: Mengevaluasi ekspresi nullif(item ->> ’cooldownTurns’, ’’) :: int, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. NULLIF mengubah dua nilai yang sama menjadi NULL sebelum pemeriksaan atau konversi lanjutan; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1917: Mengevaluasi ekspresi ord :: int, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1918: Memakai true (nilai true) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1919: Mengevaluasi ekspresi jsonb_build_object( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 1920: Menyediakan nilai literal ’prerequisiteAksi’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1921: Mengevaluasi ekspresi coalesce(item -> ’prerequisiteAksi’, ’[]’ :: jsonb) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator -> mengambil elemen JSON sambil mempertahankan tipe JSON; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1922: Menutup kelompok yang terkait , true , jsonb_build_object pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1923: Menetapkan sumber baris seed_rulesets sr cross join lateral jsonb_array_elements( sr.definition_json -> ’component_catalog’ -> ’narasi’ ) with ordinality as x(item, ord) on conflict... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1924: Mengevaluasi ekspresi seed_rulesets sr pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1925: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 1926: Mengevaluasi ekspresi sr.definition_json -> ’component_catalog’ -> ’narasi’ pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 1927: Menetapkan penanganan konflik kunci unik pada INSERT: ) with ordinality as x(item, ord) on conflict (ruleset_version_id, narrative_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed. memberi nama hasil x (nilai x).
-- Uraian sumber 1928: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 1929: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 1930: Menetapkan atau membandingkan item_name (nama item dalam katalog) terhadap excluded.item_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1931: Menetapkan atau membandingkan repeatable (nilai repeatable) terhadap excluded.repeatable, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1932: Menetapkan atau membandingkan cooldown_turns (nilai cooldown turns) terhadap excluded.cooldown_turns, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1933: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1934: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1935: Menetapkan atau membandingkan payload_json (rincian domain tambahan dalam JSONB) terhadap excluded.payload_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1937: Memulai penyisipan rekaman ke ruleset_narrative_scenes, tabel yang menyimpan urutan adegan atau teks dari setiap narasi; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 1938: Mengevaluasi ekspresi ruleset_narrative_scenes ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1939: Menempatkan ruleset_narrative_scenes.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1940: Menempatkan ruleset_narrative_scenes.ruleset_narrative_id (identitas relasi ruleset narrative) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1941: Menempatkan ruleset_narrative_scenes.scene_code (nilai scene code) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1942: Menempatkan ruleset_narrative_scenes.scene_order (urutan adegan narasi) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1943: Menempatkan ruleset_narrative_scenes.text_lines (nilai text lines) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1944: Menempatkan ruleset_narrative_scenes.media_json (dokumen JSON media) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1945: Menempatkan ruleset_narrative_scenes.payload_json (rincian domain tambahan dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1946: Menutup kelompok yang terkait ; insert into ruleset_narrative_scenes pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1947: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1948: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1949: Memakai rn.ruleset_narrative_id (identitas relasi ruleset narrative) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1950: Mengevaluasi ekspresi concat( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1951: Mengevaluasi ekspresi narrative.narrative ->> ’id’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1952: Menyediakan nilai literal ’scene’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1953: Mengevaluasi ekspresi ord :: int pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1954: Menutup kelompok yang terkait . ruleset_narrative_id , concat pada skrip basis data. Koma memisahkan unsur ini dari unsur berikutnya. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1955: Mengevaluasi ekspresi ord :: int, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1956: Membuka pemilihan nilai CASE pada skrip basis data; cabang WHEN pertama yang cocok menentukan hasil, kemudian ELSE memberi nilai cadangan bila disediakan.
-- Uraian sumber 1957: Menetapkan cabang CASE when jsonb_typeof(narrative.narrative -> ’teks’) = ’array’ then narrative.narrative -> ’teks’; syarat atau nilai setelah WHEN memilih hasil setelah THEN pada skrip basis data. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 1958: Menetapkan cabang CASE when narrative.narrative ? ’teks’ then jsonb_build_array(narrative.narrative ->> ’teks’); syarat atau nilai setelah WHEN memilih hasil setelah THEN pada skrip basis data. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1959: Menggunakan jalur atau nilai cadangan ’[]’ :: jsonb ketika cabang sebelumnya tidak cocok dalam skrip basis data. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1960: Menutup ekspresi CASE atau blok prosedural pada skrip basis data; akhiran AS, koma, atau titik koma menentukan apakah hasil diberi nama, dipakai dalam daftar, atau pernyataan diakhiri.
-- Uraian sumber 1961: Mengevaluasi ekspresi coalesce(narrative.narrative -> ’media’, ’{}’ :: jsonb), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator -> mengambil elemen JSON sambil mempertahankan tipe JSON; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1962: Mengevaluasi ekspresi jsonb_build_object( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 1963: Menyediakan nilai literal ’source’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1964: Menyediakan nilai literal ’component_catalog.narasi’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1965: Menyediakan nilai literal ’teks’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1966: Mengevaluasi ekspresi coalesce(narrative.narrative -> ’teks’, ’[]’ :: jsonb) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator -> mengambil elemen JSON sambil mempertahankan tipe JSON; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 1967: Menutup kelompok yang terkait jsonb ) , jsonb_build_object pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1968: Menetapkan sumber baris seed_rulesets sr cross join lateral jsonb_array_elements( sr.definition_json -> ’component_catalog’ -> ’narasi’ ) with ordinality as narrative(narrative, ord... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 1969: Mengevaluasi ekspresi seed_rulesets sr pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1970: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 1971: Mengevaluasi ekspresi sr.definition_json -> ’component_catalog’ -> ’narasi’ pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 1972: Mengevaluasi ekspresi ) with ordinality as narrative(narrative, ord) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil narrative (nilai narrative).
-- Uraian sumber 1973: Menggabungkan sumber join ruleset_narratives rn on rn.ruleset_version_id = sr.ruleset_version_id dalam skrip basis data. JOIN memasangkan baris yang memenuhi syarat ON sehingga identitas aturan, sesi, atau aset tetap selaras.
-- Uraian sumber 1974: Menetapkan penanganan konflik kunci unik pada INSERT: and rn.narrative_code = narrative.narrative ->> ’id’ on conflict (ruleset_narrative_id, scene_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 1975: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 1976: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 1977: Menetapkan atau membandingkan scene_order (urutan adegan narasi) terhadap excluded.scene_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1978: Menetapkan atau membandingkan text_lines (nilai text lines) terhadap excluded.text_lines, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1979: Menetapkan atau membandingkan media_json (dokumen JSON media) terhadap excluded.media_json, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1980: Menetapkan atau membandingkan payload_json (rincian domain tambahan dalam JSONB) terhadap excluded.payload_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 1982: Memulai penyisipan rekaman ke ruleset_trigger_conditions, tabel yang menyimpan syarat pemicu narasi atau efek aturan; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 1983: Mengevaluasi ekspresi ruleset_trigger_conditions ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 1984: Menempatkan ruleset_trigger_conditions.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1985: Menempatkan ruleset_trigger_conditions.trigger_owner_type (nilai trigger owner type) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1986: Menempatkan ruleset_trigger_conditions.ruleset_narrative_id (identitas relasi ruleset narrative) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1987: Menempatkan ruleset_trigger_conditions.ruleset_action_id (identitas relasi ruleset action) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1988: Menempatkan ruleset_trigger_conditions.reference_asset_id (identitas relasi reference asset) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1989: Menempatkan ruleset_trigger_conditions.operator (operator pembanding syarat) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1990: Menempatkan ruleset_trigger_conditions.threshold_numeric (nilai threshold numeric) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1991: Menempatkan ruleset_trigger_conditions.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1992: Menempatkan ruleset_trigger_conditions.condition_json (dokumen JSON condition) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1993: Menempatkan ruleset_trigger_conditions.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 1994: Menutup kelompok yang terkait ; insert into ruleset_trigger_conditions pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 1995: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 1996: Memakai rn.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1997: Menyediakan nilai literal ’NARRATIVE’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 1998: Memakai rn.ruleset_narrative_id (identitas relasi ruleset narrative) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 1999: Memakai ra.ruleset_action_id (identitas relasi ruleset action) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2000: Mengevaluasi ekspresi null :: uuid, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 2001: Menyediakan nilai literal ’COUNT_GTE’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 2002: Mengevaluasi ekspresi greatest(coalesce((prereq ->> ’value’) :: int, 1), 1), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai; GREATEST memilih nilai terbesar untuk menerapkan batas bawah atau nilai maksimum dari kandidat.
-- Uraian sumber 2003: Mengevaluasi ekspresi ord :: int, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 2004: Mengevaluasi ekspresi jsonb_build_object(’source’, ’narrative.prerequisiteAksi’), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 2005: Memakai true (nilai true) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2006: Menetapkan sumber baris seed_rulesets sr cross join lateral jsonb_array_elements( sr.definition_json -> ’component_catalog’ -> ’narasi’ ) as narrative(narrative) cross join lateral ... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 2007: Mengevaluasi ekspresi seed_rulesets sr pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2008: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 2009: Mengevaluasi ekspresi sr.definition_json -> ’component_catalog’ -> ’narasi’ pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 2010: Mengevaluasi ekspresi ) as narrative(narrative) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil narrative (nilai narrative).
-- Uraian sumber 2011: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 2012: Mengevaluasi ekspresi coalesce( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 2013: Mengevaluasi ekspresi narrative.narrative -> ’prerequisiteAksi’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 2014: Menyediakan parameter JSONB bagi skrip basis data: Menutup susunan objek atau daftar JSON yang sedang dibentuk; pemisah koma mempertahankan batas antaranggota dokumen konfigurasi. Cast ::jsonb memvalidasi struktur JSON sebelum nilai dipakai aturan atau event.
-- Uraian sumber 2015: Menutup kelompok yang terkait lateral jsonb_array_elements ( coalesce pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2016: Mengevaluasi ekspresi ) with ordinality as x(prereq, ord) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil x (nilai x).
-- Uraian sumber 2017: Menggabungkan sumber join ruleset_narratives rn on rn.ruleset_version_id = sr.ruleset_version_id dalam skrip basis data. JOIN memasangkan baris yang memenuhi syarat ON sehingga identitas aturan, sesi, atau aset tetap selaras.
-- Uraian sumber 2018: Menambahkan syarat wajib rn.narrative_code = narrative.narrative ->> ’id’ pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast.
-- Uraian sumber 2019: Menggabungkan sumber join ruleset_actions ra on ra.ruleset_version_id = sr.ruleset_version_id dalam skrip basis data. JOIN memasangkan baris yang memenuhi syarat ON sehingga identitas aturan, sesi, atau aset tetap selaras.
-- Uraian sumber 2020: Menambahkan syarat wajib lower(ra.action_id) = lower(prereq ->> ’aksi’) pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos. operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; LOWER menormalkan huruf untuk pencocokan kode/nama tanpa perbedaan kapital.
-- Uraian sumber 2021: Menetapkan penanganan konflik kunci unik pada INSERT: and ra.is_active on conflict (. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 2022: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2023: Memakai ruleset_narrative_id (identitas relasi ruleset narrative) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2024: Memakai sort_order (urutan tampilan atau evaluasi komponen) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2025: Mengevaluasi ekspresi ) do pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2026: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 2027: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 2028: Menetapkan atau membandingkan ruleset_action_id (identitas relasi ruleset action) terhadap excluded.ruleset_action_id, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2029: Menetapkan atau membandingkan reference_asset_id (identitas relasi reference asset) terhadap excluded.reference_asset_id, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2030: Menetapkan atau membandingkan operator (operator pembanding syarat) terhadap excluded.operator, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2031: Menetapkan atau membandingkan threshold_numeric (nilai threshold numeric) terhadap excluded.threshold_numeric, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2032: Menetapkan atau membandingkan condition_json (dokumen JSON condition) terhadap excluded.condition_json, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2033: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2035: Memulai penyisipan rekaman ke ruleset_rank_points, tabel yang memetakan peringkat donasi atau pensiun ke jumlah poin; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 2036: Mengevaluasi ekspresi ruleset_rank_points ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2037: Menempatkan ruleset_rank_points.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2038: Menempatkan ruleset_rank_points.rank_type (nilai rank type) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2039: Menempatkan ruleset_rank_points.rank_no (nilai rank no) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2040: Menempatkan ruleset_rank_points.points (nilai poin) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2041: Menempatkan ruleset_rank_points.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2042: Menutup kelompok yang terkait ; insert into ruleset_rank_points pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2043: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 2044: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2045: Menyediakan nilai literal ’DONATION’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 2046: Mengevaluasi ekspresi coalesce((item ->> ’rank’) :: int, ord :: int), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 2047: Mengevaluasi ekspresi coalesce((item ->> ’points’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 2048: Mengevaluasi ekspresi ord :: int pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 2049: Menetapkan sumber baris seed_rulesets sr cross join lateral jsonb_array_elements( coalesce( sr.definition_json -> ’scoring’ -> ’donation_rank_points’, ’[]’ :: jsonb ) yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 2050: Mengevaluasi ekspresi seed_rulesets sr pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2051: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 2052: Mengevaluasi ekspresi coalesce( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 2053: Mengevaluasi ekspresi sr.definition_json -> ’scoring’ -> ’donation_rank_points’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 2054: Menyediakan parameter JSONB bagi skrip basis data: Menutup susunan objek atau daftar JSON yang sedang dibentuk; pemisah koma mempertahankan batas antaranggota dokumen konfigurasi. Cast ::jsonb memvalidasi struktur JSON sebelum nilai dipakai aturan atau event.
-- Uraian sumber 2055: Menutup kelompok yang terkait lateral jsonb_array_elements ( coalesce pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2056: Menetapkan penanganan konflik kunci unik pada INSERT: ) with ordinality as x(item, ord) on conflict (ruleset_version_id, rank_type, rank_no) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed. memberi nama hasil x (nilai x).
-- Uraian sumber 2057: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 2058: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 2059: Menetapkan atau membandingkan points (nilai poin) terhadap excluded.points, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2060: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2062: Memulai penyisipan rekaman ke ruleset_gold_assets, tabel yang menyimpan kartu emas fisik beserta kuantitas dan poin kepemilikannya; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 2063: Mengevaluasi ekspresi ruleset_gold_assets ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2064: Menempatkan ruleset_gold_assets.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2065: Menempatkan ruleset_gold_assets.ruleset_game_asset_id (UUID aset pada katalog induk) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2066: Menempatkan ruleset_gold_assets.asset_code (kode aset dalam versi aturan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2067: Menempatkan ruleset_gold_assets.quantity (jumlah unit) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2068: Menempatkan ruleset_gold_assets.points (nilai poin) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2069: Menempatkan ruleset_gold_assets.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2070: Menempatkan ruleset_gold_assets.card_qty (jumlah salinan kartu fisik yang tersedia) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2071: Menempatkan ruleset_gold_assets.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2072: Menempatkan ruleset_gold_assets.payload_json (rincian domain tambahan dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2073: Menutup kelompok yang terkait ; insert into ruleset_gold_assets pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2074: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 2075: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2076: Memakai rga.ruleset_game_asset_id (UUID aset pada katalog induk) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2077: Menyediakan nilai literal ’gold_card’ :: varchar(120), dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 2078: Mengevaluasi ekspresi coalesce((item ->> ’qty’) :: int, ord :: int), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 2079: Mengevaluasi ekspresi coalesce((item ->> ’points’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 2080: Mengevaluasi ekspresi ord :: int, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 2081: Memakai 1 (nilai 1) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2082: Memakai true (nilai true) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2083: Mengevaluasi ekspresi jsonb_build_object( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 2084: Menyediakan nilai literal ’qty’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 2085: Mengevaluasi ekspresi coalesce((item ->> ’qty’) :: int, ord :: int), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 2086: Menyediakan nilai literal ’points’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 2087: Mengevaluasi ekspresi coalesce((item ->> ’points’) :: int, 0) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 2088: Menutup kelompok yang terkait , true , jsonb_build_object pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2089: Menetapkan sumber baris seed_rulesets sr cross join lateral jsonb_array_elements( coalesce( sr.definition_json -> ’scoring’ -> ’gold_points_by_qty’, ’[]’ :: jsonb ) yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 2090: Mengevaluasi ekspresi seed_rulesets sr pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2091: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 2092: Mengevaluasi ekspresi coalesce( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 2093: Mengevaluasi ekspresi sr.definition_json -> ’scoring’ -> ’gold_points_by_qty’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 2094: Menyediakan parameter JSONB bagi skrip basis data: Menutup susunan objek atau daftar JSON yang sedang dibentuk; pemisah koma mempertahankan batas antaranggota dokumen konfigurasi. Cast ::jsonb memvalidasi struktur JSON sebelum nilai dipakai aturan atau event.
-- Uraian sumber 2095: Menutup kelompok yang terkait lateral jsonb_array_elements ( coalesce pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2096: Mengevaluasi ekspresi ) with ordinality as x(item, ord) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil x (nilai x).
-- Uraian sumber 2097: Menggabungkan sumber join ruleset_game_assets rga on rga.ruleset_version_id = sr.ruleset_version_id dalam skrip basis data. JOIN memasangkan baris yang memenuhi syarat ON sehingga identitas aturan, sesi, atau aset tetap selaras.
-- Uraian sumber 2098: Menambahkan syarat wajib rga.asset_type = ’GOLD’ pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos.
-- Uraian sumber 2099: Menetapkan penanganan konflik kunci unik pada INSERT: and rga.asset_code = ’gold_card’ on conflict (ruleset_version_id, asset_code, quantity) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 2100: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 2101: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 2102: Menetapkan atau membandingkan quantity (jumlah unit) terhadap excluded.quantity, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2103: Menetapkan atau membandingkan points (nilai poin) terhadap excluded.points, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2104: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2105: Menetapkan atau membandingkan card_qty (jumlah salinan kartu fisik yang tersedia) terhadap excluded.card_qty, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2106: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2107: Menetapkan atau membandingkan payload_json (rincian domain tambahan dalam JSONB) terhadap excluded.payload_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2109: Memulai penyisipan rekaman ke ruleset_rank_points, tabel yang memetakan peringkat donasi atau pensiun ke jumlah poin; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 2110: Mengevaluasi ekspresi ruleset_rank_points ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2111: Menempatkan ruleset_rank_points.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2112: Menempatkan ruleset_rank_points.rank_type (nilai rank type) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2113: Menempatkan ruleset_rank_points.rank_no (nilai rank no) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2114: Menempatkan ruleset_rank_points.points (nilai poin) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2115: Menempatkan ruleset_rank_points.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2116: Menutup kelompok yang terkait ; insert into ruleset_rank_points pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2117: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 2118: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2119: Menyediakan nilai literal ’PENSION’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 2120: Mengevaluasi ekspresi coalesce((item ->> ’rank’) :: int, ord :: int), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 2121: Mengevaluasi ekspresi coalesce((item ->> ’points’) :: int, 0), pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong; operator ->> mengambil isi properti JSON sebagai teks untuk pembandingan atau cast; operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 2122: Mengevaluasi ekspresi ord :: int pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 2123: Menetapkan sumber baris seed_rulesets sr cross join lateral jsonb_array_elements( coalesce( sr.definition_json -> ’scoring’ -> ’pension_rank_points’, ’[]’ :: jsonb ) yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 2124: Mengevaluasi ekspresi seed_rulesets sr pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2125: Menggabungkan sumber cross join lateral jsonb_array_elements( dalam skrip basis data. LATERAL memungkinkan sumber kanan memakai nilai setiap baris kiri, misalnya memecah array JSON katalog. jsonb_array_elements memecah anggota array JSON menjadi baris agar dapat dipetakan ke katalog atau syarat relasional.
-- Uraian sumber 2126: Mengevaluasi ekspresi coalesce( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. COALESCE memilih nilai pertama yang tidak NULL agar nilai cadangan hanya dipakai saat data sebelumnya kosong.
-- Uraian sumber 2127: Mengevaluasi ekspresi sr.definition_json -> ’scoring’ -> ’pension_rank_points’, pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator -> mengambil elemen JSON sambil mempertahankan tipe JSON.
-- Uraian sumber 2128: Menyediakan parameter JSONB bagi skrip basis data: Menutup susunan objek atau daftar JSON yang sedang dibentuk; pemisah koma mempertahankan batas antaranggota dokumen konfigurasi. Cast ::jsonb memvalidasi struktur JSON sebelum nilai dipakai aturan atau event.
-- Uraian sumber 2129: Menutup kelompok yang terkait lateral jsonb_array_elements ( coalesce pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2130: Menetapkan penanganan konflik kunci unik pada INSERT: ) with ordinality as x(item, ord) on conflict (ruleset_version_id, rank_type, rank_no) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed. memberi nama hasil x (nilai x).
-- Uraian sumber 2131: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 2132: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 2133: Menetapkan atau membandingkan points (nilai poin) terhadap excluded.points, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2134: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2136: Memulai penyisipan rekaman ke ruleset_gold_prices, tabel yang menyimpan kartu harga beli dan jual emas yang dapat berlaku dalam sesi; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 2137: Mengevaluasi ekspresi ruleset_gold_prices ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2138: Menempatkan ruleset_gold_prices.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2139: Menempatkan ruleset_gold_prices.ruleset_game_asset_id (UUID aset pada katalog induk) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2140: Menempatkan ruleset_gold_prices.price_code (kode kartu harga emas) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2141: Menempatkan ruleset_gold_prices.quantity (jumlah unit) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2142: Menempatkan ruleset_gold_prices.unit_price (nilai unit price) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2143: Menempatkan ruleset_gold_prices.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2144: Menempatkan ruleset_gold_prices.card_qty (jumlah salinan kartu fisik yang tersedia) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2145: Menempatkan ruleset_gold_prices.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2146: Menempatkan ruleset_gold_prices.payload_json (rincian domain tambahan dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2147: Menutup kelompok yang terkait ; insert into ruleset_gold_prices pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2148: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 2149: Memakai extra.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2150: Memakai rga.ruleset_game_asset_id (UUID aset pada katalog induk) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2151: Memakai extra.price_code (kode kartu harga emas) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2152: Memakai extra.quantity (jumlah unit) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2153: Memakai extra.unit_price (nilai unit price) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2154: Memakai extra.sort_order (urutan tampilan atau evaluasi komponen) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2155: Memakai extra.card_qty (jumlah salinan kartu fisik yang tersedia) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2156: Memakai true (nilai true) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2157: Memakai extra.payload_json (rincian domain tambahan dalam JSONB) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2158: Menetapkan sumber baris ( values ( ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid, ’gold_price_1’, 1, yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 2159: Membuka kelompok yang terkait extra . payload_json from pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2160: Menyediakan 8 tuple nilai eksplisit untuk extra; setiap tuple membentuk satu rekaman dan mengikuti urutan ruleset_version_id, price_code, quantity, unit_price, sort_order, card_qty, payload_json.
-- Uraian sumber 2161: Membuka rekaman seed extra, rekaman ke-1; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2162: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk extra, rekaman ke-1.
-- Uraian sumber 2163: Mengisi price_code (kode kartu harga emas) dengan ’gold_price_1’ untuk extra, rekaman ke-1.
-- Uraian sumber 2164: Mengisi quantity (jumlah unit) dengan 1 untuk extra, rekaman ke-1.
-- Uraian sumber 2165: Mengisi unit_price (nilai unit price) dengan 5 untuk extra, rekaman ke-1.
-- Uraian sumber 2166: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 221 untuk extra, rekaman ke-1.
-- Uraian sumber 2167: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 2 untuk extra, rekaman ke-1.
-- Uraian sumber 2168: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”qty”:1,”price”:5}’ :: jsonb untuk extra, rekaman ke-1. Isi JSON dipakai sebagai parameter aksi/metadata: qty=1 (kuantitas aset); price=5 (biaya dalam koin).
-- Uraian sumber 2169: Menutup rekaman seed extra, rekaman ke-1; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2170: Membuka rekaman seed extra, rekaman ke-2; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2171: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk extra, rekaman ke-2.
-- Uraian sumber 2172: Mengisi price_code (kode kartu harga emas) dengan ’gold_price_2’ untuk extra, rekaman ke-2.
-- Uraian sumber 2173: Mengisi quantity (jumlah unit) dengan 1 untuk extra, rekaman ke-2.
-- Uraian sumber 2174: Mengisi unit_price (nilai unit price) dengan 6 untuk extra, rekaman ke-2.
-- Uraian sumber 2175: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 222 untuk extra, rekaman ke-2.
-- Uraian sumber 2176: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 2 untuk extra, rekaman ke-2.
-- Uraian sumber 2177: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”qty”:1,”price”:6}’ :: jsonb untuk extra, rekaman ke-2. Isi JSON dipakai sebagai parameter aksi/metadata: qty=1 (kuantitas aset); price=6 (biaya dalam koin).
-- Uraian sumber 2178: Menutup rekaman seed extra, rekaman ke-2; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2179: Membuka rekaman seed extra, rekaman ke-3; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2180: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk extra, rekaman ke-3.
-- Uraian sumber 2181: Mengisi price_code (kode kartu harga emas) dengan ’gold_price_3’ untuk extra, rekaman ke-3.
-- Uraian sumber 2182: Mengisi quantity (jumlah unit) dengan 1 untuk extra, rekaman ke-3.
-- Uraian sumber 2183: Mengisi unit_price (nilai unit price) dengan 7 untuk extra, rekaman ke-3.
-- Uraian sumber 2184: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 223 untuk extra, rekaman ke-3.
-- Uraian sumber 2185: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-3.
-- Uraian sumber 2186: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”qty”:1,”price”:7}’ :: jsonb untuk extra, rekaman ke-3. Isi JSON dipakai sebagai parameter aksi/metadata: qty=1 (kuantitas aset); price=7 (biaya dalam koin).
-- Uraian sumber 2187: Menutup rekaman seed extra, rekaman ke-3; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2188: Membuka rekaman seed extra, rekaman ke-4; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2189: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk extra, rekaman ke-4.
-- Uraian sumber 2190: Mengisi price_code (kode kartu harga emas) dengan ’gold_price_4’ untuk extra, rekaman ke-4.
-- Uraian sumber 2191: Mengisi quantity (jumlah unit) dengan 1 untuk extra, rekaman ke-4.
-- Uraian sumber 2192: Mengisi unit_price (nilai unit price) dengan 8 untuk extra, rekaman ke-4.
-- Uraian sumber 2193: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 224 untuk extra, rekaman ke-4.
-- Uraian sumber 2194: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-4.
-- Uraian sumber 2195: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”qty”:1,”price”:8}’ :: jsonb untuk extra, rekaman ke-4. Isi JSON dipakai sebagai parameter aksi/metadata: qty=1 (kuantitas aset); price=8 (biaya dalam koin).
-- Uraian sumber 2196: Menutup rekaman seed extra, rekaman ke-4; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2197: Membuka rekaman seed extra, rekaman ke-5; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2198: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-5.
-- Uraian sumber 2199: Mengisi price_code (kode kartu harga emas) dengan ’gold_price_1’ untuk extra, rekaman ke-5.
-- Uraian sumber 2200: Mengisi quantity (jumlah unit) dengan 1 untuk extra, rekaman ke-5.
-- Uraian sumber 2201: Mengisi unit_price (nilai unit price) dengan 5 untuk extra, rekaman ke-5.
-- Uraian sumber 2202: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 221 untuk extra, rekaman ke-5.
-- Uraian sumber 2203: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 2 untuk extra, rekaman ke-5.
-- Uraian sumber 2204: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”qty”:1,”price”:5}’ :: jsonb untuk extra, rekaman ke-5. Isi JSON dipakai sebagai parameter aksi/metadata: qty=1 (kuantitas aset); price=5 (biaya dalam koin).
-- Uraian sumber 2205: Menutup rekaman seed extra, rekaman ke-5; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2206: Membuka rekaman seed extra, rekaman ke-6; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2207: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-6.
-- Uraian sumber 2208: Mengisi price_code (kode kartu harga emas) dengan ’gold_price_2’ untuk extra, rekaman ke-6.
-- Uraian sumber 2209: Mengisi quantity (jumlah unit) dengan 1 untuk extra, rekaman ke-6.
-- Uraian sumber 2210: Mengisi unit_price (nilai unit price) dengan 6 untuk extra, rekaman ke-6.
-- Uraian sumber 2211: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 222 untuk extra, rekaman ke-6.
-- Uraian sumber 2212: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 2 untuk extra, rekaman ke-6.
-- Uraian sumber 2213: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”qty”:1,”price”:6}’ :: jsonb untuk extra, rekaman ke-6. Isi JSON dipakai sebagai parameter aksi/metadata: qty=1 (kuantitas aset); price=6 (biaya dalam koin).
-- Uraian sumber 2214: Menutup rekaman seed extra, rekaman ke-6; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2215: Membuka rekaman seed extra, rekaman ke-7; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2216: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-7.
-- Uraian sumber 2217: Mengisi price_code (kode kartu harga emas) dengan ’gold_price_3’ untuk extra, rekaman ke-7.
-- Uraian sumber 2218: Mengisi quantity (jumlah unit) dengan 1 untuk extra, rekaman ke-7.
-- Uraian sumber 2219: Mengisi unit_price (nilai unit price) dengan 7 untuk extra, rekaman ke-7.
-- Uraian sumber 2220: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 223 untuk extra, rekaman ke-7.
-- Uraian sumber 2221: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-7.
-- Uraian sumber 2222: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”qty”:1,”price”:7}’ :: jsonb untuk extra, rekaman ke-7. Isi JSON dipakai sebagai parameter aksi/metadata: qty=1 (kuantitas aset); price=7 (biaya dalam koin).
-- Uraian sumber 2223: Menutup rekaman seed extra, rekaman ke-7; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2224: Membuka rekaman seed extra, rekaman ke-8; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2225: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-8.
-- Uraian sumber 2226: Mengisi price_code (kode kartu harga emas) dengan ’gold_price_4’ untuk extra, rekaman ke-8.
-- Uraian sumber 2227: Mengisi quantity (jumlah unit) dengan 1 untuk extra, rekaman ke-8.
-- Uraian sumber 2228: Mengisi unit_price (nilai unit price) dengan 8 untuk extra, rekaman ke-8.
-- Uraian sumber 2229: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 224 untuk extra, rekaman ke-8.
-- Uraian sumber 2230: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-8.
-- Uraian sumber 2231: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”qty”:1,”price”:8}’ :: jsonb untuk extra, rekaman ke-8. Isi JSON dipakai sebagai parameter aksi/metadata: qty=1 (kuantitas aset); price=8 (biaya dalam koin).
-- Uraian sumber 2232: Menutup rekaman seed extra, rekaman ke-8; kueri melanjutkan pemrosesan kumpulan nilai yang telah lengkap.
-- Uraian sumber 2233: Mengevaluasi ekspresi ) as extra( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil extra (nilai extra).
-- Uraian sumber 2234: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2235: Memakai price_code (kode kartu harga emas) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2236: Memakai quantity (jumlah unit) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2237: Memakai unit_price (nilai unit price) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2238: Memakai sort_order (urutan tampilan atau evaluasi komponen) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2239: Memakai card_qty (jumlah salinan kartu fisik yang tersedia) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2240: Memakai payload_json (rincian domain tambahan dalam JSONB) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2241: Menutup kelompok yang terkait ) ) as extra pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2242: Menggabungkan sumber join ruleset_game_assets rga on rga.ruleset_version_id = extra.ruleset_version_id dalam skrip basis data. JOIN memasangkan baris yang memenuhi syarat ON sehingga identitas aturan, sesi, atau aset tetap selaras.
-- Uraian sumber 2243: Menambahkan syarat wajib rga.asset_type = ’GOLD_PRICE’ pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos.
-- Uraian sumber 2244: Menetapkan penanganan konflik kunci unik pada INSERT: and rga.asset_code = extra.price_code on conflict (ruleset_version_id, price_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 2245: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 2246: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 2247: Menetapkan atau membandingkan quantity (jumlah unit) terhadap excluded.quantity, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2248: Menetapkan atau membandingkan unit_price (nilai unit price) terhadap excluded.unit_price, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2249: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2250: Menetapkan atau membandingkan card_qty (jumlah salinan kartu fisik yang tersedia) terhadap excluded.card_qty, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2251: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2252: Menetapkan atau membandingkan payload_json (rincian domain tambahan dalam JSONB) terhadap excluded.payload_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2254: Memulai penyisipan rekaman ke ruleset_tie_breakers, tabel yang menyimpan kartu pembeda urutan ketika hasil pemain sama; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 2255: Mengevaluasi ekspresi ruleset_tie_breakers ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2256: Menempatkan ruleset_tie_breakers.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2257: Menempatkan ruleset_tie_breakers.ruleset_game_asset_id (UUID aset pada katalog induk) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2258: Menempatkan ruleset_tie_breakers.tie_breaker_code (kode kartu pemecah nilai seri) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2259: Menempatkan ruleset_tie_breakers.tie_number (nilai tie number) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2260: Menempatkan ruleset_tie_breakers.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2261: Menempatkan ruleset_tie_breakers.card_qty (jumlah salinan kartu fisik yang tersedia) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2262: Menempatkan ruleset_tie_breakers.payload_json (rincian domain tambahan dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2263: Menutup kelompok yang terkait ; insert into ruleset_tie_breakers pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2264: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 2265: Memakai extra.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2266: Memakai rga.ruleset_game_asset_id (UUID aset pada katalog induk) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2267: Memakai extra.tie_breaker_code (kode kartu pemecah nilai seri) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2268: Memakai extra.tie_number (nilai tie number) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2269: Memakai extra.sort_order (urutan tampilan atau evaluasi komponen) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2270: Memakai extra.card_qty (jumlah salinan kartu fisik yang tersedia) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2271: Memakai extra.payload_json (rincian domain tambahan dalam JSONB) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2272: Menetapkan sumber baris ( values ( ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid, ’tie_breaker_1’, 1, yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 2273: Membuka kelompok yang terkait extra . payload_json from pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2274: Menyediakan 8 tuple nilai eksplisit untuk extra; setiap tuple membentuk satu rekaman dan mengikuti urutan ruleset_version_id, tie_breaker_code, tie_number, sort_order, card_qty, payload_json.
-- Uraian sumber 2275: Membuka rekaman seed extra, rekaman ke-1; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2276: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk extra, rekaman ke-1.
-- Uraian sumber 2277: Mengisi tie_breaker_code (kode kartu pemecah nilai seri) dengan ’tie_breaker_1’ untuk extra, rekaman ke-1.
-- Uraian sumber 2278: Mengisi tie_number (nilai tie number) dengan 1 untuk extra, rekaman ke-1.
-- Uraian sumber 2279: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 241 untuk extra, rekaman ke-1.
-- Uraian sumber 2280: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-1.
-- Uraian sumber 2281: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”number”:1}’ :: jsonb untuk extra, rekaman ke-1. Isi JSON dipakai sebagai parameter aksi/metadata: number=1 (nomor pembeda kartu).
-- Uraian sumber 2282: Menutup rekaman seed extra, rekaman ke-1; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2283: Membuka rekaman seed extra, rekaman ke-2; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2284: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk extra, rekaman ke-2.
-- Uraian sumber 2285: Mengisi tie_breaker_code (kode kartu pemecah nilai seri) dengan ’tie_breaker_2’ untuk extra, rekaman ke-2.
-- Uraian sumber 2286: Mengisi tie_number (nilai tie number) dengan 2 untuk extra, rekaman ke-2.
-- Uraian sumber 2287: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 242 untuk extra, rekaman ke-2.
-- Uraian sumber 2288: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-2.
-- Uraian sumber 2289: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”number”:2}’ :: jsonb untuk extra, rekaman ke-2. Isi JSON dipakai sebagai parameter aksi/metadata: number=2 (nomor pembeda kartu).
-- Uraian sumber 2290: Menutup rekaman seed extra, rekaman ke-2; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2291: Membuka rekaman seed extra, rekaman ke-3; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2292: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk extra, rekaman ke-3.
-- Uraian sumber 2293: Mengisi tie_breaker_code (kode kartu pemecah nilai seri) dengan ’tie_breaker_3’ untuk extra, rekaman ke-3.
-- Uraian sumber 2294: Mengisi tie_number (nilai tie number) dengan 3 untuk extra, rekaman ke-3.
-- Uraian sumber 2295: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 243 untuk extra, rekaman ke-3.
-- Uraian sumber 2296: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-3.
-- Uraian sumber 2297: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”number”:3}’ :: jsonb untuk extra, rekaman ke-3. Isi JSON dipakai sebagai parameter aksi/metadata: number=3 (nomor pembeda kartu).
-- Uraian sumber 2298: Menutup rekaman seed extra, rekaman ke-3; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2299: Membuka rekaman seed extra, rekaman ke-4; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2300: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk extra, rekaman ke-4.
-- Uraian sumber 2301: Mengisi tie_breaker_code (kode kartu pemecah nilai seri) dengan ’tie_breaker_4’ untuk extra, rekaman ke-4.
-- Uraian sumber 2302: Mengisi tie_number (nilai tie number) dengan 4 untuk extra, rekaman ke-4.
-- Uraian sumber 2303: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 244 untuk extra, rekaman ke-4.
-- Uraian sumber 2304: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-4.
-- Uraian sumber 2305: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”number”:4}’ :: jsonb untuk extra, rekaman ke-4. Isi JSON dipakai sebagai parameter aksi/metadata: number=4 (nomor pembeda kartu).
-- Uraian sumber 2306: Menutup rekaman seed extra, rekaman ke-4; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2307: Membuka rekaman seed extra, rekaman ke-5; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2308: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-5.
-- Uraian sumber 2309: Mengisi tie_breaker_code (kode kartu pemecah nilai seri) dengan ’tie_breaker_1’ untuk extra, rekaman ke-5.
-- Uraian sumber 2310: Mengisi tie_number (nilai tie number) dengan 1 untuk extra, rekaman ke-5.
-- Uraian sumber 2311: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 241 untuk extra, rekaman ke-5.
-- Uraian sumber 2312: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-5.
-- Uraian sumber 2313: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”number”:1}’ :: jsonb untuk extra, rekaman ke-5. Isi JSON dipakai sebagai parameter aksi/metadata: number=1 (nomor pembeda kartu).
-- Uraian sumber 2314: Menutup rekaman seed extra, rekaman ke-5; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2315: Membuka rekaman seed extra, rekaman ke-6; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2316: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-6.
-- Uraian sumber 2317: Mengisi tie_breaker_code (kode kartu pemecah nilai seri) dengan ’tie_breaker_2’ untuk extra, rekaman ke-6.
-- Uraian sumber 2318: Mengisi tie_number (nilai tie number) dengan 2 untuk extra, rekaman ke-6.
-- Uraian sumber 2319: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 242 untuk extra, rekaman ke-6.
-- Uraian sumber 2320: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-6.
-- Uraian sumber 2321: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”number”:2}’ :: jsonb untuk extra, rekaman ke-6. Isi JSON dipakai sebagai parameter aksi/metadata: number=2 (nomor pembeda kartu).
-- Uraian sumber 2322: Menutup rekaman seed extra, rekaman ke-6; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2323: Membuka rekaman seed extra, rekaman ke-7; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2324: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-7.
-- Uraian sumber 2325: Mengisi tie_breaker_code (kode kartu pemecah nilai seri) dengan ’tie_breaker_3’ untuk extra, rekaman ke-7.
-- Uraian sumber 2326: Mengisi tie_number (nilai tie number) dengan 3 untuk extra, rekaman ke-7.
-- Uraian sumber 2327: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 243 untuk extra, rekaman ke-7.
-- Uraian sumber 2328: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-7.
-- Uraian sumber 2329: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”number”:3}’ :: jsonb untuk extra, rekaman ke-7. Isi JSON dipakai sebagai parameter aksi/metadata: number=3 (nomor pembeda kartu).
-- Uraian sumber 2330: Menutup rekaman seed extra, rekaman ke-7; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2331: Membuka rekaman seed extra, rekaman ke-8; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2332: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-8.
-- Uraian sumber 2333: Mengisi tie_breaker_code (kode kartu pemecah nilai seri) dengan ’tie_breaker_4’ untuk extra, rekaman ke-8.
-- Uraian sumber 2334: Mengisi tie_number (nilai tie number) dengan 4 untuk extra, rekaman ke-8.
-- Uraian sumber 2335: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 244 untuk extra, rekaman ke-8.
-- Uraian sumber 2336: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-8.
-- Uraian sumber 2337: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”number”:4}’ :: jsonb untuk extra, rekaman ke-8. Isi JSON dipakai sebagai parameter aksi/metadata: number=4 (nomor pembeda kartu).
-- Uraian sumber 2338: Menutup rekaman seed extra, rekaman ke-8; kueri melanjutkan pemrosesan kumpulan nilai yang telah lengkap.
-- Uraian sumber 2339: Mengevaluasi ekspresi ) as extra( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil extra (nilai extra).
-- Uraian sumber 2340: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2341: Memakai tie_breaker_code (kode kartu pemecah nilai seri) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2342: Memakai tie_number (nilai tie number) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2343: Memakai sort_order (urutan tampilan atau evaluasi komponen) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2344: Memakai card_qty (jumlah salinan kartu fisik yang tersedia) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2345: Memakai payload_json (rincian domain tambahan dalam JSONB) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2346: Menutup kelompok yang terkait ) ) as extra pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2347: Menggabungkan sumber join ruleset_game_assets rga on rga.ruleset_version_id = extra.ruleset_version_id dalam skrip basis data. JOIN memasangkan baris yang memenuhi syarat ON sehingga identitas aturan, sesi, atau aset tetap selaras.
-- Uraian sumber 2348: Menambahkan syarat wajib rga.asset_type = ’TIE_BREAKER’ pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos.
-- Uraian sumber 2349: Menetapkan penanganan konflik kunci unik pada INSERT: and rga.asset_code = extra.tie_breaker_code on conflict (ruleset_version_id, tie_breaker_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 2350: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 2351: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 2352: Menetapkan atau membandingkan tie_number (nilai tie number) terhadap excluded.tie_number, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2353: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2354: Menetapkan atau membandingkan card_qty (jumlah salinan kartu fisik yang tersedia) terhadap excluded.card_qty, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2355: Menetapkan atau membandingkan payload_json (rincian domain tambahan dalam JSONB) terhadap excluded.payload_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2357: Memulai penyisipan rekaman ke ruleset_sharia_loans, tabel yang menyimpan produk pinjaman syariah, pokok, pelunasan, dan penalti poin; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 2358: Mengevaluasi ekspresi ruleset_sharia_loans ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2359: Menempatkan ruleset_sharia_loans.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2360: Menempatkan ruleset_sharia_loans.loan_code (kode produk pinjaman syariah) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2361: Menempatkan ruleset_sharia_loans.item_name (nama item dalam katalog) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2362: Menempatkan ruleset_sharia_loans.principal (pokok pinjaman dalam koin) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2363: Menempatkan ruleset_sharia_loans.repayment_amount (nilai yang harus dibayar saat pelunasan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2364: Menempatkan ruleset_sharia_loans.duration_days (nilai duration days) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2365: Menempatkan ruleset_sharia_loans.penalty_points (pengurang poin karena kewajiban atau misi gagal) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2366: Menempatkan ruleset_sharia_loans.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2367: Menempatkan ruleset_sharia_loans.card_qty (jumlah salinan kartu fisik yang tersedia) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2368: Menempatkan ruleset_sharia_loans.payload_json (rincian domain tambahan dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2369: Menutup kelompok yang terkait ; insert into ruleset_sharia_loans pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2370: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 2371: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2372: Memakai extra.loan_code (kode produk pinjaman syariah) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2373: Memakai extra.item_name (nama item dalam katalog) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2374: Memakai extra.principal (pokok pinjaman dalam koin) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2375: Memakai extra.repayment_amount (nilai yang harus dibayar saat pelunasan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2376: Memakai extra.duration_days (nilai duration days) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2377: Memakai extra.penalty_points (pengurang poin karena kewajiban atau misi gagal) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2378: Memakai extra.sort_order (urutan tampilan atau evaluasi komponen) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2379: Memakai extra.card_qty (jumlah salinan kartu fisik yang tersedia) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2380: Memakai extra.payload_json (rincian domain tambahan dalam JSONB) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2381: Menetapkan sumber baris ( values ( ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid, ’loan_syariah_10’, ’Pinjaman Syariah 10’, yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 2382: Membuka kelompok yang terkait extra . payload_json from pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2383: Menyediakan 1 tuple nilai eksplisit untuk extra; setiap tuple membentuk satu rekaman dan mengikuti urutan ruleset_version_id, loan_code, item_name, principal, repayment_amount, duration_days, penalty_points, sort_order, card_qty, payload_json.
-- Uraian sumber 2384: Membuka rekaman seed extra, rekaman ke-1; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2385: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-1.
-- Uraian sumber 2386: Mengisi loan_code (kode produk pinjaman syariah) dengan ’loan_syariah_10’ untuk extra, rekaman ke-1.
-- Uraian sumber 2387: Mengisi item_name (nama item dalam katalog) dengan ’Pinjaman Syariah 10’ untuk extra, rekaman ke-1.
-- Uraian sumber 2388: Mengisi principal (pokok pinjaman dalam koin) dengan 10 untuk extra, rekaman ke-1.
-- Uraian sumber 2389: Mengisi repayment_amount (nilai yang harus dibayar saat pelunasan) dengan 10 untuk extra, rekaman ke-1.
-- Uraian sumber 2390: Mengisi duration_days (nilai duration days) dengan 25 untuk extra, rekaman ke-1.
-- Uraian sumber 2391: Mengisi penalty_points (pengurang poin karena kewajiban atau misi gagal) dengan 15 untuk extra, rekaman ke-1.
-- Uraian sumber 2392: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 251 untuk extra, rekaman ke-1.
-- Uraian sumber 2393: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 8 untuk extra, rekaman ke-1.
-- Uraian sumber 2394: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”principal”:10,”repayment_amount”:10,”duration”:25,”penalty_points”:15,”card_supply”:8}’ :: jsonb untuk extra, rekaman ke-1. Isi JSON dipakai sebagai parameter aksi/metadata: principal=10 (pokok pinjaman dalam koin); repayment_amount=10 (nilai yang harus dibayar saat pelunasan); duration=25 (nilai duration); penalty_points=15 (pengurang poin karena kewajiban atau misi gagal); card_supply=8 (nilai card supply).
-- Uraian sumber 2395: Menutup rekaman seed extra, rekaman ke-1; kueri melanjutkan pemrosesan kumpulan nilai yang telah lengkap.
-- Uraian sumber 2396: Mengevaluasi ekspresi ) as extra( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil extra (nilai extra).
-- Uraian sumber 2397: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2398: Memakai loan_code (kode produk pinjaman syariah) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2399: Memakai item_name (nama item dalam katalog) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2400: Memakai principal (pokok pinjaman dalam koin) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2401: Memakai repayment_amount (nilai yang harus dibayar saat pelunasan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2402: Memakai duration_days (nilai duration days) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2403: Memakai penalty_points (pengurang poin karena kewajiban atau misi gagal) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2404: Memakai sort_order (urutan tampilan atau evaluasi komponen) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2405: Memakai card_qty (jumlah salinan kartu fisik yang tersedia) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2406: Memakai payload_json (rincian domain tambahan dalam JSONB) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2407: Menutup kelompok yang terkait ) ) as extra pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2408: Menetapkan penanganan konflik kunci unik pada INSERT: join seed_rulesets sr on sr.ruleset_version_id = extra.ruleset_version_id on conflict (ruleset_version_id, loan_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 2409: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 2410: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 2411: Menetapkan atau membandingkan item_name (nama item dalam katalog) terhadap excluded.item_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2412: Menetapkan atau membandingkan principal (pokok pinjaman dalam koin) terhadap excluded.principal, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2413: Menetapkan atau membandingkan repayment_amount (nilai yang harus dibayar saat pelunasan) terhadap excluded.repayment_amount, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2414: Menetapkan atau membandingkan duration_days (nilai duration days) terhadap excluded.duration_days, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2415: Menetapkan atau membandingkan penalty_points (pengurang poin karena kewajiban atau misi gagal) terhadap excluded.penalty_points, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2416: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2417: Menetapkan atau membandingkan card_qty (jumlah salinan kartu fisik yang tersedia) terhadap excluded.card_qty, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2418: Menetapkan atau membandingkan payload_json (rincian domain tambahan dalam JSONB) terhadap excluded.payload_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2420: Memulai penyisipan rekaman ke ruleset_insurance_products, tabel yang menyimpan produk perlindungan risiko, premi, serta batas pemakaiannya; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 2421: Mengevaluasi ekspresi ruleset_insurance_products ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2422: Menempatkan ruleset_insurance_products.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2423: Menempatkan ruleset_insurance_products.product_code (kode produk asuransi) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2424: Menempatkan ruleset_insurance_products.item_name (nama item dalam katalog) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2425: Menempatkan ruleset_insurance_products.premium (biaya premi asuransi) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2426: Menempatkan ruleset_insurance_products.usage_limit (nilai usage limit) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2427: Menempatkan ruleset_insurance_products.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2428: Menempatkan ruleset_insurance_products.card_qty (jumlah salinan kartu fisik yang tersedia) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2429: Menempatkan ruleset_insurance_products.payload_json (rincian domain tambahan dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2430: Menutup kelompok yang terkait ; insert into ruleset_insurance_products pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2431: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 2432: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2433: Memakai extra.product_code (kode produk asuransi) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2434: Memakai extra.item_name (nama item dalam katalog) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2435: Memakai extra.premium (biaya premi asuransi) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2436: Memakai extra.usage_limit (nilai usage limit) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2437: Memakai extra.sort_order (urutan tampilan atau evaluasi komponen) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2438: Memakai extra.card_qty (jumlah salinan kartu fisik yang tersedia) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2439: Memakai extra.payload_json (rincian domain tambahan dalam JSONB) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2440: Menetapkan sumber baris ( values ( ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid, ’multirisk_basic’, ’Asuransi Multirisk’, yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 2441: Membuka kelompok yang terkait extra . payload_json from pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2442: Menyediakan 1 tuple nilai eksplisit untuk extra; setiap tuple membentuk satu rekaman dan mengikuti urutan ruleset_version_id, product_code, item_name, premium, usage_limit, sort_order, card_qty, payload_json.
-- Uraian sumber 2443: Membuka rekaman seed extra, rekaman ke-1; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2444: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-1.
-- Uraian sumber 2445: Mengisi product_code (kode produk asuransi) dengan ’multirisk_basic’ untuk extra, rekaman ke-1.
-- Uraian sumber 2446: Mengisi item_name (nama item dalam katalog) dengan ’Asuransi Multirisk’ untuk extra, rekaman ke-1.
-- Uraian sumber 2447: Mengisi premium (biaya premi asuransi) dengan 1 untuk extra, rekaman ke-1.
-- Uraian sumber 2448: Mengisi usage_limit (nilai usage limit) dengan 1 untuk extra, rekaman ke-1.
-- Uraian sumber 2449: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 261 untuk extra, rekaman ke-1.
-- Uraian sumber 2450: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 0 untuk extra, rekaman ke-1.
-- Uraian sumber 2451: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”premium”:1,”usage_limit”:1,”physical_card_source”:”TIE_BREAKER_BACK”}’ :: jsonb untuk extra, rekaman ke-1. Isi JSON dipakai sebagai parameter aksi/metadata: premium=1 (biaya premi asuransi); usage_limit=1 (nilai usage limit); physical_card_source=”TIE_BREAKER_BACK” (nilai physical card source).
-- Uraian sumber 2452: Menutup rekaman seed extra, rekaman ke-1; kueri melanjutkan pemrosesan kumpulan nilai yang telah lengkap.
-- Uraian sumber 2453: Mengevaluasi ekspresi ) as extra( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil extra (nilai extra).
-- Uraian sumber 2454: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2455: Memakai product_code (kode produk asuransi) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2456: Memakai item_name (nama item dalam katalog) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2457: Memakai premium (biaya premi asuransi) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2458: Memakai usage_limit (nilai usage limit) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2459: Memakai sort_order (urutan tampilan atau evaluasi komponen) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2460: Memakai card_qty (jumlah salinan kartu fisik yang tersedia) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2461: Memakai payload_json (rincian domain tambahan dalam JSONB) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2462: Menutup kelompok yang terkait ) ) as extra pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2463: Menetapkan penanganan konflik kunci unik pada INSERT: join seed_rulesets sr on sr.ruleset_version_id = extra.ruleset_version_id on conflict (ruleset_version_id, product_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 2464: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 2465: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 2466: Menetapkan atau membandingkan item_name (nama item dalam katalog) terhadap excluded.item_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2467: Menetapkan atau membandingkan premium (biaya premi asuransi) terhadap excluded.premium, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2468: Menetapkan atau membandingkan usage_limit (nilai usage limit) terhadap excluded.usage_limit, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2469: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2470: Menetapkan atau membandingkan card_qty (jumlah salinan kartu fisik yang tersedia) terhadap excluded.card_qty, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2471: Menetapkan atau membandingkan payload_json (rincian domain tambahan dalam JSONB) terhadap excluded.payload_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2473: Memulai penyisipan rekaman ke ruleset_life_risks, tabel yang menyimpan kartu risiko kehidupan, lingkup sasaran, dan efeknya; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 2474: Mengevaluasi ekspresi ruleset_life_risks ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2475: Menempatkan ruleset_life_risks.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2476: Menempatkan ruleset_life_risks.ruleset_game_asset_id (UUID aset pada katalog induk) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2477: Menempatkan ruleset_life_risks.risk_code (kode kartu risiko kehidupan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2478: Menempatkan ruleset_life_risks.item_name (nama item dalam katalog) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2479: Menempatkan ruleset_life_risks.effect_type (jenis efek risiko) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2480: Menempatkan ruleset_life_risks.direction (arah efek masuk atau keluar) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2481: Menempatkan ruleset_life_risks.amount (nominal efek atau jumlah sesuai jenis aksi) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2482: Menempatkan ruleset_life_risks.duration_days (nilai duration days) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2483: Menempatkan ruleset_life_risks.target_scope (lingkup sasaran efek) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2484: Menempatkan ruleset_life_risks.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2485: Menempatkan ruleset_life_risks.card_qty (jumlah salinan kartu fisik yang tersedia) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2486: Menempatkan ruleset_life_risks.payload_json (rincian domain tambahan dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2487: Menutup kelompok yang terkait ; insert into ruleset_life_risks pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2488: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 2489: Memakai sr.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2490: Memakai rga.ruleset_game_asset_id (UUID aset pada katalog induk) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2491: Memakai extra.risk_code (kode kartu risiko kehidupan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2492: Memakai extra.item_name (nama item dalam katalog) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2493: Memakai extra.effect_type (jenis efek risiko) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2494: Memakai extra.direction (arah efek masuk atau keluar) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2495: Memakai extra.amount (nominal efek atau jumlah sesuai jenis aksi) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2496: Memakai extra.duration_days (nilai duration days) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2497: Memakai extra.target_scope (lingkup sasaran efek) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2498: Memakai extra.sort_order (urutan tampilan atau evaluasi komponen) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2499: Memakai extra.card_qty (jumlah salinan kartu fisik yang tersedia) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2500: Memakai extra.payload_json (rincian domain tambahan dalam JSONB) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2501: Menetapkan sumber baris ( values ( ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid, ’risk_beli_peralatan_dapur’, ’Beli Peralatan Dapur’, yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 2502: Membuka kelompok yang terkait extra . payload_json from pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2503: Menyediakan 23 tuple nilai eksplisit untuk extra; setiap tuple membentuk satu rekaman dan mengikuti urutan ruleset_version_id, risk_code, item_name, effect_type, direction, amount, duration_days, target_scope, sort_order, card_qty, payload_json.
-- Uraian sumber 2504: Membuka rekaman seed extra, rekaman ke-1; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2505: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-1.
-- Uraian sumber 2506: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_beli_peralatan_dapur’ untuk extra, rekaman ke-1.
-- Uraian sumber 2507: Mengisi item_name (nama item dalam katalog) dengan ’Beli Peralatan Dapur’ untuk extra, rekaman ke-1.
-- Uraian sumber 2508: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-1.
-- Uraian sumber 2509: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-1.
-- Uraian sumber 2510: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 3 untuk extra, rekaman ke-1.
-- Uraian sumber 2511: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-1.
-- Uraian sumber 2512: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-1.
-- Uraian sumber 2513: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 272 untuk extra, rekaman ke-1.
-- Uraian sumber 2514: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-1.
-- Uraian sumber 2515: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”OUT”,”amount”:3,”target_scope”:”SELF”}’ :: jsonb untuk extra, rekaman ke-1. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=3 (nominal efek atau jumlah sesuai jenis aksi); target_scope=”SELF” (lingkup sasaran efek).
-- Uraian sumber 2516: Menutup rekaman seed extra, rekaman ke-1; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2517: Membuka rekaman seed extra, rekaman ke-2; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2518: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-2.
-- Uraian sumber 2519: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_menang_undian’ untuk extra, rekaman ke-2.
-- Uraian sumber 2520: Mengisi item_name (nama item dalam katalog) dengan ’Menang Undian’ untuk extra, rekaman ke-2.
-- Uraian sumber 2521: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-2.
-- Uraian sumber 2522: Mengisi direction (arah efek masuk atau keluar) dengan ’IN’ untuk extra, rekaman ke-2.
-- Uraian sumber 2523: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 2 untuk extra, rekaman ke-2.
-- Uraian sumber 2524: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-2.
-- Uraian sumber 2525: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-2.
-- Uraian sumber 2526: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 273 untuk extra, rekaman ke-2.
-- Uraian sumber 2527: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-2.
-- Uraian sumber 2528: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”IN”,”amount”:2}’ :: jsonb untuk extra, rekaman ke-2. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”IN” (arah efek masuk atau keluar); amount=2 (nominal efek atau jumlah sesuai jenis aksi).
-- Uraian sumber 2529: Menutup rekaman seed extra, rekaman ke-2; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2530: Membuka rekaman seed extra, rekaman ke-3; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2531: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-3.
-- Uraian sumber 2532: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_study_tour’ untuk extra, rekaman ke-3.
-- Uraian sumber 2533: Mengisi item_name (nama item dalam katalog) dengan ’Study Tour’ untuk extra, rekaman ke-3.
-- Uraian sumber 2534: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-3.
-- Uraian sumber 2535: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-3.
-- Uraian sumber 2536: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 4 untuk extra, rekaman ke-3.
-- Uraian sumber 2537: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-3.
-- Uraian sumber 2538: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-3.
-- Uraian sumber 2539: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 274 untuk extra, rekaman ke-3.
-- Uraian sumber 2540: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-3.
-- Uraian sumber 2541: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”OUT”,”amount”:4}’ :: jsonb untuk extra, rekaman ke-3. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=4 (nominal efek atau jumlah sesuai jenis aksi).
-- Uraian sumber 2542: Menutup rekaman seed extra, rekaman ke-3; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2543: Membuka rekaman seed extra, rekaman ke-4; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2544: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-4.
-- Uraian sumber 2545: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_depresi’ untuk extra, rekaman ke-4.
-- Uraian sumber 2546: Mengisi item_name (nama item dalam katalog) dengan ’Depresi’ untuk extra, rekaman ke-4.
-- Uraian sumber 2547: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-4.
-- Uraian sumber 2548: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-4.
-- Uraian sumber 2549: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 3 untuk extra, rekaman ke-4.
-- Uraian sumber 2550: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-4.
-- Uraian sumber 2551: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-4.
-- Uraian sumber 2552: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 275 untuk extra, rekaman ke-4.
-- Uraian sumber 2553: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-4.
-- Uraian sumber 2554: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”OUT”,”amount”:3}’ :: jsonb untuk extra, rekaman ke-4. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=3 (nominal efek atau jumlah sesuai jenis aksi).
-- Uraian sumber 2555: Menutup rekaman seed extra, rekaman ke-4; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2556: Membuka rekaman seed extra, rekaman ke-5; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2557: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-5.
-- Uraian sumber 2558: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_panen_melimpah’ untuk extra, rekaman ke-5.
-- Uraian sumber 2559: Mengisi item_name (nama item dalam katalog) dengan ’Panen Melimpah’ untuk extra, rekaman ke-5.
-- Uraian sumber 2560: Mengisi effect_type (jenis efek risiko) dengan ’INGREDIENT_PRICE_MODIFIER’ untuk extra, rekaman ke-5.
-- Uraian sumber 2561: Mengisi direction (arah efek masuk atau keluar) dengan null untuk extra, rekaman ke-5. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 2562: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 0 untuk extra, rekaman ke-5.
-- Uraian sumber 2563: Mengisi duration_days (nilai duration days) dengan 7 untuk extra, rekaman ke-5.
-- Uraian sumber 2564: Mengisi target_scope (lingkup sasaran efek) dengan ’ALL_PLAYERS’ untuk extra, rekaman ke-5.
-- Uraian sumber 2565: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 276 untuk extra, rekaman ke-5.
-- Uraian sumber 2566: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-5.
-- Uraian sumber 2567: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”INGREDIENT_PRICE_MODIFIER”,”target_scope”:”ALL_PLAYERS”,”value_delta”:-1,”duration_days”:7}’ :: jsonb untuk extra, rekaman ke-5. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”INGREDIENT_PRICE_MODIFIER” (jenis efek risiko); target_scope=”ALL_PLAYERS” (lingkup sasaran efek); value_delta=-1 (nilai value delta); duration_days=7 (nilai duration days).
-- Uraian sumber 2568: Menutup rekaman seed extra, rekaman ke-5; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2569: Membuka rekaman seed extra, rekaman ke-6; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2570: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-6.
-- Uraian sumber 2571: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_ekstrakurikuler_anak’ untuk extra, rekaman ke-6.
-- Uraian sumber 2572: Mengisi item_name (nama item dalam katalog) dengan ’Ekstrakurikuler Anak’ untuk extra, rekaman ke-6.
-- Uraian sumber 2573: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-6.
-- Uraian sumber 2574: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-6.
-- Uraian sumber 2575: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 3 untuk extra, rekaman ke-6.
-- Uraian sumber 2576: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-6.
-- Uraian sumber 2577: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-6.
-- Uraian sumber 2578: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 277 untuk extra, rekaman ke-6.
-- Uraian sumber 2579: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-6.
-- Uraian sumber 2580: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”OUT”,”amount”:3}’ :: jsonb untuk extra, rekaman ke-6. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=3 (nominal efek atau jumlah sesuai jenis aksi).
-- Uraian sumber 2581: Menutup rekaman seed extra, rekaman ke-6; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2582: Membuka rekaman seed extra, rekaman ke-7; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2583: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-7.
-- Uraian sumber 2584: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_ban_bocor’ untuk extra, rekaman ke-7.
-- Uraian sumber 2585: Mengisi item_name (nama item dalam katalog) dengan ’Ban Bocor’ untuk extra, rekaman ke-7.
-- Uraian sumber 2586: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-7.
-- Uraian sumber 2587: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-7.
-- Uraian sumber 2588: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 3 untuk extra, rekaman ke-7.
-- Uraian sumber 2589: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-7.
-- Uraian sumber 2590: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-7.
-- Uraian sumber 2591: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 278 untuk extra, rekaman ke-7.
-- Uraian sumber 2592: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-7.
-- Uraian sumber 2593: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”OUT”,”amount”:3}’ :: jsonb untuk extra, rekaman ke-7. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=3 (nominal efek atau jumlah sesuai jenis aksi).
-- Uraian sumber 2594: Menutup rekaman seed extra, rekaman ke-7; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2595: Membuka rekaman seed extra, rekaman ke-8; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2596: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-8.
-- Uraian sumber 2597: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_sakit_gigi’ untuk extra, rekaman ke-8.
-- Uraian sumber 2598: Mengisi item_name (nama item dalam katalog) dengan ’Sakit Gigi’ untuk extra, rekaman ke-8.
-- Uraian sumber 2599: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-8.
-- Uraian sumber 2600: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-8.
-- Uraian sumber 2601: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 4 untuk extra, rekaman ke-8.
-- Uraian sumber 2602: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-8.
-- Uraian sumber 2603: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-8.
-- Uraian sumber 2604: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 279 untuk extra, rekaman ke-8.
-- Uraian sumber 2605: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-8.
-- Uraian sumber 2606: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”OUT”,”amount”:4,”target_scope”:”SELF”}’ :: jsonb untuk extra, rekaman ke-8. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=4 (nominal efek atau jumlah sesuai jenis aksi); target_scope=”SELF” (lingkup sasaran efek).
-- Uraian sumber 2607: Menutup rekaman seed extra, rekaman ke-8; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2608: Membuka rekaman seed extra, rekaman ke-9; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2609: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-9.
-- Uraian sumber 2610: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_operasi_usus_buntu’ untuk extra, rekaman ke-9.
-- Uraian sumber 2611: Mengisi item_name (nama item dalam katalog) dengan ’Operasi Usus Buntu’ untuk extra, rekaman ke-9.
-- Uraian sumber 2612: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-9.
-- Uraian sumber 2613: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-9.
-- Uraian sumber 2614: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 6 untuk extra, rekaman ke-9.
-- Uraian sumber 2615: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-9.
-- Uraian sumber 2616: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-9.
-- Uraian sumber 2617: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 280 untuk extra, rekaman ke-9.
-- Uraian sumber 2618: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-9.
-- Uraian sumber 2619: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”OUT”,”amount”:6,”target_scope”:”SELF”}’ :: jsonb untuk extra, rekaman ke-9. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=6 (nominal efek atau jumlah sesuai jenis aksi); target_scope=”SELF” (lingkup sasaran efek).
-- Uraian sumber 2620: Menutup rekaman seed extra, rekaman ke-9; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2621: Membuka rekaman seed extra, rekaman ke-10; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2622: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-10.
-- Uraian sumber 2623: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_ganti_oli’ untuk extra, rekaman ke-10.
-- Uraian sumber 2624: Mengisi item_name (nama item dalam katalog) dengan ’Ganti Oli’ untuk extra, rekaman ke-10.
-- Uraian sumber 2625: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-10.
-- Uraian sumber 2626: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-10.
-- Uraian sumber 2627: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 3 untuk extra, rekaman ke-10.
-- Uraian sumber 2628: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-10.
-- Uraian sumber 2629: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-10.
-- Uraian sumber 2630: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 281 untuk extra, rekaman ke-10.
-- Uraian sumber 2631: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-10.
-- Uraian sumber 2632: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”OUT”,”amount”:3,”target_scope”:”SELF”}’ :: jsonb untuk extra, rekaman ke-10. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=3 (nominal efek atau jumlah sesuai jenis aksi); target_scope=”SELF” (lingkup sasaran efek).
-- Uraian sumber 2633: Menutup rekaman seed extra, rekaman ke-10; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2634: Membuka rekaman seed extra, rekaman ke-11; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2635: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-11.
-- Uraian sumber 2636: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_mobil_tabrakan’ untuk extra, rekaman ke-11.
-- Uraian sumber 2637: Mengisi item_name (nama item dalam katalog) dengan ’Mobil Tabrakan’ untuk extra, rekaman ke-11.
-- Uraian sumber 2638: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-11.
-- Uraian sumber 2639: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-11.
-- Uraian sumber 2640: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 6 untuk extra, rekaman ke-11.
-- Uraian sumber 2641: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-11.
-- Uraian sumber 2642: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-11.
-- Uraian sumber 2643: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 282 untuk extra, rekaman ke-11.
-- Uraian sumber 2644: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-11.
-- Uraian sumber 2645: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”OUT”,”amount”:6,”target_scope”:”SELF”}’ :: jsonb untuk extra, rekaman ke-11. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=6 (nominal efek atau jumlah sesuai jenis aksi); target_scope=”SELF” (lingkup sasaran efek).
-- Uraian sumber 2646: Menutup rekaman seed extra, rekaman ke-11; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2647: Membuka rekaman seed extra, rekaman ke-12; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2648: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-12.
-- Uraian sumber 2649: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_pemadaman_listrik’ untuk extra, rekaman ke-12.
-- Uraian sumber 2650: Mengisi item_name (nama item dalam katalog) dengan ’Pemadaman Listrik’ untuk extra, rekaman ke-12.
-- Uraian sumber 2651: Mengisi effect_type (jenis efek risiko) dengan ’ALL_PLAYERS_COIN_EFFECT’ untuk extra, rekaman ke-12.
-- Uraian sumber 2652: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-12.
-- Uraian sumber 2653: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 3 untuk extra, rekaman ke-12.
-- Uraian sumber 2654: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-12.
-- Uraian sumber 2655: Mengisi target_scope (lingkup sasaran efek) dengan ’ALL_PLAYERS’ untuk extra, rekaman ke-12.
-- Uraian sumber 2656: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 283 untuk extra, rekaman ke-12.
-- Uraian sumber 2657: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-12.
-- Uraian sumber 2658: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”ALL_PLAYERS_COIN_EFFECT”,”direction”:”OUT”,”amount”:3,”target_scope”:”ALL_PLAYERS”}’ :: jsonb untuk extra, rekaman ke-12. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”ALL_PLAYERS_COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=3 (nominal efek atau jumlah sesuai jenis aksi); target_scope=”ALL_PLAYERS” (lingkup sasaran efek).
-- Uraian sumber 2659: Menutup rekaman seed extra, rekaman ke-12; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2660: Membuka rekaman seed extra, rekaman ke-13; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2661: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-13.
-- Uraian sumber 2662: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_ganti_aki’ untuk extra, rekaman ke-13.
-- Uraian sumber 2663: Mengisi item_name (nama item dalam katalog) dengan ’Ganti Aki’ untuk extra, rekaman ke-13.
-- Uraian sumber 2664: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-13.
-- Uraian sumber 2665: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-13.
-- Uraian sumber 2666: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 3 untuk extra, rekaman ke-13.
-- Uraian sumber 2667: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-13.
-- Uraian sumber 2668: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-13.
-- Uraian sumber 2669: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 284 untuk extra, rekaman ke-13.
-- Uraian sumber 2670: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-13.
-- Uraian sumber 2671: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”OUT”,”amount”:3}’ :: jsonb untuk extra, rekaman ke-13. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=3 (nominal efek atau jumlah sesuai jenis aksi).
-- Uraian sumber 2672: Menutup rekaman seed extra, rekaman ke-13; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2673: Membuka rekaman seed extra, rekaman ke-14; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2674: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-14.
-- Uraian sumber 2675: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_bbm_naik’ untuk extra, rekaman ke-14.
-- Uraian sumber 2676: Mengisi item_name (nama item dalam katalog) dengan ’BBM Naik’ untuk extra, rekaman ke-14.
-- Uraian sumber 2677: Mengisi effect_type (jenis efek risiko) dengan ’INGREDIENT_PRICE_MODIFIER’ untuk extra, rekaman ke-14.
-- Uraian sumber 2678: Mengisi direction (arah efek masuk atau keluar) dengan null untuk extra, rekaman ke-14. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 2679: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 0 untuk extra, rekaman ke-14.
-- Uraian sumber 2680: Mengisi duration_days (nilai duration days) dengan 7 untuk extra, rekaman ke-14.
-- Uraian sumber 2681: Mengisi target_scope (lingkup sasaran efek) dengan ’ALL_PLAYERS’ untuk extra, rekaman ke-14.
-- Uraian sumber 2682: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 285 untuk extra, rekaman ke-14.
-- Uraian sumber 2683: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-14.
-- Uraian sumber 2684: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”INGREDIENT_PRICE_MODIFIER”,”target_scope”:”ALL_PLAYERS”,”value_delta”:1,”duration_days”:7}’ :: jsonb untuk extra, rekaman ke-14. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”INGREDIENT_PRICE_MODIFIER” (jenis efek risiko); target_scope=”ALL_PLAYERS” (lingkup sasaran efek); value_delta=1 (nilai value delta); duration_days=7 (nilai duration days).
-- Uraian sumber 2685: Menutup rekaman seed extra, rekaman ke-14; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2686: Membuka rekaman seed extra, rekaman ke-15; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2687: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-15.
-- Uraian sumber 2688: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_kecelakaan’ untuk extra, rekaman ke-15.
-- Uraian sumber 2689: Mengisi item_name (nama item dalam katalog) dengan ’Kecelakaan’ untuk extra, rekaman ke-15.
-- Uraian sumber 2690: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-15.
-- Uraian sumber 2691: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-15.
-- Uraian sumber 2692: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 5 untuk extra, rekaman ke-15.
-- Uraian sumber 2693: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-15.
-- Uraian sumber 2694: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-15.
-- Uraian sumber 2695: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 286 untuk extra, rekaman ke-15.
-- Uraian sumber 2696: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-15.
-- Uraian sumber 2697: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”OUT”,”amount”:5,”target_scope”:”SELF”}’ :: jsonb untuk extra, rekaman ke-15. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=5 (nominal efek atau jumlah sesuai jenis aksi); target_scope=”SELF” (lingkup sasaran efek).
-- Uraian sumber 2698: Menutup rekaman seed extra, rekaman ke-15; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2699: Membuka rekaman seed extra, rekaman ke-16; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2700: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-16.
-- Uraian sumber 2701: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_sakit_perut’ untuk extra, rekaman ke-16.
-- Uraian sumber 2702: Mengisi item_name (nama item dalam katalog) dengan ’Sakit Perut’ untuk extra, rekaman ke-16.
-- Uraian sumber 2703: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-16.
-- Uraian sumber 2704: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-16.
-- Uraian sumber 2705: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 3 untuk extra, rekaman ke-16.
-- Uraian sumber 2706: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-16.
-- Uraian sumber 2707: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-16.
-- Uraian sumber 2708: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 287 untuk extra, rekaman ke-16.
-- Uraian sumber 2709: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-16.
-- Uraian sumber 2710: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”OUT”,”amount”:3}’ :: jsonb untuk extra, rekaman ke-16. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=3 (nominal efek atau jumlah sesuai jenis aksi).
-- Uraian sumber 2711: Menutup rekaman seed extra, rekaman ke-16; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2712: Membuka rekaman seed extra, rekaman ke-17; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2713: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-17.
-- Uraian sumber 2714: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_sakit_asam_lambung’ untuk extra, rekaman ke-17.
-- Uraian sumber 2715: Mengisi item_name (nama item dalam katalog) dengan ’Sakit Asam Lambung’ untuk extra, rekaman ke-17.
-- Uraian sumber 2716: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-17.
-- Uraian sumber 2717: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-17.
-- Uraian sumber 2718: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 4 untuk extra, rekaman ke-17.
-- Uraian sumber 2719: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-17.
-- Uraian sumber 2720: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-17.
-- Uraian sumber 2721: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 288 untuk extra, rekaman ke-17.
-- Uraian sumber 2722: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-17.
-- Uraian sumber 2723: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”OUT”,”amount”:4}’ :: jsonb untuk extra, rekaman ke-17. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=4 (nominal efek atau jumlah sesuai jenis aksi).
-- Uraian sumber 2724: Menutup rekaman seed extra, rekaman ke-17; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2725: Membuka rekaman seed extra, rekaman ke-18; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2726: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-18.
-- Uraian sumber 2727: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_wisuda_kelulusan’ untuk extra, rekaman ke-18.
-- Uraian sumber 2728: Mengisi item_name (nama item dalam katalog) dengan ’Wisuda Kelulusan’ untuk extra, rekaman ke-18.
-- Uraian sumber 2729: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-18.
-- Uraian sumber 2730: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-18.
-- Uraian sumber 2731: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 6 untuk extra, rekaman ke-18.
-- Uraian sumber 2732: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-18.
-- Uraian sumber 2733: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-18.
-- Uraian sumber 2734: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 289 untuk extra, rekaman ke-18.
-- Uraian sumber 2735: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-18.
-- Uraian sumber 2736: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”OUT”,”amount”:6,”target_scope”:”SELF”}’ :: jsonb untuk extra, rekaman ke-18. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=6 (nominal efek atau jumlah sesuai jenis aksi); target_scope=”SELF” (lingkup sasaran efek).
-- Uraian sumber 2737: Menutup rekaman seed extra, rekaman ke-18; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2738: Membuka rekaman seed extra, rekaman ke-19; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2739: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-19.
-- Uraian sumber 2740: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_bencana_banjir’ untuk extra, rekaman ke-19.
-- Uraian sumber 2741: Mengisi item_name (nama item dalam katalog) dengan ’Bencana Banjir’ untuk extra, rekaman ke-19.
-- Uraian sumber 2742: Mengisi effect_type (jenis efek risiko) dengan ’ALL_PLAYERS_COIN_EFFECT’ untuk extra, rekaman ke-19.
-- Uraian sumber 2743: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-19.
-- Uraian sumber 2744: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 3 untuk extra, rekaman ke-19.
-- Uraian sumber 2745: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-19.
-- Uraian sumber 2746: Mengisi target_scope (lingkup sasaran efek) dengan ’ALL_PLAYERS’ untuk extra, rekaman ke-19.
-- Uraian sumber 2747: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 290 untuk extra, rekaman ke-19.
-- Uraian sumber 2748: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-19.
-- Uraian sumber 2749: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”ALL_PLAYERS_COIN_EFFECT”,”direction”:”OUT”,”amount”:3,”target_scope”:”ALL_PLAYERS”}’ :: jsonb untuk extra, rekaman ke-19. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”ALL_PLAYERS_COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=3 (nominal efek atau jumlah sesuai jenis aksi); target_scope=”ALL_PLAYERS” (lingkup sasaran efek).
-- Uraian sumber 2750: Menutup rekaman seed extra, rekaman ke-19; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2751: Membuka rekaman seed extra, rekaman ke-20; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2752: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-20.
-- Uraian sumber 2753: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_gudang_terbakar’ untuk extra, rekaman ke-20.
-- Uraian sumber 2754: Mengisi item_name (nama item dalam katalog) dengan ’Gudang Terbakar’ untuk extra, rekaman ke-20.
-- Uraian sumber 2755: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-20.
-- Uraian sumber 2756: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-20.
-- Uraian sumber 2757: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 6 untuk extra, rekaman ke-20.
-- Uraian sumber 2758: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-20.
-- Uraian sumber 2759: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-20.
-- Uraian sumber 2760: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 291 untuk extra, rekaman ke-20.
-- Uraian sumber 2761: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-20.
-- Uraian sumber 2762: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”OUT”,”amount”:6}’ :: jsonb untuk extra, rekaman ke-20. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=6 (nominal efek atau jumlah sesuai jenis aksi).
-- Uraian sumber 2763: Menutup rekaman seed extra, rekaman ke-20; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2764: Membuka rekaman seed extra, rekaman ke-21; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2765: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-21.
-- Uraian sumber 2766: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_investasi_emas’ untuk extra, rekaman ke-21.
-- Uraian sumber 2767: Mengisi item_name (nama item dalam katalog) dengan ’Investasi Emas’ untuk extra, rekaman ke-21.
-- Uraian sumber 2768: Mengisi effect_type (jenis efek risiko) dengan ’GOLD_TRADE’ untuk extra, rekaman ke-21.
-- Uraian sumber 2769: Mengisi direction (arah efek masuk atau keluar) dengan null untuk extra, rekaman ke-21. NULL menyatakan tidak ada nilai atau relasi pada rekaman ini, bukan angka nol atau teks kosong.
-- Uraian sumber 2770: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 0 untuk extra, rekaman ke-21.
-- Uraian sumber 2771: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-21.
-- Uraian sumber 2772: Mengisi target_scope (lingkup sasaran efek) dengan ’ALL_PLAYERS’ untuk extra, rekaman ke-21.
-- Uraian sumber 2773: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 292 untuk extra, rekaman ke-21.
-- Uraian sumber 2774: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 2 untuk extra, rekaman ke-21.
-- Uraian sumber 2775: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”GOLD_TRADE”,”target_scope”:”ALL_PLAYERS”,”opens_gold_trade”:true}’ :: jsonb untuk extra, rekaman ke-21. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”GOLD_TRADE” (jenis efek risiko); target_scope=”ALL_PLAYERS” (lingkup sasaran efek); opens_gold_trade=true (nilai opens gold trade).
-- Uraian sumber 2776: Menutup rekaman seed extra, rekaman ke-21; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2777: Membuka rekaman seed extra, rekaman ke-22; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2778: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-22.
-- Uraian sumber 2779: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_tahun_ajaran_baru’ untuk extra, rekaman ke-22.
-- Uraian sumber 2780: Mengisi item_name (nama item dalam katalog) dengan ’Tahun Ajaran Baru’ untuk extra, rekaman ke-22.
-- Uraian sumber 2781: Mengisi effect_type (jenis efek risiko) dengan ’COIN_EFFECT’ untuk extra, rekaman ke-22.
-- Uraian sumber 2782: Mengisi direction (arah efek masuk atau keluar) dengan ’OUT’ untuk extra, rekaman ke-22.
-- Uraian sumber 2783: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 5 untuk extra, rekaman ke-22.
-- Uraian sumber 2784: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-22.
-- Uraian sumber 2785: Mengisi target_scope (lingkup sasaran efek) dengan ’SELF’ untuk extra, rekaman ke-22.
-- Uraian sumber 2786: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 293 untuk extra, rekaman ke-22.
-- Uraian sumber 2787: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-22.
-- Uraian sumber 2788: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”COIN_EFFECT”,”direction”:”OUT”,”amount”:5,”target_scope”:”SELF”}’ :: jsonb untuk extra, rekaman ke-22. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”COIN_EFFECT” (jenis efek risiko); direction=”OUT” (arah efek masuk atau keluar); amount=5 (nominal efek atau jumlah sesuai jenis aksi); target_scope=”SELF” (lingkup sasaran efek).
-- Uraian sumber 2789: Menutup rekaman seed extra, rekaman ke-22; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2790: Membuka rekaman seed extra, rekaman ke-23; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2791: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk extra, rekaman ke-23.
-- Uraian sumber 2792: Mengisi risk_code (kode kartu risiko kehidupan) dengan ’risk_ulang_tahun’ untuk extra, rekaman ke-23.
-- Uraian sumber 2793: Mengisi item_name (nama item dalam katalog) dengan ’Ulang Tahun’ untuk extra, rekaman ke-23.
-- Uraian sumber 2794: Mengisi effect_type (jenis efek risiko) dengan ’PLAYER_TO_PLAYER_TRANSFER’ untuk extra, rekaman ke-23.
-- Uraian sumber 2795: Mengisi direction (arah efek masuk atau keluar) dengan ’IN’ untuk extra, rekaman ke-23.
-- Uraian sumber 2796: Mengisi amount (nominal efek atau jumlah sesuai jenis aksi) dengan 1 untuk extra, rekaman ke-23.
-- Uraian sumber 2797: Mengisi duration_days (nilai duration days) dengan 1 untuk extra, rekaman ke-23.
-- Uraian sumber 2798: Mengisi target_scope (lingkup sasaran efek) dengan ’OTHER_PLAYERS’ untuk extra, rekaman ke-23.
-- Uraian sumber 2799: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 294 untuk extra, rekaman ke-23.
-- Uraian sumber 2800: Mengisi card_qty (jumlah salinan kartu fisik yang tersedia) dengan 1 untuk extra, rekaman ke-23.
-- Uraian sumber 2801: Mengisi payload_json (rincian domain tambahan dalam JSONB) dengan ’{”effect_type”:”PLAYER_TO_PLAYER_TRANSFER”,”direction”:”IN”,”amount”:1,”target_scope”:”OTHER_PLAYERS”}’ :: jsonb untuk extra, rekaman ke-23. Isi JSON dipakai sebagai parameter aksi/metadata: effect_type=”PLAYER_TO_PLAYER_TRANSFER” (jenis efek risiko); direction=”IN” (arah efek masuk atau keluar); amount=1 (nominal efek atau jumlah sesuai jenis aksi); target_scope=”OTHER_PLAYERS” (lingkup sasaran efek).
-- Uraian sumber 2802: Menutup rekaman seed extra, rekaman ke-23; kueri melanjutkan pemrosesan kumpulan nilai yang telah lengkap.
-- Uraian sumber 2803: Mengevaluasi ekspresi ) as extra( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. memberi nama hasil extra (nilai extra).
-- Uraian sumber 2804: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2805: Memakai risk_code (kode kartu risiko kehidupan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2806: Memakai item_name (nama item dalam katalog) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2807: Memakai effect_type (jenis efek risiko) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2808: Memakai direction (arah efek masuk atau keluar) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2809: Memakai amount (nominal efek atau jumlah sesuai jenis aksi) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2810: Memakai duration_days (nilai duration days) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2811: Memakai target_scope (lingkup sasaran efek) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2812: Memakai sort_order (urutan tampilan atau evaluasi komponen) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2813: Memakai card_qty (jumlah salinan kartu fisik yang tersedia) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2814: Memakai payload_json (rincian domain tambahan dalam JSONB) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2815: Menutup kelompok yang terkait ) ) as extra pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2816: Menggabungkan sumber join seed_rulesets sr on sr.ruleset_version_id = extra.ruleset_version_id dalam skrip basis data. JOIN memasangkan baris yang memenuhi syarat ON sehingga identitas aturan, sesi, atau aset tetap selaras.
-- Uraian sumber 2817: Menggabungkan sumber join ruleset_game_assets rga on rga.ruleset_version_id = sr.ruleset_version_id dalam skrip basis data. JOIN memasangkan baris yang memenuhi syarat ON sehingga identitas aturan, sesi, atau aset tetap selaras.
-- Uraian sumber 2818: Menambahkan syarat wajib rga.asset_type = ’RISK’ pada skrip basis data; semua predikat yang dihubungkan AND harus bernilai TRUE untuk lolos.
-- Uraian sumber 2819: Menetapkan penanganan konflik kunci unik pada INSERT: and rga.asset_code = extra.risk_code on conflict (ruleset_version_id, risk_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 2820: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 2821: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 2822: Menetapkan atau membandingkan item_name (nama item dalam katalog) terhadap excluded.item_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2823: Menetapkan atau membandingkan effect_type (jenis efek risiko) terhadap excluded.effect_type, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2824: Menetapkan atau membandingkan direction (arah efek masuk atau keluar) terhadap excluded.direction, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2825: Menetapkan atau membandingkan amount (nominal efek atau jumlah sesuai jenis aksi) terhadap excluded.amount, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2826: Menetapkan atau membandingkan duration_days (nilai duration days) terhadap excluded.duration_days, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2827: Menetapkan atau membandingkan target_scope (lingkup sasaran efek) terhadap excluded.target_scope, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2828: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2829: Menetapkan atau membandingkan card_qty (jumlah salinan kartu fisik yang tersedia) terhadap excluded.card_qty, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2830: Menetapkan atau membandingkan payload_json (rincian domain tambahan dalam JSONB) terhadap excluded.payload_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2834: Memulai penyisipan rekaman ke ruleset_game_assets, tabel yang menjadi katalog induk seluruh aset atau kartu yang dirujuk aturan dan posisi kartu sesi; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 2835: Mengevaluasi ekspresi ruleset_game_assets ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2836: Menempatkan ruleset_game_assets.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2837: Menempatkan ruleset_game_assets.asset_type (kategori aset atau kartu) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2838: Menempatkan ruleset_game_assets.asset_code (kode aset dalam versi aturan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2839: Menempatkan ruleset_game_assets.display_name (nama yang ditampilkan kepada pengguna) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2840: Menempatkan ruleset_game_assets.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2841: Menempatkan ruleset_game_assets.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2842: Menempatkan ruleset_game_assets.metadata_json (metadata tambahan aset dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2843: Menutup kelompok yang terkait ; insert into ruleset_game_assets pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2844: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 2845: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2846: Menyediakan nilai literal ’COLLECTION_MISSION’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 2847: Memakai mission_code (kode misi koleksi) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2848: Memakai item_name (nama item dalam katalog) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2849: Menyediakan nilai literal 500 + sort_order, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 2850: Memakai is_active (penanda apakah entitas masih boleh digunakan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2851: Mengevaluasi ekspresi jsonb_build_object(’source’, ’ruleset_collection_missions’) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 2852: Menetapkan sumber baris ruleset_collection_missions on conflict (ruleset_version_id, asset_type, asset_code) do update set display_name = excluded.display_name, sort_order = exclude... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 2853: Menetapkan penanganan konflik kunci unik pada INSERT: ruleset_collection_missions on conflict (ruleset_version_id, asset_type, asset_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 2854: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 2855: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 2856: Menetapkan atau membandingkan display_name (nama yang ditampilkan kepada pengguna) terhadap excluded.display_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2857: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2858: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2859: Menetapkan atau membandingkan metadata_json (metadata tambahan aset dalam JSONB) terhadap excluded.metadata_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2862: Memulai penyisipan rekaman ke ruleset_game_assets, tabel yang menjadi katalog induk seluruh aset atau kartu yang dirujuk aturan dan posisi kartu sesi; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 2863: Mengevaluasi ekspresi ruleset_game_assets ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2864: Menempatkan ruleset_game_assets.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2865: Menempatkan ruleset_game_assets.asset_type (kategori aset atau kartu) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2866: Menempatkan ruleset_game_assets.asset_code (kode aset dalam versi aturan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2867: Menempatkan ruleset_game_assets.display_name (nama yang ditampilkan kepada pengguna) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2868: Menempatkan ruleset_game_assets.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2869: Menempatkan ruleset_game_assets.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2870: Menempatkan ruleset_game_assets.metadata_json (metadata tambahan aset dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2871: Menutup kelompok yang terkait ; insert into ruleset_game_assets pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2872: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 2873: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2874: Menyediakan nilai literal ’FINANCIAL_GOAL’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 2875: Memakai goal_code (kode tujuan finansial) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2876: Memakai item_name (nama item dalam katalog) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2877: Menyediakan nilai literal 600 + sort_order, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 2878: Memakai is_active (penanda apakah entitas masih boleh digunakan) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2879: Mengevaluasi ekspresi jsonb_build_object(’source’, ’ruleset_financial_goals’) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 2880: Menetapkan sumber baris ruleset_financial_goals on conflict (ruleset_version_id, asset_type, asset_code) do update set display_name = excluded.display_name, sort_order = excluded.so... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 2881: Menetapkan penanganan konflik kunci unik pada INSERT: ruleset_financial_goals on conflict (ruleset_version_id, asset_type, asset_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 2882: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 2883: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 2884: Menetapkan atau membandingkan display_name (nama yang ditampilkan kepada pengguna) terhadap excluded.display_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2885: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2886: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2887: Menetapkan atau membandingkan metadata_json (metadata tambahan aset dalam JSONB) terhadap excluded.metadata_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2890: Memulai penyisipan rekaman ke ruleset_game_assets, tabel yang menjadi katalog induk seluruh aset atau kartu yang dirujuk aturan dan posisi kartu sesi; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 2891: Mengevaluasi ekspresi ruleset_game_assets ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2892: Menempatkan ruleset_game_assets.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2893: Menempatkan ruleset_game_assets.asset_type (kategori aset atau kartu) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2894: Menempatkan ruleset_game_assets.asset_code (kode aset dalam versi aturan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2895: Menempatkan ruleset_game_assets.display_name (nama yang ditampilkan kepada pengguna) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2896: Menempatkan ruleset_game_assets.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2897: Menempatkan ruleset_game_assets.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2898: Menempatkan ruleset_game_assets.metadata_json (metadata tambahan aset dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2899: Menutup kelompok yang terkait ; insert into ruleset_game_assets pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2900: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 2901: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2902: Menyediakan nilai literal ’SHARIA_LOAN’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 2903: Memakai loan_code (kode produk pinjaman syariah) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2904: Memakai item_name (nama item dalam katalog) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2905: Menyediakan nilai literal 1000 + sort_order, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 2906: Memakai true (nilai true) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2907: Mengevaluasi ekspresi jsonb_build_object(’source’, ’ruleset_sharia_loans’) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 2908: Menetapkan sumber baris ruleset_sharia_loans on conflict (ruleset_version_id, asset_type, asset_code) do update set display_name = excluded.display_name, sort_order = excluded.sort_... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 2909: Menetapkan penanganan konflik kunci unik pada INSERT: ruleset_sharia_loans on conflict (ruleset_version_id, asset_type, asset_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 2910: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 2911: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 2912: Menetapkan atau membandingkan display_name (nama yang ditampilkan kepada pengguna) terhadap excluded.display_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2913: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2914: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2915: Menetapkan atau membandingkan metadata_json (metadata tambahan aset dalam JSONB) terhadap excluded.metadata_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2918: Memulai penyisipan rekaman ke ruleset_game_assets, tabel yang menjadi katalog induk seluruh aset atau kartu yang dirujuk aturan dan posisi kartu sesi; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 2919: Mengevaluasi ekspresi ruleset_game_assets ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2920: Menempatkan ruleset_game_assets.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2921: Menempatkan ruleset_game_assets.asset_type (kategori aset atau kartu) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2922: Menempatkan ruleset_game_assets.asset_code (kode aset dalam versi aturan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2923: Menempatkan ruleset_game_assets.display_name (nama yang ditampilkan kepada pengguna) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2924: Menempatkan ruleset_game_assets.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2925: Menempatkan ruleset_game_assets.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2926: Menempatkan ruleset_game_assets.metadata_json (metadata tambahan aset dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2927: Menutup kelompok yang terkait ; insert into ruleset_game_assets pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2928: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 2929: Memakai ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2930: Menyediakan nilai literal ’INSURANCE’, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 2931: Memakai product_code (kode produk asuransi) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2932: Memakai item_name (nama item dalam katalog) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2933: Menyediakan nilai literal 1100 + sort_order, dalam skrip basis data; nilai ini menjadi kandidat syarat, nilai hasil, atau argumen pada ekspresi pembungkus sesuai urutan yang tertulis.
-- Uraian sumber 2934: Memakai true (nilai true) sebagai unsur ekspresi atau daftar pada skrip basis data; posisinya mengikuti SELECT, syarat, urutan, atau argumen yang sedang dibentuk.
-- Uraian sumber 2935: Mengevaluasi ekspresi jsonb_build_object(’source’, ’ruleset_insurance_products’) pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. jsonb_build_object menyusun pasangan kunci dan nilai menjadi objek JSONB.
-- Uraian sumber 2936: Menetapkan sumber baris ruleset_insurance_products on conflict (ruleset_version_id, asset_type, asset_code) do update set display_name = excluded.display_name, sort_order = excluded... yang dibaca dalam skrip basis data; alias sumber menghubungkan referensi kolom pada SELECT, JOIN, dan filter.
-- Uraian sumber 2937: Menetapkan penanganan konflik kunci unik pada INSERT: ruleset_insurance_products on conflict (ruleset_version_id, asset_type, asset_code) do. DO NOTHING mempertahankan rekaman lama, sedangkan DO UPDATE memakai SET dan nilai EXCLUDED untuk menyinkronkan data seed.
-- Uraian sumber 2938: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 2939: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 2940: Menetapkan atau membandingkan display_name (nama yang ditampilkan kepada pengguna) terhadap excluded.display_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2941: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2942: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2943: Menetapkan atau membandingkan metadata_json (metadata tambahan aset dalam JSONB) terhadap excluded.metadata_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 2946: Memulai penyisipan rekaman ke ruleset_game_assets, tabel yang menjadi katalog induk seluruh aset atau kartu yang dirujuk aturan dan posisi kartu sesi; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 2947: Mengevaluasi ekspresi ruleset_game_assets ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 2948: Menempatkan ruleset_game_assets.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2949: Menempatkan ruleset_game_assets.asset_type (kategori aset atau kartu) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2950: Menempatkan ruleset_game_assets.asset_code (kode aset dalam versi aturan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2951: Menempatkan ruleset_game_assets.display_name (nama yang ditampilkan kepada pengguna) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2952: Menempatkan ruleset_game_assets.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2953: Menempatkan ruleset_game_assets.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2954: Menempatkan ruleset_game_assets.metadata_json (metadata tambahan aset dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 2955: Menutup kelompok yang terkait ; insert into ruleset_game_assets pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 2956: Menyediakan 6 tuple nilai eksplisit untuk ruleset_game_assets; setiap tuple membentuk satu rekaman dan mengikuti urutan ruleset_version_id, asset_type, asset_code, display_name, sort_order, is_active, metadata_json.
-- Uraian sumber 2957: Membuka rekaman seed asset_code=’donation_award_rank_1’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2958: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk asset_code=’donation_award_rank_1’.
-- Uraian sumber 2959: Mengisi asset_type (kategori aset atau kartu) dengan ’DONATION_AWARD’ untuk asset_code=’donation_award_rank_1’.
-- Uraian sumber 2960: Mengisi asset_code (kode aset dalam versi aturan) dengan ’donation_award_rank_1’ untuk asset_code=’donation_award_rank_1’.
-- Uraian sumber 2961: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Peringkat Donasi 1’ untuk asset_code=’donation_award_rank_1’.
-- Uraian sumber 2962: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1301 untuk asset_code=’donation_award_rank_1’.
-- Uraian sumber 2963: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’donation_award_rank_1’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 2964: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”card_qty”: 3}’ :: jsonb untuk asset_code=’donation_award_rank_1’. Isi JSON dipakai sebagai parameter aksi/metadata: card_qty=3 (jumlah salinan kartu fisik yang tersedia).
-- Uraian sumber 2965: Menutup rekaman seed asset_code=’donation_award_rank_1’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2966: Membuka rekaman seed asset_code=’donation_award_rank_2’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2967: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk asset_code=’donation_award_rank_2’.
-- Uraian sumber 2968: Mengisi asset_type (kategori aset atau kartu) dengan ’DONATION_AWARD’ untuk asset_code=’donation_award_rank_2’.
-- Uraian sumber 2969: Mengisi asset_code (kode aset dalam versi aturan) dengan ’donation_award_rank_2’ untuk asset_code=’donation_award_rank_2’.
-- Uraian sumber 2970: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Peringkat Donasi 2’ untuk asset_code=’donation_award_rank_2’.
-- Uraian sumber 2971: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1302 untuk asset_code=’donation_award_rank_2’.
-- Uraian sumber 2972: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’donation_award_rank_2’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 2973: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”card_qty”: 3}’ :: jsonb untuk asset_code=’donation_award_rank_2’. Isi JSON dipakai sebagai parameter aksi/metadata: card_qty=3 (jumlah salinan kartu fisik yang tersedia).
-- Uraian sumber 2974: Menutup rekaman seed asset_code=’donation_award_rank_2’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2975: Membuka rekaman seed asset_code=’donation_award_rank_3’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2976: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk asset_code=’donation_award_rank_3’.
-- Uraian sumber 2977: Mengisi asset_type (kategori aset atau kartu) dengan ’DONATION_AWARD’ untuk asset_code=’donation_award_rank_3’.
-- Uraian sumber 2978: Mengisi asset_code (kode aset dalam versi aturan) dengan ’donation_award_rank_3’ untuk asset_code=’donation_award_rank_3’.
-- Uraian sumber 2979: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Peringkat Donasi 3’ untuk asset_code=’donation_award_rank_3’.
-- Uraian sumber 2980: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1303 untuk asset_code=’donation_award_rank_3’.
-- Uraian sumber 2981: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’donation_award_rank_3’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 2982: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”card_qty”: 3}’ :: jsonb untuk asset_code=’donation_award_rank_3’. Isi JSON dipakai sebagai parameter aksi/metadata: card_qty=3 (jumlah salinan kartu fisik yang tersedia).
-- Uraian sumber 2983: Menutup rekaman seed asset_code=’donation_award_rank_3’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2984: Membuka rekaman seed asset_code=’donation_award_rank_1’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2985: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’donation_award_rank_1’.
-- Uraian sumber 2986: Mengisi asset_type (kategori aset atau kartu) dengan ’DONATION_AWARD’ untuk asset_code=’donation_award_rank_1’.
-- Uraian sumber 2987: Mengisi asset_code (kode aset dalam versi aturan) dengan ’donation_award_rank_1’ untuk asset_code=’donation_award_rank_1’.
-- Uraian sumber 2988: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Peringkat Donasi 1’ untuk asset_code=’donation_award_rank_1’.
-- Uraian sumber 2989: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1301 untuk asset_code=’donation_award_rank_1’.
-- Uraian sumber 2990: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’donation_award_rank_1’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 2991: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”card_qty”: 3}’ :: jsonb untuk asset_code=’donation_award_rank_1’. Isi JSON dipakai sebagai parameter aksi/metadata: card_qty=3 (jumlah salinan kartu fisik yang tersedia).
-- Uraian sumber 2992: Menutup rekaman seed asset_code=’donation_award_rank_1’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 2993: Membuka rekaman seed asset_code=’donation_award_rank_2’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 2994: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’donation_award_rank_2’.
-- Uraian sumber 2995: Mengisi asset_type (kategori aset atau kartu) dengan ’DONATION_AWARD’ untuk asset_code=’donation_award_rank_2’.
-- Uraian sumber 2996: Mengisi asset_code (kode aset dalam versi aturan) dengan ’donation_award_rank_2’ untuk asset_code=’donation_award_rank_2’.
-- Uraian sumber 2997: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Peringkat Donasi 2’ untuk asset_code=’donation_award_rank_2’.
-- Uraian sumber 2998: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1302 untuk asset_code=’donation_award_rank_2’.
-- Uraian sumber 2999: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’donation_award_rank_2’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 3000: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”card_qty”: 3}’ :: jsonb untuk asset_code=’donation_award_rank_2’. Isi JSON dipakai sebagai parameter aksi/metadata: card_qty=3 (jumlah salinan kartu fisik yang tersedia).
-- Uraian sumber 3001: Menutup rekaman seed asset_code=’donation_award_rank_2’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 3002: Membuka rekaman seed asset_code=’donation_award_rank_3’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 3003: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’donation_award_rank_3’.
-- Uraian sumber 3004: Mengisi asset_type (kategori aset atau kartu) dengan ’DONATION_AWARD’ untuk asset_code=’donation_award_rank_3’.
-- Uraian sumber 3005: Mengisi asset_code (kode aset dalam versi aturan) dengan ’donation_award_rank_3’ untuk asset_code=’donation_award_rank_3’.
-- Uraian sumber 3006: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Peringkat Donasi 3’ untuk asset_code=’donation_award_rank_3’.
-- Uraian sumber 3007: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1303 untuk asset_code=’donation_award_rank_3’.
-- Uraian sumber 3008: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’donation_award_rank_3’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 3009: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”card_qty”: 3}’ :: jsonb untuk asset_code=’donation_award_rank_3’. Isi JSON dipakai sebagai parameter aksi/metadata: card_qty=3 (jumlah salinan kartu fisik yang tersedia).
-- Uraian sumber 3010: Menutup rekaman seed asset_code=’donation_award_rank_3’; kueri melanjutkan pemrosesan kumpulan nilai yang telah lengkap.
-- Uraian sumber 3011: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 3012: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 3013: Menetapkan atau membandingkan display_name (nama yang ditampilkan kepada pengguna) terhadap excluded.display_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 3014: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 3015: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 3016: Menetapkan atau membandingkan metadata_json (metadata tambahan aset dalam JSONB) terhadap excluded.metadata_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 3019: Memulai penyisipan rekaman ke ruleset_game_assets, tabel yang menjadi katalog induk seluruh aset atau kartu yang dirujuk aturan dan posisi kartu sesi; kolom tujuan menentukan pemetaan nilai yang disediakan.
-- Uraian sumber 3020: Mengevaluasi ekspresi ruleset_game_assets ( pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk.
-- Uraian sumber 3021: Menempatkan ruleset_game_assets.ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 3022: Menempatkan ruleset_game_assets.asset_type (kategori aset atau kartu) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 3023: Menempatkan ruleset_game_assets.asset_code (kode aset dalam versi aturan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 3024: Menempatkan ruleset_game_assets.display_name (nama yang ditampilkan kepada pengguna) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 3025: Menempatkan ruleset_game_assets.sort_order (urutan tampilan atau evaluasi komponen) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 3026: Menempatkan ruleset_game_assets.is_active (penanda apakah entitas masih boleh digunakan) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 3027: Menempatkan ruleset_game_assets.metadata_json (metadata tambahan aset dalam JSONB) pada daftar tujuan INSERT; nilai SELECT atau VALUES harus mengikuti posisi kolom ini.
-- Uraian sumber 3028: Menutup kelompok yang terkait ; insert into ruleset_game_assets pada skrip basis data. Tanda kurung menjaga batas daftar dan prioritas evaluasi ekspresi.
-- Uraian sumber 3029: Menyediakan 6 tuple nilai eksplisit untuk ruleset_game_assets; setiap tuple membentuk satu rekaman dan mengikuti urutan ruleset_version_id, asset_type, asset_code, display_name, sort_order, is_active, metadata_json.
-- Uraian sumber 3030: Membuka rekaman seed asset_code=’pension_award_rank_1’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 3031: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk asset_code=’pension_award_rank_1’.
-- Uraian sumber 3032: Mengisi asset_type (kategori aset atau kartu) dengan ’PENSION_AWARD’ untuk asset_code=’pension_award_rank_1’.
-- Uraian sumber 3033: Mengisi asset_code (kode aset dalam versi aturan) dengan ’pension_award_rank_1’ untuk asset_code=’pension_award_rank_1’.
-- Uraian sumber 3034: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Peringkat Pensiun 1’ untuk asset_code=’pension_award_rank_1’.
-- Uraian sumber 3035: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1401 untuk asset_code=’pension_award_rank_1’.
-- Uraian sumber 3036: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’pension_award_rank_1’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 3037: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”card_qty”: 1}’ :: jsonb untuk asset_code=’pension_award_rank_1’. Isi JSON dipakai sebagai parameter aksi/metadata: card_qty=1 (jumlah salinan kartu fisik yang tersedia).
-- Uraian sumber 3038: Menutup rekaman seed asset_code=’pension_award_rank_1’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 3039: Membuka rekaman seed asset_code=’pension_award_rank_2’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 3040: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk asset_code=’pension_award_rank_2’.
-- Uraian sumber 3041: Mengisi asset_type (kategori aset atau kartu) dengan ’PENSION_AWARD’ untuk asset_code=’pension_award_rank_2’.
-- Uraian sumber 3042: Mengisi asset_code (kode aset dalam versi aturan) dengan ’pension_award_rank_2’ untuk asset_code=’pension_award_rank_2’.
-- Uraian sumber 3043: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Peringkat Pensiun 2’ untuk asset_code=’pension_award_rank_2’.
-- Uraian sumber 3044: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1402 untuk asset_code=’pension_award_rank_2’.
-- Uraian sumber 3045: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’pension_award_rank_2’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 3046: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”card_qty”: 1}’ :: jsonb untuk asset_code=’pension_award_rank_2’. Isi JSON dipakai sebagai parameter aksi/metadata: card_qty=1 (jumlah salinan kartu fisik yang tersedia).
-- Uraian sumber 3047: Menutup rekaman seed asset_code=’pension_award_rank_2’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 3048: Membuka rekaman seed asset_code=’pension_award_rank_3’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 3049: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid untuk asset_code=’pension_award_rank_3’.
-- Uraian sumber 3050: Mengisi asset_type (kategori aset atau kartu) dengan ’PENSION_AWARD’ untuk asset_code=’pension_award_rank_3’.
-- Uraian sumber 3051: Mengisi asset_code (kode aset dalam versi aturan) dengan ’pension_award_rank_3’ untuk asset_code=’pension_award_rank_3’.
-- Uraian sumber 3052: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Peringkat Pensiun 3’ untuk asset_code=’pension_award_rank_3’.
-- Uraian sumber 3053: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1403 untuk asset_code=’pension_award_rank_3’.
-- Uraian sumber 3054: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’pension_award_rank_3’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 3055: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”card_qty”: 1}’ :: jsonb untuk asset_code=’pension_award_rank_3’. Isi JSON dipakai sebagai parameter aksi/metadata: card_qty=1 (jumlah salinan kartu fisik yang tersedia).
-- Uraian sumber 3056: Menutup rekaman seed asset_code=’pension_award_rank_3’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 3057: Membuka rekaman seed asset_code=’pension_award_rank_1’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 3058: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’pension_award_rank_1’.
-- Uraian sumber 3059: Mengisi asset_type (kategori aset atau kartu) dengan ’PENSION_AWARD’ untuk asset_code=’pension_award_rank_1’.
-- Uraian sumber 3060: Mengisi asset_code (kode aset dalam versi aturan) dengan ’pension_award_rank_1’ untuk asset_code=’pension_award_rank_1’.
-- Uraian sumber 3061: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Peringkat Pensiun 1’ untuk asset_code=’pension_award_rank_1’.
-- Uraian sumber 3062: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1401 untuk asset_code=’pension_award_rank_1’.
-- Uraian sumber 3063: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’pension_award_rank_1’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 3064: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”card_qty”: 1}’ :: jsonb untuk asset_code=’pension_award_rank_1’. Isi JSON dipakai sebagai parameter aksi/metadata: card_qty=1 (jumlah salinan kartu fisik yang tersedia).
-- Uraian sumber 3065: Menutup rekaman seed asset_code=’pension_award_rank_1’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 3066: Membuka rekaman seed asset_code=’pension_award_rank_2’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 3067: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’pension_award_rank_2’.
-- Uraian sumber 3068: Mengisi asset_type (kategori aset atau kartu) dengan ’PENSION_AWARD’ untuk asset_code=’pension_award_rank_2’.
-- Uraian sumber 3069: Mengisi asset_code (kode aset dalam versi aturan) dengan ’pension_award_rank_2’ untuk asset_code=’pension_award_rank_2’.
-- Uraian sumber 3070: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Peringkat Pensiun 2’ untuk asset_code=’pension_award_rank_2’.
-- Uraian sumber 3071: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1402 untuk asset_code=’pension_award_rank_2’.
-- Uraian sumber 3072: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’pension_award_rank_2’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 3073: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”card_qty”: 1}’ :: jsonb untuk asset_code=’pension_award_rank_2’. Isi JSON dipakai sebagai parameter aksi/metadata: card_qty=1 (jumlah salinan kartu fisik yang tersedia).
-- Uraian sumber 3074: Menutup rekaman seed asset_code=’pension_award_rank_2’; koma memisahkannya dari rekaman berikutnya.
-- Uraian sumber 3075: Membuka rekaman seed asset_code=’pension_award_rank_3’; posisi nilainya mengikuti daftar kolom sehingga data demonstrasi atau katalog dapat dibentuk secara deterministik.
-- Uraian sumber 3076: Mengisi ruleset_version_id (UUID revisi aturan yang menjadi lingkup data) dengan ’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid untuk asset_code=’pension_award_rank_3’.
-- Uraian sumber 3077: Mengisi asset_type (kategori aset atau kartu) dengan ’PENSION_AWARD’ untuk asset_code=’pension_award_rank_3’.
-- Uraian sumber 3078: Mengisi asset_code (kode aset dalam versi aturan) dengan ’pension_award_rank_3’ untuk asset_code=’pension_award_rank_3’.
-- Uraian sumber 3079: Mengisi display_name (nama yang ditampilkan kepada pengguna) dengan ’Peringkat Pensiun 3’ untuk asset_code=’pension_award_rank_3’.
-- Uraian sumber 3080: Mengisi sort_order (urutan tampilan atau evaluasi komponen) dengan 1403 untuk asset_code=’pension_award_rank_3’.
-- Uraian sumber 3081: Mengisi is_active (penanda apakah entitas masih boleh digunakan) dengan true untuk asset_code=’pension_award_rank_3’. Nilai boolean TRUE mengaktifkan penanda tersebut.
-- Uraian sumber 3082: Mengisi metadata_json (metadata tambahan aset dalam JSONB) dengan ’{”card_qty”: 1}’ :: jsonb untuk asset_code=’pension_award_rank_3’. Isi JSON dipakai sebagai parameter aksi/metadata: card_qty=1 (jumlah salinan kartu fisik yang tersedia).
-- Uraian sumber 3083: Menutup rekaman seed asset_code=’pension_award_rank_3’; kueri melanjutkan pemrosesan kumpulan nilai yang telah lengkap.
-- Uraian sumber 3084: Memulai UPDATE atau bagian pembaruan saat konflik INSERT pada skrip basis data; SET berikut menentukan kolom yang benar-benar berubah.
-- Uraian sumber 3085: Membuka penetapan nilai pada kolom sasaran dalam skrip basis data; pada FOREIGN KEY, rangkaian SET NULL mengosongkan referensi saat induknya dihapus.
-- Uraian sumber 3086: Menetapkan atau membandingkan display_name (nama yang ditampilkan kepada pengguna) terhadap excluded.display_name, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 3087: Menetapkan atau membandingkan sort_order (urutan tampilan atau evaluasi komponen) terhadap excluded.sort_order, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 3088: Menetapkan atau membandingkan is_active (penanda apakah entitas masih boleh digunakan) terhadap excluded.is_active, pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 3089: Menetapkan atau membandingkan metadata_json (metadata tambahan aset dalam JSONB) terhadap excluded.metadata_json; pada skrip basis data; dalam SET ini merupakan nilai baru, sedangkan di WHERE/IF menjadi syarat kecocokan. EXCLUDED merujuk nilai calon INSERT yang berbenturan dengan kunci unik pada operasi upsert.
-- Uraian sumber 3092: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 3093: Mengevaluasi ekspresi validate_ruleset_component_counts(’f5b4c67b-0825-4970-9f07-3b68e8fcb524’ :: uuid); pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 3095: Memulai pemilihan hasil pada skrip basis data; daftar ekspresi sesudahnya menentukan kolom hasil dan sumber FROM menentukan rekaman yang dibaca.
-- Uraian sumber 3096: Mengevaluasi ekspresi validate_ruleset_component_counts(’7c3bfd8a-27d7-4468-b8d7-cf90131bc61d’ :: uuid); pada skrip basis data; hasilnya melengkapi kolom keluaran, argumen fungsi, atau predikat sesuai klausa yang sedang dibentuk. operator :: melakukan konversi ke tipe PostgreSQL yang diminta dan akan menolak format yang tidak sesuai.
-- Uraian sumber 3098: Mengesahkan semua perubahan sejak BEGIN ke basis data; tabel sementara dengan ON COMMIT DROP dibersihkan pada penyelesaian transaksi.
create extension if not exists pgcrypto;

begin;

create temporary table seed_legacy_action_ids (
    action_id text primary key
) on commit drop;

insert into seed_legacy_action_ids (action_id)
values
    ('BagikanEmasAwal'),
    ('BagikanMisiKoleksi'),
    ('KartuDiambilDariPasar')
;

delete from ruleset_actions legacy
using seed_legacy_action_ids legacy_id
where legacy.action_id = legacy_id.action_id
and not exists (
    select 1
    from events event_log
    where event_log.ruleset_action_id = legacy.ruleset_action_id
);

delete from actions legacy
using seed_legacy_action_ids legacy_id
where legacy.action_id = legacy_id.action_id
and not exists (
    select 1
    from ruleset_actions ruleset_action
    where ruleset_action.action_id = legacy.action_id
);

-- ============================================================-- 1. MASTER Action, INGREDIENT, DAN KOMPONEN-- ============================================================
insert into
    actions (
        action_id,
        action_name,
        behavior_id,
        mode,
        cashflow_direction,
        affects_coin,
        affects_happiness,
        affects_saving,
        affects_inventory,
        is_active
    )
values
    (
        'BahanMasakan',
        'Beli Bahan Masakan',
        'BahanMasakan',
        'BOTH',
        'OUT',
        true,
        false,
        false,
        true,
        true
    ),
    (
        'BuangBahanMasakan',
        'Buang Bahan Masakan',
        'BuangBahanMasakan',
        'BOTH',
        null,
        false,
        false,
        false,
        true,
        true
    ),
    (
        'JualMasakan',
        'Jual Masakan',
        'JualMasakan',
        'BOTH',
        'IN',
        true,
        false,
        false,
        true,
        true
    ),
    (
        'Kebutuhan',
        'Beli Kebutuhan',
        'Kebutuhan',
        'BOTH',
        'OUT',
        true,
        true,
        false,
        false,
        true
    ),
    (
        'KerjaLepas',
        'Kerja Lepas',
        'KerjaLepas',
        'BOTH',
        'IN',
        true,
        false,
        false,
        false,
        true
    ),
    (
        'CatatTransaksi',
        'Catat Transaksi',
        'CatatTransaksi',
        'BOTH',
        null,
        true,
        false,
        true,
        false,
        true
    ),
    (
        'Menabung',
        'Menabung',
        'Menabung',
        'MAHIR',
        'OUT',
        true,
        false,
        true,
        false,
        true
    ),
    (
        'TujuanFinansial',
        'Tujuan Finansial',
        'TujuanFinansial',
        'MAHIR',
        'OUT',
        true,
        true,
        true,
        false,
        true
    ),
    (
        'JumatBerkah',
        'Peduli Donasi',
        'JumatBerkah',
        'BOTH',
        'OUT',
        true,
        false,
        false,
        false,
        true
    ),
    (
        'InvestasiEmas',
        'Investasi Emas',
        'InvestasiEmas',
        'BOTH',
        'OUT',
        true,
        false,
        false,
        false,
        true
    ),
    (
        'JualEmas',
        'Jual Emas',
        'JualEmas',
        'BOTH',
        'IN',
        true,
        false,
        false,
        false,
        true
    ),
    (
        'LewatiTransaksiEmas',
        'Lewati Transaksi Emas',
        'LewatiTransaksiEmas',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'HariMingguLibur',
        'Hari Minggu Libur',
        'HariMingguLibur',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'PinjamanSyariah',
        'Pinjaman Syariah',
        'PinjamanSyariah',
        'MAHIR',
        'IN',
        true,
        false,
        false,
        false,
        true
    ),
    (
        'BayarPinjaman',
        'Bayar Pinjaman',
        'BayarPinjaman',
        'MAHIR',
        'OUT',
        true,
        false,
        false,
        false,
        true
    ),
    (
        'Asuransi',
        'Asuransi',
        'Asuransi',
        'MAHIR',
        'OUT',
        true,
        false,
        false,
        false,
        true
    ),
    (
        'RisikoKehidupan',
        'Risiko Kehidupan',
        'RisikoKehidupan',
        'MAHIR',
        null,
        true,
        false,
        false,
        false,
        true
    ),
    (
        'BayarRisiko',
        'Bayar Risiko',
        'BayarRisiko',
        'MAHIR',
        'OUT',
        true,
        false,
        false,
        false,
        true
    ),
    (
        'GunakanOpsiDarurat',
        'Gunakan Opsi Darurat',
        'GunakanOpsiDarurat',
        'MAHIR',
        null,
        true,
        false,
        false,
        false,
        true
    ),
    (
        'PoinPeringkatDonasi',
        'Poin Peringkat Donasi',
        'PoinPeringkatDonasi',
        'BOTH',
        null,
        false,
        true,
        false,
        false,
        true
    ),
    (
        'UmumkanJuaraDonasi',
        'Sistem: Umumkan Juara Donasi',
        'UmumkanJuaraDonasi',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'PoinEmas',
        'Poin Emas',
        'PoinEmas',
        'BOTH',
        null,
        false,
        true,
        false,
        false,
        true
    ),
    (
        'PoinPeringkatPensiun',
        'Poin Peringkat Pensiun',
        'PoinPeringkatPensiun',
        'BOTH',
        null,
        false,
        true,
        false,
        false,
        true
    ),
    (
        'BagikanTieBreaker',
        'Sistem: Bagikan Tie Breaker',
        'BagikanTieBreaker',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'MulaiSesi',
        'Sistem: Mulai Sesi',
        'MulaiSesi',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'AkhiriSesi',
        'Sistem: Akhiri Sesi',
        'AkhiriSesi',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'AkhirGiliran',
        'Akhir Giliran',
        'AkhirGiliran',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'BukaHargaEmas',
        'Buka Harga Emas',
        'BukaHargaEmas',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'SetupModalAwal',
        'Setup Modal Awal',
        'SetupModalAwal',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'SetupBahanAwal',
        'Setup Bahan Awal',
        'SetupBahanAwal',
        'BOTH',
        'OUT',
        true,
        false,
        false,
        true,
        true
    ),
    (
        'SetupEmasAwal',
        'Setup Emas Awal',
        'SetupEmasAwal',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'SetupMisiAwal',
        'Setup Misi Awal',
        'SetupMisiAwal',
        'BOTH',
        null,
        false,
        false,
        false,
        false,
        true
    ),
    (
        'SetupPinjamanAwal',
        'Setup Pinjaman Awal',
        'SetupPinjamanAwal',
        'MAHIR',
        'IN',
        true,
        false,
        false,
        false,
        true
    ),
    (
        'SetupAsuransiAwal',
        'Setup Asuransi Awal',
        'SetupAsuransiAwal',
        'MAHIR',
        null,
        false,
        false,
        false,
        false,
        true
    ) on conflict (action_id) do
update
set
    action_name = excluded.action_name,
    behavior_id = excluded.behavior_id,
    mode = excluded.mode,
    cashflow_direction = excluded.cashflow_direction,
    affects_coin = excluded.affects_coin,
    affects_happiness = excluded.affects_happiness,
    affects_saving = excluded.affects_saving,
    affects_inventory = excluded.affects_inventory,
    is_active = excluded.is_active;

-- ============================================================-- 3. RULESET DEFAULT BERBASIS JS on INPUT LOKAL-- ============================================================
create temporary table seed_rulesets (
    ruleset_id uuid not null,
    ruleset_version_id uuid not null,
    mode varchar(10) not null,
    ruleset_name varchar(160) not null,
    ruleset_description text not null,
    definition_json jsonb not null
) on commit drop;

insert into
    seed_rulesets (
        ruleset_id,
        ruleset_version_id,
        mode,
        ruleset_name,
        ruleset_description,
        definition_json
    )
values
    (
        '2f4d94db-2a9f-4d4d-9a8a-53b58c598f71',
        'f5b4c67b-0825-4970-9f07-3b68e8fcb524',
        'PEMULA',
        'Cashflowpoly Default - Mode Pemula',
        'Seed ruleset mode pemula dalam definition_json terpadu dan katalog generik.',
        $json$ { "mode": "PEMULA",
        "actions_per_turn": 2,
        "starting_cash": 20,
        "player_ordering": "PLAYER_ORDER",
        "weekday_rules": { "FRI": { "feature": "DONATION",
        "enabled": true },
        "SAT": { "feature": "GOLD_TRADE",
        "enabled": true },
        "SUN": { "feature": "REST",
        "enabled": true } },
        "constraints": { "cash_min": 0,
        "max_ingredient_total": 6,
        "max_same_ingredient": 3,
        "primary_need_max_per_day": null,
        "require_primary_before_others": true },
        "donation": { "min_amount": 1,
        "max_amount": 999999 },
        "gold_trade": { "allow_buy": true,
        "allow_sell": true },
        "advanced": { "loan": { "enabled": false },
        "insurance": { "enabled": false },
        "saving_goal": { "enabled": false } },
        "freelance": { "income": 1 },
        "scoring": { "donation_rank_points": [{ "rank": 1, "points": 7 },{ "rank": 2, "points": 5 },{ "rank": 3, "points": 2 }],
        "gold_points_by_qty": [{ "qty": 1, "points": 3 },{ "qty": 2, "points": 5 },{ "qty": 3, "points": 8 },{ "qty": 4, "points": 12 }],
        "pension_rank_points": [{ "rank": 1, "points": 5 },{ "rank": 2, "points": 3 },{ "rank": 3, "points": 1 }],
        "score_matrix": [{ "score_source": "DONATION", "rank": 1, "points": 7 },{ "score_source": "DONATION", "rank": 2, "points": 5 },{ "score_source": "DONATION", "rank": 3, "points": 2 },{ "score_source": "PENSION", "rank": 1, "points": 5 },{ "score_source": "PENSION", "rank": 2, "points": 3 },{ "score_source": "PENSION", "rank": 3, "points": 1 }] },
        "component_catalog": { "gameConfig": { "initialCoins": 20,
        "initialHappiness": 0,
        "initialSaving": 0,
        "actionsPerTurn": 2,
        "finishDay": 25,
        "minPlayers": 2,
        "maxPlayers": 4 },
        "bahan": [{ "id": "nasi_putih", "nama": "Nasi Putih", "hargaBeli": 1, "cardQty": 5 },{ "id": "sayur", "nama": "Sayur", "hargaBeli": 2, "cardQty": 5 },{ "id": "tahu_tempe", "nama": "Tahu Tempe", "hargaBeli": 3, "cardQty": 5 },{ "id": "telur", "nama": "Telur", "hargaBeli": 4, "cardQty": 5 },{ "id": "daging", "nama": "Daging", "hargaBeli": 5, "cardQty": 5 }],
        "resep": [
          { "id": "lontong_balap", "nama": "lontong balap", "hargaJual": 13, "poinKebahagiaan": 0, "cardQty": 2, "bahan": ["Sayur", "Nasi Putih"] },
        { "id": "nasi_goreng",
        "nama": "nasi goreng",
        "hargaJual": 15,
        "poinKebahagiaan": 0,
        "cardQty": 1,
        "bahan": ["Nasi Putih", "Telur"] },
        { "id": "tahu_campur",
        "nama": "tahu campur",
        "hargaJual": 16,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Daging", "Tahu Tempe"] },
        { "id": "rawon",
        "nama": "rawon",
        "hargaJual": 24,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Daging", "Telur", "Tahu Tempe"] },
        { "id": "semanggi_surabaya",
        "nama": "semanggi surabaya",
        "hargaJual": 14,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Sayur", "Sayur"] },
        { "id": "soto_daging",
        "nama": "soto daging",
        "hargaJual": 17,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Daging", "Telur"] },
        { "id": "nasi_pecel",
        "nama": "nasi pecel",
        "hargaJual": 20,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Nasi Putih", "Tahu Tempe", "Sayur"] },
        { "id": "sego_penyet",
        "nama": "sego penyet",
        "hargaJual": 22,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Telur", "Tahu Tempe", "Nasi Putih"] },
        { "id": "tahu_telur",
        "nama": "tahu telur",
        "hargaJual": 25,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Telur", "Telur", "Tahu Tempe"] },
        { "id": "sate_klopo",
        "nama": "sate klopo",
        "hargaJual": 26,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Daging", "Daging", "Nasi Putih"] },
        { "id": "rujak_cingur",
        "nama": "rujak cingur",
        "hargaJual": 28,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Sayur", "Tahu Tempe", "Nasi Putih", "Daging"] },
        { "id": "gado_gado",
        "nama": "gado gado",
        "hargaJual": 26,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Sayur", "Telur", "Tahu Tempe", "Nasi Putih"] },
        { "id": "nasi_campur",
        "nama": "nasi campur",
        "hargaJual": 27,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Nasi Putih", "Telur", "Daging", "Sayur"] } ],
        "kebutuhan": [
          { "id": "buku_1", "nama": "Buku", "tipe": "primer", "hargaBeli": 2, "poinKebahagiaan": 1, "cardQty": 2 },
          { "id": "buku_2", "nama": "Buku", "tipe": "primer", "hargaBeli": 3, "poinKebahagiaan": 2, "cardQty": 1 },
          { "id": "baju_1", "nama": "Baju", "tipe": "primer", "hargaBeli": 2, "poinKebahagiaan": 1, "cardQty": 1 },
          { "id": "baju_2", "nama": "Baju", "tipe": "primer", "hargaBeli": 3, "poinKebahagiaan": 2, "cardQty": 1 },
          { "id": "tempat_makan_1", "nama": "Tempat Makan", "tipe": "primer", "hargaBeli": 2, "poinKebahagiaan": 1, "cardQty": 1 },
          { "id": "tempat_makan_2", "nama": "Tempat Makan", "tipe": "primer", "hargaBeli": 3, "poinKebahagiaan": 2, "cardQty": 1 },
          { "id": "sepatu_1", "nama": "Sepatu", "tipe": "primer", "hargaBeli": 2, "poinKebahagiaan": 1, "cardQty": 1 },
          { "id": "sepatu_2", "nama": "Sepatu", "tipe": "primer", "hargaBeli": 3, "poinKebahagiaan": 2, "cardQty": 1 },
          { "id": "tas_1", "nama": "Tas", "tipe": "sekunder", "hargaBeli": 4, "poinKebahagiaan": 3, "cardQty": 1 },
          { "id": "tas_2", "nama": "Tas", "tipe": "sekunder", "hargaBeli": 5, "poinKebahagiaan": 4, "cardQty": 1 },
          { "id": "sepeda_1", "nama": "Sepeda", "tipe": "sekunder", "hargaBeli": 4, "poinKebahagiaan": 3, "cardQty": 1 },
          { "id": "sepeda_2", "nama": "Sepeda", "tipe": "sekunder", "hargaBeli": 5, "poinKebahagiaan": 4, "cardQty": 1 },
          { "id": "gadget_1", "nama": "Gadget", "tipe": "sekunder", "hargaBeli": 4, "poinKebahagiaan": 3, "cardQty": 1 },
          { "id": "gadget_2", "nama": "Gadget", "tipe": "sekunder", "hargaBeli": 5, "poinKebahagiaan": 4, "cardQty": 1 },
          { "id": "tempat_pensil_1", "nama": "Tempat Pensil", "tipe": "sekunder", "hargaBeli": 4, "poinKebahagiaan": 3, "cardQty": 1 },
          { "id": "tempat_pensil_2", "nama": "Tempat Pensil", "tipe": "sekunder", "hargaBeli": 5, "poinKebahagiaan": 4, "cardQty": 1 },
          { "id": "boneka_1", "nama": "Boneka", "tipe": "tersier", "hargaBeli": 6, "poinKebahagiaan": 5, "cardQty": 1 },
          { "id": "boneka_2", "nama": "Boneka", "tipe": "tersier", "hargaBeli": 7, "poinKebahagiaan": 6, "cardQty": 1 },
          { "id": "gameboy_1", "nama": "Gameboy", "tipe": "tersier", "hargaBeli": 6, "poinKebahagiaan": 5, "cardQty": 1 },
          { "id": "gameboy_2", "nama": "Gameboy", "tipe": "tersier", "hargaBeli": 7, "poinKebahagiaan": 6, "cardQty": 1 },
          { "id": "jam_1", "nama": "Jam", "tipe": "tersier", "hargaBeli": 6, "poinKebahagiaan": 5, "cardQty": 1 },
          { "id": "jam_2", "nama": "Jam", "tipe": "tersier", "hargaBeli": 7, "poinKebahagiaan": 6, "cardQty": 1 },
          { "id": "hiburan_1", "nama": "Hiburan", "tipe": "tersier", "hargaBeli": 6, "poinKebahagiaan": 5, "cardQty": 1 },
          { "id": "hiburan_2", "nama": "Hiburan", "tipe": "tersier", "hargaBeli": 7, "poinKebahagiaan": 6, "cardQty": 1 }
        ],
        "targetKebutuhan": [
          { "id": "misi_jam", "nama": "jam", "success_points": 0, "failure_points": -10, "penaltyPoints": 10, "kebutuhanTarget": [{ "order": 1, "type": "TIER", "value": "primer" }, { "order": 2, "type": "TIER", "value": "sekunder" }, { "order": 3, "type": "FAMILY", "value": "jam" }] },
        { "id": "misi_boneka",
        "nama": "boneka",
        "success_points": 0,
        "failure_points": -10,
        "penaltyPoints": 10,
        "kebutuhanTarget": [{ "order": 1, "type": "TIER", "value": "primer" }, { "order": 2, "type": "TIER", "value": "sekunder" }, { "order": 3, "type": "FAMILY", "value": "boneka" }] },
        { "id": "misi_gameboy",
        "nama": "gameboy",
        "success_points": 0,
        "failure_points": -10,
        "penaltyPoints": 10,
        "kebutuhanTarget": [{ "order": 1, "type": "TIER", "value": "primer" }, { "order": 2, "type": "TIER", "value": "sekunder" }, { "order": 3, "type": "FAMILY", "value": "gameboy" }] },
        { "id": "misi_hiburan",
        "nama": "hiburan",
        "success_points": 0,
        "failure_points": -10,
        "penaltyPoints": 10,
        "kebutuhanTarget": [{ "order": 1, "type": "TIER", "value": "primer" }, { "order": 2, "type": "TIER", "value": "sekunder" }, { "order": 3, "type": "FAMILY", "value": "hiburan" }] } ],
        "tujuanFinansial": [{ "id": "tujuan_25", "nama": "Kumpul Keluarga", "hargaBeli": 25, "poinKebahagiaan": 20 },{ "id": "tujuan_28", "nama": "Tamasya", "hargaBeli": 28, "poinKebahagiaan": 25 },{ "id": "tujuan_30", "nama": "Keluar Kota", "hargaBeli": 30, "poinKebahagiaan": 28 },{ "id": "tujuan_32", "nama": "Beli Mobil Baru", "hargaBeli": 32, "poinKebahagiaan": 30 },{ "id": "tujuan_35", "nama": "Beli Rumah Baru", "hargaBeli": 35, "poinKebahagiaan": 35 }],
        "narasi": [{"id": "jual_pertama","nama": "jual_pertama","teks": ["Penjualan pertama membuka kepercayaan diri.","Momentum baik harus dijaga."],
        "prerequisiteAksi": [{ "aksi": "JualMasakan", "value": 1 }] } ] } } $json$ :: jsonb
    ),
    (
        'a68f53f9-92a2-446f-9f62-5a4f502a0199',
        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d',
        'MAHIR',
        'Cashflowpoly Default - Mode Mahir',
        'Seed ruleset mode mahir dalam definition_json terpadu dan katalog generik.',
        $json$ { "mode": "MAHIR",
        "actions_per_turn": 2,
        "starting_cash": 10,
        "player_ordering": "PLAYER_ORDER",
        "weekday_rules": { "FRI": { "feature": "DONATION",
        "enabled": true },
        "SAT": { "feature": "GOLD_TRADE",
        "enabled": true },
        "SUN": { "feature": "REST",
        "enabled": true } },
        "constraints": { "cash_min": 0,
        "max_ingredient_total": 6,
        "max_same_ingredient": 3,
        "primary_need_max_per_day": null,
        "require_primary_before_others": true },
        "donation": { "min_amount": 1,
        "max_amount": 999999 },
        "gold_trade": { "allow_buy": true,
        "allow_sell": true },
        "advanced": { "loan": { "enabled": true },
        "insurance": { "enabled": true },
        "saving_goal": { "enabled": true } },
        "freelance": { "income": 1 },
        "scoring": { "donation_rank_points": [{ "rank": 1, "points": 7 },{ "rank": 2, "points": 5 },{ "rank": 3, "points": 2 }],
        "gold_points_by_qty": [{ "qty": 1, "points": 3 },{ "qty": 2, "points": 5 },{ "qty": 3, "points": 8 },{ "qty": 4, "points": 12 }],
        "pension_rank_points": [{ "rank": 1, "points": 5 },{ "rank": 2, "points": 3 },{ "rank": 3, "points": 1 }],
        "score_matrix": [{ "score_source": "DONATION", "rank": 1, "points": 7 },{ "score_source": "DONATION", "rank": 2, "points": 5 },{ "score_source": "DONATION", "rank": 3, "points": 2 },{ "score_source": "PENSION", "rank": 1, "points": 5 },{ "score_source": "PENSION", "rank": 2, "points": 3 },{ "score_source": "PENSION", "rank": 3, "points": 1 }] },
        "component_catalog": { "gameConfig": { "initialCoins": 10,
        "initialHappiness": 0,
        "initialSaving": 0,
        "actionsPerTurn": 2,
        "finishDay": 25,
        "minPlayers": 2,
        "maxPlayers": 4 },
        "bahan": [{ "id": "nasi_putih", "nama": "Nasi Putih", "hargaBeli": 1, "cardQty": 5 },{ "id": "sayur", "nama": "Sayur", "hargaBeli": 2, "cardQty": 5 },{ "id": "tahu_tempe", "nama": "Tahu Tempe", "hargaBeli": 3, "cardQty": 5 },{ "id": "telur", "nama": "Telur", "hargaBeli": 4, "cardQty": 5 },{ "id": "daging", "nama": "Daging", "hargaBeli": 5, "cardQty": 5 }],
        "resep": [
          { "id": "lontong_balap", "nama": "lontong balap", "hargaJual": 13, "poinKebahagiaan": 0, "cardQty": 2, "bahan": ["Sayur", "Nasi Putih"] },
        { "id": "nasi_goreng",
        "nama": "nasi goreng",
        "hargaJual": 15,
        "poinKebahagiaan": 0,
        "cardQty": 1,
        "bahan": ["Nasi Putih", "Telur"] },
        { "id": "tahu_campur",
        "nama": "tahu campur",
        "hargaJual": 16,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Daging", "Tahu Tempe"] },
        { "id": "rawon",
        "nama": "rawon",
        "hargaJual": 24,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Daging", "Telur", "Tahu Tempe"] },
        { "id": "semanggi_surabaya",
        "nama": "semanggi surabaya",
        "hargaJual": 14,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Sayur", "Sayur"] },
        { "id": "soto_daging",
        "nama": "soto daging",
        "hargaJual": 17,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Daging", "Telur"] },
        { "id": "nasi_pecel",
        "nama": "nasi pecel",
        "hargaJual": 20,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Nasi Putih", "Tahu Tempe", "Sayur"] },
        { "id": "sego_penyet",
        "nama": "sego penyet",
        "hargaJual": 22,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Telur", "Tahu Tempe", "Nasi Putih"] },
        { "id": "tahu_telur",
        "nama": "tahu telur",
        "hargaJual": 25,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Telur", "Telur", "Tahu Tempe"] },
        { "id": "sate_klopo",
        "nama": "sate klopo",
        "hargaJual": 26,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Daging", "Daging", "Nasi Putih"] },
        { "id": "rujak_cingur",
        "nama": "rujak cingur",
        "hargaJual": 28,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Sayur", "Tahu Tempe", "Nasi Putih", "Daging"] },
        { "id": "gado_gado",
        "nama": "gado gado",
        "hargaJual": 26,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Sayur", "Telur", "Tahu Tempe", "Nasi Putih"] },
        { "id": "nasi_campur",
        "nama": "nasi campur",
        "hargaJual": 27,
        "poinKebahagiaan": 0,
        "cardQty": 2,
        "bahan": ["Nasi Putih", "Telur", "Daging", "Sayur"] } ],
        "kebutuhan": [
          { "id": "buku_1", "nama": "Buku", "tipe": "primer", "hargaBeli": 2, "poinKebahagiaan": 1, "cardQty": 2 },
          { "id": "buku_2", "nama": "Buku", "tipe": "primer", "hargaBeli": 3, "poinKebahagiaan": 2, "cardQty": 1 },
          { "id": "baju_1", "nama": "Baju", "tipe": "primer", "hargaBeli": 2, "poinKebahagiaan": 1, "cardQty": 1 },
          { "id": "baju_2", "nama": "Baju", "tipe": "primer", "hargaBeli": 3, "poinKebahagiaan": 2, "cardQty": 1 },
          { "id": "tempat_makan_1", "nama": "Tempat Makan", "tipe": "primer", "hargaBeli": 2, "poinKebahagiaan": 1, "cardQty": 1 },
          { "id": "tempat_makan_2", "nama": "Tempat Makan", "tipe": "primer", "hargaBeli": 3, "poinKebahagiaan": 2, "cardQty": 1 },
          { "id": "sepatu_1", "nama": "Sepatu", "tipe": "primer", "hargaBeli": 2, "poinKebahagiaan": 1, "cardQty": 1 },
          { "id": "sepatu_2", "nama": "Sepatu", "tipe": "primer", "hargaBeli": 3, "poinKebahagiaan": 2, "cardQty": 1 },
          { "id": "tas_1", "nama": "Tas", "tipe": "sekunder", "hargaBeli": 4, "poinKebahagiaan": 3, "cardQty": 1 },
          { "id": "tas_2", "nama": "Tas", "tipe": "sekunder", "hargaBeli": 5, "poinKebahagiaan": 4, "cardQty": 1 },
          { "id": "sepeda_1", "nama": "Sepeda", "tipe": "sekunder", "hargaBeli": 4, "poinKebahagiaan": 3, "cardQty": 1 },
          { "id": "sepeda_2", "nama": "Sepeda", "tipe": "sekunder", "hargaBeli": 5, "poinKebahagiaan": 4, "cardQty": 1 },
          { "id": "gadget_1", "nama": "Gadget", "tipe": "sekunder", "hargaBeli": 4, "poinKebahagiaan": 3, "cardQty": 1 },
          { "id": "gadget_2", "nama": "Gadget", "tipe": "sekunder", "hargaBeli": 5, "poinKebahagiaan": 4, "cardQty": 1 },
          { "id": "tempat_pensil_1", "nama": "Tempat Pensil", "tipe": "sekunder", "hargaBeli": 4, "poinKebahagiaan": 3, "cardQty": 1 },
          { "id": "tempat_pensil_2", "nama": "Tempat Pensil", "tipe": "sekunder", "hargaBeli": 5, "poinKebahagiaan": 4, "cardQty": 1 },
          { "id": "boneka_1", "nama": "Boneka", "tipe": "tersier", "hargaBeli": 6, "poinKebahagiaan": 5, "cardQty": 1 },
          { "id": "boneka_2", "nama": "Boneka", "tipe": "tersier", "hargaBeli": 7, "poinKebahagiaan": 6, "cardQty": 1 },
          { "id": "gameboy_1", "nama": "Gameboy", "tipe": "tersier", "hargaBeli": 6, "poinKebahagiaan": 5, "cardQty": 1 },
          { "id": "gameboy_2", "nama": "Gameboy", "tipe": "tersier", "hargaBeli": 7, "poinKebahagiaan": 6, "cardQty": 1 },
          { "id": "jam_1", "nama": "Jam", "tipe": "tersier", "hargaBeli": 6, "poinKebahagiaan": 5, "cardQty": 1 },
          { "id": "jam_2", "nama": "Jam", "tipe": "tersier", "hargaBeli": 7, "poinKebahagiaan": 6, "cardQty": 1 },
          { "id": "hiburan_1", "nama": "Hiburan", "tipe": "tersier", "hargaBeli": 6, "poinKebahagiaan": 5, "cardQty": 1 },
          { "id": "hiburan_2", "nama": "Hiburan", "tipe": "tersier", "hargaBeli": 7, "poinKebahagiaan": 6, "cardQty": 1 }
        ],
        "targetKebutuhan": [
          { "id": "misi_jam", "nama": "jam", "success_points": 0, "failure_points": -10, "penaltyPoints": 10, "kebutuhanTarget": [{ "order": 1, "type": "TIER", "value": "primer" }, { "order": 2, "type": "TIER", "value": "sekunder" }, { "order": 3, "type": "FAMILY", "value": "jam" }] },
        { "id": "misi_boneka",
        "nama": "boneka",
        "success_points": 0,
        "failure_points": -10,
        "penaltyPoints": 10,
        "kebutuhanTarget": [{ "order": 1, "type": "TIER", "value": "primer" }, { "order": 2, "type": "TIER", "value": "sekunder" }, { "order": 3, "type": "FAMILY", "value": "boneka" }] },
        { "id": "misi_gameboy",
        "nama": "gameboy",
        "success_points": 0,
        "failure_points": -10,
        "penaltyPoints": 10,
        "kebutuhanTarget": [{ "order": 1, "type": "TIER", "value": "primer" }, { "order": 2, "type": "TIER", "value": "sekunder" }, { "order": 3, "type": "FAMILY", "value": "gameboy" }] },
        { "id": "misi_hiburan",
        "nama": "hiburan",
        "success_points": 0,
        "failure_points": -10,
        "penaltyPoints": 10,
        "kebutuhanTarget": [{ "order": 1, "type": "TIER", "value": "primer" }, { "order": 2, "type": "TIER", "value": "sekunder" }, { "order": 3, "type": "FAMILY", "value": "hiburan" }] } ],
        "tujuanFinansial": [
          { "id": "tujuan_25", "nama": "Kumpul Keluarga", "hargaBeli": 25, "poinKebahagiaan": 20 },
          { "id": "tujuan_28", "nama": "Tamasya", "hargaBeli": 28, "poinKebahagiaan": 25 },
          { "id": "tujuan_30", "nama": "Keluar Kota", "hargaBeli": 30, "poinKebahagiaan": 28 },
          { "id": "tujuan_32", "nama": "Beli Mobil Baru", "hargaBeli": 32, "poinKebahagiaan": 30 },
          { "id": "tujuan_35", "nama": "Beli Rumah Baru", "hargaBeli": 35, "poinKebahagiaan": 35 }
        ],
        "narasi": [{"id": "jual_pertama","nama": "jual_pertama","teks": ["Penjualan pertama membuka kepercayaan diri.","Momentum baik harus dijaga."],
        "prerequisiteAksi": [{ "aksi": "JualMasakan", "value": 1 }] } ] } } $json$ :: jsonb
    );

insert into
    rulesets (
        ruleset_id,
        name,
        description,
        instructor_user_id,
        created_at,
        created_by_user_id
    )
select
    ruleset_id,
    ruleset_name,
    ruleset_description,
    null,
    now(),
    null :: uuid
from
    seed_rulesets on conflict (ruleset_id) do
update
set
    name = excluded.name,
    description = excluded.description;

insert into
    ruleset_versions (
        ruleset_version_id,
        ruleset_id,
        version,
        status,
        mode,
        schema_version,
        config_hash,
        change_note,
        published_at,
        created_at,
        created_by_user_id
    )
select
    ruleset_version_id,
    ruleset_id,
    1,
    'ACTIVE',
    mode,
    '3.0.0',
    encode(digest(definition_json :: text, 'sha256'), 'hex'),
    'Canonical relational catalog seed',
    now(),
    now(),
    null :: uuid
from
    seed_rulesets on conflict (ruleset_id, version) do
update
set
    status = excluded.status,
    mode = excluded.mode,
    schema_version = excluded.schema_version,
    config_hash = excluded.config_hash,
    change_note = excluded.change_note,
    published_at = excluded.published_at;

insert into
    ruleset_game_settings (
        ruleset_version_id,
        starting_cash,
        starting_happiness,
        starting_saving,
        actions_per_turn,
        finish_day,
        min_players,
        max_players,
        cash_min,
        max_ingredient_total,
        max_same_ingredient,
        primary_need_max_per_day,
        require_primary_before_others,
        donation_min_amount,
        donation_max_amount,
        gold_trade_allow_buy,
        gold_trade_allow_sell,
        loan_enabled,
        insurance_enabled,
        saving_goal_enabled,
        freelance_income
    )
select
    sr.ruleset_version_id,
    coalesce(
        (sr.definition_json ->> 'starting_cash') :: int,
        0
    ),
    coalesce(
        (
            sr.definition_json #>>'{component_catalog,gameConfig,initialHappiness}')::int, 0),coalesce((sr.definition_json#>>'{component_catalog,gameConfig,initialSaving}')::int, 0),coalesce((sr.definition_json->>'actions_per_turn')::int, 2),coalesce((sr.definition_json#>>'{component_catalog,gameConfig,finishDay}')::int, 25),coalesce((sr.definition_json#>>'{component_catalog,gameConfig,minPlayers}')::int, 2),coalesce((sr.definition_json#>>'{component_catalog,gameConfig,maxPlayers}')::int, 4),coalesce((sr.definition_json#>>'{constraints,cash_min}')::int, 0),coalesce((sr.definition_json#>>'{constraints,max_ingredient_total}')::int, 0),coalesce((sr.definition_json#>>'{constraints,max_same_ingredient}')::int, 0),(sr.definition_json#>>'{constraints,primary_need_max_per_day}')::int,coalesce((sr.definition_json#>>'{constraints,require_primary_before_others}')::boolean, true),coalesce((sr.definition_json#>>'{donation,min_amount}')::int, 1),coalesce((sr.definition_json#>>'{donation,max_amount}')::int, 1),coalesce((sr.definition_json#>>'{gold_trade,allow_buy}')::boolean, true),coalesce((sr.definition_json#>>'{gold_trade,allow_sell}')::boolean, true),coalesce((sr.definition_json#>>'{advanced,loan,enabled}')::boolean, false),coalesce((sr.definition_json#>>'{advanced,insurance,enabled}')::boolean, false),coalesce((sr.definition_json#>>'{advanced,saving_goal,enabled}')::boolean, false),coalesce((sr.definition_json#>>'{freelance,income}')::int, 1) from seed_rulesets sr on conflict (ruleset_version_id) do update set starting_cash = excluded.starting_cash,starting_happiness = excluded.starting_happiness,starting_saving = excluded.starting_saving,actions_per_turn = excluded.actions_per_turn,finish_day = excluded.finish_day,min_players = excluded.min_players,max_players = excluded.max_players,cash_min = excluded.cash_min,max_ingredient_total = excluded.max_ingredient_total,max_same_ingredient = excluded.max_same_ingredient,primary_need_max_per_day = excluded.primary_need_max_per_day,require_primary_before_others = excluded.require_primary_before_others,donation_min_amount = excluded.donation_min_amount,donation_max_amount = excluded.donation_max_amount,gold_trade_allow_buy = excluded.gold_trade_allow_buy,gold_trade_allow_sell = excluded.gold_trade_allow_sell,loan_enabled = excluded.loan_enabled,insurance_enabled = excluded.insurance_enabled,saving_goal_enabled = excluded.saving_goal_enabled,freelance_income = excluded.freelance_income,updated_at = now();
            insert into
                ruleset_player_ordering_rules (
                    ruleset_version_id,
                    sort_order,
                    ordering_code,
                    weekday_code,
                    feature_code,
                    is_enabled
                )
            select
                sr.ruleset_version_id,
                1,
                coalesce(
                    sr.definition_json ->> 'player_ordering',
                    'PLAYER_ORDER'
                ),
                null,
                null,
                true
            from
                seed_rulesets sr on conflict (ruleset_version_id, sort_order) do
            update
            set
                ordering_code = excluded.ordering_code,
                weekday_code = excluded.weekday_code,
                feature_code = excluded.feature_code,
                is_enabled = excluded.is_enabled;

insert into
    ruleset_player_ordering_rules (
        ruleset_version_id,
        sort_order,
        ordering_code,
        weekday_code,
        feature_code,
        is_enabled
    )
select
    sr.ruleset_version_id,
    mapped.sort_order,
    null,
    mapped.weekday_code,
    mapped.feature_code,
    mapped.is_enabled
from
    seed_rulesets sr
    cross join lateral (
        values
            (
                10,
                'FRI' :: varchar(8),
                coalesce(
                    sr.definition_json #>>'{weekday_rules,friday,feature}', sr.definition_json#>>'{weekday_rules,FRI,feature}', 'DONATION')::varchar(40), coalesce(sr.definition_json#>>'{weekday_rules,friday,enabled}', sr.definition_json#>>'{weekday_rules,FRI,enabled}', 'true')::boolean),(20, 'SAT'::varchar(8), coalesce(sr.definition_json#>>'{weekday_rules,saturday,feature}', sr.definition_json#>>'{weekday_rules,SAT,feature}', 'GOLD_TRADE')::varchar(40), coalesce(sr.definition_json#>>'{weekday_rules,saturday,enabled}', sr.definition_json#>>'{weekday_rules,SAT,enabled}', 'true')::boolean),(30, 'SUN'::varchar(8), coalesce(sr.definition_json#>>'{weekday_rules,sunday,feature}', sr.definition_json#>>'{weekday_rules,SUN,feature}', 'REST')::varchar(40), coalesce(sr.definition_json#>>'{weekday_rules,sunday,enabled}', sr.definition_json#>>'{weekday_rules,SUN,enabled}', 'true')::boolean)) mapped(sort_order, weekday_code, feature_code, is_enabled) on conflict (ruleset_version_id, sort_order) do update set ordering_code = excluded.ordering_code,weekday_code = excluded.weekday_code,feature_code = excluded.feature_code,is_enabled = excluded.is_enabled;
                    update
                        ruleset_game_assets
                    set
                        is_active = false
                    where
                        ruleset_version_id = 'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid
                        and asset_type in ('RISK', 'TIE_BREAKER');

insert into
    ruleset_game_assets (
        ruleset_version_id,
        asset_type,
        asset_code,
        display_name,
        sort_order,
        is_active,
        metadata_json
    ) with json_assets as (
        select
            sr.ruleset_version_id,
            'INGREDIENT' :: varchar(40) as asset_type,
            coalesce(
                item ->> 'id',
                regexp_replace(lower(item ->> 'nama'), '\s+', '', 'g')
            ) as asset_code,
            item ->> 'nama' as display_name,
            ord :: int as sort_order,
            true as is_active,
            jsonb_build_object('source', 'component_catalog.bahan') as metadata_json
        from
            seed_rulesets sr
            cross join lateral jsonb_array_elements(
                coalesce(
                    sr.definition_json -> 'component_catalog' -> 'bahan',
                    '[]' :: jsonb
                )
            ) with ordinality as x(item, ord)
        union
        all
        select
            sr.ruleset_version_id,
            'ORDER' :: varchar(40),
            item ->> 'id',
            item ->> 'nama',
            (100 + ord) :: int,
            true,
            jsonb_build_object('source', 'component_catalog.resep')
        from
            seed_rulesets sr
            cross join lateral jsonb_array_elements(
                coalesce(
                    sr.definition_json -> 'component_catalog' -> 'resep',
                    '[]' :: jsonb
                )
            ) with ordinality as x(item, ord)
        union
        all
        select
            sr.ruleset_version_id,
            'NEED' :: varchar(40),
            item ->> 'id',
            item ->> 'nama',
            (200 + ord) :: int,
            true,
            jsonb_build_object('source', 'component_catalog.kebutuhan')
        from
            seed_rulesets sr
            cross join lateral jsonb_array_elements(
                coalesce(
                    sr.definition_json -> 'component_catalog' -> 'kebutuhan',
                    '[]' :: jsonb
                )
            ) with ordinality as x(item, ord)
        union
        all
        select
            sr.ruleset_version_id,
            'GOLD' :: varchar(40),
            'gold_card' :: varchar(120),
            'Kartu Emas' :: varchar(160),
            701 :: int,
            true,
            jsonb_build_object('source', 'ruleset_gold_assets', 'card_qty', 20)
        from
            seed_rulesets sr
    ),
    static_assets as (
        select
            *
        from
            (
                values
                    (
                        'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                        'GOLD_PRICE' :: varchar(40),
                        'gold_price_1' :: varchar(120),
                        'Harga Emas 1' :: varchar(160),
                        801,
                        true,
                        '{"source":"static.gold_price"}' :: jsonb
                    ),
                    (
                        'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                        'GOLD_PRICE',
                        'gold_price_2',
                        'Harga Emas 2',
                        802,
                        true,
                        '{"source":"static.gold_price"}' :: jsonb
                    ),
                    (
                        'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                        'GOLD_PRICE',
                        'gold_price_3',
                        'Harga Emas 3',
                        803,
                        true,
                        '{"source":"static.gold_price"}' :: jsonb
                    ),
                    (
                        'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                        'GOLD_PRICE',
                        'gold_price_4',
                        'Harga Emas 4',
                        804,
                        true,
                        '{"source":"static.gold_price"}' :: jsonb
                    ),
                    (
                        'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                        'TIE_BREAKER',
                        'tie_breaker_1',
                        'Tie Breaker 1',
                        901,
                        true,
                        '{"source":"static.tie_breaker"}' :: jsonb
                    ),
                    (
                        'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                        'TIE_BREAKER',
                        'tie_breaker_2',
                        'Tie Breaker 2',
                        902,
                        true,
                        '{"source":"static.tie_breaker"}' :: jsonb
                    ),
                    (
                        'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                        'TIE_BREAKER',
                        'tie_breaker_3',
                        'Tie Breaker 3',
                        903,
                        true,
                        '{"source":"static.tie_breaker"}' :: jsonb
                    ),
                    (
                        'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                        'TIE_BREAKER',
                        'tie_breaker_4',
                        'Tie Breaker 4',
                        904,
                        true,
                        '{"source":"static.tie_breaker"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'GOLD_PRICE',
                        'gold_price_1',
                        'Harga Emas 1',
                        801,
                        true,
                        '{"source":"static.gold_price"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'GOLD_PRICE',
                        'gold_price_2',
                        'Harga Emas 2',
                        802,
                        true,
                        '{"source":"static.gold_price"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'GOLD_PRICE',
                        'gold_price_3',
                        'Harga Emas 3',
                        803,
                        true,
                        '{"source":"static.gold_price"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'GOLD_PRICE',
                        'gold_price_4',
                        'Harga Emas 4',
                        804,
                        true,
                        '{"source":"static.gold_price"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'TIE_BREAKER',
                        'tie_breaker_1',
                        'Tie Breaker 1',
                        901,
                        true,
                        '{"source":"static.tie_breaker"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'TIE_BREAKER',
                        'tie_breaker_2',
                        'Tie Breaker 2',
                        902,
                        true,
                        '{"source":"static.tie_breaker"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'TIE_BREAKER',
                        'tie_breaker_3',
                        'Tie Breaker 3',
                        903,
                        true,
                        '{"source":"static.tie_breaker"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'TIE_BREAKER',
                        'tie_breaker_4',
                        'Tie Breaker 4',
                        904,
                        true,
                        '{"source":"static.tie_breaker"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_beli_peralatan_dapur',
                        'Beli Peralatan Dapur',
                        1202,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_menang_undian',
                        'Menang Undian',
                        1203,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_study_tour',
                        'Study Tour',
                        1204,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_depresi',
                        'Depresi',
                        1205,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_panen_melimpah',
                        'Panen Melimpah',
                        1206,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_ekstrakurikuler_anak',
                        'Ekstrakurikuler Anak',
                        1207,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_ban_bocor',
                        'Ban Bocor',
                        1208,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_sakit_gigi',
                        'Sakit Gigi',
                        1209,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_operasi_usus_buntu',
                        'Operasi Usus Buntu',
                        1210,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_ganti_oli',
                        'Ganti Oli',
                        1211,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_mobil_tabrakan',
                        'Mobil Tabrakan',
                        1212,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_pemadaman_listrik',
                        'Pemadaman Listrik',
                        1213,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_ganti_aki',
                        'Ganti Aki',
                        1214,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_bbm_naik',
                        'BBM Naik',
                        1215,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_kecelakaan',
                        'Kecelakaan',
                        1216,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_sakit_perut',
                        'Sakit Perut',
                        1217,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_sakit_asam_lambung',
                        'Sakit Asam Lambung',
                        1218,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_wisuda_kelulusan',
                        'Wisuda Kelulusan',
                        1219,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_bencana_banjir',
                        'Bencana Banjir',
                        1220,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_gudang_terbakar',
                        'Gudang Terbakar',
                        1221,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_investasi_emas',
                        'Investasi Emas',
                        1222,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_tahun_ajaran_baru',
                        'Tahun Ajaran Baru',
                        1223,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    ),
                    (
                        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                        'RISK',
                        'risk_ulang_tahun',
                        'Ulang Tahun',
                        1224,
                        true,
                        '{"source":"static.risk"}' :: jsonb
                    )
            ) as v(
                ruleset_version_id,
                asset_type,
                asset_code,
                display_name,
                sort_order,
                is_active,
                metadata_json
            )
    )
select
    ruleset_version_id,
    asset_type,
    asset_code,
    display_name,
    sort_order,
    is_active,
    metadata_json
from
    json_assets
union
all
select
    distinct on (
        sr.ruleset_version_id,
        sa.asset_type,
        sa.asset_code
    ) sr.ruleset_version_id,
    sa.asset_type,
    sa.asset_code,
    sa.display_name,
    sa.sort_order,
    sa.is_active,
    sa.metadata_json
from
    static_assets sa
    join seed_rulesets sr on sr.ruleset_version_id = sa.ruleset_version_id on conflict (ruleset_version_id, asset_type, asset_code) do
update
set
    display_name = excluded.display_name,
    sort_order = excluded.sort_order,
    is_active = excluded.is_active,
    metadata_json = excluded.metadata_json,
    updated_at = now();

insert into
    ruleset_orders (
        ruleset_version_id,
        ruleset_game_asset_id,
        order_code,
        item_name,
        sell_price,
        happiness_points,
        sort_order,
        card_qty,
        is_active,
        payload_json
    )
select
    sr.ruleset_version_id,
    rga.ruleset_game_asset_id,
    item ->> 'id',
    item ->> 'nama',
    coalesce((item ->> 'hargaJual') :: int, 0),
    coalesce((item ->> 'poinKebahagiaan') :: int, 0),
    ord :: int,
    coalesce((item ->> 'cardQty') :: int, 1),
    true,
    jsonb_build_object(
        'hargaJual',
        coalesce((item ->> 'hargaJual') :: int, 0),
        'poinKebahagiaan',
        coalesce((item ->> 'poinKebahagiaan') :: int, 0)
    )
from
    seed_rulesets sr
    cross join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'resep'
    ) with ordinality as x(item, ord)
    join ruleset_game_assets rga on rga.ruleset_version_id = sr.ruleset_version_id
    and rga.asset_type = 'ORDER'
    and rga.asset_code = item ->> 'id' on conflict (ruleset_version_id, order_code) do
update
set
    item_name = excluded.item_name,
    sell_price = excluded.sell_price,
    happiness_points = excluded.happiness_points,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    is_active = excluded.is_active,
    payload_json = excluded.payload_json;

insert into
    ruleset_actions (
        ruleset_version_id,
        action_id,
        behavior_id,
        sort_order,
        is_active
    )
select
    sr.ruleset_version_id,
    a.action_id,
    a.behavior_id,
    row_number() over (
        partition by sr.ruleset_version_id
        order by
            a.action_id
    ) :: int,
    a.is_active
from
    seed_rulesets sr
    join actions a on a.is_active
    and a.mode in ('BOTH', sr.mode) on conflict (ruleset_version_id, action_id) do
update
set
    sort_order = excluded.sort_order,
    behavior_id = excluded.behavior_id,
    is_active = excluded.is_active;

insert into
    ruleset_ingredients (
        ruleset_version_id,
        ruleset_game_asset_id,
        ingredient_code,
        item_name,
        display_name,
        purchase_price,
        sort_order,
        card_qty,
        is_active,
        payload_json
    )
select
    sr.ruleset_version_id,
    rga.ruleset_game_asset_id,
    coalesce(
        item ->> 'id',
        regexp_replace(lower(item ->> 'nama'), '\s+', '', 'g')
    ),
    item ->> 'nama',
    item ->> 'nama',
    coalesce((item ->> 'hargaBeli') :: int, 0),
    ord :: int,
    coalesce((item ->> 'cardQty') :: int, 5),
    true,
    jsonb_build_object(
        'asset_code',
        coalesce(
            item ->> 'id',
            regexp_replace(lower(item ->> 'nama'), '\s+', '', 'g')
        ),
        'display_name',
        item ->> 'nama',
        'hargaBeli',
        coalesce((item ->> 'hargaBeli') :: int, 0)
    )
from
    seed_rulesets sr
    cross join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'bahan'
    ) with ordinality as x(item, ord)
    join ruleset_game_assets rga on rga.ruleset_version_id = sr.ruleset_version_id
    and rga.asset_type = 'INGREDIENT'
    and rga.asset_code = coalesce(
        item ->> 'id',
        regexp_replace(lower(item ->> 'nama'), '\s+', '_', 'g')
    ) on conflict (ruleset_version_id, ingredient_code) do
update
set
    item_name = excluded.item_name,
    display_name = excluded.display_name,
    purchase_price = excluded.purchase_price,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    is_active = excluded.is_active,
    payload_json = excluded.payload_json;

insert into
    ruleset_order_requirements (
        ruleset_version_id,
        ruleset_order_id,
        requirement_order,
        required_asset_id,
        qty_required,
        payload_json
    )
select
    sr.ruleset_version_id,
    ro.ruleset_order_id,
    row_number() over (
        partition by sr.ruleset_version_id,
        recipe.recipe ->> 'id'
        order by
            req.ingredient_name
    ) :: int,
    ri.ruleset_game_asset_id,
    req.ingredient_qty,
    jsonb_build_object('ingredient_name', req.ingredient_name)
from
    seed_rulesets sr
    cross join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'resep'
    ) as recipe(recipe)
    cross join lateral (
        select
            ingredient_name,
            count(*) :: int as ingredient_qty
        from
            jsonb_array_elements_text(recipe.recipe -> 'bahan') as z(ingredient_name)
        group by
            ingredient_name
    ) req
    join ruleset_orders ro on ro.ruleset_version_id = sr.ruleset_version_id
    and ro.order_code = recipe.recipe ->> 'id'
    join ruleset_ingredients ri on ri.ruleset_version_id = sr.ruleset_version_id
    and lower(ri.display_name) = lower(req.ingredient_name) on conflict (
        ruleset_order_id,
        requirement_order,
        required_asset_id
    ) do
update
set
    qty_required = excluded.qty_required,
    payload_json = excluded.payload_json;

insert into
    ruleset_needs (
        ruleset_version_id,
        ruleset_game_asset_id,
        need_code,
        item_name,
        need_tier,
        purchase_price,
        happiness_points,
        sort_order,
        card_qty,
        is_active,
        need_family_code,
        payload_json
    )
select
    sr.ruleset_version_id,
    rga.ruleset_game_asset_id,
    item ->> 'id',
    item ->> 'nama',
    coalesce(item ->> 'tipe', 'primer'),
    coalesce((item ->> 'hargaBeli') :: int, 0),
    coalesce((item ->> 'poinKebahagiaan') :: int, 0),
    ord :: int,
    coalesce((item ->> 'cardQty') :: int, 1),
    true,
    coalesce(
        nullif(item ->> 'family', ''),
        regexp_replace(item ->> 'id', '_[0-9]+$', '')
    ),
    jsonb_build_object(
        'tipe',
        coalesce(item ->> 'tipe', 'primer'),
        'family',
        coalesce(
            nullif(item ->> 'family', ''),
            regexp_replace(item ->> 'id', '_[0-9]+$', '')
        ),
        'hargaBeli',
        coalesce((item ->> 'hargaBeli') :: int, 0),
        'poinKebahagiaan',
        coalesce((item ->> 'poinKebahagiaan') :: int, 0)
    )
from
    seed_rulesets sr
    cross join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'kebutuhan'
    ) with ordinality as x(item, ord)
    join ruleset_game_assets rga on rga.ruleset_version_id = sr.ruleset_version_id
    and rga.asset_type = 'NEED'
    and rga.asset_code = item ->> 'id' on conflict (ruleset_version_id, need_code) do
update
set
    item_name = excluded.item_name,
    need_tier = excluded.need_tier,
    purchase_price = excluded.purchase_price,
    happiness_points = excluded.happiness_points,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    is_active = excluded.is_active,
    need_family_code = excluded.need_family_code,
    payload_json = excluded.payload_json;

insert into
    ruleset_need_set_bonuses (
        ruleset_version_id,
        pattern_code,
        required_count,
        points,
        sort_order,
        payload_json
    )
select
    sr.ruleset_version_id,
    bonus.pattern_code,
    bonus.required_count,
    bonus.points,
    bonus.sort_order,
    jsonb_build_object(
        'pattern',
        bonus.pattern_code,
        'required_count',
        bonus.required_count
    )
from
    seed_rulesets sr
    cross join (
        values
            ('THREE_DIFFERENT' :: varchar(40), 3, 4, 1),
            ('THREE_SAME' :: varchar(40), 3, 2, 2)
    ) as bonus(pattern_code, required_count, points, sort_order) on conflict (ruleset_version_id, pattern_code) do
update
set
    required_count = excluded.required_count,
    points = excluded.points,
    sort_order = excluded.sort_order,
    payload_json = excluded.payload_json;

insert into
    ruleset_collection_missions (
        ruleset_version_id,
        mission_code,
        item_name,
        success_points,
        failure_points,
        penalty_points,
        sort_order,
        card_qty,
        is_active,
        payload_json
    )
select
    sr.ruleset_version_id,
    item ->> 'id',
    item ->> 'nama',
    coalesce((item ->> 'success_points') :: int, 0),
    coalesce((item ->> 'failure_points') :: int, 0),
    coalesce((item ->> 'penaltyPoints') :: int, 0),
    ord :: int,
    1,
    true,
    jsonb_build_object(
        'success_points',
        coalesce((item ->> 'success_points') :: int, 0),
        'failure_points',
        coalesce((item ->> 'failure_points') :: int, 0),
        'penaltyPoints',
        coalesce((item ->> 'penaltyPoints') :: int, 0)
    )
from
    seed_rulesets sr
    cross join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'targetKebutuhan'
    ) with ordinality as x(item, ord) on conflict (ruleset_version_id, mission_code) do
update
set
    item_name = excluded.item_name,
    success_points = excluded.success_points,
    failure_points = excluded.failure_points,
    penalty_points = excluded.penalty_points,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    is_active = excluded.is_active,
    payload_json = excluded.payload_json;

insert into
    ruleset_collection_mission_requirements (
        ruleset_version_id,
        ruleset_collection_mission_id,
        requirement_order,
        requirement_type,
        required_asset_id,
        required_need_tier,
        required_need_family_code,
        qty_required,
        payload_json
    )
select
    sr.ruleset_version_id,
    rcm.ruleset_collection_mission_id,
    coalesce((rule ->> 'order') :: int, ord :: int),
    case
        when upper(rule ->> 'type') in ('TIER', 'NEED_TIER') then 'NEED_TIER'
        when upper(rule ->> 'type') in ('FAMILY', 'NEED_FAMILY') then 'NEED_FAMILY'
        else 'ASSET'
    end,
    case
        when upper(rule ->> 'type') in ('TIER', 'NEED_TIER', 'FAMILY', 'NEED_FAMILY') then null
        else required_asset.ruleset_game_asset_id
    end,
    case
        when upper(rule ->> 'type') in ('TIER', 'NEED_TIER') then lower(rule ->> 'value')
        else null
    end,
    case
        when upper(rule ->> 'type') in ('FAMILY', 'NEED_FAMILY') then lower(rule ->> 'value')
        else null
    end,
    null,
    jsonb_build_object(
        'type',
        rule ->> 'type',
        'value',
        rule ->> 'value'
    )
from
    seed_rulesets sr
    cross join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'targetKebutuhan'
    ) as mission(mission)
    cross join lateral jsonb_array_elements(
        coalesce(
            mission.mission -> 'kebutuhanTarget',
            '[]' :: jsonb
        )
    ) with ordinality as x(rule, ord)
    join ruleset_collection_missions rcm on rcm.ruleset_version_id = sr.ruleset_version_id
    and rcm.mission_code = mission.mission ->> 'id'
    left join ruleset_game_assets required_asset on required_asset.ruleset_version_id = sr.ruleset_version_id
    and required_asset.asset_type = 'NEED'
    and upper(rule ->> 'type') not in ('TIER', 'NEED_TIER', 'FAMILY', 'NEED_FAMILY')
    and (
        lower(required_asset.asset_code) = lower(rule ->> 'value')
        or lower(required_asset.display_name) = lower(rule ->> 'value')
    ) on conflict (ruleset_collection_mission_id, requirement_order) do
update
set
    requirement_type = excluded.requirement_type,
    required_asset_id = excluded.required_asset_id,
    required_need_tier = excluded.required_need_tier,
    required_need_family_code = excluded.required_need_family_code,
    qty_required = excluded.qty_required,
    payload_json = excluded.payload_json;

insert into
    ruleset_financial_goals (
        ruleset_version_id,
        goal_code,
        item_name,
        purchase_price,
        happiness_points,
        sort_order,
        card_qty,
        is_active,
        payload_json
    )
select
    sr.ruleset_version_id,
    item ->> 'id',
    item ->> 'nama',
    coalesce((item ->> 'hargaBeli') :: int, 0),
    coalesce((item ->> 'poinKebahagiaan') :: int, 0),
    ord :: int,
    1,
    true,
    jsonb_build_object(
        'hargaBeli',
        coalesce((item ->> 'hargaBeli') :: int, 0),
        'poinKebahagiaan',
        coalesce((item ->> 'poinKebahagiaan') :: int, 0)
    )
from
    seed_rulesets sr
    cross join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'tujuanFinansial'
    ) with ordinality as x(item, ord)
where
    sr.mode = 'MAHIR' on conflict (ruleset_version_id, goal_code) do
update
set
    item_name = excluded.item_name,
    purchase_price = excluded.purchase_price,
    happiness_points = excluded.happiness_points,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    is_active = excluded.is_active,
    payload_json = excluded.payload_json;

insert into
    ruleset_narratives (
        ruleset_version_id,
        narrative_code,
        item_name,
        repeatable,
        cooldown_turns,
        sort_order,
        is_active,
        payload_json
    )
select
    sr.ruleset_version_id,
    item ->> 'id',
    item ->> 'nama',
    coalesce((item ->> 'repeatable') :: boolean, false),
    nullif(item ->> 'cooldownTurns', '') :: int,
    ord :: int,
    true,
    jsonb_build_object(
        'prerequisiteAksi',
        coalesce(item -> 'prerequisiteAksi', '[]' :: jsonb)
    )
from
    seed_rulesets sr
    cross join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'narasi'
    ) with ordinality as x(item, ord) on conflict (ruleset_version_id, narrative_code) do
update
set
    item_name = excluded.item_name,
    repeatable = excluded.repeatable,
    cooldown_turns = excluded.cooldown_turns,
    sort_order = excluded.sort_order,
    is_active = excluded.is_active,
    payload_json = excluded.payload_json;

insert into
    ruleset_narrative_scenes (
        ruleset_version_id,
        ruleset_narrative_id,
        scene_code,
        scene_order,
        text_lines,
        media_json,
        payload_json
    )
select
    sr.ruleset_version_id,
    rn.ruleset_narrative_id,
    concat(
        narrative.narrative ->> 'id',
        'scene',
        ord :: int
    ),
    ord :: int,
    case
        when jsonb_typeof(narrative.narrative -> 'teks') = 'array' then narrative.narrative -> 'teks'
        when narrative.narrative ? 'teks' then jsonb_build_array(narrative.narrative ->> 'teks')
        else '[]' :: jsonb
    end,
    coalesce(narrative.narrative -> 'media', '{}' :: jsonb),
    jsonb_build_object(
        'source',
        'component_catalog.narasi',
        'teks',
        coalesce(narrative.narrative -> 'teks', '[]' :: jsonb)
    )
from
    seed_rulesets sr
    cross join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'narasi'
    ) with ordinality as narrative(narrative, ord)
    join ruleset_narratives rn on rn.ruleset_version_id = sr.ruleset_version_id
    and rn.narrative_code = narrative.narrative ->> 'id' on conflict (ruleset_narrative_id, scene_code) do
update
set
    scene_order = excluded.scene_order,
    text_lines = excluded.text_lines,
    media_json = excluded.media_json,
    payload_json = excluded.payload_json;

insert into
    ruleset_trigger_conditions (
        ruleset_version_id,
        trigger_owner_type,
        ruleset_narrative_id,
        ruleset_action_id,
        reference_asset_id,
        operator,
        threshold_numeric,
        sort_order,
        condition_json,
        is_active
    )
select
    rn.ruleset_version_id,
    'NARRATIVE',
    rn.ruleset_narrative_id,
    ra.ruleset_action_id,
    null :: uuid,
    'COUNT_GTE',
    greatest(coalesce((prereq ->> 'value') :: int, 1), 1),
    ord :: int,
    jsonb_build_object('source', 'narrative.prerequisiteAksi'),
    true
from
    seed_rulesets sr
    cross join lateral jsonb_array_elements(
        sr.definition_json -> 'component_catalog' -> 'narasi'
    ) as narrative(narrative)
    cross join lateral jsonb_array_elements(
        coalesce(
            narrative.narrative -> 'prerequisiteAksi',
            '[]' :: jsonb
        )
    ) with ordinality as x(prereq, ord)
    join ruleset_narratives rn on rn.ruleset_version_id = sr.ruleset_version_id
    and rn.narrative_code = narrative.narrative ->> 'id'
    join ruleset_actions ra on ra.ruleset_version_id = sr.ruleset_version_id
    and lower(ra.action_id) = lower(prereq ->> 'aksi')
    and ra.is_active on conflict (
        ruleset_version_id,
        ruleset_narrative_id,
        sort_order
    ) do
update
set
    ruleset_action_id = excluded.ruleset_action_id,
    reference_asset_id = excluded.reference_asset_id,
    operator = excluded.operator,
    threshold_numeric = excluded.threshold_numeric,
    condition_json = excluded.condition_json,
    is_active = excluded.is_active;

insert into
    ruleset_rank_points (
        ruleset_version_id,
        rank_type,
        rank_no,
        points,
        sort_order
    )
select
    sr.ruleset_version_id,
    'DONATION',
    coalesce((item ->> 'rank') :: int, ord :: int),
    coalesce((item ->> 'points') :: int, 0),
    ord :: int
from
    seed_rulesets sr
    cross join lateral jsonb_array_elements(
        coalesce(
            sr.definition_json -> 'scoring' -> 'donation_rank_points',
            '[]' :: jsonb
        )
    ) with ordinality as x(item, ord) on conflict (ruleset_version_id, rank_type, rank_no) do
update
set
    points = excluded.points,
    sort_order = excluded.sort_order;

insert into
    ruleset_gold_assets (
        ruleset_version_id,
        ruleset_game_asset_id,
        asset_code,
        quantity,
        points,
        sort_order,
        card_qty,
        is_active,
        payload_json
    )
select
    sr.ruleset_version_id,
    rga.ruleset_game_asset_id,
    'gold_card' :: varchar(120),
    coalesce((item ->> 'qty') :: int, ord :: int),
    coalesce((item ->> 'points') :: int, 0),
    ord :: int,
    1,
    true,
    jsonb_build_object(
        'qty',
        coalesce((item ->> 'qty') :: int, ord :: int),
        'points',
        coalesce((item ->> 'points') :: int, 0)
    )
from
    seed_rulesets sr
    cross join lateral jsonb_array_elements(
        coalesce(
            sr.definition_json -> 'scoring' -> 'gold_points_by_qty',
            '[]' :: jsonb
        )
    ) with ordinality as x(item, ord)
    join ruleset_game_assets rga on rga.ruleset_version_id = sr.ruleset_version_id
    and rga.asset_type = 'GOLD'
    and rga.asset_code = 'gold_card' on conflict (ruleset_version_id, asset_code, quantity) do
update
set
    quantity = excluded.quantity,
    points = excluded.points,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    is_active = excluded.is_active,
    payload_json = excluded.payload_json;

insert into
    ruleset_rank_points (
        ruleset_version_id,
        rank_type,
        rank_no,
        points,
        sort_order
    )
select
    sr.ruleset_version_id,
    'PENSION',
    coalesce((item ->> 'rank') :: int, ord :: int),
    coalesce((item ->> 'points') :: int, 0),
    ord :: int
from
    seed_rulesets sr
    cross join lateral jsonb_array_elements(
        coalesce(
            sr.definition_json -> 'scoring' -> 'pension_rank_points',
            '[]' :: jsonb
        )
    ) with ordinality as x(item, ord) on conflict (ruleset_version_id, rank_type, rank_no) do
update
set
    points = excluded.points,
    sort_order = excluded.sort_order;

insert into
    ruleset_gold_prices (
        ruleset_version_id,
        ruleset_game_asset_id,
        price_code,
        quantity,
        unit_price,
        sort_order,
        card_qty,
        is_active,
        payload_json
    )
select
    extra.ruleset_version_id,
    rga.ruleset_game_asset_id,
    extra.price_code,
    extra.quantity,
    extra.unit_price,
    extra.sort_order,
    extra.card_qty,
    true,
    extra.payload_json
from
    (
        values
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'gold_price_1',
                1,
                5,
                221,
                2,
                '{"qty":1,"price":5}' :: jsonb
            ),
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'gold_price_2',
                1,
                6,
                222,
                2,
                '{"qty":1,"price":6}' :: jsonb
            ),
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'gold_price_3',
                1,
                7,
                223,
                1,
                '{"qty":1,"price":7}' :: jsonb
            ),
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'gold_price_4',
                1,
                8,
                224,
                1,
                '{"qty":1,"price":8}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'gold_price_1',
                1,
                5,
                221,
                2,
                '{"qty":1,"price":5}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'gold_price_2',
                1,
                6,
                222,
                2,
                '{"qty":1,"price":6}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'gold_price_3',
                1,
                7,
                223,
                1,
                '{"qty":1,"price":7}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'gold_price_4',
                1,
                8,
                224,
                1,
                '{"qty":1,"price":8}' :: jsonb
            )
    ) as extra(
        ruleset_version_id,
        price_code,
        quantity,
        unit_price,
        sort_order,
        card_qty,
        payload_json
    )
    join ruleset_game_assets rga on rga.ruleset_version_id = extra.ruleset_version_id
    and rga.asset_type = 'GOLD_PRICE'
    and rga.asset_code = extra.price_code on conflict (ruleset_version_id, price_code) do
update
set
    quantity = excluded.quantity,
    unit_price = excluded.unit_price,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    is_active = excluded.is_active,
    payload_json = excluded.payload_json;

insert into
    ruleset_tie_breakers (
        ruleset_version_id,
        ruleset_game_asset_id,
        tie_breaker_code,
        tie_number,
        sort_order,
        card_qty,
        payload_json
    )
select
    extra.ruleset_version_id,
    rga.ruleset_game_asset_id,
    extra.tie_breaker_code,
    extra.tie_number,
    extra.sort_order,
    extra.card_qty,
    extra.payload_json
from
    (
        values
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'tie_breaker_1',
                1,
                241,
                1,
                '{"number":1}' :: jsonb
            ),
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'tie_breaker_2',
                2,
                242,
                1,
                '{"number":2}' :: jsonb
            ),
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'tie_breaker_3',
                3,
                243,
                1,
                '{"number":3}' :: jsonb
            ),
            (
                'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
                'tie_breaker_4',
                4,
                244,
                1,
                '{"number":4}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'tie_breaker_1',
                1,
                241,
                1,
                '{"number":1}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'tie_breaker_2',
                2,
                242,
                1,
                '{"number":2}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'tie_breaker_3',
                3,
                243,
                1,
                '{"number":3}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'tie_breaker_4',
                4,
                244,
                1,
                '{"number":4}' :: jsonb
            )
    ) as extra(
        ruleset_version_id,
        tie_breaker_code,
        tie_number,
        sort_order,
        card_qty,
        payload_json
    )
    join ruleset_game_assets rga on rga.ruleset_version_id = extra.ruleset_version_id
    and rga.asset_type = 'TIE_BREAKER'
    and rga.asset_code = extra.tie_breaker_code on conflict (ruleset_version_id, tie_breaker_code) do
update
set
    tie_number = excluded.tie_number,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    payload_json = excluded.payload_json;

insert into
    ruleset_sharia_loans (
        ruleset_version_id,
        loan_code,
        item_name,
        principal,
        repayment_amount,
        duration_days,
        penalty_points,
        sort_order,
        card_qty,
        payload_json
    )
select
    sr.ruleset_version_id,
    extra.loan_code,
    extra.item_name,
    extra.principal,
    extra.repayment_amount,
    extra.duration_days,
    extra.penalty_points,
    extra.sort_order,
    extra.card_qty,
    extra.payload_json
from
    (
        values
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'loan_syariah_10',
                'Pinjaman Syariah 10',
                10,
                10,
                25,
                15,
                251,
                8,
                '{"principal":10,"repayment_amount":10,"duration":25,"penalty_points":15,"card_supply":8}' :: jsonb
            )
    ) as extra(
        ruleset_version_id,
        loan_code,
        item_name,
        principal,
        repayment_amount,
        duration_days,
        penalty_points,
        sort_order,
        card_qty,
        payload_json
    )
    join seed_rulesets sr on sr.ruleset_version_id = extra.ruleset_version_id on conflict (ruleset_version_id, loan_code) do
update
set
    item_name = excluded.item_name,
    principal = excluded.principal,
    repayment_amount = excluded.repayment_amount,
    duration_days = excluded.duration_days,
    penalty_points = excluded.penalty_points,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    payload_json = excluded.payload_json;

insert into
    ruleset_insurance_products (
        ruleset_version_id,
        product_code,
        item_name,
        premium,
        usage_limit,
        sort_order,
        card_qty,
        payload_json
    )
select
    sr.ruleset_version_id,
    extra.product_code,
    extra.item_name,
    extra.premium,
    extra.usage_limit,
    extra.sort_order,
    extra.card_qty,
    extra.payload_json
from
    (
        values
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'multirisk_basic',
                'Asuransi Multirisk',
                1,
                1,
                261,
                0,
                '{"premium":1,"usage_limit":1,"physical_card_source":"TIE_BREAKER_BACK"}' :: jsonb
            )
    ) as extra(
        ruleset_version_id,
        product_code,
        item_name,
        premium,
        usage_limit,
        sort_order,
        card_qty,
        payload_json
    )
    join seed_rulesets sr on sr.ruleset_version_id = extra.ruleset_version_id on conflict (ruleset_version_id, product_code) do
update
set
    item_name = excluded.item_name,
    premium = excluded.premium,
    usage_limit = excluded.usage_limit,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    payload_json = excluded.payload_json;

insert into
    ruleset_life_risks (
        ruleset_version_id,
        ruleset_game_asset_id,
        risk_code,
        item_name,
        effect_type,
        direction,
        amount,
        duration_days,
        target_scope,
        sort_order,
        card_qty,
        payload_json
    )
select
    sr.ruleset_version_id,
    rga.ruleset_game_asset_id,
    extra.risk_code,
    extra.item_name,
    extra.effect_type,
    extra.direction,
    extra.amount,
    extra.duration_days,
    extra.target_scope,
    extra.sort_order,
    extra.card_qty,
    extra.payload_json
from
    (
        values
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_beli_peralatan_dapur',
                'Beli Peralatan Dapur',
                'COIN_EFFECT',
                'OUT',
                3,
                1,
                'SELF',
                272,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":3,"target_scope":"SELF"}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_menang_undian',
                'Menang Undian',
                'COIN_EFFECT',
                'IN',
                2,
                1,
                'SELF',
                273,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"IN","amount":2}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_study_tour',
                'Study Tour',
                'COIN_EFFECT',
                'OUT',
                4,
                1,
                'SELF',
                274,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":4}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_depresi',
                'Depresi',
                'COIN_EFFECT',
                'OUT',
                3,
                1,
                'SELF',
                275,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":3}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_panen_melimpah',
                'Panen Melimpah',
                'INGREDIENT_PRICE_MODIFIER',
                null,
                0,
                7,
                'ALL_PLAYERS',
                276,
                1,
                '{"effect_type":"INGREDIENT_PRICE_MODIFIER","target_scope":"ALL_PLAYERS","value_delta":-1,"duration_days":7}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_ekstrakurikuler_anak',
                'Ekstrakurikuler Anak',
                'COIN_EFFECT',
                'OUT',
                3,
                1,
                'SELF',
                277,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":3}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_ban_bocor',
                'Ban Bocor',
                'COIN_EFFECT',
                'OUT',
                3,
                1,
                'SELF',
                278,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":3}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_sakit_gigi',
                'Sakit Gigi',
                'COIN_EFFECT',
                'OUT',
                4,
                1,
                'SELF',
                279,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":4,"target_scope":"SELF"}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_operasi_usus_buntu',
                'Operasi Usus Buntu',
                'COIN_EFFECT',
                'OUT',
                6,
                1,
                'SELF',
                280,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":6,"target_scope":"SELF"}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_ganti_oli',
                'Ganti Oli',
                'COIN_EFFECT',
                'OUT',
                3,
                1,
                'SELF',
                281,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":3,"target_scope":"SELF"}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_mobil_tabrakan',
                'Mobil Tabrakan',
                'COIN_EFFECT',
                'OUT',
                6,
                1,
                'SELF',
                282,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":6,"target_scope":"SELF"}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_pemadaman_listrik',
                'Pemadaman Listrik',
                'ALL_PLAYERS_COIN_EFFECT',
                'OUT',
                3,
                1,
                'ALL_PLAYERS',
                283,
                1,
                '{"effect_type":"ALL_PLAYERS_COIN_EFFECT","direction":"OUT","amount":3,"target_scope":"ALL_PLAYERS"}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_ganti_aki',
                'Ganti Aki',
                'COIN_EFFECT',
                'OUT',
                3,
                1,
                'SELF',
                284,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":3}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_bbm_naik',
                'BBM Naik',
                'INGREDIENT_PRICE_MODIFIER',
                null,
                0,
                7,
                'ALL_PLAYERS',
                285,
                1,
                '{"effect_type":"INGREDIENT_PRICE_MODIFIER","target_scope":"ALL_PLAYERS","value_delta":1,"duration_days":7}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_kecelakaan',
                'Kecelakaan',
                'COIN_EFFECT',
                'OUT',
                5,
                1,
                'SELF',
                286,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":5,"target_scope":"SELF"}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_sakit_perut',
                'Sakit Perut',
                'COIN_EFFECT',
                'OUT',
                3,
                1,
                'SELF',
                287,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":3}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_sakit_asam_lambung',
                'Sakit Asam Lambung',
                'COIN_EFFECT',
                'OUT',
                4,
                1,
                'SELF',
                288,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":4}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_wisuda_kelulusan',
                'Wisuda Kelulusan',
                'COIN_EFFECT',
                'OUT',
                6,
                1,
                'SELF',
                289,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":6,"target_scope":"SELF"}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_bencana_banjir',
                'Bencana Banjir',
                'ALL_PLAYERS_COIN_EFFECT',
                'OUT',
                3,
                1,
                'ALL_PLAYERS',
                290,
                1,
                '{"effect_type":"ALL_PLAYERS_COIN_EFFECT","direction":"OUT","amount":3,"target_scope":"ALL_PLAYERS"}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_gudang_terbakar',
                'Gudang Terbakar',
                'COIN_EFFECT',
                'OUT',
                6,
                1,
                'SELF',
                291,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":6}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_investasi_emas',
                'Investasi Emas',
                'GOLD_TRADE',
                null,
                0,
                1,
                'ALL_PLAYERS',
                292,
                2,
                '{"effect_type":"GOLD_TRADE","target_scope":"ALL_PLAYERS","opens_gold_trade":true}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_tahun_ajaran_baru',
                'Tahun Ajaran Baru',
                'COIN_EFFECT',
                'OUT',
                5,
                1,
                'SELF',
                293,
                1,
                '{"effect_type":"COIN_EFFECT","direction":"OUT","amount":5,"target_scope":"SELF"}' :: jsonb
            ),
            (
                '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
                'risk_ulang_tahun',
                'Ulang Tahun',
                'PLAYER_TO_PLAYER_TRANSFER',
                'IN',
                1,
                1,
                'OTHER_PLAYERS',
                294,
                1,
                '{"effect_type":"PLAYER_TO_PLAYER_TRANSFER","direction":"IN","amount":1,"target_scope":"OTHER_PLAYERS"}' :: jsonb
            )
    ) as extra(
        ruleset_version_id,
        risk_code,
        item_name,
        effect_type,
        direction,
        amount,
        duration_days,
        target_scope,
        sort_order,
        card_qty,
        payload_json
    )
    join seed_rulesets sr on sr.ruleset_version_id = extra.ruleset_version_id
    join ruleset_game_assets rga on rga.ruleset_version_id = sr.ruleset_version_id
    and rga.asset_type = 'RISK'
    and rga.asset_code = extra.risk_code on conflict (ruleset_version_id, risk_code) do
update
set
    item_name = excluded.item_name,
    effect_type = excluded.effect_type,
    direction = excluded.direction,
    amount = excluded.amount,
    duration_days = excluded.duration_days,
    target_scope = excluded.target_scope,
    sort_order = excluded.sort_order,
    card_qty = excluded.card_qty,
    payload_json = excluded.payload_json;

-- Sync physical cards as assets in ruleset_game_assets to allow positioning them
-- Sync COLLECTION_MISSION
insert into
    ruleset_game_assets (
        ruleset_version_id,
        asset_type,
        asset_code,
        display_name,
        sort_order,
        is_active,
        metadata_json
    )
select
    ruleset_version_id,
    'COLLECTION_MISSION',
    mission_code,
    item_name,
    500 + sort_order,
    is_active,
    jsonb_build_object('source', 'ruleset_collection_missions')
from
    ruleset_collection_missions on conflict (ruleset_version_id, asset_type, asset_code) do
update
set
    display_name = excluded.display_name,
    sort_order = excluded.sort_order,
    is_active = excluded.is_active,
    metadata_json = excluded.metadata_json;

-- Sync FINANCIAL_GOAL
insert into
    ruleset_game_assets (
        ruleset_version_id,
        asset_type,
        asset_code,
        display_name,
        sort_order,
        is_active,
        metadata_json
    )
select
    ruleset_version_id,
    'FINANCIAL_GOAL',
    goal_code,
    item_name,
    600 + sort_order,
    is_active,
    jsonb_build_object('source', 'ruleset_financial_goals')
from
    ruleset_financial_goals on conflict (ruleset_version_id, asset_type, asset_code) do
update
set
    display_name = excluded.display_name,
    sort_order = excluded.sort_order,
    is_active = excluded.is_active,
    metadata_json = excluded.metadata_json;

-- Sync SHARIA_LOAN
insert into
    ruleset_game_assets (
        ruleset_version_id,
        asset_type,
        asset_code,
        display_name,
        sort_order,
        is_active,
        metadata_json
    )
select
    ruleset_version_id,
    'SHARIA_LOAN',
    loan_code,
    item_name,
    1000 + sort_order,
    true,
    jsonb_build_object('source', 'ruleset_sharia_loans')
from
    ruleset_sharia_loans on conflict (ruleset_version_id, asset_type, asset_code) do
update
set
    display_name = excluded.display_name,
    sort_order = excluded.sort_order,
    is_active = excluded.is_active,
    metadata_json = excluded.metadata_json;

-- Sync INSURANCE
insert into
    ruleset_game_assets (
        ruleset_version_id,
        asset_type,
        asset_code,
        display_name,
        sort_order,
        is_active,
        metadata_json
    )
select
    ruleset_version_id,
    'INSURANCE',
    product_code,
    item_name,
    1100 + sort_order,
    true,
    jsonb_build_object('source', 'ruleset_insurance_products')
from
    ruleset_insurance_products on conflict (ruleset_version_id, asset_type, asset_code) do
update
set
    display_name = excluded.display_name,
    sort_order = excluded.sort_order,
    is_active = excluded.is_active,
    metadata_json = excluded.metadata_json;

-- Insert DONATION_AWARD
insert into
    ruleset_game_assets (
        ruleset_version_id,
        asset_type,
        asset_code,
        display_name,
        sort_order,
        is_active,
        metadata_json
    )
values
    (
        'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
        'DONATION_AWARD',
        'donation_award_rank_1',
        'Peringkat Donasi 1',
        1301,
        true,
        '{"card_qty": 3}' :: jsonb
    ),
    (
        'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
        'DONATION_AWARD',
        'donation_award_rank_2',
        'Peringkat Donasi 2',
        1302,
        true,
        '{"card_qty": 3}' :: jsonb
    ),
    (
        'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
        'DONATION_AWARD',
        'donation_award_rank_3',
        'Peringkat Donasi 3',
        1303,
        true,
        '{"card_qty": 3}' :: jsonb
    ),
    (
        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
        'DONATION_AWARD',
        'donation_award_rank_1',
        'Peringkat Donasi 1',
        1301,
        true,
        '{"card_qty": 3}' :: jsonb
    ),
    (
        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
        'DONATION_AWARD',
        'donation_award_rank_2',
        'Peringkat Donasi 2',
        1302,
        true,
        '{"card_qty": 3}' :: jsonb
    ),
    (
        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
        'DONATION_AWARD',
        'donation_award_rank_3',
        'Peringkat Donasi 3',
        1303,
        true,
        '{"card_qty": 3}' :: jsonb
    ) on conflict (ruleset_version_id, asset_type, asset_code) do
update
set
    display_name = excluded.display_name,
    sort_order = excluded.sort_order,
    is_active = excluded.is_active,
    metadata_json = excluded.metadata_json;

-- Insert PENSION_AWARD
insert into
    ruleset_game_assets (
        ruleset_version_id,
        asset_type,
        asset_code,
        display_name,
        sort_order,
        is_active,
        metadata_json
    )
values
    (
        'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
        'PENSION_AWARD',
        'pension_award_rank_1',
        'Peringkat Pensiun 1',
        1401,
        true,
        '{"card_qty": 1}' :: jsonb
    ),
    (
        'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
        'PENSION_AWARD',
        'pension_award_rank_2',
        'Peringkat Pensiun 2',
        1402,
        true,
        '{"card_qty": 1}' :: jsonb
    ),
    (
        'f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid,
        'PENSION_AWARD',
        'pension_award_rank_3',
        'Peringkat Pensiun 3',
        1403,
        true,
        '{"card_qty": 1}' :: jsonb
    ),
    (
        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
        'PENSION_AWARD',
        'pension_award_rank_1',
        'Peringkat Pensiun 1',
        1401,
        true,
        '{"card_qty": 1}' :: jsonb
    ),
    (
        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
        'PENSION_AWARD',
        'pension_award_rank_2',
        'Peringkat Pensiun 2',
        1402,
        true,
        '{"card_qty": 1}' :: jsonb
    ),
    (
        '7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid,
        'PENSION_AWARD',
        'pension_award_rank_3',
        'Peringkat Pensiun 3',
        1403,
        true,
        '{"card_qty": 1}' :: jsonb
    ) on conflict (ruleset_version_id, asset_type, asset_code) do
update
set
    display_name = excluded.display_name,
    sort_order = excluded.sort_order,
    is_active = excluded.is_active,
    metadata_json = excluded.metadata_json;

-- Audit ruleset component counts
select
    validate_ruleset_component_counts('f5b4c67b-0825-4970-9f07-3b68e8fcb524' :: uuid);

select
    validate_ruleset_component_counts('7c3bfd8a-27d7-4468-b8d7-cf90131bc61d' :: uuid);

commit;
