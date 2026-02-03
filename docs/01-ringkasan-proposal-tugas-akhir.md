# Proposal Tugas Akhir  
**Rancang Bangun Dasbor Analitika dan Sistem Informasi Manajemen Ruleset untuk Gim Papan Cashflowpoly**

Marco Marcello Hugo  
NRP 5025221102  
Program Studi S-1 Teknik Informatika, FTEIC – ITS  

Dosen Pembimbing: Hadziq Fabroyir, S.Kom., Ph.D.  
Dosen Ko‑pembimbing: Dr. Eng. Darlis Herumurti, S.Kom., M.Kom  

---

## Abstrak

Permainan papan Cashflowpoly berfungsi sebagai media latihan pengambilan keputusan finansial. Pemain melakukan transaksi, mengelola aset dan liabilitas, serta mengejar capaian misi. Implementasi permainan masih bergantung pada pencatatan manual, sehingga muncul risiko kesalahan input dan keterbatasan observabilitas proses belajar.

Penelitian ini mengembangkan sistem informasi analitik dan manajemen *ruleset* berbasis web yang:

- merekam aktivitas permainan sebagai rangkaian *event* terstruktur,  
- memvalidasi dan menyimpan data secara konsisten, serta  
- mengolah data menjadi metrik literasi finansial dan capaian misi yang tampil pada dasbor.

Pendekatan penelitian menggunakan Design Science Research (DSR) dengan pengembangan artefak melalui SDLC *prototyping* iteratif. Sistem tersusun dari lima komponen utama:

1. model data dan basis data,  
2. spesifikasi *event* dan RESTful API,  
3. modul manajemen *ruleset* berbasis konfigurasi dinamis,  
4. metode agregasi metrik dari log *event*, dan  
5. dasbor analitika untuk instruktur dan pemain.

Evaluasi sistem mencakup uji fungsional endpoint API, uji integrasi alur pemrosesan *event* dan konsistensi *state*, serta validasi dasbor dengan membandingkan metrik terhadap data di basis data. Luaran penelitian berupa layanan back‑end, modul manajemen *ruleset*, dan dasbor analitika yang mendukung pemantauan proses keputusan secara berbasis data.

**Kata kunci:** literasi finansial, analitika pembelajaran, *ruleset* dinamis, RESTful API, gim papan hibrida.

---

## 1. Pendahuluan

### 1.1 Latar Belakang

- Lingkungan keuangan modern menuntut individu mengambil banyak keputusan finansial.  
- Pendidikan literasi finansial meningkatkan pengetahuan, tetapi tidak selalu mengubah sikap dan perilaku.  
- Game‑Based Learning (GBL) dan kerangka Big‑G memadukan aktivitas bermain dengan refleksi terstruktur.  
- Cashflowpoly Entrepreneur Edition menyediakan konteks latihan keputusan finansial melalui skenario arus kas, aset–liabilitas, serta misi.  
- Implementasi berbasis papan masih lemah dari sisi pencatatan terstruktur, kualitas data, dan observabilitas proses keputusan.  
- Penelitian ini menawarkan sistem informasi analitik dan manajemen *ruleset* yang menjembatani permainan papan fisik dan analitika berbasis data.

### 1.2 Rumusan Masalah

1. Bagaimana merancang arsitektur back‑end berbasis RESTful API yang menerima, memvalidasi, dan menyimpan *event* permainan Cashflowpoly dari IDN atau simulator, serta menjaga konsistensi *state* permainan?  
2. Bagaimana mengembangkan modul manajemen *ruleset* berbasis konfigurasi dinamis yang memungkinkan instruktur mengelola parameter permainan tanpa mengubah aturan inti?  
3. Bagaimana merancang mekanisme pengolahan dan agregasi data dari log *event* menjadi metrik literasi finansial dan capaian misi serta menampilkannya melalui dasbor analitika berbasis web?

### 1.3 Tujuan

1. Merancang arsitektur RESTful API untuk integrasi *event* permainan dengan IDN/simulator dan penyimpanan data yang konsisten.  
2. Mengembangkan modul manajemen *ruleset* yang mendukung pembuatan, pembaruan, penghapusan, dan aktivasi *ruleset* tanpa modifikasi kode sumber.  
3. Merancang dan mengimplementasikan mekanisme agregasi data event menjadi metrik pembelajaran terukur serta membangun dasbor analitika yang menyediakan histori keputusan, filter, dan pengelompokan berdasarkan *ruleset* dan sesi permainan.

### 1.4 Batasan Masalah

