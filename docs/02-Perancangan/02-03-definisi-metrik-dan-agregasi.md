# Definisi Metrik dan Aturan Agregasi
## Sistem Informasi Dasbor Analitika & Manajemen *Ruleset* Cashflowpoly

### Dokumen
- Nama dokumen: Definisi Metrik dan Aturan Agregasi
- Versi: 1.4
- Tanggal: 18 Juni 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan Dokumen
Dokumen ini disusun untuk mendefinisikan metrik yang sistem hitung dari log event, beserta aturan agregasi, frekuensi perhitungan, dan format penyimpanan pada tabel `metric_snapshots`. Dokumen ini menjadi acuan implementasi modul analitika dan validasi tampilan dasbor.

Dokumen pendamping yang berisi variabel gameplay fisik dan metrik turunan:
- `docs/02-Perancangan/02-04-metrik-gameplay-fisik-dan-turunan.md`

---

## 2. Prinsip Perhitungan Metrik
### 2.1 Event sebagai input tunggal
Sistem menghitung metrik dari event yang tersimpan pada tabel `events` dan proyeksi arus kas pada tabel `event_cashflow_projections`. Sistem tidak menerima nilai metrik dari klien.

### 2.2 Metrik harus dapat ditelusuri
Setiap metrik harus memiliki:
1. sumber event yang jelas,
2. rumus perhitungan,
3. aturan agregasi waktu,
4. cara validasi dengan data basis data.

### 2.3 Metrik mengikuti konteks *ruleset*
Sistem menyertakan `ruleset_version_id` pada setiap snapshot metrik agar instruktur dapat membandingkan hasil antar konfigurasi.

### 2.4 Satuan dan tipe data
- Semua nilai koin memakai `integer`.
- Semua rasio memakai `double`.
- Semua metrik kompleks memakai `jsonb` pada kolom `metric_value_json`.

---

## 3. Waktu dan Level Agregasi
### 3.1 Level agregasi
Sistem menghasilkan metrik pada dua level:
1. level sesi (agregat semua pemain) dengan `user_id = null` dan `session_player_id = null`,
2. level Player dengan `user_id = <uuid akun Player>` dan, bila tersedia, `session_player_id = <uuid peserta sesi>`.

### 3.2 Jendela waktu
Sistem menghitung metrik pada tiga jendela:
1. sepanjang sesi (*lifetime session*),
2. per hari (`day_index`),
3. per slot aksi (`action_slot`).

Sistem menyimpan hasil hari dan giliran pada `metric_value_json` agar dasbor menampilkan tren.

### 3.3 Frekuensi perhitungan
Sistem memakai strategi berikut:
1. Sistem hitung metrik ringan setiap kali sistem menerima event arus kas atau event kepemilikan.
2. Sistem hitung metrik kepatuhan aturan saat sistem menerima event yang terkait aturan.
3. Sistem hitung ringkasan sesi saat instruktur mengakhiri sesi atau saat instruktur menekan tombol “Recompute”.

### 3.4 KPI final performa (wajib untuk scope analitika web)
Sistem menetapkan empat KPI final berikut.

1. `learning.performance.individual.score` (0-100)
2. `learning.performance.aggregate.score` (0-100)
3. `mission.performance.individual.score` (0-100)
4. `mission.performance.aggregate.score` (0-100)

Definisi komponen:
- `cashflow_component` = `clamp(50 + 5 * cashflow.net.total, 0, 100)`
- `compliance_component` = `compliance.primary_need.rate * 100`
- `happiness_component` = `clamp(5 * happiness.points.total, 0, 100)`
- `mission_success_component` = `clamp(100 - 10 * loan.unpaid.flag - mission_penalty_component, 0, 100)`
- `mission_penalty_component` = `clamp(happiness.mission.penalty + happiness.loan.penalty, 0, 100)`

Rumus KPI:
- `learning.performance.individual.score = 0.40 * cashflow_component + 0.35 * compliance_component + 0.25 * happiness_component`
- `mission.performance.individual.score = clamp(100 - mission_penalty_component, 0, 100)`

Agregasi:
- `learning.performance.aggregate.score = average(learning.performance.individual.score) per sesi atau grup ruleset`
- `mission.performance.aggregate.score = average(mission.performance.individual.score) per sesi atau grup ruleset`

Satuan:
- semua KPI dalam skala 0-100 (numeric).

