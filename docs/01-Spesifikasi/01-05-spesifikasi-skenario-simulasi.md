# Skenario Simulasi Permainan Cashflowpoly
## Sistem Informasi Dasbor Analitika dan Manajemen Ruleset Cashflowpoly

### Dokumen
- Nama dokumen: Spesifikasi Skenario Simulasi Permainan
- Versi: 1.2
- Status: Disesuaikan dengan DDL/DML dan alur event API
- Tanggal: 5 Juni 2026
- Tanggal penyesuaian: 11 Juli 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Identitas Simulasi
Dokumen ini menyatukan skenario permainan untuk Mode Pemula dan Mode Mahir. Tim pengembang dapat memakai skenario ini sebagai acuan input event, validasi alur permainan, pembuatan data simulasi, dan pengujian dashboard analitika. Penyesuaian dokumen ini menjaga urutan hari, urutan pemain, dan pola aksi utama agar alur permainan tetap utuh.

### 1.1 Instruktur
- Nama instruktur: Ibu Rina Kartika, S.Pd.
- Peran sistem: `INSTRUCTOR`
- Username contoh: `rina.kartika`

### 1.2 Sesi Mode Pemula
- Nama sesi: Simulasi Cashflowpoly Kelas XI IPS 2 - Mode Pemula - Kelompok A
- Mode: `PEMULA`
- Instruktur: Ibu Rina Kartika, S.Pd.
- Jumlah pemain: 4 pemain
- Pemain: Marco, Marcello, Hugo, Manalu
- Tujuan sesi: latihan dasar pencatatan event, pembelian bahan, klaim pesanan, kebutuhan, donasi, investasi emas, dan perhitungan poin akhir.

### 1.3 Sesi Mode Mahir
- Nama sesi: Simulasi Cashflowpoly Kelas XI IPS 2 - Mode Mahir - Kelompok B
- Mode: `MAHIR`
- Instruktur: Ibu Rina Kartika, S.Pd.
- Jumlah pemain: 4 pemain
- Pemain: Marco, Marcello, Hugo, Manalu
- Tujuan sesi: pengujian lanjutan untuk event risiko kehidupan, asuransi multi risiko, pinjaman syariah, tujuan keuangan, donasi, investasi emas, dan perhitungan poin akhir mode mahir.

### 1.4 Catatan Penggunaan
1. Nama instruktur dan nama sesi pada dokumen ini adalah data contoh realistis untuk dokumentasi, seed simulasi, dan pengujian.
2. Setiap baris aksi pemain menghasilkan satu event dengan `actor_type=PLAYER`, kecuali bagian yang secara eksplisit menyebut sistem.
3. Setiap perpindahan hari menghasilkan event sistem dengan `actor_type=SYSTEM` dan `action_type=AkhirGiliran`.
4. Setiap hari Minggu menghasilkan event sistem dengan `actor_type=SYSTEM` dan `action_type=HariMingguLibur`.
5. Seed simulasi mencatat event setup pada `day_index=0` (kotak GO) sebelum aksi utama: `MulaiSesi`, `BagikanTieBreaker`, `SetupBahanAwal`, `SetupEmasAwal`, dan `SetupMisiAwal`. Backend tidak mencatat atau mengisi deck/pasar fisik. Event setup tidak muncul sebagai aksi tanggal 1 dan tidak mengurangi jatah aksi harian pemain.
6. Seed memberi satu emas awal per pemain melalui `SetupEmasAwal`; holding tersebut dapat dipakai oleh penjualan reguler maupun opsi darurat.
7. Setup Mode Mahir memberi satu `SetupPinjamanAwal` dan satu `SetupAsuransiAwal` per pemain untuk merepresentasikan kondisi awal skenario uji. Keduanya tidak mengurangi slot aksi.
8. Sistem hanya membuat event `BayarPinjaman` bila pemain memiliki pinjaman aktif. Jika pemain tidak memiliki pinjaman aktif, sistem mencatat aksi alternatif sesuai skenario, misalnya `KerjaLepas` atau catatan audit tanpa transaksi.
9. Risiko `OUT` disimpan pending. Pemain menyelesaikannya dengan `BayarRisiko`, polis aktif, atau `GunakanOpsiDarurat`; saldo cukup tidak otomatis memilih pembayaran tunai.
10. Setiap kartu pinjaman memiliki instance sendiri. Beberapa pinjaman produk yang sama dapat aktif bersamaan selama stok fisik katalog tersedia; pelunasan selalu menutup seluruh outstanding satu instance.

### 1.5 Pemetaan Istilah Papan ke Kode Katalog
Bagian ini menjaga bahasa skenario tetap mudah dibaca, tetapi tetap cocok dengan katalog DDL/DML.

| Istilah skenario | Kode katalog | Nama katalog |
|---|---|---|
| Beras | `nasi_putih` | Nasi Putih |
| Sayur | `sayur` | Sayur |
| Bumbu | `tahu_tempe` | Tahu Tempe |
| Telur | `telur` | Telur |
| Daging | `daging` | Daging |
| Pesanan Beras + Telur | `nasi_goreng` | nasi goreng |
| Pesanan Daging + Bumbu | `tahu_campur` | tahu campur |
| Pesanan Sayur + Bumbu | `resep-sayur-bumbu` | sayur tahu tempe |
| Kebutuhan Primer | `buku` | buku |
| Kebutuhan Sekunder | `sepatu` | sepatu |
| Kebutuhan Tersier Misi Koleksi | `jam`, `boneka`, `gameboy`, `hiburan` | sesuai kartu misi pemain |

### 1.6 Konvensi Event Harian
1. Hari Senin sampai Kamis mewajibkan tepat dua aksi pemain sesuai `actions_per_turn=2`; aksi yang sama boleh diulang.
2. Hari Jumat memakai aksi donasi, pengumuman juara donasi, dan event poin peringkat donasi.
3. Hari Sabtu memakai aksi harga emas, beli emas, jual emas, atau lewati transaksi emas.
4. Hari Minggu tidak memakai aksi pemain. Sistem hanya mencatat libur dan melanjutkan hari.
5. Mode Pemula tidak memakai `PinjamanSyariah`, `BayarPinjaman`, `Asuransi`, `RisikoKehidupan`, `GunakanOpsiDarurat`, `Menabung`, `TarikTabungan`, dan `TujuanFinansial`.
6. Mode Mahir memakai fitur lanjutan sesuai skenario, yaitu risiko, asuransi, pinjaman, tabungan tujuan keuangan, dan tujuan finansial.


