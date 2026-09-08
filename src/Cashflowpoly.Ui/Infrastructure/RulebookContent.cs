// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui RulebookContent.
// Mengimpor namespace `Cashflowpoly.Ui.Models` agar tipe/ekstensi dari pustaka tersebut dapat dirujuk tanpa menulis nama lengkapnya.
using Cashflowpoly.Ui.Models;

// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

/// <summary>
/// Kelas statis yang menyusun seluruh konten panduan permainan (rulebook)
/// Cashflowpoly ke dalam struktur ViewModel bilingual (Indonesia/Inggris),
/// mencakup aturan setup, alur permainan, aksi, dan perhitungan poin.
/// </summary>
// Mendefinisikan tipe class `RulebookContent`.
public static class RulebookContent
// Membuka scope tipe RulebookContent; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    /// <summary>
    /// Membuat instance RulebookPageViewModel berisi seluruh seksi panduan permainan
    /// (komponen, setup, alur bermain, aksi, poin) beserta tabel skor,
    /// dengan teks disesuaikan berdasarkan bahasa yang dipilih.
    /// </summary>
    /// <param name="language">Kode bahasa ("id" atau "en"); default bahasa Indonesia.</param>
    /// <returns>ViewModel halaman rulebook yang siap ditampilkan di view.</returns>
    // Mendefinisikan metode `Build` dengan hasil bertipe `RulebookPageViewModel`. Membuat instance RulebookPageViewModel berisi seluruh seksi panduan
    // permainan (komponen, setup, alur bermain, aksi, poin) beserta tabel skor, dengan teks disesuaikan berdasarkan bahasa yang dipilih. Masukan:
    // Parameter `language` bertipe `string?` membawa nilai language; nilai null diizinkan ketika data opsional belum tersedia; bila argumen tidak
    // diberikan digunakan null, yaitu penanda tidak ada nilai.
    public static RulebookPageViewModel Build(string? language = null)
    // Membuka scope metode Build; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
    {
        // Menyiapkan variabel lokal `normalizedLanguage` untuk nilai normalized language dengan memanggil `UiText.NormalizeLanguage` dengan `language`.
        // Tipe variabel disimpulkan dari ekspresi nilai awal.
        var normalizedLanguage = UiText.NormalizeLanguage(language);
        // Menyiapkan variabel lokal `isEnglish` untuk nilai berstatus english dengan membandingkan kesamaan `string` dengan `normalizedLanguage`,
        // `AuthConstants.LanguageEn`, `StringComparison.OrdinalIgnoreCase`; aturan perbandingan mengikuti overload dan comparer yang diberikan. Tipe
        // variabel disimpulkan dari ekspresi nilai awal.
        var isEnglish = string.Equals(normalizedLanguage, AuthConstants.LanguageEn, StringComparison.OrdinalIgnoreCase);
        // Mendefinisikan fungsi lokal L dengan hasil `string`; fungsi ini dipakai oleh alur di dalam scope yang sama. Ekspresi hasilnya adalah hasil
        // pemilihan bersyarat: ketika `isEnglish` benar gunakan `en`, jika tidak gunakan `id`.
        string L(string id, string en) => isEnglish ? en : id;

        // Mengembalikan objek baru bertipe `RulebookPageViewModel` dengan nilai awal sesuai konstruktornya kepada pemanggil dalam Build; eksekusi jalur ini
        // selesai setelah nilai hasil ditentukan.
        return new RulebookPageViewModel
        // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
        {
            // Memperbarui `Title` menggunakan memanggil `L` dengan `”Cashflowpoly - Entrepreneur Edition”`, `”Cashflowpoly - Entrepreneur Edition”` dalam
            // Build.
            Title = L("Cashflowpoly - Entrepreneur Edition", "Cashflowpoly - Entrepreneur Edition"),
            // Memperbarui `Subtitle` menggunakan memanggil `L` dengan `”Pegang halaman ini untuk setup, giliran, dan hitung poin kebahagiaan.”`, `”Use this
            // page for setup, turn flow, and scoring.”` dalam Build.
            Subtitle = L(
                // Meneruskan nilai literal `”Pegang halaman ini untuk setup, giliran, dan hitung poin kebahagiaan.”` sebagai argumen ke `L`.
                "Pegang halaman ini untuk setup, giliran, dan hitung poin kebahagiaan.",
                // Meneruskan nilai literal `”Use this page for setup, turn flow, and scoring.”` sebagai argumen ke `L`.
                "Use this page for setup, turn flow, and scoring."),
            // Memperbarui `Sections` menggunakan objek baru bertipe `List<RulebookSectionViewModel>` dengan nilai awal sesuai konstruktornya dalam Build.
            Sections = new List<RulebookSectionViewModel>
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
            {
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Heading` menggunakan memanggil `L` dengan `”A. Komponen permainan”`, `”A. Game components”` dalam Build.
                    Heading = L("A. Komponen permainan", "A. Game components"),
                    // Memperbarui `Description` menggunakan memanggil `L` dengan `”Daftar komponen mode pemula dan mode mahir.”`, `”List of components for beginner and
                    // advanced modes.”` dalam Build.
                    Description = L("Daftar komponen mode pemula dan mode mahir.", "List of components for beginner and advanced modes."),
                    // Memperbarui `Points` menggunakan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya dalam Build.
                    Points = new List<string>
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                    {
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Komponen penunjang: buku panduan, lembar skor poin kebahagiaan, lembar catatan arus kas.”`,
                        // `”Supporting components: guidebook, happiness score sheet, and cashflow note sheet.”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Komponen penunjang: buku panduan, lembar skor poin kebahagiaan, lembar catatan arus kas.”` sebagai argumen ke `L`.
                            "Komponen penunjang: buku panduan, lembar skor poin kebahagiaan, lembar catatan arus kas.",
                            // Meneruskan nilai literal `”Supporting components: guidebook, happiness score sheet, and cashflow note sheet.”` sebagai argumen ke `L`.
                            "Supporting components: guidebook, happiness score sheet, and cashflow note sheet."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Mode pemula: token Mr.Cashflowpoly, token aksi pemain, tent card tujuan permainan, layar
                        // pemain, papan donasi, papan investasi emas, papan kalender kerja, papan pesanan, papa...`, `”Beginner mode: Mr. Cashflowpoly token, player action
                        // tokens, game-goal tent card, player screen, donation board, gold investment board, work-calendar board, order board, need...` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Mode pemula: token Mr.Cashflowpoly, token aksi pemain, tent card tujuan permainan, layar pemain, papan donasi, papan
                            // investasi emas, papan kalender kerja, papan pesanan, papa...` sebagai argumen ke `L`.
                            "Mode pemula: token Mr.Cashflowpoly, token aksi pemain, tent card tujuan permainan, layar pemain, papan donasi, papan investasi emas, papan kalender kerja, papan pesanan, papan kebutuhan, papan bahan.",
                            // Meneruskan nilai literal `”Beginner mode: Mr. Cashflowpoly token, player action tokens, game-goal tent card, player screen, donation board, gold
                            // investment board, work-calendar board, order board, need...` sebagai argumen ke `L`.
                            "Beginner mode: Mr. Cashflowpoly token, player action tokens, game-goal tent card, player screen, donation board, gold investment board, work-calendar board, order board, needs board, and ingredient board."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Kartu mode pemula: juara dana pensiun, tie breaker, misi koleksi, harga emas, juara donasi,
                        // emas, kebutuhan, pesanan, bahan.”`, `”Beginner-mode cards: pension champion, tie-breaker, collection mission, gold price, donation champion,
                        // gold, needs, orders, and ingredients.”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Kartu mode pemula: juara dana pensiun, tie breaker, misi koleksi, harga emas, juara donasi, emas, kebutuhan, pesanan,
                            // bahan.”` sebagai argumen ke `L`.
                            "Kartu mode pemula: juara dana pensiun, tie breaker, misi koleksi, harga emas, juara donasi, emas, kebutuhan, pesanan, bahan.",
                            // Meneruskan nilai literal `”Beginner-mode cards: pension champion, tie-breaker, collection mission, gold price, donation champion, gold, needs,
                            // orders, and ingredients.”` sebagai argumen ke `L`.
                            "Beginner-mode cards: pension champion, tie-breaker, collection mission, gold price, donation champion, gold, needs, orders, and ingredients."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Mode mahir menambah papan tabungan tujuan keuangan, papan risiko kehidupan, papan asuransi
                        // dan bank syariah.”`, `”Advanced mode adds saving-goal board, life-risk board, and insurance & sharia-bank board.”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Mode mahir menambah papan tabungan tujuan keuangan, papan risiko kehidupan, papan asuransi dan bank syariah.”` sebagai
                            // argumen ke `L`.
                            "Mode mahir menambah papan tabungan tujuan keuangan, papan risiko kehidupan, papan asuransi dan bank syariah.",
                            // Meneruskan nilai literal `”Advanced mode adds saving-goal board, life-risk board, and insurance & sharia-bank board.”` sebagai argumen ke `L`.
                            "Advanced mode adds saving-goal board, life-risk board, and insurance & sharia-bank board."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Kartu mode mahir: tujuan keuangan, pinjaman syariah, risiko kehidupan.”`, `”Advanced-mode
                        // cards: financial goals, sharia loans, and life risks.”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Kartu mode mahir: tujuan keuangan, pinjaman syariah, risiko kehidupan.”` sebagai argumen ke `L`.
                            "Kartu mode mahir: tujuan keuangan, pinjaman syariah, risiko kehidupan.",
                            // Meneruskan nilai literal `”Advanced-mode cards: financial goals, sharia loans, and life risks.”` sebagai argumen ke `L`.
                            "Advanced-mode cards: financial goals, sharia loans, and life risks."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Koin menggunakan denominasi 1, 5, dan 10.”`, `”Coins use denominations of 1, 5, and 10.”`
                        // dalam Build.
                        L("Koin menggunakan denominasi 1, 5, dan 10.", "Coins use denominations of 1, 5, and 10.")
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                    }
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Heading` menggunakan memanggil `L` dengan `”B. Setup awal mode pemula”`, `”B. Beginner-mode initial setup”` dalam Build.
                    Heading = L("B. Setup awal mode pemula", "B. Beginner-mode initial setup"),
                    // Memperbarui `Description` menggunakan memanggil `L` dengan `”Checklist setup meja permainan.”`, `”Gameplay table setup checklist.”` dalam Build.
                    Description = L("Checklist setup meja permainan.", "Gameplay table setup checklist."),
                    // Memperbarui `Points` menggunakan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya dalam Build.
                    Points = new List<string>
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                    {
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Dirikan tent card tujuan permainan.”`, `”Place the game-goal tent card.”` dalam Build.
                        L("Dirikan tent card tujuan permainan.", "Place the game-goal tent card."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Susun papan 3 baris: poin kebahagiaan (donasi, emas), penghasilan (kalender kerja, pesanan),
                        // pengeluaran (kebutuhan, bahan).”`, `”Arrange the board in 3 rows: happiness points (donation, gold), income (work calendar, orders), expense
                        // (needs, ingredients).”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Susun papan 3 baris: poin kebahagiaan (donasi, emas), penghasilan (kalender kerja, pesanan), pengeluaran (kebutuhan,
                            // bahan).”` sebagai argumen ke `L`.
                            "Susun papan 3 baris: poin kebahagiaan (donasi, emas), penghasilan (kalender kerja, pesanan), pengeluaran (kebutuhan, bahan).",
                            // Meneruskan nilai literal `”Arrange the board in 3 rows: happiness points (donation, gold), income (work calendar, orders), expense (needs,
                            // ingredients).”` sebagai argumen ke `L`.
                            "Arrange the board in 3 rows: happiness points (donation, gold), income (work calendar, orders), expense (needs, ingredients)."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Letakkan kartu juara donasi dan juara dana pensiun pada papan donasi.”`, `”Place donation
                        // champion and pension champion cards on the donation board.”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Letakkan kartu juara donasi dan juara dana pensiun pada papan donasi.”` sebagai argumen ke `L`.
                            "Letakkan kartu juara donasi dan juara dana pensiun pada papan donasi.",
                            // Meneruskan nilai literal `”Place donation champion and pension champion cards on the donation board.”` sebagai argumen ke `L`.
                            "Place donation champion and pension champion cards on the donation board."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Letakkan kartu emas dan dek harga emas pada papan investasi.”`, `”Place gold cards and
                        // gold-price deck on the investment board.”` dalam Build.
                        L("Letakkan kartu emas dan dek harga emas pada papan investasi.", "Place gold cards and gold-price deck on the investment board."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Letakkan token Mr.Cashflowpoly pada kotak GO.”`, `”Place Mr. Cashflowpoly token on GO.”`
                        // dalam Build.
                        L("Letakkan token Mr.Cashflowpoly pada kotak GO.", "Place Mr. Cashflowpoly token on GO."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Buka 5 kartu pesanan, 5 kartu kebutuhan primer, dan 5 kartu bahan.”`, `”Open 5 order cards,
                        // 5 primary-needs cards, and 5 ingredient cards.”` dalam Build.
                        L("Buka 5 kartu pesanan, 5 kartu kebutuhan primer, dan 5 kartu bahan.", "Open 5 order cards, 5 primary-needs cards, and 5 ingredient cards."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Aturan slot bahan: maksimum 2 kartu sejenis di slot terbuka; kartu sejenis ke-3 dibuang dan
                        // diganti.”`, `”Ingredient-slot rule: max 2 cards of the same type in open slots; the 3rd same card is discarded and replaced.”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Aturan slot bahan: maksimum 2 kartu sejenis di slot terbuka; kartu sejenis ke-3 dibuang dan diganti.”` sebagai argumen
                            // ke `L`.
                            "Aturan slot bahan: maksimum 2 kartu sejenis di slot terbuka; kartu sejenis ke-3 dibuang dan diganti.",
                            // Meneruskan nilai literal `”Ingredient-slot rule: max 2 cards of the same type in open slots; the 3rd same card is discarded and replaced.”`
                            // sebagai argumen ke `L`.
                            "Ingredient-slot rule: max 2 cards of the same type in open slots; the 3rd same card is discarded and replaced."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Siapkan bank koin dan simpan komponen yang tidak dipakai.”`, `”Prepare the coin bank and
                        // store unused components.”` dalam Build.
                        L("Siapkan bank koin dan simpan komponen yang tidak dipakai.", "Prepare the coin bank and store unused components.")
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                    }
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Heading` menggunakan memanggil `L` dengan `”C. Setup awal mode mahir”`, `”C. Advanced-mode initial setup”` dalam Build.
                    Heading = L("C. Setup awal mode mahir", "C. Advanced-mode initial setup"),
                    // Memperbarui `Description` menggunakan memanggil `L` dengan `”Tambahan setup meja dari mode pemula.”`, `”Additional setup on top of beginner
                    // mode.”` dalam Build.
                    Description = L("Tambahan setup meja dari mode pemula.", "Additional setup on top of beginner mode."),
                    // Memperbarui `Points` menggunakan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya dalam Build.
                    Points = new List<string>
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                    {
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Gunakan setup mode pemula sebagai dasar.”`, `”Use beginner-mode setup as the baseline.”`
                        // dalam Build.
                        L("Gunakan setup mode pemula sebagai dasar.", "Use beginner-mode setup as the baseline."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Tambahkan papan tabungan tujuan keuangan, papan risiko kehidupan, dan papan asuransi dan
                        // bank syariah.”`, `”Add saving-goal board, life-risk board, and insurance & sharia-bank board.”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Tambahkan papan tabungan tujuan keuangan, papan risiko kehidupan, dan papan asuransi dan bank syariah.”` sebagai
                            // argumen ke `L`.
                            "Tambahkan papan tabungan tujuan keuangan, papan risiko kehidupan, dan papan asuransi dan bank syariah.",
                            // Meneruskan nilai literal `”Add saving-goal board, life-risk board, and insurance & sharia-bank board.”` sebagai argumen ke `L`.
                            "Add saving-goal board, life-risk board, and insurance & sharia-bank board."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Letakkan 5 kartu tujuan keuangan pada papan tabungan.”`, `”Place 5 financial-goal cards on
                        // saving board.”` dalam Build.
                        L("Letakkan 5 kartu tujuan keuangan pada papan tabungan.", "Place 5 financial-goal cards on saving board."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Kocok lalu letakkan dek risiko kehidupan pada papan risiko.”`, `”Shuffle and place life-risk
                        // deck on the risk board.”` dalam Build.
                        L("Kocok lalu letakkan dek risiko kehidupan pada papan risiko.", "Shuffle and place life-risk deck on the risk board."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Letakkan kartu pinjaman syariah pada papan asuransi dan bank syariah.”`, `”Place sharia-loan
                        // cards on insurance & sharia-bank board.”` dalam Build.
                        L("Letakkan kartu pinjaman syariah pada papan asuransi dan bank syariah.", "Place sharia-loan cards on insurance & sharia-bank board.")
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                    }
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Heading` menggunakan memanggil `L` dengan `”D. Setup pemain mode pemula”`, `”D. Beginner-mode player setup”` dalam Build.
                    Heading = L("D. Setup pemain mode pemula", "D. Beginner-mode player setup"),
                    // Memperbarui `Description` menggunakan memanggil `L` dengan `”Persiapan awal untuk setiap pemain.”`, `”Initial preparation for each player.”`
                    // dalam Build.
                    Description = L("Persiapan awal untuk setiap pemain.", "Initial preparation for each player."),
                    // Memperbarui `Points` menggunakan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya dalam Build.
                    Points = new List<string>
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                    {
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Tentukan pemain pertama dengan kartu tie breaker.”`, `”Determine the first player using
                        // tie-breaker card.”` dalam Build.
                        L("Tentukan pemain pertama dengan kartu tie breaker.", "Determine the first player using tie-breaker card."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Bagikan layar pemain sesuai identitas kartu tie breaker.”`, `”Distribute player screens
                        // based on tie-breaker identity.”` dalam Build.
                        L("Bagikan layar pemain sesuai identitas kartu tie breaker.", "Distribute player screens based on tie-breaker identity."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Setiap pemain menerima 2 token aksi.”`, `”Each player receives 2 action tokens.”` dalam
                        // Build.
                        L("Setiap pemain menerima 2 token aksi.", "Each player receives 2 action tokens."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Kas awal pemain: 20 koin.”`, `”Player starting cash: 20 coins.”` dalam Build.
                        L("Kas awal pemain: 20 koin.", "Player starting cash: 20 coins."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Bagikan acak 1 kartu bahan ke setiap pemain; pemain langsung membayar harga bahan ke
                        // bank.”`, `”Deal 1 random ingredient card to each player; players immediately pay the ingredient cost to bank.”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Bagikan acak 1 kartu bahan ke setiap pemain; pemain langsung membayar harga bahan ke bank.”` sebagai argumen ke `L`.
                            "Bagikan acak 1 kartu bahan ke setiap pemain; pemain langsung membayar harga bahan ke bank.",
                            // Meneruskan nilai literal `”Deal 1 random ingredient card to each player; players immediately pay the ingredient cost to bank.”` sebagai argumen
                            // ke `L`.
                            "Deal 1 random ingredient card to each player; players immediately pay the ingredient cost to bank."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Setiap pemain menerima 1 kartu emas awal.”`, `”Each player receives 1 starting gold card.”`
                        // dalam Build.
                        L("Setiap pemain menerima 1 kartu emas awal.", "Each player receives 1 starting gold card."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Bagikan 1 kartu misi koleksi tertutup pada setiap pemain.”`, `”Deal 1 face-down collection
                        // mission card to each player.”` dalam Build.
                        L("Bagikan 1 kartu misi koleksi tertutup pada setiap pemain.", "Deal 1 face-down collection mission card to each player.")
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                    }
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Heading` menggunakan memanggil `L` dengan `”E. Setup pemain mode mahir”`, `”E. Advanced-mode player setup”` dalam Build.
                    Heading = L("E. Setup pemain mode mahir", "E. Advanced-mode player setup"),
                    // Memperbarui `Description` menggunakan memanggil `L` dengan `”Penyesuaian setup pemain pada mode mahir.”`, `”Player setup adjustments for advanced
                    // mode.”` dalam Build.
                    Description = L("Penyesuaian setup pemain pada mode mahir.", "Player setup adjustments for advanced mode."),
                    // Memperbarui `Points` menggunakan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya dalam Build.
                    Points = new List<string>
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                    {
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Gunakan setup pemain mode pemula sebagai dasar.”`, `”Use beginner-mode player setup as
                        // baseline.”` dalam Build.
                        L("Gunakan setup pemain mode pemula sebagai dasar.", "Use beginner-mode player setup as baseline."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Kas awal pemain menjadi 10 koin.”`, `”Starting cash becomes 10 coins.”` dalam Build.
                        L("Kas awal pemain menjadi 10 koin.", "Starting cash becomes 10 coins."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Kartu tie breaker dibalik sebagai kartu asuransi aktif 1x gratis.”`, `”Flip tie-breaker card
                        // as one-time free active insurance card.”` dalam Build.
                        L("Kartu tie breaker dibalik sebagai kartu asuransi aktif 1x gratis.", "Flip tie-breaker card as one-time free active insurance card."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Setiap pemain menerima 1 kartu pinjaman syariah bernilai 10 koin.”`, `”Each player receives
                        // 1 sharia-loan card worth 10 coins.”` dalam Build.
                        L("Setiap pemain menerima 1 kartu pinjaman syariah bernilai 10 koin.", "Each player receives 1 sharia-loan card worth 10 coins.")
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                    }
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Heading` menggunakan memanggil `L` dengan `”F. Cara bermain”`, `”F. How to play”` dalam Build.
                    Heading = L("F. Cara bermain", "F. How to play"),
                    // Memperbarui `Description` menggunakan memanggil `L` dengan `”Alur satu putaran sampai permainan selesai.”`, `”Round flow until game completion.”`
                    // dalam Build.
                    Description = L("Alur satu putaran sampai permainan selesai.", "Round flow until game completion."),
                    // Memperbarui `Points` menggunakan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya dalam Build.
                    Points = new List<string>
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                    {
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Setiap pemain menjalankan tepat 2 aksi per hari saat gilirannya; kedua aksi boleh sama atau
                        // berbeda.”`, `”Each player performs exactly 2 actions per day on their turn; both actions may be the same or different.”` dalam Build.
                        L("Setiap pemain menjalankan tepat 2 aksi per hari saat gilirannya; kedua aksi boleh sama atau berbeda.", "Each player performs exactly 2 actions per day on their turn; both actions may be the same or different."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Setelah semua pemain selesai, token Mr.Cashflowpoly maju 1 tanggal.”`, `”After all players
                        // finish, Mr. Cashflowpoly token moves forward by 1 date.”` dalam Build.
                        L("Setelah semua pemain selesai, token Mr.Cashflowpoly maju 1 tanggal.", "After all players finish, Mr. Cashflowpoly token moves forward by 1 date."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Semua pemain mengambil kembali 2 token aksi di akhir putaran.”`, `”All players take back 2
                        // action tokens at end of round.”` dalam Build.
                        L("Semua pemain mengambil kembali 2 token aksi di akhir putaran.", "All players take back 2 action tokens at end of round."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Hari khusus: Jumat = peduli donasi, Sabtu = investasi emas, Minggu = libur.”`, `”Special
                        // days: Friday = donation care, Saturday = gold investment, Sunday = rest.”` dalam Build.
                        L("Hari khusus: Jumat = peduli donasi, Sabtu = investasi emas, Minggu = libur.", "Special days: Friday = donation care, Saturday = gold investment, Sunday = rest."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Permainan selesai saat token melewati garis finish, lalu hitung poin kebahagiaan.”`, `”Game
                        // ends when token passes finish line, then calculate happiness points.”` dalam Build.
                        L("Permainan selesai saat token melewati garis finish, lalu hitung poin kebahagiaan.", "Game ends when token passes finish line, then calculate happiness points.")
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                    }
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Heading` menggunakan memanggil `L` dengan `”G. Pilihan aksi mode pemula”`, `”G. Beginner-mode action options”` dalam Build.
                    Heading = L("G. Pilihan aksi mode pemula", "G. Beginner-mode action options"),
                    // Memperbarui `Description` menggunakan memanggil `L` dengan `”Empat aksi utama yang menghabiskan token aksi.”`, `”Four main actions that consume
                    // action tokens.”` dalam Build.
                    Description = L("Empat aksi utama yang menghabiskan token aksi.", "Four main actions that consume action tokens."),
                    // Memperbarui `Points` menggunakan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya dalam Build.
                    Points = new List<string>
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                    {
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Beli 1 kartu bahan: letakkan token aksi di papan bahan, bayar harga kartu, ambil kartu
                        // bahan.”`, `”Buy 1 ingredient card: place action token on ingredient board, pay card cost, take ingredient card.”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Beli 1 kartu bahan: letakkan token aksi di papan bahan, bayar harga kartu, ambil kartu bahan.”` sebagai argumen ke
                            // `L`.
                            "Beli 1 kartu bahan: letakkan token aksi di papan bahan, bayar harga kartu, ambil kartu bahan.",
                            // Meneruskan nilai literal `”Buy 1 ingredient card: place action token on ingredient board, pay card cost, take ingredient card.”` sebagai argumen
                            // ke `L`.
                            "Buy 1 ingredient card: place action token on ingredient board, pay card cost, take ingredient card."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Klaim 1 kartu pesanan: letakkan token di papan pesanan, serahkan bahan sesuai syarat, ambil
                        // kartu pesanan, terima penghasilan.”`, `”Claim 1 order card: place token on order board, submit required ingredients, take order card, receive
                        // income.”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Klaim 1 kartu pesanan: letakkan token di papan pesanan, serahkan bahan sesuai syarat, ambil kartu pesanan, terima
                            // penghasilan.”` sebagai argumen ke `L`.
                            "Klaim 1 kartu pesanan: letakkan token di papan pesanan, serahkan bahan sesuai syarat, ambil kartu pesanan, terima penghasilan.",
                            // Meneruskan nilai literal `”Claim 1 order card: place token on order board, submit required ingredients, take order card, receive income.”`
                            // sebagai argumen ke `L`.
                            "Claim 1 order card: place token on order board, submit required ingredients, take order card, receive income."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Beli 1 kartu kebutuhan: letakkan token di papan kebutuhan, beli primer lebih dulu sebelum
                        // sekunder/tersier, bayar harga kartu.”`, `”Buy 1 needs card: place token on needs board, buy primary needs before secondary/tertiary, pay card
                        // cost.”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Beli 1 kartu kebutuhan: letakkan token di papan kebutuhan, beli primer lebih dulu sebelum sekunder/tersier, bayar
                            // harga kartu.”` sebagai argumen ke `L`.
                            "Beli 1 kartu kebutuhan: letakkan token di papan kebutuhan, beli primer lebih dulu sebelum sekunder/tersier, bayar harga kartu.",
                            // Meneruskan nilai literal `”Buy 1 needs card: place token on needs board, buy primary needs before secondary/tertiary, pay card cost.”` sebagai
                            // argumen ke `L`.
                            "Buy 1 needs card: place token on needs board, buy primary needs before secondary/tertiary, pay card cost."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Kerja lepas 1x: letakkan token di papan kalender kerja, terima 1 koin dari bank.”`,
                        // `”Freelance once: place token on work-calendar board, receive 1 coin from bank.”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Kerja lepas 1x: letakkan token di papan kalender kerja, terima 1 koin dari bank.”` sebagai argumen ke `L`.
                            "Kerja lepas 1x: letakkan token di papan kalender kerja, terima 1 koin dari bank.",
                            // Meneruskan nilai literal `”Freelance once: place token on work-calendar board, receive 1 coin from bank.”` sebagai argumen ke `L`.
                            "Freelance once: place token on work-calendar board, receive 1 coin from bank."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Slot kosong papan diisi ulang setelah pemain menyelesaikan 2 aksi.”`, `”Empty board slots
                        // are refilled after player completes 2 actions.”` dalam Build.
                        L("Slot kosong papan diisi ulang setelah pemain menyelesaikan 2 aksi.", "Empty board slots are refilled after player completes 2 actions.")
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                    }
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Heading` menggunakan memanggil `L` dengan `”H. Pilihan aksi mode mahir”`, `”H. Advanced-mode action options”` dalam Build.
                    Heading = L("H. Pilihan aksi mode mahir", "H. Advanced-mode action options"),
                    // Memperbarui `Description` menggunakan memanggil `L` dengan `”Aksi tambahan risiko, asuransi, tabungan, dan pinjaman.”`, `”Additional actions for
                    // risk, insurance, savings, and loans.”` dalam Build.
                    Description = L("Aksi tambahan risiko, asuransi, tabungan, dan pinjaman.", "Additional actions for risk, insurance, savings, and loans."),
                    // Memperbarui `Points` menggunakan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya dalam Build.
                    Points = new List<string>
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                    {
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Ambil kartu risiko kehidupan saat melakukan aksi klaim pesanan, lalu jalankan instruksi
                        // kartu.”`, `”Draw a life-risk card when claiming an order, then execute card instructions.”` dalam Build.
                        L("Ambil kartu risiko kehidupan saat melakukan aksi klaim pesanan, lalu jalankan instruksi kartu.", "Draw a life-risk card when claiming an order, then execute card instructions."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Jika kartu risiko menimbulkan biaya, pemain dapat pakai asuransi aktif atau membayar ke
                        // bank.”`, `”If risk card creates cost, player may use active insurance or pay bank.”` dalam Build.
                        L("Jika kartu risiko menimbulkan biaya, pemain dapat pakai asuransi aktif atau membayar ke bank.", "If risk card creates cost, player may use active insurance or pay bank."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Jika koin tidak cukup, pemain dapat menjual kebutuhan (setengah harga), menjual emas (harga
                        // aktif), atau mengambil pinjaman syariah.”`, `”If coins are insufficient, player may sell needs (half price), sell gold (active price), or take
                        // sharia loan.”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Jika koin tidak cukup, pemain dapat menjual kebutuhan (setengah harga), menjual emas (harga aktif), atau mengambil
                            // pinjaman syariah.”` sebagai argumen ke `L`.
                            "Jika koin tidak cukup, pemain dapat menjual kebutuhan (setengah harga), menjual emas (harga aktif), atau mengambil pinjaman syariah.",
                            // Meneruskan nilai literal `”If coins are insufficient, player may sell needs (half price), sell gold (active price), or take sharia loan.”`
                            // sebagai argumen ke `L`.
                            "If coins are insufficient, player may sell needs (half price), sell gold (active price), or take sharia loan."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Aktifkan asuransi multi risiko: letakkan 1 token aksi di papan asuransi dan bank syariah,
                        // bayar premi 1 koin, lalu balik kartu tie breaker ke sisi asuransi.”`, `”Activate multi-risk insurance: place 1 action token on insurance &
                        // sharia-bank board, pay 1-coin premium, then flip tie-breaker card to insurance side.”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Aktifkan asuransi multi risiko: letakkan 1 token aksi di papan asuransi dan bank syariah, bayar premi 1 koin, lalu
                            // balik kartu tie breaker ke sisi asuransi.”` sebagai argumen ke `L`.
                            "Aktifkan asuransi multi risiko: letakkan 1 token aksi di papan asuransi dan bank syariah, bayar premi 1 koin, lalu balik kartu tie breaker ke sisi asuransi.",
                            // Meneruskan nilai literal `”Activate multi-risk insurance: place 1 action token on insurance & sharia-bank board, pay 1-coin premium, then flip
                            // tie-breaker card to insurance side.”` sebagai argumen ke `L`.
                            "Activate multi-risk insurance: place 1 action token on insurance & sharia-bank board, pay 1-coin premium, then flip tie-breaker card to insurance side."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Menabung tujuan keuangan: letakkan token aksi di papan tabungan, setor maksimum 15 koin per
                        // aksi, dan ambil kartu tujuan jika nominal terpenuhi.”`, `”Save toward financial goals: place action token on saving board, deposit up to 15 coins
                        // per action, and claim goal card when amount is met.”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Menabung tujuan keuangan: letakkan token aksi di papan tabungan, setor maksimum 15 koin per aksi, dan ambil kartu
                            // tujuan jika nominal terpenuhi.”` sebagai argumen ke `L`.
                            "Menabung tujuan keuangan: letakkan token aksi di papan tabungan, setor maksimum 15 koin per aksi, dan ambil kartu tujuan jika nominal terpenuhi.",
                            // Meneruskan nilai literal `”Save toward financial goals: place action token on saving board, deposit up to 15 coins per action, and claim goal
                            // card when amount is met.”` sebagai argumen ke `L`.
                            "Save toward financial goals: place action token on saving board, deposit up to 15 coins per action, and claim goal card when amount is met."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Bayar atau pinjam syariah: gunakan 1 token aksi untuk melunasi atau mengambil pinjaman.”`,
                        // `”Repay or borrow sharia loan: use 1 action token to repay or take a loan.”` dalam Build.
                        L("Bayar atau pinjam syariah: gunakan 1 token aksi untuk melunasi atau mengambil pinjaman.", "Repay or borrow sharia loan: use 1 action token to repay or take a loan."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Pinjaman belum lunas di akhir permainan membuat kartu tujuan keuangan hangus dan terkena
                        // penalti poin kebahagiaan.”`, `”Unpaid loan at end game invalidates financial-goal cards and applies happiness point penalty.”` dalam Build.
                        L(
                            // Meneruskan nilai literal `”Pinjaman belum lunas di akhir permainan membuat kartu tujuan keuangan hangus dan terkena penalti poin kebahagiaan.”`
                            // sebagai argumen ke `L`.
                            "Pinjaman belum lunas di akhir permainan membuat kartu tujuan keuangan hangus dan terkena penalti poin kebahagiaan.",
                            // Meneruskan nilai literal `”Unpaid loan at end game invalidates financial-goal cards and applies happiness point penalty.”` sebagai argumen ke
                            // `L`.
                            "Unpaid loan at end game invalidates financial-goal cards and applies happiness point penalty.")
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                    }
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Heading` menggunakan memanggil `L` dengan `”I. Aksi bebas (tanpa token aksi)”`, `”I. Free actions (without action tokens)”` dalam
                    // Build.
                    Heading = L("I. Aksi bebas (tanpa token aksi)", "I. Free actions (without action tokens)"),
                    // Memperbarui `Description` menggunakan memanggil `L` dengan `”Aksi khusus yang dipicu oleh hari kalender.”`, `”Special actions triggered by
                    // calendar days.”` dalam Build.
                    Description = L("Aksi khusus yang dipicu oleh hari kalender.", "Special actions triggered by calendar days."),
                    // Memperbarui `Points` menggunakan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya dalam Build.
                    Points = new List<string>
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                    {
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Peduli donasi (Jumat): semua pemain donasi tertutup minimal 1 koin, buka bersamaan, lalu
                        // tentukan peringkat.”`, `”Donation care (Friday): all players make hidden donations (minimum 1 coin), reveal simultaneously, then rank results.”`
                        // dalam Build.
                        L("Peduli donasi (Jumat): semua pemain donasi tertutup minimal 1 koin, buka bersamaan, lalu tentukan peringkat.", "Donation care (Friday): all players make hidden donations (minimum 1 coin), reveal simultaneously, then rank results."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Poin Kebahagiaan donasi: juara 1 = 7 poin kebahagiaan, juara 2 = 5 poin kebahagiaan, juara 3
                        // = 2 poin kebahagiaan.”`, `”Donation happiness points: rank 1 = 7 happiness points, rank 2 = 5 happiness points, rank 3 = 2 happiness points.”`
                        // dalam Build.
                        L("Poin Kebahagiaan donasi: juara 1 = 7 poin kebahagiaan, juara 2 = 5 poin kebahagiaan, juara 3 = 2 poin kebahagiaan.", "Donation happiness points: rank 1 = 7 happiness points, rank 2 = 5 happiness points, rank 3 = 2 happiness points."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Jika donasi sama, gunakan angka tie breaker terbesar.”`, `”If donations tie, use highest
                        // tie-breaker number.”` dalam Build.
                        L("Jika donasi sama, gunakan angka tie breaker terbesar.", "If donations tie, use highest tie-breaker number."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Investasi emas (Sabtu): buka kartu harga emas, lalu pemain bebas beli atau jual emas sesuai
                        // harga aktif.”`, `”Gold investment (Saturday): reveal gold price card, then players may freely buy/sell gold at active price.”` dalam Build.
                        L("Investasi emas (Sabtu): buka kartu harga emas, lalu pemain bebas beli atau jual emas sesuai harga aktif.", "Gold investment (Saturday): reveal gold price card, then players may freely buy/sell gold at active price."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Pada hari Sabtu, setiap pemain memilih membeli atau menjual satu kali. Jumlah kartu emas
                        // dalam transaksi itu tidak dibatasi.”`, `”On Saturday, each player chooses to buy or sell once. The number of gold cards in that transaction is
                        // unlimited.”` dalam Build.
                        L("Pada hari Sabtu, setiap pemain memilih membeli atau menjual satu kali. Jumlah kartu emas dalam transaksi itu tidak dibatasi.", "On Saturday, each player chooses to buy or sell once. The number of gold cards in that transaction is unlimited.")
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                    }
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Heading` menggunakan memanggil `L` dengan `”J. Catatan arus kas (opsional)”`, `”J. Cashflow notes (optional)”` dalam Build.
                    Heading = L("J. Catatan arus kas (opsional)", "J. Cashflow notes (optional)"),
                    // Memperbarui `Description` menggunakan memanggil `L` dengan `”Format sederhana pencatatan pemasukan dan pengeluaran.”`, `”Simple format for
                    // recording income and expenses.”` dalam Build.
                    Description = L("Format sederhana pencatatan pemasukan dan pengeluaran.", "Simple format for recording income and expenses."),
                    // Memperbarui `Points` menggunakan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya dalam Build.
                    Points = new List<string>
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                    {
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Isi tanggal permainan dan nama pemain.”`, `”Fill game date and player name.”` dalam Build.
                        L("Isi tanggal permainan dan nama pemain.", "Fill game date and player name."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Catat setiap pemasukan pada kolom (+) dan pengeluaran pada kolom (-).”`, `”Record each
                        // income in (+) and each expense in (-) column.”` dalam Build.
                        L("Catat setiap pemasukan pada kolom (+) dan pengeluaran pada kolom (-).", "Record each income in (+) and each expense in (-) column."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Catat donasi Jumat setelah aksi donasi selesai.”`, `”Record Friday donation after donation
                        // action is completed.”` dalam Build.
                        L("Catat donasi Jumat setelah aksi donasi selesai.", "Record Friday donation after donation action is completed."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Catat transaksi emas hari Sabtu: jual = (+), beli = (-).”`, `”Record Saturday gold
                        // transactions: sell = (+), buy = (-).”` dalam Build.
                        L("Catat transaksi emas hari Sabtu: jual = (+), beli = (-).", "Record Saturday gold transactions: sell = (+), buy = (-)."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Lakukan rekap akhir: saldo awal, total pemasukan, total pengeluaran, saldo akhir.”`,
                        // `”Create final recap: starting balance, total income, total expense, ending balance.”` dalam Build.
                        L("Lakukan rekap akhir: saldo awal, total pemasukan, total pengeluaran, saldo akhir.", "Create final recap: starting balance, total income, total expense, ending balance.")
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                    }
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Heading` menggunakan memanggil `L` dengan `”K. Perhitungan poin kebahagiaan”`, `”K. Happiness point calculation”` dalam Build.
                    Heading = L("K. Perhitungan poin kebahagiaan", "K. Happiness point calculation"),
                    // Memperbarui `Description` menggunakan memanggil `L` dengan `”Komponen skor, penalti, dan aturan tie breaker.”`, `”Scoring components, penalties,
                    // and tie-breaker rules.”` dalam Build.
                    Description = L("Komponen skor, penalti, dan aturan tie breaker.", "Scoring components, penalties, and tie-breaker rules."),
                    // Memperbarui `Points` menggunakan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya dalam Build.
                    Points = new List<string>
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                    {
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Jumlahkan seluruh poin kebahagiaan kartu aneka kebutuhan.”`, `”Sum all happiness points from
                        // needs cards.”` dalam Build.
                        L("Jumlahkan seluruh poin kebahagiaan kartu aneka kebutuhan.", "Sum all happiness points from needs cards."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Tambahkan bonus set kebutuhan secara mandiri: 3 jenis berbeda = +4 poin kebahagiaan dan
                        // setiap 3 jenis sama = +2 poin kebahagiaan.”`, `”Count each needs-set pattern independently: 3 different types = +4 happiness points and each 3
                        // same types = +2 happiness points.”` dalam Build.
                        L("Tambahkan bonus set kebutuhan secara mandiri: 3 jenis berbeda = +4 poin kebahagiaan dan setiap 3 jenis sama = +2 poin kebahagiaan.", "Count each needs-set pattern independently: 3 different types = +4 happiness points and each 3 same types = +2 happiness points."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Tambahkan poin kebahagiaan juara donasi dan poin kebahagiaan investasi emas.”`, `”Add
                        // donation champion happiness points and gold investment happiness points.”` dalam Build.
                        L("Tambahkan poin kebahagiaan juara donasi dan poin kebahagiaan investasi emas.", "Add donation champion happiness points and gold investment happiness points."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Tambahkan poin kebahagiaan juara dana pensiun berdasarkan sisa koin akhir.”`, `”Add pension
                        // champion happiness points based on remaining end-game coins.”` dalam Build.
                        L("Tambahkan poin kebahagiaan juara dana pensiun berdasarkan sisa koin akhir.", "Add pension champion happiness points based on remaining end-game coins."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Tambahkan poin kebahagiaan kartu tujuan keuangan pada mode mahir hanya jika pinjaman
                        // lunas.”`, `”Add financial-goal card happiness points in advanced mode only if loans are repaid.”` dalam Build.
                        L("Tambahkan poin kebahagiaan kartu tujuan keuangan pada mode mahir hanya jika pinjaman lunas.", "Add financial-goal card happiness points in advanced mode only if loans are repaid."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Kurangi penalti misi koleksi gagal sebesar 10 poin kebahagiaan.”`, `”Subtract 10 happiness
                        // points for failed collection mission.”` dalam Build.
                        L("Kurangi penalti misi koleksi gagal sebesar 10 poin kebahagiaan.", "Subtract 10 happiness points for failed collection mission."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Kurangi penalti pinjaman syariah belum lunas sebesar 15 poin kebahagiaan.”`, `”Subtract 15
                        // happiness points for unpaid sharia loans.”` dalam Build.
                        L("Kurangi penalti pinjaman syariah belum lunas sebesar 15 poin kebahagiaan.", "Subtract 15 happiness points for unpaid sharia loans."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Jika total poin kebahagiaan sama, pemenang ditentukan oleh angka tie breaker terbesar.”`,
                        // `”If total happiness points tie, winner is determined by highest tie-breaker number.”` dalam Build.
                        L("Jika total poin kebahagiaan sama, pemenang ditentukan oleh angka tie breaker terbesar.", "If total happiness points tie, winner is determined by highest tie-breaker number.")
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                    }
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Heading` menggunakan memanggil `L` dengan `”L. Ucapan terima kasih”`, `”L. Acknowledgements”` dalam Build.
                    Heading = L("L. Ucapan terima kasih", "L. Acknowledgements"),
                    // Memperbarui `Description` menggunakan memanggil `L` dengan `”Apresiasi kontributor.”`, `”Contributor appreciation.”` dalam Build.
                    Description = L("Apresiasi kontributor.", "Contributor appreciation."),
                    // Memperbarui `Points` menggunakan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya dalam Build.
                    Points = new List<string>
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                    {
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Daftar lengkap ucapan terima kasih mengikuti dokumen rulebook halaman 28.”`, `”Full
                        // acknowledgements are listed in rulebook page 28.”` dalam Build.
                        L("Daftar lengkap ucapan terima kasih mengikuti dokumen rulebook halaman 28.", "Full acknowledgements are listed in rulebook page 28."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Mencakup keluarga, mentor, komunitas, akademisi, dan kolaborator board game.”`, `”Includes
                        // family, mentors, community, academics, and board-game collaborators.”` dalam Build.
                        L("Mencakup keluarga, mentor, komunitas, akademisi, dan kolaborator board game.", "Includes family, mentors, community, academics, and board-game collaborators.")
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                    }
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Heading` menggunakan memanggil `L` dengan `”M. Segera hadir”`, `”M. Coming soon”` dalam Build.
                    Heading = L("M. Segera hadir", "M. Coming soon"),
                    // Memperbarui `Description` menggunakan memanggil `L` dengan `”Cashflowpoly: Investor Edition.”`, `”Cashflowpoly: Investor Edition.”` dalam Build.
                    Description = L("Cashflowpoly: Investor Edition.", "Cashflowpoly: Investor Edition."),
                    // Memperbarui `Points` menggunakan objek baru bertipe `List<string>` dengan nilai awal sesuai konstruktornya dalam Build.
                    Points = new List<string>
                    // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                    {
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Fokus tema: investor (deposito, obligasi, saham, mata uang kripto).”`, `”Theme focus:
                        // investor (deposits, bonds, stocks, cryptocurrency).”` dalam Build.
                        L("Fokus tema: investor (deposito, obligasi, saham, mata uang kripto).", "Theme focus: investor (deposits, bonds, stocks, cryptocurrency)."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Tujuan: mendukung keuangan keluarga untuk mencapai kebahagiaan.”`, `”Goal: support family
                        // finance to achieve happiness.”` dalam Build.
                        L("Tujuan: mendukung keuangan keluarga untuk mencapai kebahagiaan.", "Goal: support family finance to achieve happiness."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Video tutorial: Instagram @CASHFLOWPOLY.”`, `”Tutorial videos: Instagram @CASHFLOWPOLY.”`
                        // dalam Build.
                        L("Video tutorial: Instagram @CASHFLOWPOLY.", "Tutorial videos: Instagram @CASHFLOWPOLY."),
                        // Melanjutkan pengolahan dengan memanggil `L` dengan `”Kontak kerja sama: adhicipta.playground@gmail.com.”`, `”Partnership contact:
                        // adhicipta.playground@gmail.com.”` dalam Build.
                        L("Kontak kerja sama: adhicipta.playground@gmail.com.", "Partnership contact: adhicipta.playground@gmail.com.")
                    // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                    }
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                }
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
            },
            // Memperbarui `Scoring` menggunakan objek baru bertipe `List<RulebookScoreItemViewModel>` dengan nilai awal sesuai konstruktornya dalam Build.
            Scoring = new List<RulebookScoreItemViewModel>
            // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
            {
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Category` menggunakan memanggil `L` dengan `”Poin Kebahagiaan Kebutuhan”`, `”Need Happiness Points”` dalam Build.
                    Category = L("Poin Kebahagiaan Kebutuhan", "Need Happiness Points"),
                    // Memperbarui `Rule` menggunakan memanggil `L` dengan `”Jumlah poin kebahagiaan seluruh kartu aneka kebutuhan.”`, `”Total happiness points from all
                    // needs cards.”` dalam Build.
                    Rule = L("Jumlah poin kebahagiaan seluruh kartu aneka kebutuhan.", "Total happiness points from all needs cards.")
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Category` menggunakan memanggil `L` dengan `”Bonus Set Kebutuhan”`, `”Need Set Bonus”` dalam Build.
                    Category = L("Bonus Set Kebutuhan", "Need Set Bonus"),
                    // Memperbarui `Rule` menggunakan memanggil `L` dengan `”Hitung mandiri: 3 jenis berbeda = +4 poin kebahagiaan; setiap 3 jenis sama = +2 poin
                    // kebahagiaan.”`, `”Count independently: 3 different types = +4 happiness points; each 3 same types = +2 happiness points.”` dalam Build.
                    Rule = L("Hitung mandiri: 3 jenis berbeda = +4 poin kebahagiaan; setiap 3 jenis sama = +2 poin kebahagiaan.", "Count independently: 3 different types = +4 happiness points; each 3 same types = +2 happiness points.")
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Category` menggunakan memanggil `L` dengan `”Poin Kebahagiaan Juara Donasi”`, `”Donation Champion Happiness Points”` dalam Build.
                    Category = L("Poin Kebahagiaan Juara Donasi", "Donation Champion Happiness Points"),
                    // Memperbarui `Rule` menggunakan memanggil `L` dengan `”Akumulasi poin kebahagiaan dari kartu juara donasi.”`, `”Accumulated happiness points from
                    // donation champion card.”` dalam Build.
                    Rule = L("Akumulasi poin kebahagiaan dari kartu juara donasi.", "Accumulated happiness points from donation champion card.")
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Category` menggunakan memanggil `L` dengan `”Poin Kebahagiaan Investasi Emas”`, `”Gold Investment Happiness Points”` dalam Build.
                    Category = L("Poin Kebahagiaan Investasi Emas", "Gold Investment Happiness Points"),
                    // Memperbarui `Rule` menggunakan memanggil `L` dengan `”1 emas = 3, 2 emas = 5, 3 emas = 8, 4 emas = 12.”`, `”1 gold = 3, 2 gold = 5, 3 gold = 8, 4
                    // gold = 12.”` dalam Build.
                    Rule = L("1 emas = 3, 2 emas = 5, 3 emas = 8, 4 emas = 12.", "1 gold = 3, 2 gold = 5, 3 gold = 8, 4 gold = 12.")
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Category` menggunakan memanggil `L` dengan `”Poin Kebahagiaan Juara Dana Pensiun”`, `”Pension Champion Happiness Points”` dalam
                    // Build.
                    Category = L("Poin Kebahagiaan Juara Dana Pensiun", "Pension Champion Happiness Points"),
                    // Memperbarui `Rule` menggunakan memanggil `L` dengan `”Hitung dari sisa koin akhir, sisa bahan masakan, dan sisa tabungan; juara 1 = +5, juara 2 =
                    // +3, juara 3 = +1.”`, `”Count ending cash, leftover ingredient cards, and remaining savings; rank 1 = +5, rank 2 = +3, rank 3 = +1.”` dalam Build.
                    Rule = L("Hitung dari sisa koin akhir, sisa bahan masakan, dan sisa tabungan; juara 1 = +5, juara 2 = +3, juara 3 = +1.", "Count ending cash, leftover ingredient cards, and remaining savings; rank 1 = +5, rank 2 = +3, rank 3 = +1.")
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Category` menggunakan memanggil `L` dengan `”Poin Kebahagiaan Tujuan Keuangan”`, `”Financial Goal Happiness Points”` dalam Build.
                    Category = L("Poin Kebahagiaan Tujuan Keuangan", "Financial Goal Happiness Points"),
                    // Memperbarui `Rule` menggunakan memanggil `L` dengan `”Hanya dihitung di mode mahir bila seluruh pinjaman syariah sudah lunas.”`, `”Count only in
                    // advanced mode when every sharia loan is fully repaid.”` dalam Build.
                    Rule = L("Hanya dihitung di mode mahir bila seluruh pinjaman syariah sudah lunas.", "Count only in advanced mode when every sharia loan is fully repaid.")
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Category` menggunakan memanggil `L` dengan `”Minus Misi Koleksi”`, `”Collection Mission Penalty”` dalam Build.
                    Category = L("Minus Misi Koleksi", "Collection Mission Penalty"),
                    // Memperbarui `Rule` menggunakan memanggil `L` dengan `”Minus 10 poin kebahagiaan bila misi koleksi tidak terpenuhi.”`, `”Subtract 10 happiness
                    // points when the collection mission is not completed.”` dalam Build.
                    Rule = L("Minus 10 poin kebahagiaan bila misi koleksi tidak terpenuhi.", "Subtract 10 happiness points when the collection mission is not completed.")
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Category` menggunakan memanggil `L` dengan `”Minus Pinjaman Syariah”`, `”Sharia Loan Penalty”` dalam Build.
                    Category = L("Minus Pinjaman Syariah", "Sharia Loan Penalty"),
                    // Memperbarui `Rule` menggunakan memanggil `L` dengan `”Penalti tetap -15 poin kebahagiaan untuk pinjaman yang belum lunas.”`, `”Fixed -15
                    // happiness point penalty for unpaid loans.”` dalam Build.
                    Rule = L("Penalti tetap -15 poin kebahagiaan untuk pinjaman yang belum lunas.", "Fixed -15 happiness point penalty for unpaid loans.")
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                },
                // Menggunakan objek baru dengan tipe mengikuti konteks tujuan dan argumen () sebagai bagian ekspresi yang sedang disusun dalam Build.
                new()
                // Membuka scope initializer yang mengisi objek atau koleksi; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam Build.
                {
                    // Memperbarui `Category` menggunakan memanggil `L` dengan `”Tie Breaker”`, `”Tie Breaker”` dalam Build.
                    Category = L("Tie Breaker", "Tie Breaker"),
                    // Memperbarui `Rule` menggunakan memanggil `L` dengan `”Jika total poin kebahagiaan sama, pemenang ditentukan oleh angka terbesar pada kartu tie
                    // breaker pemain.”`, `”If total happiness points are tied, the player with the highest tie-breaker card number wins.”` dalam Build.
                    Rule = L("Jika total poin kebahagiaan sama, pemenang ditentukan oleh angka terbesar pada kartu tie breaker pemain.", "If total happiness points are tied, the player with the highest tie-breaker card number wins.")
                // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
                }
            // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
            }
        // Menutup scope initializer yang mengisi objek atau koleksi; bagian berikut berada di luar batas blok tersebut dalam Build.
        };
    // Menutup scope metode Build; bagian berikut berada di luar batas blok tersebut dalam Build.
    }
// Menutup scope tipe RulebookContent; bagian berikut berada di luar batas blok tersebut.
}
