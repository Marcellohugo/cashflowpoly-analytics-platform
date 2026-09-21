# Panduan Manual Pengguna Dashboard
## Dasbor Web Analitika MVC Cashflowpoly

### Informasi Dokumen
- **Nama Dokumen**: Panduan Manual Pengguna Dashboard
- **Versi**: 1.6
- **Tanggal**: 13 September 2026
- **Penyusun**: Marco Marcello Hugo

---

## 1. Tujuan
Dokumen ini disusun untuk memandu pengguna (Instruktur dan Player) dalam berinteraksi dengan antarmuka Web Analitika MVC (Razor Views). Dokumen ini berfokus pada navigasi menu dashboard, cara membaca grafik, visualisasi metrik literasi keuangan, serta pengelolaan ruleset.

> [!NOTE]
> Panduan ini khusus untuk penggunaan **Web Dashboard MVC**. Untuk instruksi pengiriman event permainan dari Game Client/IDN atau perangkat uji, silakan merujuk ke spesifikasi skenario terkontrol di [01-05-spesifikasi-skenario-simulasi.md](../01-Spesifikasi/01-05-spesifikasi-skenario-simulasi.md).

---

## 2. Peran Pengguna pada Web Dashboard

### 2.1 Peran Instruktur (`INSTRUCTOR`)
Instruktur memakai dashboard dengan hak akses untuk:
- Memantau performa pembelajaran dan capaian misi peserta pada sesi miliknya.
- Membuat, mengubah (*edit*), dan mengaktifkan versi aturan baru (*ruleset*).
- Melihat statistik lintas sesi pemain yang menjadi peserta sesi miliknya. Endpoint observabilitas dan log keamanan tersedia melalui API; log keamanan dibatasi pada akun instruktur yang sedang masuk.

### 2.2 Peran Pemain (`PLAYER`)
Player memiliki hak akses terbatas yang hanya diizinkan untuk:
- Melihat daftar sesi permainan yang mencantumkan akun mereka.
- Melihat performa individu mereka sendiri secara detail.
- Melihat statistik lintas sesi untuk dirinya sendiri, dengan satu mode permainan setiap tampilan.
- Membaca panduan buku aturan (*rulebook*).

---

## 3. Navigasi dan Alur Tampilan Utama

### 3.1 Pintu Masuk Halaman (URL Utama)
Dasbor dapat diakses melalui browser pada port UI MVC (default pada environment development):
- **Halaman Utama (Home)**: `http://localhost:5203/`
- **Daftar Sesi**: `http://localhost:5203/sessions`
- **Statistik Pemain**: `http://localhost:5203/statistics`; daftar peserta tersedia pada kartu di halaman Sesi Permainan.
- **Manajemen Ruleset**: `http://localhost:5203/rulesets` (Instruktur)
- **Buku Aturan (Rulebook)**: `http://localhost:5203/rulebook`

---

## 4. Cara Penggunaan Halaman Dasbor

### 4.1 Halaman Autentikasi (Masuk & Daftar)
1. Buka halaman utama dasbor. Sistem akan mengarahkan Anda ke `/auth/login` secara otomatis jika belum masuk.
2. **Daftar Akun Baru**: Pilih Player atau Instruktur pada `/auth/register`. Registrasi Instruktur diizinkan secara default; operator dapat menonaktifkannya melalui `Auth:AllowPublicInstructorRegistration=false`. Kata sandi minimal 12 karakter dan maksimal 72 byte UTF-8; karakter multibyte dapat memakai lebih dari satu byte.
3. **Masuk**: Isi kredensial username dan password yang valid.
4. *Catatan Teknis*: JWT menjadi claim dalam tiket cookie autentikasi yang dilindungi ASP.NET Core Data Protection (terenkripsi). Browser menyimpan cookie `HttpOnly`; server UI membaca tiket tersebut untuk memanggil API. Anda tidak perlu menyalin token Bearer secara manual.

