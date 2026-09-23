// Fungsi file: Menyediakan teks Indonesia dan Inggris untuk statistik, pendaftaran peran, dan mekanik mendatang.
namespace Cashflowpoly.Ui.Infrastructure;

internal static partial class UiTextLexicon
{
    private static partial void AddStatistics(Dictionary<string, (string Id, string En)> terms)
    {
        terms["statistics.subtitle.instructor"] = ("Lihat perkembangan peserta dari sesi yang Anda kelola. Pilih satu pemain dan satu mode untuk membandingkan analitika antarsesi.", "Review participants across the sessions you manage. Choose one player and one mode to compare session analytics.");
        terms["sessions.error.timeline_incomplete"] = ("Riwayat sesi belum berhasil dimuat seluruhnya. Silakan coba lagi.", "The complete session history could not be loaded. Please try again.");
        terms["sessions.error.invalid_analytics"] = ("Data analitika yang sesuai dengan sesi ini belum dapat dimuat. Silakan coba lagi.", "Analytics matching this session could not be loaded. Please try again.");
        terms["statistics.filters"] = ("Pilih data statistik", "Select statistics");
        terms["statistics.player"] = ("Pemain dalam sesi Anda", "Player in your sessions");
        terms["statistics.search_player"] = ("Cari nama pemain...", "Search player name...");
        terms["statistics.choose_player"] = ("Pilih nama pemain dari daftar yang tersedia.", "Choose a player from the available list.");
        terms["statistics.choose_player_title"] = ("Pilih nama pemain terlebih dahulu", "Select a player first");
        terms["statistics.choose_player_hint"] = ("Silakan cari atau pilih nama pemain pada pencarian di atas, lalu tekan Tampilkan untuk melihat grafik perkembangan antarsesi.", "Search or choose a player above, then click Apply to view cross-session statistics.");
        terms["statistics.selected_point"] = ("Rincian titik terpilih", "Selected point details");
        terms["statistics.read_value"] = ("Cara membaca nilai ini", "How to read this value");
        terms["statistics.selected_player"] = ("Pemain yang ditampilkan", "Showing statistics for");
        terms["statistics.mode_hint"] = ("Pemula dan Mahir ditampilkan terpisah karena aturan permainannya berbeda. Setiap grafik hanya memuat satu mode.", "Beginner and Advanced are shown separately because their rules differ. Each chart contains only one mode.");
        terms["statistics.error.participants"] = ("Sebagian daftar peserta belum dapat dimuat. Hanya peserta dan sesi yang berhasil diverifikasi yang ditampilkan. Silakan coba lagi.", "Some participant lists could not be loaded. Only verified participants and sessions are shown. Please try again.");
        terms["sessions.results_pending"] = ("Peringkat dan poin kebahagiaan akhir tersedia setelah sesi selesai dan hasilnya berhasil dimuat.", "Final ranks and happiness points are available once the session ends and results have loaded.");
        terms["statistics.empty_hint.instructor"] = ("Statistik tersedia untuk pemain yang sudah bergabung dalam sesi yang Anda kelola.", "Statistics are available for players who have joined sessions you manage.");
        terms["statistics.title"] = ("Statistik Pemain", "Player Statistics");
        terms["statistics.kicker"] = ("PERKEMBANGAN ANTARSESI", "PROGRESS ACROSS SESSIONS");
        terms["statistics.subtitle"] = ("Telusuri hasil, keputusan, dan perubahan Anda dari seluruh sesi yang diikuti. Setiap grafik bersumber dari Analitika Pemain pada Sesi.", "Explore your results, decisions, and changes across every session you joined. Each chart comes from Player Analytics in Session.");
        terms["statistics.total_sessions"] = ("Sesi pemain pada kedua mode", "Player sessions across both modes");
        terms["statistics.shown_sessions"] = ("Sesi dalam tampilan", "Sessions shown");
        terms["statistics.analysed_sessions"] = ("Sesi dengan analitika", "Sessions with analytics");
        terms["statistics.finished_sessions"] = ("Sesi selesai dalam tampilan", "Finished sessions shown");
        terms["statistics.mode"] = ("Mode permainan", "Game mode");
        terms["statistics.mode.pemula"] = ("Pemula", "Beginner");
        terms["statistics.mode.mahir"] = ("Mahir", "Advanced");
        terms["statistics.status"] = ("Status sesi", "Session status");
        terms["statistics.all_statuses"] = ("Semua status", "All statuses");
        terms["statistics.status.created"] = ("Belum dimulai", "Not started");
        terms["statistics.status.started"] = ("Sedang berlangsung", "In progress");
        terms["statistics.status.ended"] = ("Selesai", "Finished");
        terms["statistics.apply"] = ("Tampilkan", "Apply filters");
        terms["statistics.empty_title"] = ("Belum ada statistik pemain", "No player statistics yet");
        terms["statistics.empty_hint"] = ("Setelah instruktur memasukkan Anda ke sesi, sesi tersebut akan tampil di sini. Grafik tersedia saat data permainan mulai tercatat.", "Once an instructor adds you to a session, it will appear here. Charts become available as gameplay is recorded.");
        terms["statistics.no_match"] = ("Tidak ada sesi yang cocok", "No matching sessions");
        terms["statistics.filter_hint"] = ("Pilih mode atau status lain untuk melihat sesi Anda.", "Choose another mode or status to see your sessions.");
        terms["statistics.reading_title"] = ("Tips membaca statistik pemain", "Tips for reading player statistics");
        terms["statistics.tips.step1"] = ("Pilih mode dan status sesi. Periksa modal, set aturan, dan lama permainan sebelum membandingkan hasil antarsesi.", "Choose the mode and session status. Check starting cash, rulesets, and game duration before comparing session results.");
        terms["statistics.tips.step2"] = ("Baca grafik dari sesi paling awal ke paling baru. Setiap titik mewakili satu sesi. Klik atau ketuk titik untuk membuka rincian; tutup melalui tombol × atau area di luar popup.", "Read charts from the earliest to the latest session. Each point represents one session. Click or tap a point to open its details; close with × or by clicking outside the popup.");
        terms["statistics.tips.step3"] = ("Hasil sesi berjalan dapat berubah. Celah pada grafik berarti data belum tersedia atau metrik tidak berlaku, bukan nol. Grafik ini tidak menyatakan tingkat kemampuan finansial Anda.", "In-progress results can change. A gap means data is unavailable or a metric does not apply, not zero. These charts do not rate your financial ability.");
        terms["statistics.sections"] = ("Bagian statistik", "Statistics sections");
        terms["statistics.group.overview"] = ("Hasil permainan", "Game results");
        terms["statistics.group.money"] = ("Koin dan usaha", "Coins and business");
        terms["statistics.group.play"] = ("Aksi dan pesanan", "Actions and orders");
        terms["statistics.group.future"] = ("Perencanaan dan risiko", "Planning and risk");
        terms["statistics.group.happiness"] = ("Kebutuhan dan kebahagiaan", "Needs and happiness");
        terms["statistics.group_hint.overview"] = ("Lihat poin kebahagiaan dan koin tersisa pada setiap sesi.", "Review happiness points and remaining coins in each session.");
        terms["statistics.group_hint.money"] = ("Baca perubahan kas, pemerataan pemasukan, pengeluaran bahan, dan keuntungan pesanan secara terpisah.", "Read cash changes, income distribution, ingredient spending, and order profits separately.");
        terms["statistics.group_hint.play"] = ("Lihat bagaimana aksi dipakai untuk mendapatkan koin dan menyelesaikan pesanan.", "See how actions earn coins and complete orders.");
        terms["statistics.group_hint.future"] = ("Khusus mode Mahir: risiko, pinjaman, target yang terbeli, dan porsi aksi untuk masa depan.", "Advanced mode: risks, loans, purchased goals, and actions used for future needs.");
        terms["statistics.group_hint.happiness"] = ("Telusuri pemerataan kebutuhan, komitmen donasi, dan sumber poin kebahagiaan.", "Explore needs distribution, donation commitment, and happiness point sources.");
        terms["statistics.table_pages"] = ("Halaman tabel", "Table pages");
        terms["statistics.page_status"] = ("Baris {start}–{end} dari {total}", "Rows {start}–{end} of {total}");
        terms["statistics.page_back"] = ("Kembali", "Back");
        terms["statistics.values_and_source"] = ("Lihat nilai tiap sesi, arti, dan sumber data", "View session values, meanings, and data source");
        terms["statistics.values_per_session"] = ("Nilai tiap sesi", "Values by session");
        terms["statistics.session"] = ("Sesi", "Session");
        terms["statistics.meaning"] = ("Arti", "Meaning");
        terms["statistics.source"] = ("Sumber: Analitika Pemain pada Sesi yang sama. Nilai dan arti mengikuti perhitungan analitika sesi tersebut.", "Source: Player Analytics in the same session. Values and meanings follow that session's analytics calculations.");
        terms["statistics.session_details"] = ("Rincian seluruh sesi", "All session details");
        terms["statistics.session_hint"] = ("Nomor sesi sesuai urutan pada grafik. Buka analitika untuk memeriksa aturan, transaksi, angka pembentuk, dan cara menghitungnya.", "Session numbers match the chart order. Open analytics to inspect rules, transactions, input values, and calculations.");
        terms["statistics.updated"] = ("Analitika diperbarui", "Analytics updated");
        terms["statistics.not_available"] = ("Analitika belum tersedia untuk sesi ini.", "Analytics are not available for this session yet.");
        terms["statistics.open_analytics"] = ("Buka Analitika Pemain pada Sesi", "Open Player Analytics in Session");
        terms["statistics.open_session"] = ("Buka sesi", "Open session");
        terms["statistics.no_numeric"] = ("Belum ada nilai yang dapat digambarkan untuk pilihan sesi ini.", "No chartable values are available for these sessions yet.");
        terms["statistics.tap_hint"] = ("Klik atau ketuk titik untuk membuka rincian. Tutup melalui tombol × atau area di luar popup.", "Click or tap a point to open its details. Close with × or by clicking outside the popup.");
        terms["statistics.scroll_hint"] = ("Geser ke samping untuk melihat sesi lainnya. Skala nilai tetap sama.", "Scroll sideways to see more sessions. The value scale stays the same.");
        terms["statistics.error.load"] = ("Statistik belum dapat dimuat. Silakan coba lagi.", "Statistics could not be loaded. Please try again.");
        terms["statistics.error.partial"] = ("Sebagian analitika sesi belum dapat dimuat. Sesi tetap ditampilkan, sedangkan nilai yang belum tersedia dibiarkan kosong. Silakan coba lagi.", "Some session analytics could not be loaded. Sessions remain listed with unavailable values left blank. Please try again.");
        terms["auth.register_role"] = ("Daftar sebagai", "Register as");
        terms["auth.register_role_hint"] = ("Pemain mengikuti sesi dan membaca statistik pribadi. Instruktur membuat sesi, mengelola set aturan, dan membimbing pemain.", "Players join sessions and read their own statistics. Instructors create sessions, manage rulesets, and guide players.");
        terms["auth.error.invalid_role"] = ("Pilih peran Pemain atau Instruktur.", "Choose Player or Instructor.");
        terms["rulesets.coming_soon"] = ("Belum dapat diubah", "Not editable yet");
        terms["rulesets.mechanics_held_hint"] = ("Pilihan yang dikunci tetap aktif dan belum dapat diubah. Fitur Mahir mengikuti mode permainan. Angka dan batas pada kolom yang tersedia dapat disesuaikan.", "Locked options remain enabled and cannot be changed yet. Advanced features follow the game mode. Amounts and limits in available fields can be adjusted.");
    }
}