---

## 2. Mode Pemula

### Mode Pemula - Hari 1 - Senin
- Marco Hari 1 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marco Hari 1 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Marcello Hari 1 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marcello Hari 1 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Hugo Hari 1 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Hugo Hari 1 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Manalu Hari 1 Aksi 1 -> Kerja Lepas: terima 1 koin
- Manalu Hari 1 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Sayur
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 2

### Mode Pemula - Hari 2 - Selasa
- Marco Hari 2 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur
- Marco Hari 2 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Marcello Hari 2 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur
- Marcello Hari 2 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Hugo Hari 2 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu
- Hugo Hari 2 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Manalu Hari 2 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Manalu Hari 2 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 3

### Mode Pemula - Hari 3 - Rabu
- Marco Hari 3 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Marco Hari 3 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Marcello Hari 3 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Sayur
- Marcello Hari 3 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 3 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Hugo Hari 3 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Manalu Hari 3 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur
- Manalu Hari 3 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 4

### Mode Pemula - Hari 4 - Kamis
- Marco Hari 4 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu
- Marco Hari 4 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Sekunder: sepatu (`sepatu`)
- Marcello Hari 4 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: sayur tahu tempe (`resep-sayur-bumbu`) memakai Sayur + Bumbu
- Marcello Hari 4 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Sekunder: sepatu (`sepatu`)
- Hugo Hari 4 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur
- Hugo Hari 4 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Sekunder: sepatu (`sepatu`)
- Manalu Hari 4 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Manalu Hari 4 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Sekunder: sepatu (`sepatu`)
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 5

### Mode Pemula - Hari 5 - Jumat
- Marco Hari 5 -> Peduli Donasi: donasi 2 koin
- Marcello Hari 5 -> Peduli Donasi: donasi 3 koin
- Hugo Hari 5 -> Peduli Donasi: donasi 1 koin
- Manalu Hari 5 -> Peduli Donasi: donasi 4 koin
- Sistem menentukan Juara Donasi: Manalu Juara 1, Marcello Juara 2, Marco Juara 3
- Setelah Donasi selesai -> Mr. Cashflowpoly maju ke Hari 6

### Mode Pemula - Hari 6 - Sabtu
- Sistem membuka 1 Kartu Harga Emas
- Marco Hari 6 -> Investasi Emas: beli 1 Kartu Emas
- Marcello Hari 6 -> Lewati Transaksi Emas (`LewatiTransaksiEmas`) untuk menjaga saldo setelah pembelian bahan Hari 1
- Hugo Hari 6 -> Lewati Transaksi Emas (`LewatiTransaksiEmas`): tidak membeli dan tidak menjual
- Manalu Hari 6 -> Lewati Transaksi Emas (`LewatiTransaksiEmas`) karena saldo tidak cukup
- Setelah Investasi Emas selesai -> Mr. Cashflowpoly maju ke Hari 7

### Mode Pemula - Hari 7 - Minggu
- Hari 7 -> Libur
- Mr. Cashflowpoly langsung lanjut ke Hari 8

### Mode Pemula - Hari 8 - Senin
- Marco Hari 8 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marco Hari 8 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Marcello Hari 8 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Marcello Hari 8 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 8 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Sayur
- Hugo Hari 8 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Manalu Hari 8 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu
- Manalu Hari 8 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Beras
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 9

### Mode Pemula - Hari 9 - Selasa
- Marco Hari 9 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur
- Marco Hari 9 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Tersier sesuai Misi Koleksi (`jam`/`boneka`/`gameboy`/`hiburan`)
- Marcello Hari 9 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu
- Marcello Hari 9 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Tersier sesuai Misi Koleksi (`jam`/`boneka`/`gameboy`/`hiburan`)
- Hugo Hari 9 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: sayur tahu tempe (`resep-sayur-bumbu`) memakai Sayur + Bumbu
- Hugo Hari 9 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Tersier sesuai Misi Koleksi (`jam`/`boneka`/`gameboy`/`hiburan`)
- Manalu Hari 9 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Manalu Hari 9 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Tersier sesuai Misi Koleksi (`jam`/`boneka`/`gameboy`/`hiburan`)
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 10

### Mode Pemula - Hari 10 - Rabu
- Marco Hari 10 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Marco Hari 10 Aksi 2 -> Kerja Lepas: terima 1 koin
- Marcello Hari 10 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marcello Hari 10 Aksi 2 -> Kerja Lepas: terima 1 koin
- Hugo Hari 10 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Hugo Hari 10 Aksi 2 -> Kerja Lepas: terima 1 koin
- Manalu Hari 10 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur
- Manalu Hari 10 Aksi 2 -> Kerja Lepas: terima 1 koin
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 11

### Mode Pemula - Hari 11 - Kamis
- Marco Hari 11 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Marco Hari 11 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu
- Marcello Hari 11 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Marcello Hari 11 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur
- Hugo Hari 11 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Hugo Hari 11 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur
- Manalu Hari 11 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Sayur
- Manalu Hari 11 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 12

### Mode Pemula - Hari 12 - Jumat
- Marco Hari 12 -> Peduli Donasi: donasi 5 koin
- Marcello Hari 12 -> Peduli Donasi: donasi 2 koin
- Hugo Hari 12 -> Peduli Donasi: donasi 4 koin
- Manalu Hari 12 -> Peduli Donasi: donasi 3 koin
- Sistem menentukan Juara Donasi: Marco Juara 1, Hugo Juara 2, Manalu Juara 3
- Setelah Donasi selesai -> Mr. Cashflowpoly maju ke Hari 13