### 4.2 Halaman Daftar Sesi (`/sessions`)
Halaman ini menampilkan kartu sesi yang dapat dibuka untuk melihat peserta. Tombol Rincian membuka detail sesi; ringkasan Pemain dipantau menghitung peserta unik. Tombol analitika pada akun Player hanya tersedia untuk dirinya sendiri. Sesi dibatasi sesuai akun yang sedang masuk:
1. **Instruktur**: Dapat melihat seluruh sesi yang mereka buat.
2. **Player**: Hanya dapat melihat sesi di mana akun mereka terdaftar sebagai peserta (`session_participants`).
3. Label status sesi akan menunjukkan status aktual: `CREATED` (dibuat), `STARTED` (berjalan), atau `ENDED` (selesai).
4. Klik tombol **Detail** pada salah satu baris sesi untuk melihat analitika sesi tersebut.

### 4.3 Halaman Detail Sesi (`/sessions/{id}`)
Halaman utama pemantauan analitika sesi permainan yang memuat:
1. **Kartu Ringkasan Metrik Sesi**:
   - **Total Pemasukan**: Akumulasi seluruh uang masuk dari transaksi di sesi tersebut.
   - **Total Pengeluaran**: Akumulasi seluruh uang keluar.
   - **Net Cashflow**: Selisih bersih pemasukan dikurangi pengeluaran.
   - **Jumlah Event**: Jumlah event permainan yang berhasil diproses di sesi tersebut.
2. **Tabel Peserta Sesi**: Menampilkan daftar pemain yang ikut serta beserta metrik ringkas mereka:
   - Poin Kebahagiaan saat ini.
   - Total Donasi.
   - Total Emas yang dimiliki.
   - Indikator pinjaman belum lunas (ditandai jika pemain masih memiliki utang).
3. **Timeline Event (Riwayat Perjalanan)**: Menampilkan event permainan secara berurutan dan memeriksa pembaruan berkala. Event setup dengan `day_index=0` ditempatkan pada kotak **GO**, bukan Hari 1.
4. Klik pada nama pemain di tabel untuk masuk ke halaman detail performa individu.

### 4.4 Halaman Detail Performa Pemain (`/sessions/{id}/players/{userId}`)
Menyajikan visualisasi mendalam tentang performa literasi keuangan seorang pemain:
1. **Ringkasan Statistik Pemain**: Menampilkan metrik inti dan prioritas pembahasan tanpa mengulang indikator yang sama.
2. **Cerita di Balik Hasil Pemain**: Menjelaskan arti setiap metrik. Kontrol **Lihat angka pembentuk dan rumus** menampilkan sumber data, nama variabel yang konsisten, rumus, substitusi angka aktual, dan hasil perhitungannya.
3. **Data Permainan Lengkap**: Menampilkan rincian transaksi, urutan event, inventori, dan data pembentuk lain untuk audit hasil.
4. Ketiga bagian utama memakai *accordion* native `<details>/<summary>` sehingga dapat dibuka atau ditutup tanpa kehilangan konteks.
5. Analitika khusus mode Mahir—misalnya pinjaman, asuransi, tabungan, tujuan finansial, dan risiko kehidupan—hanya muncul pada sesi mode Mahir.

### 4.5 Halaman Statistik Pemain (`/statistics`)
1. Instruktur mengetik nama pada kolom **Pemain**, memilih peserta yang muncul, lalu menekan **Tampilkan**. Pilihan hanya berasal dari sesi miliknya. Nama yang sama dibedakan dengan UUID; masukan ambigu harus dipilih lebih spesifik. Player langsung melihat dirinya sendiri.
2. Pilih **Pemula** atau **Mahir** dan status sesi, lalu terapkan filter. Grafik selalu membandingkan satu pemain dalam satu mode; metrik khusus Mahir tidak dicampur dengan Pemula.
3. Gunakan tombol kelompok metrik untuk membuka bagian yang ingin dipelajari. Arahkan pointer (**hover**) ke titik grafik atau fokuskan dengan tombol **Tab** untuk membaca nama sesi, nilai, dan artinya di bawah grafik. Rincian menutup saat pointer meninggalkan titik, fokus berpindah, atau **Escape** ditekan. Pada layar sentuh, baca nilai melalui tabel **Nilai dan sumber**; ketukan tidak membuka atau mengunci rincian titik.
4. Buka **Nilai dan sumber** pada grafik untuk melihat tabel pembentuk. Tabel menampilkan **5 baris per halaman**; gunakan tombol Sebelumnya/Berikutnya untuk menelusuri sisanya.
5. Sesi persiapan (`CREATED`) atau hasil yang belum tersedia ditampilkan sebagai data belum tersedia, bukan nol. Daftar rincian sesi menyediakan tautan analitika bila hasil tersedia dan tautan detail sesi bila belum.
6. Waktu ditampilkan menurut zona browser beserta label zona. Sebelum JavaScript berjalan, teks waktu memakai UTC secara eksplisit.

