# Variabel Gameplay Fisik dan Metrik Turunan
## Sistem Informasi Dasbor Analitika & Manajemen Ruleset Cashflowpoly

### Dokumen
- Nama dokumen: Variabel Gameplay Fisik dan Metrik Turunan
- Versi: 1.1
- Tanggal: 3 Februari 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan Dokumen
Dokumen ini mendefinisikan variabel gameplay fisik (observable) dan metrik turunan yang relevan untuk analitika pembelajaran Cashflowpoly. Dokumen ini melengkapi definisi metrik minimum pada `docs/02-Perancangan/02-02-definisi-metrik-dan-agregasi.md` dan menjadi referensi untuk perluasan metrik di masa depan.

Catatan:
- Variabel pada dokumen ini bersifat konseptual dan diturunkan dari log event.
- Sebagian variabel membutuhkan event tambahan yang belum tersedia; bagian tersebut diberi catatan implementasi.

---

## 2. Sumber Data dan Konvensi
Sistem menggunakan sumber data berikut:
- `events` sebagai log aksi utama.
- `event_cashflow_projections` untuk ringkasan pemasukan/pengeluaran koin.
- `ruleset` aktif untuk nilai seperti `starting_cash`, `actions_per_turn`, dan aturan batasan.

Konvensi pengelompokan:
- Per sesi: agregasi semua pemain (`player_id = null`).
- Per pemain: agregasi berdasarkan `player_id`.
- Per hari: gunakan `day_index` dan `weekday` pada event.
- Per giliran: gunakan `turn_number` pada event.

---

## Part 1: Physical Gameplay Variables
Variabel berikut dapat diamati dari komponen game dan diturunkan dari event.

### 1.1 Coin-Based Financial Variables
**Raw Gameplay Variables (Observable):**
- `starting_coins`: modal awal pada awal permainan.
- `coins_held_current`: koin yang dimiliki pemain pada saat tertentu.
- `coins_spent_per_turn`: koin yang dikeluarkan dalam satu aksi.
- `coins_earned_per_turn`: koin yang diterima dalam satu aksi.
- `coins_donated`: total koin donasi di hari Jumat.
- `coins_saved`: total koin yang disetor ke tujuan keuangan (mode mahir).
- `coins_net_end_game`: saldo akhir di akhir permainan.

**Sumber data di sistem:**
- `starting_coins` dari ruleset (`starting_cash`).
- `coins_held_current` dari `starting_coins + cashflow.in.total - cashflow.out.total`.
- `coins_spent_per_turn` dari `event_cashflow_projections` dengan `direction = OUT` per `turn_number`.
- `coins_earned_per_turn` dari `event_cashflow_projections` dengan `direction = IN` per `turn_number`.
- `coins_donated` dari `day.friday.donation`.
- `coins_saved` dari `saving.deposit.created` minus `saving.deposit.withdrawn`.
- `coins_net_end_game` dari saldo akhir (net cashflow + starting).

**Calculation:**
```
coins_net_end_game = starting_coins + total_income - total_expenses
Where:
  total_income = freelance_work_coins + meal_order_income + loan_received + gold_sales
  total_expenses = ingredients_purchases + needs_purchases + donations + insurance_payments + loan_repayment + life_risk_payments
```

---

### 1.2 Ingredient Card Variables
**Raw Gameplay Variables (Observable):**
- `ingredients_collected`: total kartu bahan yang dibeli.
- `ingredients_held_current`: kartu bahan yang tersisa (maks 6 total, 3 jenis sama).
- `ingredient_types_held`: distribusi jenis bahan.
- `ingredients_used_per_meal`: jumlah bahan yang dipakai untuk klaim order.
- `ingredients_wasted`: kartu bahan yang dibuang/tidak terpakai.
- `ingredient_investment_coins_total`: total koin untuk membeli bahan.

**Sumber data di sistem:**
- `ingredients_collected` dari `ingredient.purchased`.
- `ingredients_held_current` dari `ingredient.purchased` dikurangi konsumsi `order.claimed` dan `ingredient.discarded`.
- `ingredient_types_held` dari `ingredient.purchased.ingredient_name`.
- `ingredients_used_per_meal` dari `order.claimed.required_ingredient_card_ids`.
- `ingredients_wasted` dari `ingredient.discarded`.
- `ingredient_investment_coins_total` dari `ingredient.purchased.amount` (biaya) di `event_cashflow_projections`.