Aturan data tidak lengkap:
1. Jika komponen tidak tersedia, komponen tersebut diisi `null`.
2. Skor individual dihitung dari komponen non-null dengan normalisasi bobot aktif.
3. Jika seluruh komponen null, skor individual = `null`.
4. Skor agregat menghitung rata-rata hanya dari nilai individual non-null.
5. UI wajib menampilkan label `N/A` untuk skor `null`.

---

## 4. Konvensi Nama Metrik
Sistem menamai metrik dengan format:
`<domain>.<metric>.<scope>`

Contoh:
- `cashflow.in.total`
- `cashflow.out.total`
- `compliance.primary_need.rate`
- `inventory.ingredient.total`
- `rules.violations.count`

Sistem memakai nama yang sama untuk level sesi dan level Player. Sistem membedakan level melalui `user_id` dan `session_player_id`.

---

## 5. Daftar Metrik Minimum (untuk Dasbor)
Tabel berikut mendefinisikan metrik minimum yang dasbor tampilkan.

| Kode Metrik | Level | Tipe Nilai | Sumber Data | Ringkas Fungsi |
|---|---|---|---|---|
| cashflow.in.total | sesi & pemain | numeric | `event_cashflow_projections` | Total pemasukan koin. |
| cashflow.out.total | sesi & pemain | numeric | `event_cashflow_projections` | Total pengeluaran koin. |
| cashflow.net.total | sesi & pemain | numeric | hasil rumus | Selisih pemasukan dan pengeluaran. |
| donation.total | sesi & pemain | numeric | `events` (`JumatBerkah`) | Total donasi. |
| gold.qty.current | pemain | numeric | `events` (`InvestasiEmas`, `JualEmas`) | Jumlah emas saat ini. |
| orders.completed.count | pemain | numeric | `events` (`JualMasakan`) | Jumlah order yang berhasil diklaim. |
| inventory.ingredient.total | pemain | numeric | `events` (`BahanMasakan`, `JualMasakan`) | Total kartu bahan saat ini. |
| compliance.primary_need.rate | pemain | numeric | `events` + aturan ruleset | Rasio hari yang memenuhi kewajiban kebutuhan primer. |
| actions.used.total | pemain | numeric | `events.action_slot` dan `ruleset_game_settings.actions_per_turn` | Total slot aksi yang pemain pakai. |
| rules.violations.count | sesi & pemain | numeric | `validation_logs` atau hasil validasi | Jumlah pelanggaran aturan domain. |
| happiness.points.total | sesi & pemain | numeric | agregasi event poin | Total Poin Kebahagiaan pemain / sesi. |
| happiness.need.points | pemain | numeric | `events` (`Kebutuhan`) | Total poin kartu kebutuhan. |
| happiness.need.bonus | pemain | numeric | `events` (`Kebutuhan`) | Bonus set kebutuhan. |
| happiness.donation.points | pemain | numeric | `ruleset.scoring` atau `events` (`PoinPeringkatDonasi`) | Poin juara donasi. |
| happiness.gold.points | pemain | numeric | `ruleset.scoring` atau `events` (`PoinEmas`) | Poin investasi emas. |
| happiness.pension.points | pemain | numeric | `ruleset.scoring` atau `events` (`PoinPeringkatPensiun`) | Poin juara dana pensiun. |
| happiness.saving_goal.points | pemain | numeric | `events` (`TujuanFinansial`) | Poin tujuan keuangan. |
| happiness.mission.penalty | pemain | numeric | `events` (`BagikanMisiKoleksi`) | Penalti misi koleksi. |
| happiness.loan.penalty | pemain | numeric | `events` (`PinjamanSyariah`, `BayarPinjaman`) | Penalti pinjaman belum lunas. |
| loan.unpaid.flag | pemain | numeric | `events` (`PinjamanSyariah`, `BayarPinjaman`) | Indikator pinjaman belum lunas (1/0). |

Catatan:
- Sistem tetap dapat menghitung `cashflow.*` dari event langsung, namun sistem sebaiknya memakai proyeksi agar query cepat.

### 5.1 Metrik tambahan (snapshot JSON)
Sistem menyimpan snapshot variabel gameplay fisik dan metrik turunan pada metrik JSON berikut:

| Kode Metrik | Level | Tipe Nilai | Ringkas Fungsi |
|---|---|---|---|
| gameplay.raw.variables | pemain | jsonb | Variabel gameplay fisik (koin, bahan, kebutuhan, donasi, emas, tabungan, risiko). |
| gameplay.derived.metrics | pemain | jsonb | Metrik turunan (net worth index, efisiensi, ROI, risk appetite, dll). |

Catatan:
- Isi JSON mengikuti dokumen `docs/02-Perancangan/02-04-metrik-gameplay-fisik-dan-turunan.md`.

