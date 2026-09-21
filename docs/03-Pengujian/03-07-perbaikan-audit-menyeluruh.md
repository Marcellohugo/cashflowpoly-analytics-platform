# Perbaikan audit menyeluruh — 16 September 2026

Dokumen ini melacak perbaikan atas 40 catatan audit gabungan. Cakupannya working tree lokal, termasuk perubahan sebelumnya. Hasil di sini tidak membuktikan bahwa server produksi telah menerima perubahan; deployment dinilai terpisah.

Pemeriksaan lanjutan menemukan kasus tambahan di luar matriks ini. Status terbaru berada pada [perbaikan konsistensi skor dan operasional](03-08-perbaikan-konsistensi-skor-dan-operasional.md); hasil di bawah merupakan bukti untuk putaran pengujian sebelumnya.

## Perubahan dan alasan

| Catatan | Perbaikan | Mengapa diperlukan |
|---|---|---|
| A01, A03 | Kanonisasi aksi sebelum validasi dan pemeriksaan pasangan aksi/aktor pada API serta SQL. | Spasi pada nama aksi atau penggunaan SYSTEM tidak boleh melewati nominal dan batas aksi pemain. |
| A02, A09 | Slot dihitung dari aksi PLAYER; duplikat diperiksa setelah akses dan sebelum urutan sesi. | Event tujuan sistem tidak menghabiskan slot; retry event lama tetap dikenali sebagai duplikat. |
| A04, A05, A06 | Penolakan elemen batch/definisi null serta batas nama pemain 80 dan sesi 120 karakter. | Input tidak valid harus menghasilkan galat field 400, bukan exception database atau penyimpanan batch sebagian tanpa hasil. |
| A07 | Perubahan roster dan setup memakai lock baris sesi yang sama, pemeriksaan ulang kapasitas/anggota, dan penyusunan urutan tanpa posisi sementara di luar batas. | Permintaan bersamaan tidak boleh menghasilkan roster yang berbeda dari setup tersimpan atau respons 500 saat penuh. |
| A08 | Event penutupan publik ditolak dengan petunjuk endpoint `/end`. | Finalisasi state, skor, dan status sesi harus berjalan melalui transaksi yang sama. |
| A10 | Endpoint sections memakai scope kepesertaan ruleset yang sama dengan detail/components, termasuk katalog default. | Peserta sah dapat membaca aturannya, sementara ruleset sesi asing tetap tertutup. |
| B01 | Replay Seed 2 mencakup akhir sesi dan menggunakan harga yang berlaku. | Label ENDED, state permainan, kegagalan misi, serta skor harus saling cocok. |
| B02, B03, B06 | FIFO bahan mengikuti discard dan bahan awal; nominal koin pecahan ditolak. | Biaya pesanan dan inventori validasi harus mengikuti transaksi fisik yang sama. |
| B04 | Tanggal proyeksi mengikuti hari event; pergantian hari tetap memakai hari berikutnya. | Tanggal pembelian/penyelesaian tidak boleh bergeser satu hari. |
| B05, B10 | Peserta tanpa transaksi tetap mendapat modal awal; tabungan awal masuk tepat sekali ke metrik saldo, aset likuid, dan pensiun. | Peringkat serta metrik tidak boleh menghilangkan dana awal yang sah. |
| B07, B08, B09 | Efek harga membaca ValueDelta dan target pemain; pembelian bahan gratis didukung. | Harga katalog, efek risiko, validator, dan SQL harus menghasilkan harga yang sama. |
| B11 | Misi memakai persyaratan ASSET/TIER/FAMILY dari ruleset yang dikunci. | Nama misi bukan penentu persyaratan; pembelian yang sudah memenuhi misi tetap tercatat setelah penjualan. |
| C01, C02 | Batas form registrasi disamakan dengan API; tes gangguan API benar-benar mengirim permintaan. | Validasi browser dan tes harus menguji alur yang dimaksud. |
| C03, C07, C08 | Gangguan API mempertahankan input/data utama; jumlah yang belum diketahui tidak diubah menjadi nol; identitas pemain berasal dari roster. Struktur JSON formulir yang tidak valid juga dikembalikan sebagai galat formulir. | Gangguan parsial atau masukan rusak tidak boleh menghilangkan input, menghasilkan 500, atau membuat tampilan mengarang data. |
| C04 | Parsing angka memvalidasi keseluruhan nilai dan integer. | Nilai seperti `1e2` tidak boleh diam-diam berubah dari 100 menjadi 1. |
| C05, C06 | Timeline menampilkan tujuan SYSTEM milik pemain dan menjaga fokus kalender saat diperbarui. | Aktivitas relevan tetap terlihat dan navigasi keyboard tidak terputus. |
| C09 | Screenshot rincian hover diambil tanpa memindahkan viewport dari titik aktif. | Tes visual harus menangkap panel yang benar-benar terlihat, tanpa mengubah perilaku hover yang diminta pengguna. |
| D01–D09 | Manual, kontrak event, payload skenario, batas API, scope, logging, navigasi, jumlah request, dan kemampuan recompute diselaraskan. | Dokumen harus menjelaskan perilaku nyata dan prosedur yang dapat dijalankan. |
| O01 | Nginx hanya mempercayai IP tunnel tertentu, menormalisasi header pengunjung, dan memakai 429 untuk batas laju. | Pengguna tunnel tidak boleh berbagi kuota karena IP proxy yang sama atau menyuntikkan identitas lewat header. |