---

### 1.3 Meal Order Variables
**Raw Gameplay Variables (Observable):**
- `meal_orders_claimed`: jumlah order masakan yang berhasil diklaim.
- `meal_orders_available_passed`: order yang tersedia tetapi tidak diambil.
- `meal_order_income_per_order`: pendapatan per order.
- `meal_order_income_total`: total pendapatan dari order.
- `meal_orders_per_turn_average`: rata-rata order per giliran.

**Sumber data di sistem:**
- `meal_orders_claimed` dari `order.claimed`.
- `meal_orders_available_passed` dari `order.passed`.
- `meal_order_income_per_order` dari `order.claimed.income`.
- `meal_order_income_total` dari penjumlahan `order.claimed.income`.
- `meal_orders_per_turn_average` dari `order.claimed` per `turn_number`.

**Business Efficiency Pattern:**
```
business_efficiency_ratio = meal_order_income_total / ingredient_investment_coins_total

High ratio (> 2.0): profitable business
Low ratio (< 1.5): inefficient business
```

---

### 1.4 Need Card Variables
**Raw Gameplay Variables (Observable):**
- `need_cards_purchased`: total kartu kebutuhan.
- `primary_needs_owned`: jumlah kebutuhan primer.
- `secondary_needs_owned`: jumlah kebutuhan sekunder.
- `tertiary_needs_owned`: jumlah kebutuhan tersier.
- `specific_tertiary_need`: apakah target tersier misi tercapai.
- `collection_mission_complete`: status misi koleksi.
- `need_cards_coins_spent`: total koin yang dibelanjakan untuk kebutuhan.

**Sumber data di sistem:**
- `need_cards_purchased` dari `need.primary/secondary/tertiary.purchased`.
- `need_cards_coins_spent` dari `event_cashflow_projections` kategori kebutuhan.
- `specific_tertiary_need` dan `collection_mission_complete` dari `mission.assigned` dan kartu kebutuhan yang dibeli.

**Categorization:**
- Basic Profile: memiliki semua kategori kebutuhan.
- Collector Profile: variasi kebutuhan tinggi.
- Specialist Profile: terkonsentrasi pada satu kategori.

---

### 1.5 Donation Variables (Friday Actions)
**Raw Gameplay Variables (Observable):**
- `donation_amount_per_friday`: jumlah donasi per hari Jumat.
- `donation_rank_per_friday`: peringkat donasi tiap Jumat.
- `donation_total_coins`: total donasi sepanjang sesi.
- `donation_champion_cards_earned`: kartu juara donasi yang diperoleh.
- `donation_happiness_points`: total poin kebahagiaan dari donasi.

**Sumber data di sistem:**
- `donation_amount_per_friday` dari `day.friday.donation` per `day_index`.
- `donation_rank_per_friday` dari `donation.rank.awarded`.
- `donation_total_coins` dari total `day.friday.donation.amount`.
- `donation_champion_cards_earned` dari jumlah event `donation.rank.awarded`.
- `donation_happiness_points` dari `donation.rank.awarded.points`.

**Donation Strategy Pattern:**
```
donation_aggressiveness = donation_total_coins / coins_net_end_game

High (> 30%): aggressive donor
Low (< 10%): conservative donor

stability = std_deviation(donation_amount_per_friday)
```

---

### 1.6 Gold Investment Variables
**Raw Gameplay Variables (Observable):**
- `gold_cards_purchased`: total kartu emas dibeli.
- `gold_cards_sold`: total kartu emas dijual.
- `gold_cards_held_end`: emas tersisa pada akhir permainan.
- `gold_prices_per_purchase`: harga beli per transaksi.
- `gold_price_per_sale`: harga jual per transaksi.
- `gold_investment_coins_spent`: total koin untuk beli emas.
- `gold_investment_coins_earned`: total koin hasil jual emas.
- `gold_investment_net`: selisih koin beli vs jual.

**Sumber data di sistem:**
- `day.saturday.gold_trade` dengan `trade_type = BUY/SELL`.
- `gold_prices_per_purchase` dari `unit_price`.
- `gold_investment_coins_spent/earned` dari `amount` (BUY/SELL).

**Investment Efficiency:**
```
gold_roi_percentage = (gold_investment_coins_earned - gold_investment_coins_spent)
                     / gold_investment_coins_spent * 100%
```

---

