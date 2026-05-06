// Fungsi file: Menyimpan potongan leksikon UI domain Rulebook.
namespace Cashflowpoly.Ui.Infrastructure;

internal static partial class UiTextLexicon
{
    private static partial void AddRulebook(Dictionary<string, (string Id, string En)> terms)
    {
        terms["privacy.title"] = ("Buku Aturan Cashflowpoly", "Cashflowpoly Rulebook");
        terms["privacy.outline_title"] = ("Daftar Isi Aturan", "Rulebook Contents");
        terms["privacy.total_sections"] = ("Jumlah Bagian Aturan", "Total Rulebook Sections");
        terms["privacy.total_points"] = ("Jumlah Aturan Skoring", "Total Scoring Rules");
        terms["privacy.back_to_top"] = ("Kembali ke atas", "Back to top");
        terms["privacy.empty_sections"] = ("Bagian Buku Aturan belum tersedia.", "Rulebook sections are not available yet.");
        terms["privacy.empty_scoring"] = ("Ringkasan poin belum tersedia.", "Scoring summary is not available yet.");
        terms["privacy.happiness_summary"] = ("Ringkasan Poin Kebahagiaan", "Happiness Points Summary");
        terms["privacy.copyright_title"] = ("Hak Cipta Buku Aturan", "Rulebook Copyright");
        terms["privacy.copyright_desc"] = ("Materi Buku Aturan Cashflowpoly digunakan untuk keperluan pembelajaran internal. Dilarang menyalin, menggandakan, atau mendistribusikan ulang tanpa izin tertulis dari pemilik hak cipta.", "Cashflowpoly rulebook materials are for internal learning purposes. Copying, reproducing, or redistributing without written permission from the copyright owner is prohibited.");
        terms["privacy.category"] = ("Kategori", "Category");
        terms["privacy.rule"] = ("Aturan", "Rule");
    }
}