- Sistem berfokus pada back‑end (RESTful API + basis data) dan dasbor web.  
- Penelitian tidak mengembangkan aplikasi klien baru (mobile/desktop) dan tidak merancang konten naratif IDN.  
- Modul *ruleset* hanya mengelola parameter konfigurasi, bukan mengubah aturan inti Cashflowpoly.  
- Sistem menerima data dalam bentuk *event* terstruktur dari IDN/simulator, tanpa sensor atau pemrosesan citra.  
- Pengujian integrasi menggunakan IDN (bila tersedia) atau simulator pengirim *event*.  
- Aktor sistem terbatas pada instruktur dan pemain dengan hak akses berbeda.  
- Dasbor menampilkan metrik literasi finansial, capaian misi, dan indikator proses; sistem tidak melakukan analitik prediktif atau asesmen psikologis.

### 1.5 Manfaat

#### 1.5.1 Manfaat Teoritis

- Menambah literatur mengenai arsitektur sistem informasi untuk hybrid board game dengan dukungan analitika.  
- Menyajikan studi kasus penerapan metagaming dan meta‑metagaming pada permainan literasi finansial.  
- Menjadi dasar penelitian lanjutan terkait hubungan ketersediaan data analitik dengan keberlanjutan media pembelajaran berbasis permainan.

#### 1.5.2 Manfaat Praktis

- Menyediakan alat bantu evaluasi bagi instruktur melalui dasbor analitika dan modul *ruleset*.  
- Memberi akses visualisasi performa personal bagi pemain untuk refleksi berbasis data.  
- Menawarkan solusi teknologi yang lebih sederhana dan hemat biaya dibanding pengembangan gim digital penuh.

#### 1.5.3 Manfaat Sosial

- Mendukung pembelajaran berbasis data di sekolah dengan infrastruktur terbatas.  
- Membiasakan pengambilan keputusan berbasis data pada peserta didik.  
- Mendorong modernisasi ekosistem pembelajaran melalui integrasi permainan papan dan layanan digital.

---

## 2. Tinjauan Pustaka (Ringkasan)

### 2.1 Konsep Utama

- **Literasi finansial:** pengetahuan, sikap, dan perilaku yang mendukung keputusan finansial yang sehat.  
- **Game‑Based Learning:** permainan sebagai lingkungan belajar yang menekankan keputusan, konsekuensi, dan refleksi.  
- **Adaptivitas & *ruleset* dinamis:** penyesuaian parameter permainan melalui pengelolaan *ruleset* multi‑versi.  
- **Hybrid board game & IDN:** pembagian peran antara papan fisik dan aplikasi digital (IDN) sebagai pengelola narasi dan pencatatan.  
- **Analitika pembelajaran & dasbor:** pemanfaatan jejak aktivitas (*event log*) untuk monitoring dan refleksi berbasis data.  
- **Data event & RESTful API:** *event* sebagai unit data utama; API sebagai kontrak pertukaran data yang konsisten.

### 2.2 Kesenjangan Penelitian

- Pengukuran literasi finansial banyak berfokus pada hasil (pre‑post), bukan proses keputusan selama permainan.  
- Implementasi adaptivitas sering berhenti di level desain, belum mewujud menjadi *ruleset* dinamis yang praktis dikelola fasilitator.  
- Dasbor analitika sering hanya menyajikan visualisasi tanpa keterhubungan dengan tindakan lanjutan (closed‑loop).  
- Integrasi hybrid board game dengan sistem informasi analitik untuk konteks sekolah di Indonesia masih terbatas.

### 2.3 Posisi dan Kontribusi

Penelitian ini mengisi kesenjangan dengan:

- memodelkan keputusan permainan sebagai *event* terstruktur,  
- mengembangkan modul *ruleset* dinamis dengan versioning dan aktivasi per sesi, serta  
- membangun dasbor analitika yang menautkan metrik pembelajaran dengan histori keputusan dan konteks *ruleset*.

---

## 3. Metodologi

### 3.1 Pendekatan Penelitian

- Menggunakan Design Science Research (DSR).  
- Artefak utama: RESTful API, basis data, modul manajemen *ruleset*, dan dasbor analitika.  
- Evaluasi artefak melalui pengujian fungsional dan integrasi.

### 3.2 Metode Pengembangan Sistem

- Menerapkan SDLC *prototyping* iteratif.  
- Setiap iterasi menghasilkan perbaikan pada model data, kontrak API, mekanisme *ruleset*, dan rancangan metrik.

### 3.3 Metode Pengumpulan Data

- **Studi literatur**: GBL, literasi finansial, analitika pembelajaran, RESTful API, *ruleset* dinamis.  
- **Analisis dokumen**: rulebook Cashflowpoly untuk memetakan aksi, giliran, kalender permainan, dan aturan poin.  
- **Penyusunan data uji**: skenario permainan yang menghasilkan *event* contoh untuk pengujian API dan dasbor.

### 3.4 Metode Perancangan dan Pemodelan

