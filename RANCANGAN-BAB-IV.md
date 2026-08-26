# Rancangan BAB IV - Hasil dan Pembahasan

## Rancang Bangun Dasbor Analitika dan Sistem Manajemen Aturan pada Gim Papan Cashflowpoly untuk Mendukung Pembelajaran Literasi Finansial

## A. Dasar penyusunan

Rancangan ini disusun berdasarkan:

1. `LaporanTugasAkhir.pdf` versi 26 Agustus 2026 yang berada di root project;
2. rumusan masalah, tujuan, batasan, dan metodologi pada Bab I-Bab III;
3. implementasi aktual RESTful API, PostgreSQL, modul ruleset, analitik, UI MVC, Seed 2, pengujian, dan deployment project;
4. buku aturan Cashflowpoly Entrepreneur Edition versi 02;
5. pedoman laporan tugas akhir ITS yang membagi Bab Hasil dan Pembahasan menjadi hasil penelitian/perancangan serta pembahasan atau diskusi berupa analisis, sintesis, dan evaluasi.

Bab IV pada laporan saat ini baru berisi halaman judul. Struktur yang disarankan di bawah ini memisahkan:

- **Subbab 4.1-4.5 sebagai hasil penelitian/perancangan**, yaitu apa yang berhasil dibangun dan apa hasil pengujiannya;
- **Subbab 4.6 sebagai pembahasan**, yaitu makna hasil, jawaban terhadap rumusan masalah, hubungan dengan teori, kontribusi, dan keterbatasan;
- **Subbab 4.7 sebagai rangkuman bab**.

Struktur tersebut lebih kuat secara akademik daripada menjadikan seluruh Bab IV sebagai manual fitur atau kumpulan tangkapan layar.

---

## B. Pemetaan rumusan masalah ke Bab IV

| Rumusan masalah | Bagian Bab IV yang menjawab | Bukti utama |
|---|---|---|
| RM1. Bagaimana merancang back-end RESTful API untuk menerima, memvalidasi, dan menyimpan event serta menjaga konsistensi state? | 4.2, 4.5.2-4.5.3, dan 4.6.1 | Arsitektur, kontrak API, lifecycle sesi, validasi event, basis data, idempotensi, proyeksi state, dan hasil uji integrasi |
| RM2. Bagaimana merancang manajemen ruleset dinamis tanpa mengubah aturan inti? | 4.3, 4.5.4, dan 4.6.2 | Struktur ruleset, versioning, validasi, aktivasi, penguncian versi pada sesi, UI ruleset, dan hasil uji |
| RM3. Bagaimana mengolah event menjadi indikator dan capaian misi serta menyajikannya melalui dasbor? | 4.4, 4.5.5-4.5.6, dan 4.6.3 | Pipeline analitik, variabel fisik, capaian misi koleksi, metrik turunan, provenance rumus, histori keputusan, penyaringan timeline, dasbor sesi/pemain, serta validasi angka dan tampilan |

Pemetaan ini sebaiknya ditampilkan sebagai tabel awal Bab IV agar pembaca dapat melihat hubungan antara masalah, artefak, dan bukti evaluasinya.

---

# BAB IV
# HASIL DAN PEMBAHASAN

## Paragraf pembuka yang disarankan

> Bab ini memaparkan hasil pengembangan dan evaluasi sistem informasi Cashflowpoly yang terdiri atas layanan RESTful API, basis data, modul manajemen ruleset, mekanisme pengolahan analitik, dan dasbor berbasis web. Hasil disajikan berdasarkan keterkaitan antara artefak yang dibangun dan tiga rumusan masalah penelitian. Pemaparan dimulai dari gambaran umum artefak, dilanjutkan dengan hasil implementasi back-end dan basis data, modul ruleset, analitik dan dasbor, serta hasil pengujian fungsional dan integrasi. Bagian akhir membahas kemampuan sistem dalam mendukung pencatatan terstruktur, keterlacakan keputusan permainan, dan refleksi berbasis data, sekaligus menjelaskan keterbatasan penelitian.

> Sistem yang dikembangkan berfungsi sebagai pendukung permainan fisik, bukan sebagai simulator kartu atau pasar. Pembagian kartu tetap dilakukan di meja permainan dan dicatat melalui IDN atau simulator pengirim event. Oleh karena itu, hasil yang dibahas berfokus pada kemampuan sistem untuk memvalidasi dan mengolah data yang dilaporkan, bukan pada otomatisasi komponen fisik atau pengukuran langsung peningkatan literasi finansial peserta.

---

## 4.1 Hasil Pengembangan Artefak Sistem

### Tujuan subbab

Memberikan gambaran hasil akhir sistem sebelum menjelaskan detail tiap modul. Subbab ini menjawab pertanyaan “artefak apa yang dihasilkan, siapa yang menggunakannya, dan bagaimana komponen saling berhubungan?”

### 4.1.1 Ruang Lingkup Artefak yang Dihasilkan

Isi yang dibahas:

- RESTful API sebagai penerima dan penyedia data;
- PostgreSQL sebagai penyimpanan identitas, ruleset, sesi, revisi setup, event, proyeksi, transaksi, snapshot analitik, dan audit;
- modul manajemen ruleset untuk Instruktur;
- dasbor analitika sesi dan pemain;
- simulator/Seed 2 sebagai pengganti integrasi langsung dengan IDN dalam pengujian;
- dua peran pengguna: `INSTRUCTOR` dan `PLAYER`;
- batas tanggung jawab sistem terhadap permainan fisik.

Bukti yang disarankan:

- **Tabel 4.1 Komponen Artefak dan Fungsinya**;
- satu paragraf yang menegaskan bahwa pasar/deck fisik tidak disimulasikan backend.