### 1.7 Pension Fund Variables (End-Game Scoring)
**Raw Gameplay Variables (Observable):**
- `leftover_coins_end_game`: koin tersisa di akhir permainan.
- `ingredient_cards_value_end`: nilai bahan sisa (1 koin per kartu).
- `coins_in_savings_goal`: koin yang masih tersimpan di tujuan keuangan.
- `pension_fund_total`: total dari tiga komponen di atas.
- `pension_fund_rank_per_game`: peringkat dana pensiun.
- `pension_fund_happiness_points`: poin kebahagiaan dari peringkat.

**Sumber data di sistem:**
- `leftover_coins_end_game` dari saldo akhir.
- `ingredient_cards_value_end` dari `inventory.ingredient.total` pada akhir sesi.
- `coins_in_savings_goal` dari total net deposit `saving.deposit.created - saving.deposit.withdrawn`.
- `pension_fund_rank_per_game` dari `pension.rank.awarded` (jika event awarding dipakai).

---

### 1.8 Life Risk Variables (Advanced Mode Only)
**Raw Gameplay Variables (Observable):**
- `life_risk_cards_drawn`: total event risiko kehidupan.
- `life_risk_costs_per_card`: penalti per kartu risiko.
- `life_risk_costs_total`: total penalti risiko.
- `life_risk_mitigated_with_insurance`: jumlah risiko yang ditangkal asuransi.
- `insurance_payments_made`: koin untuk aktivasi asuransi.
- `emergency_options_used`: penjualan aset darurat untuk menutup risiko.

**Sumber data di sistem:**
- `life_risk_cards_drawn` dari `risk.life.drawn`.
- `life_risk_costs_per_card` dari proyeksi `cashflow` untuk risk OUT.
- `life_risk_mitigated_with_insurance` dari `insurance.multirisk.used`.
- `insurance_payments_made` dari `insurance.multirisk.purchased`.
- `emergency_options_used` dari `risk.emergency.used`.

**Risk Resilience Pattern:**
```
risk_exposure_percentage = life_risk_costs_total / total_income * 100%

risk_mitigation_effectiveness = life_risk_mitigated_with_insurance / life_risk_cards_drawn * 100%
```

---

### 1.9 Financial Goal Variables (Advanced Mode Only)
**Raw Gameplay Variables (Observable):**
- `financial_goals_attempted`: jumlah goal yang dikejar.
- `financial_goals_completed`: goal yang selesai.
- `financial_goals_coins_per_goal`: biaya tiap goal.
- `financial_goals_coins_total_invested`: total koin disetor.
- `financial_goals_incomplete_coins_wasted`: koin mengendap pada goal yang gagal.
- `sharia_loan_cards_taken`: jumlah pinjaman.
- `sharia_loans_repaid`: jumlah pinjaman yang lunas.
- `sharia_loans_unpaid_end`: pinjaman belum lunas di akhir game.
- `loan_penalty_if_unpaid`: penalti poin jika ada pinjaman belum lunas.

**Sumber data di sistem:**
- `saving.deposit.created` dan `saving.deposit.withdrawn`.
- `saving.goal.achieved` untuk goal selesai.
- `loan.syariah.taken` dan `loan.syariah.repaid`.

Catatan perhitungan:
- `financial_goals_coins_per_goal` dihitung sebagai saldo tabungan per goal (deposit - withdraw - cost goal tercapai).
- `financial_goals_incomplete_coins_wasted` dihitung dari saldo goal yang belum tercapai.

**Debt Management Pattern:**
```
debt_ratio = sharia_loans_unpaid_end / sharia_loans_taken
```

---

### 1.10 Action Token Usage Variables
**Raw Gameplay Variables (Observable):**
- `actions_per_turn`: jumlah aksi per giliran (ruleset).
- `action_repetitions_per_turn`: aksi yang sama diulang dalam satu giliran.
- `action_sequence`: urutan aksi per giliran.
- `actions_skipped`: giliran tanpa aksi.

**Sumber data di sistem:**
- `actions_per_turn` dari ruleset.
- `action_repetitions_per_turn` dan `action_sequence` dari `events.action_type` per `turn_number`.

**Catatan implementasi:**
- `actions_skipped` membutuhkan event atau indikator khusus untuk giliran kosong.

**Action Pattern Analysis:**
```
action_diversity_score = distinct_action_types / 2
```

---