### Mode Pemula - Hari 13 - Sabtu
- Sistem membuka 1 Kartu Harga Emas
- Marco Hari 13 -> Investasi Emas: beli 1 Kartu Emas
- Marcello Hari 13 -> Investasi Emas: jual 1 Kartu Emas
- Hugo Hari 13 -> Investasi Emas: beli 1 Kartu Emas
- Manalu Hari 13 -> Investasi Emas: beli 1 Kartu Emas
- Setelah Investasi Emas selesai -> Mr. Cashflowpoly maju ke Hari 14

### Mode Pemula - Hari 14 - Minggu
- Hari 14 -> Libur
- Mr. Cashflowpoly langsung lanjut ke Hari 15

### Mode Pemula - Hari 15 - Senin
- Marco Hari 15 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Marco Hari 15 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Beras
- Marcello Hari 15 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Marcello Hari 15 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Daging
- Hugo Hari 15 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Hugo Hari 15 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Manalu Hari 15 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: sayur tahu tempe (`resep-sayur-bumbu`) memakai Sayur + Bumbu
- Manalu Hari 15 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 16

### Mode Pemula - Hari 16 - Selasa
- Marco Hari 16 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Marco Hari 16 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur
- Marcello Hari 16 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Marcello Hari 16 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu
- Hugo Hari 16 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Hugo Hari 16 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu
- Manalu Hari 16 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Manalu Hari 16 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 17

### Mode Pemula - Hari 17 - Rabu
- Marco Hari 17 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Sekunder: sepatu (`sepatu`)
- Marco Hari 17 Aksi 2 -> Kerja Lepas: terima 1 koin
- Marcello Hari 17 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Sekunder: sepatu (`sepatu`)
- Marcello Hari 17 Aksi 2 -> Kerja Lepas: terima 1 koin
- Hugo Hari 17 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Sekunder: sepatu (`sepatu`)
- Hugo Hari 17 Aksi 2 -> Kerja Lepas: terima 1 koin
- Manalu Hari 17 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur
- Manalu Hari 17 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Sekunder: sepatu (`sepatu`)
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 18

### Mode Pemula - Hari 18 - Kamis
- Marco Hari 18 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Sayur
- Marco Hari 18 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Marcello Hari 18 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marcello Hari 18 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Hugo Hari 18 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Hugo Hari 18 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Manalu Hari 18 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Manalu Hari 18 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 19

### Mode Pemula - Hari 19 - Jumat
- Marco Hari 19 -> Peduli Donasi: donasi 3 koin
- Marcello Hari 19 -> Peduli Donasi: donasi 5 koin
- Hugo Hari 19 -> Peduli Donasi: donasi 2 koin
- Manalu Hari 19 -> Peduli Donasi: donasi 4 koin
- Sistem menentukan Juara Donasi: Marcello Juara 1, Manalu Juara 2, Marco Juara 3
- Setelah Donasi selesai -> Mr. Cashflowpoly maju ke Hari 20

### Mode Pemula - Hari 20 - Sabtu
- Sistem membuka 1 Kartu Harga Emas
- Marco Hari 20 -> Investasi Emas: jual 1 Kartu Emas
- Marcello Hari 20 -> Investasi Emas: beli 1 Kartu Emas
- Hugo Hari 20 -> Investasi Emas: beli 1 Kartu Emas
- Manalu Hari 20 -> Lewati Transaksi Emas (`LewatiTransaksiEmas`): tidak membeli dan tidak menjual
- Setelah Investasi Emas selesai -> Mr. Cashflowpoly maju ke Hari 21

### Mode Pemula - Hari 21 - Minggu
- Hari 21 -> Libur
- Mr. Cashflowpoly langsung lanjut ke Hari 22

### Mode Pemula - Hari 22 - Senin
- Marco Hari 22 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: sayur tahu tempe (`resep-sayur-bumbu`) memakai Sayur + Bumbu
- Marco Hari 22 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Tersier sesuai Misi Koleksi (`jam`/`boneka`/`gameboy`/`hiburan`)
- Marcello Hari 22 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur
- Marcello Hari 22 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Tersier sesuai Misi Koleksi (`jam`/`boneka`/`gameboy`/`hiburan`)
- Hugo Hari 22 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur
- Hugo Hari 22 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Tersier sesuai Misi Koleksi (`jam`/`boneka`/`gameboy`/`hiburan`)
- Manalu Hari 22 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu
- Manalu Hari 22 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Tersier sesuai Misi Koleksi (`jam`/`boneka`/`gameboy`/`hiburan`)
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 23

### Mode Pemula - Hari 23 - Selasa
- Marco Hari 23 Aksi 1 -> Kerja Lepas: terima 1 koin
- Marco Hari 23 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Beras
- Marcello Hari 23 Aksi 1 -> Kerja Lepas: terima 1 koin
- Marcello Hari 23 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 23 Aksi 1 -> Kerja Lepas: terima 1 koin
- Hugo Hari 23 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Daging
- Manalu Hari 23 Aksi 1 -> Kerja Lepas: terima 1 koin
- Manalu Hari 23 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 24

### Mode Pemula - Hari 24 - Rabu
- Marco Hari 24 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Marco Hari 24 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur
- Marcello Hari 24 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Marcello Hari 24 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu
- Hugo Hari 24 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 24 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu
- Manalu Hari 24 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Manalu Hari 24 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 25

### Mode Pemula - Hari 25 - Kamis
- Marco Hari 25 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Marco Hari 25 Aksi 2 -> Kerja Lepas: terima 1 koin
- Marcello Hari 25 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Marcello Hari 25 Aksi 2 -> Kerja Lepas: terima 1 koin
- Hugo Hari 25 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Hugo Hari 25 Aksi 2 -> Kerja Lepas: terima 1 koin
- Manalu Hari 25 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Manalu Hari 25 Aksi 2 -> Kerja Lepas: terima 1 koin
- Setelah semua pemain selesai -> Mr. Cashflowpoly menuju Finish

### Akhir Permainan Mode Pemula
- Sistem menghitung Poin Aneka Kebutuhan
- Sistem menghitung Bonus Set Aneka Kebutuhan
- Sistem menghitung Poin Juara Donasi
- Sistem menghitung Poin Investasi Emas
- Sistem menghitung Poin Dana Pensiun
- Sistem mengecek Misi Koleksi setiap pemain
- Jika pemain gagal memenuhi Misi Koleksi -> sistem mengurangi Poin Kebahagiaan pemain
- Sistem menentukan pemain dengan total Poin Kebahagiaan tertinggi sebagai pemenang
- Jika poin seri -> pemain dengan angka Kartu Tie Breaker terbesar menang

