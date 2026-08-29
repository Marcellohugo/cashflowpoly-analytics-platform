# Keputusan Produk dan Arsitektur Cashflowpoly

Status: **final dan aktif**, diperbarui 30 Agustus 2026.

Dokumen ini mencatat alasan keputusan lintas produk dan arsitektur. Gunakan dokumen ini untuk menjawab **mengapa** sistem dibangun seperti sekarang; detail kontrak API, schema database, metrik, dan hasil pengujian tetap berada pada dokumen khususnya agar informasi teknis tidak diduplikasi. Riwayat penerapan tiap modul berada pada [Riwayat Perbaikan Modular dan Status Rilis](../03-Pengujian/03-04-riwayat-perbaikan-modular-dan-status-rilis.md).

Jika keputusan di sini bertentangan dengan source code aktif, perbedaan tersebut harus diperlakukan sebagai masalah dokumentasi atau implementasi dan diselesaikan pada commit yang sama—bukan dipilih diam-diam salah satunya.

## 1. Arah produk dan sumber kebenaran

### Keputusan

Cashflowpoly adalah platform analitik untuk permainan fisik. Backend mencatat dan memvalidasi kejadian yang dilaporkan dari meja permainan; backend bukan simulator kartu, deck, atau pasar.

### Dampak

- Kartu dibagikan secara fisik oleh instruktur.
- IDN mencatat pembagian dan kejadian yang benar-benar terjadi, lalu mengirimkannya ke API.
- Backend tetap bertanggung jawab atas validasi ruleset, saldo, giliran, peserta, lifecycle sesi, proyeksi, dan analitik.
- Data yang tidak pernah diamati atau dilaporkan tidak boleh dibuat seolah-olah terjadi.

### Acuan

Aturan permainan mengikuti buku aturan Cashflowpoly Entrepreneur Edition versi 02 yang diberikan pengguna.

## 2. Keputusan gameplay

### `LewatiOrder` tidak digunakan

`LewatiOrder` dihapus dari gameplay baru karena melewati pesanan bukan aksi yang sah dalam aturan permainan. Event lama boleh tetap berada di database untuk jejak historis, tetapi tidak dipakai dalam analitik baru.

### Dua aksi utama

- Hari normal wajib menyelesaikan dua aksi utama.
- Aksi yang sama boleh dilakukan dua kali.
- `AkhirGiliran` ditolak jika baru ada satu aksi utama yang sah.
- Dua token aksi merupakan komponen fisik dan tidak disimpan sebagai kartu virtual.

### Hari khusus

- Jumat digunakan untuk donasi sesuai ruleset.
- Sabtu digunakan untuk transaksi emas sesuai ruleset.
- Minggu adalah hari libur; tidak memerlukan dua aksi pemain.

### Pasar dan deck

Backend tidak mengacak, mengisi ulang, menentukan slot, atau memproyeksikan pasar/deck virtual. Event kartu hanya divalidasi terhadap katalog ruleset, mode, saldo, kepemilikan yang dilaporkan, urutan pemain, dan status sesi.

## 3. Keputusan setup fisik dan lifecycle sesi

### Alur yang dipilih

1. Instruktur membagikan kartu fisik sesuai buku aturan.
2. IDN mencatat hasil pembagian untuk setiap pemain.
3. IDN meminta validasi payload tanpa menyimpannya.
4. Instruktur memeriksa ringkasan pembagian.
5. IDN menyimpan revisi setup.
6. Setup pertama mengunci peserta dan ruleset.
7. Revisi sebelum start hanya boleh memperbaiki pembagian dengan roster dan ruleset yang sama.
8. Start mengunci revisi terbaru dan membentuk event setup secara atomik.

### Aturan mode

- Pemula dimulai dengan kas 20, satu bahan dan biaya kartunya, satu emas bernilai 5, serta satu misi unik.
- Mahir dimulai dengan kas 10, seluruh komponen Pemula, pinjaman Syariah 10, serta proteksi gratis dari sisi belakang Tie Breaker.
- Tie Breaker unik dan menentukan urutan pemain.
- Nilai kas dan nilai tetap diturunkan dari ruleset, bukan dari nama ruleset atau tebakan UI.

### Privasi misi

Instruktur dapat melihat semua misi. Player hanya dapat melihat misi miliknya sendiri, termasuk setelah sesi berakhir. Redaksi yang sama berlaku pada setup, event, analitik, dan tampilan UI.

## 4. Keputusan analitik

### Sumber angka

Setiap nilai analitik harus dapat ditelusuri ke setup, event, kartu, koin, transaksi, atau skor yang benar-benar tercatat. Analitik dipisahkan menjadi:

1. Variabel permainan fisik.
2. Metrik turunan yang dihitung dari variabel tersebut.

### Tampilan perhitungan

Di bawah setiap bagian cara menghitung harus ditampilkan:

- asal data;
- variabel pembentuk;
- angka aktual;
- substitusi rumus dengan angka sebenarnya;
- hasil akhir.

Contoh: `15 ÷ 10 × 100% = 150%`.

Nama variabel harus sama antara API, DTO, UI, formula, Postman, dan dokumentasi. Pembagi nol atau data belum cukup menghasilkan `null` dan keterangan “Belum dapat dihitung”, bukan angka nol palsu. Metrik khusus Mahir tidak dibuat dan tidak ditampilkan pada sesi Pemula.

### Data legacy

Event lama yang berasal dari `LewatiOrder`, pasar, atau deck tetap dipertahankan sebagai histori, tetapi diabaikan oleh proyeksi dan analitik baru. Event yang ditolak tidak menjadi gameplay event, snapshot, metrik, atau pelanggaran aturan.

