# Skenario Simulasi Permainan Cashflowpoly
## Sistem Informasi Dasbor Analitika dan Manajemen Ruleset Cashflowpoly

### Dokumen
- Nama dokumen: Spesifikasi Skenario Simulasi Permainan
- Versi: 1.0
- Tanggal: 5 Juni 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Identitas Simulasi
Dokumen ini menyatukan skenario permainan untuk Mode Pemula dan Mode Mahir. Skenario ini dapat digunakan sebagai acuan input event, validasi alur permainan, pembuatan data simulasi, dan pengujian dashboard analitika.

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
1. Nama instruktur dan nama sesi pada dokumen ini adalah data contoh realistis untuk kebutuhan dokumentasi, seed simulasi, dan pengujian.
2. Setiap baris aksi dapat dipetakan menjadi event API dengan `session_id`, `event_id`, `sequence_number`, `turn_number`, `actor_type`, `player_id`, `action_type`, dan `payload`.
3. Perpindahan hari oleh Mr. Cashflowpoly dapat direpresentasikan sebagai event sistem dengan `actor_type=SYSTEM`.
4. Aksi pemain dapat direpresentasikan sebagai event pemain dengan `actor_type=PLAYER`.

---

# 2. Mode Pemula

## Hari 1 - Senin
- Marco Hari 1 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marco Hari 1 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Marcello Hari 1 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marcello Hari 1 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Hugo Hari 1 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Hugo Hari 1 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Manalu Hari 1 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Manalu Hari 1 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Sayur
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 2

## Hari 2 - Selasa
- Marco Hari 2 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Beras + Telur
- Marco Hari 2 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Marcello Hari 2 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Beras + Telur
- Marcello Hari 2 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Hugo Hari 2 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Daging + Bumbu
- Hugo Hari 2 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Manalu Hari 2 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Manalu Hari 2 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 3

## Hari 3 - Rabu
- Marco Hari 3 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Marco Hari 3 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Marcello Hari 3 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Sayur
- Marcello Hari 3 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 3 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Hugo Hari 3 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Manalu Hari 3 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Beras + Telur
- Manalu Hari 3 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 4

## Hari 4 - Kamis
- Marco Hari 4 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Daging + Bumbu
- Marco Hari 4 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Sekunder
- Marcello Hari 4 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Sayur + Bumbu
- Marcello Hari 4 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Sekunder
- Hugo Hari 4 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Beras + Telur
- Hugo Hari 4 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Sekunder
- Manalu Hari 4 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Manalu Hari 4 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Sekunder
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 5

## Hari 5 - Jumat
- Marco Hari 5 -> Peduli Donasi: donasi 2 koin
- Marcello Hari 5 -> Peduli Donasi: donasi 3 koin
- Hugo Hari 5 -> Peduli Donasi: donasi 1 koin
- Manalu Hari 5 -> Peduli Donasi: donasi 4 koin
- Sistem menentukan Juara Donasi: Manalu Juara 1, Marcello Juara 2, Marco Juara 3
- Setelah Donasi selesai -> Mr. Cashflowpoly maju ke Hari 6

## Hari 6 - Sabtu
- Sistem membuka 1 Kartu Harga Emas
- Marco Hari 6 -> Investasi Emas: beli 1 Kartu Emas
- Marcello Hari 6 -> Investasi Emas: beli 1 Kartu Emas
- Hugo Hari 6 -> Investasi Emas: tidak membeli dan tidak menjual
- Manalu Hari 6 -> Investasi Emas: beli 1 Kartu Emas
- Setelah Investasi Emas selesai -> Mr. Cashflowpoly maju ke Hari 7

## Hari 7 - Minggu
- Hari 7 -> Libur
- Mr. Cashflowpoly langsung lanjut ke Hari 8

## Hari 8 - Senin
- Marco Hari 8 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marco Hari 8 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Marcello Hari 8 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Marcello Hari 8 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 8 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Sayur
- Hugo Hari 8 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Manalu Hari 8 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Daging + Bumbu
- Manalu Hari 8 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Beras
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 9