### 4.1.2 Arsitektur Implementasi Sistem

Isi yang dibahas:

- alur IDN/simulator -> RESTful API -> PostgreSQL -> layanan analitik -> UI MVC;
- komunikasi UI dengan API;
- autentikasi Bearer JWT pada API dan sesi autentikasi pada UI;
- Nginx sebagai reverse proxy dan Cloudflare Tunnel sebagai jalur publik produksi;
- pemisahan jaringan database, API, UI, dan endpoint metrics.

Bukti yang disarankan:

- **Gambar 4.1 Arsitektur Implementasi Cashflowpoly**;
- **Gambar 4.2 Alur Data dari Permainan Fisik hingga Dasbor**.

Jangan memenuhi gambar dengan nama class. Gambar cukup memperlihatkan batas sistem, arus data, dan tanggung jawab komponen.

### 4.1.3 Hasil Prototyping Iteratif

Isi yang dibahas sebagai hasil metode pada Bab III:

- iterasi baseline: pencatatan event, ruleset, dan dasbor dasar;
- iterasi penyelarasan rulebook: setup fisik, penghapusan aksi `LewatiOrder`, dua aksi utama, hari khusus, dan penghapusan pasar/deck virtual;
- iterasi keterlacakan analitik: variabel fisik, formula baku, angka aktual, provenance, dan rekalkulasi;
- iterasi penyederhanaan antarmuka: accordion, penghapusan elemen redundan, responsivitas, mode Pemula/Mahir, dan warna pemain;
- iterasi kesiapan rilis: migrasi, pengujian rilis, container, dokumentasi, dan deployment.

Bukti yang disarankan:

- **Tabel 4.2 Ringkasan Iterasi dan Perubahan Artefak** dengan kolom: iterasi, masalah/umpan balik, perubahan, hasil verifikasi.

Gunakan riwayat commit dan dokumen keputusan sebagai bukti. Jangan menulis tanggal atau jumlah iterasi yang tidak dapat ditunjukkan dari repository.

### 4.1.4 Lingkungan Implementasi dan Deployment

Isi yang dibahas secara ringkas:

- ASP.NET Core 10, MVC Razor, PostgreSQL 16, Tailwind CSS, Docker Compose;
- deployment di VPS Ubuntu melalui Nginx dan Cloudflare Tunnel;
- domain produksi `https://narafin.org`;
- migrasi dan Seed 2 dijalankan melalui mode aplikasi;
- service health check dan pembatasan akses publik `/metrics`.

Bukti yang disarankan:

- **Tabel 4.3 Lingkungan Implementasi**;
- satu tangkapan layar halaman login/beranda produksi atau keluaran health check.

Deployment adalah bukti artefak dapat dioperasikan, tetapi bukan jawaban utama terhadap efektivitas pembelajaran.

---

## 4.2 Hasil Implementasi Back-end dan Basis Data

### Tujuan subbab

Menyajikan hasil implementasi yang menjawab Rumusan Masalah 1.

### 4.2.1 Model Data dan Migrasi Basis Data

Isi yang dibahas:

- kelompok entitas utama: pengguna, pemain, ruleset, sesi, peserta, revisi setup, event, proyeksi, transaksi, snapshot metrik, skor akhir, dan audit;
- hubungan `sessions` dengan versi ruleset yang dikunci;
- `session_setup_revisions` sebagai histori setup sebelum start;
- `events` sebagai source of truth aktivitas permainan;
- proyeksi dan `metric_snapshots` sebagai data turunan yang dapat dibangun ulang;
- migrasi berurutan dan `schema_history` dengan checksum;
- Seed 2 yang idempoten.

Bukti yang disarankan:

- **Gambar 4.3 ERD Inti Sistem**;
- **Tabel 4.4 Tabel Basis Data dan Tanggung Jawabnya**;
- cuplikan isi `schema_history`, bukan seluruh DDL.

### 4.2.2 Autentikasi dan Pembatasan Akses Berbasis Peran

Isi yang dibahas:

- login dan registrasi Player;
- JWT dengan masa berlaku delapan jam;
- pembatasan role `INSTRUCTOR` dan `PLAYER`;
- Instruktur mengelola ruleset dan sesi miliknya;
- Player hanya mengakses sesi, analitik, transaksi, dan misi miliknya;
- redaksi misi peserta lain pada setup, event, analitik, dan UI.

Bukti yang disarankan:

- **Tabel 4.5 Matriks Hak Akses**;
- contoh ringkas respons `401`, `403`, dan `200`.

### 4.2.3 Lifecycle Sesi dan Setup Permainan Fisik

Isi yang dibahas:

- status sesi `CREATED`, `STARTED`, dan `ENDED`;
- pembuatan sesi dan penambahan peserta;
- alur validasi setup tanpa simpan;
- penyimpanan revisi setup;
- setup pertama mengunci peserta dan ruleset;
- start mengunci revisi terbaru dan membentuk event setup secara atomik;
- end menghentikan penerimaan event baru dan membentuk skor final;
- aturan setup berbeda untuk Pemula dan Mahir.

Bukti yang disarankan:

- **Gambar 4.4 Sequence Diagram Lifecycle Sesi**;
- **Gambar 4.5 State Diagram Sesi**;
- **Tabel 4.6 Aturan Setup Pemula dan Mahir**.

### 4.2.4 Kontrak dan Validasi Event Permainan

Isi yang dibahas:

- struktur event: identitas, sesi, pemain, waktu, hari, giliran, slot aksi, urutan, tipe aksi, versi ruleset, payload, dan client request ID;
- validasi bentuk request, akses, status sesi, peserta, ruleset, urutan, saldo, inventory, dan aturan domain;
- kewajiban dua aksi utama pada hari normal;
- dua aksi sama diperbolehkan;
- Jumat, Sabtu, dan Minggu diproses sesuai ruleset;
- `LewatiOrder` ditolak;
- event pasar/deck virtual tidak menjadi bagian state backend;
- event ditolak tidak disimpan sebagai event atau metrik.

