# Rancangan Definisi dan Agregasi Metrik
## Sistem Informasi Dasbor Analitika Cashflowpoly

### Informasi Dokumen

- Nama dokumen: Rancangan Definisi dan Agregasi Metrik
- Versi: 2.0
- Tanggal: 26 Agustus 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan dan prinsip

Dokumen ini menjadi rujukan kanonik analitik pemain. Semua hasil berasal dari setup fisik yang dikonfirmasi, event gameplay sah, katalog ruleset, dan proyeksi transaksi. Klien tidak dapat mengirim nilai metrik jadi.

Prinsip wajib:

1. Nama variabel sama pada backend, API, UI, rumus, Postman, dan dokumentasi.
2. Setiap metrik dapat ditelusuri ke variabel serta event/projection sumber.
3. Event yang ditolak tidak menjadi event gameplay, snapshot, atau metrik.
4. Event legacy terkait aksi lewati dan deck/pasar tetap berada di histori, tetapi diabaikan dalam rekalkulasi baru.
5. Data belum cukup atau pembagi nol menghasilkan `null`, bukan nol palsu.
6. Metrik khusus Mahir tidak dibentuk dan tidak ditampilkan pada sesi Pemula.

## 2. Sumber data

| Sumber | Fungsi analitik |
|---|---|
| `session_setup_revisions` | Revisi setup terbaru yang dikunci saat start: Tie Breaker, bahan, emas, misi, pinjaman, dan proteksi awal. |
| `events` | Keputusan gameplay sah dan urut berdasarkan `sequence_number`. |
| `event_asset_references` | Provenance aset katalog yang digunakan event. |
| `event_cashflow_projections` | Arus kas masuk/keluar dan kategori transaksi. |
| Projection peserta | Saldo, inventory, kebutuhan, emas, pinjaman, asuransi, tujuan, dan skor saat ini. |
| `ruleset_*` | Nilai tetap, mode, biaya, poin, serta batas yang berlaku pada versi sesi. |
| `metric_snapshots` | Cache `gameplay.raw.variables` dan `gameplay.derived.metrics` yang dapat dibangun ulang. |

Backend tidak mengetahui urutan deck, isi pasar, refill, atau kartu terbuka yang tidak dilaporkan IDN.

## 3. Tingkat agregasi

- Level pemain: memakai `user_id` dan `session_player_id`.
- Level sesi: agregasi seluruh pemain dengan identitas pemain null.
- Lifetime sesi: sejak setup sampai event terakhir.
- Harian: dikelompokkan memakai `day_index`.
- Urutan kejadian: memakai `sequence_number`; bukan waktu klien semata.

Analitik historis dibangun ulang dengan versi ruleset yang terkunci pada sesi. Perubahan rumus dapat mengubah hasil historis tanpa mengubah event sumber.

## 4. Variabel Permainan Fisik

Payload `raw_json` pada endpoint gameplay mengelompokkan variabel menjadi sebelas kelompok berikut.

Dasbor menyajikan data tersebut dalam sepuluh kelompok antarmuka. Kelompok `turns` tidak dihilangkan; isinya digabungkan ke kelompok Koin dan Keuangan serta Penggunaan Aksi agar penyajian tidak redundan. Seluruh perhitungan indikator domain tetap dilakukan oleh API, sedangkan UI memetakan DTO, menggabungkan data hanya untuk kebutuhan penyajian, dan memformat tampilannya.

| Kelompok | Contoh variabel | Asal data |
|---|---|---|
| `coins` | `starting_coins`, `coins_held_current`, `coins_net_end_game`, `total_cash_in`, `total_cash_out`, sumber pendapatan | Setup, ruleset, proyeksi arus kas |
| `ingredients` | bahan terkumpul, biaya investasi, bahan terpakai, bahan dibuang | Setup, event bahan/pesanan, inventory |
| `meal_orders` | pesanan selesai, pemasukan pesanan, biaya bahan terpakai | Event `JualMasakan` dan asset reference |
| `needs` | jumlah Primer/Sekunder/Tersier dan poin kebutuhan | Event `Kebutuhan` dan projection kebutuhan |
| `donations` | nominal per Jumat, total donasi, partisipasi | Event `JumatBerkah` dan projection donasi |
| `gold` | jumlah emas, pembelian, penjualan, nilai/poin akhir | Setup serta event emas |
| `pension` | tabungan, nilai bahan akhir, dana pensiun, peringkat, poin | Projection saldo/inventory dan skor akhir |
| `life_risk` | risiko muncul, cara penyelesaian, penggunaan opsi darurat | Event risiko/asuransi/pinjaman; Mahir saja |
| `financial_goals` | target dicoba/selesai dan koin yang dialokasikan | Event tabungan/tujuan; Mahir saja |
| `actions` | aksi utama, aksi penghasil uang, aksi jangka panjang | Event pemain dengan slot aksi utama |
| `turns` | progres hari/giliran, perubahan kas, titik risiko/utang pertama | Event dan state; field Mahir dihilangkan pada Pemula |