## Hari 9 - Selasa
- Marco Hari 9 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Beras + Telur
- Marco Hari 9 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Tersier sesuai Misi Koleksi
- Marcello Hari 9 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Daging + Bumbu
- Marcello Hari 9 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Tersier sesuai Misi Koleksi
- Hugo Hari 9 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Sayur + Bumbu
- Hugo Hari 9 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Tersier sesuai Misi Koleksi
- Manalu Hari 9 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Manalu Hari 9 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Tersier sesuai Misi Koleksi
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 10

## Hari 10 - Rabu
- Marco Hari 10 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Marco Hari 10 Giliran 2 -> Kerja Lepas: terima 1 koin
- Marcello Hari 10 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marcello Hari 10 Giliran 2 -> Kerja Lepas: terima 1 koin
- Hugo Hari 10 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Hugo Hari 10 Giliran 2 -> Kerja Lepas: terima 1 koin
- Manalu Hari 10 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Beras + Telur
- Manalu Hari 10 Giliran 2 -> Kerja Lepas: terima 1 koin
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 11

## Hari 11 - Kamis
- Marco Hari 11 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Marco Hari 11 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Daging + Bumbu
- Marcello Hari 11 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Marcello Hari 11 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Beras + Telur
- Hugo Hari 11 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Hugo Hari 11 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Beras + Telur
- Manalu Hari 11 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Sayur
- Manalu Hari 11 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 12

## Hari 12 - Jumat
- Marco Hari 12 -> Peduli Donasi: donasi 5 koin
- Marcello Hari 12 -> Peduli Donasi: donasi 2 koin
- Hugo Hari 12 -> Peduli Donasi: donasi 4 koin
- Manalu Hari 12 -> Peduli Donasi: donasi 3 koin
- Sistem menentukan Juara Donasi: Marco Juara 1, Hugo Juara 2, Manalu Juara 3
- Setelah Donasi selesai -> Mr. Cashflowpoly maju ke Hari 13

## Hari 13 - Sabtu
- Sistem membuka 1 Kartu Harga Emas
- Marco Hari 13 -> Investasi Emas: beli 1 Kartu Emas
- Marcello Hari 13 -> Investasi Emas: jual 1 Kartu Emas
- Hugo Hari 13 -> Investasi Emas: beli 1 Kartu Emas
- Manalu Hari 13 -> Investasi Emas: beli 1 Kartu Emas
- Setelah Investasi Emas selesai -> Mr. Cashflowpoly maju ke Hari 14

## Hari 14 - Minggu
- Hari 14 -> Libur
- Mr. Cashflowpoly langsung lanjut ke Hari 15

## Hari 15 - Senin
- Marco Hari 15 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Marco Hari 15 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Beras
- Marcello Hari 15 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Marcello Hari 15 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Daging
- Hugo Hari 15 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Hugo Hari 15 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Manalu Hari 15 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Sayur + Bumbu
- Manalu Hari 15 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 16

## Hari 16 - Selasa
- Marco Hari 16 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Marco Hari 16 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Beras + Telur
- Marcello Hari 16 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Marcello Hari 16 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Daging + Bumbu
- Hugo Hari 16 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Hugo Hari 16 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Daging + Bumbu
- Manalu Hari 16 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Manalu Hari 16 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 17

## Hari 17 - Rabu
- Marco Hari 17 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Sekunder
- Marco Hari 17 Giliran 2 -> Kerja Lepas: terima 1 koin
- Marcello Hari 17 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Sekunder
- Marcello Hari 17 Giliran 2 -> Kerja Lepas: terima 1 koin
- Hugo Hari 17 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Sekunder
- Hugo Hari 17 Giliran 2 -> Kerja Lepas: terima 1 koin
- Manalu Hari 17 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Beras + Telur
- Manalu Hari 17 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Sekunder
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 18

## Hari 18 - Kamis
- Marco Hari 18 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Sayur
- Marco Hari 18 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Marcello Hari 18 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marcello Hari 18 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Hugo Hari 18 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Hugo Hari 18 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Manalu Hari 18 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Manalu Hari 18 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 19

## Hari 19 - Jumat
- Marco Hari 19 -> Peduli Donasi: donasi 3 koin
- Marcello Hari 19 -> Peduli Donasi: donasi 5 koin
- Hugo Hari 19 -> Peduli Donasi: donasi 2 koin
- Manalu Hari 19 -> Peduli Donasi: donasi 4 koin
- Sistem menentukan Juara Donasi: Marcello Juara 1, Manalu Juara 2, Marco Juara 3
- Setelah Donasi selesai -> Mr. Cashflowpoly maju ke Hari 20