### 1.11 Turn-by-Turn Progression Variables
**Raw Gameplay Variables (Observable):**
- `coins_per_turn_progression`: saldo koin per giliran.
- `net_income_per_turn`: pemasukan dikurangi pengeluaran per giliran.
- `turn_number_when_debt_introduced`: giliran pertama pinjaman diambil.
- `turn_number_when_first_risk_hit`: giliran pertama terkena risiko.
- `turn_number_game_completion`: total giliran sampai finish.

**Sumber data di sistem:**
- `coins_per_turn_progression` dari agregasi `event_cashflow_projections` per `turn_number`.
- `turn_number_when_debt_introduced` dari `loan.syariah.taken` pertama.
- `turn_number_when_first_risk_hit` dari `risk.life.drawn` pertama.
- `turn_number_game_completion` dari `turn_number` maksimum di event sesi.

**Economic Trajectory Pattern:**
```
growth_pattern = coins_end / coins_start
```

---

## Part 2: Derived Analytics Metrics from Physical Variables
Metrik turunan di bawah dihitung dari variabel fisik di atas.

### 2.1 Financial Performance Metrics
**Metric 1: Net Worth Index**
```
net_worth_index = coins_net_end_game / starting_coins * 100%
```

**Metric 2: Income Diversification Ratio**
```
income_diversification = (freelance_income + meal_income + gold_income + donations_received)
                         / total_income * 100%
```

**Metric 3: Expense Management Efficiency**
```
expense_efficiency = essential_expenses / total_expenses * 100%

essential = ingredients necessary for meal orders
non-essential = donations, insurance, financial goals
```

**Metric 4: Profit Margin on Business Operations**
```
business_profit_margin = (meal_income - ingredient_costs) / meal_income * 100%
```

---

### 2.2 Strategic Decision Metrics
**Metric 5: Risk Appetite Score (Advanced Mode)**
```
risk_appetite = (life_risks_accepted / life_risks_available)
                * average_risk_cost
                * insurance_activation_rate
```

**Metric 6: Debt Leverage Ratio**
```
debt_leverage = total_loans_outstanding / net_worth * 100%

loan_repayment_discipline = loans_repaid / loans_taken * 100%
```

**Metric 7: Goal-Setting Ambition**
```
goal_ambition = financial_goals_attempted + financial_goals_coins_invested / net_worth * 100%
```

---

### 2.3 Player Behavior Metrics
**Metric 8: Action Efficiency per Turn**
```
action_efficiency = income_actions_per_turn / total_actions_per_turn
```

**Metric 9: Meal Order Success Rate**
```
meal_order_success_rate = meal_orders_completed / meal_orders_attempted * 100%
```

**Metric 10: Long-term Planning Indicator**
```
planning_horizon = (savings_invested + goals_pursued + insurance_purchases) / total_actions
```

---

### 2.4 Flourishing-Related Metrics (Physical Components)
**Metric 11: Need Fulfillment Diversity Index**
```
fulfillment_diversity = sqrt((primary_needs^2 + secondary_needs^2 + tertiary_needs^2)) / total_needs_owned

mission_achievement = specific_tertiary_acquired * complete_primary * complete_secondary
```

**Metric 12: Donation Consistency & Commitment**
```
donation_commitment_score = (donation_stability * donation_ratio) * friday_participation_rate

Where:
  donation_stability = 100 - std_deviation(donation_amounts)
  donation_ratio = total_donations / net_worth
  friday_participation_rate = fridays_donated / total_fridays
```

**Metric 13: Happiness Points Composition Analysis**
```
happiness_portfolio = [need_cards_pts, donations_pts, gold_pts, pension_pts, financial_goals_pts, mission_bonus_pts]
```

---

## 3. Catatan Implementasi
- Variabel yang membutuhkan event tambahan (contoh: `actions_skipped`) dapat dimasukkan ke backlog event jika ingin menghitung metrik turunan secara lebih detail.
- UI hanya menampilkan metrik yang tersedia dari API (`/api/analytics/...`). UI tidak menghitung ulang rumus di sisi klien.
- Jika metrik turunan ini diimplementasikan, pastikan `metric_name` mengikuti konvensi pada dokumen `02-02-definisi-metrik-dan-agregasi`.

### 3.1 Nama metrik snapshot JSON
Implementasi saat ini menyimpan variabel dan metrik turunan pada:
- `gameplay.raw.variables` (jsonb)
- `gameplay.derived.metrics` (jsonb)