---

## 3. Mode Mahir

### Mode Mahir - Hari 1 - Senin
- Marco Hari 1 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marco Hari 1 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Marcello Hari 1 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Marcello Hari 1 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 1 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Hugo Hari 1 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Manalu Hari 1 Aksi 1 -> Kerja Lepas: terima 1 koin
- Manalu Hari 1 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 2

### Mode Mahir - Hari 2 - Selasa
- Marco Hari 2 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika sudah tersedia; jika belum tersedia, bayar biaya risiko dengan koin
- Marco Hari 2 Aksi 2 -> Aktifkan 1 Asuransi Multi Risiko; bayar 1 koin ke bank
- Marcello Hari 2 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu + ambil 1 Kartu Risiko Kehidupan; bayar biaya risiko dengan koin jika muncul risiko negatif; catat risiko sebagai `RisikoKehidupan`
- Marcello Hari 2 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Hugo Hari 2 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur + ambil 1 Kartu Risiko Kehidupan; terima manfaat jika risiko positif muncul; catat risiko sebagai `RisikoKehidupan`
- Hugo Hari 2 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Manalu Hari 2 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: sayur tahu tempe (`resep-sayur-bumbu`) memakai Sayur + Bumbu + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika sudah tersedia; jika belum tersedia, bayar biaya risiko dengan koin
- Manalu Hari 2 Aksi 2 -> Aktifkan 1 Asuransi Multi Risiko; bayar 1 koin ke bank
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 3

### Mode Mahir - Hari 3 - Rabu
- Marco Hari 3 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Marco Hari 3 Aksi 2 -> Menabung untuk Tujuan Keuangan: 5 koin
- Marcello Hari 3 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marcello Hari 3 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Hugo Hari 3 Aksi 1 -> Menabung untuk Tujuan Keuangan: 5 koin
- Hugo Hari 3 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Daging
- Manalu Hari 3 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Manalu Hari 3 Aksi 2 -> Menabung untuk Tujuan Keuangan: 5 koin
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 4

### Mode Mahir - Hari 4 - Kamis
- Marco Hari 4 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Marco Hari 4 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Marcello Hari 4 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur + ambil 1 Kartu Risiko Kehidupan; bayar biaya risiko dengan koin jika muncul risiko negatif; catat risiko sebagai `RisikoKehidupan`
- Marcello Hari 4 Aksi 2 -> Kerja Lepas: terima 1 koin
- Hugo Hari 4 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 4 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika sudah tersedia; jika belum tersedia, bayar biaya risiko dengan koin
- Manalu Hari 4 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Manalu Hari 4 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 5

### Mode Mahir - Hari 5 - Jumat
- Marco Hari 5 -> Peduli Donasi: donasi 1 koin
- Marcello Hari 5 -> Peduli Donasi: donasi 3 koin
- Hugo Hari 5 -> Peduli Donasi: donasi 2 koin
- Manalu Hari 5 -> Peduli Donasi: donasi 4 koin
- Sistem menentukan Juara Donasi: Manalu Juara 1, Marcello Juara 2, Hugo Juara 3
- Setelah Donasi selesai -> Mr. Cashflowpoly maju ke Hari 6

### Mode Mahir - Hari 6 - Sabtu
- Sistem membuka 1 Kartu Harga Emas dengan harga 5 koin
- Marco Hari 6 -> Lewati Transaksi Emas (`LewatiTransaksiEmas`) karena saldo tidak cukup
- Marcello Hari 6 -> Lewati Transaksi Emas (`LewatiTransaksiEmas`): tidak membeli dan tidak menjual
- Hugo Hari 6 -> Jual 1 Kartu Emas awal seharga 5 koin untuk menyiapkan pelunasan pinjaman setup
- Manalu Hari 6 -> Investasi Emas: beli 1 Kartu Emas seharga 5 koin
- Setelah Investasi Emas selesai -> Mr. Cashflowpoly maju ke Hari 7

### Mode Mahir - Hari 7 - Minggu
- Hari 7 -> Libur
- Mr. Cashflowpoly langsung lanjut ke Hari 8

### Mode Mahir - Hari 8 - Senin
- Marco Hari 8 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu + ambil 1 Kartu Risiko Kehidupan; bayar biaya risiko dengan koin jika muncul risiko negatif; catat risiko sebagai `RisikoKehidupan`
- Marco Hari 8 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Sekunder: sepatu (`sepatu`)
- Marcello Hari 8 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Sekunder: sepatu (`sepatu`)
- Marcello Hari 8 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Sayur
- Hugo Hari 8 Aksi 1 -> Aktifkan 1 Asuransi Multi Risiko; bayar 1 koin ke bank
- Hugo Hari 8 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Sekunder: sepatu (`sepatu`)
- Manalu Hari 8 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika sudah tersedia; jika belum tersedia, bayar biaya risiko dengan koin
- Manalu Hari 8 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Sekunder: sepatu (`sepatu`)
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 9

### Mode Mahir - Hari 9 - Selasa
- Marco Hari 9 Aksi 1 -> Menabung untuk Tujuan Keuangan: 10 koin
- Marco Hari 9 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Beras
- Marcello Hari 9 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: sayur tahu tempe (`resep-sayur-bumbu`) memakai Sayur + Bumbu + ambil 1 Kartu Risiko Kehidupan
- Marcello Hari 9 Aksi 2 -> Aktifkan 1 Asuransi Multi Risiko; bayar 1 koin ke bank
- Hugo Hari 9 Aksi 1 -> Lunasi pinjaman setup melalui `BayarPinjaman`; setelah itu gunakan `GunakanOpsiDarurat` bertipe `TAKE_SHARIA_LOAN` untuk menyelesaikan risiko pending dengan produk yang sama
- Hugo Hari 9 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Beras
- Manalu Hari 9 Aksi 1 -> Menabung untuk Tujuan Keuangan: 1 koin; selesaikan risiko pending dengan `BayarRisiko` sebagai aksi bebas
- Manalu Hari 9 Aksi 2 -> Kerja Lepas: terima 1 koin
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 10