## Hari 20 - Sabtu
- Sistem membuka 1 Kartu Harga Emas
- Marco Hari 20 -> Investasi Emas: jual 1 Kartu Emas
- Marcello Hari 20 -> Investasi Emas: beli 1 Kartu Emas
- Hugo Hari 20 -> Investasi Emas: beli 1 Kartu Emas
- Manalu Hari 20 -> Investasi Emas: tidak membeli dan tidak menjual
- Setelah Investasi Emas selesai -> Mr. Cashflowpoly maju ke Hari 21

## Hari 21 - Minggu
- Hari 21 -> Libur
- Mr. Cashflowpoly langsung lanjut ke Hari 22

## Hari 22 - Senin
- Marco Hari 22 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Sayur + Bumbu
- Marco Hari 22 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Tersier sesuai Misi Koleksi
- Marcello Hari 22 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Beras + Telur
- Marcello Hari 22 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Tersier sesuai Misi Koleksi
- Hugo Hari 22 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Beras + Telur
- Hugo Hari 22 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Tersier sesuai Misi Koleksi
- Manalu Hari 22 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Daging + Bumbu
- Manalu Hari 22 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Tersier sesuai Misi Koleksi
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 23

## Hari 23 - Selasa
- Marco Hari 23 Giliran 1 -> Kerja Lepas: terima 1 koin
- Marco Hari 23 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Beras
- Marcello Hari 23 Giliran 1 -> Kerja Lepas: terima 1 koin
- Marcello Hari 23 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 23 Giliran 1 -> Kerja Lepas: terima 1 koin
- Hugo Hari 23 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Daging
- Manalu Hari 23 Giliran 1 -> Kerja Lepas: terima 1 koin
- Manalu Hari 23 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 24

## Hari 24 - Rabu
- Marco Hari 24 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Marco Hari 24 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Beras + Telur
- Marcello Hari 24 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Marcello Hari 24 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Daging + Bumbu
- Hugo Hari 24 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 24 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Daging + Bumbu
- Manalu Hari 24 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Manalu Hari 24 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan yang membutuhkan Beras + Telur
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 25

## Hari 25 - Kamis
- Marco Hari 25 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Marco Hari 25 Giliran 2 -> Kerja Lepas: terima 1 koin
- Marcello Hari 25 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Marcello Hari 25 Giliran 2 -> Kerja Lepas: terima 1 koin
- Hugo Hari 25 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Hugo Hari 25 Giliran 2 -> Kerja Lepas: terima 1 koin
- Manalu Hari 25 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Manalu Hari 25 Giliran 2 -> Kerja Lepas: terima 1 koin
- Setelah semua pemain selesai -> Mr. Cashflowpoly menuju Finish

## Akhir Permainan Mode Pemula
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

# 3. Mode Mahir

## Hari 1 - Senin
- Marco Hari 1 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marco Hari 1 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Marcello Hari 1 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Marcello Hari 1 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 1 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Hugo Hari 1 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Manalu Hari 1 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Sayur
- Manalu Hari 1 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 2

## Hari 2 - Selasa
- Marco Hari 2 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan Beras + Telur + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika risiko meminta pembayaran
- Marco Hari 2 Giliran 2 -> Aktifkan 1x Asuransi Multi Risiko; bayar 1 koin ke bank
- Marcello Hari 2 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan Daging + Bumbu + ambil 1 Kartu Risiko Kehidupan; bayar biaya risiko dengan koin jika muncul risiko negatif
- Marcello Hari 2 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Hugo Hari 2 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan Beras + Telur + ambil 1 Kartu Risiko Kehidupan; terima manfaat jika risiko positif muncul
- Hugo Hari 2 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Manalu Hari 2 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan Sayur + Bumbu + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika risiko meminta pembayaran
- Manalu Hari 2 Giliran 2 -> Aktifkan 1x Asuransi Multi Risiko; bayar 1 koin ke bank
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 3