Field bernilai nol tetap dikirim bila nol merupakan hasil yang sah. Field yang tidak dapat diamati atau tidak berlaku pada mode tidak dibuat sebagai nol buatan.

## 5. Metrik Turunan Baku

Payload `derived_json` berisi tiga belas metrik analisis berikut serta rincian komposisi poin kebahagiaan.

| Nama UI | Key API | Rumus final |
|---|---|---|
| Perubahan Koin dari Awal | `cash_growth_percent` | UI: `(coins_net_end_game − starting_coins) ÷ starting_coins × 100%`, dengan tanda `+` untuk kenaikan. Nilai API tetap berupa rasio `coins_net_end_game ÷ starting_coins × 100` demi kompatibilitas; UI menguranginya dengan 100. |
| Diversifikasi Pendapatan | `income_diversification_index` | indeks konsentrasi ternormalisasi dari pendapatan kerja lepas, pesanan, dan penjualan emas; pinjaman serta transfer tabungan dikecualikan |
| Porsi Biaya Usaha | `business_expense_share_percent` | `ingredient_investment_coins_total ÷ total_cash_out × 100%` |
| Margin Usaha Pesanan | `meal_order_profit_margin_percent` | `(meal_order_income_total - ingredient_cost_used) ÷ meal_order_income_total × 100%` |
| Kesiapan Menghadapi Risiko | `risk_readiness_percent` | `risks_resolved_without_emergency ÷ life_risk_cards_drawn × 100%` (Mahir) |
| Beban Pinjaman | `loan_burden_percent` | `outstanding_loan ÷ (outstanding_loan + liquid_assets) × 100%` (Mahir) |
| Progres Target Finansial (API) | `financial_goal_progress_percent` | `coins_committed_to_goals ÷ attempted_goal_target_total × 100%` (Mahir); kartu analisis UI memakai jumlah pembelian selesai seperti dijelaskan di bawah |
| Fokus Aksi Penghasil Uang | `income_action_focus_percent` | `income_main_actions ÷ total_main_actions × 100%` |
| Pemanfaatan Bahan | `ingredient_utilization_percent` | `ingredients_used_in_completed_orders ÷ ingredients_collected × 100%` |
| Porsi Aksi Jangka Panjang | `long_term_action_share_percent` | `(saving_actions + financial_goal_actions + insurance_actions + loan_repayment_actions) ÷ total_main_actions × 100%` (Mahir) |
| Keragaman Pemenuhan Kebutuhan | `need_fulfillment_diversity_percent` | indeks keragaman ternormalisasi kategori Primer/Sekunder/Tersier |
| Komitmen Donasi | `donation_commitment_score` | `donation_stability_index × donated_resource_share × friday_participation_rate`, dibatasi 0-100 |
| Pemerataan Sumber Poin Kebahagiaan | `happiness_source_diversity_percent` | `[1 − Σ(poin_sumber ÷ total_poin_positif)²] ÷ (1 − 1/N) × 100%`; lima sumber pada Pemula, enam pada Mahir |
| Komposisi Poin Kebahagiaan | `happiness_points_composition` | jumlah aktual seluruh komponen poin dan penalti sesuai mode |

Pemerataan sumber poin kebahagiaan memakai kartu kebutuhan, bonus set kebutuhan, donasi, emas, pensiun, serta target finansial pada Mahir. Sumber bernilai nol tetap termasuk dalam jumlah sumber `N`. Penalti misi dan pinjaman tidak menjadi sumber pemerataan; keduanya tetap mengurangi total poin kebahagiaan. Nilai 0% berarti hanya satu sumber yang menghasilkan poin kebahagiaan, sedangkan 100% berarti semua sumber terbagi sama rata. Jika tidak ada poin kebahagiaan positif, hasilnya `null`.

Kartu analisis target finansial pada UI menampilkan **Target Finansial Berhasil Dibeli** dari `financial_goals_completed`. **Total Biaya Pembelian Target Finansial** (`financial_goals_purchase_cost_total`) menjumlahkan biaya pada catatan pembelian target; biaya ini tetap tercatat setelah tabungan dipakai membayar kartu. **Tabungan untuk Target Belum Dibeli** (`financial_goals_incomplete_coins_wasted`) ditampilkan terpisah dan hanya mencakup saldo target yang pembeliannya belum selesai. Contoh: satu target seharga 35 koin sudah dibeli dan 5 koin disimpan untuk target berikutnya menghasilkan 1 target, biaya pembelian 35 koin, dan tabungan target belum dibeli 5 koin. Metrik persentase pendanaan tetap tersedia di API untuk kompatibilitas.

