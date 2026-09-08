// Fungsi file: Menyediakan transformasi, lokalisasi, atau koneksi UI melalui UiTextLexicon.Rulesets.
// Menempatkan deklarasi pada namespace `Cashflowpoly.Ui.Infrastructure` untuk mengelompokkan komponen dan mencegah benturan nama tipe.
namespace Cashflowpoly.Ui.Infrastructure;

// Mendefinisikan tipe class `UiTextLexicon`.
internal static partial class UiTextLexicon
// Membuka scope tipe UiTextLexicon; pernyataan/deklarasi berikut berada di dalam batas blok ini.
{
    // Mendefinisikan metode `AddRulesets` dengan hasil bertipe `void`; operasi ini menangani add aturan. Masukan: Parameter `terms` bertipe
    // `Dictionary<string, (string Id, string En)>` membawa nilai terms.
    private static partial void AddRulesets(Dictionary<string, (string Id, string En)> terms)
    // Membuka scope metode AddRulesets; pernyataan/deklarasi berikut berada di dalam batas blok ini dalam AddRulesets.
    {
        // Memperbarui `terms[”analytics.tips.instructor”]` menggunakan tuple yang membawa bagian 1: ”Tips membaca analitika sesi”; bagian 2: ”Session
        // analytics tips” dalam AddRulesets.
        terms["analytics.tips.instructor"] = ("Tips membaca analitika sesi", "Session analytics tips");
        // Memperbarui `terms[”analytics.tips.player”]` menggunakan tuple yang membawa bagian 1: ”Tips membaca progres analitika Anda”; bagian 2: ”Your
        // analytics progress tips” dalam AddRulesets.
        terms["analytics.tips.player"] = ("Tips membaca progres analitika Anda", "Your analytics progress tips");
        // Memperbarui `terms[”analytics.tip_key.input”]` menggunakan tuple yang membawa bagian 1: ”Masukan”; bagian 2: ”Input” dalam AddRulesets.
        terms["analytics.tip_key.input"] = ("Masukan", "Input");
        // Memperbarui `terms[”analytics.tip_key.interpretation”]` menggunakan tuple yang membawa bagian 1: ”Interpretasi”; bagian 2: ”Interpretation” dalam
        // AddRulesets.
        terms["analytics.tip_key.interpretation"] = ("Interpretasi", "Interpretation");
        // Memperbarui `terms[”analytics.tip_key.followup”]` menggunakan tuple yang membawa bagian 1: ”Tindak lanjut”; bagian 2: ”Follow-up” dalam
        // AddRulesets.
        terms["analytics.tip_key.followup"] = ("Tindak lanjut", "Follow-up");
        // Memperbarui `terms[”analytics.tip_key.select_session”]` menggunakan tuple yang membawa bagian 1: ”Pilih Sesi”; bagian 2: ”Select Session” dalam
        // AddRulesets.
        terms["analytics.tip_key.select_session"] = ("Pilih Sesi", "Select Session");
        // Memperbarui `terms[”analytics.tip_key.focus_score”]` menggunakan tuple yang membawa bagian 1: ”Fokus Skor”; bagian 2: ”Score Focus” dalam
        // AddRulesets.
        terms["analytics.tip_key.focus_score"] = ("Fokus Skor", "Score Focus");
        // Memperbarui `terms[”analytics.tip_key.risk_control”]` menggunakan tuple yang membawa bagian 1: ”Kontrol Risiko”; bagian 2: ”Risk Control” dalam
        // AddRulesets.
        terms["analytics.tip_key.risk_control"] = ("Kontrol Risiko", "Risk Control");
        // Memperbarui `terms[”analytics.tip.input”]` menggunakan tuple yang membawa bagian 1: ”Cocokkan status, mode, dan Set Aturan aktif agar hasil
        // dibaca dengan konteks se...; bagian 2: ”Match status, mode, and the active ruleset so results are read in the correct s... dalam AddRulesets.
        terms["analytics.tip.input"] = ("Cocokkan status, mode, dan Set Aturan aktif agar hasil dibaca dengan konteks sesi yang benar.", "Match status, mode, and the active ruleset so results are read in the correct session context.");
        // Memperbarui `terms[”analytics.tip.interpretation”]` menggunakan tuple yang membawa bagian 1: ”Mulai dari ringkasan hasil, lalu gunakan tabel
        // pemain untuk menemukan perubahan...; bagian 2: ”Start with the result summary, then use the player table to find the changes mo... dalam
        // AddRulesets.
        terms["analytics.tip.interpretation"] = ("Mulai dari ringkasan hasil, lalu gunakan tabel pemain untuk menemukan perubahan yang paling perlu dibahas.", "Start with the result summary, then use the player table to find the changes most worth discussing.");
        // Memperbarui `terms[”analytics.tip.followup”]` menggunakan tuple yang membawa bagian 1: ”Telusuri urutan aktivitas untuk mengetahui kapan
        // perubahan terjadi, lalu buka A...; bagian 2: ”Trace the activity order to learn when a change occurred, then open Player Anal... dalam
        // AddRulesets.
        terms["analytics.tip.followup"] = ("Telusuri urutan aktivitas untuk mengetahui kapan perubahan terjadi, lalu buka Analitika Pemain untuk melihat penyebabnya.", "Trace the activity order to learn when a change occurred, then open Player Analytics to inspect its cause.");
        // Memperbarui `terms[”analytics.tip.select_session”]` menggunakan tuple yang membawa bagian 1: ”Pastikan nama, status, dan Set Aturan aktif sesuai
        // dengan sesi yang ingin Anda ...; bagian 2: ”Confirm that the name, status, and active ruleset match the session you want to... dalam AddRulesets.
        terms["analytics.tip.select_session"] = ("Pastikan nama, status, dan Set Aturan aktif sesuai dengan sesi yang ingin Anda evaluasi.", "Confirm that the name, status, and active ruleset match the session you want to evaluate.");
        // Memperbarui `terms[”analytics.tip.focus_score”]` menggunakan tuple yang membawa bagian 1: ”Baca arus kas bersama poin kebahagiaan kebutuhan dan
        // target tabungan agar penin...; bagian 2: ”Read cashflow alongside need and saving-goal scores so score gains do not hide ... dalam AddRulesets.
        terms["analytics.tip.focus_score"] = ("Baca arus kas bersama poin kebahagiaan kebutuhan dan target tabungan agar peningkatan skor tidak menutupi tekanan keuangan.", "Read cashflow alongside need and saving-goal scores so score gains do not hide financial pressure.");
        // Memperbarui `terms[”analytics.tip.risk_control”]` menggunakan tuple yang membawa bagian 1: ”Periksa penalti dan pinjaman belum lunas, lalu
        // cocokkan dengan aktivitas yang m...; bagian 2: ”Review penalties and unpaid loans, then match them to the activities that creat... dalam
        // AddRulesets.
        terms["analytics.tip.risk_control"] = ("Periksa penalti dan pinjaman belum lunas, lalu cocokkan dengan aktivitas yang memicu beban tersebut.", "Review penalties and unpaid loans, then match them to the activities that created the burden.");
        // Memperbarui `terms[”rulesets.management”]` menggunakan tuple yang membawa bagian 1: ”Referensi Set Aturan”; bagian 2: ”Ruleset Reference” dalam
        // AddRulesets.
        terms["rulesets.management"] = ("Referensi Set Aturan", "Ruleset Reference");
        // Memperbarui `terms[”rulesets.title”]` menggunakan tuple yang membawa bagian 1: ”Daftar Set Aturan Permainan”; bagian 2: ”Game Ruleset List” dalam
        // AddRulesets.
        terms["rulesets.title"] = ("Daftar Set Aturan Permainan", "Game Ruleset List");
        // Memperbarui `terms[”rulesets.subtitle”]` menggunakan tuple yang membawa bagian 1: ”Baca aturan setup, alur aksi, dan scoring yang dipakai sesi
        // permainan.”; bagian 2: ”Read the setup rules, action flow, and scoring used by gameplay sessions.” dalam AddRulesets.
        terms["rulesets.subtitle"] = ("Baca aturan setup, alur aksi, dan scoring yang dipakai sesi permainan.", "Read the setup rules, action flow, and scoring used by gameplay sessions.");
        // Memperbarui `terms[”rulesets.total_rulesets”]` menggunakan tuple yang membawa bagian 1: ”Total Set Aturan”; bagian 2: ”Total rulesets” dalam
        // AddRulesets.
        terms["rulesets.total_rulesets"] = ("Total Set Aturan", "Total rulesets");
        // Memperbarui `terms[”rulesets.unified_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Gunakan daftar ini untuk mengecek riwayat, versi, dan
        // komponen setiap Set Atura...; bagian 2: ”Use this list to review the history, versions, and components of each ruleset.” dalam AddRulesets.
        terms["rulesets.unified_subtitle"] = ("Gunakan daftar ini untuk mengecek riwayat, versi, dan komponen setiap Set Aturan.", "Use this list to review the history, versions, and components of each ruleset.");
        // Memperbarui `terms[”rulesets.index.summary_title”]` menggunakan tuple yang membawa bagian 1: ”Ringkasan Set Aturan”; bagian 2: ”Ruleset Summary”
        // dalam AddRulesets.
        terms["rulesets.index.summary_title"] = ("Ringkasan Set Aturan", "Ruleset Summary");
        // Memperbarui `terms[”rulesets.index.quick_tools_help”]` menggunakan tuple yang membawa bagian 1: ”Gunakan filter ini untuk menemukan ruleset
        // berdasarkan nama, mode, atau versi.”; bagian 2: ”Use these filters to find rulesets by name, mode, or version.” dalam AddRulesets.
        terms["rulesets.index.quick_tools_help"] = ("Gunakan filter ini untuk menemukan ruleset berdasarkan nama, mode, atau versi.", "Use these filters to find rulesets by name, mode, or version.");
        // Memperbarui `terms[”rulesets.index.list_title”]` menggunakan tuple yang membawa bagian 1: ”Daftar set aturan di workspace Anda”; bagian 2:
        // ”Rulesets in your workspace” dalam AddRulesets.
        terms["rulesets.index.list_title"] = ("Daftar set aturan di workspace Anda", "Rulesets in your workspace");
        // Memperbarui `terms[”rulesets.index.list_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Buka rincian untuk melihat riwayat versi, komponen,
        // dan ringkasan konfigurasi.”; bagian 2: ”Open details to review version history, components, and configuration summary.” dalam AddRulesets.
        terms["rulesets.index.list_subtitle"] = ("Buka rincian untuk melihat riwayat versi, komponen, dan ringkasan konfigurasi.", "Open details to review version history, components, and configuration summary.");
        // Memperbarui `terms[”rulesets.index.default_help”]` menggunakan tuple yang membawa bagian 1: ”Katalog ini berisi susunan komponen bawaan sebagai
        // acuan pembacaan mode Pemula ...; bagian 2: ”This catalog provides built-in component layouts as references for reading Begi... dalam AddRulesets.
        terms["rulesets.index.default_help"] = ("Katalog ini berisi susunan komponen bawaan sebagai acuan pembacaan mode Pemula dan Mahir.", "This catalog provides built-in component layouts as references for reading Beginner and Advanced modes.");
        // Memperbarui `terms[”rulesets.index.bulk_hint”]` menggunakan tuple yang membawa bagian 1: ”Perubahan Set Aturan hanya tersedia bagi instruktur
        // yang berwenang.”; bagian 2: ”Ruleset changes are available only to authorized instructors.” dalam AddRulesets.
        terms["rulesets.index.bulk_hint"] = ("Perubahan Set Aturan hanya tersedia bagi instruktur yang berwenang.", "Ruleset changes are available only to authorized instructors.");
        // Memperbarui `terms[”rulesets.create”]` menggunakan tuple yang membawa bagian 1: ”Buat Set Aturan Baru”; bagian 2: ”Create New Ruleset” dalam
        // AddRulesets.
        terms["rulesets.create"] = ("Buat Set Aturan Baru", "Create New Ruleset");
        // Memperbarui `terms[”rulesets.default_badge”]` menggunakan tuple yang membawa bagian 1: ”Bawaan”; bagian 2: ”Built-in” dalam AddRulesets.
        terms["rulesets.default_badge"] = ("Bawaan", "Built-in");
        // Memperbarui `terms[”rulesets.create_title”]` menggunakan tuple yang membawa bagian 1: ”Buat Set Aturan baru”; bagian 2: ”Create new ruleset”
        // dalam AddRulesets.
        terms["rulesets.create_title"] = ("Buat Set Aturan baru", "Create new ruleset");
        // Memperbarui `terms[”rulesets.create_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Isi form aturan permainan. JSON konfigurasi dibuat
        // otomatis.”; bagian 2: ”Fill the gameplay rules form. Configuration JSON is generated automatically.” dalam AddRulesets.
        terms["rulesets.create_subtitle"] = ("Isi form aturan permainan. JSON konfigurasi dibuat otomatis.", "Fill the gameplay rules form. Configuration JSON is generated automatically.");
        // Memperbarui `terms[”rulesets.edit”]` menggunakan tuple yang membawa bagian 1: ”Edit aturan”; bagian 2: ”Edit rules” dalam AddRulesets.
        terms["rulesets.edit"] = ("Edit aturan", "Edit rules");
        // Memperbarui `terms[”rulesets.edit_title”]` menggunakan tuple yang membawa bagian 1: ”Edit aturan Set Aturan”; bagian 2: ”Edit ruleset rules”
        // dalam AddRulesets.
        terms["rulesets.edit_title"] = ("Edit aturan Set Aturan", "Edit ruleset rules");
        // Memperbarui `terms[”rulesets.edit_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Perbarui aturan untuk membuat versi baru tanpa mengubah
        // riwayat versi lama.”; bagian 2: ”Update rules to create a new version without changing previous version history.... dalam AddRulesets.
        terms["rulesets.edit_subtitle"] = ("Perbarui aturan untuk membuat versi baru tanpa mengubah riwayat versi lama.", "Update rules to create a new version without changing previous version history.");
        // Memperbarui `terms[”rulesets.delete”]` menggunakan tuple yang membawa bagian 1: ”Hapus”; bagian 2: ”Delete” dalam AddRulesets.
        terms["rulesets.delete"] = ("Hapus", "Delete");
        // Memperbarui `terms[”rulesets.save”]` menggunakan tuple yang membawa bagian 1: ”Simpan Set Aturan”; bagian 2: ”Save Ruleset” dalam AddRulesets.
        terms["rulesets.save"] = ("Simpan Set Aturan", "Save Ruleset");
        // Memperbarui `terms[”rulesets.update”]` menggunakan tuple yang membawa bagian 1: ”Simpan Versi Baru”; bagian 2: ”Save New Version” dalam
        // AddRulesets.
        terms["rulesets.update"] = ("Simpan Versi Baru", "Save New Version");
        // Memperbarui `terms[”rulesets.set_active”]` menggunakan tuple yang membawa bagian 1: ”Aktifkan”; bagian 2: ”Set Active” dalam AddRulesets.
        terms["rulesets.set_active"] = ("Aktifkan", "Set Active");
        // Memperbarui `terms[”rulesets.delete_version”]` menggunakan tuple yang membawa bagian 1: ”Hapus Versi”; bagian 2: ”Delete Version” dalam
        // AddRulesets.
        terms["rulesets.delete_version"] = ("Hapus Versi", "Delete Version");
        // Memperbarui `terms[”rulesets.delete_confirm”]` menggunakan tuple yang membawa bagian 1: ”Hapus set aturan ini?”; bagian 2: ”Delete this ruleset?”
        // dalam AddRulesets.
        terms["rulesets.delete_confirm"] = ("Hapus set aturan ini?", "Delete this ruleset?");
        // Memperbarui `terms[”rulesets.delete_version_confirm”]` menggunakan tuple yang membawa bagian 1: ”Hapus versi ini dari riwayat?”; bagian 2:
        // ”Delete this version from history?” dalam AddRulesets.
        terms["rulesets.delete_version_confirm"] = ("Hapus versi ini dari riwayat?", "Delete this version from history?");
        // Memperbarui `terms[”rulesets.delete_version_success”]` menggunakan tuple yang membawa bagian 1: ”Versi {version} berhasil dihapus.”; bagian 2:
        // ”Version {version} was deleted successfully.” dalam AddRulesets.
        terms["rulesets.delete_version_success"] = ("Versi {version} berhasil dihapus.", "Version {version} was deleted successfully.");
        // Memperbarui `terms[”rulesets.bulk_delete”]` menggunakan tuple yang membawa bagian 1: ”Hapus Terpilih”; bagian 2: ”Delete Selected” dalam
        // AddRulesets.
        terms["rulesets.bulk_delete"] = ("Hapus Terpilih", "Delete Selected");
        // Memperbarui `terms[”rulesets.bulk_delete_confirm”]` menggunakan tuple yang membawa bagian 1: ”Hapus semua set aturan terpilih?”; bagian 2:
        // ”Delete all selected rulesets?” dalam AddRulesets.
        terms["rulesets.bulk_delete_confirm"] = ("Hapus semua set aturan terpilih?", "Delete all selected rulesets?");
        // Memperbarui `terms[”rulesets.bulk_delete_success”]` menggunakan tuple yang membawa bagian 1: ”Berhasil menghapus {count} set aturan.”; bagian 2:
        // ”Successfully deleted {count} rulesets.” dalam AddRulesets.
        terms["rulesets.bulk_delete_success"] = ("Berhasil menghapus {count} set aturan.", "Successfully deleted {count} rulesets.");
        // Memperbarui `terms[”rulesets.bulk_delete_partial”]` menggunakan tuple yang membawa bagian 1: ”Sebagian berhasil dihapus. Sukses: {success},
        // gagal: {failed}.”; bagian 2: ”Partial deletion completed. Success: {success}, failed: {failed}.” dalam AddRulesets.
        terms["rulesets.bulk_delete_partial"] = ("Sebagian berhasil dihapus. Sukses: {success}, gagal: {failed}.", "Partial deletion completed. Success: {success}, failed: {failed}.");
        // Memperbarui `terms[”rulesets.bulk_delete_failed”]` menggunakan tuple yang membawa bagian 1: ”Semua penghapusan gagal. Total gagal: {failed}.”;
        // bagian 2: ”All deletions failed. Total failed: {failed}.” dalam AddRulesets.
        terms["rulesets.bulk_delete_failed"] = ("Semua penghapusan gagal. Total gagal: {failed}.", "All deletions failed. Total failed: {failed}.");
        // Memperbarui `terms[”rulesets.selected_count”]` menggunakan tuple yang membawa bagian 1: ”Dipilih”; bagian 2: ”Selected” dalam AddRulesets.
        terms["rulesets.selected_count"] = ("Dipilih", "Selected");
        // Memperbarui `terms[”rulesets.select_all”]` menggunakan tuple yang membawa bagian 1: ”Pilih semua set aturan”; bagian 2: ”Select all rulesets”
        // dalam AddRulesets.
        terms["rulesets.select_all"] = ("Pilih semua set aturan", "Select all rulesets");
        // Memperbarui `terms[”rulesets.select_one”]` menggunakan tuple yang membawa bagian 1: ”Pilih set aturan”; bagian 2: ”Select ruleset” dalam
        // AddRulesets.
        terms["rulesets.select_one"] = ("Pilih set aturan", "Select ruleset");
        // Memperbarui `terms[”rulesets.quick_guide_title”]` menggunakan tuple yang membawa bagian 1: ”Panduan cepat mengisi Set Aturan”; bagian 2: ”Quick
        // guide to filling a ruleset” dalam AddRulesets.
        terms["rulesets.quick_guide_title"] = ("Panduan cepat mengisi Set Aturan", "Quick guide to filling a ruleset");
        // Memperbarui `terms[”rulesets.quick_guide_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Pilih mode lebih dulu; form hanya menampilkan
        // pengaturan yang berlaku untuk mod...; bagian 2: ”Choose the mode first; the form only shows settings that apply to that mode.” dalam AddRulesets.
        terms["rulesets.quick_guide_subtitle"] = ("Pilih mode lebih dulu; form hanya menampilkan pengaturan yang berlaku untuk mode tersebut.", "Choose the mode first; the form only shows settings that apply to that mode.");
        // Memperbarui `terms[”rulesets.quick_guide_beginner”]` menggunakan tuple yang membawa bagian 1: ”Gunakan Pemula untuk latihan kebutuhan, pemasukan,
        // pengeluaran, dan tabungan ta...; bagian 2: ”Use Beginner to practice needs, income, spending, and saving without advanced r... dalam AddRulesets.
        terms["rulesets.quick_guide_beginner"] = ("Gunakan Pemula untuk latihan kebutuhan, pemasukan, pengeluaran, dan tabungan tanpa fitur risiko lanjutan.", "Use Beginner to practice needs, income, spending, and saving without advanced risk features.");
        // Memperbarui `terms[”rulesets.quick_guide_advanced”]` menggunakan tuple yang membawa bagian 1: ”Gunakan Mahir saat sesi membutuhkan pinjaman,
        // asuransi, risiko, dan target jang...; bagian 2: ”Use Advanced when the session needs loans, insurance, risks, and long-term goal... dalam
        // AddRulesets.
        terms["rulesets.quick_guide_advanced"] = ("Gunakan Mahir saat sesi membutuhkan pinjaman, asuransi, risiko, dan target jangka panjang.", "Use Advanced when the session needs loans, insurance, risks, and long-term goals.");
        // Memperbarui `terms[”rulesets.quick_guide_activation”]` menggunakan tuple yang membawa bagian 1: ”Setelah menyimpan, pastikan versi yang dipilih
        // sesi berstatus aktif sebelum per...; bagian 2: ”After saving, confirm that the version selected by the session is active before... dalam
        // AddRulesets.
        terms["rulesets.quick_guide_activation"] = ("Setelah menyimpan, pastikan versi yang dipilih sesi berstatus aktif sebelum permainan dimulai.", "After saving, confirm that the version selected by the session is active before gameplay starts.");
        // Memperbarui `terms[”rulesets.quick_tools”]` menggunakan tuple yang membawa bagian 1: ”Pencarian dan Filter”; bagian 2: ”Search and Filters” dalam
        // AddRulesets.
        terms["rulesets.quick_tools"] = ("Pencarian dan Filter", "Search and Filters");
        // Memperbarui `terms[”rulesets.search”]` menggunakan tuple yang membawa bagian 1: ”Cari Set Aturan”; bagian 2: ”Search Rulesets” dalam AddRulesets.
        terms["rulesets.search"] = ("Cari Set Aturan", "Search Rulesets");
        // Memperbarui `terms[”rulesets.search_placeholder”]` menggunakan tuple yang membawa bagian 1: ”Cari nama, mode, ID, atau versi ruleset...”; bagian
        // 2: ”Search ruleset name, mode, ID, or version...” dalam AddRulesets.
        terms["rulesets.search_placeholder"] = ("Cari nama, mode, ID, atau versi ruleset...", "Search ruleset name, mode, ID, or version...");
        // Memperbarui `terms[”rulesets.sort”]` menggunakan tuple yang membawa bagian 1: ”Urutkan”; bagian 2: ”Sort” dalam AddRulesets.
        terms["rulesets.sort"] = ("Urutkan", "Sort");
        // Memperbarui `terms[”rulesets.sort.name_asc”]` menggunakan tuple yang membawa bagian 1: ”Nama A-Z”; bagian 2: ”Name A-Z” dalam AddRulesets.
        terms["rulesets.sort.name_asc"] = ("Nama A-Z", "Name A-Z");
        // Memperbarui `terms[”rulesets.sort.name_desc”]` menggunakan tuple yang membawa bagian 1: ”Nama Z-A”; bagian 2: ”Name Z-A” dalam AddRulesets.
        terms["rulesets.sort.name_desc"] = ("Nama Z-A", "Name Z-A");
        // Memperbarui `terms[”rulesets.sort.version_desc”]` menggunakan tuple yang membawa bagian 1: ”Versi tertinggi”; bagian 2: ”Highest version” dalam
        // AddRulesets.
        terms["rulesets.sort.version_desc"] = ("Versi tertinggi", "Highest version");
        // Memperbarui `terms[”rulesets.sort.version_asc”]` menggunakan tuple yang membawa bagian 1: ”Versi terendah”; bagian 2: ”Lowest version” dalam
        // AddRulesets.
        terms["rulesets.sort.version_asc"] = ("Versi terendah", "Lowest version");
        // Memperbarui `terms[”rulesets.page_size”]` menggunakan tuple yang membawa bagian 1: ”Baris per Halaman”; bagian 2: ”Rows per Page” dalam
        // AddRulesets.
        terms["rulesets.page_size"] = ("Baris per Halaman", "Rows per Page");
        // Memperbarui `terms[”rulesets.clear_filter”]` menggunakan tuple yang membawa bagian 1: ”Reset”; bagian 2: ”Reset” dalam AddRulesets.
        terms["rulesets.clear_filter"] = ("Reset", "Reset");
        // Memperbarui `terms[”rulesets.no_filtered”]` menggunakan tuple yang membawa bagian 1: ”Tidak ada set aturan yang cocok dengan filter.”; bagian 2:
        // ”No rulesets match the filter.” dalam AddRulesets.
        terms["rulesets.no_filtered"] = ("Tidak ada set aturan yang cocok dengan filter.", "No rulesets match the filter.");
        // Memperbarui `terms[”rulesets.pagination_prev”]` menggunakan tuple yang membawa bagian 1: ”Sebelumnya”; bagian 2: ”Previous” dalam AddRulesets.
        terms["rulesets.pagination_prev"] = ("Sebelumnya", "Previous");
        // Memperbarui `terms[”rulesets.pagination_next”]` menggunakan tuple yang membawa bagian 1: ”Berikutnya”; bagian 2: ”Next” dalam AddRulesets.
        terms["rulesets.pagination_next"] = ("Berikutnya", "Next");
        // Memperbarui `terms[”rulesets.tips.title”]` menggunakan tuple yang membawa bagian 1: ”Tips membaca menu Set Aturan”; bagian 2: ”Rulesets reading
        // tips” dalam AddRulesets.
        terms["rulesets.tips.title"] = ("Tips membaca menu Set Aturan", "Rulesets reading tips");
        // Memperbarui `terms[”rulesets.tip.create”]` menggunakan tuple yang membawa bagian 1: ”Baca mode terlebih dahulu karena komponen Pemula dan Mahir
        // memiliki fungsi yang...; bagian 2: ”Read the mode first because Beginner and Advanced components serve different fu... dalam AddRulesets.
        terms["rulesets.tip.create"] = ("Baca mode terlebih dahulu karena komponen Pemula dan Mahir memiliki fungsi yang berbeda.", "Read the mode first because Beginner and Advanced components serve different functions.");
        // Memperbarui `terms[”rulesets.tip.versioning”]` menggunakan tuple yang membawa bagian 1: ”Cocokkan nomor versi dengan Set Aturan aktif pada sesi
        // sebelum menafsirkan hasi...; bagian 2: ”Match the version number with the session's active ruleset before interpreting ... dalam AddRulesets.
        terms["rulesets.tip.versioning"] = ("Cocokkan nomor versi dengan Set Aturan aktif pada sesi sebelum menafsirkan hasil.", "Match the version number with the session's active ruleset before interpreting results.");
        // Memperbarui `terms[”rulesets.tip.safety”]` menggunakan tuple yang membawa bagian 1: ”Kelola perubahan dari halaman ini; pemain melihat salinan
        // aturan yang benar-ben...; bagian 2: ”Manage changes from this page; players view the rules actually used through Ses... dalam AddRulesets.
        terms["rulesets.tip.safety"] = ("Kelola perubahan dari halaman ini; pemain melihat salinan aturan yang benar-benar dipakai melalui Analitika Sesi.", "Manage changes from this page; players view the rules actually used through Session Analytics.");
        // Memperbarui `terms[”rulesets.tips.form_title”]` menggunakan tuple yang membawa bagian 1: ”Tips mengisi form Set Aturan”; bagian 2: ”Ruleset form
        // tips” dalam AddRulesets.
        terms["rulesets.tips.form_title"] = ("Tips mengisi form Set Aturan", "Ruleset form tips");
        // Memperbarui `terms[”rulesets.tip.versioning_draft”]` menggunakan tuple yang membawa bagian 1: ”Simpan perubahan sebagai versi baru agar aturan
        // lama tetap dapat ditelusuri pad...; bagian 2: ”Save changes as a new version so previous rules remain traceable in history.” dalam AddRulesets.
        terms["rulesets.tip.versioning_draft"] = ("Simpan perubahan sebagai versi baru agar aturan lama tetap dapat ditelusuri pada riwayat.", "Save changes as a new version so previous rules remain traceable in history.");
        // Memperbarui `terms[”rulesets.tip.consistency”]` menggunakan tuple yang membawa bagian 1: ”Cocokkan jumlah aksi, kas awal, batasan, dan fitur
        // harian dengan tujuan latihan...; bagian 2: ”Match action count, starting cash, constraints, and daily features to the sessi... dalam AddRulesets.
        terms["rulesets.tip.consistency"] = ("Cocokkan jumlah aksi, kas awal, batasan, dan fitur harian dengan tujuan latihan pada sesi.", "Match action count, starting cash, constraints, and daily features to the session's learning objective.");
        // Memperbarui `terms[”rulesets.tip.safety_before_start”]` menggunakan tuple yang membawa bagian 1: ”Periksa ringkasan konfigurasi dan aktifkan
        // versi yang benar sebelum sesi menggu...; bagian 2: ”Review the configuration summary and activate the correct version before the se... dalam
        // AddRulesets.
        terms["rulesets.tip.safety_before_start"] = ("Periksa ringkasan konfigurasi dan aktifkan versi yang benar sebelum sesi menggunakan aturan tersebut.", "Review the configuration summary and activate the correct version before the session uses it.");
        // Memperbarui `terms[”rulesets.usage_status.active”]` menggunakan tuple yang membawa bagian 1: ”Dipakai”; bagian 2: ”Used” dalam AddRulesets.
        terms["rulesets.usage_status.active"] = ("Dipakai", "Used");
        // Memperbarui `terms[”rulesets.usage_status.draft”]` menggunakan tuple yang membawa bagian 1: ”Belum Dipakai”; bagian 2: ”Not Used Yet” dalam
        // AddRulesets.
        terms["rulesets.usage_status.draft"] = ("Belum Dipakai", "Not Used Yet");
        // Memperbarui `terms[”rulesets.usage_status.unknown”]` menggunakan tuple yang membawa bagian 1: ”Tidak Diketahui”; bagian 2: ”Unknown” dalam
        // AddRulesets.
        terms["rulesets.usage_status.unknown"] = ("Tidak Diketahui", "Unknown");
        // Memperbarui `terms[”rulesets.detail_title”]` menggunakan tuple yang membawa bagian 1: ”Rincian Set Aturan”; bagian 2: ”Ruleset details” dalam
        // AddRulesets.
        terms["rulesets.detail_title"] = ("Rincian Set Aturan", "Ruleset details");
        // Memperbarui `terms[”rulesets.detail.empty_title”]` menggunakan tuple yang membawa bagian 1: ”Rincian set aturan belum tersedia”; bagian 2:
        // ”Ruleset details are not available yet” dalam AddRulesets.
        terms["rulesets.detail.empty_title"] = ("Rincian set aturan belum tersedia", "Ruleset details are not available yet");
        // Memperbarui `terms[”rulesets.detail.empty_body”]` menggunakan tuple yang membawa bagian 1: ”Set aturan yang Anda buka tidak ditemukan atau belum
        // bisa diakses. Kembali ke d...; bagian 2: ”The ruleset you opened was not found or is not accessible yet. Return to the li... dalam AddRulesets.
        terms["rulesets.detail.empty_body"] = ("Set aturan yang Anda buka tidak ditemukan atau belum bisa diakses. Kembali ke daftar lalu pilih set aturan lain.", "The ruleset you opened was not found or is not accessible yet. Return to the list and choose another ruleset.");
        // Memperbarui `terms[”rulesets.detail.source_default_catalog”]` menggunakan tuple yang membawa bagian 1: ”Katalog komponen default”; bagian 2:
        // ”Default component catalog” dalam AddRulesets.
        terms["rulesets.detail.source_default_catalog"] = ("Katalog komponen default", "Default component catalog");
        // Memperbarui `terms[”rulesets.readonly_hint”]` menggunakan tuple yang membawa bagian 1: ”Mode baca saja.”; bagian 2: ”Read-only mode.” dalam
        // AddRulesets.
        terms["rulesets.readonly_hint"] = ("Mode baca saja.", "Read-only mode.");
        // Memperbarui `terms[”rulesets.back_to_list”]` menggunakan tuple yang membawa bagian 1: ”Kembali ke daftar”; bagian 2: ”Back to list” dalam
        // AddRulesets.
        terms["rulesets.back_to_list"] = ("Kembali ke daftar", "Back to list");
        // Memperbarui `terms[”rulesets.status.available”]` menggunakan tuple yang membawa bagian 1: ”Tersedia”; bagian 2: ”Available” dalam AddRulesets.
        terms["rulesets.status.available"] = ("Tersedia", "Available");
        // Memperbarui `terms[”rulesets.config_summary”]` menggunakan tuple yang membawa bagian 1: ”Ringkasan Konfigurasi Aturan”; bagian 2: ”Ruleset
        // Configuration Summary” dalam AddRulesets.
        terms["rulesets.config_summary"] = ("Ringkasan Konfigurasi Aturan", "Ruleset Configuration Summary");
        // Memperbarui `terms[”rulesets.config_summary_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Ringkasan ini membantu Anda mengecek aturan
        // utama tanpa membaca JSON mentah.”; bagian 2: ”This summary helps you verify key rules without reading raw JSON.” dalam AddRulesets.
        terms["rulesets.config_summary_subtitle"] = ("Ringkasan ini membantu Anda mengecek aturan utama tanpa membaca JSON mentah.", "This summary helps you verify key rules without reading raw JSON.");
        // Memperbarui `terms[”rulesets.config_unavailable”]` menggunakan tuple yang membawa bagian 1: ”Konfigurasi belum tersedia.”; bagian 2:
        // ”Configuration is not available yet.” dalam AddRulesets.
        terms["rulesets.config_unavailable"] = ("Konfigurasi belum tersedia.", "Configuration is not available yet.");
        // Memperbarui `terms[”rulesets.version_history”]` menggunakan tuple yang membawa bagian 1: ”Riwayat Versi Aturan”; bagian 2: ”Ruleset Version
        // History” dalam AddRulesets.
        terms["rulesets.version_history"] = ("Riwayat Versi Aturan", "Ruleset Version History");
        // Memperbarui `terms[”rulesets.version_history_subtitle”]` menggunakan tuple yang membawa bagian 1: ”Daftar versi membantu Anda menelusuri versi
        // mana yang dipakai sesi permainan.”; bagian 2: ”Version history helps you trace which version is used by gameplay sessions.” dalam AddRulesets.
        terms["rulesets.version_history_subtitle"] = ("Daftar versi membantu Anda menelusuri versi mana yang dipakai sesi permainan.", "Version history helps you trace which version is used by gameplay sessions.");
        // Memperbarui `terms[”rulesets.default_components.title”]` menggunakan tuple yang membawa bagian 1: ”Komponen Default (Pemula + Mahir)”; bagian 2:
        // ”Default Components (Beginner + Advanced)” dalam AddRulesets.
        terms["rulesets.default_components.title"] = ("Komponen Default (Pemula + Mahir)", "Default Components (Beginner + Advanced)");
        // Memperbarui `terms[”rulesets.default_components.subtitle”]` menggunakan tuple yang membawa bagian 1: ”Daftar ruleset bawaan berisi susunan
        // komponen permainan.”; bagian 2: ”List of built-in rulesets containing gameplay component setup.” dalam AddRulesets.
        terms["rulesets.default_components.subtitle"] = ("Daftar ruleset bawaan berisi susunan komponen permainan.", "List of built-in rulesets containing gameplay component setup.");
        // Memperbarui `terms[”rulesets.default_components.empty”]` menggunakan tuple yang membawa bagian 1: ”Belum ada ruleset default komponen yang
        // tersedia.”; bagian 2: ”No default component rulesets are available yet.” dalam AddRulesets.
        terms["rulesets.default_components.empty"] = ("Belum ada ruleset default komponen yang tersedia.", "No default component rulesets are available yet.");
        // Memperbarui `terms[”rulesets.default_components.open_detail”]` menggunakan tuple yang membawa bagian 1: ”Lihat komponen”; bagian 2: ”View
        // components” dalam AddRulesets.
        terms["rulesets.default_components.open_detail"] = ("Lihat komponen", "View components");
        // Memperbarui `terms[”rulesets.components.title”]` menggunakan tuple yang membawa bagian 1: ”Katalog Komponen Ruleset”; bagian 2: ”Ruleset
        // Component Catalog” dalam AddRulesets.
        terms["rulesets.components.title"] = ("Katalog Komponen Ruleset", "Ruleset Component Catalog");
        // Memperbarui `terms[”rulesets.components.subtitle”]` menggunakan tuple yang membawa bagian 1: ”Data ini diambil dari endpoint komponen ruleset
        // sesuai versi aktif/terbaru.”; bagian 2: ”This data is fetched from the ruleset component endpoint for active/latest vers... dalam AddRulesets.
        terms["rulesets.components.subtitle"] = ("Data ini diambil dari endpoint komponen ruleset sesuai versi aktif/terbaru.", "This data is fetched from the ruleset component endpoint for active/latest version.");
        // Memperbarui `terms[”rulesets.components.subtitle_detail”]` menggunakan tuple yang membawa bagian 1: ”Bagian ini menampilkan komponen aktual dari
        // versi ruleset yang sedang Anda liha...; bagian 2: ”This section shows the actual components from the ruleset version you are viewi... dalam
        // AddRulesets.
        terms["rulesets.components.subtitle_detail"] = ("Bagian ini menampilkan komponen aktual dari versi ruleset yang sedang Anda lihat.", "This section shows the actual components from the ruleset version you are viewing.");
        // Memperbarui `terms[”rulesets.components.unavailable”]` menggunakan tuple yang membawa bagian 1: ”Katalog komponen belum tersedia untuk ruleset
        // ini.”; bagian 2: ”Component catalog is not available for this ruleset yet.” dalam AddRulesets.
        terms["rulesets.components.unavailable"] = ("Katalog komponen belum tersedia untuk ruleset ini.", "Component catalog is not available for this ruleset yet.");
        // Memperbarui `terms[”rulesets.components.ruleset_version_id”]` menggunakan tuple yang membawa bagian 1: ”ID Versi Set Aturan”; bagian 2: ”Ruleset
        // Version ID” dalam AddRulesets.
        terms["rulesets.components.ruleset_version_id"] = ("ID Versi Set Aturan", "Ruleset Version ID");
        // Memperbarui `terms[”rulesets.components.item_count”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Item Komponen”; bagian 2: ”Total Component
        // Items” dalam AddRulesets.
        terms["rulesets.components.item_count"] = ("Jumlah Item Komponen", "Total Component Items");
        // Memperbarui `terms[”rulesets.components.empty”]` menggunakan tuple yang membawa bagian 1: ”Belum ada item komponen yang dapat ditampilkan.”;
        // bagian 2: ”No component items are available to display.” dalam AddRulesets.
        terms["rulesets.components.empty"] = ("Belum ada item komponen yang dapat ditampilkan.", "No component items are available to display.");
        // Memperbarui `terms[”rulesets.components.raw_json”]` menggunakan tuple yang membawa bagian 1: ”Lihat JSON mentah komponen”; bagian 2: ”View raw
        // component JSON” dalam AddRulesets.
        terms["rulesets.components.raw_json"] = ("Lihat JSON mentah komponen", "View raw component JSON");
        // Memperbarui `terms[”rulesets.info.default_catalog_readonly”]` menggunakan tuple yang membawa bagian 1: ”Data ini dibuka dari katalog komponen
        // default dan hanya tersedia dalam mode bac...; bagian 2: ”This data was opened from the default component catalog and is available in rea... dalam
        // AddRulesets.
        terms["rulesets.info.default_catalog_readonly"] = ("Data ini dibuka dari katalog komponen default dan hanya tersedia dalam mode baca.", "This data was opened from the default component catalog and is available in read-only mode.");
        // Memperbarui `terms[”rulesets.info.viewing_version”]` menggunakan tuple yang membawa bagian 1: ”Menampilkan komponen untuk versi {version}.”;
        // bagian 2: ”Showing components for version {version}.” dalam AddRulesets.
        terms["rulesets.info.viewing_version"] = ("Menampilkan komponen untuk versi {version}.", "Showing components for version {version}.");
        // Memperbarui `terms[”rulesets.form.core_setup”]` menggunakan tuple yang membawa bagian 1: ”Aturan Dasar Sesi”; bagian 2: ”Session Core Rules”
        // dalam AddRulesets.
        terms["rulesets.form.core_setup"] = ("Aturan Dasar Sesi", "Session Core Rules");
        // Memperbarui `terms[”rulesets.form.mode”]` menggunakan tuple yang membawa bagian 1: ”Mode”; bagian 2: ”Mode” dalam AddRulesets.
        terms["rulesets.form.mode"] = ("Mode", "Mode");
        // Memperbarui `terms[”rulesets.form.mode_beginner”]` menggunakan tuple yang membawa bagian 1: ”PEMULA”; bagian 2: ”BEGINNER” dalam AddRulesets.
        terms["rulesets.form.mode_beginner"] = ("PEMULA", "BEGINNER");
        // Memperbarui `terms[”rulesets.form.mode_advanced”]` menggunakan tuple yang membawa bagian 1: ”MAHIR”; bagian 2: ”ADVANCED” dalam AddRulesets.
        terms["rulesets.form.mode_advanced"] = ("MAHIR", "ADVANCED");
        // Memperbarui `terms[”rulesets.form.mode_choice_hint”]` menggunakan tuple yang membawa bagian 1: ”Pilih profil aturan utama. Nilai rekomendasi akan
        // diterapkan otomatis dan masih...; bagian 2: ”Choose the main rule profile. Recommended values are applied automatically and ... dalam
        // AddRulesets.
        terms["rulesets.form.mode_choice_hint"] = ("Pilih profil aturan utama. Nilai rekomendasi akan diterapkan otomatis dan masih dapat disesuaikan.", "Choose the main rule profile. Recommended values are applied automatically and remain editable.");
        // Memperbarui `terms[”rulesets.form.actions_per_turn”]` menggunakan tuple yang membawa bagian 1: ”Jumlah Aksi per Giliran Pemain”; bagian 2:
        // ”Actions per Player Turn” dalam AddRulesets.
        terms["rulesets.form.actions_per_turn"] = ("Jumlah Aksi per Giliran Pemain", "Actions per Player Turn");
        // Memperbarui `terms[”rulesets.default_description_beginner”]` menggunakan tuple yang membawa bagian 1: ”Aturan sederhana untuk berlatih memenuhi
        // kebutuhan, mengatur pemasukan dan peng...; bagian 2: ”A simpler ruleset for practicing need fulfillment, managing income and spending... dalam
        // AddRulesets.
        terms["rulesets.default_description_beginner"] = ("Aturan sederhana untuk berlatih memenuhi kebutuhan, mengatur pemasukan dan pengeluaran, serta membangun tabungan.", "A simpler ruleset for practicing need fulfillment, managing income and spending, and building savings.");
        // Memperbarui `terms[”rulesets.default_description_advanced”]` menggunakan tuple yang membawa bagian 1: ”Aturan lengkap dengan pinjaman, asuransi,
        // risiko, dan tujuan jangka panjang unt...; bagian 2: ”A complete ruleset with loans, insurance, risk, and long-term goals for deeper ... dalam
        // AddRulesets.
        terms["rulesets.default_description_advanced"] = ("Aturan lengkap dengan pinjaman, asuransi, risiko, dan tujuan jangka panjang untuk melatih keputusan finansial yang lebih mendalam.", "A complete ruleset with loans, insurance, risk, and long-term goals for deeper financial decision-making.");
        // Memperbarui `terms[”rulesets.form.starting_cash”]` menggunakan tuple yang membawa bagian 1: ”Kas Awal per Pemain”; bagian 2: ”Starting Cash per
        // Player” dalam AddRulesets.
        terms["rulesets.form.starting_cash"] = ("Kas Awal per Pemain", "Starting Cash per Player");
        // Memperbarui `terms[”rulesets.form.player_ordering”]` menggunakan tuple yang membawa bagian 1: ”Urutan Pemain”; bagian 2: ”Player Ordering” dalam
        // AddRulesets.
        terms["rulesets.form.player_ordering"] = ("Urutan Pemain", "Player Ordering");
        // Memperbarui `terms[”rulesets.form.player_ordering_player_order”]` menggunakan tuple yang membawa bagian 1: ”Urutan Bergabung (Sesi)”; bagian 2:
        // ”Join Order (Session)” dalam AddRulesets.
        terms["rulesets.form.player_ordering_player_order"] = ("Urutan Bergabung (Sesi)", "Join Order (Session)");
        // Memperbarui `terms[”rulesets.form.player_ordering_fixed_hint”]` menggunakan tuple yang membawa bagian 1: ”Pemain ditambahkan ke sesi setelah sesi
        // dibuat, lalu urutannya ditentukan saat ...; bagian 2: ”Players are added after the session is created, then ordered as they join.” dalam
        // AddRulesets.
        terms["rulesets.form.player_ordering_fixed_hint"] = ("Pemain ditambahkan ke sesi setelah sesi dibuat, lalu urutannya ditentukan saat bergabung.", "Players are added after the session is created, then ordered as they join.");
        // Memperbarui `terms[”rulesets.form.player_ordering_event_sequence”]` menggunakan tuple yang membawa bagian 1: ”Aktivitas Pertama”; bagian 2:
        // ”First Event” dalam AddRulesets.
        terms["rulesets.form.player_ordering_event_sequence"] = ("Aktivitas Pertama", "First Event");
        // Memperbarui `terms[”rulesets.form.player_ordering_player_id”]` menggunakan tuple yang membawa bagian 1: ”ID Pemain”; bagian 2: ”Player ID” dalam
        // AddRulesets.
        terms["rulesets.form.player_ordering_player_id"] = ("ID Pemain", "Player ID");
        // Memperbarui `terms[”rulesets.form.player_ordering_username”]` menggunakan tuple yang membawa bagian 1: ”Username (A-Z)”; bagian 2: ”Username
        // (A-Z)” dalam AddRulesets.
        terms["rulesets.form.player_ordering_username"] = ("Username (A-Z)", "Username (A-Z)");
        // Memperbarui `terms[”rulesets.form.freelance_income”]` menggunakan tuple yang membawa bagian 1: ”Pendapatan Kerja Lepas”; bagian 2: ”Freelance
        // Income” dalam AddRulesets.
        terms["rulesets.form.freelance_income"] = ("Pendapatan Kerja Lepas", "Freelance Income");
        // Memperbarui `terms[”rulesets.form.weekday_features”]` menggunakan tuple yang membawa bagian 1: ”Fitur Hari”; bagian 2: ”Weekday Features” dalam
        // AddRulesets.
        terms["rulesets.form.weekday_features"] = ("Fitur Hari", "Weekday Features");
        // Memperbarui `terms[”rulesets.form.friday_donation”]` menggunakan tuple yang membawa bagian 1: ”Donasi Jumat”; bagian 2: ”Friday Donation” dalam
        // AddRulesets.
        terms["rulesets.form.friday_donation"] = ("Donasi Jumat", "Friday Donation");
        // Memperbarui `terms[”rulesets.form.saturday_gold_trade”]` menggunakan tuple yang membawa bagian 1: ”Perdagangan Emas Sabtu”; bagian 2: ”Saturday
        // Gold Trade” dalam AddRulesets.
        terms["rulesets.form.saturday_gold_trade"] = ("Perdagangan Emas Sabtu", "Saturday Gold Trade");
        // Memperbarui `terms[”rulesets.form.sunday_rest”]` menggunakan tuple yang membawa bagian 1: ”Minggu Libur”; bagian 2: ”Sunday Rest” dalam
        // AddRulesets.
        terms["rulesets.form.sunday_rest"] = ("Minggu Libur", "Sunday Rest");
        // Memperbarui `terms[”rulesets.form.constraints”]` menggunakan tuple yang membawa bagian 1: ”Batasan”; bagian 2: ”Constraints” dalam AddRulesets.
        terms["rulesets.form.constraints"] = ("Batasan", "Constraints");
        // Memperbarui `terms[”rulesets.form.cash_min”]` menggunakan tuple yang membawa bagian 1: ”Kas Minimum”; bagian 2: ”Cash Min” dalam AddRulesets.
        terms["rulesets.form.cash_min"] = ("Kas Minimum", "Cash Min");
        // Memperbarui `terms[”rulesets.form.max_ingredient_total”]` menggunakan tuple yang membawa bagian 1: ”Total Bahan Maksimum”; bagian 2: ”Max
        // Ingredient Total” dalam AddRulesets.
        terms["rulesets.form.max_ingredient_total"] = ("Total Bahan Maksimum", "Max Ingredient Total");
        // Memperbarui `terms[”rulesets.form.max_same_ingredient”]` menggunakan tuple yang membawa bagian 1: ”Batas Bahan Sejenis”; bagian 2: ”Max Same
        // Ingredient” dalam AddRulesets.
        terms["rulesets.form.max_same_ingredient"] = ("Batas Bahan Sejenis", "Max Same Ingredient");
        // Memperbarui `terms[”rulesets.form.economy_and_donation”]` menggunakan tuple yang membawa bagian 1: ”Ekonomi dan Donasi”; bagian 2: ”Economy and
        // Donation” dalam AddRulesets.
        terms["rulesets.form.economy_and_donation"] = ("Ekonomi dan Donasi", "Economy and Donation");
        // Memperbarui `terms[”rulesets.form.donation_min”]` menggunakan tuple yang membawa bagian 1: ”Donasi Minimum”; bagian 2: ”Donation Min” dalam
        // AddRulesets.
        terms["rulesets.form.donation_min"] = ("Donasi Minimum", "Donation Min");
        // Memperbarui `terms[”rulesets.form.donation_max”]` menggunakan tuple yang membawa bagian 1: ”Donasi Maksimum”; bagian 2: ”Donation Max” dalam
        // AddRulesets.
        terms["rulesets.form.donation_max"] = ("Donasi Maksimum", "Donation Max");
        // Memperbarui `terms[”rulesets.form.gold_buy”]` menggunakan tuple yang membawa bagian 1: ”Beli Emas”; bagian 2: ”Gold Buy” dalam AddRulesets.
        terms["rulesets.form.gold_buy"] = ("Beli Emas", "Gold Buy");
        // Memperbarui `terms[”rulesets.form.gold_sell”]` menggunakan tuple yang membawa bagian 1: ”Jual Emas”; bagian 2: ”Gold Sell” dalam AddRulesets.
        terms["rulesets.form.gold_sell"] = ("Jual Emas", "Gold Sell");
        // Memperbarui `terms[”rulesets.form.loan_enabled”]` menggunakan tuple yang membawa bagian 1: ”Pinjaman Aktif”; bagian 2: ”Loan Enabled” dalam
        // AddRulesets.
        terms["rulesets.form.loan_enabled"] = ("Pinjaman Aktif", "Loan Enabled");
        // Memperbarui `terms[”rulesets.form.insurance_enabled”]` menggunakan tuple yang membawa bagian 1: ”Asuransi Aktif”; bagian 2: ”Insurance Enabled”
        // dalam AddRulesets.
        terms["rulesets.form.insurance_enabled"] = ("Asuransi Aktif", "Insurance Enabled");
        // Memperbarui `terms[”rulesets.form.saving_goal_enabled”]` menggunakan tuple yang membawa bagian 1: ”Target Tabungan Aktif”; bagian 2: ”Saving Goal
        // Enabled” dalam AddRulesets.
        terms["rulesets.form.saving_goal_enabled"] = ("Target Tabungan Aktif", "Saving Goal Enabled");
        // Memperbarui `terms[”rulesets.form.advanced_features_hint”]` menggunakan tuple yang membawa bagian 1: ”Mode MAHIR otomatis mengaktifkan pinjaman,
        // asuransi, dan target tabungan.”; bagian 2: ”ADVANCED mode automatically enables loans, insurance, and saving goals.” dalam AddRulesets.
        terms["rulesets.form.advanced_features_hint"] = ("Mode MAHIR otomatis mengaktifkan pinjaman, asuransi, dan target tabungan.", "ADVANCED mode automatically enables loans, insurance, and saving goals.");
        // Memperbarui `terms[”rulesets.form.description_placeholder”]` menggunakan tuple yang membawa bagian 1: ”Contoh: Simulasi panjang dengan fokus misi
        // dan kepatuhan aturan.”; bagian 2: ”Example: Long simulation focused on mission and rules compliance.” dalam AddRulesets.
        terms["rulesets.form.description_placeholder"] = ("Contoh: Simulasi panjang dengan fokus misi dan kepatuhan aturan.", "Example: Long simulation focused on mission and rules compliance.");
        // Memperbarui `terms[”rulesets.form.description_hint”]` menggunakan tuple yang membawa bagian 1: ”Deskripsi menjelaskan tujuan/karakter aturan.
        // Nomor versi dibuat otomatis oleh ...; bagian 2: ”Description should explain the rule intent/character. Version numbers are gener... dalam
        // AddRulesets.
        terms["rulesets.form.description_hint"] = ("Deskripsi menjelaskan tujuan/karakter aturan. Nomor versi dibuat otomatis oleh sistem.", "Description should explain the rule intent/character. Version numbers are generated automatically.");
        // Memperbarui `terms[”rulesets.form.help.core_setup”]` menggunakan tuple yang membawa bagian 1: ”Atur mode permainan, jumlah aksi harian pemain,
        // kas awal, urutan pemain, dan ha...; bagian 2: ”Set the game mode, daily player actions, starting cash, player ordering, and fr... dalam
        // AddRulesets.
        terms["rulesets.form.help.core_setup"] = ("Atur mode permainan, jumlah aksi harian pemain, kas awal, urutan pemain, dan hasil kerja lepas.", "Set the game mode, daily player actions, starting cash, player ordering, and freelance reward.");
        // Memperbarui `terms[”rulesets.form.help.weekday_features”]` menggunakan tuple yang membawa bagian 1: ”Tentukan apakah event khusus
        // Jumat/Sabtu/Minggu berjalan di sesi ini.”; bagian 2: ”Decide whether Friday/Saturday/Sunday special events are enabled in this sessio... dalam
        // AddRulesets.
        terms["rulesets.form.help.weekday_features"] = ("Tentukan apakah event khusus Jumat/Sabtu/Minggu berjalan di sesi ini.", "Decide whether Friday/Saturday/Sunday special events are enabled in this session.");
        // Memperbarui `terms[”rulesets.form.help.constraints”]` menggunakan tuple yang membawa bagian 1: ”Batasan ini menjaga permainan tetap sesuai aturan
        // stok bahan, kas, dan urutan k...; bagian 2: ”These constraints keep gameplay aligned with inventory, cash floor, and need-or... dalam
        // AddRulesets.
        terms["rulesets.form.help.constraints"] = ("Batasan ini menjaga permainan tetap sesuai aturan stok bahan, kas, dan urutan kebutuhan.", "These constraints keep gameplay aligned with inventory, cash floor, and need-order rules.");
        // Memperbarui `terms[”rulesets.form.help.economy_and_donation”]` menggunakan tuple yang membawa bagian 1: ”Atur rentang donasi dan izin transaksi
        // emas. Pinjaman, asuransi, serta target t...; bagian 2: ”Set donation limits and gold-trade permissions. Loans, insurance, and saving go... dalam
        // AddRulesets.
        terms["rulesets.form.help.economy_and_donation"] = ("Atur rentang donasi dan izin transaksi emas. Pinjaman, asuransi, serta target tabungan hanya muncul pada mode Mahir.", "Set donation limits and gold-trade permissions. Loans, insurance, and saving goals appear only in Advanced mode.");
        // Memperbarui `terms[”rulesets.config_read_guide”]` menggunakan tuple yang membawa bagian 1: ”Gunakan ringkasan ini untuk cek apakah setup sesi,
        // alur aksi harian, dan aturan...; bagian 2: ”Use this summary to verify that session setup, daily action flow, and economy r... dalam AddRulesets.
        terms["rulesets.config_read_guide"] = ("Gunakan ringkasan ini untuk cek apakah setup sesi, alur aksi harian, dan aturan ekonomi sudah sesuai yang Anda inginkan.", "Use this summary to verify that session setup, daily action flow, and economy rules match your intent.");
        // Memperbarui `terms[”rulesets.error.load_list_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal mengambil set aturan. Status: {status}”;
        // bagian 2: ”Failed to fetch rulesets. Status: {status}” dalam AddRulesets.
        terms["rulesets.error.load_list_failed"] = ("Gagal mengambil set aturan. Status: {status}", "Failed to fetch rulesets. Status: {status}");
        // Memperbarui `terms[”rulesets.error.invalid_list_response”]` menggunakan tuple yang membawa bagian 1: ”Respons daftar set aturan tidak valid.”;
        // bagian 2: ”Invalid ruleset list response.” dalam AddRulesets.
        terms["rulesets.error.invalid_list_response"] = ("Respons daftar set aturan tidak valid.", "Invalid ruleset list response.");
        // Memperbarui `terms[”rulesets.error.load_detail_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal memuat rincian set aturan. Status:
        // {status}”; bagian 2: ”Failed to load ruleset details. Status: {status}” dalam AddRulesets.
        terms["rulesets.error.load_detail_failed"] = ("Gagal memuat rincian set aturan. Status: {status}", "Failed to load ruleset details. Status: {status}");
        // Memperbarui `terms[”rulesets.error.invalid_detail_response”]` menggunakan tuple yang membawa bagian 1: ”Respons rincian set aturan tidak valid.”;
        // bagian 2: ”Invalid ruleset detail response.” dalam AddRulesets.
        terms["rulesets.error.invalid_detail_response"] = ("Respons rincian set aturan tidak valid.", "Invalid ruleset detail response.");
        // Memperbarui `terms[”rulesets.error.load_for_edit_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal memuat set aturan untuk diedit.
        // Status: {status}”; bagian 2: ”Failed to load ruleset for editing. Status: {status}” dalam AddRulesets.
        terms["rulesets.error.load_for_edit_failed"] = ("Gagal memuat set aturan untuk diedit. Status: {status}", "Failed to load ruleset for editing. Status: {status}");
        // Memperbarui `terms[”rulesets.error.name_required”]` menggunakan tuple yang membawa bagian 1: ”Nama set aturan wajib diisi.”; bagian 2: ”Ruleset
        // name is required.” dalam AddRulesets.
        terms["rulesets.error.name_required"] = ("Nama set aturan wajib diisi.", "Ruleset name is required.");
        // Memperbarui `terms[”rulesets.error.invalid_definition_json”]` menggunakan tuple yang membawa bagian 1: ”Definition JSON tidak valid.”; bagian 2:
        // ”Definition JSON is invalid.” dalam AddRulesets.
        terms["rulesets.error.invalid_definition_json"] = ("Definition JSON tidak valid.", "Definition JSON is invalid.");
        // Memperbarui `terms[”rulesets.error.create_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal membuat set aturan. Status: {status}”; bagian
        // 2: ”Failed to create ruleset. Status: {status}” dalam AddRulesets.
        terms["rulesets.error.create_failed"] = ("Gagal membuat set aturan. Status: {status}", "Failed to create ruleset. Status: {status}");
        // Memperbarui `terms[”rulesets.error.update_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal memperbarui set aturan. Status: {status}”;
        // bagian 2: ”Failed to update ruleset. Status: {status}” dalam AddRulesets.
        terms["rulesets.error.update_failed"] = ("Gagal memperbarui set aturan. Status: {status}", "Failed to update ruleset. Status: {status}");
        // Memperbarui `terms[”rulesets.error.activate_version_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal mengaktifkan versi set aturan”;
        // bagian 2: ”Failed to activate ruleset version” dalam AddRulesets.
        terms["rulesets.error.activate_version_failed"] = ("Gagal mengaktifkan versi set aturan", "Failed to activate ruleset version");
        // Memperbarui `terms[”rulesets.error.delete_version_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal menghapus versi set aturan”; bagian
        // 2: ”Failed to delete ruleset version” dalam AddRulesets.
        terms["rulesets.error.delete_version_failed"] = ("Gagal menghapus versi set aturan", "Failed to delete ruleset version");
        // Memperbarui `terms[”rulesets.error.delete_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal menghapus set aturan”; bagian 2: ”Failed to
        // delete ruleset” dalam AddRulesets.
        terms["rulesets.error.delete_failed"] = ("Gagal menghapus set aturan", "Failed to delete ruleset");
        // Memperbarui `terms[”rulesets.error.bulk_delete_empty”]` menggunakan tuple yang membawa bagian 1: ”Pilih minimal satu set aturan untuk dihapus.”;
        // bagian 2: ”Select at least one ruleset to delete.” dalam AddRulesets.
        terms["rulesets.error.bulk_delete_empty"] = ("Pilih minimal satu set aturan untuk dihapus.", "Select at least one ruleset to delete.");
        // Memperbarui `terms[”rulesets.error.load_default_components_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal mengambil komponen default.
        // Status: {status}”; bagian 2: ”Failed to fetch default components. Status: {status}” dalam AddRulesets.
        terms["rulesets.error.load_default_components_failed"] = ("Gagal mengambil komponen default. Status: {status}", "Failed to fetch default components. Status: {status}");
        // Memperbarui `terms[”rulesets.error.invalid_default_components_response”]` menggunakan tuple yang membawa bagian 1: ”Respons komponen default
        // tidak valid.”; bagian 2: ”Invalid default components response.” dalam AddRulesets.
        terms["rulesets.error.invalid_default_components_response"] = ("Respons komponen default tidak valid.", "Invalid default components response.");
        // Memperbarui `terms[”rulesets.error.load_components_failed”]` menggunakan tuple yang membawa bagian 1: ”Gagal mengambil katalog komponen ruleset.
        // Status: {status}”; bagian 2: ”Failed to fetch ruleset component catalog. Status: {status}” dalam AddRulesets.
        terms["rulesets.error.load_components_failed"] = ("Gagal mengambil katalog komponen ruleset. Status: {status}", "Failed to fetch ruleset component catalog. Status: {status}");
        // Memperbarui `terms[”rulesets.error.invalid_components_response”]` menggunakan tuple yang membawa bagian 1: ”Respons katalog komponen ruleset
        // tidak valid.”; bagian 2: ”Invalid ruleset component catalog response.” dalam AddRulesets.
        terms["rulesets.error.invalid_components_response"] = ("Respons katalog komponen ruleset tidak valid.", "Invalid ruleset component catalog response.");
        // Memperbarui `terms[”rulesets.error.default_component_not_found”]` menggunakan tuple yang membawa bagian 1: ”Komponen default yang dipilih tidak
        // ditemukan.”; bagian 2: ”The selected default component was not found.” dalam AddRulesets.
        terms["rulesets.error.default_component_not_found"] = ("Komponen default yang dipilih tidak ditemukan.", "The selected default component was not found.");
        // Memperbarui `terms[”status.ruleset.active”]` menggunakan tuple yang membawa bagian 1: ”Aktif”; bagian 2: ”Active” dalam AddRulesets.
        terms["status.ruleset.active"] = ("Aktif", "Active");
        // Memperbarui `terms[”status.ruleset.draft”]` menggunakan tuple yang membawa bagian 1: ”Draf”; bagian 2: ”Draft” dalam AddRulesets.
        terms["status.ruleset.draft"] = ("Draf", "Draft");
        // Memperbarui `terms[”status.ruleset.retired”]` menggunakan tuple yang membawa bagian 1: ”Pensiun”; bagian 2: ”Retired” dalam AddRulesets.
        terms["status.ruleset.retired"] = ("Pensiun", "Retired");
        // Memperbarui `terms[”status.ruleset.unknown”]` menggunakan tuple yang membawa bagian 1: ”Tidak Diketahui”; bagian 2: ”Unknown” dalam AddRulesets.
        terms["status.ruleset.unknown"] = ("Tidak Diketahui", "Unknown");
    // Menutup scope metode AddRulesets; bagian berikut berada di luar batas blok tersebut dalam AddRulesets.
    }
// Menutup scope tipe UiTextLexicon; bagian berikut berada di luar batas blok tersebut.
}