## Hari 3 - Rabu
- Marco Hari 3 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Marco Hari 3 Giliran 2 -> Menabung Tujuan Keuangan: 5 koin
- Marcello Hari 3 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marcello Hari 3 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Hugo Hari 3 Giliran 1 -> Menabung Tujuan Keuangan: 5 koin
- Hugo Hari 3 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Daging
- Manalu Hari 3 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Manalu Hari 3 Giliran 2 -> Menabung Tujuan Keuangan: 5 koin
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 4

## Hari 4 - Kamis
- Marco Hari 4 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Marco Hari 4 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Marcello Hari 4 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan Beras + Telur + ambil 1 Kartu Risiko Kehidupan; bayar biaya risiko dengan koin jika muncul risiko negatif
- Marcello Hari 4 Giliran 2 -> Menabung Tujuan Keuangan: 5 koin
- Hugo Hari 4 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 4 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan Daging + Bumbu + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika risiko meminta pembayaran
- Manalu Hari 4 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Manalu Hari 4 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 5

## Hari 5 - Jumat
- Marco Hari 5 -> Peduli Donasi: donasi 1 koin
- Marcello Hari 5 -> Peduli Donasi: donasi 3 koin
- Hugo Hari 5 -> Peduli Donasi: donasi 2 koin
- Manalu Hari 5 -> Peduli Donasi: donasi 4 koin
- Sistem menentukan Juara Donasi: Manalu Juara 1, Marcello Juara 2, Hugo Juara 3
- Setelah Donasi selesai -> Mr. Cashflowpoly maju ke Hari 6

## Hari 6 - Sabtu
- Sistem membuka 1 Kartu Harga Emas
- Marco Hari 6 -> Investasi Emas: beli 1 Kartu Emas
- Marcello Hari 6 -> Investasi Emas: tidak membeli dan tidak menjual
- Hugo Hari 6 -> Investasi Emas: beli 1 Kartu Emas
- Manalu Hari 6 -> Investasi Emas: beli 1 Kartu Emas
- Setelah Investasi Emas selesai -> Mr. Cashflowpoly maju ke Hari 7

## Hari 7 - Minggu
- Hari 7 -> Libur
- Mr. Cashflowpoly langsung lanjut ke Hari 8

## Hari 8 - Senin
- Marco Hari 8 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan Daging + Bumbu + ambil 1 Kartu Risiko Kehidupan; bayar biaya risiko dengan koin jika muncul risiko negatif
- Marco Hari 8 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Sekunder
- Marcello Hari 8 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Sekunder
- Marcello Hari 8 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Sayur
- Hugo Hari 8 Giliran 1 -> Aktifkan 1x Asuransi Multi Risiko; bayar 1 koin ke bank
- Hugo Hari 8 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Sekunder
- Manalu Hari 8 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan Beras + Telur + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika risiko meminta pembayaran
- Manalu Hari 8 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Sekunder
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 9

## Hari 9 - Selasa
- Marco Hari 9 Giliran 1 -> Menabung Tujuan Keuangan: 10 koin
- Marco Hari 9 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Beras
- Marcello Hari 9 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan Sayur + Bumbu + ambil 1 Kartu Risiko Kehidupan; jika koin kurang, ambil 1 Kartu Pinjaman Syariah untuk menutup biaya risiko
- Marcello Hari 9 Giliran 2 -> Aktifkan 1x Asuransi Multi Risiko; bayar 1 koin ke bank
- Hugo Hari 9 Giliran 1 -> Menabung Tujuan Keuangan: 10 koin
- Hugo Hari 9 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Beras
- Manalu Hari 9 Giliran 1 -> Menabung Tujuan Keuangan: 10 koin
- Manalu Hari 9 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Daging
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 10

## Hari 10 - Rabu
- Marco Hari 10 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Marco Hari 10 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan Beras + Telur + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika risiko meminta pembayaran
- Marcello Hari 10 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marcello Hari 10 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Hugo Hari 10 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Hugo Hari 10 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan Beras + Telur + ambil 1 Kartu Risiko Kehidupan; bayar biaya risiko dengan koin jika muncul risiko negatif
- Manalu Hari 10 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Manalu Hari 10 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan Daging + Bumbu + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika risiko meminta pembayaran
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 11