### Mode Mahir - Hari 10 - Rabu
- Marco Hari 10 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Marco Hari 10 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika sudah tersedia; jika belum tersedia, bayar biaya risiko dengan koin
- Marcello Hari 10 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marcello Hari 10 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Hugo Hari 10 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Hugo Hari 10 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur + ambil 1 Kartu Risiko Kehidupan; bayar biaya risiko dengan koin jika muncul risiko negatif; catat risiko sebagai `RisikoKehidupan`
- Manalu Hari 10 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Manalu Hari 10 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika sudah tersedia; jika belum tersedia, bayar biaya risiko dengan koin
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 11

### Mode Mahir - Hari 11 - Kamis
- Marco Hari 11 Aksi 1 -> Cek Pinjaman Syariah; jika memiliki pinjaman aktif, bayar pinjaman dan serahkan 10 koin ke bank; jika tidak memiliki pinjaman aktif, lakukan Kerja Lepas: terima 1 koin
- Marco Hari 11 Aksi 2 -> Aktifkan 1 Asuransi Multi Risiko; bayar 1 koin ke bank
- Marcello Hari 11 Aksi 1 -> Cek Pinjaman Syariah; jika memiliki pinjaman aktif, bayar pinjaman dan serahkan 10 koin ke bank; jika tidak memiliki pinjaman aktif, lakukan Kerja Lepas: terima 1 koin
- Marcello Hari 11 Aksi 2 -> Menabung untuk Tujuan Keuangan: 5 koin
- Hugo Hari 11 Aksi 1 -> Cek Pinjaman Syariah; jika memiliki pinjaman aktif, bayar pinjaman dan serahkan 10 koin ke bank; jika tidak memiliki pinjaman aktif, lakukan Kerja Lepas: terima 1 koin
- Hugo Hari 11 Aksi 2 -> Aktifkan 1 Asuransi Multi Risiko; bayar 1 koin ke bank
- Manalu Hari 11 Aksi 1 -> Cek Pinjaman Syariah; jika memiliki pinjaman aktif, bayar pinjaman dan serahkan 10 koin ke bank; jika tidak memiliki pinjaman aktif, lakukan Kerja Lepas: terima 1 koin
- Manalu Hari 11 Aksi 2 -> Aktifkan 1 Asuransi Multi Risiko; bayar 1 koin ke bank
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 12

### Mode Mahir - Hari 12 - Jumat
- Marco Hari 12 -> Peduli Donasi: donasi 4 koin
- Marcello Hari 12 -> Peduli Donasi: donasi 2 koin
- Hugo Hari 12 -> Peduli Donasi: donasi 5 koin
- Manalu Hari 12 -> Peduli Donasi: donasi 3 koin
- Sistem menentukan Juara Donasi: Hugo Juara 1, Marco Juara 2, Manalu Juara 3
- Setelah Donasi selesai -> Mr. Cashflowpoly maju ke Hari 13

### Mode Mahir - Hari 13 - Sabtu
- Sistem membuka 1 Kartu Harga Emas
- Marco Hari 13 -> Investasi Emas: jual 1 Kartu Emas
- Marcello Hari 13 -> Investasi Emas: beli 1 Kartu Emas
- Hugo Hari 13 -> Investasi Emas: beli 1 Kartu Emas
- Manalu Hari 13 -> Investasi Emas: jual 1 Kartu Emas
- Setelah Investasi Emas selesai -> Mr. Cashflowpoly maju ke Hari 14

### Mode Mahir - Hari 14 - Minggu
- Hari 14 -> Libur
- Mr. Cashflowpoly langsung lanjut ke Hari 15

### Mode Mahir - Hari 15 - Senin
- Marco Hari 15 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Tersier sesuai Misi Koleksi (`jam`/`boneka`/`gameboy`/`hiburan`)
- Marco Hari 15 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Sayur
- Marcello Hari 15 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Tersier sesuai Misi Koleksi (`jam`/`boneka`/`gameboy`/`hiburan`)
- Marcello Hari 15 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 15 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Tersier sesuai Misi Koleksi (`jam`/`boneka`/`gameboy`/`hiburan`)
- Hugo Hari 15 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Daging
- Manalu Hari 15 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Tersier sesuai Misi Koleksi (`jam`/`boneka`/`gameboy`/`hiburan`)
- Manalu Hari 15 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Beras
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 16

### Mode Mahir - Hari 16 - Selasa
- Marco Hari 16 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Marco Hari 16 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: sayur tahu tempe (`resep-sayur-bumbu`) memakai Sayur + Bumbu + ambil 1 Kartu Risiko Kehidupan; bayar biaya risiko dengan koin jika muncul risiko negatif; catat risiko sebagai `RisikoKehidupan`
- Marcello Hari 16 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Marcello Hari 16 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika sudah tersedia; jika belum tersedia, bayar biaya risiko dengan koin
- Hugo Hari 16 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 16 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu + ambil 1 Kartu Risiko Kehidupan; bayar biaya risiko dengan koin jika muncul risiko negatif; catat risiko sebagai `RisikoKehidupan`
- Manalu Hari 16 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Manalu Hari 16 Aksi 2 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika sudah tersedia; jika belum tersedia, bayar biaya risiko dengan koin
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 17

### Mode Mahir - Hari 17 - Rabu
- Marco Hari 17 Aksi 1 -> Menabung untuk Tujuan Keuangan: 15 koin
- Sistem mencatat pencapaian `TujuanFinansial` jika saldo tabungan mencukupi
- Marco Hari 17 Aksi 2 -> Menabung untuk Tujuan Keuangan: 8 koin
- Marcello Hari 17 Aksi 1 -> Menabung untuk Tujuan Keuangan: 10 koin
- Marcello Hari 17 Aksi 2 -> Jika tabungan cukup, ambil 1 Kartu Tujuan Keuangan (`TujuanFinansial`)
- Hugo Hari 17 Aksi 1 -> Menabung untuk Tujuan Keuangan: 10 koin
- Hugo Hari 17 Aksi 2 -> Jika tabungan cukup, ambil 1 Kartu Tujuan Keuangan (`TujuanFinansial`)
- Manalu Hari 17 Aksi 1 -> Menabung untuk Tujuan Keuangan: 10 koin
- Manalu Hari 17 Aksi 2 -> Jika tabungan cukup, ambil 1 Kartu Tujuan Keuangan (`TujuanFinansial`)
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 18