Bukti yang disarankan:

- **Gambar 4.6 Pipeline Validasi Event**;
- **Tabel 4.7 Contoh Event Valid dan Tidak Valid**;
- contoh payload cukup satu atau dua jenis aksi, sedangkan katalog lengkap ditempatkan di lampiran atau dokumentasi API.

### 4.2.5 Idempotensi, Keterurutan, dan Konsistensi State

Isi yang dibahas:

- `event_id`, `client_request_id`, dan `sequence_number`;
- retry identik tidak menerapkan perubahan dua kali;
- konflik ID dengan payload berbeda menghasilkan `409`;
- urutan event kontigu;
- perubahan saldo, inventory, pinjaman, asuransi, tujuan, dan progres dibentuk melalui proyeksi;
- recompute membangun ulang data turunan dari event yang sah;
- pagination cursor event dan transaksi mencegah duplikasi/kehilangan data saat daftar bertambah.

Bukti yang disarankan:

- **Tabel 4.8 Mekanisme Penjagaan Konsistensi**;
- satu ilustrasi sebelum-sesudah retry atau recompute.

### 4.2.6 Penanganan Galat, Audit, dan Observability

Isi yang dibahas:

- bentuk error `error_code`, `message`, `details`, dan `trace_id`;
- pemakaian kode status `400`, `401`, `403`, `404`, `409`, `422`, `429`, dan `500`;
- audit login, setup, lifecycle sesi, ruleset, deployment, dan aktivitas akun demo;
- log penolakan tidak menyimpan payload gameplay;
- endpoint health dan metrics internal;
- `/metrics` tidak dapat diakses melalui Nginx publik.

Bukti yang disarankan:

- **Tabel 4.9 Pemetaan Kondisi Galat dan Respons API**;
- contoh satu respons error beserta `trace_id`.

---

## 4.3 Hasil Implementasi Modul Manajemen Ruleset

### Tujuan subbab

Menyajikan hasil implementasi yang menjawab Rumusan Masalah 2.

### 4.3.1 Pemisahan Aturan Inti dan Parameter Variabel

Isi yang dibahas:

- aturan inti yang tidak dapat diubah sembarang pengguna, misalnya alur sesi, validitas aksi, privasi misi, dan konsistensi event;
- parameter ruleset yang dapat dikonfigurasi, misalnya kas awal, jumlah aksi, batas pemain, hari selesai, fitur mode, nilai kartu, biaya, dan poin;
- alasan pemisahan untuk menjaga kesesuaian rulebook.

Bukti yang disarankan:

- **Tabel 4.10 Aturan Inti dan Parameter yang Dapat Dikonfigurasi**.

### 4.3.2 Struktur Ruleset dan Versioning

Isi yang dibahas:

- identitas ruleset dan versi immutable;
- struktur `definition` yang memuat settings serta katalog komponen;
- perbedaan konfigurasi mode Pemula dan Mahir;
- versi baru dibentuk ketika ruleset diperbarui;
- histori versi memungkinkan sesi lama tetap memiliki konteks aturan yang benar.

Bukti yang disarankan:

- **Gambar 4.7 Model Versioning Ruleset**;
- cuplikan JSON ringkas yang hanya menunjukkan `mode`, `settings`, dan dua contoh komponen.

### 4.3.3 Validasi, Aktivasi, dan Penghapusan Ruleset

Isi yang dibahas:

- pembuatan ruleset dari komponen default;
- validasi tipe, kelengkapan, rentang nilai, mode, dan konflik konfigurasi;
- aktivasi versi sebelum dipakai sesi;
- penghapusan hanya jika ruleset/versi belum terikat secara tidak aman;
- ruleset yang sudah digunakan sesi tetap dapat ditelusuri.

Bukti yang disarankan:

- **Gambar 4.8 Alur Pengelolaan Ruleset**;
- **Tabel 4.11 Skenario Validasi Ruleset**.

### 4.3.4 Penguncian Ruleset pada Sesi

Isi yang dibahas:

- sesi menyimpan `ruleset_version_id`, bukan hanya nama ruleset;
- setup pertama mengunci versi ruleset dan roster;
- event wajib membawa versi yang sama dengan sesi;
- analitik historis dibangun menggunakan versi yang terkunci;
- perubahan ruleset baru tidak mengubah konteks sesi lama.

Bukti yang disarankan:

- satu contoh dua sesi yang memakai mode atau versi ruleset berbeda;
- hasil penolakan event dengan ruleset version yang tidak cocok.

### 4.3.5 Antarmuka Manajemen Ruleset

Isi yang dibahas:

- halaman daftar, pembuatan/pembaruan, detail versi, aktivasi, dan penghapusan;
- pembagian form menjadi bagian yang mudah dipahami;
- tampilan komponen berbeda menurut mode;
- umpan balik validasi dari API.

Bukti yang disarankan:

- **Gambar 4.9 Halaman Daftar Ruleset**;
- **Gambar 4.10 Halaman Form dan Detail Ruleset**.

Hindari memasukkan tangkapan layar setiap field. Pilih dua gambar yang menunjukkan alur utama dan keterbacaan konfigurasi.

---

## 4.4 Hasil Pengolahan Analitik dan Implementasi Dasbor

### Tujuan subbab

Menyajikan hasil implementasi yang menjawab Rumusan Masalah 3.

### 4.4.1 Pipeline Pengolahan Data Analitik

Isi yang dibahas:

