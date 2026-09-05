# Panduan Manual Pengguna Dashboard
## Dasbor Web Analitika MVC Cashflowpoly

### Informasi Dokumen
- **Nama Dokumen**: Panduan Manual Pengguna Dashboard
- **Versi**: 1.5
- **Tanggal**: 19 Agustus 2026
- **Penyusun**: Marco Marcello Hugo

---

## 1. Tujuan
Dokumen ini disusun untuk memandu pengguna (Instruktur dan Player) dalam berinteraksi dengan antarmuka Web Analitika MVC (Razor Views). Dokumen ini berfokus pada navigasi menu dashboard, cara membaca grafik, visualisasi metrik literasi keuangan, serta pengelolaan ruleset.

> [!NOTE]
> Panduan ini khusus untuk penggunaan **Web Dashboard MVC**. Untuk instruksi pengiriman event permainan dari Game Client/IDN atau perangkat uji, silakan merujuk ke spesifikasi skenario terkontrol di [01-05-spesifikasi-skenario-simulasi.md](../01-Spesifikasi/01-05-spesifikasi-skenario-simulasi.md).

---

## 2. Peran Pengguna pada Web Dashboard

### 2.1 Peran Instruktur (`INSTRUCTOR`)
Instruktur bertindak sebagai operator penuh pada dashboard dengan hak akses untuk:
- Memantau performa pembelajaran dan capaian misi seluruh sesi serta pemain.
- Membuat, mengubah (*edit*), dan mengaktifkan versi aturan baru (*ruleset*).
- Mengakses statistik observabilitas backend dan data log audit keamanan.

### 2.2 Peran Pemain (`PLAYER`)
Player memiliki hak akses terbatas yang hanya diizinkan untuk:
- Melihat daftar sesi permainan yang mencantumkan akun mereka.
- Melihat performa individu mereka sendiri secara detail.
- Membaca katalog ruleset yang sedang aktif atau ruleset default.
- Membaca panduan buku aturan (*rulebook*).

---

## 3. Navigasi dan Alur Tampilan Utama

### 3.1 Pintu Masuk Halaman (URL Utama)
Dasbor dapat diakses melalui browser pada port UI MVC (default pada environment development):
- **Halaman Utama (Home)**: `http://localhost:5203/`
- **Daftar Sesi**: `http://localhost:5203/sessions`
- **Direktori Pemain**: `http://localhost:5203/players`
- **Manajemen Ruleset**: `http://localhost:5203/rulesets`
- **Buku Aturan (Rulebook)**: `http://localhost:5203/rulebook`

---

## 4. Cara Penggunaan Halaman Dasbor

### 4.1 Halaman Autentikasi (Masuk & Daftar)
1. Buka halaman utama dasbor. Sistem akan mengarahkan Anda ke `/auth/login` secara otomatis jika belum masuk.
2. **Daftar Akun Baru**: Player dapat memakai tautan pendaftaran (`/auth/register`). Akun Instruktur disediakan administrator.
3. **Masuk**: Isi kredensial username dan password yang valid.
4. *Catatan Teknis*: Dasbor akan menyimpan token JWT di session server-side UI secara otomatis. Anda tidak perlu menyalin token Bearer secara manual pada browser.

### 4.2 Halaman Daftar Sesi (`/sessions`)
Halaman ini menampilkan seluruh sesi permainan yang terekam di sistem:
1. **Instruktur**: Dapat melihat seluruh sesi yang mereka buat.
2. **Player**: Hanya dapat melihat sesi di mana akun mereka terdaftar sebagai peserta (`session_participants`).
3. Kolom status sesi akan menunjukkan status aktual: `CREATED` (dibuat), `STARTED` (berjalan), atau `ENDED` (selesai).
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
3. **Timeline Event (Riwayat Perjalanan)**: Menampilkan event permainan secara berurutan (*real-time*). Event setup dengan `day_index=0` ditempatkan pada kotak **GO**, bukan Hari 1.
4. Klik pada nama pemain di tabel untuk masuk ke halaman detail performa individu.

### 4.4 Halaman Detail Performa Pemain (`/sessions/{id}/players/{userId}`)
Menyajikan visualisasi mendalam tentang performa literasi keuangan seorang pemain:
1. **Ringkasan Statistik Pemain**: Menampilkan metrik inti dan prioritas pembahasan tanpa mengulang indikator yang sama.
2. **Cerita di Balik Hasil Pemain**: Menjelaskan arti setiap metrik. Kontrol **Lihat angka pembentuk dan rumus** menampilkan sumber data, nama variabel yang konsisten, rumus, substitusi angka aktual, dan hasil perhitungannya.
3. **Data Permainan Lengkap**: Menampilkan rincian transaksi, urutan event, inventori, dan data pembentuk lain untuk audit hasil.
4. Ketiga bagian utama memakai *accordion* native `<details>/<summary>` sehingga dapat dibuka atau ditutup tanpa kehilangan konteks.
5. Analitika khusus mode Mahir—misalnya pinjaman, asuransi, tabungan, tujuan finansial, dan risiko kehidupan—hanya muncul pada sesi mode Mahir.

### 4.5 Halaman Manajemen Ruleset (`/rulesets`)
Tempat Instruktur mengonfigurasi aturan permainan yang akan diikat pada sesi:
1. **Daftar Ruleset**: Menampilkan daftar konfigurasi aturan.
2. **Lihat Versi**: Membuka rincian komponen ruleset seperti harga masakan, biaya sewa, bunga pinjaman, dan nilai donasi.
3. **Membuat Ruleset Baru** *(Khusus Instruktur)*: Klik **Buat Ruleset**, isi formulir konfigurasi untuk mode pemula atau mahir, lalu simpan.
4. **Aktivasi Versi**: Klik tombol **Aktifkan** pada versi ruleset tertentu untuk menandainya sebagai versi `ACTIVE`.
5. *Catatan Penting*: Ruleset yang telah terikat pada sesi permainan aktif otomatis akan dikunci (`is_locked_by_session = true`) dan tidak dapat diedit atau dihapus demi menjaga integritas data permainan.

---

## 5. Masalah Umum (Troubleshooting) Dasbor

### 5.1 Kesalahan: "API tidak dapat diakses"
*   **Gejala**: Dasbor menampilkan halaman error atau halaman kosong saat memuat data.
*   **Penyebab**: Koneksi dari UI MVC ke backend API terputus.
*   **Solusi**:
    1. Pastikan server backend API (`Cashflowpoly.Api`) telah berjalan.
    2. Cek apakah alamat backend di `src/Cashflowpoly.Ui/appsettings.Development.json` (bagian `ApiBaseUrl`) sudah sesuai dengan alamat listen backend API.

### 5.2 Kesalahan: Dasbor tidak menampilkan data analitika terbaru
*   **Gejala**: Event permainan sudah dikirim oleh Game Client/IDN atau perangkat uji, tetapi metrik di dasbor belum berubah.
*   **Penyebab**: Event tertolak karena masalah urutan (*out of sequence*) atau kegagalan sinkronisasi DB.
*   **Solusi**:
    1. Periksa tabel log validasi untuk melihat apakah event yang dikirim ditolak oleh API.
    2. Jika inkonsistensi terjadi akibat perbaikan database secara manual, hubungi administrator untuk memicu endpoint hitung ulang metrik (`recompute`).