### Mode Mahir - Hari 18 - Kamis
- Marco Hari 18 Aksi 1 -> Gunakan polis setup aktif untuk menyelesaikan risiko pribadi pending; polis kembali `INACTIVE`
- Marco Hari 18 Aksi 2 -> Kerja Lepas: terima 1 koin
- Marcello Hari 18 Aksi 1 -> Gunakan polis aktif untuk menyelesaikan risiko pribadi pending; polis kembali `INACTIVE`
- Marcello Hari 18 Aksi 2 -> Kerja Lepas: terima 1 koin
- Hugo Hari 18 Aksi 1 -> Gunakan polis setup aktif untuk menyelesaikan risiko pribadi pending; polis kembali `INACTIVE`
- Hugo Hari 18 Aksi 2 -> Kerja Lepas: terima 1 koin
- Manalu Hari 18 Aksi 1 -> Gunakan polis setup aktif untuk menyelesaikan risiko pribadi pending; polis kembali `INACTIVE`
- Manalu Hari 18 Aksi 2 -> Kerja Lepas: terima 1 koin
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 19

### Mode Mahir - Hari 19 - Jumat
- Marco Hari 19 -> Peduli Donasi: donasi 2 koin
- Marcello Hari 19 -> Peduli Donasi: donasi 4 koin
- Hugo Hari 19 -> Peduli Donasi: donasi 3 koin
- Manalu Hari 19 -> Peduli Donasi: donasi 5 koin
- Sistem menentukan Juara Donasi: Manalu Juara 1, Marcello Juara 2, Hugo Juara 3
- Setelah Donasi selesai -> Mr. Cashflowpoly maju ke Hari 20

### Mode Mahir - Hari 20 - Sabtu
- Sistem membuka 1 Kartu Harga Emas
- Marco Hari 20 -> Investasi Emas: beli 1 Kartu Emas
- Marcello Hari 20 -> Investasi Emas: jual 1 Kartu Emas
- Hugo Hari 20 -> Investasi Emas: beli 1 Kartu Emas
- Manalu Hari 20 -> Lewati Transaksi Emas (`LewatiTransaksiEmas`): tidak membeli dan tidak menjual
- Setelah Investasi Emas selesai -> Mr. Cashflowpoly maju ke Hari 21

### Mode Mahir - Hari 21 - Minggu
- Hari 21 -> Libur
- Mr. Cashflowpoly langsung lanjut ke Hari 22

### Mode Mahir - Hari 22 - Senin
- Marco Hari 22 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marco Hari 22 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Marcello Hari 22 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Sayur
- Marcello Hari 22 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 22 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Hugo Hari 22 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Manalu Hari 22 Aksi 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Manalu Hari 22 Aksi 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 23

### Mode Mahir - Hari 23 - Selasa
- Marco Hari 23 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika sudah tersedia; jika belum tersedia, bayar biaya risiko dengan koin
- Marco Hari 23 Aksi 2 -> Kerja Lepas: terima 1 koin
- Marcello Hari 23 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: sayur tahu tempe (`resep-sayur-bumbu`) memakai Sayur + Bumbu + ambil 1 Kartu Risiko Kehidupan; bayar biaya risiko dengan koin jika muncul risiko negatif; catat risiko sebagai `RisikoKehidupan`
- Marcello Hari 23 Aksi 2 -> Kerja Lepas: terima 1 koin
- Hugo Hari 23 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: nasi goreng (`nasi_goreng`) memakai Beras + Telur + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika sudah tersedia; jika belum tersedia, bayar biaya risiko dengan koin
- Hugo Hari 23 Aksi 2 -> Kerja Lepas: terima 1 koin
- Manalu Hari 23 Aksi 1 -> Jual 1 Kartu Pesanan Masakan: tahu campur (`tahu_campur`) memakai Daging + Bumbu + ambil 1 Kartu Risiko Kehidupan; bayar biaya risiko dengan koin jika muncul risiko negatif; catat risiko sebagai `RisikoKehidupan`
- Manalu Hari 23 Aksi 2 -> Kerja Lepas: terima 1 koin
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 24

### Mode Mahir - Hari 24 - Rabu
- Marco Hari 24 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Marco Hari 24 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Sekunder: sepatu (`sepatu`)
- Marcello Hari 24 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Marcello Hari 24 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Sekunder: sepatu (`sepatu`)
- Hugo Hari 24 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Hugo Hari 24 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Sekunder: sepatu (`sepatu`)
- Manalu Hari 24 Aksi 1 -> Beli 1 Kartu Aneka Kebutuhan Primer: buku (`buku`)
- Manalu Hari 24 Aksi 2 -> Beli 1 Kartu Aneka Kebutuhan Sekunder: sepatu (`sepatu`)
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 25

### Mode Mahir - Hari 25 - Kamis
- Marco Hari 25 Aksi 1 -> Cek Pinjaman Syariah; jika masih ada pinjaman, bayar pinjaman aktif; jika tidak ada pinjaman, lanjutkan aksi berikutnya tanpa event pembayaran
- Marco Hari 25 Aksi 2 -> Kerja Lepas: terima 1 koin
- Marcello Hari 25 Aksi 1 -> Cek Pinjaman Syariah; jika masih ada pinjaman, bayar pinjaman aktif; jika tidak ada pinjaman, lanjutkan aksi berikutnya tanpa event pembayaran
- Marcello Hari 25 Aksi 2 -> Kerja Lepas: terima 1 koin
- Hugo Hari 25 Aksi 1 -> Cek Pinjaman Syariah; jika masih ada pinjaman, bayar pinjaman aktif; jika tidak ada pinjaman, lanjutkan aksi berikutnya tanpa event pembayaran
- Hugo Hari 25 Aksi 2 -> Kerja Lepas: terima 1 koin
- Manalu Hari 25 Aksi 1 -> Cek Pinjaman Syariah; jika masih ada pinjaman, bayar pinjaman aktif; jika tidak ada pinjaman, lanjutkan aksi berikutnya tanpa event pembayaran
- Manalu Hari 25 Aksi 2 -> Kerja Lepas: terima 1 koin
- Setelah semua pemain selesai -> Mr. Cashflowpoly menuju Finish

