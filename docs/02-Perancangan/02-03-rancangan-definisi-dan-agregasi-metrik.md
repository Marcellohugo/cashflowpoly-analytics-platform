# Rancangan Definisi dan Agregasi Metrik
## Sistem Informasi Dasbor Analitika & Manajemen Ruleset Cashflowpoly

### Informasi Dokumen
- **Nama Dokumen**: Rancangan Definisi dan Agregasi Metrik
- **Versi**: 1.6
- **Tanggal**: 11 Juli 2026
- **Penyusun**: Marco Marcello Hugo

---

## 1. Tujuan Dokumen
Dokumen ini mendefinisikan seluruh metrik analitika yang dihitung dari log event permainan Cashflowpoly. Dokumen ini menggabungkan definisi metrik minimum dasbor, variabel fisik yang teramati (*observable variables*), serta rumus-rumus metrik turunan untuk evaluasi literasi keuangan pemain. Dokumen ini menjadi rujukan tunggal implementasi modul analitika backend dan visualisasi grafik pada frontend MVC.

---

## 2. Prinsip Perhitungan Metrik

### 2.1 Event sebagai Input Tunggal
Metrik dihitung murni dari data tabel `events` dan tabel proyeksi transaksi `event_cashflow_projections`. Sistem tidak menerima isian nilai metrik apa pun secara langsung dari klien demi mencegah manipulasi data.

### 2.2 Traceability (Ketertelusuran)
Setiap nilai metrik yang disimpan harus memiliki riwayat asal-usul yang jelas, meliputi:
1. Jenis event pemicu (*source events*).
2. Rumus dan formula perhitungan.
3. Aturan pembagian jendela waktu dan level agregasi.
4. Metode pembuktian validasi kesesuaian nilai di database.

### 2.3 Konteks Versi Ruleset
Semua hasil metrik disimpan pada tabel `metric_snapshots` dengan mencantumkan `ruleset_version_id` yang sedang aktif saat sesi permainan berjalan. Hal ini memungkinkan Instruktur membandingkan performa antarkelompok dengan versi aturan yang berbeda.

### 2.4 Tipe Data
- Nilai kuantitas koin dan jumlah kartu menggunakan tipe data `integer`.
- Nilai rasio, skor indeks, dan persentase menggunakan tipe data `double`.
- Metrik kompleks (seperti kumpulan variabel fisik dan metrik turunan) disimpan dalam bentuk dokumen JSON terstruktur pada kolom `metric_payload_json`.

---

## 3. Tingkat Agregasi & Jendela Waktu

### 3.1 Level Agregasi
Metrik dihitung dan disajikan pada dua level:
1. **Level Sesi (Agregat Sesi)**: Mengakumulasi data seluruh pemain dalam satu sesi (disimpan dengan `user_id = null` dan `session_player_id = null`).
2. **Level Pemain (Individual)**: Khusus untuk satu peserta sesi (disimpan dengan `user_id = <uuid app_users>` dan `session_player_id = <uuid session_participants>`).

### 3.2 Jendela Waktu (Time Windows)
Sistem melacak metrik berdasarkan:
- **Lifetime Sesi**: Akumulasi total dari awal hingga akhir sesi permainan.
- **Harian (`day_index`)**: Tren perkembangan data dari hari ke hari (Senin s.d. Minggu).
- **Slot Aksi (`action_slot`)**: Poin data per giliran aksi pemain.

### 3.3 Indikator Kinerja Utama (KPI Final Performa)
Sistem menghitung empat skor kinerja utama (skala 0-100) sebagai basis analitika web:

1.  **`learning.performance.individual.score` (Pemain)**
2.  **`learning.performance.aggregate.score` (Sesi)**
3.  **`mission.performance.individual.score` (Pemain)**
4.  **`mission.performance.aggregate.score` (Sesi)**

#### Formula Komponen Skor:
-   `cashflow_component` = $\text{clamp}(50 + 5 \times \text{cashflow.net.total}, 0, 100)$
-   `compliance_component` = $\text{compliance.primary_need.rate} \times 100$
-   `happiness_component` = $\text{clamp}(5 \times \text{happiness.points.total}, 0, 100)$
-   `mission_penalty_component` = $\text{clamp}(\text{happiness.mission.penalty} + \text{happiness.loan.penalty}, 0, 100)$

#### Rumus KPI Kinerja:
-   **Skor Pembelajaran Individu**:
    $$\text{learning.performance.individual.score} = (0.40 \times \text{cashflow\_component}) + (0.35 \times \text{compliance\_component}) + (0.25 \times \text{happiness\_component})$$
-   **Skor Kinerja Misi Individu**:
    $$\text{mission.performance.individual.score} = \text{clamp}(100 - \text{mission\_penalty\_component}, 0, 100)$$

#### Agregasi Sesi:
-   **Skor Pembelajaran Sesi**: Rata-rata dari skor pembelajaran individual seluruh peserta sesi.
-   **Skor Kinerja Misi Sesi**: Rata-rata dari skor kinerja misi individual seluruh peserta sesi.

