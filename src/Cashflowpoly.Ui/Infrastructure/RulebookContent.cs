// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui RulebookContent.
using Cashflowpoly.Ui.Models;

namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Kelas statis yang menyusun seluruh konten panduan permainan (rulebook)
/// Cashflowpoly ke dalam struktur ViewModel bilingual (Indonesia/Inggris),
/// mencakup aturan setup, alur permainan, aksi, dan perhitungan poin.
/// </summary>
public static class RulebookContent
{
    /// <summary>
    /// Membuat instance RulebookPageViewModel berisi seluruh seksi panduan permainan
    /// (komponen, setup, alur bermain, aksi, poin) beserta tabel skor,
    /// dengan teks disesuaikan berdasarkan bahasa yang dipilih.
    /// </summary>
    /// <param name="language">Kode bahasa ("id" atau "en"); default bahasa Indonesia.</param>
    /// <returns>ViewModel halaman rulebook yang siap ditampilkan di view.</returns>
    public static RulebookPageViewModel Build(string? language = null)
    {
        var normalizedLanguage = UiText.NormalizeLanguage(language);
        var isEnglish = string.Equals(normalizedLanguage, AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase);
        string L(string id, string en) => isEnglish ? en : id;

        return new RulebookPageViewModel
        {
            Title = L("Cashflowpoly - Entrepreneur Edition", "Cashflowpoly - Entrepreneur Edition"),
            Subtitle = L(
                "Pegang halaman ini untuk setup, giliran, dan hitung poin kebahagiaan.",
                "Use this page for setup, turn flow, and scoring."),
            Sections = new List<RulebookSectionViewModel>
            {
                new()
                {
                    Heading = L("A. Komponen permainan", "A. Game components"),
                    Description = L("Daftar komponen mode pemula dan mode mahir.", "List of components for beginner and advanced modes."),
                    Points = new List<string>
                    {
                        L(
                            "Komponen penunjang: buku panduan, lembar skor poin kebahagiaan, lembar catatan arus kas.",
                            "Supporting components: guidebook, happiness score sheet, and cashflow note sheet."),
                        L(
                            "Mode pemula: token Mr.Cashflowpoly, token aksi pemain, tent card tujuan permainan, layar pemain, papan donasi, papan investasi emas, papan kalender kerja, papan pesanan, papan kebutuhan, papan bahan.",
                            "Beginner mode: Mr. Cashflowpoly token, player action tokens, game-goal tent card, player screen, donation board, gold investment board, work-calendar board, order board, needs board, and ingredient board."),
                        L(
                            "Kartu mode pemula: juara dana pensiun, tie breaker, misi koleksi, harga emas, juara donasi, emas, kebutuhan, pesanan, bahan.",
                            "Beginner-mode cards: pension champion, tie-breaker, collection mission, gold price, donation champion, gold, needs, orders, and ingredients."),
                        L(
                            "Mode mahir menambah papan tabungan tujuan keuangan, papan risiko kehidupan, papan asuransi dan bank syariah.",
                            "Advanced mode adds saving-goal board, life-risk board, and insurance & sharia-bank board."),
                        L(
                            "Kartu mode mahir: tujuan keuangan, pinjaman syariah, risiko kehidupan.",
                            "Advanced-mode cards: financial goals, sharia loans, and life risks."),
                        L("Koin menggunakan denominasi 1, 5, dan 10.", "Coins use denominations of 1, 5, and 10.")
                    }
                },
                new()
                {
                    Heading = L("B. Setup awal mode pemula", "B. Beginner-mode initial setup"),
                    Description = L("Checklist setup meja permainan.", "Gameplay table setup checklist."),
                    Points = new List<string>
                    {
                        L("Dirikan tent card tujuan permainan.", "Place the game-goal tent card."),
                        L(
                            "Susun papan 3 baris: poin kebahagiaan (donasi, emas), penghasilan (kalender kerja, pesanan), pengeluaran (kebutuhan, bahan).",
                            "Arrange the board in 3 rows: happiness points (donation, gold), income (work calendar, orders), expense (needs, ingredients)."),
                        L(
                            "Letakkan kartu juara donasi dan juara dana pensiun pada papan donasi.",
                            "Place donation champion and pension champion cards on the donation board."),
                        L("Letakkan kartu emas dan dek harga emas pada papan investasi.", "Place gold cards and gold-price deck on the investment board."),
                        L("Letakkan token Mr.Cashflowpoly pada kotak GO.", "Place Mr. Cashflowpoly token on GO."),
                        L("Buka 5 kartu pesanan, 5 kartu kebutuhan primer, dan 5 kartu bahan.", "Open 5 order cards, 5 primary-needs cards, and 5 ingredient cards."),
                        L(
                            "Aturan slot bahan: maksimum 2 kartu sejenis di slot terbuka; kartu sejenis ke-3 dibuang dan diganti.",
                            "Ingredient-slot rule: max 2 cards of the same type in open slots; the 3rd same card is discarded and replaced."),
                        L("Siapkan bank koin dan simpan komponen yang tidak dipakai.", "Prepare the coin bank and store unused components.")
                    }
                },
                new()
                {
                    Heading = L("C. Setup awal mode mahir", "C. Advanced-mode initial setup"),
                    Description = L("Tambahan setup meja dari mode pemula.", "Additional setup on top of beginner mode."),
                    Points = new List<string>
                    {
                        L("Gunakan setup mode pemula sebagai dasar.", "Use beginner-mode setup as the baseline."),
                        L(
                            "Tambahkan papan tabungan tujuan keuangan, papan risiko kehidupan, dan papan asuransi dan bank syariah.",
                            "Add saving-goal board, life-risk board, and insurance & sharia-bank board."),
                        L("Letakkan 5 kartu tujuan keuangan pada papan tabungan.", "Place 5 financial-goal cards on saving board."),
                        L("Kocok lalu letakkan dek risiko kehidupan pada papan risiko.", "Shuffle and place life-risk deck on the risk board."),
                        L("Letakkan kartu pinjaman syariah pada papan asuransi dan bank syariah.", "Place sharia-loan cards on insurance & sharia-bank board.")
                    }
                },
                new()
                {
                    Heading = L("D. Setup pemain mode pemula", "D. Beginner-mode player setup"),
                    Description = L("Persiapan awal untuk setiap pemain.", "Initial preparation for each player."),
                    Points = new List<string>
                    {
                        L("Tentukan pemain pertama dengan kartu tie breaker.", "Determine the first player using tie-breaker card."),
                        L("Bagikan layar pemain sesuai identitas kartu tie breaker.", "Distribute player screens based on tie-breaker identity."),
                        L("Setiap pemain menerima 2 token aksi.", "Each player receives 2 action tokens."),
                        L("Kas awal pemain: 20 koin.", "Player starting cash: 20 coins."),
                        L(
                            "Bagikan acak 1 kartu bahan ke setiap pemain; pemain langsung membayar harga bahan ke bank.",
                            "Deal 1 random ingredient card to each player; players immediately pay the ingredient cost to bank."),
                        L("Setiap pemain menerima 1 kartu emas awal.", "Each player receives 1 starting gold card."),
                        L("Bagikan 1 kartu misi koleksi tertutup pada setiap pemain.", "Deal 1 face-down collection mission card to each player.")
                    }
                },
                new()
                {
                    Heading = L("E. Setup pemain mode mahir", "E. Advanced-mode player setup"),
                    Description = L("Penyesuaian setup pemain pada mode mahir.", "Player setup adjustments for advanced mode."),
                    Points = new List<string>
                    {
                        L("Gunakan setup pemain mode pemula sebagai dasar.", "Use beginner-mode player setup as baseline."),
                        L("Kas awal pemain menjadi 10 koin.", "Starting cash becomes 10 coins."),
                        L("Kartu tie breaker dibalik sebagai kartu asuransi aktif 1x gratis.", "Flip tie-breaker card as one-time free active insurance card."),
                        L("Setiap pemain menerima 1 kartu pinjaman syariah bernilai 10 koin.", "Each player receives 1 sharia-loan card worth 10 coins.")
                    }
                },
                new()
                {
                    Heading = L("F. Cara bermain", "F. How to play"),
                    Description = L("Alur satu putaran sampai permainan selesai.", "Round flow until game completion."),
                    Points = new List<string>
                    {
                        L("Setiap pemain menjalankan tepat 2 aksi per hari saat gilirannya; kedua aksi boleh sama atau berbeda.", "Each player performs exactly 2 actions per day on their turn; both actions may be the same or different."),
                        L("Setelah semua pemain selesai, token Mr.Cashflowpoly maju 1 tanggal.", "After all players finish, Mr. Cashflowpoly token moves forward by 1 date."),
                        L("Semua pemain mengambil kembali 2 token aksi di akhir putaran.", "All players take back 2 action tokens at end of round."),
                        L("Hari khusus: Jumat = peduli donasi, Sabtu = investasi emas, Minggu = libur.", "Special days: Friday = donation care, Saturday = gold investment, Sunday = rest."),
                        L("Permainan selesai saat token melewati garis finish, lalu hitung poin kebahagiaan.", "Game ends when token passes finish line, then calculate happiness points.")
                    }
                },
                new()
                {
                    Heading = L("G. Pilihan aksi mode pemula", "G. Beginner-mode action options"),
                    Description = L("Empat aksi utama yang menghabiskan token aksi.", "Four main actions that consume action tokens."),
                    Points = new List<string>
                    {
                        L(
                            "Beli 1 kartu bahan: letakkan token aksi di papan bahan, bayar harga kartu, ambil kartu bahan.",
                            "Buy 1 ingredient card: place action token on ingredient board, pay card cost, take ingredient card."),
                        L(
                            "Klaim 1 kartu pesanan: letakkan token di papan pesanan, serahkan bahan sesuai syarat, ambil kartu pesanan, terima penghasilan.",
                            "Claim 1 order card: place token on order board, submit required ingredients, take order card, receive income."),
                        L(
                            "Beli 1 kartu kebutuhan: letakkan token di papan kebutuhan, beli primer lebih dulu sebelum sekunder/tersier, bayar harga kartu.",
                            "Buy 1 needs card: place token on needs board, buy primary needs before secondary/tertiary, pay card cost."),
                        L(
                            "Kerja lepas 1x: letakkan token di papan kalender kerja, terima 1 koin dari bank.",
                            "Freelance once: place token on work-calendar board, receive 1 coin from bank."),
                        L("Slot kosong papan diisi ulang setelah pemain menyelesaikan 2 aksi.", "Empty board slots are refilled after player completes 2 actions.")
                    }
                },
                new()
                {
                    Heading = L("H. Pilihan aksi mode mahir", "H. Advanced-mode action options"),
                    Description = L("Aksi tambahan risiko, asuransi, tabungan, dan pinjaman.", "Additional actions for risk, insurance, savings, and loans."),
                    Points = new List<string>
                    {
                        L("Ambil kartu risiko kehidupan saat melakukan aksi klaim pesanan, lalu jalankan instruksi kartu.", "Draw a life-risk card when claiming an order, then execute card instructions."),
                        L("Jika kartu risiko menimbulkan biaya, pemain dapat pakai asuransi aktif atau membayar ke bank.", "If risk card creates cost, player may use active insurance or pay bank."),
                        L(
                            "Jika koin tidak cukup, pemain dapat menjual kebutuhan (setengah harga), menjual emas (harga aktif), atau mengambil pinjaman syariah.",
                            "If coins are insufficient, player may sell needs (half price), sell gold (active price), or take sharia loan."),
                        L(
                            "Aktifkan asuransi multi risiko: letakkan 1 token aksi di papan asuransi dan bank syariah, bayar premi 1 koin, lalu balik kartu tie breaker ke sisi asuransi.",
                            "Activate multi-risk insurance: place 1 action token on insurance & sharia-bank board, pay 1-coin premium, then flip tie-breaker card to insurance side."),
                        L(
                            "Menabung tujuan keuangan: letakkan token aksi di papan tabungan, setor maksimum 15 koin per aksi, dan ambil kartu tujuan jika nominal terpenuhi.",
                            "Save toward financial goals: place action token on saving board, deposit up to 15 coins per action, and claim goal card when amount is met."),
                        L("Bayar atau pinjam syariah: gunakan 1 token aksi untuk melunasi atau mengambil pinjaman.", "Repay or borrow sharia loan: use 1 action token to repay or take a loan."),
                        L(
                            "Pinjaman belum lunas di akhir permainan membuat kartu tujuan keuangan hangus dan terkena penalti poin kebahagiaan.",
                            "Unpaid loan at end game invalidates financial-goal cards and applies happiness point penalty.")
                    }
                },
                new()
                {
                    Heading = L("I. Aksi bebas (tanpa token aksi)", "I. Free actions (without action tokens)"),
                    Description = L("Aksi khusus yang dipicu oleh hari kalender.", "Special actions triggered by calendar days."),
                    Points = new List<string>
                    {
                        L("Peduli donasi (Jumat): semua pemain donasi tertutup minimal 1 koin, buka bersamaan, lalu tentukan peringkat.", "Donation care (Friday): all players make hidden donations (minimum 1 coin), reveal simultaneously, then rank results."),
                        L("Poin Kebahagiaan donasi: juara 1 = 7 poin kebahagiaan, juara 2 = 5 poin kebahagiaan, juara 3 = 2 poin kebahagiaan.", "Donation happiness points: rank 1 = 7 happiness points, rank 2 = 5 happiness points, rank 3 = 2 happiness points."),
                        L("Jika donasi sama, gunakan angka tie breaker terbesar.", "If donations tie, use highest tie-breaker number."),
                        L("Investasi emas (Sabtu): buka kartu harga emas, lalu pemain bebas beli atau jual emas sesuai harga aktif.", "Gold investment (Saturday): reveal gold price card, then players may freely buy/sell gold at active price."),
                        L("Pada hari Sabtu, setiap pemain memilih membeli atau menjual satu kali. Jumlah kartu emas dalam transaksi itu tidak dibatasi.", "On Saturday, each player chooses to buy or sell once. The number of gold cards in that transaction is unlimited.")
                    }
                },
                new()
                {
                    Heading = L("J. Catatan arus kas (opsional)", "J. Cashflow notes (optional)"),
                    Description = L("Format sederhana pencatatan pemasukan dan pengeluaran.", "Simple format for recording income and expenses."),
                    Points = new List<string>
                    {
                        L("Isi tanggal permainan dan nama pemain.", "Fill game date and player name."),
                        L("Catat setiap pemasukan pada kolom (+) dan pengeluaran pada kolom (-).", "Record each income in (+) and each expense in (-) column."),
                        L("Catat donasi Jumat setelah aksi donasi selesai.", "Record Friday donation after donation action is completed."),
                        L("Catat transaksi emas hari Sabtu: jual = (+), beli = (-).", "Record Saturday gold transactions: sell = (+), buy = (-)."),
                        L("Lakukan rekap akhir: saldo awal, total pemasukan, total pengeluaran, saldo akhir.", "Create final recap: starting balance, total income, total expense, ending balance.")
                    }
                },
                new()
                {
                    Heading = L("K. Perhitungan poin kebahagiaan", "K. Happiness point calculation"),
                    Description = L("Komponen skor, penalti, dan aturan tie breaker.", "Scoring components, penalties, and tie-breaker rules."),
                    Points = new List<string>
                    {
                        L("Jumlahkan seluruh poin kebahagiaan kartu aneka kebutuhan.", "Sum all happiness points from needs cards."),
                        L("Tambahkan bonus set kebutuhan secara mandiri: 3 jenis berbeda = +4 poin kebahagiaan dan setiap 3 jenis sama = +2 poin kebahagiaan.", "Count each needs-set pattern independently: 3 different types = +4 happiness points and each 3 same types = +2 happiness points."),
                        L("Tambahkan poin kebahagiaan juara donasi dan poin kebahagiaan investasi emas.", "Add donation champion happiness points and gold investment happiness points."),
                        L("Tambahkan poin kebahagiaan juara dana pensiun berdasarkan sisa koin akhir.", "Add pension champion happiness points based on remaining end-game coins."),
                        L("Tambahkan poin kebahagiaan kartu tujuan keuangan pada mode mahir hanya jika pinjaman lunas.", "Add financial-goal card happiness points in advanced mode only if loans are repaid."),
                        L("Kurangi penalti misi koleksi gagal sebesar 10 poin kebahagiaan.", "Subtract 10 happiness points for failed collection mission."),
                        L("Kurangi penalti pinjaman syariah belum lunas sebesar 15 poin kebahagiaan.", "Subtract 15 happiness points for unpaid sharia loans."),
                        L("Jika total poin kebahagiaan sama, pemenang ditentukan oleh angka tie breaker terbesar.", "If total happiness points tie, winner is determined by highest tie-breaker number.")
                    }
                },
                new()
                {
                    Heading = L("L. Ucapan terima kasih", "L. Acknowledgements"),
                    Description = L("Apresiasi kontributor.", "Contributor appreciation."),
                    Points = new List<string>
                    {
                        L("Daftar lengkap ucapan terima kasih mengikuti dokumen rulebook halaman 28.", "Full acknowledgements are listed in rulebook page 28."),
                        L("Mencakup keluarga, mentor, komunitas, akademisi, dan kolaborator board game.", "Includes family, mentors, community, academics, and board-game collaborators.")
                    }
                },
                new()
                {
                    Heading = L("M. Segera hadir", "M. Coming soon"),
                    Description = L("Cashflowpoly: Investor Edition.", "Cashflowpoly: Investor Edition."),
                    Points = new List<string>
                    {
                        L("Fokus tema: investor (deposito, obligasi, saham, mata uang kripto).", "Theme focus: investor (deposits, bonds, stocks, cryptocurrency)."),
                        L("Tujuan: mendukung keuangan keluarga untuk mencapai kebahagiaan.", "Goal: support family finance to achieve happiness."),
                        L("Video tutorial: Instagram @CASHFLOWPOLY.", "Tutorial videos: Instagram @CASHFLOWPOLY."),
                        L("Kontak kerja sama: adhicipta.playground@gmail.com.", "Partnership contact: adhicipta.playground@gmail.com.")
                    }
                }
            },
            Scoring = new List<RulebookScoreItemViewModel>
            {
                new()
                {
                    Category = L("Poin Kebahagiaan Kebutuhan", "Need Happiness Points"),
                    Rule = L("Jumlah poin kebahagiaan seluruh kartu aneka kebutuhan.", "Total happiness points from all needs cards.")
                },
                new()
                {
                    Category = L("Bonus Set Kebutuhan", "Need Set Bonus"),
                    Rule = L("Hitung mandiri: 3 jenis berbeda = +4 poin kebahagiaan; setiap 3 jenis sama = +2 poin kebahagiaan.", "Count independently: 3 different types = +4 happiness points; each 3 same types = +2 happiness points.")
                },
                new()
                {
                    Category = L("Poin Kebahagiaan Juara Donasi", "Donation Champion Happiness Points"),
                    Rule = L("Akumulasi poin kebahagiaan dari kartu juara donasi.", "Accumulated happiness points from donation champion card.")
                },
                new()
                {
                    Category = L("Poin Kebahagiaan Investasi Emas", "Gold Investment Happiness Points"),
                    Rule = L("1 emas = 3, 2 emas = 5, 3 emas = 8, 4 emas = 12.", "1 gold = 3, 2 gold = 5, 3 gold = 8, 4 gold = 12.")
                },
                new()
                {
                    Category = L("Poin Kebahagiaan Juara Dana Pensiun", "Pension Champion Happiness Points"),
                    Rule = L("Hitung dari sisa koin akhir, sisa bahan masakan, dan sisa tabungan; juara 1 = +5, juara 2 = +3, juara 3 = +1.", "Count ending cash, leftover ingredient cards, and remaining savings; rank 1 = +5, rank 2 = +3, rank 3 = +1.")
                },
                new()
                {
                    Category = L("Poin Kebahagiaan Tujuan Keuangan", "Financial Goal Happiness Points"),
                    Rule = L("Hanya dihitung di mode mahir bila seluruh pinjaman syariah sudah lunas.", "Count only in advanced mode when every sharia loan is fully repaid.")
                },
                new()
                {
                    Category = L("Minus Misi Koleksi", "Collection Mission Penalty"),
                    Rule = L("Minus 10 poin kebahagiaan bila misi koleksi tidak terpenuhi.", "Subtract 10 happiness points when the collection mission is not completed.")
                },
                new()
                {
                    Category = L("Minus Pinjaman Syariah", "Sharia Loan Penalty"),
                    Rule = L("Penalti tetap -15 poin kebahagiaan untuk pinjaman yang belum lunas.", "Fixed -15 happiness point penalty for unpaid loans.")
                },
                new()
                {
                    Category = L("Tie Breaker", "Tie Breaker"),
                    Rule = L("Jika total poin kebahagiaan sama, pemenang ditentukan oleh angka terbesar pada kartu tie breaker pemain.", "If total happiness points are tied, the player with the highest tie-breaker card number wins.")
                }
            }
        };
    }
}
