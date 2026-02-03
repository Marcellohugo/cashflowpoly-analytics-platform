# Status Kesesuaian Implementasi
## Sistem Informasi Dasbor Analitika & Manajemen Ruleset Cashflowpoly

### Dokumen
- Nama dokumen: Status Kesesuaian Implementasi
- Versi: 1.0
- Tanggal: 31 Januari 2026
- Penyusun: Marco Marcello Hugo

---

## 1. Kesesuaian dengan Ringkasan Rulebook
Ringkasan berikut membandingkan mekanika utama pada `docs/00-ringkasan-rulebook-cashflowpoly.md` dengan implementasi saat ini.

| Mekanika | Status | Catatan Implementasi | Bukti Utama |
|---|---|---|---|
| Token aksi per giliran | Sesuai | Divalidasi melalui `turn.action.used` vs `actions_per_turn`. | `src/Cashflowpoly.Api/Controllers/EventsController.cs` |
| Donasi Jumat | Sesuai | Event donasi + poin juara otomatis jika ruleset scoring tersedia. | `src/Cashflowpoly.Api/Controllers/AnalyticsController.cs` |
| Investasi Emas Sabtu | Sesuai | BUY/SELL + validasi saldo & kepemilikan, poin emas otomatis via ruleset scoring. | `src/Cashflowpoly.Api/Controllers/AnalyticsController.cs` |
| Bahan & Pesanan Masakan | Sesuai | Validasi kepemilikan bahan dan klaim order tersedia. | `src/Cashflowpoly.Api/Controllers/EventsController.cs` |
| Kebutuhan primer/sekunder/tersier | Parsial | Aturan “primer dulu” + poin/bonus set dihitung jika payload menyertakan `points`. | `src/Cashflowpoly.Api/Controllers/EventsController.cs` |
| Kerja Lepas | Sesuai | Event `work.freelance.completed` + validasi amount sesuai ruleset. | `src/Cashflowpoly.Api/Controllers/EventsController.cs` |
| Risiko Kehidupan | Sesuai | Event + validasi tersedia, wajib mengikuti `order.claimed` pada mode mahir. | `src/Cashflowpoly.Api/Controllers/EventsController.cs` |
| Asuransi Multi Risiko | Parsial | Event pembelian & penggunaan ada, dampak risiko masih berbasis input event. | `src/Cashflowpoly.Api/Controllers/EventsController.cs` |
| Tabungan Tujuan Keuangan | Parsial | Deposit/withdraw/goal event tersedia, poin tujuan dihitung. | `src/Cashflowpoly.Api/Controllers/EventsController.cs` |
| Pinjaman Syariah | Parsial | Event pinjam & bayar ada, penalti dihitung dari payload. | `src/Cashflowpoly.Api/Controllers/EventsController.cs` |
| Poin Kebahagiaan | Sesuai | Dihitung dari event kebutuhan/bonus/penalti + scoring otomatis donasi/emas/pensiun bila tersedia. | `src/Cashflowpoly.Api/Controllers/AnalyticsController.cs` |

---

## 2. Kesesuaian dengan Ringkasan Proposal
Ringkasan berikut membandingkan artefak utama pada `docs/01-ringkasan-proposal-tugas-akhir.md` dengan implementasi saat ini.

| Artefak/Target | Status | Catatan Implementasi | Bukti Utama |
|---|---|---|---|
| Model data & basis data | Sesuai | Skema DB tersedia sesuai entitas utama. | `database/00_create_schema.sql` |
| RESTful API event log | Sesuai | Endpoint event & validasi tersedia. | `src/Cashflowpoly.Api/Controllers/EventsController.cs` |
| Manajemen ruleset dinamis | Sesuai | CRUD + versioning + aktivasi per sesi. | `src/Cashflowpoly.Api/Controllers/RulesetsController.cs` |
| Agregasi metrik | Sesuai | Cashflow, donasi, emas, compliance, dan poin kebahagiaan tersedia; skor poin otomatis via ruleset scoring. | `src/Cashflowpoly.Api/Controllers/AnalyticsController.cs` |
| Dasbor analitika instruktur | Sesuai | UI sesi, ringkasan analitika, detail pemain tersedia. | `src/Cashflowpoly.Ui/Views/Sessions/*` |
| Dasbor analitika pemain | Parsial | Detail pemain tersedia, belum ada pemisahan akses per peran. | `src/Cashflowpoly.Ui/Views/Players/Details.cshtml` |
| Pengujian fungsional & integrasi | Parsial | Dokumen uji ada, test otomatis bertambah namun belum menyeluruh. | `docs/03-Pengujian/*`, `tests/*` |

---

## 3. Catatan Prioritas Lanjutan
1. Menambah skenario uji otomatis untuk integrasi event kompleks (risk/loan/saving).
2. Menambahkan pemisahan akses per peran pada UI (instruktur vs pemain).
3. Menambah pengujian data riil untuk tabel scoring dari rulebook.

---

## 4. Referensi Dokumen Utama
- Ringkasan Rulebook: `docs/00-ringkasan-rulebook-cashflowpoly.md`
- Ringkasan Proposal: `docs/01-ringkasan-proposal-tugas-akhir.md`
- Spesifikasi Event & API: `docs/01-Spesifikasi/01-02-spesifikasi-event-dan-kontrak-api.md`
- Spesifikasi Ruleset: `docs/01-Spesifikasi/01-03-spesifikasi-ruleset-dan-validasi.md`
- Manual Pengguna: `docs/00-Panduan/00-02-manual-pengguna-dan-skenario-operasional.md`