- **Arsitektur sistem:** integrasi IDN/simulator → RESTful API → basis data → dasbor analitika.  
- **Pemodelan data:** ERD dan skema relasional untuk entitas pemain, sesi, *event*, *ruleset*, dan metrik.  
- **Spesifikasi RESTful API:** daftar endpoint, skema request/response, kode status, dan validasi input.  
- **Desain modul *ruleset*:** struktur konfigurasi, versioning, dan relasi dengan sesi permainan.  
- **Desain dasbor:** metrik utama, layout visual, filter ruleset/sesi/pemain.

### 3.5 Metode Pengujian Sistem

- **Pengujian fungsional API (black‑box):** memeriksa struktur payload, validasi input, dan kode status.  
- **Pengujian integrasi:** mengirim *event* melalui IDN/simulator, memeriksa konsistensi *state* dan penerapan *ruleset* aktif.  
- **Validasi dasbor:** membandingkan metrik dan visualisasi dengan data di basis data.

---

## 4. Bahan dan Peralatan

### 4.1 Bahan Penelitian

- Rulebook Cashflowpoly Entrepreneur Edition.  
- Artikel jurnal dan prosiding terkait literasi finansial, GBL, hybrid board game, analitika pembelajaran, dan RESTful API.

### 4.2 Perangkat Lunak

| No | Perangkat Lunak                 | Fungsi Penggunaan                                                                 |
|----|---------------------------------|-----------------------------------------------------------------------------------|
| 1  | Windows 11 Home                 | Sistem operasi utama pengembangan dan pengujian.                                  |
| 2  | Google Chrome                   | Pengujian antarmuka web dan visualisasi dasbor.                                   |
| 3  | ASP.NET Core 10                 | Implementasi layanan RESTful API dan logika back‑end.                             |
| 4  | PostgreSQL                      | Basis data relasional untuk event, *ruleset*, sesi, dan metrik analitik.         |
| 5  | ASP.NET Core MVC (Razor Views) | Pembangunan dasbor analitika berbasis web.                                        |
| 6  | Tailwind CSS                    | Styling antarmuka dasbor yang konsisten dan responsif.                            |
| 7  | Swagger UI                      | Dokumentasi dan uji interaktif endpoint RESTful API.                              |
| 8  | Postman                         | Pengujian fungsional API dengan skenario request–response terkontrol.            |

### 4.3 Perangkat Keras

| No | Perangkat Keras      | Spesifikasi Utama                                               |
|----|----------------------|------------------------------------------------------------------|
| 1  | Laptop pengembangan  | ASUS ROG Zephyrus M16 GU603ZE                                   |
| 2  | Sistem operasi       | Windows 11 Home 64‑bit                                          |
| 3  | Prosesor             | Intel® Core™ i7‑12700H (12th Gen, 20 logical processors)        |
| 4  | Memori (RAM)         | 16 GB                                                           |
| 5  | Media penyimpanan    | SSD internal 512 GB                                             |

---

## 5. Urutan Pelaksanaan Penelitian (Ringkasan Tahapan)

1. **Studi literatur dan analisis rulebook**  
   - Mengkaji konsep teoretis dan aturan permainan Cashflowpoly.  

2. **Analisis kebutuhan sistem**  
   - Menyusun kebutuhan fungsional, data, pengguna, dan non‑fungsional.  

3. **Perancangan sistem**  
   - Merancang arsitektur, model data, spesifikasi API, modul *ruleset*, dan rancangan dasbor.  

4. **Implementasi back‑end dan basis data**  
   - Mengimplementasikan skema basis data dan layanan RESTful API.  

5. **Implementasi modul manajemen *ruleset***  
   - Mengimplementasikan struktur konfigurasi, versioning, dan fungsi CRUD + aktivasi *ruleset*.  

6. **Implementasi dasbor analitika**  
   - Mengimplementasikan tampilan, komponen visual, serta logika agregasi metrik.  

7. **Pengujian fungsional dan integrasi**  
   - Menjalankan skenario pengujian API, integrasi *event* dengan simulator, dan validasi dasbor.  

8. **Dokumentasi teknis dan penulisan laporan**  
   - Menyusun dokumentasi sistem dan laporan tugas akhir lengkap.

---

## 6. Penutup

Dokumen ringkasan ini merangkum isi Proposal Tugas Akhir “Rancang Bangun Dasbor Analitika dan Sistem Informasi Manajemen Ruleset untuk Gim Papan Cashflowpoly”. Dokumen ini dapat ditempatkan pada direktori `docs/` repositori sebagai referensi cepat bagi pembaca teknis maupun non‑teknis mengenai konteks penelitian, tujuan, metodologi, dan artefak yang akan dikembangkan.

