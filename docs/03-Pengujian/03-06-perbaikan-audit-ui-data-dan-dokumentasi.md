# Perbaikan audit UI, akses data, dan dokumentasi

Tanggal: 13 September 2026. Cakupan: perubahan kode lokal setelah baseline `ec959af`; bukti deployment baseline terdahulu tetap berada pada [kesiapan produksi](03-05-kesiapan-produksi.md). Dokumen ini tidak menyatakan bahwa perubahan baru sudah dideploy.

Putaran ini dilanjutkan oleh [perbaikan audit menyeluruh, 16 September 2026](03-07-perbaikan-audit-menyeluruh.md). Hasil di bawah tetap menjadi catatan pengujian pada tanggalnya.

| Perubahan | Alasan |
|---|---|
| Log audit hanya untuk akun pemanggil; filter akun lain ditolak | Mencegah instruktur melihat IP dan riwayat keamanan pihak lain |
| JWT memeriksa akun aktif dan peran terkini | Penonaktifan/perubahan peran langsung berlaku pada permintaan API berikutnya |
| Halaman galat 403/404/503 dengan penjelasan dan navigasi | Pengguna tidak terjebak halaman kosong; status HTTP tetap sesuai |
| Batas nama ruleset 120 pada UI/API dan petunjuk kata sandi | Input yang diterima sesuai batas database dan kebijakan autentikasi |
| Donasi belum lengkap disembunyikan pada seluruh analitika dan transaksi | Nominal tidak dapat ditebak melalui total kas atau metrik turunan |
| Banner analitika sementara saat donasi belum lengkap | Nilai yang ditunda tidak disalahartikan sebagai hasil akhir atau tidak pernah berdonasi |
| Skor akhir dan status/state sesi satu transaksi dengan lock ingest | Event bersamaan tidak terlewat dari hasil akhir; kegagalan tidak meninggalkan hasil setengah tersimpan |
| Guard payload JSON sebelum pengayaan | Payload salah mendapat kesalahan validasi, bukan exception 500 |
| Timeline melanjutkan cursor awal dan memperbarui entri SEALED | Aktivitas baru segera diambil dan donasi terbuka tanpa reload seluruh riwayat |
| Pembacaan status donasi memakai agregasi database | Setiap halaman event tidak lagi mentransfer seluruh payload riwayat |
| Manual, UML, kontrak, skenario seed, dan indeks disinkronkan | Dokumentasi mencerminkan fitur dan hak akses aktual |

Seed 2 tetap tersedia dan tidak dihapus. Perubahan ini tidak mengganti akun, ruleset, sesi, atau data non-demo. Ketentuan registrasi publik instruktur tetap mengikuti konfigurasi yang sudah ada.

## Verifikasi

Pengujian dilakukan pada kode lokal, API `localhost:5041`, dan UI `localhost:5203`:

| Pemeriksaan | Hasil |
|---|---|
| Seluruh pengujian API | 383 lulus, 0 gagal, 0 dilewati |
| Seluruh pengujian unit UI | 406 lulus, 0 gagal, 0 dilewati |
| Regresi browser audit, desktop dan ponsel | 8 lulus |
| Regresi rilis dan umpan balik UI, desktop dan ponsel | 46 kasus lulus: 42 pada pemeriksaan awal dan 4 setelah restart API lokal |
| Kontrak dokumentasi dan tautan lokal | 26 berkas Markdown lulus |
| Pemeriksaan whitespace diff Git | Lulus |

Kasus regresi mencakup isolasi audit, pencabutan akses akun, payload tidak valid, batas nama ruleset, donasi sebelum/sesudah lengkap, refresh event lama, dan penutupan bersamaan dengan event. Tampilan halaman galat dan petunjuk kata sandi juga diperiksa secara visual pada ponsel.

Perintah yang digunakan: `dotnet test` untuk kedua proyek pengujian .NET, `npx playwright test specs/audit-fixes.spec.js`, `npx playwright test specs/release-gate.spec.js specs/ui-feedback.spec.js`, `./scripts/Test-DocumentationConsistency.ps1`, dan `git diff --check`.

Empat kasus penyimpanan ruleset sempat gagal karena proses `dotnet watch` lokal masih memuat tipe lama setelah hot reload (`TypeLoadException`). Setelah API dan UI lokal dimulai ulang, keempat kasus diuji kembali dengan `--grep 'edit ruleset'` dan lulus. Empat ruleset sementara dari percobaan gagal dibersihkan berdasarkan nama persis yang dibuat oleh pengujian; seed dan ruleset pengguna tidak ikut dihapus.

Pengulangan regresi audit setelah restart juga memicu HTTP 429 pada satu langkah login karena banyak login dari rangkaian uji sebelumnya. Kasus tersebut diulang setelah jendela pembatasan berakhir; konfigurasi pembatasan login tetap dipertahankan.

Log serta tangkapan layar pengujian lokal berada di `artifacts/audit-*`; artefak tersebut diabaikan Git. Hasil ini memverifikasi kasus yang diuji, bukan jaminan tidak ada bug pada semua kemungkinan penggunaan.