### 4.6 Halaman Manajemen Ruleset (`/rulesets`)
Tempat Instruktur mengonfigurasi aturan permainan yang akan diikat pada sesi:
1. **Daftar Ruleset**: Menampilkan daftar konfigurasi aturan.
2. **Lihat Versi**: Membuka rincian komponen ruleset seperti harga masakan, biaya sewa, bunga pinjaman, dan nilai donasi.
3. **Membuat Ruleset Baru** *(Khusus Instruktur)*: Klik **Buat Ruleset**, isi nama (maksimal 120 karakter) dan konfigurasi untuk mode pemula atau mahir, lalu simpan.
4. **Aktivasi Versi**: Klik tombol **Aktifkan** pada versi ruleset tertentu untuk menandainya sebagai versi `ACTIVE`.
5. *Catatan Penting*: Ruleset yang telah terikat pada sesi permainan, termasuk persiapan, akan dikunci (`is_locked_by_session = true`) dan tidak dapat diedit atau dihapus demi menjaga integritas data permainan.

Player yang membuka `/rulesets` atau rute turunannya diarahkan ke `/sessions`. Hak baca ruleset melalui API tetap tersedia sesuai scope; menu web manajemen ruleset hanya untuk Instruktur.

---

## 5. Masalah Umum (Troubleshooting) Dasbor

### 5.1 Kesalahan: "API tidak dapat diakses"
*   **Gejala**: Dasbor menampilkan pesan kegagalan memuat data atau halaman galat layanan.
*   **Penyebab**: Koneksi dari UI MVC ke backend API terputus.
*   **Solusi**:
    1. Pastikan server backend API (`Cashflowpoly.Api`) telah berjalan.
    2. Cek apakah alamat backend di `src/Cashflowpoly.Ui/appsettings.Development.json` (bagian `ApiBaseUrl`) sudah sesuai dengan alamat listen backend API.

### 5.2 Kesalahan: Dasbor tidak menampilkan data analitika terbaru
*   **Gejala**: Event permainan sudah dikirim oleh Game Client/IDN atau perangkat uji, tetapi metrik di dasbor belum berubah.
*   **Penyebab**: Event dapat tertolak, belum diterima API, atau halaman analitika belum dimuat ulang. Timeline memeriksa pembaruan berkala; angka pada halaman analitika perlu dimuat ulang untuk mengambil hasil terbaru. Donasi yang belum lengkap tetap dirahasiakan; banner menjelaskan bahwa angka sementara belum memasukkannya.
*   **Solusi**:
    1. Periksa respons pengiriman event pada Klien Game/IDN (`error_code`, `details`, dan `trace_id`); payload event yang ditolak tidak disimpan sebagai data permainan.
    2. Instruktur pemilik sesi dapat memicu hitung ulang metrik (`recompute`) jika snapshot perlu diperbarui. Proses ini memakai event dan proyeksi yang sudah tersimpan; proses ini tidak membangun ulang saldo, inventori, atau proyeksi cashflow yang rusak. Ketidaksesuaian proyeksi perlu diperiksa operator dan diperbaiki melalui migrasi yang diuji sebelum recompute dijalankan.

### 5.3 Akses ditolak atau halaman tidak ditemukan
Respons 403/404 menampilkan penjelasan dan tombol ke Sesi Permainan/Beranda. Gangguan layanan menampilkan halaman galat dengan nomor permintaan; tidak perlu mengaktifkan mode pengembangan di produksi. Akun yang dinonaktifkan atau berubah peran akan diminta masuk kembali pada permintaan API berikutnya.