---

## 6. Definisi Detail dan Rumus Metrik

## 6.1 `cashflow.in.total`
**Definisi:** total pemasukan koin untuk sesi atau pemain.

**Sumber event:** baris `event_cashflow_projections` dengan `direction = IN`.

**Rumus:**
- level sesi: `sum(amount) untuk session_id`
- level Player: `sum(amount) untuk session_id dan user_id`

**Validasi:** sistem cocokkan total `amount` pada proyeksi.

---

## 6.2 `cashflow.out.total`
**Definisi:** total pengeluaran koin untuk sesi atau pemain.

**Sumber event:** `event_cashflow_projections` dengan `direction = OUT`.

**Rumus:** sama seperti pemasukan, namun memakai `OUT`.

---

## 6.3 `cashflow.net.total`
**Definisi:** selisih pemasukan dan pengeluaran koin.

**Rumus:**  
`cashflow.in.total - cashflow.out.total`

---

## 6.4 `donation.total`
**Definisi:** total koin yang pemain donasikan pada hari Jumat.

**Sumber event:** `events.action_type = JumatBerkah`

**Rumus:**
- level Player: `sum(payload.amount) per user_id`
- level sesi: `sum(payload.amount) semua pemain`

---

## 6.5 `gold.qty.current`
**Definisi:** jumlah emas yang pemain pegang saat ini.

**Sumber event:** `events.action_type in ('InvestasiEmas', 'JualEmas')`

**Aturan agregasi:**
- BUY menambah `qty`
- SELL mengurangi `qty`

**Rumus (pseudocode):**
- `qty_current = sum(BUY.qty) - sum(SELL.qty)`

**Validasi:**
- Nilai tidak boleh negatif.
- Sistem menolak SELL bila stok kurang saat validasi event.

---

## 6.6 `orders.completed.count`
**Definisi:** jumlah order yang pemain klaim.

**Sumber event:** `events.action_type = JualMasakan`

**Rumus:** `count(*) per user_id`

---

## 6.7 `inventory.ingredient.total`
**Definisi:** total kartu bahan yang pemain miliki saat ini.

**Sumber event:**
- menambah: `BahanMasakan`
- mengurangi: `JualMasakan` (karena klaim order mengonsumsi bahan)

**Aturan agregasi minimum:**
- Sistem menambah 1 untuk setiap `BahanMasakan`.
- Sistem mengurangi `len(required_ingredient_card_ids)` untuk setiap `JualMasakan`.

**Rumus (pseudocode):**
- `ingredient_total = count(BahanMasakan) - sum(len(JualMasakan.required_ingredient_card_ids))`

**Validasi:**
- Nilai tidak boleh negatif.
- Sistem menolak `JualMasakan` bila pemain tidak memiliki bahan yang diminta.

---

## 6.8 `compliance.primary_need.rate`
**Definisi:** rasio kepatuhan pemain terhadap pembelian kebutuhan primer per hari sesuai ruleset.

**Input aturan:**
- `constraints.primary_need_max_per_day`
- `constraints.require_primary_before_others`

**Sumber event:**
- `Kebutuhan`
- `Kebutuhan`
- `Kebutuhan`

**Definisi hari patuh:**
Sistem menilai setiap `day_index` pada sesi:
1. Sistem anggap hari patuh jika pemain membeli kebutuhan primer minimal 1 kali saat ruleset mengharuskan.
2. Sistem anggap hari patuh jika pemain tidak membeli kebutuhan lain sebelum kebutuhan primer saat ruleset mengharuskan.
3. Sistem anggap hari patuh jika pemain tidak melebihi batas `primary_need_max_per_day`.

**Rumus:**
- `rate = jumlah_hari_patuh / jumlah_hari_yang_dinilai`

**Format penyimpanan:**
- `metric_value_numeric` menyimpan `rate`.
- `metric_value_json` menyimpan rincian per hari:
```json
{
  "days": [
    { "day_index": 0, "compliant": true, "reason": [] },
    { "day_index": 1, "compliant": false, "reason": ["BOUGHT_SECONDARY_BEFORE_PRIMARY"] }
  ],
  "evaluated_days": 2,
  "compliant_days": 1
}
```

---

## 6.9 `actions.used.total`
**Definisi:** total token aksi yang pemain pakai selama sesi.

**Sumber event:** `events.action_slot` pada event pemain.

**Rumus:**
- `count(distinct (day_index, turn_number, action_slot)) per user_id`

---