> [!NOTE]
> Jika data komponen belum lengkap (misal pada hari pertama), skor individual akan dihitung secara proporsional dari komponen yang tersedia. Jika semua komponen null, skor diisi `null` (di UI dirender sebagai `N/A`).

---

## 4. Daftar Metrik Minimum Dashboard

Sistem melacak metrik inti berikut untuk penyajian data antarmuka dasbor:

| Kode Metrik | Level | Tipe Data | Sumber Data Utama | Deskripsi |
|---|---|---|---|---|
| `cashflow.in.total` | Sesi & Pemain | Numeric | `event_cashflow_projections` | Total koin masuk. |
| `cashflow.out.total` | Sesi & Pemain | Numeric | `event_cashflow_projections` | Total koin keluar. |
| `cashflow.net.total` | Sesi & Pemain | Numeric | Formula | Selisih koin masuk dikurangi koin keluar. |
| `donation.total` | Sesi & Pemain | Numeric | Event `JumatBerkah` | Akumulasi nilai koin yang didonasikan. |
| `gold.qty.current` | Pemain | Numeric | Event `InvestasiEmas`/`JualEmas` | Jumlah lembar kartu emas yang dipegang. |
| `orders.completed.count` | Pemain | Numeric | Event `JualMasakan` | Jumlah pesanan yang berhasil dipenuhi. |
| `inventory.ingredient.total` | Pemain | Numeric | Proyeksi inventory | Jumlah kartu bahan makanan di tangan. |
| `compliance.primary_need.rate` | Pemain | Numeric | Event `Kebutuhan` + Ruleset | Rasio hari pemenuhan kebutuhan primer. |
| `actions.used.total` | Pemain | Numeric | Event `events` | Jumlah token aksi yang telah digunakan. |
| `rules.violations.count` | Sesi & Pemain | Numeric internal | `validation_logs` | Jumlah upaya event yang ditolak validasi; dipakai untuk audit/risk assessment dan tidak ditampilkan sebagai kartu statistik utama. |
| `happiness.points.total` | Sesi & Pemain | Numeric | Agregasi Poin Kebahagiaan | Total skor kebahagiaan akhir pemain. |
| `happiness.need.points` | Pemain | Numeric | Event `Kebutuhan` | Skor kebahagiaan dari kartu kebutuhan. |
| `happiness.need.bonus` | Pemain | Numeric | Rumus kombinasi | Bonus dari set kartu kebutuhan (kombinasi). |
| `happiness.donation.points` | Pemain | Numeric | Peringkat Donasi | Poin bonus dari peringkat donasi hari Jumat. |
| `happiness.gold.points` | Pemain | Numeric | Kepemilikan Emas | Poin dari kepemilikan kartu emas akhir game. |
| `happiness.pension.points` | Pemain | Numeric | Peringkat Pensiun | Poin bonus peringkat sisa koin pensiun. |
| `happiness.saving_goal.points` | Pemain | Numeric | Event `TujuanFinansial` | Poin dari penyelesaian tujuan keuangan. |
| `happiness.mission.penalty` | Pemain | Numeric | Event Setup Misi | Penalti poin jika gagal memenuhi kartu Misi Koleksi. |
| `happiness.loan.penalty` | Pemain | Numeric | Event `PinjamanSyariah` | Penalti poin jika ada utang belum lunas di akhir game. |
| `loan.unpaid.flag` | Pemain | Numeric | State Pinjaman | Status apakah masih memiliki pinjaman aktif (1/0). |

---

## 5. Definisi Detail dan Rumus Perhitungan

### 5.1 `compliance.primary_need.rate` (Kepatuhan Kebutuhan Primer)
-   **Tujuan**: Menghitung kedisiplinan pemain dalam mengutamakan kebutuhan primer sebelum kebutuhan lain.
-   **Rumus**:
    $$\text{Rasio Kepatuhan} = \frac{\text{Jumlah Hari Patuh}}{\text{Total Hari yang Dievaluasi}}$$
-   **Kriteria Hari Patuh**:
    1.  Tidak membeli kebutuhan sekunder/tersier sebelum pemain pernah membeli kebutuhan primer pada sesi tersebut.
    2.  Status pemenuhan primer dibawa ke hari berikutnya; pemain tidak wajib membeli primer ulang setiap hari.
    3.  Jumlah pembelian primer pada hari tersebut tidak melampaui batas maksimal harian ruleset.

### 5.2 `happiness.need.bonus` (Bonus Set Kebutuhan)
-   **Tujuan**: Memberikan poin tambahan bagi pemain yang memiliki variasi kebutuhan seimbang (*diversity*).
-   **Rumus**:
    $$\text{mixed\_sets} = \min(\text{primary\_count}, \text{secondary\_count}, \text{tertiary\_count})$$
    $$\text{same\_sets} = \sum_{t \in \{\text{pri, sec, ter}\}} \lfloor\frac{t\_count - \text{mixed\_sets}}{3}\rfloor$$
    $$\text{need\_bonus\_points} = (\text{mixed\_sets} \times 4) + (\text{same\_sets} \times 2)$$