### Akhir Permainan Mode Mahir
- Sistem menghitung Poin Aneka Kebutuhan
- Sistem menghitung Bonus Set Aneka Kebutuhan
- Sistem menghitung Poin Juara Donasi
- Sistem menghitung Poin Investasi Emas
- Sistem menghitung Poin Dana Pensiun
- Sistem menghitung Poin Tujuan Keuangan
- Sistem mengecek Misi Koleksi setiap pemain
- Jika pemain gagal memenuhi Misi Koleksi -> sistem mengurangi Poin Kebahagiaan pemain
- Sistem mengecek Pinjaman Syariah setiap pemain
- Jika pemain masih memiliki Pinjaman Syariah -> sistem mengurangi Poin Kebahagiaan pemain
- Jika pemain masih memiliki Pinjaman Syariah -> sistem menghapus Poin Tujuan Keuangan pemain
- Sistem menentukan pemain dengan total Poin Kebahagiaan tertinggi sebagai pemenang
- Jika poin seri -> pemain dengan angka Kartu Tie Breaker terbesar menang

---

## 4. Ringkasan Pemetaan ke Event API
Bagian ini membantu pengembang mengubah skenario naratif menjadi payload event tanpa mengubah alur hari dan urutan pemain.

| Jenis aksi | `action_type` | Actor type | Payload minimum yang disarankan |
|---|---|---|---|
| Mulai sesi | `MulaiSesi` | `SYSTEM` | `mode`, `ruleset_version_id`, daftar pemain, urutan pemain. |
| Beli bahan masakan | `BahanMasakan` | `PLAYER` | `ingredient_code`, `display_name`, `qty`, `coin_delta`. |
| Buang bahan masakan | `BuangBahanMasakan` | `PLAYER` | `ingredient_code`, `qty`, alasan bila ada. |
| Jual pesanan masakan | `JualMasakan` | `PLAYER` | `order_code`, `required_ingredients`, `sell_price`, `happiness_delta`. |
| Beli kebutuhan | `Kebutuhan` | `PLAYER` | `need_code`, `need_tier`, `purchase_price`, `happiness_delta`. |
| Kerja lepas | `KerjaLepas` | `PLAYER` | `coin_delta=+1`. |
| Catat transaksi | `CatatTransaksi` | `PLAYER` | `transaction_type`, `amount`, `note`. |
| Menabung | `Menabung` | `PLAYER` | `saving_delta`, `coin_delta`, `goal_code` bila ada. |
| Tarik tabungan | `TarikTabungan` | `PLAYER` | `saving_delta`, `coin_delta`, `reason`. |
| Tujuan finansial tercapai | `TujuanFinansial` | `PLAYER` | `goal_code`, `purchase_price`, `happiness_delta`. |
| Donasi Jumat | `JumatBerkah` | `PLAYER` | `donation_amount`, `coin_delta`. |
| Poin peringkat donasi | `PoinPeringkatDonasi` | `SYSTEM` | `rank`, `happiness_delta`. |
| Pengumuman juara donasi | `UmumkanJuaraDonasi` | `SYSTEM` | daftar `user_id` atau `session_player_id`, `rank`, `donation_amount`. |
| Beli emas | `InvestasiEmas` | `PLAYER` | `gold_price_code`, `qty`, `coin_delta`. |
| Jual emas | `JualEmas` | `PLAYER` | `gold_price_code`, `qty`, `coin_delta`. |
| Lewati transaksi emas | `LewatiTransaksiEmas` | `PLAYER` | `reason`. |
| Hari Minggu libur | `HariMingguLibur` | `SYSTEM` | `day_number`, `weekday=SUN`. |
| Pinjaman syariah | `PinjamanSyariah` | `PLAYER` | `loan_code`, `loan_id`, `principal`, `repayment_amount`, `duration_days`, `penalty_points`. |
| Bayar pinjaman | `BayarPinjaman` | `PLAYER` | `loan_id`, `amount` sebesar seluruh outstanding. |
| Asuransi multi risiko | `Asuransi` | `PLAYER` | `product_code`, `premium`, `coverage_status`. |
| Risiko kehidupan | `RisikoKehidupan` | `PLAYER` | `risk_id`, `source_order_event_id`; nilai efek diambil server dari katalog. |
| Bayar risiko | `BayarRisiko` | `PLAYER` | `risk_event_id`. |
| Opsi darurat | `GunakanOpsiDarurat` | `PLAYER` | `risk_event_id`, `option_type`, dan referensi aset sesuai opsi; `amount` dihitung server. |
| Perpindahan hari | `AkhirGiliran` | `SYSTEM` | `from_day`, `to_day`, `completed_players`. |
| Akhir sesi | `AkhiriSesi` | `SYSTEM` | `final_day`, `winner_player_id`, ringkasan skor. |