## Hari 11 - Kamis
- Marco Hari 11 Giliran 1 -> Bayar 1 Kartu Pinjaman Syariah: serahkan 10 koin ke bank
- Marco Hari 11 Giliran 2 -> Aktifkan 1x Asuransi Multi Risiko; bayar 1 koin ke bank
- Marcello Hari 11 Giliran 1 -> Bayar 1 Kartu Pinjaman Syariah: serahkan 10 koin ke bank
- Marcello Hari 11 Giliran 2 -> Menabung Tujuan Keuangan: 5 koin
- Hugo Hari 11 Giliran 1 -> Bayar 1 Kartu Pinjaman Syariah: serahkan 10 koin ke bank
- Hugo Hari 11 Giliran 2 -> Aktifkan 1x Asuransi Multi Risiko; bayar 1 koin ke bank
- Manalu Hari 11 Giliran 1 -> Bayar 1 Kartu Pinjaman Syariah: serahkan 10 koin ke bank
- Manalu Hari 11 Giliran 2 -> Aktifkan 1x Asuransi Multi Risiko; bayar 1 koin ke bank
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 12

## Hari 12 - Jumat
- Marco Hari 12 -> Peduli Donasi: donasi 4 koin
- Marcello Hari 12 -> Peduli Donasi: donasi 2 koin
- Hugo Hari 12 -> Peduli Donasi: donasi 5 koin
- Manalu Hari 12 -> Peduli Donasi: donasi 3 koin
- Sistem menentukan Juara Donasi: Hugo Juara 1, Marco Juara 2, Manalu Juara 3
- Setelah Donasi selesai -> Mr. Cashflowpoly maju ke Hari 13

## Hari 13 - Sabtu
- Sistem membuka 1 Kartu Harga Emas
- Marco Hari 13 -> Investasi Emas: jual 1 Kartu Emas
- Marcello Hari 13 -> Investasi Emas: beli 1 Kartu Emas
- Hugo Hari 13 -> Investasi Emas: beli 1 Kartu Emas
- Manalu Hari 13 -> Investasi Emas: jual 1 Kartu Emas
- Setelah Investasi Emas selesai -> Mr. Cashflowpoly maju ke Hari 14

## Hari 14 - Minggu
- Hari 14 -> Libur
- Mr. Cashflowpoly langsung lanjut ke Hari 15

## Hari 15 - Senin
- Marco Hari 15 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Tersier sesuai Misi Koleksi
- Marco Hari 15 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Sayur
- Marcello Hari 15 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Tersier sesuai Misi Koleksi
- Marcello Hari 15 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 15 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Tersier sesuai Misi Koleksi
- Hugo Hari 15 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Daging
- Manalu Hari 15 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Tersier sesuai Misi Koleksi
- Manalu Hari 15 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Beras
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 16

## Hari 16 - Selasa
- Marco Hari 16 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Marco Hari 16 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan Sayur + Bumbu + ambil 1 Kartu Risiko Kehidupan; bayar biaya risiko dengan koin jika muncul risiko negatif
- Marcello Hari 16 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Marcello Hari 16 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan Daging + Bumbu + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika risiko meminta pembayaran
- Hugo Hari 16 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 16 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan Daging + Bumbu + ambil 1 Kartu Risiko Kehidupan; bayar biaya risiko dengan koin jika muncul risiko negatif
- Manalu Hari 16 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Telur
- Manalu Hari 16 Giliran 2 -> Klaim 1 Kartu Pesanan Masakan Beras + Telur + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika risiko meminta pembayaran
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 17

## Hari 17 - Rabu
- Marco Hari 17 Giliran 1 -> Menabung Tujuan Keuangan: 10 koin
- Marco Hari 17 Giliran 2 -> Jika tabungan cukup, ambil 1 Kartu Tujuan Keuangan
- Marcello Hari 17 Giliran 1 -> Menabung Tujuan Keuangan: 10 koin
- Marcello Hari 17 Giliran 2 -> Jika tabungan cukup, ambil 1 Kartu Tujuan Keuangan
- Hugo Hari 17 Giliran 1 -> Menabung Tujuan Keuangan: 10 koin
- Hugo Hari 17 Giliran 2 -> Jika tabungan cukup, ambil 1 Kartu Tujuan Keuangan
- Manalu Hari 17 Giliran 1 -> Menabung Tujuan Keuangan: 10 koin
- Manalu Hari 17 Giliran 2 -> Jika tabungan cukup, ambil 1 Kartu Tujuan Keuangan
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 18

