# Variabel Gameplay Fisik dan Metrik Turunan
## Sistem Informasi Dasbor Analitika & Manajemen Ruleset Cashflowpoly

### Dokumen
- Nama dokumen: Variabel Gameplay Fisik dan Metrik Turunan
- Versi: 1.2
- Tanggal: 22 Februari 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Tujuan Dokumen
Dokumen ini mendefinisikan variabel gameplay fisik (observable) dan metrik turunan yang relevan untuk analitika pembelajaran Cashflowpoly. Dokumen ini melengkapi definisi metrik minimum pada `docs/02-Perancangan/02-03-definisi-metrik-dan-agregasi.md` dan menjadi referensi untuk perluasan metrik di masa depan.

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

## Bagian 1: Variabel Gameplay Fisik
Variabel berikut diamati dari komponen game dan diturunkan dari event log.

### 1.0 Gameplay Session Metadata
**Variabel Gameplay Mentah (Teramati):**
- `game_id`
- `session_id`
- `player_id`
- `player_alias`
- `game_mode` (`beginner` atau `advanced`)
- `turn_number`
- `day_label`
- `action_slot`
- `event_timestamp`

**Aturan ketersediaan mode (v1):**
- Jika `game_mode = beginner`, field mode mahir diisi `null` (bukan `0`).

---

### 1.1 Variabel Finansial Berbasis Koin
- `starting_coins`
- `coins_held_current`
- `coins_spent_per_turn`
- `coins_earned_per_turn`
- `coins_donated`
- `coins_saved`
- `coins_net_end_game`

```
coins_net_end_game = starting_coins + total_income - total_expenses
total_income = freelance_work_coins + meal_order_income + loan_received + gold_sales
total_expenses = ingredients_purchases + needs_purchases + donations + insurance_payments + loan_repayment + life_risk_payments
```

---

### 1.2 Variabel Kartu Bahan
- `ingredients_collected`
- `ingredients_held_current`
- `ingredient_types_held`
- `ingredients_used_per_meal`
- `ingredients_wasted`
- `ingredient_investment_coins_total`

---

### 1.3 Variabel Order Makanan
- `meal_orders_claimed`
- `meal_orders_available_passed`
- `meal_order_income_per_order`
- `meal_order_income_total`
- `meal_orders_per_turn_average`

```
business_efficiency_ratio = meal_order_income_total / ingredient_investment_coins_total
```

---

### 1.4 Variabel Kartu Kebutuhan
- `need_cards_purchased`
- `primary_needs_owned`
- `secondary_needs_owned`
- `tertiary_needs_owned`
- `specific_tertiary_need`
- `collection_mission_complete`
- `need_cards_coins_spent`

---

### 1.5 Variabel Donasi (Aksi Hari Jumat)
- `donation_amount_per_friday`
- `donation_rank_per_friday`
- `donation_total_coins`
- `donation_champion_cards_earned`
- `donation_happiness_points`

```
donation_aggressiveness = donation_total_coins / coins_net_end_game
stability = std_deviation(donation_amount_per_friday)
```

---

### 1.6 Variabel Investasi Emas
- `gold_cards_purchased`
- `gold_cards_sold`
- `gold_cards_held_end`
- `gold_prices_per_purchase`
- `gold_price_per_sale`
- `gold_investment_coins_spent`
- `gold_investment_coins_earned`
- `gold_investment_net`

```
gold_roi_percentage = (gold_investment_coins_earned - gold_investment_coins_spent) / gold_investment_coins_spent * 100%
```

---

### 1.7 Variabel Dana Pensiun
- `leftover_coins_end_game`
- `ingredient_cards_value_end`
- `coins_in_savings_goal`
- `pension_fund_total`
- `pension_fund_rank_per_game`
- `pension_fund_happiness_points`

---

### 1.8 Variabel Risiko Kehidupan (Mode Advanced)
- `life_risks_available`
- `life_risk_cards_drawn`
- `life_risks_accepted`
- `life_risk_costs_per_card`
- `life_risk_costs_total`
- `life_risk_mitigated_with_insurance`
- `insurance_payments_made`
- `emergency_options_used`

```
risk_exposure_percentage = life_risk_costs_total / total_income * 100%
risk_mitigation_effectiveness = life_risk_mitigated_with_insurance / life_risk_cards_drawn * 100%
```

---

### 1.9 Variabel Tujuan Keuangan (Mode Advanced)
- `financial_goals_available_total`
- `financial_goals_attempted`
- `financial_goals_completed`
- `financial_goals_coins_per_goal`
- `financial_goals_coins_total_invested`
- `financial_goals_incomplete_coins_wasted`
- `sharia_loans_taken`
- `sharia_loans_repaid`
- `sharia_loans_unpaid_end`
- `sharia_loans_outstanding_coins`
- `loan_penalty_if_unpaid`

```
debt_ratio = sharia_loans_unpaid_end / sharia_loans_taken
```

---

### 1.10 Variabel Penggunaan Token Aksi
- `actions_per_turn`
- `action_repetitions_per_turn`
- `action_sequence`
- `actions_skipped`