- event sah disimpan berurutan;
- projector memperbarui state dan transaksi;
- builder membentuk variabel fisik serta metrik turunan;
- snapshot mempercepat pembacaan dasbor;
- recompute menghasilkan ulang snapshot secara deterministik;
- event legacy dan event yang ditolak tidak mencemari hasil baru.

Bukti yang disarankan:

- **Gambar 4.11 Pipeline Event hingga Dasbor Analitika**.

### 4.4.2 Variabel Permainan Fisik

Jelaskan sebelas kelompok variabel berikut tanpa menjadikannya “sebelas skor literasi”:

1. koin dan finansial;
2. bahan masakan;
3. pesanan masakan;
4. kebutuhan;
5. donasi;
6. emas;
7. dana pensiun;
8. risiko hidup;
9. target finansial;
10. penggunaan aksi;
11. progres giliran.

Isi yang dibahas:

- definisi setiap kelompok;
- contoh variabel;
- status dan capaian Misi Koleksi ditempatkan dalam kelompok kebutuhan, termasuk target, kebutuhan yang dimiliki, status selesai/belum selesai, dan penalti apabila misi gagal;
- asal data setup/event/ruleset/proyeksi;
- mode tempat variabel berlaku;
- perlakuan nilai nol sah dan data tidak tersedia.

Bukti yang disarankan:

- **Tabel 4.12 Variabel Permainan Fisik dan Sumber Datanya**.

### 4.4.3 Metrik Turunan Pemain

Jelaskan tiga belas metrik baku:

1. Pertumbuhan Kas;
2. Diversifikasi Pendapatan;
3. Porsi Biaya Usaha;
4. Margin Usaha Pesanan;
5. Kesiapan Menghadapi Risiko (Mahir);
6. Beban Pinjaman (Mahir);
7. Progres Target Finansial (Mahir);
8. Fokus Aksi Penghasil Uang;
9. Pemanfaatan Bahan;
10. Porsi Aksi Jangka Panjang (Mahir);
11. Keragaman Pemenuhan Kebutuhan;
12. Komitmen Donasi;
13. Komposisi Poin Kebahagiaan.

Isi yang dibahas:

- definisi operasional;
- rumus;
- variabel pembentuk;
- sumber data;
- satuan keluaran;
- kondisi `null` atau “Belum dapat dihitung”;
- batas interpretasi tiap metrik.

Bukti yang disarankan:

- **Tabel 4.13 Definisi Operasional Metrik Turunan**.

Tabel utama dapat memuat nama, tujuan, rumus, sumber, dan mode. Rincian lengkap tiga belas formula dapat dipindahkan ke lampiran bila tabel menjadi terlalu panjang.

### 4.4.4 Keterlacakan Angka dan Validasi Formula

Isi yang dibahas:

- UI menampilkan asal data, variabel pembentuk, angka aktual, substitusi formula, dan hasil;
- nama variabel konsisten antara API, formula, UI, Postman, dan dokumentasi;
- contoh perhitungan aktual, misalnya `15 ÷ 10 × 100% = 150%`;
- pembagi nol menghasilkan `null`, bukan nol palsu;
- snapshot dapat dihitung ulang dari event sumber.

Bukti yang disarankan:

- **Gambar 4.12 Tampilan Rincian Cara Menghitung**;
- **Tabel 4.14 Validasi Manual Sampel Formula** dengan kolom pemain, variabel sumber, perhitungan manual, hasil API/UI, dan kesesuaian.

Pilih setidaknya satu metrik sederhana, satu metrik indeks, satu metrik Mahir, dan komposisi poin untuk validasi manual.

### 4.4.5 Dasbor Ringkasan Sesi

Isi yang dibahas:

- ringkasan kinerja sesi;
- daftar pemain dan delapan komponen poin;
- perbandingan pemain dengan warna biru, kuning, hijau, dan ungu;
- jejak perjalanan permainan hingga tanggal 25;
- histori keputusan dalam timeline campuran event pemain dan sistem yang diurutkan berdasarkan `sequence_number`;
- penyaringan timeline untuk seluruh event, event sistem, atau satu pemain tertentu, serta pemilihan tanggal pada kalender;
- transaksi dan aktivitas berdasarkan ruleset yang digunakan;
- komponen poin dalam accordion per pemain.

Bukti yang disarankan:

- **Gambar 4.13 Dasbor Ringkasan Sesi**;
- **Gambar 4.14 Jejak Perjalanan Permainan**;
- **Gambar 4.15 Perbandingan Komponen Poin Pemain**.

### 4.4.6 Dasbor Analitika Pemain

Isi yang dibahas:

- Riwayat Aktivitas Keuangan;
- Ringkasan Statistik Pemain;
- Cerita di Balik Hasil Pemain;
- Data Permainan Lengkap;
- accordion dan persistensi pilihan pengguna;
- provenance pada “Lihat angka pembentuk dan rumus”;
- tabel transaksi dan data permainan yang responsif;
- penyembunyian komponen Mahir pada sesi Pemula.
- status capaian Misi Koleksi dan dampaknya pada komposisi Poin Kebahagiaan.

Bukti yang disarankan:

- **Gambar 4.16 Ringkasan Statistik Pemain**;
- **Gambar 4.17 Cerita di Balik Hasil Pemain**;
- **Gambar 4.18 Data Permainan Lengkap dan Provenance**.

### 4.4.7 Pembaruan Near Real-time, Histori, Penyaringan, dan Konteks Analisis

Isi yang dibahas:

