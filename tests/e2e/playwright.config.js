// Fungsi file: Menjalankan pemeriksaan UI utama pada Chromium desktop dan ponsel.
// Penjelasan: Melakukan operasi dengan memuat modul CommonJS `"@playwright/test"` agar fungsi atau konfigurasi modul dapat digunakan.
const { defineConfig, devices } = require("@playwright/test");

// Penjelasan: Memperbarui `module.exports` dengan memanggil `defineConfig({` dan menggunakan hasilnya pada operasi ini; argumen memasok data yang dibutuhkan fungsi.
module.exports = defineConfig({
  // Penjelasan: Mengisi properti `testDir` pada objek atau konfigurasi dengan menggunakan literal `"./specs"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
  testDir: "./specs",
  // Penjelasan: Mengisi properti `timeout` pada objek atau konfigurasi dengan membaca nilai `30_000` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
  timeout: 30_000,
  // Penjelasan: Mengisi properti `expect` pada objek atau konfigurasi dengan membaca nilai `{ timeout: 8_000 }` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
  expect: { timeout: 8_000 },
  // Penjelasan: Mengisi properti `fullyParallel` pada objek atau konfigurasi dengan menonaktifkan flag Boolean dengan nilai false; konsumen objek membaca nilai ini melalui nama properti tersebut.
  fullyParallel: false,
  // Penjelasan: Mengisi properti `retries` pada objek atau konfigurasi dengan menggunakan konstanta numerik `0` sebagai nilai awal atau parameter perhitungan; konsumen objek membaca nilai ini melalui nama properti tersebut.
  retries: 0,
  // Penjelasan: Mengisi properti `reporter` pada objek atau konfigurasi dengan menggunakan literal `"line"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
  reporter: "line",
  // Penjelasan: Mengisi properti `use` pada objek atau konfigurasi dengan menyiapkan objek sebagai wadah pasangan properti dan nilai yang diisi pada baris berikutnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
  use: {
    // Penjelasan: Mengisi properti `baseURL` pada objek atau konfigurasi dengan menghitung ekspresi `process.env.E2E_BASE_URL || "http://localhost:5203"` dengan urutan operator untuk memperoleh nilai turunan dari data masukan; konsumen objek membaca nilai ini melalui nama properti tersebut.
    baseURL: process.env.E2E_BASE_URL || "http://localhost:5203",
    // Penjelasan: Mengisi properti `trace` pada objek atau konfigurasi dengan menggunakan literal `"retain-on-failure"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
    trace: "retain-on-failure",
    // Penjelasan: Mengisi properti `screenshot` pada objek atau konfigurasi dengan menggunakan literal `"only-on-failure"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
    screenshot: "only-on-failure"
  // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  },
  // Penjelasan: Mengisi properti `projects` pada objek atau konfigurasi dengan menyiapkan array sebagai wadah koleksi; elemen berikutnya akan mengisi urutan data; konsumen objek membaca nilai ini melalui nama properti tersebut.
  projects: [
    // Penjelasan: Membuka ruang lingkup blok untuk `fungsi interaksi halaman`; deklarasi lokal dan urutan langkah di dalamnya dikelompokkan di sini.
    {
      // Penjelasan: Mengisi properti `name` pada objek atau konfigurasi dengan menggunakan literal `"chromium-desktop"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
      name: "chromium-desktop",
      // Penjelasan: Mengisi properti `use` pada objek atau konfigurasi dengan membaca nilai `{ ...devices["Desktop Chrome"] }` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
      use: { ...devices["Desktop Chrome"] }
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    },
    // Penjelasan: Membuka ruang lingkup blok untuk `fungsi interaksi halaman`; deklarasi lokal dan urutan langkah di dalamnya dikelompokkan di sini.
    {
      // Penjelasan: Mengisi properti `name` pada objek atau konfigurasi dengan menggunakan literal `"chromium-mobile"` sebagai teks, kunci, warna, atau isi template sesuai tempat pemakaiannya; konsumen objek membaca nilai ini melalui nama properti tersebut.
      name: "chromium-mobile",
      // Penjelasan: Mengisi properti `use` pada objek atau konfigurasi dengan membaca nilai `{ ...devices["Pixel 7"] }` dari variabel, properti, atau ekspresi yang telah disiapkan sebelumnya; konsumen objek membaca nilai ini melalui nama properti tersebut.
      use: { ...devices["Pixel 7"] }
    // Penjelasan: Menutup blok atau objek yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
    }
  // Penjelasan: Menutup koleksi array yang sedang disusun sehingga ekspresi atau struktur induk dapat dilanjutkan.
  ]
// Penjelasan: Menutup blok atau objek yang sedang disusun; tanda titik koma mengakhiri pernyataan.
});