## 5. Keputusan UI dan bahasa

### Halaman pemain

- `Ringkasan Statistik Pemain`, `Cerita di Balik Hasil Pemain`, dan `Data Permainan Lengkap` menjadi accordion.
- Ringkasan terbuka pada kunjungan pertama; bagian lain tertutup.
- Pilihan accordion terakhir disimpan di `localStorage`.
- Semua kontrol buka/tutup memakai bentuk plus/minus yang sama.
- Bahasa Indonesia menjadi default dan istilah harus mudah dipahami serta konsisten.
- Penjelasan “Di luar jatah aksi” dihapus karena tidak membantu pengguna memahami data dan muncul berulang.
- Kelompok khusus Mahir disembunyikan sepenuhnya pada sesi Pemula.
- Kartu diratakan menggunakan grid responsif; tabel panjang dapat digulir horizontal tanpa memperlebar viewport.
- Tabel dan kolom pemain memakai warna pemain yang sesuai: biru, kuning, hijau, dan ungu.

### Elemen redundan yang dihapus

- badge `13 metrik utama`;
- angka dekoratif pada `Sudut Analisis`;
- jumlah kelompok/data dan label `x item`;
- kartu `Pelanggaran Aturan` pada ringkasan sesi;
- `Total Sesi` dan `0%` pada kartu sesi berjalan di beranda;
- watermark angka `01`, `02`, dan `03` pada kartu statistik beranda.

### Informasi yang dipertahankan

Data permainan lengkap, variabel nol yang sah, status belum ada data, jejak aktivitas sampai hari yang tersedia termasuk tanggal 25, serta delapan komponen poin pemain tetap dapat dibuka melalui accordion.

## 6. Keputusan API, database, dan dokumentasi

- API tetap menggunakan `/api/v1`; tidak dibuat `/api/v2` atau lapisan kompatibilitas.
- Error mempertahankan `error_code`, `message`, `details`, dan `trace_id`.
- Pagination cursor hanya diterapkan pada event dan transaksi dengan `items`, `next_cursor`, serta `has_more`.
- Migrasi database memakai urutan versi dan checksum; migration yang sudah diterapkan tidak boleh diedit.
- Setup memiliki riwayat revisi dan status `EDITABLE`/`LOCKED`.
- `README.md`, dokumen kontrak API, dokumen arsitektur database, OpenAPI, Postman, dan seluruh dokumen `docs` harus memakai kontrak serta istilah yang sama.
- Dokumentasi harus menjelaskan alur IDN, privasi misi, mode Pemula/Mahir, data legacy, pagination, Seed 2, deployment, dan batasan backup.

## 7. Keputusan akun, legal, dan metadata

- JWT delapan jam, logout perangkat saat ini, dan rate limit login dipertahankan.
- Registrasi publik hanya untuk Player; pendaftaran Instructor publik tetap ditolak.
- Akun Seed 2 ditandai sebagai akun demo dan tetap mengikuti hak akses role-nya.
- Ditambahkan halaman Kebijakan Privasi dan Ketentuan Penggunaan sebagai proyek akademik, bukan badan usaha.
- Halaman privat diberi kebijakan `noindex`; canonical, Open Graph, robots, dan sitemap berasal dari `DOMAIN`.
- Produksi memakai `https://narafin.org`.

## 8. Keputusan infrastruktur dan rilis

- Produksi memakai Nginx dan Cloudflare Tunnel; API, UI, database, serta `/metrics` tidak diekspos langsung.
- Container berjalan non-root dan health check dipertahankan.
- Audit operasional disimpan 30 hari; payload gameplay yang ditolak tidak disimpan.
- Rilis dilakukan manual melalui SSH dengan lock, migrasi, Seed 2 idempoten, rekalkulasi analitik, health check, smoke test, dan rollback image aplikasi.
- Tidak digunakan GitHub Actions atau staging terpisah.
- Tidak dibuat backup database, restore test, atau rollback schema. Rollback hanya mengembalikan image aplikasi.

## 9. Keputusan data produksi dan Seed 2

Atas persetujuan pengguna, database produksi di-reset total pada volume yang digunakan deployment. Seluruh data lama menjadi tidak dapat dipulihkan karena tidak dibuat backup. Setelah reset, baseline schema, migrasi, Seed 2, dan rekalkulasi analitik dijalankan kembali.

Hasil verifikasi terakhir:

- sesi MAHIR: 4 pemain dan 255 event;
- sesi PEMULA: 4 pemain dan 212 event;
- API, UI, PostgreSQL, Nginx, dan Cloudflare Tunnel sehat;
- login Instructor dan Player berhasil;
- `https://narafin.org/health`, `/privacy`, dan `/terms` mengembalikan 200;
- `/metrics` mengembalikan 404 dari publik.

## 10. Hal yang sengaja tidak dipilih

Keputusan berikut tidak ditambahkan karena tidak diperlukan untuk tujuan proyek:

- backend simulator pasar/deck;
- `LewatiOrder` atau aksi lewati buatan;
- metrik pelanggaran berdasarkan event yang ditolak;
- fitur profil, ganti password, logout semua perangkat, dan admin akun;
- `security_version`;
- token demo dengan durasi khusus;
- kontak badan usaha pada halaman legal;
- backup otomatis, restore test, dan rollback database;
- GitHub Actions, staging, atau versi API transisi.

## Status keputusan

Seluruh keputusan di atas berstatus **dipilih dan menjadi acuan final**. Perubahan berikutnya harus menjaga konsistensi antara aturan permainan, kontrak API, schema database, analitik, UI, Postman, dokumentasi, dan deployment.