- data baru muncul setelah event diterima API dan proyeksi/analitik diperbarui;
- beranda mengambil ulang statistik setiap 30 detik, sedangkan timeline sesi mengambil event baru setiap 10 detik melalui cursor;
- mekanisme ini adalah polling berkala, bukan WebSocket atau streaming kontinu;
- histori dijaga oleh `sequence_number`, sedangkan cursor mencegah item lama dimuat ulang sebagai item baru;
- timeline dapat disaring menurut seluruh event, event sistem, atau pemain tertentu dan dapat dibaca per tanggal;
- konteks sesi dan ruleset berasal dari halaman sesi yang dipilih, sedangkan konteks pemain berasal dari halaman analitika pemain yang dipilih;
- jangan mengklaim tersedia satu filter global sesi/ruleset/pemain apabila implementasinya berupa navigasi ke konteks halaman dan filter timeline per pemain.

Bukti yang disarankan:

- gunakan **Gambar 4.14 Jejak Perjalanan Permainan** untuk menunjukkan histori dan filter;
- sertakan hasil uji bahwa event baru tampil pada siklus polling berikutnya tanpa duplikasi.

### 4.4.8 Privasi, Mode, Bahasa, dan Kemudahan Pembacaan

Isi yang dibahas:

- misi pemain lain tidak ditampilkan kepada Player;
- kelompok Mahir tidak dirender pada DOM sesi Pemula;
- label Bahasa Indonesia dan Inggris memakai arti yang konsisten;
- elemen dekoratif yang tampak seperti data telah dihapus;
- grid kartu responsif dan tabel tidak menghasilkan overflow viewport;
- accordion dapat digunakan melalui keyboard.

Bagian ini membahas kualitas fungsi antarmuka yang dapat diverifikasi secara objektif. Jangan menyimpulkan kepuasan, kemudahan penggunaan subjektif, atau usability tanpa penelitian pengguna.

---

## 4.5 Hasil Pengujian dan Validasi Sistem

### Tujuan subbab

Menunjukkan bahwa hasil implementasi pada Subbab 4.1-4.4 bekerja sesuai spesifikasi dan bahwa angka dasbor sesuai dengan sumber data.

### 4.5.1 Lingkungan, Dataset, dan Kriteria Pengujian

Isi yang dibahas:

- lingkungan pengujian lokal;
- PostgreSQL integration test;
- Seed 2 yang terdiri atas sesi Pemula dan Mahir, masing-masing empat pemain;
- simulator sebagai pengirim event pengganti IDN;
- kriteria PASS/FAIL;
- commit atau versi sistem yang diuji;
- perintah gerbang verifikasi yang digunakan.

Bukti yang disarankan:

- **Tabel 4.15 Lingkungan dan Data Uji**;
- **Tabel 4.16 Ringkasan Skenario Seed 2**.

Angka jumlah test harus diambil dari eksekusi final terbaru, bukan dari dokumen baseline lama yang masih memuat 393 atau 492 test.

### 4.5.2 Hasil Pengujian Fungsional RESTful API

Kelompokkan test case, jangan menyalin seluruh Postman collection:

- autentikasi dan registrasi;
- pengelolaan pemain;
- lifecycle sesi dan setup;
- ruleset;
- event tunggal dan batch;
- pagination event/transaksi;
- analytics dan recompute;
- error handling.

Bukti yang disarankan:

- **Tabel 4.17 Hasil Black-box Testing RESTful API** dengan kolom kode uji, skenario, input/kondisi, hasil yang diharapkan, hasil aktual, status.

### 4.5.3 Hasil Pengujian Integrasi dan Konsistensi State

Skenario penting:

- setup Pemula dan Mahir valid;
- revisi setup, idempotensi, roster lock, start lock, dan start bersamaan;
- event berurutan dan konflik sequence;
- dua aksi sama diterima;
- satu aksi lalu `AkhirGiliran` ditolak;
- Minggu tanpa dua aksi pemain;
- saldo, inventory, transaksi, pinjaman, asuransi, risiko, tujuan, dan skor konsisten;
- event legacy tetap tersimpan tetapi tidak memengaruhi hasil baru;
- recompute menghasilkan analitik yang sama untuk dataset yang sama.

Bukti yang disarankan:

- **Tabel 4.18 Hasil Pengujian Integrasi dan State**.

### 4.5.4 Hasil Pengujian Ruleset, RBAC, dan Privasi

Isi yang diuji:

- create/update/version/activate/delete ruleset;
- validasi mode dan parameter;
- penguncian versi ruleset pada sesi;
- akses Instruktur dan Player;
- penolakan `401`/`403`;
- privasi misi pada setup, event, analitik, dan UI;
- rate limit login.

Bukti yang disarankan:

- **Tabel 4.19 Hasil Pengujian Ruleset dan Hak Akses**.

### 4.5.5 Hasil Validasi Perhitungan Analitik

Isi yang diuji:

- perbandingan variabel basis data/proyeksi dengan payload API;
- substitusi angka aktual dengan formula;
- perbandingan hasil manual, API, dan UI;
- pembagi nol dan data belum cukup;
- perbedaan Pemula/Mahir;
- komposisi skor dan penalti;
- status capaian Misi Koleksi, termasuk kondisi selesai, belum selesai, dan data belum tersedia;
- nilai deterministik setelah recompute.

Bukti yang disarankan:

- **Tabel 4.20 Hasil Validasi Metrik Analitik**;
- maksimal empat contoh perhitungan lengkap di badan bab; sisanya pada lampiran.

### 4.5.6 Hasil Pengujian UI Fungsional dan Responsif

Isi yang diuji:

- login dan navigasi sesuai role;
- halaman beranda, sesi, detail sesi, direktori pemain, detail pemain, ruleset, buku aturan, Privasi, dan Ketentuan;
- viewport desktop dan ponsel;
- tidak ada overflow horizontal halaman;
- tabel memiliki scroll lokal;
- warna pemain konsisten;
- accordion plus/minus, keyboard, dan `localStorage`;
- komponen Mahir tidak muncul pada Pemula;
- filter histori perjalanan menurut semua event, sistem, pemain, dan tanggal;
- event baru tampil melalui polling timeline 10 detik dan statistik beranda 30 detik tanpa duplikasi.

