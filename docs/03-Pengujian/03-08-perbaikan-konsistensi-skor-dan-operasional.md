# Perbaikan konsistensi skor dan operasional — 16 September 2026

Laporan ini melanjutkan [audit sebelumnya](03-07-perbaikan-audit-menyeluruh.md). Pemeriksaan tambahan menemukan jalur yang belum tercakup oleh pengujian sebelumnya. Cakupan laporan adalah kode dan lingkungan lokal; status produksi harus diverifikasi terpisah.

## Perubahan dan alasan

| Area | Perbaikan | Alasan |
|---|---|---|
| Skor awal | Poin kebahagiaan awal masuk ke analitika, komposisi UI, dan komponen skor final. | Total sebelumnya menghilangkan nilai awal ruleset. |
| Misi koleksi | Hadiah sesuai katalog dihitung dan diterapkan tepat sekali. Dua representasi penalti dinormalisasi; nilai yang bertentangan ditolak. | Konfigurasi hadiah sebelumnya disimpan tetapi tidak memengaruhi hasil. |
| Tabungan awal | Validator dan analitika memakai alokasi kronologis yang sama: dana khusus tujuan dipakai dahulu, kemudian sisa tabungan awal bersama. | Pembelian sah sebelumnya ditolak, dan setoran setelah pembelian bisa menghasilkan saldo tujuan yang keliru. Dana tujuan lain tetap terpisah. |
| Detail sesi | Gangguan permintaan status, roster, atau timeline mempertahankan analitika utama yang sudah tersedia. | Gangguan parsial tidak boleh menghapus seluruh tampilan atau mengarang nilai nol. |
| Detail ruleset | Gangguan katalog komponen tidak membatalkan detail utama ruleset. | Pengguna tetap dapat membaca data yang berhasil dimuat. |
| Perubahan mode ruleset | Saat Pemula diubah menjadi Mahir, katalog mekanik lanjutan yang belum tersedia dilengkapi dari default Mahir. Input dan katalog kustom yang sudah ada dipertahankan. | Form sebelumnya mengaktifkan mekanik dengan katalog kosong sehingga API menolak penyimpanan. |
| Validasi donasi | Tipe JSON untuk rank, points, dan player_order_no diperiksa sebelum dibaca sebagai integer. | Nilai string, null, Boolean, array, atau object harus menghasilkan galat field, bukan exception. |
| Narasi pesanan | V007 memisahkan akses field trigger berdasarkan tabel pemanggil. | Penjualan masakan dengan narasi bawaan gagal karena tabel narasi tidak memiliki kolom turn_number. |
| Jaringan deployment | Nginx memakai alamat tetap .3, tunnel .2 pada jaringan ingress. | Alokasi dinamis pada jaringan baru dapat memberikan .2 kepada Nginx dan menggagalkan startup tunnel. |
| HTTPS | UI produksi memiliki tujuan redirect port 443; route aset Nginx meneruskan skema dan identitas yang sudah dinormalisasi. | Redirect harus bekerja saat diwajibkan dan tidak berulang pada aset yang diterima melalui HTTPS. |
| Dokumentasi | Sumber tabel transaksi, batas tampilan trace ID, komponen skor, alokasi tabungan, peringkat final, dan fungsi migrasi diselaraskan. | Dokumentasi harus menjelaskan implementasi yang dapat diamati. |

V006 mengoreksi komponen skor final historis dari katalog versi ruleset sesi dan menambahkan perlindungan hadiah misi tepat sekali. V007 memperbaiki trigger bersama tanpa menulis ulang baseline atau migrasi lama. Snapshot analitika tetap dihitung ulang melalui prosedur deployment.

Seed 2 tetap dipertahankan untuk produksi. Hadziq dan Pratama masing-masing memiliki 10 ruleset yang digunakan oleh sesi contoh; sesi Pemula dan Mahir tetap terpisah.

## Bukti verifikasi

Build Release dengan warning-as-error lulus tanpa warning atau error. Seluruh 427 tes API/integrasi/performa dan 445 tes UI lulus, tanpa tes gagal atau dilewati. Regresi baru mencakup poin awal, hadiah/penalti misi kustom, finalisasi, recompute berulang, koreksi skor historis, tabungan awal lintas tujuan, kegagalan API parsial, perubahan mode ruleset, tipe payload donasi, dan penjualan masakan beserta narasinya.