## Hari 18 - Kamis
- Marco Hari 18 Giliran 1 -> Aktifkan 1x Asuransi Multi Risiko; bayar 1 koin ke bank
- Marco Hari 18 Giliran 2 -> Kerja Lepas: terima 1 koin
- Marcello Hari 18 Giliran 1 -> Aktifkan 1x Asuransi Multi Risiko; bayar 1 koin ke bank
- Marcello Hari 18 Giliran 2 -> Kerja Lepas: terima 1 koin
- Hugo Hari 18 Giliran 1 -> Aktifkan 1x Asuransi Multi Risiko; bayar 1 koin ke bank
- Hugo Hari 18 Giliran 2 -> Kerja Lepas: terima 1 koin
- Manalu Hari 18 Giliran 1 -> Aktifkan 1x Asuransi Multi Risiko; bayar 1 koin ke bank
- Manalu Hari 18 Giliran 2 -> Kerja Lepas: terima 1 koin
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 19

## Hari 19 - Jumat
- Marco Hari 19 -> Peduli Donasi: donasi 2 koin
- Marcello Hari 19 -> Peduli Donasi: donasi 4 koin
- Hugo Hari 19 -> Peduli Donasi: donasi 3 koin
- Manalu Hari 19 -> Peduli Donasi: donasi 5 koin
- Sistem menentukan Juara Donasi: Manalu Juara 1, Marcello Juara 2, Hugo Juara 3
- Setelah Donasi selesai -> Mr. Cashflowpoly maju ke Hari 20

## Hari 20 - Sabtu
- Sistem membuka 1 Kartu Harga Emas
- Marco Hari 20 -> Investasi Emas: beli 1 Kartu Emas
- Marcello Hari 20 -> Investasi Emas: jual 1 Kartu Emas
- Hugo Hari 20 -> Investasi Emas: beli 1 Kartu Emas
- Manalu Hari 20 -> Investasi Emas: tidak membeli dan tidak menjual
- Setelah Investasi Emas selesai -> Mr. Cashflowpoly maju ke Hari 21

## Hari 21 - Minggu
- Hari 21 -> Libur
- Mr. Cashflowpoly langsung lanjut ke Hari 22

## Hari 22 - Senin
- Marco Hari 22 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Marco Hari 22 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Marcello Hari 22 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Sayur
- Marcello Hari 22 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Hugo Hari 22 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Beras
- Hugo Hari 22 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Telur
- Manalu Hari 22 Giliran 1 -> Beli 1 Kartu Bahan Masakan: Daging
- Manalu Hari 22 Giliran 2 -> Beli 1 Kartu Bahan Masakan: Bumbu
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 23

## Hari 23 - Selasa
- Marco Hari 23 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan Beras + Telur + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika risiko meminta pembayaran
- Marco Hari 23 Giliran 2 -> Kerja Lepas: terima 1 koin
- Marcello Hari 23 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan Sayur + Bumbu + ambil 1 Kartu Risiko Kehidupan; bayar biaya risiko dengan koin jika muncul risiko negatif
- Marcello Hari 23 Giliran 2 -> Kerja Lepas: terima 1 koin
- Hugo Hari 23 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan Beras + Telur + ambil 1 Kartu Risiko Kehidupan; gunakan Asuransi aktif jika risiko meminta pembayaran
- Hugo Hari 23 Giliran 2 -> Kerja Lepas: terima 1 koin
- Manalu Hari 23 Giliran 1 -> Klaim 1 Kartu Pesanan Masakan Daging + Bumbu + ambil 1 Kartu Risiko Kehidupan; bayar biaya risiko dengan koin jika muncul risiko negatif
- Manalu Hari 23 Giliran 2 -> Kerja Lepas: terima 1 koin
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 24