## 6.10 `rules.violations.count`
**Definisi:** jumlah pelanggaran aturan domain yang terjadi dalam sesi atau yang terkait pemain.

**Sumber data:**
1. `validation_logs` untuk event invalid yang ditolak sebelum masuk `events`.
2. log aplikasi terstruktur sebagai bukti operasional tambahan.

**Aturan:**
- Jika sistem menolak event, sistem tetap mencatat kegagalan pada `validation_logs`.
- Sistem tidak menyimpan event gagal pada tabel `events`.

**Rumus:**
- level sesi: `count(validation_logs) untuk session_id`
- level Player: sistem pakai `details_json.user_id` atau metadata request bila tersedia

---

## 6.11 `happiness.points.total`
**Definisi:** total Poin Kebahagiaan pemain berdasarkan kartu kebutuhan, bonus set, donasi, investasi emas, dana pensiun, tujuan keuangan, dan penalti.

**Sumber data:**
- `Kebutuhan` (poin kartu kebutuhan)
- `ruleset.scoring.donation_rank_points` atau `PoinPeringkatDonasi`
- `ruleset.scoring.gold_points_by_qty` atau `PoinEmas`
- `ruleset.scoring.pension_rank_points` atau `PoinPeringkatPensiun`
- `TujuanFinansial`
- `BagikanMisiKoleksi`
- `PinjamanSyariah`, `BayarPinjaman`

**Rumus ringkas:**
```
total = need_points
        + need_set_bonus
        + donation_points
        + gold_points
        + pension_points
        + saving_goal_points (jika semua pinjaman lunas)
        - mission_penalty
        - loan_penalty
```

**Catatan:**
- `saving_goal_points` tidak dihitung bila ada pinjaman belum lunas.
- Penalti misi diambil dari `BagikanMisiKoleksi.penalty_points`.
- Penalti pinjaman diambil dari `PinjamanSyariah.penalty_points` untuk loan yang belum lunas.

---

## 6.12 `happiness.need.points`
**Definisi:** total poin yang tercantum pada kartu kebutuhan.

**Sumber event:** `need.primary/secondary/tertiary.purchased`.

**Rumus:** `sum(payload.points)`.

Catatan: `payload.points` wajib diisi pada event kebutuhan.

---

## 6.13 `happiness.need.bonus`
**Definisi:** bonus set kebutuhan.

**Rumus:**
- `mixed_sets = min(primary_count, secondary_count, tertiary_count)`
- `same_sets = floor((primary_count - mixed_sets)/3) + floor((secondary_count - mixed_sets)/3) + floor((tertiary_count - mixed_sets)/3)`
- `bonus = mixed_sets * 4 + same_sets * 2`

---

## 6.14 `happiness.donation.points`
**Definisi:** poin dari juara donasi.

**Sumber data:** tabel `ruleset.scoring.donation_rank_points` (jika tersedia), atau `PoinPeringkatDonasi`.

**Rumus:** `sum(points_awarded)` per Jumat sesuai peringkat donasi.

**Aturan tie breaker:** jika jumlah donasi sama, gunakan `BagikanTieBreaker.number` (lebih besar menang).

---

## 6.15 `happiness.gold.points`
**Definisi:** poin investasi emas.

**Sumber data:** tabel `ruleset.scoring.gold_points_by_qty` (jika tersedia), atau `PoinEmas`.

---

## 6.16 `happiness.pension.points`
**Definisi:** poin juara dana pensiun.

**Sumber data:** tabel `ruleset.scoring.pension_rank_points` (jika tersedia), atau `PoinPeringkatPensiun`.

**Aturan tie breaker:** jika saldo sama, gunakan `BagikanTieBreaker.number` (lebih besar menang).

---

## 6.17 `happiness.saving_goal.points`
**Definisi:** poin tujuan keuangan.

**Sumber event:** `TujuanFinansial`.

**Aturan:** poin hanya dihitung jika tidak ada pinjaman syariah yang belum lunas.

Catatan: `Menabung.amount` maksimal 15 koin per aksi (rulebook).

---

## 6.18 `happiness.mission.penalty`
**Definisi:** penalti misi koleksi yang gagal.

**Sumber event:** `BagikanMisiKoleksi`.

**Aturan:** jika target misi tidak terpenuhi, penalti diambil dari `payload.penalty_points`.

Catatan: rulebook menetapkan `penalty_points = 10`.

---

## 6.19 `happiness.loan.penalty`
**Definisi:** penalti pinjaman syariah yang belum lunas.

**Sumber event:** `PinjamanSyariah`, `BayarPinjaman`.