Seed 2 tetap tersedia untuk produksi: Hadziq dan Pratama masing-masing memiliki 10 ruleset yang seluruhnya terpakai, empat sesi selesai per mode, serta sesi persiapan dan berjalan. Perubahan tidak menghapus akun atau sesi non-demo.

Migrasi baru disimpan terpisah; baseline `00_create_schema.sql` dan migrasi V002–V004 tidak ditulis ulang. Perbaikan data historis harus berasal dari event atau katalog ruleset terkait, bukan perkiraan dari teks tampilan.

## Verifikasi

Build Release akhir dengan warning-as-error lulus: 0 warning dan 0 error. Seluruh 411 tes API/integrasi/performa serta 428 tes UI lulus, tanpa kegagalan atau tes dilewati. Regresi mencakup input null, batas nama, roster/setup bersamaan, akses lintas akun, harga risiko, saldo awal, FIFO bahan, serta misi custom dari sesi berjalan sampai finalisasi dan recompute. Pemeriksaan kontrak dokumentasi dan tautan lokal lulus pada 27 Markdown.

Uji beban terkontrol memakai 20 sesi × 2.000 event beserta proyeksi, dengan 20 klien × 10 permintaan per endpoint. P95 pencatatan event sebesar 136,5 ms (batas 500 ms) dan pembacaan analitika sesi 351,8 ms (batas 1.500 ms). Angka ini mengukur fixture lokal, bukan jaringan produksi atau uji beban berkepanjangan.

Pengujian browser penuh pada Chromium desktop dan emulasi Pixel 7 mencakup 82 kasus. Putaran pertama meluluskan 80 kasus; dua kasus gagal saat capture panel hover karena scroll halus belum selesai. Keempat audit halaman/data lulus dan mencocokkan 1.584 nilai grafik dengan snapshot API. Tes screenshot diperbaiki dengan menempatkan viewport secara langsung sebelum hover; pengulangan empat kasus terkait lulus seluruhnya, tanpa retry, kegagalan, atau tes dilewati. Dengan pengulangan terarah tersebut, seluruh 82 kasus memiliki hasil akhir lulus; ini bukan klaim bahwa putaran penuh pertama bebas kegagalan. Probe visual hover terpisah juga lulus: panel berada di bawah grafik, terlihat sebelum/sesudah screenshot viewport, dan tertutup setelah penunjuk keluar. Screenshot desktop dan ponsel diperiksa langsung.

Dengan hasil tersebut, seluruh 40 catatan audit pada matriks di atas sudah ditangani dan diverifikasi sesuai cakupannya. Pemeriksaan tidak meliputi perangkat IDN fisik, seluruh kombinasi custom ruleset, semua browser/perangkat, atau beban berkepanjangan. Data UI yang cocok dengan API dilengkapi regresi kalkulator dan transaksi untuk memeriksa kebenaran sumbernya.

Tes Nginx terisolasi sudah membuktikan header proxy tepercaya, penolakan spoofing, enam pengunjung dengan kuota terpisah, serta respons 429 untuk pengunjung yang melampaui batas. Empat regresi deployment dan validasi konfigurasi produksi lokal juga lulus; konektivitas VPS/tunnel publik belum diuji dalam putaran ini. Tidak ada commit, push, atau deployment pada putaran perbaikan ini.

Artefak lokal berada di `artifacts/fixes-round12/`. Folder artifacts tidak menjadi dependensi runtime atau persyaratan deployment.

Bukti utama: `build-final.log`, `results/api-verified.trx`, `results/audit-fixes-final_net10.0_20260916004516.trx` (UI), `e2e-report.json`, `e2e-feedback-verified.json`, `docs-final.log`, `nginx-test.log`, `deployment-test.log`, dan `dev-seed-verified.log`. Satu percobaan pengulangan browser gagal tersambung karena Docker/host pengujian telah berhenti setelah jeda sesi; hasil gagal tersebut disimpan pada `e2e-feedback-report.json`, lalu host terisolasi dipulihkan sebelum pengulangan yang lulus.

## Lingkungan lokal

Watcher development sempat menjalankan draft V005 yang belum dirilis sebelum isinya selesai. Database lokal dicadangkan, lalu definisi fungsi dan koreksi data V005 final diterapkan dalam satu transaksi dengan pemeriksaan checksum draft yang telah diamati. Hanya catatan V005 pada database development tersebut diselaraskan setelah SQL berhasil. Pemeriksaan checksum aplikasi tetap aktif; prosedur ini bukan petunjuk mengubah migrasi yang sudah dirilis ke produksi.

Seed 2 kemudian dijalankan ulang pada cakupan akun/sesi demo dan analitika 24 sesi dihitung ulang. Pemeriksaan lokal menunjukkan 10 ruleset seed per instruktur, semua terpakai, dan 0 ketidaksesuaian antara status ENDED dan state akhir permainan. Data non-demo tetap dipertahankan. API, UI, dan database development kembali sehat; `/health/ready` pada port 5203 mengembalikan 200.