## Hari 24 - Rabu
- Marco Hari 24 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Marco Hari 24 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Sekunder
- Marcello Hari 24 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Marcello Hari 24 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Sekunder
- Hugo Hari 24 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Hugo Hari 24 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Sekunder
- Manalu Hari 24 Giliran 1 -> Beli 1 Kartu Aneka Kebutuhan: Primer
- Manalu Hari 24 Giliran 2 -> Beli 1 Kartu Aneka Kebutuhan: Sekunder
- Setelah semua pemain selesai -> Mr. Cashflowpoly maju ke Hari 25

## Hari 25 - Kamis
- Marco Hari 25 Giliran 1 -> Cek Pinjaman Syariah; jika masih ada pinjaman, bayar 1 Kartu Pinjaman Syariah
- Marco Hari 25 Giliran 2 -> Kerja Lepas: terima 1 koin
- Marcello Hari 25 Giliran 1 -> Cek Pinjaman Syariah; jika masih ada pinjaman, bayar 1 Kartu Pinjaman Syariah
- Marcello Hari 25 Giliran 2 -> Kerja Lepas: terima 1 koin
- Hugo Hari 25 Giliran 1 -> Cek Pinjaman Syariah; jika masih ada pinjaman, bayar 1 Kartu Pinjaman Syariah
- Hugo Hari 25 Giliran 2 -> Kerja Lepas: terima 1 koin
- Manalu Hari 25 Giliran 1 -> Cek Pinjaman Syariah; jika masih ada pinjaman, bayar 1 Kartu Pinjaman Syariah
- Manalu Hari 25 Giliran 2 -> Kerja Lepas: terima 1 koin
- Setelah semua pemain selesai -> Mr. Cashflowpoly menuju Finish

## Akhir Permainan Mode Mahir
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
Bagian ini membantu mengubah skenario naratif menjadi payload event.

| Jenis aksi | Contoh action_type | Actor type | Catatan payload |
|---|---|---|---|
| Beli bahan masakan | `ingredient.purchased` | `PLAYER` | Nama bahan, jumlah, harga/koin bila tersedia. |
| Klaim pesanan masakan | `order.claimed` | `PLAYER` | Bahan yang dipakai, hasil pesanan, perubahan koin. |
| Beli kebutuhan | `need.primary.purchased`, `need.secondary.purchased`, `need.tertiary.purchased` | `PLAYER` | Tipe kebutuhan dan biaya. |
| Kerja lepas | `work.freelance.completed` | `PLAYER` | Jumlah koin diterima. |
| Donasi Jumat | `day.friday.donation` | `PLAYER` | Jumlah donasi. |
| Peringkat donasi | `donation.rank.awarded` | `SYSTEM` | Urutan juara dan poin. |
| Investasi emas | `day.saturday.gold_trade` | `PLAYER` | Beli/jual/tidak transaksi dan harga emas. |
| Tabungan tujuan keuangan | `saving.deposit.created` | `PLAYER` | Jumlah tabungan. |
| Tujuan keuangan tercapai | `saving.goal.achieved` | `PLAYER` | Kartu tujuan keuangan yang diambil. |
| Pinjaman syariah | `loan.syariah.taken`, `loan.syariah.repaid` | `PLAYER` | Nominal pinjaman atau pembayaran. |
| Asuransi multi risiko | `insurance.multirisk.purchased`, `insurance.multirisk.used` | `PLAYER` | Status asuransi dan risiko yang ditanggung. |
| Risiko kehidupan | `risk.life.drawn` | `PLAYER` | Jenis risiko, dampak, dan apakah diasuransikan. |
| Perpindahan hari | `turn.ended` atau event sistem setara | `SYSTEM` | Hari tujuan dan status giliran. |
| Akhir permainan | `session.ended` | `SYSTEM` | Status finish dan ringkasan akhir. |

## 5. Kriteria Validasi Skenario
1. Setiap sesi memiliki instruktur dan mode permainan yang jelas.
2. Setiap hari memiliki urutan aksi pemain atau aksi sistem.
3. Hari Senin sampai Kamis memiliki maksimal dua giliran per pemain.
4. Hari Jumat berisi mekanik donasi.
5. Hari Sabtu berisi mekanik investasi emas.
6. Hari Minggu berisi mekanik libur.
7. Mode Mahir memiliki event tambahan untuk risiko, asuransi, pinjaman syariah, dan tujuan keuangan.
8. Akhir permainan memiliki proses perhitungan poin dan penentuan pemenang.