```
action_diversity_score = distinct_action_types / 2
```

---

### 1.11 Variabel Progresi per Giliran
- `coins_per_turn_progression`
- `net_income_per_turn`
- `turn_number_when_debt_introduced`
- `turn_number_when_first_risk_hit`
- `turn_number_game_completion`

```
growth_pattern = coins_end / coins_start
```

---

### 1.12 End-Game Outcome Variables
- `total_happiness_points`
- `final_rank`
- `winner_flag`
- `finish_line_reached`
- `dnf_flag`

---

## Bagian 2: Metrik Analitika Turunan dari Variabel Fisik

### 2.1 Metrik Kinerja Finansial
**Metrik 1: Net Worth Index**
```
net_worth_index = coins_net_end_game / starting_coins * 100%
```

**Metrik 2: Income Diversification Index**
```
N_active_income_sources = count(income_source_i where income_source_i > 0)
Income_Share_i = income_source_i / total_income
income_diversification = ((1 - SUM(Income_Share_i^2)) / (1 - 1/N_active_income_sources)) * 100%
```

**Metrik 3: Expense Management Efficiency**
```
expense_efficiency = essential_expenses / total_expenses * 100%
```

**Metrik 4: Business Profit Margin**
```
business_profit_margin = (meal_income - ingredient_costs) / meal_income * 100%
```

---

### 2.2 Metrik Keputusan Strategis
**Metrik 5: Risk Appetite Score (Advanced)**
```
Risk_Acceptance_Rate = life_risks_accepted / life_risks_available
Insurance_Coverage_Rate = life_risk_mitigated_with_insurance / life_risk_cards_drawn
average_risk_cost = life_risk_costs_total / life_risk_cards_drawn
Risk_Cost_Intensity = average_risk_cost / starting_coins
risk_appetite = Risk_Acceptance_Rate * (1 - Insurance_Coverage_Rate) * Risk_Cost_Intensity * 100
```

**Metrik 6: Debt Leverage Ratio**
```
debt_leverage = sharia_loans_outstanding_coins / net_worth * 100%
loan_repayment_discipline = sharia_loans_repaid / sharia_loans_taken * 100%
```

**Metrik 7: Goal-Setting Ambition**
```
Goal_Attempt_Rate = financial_goals_attempted / financial_goals_available_total
Goal_Investment_Rate = financial_goals_coins_total_invested / net_worth
goal_ambition = ((Goal_Attempt_Rate * 0.4) + (Goal_Investment_Rate * 0.6)) * 100%
```

---

### 2.3 Metrik Perilaku Pemain
**Metrik 8: Action Efficiency**
```
action_efficiency = income_actions_per_turn / total_actions_per_turn
```

**Metrik 9: Meal Order Success Rate**
```
meal_order_success_rate = meal_orders_claimed / (meal_orders_claimed + meal_orders_available_passed) * 100%
```

**Metrik 10: Planning Horizon**
```
planning_horizon = (savings_invested + goals_pursued + insurance_purchases) / total_actions
```

---

### 2.4 Metrik Flourishing
**Metrik 11: Need Fulfillment Diversity**
```
p_primary = primary_needs_owned / total_needs_owned
p_secondary = secondary_needs_owned / total_needs_owned
p_tertiary = tertiary_needs_owned / total_needs_owned
fulfillment_diversity = (1 - (p_primary^2 + p_secondary^2 + p_tertiary^2)) / (1 - 1/3)
mission_achievement = if(collection_mission_complete = yes, 1, 0)
```

**Metrik 12: Donation Commitment Score**
```
donation_commitment_score = (donation_stability * donation_ratio) * friday_participation_rate
```

**Metrik 13: Happiness Portfolio**
```
happiness_portfolio = [need_cards_pts, donations_pts, gold_pts, pension_pts, financial_goals_pts, mission_bonus_pts]
```

---

### 2.5 Calculation Guardrails (v1)
- Jika penyebut `0`, hasil metrik rasio diisi `null`.
- Jika `net_worth <= 0`, metrik persen berbasis net worth diisi `null` kecuali dinyatakan eksplisit.
- Untuk field mode advanced pada mode beginner, nilai diisi `null`.
- Nilai terikat dibatasi ke rentang valid (contoh: 0-100, 0-1).

---

## 3. Catatan Implementasi
- Variabel yang membutuhkan event tambahan (contoh: `actions_skipped`) dapat dimasukkan ke backlog event jika ingin menghitung metrik turunan secara lebih detail.
- UI hanya menampilkan metrik yang tersedia dari API (`/api/v1/analytics/...`). UI tidak menghitung ulang rumus di sisi klien.
- Jika metrik turunan ini diimplementasikan, pastikan `metric_name` mengikuti konvensi pada dokumen `02-03-definisi-metrik-dan-agregasi`.

### 3.1 Nama metrik snapshot JSON
Implementasi saat ini menyimpan variabel dan metrik turunan pada:
- `gameplay.raw.variables` (jsonb)
- `gameplay.derived.metrics` (jsonb)