Uji performa terkontrol menggunakan 20 sesi × 2.000 event dan 20 klien × 10 permintaan per endpoint. P95 pencatatan event 351,8 ms (batas 500 ms); pembacaan analitika sesi 435,4 ms (batas 1.500 ms). Ini ukuran fixture lokal, bukan jaminan latensi jaringan produksi.

Empat skenario regresi skrip deployment, alokasi jaringan ingress baru, header proxy, penolakan spoofing, pemisahan kuota pengunjung, dan respons rate limit 429 lulus. Probe UI aktual menunjukkan HTTP halaman diarahkan ke HTTPS dengan 307, forwarded HTTPS halaman/aset mengembalikan 200, dan health tetap dapat dibaca lewat HTTP. Audit NuGet serta npm UI/E2E melaporkan nol kerentanan. Pemeriksaan kontrak dokumentasi dan tautan lokal lulus pada 28 Markdown.

Pengujian browser penuh meluluskan 82 dari 82 kasus pada Chromium desktop dan emulasi Pixel 7, tanpa kegagalan, pengulangan, atau kasus dilewati. Audit halaman pemain/instruktur mencocokkan 1.584 nilai grafik dengan snapshot API. Alur simpan perubahan Pemula menjadi Mahir juga lulus melalui form browser dan API nyata. Pemeriksaan visual langsung mencakup filter, formulir ruleset, susunan ponsel, dan panel rincian grafik saat hover. Kecocokan angka UI terhadap API dilengkapi regresi perhitungan serta state database di atas, bukan dianggap sebagai bukti tunggal kebenaran sumber.

Artefak putaran ini berada pada `artifacts/fixes-final/`; bukti ingress, header proxy, HTTPS, serta regresi deployment berada pada `artifacts/fixes-recheck/ops/`. Bukti utama: `build-final.log`, `verified-results/api-final.trx`, hasil UI dalam `verified-results/`, `e2e-report.json`, `docs-final.log`, dan `dev-seed-verified.log`. Artefak lokal diabaikan Git dan tidak diperlukan saat aplikasi berjalan.

Hasil gagal awal tetap disimpan agar riwayat perbaikan dapat ditelusuri. Pengulangan akhir API meluluskan seluruh 427 kasus setelah V007 memperbaiki kegagalan narasi; seluruh 445 kasus UI dan 82 kasus browser juga lulus. Semua temuan terkonfirmasi pada tabel perubahan di atas sudah ditangani dalam cakupan pengujian ini.

## Lingkungan development

Database lokal dicadangkan sebelum penerapan. V006 ternyata sudah diterapkan oleh proses development sebelumnya; checksum yang tercatat cocok persis dengan berkas saat ini setelah satu komentar deskripsi file di baris pertama dihilangkan. Tidak ada perubahan SQL fungsional di antara kedua versi tersebut. Riwayat lokal diselaraskan dalam transaksi yang memeriksa checksum lama secara eksplisit; pemeriksaan checksum aplikasi tidak diubah atau dimatikan. Ini koreksi metadata development untuk tambahan komentar pada migrasi yang belum dirilis, bukan prosedur mengubah migrasi produksi.

V007 kemudian diterapkan, Seed 2 dijalankan ulang secara idempoten, dan 24 sesi dihitung ulang. Hadziq serta Pratama masing-masing memiliki 10 ruleset dan semuanya terpakai, dengan empat sesi selesai per mode serta sesi persiapan/berjalan. Tidak ditemukan ketidaksesuaian status ENDED terhadap state akhir. API, UI, dan database development kembali berjalan; halaman login dan `/health/ready` pada port 5203 mengembalikan 200.

## Batas hasil

Tes otomatis dan pemeriksaan visual membuktikan perilaku pada kasus yang dijalankan. Hasil tersebut bukan bukti bahwa seluruh kombinasi input, perangkat fisik IDN, browser, atau kondisi server produksi bebas cacat. Tidak ada commit, push, atau deployment produksi pada putaran ini.