Bukti yang disarankan:

- **Tabel 4.21 Hasil Pengujian UI dan Viewport**;
- satu pasangan screenshot desktop dan ponsel untuk halaman paling kompleks.

### 4.5.7 Verifikasi Teknis Pendukung dan Produksi

Isi yang boleh dimasukkan sebagai bukti tambahan:

- build tanpa warning/error;
- audit dependency;
- Docker image build;
- health check seluruh service;
- domain produksi dan legal page dapat diakses;
- `/metrics` publik ditolak;
- smoke login dan pembacaan dua sesi Seed 2;
- uji performa otomatis sebagai pemeriksaan teknis pendukung.

Catatan penting: Bab III menyatakan penelitian tidak menilai performa komputasi sebagai fokus penelitian. Oleh sebab itu, hasil performa ditempatkan sebagai verifikasi teknis tambahan dan tidak dijadikan klaim utama atau kontribusi penelitian.

### 4.5.8 Rekapitulasi Hasil Pengujian

Isi yang dibahas:

- jumlah test final dan jumlah lulus;
- ringkasan PASS per modul;
- jumlah skenario E2E;
- status Seed 2;
- status integrasi, UI, API, analitik, ruleset, RBAC, dan deployment;
- temuan residual/non-blocker.

Bukti yang disarankan:

- **Tabel 4.22 Rekapitulasi Hasil Pengujian Sistem**.

Jangan berhenti pada kalimat “semua test lulus”. Jelaskan fungsi apa yang dibuktikan oleh kelompok test tersebut.

---

## 4.6 Pembahasan

### Tujuan subbab

Melakukan analisis, sintesis, dan evaluasi hasil, bukan mengulang isi Subbab 4.1-4.5.

### 4.6.1 Pembahasan Kemampuan Back-end Menjaga Data dan State Permainan

Jawaban terhadap Rumusan Masalah 1 perlu membahas:

- arsitektur event terstruktur mengurangi pencatatan manual yang tidak terstruktur;
- validasi sebelum penyimpanan mencegah data tidak sah menjadi sumber analitik;
- idempotensi dan sequence menjaga retry serta urutan event;
- version lock dan event source of truth membuat state dapat ditelusuri/dihitung ulang;
- setup fisik yang dikonfirmasi menjaga backend tetap sesuai keadaan meja;
- bukti pengujian yang mendukung setiap kesimpulan;
- keterbatasan karena kebenaran kejadian fisik tetap bergantung pada data yang dilaporkan IDN/simulator.

### 4.6.2 Pembahasan Fleksibilitas dan Batas Manajemen Ruleset

Jawaban terhadap Rumusan Masalah 2 perlu membahas:

- versioning memungkinkan variasi parameter tanpa menghilangkan konteks sesi lama;
- validasi dan pemisahan core rules mencegah konfigurasi keluar dari mekanika dasar;
- penguncian versi membuat analitik antar sesi dapat dibaca bersama konteks aturan;
- fleksibilitas bukan berarti semua aturan dapat diubah;
- bukti pengujian create/update/activate/delete dan mismatch ruleset;
- implikasi bagi instruktur dalam menyiapkan kondisi permainan berbeda.

### 4.6.3 Pembahasan Keterlacakan Analitik dan Dukungan Refleksi

Jawaban terhadap Rumusan Masalah 3 perlu membahas:

- event diubah menjadi variabel fisik dan metrik turunan yang dapat ditelusuri;
- provenance dan substitusi angka membantu pengguna memahami asal hasil;
- ringkasan sesi mendukung observasi antar pemain;
- halaman pemain mendukung refleksi keputusan individual;
- histori keputusan dan filter timeline per pemain/tanggal membantu menelusuri urutan tindakan;
- pemilihan halaman sesi, ruleset yang terkunci pada sesi, dan halaman pemain memberi konteks terhadap interpretasi tanpa diklaim sebagai satu filter global;
- status capaian Misi Koleksi menghubungkan kepemilikan kebutuhan dengan komponen penalti Poin Kebahagiaan;
- `null` mencegah interpretasi nol palsu;
- mode-specific rendering mencegah metrik tidak relevan tampil;
- indikator membantu refleksi, tetapi tidak membuktikan tingkat atau peningkatan literasi finansial.

### 4.6.4 Keterkaitan Hasil dengan Landasan Teori dan Penelitian Terdahulu

Hubungkan hasil secara analitis dengan Bab II:

- **Hybrid digital boardgame**: sistem digital mendukung pencatatan dan informasi tanpa menggantikan permainan fisik;
- **kualitas data**: validasi event mengurangi risiko kesalahan input sebelum agregasi;
- **learning analytics dashboard**: histori, provenance, dan ringkasan digunakan untuk monitoring serta refleksi, bukan hanya pelaporan angka;
- **closed-loop learning analytics**: data dan analitik menyediakan dasar tindakan instruktur, misalnya memilih atau menyusun ruleset berikutnya;
- **ruleset dinamis**: variasi parameter disimpan bersama sesi agar hasil tidak kehilangan konteks;
- **DSR dan prototyping iteratif**: artefak diperbaiki berdasarkan ketidaksesuaian rulebook, kontrak, formula, tampilan, dan hasil test.

Hindari menulis “hasil ini sejalan dengan penelitian X” tanpa menjelaskan bagian mana yang sejalan, berbeda, atau dikembangkan lebih lanjut.

### 4.6.5 Kontribusi Penelitian

Kontribusi yang dapat diklaim:

