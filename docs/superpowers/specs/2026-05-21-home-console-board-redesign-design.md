# Home Console Board Redesign

## Ringkasan

Halaman home saat ini terasa seperti template generatif: glow besar, panel glass, badge berbentuk pil yang berulang, dan daftar bullet fitur yang membuat hero terlihat seperti landing page umum, bukan dashboard operasional. Perubahan ini merombak hero menjadi `console board` yang lebih ringkas, tegas, dan fokus pada status sistem.

## Masalah Saat Ini

- Hero terlalu panjang dan terlalu dekoratif untuk halaman operasional.
- Panel kanan memakai gaya glass/glow yang membuat informasi inti terlihat kurang tegas.
- Bullet fitur berwarna di bawah CTA menambah noise visual dan memperkuat kesan template.
- Empat metrik kanan dipisah menjadi kartu-kartu kecil yang terasa melayang, bukan sebagai satu papan data.

## Tujuan

- Mengubah hero menjadi panel operasional yang lebih padat.
- Menjaga semua data realtime yang sudah ada tetap tampil dan tetap hidup.
- Mengurangi efek visual yang terlalu lembut atau dekoratif.
- Mempertahankan bahasa visual repo, tetapi dengan tampilan yang lebih tegas dan kurang “AI”.

## Bukan Tujuan

- Tidak mengubah API, polling realtime, atau format data.
- Tidak mengubah section di bawah hero.
- Tidak mengubah copy utama, kecuali penyesuaian kecil bila dibutuhkan oleh struktur visual baru.

## Desain yang Disetujui

### Struktur Umum

Hero tetap dua kolom pada desktop dan bertumpuk pada mobile, tetapi tinggi totalnya dipangkas. Hero dibagi menjadi:

- `home-console-intro`
- `home-console-board`

Kolom kiri menjadi panel identitas halaman. Kolom kanan menjadi papan status operasional tunggal.

### Kolom Kiri: Intro Ringkas

Kolom kiri hanya menampilkan:

- badge kecil
- headline
- satu subjudul singkat
- dua CTA

Daftar bullet fitur empat warna dihapus sepenuhnya. Elemen ini tidak diganti dengan komponen baru; ruang kosong yang dihasilkan dipakai untuk memperpendek hero dan memberi fokus pada panel data di kanan.

### Kolom Kanan: Console Board

Panel kanan diubah dari kartu glass menjadi satu papan status.

Isi panel:

1. Baris status kecil
   - indikator live kecil
   - label realtime
   - status badge singkat

2. Blok waktu utama
   - label waktu server
   - jam server besar
   - label sinkronisasi terakhir

3. Progress strip
   - progress aktif dibanding total sesi
   - tetap memakai data `progressPercent`

4. Grid metrik inti `2 x 2`
   - total sesi
   - sesi aktif
   - pemain dipantau / rekan sesi
   - set aturan aktif / ruleset tersedia

Empat metrik ini tidak lagi tampil sebagai kartu kecil terpisah. Mereka berada dalam satu bidang yang sama, dipisah garis tipis agar terasa seperti papan data.

## Arah Visual

### Yang Dihilangkan

- glow radial besar
- nuansa glass yang terlalu lembut
- shadow tebal yang membuat panel terasa mengambang
- daftar bullet fitur dekoratif

### Yang Dipertahankan

- identitas warna utama repo
- CTA primer dan sekunder
- indikator live
- progress bar
- semua binding ID dan hook JavaScript yang dipakai polling realtime

### Bahasa Visual Baru

- background lebih tenang dan lebih flat
- border lebih tegas
- radius sedikit lebih kecil
- shadow tipis
- aksen warna hanya di titik penting
- angka lebih dominan dan lebih stabil secara visual

Nuansa akhirnya harus terasa seperti dashboard kerja, bukan hero marketing.

## Responsif

### Desktop

- dua kolom tetap dipertahankan
- papan kanan lebih padat dan lebih vertikal
- headline kiri diberi lebar maksimum agar tidak terlalu melebar

### Mobile

- layout bertumpuk
- CTA boleh wrap menjadi dua baris
- papan status tetap memakai grid dua kolom untuk metrik inti
- jarak antarblok diperkecil agar tetap terasa ringkas

## Implementasi

### Razor

File utama:

- `src/Cashflowpoly.Ui/Views/Home/Index.cshtml`

Perubahan:

- mengganti struktur hero menjadi blok intro dan board yang lebih eksplisit
- menghapus list bullet fitur dari hero
- mempertahankan seluruh ID DOM yang dipakai script:
  - `home-live-time`
  - `home-last-synced`
  - `home-progress-bar`
  - `home-total-sessions`
  - `home-active-sessions`
  - `home-total-players`
  - `home-total-rulesets`
  - `home-realtime-error`

### CSS

File utama:

- `src/Cashflowpoly.Ui/wwwroot/css/site.css`

Perubahan:

- override khusus untuk `home-hero`
- menambah kelas layout baru untuk intro dan board
- mengubah panel kanan menjadi board tunggal
- mengurangi glow, blur, dan shadow
- menata ulang grid metrik agar lebih rapat dan terasa struktural

## Error Handling dan Data Kosong

- Jika `Model.ErrorMessage` ada, alert tetap ditampilkan di dalam board.
- Jika `hasWorkspaceData` bernilai false, empty state tetap ada, tetapi tampil sebagai sub-panel ringkas di dalam board, bukan sebagai kotak dekoratif besar.

## Pengujian

### Fungsional

- halaman home render untuk instructor
- halaman home render untuk player
- polling realtime tetap memperbarui jam, sinkronisasi, angka, dan progress

### Visual

- desktop: hero tampak lebih pendek dan lebih tegas
- mobile: CTA tidak pecah jelek dan grid metrik tetap terbaca
- tidak ada elemen glow besar yang mendominasi hero

### Regresi

- link CTA tetap benar
- `id` elemen yang dipakai script tidak berubah
- section di bawah hero tidak terpengaruh

## Risiko dan Mitigasi

- Risiko: tampilan terlalu kaku dibanding halaman lain.
  Mitigasi: warna repo tetap dipakai, tetapi dibatasi sebagai aksen.

- Risiko: script realtime rusak bila ID berubah.
  Mitigasi: semua hook DOM dipertahankan apa adanya.

- Risiko: mobile menjadi terlalu padat.
  Mitigasi: grid metrik tetap dua kolom dengan padding dan font yang disesuaikan.