**Aturan:** penalti diambil dari `payload.penalty_points` pada loan yang belum lunas.

Catatan: rulebook menetapkan `principal = 10` dan `penalty_points = 15`.

---
## 7. Pemetaan Event ke Metrik (Ringkas)
Sistem memakai pemetaan berikut sebagai aturan implementasi.

| Event | Metrik yang terpengaruh |
|---|---|
| CatatTransaksi | cashflow.in.total, cashflow.out.total, cashflow.net.total |
| JumatBerkah | donation.total, cashflow.out.total, cashflow.net.total |
| InvestasiEmas | gold.qty.current, cashflow.out.total |
| JualEmas | gold.qty.current, cashflow.in.total |
| RisikoKehidupan | cashflow.in.total / cashflow.out.total |
| Menabung | cashflow.out.total |
| TarikTabungan | cashflow.in.total |
| BahanMasakan | inventory.ingredient.total, cashflow.out.total |
| BuangBahanMasakan | inventory.ingredient.total |
| JualMasakan | orders.completed.count, inventory.ingredient.total, cashflow.in.total |
| LewatiOrder | gameplay.raw.variables (meal_orders_available_passed) |
| KerjaLepas | cashflow.in.total |
| Kebutuhan | compliance.primary_need.rate, cashflow.out.total |
| need.secondary/tertiary.purchased | compliance.primary_need.rate, cashflow.out.total |
| AkhirGiliran | actions.used.total |
| event ditolak validasi | rules.violations.count |
| BagikanMisiKoleksi | happiness.mission.penalty, happiness.points.total |
| PoinPeringkatDonasi | happiness.donation.points, happiness.points.total |
| PoinEmas | happiness.gold.points, happiness.points.total |
| PoinPeringkatPensiun | happiness.pension.points, happiness.points.total |
| TujuanFinansial | happiness.saving_goal.points, happiness.points.total |
| GunakanOpsiDarurat | cashflow.in.total, cashflow.out.total, gameplay.raw.variables (emergency_options_used) |
| PinjamanSyariah/BayarPinjaman | happiness.loan.penalty, loan.unpaid.flag |

Catatan:
- Jika tabel `ruleset.scoring.*` tersedia, sistem dapat menghitung `happiness.*` tanpa event awarding.

---

## 8. Aturan Penyimpanan Snapshot
### 8.1 Struktur record
Sistem menyimpan hasil pada tabel `metric_snapshots` dengan:
- `session_id`, opsional `user_id`, dan opsional `session_player_id`,
- `metric_name`,
- `metric_value_numeric` atau `metric_value_json`,
- `ruleset_version_id`.

Catatan implementasi:
- Snapshot agregat level sesi menyimpan `user_id = null` dan `session_player_id = null`.

### 8.2 Strategi “latest only”
Untuk dasbor real-time, sistem boleh menyimpan snapshot “terbaru saja” per metrik:
- sistem ambil snapshot terbaru per `metric_name` berdasarkan `computed_at`.

Jika sistem membutuhkan histori, sistem simpan snapshot berkala berdasarkan `action_slot` atau `day_index` pada JSON.

---

## 9. Contoh Perhitungan (Skenario Singkat)
### 9.1 Data contoh
Sesi S1, pemain P1:
1. `CatatTransaksi` OUT 5 (beli kebutuhan primer)
2. `BahanMasakan` OUT 1
3. `JualMasakan` IN 15, konsumsi 2 bahan
4. `JumatBerkah` OUT 2

### 9.2 Hasil metrik
- `cashflow.in.total = 15`
- `cashflow.out.total = 5 + 1 + 2 = 8`
- `cashflow.net.total = 15 - 8 = 7`
- `donation.total = 2`
- `orders.completed.count = 1`
- `inventory.ingredient.total = 1 - 2 = -1` → sistem tidak boleh menghasilkan nilai ini  
  Sistem harus menolak `JualMasakan` bila pemain belum punya 2 bahan.

Catatan: contoh ini menegaskan pentingnya validasi domain sebelum agregasi.

---

## 10. Kriteria Uji Metrik
Sistem lulus uji metrik jika:
1. Sistem menghasilkan nilai yang sama dengan hasil query manual pada tabel proyeksi dan event.
2. Sistem tidak menghasilkan nilai negatif untuk metrik kepemilikan (emas, bahan).
3. Sistem memperbarui snapshot setelah sistem menerima event yang relevan.
4. Sistem menampilkan nilai pada dasbor yang cocok dengan isi `metric_snapshots`.