- arsitektur pencatatan event untuk permainan fisik Cashflowpoly;
- lifecycle setup fisik yang dapat direvisi tetapi terkunci saat sesi mulai;
- manajemen ruleset berversi dan terkunci per sesi;
- analitik yang dapat ditelusuri dari formula sampai event/proyeksi sumber;
- pemisahan variabel permainan fisik dan metrik turunan;
- dasbor bagi Instruktur dan Player dengan privasi serta cakupan mode yang konsisten;
- artefak API, basis data, UI, dokumentasi, Postman, dan deployment yang dapat diuji ulang.

### 4.6.6 Keterbatasan dan Ancaman Validitas

Keterbatasan yang wajib ditulis secara eksplisit:

- integrasi diuji dengan simulator/Seed 2, bukan aplikasi IDN sebenarnya;
- sistem tidak dapat memverifikasi sendiri apakah kejadian fisik benar-benar terjadi tanpa input yang benar;
- dataset Seed 2 bersifat skenario terkontrol, bukan data eksperimen pembelajaran nyata;
- penelitian tidak melakukan pre-test/post-test literasi finansial;
- penelitian tidak melakukan studi pengguna untuk usability, kepuasan, motivasi, atau engagement;
- tidak ada analisis prediktif atau asesmen psikologis;
- hasil analitik historis dapat berubah jika formula diperbaiki, walaupun event sumber tetap;
- uji performa otomatis tidak sama dengan uji penggunaan kelas jangka panjang;
- variasi ruleset dapat memengaruhi keterbandingan nilai antar sesi, sehingga interpretasi harus menyertakan konteks versi ruleset.

### 4.6.7 Implikasi Penggunaan dan Pengembangan Lanjutan

Isi yang dibahas:

- Instruktur dapat memakai histori dan analitik sebagai bahan diskusi/refleksi setelah sesi;
- ruleset berikutnya dapat dipilih berdasarkan temuan sesi sebelumnya;
- integrasi IDN nyata dan penelitian pengguna menjadi tahap lanjutan;
- eksperimen pre-test/post-test diperlukan bila penelitian berikutnya ingin mengukur dampak terhadap literasi finansial;
- observability eksternal dan uji kelas jangka panjang dapat meningkatkan kesiapan operasional.

---

## 4.7 Rangkuman Bab

### Contoh isi

> Berdasarkan hasil implementasi dan pengujian, sistem yang dikembangkan telah menghasilkan layanan RESTful API, basis data, modul manajemen ruleset, mekanisme analitik, dan dasbor web yang saling terintegrasi. API menerima event terstruktur serta menerapkan validasi akses, urutan, idempotensi, aturan domain, dan konsistensi state. Modul ruleset menyediakan konfigurasi berversi yang dapat dikelola tanpa mengubah aturan inti dan dikunci pada setiap sesi. Data event yang sah diolah menjadi variabel permainan fisik dan metrik turunan yang dapat ditelusuri melalui angka pembentuk, rumus, serta sumber datanya. Hasil pengujian fungsional dan integrasi menunjukkan bahwa fungsi utama sistem bekerja sesuai spesifikasi pada skenario Pemula dan Mahir. Meskipun demikian, evaluasi dilakukan menggunakan simulator dan data skenario sehingga penelitian ini belum mengukur dampak penggunaan sistem terhadap peningkatan literasi finansial maupun pengalaman pengguna dalam kondisi kelas nyata.

Rangkuman Bab IV menjadi pengantar menuju Bab V. Jangan memasukkan saran baru secara panjang karena saran ditempatkan pada Bab V.

---

## C. Daftar tabel yang disarankan

| Nomor | Judul |
|---|---|
| Tabel 4.1 | Komponen Artefak dan Fungsinya |
| Tabel 4.2 | Ringkasan Iterasi dan Perubahan Artefak |
| Tabel 4.3 | Lingkungan Implementasi |
| Tabel 4.4 | Tabel Basis Data dan Tanggung Jawabnya |
| Tabel 4.5 | Matriks Hak Akses |
| Tabel 4.6 | Aturan Setup Pemula dan Mahir |
| Tabel 4.7 | Contoh Event Valid dan Tidak Valid |
| Tabel 4.8 | Mekanisme Penjagaan Konsistensi |
| Tabel 4.9 | Pemetaan Kondisi Galat dan Respons API |
| Tabel 4.10 | Aturan Inti dan Parameter yang Dapat Dikonfigurasi |
| Tabel 4.11 | Skenario Validasi Ruleset |
| Tabel 4.12 | Variabel Permainan Fisik dan Sumber Datanya |
| Tabel 4.13 | Definisi Operasional Metrik Turunan |
| Tabel 4.14 | Validasi Manual Sampel Formula |
| Tabel 4.15 | Lingkungan dan Data Uji |
| Tabel 4.16 | Ringkasan Skenario Seed 2 |
| Tabel 4.17 | Hasil Black-box Testing RESTful API |
| Tabel 4.18 | Hasil Pengujian Integrasi dan State |
| Tabel 4.19 | Hasil Pengujian Ruleset dan Hak Akses |
| Tabel 4.20 | Hasil Validasi Metrik Analitik |
| Tabel 4.21 | Hasil Pengujian UI dan Viewport |
| Tabel 4.22 | Rekapitulasi Hasil Pengujian Sistem |

Tidak semua tabel harus panjang. Gabungkan tabel yang informasinya sangat sedikit dan pindahkan data teknis lengkap ke lampiran.

## D. Daftar gambar yang disarankan