## 5. Kriteria Validasi Skenario
1. Setiap sesi memiliki instruktur, mode permainan, `ruleset_version_id`, dan empat pemain.
2. Setiap pemain memiliki `player_order_no` dari 1 sampai 4.
3. Event pemain memakai `actor_type=PLAYER`, `session_player_id`, `turn_number`, `action_slot`, `action_type`, dan `payload`.
4. Event sistem memakai `actor_type=SYSTEM` dan tidak mengambil jatah aksi pemain.
5. Hari Senin sampai Kamis memiliki tepat dua aksi per pemain; aksi yang sama boleh diulang.
6. Hari Jumat hanya menjalankan donasi, pengumuman peringkat donasi, dan perpindahan hari.
7. Hari Sabtu hanya menjalankan mekanik harga emas, beli emas, jual emas, lewati transaksi emas, dan perpindahan hari.
8. Hari Minggu hanya menjalankan `HariMingguLibur` dan `AkhirGiliran`.
9. Mode Pemula tidak menjalankan aksi pinjaman, asuransi, risiko kehidupan, tabungan tujuan keuangan, dan tujuan finansial.
10. Mode Mahir menjalankan fitur tambahan untuk risiko, asuransi, pinjaman syariah, tabungan tujuan keuangan, dan tujuan finansial.
11. Sistem hanya mencatat `BayarPinjaman` saat pemain memiliki pinjaman aktif.
12. Sistem hanya memakai perlindungan asuransi jika polis `ACTIVE` dan `remaining_uses > 0`.
13. Sistem menghitung skor akhir setelah Hari 25 selesai.
14. Jika total poin seri, sistem menentukan pemenang memakai angka Kartu Tie Breaker terbesar.
15. Setiap `JualMasakan` Mode Mahir dipasangkan dengan tepat satu `RisikoKehidupan` melalui `source_order_event_id`.

## 6. Catatan Penyesuaian terhadap DDL/DML
1. Dokumen ini mempertahankan semua hari, urutan pemain, pola Jumat-Sabtu-Minggu, dan alur akhir permainan.
2. Penyesuaian utama terjadi pada istilah aksi agar cocok dengan `action_type` yang tersedia.
3. Istilah bahan dan pesanan tetap memakai bahasa papan, tetapi tabel pemetaan menyediakan kode katalog untuk seed dan payload.
4. Mode Pemula tetap bebas dari fitur Mahir.
5. Mode Mahir memakai risiko, asuransi, pinjaman, tabungan, dan tujuan finansial dengan aturan validasi yang lebih eksplisit.
6. Kalimat tentang asuransi dan pinjaman dibuat kondisional agar skenario tetap valid saat status pemain berbeda.

---

## 7. Skenario Operasional Siklus Hidup Sesi (Integrasi API)
Bagian ini menjelaskan skenario teknis daur hidup (*lifecycle*) sesi permainan dari pembuatan hingga pembekuan data melalui Klien Game/IDN atau integrasi API.

### 7.1 Skenario Pembuatan Sesi Baru
- **Tujuan**: Instruktur/Game Client membuat sesi permainan baru untuk menerima rangkaian event.
- **Langkah**:
  1. Instruktur login ke Game Client.
  2. Game Client mengirim request `POST /api/v1/sessions` dengan membawa nama sesi, mode (`PEMULA`/`MAHIR`), dan `ruleset_version_id` yang aktif.
  3. API memvalidasi peran instruktur, keabsahan ruleset, dan mengunci ruleset tersebut untuk sesi ini.
- **Hasil**: API mengembalikan `session_id` baru dengan status `CREATED`.

### 7.2 Skenario Menambahkan Pemain ke Sesi
- **Tujuan**: Mendaftarkan akun Player ke dalam sesi permainan.
- **Langkah**:
  1. Game Client memanggil `POST /api/v1/sessions/{sessionId}/players` dengan membawa payload `user_id` milik akun `PLAYER`.
  2. API memeriksa kapasitas pemain (maksimal 4 pemain) dan memvalidasi apakah peran pengguna adalah `PLAYER`.
- **Hasil**: Akun Player terdaftar sebagai peserta sesi (`session_participants`).

### 7.3 Skenario Memulai Sesi
- **Tujuan**: Mengubah status sesi agar siap menerima event permainan.
- **Langkah**:
  1. IDN membaca `session_player_id` seluruh peserta.
  2. IDN mengirim hasil pembagian fisik melalui endpoint validasi setup.
  3. IDN menyimpan revisi setup; penyimpanan pertama mengunci peserta dan ruleset, sedangkan pembagian masih dapat direvisi sebelum start.
  4. Game Client memanggil `POST /api/v1/sessions/{sessionId}/start`.
  5. API mengunci revisi setup terbaru, membentuk event setup secara atomik, dan mengubah status sesi dari `CREATED` menjadi `STARTED`.
- **Hasil**: Sesi berstatus `STARTED`, pembagian awal dapat diaudit, dan sesi siap menerima event gameplay.

### 7.4 Skenario Ingestion Event Gameplay
- **Tujuan**: Mengirim event transaksi dan aksi permainan secara real-time.
- **Langkah**:
  1. Game Client/Simulator memanggil `POST /api/v1/events` (atau `/events/batch`) setiap kali ada keputusan pemain.
  2. API memverifikasi token pengirim, urutan `sequence_number`, kecocokan `ruleset_version_id`, dan keunikan kombinasi `session_id + event_id` (idempotensi).
  3. Event yang valid disimpan ke PostgreSQL dan memperbarui proyeksi state secara asinkron.
- **Hasil**: Respons status sukses (`200 OK`) dan data state pemain ter-update.

### 7.5 Skenario Mengakhiri Sesi (Membekukan Data)
- **Tujuan**: Menyelesaikan sesi permainan dan mematikan penerimaan event baru.
- **Langkah**:
  1. Game Client memanggil `POST /api/v1/sessions/{sessionId}/end`.
  2. API menghitung skor akhir secara final, menyimpan hasil ke `session_final_scores`, dan mengubah status sesi menjadi `ENDED`.
- **Hasil**: Sesi berstatus `ENDED`. Semua upaya penulisan event baru atau modifikasi ruleset pada sesi ini akan ditolak (`403 Forbidden` / `410 Gone`).

### 7.6 Skenario Pemicuan Hitung Ulang Metrik (Recompute)
- **Tujuan**: Membangun ulang seluruh data proyeksi dan snapshot metrik dari source of truth tabel `events` jika terjadi ketidaksesuaian.
- **Langkah**:
  1. Instruktur/Administrator memanggil `POST /api/v1/analytics/sessions/{sessionId}/recompute`.
  2. API membaca ulang seluruh log event berurutan untuk sesi tersebut dari database, menghitung ulang proyeksi saldo, asuransi, pinjaman, dan metrik, kemudian menulis ulang baris `metric_snapshots` yang bersih.
- **Hasil**: Data analitika sesi tersinkronisasi kembali dengan log event.