---

## 6. Variabel Gameplay Fisik (Konseptual)
Disimpan dalam payload JSON `gameplay.raw.variables` untuk analitika detail:
-   `starting_coins`: Uang koin awal yang dibagikan pada setup sesi.
-   `coins_held_current`: Jumlah koin di tangan pemain saat ini.
-   `ingredients_held_current`: Jumlah kartu bahan makanan yang dipegang.
-   `ingredients_wasted`: Jumlah bahan masakan yang kedaluwarsa/dibuang.
-   `meal_orders_available_passed`: Jumlah pesanan masakan yang dilewati/diabaikan.
-   `gold_cards_held_end`: Jumlah kartu emas yang dimiliki saat sesi berakhir.
-   `life_risk_cards_drawn`: Jumlah kartu risiko kehidupan yang diambil oleh pemain (Mode Mahir).
-   `sharia_loans_taken`: Frekuensi pengambilan pinjaman syariah (Mode Mahir).
-   `sharia_loans_unpaid_end`: Jumlah pinjaman syariah yang belum dilunasi di akhir sesi.

---

## 7. Metrik Analitika Turunan
Disimpan dalam payload JSON `gameplay.derived.metrics` untuk visualisasi performa tingkat lanjut:

-   **Net Worth Index**: Mengukur pertumbuhan koin bersih dibanding modal awal.
    $$\text{net\_worth\_index} = \frac{\text{coins\_net\_end\_game}}{\text{starting\_coins}} \times 100\%$$
-   **Business Profit Margin**: Tingkat profitabilitas penjualan masakan pemain.
    $$\text{business\_profit\_margin} = \frac{\text{meal\_income} - \text{ingredient\_costs}}{\text{meal\_income}} \times 100\%$$
-   **Risk Mitigation Effectiveness (Mode Mahir)**: Efektivitas asuransi dalam melindungi dari risiko negatif.
    $$\text{risk\_mitigation\_effectiveness} = \frac{\text{risiko\_terlindungi\_asuransi}}{\text{total\_kartu\_risiko\_negatif}} \times 100\%$$
-   **Debt Leverage Ratio (Mode Mahir)**: Rasio utang outstanding dibanding total kekayaan bersih.
    $$\text{debt\_leverage} = \frac{\text{sharia\_loans\_outstanding}}{\text{net\_worth}} \times 100\%$$
-   **Planning Horizon**: Mengukur fokus jangka panjang pemain melalui investasi (tabungan, emas, dan asuransi).
    $$\text{planning\_horizon} = \frac{\text{tabungan\_investasi} + \text{emas\_dibeli} + \text{premi\_asuransi}}{\text{total\_aksi\_digunakan}}$$

---

## 8. Pemetaan Event ke Metrik

| Jenis Event | Metrik yang Diperbarui |
|---|---|
| `CatatTransaksi` | `cashflow.in.total`, `cashflow.out.total`, `cashflow.net.total` |
| `JumatBerkah` | `donation.total`, `cashflow.out.total` |
| `InvestasiEmas` | `gold.qty.current`, `cashflow.out.total` |
| `JualEmas` | `gold.qty.current`, `cashflow.in.total` |
| `BahanMasakan` | `inventory.ingredient.total`, `cashflow.out.total` |
| `JualMasakan` | `orders.completed.count`, `inventory.ingredient.total`, `cashflow.in.total` |
| `Kebutuhan` | `compliance.primary_need.rate`, `happiness.need.points`, `cashflow.out.total` |
| `PinjamanSyariah` | `loan.unpaid.flag`, `happiness.loan.penalty`, `cashflow.in.total` |
| `BayarPinjaman` | `loan.unpaid.flag`, `happiness.loan.penalty`, `cashflow.out.total` |
| `TujuanFinansial` | `happiness.saving_goal.points`, `cashflow.out.total` |

---

## 9. Aturan Penyimpanan & Batasan Kalkulasi

### 9.1 Penanganan Pembagian dengan Nol (Division by Zero)
Jika penyebut dalam perhitungan rasio bernilai `0` (seperti saat total koin keluar/masuk bernilai nol), sistem akan mengisi metrik tersebut dengan nilai `null` (bukan `0`) dan frontend akan merendernya sebagai `N/A`.

### 9.2 Guardrail Batasan Nilai
-   Jumlah emas, koin, dan bahan makanan pada projection tidak boleh bernilai negatif. Jika kalkulasi event menghasilkan nilai negatif, event tersebut wajib ditolak di layer validasi API.
-   Setiap metrik rasio yang dibatasi rentang (seperti rasio kepatuhan) harus di-*clamp* secara ketat antara rentang `0.0` sampai `1.0` (atau `0` s.d. `100`).