| Nomor | Judul |
|---|---|
| Gambar 4.1 | Arsitektur Implementasi Cashflowpoly |
| Gambar 4.2 | Alur Data dari Permainan Fisik hingga Dasbor |
| Gambar 4.3 | ERD Inti Sistem |
| Gambar 4.4 | Sequence Diagram Lifecycle Sesi |
| Gambar 4.5 | State Diagram Sesi |
| Gambar 4.6 | Pipeline Validasi Event |
| Gambar 4.7 | Model Versioning Ruleset |
| Gambar 4.8 | Alur Pengelolaan Ruleset |
| Gambar 4.9 | Halaman Daftar Ruleset |
| Gambar 4.10 | Halaman Form dan Detail Ruleset |
| Gambar 4.11 | Pipeline Event hingga Dasbor Analitika |
| Gambar 4.12 | Tampilan Rincian Cara Menghitung |
| Gambar 4.13 | Dasbor Ringkasan Sesi |
| Gambar 4.14 | Jejak Perjalanan Permainan |
| Gambar 4.15 | Perbandingan Komponen Poin Pemain |
| Gambar 4.16 | Ringkasan Statistik Pemain |
| Gambar 4.17 | Cerita di Balik Hasil Pemain |
| Gambar 4.18 | Data Permainan Lengkap dan Provenance |

Setiap gambar harus dirujuk dan dijelaskan dalam paragraf. Jangan menempatkan gambar hanya sebagai dekorasi.

---

## E. Urutan penulisan yang paling efisien

1. Jalankan gerbang pengujian final dan simpan keluarannya agar jumlah test serta status benar-benar mutakhir.
2. Buat Tabel 4.1 dan tabel pemetaan rumusan masalah.
3. Buat diagram arsitektur, lifecycle sesi, ruleset, dan pipeline analitik.
4. Tulis Subbab 4.2-4.4 dari bukti implementasi project.
5. Ambil screenshot UI final dalam Bahasa Indonesia pada desktop dan ponsel.
6. Susun tabel hasil pengujian 4.17-4.22 dari output final, bukan dokumen baseline lama.
7. Tulis pembahasan 4.6 dengan pola: temuan -> bukti -> makna -> hubungan teori -> batasan.
8. Tulis 4.1 dan 4.7 setelah seluruh isi lain selesai agar ringkasannya akurat.
9. Sinkronkan kesimpulan Bab V dengan jawaban pada 4.6.1-4.6.3.

Estimasi panjang yang masuk akal adalah **25-35 halaman**, tidak termasuk lampiran. Porsi yang disarankan:

- 4.1: 2-3 halaman;
- 4.2: 5-7 halaman;
- 4.3: 3-4 halaman;
- 4.4: 6-8 halaman;
- 4.5: 5-7 halaman;
- 4.6: 4-6 halaman;
- 4.7: sekitar 1 halaman.

---

## F. Klaim yang aman dan klaim yang harus dihindari

### Klaim yang didukung project

- sistem menerima dan memvalidasi event permainan secara terstruktur;
- sistem menjaga idempotensi, urutan, lifecycle sesi, dan konsistensi proyeksi pada skenario uji;
- ruleset dapat dikelola dalam versi yang dikunci per sesi;
- analitik dapat ditelusuri ke data sumber dan formula;
- UI membedakan akses Instruktur/Player serta mode Pemula/Mahir;
- fungsi utama lulus pengujian otomatis, integrasi, dan E2E pada lingkungan yang diuji.

### Klaim yang harus dihindari

- “sistem terbukti meningkatkan literasi finansial”;
- “dasbor terbukti mudah digunakan/disukai pengguna”;
- “integrasi dengan IDN telah berhasil” jika yang diuji hanya simulator;
- “sistem mengetahui kartu yang tersedia di pasar/deck fisik”;
- “seluruh kesalahan input fisik dapat dicegah”;
- “sistem aman sepenuhnya” atau “tanpa bug”;
- “kinerja telah terbukti pada penggunaan kelas jangka panjang”.

Gunakan istilah **indikator perilaku finansial berbasis aktivitas permainan**, bukan “nilai literasi finansial”, kecuali terdapat instrumen evaluasi terpisah yang memang mengukur literasi finansial.

---

## G. Ketidakkonsistenan laporan yang perlu dibereskan sebelum Bab IV ditulis penuh

1. Bab IV dan Bab V pada PDF saat ini masih kosong.
2. Bab III menyatakan fokus pengujian tidak mencakup performa komputasi dan pengalaman pengguna subjektif. Karena project memiliki uji performa dan E2E, keduanya harus ditulis sebagai verifikasi teknis pendukung, bukan evaluasi utama penelitian.
3. Laporan menyebut IDN “jika tersedia” dan menetapkan simulator sebagai alternatif. Bab IV harus menyebut alat yang benar-benar dipakai; jangan memberi kesan ada pengujian integrasi langsung dengan IDN jika belum dilakukan.
4. Beberapa bagian memakai istilah “metrik literasi finansial”. Istilah tersebut perlu diseragamkan menjadi “indikator perilaku finansial berbasis aktivitas permainan” agar sesuai dengan Batasan Masalah butir 8.
5. Dokumen pengujian lama memuat jumlah test yang berbeda-beda karena berasal dari baseline berbeda. Bab IV hanya boleh memakai hasil eksekusi final pada commit yang secara eksplisit disebutkan.
6. Bab III menyebut “pembuatan, pembaruan, penghapusan, dan aktivasi” ruleset. Bab IV perlu menjelaskan bahwa pembaruan menghasilkan versi immutable baru dan penghapusan dibatasi ketika versi telah digunakan.
7. Bab IV perlu konsisten bahwa setup berasal dari pembagian fisik yang dicatat IDN/simulator, bukan pembagian otomatis backend.
8. Klaim near real-time perlu dijelaskan sebagai pembaruan setelah event/akhir giliran diterima, bukan streaming kontinu.
9. Halaman biodata pada PDF masih berisi identitas contoh dari template dan harus diganti sebelum laporan final.