Pada rincian penggunaan aksi, `saving_and_goal_actions` menggabungkan `saving_actions` dan `financial_goal_actions` menjadi **Aksi untuk Tabungan dan Pembelian Target**. Pembelian otomatis oleh sistem tetap menambah jumlah target berhasil dibeli, tetapi tidak menambah aksi di luar kegiatan menabung. UI memakai rincian gabungan ini agar tidak menampilkan nol aksi target di samping target yang sudah dibeli. Contoh: empat kali menabung yang menghasilkan satu pembelian target, satu pembelian asuransi, dan satu pelunasan pinjaman menggunakan `(4 + 1 + 1) ÷ 32 × 100% = 18,75%` dari 32 aksi yang dipakai. Kedua komponen terpisah tetap tersedia di API untuk kompatibilitas.

Untuk mencegah angka yang berbeda konteks terlihat bertentangan:

- `income_main_actions` hanya menghitung aksi kerja lepas dan penjualan masakan yang menghasilkan transaksi masuk. Pinjaman dan penarikan tabungan memakai aksi tetapi bukan pendapatan baru; penjualan emas menghasilkan pendapatan tanpa memakai aksi. Semuanya tetap masuk riwayat transaksi sesuai arus koin masing-masing.
- `insurance_actions` adalah aksi untuk **mengaktifkan** asuransi melalui pembayaran premi. Perlindungan awal gratis dan klaim tidak menambah aksi, sehingga jumlah risiko yang ditanggung dapat lebih besar daripada aksi atau premi yang dibayar.
- Target yang sudah dibeli tetap tercatat sebagai pembelian. Poin kebahagiaannya tidak dihitung selama ada pinjaman belum lunas; UI menjelaskan kondisi ini pada kartu pembelian target.
- `specific_tertiary_need` dan misi koleksi memakai riwayat pembelian. Kartu yang kemudian dijual tidak lagi masuk kepemilikan maupun pemerataan kartu saat ini, tetapi pembeliannya tetap memenuhi syarat misi.
- Harga emas per transaksi pada daftar harga adalah **harga per kartu**, sedangkan arus kas pembelian/penjualan menjumlahkan harga dikali jumlah kartu.
- Risiko yang belum diselesaikan tidak otomatis berarti memakai tindakan darurat. Panduan 100% hanya digunakan ketika seluruh risiko tercatat selesai tanpa tindakan darurat.
- Pemerataan satu sumber pendapatan adalah 0% menurut definisi khusus. Rincian hitung UI tidak menampilkan rumus normalisasi yang membagi dengan nol untuk kasus ini.
- Peringkat dana pensiun berasal dari hasil peringkat atau catatan pemberiannya. Poin kebahagiaan saja tidak dipakai untuk menebak peringkat karena beberapa peringkat dapat memperoleh nilai sama, termasuk nol.

## 6. Bukti perhitungan pada API dan UI

Metrik numerik memakai objek `*_components` yang sesuai dengan variabel rumus; pemerataan sumber poin kebahagiaan memakai rincian `happiness_points_composition`. Bagian **Lihat angka pembentuk dan rumus/Lihat cara menghitung** menampilkan secara berurutan:

1. asal data;
2. variabel pembentuk;
3. angka aktual;
4. substitusi rumus;
5. hasil akhir.

Contoh Perubahan Koin dari Awal:

```text
Asal data: setup awal + saldo/proyeksi akhir pemain
coins_net_end_game = 15 koin
starting_coins = 10 koin
(15 − 10) ÷ 10 × 100% = +50%
```

Pada contoh tersebut, API tetap mengirim `cash_growth_percent=150`, lalu UI menampilkan `+50%`. Jika `starting_coins=0`, hasil adalah `null` dan UI menampilkan **Belum dapat dihitung**. UI menampilkan `0%` saat koin tersisa sama dengan koin awal.

## 7. Ketentuan mode

Mode Pemula tidak membentuk:

- kelompok `life_risk` dan `financial_goals`;
- field utang/risiko Mahir pada `turns`;
- `risk_readiness_percent`;
- `loan_burden_percent`;
- `financial_goal_progress_percent`;
- `long_term_action_share_percent`.

Kelompok, kartu, badge, label, dan rumus Mahir harus tidak ada pada DOM halaman Pemula, bukan sekadar disembunyikan dengan CSS.

## 8. Rekalkulasi dan konsistensi

Mode aplikasi `--recalculate-analytics` membangun ulang snapshot seluruh sesi dari event dan ruleset yang tersimpan. Rekalkulasi:

- tidak mengubah event sumber;
- mengabaikan event legacy yang tidak lagi sah;
- tidak memakai `validation_logs` sebagai input domain;
- menghasilkan output deterministik untuk dataset yang sama;
- menulis snapshot terbaru dengan hasil formula kanonis; snapshot lama tetap menjadi jejak historis dan tidak dipilih sebagai nilai terbaru.

Nama variabel baru tidak boleh ditambahkan hanya di UI. Perubahan formula harus memperbarui builder backend, DTO/JSON, test, README API, dokumen ini, dan Postman pada commit yang sama.
