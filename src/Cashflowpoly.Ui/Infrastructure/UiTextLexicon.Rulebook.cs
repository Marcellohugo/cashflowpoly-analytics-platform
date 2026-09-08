// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui UiTextLexicon.Rulebook.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

// Mendefinisikan tipe class `UiTextLexicon`.
internal static partial class UiTextLexicon
// Membuka scope tipe UiTextLexicon; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `AddRulebook` dengan hasil bertipe `void`; operasi ini menangani add rulebook. Masukan: Parameter `terms` bertipe
    // `Dictionary<string, (string Id, string En)>` membawa nilai terms.
    private static partial void AddRulebook(Dictionary<string, (string Id, string En)> terms)
    // Membuka scope metode AddRulebook; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AddRulebook.
    {
        // Memperbarui `terms[”rulebook.title”]` menggunakan tuple yang membawa bagian 1: ”Buku Aturan Cashflowpoly”; bagian 2: ”Cashflowpoly Rulebook”
        // dalam AddRulebook.
        terms["rulebook.title"] = ("Buku Aturan Cashflowpoly", "Cashflowpoly Rulebook");
        // Memperbarui `terms[”privacy.outline_title”]` menggunakan tuple yang membawa bagian 1: ”Daftar Isi Aturan”; bagian 2: ”Rulebook Contents” dalam
        // AddRulebook.
        terms["privacy.outline_title"] = ("Daftar Isi Aturan", "Rulebook Contents");
        // Memperbarui `terms[”privacy.total_sections”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Bagian Aturan”; bagian 2: ”Total Rulebook
        // Sections” dalam AddRulebook.
        terms["privacy.total_sections"] = ("Jumlah Bagian Aturan", "Total Rulebook Sections");
        // Memperbarui `terms[”privacy.total_points”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Aturan Skoring”; bagian 2: ”Total Scoring Rules”
        // dalam AddRulebook.
        terms["privacy.total_points"] = ("Jumlah Aturan Skoring", "Total Scoring Rules");
        // Memperbarui `terms[”privacy.back_to_top”]` menggunakan tuple yang membawa bagian 1: ”Kembali ke atas”; bagian 2: ”Back to top” dalam AddRulebook.
        terms["privacy.back_to_top"] = ("Kembali ke atas", "Back to top");
        // Memperbarui `terms[”privacy.empty_sections”]` menggunakan tuple yang membawa bagian 1: ”Bagian Buku Aturan belum tersedia.”; bagian 2: ”Rulebook
        // sections are not available yet.” dalam AddRulebook.
        terms["privacy.empty_sections"] = ("Bagian Buku Aturan belum tersedia.", "Rulebook sections are not available yet.");
        // Memperbarui `terms[”privacy.empty_scoring”]` menggunakan tuple yang membawa bagian 1: ”Ringkasan poin kebahagiaan belum tersedia.”; bagian 2:
        // ”Scoring summary is not available yet.” dalam AddRulebook.
        terms["privacy.empty_scoring"] = ("Ringkasan poin kebahagiaan belum tersedia.", "Scoring summary is not available yet.");
        // Memperbarui `terms[”privacy.happiness_summary”]` menggunakan tuple yang membawa bagian 1: ”Ringkasan Poin Kebahagiaan”; bagian 2: ”Happiness
        // Points Summary” dalam AddRulebook.
        terms["privacy.happiness_summary"] = ("Ringkasan Poin Kebahagiaan", "Happiness Points Summary");
        // Memperbarui `terms[”privacy.copyright_title”]` menggunakan tuple yang membawa bagian 1: ”Hak Cipta Buku Aturan”; bagian 2: ”Rulebook Copyright”
        // dalam AddRulebook.
        terms["privacy.copyright_title"] = ("Hak Cipta Buku Aturan", "Rulebook Copyright");
        // Memperbarui `terms[”privacy.copyright_desc”]` menggunakan tuple yang membawa bagian 1: ”Materi Buku Aturan Cashflowpoly digunakan untuk keperluan
        // pembelajaran internal...; bagian 2: ”Cashflowpoly rulebook materials are for internal learning purposes. Copying, re... dalam AddRulebook.
        terms["privacy.copyright_desc"] = ("Materi Buku Aturan Cashflowpoly digunakan untuk keperluan pembelajaran internal. Dilarang menyalin, menggandakan, atau mendistribusikan ulang tanpa izin tertulis dari pemilik hak cipta.", "Cashflowpoly rulebook materials are for internal learning purposes. Copying, reproducing, or redistributing without written permission from the copyright owner is prohibited.");
        // Memperbarui `terms[”privacy.category”]` menggunakan tuple yang membawa bagian 1: ”Kategori”; bagian 2: ”Category” dalam AddRulebook.
        terms["privacy.category"] = ("Kategori", "Category");
        // Memperbarui `terms[”privacy.rule”]` menggunakan tuple yang membawa bagian 1: ”Aturan”; bagian 2: ”Rule” dalam AddRulebook.
        terms["privacy.rule"] = ("Aturan", "Rule");
    // Menutup scope metode AddRulebook; bagian berikut berada di luar batas blok tersebut dalam AddRulebook.
    }
// Menutup scope tipe UiTextLexicon; bagian berikut berada di luar batas blok tersebut.
}
