/* Fungsi file: Mengatur tema, source scan, dan ekstensi utilitas Tailwind CSS. */
// Penjelasan: Memperbarui `module.exports` dengan menyiapkan objek sebagai wadah pasangan properti dan nilai yang diisi pada baris berikutnya.
module.exports = {
  // Penjelasan: Mengisi properti `content` pada objek atau konfigurasi dengan menyiapkan array sebagai wadah koleksi; elemen berikutnya akan mengisi urutan data; konsumen objek membaca nilai ini melalui nama properti tersebut.
  content: [
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"./Views/** /*.cshtml"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "./Views/**/*.cshtml",
    // Penjelasan: Memasok nilai argumen atau anggota koleksi dengan menggunakan literal `"./wwwroot/js/** /*.js"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; posisi baris ini menentukan parameter atau urutan elemen.
    "./wwwroot/js/**/*.js"
  // Penjelasan: Menutup koleksi array yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  ],
  // Penjelasan: Mengisi properti `theme` pada objek atau konfigurasi dengan menyiapkan objek sebagai wadah pasangan properti dan nilai yang diisi pada baris berikutnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
  theme: {
    // Penjelasan: Mengisi properti `extend` pada objek atau konfigurasi dengan menyiapkan objek sebagai wadah pasangan properti dan nilai yang diisi pada baris berikutnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
    extend: {}
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  },
  // Penjelasan: Mengisi properti `plugins` pada objek atau konfigurasi dengan menyiapkan array sebagai wadah koleksi; elemen berikutnya akan mengisi urutan data; konsumen objek membaca nilai ini melalui nama properti tersebut.
  plugins: []
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
};
